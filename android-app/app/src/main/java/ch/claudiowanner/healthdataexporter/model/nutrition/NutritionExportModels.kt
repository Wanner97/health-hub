package ch.claudiowanner.healthdataexporter.model.nutrition

data class NutritionExportCluster(
    val records: List<NutritionExportRecord>
)

data class NutritionExportRecord(
    val startTime: String,
    val endTime: String,
    val mealType: String,
    val name: String?,

    val energyKcal: Double?,
    val energyFromFatKcal: Double?,

    val biotinGrams: Double?,
    val caffeineGrams: Double?,
    val calciumGrams: Double?,
    val chlorideGrams: Double?,
    val cholesterolGrams: Double?,
    val chromiumGrams: Double?,
    val copperGrams: Double?,
    val dietaryFiberGrams: Double?,
    val folateGrams: Double?,
    val folicAcidGrams: Double?,
    val iodineGrams: Double?,
    val ironGrams: Double?,
    val magnesiumGrams: Double?,
    val manganeseGrams: Double?,
    val molybdenumGrams: Double?,
    val monounsaturatedFatGrams: Double?,
    val niacinGrams: Double?,
    val pantothenicAcidGrams: Double?,
    val phosphorusGrams: Double?,
    val polyunsaturatedFatGrams: Double?,
    val potassiumGrams: Double?,
    val proteinGrams: Double?,
    val riboflavinGrams: Double?,
    val saturatedFatGrams: Double?,
    val seleniumGrams: Double?,
    val sodiumGrams: Double?,
    val sugarGrams: Double?,
    val thiaminGrams: Double?,
    val totalCarbohydrateGrams: Double?,
    val totalFatGrams: Double?,
    val transFatGrams: Double?,
    val unsaturatedFatGrams: Double?,
    val vitaminAGrams: Double?,
    val vitaminB12Grams: Double?,
    val vitaminB6Grams: Double?,
    val vitaminCGrams: Double?,
    val vitaminDGrams: Double?,
    val vitaminEGrams: Double?,
    val vitaminKGrams: Double?,
    val zincGrams: Double?
)