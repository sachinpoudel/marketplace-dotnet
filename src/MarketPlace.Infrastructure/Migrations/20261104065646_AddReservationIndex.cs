using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MarketPlace.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddReservationIndex : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CategoryIds",
                table: "Products",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "ReservedStockQuantity",
                table: "Products",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<uint>(
                name: "xmin",
                table: "Products",
                type: "xid",
                rowVersion: true,
                nullable: false,
                defaultValue: 0u);

            migrationBuilder.AddColumn<DateTime>(
                name: "CancelledAt",
                table: "Orders",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "ReservationExpiresAt",
                table: "Orders",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ReservationStatus",
                table: "Orders",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<uint>(
                name: "xmin",
                table: "Orders",
                type: "xid",
                rowVersion: true,
                nullable: false,
                defaultValue: 0u);

            migrationBuilder.CreateIndex(
                name: "IX_Orders_ReservationExpiresAt",
                table: "Orders",
                column: "ReservationExpiresAt");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_ReservationStatus",
                table: "Orders",
                column: "ReservationStatus");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Orders_ReservationExpiresAt",
                table: "Orders");

            migrationBuilder.DropIndex(
                name: "IX_Orders_ReservationStatus",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "CategoryIds",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "ReservedStockQuantity",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "xmin",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "CancelledAt",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "ReservationExpiresAt",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "ReservationStatus",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "xmin",
                table: "Orders");
        }
    }
}
