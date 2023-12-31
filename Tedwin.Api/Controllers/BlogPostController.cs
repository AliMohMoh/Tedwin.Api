﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Tedwin.Api.Data;
using Tedwin.Api.Model;
using Tedwin.Api.Model.Dto;
using Tedwin.Api.Services.BlogPostInfo;

namespace Tedwin.Api.Controllers;


[ApiController]
[Route("api/[controller]")]
public class BlogPostsController : ControllerBase
{
    private readonly IBlogPostService _blogPostService;
    private readonly ApplicationDbContext _context;

    public BlogPostsController(IBlogPostService blogPostService, ApplicationDbContext context)
    {
        _blogPostService = blogPostService;
        _context = context;
        
    }
    [AllowAnonymous]
    [HttpPost(nameof(GetPageBlogPosts))]
    public async Task<ActionResult<Response<List<BlogPost>>>> GetPageBlogPosts(PaginationInfo pagination)
    {
        var response = await _blogPostService.GetPaginatedBlogPostsAsync(pagination);
        return Ok(response);
    }
    [AllowAnonymous]
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
    [AllowAnonymous]
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
    [Authorize]
    [HttpPost]
    public async Task<IActionResult> CreateBlogPost(BlogPost blogPost)
    {

        // Retrieve the user ID from the token
        var claimsIdentity = User.Identity as ClaimsIdentity;
        var userIdClaim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim == null)
        {
            // Handle the case where the user ID claim is missing or not found in the token
            return BadRequest("User ID claim not found in the token.");
        }

        string userId = userIdClaim.Value;

        blogPost.UserId = userId; // Assign the user ID to the UserId property

        await _blogPostService.CreateBlogPostAsync(blogPost);

        var createdResponse = new Response<BlogPost>
        {
            Status = 201,
            Title = "Created",
            Data = blogPost
        };

        return CreatedAtAction(nameof(GetBlogPostById), new { id = blogPost.Id }, createdResponse);
    }
    [Authorize]
    [HttpPut]
    public async Task<IActionResult> UpdateBlogPost(BlogPost blogPost)
    {
        var claimsIdentity = User.Identity as ClaimsIdentity;
        var userIdClaim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
        string userId = userIdClaim.Value;

        if (!await _blogPostService.CanUserEditBlogPostAsync(userId,blogPost.Id))
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

        //var exit = await _context.BlogPosts.FindAsync(blogPost.Id);
        // if (exit is null)
        //{
        //    var badRequestResponse = new Response<string>
        //    {
        //        Status = 400,
        //        Title = "Bad Request",
        //        Errors = new Dictionary<string, object>
        // {
        //     { "message", "The provided ID does not match the ID in the blog post object." }
        // }
        //    };

        //    return BadRequest(badRequestResponse);
        //}

        await _blogPostService.UpdateBlogPostAsync(blogPost);

        var successResponse = new Response<string>
        {
            Status = 200,
            Title = "Success"
        };

        return Ok(successResponse);
    }
    [Authorize]
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteBlogPost(Guid id)
    {
        var claimsIdentity = User.Identity as ClaimsIdentity;
        var userIdClaim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
        string userId = userIdClaim.Value;

        if (!await _blogPostService.CanUserEditBlogPostAsync(userId, id))
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