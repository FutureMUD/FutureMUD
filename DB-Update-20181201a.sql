ALTER TABLE `dbo`.`Characters` 
ADD COLUMN `Outfits` TEXT NULL AFTER `LastLogoutTime`;
ALTER TABLE `dbo`.`WeatherEvents` 
ADD COLUMN `CountsAsId` BIGINT NULL AFTER `PermittedAtDusk`,
ADD INDEX `FK_WeatherEvents_WeatherEvents_idx` (`CountsAsId` ASC);
ALTER TABLE `dbo`.`WeatherEvents` 
ADD CONSTRAINT `FK_WeatherEvents_WeatherEvents`
  FOREIGN KEY (`CountsAsId`)
  REFERENCES `dbo`.`WeatherEvents` (`Id`)
  ON DELETE SET NULL
  ON UPDATE CASCADE;
  ALTER TABLE `dbo`.`Merits` 
ADD COLUMN `ParentId` BIGINT NULL AFTER `Definition`,
ADD INDEX `FK_Merits_Merits_idx` (`ParentId` ASC);
ALTER TABLE `dbo`.`Merits` 
ADD CONSTRAINT `FK_Merits_Merits`
  FOREIGN KEY (`ParentId`)
  REFERENCES `dbo`.`Merits` (`Id`)
  ON DELETE SET NULL
  ON UPDATE CASCADE;
