CREATE TABLE `labmud_dbo`.`ButcheryProducts` (
  `Id` BIGINT NOT NULL AUTO_INCREMENT,
  `Name` VARCHAR(100) NOT NULL,
  `TargetBodyId` BIGINT NOT NULL,
  `IsPelt` BIT NOT NULL,
  `Subcategory` VARCHAR(100) NOT NULL,
  `CanProduceProgId` BIGINT NULL,
  PRIMARY KEY (`Id`),
  INDEX `FK_ButcheryProducts_BodyProtos_idx` (`TargetBodyId` ASC),
  INDEX `FK_ButcheryProducts_FutureProgs_idx` (`CanProduceProgId` ASC),
  CONSTRAINT `FK_ButcheryProducts_BodyProtos`
    FOREIGN KEY (`TargetBodyId`)
    REFERENCES `labmud_dbo`.`BodyProtos` (`Id`)
    ON DELETE CASCADE
    ON UPDATE CASCADE,
  CONSTRAINT `FK_ButcheryProducts_FutureProgs`
    FOREIGN KEY (`CanProduceProgId`)
    REFERENCES `labmud_dbo`.`FutureProgs` (`Id`)
    ON DELETE SET NULL
    ON UPDATE CASCADE);

CREATE TABLE `labmud_dbo`.`ButcheryProducts_BodypartProtos` (
  `ButcheryProductId` BIGINT NOT NULL,
  `BodypartProtoId` BIGINT NOT NULL,
  PRIMARY KEY (`ButcheryProductId`, `BodypartProtoId`),
  INDEX `FK_ButcheryProducts_BodypartProtos_BodypartProtos_idx` (`BodypartProtoId` ASC),
  CONSTRAINT `FK_ButcheryProducts_BodypartProtos_ButcheryProducts`
    FOREIGN KEY (`ButcheryProductId`)
    REFERENCES `labmud_dbo`.`ButcheryProducts` (`Id`)
    ON DELETE CASCADE
    ON UPDATE CASCADE,
  CONSTRAINT `FK_ButcheryProducts_BodypartProtos_BodypartProtos`
    FOREIGN KEY (`BodypartProtoId`)
    REFERENCES `labmud_dbo`.`BodypartProto` (`Id`)
    ON DELETE CASCADE
    ON UPDATE CASCADE);

CREATE TABLE `labmud_dbo`.`ButcheryProductItems` (
  `Id` BIGINT NOT NULL AUTO_INCREMENT,
  `ButcheryProductId` BIGINT NOT NULL,
  `NormalProtoId` BIGINT NOT NULL,
  `DamagedProtoId` BIGINT NULL,
  `NormalQuantity` INT NOT NULL DEFAULT 0,
  `DamagedQuantity` INT NOT NULL DEFAULT 0,
  `ButcheryProductItemscol` VARCHAR(45) NULL,
  `DamageThreshold` DOUBLE NOT NULL DEFAULT 10.0,
  PRIMARY KEY (`Id`),
  INDEX `FK_ButcheryProductItems_ButcheryProducts_idx` (`ButcheryProductId` ASC),
  CONSTRAINT `FK_ButcheryProductItems_ButcheryProducts`
    FOREIGN KEY (`ButcheryProductId`)
    REFERENCES `labmud_dbo`.`ButcheryProducts` (`Id`)
    ON DELETE CASCADE
    ON UPDATE CASCADE);

CREATE TABLE `labmud_dbo`.`RaceButcheryProfiles` (
  `Id` BIGINT NOT NULL AUTO_INCREMENT,
  `Name` VARCHAR(100) NOT NULL,
  `Verb` INT NOT NULL,
  `RequiredToolTagId` BIGINT NULL,
  `DifficultySkin` INT NOT NULL,
  `CanButcherProgId` BIGINT NULL,
  `WhyCannotButcherProgId` BIGINT NULL,
  PRIMARY KEY (`Id`),
  INDEX `FK_RaceButcheryProfiles_Tags_idx` (`RequiredToolTagId` ASC),
  INDEX `FK_RaceButcheryProfiles_FutureProgs_Can_idx` (`CanButcherProgId` ASC),
  INDEX `FK_RaceButcheryProfiles_FutureProgs_Why_idx` (`WhyCannotButcherProgId` ASC),
  CONSTRAINT `FK_RaceButcheryProfiles_Tags`
    FOREIGN KEY (`RequiredToolTagId`)
    REFERENCES `labmud_dbo`.`Tags` (`Id`)
    ON DELETE SET NULL
    ON UPDATE CASCADE,
  CONSTRAINT `FK_RaceButcheryProfiles_FutureProgs_Can`
    FOREIGN KEY (`CanButcherProgId`)
    REFERENCES `labmud_dbo`.`FutureProgs` (`Id`)
    ON DELETE SET NULL
    ON UPDATE CASCADE,
  CONSTRAINT `FK_RaceButcheryProfiles_FutureProgs_Why`
    FOREIGN KEY (`WhyCannotButcherProgId`)
    REFERENCES `labmud_dbo`.`FutureProgs` (`Id`)
    ON DELETE SET NULL
    ON UPDATE CASCADE);

CREATE TABLE `labmud_dbo`.`RaceButcheryProfiles_BreakdownChecks` (
  `RaceButcheryProfileId` BIGINT NOT NULL,
  `Subcageory` VARCHAR(100) NOT NULL,
  `TraitDefinitionId` BIGINT NOT NULL,
  `Difficulty` INT NOT NULL,
  PRIMARY KEY (`RaceButcheryProfileId`, `Subcageory`),
  INDEX `FK_RaceButcheryProfiles_BreakdownChecks_TraitDefinitions_idx` (`TraitDefinitionId` ASC),
  CONSTRAINT `FK_RaceButcheryProfiles_BreakdownChecks_RaceButcheryProfiles`
    FOREIGN KEY (`RaceButcheryProfileId`)
    REFERENCES `labmud_dbo`.`RaceButcheryProfiles` (`Id`)
    ON DELETE CASCADE
    ON UPDATE CASCADE,
  CONSTRAINT `FK_RaceButcheryProfiles_BreakdownChecks_TraitDefinitions`
    FOREIGN KEY (`TraitDefinitionId`)
    REFERENCES `labmud_dbo`.`TraitDefinitions` (`Id`)
    ON DELETE RESTRICT
    ON UPDATE CASCADE);

CREATE TABLE `labmud_dbo`.`RaceButcheryProfiles_BreakdownEmotes` (
  `RaceButcheryProfileId` BIGINT NOT NULL,
  `Subcategory` VARCHAR(100) NOT NULL,
  `Emote` TEXT NOT NULL,
  `Delay` DOUBLE NOT NULL,
  `Order` INT NOT NULL,
  PRIMARY KEY (`RaceButcheryProfileId`, `Subcategory`, `Order`),
  CONSTRAINT `FK_RaceButcheryProfiles_BreakdownEmotes_RaceButcheryProfiles`
    FOREIGN KEY (`RaceButcheryProfileId`)
    REFERENCES `labmud_dbo`.`RaceButcheryProfiles` (`Id`)
    ON DELETE CASCADE
    ON UPDATE CASCADE);

CREATE TABLE `labmud_dbo`.`RaceButcheryProfiles_SkinningEmotes` (
  `RaceButcheryProfileId` BIGINT NOT NULL,
  `Subcategory` VARCHAR(100) NOT NULL,
  `Emote` TEXT NOT NULL,
  `Delay` DOUBLE NOT NULL,
  `Order` INT NOT NULL,
  PRIMARY KEY (`RaceButcheryProfileId`, `Subcategory`, `Order`),
  CONSTRAINT `FK_RaceButcheryProfiles_SkinningEmotes_RaceButcheryProfiles`
    FOREIGN KEY (`RaceButcheryProfileId`)
    REFERENCES `labmud_dbo`.`RaceButcheryProfiles` (`Id`)
    ON DELETE CASCADE
    ON UPDATE CASCADE);

CREATE TABLE `labmud_dbo`.`RaceButcheryProfiles_ButcheryProducts` (
  `RaceButcheryProfileId` BIGINT NOT NULL,
  `ButcheryProductId` BIGINT NOT NULL,
  PRIMARY KEY (`RaceButcheryProfileId`, `ButcheryProductId`),
  INDEX `FK_RaceButcheryProfiles_ButcheryProducts_ButcheryProducts_idx` (`ButcheryProductId` ASC),
  CONSTRAINT `FK_RaceButcheryProfiles_ButcheryProducts_RaceButcheryProfiles`
    FOREIGN KEY (`RaceButcheryProfileId`)
    REFERENCES `labmud_dbo`.`RaceButcheryProfiles` (`Id`)
    ON DELETE CASCADE
    ON UPDATE CASCADE,
  CONSTRAINT `FK_RaceButcheryProfiles_ButcheryProducts_ButcheryProducts`
    FOREIGN KEY (`ButcheryProductId`)
    REFERENCES `labmud_dbo`.`ButcheryProducts` (`Id`)
    ON DELETE CASCADE
    ON UPDATE CASCADE);

ALTER TABLE `labmud_dbo`.`Races` 
ADD COLUMN `RaceButcheryProfileId` BIGINT NULL AFTER `MaximumLiftWeightExpression`,
ADD INDEX `FK_Races_RaceButcheryProfiles_idx` (`RaceButcheryProfileId` ASC);
ALTER TABLE `labmud_dbo`.`Races` 
ADD CONSTRAINT `FK_Races_RaceButcheryProfiles`
  FOREIGN KEY (`RaceButcheryProfileId`)
  REFERENCES `labmud_dbo`.`RaceButcheryProfiles` (`Id`)
  ON DELETE SET NULL
  ON UPDATE CASCADE;
