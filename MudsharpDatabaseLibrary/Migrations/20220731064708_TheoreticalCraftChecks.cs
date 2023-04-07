using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MudSharp.Migrations
{
    public partial class TheoreticalCraftChecks : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<ulong>(
                name: "IsPracticalCheck",
                table: "Crafts",
                type: "bit(1)",
                nullable: false,
                defaultValueSql: "b'1'");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsPracticalCheck",
                table: "Crafts");
        }
    }
}
