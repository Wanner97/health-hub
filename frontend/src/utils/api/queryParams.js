export function buildDateRangeQuery(filters = {}) {
  const params = new URLSearchParams();

  if (filters.from) {
    params.set('from', filters.from);
  }

  if (filters.to) {
    params.set('to', filters.to);
  }

  const queryString = params.toString();

  return queryString ? `?${queryString}` : '';
}