using Microsoft.AspNetCore.Mvc;
using TweetBookII.Contracts.V1;
using TweetBookII.Contracts.V1.Requests;
using TweetBookII.Contracts.V1.Responses;
using TweetBookII.Domain;

namespace TweetBookII.Controllers.V1;

public class PostsController : Controller
{
    private readonly List<Post> _posts = new();

    public PostsController()
    {
        Enumerable.Range(0, 5)
            .ToList()
            .ForEach(_ => _posts.Add(new Post
            {
                Id = Guid.NewGuid().ToString()
            }));
    }

    [HttpGet(ApiRoutes.Posts.GetAll)]
    public IActionResult GetAll()
    {
        return Ok(_posts);
    }

    [HttpPost(ApiRoutes.Posts.Create)]
    public IActionResult Create([FromBody] CreatePostRequest request)
    {
        var post = new Post
        {
            Id = request.Id
        };

        if (string.IsNullOrEmpty(post.Id))
        {
            post.Id = Guid.NewGuid().ToString();
        }

        _posts.Add(post);

        var baseUri = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host.ToUriComponent()}";
        var responseUri = $"{baseUri}/{ApiRoutes.Posts.Get.Replace("{id}", post.Id)}";

        var response = new PostResponse
        {
            Id = post.Id
        };

        return Created(responseUri, response);
    }
}
