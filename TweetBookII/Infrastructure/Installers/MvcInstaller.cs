using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using TweetBookII.Infrastructure.Installers.Base;
using TweetBookII.Infrastructure.Options;

namespace TweetBookII.Infrastructure.Installers;

public class MvcInstaller : IInstaller
{
    public void InstallServices(IServiceCollection services, IConfiguration configuration)
    {
        services.AddControllersWithViews();

        var jwtOptions = new JwtOptions
        {
            Secret = configuration.GetSection("JwtSettings").GetSection("Secret").Value!
        };
        

        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
           options.SaveToken = true;
           options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
           {
               ValidateIssuerSigningKey = true,
               IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(jwtOptions.Secret)),
               ValidateIssuer = false,
               ValidateAudience = false,
               RequireExpirationTime = false,
               ValidateLifetime = false
           };
        });

        services.AddSingleton(jwtOptions);

    }
}
