import { formatMonthDetailsLabel } from '../../utils/date/dateFormatters';
import { formatDurationMinutes } from '../../utils/duration/durationFormatters';
import {
  getDailySegments,
  getPhaseSummaryItems,
} from '../../utils/sleepSessions/detailData';
import { formatSleepDateRange } from '../../utils/sleepSessions/formatters';
import { PERIODS } from '../../constants/periods';
import SleepPhaseStatsGrid from './SleepPhaseStatsGrid';
import SleepStagePreview from './SleepStagePreview';
import SleepStageTimeline from './SleepStageTimeline';

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
            : formatSleepDateRange(item.sleepDateStartKey, item.sleepDateEndKey)}
        </div>

        <div className="chart-details-value">
          {isMonthly
            ? `Ø ${formatDurationMinutes(item.averageSleepMinutes)} / Tag`
            : formatDurationMinutes(item.totalDurationMinutes)}
        </div>
      </div>

      {hasRecordedStages ? (
        <>
          {!isMonthly && (
            <SleepStagePreview
              segments={previewSegments}
              totalDurationMinutes={item.totalDurationMinutes}
            />
          )}

          <SleepPhaseStatsGrid items={phaseSummaryItems} />

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