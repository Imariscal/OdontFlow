using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OdontFlow.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class UpdateClientComissionPercentage : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ClientEmployee");

            migrationBuilder.AddColumn<decimal>(
                name: "CommissionPercentage",
                table: "Clients",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CommissionPercentage",
                table: "Clients");

            migrationBuilder.CreateTable(
                name: "ClientEmployee",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ClientId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    EmployeeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Active = table.Column<bool>(type: "bit", nullable: false),
                    CommissionPercentage = table.Column<float>(type: "real", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    CreationDate = table.Column<DateTime>(type: "smalldatetime", nullable: false),
                    Deleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedBy = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    DeletionDate = table.Column<DateTime>(type: "datetime2", maxLength: 50, nullable: true),
                    LastModificationDate = table.Column<DateTime>(type: "smalldatetime", nullable: false),
                    LastModifiedBy = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClientEmployee", x => x.Id);
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
                name: "IX_ClientEmployee_ClientId_EmployeeId",
                table: "ClientEmployee",
                columns: new[] { "ClientId", "EmployeeId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ClientEmployee_EmployeeId",
                table: "ClientEmployee",
                column: "EmployeeId");
        }
    }
}
