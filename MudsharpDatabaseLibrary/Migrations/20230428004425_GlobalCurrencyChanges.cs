using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MudSharp.Migrations
{
    public partial class GlobalCurrencyChanges : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "CostInBaseCurrency",
                table: "GameItemProtos",
                type: "decimal(58,29)",
                nullable: false,
                defaultValueSql: "'0'");

            migrationBuilder.AddColumn<decimal>(
                name: "BaseCurrencyToGlobalBaseCurrencyConversion",
                table: "Currencies",
                type: "decimal(58,29)",
                nullable: false,
                defaultValue: 1.0m);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CostInBaseCurrency",
                table: "GameItemProtos");

            migrationBuilder.DropColumn(
                name: "BaseCurrencyToGlobalBaseCurrencyConversion",
                table: "Currencies");
        }
    }
}
