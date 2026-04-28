using System.Text.Json.Serialization;

namespace Common.Dtos.DataImportDtos
{
    public class ActivityRecordDto
    {
        [JsonPropertyName("date")]
        public DateOnly Date { get; set; }

        [JsonPropertyName("startTime")]
        public DateTimeOffset StartTime { get; set; }

        [JsonPropertyName("endTime")]
        public DateTimeOffset EndTime { get; set; }

        [JsonPropertyName("steps")]
        public int Steps { get; set; }

        [JsonPropertyName("distanceMeters")]
        public double DistanceMeters { get; set; }

        [JsonPropertyName("totalCaloriesBurnedKcal")]
        public double TotalCaloriesBurnedKcal { get; set; }
    }
}
