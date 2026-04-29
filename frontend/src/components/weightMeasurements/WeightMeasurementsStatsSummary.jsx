import SummaryCard from '../SummaryCard';

function WeightMeasurementsStatsSummary({
  rangeLabel,
  measurementCount,
  averageWeightKg,
  minWeightKg,
  maxWeightKg,
  usesUnboundedFallback,
}) {
  return (
    <section className="summary-grid summary-grid--weight">
      <SummaryCard title="Zeitraum" value={rangeLabel} />
      <SummaryCard title="Messungen" value={measurementCount} />
      <SummaryCard title="Ø Gewicht" value={averageWeightKg} />
      <SummaryCard title="Min Gewicht" value={minWeightKg} />
      <SummaryCard title="Max Gewicht" value={maxWeightKg} />

      {usesUnboundedFallback && (
        <SummaryCard
          title="Fallback"
          value="Aktuellste Messungen"
        />
      )}
    </section>
  );
}

export default WeightMeasurementsStatsSummary;