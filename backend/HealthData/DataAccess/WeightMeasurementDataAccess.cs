using Common.Models;
using DataAccess.Context;
using DataAccess.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DataAccess
{
    public class WeightMeasurementDataAccess : IWeightMeasurementDataAccess
    {
        private readonly IDbContextFactory<AppDbContext> _dbContextFactory;

        public WeightMeasurementDataAccess(IDbContextFactory<AppDbContext> dbContextFactory)
        {
            _dbContextFactory = dbContextFactory;
        }

        public List<WeightMeasurement> GetWeightMeasurements(DateTime? fromMeasuredAtUtc, DateTime? toMeasuredAtUtc)
        {
            using (var context = _dbContextFactory.CreateDbContext())
            {
                var query = context.WeightMeasurements.AsNoTracking().AsQueryable();

                if (fromMeasuredAtUtc.HasValue)
                {
                    query = query.Where(x => x.MeasuredAtUtc >= fromMeasuredAtUtc.Value);
                }

                if (toMeasuredAtUtc.HasValue)
                {
                    query = query.Where(x => x.MeasuredAtUtc <= toMeasuredAtUtc.Value);
                }

                return query
                    .OrderBy(x => x.MeasuredAtUtc)
                    .ToList();
            }
        }

        public WeightMeasurement? GetLatestWeightMeasurement()
        {
            using (var context = _dbContextFactory.CreateDbContext())
            {
                return context.WeightMeasurements
                    .OrderByDescending(x => x.MeasuredAtUtc)
                    .FirstOrDefault();
            }
        }
    }
}