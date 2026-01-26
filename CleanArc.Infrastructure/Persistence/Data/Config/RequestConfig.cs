using CleanArc.Core.Entites;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CleanArc.Infrastructure.Persistence.Data.Config
{
    public class RequestConfig : IEntityTypeConfiguration<Request>
    {
        public void Configure(EntityTypeBuilder<Request> builder)
        {
            builder.HasKey(r => r.Reqid);

            // Userid and Useridreq are just string FKs - no navigation to ApplicationUser
            // This keeps the Core layer clean from Identity dependencies
            builder.Property(r => r.Userid)
                   .IsRequired()
                   .HasMaxLength(450);

            builder.Property(r => r.Useridreq)
                   .IsRequired()
                   .HasMaxLength(450);

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
