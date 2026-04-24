package ch.claudiowanner.healthdataexporter.ui.preview

import androidx.compose.ui.text.AnnotatedString

data class PreviewChunk(
    val id: Int,
    val content: AnnotatedString
)