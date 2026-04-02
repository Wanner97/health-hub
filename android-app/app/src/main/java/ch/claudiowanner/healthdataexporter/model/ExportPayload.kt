package ch.claudiowanner.healthdataexporter.model

data class ExportPayload(
    val source: String,
    val exportedAt: String,
    val records: List<HealthRecordExport>
)

data class HealthRecordExport(
    val type: String,
    val value: Long,
    val unit: String,
    val startTime: String,
    val endTime: String
)