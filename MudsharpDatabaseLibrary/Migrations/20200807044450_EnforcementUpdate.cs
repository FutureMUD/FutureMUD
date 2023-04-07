using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace MudSharp.Migrations
{
    public partial class EnforcementUpdate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "FilterProgId",
                table: "EnforcementAuthorities",
                type: "bigint(20)",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "PatrolRoutes",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint(20)", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(type: "varchar(100)", nullable: true),
                    LegalAuthorityId = table.Column<long>(type: "bigint(20)", nullable: false),
                    LingerTimeMajorNode = table.Column<double>(type: "DOUBLE", nullable: false),
                    LingerTimeMinorNode = table.Column<double>(type: "DOUBLE", nullable: false),
                    Priority = table.Column<int>(type: "int(11)", nullable: false),
                    PatrolStrategy = table.Column<string>(type: "varchar(100)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PatrolRoutes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PatrolRoutes_LegalAuthorities",
                        column: x => x.LegalAuthorityId,
                        principalTable: "LegalAuthorities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PatrolRoutesNodes",
                columns: table => new
                {
                    PatrolRouteId = table.Column<long>(type: "bigint(20)", nullable: false),
                    CellId = table.Column<long>(type: "bigint(20)", nullable: false),
                    Order = table.Column<int>(type: "int(11)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => new { x.PatrolRouteId, x.CellId });
                    table.ForeignKey(
                        name: "FK_PatrolRoutesNodes_Cells",
                        column: x => x.CellId,
                        principalTable: "Cells",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PatrolRoutesNodes_PatrolRoutes",
                        column: x => x.PatrolRouteId,
                        principalTable: "PatrolRoutes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PatrolRoutesNumbers",
                columns: table => new
                {
                    PatrolRouteId = table.Column<long>(type: "bigint(20)", nullable: false),
                    EnforcementAuthorityId = table.Column<long>(type: "bigint(20)", nullable: false),
                    NumberRequired = table.Column<int>(type: "int(11)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => new { x.PatrolRouteId, x.EnforcementAuthorityId });
                    table.ForeignKey(
                        name: "FK_PatrolRoutesNumbers_EnforcementAuthorities",
                        column: x => x.EnforcementAuthorityId,
                        principalTable: "EnforcementAuthorities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PatrolRoutesNumbers_PatrolRoutes",
                        column: x => x.PatrolRouteId,
                        principalTable: "PatrolRoutes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PatrolRoutesTimesOfDay",
                columns: table => new
                {
                    PatrolRouteId = table.Column<long>(type: "bigint(20)", nullable: false),
                    TimeOfDay = table.Column<int>(type: "int(11)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => new { x.PatrolRouteId, x.TimeOfDay });
                    table.ForeignKey(
                        name: "FK_PatrolRoutesTimesOfDay_PatrolRoutes",
                        column: x => x.PatrolRouteId,
                        principalTable: "PatrolRoutes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "FK_EnforcementAuthorities_FutureProgs_idx",
                table: "EnforcementAuthorities",
                column: "FilterProgId");

            migrationBuilder.CreateIndex(
                name: "FK_PatrolRoutes_LegalAuthorities_idx",
                table: "PatrolRoutes",
                column: "LegalAuthorityId");

            migrationBuilder.CreateIndex(
                name: "FK_PatrolRoutesNodes_Cells_idx",
                table: "PatrolRoutesNodes",
                column: "CellId");

            migrationBuilder.CreateIndex(
                name: "FK_PatrolRoutesNodes_PatrolRoutes_idx",
                table: "PatrolRoutesNodes",
                column: "PatrolRouteId");

            migrationBuilder.CreateIndex(
                name: "FK_PatrolRoutesNumbers_EnforcementAuthorities_idx",
                table: "PatrolRoutesNumbers",
                column: "EnforcementAuthorityId");

            migrationBuilder.CreateIndex(
                name: "FK_PatrolRoutesNumbers_PatrolRoutes_idx",
                table: "PatrolRoutesNumbers",
                column: "PatrolRouteId");

            migrationBuilder.CreateIndex(
                name: "FK_PatrolRoutesTimesOfDay_PatrolRoutes_idx",
                table: "PatrolRoutesTimesOfDay",
                column: "PatrolRouteId");

            migrationBuilder.AddForeignKey(
                name: "FK_EnforcementAuthorities_FutureProgs",
                table: "EnforcementAuthorities",
                column: "FilterProgId",
                principalTable: "FutureProgs",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EnforcementAuthorities_FutureProgs",
                table: "EnforcementAuthorities");

            migrationBuilder.DropTable(
                name: "PatrolRoutesNodes");

            migrationBuilder.DropTable(
                name: "PatrolRoutesNumbers");

            migrationBuilder.DropTable(
                name: "PatrolRoutesTimesOfDay");

            migrationBuilder.DropTable(
                name: "PatrolRoutes");

            migrationBuilder.DropIndex(
                name: "FK_EnforcementAuthorities_FutureProgs_idx",
                table: "EnforcementAuthorities");

            migrationBuilder.DropColumn(
                name: "FilterProgId",
                table: "EnforcementAuthorities");
        }
    }
}
