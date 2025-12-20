using Together.DTOs.Organ;
using Together.DTOs.Pro;
using Together.Models;
using Together.Repositories;

namespace Together.Services
{
    public class ProjectService
    {
        private readonly ProjectRepo _projectRepo;
        public ProjectService(ProjectRepo projectRepo)
        {
            _projectRepo = projectRepo;
        }

        public async Task<List<ViewProjectDto>> GetAllProjects()
        {
            var projects = await _projectRepo.GettAll();

            return projects.Select(m => new ViewProjectDto
            {
                Id = m.Id,
                Title = m.Title,
                Description = m.Description,
                StartDate = m.StartDate,
                EndDate = m.EndDate,
                OrganizationId = m.OrganizationId,
                OrganizationName = m.Organization?.Name,
                RequiredVolunteers = m.RequiredVolunteers,
                CurrentVolunteers = m.CurrentVolunteers
            }).ToList();
        }

        public async Task<ViewProjectDto?> GetProjectById(int id)
        {
            var project = await _projectRepo.GetByIdAsync(id);

            if (project == null)
                return null;

            return new ViewProjectDto
            {
                Id = project.Id,
                Title = project.Title,
                Description = project.Description,
                StartDate = project.StartDate,
                EndDate = project.EndDate,
                OrganizationId = project.OrganizationId,
                OrganizationName = project.Organization?.Name,
                RequiredVolunteers = project.RequiredVolunteers,
                CurrentVolunteers = project.CurrentVolunteers
            };
        }

        public async Task<(bool Success, string Message)> CreateProject(CreateProjectDto dto)
        {
            var project = new Project
            {
                Title = dto.Title,
                Description = dto.Description,
                StartDate = dto.StartDate,
                EndDate = dto.EndDate,
                OrganizationId = dto.OrganizationId,
                RequiredVolunteers = dto.RequiredVolunteers,
            };

            await _projectRepo.AddAsync(project);
            return (true, "Project created successfully.");
        }

        public async Task<(bool Success, string Message)> UpdateProject(int id, CreateProjectDto dto)
        {
            var project = await _projectRepo.GetByIdAsync(id);
            if (project == null)
                return (false, "Project not found.");

            project.Title = dto.Title;
            project.Description = dto.Description;
            project.StartDate = dto.StartDate;
            project.EndDate = dto.EndDate;
            project.OrganizationId = dto.OrganizationId;
            project.RequiredVolunteers = dto.RequiredVolunteers;

            await _projectRepo.UpdateAsync(project);
            return (true, "Project updated successfully.");
        }

        public async Task<(bool Success, string Message)> DeleteProject(int id)
        {
            var project = await _projectRepo.GetByIdAsync(id);
            if (project == null)
                return (false, "Project not found.");

            await _projectRepo.DeleteAsync(project);
            return (true, "Project deleted successfully.");
        }
    }
}
