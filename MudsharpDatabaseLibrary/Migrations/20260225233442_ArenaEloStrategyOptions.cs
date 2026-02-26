using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MudSharp.Migrations
{
    /// <inheritdoc />
    public partial class ArenaEloStrategyOptions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "EloKFactor",
                table: "ArenaEventTypes",
                type: "decimal(58,29)",
                nullable: false,
                defaultValue: 32.0m);

            migrationBuilder.AddColumn<int>(
                name: "EloStyle",
                table: "ArenaEventTypes",
                type: "int(11)",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EloKFactor",
                table: "ArenaEventTypes");

            migrationBuilder.DropColumn(
                name: "EloStyle",
                table: "ArenaEventTypes");
        }
    }
}
