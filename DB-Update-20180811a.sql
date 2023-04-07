ALTER TABLE `dbo`.`Seasons` 
ADD COLUMN `CelestialDayOnset` INT NOT NULL AFTER `Description`,
ADD COLUMN `CelestialId` BIGINT NOT NULL AFTER `CelestialDayOnset`,
ADD INDEX `FK_Seasons_Celestials_idx` (`CelestialId` ASC);
ALTER TABLE `dbo`.`Seasons` 
ADD CONSTRAINT `FK_Seasons_Celestials`
  FOREIGN KEY (`CelestialId`)
  REFERENCES `dbo`.`Celestials` (`Id`)
  ON DELETE RESTRICT
  ON UPDATE CASCADE;
  ALTER TABLE `dbo`.`ClimateModels` 
ADD COLUMN `MinuteProcessingInterval` INT NOT NULL AFTER `Definition`,
ADD COLUMN `MinimumMinutesBetweenFlavourEchoes` INT NOT NULL AFTER `MinuteProcessingInterval`,
ADD COLUMN `MinuteFlavourEchoChance` DOUBLE NOT NULL AFTER `MinimumMinutesBetweenFlavourEchoes`;
ALTER TABLE `dbo`.`WeatherControllers` 
DROP FOREIGN KEY `FK_WeatherControllers_WeatherEvents`;

ALTER TABLE `dbo`.`WeatherControllers` 
CHANGE COLUMN `CurrentWeatherEventId` `CurrentWeatherEventId` BIGINT(20) NOT NULL ,
ADD COLUMN `CelestialId` BIGINT NULL AFTER `MinutesCounter`,
ADD COLUMN `Elevation` DOUBLE NOT NULL AFTER `CelestialId`,
ADD COLUMN `Radius` DOUBLE NOT NULL AFTER `Elevation`,
ADD COLUMN `Latitude` DOUBLE NOT NULL AFTER `Radius`,
ADD COLUMN `Longitude` DOUBLE NOT NULL AFTER `Latitude`,
ADD INDEX `FK_WeatherControllers_Celestials_idx` (`CelestialId` ASC);
ALTER TABLE `dbo`.`WeatherControllers` 
ADD CONSTRAINT `FK_WeatherControllers_WeatherEvents`
  FOREIGN KEY (`CurrentWeatherEventId`)
  REFERENCES `dbo`.`WeatherEvents` (`Id`)
  ON DELETE NO ACTION
  ON UPDATE CASCADE,
ADD CONSTRAINT `FK_WeatherControllers_Celestials`
  FOREIGN KEY (`CelestialId`)
  REFERENCES `dbo`.`Celestials` (`Id`)
  ON DELETE RESTRICT
  ON UPDATE CASCADE;

ALTER TABLE `dbo`.`WeatherEvents` 
ADD COLUMN `PrecipitationTemperatureEffect` DOUBLE NOT NULL AFTER `AdditionalInfo`,
ADD COLUMN `WindTemperatureEffect` DOUBLE NOT NULL AFTER `PrecipitationTemperatureEffect`,
ADD COLUMN `LightLevelMultiplier` DOUBLE NOT NULL AFTER `WindTemperatureEffect`,
ADD COLUMN `ObscuresViewOfSky` BIT(1) NOT NULL AFTER `LightLevelMultiplier`,
ADD COLUMN `PermittedAtNight` BIT(1) NOT NULL AFTER `ObscuresViewOfSky`,
ADD COLUMN `PermittedAtDawn` BIT(1) NOT NULL AFTER `PermittedAtNight`,
ADD COLUMN `PermittedAtMorning` BIT(1) NOT NULL AFTER `PermittedAtDawn`,
ADD COLUMN `PermittedAtAfternoon` BIT(1) NOT NULL AFTER `PermittedAtMorning`,
ADD COLUMN `PermittedAtDusk` BIT(1) NOT NULL AFTER `PermittedAtAfternoon`;

ALTER TABLE `dbo`.`Zones` 
ADD COLUMN `WeatherControllerId` BIGINT NULL AFTER `ForagableProfileId`,
ADD INDEX `FK_Zones_WeatherControllers_idx` (`WeatherControllerId` ASC);
ALTER TABLE `dbo`.`Zones` 
ADD CONSTRAINT `FK_Zones_WeatherControllers`
  FOREIGN KEY (`WeatherControllerId`)
  REFERENCES `dbo`.`WeatherControllers` (`Id`)
  ON DELETE RESTRICT
  ON UPDATE CASCADE;

  ALTER TABLE `dbo`.`ClimateModels` 
ADD COLUMN `Type` VARCHAR(45) NOT NULL AFTER `Name`;

ALTER TABLE `dbo`.`Terrains` 
ADD COLUMN `WeatherControllerId` BIGINT NULL AFTER `TerrainEditorColour`,
ADD INDEX `FK_Terrains_WeatherControllers_idx` (`WeatherControllerId` ASC);
ALTER TABLE `dbo`.`Terrains` 
ADD CONSTRAINT `FK_Terrains_WeatherControllers`
  FOREIGN KEY (`WeatherControllerId`)
  REFERENCES `dbo`.`WeatherControllers` (`Id`)
  ON DELETE SET NULL
  ON UPDATE CASCADE;

  ALTER TABLE `dbo`.`Races` 
ADD COLUMN `TemperatureRangeFloor` DOUBLE NOT NULL DEFAULT 0.0 AFTER `CanEatMaterialsOptIn`,
ADD COLUMN `TemperatureRangeCeiling` DOUBLE NOT NULL DEFAULT 40.0 AFTER `TemperatureRangeFloor`;

ALTER TABLE `dbo`.`Cultures` 
ADD COLUMN `TolerableTemperatureFloorEffect` DOUBLE NOT NULL DEFAULT 0.0 AFTER `AvailabilityProgId`,
ADD COLUMN `TolerableTemperatureCeilingEffect` DOUBLE NOT NULL DEFAULT 0.0 AFTER `TolerableTemperatureFloorEffect`;

ALTER TABLE `dbo`.`Ethnicities` 
ADD COLUMN `TolerableTemperatureFloorEffect` DOUBLE NOT NULL DEFAULT 0.0 AFTER `PopulationBloodModelId`,
ADD COLUMN `TolerableTemperatureCeilingEffect` DOUBLE NOT NULL DEFAULT 0.0 AFTER `TolerableTemperatureFloorEffect`;
