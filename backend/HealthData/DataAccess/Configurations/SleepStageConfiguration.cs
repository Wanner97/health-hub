using Common.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DataAccess.Configurations
{
    public class SleepStageConfiguration : IEntityTypeConfiguration<SleepStage>
    {
        public void Configure(EntityTypeBuilder<SleepStage> builder)
        {
            builder.ToTable("SleepStage");

            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).ValueGeneratedOnAdd();

            builder.Property(x => x.SleepSessionId).HasColumnName("SleepSessionId").IsRequired();
            builder.Property(x => x.Stage).HasColumnName("Stage").HasMaxLength(20).IsUnicode().IsRequired();
            builder.Property(x => x.StartTimeUtc).HasColumnName("StartTimeUtc").IsRequired();
            builder.Property(x => x.EndTimeUtc).HasColumnName("EndTimeUtc").IsRequired();

            builder.HasIndex(x => x.SleepSessionId);
        }
    }
}