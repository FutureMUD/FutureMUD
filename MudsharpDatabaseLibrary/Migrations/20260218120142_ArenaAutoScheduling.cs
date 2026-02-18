using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MudSharp.Migrations
{
    /// <inheritdoc />
    public partial class ArenaAutoScheduling : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "AutoScheduleIntervalSeconds",
                table: "ArenaEventTypes",
                type: "int(11)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "AutoScheduleReferenceTime",
                table: "ArenaEventTypes",
                type: "datetime",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AutoScheduleIntervalSeconds",
                table: "ArenaEventTypes");

            migrationBuilder.DropColumn(
                name: "AutoScheduleReferenceTime",
                table: "ArenaEventTypes");
        }
    }
}
