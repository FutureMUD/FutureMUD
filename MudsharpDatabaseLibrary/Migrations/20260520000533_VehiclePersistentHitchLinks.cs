using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MudSharp.Migrations
{
    /// <inheritdoc />
    public partial class VehiclePersistentHitchLinks : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "VehicleHitchLinks",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint(20)", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    SourceType = table.Column<int>(type: "int(11)", nullable: false),
                    SourceVehicleId = table.Column<long>(type: "bigint(20)", nullable: true),
                    SourceCharacterId = table.Column<long>(type: "bigint(20)", nullable: true),
                    SourceTowPointProtoId = table.Column<long>(type: "bigint(20)", nullable: true),
                    TargetType = table.Column<int>(type: "int(11)", nullable: false),
                    TargetVehicleId = table.Column<long>(type: "bigint(20)", nullable: true),
                    TargetCharacterId = table.Column<long>(type: "bigint(20)", nullable: true),
                    TargetTowPointProtoId = table.Column<long>(type: "bigint(20)", nullable: true),
                    HitchItemId = table.Column<long>(type: "bigint(20)", nullable: true),
                    IsDisabled = table.Column<ulong>(type: "bit(1)", nullable: false),
                    CreatedDateTime = table.Column<DateTime>(type: "datetime", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.Id);
                    table.ForeignKey(
                        name: "FK_VehicleHitchLinks_GameItems",
                        column: x => x.HitchItemId,
                        principalTable: "GameItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_VehicleHitchLinks_SourceCharacters",
                        column: x => x.SourceCharacterId,
                        principalTable: "Characters",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_VehicleHitchLinks_SourceTowPointProtos",
                        column: x => x.SourceTowPointProtoId,
                        principalTable: "VehicleTowPointProtos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_VehicleHitchLinks_SourceVehicles",
                        column: x => x.SourceVehicleId,
                        principalTable: "Vehicles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_VehicleHitchLinks_TargetCharacters",
                        column: x => x.TargetCharacterId,
                        principalTable: "Characters",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_VehicleHitchLinks_TargetTowPointProtos",
                        column: x => x.TargetTowPointProtoId,
                        principalTable: "VehicleTowPointProtos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_VehicleHitchLinks_TargetVehicles",
                        column: x => x.TargetVehicleId,
                        principalTable: "Vehicles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "FK_VehicleHitchLinks_GameItems_idx",
                table: "VehicleHitchLinks",
                column: "HitchItemId");

            migrationBuilder.CreateIndex(
                name: "FK_VehicleHitchLinks_SourceCharacters_idx",
                table: "VehicleHitchLinks",
                column: "SourceCharacterId");

            migrationBuilder.CreateIndex(
                name: "FK_VehicleHitchLinks_SourceTowPointProtos_idx",
                table: "VehicleHitchLinks",
                column: "SourceTowPointProtoId");

            migrationBuilder.CreateIndex(
                name: "FK_VehicleHitchLinks_SourceVehicles_idx",
                table: "VehicleHitchLinks",
                column: "SourceVehicleId");

            migrationBuilder.CreateIndex(
                name: "FK_VehicleHitchLinks_TargetCharacters_idx",
                table: "VehicleHitchLinks",
                column: "TargetCharacterId");

            migrationBuilder.CreateIndex(
                name: "FK_VehicleHitchLinks_TargetTowPointProtos_idx",
                table: "VehicleHitchLinks",
                column: "TargetTowPointProtoId");

            migrationBuilder.CreateIndex(
                name: "FK_VehicleHitchLinks_TargetVehicles_idx",
                table: "VehicleHitchLinks",
                column: "TargetVehicleId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "VehicleHitchLinks");
        }
    }
}
