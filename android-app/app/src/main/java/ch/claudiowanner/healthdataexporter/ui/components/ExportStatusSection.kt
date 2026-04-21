package ch.claudiowanner.healthdataexporter.ui.components

import androidx.compose.foundation.layout.fillMaxWidth
import androidx.compose.foundation.layout.Column
import androidx.compose.foundation.layout.Arrangement
import androidx.compose.runtime.Composable
import androidx.compose.ui.Modifier
import androidx.compose.ui.unit.dp
import androidx.compose.material3.LinearProgressIndicator
import androidx.compose.material3.MaterialTheme
import androidx.compose.material3.Text
import ch.claudiowanner.healthdataexporter.ui.ExportUiState

@Composable
fun ExportStatusSection(
    uiState: ExportUiState,
    modifier: Modifier = Modifier
) {
    Column(
        modifier = modifier,
        verticalArrangement = Arrangement.spacedBy(8.dp)
    ) {
        Text(
            text = "Status",
            style = MaterialTheme.typography.titleMedium
        )

        Text(
            text = uiState.statusTitle,
            style = MaterialTheme.typography.titleSmall
        )

        Text(
            text = uiState.statusMessage,
            style = MaterialTheme.typography.bodyMedium
        )

        if (uiState.isBusy) {
            if (uiState.hasDeterminateProgress) {
                val progress = (
                        uiState.progressCurrent!!.toFloat() /
                                uiState.progressTotal!!.toFloat()
                        ).coerceIn(0f, 1f)

                LinearProgressIndicator(
                    progress = { progress },
                    modifier = Modifier.fillMaxWidth()
                )

                Text(
                    text = "Step ${uiState.progressCurrent} of ${uiState.progressTotal}",
                    style = MaterialTheme.typography.bodySmall
                )
            } else {
                LinearProgressIndicator(
                    modifier = Modifier.fillMaxWidth()
                )
            }

            Text(
                text = uiState.currentPhase.label,
                style = MaterialTheme.typography.bodySmall
            )
        }

        if (!uiState.lastExportPath.isNullOrBlank()) {
            Text(
                text = "Latest file: ${uiState.lastExportPath}",
                style = MaterialTheme.typography.bodySmall
            )
        }
    }
}