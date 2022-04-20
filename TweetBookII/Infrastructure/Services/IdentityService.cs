
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using TweetBookII.Domain;
using TweetBookII.Infrastructure.Options;
using TweetBookII.Infrastructure.Services.Base;

namespace TweetBookII.Infrastructure.Services;
public class IdentityService : IIdentityService
{
    private readonly UserManager<IdentityUser> _userManager;
    private readonly JwtOptions _jwtOptions;

    public IdentityService(UserManager<IdentityUser> userManager, JwtOptions jwtOptions)
    {
        _userManager = userManager;
        _jwtOptions = jwtOptions;
    }

    public async Task<AuthorizationResult> RegisterUserAsync(string email, string password)
    {
        var userExists = await _userManager.FindByEmailAsync(email) is null;
        if (!userExists)
        {
            return new AuthorizationResult
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
            return new AuthorizationResult
            {
                Errors = createUserResult.Errors.Select(_ => _.Description)
            };
        }

        return GenerateAuthenticationResult(persistentUser);
    }

    private AuthorizationResult GenerateAuthenticationResult(IdentityUser identityUser)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(_jwtOptions.Secret);
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[]
            {
                new Claim(JwtRegisteredClaimNames.Jti, identityUser.Email),
                new Claim(JwtRegisteredClaimNames.Sub, identityUser.Email),
                new Claim(JwtRegisteredClaimNames.Email, identityUser.Email),
                new Claim("id", identityUser.Id),
            }),
            Expires = DateTime.UtcNow.AddHours(2),
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };
        var token = tokenHandler.CreateToken(tokenDescriptor);
        return new AuthorizationResult
        {
            Succeded = true,
            Token = tokenHandler.WriteToken(token)
        };
    }
}
