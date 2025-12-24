using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Together.DTOs.App;
using Together.DTOs.Certi;
using Together.Models;
using Together.Services;

namespace Together.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ApplicationController : ControllerBase
    {
        private readonly ApplicationService _applicationService;

        public ApplicationController(ApplicationService applicationService)
        {
            _applicationService = applicationService;
        }

        [HttpGet]
        public async Task<ActionResult<List<ViewAppDto>>> GetAllApplications()
        {
            var apps = await _applicationService.GetAllApplicationsAsync();
            return apps;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ViewAppDto>> GetApplicationById(int id)
        {
            var app = await _applicationService.GetApplicationByIdAsync(id);
            if (app == null)
                return NotFound();
            return app;
        }

        [HttpPost]
        public async Task<ActionResult> CreateApplication([FromBody] CreateAppDto dto)
        {
            var result = await _applicationService.CreateApplicationAsync(dto);
            if (!result.Success)
            {
                return BadRequest(result.Message);
            }
            return Ok(result.Message);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateApplication(int id, [FromBody] UpdateAppDto dto)
        {
            var result = await _applicationService.UpdateApplicationAsync(id, dto);
            if (!result.Success)
            {
                return BadRequest(result.Message);
            }
            return Ok(result.Message);
        }

        [HttpPut("review/{id}")]
        public async Task<ActionResult> ReviewApplication(int id, ReviewAppDto dto)
        {
            var result = await _applicationService.ReviewApplicationAsync(id, dto);
            if (!result.Success)
            {
                return BadRequest(result.Message);
            }
            return Ok(result.Message);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteApplication(int id)
        {
            var result = await _applicationService.DeleteApplicationAsync(id);
            if (!result.Success)
            {
                return BadRequest(result.Message);
            }
            return Ok(result.Message);
        }

        [HttpGet("filter")]
        public async Task<ActionResult<List<ViewAppDto>>> GetApplicationsByFilter(
            [FromQuery] int? projectId = null,
            [FromQuery] int? volunteerId = null,
            [FromQuery] ApplicationStatus? status = null,
            [FromQuery] int? organizationId = null)
        {
            var filter = new AppFilterDto
            {
                ProjectId = projectId,
                Status = status,
                VolunteerId = volunteerId,
                OrganizationId = organizationId
            };

            var apps = await _applicationService.GetApplicationsByFilterAsync(filter);
            return apps;
        }
    }
}
