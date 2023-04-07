using Microsoft.EntityFrameworkCore.Migrations;

namespace MudSharp.Migrations
{
    public partial class ClanFame : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "FameType",
                table: "Ranks",
                type: "int(11)",
                nullable: false,
                defaultValueSql: "0");

            migrationBuilder.AddColumn<ulong>(
                name: "ShowFamousMembersInNotables",
                table: "Clans",
                type: "bit(1)",
                nullable: false,
                defaultValueSql: "b'0'");

            migrationBuilder.AddColumn<string>(
                name: "Sphere",
                table: "Clans",
                type: "varchar(100)",
                nullable: true,
                collation: "utf8_general_ci")
                .Annotation("MySql:CharSet", "utf8");

            migrationBuilder.AddColumn<int>(
                name: "FameType",
                table: "Appointments",
                type: "int(11)",
                nullable: false,
                defaultValueSql: "0");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FameType",
                table: "Ranks");

            migrationBuilder.DropColumn(
                name: "ShowFamousMembersInNotables",
                table: "Clans");

            migrationBuilder.DropColumn(
                name: "Sphere",
                table: "Clans");

            migrationBuilder.DropColumn(
                name: "FameType",
                table: "Appointments");
        }
    }
}
