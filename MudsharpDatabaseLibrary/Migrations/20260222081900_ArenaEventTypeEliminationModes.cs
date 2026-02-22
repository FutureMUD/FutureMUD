using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MudSharp.Migrations
{
    /// <inheritdoc />
    public partial class ArenaEventTypeEliminationModes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<ulong>(
                name: "AllowSurrender",
                table: "ArenaEventTypes",
                type: "bit(1)",
                nullable: false,
                defaultValueSql: "b'1'");

            migrationBuilder.AddColumn<int>(
                name: "EliminationMode",
                table: "ArenaEventTypes",
                type: "int(11)",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AllowSurrender",
                table: "ArenaEventTypes");

            migrationBuilder.DropColumn(
                name: "EliminationMode",
                table: "ArenaEventTypes");
        }
    }
}
