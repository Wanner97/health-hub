using Common.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DataAccess.Configurations
{
    public class SleepSessionConfiguration : IEntityTypeConfiguration<SleepSession>
    {
        public void Configure(EntityTypeBuilder<SleepSession> builder)
        {
            builder.ToTable("SleepSession");

            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).ValueGeneratedOnAdd();

            builder.Property(x => x.Source).HasColumnName("Source").HasMaxLength(100).IsUnicode().IsRequired();
            builder.Property(x => x.StartTimeUtc).HasColumnName("StartTimeUtc").IsRequired();
            builder.Property(x => x.EndTimeUtc).HasColumnName("EndTimeUtc").IsRequired();
            builder.Property(x => x.DurationMinutes).HasColumnName("DurationMinutes").IsRequired();
            builder.Property(x => x.LastImportedAtUtc).HasColumnName("LastImportedAtUtc").IsRequired();
            builder.Property(x => x.LastImportBatchId).HasColumnName("LastImportBatchId").IsRequired();

            builder.HasIndex(x => new { x.Source, x.StartTimeUtc })
                .IsUnique();

            builder.HasMany(x => x.SleepStages)
                .WithOne(x => x.SleepSession)
                .HasForeignKey(x => x.SleepSessionId)
                .IsRequired();
        }
    }
}