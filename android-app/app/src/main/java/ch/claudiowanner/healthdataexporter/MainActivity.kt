package ch.claudiowanner.healthdataexporter

import android.os.Bundle
import androidx.activity.ComponentActivity
import androidx.activity.compose.setContent
import ch.claudiowanner.healthdataexporter.storage.ExportFileWriter
import ch.claudiowanner.healthdataexporter.ui.ExportScreen
import ch.claudiowanner.healthdataexporter.ui.theme.HealthDataExporterTheme

class MainActivity : ComponentActivity() {
    private val exportFileWriter = ExportFileWriter()

    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)

        setContent {
            HealthDataExporterTheme {
                ExportScreen(
                    onCreateTestExport = {
                        exportFileWriter.writeTestExport(this).map { file ->
                            file.absolutePath
                        }
                    },
                    onLoadLatestExport = {
                        exportFileWriter.readLatestExport(this).map { (file, content) ->
                            file.absolutePath to content
                        }
                    }
                )
            }
        }
    }
}