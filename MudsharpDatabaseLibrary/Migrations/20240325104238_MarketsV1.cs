using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MudSharp.Migrations
{
    /// <inheritdoc />
    public partial class MarketsV1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "MarketCategories",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Description = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ElasticityFactorAbove = table.Column<double>(type: "double", nullable: false),
                    ElasticityFactorBelow = table.Column<double>(type: "double", nullable: false),
                    Tags = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MarketCategories", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "MarketInfluenceTemplates",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Description = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    CharacterKnowsAboutInfluenceProgId = table.Column<long>(type: "bigint(20)", nullable: false),
                    Impacts = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    TemplateSummary = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MarketInfluenceTemplates", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MarketInfluenceTemplates_FutureProgs_CharacterKnowsAboutInfl~",
                        column: x => x.CharacterKnowsAboutInfluenceProgId,
                        principalTable: "FutureProgs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Markets",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Description = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    EconomicZoneId = table.Column<long>(type: "bigint(20)", nullable: false),
                    MarketPriceFormula = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Markets", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Markets_EconomicZones_EconomicZoneId",
                        column: x => x.EconomicZoneId,
                        principalTable: "EconomicZones",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "MarketInfluences",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Description = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    MarketId = table.Column<long>(type: "bigint", nullable: false),
                    AppliesFrom = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    AppliesUntil = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    CharacterKnowsAboutInfluenceProgId = table.Column<long>(type: "bigint(20)", nullable: false),
                    MarketInfluenceTemplateId = table.Column<long>(type: "bigint", nullable: true),
                    Impacts = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MarketInfluences", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MarketInfluences_FutureProgs_CharacterKnowsAboutInfluencePro~",
                        column: x => x.CharacterKnowsAboutInfluenceProgId,
                        principalTable: "FutureProgs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MarketInfluences_MarketInfluenceTemplates_MarketInfluenceTem~",
                        column: x => x.MarketInfluenceTemplateId,
                        principalTable: "MarketInfluenceTemplates",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_MarketInfluences_Markets_MarketId",
                        column: x => x.MarketId,
                        principalTable: "Markets",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "MarketMarketCategory",
                columns: table => new
                {
                    MarketCategoriesId = table.Column<long>(type: "bigint", nullable: false),
                    MarketsId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MarketMarketCategory", x => new { x.MarketCategoriesId, x.MarketsId });
                    table.ForeignKey(
                        name: "FK_MarketMarketCategory_MarketCategories_MarketCategoriesId",
                        column: x => x.MarketCategoriesId,
                        principalTable: "MarketCategories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MarketMarketCategory_Markets_MarketsId",
                        column: x => x.MarketsId,
                        principalTable: "Markets",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_MarketInfluences_CharacterKnowsAboutInfluenceProgId",
                table: "MarketInfluences",
                column: "CharacterKnowsAboutInfluenceProgId");

            migrationBuilder.CreateIndex(
                name: "IX_MarketInfluences_MarketId",
                table: "MarketInfluences",
                column: "MarketId");

            migrationBuilder.CreateIndex(
                name: "IX_MarketInfluences_MarketInfluenceTemplateId",
                table: "MarketInfluences",
                column: "MarketInfluenceTemplateId");

            migrationBuilder.CreateIndex(
                name: "IX_MarketInfluenceTemplates_CharacterKnowsAboutInfluenceProgId",
                table: "MarketInfluenceTemplates",
                column: "CharacterKnowsAboutInfluenceProgId");

            migrationBuilder.CreateIndex(
                name: "IX_MarketMarketCategory_MarketsId",
                table: "MarketMarketCategory",
                column: "MarketsId");

            migrationBuilder.CreateIndex(
                name: "IX_Markets_EconomicZoneId",
                table: "Markets",
                column: "EconomicZoneId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MarketInfluences");

            migrationBuilder.DropTable(
                name: "MarketMarketCategory");

            migrationBuilder.DropTable(
                name: "MarketInfluenceTemplates");

            migrationBuilder.DropTable(
                name: "MarketCategories");

            migrationBuilder.DropTable(
                name: "Markets");
        }
    }
}
