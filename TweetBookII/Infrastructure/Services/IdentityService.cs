using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using TweetBookII.Data;
using TweetBookII.Domain;
using TweetBookII.Infrastructure.Options;
using TweetBookII.Infrastructure.Services.Base;

namespace TweetBookII.Infrastructure.Services;
public class IdentityService : IIdentityService
{
    private readonly UserManager<IdentityUser> _userManager;
    private readonly JwtOptions _jwtOptions;
    private readonly TokenValidationParameters _tokenValidationParameters;
    private readonly DataContext _dataContext;
    public IdentityService(UserManager<IdentityUser> userManager, JwtOptions jwtOptions, TokenValidationParameters tokenValidationParameters, DataContext dataContext)
    {
        _userManager = userManager;
        _jwtOptions = jwtOptions;
        _tokenValidationParameters = tokenValidationParameters;
        _dataContext = dataContext;
    }

    public async Task<Domain.AuthenticationResult> RegisterUserAsync(string email, string password)
    {
        var user = await _userManager.FindByEmailAsync(email);
        if (user is not null)
        {
            return new Domain.AuthenticationResult
            {
                Errors = new string[]
                {
                    "User with specified email already exists."
                }
            };
        }

        var persistentUser = new IdentityUser
        {
            Email = email,
            NormalizedEmail = email.ToUpper(),
            UserName = email,
            NormalizedUserName = email.ToUpper(),
        };
        var createUserResult = await _userManager.CreateAsync(persistentUser, password);
        if (!createUserResult.Succeeded)
        {
            return new Domain.AuthenticationResult
            {
                Errors = createUserResult.Errors.Select<IdentityError, string>(_ => _.Description)
            };
        }

        return await GenerateAuthenticationResultForUserAsync(persistentUser);
    }

    public async Task<Domain.AuthenticationResult> LoginAsync(string email, string password)
    {
        var user = await _userManager.FindByEmailAsync(email);
        if (user is null)
        {
            return new Domain.AuthenticationResult
            {
                Errors = new[] { "User does not exists" }
            };
        }

        var passwordIsValid = await _userManager.CheckPasswordAsync(user, password);
        if (!passwordIsValid)
        {
            return new Domain.AuthenticationResult
            {
                Errors = new string[] {
                "Bad login/password pair"}
            };
        }

        return await GenerateAuthenticationResultForUserAsync(user);
    }

    private async Task<Domain.AuthenticationResult> GenerateAuthenticationResultForUserAsync(IdentityUser identityUser)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(_jwtOptions.Secret);
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, identityUser.Email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Email, identityUser.Email),
                new Claim("id", identityUser.Id),
            }),
            Expires = DateTime.UtcNow.Add(_jwtOptions.TokenLIfetime),
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };
        var token = tokenHandler.CreateToken(tokenDescriptor);

        var refreshToken = new RefreshToken
        {
            //Token = Guid.NewGuid().ToString(),
            JwtId = token.Id,
            UserId = identityUser.Id,
            CreationDate = DateTime.UtcNow,
            ExpiryDate = DateTime.UtcNow.AddMonths(6),
        };
        await _dataContext.RefreshTokens.AddAsync(refreshToken);
        await _dataContext.SaveChangesAsync();

        return new Domain.AuthenticationResult
        {
            Succeded = true,
            Token = tokenHandler.WriteToken(token),
            RefreshToken = refreshToken.Token
        };
    }

    public async Task<Domain.AuthenticationResult> RefreshTokenAsync(string? token, string? refreshToken)
    {
        var validatedToken = GetPrincipalFromToken(token);
        if (validatedToken == null)
        {
            return new Domain.AuthenticationResult()
            {
                Errors = new[]
                {
                    "Invalid token"
                }
            };
        }
        var jwtClaim = validatedToken.Claims.Single(_ => _.Type == JwtRegisteredClaimNames.Exp).Value;
        var expiryDateUnix = long.Parse(jwtClaim);
        var expiryDateTimeUtc = new DateTime(year: 1970, month: 1, day: 1, 0, 0, 0, kind: DateTimeKind.Utc)
            .AddSeconds(expiryDateUnix); 
        if (expiryDateTimeUtc > DateTime.UtcNow)
        {
            return new Domain.AuthenticationResult()
            {
                Errors = new[]
                {
                    "This token hasn`t expired yet"
                }
            };
        }
        var jtiClaim = validatedToken.Claims.Single(_ => _.Type == JwtRegisteredClaimNames.Jti).Value;
        var storedRefreshToken = await _dataContext.RefreshTokens.SingleOrDefaultAsync(_ => _.JwtId == refreshToken);
        if (storedRefreshToken is null)
        {
            return new Domain.AuthenticationResult()
            {
                Errors = new[]
                {
                    "This refresh token doesn`t exist"
                }
            };
        }
        if (DateTime.UtcNow > storedRefreshToken.ExpiryDate)
        {
            return new Domain.AuthenticationResult()
            {
                Errors = new[]
                {
                    "This refresh token has expired"
                }
            };
        }
        if (storedRefreshToken.Invalidated)
        {
            return new Domain.AuthenticationResult()
            {
                Errors = new[]
                {
                    "This refresh token has been invalidated"
                }
            };
        }
        if (storedRefreshToken.Used)
        {
            return new Domain.AuthenticationResult()
            {
                Errors = new[]
                {
                    "This refresh token has been used"
                }
            };
        }
        if (storedRefreshToken.JwtId != jtiClaim)
        {
            return new Domain.AuthenticationResult()
            {
                Errors = new[]
                {
                    "This refresh token doesn`t match this jwt"
                }
            };
        }

        storedRefreshToken.Used = true;
        _dataContext.RefreshTokens.Update(storedRefreshToken);
        await _dataContext.SaveChangesAsync();
        
        var idClaim = validatedToken.Claims.Single(_ => _.Type == "id");
        var user = await _userManager.FindByEmailAsync(idClaim.Value);
        return await GenerateAuthenticationResultForUserAsync(user);
    }

    private ClaimsPrincipal? GetPrincipalFromToken(string? token)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        try
        {
            var principal = tokenHandler.ValidateToken(token, _tokenValidationParameters, out var validatedToken);
            if (!IsJwtWithValidSecurityAlgorithm(validatedToken))
            {
                return null;
            }
            return principal;
        }
        catch
        {
            return null;
        }
    }

    private bool IsJwtWithValidSecurityAlgorithm(SecurityToken validatedToken)
    {
        return (validatedToken is JwtSecurityToken jwtSecurityToken) &&
            jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase);
    }
}
