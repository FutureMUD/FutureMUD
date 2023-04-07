ALTER TABLE `dbo`.`UnitOfMeasure` 
ADD COLUMN `PrimaryAbbreviation` VARCHAR(45) NULL AFTER `Name`;
ALTER TABLE `dbo`.`Tags` 
ADD COLUMN `ShouldSeeProgId` BIGINT NULL AFTER `ParentId`,
ADD INDEX `FK_Tags_Futureprogs_idx` (`ShouldSeeProgId` ASC);
ALTER TABLE `dbo`.`Tags` 
ADD CONSTRAINT `FK_Tags_Futureprogs`
  FOREIGN KEY (`ShouldSeeProgId`)
  REFERENCES `dbo`.`FutureProgs` (`Id`)
  ON DELETE SET NULL
  ON UPDATE CASCADE;
CREATE TABLE `dbo`.`Materials_Tags` (
  `MaterialId` BIGINT NOT NULL,
  `TagId` BIGINT NOT NULL,
  PRIMARY KEY (`MaterialId`, `TagId`),
  INDEX `Materials_Tags_Tags_idx` (`TagId` ASC),
  CONSTRAINT `Materials_Tags_Materials`
    FOREIGN KEY (`MaterialId`)
    REFERENCES `dbo`.`Materials` (`Id`)
    ON DELETE CASCADE
    ON UPDATE CASCADE,
  CONSTRAINT `Materials_Tags_Tags`
    FOREIGN KEY (`TagId`)
    REFERENCES `dbo`.`Tags` (`Id`)
    ON DELETE CASCADE
    ON UPDATE CASCADE);
CREATE TABLE `dbo`.`Liquids_Tags` (
  `LiquidId` BIGINT NOT NULL,
  `TagId` BIGINT NOT NULL,
  PRIMARY KEY (`LiquidId`, `TagId`),
  INDEX `FK_Liquids_Tags_Tags_idx` (`TagId` ASC),
  CONSTRAINT `FK_Liquids_Tags_Liquids`
    FOREIGN KEY (`LiquidId`)
    REFERENCES `dbo`.`Liquids` (`Id`)
    ON DELETE CASCADE
    ON UPDATE CASCADE,
  CONSTRAINT `FK_Liquids_Tags_Tags`
    FOREIGN KEY (`TagId`)
    REFERENCES `dbo`.`Tags` (`Id`)
    ON DELETE CASCADE
    ON UPDATE CASCADE);
CREATE TABLE `dbo`.`Gases_Tags` (
  `GasId` BIGINT NOT NULL,
  `TagId` BIGINT NOT NULL,
  PRIMARY KEY (`GasId`, `TagId`),
  INDEX `FK_Gases_Tags_Tags_idx` (`TagId` ASC),
  CONSTRAINT `FK_Gases_Tags_Gases`
    FOREIGN KEY (`GasId`)
    REFERENCES `dbo`.`Gases` (`Id`)
    ON DELETE CASCADE
    ON UPDATE CASCADE,
  CONSTRAINT `FK_Gases_Tags_Tags`
    FOREIGN KEY (`TagId`)
    REFERENCES `dbo`.`Tags` (`Id`)
    ON DELETE CASCADE
    ON UPDATE CASCADE);
