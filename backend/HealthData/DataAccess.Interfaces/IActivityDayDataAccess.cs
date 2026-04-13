using Common.Models;

namespace DataAccess.Interfaces
{
    public interface IActivityDayDataAccess
    {
        ActivityDay? GetLatestActivityDay();
        List<ActivityDay> GetActivityDays(DateOnly? from, DateOnly? to);
    }
}