using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MudSharp.Migrations
{
    /// <inheritdoc />
    public partial class AddNPCSkillPackages : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "NPCSkillPackages",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint(20)", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(type: "varchar(200)", nullable: false, collation: "utf8mb4_unicode_ci")
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "NPCSkillPackageSkills",
                columns: table => new
                {
                    NpcSkillPackageId = table.Column<long>(type: "bigint(20)", nullable: false),
                    TraitDefinitionId = table.Column<long>(type: "bigint(20)", nullable: false),
                    Chance = table.Column<double>(type: "double", nullable: false),
                    Mean = table.Column<double>(type: "double", nullable: false),
                    StandardDeviation = table.Column<double>(type: "double", nullable: false),
                    Skewness = table.Column<double>(type: "double", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => new { x.NpcSkillPackageId, x.TraitDefinitionId });
                    table.ForeignKey(
                        name: "FK_NPCSkillPackageSkills_Packages",
                        column: x => x.NpcSkillPackageId,
                        principalTable: "NPCSkillPackages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_NPCSkillPackageSkills_Traits",
                        column: x => x.TraitDefinitionId,
                        principalTable: "TraitDefinitions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Races_NPCSkillPackages",
                columns: table => new
                {
                    RaceId = table.Column<long>(type: "bigint(20)", nullable: false),
                    NpcSkillPackageId = table.Column<long>(type: "bigint(20)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => new { x.RaceId, x.NpcSkillPackageId });
                    table.ForeignKey(
                        name: "FK_RacesNPCSkillPackages_Packages",
                        column: x => x.NpcSkillPackageId,
                        principalTable: "NPCSkillPackages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RacesNPCSkillPackages_Races",
                        column: x => x.RaceId,
                        principalTable: "Races",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "UX_NPCSkillPackages_Name",
                table: "NPCSkillPackages",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "FK_NPCSkillPackageSkills_Traits_idx",
                table: "NPCSkillPackageSkills",
                column: "TraitDefinitionId");

            migrationBuilder.CreateIndex(
                name: "IX_Races_NPCSkillPackages_NpcSkillPackageId",
                table: "Races_NPCSkillPackages",
                column: "NpcSkillPackageId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "NPCSkillPackageSkills");

            migrationBuilder.DropTable(
                name: "Races_NPCSkillPackages");

            migrationBuilder.DropTable(
                name: "NPCSkillPackages");
        }
    }
}
