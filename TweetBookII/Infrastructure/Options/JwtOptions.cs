namespace TweetBookII.Infrastructure.Options;

public class JwtOptions
{
    public string Secret { get; set; } = null!;
    public TimeSpan TokenLIfetime { get; set; }
}
