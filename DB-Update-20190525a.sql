CREATE TABLE `dbo`.`ProjectActions` (
  `Id` BIGINT NOT NULL AUTO_INCREMENT,
  `Name` VARCHAR(200) NOT NULL,
  `Description` VARCHAR(1000) NOT NULL,
  `Type` VARCHAR(45) NOT NULL,
  `SortOrder` INT NOT NULL DEFAULT 0,
  `Definition` TEXT NOT NULL,
  `ProjectPhaseId` BIGINT NOT NULL,
  PRIMARY KEY (`Id`),
  INDEX `FK_ProjectActions_ProjectPhases_idx` (`ProjectPhaseId` ASC),
  CONSTRAINT `FK_ProjectActions_ProjectPhases`
    FOREIGN KEY (`ProjectPhaseId`)
    REFERENCES `dbo`.`ProjectPhases` (`Id`)
    ON DELETE CASCADE
    ON UPDATE CASCADE);
CREATE TABLE `dbo`.`ProjectLabourImpacts` (
  `Id` BIGINT NOT NULL AUTO_INCREMENT,
  `Name` VARCHAR(200) NOT NULL,
  `Description` VARCHAR(1000) NOT NULL,
  `Type` VARCHAR(45) NOT NULL,
  `Definition` TEXT NOT NULL,
  `ProjectLabourRequirementId` BIGINT NOT NULL,
  PRIMARY KEY (`Id`),
  INDEX `FK_ProjectLabourImpacts_ProjectLabourRequirements_idx` (`ProjectLabourRequirementId` ASC),
  CONSTRAINT `FK_ProjectLabourImpacts_ProjectLabourRequirements`
    FOREIGN KEY (`ProjectLabourRequirementId`)
    REFERENCES `dbo`.`ProjectLabourRequirements` (`Id`)
    ON DELETE CASCADE
    ON UPDATE CASCADE);
