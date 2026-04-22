namespace Common.Dtos.DataReadDtos
{
    public class HeartRateDayReadDto
    {
        public DateOnly Date { get; set; }

        public int AvgBpm { get; set; }

        public int MinBpm { get; set; }

        public int MaxBpm { get; set; }

        public int MeasurementCount { get; set; }

        public List<HeartRateHourlyRecordReadDto> HourlyRecords { get; set; } = new();
    }
}