package ch.claudiowanner.healthdataexporter.model.sleep

data class SleepExportCluster(
    val sessions: List<SleepSessionExportRecord>
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