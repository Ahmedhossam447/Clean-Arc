using CleanArc.Core.Entites;
using CleanArc.Infrastructure.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CleanArc.Infrastructure.Persistence.Data.Config
{
    public class RequestConfig : IEntityTypeConfiguration<Request>
    {
        public void Configure(EntityTypeBuilder<Request> builder)
        {
            builder.HasKey(r => r.Reqid);

            builder.Property(r => r.Userid)
                   .IsRequired()
                   .HasMaxLength(450);

            builder.Property(r => r.Useridreq)
                   .IsRequired()
                   .HasMaxLength(450);

            // Foreign key to ApplicationUser (owner) - no navigation property in Core
            builder.HasOne<ApplicationUser>()
                   .WithMany()
                   .HasForeignKey(r => r.Userid)
                   .HasPrincipalKey(u => u.Id)
                   .OnDelete(DeleteBehavior.Restrict);

            // Foreign key to ApplicationUser (requester) - no navigation property in Core
            builder.HasOne<ApplicationUser>()
                   .WithMany()
                   .HasForeignKey(r => r.Useridreq)
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
