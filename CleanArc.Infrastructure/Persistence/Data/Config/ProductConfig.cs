using CleanArc.Core.Entities;
using CleanArc.Infrastructure.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CleanArc.Infrastructure.Persistence.Data.Config
{
    public class ProductConfig : IEntityTypeConfiguration<Product>
    {
        public void Configure(EntityTypeBuilder<Product> builder)
        {
            builder.HasKey(p => p.Id);

            builder.Property(p => p.Name)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(p => p.Description)
                .HasMaxLength(500);


            builder.Property(p => p.Price)
                .HasColumnType("decimal(18,2)");

     
            builder.Property(p => p.ShelterId)
                .IsRequired();

            builder.Property(p => p.RowVersion)
                .IsRowVersion();

            builder.HasOne<ApplicationUser>()
                .WithMany()
                .HasForeignKey(a=>a.ShelterId)
                .HasPrincipalKey(k=>k.Id);
        }
    }
}
