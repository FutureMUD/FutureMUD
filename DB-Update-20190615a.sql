CREATE TABLE `dbo`.`DisfigurementTemplates` (
  `Id` BIGINT NOT NULL,
  `RevisionNumber` INT NOT NULL,
  `Name` VARCHAR(500) NOT NULL,
  `Type` VARCHAR(50) NOT NULL,
  `EditableItemId` BIGINT NOT NULL,
  `ShortDescription` VARCHAR(500) NOT NULL,
  `FullDescription` VARCHAR(500) NOT NULL,
  `Definition` TEXT NOT NULL,
  PRIMARY KEY (`Id`, `RevisionNumber`),
  INDEX `FK_DisfigurementTemplates_EditableItems_idx` (`EditableItemId` ASC),
  CONSTRAINT `FK_DisfigurementTemplates_EditableItems`
    FOREIGN KEY (`EditableItemId`)
    REFERENCES `dbo`.`EditableItems` (`Id`)
    ON DELETE CASCADE
    ON UPDATE CASCADE);
	ALTER TABLE `dbo`.`BodypartProto` 
ADD COLUMN `Size` INT NOT NULL DEFAULT 5 AFTER `ImplantSpaceOccupied`;

ALTER TABLE `dbo`.`Races` 
ADD COLUMN `BodypartSizeModifier` INT NOT NULL DEFAULT 0 AFTER `TemperatureRangeCeiling`,
ADD COLUMN `BodypartHealthMultiplier` DOUBLE NOT NULL DEFAULT 1.0 AFTER `BodypartSizeModifier`;
ALTER TABLE `dbo`.`Bodies` 
ADD COLUMN `Tattoos` MEDIUMTEXT NULL AFTER `FullDescriptionPatternId`;
