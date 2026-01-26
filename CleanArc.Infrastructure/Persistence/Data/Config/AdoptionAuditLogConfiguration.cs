using CleanArc.Core.Entites;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CleanArc.Infrastructure.Persistence.Data.Config
{
    public class AdoptionAuditLogConfiguration : IEntityTypeConfiguration<AdoptionAuditLog>
    {
        public void Configure(EntityTypeBuilder<AdoptionAuditLog> builder)
        {
            builder.HasKey(a => a.Id);
            
            builder.Property(a => a.AnimalName)
                   .HasMaxLength(100)
                   .IsRequired();
            
            builder.Property(a => a.AnimalType)
                   .HasMaxLength(50)
                   .IsRequired();
            
            builder.Property(a => a.AdopterId)
                   .HasMaxLength(450)
                   .IsRequired();
            
            builder.Property(a => a.AdopterName)
                   .HasMaxLength(200)
                   .IsRequired();
            
            builder.Property(a => a.AdopterEmail)
                   .HasMaxLength(256)
                   .IsRequired();
            
            builder.Property(a => a.PreviousOwnerId)
                   .HasMaxLength(450)
                   .IsRequired();
            
            builder.Property(a => a.PreviousOwnerEmail)
                   .HasMaxLength(256)
                   .IsRequired();
            
            builder.Property(a => a.ProcessedBy)
                   .HasMaxLength(100);

            // Index for common queries
            builder.HasIndex(a => a.AdoptedAt);
            builder.HasIndex(a => a.AnimalId);
            builder.HasIndex(a => a.AdopterId);
        }
    }
}
