package ch.claudiowanner.healthdataexporter.export

data class ExportExecutionResult(
    val filePath: String,
    val exportPreview: String,
    val progressTotal: Int
)