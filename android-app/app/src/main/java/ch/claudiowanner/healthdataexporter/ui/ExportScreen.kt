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
import androidx.compose.runtime.mutableStateOf
import androidx.compose.runtime.remember
import androidx.compose.ui.Modifier
import androidx.compose.ui.unit.dp

@Composable
fun ExportScreen(
    onCreateTestExport: () -> Result<String>,
    onLoadLatestExport: () -> Result<Pair<String, String>>
) {
    val status = remember { mutableStateOf("No export created yet.") }
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
            text = "This app will later read health data and export it as JSON. For now, we test the export flow with sample data.",
            style = MaterialTheme.typography.bodyLarge
        )

        Button(
            onClick = {
                val result = onCreateTestExport()
                status.value = result.fold(
                    onSuccess = { "Export saved successfully: $it" },
                    onFailure = { "Export failed: ${it.message}" }
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
                        status.value = "Latest export loaded: $path"
                        exportPreview.value = content
                    },
                    onFailure = {
                        status.value = "Loading export failed: ${it.message}"
                    }
                )
            }
        ) {
            Text("Load latest export")
        }

        Text(
            text = status.value,
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
    }
}