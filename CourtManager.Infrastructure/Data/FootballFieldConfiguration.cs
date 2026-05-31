using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using CourtManager.Domain.Entities;

namespace CourtManager.Infrastructure.Data;

public class FootballFieldConfiguration : IEntityTypeConfiguration<FootballField>
{
    public void Configure(EntityTypeBuilder<FootballField> builder)
    {
        builder.HasKey(f => f.Id);
        builder.Property(f => f.Id).ValueGeneratedNever();
        builder.Property(f => f.FieldName).IsRequired().HasMaxLength(200);
        builder.Property(f => f.Description).HasMaxLength(1000);
        builder.Property(f => f.FieldType).IsRequired().HasConversion<string>().HasMaxLength(100);
        builder.Property(f => f.PricePerHour).HasPrecision(10, 2);
        builder.Property(f => f.CreatedAt).ValueGeneratedOnAdd().HasDefaultValueSql("CURRENT_TIMESTAMP");
        builder.Property(f => f.IsActive).HasDefaultValue(true);
        builder.HasQueryFilter(f => !f.IsDeleted);
        builder.HasMany(f => f.TimeSlots).WithOne(s => s.Field).HasForeignKey(s => s.FieldId).OnDelete(DeleteBehavior.Cascade);
        builder.ToTable("FootballFields");
    }
}
