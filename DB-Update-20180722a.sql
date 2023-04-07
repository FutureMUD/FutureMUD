CREATE TABLE `dbo`.`ClimateModels` (
  `Id` BIGINT NOT NULL AUTO_INCREMENT,
  `Name` VARCHAR(200) NOT NULL,
  `Definition` TEXT NOT NULL,
  PRIMARY KEY (`Id`));
CREATE TABLE `dbo`.`Seasons` (
  `Id` BIGINT NOT NULL AUTO_INCREMENT,
  `Name` VARCHAR(200) NOT NULL,
  PRIMARY KEY (`Id`));
CREATE TABLE `dbo`.`RegionalClimates` (
  `Id` BIGINT NOT NULL AUTO_INCREMENT,
  `Name` VARCHAR(200) NOT NULL,
  `ClimateModelId` BIGINT NOT NULL,
  PRIMARY KEY (`Id`));
CREATE TABLE `dbo`.`RegionalClimates_Seasons` (
  `RegionalClimateId` BIGINT NOT NULL,
  `SeasonId` BIGINT NOT NULL,
  `TemperatureInfo` TEXT NOT NULL,
  PRIMARY KEY (`RegionalClimateId`, `SeasonId`),
  INDEX `FK_RegionalClimates_Seasons_Seasons_idx` (`SeasonId` ASC),
  CONSTRAINT `FK_RegionalClimates_Seasons_RegionalClimates`
    FOREIGN KEY (`RegionalClimateId`)
    REFERENCES `dbo`.`RegionalClimates` (`Id`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION,
  CONSTRAINT `FK_RegionalClimates_Seasons_Seasons`
    FOREIGN KEY (`SeasonId`)
    REFERENCES `dbo`.`Seasons` (`Id`)
    ON DELETE CASCADE
    ON UPDATE CASCADE);
ALTER TABLE `dbo`.`Seasons` 
ADD COLUMN `Description` TEXT NOT NULL AFTER `Name`;
CREATE TABLE `dbo`.`WeatherEvents` (
  `Id` BIGINT NOT NULL AUTO_INCREMENT,
  `Name` VARCHAR(200) NOT NULL,
  `WeatherEventType` VARCHAR(45) NOT NULL,
  `WeatherDescription` TEXT NOT NULL,
  `WeatherRoomAddendum` TEXT NOT NULL,
  `TemperatureEffect` DOUBLE NOT NULL,
  `Precipitation` INT NOT NULL,
  `Wind` INT NOT NULL,
  `AdditionalInfo` TEXT NULL,
  PRIMARY KEY (`Id`));
