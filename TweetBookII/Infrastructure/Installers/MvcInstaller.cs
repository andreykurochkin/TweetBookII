using TweetBookII.Infrastructure.Installers.Base;

namespace TweetBookII.Infrastructure.Installers;

public class MvcInstaller : IInstaller
{
    public void InstallServices(IServiceCollection services, IConfiguration configuration)
    {
        services.AddControllersWithViews();
    }
}
