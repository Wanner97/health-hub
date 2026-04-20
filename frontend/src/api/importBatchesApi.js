import { apiRequest } from './apiClient';
import { buildDateRangeQuery } from '../utils/api/queryParams';

export async function getImportBatches(filters = {}) {
  const query = buildDateRangeQuery(filters);
  return await apiRequest(`/api/importbatch${query}`);
}