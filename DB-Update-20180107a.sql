DROP TABLE `dbo`.`CraftItems`;
DROP TABLE `dbo`.`CraftProducts`;
DROP TABLE `dbo`.`CraftPhases`;
DROP TABLE `dbo`.`Crafts`;


CREATE TABLE `dbo`.`Crafts` (
  `Id` bigint(20) NOT NULL AUTO_INCREMENT,
  `RevisionNumber` int(11) NOT NULL,
  `EditableItemId` bigint(20) NOT NULL,
  `Name` varchar(200) NOT NULL,
  `Blurb` text NOT NULL,
  `ActionDescription` varchar(200) NOT NULL,
  `Category` varchar(200) NOT NULL,
  `Interruptable` bit(1) NOT NULL,
  `ToolQualityWeighting` double NOT NULL,
  `InputQualityWeighting` double NOT NULL,
  `CheckQualityWeighting` double NOT NULL,
  `FreeSkillChecks` int(11) NOT NULL,
  `FailThreshold` int(11) NOT NULL,
  `CheckTraitId` bigint(20) NOT NULL,
  `CheckDifficulty` int(11) NOT NULL,
  `FailPhase` int(11) NOT NULL,
  `QualityFormula` text NOT NULL,
  `AppearInCraftsListProgId` bigint(20) NOT NULL,
  `CanUseProgId` bigint(20) DEFAULT NULL,
  `WhyCannotUseProgId` bigint(20) DEFAULT NULL,
  `OnUseProgStartId` bigint(20) DEFAULT NULL,
  `OnUseProgCompleteId` bigint(20) DEFAULT NULL,
  `OnUseProgCancelId` bigint(20) DEFAULT NULL,
  `PhaseInformation` text NOT NULL,
  PRIMARY KEY (`Id`,`RevisionNumber`),
  KEY `FK_Crafts_TraitDefinitions_idx` (`CheckTraitId`),
  KEY `FK_Crafts_FutureProgs_AppearInCraftsListProg_idx` (`AppearInCraftsListProgId`),
  KEY `FK_Crafts_FutureProgs_CanUseProg_idx` (`CanUseProgId`),
  KEY `FK_Crafts_FutureProgs_WhyCannotUseProg_idx` (`WhyCannotUseProgId`),
  KEY `FK_Crafts_FutureProgs_OnUseProgStart_idx` (`OnUseProgStartId`),
  KEY `FK_Crafts_FutureProgs_OnUseProgComplete_idx` (`OnUseProgCompleteId`),
  KEY `FK_Crafts_FutureProgs_OnUseProgCancel_idx` (`OnUseProgCancelId`),
  KEY `FK_Crafts_EditableItems_idx` (`EditableItemId`),
  CONSTRAINT `FK_Crafts_EditableItems` FOREIGN KEY (`EditableItemId`) REFERENCES `dbo`.`EditableItems` (`Id`) ON DELETE CASCADE ON UPDATE CASCADE,
  CONSTRAINT `FK_Crafts_FutureProgs_AppearInCraftsListProg` FOREIGN KEY (`AppearInCraftsListProgId`) REFERENCES `dbo`.`FutureProgs` (`Id`) ON UPDATE CASCADE,
  CONSTRAINT `FK_Crafts_FutureProgs_CanUseProg` FOREIGN KEY (`CanUseProgId`) REFERENCES `dbo`.`FutureProgs` (`Id`) ON DELETE SET NULL ON UPDATE CASCADE,
  CONSTRAINT `FK_Crafts_FutureProgs_OnUseProgCancel` FOREIGN KEY (`OnUseProgCancelId`) REFERENCES `dbo`.`FutureProgs` (`Id`) ON DELETE SET NULL ON UPDATE CASCADE,
  CONSTRAINT `FK_Crafts_FutureProgs_OnUseProgComplete` FOREIGN KEY (`OnUseProgCompleteId`) REFERENCES `dbo`.`FutureProgs` (`Id`) ON DELETE SET NULL ON UPDATE CASCADE,
  CONSTRAINT `FK_Crafts_FutureProgs_OnUseProgStart` FOREIGN KEY (`OnUseProgStartId`) REFERENCES `dbo`.`FutureProgs` (`Id`) ON DELETE SET NULL ON UPDATE CASCADE,
  CONSTRAINT `FK_Crafts_FutureProgs_WhyCannotUseProg` FOREIGN KEY (`WhyCannotUseProgId`) REFERENCES `dbo`.`FutureProgs` (`Id`) ON DELETE SET NULL ON UPDATE CASCADE,
  CONSTRAINT `FK_Crafts_TraitDefinitions` FOREIGN KEY (`CheckTraitId`) REFERENCES `dbo`.`TraitDefinitions` (`Id`) ON DELETE NO ACTION ON UPDATE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8;


CREATE TABLE `dbo`.`CraftInputs` (
  `Id` bigint(20) NOT NULL AUTO_INCREMENT,
  `CraftId` bigint(20) NOT NULL,
  `CraftRevisionNumber` int(11) NOT NULL,
  `InputType` varchar(45) NOT NULL,
  `InputQualityWeight` double NOT NULL,
  `Definition` text NOT NULL,
  `OriginalAdditionTime` datetime NOT NULL,
  PRIMARY KEY (`Id`),
  KEY `FK_CraftInputs_Crafts_idx` (`CraftId`,`CraftRevisionNumber`),
  CONSTRAINT `FK_CraftInputs_Crafts` FOREIGN KEY (`CraftId`, `CraftRevisionNumber`) REFERENCES `dbo`.`Crafts` (`Id`, `RevisionNumber`) ON DELETE CASCADE ON UPDATE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

CREATE TABLE `dbo`.`CraftProducts` (
  `Id` bigint(20) NOT NULL AUTO_INCREMENT,
  `CraftId` bigint(20) NOT NULL,
  `CraftRevisionNumber` int(11) NOT NULL,
  `ProductType` varchar(45) NOT NULL,
  `Definition` text NOT NULL,
  `OriginalAdditionTime` datetime NOT NULL,
  PRIMARY KEY (`Id`),
  KEY `FK_CraftProducts_Crafts_idx` (`CraftId`,`CraftRevisionNumber`),
  CONSTRAINT `FK_CraftProducts_Crafts` FOREIGN KEY (`CraftId`, `CraftRevisionNumber`) REFERENCES `dbo`.`Crafts` (`Id`, `RevisionNumber`) ON DELETE CASCADE ON UPDATE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

CREATE TABLE `dbo`.`CraftTools` (
  `Id` bigint(20) NOT NULL AUTO_INCREMENT,
  `CraftId` bigint(20) NOT NULL,
  `CraftRevisionNumber` int(11) NOT NULL,
  `OriginalAdditionTime` datetime NOT NULL,
  `ToolType` varchar(45) NOT NULL,
  `ToolQualityWeight` double NOT NULL,
  `DesiredState` int(11) NOT NULL,
  `Definition` text NOT NULL,
  PRIMARY KEY (`Id`),
  KEY `FK_CraftTools_Crafts_idx` (`CraftId`,`CraftRevisionNumber`),
  CONSTRAINT `FK_CraftTools_Crafts` FOREIGN KEY (`CraftId`, `CraftRevisionNumber`) REFERENCES `dbo`.`Crafts` (`Id`, `RevisionNumber`) ON DELETE CASCADE ON UPDATE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
