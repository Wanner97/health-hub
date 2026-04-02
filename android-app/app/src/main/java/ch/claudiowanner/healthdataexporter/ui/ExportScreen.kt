package ch.claudiowanner.healthdataexporter.ui

import androidx.compose.foundation.layout.Arrangement
import androidx.compose.foundation.layout.Column
import androidx.compose.foundation.layout.fillMaxSize
import androidx.compose.foundation.layout.padding
import androidx.compose.foundation.rememberScrollState
import androidx.compose.foundation.verticalScroll
import androidx.compose.material3.Button
import androidx.compose.material3.MaterialTheme
import androidx.compose.material3.Text
import androidx.compose.runtime.Composable
import androidx.compose.runtime.MutableState
import androidx.compose.runtime.mutableStateOf
import androidx.compose.runtime.remember
import androidx.compose.ui.Modifier
import androidx.compose.ui.unit.dp

@Composable
fun ExportScreen(
    onCreateTestExport: () -> Result<String>,
    onLoadLatestExport: () -> Result<Pair<String, String>>,
    onExportTodaySteps: (MutableState<String>, MutableState<String>) -> Unit,
    onExportLast7DaysSteps: (MutableState<String>, MutableState<String>) -> Unit,
    onCheckHealthConnect: () -> Unit,
    onRequestStepsPermission: () -> Unit,
    onReadTodaySteps: () -> Unit,
    healthConnectStatus: String,
    stepsText: String
) {
    val exportStatus = remember { mutableStateOf("No export created yet.") }
    val exportPreview = remember { mutableStateOf("No export loaded yet.") }

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
            text = "This app will later read health data and export it as JSON. For now, we test the export flow and start integrating Health Connect step by step.",
            style = MaterialTheme.typography.bodyLarge
        )

        Button(
            onClick = {
                val result = onCreateTestExport()
                exportStatus.value = result.fold(
                    onSuccess = { "Test export saved successfully: $it" },
                    onFailure = { "Test export failed: ${it.message}" }
                )
            }
        ) {
            Text("Create test export")
        }

        Button(
            onClick = {
                val result = onLoadLatestExport()
                result.fold(
                    onSuccess = { (path, content) ->
                        exportStatus.value = "Latest export loaded: $path"
                        exportPreview.value = content
                    },
                    onFailure = {
                        exportStatus.value = "Loading export failed: ${it.message}"
                    }
                )
            }
        ) {
            Text("Load latest export")
        }

        Button(
            onClick = {
                onExportTodaySteps(exportStatus, exportPreview)
            }
        ) {
            Text("Export today's steps")
        }

        Button(
            onClick = {
                onExportLast7DaysSteps(exportStatus, exportPreview)
            }
        ) {
            Text("Export last 7 days")
        }

        Text(
            text = exportStatus.value,
            style = MaterialTheme.typography.bodyMedium
        )

        Text(
            text = "Export preview",
            style = MaterialTheme.typography.titleMedium
        )

        Text(
            text = exportPreview.value,
            style = MaterialTheme.typography.bodySmall
        )

        Text(
            text = "Health Connect",
            style = MaterialTheme.typography.titleMedium
        )

        Button(onClick = onCheckHealthConnect) {
            Text("Check Health Connect")
        }

        Button(onClick = onRequestStepsPermission) {
            Text("Request steps permission")
        }

        Button(onClick = onReadTodaySteps) {
            Text("Read today's steps")
        }

        Text(
            text = healthConnectStatus,
            style = MaterialTheme.typography.bodyMedium
        )

        Text(
            text = stepsText,
            style = MaterialTheme.typography.bodyLarge
        )
    }
}