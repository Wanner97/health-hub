import { useEffect, useMemo, useState } from 'react';
import {
  SLEEP_STAGE_LABELS,
  SLEEP_STAGE_LEVELS,
} from '../../constants/sleepStages';
import { formatTimeUtc } from '../../utils/date/dateFormatters';
import {
  buildTimeLabels,
  getSegmentClassName,
  getSegmentValueClassName,
  isSameSegment,
  SLEEP_TIMELINE_LANE_HEIGHT_PERCENT,
} from '../../utils/sleepSessions/timelineHelpers';

function SleepStageTimeline({ segments, startTimeUtc, endTimeUtc }) {
  const [hoveredSegment, setHoveredSegment] = useState(null);
  const [selectedSegment, setSelectedSegment] = useState(null);

  const displayedSegment = useMemo(() => {
    return hoveredSegment ?? selectedSegment;
  }, [hoveredSegment, selectedSegment]);

  useEffect(() => {
    function handleDocumentMouseDown(event) {
      const clickedSegment = event.target.closest('.sleep-stage-timeline-segment');

      if (!clickedSegment) {
        setHoveredSegment(null);
        setSelectedSegment(null);
      }
    }

    document.addEventListener('mousedown', handleDocumentMouseDown);

    return () => {
      document.removeEventListener('mousedown', handleDocumentMouseDown);
    };
  }, []);

  if (!segments?.length || !startTimeUtc || !endTimeUtc) {
    return null;
  }

  const startMs = new Date(`${startTimeUtc}Z`).getTime();
  const endMs = new Date(`${endTimeUtc}Z`).getTime();

  if (Number.isNaN(startMs) || Number.isNaN(endMs) || endMs <= startMs) {
    return null;
  }

  const totalDuration = endMs - startMs;
  const timeLabels = buildTimeLabels(startMs, endMs);

  function handleSegmentClick(segment) {
    setSelectedSegment((currentSelectedSegment) => {
      if (isSameSegment(currentSelectedSegment, segment)) {
        return null;
      }

      return segment;
    });
  }

  return (
    <div className="sleep-stage-timeline">
      <div className="sleep-stage-timeline-chart">
        {[25, 50, 75].map((value) => (
          <div
            key={value}
            className="sleep-stage-timeline-grid-line"
            style={{ bottom: `${value}%` }}
          />
        ))}

        {segments.map((segment) => {
          const level = SLEEP_STAGE_LEVELS[segment.stage] ?? 1;
          const left = ((segment.startMs - startMs) / totalDuration) * 100;
          const width = ((segment.endMs - segment.startMs) / totalDuration) * 100;
          const bottom = (level - 1) * SLEEP_TIMELINE_LANE_HEIGHT_PERCENT;
          const isActive = isSameSegment(displayedSegment, segment);

          return (
            <div
              key={`${segment.startMs}-${segment.endMs}-${segment.stage}`}
              className={`sleep-stage-timeline-segment ${getSegmentClassName(
                segment.stage
              )} ${isActive ? 'is-active' : ''}`}
              style={{
                left: `${left}%`,
                width: `${Math.max(width, 0.45)}%`,
                bottom: `${bottom}%`,
                height: `${SLEEP_TIMELINE_LANE_HEIGHT_PERCENT}%`,
              }}
              onMouseEnter={() => setHoveredSegment(segment)}
              onMouseLeave={() => setHoveredSegment(null)}
              onClick={() => handleSegmentClick(segment)}
              title={`${formatTimeUtc(segment.startTimeUtc)} – ${formatTimeUtc(segment.endTimeUtc)}`}
            />
          );
        })}
      </div>

      <div className="sleep-stage-timeline-axis">
        {timeLabels.map((timeLabel) => (
          <span key={timeLabel.key}>{timeLabel.label}</span>
        ))}
      </div>

      <div
        className={`sleep-stage-segment-details ${
          displayedSegment ? '' : 'sleep-stage-segment-details--empty'
        }`}
      >
        <div className="sleep-stage-segment-details-label">
          {displayedSegment
            ? SLEEP_STAGE_LABELS[displayedSegment.stage] ?? ''
            : ''}
        </div>

        <div
          className={`sleep-stage-segment-details-value ${
            displayedSegment
              ? getSegmentValueClassName(displayedSegment.stage)
              : ''
          }`}
        >
          {displayedSegment
            ? `${formatTimeUtc(displayedSegment.startTimeUtc)} – ${formatTimeUtc(
                displayedSegment.endTimeUtc
              )}`
            : ''}
        </div>
      </div>
    </div>
  );
}

export default SleepStageTimeline;