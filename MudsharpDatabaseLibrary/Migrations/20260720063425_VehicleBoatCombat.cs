using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MudSharp.Migrations
{
    /// <inheritdoc />
    public partial class VehicleBoatCombat : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "AboveRangedCoverId",
                table: "VehicleOccupantSlotProtos",
                type: "bigint(20)",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "BelowRangedCoverId",
                table: "VehicleOccupantSlotProtos",
                type: "bigint(20)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "BoatStabilityDifficulty",
                table: "VehicleOccupantSlotProtos",
                type: "int(11)",
                nullable: false,
                defaultValue: 5);

            migrationBuilder.AddColumn<long>(
                name: "SameLevelRangedCoverId",
                table: "VehicleOccupantSlotProtos",
                type: "bigint(20)",
                nullable: true);

            migrationBuilder.AddColumn<ulong>(
                name: "PreferTerrestrialCombat",
                table: "CharacterCombatSettings",
                type: "bit(1)",
                nullable: false,
                defaultValueSql: "b'1'");

            migrationBuilder.CreateIndex(
                name: "FK_VehicleOccupantSlotProtos_AboveCover_idx",
                table: "VehicleOccupantSlotProtos",
                column: "AboveRangedCoverId");

            migrationBuilder.CreateIndex(
                name: "FK_VehicleOccupantSlotProtos_BelowCover_idx",
                table: "VehicleOccupantSlotProtos",
                column: "BelowRangedCoverId");

            migrationBuilder.CreateIndex(
                name: "FK_VehicleOccupantSlotProtos_SameLevelCover_idx",
                table: "VehicleOccupantSlotProtos",
                column: "SameLevelRangedCoverId");

            migrationBuilder.AddForeignKey(
                name: "FK_VehicleOccupantSlotProtos_AboveCover",
                table: "VehicleOccupantSlotProtos",
                column: "AboveRangedCoverId",
                principalTable: "RangedCovers",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_VehicleOccupantSlotProtos_BelowCover",
                table: "VehicleOccupantSlotProtos",
                column: "BelowRangedCoverId",
                principalTable: "RangedCovers",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_VehicleOccupantSlotProtos_SameLevelCover",
                table: "VehicleOccupantSlotProtos",
                column: "SameLevelRangedCoverId",
                principalTable: "RangedCovers",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_VehicleOccupantSlotProtos_AboveCover",
                table: "VehicleOccupantSlotProtos");

            migrationBuilder.DropForeignKey(
                name: "FK_VehicleOccupantSlotProtos_BelowCover",
                table: "VehicleOccupantSlotProtos");

            migrationBuilder.DropForeignKey(
                name: "FK_VehicleOccupantSlotProtos_SameLevelCover",
                table: "VehicleOccupantSlotProtos");

            migrationBuilder.DropIndex(
                name: "FK_VehicleOccupantSlotProtos_AboveCover_idx",
                table: "VehicleOccupantSlotProtos");

            migrationBuilder.DropIndex(
                name: "FK_VehicleOccupantSlotProtos_BelowCover_idx",
                table: "VehicleOccupantSlotProtos");

            migrationBuilder.DropIndex(
                name: "FK_VehicleOccupantSlotProtos_SameLevelCover_idx",
                table: "VehicleOccupantSlotProtos");

            migrationBuilder.DropColumn(
                name: "AboveRangedCoverId",
                table: "VehicleOccupantSlotProtos");

            migrationBuilder.DropColumn(
                name: "BelowRangedCoverId",
                table: "VehicleOccupantSlotProtos");

            migrationBuilder.DropColumn(
                name: "BoatStabilityDifficulty",
                table: "VehicleOccupantSlotProtos");

            migrationBuilder.DropColumn(
                name: "SameLevelRangedCoverId",
                table: "VehicleOccupantSlotProtos");

            migrationBuilder.DropColumn(
                name: "PreferTerrestrialCombat",
                table: "CharacterCombatSettings");
        }
    }
}
