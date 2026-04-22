namespace Common.Models
{
    public class HeartRateHourlyRecord
    {
        public int Id { get; set; }

        public int HeartRateDayId { get; set; }

        public HeartRateDay HeartRateDay { get; set; } = null!;

        public int Hour { get; set; }

        public DateTime StartTimeUtc { get; set; }

        public DateTime EndTimeUtc { get; set; }

        public int AvgBpm { get; set; }

        public int MinBpm { get; set; }

        public int MaxBpm { get; set; }

        public int MeasurementCount { get; set; }
    }
}