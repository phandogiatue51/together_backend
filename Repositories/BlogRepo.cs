using Microsoft.EntityFrameworkCore;
using Together.DTOs.Blog;
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

        public async Task<List<BlogPost>> GetByFilterAsync(BlogFilterDto dto)
        {
            IQueryable<BlogPost> query = WithIncludes();

            if (!string.IsNullOrEmpty(dto.Title))
            {
                query = query.Where(s => s.Title.Contains(dto.Title));
            }
            if (dto.AuthorId.HasValue)
            {
                query = query.Where(s => s.AuthorId == dto.AuthorId.Value);
            }
            if (dto.OrganizationId.HasValue)
            {
                query = query.Where(s => s.OrganizationId == dto.OrganizationId.Value);
            }
            if (dto.PublishDate.HasValue)
            {
                query = query.Where(s => s.PublishedDate.Date == dto.PublishDate.Value.Date);
            }
            if (dto.Status.HasValue)
            {
                query = query.Where(s => s.Status == dto.Status.Value);
            }
            return await query.ToListAsync();
        }
    }
}
