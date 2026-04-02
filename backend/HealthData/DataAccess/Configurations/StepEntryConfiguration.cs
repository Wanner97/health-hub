using Common.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DataAccess.Configurations
{
    public class StepEntryConfiguration : IEntityTypeConfiguration<StepEntry>
    {
        public void Configure(EntityTypeBuilder<StepEntry> builder)
        {
            builder.ToTable("StepEntry");

            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).ValueGeneratedOnAdd();

            builder.Property(x => x.ImportBatchId)
                .HasColumnName("ImportBatchId")
                .IsRequired();

            builder.Property(x => x.Date)
                .HasColumnName("Date")
                .IsRequired();

            builder.Property(x => x.Count)
                .HasColumnName("Count")
                .IsRequired();

            builder.Property(x => x.StartTime)
                .HasColumnName("StartTime")
                .IsRequired();

            builder.Property(x => x.EndTime)
                .HasColumnName("EndTime")
                .IsRequired();
        }
    }
}