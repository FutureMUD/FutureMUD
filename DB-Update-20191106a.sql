ALTER TABLE `dbo`.`Exits` 
ADD COLUMN `BlockedLayers` VARCHAR(255) NOT NULL DEFAULT '' AFTER `ClimbDifficulty`;
