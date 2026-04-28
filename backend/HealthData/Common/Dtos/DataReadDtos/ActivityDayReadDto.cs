namespace Common.Dtos.DataReadDtos
{
    public class ActivityDayReadDto
    {
        public DateOnly Date { get; set; }

        public DateTime StartTimeUtc { get; set; }

        public DateTime EndTimeUtc { get; set; }

        public int Steps { get; set; }

        public double DistanceMeters { get; set; }

        public string DistanceSource { get; set; } = string.Empty;

        public double TotalCaloriesBurnedKcal { get; set; }
    }
}