using System.ComponentModel.DataAnnotations;

namespace TweetBookII.Contracts.V1.Requests;
public class UserLoginRequest
{
    [EmailAddress]
    public string Email { get; set; } = null!;
    public string Password { get; set; } = null!;
}
