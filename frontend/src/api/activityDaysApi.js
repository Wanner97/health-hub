import { apiRequest } from './apiClient';
import { buildDateRangeQuery } from '../utils/api/queryParams';

export async function getActivityDays(filters = {}) {
  const query = buildDateRangeQuery(filters);
  return await apiRequest(`/api/activitydays${query}`);
}