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
                        text = "This app reads selected health data from Health Connect, including activity, sleep, vital measurements, body measurements and nutrition records, so you can inspect and export your own data as JSON.",
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