using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CourtManager.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddEmailVerificationToUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "EmailVerifiedAt",
                table: "Users",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsEmailVerified",
                table: "Users",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("28d446ef-917b-8b59-a814-da2a00b0b76f"),
                columns: new[] { "EmailVerifiedAt", "IsEmailVerified" },
                values: new object[] { null, false });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("2f958e63-14a1-ee5f-b359-e923bbd70ece"),
                columns: new[] { "EmailVerifiedAt", "IsEmailVerified" },
                values: new object[] { null, false });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("49f942ec-d197-7c5c-a011-6454ca64ec2c"),
                columns: new[] { "EmailVerifiedAt", "IsEmailVerified" },
                values: new object[] { null, false });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("81d10681-e36e-595b-972a-f441c8237537"),
                columns: new[] { "EmailVerifiedAt", "IsEmailVerified" },
                values: new object[] { null, false });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("b41aae5d-9596-9a5d-b8e5-0f8b199a8135"),
                columns: new[] { "EmailVerifiedAt", "IsEmailVerified" },
                values: new object[] { null, false });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("b53af497-39fc-6351-a424-0a0063d43116"),
                columns: new[] { "EmailVerifiedAt", "IsEmailVerified" },
                values: new object[] { null, false });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("cbfe125b-7a8c-335c-aa61-df49f35c448f"),
                columns: new[] { "EmailVerifiedAt", "IsEmailVerified" },
                values: new object[] { null, false });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("de68f3de-ceab-c85f-b54a-645613f6a13e"),
                columns: new[] { "EmailVerifiedAt", "IsEmailVerified" },
                values: new object[] { null, false });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("e3266388-5d3f-c459-beef-1edc2d465a3e"),
                columns: new[] { "EmailVerifiedAt", "IsEmailVerified" },
                values: new object[] { null, false });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("ef0c12c5-0bcf-4e5f-a13a-4b01b2ed44fc"),
                columns: new[] { "EmailVerifiedAt", "IsEmailVerified" },
                values: new object[] { null, false });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("eff1cca4-9f7a-0f53-a3e0-115f934fc55b"),
                columns: new[] { "EmailVerifiedAt", "IsEmailVerified" },
                values: new object[] { null, false });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("ff182b52-5005-895d-a90a-224ef11c5e61"),
                columns: new[] { "EmailVerifiedAt", "IsEmailVerified" },
                values: new object[] { null, false });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EmailVerifiedAt",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "IsEmailVerified",
                table: "Users");
        }
    }
}
