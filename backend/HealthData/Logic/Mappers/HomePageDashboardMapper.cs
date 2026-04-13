using Common.Dtos.DataReadDtos.Dashboard;
using Common.Dtos.DataReadDtos.Dashboard.SummaryDtos;
using Common.Models;

namespace Logic.Mappers
{
    public static class HomepageDashboardMapper
    {
        public static HomepageDashboardDto MapToHomepageDashboardDto(ImportBatch? latestImportBatch, ActivityDay? latestActivityDay)
        {
            return new HomepageDashboardDto
            {
                LatestImport = latestImportBatch == null ? null : new LatestImportSummaryDto
                {
                    ImportedAtUtc = latestImportBatch.ImportedAtUtc,
                    ReceivedRecordCount = latestImportBatch.ReceivedRecordCount,
                    InsertedRecordCount = latestImportBatch.InsertedRecordCount,
                    UpdatedRecordCount = latestImportBatch.UpdatedRecordCount,
                    UnchangedRecordCount = latestImportBatch.UnchangedRecordCount,
                    ExportVersion = latestImportBatch.ExportVersion
                },

                LatestActivityDay = latestActivityDay == null ? null : new LatestActivityDaySummaryDto
                {
                    Date = latestActivityDay.Date,
                    Steps = latestActivityDay.Steps
                }
            };
        }
    }
}