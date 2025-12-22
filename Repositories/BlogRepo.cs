using Microsoft.EntityFrameworkCore;
using Together.Models;

namespace Together.Repositories
{
    public class BlogRepo : BaseRepo<BlogPost>
    {
        public BlogRepo(TogetherDbContext context) : base(context) { }

        private IQueryable<BlogPost> WithIncludes()
        {
            return _dbSet
                .Include(s => s.Organization)
                .Include(s => s.Author);
        }

        public async Task<List<BlogPost>> GettAll()
        {
            return await WithIncludes()
                .OrderBy(s => s.Id)
                .ToListAsync();
        }

        public async Task<BlogPost?> GetByIdAsync(int id)
        {
            return await WithIncludes()
                .FirstOrDefaultAsync(s => s.Id == id);
        }
    }
}
