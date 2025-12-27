using CleanArc.Core.Entites;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CleanArc.Infrastructure.Persistence.Data.Config
{
    public class VaccinationNeededConfig : IEntityTypeConfiguration<VaccinationNeeded>
    {
        public void Configure(EntityTypeBuilder<VaccinationNeeded> builder)
        {
            builder.HasKey(v => v.Id);
            builder.Property(v => v.VaccineName).HasMaxLength(100);
            builder.HasOne(v => v.MedicalRecord)
                   .WithMany(m => m.VaccinationNeededs)
                   .HasForeignKey(v => v.Medicalid)
                   .OnDelete(DeleteBehavior.Cascade);

        }
    }
}
