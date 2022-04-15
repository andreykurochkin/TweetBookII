using TweetBookII.Domain;

namespace TweetBookII.Infrastructure.Services.Base;

public interface IPostsService
{
    List<Post> GetAll();
    
    Post? GetById(Guid postId);

    bool Update(Post postToUpdate);
}
