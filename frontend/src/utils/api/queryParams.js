export function buildDateRangeQuery(filters = {}, additionalKeys = []) {
  const params = new URLSearchParams();

  if (filters.from) {
    params.set('from', filters.from);
  }

  if (filters.to) {
    params.set('to', filters.to);
  }

  for (const key of additionalKeys) {
    const value = filters[key];

    if (value === undefined || value === null || value === '') {
      continue;
    }

    params.set(key, String(value));
  }

  const queryString = params.toString();

  return queryString ? `?${queryString}` : '';
}

export function buildMeasuredAtUtcRangeQuery(filters = {}) {
  const params = new URLSearchParams();

  if (filters.fromMeasuredAtUtc) {
    params.set('fromMeasuredAtUtc', filters.fromMeasuredAtUtc);
  }

  if (filters.toMeasuredAtUtc) {
    params.set('toMeasuredAtUtc', filters.toMeasuredAtUtc);
  }

  const queryString = params.toString();

  return queryString ? `?${queryString}` : '';
}