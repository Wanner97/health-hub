namespace Common.Dtos.DataReadDtos.Dashboard.SummaryDtos
{
    public class LatestImportSummaryDto
    {
        public DateTime ImportedAtUtc { get; set; }

        public int ReceivedRecordCount { get; set; }

        public int InsertedRecordCount { get; set; }

        public int UpdatedRecordCount { get; set; }

        public int UnchangedRecordCount { get; set; }

        public string ExportVersion { get; set; }
    }
}