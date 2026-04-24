package ch.claudiowanner.healthdataexporter.export

import android.content.Context
import androidx.health.connect.client.HealthConnectClient
import ch.claudiowanner.healthdataexporter.config.ExportConfig
import ch.claudiowanner.healthdataexporter.healthconnect.HealthConnectManager
import ch.claudiowanner.healthdataexporter.model.ExportClusters
import ch.claudiowanner.healthdataexporter.model.ExportPayload
import ch.claudiowanner.healthdataexporter.model.activity.ActivityExportCluster
import ch.claudiowanner.healthdataexporter.model.sleep.SleepExportCluster
import ch.claudiowanner.healthdataexporter.model.vitals.BloodOxygenDailyExportCluster
import ch.claudiowanner.healthdataexporter.model.vitals.HeartRateDailyExportCluster
import ch.claudiowanner.healthdataexporter.model.vitals.HeartRateHourlyExportCluster
import ch.claudiowanner.healthdataexporter.model.vitals.VitalsExportCluster
import ch.claudiowanner.healthdataexporter.serialization.ExportJsonSerializer
import ch.claudiowanner.healthdataexporter.storage.ExportFileWriter
import java.time.Instant
import java.time.LocalDate
import java.time.ZoneId

class ExportCoordinator(
    private val appContext: Context,
    private val healthConnectManager: HealthConnectManager,
    private val exportFileWriter: ExportFileWriter,
    private val exportJsonSerializer: ExportJsonSerializer
) {
    suspend fun executeExport(
        request: ExportRequest,
        onProgress: (ExportProgressUpdate) -> Unit
    ): Result<ExportExecutionResult> {
        return runCatching {
            onProgress(
                ExportProgressUpdate(
                    phase = ExportOperationPhase.CHECKING_PERMISSIONS,
                    title = "Preparing export",
                    message = "Preparing export range and validating Health Connect access."
                )
            )

            when (healthConnectManager.getSdkStatus()) {
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
        if (!healthConnectManager.hasAllPermissions()) {
            error("Export failed: required permissions are missing.")
        }

        val zoneId = ZoneId.systemDefault()
        val now = Instant.now()
        val today = LocalDate.now(zoneId)
        val endDateExclusive = today.plusDays(1)
        val rangeStartInstant = request.startDateInclusive.atStartOfDay(zoneId).toInstant()

        val totalSteps = 7

        onProgress(
            ExportProgressUpdate(
                phase = ExportOperationPhase.READING_ACTIVITY,
                title = "Reading activity data",
                message = "Loading steps and distance records from Health Connect.",
                progressCurrent = 1,
                progressTotal = totalSteps
            )
        )
        val activityRecords = healthConnectManager.readActivityExportRecords(
            startDateInclusive = request.startDateInclusive,
            endDateExclusive = endDateExclusive
        )

        onProgress(
            ExportProgressUpdate(
                phase = ExportOperationPhase.READING_SLEEP,
                title = "Reading sleep data",
                message = "Loading sleep sessions from Health Connect.",
                progressCurrent = 2,
                progressTotal = totalSteps
            )
        )
        val sleepSessions = healthConnectManager.readSleepSessions(
            startInstant = rangeStartInstant,
            endInstant = now
        )

        onProgress(
            ExportProgressUpdate(
                phase = ExportOperationPhase.READING_HEART_RATE_DAILY,
                title = "Reading daily heart rate data",
                message = "Loading daily heart rate summaries from Health Connect.",
                progressCurrent = 3,
                progressTotal = totalSteps
            )
        )
        val heartRateDailyRecords = healthConnectManager.readHeartRateDailyRecords(
            startDateInclusive = request.startDateInclusive,
            endDateExclusive = endDateExclusive
        )

        onProgress(
            ExportProgressUpdate(
                phase = ExportOperationPhase.READING_BLOOD_OXYGEN_DAILY,
                title = "Reading daily blood oxygen data",
                message = "Loading daily blood oxygen summaries from Health Connect.",
                progressCurrent = 4,
                progressTotal = totalSteps
            )
        )
        val bloodOxygenDailyRecords = healthConnectManager.readBloodOxygenDailyRecords(
            startInstant = rangeStartInstant,
            endInstant = now
        )

        onProgress(
            ExportProgressUpdate(
                phase = ExportOperationPhase.READING_HEART_RATE_HOURLY,
                title = "Reading hourly heart rate data",
                message = "Loading hourly heart rate summaries from Health Connect. This can take some time.",
                progressCurrent = 5,
                progressTotal = totalSteps
            )
        )
        val heartRateHourlyRecords = healthConnectManager.readHeartRateHourlyRecords(
            startInstant = rangeStartInstant,
            endInstant = now
        )

        if (
            activityRecords.isEmpty() &&
            sleepSessions.isEmpty() &&
            heartRateDailyRecords.isEmpty() &&
            heartRateHourlyRecords.isEmpty() &&
            bloodOxygenDailyRecords.isEmpty()
        ) {
            error("No activity, sleep, heart rate, or blood oxygen data found.")
        }

        onProgress(
            ExportProgressUpdate(
                phase = ExportOperationPhase.BUILDING_EXPORT,
                title = "Building export payload",
                message = "Combining all loaded records into one export file.",
                progressCurrent = 6,
                progressTotal = totalSteps
            )
        )

        val payload = ExportPayload(
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

        val json = exportJsonSerializer.serialize(payload)

        onProgress(
            ExportProgressUpdate(
                phase = ExportOperationPhase.WRITING_JSON,
                title = "Writing JSON file",
                message = "Saving the export to app storage.",
                progressCurrent = 7,
                progressTotal = totalSteps
            )
        )

        val file = exportFileWriter.writeExportJson(appContext, json).getOrThrow()

        return ExportExecutionResult(
            filePath = file.absolutePath,
            exportPreview = json,
            progressTotal = totalSteps
        )
    }
}