namespace Common.Dtos.DataReadDtos
{
    public class LatestStepsImportDto
    {
        public int ImportBatchId { get; set; }

        public string Source { get; set; } = string.Empty;

        public int ExportVersion { get; set; }

        public DateTimeOffset ExportedAt { get; set; }

        public DateTimeOffset ImportedAt { get; set; }

        public int RecordCount { get; set; }

        public List<StepEntryReadDto> StepEntries { get; set; } = new List<StepEntryReadDto>();
    }
}