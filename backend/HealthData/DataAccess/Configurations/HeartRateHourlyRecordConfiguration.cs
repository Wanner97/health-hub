using Common.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DataAccess.Configurations
{
    public class HeartRateHourlyRecordConfiguration : IEntityTypeConfiguration<HeartRateHourlyRecord>
    {
        public void Configure(EntityTypeBuilder<HeartRateHourlyRecord> builder)
        {
            builder.ToTable("HeartRateHourlyRecord");

            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).ValueGeneratedOnAdd();

            builder.Property(x => x.HeartRateDayId).HasColumnName("HeartRateDayId").IsRequired();
            builder.Property(x => x.Hour).HasColumnName("Hour").IsRequired();
            builder.Property(x => x.StartTimeUtc).HasColumnName("StartTimeUtc").IsRequired();
            builder.Property(x => x.EndTimeUtc).HasColumnName("EndTimeUtc").IsRequired();
            builder.Property(x => x.AvgBpm).HasColumnName("AvgBpm").IsRequired();
            builder.Property(x => x.MinBpm).HasColumnName("MinBpm").IsRequired();
            builder.Property(x => x.MaxBpm).HasColumnName("MaxBpm").IsRequired();
            builder.Property(x => x.MeasurementCount).HasColumnName("MeasurementCount").IsRequired();

            builder.HasIndex(x => new { x.HeartRateDayId, x.Hour })
                .IsUnique();
        }
    }
}