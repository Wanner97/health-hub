namespace Common.Dtos.DataReadDtos
{
    public class HeartRateHourlyRecordReadDto
    {
        public int Hour { get; set; }

        public DateTime StartTimeUtc { get; set; }

        public DateTime EndTimeUtc { get; set; }

        public int AvgBpm { get; set; }

        public int MinBpm { get; set; }

        public int MaxBpm { get; set; }

        public int MeasurementCount { get; set; }
    }
}