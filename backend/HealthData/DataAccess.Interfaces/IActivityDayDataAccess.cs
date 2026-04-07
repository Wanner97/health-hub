using Common.Models;

namespace DataAccess.Interfaces
{
    public interface IActivityDayDataAccess
    {
        List<ActivityDay> GetActivityDays(DateOnly? from, DateOnly? to);
    }
}