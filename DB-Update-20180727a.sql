CREATE TABLE `dbo`.`WeatherControllers` (
  `Id` BIGINT NOT NULL AUTO_INCREMENT,
  `Name` VARCHAR(500) NOT NULL,
  `FeedClockId` BIGINT NOT NULL,
  `FeedClockTimeZoneId` BIGINT NOT NULL,
  `RegionalClimateId` BIGINT NOT NULL,
  `CurrentWeatherEventId` BIGINT NULL,
  `CurrentSeasonId` BIGINT NOT NULL,
  `ConsecutiveUnchangedPeriods` INT NOT NULL DEFAULT 0,
  `MinutesCounter` INT NOT NULL DEFAULT 0,
  PRIMARY KEY (`Id`),
  INDEX `FK_WeatherControllers_RegionalClimates_idx` (`RegionalClimateId` ASC),
  INDEX `FK_WeatherControllers_Seasons_idx` (`CurrentSeasonId` ASC),
  INDEX `FK_WeatherControllers_Clocks_idx` (`FeedClockId` ASC),
  INDEX `FK_WeatherControllers_TimeZones_idx` (`FeedClockTimeZoneId` ASC),
  INDEX `FK_WeatherControllers_WeatherEvents_idx` (`CurrentWeatherEventId` ASC),
  CONSTRAINT `FK_WeatherControllers_RegionalClimates`
    FOREIGN KEY (`RegionalClimateId`)
    REFERENCES `dbo`.`RegionalClimates` (`Id`)
    ON DELETE RESTRICT
    ON UPDATE CASCADE,
  CONSTRAINT `FK_WeatherControllers_Seasons`
    FOREIGN KEY (`CurrentSeasonId`)
    REFERENCES `dbo`.`Seasons` (`Id`)
    ON DELETE NO ACTION
    ON UPDATE CASCADE,
  CONSTRAINT `FK_WeatherControllers_Clocks`
    FOREIGN KEY (`FeedClockId`)
    REFERENCES `dbo`.`Clocks` (`Id`)
    ON DELETE NO ACTION
    ON UPDATE CASCADE,
  CONSTRAINT `FK_WeatherControllers_TimeZones`
    FOREIGN KEY (`FeedClockTimeZoneId`)
    REFERENCES `dbo`.`Timezones` (`Id`)
    ON DELETE NO ACTION
    ON UPDATE CASCADE,
  CONSTRAINT `FK_WeatherControllers_WeatherEvents`
    FOREIGN KEY (`CurrentWeatherEventId`)
    REFERENCES `dbo`.`WeatherEvents` (`Id`)
    ON DELETE NO ACTION
    ON UPDATE CASCADE);
	CREATE TABLE `dbo`.`Areas` (
  `Id` BIGINT NOT NULL AUTO_INCREMENT,
  `Name` VARCHAR(200) NOT NULL,
  `WeatherControllerId` BIGINT NULL,
  PRIMARY KEY (`Id`),
  INDEX `FK_Areas_WeatherControllers_idx` (`WeatherControllerId` ASC),
  CONSTRAINT `FK_Areas_WeatherControllers`
    FOREIGN KEY (`WeatherControllerId`)
    REFERENCES `dbo`.`WeatherControllers` (`Id`)
    ON DELETE SET NULL
    ON UPDATE CASCADE);
CREATE TABLE `dbo`.`Areas_Rooms` (
  `AreaId` BIGINT NOT NULL,
  `RoomId` BIGINT NOT NULL,
  PRIMARY KEY (`AreaId`, `RoomId`),
  INDEX `FK_Areas_Rooms_Rooms_idx` (`RoomId` ASC),
  CONSTRAINT `FK_Areas_Rooms_Areas`
    FOREIGN KEY (`AreaId`)
    REFERENCES `dbo`.`Areas` (`Id`)
    ON DELETE CASCADE
    ON UPDATE CASCADE,
  CONSTRAINT `FK_Areas_Rooms_Rooms`
    FOREIGN KEY (`RoomId`)
    REFERENCES `dbo`.`Rooms` (`Id`)
    ON DELETE CASCADE
    ON UPDATE CASCADE);
