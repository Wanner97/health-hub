import { apiRequest } from './apiClient';
import { buildDateRangeQuery } from '../utils/api/queryParams';

export async function getBloodOxygenDays(filters = {}) {
  const query = buildDateRangeQuery(filters);
  return await apiRequest(`/api/BloodOxygenDays${query}`);
}