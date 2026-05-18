using Common.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DataAccess.Configurations
{
    public class NutritionDayNutrientTotalConfiguration : IEntityTypeConfiguration<NutritionDayNutrientTotal>
    {
        public void Configure(EntityTypeBuilder<NutritionDayNutrientTotal> builder)
        {
            builder.ToTable("NutritionDayNutrientTotal");

            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).ValueGeneratedOnAdd();

            builder.Property(x => x.NutritionDayId).HasColumnName("NutritionDayId").IsRequired();
            builder.Property(x => x.NutrientKey).HasColumnName("NutrientKey").HasMaxLength(100).IsUnicode(false).IsRequired();
            builder.Property(x => x.TotalAmount).HasColumnName("TotalAmount").IsRequired();
            builder.Property(x => x.Unit).HasColumnName("Unit").HasMaxLength(20).IsUnicode(false).IsRequired();

            builder.HasIndex(x => new { x.NutritionDayId, x.NutrientKey }).IsUnique();
        }
    }
}