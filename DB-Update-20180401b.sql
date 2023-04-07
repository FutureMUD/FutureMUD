ALTER TABLE `dbo`.`BodypartProto` 
ADD COLUMN `ImplantSpace` DOUBLE NOT NULL DEFAULT 0 AFTER `ArmourTypeId`,
ADD COLUMN `ImplantSpaceOccupied` DOUBLE NOT NULL DEFAULT 0 AFTER `ImplantSpace`;
