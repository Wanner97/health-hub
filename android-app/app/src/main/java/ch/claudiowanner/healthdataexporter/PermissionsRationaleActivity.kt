package ch.claudiowanner.healthdataexporter

import android.os.Bundle
import androidx.activity.ComponentActivity
import androidx.activity.compose.setContent
import androidx.compose.foundation.layout.Arrangement
import androidx.compose.foundation.layout.Column
import androidx.compose.foundation.layout.fillMaxSize
import androidx.compose.foundation.layout.padding
import androidx.compose.material3.Button
import androidx.compose.material3.MaterialTheme
import androidx.compose.material3.Text
import androidx.compose.ui.Modifier
import androidx.compose.ui.unit.dp
import ch.claudiowanner.healthdataexporter.ui.theme.HealthDataExporterTheme

class PermissionsRationaleActivity : ComponentActivity() {
    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)

        setContent {
            HealthDataExporterTheme {
                Column(
                    modifier = Modifier
                        .fillMaxSize()
                        .padding(24.dp),
                    verticalArrangement = Arrangement.spacedBy(16.dp)
                ) {
                    Text(
                        text = "Privacy policy and permissions rationale",
                        style = MaterialTheme.typography.headlineSmall
                    )

                    Text(
                        text = "This app reads your step data from Health Connect so you can inspect and later export your own health data as JSON. This screen is a local development placeholder and must be replaced with your real privacy policy before publishing.",
                        style = MaterialTheme.typography.bodyLarge
                    )

                    Button(onClick = { finish() }) {
                        Text("Close")
                    }
                }
            }
        }
    }
}