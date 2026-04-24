namespace Common.Dtos.DataReadDtos
{
    public class BloodOxygenDayReadDto
    {
        public DateOnly Date { get; set; }

        public DateTime StartTimeUtc { get; set; }

        public DateTime EndTimeUtc { get; set; }

        public double AvgPercent { get; set; }

        public double MinPercent { get; set; }

        public double MaxPercent { get; set; }

        public int MeasurementCount { get; set; }
    }
}