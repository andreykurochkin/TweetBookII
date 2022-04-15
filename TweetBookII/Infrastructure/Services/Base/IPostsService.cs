using TweetBookII.Domain;

namespace TweetBookII.Infrastructure.Services.Base;

public interface IPostsService
{
    Task<bool> CreatePostAsync(Post post);

    Task<List<Post>> GetPostsAsync();

    Task<Post?> GetPostByIdAsync(Guid postId);

    Task<bool> UpdatePostAsync(Post postToUpdate);

    Task<bool> DeletePostAsync(Guid postId);
}