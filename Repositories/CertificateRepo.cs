using Microsoft.EntityFrameworkCore;
using Together.DTOs.Certi;
using Together.Models;

namespace Together.Repositories
{
    public class CertificateRepo : BaseRepo<Certificate>
    {
        public CertificateRepo(TogetherDbContext context) : base(context) { }

        public async Task<List<Certificate>> GetAllAsync()
        {
            return await _dbSet
                .Include(c => c.Account)
                .Include(c => c.Category)
                .OrderByDescending(c => c.CreatedAt)
                .ToListAsync();
        }

        public async Task<Certificate?> GetByIdAsync(int id)
        {
            return await _dbSet
                .Include(c => c.Account)
                .Include(c => c.Category)
                .FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task<List<Certificate>> GetByAccountIdAsync(int accountId)
        {
            return await _dbSet
                .Include(c => c.Category)
                .Where(c => c.AccountId == accountId)
                .OrderByDescending(c => c.CreatedAt)
                .ToListAsync();
        }

        public async Task<List<Certificate>> GetCertificatesByIdsAsync(List<int> ids)
        {
            var query = _dbSet.Where(c => ids.Contains(c.Id));

            return await query.ToListAsync();
        }

        public async Task<List<Certificate>> GetFilteredAsync(CertiFilterDto filterDto)
        {
            var query = _context.Certificates.AsQueryable();

            if (filterDto.AccountId.HasValue)
                query = query.Where(c => c.AccountId == filterDto.AccountId.Value);
            if (filterDto.CategoryId.HasValue)
                query = query.Where(c => c.CategoryId == filterDto.CategoryId.Value);
            if (!string.IsNullOrEmpty(filterDto.CertificateName))
            {
                var nameFilter = filterDto.CertificateName.Trim().ToLower();
                query = query.Where(c => c.CertificateName.ToLower().Contains(nameFilter));
            }

            return await query.ToListAsync();
        }
    }
}