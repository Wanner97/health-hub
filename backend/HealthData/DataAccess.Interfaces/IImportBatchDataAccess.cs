using Common.Models;

namespace DataAccess.Interfaces
{
    public interface IImportBatchDataAccess
    {
        StepRecordsImportBatch CreateImportBatch(StepRecordsImportBatch stepRecordsImportBatch);
    }
}
