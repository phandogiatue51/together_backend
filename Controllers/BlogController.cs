using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Together.DTOs.Blog;
using Together.Services;

namespace Together.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BlogController : ControllerBase
    {
        private readonly BlogService _blogService;
        public BlogController(BlogService blogService)
        {
            _blogService = blogService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllBlogs()
        {
            var blogs = await _blogService.GetAllBlogPostsAsync();
            return Ok(blogs);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetBlogById(int id)
        {
            var blog = await _blogService.GetBlogPostByIdAsync(id);
            if (blog == null)
                return NotFound();
            return Ok(blog);
        }

        [HttpPost]
        public async Task<IActionResult> CreateBlog([FromForm] CreateBlogDto dto)
        {
            var result = await _blogService.CreateBlog(dto);
            if (!result.Success)
                return BadRequest(result.Message);
            return Ok(result.Message);
        }

        [HttpPut("id")]
        public async Task<IActionResult> UpdateBlog(int id, [FromForm] UpdateBlogDto dto, int accountId)
        {
            var result = await _blogService.UpdateBlog(id, dto, accountId);
            if (!result.Success)
                return BadRequest(result.Message);
            return Ok(result.Message);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBlog(int id, int accountId)
        {
            var result = await _blogService.DeleteBlog(id, accountId);
            if (!result.Success)
                return BadRequest(result.Message);
            return Ok(result.Message);
        }
    }
}
