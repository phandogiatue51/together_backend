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
                query = query.Where(o => o.Name.Contains(dto.Name));
            }
            if (!string.IsNullOrEmpty(dto.Address))
            {
                query = query.Where(o => o.Address.Contains(dto.Address));
            }
            if (!string.IsNullOrEmpty(dto.Email))
            {
                query = query.Where(o => o.Email.Contains(dto.Email));
            }
            if (!string.IsNullOrEmpty(dto.PhoneNumber))
            {
                query = query.Where(o => o.PhoneNumber.Contains(dto.PhoneNumber));
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