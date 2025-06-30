using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OdontFlow.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddOrderIrToWorkOrders : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "OrderId",
                table: "StationWorks",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_StationWorks_OrderId",
                table: "StationWorks",
                column: "OrderId");

            migrationBuilder.AddForeignKey(
                name: "FK_StationWorks_Orders_OrderId",
                table: "StationWorks",
                column: "OrderId",
                principalTable: "Orders",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_StationWorks_Orders_OrderId",
                table: "StationWorks");

            migrationBuilder.DropIndex(
                name: "IX_StationWorks_OrderId",
                table: "StationWorks");

            migrationBuilder.DropColumn(
                name: "OrderId",
                table: "StationWorks");
        }
    }
}
