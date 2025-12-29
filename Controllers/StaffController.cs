using Microsoft.AspNetCore.Mvc;
using Together.DTOs.Staf;
using Together.Models;
using Together.Services;

namespace Together.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StaffController : ControllerBase
    {
        private readonly StaffService _staffService;
        public StaffController(StaffService staffService)
        {
            _staffService = staffService;
        }

        [HttpGet]
        public async Task<ActionResult<List<ViewStaffDto>>> GetAllStaff()
        {
            var staffs = await _staffService.GetAllStaff();
            return staffs;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ViewStaffDto>> GetStaffById(int id)
        {
            var staff = await _staffService.GetStaffById(id);
            if (staff == null)
                return NotFound();
            return staff;
        }

        [HttpGet("organization/{organId}")]
        public async Task<ActionResult<List<ViewStaffDto>>> GetStaffByOrganId(int organId)
        {
            var staffs = await _staffService.GetStaffByOrganId(organId);
            return staffs;
        }

        [HttpPost]
        public async Task<ActionResult> CreateStaff([FromForm] CreateStaffDto dto)
        {
            var result = await _staffService.CreateStaff(dto);

            if (!result.Success)
                return BadRequest(result.Message);

            return Ok(result.Message);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateStaff(int id, [FromForm] UpdateStaffDto dto)
        {
            var result = await _staffService.UpdateStaff(id, dto);
            if (!result.Success)
                return BadRequest(result.Message);

            return Ok(result.Message);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteStaff(int id)
        {
            var result = await _staffService.DeleteStaff(id);
            if (!result.Success)
                return BadRequest(result.Message);

            return Ok(result.Message);
        }

        [HttpGet("filter")]
        public async Task<ActionResult<List<ViewStaffDto>>> GetStaffByFilter(
            [FromQuery] string? Name,
            [FromQuery] string? Email,
            [FromQuery] string? PhoneNumber,
            [FromQuery] int? OrganizationId,
            [FromQuery] StaffRole? Role,
            [FromQuery] DateTime? JoinedAt,
            [FromQuery] DateTime? LeftAt,
            [FromQuery] bool? IsActive)
        {
            var dto = new StaffFilterDto
            {
                Name = Name,
                Email = Email,
                PhoneNumber = PhoneNumber,
                OrganizationId = OrganizationId,
                Role = Role,
                JoinedAt = JoinedAt,
                LeftAt = LeftAt,
                IsActive = IsActive
            };
            var staffs = await _staffService.GetStaffByFilterAsync(dto);
            return staffs;
        }
    }
}
