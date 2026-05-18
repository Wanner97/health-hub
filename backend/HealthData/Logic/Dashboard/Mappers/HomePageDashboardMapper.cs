using Common.Constants;
using Common.Dashboard;
using Common.Dtos.DataReadDtos.Dashboard;
using Common.Dtos.DataReadDtos.Dashboard.SummaryDtos;
using Common.Models;

namespace Logic.Dashboard.Mappers
{
    public static class HomepageDashboardMapper
    {
        public static HomepageDashboardDto MapToHomepageDashboardDto(HomepageDashboardData dashboardData)
        {
            return new HomepageDashboardDto
            {
                LatestImport = dashboardData.LatestImportBatch == null ? null : new LatestImportSummaryDto
                {
                    ImportedAtUtc = dashboardData.LatestImportBatch.ImportedAtUtc,
                    ReceivedRecordCount = dashboardData.LatestImportBatch.ReceivedRecordCount,
                    InsertedRecordCount = dashboardData.LatestImportBatch.InsertedRecordCount,
                    UpdatedRecordCount = dashboardData.LatestImportBatch.UpdatedRecordCount,
                    UnchangedRecordCount = dashboardData.LatestImportBatch.UnchangedRecordCount,
                    ExportVersion = dashboardData.LatestImportBatch.ExportVersion
                },

                LatestActivityDay = dashboardData.LatestActivityDay == null ? null : new LatestActivityDaySummaryDto
                {
                    Date = dashboardData.LatestActivityDay.Date,
                    Steps = dashboardData.LatestActivityDay.Steps,
                    DistanceMeters = dashboardData.LatestActivityDay.DistanceMeters,
                    DistanceSource = dashboardData.LatestActivityDay.DistanceSource,
                    TotalCaloriesBurnedKcal = dashboardData.LatestActivityDay.TotalCaloriesBurnedKcal
                },

                LatestSleepSession = dashboardData.LatestSleepSession == null ? null : new LatestSleepSessionSummaryDto
                {
                    StartTimeUtc = dashboardData.LatestSleepSession.StartTimeUtc,
                    EndTimeUtc = dashboardData.LatestSleepSession.EndTimeUtc,
                    DurationMinutes = dashboardData.LatestSleepSession.DurationMinutes
                },

                LatestHeartRateDay = dashboardData.LatestHeartRateDay == null ? null : new LatestHeartRateDaySummaryDto
                {
                    Date = dashboardData.LatestHeartRateDay.Date,
                    AvgBpm = dashboardData.LatestHeartRateDay.AvgBpm,
                    MinBpm = dashboardData.LatestHeartRateDay.MinBpm,
                    MaxBpm = dashboardData.LatestHeartRateDay.MaxBpm,
                    MeasurementCount = dashboardData.LatestHeartRateDay.MeasurementCount
                },

                LatestBloodOxygenDay = dashboardData.LatestBloodOxygenDay == null ? null : new LatestBloodOxygenDaySummaryDto
                {
                    Date = dashboardData.LatestBloodOxygenDay.Date,
                    AvgPercent = dashboardData.LatestBloodOxygenDay.AvgPercent,
                    MinPercent = dashboardData.LatestBloodOxygenDay.MinPercent,
                    MaxPercent = dashboardData.LatestBloodOxygenDay.MaxPercent,
                    MeasurementCount = dashboardData.LatestBloodOxygenDay.MeasurementCount
                },

                LatestHeightMeasurement = dashboardData.LatestHeightMeasurement == null ? null : new LatestHeightMeasurementSummaryDto
                {
                    HeightCm = dashboardData.LatestHeightMeasurement.HeightCm,
                    MeasuredAtUtc = dashboardData.LatestHeightMeasurement.MeasuredAtUtc
                },

                LatestWeightMeasurement = dashboardData.LatestWeightMeasurement == null ? null : new LatestWeightMeasurementSummaryDto
                {
                    Date = dashboardData.LatestWeightMeasurement.Date,
                    WeightKg = dashboardData.LatestWeightMeasurement.WeightKg,
                    MeasuredAtUtc = dashboardData.LatestWeightMeasurement.MeasuredAtUtc
                },

                LatestNutritionDay = dashboardData.LatestNutritionDay == null ? null : new LatestNutritionDaySummaryDto
                {
                    Date = dashboardData.LatestNutritionDay.Date,
                    TotalEnergyKcal = GetNutritionTotalAmount(dashboardData.LatestNutritionDay, NutritionNutrientKeys.EnergyKcal)
                }
            };
        }

        private static double GetNutritionTotalAmount(NutritionDay nutritionDay, string nutrientKey)
        {
            return nutritionDay.NutrientTotals.FirstOrDefault(x => x.NutrientKey == nutrientKey)?.TotalAmount ?? 0d;
        }
    }
}