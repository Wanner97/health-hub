package ch.claudiowanner.healthdataexporter.model.vitals

data class VitalsExportCluster(
    val heartRateDaily: HeartRateDailyExportCluster,
    val heartRateHourly: HeartRateHourlyExportCluster,
    val bloodOxygenDaily: BloodOxygenDailyExportCluster
)

data class HeartRateDailyExportCluster(
    val records: List<HeartRateDailyExportRecord>
)

data class HeartRateHourlyExportCluster(
    val records: List<HeartRateHourlyExportRecord>
)

data class BloodOxygenDailyExportCluster(
    val records: List<BloodOxygenDailyExportRecord>
)

data class HeartRateDailyExportRecord(
    val date: String,
    val minBpm: Long,
    val maxBpm: Long,
    val avgBpm: Long,
    val measurementCount: Long,
    val startTime: String,
    val endTime: String
)

data class HeartRateHourlyExportRecord(
    val date: String,
    val hour: Int,
    val minBpm: Long,
    val maxBpm: Long,
    val avgBpm: Long,
    val measurementCount: Long,
    val startTime: String,
    val endTime: String
)

data class BloodOxygenDailyExportRecord(
    val date: String,
    val minPercent: Double,
    val maxPercent: Double,
    val avgPercent: Double,
    val measurementCount: Long,
    val startTime: String,
    val endTime: String
)