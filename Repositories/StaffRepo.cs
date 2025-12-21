using Microsoft.EntityFrameworkCore;
using Together.Models;

namespace Together.Repositories
{
    public class StaffRepo : BaseRepo<Staff>
    {
        public StaffRepo(TogetherDbContext context) : base(context) { }

        public async Task<List<Staff>> GettAll()
        {
            return await _dbSet
                .Include(s => s.Account) 
                .Include(s => s.Organization) 
                .ToListAsync();
        }

        public async Task<Staff?> GetByIdAsync(int id)
        {
            return await _dbSet
                .Include(s => s.Account)
                .Include(s => s.Organization)
                .FirstOrDefaultAsync(s => s.Id == id);
        }

        public async Task<List<Staff>> GetStaffByOrganId(int organId)
        {
            return await _dbSet
                .Where(s => s.OrganizationId == organId)
                .Include(s => s.Account)  
                .Include(s => s.Organization)  
                .ToListAsync();
        }
    }
}
