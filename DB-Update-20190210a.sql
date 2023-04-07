CREATE TABLE `dbo`.`ShopsTills` (
  `ShopId` BIGINT NOT NULL,
  `GameItemId` BIGINT NOT NULL,
  PRIMARY KEY (`ShopId`, `GameItemId`),
  INDEX `FK_ShopTills_GameItems_idx` (`GameItemId` ASC),
  CONSTRAINT `FK_ShopTills_Shops`
    FOREIGN KEY (`ShopId`)
    REFERENCES `dbo`.`Shops` (`Id`)
    ON DELETE CASCADE
    ON UPDATE CASCADE,
  CONSTRAINT `FK_ShopTills_GameItems`
    FOREIGN KEY (`GameItemId`)
    REFERENCES `dbo`.`GameItems` (`Id`)
    ON DELETE CASCADE
    ON UPDATE CASCADE);
