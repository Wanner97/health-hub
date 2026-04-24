package ch.claudiowanner.healthdataexporter.ui.preview

import androidx.compose.ui.graphics.Color
import ch.claudiowanner.healthdataexporter.config.ExportConfig

class ExportPreviewChunkBuilder {

    fun build(json: String): List<PreviewChunk> {
        val lines = json.lines()

        return lines
            .chunked(ExportConfig.FULL_PREVIEW_CHUNK_LINE_COUNT)
            .mapIndexed { index, chunkLines ->
                val chunkText = chunkLines.joinToString(separator = "\n")

                PreviewChunk(
                    id = index,
                    content = buildHighlightedJson(
                        json = chunkText,
                        colors = JsonHighlightColors(
                            keyColor = Color(0xFF4FC3F7),
                            stringColor = Color(0xFFCE93D8),
                            numberColor = Color(0xFFFFCC80),
                            literalColor = Color(0xFFEF9A9A)
                        )
                    )
                )
            }
    }
}