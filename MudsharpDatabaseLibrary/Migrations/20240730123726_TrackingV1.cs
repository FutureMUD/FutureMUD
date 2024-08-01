using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MudSharp.Migrations
{
    /// <inheritdoc />
    public partial class TrackingV1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<ulong>(
                name: "CanHaveTracks",
                table: "Terrains",
                type: "bit(1)",
                nullable: false,
                defaultValue: 1ul);

            migrationBuilder.AddColumn<double>(
                name: "TrackIntensityMultiplierOlfactory",
                table: "Terrains",
                type: "double",
                nullable: false,
                defaultValue: 1.0);

            migrationBuilder.AddColumn<double>(
                name: "TrackIntensityMultiplierVisual",
                table: "Terrains",
                type: "double",
                nullable: false,
                defaultValue: 1.0);

            migrationBuilder.AddColumn<double>(
                name: "TrackIntensityOlfactory",
                table: "Races",
                type: "double",
                nullable: false,
                defaultValue: 1.0);

            migrationBuilder.AddColumn<double>(
                name: "TrackIntensityVisual",
                table: "Races",
                type: "double",
                nullable: false,
                defaultValue: 1.0);

            migrationBuilder.AddColumn<double>(
                name: "TrackingAbilityOlfactory",
                table: "Races",
                type: "double",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "TrackingAbilityVisual",
                table: "Races",
                type: "double",
                nullable: false,
                defaultValue: 1.0);

            migrationBuilder.CreateTable(
                name: "Tracks",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint(20)", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    CharacterId = table.Column<long>(type: "bigint(20)", nullable: false),
                    BodyPrototypeId = table.Column<long>(type: "bigint(20)", nullable: false),
                    CellId = table.Column<long>(type: "bigint(20)", nullable: false),
                    RoomLayer = table.Column<int>(type: "int(11)", nullable: false),
                    MudDateTime = table.Column<string>(type: "varchar(100)", nullable: true, collation: "utf8_general_ci")
                        .Annotation("MySql:CharSet", "utf8"),
                    FromDirectionExitId = table.Column<long>(type: "bigint(20)", nullable: true),
                    ToDirectionExitId = table.Column<long>(type: "bigint(20)", nullable: true),
                    TrackCircumstances = table.Column<int>(type: "int(11)", nullable: false),
                    FromMoveSpeedId = table.Column<long>(type: "bigint(20)", nullable: true),
                    ToMoveSpeedId = table.Column<long>(type: "bigint(20)", nullable: true),
                    ExertionLevel = table.Column<int>(type: "int(11)", nullable: false),
                    TrackIntensityVisual = table.Column<double>(type: "double", nullable: false),
                    TrackIntensityOlfactory = table.Column<double>(type: "double", nullable: false),
                    TurnedAround = table.Column<ulong>(type: "bit(1)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Tracks_BodyProtos",
                        column: x => x.BodyPrototypeId,
                        principalTable: "BodyProtos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Tracks_Cells",
                        column: x => x.CellId,
                        principalTable: "Cells",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Tracks_Characters",
                        column: x => x.CharacterId,
                        principalTable: "Characters",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Tracks_Exits_From",
                        column: x => x.FromDirectionExitId,
                        principalTable: "Exits",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Tracks_Exits_To",
                        column: x => x.ToDirectionExitId,
                        principalTable: "Exits",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Tracks_MoveSpeeds_From",
                        column: x => x.FromMoveSpeedId,
                        principalTable: "MoveSpeeds",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Tracks_MoveSpeeds_To",
                        column: x => x.ToMoveSpeedId,
                        principalTable: "MoveSpeeds",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_Tracks_BodyPrototypeId",
                table: "Tracks",
                column: "BodyPrototypeId");

            migrationBuilder.CreateIndex(
                name: "IX_Tracks_CellId",
                table: "Tracks",
                column: "CellId");

            migrationBuilder.CreateIndex(
                name: "IX_Tracks_CharacterId",
                table: "Tracks",
                column: "CharacterId");

            migrationBuilder.CreateIndex(
                name: "IX_Tracks_FromDirectionExitId",
                table: "Tracks",
                column: "FromDirectionExitId");

            migrationBuilder.CreateIndex(
                name: "IX_Tracks_FromMoveSpeedId",
                table: "Tracks",
                column: "FromMoveSpeedId");

            migrationBuilder.CreateIndex(
                name: "IX_Tracks_ToDirectionExitId",
                table: "Tracks",
                column: "ToDirectionExitId");

            migrationBuilder.CreateIndex(
                name: "IX_Tracks_ToMoveSpeedId",
                table: "Tracks",
                column: "ToMoveSpeedId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Tracks");

            migrationBuilder.DropColumn(
                name: "CanHaveTracks",
                table: "Terrains");

            migrationBuilder.DropColumn(
                name: "TrackIntensityMultiplierOlfactory",
                table: "Terrains");

            migrationBuilder.DropColumn(
                name: "TrackIntensityMultiplierVisual",
                table: "Terrains");

            migrationBuilder.DropColumn(
                name: "TrackIntensityOlfactory",
                table: "Races");

            migrationBuilder.DropColumn(
                name: "TrackIntensityVisual",
                table: "Races");

            migrationBuilder.DropColumn(
                name: "TrackingAbilityOlfactory",
                table: "Races");

            migrationBuilder.DropColumn(
                name: "TrackingAbilityVisual",
                table: "Races");
        }
    }
}
