package ch.claudiowanner.healthdataexporter

import android.content.Intent
import android.os.Bundle
import androidx.activity.ComponentActivity
import androidx.activity.compose.setContent
import androidx.activity.result.contract.ActivityResultContracts
import androidx.compose.runtime.getValue
import androidx.compose.runtime.mutableStateOf
import androidx.compose.runtime.setValue
import androidx.core.content.FileProvider
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
import java.time.LocalDate

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
                "Health permissions granted."
            } else {
                "Not all requested permissions were granted."
            }
        }

    private val createDocumentLauncher =
        registerForActivityResult(
            ActivityResultContracts.CreateDocument("application/json")
        ) { uri ->
            if (uri == null) {
                statusText = "Saving export was cancelled."
                return@registerForActivityResult
            }

            if (exportPreview == "No export loaded yet.") {
                statusText = "No export is loaded yet."
                return@registerForActivityResult
            }

            val result = exportFileWriter.writeJsonToUri(
                context = this,
                uri = uri,
                jsonContent = exportPreview
            )

            result.fold(
                onSuccess = {
                    statusText = "Export saved to selected location successfully."
                },
                onFailure = {
                    statusText = "Saving export failed: ${it.message}"
                }
            )
        }

    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)

        healthConnectManager = HealthConnectManager(this)

        setContent {
            HealthDataExporterTheme {
                ExportScreen(
                    statusText = statusText,
                    exportPreview = exportPreview,
                    onRequestHealthPermissions = { requestHealthPermissions() },
                    onExportFullHistory = { exportFullHistory() },
                    onLoadLatestExport = { loadLatestExport() },
                    onSaveLatestExportToDevice = { saveLatestExportToDevice() },
                    onShareLatestExport = { shareLatestExport() }
                )
            }
        }
    }

    private fun requestHealthPermissions() {
        when (healthConnectManager.getSdkStatus()) {
            HealthConnectClient.SDK_AVAILABLE -> {
                lifecycleScope.launch {
                    val alreadyGranted = healthConnectManager.hasAllPermissions()
                    if (alreadyGranted) {
                        statusText = "Health permissions already granted."
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

    private fun exportFullHistory() {
        lifecycleScope.launch {
            try {
                when (healthConnectManager.getSdkStatus()) {
                    HealthConnectClient.SDK_AVAILABLE -> {
                        if (!healthConnectManager.hasAllPermissions()) {
                            statusText = "Export failed: required permissions are missing."
                            return@launch
                        }

                        val records = healthConnectManager.readActivityExportRecordsForFullHistory()

                        if (records.isEmpty()) {
                            statusText = "No step or distance data found."
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

                        val result = exportFileWriter.writeExport(this@MainActivity, payload)

                        result.fold(
                            onSuccess = { (file, content) ->
                                statusText = "Full history exported successfully: ${file.absolutePath}"
                                exportPreview = content
                            },
                            onFailure = {
                                statusText = "Export failed: ${it.message}"
                            }
                        )
                    }

                    HealthConnectClient.SDK_UNAVAILABLE -> {
                        statusText = "Export failed: Health Connect is unavailable on this device."
                    }

                    HealthConnectClient.SDK_UNAVAILABLE_PROVIDER_UPDATE_REQUIRED -> {
                        statusText = "Export failed: Health Connect requires an update."
                    }

                    else -> {
                        statusText = "Export failed: Health Connect status is unknown."
                    }
                }
            } catch (e: Exception) {
                statusText = "Export failed: ${e.message}"
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

    private fun saveLatestExportToDevice() {
        if (exportPreview == "No export loaded yet.") {
            statusText = "No export is loaded yet."
            return
        }

        val today = LocalDate.now().toString()
        val fileName = "activity-export-$today.json"

        createDocumentLauncher.launch(fileName)
    }

    private fun shareLatestExport() {
        val latestFile = exportFileWriter.getLatestExportFile(this)

        if (latestFile == null) {
            statusText = "No export file found to share."
            return
        }

        val uri = FileProvider.getUriForFile(
            this,
            "ch.claudiowanner.healthdataexporter.fileprovider",
            latestFile
        )

        val shareIntent = Intent(Intent.ACTION_SEND).apply {
            type = "application/json"
            putExtra(Intent.EXTRA_STREAM, uri)
            addFlags(Intent.FLAG_GRANT_READ_URI_PERMISSION)
        }

        startActivity(
            Intent.createChooser(shareIntent, "Share latest export")
        )

        statusText = "Opened share sheet for latest export."
    }
}