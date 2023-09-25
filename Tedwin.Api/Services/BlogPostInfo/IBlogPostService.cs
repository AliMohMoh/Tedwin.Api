using Microsoft.AspNetCore.Identity;
using Tedwin.Api.Model;
using Tedwin.Api.Model.Dto;

namespace Tedwin.Api.Services.BlogPostInfo;

public interface IBlogPostService
{
    Task<List<BlogPost>> GetAllBlogPostsAsync();
    Task<BlogPost> GetBlogPostByIdAsync(Guid id);
    Task CreateBlogPostAsync(BlogPost blogPost);
    Task UpdateBlogPostAsync(BlogPost blogPost);
    Task DeleteBlogPostAsync(Guid id);
    Task<bool> CanUserEditBlogPostAsync(string userId, Guid blogPostId);
    Task<Response<List<BlogPost>>> GetPaginatedBlogPostsAsync(int pageIndex, int pageSize);
}
