using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MudSharp.Migrations
{
    public partial class CombatSettingPriorityAndRaceDefault : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "PriorityProgId",
                table: "CharacterCombatSettings",
                type: "bigint(20)",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "DefaultCombatSettingId",
                table: "Races",
                type: "bigint(20)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "FK_CharacterCombatSettings_PriorityProg_idx",
                table: "CharacterCombatSettings",
                column: "PriorityProgId");

            migrationBuilder.CreateIndex(
                name: "FK_Races_CharacterCombatSettings_idx",
                table: "Races",
                column: "DefaultCombatSettingId");

            migrationBuilder.AddForeignKey(
                name: "FK_CharacterCombatSettings_PriorityProg",
                table: "CharacterCombatSettings",
                column: "PriorityProgId",
                principalTable: "FutureProgs",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Races_CharacterCombatSettings",
                table: "Races",
                column: "DefaultCombatSettingId",
                principalTable: "CharacterCombatSettings",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CharacterCombatSettings_PriorityProg",
                table: "CharacterCombatSettings");

            migrationBuilder.DropForeignKey(
                name: "FK_Races_CharacterCombatSettings",
                table: "Races");

            migrationBuilder.DropIndex(
                name: "FK_CharacterCombatSettings_PriorityProg_idx",
                table: "CharacterCombatSettings");

            migrationBuilder.DropIndex(
                name: "FK_Races_CharacterCombatSettings_idx",
                table: "Races");

            migrationBuilder.DropColumn(
                name: "PriorityProgId",
                table: "CharacterCombatSettings");

            migrationBuilder.DropColumn(
                name: "DefaultCombatSettingId",
                table: "Races");
        }
    }
}
