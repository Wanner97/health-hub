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

        public string ExportVersion { get; set; }

        public string ExportType { get; set; } = string.Empty;

        public List<ActivityDay> ActivityDayEntries { get; set; } = new();

        public List<SleepSession> SleepSessionEntries { get; set; } = new();

        public List<HeartRateDay> HeartRateDayEntries { get; set; } = new();

        public List<BloodOxygenDay> BloodOxygenDayEntries { get; set; } = new();
    }
}