package ch.claudiowanner.healthdataexporter.ui

import android.app.Application
import android.net.Uri
import androidx.compose.runtime.getValue
import androidx.compose.runtime.mutableStateOf
import androidx.compose.runtime.setValue
import androidx.health.connect.client.HealthConnectClient
import androidx.lifecycle.AndroidViewModel
import androidx.lifecycle.viewModelScope
import ch.claudiowanner.healthdataexporter.config.ExportConfig
import ch.claudiowanner.healthdataexporter.config.ExportType
import ch.claudiowanner.healthdataexporter.export.ExportCoordinator
import ch.claudiowanner.healthdataexporter.export.ExportProgressUpdate
import ch.claudiowanner.healthdataexporter.export.ExportRequest
import ch.claudiowanner.healthdataexporter.export.ExportPhase
import ch.claudiowanner.healthdataexporter.healthconnect.HealthConnectAvailability
import ch.claudiowanner.healthdataexporter.healthconnect.HealthConnectGateway
import ch.claudiowanner.healthdataexporter.serialization.ExportJsonSerializer
import ch.claudiowanner.healthdataexporter.storage.ExportFileWriter
import ch.claudiowanner.healthdataexporter.ui.preview.ExportPreviewChunkBuilder
import ch.claudiowanner.healthdataexporter.ui.preview.ExportPreviewSummaryParser
import kotlinx.coroutines.Dispatchers
import kotlinx.coroutines.launch
import kotlinx.coroutines.withContext
import java.io.File
import java.time.LocalDate
import java.time.ZoneId

class ExportViewModel(application: Application) : AndroidViewModel(application) {
    private val appContext = application.applicationContext
    private val exportFileWriter = ExportFileWriter()
    private val healthConnectGateway = HealthConnectGateway(appContext)
    private val exportJsonSerializer = ExportJsonSerializer()
    private val exportPreviewSummaryParser = ExportPreviewSummaryParser()
    private val exportPreviewChunkBuilder = ExportPreviewChunkBuilder()
    private val exportCoordinator = ExportCoordinator(
        appContext = appContext,
        healthConnectGateway = healthConnectGateway,
        exportFileWriter = exportFileWriter,
        exportJsonSerializer = exportJsonSerializer
    )

    var uiState: ExportUiState by mutableStateOf(ExportUiState())
        private set

    suspend fun shouldLaunchPermissionRequest(): Boolean {
        setBusyPhase(
            phase = ExportPhase.CHECKING_PERMISSIONS,
            title = "Checking permissions",
            message = "Checking Health Connect availability and existing permissions."
        )

        return when (healthConnectGateway.getSdkStatus()) {
            HealthConnectClient.SDK_AVAILABLE -> {
                val alreadyGranted = healthConnectGateway.hasAllPermissions()

                if (alreadyGranted) {
                    setCompletedState(
                        title = "Permissions ready",
                        message = "All required health permissions are already granted."
                    )
                    false
                } else {
                    setIdleState(
                        title = "Permission request ready",
                        message = "Opening the Health Connect permission dialog.",
                        phase = ExportPhase.CHECKING_PERMISSIONS
                    )
                    true
                }
            }

            HealthConnectClient.SDK_UNAVAILABLE -> {
                setFailureState("Health Connect is unavailable on this device.")
                false
            }

            HealthConnectClient.SDK_UNAVAILABLE_PROVIDER_UPDATE_REQUIRED -> {
                setFailureState("Health Connect requires an update before permissions can be requested.")
                false
            }

            else -> {
                setFailureState("Health Connect status is unknown.")
                false
            }
        }
    }

    fun onPermissionsResult(granted: Set<String>) {
        if (granted.containsAll(HealthConnectAvailability.PERMISSIONS)) {
            setCompletedState(
                title = "Permissions granted",
                message = "All required health permissions were granted."
            )
        } else {
            setFailureState("Not all requested permissions were granted.")
        }
    }

    fun exportFullHistory() {
        runExport(
            ExportRequest(
                exportType = ExportType.FULL_HISTORY,
                rangeDays = null,
                startDateInclusive = ExportConfig.FULL_HISTORY_START_DATE
            )
        )
    }

    fun exportLastDays(days: Int) {
        require(days > 0) { "days must be greater than 0." }

        val zoneId = ZoneId.systemDefault()
        val today = LocalDate.now(zoneId)
        val startDateInclusive = today.minusDays((days - 1).toLong())

        runExport(
            ExportRequest(
                exportType = ExportType.ROLLING_WINDOW,
                rangeDays = days,
                startDateInclusive = startDateInclusive
            )
        )
    }

    fun loadLatestExport() {
        viewModelScope.launch {
            setBusyPhase(
                phase = ExportPhase.LOADING_EXPORT,
                title = "Loading export",
                message = "Reading the latest export file from app storage.",
                progressCurrent = 1,
                progressTotal = 1
            )

            val result = exportFileWriter.readLatestExport(appContext)

            result.fold(
                onSuccess = { (file, content) ->
                    applyCompletedStateWithPreview(
                        title = "Export loaded",
                        message = "Latest export loaded: ${file.absolutePath}",
                        exportPreview = content,
                        lastExportPath = file.absolutePath,
                        progressCurrent = 1,
                        progressTotal = 1
                    )
                },
                onFailure = {
                    setFailureState("Loading export failed: ${it.message}")
                }
            )
        }
    }

    fun canLaunchSaveDialog(): Boolean {
        if (uiState.exportPreview == null) {
            setIdleState(
                title = "No export loaded",
                message = "Load or create an export before saving it."
            )
            return false
        }

        return true
    }

    fun suggestedExportFileName(): String {
        val today = LocalDate.now().toString()
        return "${ExportConfig.EXPORT_FILE_PREFIX}-$today.json"
    }

    fun onSaveCancelled() {
        setIdleState(
            title = "Saving cancelled",
            message = "Saving export was cancelled."
        )
    }

    fun saveLatestExportToUri(uri: Uri) {
        val preview = uiState.exportPreview
        if (preview == null) {
            setIdleState(
                title = "No export loaded",
                message = "Load or create an export before saving it."
            )
            return
        }

        viewModelScope.launch {
            setBusyPhase(
                phase = ExportPhase.SAVING_EXPORT,
                title = "Saving export",
                message = "Writing the current export to the selected location.",
                progressCurrent = 1,
                progressTotal = 1
            )

            val result = exportFileWriter.writeJsonToUri(
                context = appContext,
                uri = uri,
                jsonContent = preview
            )

            result.fold(
                onSuccess = {
                    setCompletedState(
                        title = "Export saved",
                        message = "Export saved to selected location successfully.",
                        progressCurrent = 1,
                        progressTotal = 1
                    )
                },
                onFailure = {
                    setFailureState("Saving export failed: ${it.message}")
                }
            )
        }
    }

    fun getLatestExportFileForSharing(): File? {
        setBusyPhase(
            phase = ExportPhase.PREPARING_SHARE,
            title = "Preparing share",
            message = "Loading the latest export file for sharing."
        )

        val latestFile = exportFileWriter.getLatestExportFile(appContext)

        if (latestFile == null) {
            setFailureState("No export file found to share.")
            return null
        }

        setIdleState(
            title = "Share ready",
            message = "Opening share sheet for latest export.",
            phase = ExportPhase.PREPARING_SHARE,
            lastExportPath = latestFile.absolutePath
        )

        return latestFile
    }

    fun onShareSheetOpened() {
        setIdleState(
            title = "Share sheet opened",
            message = "Share sheet opened for latest export.",
            phase = ExportPhase.PREPARING_SHARE
        )
    }

    private fun runExport(request: ExportRequest) {
        viewModelScope.launch {
            val result = exportCoordinator.executeExport(request) { update ->
                applyProgressUpdate(update)
            }

            result.fold(
                onSuccess = { executionResult ->
                    applyCompletedStateWithPreview(
                        title = "Export complete",
                        message = buildSuccessMessage(
                            exportType = request.exportType,
                            rangeDays = request.rangeDays,
                            path = executionResult.filePath
                        ),
                        exportPreview = executionResult.exportPreview,
                        lastExportPath = executionResult.filePath,
                        progressCurrent = executionResult.progressTotal,
                        progressTotal = executionResult.progressTotal
                    )
                },
                onFailure = {
                    setFailureState(it.message ?: "Export failed.")
                }
            )
        }
    }

    private fun applyProgressUpdate(update: ExportProgressUpdate) {
        setBusyPhase(
            phase = update.phase,
            title = update.title,
            message = update.message,
            progressCurrent = update.progressCurrent,
            progressTotal = update.progressTotal
        )
    }

    private fun buildSuccessMessage(
        exportType: ExportType,
        rangeDays: Int?,
        path: String
    ): String {
        return when (exportType) {
            ExportType.FULL_HISTORY -> "Full history exported successfully: $path"
            ExportType.ROLLING_WINDOW -> "Last ${rangeDays ?: "?"} days exported successfully: $path"
        }
    }

    private suspend fun applyCompletedStateWithPreview(
        title: String,
        message: String,
        exportPreview: String,
        lastExportPath: String?,
        progressCurrent: Int?,
        progressTotal: Int?
    ) {
        val previewSummary = exportPreviewSummaryParser.parse(exportPreview)

        uiState = uiState.copy(
            statusTitle = title,
            statusMessage = message,
            currentPhase = ExportPhase.COMPLETED,
            progressCurrent = progressCurrent,
            progressTotal = progressTotal,
            exportPreview = exportPreview,
            previewSummary = previewSummary ?: uiState.previewSummary,
            previewFullText = exportPreview,
            previewHighlightedFullChunks = emptyList(),
            isPreviewLoading = true,
            previewLoadingMessage = "Generating highlighted preview...",
            isBusy = false,
            lastExportPath = lastExportPath
        )

        val highlightedChunks = withContext(Dispatchers.Default) {
            exportPreviewChunkBuilder.build(exportPreview)
        }

        uiState = uiState.copy(
            previewHighlightedFullChunks = highlightedChunks,
            isPreviewLoading = false,
            previewLoadingMessage = null
        )
    }

    private fun setBusyPhase(
        phase: ExportPhase,
        title: String,
        message: String,
        progressCurrent: Int? = null,
        progressTotal: Int? = null
    ) {
        uiState = uiState.copy(
            statusTitle = title,
            statusMessage = message,
            currentPhase = phase,
            progressCurrent = progressCurrent,
            progressTotal = progressTotal,
            isBusy = true
        )
    }

    private fun setIdleState(
        title: String,
        message: String,
        phase: ExportPhase = ExportPhase.IDLE,
        lastExportPath: String? = uiState.lastExportPath
    ) {
        uiState = uiState.copy(
            statusTitle = title,
            statusMessage = message,
            currentPhase = phase,
            progressCurrent = null,
            progressTotal = null,
            isBusy = false,
            lastExportPath = lastExportPath
        )
    }

    private fun setCompletedState(
        title: String,
        message: String,
        exportPreview: String? = uiState.exportPreview,
        lastExportPath: String? = uiState.lastExportPath,
        progressCurrent: Int? = null,
        progressTotal: Int? = null
    ) {
        val previewSummary = exportPreview?.let { exportPreviewSummaryParser.parse(it) }

        uiState = uiState.copy(
            statusTitle = title,
            statusMessage = message,
            currentPhase = ExportPhase.COMPLETED,
            progressCurrent = progressCurrent,
            progressTotal = progressTotal,
            exportPreview = exportPreview,
            previewSummary = previewSummary ?: uiState.previewSummary,
            previewFullText = exportPreview ?: uiState.previewFullText,
            previewHighlightedFullChunks = if (exportPreview != null) emptyList() else uiState.previewHighlightedFullChunks,
            isPreviewLoading = false,
            previewLoadingMessage = null,
            isBusy = false,
            lastExportPath = lastExportPath
        )
    }

    private fun setFailureState(message: String) {
        uiState = uiState.copy(
            statusTitle = "Operation failed",
            statusMessage = message,
            currentPhase = ExportPhase.FAILED,
            progressCurrent = null,
            progressTotal = null,
            isBusy = false,
            isPreviewLoading = false,
            previewLoadingMessage = null
        )
    }
}