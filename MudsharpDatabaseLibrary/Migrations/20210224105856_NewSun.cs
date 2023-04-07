using Microsoft.EntityFrameworkCore.Migrations;

namespace MudSharp.Migrations
{
    public partial class NewSun : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CelestialType",
                table: "Celestials",
                type: "varchar(30)",
                nullable: false,
                defaultValue: "OldSun",
                collation: "utf8_general_ci")
                .Annotation("MySql:CharSet", "utf8");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CelestialType",
                table: "Celestials");
        }
    }
}
