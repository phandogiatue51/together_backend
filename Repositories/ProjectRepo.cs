using Microsoft.EntityFrameworkCore;
using Together.DTOs.Pro;
using Together.Models;

namespace Together.Repositories
{
    public class ProjectRepo : BaseRepo<Project>
    {
        public ProjectRepo(TogetherDbContext context) : base(context) { }

        private IQueryable<Project> WithIncludes()
        {
            return _dbSet
                .Include(p => p.Organization)
                .Include(p => p.Categories)
                    .ThenInclude(pc => pc.Category);
        }

        public async Task<List<Project>> GettAll()
        {
            return await WithIncludes()
                .OrderBy(p => p.Id)
                .ToListAsync();
        }

        public async Task<Project?> GetByIdAsync(int id)
        {
            return await WithIncludes()
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<List<Project>> GetByFilterAsync(ProjectFilterDto dto)
        {
            var query = WithIncludes()
                .Include(p => p.Categories)
                    .ThenInclude(pc => pc.Category)
                .AsQueryable();

            if (!string.IsNullOrEmpty(dto.Title))
            {
                var title = dto.Title.ToLower();
                query = query.Where(p => p.Title.ToLower().Contains(title));
            }
            if (!string.IsNullOrEmpty(dto.Location))
            {
                var location = dto.Location.ToLower();
                query = query.Where(p => p.Location != null && p.Location.ToLower().Contains(location));
            }
            if (dto.StartDate.HasValue)
            {
                query = query.Where(p => p.StartDate >= dto.StartDate.Value);
            }
            if (dto.EndDate.HasValue)
            {
                query = query.Where(p => p.EndDate <= dto.EndDate.Value);
            }
            if (dto.RequiredVolunteers.HasValue)
            {
                query = query.Where(p => p.RequiredVolunteers >= dto.RequiredVolunteers.Value);
            }
            if (dto.CurrentVolunteers.HasValue)
            {
                query = query.Where(p => p.CurrentVolunteers >= dto.CurrentVolunteers.Value);
            }
            if (dto.Type.HasValue)
            {
                query = query.Where(p => p.Type == dto.Type.Value);
            }
            if (dto.Status.HasValue)
            {
                query = query.Where(p => p.Status == dto.Status.Value);
            }
            if (dto.CreatedAt.HasValue)
            {
                query = query.Where(p => p.CreatedAt.Date == dto.CreatedAt.Value.Date);
            }
            if (dto.OrganizationId.HasValue)
            {
                query = query.Where(p => p.OrganizationId == dto.OrganizationId.Value);
            }
            if (dto.CategoryIds != null && dto.CategoryIds.Any())
            {
                query = query.Where(p => p.Categories.Any(pc => dto.CategoryIds.Contains(pc.CategoryId)));

            }

            return await query.ToListAsync();
        }
    }
}