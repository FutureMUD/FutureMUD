using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MudSharp.Migrations
{
    /// <inheritdoc />
    public partial class AddSpellGateRetirementCursor : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "GateLastPulseUtc",
                table: "MagicSpellLifecycles",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "GateLinksClosedUtc",
                table: "MagicSpellLifecycles",
                type: "datetime(6)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "GateLastPulseUtc",
                table: "MagicSpellLifecycles");

            migrationBuilder.DropColumn(
                name: "GateLinksClosedUtc",
                table: "MagicSpellLifecycles");
        }
    }
}
