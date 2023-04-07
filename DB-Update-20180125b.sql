CREATE TABLE `dbo`.`BodypartInternalInfos` (
  `BodypartProtoId` BIGINT NOT NULL,
  `InternalPartId` BIGINT NOT NULL,
  `IsPrimaryOrganLocation` BIT NOT NULL DEFAULT b'0',
  `HitChance` DOUBLE NOT NULL DEFAULT 5.0,
  `ProximityGroup` VARCHAR(45) NULL,
  PRIMARY KEY (`BodypartProtoId`, `InternalPartId`),
  INDEX `FK_BodypartInternalInfos_BodypartProtos_Internal_idx` (`InternalPartId` ASC),
  CONSTRAINT `FK_BodypartInternalInfos_BodypartProtos`
    FOREIGN KEY (`BodypartProtoId`)
    REFERENCES `dbo`.`BodypartProto` (`Id`)
    ON DELETE CASCADE
    ON UPDATE CASCADE,
  CONSTRAINT `FK_BodypartInternalInfos_BodypartProtos_Internal`
    FOREIGN KEY (`InternalPartId`)
    REFERENCES `dbo`.`BodypartProto` (`Id`)
    ON DELETE CASCADE
    ON UPDATE CASCADE);
