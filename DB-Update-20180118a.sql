ALTER TABLE `dbo`.`Clans` 
ADD COLUMN `PaymasterId` BIGINT NULL AFTER `ShowClanMembersInWho`,
ADD COLUMN `PaymasterItemProtoId` BIGINT NULL AFTER `PaymasterId`,
ADD COLUMN `OnPayProgId` BIGINT NULL AFTER `PaymasterItemProtoId`,
ADD COLUMN `MaximumPeriodsOfUncollectedBackPay` INT NULL AFTER `OnPayProgId`,
ADD INDEX `FK_Clans_FutureProgs_idx` (`OnPayProgId` ASC),
ADD INDEX `FK_Clans_Characters_idx` (`PaymasterId` ASC);
ALTER TABLE `dbo`.`Clans` 
ADD CONSTRAINT `FK_Clans_FutureProgs`
  FOREIGN KEY (`OnPayProgId`)
  REFERENCES `dbo`.`FutureProgs` (`Id`)
  ON DELETE SET NULL
  ON UPDATE CASCADE,
ADD CONSTRAINT `FK_Clans_Characters`
  FOREIGN KEY (`PaymasterId`)
  REFERENCES `dbo`.`Characters` (`Id`)
  ON DELETE SET NULL
  ON UPDATE CASCADE;
