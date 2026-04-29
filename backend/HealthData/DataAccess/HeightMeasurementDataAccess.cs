using Common.Models;
using DataAccess.Context;
using DataAccess.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DataAccess
{
    public class HeightMeasurementDataAccess : IHeightMeasurementDataAccess
    {
        private readonly IDbContextFactory<AppDbContext> _dbContextFactory;

        public HeightMeasurementDataAccess(IDbContextFactory<AppDbContext> dbContextFactory)
        {
            _dbContextFactory = dbContextFactory;
        }

        public List<HeightMeasurement> GetHeightMeasurements(DateTime? fromMeasuredAtUtc, DateTime? toMeasuredAtUtc)
        {
            using (var context = _dbContextFactory.CreateDbContext())
            {
                var query = context.HeightMeasurements.AsQueryable();

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

        public HeightMeasurement? GetLatestHeightMeasurement()
        {
            using (var context = _dbContextFactory.CreateDbContext())
            {
                return context.HeightMeasurements
                    .OrderByDescending(x => x.MeasuredAtUtc)
                    .FirstOrDefault();
            }
        }
    }
}