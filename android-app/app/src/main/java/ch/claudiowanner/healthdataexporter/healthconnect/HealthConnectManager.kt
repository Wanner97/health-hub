package ch.claudiowanner.healthdataexporter.healthconnect

import android.content.Context
import androidx.health.connect.client.HealthConnectClient
import androidx.health.connect.client.permission.HealthPermission
import androidx.health.connect.client.records.DistanceRecord
import androidx.health.connect.client.records.StepsRecord
import androidx.health.connect.client.request.AggregateGroupByPeriodRequest
import androidx.health.connect.client.request.AggregateRequest
import androidx.health.connect.client.time.TimeRangeFilter
import ch.claudiowanner.healthdataexporter.model.ActivityDayExportRecord
import java.time.Instant
import java.time.LocalDate
import java.time.LocalDateTime
import java.time.Period
import java.time.ZoneId

class HealthConnectManager(private val context: Context) {

    companion object {
        const val PROVIDER_PACKAGE_NAME = "com.google.android.apps.healthdata"
        private const val READ_HEALTH_DATA_HISTORY =
            "android.permission.health.READ_HEALTH_DATA_HISTORY"

        val PERMISSIONS = setOf(
            HealthPermission.getReadPermission(StepsRecord::class),
            HealthPermission.getReadPermission(DistanceRecord::class),
            READ_HEALTH_DATA_HISTORY
        )
    }

    fun getSdkStatus(): Int {
        return HealthConnectClient.getSdkStatus(context, PROVIDER_PACKAGE_NAME)
    }

    private fun client(): HealthConnectClient {
        return HealthConnectClient.getOrCreate(context)
    }

    suspend fun hasAllPermissions(): Boolean {
        val granted = client().permissionController.getGrantedPermissions()
        return granted.containsAll(PERMISSIONS)
    }

    suspend fun readTodaySteps(): Long {
        val zoneId = ZoneId.systemDefault()
        val startOfDay = LocalDate.now(zoneId).atStartOfDay(zoneId).toInstant()
        val now = Instant.now()

        val response = client().aggregate(
            AggregateRequest(
                metrics = setOf(StepsRecord.COUNT_TOTAL),
                timeRangeFilter = TimeRangeFilter.between(startOfDay, now)
            )
        )

        return response[StepsRecord.COUNT_TOTAL] ?: 0L
    }

    suspend fun readActivityExportRecordsForFullHistory(): List<ActivityDayExportRecord> {
        val zoneId = ZoneId.systemDefault()
        val overallStart = LocalDate.of(2000, 1, 1)
        val overallEndExclusive = LocalDate.now(zoneId).plusDays(1)

        val allResults = mutableListOf<ActivityDayExportRecord>()

        var chunkStart = overallStart

        while (chunkStart < overallEndExclusive) {
            val chunkEndExclusive = minOf(chunkStart.plusDays(365), overallEndExclusive)

            val chunkResults = readActivityExportRecordsForRange(
                startDateInclusive = chunkStart,
                endDateExclusive = chunkEndExclusive,
                zoneId = zoneId
            )

            allResults.addAll(chunkResults)
            chunkStart = chunkEndExclusive
        }

        return allResults
            .sortedBy { it.date }
    }

    private suspend fun readActivityExportRecordsForRange(
        startDateInclusive: LocalDate,
        endDateExclusive: LocalDate,
        zoneId: ZoneId
    ): List<ActivityDayExportRecord> {
        val response = client().aggregateGroupByPeriod(
            AggregateGroupByPeriodRequest(
                metrics = setOf(
                    StepsRecord.COUNT_TOTAL,
                    DistanceRecord.DISTANCE_TOTAL
                ),
                timeRangeFilter = TimeRangeFilter.between(
                    startDateInclusive.atStartOfDay(),
                    endDateExclusive.atStartOfDay()
                ),
                timeRangeSlicer = Period.ofDays(1)
            )
        )

        return response.mapNotNull { dayResult ->
            val steps = dayResult.result[StepsRecord.COUNT_TOTAL] ?: 0L
            val distanceMeters = dayResult.result[DistanceRecord.DISTANCE_TOTAL]?.inMeters ?: 0.0

            if (steps == 0L && distanceMeters == 0.0) {
                return@mapNotNull null
            }

            val startInstant = dayResult.startTime.atZone(zoneId).toInstant()
            val endInstant = dayResult.endTime.atZone(zoneId).toInstant()

            ActivityDayExportRecord(
                date = dayResult.startTime.toLocalDate().toString(),
                steps = steps,
                distanceMeters = distanceMeters,
                startTime = startInstant.toString(),
                endTime = endInstant.toString()
            )
        }
    }
}