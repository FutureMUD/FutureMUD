ALTER TABLE `dbo`.`ChargenRoles_Costs` 
ADD COLUMN `RequirementOnly` BIT NOT NULL DEFAULT b'0' AFTER `Amount`,
DROP PRIMARY KEY,
ADD PRIMARY KEY (`ChargenRoleId`, `ChargenResourceId`, `RequirementOnly`);

ALTER TABLE `dbo`.`Ethnicities_ChargenResources` 
ADD COLUMN `RequirementOnly` BIT NOT NULL DEFAULT b'0' AFTER `Amount`,
DROP PRIMARY KEY,
ADD PRIMARY KEY (`EthnicityId`, `ChargenResourceId`, `RequirementOnly`);

ALTER TABLE `dbo`.`Merits_ChargenResources` 
ADD COLUMN `RequirementOnly` BIT NOT NULL DEFAULT b'0' AFTER `Amount`,
DROP PRIMARY KEY,
ADD PRIMARY KEY (`MeritId`, `ChargenResourceId`, `RequirementOnly`);

ALTER TABLE `dbo`.`Races_ChargenResources` 
ADD COLUMN `RequirementOnly` BIT NOT NULL DEFAULT b'0' AFTER `Amount`,
DROP PRIMARY KEY,
ADD PRIMARY KEY (`RaceId`, `ChargenResourceId`, `RequirementOnly`);

ALTER TABLE `dbo`.`TraitDefinitions_ChargenResources` 
ADD COLUMN `RequirementOnly` BIT NOT NULL DEFAULT b'0' AFTER `Amount`,
DROP PRIMARY KEY,
ADD PRIMARY KEY (`TraitDefinitionId`, `ChargenResourceId`, `RequirementOnly`);

ALTER TABLE `dbo`.`Cultures_ChargenResources` 
ADD COLUMN `RequirementOnly` BIT NOT NULL DEFAULT b'0' AFTER `Amount`,
DROP PRIMARY KEY,
ADD PRIMARY KEY (`CultureId`, `ChargenResourceId`, `RequirementOnly`);
