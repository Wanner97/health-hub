using Common.Models;
using DataAccess.Context;
using DataAccess.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DataAccess
{
    public class NutritionDayDataAccess : INutritionDayDataAccess
    {
        private readonly IDbContextFactory<AppDbContext> _dbContextFactory;

        public NutritionDayDataAccess(IDbContextFactory<AppDbContext> dbContextFactory)
        {
            _dbContextFactory = dbContextFactory;
        }

        public List<NutritionDay> GetNutritionDays(DateOnly? from, DateOnly? to)
        {
            using (var context = _dbContextFactory.CreateDbContext())
            {
                var query = context.NutritionDays
                    .AsNoTracking()
                    .AsSplitQuery()
                    .Include(x => x.NutrientTotals)
                    .Include(x => x.Records)
                    .ThenInclude(x => x.Nutrients)
                    .AsQueryable();

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