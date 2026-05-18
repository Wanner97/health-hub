using Common.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DataAccess.Configurations
{
    public class NutritionRecordConfiguration : IEntityTypeConfiguration<NutritionRecord>
    {
        public void Configure(EntityTypeBuilder<NutritionRecord> builder)
        {
            builder.ToTable("NutritionRecord");

            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).ValueGeneratedOnAdd();

            builder.Property(x => x.Source).HasColumnName("Source").HasMaxLength(100).IsUnicode().IsRequired();
            builder.Property(x => x.HealthConnectRecordId).HasColumnName("HealthConnectRecordId").HasMaxLength(100).IsUnicode(false).IsRequired();
            builder.Property(x => x.Date).HasColumnName("Date").IsRequired();
            builder.Property(x => x.StartTimeUtc).HasColumnName("StartTimeUtc").IsRequired();
            builder.Property(x => x.EndTimeUtc).HasColumnName("EndTimeUtc").IsRequired();
            builder.Property(x => x.MealType).HasColumnName("MealType").HasMaxLength(50).IsUnicode().IsRequired();
            builder.Property(x => x.Name).HasColumnName("Name").HasMaxLength(500).IsUnicode();
            builder.Property(x => x.LastImportedAtUtc).HasColumnName("LastImportedAtUtc").IsRequired();
            builder.Property(x => x.LastImportBatchId).HasColumnName("LastImportBatchId").IsRequired();
            builder.Property(x => x.NutritionDayId).HasColumnName("NutritionDayId").IsRequired(false);

            builder.HasIndex(x => new { x.Source, x.HealthConnectRecordId }).IsUnique();

            builder.HasIndex(x => new { x.Source, x.Date });

            builder.HasMany(x => x.Nutrients)
                .WithOne(x => x.NutritionRecord)
                .HasForeignKey(x => x.NutritionRecordId)
                .IsRequired();
        }
    }
}