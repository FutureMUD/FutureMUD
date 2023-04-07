using Microsoft.EntityFrameworkCore.Migrations;

namespace MudSharp.Migrations
{
    public partial class WeaponAttackAddPositionRequirement : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "RequiredPositionStateIds",
                table: "WeaponAttacks",
                type: "varchar(250)",
                nullable: false,
                defaultValue: "1 16 17 18",
                collation: "utf8_general_ci")
                .Annotation("MySql:CharSet", "utf8");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RequiredPositionStateIds",
                table: "WeaponAttacks");
        }
    }
}
