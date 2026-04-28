export const SLEEP_TIMELINE_LANE_HEIGHT_PERCENT = 25;

export function getSegmentClassName(stage) {
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

export function getSegmentValueClassName(stage) {
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

export function buildTimeLabels(startMs, endMs) {
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

export function isSameSegment(a, b) {
  if (!a || !b) {
    return false;
  }

  return (
    a.stage === b.stage &&
    a.startMs === b.startMs &&
    a.endMs === b.endMs
  );
}