
using TweetBookII.Domain;

namespace TweetBookII.Infrastructure.Services.Base;
public interface IIdentityService
{
    Task<AuthenticationResult> RegisterUserAsync(string userNamne, string password);

    Task<AuthenticationResult> LoginAsync(string userName, string password);
    Task<AuthenticationResult> RefreshTokenAsync(string? token, string? refreshToken);
}
