using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MudSharp.Migrations;

// EF-scaffolded designer/snapshot retained. The generated drop/create body was
// reviewed against both relational models and replaced with explicit renames.
// MySQL DDL commits implicitly: a failed cutover requires verified backup recovery.
public partial class RoomTerminology : Migration
{
 protected override void Up(MigrationBuilder migrationBuilder)
 {
  migrationBuilder.Sql("""
CREATE PROCEDURE `fm_room_terminology_20261007043900`(IN after_rename BOOL)
BEGIN
 DECLARE finished BOOL DEFAULT FALSE;
 DECLARE kind_value VARCHAR(16);
 DECLARE name_value VARCHAR(64);
 DECLARE definition_value LONGTEXT;
 DECLARE diagnostic VARCHAR(128);
 DECLARE definitions CURSOR FOR
  SELECT 'view',TABLE_NAME,VIEW_DEFINITION FROM information_schema.VIEWS WHERE TABLE_SCHEMA=DATABASE()
  UNION ALL SELECT 'routine',ROUTINE_NAME,ROUTINE_DEFINITION FROM information_schema.ROUTINES
   WHERE ROUTINE_SCHEMA=DATABASE() AND ROUTINE_NAME<>'fm_room_terminology_20261007043900'
  UNION ALL SELECT 'trigger',TRIGGER_NAME,ACTION_STATEMENT FROM information_schema.TRIGGERS WHERE TRIGGER_SCHEMA=DATABASE()
  UNION ALL SELECT 'event',EVENT_NAME,EVENT_DEFINITION FROM information_schema.EVENTS WHERE EVENT_SCHEMA=DATABASE();
 DECLARE CONTINUE HANDLER FOR NOT FOUND SET finished=TRUE;
 IF NOT after_rename THEN
  IF NOT EXISTS(SELECT 1 FROM information_schema.TABLES WHERE TABLE_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='cells' AND TABLE_TYPE='BASE TABLE') THEN
   SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT='Room naming: contracted source Cells table is absent; inspect schema/history and restore partial cutover';
  END IF;
  IF EXISTS(SELECT 1 FROM information_schema.COLUMNS WHERE TABLE_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='cells' AND LOWER(COLUMN_NAME)='roomid') THEN
   SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT='Room naming: legacy grouping still exists; complete contraction first';
  END IF;
  IF NOT EXISTS(SELECT 1 FROM information_schema.TABLES WHERE TABLE_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='areas_cells' AND TABLE_TYPE='BASE TABLE') OR EXISTS(SELECT 1 FROM information_schema.TABLES WHERE TABLE_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='areas_rooms') THEN
   SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT='Room naming: missing source or colliding target Areas_Cells';
  END IF;
  IF NOT EXISTS(SELECT 1 FROM information_schema.TABLES WHERE TABLE_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='arenacells' AND TABLE_TYPE='BASE TABLE') OR EXISTS(SELECT 1 FROM information_schema.TABLES WHERE TABLE_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='arenarooms') THEN
   SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT='Room naming: missing source or colliding target ArenaCells';
  END IF;
  IF NOT EXISTS(SELECT 1 FROM information_schema.TABLES WHERE TABLE_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='cellenvironmentalstates' AND TABLE_TYPE='BASE TABLE') OR EXISTS(SELECT 1 FROM information_schema.TABLES WHERE TABLE_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='roomenvironmentalstates') THEN
   SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT='Room naming: missing source or colliding target CellEnvironmentalStates';
  END IF;
  IF NOT EXISTS(SELECT 1 FROM information_schema.TABLES WHERE TABLE_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='celloverlaypackages' AND TABLE_TYPE='BASE TABLE') OR EXISTS(SELECT 1 FROM information_schema.TABLES WHERE TABLE_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='roomoverlaypackages') THEN
   SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT='Room naming: missing source or colliding target CellOverlayPackages';
  END IF;
  IF NOT EXISTS(SELECT 1 FROM information_schema.TABLES WHERE TABLE_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='celloverlays' AND TABLE_TYPE='BASE TABLE') OR EXISTS(SELECT 1 FROM information_schema.TABLES WHERE TABLE_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='roomoverlays') THEN
   SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT='Room naming: missing source or colliding target CellOverlays';
  END IF;
  IF NOT EXISTS(SELECT 1 FROM information_schema.TABLES WHERE TABLE_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='celloverlays_exits' AND TABLE_TYPE='BASE TABLE') OR EXISTS(SELECT 1 FROM information_schema.TABLES WHERE TABLE_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='roomoverlays_exits') THEN
   SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT='Room naming: missing source or colliding target CellOverlays_Exits';
  END IF;
  IF NOT EXISTS(SELECT 1 FROM information_schema.TABLES WHERE TABLE_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='cellroomareacontractionledger' AND TABLE_TYPE='BASE TABLE') OR EXISTS(SELECT 1 FROM information_schema.TABLES WHERE TABLE_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='roomspatialareacontractionledger') THEN
   SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT='Room naming: missing source or colliding target CellRoomAreaContractionLedger';
  END IF;
  IF NOT EXISTS(SELECT 1 FROM information_schema.TABLES WHERE TABLE_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='cellroomareamigrationledger' AND TABLE_TYPE='BASE TABLE') OR EXISTS(SELECT 1 FROM information_schema.TABLES WHERE TABLE_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='roomspatialareamigrationledger') THEN
   SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT='Room naming: missing source or colliding target CellRoomAreaMigrationLedger';
  END IF;
  IF NOT EXISTS(SELECT 1 FROM information_schema.TABLES WHERE TABLE_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='cellroomcontractionledger' AND TABLE_TYPE='BASE TABLE') OR EXISTS(SELECT 1 FROM information_schema.TABLES WHERE TABLE_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='roomspatialcontractionledger') THEN
   SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT='Room naming: missing source or colliding target CellRoomContractionLedger';
  END IF;
  IF NOT EXISTS(SELECT 1 FROM information_schema.TABLES WHERE TABLE_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='cellroommigrationledger' AND TABLE_TYPE='BASE TABLE') OR EXISTS(SELECT 1 FROM information_schema.TABLES WHERE TABLE_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='roomspatialmigrationledger') THEN
   SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT='Room naming: missing source or colliding target CellRoomMigrationLedger';
  END IF;
  IF NOT EXISTS(SELECT 1 FROM information_schema.TABLES WHERE TABLE_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='cells' AND TABLE_TYPE='BASE TABLE') OR EXISTS(SELECT 1 FROM information_schema.TABLES WHERE TABLE_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='rooms') THEN
   SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT='Room naming: missing source or colliding target Cells';
  END IF;
  IF NOT EXISTS(SELECT 1 FROM information_schema.TABLES WHERE TABLE_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='cells_foragableyields' AND TABLE_TYPE='BASE TABLE') OR EXISTS(SELECT 1 FROM information_schema.TABLES WHERE TABLE_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='rooms_foragableyields') THEN
   SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT='Room naming: missing source or colliding target Cells_ForagableYields';
  END IF;
  IF NOT EXISTS(SELECT 1 FROM information_schema.TABLES WHERE TABLE_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='cells_gameitems' AND TABLE_TYPE='BASE TABLE') OR EXISTS(SELECT 1 FROM information_schema.TABLES WHERE TABLE_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='rooms_gameitems') THEN
   SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT='Room naming: missing source or colliding target Cells_GameItems';
  END IF;
  IF NOT EXISTS(SELECT 1 FROM information_schema.TABLES WHERE TABLE_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='cells_magicresources' AND TABLE_TYPE='BASE TABLE') OR EXISTS(SELECT 1 FROM information_schema.TABLES WHERE TABLE_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='rooms_magicresources') THEN
   SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT='Room naming: missing source or colliding target Cells_MagicResources';
  END IF;
  IF NOT EXISTS(SELECT 1 FROM information_schema.TABLES WHERE TABLE_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='cells_rangedcovers' AND TABLE_TYPE='BASE TABLE') OR EXISTS(SELECT 1 FROM information_schema.TABLES WHERE TABLE_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='rooms_rangedcovers') THEN
   SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT='Room naming: missing source or colliding target Cells_RangedCovers';
  END IF;
  IF NOT EXISTS(SELECT 1 FROM information_schema.TABLES WHERE TABLE_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='cells_tags' AND TABLE_TYPE='BASE TABLE') OR EXISTS(SELECT 1 FROM information_schema.TABLES WHERE TABLE_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='rooms_tags') THEN
   SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT='Room naming: missing source or colliding target Cells_Tags';
  END IF;
  IF NOT EXISTS(SELECT 1 FROM information_schema.TABLES WHERE TABLE_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='clans_administrationcells' AND TABLE_TYPE='BASE TABLE') OR EXISTS(SELECT 1 FROM information_schema.TABLES WHERE TABLE_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='clans_administrationrooms') THEN
   SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT='Room naming: missing source or colliding target Clans_AdministrationCells';
  END IF;
  IF NOT EXISTS(SELECT 1 FROM information_schema.TABLES WHERE TABLE_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='clans_hallcells' AND TABLE_TYPE='BASE TABLE') OR EXISTS(SELECT 1 FROM information_schema.TABLES WHERE TABLE_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='clans_hallrooms') THEN
   SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT='Room naming: missing source or colliding target Clans_HallCells';
  END IF;
  IF NOT EXISTS(SELECT 1 FROM information_schema.TABLES WHERE TABLE_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='clans_treasurycells' AND TABLE_TYPE='BASE TABLE') OR EXISTS(SELECT 1 FROM information_schema.TABLES WHERE TABLE_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='clans_treasuryrooms') THEN
   SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT='Room naming: missing source or colliding target Clans_TreasuryCells';
  END IF;
  IF NOT EXISTS(SELECT 1 FROM information_schema.TABLES WHERE TABLE_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='legalauthoritiycells' AND TABLE_TYPE='BASE TABLE') OR EXISTS(SELECT 1 FROM information_schema.TABLES WHERE TABLE_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='legalauthoritiyrooms') THEN
   SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT='Room naming: missing source or colliding target LegalAuthoritiyCells';
  END IF;
  IF NOT EXISTS(SELECT 1 FROM information_schema.TABLES WHERE TABLE_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='legalauthorityjailcells' AND TABLE_TYPE='BASE TABLE') OR EXISTS(SELECT 1 FROM information_schema.TABLES WHERE TABLE_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='legalauthorityjailrooms') THEN
   SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT='Room naming: missing source or colliding target LegalAuthorityJailCells';
  END IF;
  IF NOT EXISTS(SELECT 1 FROM information_schema.TABLES WHERE TABLE_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='npcspawnercells' AND TABLE_TYPE='BASE TABLE') OR EXISTS(SELECT 1 FROM information_schema.TABLES WHERE TABLE_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='npcspawnerrooms') THEN
   SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT='Room naming: missing source or colliding target NPCSpawnerCells';
  END IF;
  IF NOT EXISTS(SELECT 1 FROM information_schema.TABLES WHERE TABLE_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='restaurantcells' AND TABLE_TYPE='BASE TABLE') OR EXISTS(SELECT 1 FROM information_schema.TABLES WHERE TABLE_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='restaurantrooms') THEN
   SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT='Room naming: missing source or colliding target RestaurantCells';
  END IF;
  IF NOT EXISTS(SELECT 1 FROM information_schema.TABLES WHERE TABLE_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='routecelllandmarks' AND TABLE_TYPE='BASE TABLE') OR EXISTS(SELECT 1 FROM information_schema.TABLES WHERE TABLE_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='routeroomlandmarks') THEN
   SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT='Room naming: missing source or colliding target RouteCellLandmarks';
  END IF;
  IF NOT EXISTS(SELECT 1 FROM information_schema.TABLES WHERE TABLE_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='routecells' AND TABLE_TYPE='BASE TABLE') OR EXISTS(SELECT 1 FROM information_schema.TABLES WHERE TABLE_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='routerooms') THEN
   SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT='Room naming: missing source or colliding target RouteCells';
  END IF;
  IF NOT EXISTS(SELECT 1 FROM information_schema.TABLES WHERE TABLE_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='shops_storeroomcells' AND TABLE_TYPE='BASE TABLE') OR EXISTS(SELECT 1 FROM information_schema.TABLES WHERE TABLE_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='shops_storeroomrooms') THEN
   SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT='Room naming: missing source or colliding target Shops_StoreroomCells';
  END IF;
  IF NOT EXISTS(SELECT 1 FROM information_schema.COLUMNS WHERE TABLE_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='activeprojects' AND LOWER(COLUMN_NAME)='cellid') OR EXISTS(SELECT 1 FROM information_schema.COLUMNS WHERE TABLE_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='activeprojects' AND LOWER(COLUMN_NAME)='roomid') THEN
   SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT='Room naming: missing source or colliding column ActiveProjects.CellId';
  END IF;
  IF NOT EXISTS(SELECT 1 FROM information_schema.COLUMNS WHERE TABLE_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='activeroutemotions' AND LOWER(COLUMN_NAME)='routecellid') OR EXISTS(SELECT 1 FROM information_schema.COLUMNS WHERE TABLE_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='activeroutemotions' AND LOWER(COLUMN_NAME)='routeroomid') THEN
   SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT='Room naming: missing source or colliding column ActiveRouteMotions.RouteCellId';
  END IF;
  IF NOT EXISTS(SELECT 1 FROM information_schema.COLUMNS WHERE TABLE_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='agriculturefields' AND LOWER(COLUMN_NAME)='cellid') OR EXISTS(SELECT 1 FROM information_schema.COLUMNS WHERE TABLE_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='agriculturefields' AND LOWER(COLUMN_NAME)='roomid') THEN
   SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT='Room naming: missing source or colliding column AgricultureFields.CellId';
  END IF;
  IF NOT EXISTS(SELECT 1 FROM information_schema.COLUMNS WHERE TABLE_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='areas_cells' AND LOWER(COLUMN_NAME)='cellid') OR EXISTS(SELECT 1 FROM information_schema.COLUMNS WHERE TABLE_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='areas_cells' AND LOWER(COLUMN_NAME)='roomid') THEN
   SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT='Room naming: missing source or colliding column Areas_Cells.CellId';
  END IF;
  IF NOT EXISTS(SELECT 1 FROM information_schema.COLUMNS WHERE TABLE_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='arenacells' AND LOWER(COLUMN_NAME)='cellid') OR EXISTS(SELECT 1 FROM information_schema.COLUMNS WHERE TABLE_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='arenacells' AND LOWER(COLUMN_NAME)='roomid') THEN
   SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT='Room naming: missing source or colliding column ArenaCells.CellId';
  END IF;
  IF NOT EXISTS(SELECT 1 FROM information_schema.COLUMNS WHERE TABLE_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='auctionhouses' AND LOWER(COLUMN_NAME)='auctionhousecellid') OR EXISTS(SELECT 1 FROM information_schema.COLUMNS WHERE TABLE_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='auctionhouses' AND LOWER(COLUMN_NAME)='auctionhouseroomid') THEN
   SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT='Room naming: missing source or colliding column AuctionHouses.AuctionHouseCellId';
  END IF;
  IF NOT EXISTS(SELECT 1 FROM information_schema.COLUMNS WHERE TABLE_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='bankbranches' AND LOWER(COLUMN_NAME)='cellid') OR EXISTS(SELECT 1 FROM information_schema.COLUMNS WHERE TABLE_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='bankbranches' AND LOWER(COLUMN_NAME)='roomid') THEN
   SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT='Room naming: missing source or colliding column BankBranches.CellId';
  END IF;
  IF NOT EXISTS(SELECT 1 FROM information_schema.COLUMNS WHERE TABLE_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='cellenvironmentalstates' AND LOWER(COLUMN_NAME)='cellid') OR EXISTS(SELECT 1 FROM information_schema.COLUMNS WHERE TABLE_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='cellenvironmentalstates' AND LOWER(COLUMN_NAME)='roomid') THEN
   SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT='Room naming: missing source or colliding column CellEnvironmentalStates.CellId';
  END IF;
  IF NOT EXISTS(SELECT 1 FROM information_schema.COLUMNS WHERE TABLE_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='celloverlays' AND LOWER(COLUMN_NAME)='celldescription') OR EXISTS(SELECT 1 FROM information_schema.COLUMNS WHERE TABLE_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='celloverlays' AND LOWER(COLUMN_NAME)='roomdescription') THEN
   SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT='Room naming: missing source or colliding column CellOverlays.CellDescription';
  END IF;
  IF NOT EXISTS(SELECT 1 FROM information_schema.COLUMNS WHERE TABLE_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='celloverlays' AND LOWER(COLUMN_NAME)='cellid') OR EXISTS(SELECT 1 FROM information_schema.COLUMNS WHERE TABLE_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='celloverlays' AND LOWER(COLUMN_NAME)='roomid') THEN
   SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT='Room naming: missing source or colliding column CellOverlays.CellId';
  END IF;
  IF NOT EXISTS(SELECT 1 FROM information_schema.COLUMNS WHERE TABLE_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='celloverlays' AND LOWER(COLUMN_NAME)='cellname') OR EXISTS(SELECT 1 FROM information_schema.COLUMNS WHERE TABLE_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='celloverlays' AND LOWER(COLUMN_NAME)='roomname') THEN
   SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT='Room naming: missing source or colliding column CellOverlays.CellName';
  END IF;
  IF NOT EXISTS(SELECT 1 FROM information_schema.COLUMNS WHERE TABLE_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='celloverlays' AND LOWER(COLUMN_NAME)='celloverlaypackageid') OR EXISTS(SELECT 1 FROM information_schema.COLUMNS WHERE TABLE_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='celloverlays' AND LOWER(COLUMN_NAME)='roomoverlaypackageid') THEN
   SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT='Room naming: missing source or colliding column CellOverlays.CellOverlayPackageId';
  END IF;
  IF NOT EXISTS(SELECT 1 FROM information_schema.COLUMNS WHERE TABLE_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='celloverlays' AND LOWER(COLUMN_NAME)='celloverlaypackagerevisionnumber') OR EXISTS(SELECT 1 FROM information_schema.COLUMNS WHERE TABLE_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='celloverlays' AND LOWER(COLUMN_NAME)='roomoverlaypackagerevisionnumber') THEN
   SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT='Room naming: missing source or colliding column CellOverlays.CellOverlayPackageRevisionNumber';
  END IF;
  IF NOT EXISTS(SELECT 1 FROM information_schema.COLUMNS WHERE TABLE_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='celloverlays_exits' AND LOWER(COLUMN_NAME)='celloverlayid') OR EXISTS(SELECT 1 FROM information_schema.COLUMNS WHERE TABLE_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='celloverlays_exits' AND LOWER(COLUMN_NAME)='roomoverlayid') THEN
   SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT='Room naming: missing source or colliding column CellOverlays_Exits.CellOverlayId';
  END IF;
  IF NOT EXISTS(SELECT 1 FROM information_schema.COLUMNS WHERE TABLE_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='cellroomareacontractionledger' AND LOWER(COLUMN_NAME)='roomid') OR EXISTS(SELECT 1 FROM information_schema.COLUMNS WHERE TABLE_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='cellroomareacontractionledger' AND LOWER(COLUMN_NAME)='legacyroomid') THEN
   SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT='Room naming: missing source or colliding column CellRoomAreaContractionLedger.RoomId';
  END IF;
  IF NOT EXISTS(SELECT 1 FROM information_schema.COLUMNS WHERE TABLE_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='cellroomareacontractionledger' AND LOWER(COLUMN_NAME)='cellid') THEN
   SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT='Room naming: missing source or colliding column CellRoomAreaContractionLedger.CellId';
  END IF;
  IF NOT EXISTS(SELECT 1 FROM information_schema.COLUMNS WHERE TABLE_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='cellroomareamigrationledger' AND LOWER(COLUMN_NAME)='roomid') OR EXISTS(SELECT 1 FROM information_schema.COLUMNS WHERE TABLE_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='cellroomareamigrationledger' AND LOWER(COLUMN_NAME)='legacyroomid') THEN
   SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT='Room naming: missing source or colliding column CellRoomAreaMigrationLedger.RoomId';
  END IF;
  IF NOT EXISTS(SELECT 1 FROM information_schema.COLUMNS WHERE TABLE_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='cellroomareamigrationledger' AND LOWER(COLUMN_NAME)='cellid') THEN
   SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT='Room naming: missing source or colliding column CellRoomAreaMigrationLedger.CellId';
  END IF;
  IF NOT EXISTS(SELECT 1 FROM information_schema.COLUMNS WHERE TABLE_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='cellroomcontractionledger' AND LOWER(COLUMN_NAME)='roomid') OR EXISTS(SELECT 1 FROM information_schema.COLUMNS WHERE TABLE_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='cellroomcontractionledger' AND LOWER(COLUMN_NAME)='legacyroomid') THEN
   SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT='Room naming: missing source or colliding column CellRoomContractionLedger.RoomId';
  END IF;
  IF NOT EXISTS(SELECT 1 FROM information_schema.COLUMNS WHERE TABLE_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='cellroomcontractionledger' AND LOWER(COLUMN_NAME)='cellid') THEN
   SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT='Room naming: missing source or colliding column CellRoomContractionLedger.CellId';
  END IF;
  IF NOT EXISTS(SELECT 1 FROM information_schema.COLUMNS WHERE TABLE_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='cellroommigrationledger' AND LOWER(COLUMN_NAME)='roomid') OR EXISTS(SELECT 1 FROM information_schema.COLUMNS WHERE TABLE_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='cellroommigrationledger' AND LOWER(COLUMN_NAME)='legacyroomid') THEN
   SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT='Room naming: missing source or colliding column CellRoomMigrationLedger.RoomId';
  END IF;
  IF NOT EXISTS(SELECT 1 FROM information_schema.COLUMNS WHERE TABLE_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='cellroommigrationledger' AND LOWER(COLUMN_NAME)='cellid') THEN
   SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT='Room naming: missing source or colliding column CellRoomMigrationLedger.CellId';
  END IF;
  IF NOT EXISTS(SELECT 1 FROM information_schema.COLUMNS WHERE TABLE_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='cells_foragableyields' AND LOWER(COLUMN_NAME)='cellid') OR EXISTS(SELECT 1 FROM information_schema.COLUMNS WHERE TABLE_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='cells_foragableyields' AND LOWER(COLUMN_NAME)='roomid') THEN
   SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT='Room naming: missing source or colliding column Cells_ForagableYields.CellId';
  END IF;
  IF NOT EXISTS(SELECT 1 FROM information_schema.COLUMNS WHERE TABLE_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='cells_gameitems' AND LOWER(COLUMN_NAME)='cellid') OR EXISTS(SELECT 1 FROM information_schema.COLUMNS WHERE TABLE_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='cells_gameitems' AND LOWER(COLUMN_NAME)='roomid') THEN
   SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT='Room naming: missing source or colliding column Cells_GameItems.CellId';
  END IF;
  IF NOT EXISTS(SELECT 1 FROM information_schema.COLUMNS WHERE TABLE_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='cells_magicresources' AND LOWER(COLUMN_NAME)='cellid') OR EXISTS(SELECT 1 FROM information_schema.COLUMNS WHERE TABLE_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='cells_magicresources' AND LOWER(COLUMN_NAME)='roomid') THEN
   SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT='Room naming: missing source or colliding column Cells_MagicResources.CellId';
  END IF;
  IF NOT EXISTS(SELECT 1 FROM information_schema.COLUMNS WHERE TABLE_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='cells_rangedcovers' AND LOWER(COLUMN_NAME)='cellid') OR EXISTS(SELECT 1 FROM information_schema.COLUMNS WHERE TABLE_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='cells_rangedcovers' AND LOWER(COLUMN_NAME)='roomid') THEN
   SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT='Room naming: missing source or colliding column Cells_RangedCovers.CellId';
  END IF;
  IF NOT EXISTS(SELECT 1 FROM information_schema.COLUMNS WHERE TABLE_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='cells_tags' AND LOWER(COLUMN_NAME)='cellid') OR EXISTS(SELECT 1 FROM information_schema.COLUMNS WHERE TABLE_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='cells_tags' AND LOWER(COLUMN_NAME)='roomid') THEN
   SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT='Room naming: missing source or colliding column Cells_Tags.CellId';
  END IF;
  IF NOT EXISTS(SELECT 1 FROM information_schema.COLUMNS WHERE TABLE_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='characterlog' AND LOWER(COLUMN_NAME)='cellid') OR EXISTS(SELECT 1 FROM information_schema.COLUMNS WHERE TABLE_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='characterlog' AND LOWER(COLUMN_NAME)='roomid') THEN
   SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT='Room naming: missing source or colliding column CharacterLog.CellId';
  END IF;
  IF NOT EXISTS(SELECT 1 FROM information_schema.COLUMNS WHERE TABLE_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='clans_administrationcells' AND LOWER(COLUMN_NAME)='cellid') OR EXISTS(SELECT 1 FROM information_schema.COLUMNS WHERE TABLE_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='clans_administrationcells' AND LOWER(COLUMN_NAME)='roomid') THEN
   SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT='Room naming: missing source or colliding column Clans_AdministrationCells.CellId';
  END IF;
  IF NOT EXISTS(SELECT 1 FROM information_schema.COLUMNS WHERE TABLE_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='clans_hallcells' AND LOWER(COLUMN_NAME)='cellid') OR EXISTS(SELECT 1 FROM information_schema.COLUMNS WHERE TABLE_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='clans_hallcells' AND LOWER(COLUMN_NAME)='roomid') THEN
   SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT='Room naming: missing source or colliding column Clans_HallCells.CellId';
  END IF;
  IF NOT EXISTS(SELECT 1 FROM information_schema.COLUMNS WHERE TABLE_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='clans_treasurycells' AND LOWER(COLUMN_NAME)='cellid') OR EXISTS(SELECT 1 FROM information_schema.COLUMNS WHERE TABLE_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='clans_treasurycells' AND LOWER(COLUMN_NAME)='roomid') THEN
   SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT='Room naming: missing source or colliding column Clans_TreasuryCells.CellId';
  END IF;
  IF NOT EXISTS(SELECT 1 FROM information_schema.COLUMNS WHERE TABLE_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='conveyancinglocations' AND LOWER(COLUMN_NAME)='cellid') OR EXISTS(SELECT 1 FROM information_schema.COLUMNS WHERE TABLE_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='conveyancinglocations' AND LOWER(COLUMN_NAME)='roomid') THEN
   SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT='Room naming: missing source or colliding column ConveyancingLocations.CellId';
  END IF;
  IF NOT EXISTS(SELECT 1 FROM information_schema.COLUMNS WHERE TABLE_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='corpserecoveryreports' AND LOWER(COLUMN_NAME)='destinationcellid') OR EXISTS(SELECT 1 FROM information_schema.COLUMNS WHERE TABLE_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='corpserecoveryreports' AND LOWER(COLUMN_NAME)='destinationroomid') THEN
   SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT='Room naming: missing source or colliding column CorpseRecoveryReports.DestinationCellId';
  END IF;
  IF NOT EXISTS(SELECT 1 FROM information_schema.COLUMNS WHERE TABLE_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='corpserecoveryreports' AND LOWER(COLUMN_NAME)='sourcecellid') OR EXISTS(SELECT 1 FROM information_schema.COLUMNS WHERE TABLE_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='corpserecoveryreports' AND LOWER(COLUMN_NAME)='sourceroomid') THEN
   SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT='Room naming: missing source or colliding column CorpseRecoveryReports.SourceCellId';
  END IF;
  IF NOT EXISTS(SELECT 1 FROM information_schema.COLUMNS WHERE TABLE_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='employmentactionsteps' AND LOWER(COLUMN_NAME)='destinationcellid') OR EXISTS(SELECT 1 FROM information_schema.COLUMNS WHERE TABLE_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='employmentactionsteps' AND LOWER(COLUMN_NAME)='destinationroomid') THEN
   SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT='Room naming: missing source or colliding column EmploymentActionSteps.DestinationCellId';
  END IF;
  IF NOT EXISTS(SELECT 1 FROM information_schema.COLUMNS WHERE TABLE_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='employmentactionsteps' AND LOWER(COLUMN_NAME)='executioncellid') OR EXISTS(SELECT 1 FROM information_schema.COLUMNS WHERE TABLE_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='employmentactionsteps' AND LOWER(COLUMN_NAME)='executionroomid') THEN
   SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT='Room naming: missing source or colliding column EmploymentActionSteps.ExecutionCellId';
  END IF;
  IF NOT EXISTS(SELECT 1 FROM information_schema.COLUMNS WHERE TABLE_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='environmentalmagicoperations' AND LOWER(COLUMN_NAME)='cellid') OR EXISTS(SELECT 1 FROM information_schema.COLUMNS WHERE TABLE_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='environmentalmagicoperations' AND LOWER(COLUMN_NAME)='roomid') THEN
   SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT='Room naming: missing source or colliding column EnvironmentalMagicOperations.CellId';
  END IF;
  IF NOT EXISTS(SELECT 1 FROM information_schema.COLUMNS WHERE TABLE_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='exits' AND LOWER(COLUMN_NAME)='cellid1') OR EXISTS(SELECT 1 FROM information_schema.COLUMNS WHERE TABLE_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='exits' AND LOWER(COLUMN_NAME)='roomid1') THEN
   SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT='Room naming: missing source or colliding column Exits.CellId1';
  END IF;
  IF NOT EXISTS(SELECT 1 FROM information_schema.COLUMNS WHERE TABLE_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='exits' AND LOWER(COLUMN_NAME)='cellid2') OR EXISTS(SELECT 1 FROM information_schema.COLUMNS WHERE TABLE_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='exits' AND LOWER(COLUMN_NAME)='roomid2') THEN
   SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT='Room naming: missing source or colliding column Exits.CellId2';
  END IF;
  IF NOT EXISTS(SELECT 1 FROM information_schema.COLUMNS WHERE TABLE_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='exits' AND LOWER(COLUMN_NAME)='fallcell') OR EXISTS(SELECT 1 FROM information_schema.COLUMNS WHERE TABLE_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='exits' AND LOWER(COLUMN_NAME)='fallroom') THEN
   SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT='Room naming: missing source or colliding column Exits.FallCell';
  END IF;
  IF NOT EXISTS(SELECT 1 FROM information_schema.COLUMNS WHERE TABLE_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='hooks_perceivables' AND LOWER(COLUMN_NAME)='cellid') OR EXISTS(SELECT 1 FROM information_schema.COLUMNS WHERE TABLE_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='hooks_perceivables' AND LOWER(COLUMN_NAME)='roomid') THEN
   SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT='Room naming: missing source or colliding column Hooks_Perceivables.CellId';
  END IF;
  IF NOT EXISTS(SELECT 1 FROM information_schema.COLUMNS WHERE TABLE_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='hospitallocations' AND LOWER(COLUMN_NAME)='cellid') OR EXISTS(SELECT 1 FROM information_schema.COLUMNS WHERE TABLE_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='hospitallocations' AND LOWER(COLUMN_NAME)='roomid') THEN
   SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT='Room naming: missing source or colliding column HospitalLocations.CellId';
  END IF;
  IF NOT EXISTS(SELECT 1 FROM information_schema.COLUMNS WHERE TABLE_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='hospitalservicerequests' AND LOWER(COLUMN_NAME)='operatingtheatrecellid') OR EXISTS(SELECT 1 FROM information_schema.COLUMNS WHERE TABLE_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='hospitalservicerequests' AND LOWER(COLUMN_NAME)='operatingtheatreroomid') THEN
   SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT='Room naming: missing source or colliding column HospitalServiceRequests.OperatingTheatreCellId';
  END IF;
  IF NOT EXISTS(SELECT 1 FROM information_schema.COLUMNS WHERE TABLE_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='hospitalservicerequests' AND LOWER(COLUMN_NAME)='recoveryroomcellid') OR EXISTS(SELECT 1 FROM information_schema.COLUMNS WHERE TABLE_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='hospitalservicerequests' AND LOWER(COLUMN_NAME)='recoveryroomid') THEN
   SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT='Room naming: missing source or colliding column HospitalServiceRequests.RecoveryRoomCellId';
  END IF;
  IF NOT EXISTS(SELECT 1 FROM information_schema.COLUMNS WHERE TABLE_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='hospitalservicerequests' AND LOWER(COLUMN_NAME)='returncellid') OR EXISTS(SELECT 1 FROM information_schema.COLUMNS WHERE TABLE_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='hospitalservicerequests' AND LOWER(COLUMN_NAME)='returnroomid') THEN
   SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT='Room naming: missing source or colliding column HospitalServiceRequests.ReturnCellId';
  END IF;
  IF NOT EXISTS(SELECT 1 FROM information_schema.COLUMNS WHERE TABLE_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='hotelrooms' AND LOWER(COLUMN_NAME)='cellid') OR EXISTS(SELECT 1 FROM information_schema.COLUMNS WHERE TABLE_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='hotelrooms' AND LOWER(COLUMN_NAME)='roomid') THEN
   SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT='Room naming: missing source or colliding column HotelRooms.CellId';
  END IF;
  IF NOT EXISTS(SELECT 1 FROM information_schema.COLUMNS WHERE TABLE_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='jobfindinglocations' AND LOWER(COLUMN_NAME)='cellid') OR EXISTS(SELECT 1 FROM information_schema.COLUMNS WHERE TABLE_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='jobfindinglocations' AND LOWER(COLUMN_NAME)='roomid') THEN
   SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT='Room naming: missing source or colliding column JobFindingLocations.CellId';
  END IF;
  IF NOT EXISTS(SELECT 1 FROM information_schema.COLUMNS WHERE TABLE_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='landrejuvenationtreatments' AND LOWER(COLUMN_NAME)='cellid') OR EXISTS(SELECT 1 FROM information_schema.COLUMNS WHERE TABLE_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='landrejuvenationtreatments' AND LOWER(COLUMN_NAME)='roomid') THEN
   SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT='Room naming: missing source or colliding column LandRejuvenationTreatments.CellId';
  END IF;
  IF NOT EXISTS(SELECT 1 FROM information_schema.COLUMNS WHERE TABLE_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='legalauthoritiycells' AND LOWER(COLUMN_NAME)='cellid') OR EXISTS(SELECT 1 FROM information_schema.COLUMNS WHERE TABLE_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='legalauthoritiycells' AND LOWER(COLUMN_NAME)='roomid') THEN
   SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT='Room naming: missing source or colliding column LegalAuthoritiyCells.CellId';
  END IF;
  IF NOT EXISTS(SELECT 1 FROM information_schema.COLUMNS WHERE TABLE_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='legalauthorityjailcells' AND LOWER(COLUMN_NAME)='cellid') OR EXISTS(SELECT 1 FROM information_schema.COLUMNS WHERE TABLE_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='legalauthorityjailcells' AND LOWER(COLUMN_NAME)='roomid') THEN
   SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT='Room naming: missing source or colliding column LegalAuthorityJailCells.CellId';
  END IF;
  IF NOT EXISTS(SELECT 1 FROM information_schema.COLUMNS WHERE TABLE_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='magicgatheringoperations' AND LOWER(COLUMN_NAME)='cellid') OR EXISTS(SELECT 1 FROM information_schema.COLUMNS WHERE TABLE_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='magicgatheringoperations' AND LOWER(COLUMN_NAME)='roomid') THEN
   SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT='Room naming: missing source or colliding column MagicGatheringOperations.CellId';
  END IF;
  IF NOT EXISTS(SELECT 1 FROM information_schema.COLUMNS WHERE TABLE_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='magicgatheringparticipants' AND LOWER(COLUMN_NAME)='cellid') OR EXISTS(SELECT 1 FROM information_schema.COLUMNS WHERE TABLE_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='magicgatheringparticipants' AND LOWER(COLUMN_NAME)='roomid') THEN
   SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT='Room naming: missing source or colliding column MagicGatheringParticipants.CellId';
  END IF;
  IF NOT EXISTS(SELECT 1 FROM information_schema.COLUMNS WHERE TABLE_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='magicportalendpoints' AND LOWER(COLUMN_NAME)='cellid') OR EXISTS(SELECT 1 FROM information_schema.COLUMNS WHERE TABLE_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='magicportalendpoints' AND LOWER(COLUMN_NAME)='roomid') THEN
   SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT='Room naming: missing source or colliding column MagicPortalEndpoints.CellId';
  END IF;
  IF NOT EXISTS(SELECT 1 FROM information_schema.COLUMNS WHERE TABLE_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='npcspawnercells' AND LOWER(COLUMN_NAME)='cellid') OR EXISTS(SELECT 1 FROM information_schema.COLUMNS WHERE TABLE_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='npcspawnercells' AND LOWER(COLUMN_NAME)='roomid') THEN
   SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT='Room naming: missing source or colliding column NPCSpawnerCells.CellId';
  END IF;
  IF NOT EXISTS(SELECT 1 FROM information_schema.COLUMNS WHERE TABLE_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='patrolroutesnodes' AND LOWER(COLUMN_NAME)='cellid') OR EXISTS(SELECT 1 FROM information_schema.COLUMNS WHERE TABLE_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='patrolroutesnodes' AND LOWER(COLUMN_NAME)='roomid') THEN
   SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT='Room naming: missing source or colliding column PatrolRoutesNodes.CellId';
  END IF;
  IF NOT EXISTS(SELECT 1 FROM information_schema.COLUMNS WHERE TABLE_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='probatelocations' AND LOWER(COLUMN_NAME)='cellid') OR EXISTS(SELECT 1 FROM information_schema.COLUMNS WHERE TABLE_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='probatelocations' AND LOWER(COLUMN_NAME)='roomid') THEN
   SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT='Room naming: missing source or colliding column ProbateLocations.CellId';
  END IF;
  IF NOT EXISTS(SELECT 1 FROM information_schema.COLUMNS WHERE TABLE_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='propertylocations' AND LOWER(COLUMN_NAME)='cellid') OR EXISTS(SELECT 1 FROM information_schema.COLUMNS WHERE TABLE_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='propertylocations' AND LOWER(COLUMN_NAME)='roomid') THEN
   SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT='Room naming: missing source or colliding column PropertyLocations.CellId';
  END IF;
  IF NOT EXISTS(SELECT 1 FROM information_schema.COLUMNS WHERE TABLE_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='restaurantcells' AND LOWER(COLUMN_NAME)='cellid') OR EXISTS(SELECT 1 FROM information_schema.COLUMNS WHERE TABLE_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='restaurantcells' AND LOWER(COLUMN_NAME)='roomid') THEN
   SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT='Room naming: missing source or colliding column RestaurantCells.CellId';
  END IF;
  IF NOT EXISTS(SELECT 1 FROM information_schema.COLUMNS WHERE TABLE_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='routecelllandmarks' AND LOWER(COLUMN_NAME)='routecellid') OR EXISTS(SELECT 1 FROM information_schema.COLUMNS WHERE TABLE_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='routecelllandmarks' AND LOWER(COLUMN_NAME)='routeroomid') THEN
   SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT='Room naming: missing source or colliding column RouteCellLandmarks.RouteCellId';
  END IF;
  IF NOT EXISTS(SELECT 1 FROM information_schema.COLUMNS WHERE TABLE_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='routecells' AND LOWER(COLUMN_NAME)='cellid') OR EXISTS(SELECT 1 FROM information_schema.COLUMNS WHERE TABLE_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='routecells' AND LOWER(COLUMN_NAME)='roomid') THEN
   SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT='Room naming: missing source or colliding column RouteCells.CellId';
  END IF;
  IF NOT EXISTS(SELECT 1 FROM information_schema.COLUMNS WHERE TABLE_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='routeexitanchors' AND LOWER(COLUMN_NAME)='routecellid') OR EXISTS(SELECT 1 FROM information_schema.COLUMNS WHERE TABLE_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='routeexitanchors' AND LOWER(COLUMN_NAME)='routeroomid') THEN
   SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT='Room naming: missing source or colliding column RouteExitAnchors.RouteCellId';
  END IF;
  IF NOT EXISTS(SELECT 1 FROM information_schema.COLUMNS WHERE TABLE_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='shops' AND LOWER(COLUMN_NAME)='stockroomcellid') OR EXISTS(SELECT 1 FROM information_schema.COLUMNS WHERE TABLE_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='shops' AND LOWER(COLUMN_NAME)='stockroomid') THEN
   SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT='Room naming: missing source or colliding column Shops.StockroomCellId';
  END IF;
  IF NOT EXISTS(SELECT 1 FROM information_schema.COLUMNS WHERE TABLE_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='shops' AND LOWER(COLUMN_NAME)='workshopcellid') OR EXISTS(SELECT 1 FROM information_schema.COLUMNS WHERE TABLE_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='shops' AND LOWER(COLUMN_NAME)='workshoproomid') THEN
   SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT='Room naming: missing source or colliding column Shops.WorkshopCellId';
  END IF;
  IF NOT EXISTS(SELECT 1 FROM information_schema.COLUMNS WHERE TABLE_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='shops_storeroomcells' AND LOWER(COLUMN_NAME)='cellid') OR EXISTS(SELECT 1 FROM information_schema.COLUMNS WHERE TABLE_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='shops_storeroomcells' AND LOWER(COLUMN_NAME)='roomid') THEN
   SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT='Room naming: missing source or colliding column Shops_StoreroomCells.CellId';
  END IF;
  IF NOT EXISTS(SELECT 1 FROM information_schema.COLUMNS WHERE TABLE_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='stables' AND LOWER(COLUMN_NAME)='cellid') OR EXISTS(SELECT 1 FROM information_schema.COLUMNS WHERE TABLE_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='stables' AND LOWER(COLUMN_NAME)='roomid') THEN
   SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT='Room naming: missing source or colliding column Stables.CellId';
  END IF;
  IF NOT EXISTS(SELECT 1 FROM information_schema.COLUMNS WHERE TABLE_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='terrains' AND LOWER(COLUMN_NAME)='defaultcelloutdoorstype') OR EXISTS(SELECT 1 FROM information_schema.COLUMNS WHERE TABLE_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='terrains' AND LOWER(COLUMN_NAME)='defaultroomoutdoorstype') THEN
   SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT='Room naming: missing source or colliding column Terrains.DefaultCellOutdoorsType';
  END IF;
  IF NOT EXISTS(SELECT 1 FROM information_schema.COLUMNS WHERE TABLE_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='tracks' AND LOWER(COLUMN_NAME)='cellid') OR EXISTS(SELECT 1 FROM information_schema.COLUMNS WHERE TABLE_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='tracks' AND LOWER(COLUMN_NAME)='roomid') THEN
   SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT='Room naming: missing source or colliding column Tracks.CellId';
  END IF;
  IF NOT EXISTS(SELECT 1 FROM information_schema.COLUMNS WHERE TABLE_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='vehiclecompartments' AND LOWER(COLUMN_NAME)='interiorcellid') OR EXISTS(SELECT 1 FROM information_schema.COLUMNS WHERE TABLE_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='vehiclecompartments' AND LOWER(COLUMN_NAME)='interiorroomid') THEN
   SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT='Room naming: missing source or colliding column VehicleCompartments.InteriorCellId';
  END IF;
  IF NOT EXISTS(SELECT 1 FROM information_schema.COLUMNS WHERE TABLE_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='vehicledockings' AND LOWER(COLUMN_NAME)='exteriorcellid') OR EXISTS(SELECT 1 FROM information_schema.COLUMNS WHERE TABLE_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='vehicledockings' AND LOWER(COLUMN_NAME)='exteriorroomid') THEN
   SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT='Room naming: missing source or colliding column VehicleDockings.ExteriorCellId';
  END IF;
  IF NOT EXISTS(SELECT 1 FROM information_schema.COLUMNS WHERE TABLE_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='vehiclerouteplatformbindings' AND LOWER(COLUMN_NAME)='platformcellid') OR EXISTS(SELECT 1 FROM information_schema.COLUMNS WHERE TABLE_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='vehiclerouteplatformbindings' AND LOWER(COLUMN_NAME)='platformroomid') THEN
   SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT='Room naming: missing source or colliding column VehicleRoutePlatformBindings.PlatformCellId';
  END IF;
  IF NOT EXISTS(SELECT 1 FROM information_schema.COLUMNS WHERE TABLE_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='vehicleroutesteps' AND LOWER(COLUMN_NAME)='destinationcellid') OR EXISTS(SELECT 1 FROM information_schema.COLUMNS WHERE TABLE_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='vehicleroutesteps' AND LOWER(COLUMN_NAME)='destinationroomid') THEN
   SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT='Room naming: missing source or colliding column VehicleRouteSteps.DestinationCellId';
  END IF;
  IF NOT EXISTS(SELECT 1 FROM information_schema.COLUMNS WHERE TABLE_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='vehicleroutesteps' AND LOWER(COLUMN_NAME)='origincellid') OR EXISTS(SELECT 1 FROM information_schema.COLUMNS WHERE TABLE_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='vehicleroutesteps' AND LOWER(COLUMN_NAME)='originroomid') THEN
   SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT='Room naming: missing source or colliding column VehicleRouteSteps.OriginCellId';
  END IF;
  IF NOT EXISTS(SELECT 1 FROM information_schema.COLUMNS WHERE TABLE_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='vehicleroutestops' AND LOWER(COLUMN_NAME)='cellid') OR EXISTS(SELECT 1 FROM information_schema.COLUMNS WHERE TABLE_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='vehicleroutestops' AND LOWER(COLUMN_NAME)='roomid') THEN
   SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT='Room naming: missing source or colliding column VehicleRouteStops.CellId';
  END IF;
  IF NOT EXISTS(SELECT 1 FROM information_schema.COLUMNS WHERE TABLE_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='vehicleroutetopologypins' AND LOWER(COLUMN_NAME)='routecellid') OR EXISTS(SELECT 1 FROM information_schema.COLUMNS WHERE TABLE_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='vehicleroutetopologypins' AND LOWER(COLUMN_NAME)='routeroomid') THEN
   SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT='Room naming: missing source or colliding column VehicleRouteTopologyPins.RouteCellId';
  END IF;
  IF NOT EXISTS(SELECT 1 FROM information_schema.COLUMNS WHERE TABLE_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='vehicles' AND LOWER(COLUMN_NAME)='currentcellid') OR EXISTS(SELECT 1 FROM information_schema.COLUMNS WHERE TABLE_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='vehicles' AND LOWER(COLUMN_NAME)='currentroomid') THEN
   SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT='Room naming: missing source or colliding column Vehicles.CurrentCellId';
  END IF;
  IF NOT EXISTS(SELECT 1 FROM information_schema.COLUMNS WHERE TABLE_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='vehicles' AND LOWER(COLUMN_NAME)='destinationcellid') OR EXISTS(SELECT 1 FROM information_schema.COLUMNS WHERE TABLE_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='vehicles' AND LOWER(COLUMN_NAME)='destinationroomid') THEN
   SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT='Room naming: missing source or colliding column Vehicles.DestinationCellId';
  END IF;
  IF NOT EXISTS(SELECT 1 FROM information_schema.COLUMNS WHERE TABLE_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='zones' AND LOWER(COLUMN_NAME)='defaultcellid') OR EXISTS(SELECT 1 FROM information_schema.COLUMNS WHERE TABLE_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='zones' AND LOWER(COLUMN_NAME)='defaultroomid') THEN
   SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT='Room naming: missing source or colliding column Zones.DefaultCellId';
  END IF;
  IF NOT EXISTS(SELECT 1 FROM information_schema.STATISTICS WHERE TABLE_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='activeprojects' AND LOWER(INDEX_NAME)='fk_activeprojects_cells_idx') THEN
   SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT='Room naming: source index absent FK_ActiveProjects_Cells_idx';
  END IF;
  IF EXISTS(SELECT 1 FROM information_schema.STATISTICS WHERE TABLE_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='activeprojects' AND LOWER(INDEX_NAME)='fk_activeprojects_rooms_idx') THEN
   SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT='Room naming: colliding target index FK_ActiveProjects_Rooms_idx';
  END IF;
  IF NOT EXISTS(SELECT 1 FROM information_schema.STATISTICS WHERE TABLE_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='activeroutemotions' AND LOWER(INDEX_NAME)='ix_activeroutemotions_routecell_layer_status') THEN
   SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT='Room naming: source index absent IX_ActiveRouteMotions_RouteCell_Layer_Status';
  END IF;
  IF EXISTS(SELECT 1 FROM information_schema.STATISTICS WHERE TABLE_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='activeroutemotions' AND LOWER(INDEX_NAME)='ix_activeroutemotions_routeroom_layer_status') THEN
   SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT='Room naming: colliding target index IX_ActiveRouteMotions_RouteRoom_Layer_Status';
  END IF;
  IF NOT EXISTS(SELECT 1 FROM information_schema.STATISTICS WHERE TABLE_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='agriculturefields' AND LOWER(INDEX_NAME)='ix_agriculturefields_cellid') THEN
   SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT='Room naming: source index absent IX_AgricultureFields_CellId';
  END IF;
  IF EXISTS(SELECT 1 FROM information_schema.STATISTICS WHERE TABLE_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='agriculturefields' AND LOWER(INDEX_NAME)='ix_agriculturefields_roomid') THEN
   SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT='Room naming: colliding target index IX_AgricultureFields_RoomId';
  END IF;
  IF NOT EXISTS(SELECT 1 FROM information_schema.STATISTICS WHERE TABLE_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='areas_cells' AND LOWER(INDEX_NAME)='ix_areas_cells_cellid') THEN
   SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT='Room naming: source index absent IX_Areas_Cells_CellId';
  END IF;
  IF EXISTS(SELECT 1 FROM information_schema.STATISTICS WHERE TABLE_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='areas_cells' AND LOWER(INDEX_NAME)='ix_areas_rooms_roomid') THEN
   SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT='Room naming: colliding target index IX_Areas_Rooms_RoomId';
  END IF;
  IF NOT EXISTS(SELECT 1 FROM information_schema.STATISTICS WHERE TABLE_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='arenacells' AND LOWER(INDEX_NAME)='fk_arenacells_arenas') THEN
   SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT='Room naming: source index absent FK_ArenaCells_Arenas';
  END IF;
  IF EXISTS(SELECT 1 FROM information_schema.STATISTICS WHERE TABLE_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='arenacells' AND LOWER(INDEX_NAME)='fk_arenarooms_arenas') THEN
   SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT='Room naming: colliding target index FK_ArenaRooms_Arenas';
  END IF;
  IF NOT EXISTS(SELECT 1 FROM information_schema.STATISTICS WHERE TABLE_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='arenacells' AND LOWER(INDEX_NAME)='fk_arenacells_cells') THEN
   SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT='Room naming: source index absent FK_ArenaCells_Cells';
  END IF;
  IF EXISTS(SELECT 1 FROM information_schema.STATISTICS WHERE TABLE_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='arenacells' AND LOWER(INDEX_NAME)='fk_arenarooms_rooms') THEN
   SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT='Room naming: colliding target index FK_ArenaRooms_Rooms';
  END IF;
  IF NOT EXISTS(SELECT 1 FROM information_schema.STATISTICS WHERE TABLE_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='auctionhouses' AND LOWER(INDEX_NAME)='ix_auctionhouses_auctionhousecellid') THEN
   SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT='Room naming: source index absent IX_AuctionHouses_AuctionHouseCellId';
  END IF;
  IF EXISTS(SELECT 1 FROM information_schema.STATISTICS WHERE TABLE_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='auctionhouses' AND LOWER(INDEX_NAME)='ix_auctionhouses_auctionhouseroomid') THEN
   SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT='Room naming: colliding target index IX_AuctionHouses_AuctionHouseRoomId';
  END IF;
  IF NOT EXISTS(SELECT 1 FROM information_schema.STATISTICS WHERE TABLE_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='bankbranches' AND LOWER(INDEX_NAME)='ix_bankbranches_cellid') THEN
   SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT='Room naming: source index absent IX_BankBranches_CellId';
  END IF;
  IF EXISTS(SELECT 1 FROM information_schema.STATISTICS WHERE TABLE_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='bankbranches' AND LOWER(INDEX_NAME)='ix_bankbranches_roomid') THEN
   SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT='Room naming: colliding target index IX_BankBranches_RoomId';
  END IF;
  IF NOT EXISTS(SELECT 1 FROM information_schema.STATISTICS WHERE TABLE_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='celloverlaypackages' AND LOWER(INDEX_NAME)='fk_celloverlaypackages_editableitems') THEN
   SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT='Room naming: source index absent FK_CellOverlayPackages_EditableItems';
  END IF;
  IF EXISTS(SELECT 1 FROM information_schema.STATISTICS WHERE TABLE_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='celloverlaypackages' AND LOWER(INDEX_NAME)='fk_roomoverlaypackages_editableitems') THEN
   SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT='Room naming: colliding target index FK_RoomOverlayPackages_EditableItems';
  END IF;
  IF NOT EXISTS(SELECT 1 FROM information_schema.STATISTICS WHERE TABLE_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='celloverlays' AND LOWER(INDEX_NAME)='fk_celloverlays_celloverlaypackages') THEN
   SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT='Room naming: source index absent FK_CellOverlays_CellOverlayPackages';
  END IF;
  IF EXISTS(SELECT 1 FROM information_schema.STATISTICS WHERE TABLE_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='celloverlays' AND LOWER(INDEX_NAME)='fk_roomoverlays_roomoverlaypackages') THEN
   SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT='Room naming: colliding target index FK_RoomOverlays_RoomOverlayPackages';
  END IF;
  IF NOT EXISTS(SELECT 1 FROM information_schema.STATISTICS WHERE TABLE_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='celloverlays' AND LOWER(INDEX_NAME)='fk_celloverlays_cells') THEN
   SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT='Room naming: source index absent FK_CellOverlays_Cells';
  END IF;
  IF EXISTS(SELECT 1 FROM information_schema.STATISTICS WHERE TABLE_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='celloverlays' AND LOWER(INDEX_NAME)='fk_roomoverlays_rooms') THEN
   SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT='Room naming: colliding target index FK_RoomOverlays_Rooms';
  END IF;
  IF NOT EXISTS(SELECT 1 FROM information_schema.STATISTICS WHERE TABLE_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='celloverlays' AND LOWER(INDEX_NAME)='fk_celloverlays_hearingprofiles') THEN
   SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT='Room naming: source index absent FK_CellOverlays_HearingProfiles';
  END IF;
  IF EXISTS(SELECT 1 FROM information_schema.STATISTICS WHERE TABLE_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='celloverlays' AND LOWER(INDEX_NAME)='fk_roomoverlays_hearingprofiles') THEN
   SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT='Room naming: colliding target index FK_RoomOverlays_HearingProfiles';
  END IF;
  IF NOT EXISTS(SELECT 1 FROM information_schema.STATISTICS WHERE TABLE_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='celloverlays' AND LOWER(INDEX_NAME)='fk_celloverlays_terrains') THEN
   SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT='Room naming: source index absent FK_CellOverlays_Terrains';
  END IF;
  IF EXISTS(SELECT 1 FROM information_schema.STATISTICS WHERE TABLE_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='celloverlays' AND LOWER(INDEX_NAME)='fk_roomoverlays_terrains') THEN
   SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT='Room naming: colliding target index FK_RoomOverlays_Terrains';
  END IF;
  IF NOT EXISTS(SELECT 1 FROM information_schema.STATISTICS WHERE TABLE_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='celloverlays_exits' AND LOWER(INDEX_NAME)='fk_celloverlays_exits_exits') THEN
   SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT='Room naming: source index absent FK_CellOverlays_Exits_Exits';
  END IF;
  IF EXISTS(SELECT 1 FROM information_schema.STATISTICS WHERE TABLE_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='celloverlays_exits' AND LOWER(INDEX_NAME)='fk_roomoverlays_exits_exits') THEN
   SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT='Room naming: colliding target index FK_RoomOverlays_Exits_Exits';
  END IF;
  IF NOT EXISTS(SELECT 1 FROM information_schema.STATISTICS WHERE TABLE_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='cells' AND LOWER(INDEX_NAME)='fk_cells_celloverlays') THEN
   SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT='Room naming: source index absent FK_Cells_CellOverlays';
  END IF;
  IF EXISTS(SELECT 1 FROM information_schema.STATISTICS WHERE TABLE_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='cells' AND LOWER(INDEX_NAME)='fk_rooms_roomoverlays') THEN
   SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT='Room naming: colliding target index FK_Rooms_RoomOverlays';
  END IF;
  IF NOT EXISTS(SELECT 1 FROM information_schema.STATISTICS WHERE TABLE_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='cells' AND LOWER(INDEX_NAME)='fk_cells_hostedvehicles_idx') THEN
   SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT='Room naming: source index absent FK_Cells_HostedVehicles_idx';
  END IF;
  IF EXISTS(SELECT 1 FROM information_schema.STATISTICS WHERE TABLE_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='cells' AND LOWER(INDEX_NAME)='fk_rooms_hostedvehicles_idx') THEN
   SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT='Room naming: colliding target index FK_Rooms_HostedVehicles_idx';
  END IF;
  IF NOT EXISTS(SELECT 1 FROM information_schema.STATISTICS WHERE TABLE_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='cells' AND LOWER(INDEX_NAME)='ix_cells_environmentalmagicprofileid') THEN
   SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT='Room naming: source index absent IX_Cells_EnvironmentalMagicProfileId';
  END IF;
  IF EXISTS(SELECT 1 FROM information_schema.STATISTICS WHERE TABLE_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='cells' AND LOWER(INDEX_NAME)='ix_rooms_environmentalmagicprofileid') THEN
   SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT='Room naming: colliding target index IX_Rooms_EnvironmentalMagicProfileId';
  END IF;
  IF NOT EXISTS(SELECT 1 FROM information_schema.STATISTICS WHERE TABLE_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='cells' AND LOWER(INDEX_NAME)='ix_cells_uniquename') THEN
   SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT='Room naming: source index absent IX_Cells_UniqueName';
  END IF;
  IF EXISTS(SELECT 1 FROM information_schema.STATISTICS WHERE TABLE_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='cells' AND LOWER(INDEX_NAME)='ix_rooms_uniquename') THEN
   SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT='Room naming: colliding target index IX_Rooms_UniqueName';
  END IF;
  IF NOT EXISTS(SELECT 1 FROM information_schema.STATISTICS WHERE TABLE_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='cells' AND LOWER(INDEX_NAME)='ix_cells_zoneid') THEN
   SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT='Room naming: source index absent IX_Cells_ZoneId';
  END IF;
  IF EXISTS(SELECT 1 FROM information_schema.STATISTICS WHERE TABLE_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='cells' AND LOWER(INDEX_NAME)='ix_rooms_zoneid') THEN
   SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT='Room naming: colliding target index IX_Rooms_ZoneId';
  END IF;
  IF NOT EXISTS(SELECT 1 FROM information_schema.STATISTICS WHERE TABLE_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='cells' AND LOWER(INDEX_NAME)='ux_cells_hostedvehiclecompartments') THEN
   SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT='Room naming: source index absent UX_Cells_HostedVehicleCompartments';
  END IF;
  IF EXISTS(SELECT 1 FROM information_schema.STATISTICS WHERE TABLE_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='cells' AND LOWER(INDEX_NAME)='ux_rooms_hostedvehiclecompartments') THEN
   SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT='Room naming: colliding target index UX_Rooms_HostedVehicleCompartments';
  END IF;
  IF NOT EXISTS(SELECT 1 FROM information_schema.STATISTICS WHERE TABLE_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='cells_gameitems' AND LOWER(INDEX_NAME)='fk_cells_gameitems_gameitems') THEN
   SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT='Room naming: source index absent FK_Cells_GameItems_GameItems';
  END IF;
  IF EXISTS(SELECT 1 FROM information_schema.STATISTICS WHERE TABLE_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='cells_gameitems' AND LOWER(INDEX_NAME)='fk_rooms_gameitems_gameitems') THEN
   SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT='Room naming: colliding target index FK_Rooms_GameItems_GameItems';
  END IF;
  IF NOT EXISTS(SELECT 1 FROM information_schema.STATISTICS WHERE TABLE_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='cells_magicresources' AND LOWER(INDEX_NAME)='fk_cells_magicresources_magicresources_idx') THEN
   SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT='Room naming: source index absent FK_Cells_MagicResources_MagicResources_idx';
  END IF;
  IF EXISTS(SELECT 1 FROM information_schema.STATISTICS WHERE TABLE_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='cells_magicresources' AND LOWER(INDEX_NAME)='fk_rooms_magicresources_magicresources_idx') THEN
   SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT='Room naming: colliding target index FK_Rooms_MagicResources_MagicResources_idx';
  END IF;
  IF NOT EXISTS(SELECT 1 FROM information_schema.STATISTICS WHERE TABLE_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='cells_rangedcovers' AND LOWER(INDEX_NAME)='fk_cells_rangedcovers_rangedcovers_idx') THEN
   SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT='Room naming: source index absent FK_Cells_RangedCovers_RangedCovers_idx';
  END IF;
  IF EXISTS(SELECT 1 FROM information_schema.STATISTICS WHERE TABLE_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='cells_rangedcovers' AND LOWER(INDEX_NAME)='fk_rooms_rangedcovers_rangedcovers_idx') THEN
   SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT='Room naming: colliding target index FK_Rooms_RangedCovers_RangedCovers_idx';
  END IF;
  IF NOT EXISTS(SELECT 1 FROM information_schema.STATISTICS WHERE TABLE_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='cells_tags' AND LOWER(INDEX_NAME)='fk_cells_tags_tags_idx') THEN
   SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT='Room naming: source index absent FK_Cells_Tags_Tags_idx';
  END IF;
  IF EXISTS(SELECT 1 FROM information_schema.STATISTICS WHERE TABLE_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='cells_tags' AND LOWER(INDEX_NAME)='fk_rooms_tags_tags_idx') THEN
   SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT='Room naming: colliding target index FK_Rooms_Tags_Tags_idx';
  END IF;
  IF NOT EXISTS(SELECT 1 FROM information_schema.STATISTICS WHERE TABLE_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='characterinstances' AND LOWER(INDEX_NAME)='fk_characterinstances_cells_idx') THEN
   SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT='Room naming: source index absent FK_CharacterInstances_Cells_idx';
  END IF;
  IF EXISTS(SELECT 1 FROM information_schema.STATISTICS WHERE TABLE_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='characterinstances' AND LOWER(INDEX_NAME)='fk_characterinstances_rooms_idx') THEN
   SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT='Room naming: colliding target index FK_CharacterInstances_Rooms_idx';
  END IF;
  IF NOT EXISTS(SELECT 1 FROM information_schema.STATISTICS WHERE TABLE_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='characterlog' AND LOWER(INDEX_NAME)='fk_characterlog_cells_idx') THEN
   SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT='Room naming: source index absent FK_CharacterLog_Cells_idx';
  END IF;
  IF EXISTS(SELECT 1 FROM information_schema.STATISTICS WHERE TABLE_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='characterlog' AND LOWER(INDEX_NAME)='fk_characterlog_rooms_idx') THEN
   SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT='Room naming: colliding target index FK_CharacterLog_Rooms_idx';
  END IF;
  IF NOT EXISTS(SELECT 1 FROM information_schema.STATISTICS WHERE TABLE_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='characters' AND LOWER(INDEX_NAME)='fk_characters_cells') THEN
   SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT='Room naming: source index absent FK_Characters_Cells';
  END IF;
  IF EXISTS(SELECT 1 FROM information_schema.STATISTICS WHERE TABLE_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='characters' AND LOWER(INDEX_NAME)='fk_characters_rooms') THEN
   SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT='Room naming: colliding target index FK_Characters_Rooms';
  END IF;
  IF NOT EXISTS(SELECT 1 FROM information_schema.STATISTICS WHERE TABLE_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='clans_administrationcells' AND LOWER(INDEX_NAME)='fk_clans_administrationcells_cells_idx') THEN
   SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT='Room naming: source index absent FK_Clans_AdministrationCells_Cells_idx';
  END IF;
  IF EXISTS(SELECT 1 FROM information_schema.STATISTICS WHERE TABLE_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='clans_administrationcells' AND LOWER(INDEX_NAME)='fk_clans_administrationrooms_rooms_idx') THEN
   SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT='Room naming: colliding target index FK_Clans_AdministrationRooms_Rooms_idx';
  END IF;
  IF NOT EXISTS(SELECT 1 FROM information_schema.STATISTICS WHERE TABLE_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='clans_hallcells' AND LOWER(INDEX_NAME)='fk_clans_hallcells_cells_idx') THEN
   SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT='Room naming: source index absent FK_Clans_HallCells_Cells_idx';
  END IF;
  IF EXISTS(SELECT 1 FROM information_schema.STATISTICS WHERE TABLE_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='clans_hallcells' AND LOWER(INDEX_NAME)='fk_clans_hallrooms_rooms_idx') THEN
   SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT='Room naming: colliding target index FK_Clans_HallRooms_Rooms_idx';
  END IF;
  IF NOT EXISTS(SELECT 1 FROM information_schema.STATISTICS WHERE TABLE_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='clans_treasurycells' AND LOWER(INDEX_NAME)='fk_clans_treasurycells_cells_idx') THEN
   SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT='Room naming: source index absent FK_Clans_TreasuryCells_Cells_idx';
  END IF;
  IF EXISTS(SELECT 1 FROM information_schema.STATISTICS WHERE TABLE_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='clans_treasurycells' AND LOWER(INDEX_NAME)='fk_clans_treasuryrooms_rooms_idx') THEN
   SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT='Room naming: colliding target index FK_Clans_TreasuryRooms_Rooms_idx';
  END IF;
  IF NOT EXISTS(SELECT 1 FROM information_schema.STATISTICS WHERE TABLE_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='conveyancinglocations' AND LOWER(INDEX_NAME)='ix_conveyancinglocations_cellid') THEN
   SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT='Room naming: source index absent IX_ConveyancingLocations_CellId';
  END IF;
  IF EXISTS(SELECT 1 FROM information_schema.STATISTICS WHERE TABLE_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='conveyancinglocations' AND LOWER(INDEX_NAME)='ix_conveyancinglocations_roomid') THEN
   SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT='Room naming: colliding target index IX_ConveyancingLocations_RoomId';
  END IF;
  IF NOT EXISTS(SELECT 1 FROM information_schema.STATISTICS WHERE TABLE_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='corpserecoveryreports' AND LOWER(INDEX_NAME)='ix_corpserecoveryreports_destinationcellid') THEN
   SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT='Room naming: source index absent IX_CorpseRecoveryReports_DestinationCellId';
  END IF;
  IF EXISTS(SELECT 1 FROM information_schema.STATISTICS WHERE TABLE_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='corpserecoveryreports' AND LOWER(INDEX_NAME)='ix_corpserecoveryreports_destinationroomid') THEN
   SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT='Room naming: colliding target index IX_CorpseRecoveryReports_DestinationRoomId';
  END IF;
  IF NOT EXISTS(SELECT 1 FROM information_schema.STATISTICS WHERE TABLE_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='corpserecoveryreports' AND LOWER(INDEX_NAME)='ix_corpserecoveryreports_sourcecellid') THEN
   SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT='Room naming: source index absent IX_CorpseRecoveryReports_SourceCellId';
  END IF;
  IF EXISTS(SELECT 1 FROM information_schema.STATISTICS WHERE TABLE_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='corpserecoveryreports' AND LOWER(INDEX_NAME)='ix_corpserecoveryreports_sourceroomid') THEN
   SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT='Room naming: colliding target index IX_CorpseRecoveryReports_SourceRoomId';
  END IF;
  IF NOT EXISTS(SELECT 1 FROM information_schema.STATISTICS WHERE TABLE_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='environmentalmagicoperations' AND LOWER(INDEX_NAME)='ix_environmentalmagicoperations_cellid_atutc') THEN
   SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT='Room naming: source index absent IX_EnvironmentalMagicOperations_CellId_AtUtc';
  END IF;
  IF EXISTS(SELECT 1 FROM information_schema.STATISTICS WHERE TABLE_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='environmentalmagicoperations' AND LOWER(INDEX_NAME)='ix_environmentalmagicoperations_roomid_atutc') THEN
   SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT='Room naming: colliding target index IX_EnvironmentalMagicOperations_RoomId_AtUtc';
  END IF;
  IF NOT EXISTS(SELECT 1 FROM information_schema.STATISTICS WHERE TABLE_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='hooks_perceivables' AND LOWER(INDEX_NAME)='fk_hooks_perceivables_cells_idx') THEN
   SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT='Room naming: source index absent FK_Hooks_Perceivables_Cells_idx';
  END IF;
  IF EXISTS(SELECT 1 FROM information_schema.STATISTICS WHERE TABLE_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='hooks_perceivables' AND LOWER(INDEX_NAME)='fk_hooks_perceivables_rooms_idx') THEN
   SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT='Room naming: colliding target index FK_Hooks_Perceivables_Rooms_idx';
  END IF;
  IF NOT EXISTS(SELECT 1 FROM information_schema.STATISTICS WHERE TABLE_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='hospitallocations' AND LOWER(INDEX_NAME)='fk_hospitallocations_cells_idx') THEN
   SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT='Room naming: source index absent FK_HospitalLocations_Cells_idx';
  END IF;
  IF EXISTS(SELECT 1 FROM information_schema.STATISTICS WHERE TABLE_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='hospitallocations' AND LOWER(INDEX_NAME)='fk_hospitallocations_rooms_idx') THEN
   SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT='Room naming: colliding target index FK_HospitalLocations_Rooms_idx';
  END IF;
  IF NOT EXISTS(SELECT 1 FROM information_schema.STATISTICS WHERE TABLE_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='hospitalservicerequests' AND LOWER(INDEX_NAME)='fk_hospitalservicerequests_cells_recovery_idx') THEN
   SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT='Room naming: source index absent FK_HospitalServiceRequests_Cells_Recovery_idx';
  END IF;
  IF EXISTS(SELECT 1 FROM information_schema.STATISTICS WHERE TABLE_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='hospitalservicerequests' AND LOWER(INDEX_NAME)='fk_hospitalservicerequests_rooms_recovery_idx') THEN
   SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT='Room naming: colliding target index FK_HospitalServiceRequests_Rooms_Recovery_idx';
  END IF;
  IF NOT EXISTS(SELECT 1 FROM information_schema.STATISTICS WHERE TABLE_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='hospitalservicerequests' AND LOWER(INDEX_NAME)='fk_hospitalservicerequests_cells_return_idx') THEN
   SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT='Room naming: source index absent FK_HospitalServiceRequests_Cells_Return_idx';
  END IF;
  IF EXISTS(SELECT 1 FROM information_schema.STATISTICS WHERE TABLE_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='hospitalservicerequests' AND LOWER(INDEX_NAME)='fk_hospitalservicerequests_rooms_return_idx') THEN
   SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT='Room naming: colliding target index FK_HospitalServiceRequests_Rooms_Return_idx';
  END IF;
  IF NOT EXISTS(SELECT 1 FROM information_schema.STATISTICS WHERE TABLE_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='hospitalservicerequests' AND LOWER(INDEX_NAME)='fk_hospitalservicerequests_cells_theatre_idx') THEN
   SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT='Room naming: source index absent FK_HospitalServiceRequests_Cells_Theatre_idx';
  END IF;
  IF EXISTS(SELECT 1 FROM information_schema.STATISTICS WHERE TABLE_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='hospitalservicerequests' AND LOWER(INDEX_NAME)='fk_hospitalservicerequests_rooms_theatre_idx') THEN
   SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT='Room naming: colliding target index FK_HospitalServiceRequests_Rooms_Theatre_idx';
  END IF;
  IF NOT EXISTS(SELECT 1 FROM information_schema.STATISTICS WHERE TABLE_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='hotelrooms' AND LOWER(INDEX_NAME)='fk_hotelrooms_cells_idx') THEN
   SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT='Room naming: source index absent FK_HotelRooms_Cells_idx';
  END IF;
  IF EXISTS(SELECT 1 FROM information_schema.STATISTICS WHERE TABLE_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='hotelrooms' AND LOWER(INDEX_NAME)='fk_hotelrooms_rooms_idx') THEN
   SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT='Room naming: colliding target index FK_HotelRooms_Rooms_idx';
  END IF;
  IF NOT EXISTS(SELECT 1 FROM information_schema.STATISTICS WHERE TABLE_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='hotelrooms' AND LOWER(INDEX_NAME)='ix_hotelrooms_hotel_cell') THEN
   SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT='Room naming: source index absent IX_HotelRooms_Hotel_Cell';
  END IF;
  IF EXISTS(SELECT 1 FROM information_schema.STATISTICS WHERE TABLE_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='hotelrooms' AND LOWER(INDEX_NAME)='ix_hotelrooms_hotel_room') THEN
   SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT='Room naming: colliding target index IX_HotelRooms_Hotel_Room';
  END IF;
  IF NOT EXISTS(SELECT 1 FROM information_schema.STATISTICS WHERE TABLE_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='jobfindinglocations' AND LOWER(INDEX_NAME)='ix_jobfindinglocations_cellid') THEN
   SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT='Room naming: source index absent IX_JobFindingLocations_CellId';
  END IF;
  IF EXISTS(SELECT 1 FROM information_schema.STATISTICS WHERE TABLE_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='jobfindinglocations' AND LOWER(INDEX_NAME)='ix_jobfindinglocations_roomid') THEN
   SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT='Room naming: colliding target index IX_JobFindingLocations_RoomId';
  END IF;
  IF NOT EXISTS(SELECT 1 FROM information_schema.STATISTICS WHERE TABLE_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='landrejuvenationtreatments' AND LOWER(INDEX_NAME)='ix_landrejuvenationtreatments_cellid_status') THEN
   SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT='Room naming: source index absent IX_LandRejuvenationTreatments_CellId_Status';
  END IF;
  IF EXISTS(SELECT 1 FROM information_schema.STATISTICS WHERE TABLE_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='landrejuvenationtreatments' AND LOWER(INDEX_NAME)='ix_landrejuvenationtreatments_roomid_status') THEN
   SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT='Room naming: colliding target index IX_LandRejuvenationTreatments_RoomId_Status';
  END IF;
  IF NOT EXISTS(SELECT 1 FROM information_schema.STATISTICS WHERE TABLE_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='legalauthorities' AND LOWER(INDEX_NAME)='fk_legalauthorities_marshallingcells_idx') THEN
   SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT='Room naming: source index absent FK_LegalAuthorities_MarshallingCells_idx';
  END IF;
  IF EXISTS(SELECT 1 FROM information_schema.STATISTICS WHERE TABLE_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='legalauthorities' AND LOWER(INDEX_NAME)='fk_legalauthorities_marshallingrooms_idx') THEN
   SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT='Room naming: colliding target index FK_LegalAuthorities_MarshallingRooms_idx';
  END IF;
  IF NOT EXISTS(SELECT 1 FROM information_schema.STATISTICS WHERE TABLE_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='legalauthorities' AND LOWER(INDEX_NAME)='fk_legalauthorities_preparingcells_idx') THEN
   SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT='Room naming: source index absent FK_LegalAuthorities_PreparingCells_idx';
  END IF;
  IF EXISTS(SELECT 1 FROM information_schema.STATISTICS WHERE TABLE_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='legalauthorities' AND LOWER(INDEX_NAME)='fk_legalauthorities_preparingrooms_idx') THEN
   SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT='Room naming: colliding target index FK_LegalAuthorities_PreparingRooms_idx';
  END IF;
  IF NOT EXISTS(SELECT 1 FROM information_schema.STATISTICS WHERE TABLE_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='legalauthorities' AND LOWER(INDEX_NAME)='fk_legalauthorities_prisonbelongingscells_idx') THEN
   SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT='Room naming: source index absent FK_LegalAuthorities_PrisonBelongingsCells_idx';
  END IF;
  IF EXISTS(SELECT 1 FROM information_schema.STATISTICS WHERE TABLE_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='legalauthorities' AND LOWER(INDEX_NAME)='fk_legalauthorities_prisonbelongingsrooms_idx') THEN
   SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT='Room naming: colliding target index FK_LegalAuthorities_PrisonBelongingsRooms_idx';
  END IF;
  IF NOT EXISTS(SELECT 1 FROM information_schema.STATISTICS WHERE TABLE_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='legalauthorities' AND LOWER(INDEX_NAME)='fk_legalauthorities_prisoncells_idx') THEN
   SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT='Room naming: source index absent FK_LegalAuthorities_PrisonCells_idx';
  END IF;
  IF EXISTS(SELECT 1 FROM information_schema.STATISTICS WHERE TABLE_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='legalauthorities' AND LOWER(INDEX_NAME)='fk_legalauthorities_prisonrooms_idx') THEN
   SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT='Room naming: colliding target index FK_LegalAuthorities_PrisonRooms_idx';
  END IF;
  IF NOT EXISTS(SELECT 1 FROM information_schema.STATISTICS WHERE TABLE_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='legalauthorities' AND LOWER(INDEX_NAME)='fk_legalauthorities_prisonreleasecells_idx') THEN
   SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT='Room naming: source index absent FK_LegalAuthorities_PrisonReleaseCells_idx';
  END IF;
  IF EXISTS(SELECT 1 FROM information_schema.STATISTICS WHERE TABLE_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='legalauthorities' AND LOWER(INDEX_NAME)='fk_legalauthorities_prisonreleaserooms_idx') THEN
   SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT='Room naming: colliding target index FK_LegalAuthorities_PrisonReleaseRooms_idx';
  END IF;
  IF NOT EXISTS(SELECT 1 FROM information_schema.STATISTICS WHERE TABLE_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='legalauthorities' AND LOWER(INDEX_NAME)='fk_legalauthorities_stowingcells_idx') THEN
   SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT='Room naming: source index absent FK_LegalAuthorities_StowingCells_idx';
  END IF;
  IF EXISTS(SELECT 1 FROM information_schema.STATISTICS WHERE TABLE_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='legalauthorities' AND LOWER(INDEX_NAME)='fk_legalauthorities_stowingrooms_idx') THEN
   SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT='Room naming: colliding target index FK_LegalAuthorities_StowingRooms_idx';
  END IF;
  IF NOT EXISTS(SELECT 1 FROM information_schema.STATISTICS WHERE TABLE_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='legalauthoritiycells' AND LOWER(INDEX_NAME)='fk_legalauthoritiescells_cells_idx') THEN
   SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT='Room naming: source index absent FK_LegalAuthoritiesCells_Cells_idx';
  END IF;
  IF EXISTS(SELECT 1 FROM information_schema.STATISTICS WHERE TABLE_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='legalauthoritiycells' AND LOWER(INDEX_NAME)='fk_legalauthoritiesrooms_rooms_idx') THEN
   SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT='Room naming: colliding target index FK_LegalAuthoritiesRooms_Rooms_idx';
  END IF;
  IF NOT EXISTS(SELECT 1 FROM information_schema.STATISTICS WHERE TABLE_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='legalauthoritiycells' AND LOWER(INDEX_NAME)='fk_legalauthoritiescells_legalauthorities_idx') THEN
   SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT='Room naming: source index absent FK_LegalAuthoritiesCells_LegalAuthorities_idx';
  END IF;
  IF EXISTS(SELECT 1 FROM information_schema.STATISTICS WHERE TABLE_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='legalauthoritiycells' AND LOWER(INDEX_NAME)='fk_legalauthoritiesrooms_legalauthorities_idx') THEN
   SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT='Room naming: colliding target index FK_LegalAuthoritiesRooms_LegalAuthorities_idx';
  END IF;
  IF NOT EXISTS(SELECT 1 FROM information_schema.STATISTICS WHERE TABLE_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='legalauthorityjailcells' AND LOWER(INDEX_NAME)='fk_legalauthoritiescells_cells_jail_idx') THEN
   SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT='Room naming: source index absent FK_LegalAuthoritiesCells_Cells_Jail_idx';
  END IF;
  IF EXISTS(SELECT 1 FROM information_schema.STATISTICS WHERE TABLE_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='legalauthorityjailcells' AND LOWER(INDEX_NAME)='fk_legalauthoritiesrooms_rooms_jail_idx') THEN
   SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT='Room naming: colliding target index FK_LegalAuthoritiesRooms_Rooms_Jail_idx';
  END IF;
  IF NOT EXISTS(SELECT 1 FROM information_schema.STATISTICS WHERE TABLE_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='legalauthorityjailcells' AND LOWER(INDEX_NAME)='fk_legalauthoritiescells_legalauthorities_jail_idx') THEN
   SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT='Room naming: source index absent FK_LegalAuthoritiesCells_LegalAuthorities_Jail_idx';
  END IF;
  IF EXISTS(SELECT 1 FROM information_schema.STATISTICS WHERE TABLE_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='legalauthorityjailcells' AND LOWER(INDEX_NAME)='fk_legalauthoritiesrooms_legalauthorities_jail_idx') THEN
   SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT='Room naming: colliding target index FK_LegalAuthoritiesRooms_LegalAuthorities_Jail_idx';
  END IF;
  IF NOT EXISTS(SELECT 1 FROM information_schema.STATISTICS WHERE TABLE_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='magicgatheringoperations' AND LOWER(INDEX_NAME)='ix_magicgatheringoperations_cell_source_status') THEN
   SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT='Room naming: source index absent IX_MagicGatheringOperations_Cell_Source_Status';
  END IF;
  IF EXISTS(SELECT 1 FROM information_schema.STATISTICS WHERE TABLE_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='magicgatheringoperations' AND LOWER(INDEX_NAME)='ix_magicgatheringoperations_room_source_status') THEN
   SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT='Room naming: colliding target index IX_MagicGatheringOperations_Room_Source_Status';
  END IF;
  IF NOT EXISTS(SELECT 1 FROM information_schema.STATISTICS WHERE TABLE_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='magicgatheringparticipants' AND LOWER(INDEX_NAME)='ix_magicgatheringparticipants_cell_source') THEN
   SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT='Room naming: source index absent IX_MagicGatheringParticipants_Cell_Source';
  END IF;
  IF EXISTS(SELECT 1 FROM information_schema.STATISTICS WHERE TABLE_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='magicgatheringparticipants' AND LOWER(INDEX_NAME)='ix_magicgatheringparticipants_room_source') THEN
   SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT='Room naming: colliding target index IX_MagicGatheringParticipants_Room_Source';
  END IF;
  IF NOT EXISTS(SELECT 1 FROM information_schema.STATISTICS WHERE TABLE_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='magicportalendpoints' AND LOWER(INDEX_NAME)='fk_magicportalendpoints_cells_idx') THEN
   SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT='Room naming: source index absent FK_MagicPortalEndpoints_Cells_idx';
  END IF;
  IF EXISTS(SELECT 1 FROM information_schema.STATISTICS WHERE TABLE_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='magicportalendpoints' AND LOWER(INDEX_NAME)='fk_magicportalendpoints_rooms_idx') THEN
   SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT='Room naming: colliding target index FK_MagicPortalEndpoints_Rooms_idx';
  END IF;
  IF NOT EXISTS(SELECT 1 FROM information_schema.STATISTICS WHERE TABLE_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='npcspawnercells' AND LOWER(INDEX_NAME)='ix_npcspawnercells_cellid') THEN
   SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT='Room naming: source index absent IX_NPCSpawnerCells_CellId';
  END IF;
  IF EXISTS(SELECT 1 FROM information_schema.STATISTICS WHERE TABLE_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='npcspawnercells' AND LOWER(INDEX_NAME)='ix_npcspawnerrooms_roomid') THEN
   SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT='Room naming: colliding target index IX_NPCSpawnerRooms_RoomId';
  END IF;
  IF NOT EXISTS(SELECT 1 FROM information_schema.STATISTICS WHERE TABLE_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='patrolroutesnodes' AND LOWER(INDEX_NAME)='fk_patrolroutesnodes_cells_idx') THEN
   SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT='Room naming: source index absent FK_PatrolRoutesNodes_Cells_idx';
  END IF;
  IF EXISTS(SELECT 1 FROM information_schema.STATISTICS WHERE TABLE_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='patrolroutesnodes' AND LOWER(INDEX_NAME)='fk_patrolroutesnodes_rooms_idx') THEN
   SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT='Room naming: colliding target index FK_PatrolRoutesNodes_Rooms_idx';
  END IF;
  IF NOT EXISTS(SELECT 1 FROM information_schema.STATISTICS WHERE TABLE_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='probatelocations' AND LOWER(INDEX_NAME)='ix_probatelocations_cellid') THEN
   SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT='Room naming: source index absent IX_ProbateLocations_CellId';
  END IF;
  IF EXISTS(SELECT 1 FROM information_schema.STATISTICS WHERE TABLE_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='probatelocations' AND LOWER(INDEX_NAME)='ix_probatelocations_roomid') THEN
   SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT='Room naming: colliding target index IX_ProbateLocations_RoomId';
  END IF;
  IF NOT EXISTS(SELECT 1 FROM information_schema.STATISTICS WHERE TABLE_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='propertylocations' AND LOWER(INDEX_NAME)='ix_propertylocations_cellid') THEN
   SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT='Room naming: source index absent IX_PropertyLocations_CellId';
  END IF;
  IF EXISTS(SELECT 1 FROM information_schema.STATISTICS WHERE TABLE_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='propertylocations' AND LOWER(INDEX_NAME)='ix_propertylocations_roomid') THEN
   SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT='Room naming: colliding target index IX_PropertyLocations_RoomId';
  END IF;
  IF NOT EXISTS(SELECT 1 FROM information_schema.STATISTICS WHERE TABLE_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='restaurantcells' AND LOWER(INDEX_NAME)='ix_restaurantcells_cell') THEN
   SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT='Room naming: source index absent IX_RestaurantCells_Cell';
  END IF;
  IF EXISTS(SELECT 1 FROM information_schema.STATISTICS WHERE TABLE_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='restaurantcells' AND LOWER(INDEX_NAME)='ix_restaurantrooms_room') THEN
   SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT='Room naming: colliding target index IX_RestaurantRooms_Room';
  END IF;
  IF NOT EXISTS(SELECT 1 FROM information_schema.STATISTICS WHERE TABLE_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='restaurantcells' AND LOWER(INDEX_NAME)='ix_restaurantcells_restaurant_role') THEN
   SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT='Room naming: source index absent IX_RestaurantCells_Restaurant_Role';
  END IF;
  IF EXISTS(SELECT 1 FROM information_schema.STATISTICS WHERE TABLE_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='restaurantcells' AND LOWER(INDEX_NAME)='ix_restaurantrooms_restaurant_role') THEN
   SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT='Room naming: colliding target index IX_RestaurantRooms_Restaurant_Role';
  END IF;
  IF NOT EXISTS(SELECT 1 FROM information_schema.STATISTICS WHERE TABLE_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='routecelllandmarks' AND LOWER(INDEX_NAME)='ix_routecelllandmarks_routecell_position') THEN
   SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT='Room naming: source index absent IX_RouteCellLandmarks_RouteCell_Position';
  END IF;
  IF EXISTS(SELECT 1 FROM information_schema.STATISTICS WHERE TABLE_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='routecelllandmarks' AND LOWER(INDEX_NAME)='ix_routeroomlandmarks_routeroom_position') THEN
   SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT='Room naming: colliding target index IX_RouteRoomLandmarks_RouteRoom_Position';
  END IF;
  IF NOT EXISTS(SELECT 1 FROM information_schema.STATISTICS WHERE TABLE_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='routeexitanchors' AND LOWER(INDEX_NAME)='ix_routeexitanchors_routecell_band') THEN
   SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT='Room naming: source index absent IX_RouteExitAnchors_RouteCell_Band';
  END IF;
  IF EXISTS(SELECT 1 FROM information_schema.STATISTICS WHERE TABLE_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='routeexitanchors' AND LOWER(INDEX_NAME)='ix_routeexitanchors_routeroom_band') THEN
   SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT='Room naming: colliding target index IX_RouteExitAnchors_RouteRoom_Band';
  END IF;
  IF NOT EXISTS(SELECT 1 FROM information_schema.STATISTICS WHERE TABLE_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='shops' AND LOWER(INDEX_NAME)='fk_shops_cells_stockroom_idx') THEN
   SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT='Room naming: source index absent FK_Shops_Cells_Stockroom_idx';
  END IF;
  IF EXISTS(SELECT 1 FROM information_schema.STATISTICS WHERE TABLE_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='shops' AND LOWER(INDEX_NAME)='fk_shops_rooms_stockroom_idx') THEN
   SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT='Room naming: colliding target index FK_Shops_Rooms_Stockroom_idx';
  END IF;
  IF NOT EXISTS(SELECT 1 FROM information_schema.STATISTICS WHERE TABLE_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='shops' AND LOWER(INDEX_NAME)='fk_shops_cells_workshop_idx') THEN
   SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT='Room naming: source index absent FK_Shops_Cells_Workshop_idx';
  END IF;
  IF EXISTS(SELECT 1 FROM information_schema.STATISTICS WHERE TABLE_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='shops' AND LOWER(INDEX_NAME)='fk_shops_rooms_workshop_idx') THEN
   SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT='Room naming: colliding target index FK_Shops_Rooms_Workshop_idx';
  END IF;
  IF NOT EXISTS(SELECT 1 FROM information_schema.STATISTICS WHERE TABLE_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='shops_storeroomcells' AND LOWER(INDEX_NAME)='fk_shops_storeroomcells_cells_idx') THEN
   SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT='Room naming: source index absent FK_Shops_StoreroomCells_Cells_idx';
  END IF;
  IF EXISTS(SELECT 1 FROM information_schema.STATISTICS WHERE TABLE_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='shops_storeroomcells' AND LOWER(INDEX_NAME)='fk_shops_storeroomrooms_rooms_idx') THEN
   SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT='Room naming: colliding target index FK_Shops_StoreroomRooms_Rooms_idx';
  END IF;
  IF NOT EXISTS(SELECT 1 FROM information_schema.STATISTICS WHERE TABLE_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='stables' AND LOWER(INDEX_NAME)='fk_stables_cells_idx') THEN
   SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT='Room naming: source index absent FK_Stables_Cells_idx';
  END IF;
  IF EXISTS(SELECT 1 FROM information_schema.STATISTICS WHERE TABLE_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='stables' AND LOWER(INDEX_NAME)='fk_stables_rooms_idx') THEN
   SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT='Room naming: colliding target index FK_Stables_Rooms_idx';
  END IF;
  IF NOT EXISTS(SELECT 1 FROM information_schema.STATISTICS WHERE TABLE_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='tracks' AND LOWER(INDEX_NAME)='ix_tracks_cell_layer_routeposition') THEN
   SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT='Room naming: source index absent IX_Tracks_Cell_Layer_RoutePosition';
  END IF;
  IF EXISTS(SELECT 1 FROM information_schema.STATISTICS WHERE TABLE_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='tracks' AND LOWER(INDEX_NAME)='ix_tracks_room_layer_routeposition') THEN
   SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT='Room naming: colliding target index IX_Tracks_Room_Layer_RoutePosition';
  END IF;
  IF NOT EXISTS(SELECT 1 FROM information_schema.STATISTICS WHERE TABLE_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='vehiclecompartments' AND LOWER(INDEX_NAME)='ux_vehiclecompartments_interiorcell') THEN
   SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT='Room naming: source index absent UX_VehicleCompartments_InteriorCell';
  END IF;
  IF EXISTS(SELECT 1 FROM information_schema.STATISTICS WHERE TABLE_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='vehiclecompartments' AND LOWER(INDEX_NAME)='ux_vehiclecompartments_interiorroom') THEN
   SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT='Room naming: colliding target index UX_VehicleCompartments_InteriorRoom';
  END IF;
  IF NOT EXISTS(SELECT 1 FROM information_schema.STATISTICS WHERE TABLE_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='vehicledockings' AND LOWER(INDEX_NAME)='ix_vehicledockings_exteriorcell_layer') THEN
   SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT='Room naming: source index absent IX_VehicleDockings_ExteriorCell_Layer';
  END IF;
  IF EXISTS(SELECT 1 FROM information_schema.STATISTICS WHERE TABLE_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='vehicledockings' AND LOWER(INDEX_NAME)='ix_vehicledockings_exteriorroom_layer') THEN
   SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT='Room naming: colliding target index IX_VehicleDockings_ExteriorRoom_Layer';
  END IF;
  IF NOT EXISTS(SELECT 1 FROM information_schema.STATISTICS WHERE TABLE_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='vehiclerouteplatformbindings' AND LOWER(INDEX_NAME)='fk_vehiclerouteplatformbindings_cells_idx') THEN
   SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT='Room naming: source index absent FK_VehicleRoutePlatformBindings_Cells_idx';
  END IF;
  IF EXISTS(SELECT 1 FROM information_schema.STATISTICS WHERE TABLE_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='vehiclerouteplatformbindings' AND LOWER(INDEX_NAME)='fk_vehiclerouteplatformbindings_rooms_idx') THEN
   SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT='Room naming: colliding target index FK_VehicleRoutePlatformBindings_Rooms_idx';
  END IF;
  IF NOT EXISTS(SELECT 1 FROM information_schema.STATISTICS WHERE TABLE_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='vehicleroutesteps' AND LOWER(INDEX_NAME)='fk_vehicleroutesteps_destinationcells_idx') THEN
   SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT='Room naming: source index absent FK_VehicleRouteSteps_DestinationCells_idx';
  END IF;
  IF EXISTS(SELECT 1 FROM information_schema.STATISTICS WHERE TABLE_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='vehicleroutesteps' AND LOWER(INDEX_NAME)='fk_vehicleroutesteps_destinationrooms_idx') THEN
   SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT='Room naming: colliding target index FK_VehicleRouteSteps_DestinationRooms_idx';
  END IF;
  IF NOT EXISTS(SELECT 1 FROM information_schema.STATISTICS WHERE TABLE_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='vehicleroutesteps' AND LOWER(INDEX_NAME)='fk_vehicleroutesteps_origincells_idx') THEN
   SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT='Room naming: source index absent FK_VehicleRouteSteps_OriginCells_idx';
  END IF;
  IF EXISTS(SELECT 1 FROM information_schema.STATISTICS WHERE TABLE_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='vehicleroutesteps' AND LOWER(INDEX_NAME)='fk_vehicleroutesteps_originrooms_idx') THEN
   SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT='Room naming: colliding target index FK_VehicleRouteSteps_OriginRooms_idx';
  END IF;
  IF NOT EXISTS(SELECT 1 FROM information_schema.STATISTICS WHERE TABLE_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='vehicleroutestops' AND LOWER(INDEX_NAME)='fk_vehicleroutestops_cells_idx') THEN
   SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT='Room naming: source index absent FK_VehicleRouteStops_Cells_idx';
  END IF;
  IF EXISTS(SELECT 1 FROM information_schema.STATISTICS WHERE TABLE_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='vehicleroutestops' AND LOWER(INDEX_NAME)='fk_vehicleroutestops_rooms_idx') THEN
   SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT='Room naming: colliding target index FK_VehicleRouteStops_Rooms_idx';
  END IF;
  IF NOT EXISTS(SELECT 1 FROM information_schema.STATISTICS WHERE TABLE_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='vehicleroutetopologypins' AND LOWER(INDEX_NAME)='fk_vehicleroutetopologypins_routecells_idx') THEN
   SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT='Room naming: source index absent FK_VehicleRouteTopologyPins_RouteCells_idx';
  END IF;
  IF EXISTS(SELECT 1 FROM information_schema.STATISTICS WHERE TABLE_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='vehicleroutetopologypins' AND LOWER(INDEX_NAME)='fk_vehicleroutetopologypins_routerooms_idx') THEN
   SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT='Room naming: colliding target index FK_VehicleRouteTopologyPins_RouteRooms_idx';
  END IF;
  IF NOT EXISTS(SELECT 1 FROM information_schema.STATISTICS WHERE TABLE_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='vehicles' AND LOWER(INDEX_NAME)='fk_vehicles_cells_current_idx') THEN
   SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT='Room naming: source index absent FK_Vehicles_Cells_Current_idx';
  END IF;
  IF EXISTS(SELECT 1 FROM information_schema.STATISTICS WHERE TABLE_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='vehicles' AND LOWER(INDEX_NAME)='fk_vehicles_rooms_current_idx') THEN
   SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT='Room naming: colliding target index FK_Vehicles_Rooms_Current_idx';
  END IF;
  IF NOT EXISTS(SELECT 1 FROM information_schema.STATISTICS WHERE TABLE_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='vehicles' AND LOWER(INDEX_NAME)='fk_vehicles_cells_destination_idx') THEN
   SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT='Room naming: source index absent FK_Vehicles_Cells_Destination_idx';
  END IF;
  IF EXISTS(SELECT 1 FROM information_schema.STATISTICS WHERE TABLE_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='vehicles' AND LOWER(INDEX_NAME)='fk_vehicles_rooms_destination_idx') THEN
   SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT='Room naming: colliding target index FK_Vehicles_Rooms_Destination_idx';
  END IF;
  IF NOT EXISTS(SELECT 1 FROM information_schema.STATISTICS WHERE TABLE_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='vehicles' AND LOWER(INDEX_NAME)='ix_vehicles_cell_layer_routeposition') THEN
   SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT='Room naming: source index absent IX_Vehicles_Cell_Layer_RoutePosition';
  END IF;
  IF EXISTS(SELECT 1 FROM information_schema.STATISTICS WHERE TABLE_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='vehicles' AND LOWER(INDEX_NAME)='ix_vehicles_room_layer_routeposition') THEN
   SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT='Room naming: colliding target index IX_Vehicles_Room_Layer_RoutePosition';
  END IF;
  IF NOT EXISTS(SELECT 1 FROM information_schema.STATISTICS WHERE TABLE_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='zones' AND LOWER(INDEX_NAME)='fk_zones_cells') THEN
   SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT='Room naming: source index absent FK_Zones_Cells';
  END IF;
  IF EXISTS(SELECT 1 FROM information_schema.STATISTICS WHERE TABLE_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='zones' AND LOWER(INDEX_NAME)='fk_zones_rooms') THEN
   SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT='Room naming: colliding target index FK_Zones_Rooms';
  END IF;
  IF (SELECT COUNT(*) FROM information_schema.KEY_COLUMN_USAGE WHERE TABLE_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='aistorytellersituations' AND LOWER(CONSTRAINT_NAME)='fk_aistorytellersituations_cells_scoperoomid')<>1 OR (SELECT COUNT(*) FROM information_schema.KEY_COLUMN_USAGE WHERE TABLE_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='aistorytellersituations' AND LOWER(CONSTRAINT_NAME)='fk_aistorytellersituations_cells_scoperoomid' AND ((ORDINAL_POSITION=1 AND LOWER(COLUMN_NAME)='scoperoomid' AND REFERENCED_TABLE_SCHEMA=DATABASE() AND LOWER(REFERENCED_TABLE_NAME)='cells' AND LOWER(REFERENCED_COLUMN_NAME)='id')))<>1 OR NOT EXISTS(SELECT 1 FROM information_schema.REFERENTIAL_CONSTRAINTS WHERE CONSTRAINT_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='aistorytellersituations' AND LOWER(CONSTRAINT_NAME)='fk_aistorytellersituations_cells_scoperoomid' AND DELETE_RULE IN ('NO ACTION','RESTRICT') AND UPDATE_RULE IN ('NO ACTION','RESTRICT')) THEN
   SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT='Room naming: foreign-key shape differs FK_AIStorytellerSituations_Cells_ScopeRoomId';
  END IF;
  IF EXISTS(SELECT 1 FROM information_schema.TABLE_CONSTRAINTS WHERE CONSTRAINT_SCHEMA=DATABASE() AND LOWER(CONSTRAINT_NAME)='fk_aistorytellersituations_rooms_scoperoomid' AND CONSTRAINT_TYPE='FOREIGN KEY') THEN
   SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT='Room naming: colliding target foreign key FK_AIStorytellerSituations_Rooms_ScopeRoomId';
  END IF;
  IF (SELECT COUNT(*) FROM information_schema.KEY_COLUMN_USAGE WHERE TABLE_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='activeprojects' AND LOWER(CONSTRAINT_NAME)='fk_activeprojects_cells')<>1 OR (SELECT COUNT(*) FROM information_schema.KEY_COLUMN_USAGE WHERE TABLE_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='activeprojects' AND LOWER(CONSTRAINT_NAME)='fk_activeprojects_cells' AND ((ORDINAL_POSITION=1 AND LOWER(COLUMN_NAME)='cellid' AND REFERENCED_TABLE_SCHEMA=DATABASE() AND LOWER(REFERENCED_TABLE_NAME)='cells' AND LOWER(REFERENCED_COLUMN_NAME)='id')))<>1 OR NOT EXISTS(SELECT 1 FROM information_schema.REFERENTIAL_CONSTRAINTS WHERE CONSTRAINT_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='activeprojects' AND LOWER(CONSTRAINT_NAME)='fk_activeprojects_cells' AND DELETE_RULE IN ('NO ACTION','RESTRICT') AND UPDATE_RULE IN ('NO ACTION','RESTRICT','CASCADE')) THEN
   SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT='Room naming: foreign-key shape differs FK_ActiveProjects_Cells';
  END IF;
  IF EXISTS(SELECT 1 FROM information_schema.TABLE_CONSTRAINTS WHERE CONSTRAINT_SCHEMA=DATABASE() AND LOWER(CONSTRAINT_NAME)='fk_activeprojects_rooms' AND CONSTRAINT_TYPE='FOREIGN KEY') THEN
   SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT='Room naming: colliding target foreign key FK_ActiveProjects_Rooms';
  END IF;
  IF (SELECT COUNT(*) FROM information_schema.KEY_COLUMN_USAGE WHERE TABLE_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='activeroutemotions' AND LOWER(CONSTRAINT_NAME)='fk_activeroutemotions_routecells')<>1 OR (SELECT COUNT(*) FROM information_schema.KEY_COLUMN_USAGE WHERE TABLE_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='activeroutemotions' AND LOWER(CONSTRAINT_NAME)='fk_activeroutemotions_routecells' AND ((ORDINAL_POSITION=1 AND LOWER(COLUMN_NAME)='routecellid' AND REFERENCED_TABLE_SCHEMA=DATABASE() AND LOWER(REFERENCED_TABLE_NAME)='routecells' AND LOWER(REFERENCED_COLUMN_NAME)='cellid')))<>1 OR NOT EXISTS(SELECT 1 FROM information_schema.REFERENTIAL_CONSTRAINTS WHERE CONSTRAINT_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='activeroutemotions' AND LOWER(CONSTRAINT_NAME)='fk_activeroutemotions_routecells' AND DELETE_RULE IN ('CASCADE') AND UPDATE_RULE IN ('NO ACTION','RESTRICT')) THEN
   SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT='Room naming: foreign-key shape differs FK_ActiveRouteMotions_RouteCells';
  END IF;
  IF EXISTS(SELECT 1 FROM information_schema.TABLE_CONSTRAINTS WHERE CONSTRAINT_SCHEMA=DATABASE() AND LOWER(CONSTRAINT_NAME)='fk_activeroutemotions_routerooms' AND CONSTRAINT_TYPE='FOREIGN KEY') THEN
   SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT='Room naming: colliding target foreign key FK_ActiveRouteMotions_RouteRooms';
  END IF;
  IF (SELECT COUNT(*) FROM information_schema.KEY_COLUMN_USAGE WHERE TABLE_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='agriculturefields' AND LOWER(CONSTRAINT_NAME)='fk_agriculturefields_cells')<>1 OR (SELECT COUNT(*) FROM information_schema.KEY_COLUMN_USAGE WHERE TABLE_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='agriculturefields' AND LOWER(CONSTRAINT_NAME)='fk_agriculturefields_cells' AND ((ORDINAL_POSITION=1 AND LOWER(COLUMN_NAME)='cellid' AND REFERENCED_TABLE_SCHEMA=DATABASE() AND LOWER(REFERENCED_TABLE_NAME)='cells' AND LOWER(REFERENCED_COLUMN_NAME)='id')))<>1 OR NOT EXISTS(SELECT 1 FROM information_schema.REFERENTIAL_CONSTRAINTS WHERE CONSTRAINT_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='agriculturefields' AND LOWER(CONSTRAINT_NAME)='fk_agriculturefields_cells' AND DELETE_RULE IN ('CASCADE') AND UPDATE_RULE IN ('NO ACTION','RESTRICT')) THEN
   SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT='Room naming: foreign-key shape differs FK_AgricultureFields_Cells';
  END IF;
  IF EXISTS(SELECT 1 FROM information_schema.TABLE_CONSTRAINTS WHERE CONSTRAINT_SCHEMA=DATABASE() AND LOWER(CONSTRAINT_NAME)='fk_agriculturefields_rooms' AND CONSTRAINT_TYPE='FOREIGN KEY') THEN
   SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT='Room naming: colliding target foreign key FK_AgricultureFields_Rooms';
  END IF;
  IF (SELECT COUNT(*) FROM information_schema.KEY_COLUMN_USAGE WHERE TABLE_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='areas_cells' AND LOWER(CONSTRAINT_NAME)='fk_areas_cells_areas')<>1 OR (SELECT COUNT(*) FROM information_schema.KEY_COLUMN_USAGE WHERE TABLE_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='areas_cells' AND LOWER(CONSTRAINT_NAME)='fk_areas_cells_areas' AND ((ORDINAL_POSITION=1 AND LOWER(COLUMN_NAME)='areaid' AND REFERENCED_TABLE_SCHEMA=DATABASE() AND LOWER(REFERENCED_TABLE_NAME)='areas' AND LOWER(REFERENCED_COLUMN_NAME)='id')))<>1 OR NOT EXISTS(SELECT 1 FROM information_schema.REFERENTIAL_CONSTRAINTS WHERE CONSTRAINT_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='areas_cells' AND LOWER(CONSTRAINT_NAME)='fk_areas_cells_areas' AND DELETE_RULE IN ('CASCADE') AND UPDATE_RULE IN ('NO ACTION','RESTRICT')) THEN
   SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT='Room naming: foreign-key shape differs FK_Areas_Cells_Areas';
  END IF;
  IF EXISTS(SELECT 1 FROM information_schema.TABLE_CONSTRAINTS WHERE CONSTRAINT_SCHEMA=DATABASE() AND LOWER(CONSTRAINT_NAME)='fk_areas_rooms_areas' AND CONSTRAINT_TYPE='FOREIGN KEY') THEN
   SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT='Room naming: colliding target foreign key FK_Areas_Rooms_Areas';
  END IF;
  IF (SELECT COUNT(*) FROM information_schema.KEY_COLUMN_USAGE WHERE TABLE_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='areas_cells' AND LOWER(CONSTRAINT_NAME)='fk_areas_cells_cells')<>1 OR (SELECT COUNT(*) FROM information_schema.KEY_COLUMN_USAGE WHERE TABLE_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='areas_cells' AND LOWER(CONSTRAINT_NAME)='fk_areas_cells_cells' AND ((ORDINAL_POSITION=1 AND LOWER(COLUMN_NAME)='cellid' AND REFERENCED_TABLE_SCHEMA=DATABASE() AND LOWER(REFERENCED_TABLE_NAME)='cells' AND LOWER(REFERENCED_COLUMN_NAME)='id')))<>1 OR NOT EXISTS(SELECT 1 FROM information_schema.REFERENTIAL_CONSTRAINTS WHERE CONSTRAINT_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='areas_cells' AND LOWER(CONSTRAINT_NAME)='fk_areas_cells_cells' AND DELETE_RULE IN ('CASCADE') AND UPDATE_RULE IN ('NO ACTION','RESTRICT')) THEN
   SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT='Room naming: foreign-key shape differs FK_Areas_Cells_Cells';
  END IF;
  IF EXISTS(SELECT 1 FROM information_schema.TABLE_CONSTRAINTS WHERE CONSTRAINT_SCHEMA=DATABASE() AND LOWER(CONSTRAINT_NAME)='fk_areas_rooms_rooms' AND CONSTRAINT_TYPE='FOREIGN KEY') THEN
   SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT='Room naming: colliding target foreign key FK_Areas_Rooms_Rooms';
  END IF;
  IF (SELECT COUNT(*) FROM information_schema.KEY_COLUMN_USAGE WHERE TABLE_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='arenacells' AND LOWER(CONSTRAINT_NAME)='fk_arenacells_arenas')<>1 OR (SELECT COUNT(*) FROM information_schema.KEY_COLUMN_USAGE WHERE TABLE_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='arenacells' AND LOWER(CONSTRAINT_NAME)='fk_arenacells_arenas' AND ((ORDINAL_POSITION=1 AND LOWER(COLUMN_NAME)='arenaid' AND REFERENCED_TABLE_SCHEMA=DATABASE() AND LOWER(REFERENCED_TABLE_NAME)='arenas' AND LOWER(REFERENCED_COLUMN_NAME)='id')))<>1 OR NOT EXISTS(SELECT 1 FROM information_schema.REFERENTIAL_CONSTRAINTS WHERE CONSTRAINT_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='arenacells' AND LOWER(CONSTRAINT_NAME)='fk_arenacells_arenas' AND DELETE_RULE IN ('CASCADE') AND UPDATE_RULE IN ('NO ACTION','RESTRICT')) THEN
   SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT='Room naming: foreign-key shape differs FK_ArenaCells_Arenas';
  END IF;
  IF EXISTS(SELECT 1 FROM information_schema.TABLE_CONSTRAINTS WHERE CONSTRAINT_SCHEMA=DATABASE() AND LOWER(CONSTRAINT_NAME)='fk_arenarooms_arenas' AND CONSTRAINT_TYPE='FOREIGN KEY') THEN
   SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT='Room naming: colliding target foreign key FK_ArenaRooms_Arenas';
  END IF;
  IF (SELECT COUNT(*) FROM information_schema.KEY_COLUMN_USAGE WHERE TABLE_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='arenacells' AND LOWER(CONSTRAINT_NAME)='fk_arenacells_cells')<>1 OR (SELECT COUNT(*) FROM information_schema.KEY_COLUMN_USAGE WHERE TABLE_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='arenacells' AND LOWER(CONSTRAINT_NAME)='fk_arenacells_cells' AND ((ORDINAL_POSITION=1 AND LOWER(COLUMN_NAME)='cellid' AND REFERENCED_TABLE_SCHEMA=DATABASE() AND LOWER(REFERENCED_TABLE_NAME)='cells' AND LOWER(REFERENCED_COLUMN_NAME)='id')))<>1 OR NOT EXISTS(SELECT 1 FROM information_schema.REFERENTIAL_CONSTRAINTS WHERE CONSTRAINT_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='arenacells' AND LOWER(CONSTRAINT_NAME)='fk_arenacells_cells' AND DELETE_RULE IN ('CASCADE') AND UPDATE_RULE IN ('NO ACTION','RESTRICT')) THEN
   SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT='Room naming: foreign-key shape differs FK_ArenaCells_Cells';
  END IF;
  IF EXISTS(SELECT 1 FROM information_schema.TABLE_CONSTRAINTS WHERE CONSTRAINT_SCHEMA=DATABASE() AND LOWER(CONSTRAINT_NAME)='fk_arenarooms_rooms' AND CONSTRAINT_TYPE='FOREIGN KEY') THEN
   SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT='Room naming: colliding target foreign key FK_ArenaRooms_Rooms';
  END IF;
  IF (SELECT COUNT(*) FROM information_schema.KEY_COLUMN_USAGE WHERE TABLE_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='auctionhouses' AND LOWER(CONSTRAINT_NAME)='fk_auctionhouses_cells')<>1 OR (SELECT COUNT(*) FROM information_schema.KEY_COLUMN_USAGE WHERE TABLE_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='auctionhouses' AND LOWER(CONSTRAINT_NAME)='fk_auctionhouses_cells' AND ((ORDINAL_POSITION=1 AND LOWER(COLUMN_NAME)='auctionhousecellid' AND REFERENCED_TABLE_SCHEMA=DATABASE() AND LOWER(REFERENCED_TABLE_NAME)='cells' AND LOWER(REFERENCED_COLUMN_NAME)='id')))<>1 OR NOT EXISTS(SELECT 1 FROM information_schema.REFERENTIAL_CONSTRAINTS WHERE CONSTRAINT_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='auctionhouses' AND LOWER(CONSTRAINT_NAME)='fk_auctionhouses_cells' AND DELETE_RULE IN ('CASCADE') AND UPDATE_RULE IN ('NO ACTION','RESTRICT')) THEN
   SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT='Room naming: foreign-key shape differs FK_AuctionHouses_Cells';
  END IF;
  IF EXISTS(SELECT 1 FROM information_schema.TABLE_CONSTRAINTS WHERE CONSTRAINT_SCHEMA=DATABASE() AND LOWER(CONSTRAINT_NAME)='fk_auctionhouses_rooms' AND CONSTRAINT_TYPE='FOREIGN KEY') THEN
   SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT='Room naming: colliding target foreign key FK_AuctionHouses_Rooms';
  END IF;
  IF (SELECT COUNT(*) FROM information_schema.KEY_COLUMN_USAGE WHERE TABLE_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='bankbranches' AND LOWER(CONSTRAINT_NAME)='fk_bankbranches_cells')<>1 OR (SELECT COUNT(*) FROM information_schema.KEY_COLUMN_USAGE WHERE TABLE_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='bankbranches' AND LOWER(CONSTRAINT_NAME)='fk_bankbranches_cells' AND ((ORDINAL_POSITION=1 AND LOWER(COLUMN_NAME)='cellid' AND REFERENCED_TABLE_SCHEMA=DATABASE() AND LOWER(REFERENCED_TABLE_NAME)='cells' AND LOWER(REFERENCED_COLUMN_NAME)='id')))<>1 OR NOT EXISTS(SELECT 1 FROM information_schema.REFERENTIAL_CONSTRAINTS WHERE CONSTRAINT_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='bankbranches' AND LOWER(CONSTRAINT_NAME)='fk_bankbranches_cells' AND DELETE_RULE IN ('CASCADE') AND UPDATE_RULE IN ('NO ACTION','RESTRICT')) THEN
   SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT='Room naming: foreign-key shape differs FK_BankBranches_Cells';
  END IF;
  IF EXISTS(SELECT 1 FROM information_schema.TABLE_CONSTRAINTS WHERE CONSTRAINT_SCHEMA=DATABASE() AND LOWER(CONSTRAINT_NAME)='fk_bankbranches_rooms' AND CONSTRAINT_TYPE='FOREIGN KEY') THEN
   SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT='Room naming: colliding target foreign key FK_BankBranches_Rooms';
  END IF;
  IF (SELECT COUNT(*) FROM information_schema.KEY_COLUMN_USAGE WHERE TABLE_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='cellenvironmentalstates' AND LOWER(CONSTRAINT_NAME)='fk_cellenvironmentalstates_cells')<>1 OR (SELECT COUNT(*) FROM information_schema.KEY_COLUMN_USAGE WHERE TABLE_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='cellenvironmentalstates' AND LOWER(CONSTRAINT_NAME)='fk_cellenvironmentalstates_cells' AND ((ORDINAL_POSITION=1 AND LOWER(COLUMN_NAME)='cellid' AND REFERENCED_TABLE_SCHEMA=DATABASE() AND LOWER(REFERENCED_TABLE_NAME)='cells' AND LOWER(REFERENCED_COLUMN_NAME)='id')))<>1 OR NOT EXISTS(SELECT 1 FROM information_schema.REFERENTIAL_CONSTRAINTS WHERE CONSTRAINT_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='cellenvironmentalstates' AND LOWER(CONSTRAINT_NAME)='fk_cellenvironmentalstates_cells' AND DELETE_RULE IN ('CASCADE') AND UPDATE_RULE IN ('NO ACTION','RESTRICT')) THEN
   SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT='Room naming: foreign-key shape differs FK_CellEnvironmentalStates_Cells';
  END IF;
  IF EXISTS(SELECT 1 FROM information_schema.TABLE_CONSTRAINTS WHERE CONSTRAINT_SCHEMA=DATABASE() AND LOWER(CONSTRAINT_NAME)='fk_roomenvironmentalstates_rooms' AND CONSTRAINT_TYPE='FOREIGN KEY') THEN
   SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT='Room naming: colliding target foreign key FK_RoomEnvironmentalStates_Rooms';
  END IF;
  IF (SELECT COUNT(*) FROM information_schema.KEY_COLUMN_USAGE WHERE TABLE_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='celloverlaypackages' AND LOWER(CONSTRAINT_NAME)='fk_celloverlaypackages_editableitems')<>1 OR (SELECT COUNT(*) FROM information_schema.KEY_COLUMN_USAGE WHERE TABLE_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='celloverlaypackages' AND LOWER(CONSTRAINT_NAME)='fk_celloverlaypackages_editableitems' AND ((ORDINAL_POSITION=1 AND LOWER(COLUMN_NAME)='editableitemid' AND REFERENCED_TABLE_SCHEMA=DATABASE() AND LOWER(REFERENCED_TABLE_NAME)='editableitems' AND LOWER(REFERENCED_COLUMN_NAME)='id')))<>1 OR NOT EXISTS(SELECT 1 FROM information_schema.REFERENTIAL_CONSTRAINTS WHERE CONSTRAINT_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='celloverlaypackages' AND LOWER(CONSTRAINT_NAME)='fk_celloverlaypackages_editableitems' AND DELETE_RULE IN ('NO ACTION','RESTRICT') AND UPDATE_RULE IN ('NO ACTION','RESTRICT')) THEN
   SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT='Room naming: foreign-key shape differs FK_CellOverlayPackages_EditableItems';
  END IF;
  IF EXISTS(SELECT 1 FROM information_schema.TABLE_CONSTRAINTS WHERE CONSTRAINT_SCHEMA=DATABASE() AND LOWER(CONSTRAINT_NAME)='fk_roomoverlaypackages_editableitems' AND CONSTRAINT_TYPE='FOREIGN KEY') THEN
   SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT='Room naming: colliding target foreign key FK_RoomOverlayPackages_EditableItems';
  END IF;
  IF (SELECT COUNT(*) FROM information_schema.KEY_COLUMN_USAGE WHERE TABLE_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='celloverlays' AND LOWER(CONSTRAINT_NAME)='fk_celloverlays_celloverlaypackages')<>2 OR (SELECT COUNT(*) FROM information_schema.KEY_COLUMN_USAGE WHERE TABLE_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='celloverlays' AND LOWER(CONSTRAINT_NAME)='fk_celloverlays_celloverlaypackages' AND ((ORDINAL_POSITION=1 AND LOWER(COLUMN_NAME)='celloverlaypackageid' AND REFERENCED_TABLE_SCHEMA=DATABASE() AND LOWER(REFERENCED_TABLE_NAME)='celloverlaypackages' AND LOWER(REFERENCED_COLUMN_NAME)='id') OR (ORDINAL_POSITION=2 AND LOWER(COLUMN_NAME)='celloverlaypackagerevisionnumber' AND REFERENCED_TABLE_SCHEMA=DATABASE() AND LOWER(REFERENCED_TABLE_NAME)='celloverlaypackages' AND LOWER(REFERENCED_COLUMN_NAME)='revisionnumber')))<>2 OR NOT EXISTS(SELECT 1 FROM information_schema.REFERENTIAL_CONSTRAINTS WHERE CONSTRAINT_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='celloverlays' AND LOWER(CONSTRAINT_NAME)='fk_celloverlays_celloverlaypackages' AND DELETE_RULE IN ('CASCADE') AND UPDATE_RULE IN ('NO ACTION','RESTRICT')) THEN
   SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT='Room naming: foreign-key shape differs FK_CellOverlays_CellOverlayPackages';
  END IF;
  IF EXISTS(SELECT 1 FROM information_schema.TABLE_CONSTRAINTS WHERE CONSTRAINT_SCHEMA=DATABASE() AND LOWER(CONSTRAINT_NAME)='fk_roomoverlays_roomoverlaypackages' AND CONSTRAINT_TYPE='FOREIGN KEY') THEN
   SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT='Room naming: colliding target foreign key FK_RoomOverlays_RoomOverlayPackages';
  END IF;
  IF (SELECT COUNT(*) FROM information_schema.KEY_COLUMN_USAGE WHERE TABLE_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='celloverlays' AND LOWER(CONSTRAINT_NAME)='fk_celloverlays_cells')<>1 OR (SELECT COUNT(*) FROM information_schema.KEY_COLUMN_USAGE WHERE TABLE_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='celloverlays' AND LOWER(CONSTRAINT_NAME)='fk_celloverlays_cells' AND ((ORDINAL_POSITION=1 AND LOWER(COLUMN_NAME)='cellid' AND REFERENCED_TABLE_SCHEMA=DATABASE() AND LOWER(REFERENCED_TABLE_NAME)='cells' AND LOWER(REFERENCED_COLUMN_NAME)='id')))<>1 OR NOT EXISTS(SELECT 1 FROM information_schema.REFERENTIAL_CONSTRAINTS WHERE CONSTRAINT_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='celloverlays' AND LOWER(CONSTRAINT_NAME)='fk_celloverlays_cells' AND DELETE_RULE IN ('CASCADE') AND UPDATE_RULE IN ('NO ACTION','RESTRICT')) THEN
   SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT='Room naming: foreign-key shape differs FK_CellOverlays_Cells';
  END IF;
  IF EXISTS(SELECT 1 FROM information_schema.TABLE_CONSTRAINTS WHERE CONSTRAINT_SCHEMA=DATABASE() AND LOWER(CONSTRAINT_NAME)='fk_roomoverlays_rooms' AND CONSTRAINT_TYPE='FOREIGN KEY') THEN
   SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT='Room naming: colliding target foreign key FK_RoomOverlays_Rooms';
  END IF;
  IF (SELECT COUNT(*) FROM information_schema.KEY_COLUMN_USAGE WHERE TABLE_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='celloverlays' AND LOWER(CONSTRAINT_NAME)='fk_celloverlays_hearingprofiles')<>1 OR (SELECT COUNT(*) FROM information_schema.KEY_COLUMN_USAGE WHERE TABLE_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='celloverlays' AND LOWER(CONSTRAINT_NAME)='fk_celloverlays_hearingprofiles' AND ((ORDINAL_POSITION=1 AND LOWER(COLUMN_NAME)='hearingprofileid' AND REFERENCED_TABLE_SCHEMA=DATABASE() AND LOWER(REFERENCED_TABLE_NAME)='hearingprofiles' AND LOWER(REFERENCED_COLUMN_NAME)='id')))<>1 OR NOT EXISTS(SELECT 1 FROM information_schema.REFERENTIAL_CONSTRAINTS WHERE CONSTRAINT_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='celloverlays' AND LOWER(CONSTRAINT_NAME)='fk_celloverlays_hearingprofiles' AND DELETE_RULE IN ('SET NULL') AND UPDATE_RULE IN ('NO ACTION','RESTRICT')) THEN
   SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT='Room naming: foreign-key shape differs FK_CellOverlays_HearingProfiles';
  END IF;
  IF EXISTS(SELECT 1 FROM information_schema.TABLE_CONSTRAINTS WHERE CONSTRAINT_SCHEMA=DATABASE() AND LOWER(CONSTRAINT_NAME)='fk_roomoverlays_hearingprofiles' AND CONSTRAINT_TYPE='FOREIGN KEY') THEN
   SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT='Room naming: colliding target foreign key FK_RoomOverlays_HearingProfiles';
  END IF;
  IF (SELECT COUNT(*) FROM information_schema.KEY_COLUMN_USAGE WHERE TABLE_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='celloverlays' AND LOWER(CONSTRAINT_NAME)='fk_celloverlays_terrains')<>1 OR (SELECT COUNT(*) FROM information_schema.KEY_COLUMN_USAGE WHERE TABLE_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='celloverlays' AND LOWER(CONSTRAINT_NAME)='fk_celloverlays_terrains' AND ((ORDINAL_POSITION=1 AND LOWER(COLUMN_NAME)='terrainid' AND REFERENCED_TABLE_SCHEMA=DATABASE() AND LOWER(REFERENCED_TABLE_NAME)='terrains' AND LOWER(REFERENCED_COLUMN_NAME)='id')))<>1 OR NOT EXISTS(SELECT 1 FROM information_schema.REFERENTIAL_CONSTRAINTS WHERE CONSTRAINT_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='celloverlays' AND LOWER(CONSTRAINT_NAME)='fk_celloverlays_terrains' AND DELETE_RULE IN ('NO ACTION','RESTRICT') AND UPDATE_RULE IN ('NO ACTION','RESTRICT')) THEN
   SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT='Room naming: foreign-key shape differs FK_CellOverlays_Terrains';
  END IF;
  IF EXISTS(SELECT 1 FROM information_schema.TABLE_CONSTRAINTS WHERE CONSTRAINT_SCHEMA=DATABASE() AND LOWER(CONSTRAINT_NAME)='fk_roomoverlays_terrains' AND CONSTRAINT_TYPE='FOREIGN KEY') THEN
   SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT='Room naming: colliding target foreign key FK_RoomOverlays_Terrains';
  END IF;
  IF (SELECT COUNT(*) FROM information_schema.KEY_COLUMN_USAGE WHERE TABLE_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='celloverlays_exits' AND LOWER(CONSTRAINT_NAME)='fk_celloverlays_exits_celloverlays')<>1 OR (SELECT COUNT(*) FROM information_schema.KEY_COLUMN_USAGE WHERE TABLE_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='celloverlays_exits' AND LOWER(CONSTRAINT_NAME)='fk_celloverlays_exits_celloverlays' AND ((ORDINAL_POSITION=1 AND LOWER(COLUMN_NAME)='celloverlayid' AND REFERENCED_TABLE_SCHEMA=DATABASE() AND LOWER(REFERENCED_TABLE_NAME)='celloverlays' AND LOWER(REFERENCED_COLUMN_NAME)='id')))<>1 OR NOT EXISTS(SELECT 1 FROM information_schema.REFERENTIAL_CONSTRAINTS WHERE CONSTRAINT_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='celloverlays_exits' AND LOWER(CONSTRAINT_NAME)='fk_celloverlays_exits_celloverlays' AND DELETE_RULE IN ('CASCADE') AND UPDATE_RULE IN ('NO ACTION','RESTRICT')) THEN
   SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT='Room naming: foreign-key shape differs FK_CellOverlays_Exits_CellOverlays';
  END IF;
  IF EXISTS(SELECT 1 FROM information_schema.TABLE_CONSTRAINTS WHERE CONSTRAINT_SCHEMA=DATABASE() AND LOWER(CONSTRAINT_NAME)='fk_roomoverlays_exits_roomoverlays' AND CONSTRAINT_TYPE='FOREIGN KEY') THEN
   SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT='Room naming: colliding target foreign key FK_RoomOverlays_Exits_RoomOverlays';
  END IF;
  IF (SELECT COUNT(*) FROM information_schema.KEY_COLUMN_USAGE WHERE TABLE_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='celloverlays_exits' AND LOWER(CONSTRAINT_NAME)='fk_celloverlays_exits_exits')<>1 OR (SELECT COUNT(*) FROM information_schema.KEY_COLUMN_USAGE WHERE TABLE_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='celloverlays_exits' AND LOWER(CONSTRAINT_NAME)='fk_celloverlays_exits_exits' AND ((ORDINAL_POSITION=1 AND LOWER(COLUMN_NAME)='exitid' AND REFERENCED_TABLE_SCHEMA=DATABASE() AND LOWER(REFERENCED_TABLE_NAME)='exits' AND LOWER(REFERENCED_COLUMN_NAME)='id')))<>1 OR NOT EXISTS(SELECT 1 FROM information_schema.REFERENTIAL_CONSTRAINTS WHERE CONSTRAINT_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='celloverlays_exits' AND LOWER(CONSTRAINT_NAME)='fk_celloverlays_exits_exits' AND DELETE_RULE IN ('CASCADE') AND UPDATE_RULE IN ('NO ACTION','RESTRICT')) THEN
   SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT='Room naming: foreign-key shape differs FK_CellOverlays_Exits_Exits';
  END IF;
  IF EXISTS(SELECT 1 FROM information_schema.TABLE_CONSTRAINTS WHERE CONSTRAINT_SCHEMA=DATABASE() AND LOWER(CONSTRAINT_NAME)='fk_roomoverlays_exits_exits' AND CONSTRAINT_TYPE='FOREIGN KEY') THEN
   SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT='Room naming: colliding target foreign key FK_RoomOverlays_Exits_Exits';
  END IF;
  IF (SELECT COUNT(*) FROM information_schema.KEY_COLUMN_USAGE WHERE TABLE_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='cells' AND LOWER(CONSTRAINT_NAME)='fk_cells_celloverlays')<>1 OR (SELECT COUNT(*) FROM information_schema.KEY_COLUMN_USAGE WHERE TABLE_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='cells' AND LOWER(CONSTRAINT_NAME)='fk_cells_celloverlays' AND ((ORDINAL_POSITION=1 AND LOWER(COLUMN_NAME)='currentoverlayid' AND REFERENCED_TABLE_SCHEMA=DATABASE() AND LOWER(REFERENCED_TABLE_NAME)='celloverlays' AND LOWER(REFERENCED_COLUMN_NAME)='id')))<>1 OR NOT EXISTS(SELECT 1 FROM information_schema.REFERENTIAL_CONSTRAINTS WHERE CONSTRAINT_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='cells' AND LOWER(CONSTRAINT_NAME)='fk_cells_celloverlays' AND DELETE_RULE IN ('SET NULL') AND UPDATE_RULE IN ('NO ACTION','RESTRICT')) THEN
   SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT='Room naming: foreign-key shape differs FK_Cells_CellOverlays';
  END IF;
  IF EXISTS(SELECT 1 FROM information_schema.TABLE_CONSTRAINTS WHERE CONSTRAINT_SCHEMA=DATABASE() AND LOWER(CONSTRAINT_NAME)='fk_rooms_roomoverlays' AND CONSTRAINT_TYPE='FOREIGN KEY') THEN
   SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT='Room naming: colliding target foreign key FK_Rooms_RoomOverlays';
  END IF;
  IF (SELECT COUNT(*) FROM information_schema.KEY_COLUMN_USAGE WHERE TABLE_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='cells' AND LOWER(CONSTRAINT_NAME)='fk_cells_hostedvehiclecompartments')<>1 OR (SELECT COUNT(*) FROM information_schema.KEY_COLUMN_USAGE WHERE TABLE_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='cells' AND LOWER(CONSTRAINT_NAME)='fk_cells_hostedvehiclecompartments' AND ((ORDINAL_POSITION=1 AND LOWER(COLUMN_NAME)='hostedvehiclecompartmentid' AND REFERENCED_TABLE_SCHEMA=DATABASE() AND LOWER(REFERENCED_TABLE_NAME)='vehiclecompartments' AND LOWER(REFERENCED_COLUMN_NAME)='id')))<>1 OR NOT EXISTS(SELECT 1 FROM information_schema.REFERENTIAL_CONSTRAINTS WHERE CONSTRAINT_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='cells' AND LOWER(CONSTRAINT_NAME)='fk_cells_hostedvehiclecompartments' AND DELETE_RULE IN ('NO ACTION','RESTRICT') AND UPDATE_RULE IN ('NO ACTION','RESTRICT')) THEN
   SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT='Room naming: foreign-key shape differs FK_Cells_HostedVehicleCompartments';
  END IF;
  IF EXISTS(SELECT 1 FROM information_schema.TABLE_CONSTRAINTS WHERE CONSTRAINT_SCHEMA=DATABASE() AND LOWER(CONSTRAINT_NAME)='fk_rooms_hostedvehiclecompartments' AND CONSTRAINT_TYPE='FOREIGN KEY') THEN
   SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT='Room naming: colliding target foreign key FK_Rooms_HostedVehicleCompartments';
  END IF;
  IF (SELECT COUNT(*) FROM information_schema.KEY_COLUMN_USAGE WHERE TABLE_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='cells' AND LOWER(CONSTRAINT_NAME)='fk_cells_hostedvehicles')<>1 OR (SELECT COUNT(*) FROM information_schema.KEY_COLUMN_USAGE WHERE TABLE_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='cells' AND LOWER(CONSTRAINT_NAME)='fk_cells_hostedvehicles' AND ((ORDINAL_POSITION=1 AND LOWER(COLUMN_NAME)='hostedvehicleid' AND REFERENCED_TABLE_SCHEMA=DATABASE() AND LOWER(REFERENCED_TABLE_NAME)='vehicles' AND LOWER(REFERENCED_COLUMN_NAME)='id')))<>1 OR NOT EXISTS(SELECT 1 FROM information_schema.REFERENTIAL_CONSTRAINTS WHERE CONSTRAINT_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='cells' AND LOWER(CONSTRAINT_NAME)='fk_cells_hostedvehicles' AND DELETE_RULE IN ('NO ACTION','RESTRICT') AND UPDATE_RULE IN ('NO ACTION','RESTRICT')) THEN
   SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT='Room naming: foreign-key shape differs FK_Cells_HostedVehicles';
  END IF;
  IF EXISTS(SELECT 1 FROM information_schema.TABLE_CONSTRAINTS WHERE CONSTRAINT_SCHEMA=DATABASE() AND LOWER(CONSTRAINT_NAME)='fk_rooms_hostedvehicles' AND CONSTRAINT_TYPE='FOREIGN KEY') THEN
   SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT='Room naming: colliding target foreign key FK_Rooms_HostedVehicles';
  END IF;
  IF (SELECT COUNT(*) FROM information_schema.KEY_COLUMN_USAGE WHERE TABLE_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='cells' AND LOWER(CONSTRAINT_NAME)='fk_cells_owningzone')<>1 OR (SELECT COUNT(*) FROM information_schema.KEY_COLUMN_USAGE WHERE TABLE_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='cells' AND LOWER(CONSTRAINT_NAME)='fk_cells_owningzone' AND ((ORDINAL_POSITION=1 AND LOWER(COLUMN_NAME)='zoneid' AND REFERENCED_TABLE_SCHEMA=DATABASE() AND LOWER(REFERENCED_TABLE_NAME)='zones' AND LOWER(REFERENCED_COLUMN_NAME)='id')))<>1 OR NOT EXISTS(SELECT 1 FROM information_schema.REFERENTIAL_CONSTRAINTS WHERE CONSTRAINT_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='cells' AND LOWER(CONSTRAINT_NAME)='fk_cells_owningzone' AND DELETE_RULE IN ('NO ACTION','RESTRICT') AND UPDATE_RULE IN ('NO ACTION','RESTRICT')) THEN
   SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT='Room naming: foreign-key shape differs FK_Cells_OwningZone';
  END IF;
  IF EXISTS(SELECT 1 FROM information_schema.TABLE_CONSTRAINTS WHERE CONSTRAINT_SCHEMA=DATABASE() AND LOWER(CONSTRAINT_NAME)='fk_rooms_owningzone' AND CONSTRAINT_TYPE='FOREIGN KEY') THEN
   SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT='Room naming: colliding target foreign key FK_Rooms_OwningZone';
  END IF;
  IF (SELECT COUNT(*) FROM information_schema.KEY_COLUMN_USAGE WHERE TABLE_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='cells_foragableyields' AND LOWER(CONSTRAINT_NAME)='fk_cells_foragableyields_cells')<>1 OR (SELECT COUNT(*) FROM information_schema.KEY_COLUMN_USAGE WHERE TABLE_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='cells_foragableyields' AND LOWER(CONSTRAINT_NAME)='fk_cells_foragableyields_cells' AND ((ORDINAL_POSITION=1 AND LOWER(COLUMN_NAME)='cellid' AND REFERENCED_TABLE_SCHEMA=DATABASE() AND LOWER(REFERENCED_TABLE_NAME)='cells' AND LOWER(REFERENCED_COLUMN_NAME)='id')))<>1 OR NOT EXISTS(SELECT 1 FROM information_schema.REFERENTIAL_CONSTRAINTS WHERE CONSTRAINT_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='cells_foragableyields' AND LOWER(CONSTRAINT_NAME)='fk_cells_foragableyields_cells' AND DELETE_RULE IN ('CASCADE') AND UPDATE_RULE IN ('NO ACTION','RESTRICT','CASCADE')) THEN
   SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT='Room naming: foreign-key shape differs FK_Cells_ForagableYields_Cells';
  END IF;
  IF EXISTS(SELECT 1 FROM information_schema.TABLE_CONSTRAINTS WHERE CONSTRAINT_SCHEMA=DATABASE() AND LOWER(CONSTRAINT_NAME)='fk_rooms_foragableyields_rooms' AND CONSTRAINT_TYPE='FOREIGN KEY') THEN
   SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT='Room naming: colliding target foreign key FK_Rooms_ForagableYields_Rooms';
  END IF;
  IF (SELECT COUNT(*) FROM information_schema.KEY_COLUMN_USAGE WHERE TABLE_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='cells_gameitems' AND LOWER(CONSTRAINT_NAME)='fk_cells_gameitems_cells')<>1 OR (SELECT COUNT(*) FROM information_schema.KEY_COLUMN_USAGE WHERE TABLE_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='cells_gameitems' AND LOWER(CONSTRAINT_NAME)='fk_cells_gameitems_cells' AND ((ORDINAL_POSITION=1 AND LOWER(COLUMN_NAME)='cellid' AND REFERENCED_TABLE_SCHEMA=DATABASE() AND LOWER(REFERENCED_TABLE_NAME)='cells' AND LOWER(REFERENCED_COLUMN_NAME)='id')))<>1 OR NOT EXISTS(SELECT 1 FROM information_schema.REFERENTIAL_CONSTRAINTS WHERE CONSTRAINT_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='cells_gameitems' AND LOWER(CONSTRAINT_NAME)='fk_cells_gameitems_cells' AND DELETE_RULE IN ('CASCADE') AND UPDATE_RULE IN ('NO ACTION','RESTRICT')) THEN
   SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT='Room naming: foreign-key shape differs FK_Cells_GameItems_Cells';
  END IF;
  IF EXISTS(SELECT 1 FROM information_schema.TABLE_CONSTRAINTS WHERE CONSTRAINT_SCHEMA=DATABASE() AND LOWER(CONSTRAINT_NAME)='fk_rooms_gameitems_rooms' AND CONSTRAINT_TYPE='FOREIGN KEY') THEN
   SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT='Room naming: colliding target foreign key FK_Rooms_GameItems_Rooms';
  END IF;
  IF (SELECT COUNT(*) FROM information_schema.KEY_COLUMN_USAGE WHERE TABLE_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='cells_gameitems' AND LOWER(CONSTRAINT_NAME)='fk_cells_gameitems_gameitems')<>1 OR (SELECT COUNT(*) FROM information_schema.KEY_COLUMN_USAGE WHERE TABLE_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='cells_gameitems' AND LOWER(CONSTRAINT_NAME)='fk_cells_gameitems_gameitems' AND ((ORDINAL_POSITION=1 AND LOWER(COLUMN_NAME)='gameitemid' AND REFERENCED_TABLE_SCHEMA=DATABASE() AND LOWER(REFERENCED_TABLE_NAME)='gameitems' AND LOWER(REFERENCED_COLUMN_NAME)='id')))<>1 OR NOT EXISTS(SELECT 1 FROM information_schema.REFERENTIAL_CONSTRAINTS WHERE CONSTRAINT_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='cells_gameitems' AND LOWER(CONSTRAINT_NAME)='fk_cells_gameitems_gameitems' AND DELETE_RULE IN ('CASCADE') AND UPDATE_RULE IN ('NO ACTION','RESTRICT')) THEN
   SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT='Room naming: foreign-key shape differs FK_Cells_GameItems_GameItems';
  END IF;
  IF EXISTS(SELECT 1 FROM information_schema.TABLE_CONSTRAINTS WHERE CONSTRAINT_SCHEMA=DATABASE() AND LOWER(CONSTRAINT_NAME)='fk_rooms_gameitems_gameitems' AND CONSTRAINT_TYPE='FOREIGN KEY') THEN
   SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT='Room naming: colliding target foreign key FK_Rooms_GameItems_GameItems';
  END IF;
  IF (SELECT COUNT(*) FROM information_schema.KEY_COLUMN_USAGE WHERE TABLE_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='cells_magicresources' AND LOWER(CONSTRAINT_NAME)='fk_cells_magicresources_cells')<>1 OR (SELECT COUNT(*) FROM information_schema.KEY_COLUMN_USAGE WHERE TABLE_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='cells_magicresources' AND LOWER(CONSTRAINT_NAME)='fk_cells_magicresources_cells' AND ((ORDINAL_POSITION=1 AND LOWER(COLUMN_NAME)='cellid' AND REFERENCED_TABLE_SCHEMA=DATABASE() AND LOWER(REFERENCED_TABLE_NAME)='cells' AND LOWER(REFERENCED_COLUMN_NAME)='id')))<>1 OR NOT EXISTS(SELECT 1 FROM information_schema.REFERENTIAL_CONSTRAINTS WHERE CONSTRAINT_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='cells_magicresources' AND LOWER(CONSTRAINT_NAME)='fk_cells_magicresources_cells' AND DELETE_RULE IN ('CASCADE') AND UPDATE_RULE IN ('NO ACTION','RESTRICT','CASCADE')) THEN
   SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT='Room naming: foreign-key shape differs FK_Cells_MagicResources_Cells';
  END IF;
  IF EXISTS(SELECT 1 FROM information_schema.TABLE_CONSTRAINTS WHERE CONSTRAINT_SCHEMA=DATABASE() AND LOWER(CONSTRAINT_NAME)='fk_rooms_magicresources_rooms' AND CONSTRAINT_TYPE='FOREIGN KEY') THEN
   SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT='Room naming: colliding target foreign key FK_Rooms_MagicResources_Rooms';
  END IF;
  IF (SELECT COUNT(*) FROM information_schema.KEY_COLUMN_USAGE WHERE TABLE_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='cells_magicresources' AND LOWER(CONSTRAINT_NAME)='fk_cells_magicresources_magicresources')<>1 OR (SELECT COUNT(*) FROM information_schema.KEY_COLUMN_USAGE WHERE TABLE_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='cells_magicresources' AND LOWER(CONSTRAINT_NAME)='fk_cells_magicresources_magicresources' AND ((ORDINAL_POSITION=1 AND LOWER(COLUMN_NAME)='magicresourceid' AND REFERENCED_TABLE_SCHEMA=DATABASE() AND LOWER(REFERENCED_TABLE_NAME)='magicresources' AND LOWER(REFERENCED_COLUMN_NAME)='id')))<>1 OR NOT EXISTS(SELECT 1 FROM information_schema.REFERENTIAL_CONSTRAINTS WHERE CONSTRAINT_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='cells_magicresources' AND LOWER(CONSTRAINT_NAME)='fk_cells_magicresources_magicresources' AND DELETE_RULE IN ('CASCADE') AND UPDATE_RULE IN ('NO ACTION','RESTRICT','CASCADE')) THEN
   SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT='Room naming: foreign-key shape differs FK_Cells_MagicResources_MagicResources';
  END IF;
  IF EXISTS(SELECT 1 FROM information_schema.TABLE_CONSTRAINTS WHERE CONSTRAINT_SCHEMA=DATABASE() AND LOWER(CONSTRAINT_NAME)='fk_rooms_magicresources_magicresources' AND CONSTRAINT_TYPE='FOREIGN KEY') THEN
   SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT='Room naming: colliding target foreign key FK_Rooms_MagicResources_MagicResources';
  END IF;
  IF (SELECT COUNT(*) FROM information_schema.KEY_COLUMN_USAGE WHERE TABLE_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='cells_rangedcovers' AND LOWER(CONSTRAINT_NAME)='fk_cells_rangedcovers_cells')<>1 OR (SELECT COUNT(*) FROM information_schema.KEY_COLUMN_USAGE WHERE TABLE_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='cells_rangedcovers' AND LOWER(CONSTRAINT_NAME)='fk_cells_rangedcovers_cells' AND ((ORDINAL_POSITION=1 AND LOWER(COLUMN_NAME)='cellid' AND REFERENCED_TABLE_SCHEMA=DATABASE() AND LOWER(REFERENCED_TABLE_NAME)='cells' AND LOWER(REFERENCED_COLUMN_NAME)='id')))<>1 OR NOT EXISTS(SELECT 1 FROM information_schema.REFERENTIAL_CONSTRAINTS WHERE CONSTRAINT_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='cells_rangedcovers' AND LOWER(CONSTRAINT_NAME)='fk_cells_rangedcovers_cells' AND DELETE_RULE IN ('CASCADE') AND UPDATE_RULE IN ('NO ACTION','RESTRICT','CASCADE')) THEN
   SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT='Room naming: foreign-key shape differs FK_Cells_RangedCovers_Cells';
  END IF;
  IF EXISTS(SELECT 1 FROM information_schema.TABLE_CONSTRAINTS WHERE CONSTRAINT_SCHEMA=DATABASE() AND LOWER(CONSTRAINT_NAME)='fk_rooms_rangedcovers_rooms' AND CONSTRAINT_TYPE='FOREIGN KEY') THEN
   SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT='Room naming: colliding target foreign key FK_Rooms_RangedCovers_Rooms';
  END IF;
  IF (SELECT COUNT(*) FROM information_schema.KEY_COLUMN_USAGE WHERE TABLE_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='cells_rangedcovers' AND LOWER(CONSTRAINT_NAME)='fk_cells_rangedcovers_rangedcovers')<>1 OR (SELECT COUNT(*) FROM information_schema.KEY_COLUMN_USAGE WHERE TABLE_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='cells_rangedcovers' AND LOWER(CONSTRAINT_NAME)='fk_cells_rangedcovers_rangedcovers' AND ((ORDINAL_POSITION=1 AND LOWER(COLUMN_NAME)='rangedcoverid' AND REFERENCED_TABLE_SCHEMA=DATABASE() AND LOWER(REFERENCED_TABLE_NAME)='rangedcovers' AND LOWER(REFERENCED_COLUMN_NAME)='id')))<>1 OR NOT EXISTS(SELECT 1 FROM information_schema.REFERENTIAL_CONSTRAINTS WHERE CONSTRAINT_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='cells_rangedcovers' AND LOWER(CONSTRAINT_NAME)='fk_cells_rangedcovers_rangedcovers' AND DELETE_RULE IN ('CASCADE') AND UPDATE_RULE IN ('NO ACTION','RESTRICT','CASCADE')) THEN
   SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT='Room naming: foreign-key shape differs FK_Cells_RangedCovers_RangedCovers';
  END IF;
  IF EXISTS(SELECT 1 FROM information_schema.TABLE_CONSTRAINTS WHERE CONSTRAINT_SCHEMA=DATABASE() AND LOWER(CONSTRAINT_NAME)='fk_rooms_rangedcovers_rangedcovers' AND CONSTRAINT_TYPE='FOREIGN KEY') THEN
   SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT='Room naming: colliding target foreign key FK_Rooms_RangedCovers_RangedCovers';
  END IF;
  IF (SELECT COUNT(*) FROM information_schema.KEY_COLUMN_USAGE WHERE TABLE_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='cells_tags' AND LOWER(CONSTRAINT_NAME)='fk_cells_tags_cells')<>1 OR (SELECT COUNT(*) FROM information_schema.KEY_COLUMN_USAGE WHERE TABLE_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='cells_tags' AND LOWER(CONSTRAINT_NAME)='fk_cells_tags_cells' AND ((ORDINAL_POSITION=1 AND LOWER(COLUMN_NAME)='cellid' AND REFERENCED_TABLE_SCHEMA=DATABASE() AND LOWER(REFERENCED_TABLE_NAME)='cells' AND LOWER(REFERENCED_COLUMN_NAME)='id')))<>1 OR NOT EXISTS(SELECT 1 FROM information_schema.REFERENTIAL_CONSTRAINTS WHERE CONSTRAINT_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='cells_tags' AND LOWER(CONSTRAINT_NAME)='fk_cells_tags_cells' AND DELETE_RULE IN ('CASCADE') AND UPDATE_RULE IN ('NO ACTION','RESTRICT')) THEN
   SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT='Room naming: foreign-key shape differs FK_Cells_Tags_Cells';
  END IF;
  IF EXISTS(SELECT 1 FROM information_schema.TABLE_CONSTRAINTS WHERE CONSTRAINT_SCHEMA=DATABASE() AND LOWER(CONSTRAINT_NAME)='fk_rooms_tags_rooms' AND CONSTRAINT_TYPE='FOREIGN KEY') THEN
   SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT='Room naming: colliding target foreign key FK_Rooms_Tags_Rooms';
  END IF;
  IF (SELECT COUNT(*) FROM information_schema.KEY_COLUMN_USAGE WHERE TABLE_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='cells_tags' AND LOWER(CONSTRAINT_NAME)='fk_cells_tags_tags')<>1 OR (SELECT COUNT(*) FROM information_schema.KEY_COLUMN_USAGE WHERE TABLE_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='cells_tags' AND LOWER(CONSTRAINT_NAME)='fk_cells_tags_tags' AND ((ORDINAL_POSITION=1 AND LOWER(COLUMN_NAME)='tagid' AND REFERENCED_TABLE_SCHEMA=DATABASE() AND LOWER(REFERENCED_TABLE_NAME)='tags' AND LOWER(REFERENCED_COLUMN_NAME)='id')))<>1 OR NOT EXISTS(SELECT 1 FROM information_schema.REFERENTIAL_CONSTRAINTS WHERE CONSTRAINT_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='cells_tags' AND LOWER(CONSTRAINT_NAME)='fk_cells_tags_tags' AND DELETE_RULE IN ('CASCADE') AND UPDATE_RULE IN ('NO ACTION','RESTRICT','CASCADE')) THEN
   SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT='Room naming: foreign-key shape differs FK_Cells_Tags_Tags';
  END IF;
  IF EXISTS(SELECT 1 FROM information_schema.TABLE_CONSTRAINTS WHERE CONSTRAINT_SCHEMA=DATABASE() AND LOWER(CONSTRAINT_NAME)='fk_rooms_tags_tags' AND CONSTRAINT_TYPE='FOREIGN KEY') THEN
   SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT='Room naming: colliding target foreign key FK_Rooms_Tags_Tags';
  END IF;
  IF EXISTS(SELECT 1 FROM information_schema.TABLE_CONSTRAINTS WHERE CONSTRAINT_SCHEMA=DATABASE() AND LOWER(CONSTRAINT_NAME)='fk_characterinstances_cells' AND CONSTRAINT_TYPE='FOREIGN KEY') AND ((SELECT COUNT(*) FROM information_schema.KEY_COLUMN_USAGE WHERE TABLE_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='characterinstances' AND LOWER(CONSTRAINT_NAME)='fk_characterinstances_cells')<>1 OR (SELECT COUNT(*) FROM information_schema.KEY_COLUMN_USAGE WHERE TABLE_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='characterinstances' AND LOWER(CONSTRAINT_NAME)='fk_characterinstances_cells' AND ((ORDINAL_POSITION=1 AND LOWER(COLUMN_NAME)='locationid' AND REFERENCED_TABLE_SCHEMA=DATABASE() AND LOWER(REFERENCED_TABLE_NAME)='cells' AND LOWER(REFERENCED_COLUMN_NAME)='id')))<>1 OR NOT EXISTS(SELECT 1 FROM information_schema.REFERENTIAL_CONSTRAINTS WHERE CONSTRAINT_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='characterinstances' AND LOWER(CONSTRAINT_NAME)='fk_characterinstances_cells' AND DELETE_RULE IN ('SET NULL') AND UPDATE_RULE IN ('NO ACTION','RESTRICT'))) THEN
   SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT='Room naming: foreign-key shape differs FK_CharacterInstances_Cells';
  END IF;
  IF EXISTS(SELECT 1 FROM information_schema.TABLE_CONSTRAINTS WHERE CONSTRAINT_SCHEMA=DATABASE() AND LOWER(CONSTRAINT_NAME)='fk_characterinstances_rooms' AND CONSTRAINT_TYPE='FOREIGN KEY') THEN
   SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT='Room naming: colliding target foreign key FK_CharacterInstances_Rooms';
  END IF;
  IF (SELECT COUNT(*) FROM information_schema.KEY_COLUMN_USAGE WHERE TABLE_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='characterlog' AND LOWER(CONSTRAINT_NAME)='fk_characterlog_cells')<>1 OR (SELECT COUNT(*) FROM information_schema.KEY_COLUMN_USAGE WHERE TABLE_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='characterlog' AND LOWER(CONSTRAINT_NAME)='fk_characterlog_cells' AND ((ORDINAL_POSITION=1 AND LOWER(COLUMN_NAME)='cellid' AND REFERENCED_TABLE_SCHEMA=DATABASE() AND LOWER(REFERENCED_TABLE_NAME)='cells' AND LOWER(REFERENCED_COLUMN_NAME)='id')))<>1 OR NOT EXISTS(SELECT 1 FROM information_schema.REFERENTIAL_CONSTRAINTS WHERE CONSTRAINT_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='characterlog' AND LOWER(CONSTRAINT_NAME)='fk_characterlog_cells' AND DELETE_RULE IN ('CASCADE') AND UPDATE_RULE IN ('NO ACTION','RESTRICT','CASCADE')) THEN
   SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT='Room naming: foreign-key shape differs FK_CharacterLog_Cells';
  END IF;
  IF EXISTS(SELECT 1 FROM information_schema.TABLE_CONSTRAINTS WHERE CONSTRAINT_SCHEMA=DATABASE() AND LOWER(CONSTRAINT_NAME)='fk_characterlog_rooms' AND CONSTRAINT_TYPE='FOREIGN KEY') THEN
   SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT='Room naming: colliding target foreign key FK_CharacterLog_Rooms';
  END IF;
  IF (SELECT COUNT(*) FROM information_schema.KEY_COLUMN_USAGE WHERE TABLE_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='characters' AND LOWER(CONSTRAINT_NAME)='fk_characters_cells')<>1 OR (SELECT COUNT(*) FROM information_schema.KEY_COLUMN_USAGE WHERE TABLE_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='characters' AND LOWER(CONSTRAINT_NAME)='fk_characters_cells' AND ((ORDINAL_POSITION=1 AND LOWER(COLUMN_NAME)='location' AND REFERENCED_TABLE_SCHEMA=DATABASE() AND LOWER(REFERENCED_TABLE_NAME)='cells' AND LOWER(REFERENCED_COLUMN_NAME)='id')))<>1 OR NOT EXISTS(SELECT 1 FROM information_schema.REFERENTIAL_CONSTRAINTS WHERE CONSTRAINT_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='characters' AND LOWER(CONSTRAINT_NAME)='fk_characters_cells' AND DELETE_RULE IN ('NO ACTION','RESTRICT') AND UPDATE_RULE IN ('NO ACTION','RESTRICT')) THEN
   SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT='Room naming: foreign-key shape differs FK_Characters_Cells';
  END IF;
  IF EXISTS(SELECT 1 FROM information_schema.TABLE_CONSTRAINTS WHERE CONSTRAINT_SCHEMA=DATABASE() AND LOWER(CONSTRAINT_NAME)='fk_characters_rooms' AND CONSTRAINT_TYPE='FOREIGN KEY') THEN
   SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT='Room naming: colliding target foreign key FK_Characters_Rooms';
  END IF;
  IF (SELECT COUNT(*) FROM information_schema.KEY_COLUMN_USAGE WHERE TABLE_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='clans_administrationcells' AND LOWER(CONSTRAINT_NAME)='fk_clans_administrationcells_cells')<>1 OR (SELECT COUNT(*) FROM information_schema.KEY_COLUMN_USAGE WHERE TABLE_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='clans_administrationcells' AND LOWER(CONSTRAINT_NAME)='fk_clans_administrationcells_cells' AND ((ORDINAL_POSITION=1 AND LOWER(COLUMN_NAME)='cellid' AND REFERENCED_TABLE_SCHEMA=DATABASE() AND LOWER(REFERENCED_TABLE_NAME)='cells' AND LOWER(REFERENCED_COLUMN_NAME)='id')))<>1 OR NOT EXISTS(SELECT 1 FROM information_schema.REFERENTIAL_CONSTRAINTS WHERE CONSTRAINT_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='clans_administrationcells' AND LOWER(CONSTRAINT_NAME)='fk_clans_administrationcells_cells' AND DELETE_RULE IN ('CASCADE') AND UPDATE_RULE IN ('NO ACTION','RESTRICT','CASCADE')) THEN
   SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT='Room naming: foreign-key shape differs FK_Clans_AdministrationCells_Cells';
  END IF;
  IF EXISTS(SELECT 1 FROM information_schema.TABLE_CONSTRAINTS WHERE CONSTRAINT_SCHEMA=DATABASE() AND LOWER(CONSTRAINT_NAME)='fk_clans_administrationrooms_rooms' AND CONSTRAINT_TYPE='FOREIGN KEY') THEN
   SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT='Room naming: colliding target foreign key FK_Clans_AdministrationRooms_Rooms';
  END IF;
  IF (SELECT COUNT(*) FROM information_schema.KEY_COLUMN_USAGE WHERE TABLE_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='clans_administrationcells' AND LOWER(CONSTRAINT_NAME)='fk_clans_administrationcells_clans')<>1 OR (SELECT COUNT(*) FROM information_schema.KEY_COLUMN_USAGE WHERE TABLE_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='clans_administrationcells' AND LOWER(CONSTRAINT_NAME)='fk_clans_administrationcells_clans' AND ((ORDINAL_POSITION=1 AND LOWER(COLUMN_NAME)='clanid' AND REFERENCED_TABLE_SCHEMA=DATABASE() AND LOWER(REFERENCED_TABLE_NAME)='clans' AND LOWER(REFERENCED_COLUMN_NAME)='id')))<>1 OR NOT EXISTS(SELECT 1 FROM information_schema.REFERENTIAL_CONSTRAINTS WHERE CONSTRAINT_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='clans_administrationcells' AND LOWER(CONSTRAINT_NAME)='fk_clans_administrationcells_clans' AND DELETE_RULE IN ('CASCADE') AND UPDATE_RULE IN ('NO ACTION','RESTRICT','CASCADE')) THEN
   SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT='Room naming: foreign-key shape differs FK_Clans_AdministrationCells_Clans';
  END IF;
  IF EXISTS(SELECT 1 FROM information_schema.TABLE_CONSTRAINTS WHERE CONSTRAINT_SCHEMA=DATABASE() AND LOWER(CONSTRAINT_NAME)='fk_clans_administrationrooms_clans' AND CONSTRAINT_TYPE='FOREIGN KEY') THEN
   SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT='Room naming: colliding target foreign key FK_Clans_AdministrationRooms_Clans';
  END IF;
  IF (SELECT COUNT(*) FROM information_schema.KEY_COLUMN_USAGE WHERE TABLE_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='clans_hallcells' AND LOWER(CONSTRAINT_NAME)='fk_clans_hallcells_cells')<>1 OR (SELECT COUNT(*) FROM information_schema.KEY_COLUMN_USAGE WHERE TABLE_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='clans_hallcells' AND LOWER(CONSTRAINT_NAME)='fk_clans_hallcells_cells' AND ((ORDINAL_POSITION=1 AND LOWER(COLUMN_NAME)='cellid' AND REFERENCED_TABLE_SCHEMA=DATABASE() AND LOWER(REFERENCED_TABLE_NAME)='cells' AND LOWER(REFERENCED_COLUMN_NAME)='id')))<>1 OR NOT EXISTS(SELECT 1 FROM information_schema.REFERENTIAL_CONSTRAINTS WHERE CONSTRAINT_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='clans_hallcells' AND LOWER(CONSTRAINT_NAME)='fk_clans_hallcells_cells' AND DELETE_RULE IN ('CASCADE') AND UPDATE_RULE IN ('NO ACTION','RESTRICT')) THEN
   SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT='Room naming: foreign-key shape differs FK_Clans_HallCells_Cells';
  END IF;
  IF EXISTS(SELECT 1 FROM information_schema.TABLE_CONSTRAINTS WHERE CONSTRAINT_SCHEMA=DATABASE() AND LOWER(CONSTRAINT_NAME)='fk_clans_hallrooms_rooms' AND CONSTRAINT_TYPE='FOREIGN KEY') THEN
   SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT='Room naming: colliding target foreign key FK_Clans_HallRooms_Rooms';
  END IF;
  IF (SELECT COUNT(*) FROM information_schema.KEY_COLUMN_USAGE WHERE TABLE_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='clans_hallcells' AND LOWER(CONSTRAINT_NAME)='fk_clans_hallcells_clans')<>1 OR (SELECT COUNT(*) FROM information_schema.KEY_COLUMN_USAGE WHERE TABLE_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='clans_hallcells' AND LOWER(CONSTRAINT_NAME)='fk_clans_hallcells_clans' AND ((ORDINAL_POSITION=1 AND LOWER(COLUMN_NAME)='clanid' AND REFERENCED_TABLE_SCHEMA=DATABASE() AND LOWER(REFERENCED_TABLE_NAME)='clans' AND LOWER(REFERENCED_COLUMN_NAME)='id')))<>1 OR NOT EXISTS(SELECT 1 FROM information_schema.REFERENTIAL_CONSTRAINTS WHERE CONSTRAINT_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='clans_hallcells' AND LOWER(CONSTRAINT_NAME)='fk_clans_hallcells_clans' AND DELETE_RULE IN ('CASCADE') AND UPDATE_RULE IN ('NO ACTION','RESTRICT')) THEN
   SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT='Room naming: foreign-key shape differs FK_Clans_HallCells_Clans';
  END IF;
  IF EXISTS(SELECT 1 FROM information_schema.TABLE_CONSTRAINTS WHERE CONSTRAINT_SCHEMA=DATABASE() AND LOWER(CONSTRAINT_NAME)='fk_clans_hallrooms_clans' AND CONSTRAINT_TYPE='FOREIGN KEY') THEN
   SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT='Room naming: colliding target foreign key FK_Clans_HallRooms_Clans';
  END IF;
  IF (SELECT COUNT(*) FROM information_schema.KEY_COLUMN_USAGE WHERE TABLE_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='clans_treasurycells' AND LOWER(CONSTRAINT_NAME)='fk_clans_treasurycells_cells')<>1 OR (SELECT COUNT(*) FROM information_schema.KEY_COLUMN_USAGE WHERE TABLE_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='clans_treasurycells' AND LOWER(CONSTRAINT_NAME)='fk_clans_treasurycells_cells' AND ((ORDINAL_POSITION=1 AND LOWER(COLUMN_NAME)='cellid' AND REFERENCED_TABLE_SCHEMA=DATABASE() AND LOWER(REFERENCED_TABLE_NAME)='cells' AND LOWER(REFERENCED_COLUMN_NAME)='id')))<>1 OR NOT EXISTS(SELECT 1 FROM information_schema.REFERENTIAL_CONSTRAINTS WHERE CONSTRAINT_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='clans_treasurycells' AND LOWER(CONSTRAINT_NAME)='fk_clans_treasurycells_cells' AND DELETE_RULE IN ('CASCADE') AND UPDATE_RULE IN ('NO ACTION','RESTRICT','CASCADE')) THEN
   SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT='Room naming: foreign-key shape differs FK_Clans_TreasuryCells_Cells';
  END IF;
  IF EXISTS(SELECT 1 FROM information_schema.TABLE_CONSTRAINTS WHERE CONSTRAINT_SCHEMA=DATABASE() AND LOWER(CONSTRAINT_NAME)='fk_clans_treasuryrooms_rooms' AND CONSTRAINT_TYPE='FOREIGN KEY') THEN
   SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT='Room naming: colliding target foreign key FK_Clans_TreasuryRooms_Rooms';
  END IF;
  IF (SELECT COUNT(*) FROM information_schema.KEY_COLUMN_USAGE WHERE TABLE_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='clans_treasurycells' AND LOWER(CONSTRAINT_NAME)='fk_clans_treasurycells_clans')<>1 OR (SELECT COUNT(*) FROM information_schema.KEY_COLUMN_USAGE WHERE TABLE_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='clans_treasurycells' AND LOWER(CONSTRAINT_NAME)='fk_clans_treasurycells_clans' AND ((ORDINAL_POSITION=1 AND LOWER(COLUMN_NAME)='clanid' AND REFERENCED_TABLE_SCHEMA=DATABASE() AND LOWER(REFERENCED_TABLE_NAME)='clans' AND LOWER(REFERENCED_COLUMN_NAME)='id')))<>1 OR NOT EXISTS(SELECT 1 FROM information_schema.REFERENTIAL_CONSTRAINTS WHERE CONSTRAINT_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='clans_treasurycells' AND LOWER(CONSTRAINT_NAME)='fk_clans_treasurycells_clans' AND DELETE_RULE IN ('CASCADE') AND UPDATE_RULE IN ('NO ACTION','RESTRICT','CASCADE')) THEN
   SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT='Room naming: foreign-key shape differs FK_Clans_TreasuryCells_Clans';
  END IF;
  IF EXISTS(SELECT 1 FROM information_schema.TABLE_CONSTRAINTS WHERE CONSTRAINT_SCHEMA=DATABASE() AND LOWER(CONSTRAINT_NAME)='fk_clans_treasuryrooms_clans' AND CONSTRAINT_TYPE='FOREIGN KEY') THEN
   SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT='Room naming: colliding target foreign key FK_Clans_TreasuryRooms_Clans';
  END IF;
  IF (SELECT COUNT(*) FROM information_schema.KEY_COLUMN_USAGE WHERE TABLE_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='conveyancinglocations' AND LOWER(CONSTRAINT_NAME)='fk_conveyancinglocations_cells')<>1 OR (SELECT COUNT(*) FROM information_schema.KEY_COLUMN_USAGE WHERE TABLE_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='conveyancinglocations' AND LOWER(CONSTRAINT_NAME)='fk_conveyancinglocations_cells' AND ((ORDINAL_POSITION=1 AND LOWER(COLUMN_NAME)='cellid' AND REFERENCED_TABLE_SCHEMA=DATABASE() AND LOWER(REFERENCED_TABLE_NAME)='cells' AND LOWER(REFERENCED_COLUMN_NAME)='id')))<>1 OR NOT EXISTS(SELECT 1 FROM information_schema.REFERENTIAL_CONSTRAINTS WHERE CONSTRAINT_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='conveyancinglocations' AND LOWER(CONSTRAINT_NAME)='fk_conveyancinglocations_cells' AND DELETE_RULE IN ('CASCADE') AND UPDATE_RULE IN ('NO ACTION','RESTRICT')) THEN
   SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT='Room naming: foreign-key shape differs FK_ConveyancingLocations_Cells';
  END IF;
  IF EXISTS(SELECT 1 FROM information_schema.TABLE_CONSTRAINTS WHERE CONSTRAINT_SCHEMA=DATABASE() AND LOWER(CONSTRAINT_NAME)='fk_conveyancinglocations_rooms' AND CONSTRAINT_TYPE='FOREIGN KEY') THEN
   SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT='Room naming: colliding target foreign key FK_ConveyancingLocations_Rooms';
  END IF;
  IF (SELECT COUNT(*) FROM information_schema.KEY_COLUMN_USAGE WHERE TABLE_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='corpserecoveryreports' AND LOWER(CONSTRAINT_NAME)='fk_corpserecoveryreports_destinationcells')<>1 OR (SELECT COUNT(*) FROM information_schema.KEY_COLUMN_USAGE WHERE TABLE_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='corpserecoveryreports' AND LOWER(CONSTRAINT_NAME)='fk_corpserecoveryreports_destinationcells' AND ((ORDINAL_POSITION=1 AND LOWER(COLUMN_NAME)='destinationcellid' AND REFERENCED_TABLE_SCHEMA=DATABASE() AND LOWER(REFERENCED_TABLE_NAME)='cells' AND LOWER(REFERENCED_COLUMN_NAME)='id')))<>1 OR NOT EXISTS(SELECT 1 FROM information_schema.REFERENTIAL_CONSTRAINTS WHERE CONSTRAINT_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='corpserecoveryreports' AND LOWER(CONSTRAINT_NAME)='fk_corpserecoveryreports_destinationcells' AND DELETE_RULE IN ('CASCADE') AND UPDATE_RULE IN ('NO ACTION','RESTRICT')) THEN
   SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT='Room naming: foreign-key shape differs FK_CorpseRecoveryReports_DestinationCells';
  END IF;
  IF EXISTS(SELECT 1 FROM information_schema.TABLE_CONSTRAINTS WHERE CONSTRAINT_SCHEMA=DATABASE() AND LOWER(CONSTRAINT_NAME)='fk_corpserecoveryreports_destinationrooms' AND CONSTRAINT_TYPE='FOREIGN KEY') THEN
   SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT='Room naming: colliding target foreign key FK_CorpseRecoveryReports_DestinationRooms';
  END IF;
  IF (SELECT COUNT(*) FROM information_schema.KEY_COLUMN_USAGE WHERE TABLE_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='corpserecoveryreports' AND LOWER(CONSTRAINT_NAME)='fk_corpserecoveryreports_sourcecells')<>1 OR (SELECT COUNT(*) FROM information_schema.KEY_COLUMN_USAGE WHERE TABLE_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='corpserecoveryreports' AND LOWER(CONSTRAINT_NAME)='fk_corpserecoveryreports_sourcecells' AND ((ORDINAL_POSITION=1 AND LOWER(COLUMN_NAME)='sourcecellid' AND REFERENCED_TABLE_SCHEMA=DATABASE() AND LOWER(REFERENCED_TABLE_NAME)='cells' AND LOWER(REFERENCED_COLUMN_NAME)='id')))<>1 OR NOT EXISTS(SELECT 1 FROM information_schema.REFERENTIAL_CONSTRAINTS WHERE CONSTRAINT_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='corpserecoveryreports' AND LOWER(CONSTRAINT_NAME)='fk_corpserecoveryreports_sourcecells' AND DELETE_RULE IN ('CASCADE') AND UPDATE_RULE IN ('NO ACTION','RESTRICT')) THEN
   SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT='Room naming: foreign-key shape differs FK_CorpseRecoveryReports_SourceCells';
  END IF;
  IF EXISTS(SELECT 1 FROM information_schema.TABLE_CONSTRAINTS WHERE CONSTRAINT_SCHEMA=DATABASE() AND LOWER(CONSTRAINT_NAME)='fk_corpserecoveryreports_sourcerooms' AND CONSTRAINT_TYPE='FOREIGN KEY') THEN
   SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT='Room naming: colliding target foreign key FK_CorpseRecoveryReports_SourceRooms';
  END IF;
  IF (SELECT COUNT(*) FROM information_schema.KEY_COLUMN_USAGE WHERE TABLE_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='crimes' AND LOWER(CONSTRAINT_NAME)='fk_crimes_location')<>1 OR (SELECT COUNT(*) FROM information_schema.KEY_COLUMN_USAGE WHERE TABLE_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='crimes' AND LOWER(CONSTRAINT_NAME)='fk_crimes_location' AND ((ORDINAL_POSITION=1 AND LOWER(COLUMN_NAME)='locationid' AND REFERENCED_TABLE_SCHEMA=DATABASE() AND LOWER(REFERENCED_TABLE_NAME)='cells' AND LOWER(REFERENCED_COLUMN_NAME)='id')))<>1 OR NOT EXISTS(SELECT 1 FROM information_schema.REFERENTIAL_CONSTRAINTS WHERE CONSTRAINT_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='crimes' AND LOWER(CONSTRAINT_NAME)='fk_crimes_location' AND DELETE_RULE IN ('SET NULL') AND UPDATE_RULE IN ('NO ACTION','RESTRICT','CASCADE')) THEN
   SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT='Room naming: foreign-key shape differs FK_Crimes_Location';
  END IF;
  IF (SELECT COUNT(*) FROM information_schema.KEY_COLUMN_USAGE WHERE TABLE_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='economiczones' AND LOWER(CONSTRAINT_NAME)='fk_economiczones_morgueofficelocations')<>1 OR (SELECT COUNT(*) FROM information_schema.KEY_COLUMN_USAGE WHERE TABLE_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='economiczones' AND LOWER(CONSTRAINT_NAME)='fk_economiczones_morgueofficelocations' AND ((ORDINAL_POSITION=1 AND LOWER(COLUMN_NAME)='morgueofficelocationid' AND REFERENCED_TABLE_SCHEMA=DATABASE() AND LOWER(REFERENCED_TABLE_NAME)='cells' AND LOWER(REFERENCED_COLUMN_NAME)='id')))<>1 OR NOT EXISTS(SELECT 1 FROM information_schema.REFERENTIAL_CONSTRAINTS WHERE CONSTRAINT_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='economiczones' AND LOWER(CONSTRAINT_NAME)='fk_economiczones_morgueofficelocations' AND DELETE_RULE IN ('SET NULL') AND UPDATE_RULE IN ('NO ACTION','RESTRICT')) THEN
   SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT='Room naming: foreign-key shape differs FK_EconomicZones_MorgueOfficeLocations';
  END IF;
  IF (SELECT COUNT(*) FROM information_schema.KEY_COLUMN_USAGE WHERE TABLE_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='economiczones' AND LOWER(CONSTRAINT_NAME)='fk_economiczones_morguestoragelocations')<>1 OR (SELECT COUNT(*) FROM information_schema.KEY_COLUMN_USAGE WHERE TABLE_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='economiczones' AND LOWER(CONSTRAINT_NAME)='fk_economiczones_morguestoragelocations' AND ((ORDINAL_POSITION=1 AND LOWER(COLUMN_NAME)='morguestoragelocationid' AND REFERENCED_TABLE_SCHEMA=DATABASE() AND LOWER(REFERENCED_TABLE_NAME)='cells' AND LOWER(REFERENCED_COLUMN_NAME)='id')))<>1 OR NOT EXISTS(SELECT 1 FROM information_schema.REFERENTIAL_CONSTRAINTS WHERE CONSTRAINT_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='economiczones' AND LOWER(CONSTRAINT_NAME)='fk_economiczones_morguestoragelocations' AND DELETE_RULE IN ('SET NULL') AND UPDATE_RULE IN ('NO ACTION','RESTRICT')) THEN
   SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT='Room naming: foreign-key shape differs FK_EconomicZones_MorgueStorageLocations';
  END IF;
  IF (SELECT COUNT(*) FROM information_schema.KEY_COLUMN_USAGE WHERE TABLE_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='hooks_perceivables' AND LOWER(CONSTRAINT_NAME)='fk_hooks_perceivables_cells')<>1 OR (SELECT COUNT(*) FROM information_schema.KEY_COLUMN_USAGE WHERE TABLE_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='hooks_perceivables' AND LOWER(CONSTRAINT_NAME)='fk_hooks_perceivables_cells' AND ((ORDINAL_POSITION=1 AND LOWER(COLUMN_NAME)='cellid' AND REFERENCED_TABLE_SCHEMA=DATABASE() AND LOWER(REFERENCED_TABLE_NAME)='cells' AND LOWER(REFERENCED_COLUMN_NAME)='id')))<>1 OR NOT EXISTS(SELECT 1 FROM information_schema.REFERENTIAL_CONSTRAINTS WHERE CONSTRAINT_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='hooks_perceivables' AND LOWER(CONSTRAINT_NAME)='fk_hooks_perceivables_cells' AND DELETE_RULE IN ('CASCADE') AND UPDATE_RULE IN ('NO ACTION','RESTRICT','CASCADE')) THEN
   SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT='Room naming: foreign-key shape differs FK_Hooks_Perceivables_Cells';
  END IF;
  IF EXISTS(SELECT 1 FROM information_schema.TABLE_CONSTRAINTS WHERE CONSTRAINT_SCHEMA=DATABASE() AND LOWER(CONSTRAINT_NAME)='fk_hooks_perceivables_rooms' AND CONSTRAINT_TYPE='FOREIGN KEY') THEN
   SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT='Room naming: colliding target foreign key FK_Hooks_Perceivables_Rooms';
  END IF;
  IF (SELECT COUNT(*) FROM information_schema.KEY_COLUMN_USAGE WHERE TABLE_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='hospitallocations' AND LOWER(CONSTRAINT_NAME)='fk_hospitallocations_cells')<>1 OR (SELECT COUNT(*) FROM information_schema.KEY_COLUMN_USAGE WHERE TABLE_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='hospitallocations' AND LOWER(CONSTRAINT_NAME)='fk_hospitallocations_cells' AND ((ORDINAL_POSITION=1 AND LOWER(COLUMN_NAME)='cellid' AND REFERENCED_TABLE_SCHEMA=DATABASE() AND LOWER(REFERENCED_TABLE_NAME)='cells' AND LOWER(REFERENCED_COLUMN_NAME)='id')))<>1 OR NOT EXISTS(SELECT 1 FROM information_schema.REFERENTIAL_CONSTRAINTS WHERE CONSTRAINT_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='hospitallocations' AND LOWER(CONSTRAINT_NAME)='fk_hospitallocations_cells' AND DELETE_RULE IN ('CASCADE') AND UPDATE_RULE IN ('NO ACTION','RESTRICT')) THEN
   SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT='Room naming: foreign-key shape differs FK_HospitalLocations_Cells';
  END IF;
  IF EXISTS(SELECT 1 FROM information_schema.TABLE_CONSTRAINTS WHERE CONSTRAINT_SCHEMA=DATABASE() AND LOWER(CONSTRAINT_NAME)='fk_hospitallocations_rooms' AND CONSTRAINT_TYPE='FOREIGN KEY') THEN
   SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT='Room naming: colliding target foreign key FK_HospitalLocations_Rooms';
  END IF;
  IF (SELECT COUNT(*) FROM information_schema.KEY_COLUMN_USAGE WHERE TABLE_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='hospitalservicerequests' AND LOWER(CONSTRAINT_NAME)='fk_hospitalservicerequests_cells_recovery')<>1 OR (SELECT COUNT(*) FROM information_schema.KEY_COLUMN_USAGE WHERE TABLE_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='hospitalservicerequests' AND LOWER(CONSTRAINT_NAME)='fk_hospitalservicerequests_cells_recovery' AND ((ORDINAL_POSITION=1 AND LOWER(COLUMN_NAME)='recoveryroomcellid' AND REFERENCED_TABLE_SCHEMA=DATABASE() AND LOWER(REFERENCED_TABLE_NAME)='cells' AND LOWER(REFERENCED_COLUMN_NAME)='id')))<>1 OR NOT EXISTS(SELECT 1 FROM information_schema.REFERENTIAL_CONSTRAINTS WHERE CONSTRAINT_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='hospitalservicerequests' AND LOWER(CONSTRAINT_NAME)='fk_hospitalservicerequests_cells_recovery' AND DELETE_RULE IN ('SET NULL') AND UPDATE_RULE IN ('NO ACTION','RESTRICT')) THEN
   SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT='Room naming: foreign-key shape differs FK_HospitalServiceRequests_Cells_Recovery';
  END IF;
  IF EXISTS(SELECT 1 FROM information_schema.TABLE_CONSTRAINTS WHERE CONSTRAINT_SCHEMA=DATABASE() AND LOWER(CONSTRAINT_NAME)='fk_hospitalservicerequests_rooms_recovery' AND CONSTRAINT_TYPE='FOREIGN KEY') THEN
   SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT='Room naming: colliding target foreign key FK_HospitalServiceRequests_Rooms_Recovery';
  END IF;
  IF (SELECT COUNT(*) FROM information_schema.KEY_COLUMN_USAGE WHERE TABLE_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='hospitalservicerequests' AND LOWER(CONSTRAINT_NAME)='fk_hospitalservicerequests_cells_return')<>1 OR (SELECT COUNT(*) FROM information_schema.KEY_COLUMN_USAGE WHERE TABLE_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='hospitalservicerequests' AND LOWER(CONSTRAINT_NAME)='fk_hospitalservicerequests_cells_return' AND ((ORDINAL_POSITION=1 AND LOWER(COLUMN_NAME)='returncellid' AND REFERENCED_TABLE_SCHEMA=DATABASE() AND LOWER(REFERENCED_TABLE_NAME)='cells' AND LOWER(REFERENCED_COLUMN_NAME)='id')))<>1 OR NOT EXISTS(SELECT 1 FROM information_schema.REFERENTIAL_CONSTRAINTS WHERE CONSTRAINT_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='hospitalservicerequests' AND LOWER(CONSTRAINT_NAME)='fk_hospitalservicerequests_cells_return' AND DELETE_RULE IN ('SET NULL') AND UPDATE_RULE IN ('NO ACTION','RESTRICT')) THEN
   SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT='Room naming: foreign-key shape differs FK_HospitalServiceRequests_Cells_Return';
  END IF;
  IF EXISTS(SELECT 1 FROM information_schema.TABLE_CONSTRAINTS WHERE CONSTRAINT_SCHEMA=DATABASE() AND LOWER(CONSTRAINT_NAME)='fk_hospitalservicerequests_rooms_return' AND CONSTRAINT_TYPE='FOREIGN KEY') THEN
   SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT='Room naming: colliding target foreign key FK_HospitalServiceRequests_Rooms_Return';
  END IF;
  IF (SELECT COUNT(*) FROM information_schema.KEY_COLUMN_USAGE WHERE TABLE_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='hospitalservicerequests' AND LOWER(CONSTRAINT_NAME)='fk_hospitalservicerequests_cells_theatre')<>1 OR (SELECT COUNT(*) FROM information_schema.KEY_COLUMN_USAGE WHERE TABLE_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='hospitalservicerequests' AND LOWER(CONSTRAINT_NAME)='fk_hospitalservicerequests_cells_theatre' AND ((ORDINAL_POSITION=1 AND LOWER(COLUMN_NAME)='operatingtheatrecellid' AND REFERENCED_TABLE_SCHEMA=DATABASE() AND LOWER(REFERENCED_TABLE_NAME)='cells' AND LOWER(REFERENCED_COLUMN_NAME)='id')))<>1 OR NOT EXISTS(SELECT 1 FROM information_schema.REFERENTIAL_CONSTRAINTS WHERE CONSTRAINT_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='hospitalservicerequests' AND LOWER(CONSTRAINT_NAME)='fk_hospitalservicerequests_cells_theatre' AND DELETE_RULE IN ('SET NULL') AND UPDATE_RULE IN ('NO ACTION','RESTRICT')) THEN
   SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT='Room naming: foreign-key shape differs FK_HospitalServiceRequests_Cells_Theatre';
  END IF;
  IF EXISTS(SELECT 1 FROM information_schema.TABLE_CONSTRAINTS WHERE CONSTRAINT_SCHEMA=DATABASE() AND LOWER(CONSTRAINT_NAME)='fk_hospitalservicerequests_rooms_theatre' AND CONSTRAINT_TYPE='FOREIGN KEY') THEN
   SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT='Room naming: colliding target foreign key FK_HospitalServiceRequests_Rooms_Theatre';
  END IF;
  IF (SELECT COUNT(*) FROM information_schema.KEY_COLUMN_USAGE WHERE TABLE_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='hotelrooms' AND LOWER(CONSTRAINT_NAME)='fk_hotelrooms_cells')<>1 OR (SELECT COUNT(*) FROM information_schema.KEY_COLUMN_USAGE WHERE TABLE_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='hotelrooms' AND LOWER(CONSTRAINT_NAME)='fk_hotelrooms_cells' AND ((ORDINAL_POSITION=1 AND LOWER(COLUMN_NAME)='cellid' AND REFERENCED_TABLE_SCHEMA=DATABASE() AND LOWER(REFERENCED_TABLE_NAME)='cells' AND LOWER(REFERENCED_COLUMN_NAME)='id')))<>1 OR NOT EXISTS(SELECT 1 FROM information_schema.REFERENTIAL_CONSTRAINTS WHERE CONSTRAINT_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='hotelrooms' AND LOWER(CONSTRAINT_NAME)='fk_hotelrooms_cells' AND DELETE_RULE IN ('NO ACTION','RESTRICT') AND UPDATE_RULE IN ('NO ACTION','RESTRICT')) THEN
   SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT='Room naming: foreign-key shape differs FK_HotelRooms_Cells';
  END IF;
  IF EXISTS(SELECT 1 FROM information_schema.TABLE_CONSTRAINTS WHERE CONSTRAINT_SCHEMA=DATABASE() AND LOWER(CONSTRAINT_NAME)='fk_hotelrooms_rooms' AND CONSTRAINT_TYPE='FOREIGN KEY') THEN
   SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT='Room naming: colliding target foreign key FK_HotelRooms_Rooms';
  END IF;
  IF (SELECT COUNT(*) FROM information_schema.KEY_COLUMN_USAGE WHERE TABLE_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='jobfindinglocations' AND LOWER(CONSTRAINT_NAME)='fk_jobfindinglocations_cells')<>1 OR (SELECT COUNT(*) FROM information_schema.KEY_COLUMN_USAGE WHERE TABLE_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='jobfindinglocations' AND LOWER(CONSTRAINT_NAME)='fk_jobfindinglocations_cells' AND ((ORDINAL_POSITION=1 AND LOWER(COLUMN_NAME)='cellid' AND REFERENCED_TABLE_SCHEMA=DATABASE() AND LOWER(REFERENCED_TABLE_NAME)='cells' AND LOWER(REFERENCED_COLUMN_NAME)='id')))<>1 OR NOT EXISTS(SELECT 1 FROM information_schema.REFERENTIAL_CONSTRAINTS WHERE CONSTRAINT_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='jobfindinglocations' AND LOWER(CONSTRAINT_NAME)='fk_jobfindinglocations_cells' AND DELETE_RULE IN ('CASCADE') AND UPDATE_RULE IN ('NO ACTION','RESTRICT')) THEN
   SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT='Room naming: foreign-key shape differs FK_JobFindingLocations_Cells';
  END IF;
  IF EXISTS(SELECT 1 FROM information_schema.TABLE_CONSTRAINTS WHERE CONSTRAINT_SCHEMA=DATABASE() AND LOWER(CONSTRAINT_NAME)='fk_jobfindinglocations_rooms' AND CONSTRAINT_TYPE='FOREIGN KEY') THEN
   SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT='Room naming: colliding target foreign key FK_JobFindingLocations_Rooms';
  END IF;
  IF (SELECT COUNT(*) FROM information_schema.KEY_COLUMN_USAGE WHERE TABLE_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='legalauthorities' AND LOWER(CONSTRAINT_NAME)='fk_legalauthorities_courtroomcell')<>1 OR (SELECT COUNT(*) FROM information_schema.KEY_COLUMN_USAGE WHERE TABLE_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='legalauthorities' AND LOWER(CONSTRAINT_NAME)='fk_legalauthorities_courtroomcell' AND ((ORDINAL_POSITION=1 AND LOWER(COLUMN_NAME)='courtlocationid' AND REFERENCED_TABLE_SCHEMA=DATABASE() AND LOWER(REFERENCED_TABLE_NAME)='cells' AND LOWER(REFERENCED_COLUMN_NAME)='id')))<>1 OR NOT EXISTS(SELECT 1 FROM information_schema.REFERENTIAL_CONSTRAINTS WHERE CONSTRAINT_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='legalauthorities' AND LOWER(CONSTRAINT_NAME)='fk_legalauthorities_courtroomcell' AND DELETE_RULE IN ('NO ACTION','RESTRICT') AND UPDATE_RULE IN ('NO ACTION','RESTRICT')) THEN
   SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT='Room naming: foreign-key shape differs FK_LegalAuthorities_CourtroomCell';
  END IF;
  IF EXISTS(SELECT 1 FROM information_schema.TABLE_CONSTRAINTS WHERE CONSTRAINT_SCHEMA=DATABASE() AND LOWER(CONSTRAINT_NAME)='fk_legalauthorities_courtroomroom' AND CONSTRAINT_TYPE='FOREIGN KEY') THEN
   SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT='Room naming: colliding target foreign key FK_LegalAuthorities_CourtroomRoom';
  END IF;
  IF (SELECT COUNT(*) FROM information_schema.KEY_COLUMN_USAGE WHERE TABLE_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='legalauthorities' AND LOWER(CONSTRAINT_NAME)='fk_legalauthorities_marshallingcells')<>1 OR (SELECT COUNT(*) FROM information_schema.KEY_COLUMN_USAGE WHERE TABLE_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='legalauthorities' AND LOWER(CONSTRAINT_NAME)='fk_legalauthorities_marshallingcells' AND ((ORDINAL_POSITION=1 AND LOWER(COLUMN_NAME)='marshallinglocationid' AND REFERENCED_TABLE_SCHEMA=DATABASE() AND LOWER(REFERENCED_TABLE_NAME)='cells' AND LOWER(REFERENCED_COLUMN_NAME)='id')))<>1 OR NOT EXISTS(SELECT 1 FROM information_schema.REFERENTIAL_CONSTRAINTS WHERE CONSTRAINT_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='legalauthorities' AND LOWER(CONSTRAINT_NAME)='fk_legalauthorities_marshallingcells' AND DELETE_RULE IN ('NO ACTION','RESTRICT') AND UPDATE_RULE IN ('NO ACTION','RESTRICT')) THEN
   SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT='Room naming: foreign-key shape differs FK_LegalAuthorities_MarshallingCells';
  END IF;
  IF EXISTS(SELECT 1 FROM information_schema.TABLE_CONSTRAINTS WHERE CONSTRAINT_SCHEMA=DATABASE() AND LOWER(CONSTRAINT_NAME)='fk_legalauthorities_marshallingrooms' AND CONSTRAINT_TYPE='FOREIGN KEY') THEN
   SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT='Room naming: colliding target foreign key FK_LegalAuthorities_MarshallingRooms';
  END IF;
  IF (SELECT COUNT(*) FROM information_schema.KEY_COLUMN_USAGE WHERE TABLE_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='legalauthorities' AND LOWER(CONSTRAINT_NAME)='fk_legalauthorities_preparingcells')<>1 OR (SELECT COUNT(*) FROM information_schema.KEY_COLUMN_USAGE WHERE TABLE_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='legalauthorities' AND LOWER(CONSTRAINT_NAME)='fk_legalauthorities_preparingcells' AND ((ORDINAL_POSITION=1 AND LOWER(COLUMN_NAME)='preparinglocationid' AND REFERENCED_TABLE_SCHEMA=DATABASE() AND LOWER(REFERENCED_TABLE_NAME)='cells' AND LOWER(REFERENCED_COLUMN_NAME)='id')))<>1 OR NOT EXISTS(SELECT 1 FROM information_schema.REFERENTIAL_CONSTRAINTS WHERE CONSTRAINT_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='legalauthorities' AND LOWER(CONSTRAINT_NAME)='fk_legalauthorities_preparingcells' AND DELETE_RULE IN ('NO ACTION','RESTRICT') AND UPDATE_RULE IN ('NO ACTION','RESTRICT')) THEN
   SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT='Room naming: foreign-key shape differs FK_LegalAuthorities_PreparingCells';
  END IF;
  IF EXISTS(SELECT 1 FROM information_schema.TABLE_CONSTRAINTS WHERE CONSTRAINT_SCHEMA=DATABASE() AND LOWER(CONSTRAINT_NAME)='fk_legalauthorities_preparingrooms' AND CONSTRAINT_TYPE='FOREIGN KEY') THEN
   SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT='Room naming: colliding target foreign key FK_LegalAuthorities_PreparingRooms';
  END IF;
  IF (SELECT COUNT(*) FROM information_schema.KEY_COLUMN_USAGE WHERE TABLE_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='legalauthorities' AND LOWER(CONSTRAINT_NAME)='fk_legalauthorities_prisonbelongingscells')<>1 OR (SELECT COUNT(*) FROM information_schema.KEY_COLUMN_USAGE WHERE TABLE_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='legalauthorities' AND LOWER(CONSTRAINT_NAME)='fk_legalauthorities_prisonbelongingscells' AND ((ORDINAL_POSITION=1 AND LOWER(COLUMN_NAME)='prisonbelongingslocationid' AND REFERENCED_TABLE_SCHEMA=DATABASE() AND LOWER(REFERENCED_TABLE_NAME)='cells' AND LOWER(REFERENCED_COLUMN_NAME)='id')))<>1 OR NOT EXISTS(SELECT 1 FROM information_schema.REFERENTIAL_CONSTRAINTS WHERE CONSTRAINT_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='legalauthorities' AND LOWER(CONSTRAINT_NAME)='fk_legalauthorities_prisonbelongingscells' AND DELETE_RULE IN ('NO ACTION','RESTRICT') AND UPDATE_RULE IN ('NO ACTION','RESTRICT')) THEN
   SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT='Room naming: foreign-key shape differs FK_LegalAuthorities_PrisonBelongingsCells';
  END IF;
  IF EXISTS(SELECT 1 FROM information_schema.TABLE_CONSTRAINTS WHERE CONSTRAINT_SCHEMA=DATABASE() AND LOWER(CONSTRAINT_NAME)='fk_legalauthorities_prisonbelongingsrooms' AND CONSTRAINT_TYPE='FOREIGN KEY') THEN
   SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT='Room naming: colliding target foreign key FK_LegalAuthorities_PrisonBelongingsRooms';
  END IF;
  IF (SELECT COUNT(*) FROM information_schema.KEY_COLUMN_USAGE WHERE TABLE_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='legalauthorities' AND LOWER(CONSTRAINT_NAME)='fk_legalauthorities_prisoncells')<>1 OR (SELECT COUNT(*) FROM information_schema.KEY_COLUMN_USAGE WHERE TABLE_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='legalauthorities' AND LOWER(CONSTRAINT_NAME)='fk_legalauthorities_prisoncells' AND ((ORDINAL_POSITION=1 AND LOWER(COLUMN_NAME)='prisonlocationid' AND REFERENCED_TABLE_SCHEMA=DATABASE() AND LOWER(REFERENCED_TABLE_NAME)='cells' AND LOWER(REFERENCED_COLUMN_NAME)='id')))<>1 OR NOT EXISTS(SELECT 1 FROM information_schema.REFERENTIAL_CONSTRAINTS WHERE CONSTRAINT_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='legalauthorities' AND LOWER(CONSTRAINT_NAME)='fk_legalauthorities_prisoncells' AND DELETE_RULE IN ('NO ACTION','RESTRICT') AND UPDATE_RULE IN ('NO ACTION','RESTRICT')) THEN
   SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT='Room naming: foreign-key shape differs FK_LegalAuthorities_PrisonCells';
  END IF;
  IF EXISTS(SELECT 1 FROM information_schema.TABLE_CONSTRAINTS WHERE CONSTRAINT_SCHEMA=DATABASE() AND LOWER(CONSTRAINT_NAME)='fk_legalauthorities_prisonrooms' AND CONSTRAINT_TYPE='FOREIGN KEY') THEN
   SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT='Room naming: colliding target foreign key FK_LegalAuthorities_PrisonRooms';
  END IF;
  IF (SELECT COUNT(*) FROM information_schema.KEY_COLUMN_USAGE WHERE TABLE_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='legalauthorities' AND LOWER(CONSTRAINT_NAME)='fk_legalauthorities_prisonjailcells')<>1 OR (SELECT COUNT(*) FROM information_schema.KEY_COLUMN_USAGE WHERE TABLE_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='legalauthorities' AND LOWER(CONSTRAINT_NAME)='fk_legalauthorities_prisonjailcells' AND ((ORDINAL_POSITION=1 AND LOWER(COLUMN_NAME)='jaillocationid' AND REFERENCED_TABLE_SCHEMA=DATABASE() AND LOWER(REFERENCED_TABLE_NAME)='cells' AND LOWER(REFERENCED_COLUMN_NAME)='id')))<>1 OR NOT EXISTS(SELECT 1 FROM information_schema.REFERENTIAL_CONSTRAINTS WHERE CONSTRAINT_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='legalauthorities' AND LOWER(CONSTRAINT_NAME)='fk_legalauthorities_prisonjailcells' AND DELETE_RULE IN ('NO ACTION','RESTRICT') AND UPDATE_RULE IN ('NO ACTION','RESTRICT')) THEN
   SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT='Room naming: foreign-key shape differs FK_LegalAuthorities_PrisonJailCells';
  END IF;
  IF EXISTS(SELECT 1 FROM information_schema.TABLE_CONSTRAINTS WHERE CONSTRAINT_SCHEMA=DATABASE() AND LOWER(CONSTRAINT_NAME)='fk_legalauthorities_prisonjailrooms' AND CONSTRAINT_TYPE='FOREIGN KEY') THEN
   SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT='Room naming: colliding target foreign key FK_LegalAuthorities_PrisonJailRooms';
  END IF;
  IF (SELECT COUNT(*) FROM information_schema.KEY_COLUMN_USAGE WHERE TABLE_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='legalauthorities' AND LOWER(CONSTRAINT_NAME)='fk_legalauthorities_prisonreleasecells')<>1 OR (SELECT COUNT(*) FROM information_schema.KEY_COLUMN_USAGE WHERE TABLE_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='legalauthorities' AND LOWER(CONSTRAINT_NAME)='fk_legalauthorities_prisonreleasecells' AND ((ORDINAL_POSITION=1 AND LOWER(COLUMN_NAME)='prisonreleaselocationid' AND REFERENCED_TABLE_SCHEMA=DATABASE() AND LOWER(REFERENCED_TABLE_NAME)='cells' AND LOWER(REFERENCED_COLUMN_NAME)='id')))<>1 OR NOT EXISTS(SELECT 1 FROM information_schema.REFERENTIAL_CONSTRAINTS WHERE CONSTRAINT_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='legalauthorities' AND LOWER(CONSTRAINT_NAME)='fk_legalauthorities_prisonreleasecells' AND DELETE_RULE IN ('NO ACTION','RESTRICT') AND UPDATE_RULE IN ('NO ACTION','RESTRICT')) THEN
   SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT='Room naming: foreign-key shape differs FK_LegalAuthorities_PrisonReleaseCells';
  END IF;
  IF EXISTS(SELECT 1 FROM information_schema.TABLE_CONSTRAINTS WHERE CONSTRAINT_SCHEMA=DATABASE() AND LOWER(CONSTRAINT_NAME)='fk_legalauthorities_prisonreleaserooms' AND CONSTRAINT_TYPE='FOREIGN KEY') THEN
   SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT='Room naming: colliding target foreign key FK_LegalAuthorities_PrisonReleaseRooms';
  END IF;
  IF (SELECT COUNT(*) FROM information_schema.KEY_COLUMN_USAGE WHERE TABLE_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='legalauthorities' AND LOWER(CONSTRAINT_NAME)='fk_legalauthorities_stowingcells')<>1 OR (SELECT COUNT(*) FROM information_schema.KEY_COLUMN_USAGE WHERE TABLE_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='legalauthorities' AND LOWER(CONSTRAINT_NAME)='fk_legalauthorities_stowingcells' AND ((ORDINAL_POSITION=1 AND LOWER(COLUMN_NAME)='enforcerstowinglocationid' AND REFERENCED_TABLE_SCHEMA=DATABASE() AND LOWER(REFERENCED_TABLE_NAME)='cells' AND LOWER(REFERENCED_COLUMN_NAME)='id')))<>1 OR NOT EXISTS(SELECT 1 FROM information_schema.REFERENTIAL_CONSTRAINTS WHERE CONSTRAINT_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='legalauthorities' AND LOWER(CONSTRAINT_NAME)='fk_legalauthorities_stowingcells' AND DELETE_RULE IN ('NO ACTION','RESTRICT') AND UPDATE_RULE IN ('NO ACTION','RESTRICT')) THEN
   SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT='Room naming: foreign-key shape differs FK_LegalAuthorities_StowingCells';
  END IF;
  IF EXISTS(SELECT 1 FROM information_schema.TABLE_CONSTRAINTS WHERE CONSTRAINT_SCHEMA=DATABASE() AND LOWER(CONSTRAINT_NAME)='fk_legalauthorities_stowingrooms' AND CONSTRAINT_TYPE='FOREIGN KEY') THEN
   SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT='Room naming: colliding target foreign key FK_LegalAuthorities_StowingRooms';
  END IF;
  IF (SELECT COUNT(*) FROM information_schema.KEY_COLUMN_USAGE WHERE TABLE_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='legalauthoritiycells' AND LOWER(CONSTRAINT_NAME)='fk_legalauthoritiescells_cells')<>1 OR (SELECT COUNT(*) FROM information_schema.KEY_COLUMN_USAGE WHERE TABLE_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='legalauthoritiycells' AND LOWER(CONSTRAINT_NAME)='fk_legalauthoritiescells_cells' AND ((ORDINAL_POSITION=1 AND LOWER(COLUMN_NAME)='cellid' AND REFERENCED_TABLE_SCHEMA=DATABASE() AND LOWER(REFERENCED_TABLE_NAME)='cells' AND LOWER(REFERENCED_COLUMN_NAME)='id')))<>1 OR NOT EXISTS(SELECT 1 FROM information_schema.REFERENTIAL_CONSTRAINTS WHERE CONSTRAINT_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='legalauthoritiycells' AND LOWER(CONSTRAINT_NAME)='fk_legalauthoritiescells_cells' AND DELETE_RULE IN ('CASCADE') AND UPDATE_RULE IN ('NO ACTION','RESTRICT')) THEN
   SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT='Room naming: foreign-key shape differs FK_LegalAuthoritiesCells_Cells';
  END IF;
  IF EXISTS(SELECT 1 FROM information_schema.TABLE_CONSTRAINTS WHERE CONSTRAINT_SCHEMA=DATABASE() AND LOWER(CONSTRAINT_NAME)='fk_legalauthoritiesrooms_rooms' AND CONSTRAINT_TYPE='FOREIGN KEY') THEN
   SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT='Room naming: colliding target foreign key FK_LegalAuthoritiesRooms_Rooms';
  END IF;
  IF (SELECT COUNT(*) FROM information_schema.KEY_COLUMN_USAGE WHERE TABLE_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='legalauthoritiycells' AND LOWER(CONSTRAINT_NAME)='fk_legalauthoritiescells_legalauthorities')<>1 OR (SELECT COUNT(*) FROM information_schema.KEY_COLUMN_USAGE WHERE TABLE_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='legalauthoritiycells' AND LOWER(CONSTRAINT_NAME)='fk_legalauthoritiescells_legalauthorities' AND ((ORDINAL_POSITION=1 AND LOWER(COLUMN_NAME)='legalauthorityid' AND REFERENCED_TABLE_SCHEMA=DATABASE() AND LOWER(REFERENCED_TABLE_NAME)='legalauthorities' AND LOWER(REFERENCED_COLUMN_NAME)='id')))<>1 OR NOT EXISTS(SELECT 1 FROM information_schema.REFERENTIAL_CONSTRAINTS WHERE CONSTRAINT_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='legalauthoritiycells' AND LOWER(CONSTRAINT_NAME)='fk_legalauthoritiescells_legalauthorities' AND DELETE_RULE IN ('CASCADE') AND UPDATE_RULE IN ('NO ACTION','RESTRICT')) THEN
   SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT='Room naming: foreign-key shape differs FK_LegalAuthoritiesCells_LegalAuthorities';
  END IF;
  IF EXISTS(SELECT 1 FROM information_schema.TABLE_CONSTRAINTS WHERE CONSTRAINT_SCHEMA=DATABASE() AND LOWER(CONSTRAINT_NAME)='fk_legalauthoritiesrooms_legalauthorities' AND CONSTRAINT_TYPE='FOREIGN KEY') THEN
   SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT='Room naming: colliding target foreign key FK_LegalAuthoritiesRooms_LegalAuthorities';
  END IF;
  IF (SELECT COUNT(*) FROM information_schema.KEY_COLUMN_USAGE WHERE TABLE_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='legalauthorityjailcells' AND LOWER(CONSTRAINT_NAME)='fk_legalauthoritiescells_cells_jail')<>1 OR (SELECT COUNT(*) FROM information_schema.KEY_COLUMN_USAGE WHERE TABLE_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='legalauthorityjailcells' AND LOWER(CONSTRAINT_NAME)='fk_legalauthoritiescells_cells_jail' AND ((ORDINAL_POSITION=1 AND LOWER(COLUMN_NAME)='cellid' AND REFERENCED_TABLE_SCHEMA=DATABASE() AND LOWER(REFERENCED_TABLE_NAME)='cells' AND LOWER(REFERENCED_COLUMN_NAME)='id')))<>1 OR NOT EXISTS(SELECT 1 FROM information_schema.REFERENTIAL_CONSTRAINTS WHERE CONSTRAINT_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='legalauthorityjailcells' AND LOWER(CONSTRAINT_NAME)='fk_legalauthoritiescells_cells_jail' AND DELETE_RULE IN ('CASCADE') AND UPDATE_RULE IN ('NO ACTION','RESTRICT')) THEN
   SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT='Room naming: foreign-key shape differs FK_LegalAuthoritiesCells_Cells_Jail';
  END IF;
  IF EXISTS(SELECT 1 FROM information_schema.TABLE_CONSTRAINTS WHERE CONSTRAINT_SCHEMA=DATABASE() AND LOWER(CONSTRAINT_NAME)='fk_legalauthoritiesrooms_rooms_jail' AND CONSTRAINT_TYPE='FOREIGN KEY') THEN
   SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT='Room naming: colliding target foreign key FK_LegalAuthoritiesRooms_Rooms_Jail';
  END IF;
  IF (SELECT COUNT(*) FROM information_schema.KEY_COLUMN_USAGE WHERE TABLE_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='legalauthorityjailcells' AND LOWER(CONSTRAINT_NAME)='fk_legalauthoritiescells_legalauthorities_jail')<>1 OR (SELECT COUNT(*) FROM information_schema.KEY_COLUMN_USAGE WHERE TABLE_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='legalauthorityjailcells' AND LOWER(CONSTRAINT_NAME)='fk_legalauthoritiescells_legalauthorities_jail' AND ((ORDINAL_POSITION=1 AND LOWER(COLUMN_NAME)='legalauthorityid' AND REFERENCED_TABLE_SCHEMA=DATABASE() AND LOWER(REFERENCED_TABLE_NAME)='legalauthorities' AND LOWER(REFERENCED_COLUMN_NAME)='id')))<>1 OR NOT EXISTS(SELECT 1 FROM information_schema.REFERENTIAL_CONSTRAINTS WHERE CONSTRAINT_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='legalauthorityjailcells' AND LOWER(CONSTRAINT_NAME)='fk_legalauthoritiescells_legalauthorities_jail' AND DELETE_RULE IN ('CASCADE') AND UPDATE_RULE IN ('NO ACTION','RESTRICT')) THEN
   SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT='Room naming: foreign-key shape differs FK_LegalAuthoritiesCells_LegalAuthorities_Jail';
  END IF;
  IF EXISTS(SELECT 1 FROM information_schema.TABLE_CONSTRAINTS WHERE CONSTRAINT_SCHEMA=DATABASE() AND LOWER(CONSTRAINT_NAME)='fk_legalauthoritiesrooms_legalauthorities_jail' AND CONSTRAINT_TYPE='FOREIGN KEY') THEN
   SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT='Room naming: colliding target foreign key FK_LegalAuthoritiesRooms_LegalAuthorities_Jail';
  END IF;
  IF (SELECT COUNT(*) FROM information_schema.KEY_COLUMN_USAGE WHERE TABLE_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='magicportalendpoints' AND LOWER(CONSTRAINT_NAME)='fk_magicportalendpoints_cells')<>1 OR (SELECT COUNT(*) FROM information_schema.KEY_COLUMN_USAGE WHERE TABLE_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='magicportalendpoints' AND LOWER(CONSTRAINT_NAME)='fk_magicportalendpoints_cells' AND ((ORDINAL_POSITION=1 AND LOWER(COLUMN_NAME)='cellid' AND REFERENCED_TABLE_SCHEMA=DATABASE() AND LOWER(REFERENCED_TABLE_NAME)='cells' AND LOWER(REFERENCED_COLUMN_NAME)='id')))<>1 OR NOT EXISTS(SELECT 1 FROM information_schema.REFERENTIAL_CONSTRAINTS WHERE CONSTRAINT_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='magicportalendpoints' AND LOWER(CONSTRAINT_NAME)='fk_magicportalendpoints_cells' AND DELETE_RULE IN ('SET NULL') AND UPDATE_RULE IN ('NO ACTION','RESTRICT')) THEN
   SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT='Room naming: foreign-key shape differs FK_MagicPortalEndpoints_Cells';
  END IF;
  IF EXISTS(SELECT 1 FROM information_schema.TABLE_CONSTRAINTS WHERE CONSTRAINT_SCHEMA=DATABASE() AND LOWER(CONSTRAINT_NAME)='fk_magicportalendpoints_rooms' AND CONSTRAINT_TYPE='FOREIGN KEY') THEN
   SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT='Room naming: colliding target foreign key FK_MagicPortalEndpoints_Rooms';
  END IF;
  IF (SELECT COUNT(*) FROM information_schema.KEY_COLUMN_USAGE WHERE TABLE_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='npcspawnercells' AND LOWER(CONSTRAINT_NAME)='fk_npcspawnercells_cell')<>1 OR (SELECT COUNT(*) FROM information_schema.KEY_COLUMN_USAGE WHERE TABLE_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='npcspawnercells' AND LOWER(CONSTRAINT_NAME)='fk_npcspawnercells_cell' AND ((ORDINAL_POSITION=1 AND LOWER(COLUMN_NAME)='cellid' AND REFERENCED_TABLE_SCHEMA=DATABASE() AND LOWER(REFERENCED_TABLE_NAME)='cells' AND LOWER(REFERENCED_COLUMN_NAME)='id')))<>1 OR NOT EXISTS(SELECT 1 FROM information_schema.REFERENTIAL_CONSTRAINTS WHERE CONSTRAINT_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='npcspawnercells' AND LOWER(CONSTRAINT_NAME)='fk_npcspawnercells_cell' AND DELETE_RULE IN ('CASCADE') AND UPDATE_RULE IN ('NO ACTION','RESTRICT')) THEN
   SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT='Room naming: foreign-key shape differs FK_NPCSpawnerCells_Cell';
  END IF;
  IF EXISTS(SELECT 1 FROM information_schema.TABLE_CONSTRAINTS WHERE CONSTRAINT_SCHEMA=DATABASE() AND LOWER(CONSTRAINT_NAME)='fk_npcspawnerrooms_room' AND CONSTRAINT_TYPE='FOREIGN KEY') THEN
   SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT='Room naming: colliding target foreign key FK_NPCSpawnerRooms_Room';
  END IF;
  IF (SELECT COUNT(*) FROM information_schema.KEY_COLUMN_USAGE WHERE TABLE_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='npcspawnercells' AND LOWER(CONSTRAINT_NAME)='fk_npcspawnercells_npcspawner')<>1 OR (SELECT COUNT(*) FROM information_schema.KEY_COLUMN_USAGE WHERE TABLE_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='npcspawnercells' AND LOWER(CONSTRAINT_NAME)='fk_npcspawnercells_npcspawner' AND ((ORDINAL_POSITION=1 AND LOWER(COLUMN_NAME)='npcspawnerid' AND REFERENCED_TABLE_SCHEMA=DATABASE() AND LOWER(REFERENCED_TABLE_NAME)='npcspawners' AND LOWER(REFERENCED_COLUMN_NAME)='id')))<>1 OR NOT EXISTS(SELECT 1 FROM information_schema.REFERENTIAL_CONSTRAINTS WHERE CONSTRAINT_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='npcspawnercells' AND LOWER(CONSTRAINT_NAME)='fk_npcspawnercells_npcspawner' AND DELETE_RULE IN ('CASCADE') AND UPDATE_RULE IN ('NO ACTION','RESTRICT')) THEN
   SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT='Room naming: foreign-key shape differs FK_NPCSpawnerCells_NPCSpawner';
  END IF;
  IF EXISTS(SELECT 1 FROM information_schema.TABLE_CONSTRAINTS WHERE CONSTRAINT_SCHEMA=DATABASE() AND LOWER(CONSTRAINT_NAME)='fk_npcspawnerrooms_npcspawner' AND CONSTRAINT_TYPE='FOREIGN KEY') THEN
   SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT='Room naming: colliding target foreign key FK_NPCSpawnerRooms_NPCSpawner';
  END IF;
  IF (SELECT COUNT(*) FROM information_schema.KEY_COLUMN_USAGE WHERE TABLE_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='patrolroutesnodes' AND LOWER(CONSTRAINT_NAME)='fk_patrolroutesnodes_cells')<>1 OR (SELECT COUNT(*) FROM information_schema.KEY_COLUMN_USAGE WHERE TABLE_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='patrolroutesnodes' AND LOWER(CONSTRAINT_NAME)='fk_patrolroutesnodes_cells' AND ((ORDINAL_POSITION=1 AND LOWER(COLUMN_NAME)='cellid' AND REFERENCED_TABLE_SCHEMA=DATABASE() AND LOWER(REFERENCED_TABLE_NAME)='cells' AND LOWER(REFERENCED_COLUMN_NAME)='id')))<>1 OR NOT EXISTS(SELECT 1 FROM information_schema.REFERENTIAL_CONSTRAINTS WHERE CONSTRAINT_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='patrolroutesnodes' AND LOWER(CONSTRAINT_NAME)='fk_patrolroutesnodes_cells' AND DELETE_RULE IN ('CASCADE') AND UPDATE_RULE IN ('NO ACTION','RESTRICT')) THEN
   SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT='Room naming: foreign-key shape differs FK_PatrolRoutesNodes_Cells';
  END IF;
  IF EXISTS(SELECT 1 FROM information_schema.TABLE_CONSTRAINTS WHERE CONSTRAINT_SCHEMA=DATABASE() AND LOWER(CONSTRAINT_NAME)='fk_patrolroutesnodes_rooms' AND CONSTRAINT_TYPE='FOREIGN KEY') THEN
   SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT='Room naming: colliding target foreign key FK_PatrolRoutesNodes_Rooms';
  END IF;
  IF (SELECT COUNT(*) FROM information_schema.KEY_COLUMN_USAGE WHERE TABLE_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='patrols' AND LOWER(CONSTRAINT_NAME)='fk_patrols_lastmajornode')<>1 OR (SELECT COUNT(*) FROM information_schema.KEY_COLUMN_USAGE WHERE TABLE_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='patrols' AND LOWER(CONSTRAINT_NAME)='fk_patrols_lastmajornode' AND ((ORDINAL_POSITION=1 AND LOWER(COLUMN_NAME)='lastmajornodeid' AND REFERENCED_TABLE_SCHEMA=DATABASE() AND LOWER(REFERENCED_TABLE_NAME)='cells' AND LOWER(REFERENCED_COLUMN_NAME)='id')))<>1 OR NOT EXISTS(SELECT 1 FROM information_schema.REFERENTIAL_CONSTRAINTS WHERE CONSTRAINT_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='patrols' AND LOWER(CONSTRAINT_NAME)='fk_patrols_lastmajornode' AND DELETE_RULE IN ('NO ACTION','RESTRICT') AND UPDATE_RULE IN ('NO ACTION','RESTRICT')) THEN
   SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT='Room naming: foreign-key shape differs FK_Patrols_LastMajorNode';
  END IF;
  IF (SELECT COUNT(*) FROM information_schema.KEY_COLUMN_USAGE WHERE TABLE_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='patrols' AND LOWER(CONSTRAINT_NAME)='fk_patrols_nextmajornode')<>1 OR (SELECT COUNT(*) FROM information_schema.KEY_COLUMN_USAGE WHERE TABLE_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='patrols' AND LOWER(CONSTRAINT_NAME)='fk_patrols_nextmajornode' AND ((ORDINAL_POSITION=1 AND LOWER(COLUMN_NAME)='nextmajornodeid' AND REFERENCED_TABLE_SCHEMA=DATABASE() AND LOWER(REFERENCED_TABLE_NAME)='cells' AND LOWER(REFERENCED_COLUMN_NAME)='id')))<>1 OR NOT EXISTS(SELECT 1 FROM information_schema.REFERENTIAL_CONSTRAINTS WHERE CONSTRAINT_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='patrols' AND LOWER(CONSTRAINT_NAME)='fk_patrols_nextmajornode' AND DELETE_RULE IN ('NO ACTION','RESTRICT') AND UPDATE_RULE IN ('NO ACTION','RESTRICT')) THEN
   SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT='Room naming: foreign-key shape differs FK_Patrols_NextMajorNode';
  END IF;
  IF (SELECT COUNT(*) FROM information_schema.KEY_COLUMN_USAGE WHERE TABLE_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='probatelocations' AND LOWER(CONSTRAINT_NAME)='fk_probatelocations_cells')<>1 OR (SELECT COUNT(*) FROM information_schema.KEY_COLUMN_USAGE WHERE TABLE_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='probatelocations' AND LOWER(CONSTRAINT_NAME)='fk_probatelocations_cells' AND ((ORDINAL_POSITION=1 AND LOWER(COLUMN_NAME)='cellid' AND REFERENCED_TABLE_SCHEMA=DATABASE() AND LOWER(REFERENCED_TABLE_NAME)='cells' AND LOWER(REFERENCED_COLUMN_NAME)='id')))<>1 OR NOT EXISTS(SELECT 1 FROM information_schema.REFERENTIAL_CONSTRAINTS WHERE CONSTRAINT_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='probatelocations' AND LOWER(CONSTRAINT_NAME)='fk_probatelocations_cells' AND DELETE_RULE IN ('CASCADE') AND UPDATE_RULE IN ('NO ACTION','RESTRICT')) THEN
   SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT='Room naming: foreign-key shape differs FK_ProbateLocations_Cells';
  END IF;
  IF EXISTS(SELECT 1 FROM information_schema.TABLE_CONSTRAINTS WHERE CONSTRAINT_SCHEMA=DATABASE() AND LOWER(CONSTRAINT_NAME)='fk_probatelocations_rooms' AND CONSTRAINT_TYPE='FOREIGN KEY') THEN
   SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT='Room naming: colliding target foreign key FK_ProbateLocations_Rooms';
  END IF;
  IF (SELECT COUNT(*) FROM information_schema.KEY_COLUMN_USAGE WHERE TABLE_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='propertylocations' AND LOWER(CONSTRAINT_NAME)='fk_propertylocations_cell')<>1 OR (SELECT COUNT(*) FROM information_schema.KEY_COLUMN_USAGE WHERE TABLE_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='propertylocations' AND LOWER(CONSTRAINT_NAME)='fk_propertylocations_cell' AND ((ORDINAL_POSITION=1 AND LOWER(COLUMN_NAME)='cellid' AND REFERENCED_TABLE_SCHEMA=DATABASE() AND LOWER(REFERENCED_TABLE_NAME)='cells' AND LOWER(REFERENCED_COLUMN_NAME)='id')))<>1 OR NOT EXISTS(SELECT 1 FROM information_schema.REFERENTIAL_CONSTRAINTS WHERE CONSTRAINT_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='propertylocations' AND LOWER(CONSTRAINT_NAME)='fk_propertylocations_cell' AND DELETE_RULE IN ('CASCADE') AND UPDATE_RULE IN ('NO ACTION','RESTRICT')) THEN
   SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT='Room naming: foreign-key shape differs FK_PropertyLocations_Cell';
  END IF;
  IF EXISTS(SELECT 1 FROM information_schema.TABLE_CONSTRAINTS WHERE CONSTRAINT_SCHEMA=DATABASE() AND LOWER(CONSTRAINT_NAME)='fk_propertylocations_room' AND CONSTRAINT_TYPE='FOREIGN KEY') THEN
   SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT='Room naming: colliding target foreign key FK_PropertyLocations_Room';
  END IF;
  IF (SELECT COUNT(*) FROM information_schema.KEY_COLUMN_USAGE WHERE TABLE_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='restaurantcells' AND LOWER(CONSTRAINT_NAME)='fk_restaurantcells_restaurants')<>1 OR (SELECT COUNT(*) FROM information_schema.KEY_COLUMN_USAGE WHERE TABLE_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='restaurantcells' AND LOWER(CONSTRAINT_NAME)='fk_restaurantcells_restaurants' AND ((ORDINAL_POSITION=1 AND LOWER(COLUMN_NAME)='restaurantshopid' AND REFERENCED_TABLE_SCHEMA=DATABASE() AND LOWER(REFERENCED_TABLE_NAME)='restaurants' AND LOWER(REFERENCED_COLUMN_NAME)='shopid')))<>1 OR NOT EXISTS(SELECT 1 FROM information_schema.REFERENTIAL_CONSTRAINTS WHERE CONSTRAINT_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='restaurantcells' AND LOWER(CONSTRAINT_NAME)='fk_restaurantcells_restaurants' AND DELETE_RULE IN ('CASCADE') AND UPDATE_RULE IN ('NO ACTION','RESTRICT')) THEN
   SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT='Room naming: foreign-key shape differs FK_RestaurantCells_Restaurants';
  END IF;
  IF EXISTS(SELECT 1 FROM information_schema.TABLE_CONSTRAINTS WHERE CONSTRAINT_SCHEMA=DATABASE() AND LOWER(CONSTRAINT_NAME)='fk_restaurantrooms_restaurants' AND CONSTRAINT_TYPE='FOREIGN KEY') THEN
   SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT='Room naming: colliding target foreign key FK_RestaurantRooms_Restaurants';
  END IF;
  IF (SELECT COUNT(*) FROM information_schema.KEY_COLUMN_USAGE WHERE TABLE_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='routecelllandmarks' AND LOWER(CONSTRAINT_NAME)='fk_routecelllandmarks_routecells')<>1 OR (SELECT COUNT(*) FROM information_schema.KEY_COLUMN_USAGE WHERE TABLE_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='routecelllandmarks' AND LOWER(CONSTRAINT_NAME)='fk_routecelllandmarks_routecells' AND ((ORDINAL_POSITION=1 AND LOWER(COLUMN_NAME)='routecellid' AND REFERENCED_TABLE_SCHEMA=DATABASE() AND LOWER(REFERENCED_TABLE_NAME)='routecells' AND LOWER(REFERENCED_COLUMN_NAME)='cellid')))<>1 OR NOT EXISTS(SELECT 1 FROM information_schema.REFERENTIAL_CONSTRAINTS WHERE CONSTRAINT_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='routecelllandmarks' AND LOWER(CONSTRAINT_NAME)='fk_routecelllandmarks_routecells' AND DELETE_RULE IN ('CASCADE') AND UPDATE_RULE IN ('NO ACTION','RESTRICT')) THEN
   SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT='Room naming: foreign-key shape differs FK_RouteCellLandmarks_RouteCells';
  END IF;
  IF EXISTS(SELECT 1 FROM information_schema.TABLE_CONSTRAINTS WHERE CONSTRAINT_SCHEMA=DATABASE() AND LOWER(CONSTRAINT_NAME)='fk_routeroomlandmarks_routerooms' AND CONSTRAINT_TYPE='FOREIGN KEY') THEN
   SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT='Room naming: colliding target foreign key FK_RouteRoomLandmarks_RouteRooms';
  END IF;
  IF (SELECT COUNT(*) FROM information_schema.KEY_COLUMN_USAGE WHERE TABLE_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='routecells' AND LOWER(CONSTRAINT_NAME)='fk_routecells_cells')<>1 OR (SELECT COUNT(*) FROM information_schema.KEY_COLUMN_USAGE WHERE TABLE_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='routecells' AND LOWER(CONSTRAINT_NAME)='fk_routecells_cells' AND ((ORDINAL_POSITION=1 AND LOWER(COLUMN_NAME)='cellid' AND REFERENCED_TABLE_SCHEMA=DATABASE() AND LOWER(REFERENCED_TABLE_NAME)='cells' AND LOWER(REFERENCED_COLUMN_NAME)='id')))<>1 OR NOT EXISTS(SELECT 1 FROM information_schema.REFERENTIAL_CONSTRAINTS WHERE CONSTRAINT_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='routecells' AND LOWER(CONSTRAINT_NAME)='fk_routecells_cells' AND DELETE_RULE IN ('CASCADE') AND UPDATE_RULE IN ('NO ACTION','RESTRICT')) THEN
   SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT='Room naming: foreign-key shape differs FK_RouteCells_Cells';
  END IF;
  IF EXISTS(SELECT 1 FROM information_schema.TABLE_CONSTRAINTS WHERE CONSTRAINT_SCHEMA=DATABASE() AND LOWER(CONSTRAINT_NAME)='fk_routerooms_rooms' AND CONSTRAINT_TYPE='FOREIGN KEY') THEN
   SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT='Room naming: colliding target foreign key FK_RouteRooms_Rooms';
  END IF;
  IF (SELECT COUNT(*) FROM information_schema.KEY_COLUMN_USAGE WHERE TABLE_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='routeexitanchors' AND LOWER(CONSTRAINT_NAME)='fk_routeexitanchors_routecells')<>1 OR (SELECT COUNT(*) FROM information_schema.KEY_COLUMN_USAGE WHERE TABLE_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='routeexitanchors' AND LOWER(CONSTRAINT_NAME)='fk_routeexitanchors_routecells' AND ((ORDINAL_POSITION=1 AND LOWER(COLUMN_NAME)='routecellid' AND REFERENCED_TABLE_SCHEMA=DATABASE() AND LOWER(REFERENCED_TABLE_NAME)='routecells' AND LOWER(REFERENCED_COLUMN_NAME)='cellid')))<>1 OR NOT EXISTS(SELECT 1 FROM information_schema.REFERENTIAL_CONSTRAINTS WHERE CONSTRAINT_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='routeexitanchors' AND LOWER(CONSTRAINT_NAME)='fk_routeexitanchors_routecells' AND DELETE_RULE IN ('CASCADE') AND UPDATE_RULE IN ('NO ACTION','RESTRICT')) THEN
   SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT='Room naming: foreign-key shape differs FK_RouteExitAnchors_RouteCells';
  END IF;
  IF EXISTS(SELECT 1 FROM information_schema.TABLE_CONSTRAINTS WHERE CONSTRAINT_SCHEMA=DATABASE() AND LOWER(CONSTRAINT_NAME)='fk_routeexitanchors_routerooms' AND CONSTRAINT_TYPE='FOREIGN KEY') THEN
   SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT='Room naming: colliding target foreign key FK_RouteExitAnchors_RouteRooms';
  END IF;
  IF (SELECT COUNT(*) FROM information_schema.KEY_COLUMN_USAGE WHERE TABLE_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='shops' AND LOWER(CONSTRAINT_NAME)='fk_shops_cells_stockroom')<>1 OR (SELECT COUNT(*) FROM information_schema.KEY_COLUMN_USAGE WHERE TABLE_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='shops' AND LOWER(CONSTRAINT_NAME)='fk_shops_cells_stockroom' AND ((ORDINAL_POSITION=1 AND LOWER(COLUMN_NAME)='stockroomcellid' AND REFERENCED_TABLE_SCHEMA=DATABASE() AND LOWER(REFERENCED_TABLE_NAME)='cells' AND LOWER(REFERENCED_COLUMN_NAME)='id')))<>1 OR NOT EXISTS(SELECT 1 FROM information_schema.REFERENTIAL_CONSTRAINTS WHERE CONSTRAINT_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='shops' AND LOWER(CONSTRAINT_NAME)='fk_shops_cells_stockroom' AND DELETE_RULE IN ('SET NULL') AND UPDATE_RULE IN ('NO ACTION','RESTRICT','CASCADE')) THEN
   SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT='Room naming: foreign-key shape differs FK_Shops_Cells_Stockroom';
  END IF;
  IF EXISTS(SELECT 1 FROM information_schema.TABLE_CONSTRAINTS WHERE CONSTRAINT_SCHEMA=DATABASE() AND LOWER(CONSTRAINT_NAME)='fk_shops_rooms_stockroom' AND CONSTRAINT_TYPE='FOREIGN KEY') THEN
   SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT='Room naming: colliding target foreign key FK_Shops_Rooms_Stockroom';
  END IF;
  IF (SELECT COUNT(*) FROM information_schema.KEY_COLUMN_USAGE WHERE TABLE_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='shops' AND LOWER(CONSTRAINT_NAME)='fk_shops_cells_workshop')<>1 OR (SELECT COUNT(*) FROM information_schema.KEY_COLUMN_USAGE WHERE TABLE_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='shops' AND LOWER(CONSTRAINT_NAME)='fk_shops_cells_workshop' AND ((ORDINAL_POSITION=1 AND LOWER(COLUMN_NAME)='workshopcellid' AND REFERENCED_TABLE_SCHEMA=DATABASE() AND LOWER(REFERENCED_TABLE_NAME)='cells' AND LOWER(REFERENCED_COLUMN_NAME)='id')))<>1 OR NOT EXISTS(SELECT 1 FROM information_schema.REFERENTIAL_CONSTRAINTS WHERE CONSTRAINT_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='shops' AND LOWER(CONSTRAINT_NAME)='fk_shops_cells_workshop' AND DELETE_RULE IN ('SET NULL') AND UPDATE_RULE IN ('NO ACTION','RESTRICT','CASCADE')) THEN
   SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT='Room naming: foreign-key shape differs FK_Shops_Cells_Workshop';
  END IF;
  IF EXISTS(SELECT 1 FROM information_schema.TABLE_CONSTRAINTS WHERE CONSTRAINT_SCHEMA=DATABASE() AND LOWER(CONSTRAINT_NAME)='fk_shops_rooms_workshop' AND CONSTRAINT_TYPE='FOREIGN KEY') THEN
   SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT='Room naming: colliding target foreign key FK_Shops_Rooms_Workshop';
  END IF;
  IF (SELECT COUNT(*) FROM information_schema.KEY_COLUMN_USAGE WHERE TABLE_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='shops_storeroomcells' AND LOWER(CONSTRAINT_NAME)='fk_shops_storeroomcells_cells')<>1 OR (SELECT COUNT(*) FROM information_schema.KEY_COLUMN_USAGE WHERE TABLE_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='shops_storeroomcells' AND LOWER(CONSTRAINT_NAME)='fk_shops_storeroomcells_cells' AND ((ORDINAL_POSITION=1 AND LOWER(COLUMN_NAME)='cellid' AND REFERENCED_TABLE_SCHEMA=DATABASE() AND LOWER(REFERENCED_TABLE_NAME)='cells' AND LOWER(REFERENCED_COLUMN_NAME)='id')))<>1 OR NOT EXISTS(SELECT 1 FROM information_schema.REFERENTIAL_CONSTRAINTS WHERE CONSTRAINT_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='shops_storeroomcells' AND LOWER(CONSTRAINT_NAME)='fk_shops_storeroomcells_cells' AND DELETE_RULE IN ('CASCADE') AND UPDATE_RULE IN ('NO ACTION','RESTRICT','CASCADE')) THEN
   SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT='Room naming: foreign-key shape differs FK_Shops_StoreroomCells_Cells';
  END IF;
  IF EXISTS(SELECT 1 FROM information_schema.TABLE_CONSTRAINTS WHERE CONSTRAINT_SCHEMA=DATABASE() AND LOWER(CONSTRAINT_NAME)='fk_shops_storeroomrooms_rooms' AND CONSTRAINT_TYPE='FOREIGN KEY') THEN
   SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT='Room naming: colliding target foreign key FK_Shops_StoreroomRooms_Rooms';
  END IF;
  IF (SELECT COUNT(*) FROM information_schema.KEY_COLUMN_USAGE WHERE TABLE_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='shops_storeroomcells' AND LOWER(CONSTRAINT_NAME)='fk_shops_storeroomcells_shops')<>1 OR (SELECT COUNT(*) FROM information_schema.KEY_COLUMN_USAGE WHERE TABLE_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='shops_storeroomcells' AND LOWER(CONSTRAINT_NAME)='fk_shops_storeroomcells_shops' AND ((ORDINAL_POSITION=1 AND LOWER(COLUMN_NAME)='shopid' AND REFERENCED_TABLE_SCHEMA=DATABASE() AND LOWER(REFERENCED_TABLE_NAME)='shops' AND LOWER(REFERENCED_COLUMN_NAME)='id')))<>1 OR NOT EXISTS(SELECT 1 FROM information_schema.REFERENTIAL_CONSTRAINTS WHERE CONSTRAINT_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='shops_storeroomcells' AND LOWER(CONSTRAINT_NAME)='fk_shops_storeroomcells_shops' AND DELETE_RULE IN ('CASCADE') AND UPDATE_RULE IN ('NO ACTION','RESTRICT','CASCADE')) THEN
   SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT='Room naming: foreign-key shape differs FK_Shops_StoreroomCells_Shops';
  END IF;
  IF EXISTS(SELECT 1 FROM information_schema.TABLE_CONSTRAINTS WHERE CONSTRAINT_SCHEMA=DATABASE() AND LOWER(CONSTRAINT_NAME)='fk_shops_storeroomrooms_shops' AND CONSTRAINT_TYPE='FOREIGN KEY') THEN
   SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT='Room naming: colliding target foreign key FK_Shops_StoreroomRooms_Shops';
  END IF;
  IF (SELECT COUNT(*) FROM information_schema.KEY_COLUMN_USAGE WHERE TABLE_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='stables' AND LOWER(CONSTRAINT_NAME)='fk_stables_cells')<>1 OR (SELECT COUNT(*) FROM information_schema.KEY_COLUMN_USAGE WHERE TABLE_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='stables' AND LOWER(CONSTRAINT_NAME)='fk_stables_cells' AND ((ORDINAL_POSITION=1 AND LOWER(COLUMN_NAME)='cellid' AND REFERENCED_TABLE_SCHEMA=DATABASE() AND LOWER(REFERENCED_TABLE_NAME)='cells' AND LOWER(REFERENCED_COLUMN_NAME)='id')))<>1 OR NOT EXISTS(SELECT 1 FROM information_schema.REFERENTIAL_CONSTRAINTS WHERE CONSTRAINT_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='stables' AND LOWER(CONSTRAINT_NAME)='fk_stables_cells' AND DELETE_RULE IN ('CASCADE') AND UPDATE_RULE IN ('NO ACTION','RESTRICT')) THEN
   SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT='Room naming: foreign-key shape differs FK_Stables_Cells';
  END IF;
  IF EXISTS(SELECT 1 FROM information_schema.TABLE_CONSTRAINTS WHERE CONSTRAINT_SCHEMA=DATABASE() AND LOWER(CONSTRAINT_NAME)='fk_stables_rooms' AND CONSTRAINT_TYPE='FOREIGN KEY') THEN
   SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT='Room naming: colliding target foreign key FK_Stables_Rooms';
  END IF;
  IF (SELECT COUNT(*) FROM information_schema.KEY_COLUMN_USAGE WHERE TABLE_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='tracks' AND LOWER(CONSTRAINT_NAME)='fk_tracks_cells')<>1 OR (SELECT COUNT(*) FROM information_schema.KEY_COLUMN_USAGE WHERE TABLE_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='tracks' AND LOWER(CONSTRAINT_NAME)='fk_tracks_cells' AND ((ORDINAL_POSITION=1 AND LOWER(COLUMN_NAME)='cellid' AND REFERENCED_TABLE_SCHEMA=DATABASE() AND LOWER(REFERENCED_TABLE_NAME)='cells' AND LOWER(REFERENCED_COLUMN_NAME)='id')))<>1 OR NOT EXISTS(SELECT 1 FROM information_schema.REFERENTIAL_CONSTRAINTS WHERE CONSTRAINT_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='tracks' AND LOWER(CONSTRAINT_NAME)='fk_tracks_cells' AND DELETE_RULE IN ('CASCADE') AND UPDATE_RULE IN ('NO ACTION','RESTRICT')) THEN
   SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT='Room naming: foreign-key shape differs FK_Tracks_Cells';
  END IF;
  IF EXISTS(SELECT 1 FROM information_schema.TABLE_CONSTRAINTS WHERE CONSTRAINT_SCHEMA=DATABASE() AND LOWER(CONSTRAINT_NAME)='fk_tracks_rooms' AND CONSTRAINT_TYPE='FOREIGN KEY') THEN
   SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT='Room naming: colliding target foreign key FK_Tracks_Rooms';
  END IF;
  IF (SELECT COUNT(*) FROM information_schema.KEY_COLUMN_USAGE WHERE TABLE_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='vehiclecompartments' AND LOWER(CONSTRAINT_NAME)='fk_vehiclecompartments_interiorcells')<>1 OR (SELECT COUNT(*) FROM information_schema.KEY_COLUMN_USAGE WHERE TABLE_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='vehiclecompartments' AND LOWER(CONSTRAINT_NAME)='fk_vehiclecompartments_interiorcells' AND ((ORDINAL_POSITION=1 AND LOWER(COLUMN_NAME)='interiorcellid' AND REFERENCED_TABLE_SCHEMA=DATABASE() AND LOWER(REFERENCED_TABLE_NAME)='cells' AND LOWER(REFERENCED_COLUMN_NAME)='id')))<>1 OR NOT EXISTS(SELECT 1 FROM information_schema.REFERENTIAL_CONSTRAINTS WHERE CONSTRAINT_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='vehiclecompartments' AND LOWER(CONSTRAINT_NAME)='fk_vehiclecompartments_interiorcells' AND DELETE_RULE IN ('NO ACTION','RESTRICT') AND UPDATE_RULE IN ('NO ACTION','RESTRICT')) THEN
   SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT='Room naming: foreign-key shape differs FK_VehicleCompartments_InteriorCells';
  END IF;
  IF EXISTS(SELECT 1 FROM information_schema.TABLE_CONSTRAINTS WHERE CONSTRAINT_SCHEMA=DATABASE() AND LOWER(CONSTRAINT_NAME)='fk_vehiclecompartments_interiorrooms' AND CONSTRAINT_TYPE='FOREIGN KEY') THEN
   SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT='Room naming: colliding target foreign key FK_VehicleCompartments_InteriorRooms';
  END IF;
  IF (SELECT COUNT(*) FROM information_schema.KEY_COLUMN_USAGE WHERE TABLE_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='vehicledockings' AND LOWER(CONSTRAINT_NAME)='fk_vehicledockings_exteriorcells')<>1 OR (SELECT COUNT(*) FROM information_schema.KEY_COLUMN_USAGE WHERE TABLE_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='vehicledockings' AND LOWER(CONSTRAINT_NAME)='fk_vehicledockings_exteriorcells' AND ((ORDINAL_POSITION=1 AND LOWER(COLUMN_NAME)='exteriorcellid' AND REFERENCED_TABLE_SCHEMA=DATABASE() AND LOWER(REFERENCED_TABLE_NAME)='cells' AND LOWER(REFERENCED_COLUMN_NAME)='id')))<>1 OR NOT EXISTS(SELECT 1 FROM information_schema.REFERENTIAL_CONSTRAINTS WHERE CONSTRAINT_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='vehicledockings' AND LOWER(CONSTRAINT_NAME)='fk_vehicledockings_exteriorcells' AND DELETE_RULE IN ('NO ACTION','RESTRICT') AND UPDATE_RULE IN ('NO ACTION','RESTRICT')) THEN
   SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT='Room naming: foreign-key shape differs FK_VehicleDockings_ExteriorCells';
  END IF;
  IF EXISTS(SELECT 1 FROM information_schema.TABLE_CONSTRAINTS WHERE CONSTRAINT_SCHEMA=DATABASE() AND LOWER(CONSTRAINT_NAME)='fk_vehicledockings_exteriorrooms' AND CONSTRAINT_TYPE='FOREIGN KEY') THEN
   SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT='Room naming: colliding target foreign key FK_VehicleDockings_ExteriorRooms';
  END IF;
  IF (SELECT COUNT(*) FROM information_schema.KEY_COLUMN_USAGE WHERE TABLE_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='vehiclerouteplatformbindings' AND LOWER(CONSTRAINT_NAME)='fk_vehiclerouteplatformbindings_cells')<>1 OR (SELECT COUNT(*) FROM information_schema.KEY_COLUMN_USAGE WHERE TABLE_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='vehiclerouteplatformbindings' AND LOWER(CONSTRAINT_NAME)='fk_vehiclerouteplatformbindings_cells' AND ((ORDINAL_POSITION=1 AND LOWER(COLUMN_NAME)='platformcellid' AND REFERENCED_TABLE_SCHEMA=DATABASE() AND LOWER(REFERENCED_TABLE_NAME)='cells' AND LOWER(REFERENCED_COLUMN_NAME)='id')))<>1 OR NOT EXISTS(SELECT 1 FROM information_schema.REFERENTIAL_CONSTRAINTS WHERE CONSTRAINT_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='vehiclerouteplatformbindings' AND LOWER(CONSTRAINT_NAME)='fk_vehiclerouteplatformbindings_cells' AND DELETE_RULE IN ('NO ACTION','RESTRICT') AND UPDATE_RULE IN ('NO ACTION','RESTRICT')) THEN
   SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT='Room naming: foreign-key shape differs FK_VehicleRoutePlatformBindings_Cells';
  END IF;
  IF EXISTS(SELECT 1 FROM information_schema.TABLE_CONSTRAINTS WHERE CONSTRAINT_SCHEMA=DATABASE() AND LOWER(CONSTRAINT_NAME)='fk_vehiclerouteplatformbindings_rooms' AND CONSTRAINT_TYPE='FOREIGN KEY') THEN
   SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT='Room naming: colliding target foreign key FK_VehicleRoutePlatformBindings_Rooms';
  END IF;
  IF (SELECT COUNT(*) FROM information_schema.KEY_COLUMN_USAGE WHERE TABLE_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='vehicleroutesteps' AND LOWER(CONSTRAINT_NAME)='fk_vehicleroutesteps_destinationcells')<>1 OR (SELECT COUNT(*) FROM information_schema.KEY_COLUMN_USAGE WHERE TABLE_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='vehicleroutesteps' AND LOWER(CONSTRAINT_NAME)='fk_vehicleroutesteps_destinationcells' AND ((ORDINAL_POSITION=1 AND LOWER(COLUMN_NAME)='destinationcellid' AND REFERENCED_TABLE_SCHEMA=DATABASE() AND LOWER(REFERENCED_TABLE_NAME)='cells' AND LOWER(REFERENCED_COLUMN_NAME)='id')))<>1 OR NOT EXISTS(SELECT 1 FROM information_schema.REFERENTIAL_CONSTRAINTS WHERE CONSTRAINT_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='vehicleroutesteps' AND LOWER(CONSTRAINT_NAME)='fk_vehicleroutesteps_destinationcells' AND DELETE_RULE IN ('NO ACTION','RESTRICT') AND UPDATE_RULE IN ('NO ACTION','RESTRICT')) THEN
   SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT='Room naming: foreign-key shape differs FK_VehicleRouteSteps_DestinationCells';
  END IF;
  IF EXISTS(SELECT 1 FROM information_schema.TABLE_CONSTRAINTS WHERE CONSTRAINT_SCHEMA=DATABASE() AND LOWER(CONSTRAINT_NAME)='fk_vehicleroutesteps_destinationrooms' AND CONSTRAINT_TYPE='FOREIGN KEY') THEN
   SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT='Room naming: colliding target foreign key FK_VehicleRouteSteps_DestinationRooms';
  END IF;
  IF (SELECT COUNT(*) FROM information_schema.KEY_COLUMN_USAGE WHERE TABLE_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='vehicleroutesteps' AND LOWER(CONSTRAINT_NAME)='fk_vehicleroutesteps_origincells')<>1 OR (SELECT COUNT(*) FROM information_schema.KEY_COLUMN_USAGE WHERE TABLE_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='vehicleroutesteps' AND LOWER(CONSTRAINT_NAME)='fk_vehicleroutesteps_origincells' AND ((ORDINAL_POSITION=1 AND LOWER(COLUMN_NAME)='origincellid' AND REFERENCED_TABLE_SCHEMA=DATABASE() AND LOWER(REFERENCED_TABLE_NAME)='cells' AND LOWER(REFERENCED_COLUMN_NAME)='id')))<>1 OR NOT EXISTS(SELECT 1 FROM information_schema.REFERENTIAL_CONSTRAINTS WHERE CONSTRAINT_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='vehicleroutesteps' AND LOWER(CONSTRAINT_NAME)='fk_vehicleroutesteps_origincells' AND DELETE_RULE IN ('NO ACTION','RESTRICT') AND UPDATE_RULE IN ('NO ACTION','RESTRICT')) THEN
   SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT='Room naming: foreign-key shape differs FK_VehicleRouteSteps_OriginCells';
  END IF;
  IF EXISTS(SELECT 1 FROM information_schema.TABLE_CONSTRAINTS WHERE CONSTRAINT_SCHEMA=DATABASE() AND LOWER(CONSTRAINT_NAME)='fk_vehicleroutesteps_originrooms' AND CONSTRAINT_TYPE='FOREIGN KEY') THEN
   SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT='Room naming: colliding target foreign key FK_VehicleRouteSteps_OriginRooms';
  END IF;
  IF (SELECT COUNT(*) FROM information_schema.KEY_COLUMN_USAGE WHERE TABLE_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='vehicleroutestops' AND LOWER(CONSTRAINT_NAME)='fk_vehicleroutestops_cells')<>1 OR (SELECT COUNT(*) FROM information_schema.KEY_COLUMN_USAGE WHERE TABLE_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='vehicleroutestops' AND LOWER(CONSTRAINT_NAME)='fk_vehicleroutestops_cells' AND ((ORDINAL_POSITION=1 AND LOWER(COLUMN_NAME)='cellid' AND REFERENCED_TABLE_SCHEMA=DATABASE() AND LOWER(REFERENCED_TABLE_NAME)='cells' AND LOWER(REFERENCED_COLUMN_NAME)='id')))<>1 OR NOT EXISTS(SELECT 1 FROM information_schema.REFERENTIAL_CONSTRAINTS WHERE CONSTRAINT_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='vehicleroutestops' AND LOWER(CONSTRAINT_NAME)='fk_vehicleroutestops_cells' AND DELETE_RULE IN ('NO ACTION','RESTRICT') AND UPDATE_RULE IN ('NO ACTION','RESTRICT')) THEN
   SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT='Room naming: foreign-key shape differs FK_VehicleRouteStops_Cells';
  END IF;
  IF EXISTS(SELECT 1 FROM information_schema.TABLE_CONSTRAINTS WHERE CONSTRAINT_SCHEMA=DATABASE() AND LOWER(CONSTRAINT_NAME)='fk_vehicleroutestops_rooms' AND CONSTRAINT_TYPE='FOREIGN KEY') THEN
   SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT='Room naming: colliding target foreign key FK_VehicleRouteStops_Rooms';
  END IF;
  IF (SELECT COUNT(*) FROM information_schema.KEY_COLUMN_USAGE WHERE TABLE_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='vehicleroutetopologypins' AND LOWER(CONSTRAINT_NAME)='fk_vehicleroutetopologypins_routecells')<>1 OR (SELECT COUNT(*) FROM information_schema.KEY_COLUMN_USAGE WHERE TABLE_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='vehicleroutetopologypins' AND LOWER(CONSTRAINT_NAME)='fk_vehicleroutetopologypins_routecells' AND ((ORDINAL_POSITION=1 AND LOWER(COLUMN_NAME)='routecellid' AND REFERENCED_TABLE_SCHEMA=DATABASE() AND LOWER(REFERENCED_TABLE_NAME)='routecells' AND LOWER(REFERENCED_COLUMN_NAME)='cellid')))<>1 OR NOT EXISTS(SELECT 1 FROM information_schema.REFERENTIAL_CONSTRAINTS WHERE CONSTRAINT_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='vehicleroutetopologypins' AND LOWER(CONSTRAINT_NAME)='fk_vehicleroutetopologypins_routecells' AND DELETE_RULE IN ('NO ACTION','RESTRICT') AND UPDATE_RULE IN ('NO ACTION','RESTRICT')) THEN
   SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT='Room naming: foreign-key shape differs FK_VehicleRouteTopologyPins_RouteCells';
  END IF;
  IF EXISTS(SELECT 1 FROM information_schema.TABLE_CONSTRAINTS WHERE CONSTRAINT_SCHEMA=DATABASE() AND LOWER(CONSTRAINT_NAME)='fk_vehicleroutetopologypins_routerooms' AND CONSTRAINT_TYPE='FOREIGN KEY') THEN
   SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT='Room naming: colliding target foreign key FK_VehicleRouteTopologyPins_RouteRooms';
  END IF;
  IF (SELECT COUNT(*) FROM information_schema.KEY_COLUMN_USAGE WHERE TABLE_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='vehicles' AND LOWER(CONSTRAINT_NAME)='fk_vehicles_cells_current')<>1 OR (SELECT COUNT(*) FROM information_schema.KEY_COLUMN_USAGE WHERE TABLE_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='vehicles' AND LOWER(CONSTRAINT_NAME)='fk_vehicles_cells_current' AND ((ORDINAL_POSITION=1 AND LOWER(COLUMN_NAME)='currentcellid' AND REFERENCED_TABLE_SCHEMA=DATABASE() AND LOWER(REFERENCED_TABLE_NAME)='cells' AND LOWER(REFERENCED_COLUMN_NAME)='id')))<>1 OR NOT EXISTS(SELECT 1 FROM information_schema.REFERENTIAL_CONSTRAINTS WHERE CONSTRAINT_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='vehicles' AND LOWER(CONSTRAINT_NAME)='fk_vehicles_cells_current' AND DELETE_RULE IN ('SET NULL') AND UPDATE_RULE IN ('NO ACTION','RESTRICT')) THEN
   SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT='Room naming: foreign-key shape differs FK_Vehicles_Cells_Current';
  END IF;
  IF EXISTS(SELECT 1 FROM information_schema.TABLE_CONSTRAINTS WHERE CONSTRAINT_SCHEMA=DATABASE() AND LOWER(CONSTRAINT_NAME)='fk_vehicles_rooms_current' AND CONSTRAINT_TYPE='FOREIGN KEY') THEN
   SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT='Room naming: colliding target foreign key FK_Vehicles_Rooms_Current';
  END IF;
  IF (SELECT COUNT(*) FROM information_schema.KEY_COLUMN_USAGE WHERE TABLE_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='vehicles' AND LOWER(CONSTRAINT_NAME)='fk_vehicles_cells_destination')<>1 OR (SELECT COUNT(*) FROM information_schema.KEY_COLUMN_USAGE WHERE TABLE_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='vehicles' AND LOWER(CONSTRAINT_NAME)='fk_vehicles_cells_destination' AND ((ORDINAL_POSITION=1 AND LOWER(COLUMN_NAME)='destinationcellid' AND REFERENCED_TABLE_SCHEMA=DATABASE() AND LOWER(REFERENCED_TABLE_NAME)='cells' AND LOWER(REFERENCED_COLUMN_NAME)='id')))<>1 OR NOT EXISTS(SELECT 1 FROM information_schema.REFERENTIAL_CONSTRAINTS WHERE CONSTRAINT_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='vehicles' AND LOWER(CONSTRAINT_NAME)='fk_vehicles_cells_destination' AND DELETE_RULE IN ('SET NULL') AND UPDATE_RULE IN ('NO ACTION','RESTRICT')) THEN
   SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT='Room naming: foreign-key shape differs FK_Vehicles_Cells_Destination';
  END IF;
  IF EXISTS(SELECT 1 FROM information_schema.TABLE_CONSTRAINTS WHERE CONSTRAINT_SCHEMA=DATABASE() AND LOWER(CONSTRAINT_NAME)='fk_vehicles_rooms_destination' AND CONSTRAINT_TYPE='FOREIGN KEY') THEN
   SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT='Room naming: colliding target foreign key FK_Vehicles_Rooms_Destination';
  END IF;
  IF (SELECT COUNT(*) FROM information_schema.KEY_COLUMN_USAGE WHERE TABLE_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='zones' AND LOWER(CONSTRAINT_NAME)='fk_zones_cells')<>1 OR (SELECT COUNT(*) FROM information_schema.KEY_COLUMN_USAGE WHERE TABLE_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='zones' AND LOWER(CONSTRAINT_NAME)='fk_zones_cells' AND ((ORDINAL_POSITION=1 AND LOWER(COLUMN_NAME)='defaultcellid' AND REFERENCED_TABLE_SCHEMA=DATABASE() AND LOWER(REFERENCED_TABLE_NAME)='cells' AND LOWER(REFERENCED_COLUMN_NAME)='id')))<>1 OR NOT EXISTS(SELECT 1 FROM information_schema.REFERENTIAL_CONSTRAINTS WHERE CONSTRAINT_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='zones' AND LOWER(CONSTRAINT_NAME)='fk_zones_cells' AND DELETE_RULE IN ('NO ACTION','RESTRICT') AND UPDATE_RULE IN ('NO ACTION','RESTRICT')) THEN
   SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT='Room naming: foreign-key shape differs FK_Zones_Cells';
  END IF;
  IF EXISTS(SELECT 1 FROM information_schema.TABLE_CONSTRAINTS WHERE CONSTRAINT_SCHEMA=DATABASE() AND LOWER(CONSTRAINT_NAME)='fk_zones_rooms' AND CONSTRAINT_TYPE='FOREIGN KEY') THEN
   SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT='Room naming: colliding target foreign key FK_Zones_Rooms';
  END IF;
  IF EXISTS(SELECT 1 FROM information_schema.TABLE_CONSTRAINTS WHERE CONSTRAINT_SCHEMA=DATABASE() AND LOWER(CONSTRAINT_NAME)='ck_roomenvironmentalstates_pressure' AND CONSTRAINT_TYPE='CHECK') THEN
   SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT='Room naming: colliding target check CK_RoomEnvironmentalStates_Pressure';
  END IF;
  IF EXISTS(SELECT 1 FROM information_schema.TABLE_CONSTRAINTS WHERE CONSTRAINT_SCHEMA=DATABASE() AND LOWER(CONSTRAINT_NAME)='ck_roomenvironmentalstates_scardamage' AND CONSTRAINT_TYPE='CHECK') THEN
   SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT='Room naming: colliding target check CK_RoomEnvironmentalStates_ScarDamage';
  END IF;
  IF EXISTS(SELECT 1 FROM information_schema.TABLE_CONSTRAINTS WHERE CONSTRAINT_SCHEMA=DATABASE() AND LOWER(CONSTRAINT_NAME)='ck_roomenvironmentalstates_versions' AND CONSTRAINT_TYPE='CHECK') THEN
   SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT='Room naming: colliding target check CK_RoomEnvironmentalStates_Versions';
  END IF;
  IF EXISTS(SELECT 1 FROM information_schema.TABLE_CONSTRAINTS WHERE CONSTRAINT_SCHEMA=DATABASE() AND LOWER(CONSTRAINT_NAME)='ck_rooms_hostedvehicleownership' AND CONSTRAINT_TYPE='CHECK') THEN
   SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT='Room naming: colliding target check CK_Rooms_HostedVehicleOwnership';
  END IF;
  IF EXISTS(SELECT 1 FROM information_schema.TABLE_CONSTRAINTS WHERE CONSTRAINT_SCHEMA=DATABASE() AND LOWER(CONSTRAINT_NAME)='ck_routeroomlandmarks_position' AND CONSTRAINT_TYPE='CHECK') THEN
   SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT='Room naming: colliding target check CK_RouteRoomLandmarks_Position';
  END IF;
  IF EXISTS(SELECT 1 FROM information_schema.TABLE_CONSTRAINTS WHERE CONSTRAINT_SCHEMA=DATABASE() AND LOWER(CONSTRAINT_NAME)='ck_routerooms_defaultposition' AND CONSTRAINT_TYPE='CHECK') THEN
   SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT='Room naming: colliding target check CK_RouteRooms_DefaultPosition';
  END IF;
  IF EXISTS(SELECT 1 FROM information_schema.TABLE_CONSTRAINTS WHERE CONSTRAINT_SCHEMA=DATABASE() AND LOWER(CONSTRAINT_NAME)='ck_routerooms_length' AND CONSTRAINT_TYPE='CHECK') THEN
   SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT='Room naming: colliding target check CK_RouteRooms_Length';
  END IF;
  IF EXISTS(SELECT 1 FROM information_schema.TABLE_CONSTRAINTS WHERE CONSTRAINT_SCHEMA=DATABASE() AND LOWER(CONSTRAINT_NAME)='ck_routerooms_roomequivalent' AND CONSTRAINT_TYPE='CHECK') THEN
   SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT='Room naming: colliding target check CK_RouteRooms_RoomEquivalent';
  END IF;
  IF EXISTS(SELECT 1 FROM information_schema.TABLE_CONSTRAINTS WHERE CONSTRAINT_SCHEMA=DATABASE() AND LOWER(CONSTRAINT_NAME)='ck_routerooms_topologyversion' AND CONSTRAINT_TYPE='CHECK') THEN
   SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT='Room naming: colliding target check CK_RouteRooms_TopologyVersion';
  END IF;
  IF EXISTS(SELECT 1 FROM information_schema.KEY_COLUMN_USAGE WHERE REFERENCED_TABLE_NAME IS NOT NULL AND ((TABLE_SCHEMA=DATABASE() AND (LOWER(TABLE_NAME) IN ('areas_cells','arenacells','cellenvironmentalstates','celloverlaypackages','celloverlays','celloverlays_exits','cellroomareacontractionledger','cellroomareamigrationledger','cellroomcontractionledger','cellroommigrationledger','cells','cells_foragableyields','cells_gameitems','cells_magicresources','cells_rangedcovers','cells_tags','clans_administrationcells','clans_hallcells','clans_treasurycells','legalauthoritiycells','legalauthorityjailcells','npcspawnercells','restaurantcells','routecelllandmarks','routecells','shops_storeroomcells') OR LOWER(CONCAT(TABLE_NAME,'.',COLUMN_NAME)) IN ('activeprojects.cellid','activeroutemotions.routecellid','agriculturefields.cellid','areas_cells.cellid','arenacells.cellid','auctionhouses.auctionhousecellid','bankbranches.cellid','cellenvironmentalstates.cellid','celloverlays.celldescription','celloverlays.cellid','celloverlays.cellname','celloverlays.celloverlaypackageid','celloverlays.celloverlaypackagerevisionnumber','celloverlays_exits.celloverlayid','cellroomareacontractionledger.roomid','cellroomareacontractionledger.cellid','cellroomareamigrationledger.roomid','cellroomareamigrationledger.cellid','cellroomcontractionledger.roomid','cellroomcontractionledger.cellid','cellroommigrationledger.roomid','cellroommigrationledger.cellid','cells_foragableyields.cellid','cells_gameitems.cellid','cells_magicresources.cellid','cells_rangedcovers.cellid','cells_tags.cellid','characterlog.cellid','clans_administrationcells.cellid','clans_hallcells.cellid','clans_treasurycells.cellid','conveyancinglocations.cellid','corpserecoveryreports.destinationcellid','corpserecoveryreports.sourcecellid','employmentactionsteps.destinationcellid','employmentactionsteps.executioncellid','environmentalmagicoperations.cellid','exits.cellid1','exits.cellid2','exits.fallcell','hooks_perceivables.cellid','hospitallocations.cellid','hospitalservicerequests.operatingtheatrecellid','hospitalservicerequests.recoveryroomcellid','hospitalservicerequests.returncellid','hotelrooms.cellid','jobfindinglocations.cellid','landrejuvenationtreatments.cellid','legalauthoritiycells.cellid','legalauthorityjailcells.cellid','magicgatheringoperations.cellid','magicgatheringparticipants.cellid','magicportalendpoints.cellid','npcspawnercells.cellid','patrolroutesnodes.cellid','probatelocations.cellid','propertylocations.cellid','restaurantcells.cellid','routecelllandmarks.routecellid','routecells.cellid','routeexitanchors.routecellid','shops.stockroomcellid','shops.workshopcellid','shops_storeroomcells.cellid','stables.cellid','terrains.defaultcelloutdoorstype','tracks.cellid','vehiclecompartments.interiorcellid','vehicledockings.exteriorcellid','vehiclerouteplatformbindings.platformcellid','vehicleroutesteps.destinationcellid','vehicleroutesteps.origincellid','vehicleroutestops.cellid','vehicleroutetopologypins.routecellid','vehicles.currentcellid','vehicles.destinationcellid','zones.defaultcellid'))) OR (REFERENCED_TABLE_SCHEMA=DATABASE() AND (LOWER(REFERENCED_TABLE_NAME) IN ('areas_cells','arenacells','cellenvironmentalstates','celloverlaypackages','celloverlays','celloverlays_exits','cellroomareacontractionledger','cellroomareamigrationledger','cellroomcontractionledger','cellroommigrationledger','cells','cells_foragableyields','cells_gameitems','cells_magicresources','cells_rangedcovers','cells_tags','clans_administrationcells','clans_hallcells','clans_treasurycells','legalauthoritiycells','legalauthorityjailcells','npcspawnercells','restaurantcells','routecelllandmarks','routecells','shops_storeroomcells') OR LOWER(CONCAT(REFERENCED_TABLE_NAME,'.',REFERENCED_COLUMN_NAME)) IN ('activeprojects.cellid','activeroutemotions.routecellid','agriculturefields.cellid','areas_cells.cellid','arenacells.cellid','auctionhouses.auctionhousecellid','bankbranches.cellid','cellenvironmentalstates.cellid','celloverlays.celldescription','celloverlays.cellid','celloverlays.cellname','celloverlays.celloverlaypackageid','celloverlays.celloverlaypackagerevisionnumber','celloverlays_exits.celloverlayid','cellroomareacontractionledger.roomid','cellroomareacontractionledger.cellid','cellroomareamigrationledger.roomid','cellroomareamigrationledger.cellid','cellroomcontractionledger.roomid','cellroomcontractionledger.cellid','cellroommigrationledger.roomid','cellroommigrationledger.cellid','cells_foragableyields.cellid','cells_gameitems.cellid','cells_magicresources.cellid','cells_rangedcovers.cellid','cells_tags.cellid','characterlog.cellid','clans_administrationcells.cellid','clans_hallcells.cellid','clans_treasurycells.cellid','conveyancinglocations.cellid','corpserecoveryreports.destinationcellid','corpserecoveryreports.sourcecellid','employmentactionsteps.destinationcellid','employmentactionsteps.executioncellid','environmentalmagicoperations.cellid','exits.cellid1','exits.cellid2','exits.fallcell','hooks_perceivables.cellid','hospitallocations.cellid','hospitalservicerequests.operatingtheatrecellid','hospitalservicerequests.recoveryroomcellid','hospitalservicerequests.returncellid','hotelrooms.cellid','jobfindinglocations.cellid','landrejuvenationtreatments.cellid','legalauthoritiycells.cellid','legalauthorityjailcells.cellid','magicgatheringoperations.cellid','magicgatheringparticipants.cellid','magicportalendpoints.cellid','npcspawnercells.cellid','patrolroutesnodes.cellid','probatelocations.cellid','propertylocations.cellid','restaurantcells.cellid','routecelllandmarks.routecellid','routecells.cellid','routeexitanchors.routecellid','shops.stockroomcellid','shops.workshopcellid','shops_storeroomcells.cellid','stables.cellid','terrains.defaultcelloutdoorstype','tracks.cellid','vehiclecompartments.interiorcellid','vehicledockings.exteriorcellid','vehiclerouteplatformbindings.platformcellid','vehicleroutesteps.destinationcellid','vehicleroutesteps.origincellid','vehicleroutestops.cellid','vehicleroutetopologypins.routecellid','vehicles.currentcellid','vehicles.destinationcellid','zones.defaultcellid')))) AND (TABLE_SCHEMA<>DATABASE() OR REFERENCED_TABLE_SCHEMA<>DATABASE() OR LOWER(CONCAT(TABLE_NAME,'.',CONSTRAINT_NAME)) NOT IN ('aistorytellersituations.fk_aistorytellersituations_cells_scoperoomid','activeprojects.fk_activeprojects_cells','activeroutemotions.fk_activeroutemotions_routecells','agriculturefields.fk_agriculturefields_cells','areas_cells.fk_areas_cells_areas','areas_cells.fk_areas_cells_cells','arenacells.fk_arenacells_arenas','arenacells.fk_arenacells_cells','auctionhouses.fk_auctionhouses_cells','bankbranches.fk_bankbranches_cells','cellenvironmentalstates.fk_cellenvironmentalstates_cells','celloverlaypackages.fk_celloverlaypackages_editableitems','celloverlays.fk_celloverlays_celloverlaypackages','celloverlays.fk_celloverlays_cells','celloverlays.fk_celloverlays_hearingprofiles','celloverlays.fk_celloverlays_terrains','celloverlays_exits.fk_celloverlays_exits_celloverlays','celloverlays_exits.fk_celloverlays_exits_exits','cells.fk_cells_celloverlays','cells.fk_cells_hostedvehiclecompartments','cells.fk_cells_hostedvehicles','cells.fk_cells_owningzone','cells_foragableyields.fk_cells_foragableyields_cells','cells_gameitems.fk_cells_gameitems_cells','cells_gameitems.fk_cells_gameitems_gameitems','cells_magicresources.fk_cells_magicresources_cells','cells_magicresources.fk_cells_magicresources_magicresources','cells_rangedcovers.fk_cells_rangedcovers_cells','cells_rangedcovers.fk_cells_rangedcovers_rangedcovers','cells_tags.fk_cells_tags_cells','cells_tags.fk_cells_tags_tags','characterinstances.fk_characterinstances_cells','characterlog.fk_characterlog_cells','characters.fk_characters_cells','clans_administrationcells.fk_clans_administrationcells_cells','clans_administrationcells.fk_clans_administrationcells_clans','clans_hallcells.fk_clans_hallcells_cells','clans_hallcells.fk_clans_hallcells_clans','clans_treasurycells.fk_clans_treasurycells_cells','clans_treasurycells.fk_clans_treasurycells_clans','conveyancinglocations.fk_conveyancinglocations_cells','corpserecoveryreports.fk_corpserecoveryreports_destinationcells','corpserecoveryreports.fk_corpserecoveryreports_sourcecells','crimes.fk_crimes_location','economiczones.fk_economiczones_morgueofficelocations','economiczones.fk_economiczones_morguestoragelocations','hooks_perceivables.fk_hooks_perceivables_cells','hospitallocations.fk_hospitallocations_cells','hospitalservicerequests.fk_hospitalservicerequests_cells_recovery','hospitalservicerequests.fk_hospitalservicerequests_cells_return','hospitalservicerequests.fk_hospitalservicerequests_cells_theatre','hotelrooms.fk_hotelrooms_cells','jobfindinglocations.fk_jobfindinglocations_cells','legalauthorities.fk_legalauthorities_courtroomcell','legalauthorities.fk_legalauthorities_marshallingcells','legalauthorities.fk_legalauthorities_preparingcells','legalauthorities.fk_legalauthorities_prisonbelongingscells','legalauthorities.fk_legalauthorities_prisoncells','legalauthorities.fk_legalauthorities_prisonjailcells','legalauthorities.fk_legalauthorities_prisonreleasecells','legalauthorities.fk_legalauthorities_stowingcells','legalauthoritiycells.fk_legalauthoritiescells_cells','legalauthoritiycells.fk_legalauthoritiescells_legalauthorities','legalauthorityjailcells.fk_legalauthoritiescells_cells_jail','legalauthorityjailcells.fk_legalauthoritiescells_legalauthorities_jail','magicportalendpoints.fk_magicportalendpoints_cells','npcspawnercells.fk_npcspawnercells_cell','npcspawnercells.fk_npcspawnercells_npcspawner','patrolroutesnodes.fk_patrolroutesnodes_cells','patrols.fk_patrols_lastmajornode','patrols.fk_patrols_nextmajornode','probatelocations.fk_probatelocations_cells','propertylocations.fk_propertylocations_cell','restaurantcells.fk_restaurantcells_restaurants','routecelllandmarks.fk_routecelllandmarks_routecells','routecells.fk_routecells_cells','routeexitanchors.fk_routeexitanchors_routecells','shops.fk_shops_cells_stockroom','shops.fk_shops_cells_workshop','shops_storeroomcells.fk_shops_storeroomcells_cells','shops_storeroomcells.fk_shops_storeroomcells_shops','stables.fk_stables_cells','tracks.fk_tracks_cells','vehiclecompartments.fk_vehiclecompartments_interiorcells','vehicledockings.fk_vehicledockings_exteriorcells','vehiclerouteplatformbindings.fk_vehiclerouteplatformbindings_cells','vehicleroutesteps.fk_vehicleroutesteps_destinationcells','vehicleroutesteps.fk_vehicleroutesteps_origincells','vehicleroutestops.fk_vehicleroutestops_cells','vehicleroutetopologypins.fk_vehicleroutetopologypins_routecells','vehicles.fk_vehicles_cells_current','vehicles.fk_vehicles_cells_destination','zones.fk_zones_cells'))) THEN
   SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT='Room naming: unclassified foreign key involving affected schema; review required';
  END IF;
  IF EXISTS(SELECT 1 FROM information_schema.TRIGGERS WHERE TRIGGER_SCHEMA=DATABASE() AND LOWER(EVENT_OBJECT_TABLE) IN ('activeprojects','activeroutemotions','agriculturefields','areas_cells','arenacells','auctionhouses','bankbranches','cellenvironmentalstates','celloverlaypackages','celloverlays','celloverlays_exits','cellroomareacontractionledger','cellroomareamigrationledger','cellroomcontractionledger','cellroommigrationledger','cells','cells_foragableyields','cells_gameitems','cells_magicresources','cells_rangedcovers','cells_tags','characterlog','clans_administrationcells','clans_hallcells','clans_treasurycells','conveyancinglocations','corpserecoveryreports','employmentactionsteps','environmentalmagicoperations','exits','hooks_perceivables','hospitallocations','hospitalservicerequests','hotelrooms','jobfindinglocations','landrejuvenationtreatments','legalauthoritiycells','legalauthorityjailcells','magicgatheringoperations','magicgatheringparticipants','magicportalendpoints','npcspawnercells','patrolroutesnodes','probatelocations','propertylocations','restaurantcells','routecelllandmarks','routecells','routeexitanchors','shops','shops_storeroomcells','stables','terrains','tracks','vehiclecompartments','vehicledockings','vehiclerouteplatformbindings','vehicleroutesteps','vehicleroutestops','vehicleroutetopologypins','vehicles','zones')) THEN
   SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT='Room naming: trigger on affected schema; explicit review required';
  END IF;
  IF EXISTS(SELECT 1 FROM Zones z LEFT JOIN Cells c ON c.Id=z.DefaultCellId WHERE z.DefaultCellId IS NOT NULL AND (c.Id IS NULL OR c.ZoneId<>z.Id)) THEN
   SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT='Room naming: invalid owning-zone default before rename';
  END IF;
  IF EXISTS(SELECT 1 FROM Cells c LEFT JOIN Zones z ON z.Id=c.ZoneId LEFT JOIN CellOverlays o ON o.Id=c.CurrentOverlayId WHERE z.Id IS NULL OR o.Id IS NULL OR o.CellId<>c.Id) THEN
   SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT='Room naming: invalid direct ownership or current overlay';
  END IF;
  OPEN definitions;
  definition_scan: LOOP
   FETCH definitions INTO kind_value,name_value,definition_value;
   IF finished THEN LEAVE definition_scan; END IF;
   IF definition_value IS NULL OR REGEXP_LIKE(definition_value,'(^|[^[:alnum:]_])(Areas_Cells|ArenaCells|AuctionHouseCellId|CK_CellEnvironmentalStates_Pressure|CK_CellEnvironmentalStates_ScarDamage|CK_CellEnvironmentalStates_Versions|CK_Cells_HostedVehicleOwnership|CK_RouteCellLandmarks_Position|CK_RouteCells_DefaultPosition|CK_RouteCells_Length|CK_RouteCells_RoomEquivalent|CK_RouteCells_TopologyVersion|CellDescription|CellEnvironmentalStates|CellId|CellId1|CellId2|CellName|CellOverlayId|CellOverlayPackageId|CellOverlayPackageRevisionNumber|CellOverlayPackages|CellOverlays|CellOverlays_Exits|CellRoomAreaContractionLedger|CellRoomAreaMigrationLedger|CellRoomContractionLedger|CellRoomMigrationLedger|Cells|Cells_ForagableYields|Cells_GameItems|Cells_MagicResources|Cells_RangedCovers|Cells_Tags|Clans_AdministrationCells|Clans_HallCells|Clans_TreasuryCells|CurrentCellId|DefaultCellId|DefaultCellOutdoorsType|DestinationCellId|ExecutionCellId|ExteriorCellId|FK_AIStorytellerSituations_Cells_ScopeRoomId|FK_ActiveProjects_Cells|FK_ActiveProjects_Cells_idx|FK_ActiveRouteMotions_RouteCells|FK_AgricultureFields_Cells|FK_Areas_Cells_Areas|FK_Areas_Cells_Cells|FK_ArenaCells_Arenas|FK_ArenaCells_Cells|FK_AuctionHouses_Cells|FK_BankBranches_Cells|FK_CellEnvironmentalStates_Cells|FK_CellOverlayPackages_EditableItems|FK_CellOverlays_CellOverlayPackages|FK_CellOverlays_Cells|FK_CellOverlays_Exits_CellOverlays|FK_CellOverlays_Exits_Exits|FK_CellOverlays_HearingProfiles|FK_CellOverlays_Terrains|FK_Cells_CellOverlays|FK_Cells_ForagableYields_Cells|FK_Cells_GameItems_Cells|FK_Cells_GameItems_GameItems|FK_Cells_HostedVehicleCompartments|FK_Cells_HostedVehicles|FK_Cells_HostedVehicles_idx|FK_Cells_MagicResources_Cells|FK_Cells_MagicResources_MagicResources|FK_Cells_MagicResources_MagicResources_idx|FK_Cells_OwningZone|FK_Cells_RangedCovers_Cells|FK_Cells_RangedCovers_RangedCovers|FK_Cells_RangedCovers_RangedCovers_idx|FK_Cells_Tags_Cells|FK_Cells_Tags_Tags|FK_Cells_Tags_Tags_idx|FK_CharacterInstances_Cells|FK_CharacterInstances_Cells_idx|FK_CharacterLog_Cells|FK_CharacterLog_Cells_idx|FK_Characters_Cells|FK_Clans_AdministrationCells_Cells|FK_Clans_AdministrationCells_Cells_idx|FK_Clans_AdministrationCells_Clans|FK_Clans_HallCells_Cells|FK_Clans_HallCells_Cells_idx|FK_Clans_HallCells_Clans|FK_Clans_TreasuryCells_Cells|FK_Clans_TreasuryCells_Cells_idx|FK_Clans_TreasuryCells_Clans|FK_ConveyancingLocations_Cells|FK_CorpseRecoveryReports_DestinationCells|FK_CorpseRecoveryReports_SourceCells|FK_Crimes_Location|FK_EconomicZones_MorgueOfficeLocations|FK_EconomicZones_MorgueStorageLocations|FK_Hooks_Perceivables_Cells|FK_Hooks_Perceivables_Cells_idx|FK_HospitalLocations_Cells|FK_HospitalLocations_Cells_idx|FK_HospitalServiceRequests_Cells_Recovery|FK_HospitalServiceRequests_Cells_Recovery_idx|FK_HospitalServiceRequests_Cells_Return|FK_HospitalServiceRequests_Cells_Return_idx|FK_HospitalServiceRequests_Cells_Theatre|FK_HospitalServiceRequests_Cells_Theatre_idx|FK_HotelRooms_Cells|FK_HotelRooms_Cells_idx|FK_JobFindingLocations_Cells|FK_LegalAuthoritiesCells_Cells|FK_LegalAuthoritiesCells_Cells_Jail|FK_LegalAuthoritiesCells_Cells_Jail_idx|FK_LegalAuthoritiesCells_Cells_idx|FK_LegalAuthoritiesCells_LegalAuthorities|FK_LegalAuthoritiesCells_LegalAuthorities_Jail|FK_LegalAuthoritiesCells_LegalAuthorities_Jail_idx|FK_LegalAuthoritiesCells_LegalAuthorities_idx|FK_LegalAuthorities_CourtroomCell|FK_LegalAuthorities_MarshallingCells|FK_LegalAuthorities_MarshallingCells_idx|FK_LegalAuthorities_PreparingCells|FK_LegalAuthorities_PreparingCells_idx|FK_LegalAuthorities_PrisonBelongingsCells|FK_LegalAuthorities_PrisonBelongingsCells_idx|FK_LegalAuthorities_PrisonCells|FK_LegalAuthorities_PrisonCells_idx|FK_LegalAuthorities_PrisonJailCells|FK_LegalAuthorities_PrisonReleaseCells|FK_LegalAuthorities_PrisonReleaseCells_idx|FK_LegalAuthorities_StowingCells|FK_LegalAuthorities_StowingCells_idx|FK_MagicPortalEndpoints_Cells|FK_MagicPortalEndpoints_Cells_idx|FK_NPCSpawnerCells_Cell|FK_NPCSpawnerCells_NPCSpawner|FK_PatrolRoutesNodes_Cells|FK_PatrolRoutesNodes_Cells_idx|FK_Patrols_LastMajorNode|FK_Patrols_NextMajorNode|FK_ProbateLocations_Cells|FK_PropertyLocations_Cell|FK_RestaurantCells_Restaurants|FK_RouteCellLandmarks_RouteCells|FK_RouteCells_Cells|FK_RouteExitAnchors_RouteCells|FK_Shops_Cells_Stockroom|FK_Shops_Cells_Stockroom_idx|FK_Shops_Cells_Workshop|FK_Shops_Cells_Workshop_idx|FK_Shops_StoreroomCells_Cells|FK_Shops_StoreroomCells_Cells_idx|FK_Shops_StoreroomCells_Shops|FK_Stables_Cells|FK_Stables_Cells_idx|FK_Tracks_Cells|FK_VehicleCompartments_InteriorCells|FK_VehicleDockings_ExteriorCells|FK_VehicleRoutePlatformBindings_Cells|FK_VehicleRoutePlatformBindings_Cells_idx|FK_VehicleRouteSteps_DestinationCells|FK_VehicleRouteSteps_DestinationCells_idx|FK_VehicleRouteSteps_OriginCells|FK_VehicleRouteSteps_OriginCells_idx|FK_VehicleRouteStops_Cells|FK_VehicleRouteStops_Cells_idx|FK_VehicleRouteTopologyPins_RouteCells|FK_VehicleRouteTopologyPins_RouteCells_idx|FK_Vehicles_Cells_Current|FK_Vehicles_Cells_Current_idx|FK_Vehicles_Cells_Destination|FK_Vehicles_Cells_Destination_idx|FK_Zones_Cells|FallCell|IX_ActiveRouteMotions_RouteCell_Layer_Status|IX_AgricultureFields_CellId|IX_Areas_Cells_CellId|IX_AuctionHouses_AuctionHouseCellId|IX_BankBranches_CellId|IX_Cells_EnvironmentalMagicProfileId|IX_Cells_UniqueName|IX_Cells_ZoneId|IX_ConveyancingLocations_CellId|IX_CorpseRecoveryReports_DestinationCellId|IX_CorpseRecoveryReports_SourceCellId|IX_EnvironmentalMagicOperations_CellId_AtUtc|IX_HotelRooms_Hotel_Cell|IX_JobFindingLocations_CellId|IX_LandRejuvenationTreatments_CellId_Status|IX_MagicGatheringOperations_Cell_Source_Status|IX_MagicGatheringParticipants_Cell_Source|IX_NPCSpawnerCells_CellId|IX_ProbateLocations_CellId|IX_PropertyLocations_CellId|IX_RestaurantCells_Cell|IX_RestaurantCells_Restaurant_Role|IX_RouteCellLandmarks_RouteCell_Position|IX_RouteExitAnchors_RouteCell_Band|IX_Tracks_Cell_Layer_RoutePosition|IX_VehicleDockings_ExteriorCell_Layer|IX_Vehicles_Cell_Layer_RoutePosition|InteriorCellId|LegalAuthoritiyCells|LegalAuthorityJailCells|NPCSpawnerCells|OperatingTheatreCellId|OriginCellId|PlatformCellId|RecoveryRoomCellId|RestaurantCells|ReturnCellId|RoomId|RouteCellId|RouteCellLandmarks|RouteCells|Shops_StoreroomCells|SourceCellId|StockroomCellId|UX_Cells_HostedVehicleCompartments|UX_VehicleCompartments_InteriorCell|WorkshopCellId)([^[:alnum:]_]|$)','i') THEN
    SET diagnostic=LEFT(CONCAT('Room naming: unclassified or unreadable ',kind_value,' ',name_value),128);
    SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT=diagnostic;
   END IF;
  END LOOP;
  CLOSE definitions;
__CHECK_PREFLIGHT__
  -- These observed legacy variants have validated identical columns, targets
  -- and delete semantics. Capture their exact actions before any foreign-key DDL.
  -- A stale temporary table refuses; never reuse data from a failed cutover.
  CREATE TEMPORARY TABLE `fm_room_naming_fk_actions_20261007043900`(
   ConstraintName VARCHAR(64) NOT NULL PRIMARY KEY,
   TableName VARCHAR(64) NOT NULL,
   UpdateRule VARCHAR(12) NOT NULL,
   DeleteRule VARCHAR(12) NOT NULL);
  INSERT INTO `fm_room_naming_fk_actions_20261007043900`(ConstraintName,TableName,UpdateRule,DeleteRule)
  SELECT 'FK_ActiveProjects_Rooms','ActiveProjects',UPDATE_RULE,DELETE_RULE FROM information_schema.REFERENTIAL_CONSTRAINTS WHERE CONSTRAINT_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='activeprojects' AND LOWER(CONSTRAINT_NAME)='fk_activeprojects_cells'
  UNION ALL
  SELECT 'FK_Rooms_ForagableYields_Rooms','Rooms_ForagableYields',UPDATE_RULE,DELETE_RULE FROM information_schema.REFERENTIAL_CONSTRAINTS WHERE CONSTRAINT_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='cells_foragableyields' AND LOWER(CONSTRAINT_NAME)='fk_cells_foragableyields_cells'
  UNION ALL
  SELECT 'FK_Rooms_MagicResources_Rooms','Rooms_MagicResources',UPDATE_RULE,DELETE_RULE FROM information_schema.REFERENTIAL_CONSTRAINTS WHERE CONSTRAINT_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='cells_magicresources' AND LOWER(CONSTRAINT_NAME)='fk_cells_magicresources_cells'
  UNION ALL
  SELECT 'FK_Rooms_MagicResources_MagicResources','Rooms_MagicResources',UPDATE_RULE,DELETE_RULE FROM information_schema.REFERENTIAL_CONSTRAINTS WHERE CONSTRAINT_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='cells_magicresources' AND LOWER(CONSTRAINT_NAME)='fk_cells_magicresources_magicresources'
  UNION ALL
  SELECT 'FK_Rooms_RangedCovers_Rooms','Rooms_RangedCovers',UPDATE_RULE,DELETE_RULE FROM information_schema.REFERENTIAL_CONSTRAINTS WHERE CONSTRAINT_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='cells_rangedcovers' AND LOWER(CONSTRAINT_NAME)='fk_cells_rangedcovers_cells'
  UNION ALL
  SELECT 'FK_Rooms_RangedCovers_RangedCovers','Rooms_RangedCovers',UPDATE_RULE,DELETE_RULE FROM information_schema.REFERENTIAL_CONSTRAINTS WHERE CONSTRAINT_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='cells_rangedcovers' AND LOWER(CONSTRAINT_NAME)='fk_cells_rangedcovers_rangedcovers'
  UNION ALL
  SELECT 'FK_Rooms_Tags_Tags','Rooms_Tags',UPDATE_RULE,DELETE_RULE FROM information_schema.REFERENTIAL_CONSTRAINTS WHERE CONSTRAINT_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='cells_tags' AND LOWER(CONSTRAINT_NAME)='fk_cells_tags_tags'
  UNION ALL
  SELECT 'FK_CharacterLog_Rooms','CharacterLog',UPDATE_RULE,DELETE_RULE FROM information_schema.REFERENTIAL_CONSTRAINTS WHERE CONSTRAINT_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='characterlog' AND LOWER(CONSTRAINT_NAME)='fk_characterlog_cells'
  UNION ALL
  SELECT 'FK_Clans_AdministrationRooms_Rooms','Clans_AdministrationRooms',UPDATE_RULE,DELETE_RULE FROM information_schema.REFERENTIAL_CONSTRAINTS WHERE CONSTRAINT_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='clans_administrationcells' AND LOWER(CONSTRAINT_NAME)='fk_clans_administrationcells_cells'
  UNION ALL
  SELECT 'FK_Clans_AdministrationRooms_Clans','Clans_AdministrationRooms',UPDATE_RULE,DELETE_RULE FROM information_schema.REFERENTIAL_CONSTRAINTS WHERE CONSTRAINT_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='clans_administrationcells' AND LOWER(CONSTRAINT_NAME)='fk_clans_administrationcells_clans'
  UNION ALL
  SELECT 'FK_Clans_TreasuryRooms_Rooms','Clans_TreasuryRooms',UPDATE_RULE,DELETE_RULE FROM information_schema.REFERENTIAL_CONSTRAINTS WHERE CONSTRAINT_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='clans_treasurycells' AND LOWER(CONSTRAINT_NAME)='fk_clans_treasurycells_cells'
  UNION ALL
  SELECT 'FK_Clans_TreasuryRooms_Clans','Clans_TreasuryRooms',UPDATE_RULE,DELETE_RULE FROM information_schema.REFERENTIAL_CONSTRAINTS WHERE CONSTRAINT_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='clans_treasurycells' AND LOWER(CONSTRAINT_NAME)='fk_clans_treasurycells_clans'
  UNION ALL
  SELECT 'FK_Crimes_Location','Crimes',UPDATE_RULE,DELETE_RULE FROM information_schema.REFERENTIAL_CONSTRAINTS WHERE CONSTRAINT_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='crimes' AND LOWER(CONSTRAINT_NAME)='fk_crimes_location'
  UNION ALL
  SELECT 'FK_Hooks_Perceivables_Rooms','Hooks_Perceivables',UPDATE_RULE,DELETE_RULE FROM information_schema.REFERENTIAL_CONSTRAINTS WHERE CONSTRAINT_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='hooks_perceivables' AND LOWER(CONSTRAINT_NAME)='fk_hooks_perceivables_cells'
  UNION ALL
  SELECT 'FK_Shops_Rooms_Stockroom','Shops',UPDATE_RULE,DELETE_RULE FROM information_schema.REFERENTIAL_CONSTRAINTS WHERE CONSTRAINT_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='shops' AND LOWER(CONSTRAINT_NAME)='fk_shops_cells_stockroom'
  UNION ALL
  SELECT 'FK_Shops_Rooms_Workshop','Shops',UPDATE_RULE,DELETE_RULE FROM information_schema.REFERENTIAL_CONSTRAINTS WHERE CONSTRAINT_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='shops' AND LOWER(CONSTRAINT_NAME)='fk_shops_cells_workshop'
  UNION ALL
  SELECT 'FK_Shops_StoreroomRooms_Rooms','Shops_StoreroomRooms',UPDATE_RULE,DELETE_RULE FROM information_schema.REFERENTIAL_CONSTRAINTS WHERE CONSTRAINT_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='shops_storeroomcells' AND LOWER(CONSTRAINT_NAME)='fk_shops_storeroomcells_cells'
  UNION ALL
  SELECT 'FK_Shops_StoreroomRooms_Shops','Shops_StoreroomRooms',UPDATE_RULE,DELETE_RULE FROM information_schema.REFERENTIAL_CONSTRAINTS WHERE CONSTRAINT_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='shops_storeroomcells' AND LOWER(CONSTRAINT_NAME)='fk_shops_storeroomcells_shops';
  IF (SELECT COUNT(*) FROM `fm_room_naming_fk_actions_20261007043900`)<>18 THEN
   SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT='Room naming: legacy foreign-key action capture is incomplete';
  END IF;
  -- CharacterInstances intentionally omitted physical FKs in its first-upgrade
  -- migration. Preserve that absence as well as supported installed actions.
  CREATE TEMPORARY TABLE `fm_room_naming_optional_fk_20261007043900`(
   ConstraintName VARCHAR(64) NOT NULL PRIMARY KEY,
   IsPresent BOOL NOT NULL,
   UpdateRule VARCHAR(12) NULL,
   DeleteRule VARCHAR(12) NULL);
  INSERT INTO `fm_room_naming_optional_fk_20261007043900`(ConstraintName,IsPresent,UpdateRule,DeleteRule)
  SELECT 'FK_CharacterInstances_Rooms',original.CONSTRAINT_NAME IS NOT NULL,original.UPDATE_RULE,original.DELETE_RULE
  FROM (SELECT 1 AS singleton) singleton
  LEFT JOIN information_schema.REFERENTIAL_CONSTRAINTS original
   ON original.CONSTRAINT_SCHEMA=DATABASE() AND LOWER(original.TABLE_NAME)='characterinstances'
    AND LOWER(original.CONSTRAINT_NAME)='fk_characterinstances_cells';
  IF (SELECT COUNT(*) FROM `fm_room_naming_optional_fk_20261007043900`)<>1 THEN
   SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT='Room naming: optional foreign-key capture is incomplete';
  END IF;
  CREATE TEMPORARY TABLE `fm_room_naming_counts_20261007043900`(TableName VARCHAR(64) NOT NULL PRIMARY KEY,BeforeCount BIGINT NOT NULL);
  INSERT INTO `fm_room_naming_counts_20261007043900` SELECT 'Areas_Rooms',COUNT(*) FROM `Areas_Cells`;
  INSERT INTO `fm_room_naming_counts_20261007043900` SELECT 'ArenaRooms',COUNT(*) FROM `ArenaCells`;
  INSERT INTO `fm_room_naming_counts_20261007043900` SELECT 'RoomEnvironmentalStates',COUNT(*) FROM `CellEnvironmentalStates`;
  INSERT INTO `fm_room_naming_counts_20261007043900` SELECT 'RoomOverlayPackages',COUNT(*) FROM `CellOverlayPackages`;
  INSERT INTO `fm_room_naming_counts_20261007043900` SELECT 'RoomOverlays',COUNT(*) FROM `CellOverlays`;
  INSERT INTO `fm_room_naming_counts_20261007043900` SELECT 'RoomOverlays_Exits',COUNT(*) FROM `CellOverlays_Exits`;
  INSERT INTO `fm_room_naming_counts_20261007043900` SELECT 'RoomSpatialAreaContractionLedger',COUNT(*) FROM `CellRoomAreaContractionLedger`;
  INSERT INTO `fm_room_naming_counts_20261007043900` SELECT 'RoomSpatialAreaMigrationLedger',COUNT(*) FROM `CellRoomAreaMigrationLedger`;
  INSERT INTO `fm_room_naming_counts_20261007043900` SELECT 'RoomSpatialContractionLedger',COUNT(*) FROM `CellRoomContractionLedger`;
  INSERT INTO `fm_room_naming_counts_20261007043900` SELECT 'RoomSpatialMigrationLedger',COUNT(*) FROM `CellRoomMigrationLedger`;
  INSERT INTO `fm_room_naming_counts_20261007043900` SELECT 'Rooms',COUNT(*) FROM `Cells`;
  INSERT INTO `fm_room_naming_counts_20261007043900` SELECT 'Rooms_ForagableYields',COUNT(*) FROM `Cells_ForagableYields`;
  INSERT INTO `fm_room_naming_counts_20261007043900` SELECT 'Rooms_GameItems',COUNT(*) FROM `Cells_GameItems`;
  INSERT INTO `fm_room_naming_counts_20261007043900` SELECT 'Rooms_MagicResources',COUNT(*) FROM `Cells_MagicResources`;
  INSERT INTO `fm_room_naming_counts_20261007043900` SELECT 'Rooms_RangedCovers',COUNT(*) FROM `Cells_RangedCovers`;
  INSERT INTO `fm_room_naming_counts_20261007043900` SELECT 'Rooms_Tags',COUNT(*) FROM `Cells_Tags`;
  INSERT INTO `fm_room_naming_counts_20261007043900` SELECT 'Clans_AdministrationRooms',COUNT(*) FROM `Clans_AdministrationCells`;
  INSERT INTO `fm_room_naming_counts_20261007043900` SELECT 'Clans_HallRooms',COUNT(*) FROM `Clans_HallCells`;
  INSERT INTO `fm_room_naming_counts_20261007043900` SELECT 'Clans_TreasuryRooms',COUNT(*) FROM `Clans_TreasuryCells`;
  INSERT INTO `fm_room_naming_counts_20261007043900` SELECT 'LegalAuthoritiyRooms',COUNT(*) FROM `LegalAuthoritiyCells`;
  INSERT INTO `fm_room_naming_counts_20261007043900` SELECT 'LegalAuthorityJailRooms',COUNT(*) FROM `LegalAuthorityJailCells`;
  INSERT INTO `fm_room_naming_counts_20261007043900` SELECT 'NPCSpawnerRooms',COUNT(*) FROM `NPCSpawnerCells`;
  INSERT INTO `fm_room_naming_counts_20261007043900` SELECT 'RestaurantRooms',COUNT(*) FROM `RestaurantCells`;
  INSERT INTO `fm_room_naming_counts_20261007043900` SELECT 'RouteRoomLandmarks',COUNT(*) FROM `RouteCellLandmarks`;
  INSERT INTO `fm_room_naming_counts_20261007043900` SELECT 'RouteRooms',COUNT(*) FROM `RouteCells`;
  INSERT INTO `fm_room_naming_counts_20261007043900` SELECT 'Shops_StoreroomRooms',COUNT(*) FROM `Shops_StoreroomCells`;
 ELSE
__CHECK_POSTFLIGHT__
  IF (SELECT COUNT(*) FROM `fm_room_naming_optional_fk_20261007043900`)<>1 THEN
   SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT='Room naming: optional foreign-key capture changed after rename';
  END IF;
  IF EXISTS(
   SELECT 1 FROM `fm_room_naming_optional_fk_20261007043900` original
   LEFT JOIN information_schema.REFERENTIAL_CONSTRAINTS current_rule
    ON current_rule.CONSTRAINT_SCHEMA=DATABASE() AND LOWER(current_rule.TABLE_NAME)='characterinstances'
     AND LOWER(current_rule.CONSTRAINT_NAME)='fk_characterinstances_rooms'
   WHERE original.ConstraintName<>'FK_CharacterInstances_Rooms' OR original.IsPresent NOT IN (0,1)
    OR (SELECT COUNT(*) FROM information_schema.TABLE_CONSTRAINTS WHERE CONSTRAINT_SCHEMA=DATABASE() AND LOWER(CONSTRAINT_NAME)='fk_characterinstances_rooms' AND CONSTRAINT_TYPE='FOREIGN KEY')<>original.IsPresent
    OR (original.IsPresent=0 AND (original.UpdateRule IS NOT NULL OR original.DeleteRule IS NOT NULL))
    OR (original.IsPresent=1 AND (original.UpdateRule IS NULL OR original.DeleteRule IS NULL
     OR original.UpdateRule NOT IN ('NO ACTION','RESTRICT') OR original.DeleteRule<>'SET NULL'
     OR current_rule.CONSTRAINT_NAME IS NULL OR current_rule.UPDATE_RULE<>original.UpdateRule
     OR current_rule.DELETE_RULE<>original.DeleteRule
     OR (SELECT COUNT(*) FROM information_schema.KEY_COLUMN_USAGE WHERE TABLE_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='characterinstances' AND LOWER(CONSTRAINT_NAME)='fk_characterinstances_rooms')<>1
     OR (SELECT COUNT(*) FROM information_schema.KEY_COLUMN_USAGE WHERE TABLE_SCHEMA=DATABASE() AND LOWER(TABLE_NAME)='characterinstances' AND LOWER(CONSTRAINT_NAME)='fk_characterinstances_rooms' AND ORDINAL_POSITION=1 AND LOWER(COLUMN_NAME)='locationid' AND REFERENCED_TABLE_SCHEMA=DATABASE() AND LOWER(REFERENCED_TABLE_NAME)='rooms' AND LOWER(REFERENCED_COLUMN_NAME)='id')<>1))) THEN
   SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT='Room naming: optional foreign-key presence, shape or actions changed';
  END IF;
  IF EXISTS(SELECT 1 FROM information_schema.TABLE_CONSTRAINTS WHERE CONSTRAINT_SCHEMA=DATABASE() AND LOWER(CONSTRAINT_NAME)='fk_characterinstances_cells' AND CONSTRAINT_TYPE='FOREIGN KEY') THEN
   SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT='Room naming: old optional foreign-key name remains';
  END IF;
  IF (SELECT COUNT(*) FROM `fm_room_naming_fk_actions_20261007043900`)<>18 THEN
   SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT='Room naming: legacy foreign-key action capture changed after rename';
  END IF;
  IF EXISTS(
   SELECT 1 FROM `fm_room_naming_fk_actions_20261007043900` original
   LEFT JOIN information_schema.REFERENTIAL_CONSTRAINTS current_rule
    ON current_rule.CONSTRAINT_SCHEMA=DATABASE()
     AND LOWER(current_rule.TABLE_NAME)=LOWER(original.TableName)
     AND LOWER(current_rule.CONSTRAINT_NAME)=LOWER(original.ConstraintName)
   WHERE current_rule.CONSTRAINT_NAME IS NULL
    OR current_rule.UPDATE_RULE<>original.UpdateRule
    OR current_rule.DELETE_RULE<>original.DeleteRule) THEN
   SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT='Room naming: original foreign-key actions were not preserved';
  END IF;
  IF (SELECT COUNT(*) FROM `Areas_Rooms`)<>(SELECT BeforeCount FROM `fm_room_naming_counts_20261007043900` WHERE TableName='Areas_Rooms') THEN
   SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT='Room naming: row-count preservation failed Areas_Rooms';
  END IF;
  IF (SELECT COUNT(*) FROM `ArenaRooms`)<>(SELECT BeforeCount FROM `fm_room_naming_counts_20261007043900` WHERE TableName='ArenaRooms') THEN
   SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT='Room naming: row-count preservation failed ArenaRooms';
  END IF;
  IF (SELECT COUNT(*) FROM `RoomEnvironmentalStates`)<>(SELECT BeforeCount FROM `fm_room_naming_counts_20261007043900` WHERE TableName='RoomEnvironmentalStates') THEN
   SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT='Room naming: row-count preservation failed RoomEnvironmentalStates';
  END IF;
  IF (SELECT COUNT(*) FROM `RoomOverlayPackages`)<>(SELECT BeforeCount FROM `fm_room_naming_counts_20261007043900` WHERE TableName='RoomOverlayPackages') THEN
   SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT='Room naming: row-count preservation failed RoomOverlayPackages';
  END IF;
  IF (SELECT COUNT(*) FROM `RoomOverlays`)<>(SELECT BeforeCount FROM `fm_room_naming_counts_20261007043900` WHERE TableName='RoomOverlays') THEN
   SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT='Room naming: row-count preservation failed RoomOverlays';
  END IF;
  IF (SELECT COUNT(*) FROM `RoomOverlays_Exits`)<>(SELECT BeforeCount FROM `fm_room_naming_counts_20261007043900` WHERE TableName='RoomOverlays_Exits') THEN
   SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT='Room naming: row-count preservation failed RoomOverlays_Exits';
  END IF;
  IF (SELECT COUNT(*) FROM `RoomSpatialAreaContractionLedger`)<>(SELECT BeforeCount FROM `fm_room_naming_counts_20261007043900` WHERE TableName='RoomSpatialAreaContractionLedger') THEN
   SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT='Room naming: row-count preservation failed RoomSpatialAreaContractionLedger';
  END IF;
  IF (SELECT COUNT(*) FROM `RoomSpatialAreaMigrationLedger`)<>(SELECT BeforeCount FROM `fm_room_naming_counts_20261007043900` WHERE TableName='RoomSpatialAreaMigrationLedger') THEN
   SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT='Room naming: row-count preservation failed RoomSpatialAreaMigrationLedger';
  END IF;
  IF (SELECT COUNT(*) FROM `RoomSpatialContractionLedger`)<>(SELECT BeforeCount FROM `fm_room_naming_counts_20261007043900` WHERE TableName='RoomSpatialContractionLedger') THEN
   SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT='Room naming: row-count preservation failed RoomSpatialContractionLedger';
  END IF;
  IF (SELECT COUNT(*) FROM `RoomSpatialMigrationLedger`)<>(SELECT BeforeCount FROM `fm_room_naming_counts_20261007043900` WHERE TableName='RoomSpatialMigrationLedger') THEN
   SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT='Room naming: row-count preservation failed RoomSpatialMigrationLedger';
  END IF;
  IF (SELECT COUNT(*) FROM `Rooms`)<>(SELECT BeforeCount FROM `fm_room_naming_counts_20261007043900` WHERE TableName='Rooms') THEN
   SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT='Room naming: row-count preservation failed Rooms';
  END IF;
  IF (SELECT COUNT(*) FROM `Rooms_ForagableYields`)<>(SELECT BeforeCount FROM `fm_room_naming_counts_20261007043900` WHERE TableName='Rooms_ForagableYields') THEN
   SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT='Room naming: row-count preservation failed Rooms_ForagableYields';
  END IF;
  IF (SELECT COUNT(*) FROM `Rooms_GameItems`)<>(SELECT BeforeCount FROM `fm_room_naming_counts_20261007043900` WHERE TableName='Rooms_GameItems') THEN
   SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT='Room naming: row-count preservation failed Rooms_GameItems';
  END IF;
  IF (SELECT COUNT(*) FROM `Rooms_MagicResources`)<>(SELECT BeforeCount FROM `fm_room_naming_counts_20261007043900` WHERE TableName='Rooms_MagicResources') THEN
   SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT='Room naming: row-count preservation failed Rooms_MagicResources';
  END IF;
  IF (SELECT COUNT(*) FROM `Rooms_RangedCovers`)<>(SELECT BeforeCount FROM `fm_room_naming_counts_20261007043900` WHERE TableName='Rooms_RangedCovers') THEN
   SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT='Room naming: row-count preservation failed Rooms_RangedCovers';
  END IF;
  IF (SELECT COUNT(*) FROM `Rooms_Tags`)<>(SELECT BeforeCount FROM `fm_room_naming_counts_20261007043900` WHERE TableName='Rooms_Tags') THEN
   SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT='Room naming: row-count preservation failed Rooms_Tags';
  END IF;
  IF (SELECT COUNT(*) FROM `Clans_AdministrationRooms`)<>(SELECT BeforeCount FROM `fm_room_naming_counts_20261007043900` WHERE TableName='Clans_AdministrationRooms') THEN
   SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT='Room naming: row-count preservation failed Clans_AdministrationRooms';
  END IF;
  IF (SELECT COUNT(*) FROM `Clans_HallRooms`)<>(SELECT BeforeCount FROM `fm_room_naming_counts_20261007043900` WHERE TableName='Clans_HallRooms') THEN
   SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT='Room naming: row-count preservation failed Clans_HallRooms';
  END IF;
  IF (SELECT COUNT(*) FROM `Clans_TreasuryRooms`)<>(SELECT BeforeCount FROM `fm_room_naming_counts_20261007043900` WHERE TableName='Clans_TreasuryRooms') THEN
   SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT='Room naming: row-count preservation failed Clans_TreasuryRooms';
  END IF;
  IF (SELECT COUNT(*) FROM `LegalAuthoritiyRooms`)<>(SELECT BeforeCount FROM `fm_room_naming_counts_20261007043900` WHERE TableName='LegalAuthoritiyRooms') THEN
   SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT='Room naming: row-count preservation failed LegalAuthoritiyRooms';
  END IF;
  IF (SELECT COUNT(*) FROM `LegalAuthorityJailRooms`)<>(SELECT BeforeCount FROM `fm_room_naming_counts_20261007043900` WHERE TableName='LegalAuthorityJailRooms') THEN
   SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT='Room naming: row-count preservation failed LegalAuthorityJailRooms';
  END IF;
  IF (SELECT COUNT(*) FROM `NPCSpawnerRooms`)<>(SELECT BeforeCount FROM `fm_room_naming_counts_20261007043900` WHERE TableName='NPCSpawnerRooms') THEN
   SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT='Room naming: row-count preservation failed NPCSpawnerRooms';
  END IF;
  IF (SELECT COUNT(*) FROM `RestaurantRooms`)<>(SELECT BeforeCount FROM `fm_room_naming_counts_20261007043900` WHERE TableName='RestaurantRooms') THEN
   SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT='Room naming: row-count preservation failed RestaurantRooms';
  END IF;
  IF (SELECT COUNT(*) FROM `RouteRoomLandmarks`)<>(SELECT BeforeCount FROM `fm_room_naming_counts_20261007043900` WHERE TableName='RouteRoomLandmarks') THEN
   SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT='Room naming: row-count preservation failed RouteRoomLandmarks';
  END IF;
  IF (SELECT COUNT(*) FROM `RouteRooms`)<>(SELECT BeforeCount FROM `fm_room_naming_counts_20261007043900` WHERE TableName='RouteRooms') THEN
   SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT='Room naming: row-count preservation failed RouteRooms';
  END IF;
  IF (SELECT COUNT(*) FROM `Shops_StoreroomRooms`)<>(SELECT BeforeCount FROM `fm_room_naming_counts_20261007043900` WHERE TableName='Shops_StoreroomRooms') THEN
   SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT='Room naming: row-count preservation failed Shops_StoreroomRooms';
  END IF;
  IF EXISTS(SELECT 1 FROM Zones z LEFT JOIN Rooms r ON r.Id=z.DefaultRoomId WHERE z.DefaultRoomId IS NOT NULL AND (r.Id IS NULL OR r.ZoneId<>z.Id)) THEN
   SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT='Room naming: invalid owning-zone default after rename';
  END IF;
  DROP TEMPORARY TABLE `fm_room_naming_counts_20261007043900`;
  DROP TEMPORARY TABLE `fm_room_naming_fk_actions_20261007043900`;
  DROP TEMPORARY TABLE `fm_room_naming_optional_fk_20261007043900`;
 END IF;
END;
""".Replace("__CHECK_PREFLIGHT__", ReviewedCheckGuard(false))
   .Replace("__CHECK_POSTFLIGHT__", ReviewedCheckGuard(true)), suppressTransaction: true);
  migrationBuilder.Sql("CALL `fm_room_terminology_20261007043900`(FALSE);", suppressTransaction: true);
			migrationBuilder.DropForeignKey(name: "FK_ActiveProjects_Cells", table: "ActiveProjects");
			migrationBuilder.DropForeignKey(name: "FK_ActiveRouteMotions_RouteCells", table: "ActiveRouteMotions");
			migrationBuilder.DropForeignKey(name: "FK_AgricultureFields_Cells", table: "AgricultureFields");
			migrationBuilder.DropForeignKey(name: "FK_AIStorytellerSituations_Cells_ScopeRoomId", table: "AIStorytellerSituations");
			migrationBuilder.DropForeignKey(name: "FK_Areas_Cells_Areas", table: "Areas_Cells");
			migrationBuilder.DropForeignKey(name: "FK_Areas_Cells_Cells", table: "Areas_Cells");
			migrationBuilder.DropForeignKey(name: "FK_ArenaCells_Arenas", table: "ArenaCells");
			migrationBuilder.DropForeignKey(name: "FK_ArenaCells_Cells", table: "ArenaCells");
			migrationBuilder.DropForeignKey(name: "FK_AuctionHouses_Cells", table: "AuctionHouses");
			migrationBuilder.DropForeignKey(name: "FK_BankBranches_Cells", table: "BankBranches");
			migrationBuilder.DropForeignKey(name: "FK_CellEnvironmentalStates_Cells", table: "CellEnvironmentalStates");
			migrationBuilder.DropForeignKey(name: "FK_CellOverlayPackages_EditableItems", table: "CellOverlayPackages");
			migrationBuilder.DropForeignKey(name: "FK_CellOverlays_CellOverlayPackages", table: "CellOverlays");
			migrationBuilder.DropForeignKey(name: "FK_CellOverlays_Cells", table: "CellOverlays");
			migrationBuilder.DropForeignKey(name: "FK_CellOverlays_HearingProfiles", table: "CellOverlays");
			migrationBuilder.DropForeignKey(name: "FK_CellOverlays_Terrains", table: "CellOverlays");
			migrationBuilder.DropForeignKey(name: "FK_CellOverlays_Exits_CellOverlays", table: "CellOverlays_Exits");
			migrationBuilder.DropForeignKey(name: "FK_CellOverlays_Exits_Exits", table: "CellOverlays_Exits");
			migrationBuilder.DropForeignKey(name: "FK_Cells_CellOverlays", table: "Cells");
			migrationBuilder.DropForeignKey(name: "FK_Cells_HostedVehicleCompartments", table: "Cells");
			migrationBuilder.DropForeignKey(name: "FK_Cells_HostedVehicles", table: "Cells");
			migrationBuilder.DropForeignKey(name: "FK_Cells_OwningZone", table: "Cells");
			migrationBuilder.DropForeignKey(name: "FK_Cells_ForagableYields_Cells", table: "Cells_ForagableYields");
			migrationBuilder.DropForeignKey(name: "FK_Cells_GameItems_Cells", table: "Cells_GameItems");
			migrationBuilder.DropForeignKey(name: "FK_Cells_GameItems_GameItems", table: "Cells_GameItems");
			migrationBuilder.DropForeignKey(name: "FK_Cells_MagicResources_Cells", table: "Cells_MagicResources");
			migrationBuilder.DropForeignKey(name: "FK_Cells_MagicResources_MagicResources", table: "Cells_MagicResources");
			migrationBuilder.DropForeignKey(name: "FK_Cells_RangedCovers_Cells", table: "Cells_RangedCovers");
			migrationBuilder.DropForeignKey(name: "FK_Cells_RangedCovers_RangedCovers", table: "Cells_RangedCovers");
			migrationBuilder.DropForeignKey(name: "FK_Cells_Tags_Cells", table: "Cells_Tags");
			migrationBuilder.DropForeignKey(name: "FK_Cells_Tags_Tags", table: "Cells_Tags");
			ChangeOptionalCharacterInstanceForeignKey(migrationBuilder, restore: false);
			migrationBuilder.DropForeignKey(name: "FK_CharacterLog_Cells", table: "CharacterLog");
			migrationBuilder.DropForeignKey(name: "FK_Characters_Cells", table: "Characters");
			migrationBuilder.DropForeignKey(name: "FK_Clans_AdministrationCells_Cells", table: "Clans_AdministrationCells");
			migrationBuilder.DropForeignKey(name: "FK_Clans_AdministrationCells_Clans", table: "Clans_AdministrationCells");
			migrationBuilder.DropForeignKey(name: "FK_Clans_HallCells_Cells", table: "Clans_HallCells");
			migrationBuilder.DropForeignKey(name: "FK_Clans_HallCells_Clans", table: "Clans_HallCells");
			migrationBuilder.DropForeignKey(name: "FK_Clans_TreasuryCells_Cells", table: "Clans_TreasuryCells");
			migrationBuilder.DropForeignKey(name: "FK_Clans_TreasuryCells_Clans", table: "Clans_TreasuryCells");
			migrationBuilder.DropForeignKey(name: "FK_ConveyancingLocations_Cells", table: "ConveyancingLocations");
			migrationBuilder.DropForeignKey(name: "FK_CorpseRecoveryReports_DestinationCells", table: "CorpseRecoveryReports");
			migrationBuilder.DropForeignKey(name: "FK_CorpseRecoveryReports_SourceCells", table: "CorpseRecoveryReports");
			migrationBuilder.DropForeignKey(name: "FK_Crimes_Location", table: "Crimes");
			migrationBuilder.DropForeignKey(name: "FK_EconomicZones_MorgueOfficeLocations", table: "EconomicZones");
			migrationBuilder.DropForeignKey(name: "FK_EconomicZones_MorgueStorageLocations", table: "EconomicZones");
			migrationBuilder.DropForeignKey(name: "FK_Hooks_Perceivables_Cells", table: "Hooks_Perceivables");
			migrationBuilder.DropForeignKey(name: "FK_HospitalLocations_Cells", table: "HospitalLocations");
			migrationBuilder.DropForeignKey(name: "FK_HospitalServiceRequests_Cells_Recovery", table: "HospitalServiceRequests");
			migrationBuilder.DropForeignKey(name: "FK_HospitalServiceRequests_Cells_Return", table: "HospitalServiceRequests");
			migrationBuilder.DropForeignKey(name: "FK_HospitalServiceRequests_Cells_Theatre", table: "HospitalServiceRequests");
			migrationBuilder.DropForeignKey(name: "FK_HotelRooms_Cells", table: "HotelRooms");
			migrationBuilder.DropForeignKey(name: "FK_JobFindingLocations_Cells", table: "JobFindingLocations");
			migrationBuilder.DropForeignKey(name: "FK_LegalAuthorities_CourtroomCell", table: "LegalAuthorities");
			migrationBuilder.DropForeignKey(name: "FK_LegalAuthorities_MarshallingCells", table: "LegalAuthorities");
			migrationBuilder.DropForeignKey(name: "FK_LegalAuthorities_PreparingCells", table: "LegalAuthorities");
			migrationBuilder.DropForeignKey(name: "FK_LegalAuthorities_PrisonBelongingsCells", table: "LegalAuthorities");
			migrationBuilder.DropForeignKey(name: "FK_LegalAuthorities_PrisonCells", table: "LegalAuthorities");
			migrationBuilder.DropForeignKey(name: "FK_LegalAuthorities_PrisonJailCells", table: "LegalAuthorities");
			migrationBuilder.DropForeignKey(name: "FK_LegalAuthorities_PrisonReleaseCells", table: "LegalAuthorities");
			migrationBuilder.DropForeignKey(name: "FK_LegalAuthorities_StowingCells", table: "LegalAuthorities");
			migrationBuilder.DropForeignKey(name: "FK_LegalAuthoritiesCells_Cells", table: "LegalAuthoritiyCells");
			migrationBuilder.DropForeignKey(name: "FK_LegalAuthoritiesCells_LegalAuthorities", table: "LegalAuthoritiyCells");
			migrationBuilder.DropForeignKey(name: "FK_LegalAuthoritiesCells_Cells_Jail", table: "LegalAuthorityJailCells");
			migrationBuilder.DropForeignKey(name: "FK_LegalAuthoritiesCells_LegalAuthorities_Jail", table: "LegalAuthorityJailCells");
			migrationBuilder.DropForeignKey(name: "FK_MagicPortalEndpoints_Cells", table: "MagicPortalEndpoints");
			migrationBuilder.DropForeignKey(name: "FK_NPCSpawnerCells_Cell", table: "NPCSpawnerCells");
			migrationBuilder.DropForeignKey(name: "FK_NPCSpawnerCells_NPCSpawner", table: "NPCSpawnerCells");
			migrationBuilder.DropForeignKey(name: "FK_PatrolRoutesNodes_Cells", table: "PatrolRoutesNodes");
			migrationBuilder.DropForeignKey(name: "FK_Patrols_LastMajorNode", table: "Patrols");
			migrationBuilder.DropForeignKey(name: "FK_Patrols_NextMajorNode", table: "Patrols");
			migrationBuilder.DropForeignKey(name: "FK_ProbateLocations_Cells", table: "ProbateLocations");
			migrationBuilder.DropForeignKey(name: "FK_PropertyLocations_Cell", table: "PropertyLocations");
			migrationBuilder.DropForeignKey(name: "FK_RestaurantCells_Restaurants", table: "RestaurantCells");
			migrationBuilder.DropForeignKey(name: "FK_RouteCellLandmarks_RouteCells", table: "RouteCellLandmarks");
			migrationBuilder.DropForeignKey(name: "FK_RouteCells_Cells", table: "RouteCells");
			migrationBuilder.DropForeignKey(name: "FK_RouteExitAnchors_RouteCells", table: "RouteExitAnchors");
			migrationBuilder.DropForeignKey(name: "FK_Shops_Cells_Stockroom", table: "Shops");
			migrationBuilder.DropForeignKey(name: "FK_Shops_Cells_Workshop", table: "Shops");
			migrationBuilder.DropForeignKey(name: "FK_Shops_StoreroomCells_Cells", table: "Shops_StoreroomCells");
			migrationBuilder.DropForeignKey(name: "FK_Shops_StoreroomCells_Shops", table: "Shops_StoreroomCells");
			migrationBuilder.DropForeignKey(name: "FK_Stables_Cells", table: "Stables");
			migrationBuilder.DropForeignKey(name: "FK_Tracks_Cells", table: "Tracks");
			migrationBuilder.DropForeignKey(name: "FK_VehicleCompartments_InteriorCells", table: "VehicleCompartments");
			migrationBuilder.DropForeignKey(name: "FK_VehicleDockings_ExteriorCells", table: "VehicleDockings");
			migrationBuilder.DropForeignKey(name: "FK_VehicleRoutePlatformBindings_Cells", table: "VehicleRoutePlatformBindings");
			migrationBuilder.DropForeignKey(name: "FK_VehicleRouteSteps_DestinationCells", table: "VehicleRouteSteps");
			migrationBuilder.DropForeignKey(name: "FK_VehicleRouteSteps_OriginCells", table: "VehicleRouteSteps");
			migrationBuilder.DropForeignKey(name: "FK_VehicleRouteStops_Cells", table: "VehicleRouteStops");
			migrationBuilder.DropForeignKey(name: "FK_VehicleRouteTopologyPins_RouteCells", table: "VehicleRouteTopologyPins");
			migrationBuilder.DropForeignKey(name: "FK_Vehicles_Cells_Current", table: "Vehicles");
			migrationBuilder.DropForeignKey(name: "FK_Vehicles_Cells_Destination", table: "Vehicles");
			migrationBuilder.DropForeignKey(name: "FK_Zones_Cells", table: "Zones");
			migrationBuilder.DropCheckConstraint(name: "CK_CellEnvironmentalStates_Pressure", table: "CellEnvironmentalStates");
			migrationBuilder.DropCheckConstraint(name: "CK_CellEnvironmentalStates_ScarDamage", table: "CellEnvironmentalStates");
			migrationBuilder.DropCheckConstraint(name: "CK_CellEnvironmentalStates_Versions", table: "CellEnvironmentalStates");
			migrationBuilder.DropCheckConstraint(name: "CK_Cells_HostedVehicleOwnership", table: "Cells");
			migrationBuilder.DropCheckConstraint(name: "CK_RouteCellLandmarks_Position", table: "RouteCellLandmarks");
			migrationBuilder.DropCheckConstraint(name: "CK_RouteCells_DefaultPosition", table: "RouteCells");
			migrationBuilder.DropCheckConstraint(name: "CK_RouteCells_Length", table: "RouteCells");
			migrationBuilder.DropCheckConstraint(name: "CK_RouteCells_RoomEquivalent", table: "RouteCells");
			migrationBuilder.DropCheckConstraint(name: "CK_RouteCells_TopologyVersion", table: "RouteCells");
			migrationBuilder.DropCheckConstraint(name: "CK_VehicleRouteSteps_TypedPayload", table: "VehicleRouteSteps");
			migrationBuilder.RenameTable(name: "Areas_Cells", newName: "Areas_Rooms");
			migrationBuilder.RenameTable(name: "ArenaCells", newName: "ArenaRooms");
			migrationBuilder.RenameTable(name: "CellEnvironmentalStates", newName: "RoomEnvironmentalStates");
			migrationBuilder.RenameTable(name: "CellOverlayPackages", newName: "RoomOverlayPackages");
			migrationBuilder.RenameTable(name: "CellOverlays", newName: "RoomOverlays");
			migrationBuilder.RenameTable(name: "CellOverlays_Exits", newName: "RoomOverlays_Exits");
			migrationBuilder.RenameTable(name: "CellRoomAreaContractionLedger", newName: "RoomSpatialAreaContractionLedger");
			migrationBuilder.RenameTable(name: "CellRoomAreaMigrationLedger", newName: "RoomSpatialAreaMigrationLedger");
			migrationBuilder.RenameTable(name: "CellRoomContractionLedger", newName: "RoomSpatialContractionLedger");
			migrationBuilder.RenameTable(name: "CellRoomMigrationLedger", newName: "RoomSpatialMigrationLedger");
			migrationBuilder.RenameTable(name: "Cells", newName: "Rooms");
			migrationBuilder.RenameTable(name: "Cells_ForagableYields", newName: "Rooms_ForagableYields");
			migrationBuilder.RenameTable(name: "Cells_GameItems", newName: "Rooms_GameItems");
			migrationBuilder.RenameTable(name: "Cells_MagicResources", newName: "Rooms_MagicResources");
			migrationBuilder.RenameTable(name: "Cells_RangedCovers", newName: "Rooms_RangedCovers");
			migrationBuilder.RenameTable(name: "Cells_Tags", newName: "Rooms_Tags");
			migrationBuilder.RenameTable(name: "Clans_AdministrationCells", newName: "Clans_AdministrationRooms");
			migrationBuilder.RenameTable(name: "Clans_HallCells", newName: "Clans_HallRooms");
			migrationBuilder.RenameTable(name: "Clans_TreasuryCells", newName: "Clans_TreasuryRooms");
			migrationBuilder.RenameTable(name: "LegalAuthoritiyCells", newName: "LegalAuthoritiyRooms");
			migrationBuilder.RenameTable(name: "LegalAuthorityJailCells", newName: "LegalAuthorityJailRooms");
			migrationBuilder.RenameTable(name: "NPCSpawnerCells", newName: "NPCSpawnerRooms");
			migrationBuilder.RenameTable(name: "RestaurantCells", newName: "RestaurantRooms");
			migrationBuilder.RenameTable(name: "RouteCellLandmarks", newName: "RouteRoomLandmarks");
			migrationBuilder.RenameTable(name: "RouteCells", newName: "RouteRooms");
			migrationBuilder.RenameTable(name: "Shops_StoreroomCells", newName: "Shops_StoreroomRooms");
			migrationBuilder.RenameColumn(name: "CellId", table: "ActiveProjects", newName: "RoomId");
			migrationBuilder.RenameColumn(name: "RouteCellId", table: "ActiveRouteMotions", newName: "RouteRoomId");
			migrationBuilder.RenameColumn(name: "CellId", table: "AgricultureFields", newName: "RoomId");
			migrationBuilder.RenameColumn(name: "CellId", table: "Areas_Rooms", newName: "RoomId");
			migrationBuilder.RenameColumn(name: "CellId", table: "ArenaRooms", newName: "RoomId");
			migrationBuilder.RenameColumn(name: "AuctionHouseCellId", table: "AuctionHouses", newName: "AuctionHouseRoomId");
			migrationBuilder.RenameColumn(name: "CellId", table: "BankBranches", newName: "RoomId");
			migrationBuilder.RenameColumn(name: "CellId", table: "RoomEnvironmentalStates", newName: "RoomId");
			migrationBuilder.RenameColumn(name: "CellDescription", table: "RoomOverlays", newName: "RoomDescription");
			migrationBuilder.RenameColumn(name: "CellId", table: "RoomOverlays", newName: "RoomId");
			migrationBuilder.RenameColumn(name: "CellName", table: "RoomOverlays", newName: "RoomName");
			migrationBuilder.RenameColumn(name: "CellOverlayPackageId", table: "RoomOverlays", newName: "RoomOverlayPackageId");
			migrationBuilder.RenameColumn(name: "CellOverlayPackageRevisionNumber", table: "RoomOverlays", newName: "RoomOverlayPackageRevisionNumber");
			migrationBuilder.RenameColumn(name: "CellOverlayId", table: "RoomOverlays_Exits", newName: "RoomOverlayId");
			migrationBuilder.RenameColumn(name: "RoomId", table: "RoomSpatialAreaContractionLedger", newName: "LegacyRoomId");
			migrationBuilder.RenameColumn(name: "CellId", table: "RoomSpatialAreaContractionLedger", newName: "RoomId");
			migrationBuilder.RenameColumn(name: "RoomId", table: "RoomSpatialAreaMigrationLedger", newName: "LegacyRoomId");
			migrationBuilder.RenameColumn(name: "CellId", table: "RoomSpatialAreaMigrationLedger", newName: "RoomId");
			migrationBuilder.RenameColumn(name: "RoomId", table: "RoomSpatialContractionLedger", newName: "LegacyRoomId");
			migrationBuilder.RenameColumn(name: "CellId", table: "RoomSpatialContractionLedger", newName: "RoomId");
			migrationBuilder.RenameColumn(name: "RoomId", table: "RoomSpatialMigrationLedger", newName: "LegacyRoomId");
			migrationBuilder.RenameColumn(name: "CellId", table: "RoomSpatialMigrationLedger", newName: "RoomId");
			migrationBuilder.RenameColumn(name: "CellId", table: "Rooms_ForagableYields", newName: "RoomId");
			migrationBuilder.RenameColumn(name: "CellId", table: "Rooms_GameItems", newName: "RoomId");
			migrationBuilder.RenameColumn(name: "CellId", table: "Rooms_MagicResources", newName: "RoomId");
			migrationBuilder.RenameColumn(name: "CellId", table: "Rooms_RangedCovers", newName: "RoomId");
			migrationBuilder.RenameColumn(name: "CellId", table: "Rooms_Tags", newName: "RoomId");
			migrationBuilder.RenameColumn(name: "CellId", table: "CharacterLog", newName: "RoomId");
			migrationBuilder.RenameColumn(name: "CellId", table: "Clans_AdministrationRooms", newName: "RoomId");
			migrationBuilder.RenameColumn(name: "CellId", table: "Clans_HallRooms", newName: "RoomId");
			migrationBuilder.RenameColumn(name: "CellId", table: "Clans_TreasuryRooms", newName: "RoomId");
			migrationBuilder.RenameColumn(name: "CellId", table: "ConveyancingLocations", newName: "RoomId");
			migrationBuilder.RenameColumn(name: "DestinationCellId", table: "CorpseRecoveryReports", newName: "DestinationRoomId");
			migrationBuilder.RenameColumn(name: "SourceCellId", table: "CorpseRecoveryReports", newName: "SourceRoomId");
			migrationBuilder.RenameColumn(name: "DestinationCellId", table: "EmploymentActionSteps", newName: "DestinationRoomId");
			migrationBuilder.RenameColumn(name: "ExecutionCellId", table: "EmploymentActionSteps", newName: "ExecutionRoomId");
			migrationBuilder.RenameColumn(name: "CellId", table: "EnvironmentalMagicOperations", newName: "RoomId");
			migrationBuilder.RenameColumn(name: "CellId1", table: "Exits", newName: "RoomId1");
			migrationBuilder.RenameColumn(name: "CellId2", table: "Exits", newName: "RoomId2");
			migrationBuilder.RenameColumn(name: "FallCell", table: "Exits", newName: "FallRoom");
			migrationBuilder.RenameColumn(name: "CellId", table: "Hooks_Perceivables", newName: "RoomId");
			migrationBuilder.RenameColumn(name: "CellId", table: "HospitalLocations", newName: "RoomId");
			migrationBuilder.RenameColumn(name: "OperatingTheatreCellId", table: "HospitalServiceRequests", newName: "OperatingTheatreRoomId");
			migrationBuilder.RenameColumn(name: "RecoveryRoomCellId", table: "HospitalServiceRequests", newName: "RecoveryRoomId");
			migrationBuilder.RenameColumn(name: "ReturnCellId", table: "HospitalServiceRequests", newName: "ReturnRoomId");
			migrationBuilder.RenameColumn(name: "CellId", table: "HotelRooms", newName: "RoomId");
			migrationBuilder.RenameColumn(name: "CellId", table: "JobFindingLocations", newName: "RoomId");
			migrationBuilder.RenameColumn(name: "CellId", table: "LandRejuvenationTreatments", newName: "RoomId");
			migrationBuilder.RenameColumn(name: "CellId", table: "LegalAuthoritiyRooms", newName: "RoomId");
			migrationBuilder.RenameColumn(name: "CellId", table: "LegalAuthorityJailRooms", newName: "RoomId");
			migrationBuilder.RenameColumn(name: "CellId", table: "MagicGatheringOperations", newName: "RoomId");
			migrationBuilder.RenameColumn(name: "CellId", table: "MagicGatheringParticipants", newName: "RoomId");
			migrationBuilder.RenameColumn(name: "CellId", table: "MagicPortalEndpoints", newName: "RoomId");
			migrationBuilder.RenameColumn(name: "CellId", table: "NPCSpawnerRooms", newName: "RoomId");
			migrationBuilder.RenameColumn(name: "CellId", table: "PatrolRoutesNodes", newName: "RoomId");
			migrationBuilder.RenameColumn(name: "CellId", table: "ProbateLocations", newName: "RoomId");
			migrationBuilder.RenameColumn(name: "CellId", table: "PropertyLocations", newName: "RoomId");
			migrationBuilder.RenameColumn(name: "CellId", table: "RestaurantRooms", newName: "RoomId");
			migrationBuilder.RenameColumn(name: "RouteCellId", table: "RouteRoomLandmarks", newName: "RouteRoomId");
			migrationBuilder.RenameColumn(name: "CellId", table: "RouteRooms", newName: "RoomId");
			migrationBuilder.RenameColumn(name: "RouteCellId", table: "RouteExitAnchors", newName: "RouteRoomId");
			migrationBuilder.RenameColumn(name: "StockroomCellId", table: "Shops", newName: "StockroomId");
			migrationBuilder.RenameColumn(name: "WorkshopCellId", table: "Shops", newName: "WorkshopRoomId");
			migrationBuilder.RenameColumn(name: "CellId", table: "Shops_StoreroomRooms", newName: "RoomId");
			migrationBuilder.RenameColumn(name: "CellId", table: "Stables", newName: "RoomId");
			migrationBuilder.RenameColumn(name: "DefaultCellOutdoorsType", table: "Terrains", newName: "DefaultRoomOutdoorsType");
			migrationBuilder.RenameColumn(name: "CellId", table: "Tracks", newName: "RoomId");
			migrationBuilder.RenameColumn(name: "InteriorCellId", table: "VehicleCompartments", newName: "InteriorRoomId");
			migrationBuilder.RenameColumn(name: "ExteriorCellId", table: "VehicleDockings", newName: "ExteriorRoomId");
			migrationBuilder.RenameColumn(name: "PlatformCellId", table: "VehicleRoutePlatformBindings", newName: "PlatformRoomId");
			migrationBuilder.RenameColumn(name: "DestinationCellId", table: "VehicleRouteSteps", newName: "DestinationRoomId");
			migrationBuilder.RenameColumn(name: "OriginCellId", table: "VehicleRouteSteps", newName: "OriginRoomId");
			migrationBuilder.RenameColumn(name: "CellId", table: "VehicleRouteStops", newName: "RoomId");
			migrationBuilder.RenameColumn(name: "RouteCellId", table: "VehicleRouteTopologyPins", newName: "RouteRoomId");
			migrationBuilder.RenameColumn(name: "CurrentCellId", table: "Vehicles", newName: "CurrentRoomId");
			migrationBuilder.RenameColumn(name: "DestinationCellId", table: "Vehicles", newName: "DestinationRoomId");
			migrationBuilder.RenameColumn(name: "DefaultCellId", table: "Zones", newName: "DefaultRoomId");
			migrationBuilder.RenameIndex(name: "FK_ActiveProjects_Cells_idx", table: "ActiveProjects", newName: "FK_ActiveProjects_Rooms_idx");
			migrationBuilder.RenameIndex(name: "IX_ActiveRouteMotions_RouteCell_Layer_Status", table: "ActiveRouteMotions", newName: "IX_ActiveRouteMotions_RouteRoom_Layer_Status");
			migrationBuilder.RenameIndex(name: "IX_AgricultureFields_CellId", table: "AgricultureFields", newName: "IX_AgricultureFields_RoomId");
			migrationBuilder.RenameIndex(name: "IX_Areas_Cells_CellId", table: "Areas_Rooms", newName: "IX_Areas_Rooms_RoomId");
			migrationBuilder.RenameIndex(name: "FK_ArenaCells_Arenas", table: "ArenaRooms", newName: "FK_ArenaRooms_Arenas");
			migrationBuilder.RenameIndex(name: "FK_ArenaCells_Cells", table: "ArenaRooms", newName: "FK_ArenaRooms_Rooms");
			migrationBuilder.RenameIndex(name: "IX_AuctionHouses_AuctionHouseCellId", table: "AuctionHouses", newName: "IX_AuctionHouses_AuctionHouseRoomId");
			migrationBuilder.RenameIndex(name: "IX_BankBranches_CellId", table: "BankBranches", newName: "IX_BankBranches_RoomId");
			migrationBuilder.RenameIndex(name: "FK_CellOverlayPackages_EditableItems", table: "RoomOverlayPackages", newName: "FK_RoomOverlayPackages_EditableItems");
			migrationBuilder.RenameIndex(name: "FK_CellOverlays_CellOverlayPackages", table: "RoomOverlays", newName: "FK_RoomOverlays_RoomOverlayPackages");
			migrationBuilder.RenameIndex(name: "FK_CellOverlays_Cells", table: "RoomOverlays", newName: "FK_RoomOverlays_Rooms");
			migrationBuilder.RenameIndex(name: "FK_CellOverlays_HearingProfiles", table: "RoomOverlays", newName: "FK_RoomOverlays_HearingProfiles");
			migrationBuilder.RenameIndex(name: "FK_CellOverlays_Terrains", table: "RoomOverlays", newName: "FK_RoomOverlays_Terrains");
			migrationBuilder.RenameIndex(name: "FK_CellOverlays_Exits_Exits", table: "RoomOverlays_Exits", newName: "FK_RoomOverlays_Exits_Exits");
			migrationBuilder.RenameIndex(name: "FK_Cells_CellOverlays", table: "Rooms", newName: "FK_Rooms_RoomOverlays");
			migrationBuilder.RenameIndex(name: "FK_Cells_HostedVehicles_idx", table: "Rooms", newName: "FK_Rooms_HostedVehicles_idx");
			migrationBuilder.RenameIndex(name: "IX_Cells_EnvironmentalMagicProfileId", table: "Rooms", newName: "IX_Rooms_EnvironmentalMagicProfileId");
			migrationBuilder.RenameIndex(name: "IX_Cells_UniqueName", table: "Rooms", newName: "IX_Rooms_UniqueName");
			migrationBuilder.RenameIndex(name: "IX_Cells_ZoneId", table: "Rooms", newName: "IX_Rooms_ZoneId");
			migrationBuilder.RenameIndex(name: "UX_Cells_HostedVehicleCompartments", table: "Rooms", newName: "UX_Rooms_HostedVehicleCompartments");
			migrationBuilder.RenameIndex(name: "FK_Cells_GameItems_GameItems", table: "Rooms_GameItems", newName: "FK_Rooms_GameItems_GameItems");
			migrationBuilder.RenameIndex(name: "FK_Cells_MagicResources_MagicResources_idx", table: "Rooms_MagicResources", newName: "FK_Rooms_MagicResources_MagicResources_idx");
			migrationBuilder.RenameIndex(name: "FK_Cells_RangedCovers_RangedCovers_idx", table: "Rooms_RangedCovers", newName: "FK_Rooms_RangedCovers_RangedCovers_idx");
			migrationBuilder.RenameIndex(name: "FK_Cells_Tags_Tags_idx", table: "Rooms_Tags", newName: "FK_Rooms_Tags_Tags_idx");
			migrationBuilder.RenameIndex(name: "FK_CharacterInstances_Cells_idx", table: "CharacterInstances", newName: "FK_CharacterInstances_Rooms_idx");
			migrationBuilder.RenameIndex(name: "FK_CharacterLog_Cells_idx", table: "CharacterLog", newName: "FK_CharacterLog_Rooms_idx");
			migrationBuilder.RenameIndex(name: "FK_Characters_Cells", table: "Characters", newName: "FK_Characters_Rooms");
			migrationBuilder.RenameIndex(name: "FK_Clans_AdministrationCells_Cells_idx", table: "Clans_AdministrationRooms", newName: "FK_Clans_AdministrationRooms_Rooms_idx");
			migrationBuilder.RenameIndex(name: "FK_Clans_HallCells_Cells_idx", table: "Clans_HallRooms", newName: "FK_Clans_HallRooms_Rooms_idx");
			migrationBuilder.RenameIndex(name: "FK_Clans_TreasuryCells_Cells_idx", table: "Clans_TreasuryRooms", newName: "FK_Clans_TreasuryRooms_Rooms_idx");
			migrationBuilder.RenameIndex(name: "IX_ConveyancingLocations_CellId", table: "ConveyancingLocations", newName: "IX_ConveyancingLocations_RoomId");
			migrationBuilder.RenameIndex(name: "IX_CorpseRecoveryReports_DestinationCellId", table: "CorpseRecoveryReports", newName: "IX_CorpseRecoveryReports_DestinationRoomId");
			migrationBuilder.RenameIndex(name: "IX_CorpseRecoveryReports_SourceCellId", table: "CorpseRecoveryReports", newName: "IX_CorpseRecoveryReports_SourceRoomId");
			migrationBuilder.RenameIndex(name: "IX_EnvironmentalMagicOperations_CellId_AtUtc", table: "EnvironmentalMagicOperations", newName: "IX_EnvironmentalMagicOperations_RoomId_AtUtc");
			migrationBuilder.RenameIndex(name: "FK_Hooks_Perceivables_Cells_idx", table: "Hooks_Perceivables", newName: "FK_Hooks_Perceivables_Rooms_idx");
			migrationBuilder.RenameIndex(name: "FK_HospitalLocations_Cells_idx", table: "HospitalLocations", newName: "FK_HospitalLocations_Rooms_idx");
			migrationBuilder.RenameIndex(name: "FK_HospitalServiceRequests_Cells_Recovery_idx", table: "HospitalServiceRequests", newName: "FK_HospitalServiceRequests_Rooms_Recovery_idx");
			migrationBuilder.RenameIndex(name: "FK_HospitalServiceRequests_Cells_Return_idx", table: "HospitalServiceRequests", newName: "FK_HospitalServiceRequests_Rooms_Return_idx");
			migrationBuilder.RenameIndex(name: "FK_HospitalServiceRequests_Cells_Theatre_idx", table: "HospitalServiceRequests", newName: "FK_HospitalServiceRequests_Rooms_Theatre_idx");
			migrationBuilder.RenameIndex(name: "FK_HotelRooms_Cells_idx", table: "HotelRooms", newName: "FK_HotelRooms_Rooms_idx");
			migrationBuilder.RenameIndex(name: "IX_HotelRooms_Hotel_Cell", table: "HotelRooms", newName: "IX_HotelRooms_Hotel_Room");
			migrationBuilder.RenameIndex(name: "IX_JobFindingLocations_CellId", table: "JobFindingLocations", newName: "IX_JobFindingLocations_RoomId");
			migrationBuilder.RenameIndex(name: "IX_LandRejuvenationTreatments_CellId_Status", table: "LandRejuvenationTreatments", newName: "IX_LandRejuvenationTreatments_RoomId_Status");
			migrationBuilder.RenameIndex(name: "FK_LegalAuthorities_MarshallingCells_idx", table: "LegalAuthorities", newName: "FK_LegalAuthorities_MarshallingRooms_idx");
			migrationBuilder.RenameIndex(name: "FK_LegalAuthorities_PreparingCells_idx", table: "LegalAuthorities", newName: "FK_LegalAuthorities_PreparingRooms_idx");
			migrationBuilder.RenameIndex(name: "FK_LegalAuthorities_PrisonBelongingsCells_idx", table: "LegalAuthorities", newName: "FK_LegalAuthorities_PrisonBelongingsRooms_idx");
			migrationBuilder.RenameIndex(name: "FK_LegalAuthorities_PrisonCells_idx", table: "LegalAuthorities", newName: "FK_LegalAuthorities_PrisonRooms_idx");
			migrationBuilder.RenameIndex(name: "FK_LegalAuthorities_PrisonReleaseCells_idx", table: "LegalAuthorities", newName: "FK_LegalAuthorities_PrisonReleaseRooms_idx");
			migrationBuilder.RenameIndex(name: "FK_LegalAuthorities_StowingCells_idx", table: "LegalAuthorities", newName: "FK_LegalAuthorities_StowingRooms_idx");
			migrationBuilder.RenameIndex(name: "FK_LegalAuthoritiesCells_Cells_idx", table: "LegalAuthoritiyRooms", newName: "FK_LegalAuthoritiesRooms_Rooms_idx");
			migrationBuilder.RenameIndex(name: "FK_LegalAuthoritiesCells_LegalAuthorities_idx", table: "LegalAuthoritiyRooms", newName: "FK_LegalAuthoritiesRooms_LegalAuthorities_idx");
			migrationBuilder.RenameIndex(name: "FK_LegalAuthoritiesCells_Cells_Jail_idx", table: "LegalAuthorityJailRooms", newName: "FK_LegalAuthoritiesRooms_Rooms_Jail_idx");
			migrationBuilder.RenameIndex(name: "FK_LegalAuthoritiesCells_LegalAuthorities_Jail_idx", table: "LegalAuthorityJailRooms", newName: "FK_LegalAuthoritiesRooms_LegalAuthorities_Jail_idx");
			migrationBuilder.RenameIndex(name: "IX_MagicGatheringOperations_Cell_Source_Status", table: "MagicGatheringOperations", newName: "IX_MagicGatheringOperations_Room_Source_Status");
			migrationBuilder.RenameIndex(name: "IX_MagicGatheringParticipants_Cell_Source", table: "MagicGatheringParticipants", newName: "IX_MagicGatheringParticipants_Room_Source");
			migrationBuilder.RenameIndex(name: "FK_MagicPortalEndpoints_Cells_idx", table: "MagicPortalEndpoints", newName: "FK_MagicPortalEndpoints_Rooms_idx");
			migrationBuilder.RenameIndex(name: "IX_NPCSpawnerCells_CellId", table: "NPCSpawnerRooms", newName: "IX_NPCSpawnerRooms_RoomId");
			migrationBuilder.RenameIndex(name: "FK_PatrolRoutesNodes_Cells_idx", table: "PatrolRoutesNodes", newName: "FK_PatrolRoutesNodes_Rooms_idx");
			migrationBuilder.RenameIndex(name: "IX_ProbateLocations_CellId", table: "ProbateLocations", newName: "IX_ProbateLocations_RoomId");
			migrationBuilder.RenameIndex(name: "IX_PropertyLocations_CellId", table: "PropertyLocations", newName: "IX_PropertyLocations_RoomId");
			migrationBuilder.RenameIndex(name: "IX_RestaurantCells_Cell", table: "RestaurantRooms", newName: "IX_RestaurantRooms_Room");
			migrationBuilder.RenameIndex(name: "IX_RestaurantCells_Restaurant_Role", table: "RestaurantRooms", newName: "IX_RestaurantRooms_Restaurant_Role");
			migrationBuilder.RenameIndex(name: "IX_RouteCellLandmarks_RouteCell_Position", table: "RouteRoomLandmarks", newName: "IX_RouteRoomLandmarks_RouteRoom_Position");
			migrationBuilder.RenameIndex(name: "IX_RouteExitAnchors_RouteCell_Band", table: "RouteExitAnchors", newName: "IX_RouteExitAnchors_RouteRoom_Band");
			migrationBuilder.RenameIndex(name: "FK_Shops_Cells_Stockroom_idx", table: "Shops", newName: "FK_Shops_Rooms_Stockroom_idx");
			migrationBuilder.RenameIndex(name: "FK_Shops_Cells_Workshop_idx", table: "Shops", newName: "FK_Shops_Rooms_Workshop_idx");
			migrationBuilder.RenameIndex(name: "FK_Shops_StoreroomCells_Cells_idx", table: "Shops_StoreroomRooms", newName: "FK_Shops_StoreroomRooms_Rooms_idx");
			migrationBuilder.RenameIndex(name: "FK_Stables_Cells_idx", table: "Stables", newName: "FK_Stables_Rooms_idx");
			migrationBuilder.RenameIndex(name: "IX_Tracks_Cell_Layer_RoutePosition", table: "Tracks", newName: "IX_Tracks_Room_Layer_RoutePosition");
			migrationBuilder.RenameIndex(name: "UX_VehicleCompartments_InteriorCell", table: "VehicleCompartments", newName: "UX_VehicleCompartments_InteriorRoom");
			migrationBuilder.RenameIndex(name: "IX_VehicleDockings_ExteriorCell_Layer", table: "VehicleDockings", newName: "IX_VehicleDockings_ExteriorRoom_Layer");
			migrationBuilder.RenameIndex(name: "FK_VehicleRoutePlatformBindings_Cells_idx", table: "VehicleRoutePlatformBindings", newName: "FK_VehicleRoutePlatformBindings_Rooms_idx");
			migrationBuilder.RenameIndex(name: "FK_VehicleRouteSteps_DestinationCells_idx", table: "VehicleRouteSteps", newName: "FK_VehicleRouteSteps_DestinationRooms_idx");
			migrationBuilder.RenameIndex(name: "FK_VehicleRouteSteps_OriginCells_idx", table: "VehicleRouteSteps", newName: "FK_VehicleRouteSteps_OriginRooms_idx");
			migrationBuilder.RenameIndex(name: "FK_VehicleRouteStops_Cells_idx", table: "VehicleRouteStops", newName: "FK_VehicleRouteStops_Rooms_idx");
			migrationBuilder.RenameIndex(name: "FK_VehicleRouteTopologyPins_RouteCells_idx", table: "VehicleRouteTopologyPins", newName: "FK_VehicleRouteTopologyPins_RouteRooms_idx");
			migrationBuilder.RenameIndex(name: "FK_Vehicles_Cells_Current_idx", table: "Vehicles", newName: "FK_Vehicles_Rooms_Current_idx");
			migrationBuilder.RenameIndex(name: "FK_Vehicles_Cells_Destination_idx", table: "Vehicles", newName: "FK_Vehicles_Rooms_Destination_idx");
			migrationBuilder.RenameIndex(name: "IX_Vehicles_Cell_Layer_RoutePosition", table: "Vehicles", newName: "IX_Vehicles_Room_Layer_RoutePosition");
			migrationBuilder.RenameIndex(name: "FK_Zones_Cells", table: "Zones", newName: "FK_Zones_Rooms");

			AddForeignKeyWithPreservedActions(migrationBuilder, "FK_ActiveProjects_Rooms", "ALTER TABLE `ActiveProjects` ADD CONSTRAINT `FK_ActiveProjects_Rooms` FOREIGN KEY (`RoomId`) REFERENCES `Rooms` (`Id`)");
			migrationBuilder.AddForeignKey(name: "FK_ActiveRouteMotions_RouteRooms", table: "ActiveRouteMotions", columns: new[] { "RouteRoomId" }, principalTable: "RouteRooms", principalColumns: new[] { "RoomId" }, onDelete: ReferentialAction.Cascade);
			migrationBuilder.AddForeignKey(name: "FK_AgricultureFields_Rooms", table: "AgricultureFields", columns: new[] { "RoomId" }, principalTable: "Rooms", principalColumns: new[] { "Id" }, onDelete: ReferentialAction.Cascade);
			migrationBuilder.AddForeignKey(name: "FK_AIStorytellerSituations_Rooms_ScopeRoomId", table: "AIStorytellerSituations", columns: new[] { "ScopeRoomId" }, principalTable: "Rooms", principalColumns: new[] { "Id" }, onDelete: ReferentialAction.NoAction);
			migrationBuilder.AddForeignKey(name: "FK_Areas_Rooms_Areas", table: "Areas_Rooms", columns: new[] { "AreaId" }, principalTable: "Areas", principalColumns: new[] { "Id" }, onDelete: ReferentialAction.Cascade);
			migrationBuilder.AddForeignKey(name: "FK_Areas_Rooms_Rooms", table: "Areas_Rooms", columns: new[] { "RoomId" }, principalTable: "Rooms", principalColumns: new[] { "Id" }, onDelete: ReferentialAction.Cascade);
			migrationBuilder.AddForeignKey(name: "FK_ArenaRooms_Arenas", table: "ArenaRooms", columns: new[] { "ArenaId" }, principalTable: "Arenas", principalColumns: new[] { "Id" }, onDelete: ReferentialAction.Cascade);
			migrationBuilder.AddForeignKey(name: "FK_ArenaRooms_Rooms", table: "ArenaRooms", columns: new[] { "RoomId" }, principalTable: "Rooms", principalColumns: new[] { "Id" }, onDelete: ReferentialAction.Cascade);
			migrationBuilder.AddForeignKey(name: "FK_AuctionHouses_Rooms", table: "AuctionHouses", columns: new[] { "AuctionHouseRoomId" }, principalTable: "Rooms", principalColumns: new[] { "Id" }, onDelete: ReferentialAction.Cascade);
			migrationBuilder.AddForeignKey(name: "FK_BankBranches_Rooms", table: "BankBranches", columns: new[] { "RoomId" }, principalTable: "Rooms", principalColumns: new[] { "Id" }, onDelete: ReferentialAction.Cascade);
			migrationBuilder.AddForeignKey(name: "FK_RoomEnvironmentalStates_Rooms", table: "RoomEnvironmentalStates", columns: new[] { "RoomId" }, principalTable: "Rooms", principalColumns: new[] { "Id" }, onDelete: ReferentialAction.Cascade);
			migrationBuilder.AddForeignKey(name: "FK_RoomOverlayPackages_EditableItems", table: "RoomOverlayPackages", columns: new[] { "EditableItemId" }, principalTable: "EditableItems", principalColumns: new[] { "Id" }, onDelete: ReferentialAction.NoAction);
			migrationBuilder.AddForeignKey(name: "FK_RoomOverlays_RoomOverlayPackages", table: "RoomOverlays", columns: new[] { "RoomOverlayPackageId", "RoomOverlayPackageRevisionNumber" }, principalTable: "RoomOverlayPackages", principalColumns: new[] { "Id", "RevisionNumber" }, onDelete: ReferentialAction.Cascade);
			migrationBuilder.AddForeignKey(name: "FK_RoomOverlays_Rooms", table: "RoomOverlays", columns: new[] { "RoomId" }, principalTable: "Rooms", principalColumns: new[] { "Id" }, onDelete: ReferentialAction.Cascade);
			migrationBuilder.AddForeignKey(name: "FK_RoomOverlays_HearingProfiles", table: "RoomOverlays", columns: new[] { "HearingProfileId" }, principalTable: "HearingProfiles", principalColumns: new[] { "Id" }, onDelete: ReferentialAction.SetNull);
			migrationBuilder.AddForeignKey(name: "FK_RoomOverlays_Terrains", table: "RoomOverlays", columns: new[] { "TerrainId" }, principalTable: "Terrains", principalColumns: new[] { "Id" }, onDelete: ReferentialAction.Restrict);
			migrationBuilder.AddForeignKey(name: "FK_RoomOverlays_Exits_RoomOverlays", table: "RoomOverlays_Exits", columns: new[] { "RoomOverlayId" }, principalTable: "RoomOverlays", principalColumns: new[] { "Id" }, onDelete: ReferentialAction.Cascade);
			migrationBuilder.AddForeignKey(name: "FK_RoomOverlays_Exits_Exits", table: "RoomOverlays_Exits", columns: new[] { "ExitId" }, principalTable: "Exits", principalColumns: new[] { "Id" }, onDelete: ReferentialAction.Cascade);
			migrationBuilder.AddForeignKey(name: "FK_Rooms_RoomOverlays", table: "Rooms", columns: new[] { "CurrentOverlayId" }, principalTable: "RoomOverlays", principalColumns: new[] { "Id" }, onDelete: ReferentialAction.SetNull);
			migrationBuilder.AddForeignKey(name: "FK_Rooms_HostedVehicleCompartments", table: "Rooms", columns: new[] { "HostedVehicleCompartmentId" }, principalTable: "VehicleCompartments", principalColumns: new[] { "Id" }, onDelete: ReferentialAction.Restrict);
			migrationBuilder.AddForeignKey(name: "FK_Rooms_HostedVehicles", table: "Rooms", columns: new[] { "HostedVehicleId" }, principalTable: "Vehicles", principalColumns: new[] { "Id" }, onDelete: ReferentialAction.Restrict);
			migrationBuilder.AddForeignKey(name: "FK_Rooms_OwningZone", table: "Rooms", columns: new[] { "ZoneId" }, principalTable: "Zones", principalColumns: new[] { "Id" }, onDelete: ReferentialAction.Restrict);

			AddForeignKeyWithPreservedActions(migrationBuilder, "FK_Rooms_ForagableYields_Rooms", "ALTER TABLE `Rooms_ForagableYields` ADD CONSTRAINT `FK_Rooms_ForagableYields_Rooms` FOREIGN KEY (`RoomId`) REFERENCES `Rooms` (`Id`)");
			migrationBuilder.AddForeignKey(name: "FK_Rooms_GameItems_Rooms", table: "Rooms_GameItems", columns: new[] { "RoomId" }, principalTable: "Rooms", principalColumns: new[] { "Id" }, onDelete: ReferentialAction.Cascade);
			migrationBuilder.AddForeignKey(name: "FK_Rooms_GameItems_GameItems", table: "Rooms_GameItems", columns: new[] { "GameItemId" }, principalTable: "GameItems", principalColumns: new[] { "Id" }, onDelete: ReferentialAction.Cascade);

			AddForeignKeyWithPreservedActions(migrationBuilder, "FK_Rooms_MagicResources_Rooms", "ALTER TABLE `Rooms_MagicResources` ADD CONSTRAINT `FK_Rooms_MagicResources_Rooms` FOREIGN KEY (`RoomId`) REFERENCES `Rooms` (`Id`)");

			AddForeignKeyWithPreservedActions(migrationBuilder, "FK_Rooms_MagicResources_MagicResources", "ALTER TABLE `Rooms_MagicResources` ADD CONSTRAINT `FK_Rooms_MagicResources_MagicResources` FOREIGN KEY (`MagicResourceId`) REFERENCES `MagicResources` (`Id`)");

			AddForeignKeyWithPreservedActions(migrationBuilder, "FK_Rooms_RangedCovers_Rooms", "ALTER TABLE `Rooms_RangedCovers` ADD CONSTRAINT `FK_Rooms_RangedCovers_Rooms` FOREIGN KEY (`RoomId`) REFERENCES `Rooms` (`Id`)");

			AddForeignKeyWithPreservedActions(migrationBuilder, "FK_Rooms_RangedCovers_RangedCovers", "ALTER TABLE `Rooms_RangedCovers` ADD CONSTRAINT `FK_Rooms_RangedCovers_RangedCovers` FOREIGN KEY (`RangedCoverId`) REFERENCES `RangedCovers` (`Id`)");
			migrationBuilder.AddForeignKey(name: "FK_Rooms_Tags_Rooms", table: "Rooms_Tags", columns: new[] { "RoomId" }, principalTable: "Rooms", principalColumns: new[] { "Id" }, onDelete: ReferentialAction.Cascade);

			AddForeignKeyWithPreservedActions(migrationBuilder, "FK_Rooms_Tags_Tags", "ALTER TABLE `Rooms_Tags` ADD CONSTRAINT `FK_Rooms_Tags_Tags` FOREIGN KEY (`TagId`) REFERENCES `Tags` (`Id`)");
			ChangeOptionalCharacterInstanceForeignKey(migrationBuilder, restore: true);

			AddForeignKeyWithPreservedActions(migrationBuilder, "FK_CharacterLog_Rooms", "ALTER TABLE `CharacterLog` ADD CONSTRAINT `FK_CharacterLog_Rooms` FOREIGN KEY (`RoomId`) REFERENCES `Rooms` (`Id`)");
			migrationBuilder.AddForeignKey(name: "FK_Characters_Rooms", table: "Characters", columns: new[] { "Location" }, principalTable: "Rooms", principalColumns: new[] { "Id" }, onDelete: ReferentialAction.NoAction);

			AddForeignKeyWithPreservedActions(migrationBuilder, "FK_Clans_AdministrationRooms_Rooms", "ALTER TABLE `Clans_AdministrationRooms` ADD CONSTRAINT `FK_Clans_AdministrationRooms_Rooms` FOREIGN KEY (`RoomId`) REFERENCES `Rooms` (`Id`)");

			AddForeignKeyWithPreservedActions(migrationBuilder, "FK_Clans_AdministrationRooms_Clans", "ALTER TABLE `Clans_AdministrationRooms` ADD CONSTRAINT `FK_Clans_AdministrationRooms_Clans` FOREIGN KEY (`ClanId`) REFERENCES `Clans` (`Id`)");
			migrationBuilder.AddForeignKey(name: "FK_Clans_HallRooms_Rooms", table: "Clans_HallRooms", columns: new[] { "RoomId" }, principalTable: "Rooms", principalColumns: new[] { "Id" }, onDelete: ReferentialAction.Cascade);
			migrationBuilder.AddForeignKey(name: "FK_Clans_HallRooms_Clans", table: "Clans_HallRooms", columns: new[] { "ClanId" }, principalTable: "Clans", principalColumns: new[] { "Id" }, onDelete: ReferentialAction.Cascade);

			AddForeignKeyWithPreservedActions(migrationBuilder, "FK_Clans_TreasuryRooms_Rooms", "ALTER TABLE `Clans_TreasuryRooms` ADD CONSTRAINT `FK_Clans_TreasuryRooms_Rooms` FOREIGN KEY (`RoomId`) REFERENCES `Rooms` (`Id`)");

			AddForeignKeyWithPreservedActions(migrationBuilder, "FK_Clans_TreasuryRooms_Clans", "ALTER TABLE `Clans_TreasuryRooms` ADD CONSTRAINT `FK_Clans_TreasuryRooms_Clans` FOREIGN KEY (`ClanId`) REFERENCES `Clans` (`Id`)");
			migrationBuilder.AddForeignKey(name: "FK_ConveyancingLocations_Rooms", table: "ConveyancingLocations", columns: new[] { "RoomId" }, principalTable: "Rooms", principalColumns: new[] { "Id" }, onDelete: ReferentialAction.Cascade);
			migrationBuilder.AddForeignKey(name: "FK_CorpseRecoveryReports_DestinationRooms", table: "CorpseRecoveryReports", columns: new[] { "DestinationRoomId" }, principalTable: "Rooms", principalColumns: new[] { "Id" }, onDelete: ReferentialAction.Cascade);
			migrationBuilder.AddForeignKey(name: "FK_CorpseRecoveryReports_SourceRooms", table: "CorpseRecoveryReports", columns: new[] { "SourceRoomId" }, principalTable: "Rooms", principalColumns: new[] { "Id" }, onDelete: ReferentialAction.Cascade);

			AddForeignKeyWithPreservedActions(migrationBuilder, "FK_Crimes_Location", "ALTER TABLE `Crimes` ADD CONSTRAINT `FK_Crimes_Location` FOREIGN KEY (`LocationId`) REFERENCES `Rooms` (`Id`)");
			migrationBuilder.AddForeignKey(name: "FK_EconomicZones_MorgueOfficeLocations", table: "EconomicZones", columns: new[] { "MorgueOfficeLocationId" }, principalTable: "Rooms", principalColumns: new[] { "Id" }, onDelete: ReferentialAction.SetNull);
			migrationBuilder.AddForeignKey(name: "FK_EconomicZones_MorgueStorageLocations", table: "EconomicZones", columns: new[] { "MorgueStorageLocationId" }, principalTable: "Rooms", principalColumns: new[] { "Id" }, onDelete: ReferentialAction.SetNull);

			AddForeignKeyWithPreservedActions(migrationBuilder, "FK_Hooks_Perceivables_Rooms", "ALTER TABLE `Hooks_Perceivables` ADD CONSTRAINT `FK_Hooks_Perceivables_Rooms` FOREIGN KEY (`RoomId`) REFERENCES `Rooms` (`Id`)");
			migrationBuilder.AddForeignKey(name: "FK_HospitalLocations_Rooms", table: "HospitalLocations", columns: new[] { "RoomId" }, principalTable: "Rooms", principalColumns: new[] { "Id" }, onDelete: ReferentialAction.Cascade);
			migrationBuilder.AddForeignKey(name: "FK_HospitalServiceRequests_Rooms_Recovery", table: "HospitalServiceRequests", columns: new[] { "RecoveryRoomId" }, principalTable: "Rooms", principalColumns: new[] { "Id" }, onDelete: ReferentialAction.SetNull);
			migrationBuilder.AddForeignKey(name: "FK_HospitalServiceRequests_Rooms_Return", table: "HospitalServiceRequests", columns: new[] { "ReturnRoomId" }, principalTable: "Rooms", principalColumns: new[] { "Id" }, onDelete: ReferentialAction.SetNull);
			migrationBuilder.AddForeignKey(name: "FK_HospitalServiceRequests_Rooms_Theatre", table: "HospitalServiceRequests", columns: new[] { "OperatingTheatreRoomId" }, principalTable: "Rooms", principalColumns: new[] { "Id" }, onDelete: ReferentialAction.SetNull);
			migrationBuilder.AddForeignKey(name: "FK_HotelRooms_Rooms", table: "HotelRooms", columns: new[] { "RoomId" }, principalTable: "Rooms", principalColumns: new[] { "Id" }, onDelete: ReferentialAction.Restrict);
			migrationBuilder.AddForeignKey(name: "FK_JobFindingLocations_Rooms", table: "JobFindingLocations", columns: new[] { "RoomId" }, principalTable: "Rooms", principalColumns: new[] { "Id" }, onDelete: ReferentialAction.Cascade);
			migrationBuilder.AddForeignKey(name: "FK_LegalAuthorities_CourtroomRoom", table: "LegalAuthorities", columns: new[] { "CourtLocationId" }, principalTable: "Rooms", principalColumns: new[] { "Id" }, onDelete: ReferentialAction.NoAction);
			migrationBuilder.AddForeignKey(name: "FK_LegalAuthorities_MarshallingRooms", table: "LegalAuthorities", columns: new[] { "MarshallingLocationId" }, principalTable: "Rooms", principalColumns: new[] { "Id" }, onDelete: ReferentialAction.NoAction);
			migrationBuilder.AddForeignKey(name: "FK_LegalAuthorities_PreparingRooms", table: "LegalAuthorities", columns: new[] { "PreparingLocationId" }, principalTable: "Rooms", principalColumns: new[] { "Id" }, onDelete: ReferentialAction.NoAction);
			migrationBuilder.AddForeignKey(name: "FK_LegalAuthorities_PrisonBelongingsRooms", table: "LegalAuthorities", columns: new[] { "PrisonBelongingsLocationId" }, principalTable: "Rooms", principalColumns: new[] { "Id" }, onDelete: ReferentialAction.NoAction);
			migrationBuilder.AddForeignKey(name: "FK_LegalAuthorities_PrisonRooms", table: "LegalAuthorities", columns: new[] { "PrisonLocationId" }, principalTable: "Rooms", principalColumns: new[] { "Id" }, onDelete: ReferentialAction.NoAction);
			migrationBuilder.AddForeignKey(name: "FK_LegalAuthorities_PrisonJailRooms", table: "LegalAuthorities", columns: new[] { "JailLocationId" }, principalTable: "Rooms", principalColumns: new[] { "Id" }, onDelete: ReferentialAction.NoAction);
			migrationBuilder.AddForeignKey(name: "FK_LegalAuthorities_PrisonReleaseRooms", table: "LegalAuthorities", columns: new[] { "PrisonReleaseLocationId" }, principalTable: "Rooms", principalColumns: new[] { "Id" }, onDelete: ReferentialAction.NoAction);
			migrationBuilder.AddForeignKey(name: "FK_LegalAuthorities_StowingRooms", table: "LegalAuthorities", columns: new[] { "EnforcerStowingLocationId" }, principalTable: "Rooms", principalColumns: new[] { "Id" }, onDelete: ReferentialAction.NoAction);
			migrationBuilder.AddForeignKey(name: "FK_LegalAuthoritiesRooms_Rooms", table: "LegalAuthoritiyRooms", columns: new[] { "RoomId" }, principalTable: "Rooms", principalColumns: new[] { "Id" }, onDelete: ReferentialAction.Cascade);
			migrationBuilder.AddForeignKey(name: "FK_LegalAuthoritiesRooms_LegalAuthorities", table: "LegalAuthoritiyRooms", columns: new[] { "LegalAuthorityId" }, principalTable: "LegalAuthorities", principalColumns: new[] { "Id" }, onDelete: ReferentialAction.Cascade);
			migrationBuilder.AddForeignKey(name: "FK_LegalAuthoritiesRooms_Rooms_Jail", table: "LegalAuthorityJailRooms", columns: new[] { "RoomId" }, principalTable: "Rooms", principalColumns: new[] { "Id" }, onDelete: ReferentialAction.Cascade);
			migrationBuilder.AddForeignKey(name: "FK_LegalAuthoritiesRooms_LegalAuthorities_Jail", table: "LegalAuthorityJailRooms", columns: new[] { "LegalAuthorityId" }, principalTable: "LegalAuthorities", principalColumns: new[] { "Id" }, onDelete: ReferentialAction.Cascade);
			migrationBuilder.AddForeignKey(name: "FK_MagicPortalEndpoints_Rooms", table: "MagicPortalEndpoints", columns: new[] { "RoomId" }, principalTable: "Rooms", principalColumns: new[] { "Id" }, onDelete: ReferentialAction.SetNull);
			migrationBuilder.AddForeignKey(name: "FK_NPCSpawnerRooms_Room", table: "NPCSpawnerRooms", columns: new[] { "RoomId" }, principalTable: "Rooms", principalColumns: new[] { "Id" }, onDelete: ReferentialAction.Cascade);
			migrationBuilder.AddForeignKey(name: "FK_NPCSpawnerRooms_NPCSpawner", table: "NPCSpawnerRooms", columns: new[] { "NPCSpawnerId" }, principalTable: "NPCSpawners", principalColumns: new[] { "Id" }, onDelete: ReferentialAction.Cascade);
			migrationBuilder.AddForeignKey(name: "FK_PatrolRoutesNodes_Rooms", table: "PatrolRoutesNodes", columns: new[] { "RoomId" }, principalTable: "Rooms", principalColumns: new[] { "Id" }, onDelete: ReferentialAction.Cascade);
			migrationBuilder.AddForeignKey(name: "FK_Patrols_LastMajorNode", table: "Patrols", columns: new[] { "LastMajorNodeId" }, principalTable: "Rooms", principalColumns: new[] { "Id" }, onDelete: ReferentialAction.NoAction);
			migrationBuilder.AddForeignKey(name: "FK_Patrols_NextMajorNode", table: "Patrols", columns: new[] { "NextMajorNodeId" }, principalTable: "Rooms", principalColumns: new[] { "Id" }, onDelete: ReferentialAction.NoAction);
			migrationBuilder.AddForeignKey(name: "FK_ProbateLocations_Rooms", table: "ProbateLocations", columns: new[] { "RoomId" }, principalTable: "Rooms", principalColumns: new[] { "Id" }, onDelete: ReferentialAction.Cascade);
			migrationBuilder.AddForeignKey(name: "FK_PropertyLocations_Room", table: "PropertyLocations", columns: new[] { "RoomId" }, principalTable: "Rooms", principalColumns: new[] { "Id" }, onDelete: ReferentialAction.Cascade);
			migrationBuilder.AddForeignKey(name: "FK_RestaurantRooms_Restaurants", table: "RestaurantRooms", columns: new[] { "RestaurantShopId" }, principalTable: "Restaurants", principalColumns: new[] { "ShopId" }, onDelete: ReferentialAction.Cascade);
			migrationBuilder.AddForeignKey(name: "FK_RouteRoomLandmarks_RouteRooms", table: "RouteRoomLandmarks", columns: new[] { "RouteRoomId" }, principalTable: "RouteRooms", principalColumns: new[] { "RoomId" }, onDelete: ReferentialAction.Cascade);
			migrationBuilder.AddForeignKey(name: "FK_RouteRooms_Rooms", table: "RouteRooms", columns: new[] { "RoomId" }, principalTable: "Rooms", principalColumns: new[] { "Id" }, onDelete: ReferentialAction.Cascade);
			migrationBuilder.AddForeignKey(name: "FK_RouteExitAnchors_RouteRooms", table: "RouteExitAnchors", columns: new[] { "RouteRoomId" }, principalTable: "RouteRooms", principalColumns: new[] { "RoomId" }, onDelete: ReferentialAction.Cascade);

			AddForeignKeyWithPreservedActions(migrationBuilder, "FK_Shops_Rooms_Stockroom", "ALTER TABLE `Shops` ADD CONSTRAINT `FK_Shops_Rooms_Stockroom` FOREIGN KEY (`StockroomId`) REFERENCES `Rooms` (`Id`)");

			AddForeignKeyWithPreservedActions(migrationBuilder, "FK_Shops_Rooms_Workshop", "ALTER TABLE `Shops` ADD CONSTRAINT `FK_Shops_Rooms_Workshop` FOREIGN KEY (`WorkshopRoomId`) REFERENCES `Rooms` (`Id`)");

			AddForeignKeyWithPreservedActions(migrationBuilder, "FK_Shops_StoreroomRooms_Rooms", "ALTER TABLE `Shops_StoreroomRooms` ADD CONSTRAINT `FK_Shops_StoreroomRooms_Rooms` FOREIGN KEY (`RoomId`) REFERENCES `Rooms` (`Id`)");

			AddForeignKeyWithPreservedActions(migrationBuilder, "FK_Shops_StoreroomRooms_Shops", "ALTER TABLE `Shops_StoreroomRooms` ADD CONSTRAINT `FK_Shops_StoreroomRooms_Shops` FOREIGN KEY (`ShopId`) REFERENCES `Shops` (`Id`)");
			migrationBuilder.AddForeignKey(name: "FK_Stables_Rooms", table: "Stables", columns: new[] { "RoomId" }, principalTable: "Rooms", principalColumns: new[] { "Id" }, onDelete: ReferentialAction.Cascade);
			migrationBuilder.AddForeignKey(name: "FK_Tracks_Rooms", table: "Tracks", columns: new[] { "RoomId" }, principalTable: "Rooms", principalColumns: new[] { "Id" }, onDelete: ReferentialAction.Cascade);
			migrationBuilder.AddForeignKey(name: "FK_VehicleCompartments_InteriorRooms", table: "VehicleCompartments", columns: new[] { "InteriorRoomId" }, principalTable: "Rooms", principalColumns: new[] { "Id" }, onDelete: ReferentialAction.Restrict);
			migrationBuilder.AddForeignKey(name: "FK_VehicleDockings_ExteriorRooms", table: "VehicleDockings", columns: new[] { "ExteriorRoomId" }, principalTable: "Rooms", principalColumns: new[] { "Id" }, onDelete: ReferentialAction.Restrict);
			migrationBuilder.AddForeignKey(name: "FK_VehicleRoutePlatformBindings_Rooms", table: "VehicleRoutePlatformBindings", columns: new[] { "PlatformRoomId" }, principalTable: "Rooms", principalColumns: new[] { "Id" }, onDelete: ReferentialAction.Restrict);
			migrationBuilder.AddForeignKey(name: "FK_VehicleRouteSteps_DestinationRooms", table: "VehicleRouteSteps", columns: new[] { "DestinationRoomId" }, principalTable: "Rooms", principalColumns: new[] { "Id" }, onDelete: ReferentialAction.Restrict);
			migrationBuilder.AddForeignKey(name: "FK_VehicleRouteSteps_OriginRooms", table: "VehicleRouteSteps", columns: new[] { "OriginRoomId" }, principalTable: "Rooms", principalColumns: new[] { "Id" }, onDelete: ReferentialAction.Restrict);
			migrationBuilder.AddForeignKey(name: "FK_VehicleRouteStops_Rooms", table: "VehicleRouteStops", columns: new[] { "RoomId" }, principalTable: "Rooms", principalColumns: new[] { "Id" }, onDelete: ReferentialAction.Restrict);
			migrationBuilder.AddForeignKey(name: "FK_VehicleRouteTopologyPins_RouteRooms", table: "VehicleRouteTopologyPins", columns: new[] { "RouteRoomId" }, principalTable: "RouteRooms", principalColumns: new[] { "RoomId" }, onDelete: ReferentialAction.Restrict);
			migrationBuilder.AddForeignKey(name: "FK_Vehicles_Rooms_Current", table: "Vehicles", columns: new[] { "CurrentRoomId" }, principalTable: "Rooms", principalColumns: new[] { "Id" }, onDelete: ReferentialAction.SetNull);
			migrationBuilder.AddForeignKey(name: "FK_Vehicles_Rooms_Destination", table: "Vehicles", columns: new[] { "DestinationRoomId" }, principalTable: "Rooms", principalColumns: new[] { "Id" }, onDelete: ReferentialAction.SetNull);
			migrationBuilder.AddForeignKey(name: "FK_Zones_Rooms", table: "Zones", columns: new[] { "DefaultRoomId" }, principalTable: "Rooms", principalColumns: new[] { "Id" }, onDelete: ReferentialAction.NoAction);

  AddChecksWithPreservedEnforcement(migrationBuilder);

  // Older contracted databases may retain required child columns in these
  // independent ledgers. Allow explicit empty-parent membership dispositions.
  migrationBuilder.AlterColumn<long>(name: "RoomId", table: "RoomSpatialAreaMigrationLedger", type: "bigint(20)", nullable: true, oldClrType: typeof(long), oldType: "bigint(20)");
  migrationBuilder.AlterColumn<long>(name: "RoomId", table: "RoomSpatialAreaContractionLedger", type: "bigint(20)", nullable: true, oldClrType: typeof(long), oldType: "bigint(20)");
  migrationBuilder.Sql("CALL `fm_room_terminology_20261007043900`(TRUE);", suppressTransaction: true);
  migrationBuilder.Sql("DROP PROCEDURE `fm_room_terminology_20261007043900`;", suppressTransaction: true);
 }

	private static void ChangeOptionalCharacterInstanceForeignKey(MigrationBuilder migrationBuilder, bool restore)
	{
		var statement = restore
			? "CONCAT('ALTER TABLE `CharacterInstances` ADD CONSTRAINT `FK_CharacterInstances_Rooms` FOREIGN KEY (`LocationId`) REFERENCES `Rooms` (`Id`) ON DELETE ',DeleteRule,' ON UPDATE ',UpdateRule)"
			: "'ALTER TABLE `CharacterInstances` DROP FOREIGN KEY `FK_CharacterInstances_Cells`'";
		migrationBuilder.Sql($"""
SET @fm_room_naming_optional_statement_20261007043900 = (
 SELECT CASE IsPresent WHEN 0 THEN 'DO 0' WHEN 1 THEN {statement} END
 FROM `fm_room_naming_optional_fk_20261007043900`
 WHERE ConstraintName='FK_CharacterInstances_Rooms'
  AND ((IsPresent=0 AND UpdateRule IS NULL AND DeleteRule IS NULL)
   OR (IsPresent=1 AND UpdateRule IN ('NO ACTION','RESTRICT') AND DeleteRule='SET NULL')));
PREPARE fm_room_naming_optional_statement_20261007043900 FROM @fm_room_naming_optional_statement_20261007043900;
EXECUTE fm_room_naming_optional_statement_20261007043900;
DEALLOCATE PREPARE fm_room_naming_optional_statement_20261007043900;
SET @fm_room_naming_optional_statement_20261007043900 = NULL;
""", suppressTransaction: true);
	}

	/// <summary>
	/// Recreates a reviewed renamed constraint using its validated source actions.
	/// The identifiers come from this migration, never from database content.
	/// </summary>
	private static void AddForeignKeyWithPreservedActions(MigrationBuilder migrationBuilder, string constraintName,
		string statement)
	{
		var nameLiteral = constraintName.Replace("'", "''");
		var statementLiteral = statement.Replace("'", "''");
		migrationBuilder.Sql($"""
SET @fm_room_naming_fk_statement_20261007043900 = (
 SELECT CONCAT('{statementLiteral} ON DELETE ',DeleteRule,' ON UPDATE ',UpdateRule)
 FROM `fm_room_naming_fk_actions_20261007043900`
 WHERE ConstraintName='{nameLiteral}'
  AND UpdateRule IN ('NO ACTION','RESTRICT','CASCADE')
  AND DeleteRule IN ('NO ACTION','RESTRICT','CASCADE','SET NULL'));
PREPARE fm_room_naming_fk_statement_20261007043900 FROM @fm_room_naming_fk_statement_20261007043900;
EXECUTE fm_room_naming_fk_statement_20261007043900;
DEALLOCATE PREPARE fm_room_naming_fk_statement_20261007043900;
SET @fm_room_naming_fk_statement_20261007043900 = NULL;
""", suppressTransaction: true);
	}

	// Reviewed against every preceding Up operation and both relational models.
	// Native signatures come from the maintained MySQL 8 snapshot, without its
	// CHECK grammar wrapper. Predicate parentheses/operators remain significant.
	private sealed record ReviewedCheck(string SourceTable, string SourceName, string TargetTable,
		string TargetName, string SourceSql, string SourceNativeSql, string TargetSql,
		string TargetNativeSql, bool Recreate);

	private static readonly ReviewedCheck[] ReviewedChecks =
	[
		new("ActiveRouteMotions", "CK_ActiveRouteMotions_Checkpoint", "ActiveRouteMotions", "CK_ActiveRouteMotions_Checkpoint", "`CheckpointPositionMetres` >= 0", "(`CheckpointPositionMetres` >= 0)", "`CheckpointPositionMetres` >= 0", "(`CheckpointPositionMetres` >= 0)", false),
		new("ActiveRouteMotions", "CK_ActiveRouteMotions_Direction", "ActiveRouteMotions", "CK_ActiveRouteMotions_Direction", "`Direction` IN (-1, 1)", "(`Direction` in (-(1),1))", "`Direction` IN (-1, 1)", "(`Direction` in (-(1),1))", false),
		new("ActiveRouteMotions", "CK_ActiveRouteMotions_RemainingDuration", "ActiveRouteMotions", "CK_ActiveRouteMotions_RemainingDuration", "`RemainingDurationMilliseconds` >= 0", "(`RemainingDurationMilliseconds` >= 0)", "`RemainingDurationMilliseconds` >= 0", "(`RemainingDurationMilliseconds` >= 0)", false),
		new("ActiveRouteMotions", "CK_ActiveRouteMotions_Sequence", "ActiveRouteMotions", "CK_ActiveRouteMotions_Sequence", "`CheckpointSequence` >= 0", "(`CheckpointSequence` >= 0)", "`CheckpointSequence` >= 0", "(`CheckpointSequence` >= 0)", false),
		new("ActiveRouteMotions", "CK_ActiveRouteMotions_Speed", "ActiveRouteMotions", "CK_ActiveRouteMotions_Speed", "`SpeedMetresPerSecond` > 0", "(`SpeedMetresPerSecond` > 0)", "`SpeedMetresPerSecond` > 0", "(`SpeedMetresPerSecond` > 0)", false),
		new("ActiveRouteMotions", "CK_ActiveRouteMotions_TargetBand", "ActiveRouteMotions", "CK_ActiveRouteMotions_TargetBand", "`TargetMinimumPositionMetres` >= 0 AND `TargetMaximumPositionMetres` >= `TargetMinimumPositionMetres`", "((`TargetMinimumPositionMetres` >= 0) and (`TargetMaximumPositionMetres` >= `TargetMinimumPositionMetres`))", "`TargetMinimumPositionMetres` >= 0 AND `TargetMaximumPositionMetres` >= `TargetMinimumPositionMetres`", "((`TargetMinimumPositionMetres` >= 0) and (`TargetMaximumPositionMetres` >= `TargetMinimumPositionMetres`))", false),
		new("ActiveRouteMotions", "CK_ActiveRouteMotions_TopologyVersion", "ActiveRouteMotions", "CK_ActiveRouteMotions_TopologyVersion", "`TopologyVersion` >= 1", "(`TopologyVersion` >= 1)", "`TopologyVersion` >= 1", "(`TopologyVersion` >= 1)", false),
		new("CellEnvironmentalStates", "CK_CellEnvironmentalStates_Pressure", "RoomEnvironmentalStates", "CK_RoomEnvironmentalStates_Pressure", "`RecentPressure` >= 0 AND `PressureHalfLifeSeconds` > 0", "((`RecentPressure` >= 0) and (`PressureHalfLifeSeconds` > 0))", "`RecentPressure` >= 0 AND `PressureHalfLifeSeconds` > 0", "((`RecentPressure` >= 0) and (`PressureHalfLifeSeconds` > 0))", true),
		new("CellEnvironmentalStates", "CK_CellEnvironmentalStates_ScarDamage", "RoomEnvironmentalStates", "CK_RoomEnvironmentalStates_ScarDamage", "`ScarDamage` >= 0", "(`ScarDamage` >= 0)", "`ScarDamage` >= 0", "(`ScarDamage` >= 0)", true),
		new("CellEnvironmentalStates", "CK_CellEnvironmentalStates_Versions", "RoomEnvironmentalStates", "CK_RoomEnvironmentalStates_Versions", "`SchemaVersion` >= 1 AND `Revision` >= 0", "((`SchemaVersion` >= 1) and (`Revision` >= 0))", "`SchemaVersion` >= 1 AND `Revision` >= 0", "((`SchemaVersion` >= 1) and (`Revision` >= 0))", true),
		new("Cells", "CK_Cells_HostedVehicleOwnership", "Rooms", "CK_Rooms_HostedVehicleOwnership", "(`HostedVehicleId` IS NULL AND `HostedVehicleCompartmentId` IS NULL) OR (`HostedVehicleId` IS NOT NULL AND `HostedVehicleCompartmentId` IS NOT NULL)", "(((`HostedVehicleId` is null) and (`HostedVehicleCompartmentId` is null)) or ((`HostedVehicleId` is not null) and (`HostedVehicleCompartmentId` is not null)))", "(`HostedVehicleId` IS NULL AND `HostedVehicleCompartmentId` IS NULL) OR (`HostedVehicleId` IS NOT NULL AND `HostedVehicleCompartmentId` IS NOT NULL)", "(((`HostedVehicleId` is null) and (`HostedVehicleCompartmentId` is null)) or ((`HostedVehicleId` is not null) and (`HostedVehicleCompartmentId` is not null)))", true),
		new("RouteCellLandmarks", "CK_RouteCellLandmarks_Position", "RouteRoomLandmarks", "CK_RouteRoomLandmarks_Position", "`PositionMetres` >= 0", "(`PositionMetres` >= 0)", "`PositionMetres` >= 0", "(`PositionMetres` >= 0)", true),
		new("RouteCells", "CK_RouteCells_DefaultPosition", "RouteRooms", "CK_RouteRooms_DefaultPosition", "`DefaultPositionMetres` >= 0 AND `DefaultPositionMetres` <= `LengthMetres`", "((`DefaultPositionMetres` >= 0) and (`DefaultPositionMetres` <= `LengthMetres`))", "`DefaultPositionMetres` >= 0 AND `DefaultPositionMetres` <= `LengthMetres`", "((`DefaultPositionMetres` >= 0) and (`DefaultPositionMetres` <= `LengthMetres`))", true),
		new("RouteCells", "CK_RouteCells_Length", "RouteRooms", "CK_RouteRooms_Length", "`LengthMetres` > 0", "(`LengthMetres` > 0)", "`LengthMetres` > 0", "(`LengthMetres` > 0)", true),
		new("RouteCells", "CK_RouteCells_RoomEquivalent", "RouteRooms", "CK_RouteRooms_RoomEquivalent", "`MetresPerRoomEquivalent` > 0", "(`MetresPerRoomEquivalent` > 0)", "`MetresPerRoomEquivalent` > 0", "(`MetresPerRoomEquivalent` > 0)", true),
		new("RouteCells", "CK_RouteCells_TopologyVersion", "RouteRooms", "CK_RouteRooms_TopologyVersion", "`TopologyVersion` >= 1", "(`TopologyVersion` >= 1)", "`TopologyVersion` >= 1", "(`TopologyVersion` >= 1)", true),
		new("RouteExitAnchors", "CK_RouteExitAnchors_Arrival", "RouteExitAnchors", "CK_RouteExitAnchors_Arrival", "`ArrivalPositionMetres` >= `MinimumPositionMetres` AND `ArrivalPositionMetres` <= `MaximumPositionMetres`", "((`ArrivalPositionMetres` >= `MinimumPositionMetres`) and (`ArrivalPositionMetres` <= `MaximumPositionMetres`))", "`ArrivalPositionMetres` >= `MinimumPositionMetres` AND `ArrivalPositionMetres` <= `MaximumPositionMetres`", "((`ArrivalPositionMetres` >= `MinimumPositionMetres`) and (`ArrivalPositionMetres` <= `MaximumPositionMetres`))", false),
		new("RouteExitAnchors", "CK_RouteExitAnchors_Band", "RouteExitAnchors", "CK_RouteExitAnchors_Band", "`MinimumPositionMetres` >= 0 AND `MaximumPositionMetres` >= `MinimumPositionMetres`", "((`MinimumPositionMetres` >= 0) and (`MaximumPositionMetres` >= `MinimumPositionMetres`))", "`MinimumPositionMetres` >= 0 AND `MaximumPositionMetres` >= `MinimumPositionMetres`", "((`MinimumPositionMetres` >= 0) and (`MaximumPositionMetres` >= `MinimumPositionMetres`))", false),
		new("Tracks", "CK_Tracks_Owner", "Tracks", "CK_Tracks_Owner", "(`VehicleId` IS NULL AND `CharacterId` IS NOT NULL AND `BodyPrototypeId` IS NOT NULL) OR (`VehicleId` IS NOT NULL AND `CharacterId` IS NULL AND `BodyPrototypeId` IS NULL)", "(((`VehicleId` is null) and (`CharacterId` is not null) and (`BodyPrototypeId` is not null)) or ((`VehicleId` is not null) and (`CharacterId` is null) and (`BodyPrototypeId` is null)))", "(`VehicleId` IS NULL AND `CharacterId` IS NOT NULL AND `BodyPrototypeId` IS NOT NULL) OR (`VehicleId` IS NOT NULL AND `CharacterId` IS NULL AND `BodyPrototypeId` IS NULL)", "(((`VehicleId` is null) and (`CharacterId` is not null) and (`BodyPrototypeId` is not null)) or ((`VehicleId` is not null) and (`CharacterId` is null) and (`BodyPrototypeId` is null)))", false),
		new("VehicleRoutePlatformBindings", "CK_VehicleRoutePlatformBindings_Tolerance", "VehicleRoutePlatformBindings", "CK_VehicleRoutePlatformBindings_Tolerance", "`DockingToleranceMetres` >= 0", "(`DockingToleranceMetres` >= 0)", "`DockingToleranceMetres` >= 0", "(`DockingToleranceMetres` >= 0)", false),
		new("VehicleRouteSteps", "CK_VehicleRouteSteps_Positions", "VehicleRouteSteps", "CK_VehicleRouteSteps_Positions", "(`OriginRoutePositionMetres` IS NULL OR `OriginRoutePositionMetres` >= 0) AND (`DestinationRoutePositionMetres` IS NULL OR `DestinationRoutePositionMetres` >= 0) AND ((`OriginRoutePositionMetres` IS NULL AND `PinnedTopologyVersion` IS NULL) OR (`OriginRoutePositionMetres` IS NOT NULL AND `PinnedTopologyVersion` IS NOT NULL AND `PinnedTopologyVersion` >= 1)) AND ((`DestinationRoutePositionMetres` IS NULL AND `DestinationTopologyVersion` IS NULL) OR (`DestinationRoutePositionMetres` IS NOT NULL AND `DestinationTopologyVersion` IS NOT NULL AND `DestinationTopologyVersion` >= 1))", "(((`OriginRoutePositionMetres` is null) or (`OriginRoutePositionMetres` >= 0)) and ((`DestinationRoutePositionMetres` is null) or (`DestinationRoutePositionMetres` >= 0)) and (((`OriginRoutePositionMetres` is null) and (`PinnedTopologyVersion` is null)) or ((`OriginRoutePositionMetres` is not null) and (`PinnedTopologyVersion` is not null) and (`PinnedTopologyVersion` >= 1))) and (((`DestinationRoutePositionMetres` is null) and (`DestinationTopologyVersion` is null)) or ((`DestinationRoutePositionMetres` is not null) and (`DestinationTopologyVersion` is not null) and (`DestinationTopologyVersion` >= 1))))", "(`OriginRoutePositionMetres` IS NULL OR `OriginRoutePositionMetres` >= 0) AND (`DestinationRoutePositionMetres` IS NULL OR `DestinationRoutePositionMetres` >= 0) AND ((`OriginRoutePositionMetres` IS NULL AND `PinnedTopologyVersion` IS NULL) OR (`OriginRoutePositionMetres` IS NOT NULL AND `PinnedTopologyVersion` IS NOT NULL AND `PinnedTopologyVersion` >= 1)) AND ((`DestinationRoutePositionMetres` IS NULL AND `DestinationTopologyVersion` IS NULL) OR (`DestinationRoutePositionMetres` IS NOT NULL AND `DestinationTopologyVersion` IS NOT NULL AND `DestinationTopologyVersion` >= 1))", "(((`OriginRoutePositionMetres` is null) or (`OriginRoutePositionMetres` >= 0)) and ((`DestinationRoutePositionMetres` is null) or (`DestinationRoutePositionMetres` >= 0)) and (((`OriginRoutePositionMetres` is null) and (`PinnedTopologyVersion` is null)) or ((`OriginRoutePositionMetres` is not null) and (`PinnedTopologyVersion` is not null) and (`PinnedTopologyVersion` >= 1))) and (((`DestinationRoutePositionMetres` is null) and (`DestinationTopologyVersion` is null)) or ((`DestinationRoutePositionMetres` is not null) and (`DestinationTopologyVersion` is not null) and (`DestinationTopologyVersion` >= 1))))", false),
		new("VehicleRouteSteps", "CK_VehicleRouteSteps_RoomEquivalentCost", "VehicleRouteSteps", "CK_VehicleRouteSteps_RoomEquivalentCost", "`RoomEquivalentCost` >= 0", "(`RoomEquivalentCost` >= 0)", "`RoomEquivalentCost` >= 0", "(`RoomEquivalentCost` >= 0)", false),
		new("VehicleRouteSteps", "CK_VehicleRouteSteps_Sequence", "VehicleRouteSteps", "CK_VehicleRouteSteps_Sequence", "`Sequence` >= 0", "(`Sequence` >= 0)", "`Sequence` >= 0", "(`Sequence` >= 0)", false),
		new("VehicleRouteSteps", "CK_VehicleRouteSteps_TypedPayload", "VehicleRouteSteps", "CK_VehicleRouteSteps_TypedPayload", "(`StepType` = 0 AND `ExitId` IS NULL AND `Direction` IS NOT NULL AND `Direction` IN (-1, 1) AND `PinnedTopologyVersion` IS NOT NULL AND `DestinationTopologyVersion` = `PinnedTopologyVersion` AND `DistanceMetres` IS NOT NULL AND `DistanceMetres` >= 0 AND `OriginRoutePositionMetres` IS NOT NULL AND `DestinationRoutePositionMetres` IS NOT NULL AND `OriginCellId` = `DestinationCellId` AND `OriginRoomLayer` = `DestinationRoomLayer`) OR (`StepType` = 1 AND `ExitId` IS NOT NULL AND `Direction` IS NULL AND `DistanceMetres` IS NULL)", "(((`StepType` = 0) and (`ExitId` is null) and (`Direction` is not null) and (`Direction` in (-(1),1)) and (`PinnedTopologyVersion` is not null) and (`DestinationTopologyVersion` = `PinnedTopologyVersion`) and (`DistanceMetres` is not null) and (`DistanceMetres` >= 0) and (`OriginRoutePositionMetres` is not null) and (`DestinationRoutePositionMetres` is not null) and (`OriginCellId` = `DestinationCellId`) and (`OriginRoomLayer` = `DestinationRoomLayer`)) or ((`StepType` = 1) and (`ExitId` is not null) and (`Direction` is null) and (`DistanceMetres` is null)))", "(`StepType` = 0 AND `ExitId` IS NULL AND `Direction` IS NOT NULL AND `Direction` IN (-1, 1) AND `PinnedTopologyVersion` IS NOT NULL AND `DestinationTopologyVersion` = `PinnedTopologyVersion` AND `DistanceMetres` IS NOT NULL AND `DistanceMetres` >= 0 AND `OriginRoutePositionMetres` IS NOT NULL AND `DestinationRoutePositionMetres` IS NOT NULL AND `OriginRoomId` = `DestinationRoomId` AND `OriginRoomLayer` = `DestinationRoomLayer`) OR (`StepType` = 1 AND `ExitId` IS NOT NULL AND `Direction` IS NULL AND `DistanceMetres` IS NULL)", "(((`StepType` = 0) and (`ExitId` is null) and (`Direction` is not null) and (`Direction` in (-(1),1)) and (`PinnedTopologyVersion` is not null) and (`DestinationTopologyVersion` = `PinnedTopologyVersion`) and (`DistanceMetres` is not null) and (`DistanceMetres` >= 0) and (`OriginRoutePositionMetres` is not null) and (`DestinationRoutePositionMetres` is not null) and (`OriginRoomId` = `DestinationRoomId`) and (`OriginRoomLayer` = `DestinationRoomLayer`)) or ((`StepType` = 1) and (`ExitId` is not null) and (`Direction` is null) and (`DistanceMetres` is null)))", true),
		new("VehicleRouteStops", "CK_VehicleRouteStops_Dwell", "VehicleRouteStops", "CK_VehicleRouteStops_Dwell", "`DwellDurationMilliseconds` >= 0", "(`DwellDurationMilliseconds` >= 0)", "`DwellDurationMilliseconds` >= 0", "(`DwellDurationMilliseconds` >= 0)", false),
		new("VehicleRouteStops", "CK_VehicleRouteStops_RoutePosition", "VehicleRouteStops", "CK_VehicleRouteStops_RoutePosition", "`RoutePositionMetres` IS NULL OR `RoutePositionMetres` >= 0", "((`RoutePositionMetres` is null) or (`RoutePositionMetres` >= 0))", "`RoutePositionMetres` IS NULL OR `RoutePositionMetres` >= 0", "((`RoutePositionMetres` is null) or (`RoutePositionMetres` >= 0))", false),
		new("VehicleRouteStops", "CK_VehicleRouteStops_Sequence", "VehicleRouteStops", "CK_VehicleRouteStops_Sequence", "`Sequence` >= 0", "(`Sequence` >= 0)", "`Sequence` >= 0", "(`Sequence` >= 0)", false),
		new("VehicleRouteTopologyPins", "CK_VehicleRouteTopologyPins_Version", "VehicleRouteTopologyPins", "CK_VehicleRouteTopologyPins_Version", "`TopologyVersion` >= 1", "(`TopologyVersion` >= 1)", "`TopologyVersion` >= 1", "(`TopologyVersion` >= 1)", false),
	];

	private static readonly string[] SourceCheckTables = ["activeprojects", "activeroutemotions", "agriculturefields", "areas_cells", "arenacells", "auctionhouses", "bankbranches", "cellenvironmentalstates", "celloverlaypackages", "celloverlays", "celloverlays_exits", "cellroomareacontractionledger", "cellroomareamigrationledger", "cellroomcontractionledger", "cellroommigrationledger", "cells", "cells_foragableyields", "cells_gameitems", "cells_magicresources", "cells_rangedcovers", "cells_tags", "characterlog", "clans_administrationcells", "clans_hallcells", "clans_treasurycells", "conveyancinglocations", "corpserecoveryreports", "employmentactionsteps", "environmentalmagicoperations", "exits", "hooks_perceivables", "hospitallocations", "hospitalservicerequests", "hotelrooms", "jobfindinglocations", "landrejuvenationtreatments", "legalauthoritiycells", "legalauthorityjailcells", "magicgatheringoperations", "magicgatheringparticipants", "magicportalendpoints", "npcspawnercells", "patrolroutesnodes", "probatelocations", "propertylocations", "restaurantcells", "routecelllandmarks", "routecells", "routeexitanchors", "shops", "shops_storeroomcells", "stables", "terrains", "tracks", "vehiclecompartments", "vehicledockings", "vehiclerouteplatformbindings", "vehicleroutesteps", "vehicleroutestops", "vehicleroutetopologypins", "vehicles", "zones"];
	private static readonly string[] TargetCheckTables = ["activeprojects", "activeroutemotions", "agriculturefields", "areas_rooms", "arenarooms", "auctionhouses", "bankbranches", "roomenvironmentalstates", "roomoverlaypackages", "roomoverlays", "roomoverlays_exits", "roomspatialareacontractionledger", "roomspatialareamigrationledger", "roomspatialcontractionledger", "roomspatialmigrationledger", "rooms", "rooms_foragableyields", "rooms_gameitems", "rooms_magicresources", "rooms_rangedcovers", "rooms_tags", "characterlog", "clans_administrationrooms", "clans_hallrooms", "clans_treasuryrooms", "conveyancinglocations", "corpserecoveryreports", "employmentactionsteps", "environmentalmagicoperations", "exits", "hooks_perceivables", "hospitallocations", "hospitalservicerequests", "hotelrooms", "jobfindinglocations", "landrejuvenationtreatments", "legalauthoritiyrooms", "legalauthorityjailrooms", "magicgatheringoperations", "magicgatheringparticipants", "magicportalendpoints", "npcspawnerrooms", "patrolroutesnodes", "probatelocations", "propertylocations", "restaurantrooms", "routeroomlandmarks", "routerooms", "routeexitanchors", "shops", "shops_storeroomrooms", "stables", "terrains", "tracks", "vehiclecompartments", "vehicledockings", "vehiclerouteplatformbindings", "vehicleroutesteps", "vehicleroutestops", "vehicleroutetopologypins", "vehicles", "zones"];
	private const string CheckCaptureTable = "fm_room_naming_checks_20261007043900";

	private static string CheckLiteral(string value) => "'" + value.Replace("'", "''") + "'";

	private static string CheckSignature(string value) =>
		System.Text.RegularExpressions.Regex.Replace(value.Replace("`", ""), @"\s", "")
			.ToLowerInvariant();

	private static string ReviewedCheckGuard(bool afterRename)
	{
		var sql = new System.Text.StringBuilder();
		if (!afterRename)
		{
			sql.AppendLine($"""
  CREATE TEMPORARY TABLE `{CheckCaptureTable}`(
   SourceTable VARCHAR(64) NOT NULL,SourceName VARCHAR(64) COLLATE utf8mb4_bin NOT NULL PRIMARY KEY,
   TargetTable VARCHAR(64) NOT NULL,TargetName VARCHAR(64) COLLATE utf8mb4_bin NOT NULL,
   SourceSignature LONGTEXT NOT NULL,SourceNativeSignature LONGTEXT NOT NULL,
   TargetSignature LONGTEXT NOT NULL,TargetNativeSignature LONGTEXT NOT NULL,
   NeedsRecreate BOOL NOT NULL,OriginalEnforced VARCHAR(3) COLLATE utf8mb4_bin NULL)
   CHARACTER SET utf8mb4 COLLATE utf8mb4_bin;
""");
			foreach (var check in ReviewedChecks)
			{
				var values = new[] {check.SourceTable, check.SourceName, check.TargetTable, check.TargetName,
					CheckSignature(check.SourceSql), CheckSignature(check.SourceNativeSql),
					CheckSignature(check.TargetSql), CheckSignature(check.TargetNativeSql)};
				sql.AppendLine($"  INSERT INTO `{CheckCaptureTable}` VALUES ({string.Join(",", System.Array.ConvertAll(values, CheckLiteral))},{(check.Recreate ? 1 : 0)},NULL);");
			}
		}
		// Use each temporary table once per statement: MySQL cannot reopen it.
		var phase = afterRename ? "Target" : "Source";
		var tables = afterRename ? TargetCheckTables : SourceCheckTables;
		var tableList = string.Join(",", System.Array.ConvertAll(tables, CheckLiteral));
		sql.AppendLine($"""
  SET diagnostic=NULL;
  SELECT LEFT(CONCAT('Room naming: unclassified check ',t.TABLE_NAME,'.',t.CONSTRAINT_NAME),128) INTO diagnostic
   FROM information_schema.TABLE_CONSTRAINTS t
   LEFT JOIN `{CheckCaptureTable}` expected ON BINARY t.CONSTRAINT_NAME=BINARY expected.{phase}Name
    AND LOWER(t.TABLE_NAME)=LOWER(expected.{phase}Table)
   WHERE t.CONSTRAINT_SCHEMA=DATABASE() AND t.CONSTRAINT_TYPE='CHECK'
    AND LOWER(t.TABLE_NAME) IN ({tableList}) AND expected.SourceName IS NULL LIMIT 1;
  IF diagnostic IS NOT NULL THEN SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT=diagnostic; END IF;
  IF (SELECT COUNT(*) FROM `{CheckCaptureTable}`)<>28 THEN
   SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT='Room naming: reviewed check capture is incomplete';
  END IF;
  IF (SELECT COUNT(*) FROM `{CheckCaptureTable}` WHERE NeedsRecreate=1)<>10 THEN
   SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT='Room naming: reviewed check dependency set differs';
  END IF;
  SET diagnostic=NULL;
  SELECT LEFT(CONCAT('Room naming: check shape/enforcement differs ',expected.{phase}Name),128) INTO diagnostic
   FROM `{CheckCaptureTable}` expected
   LEFT JOIN information_schema.TABLE_CONSTRAINTS t ON t.CONSTRAINT_SCHEMA=DATABASE()
    AND BINARY t.CONSTRAINT_NAME=BINARY expected.{phase}Name
    AND LOWER(t.TABLE_NAME)=LOWER(expected.{phase}Table) AND t.CONSTRAINT_TYPE='CHECK'
   LEFT JOIN information_schema.CHECK_CONSTRAINTS c ON c.CONSTRAINT_SCHEMA=t.CONSTRAINT_SCHEMA
    AND BINARY c.CONSTRAINT_NAME=BINARY t.CONSTRAINT_NAME
   WHERE c.CHECK_CLAUSE IS NULL OR t.ENFORCED IS NULL OR BINARY t.ENFORCED NOT IN ('YES','NO')
    OR LOCATE('``',c.CHECK_CLAUSE)>0
    OR REGEXP_LIKE(REGEXP_REPLACE(c.CHECK_CLAUSE,'`[A-Za-z_][A-Za-z0-9_]*`',''),'[`''"]')
    OR BINARY LOWER(REGEXP_REPLACE(REPLACE(c.CHECK_CLAUSE,'`',''),'[[:space:]]',''))
     NOT IN (BINARY expected.{phase}Signature,BINARY expected.{phase}NativeSignature)
""");
		if (afterRename)
		{
			sql.AppendLine("    OR expected.OriginalEnforced IS NULL OR BINARY t.ENFORCED<>BINARY expected.OriginalEnforced");
		}
		sql.AppendLine("   LIMIT 1;\n  IF diagnostic IS NOT NULL THEN SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT=diagnostic; END IF;");
		if (afterRename)
		{
			sql.AppendLine($"  DROP TEMPORARY TABLE `{CheckCaptureTable}`;");
		}
		else
		{
			sql.AppendLine($"""
  UPDATE `{CheckCaptureTable}` expected
   JOIN information_schema.TABLE_CONSTRAINTS t ON t.CONSTRAINT_SCHEMA=DATABASE()
    AND BINARY t.CONSTRAINT_NAME=BINARY expected.SourceName
    AND LOWER(t.TABLE_NAME)=LOWER(expected.SourceTable) AND t.CONSTRAINT_TYPE='CHECK'
   SET expected.OriginalEnforced=t.ENFORCED;
  IF EXISTS(SELECT 1 FROM `{CheckCaptureTable}` WHERE OriginalEnforced IS NULL OR BINARY OriginalEnforced NOT IN ('YES','NO')) THEN
   SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT='Room naming: original check enforcement capture is incomplete';
  END IF;
""");
		}
		return sql.ToString();
	}

	private static void AddChecksWithPreservedEnforcement(MigrationBuilder migrationBuilder)
	{
		foreach (var check in ReviewedChecks)
		{
			if (!check.Recreate) continue;
			var statement = $"ALTER TABLE `{check.TargetTable}` ADD CONSTRAINT `{check.TargetName}` CHECK ({check.TargetSql})";
			migrationBuilder.Sql($"""
SET @fm_room_naming_check_statement_20261007043900 = (
 SELECT CONCAT({CheckLiteral(statement)},CASE BINARY OriginalEnforced WHEN 'YES' THEN ' ENFORCED' WHEN 'NO' THEN ' NOT ENFORCED' END)
 FROM `{CheckCaptureTable}`
 WHERE BINARY SourceName={CheckLiteral(check.SourceName)} AND NeedsRecreate=1
  AND BINARY OriginalEnforced IN ('YES','NO'));
PREPARE fm_room_naming_check_statement_20261007043900 FROM @fm_room_naming_check_statement_20261007043900;
EXECUTE fm_room_naming_check_statement_20261007043900;
DEALLOCATE PREPARE fm_room_naming_check_statement_20261007043900;
SET @fm_room_naming_check_statement_20261007043900 = NULL;
""", suppressTransaction: true);
		}
	}

 protected override void Down(MigrationBuilder migrationBuilder) =>
  throw new System.NotSupportedException("Room cutover has no supported automatic downgrade. Stop writers and restore the verified pre-cutover database, matching binary and external files.");
}
