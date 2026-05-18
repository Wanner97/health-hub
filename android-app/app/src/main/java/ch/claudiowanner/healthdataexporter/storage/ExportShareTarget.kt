package ch.claudiowanner.healthdataexporter.storage

import android.net.Uri
import java.io.File

data class ExportShareTarget(
    val file: File,
    val uri: Uri
)