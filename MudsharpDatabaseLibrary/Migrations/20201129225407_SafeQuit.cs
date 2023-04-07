using Microsoft.EntityFrameworkCore.Migrations;

namespace MudSharp.Migrations
{
    public partial class SafeQuit : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<ulong>(
                name: "SafeQuit",
                table: "CellOverlays",
                type: "bit(1)",
                nullable: false,
                defaultValueSql: "b'1'");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SafeQuit",
                table: "CellOverlays");
        }
    }
}
