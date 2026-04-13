import { apiRequest } from './apiClient';

export async function getImportBatches(filters = {}) {
  const params = new URLSearchParams();

  if (filters.from) {
    params.set('from', filters.from);
  }

  if (filters.to) {
    params.set('to', filters.to);
  }

  const queryString = params.toString();
  const path = queryString
    ? `/api/importbatch?${queryString}`
    : '/api/importbatch';

  return await apiRequest(path);
}