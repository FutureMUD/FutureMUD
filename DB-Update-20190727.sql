ALTER TABLE `dbo`.`Races` 
ADD COLUMN `BreathingVolumeExpression` VARCHAR(500) NOT NULL DEFAULT '7' AFTER `BodypartHealthMultiplier`,
ADD COLUMN `HoldBreathLengthExpression` VARCHAR(500) NOT NULL DEFAULT '120' AFTER `BreathingVolumeExpression`;
ALTER TABLE `dbo`.`Races_BreathableGases` 
ADD COLUMN `Multiplier` DOUBLE NOT NULL DEFAULT 1.0 AFTER `GasId`;
ALTER TABLE `dbo`.`Races_BreathableLiquids` 
ADD COLUMN `Multiplier` DOUBLE NOT NULL DEFAULT 1.0 AFTER `LiquidId`;
ALTER TABLE `dbo`.`Bodies` 
ADD COLUMN `HeldBreathLength` INT NOT NULL DEFAULT 0 AFTER `Tattoos`;
ALTER TABLE `dbo`.`UnitOfMeasure` 
ADD COLUMN `DefaultUnitForSystem` BIT NOT NULL DEFAULT b'0' AFTER `System`;
