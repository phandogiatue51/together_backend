using Microsoft.EntityFrameworkCore;
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
    }
}