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
}
