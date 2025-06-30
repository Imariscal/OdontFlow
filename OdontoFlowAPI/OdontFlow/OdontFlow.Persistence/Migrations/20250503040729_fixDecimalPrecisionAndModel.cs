﻿using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OdontFlow.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class fixDecimalPrecisionAndModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Clients_Employees_EmployeeId",
                table: "Clients");

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

            migrationBuilder.AddForeignKey(
                name: "FK_Clients_Employees_EmployeeId",
                table: "Clients",
                column: "EmployeeId",
                principalTable: "Employees",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
