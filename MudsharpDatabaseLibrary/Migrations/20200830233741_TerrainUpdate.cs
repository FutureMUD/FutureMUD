using Microsoft.EntityFrameworkCore.Migrations;

namespace MudSharp.Migrations
{
    public partial class TerrainUpdate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "GasFormId",
                table: "Materials");

            migrationBuilder.DropColumn(
                name: "SolidFormId",
                table: "Materials");

            migrationBuilder.AddColumn<int>(
                name: "DefaultCellOutdoorsType",
                table: "Terrains",
                type: "int(11)",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "TerrainEditorText",
                table: "Terrains",
                type: "varchar(45)",
                nullable: true);

            migrationBuilder.AlterColumn<long>(
                name: "LiquidFormId",
                table: "Materials",
                nullable: true,
                oldClrType: typeof(long),
                oldType: "bigint(20)",
                oldNullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DefaultCellOutdoorsType",
                table: "Terrains");

            migrationBuilder.DropColumn(
                name: "TerrainEditorText",
                table: "Terrains");

            migrationBuilder.AlterColumn<long>(
                name: "LiquidFormId",
                table: "Materials",
                type: "bigint(20)",
                nullable: true,
                oldClrType: typeof(long),
                oldNullable: true);

            migrationBuilder.AddColumn<long>(
                name: "GasFormId",
                table: "Materials",
                type: "bigint(20)",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "SolidFormId",
                table: "Materials",
                type: "bigint(20)",
                nullable: true);
        }
    }
}
