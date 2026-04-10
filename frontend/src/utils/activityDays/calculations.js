export function calculateTotalSteps(activityDays) {
  if (!activityDays?.length) {
    return 0;
  }

  return activityDays.reduce((sum, day) => sum + (day.steps ?? 0), 0);
}

export function calculateAverageSteps(activityDays) {
  if (!activityDays?.length) {
    return 0;
  }

  return Math.round(calculateTotalSteps(activityDays) / activityDays.length);
}

export function calculateTotalDistanceMeters(activityDays) {
  if (!activityDays?.length) {
    return 0;
  }

  return activityDays.reduce((sum, day) => sum + (day.distanceMeters ?? 0), 0);
}

export function calculateAverageDistanceMeters(activityDays) {
  if (!activityDays?.length) {
    return 0;
  }

  return calculateTotalDistanceMeters(activityDays) / activityDays.length;
}