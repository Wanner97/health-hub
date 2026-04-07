using System.Text.Json.Serialization;

namespace Common.Dtos.DataImportDtos
{
    public class ActivityExportDto
    {
        [JsonPropertyName("exportVersion")]
        public int ExportVersion { get; set; }

        [JsonPropertyName("exportedAt")]
        public DateTimeOffset ExportedAt { get; set; }

        [JsonPropertyName("rangeStart")]
        public DateTimeOffset RangeStart { get; set; }

        [JsonPropertyName("rangeEnd")]
        public DateTimeOffset RangeEnd { get; set; }

        [JsonPropertyName("source")]
        public string Source { get; set; } = string.Empty;

        [JsonPropertyName("records")]
        public List<ActivityRecordDto> Records { get; set; } = new List<ActivityRecordDto>();
    }
}
