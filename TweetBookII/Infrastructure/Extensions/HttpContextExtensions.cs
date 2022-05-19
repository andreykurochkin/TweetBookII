namespace TweetBookII.Infrastructure.Extensions;
public static class HttpContextExtensions
{
    public static string GetUserId(this HttpContext httpContext)
    {
        return httpContext.User?.Claims.FirstOrDefault(_ => _.Type == "id")?.Value ?? string.Empty;
    }
}
