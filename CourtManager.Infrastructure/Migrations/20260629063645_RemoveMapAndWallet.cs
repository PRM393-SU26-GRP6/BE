using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CourtManager.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RemoveMapAndWallet : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "WalletTransactions");

            migrationBuilder.DropTable(
                name: "WithdrawalRequests");

            migrationBuilder.DropColumn(
                name: "Latitude",
                table: "Venues");

            migrationBuilder.DropColumn(
                name: "Longitude",
                table: "Venues");

            migrationBuilder.AlterColumn<decimal>(
                name: "WalletBalance",
                table: "Users",
                type: "numeric",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric(10,2)",
                oldPrecision: 10,
                oldScale: 2,
                oldDefaultValue: 0m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "Latitude",
                table: "Venues",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "Longitude",
                table: "Venues",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AlterColumn<decimal>(
                name: "WalletBalance",
                table: "Users",
                type: "numeric(10,2)",
                precision: 10,
                scale: 2,
                nullable: false,
                defaultValue: 0m,
                oldClrType: typeof(decimal),
                oldType: "numeric");

            migrationBuilder.CreateTable(
                name: "WithdrawalRequests",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ApprovedByAdminId = table.Column<Guid>(type: "uuid", nullable: true),
                    OwnerId = table.Column<Guid>(type: "uuid", nullable: false),
                    Amount = table.Column<decimal>(type: "numeric(10,2)", precision: 10, scale: 2, nullable: false),
                    ApprovedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    BankAccountHolderName = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    BankAccountNumber = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    BankName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    RejectionReason = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    Status = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false, defaultValue: "Pending")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WithdrawalRequests", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WithdrawalRequests_Users_ApprovedByAdminId",
                        column: x => x.ApprovedByAdminId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_WithdrawalRequests_Users_OwnerId",
                        column: x => x.OwnerId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "WalletTransactions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    OwnerId = table.Column<Guid>(type: "uuid", nullable: false),
                    RelatedBookingId = table.Column<Guid>(type: "uuid", nullable: true),
                    RelatedWithdrawalId = table.Column<Guid>(type: "uuid", nullable: true),
                    Amount = table.Column<decimal>(type: "numeric(10,2)", precision: 10, scale: 2, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    Description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    Type = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WalletTransactions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WalletTransactions_Bookings_RelatedBookingId",
                        column: x => x.RelatedBookingId,
                        principalTable: "Bookings",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_WalletTransactions_Users_OwnerId",
                        column: x => x.OwnerId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_WalletTransactions_WithdrawalRequests_RelatedWithdrawalId",
                        column: x => x.RelatedWithdrawalId,
                        principalTable: "WithdrawalRequests",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.UpdateData(
                table: "Venues",
                keyColumn: "VenueId",
                keyValue: new Guid("1285e289-4aec-e150-a19d-a18470d844c4"),
                columns: new[] { "Latitude", "Longitude" },
                values: new object[] { 10.729210m, 106.721916m });

            migrationBuilder.UpdateData(
                table: "Venues",
                keyColumn: "VenueId",
                keyValue: new Guid("74b290df-251c-135a-949e-b8dd5d6d520e"),
                columns: new[] { "Latitude", "Longitude" },
                values: new object[] { 10.815713m, 106.731719m });

            migrationBuilder.UpdateData(
                table: "Venues",
                keyColumn: "VenueId",
                keyValue: new Guid("76b2e98e-5d43-2452-ad77-c411fbb2e388"),
                columns: new[] { "Latitude", "Longitude" },
                values: new object[] { 10.732221m, 106.704730m });

            migrationBuilder.UpdateData(
                table: "Venues",
                keyColumn: "VenueId",
                keyValue: new Guid("a1067327-953b-345a-9d63-9f4932f73bc2"),
                columns: new[] { "Latitude", "Longitude" },
                values: new object[] { 10.871823m, 106.779496m });

            migrationBuilder.UpdateData(
                table: "Venues",
                keyColumn: "VenueId",
                keyValue: new Guid("aea33891-2194-505e-898a-64c536f8408c"),
                columns: new[] { "Latitude", "Longitude" },
                values: new object[] { 21.055408m, 105.813839m });

            migrationBuilder.UpdateData(
                table: "Venues",
                keyColumn: "VenueId",
                keyValue: new Guid("b444aa42-e678-3c53-ab15-e5e05da85358"),
                columns: new[] { "Latitude", "Longitude" },
                values: new object[] { 10.753894m, 106.607990m });

            migrationBuilder.UpdateData(
                table: "Venues",
                keyColumn: "VenueId",
                keyValue: new Guid("b81936c7-48de-fd50-be07-27f4af2021f3"),
                columns: new[] { "Latitude", "Longitude" },
                values: new object[] { 10.034103m, 105.788535m });

            migrationBuilder.UpdateData(
                table: "Venues",
                keyColumn: "VenueId",
                keyValue: new Guid("c5a685df-a31c-4755-95f3-c8398a3d9bcd"),
                columns: new[] { "Latitude", "Longitude" },
                values: new object[] { 10.849643m, 106.771566m });

            migrationBuilder.UpdateData(
                table: "Venues",
                keyColumn: "VenueId",
                keyValue: new Guid("c725798d-b572-2957-a60b-7a42bda965f3"),
                columns: new[] { "Latitude", "Longitude" },
                values: new object[] { 10.813651m, 106.665408m });

            migrationBuilder.UpdateData(
                table: "Venues",
                keyColumn: "VenueId",
                keyValue: new Guid("e44e7fa2-d7ec-5f55-af24-333a327d8b39"),
                columns: new[] { "Latitude", "Longitude" },
                values: new object[] { 10.791054m, 106.719809m });

            migrationBuilder.UpdateData(
                table: "Venues",
                keyColumn: "VenueId",
                keyValue: new Guid("e958c525-1a45-f854-97aa-247ccd2a75cd"),
                columns: new[] { "Latitude", "Longitude" },
                values: new object[] { 16.070884m, 108.229401m });

            migrationBuilder.UpdateData(
                table: "Venues",
                keyColumn: "VenueId",
                keyValue: new Guid("f4e18d2f-7b53-f353-a00f-02da102573be"),
                columns: new[] { "Latitude", "Longitude" },
                values: new object[] { 10.833116m, 106.680982m });

            migrationBuilder.CreateIndex(
                name: "IX_WalletTransactions_CreatedAt",
                table: "WalletTransactions",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_WalletTransactions_OwnerId",
                table: "WalletTransactions",
                column: "OwnerId");

            migrationBuilder.CreateIndex(
                name: "IX_WalletTransactions_RelatedBookingId",
                table: "WalletTransactions",
                column: "RelatedBookingId");

            migrationBuilder.CreateIndex(
                name: "IX_WalletTransactions_RelatedWithdrawalId",
                table: "WalletTransactions",
                column: "RelatedWithdrawalId");

            migrationBuilder.CreateIndex(
                name: "IX_WalletTransactions_Type",
                table: "WalletTransactions",
                column: "Type");

            migrationBuilder.CreateIndex(
                name: "IX_WithdrawalRequests_ApprovedByAdminId",
                table: "WithdrawalRequests",
                column: "ApprovedByAdminId");

            migrationBuilder.CreateIndex(
                name: "IX_WithdrawalRequests_CreatedAt",
                table: "WithdrawalRequests",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_WithdrawalRequests_OwnerId",
                table: "WithdrawalRequests",
                column: "OwnerId");

            migrationBuilder.CreateIndex(
                name: "IX_WithdrawalRequests_Status",
                table: "WithdrawalRequests",
                column: "Status");
        }
    }
}
