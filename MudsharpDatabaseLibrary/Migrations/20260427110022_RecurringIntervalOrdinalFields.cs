using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MudSharp.Migrations
{
    /// <inheritdoc />
    public partial class RecurringIntervalOrdinalFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "IntervalFallback",
                table: "ProgSchedules",
                type: "int(11)",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "IntervalOtherSecondary",
                table: "ProgSchedules",
                type: "int(11)",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "IntervalFallback",
                table: "EconomicZones",
                type: "int(11)",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "IntervalOther",
                table: "EconomicZones",
                type: "int(11)",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "PayIntervalFallback",
                table: "Clans",
                type: "int(11)",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "PayIntervalOtherSecondary",
                table: "Clans",
                type: "int(11)",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IntervalFallback",
                table: "ProgSchedules");

            migrationBuilder.DropColumn(
                name: "IntervalOtherSecondary",
                table: "ProgSchedules");

            migrationBuilder.DropColumn(
                name: "IntervalFallback",
                table: "EconomicZones");

            migrationBuilder.DropColumn(
                name: "IntervalOther",
                table: "EconomicZones");

            migrationBuilder.DropColumn(
                name: "PayIntervalFallback",
                table: "Clans");

            migrationBuilder.DropColumn(
                name: "PayIntervalOtherSecondary",
                table: "Clans");
        }
    }
}
