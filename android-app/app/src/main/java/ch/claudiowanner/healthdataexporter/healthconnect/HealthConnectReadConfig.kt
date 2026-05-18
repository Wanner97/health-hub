package ch.claudiowanner.healthdataexporter.healthconnect

object HealthConnectReadConfig {
    const val PAGE_SIZE = 1000
    const val RAW_RECORD_CHUNK_DAYS = 365L
    const val DAILY_AGGREGATION_CHUNK_DAYS = 365L
    const val HOURLY_AGGREGATION_CHUNK_DAYS = 31L
}