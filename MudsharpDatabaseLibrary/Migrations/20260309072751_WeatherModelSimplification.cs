using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MudSharp.Migrations
{
    /// <inheritdoc />
    public partial class WeatherModelSimplification : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "CurrentTemperatureFluctuation",
                table: "WeatherControllers",
                type: "double",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<ulong>(
                name: "OppositeHemisphere",
                table: "WeatherControllers",
                type: "bit(1)",
                nullable: false,
                defaultValue: 0ul);

            migrationBuilder.AddColumn<int>(
                name: "TemperatureFluctuationPeriodMinutes",
                table: "RegionalClimates",
                type: "int(11)",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<double>(
                name: "TemperatureFluctuationStandardDeviation",
                table: "RegionalClimates",
                type: "double",
                nullable: false,
                defaultValue: 0.0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CurrentTemperatureFluctuation",
                table: "WeatherControllers");

            migrationBuilder.DropColumn(
                name: "OppositeHemisphere",
                table: "WeatherControllers");

            migrationBuilder.DropColumn(
                name: "TemperatureFluctuationPeriodMinutes",
                table: "RegionalClimates");

            migrationBuilder.DropColumn(
                name: "TemperatureFluctuationStandardDeviation",
                table: "RegionalClimates");
        }
    }
}
