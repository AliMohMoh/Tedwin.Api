using Microsoft.EntityFrameworkCore;
using Tedwin.Api.Data;
using Tedwin.Api.Model;
using System.Linq;
using Tedwin.Api.Model.Dto;

namespace Tedwin.Api.Services.SearchInfo;

public class SearchAndFilterService : ISearchAndFilterService
{
    private readonly ApplicationDbContext _dbContext;

    public SearchAndFilterService(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Response<List<BlogPostWithTags>>> SearchAsync(string searchTerm, List<string> tagNames, int pageNumber, int pageSize)
    {
        var response = new Response<List<BlogPostWithTags>>();

        try
        {
            var blogPostsWithTags = await GetBlogPostsWithTagsAsync(searchTerm, tagNames, pageNumber, pageSize);

            var totalItems = await CalculateTotalItemsAsync(searchTerm, tagNames);
            var totalPages = (int)Math.Ceiling((double)totalItems / pageSize);

            var pageInfo = new PaginationInfo
            {
                TotalPages = totalPages,
                TotalItems = totalItems,
                PageIndex = pageNumber,
                PageSize = pageSize
            };

            response.Status = 200;
            response.Title = "Success";
            response.Data = blogPostsWithTags;
            response.PageInfo = pageInfo;
        }
        catch (Exception ex)
        {
            response.Status = 500;
            response.Title = "Error";
            response.Errors = new Dictionary<string, object>
        {
            { "ErrorMessage", ex.Message }
        };
        }

        return response;
    }
    private async Task<int> CalculateTotalItemsAsync(string searchTerm, List<string> tagNames)
    {
        var query = _dbContext.BlogPosts.AsQueryable();

        if (!string.IsNullOrEmpty(searchTerm))
        {
            query = query.Where(post => post.Title.Contains(searchTerm));
        }

        if (tagNames != null && tagNames.Any())
        {
            var tagIds = await _dbContext.Tags
                .Where(tag => tagNames.Contains(tag.Name))
                .Select(tag => tag.Id)
                .ToListAsync();

            query = query.Where(post => post.BlogPostTags.Any(postTag => tagIds.Contains(postTag.TagId)));
        }

        var totalItems = await query.CountAsync();

        return totalItems;
    }
    private async Task<List<BlogPostWithTags>> GetBlogPostsWithTagsAsync(string searchTerm, List<string> tagNames, int pageNumber, int pageSize)
    {
        var query = _dbContext.BlogPosts.AsQueryable();

        if (!string.IsNullOrEmpty(searchTerm))
        {
            query = query.Where(post => post.Title.Contains(searchTerm));
        }

        if (tagNames != null && tagNames.Any())
        {
            var tagIds = await _dbContext.Tags
                .Where(tag => tagNames.Contains(tag.Name))
                .Select(tag => tag.Id)
                .ToListAsync();

            query = query.Where(post => post.BlogPostTags.Any(postTag => tagIds.Contains(postTag.TagId)));
        }

        var results = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        var blogPostsWithTags = results.Select(post => new BlogPostWithTags
        {
            BlogPost = post,
            Tags =  GetTagsForBlogPost(post.Id).Result
        }).ToList();

        return blogPostsWithTags;
    }

    private async Task<List<Tag>> GetTagsForBlogPost(Guid blogPostId)
    {
        var tagIds = await _dbContext.BlogPostTags
            .Where(postTag => postTag.BlogPostId == blogPostId)
            .Select(postTag => postTag.TagId)
            .ToListAsync();

        var tags = await _dbContext.Tags
            .Where(tag => tagIds.Contains(tag.Id))
            .ToListAsync();

        return tags;
    }
}





