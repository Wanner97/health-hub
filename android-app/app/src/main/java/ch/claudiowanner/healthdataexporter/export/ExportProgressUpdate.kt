package ch.claudiowanner.healthdataexporter.export

data class ExportProgressUpdate(
    val phase: ExportOperationPhase,
    val title: String,
    val message: String,
    val progressCurrent: Int? = null,
    val progressTotal: Int? = null
)