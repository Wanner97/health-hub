package ch.claudiowanner.healthdataexporter.export

enum class ExportOperationPhase {
    CHECKING_PERMISSIONS,
    READING_ACTIVITY,
    READING_SLEEP,
    READING_HEART_RATE_DAILY,
    READING_HEART_RATE_HOURLY,
    BUILDING_EXPORT,
    WRITING_JSON
}