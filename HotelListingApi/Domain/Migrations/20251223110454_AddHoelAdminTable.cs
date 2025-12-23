using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HotelListingApi.Migrations
{
    /// <inheritdoc />
    public partial class AddHoelAdminTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ApplicationUserId",
                table: "HotelAdmins",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_HotelAdmins_ApplicationUserId",
                table: "HotelAdmins",
                column: "ApplicationUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_HotelAdmins_ApplicationUser_ApplicationUserId",
                table: "HotelAdmins",
                column: "ApplicationUserId",
                principalTable: "ApplicationUser",
                principalColumn: "ApplicationUserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_HotelAdmins_ApplicationUser_ApplicationUserId",
                table: "HotelAdmins");

            migrationBuilder.DropIndex(
                name: "IX_HotelAdmins_ApplicationUserId",
                table: "HotelAdmins");

            migrationBuilder.DropColumn(
                name: "ApplicationUserId",
                table: "HotelAdmins");
        }
    }
}
