using Microsoft.AspNetCore.Mvc;
using Tedwin.Api.Model;
using Tedwin.Api.Model.Dto;
using Tedwin.Api.Services.TagsInfo;

namespace Tedwin.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TagsController : ControllerBase
{
    private readonly ITagService _tagService;

    public TagsController(ITagService tagService)
    {
        _tagService = tagService;
    }

    [HttpGet]
    public async Task<ActionResult<Response<List<Tag>>>> GetAllTags()
    {
        var tags = await _tagService.GetAllTagsAsync();
        var response = new Response<List<Tag>>
        {
            Status = 200,
            Title = null,
            Errors = new Dictionary<string, object>(),
            Data = tags
        };
        return Ok(response);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Response<Tag>>> GetTagById(int id)
    {
        var tag = await _tagService.GetTagByIdAsync(id);
        if (tag == null)
        {
            var notFoundResponse = new Response<Tag>
            {
                Status = 404,
                Title = "Tag not found",
                Errors = new Dictionary<string, object>(),
                Data = null
            };
            return NotFound(notFoundResponse);
        }
        var response = new Response<Tag>
        {
            Status = 200,
            Title = null,
            Errors = new Dictionary<string, object>(),
            Data = tag
        };
        return Ok(response);
    }

    [HttpPost]
    public async Task<IActionResult> CreateTag(Tag tag)
    {
        await _tagService.CreateTagAsync(tag);
        var response = new Response<Tag>
        {
            Status = 200,
            Title = null,
            Errors = new Dictionary<string, object>(),
            Data = tag
        };
        return Ok(response);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateTag(int id, Tag tag)
    {
        if (id != tag.Id)
        {
            return BadRequest();
        }

        await _tagService.UpdateTagAsync(tag);
        var response = new Response<Tag>
        {
            Status = 200,
            Title = null,
            Errors = new Dictionary<string, object>(),
            Data = tag
        };
        return Ok(response);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteTag(int id)
    {
        var tag = await _tagService.GetTagByIdAsync(id);
        if (tag == null)
        {
            var notFoundResponse = new Response<Tag>
            {
                Status = 404,
                Title = "Tag not found",
                Errors = new Dictionary<string, object>(),
                Data = null
            };
            return NotFound(notFoundResponse);
        }

        await _tagService.DeleteTagAsync(tag);
        var response = new Response<Tag>
        {
            Status = 200,
            Title = null,
            Errors = new Dictionary<string, object>(),
            Data = tag
        };
        return Ok(response);
    }
}