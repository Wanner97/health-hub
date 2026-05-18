package ch.claudiowanner.healthdataexporter.healthconnect.nutrition

import androidx.health.connect.client.records.NutritionRecord
import ch.claudiowanner.healthdataexporter.model.nutrition.NutritionExportRecord
import java.time.ZoneId

class NutritionRecordMapper {
    fun map(
        record: NutritionRecord,
        zoneId: ZoneId
    ): NutritionExportRecord {
        return NutritionExportRecord(
            healthConnectRecordId = record.metadata.id,
            date = record.startTime.atZone(zoneId).toLocalDate().toString(),
            startTime = record.startTime.toString(),
            endTime = record.endTime.toString(),
            mealType = MealTypeMapper.toExportValue(record.mealType),
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
}