CREATE TABLE `dbo`.`AutobuilderRoomTemplates` (
  `ID` BIGINT NOT NULL AUTO_INCREMENT,
  `Name` VARCHAR(100) NOT NULL,
  `TemplateType` VARCHAR(45) NOT NULL,
  `Definition` LONGTEXT NOT NULL,
  PRIMARY KEY (`ID`));

CREATE TABLE `dbo`.`AutobuilderAreaTemplates` (
  `ID` BIGINT NOT NULL AUTO_INCREMENT,
  `Name` VARCHAR(100) NOT NULL,
  `TemplateType` VARCHAR(45) NOT NULL,
  `Definition` LONGTEXT NOT NULL,
  PRIMARY KEY (`ID`));
