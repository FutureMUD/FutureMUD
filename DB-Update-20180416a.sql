CREATE TABLE `dbo`.`CharacterIntroTemplates` (
  `Id` BIGINT NOT NULL AUTO_INCREMENT,
  `Name` VARCHAR(200) NOT NULL,
  `ResolutionPriority` INT NOT NULL DEFAULT 1,
  `AppliesToCharacterProgId` BIGINT NOT NULL,
  `Definition` TEXT NOT NULL,
  PRIMARY KEY (`Id`),
  INDEX `FK_CharacterIntroTemplates_FutureProgs_idx` (`AppliesToCharacterProgId` ASC),
  CONSTRAINT `FK_CharacterIntroTemplates_FutureProgs`
    FOREIGN KEY (`AppliesToCharacterProgId`)
    REFERENCES `dbo`.`FutureProgs` (`Id`)
    ON DELETE NO ACTION
    ON UPDATE CASCADE);
	ALTER TABLE `dbo`.`CharacterIntroTemplates` 
ADD COLUMN `Order` INT NOT NULL DEFAULT 0 AFTER `Definition`;

