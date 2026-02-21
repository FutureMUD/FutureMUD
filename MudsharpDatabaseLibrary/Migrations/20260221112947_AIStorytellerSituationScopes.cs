using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MudSharp.Migrations
{
    /// <inheritdoc />
    public partial class AIStorytellerSituationScopes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "ScopeCharacterId",
                table: "AIStorytellerSituations",
                type: "bigint(20)",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "ScopeRoomId",
                table: "AIStorytellerSituations",
                type: "bigint(20)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_AIStorytellerSituations_ScopeCharacterId",
                table: "AIStorytellerSituations",
                column: "ScopeCharacterId");

            migrationBuilder.CreateIndex(
                name: "IX_AIStorytellerSituations_ScopeRoomId",
                table: "AIStorytellerSituations",
                column: "ScopeRoomId");

            migrationBuilder.AddForeignKey(
                name: "FK_AIStorytellerSituations_Cells_ScopeRoomId",
                table: "AIStorytellerSituations",
                column: "ScopeRoomId",
                principalTable: "Cells",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_AIStorytellerSituations_Characters_ScopeCharacterId",
                table: "AIStorytellerSituations",
                column: "ScopeCharacterId",
                principalTable: "Characters",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AIStorytellerSituations_Cells_ScopeRoomId",
                table: "AIStorytellerSituations");

            migrationBuilder.DropForeignKey(
                name: "FK_AIStorytellerSituations_Characters_ScopeCharacterId",
                table: "AIStorytellerSituations");

            migrationBuilder.DropIndex(
                name: "IX_AIStorytellerSituations_ScopeCharacterId",
                table: "AIStorytellerSituations");

            migrationBuilder.DropIndex(
                name: "IX_AIStorytellerSituations_ScopeRoomId",
                table: "AIStorytellerSituations");

            migrationBuilder.DropColumn(
                name: "ScopeCharacterId",
                table: "AIStorytellerSituations");

            migrationBuilder.DropColumn(
                name: "ScopeRoomId",
                table: "AIStorytellerSituations");
        }
    }
}
