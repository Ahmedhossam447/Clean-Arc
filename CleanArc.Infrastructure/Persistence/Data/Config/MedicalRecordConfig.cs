using CleanArc.Core.Entites;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CleanArc.Infrastructure.Persistence.Data.Config
{
    public class MedicalRecordConfig : IEntityTypeConfiguration<MedicalRecord>
    {
        public void Configure(EntityTypeBuilder<MedicalRecord> builder)
        {
            builder.HasKey(m => m.Recordid);
            builder.Property(m => m.injurys).HasMaxLength(500);
            builder.Property(m => m.Status).HasMaxLength(100);
            builder.HasOne(m => m.Animal)
                   .WithMany(a => a.MedicalRecords)
                   .HasForeignKey(m => m.Animalid);
            builder.HasMany(m => m.VaccinationNeededs)
                   .WithOne(v => v.MedicalRecord)
                   .HasForeignKey(v => v.Medicalid);
        }
    }
}
