using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace CourtManager.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class SeedVenuesFieldsReviews : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Bookings",
                columns: new[] { "Id", "BookingStatus", "CreatedAt", "DeletedAt", "DepositAmount", "FootballFieldId", "IsDeleted", "Note", "TotalPrice", "UpdatedAt", "UserId" },
                values: new object[,]
                {
                    { new Guid("60000000-0000-0000-0000-000000000001"), "Completed", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, 100000m, null, false, null, 200000m, null, new Guid("20000000-0000-0000-0000-000000000006") },
                    { new Guid("60000000-0000-0000-0000-000000000002"), "Completed", new DateTime(2025, 1, 2, 0, 0, 0, 0, DateTimeKind.Utc), null, 100000m, null, false, null, 200000m, null, new Guid("20000000-0000-0000-0000-000000000007") },
                    { new Guid("60000000-0000-0000-0000-000000000003"), "Completed", new DateTime(2025, 1, 3, 0, 0, 0, 0, DateTimeKind.Utc), null, 100000m, null, false, null, 200000m, null, new Guid("20000000-0000-0000-0000-000000000008") }
                });

            migrationBuilder.InsertData(
                table: "Venues",
                columns: new[] { "VenueId", "Address", "CreatedAt", "DeletedAt", "Description", "IsActive", "IsDeleted", "Latitude", "Longitude", "OpeningHours", "OwnerId", "PhoneContact", "UpdatedAt", "VenueName" },
                values: new object[,]
                {
                    { new Guid("30000000-0000-0000-0000-000000000001"), "30 Phan Thúc Duyện, Tân Bình, TP.HCM", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "Cụm sân bóng mini lớn nhất Tân Bình.", true, false, 10.8016m, 106.6653m, "06:00 - 23:00", new Guid("20000000-0000-0000-0000-000000000003"), "0900000003", null, "Sân Bóng Chảo Lửa" },
                    { new Guid("30000000-0000-0000-0000-000000000002"), "Hẻm 12 Thăng Long, Tân Bình, TP.HCM", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "Sân cỏ nhân tạo mới thay, chất lượng cao.", true, false, 10.8030m, 106.6620m, "05:00 - 24:00", new Guid("20000000-0000-0000-0000-000000000003"), "0900000003", null, "Sân Bóng Thăng Long" }
                });

            migrationBuilder.InsertData(
                table: "FootballFields",
                columns: new[] { "Id", "CreatedAt", "DeletedAt", "FieldName", "FieldType", "IsActive", "IsDeleted", "PricePerHour", "UpdatedAt", "VenueId" },
                values: new object[,]
                {
                    { new Guid("40000000-0000-0000-0000-000000000001"), new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "Sân 5 số 1", "FiveASide", true, false, 200000m, null, new Guid("30000000-0000-0000-0000-000000000001") },
                    { new Guid("40000000-0000-0000-0000-000000000002"), new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "Sân 5 số 2", "FiveASide", true, false, 220000m, null, new Guid("30000000-0000-0000-0000-000000000001") },
                    { new Guid("40000000-0000-0000-0000-000000000003"), new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "Sân 7 VIP", "SevenASide", true, false, 450000m, null, new Guid("30000000-0000-0000-0000-000000000001") },
                    { new Guid("40000000-0000-0000-0000-000000000004"), new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "Sân A", "FiveASide", true, false, 180000m, null, new Guid("30000000-0000-0000-0000-000000000002") },
                    { new Guid("40000000-0000-0000-0000-000000000005"), new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "Sân B", "SevenASide", true, false, 400000m, null, new Guid("30000000-0000-0000-0000-000000000002") }
                });

            migrationBuilder.InsertData(
                table: "Reviews",
                columns: new[] { "ReviewId", "BookingId", "Comment", "CreatedAt", "DeletedAt", "IsDeleted", "Rating", "UserId", "VenueId" },
                values: new object[,]
                {
                    { new Guid("50000000-0000-0000-0000-000000000001"), new Guid("60000000-0000-0000-0000-000000000001"), "Sân rất đẹp, đèn sáng.", new DateTime(2025, 1, 2, 0, 0, 0, 0, DateTimeKind.Utc), null, false, 5, new Guid("20000000-0000-0000-0000-000000000006"), new Guid("30000000-0000-0000-0000-000000000001") },
                    { new Guid("50000000-0000-0000-0000-000000000002"), new Guid("60000000-0000-0000-0000-000000000002"), "Cỏ hơi mòn ở khu vực giữa sân.", new DateTime(2025, 1, 3, 0, 0, 0, 0, DateTimeKind.Utc), null, false, 4, new Guid("20000000-0000-0000-0000-000000000007"), new Guid("30000000-0000-0000-0000-000000000001") },
                    { new Guid("50000000-0000-0000-0000-000000000003"), new Guid("60000000-0000-0000-0000-000000000003"), "Giá cả hợp lý, chủ sân nhiệt tình.", new DateTime(2025, 1, 4, 0, 0, 0, 0, DateTimeKind.Utc), null, false, 5, new Guid("20000000-0000-0000-0000-000000000008"), new Guid("30000000-0000-0000-0000-000000000002") }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "FootballFields",
                keyColumn: "Id",
                keyValue: new Guid("40000000-0000-0000-0000-000000000001"));

            migrationBuilder.DeleteData(
                table: "FootballFields",
                keyColumn: "Id",
                keyValue: new Guid("40000000-0000-0000-0000-000000000002"));

            migrationBuilder.DeleteData(
                table: "FootballFields",
                keyColumn: "Id",
                keyValue: new Guid("40000000-0000-0000-0000-000000000003"));

            migrationBuilder.DeleteData(
                table: "FootballFields",
                keyColumn: "Id",
                keyValue: new Guid("40000000-0000-0000-0000-000000000004"));

            migrationBuilder.DeleteData(
                table: "FootballFields",
                keyColumn: "Id",
                keyValue: new Guid("40000000-0000-0000-0000-000000000005"));

            migrationBuilder.DeleteData(
                table: "Reviews",
                keyColumn: "ReviewId",
                keyValue: new Guid("50000000-0000-0000-0000-000000000001"));

            migrationBuilder.DeleteData(
                table: "Reviews",
                keyColumn: "ReviewId",
                keyValue: new Guid("50000000-0000-0000-0000-000000000002"));

            migrationBuilder.DeleteData(
                table: "Reviews",
                keyColumn: "ReviewId",
                keyValue: new Guid("50000000-0000-0000-0000-000000000003"));

            migrationBuilder.DeleteData(
                table: "Bookings",
                keyColumn: "Id",
                keyValue: new Guid("60000000-0000-0000-0000-000000000001"));

            migrationBuilder.DeleteData(
                table: "Bookings",
                keyColumn: "Id",
                keyValue: new Guid("60000000-0000-0000-0000-000000000002"));

            migrationBuilder.DeleteData(
                table: "Bookings",
                keyColumn: "Id",
                keyValue: new Guid("60000000-0000-0000-0000-000000000003"));

            migrationBuilder.DeleteData(
                table: "Venues",
                keyColumn: "VenueId",
                keyValue: new Guid("30000000-0000-0000-0000-000000000001"));

            migrationBuilder.DeleteData(
                table: "Venues",
                keyColumn: "VenueId",
                keyValue: new Guid("30000000-0000-0000-0000-000000000002"));
        }
    }
}
