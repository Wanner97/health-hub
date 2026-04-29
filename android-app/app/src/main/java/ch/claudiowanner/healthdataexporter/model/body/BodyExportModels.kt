package ch.claudiowanner.healthdataexporter.model.body

data class BodyExportCluster(
    val latestHeight: HeightExportRecord?,
    val weightRecords: List<WeightExportRecord>
)

data class HeightExportRecord(
    val heightCm: Double,
    val measuredAt: String
)

data class WeightExportRecord(
    val date: String,
    val weightKg: Double,
    val measuredAt: String
)