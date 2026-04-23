export function buildHourlyHeartRateChartData(hourlyRecords) {
  const recordMap = new Map();

  for (const record of hourlyRecords ?? []) {
    const hour = record?.hour;

    if (!Number.isInteger(hour) || hour < 0 || hour > 23) {
      continue;
    }

    recordMap.set(hour, record);
  }

  return Array.from({ length: 24 }, (_, hour) => {
    const record = recordMap.get(hour) ?? null;

    return {
      key: `hour-${hour}`,
      hour,
      label: String(hour),
      fullLabel: `${String(hour).padStart(2, '0')}:00–${String(hour).padStart(2, '0')}:59`,
      hasData: record != null,
      minBpm: record?.minBpm ?? null,
      maxBpm: record?.maxBpm ?? null,
      avgBpm: record?.avgBpm ?? null,
      measurementCount: record?.measurementCount ?? 0,
    };
  });
}