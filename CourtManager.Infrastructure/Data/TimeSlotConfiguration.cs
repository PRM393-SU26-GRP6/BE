using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using CourtManager.Domain.Entities;

namespace CourtManager.Infrastructure.Data;

public class TimeSlotConfiguration : IEntityTypeConfiguration<TimeSlot>
{
    public void Configure(EntityTypeBuilder<TimeSlot> builder)
    {
        builder.HasKey(s => s.SlotId);
        builder.Property(s => s.StartTime).IsRequired();
        builder.Property(s => s.EndTime).IsRequired();
        builder.Property(s => s.SlotStatus).IsRequired().HasConversion<string>().HasDefaultValue(CourtManager.Domain.Enums.SlotStatus.Available).HasMaxLength(50);
        builder.Property(s => s.CreatedAt).ValueGeneratedOnAdd().HasDefaultValueSql("CURRENT_TIMESTAMP");
        builder.HasQueryFilter(s => !s.IsDeleted);
        builder.HasMany(s => s.BookingItems).WithOne(bi => bi.Slot).HasForeignKey(bi => bi.SlotId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(s => s.LockedByUser).WithMany().HasForeignKey(s => s.LockedBy).OnDelete(DeleteBehavior.Restrict);
        builder.HasIndex(s => new { s.FieldId, s.StartTime });
        builder.ToTable("TimeSlots");
    }
}
