ALTER TABLE `dbo`.`BodyProtos` 
ADD COLUMN `CountsAsId` BIGINT NULL AFTER `Name`,
ADD INDEX `FK_BodyPrototype_BodyPrototype_idx` (`CountsAsId` ASC);
;
ALTER TABLE `dbo`.`BodyProtos` 
ADD CONSTRAINT `FK_BodyPrototype_BodyPrototype`
  FOREIGN KEY (`CountsAsId`)
  REFERENCES `dbo`.`BodyProtos` (`Id`)
  ON DELETE SET NULL
  ON UPDATE CASCADE;
