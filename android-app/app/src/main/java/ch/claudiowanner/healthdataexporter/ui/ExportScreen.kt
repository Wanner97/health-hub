package ch.claudiowanner.healthdataexporter.ui

import androidx.compose.foundation.layout.Arrangement
import androidx.compose.foundation.layout.Box
import androidx.compose.foundation.layout.Column
import androidx.compose.foundation.layout.WindowInsets
import androidx.compose.foundation.layout.fillMaxSize
import androidx.compose.foundation.layout.padding
import androidx.compose.foundation.layout.safeDrawing
import androidx.compose.foundation.layout.windowInsetsPadding
import androidx.compose.foundation.rememberScrollState
import androidx.compose.foundation.verticalScroll
import androidx.compose.material3.MaterialTheme
import androidx.compose.material3.Text
import androidx.compose.runtime.Composable
import androidx.compose.ui.Modifier
import androidx.compose.ui.unit.dp
import ch.claudiowanner.healthdataexporter.ui.components.ExportActionsSection
import ch.claudiowanner.healthdataexporter.ui.components.ExportPreviewSection
import ch.claudiowanner.healthdataexporter.ui.components.ExportStatusSection

@Composable
fun ExportScreen(
    uiState: ExportUiState,
    onRequestHealthPermissions: () -> Unit,
    onExportFullHistory: () -> Unit,
    onExportRollingWindow: (Int) -> Unit,
    onLoadLatestExport: () -> Unit,
    onSaveLatestExportToDevice: () -> Unit,
    onShareLatestExport: () -> Unit,
    onLoadFullPreview: () -> Unit,
    onShowSnippetPreview: () -> Unit
) {
    Box(
        modifier = Modifier
            .fillMaxSize()
            .windowInsetsPadding(WindowInsets.safeDrawing)
    ) {
        Column(
            modifier = Modifier
                .fillMaxSize()
                .verticalScroll(rememberScrollState())
                .padding(24.dp),
            verticalArrangement = Arrangement.spacedBy(16.dp)
        ) {
            Text(
                text = "Health Data Exporter",
                style = MaterialTheme.typography.headlineMedium
            )

            Text(
                text = "This app exports personal health data from Health Connect as JSON.",
                style = MaterialTheme.typography.bodyLarge
            )

            ExportActionsSection(
                isBusy = uiState.isBusy,
                onRequestHealthPermissions = onRequestHealthPermissions,
                onExportFullHistory = onExportFullHistory,
                onExportRollingWindow = onExportRollingWindow,
                onLoadLatestExport = onLoadLatestExport,
                onSaveLatestExportToDevice = onSaveLatestExportToDevice,
                onShareLatestExport = onShareLatestExport
            )

            ExportStatusSection(
                uiState = uiState
            )

            ExportPreviewSection(
                uiState = uiState,
                onLoadFullPreview = onLoadFullPreview,
                onShowSnippetPreview = onShowSnippetPreview
            )
        }
    }
}