package ch.claudiowanner.healthdataexporter.storage

import android.content.Context
import android.net.Uri
import ch.claudiowanner.healthdataexporter.model.ExportPayload
import com.google.gson.Gson
import com.google.gson.GsonBuilder
import com.google.gson.JsonNull
import com.google.gson.JsonObject
import java.io.File

class ExportFileWriter {
    private val gson: Gson = GsonBuilder()
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

    fun writeJsonToUri(
        context: Context,
        uri: Uri,
        jsonContent: String
    ): Result<Unit> {
        return runCatching {
            val outputStream = context.contentResolver.openOutputStream(uri)
                ?: error("Could not open output stream for selected file.")

            outputStream.bufferedWriter().use { writer ->
                writer.write(jsonContent)
            }
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

        val json = buildOrderedExportJson(payload)
        file.writeText(json)

        return file
    }

    private fun buildOrderedExportJson(payload: ExportPayload): String {
        val root = JsonObject()

        root.addProperty("exportVersion", payload.exportVersion)
        root.addProperty("source", payload.source)
        root.addProperty("exportedAt", payload.exportedAt)
        root.addProperty("exportType", payload.exportType)

        if (payload.rangeDays != null) {
            root.addProperty("rangeDays", payload.rangeDays)
        } else {
            root.add("rangeDays", JsonNull.INSTANCE)
        }

        root.addProperty("rangeStart", payload.rangeStart)
        root.addProperty("rangeEnd", payload.rangeEnd)

        val clusters = JsonObject()

        val activity = JsonObject()
        activity.add("records", gson.toJsonTree(payload.clusters.activity.records))
        clusters.add("activity", activity)

        val sleep = JsonObject()
        sleep.add("sessions", gson.toJsonTree(payload.clusters.sleep.sessions))
        clusters.add("sleep", sleep)

        val vitals = JsonObject()

        val heartRateDaily = JsonObject()
        heartRateDaily.add("records", gson.toJsonTree(payload.clusters.vitals.heartRateDaily.records))
        vitals.add("heartRateDaily", heartRateDaily)

        val heartRateHourly = JsonObject()
        heartRateHourly.add("records", gson.toJsonTree(payload.clusters.vitals.heartRateHourly.records))
        vitals.add("heartRateHourly", heartRateHourly)

        clusters.add("vitals", vitals)

        root.add("clusters", clusters)

        return gson.toJson(root)
    }
}