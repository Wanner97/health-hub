using Common.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DataAccess.Configurations
{
    public class HeartRateDayConfiguration : IEntityTypeConfiguration<HeartRateDay>
    {
        public void Configure(EntityTypeBuilder<HeartRateDay> builder)
        {
            builder.ToTable("HeartRateDay");

            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).ValueGeneratedOnAdd();

            builder.Property(x => x.Source).HasColumnName("Source").HasMaxLength(100).IsUnicode().IsRequired();
            builder.Property(x => x.Date).HasColumnName("Date").IsRequired();
            builder.Property(x => x.StartTimeUtc).HasColumnName("StartTimeUtc").IsRequired();
            builder.Property(x => x.EndTimeUtc).HasColumnName("EndTimeUtc").IsRequired();
            builder.Property(x => x.AvgBpm).HasColumnName("AvgBpm").IsRequired();
            builder.Property(x => x.MinBpm).HasColumnName("MinBpm").IsRequired();
            builder.Property(x => x.MaxBpm).HasColumnName("MaxBpm").IsRequired();
            builder.Property(x => x.MeasurementCount).HasColumnName("MeasurementCount").IsRequired();
            builder.Property(x => x.LastImportedAtUtc).HasColumnName("LastImportedAtUtc").IsRequired();
            builder.Property(x => x.LastImportBatchId).HasColumnName("LastImportBatchId").IsRequired();

            builder.HasIndex(x => new { x.Source, x.Date }).IsUnique();

            builder.HasMany(x => x.HourlyRecords)
                .WithOne(x => x.HeartRateDay)
                .HasForeignKey(x => x.HeartRateDayId)
                .IsRequired();
        }
    }
}