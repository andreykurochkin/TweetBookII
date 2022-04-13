using Microsoft.EntityFrameworkCore;
using TweetBookII.Data;
using Microsoft.AspNetCore.Identity;
using TweetBookII.Infrastructure.Installers.Base;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.InstallServicesInAssembly(builder.Configuration);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

var swaggerOptions = new TweetBookII.Infrastructure.SwaggerOptions();
app.Configuration.GetSection(nameof(TweetBookII.Infrastructure.SwaggerOptions)).Bind(swaggerOptions);
app.UseSwagger(_ => _.RouteTemplate = swaggerOptions.JsonRoute);
app.UseSwaggerUI(_ => _.SwaggerEndpoint(swaggerOptions.UIEndPoint, swaggerOptions.Description));

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
