using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;

namespace TweetBookII.Domain;

public class Post
{
    public Guid Id { get; set; }
    public string? Name { get; set; }

    public string? UserId { get; set; }

    [ForeignKey(nameof(UserId))]
    public IdentityUser? User { get; set; }
    public string? Content { get; set; }
}
