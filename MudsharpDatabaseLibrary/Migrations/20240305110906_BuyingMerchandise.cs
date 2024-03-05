using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MudSharp.Migrations
{
    /// <inheritdoc />
    public partial class BuyingMerchandise : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "MinimumFloatToBuyItems",
                table: "Shops",
                type: "decimal(65,30)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "BaseBuyModifier",
                table: "Merchandises",
                type: "decimal(58,29)",
                nullable: false,
                defaultValue: 0.3m);

            migrationBuilder.AddColumn<int>(
                name: "MaximumStockLevelsToBuy",
                table: "Merchandises",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<double>(
                name: "MinimumConditionToBuy",
                table: "Merchandises",
                type: "double",
                nullable: false,
                defaultValue: 0.94999999999999996);

            migrationBuilder.AddColumn<ulong>(
                name: "WillBuy",
                table: "Merchandises",
                type: "bit(1)",
                nullable: false,
                defaultValue: 0ul);

            migrationBuilder.AddColumn<ulong>(
                name: "WillSell",
                table: "Merchandises",
                type: "bit(1)",
                nullable: false,
                defaultValue: 1ul);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MinimumFloatToBuyItems",
                table: "Shops");

            migrationBuilder.DropColumn(
                name: "BaseBuyModifier",
                table: "Merchandises");

            migrationBuilder.DropColumn(
                name: "MaximumStockLevelsToBuy",
                table: "Merchandises");

            migrationBuilder.DropColumn(
                name: "MinimumConditionToBuy",
                table: "Merchandises");

            migrationBuilder.DropColumn(
                name: "WillBuy",
                table: "Merchandises");

            migrationBuilder.DropColumn(
                name: "WillSell",
                table: "Merchandises");
        }
    }
}
