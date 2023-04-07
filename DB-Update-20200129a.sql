ALTER TABLE `dbo`.`WitnessProfiles_IgnoredCriminalClasses` 
DROP FOREIGN KEY `FK_WitnessProfiles_IgnoredCriminalClasses_LegalClasses`;
ALTER TABLE `dbo`.`WitnessProfiles_IgnoredCriminalClasses` 
CHANGE COLUMN `LegalClassId` `LegalClassId` BIGINT(20) NOT NULL ,
DROP PRIMARY KEY,
ADD PRIMARY KEY (`WitnessProfileId`, `LegalClassId`);
;
ALTER TABLE `dbo`.`WitnessProfiles_IgnoredCriminalClasses` 
ADD CONSTRAINT `FK_WitnessProfiles_IgnoredCriminalClasses_LegalClasses`
  FOREIGN KEY (`LegalClassId`)
  REFERENCES `dbo`.`LegalClasses` (`Id`)
  ON DELETE CASCADE
  ON UPDATE CASCADE;
