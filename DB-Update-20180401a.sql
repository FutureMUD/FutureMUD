ALTER TABLE `dbo`.`CellOverlays` 
ADD COLUMN `AtmosphereId` BIGINT NULL AFTER `AddedLight`,
ADD COLUMN `AtmosphereType` VARCHAR(45) NOT NULL DEFAULT 'gas' AFTER `AtmosphereId`;
