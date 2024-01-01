using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Major_BeanScene_ReservationSystem.Migrations
{
    /// <inheritdoc />
    public partial class PeopleTablePleaseWork : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "a9a3baff-c74b-4c43-886f-e1f198f73701");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "bfc688c7-26ed-4565-ad6e-061782030aae");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "e1cbb1b6-a3ca-458f-9130-1405f7c4a9e0");

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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
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
                    { "a9a3baff-c74b-4c43-886f-e1f198f73701", null, "Manager", "MANAGER" },
                    { "bfc688c7-26ed-4565-ad6e-061782030aae", null, "Staff", "STAFF" },
                    { "e1cbb1b6-a3ca-458f-9130-1405f7c4a9e0", null, "Member", "MEMBER" }
                });
        }
    }
}
