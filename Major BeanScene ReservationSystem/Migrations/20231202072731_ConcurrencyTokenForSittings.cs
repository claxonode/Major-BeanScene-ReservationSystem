using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Major_BeanScene_ReservationSystem.Migrations
{
    /// <inheritdoc />
    public partial class ConcurrencyTokenForSittings : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "3c001b0f-1826-43d2-b8d8-1d08ec582535");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "5e0cecc6-65e1-4afa-80a4-5979ef80a8e0");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "e3c22e8a-d0ad-4590-b532-40a556457769");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "106a890d-45ca-4969-8f0c-ec32b43afaaf", null, "Staff", "STAFF" },
                    { "56fddaba-9fc2-4f24-ac88-fedfe7a6f017", null, "Member", "MEMBER" },
                    { "e174f904-8ebb-462c-809e-dd5e7badb2ba", null, "Manager", "MANAGER" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "106a890d-45ca-4969-8f0c-ec32b43afaaf");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "56fddaba-9fc2-4f24-ac88-fedfe7a6f017");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "e174f904-8ebb-462c-809e-dd5e7badb2ba");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "3c001b0f-1826-43d2-b8d8-1d08ec582535", null, "Staff", "STAFF" },
                    { "5e0cecc6-65e1-4afa-80a4-5979ef80a8e0", null, "Manager", "MANAGER" },
                    { "e3c22e8a-d0ad-4590-b532-40a556457769", null, "Member", "MEMBER" }
                });
        }
    }
}
