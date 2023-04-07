ALTER TABLE `dbo`.`WeatherControllers` 
ADD COLUMN `HighestRecentPrecipitationLevel` INT NOT NULL DEFAULT 0 AFTER `Longitude`,
ADD COLUMN `PeriodsSinceHighestPrecipitation` INT NOT NULL DEFAULT 0 AFTER `HighestRecentPrecipitationLevel`;
