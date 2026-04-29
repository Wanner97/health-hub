using Common.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DataAccess.Configurations
{
    public class HeightMeasurementConfiguration : IEntityTypeConfiguration<HeightMeasurement>
    {
        public void Configure(EntityTypeBuilder<HeightMeasurement> builder)
        {
            builder.ToTable("HeightMeasurement");

            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).ValueGeneratedOnAdd();

            builder.Property(x => x.Source).HasColumnName("Source").HasMaxLength(100).IsUnicode().IsRequired();
            builder.Property(x => x.HeightCm).HasColumnName("HeightCm").IsRequired();
            builder.Property(x => x.MeasuredAtUtc).HasColumnName("MeasuredAtUtc").IsRequired();
            builder.Property(x => x.LastImportedAtUtc).HasColumnName("LastImportedAtUtc").IsRequired();
            builder.Property(x => x.LastImportBatchId).HasColumnName("LastImportBatchId").IsRequired();

            builder.HasIndex(x => new { x.Source, x.MeasuredAtUtc }).IsUnique();
        }
    }
}