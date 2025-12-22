using Microsoft.EntityFrameworkCore;
using Together.Models;

namespace Together.Repositories
{
    public class ProjectRepo : BaseRepo<Project>
    {
        public ProjectRepo(TogetherDbContext context) : base(context) { }

        private IQueryable<Project> WithIncludes()
        {
            return _dbSet
                .Include(p => p.Organization)
                .Include(p => p.Categories)
                    .ThenInclude(pc => pc.Category);
        }

        public async Task<List<Project>> GettAll()
        {
            return await WithIncludes()
                .OrderBy(p => p.Id)
                .ToListAsync();
        }

        public async Task<Project?> GetByIdAsync(int id)
        {
            return await WithIncludes()
                .FirstOrDefaultAsync(p => p.Id == id);
        }
    }
}