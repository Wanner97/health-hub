using Common.Dtos.DataImportDtos;
using Common.Models;

namespace Logic.Import.Mappers
{
    public static class BodyImportMapper
    {
        public static List<HeightMeasurement> MapToHeightMeasurements(
            string source,
            DateTime importedAtUtc,
            BodyClusterDto? bodyCluster)
        {
            if (bodyCluster?.LatestHeight == null)
            {
                return new List<HeightMeasurement>();
            }

            return new List<HeightMeasurement>
            {
                new HeightMeasurement
                {
                    Source = source,
                    HeightCm = bodyCluster.LatestHeight.HeightCm,
                    MeasuredAtUtc = bodyCluster.LatestHeight.MeasuredAt.UtcDateTime,
                    LastImportedAtUtc = importedAtUtc
                }
            };
        }

        public static List<WeightMeasurement> MapToWeightMeasurements(
            string source,
            DateTime importedAtUtc,
            BodyClusterDto? bodyCluster)
        {
            var weightRecordDtos = bodyCluster?.WeightRecords ?? new List<WeightRecordDto>();

            if (weightRecordDtos.Count == 0)
            {
                return new List<WeightMeasurement>();
            }

            return weightRecordDtos
                .Select(x => new WeightMeasurement
                {
                    Source = source,
                    Date = x.Date,
                    WeightKg = x.WeightKg,
                    MeasuredAtUtc = x.MeasuredAt.UtcDateTime,
                    LastImportedAtUtc = importedAtUtc
                })
                .OrderBy(x => x.MeasuredAtUtc)
                .ToList();
        }
    }
}