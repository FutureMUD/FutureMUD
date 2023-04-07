ALTER TABLE `dbo`.`Races` 
ADD COLUMN `RaceUsesStamina` BIT NOT NULL DEFAULT b'1' AFTER `BloodModelId`;
