package ch.claudiowanner.healthdataexporter

import android.content.Intent
import android.os.Bundle
import androidx.activity.ComponentActivity
import androidx.activity.compose.setContent
import androidx.activity.result.contract.ActivityResultContracts
import androidx.core.content.FileProvider
import androidx.lifecycle.ViewModelProvider
import androidx.lifecycle.lifecycleScope
import ch.claudiowanner.healthdataexporter.healthconnect.HealthConnectManager
import ch.claudiowanner.healthdataexporter.ui.ExportScreen
import ch.claudiowanner.healthdataexporter.ui.ExportViewModel
import ch.claudiowanner.healthdataexporter.ui.theme.HealthDataExporterTheme
import kotlinx.coroutines.launch
import androidx.health.connect.client.PermissionController

class MainActivity : ComponentActivity() {
    private lateinit var viewModel: ExportViewModel

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

        viewModel = ViewModelProvider(this)[ExportViewModel::class.java]

        setContent {
            val uiState = viewModel.uiState

            HealthDataExporterTheme {
                ExportScreen(
                    statusText = uiState.statusText,
                    exportPreview = uiState.exportPreview,
                    isBusy = uiState.isBusy,
                    onRequestHealthPermissions = { requestHealthPermissions() },
                    onExportFullHistory = { viewModel.exportFullHistory() },
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
                requestPermissions.launch(HealthConnectManager.PERMISSIONS)
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

        viewModel.onShareSheetOpened()
    }
}