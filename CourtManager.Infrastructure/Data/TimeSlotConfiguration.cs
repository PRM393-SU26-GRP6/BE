using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using CourtManager.Domain.Entities;

namespace CourtManager.Infrastructure.Data;

public class TimeSlotConfiguration : IEntityTypeConfiguration<TimeSlot>
{
    public void Configure(EntityTypeBuilder<TimeSlot> builder)
    {
        builder.HasKey(s => s.SlotId);
        builder.Property(s => s.SlotId).ValueGeneratedNever();
        builder.Property(s => s.StartTime).IsRequired().HasColumnType("time");
        builder.Property(s => s.EndTime).IsRequired().HasColumnType("time");
        builder.Property(s => s.SelectedDate).IsRequired().HasColumnType("date");
        builder.Property(s => s.SlotStatus).IsRequired().HasConversion<string>().HasDefaultValue(CourtManager.Domain.Enums.SlotStatus.Available).HasMaxLength(50);
        builder.Property(s => s.CreatedAt).ValueGeneratedOnAdd().HasDefaultValueSql("CURRENT_TIMESTAMP");
        builder.HasQueryFilter(s => !s.IsDeleted);
        builder.HasMany(s => s.BookingItems).WithOne(bi => bi.Slot).HasForeignKey(bi => bi.SlotId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(s => s.LockedByUser).WithMany().HasForeignKey(s => s.LockedBy).OnDelete(DeleteBehavior.Restrict);
        builder.HasIndex(s => new { s.FieldId, s.SelectedDate, s.StartTime }).HasDatabaseName("IX_TimeSlots_FieldId_SelectedDate_StartTime");

        // Optimistic concurrency control via version number (PostgreSQL compatible)
        builder.Property(s => s.RowVersion).IsConcurrencyToken();

        builder.ToTable("TimeSlots");
    }
}
