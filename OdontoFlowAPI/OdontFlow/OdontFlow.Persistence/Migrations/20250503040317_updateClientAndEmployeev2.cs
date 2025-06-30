using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OdontFlow.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class updateClientAndEmployeev2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Clients_Employees_EmployeeId",
                table: "Clients");

            migrationBuilder.DropIndex(
                name: "IX_Clients_EmployeeId",
                table: "Clients");

            migrationBuilder.DropColumn(
                name: "EmployeeId",
                table: "Clients");
        }
    }
}
