package ch.claudiowanner.healthdataexporter.ui

import androidx.compose.foundation.layout.Arrangement
import androidx.compose.foundation.layout.Column
import androidx.compose.foundation.layout.fillMaxSize
import androidx.compose.foundation.layout.padding
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
    onCreateTestExport: () -> Result<String>
) {
    val status = remember { mutableStateOf("No export created yet.") }

    Column(
        modifier = Modifier
            .fillMaxSize()
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

        Text(
            text = status.value,
            style = MaterialTheme.typography.bodyMedium
        )
    }
}