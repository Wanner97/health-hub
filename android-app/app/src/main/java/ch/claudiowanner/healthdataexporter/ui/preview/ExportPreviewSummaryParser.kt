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

            ExportPreviewSummary(
                exportType = exportType,
                rangeDescription = when {
                    exportType == ExportType.FULL_HISTORY.jsonValue -> "Full history"
                    rangeDays != null -> "Last $rangeDays days"
                    else -> null
                },
                activityRecordCount = activityCount,
                sleepSessionCount = sleepCount,
                heartRateDailyCount = heartRateDailyCount,
                heartRateHourlyCount = heartRateHourlyCount,
                bloodOxygenDailyCount = bloodOxygenDailyCount,
                weightRecordCount = weightRecordCount,
                hasLatestHeight = hasLatestHeight,
                nutritionRecordCount = nutritionRecordCount
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
}