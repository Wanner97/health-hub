import {
  formatDate,
  formatMonthDetailsLabel,
} from '../../utils/date/dateFormatters';
import { formatDurationMinutes } from '../../utils/duration/durationFormatters';
import { PERIODS } from '../../constants/periods';
import SleepStageTimeline from './SleepStageTimeline';

function formatSleepRange(startKey, endKey) {
  if (!startKey || !endKey) {
    return '';
  }

  return `${formatDate(startKey)} – ${formatDate(endKey)}`;
}

function formatPercentage(value) {
  if (value == null) {
    return '-';
  }

  return `${Math.round(value)}%`;
}

function getDailySegments(item) {
  if (!item) {
    return [];
  }

  const totalKnownMinutes =
    (item.awakeMinutes ?? 0) +
    (item.remMinutes ?? 0) +
    (item.lightMinutes ?? 0) +
    (item.deepMinutes ?? 0);

  const unknownMinutes = Math.max(
    0,
    (item.totalDurationMinutes ?? 0) - totalKnownMinutes
  );

  return [
    {
      key: 'awake',
      minutes: item.awakeMinutes ?? 0,
      className: 'sleep-stage-preview-segment--awake',
    },
    {
      key: 'rem',
      minutes: item.remMinutes ?? 0,
      className: 'sleep-stage-preview-segment--rem',
    },
    {
      key: 'light',
      minutes: item.lightMinutes ?? 0,
      className: 'sleep-stage-preview-segment--light',
    },
    {
      key: 'deep',
      minutes: item.deepMinutes ?? 0,
      className: 'sleep-stage-preview-segment--deep',
    },
    {
      key: 'unknown',
      minutes: unknownMinutes,
      className: 'sleep-stage-preview-segment--unknown',
    },
  ].filter((segment) => segment.minutes > 0);
}

function getPhaseSummaryItems(item, isMonthly) {
  return [
    {
      key: 'awake',
      label: 'Wach',
      minutes: isMonthly
        ? item.averageAwakeMinutes ?? 0
        : item.awakeMinutes ?? 0,
      percentage: item.awakePercentage,
      valueClassName: 'sleep-phase-stat-value--awake',
    },
    {
      key: 'rem',
      label: 'REM',
      minutes: isMonthly
        ? item.averageRemMinutes ?? 0
        : item.remMinutes ?? 0,
      percentage: item.remPercentage,
      valueClassName: 'sleep-phase-stat-value--rem',
    },
    {
      key: 'light',
      label: 'Leicht',
      minutes: isMonthly
        ? item.averageLightMinutes ?? 0
        : item.lightMinutes ?? 0,
      percentage: item.lightPercentage,
      valueClassName: 'sleep-phase-stat-value--light',
    },
    {
      key: 'deep',
      label: 'Tief',
      minutes: isMonthly
        ? item.averageDeepMinutes ?? 0
        : item.deepMinutes ?? 0,
      percentage: item.deepPercentage,
      valueClassName: 'sleep-phase-stat-value--deep',
    },
  ];
}

function SleepPhaseDetails({ item, period }) {
  const hasItem = Boolean(item);

  if (!hasItem) {
    return <div className="chart-details sleep-chart-details" />;
  }

  const isMonthly = period === PERIODS.TWELVE_MONTHS;
  const previewSegments = isMonthly ? [] : getDailySegments(item);
  const hasRecordedStages = (item.totalStageCount ?? 0) > 0;
  const phaseSummaryItems = getPhaseSummaryItems(item, isMonthly);

  return (
    <div className="chart-details sleep-chart-details">
      <div className="sleep-chart-details-header">
        <div className="chart-details-label">
          {isMonthly
            ? formatMonthDetailsLabel(item.monthKey)
            : formatSleepRange(item.sleepDateStartKey, item.sleepDateEndKey)}
        </div>

        <div className="chart-details-value">
          {isMonthly
            ? `Ø ${formatDurationMinutes(item.averageSleepMinutes)} / Tag`
            : formatDurationMinutes(item.totalDurationMinutes)}
        </div>
      </div>

      {hasRecordedStages ? (
        <>
          {!isMonthly && previewSegments.length > 0 && (
            <div className="sleep-stage-preview">
              {previewSegments.map((segment) => (
                <div
                  key={segment.key}
                  className={`sleep-stage-preview-segment ${segment.className}`}
                  style={{
                    width: `${(segment.minutes / item.totalDurationMinutes) * 100}%`,
                  }}
                />
              ))}
            </div>
          )}

          <div className="sleep-phase-stats-grid">
            {phaseSummaryItems.map((phaseItem) => (
              <div key={phaseItem.key} className="sleep-phase-stat">
                <span className="sleep-phase-stat-label">
                  {phaseItem.label} ({formatDurationMinutes(phaseItem.minutes)})
                </span>
                <span
                  className={`sleep-phase-stat-value ${phaseItem.valueClassName}`}
                >
                  {formatPercentage(phaseItem.percentage)}
                </span>
              </div>
            ))}
          </div>

          {!isMonthly && item.sleepTimelineSegments?.length > 0 && (
            <div className="sleep-phase-timeline-wrapper">
              <SleepStageTimeline
                segments={item.sleepTimelineSegments}
                startTimeUtc={item.timelineStartUtc}
                endTimeUtc={item.timelineEndUtc}
              />
            </div>
          )}
        </>
      ) : (
        <div className="sleep-no-phase-data">
          Keine Schlafphasen erfasst.
        </div>
      )}
    </div>
  );
}

export default SleepPhaseDetails;