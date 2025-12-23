using Microsoft.EntityFrameworkCore;
using Together.DTOs.Staf;
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

        public async Task<Staff?> GetByAccountId(int accountId)
        {
            return await WithIncludes()
                .FirstOrDefaultAsync(s => s.AccountId == accountId);
        }

        public async Task<List<Staff>> GetByFilterAsync(StaffFilterDto dto)
        {
            IQueryable<Staff> query = WithIncludes();

            if (!string.IsNullOrEmpty(dto.Name))
                query = query.Where(s => s.Account.Name.Contains(dto.Name));

            if (!string.IsNullOrEmpty(dto.Email))
                query = query.Where(s => s.Account.Email.Contains(dto.Email));

            if (!string.IsNullOrEmpty(dto.PhoneNumber))
                query = query.Where(s => s.Account.PhoneNumber.Contains(dto.PhoneNumber));

            if (dto.OrganizationId.HasValue)
                query = query.Where(s => s.OrganizationId == dto.OrganizationId.Value);

            if (dto.Role.HasValue)
                query = query.Where(s => s.Role == dto.Role.Value);

            if (dto.JoinedAt.HasValue)
                query = query.Where(s => s.JoinedAt.Date == dto.JoinedAt.Value.Date);

            if (dto.LeftAt.HasValue)
                query = query.Where(s => s.LeftAt.HasValue && s.LeftAt.Value.Date == dto.LeftAt.Value.Date);

            if (dto.IsActive.HasValue)
                query = query.Where(s => s.IsActive == dto.IsActive.Value);

            return await query.ToListAsync();
        }
    }
}
