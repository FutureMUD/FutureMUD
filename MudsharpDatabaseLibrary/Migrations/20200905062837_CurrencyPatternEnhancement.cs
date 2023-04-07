using Microsoft.EntityFrameworkCore.Migrations;

namespace MudSharp.Migrations
{
    public partial class CurrencyPatternEnhancement : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<ulong>(
                name: "UseNaturalAggregationStyle",
                table: "CurrencyDescriptionPatterns",
                type: "bit(1)",
                nullable: false,
                defaultValue: 0ul);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UseNaturalAggregationStyle",
                table: "CurrencyDescriptionPatterns");
        }
    }
}
