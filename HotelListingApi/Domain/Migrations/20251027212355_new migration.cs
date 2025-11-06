using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HotelListingApi.Migrations
{
    /// <inheritdoc />
    public partial class newmigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "IdentityRole",
                keyColumn: "Id",
                keyValue: "1",
                column: "ConcurrencyStamp",
                value: "d6e8f9a5-7d3a-4b10-b8da-0b2dc20e1c1a");

            migrationBuilder.UpdateData(
                table: "IdentityRole",
                keyColumn: "Id",
                keyValue: "2",
                column: "ConcurrencyStamp",
                value: "4d5e3b9c-8a1f-4e5a-bf9d-53c7a68ed019");

            migrationBuilder.UpdateData(
                table: "IdentityRole",
                keyColumn: "Id",
                keyValue: "3",
                column: "ConcurrencyStamp",
                value: "77ccffb4-6b6e-4b77-8c0a-75fcd2186f88");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "IdentityRole",
                keyColumn: "Id",
                keyValue: "1",
                column: "ConcurrencyStamp",
                value: "de362a34-a7f7-4cec-b4df-88c1916373ed");

            migrationBuilder.UpdateData(
                table: "IdentityRole",
                keyColumn: "Id",
                keyValue: "2",
                column: "ConcurrencyStamp",
                value: "a1bd4b20-d120-4210-b6b9-1c5f1c58f895");

            migrationBuilder.UpdateData(
                table: "IdentityRole",
                keyColumn: "Id",
                keyValue: "3",
                column: "ConcurrencyStamp",
                value: "fafa1b3f-2f9d-458f-9c2f-cfa153ce119f");
        }
    }
}
