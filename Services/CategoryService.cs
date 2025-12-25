using Together.DTOs;
using Together.DTOs.Blog;
using Together.Repositories;

namespace Together.Services
{
    public class CategoryService
    {
        private readonly CategoryRepo _cateRepo;
        private readonly CertificateRepo _certificateRepo;
        private readonly ProjectRepo _projectRepo;

        public CategoryService(CategoryRepo cateRepo, CertificateRepo certificateRepo, ProjectRepo projectRepo)
        {
            _cateRepo = cateRepo;
            _certificateRepo = certificateRepo;
            _projectRepo = projectRepo;
        }

        public async Task<List<ViewCateDto>> GetAllCategoriesAsync()
        {
            var categories = await _cateRepo.GetActiveCategoriesAsync();
            return categories.Select(categories => new ViewCateDto
            {
                Id = categories.Id,
                Name = categories.Name,
                Code = categories.Code,
                Icon = categories.Icon,
                Color = categories.Color,
                IsActive = categories.IsActive,
                CreatedAt = categories.CreatedAt,
                UpdatedAt = categories.UpdatedAt
            }).ToList();
        }

        public async Task<List<CategoryPopularityDto>> GetSkillSuggestions(int accountId)
        {
            var volunteerCerts = await _certificateRepo.GetByAccountIdAsync(accountId);
            var volunteerCategoryIds = volunteerCerts.Select(c => c.CategoryId).Distinct().ToList();

            var popularCategories = await _cateRepo.GetPopularCategoriesWithStatsAsync(
                count: 3,
                excludeIds: volunteerCategoryIds
            );

            return popularCategories;
        }
    }
}
