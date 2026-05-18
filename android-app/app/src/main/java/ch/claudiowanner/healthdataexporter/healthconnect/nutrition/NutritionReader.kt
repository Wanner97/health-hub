package ch.claudiowanner.healthdataexporter.healthconnect.nutrition

import androidx.health.connect.client.HealthConnectClient
import androidx.health.connect.client.records.NutritionRecord
import ch.claudiowanner.healthdataexporter.model.nutrition.NutritionExportRecord
import ch.claudiowanner.healthdataexporter.healthconnect.common.HealthConnectReadSupport
import java.time.Instant
import java.time.ZoneId

class NutritionReader(
    private val client: HealthConnectClient
) {
    private val nutritionRecordMapper = NutritionRecordMapper()
    private val readSupport = HealthConnectReadSupport(client)

    suspend fun readNutritionRecords(
        startInstant: Instant,
        endInstant: Instant
    ): List<NutritionExportRecord> {
        val zoneId = ZoneId.systemDefault()

        return readSupport.readChunkedPagedRecords(
            recordType = NutritionRecord::class,
            startInstant = startInstant,
            endInstant = endInstant
        ) { record ->
            nutritionRecordMapper.map(
                record = record,
                zoneId = zoneId
            )
        }.sortedBy { it.startTime }
    }
}