using Common.Dtos.DataReadDtos;

namespace Logic.Interfaces
{
    public interface IHeartRateDayReadLogic
    {
        List<HeartRateDayReadDto> GetHeartRateDays(DateOnly? from, DateOnly? to, bool includeHourlyRecords);
    }
}