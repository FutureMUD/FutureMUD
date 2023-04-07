ALTER TABLE `dbo`.`GameItemProtos` 
ADD COLUMN `CustomColour` VARCHAR(45) NULL AFTER `BaseItemQuality`,
ADD COLUMN `HighPriority` BIT(1) NOT NULL DEFAULT b'0' AFTER `CustomColour`;
ALTER TABLE `dbo`.`CharacterCombatSettings` 
ADD COLUMN `MinimumStaminaToAttack` DOUBLE NOT NULL DEFAULT 0.0 AFTER `ManualPositionManagement`;