package ch.claudiowanner.healthdataexporter.ui

data class ExportUiState(
    val statusText: String = "No export created yet.",
    val exportPreview: String? = null,
    val isBusy: Boolean = false
)