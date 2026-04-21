package ch.claudiowanner.healthdataexporter.ui.components

import androidx.compose.foundation.layout.Arrangement
import androidx.compose.foundation.layout.Column
import androidx.compose.runtime.Composable
import androidx.compose.ui.Modifier
import androidx.compose.ui.unit.dp
import androidx.compose.material3.Button
import androidx.compose.material3.Text
import ch.claudiowanner.healthdataexporter.config.ExportConfig

@Composable
fun ExportActionsSection(
    isBusy: Boolean,
    onRequestHealthPermissions: () -> Unit,
    onExportFullHistory: () -> Unit,
    onExportRollingWindow: (Int) -> Unit,
    onLoadLatestExport: () -> Unit,
    onSaveLatestExportToDevice: () -> Unit,
    onShareLatestExport: () -> Unit,
    modifier: Modifier = Modifier
) {
    Column(
        modifier = modifier,
        verticalArrangement = Arrangement.spacedBy(16.dp)
    ) {
        Button(
            onClick = onRequestHealthPermissions,
            enabled = !isBusy
        ) {
            Text("Request health permissions")
        }

        Button(
            onClick = onExportFullHistory,
            enabled = !isBusy
        ) {
            Text("Export full history")
        }

        ExportConfig.SUPPORTED_ROLLING_WINDOWS.forEach { days ->
            Button(
                onClick = { onExportRollingWindow(days) },
                enabled = !isBusy
            ) {
                Text("Export last $days days")
            }
        }

        Button(
            onClick = onLoadLatestExport,
            enabled = !isBusy
        ) {
            Text("Load latest export")
        }

        Button(
            onClick = onSaveLatestExportToDevice,
            enabled = !isBusy
        ) {
            Text("Save latest export to device")
        }

        Button(
            onClick = onShareLatestExport,
            enabled = !isBusy
        ) {
            Text("Share latest export")
        }
    }
}