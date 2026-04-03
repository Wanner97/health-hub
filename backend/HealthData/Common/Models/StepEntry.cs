namespace Common.Models
{
    public class StepEntry
    {
        public int Id { get; set; }

        public int ImportBatchId { get; set; }

        public StepRecordsImportBatch StepRecordsImportBatch { get; set; } = null!;

        public DateOnly Date { get; set; }

        public int Count { get; set; }

        public DateTimeOffset StartTime { get; set; }

        public DateTimeOffset EndTime { get; set; }
    }
}
