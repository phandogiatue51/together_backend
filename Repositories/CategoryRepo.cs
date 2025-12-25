using Microsoft.EntityFrameworkCore;
using Together.DTOs;
using Together.Models;

namespace Together.Repositories
{
    public class CategoryRepo : BaseRepo<Category>
    {
        public CategoryRepo(TogetherDbContext context) : base(context) { }

        public async Task<List<Category>> GetActiveCategoriesAsync()
        {
            return await _dbSet
                .Where(c => c.IsActive)
                .OrderBy(c => c.Name)
                .ToListAsync();
        }

        public async Task<Category?> GetByIdAsync(int id)
        {
            return await _dbSet
                .FirstOrDefaultAsync(c => c.Id == id && c.IsActive);
        }

        public async Task<bool> CategoriesExistAsync(List<int> categoryIds)
        {
            var existingCount = await _dbSet
                .Where(c => categoryIds.Contains(c.Id) && c.IsActive)
                .CountAsync();
            return existingCount == categoryIds.Count;
        }

        public async Task<List<CategoryPopularityDto>> GetPopularCategoriesWithStatsAsync(int count = 3, List<int>? excludeIds = null)
        {
            var query = _context.Set<ProjectCategory>()
                .Where(pc => pc.Project.Status == ProjectStatus.Recruiting && pc.Category.IsActive);

            if (excludeIds != null && excludeIds.Any())
            {
                query = query.Where(pc => !excludeIds.Contains(pc.CategoryId));
            }

            return await query
                .GroupBy(pc => new { pc.CategoryId, pc.Category.Name })
                .OrderByDescending(g => g.Count())
                .Take(count)
                .Select(g => new CategoryPopularityDto
                {
                    Id = g.Key.CategoryId,
                    Name = g.Key.Name,
                    ProjectCount = g.Count(),
                    LatestProject = g.OrderByDescending(pc => pc.Project.CreatedAt)
                                     .Select(pc => pc.Project.Title)
                                     .FirstOrDefault()
                })
                .ToListAsync();
        }
    }
}