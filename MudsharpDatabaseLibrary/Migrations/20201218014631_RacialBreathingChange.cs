using Microsoft.EntityFrameworkCore.Migrations;

namespace MudSharp.Migrations
{
    public partial class RacialBreathingChange : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "BreathingModel",
                table: "Races",
                type: "varchar(100)",
                nullable: true,
                defaultValueSql: "'simple'",
                collation: "utf8_general_ci")
                .Annotation("MySql:CharSet", "utf8");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BreathingModel",
                table: "Races");
        }
    }
}
