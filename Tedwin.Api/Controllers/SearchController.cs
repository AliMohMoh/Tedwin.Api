using Microsoft.AspNetCore.Mvc;
using Tedwin.Api.Model;
using Tedwin.Api.Model.Dto;
using Tedwin.Api.Services.BlogPostInfo;
using Tedwin.Api.Services.SearchInfo;

namespace Tedwin.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SearchController : ControllerBase
{
    private readonly ISearchAndFilterService _searchAndFilterService;

    public SearchController(ISearchAndFilterService searchAndFilterService)
    {
        _searchAndFilterService = searchAndFilterService;
    }

    [HttpGet("SearchAndFilter")]
    public async Task<ActionResult<Response<List<BlogPostWithTags>>>> Search(string? searchTerm,[FromQuery] List<string> tagNames, int pageNumber, int pageSize)
    {
        var response = await _searchAndFilterService.SearchAsync(searchTerm, tagNames, pageNumber, pageSize);
        return StatusCode(response.Status, response);
    }
}


