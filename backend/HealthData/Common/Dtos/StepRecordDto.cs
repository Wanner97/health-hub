using System.Text.Json.Serialization;

namespace Common.Dtos
{
    public class StepRecordDto
    {
        [JsonPropertyName("count")]
        public int Count { get; set; }

        [JsonPropertyName("date")]
        public DateOnly Date { get; set; }

        [JsonPropertyName("startTime")]
        public DateTimeOffset StartTime { get; set; }

        [JsonPropertyName("endTime")]
        public DateTimeOffset EndTime { get; set; }
    }
}
