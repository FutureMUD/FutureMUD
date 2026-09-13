using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MudSharp.Migrations
{
    /// <inheritdoc />
    public partial class EnvironmentalMagic : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "EnvironmentalMagicProfileId",
                table: "Terrains",
                type: "bigint(20)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "EnvironmentalMagicBindingMode",
                table: "Cells",
                type: "int(11)",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<long>(
                name: "EnvironmentalMagicProfileId",
                table: "Cells",
                type: "bigint(20)",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "CellEnvironmentalStates",
                columns: table => new
                {
                    CellId = table.Column<long>(type: "bigint(20)", nullable: false),
                    SchemaVersion = table.Column<int>(type: "int(11)", nullable: false, defaultValue: 1),
                    Revision = table.Column<long>(type: "bigint(20)", nullable: false, defaultValue: 0L),
                    ScarDamage = table.Column<double>(type: "double", nullable: false, defaultValue: 0.0),
                    LastDefileUtc = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    RecentPressure = table.Column<double>(type: "double", nullable: false, defaultValue: 0.0),
                    PressureReferenceUtc = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    PressureHalfLifeSeconds = table.Column<double>(type: "double", nullable: false, defaultValue: 3600.0),
                    PressureProfileId = table.Column<long>(type: "bigint(20)", nullable: true),
                    PressureDecayAnchor = table.Column<double>(type: "double", nullable: false, defaultValue: 0.0)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.CellId);
                    table.CheckConstraint("CK_CellEnvironmentalStates_Pressure", "`RecentPressure` >= 0 AND `PressureHalfLifeSeconds` > 0");
                    table.CheckConstraint("CK_CellEnvironmentalStates_ScarDamage", "`ScarDamage` >= 0");
                    table.CheckConstraint("CK_CellEnvironmentalStates_Versions", "`SchemaVersion` >= 1 AND `Revision` >= 0");
                    table.ForeignKey(
                        name: "FK_CellEnvironmentalStates_Cells",
                        column: x => x.CellId,
                        principalTable: "Cells",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "EnvironmentalMagicOperations",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    CellId = table.Column<long>(type: "bigint(20)", nullable: false),
                    Kind = table.Column<string>(type: "varchar(40)", maxLength: 40, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    RequestedDamage = table.Column<double>(type: "double", nullable: false),
                    RequestedPressure = table.Column<double>(type: "double", nullable: false),
                    RequestedRepair = table.Column<double>(type: "double", nullable: false),
                    AppliedDamage = table.Column<double>(type: "double", nullable: false),
                    AppliedPressure = table.Column<double>(type: "double", nullable: false),
                    AppliedRepair = table.Column<double>(type: "double", nullable: false),
                    AtUtc = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    ActorId = table.Column<long>(type: "bigint(20)", nullable: true),
                    Attribution = table.Column<string>(type: "text", nullable: false, collation: "utf8mb4_unicode_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Status = table.Column<string>(type: "varchar(40)", maxLength: 40, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Diagnostic = table.Column<string>(type: "text", nullable: false, collation: "utf8mb4_unicode_ci")
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_Terrains_EnvironmentalMagicProfileId",
                table: "Terrains",
                column: "EnvironmentalMagicProfileId");

            migrationBuilder.CreateIndex(
                name: "IX_Cells_EnvironmentalMagicProfileId",
                table: "Cells",
                column: "EnvironmentalMagicProfileId");

            migrationBuilder.CreateIndex(
                name: "IX_EnvironmentalMagicOperations_CellId_AtUtc",
                table: "EnvironmentalMagicOperations",
                columns: new[] { "CellId", "AtUtc" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CellEnvironmentalStates");

            migrationBuilder.DropTable(
                name: "EnvironmentalMagicOperations");

            migrationBuilder.DropIndex(
                name: "IX_Terrains_EnvironmentalMagicProfileId",
                table: "Terrains");

            migrationBuilder.DropIndex(
                name: "IX_Cells_EnvironmentalMagicProfileId",
                table: "Cells");

            migrationBuilder.DropColumn(
                name: "EnvironmentalMagicProfileId",
                table: "Terrains");

            migrationBuilder.DropColumn(
                name: "EnvironmentalMagicBindingMode",
                table: "Cells");

            migrationBuilder.DropColumn(
                name: "EnvironmentalMagicProfileId",
                table: "Cells");
        }
    }
}
