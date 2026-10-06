using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MudSharp.Migrations
{
    /// <inheritdoc />
    public partial class SpellOwnedRemainsRemovalIntent : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "RemainsNotificationAttemptedUtc",
                table: "MagicSpellLifecycles",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "RemainsNotificationCompletedUtc",
                table: "MagicSpellLifecycles",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "RemainsRemovalRequestedUtc",
                table: "MagicSpellLifecycles",
                type: "datetime(6)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RemainsNotificationAttemptedUtc",
                table: "MagicSpellLifecycles");

            migrationBuilder.DropColumn(
                name: "RemainsNotificationCompletedUtc",
                table: "MagicSpellLifecycles");

            migrationBuilder.DropColumn(
                name: "RemainsRemovalRequestedUtc",
                table: "MagicSpellLifecycles");
        }
    }
}
