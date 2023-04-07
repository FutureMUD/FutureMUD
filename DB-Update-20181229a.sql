CREATE TABLE `dbo`.`Cells_Tags` (
  `CellId` BIGINT NOT NULL,
  `TagId` BIGINT NOT NULL,
  PRIMARY KEY (`CellId`, `TagId`),
  INDEX `FK_Cells_Tags_Tags_idx` (`TagId` ASC),
  CONSTRAINT `FK_Cells_Tags_Cells`
    FOREIGN KEY (`CellId`)
    REFERENCES `dbo`.`Cells` (`Id`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION,
  CONSTRAINT `FK_Cells_Tags_Tags`
    FOREIGN KEY (`TagId`)
    REFERENCES `dbo`.`Tags` (`Id`)
    ON DELETE CASCADE
    ON UPDATE CASCADE);
