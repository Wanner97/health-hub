using Common.Models;

namespace DataAccess.Interfaces
{
    public interface IImportBatchDataAccess
    {
        ImportBatch? GetLatestImportBatch();
        List<ImportBatch> GetImportBatches(DateOnly? from, DateOnly? to);
        Dictionary<DateOnly, ActivityDay> GetExistingActivityDays(string source, IEnumerable<DateOnly> dates);

        ImportBatch ApplyImport(
            ImportBatch importBatch,
            List<ActivityDay> insertedActivityDays,
            List<ActivityDay> updatedActivityDays);
    }
}
