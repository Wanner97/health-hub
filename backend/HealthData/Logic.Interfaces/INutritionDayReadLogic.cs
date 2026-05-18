using Common.Dtos.DataReadDtos;

namespace Logic.Interfaces
{
    public interface INutritionDayReadLogic
    {
        List<NutritionDayReadDto> GetNutritionDays(DateOnly? from, DateOnly? to);
    }
}