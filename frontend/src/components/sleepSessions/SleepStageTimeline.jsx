import { useEffect, useMemo, useState } from 'react';
import { formatTimeUtc } from '../../utils/date/dateFormatters';

const STAGE_LEVELS = {
  deep: 1,
  light: 2,
  rem: 3,
  awake: 4,
};

const LANE_HEIGHT_PERCENT = 25;

function getSegmentClassName(stage) {
  if (stage === 'awake') {
    return 'sleep-stage-timeline-segment--awake';
  }

  if (stage === 'rem') {
    return 'sleep-stage-timeline-segment--rem';
  }

  if (stage === 'light') {
    return 'sleep-stage-timeline-segment--light';
  }

  return 'sleep-stage-timeline-segment--deep';
}

function getStageLabel(stage) {
  if (stage === 'awake') {
    return 'Wach';
  }

  if (stage === 'rem') {
    return 'REM';
  }

  if (stage === 'light') {
    return 'Leicht';
  }

  return 'Tief';
}

function getSegmentValueClassName(stage) {
  if (stage === 'awake') {
    return 'sleep-stage-segment-details-value--awake';
  }

  if (stage === 'rem') {
    return 'sleep-stage-segment-details-value--rem';
  }

  if (stage === 'light') {
    return 'sleep-stage-segment-details-value--light';
  }

  return 'sleep-stage-segment-details-value--deep';
}

function buildTimeLabels(startMs, endMs) {
  if (!startMs || !endMs || endMs <= startMs) {
    return [];
  }

  const totalDuration = endMs - startMs;

  return [0, 1 / 3, 2 / 3, 1].map((ratio, index) => {
    const valueMs = Math.round(startMs + totalDuration * ratio);

    return {
      key: `${valueMs}-${index}`,
      label: new Date(valueMs).toLocaleTimeString('de-CH', {
        hour: '2-digit',
        minute: '2-digit',
      }),
    };
  });
}

function isSameSegment(a, b) {
  if (!a || !b) {
    return false;
  }

  return (
    a.stage === b.stage &&
    a.startMs === b.startMs &&
    a.endMs === b.endMs
  );
}

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
          const level = STAGE_LEVELS[segment.stage] ?? 1;
          const left = ((segment.startMs - startMs) / totalDuration) * 100;
          const width = ((segment.endMs - segment.startMs) / totalDuration) * 100;
          const bottom = (level - 1) * LANE_HEIGHT_PERCENT;
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
                height: `${LANE_HEIGHT_PERCENT}%`,
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
          {displayedSegment ? getStageLabel(displayedSegment.stage) : ''}
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