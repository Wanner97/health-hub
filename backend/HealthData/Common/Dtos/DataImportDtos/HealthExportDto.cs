namespace Common.Dtos.DataImportDtos
{
    public class HealthExportDto
    {
        public string ExportVersion { get; set; }

        public string Source { get; set; } = string.Empty;

        public DateTimeOffset ExportedAt { get; set; }

        public string ExportType { get; set; } = string.Empty;

        public DateTimeOffset RangeStart { get; set; }

        public DateTimeOffset RangeEnd { get; set; }

        public HealthExportClustersDto Clusters { get; set; } = new();
    }
}