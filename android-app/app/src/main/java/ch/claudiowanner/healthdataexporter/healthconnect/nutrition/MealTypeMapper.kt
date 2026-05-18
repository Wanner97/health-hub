package ch.claudiowanner.healthdataexporter.healthconnect.nutrition

import androidx.health.connect.client.records.MealType

object MealTypeMapper {
    fun toExportValue(mealType: Int): String {
        return when (mealType) {
            MealType.MEAL_TYPE_BREAKFAST -> "breakfast"
            MealType.MEAL_TYPE_LUNCH -> "lunch"
            MealType.MEAL_TYPE_DINNER -> "dinner"
            MealType.MEAL_TYPE_SNACK -> "snack"
            MealType.MEAL_TYPE_UNKNOWN -> "unknown"
            else -> "unsupported_$mealType"
        }
    }
}