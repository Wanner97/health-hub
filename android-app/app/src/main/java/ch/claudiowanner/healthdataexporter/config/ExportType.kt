package ch.claudiowanner.healthdataexporter.config

enum class ExportType(val jsonValue: String) {
    FULL_HISTORY("full_history"),
    ROLLING_WINDOW("rolling_window")
}