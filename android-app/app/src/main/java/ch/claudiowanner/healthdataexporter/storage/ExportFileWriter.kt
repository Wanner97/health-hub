package ch.claudiowanner.healthdataexporter.storage

import android.content.Context
import ch.claudiowanner.healthdataexporter.model.ExportPayload
import com.google.gson.GsonBuilder
import java.io.File

class ExportFileWriter {
    private val gson = GsonBuilder()
        .setPrettyPrinting()
        .create()

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

        val fileName = "steps-export-${System.currentTimeMillis()}.json"
        val file = File(exportDirectory, fileName)

        val json = gson.toJson(payload)
        file.writeText(json)

        return file
    }
}