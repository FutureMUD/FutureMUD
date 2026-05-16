using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MudSharp.Migrations
{
    /// <inheritdoc />
    public partial class VehiclesHybridModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "VehicleProtos",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint(20)", nullable: false),
                    RevisionNumber = table.Column<int>(type: "int(11)", nullable: false),
                    EditableItemId = table.Column<long>(type: "bigint(20)", nullable: false),
                    Name = table.Column<string>(type: "varchar(200)", nullable: false, collation: "utf8_general_ci")
                        .Annotation("MySql:CharSet", "utf8"),
                    Description = table.Column<string>(type: "mediumtext", nullable: false, collation: "utf8_general_ci")
                        .Annotation("MySql:CharSet", "utf8"),
                    VehicleScale = table.Column<int>(type: "int(11)", nullable: false),
                    ExteriorItemProtoId = table.Column<long>(type: "bigint(20)", nullable: true),
                    ExteriorItemProtoRevision = table.Column<int>(type: "int(11)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => new { x.Id, x.RevisionNumber });
                    table.ForeignKey(
                        name: "FK_VehicleProtos_EditableItems",
                        column: x => x.EditableItemId,
                        principalTable: "EditableItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_VehicleProtos_GameItemProtos",
                        columns: x => new { x.ExteriorItemProtoId, x.ExteriorItemProtoRevision },
                        principalTable: "GameItemProtos",
                        principalColumns: new[] { "Id", "RevisionNumber" },
                        onDelete: ReferentialAction.SetNull);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "VehicleCompartmentProtos",
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
                    DisplayOrder = table.Column<int>(type: "int(11)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.Id);
                    table.ForeignKey(
                        name: "FK_VehicleCompartmentProtos_VehicleProtos",
                        columns: x => new { x.VehicleProtoId, x.VehicleProtoRevision },
                        principalTable: "VehicleProtos",
                        principalColumns: new[] { "Id", "RevisionNumber" },
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "VehicleMovementProfileProtos",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint(20)", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    VehicleProtoId = table.Column<long>(type: "bigint(20)", nullable: false),
                    VehicleProtoRevision = table.Column<int>(type: "int(11)", nullable: false),
                    Name = table.Column<string>(type: "varchar(200)", nullable: false, collation: "utf8_general_ci")
                        .Annotation("MySql:CharSet", "utf8"),
                    MovementType = table.Column<int>(type: "int(11)", nullable: false),
                    IsDefault = table.Column<ulong>(type: "bit(1)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.Id);
                    table.ForeignKey(
                        name: "FK_VehicleMovementProfileProtos_VehicleProtos",
                        columns: x => new { x.VehicleProtoId, x.VehicleProtoRevision },
                        principalTable: "VehicleProtos",
                        principalColumns: new[] { "Id", "RevisionNumber" },
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "VehicleOccupantSlotProtos",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint(20)", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    VehicleProtoId = table.Column<long>(type: "bigint(20)", nullable: false),
                    VehicleProtoRevision = table.Column<int>(type: "int(11)", nullable: false),
                    VehicleCompartmentProtoId = table.Column<long>(type: "bigint(20)", nullable: false),
                    Name = table.Column<string>(type: "varchar(200)", nullable: false, collation: "utf8_general_ci")
                        .Annotation("MySql:CharSet", "utf8"),
                    SlotType = table.Column<int>(type: "int(11)", nullable: false),
                    Capacity = table.Column<int>(type: "int(11)", nullable: false),
                    RequiredForMovement = table.Column<ulong>(type: "bit(1)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.Id);
                    table.ForeignKey(
                        name: "FK_VehicleOccupantSlotProtos_Compartments",
                        column: x => x.VehicleCompartmentProtoId,
                        principalTable: "VehicleCompartmentProtos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_VehicleOccupantSlotProtos_VehicleProtos",
                        columns: x => new { x.VehicleProtoId, x.VehicleProtoRevision },
                        principalTable: "VehicleProtos",
                        principalColumns: new[] { "Id", "RevisionNumber" },
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Vehicles",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint(20)", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    VehicleProtoId = table.Column<long>(type: "bigint(20)", nullable: false),
                    VehicleProtoRevision = table.Column<int>(type: "int(11)", nullable: false),
                    Name = table.Column<string>(type: "varchar(200)", nullable: false, collation: "utf8_general_ci")
                        .Annotation("MySql:CharSet", "utf8"),
                    ExteriorItemId = table.Column<long>(type: "bigint(20)", nullable: true),
                    LocationType = table.Column<int>(type: "int(11)", nullable: false),
                    CurrentCellId = table.Column<long>(type: "bigint(20)", nullable: true),
                    CurrentRoomLayer = table.Column<int>(type: "int(11)", nullable: false),
                    MovementStatus = table.Column<int>(type: "int(11)", nullable: false),
                    CurrentExitId = table.Column<long>(type: "bigint(20)", nullable: true),
                    DestinationCellId = table.Column<long>(type: "bigint(20)", nullable: true),
                    MovementProfileProtoId = table.Column<long>(type: "bigint(20)", nullable: true),
                    CreatedDateTime = table.Column<DateTime>(type: "datetime", nullable: false),
                    LastMovementDateTime = table.Column<DateTime>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Vehicles_Cells_Current",
                        column: x => x.CurrentCellId,
                        principalTable: "Cells",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_Vehicles_Cells_Destination",
                        column: x => x.DestinationCellId,
                        principalTable: "Cells",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_Vehicles_Exits",
                        column: x => x.CurrentExitId,
                        principalTable: "Exits",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_Vehicles_GameItems_Exterior",
                        column: x => x.ExteriorItemId,
                        principalTable: "GameItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_Vehicles_MovementProfileProtos",
                        column: x => x.MovementProfileProtoId,
                        principalTable: "VehicleMovementProfileProtos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_Vehicles_VehicleProtos",
                        columns: x => new { x.VehicleProtoId, x.VehicleProtoRevision },
                        principalTable: "VehicleProtos",
                        principalColumns: new[] { "Id", "RevisionNumber" },
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "VehicleControlStationProtos",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint(20)", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    VehicleProtoId = table.Column<long>(type: "bigint(20)", nullable: false),
                    VehicleProtoRevision = table.Column<int>(type: "int(11)", nullable: false),
                    VehicleOccupantSlotProtoId = table.Column<long>(type: "bigint(20)", nullable: false),
                    Name = table.Column<string>(type: "varchar(200)", nullable: false, collation: "utf8_general_ci")
                        .Annotation("MySql:CharSet", "utf8"),
                    IsPrimary = table.Column<ulong>(type: "bit(1)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.Id);
                    table.ForeignKey(
                        name: "FK_VehicleControlStationProtos_Slots",
                        column: x => x.VehicleOccupantSlotProtoId,
                        principalTable: "VehicleOccupantSlotProtos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_VehicleControlStationProtos_VehicleProtos",
                        columns: x => new { x.VehicleProtoId, x.VehicleProtoRevision },
                        principalTable: "VehicleProtos",
                        principalColumns: new[] { "Id", "RevisionNumber" },
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "VehicleAccessStates",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint(20)", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    VehicleId = table.Column<long>(type: "bigint(20)", nullable: false),
                    CharacterId = table.Column<long>(type: "bigint(20)", nullable: true),
                    AccessTag = table.Column<string>(type: "varchar(200)", nullable: false, collation: "utf8_general_ci")
                        .Annotation("MySql:CharSet", "utf8"),
                    AccessLevel = table.Column<int>(type: "int(11)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.Id);
                    table.ForeignKey(
                        name: "FK_VehicleAccessStates_Characters",
                        column: x => x.CharacterId,
                        principalTable: "Characters",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_VehicleAccessStates_Vehicles",
                        column: x => x.VehicleId,
                        principalTable: "Vehicles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "VehicleCompartments",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint(20)", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    VehicleId = table.Column<long>(type: "bigint(20)", nullable: false),
                    VehicleCompartmentProtoId = table.Column<long>(type: "bigint(20)", nullable: false),
                    Name = table.Column<string>(type: "varchar(200)", nullable: false, collation: "utf8_general_ci")
                        .Annotation("MySql:CharSet", "utf8")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.Id);
                    table.ForeignKey(
                        name: "FK_VehicleCompartments_Protos",
                        column: x => x.VehicleCompartmentProtoId,
                        principalTable: "VehicleCompartmentProtos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_VehicleCompartments_Vehicles",
                        column: x => x.VehicleId,
                        principalTable: "Vehicles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "VehicleOccupancies",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint(20)", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    VehicleId = table.Column<long>(type: "bigint(20)", nullable: false),
                    CharacterId = table.Column<long>(type: "bigint(20)", nullable: false),
                    VehicleOccupantSlotProtoId = table.Column<long>(type: "bigint(20)", nullable: false),
                    IsController = table.Column<ulong>(type: "bit(1)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.Id);
                    table.ForeignKey(
                        name: "FK_VehicleOccupancies_Characters",
                        column: x => x.CharacterId,
                        principalTable: "Characters",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_VehicleOccupancies_Slots",
                        column: x => x.VehicleOccupantSlotProtoId,
                        principalTable: "VehicleOccupantSlotProtos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_VehicleOccupancies_Vehicles",
                        column: x => x.VehicleId,
                        principalTable: "Vehicles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "FK_VehicleAccessStates_Characters_idx",
                table: "VehicleAccessStates",
                column: "CharacterId");

            migrationBuilder.CreateIndex(
                name: "FK_VehicleAccessStates_Vehicles_idx",
                table: "VehicleAccessStates",
                column: "VehicleId");

            migrationBuilder.CreateIndex(
                name: "FK_VehicleCompartmentProtos_VehicleProtos_idx",
                table: "VehicleCompartmentProtos",
                columns: new[] { "VehicleProtoId", "VehicleProtoRevision" });

            migrationBuilder.CreateIndex(
                name: "FK_VehicleCompartments_Protos_idx",
                table: "VehicleCompartments",
                column: "VehicleCompartmentProtoId");

            migrationBuilder.CreateIndex(
                name: "FK_VehicleCompartments_Vehicles_idx",
                table: "VehicleCompartments",
                column: "VehicleId");

            migrationBuilder.CreateIndex(
                name: "FK_VehicleControlStationProtos_Slots_idx",
                table: "VehicleControlStationProtos",
                column: "VehicleOccupantSlotProtoId");

            migrationBuilder.CreateIndex(
                name: "FK_VehicleControlStationProtos_VehicleProtos_idx",
                table: "VehicleControlStationProtos",
                columns: new[] { "VehicleProtoId", "VehicleProtoRevision" });

            migrationBuilder.CreateIndex(
                name: "FK_VehicleMovementProfileProtos_VehicleProtos_idx",
                table: "VehicleMovementProfileProtos",
                columns: new[] { "VehicleProtoId", "VehicleProtoRevision" });

            migrationBuilder.CreateIndex(
                name: "FK_VehicleOccupancies_Characters_idx",
                table: "VehicleOccupancies",
                column: "CharacterId");

            migrationBuilder.CreateIndex(
                name: "FK_VehicleOccupancies_Slots_idx",
                table: "VehicleOccupancies",
                column: "VehicleOccupantSlotProtoId");

            migrationBuilder.CreateIndex(
                name: "FK_VehicleOccupancies_Vehicles_idx",
                table: "VehicleOccupancies",
                column: "VehicleId");

            migrationBuilder.CreateIndex(
                name: "IX_VehicleOccupancies_Vehicle_Character",
                table: "VehicleOccupancies",
                columns: new[] { "VehicleId", "CharacterId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "FK_VehicleOccupantSlotProtos_Compartments_idx",
                table: "VehicleOccupantSlotProtos",
                column: "VehicleCompartmentProtoId");

            migrationBuilder.CreateIndex(
                name: "FK_VehicleOccupantSlotProtos_VehicleProtos_idx",
                table: "VehicleOccupantSlotProtos",
                columns: new[] { "VehicleProtoId", "VehicleProtoRevision" });

            migrationBuilder.CreateIndex(
                name: "FK_VehicleProtos_EditableItems_idx",
                table: "VehicleProtos",
                column: "EditableItemId");

            migrationBuilder.CreateIndex(
                name: "FK_VehicleProtos_GameItemProtos_idx",
                table: "VehicleProtos",
                columns: new[] { "ExteriorItemProtoId", "ExteriorItemProtoRevision" });

            migrationBuilder.CreateIndex(
                name: "FK_Vehicles_Cells_Current_idx",
                table: "Vehicles",
                column: "CurrentCellId");

            migrationBuilder.CreateIndex(
                name: "FK_Vehicles_Cells_Destination_idx",
                table: "Vehicles",
                column: "DestinationCellId");

            migrationBuilder.CreateIndex(
                name: "FK_Vehicles_Exits_idx",
                table: "Vehicles",
                column: "CurrentExitId");

            migrationBuilder.CreateIndex(
                name: "FK_Vehicles_GameItems_Exterior_idx",
                table: "Vehicles",
                column: "ExteriorItemId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "FK_Vehicles_MovementProfileProtos_idx",
                table: "Vehicles",
                column: "MovementProfileProtoId");

            migrationBuilder.CreateIndex(
                name: "FK_Vehicles_VehicleProtos_idx",
                table: "Vehicles",
                columns: new[] { "VehicleProtoId", "VehicleProtoRevision" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "VehicleAccessStates");

            migrationBuilder.DropTable(
                name: "VehicleCompartments");

            migrationBuilder.DropTable(
                name: "VehicleControlStationProtos");

            migrationBuilder.DropTable(
                name: "VehicleOccupancies");

            migrationBuilder.DropTable(
                name: "VehicleOccupantSlotProtos");

            migrationBuilder.DropTable(
                name: "Vehicles");

            migrationBuilder.DropTable(
                name: "VehicleCompartmentProtos");

            migrationBuilder.DropTable(
                name: "VehicleMovementProfileProtos");

            migrationBuilder.DropTable(
                name: "VehicleProtos");
        }
    }
}
