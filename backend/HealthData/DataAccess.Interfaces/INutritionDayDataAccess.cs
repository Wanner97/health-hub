using Common.Models;

namespace DataAccess.Interfaces
{
    public interface INutritionDayDataAccess
    {
        List<NutritionDay> GetNutritionDays(DateOnly? from, DateOnly? to);
    }
}