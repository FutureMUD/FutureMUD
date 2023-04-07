ALTER TABLE `dbo`.`GameItems` 
ADD COLUMN `Condition` DOUBLE NOT NULL DEFAULT 1.0 AFTER `GameItemProtoRevision`;
