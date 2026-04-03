namespace Common.Models
{
    public class StepRecordsImportBatch
    {
        public int Id { get; set; }

        public string Source { get; set; } = string.Empty;

        public int ExportVersion { get; set; }

        public DateTimeOffset ExportedAt { get; set; }

        public DateTimeOffset ImportedAt { get; set; }

        public int RecordCount { get; set; }

        public List<StepEntry> StepEntries { get; set; } = new List<StepEntry>();
    }
}
