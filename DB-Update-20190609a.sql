ALTER TABLE `dbo`.`ClanMemberships` 
ADD COLUMN `PersonalName` TEXT NULL AFTER `PersonalNameId`;
ALTER TABLE `dbo`.`Characters` 
ADD COLUMN `NameInfo` TEXT NULL AFTER `CurrentProjectHours`;
ALTER TABLE `dbo`.`Characters` 
DROP FOREIGN KEY `FK_Characters_PersonalName_True`,
DROP FOREIGN KEY `FK_Characters_PersonalName_Current`;
ALTER TABLE `dbo`.`Characters` 
DROP COLUMN `CurrentNameId`,
DROP COLUMN `PersonalNameId`,
DROP INDEX `FK_Characters_PersonalName_True` ,
DROP INDEX `FK_Characters_PersonalName_Current` ;
;
ALTER TABLE `dbo`.`ClanMemberships` 
DROP FOREIGN KEY `FK_ClanMemberships_PersonalName`;
ALTER TABLE `dbo`.`ClanMemberships` 
DROP COLUMN `PersonalNameId`,
DROP INDEX `FK_ClanMemberships_PersonalName` ;
;
DROP TABLE `dbo`.`Characters_Aliases`;
DROP TABLE `dbo`.`PersonalNameElement`;
DROP TABLE `dbo`.`PersonalName`;