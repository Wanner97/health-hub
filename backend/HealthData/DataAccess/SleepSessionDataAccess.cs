using Common.Models;
using DataAccess.Context;
using DataAccess.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DataAccess
{
    public class SleepSessionDataAccess : ISleepSessionDataAccess
    {
        private readonly IDbContextFactory<AppDbContext> _dbContextFactory;

        public SleepSessionDataAccess(IDbContextFactory<AppDbContext> dbContextFactory)
        {
            _dbContextFactory = dbContextFactory;
        }

        public List<SleepSession> GetSleepSessions(DateOnly? from, DateOnly? to)
        {
            using (var context = _dbContextFactory.CreateDbContext())
            {
                var query = context.SleepSessions
                    .Include(x => x.SleepStages)
                    .AsQueryable();

                if (from.HasValue)
                {
                    var fromDateTimeUtc = from.Value.ToDateTime(TimeOnly.MinValue);

                    query = query.Where(x => x.EndTimeUtc >= fromDateTimeUtc);
                }

                if (to.HasValue)
                {
                    var toDateTimeUtc = to.Value.ToDateTime(TimeOnly.MaxValue);

                    query = query.Where(x => x.StartTimeUtc <= toDateTimeUtc);
                }

                return query
                    .OrderBy(x => x.StartTimeUtc)
                    .ToList();
            }
        }

        public SleepSession? GetLatestSleepSession()
        {
            using (var context = _dbContextFactory.CreateDbContext())
            {
                return context.SleepSessions
                    .OrderByDescending(x => x.EndTimeUtc)
                    .FirstOrDefault();
            }
        }
    }
}