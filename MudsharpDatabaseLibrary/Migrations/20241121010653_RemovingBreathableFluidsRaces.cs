using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MudSharp.Migrations
{
    /// <inheritdoc />
    public partial class RemovingBreathableFluidsRaces : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Races_RemoveBreathableGases",
                columns: table => new
                {
                    RaceId = table.Column<long>(type: "bigint(20)", nullable: false),
                    GasId = table.Column<long>(type: "bigint(20)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => new { x.RaceId, x.GasId });
                    table.ForeignKey(
                        name: "FK_Races_RemoveBreathableGases_Gases",
                        column: x => x.GasId,
                        principalTable: "Gases",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Races_RemoveBreathableGases_Races",
                        column: x => x.RaceId,
                        principalTable: "Races",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Races_RemoveBreathableLiquids",
                columns: table => new
                {
                    RaceId = table.Column<long>(type: "bigint(20)", nullable: false),
                    LiquidId = table.Column<long>(type: "bigint(20)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => new { x.RaceId, x.LiquidId });
                    table.ForeignKey(
                        name: "FK_Races_RemoveBreathableLiquids_Liquids",
                        column: x => x.LiquidId,
                        principalTable: "Liquids",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Races_RemoveBreathableLiquids_Races",
                        column: x => x.RaceId,
                        principalTable: "Races",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "FK_Races_RemoveBreathableGases_Gases_idx",
                table: "Races_RemoveBreathableGases",
                column: "GasId");

            migrationBuilder.CreateIndex(
                name: "FK_Races_RemoveBreathableLiquids_Liquids_idx",
                table: "Races_RemoveBreathableLiquids",
                column: "LiquidId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Races_RemoveBreathableGases");

            migrationBuilder.DropTable(
                name: "Races_RemoveBreathableLiquids");
        }
    }
}
