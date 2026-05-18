import { apiRequest } from './apiClient';
import { buildDateRangeQuery } from '../utils/api/queryParams';

export async function getNutritionDays(filters = {}) {
  const query = buildDateRangeQuery(filters);
  return await apiRequest(`/api/NutritionDays${query}`);
}