using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyWishList.Web.Migrations
{
    /// <inheritdoc />
    public partial class FullFeatureExpansion : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "Wishlists",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: true);

            migrationBuilder.AddColumn<DateOnly>(
                name: "EventDate",
                table: "Wishlists",
                type: "date",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsPurchased",
                table: "Items",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsReceived",
                table: "Items",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "PurchasedAtUtc",
                table: "Items",
                type: "datetimeoffset",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PurchasedByName",
                table: "Items",
                type: "nvarchar(120)",
                maxLength: 120,
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "ReceivedAtUtc",
                table: "Items",
                type: "datetimeoffset",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "CashContributions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    WishlistId = table.Column<int>(type: "int", nullable: false),
                    Provider = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    ContributorName = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: false),
                    ContributorEmail = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Message = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: true),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    ExternalReference = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    CreatedAtUtc = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CashContributions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CashContributions_Wishlists_WishlistId",
                        column: x => x.WishlistId,
                        principalTable: "Wishlists",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CashContributions_WishlistId",
                table: "CashContributions",
                column: "WishlistId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CashContributions");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "Wishlists");

            migrationBuilder.DropColumn(
                name: "EventDate",
                table: "Wishlists");

            migrationBuilder.DropColumn(
                name: "IsPurchased",
                table: "Items");

            migrationBuilder.DropColumn(
                name: "IsReceived",
                table: "Items");

            migrationBuilder.DropColumn(
                name: "PurchasedAtUtc",
                table: "Items");

            migrationBuilder.DropColumn(
                name: "PurchasedByName",
                table: "Items");

            migrationBuilder.DropColumn(
                name: "ReceivedAtUtc",
                table: "Items");
        }
    }
}
