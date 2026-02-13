using CleanArc.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CleanArc.Infrastructure.Persistence.Data.Config
{
    public class MedicalRecordConfig : IEntityTypeConfiguration<MedicalRecord>
    {
        public void Configure(EntityTypeBuilder<MedicalRecord> builder)
        {
            builder.HasKey(m => m.Id);

            builder.HasOne(m => m.Animal)
                   .WithOne(a => a.MedicalRecord)
                   .HasForeignKey<MedicalRecord>(m => m.AnimalId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(m => m.Vaccinations)
                   .WithOne(v => v.MedicalRecord)
                   .HasForeignKey(v => v.MedicalRecordId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.Property(m => m.BloodType).HasMaxLength(50);
            builder.Property(m => m.MedicalHistoryNotes).HasMaxLength(2000);
            builder.Property(m => m.Injuries).HasMaxLength(500);
            builder.Property(m => m.Status).HasMaxLength(100);
        }
    }
}
