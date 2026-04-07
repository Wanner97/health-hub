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
import androidx.compose.material3.Button
import androidx.compose.material3.MaterialTheme
import androidx.compose.material3.Text
import androidx.compose.runtime.Composable
import androidx.compose.ui.Modifier
import androidx.compose.ui.unit.dp

@Composable
fun ExportScreen(
    statusText: String,
    exportPreview: String,
    onRequestHealthPermissions: () -> Unit,
    onExportFullHistory: () -> Unit,
    onLoadLatestExport: () -> Unit,
    onSaveLatestExportToDevice: () -> Unit,
    onShareLatestExport: () -> Unit
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
                text = "This app exports steps and distance from Health Connect as JSON.",
                style = MaterialTheme.typography.bodyLarge
            )

            Button(onClick = onRequestHealthPermissions) {
                Text("Request health permissions")
            }

            Button(onClick = onExportFullHistory) {
                Text("Export full history")
            }

            Button(onClick = onLoadLatestExport) {
                Text("Load latest export")
            }

            Button(onClick = onSaveLatestExportToDevice) {
                Text("Save latest export to device")
            }

            Button(onClick = onShareLatestExport) {
                Text("Share latest export")
            }

            Text(
                text = "Status",
                style = MaterialTheme.typography.titleMedium
            )

            Text(
                text = statusText,
                style = MaterialTheme.typography.bodyMedium
            )

            Text(
                text = "Export preview",
                style = MaterialTheme.typography.titleMedium
            )

            Text(
                text = exportPreview,
                style = MaterialTheme.typography.bodySmall
            )
        }
    }
}