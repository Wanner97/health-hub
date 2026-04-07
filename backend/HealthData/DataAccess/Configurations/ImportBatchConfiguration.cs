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

            builder.Property(x => x.Source)
                .HasColumnName("Source")
                .HasMaxLength(100)
                .IsUnicode()
                .IsRequired();

            builder.Property(x => x.ExportVersion)
                .HasColumnName("ExportVersion")
                .IsRequired();

            builder.Property(x => x.ExportedAt)
                .HasColumnName("ExportedAt")
                .IsRequired();

            builder.Property(x => x.ImportedAt)
                .HasColumnName("ImportedAt")
                .IsRequired();

            builder.Property(x => x.RecordCount)
                .HasColumnName("RecordCount")
                .IsRequired();

            builder.HasMany(x => x.StepEntries)
                .WithOne(x => x.ImportBatch)
                .HasForeignKey(x => x.ImportBatchId)
                .IsRequired();
        }
    }
}