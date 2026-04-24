import SummaryCard from '../SummaryCard';

function BloodOxygenStatsSummary({
  rangeLabel,
  dayCount,
  averagePercent,
  minPercent,
  maxPercent,
  totalMeasurements,
}) {
  return (
    <section className="summary-grid summary-grid--blood-oxygen">
      <SummaryCard title="Zeitraum" value={rangeLabel} />
      <SummaryCard title="Tage im Datensatz" value={dayCount} />
      <SummaryCard title="Ø SpO₂" value={averagePercent} />
      <SummaryCard title="Min %" value={minPercent} />
      <SummaryCard title="Max %" value={maxPercent} />
      <SummaryCard title="Messungen" value={totalMeasurements} />
    </section>
  );
}

export default BloodOxygenStatsSummary;