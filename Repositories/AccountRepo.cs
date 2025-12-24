using Microsoft.EntityFrameworkCore;
using Together.DTOs.App;
using Together.DTOs.User;
using Together.Models;
using Together.Repositories;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace Together.Repositories
{
    public class AccountRepo : BaseRepo<Account>
    {
        public AccountRepo(TogetherDbContext context) : base(context) { }

        public async Task<List<Account>> GettAll()
        {
            return await _dbSet
                .OrderBy(a => a.Id)
                .ToListAsync();
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

        public async Task<List<Account>> GetByFilterAsync(UserFilterDto filter)
        {
            IQueryable<Account> query = _dbSet;   

            if (filter.Role.HasValue)
            {
                query = query.Where(a => a.Role == filter.Role.Value);
            }
            if (filter.Status.HasValue)
            {
                query = query.Where(a => a.Status == filter.Status.Value);
            }
            if (!string.IsNullOrEmpty(filter.Name))
            {
                var name = filter.Name.ToLower();
                query = query.Where(a => a.Name.ToLower().Contains(name));
            }
            if (!string.IsNullOrEmpty(filter.Email))
            {
                query = query.Where(a => a.Email.Contains(filter.Email));
            }

            return await query.ToListAsync();
        }
    }
}