using Microsoft.EntityFrameworkCore.Migrations;

namespace MudSharp.Migrations
{
    public partial class AttributesUpdate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<ulong>(
                name: "DisplayAsSubAttribute",
                table: "TraitDefinitions",
                type: "bit(1)",
                nullable: false,
                defaultValueSql: "b'0'");

            migrationBuilder.AddColumn<int>(
                name: "DisplayOrder",
                table: "TraitDefinitions",
                type: "int(11)",
                nullable: false,
                defaultValueSql: "1");

            migrationBuilder.AddColumn<ulong>(
                name: "ShowInAttributeCommand",
                table: "TraitDefinitions",
                type: "bit(1)",
                nullable: false,
                defaultValueSql: "b'1'");

            migrationBuilder.AddColumn<ulong>(
                name: "ShowInScoreCommand",
                table: "TraitDefinitions",
                type: "bit(1)",
                nullable: false,
                defaultValueSql: "b'1'");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DisplayAsSubAttribute",
                table: "TraitDefinitions");

            migrationBuilder.DropColumn(
                name: "DisplayOrder",
                table: "TraitDefinitions");

            migrationBuilder.DropColumn(
                name: "ShowInAttributeCommand",
                table: "TraitDefinitions");

            migrationBuilder.DropColumn(
                name: "ShowInScoreCommand",
                table: "TraitDefinitions");
        }
    }
}
