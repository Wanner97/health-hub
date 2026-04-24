using Common.Dtos.DataReadDtos;

namespace Logic.Interfaces
{
    public interface IBloodOxygenDayReadLogic
    {
        List<BloodOxygenDayReadDto> GetBloodOxygenDays(DateOnly? from, DateOnly? to);
    }
}