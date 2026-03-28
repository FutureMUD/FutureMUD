using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MudSharp.Migrations
{
    /// <inheritdoc />
    public partial class EstateProbateMorgueWorkflow : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "MorgueOfficeLocationId",
                table: "EconomicZones",
                type: "bigint(20)",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "MorgueStorageLocationId",
                table: "EconomicZones",
                type: "bigint(20)",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "CorpseRecoveryReports",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint(20)", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    LegalAuthorityId = table.Column<long>(type: "bigint(20)", nullable: false),
                    EconomicZoneId = table.Column<long>(type: "bigint(20)", nullable: false),
                    CorpseId = table.Column<long>(type: "bigint(20)", nullable: false),
                    SourceCellId = table.Column<long>(type: "bigint(20)", nullable: false),
                    DestinationCellId = table.Column<long>(type: "bigint(20)", nullable: false),
                    ReporterId = table.Column<long>(type: "bigint(20)", nullable: true),
                    AssignedPatrolId = table.Column<long>(type: "bigint(20)", nullable: true),
                    Status = table.Column<int>(type: "int(11)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CorpseRecoveryReports", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CorpseRecoveryReports_Characters",
                        column: x => x.ReporterId,
                        principalTable: "Characters",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_CorpseRecoveryReports_DestinationCells",
                        column: x => x.DestinationCellId,
                        principalTable: "Cells",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CorpseRecoveryReports_EconomicZones",
                        column: x => x.EconomicZoneId,
                        principalTable: "EconomicZones",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CorpseRecoveryReports_GameItems",
                        column: x => x.CorpseId,
                        principalTable: "GameItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CorpseRecoveryReports_LegalAuthorities",
                        column: x => x.LegalAuthorityId,
                        principalTable: "LegalAuthorities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CorpseRecoveryReports_Patrols",
                        column: x => x.AssignedPatrolId,
                        principalTable: "Patrols",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_CorpseRecoveryReports_SourceCells",
                        column: x => x.SourceCellId,
                        principalTable: "Cells",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "ProbateLocations",
                columns: table => new
                {
                    EconomicZoneId = table.Column<long>(type: "bigint(20)", nullable: false),
                    CellId = table.Column<long>(type: "bigint(20)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => new { x.EconomicZoneId, x.CellId });
                    table.ForeignKey(
                        name: "FK_ProbateLocations_Cells",
                        column: x => x.CellId,
                        principalTable: "Cells",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ProbateLocations_EconomicZones",
                        column: x => x.EconomicZoneId,
                        principalTable: "EconomicZones",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_EconomicZones_MorgueOfficeLocationId",
                table: "EconomicZones",
                column: "MorgueOfficeLocationId");

            migrationBuilder.CreateIndex(
                name: "IX_EconomicZones_MorgueStorageLocationId",
                table: "EconomicZones",
                column: "MorgueStorageLocationId");

            migrationBuilder.CreateIndex(
                name: "IX_CorpseRecoveryReports_AssignedPatrolId",
                table: "CorpseRecoveryReports",
                column: "AssignedPatrolId");

            migrationBuilder.CreateIndex(
                name: "IX_CorpseRecoveryReports_CorpseId",
                table: "CorpseRecoveryReports",
                column: "CorpseId");

            migrationBuilder.CreateIndex(
                name: "IX_CorpseRecoveryReports_DestinationCellId",
                table: "CorpseRecoveryReports",
                column: "DestinationCellId");

            migrationBuilder.CreateIndex(
                name: "IX_CorpseRecoveryReports_EconomicZoneId",
                table: "CorpseRecoveryReports",
                column: "EconomicZoneId");

            migrationBuilder.CreateIndex(
                name: "IX_CorpseRecoveryReports_LegalAuthorityId",
                table: "CorpseRecoveryReports",
                column: "LegalAuthorityId");

            migrationBuilder.CreateIndex(
                name: "IX_CorpseRecoveryReports_ReporterId",
                table: "CorpseRecoveryReports",
                column: "ReporterId");

            migrationBuilder.CreateIndex(
                name: "IX_CorpseRecoveryReports_SourceCellId",
                table: "CorpseRecoveryReports",
                column: "SourceCellId");

            migrationBuilder.CreateIndex(
                name: "IX_ProbateLocations_CellId",
                table: "ProbateLocations",
                column: "CellId");

            migrationBuilder.AddForeignKey(
                name: "FK_EconomicZones_MorgueOfficeLocations",
                table: "EconomicZones",
                column: "MorgueOfficeLocationId",
                principalTable: "Cells",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_EconomicZones_MorgueStorageLocations",
                table: "EconomicZones",
                column: "MorgueStorageLocationId",
                principalTable: "Cells",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EconomicZones_MorgueOfficeLocations",
                table: "EconomicZones");

            migrationBuilder.DropForeignKey(
                name: "FK_EconomicZones_MorgueStorageLocations",
                table: "EconomicZones");

            migrationBuilder.DropTable(
                name: "CorpseRecoveryReports");

            migrationBuilder.DropTable(
                name: "ProbateLocations");

            migrationBuilder.DropIndex(
                name: "IX_EconomicZones_MorgueOfficeLocationId",
                table: "EconomicZones");

            migrationBuilder.DropIndex(
                name: "IX_EconomicZones_MorgueStorageLocationId",
                table: "EconomicZones");

            migrationBuilder.DropColumn(
                name: "MorgueOfficeLocationId",
                table: "EconomicZones");

            migrationBuilder.DropColumn(
                name: "MorgueStorageLocationId",
                table: "EconomicZones");
        }
    }
}
