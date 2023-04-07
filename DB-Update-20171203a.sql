CREATE TABLE `dbo`.`BloodtypeAntigens` (
  `Id` BIGINT NOT NULL AUTO_INCREMENT,
  `Name` VARCHAR(200) NOT NULL,
  PRIMARY KEY (`Id`));
CREATE TABLE `dbo`.`Bloodtypes` (
  `Id` BIGINT NOT NULL AUTO_INCREMENT,
  `Name` VARCHAR(200) NOT NULL,
  PRIMARY KEY (`Id`));
CREATE TABLE `dbo`.`Bloodtypes_BloodtypeAntigens` (
  `BloodtypeId` BIGINT NOT NULL,
  `BloodtypeAntigenId` BIGINT NOT NULL,
  PRIMARY KEY (`BloodtypeId`, `BloodtypeAntigenId`),
  INDEX `FK_Bloodtypes_BloodtypeAntigens_BloodtypeAntigens_idx` (`BloodtypeAntigenId` ASC),
  CONSTRAINT `FK_Bloodtypes_BloodtypeAntigens_Bloodtypes`
    FOREIGN KEY (`BloodtypeId`)
    REFERENCES `dbo`.`Bloodtypes` (`Id`)
    ON DELETE CASCADE
    ON UPDATE CASCADE,
  CONSTRAINT `FK_Bloodtypes_BloodtypeAntigens_BloodtypeAntigens`
    FOREIGN KEY (`BloodtypeAntigenId`)
    REFERENCES `dbo`.`BloodtypeAntigens` (`Id`)
    ON DELETE CASCADE
    ON UPDATE CASCADE);
CREATE TABLE `dbo`.`BloodModels` (
  `Id` BIGINT NOT NULL AUTO_INCREMENT,
  `Name` VARCHAR(200) NOT NULL,
  PRIMARY KEY (`Id`));
CREATE TABLE `dbo`.`BloodModels_Bloodtypes` (
  `BloodModelId` BIGINT NOT NULL,
  `BloodtypeId` BIGINT NOT NULL,
  PRIMARY KEY (`BloodModelId`, `BloodtypeId`),
  INDEX `FK_BloodModels_Bloodtypes_Bloodtypes_idx` (`BloodtypeId` ASC),
  CONSTRAINT `FK_BloodModels_Bloodtypes_BloodModels`
    FOREIGN KEY (`BloodModelId`)
    REFERENCES `dbo`.`BloodModels` (`Id`)
    ON DELETE CASCADE
    ON UPDATE CASCADE,
  CONSTRAINT `FK_BloodModels_Bloodtypes_Bloodtypes`
    FOREIGN KEY (`BloodtypeId`)
    REFERENCES `dbo`.`Bloodtypes` (`Id`)
    ON DELETE CASCADE
    ON UPDATE CASCADE);
	ALTER TABLE `dbo`.`Bodies` 
ADD COLUMN `BloodtypeId` BIGINT NULL AFTER `EthnicityId`,
ADD INDEX `FK_Bodies_Bloodtypes_idx` (`BloodtypeId` ASC);
ALTER TABLE `dbo`.`Bodies` 
ADD CONSTRAINT `FK_Bodies_Bloodtypes`
  FOREIGN KEY (`BloodtypeId`)
  REFERENCES `dbo`.`Bloodtypes` (`Id`)
  ON DELETE SET NULL
  ON UPDATE CASCADE;
ALTER TABLE `dbo`.`Races` 
ADD COLUMN `BloodModelId` BIGINT NULL AFTER `RaceButcheryProfileId`,
ADD INDEX `FK_Races_BloodModels_idx` (`BloodModelId` ASC);
ALTER TABLE `dbo`.`Races` 
ADD CONSTRAINT `FK_Races_BloodModels`
  FOREIGN KEY (`BloodModelId`)
  REFERENCES `dbo`.`BloodModels` (`Id`)
  ON DELETE SET NULL
  ON UPDATE CASCADE;
