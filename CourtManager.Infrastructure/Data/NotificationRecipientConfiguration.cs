using CourtManager.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CourtManager.Infrastructure.Data;

public class NotificationRecipientConfiguration : IEntityTypeConfiguration<NotificationRecipient>
{
    public void Configure(EntityTypeBuilder<NotificationRecipient> builder)
    {
        builder.HasKey(nr => nr.RecipientId);

        builder.Property(nr => nr.NotificationId)
            .IsRequired();

        builder.Property(nr => nr.UserId)
            .IsRequired();

        builder.Property(nr => nr.ReadAt)
            .IsRequired(false);

        // Soft delete query filter
        builder.HasQueryFilter(nr => !nr.IsDeleted);

        builder.HasOne(nr => nr.Notification)
            .WithMany(n => n.Recipients)
            .HasForeignKey(nr => nr.NotificationId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(nr => nr.User)
            .WithMany(u => u.NotificationRecipients)
            .HasForeignKey(nr => nr.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(nr => new { nr.NotificationId, nr.UserId })
            .IsUnique();
    }
}
