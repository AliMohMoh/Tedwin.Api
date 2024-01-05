using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Tedwin.Api.Data;
using Tedwin.Api.Model;
using Tedwin.Api.Model.Dto;

namespace Tedwin.Api.Services.BlogPostInfo;

public class BlogPostService : IBlogPostService
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<IdentityUser> _userManager;
    public BlogPostService(ApplicationDbContext context, UserManager<IdentityUser> userManager)
    {
        _context = context;
        _userManager = userManager;
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
        blogPost.Id = Guid.NewGuid();
        blogPost.CreatedAt = DateTime.UtcNow;
    
        _context.BlogPosts.Add(blogPost);

        if (blogPost.BlogPostTags is not null)
        {
            foreach (var blogPostTag in blogPost.BlogPostTags)
            {
                blogPostTag.BlogPostId = blogPost.Id;
            }
            _context.BlogPostTags.AddRange(blogPost.BlogPostTags);
        }
        //_context.BlogPostTags.AddRange(blogPost.BlogPostTags);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateBlogPostAsync(BlogPost blogPost)
    {
        var existingBlogPost = await _context.BlogPosts.FindAsync(blogPost.Id);
        if (existingBlogPost is null)
        {
            // Handle the case where the blog post does not exist
            throw new InvalidOperationException("The blog post does not exist.");
        }

        // Update the properties of the existing blog post
        existingBlogPost.Title = blogPost.Title;
        existingBlogPost.Content = blogPost.Content;
        // ... update other properties as needed

        // Update the related blog post tags
        if (blogPost.BlogPostTags != null)
        {
            // Remove the existing tags if any
            if (existingBlogPost.BlogPostTags != null && existingBlogPost.BlogPostTags.Any())
            {
                _context.BlogPostTags.RemoveRange(existingBlogPost.BlogPostTags);
            }
            // Add the updated tags
            existingBlogPost.BlogPostTags = blogPost.BlogPostTags;
        }

        await _context.SaveChangesAsync();
    }

    public async Task DeleteBlogPostAsync(Guid id)
    {
        var blogPost = await _context.BlogPosts.FirstOrDefaultAsync(bp => bp.Id == id);
        var blogPostTags = await _context.BlogPostTags.Where(bp => bp.BlogPostId == id).ToListAsync();
        if (blogPost != null)
        {
            _context.BlogPostTags.RemoveRange(blogPostTags);
            _context.BlogPosts.Remove(blogPost);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<bool> CanUserEditBlogPostAsync(string userId, Guid blogPostId)
    {
        var blogPost = await _context.BlogPosts.FirstOrDefaultAsync(bp => bp.Id == blogPostId);
       
        return blogPost != null && blogPost.UserId == userId;
    }
    public async Task<Response<List<BlogPost>>> GetPaginatedBlogPostsAsync(PaginationInfo pagination /*int pageIndex, int pageSize*/)
    {
        var totalItems = await _context.BlogPosts.CountAsync();
        var totalPages = (int)Math.Ceiling((double)totalItems / pagination.PageSize);

        var skip = (pagination.PageIndex - 1) * pagination.PageSize;
        var blogPosts = await _context.BlogPosts.Skip(skip).Take(pagination.PageSize).ToListAsync();

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
                PageIndex = pagination.PageIndex,
                PageSize = pagination.PageSize
            }
        };

        return response;
    }
}
