using Microsoft.AspNetCore.Mvc;
using Together.DTOs.Organ;
using Together.DTOs.Pro;
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
        public async Task<IActionResult> GetAllProjects()
        {
            var projects = await _projectService.GetAllProjects();
            return Ok(projects);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetProjectById(int id)
        {
            var project = await _projectService.GetProjectById(id);
            if (project == null)
                return NotFound(new { message = "Project not found" });

            return Ok(project);
        }

        [HttpPost]
        public async Task<IActionResult> CreateProject([FromForm] CreateProjectDto dto)
        {
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
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateProject(int id, [FromForm] UpdateProjectDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _projectService.UpdateProject(id, dto, dto.ImageUrl);

            if (!result.Success)
                return BadRequest(new { message = result.Message });

            return Ok(new { message = result.Message });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProject(int id)
        {
            var result = await _projectService.DeleteProject(id);
            if (!result.Success)
                return BadRequest(new { message = result.Message });
            return Ok(new { message = result.Message });
        }
    }
}
