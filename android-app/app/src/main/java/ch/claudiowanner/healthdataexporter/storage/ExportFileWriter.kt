package ch.claudiowanner.healthdataexporter.storage

import android.content.Context
import ch.claudiowanner.healthdataexporter.model.ExportPayload
import ch.claudiowanner.healthdataexporter.model.HealthRecordExport
import com.google.gson.GsonBuilder
import java.io.File
import java.time.Instant

class ExportFileWriter {
    private val gson = GsonBuilder()
        .setPrettyPrinting()
        .create()

    fun writeTestExport(context: Context): Result<File> {
        return runCatching {
            val payload = ExportPayload(
                source = "manual-test",
                exportedAt = Instant.now().toString(),
                records = listOf(
                    HealthRecordExport(
                        type = "steps",
                        value = 8342,
                        unit = "count",
                        startTime = "2026-04-02T00:00:00Z",
                        endTime = "2026-04-02T23:59:59Z"
                    )
                )
            )

            val exportDirectory = File(context.filesDir, "exports")
            if (!exportDirectory.exists()) {
                exportDirectory.mkdirs()
            }

            val fileName = "health-export-${System.currentTimeMillis()}.json"
            val file = File(exportDirectory, fileName)

            val json = gson.toJson(payload)
            file.writeText(json)

            file
        }
    }
}