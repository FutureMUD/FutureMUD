using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MudSharp.Migrations
{
    public partial class TerrainMapColourAddition : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "TerrainANSIColour",
                table: "Terrains",
                type: "varchar(45)",
                nullable: false,
                defaultValueSql: "'7'",
                collation: "utf8_general_ci")
                .Annotation("MySql:CharSet", "utf8");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TerrainANSIColour",
                table: "Terrains");

            migrationBuilder.AlterColumn<string>(
                name: "AccountName",
                table: "LineOfCreditAccounts",
                type: "longtext CHARACTER SET utf8mb4",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "longtext",
                oldNullable: true)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");
        }
    }
}
