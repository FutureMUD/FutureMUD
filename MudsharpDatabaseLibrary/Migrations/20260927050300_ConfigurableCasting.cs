using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MudSharp.Migrations
{
    /// <inheritdoc />
    public partial class ConfigurableCasting : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CharacterAcquiredSpells",
                columns: table => new
                {
                    CharacterId = table.Column<long>(type: "bigint(20)", nullable: false),
                    MagicSpellId = table.Column<long>(type: "bigint(20)", nullable: false),
                    AcquiredUtc = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    Provenance = table.Column<string>(type: "text", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ControlledGrade = table.Column<int>(type: "int", nullable: false),
                    ProfileVersion = table.Column<int>(type: "int", nullable: false),
                    NextMasteryUtc = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    StateVersion = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CharacterAcquiredSpells", x => new { x.CharacterId, x.MagicSpellId });
                    table.ForeignKey(
                        name: "FK_CharacterAcquiredSpells_Characters_CharacterId",
                        column: x => x.CharacterId,
                        principalTable: "Characters",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CharacterAcquiredSpells_MagicSpells_MagicSpellId",
                        column: x => x.MagicSpellId,
                        principalTable: "MagicSpells",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "CharacterCastingEnrolments",
                columns: table => new
                {
                    CharacterId = table.Column<long>(type: "bigint(20)", nullable: false),
                    CapabilityIdentity = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    MagicCapabilityId = table.Column<long>(type: "bigint", nullable: false),
                    EnrolledUtc = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    CompletedStartingGrantVersion = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CharacterCastingEnrolments", x => new { x.CharacterId, x.CapabilityIdentity });
                    table.ForeignKey(
                        name: "FK_CharacterCastingEnrolments_Characters_CharacterId",
                        column: x => x.CharacterId,
                        principalTable: "Characters",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "CharacterMagicSkillOpportunities",
                columns: table => new
                {
                    CharacterId = table.Column<long>(type: "bigint(20)", nullable: false),
                    TraitDefinitionId = table.Column<long>(type: "bigint(20)", nullable: false),
                    NextOpportunityUtc = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    StateVersion = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CharacterMagicSkillOpportunities", x => new { x.CharacterId, x.TraitDefinitionId });
                    table.ForeignKey(
                        name: "FK_CharacterMagicSkillOpportunities_Characters_CharacterId",
                        column: x => x.CharacterId,
                        principalTable: "Characters",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CharacterMagicSkillOpportunities_TraitDefinitions_TraitDefin~",
                        column: x => x.TraitDefinitionId,
                        principalTable: "TraitDefinitions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "MagicCastingOperations",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    CharacterId = table.Column<long>(type: "bigint", nullable: false),
                    ActorId = table.Column<long>(type: "bigint", nullable: false),
                    BodyId = table.Column<long>(type: "bigint", nullable: false),
                    MagicCapabilityId = table.Column<long>(type: "bigint", nullable: false),
                    MagicSpellId = table.Column<long>(type: "bigint", nullable: false),
                    TraitDefinitionId = table.Column<long>(type: "bigint", nullable: false),
                    ReserveId = table.Column<long>(type: "bigint", nullable: false),
                    Stage = table.Column<string>(type: "varchar(40)", maxLength: 40, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Definition = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    CreatedUtc = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    UpdatedUtc = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    Diagnostic = table.Column<string>(type: "text", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MagicCastingOperations", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_CharacterAcquiredSpells_MagicSpellId",
                table: "CharacterAcquiredSpells",
                column: "MagicSpellId");

            migrationBuilder.CreateIndex(
                name: "IX_CharacterMagicSkillOpportunities_TraitDefinitionId",
                table: "CharacterMagicSkillOpportunities",
                column: "TraitDefinitionId");

            migrationBuilder.CreateIndex(
                name: "IX_MagicCastingOperations_CharacterId_Stage",
                table: "MagicCastingOperations",
                columns: new[] { "CharacterId", "Stage" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CharacterAcquiredSpells");

            migrationBuilder.DropTable(
                name: "CharacterCastingEnrolments");

            migrationBuilder.DropTable(
                name: "CharacterMagicSkillOpportunities");

            migrationBuilder.DropTable(
                name: "MagicCastingOperations");
        }
    }
}
