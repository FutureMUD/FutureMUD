ALTER TABLE `dbo`.`NPCs` 
ADD COLUMN `BodyguardCharacterId` BIGINT NULL AFTER `TemplateRevnum`,
ADD INDEX `FK_NPCs_Characters_Bodyguard_idx` (`BodyguardCharacterId` ASC);
ALTER TABLE `dbo`.`NPCs` 
ADD CONSTRAINT `FK_NPCs_Characters_Bodyguard`
  FOREIGN KEY (`BodyguardCharacterId`)
  REFERENCES `dbo`.`Characters` (`Id`)
  ON DELETE SET NULL
  ON UPDATE CASCADE;
