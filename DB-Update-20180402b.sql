ALTER TABLE `dbo`.`Liquids` 
ADD COLUMN `ResidueVolumePercentage` DOUBLE NOT NULL DEFAULT 0.05 AFTER `InjectionConsequence`;
ALTER TABLE `dbo`.`Materials` 
ADD COLUMN `Absorbency` DOUBLE NOT NULL DEFAULT 0.25 AFTER `ResidueColour`;
