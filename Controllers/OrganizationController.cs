using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Together.DTOs.Organ;
using Together.Models;
using Together.Services;

namespace Together.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrganizationController : ControllerBase
    {
        private readonly OrganizationService _organService;

        public OrganizationController(OrganizationService organService)
        {
            _organService = organService;
        }

        [HttpGet]
        public async Task<ActionResult<List<ViewOrganDto>>> GetAllOrgans()
        {
            var organs = await _organService.GetAllOrgans();
            return organs;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ViewOrganDto>> GetOrganById(int id)
        {
            var organ = await _organService.GetOrganById(id);
            if (organ == null)
                return NotFound();
            return organ;
        }

        [HttpPost]
        public async Task<ActionResult> CreateOrgan([FromForm] CreateOrganDto dto)
        {
            var result = await _organService.CreateOrgan(dto, dto.ImageFile);

            if (!result.Success)
                return BadRequest(result.Message);

            return Ok(result.Message);
        }

        [HttpPost("create-with-manager")]
        public async Task<ActionResult> RegisterOrganization([FromForm] CreateOrganWithManagerDto dto)
        {
            var result = await _organService.CreateOrganWithManager(dto, dto.ImageFile);

            if (!result.Success)
                return BadRequest(result.Message);

            return Ok(new
            {
                result.Message, result.OrganizationId, result.StaffId
            });
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateOrgan(int id, [FromForm] CreateOrganDto dto)
        {
            var result = await _organService.UpdateOrgan(id, dto, dto.ImageFile);

            if (!result.Success)
                return BadRequest(result.Message);

            return Ok(result.Message);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteOrgan(int id)
        {
            var result = await _organService.DeleteOrgan(id);
            if (!result.Success)
            {
                return BadRequest(result.Message);
            }
            return Ok(result.Message);
        }

        [HttpPut("verify/{id}")]
        public async Task<ActionResult> VerifyOrgan(int id, VerifyOrganDto dto)
        {
            var result = await _organService.VerifyOrgan(id, dto);
            if (!result.Success)
            {
                return BadRequest(result.Message);
            }
            return Ok(result.Message);
        }

        [HttpGet("filter")]
        public async Task<ActionResult<List<ViewOrganDto>>> FilterOrgans(
            [FromQuery] string? name, 
            [FromQuery] string? address,
            [FromQuery] string? email,
            [FromQuery] string? phoneNumber,
            [FromQuery] OrganizationType? type,
            [FromQuery] OrganzationStatus? status)
        {
            var filter = new OrganFilterDto
            {
                Name = name,
                Address = address,
                Email = email,
                PhoneNumber = phoneNumber,
                Type = type,
                Status = status
            };
            var organs = await _organService.GetOrgansByFilter(filter);
            return organs;
        }
    }
}
