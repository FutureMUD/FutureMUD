using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MudSharp.Migrations
{
    public partial class NPCSpawners : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "NPCSpawners",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint(20)", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(type: "varchar(255)", nullable: false, collation: "utf8_general_ci")
                        .Annotation("MySql:CharSet", "utf8"),
                    TargetTemplateId = table.Column<long>(type: "bigint(20)", nullable: true),
                    TargetCount = table.Column<int>(type: "int(11)", nullable: false),
                    MinimumCount = table.Column<int>(type: "int(11)", nullable: false),
                    OnSpawnProgId = table.Column<long>(type: "bigint(20)", nullable: true),
                    CountsAsProgId = table.Column<long>(type: "bigint(20)", nullable: true),
                    IsActiveProgId = table.Column<long>(type: "bigint(20)", nullable: true),
                    SpawnStrategy = table.Column<int>(type: "int(11)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NPCSpawners", x => x.Id);
                    table.ForeignKey(
                        name: "FK_NPCSpawners_CountsAsProg",
                        column: x => x.CountsAsProgId,
                        principalTable: "FutureProgs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_NPCSpawners_IsActiveProg",
                        column: x => x.IsActiveProgId,
                        principalTable: "FutureProgs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_NPCSpawners_OnSpawnProg",
                        column: x => x.OnSpawnProgId,
                        principalTable: "FutureProgs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "NPCSpawnerCells",
                columns: table => new
                {
                    NPCSpawnerId = table.Column<long>(type: "bigint(20)", nullable: false),
                    CellId = table.Column<long>(type: "bigint(20)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => new { x.NPCSpawnerId, x.CellId });
                    table.ForeignKey(
                        name: "FK_NPCSpawnerCells_Cell",
                        column: x => x.CellId,
                        principalTable: "Cells",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_NPCSpawnerCells_NPCSpawner",
                        column: x => x.NPCSpawnerId,
                        principalTable: "NPCSpawners",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "NPCSpawnerZones",
                columns: table => new
                {
                    NPCSpawnerId = table.Column<long>(type: "bigint(20)", nullable: false),
                    ZoneId = table.Column<long>(type: "bigint(20)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => new { x.NPCSpawnerId, x.ZoneId });
                    table.ForeignKey(
                        name: "FK_NPCSpawnerZones_NPCSpawner",
                        column: x => x.NPCSpawnerId,
                        principalTable: "NPCSpawners",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_NPCSpawnerZones_Zone",
                        column: x => x.ZoneId,
                        principalTable: "Zones",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_NPCSpawnerCells_CellId",
                table: "NPCSpawnerCells",
                column: "CellId");

            migrationBuilder.CreateIndex(
                name: "IX_NPCSpawners_CountsAsProgId",
                table: "NPCSpawners",
                column: "CountsAsProgId");

            migrationBuilder.CreateIndex(
                name: "IX_NPCSpawners_IsActiveProgId",
                table: "NPCSpawners",
                column: "IsActiveProgId");

            migrationBuilder.CreateIndex(
                name: "IX_NPCSpawners_OnSpawnProgId",
                table: "NPCSpawners",
                column: "OnSpawnProgId");

            migrationBuilder.CreateIndex(
                name: "IX_NPCSpawnerZones_ZoneId",
                table: "NPCSpawnerZones",
                column: "ZoneId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "NPCSpawnerCells");

            migrationBuilder.DropTable(
                name: "NPCSpawnerZones");

            migrationBuilder.DropTable(
                name: "NPCSpawners");
        }
    }
}
