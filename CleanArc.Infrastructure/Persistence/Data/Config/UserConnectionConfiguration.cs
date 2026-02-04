using CleanArc.Core.Entites;
using CleanArc.Infrastructure.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CleanArc.Infrastructure.Persistence.Data.Config
{
    public class UserConnectionConfiguration : IEntityTypeConfiguration<UserConnection>
    {
        public void Configure(EntityTypeBuilder<UserConnection> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.UserId)
                .IsRequired();

            builder.Property(x => x.ConnectionId)
                .IsRequired()
                .HasMaxLength(100);

            builder.HasOne<ApplicationUser>()
                .WithMany()
                .HasForeignKey(x => x.UserId)
                .HasPrincipalKey(u => u.Id)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasIndex(x => x.ConnectionId).IsUnique();
            builder.HasIndex(x => x.UserId);
        }
    }
}
