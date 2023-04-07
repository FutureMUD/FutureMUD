ALTER TABLE `dbo`.`WearProfiles` 
ADD COLUMN `RequireContainerIsEmpty` BIT NOT NULL DEFAULT b'0' AFTER `Description`;
