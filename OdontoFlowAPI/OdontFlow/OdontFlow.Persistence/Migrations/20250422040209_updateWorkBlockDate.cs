using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OdontFlow.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class updateWorkBlockDate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "BlockedDate",
                table: "StationWorks",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "UnblockedDate",
                table: "StationWorks",
                type: "datetime2",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BlockedDate",
                table: "StationWorks");

            migrationBuilder.DropColumn(
                name: "UnblockedDate",
                table: "StationWorks");
        }
    }
}
