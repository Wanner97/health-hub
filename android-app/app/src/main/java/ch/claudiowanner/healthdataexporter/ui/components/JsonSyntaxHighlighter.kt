package ch.claudiowanner.healthdataexporter.ui.components

import androidx.compose.ui.graphics.Color
import androidx.compose.ui.text.AnnotatedString
import androidx.compose.ui.text.SpanStyle
import androidx.compose.ui.text.buildAnnotatedString

data class JsonHighlightColors(
    val keyColor: Color,
    val stringColor: Color,
    val numberColor: Color,
    val literalColor: Color
)

fun buildHighlightedJson(
    json: String,
    colors: JsonHighlightColors
): AnnotatedString {
    return buildAnnotatedString {
        var index = 0

        while (index < json.length) {
            val current = json[index]

            when {
                current == '"' -> {
                    val endIndex = findStringEnd(json, index)
                    val token = json.substring(index, endIndex + 1)
                    val isKey = isJsonKey(json, endIndex + 1)

                    pushStyle(
                        SpanStyle(
                            color = if (isKey) colors.keyColor else colors.stringColor
                        )
                    )
                    append(token)
                    pop()

                    index = endIndex + 1
                }

                current.isDigit() || current == '-' -> {
                    val endIndex = findNumberEnd(json, index)
                    val token = json.substring(index, endIndex)

                    pushStyle(SpanStyle(color = colors.numberColor))
                    append(token)
                    pop()

                    index = endIndex
                }

                json.startsWith("true", index) -> {
                    pushStyle(SpanStyle(color = colors.literalColor))
                    append("true")
                    pop()
                    index += 4
                }

                json.startsWith("false", index) -> {
                    pushStyle(SpanStyle(color = colors.literalColor))
                    append("false")
                    pop()
                    index += 5
                }

                json.startsWith("null", index) -> {
                    pushStyle(SpanStyle(color = colors.literalColor))
                    append("null")
                    pop()
                    index += 4
                }

                else -> {
                    append(current)
                    index++
                }
            }
        }
    }
}

private fun findStringEnd(text: String, startIndex: Int): Int {
    var index = startIndex + 1
    var escaped = false

    while (index < text.length) {
        val current = text[index]

        if (escaped) {
            escaped = false
        } else {
            when (current) {
                '\\' -> escaped = true
                '"' -> return index
            }
        }

        index++
    }

    return text.lastIndex
}

private fun isJsonKey(text: String, startIndex: Int): Boolean {
    var index = startIndex

    while (index < text.length && text[index].isWhitespace()) {
        index++
    }

    return index < text.length && text[index] == ':'
}

private fun findNumberEnd(text: String, startIndex: Int): Int {
    var index = startIndex

    while (index < text.length) {
        val current = text[index]

        if (
            current.isDigit() ||
            current == '-' ||
            current == '+' ||
            current == '.' ||
            current == 'e' ||
            current == 'E'
        ) {
            index++
        } else {
            break
        }
    }

    return index
}