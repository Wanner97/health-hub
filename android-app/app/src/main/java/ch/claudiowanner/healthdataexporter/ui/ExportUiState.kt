package ch.claudiowanner.healthdataexporter.ui

import ch.claudiowanner.healthdataexporter.export.ExportPhase
import ch.claudiowanner.healthdataexporter.ui.preview.ExportPreviewSummary
import ch.claudiowanner.healthdataexporter.ui.preview.PreviewChunk

data class ExportUiState(
    val statusTitle: String = "Ready",
    val statusMessage: String = "No export created yet.",
    val currentPhase: ExportPhase = ExportPhase.IDLE,
    val progressCurrent: Int? = null,
    val progressTotal: Int? = null,
    val exportPreview: String? = null,
    val previewSummary: ExportPreviewSummary? = null,
    val previewFullText: String? = null,
    val previewHighlightedFullChunks: List<PreviewChunk> = emptyList(),
    val isPreviewLoading: Boolean = false,
    val previewLoadingMessage: String? = null,
    val isBusy: Boolean = false,
    val lastExportPath: String? = null
) {
    val hasDeterminateProgress: Boolean
        get() = progressCurrent != null && progressTotal != null && progressTotal > 0
}