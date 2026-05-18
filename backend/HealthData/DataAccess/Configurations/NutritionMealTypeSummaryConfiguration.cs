using Common.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DataAccess.Configurations
{
    public class NutritionMealTypeSummaryConfiguration : IEntityTypeConfiguration<NutritionMealTypeSummary>
    {
        public void Configure(EntityTypeBuilder<NutritionMealTypeSummary> builder)
        {
            builder.ToTable("NutritionMealTypeSummary");

            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).ValueGeneratedOnAdd();

            builder.Property(x => x.NutritionDayId).HasColumnName("NutritionDayId").IsRequired();
            builder.Property(x => x.MealType).HasColumnName("MealType").HasMaxLength(50).IsUnicode().IsRequired();
            builder.Property(x => x.RecordCount).HasColumnName("RecordCount").IsRequired();

            builder.HasIndex(x => new { x.NutritionDayId, x.MealType }).IsUnique();

            builder.HasMany(x => x.NutrientTotals)
                .WithOne(x => x.NutritionMealTypeSummary)
                .HasForeignKey(x => x.NutritionMealTypeSummaryId)
                .IsRequired();
        }
    }
}