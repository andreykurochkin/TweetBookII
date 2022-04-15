using TweetBookII.Domain;
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

    public Post? GetPostById(Guid postId) => _posts.FirstOrDefault(post => post.Id == postId);

    public bool Update(Post postToUpdate)
    {
        var exists = GetPostById(postToUpdate.Id) is null;
        if (!exists)
        {
            return false;
        }
        var index = _posts.FindIndex(_ => _.Id == postToUpdate.Id);
        _posts[index] = postToUpdate;
        return true;
    }

    public bool Delete(Guid postId)
    {
        var post = GetPostById(postId);
        if (post is null)
        {
            return false;
        }
        _posts.Remove(post);
        return true;
    }
}
