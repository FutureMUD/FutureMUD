using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using MudSharp.Database;

#nullable disable

namespace MudSharp.Migrations
{
    /// <inheritdoc />
    [DbContext(typeof(FuturemudDatabaseContext))]
    [Migration("20260627000000_VehicleTowStressPolicy")]
    public partial class VehicleTowStressPolicy : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "TowStressDamageMultiplier",
                table: "VehicleTowPointProtos",
                type: "double",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "TowStressFailureStartRatio",
                table: "VehicleTowPointProtos",
                type: "double",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "TowStressMaximumFailureChance",
                table: "VehicleTowPointProtos",
                type: "double",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "TowStressWarningRatio",
                table: "VehicleTowPointProtos",
                type: "double",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TowStressDamageMultiplier",
                table: "VehicleTowPointProtos");

            migrationBuilder.DropColumn(
                name: "TowStressFailureStartRatio",
                table: "VehicleTowPointProtos");

            migrationBuilder.DropColumn(
                name: "TowStressMaximumFailureChance",
                table: "VehicleTowPointProtos");

            migrationBuilder.DropColumn(
                name: "TowStressWarningRatio",
                table: "VehicleTowPointProtos");
        }
    }
}