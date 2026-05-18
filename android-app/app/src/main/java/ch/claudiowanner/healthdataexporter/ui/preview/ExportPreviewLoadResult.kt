package ch.claudiowanner.healthdataexporter.ui.preview

data class ExportPreviewLoadResult(
    val summary: ExportPreviewSummary?,
    val fullText: String,
    val highlightedChunks: List<PreviewChunk>
)