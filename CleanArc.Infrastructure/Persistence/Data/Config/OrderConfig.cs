using CleanArc.Core.Entities;
using CleanArc.Infrastructure.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CleanArc.Infrastructure.Persistence.Data.Config
{
    public class OrderConfig : IEntityTypeConfiguration<Order>
    {
        public void Configure(EntityTypeBuilder<Order> builder)
        {
            builder.HasKey(o => o.Id);

            builder.Property(o => o.BuyerId)
                .IsRequired();

            builder.Property(o => o.Status)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(o => o.Subtotal)
                .HasColumnType("decimal(18,2)");

            builder.Property(o => o.OrderDate)
                .IsRequired();

            // FK to Buyer (ApplicationUser)
            builder.HasOne<ApplicationUser>()
                .WithMany()
                .HasForeignKey(o => o.BuyerId)
                .HasPrincipalKey(u => u.Id)
                .OnDelete(DeleteBehavior.Restrict);

            // One-to-One: Order -> PaymentTransaction
            builder.HasOne(o => o.PaymentTransaction)
                .WithOne()
                .HasForeignKey<Order>(o => o.PaymentTransactionId)
                .OnDelete(DeleteBehavior.SetNull);

            // Order -> OrderItems (cascade delete)
            builder.HasMany(o => o.OrderItems)
                .WithOne(oi => oi.Order)
                .HasForeignKey(oi => oi.OrderId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
