using Microsoft.EntityFrameworkCore;
using Together.DTOs.Organ;
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

        public async Task<List<Organization>> GetByFilterAsync(OrganFilterDto dto)
        {
            var query = _dbSet.AsQueryable();

            if (!string.IsNullOrEmpty(dto.Name))
            {
                var name = dto.Name.ToLower();
                query = query.Where(o => o.Name.ToLower().Contains(name));
            }
            if (!string.IsNullOrEmpty(dto.Address))
            {
                var address = dto.Address.ToLower();
                query = query.Where(o => o.Address.ToLower().Contains(address));
            }
            if (!string.IsNullOrEmpty(dto.Email))
            {
                var email = dto.Email.ToLower();
                query = query.Where(o => o.Email.ToLower().Contains(email));
            }
            if (!string.IsNullOrEmpty(dto.PhoneNumber))
            {
                var phone = dto.PhoneNumber.ToLower();
                query = query.Where(o => o.PhoneNumber.ToLower().Contains(phone));
            }
            if (dto.Status.HasValue)
            {
                query = query.Where(o => o.Status == dto.Status.Value);
            }
            if (dto.Type.HasValue)
            {
                query = query.Where(o => o.Type == dto.Type.Value);
            }
            
            return await query
                .OrderBy(o => o.Id)
                .ToListAsync();
        }
    }    
}