using CleanArc.Core.Entities;
using CleanArc.Infrastructure.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CleanArc.Infrastructure.Persistence.Data.Config
{
    public class OrderItemConfig : IEntityTypeConfiguration<OrderItem>
    {
        public void Configure(EntityTypeBuilder<OrderItem> builder)
        {
            builder.HasKey(oi => oi.Id);

            builder.Property(oi => oi.ProductName)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(oi => oi.PictureUrl)
                .HasMaxLength(500);

            builder.Property(oi => oi.Price)
                .HasColumnType("decimal(18,2)");

            builder.Property(oi => oi.Quantity)
                .IsRequired();

            builder.Property(oi => oi.ShelterId)
                .IsRequired();

            builder.Property(oi => oi.Status)
                .IsRequired()
                .HasMaxLength(50)
                .HasDefaultValue("Pending");

            // FK to Product
            builder.HasOne(oi => oi.Product)
                .WithMany()
                .HasForeignKey(oi => oi.ProductId)
                .OnDelete(DeleteBehavior.Restrict);

            // FK to Shelter (ApplicationUser)
            builder.HasOne<ApplicationUser>()
                .WithMany()
                .HasForeignKey(oi => oi.ShelterId)
                .HasPrincipalKey(u => u.Id)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
