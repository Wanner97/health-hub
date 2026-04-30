package ch.claudiowanner.healthdataexporter.healthconnect

import android.content.Context
import androidx.health.connect.client.HealthConnectClient
import ch.claudiowanner.healthdataexporter.healthconnect.activity.ActivityReader
import ch.claudiowanner.healthdataexporter.healthconnect.body.BodyReader
import ch.claudiowanner.healthdataexporter.healthconnect.sleep.SleepReader
import ch.claudiowanner.healthdataexporter.healthconnect.vitals.BloodOxygenReader
import ch.claudiowanner.healthdataexporter.healthconnect.vitals.HeartRateReader
import ch.claudiowanner.healthdataexporter.healthconnect.nutrition.NutritionReader
import ch.claudiowanner.healthdataexporter.model.activity.ActivityDayExportRecord
import ch.claudiowanner.healthdataexporter.model.body.HeightExportRecord
import ch.claudiowanner.healthdataexporter.model.body.WeightExportRecord
import ch.claudiowanner.healthdataexporter.model.sleep.SleepSessionExportRecord
import ch.claudiowanner.healthdataexporter.model.vitals.BloodOxygenDailyExportRecord
import ch.claudiowanner.healthdataexporter.model.vitals.HeartRateDailyExportRecord
import ch.claudiowanner.healthdataexporter.model.vitals.HeartRateHourlyExportRecord
import ch.claudiowanner.healthdataexporter.model.nutrition.NutritionExportRecord
import java.time.Instant
import java.time.LocalDate

class HealthConnectGateway(
    private val context: Context
) {
    private val client: HealthConnectClient by lazy {
        HealthConnectClient.getOrCreate(context)
    }

    private val activityReader: ActivityReader by lazy {
        ActivityReader(client)
    }

    private val bodyReader: BodyReader by lazy {
        BodyReader(client)
    }

    private val sleepReader: SleepReader by lazy {
        SleepReader(client)
    }

    private val heartRateReader: HeartRateReader by lazy {
        HeartRateReader(client)
    }

    private val bloodOxygenReader: BloodOxygenReader by lazy {
        BloodOxygenReader(client)
    }

    private val nutritionReader: NutritionReader by lazy {
        NutritionReader(client)
    }

    fun getSdkStatus(): Int {
        return HealthConnectClient.getSdkStatus(
            context,
            HealthConnectAvailability.PROVIDER_PACKAGE_NAME
        )
    }

    suspend fun hasAllPermissions(): Boolean {
        val granted = client.permissionController.getGrantedPermissions()
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

    suspend fun readWeightRecords(
        startInstant: Instant,
        endInstant: Instant
    ): List<WeightExportRecord> {
        return bodyReader.readWeightRecords(
            startInstant = startInstant,
            endInstant = endInstant
        )
    }

    suspend fun readLatestHeightRecord(
        startInstant: Instant,
        endInstant: Instant
    ): HeightExportRecord? {
        return bodyReader.readLatestHeightRecord(
            startInstant = startInstant,
            endInstant = endInstant
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

    suspend fun readBloodOxygenDailyRecords(
        startInstant: Instant,
        endInstant: Instant
    ): List<BloodOxygenDailyExportRecord> {
        return bloodOxygenReader.readBloodOxygenDailyRecords(
            startInstant = startInstant,
            endInstant = endInstant
        )
    }

    suspend fun readNutritionRecords(
        startInstant: Instant,
        endInstant: Instant
    ): List<NutritionExportRecord> {
        return nutritionReader.readNutritionRecords(
            startInstant = startInstant,
            endInstant = endInstant
        )
    }
}