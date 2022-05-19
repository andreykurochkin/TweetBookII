using Microsoft.AspNetCore.Mvc.Testing;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using TweetBookII.Data;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using System.Net.Http.Json;
using TweetBookII.Contracts.V1;
using TweetBookII.Contracts.V1.Requests;
using TweetBookII.Contracts.V1.Responses;
using TweetBookII.Domain;

namespace TweetBookII.IntegrationTests;
public class IntegrationTest
{
    const string Bearer = "Bearer";
    const string DbName = "TestDb";
    protected readonly HttpClient TestClient;
    public IntegrationTest()
    {
        var appFactory = new WebApplicationFactory<Program>()
            .WithWebHostBuilder(builder =>
            {
                builder.ConfigureServices(services =>
                {
                    services.RemoveAll(typeof(DbContextOptions<DataContext>));
                    services.AddDbContext<DataContext>(options =>
                    {
                        options.UseInMemoryDatabase(DbName);
                    });
                });
            });
        
        TestClient = appFactory.CreateClient();
    }

    protected async Task AuthenticateAsync()
    {
        TestClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(scheme: Bearer, parameter: await GetJwtAsync());
    }

    protected async Task<PostResponse> CreatePostAsync(CreatePostRequest createPostRequest)
    {
        var response = await TestClient.PostAsJsonAsync(requestUri: ApiRoutes.Posts.Create, createPostRequest);
        return await response.Content.ReadAsAsync<PostResponse>();
    }

    private async Task<string?> GetJwtAsync()
    {
        var response = await TestClient.PostAsJsonAsync(requestUri: ApiRoutes.Identity.Registration, new UserRegistrationRequest
        {
            Email = "test@integration.com",
            Password = "VbnWsd4$123"
        });

        var registrationResponse = await response.Content.ReadAsAsync<AuthSuccessResponse>();
        return registrationResponse.Token;
    }
}
