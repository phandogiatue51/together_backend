using Microsoft.EntityFrameworkCore;
using Together.Models;

namespace Together.Repositories
{
    public class OrganizationRepo : BaseRepo<Organization>
    {
        public OrganizationRepo(TogetherDbContext context) : base(context) { }

        public async Task<List<Organization>> GettAll()
        {
            return await _dbSet
                .OrderBy(s => s.Id)
                .ToListAsync();
        }

        public async Task<Organization?> GetByIdAsync(int id)
        {
            return await _dbSet
                .Include(o => o.Projects)
                .FirstOrDefaultAsync(o => o.Id == id);
        }

        public async Task<List<Organization>> SearchByNameAsync(string name)
        {
            return await _dbSet
                .Where(o => o.Name.Contains(name))
                .Take(10)
                .ToListAsync();
        }
    }    
}