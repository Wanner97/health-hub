package ch.claudiowanner.healthdataexporter.model

data class ExportPayload(
    val exportVersion: Int,
    val source: String,
    val exportedAt: String,
    val records: List<StepExportRecord>
)

data class StepExportRecord(
    val count: Long,
    val startTime: String,
    val endTime: String
)