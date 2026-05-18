using Common.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DataAccess.Configurations
{
    public class NutritionMealTypeNutrientTotalConfiguration : IEntityTypeConfiguration<NutritionMealTypeNutrientTotal>
    {
        public void Configure(EntityTypeBuilder<NutritionMealTypeNutrientTotal> builder)
        {
            builder.ToTable("NutritionMealTypeNutrientTotal");

            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).ValueGeneratedOnAdd();

            builder.Property(x => x.NutritionMealTypeSummaryId).HasColumnName("NutritionMealTypeSummaryId").IsRequired();
            builder.Property(x => x.NutrientKey).HasColumnName("NutrientKey").HasMaxLength(100).IsUnicode(false).IsRequired();
            builder.Property(x => x.TotalAmount).HasColumnName("TotalAmount").IsRequired();
            builder.Property(x => x.Unit).HasColumnName("Unit").HasMaxLength(20).IsUnicode(false).IsRequired();

            builder.HasIndex(x => new { x.NutritionMealTypeSummaryId, x.NutrientKey }).IsUnique();
        }
    }
}