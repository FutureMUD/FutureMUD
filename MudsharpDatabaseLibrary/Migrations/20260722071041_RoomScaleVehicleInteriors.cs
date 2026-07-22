using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MudSharp.Migrations
{
    /// <inheritdoc />
    public partial class RoomScaleVehicleInteriors : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "InteriorCellId",
                table: "VehicleCompartments",
                type: "bigint(20)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "InteriorOutdoorsType",
                table: "VehicleCompartmentProtos",
                type: "int(11)",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<long>(
                name: "InteriorTerrainId",
                table: "VehicleCompartmentProtos",
                type: "bigint(20)",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "HostedVehicleCompartmentId",
                table: "Cells",
                type: "bigint(20)",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "HostedVehicleId",
                table: "Cells",
                type: "bigint(20)",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "VehicleCompartmentLinkProtos",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint(20)", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    VehicleProtoId = table.Column<long>(type: "bigint(20)", nullable: false),
                    VehicleProtoRevision = table.Column<int>(type: "int(11)", nullable: false),
                    SourceVehicleCompartmentProtoId = table.Column<long>(type: "bigint(20)", nullable: false),
                    DestinationVehicleCompartmentProtoId = table.Column<long>(type: "bigint(20)", nullable: false),
                    OutboundDirection = table.Column<string>(type: "varchar(100)", nullable: false, collation: "utf8mb4_unicode_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    InboundDirection = table.Column<string>(type: "varchar(100)", nullable: false, collation: "utf8mb4_unicode_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    OutboundDescription = table.Column<string>(type: "varchar(500)", nullable: false, collation: "utf8mb4_unicode_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    InboundDescription = table.Column<string>(type: "varchar(500)", nullable: false, collation: "utf8mb4_unicode_ci")
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.Id);
                    table.ForeignKey(
                        name: "FK_VehicleCompartmentLinkProtos_Destination",
                        column: x => x.DestinationVehicleCompartmentProtoId,
                        principalTable: "VehicleCompartmentProtos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_VehicleCompartmentLinkProtos_Source",
                        column: x => x.SourceVehicleCompartmentProtoId,
                        principalTable: "VehicleCompartmentProtos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_VehicleCompartmentLinkProtos_VehicleProtos",
                        columns: x => new { x.VehicleProtoId, x.VehicleProtoRevision },
                        principalTable: "VehicleProtos",
                        principalColumns: new[] { "Id", "RevisionNumber" },
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "VehicleDockings",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint(20)", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    VehicleId = table.Column<long>(type: "bigint(20)", nullable: false),
                    VehicleAccessPointId = table.Column<long>(type: "bigint(20)", nullable: false),
                    VehicleCompartmentId = table.Column<long>(type: "bigint(20)", nullable: false),
                    ExteriorCellId = table.Column<long>(type: "bigint(20)", nullable: false),
                    ExteriorRoomLayer = table.Column<int>(type: "int(11)", nullable: false),
                    State = table.Column<int>(type: "int(11)", nullable: false, defaultValue: 0)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.Id);
                    table.ForeignKey(
                        name: "FK_VehicleDockings_AccessPoints",
                        column: x => x.VehicleAccessPointId,
                        principalTable: "VehicleAccessPoints",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_VehicleDockings_Compartments",
                        column: x => x.VehicleCompartmentId,
                        principalTable: "VehicleCompartments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_VehicleDockings_ExteriorCells",
                        column: x => x.ExteriorCellId,
                        principalTable: "Cells",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_VehicleDockings_Vehicles",
                        column: x => x.VehicleId,
                        principalTable: "Vehicles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "UX_VehicleCompartments_InteriorCell",
                table: "VehicleCompartments",
                column: "InteriorCellId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "FK_VehicleCompartmentProtos_Terrains_idx",
                table: "VehicleCompartmentProtos",
                column: "InteriorTerrainId");

            migrationBuilder.CreateIndex(
                name: "FK_Cells_HostedVehicles_idx",
                table: "Cells",
                column: "HostedVehicleId");

            migrationBuilder.CreateIndex(
                name: "UX_Cells_HostedVehicleCompartments",
                table: "Cells",
                column: "HostedVehicleCompartmentId",
                unique: true);

            migrationBuilder.AddCheckConstraint(
                name: "CK_Cells_HostedVehicleOwnership",
                table: "Cells",
                sql: "(`HostedVehicleId` IS NULL AND `HostedVehicleCompartmentId` IS NULL) OR (`HostedVehicleId` IS NOT NULL AND `HostedVehicleCompartmentId` IS NOT NULL)");

            migrationBuilder.CreateIndex(
                name: "FK_VehicleCompartmentLinkProtos_Destination_idx",
                table: "VehicleCompartmentLinkProtos",
                column: "DestinationVehicleCompartmentProtoId");

            migrationBuilder.CreateIndex(
                name: "FK_VehicleCompartmentLinkProtos_Source_idx",
                table: "VehicleCompartmentLinkProtos",
                column: "SourceVehicleCompartmentProtoId");

            migrationBuilder.CreateIndex(
                name: "FK_VehicleCompartmentLinkProtos_VehicleProtos_idx",
                table: "VehicleCompartmentLinkProtos",
                columns: new[] { "VehicleProtoId", "VehicleProtoRevision" });

            migrationBuilder.CreateIndex(
                name: "FK_VehicleDockings_Compartments_idx",
                table: "VehicleDockings",
                column: "VehicleCompartmentId");

            migrationBuilder.CreateIndex(
                name: "IX_VehicleDockings_ExteriorCell_Layer",
                table: "VehicleDockings",
                columns: new[] { "ExteriorCellId", "ExteriorRoomLayer" });

            migrationBuilder.CreateIndex(
                name: "IX_VehicleDockings_Vehicle_State",
                table: "VehicleDockings",
                columns: new[] { "VehicleId", "State" });

            migrationBuilder.CreateIndex(
                name: "UX_VehicleDockings_AccessPoint",
                table: "VehicleDockings",
                column: "VehicleAccessPointId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Cells_HostedVehicleCompartments",
                table: "Cells",
                column: "HostedVehicleCompartmentId",
                principalTable: "VehicleCompartments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Cells_HostedVehicles",
                table: "Cells",
                column: "HostedVehicleId",
                principalTable: "Vehicles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_VehicleCompartmentProtos_Terrains",
                table: "VehicleCompartmentProtos",
                column: "InteriorTerrainId",
                principalTable: "Terrains",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_VehicleCompartments_InteriorCells",
                table: "VehicleCompartments",
                column: "InteriorCellId",
                principalTable: "Cells",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Cells_HostedVehicleCompartments",
                table: "Cells");

            migrationBuilder.DropForeignKey(
                name: "FK_Cells_HostedVehicles",
                table: "Cells");

            migrationBuilder.DropForeignKey(
                name: "FK_VehicleCompartmentProtos_Terrains",
                table: "VehicleCompartmentProtos");

            migrationBuilder.DropForeignKey(
                name: "FK_VehicleCompartments_InteriorCells",
                table: "VehicleCompartments");

            migrationBuilder.DropTable(
                name: "VehicleCompartmentLinkProtos");

            migrationBuilder.DropTable(
                name: "VehicleDockings");

            migrationBuilder.DropIndex(
                name: "UX_VehicleCompartments_InteriorCell",
                table: "VehicleCompartments");

            migrationBuilder.DropIndex(
                name: "FK_VehicleCompartmentProtos_Terrains_idx",
                table: "VehicleCompartmentProtos");

            migrationBuilder.DropIndex(
                name: "FK_Cells_HostedVehicles_idx",
                table: "Cells");

            migrationBuilder.DropIndex(
                name: "UX_Cells_HostedVehicleCompartments",
                table: "Cells");

            migrationBuilder.DropCheckConstraint(
                name: "CK_Cells_HostedVehicleOwnership",
                table: "Cells");

            migrationBuilder.DropColumn(
                name: "InteriorCellId",
                table: "VehicleCompartments");

            migrationBuilder.DropColumn(
                name: "InteriorOutdoorsType",
                table: "VehicleCompartmentProtos");

            migrationBuilder.DropColumn(
                name: "InteriorTerrainId",
                table: "VehicleCompartmentProtos");

            migrationBuilder.DropColumn(
                name: "HostedVehicleCompartmentId",
                table: "Cells");

            migrationBuilder.DropColumn(
                name: "HostedVehicleId",
                table: "Cells");
        }
    }
}
