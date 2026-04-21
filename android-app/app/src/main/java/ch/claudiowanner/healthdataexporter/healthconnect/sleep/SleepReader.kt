package ch.claudiowanner.healthdataexporter.healthconnect.sleep

import androidx.health.connect.client.HealthConnectClient
import androidx.health.connect.client.records.SleepSessionRecord
import androidx.health.connect.client.request.ReadRecordsRequest
import androidx.health.connect.client.time.TimeRangeFilter
import ch.claudiowanner.healthdataexporter.model.sleep.SleepSessionExportRecord
import ch.claudiowanner.healthdataexporter.model.sleep.SleepStageExportRecord
import java.time.Duration
import java.time.Instant

class SleepReader(
    private val client: HealthConnectClient
) {
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
            val response = client.readRecords(
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