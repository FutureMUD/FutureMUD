using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MudSharp.Migrations
{
    /// <inheritdoc />
    public partial class VehicleSurfaceWaterPropulsion : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "ActivePropulsionProfileProtoId",
                table: "Vehicles",
                type: "bigint(20)",
                nullable: true);

            migrationBuilder.AddColumn<ulong>(
                name: "ContributesToPropulsion",
                table: "VehicleOccupantSlotProtos",
                type: "bit(1)",
                nullable: false,
                defaultValue: 0ul);

            migrationBuilder.CreateTable(
                name: "VehiclePropulsionProfileProtos",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint(20)", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    VehicleMovementProfileProtoId = table.Column<long>(type: "bigint(20)", nullable: false),
                    PropulsionType = table.Column<int>(type: "int(11)", nullable: false),
                    IsDefault = table.Column<ulong>(type: "bit(1)", nullable: false),
                    BaseMoveTimeMilliseconds = table.Column<double>(type: "double", nullable: false),
                    PropulsionTraitDefinitionId = table.Column<long>(type: "bigint(20)", nullable: true),
                    CheckDifficulty = table.Column<int>(type: "int(11)", nullable: false),
                    SpeedMultiplierExpression = table.Column<string>(type: "varchar(1000)", nullable: false, collation: "utf8_general_ci")
                        .Annotation("MySql:CharSet", "utf8"),
                    StaminaCostExpression = table.Column<string>(type: "varchar(1000)", nullable: false, collation: "utf8_general_ci")
                        .Annotation("MySql:CharSet", "utf8")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.Id);
                    table.ForeignKey(
                        name: "FK_VehiclePropulsionProfileProtos_MovementProfiles",
                        column: x => x.VehicleMovementProfileProtoId,
                        principalTable: "VehicleMovementProfileProtos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_VehiclePropulsionProfileProtos_Traits",
                        column: x => x.PropulsionTraitDefinitionId,
                        principalTable: "TraitDefinitions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "FK_Vehicles_PropulsionProfileProtos_idx",
                table: "Vehicles",
                column: "ActivePropulsionProfileProtoId");

            migrationBuilder.CreateIndex(
                name: "FK_VehiclePropulsionProfileProtos_MovementProfiles_idx",
                table: "VehiclePropulsionProfileProtos",
                column: "VehicleMovementProfileProtoId");

            migrationBuilder.CreateIndex(
                name: "FK_VehiclePropulsionProfileProtos_Traits_idx",
                table: "VehiclePropulsionProfileProtos",
                column: "PropulsionTraitDefinitionId");

            migrationBuilder.CreateIndex(
                name: "UX_VehiclePropulsionProfileProtos_Profile_Type",
                table: "VehiclePropulsionProfileProtos",
                columns: new[] { "VehicleMovementProfileProtoId", "PropulsionType" },
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Vehicles_PropulsionProfileProtos",
                table: "Vehicles",
                column: "ActivePropulsionProfileProtoId",
                principalTable: "VehiclePropulsionProfileProtos",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Vehicles_PropulsionProfileProtos",
                table: "Vehicles");

            migrationBuilder.DropTable(
                name: "VehiclePropulsionProfileProtos");

            migrationBuilder.DropIndex(
                name: "FK_Vehicles_PropulsionProfileProtos_idx",
                table: "Vehicles");

            migrationBuilder.DropColumn(
                name: "ActivePropulsionProfileProtoId",
                table: "Vehicles");

            migrationBuilder.DropColumn(
                name: "ContributesToPropulsion",
                table: "VehicleOccupantSlotProtos");
        }
    }
}
