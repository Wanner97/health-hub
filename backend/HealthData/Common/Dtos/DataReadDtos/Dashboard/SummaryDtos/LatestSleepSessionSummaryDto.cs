namespace Common.Dtos.DataReadDtos.Dashboard.SummaryDtos
{
    public class LatestSleepSessionSummaryDto
    {
        public DateTime StartTimeUtc { get; set; }

        public DateTime EndTimeUtc { get; set; }

        public int DurationMinutes { get; set; }
    }
}