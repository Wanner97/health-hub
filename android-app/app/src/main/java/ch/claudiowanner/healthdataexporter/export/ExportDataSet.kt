package ch.claudiowanner.healthdataexporter.export

import ch.claudiowanner.healthdataexporter.model.activity.ActivityDayExportRecord
import ch.claudiowanner.healthdataexporter.model.body.BodyExportCluster
import ch.claudiowanner.healthdataexporter.model.nutrition.NutritionExportRecord
import ch.claudiowanner.healthdataexporter.model.sleep.SleepSessionExportRecord
import ch.claudiowanner.healthdataexporter.model.vitals.BloodOxygenDailyExportRecord
import ch.claudiowanner.healthdataexporter.model.vitals.HeartRateDailyExportRecord
import ch.claudiowanner.healthdataexporter.model.vitals.HeartRateHourlyExportRecord

data class ExportDataSet(
    val activityRecords: List<ActivityDayExportRecord>,
    val bodyCluster: BodyExportCluster,
    val sleepSessions: List<SleepSessionExportRecord>,
    val heartRateDailyRecords: List<HeartRateDailyExportRecord>,
    val heartRateHourlyRecords: List<HeartRateHourlyExportRecord>,
    val bloodOxygenDailyRecords: List<BloodOxygenDailyExportRecord>,
    val nutritionRecords: List<NutritionExportRecord>
)