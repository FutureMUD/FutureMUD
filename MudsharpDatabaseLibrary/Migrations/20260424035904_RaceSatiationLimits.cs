using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MudSharp.Migrations
{
    /// <inheritdoc />
    public partial class RaceSatiationLimits : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "MaximumDrinkSatiatedHours",
                table: "Races",
                type: "double",
                nullable: false,
                defaultValue: 8.0);

            migrationBuilder.AddColumn<double>(
                name: "MaximumFoodSatiatedHours",
                table: "Races",
                type: "double",
                nullable: false,
                defaultValue: 16.0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MaximumDrinkSatiatedHours",
                table: "Races");

            migrationBuilder.DropColumn(
                name: "MaximumFoodSatiatedHours",
                table: "Races");
        }
    }
}
