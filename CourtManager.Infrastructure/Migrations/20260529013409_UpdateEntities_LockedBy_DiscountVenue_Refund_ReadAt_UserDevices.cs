using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CourtManager.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateEntities_LockedBy_DiscountVenue_Refund_ReadAt_UserDevices : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Discounts_FootballFields_FieldId",
                table: "Discounts");

            migrationBuilder.DropForeignKey(
                name: "FK_Discounts_Users_OwnerId",
                table: "Discounts");

            migrationBuilder.DropIndex(
                name: "IX_TimeSlots_FieldId",
                table: "TimeSlots");

            migrationBuilder.DropIndex(
                name: "IX_ChatRooms_CustomerId",
                table: "ChatRooms");

            migrationBuilder.DropColumn(
                name: "IsRead",
                table: "NotificationRecipients");

            migrationBuilder.DropColumn(
                name: "IsRead",
                table: "Messages");

            migrationBuilder.RenameColumn(
                name: "FieldId",
                table: "Discounts",
                newName: "VenueId");

            migrationBuilder.RenameIndex(
                name: "IX_Discounts_FieldId",
                table: "Discounts",
                newName: "IX_Discounts_VenueId");

            migrationBuilder.AddColumn<Guid>(
                name: "LockedBy",
                table: "TimeSlots",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "RefundAmount",
                table: "Payments",
                type: "numeric(10,2)",
                precision: 10,
                scale: 2,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RefundReason",
                table: "Payments",
                type: "character varying(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ReadAt",
                table: "Messages",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "Value",
                table: "Discounts",
                type: "numeric(10,2)",
                precision: 10,
                scale: 2,
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Discounts",
                type: "character varying(200)",
                maxLength: 200,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<decimal>(
                name: "MinBookingAmount",
                table: "Discounts",
                type: "numeric(10,2)",
                precision: 10,
                scale: 2,
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric");

            migrationBuilder.AlterColumn<decimal>(
                name: "MaxDiscountAmount",
                table: "Discounts",
                type: "numeric(10,2)",
                precision: 10,
                scale: 2,
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric");

            migrationBuilder.AlterColumn<string>(
                name: "DiscountType",
                table: "Discounts",
                type: "character varying(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                table: "Discounts",
                type: "timestamp with time zone",
                nullable: false,
                defaultValueSql: "CURRENT_TIMESTAMP",
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone");

            migrationBuilder.AlterColumn<string>(
                name: "Code",
                table: "Discounts",
                type: "character varying(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

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

            migrationBuilder.CreateIndex(
                name: "IX_TimeSlots_FieldId_StartTime",
                table: "TimeSlots",
                columns: new[] { "FieldId", "StartTime" });

            migrationBuilder.CreateIndex(
                name: "IX_TimeSlots_LockedBy",
                table: "TimeSlots",
                column: "LockedBy");

            migrationBuilder.CreateIndex(
                name: "IX_Discounts_Code",
                table: "Discounts",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ChatRooms_CustomerId_HostId",
                table: "ChatRooms",
                columns: new[] { "CustomerId", "HostId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserDevices_FcmToken",
                table: "UserDevices",
                column: "FcmToken",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserDevices_UserId",
                table: "UserDevices",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Discounts_Users_OwnerId",
                table: "Discounts",
                column: "OwnerId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Discounts_Venues_VenueId",
                table: "Discounts",
                column: "VenueId",
                principalTable: "Venues",
                principalColumn: "VenueId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_TimeSlots_Users_LockedBy",
                table: "TimeSlots",
                column: "LockedBy",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Discounts_Users_OwnerId",
                table: "Discounts");

            migrationBuilder.DropForeignKey(
                name: "FK_Discounts_Venues_VenueId",
                table: "Discounts");

            migrationBuilder.DropForeignKey(
                name: "FK_TimeSlots_Users_LockedBy",
                table: "TimeSlots");

            migrationBuilder.DropTable(
                name: "UserDevices");

            migrationBuilder.DropIndex(
                name: "IX_TimeSlots_FieldId_StartTime",
                table: "TimeSlots");

            migrationBuilder.DropIndex(
                name: "IX_TimeSlots_LockedBy",
                table: "TimeSlots");

            migrationBuilder.DropIndex(
                name: "IX_Discounts_Code",
                table: "Discounts");

            migrationBuilder.DropIndex(
                name: "IX_ChatRooms_CustomerId_HostId",
                table: "ChatRooms");

            migrationBuilder.DropColumn(
                name: "LockedBy",
                table: "TimeSlots");

            migrationBuilder.DropColumn(
                name: "RefundAmount",
                table: "Payments");

            migrationBuilder.DropColumn(
                name: "RefundReason",
                table: "Payments");

            migrationBuilder.DropColumn(
                name: "ReadAt",
                table: "Messages");

            migrationBuilder.RenameColumn(
                name: "VenueId",
                table: "Discounts",
                newName: "FieldId");

            migrationBuilder.RenameIndex(
                name: "IX_Discounts_VenueId",
                table: "Discounts",
                newName: "IX_Discounts_FieldId");

            migrationBuilder.AddColumn<bool>(
                name: "IsRead",
                table: "NotificationRecipients",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsRead",
                table: "Messages",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AlterColumn<decimal>(
                name: "Value",
                table: "Discounts",
                type: "numeric",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric(10,2)",
                oldPrecision: 10,
                oldScale: 2);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Discounts",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(200)",
                oldMaxLength: 200);

            migrationBuilder.AlterColumn<decimal>(
                name: "MinBookingAmount",
                table: "Discounts",
                type: "numeric",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric(10,2)",
                oldPrecision: 10,
                oldScale: 2);

            migrationBuilder.AlterColumn<decimal>(
                name: "MaxDiscountAmount",
                table: "Discounts",
                type: "numeric",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric(10,2)",
                oldPrecision: 10,
                oldScale: 2);

            migrationBuilder.AlterColumn<int>(
                name: "DiscountType",
                table: "Discounts",
                type: "integer",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(50)",
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                table: "Discounts",
                type: "timestamp with time zone",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldDefaultValueSql: "CURRENT_TIMESTAMP");

            migrationBuilder.AlterColumn<string>(
                name: "Code",
                table: "Discounts",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(50)",
                oldMaxLength: 50);

            migrationBuilder.CreateIndex(
                name: "IX_TimeSlots_FieldId",
                table: "TimeSlots",
                column: "FieldId");

            migrationBuilder.CreateIndex(
                name: "IX_ChatRooms_CustomerId",
                table: "ChatRooms",
                column: "CustomerId");

            migrationBuilder.AddForeignKey(
                name: "FK_Discounts_FootballFields_FieldId",
                table: "Discounts",
                column: "FieldId",
                principalTable: "FootballFields",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Discounts_Users_OwnerId",
                table: "Discounts",
                column: "OwnerId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
