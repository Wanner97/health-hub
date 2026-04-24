namespace Common.Dtos.DataImportDtos
{
    public class BloodOxygenDailyClusterDto
    {
        public List<BloodOxygenDayDto> Records { get; set; } = new();
    }
}