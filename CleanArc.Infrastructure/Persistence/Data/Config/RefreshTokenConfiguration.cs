using CleanArc.Core.Entities;
using CleanArc.Infrastructure.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CleanArc.Infrastructure.Persistence.Data.Config
{
    public class RefreshTokenConfiguration : IEntityTypeConfiguration<RefreshToken>
    {
        public void Configure(EntityTypeBuilder<RefreshToken> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Token)
                .IsRequired()
                .HasMaxLength(256);

            builder.Property(x => x.UserId)
                .IsRequired();

            builder.Property(x => x.RevokedReason)
                .HasMaxLength(256);

            builder.Property(x => x.ReplacedByToken)
                .HasMaxLength(256);

            builder.HasOne<ApplicationUser>()
                .WithMany()
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasIndex(x => x.Token)
                .IsUnique();

            builder.HasIndex(x => x.UserId);

            builder.HasIndex(x => x.ExpiresAt);
        }
    }
}
