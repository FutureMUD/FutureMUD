ALTER TABLE `dbo`.`Characters` 
ADD COLUMN `CurrentProjectHours` DOUBLE NOT NULL DEFAULT 0.0 AFTER `CurrentProjectId`;
ALTER TABLE `dbo`.`ProjectLabourImpacts` 
ADD COLUMN `MinimumHoursForImpactToKickIn` DOUBLE NOT NULL DEFAULT 0.0 AFTER `ProjectLabourRequirementId`;
