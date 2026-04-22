using Common.Models;
using DataAccess.Context;
using DataAccess.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DataAccess
{
    public class HeartRateDayDataAccess : IHeartRateDayDataAccess
    {
        private readonly IDbContextFactory<AppDbContext> _dbContextFactory;

        public HeartRateDayDataAccess(IDbContextFactory<AppDbContext> dbContextFactory)
        {
            _dbContextFactory = dbContextFactory;
        }

        public List<HeartRateDay> GetHeartRateDays(DateOnly? from, DateOnly? to, bool includeHourlyRecords)
        {
            using (var context = _dbContextFactory.CreateDbContext())
            {
                IQueryable<HeartRateDay> query = context.HeartRateDays;

                if (includeHourlyRecords)
                {
                    query = query.Include(x => x.HourlyRecords);
                }

                if (from.HasValue)
                {
                    query = query.Where(x => x.Date >= from.Value);
                }

                if (to.HasValue)
                {
                    query = query.Where(x => x.Date <= to.Value);
                }

                return query
                    .OrderBy(x => x.Date)
                    .ToList();
            }
        }

        public HeartRateDay? GetLatestHeartRateDay()
        {
            using (var context = _dbContextFactory.CreateDbContext())
            {
                return context.HeartRateDays
                    .OrderByDescending(x => x.Date)
                    .FirstOrDefault();
            }
        }
    }
}