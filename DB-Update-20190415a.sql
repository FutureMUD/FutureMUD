ALTER TABLE `dbo`.`Characters` 
ADD COLUMN `PreferredDefenseType` INT(11) NOT NULL DEFAULT 0 AFTER `CurrentCombatSettingId`;
