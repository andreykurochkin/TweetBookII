using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using TweetBookII.Data;
using TweetBookII.Infrastructure.Installers.Base;
using TweetBookII.Infrastructure.Services;
using TweetBookII.Infrastructure.Services.Base;

namespace TweetBookII.Infrastructure.Installers
{
    public class DbInstaller : IInstaller
    {
        public void InstallServices(IServiceCollection services, IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("DefaultCoinnection");
            services.AddDbContext<DataContext>(_ => _.UseSqlServer(connectionString!));
            services.AddIdentityCore<IdentityUser>().AddEntityFrameworkStores<DataContext>();

            services.AddSingleton<IPostsService, PostsService>();
        }
    }
}
