using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MudSharp.Migrations
{
    /// <inheritdoc />
    public partial class AuxiliaryMoves2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "AuxiliaryProgId",
                table: "CombatMessages",
                type: "bigint(20)",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Races_CombatActions",
                columns: table => new
                {
                    RaceId = table.Column<long>(type: "bigint(20)", nullable: false),
                    CombatActionId = table.Column<long>(type: "bigint(20)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => new { x.RaceId, x.CombatActionId });
                    table.ForeignKey(
                        name: "FK_Races_CombatActions_CombatActions",
                        column: x => x.CombatActionId,
                        principalTable: "CombatActions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Races_CombatActions_Races",
                        column: x => x.RaceId,
                        principalTable: "Races",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_CombatMessages_AuxiliaryProgId",
                table: "CombatMessages",
                column: "AuxiliaryProgId");

            migrationBuilder.CreateIndex(
                name: "IX_Races_CombatActions_CombatActionId",
                table: "Races_CombatActions",
                column: "CombatActionId");

            migrationBuilder.AddForeignKey(
                name: "FK_CombatMessages_FutureProgs_Auxiliary",
                table: "CombatMessages",
                column: "AuxiliaryProgId",
                principalTable: "FutureProgs",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CombatMessages_FutureProgs_Auxiliary",
                table: "CombatMessages");

            migrationBuilder.DropTable(
                name: "Races_CombatActions");

            migrationBuilder.DropIndex(
                name: "IX_CombatMessages_AuxiliaryProgId",
                table: "CombatMessages");

            migrationBuilder.DropColumn(
                name: "AuxiliaryProgId",
                table: "CombatMessages");
        }
    }
}
