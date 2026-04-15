package ch.claudiowanner.healthdataexporter.healthconnect

import android.content.Context
import androidx.health.connect.client.HealthConnectClient
import androidx.health.connect.client.permission.HealthPermission
import androidx.health.connect.client.records.DistanceRecord
import androidx.health.connect.client.records.SleepSessionRecord
import androidx.health.connect.client.records.StepsRecord
import androidx.health.connect.client.request.AggregateGroupByPeriodRequest
import androidx.health.connect.client.request.ReadRecordsRequest
import androidx.health.connect.client.time.TimeRangeFilter
import ch.claudiowanner.healthdataexporter.model.ActivityDayExportRecord
import ch.claudiowanner.healthdataexporter.model.SleepSessionExportRecord
import ch.claudiowanner.healthdataexporter.model.SleepStageExportRecord
import java.time.Duration
import java.time.Instant
import java.time.LocalDate
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
            HealthPermission.getReadPermission(SleepSessionRecord::class),
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

    suspend fun readActivityExportRecords(
        startDateInclusive: LocalDate,
        endDateExclusive: LocalDate
    ): List<ActivityDayExportRecord> {
        require(startDateInclusive < endDateExclusive) {
            "startDateInclusive must be before endDateExclusive."
        }

        val zoneId = ZoneId.systemDefault()
        val allResults = mutableListOf<ActivityDayExportRecord>()
        var chunkStart = startDateInclusive

        while (chunkStart < endDateExclusive) {
            val chunkEndExclusive = minOf(chunkStart.plusDays(365), endDateExclusive)

            val chunkResults = readActivityExportRecordsForRange(
                startDateInclusive = chunkStart,
                endDateExclusive = chunkEndExclusive,
                zoneId = zoneId
            )

            allResults.addAll(chunkResults)
            chunkStart = chunkEndExclusive
        }

        return allResults.sortedBy { it.date }
    }

    suspend fun readSleepSessions(
        startInstant: Instant,
        endInstant: Instant
    ): List<SleepSessionExportRecord> {
        require(startInstant < endInstant) {
            "startInstant must be before endInstant."
        }

        val allResults = mutableListOf<SleepSessionExportRecord>()
        var pageToken: String? = null

        do {
            val response = client().readRecords(
                ReadRecordsRequest(
                    recordType = SleepSessionRecord::class,
                    timeRangeFilter = TimeRangeFilter.between(startInstant, endInstant),
                    pageSize = 1000,
                    pageToken = pageToken
                )
            )

            val mapped = response.records.map { session ->
                SleepSessionExportRecord(
                    startTime = session.startTime.toString(),
                    endTime = session.endTime.toString(),
                    durationMinutes = Duration.between(
                        session.startTime,
                        session.endTime
                    ).toMinutes(),
                    title = session.title,
                    notes = session.notes,
                    stages = session.stages.map { stage ->
                        SleepStageExportRecord(
                            startTime = stage.startTime.toString(),
                            endTime = stage.endTime.toString(),
                            stage = mapSleepStage(stage.stage)
                        )
                    }
                )
            }

            allResults.addAll(mapped)
            pageToken = response.pageToken
        } while (pageToken != null)

        return allResults.sortedBy { it.startTime }
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

    private fun mapSleepStage(stageType: Int): String {
        return when (stageType) {
            SleepSessionRecord.STAGE_TYPE_UNKNOWN -> "unknown"
            SleepSessionRecord.STAGE_TYPE_AWAKE -> "awake"
            SleepSessionRecord.STAGE_TYPE_SLEEPING -> "sleeping"
            SleepSessionRecord.STAGE_TYPE_OUT_OF_BED -> "out_of_bed"
            SleepSessionRecord.STAGE_TYPE_LIGHT -> "light"
            SleepSessionRecord.STAGE_TYPE_DEEP -> "deep"
            SleepSessionRecord.STAGE_TYPE_REM -> "rem"
            else -> "unknown"
        }
    }
}