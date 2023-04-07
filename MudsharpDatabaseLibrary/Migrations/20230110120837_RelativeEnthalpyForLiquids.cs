using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MudSharp.Migrations
{
    public partial class RelativeEnthalpyForLiquids : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "RelativeEnthalpy",
                table: "Liquids",
                type: "double",
                nullable: false,
                defaultValueSql: "'1.0'");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RelativeEnthalpy",
                table: "Liquids");
        }
    }
}
