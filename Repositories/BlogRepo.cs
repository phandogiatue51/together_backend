using Microsoft.EntityFrameworkCore;
using Together.Models;

namespace Together.Repositories
{
    public class BlogRepo : BaseRepo<BlogPost>
    {
        public BlogRepo(TogetherDbContext context) : base(context) { }
        public async Task<List<BlogPost>> GettAll()
        {
            return await _dbSet
                .Include(s => s.Organization)
                .Include(s => s.Author)
                .ToListAsync();
        }

        public async Task<BlogPost?> GetByIdAsync(int id)
        {
            return await _dbSet
                .Include(s => s.Organization)
                .Include(s => s.Author)
                .FirstOrDefaultAsync(s => s.Id == id);
        }
    }
}
