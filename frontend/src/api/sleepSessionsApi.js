import { apiRequest } from './apiClient';

export async function getSleepSessions(filters = {}) {
  const params = new URLSearchParams();

  if (filters.from) {
    params.set('from', filters.from);
  }

  if (filters.to) {
    params.set('to', filters.to);
  }

  const queryString = params.toString();
  const path = queryString
    ? `/api/sleepsessions?${queryString}`
    : '/api/sleepsessions';

  return await apiRequest(path);
}