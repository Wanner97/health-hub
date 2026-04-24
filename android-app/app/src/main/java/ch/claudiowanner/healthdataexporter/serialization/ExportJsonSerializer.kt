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

        val bloodOxygenDaily = JsonObject()
        bloodOxygenDaily.add("records", gson.toJsonTree(payload.clusters.vitals.bloodOxygenDaily.records))
        vitals.add("bloodOxygenDaily", bloodOxygenDaily)

        clusters.add("vitals", vitals)
        root.add("clusters", clusters)

        return gson.toJson(root)
    }
}