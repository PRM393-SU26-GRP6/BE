using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using CourtManager.Domain.Entities;

namespace CourtManager.Infrastructure.Data;

public class UserDeviceConfiguration : IEntityTypeConfiguration<UserDevice>
{
    public void Configure(EntityTypeBuilder<UserDevice> builder)
    {
        builder.HasKey(d => d.DeviceId);
        builder.Property(d => d.FcmToken).IsRequired().HasMaxLength(500);
        builder.Property(d => d.DeviceType).IsRequired().HasMaxLength(20);
        builder.Property(d => d.CreatedAt).ValueGeneratedOnAdd().HasDefaultValueSql("CURRENT_TIMESTAMP");
        builder.HasIndex(d => d.FcmToken).IsUnique();

        builder.HasOne(d => d.User)
            .WithMany(u => u.Devices)
            .HasForeignKey(d => d.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.ToTable("UserDevices");
    }
}
