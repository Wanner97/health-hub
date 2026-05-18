using Common.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DataAccess.Configurations
{
    public class ImportBatchConfiguration : IEntityTypeConfiguration<ImportBatch>
    {
        public void Configure(EntityTypeBuilder<ImportBatch> builder)
        {
            builder.ToTable("ImportBatch");

            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).ValueGeneratedOnAdd();

            builder.Property(x => x.Source).HasColumnName("Source").HasMaxLength(100).IsUnicode().IsRequired();
            builder.Property(x => x.ExportVersion).HasColumnName("ExportVersion").IsRequired();
            builder.Property(x => x.ExportType).HasColumnName("ExportType").HasMaxLength(50).IsUnicode().IsRequired();
            builder.Property(x => x.ExportedAtUtc).HasColumnName("ExportedAtUtc").IsRequired();
            builder.Property(x => x.ImportedAtUtc).HasColumnName("ImportedAtUtc").IsRequired();
            builder.Property(x => x.RangeStartUtc).HasColumnName("RangeStartUtc").IsRequired();
            builder.Property(x => x.RangeEndUtc).HasColumnName("RangeEndUtc").IsRequired();
            builder.Property(x => x.ReceivedRecordCount).HasColumnName("ReceivedRecordCount").IsRequired();
            builder.Property(x => x.InsertedRecordCount).HasColumnName("InsertedRecordCount").IsRequired();
            builder.Property(x => x.UpdatedRecordCount).HasColumnName("UpdatedRecordCount").IsRequired();
            builder.Property(x => x.UnchangedRecordCount).HasColumnName("UnchangedRecordCount").IsRequired();

            builder.HasMany(x => x.ActivityDayEntries)
                .WithOne(x => x.LastImportBatch)
                .HasForeignKey(x => x.LastImportBatchId)
                .IsRequired();

            builder.HasMany(x => x.SleepSessionEntries)
                .WithOne(x => x.LastImportBatch)
                .HasForeignKey(x => x.LastImportBatchId)
                .IsRequired();

            builder.HasMany(x => x.HeartRateDayEntries)
                .WithOne(x => x.LastImportBatch)
                .HasForeignKey(x => x.LastImportBatchId)
                .IsRequired();

            builder.HasMany(x => x.BloodOxygenDayEntries)
                .WithOne(x => x.LastImportBatch)
                .HasForeignKey(x => x.LastImportBatchId)
                .IsRequired();

            builder.HasMany(x => x.HeightMeasurementEntries)
                .WithOne(x => x.LastImportBatch)
                .HasForeignKey(x => x.LastImportBatchId)
                .IsRequired();

            builder.HasMany(x => x.WeightMeasurementEntries)
                .WithOne(x => x.LastImportBatch)
                .HasForeignKey(x => x.LastImportBatchId)
                .IsRequired();

            builder.HasMany(x => x.NutritionRecordEntries)
                .WithOne(x => x.LastImportBatch)
                .HasForeignKey(x => x.LastImportBatchId)
                .IsRequired();

            builder.HasMany(x => x.NutritionDayEntries)
                .WithOne(x => x.LastImportBatch)
                .HasForeignKey(x => x.LastImportBatchId)
                .IsRequired();
        }
    }
}