package ch.claudiowanner.healthdataexporter.ui

import android.app.Application
import android.net.Uri
import androidx.compose.runtime.getValue
import androidx.compose.runtime.mutableStateOf
import androidx.compose.runtime.setValue
import androidx.health.connect.client.HealthConnectClient
import androidx.lifecycle.AndroidViewModel
import androidx.lifecycle.viewModelScope
import ch.claudiowanner.healthdataexporter.healthconnect.HealthConnectManager
import ch.claudiowanner.healthdataexporter.model.ExportPayload
import ch.claudiowanner.healthdataexporter.storage.ExportFileWriter
import kotlinx.coroutines.launch
import java.io.File
import java.time.Instant
import java.time.LocalDate

class ExportViewModel(application: Application) : AndroidViewModel(application) {
    private val appContext = application.applicationContext
    private val exportFileWriter = ExportFileWriter()
    private val healthConnectManager = HealthConnectManager(appContext)

    var uiState by mutableStateOf(ExportUiState())
        private set

    suspend fun shouldLaunchPermissionRequest(): Boolean {
        return when (healthConnectManager.getSdkStatus()) {
            HealthConnectClient.SDK_AVAILABLE -> {
                val alreadyGranted = healthConnectManager.hasAllPermissions()
                if (alreadyGranted) {
                    updateStatus("Health permissions already granted.")
                    false
                } else {
                    true
                }
            }

            HealthConnectClient.SDK_UNAVAILABLE -> {
                updateStatus("Health Connect is unavailable on this device.")
                false
            }

            HealthConnectClient.SDK_UNAVAILABLE_PROVIDER_UPDATE_REQUIRED -> {
                updateStatus("Health Connect requires an update before permissions can be requested.")
                false
            }

            else -> {
                updateStatus("Health Connect status is unknown.")
                false
            }
        }
    }

    fun onPermissionsResult(granted: Set<String>) {
        updateStatus(
            if (granted.containsAll(HealthConnectManager.PERMISSIONS)) {
                "Health permissions granted."
            } else {
                "Not all requested permissions were granted."
            }
        )
    }

    fun exportFullHistory() {
        viewModelScope.launch {
            setBusy(true)

            try {
                when (healthConnectManager.getSdkStatus()) {
                    HealthConnectClient.SDK_AVAILABLE -> {
                        if (!healthConnectManager.hasAllPermissions()) {
                            updateStatus("Export failed: required permissions are missing.")
                            return@launch
                        }

                        val records = healthConnectManager.readActivityExportRecordsForFullHistory()

                        if (records.isEmpty()) {
                            updateStatus("No step or distance data found.")
                            return@launch
                        }

                        val payload = ExportPayload(
                            exportVersion = 2,
                            source = "health-connect",
                            exportedAt = Instant.now().toString(),
                            rangeStart = records.first().startTime,
                            rangeEnd = records.last().endTime,
                            records = records
                        )

                        val result = exportFileWriter.writeExport(appContext, payload)

                        result.fold(
                            onSuccess = { (file, content) ->
                                uiState = uiState.copy(
                                    statusText = "Full history exported successfully: ${file.absolutePath}",
                                    exportPreview = content
                                )
                            },
                            onFailure = {
                                updateStatus("Export failed: ${it.message}")
                            }
                        )
                    }

                    HealthConnectClient.SDK_UNAVAILABLE -> {
                        updateStatus("Export failed: Health Connect is unavailable on this device.")
                    }

                    HealthConnectClient.SDK_UNAVAILABLE_PROVIDER_UPDATE_REQUIRED -> {
                        updateStatus("Export failed: Health Connect requires an update.")
                    }

                    else -> {
                        updateStatus("Export failed: Health Connect status is unknown.")
                    }
                }
            } catch (e: Exception) {
                updateStatus("Export failed: ${e.message}")
            } finally {
                setBusy(false)
            }
        }
    }

    fun loadLatestExport() {
        val result = exportFileWriter.readLatestExport(appContext)

        result.fold(
            onSuccess = { (file, content) ->
                uiState = uiState.copy(
                    statusText = "Latest export loaded: ${file.absolutePath}",
                    exportPreview = content
                )
            },
            onFailure = {
                updateStatus("Loading export failed: ${it.message}")
            }
        )
    }

    fun canLaunchSaveDialog(): Boolean {
        if (uiState.exportPreview == null) {
            updateStatus("No export is loaded yet.")
            return false
        }

        return true
    }

    fun suggestedExportFileName(): String {
        val today = LocalDate.now().toString()
        return "activity-export-$today.json"
    }

    fun onSaveCancelled() {
        updateStatus("Saving export was cancelled.")
    }

    fun saveLatestExportToUri(uri: Uri) {
        val preview = uiState.exportPreview
        if (preview == null) {
            updateStatus("No export is loaded yet.")
            return
        }

        val result = exportFileWriter.writeJsonToUri(
            context = appContext,
            uri = uri,
            jsonContent = preview
        )

        result.fold(
            onSuccess = {
                updateStatus("Export saved to selected location successfully.")
            },
            onFailure = {
                updateStatus("Saving export failed: ${it.message}")
            }
        )
    }

    fun getLatestExportFileForSharing(): File? {
        val latestFile = exportFileWriter.getLatestExportFile(appContext)

        if (latestFile == null) {
            updateStatus("No export file found to share.")
        }

        return latestFile
    }

    fun onShareSheetOpened() {
        updateStatus("Opened share sheet for latest export.")
    }

    private fun updateStatus(message: String) {
        uiState = uiState.copy(statusText = message)
    }

    private fun setBusy(isBusy: Boolean) {
        uiState = uiState.copy(isBusy = isBusy)
    }
}