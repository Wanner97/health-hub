using Common.Dtos.DataImportDtos;
using Common.Models;
using Logic.Import.Mappers;
using System.Text.Json;

namespace Logic.Import.Helpers
{
    public static class HealthImportHelper
    {
        public static HealthExportDto DeserializeHealthExport(Stream stream)
        {
            using (var reader = new StreamReader(stream))
            {
                var json = reader.ReadToEnd();

                var dto = JsonSerializer.Deserialize<HealthExportDto>(
                    json,
                    new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });

                if (dto == null)
                {
                    throw new InvalidOperationException("The JSON file could not be deserialized.");
                }

                return dto;
            }
        }

        public static HealthImportData BuildHealthImportData(HealthExportDto dto, DateTime importedAtUtc)
        {
            var importedActivityDays = ActivityImportMapper.MapToActivityDays(
                dto.Source,
                dto.Clusters.Activity);

            var consolidatedSleepSessionDtos = SleepSessionConsolidationHelper.ConsolidateSleepSessions(
                dto.Clusters.Sleep?.Sessions ?? new List<SleepSessionDto>());

            var importedSleepSessions = SleepImportMapper.MapToSleepSessions(
                dto.Source,
                importedAtUtc,
                consolidatedSleepSessionDtos);

            var importedHeartRateDays = HeartRateImportMapper.MapToHeartRateDays(
                dto.Source,
                importedAtUtc,
                dto.Clusters.Vitals?.HeartRateDaily,
                dto.Clusters.Vitals?.HeartRateHourly);

            var importedBloodOxygenDays = BloodOxygenImportMapper.MapToBloodOxygenDays(
                dto.Source,
                importedAtUtc,
                dto.Clusters.Vitals?.BloodOxygenDaily);

            var importedHeightMeasurements = BodyImportMapper.MapToHeightMeasurements(
                dto.Source,
                importedAtUtc,
                dto.Clusters.Body);

            var importedWeightMeasurements = BodyImportMapper.MapToWeightMeasurements(
                dto.Source,
                importedAtUtc,
                dto.Clusters.Body);

            var importBatch = ImportBatchImportMapper.MapToImportBatch(
                dto,
                importedAtUtc,
                importedActivityDays,
                importedSleepSessions,
                importedHeartRateDays,
                importedBloodOxygenDays,
                importedHeightMeasurements,
                importedWeightMeasurements);

            return new HealthImportData
            {
                ExportDto = dto,
                ImportBatch = importBatch,
                ActivityDays = importedActivityDays,
                SleepSessions = importedSleepSessions,
                HeartRateDays = importedHeartRateDays,
                BloodOxygenDays = importedBloodOxygenDays,
                HeightMeasurements = importedHeightMeasurements,
                WeightMeasurements = importedWeightMeasurements
            };
        }

        public static void ClearImportBatchNavigationEntries(ImportBatch importBatch)
        {
            importBatch.ActivityDayEntries = new List<ActivityDay>();
            importBatch.SleepSessionEntries = new List<SleepSession>();
            importBatch.HeartRateDayEntries = new List<HeartRateDay>();
            importBatch.BloodOxygenDayEntries = new List<BloodOxygenDay>();
            importBatch.HeightMeasurementEntries = new List<HeightMeasurement>();
            importBatch.WeightMeasurementEntries = new List<WeightMeasurement>();
        }
    }
}