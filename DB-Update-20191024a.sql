ALTER TABLE `dbo`.`Characters` 
ADD COLUMN `EffectData` MEDIUMTEXT NOT NULL AFTER `CultureId`;
ALTER TABLE `dbo`.`Bodies` 
ADD COLUMN `EffectData` MEDIUMTEXT NOT NULL AFTER `HeldBreathLength`;
ALTER TABLE `dbo`.`GameItems` 
ADD COLUMN `EffectData` MEDIUMTEXT NOT NULL AFTER `MorphTimeRemaining`;
ALTER TABLE `dbo`.`Cells` 
ADD COLUMN `EffectData` MEDIUMTEXT NOT NULL AFTER `Temporary`;
DROP TABLE `dbo`.`Effects`
