ALTER TABLE `dbo`.`Races` 
ADD COLUMN `CanEatCorpses` BIT NOT NULL DEFAULT b'0' AFTER `RaceUsesStamina`,
ADD COLUMN `BiteWeight` DOUBLE NOT NULL DEFAULT 1000 AFTER `CanEatCorpses`,
ADD COLUMN `EatCorpseEmoteText` VARCHAR(500) NOT NULL DEFAULT '@ eat|eats {0}$1' AFTER `BiteWeight`,
ADD COLUMN `EatYieldEmoteText` VARCHAR(500) NOT NULL DEFAULT '@ eat|eats {0} from the location.' AFTER `EatCorpseEmoteText`,
ADD COLUMN `CanEatMaterialsOptIn` BIT NOT NULL DEFAULT b'0' AFTER `EatYieldEmoteText`;

CREATE TABLE `dbo`.`RaceEdibleForagableYields` (
  `RaceId` BIGINT NOT NULL,
  `YieldType` VARCHAR(50) NOT NULL,
  `BiteYield` DOUBLE NOT NULL,
  `CaloriesPerYield` DOUBLE NOT NULL,
  `HungerPerYield` DOUBLE NOT NULL,
  `WaterPerYield` DOUBLE NOT NULL,
  `ThirstPerYield` DOUBLE NOT NULL,
  `AlcoholPerYield` DOUBLE NOT NULL,
  `EatEmote` VARCHAR(1000) NOT NULL DEFAULT '@ eat|eats {0} from the location.',
  PRIMARY KEY (`RaceId`, `YieldType`),
  CONSTRAINT `FK_RaceEdibleForagableYields_Races`
    FOREIGN KEY (`RaceId`)
    REFERENCES `dbo`.`Races` (`Id`)
    ON DELETE CASCADE
    ON UPDATE CASCADE);


CREATE TABLE `dbo`.`Races_EdibleMaterials` (
  `RaceId` BIGINT NOT NULL,
  `MaterialId` BIGINT NOT NULL,
  `CaloriesPerKilogram` DOUBLE NOT NULL,
  `HungerPerKilogram` DOUBLE NOT NULL,
  `ThirstPerKilogram` DOUBLE NOT NULL,
  `WaterPerKilogram` DOUBLE NOT NULL,
  `AlcoholPerKilogram` DOUBLE NOT NULL,
  PRIMARY KEY (`RaceId`, `MaterialId`),
  INDEX `FK_Races_EdibleMaterials_Materials_idx` (`MaterialId` ASC),
  CONSTRAINT `FK_Races_EdibleMaterials_Races`
    FOREIGN KEY (`RaceId`)
    REFERENCES `dbo`.`Races` (`Id`)
    ON DELETE CASCADE
    ON UPDATE CASCADE,
  CONSTRAINT `FK_Races_EdibleMaterials_Materials`
    FOREIGN KEY (`MaterialId`)
    REFERENCES `dbo`.`Materials` (`Id`)
    ON DELETE CASCADE
    ON UPDATE CASCADE);
