using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using TweetBookII.Infrastructure.Installers.Base;
using TweetBookII.Infrastructure.Options;
using TweetBookII.Infrastructure.Services;
using TweetBookII.Infrastructure.Services.Base;

namespace TweetBookII.Infrastructure.Installers;

public class MvcInstaller : IInstaller
{
    public void InstallServices(IServiceCollection services, IConfiguration configuration)
    {
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



        //services.AddRouting();
        services.AddControllers(/*options => { options.EnableEndpointRouting = true; }*/);

        services.AddSingleton(jwtOptions);
        services.AddScoped<IIdentityService, IdentityService>();

    }
}
