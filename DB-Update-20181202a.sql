ALTER TABLE `dbo`.`Exits` 
ADD COLUMN `FallCell` BIGINT NULL AFTER `MaximumSizeToEnterUpright`,
ADD COLUMN `IsClimbExit` BIT NOT NULL DEFAULT b'0' AFTER `FallCell`,
ADD COLUMN `ClimbDifficulty` INT NOT NULL DEFAULT 5 AFTER `IsClimbExit`;
