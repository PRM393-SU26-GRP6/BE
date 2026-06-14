using CourtManager.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CourtManager.Infrastructure.Data;

public class FieldScheduleConfiguration : IEntityTypeConfiguration<FieldSchedule>
{
    public void Configure(EntityTypeBuilder<FieldSchedule> builder)
    {
        builder.HasKey(s => s.ScheduleId);

        builder.Property(s => s.ScheduleId)
            .ValueGeneratedNever();

        builder.Property(s => s.DayOfWeek)
            .IsRequired();

        builder.Property(s => s.OpenTime)
            .IsRequired();

        builder.Property(s => s.CloseTime)
            .IsRequired();

        builder.Property(s => s.SlotDurationMinutes)
            .IsRequired()
            .HasDefaultValue(60);

        builder.Property(s => s.IsActive)
            .IsRequired()
            .HasDefaultValue(true);

        builder.Property(s => s.CreatedAt)
            .IsRequired();

        // Unique index: one row per (FieldId, DayOfWeek)
        builder.HasIndex(s => new { s.FieldId, s.DayOfWeek })
            .IsUnique()
            .HasDatabaseName("IX_FieldSchedules_FieldId_DayOfWeek");

        builder.HasOne(s => s.Field)
            .WithMany()
            .HasForeignKey(s => s.FieldId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
