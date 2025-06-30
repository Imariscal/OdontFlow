using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OdontFlow.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class updateProductIdv1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_StationWorks_Employees_ProductId",
                table: "StationWorks");

            migrationBuilder.AddForeignKey(
                name: "FK_StationWorks_Products_ProductId",
                table: "StationWorks",
                column: "ProductId",
                principalTable: "Products",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_StationWorks_Products_ProductId",
                table: "StationWorks");

            migrationBuilder.AddForeignKey(
                name: "FK_StationWorks_Employees_ProductId",
                table: "StationWorks",
                column: "ProductId",
                principalTable: "Employees",
                principalColumn: "Id");
        }
    }
}
