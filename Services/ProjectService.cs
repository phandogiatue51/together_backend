using Together.DTOs;
using Together.DTOs.Pro;
using Together.Helpers;
using Together.Models;
using Together.Repositories;

namespace Together.Services
{
    public class ProjectService
    {
        private readonly ProjectRepo _projectRepo;
        private readonly CategoryRepo _categoryRepo;
        private readonly OrganizationRepo _organRepo;
        private readonly CloudinaryService _imageStorageService;
        private readonly CertificateRepo _certificateRepo;
        private readonly CalculateScore _calculateScore;

        public ProjectService(ProjectRepo projectRepo, CategoryRepo categoryRepo, OrganizationRepo organRepo, 
            CloudinaryService imageStorageService, CertificateRepo certificateRepo, CalculateScore calculateScore)
        {
            _projectRepo = projectRepo;
            _categoryRepo = categoryRepo;
            _organRepo = organRepo;
            _imageStorageService = imageStorageService;
            _certificateRepo = certificateRepo;
            _calculateScore = calculateScore;
        }

        public async Task<List<ViewProjectDto>> GetAllProjects()
        {
            var projects = await _projectRepo.GettAll();

            return projects.Select(project => MapToViewProjectDto(project)).ToList();
        }

        public async Task<ViewProjectDto?> GetProjectById(int id)
        {
            var project = await _projectRepo.GetByIdAsync(id);
            return project == null ? null : MapToViewProjectDto(project);
        }

        public async Task<(bool Success, string Message, int? ProjectId)> CreateProject(CreateProjectDto dto, IFormFile? ImageFile)
        {
            try
            {
                var organization = await _organRepo.GetByIdAsync(dto.OrganizationId);
                if (organization == null || !organization.Status.Equals(OrganzationStatus.Active))
                    return (false, "Organization not found or inactive.", null);

                if (dto.CategoryIds == null || !dto.CategoryIds.Any())
                    return (false, "At least one category is required.", null);

                var categoriesExist = await _categoryRepo.CategoriesExistAsync(dto.CategoryIds);
                if (!categoriesExist)
                    return (false, "One or more categories do not exist or are inactive.", null);

                var project = new Project
                {
                    Title = dto.Title,
                    Description = dto.Description,
                    Type = dto.Type,
                    StartDate = dto.StartDate,
                    EndDate = dto.EndDate,
                    Location = dto.Location,
                    OrganizationId = dto.OrganizationId,
                    RequiredVolunteers = dto.RequiredVolunteers,
                    Status = ProjectStatus.Draft,
                    CurrentVolunteers = 0,
                    CreatedAt = DateTime.UtcNow
                }; 

                if (ImageFile != null && ImageFile.Length > 0)
                {
                    project.ImageUrl = await _imageStorageService.UploadImageAsync(ImageFile);
                }

                foreach (var categoryId in dto.CategoryIds.Distinct())
                {
                    project.Categories.Add(new ProjectCategory
                    {
                        Project = project,
                        CategoryId = categoryId
                    });
                }

                await _projectRepo.AddAsync(project);
                return (true, "Project created successfully.", project.Id);
            }
            catch (Exception ex)
            {
                return (false, $"Error: {ex.Message}", null);
            }
        }

        public async Task<(bool Success, string Message)> UpdateProject(int id, UpdateProjectDto dto, IFormFile? newImageFile)
        {
            try
            {
                var project = await _projectRepo.GetByIdAsync(id);
                if (project == null)
                    return (false, "Project not found.");

                if (dto.CategoryIds == null || !dto.CategoryIds.Any())
                    return (false, "At least one category is required.");

                var categoriesExist = await _categoryRepo.CategoriesExistAsync(dto.CategoryIds);
                if (!categoriesExist)
                    return (false, "One or more categories do not exist or are inactive.");

                project.Title = dto.Title;
                project.Description = dto.Description;
                project.Type = dto.Type;
                project.StartDate = dto.StartDate;
                project.EndDate = dto.EndDate;
                project.Location = dto.Location;
                project.OrganizationId = dto.OrganizationId;
                project.RequiredVolunteers = dto.RequiredVolunteers;
                project.Status = dto.Status;
                project.UpdatedAt = DateTime.UtcNow;

                project.Categories.Clear();
                foreach (var categoryId in dto.CategoryIds.Distinct())
                {
                    project.Categories.Add(new ProjectCategory
                    {
                        ProjectId = project.Id,
                        CategoryId = categoryId
                    });
                }

                if (newImageFile != null && newImageFile.Length > 0)
                {
                    project.ImageUrl = await _imageStorageService.UpdateImageAsync(project.ImageUrl, newImageFile);
                }

                await _projectRepo.UpdateAsync(project);
                return (true, "Project updated successfully.");
            }
            catch (Exception ex)
            {
                return (false, $"Error: {ex.Message}");
            }
        }

        public async Task<(bool Success, string Message)> AddCategoryToProject(int projectId, int categoryId)
        {
            try
            {
                var project = await _projectRepo.GetByIdAsync(projectId);
                if (project == null)
                    return (false, "Project not found.");

                var category = await _categoryRepo.GetByIdAsync(categoryId); 
                if (category == null || !category.IsActive)
                    return (false, "Category not found or inactive.");

                if (project.Categories.Any(pc => pc.CategoryId == categoryId))
                    return (false, "Category already added to project.");

                project.Categories.Add(new ProjectCategory
                {
                    ProjectId = projectId,
                    CategoryId = categoryId
                });

                await _projectRepo.UpdateAsync(project);
                return (true, "Category added to project successfully.");
            }
            catch (Exception ex)
            {
                return (false, $"Error adding category: {ex.Message}");
            }
        }

        public async Task<List<ViewProjectDto>> GetProjectsByFilter(ProjectFilterDto dto)
        {
            var projects = await _projectRepo.GetByFilterAsync(dto);
            return projects.Select(project => MapToViewProjectDto(project)).ToList();
        }

        public async Task<(bool Success, string Message)> RemoveCategoryFromProject(int projectId, int categoryId)
        {
            var project = await _projectRepo.GetByIdAsync(projectId);
            if (project == null)
                return (false, "Project not found.");

            var projectCategory = project.Categories.FirstOrDefault(pc => pc.CategoryId == categoryId);
            if (projectCategory == null)
                return (false, "Category not found in project.");

            project.Categories.Remove(projectCategory);
            await _projectRepo.UpdateAsync(project);
            return (true, "Category removed from project successfully.");
        }

        public async Task<(bool Success, string Message)> DeleteProject(int id)
        {
            var project = await _projectRepo.GetByIdAsync(id);
            if (project == null)
                return (false, "Project does not exist!");

            if (!string.IsNullOrEmpty(project.ImageUrl))
            {
                await _imageStorageService.DeleteImageAsync(project.ImageUrl);
            }

            await _projectRepo.DeleteAsync(project);
            return (true, "Project deleted successfully.");
        }

        public async Task<List<ViewMatchedProjectDto>> GetMatchedProject(int accountId, string? location)
        {
            var certificates = await _certificateRepo.GetByAccountIdAsync(accountId);

            if (!certificates.Any())
                return new List<ViewMatchedProjectDto>(); 

            var certificateCategoryIds = certificates
                .Select(c => c.CategoryId)
                .Distinct()
                .ToList();

            var filter = new ProjectFilterDto
            {
                Status = ProjectStatus.Recruiting, 
                CategoryIds = certificateCategoryIds 
            };

            if (!string.IsNullOrEmpty(location))
            {
                filter.Location = location;
            }

            var projects = await _projectRepo.GetByFilterAsync(filter);

            var matchedProjects = projects
                .Select(project => new
                {
                    Project = project,
                    MatchScore = _calculateScore.CalculateMatchScore(project, certificates, certificateCategoryIds),
                    MatchingCategories = _calculateScore.GetMatchingCategories(project, certificateCategoryIds)
                })
                .Where(result => result.MatchScore > 0) 
                .OrderByDescending(result => result.MatchScore) 
                .ToList();

            var result = matchedProjects.Select(mp => new ViewMatchedProjectDto
            {
                Id = mp.Project.Id,
                Title = mp.Project.Title,
                Description = mp.Project.Description,
                Type = mp.Project.Type,
                TypeName = mp.Project.Type.ToFriendlyName(), 
                StartDate = mp.Project.StartDate,
                EndDate = mp.Project.EndDate,
                Location = mp.Project.Location,
                ImageUrl = mp.Project.ImageUrl,
                OrganizationId = mp.Project.OrganizationId,
                OrganizationName = mp.Project.Organization?.Name,
                Status = mp.Project.Status,
                StatusName = mp.Project.Status.ToFriendlyName(), 
                RequiredVolunteers = mp.Project.RequiredVolunteers,
                CurrentVolunteers = mp.Project.CurrentVolunteers,
                CreatedAt = mp.Project.CreatedAt,
                UpdatedAt = mp.Project.UpdatedAt,

                Categories = mp.Project.Categories?.Select(pc => new ProjectCategoryDto
                {
                    CategoryId = pc.CategoryId,
                    CategoryName = pc.Category?.Name ?? "Unknown",
                    CategoryIcon = pc.Category?.Icon,
                    CategoryColor = pc.Category?.Color
                }).ToList() ?? new List<ProjectCategoryDto>(),


                MatchPercentage = mp.MatchScore,
                MatchingSkillCount = mp.MatchingCategories.Count,
                MatchingSkills = mp.MatchingCategories,
                MatchExplanation = _calculateScore.GenerateMatchExplanation(mp.Project, mp.MatchingCategories, mp.MatchScore)
            }).ToList();

            return result;
        }

        private ViewProjectDto MapToViewProjectDto(Project project)
        {
            return new ViewProjectDto
            {
                Id = project.Id,
                Title = project.Title,
                Description = project.Description,
                Type = project.Type,
                TypeName = project.Type.ToFriendlyName(),
                StartDate = project.StartDate,
                EndDate = project.EndDate,
                Location = project.Location,
                ImageUrl = project.ImageUrl,
                OrganizationId = project.OrganizationId,
                OrganizationName = project.Organization?.Name,
                Status = project.Status,
                StatusName = project.Status.ToFriendlyName(),
                RequiredVolunteers = project.RequiredVolunteers,
                CurrentVolunteers = project.CurrentVolunteers,
                CreatedAt = project.CreatedAt,
                UpdatedAt = project.UpdatedAt,
                Categories = project.Categories?.Select(pc => new ProjectCategoryDto
                {
                    CategoryId = pc.CategoryId,
                    CategoryName = pc.Category?.Name ?? "Unknown",
                    CategoryIcon = pc.Category?.Icon,
                    CategoryColor = pc.Category?.Color
                }).ToList() ?? new List<ProjectCategoryDto>(),
            };
        }
    }
}