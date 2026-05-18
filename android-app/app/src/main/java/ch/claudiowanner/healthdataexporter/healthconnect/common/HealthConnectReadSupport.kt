package ch.claudiowanner.healthdataexporter.healthconnect.common

import androidx.health.connect.client.HealthConnectClient
import androidx.health.connect.client.records.Record
import androidx.health.connect.client.request.ReadRecordsRequest
import androidx.health.connect.client.time.TimeRangeFilter
import ch.claudiowanner.healthdataexporter.healthconnect.HealthConnectReadConfig
import java.time.Duration
import java.time.Instant
import kotlin.reflect.KClass

class HealthConnectReadSupport(
    private val client: HealthConnectClient
) {
    suspend fun <T : Record, R> readChunkedPagedRecords(
        recordType: KClass<T>,
        startInstant: Instant,
        endInstant: Instant,
        chunkDays: Long = HealthConnectReadConfig.RAW_RECORD_CHUNK_DAYS,
        mapper: (T) -> R
    ): List<R> {
        require(startInstant < endInstant) {
            "startInstant must be before endInstant."
        }

        val allResults = mutableListOf<R>()
        var chunkStart = startInstant

        while (chunkStart < endInstant) {
            val chunkEnd = minOf(
                chunkStart.plus(Duration.ofDays(chunkDays)),
                endInstant
            )

            allResults.addAll(
                readPagedRecords(
                    recordType = recordType,
                    startInstant = chunkStart,
                    endInstant = chunkEnd,
                    mapper = mapper
                )
            )

            chunkStart = chunkEnd
        }

        return allResults
    }

    suspend fun <T : Record, R> readPagedRecords(
        recordType: KClass<T>,
        startInstant: Instant,
        endInstant: Instant,
        mapper: (T) -> R
    ): List<R> {
        require(startInstant < endInstant) {
            "startInstant must be before endInstant."
        }

        val results = mutableListOf<R>()
        var pageToken: String? = null

        do {
            val response = client.readRecords(
                ReadRecordsRequest(
                    recordType = recordType,
                    timeRangeFilter = TimeRangeFilter.between(startInstant, endInstant),
                    pageSize = HealthConnectReadConfig.PAGE_SIZE,
                    pageToken = pageToken
                )
            )

            results.addAll(response.records.map(mapper))
            pageToken = response.pageToken
        } while (pageToken != null)

        return results
    }
}