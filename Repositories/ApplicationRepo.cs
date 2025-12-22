using Microsoft.EntityFrameworkCore;
using Together.DTOs.App;
using Together.Models;
using Together.Repositories;

public class ApplicationRepo : BaseRepo<VolunteerApplication>
{
    public ApplicationRepo(TogetherDbContext context) : base(context) { }

    private IQueryable<VolunteerApplication> WithIncludes()
    {
        return _dbSet
            .Include(a => a.Project)
            .Include(a => a.Volunteer)
            .Include(a => a.SelectedCertificates)
            .Include(a => a.ReviewedByStaff);
    }

    public async Task<List<VolunteerApplication>> GetAllAsync()
    {
        return await WithIncludes()
            .OrderByDescending(a => a.AppliedAt)
            .ToListAsync();
    }

    public async Task<VolunteerApplication?> GetByIdAsync(int id)
    {
        return await WithIncludes()
            .FirstOrDefaultAsync(a => a.Id == id);
    }

    public async Task<List<VolunteerApplication>> GetByFilterAsync(AppFilterDto filter)
    {
        var query = WithIncludes().AsQueryable();

        if (filter.ProjectId.HasValue)
            query = query.Where(a => a.ProjectId == filter.ProjectId.Value);

        if (filter.VolunteerId.HasValue)
            query = query.Where(a => a.VolunteerId == filter.VolunteerId.Value);

        if (filter.OrganizationId.HasValue)
            query = query.Where(a => a.Project.OrganizationId == filter.OrganizationId.Value);

        if (!string.IsNullOrEmpty(filter.Status))
        {
            if (Enum.TryParse<ApplicationStatus>(filter.Status, out var statusEnum))
                query = query.Where(a => a.Status == statusEnum);
        }

        return await query
            .OrderByDescending(a => a.AppliedAt)
            .ToListAsync();
    }

    public async Task<VolunteerApplication?> GetExistingApplicationAsync(int projectId, int volunteerId)
    {
        return await WithIncludes()
            .Where(a => a.ProjectId == projectId && a.VolunteerId == volunteerId)
            .FirstOrDefaultAsync();
    }
}