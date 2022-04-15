using TweetBookII.Domain;

namespace TweetBookII.Infrastructure.Services.Base;

public interface IPostsService
{
    List<Post> GetAll();
    
    Post? GetPostById(Guid postId);

    bool Update(Post postToUpdate);
    bool Delete(Guid postId);
}
