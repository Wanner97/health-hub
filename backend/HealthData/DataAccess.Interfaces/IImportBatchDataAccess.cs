using Common.Models;

namespace DataAccess.Interfaces
{
    public interface IImportBatchDataAccess
    {
        Dictionary<DateOnly, ActivityDay> GetExistingActivityDays(string source, IEnumerable<DateOnly> dates);

        ImportBatch ApplyImport(
            ImportBatch importBatch,
            List<ActivityDay> insertedActivityDays,
            List<ActivityDay> updatedActivityDays);
    }
}
