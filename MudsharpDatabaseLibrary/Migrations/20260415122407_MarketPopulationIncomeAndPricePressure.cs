using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MudSharp.Migrations
{
    /// <inheritdoc />
    public partial class MarketPopulationIncomeAndPricePressure : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "IncomeFactor",
                table: "MarketPopulations",
                type: "decimal(65,30)",
                nullable: false,
                defaultValue: 1.0m);

            migrationBuilder.AddColumn<decimal>(
                name: "Savings",
                table: "MarketPopulations",
                type: "decimal(65,30)",
                nullable: false,
                defaultValue: 0.0m);

            migrationBuilder.AddColumn<decimal>(
                name: "SavingsCap",
                table: "MarketPopulations",
                type: "decimal(65,30)",
                nullable: false,
                defaultValue: 0.0m);

            migrationBuilder.AddColumn<string>(
                name: "PopulationImpacts",
                table: "MarketInfluenceTemplates",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "PopulationImpacts",
                table: "MarketInfluences",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "CelestialType",
                table: "Celestials",
                type: "varchar(30)",
                nullable: false,
                defaultValue: "Sun",
                collation: "utf8_general_ci",
                oldClrType: typeof(string),
                oldType: "varchar(30)",
                oldDefaultValue: "OldSun",
                oldCollation: "utf8_general_ci")
                .Annotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:CharSet", "utf8");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IncomeFactor",
                table: "MarketPopulations");

            migrationBuilder.DropColumn(
                name: "Savings",
                table: "MarketPopulations");

            migrationBuilder.DropColumn(
                name: "SavingsCap",
                table: "MarketPopulations");

            migrationBuilder.DropColumn(
                name: "PopulationImpacts",
                table: "MarketInfluenceTemplates");

            migrationBuilder.DropColumn(
                name: "PopulationImpacts",
                table: "MarketInfluences");

            migrationBuilder.AlterColumn<string>(
                name: "CelestialType",
                table: "Celestials",
                type: "varchar(30)",
                nullable: false,
                defaultValue: "OldSun",
                collation: "utf8_general_ci",
                oldClrType: typeof(string),
                oldType: "varchar(30)",
                oldDefaultValue: "Sun",
                oldCollation: "utf8_general_ci")
                .Annotation("MySql:CharSet", "utf8")
                .OldAnnotation("MySql:CharSet", "utf8");
        }
    }
}
