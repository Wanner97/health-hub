package ch.claudiowanner.healthdataexporter.healthconnect.vitals

import androidx.health.connect.client.HealthConnectClient
import androidx.health.connect.client.records.OxygenSaturationRecord
import androidx.health.connect.client.request.ReadRecordsRequest
import androidx.health.connect.client.time.TimeRangeFilter
import ch.claudiowanner.healthdataexporter.model.vitals.BloodOxygenDailyExportRecord
import java.time.Duration
import java.time.Instant
import java.time.LocalDate
import java.time.ZoneId
import java.util.SortedMap
import java.util.TreeMap

class BloodOxygenReader(
    private val client: HealthConnectClient
) {
    suspend fun readBloodOxygenDailyRecords(
        startInstant: Instant,
        endInstant: Instant
    ): List<BloodOxygenDailyExportRecord> {
        require(startInstant < endInstant) {
            "startInstant must be before endInstant."
        }

        val zoneId = ZoneId.systemDefault()
        val buckets: SortedMap<LocalDate, BloodOxygenDayAccumulator> = TreeMap()

        var chunkStart = startInstant
        while (chunkStart < endInstant) {
            val chunkEnd = minOf(chunkStart.plus(Duration.ofDays(365)), endInstant)
            readChunkIntoBuckets(
                startInstant = chunkStart,
                endInstant = chunkEnd,
                zoneId = zoneId,
                buckets = buckets
            )
            chunkStart = chunkEnd
        }

        return buckets.map { (date, accumulator) ->
            BloodOxygenDailyExportRecord(
                date = date.toString(),
                minPercent = accumulator.minPercent,
                maxPercent = accumulator.maxPercent,
                avgPercent = accumulator.sumPercent / accumulator.measurementCount.toDouble(),
                measurementCount = accumulator.measurementCount,
                startTime = date.atStartOfDay(zoneId).toInstant().toString(),
                endTime = date.plusDays(1).atStartOfDay(zoneId).toInstant().toString()
            )
        }
    }

    private suspend fun readChunkIntoBuckets(
        startInstant: Instant,
        endInstant: Instant,
        zoneId: ZoneId,
        buckets: SortedMap<LocalDate, BloodOxygenDayAccumulator>
    ) {
        var pageToken: String? = null

        do {
            val response = client.readRecords(
                ReadRecordsRequest(
                    recordType = OxygenSaturationRecord::class,
                    timeRangeFilter = TimeRangeFilter.between(startInstant, endInstant),
                    pageSize = 1000,
                    pageToken = pageToken
                )
            )

            response.records.forEach { record ->
                val date = record.time.atZone(zoneId).toLocalDate()
                val value = record.percentage.value

                val existing = buckets[date]
                if (existing == null) {
                    buckets[date] = BloodOxygenDayAccumulator(
                        minPercent = value,
                        maxPercent = value,
                        sumPercent = value,
                        measurementCount = 1
                    )
                } else {
                    existing.minPercent = minOf(existing.minPercent, value)
                    existing.maxPercent = maxOf(existing.maxPercent, value)
                    existing.sumPercent += value
                    existing.measurementCount += 1
                }
            }

            pageToken = response.pageToken
        } while (pageToken != null)
    }

    private data class BloodOxygenDayAccumulator(
        var minPercent: Double,
        var maxPercent: Double,
        var sumPercent: Double,
        var measurementCount: Long
    )
}