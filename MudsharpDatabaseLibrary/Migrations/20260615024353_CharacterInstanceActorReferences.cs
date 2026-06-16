using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MudSharp.Migrations
{
    /// <inheritdoc />
    public partial class CharacterInstanceActorReferences : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_VehicleOccupancies_Vehicle_Character",
                table: "VehicleOccupancies");

            migrationBuilder.AddColumn<long>(
                name: "CharacterInstanceId",
                table: "VehicleOccupancies",
                type: "bigint(20)",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "SourceCharacterInstanceId",
                table: "VehicleHitchLinks",
                type: "bigint(20)",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "TargetCharacterInstanceId",
                table: "VehicleHitchLinks",
                type: "bigint(20)",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "ActiveCharacterInstanceId",
                table: "ArenaSignups",
                type: "bigint(20)",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "CharacterInstanceKey",
                table: "VehicleOccupancies",
                type: "bigint(20)",
                nullable: false,
                computedColumnSql: "COALESCE(`CharacterInstanceId`, 0)",
                stored: true);

            migrationBuilder.CreateIndex(
                name: "FK_VehicleOccupancies_CharacterInstances_idx",
                table: "VehicleOccupancies",
                column: "CharacterInstanceId");

            migrationBuilder.CreateIndex(
                name: "IX_VehicleOccupancies_Vehicle_Character_Instance",
                table: "VehicleOccupancies",
                columns: new[] { "VehicleId", "CharacterId", "CharacterInstanceKey" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "FK_VehicleHitchLinks_SourceCharacterInstances_idx",
                table: "VehicleHitchLinks",
                column: "SourceCharacterInstanceId");

            migrationBuilder.CreateIndex(
                name: "FK_VehicleHitchLinks_TargetCharacterInstances_idx",
                table: "VehicleHitchLinks",
                column: "TargetCharacterInstanceId");

            migrationBuilder.CreateIndex(
                name: "FK_ArenaSignups_ActiveCharacterInstances",
                table: "ArenaSignups",
                column: "ActiveCharacterInstanceId");

        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "FK_VehicleOccupancies_CharacterInstances_idx",
                table: "VehicleOccupancies");

            migrationBuilder.DropIndex(
                name: "IX_VehicleOccupancies_Vehicle_Character_Instance",
                table: "VehicleOccupancies");

            migrationBuilder.DropIndex(
                name: "FK_VehicleHitchLinks_SourceCharacterInstances_idx",
                table: "VehicleHitchLinks");

            migrationBuilder.DropIndex(
                name: "FK_VehicleHitchLinks_TargetCharacterInstances_idx",
                table: "VehicleHitchLinks");

            migrationBuilder.DropIndex(
                name: "FK_ArenaSignups_ActiveCharacterInstances",
                table: "ArenaSignups");

            migrationBuilder.DropColumn(
                name: "CharacterInstanceKey",
                table: "VehicleOccupancies");

            migrationBuilder.DropColumn(
                name: "CharacterInstanceId",
                table: "VehicleOccupancies");

            migrationBuilder.DropColumn(
                name: "SourceCharacterInstanceId",
                table: "VehicleHitchLinks");

            migrationBuilder.DropColumn(
                name: "TargetCharacterInstanceId",
                table: "VehicleHitchLinks");

            migrationBuilder.DropColumn(
                name: "ActiveCharacterInstanceId",
                table: "ArenaSignups");

            migrationBuilder.CreateIndex(
                name: "IX_VehicleOccupancies_Vehicle_Character",
                table: "VehicleOccupancies",
                columns: new[] { "VehicleId", "CharacterId" },
                unique: true);
        }
    }
}
