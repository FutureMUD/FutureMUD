ALTER TABLE `dbo`.`TraitDefinitions` 
ADD COLUMN `BranchMultiplier` DOUBLE NOT NULL DEFAULT 1.0 AFTER `ChargenBlurb`;
