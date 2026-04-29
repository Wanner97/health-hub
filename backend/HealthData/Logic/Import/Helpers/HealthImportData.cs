using Common.Dtos.DataImportDtos;
using Common.Models;

namespace Logic.Import.Helpers
{
    public class HealthImportData
    {
        public HealthExportDto ExportDto { get; set; } = null!;

        public ImportBatch ImportBatch { get; set; } = null!;

        public List<ActivityDay> ActivityDays { get; set; } = new();

        public List<SleepSession> SleepSessions { get; set; } = new();

        public List<HeartRateDay> HeartRateDays { get; set; } = new();

        public List<BloodOxygenDay> BloodOxygenDays { get; set; } = new();

        public List<HeightMeasurement> HeightMeasurements { get; set; } = new();

        public List<WeightMeasurement> WeightMeasurements { get; set; } = new();
    }
}