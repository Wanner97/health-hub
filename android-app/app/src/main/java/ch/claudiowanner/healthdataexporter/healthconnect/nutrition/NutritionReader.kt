package ch.claudiowanner.healthdataexporter.healthconnect.nutrition

import androidx.health.connect.client.HealthConnectClient
import androidx.health.connect.client.records.MealType
import androidx.health.connect.client.records.NutritionRecord
import androidx.health.connect.client.request.ReadRecordsRequest
import androidx.health.connect.client.time.TimeRangeFilter
import ch.claudiowanner.healthdataexporter.model.nutrition.NutritionExportRecord
import java.time.Duration
import java.time.Instant
import java.time.ZoneId

class NutritionReader(
    private val client: HealthConnectClient
) {
    suspend fun readNutritionRecords(
        startInstant: Instant,
        endInstant: Instant
    ): List<NutritionExportRecord> {
        require(startInstant < endInstant) {
            "startInstant must be before endInstant."
        }

        val allResults = mutableListOf<NutritionExportRecord>()
        var chunkStart = startInstant
        val zoneId = ZoneId.systemDefault()

        while (chunkStart < endInstant) {
            val chunkEnd = minOf(chunkStart.plus(Duration.ofDays(365)), endInstant)

            allResults.addAll(
                readNutritionRecordsForRange(
                    startInstant = chunkStart,
                    endInstant = chunkEnd,
                    zoneId = zoneId
                )
            )

            chunkStart = chunkEnd
        }

        return allResults.sortedBy { it.startTime }
    }

    private suspend fun readNutritionRecordsForRange(
        startInstant: Instant,
        endInstant: Instant,
        zoneId: ZoneId
    ): List<NutritionExportRecord> {
        val results = mutableListOf<NutritionExportRecord>()
        var pageToken: String? = null

        do {
            val response = client.readRecords(
                ReadRecordsRequest(
                    recordType = NutritionRecord::class,
                    timeRangeFilter = TimeRangeFilter.between(startInstant, endInstant),
                    pageSize = 1000,
                    pageToken = pageToken
                )
            )

            val mapped = response.records.map { record ->
                NutritionExportRecord(
                    healthConnectRecordId = record.metadata.id,
                    date = record.startTime.atZone(zoneId).toLocalDate().toString(),
                    startTime = record.startTime.toString(),
                    endTime = record.endTime.toString(),
                    mealType = mapMealType(record.mealType),
                    name = record.name,

                    energyKcal = record.energy?.inKilocalories,
                    energyFromFatKcal = record.energyFromFat?.inKilocalories,

                    biotinGrams = record.biotin?.inGrams,
                    caffeineGrams = record.caffeine?.inGrams,
                    calciumGrams = record.calcium?.inGrams,
                    chlorideGrams = record.chloride?.inGrams,
                    cholesterolGrams = record.cholesterol?.inGrams,
                    chromiumGrams = record.chromium?.inGrams,
                    copperGrams = record.copper?.inGrams,
                    dietaryFiberGrams = record.dietaryFiber?.inGrams,
                    folateGrams = record.folate?.inGrams,
                    folicAcidGrams = record.folicAcid?.inGrams,
                    iodineGrams = record.iodine?.inGrams,
                    ironGrams = record.iron?.inGrams,
                    magnesiumGrams = record.magnesium?.inGrams,
                    manganeseGrams = record.manganese?.inGrams,
                    molybdenumGrams = record.molybdenum?.inGrams,
                    monounsaturatedFatGrams = record.monounsaturatedFat?.inGrams,
                    niacinGrams = record.niacin?.inGrams,
                    pantothenicAcidGrams = record.pantothenicAcid?.inGrams,
                    phosphorusGrams = record.phosphorus?.inGrams,
                    polyunsaturatedFatGrams = record.polyunsaturatedFat?.inGrams,
                    potassiumGrams = record.potassium?.inGrams,
                    proteinGrams = record.protein?.inGrams,
                    riboflavinGrams = record.riboflavin?.inGrams,
                    saturatedFatGrams = record.saturatedFat?.inGrams,
                    seleniumGrams = record.selenium?.inGrams,
                    sodiumGrams = record.sodium?.inGrams,
                    sugarGrams = record.sugar?.inGrams,
                    thiaminGrams = record.thiamin?.inGrams,
                    totalCarbohydrateGrams = record.totalCarbohydrate?.inGrams,
                    totalFatGrams = record.totalFat?.inGrams,
                    transFatGrams = record.transFat?.inGrams,
                    unsaturatedFatGrams = record.unsaturatedFat?.inGrams,
                    vitaminAGrams = record.vitaminA?.inGrams,
                    vitaminB12Grams = record.vitaminB12?.inGrams,
                    vitaminB6Grams = record.vitaminB6?.inGrams,
                    vitaminCGrams = record.vitaminC?.inGrams,
                    vitaminDGrams = record.vitaminD?.inGrams,
                    vitaminEGrams = record.vitaminE?.inGrams,
                    vitaminKGrams = record.vitaminK?.inGrams,
                    zincGrams = record.zinc?.inGrams
                )
            }

            results.addAll(mapped)
            pageToken = response.pageToken
        } while (pageToken != null)

        return results
    }

    private fun mapMealType(mealType: Int): String {
        return when (mealType) {
            MealType.MEAL_TYPE_BREAKFAST -> "breakfast"
            MealType.MEAL_TYPE_LUNCH -> "lunch"
            MealType.MEAL_TYPE_DINNER -> "dinner"
            MealType.MEAL_TYPE_SNACK -> "snack"
            MealType.MEAL_TYPE_UNKNOWN -> "unknown"
            else -> "unknown"
        }
    }
}