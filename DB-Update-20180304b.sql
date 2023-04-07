CREATE TABLE `dbo`.`CraftPhases` (
  `CraftPhaseId` BIGINT NOT NULL,
  `CraftPhaseRevisionNumber` INT NOT NULL,
  `PhaseNumber` INT NOT NULL,
  `PhaseLengthInSeconds` DOUBLE NOT NULL,
  `Echo` TEXT NOT NULL,
  `FailEcho` TEXT NULL,
  PRIMARY KEY (`CraftPhaseId`, `CraftPhaseRevisionNumber`, `PhaseNumber`),
  CONSTRAINT `FK_CraftPhases_Crafts`
    FOREIGN KEY (`CraftPhaseId` , `CraftPhaseRevisionNumber`)
    REFERENCES `dbo`.`Crafts` (`Id` , `RevisionNumber`)
    ON DELETE CASCADE
    ON UPDATE CASCADE);
ALTER TABLE `dbo`.`CraftProducts` 
ADD COLUMN `IsFailProduct` BIT NOT NULL DEFAULT b'0' AFTER `OriginalAdditionTime`;
