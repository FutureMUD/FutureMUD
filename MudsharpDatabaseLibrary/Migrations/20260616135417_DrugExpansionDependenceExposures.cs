using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MudSharp.Migrations
{
    /// <inheritdoc />
    public partial class DrugExpansionDependenceExposures : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Bodies_DrugExposures",
                columns: table => new
                {
                    BodyId = table.Column<long>(type: "bigint(20)", nullable: false),
                    DrugId = table.Column<long>(type: "bigint(20)", nullable: false),
                    Exposure = table.Column<double>(type: "double", nullable: false),
                    PeakExposure = table.Column<double>(type: "double", nullable: false),
                    WithdrawalIntensity = table.Column<double>(type: "double", nullable: false),
                    LastUpdatedAtUtc = table.Column<DateTime>(type: "datetime", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => new { x.BodyId, x.DrugId });
                    table.ForeignKey(
                        name: "FK_Bodies_DrugExposures_Bodies",
                        column: x => x.BodyId,
                        principalTable: "Bodies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Bodies_DrugExposures_Drugs",
                        column: x => x.DrugId,
                        principalTable: "Drugs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "FK_Bodies_DrugExposures_Drugs_idx",
                table: "Bodies_DrugExposures",
                column: "DrugId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Bodies_DrugExposures");
        }
    }
}
