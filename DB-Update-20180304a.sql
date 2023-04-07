ALTER TABLE `dbo`.`BodypartProto` 
ADD COLUMN `ArmourTypeId` BIGINT NULL AFTER `IsVital`,
ADD INDEX `FK_BodypartProto_ArmourTypes_idx` (`ArmourTypeId` ASC);
ALTER TABLE `dbo`.`BodypartProto` 
ADD CONSTRAINT `FK_BodypartProto_ArmourTypes`
  FOREIGN KEY (`ArmourTypeId`)
  REFERENCES `dbo`.`ArmourTypes` (`Id`)
  ON DELETE SET NULL
  ON UPDATE CASCADE;