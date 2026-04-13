namespace Common.Dtos.DataReadDtos
{
    public class ImportBatchReadDto
    {
        public DateTime ExportedAtUtc { get; set; }

        public DateTime ImportedAtUtc { get; set; }

        public DateTime RangeStartUtc { get; set; }

        public DateTime RangeEndUtc { get; set; }

        public int ReceivedRecordCount { get; set; }

        public int InsertedRecordCount { get; set; }

        public int UpdatedRecordCount { get; set; }

        public int UnchangedRecordCount { get; set; }

        public int ExportVersion { get; set; }
    }
}
