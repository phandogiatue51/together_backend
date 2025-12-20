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
                .Include(s => s.Organization)
                .ToListAsync();
        }

        public async Task<Project?> GetByIdAsync(int id)
        {
            return await _dbSet
                 .Include(s => s.Organization)
                .FirstOrDefaultAsync(s => s.Id == id);
        }
    }
}
