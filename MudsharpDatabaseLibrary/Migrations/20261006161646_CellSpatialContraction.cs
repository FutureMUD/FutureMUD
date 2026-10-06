using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MudSharp.Migrations
{
    /// <inheritdoc />
    public partial class CellSpatialContraction : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
			// MySQL DDL is not transactional: guard, snapshot/copy, assert, then destructive DDL.
			migrationBuilder.Sql("""
CREATE PROCEDURE `fm_cell_spatial_contract_20261006161646`(IN after_copy BOOL)
BEGIN
 DECLARE finished BOOL DEFAULT FALSE;
 DECLARE table_name_value VARCHAR(64);
 DECLARE column_name_value VARCHAR(64);
 DECLARE diagnostic VARCHAR(128);
 DECLARE object_definition LONGTEXT;
 DECLARE candidate_definitions CURSOR FOR
  SELECT 'view', TABLE_NAME, VIEW_DEFINITION FROM information_schema.VIEWS WHERE TABLE_SCHEMA=DATABASE()
  UNION ALL SELECT 'routine', ROUTINE_NAME, ROUTINE_DEFINITION FROM information_schema.ROUTINES
   WHERE ROUTINE_SCHEMA=DATABASE() AND ROUTINE_NAME <> 'fm_cell_spatial_contract_20261006161646'
  UNION ALL SELECT 'trigger', TRIGGER_NAME, ACTION_STATEMENT FROM information_schema.TRIGGERS WHERE TRIGGER_SCHEMA=DATABASE()
  UNION ALL SELECT 'event', EVENT_NAME, EVENT_DEFINITION FROM information_schema.EVENTS WHERE EVENT_SCHEMA=DATABASE();
 DECLARE candidate_columns CURSOR FOR
  SELECT TABLE_NAME, COLUMN_NAME FROM information_schema.COLUMNS
  WHERE TABLE_SCHEMA=DATABASE() AND DATA_TYPE IN ('char','varchar','tinytext','text','mediumtext','longtext','json');
 DECLARE CONTINUE HANDLER FOR NOT FOUND SET finished=TRUE;
 IF EXISTS(SELECT 1 FROM `Rooms`) AND COALESCE(@FutureMUD_CellSpatialContractionMaintenance,0) <> 1 THEN
  SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT='Cell spatial contraction requires frozen writers, verified backup and maintenance session opt-in';
 END IF;
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
 IF EXISTS(SELECT 1 FROM `Areas_Rooms` a LEFT JOIN `Cells` c ON c.RoomId=a.RoomId WHERE c.Id IS NULL) THEN
  SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT='Cell spatial preflight: Area references an empty Room; explicit disposition required';
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
           AND LOWER(TABLE_NAME) NOT IN ('cells','areas_rooms','cellroommigrationledger','cellroomareamigrationledger','cellroomcontractionledger','cellroomareacontractionledger')) THEN
  SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT='Cell spatial preflight: unclassified RoomId column';
 END IF;
 IF EXISTS(SELECT 1 FROM Zones z LEFT JOIN Cells c ON c.Id=z.DefaultCellId
           WHERE z.DefaultCellId IS NOT NULL AND (c.Id IS NULL OR NOT(c.RoomId IN (SELECT Id FROM Rooms WHERE ZoneId=z.Id)))) THEN
  SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT='Cell spatial contraction: invalid owning-zone default Cell reference';
 END IF;
 IF EXISTS(SELECT 1 FROM Areas_Cells a LEFT JOIN Areas ar ON ar.Id=a.AreaId LEFT JOIN Cells c ON c.Id=a.CellId WHERE ar.Id IS NULL OR c.Id IS NULL)
    OR EXISTS(SELECT 1 FROM Cells c LEFT JOIN Zones z ON z.Id=c.ZoneId WHERE c.ZoneId IS NOT NULL AND z.Id IS NULL) THEN
  SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT='Cell spatial contraction: orphan direct-cell afterimage';
 END IF;
 IF EXISTS(SELECT 1 FROM information_schema.KEY_COLUMN_USAGE WHERE REFERENCED_TABLE_SCHEMA=DATABASE()
           AND (LOWER(REFERENCED_TABLE_NAME)='areas_rooms' OR (LOWER(REFERENCED_TABLE_NAME)='cells' AND LOWER(REFERENCED_COLUMN_NAME)='roomid'))) THEN
  SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT='Cell spatial contraction: unknown referent to legacy membership or Cell.RoomId';
 END IF;
 OPEN candidate_definitions;
 scan_definitions: LOOP
  FETCH candidate_definitions INTO column_name_value,table_name_value,object_definition;
  IF finished THEN LEAVE scan_definitions; END IF;
  IF object_definition IS NULL OR REGEXP_LIKE(object_definition,'(^|[^[:alnum:]_])(rooms|areas_rooms|roomid)([^[:alnum:]_]|$)','i') THEN
   SET diagnostic=LEFT(CONCAT('Cell contraction: unclassified or unreadable ',column_name_value,' ',table_name_value,'; review required'),128);
   SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT=diagnostic;
  END IF;
 END LOOP;
 CLOSE candidate_definitions;
 SET finished=FALSE;
 IF EXISTS(SELECT 1 FROM Cells c JOIN Rooms r ON r.Id=c.RoomId
           WHERE NOT(c.ZoneId<=>r.ZoneId) OR NOT(c.X<=>r.X) OR NOT(c.Y<=>r.Y) OR NOT(c.Z<=>r.Z))
    OR EXISTS(SELECT 1 FROM Areas_Rooms a JOIN Cells c ON c.RoomId=a.RoomId LEFT JOIN Areas_Cells n ON n.AreaId=a.AreaId AND n.CellId=c.Id WHERE n.CellId IS NULL)
    OR EXISTS(SELECT 1 FROM Areas_Cells n LEFT JOIN Cells c ON c.Id=n.CellId LEFT JOIN Areas_Rooms a ON a.AreaId=n.AreaId AND a.RoomId=c.RoomId WHERE a.RoomId IS NULL) THEN
  IF after_copy OR COALESCE(@FutureMUD_CellSpatialReconcile,0) <> 1 THEN
   SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT='Cell contraction: stale spatial afterimage; explicit frozen-session reconciliation required';
  END IF;
  IF EXISTS(SELECT 1 FROM information_schema.TRIGGERS WHERE TRIGGER_SCHEMA=DATABASE() AND LOWER(EVENT_OBJECT_TABLE) IN ('cells','areas_cells'))
     OR EXISTS(SELECT 1 FROM information_schema.KEY_COLUMN_USAGE WHERE REFERENCED_TABLE_SCHEMA=DATABASE() AND LOWER(REFERENCED_TABLE_NAME)='areas_cells') THEN
   SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT='Cell contraction: reconciliation has unclassified trigger or Area-Cell dependents';
  END IF;
 END IF;
 IF after_copy THEN
  IF (SELECT COUNT(*) FROM CellRoomContractionLedger)<>(SELECT COUNT(*) FROM Rooms)
     OR EXISTS(SELECT 1 FROM Rooms r LEFT JOIN Cells c ON c.RoomId=r.Id LEFT JOIN CellRoomContractionLedger m ON m.RoomId=r.Id
               WHERE m.RoomId IS NULL OR NOT(m.CellId<=>c.Id) OR m.ZoneId<>r.ZoneId OR m.X<>r.X OR m.Y<>r.Y OR m.Z<>r.Z
                     OR (c.Id IS NULL AND (m.Warning IS NULL OR m.Warning='')))
     OR (SELECT COUNT(*) FROM CellRoomAreaContractionLedger)<>(SELECT COUNT(*) FROM Areas_Rooms)
     OR EXISTS(SELECT 1 FROM Areas_Rooms a JOIN Cells c ON c.RoomId=a.RoomId LEFT JOIN CellRoomAreaContractionLedger m ON m.AreaId=a.AreaId AND m.RoomId=a.RoomId AND m.CellId=c.Id WHERE m.CellId IS NULL) THEN
   SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT='Cell contraction: final provenance or complete mapping invariant failed; no Room data may be dropped';
  END IF;
 END IF;
 OPEN candidate_columns;
 scan_columns: LOOP
  FETCH candidate_columns INTO table_name_value,column_name_value;
  IF finished THEN LEAVE scan_columns; END IF;
  SET @fm_cell_spatial_hits=0;
  IF LOWER(column_name_value) LIKE '%type' THEN
   SET @fm_cell_spatial_pattern='^[[:space:]]*Room[[:space:]]*$';
  ELSE
   SET @fm_cell_spatial_pattern='([[:alnum:]_]*Type["'']?[[:space:]]*[:=][[:space:]]*["'']Room["'']|<([[:alnum:]_]*Type)>[[:space:]]*Room[[:space:]]*</)';
  END IF;
  SET @fm_cell_spatial_query=CONCAT('SELECT COUNT(*) INTO @fm_cell_spatial_hits FROM `',REPLACE(table_name_value,'`','``'),
   '` WHERE REGEXP_LIKE(`',REPLACE(column_name_value,'`','``'),'`, ?, ''i'')');
  PREPARE fm_cell_spatial_statement FROM @fm_cell_spatial_query;
  EXECUTE fm_cell_spatial_statement USING @fm_cell_spatial_pattern;
  DEALLOCATE PREPARE fm_cell_spatial_statement;
  IF @fm_cell_spatial_hits>0 THEN
   SET diagnostic=LEFT(CONCAT('Cell spatial preflight: Room reference in ',table_name_value,'.',column_name_value,'; review required'),128);
   SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT=diagnostic;
  END IF;
 END LOOP;
 CLOSE candidate_columns;
END;
""", suppressTransaction: true);
			migrationBuilder.Sql("CALL `fm_cell_spatial_contract_20261006161646`(FALSE);", suppressTransaction: true);
            migrationBuilder.CreateTable(
                name: "CellRoomAreaContractionLedger",
                columns: table => new
                {
                    AreaId = table.Column<long>(type: "bigint(20)", nullable: false),
                    RoomId = table.Column<long>(type: "bigint(20)", nullable: false),
                    CellId = table.Column<long>(type: "bigint(20)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CellRoomAreaContractionLedger", x => new { x.AreaId, x.RoomId });
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "CellRoomContractionLedger",
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
                    table.PrimaryKey("PK_CellRoomContractionLedger", x => x.RoomId);
                })
                .Annotation("MySql:CharSet", "utf8mb4");
			migrationBuilder.Sql("""
START TRANSACTION;
INSERT INTO CellRoomContractionLedger(RoomId,CellId,ZoneId,X,Y,Z,Warning)
SELECT r.Id,c.Id,r.ZoneId,r.X,r.Y,r.Z,
       CASE WHEN c.Id IS NULL THEN 'Unreferenced empty Room removed at contraction; final original metadata retained here' ELSE NULL END
FROM Rooms r LEFT JOIN Cells c ON c.RoomId=r.Id;
INSERT INTO CellRoomAreaContractionLedger(AreaId,RoomId,CellId)
SELECT a.AreaId,a.RoomId,c.Id FROM Areas_Rooms a JOIN Cells c ON c.RoomId=a.RoomId;
UPDATE Cells c JOIN Rooms r ON r.Id=c.RoomId SET c.ZoneId=r.ZoneId,c.X=r.X,c.Y=r.Y,c.Z=r.Z
WHERE NOT(c.ZoneId<=>r.ZoneId) OR NOT(c.X<=>r.X) OR NOT(c.Y<=>r.Y) OR NOT(c.Z<=>r.Z);
DELETE n FROM Areas_Cells n LEFT JOIN Cells c ON c.Id=n.CellId LEFT JOIN Areas_Rooms a ON a.AreaId=n.AreaId AND a.RoomId=c.RoomId WHERE a.RoomId IS NULL;
INSERT INTO Areas_Cells(AreaId,CellId)
SELECT a.AreaId,c.Id FROM Areas_Rooms a JOIN Cells c ON c.RoomId=a.RoomId
LEFT JOIN Areas_Cells n ON n.AreaId=a.AreaId AND n.CellId=c.Id WHERE n.CellId IS NULL;
COMMIT;
""", suppressTransaction: true);
			migrationBuilder.Sql("CALL `fm_cell_spatial_contract_20261006161646`(TRUE);", suppressTransaction: true);
			migrationBuilder.Sql("DROP PROCEDURE `fm_cell_spatial_contract_20261006161646`;", suppressTransaction: true);
            migrationBuilder.DropForeignKey(
                name: "FK_Cells_Rooms",
                table: "Cells");

            migrationBuilder.DropTable(
                name: "Areas_Rooms");

            migrationBuilder.DropTable(
                name: "Rooms");

            migrationBuilder.DropIndex(
                name: "FK_Cells_Rooms",
                table: "Cells");

            migrationBuilder.DropColumn(
                name: "RoomId",
                table: "Cells");

            migrationBuilder.AlterColumn<long>(
                name: "ZoneId",
                table: "Cells",
                type: "bigint(20)",
                nullable: false,
                defaultValue: 0L,
                oldClrType: typeof(long),
                oldType: "bigint(20)",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "Z",
                table: "Cells",
                type: "int(11)",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int(11)",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "Y",
                table: "Cells",
                type: "int(11)",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int(11)",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "X",
                table: "Cells",
                type: "int(11)",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int(11)",
                oldNullable: true);

        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            throw new System.NotSupportedException("Room contraction has no lossless automatic downgrade. Keep writers stopped and restore the verified pre-cutover database with its matching binary and external files.");
        }
    }
}
