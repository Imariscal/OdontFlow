using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OdontFlow.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class updateProductId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "ProductId",
                table: "StationWorks",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_StationWorks_ProductId",
                table: "StationWorks",
                column: "ProductId");

            migrationBuilder.AddForeignKey(
                name: "FK_StationWorks_Employees_ProductId",
                table: "StationWorks",
                column: "ProductId",
                principalTable: "Employees",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_StationWorks_Employees_ProductId",
                table: "StationWorks");

            migrationBuilder.DropIndex(
                name: "IX_StationWorks_ProductId",
                table: "StationWorks");

            migrationBuilder.DropColumn(
                name: "ProductId",
                table: "StationWorks");
        }
    }
}
