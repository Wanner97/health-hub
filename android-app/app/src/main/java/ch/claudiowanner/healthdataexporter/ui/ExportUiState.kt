package ch.claudiowanner.healthdataexporter.ui

import androidx.compose.ui.text.AnnotatedString

data class ExportUiState(
    val statusTitle: String = "Ready",
    val statusMessage: String = "No export created yet.",
    val currentPhase: ExportProgressPhase = ExportProgressPhase.IDLE,
    val progressCurrent: Int? = null,
    val progressTotal: Int? = null,

    // Bestehende Preview, vorerst für Kompatibilität beibehalten
    val exportPreview: String? = null,

    // Neue Preview-Zustände
    val previewSummary: ExportPreviewSummary? = null,
    val previewDisplayMode: PreviewDisplayMode = PreviewDisplayMode.SNIPPET,
    val previewShortText: String = "No export loaded yet.",
    val previewFullText: String? = null,
    val previewHighlightedFullChunks: List<PreviewChunk> = emptyList(),
    val isPreviewLoading: Boolean = false,
    val previewLoadingMessage: String? = null,
    val canLoadFullPreview: Boolean = false,

    val isBusy: Boolean = false,
    val lastExportPath: String? = null
) {
    val hasDeterminateProgress: Boolean
        get() = progressCurrent != null && progressTotal != null && progressTotal > 0
}