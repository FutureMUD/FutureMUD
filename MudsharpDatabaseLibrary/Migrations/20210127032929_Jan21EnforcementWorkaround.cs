using Microsoft.EntityFrameworkCore.Migrations;

namespace MudSharp.Migrations
{
    public partial class Jan21EnforcementWorkaround : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EnforcementAuthorities_ArrestableClasses");

            migrationBuilder.AddColumn<ulong>(
                name: "IsReady",
                table: "PatrolRoutes",
                type: "bit(1)",
                nullable: false,
                defaultValueSql: "b'0'");

            migrationBuilder.AddColumn<long>(
                name: "OnImprisonProgId",
                table: "LegalAuthorities",
                type: "bigint(20)",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "OnReleaseProgId",
                table: "LegalAuthorities",
                type: "bigint(20)",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "PrisonBelongingsLocationId",
                table: "LegalAuthorities",
                type: "bigint(20)",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "PrisonReleaseLocationId",
                table: "LegalAuthorities",
                type: "bigint(20)",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "EnforcementAuthoritiesArrestableClasses",
                columns: table => new
                {
                    EnforcementAuthorityId = table.Column<long>(type: "bigint(20)", nullable: false),
                    LegalClassId = table.Column<long>(type: "bigint(20)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EnforcementAuthoritiesArrestableClasses", x => new { x.EnforcementAuthorityId, x.LegalClassId });
                    table.ForeignKey(
                        name: "FK_EnforcementAuthoritiesArrestableLegalClasses_Enforce",
                        column: x => x.EnforcementAuthorityId,
                        principalTable: "EnforcementAuthorities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EnforcementAuthoritiesArrestableLegalClasses_LegalClasses",
                        column: x => x.LegalClassId,
                        principalTable: "LegalClasses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "FK_LegalAuthorities_FutureprogsImprison_idx",
                table: "LegalAuthorities",
                column: "OnImprisonProgId");

            migrationBuilder.CreateIndex(
                name: "FK_LegalAuthorities_FutureprogsRelease_idx",
                table: "LegalAuthorities",
                column: "OnReleaseProgId");

            migrationBuilder.CreateIndex(
                name: "FK_LegalAuthorities_PrisonBelongingsCells_idx",
                table: "LegalAuthorities",
                column: "PrisonBelongingsLocationId");

            migrationBuilder.CreateIndex(
                name: "FK_LegalAuthorities_PrisonReleaseCells_idx",
                table: "LegalAuthorities",
                column: "PrisonReleaseLocationId");

            migrationBuilder.CreateIndex(
                name: "FK_EnforcementAuthoritiesArrestableLegalClasses_Enforce_idx",
                table: "EnforcementAuthoritiesArrestableClasses",
                column: "EnforcementAuthorityId");

            migrationBuilder.CreateIndex(
                name: "FK_EnforcementAuthoritiesArrestableLegalClasses_LegalClasses_idx",
                table: "EnforcementAuthoritiesArrestableClasses",
                column: "LegalClassId");

            migrationBuilder.AddForeignKey(
                name: "FK_LegalAuthorities_FutureprogsImprison",
                table: "LegalAuthorities",
                column: "OnImprisonProgId",
                principalTable: "FutureProgs",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_LegalAuthorities_FutureprogsRelease",
                table: "LegalAuthorities",
                column: "OnReleaseProgId",
                principalTable: "FutureProgs",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_LegalAuthorities_PrisonBelongingsCells",
                table: "LegalAuthorities",
                column: "PrisonBelongingsLocationId",
                principalTable: "Cells",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_LegalAuthorities_PrisonReleaseCells",
                table: "LegalAuthorities",
                column: "PrisonReleaseLocationId",
                principalTable: "Cells",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LegalAuthorities_FutureprogsImprison",
                table: "LegalAuthorities");

            migrationBuilder.DropForeignKey(
                name: "FK_LegalAuthorities_FutureprogsRelease",
                table: "LegalAuthorities");

            migrationBuilder.DropForeignKey(
                name: "FK_LegalAuthorities_PrisonBelongingsCells",
                table: "LegalAuthorities");

            migrationBuilder.DropForeignKey(
                name: "FK_LegalAuthorities_PrisonReleaseCells",
                table: "LegalAuthorities");

            migrationBuilder.DropTable(
                name: "EnforcementAuthoritiesArrestableClasses");

            migrationBuilder.DropIndex(
                name: "FK_LegalAuthorities_FutureprogsImprison_idx",
                table: "LegalAuthorities");

            migrationBuilder.DropIndex(
                name: "FK_LegalAuthorities_FutureprogsRelease_idx",
                table: "LegalAuthorities");

            migrationBuilder.DropIndex(
                name: "FK_LegalAuthorities_PrisonBelongingsCells_idx",
                table: "LegalAuthorities");

            migrationBuilder.DropIndex(
                name: "FK_LegalAuthorities_PrisonReleaseCells_idx",
                table: "LegalAuthorities");

            migrationBuilder.DropColumn(
                name: "IsReady",
                table: "PatrolRoutes");

            migrationBuilder.DropColumn(
                name: "OnImprisonProgId",
                table: "LegalAuthorities");

            migrationBuilder.DropColumn(
                name: "OnReleaseProgId",
                table: "LegalAuthorities");

            migrationBuilder.DropColumn(
                name: "PrisonBelongingsLocationId",
                table: "LegalAuthorities");

            migrationBuilder.DropColumn(
                name: "PrisonReleaseLocationId",
                table: "LegalAuthorities");

            migrationBuilder.CreateTable(
                name: "EnforcementAuthorities_ArrestableClasses",
                columns: table => new
                {
                    EnforcementAuthorityId = table.Column<long>(type: "bigint(20)", nullable: false),
                    LegalClassId = table.Column<long>(type: "bigint(20)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.EnforcementAuthorityId);
                    table.ForeignKey(
                        name: "FK_EnforcementAuthorities_ArrestableClasses_Enforce",
                        column: x => x.EnforcementAuthorityId,
                        principalTable: "EnforcementAuthorities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EnforcementAuthorities_ArrestableClasses_LegalClasses",
                        column: x => x.LegalClassId,
                        principalTable: "LegalClasses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "FK_EnforcementAuthorities_ArrestableClasses_LegalClasses_idx",
                table: "EnforcementAuthorities_ArrestableClasses",
                column: "LegalClassId");
        }
    }
}
