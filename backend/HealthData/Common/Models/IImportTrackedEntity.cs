namespace Common.Models
{
    public interface IImportTrackedEntity
    {
        DateTime LastImportedAtUtc { get; set; }

        int LastImportBatchId { get; set; }

        ImportBatch LastImportBatch { get; set; }
    }
}