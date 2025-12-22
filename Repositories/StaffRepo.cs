using Microsoft.EntityFrameworkCore;
using Together.Models;

namespace Together.Repositories
{
    public class StaffRepo : BaseRepo<Staff>
    {
        public StaffRepo(TogetherDbContext context) : base(context) { }

        private IQueryable<Staff> WithIncludes()
        {
            return _dbSet
                .Include(s => s.Account)
                .Include(s => s.Organization);
        }

        public async Task<List<Staff>> GettAll()
        {
            return await WithIncludes()
                .ToListAsync();
        }

        public async Task<Staff?> GetByIdAsync(int id)
        {
            return await WithIncludes()
                .FirstOrDefaultAsync(s => s.Id == id);
        }

        public async Task<List<Staff>> GetStaffByOrganId(int organId)
        {
            return await WithIncludes()
                .Where(s => s.OrganizationId == organId) 
                .ToListAsync();
        }
    }
}
