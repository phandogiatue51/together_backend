using Microsoft.EntityFrameworkCore;
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
                .Include(c => c.VerifiedByAdmin)
                .OrderByDescending(c => c.CreatedAt)
                .ToListAsync();
        }

        public async Task<Certificate?> GetByIdAsync(int id)
        {
            return await _dbSet
                .Include(c => c.Account)
                .Include(c => c.Category)
                .Include(c => c.VerifiedByAdmin)
                .FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task<List<Certificate>> GetByAccountIdAsync(int accountId)
        {
            return await _dbSet
                .Include(c => c.Category)
                .Include(c => c.VerifiedByAdmin)
                .Where(c => c.AccountId == accountId)
                .OrderByDescending(c => c.CreatedAt)
                .ToListAsync();
        }

        public async Task<List<Certificate>> GetByCategoryIdAsync(int categoryId)
        {
            return await _dbSet
                .Include(c => c.Account)
                .Where(c => c.CategoryId == categoryId && c.Status == CertificateStatus.Verified)
                .ToListAsync();
        }

        public async Task<List<Certificate>> GetPendingCertificatesAsync()
        {
            return await _dbSet
                .Include(c => c.Account)
                .Include(c => c.Category)
                .Where(c => c.Status == CertificateStatus.Pending)
                .OrderBy(c => c.CreatedAt)
                .ToListAsync();
        }
    }
}