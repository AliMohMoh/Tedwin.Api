//using Microsoft.AspNetCore.Authorization;
//using Microsoft.AspNetCore.Identity;
//using Microsoft.AspNetCore.Mvc;
//using System;
//using System.Security.Claims;
//using System.Threading.Tasks;
//using Tedwin.Api.Model;
//using Tedwin.Api.Services.BlogPostInfo;

//namespace Tedwin.Api.Controllers;

////[Authorize]
//[ApiController]
//[Route("api/[controller]")]
//public class BlogPostsController : ControllerBase
//{
//    private readonly IBlogPostService _blogPostService;

//    public BlogPostsController(IBlogPostService blogPostService)
//    {
//        _blogPostService = blogPostService;
//    }

//    [HttpGet(nameof(GetPageBlogPosts))]
//    public async Task<ActionResult<IEnumerable<BlogPost>>> GetPageBlogPosts(string tagName = null, int pageNumber = 1, int pageSize = 10)
//    {
//        var blogPosts = await _blogPostService.GetPagedBlogPosts(tagName, pageNumber, pageSize);

//        return Ok(blogPosts);
//    }
//    [HttpGet]
//    public async Task<ActionResult<List<BlogPost>>> GetAllBlogPosts()
//    {
//        var blogPosts = await _blogPostService.GetAllBlogPosts();
//        return Ok(blogPosts);
//    }

//    [HttpGet("{id}")]
//    public async Task<ActionResult<BlogPost>> GetBlogPostById(Guid id)
//    {
//        var blogPost = await _blogPostService.GetBlogPostById(id);
//        if (blogPost == null)
//            return NotFound();

//        return Ok(blogPost);
//    }

//    [HttpPost]
//    public async Task<ActionResult<BlogPost>> CreateBlogPost(BlogPost blogPost)
//    {
//        var createdBlogPost = await _blogPostService.CreateBlogPost(blogPost);
//        return CreatedAtAction(nameof(GetBlogPostById), new { id = createdBlogPost.Id }, createdBlogPost);
//    }

//    [HttpPut("{id}")]
//    public async Task<ActionResult> UpdateBlogPost(Guid id, BlogPost updatedBlogPost)
//    {
//        var blogPost = await _blogPostService.GetBlogPostById(id);
//        if (blogPost == null)
//            return NotFound();

//        // Check if the authenticated user is the owner of the blog post
//        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
//        if (blogPost.UserId != userId)
//            return Forbid();

//        var success = await _blogPostService.UpdateBlogPost(id, updatedBlogPost);
//        if (!success)
//            return NotFound();

//        return NoContent();
//    }

//    [HttpDelete("{id}")]
//    public async Task<ActionResult> DeleteBlogPost(Guid id)
//    {
//        var blogPost = await _blogPostService.GetBlogPostById(id);
//        if (blogPost == null)
//            return NotFound();

//        // Check if the authenticated user is the owner of the blog post
//        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
//        if (blogPost.UserId != userId)
//            return Forbid();

//        var success = await _blogPostService.DeleteBlogPost(id);
//        if (!success)
//            return NotFound();

//        return NoContent();
//    }
//}
