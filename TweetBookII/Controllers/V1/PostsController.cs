using Microsoft.AspNetCore.Mvc;
using TweetBookII.Contracts.V1;
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
    public IActionResult Create([FromBody] Post post)
    {
        if (string.IsNullOrEmpty(post.Id))
        {
            post.Id = Guid.NewGuid().ToString();
        }

        var baseUri = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host.ToUriComponent()}";
        var responseUri = $"{baseUri}/{ApiRoutes.Posts.Get.Replace("{id}", post.Id)}";

        return Created(responseUri, post);
    }
}
