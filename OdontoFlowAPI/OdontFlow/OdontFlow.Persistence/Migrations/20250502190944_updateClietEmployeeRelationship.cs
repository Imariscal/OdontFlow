using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OdontFlow.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class updateClietEmployeeRelationship : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Clients_Employees_EmployeeId",
                table: "Clients");

            //migrationBuilder.DropIndex(
            //    name: "IX_Clients_EmployeeId",
            //    table: "Clients");

            migrationBuilder.DropColumn(
                name: "EmployeeId",
                table: "Clients");

            migrationBuilder.CreateTable(
                name: "ClientEmployee",
                columns: table => new
                {
                    ClientId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    EmployeeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CommissionPercentage = table.Column<float>(type: "real", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClientEmployee", x => new { x.ClientId, x.EmployeeId });
                    table.ForeignKey(
                        name: "FK_ClientEmployee_Clients_ClientId",
                        column: x => x.ClientId,
                        principalTable: "Clients",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ClientEmployee_Employees_EmployeeId",
                        column: x => x.EmployeeId,
                        principalTable: "Employees",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ClientEmployee_EmployeeId",
                table: "ClientEmployee",
                column: "EmployeeId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ClientEmployee");

            migrationBuilder.AddColumn<Guid>(
                name: "EmployeeId",
                table: "Clients",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Clients_EmployeeId",
                table: "Clients",
                column: "EmployeeId");

            migrationBuilder.AddForeignKey(
                name: "FK_Clients_Employees_EmployeeId",
                table: "Clients",
                column: "EmployeeId",
                principalTable: "Employees",
                principalColumn: "Id");
        }
    }
}
