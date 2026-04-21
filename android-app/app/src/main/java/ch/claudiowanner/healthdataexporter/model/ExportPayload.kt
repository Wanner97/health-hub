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
    val sleep: SleepExportCluster,
    val vitals: VitalsExportCluster
)

data class ActivityExportCluster(
    val records: List<ActivityDayExportRecord>
)

data class SleepExportCluster(
    val sessions: List<SleepSessionExportRecord>
)

data class VitalsExportCluster(
    val heartRateDaily: HeartRateDailyExportCluster,
    val heartRateHourly: HeartRateHourlyExportCluster
)

data class HeartRateDailyExportCluster(
    val records: List<HeartRateDailyExportRecord>
)

data class HeartRateHourlyExportCluster(
    val records: List<HeartRateHourlyExportRecord>
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

data class HeartRateDailyExportRecord(
    val date: String,
    val minBpm: Long,
    val maxBpm: Long,
    val avgBpm: Long,
    val measurementCount: Long,
    val startTime: String,
    val endTime: String
)

data class HeartRateHourlyExportRecord(
    val date: String,
    val hour: Int,
    val minBpm: Long,
    val maxBpm: Long,
    val avgBpm: Long,
    val measurementCount: Long,
    val startTime: String,
    val endTime: String
)