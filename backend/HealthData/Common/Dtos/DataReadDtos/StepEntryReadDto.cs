namespace Common.Dtos.DataReadDtos
{
    public class StepEntryReadDto
    {
        public DateOnly Date { get; set; }

        public int Count { get; set; }

        public DateTimeOffset StartTime { get; set; }

        public DateTimeOffset EndTime { get; set; }
    }
}