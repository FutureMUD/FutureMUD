using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MudSharp.Migrations
{
    public partial class CurrencyPatternRegexCaseFix : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<ulong>(
                name: "IgnoreCase",
                table: "CurrencyDivisions",
                type: "bit(1)",
                nullable: false,
                defaultValue: 1ul);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IgnoreCase",
                table: "CurrencyDivisions");
        }
    }
}
