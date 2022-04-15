﻿using Microsoft.AspNetCore.Mvc;
using TweetBookII.Contracts.V1;
using TweetBookII.Contracts.V1.Requests;
using TweetBookII.Contracts.V1.Responses;
using TweetBookII.Domain;
using TweetBookII.Infrastructure.Services.Base;

namespace TweetBookII.Controllers.V1;

public class PostsController : Controller
{
    private readonly IPostsService _postsService;

    public PostsController(IPostsService postsService)
    {
        _postsService = postsService;
    }

    [HttpGet(ApiRoutes.Posts.GetAll)]
    public IActionResult GetAll()
    {
        return Ok(_postsService.GetAll());
    }

    [HttpPost(ApiRoutes.Posts.Create)]
    public IActionResult Create([FromBody] CreatePostRequest request)
    {
        var post = new Post
        {
            Id = request.Id
        };

        if (post.Id == Guid.Empty)
        {
            post.Id = Guid.NewGuid();
        }

        _postsService.GetAll().Add(post);

        var baseUri = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host.ToUriComponent()}";
        var responseUri = $"{baseUri}/{ApiRoutes.Posts.Get.Replace("{id}", post.Id.ToString())}";

        var response = new PostResponse
        {
            Id = post.Id
        };

        return Created(responseUri, response);
    }

    [HttpGet(ApiRoutes.Posts.Get)]
    public IActionResult Get([FromRoute] Guid postId)
    {
        var post = _postsService.GetAll().FirstOrDefault(_ => _.Id == postId);
        if (post is null)
        {
            return NotFound();
        }
        return Ok(post);
    }

    [HttpPut(ApiRoutes.Posts.Update)]
    public IActionResult Update([FromRoute] Guid postId, [FromBody] UpdatePostRequest updatePostRequest)
    {
        var post = new Post
        {
            Id = postId,
            Name = updatePostRequest.Name
        };

        var updated = _postsService.Update(post);
        if (updated)
        {
            return Ok(post);
        }
        return NotFound(post);
    }

    [HttpDelete(ApiRoutes.Posts.Delete)]
    public IActionResult Delete([FromRoute] Guid postId)
    {
        var deleted = _postsService.Delete(postId);
        if (deleted)
        {
            return NoContent();
        }
        return NotFound();
    }
}
