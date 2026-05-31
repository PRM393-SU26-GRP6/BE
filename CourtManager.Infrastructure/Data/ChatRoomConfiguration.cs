using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using CourtManager.Domain.Entities;

namespace CourtManager.Infrastructure.Data;

/// <summary>
/// Entity configuration for ChatRoom entity using Fluent API.
/// </summary>
public class ChatRoomConfiguration : IEntityTypeConfiguration<ChatRoom>
{
    public void Configure(EntityTypeBuilder<ChatRoom> builder)
    {
        // Primary Key
        builder.HasKey(r => r.RoomId);

        // Properties
        builder.Property(r => r.CustomerId)
            .IsRequired();

        builder.Property(r => r.HostId)
            .IsRequired();

        builder.Property(r => r.CreatedAt)
            .ValueGeneratedOnAdd()
            .HasDefaultValueSql("CURRENT_TIMESTAMP");

        // Soft delete query filter
        builder.HasQueryFilter(r => !r.IsDeleted);

        // One chat room per customer-host pair
        builder.HasIndex(r => new { r.CustomerId, r.HostId }).IsUnique();

        // Relationships
        builder.HasOne(r => r.Customer)
            .WithMany(u => u.CustomerChatRooms)
            .HasForeignKey(r => r.CustomerId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(r => r.Host)
            .WithMany(u => u.HostChatRooms)
            .HasForeignKey(r => r.HostId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(r => r.Messages)
            .WithOne(m => m.Room)
            .HasForeignKey(m => m.RoomId)
            .OnDelete(DeleteBehavior.Cascade);

        // Table configuration
        builder.ToTable("ChatRooms");
    }
}
