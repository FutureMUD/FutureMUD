CREATE TABLE `dbo`.`MagicResources` (
  `Id` BIGINT NOT NULL AUTO_INCREMENT,
  `Name` VARCHAR(200) NOT NULL,
  `Type` VARCHAR(45) NOT NULL,
  `Definition` TEXT NOT NULL,
  `MagicResourceType` INT NOT NULL,
  PRIMARY KEY (`Id`));
CREATE TABLE `dbo`.`Characters_MagicResources` (
  `CharacterId` BIGINT NOT NULL,
  `MagicResourceId` BIGINT NOT NULL,
  `Amount` DOUBLE NOT NULL DEFAULT 0.0,
  PRIMARY KEY (`CharacterId`, `MagicResourceId`),
  INDEX `FK_Characters_MagicResources_MagicResources_idx` (`MagicResourceId` ASC),
  CONSTRAINT `FK_Characters_MagicResources_Characters`
    FOREIGN KEY (`CharacterId`)
    REFERENCES `dbo`.`Characters` (`Id`)
    ON DELETE CASCADE
    ON UPDATE CASCADE,
  CONSTRAINT `FK_Characters_MagicResources_MagicResources`
    FOREIGN KEY (`MagicResourceId`)
    REFERENCES `dbo`.`MagicResources` (`Id`)
    ON DELETE CASCADE
    ON UPDATE CASCADE);
	CREATE TABLE `dbo`.`GameItems_MagicResources` (
  `GameItemId` BIGINT NOT NULL,
  `MagicResourceId` BIGINT NOT NULL,
  `Amount` DOUBLE NOT NULL DEFAULT 0.0,
  PRIMARY KEY (`GameItemId`, `MagicResourceId`),
  INDEX `FK_GameItems_MagicResources_MagicResources_idx` (`MagicResourceId` ASC),
  CONSTRAINT `FK_GameItems_MagicResources_GameItems`
    FOREIGN KEY (`GameItemId`)
    REFERENCES `dbo`.`GameItems` (`Id`)
    ON DELETE CASCADE
    ON UPDATE CASCADE,
  CONSTRAINT `FK_GameItems_MagicResources_MagicResources`
    FOREIGN KEY (`MagicResourceId`)
    REFERENCES `dbo`.`MagicResources` (`Id`)
    ON DELETE CASCADE
    ON UPDATE CASCADE);
CREATE TABLE `dbo`.`Cells_MagicResources` (
  `CellId` BIGINT NOT NULL,
  `MagicResourceId` BIGINT NOT NULL,
  `Amount` DOUBLE NOT NULL DEFAULT 0.0,
  PRIMARY KEY (`CellId`, `MagicResourceId`),
  INDEX `FK_Cells_MagicResources_MagicResources_idx` (`MagicResourceId` ASC),
  CONSTRAINT `FK_Cells_MagicResources_Cells`
    FOREIGN KEY (`CellId`)
    REFERENCES `dbo`.`Cells` (`Id`)
    ON DELETE CASCADE
    ON UPDATE CASCADE,
  CONSTRAINT `FK_Cells_MagicResources_MagicResources`
    FOREIGN KEY (`MagicResourceId`)
    REFERENCES `dbo`.`MagicResources` (`Id`)
    ON DELETE CASCADE
    ON UPDATE CASCADE);
