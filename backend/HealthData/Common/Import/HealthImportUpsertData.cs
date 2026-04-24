using Common.Models;

namespace Common.Import
{
    public class HealthImportUpsertData
    {
        public ImportUpsertData<ActivityDay> ActivityDays { get; set; } = new();

        public ImportUpsertData<SleepSession> SleepSessions { get; set; } = new();

        public ImportUpsertData<HeartRateDay> HeartRateDays { get; set; } = new();

        public ImportUpsertData<BloodOxygenDay> BloodOxygenDays { get; set; } = new();
    }
}