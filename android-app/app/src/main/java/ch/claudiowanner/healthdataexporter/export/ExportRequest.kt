package ch.claudiowanner.healthdataexporter.export

import ch.claudiowanner.healthdataexporter.config.ExportType
import java.time.LocalDate

data class ExportRequest(
    val exportType: ExportType,
    val rangeDays: Int?,
    val startDateInclusive: LocalDate
)