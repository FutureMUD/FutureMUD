CREATE TABLE `dbo`.`Grids` (
  `Id` BIGINT NOT NULL AUTO_INCREMENT,
  `GridType` VARCHAR(45) NOT NULL,
  `Definition` MEDIUMTEXT NOT NULL,
  PRIMARY KEY (`Id`, `GridType`));
  ALTER TABLE `dbo`.`Grids` 
DROP PRIMARY KEY,
ADD PRIMARY KEY (`Id`);
;
