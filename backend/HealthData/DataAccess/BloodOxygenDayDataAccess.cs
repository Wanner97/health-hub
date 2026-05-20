using Common.Models;
using DataAccess.Context;
using DataAccess.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DataAccess
{
    public class BloodOxygenDayDataAccess : IBloodOxygenDayDataAccess
    {
        private readonly IDbContextFactory<AppDbContext> _dbContextFactory;

        public BloodOxygenDayDataAccess(IDbContextFactory<AppDbContext> dbContextFactory)
        {
            _dbContextFactory = dbContextFactory;
        }

        public List<BloodOxygenDay> GetBloodOxygenDays(DateOnly? from, DateOnly? to)
        {
            using (var context = _dbContextFactory.CreateDbContext())
            {
                var query = context.BloodOxygenDays.AsNoTracking().AsQueryable();

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

        public BloodOxygenDay? GetLatestBloodOxygenDay()
        {
            using (var context = _dbContextFactory.CreateDbContext())
            {
                return context.BloodOxygenDays
                    .OrderByDescending(x => x.Date)
                    .FirstOrDefault();
            }
        }
    }
}