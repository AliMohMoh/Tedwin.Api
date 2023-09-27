using Tedwin.Api.Model;
using Tedwin.Api.Model.Dto;

namespace Tedwin.Api.Services.SearchInfo;

public interface ISearchAndFilterService
{
    Task<Response<List<BlogPostWithTags>>> SearchAsync(string searchTerm, List<string> tagNames, int pageNumber, int pageSize);
}
