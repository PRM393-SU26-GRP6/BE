using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace CourtManager.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Amenities",
                columns: table => new
                {
                    AmenityId = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Icon = table.Column<string>(type: "text", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Amenities", x => x.AmenityId);
                });

            migrationBuilder.CreateTable(
                name: "AspNetRoles",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    NormalizedName = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUsers",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserName = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    NormalizedUserName = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    Email = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    NormalizedEmail = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    EmailConfirmed = table.Column<bool>(type: "boolean", nullable: false),
                    PasswordHash = table.Column<string>(type: "text", nullable: true),
                    SecurityStamp = table.Column<string>(type: "text", nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "text", nullable: true),
                    PhoneNumber = table.Column<string>(type: "text", nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(type: "boolean", nullable: false),
                    TwoFactorEnabled = table.Column<bool>(type: "boolean", nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    LockoutEnabled = table.Column<bool>(type: "boolean", nullable: false),
                    AccessFailedCount = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUsers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Roles",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    NormalizedName = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    Description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Roles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserName = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    Email = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    PhoneNumber = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    FullName = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Phone = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    AvatarUrl = table.Column<string>(type: "text", nullable: true),
                    LoyaltyPoints = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    RefreshToken = table.Column<string>(type: "text", nullable: true),
                    RefreshTokenExpiryTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "RoleClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    RoleId = table.Column<Guid>(type: "uuid", nullable: false),
                    ClaimType = table.Column<string>(type: "text", nullable: true),
                    ClaimValue = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RoleClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RoleClaims_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserRoles",
                columns: table => new
                {
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    RoleId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserRoles", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    ClaimType = table.Column<string>(type: "text", nullable: true),
                    ClaimValue = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserClaims_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserLogins",
                columns: table => new
                {
                    LoginProvider = table.Column<string>(type: "text", nullable: false),
                    ProviderKey = table.Column<string>(type: "text", nullable: false),
                    ProviderDisplayName = table.Column<string>(type: "text", nullable: true),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserLogins", x => new { x.LoginProvider, x.ProviderKey });
                    table.ForeignKey(
                        name: "FK_UserLogins_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserTokens",
                columns: table => new
                {
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    LoginProvider = table.Column<string>(type: "text", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Value = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserTokens", x => new { x.UserId, x.LoginProvider, x.Name });
                    table.ForeignKey(
                        name: "FK_UserTokens_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ChatRooms",
                columns: table => new
                {
                    RoomId = table.Column<Guid>(type: "uuid", nullable: false),
                    CustomerId = table.Column<Guid>(type: "uuid", nullable: false),
                    HostId = table.Column<Guid>(type: "uuid", nullable: false),
                    LastMessageAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChatRooms", x => x.RoomId);
                    table.ForeignKey(
                        name: "FK_ChatRooms_Users_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ChatRooms_Users_HostId",
                        column: x => x.HostId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Notifications",
                columns: table => new
                {
                    NotificationId = table.Column<Guid>(type: "uuid", nullable: false),
                    Title = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Message = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: false),
                    Type = table.Column<int>(type: "integer", nullable: false),
                    RefId = table.Column<string>(type: "text", nullable: false),
                    SenderId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    UserId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Notifications", x => x.NotificationId);
                    table.ForeignKey(
                        name: "FK_Notifications_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "UserDevices",
                columns: table => new
                {
                    DeviceId = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    FcmToken = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    DeviceType = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserDevices", x => x.DeviceId);
                    table.ForeignKey(
                        name: "FK_UserDevices_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserRoles",
                columns: table => new
                {
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    RoleId = table.Column<Guid>(type: "uuid", nullable: false),
                    AssignedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserRoles", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_UserRoles_Roles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "Roles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserRoles_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Venues",
                columns: table => new
                {
                    VenueId = table.Column<Guid>(type: "uuid", nullable: false),
                    OwnerId = table.Column<Guid>(type: "uuid", nullable: false),
                    VenueName = table.Column<string>(type: "text", nullable: false),
                    Address = table.Column<string>(type: "text", nullable: false),
                    Latitude = table.Column<decimal>(type: "numeric", nullable: false),
                    Longitude = table.Column<decimal>(type: "numeric", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: false),
                    OpeningHours = table.Column<string>(type: "text", nullable: false),
                    PhoneContact = table.Column<string>(type: "text", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Venues", x => x.VenueId);
                    table.ForeignKey(
                        name: "FK_Venues_Users_OwnerId",
                        column: x => x.OwnerId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Messages",
                columns: table => new
                {
                    MessageId = table.Column<Guid>(type: "uuid", nullable: false),
                    RoomId = table.Column<Guid>(type: "uuid", nullable: false),
                    SenderId = table.Column<Guid>(type: "uuid", nullable: false),
                    MessageText = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: false),
                    ReadAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    SentAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Messages", x => x.MessageId);
                    table.ForeignKey(
                        name: "FK_Messages_ChatRooms_RoomId",
                        column: x => x.RoomId,
                        principalTable: "ChatRooms",
                        principalColumn: "RoomId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Messages_Users_SenderId",
                        column: x => x.SenderId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "NotificationRecipients",
                columns: table => new
                {
                    RecipientId = table.Column<Guid>(type: "uuid", nullable: false),
                    NotificationId = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    ReadAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NotificationRecipients", x => x.RecipientId);
                    table.ForeignKey(
                        name: "FK_NotificationRecipients_Notifications_NotificationId",
                        column: x => x.NotificationId,
                        principalTable: "Notifications",
                        principalColumn: "NotificationId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_NotificationRecipients_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Discounts",
                columns: table => new
                {
                    DiscountId = table.Column<Guid>(type: "uuid", nullable: false),
                    OwnerId = table.Column<Guid>(type: "uuid", nullable: false),
                    VenueId = table.Column<Guid>(type: "uuid", nullable: true),
                    Code = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    DiscountType = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Value = table.Column<decimal>(type: "numeric(10,2)", precision: 10, scale: 2, nullable: false),
                    MinBookingAmount = table.Column<decimal>(type: "numeric(10,2)", precision: 10, scale: 2, nullable: false),
                    MaxDiscountAmount = table.Column<decimal>(type: "numeric(10,2)", precision: 10, scale: 2, nullable: false),
                    UsageLimit = table.Column<int>(type: "integer", nullable: false),
                    UsedCount = table.Column<int>(type: "integer", nullable: false),
                    StartDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    EndDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Discounts", x => x.DiscountId);
                    table.ForeignKey(
                        name: "FK_Discounts_Users_OwnerId",
                        column: x => x.OwnerId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Discounts_Venues_VenueId",
                        column: x => x.VenueId,
                        principalTable: "Venues",
                        principalColumn: "VenueId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "FootballFields",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    VenueId = table.Column<Guid>(type: "uuid", nullable: false),
                    FieldName = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: false),
                    FieldType = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    PricePerHour = table.Column<decimal>(type: "numeric(10,2)", precision: 10, scale: 2, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FootballFields", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FootballFields_Venues_VenueId",
                        column: x => x.VenueId,
                        principalTable: "Venues",
                        principalColumn: "VenueId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "VenueAmenities",
                columns: table => new
                {
                    VenueId = table.Column<Guid>(type: "uuid", nullable: false),
                    AmenityId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VenueAmenities", x => new { x.VenueId, x.AmenityId });
                    table.ForeignKey(
                        name: "FK_VenueAmenities_Amenities_AmenityId",
                        column: x => x.AmenityId,
                        principalTable: "Amenities",
                        principalColumn: "AmenityId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_VenueAmenities_Venues_VenueId",
                        column: x => x.VenueId,
                        principalTable: "Venues",
                        principalColumn: "VenueId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "VenueImages",
                columns: table => new
                {
                    ImageId = table.Column<Guid>(type: "uuid", nullable: false),
                    VenueId = table.Column<Guid>(type: "uuid", nullable: false),
                    ImageUrl = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    IsPrimary = table.Column<bool>(type: "boolean", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VenueImages", x => x.ImageId);
                    table.ForeignKey(
                        name: "FK_VenueImages_Venues_VenueId",
                        column: x => x.VenueId,
                        principalTable: "Venues",
                        principalColumn: "VenueId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Bookings",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    TotalPrice = table.Column<decimal>(type: "numeric(10,2)", precision: 10, scale: 2, nullable: false),
                    DepositAmount = table.Column<decimal>(type: "numeric", nullable: false),
                    BookingStatus = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false, defaultValue: "Pending"),
                    Note = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    FootballFieldId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Bookings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Bookings_FootballFields_FootballFieldId",
                        column: x => x.FootballFieldId,
                        principalTable: "FootballFields",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Bookings_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TimeSlots",
                columns: table => new
                {
                    SlotId = table.Column<Guid>(type: "uuid", nullable: false),
                    FieldId = table.Column<Guid>(type: "uuid", nullable: false),
                    StartTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    EndTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Price = table.Column<decimal>(type: "numeric", nullable: false),
                    SlotStatus = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false, defaultValue: "Available"),
                    LockedUntil = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    LockedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TimeSlots", x => x.SlotId);
                    table.ForeignKey(
                        name: "FK_TimeSlots_FootballFields_FieldId",
                        column: x => x.FieldId,
                        principalTable: "FootballFields",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TimeSlots_Users_LockedBy",
                        column: x => x.LockedBy,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "BookingDiscounts",
                columns: table => new
                {
                    BookingId = table.Column<Guid>(type: "uuid", nullable: false),
                    DiscountId = table.Column<Guid>(type: "uuid", nullable: false),
                    DiscountAmount = table.Column<decimal>(type: "numeric", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BookingDiscounts", x => new { x.BookingId, x.DiscountId });
                    table.ForeignKey(
                        name: "FK_BookingDiscounts_Bookings_BookingId",
                        column: x => x.BookingId,
                        principalTable: "Bookings",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BookingDiscounts_Discounts_DiscountId",
                        column: x => x.DiscountId,
                        principalTable: "Discounts",
                        principalColumn: "DiscountId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Payments",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    BookingId = table.Column<Guid>(type: "uuid", nullable: false),
                    Amount = table.Column<decimal>(type: "numeric(10,2)", precision: 10, scale: 2, nullable: false),
                    PaymentType = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    PaymentMethod = table.Column<int>(type: "integer", nullable: false),
                    PaymentStatus = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false, defaultValue: "Pending"),
                    TransactionCode = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Gateway = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    GatewayTransactionId = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    GatewayReferenceCode = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    GatewayAccountNumber = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    GatewayRawContent = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    PaidAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    RefundAmount = table.Column<decimal>(type: "numeric(10,2)", precision: 10, scale: 2, nullable: true),
                    RefundReason = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Payments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Payments_Bookings_BookingId",
                        column: x => x.BookingId,
                        principalTable: "Bookings",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Reviews",
                columns: table => new
                {
                    ReviewId = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    VenueId = table.Column<Guid>(type: "uuid", nullable: false),
                    BookingId = table.Column<Guid>(type: "uuid", nullable: false),
                    Rating = table.Column<int>(type: "integer", nullable: false),
                    Comment = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Reviews", x => x.ReviewId);
                    table.ForeignKey(
                        name: "FK_Reviews_Bookings_BookingId",
                        column: x => x.BookingId,
                        principalTable: "Bookings",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Reviews_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Reviews_Venues_VenueId",
                        column: x => x.VenueId,
                        principalTable: "Venues",
                        principalColumn: "VenueId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "BookingItems",
                columns: table => new
                {
                    BookingItemId = table.Column<Guid>(type: "uuid", nullable: false),
                    BookingId = table.Column<Guid>(type: "uuid", nullable: false),
                    SlotId = table.Column<Guid>(type: "uuid", nullable: false),
                    Price = table.Column<decimal>(type: "numeric(10,2)", precision: 10, scale: 2, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BookingItems", x => x.BookingItemId);
                    table.ForeignKey(
                        name: "FK_BookingItems_Bookings_BookingId",
                        column: x => x.BookingId,
                        principalTable: "Bookings",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BookingItems_TimeSlots_SlotId",
                        column: x => x.SlotId,
                        principalTable: "TimeSlots",
                        principalColumn: "SlotId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.InsertData(
                table: "Amenities",
                columns: new[] { "AmenityId", "DeletedAt", "Icon", "IsDeleted", "Name" },
                values: new object[,]
                {
                    { new Guid("23c9f58d-b578-e552-872f-c81516bbbb45"), null, "table", false, "Scoreboard" },
                    { new Guid("6215b4d4-8e66-845d-ad2f-53d4f1d3cbeb"), null, "wifi", false, "Free wifi" },
                    { new Guid("72951c7c-043d-1c5c-a39a-e299eaece521"), null, "cross", false, "First aid kit" },
                    { new Guid("74d70a98-b403-3255-a580-98eda5f581fc"), null, "droplets", false, "Drinking water" },
                    { new Guid("8077e70b-71ae-925c-87b1-707df6b54bc0"), null, "parking", false, "Covered parking" },
                    { new Guid("81815209-f0e6-f752-a864-5236f3aafd94"), null, "coffee", false, "Cafe lounge" },
                    { new Guid("8f3b4b0d-4710-9059-8a85-c256548f118c"), null, "shirt", false, "Changing room" },
                    { new Guid("99335921-22b7-3b51-8340-e756ae4c4930"), null, "shower-head", false, "Shower area" },
                    { new Guid("a745c0e2-627f-085e-af23-316c1a756cbe"), null, "bike", false, "Bike parking" },
                    { new Guid("b8358615-9686-2d50-8067-ba459980f291"), null, "lock", false, "Security locker" },
                    { new Guid("bd8a547b-4ea6-065e-84eb-082bb11c0ce5"), null, "package", false, "Equipment rental" },
                    { new Guid("f5a935e4-be7d-f95f-953e-298059f47da2"), null, "lightbulb", false, "Night lighting" }
                });

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { new Guid("07371171-eec1-3255-b1b2-1d8e8e81ede7"), "07371171-eec1-3255-b1b2-1d8e8e81ede7", "Owner", "OWNER" },
                    { new Guid("76075424-3dac-6259-a0f7-00a4c6c20191"), "76075424-3dac-6259-a0f7-00a4c6c20191", "User", "USER" },
                    { new Guid("b5abbaf1-931c-5353-b9ab-1f38eb30b8b8"), "b5abbaf1-931c-5353-b9ab-1f38eb30b8b8", "Admin", "ADMIN" }
                });

            migrationBuilder.InsertData(
                table: "AspNetUsers",
                columns: new[] { "Id", "AccessFailedCount", "ConcurrencyStamp", "Email", "EmailConfirmed", "LockoutEnabled", "LockoutEnd", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "PhoneNumber", "PhoneNumberConfirmed", "SecurityStamp", "TwoFactorEnabled", "UserName" },
                values: new object[,]
                {
                    { new Guid("28d446ef-917b-8b59-a814-da2a00b0b76f"), 0, "28d446ef-917b-8b59-a814-da2a00b0b76f", "andang.football@gmail.com", true, false, null, "ANDANG.FOOTBALL@GMAIL.COM", "ANDANG.FOOTBALL@GMAIL.COM", "AQAAAAIAAYagAAAAEMhNOhWJhrehCy84iiKMjD+gAwmKtd2V+CHm4EhzxmaTyXKW9OS5bmKjoFGKqWDFAg==", "0902311007", true, "28d446ef-917b-8b59-a814-da2a00b0b76f", false, "andang.football@gmail.com" },
                    { new Guid("2f958e63-14a1-ee5f-b359-e923bbd70ece"), 0, "2f958e63-14a1-ee5f-b359-e923bbd70ece", "hanh.le@saigonfields.vn", true, false, null, "HANH.LE@SAIGONFIELDS.VN", "HANH.LE@SAIGONFIELDS.VN", "AQAAAAIAAYagAAAAEMhNOhWJhrehCy84iiKMjD+gAwmKtd2V+CHm4EhzxmaTyXKW9OS5bmKjoFGKqWDFAg==", "0902311004", true, "2f958e63-14a1-ee5f-b359-e923bbd70ece", false, "hanh.le@saigonfields.vn" },
                    { new Guid("49f942ec-d197-7c5c-a011-6454ca64ec2c"), 0, "49f942ec-d197-7c5c-a011-6454ca64ec2c", "bao.hoang@cityarena.vn", true, false, null, "BAO.HOANG@CITYARENA.VN", "BAO.HOANG@CITYARENA.VN", "AQAAAAIAAYagAAAAEMhNOhWJhrehCy84iiKMjD+gAwmKtd2V+CHm4EhzxmaTyXKW9OS5bmKjoFGKqWDFAg==", "0902311006", true, "49f942ec-d197-7c5c-a011-6454ca64ec2c", false, "bao.hoang@cityarena.vn" },
                    { new Guid("81d10681-e36e-595b-972a-f441c8237537"), 0, "81d10681-e36e-595b-972a-f441c8237537", "linhhuynh.club@gmail.com", true, false, null, "LINHHUYNH.CLUB@GMAIL.COM", "LINHHUYNH.CLUB@GMAIL.COM", "AQAAAAIAAYagAAAAEMhNOhWJhrehCy84iiKMjD+gAwmKtd2V+CHm4EhzxmaTyXKW9OS5bmKjoFGKqWDFAg==", "0902311012", true, "81d10681-e36e-595b-972a-f441c8237537", false, "linhhuynh.club@gmail.com" },
                    { new Guid("b41aae5d-9596-9a5d-b8e5-0f8b199a8135"), 0, "b41aae5d-9596-9a5d-b8e5-0f8b199a8135", "lan.nguyen@courtmanager.vn", true, false, null, "LAN.NGUYEN@COURTMANAGER.VN", "LAN.NGUYEN@COURTMANAGER.VN", "AQAAAAIAAYagAAAAEMhNOhWJhrehCy84iiKMjD+gAwmKtd2V+CHm4EhzxmaTyXKW9OS5bmKjoFGKqWDFAg==", "0902311001", true, "b41aae5d-9596-9a5d-b8e5-0f8b199a8135", false, "lan.nguyen@courtmanager.vn" },
                    { new Guid("b53af497-39fc-6351-a424-0a0063d43116"), 0, "b53af497-39fc-6351-a424-0a0063d43116", "mypham.saigon@gmail.com", true, false, null, "MYPHAM.SAIGON@GMAIL.COM", "MYPHAM.SAIGON@GMAIL.COM", "AQAAAAIAAYagAAAAEMhNOhWJhrehCy84iiKMjD+gAwmKtd2V+CHm4EhzxmaTyXKW9OS5bmKjoFGKqWDFAg==", "0902311008", true, "b53af497-39fc-6351-a424-0a0063d43116", false, "mypham.saigon@gmail.com" },
                    { new Guid("cbfe125b-7a8c-335c-aa61-df49f35c448f"), 0, "cbfe125b-7a8c-335c-aa61-df49f35c448f", "khoabui.runner@outlook.com", true, false, null, "KHOABUI.RUNNER@OUTLOOK.COM", "KHOABUI.RUNNER@OUTLOOK.COM", "AQAAAAIAAYagAAAAEMhNOhWJhrehCy84iiKMjD+gAwmKtd2V+CHm4EhzxmaTyXKW9OS5bmKjoFGKqWDFAg==", "0902311009", true, "cbfe125b-7a8c-335c-aa61-df49f35c448f", false, "khoabui.runner@outlook.com" },
                    { new Guid("de68f3de-ceab-c85f-b54a-645613f6a13e"), 0, "de68f3de-ceab-c85f-b54a-645613f6a13e", "thaodo.booking@gmail.com", true, false, null, "THAODO.BOOKING@GMAIL.COM", "THAODO.BOOKING@GMAIL.COM", "AQAAAAIAAYagAAAAEMhNOhWJhrehCy84iiKMjD+gAwmKtd2V+CHm4EhzxmaTyXKW9OS5bmKjoFGKqWDFAg==", "0902311010", true, "de68f3de-ceab-c85f-b54a-645613f6a13e", false, "thaodo.booking@gmail.com" },
                    { new Guid("e3266388-5d3f-c459-beef-1edc2d465a3e"), 0, "e3266388-5d3f-c459-beef-1edc2d465a3e", "minh.tran@courtmanager.vn", true, false, null, "MINH.TRAN@COURTMANAGER.VN", "MINH.TRAN@COURTMANAGER.VN", "AQAAAAIAAYagAAAAEMhNOhWJhrehCy84iiKMjD+gAwmKtd2V+CHm4EhzxmaTyXKW9OS5bmKjoFGKqWDFAg==", "0902311002", true, "e3266388-5d3f-c459-beef-1edc2d465a3e", false, "minh.tran@courtmanager.vn" },
                    { new Guid("ef0c12c5-0bcf-4e5f-a13a-4b01b2ed44fc"), 0, "ef0c12c5-0bcf-4e5f-a13a-4b01b2ed44fc", "duy.pham@sporthub.vn", true, false, null, "DUY.PHAM@SPORTHUB.VN", "DUY.PHAM@SPORTHUB.VN", "AQAAAAIAAYagAAAAEMhNOhWJhrehCy84iiKMjD+gAwmKtd2V+CHm4EhzxmaTyXKW9OS5bmKjoFGKqWDFAg==", "0902311003", true, "ef0c12c5-0bcf-4e5f-a13a-4b01b2ed44fc", false, "duy.pham@sporthub.vn" },
                    { new Guid("eff1cca4-9f7a-0f53-a3e0-115f934fc55b"), 0, "eff1cca4-9f7a-0f53-a3e0-115f934fc55b", "tuanmai.sports@yahoo.com", true, false, null, "TUANMAI.SPORTS@YAHOO.COM", "TUANMAI.SPORTS@YAHOO.COM", "AQAAAAIAAYagAAAAEMhNOhWJhrehCy84iiKMjD+gAwmKtd2V+CHm4EhzxmaTyXKW9OS5bmKjoFGKqWDFAg==", "0902311011", true, "eff1cca4-9f7a-0f53-a3e0-115f934fc55b", false, "tuanmai.sports@yahoo.com" },
                    { new Guid("ff182b52-5005-895d-a90a-224ef11c5e61"), 0, "ff182b52-5005-895d-a90a-224ef11c5e61", "quang.vo@greenpitch.vn", true, false, null, "QUANG.VO@GREENPITCH.VN", "QUANG.VO@GREENPITCH.VN", "AQAAAAIAAYagAAAAEMhNOhWJhrehCy84iiKMjD+gAwmKtd2V+CHm4EhzxmaTyXKW9OS5bmKjoFGKqWDFAg==", "0902311005", true, "ff182b52-5005-895d-a90a-224ef11c5e61", false, "quang.vo@greenpitch.vn" }
                });

            migrationBuilder.InsertData(
                table: "Notifications",
                columns: new[] { "NotificationId", "CreatedAt", "DeletedAt", "IsDeleted", "Message", "RefId", "SenderId", "Title", "Type", "UserId" },
                values: new object[,]
                {
                    { new Guid("16eb01d8-478c-1052-a848-5b5d331a27e3"), new DateTime(2026, 5, 29, 0, 0, 0, 0, DateTimeKind.Utc), null, false, "The venue manager could not accept the requested slot.", "688cad1e-f0d6-1b5c-add2-059e8ee912b2", new Guid("2f958e63-14a1-ee5f-b359-e923bbd70ece"), "Booking rejected", 0, null },
                    { new Guid("248e767b-fbd1-765e-a621-af5c5c5d17f4"), new DateTime(2026, 5, 28, 0, 0, 0, 0, DateTimeKind.Utc), null, false, "You have a new message about your booking.", "f05fe560-3108-5857-ad8a-7c9005cf0dba", new Guid("28d446ef-917b-8b59-a814-da2a00b0b76f"), "New chat message", 1, null },
                    { new Guid("4c77ec9c-2b1a-dc5b-8a29-eabe13fc63a3"), new DateTime(2026, 6, 3, 0, 0, 0, 0, DateTimeKind.Utc), null, false, "A customer has submitted a new booking request.", "682ba49f-52d0-7f51-934f-f573b2c6e822", new Guid("28d446ef-917b-8b59-a814-da2a00b0b76f"), "Booking request received", 0, null },
                    { new Guid("4fc8ee4a-65d5-d551-a569-fe7cff7fdb69"), new DateTime(2026, 5, 31, 0, 0, 0, 0, DateTimeKind.Utc), null, false, "Your booking has been fully paid.", "9e03adc8-fc50-e257-b572-26ece917a5b7", new Guid("49f942ec-d197-7c5c-a011-6454ca64ec2c"), "Final payment completed", 3, null },
                    { new Guid("7b322e08-b6c7-9051-accc-f23db0876077"), new DateTime(2026, 5, 30, 0, 0, 0, 0, DateTimeKind.Utc), null, false, "A booking was cancelled before the match time.", "e628c15a-2980-635d-bbfb-55eac35bf6f6", new Guid("eff1cca4-9f7a-0f53-a3e0-115f934fc55b"), "Booking cancelled", 4, null },
                    { new Guid("8026f84c-ed63-de5c-9b00-f9ad4ecba28a"), new DateTime(2026, 5, 25, 0, 0, 0, 0, DateTimeKind.Utc), null, false, "A payment is waiting for bank confirmation.", "60eed09f-32b4-c65a-9b85-fa0dcaac75b8", new Guid("2f958e63-14a1-ee5f-b359-e923bbd70ece"), "Payment pending", 4, null },
                    { new Guid("8a401056-dda9-765c-a486-ca31419762f1"), new DateTime(2026, 6, 2, 0, 0, 0, 0, DateTimeKind.Utc), null, false, "Your booking has been accepted by the venue manager.", "75ab8d9a-f5ab-9d59-9857-e77407addcd8", new Guid("2f958e63-14a1-ee5f-b359-e923bbd70ece"), "Booking accepted", 1, null },
                    { new Guid("97035cd2-8716-6451-9bac-10e034a00ed9"), new DateTime(2026, 5, 27, 0, 0, 0, 0, DateTimeKind.Utc), null, false, "A discount code was applied to a booking.", "3ced2bd5-38e9-8b5a-8fc8-4429391d9e0c", new Guid("49f942ec-d197-7c5c-a011-6454ca64ec2c"), "Discount applied", 2, null },
                    { new Guid("a39a0ace-4fc8-585f-92f5-cfd19f98c6ae"), new DateTime(2026, 5, 23, 0, 0, 0, 0, DateTimeKind.Utc), null, false, "System maintenance is scheduled after midnight.", "1a4b8f37-e3c9-4357-99a8-b997bebeb529", new Guid("49f942ec-d197-7c5c-a011-6454ca64ec2c"), "System maintenance notice", 1, null },
                    { new Guid("ca73a25b-b382-1054-b57c-84ea963d9eaf"), new DateTime(2026, 6, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, false, "Your deposit payment has been confirmed.", "e909180c-8cea-915e-b9c0-47520fe4a6ad", new Guid("cbfe125b-7a8c-335c-aa61-df49f35c448f"), "Deposit confirmed", 2, null },
                    { new Guid("d5c82e74-5b49-d554-a368-ee5e14ada28a"), new DateTime(2026, 5, 24, 0, 0, 0, 0, DateTimeKind.Utc), null, false, "A customer submitted a venue review.", "523e651a-d316-495b-9e88-614fc24c402e", new Guid("eff1cca4-9f7a-0f53-a3e0-115f934fc55b"), "Review submitted", 0, null },
                    { new Guid("f1b6d9ba-bf7a-665f-aa99-bd45e5b4eda3"), new DateTime(2026, 5, 26, 0, 0, 0, 0, DateTimeKind.Utc), null, false, "One of your selected slots changed status.", "33f8c662-d9af-335d-b547-7e116e8b4d74", new Guid("cbfe125b-7a8c-335c-aa61-df49f35c448f"), "Slot status updated", 3, null }
                });

            migrationBuilder.InsertData(
                table: "Roles",
                columns: new[] { "Id", "CreatedAt", "Description", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { new Guid("07371171-eec1-3255-b1b2-1d8e8e81ede7"), new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Venue and field owner", "Owner", "OWNER" },
                    { new Guid("76075424-3dac-6259-a0f7-00a4c6c20191"), new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Regular booking user", "User", "USER" },
                    { new Guid("b5abbaf1-931c-5353-b9ab-1f38eb30b8b8"), new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Administrator with full access", "Admin", "ADMIN" }
                });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "AvatarUrl", "CreatedAt", "DeletedAt", "Email", "FullName", "IsActive", "IsDeleted", "LoyaltyPoints", "Phone", "PhoneNumber", "RefreshToken", "RefreshTokenExpiryTime", "UpdatedAt", "UserName" },
                values: new object[] { new Guid("28d446ef-917b-8b59-a814-da2a00b0b76f"), null, new DateTime(2026, 1, 5, 0, 0, 0, 0, DateTimeKind.Utc), null, "andang.football@gmail.com", "An Dang", true, false, 120, "0902311007", "0902311007", null, null, null, "andang.football@gmail.com" });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "AvatarUrl", "CreatedAt", "DeletedAt", "Email", "FullName", "IsActive", "IsDeleted", "Phone", "PhoneNumber", "RefreshToken", "RefreshTokenExpiryTime", "UpdatedAt", "UserName" },
                values: new object[,]
                {
                    { new Guid("2f958e63-14a1-ee5f-b359-e923bbd70ece"), null, new DateTime(2026, 1, 5, 0, 0, 0, 0, DateTimeKind.Utc), null, "hanh.le@saigonfields.vn", "Hanh Le", true, false, "0902311004", "0902311004", null, null, null, "hanh.le@saigonfields.vn" },
                    { new Guid("49f942ec-d197-7c5c-a011-6454ca64ec2c"), null, new DateTime(2026, 1, 5, 0, 0, 0, 0, DateTimeKind.Utc), null, "bao.hoang@cityarena.vn", "Bao Hoang", true, false, "0902311006", "0902311006", null, null, null, "bao.hoang@cityarena.vn" }
                });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "AvatarUrl", "CreatedAt", "DeletedAt", "Email", "FullName", "IsActive", "IsDeleted", "LoyaltyPoints", "Phone", "PhoneNumber", "RefreshToken", "RefreshTokenExpiryTime", "UpdatedAt", "UserName" },
                values: new object[] { new Guid("81d10681-e36e-595b-972a-f441c8237537"), null, new DateTime(2026, 1, 5, 0, 0, 0, 0, DateTimeKind.Utc), null, "linhhuynh.club@gmail.com", "Linh Huynh", true, false, 120, "0902311012", "0902311012", null, null, null, "linhhuynh.club@gmail.com" });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "AvatarUrl", "CreatedAt", "DeletedAt", "Email", "FullName", "IsActive", "IsDeleted", "Phone", "PhoneNumber", "RefreshToken", "RefreshTokenExpiryTime", "UpdatedAt", "UserName" },
                values: new object[] { new Guid("b41aae5d-9596-9a5d-b8e5-0f8b199a8135"), null, new DateTime(2026, 1, 5, 0, 0, 0, 0, DateTimeKind.Utc), null, "lan.nguyen@courtmanager.vn", "Lan Nguyen", true, false, "0902311001", "0902311001", null, null, null, "lan.nguyen@courtmanager.vn" });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "AvatarUrl", "CreatedAt", "DeletedAt", "Email", "FullName", "IsActive", "IsDeleted", "LoyaltyPoints", "Phone", "PhoneNumber", "RefreshToken", "RefreshTokenExpiryTime", "UpdatedAt", "UserName" },
                values: new object[,]
                {
                    { new Guid("b53af497-39fc-6351-a424-0a0063d43116"), null, new DateTime(2026, 1, 5, 0, 0, 0, 0, DateTimeKind.Utc), null, "mypham.saigon@gmail.com", "My Pham", true, false, 120, "0902311008", "0902311008", null, null, null, "mypham.saigon@gmail.com" },
                    { new Guid("cbfe125b-7a8c-335c-aa61-df49f35c448f"), null, new DateTime(2026, 1, 5, 0, 0, 0, 0, DateTimeKind.Utc), null, "khoabui.runner@outlook.com", "Khoa Bui", true, false, 120, "0902311009", "0902311009", null, null, null, "khoabui.runner@outlook.com" },
                    { new Guid("de68f3de-ceab-c85f-b54a-645613f6a13e"), null, new DateTime(2026, 1, 5, 0, 0, 0, 0, DateTimeKind.Utc), null, "thaodo.booking@gmail.com", "Thao Do", true, false, 120, "0902311010", "0902311010", null, null, null, "thaodo.booking@gmail.com" }
                });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "AvatarUrl", "CreatedAt", "DeletedAt", "Email", "FullName", "IsActive", "IsDeleted", "Phone", "PhoneNumber", "RefreshToken", "RefreshTokenExpiryTime", "UpdatedAt", "UserName" },
                values: new object[,]
                {
                    { new Guid("e3266388-5d3f-c459-beef-1edc2d465a3e"), null, new DateTime(2026, 1, 5, 0, 0, 0, 0, DateTimeKind.Utc), null, "minh.tran@courtmanager.vn", "Minh Tran", true, false, "0902311002", "0902311002", null, null, null, "minh.tran@courtmanager.vn" },
                    { new Guid("ef0c12c5-0bcf-4e5f-a13a-4b01b2ed44fc"), null, new DateTime(2026, 1, 5, 0, 0, 0, 0, DateTimeKind.Utc), null, "duy.pham@sporthub.vn", "Duy Pham", true, false, "0902311003", "0902311003", null, null, null, "duy.pham@sporthub.vn" }
                });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "AvatarUrl", "CreatedAt", "DeletedAt", "Email", "FullName", "IsActive", "IsDeleted", "LoyaltyPoints", "Phone", "PhoneNumber", "RefreshToken", "RefreshTokenExpiryTime", "UpdatedAt", "UserName" },
                values: new object[] { new Guid("eff1cca4-9f7a-0f53-a3e0-115f934fc55b"), null, new DateTime(2026, 1, 5, 0, 0, 0, 0, DateTimeKind.Utc), null, "tuanmai.sports@yahoo.com", "Tuan Mai", true, false, 120, "0902311011", "0902311011", null, null, null, "tuanmai.sports@yahoo.com" });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "AvatarUrl", "CreatedAt", "DeletedAt", "Email", "FullName", "IsActive", "IsDeleted", "Phone", "PhoneNumber", "RefreshToken", "RefreshTokenExpiryTime", "UpdatedAt", "UserName" },
                values: new object[] { new Guid("ff182b52-5005-895d-a90a-224ef11c5e61"), null, new DateTime(2026, 1, 5, 0, 0, 0, 0, DateTimeKind.Utc), null, "quang.vo@greenpitch.vn", "Quang Vo", true, false, "0902311005", "0902311005", null, null, null, "quang.vo@greenpitch.vn" });

            migrationBuilder.InsertData(
                table: "AspNetUserRoles",
                columns: new[] { "RoleId", "UserId" },
                values: new object[,]
                {
                    { new Guid("76075424-3dac-6259-a0f7-00a4c6c20191"), new Guid("28d446ef-917b-8b59-a814-da2a00b0b76f") },
                    { new Guid("07371171-eec1-3255-b1b2-1d8e8e81ede7"), new Guid("2f958e63-14a1-ee5f-b359-e923bbd70ece") },
                    { new Guid("07371171-eec1-3255-b1b2-1d8e8e81ede7"), new Guid("49f942ec-d197-7c5c-a011-6454ca64ec2c") },
                    { new Guid("76075424-3dac-6259-a0f7-00a4c6c20191"), new Guid("81d10681-e36e-595b-972a-f441c8237537") },
                    { new Guid("b5abbaf1-931c-5353-b9ab-1f38eb30b8b8"), new Guid("b41aae5d-9596-9a5d-b8e5-0f8b199a8135") },
                    { new Guid("76075424-3dac-6259-a0f7-00a4c6c20191"), new Guid("b53af497-39fc-6351-a424-0a0063d43116") },
                    { new Guid("76075424-3dac-6259-a0f7-00a4c6c20191"), new Guid("cbfe125b-7a8c-335c-aa61-df49f35c448f") },
                    { new Guid("76075424-3dac-6259-a0f7-00a4c6c20191"), new Guid("de68f3de-ceab-c85f-b54a-645613f6a13e") },
                    { new Guid("b5abbaf1-931c-5353-b9ab-1f38eb30b8b8"), new Guid("e3266388-5d3f-c459-beef-1edc2d465a3e") },
                    { new Guid("07371171-eec1-3255-b1b2-1d8e8e81ede7"), new Guid("ef0c12c5-0bcf-4e5f-a13a-4b01b2ed44fc") },
                    { new Guid("76075424-3dac-6259-a0f7-00a4c6c20191"), new Guid("eff1cca4-9f7a-0f53-a3e0-115f934fc55b") },
                    { new Guid("07371171-eec1-3255-b1b2-1d8e8e81ede7"), new Guid("ff182b52-5005-895d-a90a-224ef11c5e61") }
                });

            migrationBuilder.InsertData(
                table: "Bookings",
                columns: new[] { "Id", "BookingStatus", "CreatedAt", "DeletedAt", "DepositAmount", "FootballFieldId", "IsDeleted", "Note", "TotalPrice", "UpdatedAt", "UserId" },
                values: new object[,]
                {
                    { new Guid("1a4b8f37-e3c9-4357-99a8-b997bebeb529"), "Cancelled", new DateTime(2026, 5, 25, 0, 0, 0, 0, DateTimeKind.Utc), null, 147500m, null, false, "Schedule changed by customer.", 295000m, new DateTime(2026, 5, 25, 3, 0, 0, 0, DateTimeKind.Utc), new Guid("81d10681-e36e-595b-972a-f441c8237537") },
                    { new Guid("33f8c662-d9af-335d-b547-7e116e8b4d74"), "Accepted", new DateTime(2026, 6, 2, 0, 0, 0, 0, DateTimeKind.Utc), null, 142500m, null, false, "Awaiting deposit payment.", 285000m, new DateTime(2026, 6, 2, 3, 0, 0, 0, DateTimeKind.Utc), new Guid("cbfe125b-7a8c-335c-aa61-df49f35c448f") },
                    { new Guid("3ced2bd5-38e9-8b5a-8fc8-4429391d9e0c"), "Completed", new DateTime(2026, 5, 27, 0, 0, 0, 0, DateTimeKind.Utc), null, 225000m, null, false, "Company friendly match.", 450000m, new DateTime(2026, 5, 27, 3, 0, 0, 0, DateTimeKind.Utc), new Guid("b53af497-39fc-6351-a424-0a0063d43116") },
                    { new Guid("523e651a-d316-495b-9e88-614fc24c402e"), "Completed", new DateTime(2026, 5, 26, 0, 0, 0, 0, DateTimeKind.Utc), null, 187500m, null, false, "Weekend tournament slot.", 375000m, new DateTime(2026, 5, 26, 3, 0, 0, 0, DateTimeKind.Utc), new Guid("eff1cca4-9f7a-0f53-a3e0-115f934fc55b") }
                });

            migrationBuilder.InsertData(
                table: "Bookings",
                columns: new[] { "Id", "CreatedAt", "DeletedAt", "DepositAmount", "FootballFieldId", "IsDeleted", "Note", "TotalPrice", "UpdatedAt", "UserId" },
                values: new object[,]
                {
                    { new Guid("60eed09f-32b4-c65a-9b85-fa0dcaac75b8"), new DateTime(2026, 6, 3, 4, 0, 0, 0, DateTimeKind.Utc), null, 155000m, null, false, "New booking request.", 310000m, null, new Guid("de68f3de-ceab-c85f-b54a-645613f6a13e") },
                    { new Guid("682ba49f-52d0-7f51-934f-f573b2c6e822"), new DateTime(2026, 5, 30, 0, 0, 0, 0, DateTimeKind.Utc), null, 175000m, null, false, "Waiting for owner confirmation.", 350000m, null, new Guid("28d446ef-917b-8b59-a814-da2a00b0b76f") }
                });

            migrationBuilder.InsertData(
                table: "Bookings",
                columns: new[] { "Id", "BookingStatus", "CreatedAt", "DeletedAt", "DepositAmount", "FootballFieldId", "IsDeleted", "Note", "TotalPrice", "UpdatedAt", "UserId" },
                values: new object[,]
                {
                    { new Guid("688cad1e-f0d6-1b5c-add2-059e8ee912b2"), "Rejected", new DateTime(2026, 5, 29, 0, 0, 0, 0, DateTimeKind.Utc), null, 205000m, null, false, "Venue rejected due to maintenance.", 410000m, new DateTime(2026, 5, 29, 3, 0, 0, 0, DateTimeKind.Utc), new Guid("81d10681-e36e-595b-972a-f441c8237537") },
                    { new Guid("75ab8d9a-f5ab-9d59-9857-e77407addcd8"), "Accepted", new DateTime(2026, 5, 31, 0, 0, 0, 0, DateTimeKind.Utc), null, 180000m, null, false, "Accepted by venue owner.", 360000m, new DateTime(2026, 5, 31, 3, 0, 0, 0, DateTimeKind.Utc), new Guid("b53af497-39fc-6351-a424-0a0063d43116") },
                    { new Guid("9e03adc8-fc50-e257-b572-26ece917a5b7"), "Completed", new DateTime(2026, 6, 2, 0, 0, 0, 0, DateTimeKind.Utc), null, 210000m, null, false, "Completed after final payment.", 420000m, new DateTime(2026, 6, 2, 3, 0, 0, 0, DateTimeKind.Utc), new Guid("de68f3de-ceab-c85f-b54a-645613f6a13e") },
                    { new Guid("e628c15a-2980-635d-bbfb-55eac35bf6f6"), "Cancelled", new DateTime(2026, 5, 28, 0, 0, 0, 0, DateTimeKind.Utc), null, 137500m, null, false, "Customer cancelled before deposit.", 275000m, new DateTime(2026, 5, 28, 3, 0, 0, 0, DateTimeKind.Utc), new Guid("eff1cca4-9f7a-0f53-a3e0-115f934fc55b") },
                    { new Guid("e909180c-8cea-915e-b9c0-47520fe4a6ad"), "Deposited", new DateTime(2026, 6, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, 165000m, null, false, "Deposit paid through SePay.", 330000m, new DateTime(2026, 6, 1, 3, 0, 0, 0, DateTimeKind.Utc), new Guid("cbfe125b-7a8c-335c-aa61-df49f35c448f") },
                    { new Guid("f05fe560-3108-5857-ad8a-7c9005cf0dba"), "Deposited", new DateTime(2026, 6, 3, 0, 0, 0, 0, DateTimeKind.Utc), null, 197500m, null, false, "Team league match.", 395000m, new DateTime(2026, 6, 3, 3, 0, 0, 0, DateTimeKind.Utc), new Guid("28d446ef-917b-8b59-a814-da2a00b0b76f") }
                });

            migrationBuilder.InsertData(
                table: "ChatRooms",
                columns: new[] { "RoomId", "CreatedAt", "CustomerId", "DeletedAt", "HostId", "IsDeleted", "LastMessageAt" },
                values: new object[,]
                {
                    { new Guid("17bb8f15-6089-905d-abba-576a0517cf95"), new DateTime(2026, 5, 26, 0, 0, 0, 0, DateTimeKind.Utc), new Guid("cbfe125b-7a8c-335c-aa61-df49f35c448f"), null, new Guid("ff182b52-5005-895d-a90a-224ef11c5e61"), false, new DateTime(2026, 5, 26, 0, 15, 0, 0, DateTimeKind.Utc) },
                    { new Guid("40fc32bb-e297-3e53-8885-3e7976cbbdc1"), new DateTime(2026, 6, 4, 0, 0, 0, 0, DateTimeKind.Utc), new Guid("81d10681-e36e-595b-972a-f441c8237537"), null, new Guid("49f942ec-d197-7c5c-a011-6454ca64ec2c"), false, new DateTime(2026, 6, 4, 0, 15, 0, 0, DateTimeKind.Utc) },
                    { new Guid("58d096b8-dcec-8351-a96d-6becd8df96c7"), new DateTime(2026, 6, 2, 0, 0, 0, 0, DateTimeKind.Utc), new Guid("de68f3de-ceab-c85f-b54a-645613f6a13e"), null, new Guid("2f958e63-14a1-ee5f-b359-e923bbd70ece"), false, new DateTime(2026, 6, 2, 0, 15, 0, 0, DateTimeKind.Utc) },
                    { new Guid("69ba1bf9-439b-de57-b77d-f7c0f97db207"), new DateTime(2026, 6, 3, 0, 0, 0, 0, DateTimeKind.Utc), new Guid("eff1cca4-9f7a-0f53-a3e0-115f934fc55b"), null, new Guid("ff182b52-5005-895d-a90a-224ef11c5e61"), false, new DateTime(2026, 6, 3, 0, 15, 0, 0, DateTimeKind.Utc) },
                    { new Guid("814a7c60-ac01-0f5b-bc59-7dee869d6bf2"), new DateTime(2026, 5, 31, 0, 0, 0, 0, DateTimeKind.Utc), new Guid("b53af497-39fc-6351-a424-0a0063d43116"), null, new Guid("49f942ec-d197-7c5c-a011-6454ca64ec2c"), false, new DateTime(2026, 5, 31, 0, 15, 0, 0, DateTimeKind.Utc) },
                    { new Guid("8f4c756e-90fb-7354-9748-91474d62a7e5"), new DateTime(2026, 5, 25, 0, 0, 0, 0, DateTimeKind.Utc), new Guid("b53af497-39fc-6351-a424-0a0063d43116"), null, new Guid("2f958e63-14a1-ee5f-b359-e923bbd70ece"), false, new DateTime(2026, 5, 25, 0, 15, 0, 0, DateTimeKind.Utc) },
                    { new Guid("94f42dce-592b-6d52-a51d-2b2b6d1a5865"), new DateTime(2026, 6, 1, 0, 0, 0, 0, DateTimeKind.Utc), new Guid("cbfe125b-7a8c-335c-aa61-df49f35c448f"), null, new Guid("ef0c12c5-0bcf-4e5f-a13a-4b01b2ed44fc"), false, new DateTime(2026, 6, 1, 0, 15, 0, 0, DateTimeKind.Utc) },
                    { new Guid("bea19fa6-d965-0e52-8eb6-b0cabc857c85"), new DateTime(2026, 5, 29, 0, 0, 0, 0, DateTimeKind.Utc), new Guid("81d10681-e36e-595b-972a-f441c8237537"), null, new Guid("2f958e63-14a1-ee5f-b359-e923bbd70ece"), false, new DateTime(2026, 5, 29, 0, 15, 0, 0, DateTimeKind.Utc) },
                    { new Guid("e5c97dc5-a377-ef58-b975-db03712736cb"), new DateTime(2026, 5, 28, 0, 0, 0, 0, DateTimeKind.Utc), new Guid("eff1cca4-9f7a-0f53-a3e0-115f934fc55b"), null, new Guid("ef0c12c5-0bcf-4e5f-a13a-4b01b2ed44fc"), false, new DateTime(2026, 5, 28, 0, 15, 0, 0, DateTimeKind.Utc) },
                    { new Guid("e68230ab-13d8-6358-b0b2-48f52ab34f52"), new DateTime(2026, 5, 27, 0, 0, 0, 0, DateTimeKind.Utc), new Guid("de68f3de-ceab-c85f-b54a-645613f6a13e"), null, new Guid("49f942ec-d197-7c5c-a011-6454ca64ec2c"), false, new DateTime(2026, 5, 27, 0, 15, 0, 0, DateTimeKind.Utc) },
                    { new Guid("fbb29be8-a6e1-6756-a051-7a5bf64d1d11"), new DateTime(2026, 5, 24, 0, 0, 0, 0, DateTimeKind.Utc), new Guid("28d446ef-917b-8b59-a814-da2a00b0b76f"), null, new Guid("ef0c12c5-0bcf-4e5f-a13a-4b01b2ed44fc"), false, new DateTime(2026, 5, 24, 0, 15, 0, 0, DateTimeKind.Utc) },
                    { new Guid("fe2a7ed3-0107-d356-892e-ca702fa22e46"), new DateTime(2026, 5, 30, 0, 0, 0, 0, DateTimeKind.Utc), new Guid("28d446ef-917b-8b59-a814-da2a00b0b76f"), null, new Guid("ff182b52-5005-895d-a90a-224ef11c5e61"), false, new DateTime(2026, 5, 30, 0, 15, 0, 0, DateTimeKind.Utc) }
                });

            migrationBuilder.InsertData(
                table: "Discounts",
                columns: new[] { "DiscountId", "Code", "CreatedAt", "DeletedAt", "DiscountType", "EndDate", "IsActive", "IsDeleted", "MaxDiscountAmount", "MinBookingAmount", "Name", "OwnerId", "StartDate", "UsageLimit", "UsedCount", "Value", "VenueId" },
                values: new object[,]
                {
                    { new Guid("3eebaaae-ed4d-f455-b9e7-fde3bb9a5c47"), "COMMUNITY50", new DateTime(2026, 5, 12, 0, 0, 0, 0, DateTimeKind.Utc), null, "Fixed", new DateTime(2026, 8, 5, 0, 0, 0, 0, DateTimeKind.Utc), true, false, 50000m, 300000m, "Community club voucher", new Guid("ef0c12c5-0bcf-4e5f-a13a-4b01b2ed44fc"), new DateTime(2026, 5, 5, 0, 0, 0, 0, DateTimeKind.Utc), 30, 8, 50000m, null },
                    { new Guid("6b80024f-281f-d95f-a9a6-82a579c9622f"), "WEEKEND40", new DateTime(2026, 5, 18, 0, 0, 0, 0, DateTimeKind.Utc), null, "Fixed", new DateTime(2026, 8, 11, 0, 0, 0, 0, DateTimeKind.Utc), true, false, 40000m, 250000m, "Weekend booking voucher", new Guid("49f942ec-d197-7c5c-a011-6454ca64ec2c"), new DateTime(2026, 5, 5, 0, 0, 0, 0, DateTimeKind.Utc), 45, 9, 40000m, null },
                    { new Guid("846a4946-cdf8-185b-9837-ffce189e38c3"), "ARENA75", new DateTime(2026, 5, 14, 0, 0, 0, 0, DateTimeKind.Utc), null, "Fixed", new DateTime(2026, 8, 7, 0, 0, 0, 0, DateTimeKind.Utc), true, false, 75000m, 500000m, "Arena loyalty discount", new Guid("2f958e63-14a1-ee5f-b359-e923bbd70ece"), new DateTime(2026, 5, 5, 0, 0, 0, 0, DateTimeKind.Utc), 20, 6, 75000m, null },
                    { new Guid("dd9dd966-65c1-f051-9431-43662195c57a"), "THUDUC30", new DateTime(2026, 5, 16, 0, 0, 0, 0, DateTimeKind.Utc), null, "Fixed", new DateTime(2026, 8, 9, 0, 0, 0, 0, DateTimeKind.Utc), true, false, 30000m, 180000m, "Thu Duc neighborhood voucher", new Guid("ff182b52-5005-895d-a90a-224ef11c5e61"), new DateTime(2026, 5, 5, 0, 0, 0, 0, DateTimeKind.Utc), 80, 18, 30000m, null }
                });

            migrationBuilder.InsertData(
                table: "NotificationRecipients",
                columns: new[] { "RecipientId", "NotificationId", "ReadAt", "UserId" },
                values: new object[,]
                {
                    { new Guid("2202b536-add3-c75e-b4fd-cb421a5b807f"), new Guid("97035cd2-8716-6451-9bac-10e034a00ed9"), null, new Guid("49f942ec-d197-7c5c-a011-6454ca64ec2c") },
                    { new Guid("25f572cf-feed-f45e-8cad-9649a06f15f5"), new Guid("248e767b-fbd1-765e-a621-af5c5c5d17f4"), null, new Guid("28d446ef-917b-8b59-a814-da2a00b0b76f") },
                    { new Guid("29b7bbef-32cd-2b5d-bcd3-f326e1a06d67"), new Guid("ca73a25b-b382-1054-b57c-84ea963d9eaf"), null, new Guid("cbfe125b-7a8c-335c-aa61-df49f35c448f") },
                    { new Guid("319efd5e-d6ec-cb5a-a5d7-e0ce3c0e9d00"), new Guid("8026f84c-ed63-de5c-9b00-f9ad4ecba28a"), null, new Guid("2f958e63-14a1-ee5f-b359-e923bbd70ece") },
                    { new Guid("53489e62-b880-0c50-9bd3-4708d5c7a68e"), new Guid("8a401056-dda9-765c-a486-ca31419762f1"), null, new Guid("2f958e63-14a1-ee5f-b359-e923bbd70ece") },
                    { new Guid("63d03e2c-b6a7-5657-846d-c575b002ba23"), new Guid("d5c82e74-5b49-d554-a368-ee5e14ada28a"), null, new Guid("eff1cca4-9f7a-0f53-a3e0-115f934fc55b") },
                    { new Guid("6a496349-045a-e851-865c-438c077c10a6"), new Guid("4c77ec9c-2b1a-dc5b-8a29-eabe13fc63a3"), new DateTime(2026, 6, 3, 2, 0, 0, 0, DateTimeKind.Utc), new Guid("28d446ef-917b-8b59-a814-da2a00b0b76f") },
                    { new Guid("ab334594-f033-465f-bbc2-b3d9c315990a"), new Guid("7b322e08-b6c7-9051-accc-f23db0876077"), new DateTime(2026, 5, 30, 2, 0, 0, 0, DateTimeKind.Utc), new Guid("eff1cca4-9f7a-0f53-a3e0-115f934fc55b") },
                    { new Guid("c49fadcb-01f3-6154-985b-58493f27b254"), new Guid("4fc8ee4a-65d5-d551-a569-fe7cff7fdb69"), null, new Guid("49f942ec-d197-7c5c-a011-6454ca64ec2c") },
                    { new Guid("cb487dbe-c95b-d754-987c-9c6b7ee3e90c"), new Guid("16eb01d8-478c-1052-a848-5b5d331a27e3"), null, new Guid("2f958e63-14a1-ee5f-b359-e923bbd70ece") },
                    { new Guid("e0f1866e-bfb5-e250-b3ad-6951a58d7b9c"), new Guid("a39a0ace-4fc8-585f-92f5-cfd19f98c6ae"), null, new Guid("49f942ec-d197-7c5c-a011-6454ca64ec2c") },
                    { new Guid("f2b34149-15fe-bb5b-992a-de4d4834bd85"), new Guid("f1b6d9ba-bf7a-665f-aa99-bd45e5b4eda3"), new DateTime(2026, 5, 26, 2, 0, 0, 0, DateTimeKind.Utc), new Guid("cbfe125b-7a8c-335c-aa61-df49f35c448f") }
                });

            migrationBuilder.InsertData(
                table: "UserRoles",
                columns: new[] { "RoleId", "UserId", "AssignedAt" },
                values: new object[,]
                {
                    { new Guid("76075424-3dac-6259-a0f7-00a4c6c20191"), new Guid("28d446ef-917b-8b59-a814-da2a00b0b76f"), new DateTime(2026, 1, 5, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("07371171-eec1-3255-b1b2-1d8e8e81ede7"), new Guid("2f958e63-14a1-ee5f-b359-e923bbd70ece"), new DateTime(2026, 1, 5, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("07371171-eec1-3255-b1b2-1d8e8e81ede7"), new Guid("49f942ec-d197-7c5c-a011-6454ca64ec2c"), new DateTime(2026, 1, 5, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("76075424-3dac-6259-a0f7-00a4c6c20191"), new Guid("81d10681-e36e-595b-972a-f441c8237537"), new DateTime(2026, 1, 5, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("b5abbaf1-931c-5353-b9ab-1f38eb30b8b8"), new Guid("b41aae5d-9596-9a5d-b8e5-0f8b199a8135"), new DateTime(2026, 1, 5, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("76075424-3dac-6259-a0f7-00a4c6c20191"), new Guid("b53af497-39fc-6351-a424-0a0063d43116"), new DateTime(2026, 1, 5, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("76075424-3dac-6259-a0f7-00a4c6c20191"), new Guid("cbfe125b-7a8c-335c-aa61-df49f35c448f"), new DateTime(2026, 1, 5, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("76075424-3dac-6259-a0f7-00a4c6c20191"), new Guid("de68f3de-ceab-c85f-b54a-645613f6a13e"), new DateTime(2026, 1, 5, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("b5abbaf1-931c-5353-b9ab-1f38eb30b8b8"), new Guid("e3266388-5d3f-c459-beef-1edc2d465a3e"), new DateTime(2026, 1, 5, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("07371171-eec1-3255-b1b2-1d8e8e81ede7"), new Guid("ef0c12c5-0bcf-4e5f-a13a-4b01b2ed44fc"), new DateTime(2026, 1, 5, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("76075424-3dac-6259-a0f7-00a4c6c20191"), new Guid("eff1cca4-9f7a-0f53-a3e0-115f934fc55b"), new DateTime(2026, 1, 5, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("07371171-eec1-3255-b1b2-1d8e8e81ede7"), new Guid("ff182b52-5005-895d-a90a-224ef11c5e61"), new DateTime(2026, 1, 5, 0, 0, 0, 0, DateTimeKind.Utc) }
                });

            migrationBuilder.InsertData(
                table: "Venues",
                columns: new[] { "VenueId", "Address", "CreatedAt", "DeletedAt", "Description", "IsActive", "IsDeleted", "Latitude", "Longitude", "OpeningHours", "OwnerId", "PhoneContact", "UpdatedAt", "VenueName" },
                values: new object[,]
                {
                    { new Guid("1285e289-4aec-e150-a19d-a18470d844c4"), "45 Nguyen Luong Bang, District 7, Ho Chi Minh City", new DateTime(2026, 1, 3, 0, 0, 0, 0, DateTimeKind.Utc), null, "Well maintained turf fields for evening leagues.", true, false, 10.729210m, 106.721916m, "06:00-23:30", new Guid("2f958e63-14a1-ee5f-b359-e923bbd70ece"), "02873010003", null, "District 7 Green Pitch" },
                    { new Guid("74b290df-251c-135a-949e-b8dd5d6d520e"), "91 Binh Quoi, Binh Thanh, Ho Chi Minh City", new DateTime(2026, 1, 2, 0, 0, 0, 0, DateTimeKind.Utc), null, "Community sports hub near Thanh Da peninsula.", true, false, 10.815713m, 106.731719m, "05:30-22:30", new Guid("ef0c12c5-0bcf-4e5f-a13a-4b01b2ed44fc"), "02873010002", null, "Thanh Da Community Football Hub" },
                    { new Guid("76b2e98e-5d43-2452-ad77-c411fbb2e388"), "19 Ton Dat Tien, District 7, Ho Chi Minh City", new DateTime(2026, 1, 4, 0, 0, 0, 0, DateTimeKind.Utc), null, "Premium arena close to office and residential areas.", true, false, 10.732221m, 106.704730m, "06:00-23:00", new Guid("2f958e63-14a1-ee5f-b359-e923bbd70ece"), "02873010004", null, "Phu My Hung Arena" },
                    { new Guid("a1067327-953b-345a-9d63-9f4932f73bc2"), "37 Linh Trung, Thu Duc City, Ho Chi Minh City", new DateTime(2026, 1, 6, 0, 0, 0, 0, DateTimeKind.Utc), null, "Neighborhood five-a-side club with loyal weekly players.", true, false, 10.871823m, 106.779496m, "05:30-22:30", new Guid("ff182b52-5005-895d-a90a-224ef11c5e61"), "02873010006", null, "Linh Trung Five-A-Side Club" },
                    { new Guid("aea33891-2194-505e-898a-64c536f8408c"), "28 Trich Sai, Tay Ho, Hanoi", new DateTime(2026, 1, 11, 0, 0, 0, 0, DateTimeKind.Utc), null, "Compact football complex near West Lake.", true, false, 21.055408m, 105.813839m, "06:00-22:30", new Guid("ff182b52-5005-895d-a90a-224ef11c5e61"), "02473010011", null, "Hanoi West Lake Mini Pitch" },
                    { new Guid("b444aa42-e678-3c53-ab15-e5e05da85358"), "22 Ten Lua, Binh Tan, Ho Chi Minh City", new DateTime(2026, 1, 9, 0, 0, 0, 0, DateTimeKind.Utc), null, "Accessible west-side venue with affordable slots.", true, false, 10.753894m, 106.607990m, "05:30-22:00", new Guid("ef0c12c5-0bcf-4e5f-a13a-4b01b2ed44fc"), "02873010009", null, "Binh Tan Sports Yard" },
                    { new Guid("b81936c7-48de-fd50-be07-27f4af2021f3"), "9 Hai Ba Trung, Ninh Kieu, Can Tho", new DateTime(2026, 1, 12, 0, 0, 0, 0, DateTimeKind.Utc), null, "Central Can Tho venue for evening bookings.", true, false, 10.034103m, 105.788535m, "05:30-22:30", new Guid("49f942ec-d197-7c5c-a011-6454ca64ec2c"), "02927301012", null, "Can Tho Ninh Kieu Sports Ground" },
                    { new Guid("c5a685df-a31c-4755-95f3-c8398a3d9bcd"), "1 Vo Van Ngan, Thu Duc City, Ho Chi Minh City", new DateTime(2026, 1, 5, 0, 0, 0, 0, DateTimeKind.Utc), null, "Large venue suitable for student tournaments.", true, false, 10.849643m, 106.771566m, "05:00-22:00", new Guid("ff182b52-5005-895d-a90a-224ef11c5e61"), "02873010005", null, "Thu Duc University Stadium" },
                    { new Guid("c725798d-b572-2957-a60b-7a42bda965f3"), "88 Bach Dang, Tan Binh, Ho Chi Minh City", new DateTime(2026, 1, 7, 0, 0, 0, 0, DateTimeKind.Utc), null, "Convenient fields near the airport corridor.", true, false, 10.813651m, 106.665408m, "06:00-23:00", new Guid("49f942ec-d197-7c5c-a011-6454ca64ec2c"), "02873010007", null, "Tan Binh Flight Path Fields" },
                    { new Guid("e44e7fa2-d7ec-5f55-af24-333a327d8b39"), "12 Nguyen Huu Canh, Binh Thanh, Ho Chi Minh City", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "Riverside venue with four compact football fields.", true, false, 10.791054m, 106.719809m, "06:00-23:00", new Guid("ef0c12c5-0bcf-4e5f-a13a-4b01b2ed44fc"), "02873010001", null, "Saigon Riverside Sports Park" },
                    { new Guid("e958c525-1a45-f854-97aa-247ccd2a75cd"), "75 Tran Hung Dao, Son Tra, Da Nang", new DateTime(2026, 1, 10, 0, 0, 0, 0, DateTimeKind.Utc), null, "Modern riverside venue in central Da Nang.", true, false, 16.070884m, 108.229401m, "06:00-23:00", new Guid("2f958e63-14a1-ee5f-b359-e923bbd70ece"), "02367301010", null, "Da Nang Han River Football Center" },
                    { new Guid("f4e18d2f-7b53-f353-a00f-02da102573be"), "154 Phan Van Tri, Go Vap, Ho Chi Minh City", new DateTime(2026, 1, 8, 0, 0, 0, 0, DateTimeKind.Utc), null, "Popular weekend venue for amateur clubs.", true, false, 10.833116m, 106.680982m, "06:00-23:30", new Guid("49f942ec-d197-7c5c-a011-6454ca64ec2c"), "02873010008", null, "Go Vap Weekend Arena" }
                });

            migrationBuilder.InsertData(
                table: "BookingDiscounts",
                columns: new[] { "BookingId", "DiscountId", "DiscountAmount" },
                values: new object[,]
                {
                    { new Guid("3ced2bd5-38e9-8b5a-8fc8-4429391d9e0c"), new Guid("dd9dd966-65c1-f051-9431-43662195c57a"), 30000m },
                    { new Guid("523e651a-d316-495b-9e88-614fc24c402e"), new Guid("6b80024f-281f-d95f-a9a6-82a579c9622f"), 40000m },
                    { new Guid("75ab8d9a-f5ab-9d59-9857-e77407addcd8"), new Guid("3eebaaae-ed4d-f455-b9e7-fde3bb9a5c47"), 40000m },
                    { new Guid("9e03adc8-fc50-e257-b572-26ece917a5b7"), new Guid("846a4946-cdf8-185b-9837-ffce189e38c3"), 60000m }
                });

            migrationBuilder.InsertData(
                table: "Discounts",
                columns: new[] { "DiscountId", "Code", "CreatedAt", "DeletedAt", "DiscountType", "EndDate", "IsActive", "IsDeleted", "MaxDiscountAmount", "MinBookingAmount", "Name", "OwnerId", "StartDate", "UsageLimit", "UsedCount", "Value", "VenueId" },
                values: new object[,]
                {
                    { new Guid("033a6084-5112-7251-a441-fd9a61bd9e1b"), "WESTSIDE8", new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), null, "Percentage", new DateTime(2026, 8, 12, 0, 0, 0, 0, DateTimeKind.Utc), true, false, 30000m, 0m, "West side happy hour", new Guid("ef0c12c5-0bcf-4e5f-a13a-4b01b2ed44fc"), new DateTime(2026, 5, 5, 0, 0, 0, 0, DateTimeKind.Utc), 50, 3, 8m, new Guid("b444aa42-e678-3c53-ab15-e5e05da85358") },
                    { new Guid("10bd247b-ac20-e859-babc-1f43a53e1eb0"), "GREEN10", new DateTime(2026, 5, 13, 0, 0, 0, 0, DateTimeKind.Utc), null, "Percentage", new DateTime(2026, 8, 6, 0, 0, 0, 0, DateTimeKind.Utc), true, false, 40000m, 0m, "Green Pitch early week", new Guid("2f958e63-14a1-ee5f-b359-e923bbd70ece"), new DateTime(2026, 5, 5, 0, 0, 0, 0, DateTimeKind.Utc), 25, 4, 10m, new Guid("1285e289-4aec-e150-a19d-a18470d844c4") },
                    { new Guid("2bb0a6c3-3938-0f5c-bf38-734441d43f8f"), "NINHKIEU25", new DateTime(2026, 5, 22, 0, 0, 0, 0, DateTimeKind.Utc), null, "Fixed", new DateTime(2026, 8, 15, 0, 0, 0, 0, DateTimeKind.Utc), true, false, 25000m, 120000m, "Ninh Kieu off-peak voucher", new Guid("49f942ec-d197-7c5c-a011-6454ca64ec2c"), new DateTime(2026, 5, 5, 0, 0, 0, 0, DateTimeKind.Utc), 70, 11, 25000m, new Guid("b81936c7-48de-fd50-be07-27f4af2021f3") },
                    { new Guid("348ee346-d6c6-4554-9024-bf35b86b7fd2"), "FLIGHT12", new DateTime(2026, 5, 17, 0, 0, 0, 0, DateTimeKind.Utc), null, "Percentage", new DateTime(2026, 8, 10, 0, 0, 0, 0, DateTimeKind.Utc), true, false, 50000m, 200000m, "Airport field promotion", new Guid("49f942ec-d197-7c5c-a011-6454ca64ec2c"), new DateTime(2026, 5, 5, 0, 0, 0, 0, DateTimeKind.Utc), 35, 7, 12m, new Guid("c725798d-b572-2957-a60b-7a42bda965f3") },
                    { new Guid("68f1ee5c-cb34-3158-a18d-c174cfdc8386"), "STUDENT15", new DateTime(2026, 5, 15, 0, 0, 0, 0, DateTimeKind.Utc), null, "Percentage", new DateTime(2026, 8, 8, 0, 0, 0, 0, DateTimeKind.Utc), true, false, 45000m, 150000m, "Student evening offer", new Guid("ff182b52-5005-895d-a90a-224ef11c5e61"), new DateTime(2026, 5, 5, 0, 0, 0, 0, DateTimeKind.Utc), 60, 12, 15m, new Guid("c5a685df-a31c-4755-95f3-c8398a3d9bcd") },
                    { new Guid("d83505b1-1f9a-7752-87ad-603d3926437d"), "HANRIVER60", new DateTime(2026, 5, 20, 0, 0, 0, 0, DateTimeKind.Utc), null, "Fixed", new DateTime(2026, 8, 13, 0, 0, 0, 0, DateTimeKind.Utc), true, false, 60000m, 360000m, "Han River group voucher", new Guid("2f958e63-14a1-ee5f-b359-e923bbd70ece"), new DateTime(2026, 5, 5, 0, 0, 0, 0, DateTimeKind.Utc), 28, 5, 60000m, new Guid("e958c525-1a45-f854-97aa-247ccd2a75cd") },
                    { new Guid("f4886193-d1cc-f65a-b1f9-f8fce52c7cdf"), "RIVER20", new DateTime(2026, 5, 11, 0, 0, 0, 0, DateTimeKind.Utc), null, "Percentage", new DateTime(2026, 8, 4, 0, 0, 0, 0, DateTimeKind.Utc), true, false, 60000m, 200000m, "Riverside weekday promotion", new Guid("ef0c12c5-0bcf-4e5f-a13a-4b01b2ed44fc"), new DateTime(2026, 5, 5, 0, 0, 0, 0, DateTimeKind.Utc), 40, 5, 20m, new Guid("e44e7fa2-d7ec-5f55-af24-333a327d8b39") },
                    { new Guid("f99c851e-2926-f95d-bfa2-b987220f2de2"), "WESTLAKE18", new DateTime(2026, 5, 21, 0, 0, 0, 0, DateTimeKind.Utc), null, "Percentage", new DateTime(2026, 8, 14, 0, 0, 0, 0, DateTimeKind.Utc), true, false, 70000m, 300000m, "West Lake membership discount", new Guid("ff182b52-5005-895d-a90a-224ef11c5e61"), new DateTime(2026, 5, 5, 0, 0, 0, 0, DateTimeKind.Utc), 22, 10, 18m, new Guid("aea33891-2194-505e-898a-64c536f8408c") }
                });

            migrationBuilder.InsertData(
                table: "FootballFields",
                columns: new[] { "Id", "CreatedAt", "DeletedAt", "Description", "FieldName", "FieldType", "IsActive", "IsDeleted", "PricePerHour", "UpdatedAt", "VenueId" },
                values: new object[,]
                {
                    { new Guid("09e80b9f-6396-1753-b72e-17e13cc94abb"), new DateTime(2026, 1, 19, 0, 0, 0, 0, DateTimeKind.Utc), null, "Main artificial turf field.", "Thu Duc University Stadium A", "SevenASide", true, false, 200000m, null, new Guid("c5a685df-a31c-4755-95f3-c8398a3d9bcd") },
                    { new Guid("14cf58fc-b1e6-de5b-bcec-45b06e83c66a"), new DateTime(2026, 1, 18, 0, 0, 0, 0, DateTimeKind.Utc), null, "Secondary field for friendly matches.", "Phu My Hung Arena B", "FiveASide", true, false, 235000m, null, new Guid("76b2e98e-5d43-2452-ad77-c411fbb2e388") },
                    { new Guid("208d5f71-5196-6a53-ad42-9156ad65bf46"), new DateTime(2026, 1, 25, 0, 0, 0, 0, DateTimeKind.Utc), null, "Main artificial turf field.", "Go Vap Weekend Arena A", "SevenASide", true, false, 215000m, null, new Guid("f4e18d2f-7b53-f353-a00f-02da102573be") },
                    { new Guid("20c94649-263e-9359-90db-156fbf078756"), new DateTime(2026, 2, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "Secondary field for friendly matches.", "Hanoi West Lake Mini Pitch B", "FiveASide", true, false, 270000m, null, new Guid("aea33891-2194-505e-898a-64c536f8408c") }
                });

            migrationBuilder.InsertData(
                table: "FootballFields",
                columns: new[] { "Id", "CreatedAt", "DeletedAt", "Description", "FieldName", "FieldType", "IsDeleted", "PricePerHour", "UpdatedAt", "VenueId" },
                values: new object[] { new Guid("23369eca-5734-b452-95e7-0aa5558fdd64"), new DateTime(2026, 1, 28, 0, 0, 0, 0, DateTimeKind.Utc), null, "Secondary field for friendly matches.", "Binh Tan Sports Yard B", "ElevenASide", false, 260000m, null, new Guid("b444aa42-e678-3c53-ab15-e5e05da85358") });

            migrationBuilder.InsertData(
                table: "FootballFields",
                columns: new[] { "Id", "CreatedAt", "DeletedAt", "Description", "FieldName", "FieldType", "IsActive", "IsDeleted", "PricePerHour", "UpdatedAt", "VenueId" },
                values: new object[,]
                {
                    { new Guid("294eaa8c-61fd-fc53-a189-e2605d6550ae"), new DateTime(2026, 1, 12, 0, 0, 0, 0, DateTimeKind.Utc), null, "Secondary field for friendly matches.", "Saigon Riverside Sports Park B", "FiveASide", true, false, 220000m, null, new Guid("e44e7fa2-d7ec-5f55-af24-333a327d8b39") },
                    { new Guid("2abd8ce8-1d77-9954-9421-394da8d9b623"), new DateTime(2026, 2, 3, 0, 0, 0, 0, DateTimeKind.Utc), null, "Secondary field for friendly matches.", "Can Tho Ninh Kieu Sports Ground B", "ElevenASide", true, false, 275000m, null, new Guid("b81936c7-48de-fd50-be07-27f4af2021f3") },
                    { new Guid("3682998c-7f03-a350-89d8-2ce45ea05eb9"), new DateTime(2026, 1, 23, 0, 0, 0, 0, DateTimeKind.Utc), null, "Main artificial turf field.", "Tan Binh Flight Path Fields A", "FiveASide", true, false, 210000m, null, new Guid("c725798d-b572-2957-a60b-7a42bda965f3") },
                    { new Guid("45af42d7-f17d-4e5e-be92-aa2f64bc139b"), new DateTime(2026, 1, 11, 0, 0, 0, 0, DateTimeKind.Utc), null, "Main artificial turf field.", "Saigon Riverside Sports Park A", "FiveASide", true, false, 180000m, null, new Guid("e44e7fa2-d7ec-5f55-af24-333a327d8b39") },
                    { new Guid("59f6e46d-42d7-a55c-882c-e1487fbc48e7"), new DateTime(2026, 1, 30, 0, 0, 0, 0, DateTimeKind.Utc), null, "Secondary field for friendly matches.", "Da Nang Han River Football Center B", "FiveASide", true, false, 265000m, null, new Guid("e958c525-1a45-f854-97aa-247ccd2a75cd") },
                    { new Guid("5c7cc6ae-9bba-d454-8042-305cc655d89f"), new DateTime(2026, 1, 29, 0, 0, 0, 0, DateTimeKind.Utc), null, "Main artificial turf field.", "Da Nang Han River Football Center A", "FiveASide", true, false, 225000m, null, new Guid("e958c525-1a45-f854-97aa-247ccd2a75cd") },
                    { new Guid("6410c6c3-9f7f-b05a-b040-d6edfba2d8aa"), new DateTime(2026, 1, 22, 0, 0, 0, 0, DateTimeKind.Utc), null, "Secondary field for friendly matches.", "Linh Trung Five-A-Side Club B", "ElevenASide", true, false, 245000m, null, new Guid("a1067327-953b-345a-9d63-9f4932f73bc2") },
                    { new Guid("6c0c039f-c8ee-ec55-9e58-e7aa2e93778c"), new DateTime(2026, 1, 24, 0, 0, 0, 0, DateTimeKind.Utc), null, "Secondary field for friendly matches.", "Tan Binh Flight Path Fields B", "FiveASide", true, false, 250000m, null, new Guid("c725798d-b572-2957-a60b-7a42bda965f3") },
                    { new Guid("724604cf-d676-845b-a538-85d3af910e6c"), new DateTime(2026, 1, 27, 0, 0, 0, 0, DateTimeKind.Utc), null, "Main artificial turf field.", "Binh Tan Sports Yard A", "SevenASide", true, false, 220000m, null, new Guid("b444aa42-e678-3c53-ab15-e5e05da85358") },
                    { new Guid("79f3417a-16a9-245b-ba20-06b22c6d3499"), new DateTime(2026, 1, 13, 0, 0, 0, 0, DateTimeKind.Utc), null, "Main artificial turf field.", "Thanh Da Community Football Hub A", "SevenASide", true, false, 185000m, null, new Guid("74b290df-251c-135a-949e-b8dd5d6d520e") },
                    { new Guid("85433577-1e55-3153-8189-16c078122b94"), new DateTime(2026, 1, 17, 0, 0, 0, 0, DateTimeKind.Utc), null, "Main artificial turf field.", "Phu My Hung Arena A", "FiveASide", true, false, 195000m, null, new Guid("76b2e98e-5d43-2452-ad77-c411fbb2e388") },
                    { new Guid("8d6a77e0-82a1-045f-8f29-95f3f02bf455"), new DateTime(2026, 1, 26, 0, 0, 0, 0, DateTimeKind.Utc), null, "Secondary field for friendly matches.", "Go Vap Weekend Arena B", "FiveASide", true, false, 255000m, null, new Guid("f4e18d2f-7b53-f353-a00f-02da102573be") },
                    { new Guid("a0e5a644-336c-2b59-bc7a-9f524ab67be9"), new DateTime(2026, 1, 31, 0, 0, 0, 0, DateTimeKind.Utc), null, "Main artificial turf field.", "Hanoi West Lake Mini Pitch A", "SevenASide", true, false, 230000m, null, new Guid("aea33891-2194-505e-898a-64c536f8408c") },
                    { new Guid("a495e9ef-94ab-df5c-b307-f2c002ddcfe1"), new DateTime(2026, 1, 14, 0, 0, 0, 0, DateTimeKind.Utc), null, "Secondary field for friendly matches.", "Thanh Da Community Football Hub B", "FiveASide", true, false, 225000m, null, new Guid("74b290df-251c-135a-949e-b8dd5d6d520e") },
                    { new Guid("ad617cd9-d99d-c958-8a5a-f9cadf1cdcff"), new DateTime(2026, 1, 21, 0, 0, 0, 0, DateTimeKind.Utc), null, "Main artificial turf field.", "Linh Trung Five-A-Side Club A", "SevenASide", true, false, 205000m, null, new Guid("a1067327-953b-345a-9d63-9f4932f73bc2") },
                    { new Guid("ad88bad6-8267-2d57-a1a1-7320527a9aec"), new DateTime(2026, 1, 20, 0, 0, 0, 0, DateTimeKind.Utc), null, "Secondary field for friendly matches.", "Thu Duc University Stadium B", "FiveASide", true, false, 240000m, null, new Guid("c5a685df-a31c-4755-95f3-c8398a3d9bcd") },
                    { new Guid("d69222bd-d845-145a-8281-891e544ed55a"), new DateTime(2026, 2, 2, 0, 0, 0, 0, DateTimeKind.Utc), null, "Main artificial turf field.", "Can Tho Ninh Kieu Sports Ground A", "SevenASide", true, false, 235000m, null, new Guid("b81936c7-48de-fd50-be07-27f4af2021f3") },
                    { new Guid("d8b4c3b4-eae4-235e-ad10-312e902e6f4c"), new DateTime(2026, 1, 15, 0, 0, 0, 0, DateTimeKind.Utc), null, "Main artificial turf field.", "District 7 Green Pitch A", "SevenASide", true, false, 190000m, null, new Guid("1285e289-4aec-e150-a19d-a18470d844c4") },
                    { new Guid("f2f0e4ec-ebfe-ff54-a6e2-e67e6e24d0c7"), new DateTime(2026, 1, 16, 0, 0, 0, 0, DateTimeKind.Utc), null, "Secondary field for friendly matches.", "District 7 Green Pitch B", "ElevenASide", true, false, 230000m, null, new Guid("1285e289-4aec-e150-a19d-a18470d844c4") }
                });

            migrationBuilder.InsertData(
                table: "Messages",
                columns: new[] { "MessageId", "DeletedAt", "IsDeleted", "MessageText", "ReadAt", "RoomId", "SenderId", "SentAt" },
                values: new object[,]
                {
                    { new Guid("26133014-7474-0054-bde1-fcecaaf288f4"), null, false, "Ben em da xac nhan coc, lich da duoc giu thanh cong.", new DateTime(2026, 5, 31, 0, 15, 0, 0, DateTimeKind.Utc), new Guid("814a7c60-ac01-0f5b-bc59-7dee869d6bf2"), new Guid("49f942ec-d197-7c5c-a011-6454ca64ec2c"), new DateTime(2026, 5, 31, 0, 15, 0, 0, DateTimeKind.Utc) },
                    { new Guid("288071c8-fabf-675a-8bbe-985a613a5fb8"), null, false, "Duoc anh, ben em se ho tro doi lich neu thong bao truoc gio da.", new DateTime(2026, 5, 29, 0, 15, 0, 0, DateTimeKind.Utc), new Guid("bea19fa6-d965-0e52-8eb6-b0cabc857c85"), new Guid("2f958e63-14a1-ee5f-b359-e923bbd70ece"), new DateTime(2026, 5, 29, 0, 15, 0, 0, DateTimeKind.Utc) },
                    { new Guid("38aea5e6-ed1a-6c5c-bff4-f8486794b909"), null, false, "San co cho gui xe may rieng khong?", new DateTime(2026, 6, 3, 0, 15, 0, 0, DateTimeKind.Utc), new Guid("69ba1bf9-439b-de57-b77d-f7c0f97db207"), new Guid("eff1cca4-9f7a-0f53-a3e0-115f934fc55b"), new DateTime(2026, 6, 3, 0, 15, 0, 0, DateTimeKind.Utc) },
                    { new Guid("5a2b7d0d-b91b-3559-93b1-f84cc0454c93"), null, false, "Chao anh, toi muon dat san thu bay luc 19h, san con trong khong?", null, new Guid("fbb29be8-a6e1-6756-a051-7a5bf64d1d11"), new Guid("28d446ef-917b-8b59-a814-da2a00b0b76f"), new DateTime(2026, 5, 24, 0, 15, 0, 0, DateTimeKind.Utc) },
                    { new Guid("5f1c0c77-d0d8-865d-9c7d-665bb6214275"), null, false, "Ben em co san bong, ao bib va nuoc uong tai quay le tan.", null, new Guid("e68230ab-13d8-6358-b0b2-48f52ab34f52"), new Guid("49f942ec-d197-7c5c-a011-6454ca64ec2c"), new DateTime(2026, 5, 27, 0, 15, 0, 0, DateTimeKind.Utc) },
                    { new Guid("8ec140a5-200f-6d53-b264-2e25af44be05"), null, false, "Toi can hoa don cho cong ty sau tran dau.", new DateTime(2026, 6, 1, 0, 15, 0, 0, DateTimeKind.Utc), new Guid("94f42dce-592b-6d52-a51d-2b2b6d1a5865"), new Guid("cbfe125b-7a8c-335c-aa61-df49f35c448f"), new DateTime(2026, 6, 1, 0, 15, 0, 0, DateTimeKind.Utc) },
                    { new Guid("993390e2-028a-6358-bf9f-9ef504fa1f7e"), null, false, "San con trong, anh vui long coc truoc 30 phut de giu lich.", new DateTime(2026, 5, 25, 0, 15, 0, 0, DateTimeKind.Utc), new Guid("8f4c756e-90fb-7354-9748-91474d62a7e5"), new Guid("2f958e63-14a1-ee5f-b359-e923bbd70ece"), new DateTime(2026, 5, 25, 0, 15, 0, 0, DateTimeKind.Utc) },
                    { new Guid("9cf6cd8f-931a-7850-9fb3-add16ebe26b3"), null, false, "Anh gui thong tin cong ty, ben em se gui hoa don trong ngay.", null, new Guid("58d096b8-dcec-8351-a96d-6becd8df96c7"), new Guid("2f958e63-14a1-ee5f-b359-e923bbd70ece"), new DateTime(2026, 6, 2, 0, 15, 0, 0, DateTimeKind.Utc) },
                    { new Guid("b48c9d78-0185-7050-8bf6-6ae5fd4d5b1f"), null, false, "Doi cua minh can thue bong va ao bib, san co ho tro khong?", new DateTime(2026, 5, 26, 0, 15, 0, 0, DateTimeKind.Utc), new Guid("17bb8f15-6089-905d-abba-576a0517cf95"), new Guid("cbfe125b-7a8c-335c-aa61-df49f35c448f"), new DateTime(2026, 5, 26, 0, 15, 0, 0, DateTimeKind.Utc) },
                    { new Guid("b6525ee6-a8ad-e95c-8a83-12eb5f2d19dd"), null, false, "Neu troi mua lon thi minh co doi lich sang ngay khac duoc khong?", new DateTime(2026, 5, 28, 0, 15, 0, 0, DateTimeKind.Utc), new Guid("e5c97dc5-a377-ef58-b975-db03712736cb"), new Guid("eff1cca4-9f7a-0f53-a3e0-115f934fc55b"), new DateTime(2026, 5, 28, 0, 15, 0, 0, DateTimeKind.Utc) },
                    { new Guid("e30d9931-d63f-8157-9268-28592d75091e"), null, false, "Minh da thanh toan coc, nho kiem tra giup ma giao dich.", null, new Guid("fe2a7ed3-0107-d356-892e-ca702fa22e46"), new Guid("28d446ef-917b-8b59-a814-da2a00b0b76f"), new DateTime(2026, 5, 30, 0, 15, 0, 0, DateTimeKind.Utc) },
                    { new Guid("eee9300c-5149-415f-b580-0811275b81e7"), null, false, "Co khu gui xe rieng ngay cong vao, mien phi cho nguoi choi.", new DateTime(2026, 6, 4, 0, 15, 0, 0, DateTimeKind.Utc), new Guid("40fc32bb-e297-3e53-8885-3e7976cbbdc1"), new Guid("49f942ec-d197-7c5c-a011-6454ca64ec2c"), new DateTime(2026, 6, 4, 0, 15, 0, 0, DateTimeKind.Utc) }
                });

            migrationBuilder.InsertData(
                table: "Payments",
                columns: new[] { "Id", "Amount", "BookingId", "DeletedAt", "Gateway", "GatewayAccountNumber", "GatewayRawContent", "GatewayReferenceCode", "GatewayTransactionId", "IsDeleted", "PaidAt", "PaymentMethod", "PaymentStatus", "PaymentType", "RefundAmount", "RefundReason", "TransactionCode" },
                values: new object[,]
                {
                    { new Guid("05080d93-e6b1-2e51-91be-a66268fcffd6"), 210000m, new Guid("9e03adc8-fc50-e257-b572-26ece917a5b7"), null, "Cash", null, null, null, null, false, new DateTime(2026, 6, 2, 1, 0, 0, 0, DateTimeKind.Utc), 0, "Success", "Deposit", null, null, "DEP-2026-0002" },
                    { new Guid("089abb78-e465-6d50-b047-ec062133573a"), 225000m, new Guid("3ced2bd5-38e9-8b5a-8fc8-4429391d9e0c"), null, "SePay", "84519828888", "CMFIN-2026-0006", "FT2026050006", "SEPAY-20260006", false, new DateTime(2026, 5, 27, 4, 0, 0, 0, DateTimeKind.Utc), 3, "Success", "Final", null, null, "FIN-2026-0006" },
                    { new Guid("1f0e3fc5-ab2f-455c-b5b5-0cc3146dbf86"), 165000m, new Guid("e909180c-8cea-915e-b9c0-47520fe4a6ad"), null, "SePay", "84519828888", "CMDEP-2026-0001", "FT2026050001", "SEPAY-20260001", false, new DateTime(2026, 6, 1, 1, 0, 0, 0, DateTimeKind.Utc), 3, "Success", "Deposit", null, null, "DEP-2026-0001" },
                    { new Guid("2fbfbeae-bf78-a452-be7d-d8d36728f0ef"), 205000m, new Guid("688cad1e-f0d6-1b5c-add2-059e8ee912b2"), null, "SePay", "84519828888", "CMDEP-2026-0011", "FT2026050011", "SEPAY-20260011", false, null, 3, "Failed", "Deposit", null, null, "DEP-2026-0011" },
                    { new Guid("58261033-9428-0b54-a68a-64cae236f667"), 187500m, new Guid("523e651a-d316-495b-9e88-614fc24c402e"), null, "MoMo", null, null, null, null, false, new DateTime(2026, 5, 26, 2, 0, 0, 0, DateTimeKind.Utc), 1, "Success", "Deposit", null, null, "DEP-2026-0007" },
                    { new Guid("5d253d5b-ae53-ab58-acef-7ca3d9a2ba2e"), 225000m, new Guid("3ced2bd5-38e9-8b5a-8fc8-4429391d9e0c"), null, "SePay", "84519828888", "CMDEP-2026-0005", "FT2026050005", "SEPAY-20260005", false, new DateTime(2026, 5, 27, 2, 0, 0, 0, DateTimeKind.Utc), 3, "Success", "Deposit", null, null, "DEP-2026-0005" },
                    { new Guid("611028e1-59ad-f352-8340-141eb3d2bedd"), 187500m, new Guid("523e651a-d316-495b-9e88-614fc24c402e"), null, "MoMo", null, null, null, null, false, new DateTime(2026, 5, 26, 5, 0, 0, 0, DateTimeKind.Utc), 1, "Success", "Final", null, null, "FIN-2026-0008" },
                    { new Guid("6d37d298-3fba-9c55-bae7-67c8fac57c4a"), 147500m, new Guid("1a4b8f37-e3c9-4357-99a8-b997bebeb529"), null, "Cash", null, null, null, null, false, new DateTime(2026, 5, 25, 1, 0, 0, 0, DateTimeKind.Utc), 0, "Refunded", "Deposit", null, null, "DEP-2026-0012" },
                    { new Guid("943bab94-5bde-3a5b-9338-a6fb552c5ca6"), 210000m, new Guid("9e03adc8-fc50-e257-b572-26ece917a5b7"), null, "Cash", null, null, null, null, false, new DateTime(2026, 6, 2, 3, 0, 0, 0, DateTimeKind.Utc), 0, "Success", "Final", null, null, "FIN-2026-0003" },
                    { new Guid("b2e7df7a-bdfd-8054-9818-57e34fb7058f"), 197500m, new Guid("f05fe560-3108-5857-ad8a-7c9005cf0dba"), null, "VNPay", null, null, null, null, false, new DateTime(2026, 6, 3, 1, 0, 0, 0, DateTimeKind.Utc), 2, "Success", "Deposit", null, null, "DEP-2026-0004" }
                });

            migrationBuilder.InsertData(
                table: "Payments",
                columns: new[] { "Id", "Amount", "BookingId", "DeletedAt", "Gateway", "GatewayAccountNumber", "GatewayRawContent", "GatewayReferenceCode", "GatewayTransactionId", "IsDeleted", "PaidAt", "PaymentMethod", "PaymentType", "RefundAmount", "RefundReason", "TransactionCode" },
                values: new object[,]
                {
                    { new Guid("dfd3965f-0463-9e57-ac42-ec4ba50502c6"), 180000m, new Guid("75ab8d9a-f5ab-9d59-9857-e77407addcd8"), null, "SePay", "84519828888", "CMDEP-2026-0009", "FT2026050009", "SEPAY-20260009", false, null, 3, "Deposit", null, null, "DEP-2026-0009" },
                    { new Guid("f82c5aa7-c89e-e454-9a4d-43569e509ef2"), 142500m, new Guid("33f8c662-d9af-335d-b547-7e116e8b4d74"), null, "VNPay", null, null, null, null, false, null, 2, "Deposit", null, null, "DEP-2026-0010" }
                });

            migrationBuilder.InsertData(
                table: "Reviews",
                columns: new[] { "ReviewId", "BookingId", "Comment", "CreatedAt", "DeletedAt", "IsDeleted", "Rating", "UserId", "VenueId" },
                values: new object[,]
                {
                    { new Guid("07bb5e52-8236-a652-8f11-c23e7bd78399"), new Guid("1a4b8f37-e3c9-4357-99a8-b997bebeb529"), "San phu hop da giao huu cong ty.", new DateTime(2026, 5, 27, 0, 0, 0, 0, DateTimeKind.Utc), null, false, 3, new Guid("81d10681-e36e-595b-972a-f441c8237537"), new Guid("b81936c7-48de-fd50-be07-27f4af2021f3") },
                    { new Guid("34cd8723-ab28-665f-ab88-3aeba15cc879"), new Guid("523e651a-d316-495b-9e88-614fc24c402e"), "Gia cuoi tuan hoi cao nhung dich vu tot.", new DateTime(2026, 5, 26, 0, 0, 0, 0, DateTimeKind.Utc), null, false, 5, new Guid("eff1cca4-9f7a-0f53-a3e0-115f934fc55b"), new Guid("aea33891-2194-505e-898a-64c536f8408c") },
                    { new Guid("4d0ea3ba-1f98-3750-b6aa-8476b1455bce"), new Guid("75ab8d9a-f5ab-9d59-9857-e77407addcd8"), "Vi tri de tim, bai xe rong, gia hop ly.", new DateTime(2026, 5, 17, 0, 0, 0, 0, DateTimeKind.Utc), null, false, 5, new Guid("b53af497-39fc-6351-a424-0a0063d43116"), new Guid("74b290df-251c-135a-949e-b8dd5d6d520e") },
                    { new Guid("66b77147-0055-1659-8664-bd1eb63d8f20"), new Guid("688cad1e-f0d6-1b5c-add2-059e8ee912b2"), "Co day du bong va ao bib cho doi minh.", new DateTime(2026, 5, 21, 0, 0, 0, 0, DateTimeKind.Utc), null, false, 3, new Guid("81d10681-e36e-595b-972a-f441c8237537"), new Guid("a1067327-953b-345a-9d63-9f4932f73bc2") },
                    { new Guid("9f77c259-40af-3751-babe-a8dc7057d518"), new Guid("e909180c-8cea-915e-b9c0-47520fe4a6ad"), "San tot nhung phong thay do hoi dong vao cuoi tuan.", new DateTime(2026, 5, 18, 0, 0, 0, 0, DateTimeKind.Utc), null, false, 3, new Guid("cbfe125b-7a8c-335c-aa61-df49f35c448f"), new Guid("1285e289-4aec-e150-a19d-a18470d844c4") },
                    { new Guid("c7c87667-9694-9950-b977-08433f9ae3e8"), new Guid("f05fe560-3108-5857-ad8a-7c9005cf0dba"), "Khu cho doi sach se, phu hop di cung gia dinh.", new DateTime(2026, 5, 22, 0, 0, 0, 0, DateTimeKind.Utc), null, false, 4, new Guid("28d446ef-917b-8b59-a814-da2a00b0b76f"), new Guid("c725798d-b572-2957-a60b-7a42bda965f3") },
                    { new Guid("dc361c44-3c33-4453-bd7f-4b7cf26aa407"), new Guid("682ba49f-52d0-7f51-934f-f573b2c6e822"), "Mat san em, den sang va nhan vien huong dan nhanh.", new DateTime(2026, 5, 16, 0, 0, 0, 0, DateTimeKind.Utc), null, false, 4, new Guid("28d446ef-917b-8b59-a814-da2a00b0b76f"), new Guid("e44e7fa2-d7ec-5f55-af24-333a327d8b39") },
                    { new Guid("e1388f9b-eece-dc50-b44f-b5074241e076"), new Guid("33f8c662-d9af-335d-b547-7e116e8b4d74"), "Nhan vien check-in dung gio, khong phai cho lau.", new DateTime(2026, 5, 24, 0, 0, 0, 0, DateTimeKind.Utc), null, false, 3, new Guid("cbfe125b-7a8c-335c-aa61-df49f35c448f"), new Guid("b444aa42-e678-3c53-ab15-e5e05da85358") },
                    { new Guid("e5f74a1b-9148-f851-966b-7486b7489c51"), new Guid("e628c15a-2980-635d-bbfb-55eac35bf6f6"), "Chu san ho tro doi gio rat linh hoat.", new DateTime(2026, 5, 20, 0, 0, 0, 0, DateTimeKind.Utc), null, false, 5, new Guid("eff1cca4-9f7a-0f53-a3e0-115f934fc55b"), new Guid("c5a685df-a31c-4755-95f3-c8398a3d9bcd") },
                    { new Guid("eea05db7-6429-0353-a2be-3bd74ca9677c"), new Guid("60eed09f-32b4-c65a-9b85-fa0dcaac75b8"), "Can cai thien them bang diem dien tu.", new DateTime(2026, 5, 25, 0, 0, 0, 0, DateTimeKind.Utc), null, false, 4, new Guid("de68f3de-ceab-c85f-b54a-645613f6a13e"), new Guid("e958c525-1a45-f854-97aa-247ccd2a75cd") },
                    { new Guid("f05f54a6-7eee-0250-a25a-dff608f24fcb"), new Guid("9e03adc8-fc50-e257-b572-26ece917a5b7"), "Dat lich nhanh, thanh toan thuan tien.", new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), null, false, 4, new Guid("de68f3de-ceab-c85f-b54a-645613f6a13e"), new Guid("76b2e98e-5d43-2452-ad77-c411fbb2e388") },
                    { new Guid("f0ba7eb1-7891-c356-afa5-ee2f27e6b24a"), new Guid("3ced2bd5-38e9-8b5a-8fc8-4429391d9e0c"), "Mat co on dinh, khong bi tron khi troi am.", new DateTime(2026, 5, 23, 0, 0, 0, 0, DateTimeKind.Utc), null, false, 5, new Guid("b53af497-39fc-6351-a424-0a0063d43116"), new Guid("f4e18d2f-7b53-f353-a00f-02da102573be") }
                });

            migrationBuilder.InsertData(
                table: "VenueAmenities",
                columns: new[] { "AmenityId", "VenueId" },
                values: new object[,]
                {
                    { new Guid("6215b4d4-8e66-845d-ad2f-53d4f1d3cbeb"), new Guid("1285e289-4aec-e150-a19d-a18470d844c4") },
                    { new Guid("99335921-22b7-3b51-8340-e756ae4c4930"), new Guid("1285e289-4aec-e150-a19d-a18470d844c4") },
                    { new Guid("bd8a547b-4ea6-065e-84eb-082bb11c0ce5"), new Guid("1285e289-4aec-e150-a19d-a18470d844c4") },
                    { new Guid("72951c7c-043d-1c5c-a39a-e299eaece521"), new Guid("74b290df-251c-135a-949e-b8dd5d6d520e") },
                    { new Guid("74d70a98-b403-3255-a580-98eda5f581fc"), new Guid("74b290df-251c-135a-949e-b8dd5d6d520e") },
                    { new Guid("8f3b4b0d-4710-9059-8a85-c256548f118c"), new Guid("74b290df-251c-135a-949e-b8dd5d6d520e") },
                    { new Guid("81815209-f0e6-f752-a864-5236f3aafd94"), new Guid("76b2e98e-5d43-2452-ad77-c411fbb2e388") },
                    { new Guid("b8358615-9686-2d50-8067-ba459980f291"), new Guid("76b2e98e-5d43-2452-ad77-c411fbb2e388") },
                    { new Guid("f5a935e4-be7d-f95f-953e-298059f47da2"), new Guid("76b2e98e-5d43-2452-ad77-c411fbb2e388") },
                    { new Guid("23c9f58d-b578-e552-872f-c81516bbbb45"), new Guid("a1067327-953b-345a-9d63-9f4932f73bc2") },
                    { new Guid("6215b4d4-8e66-845d-ad2f-53d4f1d3cbeb"), new Guid("a1067327-953b-345a-9d63-9f4932f73bc2") },
                    { new Guid("bd8a547b-4ea6-065e-84eb-082bb11c0ce5"), new Guid("a1067327-953b-345a-9d63-9f4932f73bc2") },
                    { new Guid("74d70a98-b403-3255-a580-98eda5f581fc"), new Guid("aea33891-2194-505e-898a-64c536f8408c") },
                    { new Guid("8f3b4b0d-4710-9059-8a85-c256548f118c"), new Guid("aea33891-2194-505e-898a-64c536f8408c") },
                    { new Guid("a745c0e2-627f-085e-af23-316c1a756cbe"), new Guid("aea33891-2194-505e-898a-64c536f8408c") },
                    { new Guid("23c9f58d-b578-e552-872f-c81516bbbb45"), new Guid("b444aa42-e678-3c53-ab15-e5e05da85358") },
                    { new Guid("6215b4d4-8e66-845d-ad2f-53d4f1d3cbeb"), new Guid("b444aa42-e678-3c53-ab15-e5e05da85358") },
                    { new Guid("99335921-22b7-3b51-8340-e756ae4c4930"), new Guid("b444aa42-e678-3c53-ab15-e5e05da85358") },
                    { new Guid("23c9f58d-b578-e552-872f-c81516bbbb45"), new Guid("b81936c7-48de-fd50-be07-27f4af2021f3") },
                    { new Guid("99335921-22b7-3b51-8340-e756ae4c4930"), new Guid("b81936c7-48de-fd50-be07-27f4af2021f3") },
                    { new Guid("bd8a547b-4ea6-065e-84eb-082bb11c0ce5"), new Guid("b81936c7-48de-fd50-be07-27f4af2021f3") },
                    { new Guid("72951c7c-043d-1c5c-a39a-e299eaece521"), new Guid("c5a685df-a31c-4755-95f3-c8398a3d9bcd") },
                    { new Guid("74d70a98-b403-3255-a580-98eda5f581fc"), new Guid("c5a685df-a31c-4755-95f3-c8398a3d9bcd") },
                    { new Guid("a745c0e2-627f-085e-af23-316c1a756cbe"), new Guid("c5a685df-a31c-4755-95f3-c8398a3d9bcd") },
                    { new Guid("8077e70b-71ae-925c-87b1-707df6b54bc0"), new Guid("c725798d-b572-2957-a60b-7a42bda965f3") },
                    { new Guid("81815209-f0e6-f752-a864-5236f3aafd94"), new Guid("c725798d-b572-2957-a60b-7a42bda965f3") },
                    { new Guid("b8358615-9686-2d50-8067-ba459980f291"), new Guid("c725798d-b572-2957-a60b-7a42bda965f3") },
                    { new Guid("8077e70b-71ae-925c-87b1-707df6b54bc0"), new Guid("e44e7fa2-d7ec-5f55-af24-333a327d8b39") },
                    { new Guid("81815209-f0e6-f752-a864-5236f3aafd94"), new Guid("e44e7fa2-d7ec-5f55-af24-333a327d8b39") },
                    { new Guid("f5a935e4-be7d-f95f-953e-298059f47da2"), new Guid("e44e7fa2-d7ec-5f55-af24-333a327d8b39") },
                    { new Guid("8077e70b-71ae-925c-87b1-707df6b54bc0"), new Guid("e958c525-1a45-f854-97aa-247ccd2a75cd") },
                    { new Guid("b8358615-9686-2d50-8067-ba459980f291"), new Guid("e958c525-1a45-f854-97aa-247ccd2a75cd") },
                    { new Guid("f5a935e4-be7d-f95f-953e-298059f47da2"), new Guid("e958c525-1a45-f854-97aa-247ccd2a75cd") },
                    { new Guid("72951c7c-043d-1c5c-a39a-e299eaece521"), new Guid("f4e18d2f-7b53-f353-a00f-02da102573be") },
                    { new Guid("8f3b4b0d-4710-9059-8a85-c256548f118c"), new Guid("f4e18d2f-7b53-f353-a00f-02da102573be") },
                    { new Guid("a745c0e2-627f-085e-af23-316c1a756cbe"), new Guid("f4e18d2f-7b53-f353-a00f-02da102573be") }
                });

            migrationBuilder.InsertData(
                table: "VenueImages",
                columns: new[] { "ImageId", "DeletedAt", "ImageUrl", "IsDeleted", "IsPrimary", "VenueId" },
                values: new object[,]
                {
                    { new Guid("0be32b6f-d831-b052-b2e3-86d585cc3dcb"), null, "https://images.courtmanager.vn/venues/3-field.jpg", false, false, new Guid("1285e289-4aec-e150-a19d-a18470d844c4") },
                    { new Guid("13c493e8-6551-4852-9ada-181a936a9f69"), null, "https://images.courtmanager.vn/venues/7-field.jpg", false, false, new Guid("c725798d-b572-2957-a60b-7a42bda965f3") },
                    { new Guid("1d259280-e9cb-955f-97a8-f56aecdcddad"), null, "https://images.courtmanager.vn/venues/12-field.jpg", false, false, new Guid("b81936c7-48de-fd50-be07-27f4af2021f3") },
                    { new Guid("1f6cb772-e116-4a59-b0d0-5ccf51bfe681"), null, "https://images.courtmanager.vn/venues/5-cover.jpg", false, true, new Guid("c5a685df-a31c-4755-95f3-c8398a3d9bcd") },
                    { new Guid("2383662e-fec7-0f50-834c-ce09582f040a"), null, "https://images.courtmanager.vn/venues/8-cover.jpg", false, true, new Guid("f4e18d2f-7b53-f353-a00f-02da102573be") },
                    { new Guid("28382b24-c202-4d5c-9c0a-d5da02d0b2ab"), null, "https://images.courtmanager.vn/venues/12-cover.jpg", false, true, new Guid("b81936c7-48de-fd50-be07-27f4af2021f3") },
                    { new Guid("2dcb3eb4-2891-095a-b7f1-bf412ab92403"), null, "https://images.courtmanager.vn/venues/11-field.jpg", false, false, new Guid("aea33891-2194-505e-898a-64c536f8408c") },
                    { new Guid("31981a18-d937-735d-876b-4a8ad53a96dc"), null, "https://images.courtmanager.vn/venues/5-field.jpg", false, false, new Guid("c5a685df-a31c-4755-95f3-c8398a3d9bcd") },
                    { new Guid("35082070-0c36-9b5b-856d-e975a537d9b4"), null, "https://images.courtmanager.vn/venues/9-field.jpg", false, false, new Guid("b444aa42-e678-3c53-ab15-e5e05da85358") },
                    { new Guid("394e958f-ae3d-2e51-9184-614bb7eac2f3"), null, "https://images.courtmanager.vn/venues/4-field.jpg", false, false, new Guid("76b2e98e-5d43-2452-ad77-c411fbb2e388") },
                    { new Guid("3ea9c8b1-c9b3-9b58-b278-de5903f9accd"), null, "https://images.courtmanager.vn/venues/1-field.jpg", false, false, new Guid("e44e7fa2-d7ec-5f55-af24-333a327d8b39") },
                    { new Guid("5c1f126e-21ca-b456-9196-08623510c396"), null, "https://images.courtmanager.vn/venues/4-cover.jpg", false, true, new Guid("76b2e98e-5d43-2452-ad77-c411fbb2e388") },
                    { new Guid("63fd918a-007d-fd57-a84b-52ff3f886fdf"), null, "https://images.courtmanager.vn/venues/10-cover.jpg", false, true, new Guid("e958c525-1a45-f854-97aa-247ccd2a75cd") },
                    { new Guid("6d8f0aa8-5100-c05f-80cd-4ff3301ae171"), null, "https://images.courtmanager.vn/venues/1-cover.jpg", false, true, new Guid("e44e7fa2-d7ec-5f55-af24-333a327d8b39") },
                    { new Guid("76c837b7-9902-f858-91c0-d0508f1c0bfa"), null, "https://images.courtmanager.vn/venues/3-cover.jpg", false, true, new Guid("1285e289-4aec-e150-a19d-a18470d844c4") },
                    { new Guid("8f41fab7-2efe-0856-b844-f323d084d99d"), null, "https://images.courtmanager.vn/venues/2-field.jpg", false, false, new Guid("74b290df-251c-135a-949e-b8dd5d6d520e") },
                    { new Guid("950ba886-f8d2-d555-b516-2c5503be75f2"), null, "https://images.courtmanager.vn/venues/11-cover.jpg", false, true, new Guid("aea33891-2194-505e-898a-64c536f8408c") },
                    { new Guid("9d59949b-e1d2-3f5e-b246-14eda540acfc"), null, "https://images.courtmanager.vn/venues/6-cover.jpg", false, true, new Guid("a1067327-953b-345a-9d63-9f4932f73bc2") },
                    { new Guid("a1e7c7c2-b969-4a51-a57d-0d2e810cd868"), null, "https://images.courtmanager.vn/venues/7-cover.jpg", false, true, new Guid("c725798d-b572-2957-a60b-7a42bda965f3") },
                    { new Guid("a83a48cf-debb-6152-a15a-24f40e78334b"), null, "https://images.courtmanager.vn/venues/10-field.jpg", false, false, new Guid("e958c525-1a45-f854-97aa-247ccd2a75cd") },
                    { new Guid("b37671e6-eae1-d15a-9fb8-3712830e5f52"), null, "https://images.courtmanager.vn/venues/6-field.jpg", false, false, new Guid("a1067327-953b-345a-9d63-9f4932f73bc2") },
                    { new Guid("c0835fac-188d-4a5f-b0b5-2428741d8043"), null, "https://images.courtmanager.vn/venues/9-cover.jpg", false, true, new Guid("b444aa42-e678-3c53-ab15-e5e05da85358") },
                    { new Guid("db576a38-eeff-2e59-87e8-4d4f39f2d890"), null, "https://images.courtmanager.vn/venues/8-field.jpg", false, false, new Guid("f4e18d2f-7b53-f353-a00f-02da102573be") },
                    { new Guid("eea5c71a-29cc-8e56-b6ac-2e5b1a37594e"), null, "https://images.courtmanager.vn/venues/2-cover.jpg", false, true, new Guid("74b290df-251c-135a-949e-b8dd5d6d520e") }
                });

            migrationBuilder.InsertData(
                table: "BookingDiscounts",
                columns: new[] { "BookingId", "DiscountId", "DiscountAmount" },
                values: new object[,]
                {
                    { new Guid("33f8c662-d9af-335d-b547-7e116e8b4d74"), new Guid("348ee346-d6c6-4554-9024-bf35b86b7fd2"), 35000m },
                    { new Guid("682ba49f-52d0-7f51-934f-f573b2c6e822"), new Guid("f4886193-d1cc-f65a-b1f9-f8fce52c7cdf"), 10000m },
                    { new Guid("e909180c-8cea-915e-b9c0-47520fe4a6ad"), new Guid("10bd247b-ac20-e859-babc-1f43a53e1eb0"), 30000m },
                    { new Guid("f05fe560-3108-5857-ad8a-7c9005cf0dba"), new Guid("68f1ee5c-cb34-3158-a18d-c174cfdc8386"), 25000m }
                });

            migrationBuilder.InsertData(
                table: "TimeSlots",
                columns: new[] { "SlotId", "CreatedAt", "DeletedAt", "EndTime", "FieldId", "IsDeleted", "LockedBy", "LockedUntil", "Price", "StartTime", "UpdatedAt" },
                values: new object[,]
                {
                    { new Guid("040bc61c-499f-4750-b7e2-c7fe4f5938ab"), new DateTime(2026, 5, 23, 0, 0, 0, 0, DateTimeKind.Utc), null, new DateTime(2026, 6, 6, 22, 0, 0, 0, DateTimeKind.Utc), new Guid("14cf58fc-b1e6-de5b-bcec-45b06e83c66a"), false, null, null, 235000m, new DateTime(2026, 6, 6, 21, 0, 0, 0, DateTimeKind.Utc), null },
                    { new Guid("06d7859e-0ab5-045d-8b38-8b88c4272db0"), new DateTime(2026, 5, 23, 0, 0, 0, 0, DateTimeKind.Utc), null, new DateTime(2026, 6, 6, 20, 0, 0, 0, DateTimeKind.Utc), new Guid("f2f0e4ec-ebfe-ff54-a6e2-e67e6e24d0c7"), false, null, null, 230000m, new DateTime(2026, 6, 6, 19, 0, 0, 0, DateTimeKind.Utc), null },
                    { new Guid("15532bdb-abe3-7150-b1e0-0ee022f9f559"), new DateTime(2026, 5, 21, 0, 0, 0, 0, DateTimeKind.Utc), null, new DateTime(2026, 6, 6, 20, 0, 0, 0, DateTimeKind.Utc), new Guid("79f3417a-16a9-245b-ba20-06b22c6d3499"), false, null, null, 185000m, new DateTime(2026, 6, 6, 19, 0, 0, 0, DateTimeKind.Utc), null },
                    { new Guid("1cc39181-5644-5053-bbba-2749a077476c"), new DateTime(2026, 5, 22, 0, 0, 0, 0, DateTimeKind.Utc), null, new DateTime(2026, 6, 6, 21, 0, 0, 0, DateTimeKind.Utc), new Guid("14cf58fc-b1e6-de5b-bcec-45b06e83c66a"), false, null, null, 235000m, new DateTime(2026, 6, 6, 20, 0, 0, 0, DateTimeKind.Utc), null },
                    { new Guid("2d70063b-e039-815c-a3e3-9aa9558796aa"), new DateTime(2026, 5, 23, 0, 0, 0, 0, DateTimeKind.Utc), null, new DateTime(2026, 6, 6, 19, 0, 0, 0, DateTimeKind.Utc), new Guid("ad88bad6-8267-2d57-a1a1-7320527a9aec"), false, null, null, 240000m, new DateTime(2026, 6, 6, 18, 0, 0, 0, DateTimeKind.Utc), null },
                    { new Guid("31d79c72-1f45-2c59-865a-049d82fbbb3e"), new DateTime(2026, 5, 24, 0, 0, 0, 0, DateTimeKind.Utc), null, new DateTime(2026, 6, 6, 21, 0, 0, 0, DateTimeKind.Utc), new Guid("45af42d7-f17d-4e5e-be92-aa2f64bc139b"), false, null, null, 180000m, new DateTime(2026, 6, 6, 20, 0, 0, 0, DateTimeKind.Utc), null }
                });

            migrationBuilder.InsertData(
                table: "TimeSlots",
                columns: new[] { "SlotId", "CreatedAt", "DeletedAt", "EndTime", "FieldId", "IsDeleted", "LockedBy", "LockedUntil", "Price", "SlotStatus", "StartTime", "UpdatedAt" },
                values: new object[] { new Guid("3c006a7f-08a8-215a-8a32-a7e8bfd55cdf"), new DateTime(2026, 5, 21, 0, 0, 0, 0, DateTimeKind.Utc), null, new DateTime(2026, 6, 6, 19, 0, 0, 0, DateTimeKind.Utc), new Guid("6410c6c3-9f7f-b05a-b040-d6edfba2d8aa"), false, null, null, 245000m, "Booked", new DateTime(2026, 6, 6, 18, 0, 0, 0, DateTimeKind.Utc), null });

            migrationBuilder.InsertData(
                table: "TimeSlots",
                columns: new[] { "SlotId", "CreatedAt", "DeletedAt", "EndTime", "FieldId", "IsDeleted", "LockedBy", "LockedUntil", "Price", "StartTime", "UpdatedAt" },
                values: new object[,]
                {
                    { new Guid("3d2a384b-1331-8952-adff-a38ef7f53b6b"), new DateTime(2026, 5, 25, 0, 0, 0, 0, DateTimeKind.Utc), null, new DateTime(2026, 6, 6, 20, 0, 0, 0, DateTimeKind.Utc), new Guid("09e80b9f-6396-1753-b72e-17e13cc94abb"), false, null, null, 200000m, new DateTime(2026, 6, 6, 19, 0, 0, 0, DateTimeKind.Utc), null },
                    { new Guid("4e889b07-277b-7655-9a63-ad0a363ca856"), new DateTime(2026, 5, 22, 0, 0, 0, 0, DateTimeKind.Utc), null, new DateTime(2026, 6, 6, 20, 0, 0, 0, DateTimeKind.Utc), new Guid("85433577-1e55-3153-8189-16c078122b94"), false, null, null, 195000m, new DateTime(2026, 6, 6, 19, 0, 0, 0, DateTimeKind.Utc), null },
                    { new Guid("5121ac84-ad91-f157-b35b-077a398e9bd6"), new DateTime(2026, 5, 22, 0, 0, 0, 0, DateTimeKind.Utc), null, new DateTime(2026, 6, 6, 21, 0, 0, 0, DateTimeKind.Utc), new Guid("79f3417a-16a9-245b-ba20-06b22c6d3499"), false, null, null, 185000m, new DateTime(2026, 6, 6, 20, 0, 0, 0, DateTimeKind.Utc), null },
                    { new Guid("576e404d-c1f3-8f5c-a170-d77c18465e8f"), new DateTime(2026, 5, 21, 0, 0, 0, 0, DateTimeKind.Utc), null, new DateTime(2026, 6, 6, 22, 0, 0, 0, DateTimeKind.Utc), new Guid("ad88bad6-8267-2d57-a1a1-7320527a9aec"), false, null, null, 240000m, new DateTime(2026, 6, 6, 21, 0, 0, 0, DateTimeKind.Utc), null },
                    { new Guid("5b93bade-b937-9557-9567-b335ba4a7409"), new DateTime(2026, 5, 22, 0, 0, 0, 0, DateTimeKind.Utc), null, new DateTime(2026, 6, 6, 19, 0, 0, 0, DateTimeKind.Utc), new Guid("ad617cd9-d99d-c958-8a5a-f9cadf1cdcff"), false, null, null, 205000m, new DateTime(2026, 6, 6, 18, 0, 0, 0, DateTimeKind.Utc), null },
                    { new Guid("63acab69-d781-915d-bf5e-c1994a1a8311"), new DateTime(2026, 5, 23, 0, 0, 0, 0, DateTimeKind.Utc), null, new DateTime(2026, 6, 6, 21, 0, 0, 0, DateTimeKind.Utc), new Guid("6410c6c3-9f7f-b05a-b040-d6edfba2d8aa"), false, null, null, 245000m, new DateTime(2026, 6, 6, 20, 0, 0, 0, DateTimeKind.Utc), null },
                    { new Guid("6b592d94-6309-df5f-8915-6202c84c5d8c"), new DateTime(2026, 5, 22, 0, 0, 0, 0, DateTimeKind.Utc), null, new DateTime(2026, 6, 6, 22, 0, 0, 0, DateTimeKind.Utc), new Guid("a495e9ef-94ab-df5c-b307-f2c002ddcfe1"), false, null, null, 225000m, new DateTime(2026, 6, 6, 21, 0, 0, 0, DateTimeKind.Utc), null },
                    { new Guid("6fbced5a-84b1-5d5b-8a2c-3b71d7731091"), new DateTime(2026, 5, 22, 0, 0, 0, 0, DateTimeKind.Utc), null, new DateTime(2026, 6, 6, 19, 0, 0, 0, DateTimeKind.Utc), new Guid("45af42d7-f17d-4e5e-be92-aa2f64bc139b"), false, null, null, 180000m, new DateTime(2026, 6, 6, 18, 0, 0, 0, DateTimeKind.Utc), null },
                    { new Guid("7106e4d2-0b62-525f-9b81-c5f41ebafb89"), new DateTime(2026, 5, 24, 0, 0, 0, 0, DateTimeKind.Utc), null, new DateTime(2026, 6, 6, 21, 0, 0, 0, DateTimeKind.Utc), new Guid("ad617cd9-d99d-c958-8a5a-f9cadf1cdcff"), false, null, null, 205000m, new DateTime(2026, 6, 6, 20, 0, 0, 0, DateTimeKind.Utc), null },
                    { new Guid("75166681-8a81-9a59-b666-37c2e249c1cb"), new DateTime(2026, 5, 23, 0, 0, 0, 0, DateTimeKind.Utc), null, new DateTime(2026, 6, 6, 22, 0, 0, 0, DateTimeKind.Utc), new Guid("79f3417a-16a9-245b-ba20-06b22c6d3499"), false, null, null, 185000m, new DateTime(2026, 6, 6, 21, 0, 0, 0, DateTimeKind.Utc), null }
                });

            migrationBuilder.InsertData(
                table: "TimeSlots",
                columns: new[] { "SlotId", "CreatedAt", "DeletedAt", "EndTime", "FieldId", "IsDeleted", "LockedBy", "LockedUntil", "Price", "SlotStatus", "StartTime", "UpdatedAt" },
                values: new object[] { new Guid("78d595b5-43ea-075b-aaeb-4886c784d4b7"), new DateTime(2026, 5, 22, 0, 0, 0, 0, DateTimeKind.Utc), null, new DateTime(2026, 6, 6, 22, 0, 0, 0, DateTimeKind.Utc), new Guid("09e80b9f-6396-1753-b72e-17e13cc94abb"), false, null, null, 200000m, "Booked", new DateTime(2026, 6, 6, 21, 0, 0, 0, DateTimeKind.Utc), null });

            migrationBuilder.InsertData(
                table: "TimeSlots",
                columns: new[] { "SlotId", "CreatedAt", "DeletedAt", "EndTime", "FieldId", "IsDeleted", "LockedBy", "LockedUntil", "Price", "StartTime", "UpdatedAt" },
                values: new object[,]
                {
                    { new Guid("7f0006ba-6bb4-ab5f-8754-d74e5841e645"), new DateTime(2026, 5, 25, 0, 0, 0, 0, DateTimeKind.Utc), null, new DateTime(2026, 6, 6, 22, 0, 0, 0, DateTimeKind.Utc), new Guid("f2f0e4ec-ebfe-ff54-a6e2-e67e6e24d0c7"), false, null, null, 230000m, new DateTime(2026, 6, 6, 21, 0, 0, 0, DateTimeKind.Utc), null },
                    { new Guid("84bbc889-74d1-065b-84d0-d3a34d1677cf"), new DateTime(2026, 5, 23, 0, 0, 0, 0, DateTimeKind.Utc), null, new DateTime(2026, 6, 6, 19, 0, 0, 0, DateTimeKind.Utc), new Guid("d8b4c3b4-eae4-235e-ad10-312e902e6f4c"), false, null, null, 190000m, new DateTime(2026, 6, 6, 18, 0, 0, 0, DateTimeKind.Utc), null },
                    { new Guid("8845749a-92f4-fe53-8a01-036c051fbdd4"), new DateTime(2026, 5, 21, 0, 0, 0, 0, DateTimeKind.Utc), null, new DateTime(2026, 6, 6, 22, 0, 0, 0, DateTimeKind.Utc), new Guid("d8b4c3b4-eae4-235e-ad10-312e902e6f4c"), false, null, null, 190000m, new DateTime(2026, 6, 6, 21, 0, 0, 0, DateTimeKind.Utc), null },
                    { new Guid("88ad348b-db86-e555-9a17-01e4144cb549"), new DateTime(2026, 5, 25, 0, 0, 0, 0, DateTimeKind.Utc), null, new DateTime(2026, 6, 6, 21, 0, 0, 0, DateTimeKind.Utc), new Guid("ad88bad6-8267-2d57-a1a1-7320527a9aec"), false, null, null, 240000m, new DateTime(2026, 6, 6, 20, 0, 0, 0, DateTimeKind.Utc), null }
                });

            migrationBuilder.InsertData(
                table: "TimeSlots",
                columns: new[] { "SlotId", "CreatedAt", "DeletedAt", "EndTime", "FieldId", "IsDeleted", "LockedBy", "LockedUntil", "Price", "SlotStatus", "StartTime", "UpdatedAt" },
                values: new object[] { new Guid("8b70691f-7e6b-cd5a-8f2b-abc9d0272569"), new DateTime(2026, 5, 25, 0, 0, 0, 0, DateTimeKind.Utc), null, new DateTime(2026, 6, 6, 20, 0, 0, 0, DateTimeKind.Utc), new Guid("a495e9ef-94ab-df5c-b307-f2c002ddcfe1"), false, null, new DateTime(2026, 6, 4, 0, 20, 0, 0, DateTimeKind.Utc), 225000m, "Locked", new DateTime(2026, 6, 6, 19, 0, 0, 0, DateTimeKind.Utc), null });

            migrationBuilder.InsertData(
                table: "TimeSlots",
                columns: new[] { "SlotId", "CreatedAt", "DeletedAt", "EndTime", "FieldId", "IsDeleted", "LockedBy", "LockedUntil", "Price", "StartTime", "UpdatedAt" },
                values: new object[,]
                {
                    { new Guid("8dae3ee0-2266-5d54-8524-c6028d2418d6"), new DateTime(2026, 5, 21, 0, 0, 0, 0, DateTimeKind.Utc), null, new DateTime(2026, 6, 6, 19, 0, 0, 0, DateTimeKind.Utc), new Guid("85433577-1e55-3153-8189-16c078122b94"), false, null, null, 195000m, new DateTime(2026, 6, 6, 18, 0, 0, 0, DateTimeKind.Utc), null },
                    { new Guid("8de6ef22-b0c5-d454-be60-4621d5c847b4"), new DateTime(2026, 5, 22, 0, 0, 0, 0, DateTimeKind.Utc), null, new DateTime(2026, 6, 6, 20, 0, 0, 0, DateTimeKind.Utc), new Guid("6410c6c3-9f7f-b05a-b040-d6edfba2d8aa"), false, null, null, 245000m, new DateTime(2026, 6, 6, 19, 0, 0, 0, DateTimeKind.Utc), null },
                    { new Guid("919d9e36-5928-bf55-bab4-fffc84de0838"), new DateTime(2026, 5, 24, 0, 0, 0, 0, DateTimeKind.Utc), null, new DateTime(2026, 6, 6, 19, 0, 0, 0, DateTimeKind.Utc), new Guid("09e80b9f-6396-1753-b72e-17e13cc94abb"), false, null, null, 200000m, new DateTime(2026, 6, 6, 18, 0, 0, 0, DateTimeKind.Utc), null },
                    { new Guid("953f198b-b4c7-a25f-b1fa-9d8fa5dc23f5"), new DateTime(2026, 5, 24, 0, 0, 0, 0, DateTimeKind.Utc), null, new DateTime(2026, 6, 6, 19, 0, 0, 0, DateTimeKind.Utc), new Guid("a495e9ef-94ab-df5c-b307-f2c002ddcfe1"), false, null, null, 225000m, new DateTime(2026, 6, 6, 18, 0, 0, 0, DateTimeKind.Utc), null },
                    { new Guid("96214fbd-d81c-2b50-87a5-27362aa8675a"), new DateTime(2026, 5, 25, 0, 0, 0, 0, DateTimeKind.Utc), null, new DateTime(2026, 6, 6, 22, 0, 0, 0, DateTimeKind.Utc), new Guid("ad617cd9-d99d-c958-8a5a-f9cadf1cdcff"), false, null, null, 205000m, new DateTime(2026, 6, 6, 21, 0, 0, 0, DateTimeKind.Utc), null }
                });

            migrationBuilder.InsertData(
                table: "TimeSlots",
                columns: new[] { "SlotId", "CreatedAt", "DeletedAt", "EndTime", "FieldId", "IsDeleted", "LockedBy", "LockedUntil", "Price", "SlotStatus", "StartTime", "UpdatedAt" },
                values: new object[] { new Guid("9cdfa4ae-ceb2-a156-9375-71fda5d069c0"), new DateTime(2026, 5, 25, 0, 0, 0, 0, DateTimeKind.Utc), null, new DateTime(2026, 6, 6, 19, 0, 0, 0, DateTimeKind.Utc), new Guid("79f3417a-16a9-245b-ba20-06b22c6d3499"), false, null, null, 185000m, "Booked", new DateTime(2026, 6, 6, 18, 0, 0, 0, DateTimeKind.Utc), null });

            migrationBuilder.InsertData(
                table: "TimeSlots",
                columns: new[] { "SlotId", "CreatedAt", "DeletedAt", "EndTime", "FieldId", "IsDeleted", "LockedBy", "LockedUntil", "Price", "StartTime", "UpdatedAt" },
                values: new object[] { new Guid("a41aa1a2-afdb-9850-aba5-cb7276ba896b"), new DateTime(2026, 5, 24, 0, 0, 0, 0, DateTimeKind.Utc), null, new DateTime(2026, 6, 6, 22, 0, 0, 0, DateTimeKind.Utc), new Guid("294eaa8c-61fd-fc53-a189-e2605d6550ae"), false, null, null, 220000m, new DateTime(2026, 6, 6, 21, 0, 0, 0, DateTimeKind.Utc), null });

            migrationBuilder.InsertData(
                table: "TimeSlots",
                columns: new[] { "SlotId", "CreatedAt", "DeletedAt", "EndTime", "FieldId", "IsDeleted", "LockedBy", "LockedUntil", "Price", "SlotStatus", "StartTime", "UpdatedAt" },
                values: new object[] { new Guid("a8f5a04b-4730-e95e-9dff-3e62777a5c7c"), new DateTime(2026, 5, 23, 0, 0, 0, 0, DateTimeKind.Utc), null, new DateTime(2026, 6, 6, 20, 0, 0, 0, DateTimeKind.Utc), new Guid("ad617cd9-d99d-c958-8a5a-f9cadf1cdcff"), false, null, new DateTime(2026, 6, 4, 0, 20, 0, 0, DateTimeKind.Utc), 205000m, "Locked", new DateTime(2026, 6, 6, 19, 0, 0, 0, DateTimeKind.Utc), null });

            migrationBuilder.InsertData(
                table: "TimeSlots",
                columns: new[] { "SlotId", "CreatedAt", "DeletedAt", "EndTime", "FieldId", "IsDeleted", "LockedBy", "LockedUntil", "Price", "StartTime", "UpdatedAt" },
                values: new object[,]
                {
                    { new Guid("b5288789-3ec5-d05e-b715-7d1d5b3c7548"), new DateTime(2026, 5, 24, 0, 0, 0, 0, DateTimeKind.Utc), null, new DateTime(2026, 6, 6, 20, 0, 0, 0, DateTimeKind.Utc), new Guid("ad88bad6-8267-2d57-a1a1-7320527a9aec"), false, null, null, 240000m, new DateTime(2026, 6, 6, 19, 0, 0, 0, DateTimeKind.Utc), null },
                    { new Guid("b5444c8b-63f8-0a55-9419-6de7c1b9e0b6"), new DateTime(2026, 5, 23, 0, 0, 0, 0, DateTimeKind.Utc), null, new DateTime(2026, 6, 6, 20, 0, 0, 0, DateTimeKind.Utc), new Guid("45af42d7-f17d-4e5e-be92-aa2f64bc139b"), false, null, null, 180000m, new DateTime(2026, 6, 6, 19, 0, 0, 0, DateTimeKind.Utc), null },
                    { new Guid("b6a8de4f-1fa4-425f-a4a1-0b7860359450"), new DateTime(2026, 5, 25, 0, 0, 0, 0, DateTimeKind.Utc), null, new DateTime(2026, 6, 6, 21, 0, 0, 0, DateTimeKind.Utc), new Guid("d8b4c3b4-eae4-235e-ad10-312e902e6f4c"), false, null, null, 190000m, new DateTime(2026, 6, 6, 20, 0, 0, 0, DateTimeKind.Utc), null },
                    { new Guid("bbe9d84d-24a1-8459-a75e-24a4c39a7c6e"), new DateTime(2026, 5, 24, 0, 0, 0, 0, DateTimeKind.Utc), null, new DateTime(2026, 6, 6, 22, 0, 0, 0, DateTimeKind.Utc), new Guid("6410c6c3-9f7f-b05a-b040-d6edfba2d8aa"), false, null, null, 245000m, new DateTime(2026, 6, 6, 21, 0, 0, 0, DateTimeKind.Utc), null }
                });

            migrationBuilder.InsertData(
                table: "TimeSlots",
                columns: new[] { "SlotId", "CreatedAt", "DeletedAt", "EndTime", "FieldId", "IsDeleted", "LockedBy", "LockedUntil", "Price", "SlotStatus", "StartTime", "UpdatedAt" },
                values: new object[] { new Guid("c0253a2a-9205-0b53-8ffd-5e912843b5c6"), new DateTime(2026, 5, 24, 0, 0, 0, 0, DateTimeKind.Utc), null, new DateTime(2026, 6, 6, 20, 0, 0, 0, DateTimeKind.Utc), new Guid("d8b4c3b4-eae4-235e-ad10-312e902e6f4c"), false, null, null, 190000m, "Booked", new DateTime(2026, 6, 6, 19, 0, 0, 0, DateTimeKind.Utc), null });

            migrationBuilder.InsertData(
                table: "TimeSlots",
                columns: new[] { "SlotId", "CreatedAt", "DeletedAt", "EndTime", "FieldId", "IsDeleted", "LockedBy", "LockedUntil", "Price", "StartTime", "UpdatedAt" },
                values: new object[,]
                {
                    { new Guid("c16bb36b-86b9-f45c-bda0-955b92ba386f"), new DateTime(2026, 5, 22, 0, 0, 0, 0, DateTimeKind.Utc), null, new DateTime(2026, 6, 6, 20, 0, 0, 0, DateTimeKind.Utc), new Guid("294eaa8c-61fd-fc53-a189-e2605d6550ae"), false, null, null, 220000m, new DateTime(2026, 6, 6, 19, 0, 0, 0, DateTimeKind.Utc), null },
                    { new Guid("c23792c7-d1d8-cd54-990c-c4c26c5f8ec1"), new DateTime(2026, 5, 25, 0, 0, 0, 0, DateTimeKind.Utc), null, new DateTime(2026, 6, 6, 22, 0, 0, 0, DateTimeKind.Utc), new Guid("45af42d7-f17d-4e5e-be92-aa2f64bc139b"), false, null, null, 180000m, new DateTime(2026, 6, 6, 21, 0, 0, 0, DateTimeKind.Utc), null }
                });

            migrationBuilder.InsertData(
                table: "TimeSlots",
                columns: new[] { "SlotId", "CreatedAt", "DeletedAt", "EndTime", "FieldId", "IsDeleted", "LockedBy", "LockedUntil", "Price", "SlotStatus", "StartTime", "UpdatedAt" },
                values: new object[] { new Guid("c2a327a5-8634-ab58-ac77-28d113d9c3c1"), new DateTime(2026, 5, 23, 0, 0, 0, 0, DateTimeKind.Utc), null, new DateTime(2026, 6, 6, 21, 0, 0, 0, DateTimeKind.Utc), new Guid("85433577-1e55-3153-8189-16c078122b94"), false, null, null, 195000m, "Booked", new DateTime(2026, 6, 6, 20, 0, 0, 0, DateTimeKind.Utc), null });

            migrationBuilder.InsertData(
                table: "TimeSlots",
                columns: new[] { "SlotId", "CreatedAt", "DeletedAt", "EndTime", "FieldId", "IsDeleted", "LockedBy", "LockedUntil", "Price", "StartTime", "UpdatedAt" },
                values: new object[,]
                {
                    { new Guid("c311314f-c845-c75e-a009-9907198429b9"), new DateTime(2026, 5, 21, 0, 0, 0, 0, DateTimeKind.Utc), null, new DateTime(2026, 6, 6, 20, 0, 0, 0, DateTimeKind.Utc), new Guid("14cf58fc-b1e6-de5b-bcec-45b06e83c66a"), false, null, null, 235000m, new DateTime(2026, 6, 6, 19, 0, 0, 0, DateTimeKind.Utc), null },
                    { new Guid("c9e9df36-ea22-1758-b5bd-12754e5ea1f4"), new DateTime(2026, 5, 25, 0, 0, 0, 0, DateTimeKind.Utc), null, new DateTime(2026, 6, 6, 19, 0, 0, 0, DateTimeKind.Utc), new Guid("14cf58fc-b1e6-de5b-bcec-45b06e83c66a"), false, null, null, 235000m, new DateTime(2026, 6, 6, 18, 0, 0, 0, DateTimeKind.Utc), null },
                    { new Guid("d80e35a2-7e78-4351-97a7-26145dbe3882"), new DateTime(2026, 5, 24, 0, 0, 0, 0, DateTimeKind.Utc), null, new DateTime(2026, 6, 6, 21, 0, 0, 0, DateTimeKind.Utc), new Guid("f2f0e4ec-ebfe-ff54-a6e2-e67e6e24d0c7"), false, null, null, 230000m, new DateTime(2026, 6, 6, 20, 0, 0, 0, DateTimeKind.Utc), null }
                });

            migrationBuilder.InsertData(
                table: "TimeSlots",
                columns: new[] { "SlotId", "CreatedAt", "DeletedAt", "EndTime", "FieldId", "IsDeleted", "LockedBy", "LockedUntil", "Price", "SlotStatus", "StartTime", "UpdatedAt" },
                values: new object[,]
                {
                    { new Guid("e127cc3d-3a8f-f550-b03b-be63c8330dc4"), new DateTime(2026, 5, 22, 0, 0, 0, 0, DateTimeKind.Utc), null, new DateTime(2026, 6, 6, 19, 0, 0, 0, DateTimeKind.Utc), new Guid("f2f0e4ec-ebfe-ff54-a6e2-e67e6e24d0c7"), false, null, new DateTime(2026, 6, 4, 0, 20, 0, 0, DateTimeKind.Utc), 230000m, "Locked", new DateTime(2026, 6, 6, 18, 0, 0, 0, DateTimeKind.Utc), null },
                    { new Guid("e2de90d9-940a-7557-82be-ce34cd51e2e8"), new DateTime(2026, 5, 24, 0, 0, 0, 0, DateTimeKind.Utc), null, new DateTime(2026, 6, 6, 22, 0, 0, 0, DateTimeKind.Utc), new Guid("85433577-1e55-3153-8189-16c078122b94"), false, null, new DateTime(2026, 6, 4, 0, 20, 0, 0, DateTimeKind.Utc), 195000m, "Locked", new DateTime(2026, 6, 6, 21, 0, 0, 0, DateTimeKind.Utc), null },
                    { new Guid("e83985ff-e394-575d-8128-92fbc8afa75c"), new DateTime(2026, 5, 21, 0, 0, 0, 0, DateTimeKind.Utc), null, new DateTime(2026, 6, 6, 21, 0, 0, 0, DateTimeKind.Utc), new Guid("09e80b9f-6396-1753-b72e-17e13cc94abb"), false, null, new DateTime(2026, 6, 4, 0, 20, 0, 0, DateTimeKind.Utc), 200000m, "Locked", new DateTime(2026, 6, 6, 20, 0, 0, 0, DateTimeKind.Utc), null }
                });

            migrationBuilder.InsertData(
                table: "TimeSlots",
                columns: new[] { "SlotId", "CreatedAt", "DeletedAt", "EndTime", "FieldId", "IsDeleted", "LockedBy", "LockedUntil", "Price", "StartTime", "UpdatedAt" },
                values: new object[,]
                {
                    { new Guid("f2a0f988-40e3-af5f-bf1f-a5dd453b8371"), new DateTime(2026, 5, 21, 0, 0, 0, 0, DateTimeKind.Utc), null, new DateTime(2026, 6, 6, 19, 0, 0, 0, DateTimeKind.Utc), new Guid("294eaa8c-61fd-fc53-a189-e2605d6550ae"), false, null, null, 220000m, new DateTime(2026, 6, 6, 18, 0, 0, 0, DateTimeKind.Utc), null },
                    { new Guid("f92a0d42-7929-3e5b-8f1e-c8f58be22caa"), new DateTime(2026, 5, 21, 0, 0, 0, 0, DateTimeKind.Utc), null, new DateTime(2026, 6, 6, 21, 0, 0, 0, DateTimeKind.Utc), new Guid("a495e9ef-94ab-df5c-b307-f2c002ddcfe1"), false, null, null, 225000m, new DateTime(2026, 6, 6, 20, 0, 0, 0, DateTimeKind.Utc), null }
                });

            migrationBuilder.InsertData(
                table: "TimeSlots",
                columns: new[] { "SlotId", "CreatedAt", "DeletedAt", "EndTime", "FieldId", "IsDeleted", "LockedBy", "LockedUntil", "Price", "SlotStatus", "StartTime", "UpdatedAt" },
                values: new object[] { new Guid("fe2f8199-f3f3-ea56-aee3-a0772ff88948"), new DateTime(2026, 5, 23, 0, 0, 0, 0, DateTimeKind.Utc), null, new DateTime(2026, 6, 6, 21, 0, 0, 0, DateTimeKind.Utc), new Guid("294eaa8c-61fd-fc53-a189-e2605d6550ae"), false, null, new DateTime(2026, 6, 4, 0, 20, 0, 0, DateTimeKind.Utc), 220000m, "Locked", new DateTime(2026, 6, 6, 20, 0, 0, 0, DateTimeKind.Utc), null });

            migrationBuilder.InsertData(
                table: "BookingItems",
                columns: new[] { "BookingItemId", "BookingId", "CreatedAt", "DeletedAt", "IsDeleted", "Price", "SlotId" },
                values: new object[,]
                {
                    { new Guid("04bda27f-533b-305f-acad-2547b349a71f"), new Guid("1a4b8f37-e3c9-4357-99a8-b997bebeb529"), new DateTime(2026, 5, 25, 0, 0, 0, 0, DateTimeKind.Utc), null, false, 185000m, new Guid("75166681-8a81-9a59-b666-37c2e249c1cb") },
                    { new Guid("29845604-71be-505a-b33a-d83db4c04cf1"), new Guid("e909180c-8cea-915e-b9c0-47520fe4a6ad"), new DateTime(2026, 6, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, false, 180000m, new Guid("31d79c72-1f45-2c59-865a-049d82fbbb3e") },
                    { new Guid("41cbdb3c-ca19-1c55-835a-4668e7feb824"), new Guid("75ab8d9a-f5ab-9d59-9857-e77407addcd8"), new DateTime(2026, 5, 31, 0, 0, 0, 0, DateTimeKind.Utc), null, false, 180000m, new Guid("b5444c8b-63f8-0a55-9419-6de7c1b9e0b6") },
                    { new Guid("812c1d7c-0019-ad5c-8142-8e5ce34f1bb5"), new Guid("9e03adc8-fc50-e257-b572-26ece917a5b7"), new DateTime(2026, 6, 2, 0, 0, 0, 0, DateTimeKind.Utc), null, false, 180000m, new Guid("c23792c7-d1d8-cd54-990c-c4c26c5f8ec1") },
                    { new Guid("9c1c985c-fd82-175f-89cd-465efd7e9b0a"), new Guid("f05fe560-3108-5857-ad8a-7c9005cf0dba"), new DateTime(2026, 6, 3, 0, 0, 0, 0, DateTimeKind.Utc), null, false, 220000m, new Guid("fe2f8199-f3f3-ea56-aee3-a0772ff88948") },
                    { new Guid("aa7d602f-8317-9452-bb92-77c164e137ea"), new Guid("682ba49f-52d0-7f51-934f-f573b2c6e822"), new DateTime(2026, 5, 30, 0, 0, 0, 0, DateTimeKind.Utc), null, false, 180000m, new Guid("6fbced5a-84b1-5d5b-8a2c-3b71d7731091") },
                    { new Guid("b3ad338e-9448-d35f-80c2-02348865d8be"), new Guid("60eed09f-32b4-c65a-9b85-fa0dcaac75b8"), new DateTime(2026, 6, 3, 4, 0, 0, 0, DateTimeKind.Utc), null, false, 185000m, new Guid("15532bdb-abe3-7150-b1e0-0ee022f9f559") },
                    { new Guid("be8038ff-ead9-e455-a078-aa7b130e0ac8"), new Guid("3ced2bd5-38e9-8b5a-8fc8-4429391d9e0c"), new DateTime(2026, 5, 27, 0, 0, 0, 0, DateTimeKind.Utc), null, false, 220000m, new Guid("a41aa1a2-afdb-9850-aba5-cb7276ba896b") },
                    { new Guid("d895d4b5-f8f6-a85e-8035-cc235bdebc15"), new Guid("e628c15a-2980-635d-bbfb-55eac35bf6f6"), new DateTime(2026, 5, 28, 0, 0, 0, 0, DateTimeKind.Utc), null, false, 220000m, new Guid("f2a0f988-40e3-af5f-bf1f-a5dd453b8371") },
                    { new Guid("dd92069c-9c13-3454-ba79-576acb3a5f58"), new Guid("688cad1e-f0d6-1b5c-add2-059e8ee912b2"), new DateTime(2026, 5, 29, 0, 0, 0, 0, DateTimeKind.Utc), null, false, 220000m, new Guid("c16bb36b-86b9-f45c-bda0-955b92ba386f") },
                    { new Guid("ec1f6f6e-fe37-8653-aee8-89d2c86a410f"), new Guid("523e651a-d316-495b-9e88-614fc24c402e"), new DateTime(2026, 5, 26, 0, 0, 0, 0, DateTimeKind.Utc), null, false, 185000m, new Guid("5121ac84-ad91-f157-b35b-077a398e9bd6") },
                    { new Guid("ffcd142a-13cd-4d53-bb51-68a5dde7c565"), new Guid("33f8c662-d9af-335d-b547-7e116e8b4d74"), new DateTime(2026, 6, 2, 0, 0, 0, 0, DateTimeKind.Utc), null, false, 185000m, new Guid("9cdfa4ae-ceb2-a156-9375-71fda5d069c0") }
                });

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                table: "AspNetRoles",
                column: "NormalizedName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserRoles_RoleId",
                table: "AspNetUserRoles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                table: "AspNetUsers",
                column: "NormalizedEmail");

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                table: "AspNetUsers",
                column: "NormalizedUserName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_BookingDiscounts_DiscountId",
                table: "BookingDiscounts",
                column: "DiscountId");

            migrationBuilder.CreateIndex(
                name: "IX_BookingItems_BookingId_SlotId",
                table: "BookingItems",
                columns: new[] { "BookingId", "SlotId" });

            migrationBuilder.CreateIndex(
                name: "IX_BookingItems_SlotId",
                table: "BookingItems",
                column: "SlotId");

            migrationBuilder.CreateIndex(
                name: "IX_Bookings_FootballFieldId",
                table: "Bookings",
                column: "FootballFieldId");

            migrationBuilder.CreateIndex(
                name: "IX_Bookings_UserId",
                table: "Bookings",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_ChatRooms_CustomerId_HostId",
                table: "ChatRooms",
                columns: new[] { "CustomerId", "HostId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ChatRooms_HostId",
                table: "ChatRooms",
                column: "HostId");

            migrationBuilder.CreateIndex(
                name: "IX_Discounts_Code",
                table: "Discounts",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Discounts_OwnerId",
                table: "Discounts",
                column: "OwnerId");

            migrationBuilder.CreateIndex(
                name: "IX_Discounts_VenueId",
                table: "Discounts",
                column: "VenueId");

            migrationBuilder.CreateIndex(
                name: "IX_FootballFields_VenueId",
                table: "FootballFields",
                column: "VenueId");

            migrationBuilder.CreateIndex(
                name: "IX_Messages_RoomId_SentAt",
                table: "Messages",
                columns: new[] { "RoomId", "SentAt" });

            migrationBuilder.CreateIndex(
                name: "IX_Messages_SenderId",
                table: "Messages",
                column: "SenderId");

            migrationBuilder.CreateIndex(
                name: "IX_NotificationRecipients_NotificationId",
                table: "NotificationRecipients",
                column: "NotificationId");

            migrationBuilder.CreateIndex(
                name: "IX_NotificationRecipients_UserId",
                table: "NotificationRecipients",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Notifications_UserId",
                table: "Notifications",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Payments_BookingId",
                table: "Payments",
                column: "BookingId");

            migrationBuilder.CreateIndex(
                name: "IX_Payments_Gateway_GatewayReferenceCode",
                table: "Payments",
                columns: new[] { "Gateway", "GatewayReferenceCode" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Payments_Gateway_GatewayTransactionId",
                table: "Payments",
                columns: new[] { "Gateway", "GatewayTransactionId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Payments_TransactionCode",
                table: "Payments",
                column: "TransactionCode",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Reviews_BookingId",
                table: "Reviews",
                column: "BookingId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Reviews_UserId",
                table: "Reviews",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Reviews_VenueId",
                table: "Reviews",
                column: "VenueId");

            migrationBuilder.CreateIndex(
                name: "IX_RoleClaims_RoleId",
                table: "RoleClaims",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_TimeSlots_FieldId_StartTime",
                table: "TimeSlots",
                columns: new[] { "FieldId", "StartTime" });

            migrationBuilder.CreateIndex(
                name: "IX_TimeSlots_LockedBy",
                table: "TimeSlots",
                column: "LockedBy");

            migrationBuilder.CreateIndex(
                name: "IX_UserClaims_UserId",
                table: "UserClaims",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_UserDevices_FcmToken",
                table: "UserDevices",
                column: "FcmToken",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserDevices_UserId",
                table: "UserDevices",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_UserLogins_UserId",
                table: "UserLogins",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_UserRoles_RoleId",
                table: "UserRoles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_Email",
                table: "Users",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_VenueAmenities_AmenityId",
                table: "VenueAmenities",
                column: "AmenityId");

            migrationBuilder.CreateIndex(
                name: "IX_VenueImages_VenueId",
                table: "VenueImages",
                column: "VenueId");

            migrationBuilder.CreateIndex(
                name: "IX_Venues_OwnerId",
                table: "Venues",
                column: "OwnerId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AspNetUserRoles");

            migrationBuilder.DropTable(
                name: "BookingDiscounts");

            migrationBuilder.DropTable(
                name: "BookingItems");

            migrationBuilder.DropTable(
                name: "Messages");

            migrationBuilder.DropTable(
                name: "NotificationRecipients");

            migrationBuilder.DropTable(
                name: "Payments");

            migrationBuilder.DropTable(
                name: "Reviews");

            migrationBuilder.DropTable(
                name: "RoleClaims");

            migrationBuilder.DropTable(
                name: "UserClaims");

            migrationBuilder.DropTable(
                name: "UserDevices");

            migrationBuilder.DropTable(
                name: "UserLogins");

            migrationBuilder.DropTable(
                name: "UserRoles");

            migrationBuilder.DropTable(
                name: "UserTokens");

            migrationBuilder.DropTable(
                name: "VenueAmenities");

            migrationBuilder.DropTable(
                name: "VenueImages");

            migrationBuilder.DropTable(
                name: "Discounts");

            migrationBuilder.DropTable(
                name: "TimeSlots");

            migrationBuilder.DropTable(
                name: "ChatRooms");

            migrationBuilder.DropTable(
                name: "Notifications");

            migrationBuilder.DropTable(
                name: "Bookings");

            migrationBuilder.DropTable(
                name: "AspNetRoles");

            migrationBuilder.DropTable(
                name: "Roles");

            migrationBuilder.DropTable(
                name: "AspNetUsers");

            migrationBuilder.DropTable(
                name: "Amenities");

            migrationBuilder.DropTable(
                name: "FootballFields");

            migrationBuilder.DropTable(
                name: "Venues");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
