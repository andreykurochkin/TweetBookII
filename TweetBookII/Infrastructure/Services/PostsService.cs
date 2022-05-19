using Microsoft.EntityFrameworkCore;
using TweetBookII.Data;
using TweetBookII.Domain;
using TweetBookII.Infrastructure.Services.Base;

namespace TweetBookII.Infrastructure.Services;

public class PostsService : IPostsService
{
    private readonly DataContext _dataContext;

    public PostsService(DataContext dataContext)
    {
        _dataContext = dataContext;
    }

    public Task<List<Post>> GetPostsAsync()
    {
        return _dataContext.Posts.ToListAsync();
    }

    public Task<Post?> GetPostByIdAsync(Guid postId) => _dataContext.Posts.AsNoTracking().FirstOrDefaultAsync(post => post.Id == postId);

    public async Task<bool> UpdatePostAsync(Post postToUpdate)
    {
        _dataContext.Posts.Update(postToUpdate);
        var saveResult = await _dataContext.SaveChangesAsync();
        return saveResult > 0;
    }

    public async Task<bool> CreatePostAsync(Post post)
    {
        await _dataContext.Posts.AddAsync(post);
        var created = await _dataContext.SaveChangesAsync();
        return created > 0;
    }

    public async Task<bool> DeletePostAsync(Guid postId)
    {
        var post = await GetPostByIdAsync(postId);
        if (post is null)
        {
            return false;
        }
        _dataContext.Posts.Remove(post);
        var deleteResult = await _dataContext.SaveChangesAsync();
        return deleteResult > 0;
    }

    public async Task<bool> UserOwnsPost(string userId, Guid postId)
    {
        var post = await GetPostByIdAsync(postId);
        var postOwnerId = post?.UserId ?? String.Empty;
        return userId == postOwnerId;    
    }
}
