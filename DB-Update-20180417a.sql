CREATE TABLE `dbo`.`ChargenAdvices` (
  `Id` BIGINT NOT NULL AUTO_INCREMENT,
  `ChargenStage` INT NOT NULL,
  `AdviceTitle` TEXT NOT NULL,
  `AdviceText` TEXT NOT NULL,
  `ShouldShowAdviceProgId` BIGINT NULL,
  PRIMARY KEY (`Id`),
  INDEX `FK_ChargenAdvices_FutureProgs_idx` (`ShouldShowAdviceProgId` ASC),
  CONSTRAINT `FK_ChargenAdvices_FutureProgs`
    FOREIGN KEY (`ShouldShowAdviceProgId`)
    REFERENCES `dbo`.`FutureProgs` (`Id`)
    ON DELETE SET NULL
    ON UPDATE CASCADE);
CREATE TABLE `dbo`.`ChargenAdvices_ChargenRoles` (
  `ChargenAdviceId` BIGINT NOT NULL,
  `ChargenRoleId` BIGINT NOT NULL,
  PRIMARY KEY (`ChargenAdviceId`, `ChargenRoleId`),
  INDEX `FK_ChargenAdvices_ChargenRoles_ChargenRoles_idx` (`ChargenRoleId` ASC),
  CONSTRAINT `FK_ChargenAdvices_ChargenRoles_ChargenRoles`
    FOREIGN KEY (`ChargenRoleId`)
    REFERENCES `dbo`.`ChargenRoles` (`Id`)
    ON DELETE CASCADE
    ON UPDATE CASCADE,
  CONSTRAINT `FK_ChargenAdvices_ChargenRoles_ChargenAdvices`
    FOREIGN KEY (`ChargenAdviceId`)
    REFERENCES `dbo`.`ChargenAdvices` (`Id`)
    ON DELETE CASCADE
    ON UPDATE CASCADE);
CREATE TABLE `dbo`.`ChargenAdvices_Races` (
  `ChargenAdviceId` BIGINT NOT NULL,
  `RaceId` BIGINT NOT NULL,
  PRIMARY KEY (`ChargenAdviceId`, `RaceId`),
  INDEX `FK_ChargenAdvices_Races_Races_idx` (`RaceId` ASC),
  CONSTRAINT `FK_ChargenAdvices_Races_Races`
    FOREIGN KEY (`RaceId`)
    REFERENCES `dbo`.`Races` (`Id`)
    ON DELETE CASCADE
    ON UPDATE CASCADE,
  CONSTRAINT `FK_ChargenAdvices_Races_ChargenAdvices`
    FOREIGN KEY (`ChargenAdviceId`)
    REFERENCES `dbo`.`ChargenAdvices` (`Id`)
    ON DELETE CASCADE
    ON UPDATE CASCADE);
CREATE TABLE `dbo`.`ChargenAdvices_Cultures` (
  `ChargenAdviceId` BIGINT NOT NULL,
  `CultureId` BIGINT NOT NULL,
  PRIMARY KEY (`ChargenAdviceId`, `CultureId`),
  INDEX `FK_ChargenAdvices_Cultures_Cultures_idx` (`CultureId` ASC),
  CONSTRAINT `FK_ChargenAdvices_Cultures_Cultures`
    FOREIGN KEY (`CultureId`)
    REFERENCES `dbo`.`Cultures` (`Id`)
    ON DELETE CASCADE
    ON UPDATE CASCADE,
  CONSTRAINT `FK_ChargenAdvices_Cultures_ChargenAdvices`
    FOREIGN KEY (`ChargenAdviceId`)
    REFERENCES `dbo`.`ChargenAdvices` (`Id`)
    ON DELETE CASCADE
    ON UPDATE CASCADE);
CREATE TABLE `dbo`.`ChargenAdvices_Ethnicities` (
  `ChargenAdviceId` BIGINT NOT NULL,
  `EthnicityId` BIGINT NOT NULL,
  PRIMARY KEY (`ChargenAdviceId`, `EthnicityId`),
  INDEX `FK_ChargenAdvices_Ethnicities_Ethnicities_idx` (`EthnicityId` ASC),
  CONSTRAINT `FK_ChargenAdvices_Ethnicities_ChargenAdvices`
    FOREIGN KEY (`ChargenAdviceId`)
    REFERENCES `dbo`.`ChargenAdvices` (`Id`)
    ON DELETE CASCADE
    ON UPDATE CASCADE,
  CONSTRAINT `FK_ChargenAdvices_Ethnicities_Ethnicities`
    FOREIGN KEY (`EthnicityId`)
    REFERENCES `dbo`.`Ethnicities` (`Id`)
    ON DELETE CASCADE
    ON UPDATE CASCADE);
