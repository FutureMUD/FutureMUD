using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MudSharp.Migrations
{
    /// <inheritdoc />
    public partial class CellSpatialExpansion : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
			// MySQL DDL commits implicitly. Refuse unsafe worlds before the first table change.
			// Populated worlds require an explicit maintenance-session opt-in; normal startup cannot cut over.
			// A preexisting object at this name must be inspected; never replace an unknown routine.
			migrationBuilder.Sql("""
CREATE PROCEDURE `fm_cell_spatial_preflight_20261006143539`()
BEGIN
 DECLARE finished BOOL DEFAULT FALSE;
 DECLARE table_name_value VARCHAR(64);
 DECLARE column_name_value VARCHAR(64);
 DECLARE diagnostic VARCHAR(128);
 DECLARE CONTINUE HANDLER FOR NOT FOUND SET finished=TRUE;
 IF EXISTS(SELECT 1 FROM `Cells` GROUP BY RoomId HAVING COUNT(*)>1) THEN
  SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT='Cell spatial preflight: Room has multiple Cells; no child will be chosen';
 END IF;
 IF EXISTS(SELECT 1 FROM `Cells` c LEFT JOIN `Rooms` r ON r.Id=c.RoomId WHERE r.Id IS NULL) THEN
  SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT='Cell spatial preflight: orphan Cell.RoomId';
 END IF;
 IF EXISTS(SELECT 1 FROM `Rooms` r LEFT JOIN `Zones` z ON z.Id=r.ZoneId WHERE z.Id IS NULL) THEN
  SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT='Cell spatial preflight: orphan Room.ZoneId';
 END IF;
 IF EXISTS(SELECT 1 FROM `Zones` z LEFT JOIN `Shards` s ON s.Id=z.ShardId WHERE s.Id IS NULL) THEN
  SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT='Cell spatial preflight: orphan Zone.ShardId';
 END IF;
 IF EXISTS(SELECT 1 FROM `Areas_Rooms` a LEFT JOIN `Rooms` r ON r.Id=a.RoomId LEFT JOIN `Areas` ar ON ar.Id=a.AreaId
           WHERE r.Id IS NULL OR ar.Id IS NULL) THEN
  SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT='Cell spatial preflight: orphan Area membership';
 END IF;
 IF EXISTS(SELECT 1 FROM `Cells` c LEFT JOIN `CellOverlays` o ON o.Id=c.CurrentOverlayId
           WHERE o.Id IS NULL OR o.CellId<>c.Id) THEN
  SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT='Cell spatial preflight: missing or foreign current overlay';
 END IF;
 IF EXISTS(SELECT 1 FROM information_schema.KEY_COLUMN_USAGE
           WHERE REFERENCED_TABLE_SCHEMA=DATABASE() AND LOWER(REFERENCED_TABLE_NAME)='rooms'
           AND NOT(LOWER(TABLE_NAME) IN ('cells','areas_rooms') AND LOWER(COLUMN_NAME)='roomid'
                   AND LOWER(REFERENCED_COLUMN_NAME)='id')) THEN
  SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT='Cell spatial preflight: unknown foreign key to Rooms; no reference will be retargeted';
 END IF;
 IF EXISTS(SELECT 1 FROM information_schema.COLUMNS WHERE TABLE_SCHEMA=DATABASE() AND LOWER(COLUMN_NAME)='roomid'
           AND LOWER(TABLE_NAME) NOT IN ('cells','areas_rooms','aistorytellersituations')) THEN
  SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT='Cell spatial preflight: unclassified RoomId column';
 END IF;
END;
""", suppressTransaction: true);
			migrationBuilder.Sql("CALL `fm_cell_spatial_preflight_20261006143539`();", suppressTransaction: true);
			migrationBuilder.Sql("DROP PROCEDURE `fm_cell_spatial_preflight_20261006143539`;", suppressTransaction: true);
			migrationBuilder.Sql(MudSharp.Database.RoomReferenceMigrationInterceptor.Expansion, suppressTransaction: true);
            migrationBuilder.AddColumn<int>(
                name: "X",
                table: "Cells",
                type: "int(11)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Y",
                table: "Cells",
                type: "int(11)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Z",
                table: "Cells",
                type: "int(11)",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "ZoneId",
                table: "Cells",
                type: "bigint(20)",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Areas_Cells",
                columns: table => new
                {
                    AreaId = table.Column<long>(type: "bigint(20)", nullable: false),
                    CellId = table.Column<long>(type: "bigint(20)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Areas_Cells", x => new { x.AreaId, x.CellId });
                    table.ForeignKey(
                        name: "FK_Areas_Cells_Areas",
                        column: x => x.AreaId,
                        principalTable: "Areas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Areas_Cells_Cells",
                        column: x => x.CellId,
                        principalTable: "Cells",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "CellRoomAreaMigrationLedger",
                columns: table => new
                {
                    AreaId = table.Column<long>(type: "bigint(20)", nullable: false),
                    RoomId = table.Column<long>(type: "bigint(20)", nullable: false),
                    CellId = table.Column<long>(type: "bigint(20)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CellRoomAreaMigrationLedger", x => new { x.AreaId, x.RoomId });
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "CellRoomMigrationLedger",
                columns: table => new
                {
                    RoomId = table.Column<long>(type: "bigint(20)", nullable: false),
                    CellId = table.Column<long>(type: "bigint(20)", nullable: true),
                    ZoneId = table.Column<long>(type: "bigint(20)", nullable: false),
                    X = table.Column<int>(type: "int(11)", nullable: false),
                    Y = table.Column<int>(type: "int(11)", nullable: false),
                    Z = table.Column<int>(type: "int(11)", nullable: false),
                    Warning = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: true, collation: "utf8mb4_general_ci")
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CellRoomMigrationLedger", x => x.RoomId);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_Cells_ZoneId",
                table: "Cells",
                column: "ZoneId");

            migrationBuilder.CreateIndex(
                name: "IX_Areas_Cells_CellId",
                table: "Areas_Cells",
                column: "CellId");

            migrationBuilder.AddForeignKey(
                name: "FK_Cells_OwningZone",
                table: "Cells",
                column: "ZoneId",
                principalTable: "Zones",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

			// All old tables/columns remain intact. This is a frozen-writer snapshot, not dual-write synchronisation.
			migrationBuilder.Sql("""
START TRANSACTION;
INSERT INTO `CellRoomMigrationLedger`(RoomId,CellId,ZoneId,X,Y,Z,Warning)
SELECT r.Id,c.Id,r.ZoneId,r.X,r.Y,r.Z,
       CASE WHEN c.Id IS NULL THEN 'Empty Room: removal permitted; original metadata and discarded Area memberships retained in ledgers' ELSE NULL END
FROM `Rooms` r LEFT JOIN `Cells` c ON c.RoomId=r.Id;
INSERT INTO `CellRoomAreaMigrationLedger`(AreaId,RoomId,CellId)
SELECT a.AreaId,a.RoomId,c.Id FROM `Areas_Rooms` a LEFT JOIN `Cells` c ON c.RoomId=a.RoomId;
UPDATE `Cells` c JOIN `CellRoomMigrationLedger` m ON m.CellId=c.Id
SET c.ZoneId=m.ZoneId,c.X=m.X,c.Y=m.Y,c.Z=m.Z;
INSERT INTO `Areas_Cells`(AreaId,CellId)
SELECT AreaId,CellId FROM `CellRoomAreaMigrationLedger` WHERE CellId IS NOT NULL;
COMMIT;
""", suppressTransaction: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Cells_OwningZone",
                table: "Cells");

            migrationBuilder.DropTable(
                name: "Areas_Cells");

            migrationBuilder.DropTable(
                name: "CellRoomAreaMigrationLedger");

            migrationBuilder.DropTable(
                name: "CellRoomMigrationLedger");

            migrationBuilder.DropIndex(
                name: "IX_Cells_ZoneId",
                table: "Cells");

            migrationBuilder.DropColumn(
                name: "X",
                table: "Cells");

            migrationBuilder.DropColumn(
                name: "Y",
                table: "Cells");

            migrationBuilder.DropColumn(
                name: "Z",
                table: "Cells");

            migrationBuilder.DropColumn(
                name: "ZoneId",
                table: "Cells");
        }
    }
}
