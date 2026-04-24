package ch.claudiowanner.healthdataexporter.config

import ch.claudiowanner.healthdataexporter.BuildConfig
import java.time.LocalDate

object ExportConfig {
    const val EXPORT_VERSION = BuildConfig.SERVICE_VERSION
    const val FULL_PREVIEW_CHUNK_LINE_COUNT = 80
    const val EXPORT_FILE_PREFIX = "health-export"

    private const val FILE_PROVIDER_SUFFIX = ".fileprovider"

    val FILE_PROVIDER_AUTHORITY: String
        get() = BuildConfig.APPLICATION_ID + FILE_PROVIDER_SUFFIX

    val FULL_HISTORY_START_DATE: LocalDate = LocalDate.of(2000, 1, 1)

    const val WINDOW_7_DAYS = 7
    const val WINDOW_31_DAYS = 31
    const val WINDOW_62_DAYS = 62

    val SUPPORTED_ROLLING_WINDOWS = listOf(
        WINDOW_7_DAYS,
        WINDOW_31_DAYS,
        WINDOW_62_DAYS
    )
}