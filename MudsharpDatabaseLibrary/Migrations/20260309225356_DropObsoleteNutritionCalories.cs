using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MudSharp.Migrations
{
    /// <inheritdoc />
    public partial class DropObsoleteNutritionCalories : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CaloriesPerKilogram",
                table: "Races_EdibleMaterials");

            migrationBuilder.DropColumn(
                name: "CaloriesPerYield",
                table: "RaceEdibleForagableYields");

            migrationBuilder.DropColumn(
                name: "CaloriesPerLitre",
                table: "Liquids");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "CaloriesPerKilogram",
                table: "Races_EdibleMaterials",
                type: "double",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "CaloriesPerYield",
                table: "RaceEdibleForagableYields",
                type: "double",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "CaloriesPerLitre",
                table: "Liquids",
                type: "double",
                nullable: false,
                defaultValue: 0.0);
        }
    }
}
