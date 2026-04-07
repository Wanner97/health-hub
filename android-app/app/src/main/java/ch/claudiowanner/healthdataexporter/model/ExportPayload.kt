package ch.claudiowanner.healthdataexporter.model

data class ExportPayload(
    val exportVersion: Int,
    val source: String,
    val exportedAt: String,
    val rangeStart: String,
    val rangeEnd: String,
    val records: List<ActivityDayExportRecord>
)

data class ActivityDayExportRecord(
    val date: String,
    val steps: Long,
    val distanceMeters: Double,
    val startTime: String,
    val endTime: String
)