//using Microsoft.AspNetCore.Identity;
//using Microsoft.EntityFrameworkCore;
//using Tedwin.Api.Data;
//using Tedwin.Api.Model;

//namespace Tedwin.Api.Services.BlogPostInfo;

//public class BlogPostService : IBlogPostService
//{
//    private readonly ApplicationDbContext _dbContext;

//    public BlogPostService(ApplicationDbContext dbContext)
//    {
//        _dbContext = dbContext;
//    }
//    public async Task<List<BlogPost>> GetPagedBlogPosts(int pageNumber, int pageSize)
//    {
//        var blogPostsQuery = _dbContext.BlogPosts.AsQueryable();

//        var pagedBlogPosts = await blogPostsQuery
//            .Skip((pageNumber - 1) * pageSize)
//            .Take(pageSize)
//            .ToListAsync();

//        foreach (var blogPost in pagedBlogPosts)
//        {
//            var tags = await _dbContext.Tags
//                .Where(t => t.BlogPostId == blogPost.Id)
//                .Select(t => t.Name)
//                .ToListAsync();

//            blogPost.Tags = tags;
//        }

//        return pagedBlogPosts;
//    }
//    public async Task<List<BlogPost>> GetAllBlogPosts()
//    {
//        return await _dbContext.BlogPosts
//            .Include(b => b.Tags)
//            .ToListAsync();
//    }

//    public async Task<BlogPost> GetBlogPostById(Guid id)
//    {
//        return await _dbContext.BlogPosts
//            .Include(b => b.Tags)
//            .FirstOrDefaultAsync(b => b.Id == id);
//    }

//    public async Task<BlogPost> CreateBlogPost(BlogPost blogPost)
//    {
//        blogPost.Id=Guid.NewGuid();
//        _dbContext.BlogPosts.Add(blogPost);
//        await _dbContext.SaveChangesAsync();
//        return blogPost;
//    }

//    public async Task<bool> UpdateBlogPost(Guid id, BlogPost updatedBlogPost)
//    {
//        var blogPost = await _dbContext.BlogPosts.FirstOrDefaultAsync(b => b.Id == id);
//        if (blogPost == null)
//            return false;

//        blogPost.Title = updatedBlogPost.Title;
//        blogPost.Content = updatedBlogPost.Content;
//        blogPost.Tags = updatedBlogPost.Tags;

//        await _dbContext.SaveChangesAsync();
//        return true;
//    }

//    public async Task<bool> DeleteBlogPost(Guid id)
//    {
//        var blogPost = await _dbContext.BlogPosts.FirstOrDefaultAsync(b => b.Id == id);
//        if (blogPost == null)
//            return false;

//        _dbContext.BlogPosts.Remove(blogPost);
//        await _dbContext.SaveChangesAsync();
//        return true;
//    }
//}
