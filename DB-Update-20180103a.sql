ALTER TABLE `dbo`.`WeaponAttacks` 
CHANGE COLUMN `Intentions` `Intentions` BIGINT NOT NULL DEFAULT '0' ;
ALTER TABLE `dbo`.`CharacterCombatSettings` 
ADD COLUMN `PreferredWeaponSetup` INT NOT NULL DEFAULT 0 AFTER `MoveToMeleeIfCannotEngageInRangedCombat`;
