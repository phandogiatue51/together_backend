using Microsoft.EntityFrameworkCore;
using Together.Models;

namespace Together.Repositories
{
    public class OrganRepo : BaseRepo<Organization>
    {
        public OrganRepo(TogetherDbContext context) : base(context) { }

        public async Task<List<Organization>> GettAll()
        {
            return await _dbSet.ToListAsync();
        }

        public async Task<Organization?> GetByIdAsync(int id)
        {
            return await _dbSet.FindAsync(id);
        }

        public async Task<Organization?> GetByIdWithStaffAsync(int id)
        {
            return await _dbSet
                .Include(o => o.Staffs)
                .Include(o => o.Projects)
                .FirstOrDefaultAsync(o => o.Id == id);
        }

        public async Task<List<Organization>> GetActiveOrganizationsAsync()
        {
            return await _dbSet
                .Where(o => o.IsActive)
                .OrderBy(o => o.Name)
                .ToListAsync();
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