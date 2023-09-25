using Microsoft.AspNetCore.Identity;
using Tedwin.Api.Model;

namespace Tedwin.Api.Services.BlogPostInfo;

public interface IBlogPostService
{
    Task<List<BlogPost>> GetAllBlogPostsAsync();
    Task<BlogPost> GetBlogPostByIdAsync(Guid id);
    Task CreateBlogPostAsync(BlogPost blogPost);
    Task UpdateBlogPostAsync(BlogPost blogPost);
    Task DeleteBlogPostAsync(Guid id);
    Task<bool> CanUserEditBlogPostAsync(string userId, Guid blogPostId);
}
