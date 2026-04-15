using Common.Models;

namespace DataAccess.Interfaces
{
    public interface IImportBatchDataAccess
    {
        ImportBatch? GetLatestImportBatch();
        List<ImportBatch> GetImportBatches(DateOnly? from, DateOnly? to);
        Dictionary<DateOnly, ActivityDay> GetExistingActivityDays(string source, IEnumerable<DateOnly> dates);
        Dictionary<DateTime, SleepSession> GetExistingSleepSessions(string source, IEnumerable<DateTime> startTimes);

        ImportBatch ApplyImport(
            ImportBatch importBatch,
            List<ActivityDay> insertedActivityDays,
            List<ActivityDay> updatedActivityDays,
            List<SleepSession> insertedSleepSessions,
            List<SleepSession> updatedSleepSessions);
    }
}
