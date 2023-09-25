using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Tedwin.Api.Data;
using Tedwin.Api.Model;

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
}
