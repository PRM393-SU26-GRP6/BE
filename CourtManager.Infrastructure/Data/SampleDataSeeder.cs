using CourtManager.Domain.Entities;
using CourtManager.Domain.Enums;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Security.Cryptography;
using System.Text;

namespace CourtManager.Infrastructure.Data;

public static class SampleDataSeeder
{
    private const string DefaultPassword = "Password@123";

    private static readonly Guid UserRoleId = StableGuid("role:user");
    private static readonly Guid AdminRoleId = StableGuid("role:admin");
    private static readonly Guid OwnerRoleId = StableGuid("role:owner");

    public static async Task SeedSampleDataAsync(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var services = scope.ServiceProvider;

        var context = services.GetRequiredService<ApplicationDbContext>();
        await context.Database.MigrateAsync();

        var userManager = services.GetRequiredService<UserManager<User>>();
        var roleManager = services.GetRequiredService<RoleManager<Role>>();

        await EnsureRolesAsync(roleManager);

        var users = await EnsureUsersAsync(userManager);
        if (await context.Venues.AnyAsync(v => v.VenueName == "Saigon Riverside Sports Park"))
        {
            return;
        }

        await SeedDomainDataAsync(context, users);
    }

    private static async Task EnsureRolesAsync(RoleManager<Role> roleManager)
    {
        var roles = new[]
        {
            new Role
            {
                Id = UserRoleId,
                Name = "User",
                NormalizedName = "USER",
                Description = "Regular booking user",
                CreatedAt = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                ConcurrencyStamp = UserRoleId.ToString()
            },
            new Role
            {
                Id = AdminRoleId,
                Name = "Admin",
                NormalizedName = "ADMIN",
                Description = "Administrator with full access",
                CreatedAt = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                ConcurrencyStamp = AdminRoleId.ToString()
            },
            new Role
            {
                Id = OwnerRoleId,
                Name = "Owner",
                NormalizedName = "OWNER",
                Description = "Venue and field owner",
                CreatedAt = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                ConcurrencyStamp = OwnerRoleId.ToString()
            }
        };

        foreach (var role in roles)
        {
            if (!await roleManager.RoleExistsAsync(role.Name!))
            {
                await roleManager.CreateAsync(role);
            }
        }
    }

    private static async Task<SeedUsers> EnsureUsersAsync(UserManager<User> userManager)
    {
        var seedUsers = new[]
        {
            MakeUserSpec("lan-nguyen", "Lan Nguyen", "lan.nguyen@courtmanager.vn", "0902311001", "Admin"),
            MakeUserSpec("minh-tran", "Minh Tran", "minh.tran@courtmanager.vn", "0902311002", "Admin"),
            MakeUserSpec("duy-pham", "Duy Pham", "duy.pham@sporthub.vn", "0902311003", "Owner"),
            MakeUserSpec("hanh-le", "Hanh Le", "hanh.le@saigonfields.vn", "0902311004", "Owner"),
            MakeUserSpec("quang-vo", "Quang Vo", "quang.vo@greenpitch.vn", "0902311005", "Owner"),
            MakeUserSpec("bao-hoang", "Bao Hoang", "bao.hoang@cityarena.vn", "0902311006", "Owner"),
            MakeUserSpec("an-dang", "An Dang", "andang.football@gmail.com", "0902311007", "User"),
            MakeUserSpec("my-pham", "My Pham", "mypham.saigon@gmail.com", "0902311008", "User"),
            MakeUserSpec("khoa-bui", "Khoa Bui", "khoabui.runner@outlook.com", "0902311009", "User"),
            MakeUserSpec("thao-do", "Thao Do", "thaodo.booking@gmail.com", "0902311010", "User"),
            MakeUserSpec("tuan-mai", "Tuan Mai", "tuanmai.sports@yahoo.com", "0902311011", "User"),
            MakeUserSpec("linh-huynh", "Linh Huynh", "linhhuynh.club@gmail.com", "0902311012", "User")
        };

        foreach (var spec in seedUsers)
        {
            var user = await userManager.FindByEmailAsync(spec.Email);
            if (user == null)
            {
                user = new User
                {
                    Id = spec.Id,
                    FullName = spec.FullName,
                    UserName = spec.Email,
                    NormalizedUserName = spec.Email.ToUpperInvariant(),
                    Email = spec.Email,
                    NormalizedEmail = spec.Email.ToUpperInvariant(),
                    Phone = spec.Phone,
                    PhoneNumber = spec.Phone,
                    EmailConfirmed = true,
                    PhoneNumberConfirmed = true,
                    CreatedAt = new DateTime(2026, 1, 5, 0, 0, 0, DateTimeKind.Utc),
                    IsActive = true,
                    LoyaltyPoints = spec.Role == "User" ? 120 : 0,
                    SecurityStamp = spec.Id.ToString(),
                    ConcurrencyStamp = spec.Id.ToString()
                };

                var createResult = await userManager.CreateAsync(user, DefaultPassword);
                if (!createResult.Succeeded)
                {
                    throw new InvalidOperationException($"Cannot create sample user {spec.Email}: {string.Join(", ", createResult.Errors.Select(e => e.Description))}");
                }
            }

            if (!await userManager.IsInRoleAsync(user, spec.Role))
            {
                await userManager.AddToRoleAsync(user, spec.Role);
            }
        }

        return new SeedUsers(
            Admins: seedUsers.Where(u => u.Role == "Admin").Select(u => u.Id).ToArray(),
            Owners: seedUsers.Where(u => u.Role == "Owner").Select(u => u.Id).ToArray(),
            Customers: seedUsers.Where(u => u.Role == "User").Select(u => u.Id).ToArray());
    }

    private static async Task SeedDomainDataAsync(ApplicationDbContext context, SeedUsers users)
    {
        var now = DateTime.UtcNow;
        var baseDate = DateTime.SpecifyKind(now.Date.AddDays(2), DateTimeKind.Utc);

        var amenities = new[]
        {
            new Amenity { AmenityId = Id("amenity", 1), Name = "Covered parking", Icon = "parking" },
            new Amenity { AmenityId = Id("amenity", 2), Name = "Changing room", Icon = "shirt" },
            new Amenity { AmenityId = Id("amenity", 3), Name = "Shower area", Icon = "shower-head" },
            new Amenity { AmenityId = Id("amenity", 4), Name = "Night lighting", Icon = "lightbulb" },
            new Amenity { AmenityId = Id("amenity", 5), Name = "Drinking water", Icon = "droplets" },
            new Amenity { AmenityId = Id("amenity", 6), Name = "Equipment rental", Icon = "package" },
            new Amenity { AmenityId = Id("amenity", 7), Name = "Cafe lounge", Icon = "coffee" },
            new Amenity { AmenityId = Id("amenity", 8), Name = "First aid kit", Icon = "cross" },
            new Amenity { AmenityId = Id("amenity", 9), Name = "Free wifi", Icon = "wifi" },
            new Amenity { AmenityId = Id("amenity", 10), Name = "Security locker", Icon = "lock" },
            new Amenity { AmenityId = Id("amenity", 11), Name = "Bike parking", Icon = "bike" },
            new Amenity { AmenityId = Id("amenity", 12), Name = "Scoreboard", Icon = "table" }
        };

        var venues = new[]
        {
            Venue(1, users.Owners[0], "Saigon Riverside Sports Park", "12 Nguyen Huu Canh, Binh Thanh, Ho Chi Minh City", 10.791054m, 106.719809m, "Riverside venue with four compact football fields.", "06:00-23:00", "02873010001", true),
            Venue(2, users.Owners[0], "Thanh Da Community Football Hub", "91 Binh Quoi, Binh Thanh, Ho Chi Minh City", 10.815713m, 106.731719m, "Community sports hub near Thanh Da peninsula.", "05:30-22:30", "02873010002", true),
            Venue(3, users.Owners[1], "District 7 Green Pitch", "45 Nguyen Luong Bang, District 7, Ho Chi Minh City", 10.729210m, 106.721916m, "Well maintained turf fields for evening leagues.", "06:00-23:30", "02873010003", true),
            Venue(4, users.Owners[1], "Phu My Hung Arena", "19 Ton Dat Tien, District 7, Ho Chi Minh City", 10.732221m, 106.704730m, "Premium arena close to office and residential areas.", "06:00-23:00", "02873010004", true),
            Venue(5, users.Owners[2], "Thu Duc University Stadium", "1 Vo Van Ngan, Thu Duc City, Ho Chi Minh City", 10.849643m, 106.771566m, "Large venue suitable for student tournaments.", "05:00-22:00", "02873010005", true),
            Venue(6, users.Owners[2], "Linh Trung Five-A-Side Club", "37 Linh Trung, Thu Duc City, Ho Chi Minh City", 10.871823m, 106.779496m, "Neighborhood five-a-side club with loyal weekly players.", "05:30-22:30", "02873010006", true),
            Venue(7, users.Owners[3], "Tan Binh Flight Path Fields", "88 Bach Dang, Tan Binh, Ho Chi Minh City", 10.813651m, 106.665408m, "Convenient fields near the airport corridor.", "06:00-23:00", "02873010007", true),
            Venue(8, users.Owners[3], "Go Vap Weekend Arena", "154 Phan Van Tri, Go Vap, Ho Chi Minh City", 10.833116m, 106.680982m, "Popular weekend venue for amateur clubs.", "06:00-23:30", "02873010008", true),
            Venue(9, users.Owners[0], "Binh Tan Sports Yard", "22 Ten Lua, Binh Tan, Ho Chi Minh City", 10.753894m, 106.607990m, "Accessible west-side venue with affordable slots.", "05:30-22:00", "02873010009", true),
            Venue(10, users.Owners[1], "Da Nang Han River Football Center", "75 Tran Hung Dao, Son Tra, Da Nang", 16.070884m, 108.229401m, "Modern riverside venue in central Da Nang.", "06:00-23:00", "02367301010", true),
            Venue(11, users.Owners[2], "Hanoi West Lake Mini Pitch", "28 Trich Sai, Tay Ho, Hanoi", 21.055408m, 105.813839m, "Compact football complex near West Lake.", "06:00-22:30", "02473010011", true),
            Venue(12, users.Owners[3], "Can Tho Ninh Kieu Sports Ground", "9 Hai Ba Trung, Ninh Kieu, Can Tho", 10.034103m, 105.788535m, "Central Can Tho venue for evening bookings.", "05:30-22:30", "02927301012", true)
        };

        var fields = venues.SelectMany((venue, index) => new[]
        {
            Field(index * 2 + 1, venue.VenueId, $"{venue.VenueName} A", "Main artificial turf field.", index % 3 == 0 ? FieldType.FiveASide : FieldType.SevenASide, 180000 + index * 5000, true),
            Field(index * 2 + 2, venue.VenueId, $"{venue.VenueName} B", "Secondary field for friendly matches.", index % 3 == 2 ? FieldType.ElevenASide : FieldType.FiveASide, 220000 + index * 5000, index != 8)
        }).ToArray();

        var slots = new List<TimeSlot>();
        var slotIndex = 1;
        foreach (var field in fields.Take(12))
        {
            for (var hour = 18; hour < 22; hour++)
            {
                var status = slotIndex % 9 == 0 ? SlotStatus.Booked : slotIndex % 7 == 0 ? SlotStatus.Locked : SlotStatus.Available;
                slots.Add(new TimeSlot
                {
                    SlotId = Id("slot", slotIndex),
                    FieldId = field.Id,
                    StartTime = baseDate.AddHours(hour),
                    EndTime = baseDate.AddHours(hour + 1),
                    Price = field.PricePerHour,
                    SlotStatus = status,
                    LockedUntil = status == SlotStatus.Locked ? now.AddMinutes(20) : null,
                    CreatedAt = now.AddDays(-14 + slotIndex % 5)
                });
                slotIndex++;
            }
        }

        var discounts = new[]
        {
            Discount(1, users.Owners[0], venues[0].VenueId, "RIVER20", "Riverside weekday promotion", DiscountType.Percentage, 20, 200000, 60000, 40, 5, true, now),
            Discount(2, users.Owners[0], null, "COMMUNITY50", "Community club voucher", DiscountType.Fixed, 50000, 300000, 50000, 30, 8, true, now),
            Discount(3, users.Owners[1], venues[2].VenueId, "GREEN10", "Green Pitch early week", DiscountType.Percentage, 10, 0, 40000, 25, 4, true, now),
            Discount(4, users.Owners[1], null, "ARENA75", "Arena loyalty discount", DiscountType.Fixed, 75000, 500000, 75000, 20, 6, true, now),
            Discount(5, users.Owners[2], venues[4].VenueId, "STUDENT15", "Student evening offer", DiscountType.Percentage, 15, 150000, 45000, 60, 12, true, now),
            Discount(6, users.Owners[2], null, "THUDUC30", "Thu Duc neighborhood voucher", DiscountType.Fixed, 30000, 180000, 30000, 80, 18, true, now),
            Discount(7, users.Owners[3], venues[6].VenueId, "FLIGHT12", "Airport field promotion", DiscountType.Percentage, 12, 200000, 50000, 35, 7, true, now),
            Discount(8, users.Owners[3], null, "WEEKEND40", "Weekend booking voucher", DiscountType.Fixed, 40000, 250000, 40000, 45, 9, true, now),
            Discount(9, users.Owners[0], venues[8].VenueId, "WESTSIDE8", "West side happy hour", DiscountType.Percentage, 8, 0, 30000, 50, 3, true, now),
            Discount(10, users.Owners[1], venues[9].VenueId, "HANRIVER60", "Han River group voucher", DiscountType.Fixed, 60000, 360000, 60000, 28, 5, true, now),
            Discount(11, users.Owners[2], venues[10].VenueId, "WESTLAKE18", "West Lake membership discount", DiscountType.Percentage, 18, 300000, 70000, 22, 10, true, now),
            Discount(12, users.Owners[3], venues[11].VenueId, "NINHKIEU25", "Ninh Kieu off-peak voucher", DiscountType.Fixed, 25000, 120000, 25000, 70, 11, true, now)
        };

        var bookings = new[]
        {
            Booking(1, users.Customers[0], 350000, 175000, BookingStatus.Pending, "Waiting for owner confirmation.", now.AddDays(-5)),
            Booking(2, users.Customers[1], 360000, 180000, BookingStatus.Accepted, "Accepted by venue owner.", now.AddDays(-4)),
            Booking(3, users.Customers[2], 330000, 165000, BookingStatus.Deposited, "Deposit paid through SePay.", now.AddDays(-3)),
            Booking(4, users.Customers[3], 420000, 210000, BookingStatus.Completed, "Completed after final payment.", now.AddDays(-2)),
            Booking(5, users.Customers[4], 275000, 137500, BookingStatus.Cancelled, "Customer cancelled before deposit.", now.AddDays(-7)),
            Booking(6, users.Customers[5], 410000, 205000, BookingStatus.Rejected, "Venue rejected due to maintenance.", now.AddDays(-6)),
            Booking(7, users.Customers[0], 395000, 197500, BookingStatus.Deposited, "Team league match.", now.AddDays(-1)),
            Booking(8, users.Customers[1], 450000, 225000, BookingStatus.Completed, "Company friendly match.", now.AddDays(-8)),
            Booking(9, users.Customers[2], 285000, 142500, BookingStatus.Accepted, "Awaiting deposit payment.", now.AddDays(-2)),
            Booking(10, users.Customers[3], 310000, 155000, BookingStatus.Pending, "New booking request.", now.AddHours(-20)),
            Booking(11, users.Customers[4], 375000, 187500, BookingStatus.Completed, "Weekend tournament slot.", now.AddDays(-9)),
            Booking(12, users.Customers[5], 295000, 147500, BookingStatus.Cancelled, "Schedule changed by customer.", now.AddDays(-10))
        };

        var bookingItems = bookings.Select((booking, index) => new BookingItem
        {
            BookingItemId = Id("booking-item", index + 1),
            BookingId = booking.Id,
            SlotId = slots[index].SlotId,
            Price = slots[index].Price,
            CreatedAt = booking.CreatedAt
        }).ToArray();

        var bookingDiscounts = new[]
        {
            new BookingDiscount { BookingId = bookings[0].Id, DiscountId = discounts[0].DiscountId, DiscountAmount = 10000 },
            new BookingDiscount { BookingId = bookings[1].Id, DiscountId = discounts[1].DiscountId, DiscountAmount = 40000 },
            new BookingDiscount { BookingId = bookings[2].Id, DiscountId = discounts[2].DiscountId, DiscountAmount = 30000 },
            new BookingDiscount { BookingId = bookings[3].Id, DiscountId = discounts[3].DiscountId, DiscountAmount = 60000 },
            new BookingDiscount { BookingId = bookings[6].Id, DiscountId = discounts[4].DiscountId, DiscountAmount = 25000 },
            new BookingDiscount { BookingId = bookings[7].Id, DiscountId = discounts[5].DiscountId, DiscountAmount = 30000 },
            new BookingDiscount { BookingId = bookings[8].Id, DiscountId = discounts[6].DiscountId, DiscountAmount = 35000 },
            new BookingDiscount { BookingId = bookings[10].Id, DiscountId = discounts[7].DiscountId, DiscountAmount = 40000 }
        };

        var payments = new[]
        {
            Payment(1, bookings[2].Id, bookings[2].DepositAmount, PaymentType.Deposit, PaymentMethod.SePay, PaymentStatus.Success, "DEP-2026-0001", now.AddDays(-3).AddHours(1)),
            Payment(2, bookings[3].Id, bookings[3].DepositAmount, PaymentType.Deposit, PaymentMethod.Cash, PaymentStatus.Success, "DEP-2026-0002", now.AddDays(-2).AddHours(1)),
            Payment(3, bookings[3].Id, bookings[3].TotalPrice - bookings[3].DepositAmount, PaymentType.Final, PaymentMethod.Cash, PaymentStatus.Success, "FIN-2026-0003", now.AddDays(-2).AddHours(3)),
            Payment(4, bookings[6].Id, bookings[6].DepositAmount, PaymentType.Deposit, PaymentMethod.VNPay, PaymentStatus.Success, "DEP-2026-0004", now.AddDays(-1).AddHours(1)),
            Payment(5, bookings[7].Id, bookings[7].DepositAmount, PaymentType.Deposit, PaymentMethod.SePay, PaymentStatus.Success, "DEP-2026-0005", now.AddDays(-8).AddHours(2)),
            Payment(6, bookings[7].Id, bookings[7].TotalPrice - bookings[7].DepositAmount, PaymentType.Final, PaymentMethod.SePay, PaymentStatus.Success, "FIN-2026-0006", now.AddDays(-8).AddHours(4)),
            Payment(7, bookings[10].Id, bookings[10].DepositAmount, PaymentType.Deposit, PaymentMethod.MoMo, PaymentStatus.Success, "DEP-2026-0007", now.AddDays(-9).AddHours(2)),
            Payment(8, bookings[10].Id, bookings[10].TotalPrice - bookings[10].DepositAmount, PaymentType.Final, PaymentMethod.MoMo, PaymentStatus.Success, "FIN-2026-0008", now.AddDays(-9).AddHours(5)),
            Payment(9, bookings[1].Id, bookings[1].DepositAmount, PaymentType.Deposit, PaymentMethod.SePay, PaymentStatus.Pending, "DEP-2026-0009", null),
            Payment(10, bookings[8].Id, bookings[8].DepositAmount, PaymentType.Deposit, PaymentMethod.VNPay, PaymentStatus.Pending, "DEP-2026-0010", null),
            Payment(11, bookings[5].Id, bookings[5].DepositAmount, PaymentType.Deposit, PaymentMethod.SePay, PaymentStatus.Failed, "DEP-2026-0011", null),
            Payment(12, bookings[11].Id, bookings[11].DepositAmount, PaymentType.Deposit, PaymentMethod.Cash, PaymentStatus.Refunded, "DEP-2026-0012", now.AddDays(-10).AddHours(1))
        };

        var rooms = Enumerable.Range(1, 12)
            .Select(i => new ChatRoom
            {
                RoomId = Id("chat-room", i),
                CustomerId = users.Customers[(i - 1) % users.Customers.Length],
                HostId = users.Owners[(i - 1) % users.Owners.Length],
                CreatedAt = now.AddDays(-12 + i),
                LastMessageAt = now.AddDays(-12 + i).AddMinutes(15)
            })
            .ToArray();

        var messages = rooms.Select((room, index) => new Message
        {
            MessageId = Id("message", index + 1),
            RoomId = room.RoomId,
            SenderId = index % 2 == 0 ? room.CustomerId : room.HostId,
            MessageText = SampleMessages[index],
            ReadAt = index % 3 != 0 ? (room.LastMessageAt ?? now) : null,
            SentAt = room.LastMessageAt ?? now
        }).ToArray();

        var notifications = Enumerable.Range(1, 12)
            .Select(i => new Notification
            {
                NotificationId = Id("notification", i),
                SenderId = i % 2 == 0 ? users.Owners[(i - 1) % users.Owners.Length] : users.Customers[(i - 1) % users.Customers.Length],
                Title = SampleNotificationTitles[i - 1],
                Message = SampleNotificationMessages[i - 1],
                Type = (NotificationType)((i - 1) % 5),
                RefId = bookings[(i - 1) % bookings.Length].Id.ToString(),
                CreatedAt = now.AddDays(-i)
            })
            .ToArray();

        var notificationRecipients = notifications.Select((notification, index) => new NotificationRecipient
        {
            RecipientId = Id("notification-recipient", index + 1),
            NotificationId = notification.NotificationId,
            UserId = index % 2 == 0 ? users.Customers[index % users.Customers.Length] : users.Owners[index % users.Owners.Length],
            ReadAt = index % 4 == 0 ? notification.CreatedAt.AddHours(2) : null
        }).ToArray();

        var reviews = Enumerable.Range(1, 12)
            .Select(i => new Review
            {
                ReviewId = Id("review", i),
                UserId = users.Customers[(i - 1) % users.Customers.Length],
                VenueId = venues[(i - 1) % venues.Length].VenueId,
                BookingId = bookings[(i - 1) % bookings.Length].Id,
                Rating = 3 + (i % 3),
                Comment = SampleReviewComments[i - 1],
                CreatedAt = now.AddDays(-20 + i)
            })
            .ToArray();

        var venueImages = venues.SelectMany((venue, index) => new[]
        {
            new VenueImage { ImageId = Id("venue-image", index * 2 + 1), VenueId = venue.VenueId, ImageUrl = $"https://images.courtmanager.vn/venues/{index + 1}-cover.jpg", IsPrimary = true },
            new VenueImage { ImageId = Id("venue-image", index * 2 + 2), VenueId = venue.VenueId, ImageUrl = $"https://images.courtmanager.vn/venues/{index + 1}-field.jpg", IsPrimary = false }
        }).ToArray();

        var venueAmenities = venues.SelectMany((venue, index) => new[]
        {
            new VenueAmenity { VenueId = venue.VenueId, AmenityId = amenities[index % amenities.Length].AmenityId },
            new VenueAmenity { VenueId = venue.VenueId, AmenityId = amenities[(index + 3) % amenities.Length].AmenityId },
            new VenueAmenity { VenueId = venue.VenueId, AmenityId = amenities[(index + 6) % amenities.Length].AmenityId }
        }).ToArray();

        await AddMissingAsync(context, context.Amenities, amenities);
        await AddMissingAsync(context, context.Venues, venues);
        await AddMissingAsync(context, context.FootballFields, fields);
        await AddMissingAsync(context, context.VenueImages, venueImages);
        await AddMissingCompositeAsync(context.VenueAmenities, venueAmenities, x => new { x.VenueId, x.AmenityId });
        await AddMissingAsync(context, context.TimeSlots, slots);
        await AddMissingAsync(context, context.Discounts, discounts);
        await AddMissingAsync(context, context.Bookings, bookings);
        await AddMissingAsync(context, context.BookingItems, bookingItems);
        await AddMissingCompositeAsync(context.BookingDiscounts, bookingDiscounts, x => new { x.BookingId, x.DiscountId });
        await AddMissingAsync(context, context.Payments, payments);
        await AddMissingAsync(context, context.ChatRooms, rooms);
        await AddMissingAsync(context, context.Messages, messages);
        await AddMissingAsync(context, context.Notifications, notifications);
        await AddMissingAsync(context, context.NotificationRecipients, notificationRecipients);
        await AddMissingAsync(context, context.Reviews, reviews);

        await context.SaveChangesAsync();
    }

    private static async Task AddMissingAsync<TEntity>(ApplicationDbContext context, DbSet<TEntity> set, IEnumerable<TEntity> entities)
        where TEntity : class
    {
        var entityType = context.Model.FindEntityType(typeof(TEntity))
            ?? throw new InvalidOperationException($"{typeof(TEntity).Name} is not part of the DbContext model.");
        var key = entityType.FindPrimaryKey() ?? throw new InvalidOperationException($"{entityType.Name} has no primary key.");
        var keyProperty = key.Properties.Single();

        foreach (var entity in entities)
        {
            var keyValue = keyProperty.PropertyInfo!.GetValue(entity);
            if (await set.FindAsync(keyValue) == null)
            {
                await set.AddAsync(entity);
            }
        }
    }

    private static async Task AddMissingCompositeAsync<TEntity, TKey>(DbSet<TEntity> set, IEnumerable<TEntity> entities, Func<TEntity, TKey> keySelector)
        where TEntity : class
    {
        var existing = await set.AsNoTracking().ToListAsync();
        var keys = existing.Select(keySelector).ToHashSet();
        foreach (var entity in entities)
        {
            if (!keys.Contains(keySelector(entity)))
            {
                await set.AddAsync(entity);
            }
        }
    }

    private static UserSpec MakeUserSpec(string key, string fullName, string email, string phone, string role)
        => new(StableGuid($"user:{key}"), fullName, email, phone, role);

    private static Guid Id(string category, int value) => StableGuid($"{category}:{value}");

    private static Guid StableGuid(string key)
    {
        var bytes = MD5.HashData(Encoding.UTF8.GetBytes($"CourtManager.SampleData:{key}"));
        bytes[6] = (byte)((bytes[6] & 0x0F) | 0x50);
        bytes[8] = (byte)((bytes[8] & 0x3F) | 0x80);
        return new Guid(bytes);
    }

    private static Venue Venue(int id, Guid ownerId, string name, string address, decimal latitude, decimal longitude, string description, string openingHours, string phone, bool isActive)
        => new()
        {
            VenueId = Id("venue", id),
            OwnerId = ownerId,
            VenueName = name,
            Address = address,
            Latitude = latitude,
            Longitude = longitude,
            Description = description,
            OpeningHours = openingHours,
            PhoneContact = phone,
            IsActive = isActive,
            CreatedAt = new DateTime(2026, 1, id, 0, 0, 0, DateTimeKind.Utc)
        };

    private static FootballField Field(int id, Guid venueId, string name, string description, FieldType fieldType, decimal price, bool isActive)
        => new()
        {
            Id = Id("field", id),
            VenueId = venueId,
            FieldName = name,
            Description = description,
            FieldType = fieldType,
            PricePerHour = price,
            CreatedAt = new DateTime(2026, 1, 10, 0, 0, 0, DateTimeKind.Utc).AddDays(id),
            IsActive = isActive
        };

    private static Discount Discount(int id, Guid ownerId, Guid? fieldId, string code, string name, DiscountType type, decimal value, decimal minAmount, decimal maxAmount, int usageLimit, int usedCount, bool isActive, DateTime now)
        => new()
        {
            DiscountId = Id("discount", id),
            OwnerId = ownerId,
            VenueId = fieldId,
            Code = code,
            Name = name,
            DiscountType = type,
            Value = value,
            MinBookingAmount = minAmount,
            MaxDiscountAmount = maxAmount,
            UsageLimit = usageLimit,
            UsedCount = usedCount,
            StartDate = now.AddDays(-30),
            EndDate = now.AddDays(60 + id),
            IsActive = isActive,
            CreatedAt = now.AddDays(-25 + id)
        };

    private static Booking Booking(int id, Guid userId, decimal total, decimal deposit, BookingStatus status, string note, DateTime createdAt)
        => new()
        {
            Id = Id("booking", id),
            UserId = userId,
            TotalPrice = total,
            DepositAmount = deposit,
            BookingStatus = status,
            Note = note,
            CreatedAt = createdAt,
            UpdatedAt = status == BookingStatus.Pending ? null : createdAt.AddHours(3)
        };

    private static Payment Payment(int id, Guid bookingId, decimal amount, PaymentType type, PaymentMethod method, PaymentStatus status, string transactionCode, DateTime? paidAt)
        => new()
        {
            Id = Id("payment", id),
            BookingId = bookingId,
            Amount = amount,
            PaymentType = type,
            PaymentMethod = method,
            PaymentStatus = status,
            TransactionCode = transactionCode,
            Gateway = method.ToString(),
            GatewayTransactionId = method == PaymentMethod.SePay ? $"SEPAY-{20260000 + id}" : null,
            GatewayReferenceCode = method == PaymentMethod.SePay ? $"FT{2026050000 + id}" : null,
            GatewayAccountNumber = method == PaymentMethod.SePay ? "84519828888" : null,
            GatewayRawContent = method == PaymentMethod.SePay ? $"CM{transactionCode}" : null,
            PaidAt = paidAt
        };

    private static readonly string[] SampleMessages =
    [
        "Chao anh, toi muon dat san thu bay luc 19h, san con trong khong?",
        "San con trong, anh vui long coc truoc 30 phut de giu lich.",
        "Doi cua minh can thue bong va ao bib, san co ho tro khong?",
        "Ben em co san bong, ao bib va nuoc uong tai quay le tan.",
        "Neu troi mua lon thi minh co doi lich sang ngay khac duoc khong?",
        "Duoc anh, ben em se ho tro doi lich neu thong bao truoc gio da.",
        "Minh da thanh toan coc, nho kiem tra giup ma giao dich.",
        "Ben em da xac nhan coc, lich da duoc giu thanh cong.",
        "Toi can hoa don cho cong ty sau tran dau.",
        "Anh gui thong tin cong ty, ben em se gui hoa don trong ngay.",
        "San co cho gui xe may rieng khong?",
        "Co khu gui xe rieng ngay cong vao, mien phi cho nguoi choi."
    ];

    private static readonly string[] SampleNotificationTitles =
    [
        "Booking request received",
        "Booking accepted",
        "Deposit confirmed",
        "Final payment completed",
        "Booking cancelled",
        "Booking rejected",
        "New chat message",
        "Discount applied",
        "Slot status updated",
        "Payment pending",
        "Review submitted",
        "System maintenance notice"
    ];

    private static readonly string[] SampleNotificationMessages =
    [
        "A customer has submitted a new booking request.",
        "Your booking has been accepted by the venue manager.",
        "Your deposit payment has been confirmed.",
        "Your booking has been fully paid.",
        "A booking was cancelled before the match time.",
        "The venue manager could not accept the requested slot.",
        "You have a new message about your booking.",
        "A discount code was applied to a booking.",
        "One of your selected slots changed status.",
        "A payment is waiting for bank confirmation.",
        "A customer submitted a venue review.",
        "System maintenance is scheduled after midnight."
    ];

    private static readonly string[] SampleReviewComments =
    [
        "Mat san em, den sang va nhan vien huong dan nhanh.",
        "Vi tri de tim, bai xe rong, gia hop ly.",
        "San tot nhung phong thay do hoi dong vao cuoi tuan.",
        "Dat lich nhanh, thanh toan thuan tien.",
        "Chu san ho tro doi gio rat linh hoat.",
        "Co day du bong va ao bib cho doi minh.",
        "Khu cho doi sach se, phu hop di cung gia dinh.",
        "Mat co on dinh, khong bi tron khi troi am.",
        "Nhan vien check-in dung gio, khong phai cho lau.",
        "Can cai thien them bang diem dien tu.",
        "Gia cuoi tuan hoi cao nhung dich vu tot.",
        "San phu hop da giao huu cong ty."
    ];

    private sealed record UserSpec(Guid Id, string FullName, string Email, string Phone, string Role);

    private sealed record SeedUsers(Guid[] Admins, Guid[] Owners, Guid[] Customers);
}
