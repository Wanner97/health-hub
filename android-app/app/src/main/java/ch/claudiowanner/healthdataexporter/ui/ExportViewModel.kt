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
import ch.claudiowanner.healthdataexporter.export.ExportExecutionResult
import ch.claudiowanner.healthdataexporter.export.ExportOperationPhase
import ch.claudiowanner.healthdataexporter.export.ExportProgressUpdate
import ch.claudiowanner.healthdataexporter.export.ExportRequest
import ch.claudiowanner.healthdataexporter.healthconnect.HealthConnectAvailability
import ch.claudiowanner.healthdataexporter.healthconnect.HealthConnectManager
import ch.claudiowanner.healthdataexporter.serialization.ExportJsonSerializer
import ch.claudiowanner.healthdataexporter.storage.ExportFileWriter
import androidx.compose.ui.graphics.Color
import ch.claudiowanner.healthdataexporter.ui.components.JsonHighlightColors
import ch.claudiowanner.healthdataexporter.ui.components.buildHighlightedJson
import com.google.gson.JsonObject
import com.google.gson.JsonParser
import kotlinx.coroutines.Dispatchers
import kotlinx.coroutines.launch
import kotlinx.coroutines.withContext
import java.io.File
import java.time.LocalDate
import java.time.ZoneId

class ExportViewModel(application: Application) : AndroidViewModel(application) {
    private val appContext = application.applicationContext
    private val exportFileWriter = ExportFileWriter()
    private val healthConnectManager = HealthConnectManager(appContext)
    private val exportJsonSerializer = ExportJsonSerializer()
    private val exportCoordinator = ExportCoordinator(
        appContext = appContext,
        healthConnectManager = healthConnectManager,
        exportFileWriter = exportFileWriter,
        exportJsonSerializer = exportJsonSerializer
    )

    var uiState: ExportUiState by mutableStateOf(ExportUiState())
        private set

    suspend fun shouldLaunchPermissionRequest(): Boolean {
        setBusyPhase(
            phase = ExportProgressPhase.CHECKING_PERMISSIONS,
            title = "Checking permissions",
            message = "Checking Health Connect availability and existing permissions."
        )

        return when (healthConnectManager.getSdkStatus()) {
            HealthConnectClient.SDK_AVAILABLE -> {
                val alreadyGranted = healthConnectManager.hasAllPermissions()

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
                        phase = ExportProgressPhase.CHECKING_PERMISSIONS
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
                phase = ExportProgressPhase.LOADING_EXPORT,
                title = "Loading export",
                message = "Reading the latest export file from app storage.",
                progressCurrent = 1,
                progressTotal = 1
            )

            val result = exportFileWriter.readLatestExport(appContext)

            result.fold(
                onSuccess = { (file, content) ->
                    setCompletedState(
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
                phase = ExportProgressPhase.SAVING_EXPORT,
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
            phase = ExportProgressPhase.PREPARING_SHARE,
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
            phase = ExportProgressPhase.PREPARING_SHARE,
            lastExportPath = latestFile.absolutePath
        )

        return latestFile
    }

    fun onShareSheetOpened() {
        setIdleState(
            title = "Share sheet opened",
            message = "Share sheet opened for latest export.",
            phase = ExportProgressPhase.PREPARING_SHARE
        )
    }

    fun loadFullPreview() {
        val fullJson = uiState.exportPreview ?: return

        if (
            uiState.previewDisplayMode == PreviewDisplayMode.FULL &&
            uiState.previewHighlightedFullChunks.isNotEmpty()
        ) {
            return
        }

        viewModelScope.launch {
            uiState = uiState.copy(
                isPreviewLoading = true,
                previewLoadingMessage = "Generating highlighted preview...",
                canLoadFullPreview = false
            )

            val highlightedChunks = withContext(Dispatchers.Default) {
                buildHighlightedPreviewChunks(fullJson)
            }

            uiState = uiState.copy(
                previewDisplayMode = PreviewDisplayMode.FULL,
                previewFullText = fullJson,
                previewHighlightedFullChunks = highlightedChunks,
                isPreviewLoading = false,
                previewLoadingMessage = null,
                canLoadFullPreview = true
            )
        }
    }

    fun showPreviewSnippet() {
        uiState = uiState.copy(
            previewDisplayMode = PreviewDisplayMode.SNIPPET,
            isPreviewLoading = false,
            previewLoadingMessage = null
        )
    }

    private fun runExport(request: ExportRequest) {
        viewModelScope.launch {
            val result = exportCoordinator.executeExport(request) { update ->
                applyProgressUpdate(update)
            }

            result.fold(
                onSuccess = { executionResult ->
                    handleSuccessfulExport(request, executionResult)
                },
                onFailure = {
                    setFailureState(it.message ?: "Export failed.")
                }
            )
        }
    }

    private fun handleSuccessfulExport(
        request: ExportRequest,
        executionResult: ExportExecutionResult
    ) {
        setCompletedState(
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
    }

    private fun applyProgressUpdate(update: ExportProgressUpdate) {
        setBusyPhase(
            phase = update.phase.toUiPhase(),
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

    private fun createPreviewSnippet(previewContent: String): String {
        return if (previewContent.length <= ExportConfig.PREVIEW_SNIPPET_LENGTH) {
            previewContent
        } else {
            previewContent.take(ExportConfig.PREVIEW_SNIPPET_LENGTH) + "…"
        }
    }

    private fun buildPreviewSummary(previewContent: String): ExportPreviewSummary? {
        return runCatching {
            val root = JsonParser.parseString(previewContent).asJsonObject

            val exportType = root.stringOrNull("exportType")
            val rangeDays = root.intOrNull("rangeDays")
            val clusters = root.objectOrNull("clusters")

            val activityCount = clusters
                ?.objectOrNull("activity")
                ?.arraySizeOrNull("records")

            val sleepCount = clusters
                ?.objectOrNull("sleep")
                ?.arraySizeOrNull("sessions")

            val heartRateDailyCount = clusters
                ?.objectOrNull("vitals")
                ?.objectOrNull("heartRateDaily")
                ?.arraySizeOrNull("records")

            val heartRateHourlyCount = clusters
                ?.objectOrNull("vitals")
                ?.objectOrNull("heartRateHourly")
                ?.arraySizeOrNull("records")

            ExportPreviewSummary(
                exportType = exportType,
                rangeDescription = when {
                    exportType == ExportType.FULL_HISTORY.jsonValue -> "Full history"
                    rangeDays != null -> "Last $rangeDays days"
                    else -> null
                },
                activityRecordCount = activityCount,
                sleepSessionCount = sleepCount,
                heartRateDailyCount = heartRateDailyCount,
                heartRateHourlyCount = heartRateHourlyCount
            )
        }.getOrNull()
    }

    private fun buildHighlightedPreviewChunks(json: String): List<PreviewChunk> {
        val lines = json.lines()

        return lines
            .chunked(ExportConfig.FULL_PREVIEW_CHUNK_LINE_COUNT)
            .mapIndexed { index, chunkLines ->
                val chunkText = chunkLines.joinToString(separator = "\n")

                PreviewChunk(
                    id = index,
                    content = buildHighlightedJson(
                        json = chunkText,
                        colors = JsonHighlightColors(
                            keyColor = Color(0xFF4FC3F7),
                            stringColor = Color(0xFFCE93D8),
                            numberColor = Color(0xFFFFCC80),
                            literalColor = Color(0xFFEF9A9A)
                        )
                    )
                )
            }
    }

    private fun JsonObject.stringOrNull(name: String): String? {
        val value = get(name) ?: return null
        return if (value.isJsonNull) null else value.asString
    }

    private fun JsonObject.intOrNull(name: String): Int? {
        val value = get(name) ?: return null
        return if (value.isJsonNull) null else value.asInt
    }

    private fun JsonObject.objectOrNull(name: String): JsonObject? {
        val value = get(name) ?: return null
        return if (value.isJsonObject) value.asJsonObject else null
    }

    private fun JsonObject.arraySizeOrNull(name: String): Int? {
        val value = get(name) ?: return null
        return if (value.isJsonArray) value.asJsonArray.size() else null
    }

    private fun setBusyPhase(
        phase: ExportProgressPhase,
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
        phase: ExportProgressPhase = ExportProgressPhase.IDLE,
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
        val previewSummary = exportPreview?.let { buildPreviewSummary(it) }
        val previewShortText = exportPreview?.let { createPreviewSnippet(it) } ?: uiState.previewShortText

        uiState = uiState.copy(
            statusTitle = title,
            statusMessage = message,
            currentPhase = ExportProgressPhase.COMPLETED,
            progressCurrent = progressCurrent,
            progressTotal = progressTotal,
            exportPreview = exportPreview,
            previewSummary = previewSummary ?: uiState.previewSummary,
            previewDisplayMode = if (exportPreview != null) PreviewDisplayMode.SNIPPET else uiState.previewDisplayMode,
            previewShortText = previewShortText,
            previewFullText = if (exportPreview != null) null else uiState.previewFullText,
            previewHighlightedFullChunks = emptyList(),
            isPreviewLoading = false,
            previewLoadingMessage = null,
            canLoadFullPreview = !exportPreview.isNullOrBlank(),
            isBusy = false,
            lastExportPath = lastExportPath
        )
    }

    private fun setFailureState(message: String) {
        uiState = uiState.copy(
            statusTitle = "Operation failed",
            statusMessage = message,
            currentPhase = ExportProgressPhase.FAILED,
            progressCurrent = null,
            progressTotal = null,
            isBusy = false,
            isPreviewLoading = false,
            previewLoadingMessage = null
        )
    }
}

private fun ExportOperationPhase.toUiPhase(): ExportProgressPhase {
    return when (this) {
        ExportOperationPhase.CHECKING_PERMISSIONS -> ExportProgressPhase.CHECKING_PERMISSIONS
        ExportOperationPhase.READING_ACTIVITY -> ExportProgressPhase.READING_ACTIVITY
        ExportOperationPhase.READING_SLEEP -> ExportProgressPhase.READING_SLEEP
        ExportOperationPhase.READING_HEART_RATE_DAILY -> ExportProgressPhase.READING_HEART_RATE_DAILY
        ExportOperationPhase.READING_HEART_RATE_HOURLY -> ExportProgressPhase.READING_HEART_RATE_HOURLY
        ExportOperationPhase.BUILDING_EXPORT -> ExportProgressPhase.BUILDING_EXPORT
        ExportOperationPhase.WRITING_JSON -> ExportProgressPhase.WRITING_JSON
    }
}