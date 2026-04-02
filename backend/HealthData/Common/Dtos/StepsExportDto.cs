using System.Text.Json.Serialization;

namespace Common.Dtos
{
    public class StepsExportDto
    {
        [JsonPropertyName("exportVersion")]
        public int ExportVersion { get; set; }

        [JsonPropertyName("exportedAt")]
        public DateTimeOffset ExportedAt { get; set; }

        [JsonPropertyName("source")]
        public string Source { get; set; } = string.Empty;

        [JsonPropertyName("records")]
        public List<StepRecordDto> Records { get; set; } = new List<StepRecordDto>();
    }
}
