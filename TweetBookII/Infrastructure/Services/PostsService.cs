﻿using TweetBookII.Domain;
using TweetBookII.Infrastructure.Services.Base;

namespace TweetBookII.Infrastructure.Services;

public class PostsService : IPostsService
{
    private readonly List<Post> _posts = new();

    public PostsService()
    {
        Enumerable.Range(0, 5)
            .Select(_ => new { Index = _ })
            .ToList()
            .ForEach(_ => _posts.Add(new Post
            {
                Id = Guid.NewGuid(),
                Name = _.Index.ToString()
            }));
    }
    public List<Post> GetAll() => _posts;

    public Post? GetById(Guid postId) => _posts.FirstOrDefault(post => post.Id == postId);

    public bool Update(Post postToUpdate)
    {
        var exists = _posts.Any(_ => _.Id == postToUpdate.Id);
        if (!exists)
        {
            return false;
        }
        var index = _posts.FindIndex(_ => _.Id == postToUpdate.Id);
        _posts[index] = postToUpdate;
        return true;
    }
}