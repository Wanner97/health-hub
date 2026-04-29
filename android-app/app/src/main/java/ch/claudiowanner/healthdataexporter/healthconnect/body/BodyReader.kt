package ch.claudiowanner.healthdataexporter.healthconnect.body

import androidx.health.connect.client.HealthConnectClient
import androidx.health.connect.client.records.HeightRecord
import androidx.health.connect.client.records.WeightRecord
import androidx.health.connect.client.request.ReadRecordsRequest
import androidx.health.connect.client.time.TimeRangeFilter
import ch.claudiowanner.healthdataexporter.model.body.HeightExportRecord
import ch.claudiowanner.healthdataexporter.model.body.WeightExportRecord
import java.time.Duration
import java.time.Instant
import java.time.ZoneId

class BodyReader(
    private val client: HealthConnectClient
) {
    suspend fun readWeightRecords(
        startInstant: Instant,
        endInstant: Instant
    ): List<WeightExportRecord> {
        require(startInstant < endInstant) {
            "startInstant must be before endInstant."
        }

        val zoneId = ZoneId.systemDefault()
        val allResults = mutableListOf<WeightExportRecord>()

        var chunkStart = startInstant
        while (chunkStart < endInstant) {
            val chunkEnd = minOf(chunkStart.plus(Duration.ofDays(365)), endInstant)

            val chunkResults = readWeightRecordsForRange(
                startInstant = chunkStart,
                endInstant = chunkEnd,
                zoneId = zoneId
            )

            allResults.addAll(chunkResults)
            chunkStart = chunkEnd
        }

        return allResults.sortedBy { it.measuredAt }
    }

    suspend fun readLatestHeightRecord(
        startInstant: Instant,
        endInstant: Instant
    ): HeightExportRecord? {
        require(startInstant < endInstant) {
            "startInstant must be before endInstant."
        }

        var latestRecord: HeightRecord? = null

        var chunkStart = startInstant
        while (chunkStart < endInstant) {
            val chunkEnd = minOf(chunkStart.plus(Duration.ofDays(365)), endInstant)

            latestRecord = readLatestHeightRecordForRange(
                startInstant = chunkStart,
                endInstant = chunkEnd,
                currentLatest = latestRecord
            )

            chunkStart = chunkEnd
        }

        return latestRecord?.let { record ->
            HeightExportRecord(
                heightCm = record.height.inMeters * 100.0,
                measuredAt = record.time.toString()
            )
        }
    }

    private suspend fun readWeightRecordsForRange(
        startInstant: Instant,
        endInstant: Instant,
        zoneId: ZoneId
    ): List<WeightExportRecord> {
        val results = mutableListOf<WeightExportRecord>()
        var pageToken: String? = null

        do {
            val response = client.readRecords(
                ReadRecordsRequest(
                    recordType = WeightRecord::class,
                    timeRangeFilter = TimeRangeFilter.between(startInstant, endInstant),
                    pageSize = 1000,
                    pageToken = pageToken
                )
            )

            val mapped = response.records.map { record ->
                WeightExportRecord(
                    date = record.time.atZone(zoneId).toLocalDate().toString(),
                    weightKg = record.weight.inKilograms,
                    measuredAt = record.time.toString()
                )
            }

            results.addAll(mapped)
            pageToken = response.pageToken
        } while (pageToken != null)

        return results
    }

    private suspend fun readLatestHeightRecordForRange(
        startInstant: Instant,
        endInstant: Instant,
        currentLatest: HeightRecord?
    ): HeightRecord? {
        var latest = currentLatest
        var pageToken: String? = null

        do {
            val response = client.readRecords(
                ReadRecordsRequest(
                    recordType = HeightRecord::class,
                    timeRangeFilter = TimeRangeFilter.between(startInstant, endInstant),
                    pageSize = 1000,
                    pageToken = pageToken
                )
            )

            response.records.forEach { record ->
                if (latest == null || record.time > latest!!.time) {
                    latest = record
                }
            }

            pageToken = response.pageToken
        } while (pageToken != null)

        return latest
    }
}