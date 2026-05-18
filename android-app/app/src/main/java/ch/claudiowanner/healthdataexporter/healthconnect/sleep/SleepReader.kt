package ch.claudiowanner.healthdataexporter.healthconnect.sleep

import androidx.health.connect.client.HealthConnectClient
import androidx.health.connect.client.records.SleepSessionRecord
import ch.claudiowanner.healthdataexporter.model.sleep.SleepSessionExportRecord
import ch.claudiowanner.healthdataexporter.model.sleep.SleepStageExportRecord
import ch.claudiowanner.healthdataexporter.healthconnect.common.HealthConnectReadSupport
import java.time.Duration
import java.time.Instant

class SleepReader(
    private val client: HealthConnectClient
) {
    private val readSupport = HealthConnectReadSupport(client)

    suspend fun readSleepSessions(
        startInstant: Instant,
        endInstant: Instant
    ): List<SleepSessionExportRecord> {
        return readSupport.readPagedRecords(
            recordType = SleepSessionRecord::class,
            startInstant = startInstant,
            endInstant = endInstant
        ) { session ->
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
        }.sortedBy { it.startTime }
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