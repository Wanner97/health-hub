package ch.claudiowanner.healthdataexporter.ui.preview

data class ExportPreviewSummary(
    val exportType: String? = null,
    val rangeDescription: String? = null,
    val activityRecordCount: Int? = null,
    val sleepSessionCount: Int? = null,
    val heartRateDailyCount: Int? = null,
    val heartRateHourlyCount: Int? = null,
    val bloodOxygenDailyCount: Int? = null,
    val weightRecordCount: Int? = null,
    val hasLatestHeight: Boolean? = null,
    val nutritionRecordCount: Int? = null,
)