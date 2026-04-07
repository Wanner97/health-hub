namespace Common.Dtos.DataReadDtos
{
    public class ActivityDayReadDto
    {
        public DateOnly Date { get; set; }

        public int Steps { get; set; }

        public double DistanceMeters { get; set; }

        public DateTime StartTimeUtc { get; set; }

        public DateTime EndTimeUtc { get; set; }
    }
}