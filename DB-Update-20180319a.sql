CREATE TABLE `dbo`.`PopulationBloodModels` (
  `Id` BIGINT NOT NULL AUTO_INCREMENT,
  `Name` VARCHAR(200) NOT NULL,
  PRIMARY KEY (`Id`));
CREATE TABLE `dbo`.`PopulationBloodModels_Bloodtypes` (
  `BloodtypeId` BIGINT NOT NULL,
  `PopulationBloodModelId` BIGINT NOT NULL,
  `Weight` DOUBLE NOT NULL DEFAULT 1.0,
  PRIMARY KEY (`BloodtypeId`, `PopulationBloodModelId`),
  INDEX `FK_PopulationBloodModels_Bloodtypes_PopulationBloodModels_idx` (`PopulationBloodModelId` ASC),
  CONSTRAINT `FK_PopulationBloodModels_Bloodtypes_Bloodtypes`
    FOREIGN KEY (`BloodtypeId`)
    REFERENCES `dbo`.`Bloodtypes` (`Id`)
    ON DELETE CASCADE
    ON UPDATE CASCADE,
  CONSTRAINT `FK_PopulationBloodModels_Bloodtypes_PopulationBloodModels`
    FOREIGN KEY (`PopulationBloodModelId`)
    REFERENCES `dbo`.`PopulationBloodModels` (`Id`)
    ON DELETE CASCADE
    ON UPDATE CASCADE);
ALTER TABLE `dbo`.`Ethnicities` 
ADD COLUMN `PopulationBloodModelId` BIGINT NULL AFTER `EthnicSubgroup`,
ADD INDEX `FK_Ethnicities_PopulationBloodModels_idx` (`PopulationBloodModelId` ASC);
ALTER TABLE `dbo`.`Ethnicities` 
ADD CONSTRAINT `FK_Ethnicities_PopulationBloodModels`
  FOREIGN KEY (`PopulationBloodModelId`)
  REFERENCES `dbo`.`PopulationBloodModels` (`Id`)
  ON DELETE SET NULL
  ON UPDATE CASCADE;
ALTER TABLE `dbo`.`CraftProducts` 
CHANGE COLUMN `Id` `Id` BIGINT(20) NOT NULL ,
ADD COLUMN `MaterialDefiningInputId` BIGINT NULL AFTER `IsFailProduct`;
ALTER TABLE `dbo`.`CraftProducts` 
CHANGE COLUMN `MaterialDefiningInputId` `MaterialDefiningInputIndex` BIGINT(20) NULL DEFAULT NULL ;
ALTER TABLE `dbo`.`CraftProducts` 
CHANGE COLUMN `MaterialDefiningInputIndex` `MaterialDefiningInputIndex` INT NULL DEFAULT NULL ;
