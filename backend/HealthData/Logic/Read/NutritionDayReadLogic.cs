using Common.Dtos.DataReadDtos;
using DataAccess.Interfaces;
using Logic.Interfaces;
using Logic.Read.Mappers;

namespace Logic.Read
{
    public class NutritionDayReadLogic : INutritionDayReadLogic
    {
        private readonly INutritionDayDataAccess _nutritionDayDataAccess;

        public NutritionDayReadLogic(INutritionDayDataAccess nutritionDayDataAccess)
        {
            _nutritionDayDataAccess = nutritionDayDataAccess;
        }

        public List<NutritionDayReadDto> GetNutritionDays(DateOnly? from, DateOnly? to)
        {
            var nutritionDays = _nutritionDayDataAccess.GetNutritionDays(from, to);

            return NutritionDayReadMapper.MapToReadDtos(nutritionDays);
        }
    }
}