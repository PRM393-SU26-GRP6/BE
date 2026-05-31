using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using CourtManager.Domain.Entities;
using CourtManager.Infrastructure.Data;

namespace CourtManager.Infrastructure;

/// <summary>
/// Application DbContext for Code First approach with Entity Framework Core.
/// Configures entities, relationships, and database mappings using Fluent API.
/// </summary>
public class ApplicationDbContext : IdentityDbContext<User, Role, Guid, IdentityUserClaim<Guid>, UserRole, IdentityUserLogin<Guid>, IdentityRoleClaim<Guid>, IdentityUserToken<Guid>>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options) { }

    // DbSets for entities
    public DbSet<Venue> Venues => Set<Venue>();
    public DbSet<VenueImage> VenueImages => Set<VenueImage>();
    public DbSet<FootballField> FootballFields => Set<FootballField>();
    public DbSet<Amenity> Amenities => Set<Amenity>();
    public DbSet<VenueAmenity> VenueAmenities => Set<VenueAmenity>();
    public DbSet<TimeSlot> TimeSlots => Set<TimeSlot>();
    public DbSet<Discount> Discounts => Set<Discount>();
    public DbSet<BookingDiscount> BookingDiscounts => Set<BookingDiscount>();
    public DbSet<Booking> Bookings => Set<Booking>();
    public DbSet<BookingItem> BookingItems => Set<BookingItem>();
    public DbSet<Payment> Payments => Set<Payment>();
    public DbSet<ChatRoom> ChatRooms => Set<ChatRoom>();
    public DbSet<Message> Messages => Set<Message>();
    public DbSet<Notification> Notifications => Set<Notification>();
    public DbSet<NotificationRecipient> NotificationRecipients => Set<NotificationRecipient>();
    public DbSet<Review> Reviews => Set<Review>();
    public DbSet<UserDevice> UserDevices => Set<UserDevice>();

    /// <summary>
    /// Configures the model and applies all entity configurations.
    /// </summary>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<User>().ToTable("Users");
        modelBuilder.Entity<Role>().ToTable("Roles");
        modelBuilder.Entity<UserRole>().ToTable("UserRoles");
        modelBuilder.Entity<IdentityUserClaim<Guid>>().ToTable("UserClaims");
        modelBuilder.Entity<IdentityUserLogin<Guid>>().ToTable("UserLogins");
        modelBuilder.Entity<IdentityRoleClaim<Guid>>().ToTable("RoleClaims");
        modelBuilder.Entity<IdentityUserToken<Guid>>().ToTable("UserTokens");

        // Apply all entity configurations
        modelBuilder.ApplyConfiguration(new UserConfiguration());
        modelBuilder.ApplyConfiguration(new FootballFieldConfiguration());
        modelBuilder.ApplyConfiguration(new VenueImageConfiguration());
        modelBuilder.ApplyConfiguration(new TimeSlotConfiguration());
        modelBuilder.ApplyConfiguration(new BookingConfiguration());
        modelBuilder.ApplyConfiguration(new BookingItemConfiguration());
        modelBuilder.ApplyConfiguration(new PaymentConfiguration());
        modelBuilder.ApplyConfiguration(new ChatRoomConfiguration());
        modelBuilder.ApplyConfiguration(new MessageConfiguration());
        modelBuilder.ApplyConfiguration(new NotificationConfiguration());
        modelBuilder.ApplyConfiguration(new ReviewConfiguration());
        modelBuilder.ApplyConfiguration(new RoleConfiguration());
        modelBuilder.ApplyConfiguration(new UserRoleConfiguration());
        modelBuilder.ApplyConfiguration(new DiscountConfiguration());
        modelBuilder.ApplyConfiguration(new UserDeviceConfiguration());

        modelBuilder.Entity<BookingDiscount>().HasKey(bd => new { bd.BookingId, bd.DiscountId });
        modelBuilder.Entity<VenueAmenity>().HasKey(va => new { va.VenueId, va.AmenityId });
        modelBuilder.Entity<NotificationRecipient>().HasKey(nr => nr.RecipientId);

        // Seed initial data (optional)
        SeedData(modelBuilder);
    }

    /// <summary>
    /// Seeds initial data into the database.
    /// </summary>
    private static void SeedData(ModelBuilder modelBuilder)
    {
        // Hardcoded password hash for "Password@123" to avoid EF Core dynamic model change warnings
        var defaultPasswordHash = "AQAAAAIAAYagAAAAEMhNOhWJhrehCy84iiKMjD+gAwmKtd2V+CHm4EhzxmaTyXKW9OS5bmKjoFGKqWDFAg==";

        // Role GUIDs from RoleConfiguration
        var adminRoleId = new Guid("10000000-0000-0000-0000-000000000001");
        var ownerRoleId = new Guid("10000000-0000-0000-0000-000000000002");
        var userRoleId = new Guid("10000000-0000-0000-0000-000000000003");

        var users = new List<User>();
        var userRoles = new List<UserRole>();

        var accountData = new[]
        {
            new { Id = new Guid("20000000-0000-0000-0000-000000000001"), Role = adminRoleId, FullName = "System Admin1", Email = "admin1@court.com", Phone = "0900000001" },
            new { Id = new Guid("20000000-0000-0000-0000-000000000002"), Role = adminRoleId, FullName = "System Admin2", Email = "admin2@court.com", Phone = "0900000002" },
            new { Id = new Guid("20000000-0000-0000-0000-000000000003"), Role = ownerRoleId, FullName = "Court Owner1", Email = "owner1@court.com", Phone = "0900000003" },
            new { Id = new Guid("20000000-0000-0000-0000-000000000004"), Role = ownerRoleId, FullName = "Court Owner2", Email = "owner2@court.com", Phone = "0900000004" },
            new { Id = new Guid("20000000-0000-0000-0000-000000000005"), Role = ownerRoleId, FullName = "Court Owner3", Email = "owner3@court.com", Phone = "0900000005" },
            new { Id = new Guid("20000000-0000-0000-0000-000000000006"), Role = userRoleId, FullName = "Pro User1", Email = "user1@court.com", Phone = "0900000006" },
            new { Id = new Guid("20000000-0000-0000-0000-000000000007"), Role = userRoleId, FullName = "Pro User2", Email = "user2@court.com", Phone = "0900000007" },
            new { Id = new Guid("20000000-0000-0000-0000-000000000008"), Role = userRoleId, FullName = "Casual User3", Email = "user3@court.com", Phone = "0900000008" },
            new { Id = new Guid("20000000-0000-0000-0000-000000000009"), Role = userRoleId, FullName = "Casual User4", Email = "user4@court.com", Phone = "0900000009" },
            new { Id = new Guid("20000000-0000-0000-0000-000000000010"), Role = userRoleId, FullName = "Newbie User5", Email = "user5@court.com", Phone = "0900000010" }
        };

        foreach (var data in accountData)
        {
            users.Add(new User
            {
                Id = data.Id,
                FullName = data.FullName,
                Phone = data.Phone,
                UserName = data.Email,
                NormalizedUserName = data.Email.ToUpper(),
                Email = data.Email,
                NormalizedEmail = data.Email.ToUpper(),
                PhoneNumber = data.Phone,
                PasswordHash = defaultPasswordHash,
                CreatedAt = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                IsActive = true,
                ConcurrencyStamp = data.Id.ToString(),
                SecurityStamp = data.Id.ToString()
            });

            userRoles.Add(new UserRole
            {
                UserId = data.Id,
                RoleId = data.Role,
                AssignedAt = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            });
        }

        modelBuilder.Entity<User>().HasData(users);
        modelBuilder.Entity<UserRole>().HasData(userRoles);

        // --- Seed Venues ---
        var venue1Id = new Guid("30000000-0000-0000-0000-000000000001");
        var venue2Id = new Guid("30000000-0000-0000-0000-000000000002");
        var owner1Id = new Guid("20000000-0000-0000-0000-000000000003"); // Court Manager1

        var venues = new List<Venue>
        {
            new Venue
            {
                VenueId = venue1Id,
                OwnerId = owner1Id,
                VenueName = "Sân Bóng Chảo Lửa",
                Address = "30 Phan Thúc Duyện, Tân Bình, TP.HCM",
                Latitude = 10.8016m,
                Longitude = 106.6653m,
                Description = "Cụm sân bóng mini lớn nhất Tân Bình.",
                OpeningHours = "06:00 - 23:00",
                PhoneContact = "0900000003",
                IsActive = true,
                CreatedAt = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            },
            new Venue
            {
                VenueId = venue2Id,
                OwnerId = owner1Id,
                VenueName = "Sân Bóng Thăng Long",
                Address = "Hẻm 12 Thăng Long, Tân Bình, TP.HCM",
                Latitude = 10.8030m,
                Longitude = 106.6620m,
                Description = "Sân cỏ nhân tạo mới thay, chất lượng cao.",
                OpeningHours = "05:00 - 24:00",
                PhoneContact = "0900000003",
                IsActive = true,
                CreatedAt = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            }
        };
        modelBuilder.Entity<Venue>().HasData(venues);

        // --- Seed Fields ---
        var fields = new List<FootballField>
        {
            new FootballField { Id = new Guid("40000000-0000-0000-0000-000000000001"), VenueId = venue1Id, FieldName = "Sân 5 số 1", FieldType = CourtManager.Domain.Enums.FieldType.FiveASide, PricePerHour = 200000, IsActive = true, CreatedAt = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
            new FootballField { Id = new Guid("40000000-0000-0000-0000-000000000002"), VenueId = venue1Id, FieldName = "Sân 5 số 2", FieldType = CourtManager.Domain.Enums.FieldType.FiveASide, PricePerHour = 220000, IsActive = true, CreatedAt = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
            new FootballField { Id = new Guid("40000000-0000-0000-0000-000000000003"), VenueId = venue1Id, FieldName = "Sân 7 VIP", FieldType = CourtManager.Domain.Enums.FieldType.SevenASide, PricePerHour = 450000, IsActive = true, CreatedAt = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
            
            new FootballField { Id = new Guid("40000000-0000-0000-0000-000000000004"), VenueId = venue2Id, FieldName = "Sân A", FieldType = CourtManager.Domain.Enums.FieldType.FiveASide, PricePerHour = 180000, IsActive = true, CreatedAt = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
            new FootballField { Id = new Guid("40000000-0000-0000-0000-000000000005"), VenueId = venue2Id, FieldName = "Sân B", FieldType = CourtManager.Domain.Enums.FieldType.SevenASide, PricePerHour = 400000, IsActive = true, CreatedAt = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc) }
        };
        modelBuilder.Entity<FootballField>().HasData(fields);

        // --- Seed Dummy Booking for Reviews ---
        var dummyBookingId1 = new Guid("60000000-0000-0000-0000-000000000001");
        var dummyBookingId2 = new Guid("60000000-0000-0000-0000-000000000002");
        var dummyBookingId3 = new Guid("60000000-0000-0000-0000-000000000003");

        var dummyBookings = new List<Booking>
        {
            new Booking { Id = dummyBookingId1, UserId = new Guid("20000000-0000-0000-0000-000000000006"), TotalPrice = 200000, DepositAmount = 100000, BookingStatus = CourtManager.Domain.Enums.BookingStatus.Completed, CreatedAt = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
            new Booking { Id = dummyBookingId2, UserId = new Guid("20000000-0000-0000-0000-000000000007"), TotalPrice = 200000, DepositAmount = 100000, BookingStatus = CourtManager.Domain.Enums.BookingStatus.Completed, CreatedAt = new DateTime(2025, 1, 2, 0, 0, 0, DateTimeKind.Utc) },
            new Booking { Id = dummyBookingId3, UserId = new Guid("20000000-0000-0000-0000-000000000008"), TotalPrice = 200000, DepositAmount = 100000, BookingStatus = CourtManager.Domain.Enums.BookingStatus.Completed, CreatedAt = new DateTime(2025, 1, 3, 0, 0, 0, DateTimeKind.Utc) }
        };
        modelBuilder.Entity<Booking>().HasData(dummyBookings);

        // --- Seed Reviews ---
        var reviews = new List<Review>
        {
            new Review { ReviewId = new Guid("50000000-0000-0000-0000-000000000001"), VenueId = venue1Id, UserId = new Guid("20000000-0000-0000-0000-000000000006"), BookingId = dummyBookingId1, Rating = 5, Comment = "Sân rất đẹp, đèn sáng.", CreatedAt = new DateTime(2025, 1, 2, 0, 0, 0, DateTimeKind.Utc) },
            new Review { ReviewId = new Guid("50000000-0000-0000-0000-000000000002"), VenueId = venue1Id, UserId = new Guid("20000000-0000-0000-0000-000000000007"), BookingId = dummyBookingId2, Rating = 4, Comment = "Cỏ hơi mòn ở khu vực giữa sân.", CreatedAt = new DateTime(2025, 1, 3, 0, 0, 0, DateTimeKind.Utc) },
            new Review { ReviewId = new Guid("50000000-0000-0000-0000-000000000003"), VenueId = venue2Id, UserId = new Guid("20000000-0000-0000-0000-000000000008"), BookingId = dummyBookingId3, Rating = 5, Comment = "Giá cả hợp lý, chủ sân nhiệt tình.", CreatedAt = new DateTime(2025, 1, 4, 0, 0, 0, DateTimeKind.Utc) }
        };
        modelBuilder.Entity<Review>().HasData(reviews);
    }
}
