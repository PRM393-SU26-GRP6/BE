using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using CourtManager.Domain.Entities;

namespace CourtManager.Infrastructure.Data;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.Property(u => u.FullName).IsRequired().HasMaxLength(200);
        builder.Property(u => u.Phone).IsRequired().HasMaxLength(20);
        builder.Property(u => u.AvatarUrl).IsRequired(false);
        builder.Property(u => u.LoyaltyPoints).HasDefaultValue(0);
        builder.Property(u => u.CreatedAt).ValueGeneratedOnAdd().HasDefaultValueSql("CURRENT_TIMESTAMP");
        builder.Property(u => u.IsActive).HasDefaultValue(true);
        builder.HasQueryFilter(u => !u.IsDeleted);
        builder.HasMany(u => u.Bookings).WithOne(b => b.User).HasForeignKey(b => b.UserId).OnDelete(DeleteBehavior.Restrict);
        builder.HasMany(u => u.CustomerChatRooms).WithOne(r => r.Customer).HasForeignKey(r => r.CustomerId).OnDelete(DeleteBehavior.Cascade);
        builder.HasMany(u => u.HostChatRooms).WithOne(r => r.Host).HasForeignKey(r => r.HostId).OnDelete(DeleteBehavior.Cascade);
        builder.HasMany(u => u.SentMessages).WithOne(m => m.Sender).HasForeignKey(m => m.SenderId).OnDelete(DeleteBehavior.Cascade);
        builder.HasMany(u => u.Reviews).WithOne(r => r.User).HasForeignKey(r => r.UserId).OnDelete(DeleteBehavior.Restrict);
    }
}
