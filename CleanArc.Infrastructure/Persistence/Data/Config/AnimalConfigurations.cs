using CleanArc.Core.Entites;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CleanArc.Infrastructure.Persistence.Data.Config
{
    public class AnimalConfigurations : IEntityTypeConfiguration<Animal>
    {
        public void Configure(EntityTypeBuilder<Animal> builder)
        {
            builder.HasKey(a => a.AnimalId);
            builder.Property(a => a.Name).HasMaxLength(100).IsRequired();
            builder.Property(a => a.Type).HasMaxLength(50).IsRequired();
            builder.Property(a => a.Breed).HasMaxLength(50).IsRequired();
            builder.Property(a => a.About).HasMaxLength(1000);
            builder.Property(a =>a.Photo).IsRequired();
            builder.Property(a => a.IsAdopted).HasDefaultValue(false);
            builder.HasMany(a => a.MedicalRecords)
                   .WithOne(m => m.Animal)
                   .HasForeignKey(m => m.Animalid)
                   .OnDelete(DeleteBehavior.Cascade);
            builder.HasMany(a => a.Requests)
                .WithOne(r => r.Animal)
                .HasForeignKey(a => a.AnimalId);
        }
    }
}
