using Common.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DataAccess.Configurations
{
    public class ActivityDayConfiguration : IEntityTypeConfiguration<ActivityDay>
    {
        public void Configure(EntityTypeBuilder<ActivityDay> builder)
        {
            builder.ToTable("ActivityDay");

            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).ValueGeneratedOnAdd();

            builder.Property(x => x.Source).HasColumnName("Source").HasMaxLength(100).IsUnicode().IsRequired();
            builder.Property(x => x.Date).HasColumnName("Date").IsRequired();
            builder.Property(x => x.StartTimeUtc).HasColumnName("StartTimeUtc").IsRequired();
            builder.Property(x => x.EndTimeUtc).HasColumnName("EndTimeUtc").IsRequired();
            builder.Property(x => x.Steps).HasColumnName("Steps").IsRequired();
            builder.Property(x => x.DistanceMeters).HasColumnName("DistanceMeters").IsRequired();
            builder.Property(x => x.DistanceSource).HasColumnName("DistanceSource").HasMaxLength(50).IsUnicode().IsRequired();
            builder.Property(x => x.TotalCaloriesBurnedKcal).HasColumnName("TotalCaloriesBurnedKcal").IsRequired();
            builder.Property(x => x.LastImportedAtUtc).HasColumnName("LastImportedAtUtc").IsRequired();
            builder.Property(x => x.LastImportBatchId).HasColumnName("LastImportBatchId").IsRequired();

            builder.HasIndex(x => new { x.Source, x.Date }).IsUnique();
        }
    }
}