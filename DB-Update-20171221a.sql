ALTER TABLE `dbo`.`CharacterCombatSettings` 
ADD COLUMN `MoveToMeleeIfCannotEngageInRangedCombat` BIT NOT NULL DEFAULT b'1' AFTER `MinimumStaminaToAttack`;
