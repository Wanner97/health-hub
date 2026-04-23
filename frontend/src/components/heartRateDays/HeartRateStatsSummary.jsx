import SummaryCard from '../SummaryCard';

function HeartRateStatsSummary({
  rangeLabel,
  dayCount,
  averageBpm,
  minBpm,
  maxBpm,
  totalMeasurements,
}) {
  return (
    <section className="summary-grid summary-grid--heart-rate">
      <SummaryCard title="Zeitraum" value={rangeLabel} />
      <SummaryCard title="Tage im Datensatz" value={dayCount} />
      <SummaryCard title="Ø BPM" value={averageBpm} />
      <SummaryCard title="Min BPM" value={minBpm} />
      <SummaryCard title="Max BPM" value={maxBpm} />
      <SummaryCard title="Messungen" value={totalMeasurements} />
    </section>
  );
}

export default HeartRateStatsSummary;