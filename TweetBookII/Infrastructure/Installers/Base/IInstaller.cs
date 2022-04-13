namespace TweetBookII.Infrastructure.Installers.Base;

public interface IInstaller
{
    void InstallServices(IServiceCollection services, IConfiguration configuration);
}
