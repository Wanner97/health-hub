package ch.claudiowanner.healthdataexporter

import android.os.Bundle
import androidx.activity.ComponentActivity
import androidx.activity.compose.setContent
import androidx.compose.runtime.getValue
import androidx.compose.runtime.mutableStateOf
import androidx.compose.runtime.setValue
import androidx.health.connect.client.HealthConnectClient
import androidx.health.connect.client.PermissionController
import androidx.lifecycle.lifecycleScope
import ch.claudiowanner.healthdataexporter.healthconnect.HealthConnectManager
import ch.claudiowanner.healthdataexporter.model.ExportPayload
import ch.claudiowanner.healthdataexporter.storage.ExportFileWriter
import ch.claudiowanner.healthdataexporter.ui.ExportScreen
import ch.claudiowanner.healthdataexporter.ui.theme.HealthDataExporterTheme
import kotlinx.coroutines.launch
import java.time.Instant

class MainActivity : ComponentActivity() {
    private val exportFileWriter = ExportFileWriter()
    private lateinit var healthConnectManager: HealthConnectManager

    private var statusText by mutableStateOf("No export created yet.")
    private var exportPreview by mutableStateOf("No export loaded yet.")

    private val requestPermissions =
        registerForActivityResult(
            PermissionController.createRequestPermissionResultContract()
        ) { granted ->
            statusText = if (granted.containsAll(HealthConnectManager.PERMISSIONS)) {
                "Steps permission granted."
            } else {
                "Steps permission was not granted."
            }
        }

    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)

        healthConnectManager = HealthConnectManager(this)

        setContent {
            HealthDataExporterTheme {
                ExportScreen(
                    statusText = statusText,
                    exportPreview = exportPreview,
                    onRequestStepsPermission = { requestStepsPermission() },
                    onExportLast7DaysSteps = { exportLast7DaysSteps() },
                    onLoadLatestExport = { loadLatestExport() }
                )
            }
        }
    }

    private fun requestStepsPermission() {
        when (healthConnectManager.getSdkStatus()) {
            HealthConnectClient.SDK_AVAILABLE -> {
                lifecycleScope.launch {
                    val alreadyGranted = healthConnectManager.hasAllPermissions()
                    if (alreadyGranted) {
                        statusText = "Steps permission already granted."
                    } else {
                        requestPermissions.launch(HealthConnectManager.PERMISSIONS)
                    }
                }
            }

            HealthConnectClient.SDK_UNAVAILABLE -> {
                statusText = "Health Connect is unavailable on this device."
            }

            HealthConnectClient.SDK_UNAVAILABLE_PROVIDER_UPDATE_REQUIRED -> {
                statusText = "Health Connect requires an update before permissions can be requested."
            }

            else -> {
                statusText = "Health Connect status is unknown."
            }
        }
    }

    private fun exportLast7DaysSteps() {
        lifecycleScope.launch {
            try {
                when (healthConnectManager.getSdkStatus()) {
                    HealthConnectClient.SDK_AVAILABLE -> {
                        if (!healthConnectManager.hasAllPermissions()) {
                            statusText = "Exporting step data failed: Steps permission is missing."
                            return@launch
                        }

                        val records = healthConnectManager.readStepExportRecordsForLastDays(7)

                        val payload = ExportPayload(
                            exportVersion = 1,
                            source = "health-connect",
                            exportedAt = Instant.now().toString(),
                            records = records
                        )

                        val result = exportFileWriter.writeExport(this@MainActivity, payload)

                        result.fold(
                            onSuccess = { (file, content) ->
                                statusText = "Last 7 days exported successfully: ${file.absolutePath}"
                                exportPreview = content
                            },
                            onFailure = {
                                statusText = "Exporting step data failed: ${it.message}"
                            }
                        )
                    }

                    HealthConnectClient.SDK_UNAVAILABLE -> {
                        statusText = "Exporting step data failed: Health Connect is unavailable on this device."
                    }

                    HealthConnectClient.SDK_UNAVAILABLE_PROVIDER_UPDATE_REQUIRED -> {
                        statusText = "Exporting step data failed: Health Connect requires an update."
                    }

                    else -> {
                        statusText = "Exporting step data failed: Health Connect status is unknown."
                    }
                }
            } catch (e: Exception) {
                statusText = "Exporting step data failed: ${e.message}"
            }
        }
    }

    private fun loadLatestExport() {
        val result = exportFileWriter.readLatestExport(this)

        result.fold(
            onSuccess = { (file, content) ->
                statusText = "Latest export loaded: ${file.absolutePath}"
                exportPreview = content
            },
            onFailure = {
                statusText = "Loading export failed: ${it.message}"
            }
        )
    }
}