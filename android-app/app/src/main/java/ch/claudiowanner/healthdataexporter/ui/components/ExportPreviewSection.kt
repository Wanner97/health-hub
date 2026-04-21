package ch.claudiowanner.healthdataexporter.ui.components

import androidx.compose.foundation.background
import androidx.compose.foundation.border
import androidx.compose.foundation.horizontalScroll
import androidx.compose.foundation.layout.Arrangement
import androidx.compose.foundation.layout.Box
import androidx.compose.foundation.layout.Column
import androidx.compose.foundation.layout.fillMaxWidth
import androidx.compose.foundation.layout.heightIn
import androidx.compose.foundation.layout.padding
import androidx.compose.foundation.lazy.LazyColumn
import androidx.compose.foundation.lazy.items
import androidx.compose.foundation.rememberScrollState
import androidx.compose.foundation.shape.RoundedCornerShape
import androidx.compose.foundation.text.selection.SelectionContainer
import androidx.compose.foundation.verticalScroll
import androidx.compose.material3.AlertDialog
import androidx.compose.material3.Button
import androidx.compose.material3.LinearProgressIndicator
import androidx.compose.material3.MaterialTheme
import androidx.compose.material3.Text
import androidx.compose.material3.TextButton
import androidx.compose.runtime.Composable
import androidx.compose.runtime.mutableStateOf
import androidx.compose.runtime.remember
import androidx.compose.runtime.getValue
import androidx.compose.runtime.setValue
import androidx.compose.ui.Modifier
import androidx.compose.ui.text.font.FontFamily
import androidx.compose.ui.unit.dp
import ch.claudiowanner.healthdataexporter.ui.ExportUiState
import ch.claudiowanner.healthdataexporter.ui.PreviewDisplayMode

@Composable
fun ExportPreviewSection(
    uiState: ExportUiState,
    onLoadFullPreview: () -> Unit,
    onShowSnippetPreview: () -> Unit,
    modifier: Modifier = Modifier
) {
    val snippetVerticalScrollState = rememberScrollState()
    val snippetHorizontalScrollState = rememberScrollState()
    val summary = uiState.previewSummary

    var showFullPreviewDialog by remember { mutableStateOf(false) }

    if (showFullPreviewDialog) {
        AlertDialog(
            onDismissRequest = { showFullPreviewDialog = false },
            title = {
                Text("Load full JSON preview?")
            },
            text = {
                Text(
                    "Large export files can take noticeable time to process and render. " +
                            "During this time, the preview may need a moment to become ready."
                )
            },
            confirmButton = {
                TextButton(
                    onClick = {
                        showFullPreviewDialog = false
                        onLoadFullPreview()
                    }
                ) {
                    Text("Continue")
                }
            },
            dismissButton = {
                TextButton(
                    onClick = {
                        showFullPreviewDialog = false
                    }
                ) {
                    Text("Cancel")
                }
            }
        )
    }

    Column(
        modifier = modifier,
        verticalArrangement = Arrangement.spacedBy(8.dp)
    ) {
        Text(
            text = "Export preview",
            style = MaterialTheme.typography.titleMedium,
            color = MaterialTheme.colorScheme.onSurface
        )

        if (summary != null) {
            Column(
                verticalArrangement = Arrangement.spacedBy(4.dp)
            ) {
                summary.exportType?.let {
                    Text(
                        text = "Export type: $it",
                        style = MaterialTheme.typography.bodySmall,
                        color = MaterialTheme.colorScheme.onSurface
                    )
                }

                summary.rangeDescription?.let {
                    Text(
                        text = "Range: $it",
                        style = MaterialTheme.typography.bodySmall,
                        color = MaterialTheme.colorScheme.onSurface
                    )
                }

                summary.activityRecordCount?.let {
                    Text(
                        text = "Activity records: $it",
                        style = MaterialTheme.typography.bodySmall,
                        color = MaterialTheme.colorScheme.onSurface
                    )
                }

                summary.sleepSessionCount?.let {
                    Text(
                        text = "Sleep sessions: $it",
                        style = MaterialTheme.typography.bodySmall,
                        color = MaterialTheme.colorScheme.onSurface
                    )
                }

                summary.heartRateDailyCount?.let {
                    Text(
                        text = "Heart rate daily records: $it",
                        style = MaterialTheme.typography.bodySmall,
                        color = MaterialTheme.colorScheme.onSurface
                    )
                }

                summary.heartRateHourlyCount?.let {
                    Text(
                        text = "Heart rate hourly records: $it",
                        style = MaterialTheme.typography.bodySmall,
                        color = MaterialTheme.colorScheme.onSurface
                    )
                }
            }
        }

        if (uiState.previewDisplayMode == PreviewDisplayMode.SNIPPET) {
            Button(
                onClick = { showFullPreviewDialog = true },
                enabled = !uiState.isPreviewLoading && uiState.canLoadFullPreview
            ) {
                Text("Load full JSON preview")
            }
        } else {
            Button(
                onClick = onShowSnippetPreview,
                enabled = !uiState.isPreviewLoading
            ) {
                Text("Show short preview")
            }
        }

        Box(
            modifier = Modifier
                .fillMaxWidth()
                .heightIn(min = 180.dp, max = 420.dp)
                .border(
                    width = 1.dp,
                    color = MaterialTheme.colorScheme.outlineVariant,
                    shape = RoundedCornerShape(12.dp)
                )
                .background(
                    color = MaterialTheme.colorScheme.surfaceVariant,
                    shape = RoundedCornerShape(12.dp)
                )
                .padding(12.dp)
        ) {
            when {
                uiState.isPreviewLoading -> {
                    Column(
                        verticalArrangement = Arrangement.spacedBy(8.dp)
                    ) {
                        LinearProgressIndicator(
                            modifier = Modifier.fillMaxWidth()
                        )

                        Text(
                            text = uiState.previewLoadingMessage ?: "Preparing preview...",
                            style = MaterialTheme.typography.bodySmall,
                            color = MaterialTheme.colorScheme.onSurfaceVariant
                        )
                    }
                }

                uiState.previewDisplayMode == PreviewDisplayMode.FULL &&
                        uiState.previewHighlightedFullChunks.isNotEmpty() -> {
                    LazyColumn(
                        verticalArrangement = Arrangement.spacedBy(8.dp)
                    ) {
                        items(
                            items = uiState.previewHighlightedFullChunks,
                            key = { it.id }
                        ) { chunk ->
                            val horizontalScrollState = rememberScrollState()

                            Text(
                                text = chunk.content,
                                style = MaterialTheme.typography.bodySmall,
                                fontFamily = FontFamily.Monospace,
                                color = MaterialTheme.colorScheme.onSurfaceVariant,
                                modifier = Modifier
                                    .fillMaxWidth()
                                    .horizontalScroll(horizontalScrollState)
                            )
                        }
                    }
                }

                else -> {
                    SelectionContainer {
                        Text(
                            text = uiState.previewShortText,
                            style = MaterialTheme.typography.bodySmall,
                            fontFamily = FontFamily.Monospace,
                            color = MaterialTheme.colorScheme.onSurfaceVariant,
                            modifier = Modifier
                                .verticalScroll(snippetVerticalScrollState)
                                .horizontalScroll(snippetHorizontalScrollState)
                        )
                    }
                }
            }
        }
    }
}