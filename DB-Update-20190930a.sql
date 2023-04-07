ALTER TABLE `dbo`.`GameItems` 
ADD COLUMN `RoomLayer` INT NOT NULL DEFAULT 0 AFTER `GameItemProtoRevision`;
ALTER TABLE `dbo`.`Characters` 
ADD COLUMN `RoomLayer` INT NOT NULL DEFAULT 0 AFTER `NameInfo`;
