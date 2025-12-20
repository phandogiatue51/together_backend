using Microsoft.AspNetCore.Mvc;
using Together.DTOs.Staf;
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
        public async Task<IActionResult> GetAllStaff()
        {
            var staffs = await _staffService.GetAllStaff();
            return Ok(staffs);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetStaffById(int id)
        {
            var staff = await _staffService.GetStaffById(id);
            if (staff == null)
                return NotFound();
            return Ok(staff);
        }

        [HttpGet("organization/{organId}")]
        public async Task<IActionResult> GetStaffByOrganId(int organId)
        {
            var staffs = await _staffService.GetStaffByOrganId(organId);
            return Ok(staffs);
        }

        [HttpPost]
        public async Task<IActionResult> CreateStaff([FromBody] CreateStaffDto dto)
        {
            var result = await _staffService.CreateStaff(dto);

            if (!result.Success)
                return BadRequest(result.Message);

            return Ok(result.Message);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateStaff(int id, [FromBody] UpdateStaffDto dto)
        {
            var result = await _staffService.UpdateStaff(id, dto);
            if (!result.Success)
                return BadRequest(result.Message);

            return Ok(result.Message);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteStaff([FromQuery] int staffId)
        {
            var result = await _staffService.DeleteStaff(staffId);
            if (!result.Success)
                return BadRequest(result.Message);

            return Ok(result.Message);
        }
    }
}
