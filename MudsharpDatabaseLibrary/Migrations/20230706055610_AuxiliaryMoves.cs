using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MudSharp.Migrations
{
    /// <inheritdoc />
    public partial class AuxiliaryMoves : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "AuxillaryPercentage",
                table: "CharacterCombatSettings",
                newName: "AuxiliaryPercentage");

            migrationBuilder.CreateTable(
                name: "CombatActions",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    UsabilityProgId = table.Column<long>(type: "bigint(20)", nullable: true),
                    RecoveryDifficultySuccess = table.Column<int>(type: "int", nullable: false),
                    RecoveryDifficultyFailure = table.Column<int>(type: "int", nullable: false),
                    MoveType = table.Column<int>(type: "int", nullable: false),
                    Intentions = table.Column<long>(type: "bigint", nullable: false),
                    ExertionLevel = table.Column<int>(type: "int", nullable: false),
                    Weighting = table.Column<double>(type: "double", nullable: false),
                    StaminaCost = table.Column<double>(type: "double", nullable: false),
                    BaseDelay = table.Column<double>(type: "double", nullable: false),
                    AdditionalInfo = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    RequiredPositionStateIds = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    MoveDifficulty = table.Column<int>(type: "int", nullable: false),
                    TraitDefinitionId = table.Column<long>(type: "bigint(20)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CombatActions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CombatActions_FutureProgs_UsabilityProgId",
                        column: x => x.UsabilityProgId,
                        principalTable: "FutureProgs",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_CombatActions_TraitDefinitions_TraitDefinitionId",
                        column: x => x.TraitDefinitionId,
                        principalTable: "TraitDefinitions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "CombatMessages_CombatActions",
                columns: table => new
                {
                    CombatMessageId = table.Column<long>(type: "bigint(20)", nullable: false),
                    CombatActionId = table.Column<long>(type: "bigint(20)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => new { x.CombatMessageId, x.CombatActionId });
                    table.ForeignKey(
                        name: "FK_CombatMessages_CombatActions_CombatMessages",
                        column: x => x.CombatMessageId,
                        principalTable: "CombatMessages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CombatMessages_CombatActions_WeaponAttacks",
                        column: x => x.CombatActionId,
                        principalTable: "CombatActions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_CombatActions_TraitDefinitionId",
                table: "CombatActions",
                column: "TraitDefinitionId");

            migrationBuilder.CreateIndex(
                name: "IX_CombatActions_UsabilityProgId",
                table: "CombatActions",
                column: "UsabilityProgId");

            migrationBuilder.CreateIndex(
                name: "FK_CombatMessages_CombatActions_WeaponAttacks_idx",
                table: "CombatMessages_CombatActions",
                column: "CombatActionId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CombatMessages_CombatActions");

            migrationBuilder.DropTable(
                name: "CombatActions");

            migrationBuilder.RenameColumn(
                name: "AuxiliaryPercentage",
                table: "CharacterCombatSettings",
                newName: "AuxillaryPercentage");
        }
    }
}
