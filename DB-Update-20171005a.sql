ALTER TABLE `dbo`.`Cells` 
ADD COLUMN `Temporary` BIT(1) NOT NULL DEFAULT b'0' AFTER `ForagableProfileId`;
