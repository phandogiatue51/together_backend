using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Together.DTOs.Pro;
using Together.Models;
using Together.Repositories;
using Together.Services;

namespace Together.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProjectsController : ControllerBase
    {
        private readonly ProjectService _projectService;
        private readonly CategoryService _categoryService;

        public ProjectsController(ProjectService projectService, CategoryService categoryService)
        {
            _projectService = projectService;
            _categoryService = categoryService;
        }

        [HttpGet]
        public async Task<ActionResult<List<ViewProjectDto>>> GetAllProjects()
        {
            var projects = await _projectService.GetAllProjects();
            return projects;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ViewProjectDto>> GetProjectById(int id)
        {
            var project = await _projectService.GetProjectById(id);
            if (project == null)
                return NotFound(new { message = "Project not found" });

            return project;
        }

        [Authorize(Roles = "Admin,Staff")]
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

        [Authorize(Roles = "Admin,Staff")]
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

        [Authorize(Roles = "Admin,Staff")]
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
            [FromQuery] DateTime? CreatedAt,
            [FromQuery] int? organizationId,
            [FromQuery] List<int>? categoryIds)
        {
            var filter = new ProjectFilterDto
            {
                Title = Title,
                Type = Type,
                StartDate = StartDate,
                EndDate = EndDate,
                Location = Location,
                Status = Status,
                CreatedAt = CreatedAt,
                OrganizationId = organizationId,
                CategoryIds = categoryIds ?? new List<int>()
            };
            var projects = await _projectService.GetProjectsByFilter(filter);
            return projects;
        }

        [HttpGet("homepage-project")]
        public async Task<ActionResult<List<ViewProjectDto>>> GetHomepageProject()
        {
            var projects = await _projectService.GetHomePageProject();
            return projects;
        }

        [HttpGet("matched-projects/{accountId}")]
        public async Task<ActionResult<List<ViewMatchedProjectDto>>> GetMatchedProjects(int accountId, [FromQuery] string? location)
        {
            try
            {
                var matchedProjects = await _projectService.GetMatchedProject(accountId, location);

                if (!matchedProjects.Any())
                {
                    return Ok(new
                    {
                        Message = "No matching projects found. Consider adding more certificates to your profile.",
                        Suggestions = await _categoryService.GetSkillSuggestions(accountId)
                    });
                }

                return Ok(new
                {
                    TotalMatches = matchedProjects.Count,
                    PerfectMatches = matchedProjects.Count(p => p.MatchPercentage >= 80),
                    Projects = matchedProjects
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error finding matches: {ex.Message}");
            }
        }
    }
}