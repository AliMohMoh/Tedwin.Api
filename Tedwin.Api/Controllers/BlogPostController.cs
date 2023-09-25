using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Tedwin.Api.Model;
using Tedwin.Api.Model.Dto;
using Tedwin.Api.Services.BlogPostInfo;

namespace Tedwin.Api.Controllers;

//[Authorize]
[ApiController]
[Route("api/[controller]")]
public class BlogPostsController : ControllerBase
{
    private readonly IBlogPostService _blogPostService;

    public BlogPostsController(IBlogPostService blogPostService)
    {
        _blogPostService = blogPostService;
    }

    [HttpGet(nameof(GetPageBlogPosts))]
    public async Task<ActionResult<Response<List<BlogPost>>>> GetPageBlogPosts(int pageIndex = 1, int pageSize = 100)
    {
        var response = await _blogPostService.GetPaginatedBlogPostsAsync(pageIndex, pageSize);
        return Ok(response);
    }
    [HttpGet]
    public async Task<ActionResult<Response<List<BlogPost>>>> GetAllBlogPosts()
    {
        var blogPosts = await _blogPostService.GetAllBlogPostsAsync();

        var response = new Response<List<BlogPost>>
        {
            Status = 200,
            Title = "Success",
            Data = blogPosts
        };

        return Ok(response);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Response<BlogPost>>> GetBlogPostById(Guid id)
    {
        var blogPost = await _blogPostService.GetBlogPostByIdAsync(id);

        if (blogPost == null)
        {
            var notFoundResponse = new Response<BlogPost>
            {
                Status = 404,
                Title = "Not Found"
            };

            return NotFound(notFoundResponse);
        }

        var successResponse = new Response<BlogPost>
        {
            Status = 200,
            Title = "Success",
            Data = blogPost
        };

        return Ok(successResponse);
    }

    [HttpPost]
    public async Task<IActionResult> CreateBlogPost(BlogPost blogPost)
    {
        await _blogPostService.CreateBlogPostAsync(blogPost);

        var createdResponse = new Response<BlogPost>
        {
            Status = 201,
            Title = "Created",
            Data = blogPost
        };

        return CreatedAtAction(nameof(GetBlogPostById), new { id = blogPost.Id }, createdResponse);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateBlogPost(Guid id, BlogPost blogPost)
    {
        if (!await _blogPostService.CanUserEditBlogPostAsync(User.Identity.Name, id))
        {
            var forbiddenResponse = new Response<string>
            {
                Status = 403,
                Title = "Forbidden",
                Errors = new Dictionary<string, object>
                {
                    { "message", "You are not authorized to edit this blog post." }
                }
            };

            return StatusCode(403, forbiddenResponse);
        }

        if (id != blogPost.Id)
        {
            var badRequestResponse = new Response<string>
            {
                Status = 400,
                Title = "Bad Request",
                Errors = new Dictionary<string, object>
                {
                    { "message", "The provided ID does not match the ID in the blog post object." }
                }
            };

            return BadRequest(badRequestResponse);
        }

        await _blogPostService.UpdateBlogPostAsync(blogPost);

        var successResponse = new Response<string>
        {
            Status = 200,
            Title = "Success"
        };

        return Ok(successResponse);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteBlogPost(Guid id)
    {
        if (!await _blogPostService.CanUserEditBlogPostAsync(User.Identity.Name, id))
        {
            var forbiddenResponse = new Response<string>
            {
                Status = 403,
                Title = "Forbidden",
                Errors = new Dictionary<string, object>
                {
                    { "message", "You are not authorized to delete this blog post." }
                }
            };

            return StatusCode(403, forbiddenResponse);
        }

        await _blogPostService.DeleteBlogPostAsync(id);

        var successResponse = new Response<string>
        {
            Status = 200,
            Title = "Success"
        };

        return Ok(successResponse);
    }
}