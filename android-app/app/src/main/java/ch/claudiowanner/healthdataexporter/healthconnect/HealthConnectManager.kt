package ch.claudiowanner.healthdataexporter.healthconnect

import android.content.Context
import androidx.health.connect.client.HealthConnectClient
import ch.claudiowanner.healthdataexporter.healthconnect.activity.ActivityReader
import ch.claudiowanner.healthdataexporter.healthconnect.sleep.SleepReader
import ch.claudiowanner.healthdataexporter.healthconnect.vitals.HeartRateReader
import ch.claudiowanner.healthdataexporter.model.activity.ActivityDayExportRecord
import ch.claudiowanner.healthdataexporter.model.sleep.SleepSessionExportRecord
import ch.claudiowanner.healthdataexporter.model.vitals.HeartRateDailyExportRecord
import ch.claudiowanner.healthdataexporter.model.vitals.HeartRateHourlyExportRecord
import java.time.Instant
import java.time.LocalDate

class HealthConnectManager(private val context: Context) {
    private fun client(): HealthConnectClient {
        return HealthConnectClient.getOrCreate(context)
    }

    private val activityReader: ActivityReader
        get() = ActivityReader(client())

    private val sleepReader: SleepReader
        get() = SleepReader(client())

    private val heartRateReader: HeartRateReader
        get() = HeartRateReader(client())

    fun getSdkStatus(): Int {
        return HealthConnectClient.getSdkStatus(
            context,
            HealthConnectAvailability.PROVIDER_PACKAGE_NAME
        )
    }

    suspend fun hasAllPermissions(): Boolean {
        val granted = client().permissionController.getGrantedPermissions()
        return granted.containsAll(HealthConnectAvailability.PERMISSIONS)
    }

    suspend fun readActivityExportRecords(
        startDateInclusive: LocalDate,
        endDateExclusive: LocalDate
    ): List<ActivityDayExportRecord> {
        return activityReader.readActivityExportRecords(
            startDateInclusive = startDateInclusive,
            endDateExclusive = endDateExclusive
        )
    }

    suspend fun readSleepSessions(
        startInstant: Instant,
        endInstant: Instant
    ): List<SleepSessionExportRecord> {
        return sleepReader.readSleepSessions(
            startInstant = startInstant,
            endInstant = endInstant
        )
    }

    suspend fun readHeartRateDailyRecords(
        startDateInclusive: LocalDate,
        endDateExclusive: LocalDate
    ): List<HeartRateDailyExportRecord> {
        return heartRateReader.readHeartRateDailyRecords(
            startDateInclusive = startDateInclusive,
            endDateExclusive = endDateExclusive
        )
    }

    suspend fun readHeartRateHourlyRecords(
        startInstant: Instant,
        endInstant: Instant
    ): List<HeartRateHourlyExportRecord> {
        return heartRateReader.readHeartRateHourlyRecords(
            startInstant = startInstant,
            endInstant = endInstant
        )
    }
}