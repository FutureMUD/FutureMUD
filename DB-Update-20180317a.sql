ALTER TABLE `dbo`.`Crafts` 
DROP FOREIGN KEY `FK_Crafts_FutureProgs_AppearInCraftsListProg`,
DROP FOREIGN KEY `FK_Crafts_TraitDefinitions`;
ALTER TABLE `dbo`.`Crafts` 
CHANGE COLUMN `CheckTraitId` `CheckTraitId` BIGINT(20) NULL ,
CHANGE COLUMN `AppearInCraftsListProgId` `AppearInCraftsListProgId` BIGINT(20) NULL ;
ALTER TABLE `dbo`.`Crafts` 
ADD CONSTRAINT `FK_Crafts_FutureProgs_AppearInCraftsListProg`
  FOREIGN KEY (`AppearInCraftsListProgId`)
  REFERENCES `dbo`.`FutureProgs` (`Id`)
  ON UPDATE CASCADE,
ADD CONSTRAINT `FK_Crafts_TraitDefinitions`
  FOREIGN KEY (`CheckTraitId`)
  REFERENCES `dbo`.`TraitDefinitions` (`Id`)
  ON DELETE NO ACTION
  ON UPDATE CASCADE;
