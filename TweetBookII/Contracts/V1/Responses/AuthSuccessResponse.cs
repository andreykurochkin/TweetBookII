namespace TweetBookII.Contracts.V1.Responses;
public class AuthSuccessResponse
{
    public string Token { get; set; } = null!;
    public string RefreshToken { get; set; } = null!;
}
