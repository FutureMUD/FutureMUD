CREATE TABLE `dbo`.`Projects` (
  `Id` BIGINT NOT NULL,
  `Type` VARCHAR(45) NOT NULL,
  `Name` VARCHAR(200) NOT NULL,
  `RevisionNumber` INT NOT NULL,
  `EditableItemId` BIGINT NOT NULL,
  `Definition` TEXT NULL,
  PRIMARY KEY (`Id`, `RevisionNumber`),
  INDEX `FK_Projects_EditableItems_idx` (`EditableItemId` ASC),
  CONSTRAINT `FK_Projects_EditableItems`
    FOREIGN KEY (`EditableItemId`)
    REFERENCES `dbo`.`EditableItems` (`Id`)
    ON DELETE CASCADE
    ON UPDATE CASCADE);
CREATE TABLE `dbo`.`ProjectPhases` (
  `Id` BIGINT NOT NULL AUTO_INCREMENT,
  `ProjectId` BIGINT NOT NULL,
  `ProjectRevisionNumber` INT NOT NULL,
  `PhaseNumber` INT NOT NULL,
  `Description` TEXT NOT NULL,
  PRIMARY KEY (`Id`),
  INDEX `FK_ProjectPhases_Projects_idx` (`ProjectId` ASC, `ProjectRevisionNumber` ASC),
  CONSTRAINT `FK_ProjectPhases_Projects`
    FOREIGN KEY (`ProjectId` , `ProjectRevisionNumber`)
    REFERENCES `dbo`.`Projects` (`Id` , `RevisionNumber`)
    ON DELETE CASCADE
    ON UPDATE CASCADE);
CREATE TABLE `dbo`.`ProjectLabourRequirements` (
  `Id` BIGINT NOT NULL AUTO_INCREMENT,
  `Type` VARCHAR(45) NOT NULL,
  `Name` VARCHAR(200) NOT NULL,
  `ProjectPhaseId` BIGINT NOT NULL,
  `Description` TEXT NOT NULL,
  `TotalProgressRequired` DOUBLE NOT NULL,
  `MaximumSimultaneousWorkers` INT NOT NULL,
  `Definition` TEXT NOT NULL,
  PRIMARY KEY (`Id`),
  INDEX `FK_ProjectLabourRequirements_ProjectPhases_idx` (`ProjectPhaseId` ASC),
  CONSTRAINT `FK_ProjectLabourRequirements_ProjectPhases`
    FOREIGN KEY (`ProjectPhaseId`)
    REFERENCES `dbo`.`ProjectPhases` (`Id`)
    ON DELETE CASCADE
    ON UPDATE CASCADE);
CREATE TABLE `dbo`.`ProjectMaterialRequirements` (
  `Id` BIGINT NOT NULL AUTO_INCREMENT,
  `Type` VARCHAR(45) NOT NULL,
  `ProjectPhaseId` BIGINT NOT NULL,
  `Name` VARCHAR(200) NOT NULL,
  `Description` TEXT NOT NULL,
  `Definition` TEXT NOT NULL,
  `IsMandatoryForProjectCompletion` BIT NOT NULL DEFAULT b'1',
  PRIMARY KEY (`Id`),
  INDEX `FK_ProjectMaterialRequirements_ProjectPhases_idx` (`ProjectPhaseId` ASC),
  CONSTRAINT `FK_ProjectMaterialRequirements_ProjectPhases`
    FOREIGN KEY (`ProjectPhaseId`)
    REFERENCES `dbo`.`ProjectPhases` (`Id`)
    ON DELETE CASCADE
    ON UPDATE CASCADE);
CREATE TABLE `dbo`.`ActiveProjects` (
  `Id` BIGINT NOT NULL AUTO_INCREMENT,
  `ProjectId` BIGINT NOT NULL,
  `ProjectRevisionNumber` INT NOT NULL,
  `CurrentPhaseId` BIGINT NOT NULL,
  `CharacterId` BIGINT NULL,
  `CellId` BIGINT NULL,
  PRIMARY KEY (`Id`),
  INDEX `FK_ActiveProjects_Projects_idx` (`ProjectId` ASC, `ProjectRevisionNumber` ASC),
  INDEX `FK_ActiveProjects_ProjectPhases_idx` (`CurrentPhaseId` ASC),
  INDEX `FK_ActiveProjects_Characters_idx` (`CharacterId` ASC),
  INDEX `FK_ActiveProjects_Cells_idx` (`CellId` ASC),
  CONSTRAINT `FK_ActiveProjects_Projects`
    FOREIGN KEY (`ProjectId` , `ProjectRevisionNumber`)
    REFERENCES `dbo`.`Projects` (`Id` , `RevisionNumber`)
    ON DELETE CASCADE
    ON UPDATE CASCADE,
  CONSTRAINT `FK_ActiveProjects_ProjectPhases`
    FOREIGN KEY (`CurrentPhaseId`)
    REFERENCES `dbo`.`ProjectPhases` (`Id`)
    ON DELETE CASCADE
    ON UPDATE CASCADE,
  CONSTRAINT `FK_ActiveProjects_Characters`
    FOREIGN KEY (`CharacterId`)
    REFERENCES `dbo`.`Characters` (`Id`)
    ON DELETE NO ACTION
    ON UPDATE CASCADE,
  CONSTRAINT `FK_ActiveProjects_Cells`
    FOREIGN KEY (`CellId`)
    REFERENCES `dbo`.`Cells` (`Id`)
    ON DELETE NO ACTION
    ON UPDATE CASCADE);
CREATE TABLE `dbo`.`ActiveProjectLabours` (
  `ActiveProjectId` BIGINT NOT NULL,
  `ProjectLabourRequirementsId` BIGINT NOT NULL,
  `Progress` DOUBLE NOT NULL,
  PRIMARY KEY (`ActiveProjectId`, `ProjectLabourRequirementsId`),
  INDEX `FK_ActiveProjectLabours_ProjectLabourRequirements_idx` (`ProjectLabourRequirementsId` ASC),
  CONSTRAINT `FK_ActiveProjectLabours_ActiveProjects`
    FOREIGN KEY (`ActiveProjectId`)
    REFERENCES `dbo`.`ActiveProjects` (`Id`)
    ON DELETE CASCADE
    ON UPDATE CASCADE,
  CONSTRAINT `FK_ActiveProjectLabours_ProjectLabourRequirements`
    FOREIGN KEY (`ProjectLabourRequirementsId`)
    REFERENCES `dbo`.`ProjectLabourRequirements` (`Id`)
    ON DELETE CASCADE
    ON UPDATE CASCADE);
CREATE TABLE `dbo`.`ActiveProjectMaterials` (
  `ActiveProjectId` BIGINT NOT NULL,
  `ProjectMaterialRequirementsId` BIGINT NOT NULL,
  `Progress` DOUBLE NOT NULL,
  PRIMARY KEY (`ActiveProjectId`, `ProjectMaterialRequirementsId`),
  INDEX `FK_ActiveProjectMaterials_ProjectMaterialRequirements_idx` (`ProjectMaterialRequirementsId` ASC),
  CONSTRAINT `FK_ActiveProjectMaterials_ActiveProjects`
    FOREIGN KEY (`ActiveProjectId`)
    REFERENCES `dbo`.`ActiveProjects` (`Id`)
    ON DELETE CASCADE
    ON UPDATE CASCADE,
  CONSTRAINT `FK_ActiveProjectMaterials_ProjectMaterialRequirements`
    FOREIGN KEY (`ProjectMaterialRequirementsId`)
    REFERENCES `dbo`.`ProjectMaterialRequirements` (`Id`)
    ON DELETE CASCADE
    ON UPDATE CASCADE);
ALTER TABLE `dbo`.`Characters` 
ADD COLUMN `CurrentProjectLabourId` BIGINT NULL AFTER `Outfits`,
ADD COLUMN `CurrentProjectId` BIGINT NULL AFTER `CurrentProjectLabourId`,
ADD INDEX `FK_Characters_ActiveProjects_idx` (`CurrentProjectId` ASC),
ADD INDEX `FK_Characters_ProjectLabourRequirements_idx` (`CurrentProjectLabourId` ASC);
;
ALTER TABLE `dbo`.`Characters` 
ADD CONSTRAINT `FK_Characters_ActiveProjects`
  FOREIGN KEY (`CurrentProjectId`)
  REFERENCES `dbo`.`ActiveProjects` (`Id`)
  ON DELETE SET NULL
  ON UPDATE CASCADE,
ADD CONSTRAINT `FK_Characters_ProjectLabourRequirements`
  FOREIGN KEY (`CurrentProjectLabourId`)
  REFERENCES `dbo`.`ProjectLabourRequirements` (`Id`)
  ON DELETE SET NULL
  ON UPDATE CASCADE;
