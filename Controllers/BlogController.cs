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
        public async Task<ActionResult<List<ViewBlogDto>>> GetAllBlogs()
        {
            var blogs = await _blogService.GetAllBlogPostsAsync();
            return Ok(blogs);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ViewBlogDto>> GetBlogById(int id)
        {
            var blog = await _blogService.GetBlogPostByIdAsync(id);
            if (blog == null)
                return NotFound();
            return Ok(blog);
        }

        [HttpPost]
        public async Task<ActionResult> CreateBlog([FromForm] CreateBlogDto dto)
        {
            var result = await _blogService.CreateBlog(dto);
            if (!result.Success)
                return BadRequest(result.Message);
            return Ok(result.Message);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateBlog(int id, [FromForm] UpdateBlogDto dto, int accountId)
        {
            var result = await _blogService.UpdateBlog(id, dto, accountId);
            if (!result.Success)
                return BadRequest(result.Message);
            return Ok(result.Message);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteBlog(int id, int accountId)
        {
            var result = await _blogService.DeleteBlog(id, accountId);
            if (!result.Success)
                return BadRequest(result.Message);
            return Ok(result.Message);
        }

        [HttpGet("filter")]
        public async Task<ActionResult<List<ViewBlogDto>>> GetBlogsByFilter(
            [FromQuery] string? Title,
            [FromQuery] int? AuthorId,
            [FromQuery] int? OrganizationId,
            [FromQuery] DateTime? PublishDate,
            [FromQuery] bool? Status)
        {
            var filter = new BlogFilterDto
            {
                Title = Title,
                AuthorId = AuthorId,
                OrganizationId = OrganizationId,
                PublishDate = PublishDate,
                Status = Status
            };
            var blogs = await _blogService.GetBlogsByFilterAsync(filter);
            return Ok(blogs);
        }
    }
}
