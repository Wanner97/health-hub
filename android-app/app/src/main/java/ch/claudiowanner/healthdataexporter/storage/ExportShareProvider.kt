package ch.claudiowanner.healthdataexporter.storage

import android.content.Context
import androidx.core.content.FileProvider
import ch.claudiowanner.healthdataexporter.config.ExportConfig

class ExportShareProvider(
    private val exportFileWriter: ExportFileWriter
) {
    fun prepareLatestExportForSharing(
        context: Context
    ): Result<ExportShareTarget> {
        return runCatching {
            val latestFile = exportFileWriter.getLatestExportFile(context)
                ?: error("No export file found to share.")

            val uri = FileProvider.getUriForFile(
                context,
                ExportConfig.FILE_PROVIDER_AUTHORITY,
                latestFile
            )

            ExportShareTarget(
                file = latestFile,
                uri = uri
            )
        }
    }
}