package ch.claudiowanner.healthdataexporter.export

import android.content.Context
import androidx.health.connect.client.HealthConnectClient
import ch.claudiowanner.healthdataexporter.config.ExportConfig
import ch.claudiowanner.healthdataexporter.healthconnect.HealthConnectGateway
import ch.claudiowanner.healthdataexporter.model.ExportClusters
import ch.claudiowanner.healthdataexporter.model.ExportPayload
import ch.claudiowanner.healthdataexporter.model.activity.ActivityExportCluster
import ch.claudiowanner.healthdataexporter.model.activity.ActivityDayExportRecord
import ch.claudiowanner.healthdataexporter.model.sleep.SleepExportCluster
import ch.claudiowanner.healthdataexporter.model.sleep.SleepSessionExportRecord
import ch.claudiowanner.healthdataexporter.model.vitals.BloodOxygenDailyExportCluster
import ch.claudiowanner.healthdataexporter.model.vitals.BloodOxygenDailyExportRecord
import ch.claudiowanner.healthdataexporter.model.vitals.HeartRateDailyExportCluster
import ch.claudiowanner.healthdataexporter.model.vitals.HeartRateDailyExportRecord
import ch.claudiowanner.healthdataexporter.model.vitals.HeartRateHourlyExportCluster
import ch.claudiowanner.healthdataexporter.model.vitals.HeartRateHourlyExportRecord
import ch.claudiowanner.healthdataexporter.model.vitals.VitalsExportCluster
import ch.claudiowanner.healthdataexporter.serialization.ExportJsonSerializer
import ch.claudiowanner.healthdataexporter.storage.ExportFileWriter
import java.time.Instant
import java.time.LocalDate
import java.time.ZoneId

class ExportCoordinator(
    private val appContext: Context,
    private val healthConnectGateway: HealthConnectGateway,
    private val exportFileWriter: ExportFileWriter,
    private val exportJsonSerializer: ExportJsonSerializer
) {
    suspend fun executeExport(
        request: ExportRequest,
        onProgress: (ExportProgressUpdate) -> Unit
    ): Result<ExportExecutionResult> {
        return runCatching {
            reportPreparation(onProgress)

            when (healthConnectGateway.getSdkStatus()) {
                HealthConnectClient.SDK_AVAILABLE -> {
                    executeAvailableExport(request, onProgress)
                }

                HealthConnectClient.SDK_UNAVAILABLE -> {
                    error("Export failed: Health Connect is unavailable on this device.")
                }

                HealthConnectClient.SDK_UNAVAILABLE_PROVIDER_UPDATE_REQUIRED -> {
                    error("Export failed: Health Connect requires an update.")
                }

                else -> {
                    error("Export failed: Health Connect status is unknown.")
                }
            }
        }
    }

    private suspend fun executeAvailableExport(
        request: ExportRequest,
        onProgress: (ExportProgressUpdate) -> Unit
    ): ExportExecutionResult {
        ensurePermissionsGranted()

        val exportContext = createExportContext(request)
        val totalSteps = 7

        val activityRecords = readActivityRecords(
            request = request,
            endDateExclusive = exportContext.endDateExclusive,
            totalSteps = totalSteps,
            onProgress = onProgress
        )

        val sleepSessions = readSleepSessions(
            rangeStartInstant = exportContext.rangeStartInstant,
            now = exportContext.now,
            totalSteps = totalSteps,
            onProgress = onProgress
        )

        val heartRateDailyRecords = readHeartRateDailyRecords(
            request = request,
            endDateExclusive = exportContext.endDateExclusive,
            totalSteps = totalSteps,
            onProgress = onProgress
        )

        val bloodOxygenDailyRecords = readBloodOxygenDailyRecords(
            rangeStartInstant = exportContext.rangeStartInstant,
            now = exportContext.now,
            totalSteps = totalSteps,
            onProgress = onProgress
        )

        val heartRateHourlyRecords = readHeartRateHourlyRecords(
            rangeStartInstant = exportContext.rangeStartInstant,
            now = exportContext.now,
            totalSteps = totalSteps,
            onProgress = onProgress
        )

        ensureAnyDataFound(
            activityRecords = activityRecords,
            sleepSessions = sleepSessions,
            heartRateDailyRecords = heartRateDailyRecords,
            heartRateHourlyRecords = heartRateHourlyRecords,
            bloodOxygenDailyRecords = bloodOxygenDailyRecords
        )

        val payload = buildPayload(
            request = request,
            now = exportContext.now,
            rangeStartInstant = exportContext.rangeStartInstant,
            activityRecords = activityRecords,
            sleepSessions = sleepSessions,
            heartRateDailyRecords = heartRateDailyRecords,
            heartRateHourlyRecords = heartRateHourlyRecords,
            bloodOxygenDailyRecords = bloodOxygenDailyRecords,
            totalSteps = totalSteps,
            onProgress = onProgress
        )

        return writeExportFile(
            payload = payload,
            totalSteps = totalSteps,
            onProgress = onProgress
        )
    }

    private fun reportPreparation(
        onProgress: (ExportProgressUpdate) -> Unit
    ) {
        onProgress(
            ExportProgressUpdate(
                phase = ExportPhase.CHECKING_PERMISSIONS,
                title = "Preparing export",
                message = "Preparing export range and validating Health Connect access."
            )
        )
    }

    private suspend fun ensurePermissionsGranted() {
        if (!healthConnectGateway.hasAllPermissions()) {
            error("Export failed: required permissions are missing.")
        }
    }

    private fun createExportContext(
        request: ExportRequest
    ): ExportContext {
        val zoneId = ZoneId.systemDefault()
        val now = Instant.now()
        val today = LocalDate.now(zoneId)
        val endDateExclusive = today.plusDays(1)
        val rangeStartInstant = request.startDateInclusive.atStartOfDay(zoneId).toInstant()

        return ExportContext(
            now = now,
            endDateExclusive = endDateExclusive,
            rangeStartInstant = rangeStartInstant
        )
    }

    private suspend fun readActivityRecords(
        request: ExportRequest,
        endDateExclusive: LocalDate,
        totalSteps: Int,
        onProgress: (ExportProgressUpdate) -> Unit
    ): List<ActivityDayExportRecord> {
        onProgress(
            ExportProgressUpdate(
                phase = ExportPhase.READING_ACTIVITY,
                title = "Reading activity data",
                message = "Loading steps and distance records from Health Connect.",
                progressCurrent = 1,
                progressTotal = totalSteps
            )
        )

        return healthConnectGateway.readActivityExportRecords(
            startDateInclusive = request.startDateInclusive,
            endDateExclusive = endDateExclusive
        )
    }

    private suspend fun readSleepSessions(
        rangeStartInstant: Instant,
        now: Instant,
        totalSteps: Int,
        onProgress: (ExportProgressUpdate) -> Unit
    ): List<SleepSessionExportRecord> {
        onProgress(
            ExportProgressUpdate(
                phase = ExportPhase.READING_SLEEP,
                title = "Reading sleep data",
                message = "Loading sleep sessions from Health Connect.",
                progressCurrent = 2,
                progressTotal = totalSteps
            )
        )

        return healthConnectGateway.readSleepSessions(
            startInstant = rangeStartInstant,
            endInstant = now
        )
    }

    private suspend fun readHeartRateDailyRecords(
        request: ExportRequest,
        endDateExclusive: LocalDate,
        totalSteps: Int,
        onProgress: (ExportProgressUpdate) -> Unit
    ): List<HeartRateDailyExportRecord> {
        onProgress(
            ExportProgressUpdate(
                phase = ExportPhase.READING_HEART_RATE_DAILY,
                title = "Reading daily heart rate data",
                message = "Loading daily heart rate summaries from Health Connect.",
                progressCurrent = 3,
                progressTotal = totalSteps
            )
        )

        return healthConnectGateway.readHeartRateDailyRecords(
            startDateInclusive = request.startDateInclusive,
            endDateExclusive = endDateExclusive
        )
    }

    private suspend fun readBloodOxygenDailyRecords(
        rangeStartInstant: Instant,
        now: Instant,
        totalSteps: Int,
        onProgress: (ExportProgressUpdate) -> Unit
    ): List<BloodOxygenDailyExportRecord> {
        onProgress(
            ExportProgressUpdate(
                phase = ExportPhase.READING_BLOOD_OXYGEN_DAILY,
                title = "Reading daily blood oxygen data",
                message = "Loading daily blood oxygen summaries from Health Connect.",
                progressCurrent = 4,
                progressTotal = totalSteps
            )
        )

        return healthConnectGateway.readBloodOxygenDailyRecords(
            startInstant = rangeStartInstant,
            endInstant = now
        )
    }

    private suspend fun readHeartRateHourlyRecords(
        rangeStartInstant: Instant,
        now: Instant,
        totalSteps: Int,
        onProgress: (ExportProgressUpdate) -> Unit
    ): List<HeartRateHourlyExportRecord> {
        onProgress(
            ExportProgressUpdate(
                phase = ExportPhase.READING_HEART_RATE_HOURLY,
                title = "Reading hourly heart rate data",
                message = "Loading hourly heart rate summaries from Health Connect. This can take some time.",
                progressCurrent = 5,
                progressTotal = totalSteps
            )
        )

        return healthConnectGateway.readHeartRateHourlyRecords(
            startInstant = rangeStartInstant,
            endInstant = now
        )
    }

    private fun ensureAnyDataFound(
        activityRecords: List<ActivityDayExportRecord>,
        sleepSessions: List<SleepSessionExportRecord>,
        heartRateDailyRecords: List<HeartRateDailyExportRecord>,
        heartRateHourlyRecords: List<HeartRateHourlyExportRecord>,
        bloodOxygenDailyRecords: List<BloodOxygenDailyExportRecord>
    ) {
        if (
            activityRecords.isEmpty() &&
            sleepSessions.isEmpty() &&
            heartRateDailyRecords.isEmpty() &&
            heartRateHourlyRecords.isEmpty() &&
            bloodOxygenDailyRecords.isEmpty()
        ) {
            error("No activity, sleep, heart rate, or blood oxygen data found.")
        }
    }

    private fun buildPayload(
        request: ExportRequest,
        now: Instant,
        rangeStartInstant: Instant,
        activityRecords: List<ActivityDayExportRecord>,
        sleepSessions: List<SleepSessionExportRecord>,
        heartRateDailyRecords: List<HeartRateDailyExportRecord>,
        heartRateHourlyRecords: List<HeartRateHourlyExportRecord>,
        bloodOxygenDailyRecords: List<BloodOxygenDailyExportRecord>,
        totalSteps: Int,
        onProgress: (ExportProgressUpdate) -> Unit
    ): ExportPayload {
        onProgress(
            ExportProgressUpdate(
                phase = ExportPhase.BUILDING_EXPORT,
                title = "Building export payload",
                message = "Combining all loaded records into one export file.",
                progressCurrent = 6,
                progressTotal = totalSteps
            )
        )

        return ExportPayload(
            exportVersion = ExportConfig.EXPORT_VERSION,
            source = "health-connect",
            exportedAt = now.toString(),
            exportType = request.exportType.jsonValue,
            rangeDays = request.rangeDays,
            rangeStart = rangeStartInstant.toString(),
            rangeEnd = now.toString(),
            clusters = ExportClusters(
                activity = ActivityExportCluster(
                    records = activityRecords
                ),
                sleep = SleepExportCluster(
                    sessions = sleepSessions
                ),
                vitals = VitalsExportCluster(
                    heartRateDaily = HeartRateDailyExportCluster(
                        records = heartRateDailyRecords
                    ),
                    heartRateHourly = HeartRateHourlyExportCluster(
                        records = heartRateHourlyRecords
                    ),
                    bloodOxygenDaily = BloodOxygenDailyExportCluster(
                        records = bloodOxygenDailyRecords
                    )
                )
            )
        )
    }

    private suspend fun writeExportFile(
        payload: ExportPayload,
        totalSteps: Int,
        onProgress: (ExportProgressUpdate) -> Unit
    ): ExportExecutionResult {
        onProgress(
            ExportProgressUpdate(
                phase = ExportPhase.WRITING_JSON,
                title = "Writing JSON file",
                message = "Saving the export to app storage.",
                progressCurrent = 7,
                progressTotal = totalSteps
            )
        )

        val json = exportJsonSerializer.serialize(payload)
        val file = exportFileWriter.writeExportJson(appContext, json).getOrThrow()

        return ExportExecutionResult(
            filePath = file.absolutePath,
            exportPreview = json,
            progressTotal = totalSteps
        )
    }

    private data class ExportContext(
        val now: Instant,
        val endDateExclusive: LocalDate,
        val rangeStartInstant: Instant
    )
}