using CleanArc.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CleanArc.Infrastructure.Persistence.Data.Config
{
    public class VaccinationConfiguration : IEntityTypeConfiguration<Vaccination>
    {
        public void Configure(EntityTypeBuilder<Vaccination> builder)
        {
            builder.HasKey(v => v.Id);

            builder.HasOne(v => v.MedicalRecord)
                   .WithMany(m => m.Vaccinations)
                   .HasForeignKey(v => v.MedicalRecordId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.Property(v => v.Name).HasMaxLength(100).IsRequired();
        }
    }
}
