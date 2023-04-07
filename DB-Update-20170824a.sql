ALTER TABLE `dbo`.`Checks` 
ADD COLUMN `MaximumDifficultyForImprovement` INT(11) NOT NULL DEFAULT 10 AFTER `CheckTemplateId`;
ALTER TABLE `dbo`.`TraitExpressionParameters` 

ADD COLUMN `CanImprove` BIT NOT NULL DEFAULT b'1' AFTER `Parameter`,

ADD COLUMN `CanBranch` BIT NOT NULL DEFAULT b'1' AFTER `CanImprove`;
