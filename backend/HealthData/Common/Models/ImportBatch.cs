namespace Common.Models
{
    public class ImportBatch
    {
        public int Id { get; set; }

        public DateTime ExportedAtUtc { get; set; }

        public DateTime ImportedAtUtc { get; set; }

        public DateTime RangeStartUtc { get; set; }

        public DateTime RangeEndUtc { get; set; }

        public int ReceivedRecordCount { get; set; }

        public int InsertedRecordCount { get; set; }

        public int UpdatedRecordCount { get; set; }

        public int UnchangedRecordCount { get; set; }

        public string Source { get; set; } = string.Empty;

        public int ExportVersion { get; set; }
        
        public List<ActivityDay> ActivityDayEntries { get; set; } = new List<ActivityDay>();
    }
}
