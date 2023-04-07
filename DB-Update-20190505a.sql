ALTER TABLE `dbo`.`CharacterCombatSettings` 
ADD COLUMN `RequiredMinimumAim` DOUBLE NOT NULL DEFAULT 0.5 AFTER `PreferredWeaponSetup`;
