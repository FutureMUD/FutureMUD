using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MudSharp.Migrations
{
    /// <inheritdoc />
    public partial class ClimateModelSimplification : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Definition",
                table: "ClimateModels");

            migrationBuilder.CreateTable(
                name: "ClimateModelSeason",
                columns: table => new
                {
                    ClimateModelId = table.Column<long>(type: "bigint(20)", nullable: false),
                    SeasonId = table.Column<long>(type: "bigint(20)", nullable: false),
                    MaximumAdditionalChangeChanceFromStableWeather = table.Column<double>(type: "double", nullable: false),
                    IncrementalAdditionalChangeChanceFromStableWeather = table.Column<double>(type: "double", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => new { x.ClimateModelId, x.SeasonId });
                    table.ForeignKey(
                        name: "FK_ClimateModelSeasons_ClimateModels",
                        column: x => x.ClimateModelId,
                        principalTable: "ClimateModels",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ClimateModelSeasons_Seasons",
                        column: x => x.SeasonId,
                        principalTable: "Seasons",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "ClimateModelSeasonEvent",
                columns: table => new
                {
                    ClimateModelId = table.Column<long>(type: "bigint(20)", nullable: false),
                    SeasonId = table.Column<long>(type: "bigint(20)", nullable: false),
                    WeatherEventId = table.Column<long>(type: "bigint(20)", nullable: false),
                    ChangeChance = table.Column<double>(type: "double", nullable: false),
                    Transitions = table.Column<string>(type: "mediumtext", nullable: false, collation: "utf8_general_ci")
                        .Annotation("MySql:CharSet", "utf8")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => new { x.ClimateModelId, x.SeasonId, x.WeatherEventId });
                    table.ForeignKey(
                        name: "FK_ClimateModelSeasonEvent_ClimateModelSeason_ClimateModelId_Se~",
                        columns: x => new { x.ClimateModelId, x.SeasonId },
                        principalTable: "ClimateModelSeason",
                        principalColumns: new[] { "ClimateModelId", "SeasonId" },
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ClimateModelSeasonEvents_ClimateModels",
                        column: x => x.ClimateModelId,
                        principalTable: "ClimateModels",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ClimateModelSeasonEvents_Seasons",
                        column: x => x.SeasonId,
                        principalTable: "Seasons",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ClimateModelSeasonEvents_WeatherEvents",
                        column: x => x.WeatherEventId,
                        principalTable: "WeatherEvents",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_ClimateModelSeason_SeasonId",
                table: "ClimateModelSeason",
                column: "SeasonId");

            migrationBuilder.CreateIndex(
                name: "IX_ClimateModelSeasonEvent_SeasonId",
                table: "ClimateModelSeasonEvent",
                column: "SeasonId");

            migrationBuilder.CreateIndex(
                name: "IX_ClimateModelSeasonEvent_WeatherEventId",
                table: "ClimateModelSeasonEvent",
                column: "WeatherEventId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ClimateModelSeasonEvent");

            migrationBuilder.DropTable(
                name: "ClimateModelSeason");

            migrationBuilder.AddColumn<string>(
                name: "Definition",
                table: "ClimateModels",
                type: "mediumtext",
                nullable: false,
                collation: "utf8_general_ci")
                .Annotation("MySql:CharSet", "utf8");
        }
    }
}
