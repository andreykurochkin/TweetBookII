
using TweetBookII.Domain;

namespace TweetBookII.Infrastructure.Services.Base;
public interface IIdentityService
{
    Task<AuthorizationResult> RegisterUserAsync(string userNamne, string password);

    Task<AuthorizationResult> LoginAsync(string userName, string password);
}
