using Common.Models;
using DataAccess.Context;
using DataAccess.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DataAccess
{
    public class ActivityDayDataAccess : IActivityDayDataAccess
    {
        private readonly IDbContextFactory<AppDbContext> _dbContextFactory;

        public ActivityDayDataAccess(IDbContextFactory<AppDbContext> dbContextFactory)
        {
            _dbContextFactory = dbContextFactory;
        }

        public List<ActivityDay> GetActivityDays(DateOnly? from, DateOnly? to)
        {
            using (var context = _dbContextFactory.CreateDbContext())
            {
                var query = context.ActivityDays.AsQueryable();

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
    }
}