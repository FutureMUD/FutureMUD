CREATE TABLE `dbo`.`Limbs_SpinalParts` (
  `LimbId` BIGINT NOT NULL,
  `BodypartProtoId` BIGINT NOT NULL,
  PRIMARY KEY (`LimbId`, `BodypartProtoId`),
  INDEX `FK_Limbs_SpinalParts_BodypartProtos_idx` (`BodypartProtoId` ASC),
  CONSTRAINT `FK_Limbs_SpinalParts_Limbs`
    FOREIGN KEY (`LimbId`)
    REFERENCES `dbo`.`Limbs` (`Id`)
    ON DELETE CASCADE
    ON UPDATE CASCADE,
  CONSTRAINT `FK_Limbs_SpinalParts_BodypartProtos`
    FOREIGN KEY (`BodypartProtoId`)
    REFERENCES `dbo`.`BodypartProto` (`Id`)
    ON DELETE CASCADE
    ON UPDATE CASCADE);
