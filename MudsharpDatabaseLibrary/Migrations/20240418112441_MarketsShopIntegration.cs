using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MudSharp.Migrations
{
    /// <inheritdoc />
    public partial class MarketsShopIntegration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
	        migrationBuilder.AddColumn<bool>(
		        name: "IgnoreMarketPricing",
		        table: "Merchandises",
		        type: "tinyint(1)",
		        nullable: false,
		        defaultValue: false);

	        migrationBuilder.AddColumn<long>(
		        name: "MarketId",
		        table: "Shops",
		        type: "bigint",
		        nullable: true);

	        migrationBuilder.CreateIndex(
		        name: "IX_Shops_MarketId",
		        table: "Shops",
		        column: "MarketId");

	        migrationBuilder.AddForeignKey(
		        name: "FK_Shops_Markets_MarketId",
		        table: "Shops",
		        column: "MarketId",
		        principalTable: "Markets",
		        principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Shops_Markets_MarketId",
                table: "Shops");

            migrationBuilder.DropIndex(
                name: "IX_Shops_MarketId",
                table: "Shops");

            migrationBuilder.DropColumn(
                name: "MarketId",
                table: "Shops");

            migrationBuilder.DropColumn(
                name: "IgnoreMarketPricing",
                table: "Merchandises");
        }
    }
}
