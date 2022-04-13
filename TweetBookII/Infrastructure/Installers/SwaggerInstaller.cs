using TweetBookII.Infrastructure.Installers.Base;

namespace TweetBookII.Infrastructure.Installers;

public class SwaggerInstaller : IInstaller
{
    public void InstallServices(IServiceCollection services, IConfiguration configuration)
    {
        services.AddSwaggerGen(_ =>
        {
            _.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
            {
                Title = "Tweetbook",
                Version = "v1"
            });
        });
    }
}
