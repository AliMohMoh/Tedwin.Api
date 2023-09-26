//using Microsoft.EntityFrameworkCore;
//using Tedwin.Api.Data;
//using Tedwin.Api.Model;
//using System.Linq;

//namespace Tedwin.Api.Services.SearchInfo;

//public class SearchAndFilterService : ISearchAndFilterService
//{
//    private readonly ApplicationDbContext _dbContext;

//    public SearchAndFilterService(ApplicationDbContext dbContext)
//    {
//        _dbContext = dbContext;
//    }

//    public async Task<List<BlogPost>> SearchPosts(string searchTerm, List<string> tags)
//    {
//        var query = _dbContext.BlogPosts
//            .Include(bp => bp.BlogPostTags)
//                .ThenInclude(bpt => bpt.Tag)
//            .AsQueryable();

//        if (!string.IsNullOrEmpty(searchTerm))
//        {
//            query = query.Where(bp => bp.Title.Contains(searchTerm));
//        }

//        if (tags != null && tags.Any())
//        {
//            query = query.Where(bp => bp.BlogPostTags.Any(bpt => tags.Contains(bpt.Tag.Name)));
//        }

//        query = query.OrderByDescending(bp => bp.CreatedAt);

//        return await query.ToListAsync();
//    }
//}



