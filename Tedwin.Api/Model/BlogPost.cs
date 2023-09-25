using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Tedwin.Api.Model;

public class BlogPost
{
    [Key]
    public Guid Id { get; set; }
    [Required,MaxLength(255)]
    public string Title { get; set; }
    [Required, MaxLength(5000)]
    public string Content { get; set; }
    //public ICollection<Tag> Tags { get; set; }
    public DateTime CreatedAt { get; set; }
    public ICollection<BlogPostTags> BlogPostTags { get; set; }
    //public IdentityUser User { get; set; }
    public string UserId { get; set; }
}
