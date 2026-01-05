using Microsoft.EntityFrameworkCore;
using Together.Models;

namespace Together.Repositories
{
    public class HourRepo : BaseRepo<VolunteerHour>
    {
        public HourRepo(TogetherDbContext context) : base(context) { }

        private IQueryable<VolunteerHour> WithIncludes()
        {
            return _dbSet
                .Include(vh => vh.VolunteerApplication)
                    .ThenInclude(va => va.Volunteer);
        }

        public async Task<List<VolunteerHour>> GetAll()
        {
            return await WithIncludes()
                .OrderBy(s => s.RecordId)
                .ToListAsync();
        }

        public async Task<VolunteerHour?> GetByIdAsync(int id)
        {
            return await WithIncludes()
                .FirstOrDefaultAsync(s => s.RecordId == id);
        }

        public async Task<VolunteerHour?> GetByVolunteerApplicationId(int volunteerApplicationId)
        {
            return await WithIncludes()
                .FirstOrDefaultAsync(s => s.VolunteerApplicationId == volunteerApplicationId);
        }

        public async Task<List<VolunteerHour>> GetByProjectId(int projectId)
        {
            return await WithIncludes()
                .Where(s => s.VolunteerApplication.ProjectId == projectId)
                .ToListAsync();
        }

        public async Task<VolunteerHour> UpdateWithTransactionAsync(VolunteerHour hourRecord, Account account, Action<Account> updateAccountAction)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                _context.VolunteerHours.Update(hourRecord);

                updateAccountAction?.Invoke(account);
                _context.Accounts.Update(account);

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return hourRecord;
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<VolunteerHour?> GetActiveRecordAsync(int applicationId, DateTime today)
        {
            return await _dbSet.FirstOrDefaultAsync(vh =>
                vh.VolunteerApplicationId == applicationId &&
                vh.CheckIn.HasValue &&
                vh.CheckIn.Value.Date == today.Date &&
                !vh.CheckOut.HasValue);
        }
    }
}