package ch.claudiowanner.healthdataexporter.storage

import android.content.Context
import ch.claudiowanner.healthdataexporter.model.ExportPayload
import ch.claudiowanner.healthdataexporter.model.StepExportRecord
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
                exportVersion = 1,
                source = "manual-test",
                exportedAt = Instant.now().toString(),
                records = listOf(
                    StepExportRecord(
                        count = 8342,
                        startTime = "2026-04-02T00:00:00Z",
                        endTime = "2026-04-02T23:59:59Z"
                    )
                )
            )

            writeExportToFile(context, payload)
        }
    }

    fun writeExport(
        context: Context,
        payload: ExportPayload
    ): Result<Pair<File, String>> {
        return runCatching {
            val file = writeExportToFile(context, payload)
            val content = file.readText()
            file to content
        }
    }

    fun readLatestExport(context: Context): Result<Pair<File, String>> {
        return runCatching {
            val exportDirectory = File(context.filesDir, "exports")

            if (!exportDirectory.exists() || !exportDirectory.isDirectory) {
                error("No export directory found.")
            }

            val latestFile = exportDirectory
                .listFiles { file -> file.isFile && file.extension == "json" }
                ?.maxByOrNull { it.lastModified() }
                ?: error("No export file found.")

            val content = latestFile.readText()

            latestFile to content
        }
    }

    private fun writeExportToFile(
        context: Context,
        payload: ExportPayload
    ): File {
        val exportDirectory = File(context.filesDir, "exports")
        if (!exportDirectory.exists()) {
            exportDirectory.mkdirs()
        }

        val fileName = "health-export-${System.currentTimeMillis()}.json"
        val file = File(exportDirectory, fileName)

        val json = gson.toJson(payload)
        file.writeText(json)

        return file
    }
}