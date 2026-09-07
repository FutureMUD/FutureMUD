using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MudSharp.Migrations
{
    /// <inheritdoc />
    public partial class ChargenSkillSelectionGroups : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ChargenSkillSelectionGroups",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    StableKey = table.Column<string>(type: "varchar(191)", maxLength: 191, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Name = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Description = table.Column<string>(type: "text", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    DisplayOrder = table.Column<int>(type: "int", nullable: false),
                    Enabled = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    Retired = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    MinimumPicks = table.Column<int>(type: "int", nullable: false),
                    MaximumPicks = table.Column<int>(type: "int", nullable: false),
                    Revision = table.Column<int>(type: "int", nullable: false),
                    ExistingSkillPolicy = table.Column<int>(type: "int", nullable: false),
                    EligibilityProgId = table.Column<long>(type: "bigint(20)", nullable: true),
                    MemberEligibilityProgId = table.Column<long>(type: "bigint(20)", nullable: true),
                    SeedBaseline = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChargenSkillSelectionGroups", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ChargenSkillSelectionGroups_FutureProgs_EligibilityProgId",
                        column: x => x.EligibilityProgId,
                        principalTable: "FutureProgs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ChargenSkillSelectionGroups_FutureProgs_MemberEligibilityPro~",
                        column: x => x.MemberEligibilityProgId,
                        principalTable: "FutureProgs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "ChargenSkillSelectionGroupMembers",
                columns: table => new
                {
                    GroupId = table.Column<long>(type: "bigint", nullable: false),
                    TraitDefinitionId = table.Column<long>(type: "bigint(20)", nullable: false),
                    DisplayOrder = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChargenSkillSelectionGroupMembers", x => new { x.GroupId, x.TraitDefinitionId });
                    table.ForeignKey(
                        name: "FK_ChargenSkillSelectionGroupMembers_ChargenSkillSelectionGroup~",
                        column: x => x.GroupId,
                        principalTable: "ChargenSkillSelectionGroups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ChargenSkillSelectionGroupMembers_TraitDefinitions_TraitDefi~",
                        column: x => x.TraitDefinitionId,
                        principalTable: "TraitDefinitions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_ChargenSkillSelectionGroupMembers_TraitDefinitionId",
                table: "ChargenSkillSelectionGroupMembers",
                column: "TraitDefinitionId");

            migrationBuilder.CreateIndex(
                name: "IX_ChargenSkillSelectionGroups_EligibilityProgId",
                table: "ChargenSkillSelectionGroups",
                column: "EligibilityProgId");

            migrationBuilder.CreateIndex(
                name: "IX_ChargenSkillSelectionGroups_MemberEligibilityProgId",
                table: "ChargenSkillSelectionGroups",
                column: "MemberEligibilityProgId");

            migrationBuilder.CreateIndex(
                name: "IX_ChargenSkillSelectionGroups_StableKey",
                table: "ChargenSkillSelectionGroups",
                column: "StableKey",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ChargenSkillSelectionGroupMembers");

            migrationBuilder.DropTable(
                name: "ChargenSkillSelectionGroups");
        }
    }
}
