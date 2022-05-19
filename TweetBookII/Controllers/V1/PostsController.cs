using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TweetBookII.Contracts.V1;
using TweetBookII.Contracts.V1.Requests;
using TweetBookII.Contracts.V1.Responses;
using TweetBookII.Domain;
using TweetBookII.Infrastructure.Extensions;
using TweetBookII.Infrastructure.Services.Base;

namespace TweetBookII.Controllers.V1;

[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
public class PostsController : Controller
{
    private readonly IPostsService _postsService;

    public PostsController(IPostsService postsService)
    {
        _postsService = postsService;
    }

    [HttpGet(ApiRoutes.Posts.GetAll)]
    public async Task<IActionResult> GetAll()
    {
        var posts = await _postsService.GetPostsAsync();
        return Ok(posts);
    }

    [HttpPost(ApiRoutes.Posts.Create)]
    public async Task<IActionResult> Create([FromBody] CreatePostRequest request)
    {
        var post = new Post
        {
            Name = request.Name,
            UserId = HttpContext.GetUserId()
        };
        await _postsService.CreatePostAsync(post);

        var baseUri = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host.ToUriComponent()}";
        var responseUri = $"{baseUri}/{ApiRoutes.Posts.Get.Replace("{id}", post.Id.ToString())}";

        var response = new PostResponse
        {
            Id = post.Id
        };

        return Created(responseUri, response);
    }

    [HttpGet(ApiRoutes.Posts.Get)]
    public async Task<IActionResult> Get([FromRoute] Guid postId)
    {
        var post = await _postsService.GetPostByIdAsync(postId);
        if (post is null)
        {
            return NotFound();
        }
        return Ok(post);
    }

    [HttpPut(ApiRoutes.Posts.Update)]
    public async Task<IActionResult> Update([FromRoute] Guid postId, [FromBody] UpdatePostRequest updatePostRequest)
    {
        var post = await _postsService.GetPostByIdAsync(postId);
        if (post is null)
        {
            return NotFound(new PostResponse 
            { 
                Id = postId 
            });
        }
        var userOwnsPost = await _postsService.UserOwnsPost(HttpContext.GetUserId(), postId);
        if (!userOwnsPost)
        {
            return BadRequest(new
            {
                error = "User doesn`t own post"
            });
        }
        post.Name = updatePostRequest.Name;

        var updated = await _postsService.UpdatePostAsync(post);
        if (updated)
        {
            return Ok(post);
        }
        return NotFound(post);
    }

    [HttpDelete(ApiRoutes.Posts.Delete)]
    public async Task<IActionResult> Delete([FromRoute] Guid postId)
    {
        var userOwnsPost = await _postsService.UserOwnsPost(HttpContext.GetUserId(), postId);
        if (!userOwnsPost)
        {
            return BadRequest(new
            {
                error = "User doesn`t own post"
            });
        };
        var deleted = await _postsService.DeletePostAsync(postId);
        if (deleted)
        {
            return NoContent();
        }
        return NotFound();
    }
}
