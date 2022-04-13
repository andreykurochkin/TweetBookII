namespace TweetBookII.Infrastructure.Installers.Base;

public static class InstallerExtension
{
    public static void InstallServicesInAssembly(this IServiceCollection services, IConfiguration configuration)
    {
        var installers = typeof(Program).Assembly.ExportedTypes
            .Where(type => type.IsAssignableTo(typeof(IInstaller)) && !type.IsInterface && !type.IsAbstract)
            .Select(Activator.CreateInstance)
            .Cast<IInstaller>()
            .ToList();
        installers.ForEach(installer => installer.InstallServices(services, configuration));
    }
}
