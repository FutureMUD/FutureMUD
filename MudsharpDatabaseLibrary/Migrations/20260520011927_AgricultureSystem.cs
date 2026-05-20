using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MudSharp.Migrations
{
    /// <inheritdoc />
    public partial class AgricultureSystem : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "DefaultAgricultureFieldProfileId",
                table: "Terrains",
                type: "bigint(20)",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "AgricultureCropDefinitions",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint(20)", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(type: "varchar(200)", nullable: false, collation: "utf8_general_ci")
                        .Annotation("MySql:CharSet", "utf8"),
                    Description = table.Column<string>(type: "mediumtext", nullable: false, collation: "utf8_general_ci")
                        .Annotation("MySql:CharSet", "utf8"),
                    Category = table.Column<string>(type: "varchar(100)", nullable: false, collation: "utf8_general_ci")
                        .Annotation("MySql:CharSet", "utf8"),
                    Definition = table.Column<string>(type: "mediumtext", nullable: false, collation: "utf8_general_ci")
                        .Annotation("MySql:CharSet", "utf8")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "AgricultureFieldProfiles",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint(20)", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(type: "varchar(200)", nullable: false, collation: "utf8_general_ci")
                        .Annotation("MySql:CharSet", "utf8"),
                    Description = table.Column<string>(type: "mediumtext", nullable: false, collation: "utf8_general_ci")
                        .Annotation("MySql:CharSet", "utf8"),
                    Definition = table.Column<string>(type: "mediumtext", nullable: false, collation: "utf8_general_ci")
                        .Annotation("MySql:CharSet", "utf8")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "AgricultureHerdDefinitions",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint(20)", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(type: "varchar(200)", nullable: false, collation: "utf8_general_ci")
                        .Annotation("MySql:CharSet", "utf8"),
                    Description = table.Column<string>(type: "mediumtext", nullable: false, collation: "utf8_general_ci")
                        .Annotation("MySql:CharSet", "utf8"),
                    Definition = table.Column<string>(type: "mediumtext", nullable: false, collation: "utf8_general_ci")
                        .Annotation("MySql:CharSet", "utf8"),
                    NpcTemplateId = table.Column<long>(type: "bigint(20)", nullable: true),
                    NpcTemplateRevisionNumber = table.Column<int>(type: "int(11)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AgricultureHerdDefinitions_NpcTemplates",
                        columns: x => new { x.NpcTemplateId, x.NpcTemplateRevisionNumber },
                        principalTable: "NPCTemplates",
                        principalColumns: new[] { "Id", "RevisionNumber" },
                        onDelete: ReferentialAction.SetNull);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "AgricultureOperations",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint(20)", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(type: "varchar(200)", nullable: false, collation: "utf8_general_ci")
                        .Annotation("MySql:CharSet", "utf8"),
                    Description = table.Column<string>(type: "mediumtext", nullable: false, collation: "utf8_general_ci")
                        .Annotation("MySql:CharSet", "utf8"),
                    OperationType = table.Column<int>(type: "int(11)", nullable: false),
                    TargetType = table.Column<int>(type: "int(11)", nullable: false),
                    RequiredUse = table.Column<int>(type: "int(11)", nullable: false),
                    ResultUse = table.Column<int>(type: "int(11)", nullable: false),
                    ProjectId = table.Column<long>(type: "bigint(20)", nullable: false),
                    ProjectRevisionNumber = table.Column<int>(type: "int(11)", nullable: false),
                    CompletionProgId = table.Column<long>(type: "bigint(20)", nullable: true),
                    Definition = table.Column<string>(type: "mediumtext", nullable: false, collation: "utf8_general_ci")
                        .Annotation("MySql:CharSet", "utf8")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AgricultureOperations_FutureProgs",
                        column: x => x.CompletionProgId,
                        principalTable: "FutureProgs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_AgricultureOperations_Projects",
                        columns: x => new { x.ProjectId, x.ProjectRevisionNumber },
                        principalTable: "Projects",
                        principalColumns: new[] { "Id", "RevisionNumber" },
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "AgricultureWoodlandDefinitions",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint(20)", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(type: "varchar(200)", nullable: false, collation: "utf8_general_ci")
                        .Annotation("MySql:CharSet", "utf8"),
                    Description = table.Column<string>(type: "mediumtext", nullable: false, collation: "utf8_general_ci")
                        .Annotation("MySql:CharSet", "utf8"),
                    WoodlandType = table.Column<string>(type: "varchar(100)", nullable: false, collation: "utf8_general_ci")
                        .Annotation("MySql:CharSet", "utf8"),
                    Definition = table.Column<string>(type: "mediumtext", nullable: false, collation: "utf8_general_ci")
                        .Annotation("MySql:CharSet", "utf8")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "AgricultureFields",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint(20)", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    CellId = table.Column<long>(type: "bigint(20)", nullable: false),
                    ProfileId = table.Column<long>(type: "bigint(20)", nullable: false),
                    CurrentUse = table.Column<int>(type: "int(11)", nullable: false),
                    Moisture = table.Column<int>(type: "int", nullable: false),
                    Drainage = table.Column<int>(type: "int", nullable: false),
                    Nutrients = table.Column<int>(type: "int", nullable: false),
                    Salinity = table.Column<int>(type: "int", nullable: false),
                    Topsoil = table.Column<int>(type: "int", nullable: false),
                    Tilth = table.Column<int>(type: "int", nullable: false),
                    Rockiness = table.Column<int>(type: "int", nullable: false),
                    Weeds = table.Column<int>(type: "int", nullable: false),
                    Pests = table.Column<int>(type: "int", nullable: false),
                    Fence = table.Column<int>(type: "int", nullable: false),
                    Pasture = table.Column<int>(type: "int", nullable: false),
                    Condition = table.Column<int>(type: "int", nullable: false),
                    LastTickMudDateTime = table.Column<string>(type: "varchar(500)", nullable: true, collation: "utf8_general_ci")
                        .Annotation("MySql:CharSet", "utf8"),
                    Definition = table.Column<string>(type: "mediumtext", nullable: false, collation: "utf8_general_ci")
                        .Annotation("MySql:CharSet", "utf8")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AgricultureFields_Cells",
                        column: x => x.CellId,
                        principalTable: "Cells",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AgricultureFields_Profiles",
                        column: x => x.ProfileId,
                        principalTable: "AgricultureFieldProfiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "AgricultureFieldCrops",
                columns: table => new
                {
                    AgricultureFieldId = table.Column<long>(type: "bigint(20)", nullable: false),
                    CropDefinitionId = table.Column<long>(type: "bigint(20)", nullable: false),
                    Stage = table.Column<int>(type: "int(11)", nullable: false),
                    GrowthDays = table.Column<int>(type: "int(11)", nullable: false),
                    Health = table.Column<int>(type: "int(11)", nullable: false),
                    YieldPotential = table.Column<int>(type: "int(11)", nullable: false),
                    PlantedMudDateTime = table.Column<string>(type: "varchar(500)", nullable: true, collation: "utf8_general_ci")
                        .Annotation("MySql:CharSet", "utf8"),
                    Definition = table.Column<string>(type: "mediumtext", nullable: false, collation: "utf8_general_ci")
                        .Annotation("MySql:CharSet", "utf8")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.AgricultureFieldId);
                    table.ForeignKey(
                        name: "FK_AgricultureFieldCrops_Crops",
                        column: x => x.CropDefinitionId,
                        principalTable: "AgricultureCropDefinitions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AgricultureFieldCrops_Fields",
                        column: x => x.AgricultureFieldId,
                        principalTable: "AgricultureFields",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "AgricultureFieldHerds",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint(20)", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    AgricultureFieldId = table.Column<long>(type: "bigint(20)", nullable: false),
                    HerdDefinitionId = table.Column<long>(type: "bigint(20)", nullable: false),
                    HeadCount = table.Column<int>(type: "int(11)", nullable: false),
                    Condition = table.Column<double>(type: "double", nullable: false),
                    Definition = table.Column<string>(type: "mediumtext", nullable: false, collation: "utf8_general_ci")
                        .Annotation("MySql:CharSet", "utf8")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AgricultureFieldHerds_Fields",
                        column: x => x.AgricultureFieldId,
                        principalTable: "AgricultureFields",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AgricultureFieldHerds_Herds",
                        column: x => x.HerdDefinitionId,
                        principalTable: "AgricultureHerdDefinitions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "AgricultureFieldWoodlands",
                columns: table => new
                {
                    AgricultureFieldId = table.Column<long>(type: "bigint(20)", nullable: false),
                    WoodlandDefinitionId = table.Column<long>(type: "bigint(20)", nullable: false),
                    GrowthDays = table.Column<int>(type: "int(11)", nullable: false),
                    Health = table.Column<int>(type: "int(11)", nullable: false),
                    YieldPotential = table.Column<int>(type: "int(11)", nullable: false),
                    PlantedMudDateTime = table.Column<string>(type: "varchar(500)", nullable: true, collation: "utf8_general_ci")
                        .Annotation("MySql:CharSet", "utf8"),
                    Definition = table.Column<string>(type: "mediumtext", nullable: false, collation: "utf8_general_ci")
                        .Annotation("MySql:CharSet", "utf8")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.AgricultureFieldId);
                    table.ForeignKey(
                        name: "FK_AgricultureFieldWoodlands_Fields",
                        column: x => x.AgricultureFieldId,
                        principalTable: "AgricultureFields",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AgricultureFieldWoodlands_Woodlands",
                        column: x => x.WoodlandDefinitionId,
                        principalTable: "AgricultureWoodlandDefinitions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "AgricultureProjectContexts",
                columns: table => new
                {
                    ActiveProjectId = table.Column<long>(type: "bigint(20)", nullable: false),
                    AgricultureFieldId = table.Column<long>(type: "bigint(20)", nullable: false),
                    OperationId = table.Column<long>(type: "bigint(20)", nullable: false),
                    TargetType = table.Column<int>(type: "int(11)", nullable: false),
                    TargetId = table.Column<long>(type: "bigint(20)", nullable: true),
                    TargetText = table.Column<string>(type: "varchar(500)", nullable: true, collation: "utf8_general_ci")
                        .Annotation("MySql:CharSet", "utf8"),
                    ActorId = table.Column<long>(type: "bigint(20)", nullable: true),
                    Definition = table.Column<string>(type: "mediumtext", nullable: false, collation: "utf8_general_ci")
                        .Annotation("MySql:CharSet", "utf8")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.ActiveProjectId);
                    table.ForeignKey(
                        name: "FK_AgricultureProjectContexts_ActiveProjects",
                        column: x => x.ActiveProjectId,
                        principalTable: "ActiveProjects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AgricultureProjectContexts_Characters",
                        column: x => x.ActorId,
                        principalTable: "Characters",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_AgricultureProjectContexts_Fields",
                        column: x => x.AgricultureFieldId,
                        principalTable: "AgricultureFields",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AgricultureProjectContexts_Operations",
                        column: x => x.OperationId,
                        principalTable: "AgricultureOperations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "FK_Terrains_AgricultureFieldProfiles_idx",
                table: "Terrains",
                column: "DefaultAgricultureFieldProfileId");

            migrationBuilder.CreateIndex(
                name: "FK_AgricultureFieldCrops_Crops_idx",
                table: "AgricultureFieldCrops",
                column: "CropDefinitionId");

            migrationBuilder.CreateIndex(
                name: "FK_AgricultureFieldHerds_Fields_idx",
                table: "AgricultureFieldHerds",
                column: "AgricultureFieldId");

            migrationBuilder.CreateIndex(
                name: "FK_AgricultureFieldHerds_Herds_idx",
                table: "AgricultureFieldHerds",
                column: "HerdDefinitionId");

            migrationBuilder.CreateIndex(
                name: "FK_AgricultureFields_Profiles_idx",
                table: "AgricultureFields",
                column: "ProfileId");

            migrationBuilder.CreateIndex(
                name: "IX_AgricultureFields_CellId",
                table: "AgricultureFields",
                column: "CellId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "FK_AgricultureFieldWoodlands_Woodlands_idx",
                table: "AgricultureFieldWoodlands",
                column: "WoodlandDefinitionId");

            migrationBuilder.CreateIndex(
                name: "FK_AgricultureHerdDefinitions_NpcTemplates_idx",
                table: "AgricultureHerdDefinitions",
                columns: new[] { "NpcTemplateId", "NpcTemplateRevisionNumber" });

            migrationBuilder.CreateIndex(
                name: "FK_AgricultureOperations_FutureProgs_idx",
                table: "AgricultureOperations",
                column: "CompletionProgId");

            migrationBuilder.CreateIndex(
                name: "FK_AgricultureOperations_Projects_idx",
                table: "AgricultureOperations",
                columns: new[] { "ProjectId", "ProjectRevisionNumber" });

            migrationBuilder.CreateIndex(
                name: "FK_AgricultureProjectContexts_Actors_idx",
                table: "AgricultureProjectContexts",
                column: "ActorId");

            migrationBuilder.CreateIndex(
                name: "FK_AgricultureProjectContexts_Fields_idx",
                table: "AgricultureProjectContexts",
                column: "AgricultureFieldId");

            migrationBuilder.CreateIndex(
                name: "FK_AgricultureProjectContexts_Operations_idx",
                table: "AgricultureProjectContexts",
                column: "OperationId");

            migrationBuilder.AddForeignKey(
                name: "FK_Terrains_AgricultureFieldProfiles",
                table: "Terrains",
                column: "DefaultAgricultureFieldProfileId",
                principalTable: "AgricultureFieldProfiles",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Terrains_AgricultureFieldProfiles",
                table: "Terrains");

            migrationBuilder.DropTable(
                name: "AgricultureFieldCrops");

            migrationBuilder.DropTable(
                name: "AgricultureFieldHerds");

            migrationBuilder.DropTable(
                name: "AgricultureFieldWoodlands");

            migrationBuilder.DropTable(
                name: "AgricultureProjectContexts");

            migrationBuilder.DropTable(
                name: "AgricultureCropDefinitions");

            migrationBuilder.DropTable(
                name: "AgricultureHerdDefinitions");

            migrationBuilder.DropTable(
                name: "AgricultureWoodlandDefinitions");

            migrationBuilder.DropTable(
                name: "AgricultureFields");

            migrationBuilder.DropTable(
                name: "AgricultureOperations");

            migrationBuilder.DropTable(
                name: "AgricultureFieldProfiles");

            migrationBuilder.DropIndex(
                name: "FK_Terrains_AgricultureFieldProfiles_idx",
                table: "Terrains");

            migrationBuilder.DropColumn(
                name: "DefaultAgricultureFieldProfileId",
                table: "Terrains");
        }
    }
}
