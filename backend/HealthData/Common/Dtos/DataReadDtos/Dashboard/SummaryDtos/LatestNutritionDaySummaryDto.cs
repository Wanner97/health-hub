namespace Common.Dtos.DataReadDtos.Dashboard.SummaryDtos
{
    public class LatestNutritionDaySummaryDto
    {
        public DateOnly Date { get; set; }

        public double TotalEnergyKcal { get; set; }
    }
}