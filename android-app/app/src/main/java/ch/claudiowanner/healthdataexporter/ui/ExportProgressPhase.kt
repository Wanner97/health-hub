package ch.claudiowanner.healthdataexporter.ui

enum class ExportProgressPhase(val label: String) {
    IDLE("Ready"),
    CHECKING_PERMISSIONS("Checking permissions"),
    READING_ACTIVITY("Reading activity data"),
    READING_SLEEP("Reading sleep data"),
    READING_HEART_RATE_DAILY("Reading daily heart rate data"),
    READING_HEART_RATE_HOURLY("Reading hourly heart rate data"),
    BUILDING_EXPORT("Building export payload"),
    WRITING_JSON("Writing JSON file"),
    LOADING_EXPORT("Loading export file"),
    SAVING_EXPORT("Saving export file"),
    PREPARING_SHARE("Preparing share"),
    COMPLETED("Completed"),
    FAILED("Failed")
}