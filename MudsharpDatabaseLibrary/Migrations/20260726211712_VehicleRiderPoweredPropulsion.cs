using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MudSharp.Migrations
{
    /// <inheritdoc />
    public partial class VehicleRiderPoweredPropulsion : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "RiderStaminaMultiplier",
                table: "VehiclePropulsionProfileProtos",
                type: "double",
                nullable: false,
                defaultValue: 1.0);

            migrationBuilder.CreateTable(
                name: "VehicleRiderStaminaModifierProtos",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint(20)", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    VehiclePropulsionProfileProtoId = table.Column<long>(type: "bigint(20)", nullable: false),
                    TerrainId = table.Column<long>(type: "bigint(20)", nullable: true),
                    TerrainTagId = table.Column<long>(type: "bigint(20)", nullable: true),
                    Multiplier = table.Column<double>(type: "double", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.Id);
                    table.CheckConstraint("CK_VehicleRiderStaminaModifierProtos_Multiplier", "`Multiplier` >= 0");
                    table.CheckConstraint("CK_VehicleRiderStaminaModifierProtos_Target", "(`TerrainId` IS NULL AND `TerrainTagId` IS NOT NULL) OR (`TerrainId` IS NOT NULL AND `TerrainTagId` IS NULL)");
                    table.ForeignKey(
                        name: "FK_VehicleRiderStaminaModifierProtos_PropulsionProfiles",
                        column: x => x.VehiclePropulsionProfileProtoId,
                        principalTable: "VehiclePropulsionProfileProtos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_VehicleRiderStaminaModifierProtos_Tags",
                        column: x => x.TerrainTagId,
                        principalTable: "Tags",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_VehicleRiderStaminaModifierProtos_Terrains",
                        column: x => x.TerrainId,
                        principalTable: "Terrains",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddCheckConstraint(
                name: "CK_VehiclePropulsionProfileProtos_RiderStaminaMultiplier",
                table: "VehiclePropulsionProfileProtos",
                sql: "`RiderStaminaMultiplier` >= 0");

            migrationBuilder.CreateIndex(
                name: "FK_VehicleRiderStaminaModifierProtos_PropulsionProfiles_idx",
                table: "VehicleRiderStaminaModifierProtos",
                column: "VehiclePropulsionProfileProtoId");

            migrationBuilder.CreateIndex(
                name: "FK_VehicleRiderStaminaModifierProtos_Tags_idx",
                table: "VehicleRiderStaminaModifierProtos",
                column: "TerrainTagId");

            migrationBuilder.CreateIndex(
                name: "FK_VehicleRiderStaminaModifierProtos_Terrains_idx",
                table: "VehicleRiderStaminaModifierProtos",
                column: "TerrainId");

            migrationBuilder.CreateIndex(
                name: "UX_VehicleRiderStaminaModifierProtos_Profile_Tag",
                table: "VehicleRiderStaminaModifierProtos",
                columns: new[] { "VehiclePropulsionProfileProtoId", "TerrainTagId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "UX_VehicleRiderStaminaModifierProtos_Profile_Terrain",
                table: "VehicleRiderStaminaModifierProtos",
                columns: new[] { "VehiclePropulsionProfileProtoId", "TerrainId" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "VehicleRiderStaminaModifierProtos");

            migrationBuilder.DropCheckConstraint(
                name: "CK_VehiclePropulsionProfileProtos_RiderStaminaMultiplier",
                table: "VehiclePropulsionProfileProtos");

            migrationBuilder.DropColumn(
                name: "RiderStaminaMultiplier",
                table: "VehiclePropulsionProfileProtos");
        }
    }
}
