package ch.claudiowanner.healthdataexporter

import android.content.Intent
import android.os.Bundle
import androidx.activity.ComponentActivity
import androidx.activity.compose.setContent
import androidx.activity.result.contract.ActivityResultContracts
import androidx.activity.viewModels
import androidx.core.content.FileProvider
import androidx.health.connect.client.PermissionController
import androidx.lifecycle.lifecycleScope
import ch.claudiowanner.healthdataexporter.config.ExportConfig
import ch.claudiowanner.healthdataexporter.healthconnect.HealthConnectAvailability
import ch.claudiowanner.healthdataexporter.ui.ExportScreen
import ch.claudiowanner.healthdataexporter.ui.ExportViewModel
import ch.claudiowanner.healthdataexporter.ui.theme.HealthDataExporterTheme
import kotlinx.coroutines.launch

class MainActivity : ComponentActivity() {

    private val viewModel: ExportViewModel by viewModels()

    private val requestPermissions =
        registerForActivityResult(
            PermissionController.createRequestPermissionResultContract()
        ) { granted ->
            viewModel.onPermissionsResult(granted)
        }

    private val createDocumentLauncher =
        registerForActivityResult(
            ActivityResultContracts.CreateDocument("application/json")
        ) { uri ->
            if (uri == null) {
                viewModel.onSaveCancelled()
                return@registerForActivityResult
            }

            viewModel.saveLatestExportToUri(uri)
        }

    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)

        setContent {
            val uiState = viewModel.uiState

            HealthDataExporterTheme {
                ExportScreen(
                    uiState = uiState,
                    onRequestHealthPermissions = { requestHealthPermissions() },
                    onExportFullHistory = { viewModel.exportFullHistory() },
                    onExportRollingWindow = { days -> viewModel.exportLastDays(days) },
                    onLoadLatestExport = { viewModel.loadLatestExport() },
                    onSaveLatestExportToDevice = { saveLatestExportToDevice() },
                    onShareLatestExport = { shareLatestExport() }
                )
            }
        }
    }

    private fun requestHealthPermissions() {
        lifecycleScope.launch {
            val shouldLaunch = viewModel.shouldLaunchPermissionRequest()
            if (shouldLaunch) {
                requestPermissions.launch(HealthConnectAvailability.PERMISSIONS)
            }
        }
    }

    private fun saveLatestExportToDevice() {
        if (!viewModel.canLaunchSaveDialog()) {
            return
        }

        createDocumentLauncher.launch(viewModel.suggestedExportFileName())
    }

    private fun shareLatestExport() {
        val latestFile = viewModel.getLatestExportFileForSharing() ?: return

        val uri = FileProvider.getUriForFile(
            this,
            ExportConfig.FILE_PROVIDER_AUTHORITY,
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

        viewModel.onShareSheetOpened()
    }
}