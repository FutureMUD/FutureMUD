ALTER TABLE `dbo`.`BodypartProto` 
ADD COLUMN `CountAsId` BIGINT NULL AFTER `Description`,
ADD INDEX `FK_BodypartProto_BodypartProto_idx` (`CountAsId` ASC);
;
ALTER TABLE `dbo`.`BodypartProto` 
ADD CONSTRAINT `FK_BodypartProto_BodypartProto`
  FOREIGN KEY (`CountAsId`)
  REFERENCES `dbo`.`BodypartProto` (`Id`)
  ON DELETE SET NULL
  ON UPDATE CASCADE;
