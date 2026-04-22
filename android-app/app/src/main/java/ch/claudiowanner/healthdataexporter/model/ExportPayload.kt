package ch.claudiowanner.healthdataexporter.model

import ch.claudiowanner.healthdataexporter.model.activity.ActivityExportCluster
import ch.claudiowanner.healthdataexporter.model.sleep.SleepExportCluster
import ch.claudiowanner.healthdataexporter.model.vitals.VitalsExportCluster

data class ExportPayload(
    val exportVersion: String,
    val source: String,
    val exportedAt: String,
    val exportType: String,
    val rangeDays: Int?,
    val rangeStart: String,
    val rangeEnd: String,
    val clusters: ExportClusters
)

data class ExportClusters(
    val activity: ActivityExportCluster,
    val sleep: SleepExportCluster,
    val vitals: VitalsExportCluster
)