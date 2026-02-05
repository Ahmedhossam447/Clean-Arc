using CleanArc.Core.Entites;
using CleanArc.Infrastructure.Identity;
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
            builder.Property(a => a.RowVersion).IsRowVersion();

            // Foreign key to ApplicationUser (owner) - no navigation property in Core
            builder.HasOne<ApplicationUser>()
                   .WithMany()
                   .HasForeignKey(a => a.Userid)
                   .HasPrincipalKey(u => u.Id)
                   .OnDelete(DeleteBehavior.Restrict);
            
            builder.HasOne(a => a.MedicalRecord)
                   .WithOne(m => m.Animal)
                   .HasForeignKey<MedicalRecord>(m => m.AnimalId)
                   .OnDelete(DeleteBehavior.Cascade);
            builder.HasMany(a => a.Requests)
                .WithOne(r => r.Animal)
                .HasForeignKey(a => a.AnimalId);
        }
    }
}
