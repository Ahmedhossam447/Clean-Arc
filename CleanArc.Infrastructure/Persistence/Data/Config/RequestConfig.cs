using CleanArc.Core.Entities;
using CleanArc.Infrastructure.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CleanArc.Infrastructure.Persistence.Data.Config
{
    public class RequestConfig : IEntityTypeConfiguration<Request>
    {
        public void Configure(EntityTypeBuilder<Request> builder)
        {
            builder.HasKey(r => r.Id);

            // Map to new column names (migration will rename)
            builder.Property(r => r.Id).HasColumnName("Id");
            builder.Property(r => r.OwnerId).HasColumnName("OwnerId");
            builder.Property(r => r.RequesterId).HasColumnName("RequesterId");

            builder.Property(r => r.OwnerId)
                   .IsRequired()
                   .HasMaxLength(450);

            builder.Property(r => r.RequesterId)
                   .IsRequired()
                   .HasMaxLength(450);

            // Foreign key to ApplicationUser (owner) - no navigation property in Core
            builder.HasOne<ApplicationUser>()
                   .WithMany()
                   .HasForeignKey(r => r.OwnerId)
                   .HasPrincipalKey(u => u.Id)
                   .OnDelete(DeleteBehavior.Restrict);

            // Foreign key to ApplicationUser (requester) - no navigation property in Core
            builder.HasOne<ApplicationUser>()
                   .WithMany()
                   .HasForeignKey(r => r.RequesterId)
                   .HasPrincipalKey(u => u.Id)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(r => r.Animal)
                   .WithMany(a => a.Requests)
                   .HasForeignKey(r => r.AnimalId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.Property(r => r.Status)
                   .HasMaxLength(50)
                   .HasDefaultValue("Pending");
        }
    }
}
