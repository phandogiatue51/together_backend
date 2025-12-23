using Microsoft.AspNetCore.Mvc;
using Together.DTOs.Pro;
using Together.Models;
using Together.Services;

namespace Together.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProjectsController : ControllerBase
    {
        private readonly ProjectService _projectService;

        public ProjectsController(ProjectService projectService)
        {
            _projectService = projectService;
        }

        [HttpGet]
        public async Task<ActionResult<List<ViewProjectDto>>> GetAllProjects()
        {
            var projects = await _projectService.GetAllProjects();
            return Ok(projects);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ViewProjectDto>> GetProjectById(int id)
        {
            var project = await _projectService.GetProjectById(id);
            if (project == null)
                return NotFound(new { message = "Project not found" });

            return Ok(project);
        }

        [HttpPost]
        public async Task<ActionResult> CreateProject([FromForm] CreateProjectDto dto)
        {

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _projectService.CreateProject(dto, dto.ImageUrl);

            if (!result.Success)
                return BadRequest(new { message = result.Message });

            return CreatedAtAction(nameof(GetProjectById),
                new { id = result.ProjectId },
                new { message = result.Message, projectId = result.ProjectId });
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateProject(int id, [FromForm] UpdateProjectDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _projectService.UpdateProject(id, dto, dto.ImageUrl);

            if (!result.Success)
                return BadRequest(new { message = result.Message });

            return Ok(new { message = result.Message });
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteProject(int id)
        {
            var result = await _projectService.DeleteProject(id);
            if (!result.Success)
                return BadRequest(new { message = result.Message });
            return Ok(new { message = result.Message });
        }

        [HttpGet("filter")]
        public async Task<ActionResult<List<ViewProjectDto>>> GetProjectsByFilter(
            [FromQuery] string? Title,
            [FromQuery] ProjectType? Type,
            [FromQuery] DateTime? StartDate,
            [FromQuery] DateTime? EndDate,
            [FromQuery] string? Location,
            [FromQuery] ProjectStatus? Status,
            [FromQuery] int? RequiredVolunteers,
            [FromQuery] int? CurrentVolunteers,
            [FromQuery] DateTime? CreatedAt)
        {
            var filter = new ProjectFilterDto
            {
                Title = Title,
                Type = Type,
                StartDate = StartDate,
                EndDate = EndDate,
                Location = Location,
                Status = Status,
                RequiredVolunteers = RequiredVolunteers,
                CurrentVolunteers = CurrentVolunteers,
                CreatedAt = CreatedAt
            };
            var projects = await _projectService.GetProjectsByFilter(filter);
            return Ok(projects);
        }
    }
}
