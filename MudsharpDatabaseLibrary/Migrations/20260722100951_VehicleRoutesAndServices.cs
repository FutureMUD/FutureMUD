using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MudSharp.Migrations
{
    /// <inheritdoc />
    public partial class VehicleRoutesAndServices : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<ulong>(
                name: "AutomaticOperationCapable",
                table: "VehicleMovementProfileProtos",
                type: "bit(1)",
                nullable: false,
                defaultValue: 0ul);

            migrationBuilder.AddColumn<double>(
                name: "RouteFuelVolumePerMetre",
                table: "VehicleMovementProfileProtos",
                type: "double",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "RoutePowerDrawWatts",
                table: "VehicleMovementProfileProtos",
                type: "double",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<int>(
                name: "RoutePropulsionMode",
                table: "VehicleMovementProfileProtos",
                type: "int(11)",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<double>(
                name: "RouteSpeedMetresPerSecond",
                table: "VehicleMovementProfileProtos",
                type: "double",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<long>(
                name: "VehicleRouteStopId",
                table: "VehicleDockings",
                type: "bigint(20)",
                nullable: true);

            migrationBuilder.AlterColumn<long>(
                name: "CharacterId",
                table: "Tracks",
                type: "bigint(20)",
                nullable: true,
                oldClrType: typeof(long),
                oldType: "bigint(20)");

            migrationBuilder.AlterColumn<long>(
                name: "BodyPrototypeId",
                table: "Tracks",
                type: "bigint(20)",
                nullable: true,
                oldClrType: typeof(long),
                oldType: "bigint(20)");

            migrationBuilder.AddColumn<long>(
                name: "VehicleId",
                table: "Tracks",
                type: "bigint(20)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "RoomLayer",
                table: "ActiveProjects",
                type: "int(11)",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<decimal>(
                name: "RoutePosition",
                table: "ActiveProjects",
                type: "decimal(18,3)",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "VehicleRoutes",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint(20)", nullable: false),
                    RevisionNumber = table.Column<int>(type: "int(11)", nullable: false),
                    EditableItemId = table.Column<long>(type: "bigint(20)", nullable: false),
                    Name = table.Column<string>(type: "varchar(200)", nullable: false, collation: "utf8mb4_unicode_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Description = table.Column<string>(type: "mediumtext", nullable: false, collation: "utf8mb4_unicode_ci")
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => new { x.Id, x.RevisionNumber });
                    table.ForeignKey(
                        name: "FK_VehicleRoutes_EditableItems",
                        column: x => x.EditableItemId,
                        principalTable: "EditableItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "VehicleRouteStops",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint(20)", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    VehicleRouteId = table.Column<long>(type: "bigint(20)", nullable: false),
                    VehicleRouteRevision = table.Column<int>(type: "int(11)", nullable: false),
                    Name = table.Column<string>(type: "varchar(200)", nullable: false, collation: "utf8mb4_unicode_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Sequence = table.Column<int>(type: "int(11)", nullable: false),
                    CellId = table.Column<long>(type: "bigint(20)", nullable: false),
                    RoomLayer = table.Column<int>(type: "int(11)", nullable: false),
                    RoutePositionMetres = table.Column<decimal>(type: "decimal(18,3)", nullable: true),
                    DwellDurationMilliseconds = table.Column<long>(type: "bigint(20)", nullable: false, defaultValue: 0L)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.Id);
                    table.UniqueConstraint("AK_VehicleRouteStops_Id_Route", x => new { x.Id, x.VehicleRouteId, x.VehicleRouteRevision });
                    table.CheckConstraint("CK_VehicleRouteStops_Dwell", "`DwellDurationMilliseconds` >= 0");
                    table.CheckConstraint("CK_VehicleRouteStops_RoutePosition", "`RoutePositionMetres` IS NULL OR `RoutePositionMetres` >= 0");
                    table.CheckConstraint("CK_VehicleRouteStops_Sequence", "`Sequence` >= 0");
                    table.ForeignKey(
                        name: "FK_VehicleRouteStops_Cells",
                        column: x => x.CellId,
                        principalTable: "Cells",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_VehicleRouteStops_VehicleRoutes",
                        columns: x => new { x.VehicleRouteId, x.VehicleRouteRevision },
                        principalTable: "VehicleRoutes",
                        principalColumns: new[] { "Id", "RevisionNumber" },
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "VehicleRouteTopologyPins",
                columns: table => new
                {
                    VehicleRouteId = table.Column<long>(type: "bigint(20)", nullable: false),
                    VehicleRouteRevision = table.Column<int>(type: "int(11)", nullable: false),
                    RouteCellId = table.Column<long>(type: "bigint(20)", nullable: false),
                    TopologyVersion = table.Column<long>(type: "bigint(20)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => new { x.VehicleRouteId, x.VehicleRouteRevision, x.RouteCellId });
                    table.CheckConstraint("CK_VehicleRouteTopologyPins_Version", "`TopologyVersion` >= 1");
                    table.ForeignKey(
                        name: "FK_VehicleRouteTopologyPins_RouteCells",
                        column: x => x.RouteCellId,
                        principalTable: "RouteCells",
                        principalColumn: "CellId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_VehicleRouteTopologyPins_VehicleRoutes",
                        columns: x => new { x.VehicleRouteId, x.VehicleRouteRevision },
                        principalTable: "VehicleRoutes",
                        principalColumns: new[] { "Id", "RevisionNumber" },
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "VehicleServices",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint(20)", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(type: "varchar(200)", nullable: false, collation: "utf8mb4_unicode_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Keywords = table.Column<string>(type: "varchar(1000)", nullable: false, collation: "utf8mb4_unicode_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    VehicleRouteId = table.Column<long>(type: "bigint(20)", nullable: false),
                    VehicleRouteRevision = table.Column<int>(type: "int(11)", nullable: false),
                    VehicleId = table.Column<long>(type: "bigint(20)", nullable: false),
                    OperatorMode = table.Column<int>(type: "int(11)", nullable: false),
                    RetryIntervalMilliseconds = table.Column<long>(type: "bigint(20)", nullable: false, defaultValue: 30000L),
                    MaximumHoldMilliseconds = table.Column<long>(type: "bigint(20)", nullable: false, defaultValue: 900000L),
                    Enabled = table.Column<ulong>(type: "bit(1)", nullable: false, defaultValue: 1ul)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.Id);
                    table.CheckConstraint("CK_VehicleServices_MaximumHold", "`MaximumHoldMilliseconds` >= 0");
                    table.CheckConstraint("CK_VehicleServices_OperatorMode", "`OperatorMode` IN (0, 1)");
                    table.CheckConstraint("CK_VehicleServices_RetryInterval", "`RetryIntervalMilliseconds` > 0");
                    table.ForeignKey(
                        name: "FK_VehicleServices_VehicleRoutes",
                        columns: x => new { x.VehicleRouteId, x.VehicleRouteRevision },
                        principalTable: "VehicleRoutes",
                        principalColumns: new[] { "Id", "RevisionNumber" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_VehicleServices_Vehicles",
                        column: x => x.VehicleId,
                        principalTable: "Vehicles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "VehicleRouteLegs",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint(20)", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    VehicleRouteId = table.Column<long>(type: "bigint(20)", nullable: false),
                    VehicleRouteRevision = table.Column<int>(type: "int(11)", nullable: false),
                    Sequence = table.Column<int>(type: "int(11)", nullable: false),
                    OriginStopId = table.Column<long>(type: "bigint(20)", nullable: false),
                    DestinationStopId = table.Column<long>(type: "bigint(20)", nullable: false),
                    RouteDistanceMetres = table.Column<decimal>(type: "decimal(18,3)", nullable: false),
                    RoomEquivalentCost = table.Column<decimal>(type: "decimal(18,6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.Id);
                    table.CheckConstraint("CK_VehicleRouteLegs_Distance", "`RouteDistanceMetres` >= 0");
                    table.CheckConstraint("CK_VehicleRouteLegs_DistinctStops", "`OriginStopId` <> `DestinationStopId`");
                    table.CheckConstraint("CK_VehicleRouteLegs_RoomEquivalentCost", "`RoomEquivalentCost` >= 0");
                    table.CheckConstraint("CK_VehicleRouteLegs_Sequence", "`Sequence` >= 0");
                    table.ForeignKey(
                        name: "FK_VehicleRouteLegs_DestinationStops",
                        columns: x => new { x.DestinationStopId, x.VehicleRouteId, x.VehicleRouteRevision },
                        principalTable: "VehicleRouteStops",
                        principalColumns: new[] { "Id", "VehicleRouteId", "VehicleRouteRevision" },
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_VehicleRouteLegs_OriginStops",
                        columns: x => new { x.OriginStopId, x.VehicleRouteId, x.VehicleRouteRevision },
                        principalTable: "VehicleRouteStops",
                        principalColumns: new[] { "Id", "VehicleRouteId", "VehicleRouteRevision" },
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_VehicleRouteLegs_VehicleRoutes",
                        columns: x => new { x.VehicleRouteId, x.VehicleRouteRevision },
                        principalTable: "VehicleRoutes",
                        principalColumns: new[] { "Id", "RevisionNumber" },
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "VehicleRoutePlatformBindings",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint(20)", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    VehicleRouteStopId = table.Column<long>(type: "bigint(20)", nullable: false),
                    PlatformCellId = table.Column<long>(type: "bigint(20)", nullable: false),
                    VehicleAccessPointProtoId = table.Column<long>(type: "bigint(20)", nullable: false),
                    DockingToleranceMetres = table.Column<decimal>(type: "decimal(18,3)", nullable: false, defaultValue: 2.0m)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.Id);
                    table.CheckConstraint("CK_VehicleRoutePlatformBindings_Tolerance", "`DockingToleranceMetres` >= 0");
                    table.ForeignKey(
                        name: "FK_VehicleRoutePlatformBindings_AccessPointProtos",
                        column: x => x.VehicleAccessPointProtoId,
                        principalTable: "VehicleAccessPointProtos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_VehicleRoutePlatformBindings_Cells",
                        column: x => x.PlatformCellId,
                        principalTable: "Cells",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_VehicleRoutePlatformBindings_VehicleRouteStops",
                        column: x => x.VehicleRouteStopId,
                        principalTable: "VehicleRouteStops",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "VehicleJourneys",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint(20)", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    OperationId = table.Column<string>(type: "varchar(64)", nullable: false, collation: "ascii_general_ci")
                        .Annotation("MySql:CharSet", "ascii"),
                    VehicleServiceId = table.Column<long>(type: "bigint(20)", nullable: false),
                    VehicleRouteId = table.Column<long>(type: "bigint(20)", nullable: false),
                    VehicleRouteRevision = table.Column<int>(type: "int(11)", nullable: false),
                    VehicleId = table.Column<long>(type: "bigint(20)", nullable: false),
                    State = table.Column<int>(type: "int(11)", nullable: false, defaultValue: 0),
                    CurrentStopId = table.Column<long>(type: "bigint(20)", nullable: true),
                    NextStopId = table.Column<long>(type: "bigint(20)", nullable: true),
                    ScheduledDeparture = table.Column<string>(type: "varchar(500)", nullable: false, collation: "utf8_general_ci")
                        .Annotation("MySql:CharSet", "utf8"),
                    ExpectedDeparture = table.Column<string>(type: "varchar(500)", nullable: false, collation: "utf8_general_ci")
                        .Annotation("MySql:CharSet", "utf8"),
                    DelayMilliseconds = table.Column<long>(type: "bigint(20)", nullable: false, defaultValue: 0L),
                    LastCheckpointUtc = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.Id);
                    table.CheckConstraint("CK_VehicleJourneys_Delay", "`DelayMilliseconds` >= 0");
                    table.CheckConstraint("CK_VehicleJourneys_State", "`State` BETWEEN 0 AND 8");
                    table.ForeignKey(
                        name: "FK_VehicleJourneys_CurrentStops",
                        columns: x => new { x.CurrentStopId, x.VehicleRouteId, x.VehicleRouteRevision },
                        principalTable: "VehicleRouteStops",
                        principalColumns: new[] { "Id", "VehicleRouteId", "VehicleRouteRevision" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_VehicleJourneys_NextStops",
                        columns: x => new { x.NextStopId, x.VehicleRouteId, x.VehicleRouteRevision },
                        principalTable: "VehicleRouteStops",
                        principalColumns: new[] { "Id", "VehicleRouteId", "VehicleRouteRevision" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_VehicleJourneys_VehicleRoutes",
                        columns: x => new { x.VehicleRouteId, x.VehicleRouteRevision },
                        principalTable: "VehicleRoutes",
                        principalColumns: new[] { "Id", "RevisionNumber" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_VehicleJourneys_VehicleServices",
                        column: x => x.VehicleServiceId,
                        principalTable: "VehicleServices",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_VehicleJourneys_Vehicles",
                        column: x => x.VehicleId,
                        principalTable: "Vehicles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "VehicleServiceSchedules",
                columns: table => new
                {
                    VehicleServiceId = table.Column<long>(type: "bigint(20)", nullable: false),
                    ReferenceDeparture = table.Column<string>(type: "varchar(500)", nullable: false, collation: "utf8_general_ci")
                        .Annotation("MySql:CharSet", "utf8"),
                    NextDeparture = table.Column<string>(type: "varchar(500)", nullable: false, collation: "utf8_general_ci")
                        .Annotation("MySql:CharSet", "utf8"),
                    RecurrenceType = table.Column<int>(type: "int(11)", nullable: false),
                    RecurrenceIntervalAmount = table.Column<int>(type: "int(11)", nullable: false),
                    RecurrenceModifier = table.Column<int>(type: "int(11)", nullable: false),
                    RecurrenceSecondaryModifier = table.Column<int>(type: "int(11)", nullable: false),
                    RecurrenceFallbackMode = table.Column<int>(type: "int(11)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.VehicleServiceId);
                    table.CheckConstraint("CK_VehicleServiceSchedules_RecurrenceInterval", "`RecurrenceIntervalAmount` > 0");
                    table.ForeignKey(
                        name: "FK_VehicleServiceSchedules_VehicleServices",
                        column: x => x.VehicleServiceId,
                        principalTable: "VehicleServices",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "VehicleRouteSteps",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint(20)", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    VehicleRouteLegId = table.Column<long>(type: "bigint(20)", nullable: false),
                    Sequence = table.Column<int>(type: "int(11)", nullable: false),
                    StepType = table.Column<int>(type: "int(11)", nullable: false),
                    OriginCellId = table.Column<long>(type: "bigint(20)", nullable: false),
                    OriginRoomLayer = table.Column<int>(type: "int(11)", nullable: false),
                    OriginRoutePositionMetres = table.Column<decimal>(type: "decimal(18,3)", nullable: true),
                    DestinationCellId = table.Column<long>(type: "bigint(20)", nullable: false),
                    DestinationRoomLayer = table.Column<int>(type: "int(11)", nullable: false),
                    DestinationRoutePositionMetres = table.Column<decimal>(type: "decimal(18,3)", nullable: true),
                    DistanceMetres = table.Column<decimal>(type: "decimal(18,3)", nullable: true),
                    RoomEquivalentCost = table.Column<decimal>(type: "decimal(18,6)", nullable: false),
                    Direction = table.Column<int>(type: "int(11)", nullable: true),
                    PinnedTopologyVersion = table.Column<long>(type: "bigint(20)", nullable: true),
                    DestinationTopologyVersion = table.Column<long>(type: "bigint(20)", nullable: true),
                    ExitId = table.Column<long>(type: "bigint(20)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.Id);
                    table.CheckConstraint("CK_VehicleRouteSteps_Positions", "(`OriginRoutePositionMetres` IS NULL OR `OriginRoutePositionMetres` >= 0) AND (`DestinationRoutePositionMetres` IS NULL OR `DestinationRoutePositionMetres` >= 0) AND ((`OriginRoutePositionMetres` IS NULL AND `PinnedTopologyVersion` IS NULL) OR (`OriginRoutePositionMetres` IS NOT NULL AND `PinnedTopologyVersion` IS NOT NULL AND `PinnedTopologyVersion` >= 1)) AND ((`DestinationRoutePositionMetres` IS NULL AND `DestinationTopologyVersion` IS NULL) OR (`DestinationRoutePositionMetres` IS NOT NULL AND `DestinationTopologyVersion` IS NOT NULL AND `DestinationTopologyVersion` >= 1))");
                    table.CheckConstraint("CK_VehicleRouteSteps_RoomEquivalentCost", "`RoomEquivalentCost` >= 0");
                    table.CheckConstraint("CK_VehicleRouteSteps_Sequence", "`Sequence` >= 0");
                    table.CheckConstraint("CK_VehicleRouteSteps_TypedPayload", "(`StepType` = 0 AND `ExitId` IS NULL AND `Direction` IS NOT NULL AND `Direction` IN (-1, 1) AND `PinnedTopologyVersion` IS NOT NULL AND `DestinationTopologyVersion` = `PinnedTopologyVersion` AND `DistanceMetres` IS NOT NULL AND `DistanceMetres` >= 0 AND `OriginRoutePositionMetres` IS NOT NULL AND `DestinationRoutePositionMetres` IS NOT NULL AND `OriginCellId` = `DestinationCellId` AND `OriginRoomLayer` = `DestinationRoomLayer`) OR (`StepType` = 1 AND `ExitId` IS NOT NULL AND `Direction` IS NULL AND `DistanceMetres` IS NULL)");
                    table.ForeignKey(
                        name: "FK_VehicleRouteSteps_DestinationCells",
                        column: x => x.DestinationCellId,
                        principalTable: "Cells",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_VehicleRouteSteps_Exits",
                        column: x => x.ExitId,
                        principalTable: "Exits",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_VehicleRouteSteps_OriginCells",
                        column: x => x.OriginCellId,
                        principalTable: "Cells",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_VehicleRouteSteps_VehicleRouteLegs",
                        column: x => x.VehicleRouteLegId,
                        principalTable: "VehicleRouteLegs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "VehicleJourneyEvents",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint(20)", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    VehicleJourneyId = table.Column<long>(type: "bigint(20)", nullable: false),
                    Sequence = table.Column<long>(type: "bigint(20)", nullable: false),
                    IdempotencyKey = table.Column<string>(type: "varchar(128)", nullable: false, collation: "ascii_general_ci")
                        .Annotation("MySql:CharSet", "ascii"),
                    EventType = table.Column<int>(type: "int(11)", nullable: false),
                    State = table.Column<int>(type: "int(11)", nullable: false),
                    OccurredAtUtc = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    WorldTime = table.Column<string>(type: "varchar(500)", nullable: true, collation: "utf8_general_ci")
                        .Annotation("MySql:CharSet", "utf8"),
                    Message = table.Column<string>(type: "mediumtext", nullable: false, collation: "utf8mb4_unicode_ci")
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.Id);
                    table.CheckConstraint("CK_VehicleJourneyEvents_EventType", "`EventType` BETWEEN 0 AND 11");
                    table.CheckConstraint("CK_VehicleJourneyEvents_Sequence", "`Sequence` >= 0");
                    table.CheckConstraint("CK_VehicleJourneyEvents_State", "`State` BETWEEN 0 AND 8");
                    table.ForeignKey(
                        name: "FK_VehicleJourneyEvents_VehicleJourneys",
                        column: x => x.VehicleJourneyId,
                        principalTable: "VehicleJourneys",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "FK_VehicleDockings_VehicleRouteStops_idx",
                table: "VehicleDockings",
                column: "VehicleRouteStopId");

            migrationBuilder.CreateIndex(
                name: "FK_Tracks_Vehicles_idx",
                table: "Tracks",
                column: "VehicleId");

            migrationBuilder.AddCheckConstraint(
                name: "CK_Tracks_Owner",
                table: "Tracks",
                sql: "(`VehicleId` IS NULL AND `CharacterId` IS NOT NULL AND `BodyPrototypeId` IS NOT NULL) OR (`VehicleId` IS NOT NULL AND `CharacterId` IS NULL AND `BodyPrototypeId` IS NULL)");

            migrationBuilder.CreateIndex(
                name: "UX_VehicleJourneyEvents_Idempotency",
                table: "VehicleJourneyEvents",
                column: "IdempotencyKey",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "UX_VehicleJourneyEvents_Journey_Sequence",
                table: "VehicleJourneyEvents",
                columns: new[] { "VehicleJourneyId", "Sequence" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "FK_VehicleJourneys_CurrentStops_idx",
                table: "VehicleJourneys",
                columns: new[] { "CurrentStopId", "VehicleRouteId", "VehicleRouteRevision" });

            migrationBuilder.CreateIndex(
                name: "FK_VehicleJourneys_NextStops_idx",
                table: "VehicleJourneys",
                columns: new[] { "NextStopId", "VehicleRouteId", "VehicleRouteRevision" });

            migrationBuilder.CreateIndex(
                name: "FK_VehicleJourneys_VehicleRoutes_idx",
                table: "VehicleJourneys",
                columns: new[] { "VehicleRouteId", "VehicleRouteRevision" });

            migrationBuilder.CreateIndex(
                name: "FK_VehicleJourneys_Vehicles_idx",
                table: "VehicleJourneys",
                column: "VehicleId");

            migrationBuilder.CreateIndex(
                name: "FK_VehicleJourneys_VehicleServices_idx",
                table: "VehicleJourneys",
                column: "VehicleServiceId");

            migrationBuilder.CreateIndex(
                name: "IX_VehicleJourneys_Service_State",
                table: "VehicleJourneys",
                columns: new[] { "VehicleServiceId", "State" });

            migrationBuilder.CreateIndex(
                name: "UX_VehicleJourneys_Operation",
                table: "VehicleJourneys",
                column: "OperationId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "UX_VehicleJourneys_Service_ScheduledDeparture",
                table: "VehicleJourneys",
                columns: new[] { "VehicleServiceId", "ScheduledDeparture" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "FK_VehicleRouteLegs_DestinationStops_idx",
                table: "VehicleRouteLegs",
                columns: new[] { "DestinationStopId", "VehicleRouteId", "VehicleRouteRevision" });

            migrationBuilder.CreateIndex(
                name: "FK_VehicleRouteLegs_OriginStops_idx",
                table: "VehicleRouteLegs",
                columns: new[] { "OriginStopId", "VehicleRouteId", "VehicleRouteRevision" });

            migrationBuilder.CreateIndex(
                name: "UX_VehicleRouteLegs_Route_Sequence",
                table: "VehicleRouteLegs",
                columns: new[] { "VehicleRouteId", "VehicleRouteRevision", "Sequence" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "FK_VehicleRoutePlatformBindings_AccessPointProtos_idx",
                table: "VehicleRoutePlatformBindings",
                column: "VehicleAccessPointProtoId");

            migrationBuilder.CreateIndex(
                name: "FK_VehicleRoutePlatformBindings_Cells_idx",
                table: "VehicleRoutePlatformBindings",
                column: "PlatformCellId");

            migrationBuilder.CreateIndex(
                name: "UX_VehicleRoutePlatformBindings_Stop_Platform_AccessPoint",
                table: "VehicleRoutePlatformBindings",
                columns: new[] { "VehicleRouteStopId", "PlatformCellId", "VehicleAccessPointProtoId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "FK_VehicleRoutes_EditableItems_idx",
                table: "VehicleRoutes",
                column: "EditableItemId");

            migrationBuilder.CreateIndex(
                name: "FK_VehicleRouteSteps_DestinationCells_idx",
                table: "VehicleRouteSteps",
                column: "DestinationCellId");

            migrationBuilder.CreateIndex(
                name: "FK_VehicleRouteSteps_Exits_idx",
                table: "VehicleRouteSteps",
                column: "ExitId");

            migrationBuilder.CreateIndex(
                name: "FK_VehicleRouteSteps_OriginCells_idx",
                table: "VehicleRouteSteps",
                column: "OriginCellId");

            migrationBuilder.CreateIndex(
                name: "UX_VehicleRouteSteps_Leg_Sequence",
                table: "VehicleRouteSteps",
                columns: new[] { "VehicleRouteLegId", "Sequence" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "FK_VehicleRouteStops_Cells_idx",
                table: "VehicleRouteStops",
                column: "CellId");

            migrationBuilder.CreateIndex(
                name: "UX_VehicleRouteStops_Route_Sequence",
                table: "VehicleRouteStops",
                columns: new[] { "VehicleRouteId", "VehicleRouteRevision", "Sequence" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "FK_VehicleRouteTopologyPins_RouteCells_idx",
                table: "VehicleRouteTopologyPins",
                column: "RouteCellId");

            migrationBuilder.CreateIndex(
                name: "FK_VehicleServices_VehicleRoutes_idx",
                table: "VehicleServices",
                columns: new[] { "VehicleRouteId", "VehicleRouteRevision" });

            migrationBuilder.CreateIndex(
                name: "IX_VehicleServices_Name",
                table: "VehicleServices",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_VehicleServices_Vehicle_Enabled",
                table: "VehicleServices",
                columns: new[] { "VehicleId", "Enabled" });

            migrationBuilder.AddForeignKey(
                name: "FK_Tracks_Vehicles",
                table: "Tracks",
                column: "VehicleId",
                principalTable: "Vehicles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_VehicleDockings_VehicleRouteStops",
                table: "VehicleDockings",
                column: "VehicleRouteStopId",
                principalTable: "VehicleRouteStops",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Tracks_Vehicles",
                table: "Tracks");

            migrationBuilder.DropForeignKey(
                name: "FK_VehicleDockings_VehicleRouteStops",
                table: "VehicleDockings");

            migrationBuilder.DropTable(
                name: "VehicleJourneyEvents");

            migrationBuilder.DropTable(
                name: "VehicleRoutePlatformBindings");

            migrationBuilder.DropTable(
                name: "VehicleRouteSteps");

            migrationBuilder.DropTable(
                name: "VehicleRouteTopologyPins");

            migrationBuilder.DropTable(
                name: "VehicleServiceSchedules");

            migrationBuilder.DropTable(
                name: "VehicleJourneys");

            migrationBuilder.DropTable(
                name: "VehicleRouteLegs");

            migrationBuilder.DropTable(
                name: "VehicleServices");

            migrationBuilder.DropTable(
                name: "VehicleRouteStops");

            migrationBuilder.DropTable(
                name: "VehicleRoutes");

            migrationBuilder.DropIndex(
                name: "FK_VehicleDockings_VehicleRouteStops_idx",
                table: "VehicleDockings");

            migrationBuilder.DropIndex(
                name: "FK_Tracks_Vehicles_idx",
                table: "Tracks");

            migrationBuilder.DropCheckConstraint(
                name: "CK_Tracks_Owner",
                table: "Tracks");

            migrationBuilder.DropColumn(
                name: "AutomaticOperationCapable",
                table: "VehicleMovementProfileProtos");

            migrationBuilder.DropColumn(
                name: "RouteFuelVolumePerMetre",
                table: "VehicleMovementProfileProtos");

            migrationBuilder.DropColumn(
                name: "RoutePowerDrawWatts",
                table: "VehicleMovementProfileProtos");

            migrationBuilder.DropColumn(
                name: "RoutePropulsionMode",
                table: "VehicleMovementProfileProtos");

            migrationBuilder.DropColumn(
                name: "RouteSpeedMetresPerSecond",
                table: "VehicleMovementProfileProtos");

            migrationBuilder.DropColumn(
                name: "VehicleRouteStopId",
                table: "VehicleDockings");

            migrationBuilder.DropColumn(
                name: "VehicleId",
                table: "Tracks");

            migrationBuilder.DropColumn(
                name: "RoomLayer",
                table: "ActiveProjects");

            migrationBuilder.DropColumn(
                name: "RoutePosition",
                table: "ActiveProjects");

            migrationBuilder.AlterColumn<long>(
                name: "CharacterId",
                table: "Tracks",
                type: "bigint(20)",
                nullable: false,
                defaultValue: 0L,
                oldClrType: typeof(long),
                oldType: "bigint(20)",
                oldNullable: true);

            migrationBuilder.AlterColumn<long>(
                name: "BodyPrototypeId",
                table: "Tracks",
                type: "bigint(20)",
                nullable: false,
                defaultValue: 0L,
                oldClrType: typeof(long),
                oldType: "bigint(20)",
                oldNullable: true);
        }
    }
}
