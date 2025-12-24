using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Together.DTOs;
using Together.Services;

namespace Together.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private readonly CategoryService _cateService;
        public CategoryController(CategoryService cateService)
        {
            _cateService = cateService;
        }

        [HttpGet]
        public async Task<ActionResult<List<ViewCateDto>>> GetAllCategories()
        {
            var categories = await _cateService.GetAllCategoriesAsync();
            return categories;
        }
    }
}
