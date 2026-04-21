package ch.claudiowanner.healthdataexporter.model.activity

data class ActivityExportCluster(
    val records: List<ActivityDayExportRecord>
)

data class ActivityDayExportRecord(
    val date: String,
    val steps: Long,
    val distanceMeters: Double,
    val startTime: String,
    val endTime: String
)