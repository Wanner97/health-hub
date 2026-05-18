package ch.claudiowanner.healthdataexporter.ui.preview

import ch.claudiowanner.healthdataexporter.config.ExportType
import com.google.gson.JsonObject
import com.google.gson.JsonParser

class ExportPreviewSummaryParser {
    fun parse(previewContent: String): ExportPreviewSummary? {
        return runCatching {
            val root = JsonParser.parseString(previewContent).asJsonObject
            val exportType = root.stringOrNull("exportType")
            val rangeDays = root.intOrNull("rangeDays")
            val clusters = root.objectOrNull("clusters")

            val activityCount = clusters
                ?.objectOrNull("activity")
                ?.arraySizeOrNull("records")

            val sleepCount = clusters
                ?.objectOrNull("sleep")
                ?.arraySizeOrNull("sessions")

            val heartRateDailyCount = clusters
                ?.objectOrNull("vitals")
                ?.objectOrNull("heartRateDaily")
                ?.arraySizeOrNull("records")

            val heartRateHourlyCount = clusters
                ?.objectOrNull("vitals")
                ?.objectOrNull("heartRateHourly")
                ?.arraySizeOrNull("records")

            val bloodOxygenDailyCount = clusters
                ?.objectOrNull("vitals")
                ?.objectOrNull("bloodOxygenDaily")
                ?.arraySizeOrNull("records")

            val body = clusters?.objectOrNull("body")

            val weightRecordCount = body
                ?.arraySizeOrNull("weightRecords")

            val hasLatestHeight = body
                ?.hasNonNull("latestHeight")

            val nutritionRecordCount = clusters
                ?.objectOrNull("nutrition")
                ?.arraySizeOrNull("records")

            val rangeDescription = when {
                exportType == ExportType.FULL_HISTORY.jsonValue -> "Full history"
                rangeDays != null -> "Last $rangeDays days"
                else -> null
            }

            ExportPreviewSummary(
                items = buildList {
                    addSummaryItem("Export type", exportType)
                    addSummaryItem("Range", rangeDescription)
                    addSummaryItem("Activity records", activityCount)
                    addSummaryItem("Nutrition records", nutritionRecordCount)
                    addSummaryItem("Weight records", weightRecordCount)
                    addSummaryItem(
                        "Latest height available",
                        hasLatestHeight?.let { if (it) "Yes" else "No" }
                    )
                    addSummaryItem("Sleep sessions", sleepCount)
                    addSummaryItem("Heart rate daily records", heartRateDailyCount)
                    addSummaryItem("Heart rate hourly records", heartRateHourlyCount)
                    addSummaryItem("Blood oxygen daily records", bloodOxygenDailyCount)
                }
            )
        }.getOrNull()
    }

    private fun JsonObject.stringOrNull(name: String): String? {
        val value = get(name) ?: return null
        return if (value.isJsonNull) null else value.asString
    }

    private fun JsonObject.intOrNull(name: String): Int? {
        val value = get(name) ?: return null
        return if (value.isJsonNull) null else value.asInt
    }

    private fun JsonObject.objectOrNull(name: String): JsonObject? {
        val value = get(name) ?: return null
        return if (value.isJsonObject) value.asJsonObject else null
    }

    private fun JsonObject.arraySizeOrNull(name: String): Int? {
        val value = get(name) ?: return null
        return if (value.isJsonArray) value.asJsonArray.size() else null
    }

    private fun JsonObject.hasNonNull(name: String): Boolean {
        val value = get(name) ?: return false
        return !value.isJsonNull
    }

    private fun MutableList<ExportPreviewSummaryItem>.addSummaryItem(
        label: String,
        value: Any?
    ) {
        if (value == null) {
            return
        }

        add(
            ExportPreviewSummaryItem(
                label = label,
                value = value.toString()
            )
        )
    }
}