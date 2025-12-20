using Microsoft.EntityFrameworkCore;
using Together.Models;
using Together.Repositories;

namespace Together.Repositories
{
    public class AccountRepo : BaseRepo<Account>
    {
        public AccountRepo(TogetherDbContext context) : base(context) { }

        public async Task<List<Account>> GettAll()
        {
            return await _dbSet.ToListAsync();
        }

        public async Task<Account?> GetByIdAsync(int id)
        {
            return await _dbSet.FindAsync(id);
        }

        public async Task<Account?> GetByEmailAsync(string email)
        {
            return await _dbSet
                .FirstOrDefaultAsync(a => a.Email == email);
        }

        public async Task<List<Account>> GetByRoleAsync(AccountRole role)
        {
            return await _dbSet
                .Where(a => a.Role == role)
                .ToListAsync();
        }
    }
}