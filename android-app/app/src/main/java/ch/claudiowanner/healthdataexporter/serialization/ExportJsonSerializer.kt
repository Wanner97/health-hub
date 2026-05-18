package ch.claudiowanner.healthdataexporter.serialization

import ch.claudiowanner.healthdataexporter.model.ExportPayload
import com.google.gson.Gson
import com.google.gson.GsonBuilder
import com.google.gson.JsonNull
import com.google.gson.JsonObject

class ExportJsonSerializer {
    private val gson: Gson = GsonBuilder()
        .setPrettyPrinting()
        .create()

    fun serialize(payload: ExportPayload): String {
        val root = buildRootJson(payload)
        return gson.toJson(root)
    }

    private fun buildRootJson(payload: ExportPayload): JsonObject {
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
        root.add("clusters", buildClustersJson(payload))

        return root
    }

    private fun buildClustersJson(payload: ExportPayload): JsonObject {
        val clusters = JsonObject()

        clusters.add("activity", buildActivityJson(payload))
        clusters.add("sleep", buildSleepJson(payload))
        clusters.add("vitals", buildVitalsJson(payload))
        clusters.add("body", buildBodyJson(payload))
        clusters.add("nutrition", buildNutritionJson(payload))

        return clusters
    }

    private fun buildActivityJson(payload: ExportPayload): JsonObject {
        val activity = JsonObject()
        activity.add("records", gson.toJsonTree(payload.clusters.activity.records))
        return activity
    }

    private fun buildSleepJson(payload: ExportPayload): JsonObject {
        val sleep = JsonObject()
        sleep.add("sessions", gson.toJsonTree(payload.clusters.sleep.sessions))
        return sleep
    }

    private fun buildVitalsJson(payload: ExportPayload): JsonObject {
        val vitals = JsonObject()

        val heartRateDaily = JsonObject()
        heartRateDaily.add("records", gson.toJsonTree(payload.clusters.vitals.heartRateDaily.records))
        vitals.add("heartRateDaily", heartRateDaily)

        val heartRateHourly = JsonObject()
        heartRateHourly.add("records", gson.toJsonTree(payload.clusters.vitals.heartRateHourly.records))
        vitals.add("heartRateHourly", heartRateHourly)

        val bloodOxygenDaily = JsonObject()
        bloodOxygenDaily.add("records", gson.toJsonTree(payload.clusters.vitals.bloodOxygenDaily.records))
        vitals.add("bloodOxygenDaily", bloodOxygenDaily)

        return vitals
    }

    private fun buildBodyJson(payload: ExportPayload): JsonObject {
        val body = JsonObject()
        body.add("latestHeight", gson.toJsonTree(payload.clusters.body.latestHeight))
        body.add("weightRecords", gson.toJsonTree(payload.clusters.body.weightRecords))
        return body
    }

    private fun buildNutritionJson(payload: ExportPayload): JsonObject {
        val nutrition = JsonObject()
        nutrition.add("records", gson.toJsonTree(payload.clusters.nutrition.records))
        return nutrition
    }
}