using Microsoft.EntityFrameworkCore.Migrations;

namespace MudSharp.Migrations
{
    public partial class OngoingCheckForCharacteristics : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "OngoingValidityProgId",
                table: "CharacteristicValues",
                type: "bigint(20)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_CharacteristicValues_OngoingValidityProgId",
                table: "CharacteristicValues",
                column: "OngoingValidityProgId");

            migrationBuilder.AddForeignKey(
                name: "FK_CharacteristicValues_FutureProgs_Ongoing",
                table: "CharacteristicValues",
                column: "OngoingValidityProgId",
                principalTable: "FutureProgs",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CharacteristicValues_FutureProgs_Ongoing",
                table: "CharacteristicValues");

            migrationBuilder.DropIndex(
                name: "IX_CharacteristicValues_OngoingValidityProgId",
                table: "CharacteristicValues");

            migrationBuilder.DropColumn(
                name: "OngoingValidityProgId",
                table: "CharacteristicValues");
        }
    }
}
