using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HotelListingApi.Migrations
{
    /// <inheritdoc />
    public partial class UpdateUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AccessFailedCount",
                table: "ApplicationUser");

            migrationBuilder.DropColumn(
                name: "ConcurrencyStamp",
                table: "ApplicationUser");

            migrationBuilder.DropColumn(
                name: "EmailConfirmed",
                table: "ApplicationUser");

            migrationBuilder.DropColumn(
                name: "LockoutEnabled",
                table: "ApplicationUser");

            migrationBuilder.DropColumn(
                name: "LockoutEnd",
                table: "ApplicationUser");

            migrationBuilder.DropColumn(
                name: "NormalizedEmail",
                table: "ApplicationUser");

            migrationBuilder.DropColumn(
                name: "NormalizedUserName",
                table: "ApplicationUser");

            migrationBuilder.DropColumn(
                name: "PasswordHash",
                table: "ApplicationUser");

            migrationBuilder.DropColumn(
                name: "PhoneNumber",
                table: "ApplicationUser");

            migrationBuilder.DropColumn(
                name: "PhoneNumberConfirmed",
                table: "ApplicationUser");

            migrationBuilder.DropColumn(
                name: "SecurityStamp",
                table: "ApplicationUser");

            migrationBuilder.DropColumn(
                name: "TwoFactorEnabled",
                table: "ApplicationUser");

            migrationBuilder.DropColumn(
                name: "UserName",
                table: "ApplicationUser");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "ApplicationUser",
                newName: "ApplicationUserId");

            migrationBuilder.AlterColumn<string>(
                name: "Email",
                table: "ApplicationUser",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ApplicationUserId",
                table: "ApplicationUser",
                newName: "Id");

            migrationBuilder.AlterColumn<string>(
                name: "Email",
                table: "ApplicationUser",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<int>(
                name: "AccessFailedCount",
                table: "ApplicationUser",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "ConcurrencyStamp",
                table: "ApplicationUser",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "EmailConfirmed",
                table: "ApplicationUser",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "LockoutEnabled",
                table: "ApplicationUser",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "LockoutEnd",
                table: "ApplicationUser",
                type: "datetimeoffset",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "NormalizedEmail",
                table: "ApplicationUser",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "NormalizedUserName",
                table: "ApplicationUser",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PasswordHash",
                table: "ApplicationUser",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PhoneNumber",
                table: "ApplicationUser",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "PhoneNumberConfirmed",
                table: "ApplicationUser",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "SecurityStamp",
                table: "ApplicationUser",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "TwoFactorEnabled",
                table: "ApplicationUser",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "UserName",
                table: "ApplicationUser",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
