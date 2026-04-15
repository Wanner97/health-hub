package ch.claudiowanner.healthdataexporter.model

data class ExportPayload(
    val exportVersion: Int,
    val source: String,
    val exportedAt: String,
    val exportType: String,
    val rangeDays: Int?,
    val rangeStart: String,
    val rangeEnd: String,
    val clusters: ExportClusters
)

data class ExportClusters(
    val activity: ActivityExportCluster,
    val sleep: SleepExportCluster
)

data class ActivityExportCluster(
    val records: List<ActivityDayExportRecord>
)

data class SleepExportCluster(
    val sessions: List<SleepSessionExportRecord>
)

data class ActivityDayExportRecord(
    val date: String,
    val steps: Long,
    val distanceMeters: Double,
    val startTime: String,
    val endTime: String
)

data class SleepSessionExportRecord(
    val startTime: String,
    val endTime: String,
    val durationMinutes: Long,
    val title: String?,
    val notes: String?,
    val stages: List<SleepStageExportRecord>
)

data class SleepStageExportRecord(
    val startTime: String,
    val endTime: String,
    val stage: String
)