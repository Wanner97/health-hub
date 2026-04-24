using Common.Dtos.DataImportDtos;
using Common.Models;

namespace Logic.Import.Mappers
{
    public static class BloodOxygenImportMapper
    {
        public static List<BloodOxygenDay> MapToBloodOxygenDays(
            string source,
            DateTime importedAtUtc,
            BloodOxygenDailyClusterDto? bloodOxygenDailyCluster)
        {
            if (bloodOxygenDailyCluster?.Records == null || bloodOxygenDailyCluster.Records.Count == 0)
            {
                return new List<BloodOxygenDay>();
            }

            return bloodOxygenDailyCluster.Records
                .Select(x => new BloodOxygenDay
                {
                    Source = source,
                    Date = x.Date,
                    StartTimeUtc = x.StartTime.UtcDateTime,
                    EndTimeUtc = x.EndTime.UtcDateTime,
                    AvgPercent = x.AvgPercent,
                    MinPercent = x.MinPercent,
                    MaxPercent = x.MaxPercent,
                    MeasurementCount = x.MeasurementCount,
                    LastImportedAtUtc = importedAtUtc
                })
                .OrderBy(x => x.Date)
                .ToList();
        }
    }
}