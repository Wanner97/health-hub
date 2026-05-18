using Common.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DataAccess.Configurations
{
    public class NutritionRecordNutrientConfiguration : IEntityTypeConfiguration<NutritionRecordNutrient>
    {
        public void Configure(EntityTypeBuilder<NutritionRecordNutrient> builder)
        {
            builder.ToTable("NutritionRecordNutrient");

            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).ValueGeneratedOnAdd();

            builder.Property(x => x.NutritionRecordId).HasColumnName("NutritionRecordId").IsRequired();
            builder.Property(x => x.NutrientKey).HasColumnName("NutrientKey").HasMaxLength(100).IsUnicode(false).IsRequired();
            builder.Property(x => x.Amount).HasColumnName("Amount").IsRequired();
            builder.Property(x => x.Unit).HasColumnName("Unit").HasMaxLength(20).IsUnicode(false).IsRequired();

            builder.HasIndex(x => new { x.NutritionRecordId, x.NutrientKey }).IsUnique();
        }
    }
}