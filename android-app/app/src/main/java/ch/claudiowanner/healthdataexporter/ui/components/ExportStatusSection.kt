package ch.claudiowanner.healthdataexporter.ui.components

import androidx.compose.runtime.LaunchedEffect
import androidx.compose.runtime.getValue
import androidx.compose.runtime.mutableIntStateOf
import androidx.compose.runtime.remember
import androidx.compose.runtime.setValue
import kotlinx.coroutines.delay
import androidx.compose.animation.core.LinearEasing
import androidx.compose.animation.core.animateFloat
import androidx.compose.animation.core.animateFloatAsState
import androidx.compose.animation.core.infiniteRepeatable
import androidx.compose.animation.core.rememberInfiniteTransition
import androidx.compose.animation.core.tween
import androidx.compose.foundation.background
import androidx.compose.foundation.layout.Box
import androidx.compose.foundation.layout.fillMaxHeight
import androidx.compose.foundation.layout.height
import androidx.compose.foundation.shape.RoundedCornerShape
import androidx.compose.ui.draw.clip
import androidx.compose.ui.draw.drawWithCache
import androidx.compose.ui.geometry.Offset
import androidx.compose.ui.graphics.Brush
import androidx.compose.ui.graphics.Color
import androidx.compose.foundation.layout.fillMaxWidth
import androidx.compose.foundation.layout.Column
import androidx.compose.foundation.layout.Arrangement
import androidx.compose.runtime.Composable
import androidx.compose.ui.Modifier
import androidx.compose.ui.unit.dp
import androidx.compose.material3.LinearProgressIndicator
import androidx.compose.material3.MaterialTheme
import androidx.compose.material3.Text
import ch.claudiowanner.healthdataexporter.ui.ExportUiState

@Composable
fun ExportStatusSection(
    uiState: ExportUiState,
    modifier: Modifier = Modifier
) {
    Column(
        modifier = modifier,
        verticalArrangement = Arrangement.spacedBy(8.dp)
    ) {
        Text(
            text = "Status",
            style = MaterialTheme.typography.titleMedium
        )

        Text(
            text = uiState.statusTitle,
            style = MaterialTheme.typography.titleSmall
        )

        Text(
            text = uiState.statusMessage,
            style = MaterialTheme.typography.bodyMedium
        )

        if (uiState.isBusy) {
            if (uiState.hasDeterminateProgress) {
                val progress = (
                        uiState.progressCurrent!!.toFloat() /
                                uiState.progressTotal!!.toFloat()
                        ).coerceIn(0f, 1f)

                AnimatedDeterminateProgressBar(
                    progress = progress,
                    isActive = uiState.isBusy,
                    modifier = Modifier
                        .fillMaxWidth()
                        .height(8.dp)
                )

                Text(
                    text = "Step ${uiState.progressCurrent} of ${uiState.progressTotal}",
                    style = MaterialTheme.typography.bodySmall
                )
            } else {
                LinearProgressIndicator(
                    modifier = Modifier.fillMaxWidth()
                )
            }

            AnimatedPhaseLabel(
                baseLabel = uiState.currentPhase.label,
                isActive = true
            )
        }

        if (!uiState.lastExportPath.isNullOrBlank()) {
            Text(
                text = "Latest file: ${uiState.lastExportPath}",
                style = MaterialTheme.typography.bodySmall
            )
        }
    }
}

@Composable
private fun AnimatedDeterminateProgressBar(
    progress: Float,
    isActive: Boolean,
    modifier: Modifier = Modifier
) {
    val clampedProgress = progress.coerceIn(0f, 1f)

    val animatedProgress by animateFloatAsState(
        targetValue = clampedProgress,
        animationSpec = tween(durationMillis = 450),
        label = "animatedProgress"
    )

    val infiniteTransition = rememberInfiniteTransition(label = "progressAlive")
    val shimmerOffset by infiniteTransition.animateFloat(
        initialValue = -1f,
        targetValue = 2f,
        animationSpec = infiniteRepeatable(
            animation = tween(
                durationMillis = 1100,
                easing = LinearEasing
            )
        ),
        label = "shimmerOffset"
    )

    val trackColor = MaterialTheme.colorScheme.surfaceVariant
    val fillColor = MaterialTheme.colorScheme.primary
    val shimmerColor = MaterialTheme.colorScheme.onPrimary.copy(alpha = 0.28f)

    Box(
        modifier = modifier
            .clip(RoundedCornerShape(percent = 50))
            .background(trackColor)
    ) {
        Box(
            modifier = Modifier
                .fillMaxHeight()
                .fillMaxWidth(animatedProgress)
                .background(fillColor)
                .drawWithCache {
                    val brush = Brush.linearGradient(
                        colors = listOf(
                            Color.Transparent,
                            shimmerColor,
                            Color.Transparent
                        ),
                        start = Offset(size.width * (shimmerOffset - 0.35f), 0f),
                        end = Offset(size.width * shimmerOffset, size.height)
                    )

                    onDrawWithContent {
                        drawContent()

                        if (isActive && animatedProgress > 0f) {
                            drawRect(brush = brush)
                        }
                    }
                }
        )
    }
}

@Composable
private fun AnimatedPhaseLabel(
    baseLabel: String,
    isActive: Boolean,
    modifier: Modifier = Modifier
) {
    var dotCount by remember(baseLabel, isActive) {
        mutableIntStateOf(0)
    }

    LaunchedEffect(baseLabel, isActive) {
        if (!isActive) {
            dotCount = 0
            return@LaunchedEffect
        }

        while (true) {
            delay(400)
            dotCount = (dotCount + 1) % 4
        }
    }

    val animatedSuffix = when (dotCount) {
        0 -> "\u00A0\u00A0\u00A0"
        1 -> ".\u00A0\u00A0"
        2 -> "..\u00A0"
        else -> "..."
    }

    Text(
        text = baseLabel + animatedSuffix,
        style = MaterialTheme.typography.bodySmall,
        modifier = modifier
    )
}