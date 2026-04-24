using Common.Models;

namespace DataAccess.Interfaces
{
    public interface IBloodOxygenDayDataAccess
    {
        List<BloodOxygenDay> GetBloodOxygenDays(DateOnly? from, DateOnly? to);

        BloodOxygenDay? GetLatestBloodOxygenDay();
    }
}