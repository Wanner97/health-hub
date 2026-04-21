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
import ch.claudiowanner.healthdataexporter.model.ActivityExportCluster
import ch.claudiowanner.healthdataexporter.model.ExportClusters
import ch.claudiowanner.healthdataexporter.model.ExportPayload
import ch.claudiowanner.healthdataexporter.model.HeartRateDailyExportCluster
import ch.claudiowanner.healthdataexporter.model.HeartRateHourlyExportCluster
import ch.claudiowanner.healthdataexporter.model.SleepExportCluster
import ch.claudiowanner.healthdataexporter.model.VitalsExportCluster
import ch.claudiowanner.healthdataexporter.storage.ExportFileWriter
import kotlinx.coroutines.launch
import java.io.File
import java.time.Instant
import java.time.LocalDate
import java.time.ZoneId

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
        exportHistory(
            exportType = "full_history",
            rangeDays = null,
            startDateInclusive = LocalDate.of(2000, 1, 1)
        )
    }

    fun exportLastDays(days: Int) {
        require(days > 0) { "days must be greater than 0." }

        val zoneId = ZoneId.systemDefault()
        val today = LocalDate.now(zoneId)
        val startDateInclusive = today.minusDays((days - 1).toLong())

        exportHistory(
            exportType = "rolling_window",
            rangeDays = days,
            startDateInclusive = startDateInclusive
        )
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
        return "health-export-$today.json"
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

    private fun exportHistory(
        exportType: String,
        rangeDays: Int?,
        startDateInclusive: LocalDate
    ) {
        viewModelScope.launch {
            setBusy(true)

            try {
                when (healthConnectManager.getSdkStatus()) {
                    HealthConnectClient.SDK_AVAILABLE -> {
                        if (!healthConnectManager.hasAllPermissions()) {
                            updateStatus("Export failed: required permissions are missing.")
                            return@launch
                        }

                        val zoneId = ZoneId.systemDefault()
                        val now = Instant.now()
                        val today = LocalDate.now(zoneId)
                        val endDateExclusive = today.plusDays(1)
                        val rangeStartInstant = startDateInclusive.atStartOfDay(zoneId).toInstant()

                        val activityRecords = healthConnectManager.readActivityExportRecords(
                            startDateInclusive = startDateInclusive,
                            endDateExclusive = endDateExclusive
                        )

                        val sleepSessions = healthConnectManager.readSleepSessions(
                            startInstant = rangeStartInstant,
                            endInstant = now
                        )

                        val heartRateDailyRecords = healthConnectManager.readHeartRateDailyRecords(
                            startDateInclusive = startDateInclusive,
                            endDateExclusive = endDateExclusive
                        )

                        val heartRateHourlyRecords = healthConnectManager.readHeartRateHourlyRecords(
                            startInstant = rangeStartInstant,
                            endInstant = now
                        )

                        if (
                            activityRecords.isEmpty() &&
                            sleepSessions.isEmpty() &&
                            heartRateDailyRecords.isEmpty() &&
                            heartRateHourlyRecords.isEmpty()
                        ) {
                            updateStatus("No activity, sleep, or heart rate data found.")
                            return@launch
                        }

                        val payload = ExportPayload(
                            exportVersion = 4,
                            source = "health-connect",
                            exportedAt = now.toString(),
                            exportType = exportType,
                            rangeDays = rangeDays,
                            rangeStart = rangeStartInstant.toString(),
                            rangeEnd = now.toString(),
                            clusters = ExportClusters(
                                activity = ActivityExportCluster(
                                    records = activityRecords
                                ),
                                sleep = SleepExportCluster(
                                    sessions = sleepSessions
                                ),
                                vitals = VitalsExportCluster(
                                    heartRateDaily = HeartRateDailyExportCluster(
                                        records = heartRateDailyRecords
                                    ),
                                    heartRateHourly = HeartRateHourlyExportCluster(
                                        records = heartRateHourlyRecords
                                    )
                                )
                            )
                        )

                        val result = exportFileWriter.writeExport(appContext, payload)

                        result.fold(
                            onSuccess = { (file, content) ->
                                uiState = uiState.copy(
                                    statusText = buildSuccessMessage(exportType, rangeDays, file.absolutePath),
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

    private fun buildSuccessMessage(
        exportType: String,
        rangeDays: Int?,
        path: String
    ): String {
        return when (exportType) {
            "full_history" -> "Full history exported successfully: $path"
            "rolling_window" -> "Last ${rangeDays ?: "?"} days exported successfully: $path"
            else -> "Export completed successfully: $path"
        }
    }

    private fun updateStatus(message: String) {
        uiState = uiState.copy(statusText = message)
    }

    private fun setBusy(isBusy: Boolean) {
        uiState = uiState.copy(isBusy = isBusy)
    }
}