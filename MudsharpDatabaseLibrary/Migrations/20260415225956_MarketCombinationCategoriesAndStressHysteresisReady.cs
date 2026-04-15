using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MudSharp.Migrations
{
    /// <inheritdoc />
    public partial class MarketCombinationCategoriesAndStressHysteresisReady : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "StressFlickerThreshold",
                table: "MarketPopulations",
                type: "decimal(65,30)",
                nullable: false,
                defaultValue: 0.01m);

            migrationBuilder.AddColumn<string>(
                name: "CombinationCategories",
                table: "MarketCategories",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<int>(
                name: "MarketCategoryType",
                table: "MarketCategories",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "StressFlickerThreshold",
                table: "MarketPopulations");

            migrationBuilder.DropColumn(
                name: "CombinationCategories",
                table: "MarketCategories");

            migrationBuilder.DropColumn(
                name: "MarketCategoryType",
                table: "MarketCategories");
        }
    }
}
