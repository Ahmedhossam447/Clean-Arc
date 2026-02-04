using CleanArc.Core.Entites;
using CleanArc.Infrastructure.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CleanArc.Infrastructure.Persistence.Data.Config
{
    public class MessageConfiguration : IEntityTypeConfiguration<Message>
    {
        public void Configure(EntityTypeBuilder<Message> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.SenderId)
                .IsRequired();

            builder.Property(x => x.ReceiverId)
                .IsRequired();

            builder.Property(x => x.Content)
                .IsRequired()
                .HasMaxLength(2000);

            builder.HasOne<ApplicationUser>()
                .WithMany()
                .HasForeignKey(x => x.SenderId)
                .HasPrincipalKey(u => u.Id)
                .OnDelete(DeleteBehavior.NoAction);

            builder.HasOne<ApplicationUser>()
                .WithMany()
                .HasForeignKey(x => x.ReceiverId)
                .HasPrincipalKey(u => u.Id)
                .OnDelete(DeleteBehavior.NoAction);

            builder.HasIndex(x => x.SenderId);
            builder.HasIndex(x => x.ReceiverId);
            builder.HasIndex(x => x.SentAt);
        }
    }
}
