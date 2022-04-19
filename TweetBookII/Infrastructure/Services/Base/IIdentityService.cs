
using TweetBookII.Domain;

namespace TweetBookII.Infrastructure.Services.Base;
public interface IIdentityService
{
    Task<AuthorizationResult> RegisterUserAsync(string email, string password);
}
