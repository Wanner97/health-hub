using Common.Import;
using Common.Models;

namespace DataAccess.Interfaces
{
    public interface IImportBatchDataAccess
    {
        ImportBatch? GetLatestImportBatch();

        List<ImportBatch> GetImportBatches(DateOnly? from, DateOnly? to);

        Dictionary<DateOnly, ActivityDay> GetExistingActivityDays(string source, IEnumerable<DateOnly> dates);

        Dictionary<DateTime, SleepSession> GetExistingSleepSessions(string source, IEnumerable<DateTime> startTimes);

        Dictionary<DateOnly, HeartRateDay> GetExistingHeartRateDays(string source, IEnumerable<DateOnly> dates);

        Dictionary<DateOnly, BloodOxygenDay> GetExistingBloodOxygenDays(string source, IEnumerable<DateOnly> dates);

        ImportBatch ApplyImport(ImportBatch importBatch, HealthImportUpsertData upsertData);
    }
}