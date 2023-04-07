ALTER TABLE `dbo`.`CharacterCombatSettings` 
ADD COLUMN `GrappleResponse` INT(11) NOT NULL DEFAULT '0' AFTER `MeleeAttackOrderPreference`;