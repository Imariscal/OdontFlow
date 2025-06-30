using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OdontFlow.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class updateClientAndEmployee : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_ClientEmployee",
                table: "ClientEmployee");

            migrationBuilder.AddColumn<Guid>(
                name: "Id",
                table: "ClientEmployee",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<bool>(
                name: "Active",
                table: "ClientEmployee",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                table: "ClientEmployee",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "CreationDate",
                table: "ClientEmployee",
                type: "smalldatetime",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<bool>(
                name: "Deleted",
                table: "ClientEmployee",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "DeletedBy",
                table: "ClientEmployee",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletionDate",
                table: "ClientEmployee",
                type: "datetime2",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastModificationDate",
                table: "ClientEmployee",
                type: "smalldatetime",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "LastModifiedBy",
                table: "ClientEmployee",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ClientEmployee",
                table: "ClientEmployee",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_ClientEmployee_ClientId_EmployeeId",
                table: "ClientEmployee",
                columns: new[] { "ClientId", "EmployeeId" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_ClientEmployee",
                table: "ClientEmployee");

            migrationBuilder.DropIndex(
                name: "IX_ClientEmployee_ClientId_EmployeeId",
                table: "ClientEmployee");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "ClientEmployee");

            migrationBuilder.DropColumn(
                name: "Active",
                table: "ClientEmployee");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "ClientEmployee");

            migrationBuilder.DropColumn(
                name: "CreationDate",
                table: "ClientEmployee");

            migrationBuilder.DropColumn(
                name: "Deleted",
                table: "ClientEmployee");

            migrationBuilder.DropColumn(
                name: "DeletedBy",
                table: "ClientEmployee");

            migrationBuilder.DropColumn(
                name: "DeletionDate",
                table: "ClientEmployee");

            migrationBuilder.DropColumn(
                name: "LastModificationDate",
                table: "ClientEmployee");

            migrationBuilder.DropColumn(
                name: "LastModifiedBy",
                table: "ClientEmployee");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ClientEmployee",
                table: "ClientEmployee",
                columns: new[] { "ClientId", "EmployeeId" });
        }
    }
}
