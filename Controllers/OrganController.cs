using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Together.DTOs.Organ;
using Together.Services;

namespace Together.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrganController : ControllerBase
    {
        private readonly OrganizationService _organService;

        public OrganController(OrganizationService organService)
        {
            _organService = organService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllOrgans()
        {
            var organs = await _organService.GetAllOrgans();
            return Ok(organs);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetOrganById(int id)
        {
            var organ = await _organService.GetOrganById(id);
            if (organ == null)
                return NotFound();
            return Ok(organ);
        }

        [HttpPost]
        public async Task<IActionResult> CreateOrgan([FromForm] CreateOrganDto dto)
        {
            var result = await _organService.CreateOrgan(dto, dto.ImageFile);

            if (!result.Success)
                return BadRequest(result.Message);

            return Ok(result.Message);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateOrgan(int id, [FromForm] CreateOrganDto dto)
        {
            var result = await _organService.UpdateOrgan(id, dto, dto.ImageFile);

            if (!result.Success)
                return BadRequest(result.Message);

            return Ok(result.Message);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteOrgan(int id)
        {
            var result = await _organService.DeleteOrgan(id);
            if (!result.Success)
            {
                return BadRequest(result.Message);
            }
            return Ok(result.Message);
        }
    }
}
