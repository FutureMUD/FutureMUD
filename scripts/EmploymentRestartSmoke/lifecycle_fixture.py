"""Narrow stopped-world fixture for a lodged item in saved severed remains."""
from __future__ import annotations

import re
import subprocess


def seed_lodged_remains_fixture(client: list[str], database: str, run_process,
                                parent_item_id: int, lodged_item_id: int,
                                creationflags: int) -> dict[str, str]:
    """Attach one exact, unclaimed room item to one exact severed part and its wound.

    The caller must first validate the owned MySQL instance and require its MUD to
    be stopped. Identifiers are checked here before any SQL is constructed.
    """
    if run_process is not None:
        raise RuntimeError("Lodged-remains fixtures require the owned MUD to be stopped.")
    if (type(parent_item_id) is not int or type(lodged_item_id) is not int or
            parent_item_id <= 0 or lodged_item_id <= 0 or parent_item_id == lodged_item_id):
        raise ValueError("Fixture item IDs must be distinct positive integers.")

    def run(query: str) -> str:
        result = subprocess.run(client + [database, "-e", query], capture_output=True,
                                text=True, encoding="utf-8", errors="replace",
                                creationflags=creationflags, timeout=60)
        if result.returncode:
            raise RuntimeError(result.stderr[:2000])
        return result.stdout.strip()

    parent, child = parent_item_id, lodged_item_id
    parent_rows = run(
        "SELECT c.Id,p.Type,"
        "c.Definition REGEXP '<Wounds[[:space:]]*/>', "
        "c.Definition REGEXP '<Wound>', "
        "REGEXP_SUBSTR(c.Definition,'<Part>[0-9]+</Part>') "
        "FROM GameItemComponents c JOIN GameItemComponentProtos p "
        "ON p.Id=c.GameItemComponentProtoId "
        "AND p.RevisionNumber=c.GameItemComponentProtoRevision "
        f"WHERE c.GameItemId={parent} AND p.Type='Bodypart'")
    rows = parent_rows.splitlines()
    if len(rows) != 1:
        raise RuntimeError("The exact remains item must have exactly one Bodypart component.")
    fields = rows[0].split("\t")
    if len(fields) != 5 or fields[2:4] != ["1", "0"]:
        raise RuntimeError("The remains Bodypart component must have an empty canonical Wounds list.")
    part_match = re.fullmatch(r"<Part>([1-9][0-9]*)</Part>", fields[4])
    if part_match is None:
        raise RuntimeError("The remains component has no supported part prototype to associate with the wound.")
    component_id = int(fields[0])
    bodypart_id = int(part_match.group(1))

    item_count = run(f"SELECT COUNT(*) FROM GameItems WHERE Id IN ({parent},{child})")
    if item_count != "2":
        raise RuntimeError("Both exact fixture items must exist in the owned world.")

    child_claims = run(
        "SELECT "
        f"(SELECT COUNT(*) FROM GameItems WHERE Id={child} AND ContainerId IS NULL),"
        f"(SELECT COUNT(*) FROM Rooms_GameItems WHERE GameItemId={child}),"
        f"(SELECT COUNT(*) FROM Bodies_GameItems WHERE GameItemId={child}),"
        f"(SELECT COUNT(*) FROM Bodies_Implants WHERE ImplantId={child}),"
        f"(SELECT COUNT(*) FROM Bodies_Prosthetics WHERE ProstheticId={child}),"
        f"(SELECT COUNT(*) FROM Wounds WHERE GameItemId={child} OR LodgedItemId={child} OR ToolOriginId={child}),"
        f"(SELECT COUNT(*) FROM GameItemComponents WHERE Definition REGEXP '<Item>[[:space:]]*{child}[[:space:]]*</Item>')")
    claims = child_claims.split("\t")
    if claims != ["1", "1", "0", "0", "0", "0", "0"]:
        raise RuntimeError("The lodged item must have one room claim and no competing native custody or wound claims: " + child_claims)

    existing_wound = run(
        f"SELECT COUNT(*) FROM Wounds WHERE GameItemId={parent} OR LodgedItemId={child}")
    if existing_wound != "0":
        raise RuntimeError("The exact remains and lodged item already participate in a wound row.")

    procedure = f"QA_SEED_LODGED_{parent}_{child}"
    procedure_sql = f"""CREATE PROCEDURE {procedure}()
BEGIN
  DECLARE wound_id BIGINT;
  DECLARE changed_rows INT;
  DECLARE EXIT HANDLER FOR SQLEXCEPTION BEGIN ROLLBACK; RESIGNAL; END;
  START TRANSACTION;
  INSERT INTO Wounds (GameItemId, OriginalDamage, CurrentDamage, CurrentPain, CurrentShock,
    CurrentStun, LodgedItemId, DamageType, Internal, BodypartProtoId, ExtraInformation,
    ActorOriginId, ToolOriginId, WoundType, VehicleDamageZoneId, VehicleId, RealTimeOfWound)
  VALUES ({parent}, 1.0, 1.0, 0.0, 0.0, 0.0, {child}, 0, b'0', {bodypart_id},
    '<Definition><IsFriendlyWound>false</IsFriendlyWound></Definition>', NULL, NULL,
    'Simple', NULL, NULL, UTC_TIMESTAMP());
  SET wound_id = LAST_INSERT_ID();
  UPDATE GameItemComponents
  SET Definition = REPLACE(Definition, '<Wounds />',
      CONCAT('<Wounds><Wound>', wound_id, '</Wound></Wounds>'))
  WHERE Id = {component_id} AND GameItemId = {parent}
    AND Definition REGEXP '<Wounds[[:space:]]*/>';
  SET changed_rows = ROW_COUNT();
  IF changed_rows <> 1 THEN
    SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT = 'QA_REMAINS_COMPONENT_NOT_UPDATED';
  END IF;
  DELETE FROM Rooms_GameItems WHERE GameItemId = {child};
  SET changed_rows = ROW_COUNT();
  IF changed_rows <> 1 THEN
    SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT = 'QA_LODGED_ROOM_CLAIM_NOT_REMOVED';
  END IF;
  COMMIT;
  SELECT wound_id;
END"""
    created = False
    try:
        created = True
        create = subprocess.run(client + ["--delimiter=//", database, "-e",
            procedure_sql + f"// CALL {procedure}()// DROP PROCEDURE {procedure}//"],
            capture_output=True, text=True, encoding="utf-8", errors="replace",
            creationflags=creationflags, timeout=60)
        if create.returncode:
            raise RuntimeError(create.stderr[:2000])
        wound_id = create.stdout.strip().splitlines()[-1]
        if not re.fullmatch(r"[1-9][0-9]*", wound_id):
            raise RuntimeError("The stopped-world fixture did not return a valid wound ID.")
        created = False
    finally:
        if created:
            run(f"DROP PROCEDURE IF EXISTS {procedure}")

    verification = run(
        f"SELECT w.Id,w.GameItemId,w.LodgedItemId,w.WoundType,"
        f"c.Definition REGEXP CONCAT('<Wound>[[:space:]]*',w.Id,'[[:space:]]*</Wound>') "
        f"FROM Wounds w JOIN GameItemComponents c ON c.GameItemId=w.GameItemId "
        f"JOIN GameItemComponentProtos p ON p.Id=c.GameItemComponentProtoId "
        f"AND p.RevisionNumber=c.GameItemComponentProtoRevision "
        f"WHERE w.Id={wound_id} AND w.GameItemId={parent} AND w.LodgedItemId={child} "
        f"AND c.Id={component_id}")
    if verification != f"{wound_id}\t{parent}\t{child}\tSimple\t1":
        raise RuntimeError("The committed wound-to-remains fixture failed exact-ID verification.")
    return {"parentItemId": str(parent), "lodgedItemId": str(child),
            "woundId": wound_id, "bodypartId": str(bodypart_id),
            "componentId": str(component_id), "verification": verification}


def seed_item_lodging_fixture(client: list[str], database: str, run_process,
                              parent_item_id: int, lodged_item_id: int,
                              creationflags: int) -> dict[str, str]:
    """Add one exact supported leaf item as a wound-lodged child of another."""
    if run_process is not None:
        raise RuntimeError("Item-lodging fixtures require the owned MUD to be stopped.")
    if (type(parent_item_id) is not int or type(lodged_item_id) is not int or
            parent_item_id <= 0 or lodged_item_id <= 0 or parent_item_id == lodged_item_id):
        raise ValueError("Fixture item IDs must be distinct positive integers.")

    def run(query: str) -> str:
        result = subprocess.run(client + [database, "-e", query], capture_output=True,
                            text=True, encoding="utf-8", errors="replace",
                            creationflags=creationflags, timeout=60)
        if result.returncode:
            raise RuntimeError(result.stderr[:2000])
        return result.stdout.strip()

    parent, child = parent_item_id, lodged_item_id
    if run(f"SELECT COUNT(*) FROM GameItems WHERE Id IN ({parent},{child})") != "2":
        raise RuntimeError("Both exact fixture items must exist in the owned world.")
    parent_state = run(
        "SELECT "
        f"(SELECT COUNT(*) FROM Rooms_GameItems WHERE GameItemId={parent}),"
        f"(SELECT COUNT(*) FROM Bodies_GameItems WHERE GameItemId={parent}),"
        f"(SELECT COUNT(*) FROM Bodies_Implants WHERE ImplantId={parent}),"
        f"(SELECT COUNT(*) FROM Bodies_Prosthetics WHERE ProstheticId={parent}),"
        f"(SELECT COUNT(*) FROM GameItems WHERE Id={parent} AND ContainerId IS NULL),"
        f"(SELECT COUNT(*) FROM Wounds WHERE GameItemId={parent} OR LodgedItemId={parent})")
    if parent_state.split("\t") != ["1", "0", "0", "0", "1", "0"]:
        raise RuntimeError("Atomic-test parent must be one room item with no competing custody or wounds: " + parent_state)
    child_state = run(
        "SELECT "
        f"(SELECT COUNT(*) FROM Rooms_GameItems WHERE GameItemId={child}),"
        f"(SELECT COUNT(*) FROM Bodies_GameItems WHERE GameItemId={child}),"
        f"(SELECT COUNT(*) FROM Bodies_Implants WHERE ImplantId={child}),"
        f"(SELECT COUNT(*) FROM Bodies_Prosthetics WHERE ProstheticId={child}),"
        f"(SELECT COUNT(*) FROM GameItems WHERE Id={child} AND ContainerId IS NULL),"
        f"(SELECT COUNT(*) FROM Wounds WHERE GameItemId={child} OR LodgedItemId={child} OR ToolOriginId={child}),"
        f"(SELECT COUNT(*) FROM GameItemComponents WHERE Definition REGEXP '<Item>[[:space:]]*{child}[[:space:]]*</Item>')")
    if child_state.split("\t") != ["1", "0", "0", "0", "1", "0", "0"]:
        raise RuntimeError("Lodged leaf must be one room item with no competing native custody or wounds: " + child_state)
    allowed = "'Holdable','Stackable','Ammunition','Simple Lock'"
    for item in (parent, child):
        components = run(
            f"SELECT COUNT(*),SUM(p.Type NOT IN ({allowed})) "
            f"FROM GameItemComponents c JOIN GameItemComponentProtos p "
            f"ON p.Id=c.GameItemComponentProtoId AND p.RevisionNumber=c.GameItemComponentProtoRevision "
            f"WHERE c.GameItemId={item}")
        component_fields = components.split("\t")
        if (len(component_fields) != 2 or not component_fields[0].isdigit() or
                int(component_fields[0]) < 1 or component_fields[1] != "0"):
            raise RuntimeError(f"Item {item} must have only the exact supported leaf components: {components}")

    procedure = f"QA_SEED_ITEM_LODGING_{parent}_{child}"
    procedure_sql = f"""CREATE PROCEDURE {procedure}()
BEGIN
  DECLARE changed_rows INT;
  DECLARE EXIT HANDLER FOR SQLEXCEPTION BEGIN ROLLBACK; RESIGNAL; END;
  START TRANSACTION;
  INSERT INTO Wounds (GameItemId, OriginalDamage, CurrentDamage, CurrentPain, CurrentShock,
    CurrentStun, LodgedItemId, DamageType, Internal, BodypartProtoId, ExtraInformation,
    ActorOriginId, ToolOriginId, WoundType, VehicleDamageZoneId, VehicleId, RealTimeOfWound)
  VALUES ({parent}, 1.0, 1.0, 0.0, 0.0, 0.0, {child}, 0, b'0', NULL,
    '<Definition><IsFriendlyWound>false</IsFriendlyWound></Definition>', NULL, NULL,
    'Simple', NULL, NULL, UTC_TIMESTAMP());
  DELETE FROM Rooms_GameItems WHERE GameItemId = {child};
  SET changed_rows = ROW_COUNT();
  IF changed_rows <> 1 THEN
    SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT = 'QA_ITEM_LODGING_ROOM_CLAIM_NOT_REMOVED';
  END IF;
  COMMIT;
END"""
    created = True
    try:
        create = subprocess.run(client + ["--delimiter=//", database, "-e",
            procedure_sql + f"// CALL {procedure}()// DROP PROCEDURE {procedure}//"],
            capture_output=True, text=True, encoding="utf-8", errors="replace",
            creationflags=creationflags, timeout=60)
        if create.returncode:
            raise RuntimeError(create.stderr[:2000])
        created = False
    finally:
        if created:
            run(f"DROP PROCEDURE IF EXISTS {procedure}")

    verification = run(
        f"SELECT w.Id,w.GameItemId,w.LodgedItemId,w.WoundType,"
        f"(SELECT COUNT(*) FROM Rooms_GameItems WHERE GameItemId={child}),"
        f"(SELECT COUNT(*) FROM Wounds WHERE GameItemId={parent} AND LodgedItemId={child}) "
        f"FROM Wounds w WHERE w.GameItemId={parent} AND w.LodgedItemId={child} "
        f"AND w.WoundType='Simple'")
    fields = verification.split("\t")
    if len(fields) != 6 or fields[1:6] != [str(parent), str(child), "Simple", "0", "1"]:
        raise RuntimeError("Committed item lodging fixture failed exact-ID verification: " + verification)
    return {"parentItemId": str(parent), "lodgedItemId": str(child),
            "woundId": fields[0], "verification": verification}
