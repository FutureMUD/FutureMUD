CREATE TABLE `dbo`.`BoneOrganCoverages` (
  `BoneId` BIGINT NOT NULL,
  `OrganId` BIGINT NOT NULL,
  `CoverageChance` DOUBLE NOT NULL,
  PRIMARY KEY (`BoneId`, `OrganId`),
  INDEX `FK_BoneOrganCoverages_BodypartProto_Organ_idx` (`OrganId` ASC),
  CONSTRAINT `FK_BoneOrganCoverages_BodypartProto_Bone`
    FOREIGN KEY (`BoneId`)
    REFERENCES `dbo`.`BodypartProto` (`Id`)
    ON DELETE CASCADE
    ON UPDATE CASCADE,
  CONSTRAINT `FK_BoneOrganCoverages_BodypartProto_Organ`
    FOREIGN KEY (`OrganId`)
    REFERENCES `dbo`.`BodypartProto` (`Id`)
    ON DELETE CASCADE
    ON UPDATE CASCADE);

	ALTER TABLE `dbo`.`Crafts` 
DROP COLUMN `PhaseInformation`;
