using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MudSharp.Migrations
{
    /// <inheritdoc />
    public partial class RouteCellSpatialFoundation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "CurrentRoutePosition",
                table: "Vehicles",
                type: "decimal(18,3)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "RouteDirection",
                table: "Tracks",
                type: "int(11)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "RoutePosition",
                table: "Tracks",
                type: "decimal(18,3)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "RoutePosition",
                table: "GameItems",
                type: "decimal(18,3)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "RoutePosition",
                table: "Characters",
                type: "decimal(18,3)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "RoutePosition",
                table: "CharacterInstances",
                type: "decimal(18,3)",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "RouteCells",
                columns: table => new
                {
                    CellId = table.Column<long>(type: "bigint(20)", nullable: false),
                    LengthMetres = table.Column<decimal>(type: "decimal(18,3)", nullable: false),
                    DefaultPositionMetres = table.Column<decimal>(type: "decimal(18,3)", nullable: false),
                    PositiveDirectionName = table.Column<string>(type: "varchar(100)", nullable: false, collation: "utf8mb4_unicode_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    NegativeDirectionName = table.Column<string>(type: "varchar(100)", nullable: false, collation: "utf8mb4_unicode_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    MetresPerRoomEquivalent = table.Column<decimal>(type: "decimal(18,3)", nullable: false),
                    TopologyVersion = table.Column<long>(type: "bigint(20)", nullable: false, defaultValue: 1L)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.CellId);
                    table.CheckConstraint("CK_RouteCells_DefaultPosition", "`DefaultPositionMetres` >= 0 AND `DefaultPositionMetres` <= `LengthMetres`");
                    table.CheckConstraint("CK_RouteCells_Length", "`LengthMetres` > 0");
                    table.CheckConstraint("CK_RouteCells_RoomEquivalent", "`MetresPerRoomEquivalent` > 0");
                    table.CheckConstraint("CK_RouteCells_TopologyVersion", "`TopologyVersion` >= 1");
                    table.ForeignKey(
                        name: "FK_RouteCells_Cells",
                        column: x => x.CellId,
                        principalTable: "Cells",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "ActiveRouteMotions",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint(20)", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    MoverType = table.Column<int>(type: "int(11)", nullable: false),
                    MoverId = table.Column<long>(type: "bigint(20)", nullable: false),
                    RouteCellId = table.Column<long>(type: "bigint(20)", nullable: false),
                    RoomLayer = table.Column<int>(type: "int(11)", nullable: false),
                    CheckpointPositionMetres = table.Column<decimal>(type: "decimal(18,3)", nullable: false),
                    TargetMinimumPositionMetres = table.Column<decimal>(type: "decimal(18,3)", nullable: false),
                    TargetMaximumPositionMetres = table.Column<decimal>(type: "decimal(18,3)", nullable: false),
                    Direction = table.Column<int>(type: "int(11)", nullable: false),
                    SpeedMetresPerSecond = table.Column<decimal>(type: "decimal(18,6)", nullable: false),
                    RemainingDurationMilliseconds = table.Column<long>(type: "bigint(20)", nullable: false),
                    TopologyVersion = table.Column<long>(type: "bigint(20)", nullable: false),
                    Status = table.Column<int>(type: "int(11)", nullable: false),
                    OperationId = table.Column<string>(type: "varchar(64)", nullable: false, collation: "ascii_general_ci")
                        .Annotation("MySql:CharSet", "ascii"),
                    CheckpointSequence = table.Column<long>(type: "bigint(20)", nullable: false),
                    SelectedExitId = table.Column<long>(type: "bigint(20)", nullable: true),
                    StateData = table.Column<string>(type: "mediumtext", nullable: true, collation: "utf8mb4_unicode_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    CreatedDateTime = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    LastCheckpointDateTime = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.Id);
                    table.CheckConstraint("CK_ActiveRouteMotions_Checkpoint", "`CheckpointPositionMetres` >= 0");
                    table.CheckConstraint("CK_ActiveRouteMotions_Direction", "`Direction` IN (-1, 1)");
                    table.CheckConstraint("CK_ActiveRouteMotions_RemainingDuration", "`RemainingDurationMilliseconds` >= 0");
                    table.CheckConstraint("CK_ActiveRouteMotions_Sequence", "`CheckpointSequence` >= 0");
                    table.CheckConstraint("CK_ActiveRouteMotions_Speed", "`SpeedMetresPerSecond` > 0");
                    table.CheckConstraint("CK_ActiveRouteMotions_TargetBand", "`TargetMinimumPositionMetres` >= 0 AND `TargetMaximumPositionMetres` >= `TargetMinimumPositionMetres`");
                    table.CheckConstraint("CK_ActiveRouteMotions_TopologyVersion", "`TopologyVersion` >= 1");
                    table.ForeignKey(
                        name: "FK_ActiveRouteMotions_Exits",
                        column: x => x.SelectedExitId,
                        principalTable: "Exits",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_ActiveRouteMotions_RouteCells",
                        column: x => x.RouteCellId,
                        principalTable: "RouteCells",
                        principalColumn: "CellId",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "RouteCellLandmarks",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint(20)", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    RouteCellId = table.Column<long>(type: "bigint(20)", nullable: false),
                    Name = table.Column<string>(type: "varchar(200)", nullable: false, collation: "utf8mb4_unicode_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Keywords = table.Column<string>(type: "varchar(500)", nullable: false, collation: "utf8mb4_unicode_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Description = table.Column<string>(type: "text", nullable: false, collation: "utf8mb4_unicode_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    PositionMetres = table.Column<decimal>(type: "decimal(18,3)", nullable: false),
                    DisplayOrder = table.Column<int>(type: "int(11)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.Id);
                    table.CheckConstraint("CK_RouteCellLandmarks_Position", "`PositionMetres` >= 0");
                    table.ForeignKey(
                        name: "FK_RouteCellLandmarks_RouteCells",
                        column: x => x.RouteCellId,
                        principalTable: "RouteCells",
                        principalColumn: "CellId",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "RouteExitAnchors",
                columns: table => new
                {
                    ExitId = table.Column<long>(type: "bigint(20)", nullable: false),
                    RouteCellId = table.Column<long>(type: "bigint(20)", nullable: false),
                    MinimumPositionMetres = table.Column<decimal>(type: "decimal(18,3)", nullable: false),
                    MaximumPositionMetres = table.Column<decimal>(type: "decimal(18,3)", nullable: false),
                    ArrivalPositionMetres = table.Column<decimal>(type: "decimal(18,3)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => new { x.ExitId, x.RouteCellId });
                    table.CheckConstraint("CK_RouteExitAnchors_Arrival", "`ArrivalPositionMetres` >= `MinimumPositionMetres` AND `ArrivalPositionMetres` <= `MaximumPositionMetres`");
                    table.CheckConstraint("CK_RouteExitAnchors_Band", "`MinimumPositionMetres` >= 0 AND `MaximumPositionMetres` >= `MinimumPositionMetres`");
                    table.ForeignKey(
                        name: "FK_RouteExitAnchors_Exits",
                        column: x => x.ExitId,
                        principalTable: "Exits",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RouteExitAnchors_RouteCells",
                        column: x => x.RouteCellId,
                        principalTable: "RouteCells",
                        principalColumn: "CellId",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "RouteMotionResourceLedgers",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint(20)", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    ActiveRouteMotionId = table.Column<long>(type: "bigint(20)", nullable: false),
                    CheckpointSequence = table.Column<long>(type: "bigint(20)", nullable: false),
                    IdempotencyKey = table.Column<string>(type: "varchar(200)", nullable: false, collation: "ascii_general_ci")
                        .Annotation("MySql:CharSet", "ascii"),
                    ResourceOwnerType = table.Column<int>(type: "int(11)", nullable: false),
                    ResourceOwnerId = table.Column<long>(type: "bigint(20)", nullable: false),
                    ResourceType = table.Column<int>(type: "int(11)", nullable: false),
                    ResourceReferenceId = table.Column<long>(type: "bigint(20)", nullable: true),
                    ResourceKey = table.Column<string>(type: "varchar(200)", nullable: false, collation: "utf8mb4_unicode_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ReservedAmount = table.Column<decimal>(type: "decimal(18,6)", nullable: false),
                    ConsumedAmount = table.Column<decimal>(type: "decimal(18,6)", nullable: false),
                    Status = table.Column<int>(type: "int(11)", nullable: false),
                    CreatedDateTime = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    CommittedDateTime = table.Column<DateTime>(type: "datetime(6)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.Id);
                    table.CheckConstraint("CK_RouteMotionResourceLedgers_Amounts", "`ReservedAmount` >= 0 AND `ConsumedAmount` >= 0 AND `ConsumedAmount` <= `ReservedAmount`");
                    table.CheckConstraint("CK_RouteMotionResourceLedgers_Sequence", "`CheckpointSequence` >= 0");
                    table.ForeignKey(
                        name: "FK_RouteMotionResourceLedgers_ActiveRouteMotions",
                        column: x => x.ActiveRouteMotionId,
                        principalTable: "ActiveRouteMotions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_Vehicles_Cell_Layer_RoutePosition",
                table: "Vehicles",
                columns: new[] { "CurrentCellId", "CurrentRoomLayer", "CurrentRoutePosition" });

            migrationBuilder.CreateIndex(
                name: "IX_Tracks_Cell_Layer_RoutePosition",
                table: "Tracks",
                columns: new[] { "CellId", "RoomLayer", "RoutePosition" });

            // MySQL requires an index whose leading column is CellId while FK_Tracks_Cells
            // exists. Create the replacement composite index before removing the old one.
            migrationBuilder.DropIndex(
                name: "IX_Tracks_CellId",
                table: "Tracks");

            migrationBuilder.CreateIndex(
                name: "IX_GameItems_RoutePosition",
                table: "GameItems",
                column: "RoutePosition");

            migrationBuilder.CreateIndex(
                name: "IX_Characters_Location_Layer_RoutePosition",
                table: "Characters",
                columns: new[] { "Location", "RoomLayer", "RoutePosition" });

            migrationBuilder.CreateIndex(
                name: "IX_CharacterInstances_Location_Layer_RoutePosition",
                table: "CharacterInstances",
                columns: new[] { "LocationId", "RoomLayer", "RoutePosition" });

            migrationBuilder.CreateIndex(
                name: "FK_ActiveRouteMotions_Exits_idx",
                table: "ActiveRouteMotions",
                column: "SelectedExitId");

            migrationBuilder.CreateIndex(
                name: "IX_ActiveRouteMotions_RouteCell_Layer_Status",
                table: "ActiveRouteMotions",
                columns: new[] { "RouteCellId", "RoomLayer", "Status" });

            migrationBuilder.CreateIndex(
                name: "UX_ActiveRouteMotions_Mover",
                table: "ActiveRouteMotions",
                columns: new[] { "MoverType", "MoverId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "UX_ActiveRouteMotions_Operation",
                table: "ActiveRouteMotions",
                column: "OperationId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RouteCellLandmarks_RouteCell_Position",
                table: "RouteCellLandmarks",
                columns: new[] { "RouteCellId", "PositionMetres" });

            migrationBuilder.CreateIndex(
                name: "IX_RouteExitAnchors_RouteCell_Band",
                table: "RouteExitAnchors",
                columns: new[] { "RouteCellId", "MinimumPositionMetres", "MaximumPositionMetres" });

            migrationBuilder.CreateIndex(
                name: "IX_RouteMotionResourceLedgers_Motion_Sequence",
                table: "RouteMotionResourceLedgers",
                columns: new[] { "ActiveRouteMotionId", "CheckpointSequence" });

            migrationBuilder.CreateIndex(
                name: "UX_RouteMotionResourceLedgers_Idempotency",
                table: "RouteMotionResourceLedgers",
                column: "IdempotencyKey",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RouteCellLandmarks");

            migrationBuilder.DropTable(
                name: "RouteExitAnchors");

            migrationBuilder.DropTable(
                name: "RouteMotionResourceLedgers");

            migrationBuilder.DropTable(
                name: "ActiveRouteMotions");

            migrationBuilder.DropTable(
                name: "RouteCells");

            migrationBuilder.DropIndex(
                name: "IX_Vehicles_Cell_Layer_RoutePosition",
                table: "Vehicles");

            // Restore the legacy FK-supporting index before removing its composite replacement.
            migrationBuilder.CreateIndex(
                name: "IX_Tracks_CellId",
                table: "Tracks",
                column: "CellId");

            migrationBuilder.DropIndex(
                name: "IX_Tracks_Cell_Layer_RoutePosition",
                table: "Tracks");

            migrationBuilder.DropIndex(
                name: "IX_GameItems_RoutePosition",
                table: "GameItems");

            migrationBuilder.DropIndex(
                name: "IX_Characters_Location_Layer_RoutePosition",
                table: "Characters");

            migrationBuilder.DropIndex(
                name: "IX_CharacterInstances_Location_Layer_RoutePosition",
                table: "CharacterInstances");

            migrationBuilder.DropColumn(
                name: "CurrentRoutePosition",
                table: "Vehicles");

            migrationBuilder.DropColumn(
                name: "RouteDirection",
                table: "Tracks");

            migrationBuilder.DropColumn(
                name: "RoutePosition",
                table: "Tracks");

            migrationBuilder.DropColumn(
                name: "RoutePosition",
                table: "GameItems");

            migrationBuilder.DropColumn(
                name: "RoutePosition",
                table: "Characters");

            migrationBuilder.DropColumn(
                name: "RoutePosition",
                table: "CharacterInstances");

        }
    }
}
