using Microsoft.EntityFrameworkCore.Migrations;

namespace MudSharp.Migrations
{
    public partial class MoreSpellStuff4 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "MinimumSuccessThreshold",
                table: "MagicSpells",
                type: "int(11)",
                nullable: false,
                defaultValueSql: "4");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MinimumSuccessThreshold",
                table: "MagicSpells");
        }
    }
}
