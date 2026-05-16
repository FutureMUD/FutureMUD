using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MudSharp.Migrations
{
    /// <inheritdoc />
    public partial class VehicleSystemsPhase2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "VehicleDamageZoneId",
                table: "Wounds",
                type: "bigint(20)",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "VehicleId",
                table: "Wounds",
                type: "bigint(20)",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "FuelLiquidId",
                table: "VehicleMovementProfileProtos",
                type: "bigint(20)",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "FuelVolumePerMove",
                table: "VehicleMovementProfileProtos",
                type: "double",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<string>(
                name: "RequiredInstalledRole",
                table: "VehicleMovementProfileProtos",
                type: "varchar(200)",
                nullable: false,
                defaultValue: "",
                collation: "utf8_general_ci")
                .Annotation("MySql:CharSet", "utf8");

            migrationBuilder.AddColumn<double>(
                name: "RequiredPowerSpikeInWatts",
                table: "VehicleMovementProfileProtos",
                type: "double",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<ulong>(
                name: "RequiresAccessPointsClosed",
                table: "VehicleMovementProfileProtos",
                type: "bit(1)",
                nullable: false,
                defaultValue: 0ul);

            migrationBuilder.AddColumn<ulong>(
                name: "RequiresTowLinksClosed",
                table: "VehicleMovementProfileProtos",
                type: "bit(1)",
                nullable: false,
                defaultValue: 0ul);

            migrationBuilder.CreateTable(
                name: "VehicleAccessPointProtos",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint(20)", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    VehicleProtoId = table.Column<long>(type: "bigint(20)", nullable: false),
                    VehicleProtoRevision = table.Column<int>(type: "int(11)", nullable: false),
                    VehicleCompartmentProtoId = table.Column<long>(type: "bigint(20)", nullable: true),
                    Name = table.Column<string>(type: "varchar(200)", nullable: false, collation: "utf8_general_ci")
                        .Annotation("MySql:CharSet", "utf8"),
                    Description = table.Column<string>(type: "mediumtext", nullable: false, collation: "utf8_general_ci")
                        .Annotation("MySql:CharSet", "utf8"),
                    AccessPointType = table.Column<int>(type: "int(11)", nullable: false),
                    ProjectionItemProtoId = table.Column<long>(type: "bigint(20)", nullable: true),
                    ProjectionItemProtoRevision = table.Column<int>(type: "int(11)", nullable: true),
                    StartsOpen = table.Column<ulong>(type: "bit(1)", nullable: false),
                    MustBeClosedForMovement = table.Column<ulong>(type: "bit(1)", nullable: false),
                    DisplayOrder = table.Column<int>(type: "int(11)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.Id);
                    table.ForeignKey(
                        name: "FK_VehicleAccessPointProtos_Compartments",
                        column: x => x.VehicleCompartmentProtoId,
                        principalTable: "VehicleCompartmentProtos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_VehicleAccessPointProtos_ItemProtos",
                        columns: x => new { x.ProjectionItemProtoId, x.ProjectionItemProtoRevision },
                        principalTable: "GameItemProtos",
                        principalColumns: new[] { "Id", "RevisionNumber" },
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_VehicleAccessPointProtos_VehicleProtos",
                        columns: x => new { x.VehicleProtoId, x.VehicleProtoRevision },
                        principalTable: "VehicleProtos",
                        principalColumns: new[] { "Id", "RevisionNumber" },
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "VehicleDamageZoneProtos",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint(20)", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    VehicleProtoId = table.Column<long>(type: "bigint(20)", nullable: false),
                    VehicleProtoRevision = table.Column<int>(type: "int(11)", nullable: false),
                    Name = table.Column<string>(type: "varchar(200)", nullable: false, collation: "utf8_general_ci")
                        .Annotation("MySql:CharSet", "utf8"),
                    Description = table.Column<string>(type: "mediumtext", nullable: false, collation: "utf8_general_ci")
                        .Annotation("MySql:CharSet", "utf8"),
                    MaximumDamage = table.Column<double>(type: "double", nullable: false),
                    HitWeight = table.Column<double>(type: "double", nullable: false),
                    DisabledThreshold = table.Column<double>(type: "double", nullable: false),
                    DestroyedThreshold = table.Column<double>(type: "double", nullable: false),
                    DisablesMovement = table.Column<ulong>(type: "bit(1)", nullable: false),
                    DisplayOrder = table.Column<int>(type: "int(11)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.Id);
                    table.ForeignKey(
                        name: "FK_VehicleDamageZoneProtos_VehicleProtos",
                        columns: x => new { x.VehicleProtoId, x.VehicleProtoRevision },
                        principalTable: "VehicleProtos",
                        principalColumns: new[] { "Id", "RevisionNumber" },
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "VehicleAccessPoints",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint(20)", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    VehicleId = table.Column<long>(type: "bigint(20)", nullable: false),
                    VehicleAccessPointProtoId = table.Column<long>(type: "bigint(20)", nullable: false),
                    Name = table.Column<string>(type: "varchar(200)", nullable: false, collation: "utf8_general_ci")
                        .Annotation("MySql:CharSet", "utf8"),
                    ProjectionItemId = table.Column<long>(type: "bigint(20)", nullable: true),
                    IsOpen = table.Column<ulong>(type: "bit(1)", nullable: false),
                    IsDisabled = table.Column<ulong>(type: "bit(1)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.Id);
                    table.ForeignKey(
                        name: "FK_VehicleAccessPoints_GameItems",
                        column: x => x.ProjectionItemId,
                        principalTable: "GameItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_VehicleAccessPoints_Protos",
                        column: x => x.VehicleAccessPointProtoId,
                        principalTable: "VehicleAccessPointProtos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_VehicleAccessPoints_Vehicles",
                        column: x => x.VehicleId,
                        principalTable: "Vehicles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "VehicleCargoSpaceProtos",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint(20)", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    VehicleProtoId = table.Column<long>(type: "bigint(20)", nullable: false),
                    VehicleProtoRevision = table.Column<int>(type: "int(11)", nullable: false),
                    VehicleCompartmentProtoId = table.Column<long>(type: "bigint(20)", nullable: true),
                    RequiredAccessPointProtoId = table.Column<long>(type: "bigint(20)", nullable: true),
                    Name = table.Column<string>(type: "varchar(200)", nullable: false, collation: "utf8_general_ci")
                        .Annotation("MySql:CharSet", "utf8"),
                    Description = table.Column<string>(type: "mediumtext", nullable: false, collation: "utf8_general_ci")
                        .Annotation("MySql:CharSet", "utf8"),
                    ProjectionItemProtoId = table.Column<long>(type: "bigint(20)", nullable: true),
                    ProjectionItemProtoRevision = table.Column<int>(type: "int(11)", nullable: true),
                    DisplayOrder = table.Column<int>(type: "int(11)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.Id);
                    table.ForeignKey(
                        name: "FK_VehicleCargoSpaceProtos_AccessPoints",
                        column: x => x.RequiredAccessPointProtoId,
                        principalTable: "VehicleAccessPointProtos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_VehicleCargoSpaceProtos_Compartments",
                        column: x => x.VehicleCompartmentProtoId,
                        principalTable: "VehicleCompartmentProtos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_VehicleCargoSpaceProtos_ItemProtos",
                        columns: x => new { x.ProjectionItemProtoId, x.ProjectionItemProtoRevision },
                        principalTable: "GameItemProtos",
                        principalColumns: new[] { "Id", "RevisionNumber" },
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_VehicleCargoSpaceProtos_VehicleProtos",
                        columns: x => new { x.VehicleProtoId, x.VehicleProtoRevision },
                        principalTable: "VehicleProtos",
                        principalColumns: new[] { "Id", "RevisionNumber" },
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "VehicleInstallationPointProtos",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint(20)", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    VehicleProtoId = table.Column<long>(type: "bigint(20)", nullable: false),
                    VehicleProtoRevision = table.Column<int>(type: "int(11)", nullable: false),
                    RequiredAccessPointProtoId = table.Column<long>(type: "bigint(20)", nullable: true),
                    Name = table.Column<string>(type: "varchar(200)", nullable: false, collation: "utf8_general_ci")
                        .Annotation("MySql:CharSet", "utf8"),
                    Description = table.Column<string>(type: "mediumtext", nullable: false, collation: "utf8_general_ci")
                        .Annotation("MySql:CharSet", "utf8"),
                    MountType = table.Column<string>(type: "varchar(200)", nullable: false, collation: "utf8_general_ci")
                        .Annotation("MySql:CharSet", "utf8"),
                    RequiredRole = table.Column<string>(type: "varchar(200)", nullable: false, collation: "utf8_general_ci")
                        .Annotation("MySql:CharSet", "utf8"),
                    RequiredForMovement = table.Column<ulong>(type: "bit(1)", nullable: false),
                    DisplayOrder = table.Column<int>(type: "int(11)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.Id);
                    table.ForeignKey(
                        name: "FK_VehicleInstallationPointProtos_AccessPoints",
                        column: x => x.RequiredAccessPointProtoId,
                        principalTable: "VehicleAccessPointProtos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_VehicleInstallationPointProtos_VehicleProtos",
                        columns: x => new { x.VehicleProtoId, x.VehicleProtoRevision },
                        principalTable: "VehicleProtos",
                        principalColumns: new[] { "Id", "RevisionNumber" },
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "VehicleTowPointProtos",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint(20)", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    VehicleProtoId = table.Column<long>(type: "bigint(20)", nullable: false),
                    VehicleProtoRevision = table.Column<int>(type: "int(11)", nullable: false),
                    RequiredAccessPointProtoId = table.Column<long>(type: "bigint(20)", nullable: true),
                    Name = table.Column<string>(type: "varchar(200)", nullable: false, collation: "utf8_general_ci")
                        .Annotation("MySql:CharSet", "utf8"),
                    Description = table.Column<string>(type: "mediumtext", nullable: false, collation: "utf8_general_ci")
                        .Annotation("MySql:CharSet", "utf8"),
                    TowType = table.Column<string>(type: "varchar(200)", nullable: false, collation: "utf8_general_ci")
                        .Annotation("MySql:CharSet", "utf8"),
                    CanTow = table.Column<ulong>(type: "bit(1)", nullable: false),
                    CanBeTowed = table.Column<ulong>(type: "bit(1)", nullable: false),
                    MaximumTowedWeight = table.Column<double>(type: "double", nullable: false),
                    DisplayOrder = table.Column<int>(type: "int(11)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.Id);
                    table.ForeignKey(
                        name: "FK_VehicleTowPointProtos_AccessPoints",
                        column: x => x.RequiredAccessPointProtoId,
                        principalTable: "VehicleAccessPointProtos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_VehicleTowPointProtos_VehicleProtos",
                        columns: x => new { x.VehicleProtoId, x.VehicleProtoRevision },
                        principalTable: "VehicleProtos",
                        principalColumns: new[] { "Id", "RevisionNumber" },
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "VehicleDamageZoneEffectProtos",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint(20)", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    VehicleDamageZoneProtoId = table.Column<long>(type: "bigint(20)", nullable: false),
                    TargetType = table.Column<int>(type: "int(11)", nullable: false),
                    TargetProtoId = table.Column<long>(type: "bigint(20)", nullable: true),
                    MinimumStatus = table.Column<int>(type: "int(11)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.Id);
                    table.ForeignKey(
                        name: "FK_VehicleDamageZoneEffectProtos_DamageZones",
                        column: x => x.VehicleDamageZoneProtoId,
                        principalTable: "VehicleDamageZoneProtos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "VehicleDamageZones",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint(20)", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    VehicleId = table.Column<long>(type: "bigint(20)", nullable: false),
                    VehicleDamageZoneProtoId = table.Column<long>(type: "bigint(20)", nullable: false),
                    Name = table.Column<string>(type: "varchar(200)", nullable: false, collation: "utf8_general_ci")
                        .Annotation("MySql:CharSet", "utf8"),
                    CurrentDamage = table.Column<double>(type: "double", nullable: false),
                    Status = table.Column<int>(type: "int(11)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.Id);
                    table.ForeignKey(
                        name: "FK_VehicleDamageZones_Protos",
                        column: x => x.VehicleDamageZoneProtoId,
                        principalTable: "VehicleDamageZoneProtos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_VehicleDamageZones_Vehicles",
                        column: x => x.VehicleId,
                        principalTable: "Vehicles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "VehicleAccessPointLocks",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint(20)", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    VehicleAccessPointId = table.Column<long>(type: "bigint(20)", nullable: false),
                    LockItemId = table.Column<long>(type: "bigint(20)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.Id);
                    table.ForeignKey(
                        name: "FK_VehicleAccessPointLocks_AccessPoints",
                        column: x => x.VehicleAccessPointId,
                        principalTable: "VehicleAccessPoints",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_VehicleAccessPointLocks_GameItems",
                        column: x => x.LockItemId,
                        principalTable: "GameItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "VehicleCargoSpaces",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint(20)", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    VehicleId = table.Column<long>(type: "bigint(20)", nullable: false),
                    VehicleCargoSpaceProtoId = table.Column<long>(type: "bigint(20)", nullable: false),
                    Name = table.Column<string>(type: "varchar(200)", nullable: false, collation: "utf8_general_ci")
                        .Annotation("MySql:CharSet", "utf8"),
                    ProjectionItemId = table.Column<long>(type: "bigint(20)", nullable: true),
                    IsDisabled = table.Column<ulong>(type: "bit(1)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.Id);
                    table.ForeignKey(
                        name: "FK_VehicleCargoSpaces_GameItems",
                        column: x => x.ProjectionItemId,
                        principalTable: "GameItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_VehicleCargoSpaces_Protos",
                        column: x => x.VehicleCargoSpaceProtoId,
                        principalTable: "VehicleCargoSpaceProtos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_VehicleCargoSpaces_Vehicles",
                        column: x => x.VehicleId,
                        principalTable: "Vehicles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "VehicleInstallations",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint(20)", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    VehicleId = table.Column<long>(type: "bigint(20)", nullable: false),
                    VehicleInstallationPointProtoId = table.Column<long>(type: "bigint(20)", nullable: false),
                    InstalledItemId = table.Column<long>(type: "bigint(20)", nullable: true),
                    IsDisabled = table.Column<ulong>(type: "bit(1)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.Id);
                    table.ForeignKey(
                        name: "FK_VehicleInstallations_GameItems",
                        column: x => x.InstalledItemId,
                        principalTable: "GameItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_VehicleInstallations_Protos",
                        column: x => x.VehicleInstallationPointProtoId,
                        principalTable: "VehicleInstallationPointProtos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_VehicleInstallations_Vehicles",
                        column: x => x.VehicleId,
                        principalTable: "Vehicles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "VehicleTowLinks",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint(20)", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    SourceVehicleId = table.Column<long>(type: "bigint(20)", nullable: false),
                    TargetVehicleId = table.Column<long>(type: "bigint(20)", nullable: false),
                    SourceTowPointProtoId = table.Column<long>(type: "bigint(20)", nullable: false),
                    TargetTowPointProtoId = table.Column<long>(type: "bigint(20)", nullable: false),
                    HitchItemId = table.Column<long>(type: "bigint(20)", nullable: true),
                    IsDisabled = table.Column<ulong>(type: "bit(1)", nullable: false),
                    CreatedDateTime = table.Column<DateTime>(type: "datetime", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.Id);
                    table.ForeignKey(
                        name: "FK_VehicleTowLinks_GameItems",
                        column: x => x.HitchItemId,
                        principalTable: "GameItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_VehicleTowLinks_SourceTowPointProtos",
                        column: x => x.SourceTowPointProtoId,
                        principalTable: "VehicleTowPointProtos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_VehicleTowLinks_SourceVehicles",
                        column: x => x.SourceVehicleId,
                        principalTable: "Vehicles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_VehicleTowLinks_TargetTowPointProtos",
                        column: x => x.TargetTowPointProtoId,
                        principalTable: "VehicleTowPointProtos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_VehicleTowLinks_TargetVehicles",
                        column: x => x.TargetVehicleId,
                        principalTable: "Vehicles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "FK_Wounds_VehicleDamageZones_idx",
                table: "Wounds",
                column: "VehicleDamageZoneId");

            migrationBuilder.CreateIndex(
                name: "FK_Wounds_Vehicles_idx",
                table: "Wounds",
                column: "VehicleId");

            migrationBuilder.CreateIndex(
                name: "FK_VehicleAccessPointLocks_AccessPoints_idx",
                table: "VehicleAccessPointLocks",
                column: "VehicleAccessPointId");

            migrationBuilder.CreateIndex(
                name: "FK_VehicleAccessPointLocks_GameItems_idx",
                table: "VehicleAccessPointLocks",
                column: "LockItemId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "FK_VehicleAccessPointProtos_Compartments_idx",
                table: "VehicleAccessPointProtos",
                column: "VehicleCompartmentProtoId");

            migrationBuilder.CreateIndex(
                name: "FK_VehicleAccessPointProtos_ItemProtos_idx",
                table: "VehicleAccessPointProtos",
                columns: new[] { "ProjectionItemProtoId", "ProjectionItemProtoRevision" });

            migrationBuilder.CreateIndex(
                name: "FK_VehicleAccessPointProtos_VehicleProtos_idx",
                table: "VehicleAccessPointProtos",
                columns: new[] { "VehicleProtoId", "VehicleProtoRevision" });

            migrationBuilder.CreateIndex(
                name: "FK_VehicleAccessPoints_GameItems_idx",
                table: "VehicleAccessPoints",
                column: "ProjectionItemId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "FK_VehicleAccessPoints_Protos_idx",
                table: "VehicleAccessPoints",
                column: "VehicleAccessPointProtoId");

            migrationBuilder.CreateIndex(
                name: "FK_VehicleAccessPoints_Vehicles_idx",
                table: "VehicleAccessPoints",
                column: "VehicleId");

            migrationBuilder.CreateIndex(
                name: "FK_VehicleCargoSpaceProtos_AccessPoints_idx",
                table: "VehicleCargoSpaceProtos",
                column: "RequiredAccessPointProtoId");

            migrationBuilder.CreateIndex(
                name: "FK_VehicleCargoSpaceProtos_Compartments_idx",
                table: "VehicleCargoSpaceProtos",
                column: "VehicleCompartmentProtoId");

            migrationBuilder.CreateIndex(
                name: "FK_VehicleCargoSpaceProtos_ItemProtos_idx",
                table: "VehicleCargoSpaceProtos",
                columns: new[] { "ProjectionItemProtoId", "ProjectionItemProtoRevision" });

            migrationBuilder.CreateIndex(
                name: "FK_VehicleCargoSpaceProtos_VehicleProtos_idx",
                table: "VehicleCargoSpaceProtos",
                columns: new[] { "VehicleProtoId", "VehicleProtoRevision" });

            migrationBuilder.CreateIndex(
                name: "FK_VehicleCargoSpaces_GameItems_idx",
                table: "VehicleCargoSpaces",
                column: "ProjectionItemId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "FK_VehicleCargoSpaces_Protos_idx",
                table: "VehicleCargoSpaces",
                column: "VehicleCargoSpaceProtoId");

            migrationBuilder.CreateIndex(
                name: "FK_VehicleCargoSpaces_Vehicles_idx",
                table: "VehicleCargoSpaces",
                column: "VehicleId");

            migrationBuilder.CreateIndex(
                name: "FK_VehicleDamageZoneEffectProtos_DamageZones_idx",
                table: "VehicleDamageZoneEffectProtos",
                column: "VehicleDamageZoneProtoId");

            migrationBuilder.CreateIndex(
                name: "FK_VehicleDamageZoneProtos_VehicleProtos_idx",
                table: "VehicleDamageZoneProtos",
                columns: new[] { "VehicleProtoId", "VehicleProtoRevision" });

            migrationBuilder.CreateIndex(
                name: "FK_VehicleDamageZones_Protos_idx",
                table: "VehicleDamageZones",
                column: "VehicleDamageZoneProtoId");

            migrationBuilder.CreateIndex(
                name: "FK_VehicleDamageZones_Vehicles_idx",
                table: "VehicleDamageZones",
                column: "VehicleId");

            migrationBuilder.CreateIndex(
                name: "FK_VehicleInstallationPointProtos_AccessPoints_idx",
                table: "VehicleInstallationPointProtos",
                column: "RequiredAccessPointProtoId");

            migrationBuilder.CreateIndex(
                name: "FK_VehicleInstallationPointProtos_VehicleProtos_idx",
                table: "VehicleInstallationPointProtos",
                columns: new[] { "VehicleProtoId", "VehicleProtoRevision" });

            migrationBuilder.CreateIndex(
                name: "FK_VehicleInstallations_GameItems_idx",
                table: "VehicleInstallations",
                column: "InstalledItemId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "FK_VehicleInstallations_Protos_idx",
                table: "VehicleInstallations",
                column: "VehicleInstallationPointProtoId");

            migrationBuilder.CreateIndex(
                name: "FK_VehicleInstallations_Vehicles_idx",
                table: "VehicleInstallations",
                column: "VehicleId");

            migrationBuilder.CreateIndex(
                name: "FK_VehicleTowLinks_GameItems_idx",
                table: "VehicleTowLinks",
                column: "HitchItemId");

            migrationBuilder.CreateIndex(
                name: "FK_VehicleTowLinks_SourceTowPointProtos_idx",
                table: "VehicleTowLinks",
                column: "SourceTowPointProtoId");

            migrationBuilder.CreateIndex(
                name: "FK_VehicleTowLinks_SourceVehicles_idx",
                table: "VehicleTowLinks",
                column: "SourceVehicleId");

            migrationBuilder.CreateIndex(
                name: "FK_VehicleTowLinks_TargetTowPointProtos_idx",
                table: "VehicleTowLinks",
                column: "TargetTowPointProtoId");

            migrationBuilder.CreateIndex(
                name: "FK_VehicleTowLinks_TargetVehicles_idx",
                table: "VehicleTowLinks",
                column: "TargetVehicleId");

            migrationBuilder.CreateIndex(
                name: "FK_VehicleTowPointProtos_AccessPoints_idx",
                table: "VehicleTowPointProtos",
                column: "RequiredAccessPointProtoId");

            migrationBuilder.CreateIndex(
                name: "FK_VehicleTowPointProtos_VehicleProtos_idx",
                table: "VehicleTowPointProtos",
                columns: new[] { "VehicleProtoId", "VehicleProtoRevision" });

            migrationBuilder.AddForeignKey(
                name: "FK_Wounds_VehicleDamageZones",
                table: "Wounds",
                column: "VehicleDamageZoneId",
                principalTable: "VehicleDamageZones",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Wounds_Vehicles",
                table: "Wounds",
                column: "VehicleId",
                principalTable: "Vehicles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Wounds_VehicleDamageZones",
                table: "Wounds");

            migrationBuilder.DropForeignKey(
                name: "FK_Wounds_Vehicles",
                table: "Wounds");

            migrationBuilder.DropTable(
                name: "VehicleAccessPointLocks");

            migrationBuilder.DropTable(
                name: "VehicleCargoSpaces");

            migrationBuilder.DropTable(
                name: "VehicleDamageZoneEffectProtos");

            migrationBuilder.DropTable(
                name: "VehicleDamageZones");

            migrationBuilder.DropTable(
                name: "VehicleInstallations");

            migrationBuilder.DropTable(
                name: "VehicleTowLinks");

            migrationBuilder.DropTable(
                name: "VehicleAccessPoints");

            migrationBuilder.DropTable(
                name: "VehicleCargoSpaceProtos");

            migrationBuilder.DropTable(
                name: "VehicleDamageZoneProtos");

            migrationBuilder.DropTable(
                name: "VehicleInstallationPointProtos");

            migrationBuilder.DropTable(
                name: "VehicleTowPointProtos");

            migrationBuilder.DropTable(
                name: "VehicleAccessPointProtos");

            migrationBuilder.DropIndex(
                name: "FK_Wounds_VehicleDamageZones_idx",
                table: "Wounds");

            migrationBuilder.DropIndex(
                name: "FK_Wounds_Vehicles_idx",
                table: "Wounds");

            migrationBuilder.DropColumn(
                name: "VehicleDamageZoneId",
                table: "Wounds");

            migrationBuilder.DropColumn(
                name: "VehicleId",
                table: "Wounds");

            migrationBuilder.DropColumn(
                name: "FuelLiquidId",
                table: "VehicleMovementProfileProtos");

            migrationBuilder.DropColumn(
                name: "FuelVolumePerMove",
                table: "VehicleMovementProfileProtos");

            migrationBuilder.DropColumn(
                name: "RequiredInstalledRole",
                table: "VehicleMovementProfileProtos");

            migrationBuilder.DropColumn(
                name: "RequiredPowerSpikeInWatts",
                table: "VehicleMovementProfileProtos");

            migrationBuilder.DropColumn(
                name: "RequiresAccessPointsClosed",
                table: "VehicleMovementProfileProtos");

            migrationBuilder.DropColumn(
                name: "RequiresTowLinksClosed",
                table: "VehicleMovementProfileProtos");
        }
    }
}
