package ch.claudiowanner.healthdataexporter.healthconnect.activity

import androidx.health.connect.client.HealthConnectClient
import androidx.health.connect.client.records.DistanceRecord
import androidx.health.connect.client.records.StepsRecord
import androidx.health.connect.client.records.TotalCaloriesBurnedRecord
import androidx.health.connect.client.request.AggregateGroupByPeriodRequest
import androidx.health.connect.client.time.TimeRangeFilter
import ch.claudiowanner.healthdataexporter.model.activity.ActivityDayExportRecord
import ch.claudiowanner.healthdataexporter.healthconnect.HealthConnectReadConfig
import java.time.LocalDate
import java.time.Period
import java.time.ZoneId

class ActivityReader(
    private val client: HealthConnectClient
) {
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
            val chunkEndExclusive = minOf(
                chunkStart.plusDays(HealthConnectReadConfig.DAILY_AGGREGATION_CHUNK_DAYS),
                endDateExclusive
            )

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

    private suspend fun readActivityExportRecordsForRange(
        startDateInclusive: LocalDate,
        endDateExclusive: LocalDate,
        zoneId: ZoneId
    ): List<ActivityDayExportRecord> {
        val response = client.aggregateGroupByPeriod(
            AggregateGroupByPeriodRequest(
                metrics = setOf(
                    StepsRecord.COUNT_TOTAL,
                    DistanceRecord.DISTANCE_TOTAL,
                    TotalCaloriesBurnedRecord.ENERGY_TOTAL
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
            val totalCaloriesBurnedKcal =
                dayResult.result[TotalCaloriesBurnedRecord.ENERGY_TOTAL]?.inKilocalories ?: 0.0

            if (steps == 0L && distanceMeters == 0.0 && totalCaloriesBurnedKcal == 0.0) {
                return@mapNotNull null
            }

            val startInstant = dayResult.startTime.atZone(zoneId).toInstant()
            val endInstant = dayResult.endTime.atZone(zoneId).toInstant()

            ActivityDayExportRecord(
                date = dayResult.startTime.toLocalDate().toString(),
                steps = steps,
                distanceMeters = distanceMeters,
                totalCaloriesBurnedKcal = totalCaloriesBurnedKcal,
                startTime = startInstant.toString(),
                endTime = endInstant.toString()
            )
        }
    }
}