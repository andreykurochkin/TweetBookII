using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using TweetBookII.Contracts.V1;
using TweetBookII.Contracts.V1.Requests;
using TweetBookII.Domain;
using Xunit;

namespace TweetBookII.IntegrationTests;
public class PostsControllerTests : IntegrationTest
{
    const string ExpectedPostName = "Test post";

    [Fact]
    public async Task GetAll_ReturnsEmptyResponse_WhenThereIsNoPostInTheDatabase()
    {
        await AuthenticateAsync();

        var response = await TestClient.GetAsync(ApiRoutes.Posts.GetAll);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        (await response.Content.ReadAsAsync<List<Post>>()).Should().BeEmpty();
    }

    [Fact]
    public async Task Get_ReturnsPost_WhenPostExistsInTheDatabase()
    {
        await AuthenticateAsync();
        var createPostResponse = await CreatePostAsync(new CreatePostRequest
        {
            Name = ExpectedPostName
        });

        var response = await TestClient.GetAsync(ApiRoutes.Posts.Get.Replace("{postId}", createPostResponse.Id.ToString()));

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var returnedPost = await response.Content.ReadAsAsync<Post>();
        returnedPost.Id.Should().Be(createPostResponse.Id);
        returnedPost.Name.Should().Be(ExpectedPostName);
    }
}
