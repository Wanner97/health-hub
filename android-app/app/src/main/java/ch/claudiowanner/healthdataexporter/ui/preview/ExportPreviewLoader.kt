package ch.claudiowanner.healthdataexporter.ui.preview

import kotlinx.coroutines.Dispatchers
import kotlinx.coroutines.withContext

class ExportPreviewLoader(
    private val summaryParser: ExportPreviewSummaryParser = ExportPreviewSummaryParser(),
    private val chunkBuilder: ExportPreviewChunkBuilder = ExportPreviewChunkBuilder()
) {
    fun parseSummary(
        previewContent: String
    ): ExportPreviewSummary? {
        return summaryParser.parse(previewContent)
    }

    suspend fun load(
        previewContent: String
    ): ExportPreviewLoadResult {
        val summary = parseSummary(previewContent)

        val highlightedChunks = withContext(Dispatchers.Default) {
            chunkBuilder.build(previewContent)
        }

        return ExportPreviewLoadResult(
            summary = summary,
            fullText = previewContent,
            highlightedChunks = highlightedChunks
        )
    }
}