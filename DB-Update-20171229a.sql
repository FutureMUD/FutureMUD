ALTER TABLE `dbo`.`GameItemProtos` 
ADD COLUMN `MorphGameItemProtoId` BIGINT NULL AFTER `HighPriority`,
ADD COLUMN `MorphTimeSeconds` INT NOT NULL DEFAULT 0 AFTER `MorphGameItemProtoId`,
ADD COLUMN `MorphEmote` VARCHAR(1000) NOT NULL DEFAULT '$0 $?1|morphs into $1|decays into nothing$.' AFTER `MorphTimeSeconds`;
ALTER TABLE `dbo`.`GameItems` 
ADD COLUMN `MorphTimeRemaining` INT NULL AFTER `PositionEmote`;
