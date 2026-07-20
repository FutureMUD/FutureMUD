using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MudSharp.Migrations
{
    /// <inheritdoc />
    public partial class VehicleSurfaceWaterMovementProfiles : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<ulong>(
                name: "ExposesOccupantsToWater",
                table: "VehicleMovementProfileProtos",
                type: "bit(1)",
                nullable: false,
                defaultValue: 0ul);

            migrationBuilder.AddColumn<int>(
                name: "MovementEnvironment",
                table: "VehicleMovementProfileProtos",
                type: "int(11)",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ExposesOccupantsToWater",
                table: "VehicleMovementProfileProtos");

            migrationBuilder.DropColumn(
                name: "MovementEnvironment",
                table: "VehicleMovementProfileProtos");
        }
    }
}
