package ch.claudiowanner.healthdataexporter.healthconnect

import android.content.Context
import androidx.health.connect.client.HealthConnectClient
import androidx.health.connect.client.permission.HealthPermission
import androidx.health.connect.client.records.StepsRecord
import androidx.health.connect.client.request.AggregateRequest
import androidx.health.connect.client.time.TimeRangeFilter
import ch.claudiowanner.healthdataexporter.model.StepExportRecord
import java.time.Instant
import java.time.LocalDate
import java.time.ZoneId

class HealthConnectManager(private val context: Context) {

    companion object {
        const val PROVIDER_PACKAGE_NAME = "com.google.android.apps.healthdata"

        val PERMISSIONS = setOf(
            HealthPermission.getReadPermission(StepsRecord::class)
        )
    }

    fun getSdkStatus(): Int {
        return HealthConnectClient.getSdkStatus(context, PROVIDER_PACKAGE_NAME)
    }

    private fun client(): HealthConnectClient {
        return HealthConnectClient.getOrCreate(context)
    }

    suspend fun hasAllPermissions(): Boolean {
        val granted = client().permissionController.getGrantedPermissions()
        return granted.containsAll(PERMISSIONS)
    }

    suspend fun readTodaySteps(): Long {
        val zoneId = ZoneId.systemDefault()
        val startOfDay = LocalDate.now(zoneId).atStartOfDay(zoneId).toInstant()
        val now = Instant.now()

        return aggregateStepsBetween(startOfDay, now)
    }

    suspend fun readTodayStepExportRecord(): StepExportRecord {
        val zoneId = ZoneId.systemDefault()
        val today = LocalDate.now(zoneId)

        return readStepExportRecordForDate(today)
    }

    suspend fun readStepExportRecordsForLastDays(days: Int): List<StepExportRecord> {
        require(days > 0) { "days must be greater than 0." }

        val zoneId = ZoneId.systemDefault()
        val today = LocalDate.now(zoneId)

        return (days - 1 downTo 0).map { offset ->
            val date = today.minusDays(offset.toLong())
            readStepExportRecordForDate(date)
        }
    }

    private suspend fun readStepExportRecordForDate(date: LocalDate): StepExportRecord {
        val zoneId = ZoneId.systemDefault()
        val start = date.atStartOfDay(zoneId).toInstant()
        val endExclusive = if (date == LocalDate.now(zoneId)) {
            Instant.now()
        } else {
            date.plusDays(1).atStartOfDay(zoneId).toInstant()
        }

        val steps = aggregateStepsBetween(start, endExclusive)

        return StepExportRecord(
            count = steps,
            startTime = start.toString(),
            endTime = endExclusive.toString()
        )
    }

    private suspend fun aggregateStepsBetween(
        start: Instant,
        end: Instant
    ): Long {
        val response = client().aggregate(
            AggregateRequest(
                metrics = setOf(StepsRecord.COUNT_TOTAL),
                timeRangeFilter = TimeRangeFilter.between(start, end)
            )
        )

        return response[StepsRecord.COUNT_TOTAL] ?: 0L
    }
}