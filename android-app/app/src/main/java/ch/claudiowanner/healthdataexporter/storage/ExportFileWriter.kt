package ch.claudiowanner.healthdataexporter.storage

import android.content.Context
import android.net.Uri
import ch.claudiowanner.healthdataexporter.config.ExportConfig
import kotlinx.coroutines.Dispatchers
import kotlinx.coroutines.withContext
import java.io.File

class ExportFileWriter {

    suspend fun writeExportJson(
        context: Context,
        jsonContent: String
    ): Result<File> = withContext(Dispatchers.IO) {
        runCatching {
            val exportDirectory = File(context.filesDir, "exports")
            if (!exportDirectory.exists()) {
                exportDirectory.mkdirs()
            }

            val fileName = "${ExportConfig.EXPORT_FILE_PREFIX}-${System.currentTimeMillis()}.json"
            val file = File(exportDirectory, fileName)

            file.writeText(jsonContent)

            file
        }
    }

    suspend fun readLatestExport(context: Context): Result<Pair<File, String>> =
        withContext(Dispatchers.IO) {
            runCatching {
                val latestFile = getLatestExportFile(context)
                    ?: error("No export file found.")

                val content = latestFile.readText()
                latestFile to content
            }
        }

    fun getLatestExportFile(context: Context): File? {
        val exportDirectory = File(context.filesDir, "exports")

        if (!exportDirectory.exists() || !exportDirectory.isDirectory) {
            return null
        }

        return exportDirectory
            .listFiles { file -> file.isFile && file.extension == "json" }
            ?.maxByOrNull { it.lastModified() }
    }

    suspend fun writeJsonToUri(
        context: Context,
        uri: Uri,
        jsonContent: String
    ): Result<Unit> = withContext(Dispatchers.IO) {
        runCatching {
            val outputStream = context.contentResolver.openOutputStream(uri)
                ?: error("Could not open output stream for selected file.")

            outputStream.bufferedWriter().use { writer ->
                writer.write(jsonContent)
            }
        }
    }
}