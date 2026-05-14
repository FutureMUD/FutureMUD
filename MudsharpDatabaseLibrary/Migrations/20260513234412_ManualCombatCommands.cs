using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MudSharp.Migrations
{
    /// <inheritdoc />
    public partial class ManualCombatCommands : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ManualCombatCommands",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint(20)", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(type: "varchar(100)", nullable: false, collation: "utf8mb4_unicode_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    PrimaryVerb = table.Column<string>(type: "varchar(50)", nullable: false, collation: "utf8mb4_unicode_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    AdditionalVerbs = table.Column<string>(type: "varchar(500)", nullable: false, defaultValueSql: "''", collation: "utf8mb4_unicode_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ActionKind = table.Column<int>(type: "int(11)", nullable: false),
                    WeaponAttackId = table.Column<long>(type: "bigint(20)", nullable: true),
                    CombatActionId = table.Column<long>(type: "bigint(20)", nullable: true),
                    PlayerUsable = table.Column<ulong>(type: "bit(1)", nullable: false, defaultValueSql: "b'1'"),
                    NpcUsable = table.Column<ulong>(type: "bit(1)", nullable: false, defaultValueSql: "b'1'"),
                    UsabilityProgId = table.Column<long>(type: "bigint(20)", nullable: true),
                    CooldownSeconds = table.Column<double>(type: "double", nullable: false),
                    CooldownMessage = table.Column<string>(type: "varchar(500)", nullable: false, defaultValueSql: "'You must wait a short time before doing that again.'", collation: "utf8mb4_unicode_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    DefaultAiWeightMultiplier = table.Column<double>(type: "double", nullable: false, defaultValueSql: "'1'")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ManualCombatCommands", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ManualCombatCommands_CombatActions",
                        column: x => x.CombatActionId,
                        principalTable: "CombatActions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ManualCombatCommands_FutureProgs",
                        column: x => x.UsabilityProgId,
                        principalTable: "FutureProgs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_ManualCombatCommands_WeaponAttacks",
                        column: x => x.WeaponAttackId,
                        principalTable: "WeaponAttacks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "CharacterCombatSettings_ManualCombatCommands",
                columns: table => new
                {
                    CharacterCombatSettingId = table.Column<long>(type: "bigint(20)", nullable: false),
                    ManualCombatCommandId = table.Column<long>(type: "bigint(20)", nullable: false),
                    WeightMultiplier = table.Column<double>(type: "double", nullable: false, defaultValueSql: "'1'")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => new { x.CharacterCombatSettingId, x.ManualCombatCommandId });
                    table.ForeignKey(
                        name: "FK_CCS_ManualCombatCommands_CharacterCombatSettings",
                        column: x => x.CharacterCombatSettingId,
                        principalTable: "CharacterCombatSettings",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CCS_ManualCombatCommands_ManualCombatCommands",
                        column: x => x.ManualCombatCommandId,
                        principalTable: "ManualCombatCommands",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "FK_CCS_ManualCombatCommands_ManualCombatCommands_idx",
                table: "CharacterCombatSettings_ManualCombatCommands",
                column: "ManualCombatCommandId");

            migrationBuilder.CreateIndex(
                name: "FK_ManualCombatCommands_CombatActions_idx",
                table: "ManualCombatCommands",
                column: "CombatActionId");

            migrationBuilder.CreateIndex(
                name: "FK_ManualCombatCommands_FutureProgs_idx",
                table: "ManualCombatCommands",
                column: "UsabilityProgId");

            migrationBuilder.CreateIndex(
                name: "FK_ManualCombatCommands_WeaponAttacks_idx",
                table: "ManualCombatCommands",
                column: "WeaponAttackId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CharacterCombatSettings_ManualCombatCommands");

            migrationBuilder.DropTable(
                name: "ManualCombatCommands");
        }
    }
}
