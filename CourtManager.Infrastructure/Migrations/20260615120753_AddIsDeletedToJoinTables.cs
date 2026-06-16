using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CourtManager.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddIsDeletedToJoinTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BookingDiscounts_Discounts_DiscountId",
                table: "BookingDiscounts");

            migrationBuilder.DropIndex(
                name: "IX_NotificationRecipients_NotificationId",
                table: "NotificationRecipients");

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "UserRoles",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "UserDevices",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "NotificationRecipients",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "FieldSchedules",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AlterColumn<decimal>(
                name: "DiscountAmount",
                table: "BookingDiscounts",
                type: "numeric(10,2)",
                precision: 10,
                scale: 2,
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric");

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "BookingDiscounts",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.UpdateData(
                table: "BookingDiscounts",
                keyColumns: new[] { "BookingId", "DiscountId" },
                keyValues: new object[] { new Guid("33f8c662-d9af-335d-b547-7e116e8b4d74"), new Guid("348ee346-d6c6-4554-9024-bf35b86b7fd2") },
                column: "IsDeleted",
                value: false);

            migrationBuilder.UpdateData(
                table: "BookingDiscounts",
                keyColumns: new[] { "BookingId", "DiscountId" },
                keyValues: new object[] { new Guid("3ced2bd5-38e9-8b5a-8fc8-4429391d9e0c"), new Guid("dd9dd966-65c1-f051-9431-43662195c57a") },
                column: "IsDeleted",
                value: false);

            migrationBuilder.UpdateData(
                table: "BookingDiscounts",
                keyColumns: new[] { "BookingId", "DiscountId" },
                keyValues: new object[] { new Guid("523e651a-d316-495b-9e88-614fc24c402e"), new Guid("6b80024f-281f-d95f-a9a6-82a579c9622f") },
                column: "IsDeleted",
                value: false);

            migrationBuilder.UpdateData(
                table: "BookingDiscounts",
                keyColumns: new[] { "BookingId", "DiscountId" },
                keyValues: new object[] { new Guid("682ba49f-52d0-7f51-934f-f573b2c6e822"), new Guid("f4886193-d1cc-f65a-b1f9-f8fce52c7cdf") },
                column: "IsDeleted",
                value: false);

            migrationBuilder.UpdateData(
                table: "BookingDiscounts",
                keyColumns: new[] { "BookingId", "DiscountId" },
                keyValues: new object[] { new Guid("75ab8d9a-f5ab-9d59-9857-e77407addcd8"), new Guid("3eebaaae-ed4d-f455-b9e7-fde3bb9a5c47") },
                column: "IsDeleted",
                value: false);

            migrationBuilder.UpdateData(
                table: "BookingDiscounts",
                keyColumns: new[] { "BookingId", "DiscountId" },
                keyValues: new object[] { new Guid("9e03adc8-fc50-e257-b572-26ece917a5b7"), new Guid("846a4946-cdf8-185b-9837-ffce189e38c3") },
                column: "IsDeleted",
                value: false);

            migrationBuilder.UpdateData(
                table: "BookingDiscounts",
                keyColumns: new[] { "BookingId", "DiscountId" },
                keyValues: new object[] { new Guid("e909180c-8cea-915e-b9c0-47520fe4a6ad"), new Guid("10bd247b-ac20-e859-babc-1f43a53e1eb0") },
                column: "IsDeleted",
                value: false);

            migrationBuilder.UpdateData(
                table: "BookingDiscounts",
                keyColumns: new[] { "BookingId", "DiscountId" },
                keyValues: new object[] { new Guid("f05fe560-3108-5857-ad8a-7c9005cf0dba"), new Guid("68f1ee5c-cb34-3158-a18d-c174cfdc8386") },
                column: "IsDeleted",
                value: false);

            migrationBuilder.UpdateData(
                table: "FieldSchedules",
                keyColumn: "ScheduleId",
                keyValue: new Guid("013bb6ad-b7a7-ae59-8b5f-4bf79ce99180"),
                column: "IsDeleted",
                value: false);

            migrationBuilder.UpdateData(
                table: "FieldSchedules",
                keyColumn: "ScheduleId",
                keyValue: new Guid("02531572-e57a-2754-b6eb-a7d22e665c1a"),
                column: "IsDeleted",
                value: false);

            migrationBuilder.UpdateData(
                table: "FieldSchedules",
                keyColumn: "ScheduleId",
                keyValue: new Guid("0491b9a9-3a92-4e54-ae35-952797854a71"),
                column: "IsDeleted",
                value: false);

            migrationBuilder.UpdateData(
                table: "FieldSchedules",
                keyColumn: "ScheduleId",
                keyValue: new Guid("0575fbbf-1c9f-785a-8a6b-4bc18ffb1d3b"),
                column: "IsDeleted",
                value: false);

            migrationBuilder.UpdateData(
                table: "FieldSchedules",
                keyColumn: "ScheduleId",
                keyValue: new Guid("058e4b2f-9e28-ca50-b378-40125554e313"),
                column: "IsDeleted",
                value: false);

            migrationBuilder.UpdateData(
                table: "FieldSchedules",
                keyColumn: "ScheduleId",
                keyValue: new Guid("063a89d8-e62a-9e5a-aae6-ac9e5cac8756"),
                column: "IsDeleted",
                value: false);

            migrationBuilder.UpdateData(
                table: "FieldSchedules",
                keyColumn: "ScheduleId",
                keyValue: new Guid("0685f776-dd81-bf53-be0e-f5cd9f105891"),
                column: "IsDeleted",
                value: false);

            migrationBuilder.UpdateData(
                table: "FieldSchedules",
                keyColumn: "ScheduleId",
                keyValue: new Guid("07132cc8-e78a-8458-a6f1-c5603a88d0b8"),
                column: "IsDeleted",
                value: false);

            migrationBuilder.UpdateData(
                table: "FieldSchedules",
                keyColumn: "ScheduleId",
                keyValue: new Guid("09e87817-6a82-6559-925f-fb0cebff2730"),
                column: "IsDeleted",
                value: false);

            migrationBuilder.UpdateData(
                table: "FieldSchedules",
                keyColumn: "ScheduleId",
                keyValue: new Guid("0b1338b9-6192-a25b-addc-06b07744f34f"),
                column: "IsDeleted",
                value: false);

            migrationBuilder.UpdateData(
                table: "FieldSchedules",
                keyColumn: "ScheduleId",
                keyValue: new Guid("0b4b6b6e-0f64-a859-8096-27ad8e045289"),
                column: "IsDeleted",
                value: false);

            migrationBuilder.UpdateData(
                table: "FieldSchedules",
                keyColumn: "ScheduleId",
                keyValue: new Guid("0c8e5cec-bd52-9751-93fb-2faa4400dded"),
                column: "IsDeleted",
                value: false);

            migrationBuilder.UpdateData(
                table: "FieldSchedules",
                keyColumn: "ScheduleId",
                keyValue: new Guid("10464588-2765-a55c-8d0b-93c78705952e"),
                column: "IsDeleted",
                value: false);

            migrationBuilder.UpdateData(
                table: "FieldSchedules",
                keyColumn: "ScheduleId",
                keyValue: new Guid("107287c0-b67e-eb59-8607-aa45bf25e669"),
                column: "IsDeleted",
                value: false);

            migrationBuilder.UpdateData(
                table: "FieldSchedules",
                keyColumn: "ScheduleId",
                keyValue: new Guid("12a7b9e5-c8ea-ce57-8846-22c417fcc4de"),
                column: "IsDeleted",
                value: false);

            migrationBuilder.UpdateData(
                table: "FieldSchedules",
                keyColumn: "ScheduleId",
                keyValue: new Guid("130299c8-3402-c05c-a72d-f172fb2f0bd0"),
                column: "IsDeleted",
                value: false);

            migrationBuilder.UpdateData(
                table: "FieldSchedules",
                keyColumn: "ScheduleId",
                keyValue: new Guid("13af3102-b76f-d157-a217-b08cd15c8490"),
                column: "IsDeleted",
                value: false);

            migrationBuilder.UpdateData(
                table: "FieldSchedules",
                keyColumn: "ScheduleId",
                keyValue: new Guid("148c5fd1-dec1-6058-94b4-a7e276b2c5e4"),
                column: "IsDeleted",
                value: false);

            migrationBuilder.UpdateData(
                table: "FieldSchedules",
                keyColumn: "ScheduleId",
                keyValue: new Guid("19c02692-ebea-1752-8ece-fb21f8520f6d"),
                column: "IsDeleted",
                value: false);

            migrationBuilder.UpdateData(
                table: "FieldSchedules",
                keyColumn: "ScheduleId",
                keyValue: new Guid("1af32550-e819-a75d-bf17-29beefa07477"),
                column: "IsDeleted",
                value: false);

            migrationBuilder.UpdateData(
                table: "FieldSchedules",
                keyColumn: "ScheduleId",
                keyValue: new Guid("1dfea34a-f4a9-d05d-ba58-a173a4ba106b"),
                column: "IsDeleted",
                value: false);

            migrationBuilder.UpdateData(
                table: "FieldSchedules",
                keyColumn: "ScheduleId",
                keyValue: new Guid("213e63de-9c83-b65f-be81-d3f90842bded"),
                column: "IsDeleted",
                value: false);

            migrationBuilder.UpdateData(
                table: "FieldSchedules",
                keyColumn: "ScheduleId",
                keyValue: new Guid("22bd0656-60a5-2c5b-9f58-11dc374a12a8"),
                column: "IsDeleted",
                value: false);

            migrationBuilder.UpdateData(
                table: "FieldSchedules",
                keyColumn: "ScheduleId",
                keyValue: new Guid("235fc545-c47d-1d59-a97b-3f7c1d5a1a54"),
                column: "IsDeleted",
                value: false);

            migrationBuilder.UpdateData(
                table: "FieldSchedules",
                keyColumn: "ScheduleId",
                keyValue: new Guid("24ebfbf1-3a5c-7957-8792-337722239d30"),
                column: "IsDeleted",
                value: false);

            migrationBuilder.UpdateData(
                table: "FieldSchedules",
                keyColumn: "ScheduleId",
                keyValue: new Guid("25a3bdfc-492f-8851-9015-940fc9ea1597"),
                column: "IsDeleted",
                value: false);

            migrationBuilder.UpdateData(
                table: "FieldSchedules",
                keyColumn: "ScheduleId",
                keyValue: new Guid("27fb87fd-7a43-335b-aaba-9747be6abd44"),
                column: "IsDeleted",
                value: false);

            migrationBuilder.UpdateData(
                table: "FieldSchedules",
                keyColumn: "ScheduleId",
                keyValue: new Guid("2b97b93b-c6a7-c054-bd2b-436812c481f3"),
                column: "IsDeleted",
                value: false);

            migrationBuilder.UpdateData(
                table: "FieldSchedules",
                keyColumn: "ScheduleId",
                keyValue: new Guid("2c7a1f45-32fa-dd5a-817c-f67414863d87"),
                column: "IsDeleted",
                value: false);

            migrationBuilder.UpdateData(
                table: "FieldSchedules",
                keyColumn: "ScheduleId",
                keyValue: new Guid("2d361750-68c9-6152-bc9f-7fa3963b830e"),
                column: "IsDeleted",
                value: false);

            migrationBuilder.UpdateData(
                table: "FieldSchedules",
                keyColumn: "ScheduleId",
                keyValue: new Guid("2d92b241-437f-ef52-b6ab-9853b9141bf3"),
                column: "IsDeleted",
                value: false);

            migrationBuilder.UpdateData(
                table: "FieldSchedules",
                keyColumn: "ScheduleId",
                keyValue: new Guid("2da20770-9319-d056-893f-67bd8f9bd0f7"),
                column: "IsDeleted",
                value: false);

            migrationBuilder.UpdateData(
                table: "FieldSchedules",
                keyColumn: "ScheduleId",
                keyValue: new Guid("30168f79-ae50-8550-9e05-1a306d003510"),
                column: "IsDeleted",
                value: false);

            migrationBuilder.UpdateData(
                table: "FieldSchedules",
                keyColumn: "ScheduleId",
                keyValue: new Guid("34b599f8-53aa-3253-afcb-da0acd0b8943"),
                column: "IsDeleted",
                value: false);

            migrationBuilder.UpdateData(
                table: "FieldSchedules",
                keyColumn: "ScheduleId",
                keyValue: new Guid("35387aa7-9241-b751-b317-ce8345152171"),
                column: "IsDeleted",
                value: false);

            migrationBuilder.UpdateData(
                table: "FieldSchedules",
                keyColumn: "ScheduleId",
                keyValue: new Guid("3703a90a-ac04-305b-a88e-125076ac55f9"),
                column: "IsDeleted",
                value: false);

            migrationBuilder.UpdateData(
                table: "FieldSchedules",
                keyColumn: "ScheduleId",
                keyValue: new Guid("383a0fc6-a233-ca57-b2cf-a13d85d5ee1b"),
                column: "IsDeleted",
                value: false);

            migrationBuilder.UpdateData(
                table: "FieldSchedules",
                keyColumn: "ScheduleId",
                keyValue: new Guid("397516f4-7ff9-bd58-aaaa-507f46bc3ea7"),
                column: "IsDeleted",
                value: false);

            migrationBuilder.UpdateData(
                table: "FieldSchedules",
                keyColumn: "ScheduleId",
                keyValue: new Guid("3c39563e-eda6-2356-9f3d-28f6f62f55ca"),
                column: "IsDeleted",
                value: false);

            migrationBuilder.UpdateData(
                table: "FieldSchedules",
                keyColumn: "ScheduleId",
                keyValue: new Guid("3cbf7585-e4cd-0a52-9eac-dfc059972d64"),
                column: "IsDeleted",
                value: false);

            migrationBuilder.UpdateData(
                table: "FieldSchedules",
                keyColumn: "ScheduleId",
                keyValue: new Guid("3d07bcb1-5260-4556-bf03-d7faf71fdd49"),
                column: "IsDeleted",
                value: false);

            migrationBuilder.UpdateData(
                table: "FieldSchedules",
                keyColumn: "ScheduleId",
                keyValue: new Guid("3dc423ba-dd8c-2851-9a0a-cc037908b97d"),
                column: "IsDeleted",
                value: false);

            migrationBuilder.UpdateData(
                table: "FieldSchedules",
                keyColumn: "ScheduleId",
                keyValue: new Guid("42361363-497b-a351-ada1-7cf627d9311e"),
                column: "IsDeleted",
                value: false);

            migrationBuilder.UpdateData(
                table: "FieldSchedules",
                keyColumn: "ScheduleId",
                keyValue: new Guid("4ab3b1b8-4412-2c5e-9793-a761c8547b1b"),
                column: "IsDeleted",
                value: false);

            migrationBuilder.UpdateData(
                table: "FieldSchedules",
                keyColumn: "ScheduleId",
                keyValue: new Guid("4bd47ffc-6b2c-6953-a77e-257432e3ed4a"),
                column: "IsDeleted",
                value: false);

            migrationBuilder.UpdateData(
                table: "FieldSchedules",
                keyColumn: "ScheduleId",
                keyValue: new Guid("4be2767e-449d-365b-8b9d-5f6f9a2d9a84"),
                column: "IsDeleted",
                value: false);

            migrationBuilder.UpdateData(
                table: "FieldSchedules",
                keyColumn: "ScheduleId",
                keyValue: new Guid("4c1ff72b-ad79-c15d-a3ca-90b942f79cd6"),
                column: "IsDeleted",
                value: false);

            migrationBuilder.UpdateData(
                table: "FieldSchedules",
                keyColumn: "ScheduleId",
                keyValue: new Guid("4c416e03-7249-1f56-8165-54666aeb75bf"),
                column: "IsDeleted",
                value: false);

            migrationBuilder.UpdateData(
                table: "FieldSchedules",
                keyColumn: "ScheduleId",
                keyValue: new Guid("4d3248f8-1b69-635b-b239-790154384404"),
                column: "IsDeleted",
                value: false);

            migrationBuilder.UpdateData(
                table: "FieldSchedules",
                keyColumn: "ScheduleId",
                keyValue: new Guid("4dd5bae4-3c22-3a58-aa49-0cbf872e6e50"),
                column: "IsDeleted",
                value: false);

            migrationBuilder.UpdateData(
                table: "FieldSchedules",
                keyColumn: "ScheduleId",
                keyValue: new Guid("4e2db5c3-fc71-7e54-925d-e8a67ae9c493"),
                column: "IsDeleted",
                value: false);

            migrationBuilder.UpdateData(
                table: "FieldSchedules",
                keyColumn: "ScheduleId",
                keyValue: new Guid("4e77fd5f-c5c6-dd50-8dbe-45e28e51d136"),
                column: "IsDeleted",
                value: false);

            migrationBuilder.UpdateData(
                table: "FieldSchedules",
                keyColumn: "ScheduleId",
                keyValue: new Guid("4f064eee-3c20-b35d-8a3c-36a1cc586d82"),
                column: "IsDeleted",
                value: false);

            migrationBuilder.UpdateData(
                table: "FieldSchedules",
                keyColumn: "ScheduleId",
                keyValue: new Guid("527dda9f-78d6-a859-8040-2a31b0ff4f7e"),
                column: "IsDeleted",
                value: false);

            migrationBuilder.UpdateData(
                table: "FieldSchedules",
                keyColumn: "ScheduleId",
                keyValue: new Guid("539eb1a6-0d60-325c-92ce-7d7e4be76a21"),
                column: "IsDeleted",
                value: false);

            migrationBuilder.UpdateData(
                table: "FieldSchedules",
                keyColumn: "ScheduleId",
                keyValue: new Guid("5428d4ab-1280-5d59-9b05-149ce9340e10"),
                column: "IsDeleted",
                value: false);

            migrationBuilder.UpdateData(
                table: "FieldSchedules",
                keyColumn: "ScheduleId",
                keyValue: new Guid("54d30a4f-8b69-c459-83be-f10765e1960b"),
                column: "IsDeleted",
                value: false);

            migrationBuilder.UpdateData(
                table: "FieldSchedules",
                keyColumn: "ScheduleId",
                keyValue: new Guid("55d1fcc5-ac09-b55d-9786-62040e31c0ec"),
                column: "IsDeleted",
                value: false);

            migrationBuilder.UpdateData(
                table: "FieldSchedules",
                keyColumn: "ScheduleId",
                keyValue: new Guid("56d47446-48af-885f-b16f-ddd9e647acaf"),
                column: "IsDeleted",
                value: false);

            migrationBuilder.UpdateData(
                table: "FieldSchedules",
                keyColumn: "ScheduleId",
                keyValue: new Guid("588a89ce-9cd2-4952-b870-a19d103ae652"),
                column: "IsDeleted",
                value: false);

            migrationBuilder.UpdateData(
                table: "FieldSchedules",
                keyColumn: "ScheduleId",
                keyValue: new Guid("593ef78a-d008-5253-9d1a-24429f8bf2e6"),
                column: "IsDeleted",
                value: false);

            migrationBuilder.UpdateData(
                table: "FieldSchedules",
                keyColumn: "ScheduleId",
                keyValue: new Guid("59d57f9a-78ba-9f5e-a1cd-fd4933a9a9d5"),
                column: "IsDeleted",
                value: false);

            migrationBuilder.UpdateData(
                table: "FieldSchedules",
                keyColumn: "ScheduleId",
                keyValue: new Guid("5a3f197e-eb74-1f5a-9706-77345a1f0acb"),
                column: "IsDeleted",
                value: false);

            migrationBuilder.UpdateData(
                table: "FieldSchedules",
                keyColumn: "ScheduleId",
                keyValue: new Guid("5b104e88-7e09-ae50-891d-8ac0a0fb7ab7"),
                column: "IsDeleted",
                value: false);

            migrationBuilder.UpdateData(
                table: "FieldSchedules",
                keyColumn: "ScheduleId",
                keyValue: new Guid("5b4f9e6a-692b-2a55-b93d-2a4ffe143355"),
                column: "IsDeleted",
                value: false);

            migrationBuilder.UpdateData(
                table: "FieldSchedules",
                keyColumn: "ScheduleId",
                keyValue: new Guid("5f3f0704-79f6-5656-b28b-1536b13dd772"),
                column: "IsDeleted",
                value: false);

            migrationBuilder.UpdateData(
                table: "FieldSchedules",
                keyColumn: "ScheduleId",
                keyValue: new Guid("610b506f-8790-905f-aa1d-4b6e18597e1f"),
                column: "IsDeleted",
                value: false);

            migrationBuilder.UpdateData(
                table: "FieldSchedules",
                keyColumn: "ScheduleId",
                keyValue: new Guid("633c559c-12f4-3552-a255-3925f7316fa1"),
                column: "IsDeleted",
                value: false);

            migrationBuilder.UpdateData(
                table: "FieldSchedules",
                keyColumn: "ScheduleId",
                keyValue: new Guid("63dddb67-db53-705c-96bc-54ed180f004e"),
                column: "IsDeleted",
                value: false);

            migrationBuilder.UpdateData(
                table: "FieldSchedules",
                keyColumn: "ScheduleId",
                keyValue: new Guid("63e795bc-4f47-a950-90eb-d9bed900f1d3"),
                column: "IsDeleted",
                value: false);

            migrationBuilder.UpdateData(
                table: "FieldSchedules",
                keyColumn: "ScheduleId",
                keyValue: new Guid("64a29cdd-04e5-d058-9b3b-d7d8f6172c7b"),
                column: "IsDeleted",
                value: false);

            migrationBuilder.UpdateData(
                table: "FieldSchedules",
                keyColumn: "ScheduleId",
                keyValue: new Guid("65552436-6486-1156-9a30-260f1e990808"),
                column: "IsDeleted",
                value: false);

            migrationBuilder.UpdateData(
                table: "FieldSchedules",
                keyColumn: "ScheduleId",
                keyValue: new Guid("66989fa2-de1e-d15e-9cba-19a4f1202d9d"),
                column: "IsDeleted",
                value: false);

            migrationBuilder.UpdateData(
                table: "FieldSchedules",
                keyColumn: "ScheduleId",
                keyValue: new Guid("67460a37-177f-8656-b281-1981ed3d1a0e"),
                column: "IsDeleted",
                value: false);

            migrationBuilder.UpdateData(
                table: "FieldSchedules",
                keyColumn: "ScheduleId",
                keyValue: new Guid("67c4a597-9a61-3353-a269-52cf5d7c2312"),
                column: "IsDeleted",
                value: false);

            migrationBuilder.UpdateData(
                table: "FieldSchedules",
                keyColumn: "ScheduleId",
                keyValue: new Guid("69956a9d-7bd4-cc58-91b7-8f437f22df73"),
                column: "IsDeleted",
                value: false);

            migrationBuilder.UpdateData(
                table: "FieldSchedules",
                keyColumn: "ScheduleId",
                keyValue: new Guid("6c8a7f93-b59a-4752-825f-164bbf47f31f"),
                column: "IsDeleted",
                value: false);

            migrationBuilder.UpdateData(
                table: "FieldSchedules",
                keyColumn: "ScheduleId",
                keyValue: new Guid("6dc8bf32-96b3-bc5a-b704-7fc8441610ed"),
                column: "IsDeleted",
                value: false);

            migrationBuilder.UpdateData(
                table: "FieldSchedules",
                keyColumn: "ScheduleId",
                keyValue: new Guid("6e2503ab-182d-c15f-bfba-aaddd31c952e"),
                column: "IsDeleted",
                value: false);

            migrationBuilder.UpdateData(
                table: "FieldSchedules",
                keyColumn: "ScheduleId",
                keyValue: new Guid("7066d932-76a6-1d53-8134-afce5494d08c"),
                column: "IsDeleted",
                value: false);

            migrationBuilder.UpdateData(
                table: "FieldSchedules",
                keyColumn: "ScheduleId",
                keyValue: new Guid("772773e7-7bb3-0b51-a659-5e23e107bc39"),
                column: "IsDeleted",
                value: false);

            migrationBuilder.UpdateData(
                table: "FieldSchedules",
                keyColumn: "ScheduleId",
                keyValue: new Guid("780f3263-0592-e356-891e-fea44070afc0"),
                column: "IsDeleted",
                value: false);

            migrationBuilder.UpdateData(
                table: "FieldSchedules",
                keyColumn: "ScheduleId",
                keyValue: new Guid("786f33a3-720b-0251-8a68-6df94ecd1c20"),
                column: "IsDeleted",
                value: false);

            migrationBuilder.UpdateData(
                table: "FieldSchedules",
                keyColumn: "ScheduleId",
                keyValue: new Guid("7879c859-6e2f-c950-bd8e-5ea25b505471"),
                column: "IsDeleted",
                value: false);

            migrationBuilder.UpdateData(
                table: "FieldSchedules",
                keyColumn: "ScheduleId",
                keyValue: new Guid("7b246b65-b13f-475b-94b7-852bbf0e6e32"),
                column: "IsDeleted",
                value: false);

            migrationBuilder.UpdateData(
                table: "FieldSchedules",
                keyColumn: "ScheduleId",
                keyValue: new Guid("7bb9ab64-da31-2d52-a2f3-6ec41da82213"),
                column: "IsDeleted",
                value: false);

            migrationBuilder.UpdateData(
                table: "FieldSchedules",
                keyColumn: "ScheduleId",
                keyValue: new Guid("7c3720a7-3309-d05b-9e46-7ec20328c46e"),
                column: "IsDeleted",
                value: false);

            migrationBuilder.UpdateData(
                table: "FieldSchedules",
                keyColumn: "ScheduleId",
                keyValue: new Guid("7d7db3d7-f818-ae51-b732-6ef289ff7d45"),
                column: "IsDeleted",
                value: false);

            migrationBuilder.UpdateData(
                table: "FieldSchedules",
                keyColumn: "ScheduleId",
                keyValue: new Guid("7d9fe12c-0cd9-735d-aa32-c9c233b49d23"),
                column: "IsDeleted",
                value: false);

            migrationBuilder.UpdateData(
                table: "FieldSchedules",
                keyColumn: "ScheduleId",
                keyValue: new Guid("7df2dd95-fd28-cf56-b22e-0d179be410ef"),
                column: "IsDeleted",
                value: false);

            migrationBuilder.UpdateData(
                table: "FieldSchedules",
                keyColumn: "ScheduleId",
                keyValue: new Guid("7e31cc84-425d-cd57-abd7-64b433384b6a"),
                column: "IsDeleted",
                value: false);

            migrationBuilder.UpdateData(
                table: "FieldSchedules",
                keyColumn: "ScheduleId",
                keyValue: new Guid("80188555-31b7-175d-b409-03eed0aa6d1a"),
                column: "IsDeleted",
                value: false);

            migrationBuilder.UpdateData(
                table: "FieldSchedules",
                keyColumn: "ScheduleId",
                keyValue: new Guid("81a3ffa3-afb5-5c55-a64d-1a0b63493570"),
                column: "IsDeleted",
                value: false);

            migrationBuilder.UpdateData(
                table: "FieldSchedules",
                keyColumn: "ScheduleId",
                keyValue: new Guid("82ef65a3-ab2c-8d53-b7e0-b5e456f2574a"),
                column: "IsDeleted",
                value: false);

            migrationBuilder.UpdateData(
                table: "FieldSchedules",
                keyColumn: "ScheduleId",
                keyValue: new Guid("864938f0-9e90-bc5c-8acf-79187c58fc6b"),
                column: "IsDeleted",
                value: false);

            migrationBuilder.UpdateData(
                table: "FieldSchedules",
                keyColumn: "ScheduleId",
                keyValue: new Guid("879d326f-a897-565c-8f5c-8b9411a80c1d"),
                column: "IsDeleted",
                value: false);

            migrationBuilder.UpdateData(
                table: "FieldSchedules",
                keyColumn: "ScheduleId",
                keyValue: new Guid("8aa882d1-2d84-1855-8da2-a9269e73e48c"),
                column: "IsDeleted",
                value: false);

            migrationBuilder.UpdateData(
                table: "FieldSchedules",
                keyColumn: "ScheduleId",
                keyValue: new Guid("8bfabdff-2cdd-545b-b6a0-cb0396b1d7b6"),
                column: "IsDeleted",
                value: false);

            migrationBuilder.UpdateData(
                table: "FieldSchedules",
                keyColumn: "ScheduleId",
                keyValue: new Guid("8c02bc89-c6b5-3352-b3e5-40b3b1f6a085"),
                column: "IsDeleted",
                value: false);

            migrationBuilder.UpdateData(
                table: "FieldSchedules",
                keyColumn: "ScheduleId",
                keyValue: new Guid("8cf9b6e5-4a80-a95e-859c-7ef0fab53876"),
                column: "IsDeleted",
                value: false);

            migrationBuilder.UpdateData(
                table: "FieldSchedules",
                keyColumn: "ScheduleId",
                keyValue: new Guid("8e14f727-fe53-995a-8e33-3d5304db7075"),
                column: "IsDeleted",
                value: false);

            migrationBuilder.UpdateData(
                table: "FieldSchedules",
                keyColumn: "ScheduleId",
                keyValue: new Guid("91c27232-67b0-7659-a0db-a62d4d3a1700"),
                column: "IsDeleted",
                value: false);

            migrationBuilder.UpdateData(
                table: "FieldSchedules",
                keyColumn: "ScheduleId",
                keyValue: new Guid("93234eab-d76c-be51-b7fb-e042e18c9f5f"),
                column: "IsDeleted",
                value: false);

            migrationBuilder.UpdateData(
                table: "FieldSchedules",
                keyColumn: "ScheduleId",
                keyValue: new Guid("97ef332a-c283-bf51-ab92-89621d15d70f"),
                column: "IsDeleted",
                value: false);

            migrationBuilder.UpdateData(
                table: "FieldSchedules",
                keyColumn: "ScheduleId",
                keyValue: new Guid("9a9bde9b-7505-005e-a0cb-4bf84036d55b"),
                column: "IsDeleted",
                value: false);

            migrationBuilder.UpdateData(
                table: "FieldSchedules",
                keyColumn: "ScheduleId",
                keyValue: new Guid("9d0fcc7d-a12c-4950-8b75-21f084034310"),
                column: "IsDeleted",
                value: false);

            migrationBuilder.UpdateData(
                table: "FieldSchedules",
                keyColumn: "ScheduleId",
                keyValue: new Guid("a21996a1-a03d-725a-b1e3-330749c954c7"),
                column: "IsDeleted",
                value: false);

            migrationBuilder.UpdateData(
                table: "FieldSchedules",
                keyColumn: "ScheduleId",
                keyValue: new Guid("a2607424-e063-1254-a612-f03832cf753e"),
                column: "IsDeleted",
                value: false);

            migrationBuilder.UpdateData(
                table: "FieldSchedules",
                keyColumn: "ScheduleId",
                keyValue: new Guid("a289c1f6-5a69-ef5b-ad1e-c7c78e9eaaea"),
                column: "IsDeleted",
                value: false);

            migrationBuilder.UpdateData(
                table: "FieldSchedules",
                keyColumn: "ScheduleId",
                keyValue: new Guid("a5248dab-4b10-5d5d-b5d7-f4921d42a550"),
                column: "IsDeleted",
                value: false);

            migrationBuilder.UpdateData(
                table: "FieldSchedules",
                keyColumn: "ScheduleId",
                keyValue: new Guid("a53f91db-0ae2-ae59-b931-d421c634d09c"),
                column: "IsDeleted",
                value: false);

            migrationBuilder.UpdateData(
                table: "FieldSchedules",
                keyColumn: "ScheduleId",
                keyValue: new Guid("a92fcd8f-8540-b852-bdc0-e8067f833cbb"),
                column: "IsDeleted",
                value: false);

            migrationBuilder.UpdateData(
                table: "FieldSchedules",
                keyColumn: "ScheduleId",
                keyValue: new Guid("acb8da59-e794-295d-85d7-66d5d296b9cc"),
                column: "IsDeleted",
                value: false);

            migrationBuilder.UpdateData(
                table: "FieldSchedules",
                keyColumn: "ScheduleId",
                keyValue: new Guid("ad560fe3-f427-815c-9176-fbb5d2ce9e2f"),
                column: "IsDeleted",
                value: false);

            migrationBuilder.UpdateData(
                table: "FieldSchedules",
                keyColumn: "ScheduleId",
                keyValue: new Guid("ae9b3e33-1b09-c354-ae20-95fdb49c3d9f"),
                column: "IsDeleted",
                value: false);

            migrationBuilder.UpdateData(
                table: "FieldSchedules",
                keyColumn: "ScheduleId",
                keyValue: new Guid("aff550e2-478f-3b58-8f70-538553ee317e"),
                column: "IsDeleted",
                value: false);

            migrationBuilder.UpdateData(
                table: "FieldSchedules",
                keyColumn: "ScheduleId",
                keyValue: new Guid("b32ce2d8-7484-915b-b59a-a41337c6d401"),
                column: "IsDeleted",
                value: false);

            migrationBuilder.UpdateData(
                table: "FieldSchedules",
                keyColumn: "ScheduleId",
                keyValue: new Guid("b3b28fb6-258c-8657-8b0b-2061fe1c9873"),
                column: "IsDeleted",
                value: false);

            migrationBuilder.UpdateData(
                table: "FieldSchedules",
                keyColumn: "ScheduleId",
                keyValue: new Guid("b50bc7cc-38df-0a50-ade0-9d9c9058a6eb"),
                column: "IsDeleted",
                value: false);

            migrationBuilder.UpdateData(
                table: "FieldSchedules",
                keyColumn: "ScheduleId",
                keyValue: new Guid("b527e481-38fe-d958-ab84-68ed79a0efec"),
                column: "IsDeleted",
                value: false);

            migrationBuilder.UpdateData(
                table: "FieldSchedules",
                keyColumn: "ScheduleId",
                keyValue: new Guid("b9aba5fb-afd6-8a52-b4e2-04352c916e32"),
                column: "IsDeleted",
                value: false);

            migrationBuilder.UpdateData(
                table: "FieldSchedules",
                keyColumn: "ScheduleId",
                keyValue: new Guid("b9b52451-bfa3-4d5e-ab23-a1b7764a4b16"),
                column: "IsDeleted",
                value: false);

            migrationBuilder.UpdateData(
                table: "FieldSchedules",
                keyColumn: "ScheduleId",
                keyValue: new Guid("ba45dad0-996c-175e-9ba5-cc7543ccac71"),
                column: "IsDeleted",
                value: false);

            migrationBuilder.UpdateData(
                table: "FieldSchedules",
                keyColumn: "ScheduleId",
                keyValue: new Guid("ba5b72d7-029a-975a-9ed2-cc19bd252542"),
                column: "IsDeleted",
                value: false);

            migrationBuilder.UpdateData(
                table: "FieldSchedules",
                keyColumn: "ScheduleId",
                keyValue: new Guid("baa7493c-3008-9557-ac1d-c1bccd82162f"),
                column: "IsDeleted",
                value: false);

            migrationBuilder.UpdateData(
                table: "FieldSchedules",
                keyColumn: "ScheduleId",
                keyValue: new Guid("bb9daa27-a51e-c956-ad98-a3c248e59567"),
                column: "IsDeleted",
                value: false);

            migrationBuilder.UpdateData(
                table: "FieldSchedules",
                keyColumn: "ScheduleId",
                keyValue: new Guid("bbb58f04-9698-0b58-8bc9-51f1a8611a06"),
                column: "IsDeleted",
                value: false);

            migrationBuilder.UpdateData(
                table: "FieldSchedules",
                keyColumn: "ScheduleId",
                keyValue: new Guid("bf5ba8f9-7fa6-8c5f-863f-06320b7cf70d"),
                column: "IsDeleted",
                value: false);

            migrationBuilder.UpdateData(
                table: "FieldSchedules",
                keyColumn: "ScheduleId",
                keyValue: new Guid("c046f81b-e93f-035e-8b97-d30855278d83"),
                column: "IsDeleted",
                value: false);

            migrationBuilder.UpdateData(
                table: "FieldSchedules",
                keyColumn: "ScheduleId",
                keyValue: new Guid("c1a43f0d-c849-c15b-a4cb-271bd8044d07"),
                column: "IsDeleted",
                value: false);

            migrationBuilder.UpdateData(
                table: "FieldSchedules",
                keyColumn: "ScheduleId",
                keyValue: new Guid("c271bea2-3c47-7d57-88b9-0e5afe7625f2"),
                column: "IsDeleted",
                value: false);

            migrationBuilder.UpdateData(
                table: "FieldSchedules",
                keyColumn: "ScheduleId",
                keyValue: new Guid("c35e865c-88b3-d859-ba20-c09b559b6083"),
                column: "IsDeleted",
                value: false);

            migrationBuilder.UpdateData(
                table: "FieldSchedules",
                keyColumn: "ScheduleId",
                keyValue: new Guid("c4272481-d40f-9250-90c8-5f3ece10ae8f"),
                column: "IsDeleted",
                value: false);

            migrationBuilder.UpdateData(
                table: "FieldSchedules",
                keyColumn: "ScheduleId",
                keyValue: new Guid("c5445474-dda9-0153-a0bf-646172bf3cc1"),
                column: "IsDeleted",
                value: false);

            migrationBuilder.UpdateData(
                table: "FieldSchedules",
                keyColumn: "ScheduleId",
                keyValue: new Guid("c7770ca5-1627-7c56-a799-8261e05bf970"),
                column: "IsDeleted",
                value: false);

            migrationBuilder.UpdateData(
                table: "FieldSchedules",
                keyColumn: "ScheduleId",
                keyValue: new Guid("c7dae73c-4e76-a05d-8f77-d208136ce219"),
                column: "IsDeleted",
                value: false);

            migrationBuilder.UpdateData(
                table: "FieldSchedules",
                keyColumn: "ScheduleId",
                keyValue: new Guid("c9dcd61c-8a78-9b57-8648-cff23d08e47b"),
                column: "IsDeleted",
                value: false);

            migrationBuilder.UpdateData(
                table: "FieldSchedules",
                keyColumn: "ScheduleId",
                keyValue: new Guid("cb24ab40-0ab5-3a53-955c-79d09ee72f30"),
                column: "IsDeleted",
                value: false);

            migrationBuilder.UpdateData(
                table: "FieldSchedules",
                keyColumn: "ScheduleId",
                keyValue: new Guid("ce7f2faa-03aa-e85a-8d99-c20616b94096"),
                column: "IsDeleted",
                value: false);

            migrationBuilder.UpdateData(
                table: "FieldSchedules",
                keyColumn: "ScheduleId",
                keyValue: new Guid("ceacae8c-4aca-345b-9e04-e199fcb29885"),
                column: "IsDeleted",
                value: false);

            migrationBuilder.UpdateData(
                table: "FieldSchedules",
                keyColumn: "ScheduleId",
                keyValue: new Guid("d1f9dab5-1451-7553-a423-5fbd34a89f42"),
                column: "IsDeleted",
                value: false);

            migrationBuilder.UpdateData(
                table: "FieldSchedules",
                keyColumn: "ScheduleId",
                keyValue: new Guid("d222db0a-b926-c059-ac5a-eeb800dc824c"),
                column: "IsDeleted",
                value: false);

            migrationBuilder.UpdateData(
                table: "FieldSchedules",
                keyColumn: "ScheduleId",
                keyValue: new Guid("d24a0898-73cf-9b54-a396-b93b8dd3ae88"),
                column: "IsDeleted",
                value: false);

            migrationBuilder.UpdateData(
                table: "FieldSchedules",
                keyColumn: "ScheduleId",
                keyValue: new Guid("d33f3b98-ecdf-b458-9df1-ad6e3bb0dcd1"),
                column: "IsDeleted",
                value: false);

            migrationBuilder.UpdateData(
                table: "FieldSchedules",
                keyColumn: "ScheduleId",
                keyValue: new Guid("d4446d01-757e-f956-8fb3-fd36860caaab"),
                column: "IsDeleted",
                value: false);

            migrationBuilder.UpdateData(
                table: "FieldSchedules",
                keyColumn: "ScheduleId",
                keyValue: new Guid("d9ba2c7d-208a-0659-b9f6-6212c2eff4e5"),
                column: "IsDeleted",
                value: false);

            migrationBuilder.UpdateData(
                table: "FieldSchedules",
                keyColumn: "ScheduleId",
                keyValue: new Guid("dc9b43da-5c13-cd53-9d03-c4d06f6c3a7a"),
                column: "IsDeleted",
                value: false);

            migrationBuilder.UpdateData(
                table: "FieldSchedules",
                keyColumn: "ScheduleId",
                keyValue: new Guid("dd990614-c088-c155-9629-b4069161464f"),
                column: "IsDeleted",
                value: false);

            migrationBuilder.UpdateData(
                table: "FieldSchedules",
                keyColumn: "ScheduleId",
                keyValue: new Guid("df9c0640-9704-df51-9a81-1dd4017b60a1"),
                column: "IsDeleted",
                value: false);

            migrationBuilder.UpdateData(
                table: "FieldSchedules",
                keyColumn: "ScheduleId",
                keyValue: new Guid("e0512a68-812a-9556-8099-a5bb77d1c255"),
                column: "IsDeleted",
                value: false);

            migrationBuilder.UpdateData(
                table: "FieldSchedules",
                keyColumn: "ScheduleId",
                keyValue: new Guid("e2948d06-2792-2456-877f-5b51c511f664"),
                column: "IsDeleted",
                value: false);

            migrationBuilder.UpdateData(
                table: "FieldSchedules",
                keyColumn: "ScheduleId",
                keyValue: new Guid("e430a4fc-b36e-e055-9c82-0e0872af2bb8"),
                column: "IsDeleted",
                value: false);

            migrationBuilder.UpdateData(
                table: "FieldSchedules",
                keyColumn: "ScheduleId",
                keyValue: new Guid("e487cec2-6f0b-ba5a-8955-bf4ca4282268"),
                column: "IsDeleted",
                value: false);

            migrationBuilder.UpdateData(
                table: "FieldSchedules",
                keyColumn: "ScheduleId",
                keyValue: new Guid("e7261244-a1f4-115f-93b2-3b750a63b2fc"),
                column: "IsDeleted",
                value: false);

            migrationBuilder.UpdateData(
                table: "FieldSchedules",
                keyColumn: "ScheduleId",
                keyValue: new Guid("e8cf149d-e517-3d5b-bd0a-a4fed7d79c2c"),
                column: "IsDeleted",
                value: false);

            migrationBuilder.UpdateData(
                table: "FieldSchedules",
                keyColumn: "ScheduleId",
                keyValue: new Guid("e9188ed7-2e1a-7557-9d01-c6b1327b680a"),
                column: "IsDeleted",
                value: false);

            migrationBuilder.UpdateData(
                table: "FieldSchedules",
                keyColumn: "ScheduleId",
                keyValue: new Guid("eab81cf5-7be8-5258-b3f6-e461a46bd8f9"),
                column: "IsDeleted",
                value: false);

            migrationBuilder.UpdateData(
                table: "FieldSchedules",
                keyColumn: "ScheduleId",
                keyValue: new Guid("eb7504d5-cf54-015a-b367-f584c78767eb"),
                column: "IsDeleted",
                value: false);

            migrationBuilder.UpdateData(
                table: "FieldSchedules",
                keyColumn: "ScheduleId",
                keyValue: new Guid("ed5488e2-e404-8e54-8773-8dbf87e9fc8e"),
                column: "IsDeleted",
                value: false);

            migrationBuilder.UpdateData(
                table: "FieldSchedules",
                keyColumn: "ScheduleId",
                keyValue: new Guid("ef2a63c6-3fcf-3158-bab5-cf60fd1253d8"),
                column: "IsDeleted",
                value: false);

            migrationBuilder.UpdateData(
                table: "FieldSchedules",
                keyColumn: "ScheduleId",
                keyValue: new Guid("f2d9d27f-6117-bd5f-9819-b3c3a4496f0e"),
                column: "IsDeleted",
                value: false);

            migrationBuilder.UpdateData(
                table: "FieldSchedules",
                keyColumn: "ScheduleId",
                keyValue: new Guid("f6cb6d87-97b4-8b56-8806-0ac6f374ead1"),
                column: "IsDeleted",
                value: false);

            migrationBuilder.UpdateData(
                table: "FieldSchedules",
                keyColumn: "ScheduleId",
                keyValue: new Guid("f75bac56-b68b-9b58-973f-9f9db814999a"),
                column: "IsDeleted",
                value: false);

            migrationBuilder.UpdateData(
                table: "FieldSchedules",
                keyColumn: "ScheduleId",
                keyValue: new Guid("f8d023f7-42e5-845d-a11f-f8dbf376c270"),
                column: "IsDeleted",
                value: false);

            migrationBuilder.UpdateData(
                table: "FieldSchedules",
                keyColumn: "ScheduleId",
                keyValue: new Guid("f94c8a69-c7cf-5a50-b268-2eb56af6b12c"),
                column: "IsDeleted",
                value: false);

            migrationBuilder.UpdateData(
                table: "FieldSchedules",
                keyColumn: "ScheduleId",
                keyValue: new Guid("feb08943-7f26-6b54-b4bb-363483be254b"),
                column: "IsDeleted",
                value: false);

            migrationBuilder.UpdateData(
                table: "FieldSchedules",
                keyColumn: "ScheduleId",
                keyValue: new Guid("ff88ddea-c2d7-f452-a370-5f0092c07339"),
                column: "IsDeleted",
                value: false);

            migrationBuilder.UpdateData(
                table: "FieldSchedules",
                keyColumn: "ScheduleId",
                keyValue: new Guid("ffc9ce3c-834e-775e-a7e3-ad3bd1a08672"),
                column: "IsDeleted",
                value: false);

            migrationBuilder.UpdateData(
                table: "NotificationRecipients",
                keyColumn: "RecipientId",
                keyValue: new Guid("2202b536-add3-c75e-b4fd-cb421a5b807f"),
                column: "IsDeleted",
                value: false);

            migrationBuilder.UpdateData(
                table: "NotificationRecipients",
                keyColumn: "RecipientId",
                keyValue: new Guid("25f572cf-feed-f45e-8cad-9649a06f15f5"),
                column: "IsDeleted",
                value: false);

            migrationBuilder.UpdateData(
                table: "NotificationRecipients",
                keyColumn: "RecipientId",
                keyValue: new Guid("29b7bbef-32cd-2b5d-bcd3-f326e1a06d67"),
                column: "IsDeleted",
                value: false);

            migrationBuilder.UpdateData(
                table: "NotificationRecipients",
                keyColumn: "RecipientId",
                keyValue: new Guid("319efd5e-d6ec-cb5a-a5d7-e0ce3c0e9d00"),
                column: "IsDeleted",
                value: false);

            migrationBuilder.UpdateData(
                table: "NotificationRecipients",
                keyColumn: "RecipientId",
                keyValue: new Guid("53489e62-b880-0c50-9bd3-4708d5c7a68e"),
                column: "IsDeleted",
                value: false);

            migrationBuilder.UpdateData(
                table: "NotificationRecipients",
                keyColumn: "RecipientId",
                keyValue: new Guid("63d03e2c-b6a7-5657-846d-c575b002ba23"),
                column: "IsDeleted",
                value: false);

            migrationBuilder.UpdateData(
                table: "NotificationRecipients",
                keyColumn: "RecipientId",
                keyValue: new Guid("6a496349-045a-e851-865c-438c077c10a6"),
                column: "IsDeleted",
                value: false);

            migrationBuilder.UpdateData(
                table: "NotificationRecipients",
                keyColumn: "RecipientId",
                keyValue: new Guid("ab334594-f033-465f-bbc2-b3d9c315990a"),
                column: "IsDeleted",
                value: false);

            migrationBuilder.UpdateData(
                table: "NotificationRecipients",
                keyColumn: "RecipientId",
                keyValue: new Guid("c49fadcb-01f3-6154-985b-58493f27b254"),
                column: "IsDeleted",
                value: false);

            migrationBuilder.UpdateData(
                table: "NotificationRecipients",
                keyColumn: "RecipientId",
                keyValue: new Guid("cb487dbe-c95b-d754-987c-9c6b7ee3e90c"),
                column: "IsDeleted",
                value: false);

            migrationBuilder.UpdateData(
                table: "NotificationRecipients",
                keyColumn: "RecipientId",
                keyValue: new Guid("e0f1866e-bfb5-e250-b3ad-6951a58d7b9c"),
                column: "IsDeleted",
                value: false);

            migrationBuilder.UpdateData(
                table: "NotificationRecipients",
                keyColumn: "RecipientId",
                keyValue: new Guid("f2b34149-15fe-bb5b-992a-de4d4834bd85"),
                column: "IsDeleted",
                value: false);

            migrationBuilder.UpdateData(
                table: "UserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { new Guid("76075424-3dac-6259-a0f7-00a4c6c20191"), new Guid("28d446ef-917b-8b59-a814-da2a00b0b76f") },
                column: "IsDeleted",
                value: false);

            migrationBuilder.UpdateData(
                table: "UserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { new Guid("07371171-eec1-3255-b1b2-1d8e8e81ede7"), new Guid("2f958e63-14a1-ee5f-b359-e923bbd70ece") },
                column: "IsDeleted",
                value: false);

            migrationBuilder.UpdateData(
                table: "UserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { new Guid("07371171-eec1-3255-b1b2-1d8e8e81ede7"), new Guid("49f942ec-d197-7c5c-a011-6454ca64ec2c") },
                column: "IsDeleted",
                value: false);

            migrationBuilder.UpdateData(
                table: "UserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { new Guid("76075424-3dac-6259-a0f7-00a4c6c20191"), new Guid("81d10681-e36e-595b-972a-f441c8237537") },
                column: "IsDeleted",
                value: false);

            migrationBuilder.UpdateData(
                table: "UserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { new Guid("b5abbaf1-931c-5353-b9ab-1f38eb30b8b8"), new Guid("b41aae5d-9596-9a5d-b8e5-0f8b199a8135") },
                column: "IsDeleted",
                value: false);

            migrationBuilder.UpdateData(
                table: "UserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { new Guid("76075424-3dac-6259-a0f7-00a4c6c20191"), new Guid("b53af497-39fc-6351-a424-0a0063d43116") },
                column: "IsDeleted",
                value: false);

            migrationBuilder.UpdateData(
                table: "UserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { new Guid("76075424-3dac-6259-a0f7-00a4c6c20191"), new Guid("cbfe125b-7a8c-335c-aa61-df49f35c448f") },
                column: "IsDeleted",
                value: false);

            migrationBuilder.UpdateData(
                table: "UserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { new Guid("76075424-3dac-6259-a0f7-00a4c6c20191"), new Guid("de68f3de-ceab-c85f-b54a-645613f6a13e") },
                column: "IsDeleted",
                value: false);

            migrationBuilder.UpdateData(
                table: "UserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { new Guid("b5abbaf1-931c-5353-b9ab-1f38eb30b8b8"), new Guid("e3266388-5d3f-c459-beef-1edc2d465a3e") },
                column: "IsDeleted",
                value: false);

            migrationBuilder.UpdateData(
                table: "UserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { new Guid("07371171-eec1-3255-b1b2-1d8e8e81ede7"), new Guid("ef0c12c5-0bcf-4e5f-a13a-4b01b2ed44fc") },
                column: "IsDeleted",
                value: false);

            migrationBuilder.UpdateData(
                table: "UserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { new Guid("76075424-3dac-6259-a0f7-00a4c6c20191"), new Guid("eff1cca4-9f7a-0f53-a3e0-115f934fc55b") },
                column: "IsDeleted",
                value: false);

            migrationBuilder.UpdateData(
                table: "UserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { new Guid("07371171-eec1-3255-b1b2-1d8e8e81ede7"), new Guid("ff182b52-5005-895d-a90a-224ef11c5e61") },
                column: "IsDeleted",
                value: false);

            migrationBuilder.CreateIndex(
                name: "IX_NotificationRecipients_NotificationId_UserId",
                table: "NotificationRecipients",
                columns: new[] { "NotificationId", "UserId" },
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_BookingDiscounts_Discounts_DiscountId",
                table: "BookingDiscounts",
                column: "DiscountId",
                principalTable: "Discounts",
                principalColumn: "DiscountId",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BookingDiscounts_Discounts_DiscountId",
                table: "BookingDiscounts");

            migrationBuilder.DropIndex(
                name: "IX_NotificationRecipients_NotificationId_UserId",
                table: "NotificationRecipients");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "UserRoles");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "UserDevices");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "NotificationRecipients");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "FieldSchedules");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "BookingDiscounts");

            migrationBuilder.AlterColumn<decimal>(
                name: "DiscountAmount",
                table: "BookingDiscounts",
                type: "numeric",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric(10,2)",
                oldPrecision: 10,
                oldScale: 2);

            migrationBuilder.CreateIndex(
                name: "IX_NotificationRecipients_NotificationId",
                table: "NotificationRecipients",
                column: "NotificationId");

            migrationBuilder.AddForeignKey(
                name: "FK_BookingDiscounts_Discounts_DiscountId",
                table: "BookingDiscounts",
                column: "DiscountId",
                principalTable: "Discounts",
                principalColumn: "DiscountId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
