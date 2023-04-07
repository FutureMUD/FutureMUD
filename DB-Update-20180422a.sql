CREATE TABLE `dbo`.`MagicSchools` (
  `Id` BIGINT NOT NULL AUTO_INCREMENT,
  `Name` VARCHAR(200) NOT NULL,
  `ParentSchoolId` BIGINT NULL,
  `SchoolVerb` VARCHAR(45) NOT NULL,
  `SchoolAdjective` VARCHAR(45) NOT NULL,
  `PowerListColour` VARCHAR(45) NOT NULL,
  PRIMARY KEY (`Id`));
ALTER TABLE `dbo`.`MagicSchools` 
ADD INDEX `FK_MagicSchools_MagicSchools_idx` (`ParentSchoolId` ASC);
ALTER TABLE `dbo`.`MagicSchools` 
ADD CONSTRAINT `FK_MagicSchools_MagicSchools`
  FOREIGN KEY (`ParentSchoolId`)
  REFERENCES `dbo`.`MagicSchools` (`Id`)
  ON DELETE CASCADE
  ON UPDATE CASCADE;
CREATE TABLE `dbo`.`MagicCapabilities` (
  `Id` BIGINT NOT NULL AUTO_INCREMENT,
  `Name` VARCHAR(200) NOT NULL,
  `CapabilityModel` VARCHAR(45) NOT NULL,
  `PowerLevel` INT NOT NULL DEFAULT 1,
  `Definition` TEXT NOT NULL,
  `MagicSchoolId` BIGINT NOT NULL,
  PRIMARY KEY (`Id`),
  INDEX `FK_MagicCapabilities_MagicSchools_idx` (`MagicSchoolId` ASC),
  CONSTRAINT `FK_MagicCapabilities_MagicSchools`
    FOREIGN KEY (`MagicSchoolId`)
    REFERENCES `dbo`.`MagicSchools` (`Id`)
    ON DELETE CASCADE
    ON UPDATE CASCADE);
CREATE TABLE `dbo`.`MagicPowers` (
  `Id` BIGINT NOT NULL AUTO_INCREMENT,
  `Name` VARCHAR(200) NOT NULL,
  `Blurb` VARCHAR(500) NOT NULL,
  `ShowHelp` TEXT NOT NULL,
  `PowerModel` VARCHAR(45) NOT NULL,
  `Definition` TEXT NOT NULL,
  `MagicSchoolId` BIGINT NOT NULL,
  PRIMARY KEY (`Id`),
  INDEX `FK_MagicPowers_MagicSchools_idx` (`MagicSchoolId` ASC),
  CONSTRAINT `FK_MagicPowers_MagicSchools`
    FOREIGN KEY (`MagicSchoolId`)
    REFERENCES `dbo`.`MagicSchools` (`Id`)
    ON DELETE CASCADE
    ON UPDATE CASCADE);
