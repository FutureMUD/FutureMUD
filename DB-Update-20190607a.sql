ALTER TABLE `dbo`.`CharacterCombatSettings` 
ADD COLUMN `MeleeAttackOrderPreference` VARCHAR(100) NOT NULL DEFAULT '0 1 2 3 4' AFTER `RequiredMinimumAim`;

ALTER TABLE `dbo`.`Bodies` 
ADD COLUMN `ShortDescription` VARCHAR(1000) NULL AFTER `BloodtypeId`,
ADD COLUMN `FullDescription` VARCHAR(4000) NULL AFTER `ShortDescription`;
ALTER TABLE `dbo`.`GameItemProtos` 
ADD COLUMN `ShortDescription` VARCHAR(1000) NULL AFTER `MorphEmote`,
ADD COLUMN `FullDescription` VARCHAR(4000) NULL AFTER `ShortDescription`;

ALTER TABLE `dbo`.`Bodies` 
ADD COLUMN `ShortDescriptionPatternId` BIGINT NULL AFTER `FullDescription`,
ADD COLUMN `FullDescriptionPatternId` BIGINT NULL AFTER `ShortDescriptionPatternId`,
ADD INDEX `FK_Bodies_EntityDescriptionPatterns_Short_idx` (`ShortDescriptionPatternId` ASC),
ADD INDEX `FK_Bodies_EntityDescriptionPatterns_Full_idx` (`FullDescriptionPatternId` ASC);
;
ALTER TABLE `dbo`.`Bodies` 
ADD CONSTRAINT `FK_Bodies_EntityDescriptionPatterns_Short`
  FOREIGN KEY (`ShortDescriptionPatternId`)
  REFERENCES `dbo`.`EntityDescriptionPatterns` (`Id`)
  ON DELETE RESTRICT
  ON UPDATE CASCADE,
ADD CONSTRAINT `FK_Bodies_EntityDescriptionPatterns_Full`
  FOREIGN KEY (`FullDescriptionPatternId`)
  REFERENCES `dbo`.`EntityDescriptionPatterns` (`Id`)
  ON DELETE RESTRICT
  ON UPDATE CASCADE;

  ALTER TABLE `dbo`.`Bodies` 
ADD COLUMN `Gender` SMALLINT NOT NULL DEFAULT 0 AFTER `BloodtypeId`;


SET SQL_SAFE_UPDATES = 0;
update dbo.Bodies b
inner join dbo.EntityDescriptions e on b.EntityDescriptionId = e.Id
set b.ShortDescription = e.ShortDescription, b.FullDescription = e.FullDescription;

update dbo.Bodies b
inner join dbo.EntityDescriptions e on b.EntityDescriptionId = e.Id
inner join dbo.EntityDescriptionPatterns_EntityDescriptions pe on pe.EntityDescriptionId = e.Id
inner join dbo.EntityDescriptionPatterns p on pe.PatternId = p.Id and p.Type = 0 
set b.ShortDescriptionPatternId = p.Id;

update dbo.Bodies b
inner join dbo.EntityDescriptions e on b.EntityDescriptionId = e.Id
set b.Gender = e.DisplaySex;

update dbo.Bodies b
inner join dbo.EntityDescriptions e on b.EntityDescriptionId = e.Id
inner join dbo.EntityDescriptionPatterns_EntityDescriptions pe on pe.EntityDescriptionId = e.Id
inner join dbo.EntityDescriptionPatterns p on pe.PatternId = p.Id and p.Type = 1 
set b.FullDescriptionPatternId = p.Id;

update dbo.GameItemProtos b
inner join dbo.EntityDescriptions e on b.EntityDescriptionId = e.Id
set b.ShortDescription = e.ShortDescription, b.FullDescription = e.FullDescription;
SET SQL_SAFE_UPDATES = 1;

ALTER TABLE `dbo`.`Bodies` 
DROP FOREIGN KEY `FK_Bodies_EntityDescriptions`;
ALTER TABLE `dbo`.`Bodies` 
DROP COLUMN `EntityDescriptionID`,
DROP INDEX `FK_Bodies_EntityDescriptions` ;
;

ALTER TABLE `dbo`.`GameItemProtos` 
DROP FOREIGN KEY `FK_GameItemProtos_EntityDescriptions`;
ALTER TABLE `dbo`.`GameItemProtos` 
DROP COLUMN `EntityDescriptionId`,
DROP INDEX `FK_GameItemProtos_EntityDescriptions` ;
;

ALTER TABLE `dbo`.`BodyProtos` 
DROP FOREIGN KEY `FK_BodyPrototype_EntityDescriptions`;
ALTER TABLE `dbo`.`BodyProtos` 
DROP COLUMN `EntityDescriptionId`,
DROP INDEX `FK_BodyPrototype_EntityDescriptions` ;
;
