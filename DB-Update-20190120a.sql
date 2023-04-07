ALTER TABLE `dbo`.`Merchandises` 
DROP FOREIGN KEY `FK_Merchandises_GameItems`;
ALTER TABLE `dbo`.`Merchandises` 
CHANGE COLUMN `PreferredDisplayContainerId` `PreferredDisplayContainerId` BIGINT(20) NULL ;
ALTER TABLE `dbo`.`Merchandises` 
ADD CONSTRAINT `FK_Merchandises_GameItems`
  FOREIGN KEY (`PreferredDisplayContainerId`)
  REFERENCES `dbo`.`GameItems` (`Id`)
  ON DELETE CASCADE
  ON UPDATE CASCADE;
ALTER TABLE `dbo`.`Merchandises` 
ADD COLUMN `Name` VARCHAR(1000) NOT NULL AFTER `Id`;
ALTER TABLE `dbo`.`Merchandises` 
DROP COLUMN `TagsForMerchandise`;
ALTER TABLE `dbo`.`EconomicZones` 
ADD COLUMN `CurrentFinancialPeriodId` BIGINT NULL AFTER `CurrencyId`,
ADD INDEX `FK_EconomicZones_FinancialPeriods_idx` (`CurrentFinancialPeriodId` ASC);
ALTER TABLE `dbo`.`EconomicZones` 
ADD CONSTRAINT `FK_EconomicZones_FinancialPeriods`
  FOREIGN KEY (`CurrentFinancialPeriodId`)
  REFERENCES `dbo`.`FinancialPeriods` (`Id`)
  ON DELETE SET NULL
  ON UPDATE CASCADE;
ALTER TABLE `dbo`.`EconomicZones` 
ADD COLUMN `ReferenceTimezoneId` BIGINT NOT NULL AFTER `CurrentFinancialPeriodId`,
ADD COLUMN `ReferenceTime` VARCHAR(100) NOT NULL AFTER `ReferenceTimezoneId`,
ADD COLUMN `IntervalType` INT NOT NULL DEFAULT 2 AFTER `ReferenceTime`,
ADD COLUMN `IntervalModifier` INT NOT NULL DEFAULT 0 AFTER `IntervalType`,
ADD COLUMN `IntervalAmount` INT NOT NULL DEFAULT 1 AFTER `IntervalModifier`,
ADD INDEX `FK_EconomicZones_Timezones_idx` (`ReferenceTimezoneId` ASC);
ALTER TABLE `dbo`.`EconomicZones` 
ADD CONSTRAINT `FK_EconomicZones_Timezones`
  FOREIGN KEY (`ReferenceTimezoneId`)
  REFERENCES `dbo`.`Timezones` (`Id`)
  ON DELETE RESTRICT
  ON UPDATE CASCADE;
ALTER TABLE `dbo`.`EconomicZones` 
DROP FOREIGN KEY `FK_EconomicZones_Timezones`,
DROP FOREIGN KEY `FK_EconomicZones_Zones`;
ALTER TABLE `dbo`.`EconomicZones` 
CHANGE COLUMN `ReferenceTimezoneId` `ReferenceClockId` BIGINT(20) NOT NULL ,
DROP INDEX `FK_EconomicZones_Zones_idx` ;
ALTER TABLE `dbo`.`EconomicZones` 
ADD CONSTRAINT `FK_EconomicZones_Timezones`
  FOREIGN KEY (`ReferenceClockId`)
  REFERENCES `dbo`.`Timezones` (`Id`)
  ON UPDATE CASCADE,
ADD CONSTRAINT `FK_EconomicZones_Clocks`
  FOREIGN KEY (`ReferenceClockId`)
  REFERENCES `dbo`.`Clocks` (`Id`)
  ON UPDATE CASCADE;
