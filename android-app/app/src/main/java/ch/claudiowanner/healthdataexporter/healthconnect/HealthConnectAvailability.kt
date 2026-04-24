package ch.claudiowanner.healthdataexporter.healthconnect

import androidx.health.connect.client.permission.HealthPermission
import androidx.health.connect.client.records.DistanceRecord
import androidx.health.connect.client.records.HeartRateRecord
import androidx.health.connect.client.records.OxygenSaturationRecord
import androidx.health.connect.client.records.SleepSessionRecord
import androidx.health.connect.client.records.StepsRecord

object HealthConnectAvailability {
    const val PROVIDER_PACKAGE_NAME = "com.google.android.apps.healthdata"

    private const val READ_HEALTH_DATA_HISTORY =
        "android.permission.health.READ_HEALTH_DATA_HISTORY"

    val PERMISSIONS = setOf(
        HealthPermission.getReadPermission(StepsRecord::class),
        HealthPermission.getReadPermission(DistanceRecord::class),
        HealthPermission.getReadPermission(SleepSessionRecord::class),
        HealthPermission.getReadPermission(HeartRateRecord::class),
        HealthPermission.getReadPermission(OxygenSaturationRecord::class),
        READ_HEALTH_DATA_HISTORY
    )
}