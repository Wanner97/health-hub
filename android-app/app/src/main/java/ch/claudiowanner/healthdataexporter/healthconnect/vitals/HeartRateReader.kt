package ch.claudiowanner.healthdataexporter.healthconnect.vitals

import androidx.health.connect.client.HealthConnectClient
import androidx.health.connect.client.records.HeartRateRecord
import androidx.health.connect.client.request.AggregateGroupByDurationRequest
import androidx.health.connect.client.request.AggregateGroupByPeriodRequest
import androidx.health.connect.client.time.TimeRangeFilter
import ch.claudiowanner.healthdataexporter.model.vitals.HeartRateDailyExportRecord
import ch.claudiowanner.healthdataexporter.model.vitals.HeartRateHourlyExportRecord
import ch.claudiowanner.healthdataexporter.healthconnect.HealthConnectReadConfig
import java.time.Duration
import java.time.Instant
import java.time.LocalDate
import java.time.Period
import java.time.ZoneId

class HeartRateReader(
    private val client: HealthConnectClient
) {
    suspend fun readHeartRateDailyRecords(
        startDateInclusive: LocalDate,
        endDateExclusive: LocalDate
    ): List<HeartRateDailyExportRecord> {
        require(startDateInclusive < endDateExclusive) {
            "startDateInclusive must be before endDateExclusive."
        }

        val zoneId = ZoneId.systemDefault()
        val allResults = mutableListOf<HeartRateDailyExportRecord>()
        var chunkStart = startDateInclusive

        while (chunkStart < endDateExclusive) {
            val chunkEndExclusive = minOf(
                chunkStart.plusDays(HealthConnectReadConfig.DAILY_AGGREGATION_CHUNK_DAYS),
                endDateExclusive
            )

            val response = client.aggregateGroupByPeriod(
                AggregateGroupByPeriodRequest(
                    metrics = setOf(
                        HeartRateRecord.BPM_MIN,
                        HeartRateRecord.BPM_MAX,
                        HeartRateRecord.BPM_AVG,
                        HeartRateRecord.MEASUREMENTS_COUNT
                    ),
                    timeRangeFilter = TimeRangeFilter.between(
                        chunkStart.atStartOfDay(),
                        chunkEndExclusive.atStartOfDay()
                    ),
                    timeRangeSlicer = Period.ofDays(1)
                )
            )

            val mapped = response.mapNotNull { dayResult ->
                val minBpm = dayResult.result[HeartRateRecord.BPM_MIN]
                val maxBpm = dayResult.result[HeartRateRecord.BPM_MAX]
                val avgBpm = dayResult.result[HeartRateRecord.BPM_AVG]
                val measurementCount = dayResult.result[HeartRateRecord.MEASUREMENTS_COUNT] ?: 0L

                if (minBpm == null && maxBpm == null && avgBpm == null && measurementCount == 0L) {
                    return@mapNotNull null
                }

                val startInstant = dayResult.startTime.atZone(zoneId).toInstant()
                val endInstant = dayResult.endTime.atZone(zoneId).toInstant()

                HeartRateDailyExportRecord(
                    date = dayResult.startTime.toLocalDate().toString(),
                    minBpm = minBpm ?: 0L,
                    maxBpm = maxBpm ?: 0L,
                    avgBpm = avgBpm ?: 0L,
                    measurementCount = measurementCount,
                    startTime = startInstant.toString(),
                    endTime = endInstant.toString()
                )
            }

            allResults.addAll(mapped)
            chunkStart = chunkEndExclusive
        }

        return allResults.sortedBy { it.date }
    }

    suspend fun readHeartRateHourlyRecords(
        startInstant: Instant,
        endInstant: Instant
    ): List<HeartRateHourlyExportRecord> {
        require(startInstant < endInstant) {
            "startInstant must be before endInstant."
        }

        val zoneId = ZoneId.systemDefault()
        val allResults = mutableListOf<HeartRateHourlyExportRecord>()
        var chunkStart = startInstant

        while (chunkStart < endInstant) {
            val chunkEnd = minOf(
                chunkStart.plus(Duration.ofDays(HealthConnectReadConfig.HOURLY_AGGREGATION_CHUNK_DAYS)),
                endInstant
            )

            val response = client.aggregateGroupByDuration(
                AggregateGroupByDurationRequest(
                    metrics = setOf(
                        HeartRateRecord.BPM_MIN,
                        HeartRateRecord.BPM_MAX,
                        HeartRateRecord.BPM_AVG,
                        HeartRateRecord.MEASUREMENTS_COUNT
                    ),
                    timeRangeFilter = TimeRangeFilter.between(chunkStart, chunkEnd),
                    timeRangeSlicer = Duration.ofHours(1)
                )
            )

            val mapped = response.mapNotNull { hourResult ->
                val minBpm = hourResult.result[HeartRateRecord.BPM_MIN]
                val maxBpm = hourResult.result[HeartRateRecord.BPM_MAX]
                val avgBpm = hourResult.result[HeartRateRecord.BPM_AVG]
                val measurementCount = hourResult.result[HeartRateRecord.MEASUREMENTS_COUNT] ?: 0L

                if (minBpm == null && maxBpm == null && avgBpm == null && measurementCount == 0L) {
                    return@mapNotNull null
                }

                val bucketStart = hourResult.startTime
                val bucketEnd = hourResult.endTime
                val localBucketStart = bucketStart.atZone(zoneId)

                HeartRateHourlyExportRecord(
                    date = localBucketStart.toLocalDate().toString(),
                    hour = localBucketStart.hour,
                    minBpm = minBpm ?: 0L,
                    maxBpm = maxBpm ?: 0L,
                    avgBpm = avgBpm ?: 0L,
                    measurementCount = measurementCount,
                    startTime = bucketStart.toString(),
                    endTime = bucketEnd.toString()
                )
            }

            allResults.addAll(mapped)
            chunkStart = chunkEnd
        }

        return allResults.sortedWith(
            compareBy<HeartRateHourlyExportRecord> { it.date }
                .thenBy { it.hour }
        )
    }
}