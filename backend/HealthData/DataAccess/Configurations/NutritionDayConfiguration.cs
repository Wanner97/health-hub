 using Common.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DataAccess.Configurations
{
    public class NutritionDayConfiguration : IEntityTypeConfiguration<NutritionDay>
    {
        public void Configure(EntityTypeBuilder<NutritionDay> builder)
        {
            builder.ToTable("NutritionDay");

            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).ValueGeneratedOnAdd();

            builder.Property(x => x.Source).HasColumnName("Source").HasMaxLength(100).IsUnicode().IsRequired();
            builder.Property(x => x.Date).HasColumnName("Date").IsRequired();
            builder.Property(x => x.RecordCount).HasColumnName("RecordCount").IsRequired();
            builder.Property(x => x.LastCalculatedAtUtc).HasColumnName("LastCalculatedAtUtc").IsRequired();
            builder.Property(x => x.LastImportBatchId).HasColumnName("LastImportBatchId").IsRequired();

            builder.HasIndex(x => new { x.Source, x.Date }).IsUnique();

            builder.HasMany(x => x.Records)
                .WithOne(x => x.NutritionDay)
                .HasForeignKey(x => x.NutritionDayId);

            builder.HasMany(x => x.NutrientTotals)
                .WithOne(x => x.NutritionDay)
                .HasForeignKey(x => x.NutritionDayId)
                .IsRequired();

            builder.HasMany(x => x.MealTypeSummaries)
                .WithOne(x => x.NutritionDay)
                .HasForeignKey(x => x.NutritionDayId)
                .IsRequired();
        }
    }
}