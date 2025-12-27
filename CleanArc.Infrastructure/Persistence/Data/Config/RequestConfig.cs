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
            builder.HasOne(r => r.User)
                   .WithMany()
                   .HasForeignKey(r => r.Userid)
                   .OnDelete(DeleteBehavior.Restrict);
            builder.HasOne(r => r.User2).WithMany()
                .HasForeignKey(r => r.Useridreq)
                .OnDelete(DeleteBehavior.Restrict);
            builder.HasOne(r => r.Animal)
                   .WithMany(r=>r.Requests)
                   .HasForeignKey(r => r.AnimalId)
                   .OnDelete(DeleteBehavior.Cascade);
            builder.Property(r => r.Status)
                   .HasMaxLength(50).HasDefaultValue("Pending");
        }
    }
}
