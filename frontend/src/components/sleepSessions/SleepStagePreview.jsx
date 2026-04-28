function SleepStagePreview({ segments, totalDurationMinutes }) {
  if (!segments?.length || !totalDurationMinutes) {
    return null;
  }

  return (
    <div className="sleep-stage-preview">
      {segments.map((segment) => (
        <div
          key={segment.key}
          className={`sleep-stage-preview-segment ${segment.className}`}
          style={{
            width: `${(segment.minutes / totalDurationMinutes) * 100}%`,
          }}
        />
      ))}
    </div>
  );
}

export default SleepStagePreview;