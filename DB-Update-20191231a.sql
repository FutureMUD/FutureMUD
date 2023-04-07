ALTER TABLE `dbo`.`EconomicZones` 
ADD COLUMN `ReferenceCalendarId` BIGINT NULL AFTER `CurrentFinancialPeriodId`,
ADD INDEX `FK_EconomicZones_Calendars_idx` (`ReferenceCalendarId` ASC);
;
ALTER TABLE `dbo`.`EconomicZones` 
ADD CONSTRAINT `FK_EconomicZones_Calendars`
  FOREIGN KEY (`ReferenceCalendarId`)
  REFERENCES `dbo`.`Calendars` (`Id`)
  ON DELETE SET NULL
  ON UPDATE CASCADE;
