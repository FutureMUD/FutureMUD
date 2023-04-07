CREATE TABLE `dbo`.`EconomicZones` (
  `Id` BIGINT NOT NULL AUTO_INCREMENT,
  `Name` VARCHAR(200) NOT NULL,
  `CurrentFinancialPeriodStart` DATETIME NOT NULL,
  `CurrentFinancialPeriodStartMUD` VARCHAR(200) NOT NULL,
  `PreviousFinancialPeriodsToKeep` INT NOT NULL DEFAULT 50,
  `ZoneForTimePurposesId` BIGINT NOT NULL,
  `PermitTaxableLosses` BIT(1) NOT NULL DEFAULT b'1',
  `OutstandingTaxesOwed` DECIMAL NOT NULL DEFAULT 0.0,
  `CurrencyId` BIGINT NOT NULL,
  PRIMARY KEY (`Id`),
  INDEX `FK_EconomicZones_Currencies_idx` (`CurrencyId` ASC),
  INDEX `FK_EconomicZones_Zones_idx` (`ZoneForTimePurposesId` ASC),
  CONSTRAINT `FK_EconomicZones_Currencies`
    FOREIGN KEY (`CurrencyId`)
    REFERENCES `dbo`.`Currencies` (`Id`)
    ON DELETE RESTRICT
    ON UPDATE CASCADE,
  CONSTRAINT `FK_EconomicZones_Zones`
    FOREIGN KEY (`ZoneForTimePurposesId`)
    REFERENCES `dbo`.`Zones` (`Id`)
    ON DELETE RESTRICT
    ON UPDATE CASCADE);
CREATE TABLE `dbo`.`FinancialPeriods` (
  `Id` BIGINT NOT NULL AUTO_INCREMENT,
  `EconomicZoneId` BIGINT NOT NULL,
  `PeriodStart` DATETIME NOT NULL,
  `PeriodEnd` DATETIME NOT NULL,
  `MudPeriodStart` VARCHAR(200) NOT NULL,
  `MudPeriodEnd` VARCHAR(200) NOT NULL,
  PRIMARY KEY (`Id`),
  INDEX `FK_FinancialPeriods_EconomicZones_idx` (`EconomicZoneId` ASC),
  CONSTRAINT `FK_FinancialPeriods_EconomicZones`
    FOREIGN KEY (`EconomicZoneId`)
    REFERENCES `dbo`.`EconomicZones` (`Id`)
    ON DELETE CASCADE
    ON UPDATE CASCADE);

CREATE TABLE `dbo`.`EconomicZoneRevenues` (
  `EconomicZoneId` BIGINT NOT NULL,
  `FinancialPeriodStart` DATETIME NOT NULL,
  `TotalTaxRevenue` DECIMAL NOT NULL,
  PRIMARY KEY (`EconomicZoneId`, `FinancialPeriodStart`),
  CONSTRAINT `FK_EconomicZoneRevenues`
    FOREIGN KEY (`EconomicZoneId`)
    REFERENCES `dbo`.`EconomicZones` (`Id`)
    ON DELETE CASCADE
    ON UPDATE CASCADE,
CREATE TABLE `dbo`.`Shops` (
  `Id` BIGINT NOT NULL AUTO_INCREMENT,
  `Name` VARCHAR(200) NOT NULL,
  `WorkshopCellId` BIGINT NULL,
  `StockroomCellId` BIGINT NULL,
  `CanShopProgId` BIGINT NULL,
  `WhyCannotShopProgId` BIGINT NULL,
  `CurrencyId` BIGINT NOT NULL,
  `IsTrading` BIT(1) NOT NULL,
  PRIMARY KEY (`Id`),
  INDEX `FK_Shops_Cells_Workshop_idx` (`WorkshopCellId` ASC),
  INDEX `FK_Shops_Cells_Stockroom_idx` (`StockroomCellId` ASC),
  INDEX `FK_Shops_FutureProgs_Can_idx` (`CanShopProgId` ASC),
  INDEX `FK_Shops_FutureProgs_WhyCant_idx` (`WhyCannotShopProgId` ASC),
  INDEX `FK_Shops_Currencies_idx` (`CurrencyId` ASC),
  CONSTRAINT `FK_Shops_Cells_Workshop`
    FOREIGN KEY (`WorkshopCellId`)
    REFERENCES `dbo`.`Cells` (`Id`)
    ON DELETE SET NULL
    ON UPDATE CASCADE,
  CONSTRAINT `FK_Shops_Cells_Stockroom`
    FOREIGN KEY (`StockroomCellId`)
    REFERENCES `dbo`.`Cells` (`Id`)
    ON DELETE SET NULL
    ON UPDATE CASCADE,
  CONSTRAINT `FK_Shops_FutureProgs_Can`
    FOREIGN KEY (`CanShopProgId`)
    REFERENCES `dbo`.`FutureProgs` (`Id`)
    ON DELETE SET NULL
    ON UPDATE CASCADE,
  CONSTRAINT `FK_Shops_FutureProgs_WhyCant`
    FOREIGN KEY (`WhyCannotShopProgId`)
    REFERENCES `dbo`.`FutureProgs` (`Id`)
    ON DELETE SET NULL
    ON UPDATE CASCADE,
  CONSTRAINT `FK_Shops_Currencies`
    FOREIGN KEY (`CurrencyId`)
    REFERENCES `dbo`.`Currencies` (`Id`)
    ON DELETE RESTRICT
    ON UPDATE CASCADE);

	CREATE TABLE `dbo`.`EconomicZoneShopTaxes` (
  `EconomicZoneId` BIGINT NOT NULL,
  `ShopId` BIGINT NOT NULL,
  `OutstandingProfitTaxes` DECIMAL NOT NULL,
  `OutstandingSalesTaxes` DECIMAL NOT NULL,
  `TaxesInCredits` DECIMAL NOT NULL,
  PRIMARY KEY (`EconomicZoneId`, `ShopId`),
  INDEX `FK_EconomicZoneShopTaxes_Shops_idx` (`ShopId` ASC),
  CONSTRAINT `FK_EconomicZoneShopTaxes_EconomicZones`
    FOREIGN KEY (`EconomicZoneId`)
    REFERENCES `dbo`.`EconomicZones` (`Id`)
    ON DELETE CASCADE
    ON UPDATE CASCADE,
  CONSTRAINT `FK_EconomicZoneShopTaxes_Shops`
    FOREIGN KEY (`ShopId`)
    REFERENCES `dbo`.`Shops` (`Id`)
    ON DELETE CASCADE
    ON UPDATE CASCADE);
CREATE TABLE `dbo`.`ShopFinancialPeriodResults` (
  `EconomicZoneId` BIGINT(20) NOT NULL,
  `ShopId` BIGINT NOT NULL,
  `FinancialPeriodStart` DATETIME NOT NULL,
  `GrossRevenue` DECIMAL NOT NULL,
  `NetRevenue` DECIMAL NOT NULL,
  `SalesTax` DECIMAL NOT NULL,
  `ProfitsTax` DECIMAL NOT NULL,
  PRIMARY KEY (`EconomicZoneId`, `ShopId`, `FinancialPeriodStart`),
  INDEX `FK_ShopFinancialPeriodResults_Shops_idx` (`ShopId` ASC),
  CONSTRAINT `FK_ShopFinancialPeriodResults_EconomicZones`
    FOREIGN KEY (`EconomicZoneId`)
    REFERENCES `dbo`.`EconomicZones` (`Id`)
    ON DELETE CASCADE
    ON UPDATE CASCADE,
  CONSTRAINT `FK_ShopFinancialPeriodResults_Shops`
    FOREIGN KEY (`ShopId`)
    REFERENCES `dbo`.`Shops` (`Id`)
    ON DELETE CASCADE
    ON UPDATE CASCADE);

	ALTER TABLE `dbo`.`ShopFinancialPeriodResults` 
CHANGE COLUMN `FinancialPeriodStart` `FinancialPeriodId` BIGINT NOT NULL ,
ADD INDEX `FK_ShopFinancialPeriodResults_FinancialPeriods_idx` (`FinancialPeriodId` ASC);
ALTER TABLE `dbo`.`ShopFinancialPeriodResults` 
ADD CONSTRAINT `FK_ShopFinancialPeriodResults_FinancialPeriods`
  FOREIGN KEY (`FinancialPeriodId`)
  REFERENCES `dbo`.`FinancialPeriods` (`Id`)
  ON DELETE CASCADE
  ON UPDATE CASCADE;

  ALTER TABLE `dbo`.`EconomicZoneRevenues` 
CHANGE COLUMN `FinancialPeriodStart` `FinancialPeriodId` BIGINT NOT NULL ,
ADD INDEX `FK_EconomicZoneRevenues_FinancialPeriods_idx` (`FinancialPeriodId` ASC);
ALTER TABLE `dbo`.`EconomicZoneRevenues` 
ADD CONSTRAINT `FK_EconomicZoneRevenues_FinancialPeriods`
  FOREIGN KEY (`FinancialPeriodId`)
  REFERENCES `dbo`.`FinancialPeriods` (`Id`)
  ON DELETE CASCADE
  ON UPDATE CASCADE;

  CREATE TABLE `dbo`.`Merchandises` (
  `Id` BIGINT NOT NULL AUTO_INCREMENT,
  `ShopId` BIGINT NOT NULL,
  `AutoReordering` BIT NOT NULL,
  `AutoReorderPrice` DECIMAL NOT NULL,
  `BasePrice` DECIMAL NOT NULL,
  `DefaultMerchandiseForItem` BIT NOT NULL,
  `ItemProtoId` BIGINT NOT NULL,
  `PreferredDisplayContainerId` BIGINT NOT NULL,
  `ListDescription` VARCHAR(500) NULL,
  `MinimumStockLevels` INT NOT NULL,
  `MinimumStockLevelsByWeight` DOUBLE NOT NULL,
  `PreserveVariablesOnReorder` BIT NOT NULL,
  `TagsForMerchandise` VARCHAR(1000) NOT NULL,
  PRIMARY KEY (`Id`),
  INDEX `FK_Merchandises_Shops_idx` (`ShopId` ASC),
  INDEX `FK_Merchandises_GameItems_idx` (`PreferredDisplayContainerId` ASC),
  CONSTRAINT `FK_Merchandises_Shops`
    FOREIGN KEY (`ShopId`)
    REFERENCES `dbo`.`Shops` (`Id`)
    ON DELETE CASCADE
    ON UPDATE CASCADE,
  CONSTRAINT `FK_Merchandises_GameItems`
    FOREIGN KEY (`PreferredDisplayContainerId`)
    REFERENCES `dbo`.`GameItems` (`Id`)
    ON DELETE CASCADE
    ON UPDATE CASCADE);

	ALTER TABLE `dbo`.`Shops` 
ADD COLUMN `EconomicZoneId` BIGINT NOT NULL AFTER `IsTrading`,
ADD INDEX `FK_Shops_EconomicZonesa_idx` (`EconomicZoneId` ASC);
ALTER TABLE `dbo`.`Shops` 
ADD CONSTRAINT `FK_Shops_EconomicZones`
  FOREIGN KEY (`EconomicZoneId`)
  REFERENCES `dbo`.`EconomicZones` (`Id`)
  ON DELETE CASCADE
  ON UPDATE CASCADE;

  ALTER TABLE `dbo`.`Shops` 
ADD COLUMN `EmployeeRecords` MEDIUMTEXT NOT NULL AFTER `EconomicZoneId`;

CREATE TABLE `dbo`.`ShopTransactionRecords` (
  `Id` BIGINT NOT NULL AUTO_INCREMENT,
  `CurrencyId` BIGINT NOT NULL,
  `PretaxValue` DECIMAL NOT NULL,
  `Tax` DECIMAL NOT NULL,
  `TransactionType` INT NOT NULL,
  `ShopId` BIGINT NOT NULL,
  `ThirdPartyId` BIGINT NULL,
  `RealDateTime` DATETIME NOT NULL,
  `MudDateTime` VARCHAR(500) NOT NULL,
  PRIMARY KEY (`Id`),
  INDEX `FK_ShopTransactionRecords_Shops_idx` (`ShopId` ASC),
  INDEX `FK_ShopTransactionRecords_Currencies_idx` (`CurrencyId` ASC),
  CONSTRAINT `FK_ShopTransactionRecords_Shops`
    FOREIGN KEY (`ShopId`)
    REFERENCES `dbo`.`Shops` (`Id`)
    ON DELETE CASCADE
    ON UPDATE CASCADE,
  CONSTRAINT `FK_ShopTransactionRecords_Currencies`
    FOREIGN KEY (`CurrencyId`)
    REFERENCES `dbo`.`Currencies` (`Id`)
    ON DELETE CASCADE
    ON UPDATE CASCADE);

	CREATE TABLE `dbo`.`Shops_StoreroomCells` (
  `ShopId` BIGINT NOT NULL,
  `CellId` BIGINT NOT NULL,
  PRIMARY KEY (`ShopId`, `CellId`),
  INDEX `FK_Shops_StoreroomCells_Cells_idx` (`CellId` ASC),
  CONSTRAINT `FK_Shops_StoreroomCells_Shops`
    FOREIGN KEY (`ShopId`)
    REFERENCES `dbo`.`Shops` (`Id`)
    ON DELETE CASCADE
    ON UPDATE CASCADE,
  CONSTRAINT `FK_Shops_StoreroomCells_Cells`
    FOREIGN KEY (`CellId`)
    REFERENCES `dbo`.`Cells` (`Id`)
    ON DELETE CASCADE
    ON UPDATE CASCADE);
