using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MudSharp.Migrations
{
    /// <inheritdoc />
    public partial class CommodityMerchandiseWeightedSales : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CommodityCharacteristics",
                table: "Merchandises",
                type: "text",
                nullable: true,
                collation: "utf8_general_ci")
                .Annotation("MySql:CharSet", "utf8");

            migrationBuilder.AddColumn<long>(
                name: "CommodityMaterialId",
                table: "Merchandises",
                type: "bigint(20)",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "CommodityPricingWeight",
                table: "Merchandises",
                type: "double",
                nullable: false,
                defaultValue: 1.0);

            migrationBuilder.AddColumn<long>(
                name: "CommodityTagId",
                table: "Merchandises",
                type: "bigint(20)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "MerchandiseType",
                table: "Merchandises",
                type: "int(11)",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CommodityCharacteristics",
                table: "Merchandises");

            migrationBuilder.DropColumn(
                name: "CommodityMaterialId",
                table: "Merchandises");

            migrationBuilder.DropColumn(
                name: "CommodityPricingWeight",
                table: "Merchandises");

            migrationBuilder.DropColumn(
                name: "CommodityTagId",
                table: "Merchandises");

            migrationBuilder.DropColumn(
                name: "MerchandiseType",
                table: "Merchandises");
        }
    }
}
