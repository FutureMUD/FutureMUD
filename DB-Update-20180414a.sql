ALTER TABLE `dbo`.`CharacteristicDefinitions` 
ADD COLUMN `Model` VARCHAR(45) NOT NULL DEFAULT 'standard' AFTER `ChargenDisplayType`;
ALTER TABLE `labmud_dbo`.`CharacteristicDefinitions` 
ADD COLUMN `Definition` TEXT NULL AFTER `Model`;
