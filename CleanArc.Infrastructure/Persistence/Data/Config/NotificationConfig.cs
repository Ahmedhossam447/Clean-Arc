using CleanArc.Core.Entities;
using CleanArc.Infrastructure.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CleanArc.Infrastructure.Persistence.Data.Config
{
    internal class NotificationConfig : IEntityTypeConfiguration<Notification>
    {
        public void Configure(EntityTypeBuilder<Notification> builder)
        {
           builder.HasKey(n => n.Id);
              builder.Property(n => n.UserId).IsRequired().HasMaxLength(450);
            builder.Property(n => n.Message).IsRequired().HasMaxLength(1000);
            builder.Property(n => n.IsRead).IsRequired();
            builder.Property(n => n.CreatedAt).IsRequired();
            builder.HasIndex(n => n.UserId).IncludeProperties(n=>n.IsRead);
            builder.HasOne<ApplicationUser>().WithMany().
                HasForeignKey(n => n.UserId).
                OnDelete(DeleteBehavior.Cascade);



        }
    }
}