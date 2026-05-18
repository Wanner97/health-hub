package ch.claudiowanner.healthdataexporter.healthconnect.body

import androidx.health.connect.client.HealthConnectClient
import androidx.health.connect.client.records.HeightRecord
import androidx.health.connect.client.records.WeightRecord
import ch.claudiowanner.healthdataexporter.model.body.HeightExportRecord
import ch.claudiowanner.healthdataexporter.model.body.WeightExportRecord
import ch.claudiowanner.healthdataexporter.healthconnect.common.HealthConnectReadSupport
import java.time.Instant
import java.time.ZoneId

class BodyReader(
    private val client: HealthConnectClient
) {
    private val readSupport = HealthConnectReadSupport(client)

    suspend fun readWeightRecords(
        startInstant: Instant,
        endInstant: Instant
    ): List<WeightExportRecord> {
        val zoneId = ZoneId.systemDefault()

        return readSupport.readChunkedPagedRecords(
            recordType = WeightRecord::class,
            startInstant = startInstant,
            endInstant = endInstant
        ) { record ->
            WeightExportRecord(
                date = record.time.atZone(zoneId).toLocalDate().toString(),
                weightKg = record.weight.inKilograms,
                measuredAt = record.time.toString()
            )
        }.sortedBy { it.measuredAt }
    }

    suspend fun readLatestHeightRecord(
        startInstant: Instant,
        endInstant: Instant
    ): HeightExportRecord? {
        val latestRecord = readSupport.readChunkedPagedRecords(
            recordType = HeightRecord::class,
            startInstant = startInstant,
            endInstant = endInstant
        ) { record ->
            record
        }.maxByOrNull { record ->
            record.time
        }

        return latestRecord?.let { record ->
            HeightExportRecord(
                heightCm = record.height.inMeters * 100.0,
                measuredAt = record.time.toString()
            )
        }
    }
}