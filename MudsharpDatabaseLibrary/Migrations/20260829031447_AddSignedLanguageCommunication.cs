using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MudSharp.Migrations
{
    /// <inheritdoc />
    public partial class AddSignedLanguageCommunication : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "CurrentSignedLanguageId",
                table: "Characters",
                type: "bigint(20)",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "CurrentSignedLanguageVarietyId",
                table: "Characters",
                type: "bigint(20)",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "SignedLanguages",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint(20)", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: false, collation: "utf8_general_ci")
                        .Annotation("MySql:CharSet", "utf8"),
                    DifficultyModelId = table.Column<long>(type: "bigint(20)", nullable: false),
                    LinkedTraitId = table.Column<long>(type: "bigint(20)", nullable: false),
                    UnknownLanguageDescription = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: false, collation: "utf8_general_ci")
                        .Annotation("MySql:CharSet", "utf8"),
                    LanguageObfuscationFactor = table.Column<double>(type: "double", nullable: false, defaultValue: 0.20000000000000001)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SignedLanguages_DifficultyModels",
                        column: x => x.DifficultyModelId,
                        principalTable: "LanguageDifficultyModels",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SignedLanguages_TraitDefinitions",
                        column: x => x.LinkedTraitId,
                        principalTable: "TraitDefinitions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Characters_SignedLanguages",
                columns: table => new
                {
                    CharacterId = table.Column<long>(type: "bigint(20)", nullable: false),
                    SignedLanguageId = table.Column<long>(type: "bigint(20)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => new { x.CharacterId, x.SignedLanguageId });
                    table.ForeignKey(
                        name: "FK_Characters_SignedLanguages_Characters",
                        column: x => x.CharacterId,
                        principalTable: "Characters",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Characters_SignedLanguages_Languages",
                        column: x => x.SignedLanguageId,
                        principalTable: "SignedLanguages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "SignedLanguageArticulationProfiles",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint(20)", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    SignedLanguageId = table.Column<long>(type: "bigint(20)", nullable: false),
                    BodyPrototypeId = table.Column<long>(type: "bigint(20)", nullable: false),
                    Name = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: false, collation: "utf8_general_ci")
                        .Annotation("MySql:CharSet", "utf8")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SignedLanguageArticulationProfiles_BodyProtos",
                        column: x => x.BodyPrototypeId,
                        principalTable: "BodyProtos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SignedLanguageArticulationProfiles_Languages",
                        column: x => x.SignedLanguageId,
                        principalTable: "SignedLanguages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "SignedLanguageMutualIntelligibilities",
                columns: table => new
                {
                    ListenerLanguageId = table.Column<long>(type: "bigint(20)", nullable: false),
                    TargetLanguageId = table.Column<long>(type: "bigint(20)", nullable: false),
                    IntelligibilityDifficulty = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => new { x.ListenerLanguageId, x.TargetLanguageId });
                    table.ForeignKey(
                        name: "FK_SignedLanguageMutual_Listener",
                        column: x => x.ListenerLanguageId,
                        principalTable: "SignedLanguages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SignedLanguageMutual_Target",
                        column: x => x.TargetLanguageId,
                        principalTable: "SignedLanguages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "SignedLanguageVarieties",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint(20)", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    SignedLanguageId = table.Column<long>(type: "bigint(20)", nullable: false),
                    Name = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: false, collation: "utf8_general_ci")
                        .Annotation("MySql:CharSet", "utf8"),
                    Description = table.Column<string>(type: "varchar(2000)", maxLength: 2000, nullable: false, collation: "utf8_general_ci")
                        .Annotation("MySql:CharSet", "utf8"),
                    Suffix = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: false, collation: "utf8_general_ci")
                        .Annotation("MySql:CharSet", "utf8"),
                    VagueSuffix = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: false, collation: "utf8_general_ci")
                        .Annotation("MySql:CharSet", "utf8"),
                    RecognitionDifficulty = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SignedLanguageVarieties_SignedLanguages",
                        column: x => x.SignedLanguageId,
                        principalTable: "SignedLanguages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "SignedLanguageArticulationRequirements",
                columns: table => new
                {
                    ArticulationProfileId = table.Column<long>(type: "bigint(20)", nullable: false),
                    BodypartShapeId = table.Column<long>(type: "bigint(20)", nullable: false),
                    MinimumCount = table.Column<int>(type: "int", nullable: false),
                    PreferredCount = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => new { x.ArticulationProfileId, x.BodypartShapeId });
                    table.ForeignKey(
                        name: "FK_SignedLanguageArticulationRequirements_BodypartShapes",
                        column: x => x.BodypartShapeId,
                        principalTable: "BodypartShape",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SignedLanguageArticulationRequirements_Profiles",
                        column: x => x.ArticulationProfileId,
                        principalTable: "SignedLanguageArticulationProfiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Characters_SignedLanguageVarieties",
                columns: table => new
                {
                    CharacterId = table.Column<long>(type: "bigint(20)", nullable: false),
                    SignedLanguageVarietyId = table.Column<long>(type: "bigint(20)", nullable: false),
                    Familiarity = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => new { x.CharacterId, x.SignedLanguageVarietyId });
                    table.ForeignKey(
                        name: "FK_Characters_SignedLanguageVarieties_Characters",
                        column: x => x.CharacterId,
                        principalTable: "Characters",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Characters_SignedLanguageVarieties_Varieties",
                        column: x => x.SignedLanguageVarietyId,
                        principalTable: "SignedLanguageVarieties",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "FK_Characters_CurrentSignedLanguage_idx",
                table: "Characters",
                column: "CurrentSignedLanguageId");

            migrationBuilder.CreateIndex(
                name: "FK_Characters_CurrentSignedLanguageVariety_idx",
                table: "Characters",
                column: "CurrentSignedLanguageVarietyId");

            migrationBuilder.CreateIndex(
                name: "FK_Characters_SignedLanguages_Languages_idx",
                table: "Characters_SignedLanguages",
                column: "SignedLanguageId");

            migrationBuilder.CreateIndex(
                name: "FK_Characters_SignedLanguageVarieties_Varieties_idx",
                table: "Characters_SignedLanguageVarieties",
                column: "SignedLanguageVarietyId");

            migrationBuilder.CreateIndex(
                name: "FK_SignedLanguageArticulationProfiles_BodyProtos_idx",
                table: "SignedLanguageArticulationProfiles",
                column: "BodyPrototypeId");

            migrationBuilder.CreateIndex(
                name: "UX_SignedLanguageArticulationProfiles_Language_Name",
                table: "SignedLanguageArticulationProfiles",
                columns: new[] { "SignedLanguageId", "Name" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "FK_SignedLanguageArticulationRequirements_BodypartShapes_idx",
                table: "SignedLanguageArticulationRequirements",
                column: "BodypartShapeId");

            migrationBuilder.CreateIndex(
                name: "FK_SignedLanguageMutual_Target_idx",
                table: "SignedLanguageMutualIntelligibilities",
                column: "TargetLanguageId");

            migrationBuilder.CreateIndex(
                name: "FK_SignedLanguages_DifficultyModels_idx",
                table: "SignedLanguages",
                column: "DifficultyModelId");

            migrationBuilder.CreateIndex(
                name: "FK_SignedLanguages_TraitDefinitions_idx",
                table: "SignedLanguages",
                column: "LinkedTraitId");

            migrationBuilder.CreateIndex(
                name: "UX_SignedLanguages_Name",
                table: "SignedLanguages",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "UX_SignedLanguageVarieties_Language_Name",
                table: "SignedLanguageVarieties",
                columns: new[] { "SignedLanguageId", "Name" },
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Characters_CurrentSignedLanguage",
                table: "Characters",
                column: "CurrentSignedLanguageId",
                principalTable: "SignedLanguages",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Characters_CurrentSignedLanguageVariety",
                table: "Characters",
                column: "CurrentSignedLanguageVarietyId",
                principalTable: "SignedLanguageVarieties",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Characters_CurrentSignedLanguage",
                table: "Characters");

            migrationBuilder.DropForeignKey(
                name: "FK_Characters_CurrentSignedLanguageVariety",
                table: "Characters");

            migrationBuilder.DropTable(
                name: "Characters_SignedLanguages");

            migrationBuilder.DropTable(
                name: "Characters_SignedLanguageVarieties");

            migrationBuilder.DropTable(
                name: "SignedLanguageArticulationRequirements");

            migrationBuilder.DropTable(
                name: "SignedLanguageMutualIntelligibilities");

            migrationBuilder.DropTable(
                name: "SignedLanguageVarieties");

            migrationBuilder.DropTable(
                name: "SignedLanguageArticulationProfiles");

            migrationBuilder.DropTable(
                name: "SignedLanguages");

            migrationBuilder.DropIndex(
                name: "FK_Characters_CurrentSignedLanguage_idx",
                table: "Characters");

            migrationBuilder.DropIndex(
                name: "FK_Characters_CurrentSignedLanguageVariety_idx",
                table: "Characters");

            migrationBuilder.DropColumn(
                name: "CurrentSignedLanguageId",
                table: "Characters");

            migrationBuilder.DropColumn(
                name: "CurrentSignedLanguageVarietyId",
                table: "Characters");
        }
    }
}
