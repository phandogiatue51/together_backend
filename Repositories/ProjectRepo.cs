using Microsoft.EntityFrameworkCore;
using Together.Models;

namespace Together.Repositories
{
    public class ProjectRepo : BaseRepo<Project>
    {
        public ProjectRepo(TogetherDbContext context) : base(context) { }

        public async Task<List<Project>> GettAll()
        {
            return await _dbSet
                .Include(p => p.Organization)
                .Include(p => p.Categories)
                    .ThenInclude(pc => pc.Category)
                .OrderBy(p => p.Id)
                .ToListAsync();
        }

        public async Task<Project?> GetByIdAsync(int id)
        {
            return await _dbSet
                .Include(p => p.Organization)
                .Include(p => p.Categories)
                    .ThenInclude(pc => pc.Category)
                .FirstOrDefaultAsync(p => p.Id == id);
        }
    }
}