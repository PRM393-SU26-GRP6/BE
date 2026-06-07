using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CourtManager.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddGatewayDescriptionToPayment : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "GatewayDescription",
                table: "Payments",
                type: "character varying(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TransactionDate",
                table: "Payments",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.UpdateData(
                table: "Payments",
                keyColumn: "Id",
                keyValue: new Guid("05080d93-e6b1-2e51-91be-a66268fcffd6"),
                columns: new[] { "GatewayDescription", "TransactionDate" },
                values: new object[] { null, null });

            migrationBuilder.UpdateData(
                table: "Payments",
                keyColumn: "Id",
                keyValue: new Guid("089abb78-e465-6d50-b047-ec062133573a"),
                columns: new[] { "GatewayDescription", "TransactionDate" },
                values: new object[] { null, null });

            migrationBuilder.UpdateData(
                table: "Payments",
                keyColumn: "Id",
                keyValue: new Guid("1f0e3fc5-ab2f-455c-b5b5-0cc3146dbf86"),
                columns: new[] { "GatewayDescription", "TransactionDate" },
                values: new object[] { null, null });

            migrationBuilder.UpdateData(
                table: "Payments",
                keyColumn: "Id",
                keyValue: new Guid("2fbfbeae-bf78-a452-be7d-d8d36728f0ef"),
                columns: new[] { "GatewayDescription", "TransactionDate" },
                values: new object[] { null, null });

            migrationBuilder.UpdateData(
                table: "Payments",
                keyColumn: "Id",
                keyValue: new Guid("58261033-9428-0b54-a68a-64cae236f667"),
                columns: new[] { "GatewayDescription", "TransactionDate" },
                values: new object[] { null, null });

            migrationBuilder.UpdateData(
                table: "Payments",
                keyColumn: "Id",
                keyValue: new Guid("5d253d5b-ae53-ab58-acef-7ca3d9a2ba2e"),
                columns: new[] { "GatewayDescription", "TransactionDate" },
                values: new object[] { null, null });

            migrationBuilder.UpdateData(
                table: "Payments",
                keyColumn: "Id",
                keyValue: new Guid("611028e1-59ad-f352-8340-141eb3d2bedd"),
                columns: new[] { "GatewayDescription", "TransactionDate" },
                values: new object[] { null, null });

            migrationBuilder.UpdateData(
                table: "Payments",
                keyColumn: "Id",
                keyValue: new Guid("6d37d298-3fba-9c55-bae7-67c8fac57c4a"),
                columns: new[] { "GatewayDescription", "TransactionDate" },
                values: new object[] { null, null });

            migrationBuilder.UpdateData(
                table: "Payments",
                keyColumn: "Id",
                keyValue: new Guid("943bab94-5bde-3a5b-9338-a6fb552c5ca6"),
                columns: new[] { "GatewayDescription", "TransactionDate" },
                values: new object[] { null, null });

            migrationBuilder.UpdateData(
                table: "Payments",
                keyColumn: "Id",
                keyValue: new Guid("b2e7df7a-bdfd-8054-9818-57e34fb7058f"),
                columns: new[] { "GatewayDescription", "TransactionDate" },
                values: new object[] { null, null });

            migrationBuilder.UpdateData(
                table: "Payments",
                keyColumn: "Id",
                keyValue: new Guid("dfd3965f-0463-9e57-ac42-ec4ba50502c6"),
                columns: new[] { "GatewayDescription", "TransactionDate" },
                values: new object[] { null, null });

            migrationBuilder.UpdateData(
                table: "Payments",
                keyColumn: "Id",
                keyValue: new Guid("f82c5aa7-c89e-e454-9a4d-43569e509ef2"),
                columns: new[] { "GatewayDescription", "TransactionDate" },
                values: new object[] { null, null });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "GatewayDescription",
                table: "Payments");

            migrationBuilder.DropColumn(
                name: "TransactionDate",
                table: "Payments");
        }
    }
}
