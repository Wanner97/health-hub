package ch.claudiowanner.healthdataexporter

import android.os.Bundle
import androidx.activity.ComponentActivity
import androidx.activity.compose.setContent
import androidx.activity.result.contract.ActivityResultContracts
import androidx.compose.runtime.getValue
import androidx.compose.runtime.mutableStateOf
import androidx.compose.runtime.setValue
import androidx.health.connect.client.HealthConnectClient
import androidx.health.connect.client.PermissionController
import androidx.lifecycle.lifecycleScope
import ch.claudiowanner.healthdataexporter.healthconnect.HealthConnectManager
import ch.claudiowanner.healthdataexporter.storage.ExportFileWriter
import ch.claudiowanner.healthdataexporter.ui.ExportScreen
import ch.claudiowanner.healthdataexporter.ui.theme.HealthDataExporterTheme
import kotlinx.coroutines.launch

class MainActivity : ComponentActivity() {
    private val exportFileWriter = ExportFileWriter()
    private lateinit var healthConnectManager: HealthConnectManager

    private var healthConnectStatus by mutableStateOf("Health Connect not checked yet.")
    private var stepsText by mutableStateOf("No step data loaded yet.")

    private val requestPermissions =
        registerForActivityResult(
            PermissionController.createRequestPermissionResultContract()
        ) { granted ->
            if (granted.containsAll(HealthConnectManager.PERMISSIONS)) {
                healthConnectStatus = "Steps permission granted."
            } else {
                healthConnectStatus = "Steps permission was not granted."
            }
        }

    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)

        healthConnectManager = HealthConnectManager(this)

        setContent {
            HealthDataExporterTheme {
                ExportScreen(
                    onCreateTestExport = {
                        exportFileWriter.writeTestExport(this).map { file ->
                            file.absolutePath
                        }
                    },
                    onLoadLatestExport = {
                        exportFileWriter.readLatestExport(this).map { (file, content) ->
                            file.absolutePath to content
                        }
                    },
                    onCheckHealthConnect = {
                        healthConnectStatus = when (healthConnectManager.getSdkStatus()) {
                            HealthConnectClient.SDK_AVAILABLE ->
                                "Health Connect is available."

                            HealthConnectClient.SDK_UNAVAILABLE ->
                                "Health Connect is unavailable on this device."

                            HealthConnectClient.SDK_UNAVAILABLE_PROVIDER_UPDATE_REQUIRED ->
                                "Health Connect is installed, but requires an update."

                            else ->
                                "Health Connect status is unknown."
                        }
                    },
                    onRequestStepsPermission = {
                        when (healthConnectManager.getSdkStatus()) {
                            HealthConnectClient.SDK_AVAILABLE -> {
                                lifecycleScope.launch {
                                    val alreadyGranted = healthConnectManager.hasAllPermissions()
                                    if (alreadyGranted) {
                                        healthConnectStatus = "Steps permission already granted."
                                    } else {
                                        requestPermissions.launch(HealthConnectManager.PERMISSIONS)
                                    }
                                }
                            }

                            HealthConnectClient.SDK_UNAVAILABLE -> {
                                healthConnectStatus = "Health Connect is unavailable on this device."
                            }

                            HealthConnectClient.SDK_UNAVAILABLE_PROVIDER_UPDATE_REQUIRED -> {
                                healthConnectStatus = "Health Connect requires an update before permissions can be requested."
                            }

                            else -> {
                                healthConnectStatus = "Health Connect status is unknown."
                            }
                        }
                    },
                    onReadTodaySteps = {
                        when (healthConnectManager.getSdkStatus()) {
                            HealthConnectClient.SDK_AVAILABLE -> {
                                lifecycleScope.launch {
                                    try {
                                        if (!healthConnectManager.hasAllPermissions()) {
                                            healthConnectStatus = "Steps permission is missing."
                                            return@launch
                                        }

                                        val steps = healthConnectManager.readTodaySteps()
                                        healthConnectStatus = "Steps loaded successfully."
                                        stepsText = "Today's steps: $steps"
                                    } catch (e: Exception) {
                                        healthConnectStatus = "Reading steps failed: ${e.message}"
                                    }
                                }
                            }

                            HealthConnectClient.SDK_UNAVAILABLE -> {
                                healthConnectStatus = "Health Connect is unavailable on this device."
                            }

                            HealthConnectClient.SDK_UNAVAILABLE_PROVIDER_UPDATE_REQUIRED -> {
                                healthConnectStatus = "Health Connect requires an update before steps can be read."
                            }

                            else -> {
                                healthConnectStatus = "Health Connect status is unknown."
                            }
                        }
                    },
                    healthConnectStatus = healthConnectStatus,
                    stepsText = stepsText
                )
            }
        }
    }
}