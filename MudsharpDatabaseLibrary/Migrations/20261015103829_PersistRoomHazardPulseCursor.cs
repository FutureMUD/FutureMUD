using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MudSharp.Migrations
{
    /// <inheritdoc />
    public partial class PersistRoomHazardPulseCursor : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "RoomHazardLastPulseUtc",
                table: "MagicSpellLifecycles",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "RoomHazardNextPulseUtc",
                table: "MagicSpellLifecycles",
                type: "datetime(6)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RoomHazardLastPulseUtc",
                table: "MagicSpellLifecycles");

            migrationBuilder.DropColumn(
                name: "RoomHazardNextPulseUtc",
                table: "MagicSpellLifecycles");
        }
    }
}
