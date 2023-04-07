ALTER TABLE `dbo`.`RangedWeaponTypes` 
ADD COLUMN `RequiresFreeHandToReady` BIT NOT NULL DEFAULT b'1' AFTER `AimBonusLostPerShot`;
ALTER TABLE `dbo`.`RangedWeaponTypes` 
ADD COLUMN `AlwaysRequiresTwoHandsToWield` BIT NOT NULL DEFAULT b'0' AFTER `RequiresFreeHandToReady`;
