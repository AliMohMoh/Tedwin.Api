using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Tedwin.Api.Data;
using Tedwin.Api.Model;
using Tedwin.Api.Model.Dto;

namespace Tedwin.Api.Services.BlogPostInfo;

public class BlogPostService : IBlogPostService
{
    private readonly ApplicationDbContext _context;

    public BlogPostService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<BlogPost>> GetAllBlogPostsAsync()
    {
        return await _context.BlogPosts.Include(bp => bp.BlogPostTags).ToListAsync();
    }

    public async Task<BlogPost> GetBlogPostByIdAsync(Guid id)
    {
        return await _context.BlogPosts.Include(bp => bp.BlogPostTags).FirstOrDefaultAsync(bp => bp.Id == id);
    }

    public async Task CreateBlogPostAsync(BlogPost blogPost)
    {
        blogPost.Id= Guid.NewGuid();
        blogPost.CreatedAt = DateTime.UtcNow;
        _context.BlogPosts.Add(blogPost);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateBlogPostAsync(BlogPost blogPost)
    {
        _context.BlogPosts.Update(blogPost);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteBlogPostAsync(Guid id)
    {
        var blogPost = await _context.BlogPosts.FirstOrDefaultAsync(bp => bp.Id == id);
        if (blogPost != null)
        {
            _context.BlogPosts.Remove(blogPost);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<bool> CanUserEditBlogPostAsync(string userId, Guid blogPostId)
    {
        var blogPost = await _context.BlogPosts.FirstOrDefaultAsync(bp => bp.Id == blogPostId);
        return blogPost != null && blogPost.UserId == userId;
    }
    public async Task<Response<List<BlogPost>>> GetPaginatedBlogPostsAsync(int pageIndex, int pageSize)
    {
        var totalItems = await _context.BlogPosts.CountAsync();
        var totalPages = (int)Math.Ceiling((double)totalItems / pageSize);

        var skip = (pageIndex - 1) * pageSize;
        var blogPosts = await _context.BlogPosts.Skip(skip).Take(pageSize).ToListAsync();

        var response = new Response<List<BlogPost>>
        {
            Status = 200,
            Title = "Success",
            Errors = null,
            Data = blogPosts,
            PageInfo = new PaginationInfo
            {
                TotalPages = totalPages,
                TotalItems = totalItems,
                PageIndex = pageIndex,
                PageSize = pageSize
            }
        };

        return response;
    }
}
