using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyWishList.Web.Migrations
{
    /// <inheritdoc />
    public partial class RegistryV1Enhancements : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "CashFundGoal",
                table: "Wishlists",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "CashFundRaised",
                table: "Wishlists",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "RegistryType",
                table: "Wishlists",
                type: "nvarchar(40)",
                maxLength: 40,
                nullable: false,
                defaultValue: "General");

            migrationBuilder.AddColumn<string>(
                name: "ShareToken",
                table: "Wishlists",
                type: "nvarchar(64)",
                maxLength: 64,
                nullable: false,
                defaultValueSql: "REPLACE(CONVERT(nvarchar(36), NEWID()), '-', '')");

            migrationBuilder.AddColumn<string>(
                name: "Visibility",
                table: "Wishlists",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "Private");

            migrationBuilder.AddColumn<bool>(
                name: "IsReserved",
                table: "Items",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "ReservedAtUtc",
                table: "Items",
                type: "datetimeoffset",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ReservedByName",
                table: "Items",
                type: "nvarchar(120)",
                maxLength: 120,
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Wishlists_ShareToken",
                table: "Wishlists",
                column: "ShareToken",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Wishlists_ShareToken",
                table: "Wishlists");

            migrationBuilder.DropColumn(
                name: "CashFundGoal",
                table: "Wishlists");

            migrationBuilder.DropColumn(
                name: "CashFundRaised",
                table: "Wishlists");

            migrationBuilder.DropColumn(
                name: "RegistryType",
                table: "Wishlists");

            migrationBuilder.DropColumn(
                name: "ShareToken",
                table: "Wishlists");

            migrationBuilder.DropColumn(
                name: "Visibility",
                table: "Wishlists");

            migrationBuilder.DropColumn(
                name: "IsReserved",
                table: "Items");

            migrationBuilder.DropColumn(
                name: "ReservedAtUtc",
                table: "Items");

            migrationBuilder.DropColumn(
                name: "ReservedByName",
                table: "Items");
        }
    }
}
