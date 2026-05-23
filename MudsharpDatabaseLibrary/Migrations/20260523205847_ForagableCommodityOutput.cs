using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MudSharp.Migrations
{
    /// <inheritdoc />
    public partial class ForagableCommodityOutput : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "CommodityMaterialId",
                table: "Foragables",
                type: "bigint(20)",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "CommodityTagId",
                table: "Foragables",
                type: "bigint(20)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CommodityWeightExpression",
                table: "Foragables",
                type: "varchar(200)",
                nullable: true,
                collation: "utf8mb4_unicode_ci")
                .Annotation("MySql:CharSet", "utf8mb4");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CommodityMaterialId",
                table: "Foragables");

            migrationBuilder.DropColumn(
                name: "CommodityTagId",
                table: "Foragables");

            migrationBuilder.DropColumn(
                name: "CommodityWeightExpression",
                table: "Foragables");
        }
    }
}
