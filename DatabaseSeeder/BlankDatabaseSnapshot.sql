CREATE DATABASE IF NOT EXISTS `__FUTUREMUD_DATABASE__`;
USE `__FUTUREMUD_DATABASE__`;
CREATE TABLE IF NOT EXISTS `__EFMigrationsHistory` (
    `MigrationId` varchar(150) CHARACTER SET utf8mb4 NOT NULL,
    `ProductVersion` varchar(32) CHARACTER SET utf8mb4 NOT NULL,
    CONSTRAINT `PK___EFMigrationsHistory` PRIMARY KEY (`MigrationId`)
) CHARACTER SET=utf8mb4;

START TRANSACTION;
CREATE TABLE `AmmunitionTypes` (
    `Id` bigint(20) NOT NULL AUTO_INCREMENT,
    `Name` varchar(200) CHARACTER SET utf8mb4 NOT NULL,
    `SpecificType` text CHARACTER SET utf8mb4 NOT NULL,
    `RangedWeaponTypes` varchar(200) CHARACTER SET utf8mb4 NOT NULL,
    `BaseAccuracy` double NOT NULL,
    `Loudness` int(11) NOT NULL,
    `BreakChanceOnHit` double NOT NULL,
    `BreakChanceOnMiss` double NOT NULL,
    `BaseBlockDifficulty` int(11) NOT NULL,
    `BaseDodgeDifficulty` int(11) NOT NULL,
    `DamageExpression` text CHARACTER SET utf8mb4 NOT NULL,
    `StunExpression` text CHARACTER SET utf8mb4 NOT NULL,
    `PainExpression` text CHARACTER SET utf8mb4 NOT NULL,
    `DamageType` int(11) NOT NULL,
    CONSTRAINT `PK_AmmunitionTypes` PRIMARY KEY (`Id`)
);

CREATE TABLE `ArmourTypes` (
    `Id` bigint(20) NOT NULL AUTO_INCREMENT,
    `Name` varchar(200) CHARACTER SET utf8mb4 NOT NULL,
    `MinimumPenetrationDegree` int(11) NOT NULL,
    `BaseDifficultyDegrees` int(11) NOT NULL,
    `StackedDifficultyDegrees` int(11) NOT NULL,
    `Definition` text CHARACTER SET utf8mb4 NOT NULL,
    CONSTRAINT `PK_ArmourTypes` PRIMARY KEY (`Id`)
);

CREATE TABLE `ArtificialIntelligences` (
    `Id` bigint(20) NOT NULL AUTO_INCREMENT,
    `Name` varchar(4000) CHARACTER SET utf8 NOT NULL,
    `Type` varchar(4000) CHARACTER SET utf8 NOT NULL,
    `Definition` mediumtext CHARACTER SET utf8 NOT NULL,
    CONSTRAINT `PK_ArtificialIntelligences` PRIMARY KEY (`Id`)
);

CREATE TABLE `AuthorityGroups` (
    `Id` bigint(20) NOT NULL AUTO_INCREMENT,
    `Name` varchar(4000) CHARACTER SET utf8 NOT NULL,
    `AuthorityLevel` int(11) NOT NULL,
    `InformationLevel` int(11) NOT NULL,
    `AccountsLevel` int(11) NOT NULL,
    `CharactersLevel` int(11) NOT NULL,
    `CharacterApprovalLevel` int(11) NOT NULL,
    `CharacterApprovalRisk` int(11) NOT NULL,
    `ItemsLevel` int(11) NOT NULL,
    `PlanesLevel` int(11) NOT NULL,
    `RoomsLevel` int(11) NOT NULL,
    CONSTRAINT `PK_AuthorityGroups` PRIMARY KEY (`Id`)
);

CREATE TABLE `AutobuilderAreaTemplates` (
    `ID` bigint(20) NOT NULL AUTO_INCREMENT,
    `Name` varchar(100) CHARACTER SET utf8 NOT NULL,
    `TemplateType` varchar(45) CHARACTER SET utf8 NOT NULL,
    `Definition` longtext CHARACTER SET utf8 NOT NULL,
    CONSTRAINT `PK_AutobuilderAreaTemplates` PRIMARY KEY (`ID`)
);

CREATE TABLE `AutobuilderRoomTemplates` (
    `ID` bigint(20) NOT NULL AUTO_INCREMENT,
    `Name` varchar(100) CHARACTER SET utf8 NOT NULL,
    `TemplateType` varchar(45) CHARACTER SET utf8 NOT NULL,
    `Definition` longtext CHARACTER SET utf8 NOT NULL,
    CONSTRAINT `PK_AutobuilderRoomTemplates` PRIMARY KEY (`ID`)
);

CREATE TABLE `BloodModels` (
    `Id` bigint(20) NOT NULL AUTO_INCREMENT,
    `Name` varchar(200) CHARACTER SET utf8 NOT NULL,
    CONSTRAINT `PK_BloodModels` PRIMARY KEY (`Id`)
);

CREATE TABLE `BloodtypeAntigens` (
    `Id` bigint(20) NOT NULL AUTO_INCREMENT,
    `Name` varchar(200) CHARACTER SET utf8 NOT NULL,
    CONSTRAINT `PK_BloodtypeAntigens` PRIMARY KEY (`Id`)
);

CREATE TABLE `Bloodtypes` (
    `Id` bigint(20) NOT NULL AUTO_INCREMENT,
    `Name` varchar(200) CHARACTER SET utf8 NOT NULL,
    CONSTRAINT `PK_Bloodtypes` PRIMARY KEY (`Id`)
);

CREATE TABLE `Boards` (
    `Id` bigint(20) NOT NULL AUTO_INCREMENT,
    `Name` varchar(45) CHARACTER SET utf8 NOT NULL,
    `ShowOnLogin` bit(1) NOT NULL,
    CONSTRAINT `PK_Boards` PRIMARY KEY (`Id`)
);

CREATE TABLE `BodypartGroupDescribers` (
    `Id` bigint(20) NOT NULL AUTO_INCREMENT,
    `DescribedAs` varchar(4000) CHARACTER SET utf8 NOT NULL,
    `Comment` varchar(4000) CHARACTER SET utf8 NULL,
    `Type` varchar(50) CHARACTER SET utf8 NOT NULL,
    CONSTRAINT `PK_BodypartGroupDescribers` PRIMARY KEY (`Id`)
);

CREATE TABLE `BodypartShape` (
    `Id` bigint(20) NOT NULL AUTO_INCREMENT,
    `Name` varchar(4000) CHARACTER SET utf8 NOT NULL,
    CONSTRAINT `PK_BodypartShape` PRIMARY KEY (`Id`)
);

CREATE TABLE `bodypartshapecountview` (
    `BodypartGroupDescriptionRuleId` tinyint(4) NOT NULL,
    `DescribedAs` tinyint(4) NOT NULL,
    `MinCount` tinyint(4) NOT NULL,
    `MaxCount` tinyint(4) NOT NULL,
    `TargetId` tinyint(4) NOT NULL,
    `Name` tinyint(4) NOT NULL
);

CREATE TABLE `Calendars` (
    `Id` bigint(20) NOT NULL AUTO_INCREMENT,
    `Definition` text CHARACTER SET utf8 NOT NULL,
    `Date` varchar(50) CHARACTER SET utf8 NOT NULL,
    `FeedClockId` bigint(20) NOT NULL,
    CONSTRAINT `PK_Calendars` PRIMARY KEY (`Id`)
);

CREATE TABLE `Celestials` (
    `Id` bigint(20) NOT NULL AUTO_INCREMENT,
    `Definition` text CHARACTER SET utf8 NOT NULL,
    `Minutes` int(11) NOT NULL,
    `FeedClockId` bigint(20) NOT NULL,
    `CelestialYear` int(11) NOT NULL,
    `LastYearBump` int(11) NOT NULL,
    CONSTRAINT `PK_Celestials` PRIMARY KEY (`Id`)
);

CREATE TABLE `CharacteristicDefinitions` (
    `Id` bigint(20) NOT NULL AUTO_INCREMENT,
    `Name` varchar(4000) CHARACTER SET utf8 NOT NULL,
    `Type` int(11) NOT NULL,
    `Pattern` varchar(4000) CHARACTER SET utf8 NOT NULL,
    `Description` varchar(4000) CHARACTER SET utf8 NOT NULL,
    `ParentId` bigint(20) NULL,
    `ChargenDisplayType` int(11) NULL,
    `Model` varchar(45) CHARACTER SET utf8 NOT NULL DEFAULT 'standard',
    `Definition` text CHARACTER SET utf8 NULL,
    CONSTRAINT `PK_CharacteristicDefinitions` PRIMARY KEY (`Id`),
    CONSTRAINT `FK_CharacteristicDefinitions_Parent` FOREIGN KEY (`ParentId`) REFERENCES `CharacteristicDefinitions` (`Id`) ON DELETE RESTRICT
);

CREATE TABLE `ChargenResources` (
    `Id` bigint(20) NOT NULL AUTO_INCREMENT,
    `Name` varchar(50) CHARACTER SET utf8 NOT NULL,
    `PluralName` varchar(50) CHARACTER SET utf8 NOT NULL,
    `Alias` varchar(50) CHARACTER SET utf8 NOT NULL,
    `MinimumTimeBetweenAwards` int(11) NOT NULL,
    `MaximumNumberAwardedPerAward` int(11) NOT NULL,
    `PermissionLevelRequiredToAward` int(11) NOT NULL,
    `PermissionLevelRequiredToCircumventMinimumTime` int(11) NOT NULL,
    `ShowToPlayerInScore` bit(1) NOT NULL,
    `TextDisplayedToPlayerOnAward` varchar(4000) CHARACTER SET utf8 NOT NULL,
    `TextDisplayedToPlayerOnDeduct` varchar(4000) CHARACTER SET utf8 NOT NULL,
    `MaximumResourceId` bigint(20) NULL,
    `MaximumResourceFormula` varchar(4000) CHARACTER SET utf8 NOT NULL,
    `Type` varchar(50) CHARACTER SET utf8 NOT NULL,
    CONSTRAINT `PK_ChargenResources` PRIMARY KEY (`Id`)
);

CREATE TABLE `CheckTemplates` (
    `Id` bigint(20) NOT NULL AUTO_INCREMENT,
    `Name` varchar(4000) CHARACTER SET utf8 NOT NULL,
    `Definition` text CHARACTER SET utf8 NULL,
    `CheckMethod` varchar(25) CHARACTER SET utf8 NOT NULL DEFAULT 'Standard',
    `ImproveTraits` bit(1) NOT NULL DEFAULT b'0',
    `FailIfTraitMissingMode` smallint(6) NOT NULL,
    `CanBranchIfTraitMissing` bit(1) NOT NULL DEFAULT b'0',
    CONSTRAINT `PK_CheckTemplates` PRIMARY KEY (`Id`)
);

CREATE TABLE `ClimateModels` (
    `Id` bigint(20) NOT NULL AUTO_INCREMENT,
    `Name` varchar(200) CHARACTER SET utf8 NOT NULL,
    `Type` varchar(45) CHARACTER SET utf8 NOT NULL,
    `Definition` mediumtext CHARACTER SET utf8 NOT NULL,
    `MinuteProcessingInterval` int(11) NOT NULL,
    `MinimumMinutesBetweenFlavourEchoes` int(11) NOT NULL,
    `MinuteFlavourEchoChance` double NOT NULL,
    CONSTRAINT `PK_ClimateModels` PRIMARY KEY (`Id`)
);

CREATE TABLE `Clocks` (
    `Id` bigint(20) NOT NULL AUTO_INCREMENT,
    `Definition` text CHARACTER SET utf8 NOT NULL,
    `Seconds` int(11) NOT NULL,
    `Minutes` int(11) NOT NULL,
    `Hours` int(11) NOT NULL,
    `PrimaryTimezoneId` bigint(20) NOT NULL,
    CONSTRAINT `PK_Clocks` PRIMARY KEY (`Id`)
);

CREATE TABLE `Colours` (
    `Id` bigint(20) NOT NULL AUTO_INCREMENT,
    `Name` varchar(50) CHARACTER SET utf8 NOT NULL,
    `Basic` int(11) NOT NULL,
    `Red` int(11) NOT NULL,
    `Green` int(11) NOT NULL,
    `Blue` int(11) NOT NULL,
    `Fancy` varchar(4000) CHARACTER SET utf8 NULL,
    CONSTRAINT `PK_Colours` PRIMARY KEY (`Id`)
);

CREATE TABLE `CorpseModels` (
    `Id` bigint(20) NOT NULL AUTO_INCREMENT,
    `Name` varchar(100) CHARACTER SET utf8mb4 NULL,
    `Description` text CHARACTER SET utf8mb4 NULL,
    `Definition` text CHARACTER SET utf8mb4 NULL,
    `Type` varchar(45) CHARACTER SET utf8mb4 NOT NULL,
    CONSTRAINT `PK_CorpseModels` PRIMARY KEY (`Id`)
);

CREATE TABLE `CultureInfos` (
    `Id` varchar(50) CHARACTER SET utf8 NOT NULL,
    `DisplayName` varchar(4000) CHARACTER SET utf8 NOT NULL,
    `Order` int(11) NOT NULL,
    CONSTRAINT `PK_CultureInfos` PRIMARY KEY (`Id`)
);

CREATE TABLE `Currencies` (
    `Id` bigint(20) NOT NULL AUTO_INCREMENT,
    `Name` varchar(4000) CHARACTER SET utf8 NOT NULL,
    CONSTRAINT `PK_Currencies` PRIMARY KEY (`Id`)
);

CREATE TABLE `DamagePatterns` (
    `Id` bigint(20) NOT NULL AUTO_INCREMENT,
    `Name` varchar(4000) CHARACTER SET utf8 NOT NULL,
    `DamageType` int(11) NOT NULL,
    `Dice` int(11) NOT NULL,
    `Sides` int(11) NOT NULL,
    `Bonus` int(11) NOT NULL,
    CONSTRAINT `PK_DamagePatterns` PRIMARY KEY (`Id`)
);

CREATE TABLE `Drugs` (
    `Id` bigint(20) NOT NULL AUTO_INCREMENT,
    `Name` varchar(200) CHARACTER SET utf8mb4 NOT NULL,
    `DrugTypes` int(11) NOT NULL,
    `DrugVectors` int(11) NOT NULL,
    `IntensityPerGram` double NOT NULL,
    `RelativeMetabolisationRate` double NOT NULL,
    CONSTRAINT `PK_Drugs` PRIMARY KEY (`Id`)
);

CREATE TABLE `EditableItems` (
    `Id` bigint(20) NOT NULL AUTO_INCREMENT,
    `RevisionNumber` int(11) NOT NULL,
    `RevisionStatus` int(11) NOT NULL,
    `BuilderAccountId` bigint(20) NOT NULL,
    `ReviewerAccountId` bigint(20) NULL,
    `BuilderComment` varchar(4000) CHARACTER SET utf8 NULL,
    `ReviewerComment` varchar(4000) CHARACTER SET utf8 NULL,
    `BuilderDate` datetime NOT NULL,
    `ReviewerDate` datetime NULL,
    `ObsoleteDate` datetime NULL,
    CONSTRAINT `PK_EditableItems` PRIMARY KEY (`Id`)
);

CREATE TABLE `EmailTemplates` (
    `TemplateType` int(11) NOT NULL AUTO_INCREMENT,
    `Content` varchar(4000) CHARACTER SET utf8 NOT NULL,
    `Subject` varchar(4000) CHARACTER SET utf8 NOT NULL,
    `ReturnAddress` varchar(4000) CHARACTER SET utf8 NOT NULL,
    CONSTRAINT `PRIMARY` PRIMARY KEY (`TemplateType`)
);

CREATE TABLE `EntityDescriptions` (
    `Id` bigint(20) NOT NULL AUTO_INCREMENT,
    `ShortDescription` varchar(4000) CHARACTER SET utf8 NULL,
    `FullDescription` varchar(4000) CHARACTER SET utf8 NULL,
    `DisplaySex` smallint(6) NOT NULL,
    CONSTRAINT `PK_EntityDescriptions` PRIMARY KEY (`Id`)
);

CREATE TABLE `Exits` (
    `Id` bigint(20) NOT NULL AUTO_INCREMENT,
    `Keywords1` varchar(255) CHARACTER SET utf8 NULL,
    `Keywords2` varchar(255) CHARACTER SET utf8 NULL,
    `CellId1` bigint(20) NOT NULL,
    `CellId2` bigint(20) NOT NULL,
    `DoorId` bigint(20) NULL,
    `Direction1` int(11) NOT NULL,
    `Direction2` int(11) NOT NULL,
    `TimeMultiplier` double NOT NULL,
    `InboundDescription1` varchar(255) CHARACTER SET utf8 NULL,
    `InboundDescription2` varchar(255) CHARACTER SET utf8 NULL,
    `OutboundDescription1` varchar(255) CHARACTER SET utf8 NULL,
    `OutboundDescription2` varchar(255) CHARACTER SET utf8 NULL,
    `InboundTarget1` varchar(255) CHARACTER SET utf8 NULL,
    `InboundTarget2` varchar(255) CHARACTER SET utf8 NULL,
    `OutboundTarget1` varchar(255) CHARACTER SET utf8 NULL,
    `OutboundTarget2` varchar(255) CHARACTER SET utf8 NULL,
    `Verb1` varchar(255) CHARACTER SET utf8 NULL,
    `Verb2` varchar(255) CHARACTER SET utf8 NULL,
    `PrimaryKeyword1` varchar(255) CHARACTER SET utf8 NULL,
    `PrimaryKeyword2` varchar(255) CHARACTER SET utf8 NULL,
    `AcceptsDoor` bit(1) NOT NULL,
    `DoorSize` int(11) NULL,
    `MaximumSizeToEnter` int(11) NOT NULL DEFAULT '12',
    `MaximumSizeToEnterUpright` int(11) NOT NULL DEFAULT '12',
    `FallCell` bigint(20) NULL,
    `IsClimbExit` bit(1) NOT NULL DEFAULT b'0',
    `ClimbDifficulty` int(11) NOT NULL DEFAULT '5',
    `BlockedLayers` varchar(255) CHARACTER SET utf8 NOT NULL DEFAULT '',
    CONSTRAINT `PK_Exits` PRIMARY KEY (`Id`)
);

CREATE TABLE `FutureProgs` (
    `Id` bigint(20) NOT NULL AUTO_INCREMENT,
    `FunctionName` varchar(255) CHARACTER SET utf8 NOT NULL,
    `FunctionComment` text CHARACTER SET utf8 NOT NULL,
    `FunctionText` text CHARACTER SET utf8mb4 NOT NULL,
    `ReturnType` bigint(20) NOT NULL,
    `Category` varchar(255) CHARACTER SET utf8 NOT NULL,
    `Subcategory` varchar(255) CHARACTER SET utf8 NOT NULL,
    `Public` bit(1) NOT NULL DEFAULT b'0',
    `AcceptsAnyParameters` bit(1) NOT NULL DEFAULT b'0',
    `StaticType` int(11) NOT NULL,
    CONSTRAINT `PK_FutureProgs` PRIMARY KEY (`Id`)
);

CREATE TABLE `gameitemeditingview` (
    `Id` tinyint(4) NOT NULL,
    `Name` tinyint(4) NOT NULL,
    `MaterialId` tinyint(4) NOT NULL,
    `ProtoMaterial` tinyint(4) NOT NULL,
    `Quality` tinyint(4) NOT NULL,
    `Size` tinyint(4) NOT NULL
);

CREATE TABLE `GameItems` (
    `Id` bigint(20) NOT NULL AUTO_INCREMENT,
    `Quality` int(11) NOT NULL,
    `GameItemProtoId` bigint(20) NOT NULL,
    `GameItemProtoRevision` int(11) NOT NULL,
    `RoomLayer` int(11) NOT NULL,
    `Condition` double NOT NULL DEFAULT '1',
    `MaterialId` bigint(20) NOT NULL,
    `Size` int(11) NOT NULL,
    `ContainerId` bigint(20) NULL,
    `PositionId` int(11) NOT NULL DEFAULT '1',
    `PositionModifier` int(11) NOT NULL,
    `PositionTargetId` bigint(20) NULL,
    `PositionTargetType` varchar(45) CHARACTER SET utf8 NULL,
    `PositionEmote` text CHARACTER SET utf8 NULL,
    `MorphTimeRemaining` int(11) NULL,
    `EffectData` mediumtext CHARACTER SET utf8 NOT NULL,
    CONSTRAINT `PK_GameItems` PRIMARY KEY (`Id`),
    CONSTRAINT `FK_GameItems_GameItems_Containers` FOREIGN KEY (`ContainerId`) REFERENCES `GameItems` (`Id`) ON DELETE SET NULL
);

CREATE TABLE `Grids` (
    `Id` bigint(20) NOT NULL AUTO_INCREMENT,
    `GridType` varchar(45) CHARACTER SET utf8 NOT NULL,
    `Definition` mediumtext CHARACTER SET utf8 NOT NULL,
    CONSTRAINT `PK_Grids` PRIMARY KEY (`Id`)
);

CREATE TABLE `GroupAITemplates` (
    `Id` bigint(20) NOT NULL AUTO_INCREMENT,
    `Name` varchar(200) CHARACTER SET utf8 NOT NULL,
    `Definition` mediumtext CHARACTER SET utf8 NOT NULL,
    CONSTRAINT `PK_GroupAITemplates` PRIMARY KEY (`Id`)
);

CREATE TABLE `HealthStrategies` (
    `Id` bigint(20) NOT NULL AUTO_INCREMENT,
    `Name` varchar(200) CHARACTER SET utf8mb4 NOT NULL,
    `Type` varchar(100) CHARACTER SET utf8mb4 NOT NULL,
    `Definition` text CHARACTER SET utf8mb4 NOT NULL,
    CONSTRAINT `PK_HealthStrategies` PRIMARY KEY (`Id`)
);

CREATE TABLE `HearingProfiles` (
    `Id` bigint(20) NOT NULL AUTO_INCREMENT,
    `Name` varchar(4000) CHARACTER SET utf8 NOT NULL,
    `Definition` text CHARACTER SET utf8 NOT NULL,
    `Type` varchar(4000) CHARACTER SET utf8 NOT NULL,
    `SurveyDescription` varchar(4000) CHARACTER SET utf8 NOT NULL,
    CONSTRAINT `PK_HearingProfiles` PRIMARY KEY (`Id`)
);

CREATE TABLE `HeightWeightModels` (
    `Id` bigint(20) NOT NULL AUTO_INCREMENT,
    `Name` varchar(4000) CHARACTER SET utf8 NOT NULL,
    `MeanHeight` double NOT NULL,
    `MeanBMI` double NOT NULL,
    `StddevHeight` double NOT NULL,
    `StddevBMI` double NOT NULL,
    `BMIMultiplier` double NOT NULL,
    CONSTRAINT `PK_HeightWeightModels` PRIMARY KEY (`Id`)
);

CREATE TABLE `Hooks` (
    `Id` bigint(20) NOT NULL AUTO_INCREMENT,
    `Name` varchar(255) CHARACTER SET utf8 NOT NULL,
    `Definition` text CHARACTER SET utf8mb4 NOT NULL,
    `Type` varchar(255) CHARACTER SET utf8 NOT NULL,
    `Category` varchar(255) CHARACTER SET utf8 NOT NULL,
    `TargetEventType` int(11) NOT NULL,
    CONSTRAINT `PK_Hooks` PRIMARY KEY (`Id`)
);

CREATE TABLE `Improvers` (
    `Id` bigint(20) NOT NULL AUTO_INCREMENT,
    `Name` varchar(255) CHARACTER SET utf8 NOT NULL,
    `Definition` text CHARACTER SET utf8 NOT NULL,
    `Type` varchar(50) CHARACTER SET utf8 NOT NULL,
    CONSTRAINT `PK_Improvers` PRIMARY KEY (`Id`)
);

CREATE TABLE `ItemGroups` (
    `Id` bigint(20) NOT NULL AUTO_INCREMENT,
    `Name` varchar(100) CHARACTER SET utf8mb4 NULL,
    `Keywords` varchar(1000) CHARACTER SET utf8mb4 NULL,
    CONSTRAINT `PK_ItemGroups` PRIMARY KEY (`Id`)
);

CREATE TABLE `LanguageDifficultyModels` (
    `Id` bigint(20) NOT NULL AUTO_INCREMENT,
    `Definition` longtext CHARACTER SET utf8 NOT NULL,
    `Type` varchar(255) CHARACTER SET utf8 NOT NULL,
    `Name` varchar(255) CHARACTER SET utf8 NOT NULL,
    CONSTRAINT `PK_LanguageDifficultyModels` PRIMARY KEY (`Id`)
);

CREATE TABLE `Locks` (
    `Id` bigint(20) NOT NULL AUTO_INCREMENT,
    `Name` varchar(255) CHARACTER SET utf8 NOT NULL,
    `Style` int(11) NOT NULL,
    `Strength` int(11) NOT NULL,
    CONSTRAINT `PK_Locks` PRIMARY KEY (`Id`)
);

CREATE TABLE `MagicGenerators` (
    `Id` bigint(20) NOT NULL AUTO_INCREMENT,
    `Name` varchar(200) CHARACTER SET utf8 NOT NULL,
    `Type` varchar(45) CHARACTER SET utf8 NOT NULL,
    `Definition` text CHARACTER SET utf8 NOT NULL,
    CONSTRAINT `PK_MagicGenerators` PRIMARY KEY (`Id`)
);

CREATE TABLE `MagicResources` (
    `Id` bigint(20) NOT NULL AUTO_INCREMENT,
    `Name` varchar(200) CHARACTER SET utf8 NOT NULL,
    `Type` varchar(45) CHARACTER SET utf8 NOT NULL,
    `Definition` text CHARACTER SET utf8 NOT NULL,
    `MagicResourceType` int(11) NOT NULL,
    CONSTRAINT `PK_MagicResources` PRIMARY KEY (`Id`)
);

CREATE TABLE `MagicSchools` (
    `Id` bigint(20) NOT NULL AUTO_INCREMENT,
    `Name` varchar(200) CHARACTER SET utf8 NOT NULL,
    `ParentSchoolId` bigint(20) NULL,
    `SchoolVerb` varchar(45) CHARACTER SET utf8 NOT NULL,
    `SchoolAdjective` varchar(45) CHARACTER SET utf8 NOT NULL,
    `PowerListColour` varchar(45) CHARACTER SET utf8 NOT NULL,
    CONSTRAINT `PK_MagicSchools` PRIMARY KEY (`Id`),
    CONSTRAINT `FK_MagicSchools_MagicSchools` FOREIGN KEY (`ParentSchoolId`) REFERENCES `MagicSchools` (`Id`) ON DELETE CASCADE
);

CREATE TABLE `Materials` (
    `Id` bigint(20) NOT NULL AUTO_INCREMENT,
    `Name` varchar(255) CHARACTER SET utf8 NOT NULL,
    `MaterialDescription` varchar(4000) CHARACTER SET utf8 NOT NULL,
    `Density` double NOT NULL,
    `Organic` bit(1) NOT NULL,
    `Type` int(11) NOT NULL,
    `BehaviourType` int(11) NULL,
    `ThermalConductivity` double NOT NULL,
    `ElectricalConductivity` double NOT NULL,
    `SpecificHeatCapacity` double NOT NULL,
    `SolidFormId` bigint(20) NULL,
    `LiquidFormId` bigint(20) NULL,
    `GasFormId` bigint(20) NULL,
    `Viscosity` double NULL,
    `MeltingPoint` double NULL,
    `BoilingPoint` double NULL,
    `IgnitionPoint` double NULL,
    `HeatDamagePoint` double NULL,
    `ImpactFracture` double NULL,
    `ImpactYield` double NULL,
    `ImpactStrainAtYield` double NULL,
    `ShearFracture` double NULL,
    `ShearYield` double NULL,
    `ShearStrainAtYield` double NULL,
    `YoungsModulus` double NULL,
    `SolventId` bigint(20) NULL,
    `SolventVolumeRatio` double NOT NULL DEFAULT '1',
    `ResidueSdesc` text CHARACTER SET utf8 NULL,
    `ResidueDesc` text CHARACTER SET utf8 NULL,
    `ResidueColour` varchar(45) CHARACTER SET utf8 NULL DEFAULT 'white',
    `Absorbency` double NOT NULL DEFAULT '0.25',
    CONSTRAINT `PK_Materials` PRIMARY KEY (`Id`)
);

CREATE TABLE `Merits` (
    `Id` bigint(20) NOT NULL AUTO_INCREMENT,
    `Name` varchar(200) CHARACTER SET utf8mb4 NOT NULL,
    `Type` varchar(50) CHARACTER SET utf8mb4 NOT NULL,
    `MeritType` int(11) NOT NULL,
    `MeritScope` int(11) NOT NULL,
    `Definition` text CHARACTER SET utf8mb4 NOT NULL,
    `ParentId` bigint(20) NULL,
    CONSTRAINT `PK_Merits` PRIMARY KEY (`Id`),
    CONSTRAINT `FK_Merits_Merits` FOREIGN KEY (`ParentId`) REFERENCES `Merits` (`Id`) ON DELETE SET NULL
);

CREATE TABLE `NameCulture` (
    `Id` bigint(20) NOT NULL AUTO_INCREMENT,
    `Name` varchar(255) CHARACTER SET utf8 NOT NULL,
    `Definition` text CHARACTER SET utf8 NOT NULL,
    CONSTRAINT `PK_NameCulture` PRIMARY KEY (`Id`)
);

CREATE TABLE `NonCardinalExitTemplates` (
    `Id` bigint(20) NOT NULL AUTO_INCREMENT,
    `Name` varchar(255) CHARACTER SET utf8 NOT NULL,
    `OriginOutboundPreface` varchar(255) CHARACTER SET utf8 NOT NULL,
    `OriginInboundPreface` varchar(255) CHARACTER SET utf8 NOT NULL,
    `DestinationOutboundPreface` varchar(255) CHARACTER SET utf8 NOT NULL,
    `DestinationInboundPreface` varchar(255) CHARACTER SET utf8 NOT NULL,
    `OutboundVerb` varchar(255) CHARACTER SET utf8 NOT NULL,
    `InboundVerb` varchar(255) CHARACTER SET utf8 NOT NULL,
    CONSTRAINT `PK_NonCardinalExitTemplates` PRIMARY KEY (`Id`)
);

CREATE TABLE `PopulationBloodModels` (
    `Id` bigint(20) NOT NULL AUTO_INCREMENT,
    `Name` varchar(200) CHARACTER SET utf8 NOT NULL,
    CONSTRAINT `PK_PopulationBloodModels` PRIMARY KEY (`Id`)
);

CREATE TABLE `RangedCovers` (
    `Id` bigint(20) NOT NULL AUTO_INCREMENT,
    `Name` varchar(200) CHARACTER SET utf8mb4 NOT NULL,
    `CoverType` int(11) NOT NULL,
    `CoverExtent` int(11) NOT NULL,
    `HighestPositionState` int(11) NOT NULL,
    `DescriptionString` text CHARACTER SET utf8mb4 NOT NULL,
    `ActionDescriptionString` text CHARACTER SET utf8mb4 NOT NULL,
    `MaximumSimultaneousCovers` int(11) NOT NULL,
    `CoverStaysWhileMoving` bit(1) NOT NULL,
    CONSTRAINT `PK_RangedCovers` PRIMARY KEY (`Id`)
);

CREATE TABLE `RegionalClimates` (
    `Id` bigint(20) NOT NULL AUTO_INCREMENT,
    `Name` varchar(200) CHARACTER SET utf8 NOT NULL,
    `ClimateModelId` bigint(20) NOT NULL,
    CONSTRAINT `PK_RegionalClimates` PRIMARY KEY (`Id`)
);

CREATE TABLE `SkyDescriptionTemplates` (
    `Id` bigint(20) NOT NULL AUTO_INCREMENT,
    `Name` varchar(255) CHARACTER SET utf8 NOT NULL,
    CONSTRAINT `PK_SkyDescriptionTemplates` PRIMARY KEY (`Id`)
);

CREATE TABLE `StackDecorators` (
    `Id` bigint(20) NOT NULL AUTO_INCREMENT,
    `Name` varchar(255) CHARACTER SET utf8 NOT NULL,
    `Type` varchar(50) CHARACTER SET utf8 NOT NULL,
    `Definition` varchar(10000) CHARACTER SET utf8 NOT NULL,
    `Description` varchar(1000) CHARACTER SET utf8 NULL,
    CONSTRAINT `PK_StackDecorators` PRIMARY KEY (`Id`)
);

CREATE TABLE `StaticConfigurations` (
    `SettingName` varchar(100) CHARACTER SET utf8 NOT NULL,
    `Definition` text CHARACTER SET utf8 NOT NULL,
    CONSTRAINT `PRIMARY` PRIMARY KEY (`SettingName`)
);

CREATE TABLE `StaticStrings` (
    `Id` varchar(50) CHARACTER SET utf8 NOT NULL,
    `Text` text CHARACTER SET utf8 NOT NULL,
    CONSTRAINT `PK_StaticStrings` PRIMARY KEY (`Id`)
);

CREATE TABLE `TimeZoneInfos` (
    `Id` varchar(100) CHARACTER SET utf8 NOT NULL,
    `Display` varchar(1000) CHARACTER SET utf8 NOT NULL,
    `Order` decimal(18,2) NOT NULL,
    CONSTRAINT `PK_TimeZoneInfos` PRIMARY KEY (`Id`)
);

CREATE TABLE `TraitDecorators` (
    `Id` bigint(20) NOT NULL AUTO_INCREMENT,
    `Name` varchar(255) CHARACTER SET utf8 NOT NULL,
    `Type` varchar(255) CHARACTER SET utf8 NOT NULL,
    `Contents` text CHARACTER SET utf8 NULL,
    CONSTRAINT `PK_TraitDecorators` PRIMARY KEY (`Id`)
);

CREATE TABLE `TraitExpression` (
    `Id` bigint(20) NOT NULL AUTO_INCREMENT,
    `Expression` varchar(4000) CHARACTER SET utf8 NOT NULL,
    `Name` varchar(200) CHARACTER SET utf8 NOT NULL DEFAULT 'Unnamed Expression',
    CONSTRAINT `PK_TraitExpression` PRIMARY KEY (`Id`)
);

CREATE TABLE `UnitOfMeasure` (
    `Id` bigint(20) NOT NULL AUTO_INCREMENT,
    `Name` varchar(50) CHARACTER SET utf8 NOT NULL,
    `PrimaryAbbreviation` varchar(45) CHARACTER SET utf8 NULL,
    `Abbreviations` varchar(4000) CHARACTER SET utf8 NOT NULL,
    `BaseMultiplier` double NOT NULL,
    `PreMultiplierBaseOffset` double NOT NULL,
    `PostMultiplierBaseOffset` double NOT NULL,
    `Type` int(11) NOT NULL,
    `Describer` bit(1) NOT NULL,
    `SpaceBetween` bit(1) NOT NULL DEFAULT b'1',
    `System` varchar(50) CHARACTER SET utf8 NOT NULL,
    `DefaultUnitForSystem` bit(1) NOT NULL DEFAULT b'0',
    CONSTRAINT `PK_UnitOfMeasure` PRIMARY KEY (`Id`)
);

CREATE TABLE `VariableDefaults` (
    `OwnerType` bigint(20) NOT NULL,
    `Property` varchar(50) CHARACTER SET utf8 NOT NULL,
    `DefaultValue` varchar(4000) CHARACTER SET utf8 NOT NULL,
    CONSTRAINT `PRIMARY` PRIMARY KEY (`OwnerType`, `Property`)
);

CREATE TABLE `VariableDefinitions` (
    `OwnerType` bigint(20) NOT NULL,
    `Property` varchar(50) CHARACTER SET utf8 NOT NULL,
    `ContainedType` bigint(20) NOT NULL,
    CONSTRAINT `PRIMARY` PRIMARY KEY (`OwnerType`, `Property`)
);

CREATE TABLE `VariableValues` (
    `ReferenceType` bigint(20) NOT NULL,
    `ReferenceId` bigint(20) NOT NULL,
    `ReferenceProperty` varchar(50) CHARACTER SET utf8 NOT NULL,
    `ValueDefinition` varchar(4000) CHARACTER SET utf8 NOT NULL,
    `ValueType` bigint(20) NOT NULL,
    CONSTRAINT `PRIMARY` PRIMARY KEY (`ReferenceType`, `ReferenceId`, `ReferenceProperty`)
);

CREATE TABLE `WearableSizes` (
    `Id` bigint(20) NOT NULL AUTO_INCREMENT,
    `OneSizeFitsAll` bit(1) NOT NULL,
    `Height` double NULL,
    `Weight` double NULL,
    `TraitValue` double NULL,
    `BodyPrototypeId` bigint(20) NOT NULL,
    CONSTRAINT `PK_WearableSizes` PRIMARY KEY (`Id`)
);

CREATE TABLE `WearProfiles` (
    `Id` bigint(20) NOT NULL AUTO_INCREMENT,
    `Name` varchar(255) CHARACTER SET utf8 NOT NULL,
    `BodyPrototypeId` bigint(20) NOT NULL,
    `WearStringInventory` varchar(255) CHARACTER SET utf8 NOT NULL DEFAULT 'worn on',
    `WearAction1st` varchar(255) CHARACTER SET utf8 NOT NULL DEFAULT 'put',
    `WearAction3rd` varchar(255) CHARACTER SET utf8 NOT NULL DEFAULT 'puts',
    `WearAffix` varchar(255) CHARACTER SET utf8 NOT NULL DEFAULT 'on',
    `WearlocProfiles` text CHARACTER SET utf8 NOT NULL,
    `Type` varchar(50) CHARACTER SET utf8 NOT NULL DEFAULT 'Direct',
    `Description` text CHARACTER SET utf8 NULL,
    `RequireContainerIsEmpty` bit(1) NOT NULL DEFAULT b'0',
    CONSTRAINT `PK_WearProfiles` PRIMARY KEY (`Id`)
);

CREATE TABLE `WeatherEvents` (
    `Id` bigint(20) NOT NULL AUTO_INCREMENT,
    `Name` varchar(200) CHARACTER SET utf8 NOT NULL,
    `WeatherEventType` varchar(45) CHARACTER SET utf8 NOT NULL,
    `WeatherDescription` text CHARACTER SET utf8 NOT NULL,
    `WeatherRoomAddendum` text CHARACTER SET utf8 NOT NULL,
    `TemperatureEffect` double NOT NULL,
    `Precipitation` int(11) NOT NULL,
    `Wind` int(11) NOT NULL,
    `AdditionalInfo` text CHARACTER SET utf8 NULL,
    `PrecipitationTemperatureEffect` double NOT NULL,
    `WindTemperatureEffect` double NOT NULL,
    `LightLevelMultiplier` double NOT NULL,
    `ObscuresViewOfSky` bit(1) NOT NULL,
    `PermittedAtNight` bit(1) NOT NULL,
    `PermittedAtDawn` bit(1) NOT NULL,
    `PermittedAtMorning` bit(1) NOT NULL,
    `PermittedAtAfternoon` bit(1) NOT NULL,
    `PermittedAtDusk` bit(1) NOT NULL,
    `CountsAsId` bigint(20) NULL,
    CONSTRAINT `PK_WeatherEvents` PRIMARY KEY (`Id`),
    CONSTRAINT `FK_WeatherEvents_WeatherEvents` FOREIGN KEY (`CountsAsId`) REFERENCES `WeatherEvents` (`Id`) ON DELETE SET NULL
);

CREATE TABLE `Accounts` (
    `Id` bigint(20) NOT NULL AUTO_INCREMENT,
    `Name` varchar(4000) CHARACTER SET utf8 NOT NULL,
    `Password` varchar(1000) CHARACTER SET utf8mb4 NULL,
    `Salt` bigint(20) NOT NULL,
    `AccessStatus` int(11) NOT NULL,
    `AuthorityGroupId` bigint(20) NULL DEFAULT '0',
    `Email` varchar(4000) CHARACTER SET utf8 NULL,
    `LastLoginTime` datetime NULL,
    `LastLoginIP` varchar(50) CHARACTER SET utf8 NULL,
    `FormatLength` int(11) NOT NULL DEFAULT '110',
    `InnerFormatLength` int(11) NOT NULL DEFAULT '80',
    `UseMXP` bit(1) NOT NULL DEFAULT b'0',
    `UseMSP` bit(1) NOT NULL DEFAULT b'0',
    `UseMCCP` bit(1) NOT NULL DEFAULT b'0',
    `ActiveCharactersAllowed` int(11) NOT NULL DEFAULT '1',
    `UseUnicode` bit(1) NOT NULL DEFAULT b'0',
    `TimeZoneId` varchar(100) CHARACTER SET utf8 NOT NULL,
    `CultureName` varchar(50) CHARACTER SET utf8 NOT NULL,
    `RegistrationCode` varchar(50) CHARACTER SET utf8 NULL,
    `IsRegistered` bit(1) NOT NULL DEFAULT b'0',
    `RecoveryCode` varchar(4000) CHARACTER SET utf8 NULL,
    `UnitPreference` varchar(50) CHARACTER SET utf8 NOT NULL,
    `CreationDate` datetime NOT NULL,
    `PageLength` int(11) NOT NULL DEFAULT '22',
    `PromptType` int(11) NOT NULL,
    `TabRoomDescriptions` bit(1) NOT NULL DEFAULT b'1',
    `CodedRoomDescriptionAdditionsOnNewLine` bit(1) NOT NULL DEFAULT b'1',
    `CharacterNameOverlaySetting` int(11) NOT NULL,
    `AppendNewlinesBetweenMultipleEchoesPerPrompt` bit(1) NOT NULL DEFAULT b'1',
    CONSTRAINT `PK_Accounts` PRIMARY KEY (`Id`),
    CONSTRAINT `FK_Accounts_AuthorityGroups` FOREIGN KEY (`AuthorityGroupId`) REFERENCES `AuthorityGroups` (`Id`) ON DELETE RESTRICT
);

CREATE TABLE `BloodModels_Bloodtypes` (
    `BloodModelId` bigint(20) NOT NULL,
    `BloodtypeId` bigint(20) NOT NULL,
    CONSTRAINT `PRIMARY` PRIMARY KEY (`BloodModelId`, `BloodtypeId`),
    CONSTRAINT `FK_BloodModels_Bloodtypes_BloodModels` FOREIGN KEY (`BloodModelId`) REFERENCES `BloodModels` (`Id`) ON DELETE CASCADE,
    CONSTRAINT `FK_BloodModels_Bloodtypes_Bloodtypes` FOREIGN KEY (`BloodtypeId`) REFERENCES `Bloodtypes` (`Id`) ON DELETE CASCADE
);

CREATE TABLE `Bloodtypes_BloodtypeAntigens` (
    `BloodtypeId` bigint(20) NOT NULL,
    `BloodtypeAntigenId` bigint(20) NOT NULL,
    CONSTRAINT `PRIMARY` PRIMARY KEY (`BloodtypeId`, `BloodtypeAntigenId`),
    CONSTRAINT `FK_Bloodtypes_BloodtypeAntigens_BloodtypeAntigens` FOREIGN KEY (`BloodtypeAntigenId`) REFERENCES `BloodtypeAntigens` (`Id`) ON DELETE CASCADE,
    CONSTRAINT `FK_Bloodtypes_BloodtypeAntigens_Bloodtypes` FOREIGN KEY (`BloodtypeId`) REFERENCES `Bloodtypes` (`Id`) ON DELETE CASCADE
);

CREATE TABLE `BodypartGroupDescribers_ShapeCount` (
    `BodypartGroupDescriptionRuleId` bigint(20) NOT NULL,
    `TargetId` bigint(20) NOT NULL,
    `MinCount` int(11) NOT NULL,
    `MaxCount` int(11) NOT NULL,
    CONSTRAINT `PRIMARY` PRIMARY KEY (`BodypartGroupDescriptionRuleId`, `TargetId`),
    CONSTRAINT `FK_BGD_ShapeCount_BodypartGroupDescribers` FOREIGN KEY (`BodypartGroupDescriptionRuleId`) REFERENCES `BodypartGroupDescribers` (`Id`) ON DELETE RESTRICT,
    CONSTRAINT `FK_BGD_ShapeCount_BodypartShape` FOREIGN KEY (`TargetId`) REFERENCES `BodypartShape` (`Id`) ON DELETE RESTRICT
);

CREATE TABLE `Seasons` (
    `Id` bigint(20) NOT NULL AUTO_INCREMENT,
    `Name` varchar(200) CHARACTER SET utf8 NOT NULL,
    `Description` text CHARACTER SET utf8 NOT NULL,
    `CelestialDayOnset` int(11) NOT NULL,
    `CelestialId` bigint(20) NOT NULL,
    CONSTRAINT `PK_Seasons` PRIMARY KEY (`Id`),
    CONSTRAINT `FK_Seasons_Celestials` FOREIGN KEY (`CelestialId`) REFERENCES `Celestials` (`Id`) ON DELETE RESTRICT
);

CREATE TABLE `CharacteristicProfiles` (
    `Id` bigint(20) NOT NULL AUTO_INCREMENT,
    `Name` varchar(50) CHARACTER SET utf8 NOT NULL,
    `Definition` text CHARACTER SET utf8 NOT NULL,
    `Type` varchar(50) CHARACTER SET utf8 NOT NULL,
    `TargetDefinitionId` bigint(20) NOT NULL,
    `Description` varchar(4000) CHARACTER SET utf8 NOT NULL,
    CONSTRAINT `PK_CharacteristicProfiles` PRIMARY KEY (`Id`),
    CONSTRAINT `FK_CharacteristicProfiles_CharacteristicDefinitions` FOREIGN KEY (`TargetDefinitionId`) REFERENCES `CharacteristicDefinitions` (`Id`) ON DELETE CASCADE
);

CREATE TABLE `CheckTemplateDifficulties` (
    `CheckTemplateId` bigint(20) NOT NULL,
    `Difficulty` int(11) NOT NULL,
    `Modifier` double NOT NULL,
    CONSTRAINT `PRIMARY` PRIMARY KEY (`Difficulty`, `CheckTemplateId`),
    CONSTRAINT `FK_CheckTemplateDifficulties_CheckTemplates` FOREIGN KEY (`CheckTemplateId`) REFERENCES `CheckTemplates` (`Id`) ON DELETE RESTRICT
);

CREATE TABLE `Timezones` (
    `Id` bigint(20) NOT NULL AUTO_INCREMENT,
    `Name` varchar(255) CHARACTER SET utf8 NOT NULL,
    `Description` varchar(4000) CHARACTER SET utf8 NOT NULL,
    `OffsetMinutes` int(11) NOT NULL,
    `OffsetHours` int(11) NOT NULL,
    `ClockId` bigint(20) NOT NULL,
    CONSTRAINT `PK_Timezones` PRIMARY KEY (`Id`),
    CONSTRAINT `FK_Timezones_Clocks` FOREIGN KEY (`ClockId`) REFERENCES `Clocks` (`Id`) ON DELETE RESTRICT
);

CREATE TABLE `Coins` (
    `Id` bigint(20) NOT NULL AUTO_INCREMENT,
    `Name` varchar(4000) CHARACTER SET utf8 NOT NULL,
    `ShortDescription` varchar(4000) CHARACTER SET utf8 NOT NULL,
    `FullDescription` varchar(4000) CHARACTER SET utf8 NOT NULL,
    `Value` decimal(18,0) NOT NULL,
    `CurrencyId` bigint(20) NOT NULL,
    `Weight` double NOT NULL,
    `GeneralForm` varchar(4000) CHARACTER SET utf8 NOT NULL,
    `PluralWord` varchar(4000) CHARACTER SET utf8 NULL,
    CONSTRAINT `PK_Coins` PRIMARY KEY (`Id`),
    CONSTRAINT `FK_Coins_Currencies` FOREIGN KEY (`CurrencyId`) REFERENCES `Currencies` (`Id`) ON DELETE CASCADE
);

CREATE TABLE `CurrencyDivisions` (
    `Id` bigint(20) NOT NULL AUTO_INCREMENT,
    `Name` varchar(4000) CHARACTER SET utf8 NOT NULL,
    `BaseUnitConversionRate` decimal(18,4) NOT NULL,
    `CurrencyId` bigint(20) NOT NULL,
    CONSTRAINT `PK_CurrencyDivisions` PRIMARY KEY (`Id`),
    CONSTRAINT `FK_CurrencyDivisions_Currencies` FOREIGN KEY (`CurrencyId`) REFERENCES `Currencies` (`Id`) ON DELETE CASCADE
);

CREATE TABLE `LegalAuthorities` (
    `Id` bigint(20) NOT NULL AUTO_INCREMENT,
    `Name` varchar(250) CHARACTER SET utf8 NOT NULL,
    `CurrencyId` bigint(20) NOT NULL,
    CONSTRAINT `PK_LegalAuthorities` PRIMARY KEY (`Id`),
    CONSTRAINT `FK_LegalAuthorities_Currencies` FOREIGN KEY (`CurrencyId`) REFERENCES `Currencies` (`Id`) ON DELETE CASCADE
);

CREATE TABLE `DrugsIntensities` (
    `DrugId` bigint(20) NOT NULL AUTO_INCREMENT,
    `DrugType` int(11) NOT NULL,
    `RelativeIntensity` double NOT NULL,
    `AdditionalEffects` text CHARACTER SET utf8mb4 NULL,
    CONSTRAINT `PRIMARY` PRIMARY KEY (`DrugId`, `DrugType`),
    CONSTRAINT `FK_Drugs_DrugIntensities` FOREIGN KEY (`DrugId`) REFERENCES `Drugs` (`Id`) ON DELETE CASCADE
);

CREATE TABLE `CellOverlayPackages` (
    `Id` bigint(20) NOT NULL,
    `RevisionNumber` int(11) NOT NULL,
    `Name` varchar(4000) CHARACTER SET utf8 NOT NULL,
    `EditableItemId` bigint(20) NOT NULL,
    CONSTRAINT `PRIMARY` PRIMARY KEY (`Id`, `RevisionNumber`),
    CONSTRAINT `FK_CellOverlayPackages_EditableItems` FOREIGN KEY (`EditableItemId`) REFERENCES `EditableItems` (`Id`) ON DELETE RESTRICT
);

CREATE TABLE `DisfigurementTemplates` (
    `Id` bigint(20) NOT NULL,
    `RevisionNumber` int(11) NOT NULL,
    `Name` varchar(500) CHARACTER SET utf8 NOT NULL,
    `Type` varchar(50) CHARACTER SET utf8 NOT NULL,
    `EditableItemId` bigint(20) NOT NULL,
    `ShortDescription` varchar(500) CHARACTER SET utf8 NOT NULL,
    `FullDescription` varchar(5000) CHARACTER SET utf8 NOT NULL,
    `Definition` text CHARACTER SET utf8 NOT NULL,
    CONSTRAINT `PRIMARY` PRIMARY KEY (`Id`, `RevisionNumber`),
    CONSTRAINT `FK_DisfigurementTemplates_EditableItems` FOREIGN KEY (`EditableItemId`) REFERENCES `EditableItems` (`Id`) ON DELETE CASCADE
);

CREATE TABLE `ForagableProfiles` (
    `Id` bigint(20) NOT NULL,
    `RevisionNumber` int(11) NOT NULL,
    `EditableItemId` bigint(20) NOT NULL,
    `Name` varchar(200) CHARACTER SET utf8mb4 NOT NULL,
    CONSTRAINT `PRIMARY` PRIMARY KEY (`Id`, `RevisionNumber`),
    CONSTRAINT `FK_ForagableProfiles_EditableItems` FOREIGN KEY (`EditableItemId`) REFERENCES `EditableItems` (`Id`) ON DELETE CASCADE
);

CREATE TABLE `Foragables` (
    `Id` bigint(20) NOT NULL,
    `RevisionNumber` int(11) NOT NULL,
    `Name` varchar(200) CHARACTER SET utf8mb4 NOT NULL,
    `ForagableTypes` text CHARACTER SET utf8mb4 NOT NULL,
    `ForageDifficulty` int(11) NOT NULL,
    `RelativeChance` int(11) NOT NULL,
    `MinimumOutcome` int(11) NOT NULL,
    `MaximumOutcome` int(11) NOT NULL,
    `QuantityDiceExpression` varchar(200) CHARACTER SET utf8mb4 NOT NULL,
    `ItemProtoId` bigint(20) NOT NULL,
    `OnForageProgId` bigint(20) NULL,
    `CanForageProgId` bigint(20) NULL,
    `EditableItemId` bigint(20) NOT NULL,
    CONSTRAINT `PRIMARY` PRIMARY KEY (`Id`, `RevisionNumber`),
    CONSTRAINT `FK_Foragables_EditableItems` FOREIGN KEY (`EditableItemId`) REFERENCES `EditableItems` (`Id`) ON DELETE CASCADE
);

CREATE TABLE `GameItemComponentProtos` (
    `Id` bigint(20) NOT NULL,
    `RevisionNumber` int(11) NOT NULL,
    `Type` varchar(50) CHARACTER SET utf8 NOT NULL,
    `Definition` text CHARACTER SET utf8 NOT NULL,
    `EditableItemId` bigint(20) NOT NULL,
    `Description` text CHARACTER SET utf8 NOT NULL,
    `Name` varchar(4000) CHARACTER SET utf8 NOT NULL,
    CONSTRAINT `PRIMARY` PRIMARY KEY (`Id`, `RevisionNumber`),
    CONSTRAINT `FK_GameItemComponentProtos_EditableItems` FOREIGN KEY (`EditableItemId`) REFERENCES `EditableItems` (`Id`) ON DELETE RESTRICT
);

CREATE TABLE `NPCTemplates` (
    `Id` bigint(20) NOT NULL,
    `RevisionNumber` int(11) NOT NULL,
    `Type` varchar(255) CHARACTER SET utf8 NOT NULL,
    `Name` varchar(255) CHARACTER SET utf8 NOT NULL,
    `Definition` text CHARACTER SET utf8 NOT NULL,
    `EditableItemId` bigint(20) NOT NULL,
    CONSTRAINT `PRIMARY` PRIMARY KEY (`Id`, `RevisionNumber`),
    CONSTRAINT `FK_NPCTemplates_EditableItems` FOREIGN KEY (`EditableItemId`) REFERENCES `EditableItems` (`Id`) ON DELETE RESTRICT
);

CREATE TABLE `Projects` (
    `Id` bigint(20) NOT NULL,
    `RevisionNumber` int(11) NOT NULL,
    `Type` varchar(45) CHARACTER SET utf8 NOT NULL,
    `Name` varchar(200) CHARACTER SET utf8 NOT NULL,
    `EditableItemId` bigint(20) NOT NULL,
    `Definition` text CHARACTER SET utf8 NULL,
    CONSTRAINT `PRIMARY` PRIMARY KEY (`Id`, `RevisionNumber`),
    CONSTRAINT `FK_Projects_EditableItems` FOREIGN KEY (`EditableItemId`) REFERENCES `EditableItems` (`Id`) ON DELETE CASCADE
);

CREATE TABLE `Channels` (
    `Id` bigint(20) NOT NULL AUTO_INCREMENT,
    `ChannelName` varchar(4000) CHARACTER SET utf8 NOT NULL,
    `ChannelListenerProgId` bigint(20) NOT NULL,
    `ChannelSpeakerProgId` bigint(20) NOT NULL,
    `AnnounceChannelJoiners` bit(1) NOT NULL,
    `ChannelColour` char(10) CHARACTER SET utf8 NOT NULL,
    `Mode` int(11) NOT NULL,
    `AnnounceMissedListeners` bit(1) NOT NULL,
    `AddToPlayerCommandTree` bit(1) NOT NULL DEFAULT b'0',
    `AddToGuideCommandTree` bit(1) NOT NULL DEFAULT b'0',
    CONSTRAINT `PK_Channels` PRIMARY KEY (`Id`),
    CONSTRAINT `FK_Channels_FutureProgs_Listener` FOREIGN KEY (`ChannelListenerProgId`) REFERENCES `FutureProgs` (`Id`) ON DELETE RESTRICT,
    CONSTRAINT `FK_Channels_FutureProgs_Speaker` FOREIGN KEY (`ChannelSpeakerProgId`) REFERENCES `FutureProgs` (`Id`) ON DELETE RESTRICT
);

CREATE TABLE `CharacterIntroTemplates` (
    `Id` bigint(20) NOT NULL AUTO_INCREMENT,
    `Name` varchar(200) CHARACTER SET utf8 NOT NULL,
    `ResolutionPriority` int(11) NOT NULL DEFAULT '1',
    `AppliesToCharacterProgId` bigint(20) NOT NULL,
    `Definition` text CHARACTER SET utf8 NOT NULL,
    `Order` int(11) NOT NULL,
    CONSTRAINT `PK_CharacterIntroTemplates` PRIMARY KEY (`Id`),
    CONSTRAINT `FK_CharacterIntroTemplates_FutureProgs` FOREIGN KEY (`AppliesToCharacterProgId`) REFERENCES `FutureProgs` (`Id`) ON DELETE RESTRICT
);

CREATE TABLE `CharacteristicValues` (
    `Id` bigint(20) NOT NULL AUTO_INCREMENT,
    `Name` varchar(4000) CHARACTER SET utf8 NOT NULL,
    `DefinitionId` bigint(20) NOT NULL,
    `Value` varchar(50) CHARACTER SET utf8 NULL,
    `Default` bit(1) NOT NULL DEFAULT b'0',
    `AdditionalValue` varchar(4000) CHARACTER SET utf8 NULL,
    `FutureProgId` bigint(20) NULL,
    `Pluralisation` int(11) NOT NULL,
    CONSTRAINT `PK_CharacteristicValues` PRIMARY KEY (`Id`),
    CONSTRAINT `FK_CharacteristicValues_CharacteristicDefinitions` FOREIGN KEY (`DefinitionId`) REFERENCES `CharacteristicDefinitions` (`Id`) ON DELETE CASCADE,
    CONSTRAINT `FK_CharacteristicValues_FutureProgs` FOREIGN KEY (`FutureProgId`) REFERENCES `FutureProgs` (`Id`) ON DELETE SET NULL
);

CREATE TABLE `ChargenAdvices` (
    `Id` bigint(20) NOT NULL AUTO_INCREMENT,
    `ChargenStage` int(11) NOT NULL,
    `AdviceTitle` text CHARACTER SET utf8 NOT NULL,
    `AdviceText` text CHARACTER SET utf8 NOT NULL,
    `ShouldShowAdviceProgId` bigint(20) NULL,
    CONSTRAINT `PK_ChargenAdvices` PRIMARY KEY (`Id`),
    CONSTRAINT `FK_ChargenAdvices_FutureProgs` FOREIGN KEY (`ShouldShowAdviceProgId`) REFERENCES `FutureProgs` (`Id`) ON DELETE SET NULL
);

CREATE TABLE `CombatMessages` (
    `Id` bigint(20) NOT NULL AUTO_INCREMENT,
    `Type` int(11) NOT NULL,
    `Outcome` int(11) NULL,
    `Message` text CHARACTER SET utf8mb4 NOT NULL,
    `ProgId` bigint(20) NULL,
    `Priority` int(11) NOT NULL,
    `Verb` int(11) NULL,
    `Chance` double NOT NULL DEFAULT '1',
    `FailureMessage` text CHARACTER SET utf8mb4 NOT NULL,
    CONSTRAINT `PK_CombatMessages` PRIMARY KEY (`Id`),
    CONSTRAINT `FK_CombatMessages_FutureProgs` FOREIGN KEY (`ProgId`) REFERENCES `FutureProgs` (`Id`) ON DELETE SET NULL
);

CREATE TABLE `CurrencyDescriptionPatterns` (
    `Id` bigint(20) NOT NULL AUTO_INCREMENT,
    `Type` int(11) NOT NULL,
    `CurrencyId` bigint(20) NOT NULL,
    `FutureProgId` bigint(20) NULL,
    `NegativePrefix` varchar(4000) CHARACTER SET utf8 NOT NULL,
    `Order` int(11) NOT NULL,
    CONSTRAINT `PK_CurrencyDescriptionPatterns` PRIMARY KEY (`Id`),
    CONSTRAINT `FK_CurrencyDescriptionPatterns_Currencies` FOREIGN KEY (`CurrencyId`) REFERENCES `Currencies` (`Id`) ON DELETE CASCADE,
    CONSTRAINT `FK_CurrencyDescriptionPatterns_FutureProgs` FOREIGN KEY (`FutureProgId`) REFERENCES `FutureProgs` (`Id`) ON DELETE CASCADE
);

CREATE TABLE `Dreams` (
    `Id` bigint(20) NOT NULL AUTO_INCREMENT,
    `Name` varchar(200) CHARACTER SET utf8mb4 NOT NULL,
    `CanDreamProgId` bigint(20) NULL,
    `OnDreamProgId` bigint(20) NULL,
    `OnWakeDuringDreamingProgId` bigint(20) NULL,
    `OnlyOnce` bit(1) NOT NULL DEFAULT b'0',
    `Priority` int(11) NOT NULL DEFAULT '100',
    CONSTRAINT `PK_Dreams` PRIMARY KEY (`Id`),
    CONSTRAINT `FK_Dreams_FutureProgs_CanDream` FOREIGN KEY (`CanDreamProgId`) REFERENCES `FutureProgs` (`Id`) ON DELETE SET NULL,
    CONSTRAINT `FK_Dreams_FutureProgs_OnDream` FOREIGN KEY (`OnDreamProgId`) REFERENCES `FutureProgs` (`Id`) ON DELETE SET NULL,
    CONSTRAINT `FK_Dreams_FutureProgs_OnWake` FOREIGN KEY (`OnWakeDuringDreamingProgId`) REFERENCES `FutureProgs` (`Id`) ON DELETE SET NULL
);

CREATE TABLE `EntityDescriptionPatterns` (
    `Id` bigint(20) NOT NULL AUTO_INCREMENT,
    `Pattern` varchar(4000) CHARACTER SET utf8 NOT NULL,
    `Type` int(11) NOT NULL,
    `ApplicabilityProgId` bigint(20) NULL,
    `RelativeWeight` int(11) NOT NULL,
    CONSTRAINT `PK_EntityDescriptionPatterns` PRIMARY KEY (`Id`),
    CONSTRAINT `FK_EntityDescriptionPatterns_FutureProgs` FOREIGN KEY (`ApplicabilityProgId`) REFERENCES `FutureProgs` (`Id`) ON DELETE RESTRICT
);

CREATE TABLE `FutureProgs_Parameters` (
    `FutureProgId` bigint(20) NOT NULL,
    `ParameterIndex` int(11) NOT NULL,
    `ParameterType` bigint(20) NOT NULL,
    `ParameterName` varchar(4000) CHARACTER SET utf8 NOT NULL,
    CONSTRAINT `PRIMARY` PRIMARY KEY (`FutureProgId`, `ParameterIndex`),
    CONSTRAINT `FK_FutureProgs_Parameters_FutureProgs` FOREIGN KEY (`FutureProgId`) REFERENCES `FutureProgs` (`Id`) ON DELETE CASCADE
);

CREATE TABLE `Helpfiles` (
    `Id` bigint(20) NOT NULL AUTO_INCREMENT,
    `Name` varchar(255) CHARACTER SET utf8 NOT NULL,
    `Category` varchar(255) CHARACTER SET utf8 NOT NULL,
    `Subcategory` varchar(255) CHARACTER SET utf8 NOT NULL,
    `TagLine` varchar(500) CHARACTER SET utf8 NOT NULL,
    `PublicText` text CHARACTER SET utf8 NOT NULL,
    `RuleId` bigint(20) NULL,
    `Keywords` varchar(500) CHARACTER SET utf8 NOT NULL,
    `LastEditedBy` varchar(100) CHARACTER SET utf8 NOT NULL,
    `LastEditedDate` datetime NOT NULL,
    CONSTRAINT `PK_Helpfiles` PRIMARY KEY (`Id`),
    CONSTRAINT `FK_Helpfiles_FutureProgs` FOREIGN KEY (`RuleId`) REFERENCES `FutureProgs` (`Id`) ON DELETE RESTRICT
);

CREATE TABLE `knowledges` (
    `Id` bigint(20) NOT NULL AUTO_INCREMENT,
    `Name` varchar(200) CHARACTER SET utf8 NOT NULL,
    `Description` text CHARACTER SET utf8 NOT NULL,
    `LongDescription` text CHARACTER SET utf8 NOT NULL,
    `Type` varchar(200) CHARACTER SET utf8 NOT NULL,
    `Subtype` varchar(200) CHARACTER SET utf8 NOT NULL,
    `LearnableType` int(11) NOT NULL,
    `LearnDifficulty` int(11) NOT NULL DEFAULT '7',
    `TeachDifficulty` int(11) NOT NULL DEFAULT '7',
    `LearningSessionsRequired` int(11) NOT NULL,
    `CanAcquireProgId` bigint(20) NULL,
    `CanLearnProgId` bigint(20) NULL,
    CONSTRAINT `PK_knowledges` PRIMARY KEY (`Id`),
    CONSTRAINT `FK_KNOWLEDGES_FUTUREPROGS_ACQUIRE` FOREIGN KEY (`CanAcquireProgId`) REFERENCES `FutureProgs` (`Id`) ON DELETE SET NULL,
    CONSTRAINT `FK_KNOWLEDGES_FUTUREPROGS_LEARN` FOREIGN KEY (`CanLearnProgId`) REFERENCES `FutureProgs` (`Id`) ON DELETE SET NULL
);

CREATE TABLE `ProgSchedules` (
    `Id` bigint(20) NOT NULL AUTO_INCREMENT,
    `Name` varchar(200) CHARACTER SET utf8mb4 NOT NULL,
    `IntervalType` int(11) NOT NULL,
    `IntervalModifier` int(11) NOT NULL,
    `IntervalOther` int(11) NOT NULL,
    `ReferenceTime` varchar(500) CHARACTER SET utf8mb4 NOT NULL,
    `ReferenceDate` varchar(500) CHARACTER SET utf8mb4 NOT NULL,
    `FutureProgId` bigint(20) NOT NULL,
    CONSTRAINT `PK_ProgSchedules` PRIMARY KEY (`Id`),
    CONSTRAINT `FK_ProgSchedules_FutureProgs` FOREIGN KEY (`FutureProgId`) REFERENCES `FutureProgs` (`Id`) ON DELETE CASCADE
);

CREATE TABLE `Socials` (
    `Name` varchar(100) CHARACTER SET utf8 NOT NULL,
    `NoTargetEcho` varchar(1000) CHARACTER SET utf8 NOT NULL,
    `OneTargetEcho` varchar(1000) CHARACTER SET utf8 NOT NULL,
    `FutureProgId` bigint(20) NULL,
    `DirectionTargetEcho` varchar(1000) CHARACTER SET utf8 NULL,
    `MultiTargetEcho` varchar(1000) CHARACTER SET utf8 NULL,
    CONSTRAINT `PRIMARY` PRIMARY KEY (`Name`),
    CONSTRAINT `FK_Socials_FutureProgs` FOREIGN KEY (`FutureProgId`) REFERENCES `FutureProgs` (`Id`) ON DELETE SET NULL
);

CREATE TABLE `Tags` (
    `Id` bigint(20) NOT NULL AUTO_INCREMENT,
    `Name` varchar(255) CHARACTER SET utf8 NOT NULL,
    `ParentId` bigint(20) NULL,
    `ShouldSeeProgId` bigint(20) NULL,
    CONSTRAINT `PK_Tags` PRIMARY KEY (`Id`),
    CONSTRAINT `FK_Tags_Parent` FOREIGN KEY (`ParentId`) REFERENCES `Tags` (`Id`) ON DELETE SET NULL,
    CONSTRAINT `FK_Tags_Futureprogs` FOREIGN KEY (`ShouldSeeProgId`) REFERENCES `FutureProgs` (`Id`) ON DELETE SET NULL
);

CREATE TABLE `WitnessProfiles` (
    `Id` bigint(20) NOT NULL AUTO_INCREMENT,
    `Name` varchar(250) CHARACTER SET utf8 NOT NULL,
    `IdentityKnownProgId` bigint(20) NOT NULL,
    `ReportingMultiplierProgId` bigint(20) NOT NULL,
    `ReportingReliability` double NOT NULL,
    `MinimumSkillToDetermineTimeOfDay` double NOT NULL,
    `MinimumSkillToDetermineBiases` double NOT NULL,
    `BaseReportingChanceNight` double NOT NULL,
    `BaseReportingChanceDawn` double NOT NULL,
    `BaseReportingChanceMorning` double NOT NULL,
    `BaseReportingChanceAfternoon` double NOT NULL,
    `BaseReportingChanceDusk` double NOT NULL,
    CONSTRAINT `PK_WitnessProfiles` PRIMARY KEY (`Id`),
    CONSTRAINT `FK_WitnessProfiles_IdentityProg` FOREIGN KEY (`IdentityKnownProgId`) REFERENCES `FutureProgs` (`Id`) ON DELETE RESTRICT,
    CONSTRAINT `FK_WitnessProfiles_MultiplierProg` FOREIGN KEY (`ReportingMultiplierProgId`) REFERENCES `FutureProgs` (`Id`) ON DELETE RESTRICT
);

CREATE TABLE `GameItemComponents` (
    `Id` bigint(20) NOT NULL AUTO_INCREMENT,
    `GameItemComponentProtoId` bigint(20) NOT NULL,
    `GameItemComponentProtoRevision` int(11) NOT NULL,
    `Definition` text CHARACTER SET utf8 NOT NULL,
    `GameItemId` bigint(20) NOT NULL,
    CONSTRAINT `PK_GameItemComponents` PRIMARY KEY (`Id`),
    CONSTRAINT `FK_GameItemComponents_GameItems` FOREIGN KEY (`GameItemId`) REFERENCES `GameItems` (`Id`) ON DELETE CASCADE
);

CREATE TABLE `GroupAIs` (
    `Id` bigint(20) NOT NULL AUTO_INCREMENT,
    `Name` varchar(200) CHARACTER SET utf8 NOT NULL,
    `GroupAITemplateId` bigint(20) NOT NULL,
    `Data` mediumtext CHARACTER SET utf8 NOT NULL,
    `Definition` mediumtext CHARACTER SET utf8 NOT NULL,
    CONSTRAINT `PK_GroupAIs` PRIMARY KEY (`Id`),
    CONSTRAINT `FK_GroupAIs_GroupAITemplates` FOREIGN KEY (`GroupAITemplateId`) REFERENCES `GroupAITemplates` (`Id`) ON DELETE CASCADE
);

CREATE TABLE `DefaultHooks` (
    `HookId` bigint(20) NOT NULL,
    `PerceivableType` varchar(45) CHARACTER SET utf8mb4 NOT NULL,
    `FutureProgId` bigint(20) NOT NULL,
    CONSTRAINT `PRIMARY` PRIMARY KEY (`HookId`, `PerceivableType`, `FutureProgId`),
    CONSTRAINT `FK_DefaultHooks_Futureprogs` FOREIGN KEY (`FutureProgId`) REFERENCES `FutureProgs` (`Id`) ON DELETE CASCADE,
    CONSTRAINT `FK_DefaultHooks_Hooks` FOREIGN KEY (`HookId`) REFERENCES `Hooks` (`Id`) ON DELETE CASCADE
);

CREATE TABLE `GameItemProtos` (
    `Id` bigint(20) NOT NULL,
    `RevisionNumber` int(11) NOT NULL,
    `Name` varchar(255) CHARACTER SET utf8 NOT NULL,
    `Keywords` text CHARACTER SET utf8 NOT NULL,
    `MaterialId` bigint(20) NOT NULL,
    `EditableItemId` bigint(20) NOT NULL,
    `Size` int(11) NOT NULL,
    `Weight` double NOT NULL,
    `ReadOnly` bit(1) NOT NULL DEFAULT b'0',
    `LongDescription` text CHARACTER SET utf8 NULL,
    `ItemGroupId` bigint(20) NULL,
    `OnDestroyedGameItemProtoId` bigint(20) NULL,
    `HealthStrategyId` bigint(20) NULL,
    `BaseItemQuality` int(11) NOT NULL DEFAULT '5',
    `CustomColour` varchar(45) CHARACTER SET utf8 NULL,
    `HighPriority` bit(1) NOT NULL DEFAULT b'0',
    `MorphGameItemProtoId` bigint(20) NULL,
    `MorphTimeSeconds` int(11) NOT NULL,
    `MorphEmote` varchar(1000) CHARACTER SET utf8 NOT NULL DEFAULT '$0 $?1|morphs into $1|decays into nothing$.',
    `ShortDescription` varchar(1000) CHARACTER SET utf8 NULL,
    `FullDescription` varchar(4000) CHARACTER SET utf8 NULL,
    CONSTRAINT `PRIMARY` PRIMARY KEY (`Id`, `RevisionNumber`),
    CONSTRAINT `FK_GameItemProtos_EditableItems` FOREIGN KEY (`EditableItemId`) REFERENCES `EditableItems` (`Id`) ON DELETE RESTRICT,
    CONSTRAINT `FK_GameItemProtos_ItemGroups` FOREIGN KEY (`ItemGroupId`) REFERENCES `ItemGroups` (`Id`) ON DELETE SET NULL
);

CREATE TABLE `ItemGroupForms` (
    `Id` bigint(20) NOT NULL AUTO_INCREMENT,
    `ItemGroupId` bigint(20) NOT NULL,
    `Type` varchar(45) CHARACTER SET utf8mb4 NOT NULL,
    `Definition` text CHARACTER SET utf8mb4 NOT NULL,
    CONSTRAINT `PK_ItemGroupForms` PRIMARY KEY (`Id`),
    CONSTRAINT `FK_ItemGroupForms_ItemGroups` FOREIGN KEY (`ItemGroupId`) REFERENCES `ItemGroups` (`Id`) ON DELETE CASCADE
);

CREATE TABLE `Doors` (
    `Id` bigint(20) NOT NULL AUTO_INCREMENT,
    `Name` varchar(4000) CHARACTER SET utf8 NOT NULL,
    `Style` int(11) NOT NULL,
    `IsOpen` bit(1) NOT NULL,
    `LockedWith` bigint(20) NULL,
    CONSTRAINT `PK_Doors` PRIMARY KEY (`Id`),
    CONSTRAINT `FK_Doors_Locks` FOREIGN KEY (`LockedWith`) REFERENCES `Locks` (`Id`) ON DELETE RESTRICT
);

CREATE TABLE `GameItems_MagicResources` (
    `GameItemId` bigint(20) NOT NULL,
    `MagicResourceId` bigint(20) NOT NULL,
    `Amount` double NOT NULL,
    CONSTRAINT `PRIMARY` PRIMARY KEY (`GameItemId`, `MagicResourceId`),
    CONSTRAINT `FK_GameItems_MagicResources_GameItems` FOREIGN KEY (`GameItemId`) REFERENCES `GameItems` (`Id`) ON DELETE CASCADE,
    CONSTRAINT `FK_GameItems_MagicResources_MagicResources` FOREIGN KEY (`MagicResourceId`) REFERENCES `MagicResources` (`Id`) ON DELETE CASCADE
);

CREATE TABLE `MagicCapabilities` (
    `Id` bigint(20) NOT NULL AUTO_INCREMENT,
    `Name` varchar(200) CHARACTER SET utf8 NOT NULL,
    `CapabilityModel` varchar(45) CHARACTER SET utf8 NOT NULL,
    `PowerLevel` int(11) NOT NULL DEFAULT '1',
    `Definition` text CHARACTER SET utf8 NOT NULL,
    `MagicSchoolId` bigint(20) NOT NULL,
    CONSTRAINT `PK_MagicCapabilities` PRIMARY KEY (`Id`),
    CONSTRAINT `FK_MagicCapabilities_MagicSchools` FOREIGN KEY (`MagicSchoolId`) REFERENCES `MagicSchools` (`Id`) ON DELETE CASCADE
);

CREATE TABLE `MagicPowers` (
    `Id` bigint(20) NOT NULL AUTO_INCREMENT,
    `Name` varchar(200) CHARACTER SET utf8 NOT NULL,
    `Blurb` varchar(500) CHARACTER SET utf8 NOT NULL,
    `ShowHelp` text CHARACTER SET utf8 NOT NULL,
    `PowerModel` varchar(45) CHARACTER SET utf8 NOT NULL,
    `Definition` text CHARACTER SET utf8 NOT NULL,
    `MagicSchoolId` bigint(20) NOT NULL,
    CONSTRAINT `PK_MagicPowers` PRIMARY KEY (`Id`),
    CONSTRAINT `FK_MagicPowers_MagicSchools` FOREIGN KEY (`MagicSchoolId`) REFERENCES `MagicSchools` (`Id`) ON DELETE CASCADE
);

CREATE TABLE `Liquids` (
    `Id` bigint(20) NOT NULL AUTO_INCREMENT,
    `Name` varchar(200) CHARACTER SET utf8 NOT NULL,
    `Description` text CHARACTER SET utf8 NULL,
    `LongDescription` text CHARACTER SET utf8 NULL,
    `TasteText` text CHARACTER SET utf8 NULL,
    `VagueTasteText` text CHARACTER SET utf8 NULL,
    `SmellText` text CHARACTER SET utf8 NULL,
    `VagueSmellText` text CHARACTER SET utf8 NULL,
    `TasteIntensity` double NOT NULL DEFAULT '100',
    `SmellIntensity` double NOT NULL DEFAULT '10',
    `AlcoholLitresPerLitre` double NOT NULL,
    `WaterLitresPerLitre` double NOT NULL DEFAULT '1',
    `FoodSatiatedHoursPerLitre` double NOT NULL,
    `DrinkSatiatedHoursPerLitre` double NOT NULL DEFAULT '12',
    `CaloriesPerLitre` double NOT NULL,
    `Viscosity` double NOT NULL DEFAULT '1',
    `Density` double NOT NULL DEFAULT '1',
    `Organic` bit(1) NOT NULL DEFAULT b'0',
    `ThermalConductivity` double NOT NULL DEFAULT '0.609',
    `ElectricalConductivity` double NOT NULL DEFAULT '0.005',
    `SpecificHeatCapacity` double NOT NULL DEFAULT '4181',
    `IgnitionPoint` double NULL,
    `FreezingPoint` double NULL DEFAULT '273.15',
    `BoilingPoint` double NULL DEFAULT '373.15',
    `DraughtProgId` bigint(20) NULL,
    `SolventId` bigint(20) NULL,
    `CountAsId` bigint(20) NULL,
    `CountAsQuality` int(11) NOT NULL,
    `DisplayColour` varchar(45) CHARACTER SET utf8 NOT NULL DEFAULT 'blue',
    `DampDescription` text CHARACTER SET utf8 NULL,
    `WetDescription` text CHARACTER SET utf8 NULL,
    `DrenchedDescription` text CHARACTER SET utf8 NULL,
    `DampShortDescription` text CHARACTER SET utf8 NULL,
    `WetShortDescription` text CHARACTER SET utf8 NULL,
    `DrenchedShortDescription` text CHARACTER SET utf8 NULL,
    `SolventVolumeRatio` double NOT NULL DEFAULT '1',
    `DriedResidueId` bigint(20) NULL,
    `DrugId` bigint(20) NULL,
    `DrugGramsPerUnitVolume` double NOT NULL,
    `InjectionConsequence` int(11) NOT NULL,
    `ResidueVolumePercentage` double NOT NULL DEFAULT '0.05',
    CONSTRAINT `PK_Liquids` PRIMARY KEY (`Id`),
    CONSTRAINT `FK_Liquids_Liquids_CountasAs` FOREIGN KEY (`CountAsId`) REFERENCES `Liquids` (`Id`) ON DELETE SET NULL,
    CONSTRAINT `FK_Liquids_Materials` FOREIGN KEY (`DriedResidueId`) REFERENCES `Materials` (`Id`) ON DELETE SET NULL,
    CONSTRAINT `FK_Liquids_Drugs` FOREIGN KEY (`DrugId`) REFERENCES `Drugs` (`Id`) ON DELETE SET NULL,
    CONSTRAINT `FK_Liquids_Liquids` FOREIGN KEY (`SolventId`) REFERENCES `Liquids` (`Id`) ON DELETE SET NULL
);

CREATE TABLE `Merits_ChargenResources` (
    `MeritId` bigint(20) NOT NULL,
    `ChargenResourceId` bigint(20) NOT NULL,
    `RequirementOnly` bit(1) NOT NULL DEFAULT b'0',
    `Amount` int(11) NOT NULL,
    CONSTRAINT `PRIMARY` PRIMARY KEY (`MeritId`, `ChargenResourceId`, `RequirementOnly`),
    CONSTRAINT `FK_Merits_ChargenResources_ChargenResources` FOREIGN KEY (`ChargenResourceId`) REFERENCES `ChargenResources` (`Id`) ON DELETE CASCADE,
    CONSTRAINT `FK_Merits_ChargenResources_Merits` FOREIGN KEY (`MeritId`) REFERENCES `Merits` (`Id`) ON DELETE CASCADE
);

CREATE TABLE `Cultures` (
    `Id` bigint(20) NOT NULL AUTO_INCREMENT,
    `Name` varchar(255) CHARACTER SET utf8 NOT NULL,
    `Description` varchar(4000) CHARACTER SET utf8 NOT NULL,
    `NameCultureId` bigint(20) NOT NULL,
    `PersonWordMale` varchar(255) CHARACTER SET utf8 NULL,
    `PersonWordFemale` varchar(255) CHARACTER SET utf8 NULL,
    `PersonWordNeuter` varchar(255) CHARACTER SET utf8 NULL,
    `PersonWordIndeterminate` varchar(255) CHARACTER SET utf8 NOT NULL,
    `PrimaryCalendarId` bigint(20) NOT NULL,
    `SkillStartingValueProgId` bigint(20) NOT NULL,
    `AvailabilityProgId` bigint(20) NULL,
    `TolerableTemperatureFloorEffect` double NOT NULL,
    `TolerableTemperatureCeilingEffect` double NOT NULL,
    CONSTRAINT `PK_Cultures` PRIMARY KEY (`Id`),
    CONSTRAINT `FK_Cultures_AvailabilityProg` FOREIGN KEY (`AvailabilityProgId`) REFERENCES `FutureProgs` (`Id`) ON DELETE SET NULL,
    CONSTRAINT `FK_Cultures_NameCulture` FOREIGN KEY (`NameCultureId`) REFERENCES `NameCulture` (`Id`) ON DELETE RESTRICT,
    CONSTRAINT `FK_Cultures_SkillStartingProg` FOREIGN KEY (`SkillStartingValueProgId`) REFERENCES `FutureProgs` (`Id`) ON DELETE RESTRICT
);

CREATE TABLE `RandomNameProfiles` (
    `Id` bigint(20) NOT NULL AUTO_INCREMENT,
    `Gender` int(11) NOT NULL,
    `NameCultureId` bigint(20) NOT NULL,
    `Name` varchar(255) CHARACTER SET utf8 NOT NULL,
    CONSTRAINT `PK_RandomNameProfiles` PRIMARY KEY (`Id`),
    CONSTRAINT `FK_RandomNameProfiles_NameCulture` FOREIGN KEY (`NameCultureId`) REFERENCES `NameCulture` (`Id`) ON DELETE RESTRICT
);

CREATE TABLE `PopulationBloodModels_Bloodtypes` (
    `BloodtypeId` bigint(20) NOT NULL,
    `PopulationBloodModelId` bigint(20) NOT NULL,
    `Weight` double NOT NULL DEFAULT '1',
    CONSTRAINT `PRIMARY` PRIMARY KEY (`BloodtypeId`, `PopulationBloodModelId`),
    CONSTRAINT `FK_PopulationBloodModels_Bloodtypes_Bloodtypes` FOREIGN KEY (`BloodtypeId`) REFERENCES `Bloodtypes` (`Id`) ON DELETE CASCADE,
    CONSTRAINT `FK_PopulationBloodModels_Bloodtypes_PopulationBloodModels` FOREIGN KEY (`PopulationBloodModelId`) REFERENCES `PopulationBloodModels` (`Id`) ON DELETE CASCADE
);

CREATE TABLE `Shards` (
    `Id` bigint(20) NOT NULL AUTO_INCREMENT,
    `Name` varchar(255) CHARACTER SET utf8 NOT NULL,
    `MinimumTerrestrialLux` double NOT NULL,
    `SkyDescriptionTemplateId` bigint(20) NOT NULL,
    `SphericalRadiusMetres` double NOT NULL DEFAULT '6371000',
    CONSTRAINT `PK_Shards` PRIMARY KEY (`Id`),
    CONSTRAINT `FK_Shards_SkyDescriptionTemplates` FOREIGN KEY (`SkyDescriptionTemplateId`) REFERENCES `SkyDescriptionTemplates` (`Id`) ON DELETE RESTRICT
);

CREATE TABLE `SkyDescriptionTemplates_Values` (
    `SkyDescriptionTemplateId` bigint(20) NOT NULL,
    `LowerBound` double NOT NULL,
    `UpperBound` double NOT NULL,
    `Description` varchar(4000) CHARACTER SET utf8 NOT NULL,
    CONSTRAINT `PRIMARY` PRIMARY KEY (`SkyDescriptionTemplateId`, `LowerBound`),
    CONSTRAINT `FK_SkyDescriptionTemplates_Values_SkyDescriptionTemplates` FOREIGN KEY (`SkyDescriptionTemplateId`) REFERENCES `SkyDescriptionTemplates` (`Id`) ON DELETE CASCADE
);

CREATE TABLE `Checks` (
    `Type` int(11) NOT NULL AUTO_INCREMENT,
    `TraitExpressionId` bigint(20) NOT NULL,
    `CheckTemplateId` bigint(20) NOT NULL,
    `MaximumDifficultyForImprovement` int(11) NOT NULL DEFAULT '10',
    CONSTRAINT `PRIMARY` PRIMARY KEY (`Type`),
    CONSTRAINT `FK_Checks_CheckTemplates` FOREIGN KEY (`CheckTemplateId`) REFERENCES `CheckTemplates` (`Id`) ON DELETE RESTRICT,
    CONSTRAINT `FK_Checks_TraitExpression` FOREIGN KEY (`TraitExpressionId`) REFERENCES `TraitExpression` (`Id`) ON DELETE RESTRICT
);

CREATE TABLE `TraitDefinitions` (
    `Id` bigint(20) NOT NULL AUTO_INCREMENT,
    `Name` varchar(255) CHARACTER SET utf8 NOT NULL,
    `Type` int(11) NOT NULL,
    `DecoratorId` bigint(20) NOT NULL,
    `TraitGroup` varchar(255) CHARACTER SET utf8 NOT NULL,
    `DerivedType` int(11) NOT NULL,
    `ExpressionId` bigint(20) NULL,
    `ImproverId` bigint(20) NULL,
    `Hidden` bit(1) NULL DEFAULT b'0',
    `ChargenBlurb` varchar(4000) CHARACTER SET utf8 NULL,
    `BranchMultiplier` double NOT NULL DEFAULT '1',
    `Alias` varchar(255) CHARACTER SET utf8 NULL,
    `AvailabilityProgId` bigint(20) NULL,
    `TeachableProgId` bigint(20) NULL,
    `LearnableProgId` bigint(20) NULL,
    `TeachDifficulty` int(11) NOT NULL DEFAULT '7',
    `LearnDifficulty` int(11) NOT NULL DEFAULT '7',
    `ValueExpression` text CHARACTER SET utf8 NULL,
    CONSTRAINT `PK_TraitDefinitions` PRIMARY KEY (`Id`),
    CONSTRAINT `FK_TraitDefinitions_AvailabilityProg` FOREIGN KEY (`AvailabilityProgId`) REFERENCES `FutureProgs` (`Id`) ON DELETE SET NULL,
    CONSTRAINT `FK_TraitDefinitions_TraitExpression` FOREIGN KEY (`ExpressionId`) REFERENCES `TraitExpression` (`Id`) ON DELETE RESTRICT,
    CONSTRAINT `FK_TraitDefinitions_LearnableProg` FOREIGN KEY (`LearnableProgId`) REFERENCES `FutureProgs` (`Id`) ON DELETE SET NULL,
    CONSTRAINT `FK_TraitDefinitions_TeachableProg` FOREIGN KEY (`TeachableProgId`) REFERENCES `FutureProgs` (`Id`) ON DELETE SET NULL
);

CREATE TABLE `AccountNotes` (
    `Id` bigint(20) NOT NULL AUTO_INCREMENT,
    `AccountId` bigint(20) NOT NULL,
    `Text` text CHARACTER SET utf8 NOT NULL,
    `Subject` varchar(4000) CHARACTER SET utf8 NOT NULL,
    `TimeStamp` datetime NOT NULL,
    `AuthorId` bigint(20) NULL,
    CONSTRAINT `PK_AccountNotes` PRIMARY KEY (`Id`),
    CONSTRAINT `FK_AccountNotes_Accounts` FOREIGN KEY (`AccountId`) REFERENCES `Accounts` (`Id`) ON DELETE CASCADE,
    CONSTRAINT `FK_AccountNotes_Author` FOREIGN KEY (`AuthorId`) REFERENCES `Accounts` (`Id`) ON DELETE RESTRICT
);

CREATE TABLE `Accounts_ChargenResources` (
    `AccountId` bigint(20) NOT NULL,
    `ChargenResourceId` bigint(20) NOT NULL,
    `Amount` int(11) NOT NULL,
    `LastAwardDate` datetime NOT NULL,
    CONSTRAINT `PRIMARY` PRIMARY KEY (`AccountId`, `ChargenResourceId`),
    CONSTRAINT `FK_Accounts_ChargenResources_Accounts` FOREIGN KEY (`AccountId`) REFERENCES `Accounts` (`Id`) ON DELETE CASCADE,
    CONSTRAINT `FK_Accounts_ChargenResources_ChargenResources` FOREIGN KEY (`ChargenResourceId`) REFERENCES `ChargenResources` (`Id`) ON DELETE CASCADE
);

CREATE TABLE `Bans` (
    `Id` bigint(20) NOT NULL AUTO_INCREMENT,
    `IpMask` varchar(200) CHARACTER SET utf8 NOT NULL,
    `BannerAccountId` bigint(20) NULL,
    `Reason` varchar(4000) CHARACTER SET utf8 NOT NULL,
    `Expiry` datetime NULL,
    CONSTRAINT `PK_Bans` PRIMARY KEY (`Id`),
    CONSTRAINT `FK_Bans_Accounts` FOREIGN KEY (`BannerAccountId`) REFERENCES `Accounts` (`Id`) ON DELETE SET NULL
);

CREATE TABLE `BoardPosts` (
    `Id` bigint(20) NOT NULL AUTO_INCREMENT,
    `BoardId` bigint(20) NOT NULL,
    `Title` varchar(200) CHARACTER SET utf8 NOT NULL,
    `Content` text CHARACTER SET utf8 NOT NULL,
    `AuthorId` bigint(20) NULL,
    `PostTime` datetime NOT NULL,
    CONSTRAINT `PK_BoardPosts` PRIMARY KEY (`Id`),
    CONSTRAINT `FK_BoardsPosts_Accounts` FOREIGN KEY (`AuthorId`) REFERENCES `Accounts` (`Id`) ON DELETE SET NULL,
    CONSTRAINT `FK_BoardPosts_Boards` FOREIGN KEY (`BoardId`) REFERENCES `Boards` (`Id`) ON DELETE CASCADE
);

CREATE TABLE `ChargenRoles` (
    `Id` bigint(20) NOT NULL AUTO_INCREMENT,
    `Name` varchar(4000) CHARACTER SET utf8 NOT NULL,
    `Type` int(11) NOT NULL,
    `PosterId` bigint(20) NOT NULL,
    `MaximumNumberAlive` int(11) NOT NULL,
    `MaximumNumberTotal` int(11) NOT NULL,
    `ChargenBlurb` varchar(4000) CHARACTER SET utf8 NOT NULL,
    `AvailabilityProgId` bigint(20) NULL,
    `Expired` bit(1) NOT NULL DEFAULT b'0',
    `MinimumAuthorityToApprove` int(11) NOT NULL,
    `MinimumAuthorityToView` int(11) NOT NULL,
    CONSTRAINT `PK_ChargenRoles` PRIMARY KEY (`Id`),
    CONSTRAINT `FK_ChargenRoles_FutureProgs` FOREIGN KEY (`AvailabilityProgId`) REFERENCES `FutureProgs` (`Id`) ON DELETE SET NULL,
    CONSTRAINT `FK_ChargenRoles_Accounts` FOREIGN KEY (`PosterId`) REFERENCES `Accounts` (`Id`) ON DELETE RESTRICT
);

CREATE TABLE `Chargens` (
    `Id` bigint(20) NOT NULL AUTO_INCREMENT,
    `AccountId` bigint(20) NOT NULL,
    `Name` varchar(12000) CHARACTER SET utf8 NOT NULL,
    `Definition` text CHARACTER SET utf8 NOT NULL,
    `Status` int(11) NOT NULL,
    `SubmitTime` datetime NULL,
    `MinimumApprovalAuthority` int(11) NULL,
    `ApprovedById` bigint(20) NULL,
    `ApprovalTime` datetime NULL,
    CONSTRAINT `PK_Chargens` PRIMARY KEY (`Id`),
    CONSTRAINT `FK_Chargens_Accounts` FOREIGN KEY (`AccountId`) REFERENCES `Accounts` (`Id`) ON DELETE RESTRICT
);

CREATE TABLE `LoginIPs` (
    `IpAddress` varchar(200) CHARACTER SET utf8 NOT NULL,
    `AccountId` bigint(20) NOT NULL,
    `FirstDate` datetime NOT NULL,
    `AccountRegisteredOnThisIP` bit(1) NOT NULL DEFAULT b'0',
    CONSTRAINT `PRIMARY` PRIMARY KEY (`IpAddress`, `AccountId`),
    CONSTRAINT `FK_LoginIPs_Accounts` FOREIGN KEY (`AccountId`) REFERENCES `Accounts` (`Id`) ON DELETE CASCADE
);

CREATE TABLE `RegionalClimates_Seasons` (
    `RegionalClimateId` bigint(20) NOT NULL,
    `SeasonId` bigint(20) NOT NULL,
    `TemperatureInfo` text CHARACTER SET utf8 NOT NULL,
    CONSTRAINT `PRIMARY` PRIMARY KEY (`RegionalClimateId`, `SeasonId`),
    CONSTRAINT `FK_RegionalClimates_Seasons_RegionalClimates` FOREIGN KEY (`RegionalClimateId`) REFERENCES `RegionalClimates` (`Id`) ON DELETE RESTRICT,
    CONSTRAINT `FK_RegionalClimates_Seasons_Seasons` FOREIGN KEY (`SeasonId`) REFERENCES `Seasons` (`Id`) ON DELETE CASCADE
);

CREATE TABLE `WeatherControllers` (
    `Id` bigint(20) NOT NULL AUTO_INCREMENT,
    `Name` varchar(500) CHARACTER SET utf8 NOT NULL,
    `FeedClockId` bigint(20) NOT NULL,
    `FeedClockTimeZoneId` bigint(20) NOT NULL,
    `RegionalClimateId` bigint(20) NOT NULL,
    `CurrentWeatherEventId` bigint(20) NOT NULL,
    `CurrentSeasonId` bigint(20) NOT NULL,
    `ConsecutiveUnchangedPeriods` int(11) NOT NULL,
    `MinutesCounter` int(11) NOT NULL,
    `CelestialId` bigint(20) NULL,
    `Elevation` double NOT NULL,
    `Radius` double NOT NULL,
    `Latitude` double NOT NULL,
    `Longitude` double NOT NULL,
    `HighestRecentPrecipitationLevel` int(11) NOT NULL,
    `PeriodsSinceHighestPrecipitation` int(11) NOT NULL,
    CONSTRAINT `PK_WeatherControllers` PRIMARY KEY (`Id`),
    CONSTRAINT `FK_WeatherControllers_Celestials` FOREIGN KEY (`CelestialId`) REFERENCES `Celestials` (`Id`) ON DELETE RESTRICT,
    CONSTRAINT `FK_WeatherControllers_Seasons` FOREIGN KEY (`CurrentSeasonId`) REFERENCES `Seasons` (`Id`) ON DELETE RESTRICT,
    CONSTRAINT `FK_WeatherControllers_WeatherEvents` FOREIGN KEY (`CurrentWeatherEventId`) REFERENCES `WeatherEvents` (`Id`) ON DELETE RESTRICT,
    CONSTRAINT `FK_WeatherControllers_Clocks` FOREIGN KEY (`FeedClockId`) REFERENCES `Clocks` (`Id`) ON DELETE RESTRICT,
    CONSTRAINT `FK_WeatherControllers_TimeZones` FOREIGN KEY (`FeedClockTimeZoneId`) REFERENCES `Timezones` (`Id`) ON DELETE RESTRICT,
    CONSTRAINT `FK_WeatherControllers_RegionalClimates` FOREIGN KEY (`RegionalClimateId`) REFERENCES `RegionalClimates` (`Id`) ON DELETE RESTRICT
);

CREATE TABLE `CurrencyDivisionAbbreviations` (
    `Pattern` varchar(150) CHARACTER SET utf8 NOT NULL,
    `CurrencyDivisionId` bigint(20) NOT NULL,
    CONSTRAINT `PRIMARY` PRIMARY KEY (`Pattern`, `CurrencyDivisionId`),
    CONSTRAINT `FK_CurrencyDivisionAbbreviations_CurrencyDivisions` FOREIGN KEY (`CurrencyDivisionId`) REFERENCES `CurrencyDivisions` (`Id`) ON DELETE CASCADE
);

CREATE TABLE `EnforcementAuthorities` (
    `Id` bigint(20) NOT NULL AUTO_INCREMENT,
    `Name` varchar(250) CHARACTER SET utf8 NOT NULL,
    `LegalAuthorityId` bigint(20) NOT NULL,
    `Priority` int(11) NOT NULL,
    `CanAccuse` bit(1) NOT NULL,
    `CanForgive` bit(1) NOT NULL,
    `CanConvict` bit(1) NOT NULL,
    CONSTRAINT `PK_EnforcementAuthorities` PRIMARY KEY (`Id`),
    CONSTRAINT `FK_EnforcementAuthorities_LegalAuthorities` FOREIGN KEY (`LegalAuthorityId`) REFERENCES `LegalAuthorities` (`Id`) ON DELETE CASCADE
);

CREATE TABLE `Laws` (
    `Id` bigint(20) NOT NULL AUTO_INCREMENT,
    `Name` varchar(250) CHARACTER SET utf8 NOT NULL,
    `LegalAuthorityId` bigint(20) NOT NULL,
    `CrimeType` int(11) NOT NULL,
    `ActivePeriod` double NOT NULL,
    `EnforcementStrategy` varchar(500) CHARACTER SET utf8 NOT NULL,
    `LawAppliesProgId` bigint(20) NULL,
    `EnforcementPriority` int(11) NOT NULL,
    `MinimumFine` decimal(10,0) NOT NULL,
    `StandardFine` decimal(10,0) NOT NULL,
    `MaximumFine` decimal(10,0) NOT NULL,
    `CanBeAppliedAutomatically` bit(1) NOT NULL,
    `CanBeArrested` bit(1) NOT NULL,
    `CanBeOfferedBail` bit(1) NOT NULL,
    CONSTRAINT `PK_Laws` PRIMARY KEY (`Id`),
    CONSTRAINT `FK_Laws_FutureProgs` FOREIGN KEY (`LawAppliesProgId`) REFERENCES `FutureProgs` (`Id`) ON DELETE SET NULL,
    CONSTRAINT `FK_Laws_LegalAuthority` FOREIGN KEY (`LegalAuthorityId`) REFERENCES `LegalAuthorities` (`Id`) ON DELETE CASCADE
);

CREATE TABLE `LegalClasses` (
    `Id` bigint(20) NOT NULL AUTO_INCREMENT,
    `Name` varchar(250) CHARACTER SET utf8 NOT NULL,
    `LegalAuthorityId` bigint(20) NOT NULL,
    `LegalClassPriority` int(11) NOT NULL,
    `MembershipProgId` bigint(20) NOT NULL,
    `CanBeDetainedUntilFinesPaid` bit(1) NOT NULL,
    CONSTRAINT `PK_LegalClasses` PRIMARY KEY (`Id`),
    CONSTRAINT `FK_LegalClasses_LegalAuthorities` FOREIGN KEY (`LegalAuthorityId`) REFERENCES `LegalAuthorities` (`Id`) ON DELETE CASCADE,
    CONSTRAINT `FK_LegalClasses_FutureProgs` FOREIGN KEY (`MembershipProgId`) REFERENCES `FutureProgs` (`Id`) ON DELETE CASCADE
);

CREATE TABLE `ForagableProfiles_Foragables` (
    `ForagableProfileId` bigint(20) NOT NULL,
    `ForagableProfileRevisionNumber` int(11) NOT NULL,
    `ForagableId` bigint(20) NOT NULL,
    CONSTRAINT `PRIMARY` PRIMARY KEY (`ForagableProfileId`, `ForagableProfileRevisionNumber`, `ForagableId`),
    CONSTRAINT `FK_ForagableProfiles_Foragables_ForagableProfiles` FOREIGN KEY (`ForagableProfileId`, `ForagableProfileRevisionNumber`) REFERENCES `ForagableProfiles` (`Id`, `RevisionNumber`) ON DELETE CASCADE
);

CREATE TABLE `ForagableProfiles_HourlyYieldGains` (
    `ForagableProfileId` bigint(20) NOT NULL,
    `ForagableProfileRevisionNumber` int(11) NOT NULL,
    `ForageType` varchar(100) CHARACTER SET utf8mb4 NOT NULL,
    `Yield` double NOT NULL,
    CONSTRAINT `PRIMARY` PRIMARY KEY (`ForagableProfileId`, `ForagableProfileRevisionNumber`, `ForageType`),
    CONSTRAINT `FK_ForagableProfiles_HourlyYieldGains_ForagableProfiles` FOREIGN KEY (`ForagableProfileId`, `ForagableProfileRevisionNumber`) REFERENCES `ForagableProfiles` (`Id`, `RevisionNumber`) ON DELETE CASCADE
);

CREATE TABLE `ForagableProfiles_MaximumYields` (
    `ForagableProfileId` bigint(20) NOT NULL,
    `ForagableProfileRevisionNumber` int(11) NOT NULL,
    `ForageType` varchar(100) CHARACTER SET utf8mb4 NOT NULL,
    `Yield` double NOT NULL,
    CONSTRAINT `PRIMARY` PRIMARY KEY (`ForagableProfileId`, `ForagableProfileRevisionNumber`, `ForageType`),
    CONSTRAINT `FK_ForagableProfiles_MaximumYields_ForagableProfiles` FOREIGN KEY (`ForagableProfileId`, `ForagableProfileRevisionNumber`) REFERENCES `ForagableProfiles` (`Id`, `RevisionNumber`) ON DELETE CASCADE
);

CREATE TABLE `NPCTemplates_ArtificalIntelligences` (
    `NPCTemplateId` bigint(20) NOT NULL,
    `AIId` bigint(20) NOT NULL,
    `NPCTemplateRevisionNumber` int(11) NOT NULL,
    CONSTRAINT `PRIMARY` PRIMARY KEY (`NPCTemplateRevisionNumber`, `NPCTemplateId`, `AIId`),
    CONSTRAINT `FK_NTAI_ArtificalIntelligences` FOREIGN KEY (`AIId`) REFERENCES `ArtificialIntelligences` (`Id`) ON DELETE CASCADE,
    CONSTRAINT `FK_NTAI_NPCTemplates` FOREIGN KEY (`NPCTemplateId`, `NPCTemplateRevisionNumber`) REFERENCES `NPCTemplates` (`Id`, `RevisionNumber`) ON DELETE CASCADE
);

CREATE TABLE `ProjectPhases` (
    `Id` bigint(20) NOT NULL AUTO_INCREMENT,
    `ProjectId` bigint(20) NOT NULL,
    `ProjectRevisionNumber` int(11) NOT NULL,
    `PhaseNumber` int(11) NOT NULL,
    `Description` text CHARACTER SET utf8 NOT NULL,
    CONSTRAINT `PK_ProjectPhases` PRIMARY KEY (`Id`),
    CONSTRAINT `FK_ProjectPhases_Projects` FOREIGN KEY (`ProjectId`, `ProjectRevisionNumber`) REFERENCES `Projects` (`Id`, `RevisionNumber`) ON DELETE CASCADE
);

CREATE TABLE `ChannelCommandWords` (
    `Word` varchar(50) CHARACTER SET utf8 NOT NULL,
    `ChannelId` bigint(20) NOT NULL,
    CONSTRAINT `PRIMARY` PRIMARY KEY (`Word`),
    CONSTRAINT `FK_ChannelCommandWords_Channels` FOREIGN KEY (`ChannelId`) REFERENCES `Channels` (`Id`) ON DELETE CASCADE
);

CREATE TABLE `ChannelIgnorers` (
    `ChannelId` bigint(20) NOT NULL,
    `AccountId` bigint(20) NOT NULL,
    CONSTRAINT `PRIMARY` PRIMARY KEY (`ChannelId`, `AccountId`),
    CONSTRAINT `FK_ChannelIgnorers_Accounts` FOREIGN KEY (`AccountId`) REFERENCES `Accounts` (`Id`) ON DELETE CASCADE,
    CONSTRAINT `FK_ChannelIgnorers_Channels` FOREIGN KEY (`ChannelId`) REFERENCES `Channels` (`Id`) ON DELETE CASCADE
);

CREATE TABLE `CurrencyDescriptionPatternElements` (
    `Id` bigint(20) NOT NULL AUTO_INCREMENT,
    `Pattern` varchar(4000) CHARACTER SET utf8 NOT NULL,
    `Order` int(11) NOT NULL,
    `ShowIfZero` bit(1) NOT NULL,
    `CurrencyDivisionId` bigint(20) NOT NULL,
    `CurrencyDescriptionPatternId` bigint(20) NOT NULL,
    `PluraliseWord` varchar(4000) CHARACTER SET utf8 NOT NULL,
    `AlternatePattern` varchar(4000) CHARACTER SET utf8 NULL,
    `RoundingMode` int(11) NOT NULL,
    `SpecialValuesOverrideFormat` bit(1) NOT NULL DEFAULT b'0',
    CONSTRAINT `PK_CurrencyDescriptionPatternElements` PRIMARY KEY (`Id`),
    CONSTRAINT `FK_CDPE_CurrencyDescriptionPatterns` FOREIGN KEY (`CurrencyDescriptionPatternId`) REFERENCES `CurrencyDescriptionPatterns` (`Id`) ON DELETE CASCADE,
    CONSTRAINT `FK_CDPE_CurrencyDivisions` FOREIGN KEY (`CurrencyDivisionId`) REFERENCES `CurrencyDivisions` (`Id`) ON DELETE RESTRICT
);

CREATE TABLE `Dream_Phases` (
    `DreamId` bigint(20) NOT NULL,
    `PhaseId` int(11) NOT NULL,
    `DreamerText` text CHARACTER SET utf8mb4 NOT NULL,
    `DreamerCommand` text CHARACTER SET utf8mb4 NOT NULL,
    `WaitSeconds` int(11) NOT NULL DEFAULT '30',
    CONSTRAINT `PRIMARY` PRIMARY KEY (`DreamId`, `PhaseId`),
    CONSTRAINT `FK_Dream_Phases_Dreams` FOREIGN KEY (`DreamId`) REFERENCES `Dreams` (`Id`) ON DELETE CASCADE
);

CREATE TABLE `EntityDescriptionPatterns_EntityDescriptions` (
    `PatternId` bigint(20) NOT NULL,
    `EntityDescriptionId` bigint(20) NOT NULL,
    CONSTRAINT `PRIMARY` PRIMARY KEY (`PatternId`, `EntityDescriptionId`),
    CONSTRAINT `FK_EDP_EntityDescriptions_EntityDescriptions` FOREIGN KEY (`EntityDescriptionId`) REFERENCES `EntityDescriptions` (`Id`) ON DELETE CASCADE,
    CONSTRAINT `FK_EDP_EntityDescriptions_EntityDescriptionPatterns` FOREIGN KEY (`PatternId`) REFERENCES `EntityDescriptionPatterns` (`Id`) ON DELETE RESTRICT
);

CREATE TABLE `Helpfiles_ExtraTexts` (
    `HelpfileId` bigint(20) NOT NULL,
    `DisplayOrder` int(11) NOT NULL,
    `Text` text CHARACTER SET utf8 NOT NULL,
    `RuleId` bigint(20) NOT NULL,
    CONSTRAINT `PRIMARY` PRIMARY KEY (`HelpfileId`, `DisplayOrder`),
    CONSTRAINT `FK_Helpfiles_ExtraTexts_Helpfiles` FOREIGN KEY (`HelpfileId`) REFERENCES `Helpfiles` (`Id`) ON DELETE RESTRICT,
    CONSTRAINT `FK_Helpfiles_ExtraTexts_FutureProgs` FOREIGN KEY (`RuleId`) REFERENCES `FutureProgs` (`Id`) ON DELETE RESTRICT
);

CREATE TABLE `Scripts` (
    `Id` bigint(20) NOT NULL AUTO_INCREMENT,
    `Name` text CHARACTER SET utf8 NOT NULL,
    `KnownScriptDescription` text CHARACTER SET utf8 NOT NULL,
    `UnknownScriptDescription` text CHARACTER SET utf8 NOT NULL,
    `KnowledgeId` bigint(20) NOT NULL,
    `DocumentLengthModifier` double NOT NULL DEFAULT '1',
    `InkUseModifier` double NOT NULL DEFAULT '1',
    CONSTRAINT `PK_Scripts` PRIMARY KEY (`Id`),
    CONSTRAINT `FK_Scripts_Knowledges` FOREIGN KEY (`KnowledgeId`) REFERENCES `knowledges` (`Id`) ON DELETE CASCADE
);

CREATE TABLE `SurgicalProcedures` (
    `Id` bigint(20) NOT NULL AUTO_INCREMENT,
    `Name` varchar(100) CHARACTER SET utf8mb4 NOT NULL,
    `ProcedureName` varchar(100) CHARACTER SET utf8mb4 NOT NULL,
    `Procedure` int(11) NOT NULL,
    `BaseCheckBonus` double NOT NULL,
    `Check` int(11) NOT NULL,
    `MedicalSchool` varchar(100) CHARACTER SET utf8mb4 NULL,
    `KnowledgeRequiredId` bigint(20) NULL,
    `UsabilityProgId` bigint(20) NULL,
    `WhyCannotUseProgId` bigint(20) NULL,
    `CompletionProgId` bigint(20) NULL,
    `AbortProgId` bigint(20) NULL,
    `ProcedureBeginEmote` text CHARACTER SET utf8mb4 NULL,
    `ProcedureDescriptionEmote` text CHARACTER SET utf8mb4 NULL,
    `ProcedureGerund` text CHARACTER SET utf8mb4 NULL,
    `Definition` text CHARACTER SET utf8mb4 NULL,
    CONSTRAINT `PK_SurgicalProcedures` PRIMARY KEY (`Id`),
    CONSTRAINT `FK_SurgicalProcedures_FutureProgs_AbortProg` FOREIGN KEY (`AbortProgId`) REFERENCES `FutureProgs` (`Id`) ON DELETE SET NULL,
    CONSTRAINT `FK_SurgicalProcedures_FutureProgs_CompletionProg` FOREIGN KEY (`CompletionProgId`) REFERENCES `FutureProgs` (`Id`) ON DELETE SET NULL,
    CONSTRAINT `FK_SurgicalProcedures_Knowledges` FOREIGN KEY (`KnowledgeRequiredId`) REFERENCES `knowledges` (`Id`) ON DELETE SET NULL,
    CONSTRAINT `FK_SurgicalProcedures_FutureProgs_Usability` FOREIGN KEY (`UsabilityProgId`) REFERENCES `FutureProgs` (`Id`) ON DELETE SET NULL,
    CONSTRAINT `FK_SurgicalProcedures_FutureProgs_WhyCannotUseProg` FOREIGN KEY (`WhyCannotUseProgId`) REFERENCES `FutureProgs` (`Id`) ON DELETE SET NULL
);

CREATE TABLE `Materials_Tags` (
    `MaterialId` bigint(20) NOT NULL,
    `TagId` bigint(20) NOT NULL,
    CONSTRAINT `PRIMARY` PRIMARY KEY (`MaterialId`, `TagId`),
    CONSTRAINT `Materials_Tags_Materials` FOREIGN KEY (`MaterialId`) REFERENCES `Materials` (`Id`) ON DELETE CASCADE,
    CONSTRAINT `Materials_Tags_Tags` FOREIGN KEY (`TagId`) REFERENCES `Tags` (`Id`) ON DELETE CASCADE
);

CREATE TABLE `RaceButcheryProfiles` (
    `Id` bigint(20) NOT NULL AUTO_INCREMENT,
    `Name` varchar(100) CHARACTER SET utf8 NOT NULL,
    `Verb` int(11) NOT NULL,
    `RequiredToolTagId` bigint(20) NULL,
    `DifficultySkin` int(11) NOT NULL,
    `CanButcherProgId` bigint(20) NULL,
    `WhyCannotButcherProgId` bigint(20) NULL,
    CONSTRAINT `PK_RaceButcheryProfiles` PRIMARY KEY (`Id`),
    CONSTRAINT `FK_RaceButcheryProfiles_FutureProgs_Can` FOREIGN KEY (`CanButcherProgId`) REFERENCES `FutureProgs` (`Id`) ON DELETE SET NULL,
    CONSTRAINT `FK_RaceButcheryProfiles_Tags` FOREIGN KEY (`RequiredToolTagId`) REFERENCES `Tags` (`Id`) ON DELETE SET NULL,
    CONSTRAINT `FK_RaceButcheryProfiles_FutureProgs_Why` FOREIGN KEY (`WhyCannotButcherProgId`) REFERENCES `FutureProgs` (`Id`) ON DELETE SET NULL
);

CREATE TABLE `WitnessProfiles_CooperatingAuthorities` (
    `WitnessProfileId` bigint(20) NOT NULL,
    `LegalAuthorityId` bigint(20) NOT NULL,
    CONSTRAINT `PRIMARY` PRIMARY KEY (`WitnessProfileId`, `LegalAuthorityId`),
    CONSTRAINT `FK_WitnessProfiles_CooperatingAuthorities_LegalAuthorities` FOREIGN KEY (`LegalAuthorityId`) REFERENCES `LegalAuthorities` (`Id`) ON DELETE CASCADE,
    CONSTRAINT `FK_WitnessProfiles_CooperatingAuthorities_WitnessProfiles` FOREIGN KEY (`WitnessProfileId`) REFERENCES `WitnessProfiles` (`Id`) ON DELETE CASCADE
);

CREATE TABLE `GameItemProtos_DefaultVariables` (
    `GameItemProtoId` bigint(20) NOT NULL,
    `VariableName` varchar(100) CHARACTER SET utf8mb4 NOT NULL,
    `GameItemProtoRevNum` int(11) NOT NULL,
    `VariableValue` text CHARACTER SET utf8mb4 NOT NULL,
    CONSTRAINT `PRIMARY` PRIMARY KEY (`GameItemProtoId`, `GameItemProtoRevNum`, `VariableName`),
    CONSTRAINT `FK_GameItemProtos_DefaultValues_GameItemProtos` FOREIGN KEY (`GameItemProtoId`, `GameItemProtoRevNum`) REFERENCES `GameItemProtos` (`Id`, `RevisionNumber`) ON DELETE CASCADE
);

CREATE TABLE `GameItemProtos_GameItemComponentProtos` (
    `GameItemProtoId` bigint(20) NOT NULL,
    `GameItemComponentProtoId` bigint(20) NOT NULL,
    `GameItemProtoRevision` int(11) NOT NULL,
    `GameItemComponentRevision` int(11) NOT NULL,
    CONSTRAINT `PRIMARY` PRIMARY KEY (`GameItemProtoId`, `GameItemComponentProtoId`, `GameItemProtoRevision`, `GameItemComponentRevision`),
    CONSTRAINT `FK_GIPGICP_GameItemComponentProtos` FOREIGN KEY (`GameItemComponentProtoId`, `GameItemComponentRevision`) REFERENCES `GameItemComponentProtos` (`Id`, `RevisionNumber`) ON DELETE CASCADE,
    CONSTRAINT `FK_GIPGICP_GameItemProtos` FOREIGN KEY (`GameItemProtoId`, `GameItemProtoRevision`) REFERENCES `GameItemProtos` (`Id`, `RevisionNumber`) ON DELETE CASCADE
);

CREATE TABLE `GameItemProtos_OnLoadProgs` (
    `GameItemProtoId` bigint(20) NOT NULL,
    `GameItemProtoRevisionNumber` int(11) NOT NULL,
    `FutureProgId` bigint(20) NOT NULL,
    CONSTRAINT `PRIMARY` PRIMARY KEY (`GameItemProtoId`, `GameItemProtoRevisionNumber`, `FutureProgId`),
    CONSTRAINT `FK_GameItemProtos_OnLoadProgs_FutureProgs` FOREIGN KEY (`FutureProgId`) REFERENCES `FutureProgs` (`Id`) ON DELETE RESTRICT,
    CONSTRAINT `FK_GameItemProtos_OnLoadProgs_GameItemProtos` FOREIGN KEY (`GameItemProtoId`, `GameItemProtoRevisionNumber`) REFERENCES `GameItemProtos` (`Id`, `RevisionNumber`) ON DELETE CASCADE
);

CREATE TABLE `GameItemProtos_Tags` (
    `GameItemProtoId` bigint(20) NOT NULL,
    `TagId` bigint(20) NOT NULL,
    `GameItemProtoRevisionNumber` int(11) NOT NULL,
    CONSTRAINT `PRIMARY` PRIMARY KEY (`GameItemProtoId`, `TagId`, `GameItemProtoRevisionNumber`),
    CONSTRAINT `FK_GameItemProtos_Tags_Tags` FOREIGN KEY (`TagId`) REFERENCES `Tags` (`Id`) ON DELETE CASCADE,
    CONSTRAINT `FK_GameItemProtos_Tags_GameItemProtos` FOREIGN KEY (`GameItemProtoId`, `GameItemProtoRevisionNumber`) REFERENCES `GameItemProtos` (`Id`, `RevisionNumber`) ON DELETE CASCADE
);

CREATE TABLE `Gases` (
    `Id` bigint(20) NOT NULL AUTO_INCREMENT,
    `Name` varchar(100) CHARACTER SET utf8mb4 NOT NULL,
    `Description` varchar(200) CHARACTER SET utf8mb4 NOT NULL,
    `Density` double NOT NULL DEFAULT '0.001205',
    `ThermalConductivity` double NOT NULL DEFAULT '0.0257',
    `ElectricalConductivity` double NOT NULL DEFAULT '0.000005',
    `Organic` bit(1) NOT NULL DEFAULT b'0',
    `SpecificHeatCapacity` double NOT NULL DEFAULT '1.005',
    `BoilingPoint` double NOT NULL DEFAULT '5',
    `CountAsId` bigint(20) NULL,
    `CountsAsQuality` int(11) NULL,
    `DisplayColour` varchar(40) CHARACTER SET utf8mb4 NOT NULL,
    `PrecipitateId` bigint(20) NULL,
    `SmellIntensity` double NOT NULL,
    `SmellText` text CHARACTER SET utf8mb4 NOT NULL,
    `VagueSmellText` text CHARACTER SET utf8mb4 NOT NULL,
    `Viscosity` double NOT NULL DEFAULT '15',
    CONSTRAINT `PK_Gases` PRIMARY KEY (`Id`),
    CONSTRAINT `FK_Gases_Gases` FOREIGN KEY (`CountAsId`) REFERENCES `Gases` (`Id`) ON DELETE SET NULL,
    CONSTRAINT `FK_Gases_Liquids` FOREIGN KEY (`PrecipitateId`) REFERENCES `Liquids` (`Id`) ON DELETE SET NULL
);

CREATE TABLE `Liquids_Tags` (
    `LiquidId` bigint(20) NOT NULL,
    `TagId` bigint(20) NOT NULL,
    CONSTRAINT `PRIMARY` PRIMARY KEY (`LiquidId`, `TagId`),
    CONSTRAINT `FK_Liquids_Tags_Liquids` FOREIGN KEY (`LiquidId`) REFERENCES `Liquids` (`Id`) ON DELETE CASCADE,
    CONSTRAINT `FK_Liquids_Tags_Tags` FOREIGN KEY (`TagId`) REFERENCES `Tags` (`Id`) ON DELETE CASCADE
);

CREATE TABLE `ChargenAdvices_Cultures` (
    `ChargenAdviceId` bigint(20) NOT NULL,
    `CultureId` bigint(20) NOT NULL,
    CONSTRAINT `PRIMARY` PRIMARY KEY (`ChargenAdviceId`, `CultureId`),
    CONSTRAINT `FK_ChargenAdvices_Cultures_ChargenAdvices` FOREIGN KEY (`ChargenAdviceId`) REFERENCES `ChargenAdvices` (`Id`) ON DELETE CASCADE,
    CONSTRAINT `FK_ChargenAdvices_Cultures_Cultures` FOREIGN KEY (`CultureId`) REFERENCES `Cultures` (`Id`) ON DELETE CASCADE
);

CREATE TABLE `Cultures_ChargenResources` (
    `CultureId` bigint(20) NOT NULL,
    `ChargenResourceId` bigint(20) NOT NULL,
    `RequirementOnly` bit(1) NOT NULL DEFAULT b'0',
    `Amount` int(11) NOT NULL,
    CONSTRAINT `PRIMARY` PRIMARY KEY (`CultureId`, `ChargenResourceId`, `RequirementOnly`),
    CONSTRAINT `FK_Cultures_ChargenResources_ChargenResources` FOREIGN KEY (`ChargenResourceId`) REFERENCES `ChargenResources` (`Id`) ON DELETE CASCADE,
    CONSTRAINT `FK_Cultures_ChargenResources_Races` FOREIGN KEY (`CultureId`) REFERENCES `Cultures` (`Id`) ON DELETE CASCADE
);

CREATE TABLE `RandomNameProfiles_DiceExpressions` (
    `RandomNameProfileId` bigint(20) NOT NULL,
    `NameUsage` int(11) NOT NULL,
    `DiceExpression` varchar(255) CHARACTER SET utf8 NOT NULL,
    CONSTRAINT `PRIMARY` PRIMARY KEY (`RandomNameProfileId`, `NameUsage`),
    CONSTRAINT `FK_RandomNameProfiles_DiceExpressions_RandomNameProfiles` FOREIGN KEY (`RandomNameProfileId`) REFERENCES `RandomNameProfiles` (`Id`) ON DELETE CASCADE
);

CREATE TABLE `RandomNameProfiles_Elements` (
    `RandomNameProfileId` bigint(20) NOT NULL,
    `NameUsage` int(11) NOT NULL,
    `Name` varchar(100) CHARACTER SET utf8 NOT NULL,
    `Weighting` int(11) NOT NULL,
    CONSTRAINT `PRIMARY` PRIMARY KEY (`RandomNameProfileId`, `NameUsage`, `Name`),
    CONSTRAINT `FK_RandomNameProfiles_Elements_RandomNameProfiles` FOREIGN KEY (`RandomNameProfileId`) REFERENCES `RandomNameProfiles` (`Id`) ON DELETE CASCADE
);

CREATE TABLE `Shards_Calendars` (
    `ShardId` bigint(20) NOT NULL,
    `CalendarId` bigint(20) NOT NULL,
    CONSTRAINT `PRIMARY` PRIMARY KEY (`ShardId`, `CalendarId`),
    CONSTRAINT `FK_Shards_Calendars_Shards` FOREIGN KEY (`ShardId`) REFERENCES `Shards` (`Id`) ON DELETE CASCADE
);

CREATE TABLE `Shards_Celestials` (
    `ShardId` bigint(20) NOT NULL,
    `CelestialId` bigint(20) NOT NULL,
    CONSTRAINT `PRIMARY` PRIMARY KEY (`ShardId`, `CelestialId`),
    CONSTRAINT `FK_Shards_Celestials_Shards` FOREIGN KEY (`ShardId`) REFERENCES `Shards` (`Id`) ON DELETE CASCADE
);

CREATE TABLE `Shards_Clocks` (
    `ShardId` bigint(20) NOT NULL,
    `ClockId` bigint(20) NOT NULL,
    CONSTRAINT `PRIMARY` PRIMARY KEY (`ShardId`, `ClockId`),
    CONSTRAINT `FK_Shards_Clocks_Shards` FOREIGN KEY (`ShardId`) REFERENCES `Shards` (`Id`) ON DELETE CASCADE
);

CREATE TABLE `Crafts` (
    `Id` bigint(20) NOT NULL,
    `RevisionNumber` int(11) NOT NULL,
    `EditableItemId` bigint(20) NOT NULL,
    `Name` varchar(200) CHARACTER SET utf8 NOT NULL,
    `Blurb` text CHARACTER SET utf8 NOT NULL,
    `ActionDescription` varchar(200) CHARACTER SET utf8 NOT NULL,
    `Category` varchar(200) CHARACTER SET utf8 NOT NULL,
    `Interruptable` bit(1) NOT NULL,
    `ToolQualityWeighting` double NOT NULL,
    `InputQualityWeighting` double NOT NULL,
    `CheckQualityWeighting` double NOT NULL,
    `FreeSkillChecks` int(11) NOT NULL,
    `FailThreshold` int(11) NOT NULL,
    `CheckTraitId` bigint(20) NULL,
    `CheckDifficulty` int(11) NOT NULL,
    `FailPhase` int(11) NOT NULL,
    `QualityFormula` text CHARACTER SET utf8 NOT NULL,
    `AppearInCraftsListProgId` bigint(20) NULL,
    `CanUseProgId` bigint(20) NULL,
    `WhyCannotUseProgId` bigint(20) NULL,
    `OnUseProgStartId` bigint(20) NULL,
    `OnUseProgCompleteId` bigint(20) NULL,
    `OnUseProgCancelId` bigint(20) NULL,
    `ActiveCraftItemSDesc` varchar(200) CHARACTER SET utf8 NOT NULL DEFAULT 'a craft in progress',
    CONSTRAINT `PRIMARY` PRIMARY KEY (`Id`, `RevisionNumber`),
    CONSTRAINT `FK_Crafts_FutureProgs_AppearInCraftsListProg` FOREIGN KEY (`AppearInCraftsListProgId`) REFERENCES `FutureProgs` (`Id`) ON DELETE RESTRICT,
    CONSTRAINT `FK_Crafts_FutureProgs_CanUseProg` FOREIGN KEY (`CanUseProgId`) REFERENCES `FutureProgs` (`Id`) ON DELETE SET NULL,
    CONSTRAINT `FK_Crafts_TraitDefinitions` FOREIGN KEY (`CheckTraitId`) REFERENCES `TraitDefinitions` (`Id`) ON DELETE RESTRICT,
    CONSTRAINT `FK_Crafts_EditableItems` FOREIGN KEY (`EditableItemId`) REFERENCES `EditableItems` (`Id`) ON DELETE CASCADE,
    CONSTRAINT `FK_Crafts_FutureProgs_OnUseProgCancel` FOREIGN KEY (`OnUseProgCancelId`) REFERENCES `FutureProgs` (`Id`) ON DELETE SET NULL,
    CONSTRAINT `FK_Crafts_FutureProgs_OnUseProgComplete` FOREIGN KEY (`OnUseProgCompleteId`) REFERENCES `FutureProgs` (`Id`) ON DELETE SET NULL,
    CONSTRAINT `FK_Crafts_FutureProgs_OnUseProgStart` FOREIGN KEY (`OnUseProgStartId`) REFERENCES `FutureProgs` (`Id`) ON DELETE SET NULL,
    CONSTRAINT `FK_Crafts_FutureProgs_WhyCannotUseProg` FOREIGN KEY (`WhyCannotUseProgId`) REFERENCES `FutureProgs` (`Id`) ON DELETE SET NULL
);

CREATE TABLE `RangedWeaponTypes` (
    `Id` bigint(20) NOT NULL AUTO_INCREMENT,
    `Name` varchar(200) CHARACTER SET utf8mb4 NOT NULL,
    `Classification` int(11) NOT NULL,
    `FireTraitId` bigint(20) NOT NULL,
    `OperateTraitId` bigint(20) NOT NULL,
    `FireableInMelee` bit(1) NOT NULL,
    `DefaultRangeInRooms` int(11) NOT NULL,
    `AccuracyBonusExpression` text CHARACTER SET utf8mb4 NOT NULL,
    `DamageBonusExpression` text CHARACTER SET utf8mb4 NOT NULL,
    `AmmunitionLoadType` int(11) NOT NULL,
    `SpecificAmmunitionGrade` text CHARACTER SET utf8mb4 NOT NULL,
    `AmmunitionCapacity` int(11) NOT NULL,
    `RangedWeaponType` int(11) NOT NULL,
    `StaminaToFire` double NOT NULL,
    `StaminaPerLoadStage` double NOT NULL,
    `CoverBonus` double NOT NULL,
    `BaseAimDifficulty` int(11) NOT NULL,
    `LoadDelay` double NOT NULL DEFAULT '0.5',
    `ReadyDelay` double NOT NULL DEFAULT '0.1',
    `FireDelay` double NOT NULL DEFAULT '0.5',
    `AimBonusLostPerShot` double NOT NULL DEFAULT '1',
    `RequiresFreeHandToReady` bit(1) NOT NULL DEFAULT b'1',
    `AlwaysRequiresTwoHandsToWield` bit(1) NOT NULL DEFAULT b'0',
    CONSTRAINT `PK_RangedWeaponTypes` PRIMARY KEY (`Id`),
    CONSTRAINT `FK_RangedWeaponTypes_TraitDefinitions_Fire` FOREIGN KEY (`FireTraitId`) REFERENCES `TraitDefinitions` (`Id`) ON DELETE RESTRICT,
    CONSTRAINT `FK_RangedWeaponTypes_TraitDefinitions_Operate` FOREIGN KEY (`OperateTraitId`) REFERENCES `TraitDefinitions` (`Id`) ON DELETE RESTRICT
);

CREATE TABLE `ShieldTypes` (
    `Id` bigint(20) NOT NULL AUTO_INCREMENT,
    `Name` text CHARACTER SET utf8mb4 NOT NULL,
    `BlockTraitId` bigint(20) NOT NULL,
    `BlockBonus` double NOT NULL,
    `StaminaPerBlock` double NOT NULL,
    `EffectiveArmourTypeId` bigint(20) NULL,
    CONSTRAINT `PK_ShieldTypes` PRIMARY KEY (`Id`),
    CONSTRAINT `FK_ShieldTypes_TraitDefinitions` FOREIGN KEY (`BlockTraitId`) REFERENCES `TraitDefinitions` (`Id`) ON DELETE RESTRICT,
    CONSTRAINT `FK_ShieldTypes_ArmourTypes` FOREIGN KEY (`EffectiveArmourTypeId`) REFERENCES `ArmourTypes` (`Id`) ON DELETE SET NULL
);

CREATE TABLE `TraitDefinitions_ChargenResources` (
    `TraitDefinitionId` bigint(20) NOT NULL,
    `ChargenResourceId` bigint(20) NOT NULL,
    `RequirementOnly` bit(1) NOT NULL DEFAULT b'0',
    `Amount` int(11) NOT NULL,
    CONSTRAINT `PRIMARY` PRIMARY KEY (`TraitDefinitionId`, `ChargenResourceId`, `RequirementOnly`),
    CONSTRAINT `FK_TraitDefinitions_ChargenResources_ChargenResources` FOREIGN KEY (`ChargenResourceId`) REFERENCES `ChargenResources` (`Id`) ON DELETE CASCADE,
    CONSTRAINT `FK_TraitDefinitions_ChargenResources_Races` FOREIGN KEY (`TraitDefinitionId`) REFERENCES `TraitDefinitions` (`Id`) ON DELETE CASCADE
);

CREATE TABLE `TraitExpressionParameters` (
    `TraitExpressionId` bigint(20) NOT NULL,
    `Parameter` varchar(50) CHARACTER SET utf8 NOT NULL,
    `TraitDefinitionId` bigint(20) NOT NULL,
    `CanImprove` bit(1) NOT NULL DEFAULT b'1',
    `CanBranch` bit(1) NOT NULL DEFAULT b'1',
    CONSTRAINT `PRIMARY` PRIMARY KEY (`Parameter`, `TraitExpressionId`),
    CONSTRAINT `FK_TraitExpressionParameters_TraitDefinitions` FOREIGN KEY (`TraitDefinitionId`) REFERENCES `TraitDefinitions` (`Id`) ON DELETE RESTRICT,
    CONSTRAINT `FK_TraitExpressionParameters_TraitExpression` FOREIGN KEY (`TraitExpressionId`) REFERENCES `TraitExpression` (`Id`) ON DELETE RESTRICT
);

CREATE TABLE `WeaponTypes` (
    `Id` bigint(20) NOT NULL AUTO_INCREMENT,
    `Name` varchar(200) CHARACTER SET utf8mb4 NOT NULL,
    `Classification` int(11) NOT NULL,
    `AttackTraitId` bigint(20) NULL,
    `ParryTraitId` bigint(20) NULL,
    `ParryBonus` int(11) NOT NULL,
    `Reach` int(11) NOT NULL DEFAULT '1',
    `StaminaPerParry` double NOT NULL,
    CONSTRAINT `PK_WeaponTypes` PRIMARY KEY (`Id`),
    CONSTRAINT `FK_WeaponTypes_TraitDefinitions_Attack` FOREIGN KEY (`AttackTraitId`) REFERENCES `TraitDefinitions` (`Id`) ON DELETE SET NULL,
    CONSTRAINT `FK_WeaponTypes_TraitDefinitions_Parry` FOREIGN KEY (`ParryTraitId`) REFERENCES `TraitDefinitions` (`Id`) ON DELETE SET NULL
);

CREATE TABLE `WearableSizeParameterRule` (
    `Id` bigint(20) NOT NULL AUTO_INCREMENT,
    `MinHeightFactor` double NOT NULL,
    `MaxHeightFactor` double NOT NULL,
    `MinWeightFactor` double NOT NULL,
    `MaxWeightFactor` double NULL,
    `MinTraitFactor` double NULL,
    `MaxTraitFactor` double NULL,
    `TraitId` bigint(20) NULL,
    `BodyProtoId` bigint(20) NOT NULL,
    `IgnoreTrait` bit(1) NOT NULL DEFAULT b'1',
    `WeightVolumeRatios` varchar(4000) CHARACTER SET utf8 NULL,
    `TraitVolumeRatios` varchar(4000) CHARACTER SET utf8 NULL,
    `HeightLinearRatios` varchar(4000) CHARACTER SET utf8 NULL,
    CONSTRAINT `PK_WearableSizeParameterRule` PRIMARY KEY (`Id`),
    CONSTRAINT `FK_WearableSizeParameterRule_TraitDefinitions` FOREIGN KEY (`TraitId`) REFERENCES `TraitDefinitions` (`Id`) ON DELETE RESTRICT
);

CREATE TABLE `ChargenAdvices_ChargenRoles` (
    `ChargenAdviceId` bigint(20) NOT NULL,
    `ChargenRoleId` bigint(20) NOT NULL,
    CONSTRAINT `PRIMARY` PRIMARY KEY (`ChargenAdviceId`, `ChargenRoleId`),
    CONSTRAINT `FK_ChargenAdvices_ChargenRoles_ChargenAdvices` FOREIGN KEY (`ChargenAdviceId`) REFERENCES `ChargenAdvices` (`Id`) ON DELETE CASCADE,
    CONSTRAINT `FK_ChargenAdvices_ChargenRoles_ChargenRoles` FOREIGN KEY (`ChargenRoleId`) REFERENCES `ChargenRoles` (`Id`) ON DELETE CASCADE
);

CREATE TABLE `ChargenRoles_Approvers` (
    `ChargenRoleId` bigint(20) NOT NULL,
    `ApproverId` bigint(20) NOT NULL,
    CONSTRAINT `PRIMARY` PRIMARY KEY (`ChargenRoleId`, `ApproverId`),
    CONSTRAINT `FK_ChargenRoles_Approvers_Accounts` FOREIGN KEY (`ApproverId`) REFERENCES `Accounts` (`Id`) ON DELETE RESTRICT,
    CONSTRAINT `FK_ChargenRoles_Approvers_ChargenRoles` FOREIGN KEY (`ChargenRoleId`) REFERENCES `ChargenRoles` (`Id`) ON DELETE CASCADE
);

CREATE TABLE `ChargenRoles_Costs` (
    `ChargenRoleId` bigint(20) NOT NULL,
    `ChargenResourceId` bigint(20) NOT NULL,
    `RequirementOnly` bit(1) NOT NULL DEFAULT b'0',
    `Amount` int(11) NOT NULL,
    CONSTRAINT `PRIMARY` PRIMARY KEY (`ChargenRoleId`, `ChargenResourceId`, `RequirementOnly`),
    CONSTRAINT `FK_ChargenRoles_Costs_ChargenResources` FOREIGN KEY (`ChargenResourceId`) REFERENCES `ChargenResources` (`Id`) ON DELETE CASCADE,
    CONSTRAINT `FK_ChargenRoles_Costs_ChargenRoles` FOREIGN KEY (`ChargenRoleId`) REFERENCES `ChargenRoles` (`Id`) ON DELETE CASCADE
);

CREATE TABLE `ChargenRoles_Currencies` (
    `ChargenRoleId` bigint(20) NOT NULL,
    `CurrencyId` bigint(20) NOT NULL,
    `Amount` decimal(18,0) NOT NULL,
    CONSTRAINT `PRIMARY` PRIMARY KEY (`ChargenRoleId`, `CurrencyId`),
    CONSTRAINT `FK_ChargenRoles_Currencies_ChargenRoles` FOREIGN KEY (`ChargenRoleId`) REFERENCES `ChargenRoles` (`Id`) ON DELETE CASCADE,
    CONSTRAINT `FK_ChargenRoles_Currencies_Currencies` FOREIGN KEY (`CurrencyId`) REFERENCES `Currencies` (`Id`) ON DELETE CASCADE
);

CREATE TABLE `ChargenRoles_Merits` (
    `ChargenRoleId` bigint(20) NOT NULL,
    `MeritId` bigint(20) NOT NULL,
    CONSTRAINT `PRIMARY` PRIMARY KEY (`ChargenRoleId`, `MeritId`),
    CONSTRAINT `FK_ChargenRoles_Merits_ChargenRoles` FOREIGN KEY (`ChargenRoleId`) REFERENCES `ChargenRoles` (`Id`) ON DELETE CASCADE,
    CONSTRAINT `FK_ChargenRoles_Merits_Merits` FOREIGN KEY (`MeritId`) REFERENCES `Merits` (`Id`) ON DELETE CASCADE
);

CREATE TABLE `ChargenRoles_Traits` (
    `ChargenRoleId` bigint(20) NOT NULL,
    `TraitId` bigint(20) NOT NULL,
    `Amount` double NOT NULL,
    `GiveIfDoesntHave` bit(1) NOT NULL DEFAULT b'0',
    CONSTRAINT `PRIMARY` PRIMARY KEY (`ChargenRoleId`, `TraitId`),
    CONSTRAINT `FK_ChargenRoles_Traits_ChargenRoles` FOREIGN KEY (`ChargenRoleId`) REFERENCES `ChargenRoles` (`Id`) ON DELETE CASCADE,
    CONSTRAINT `FK_ChargenRoles_Traits_Currencies` FOREIGN KEY (`TraitId`) REFERENCES `TraitDefinitions` (`Id`) ON DELETE CASCADE
);

CREATE TABLE `Areas` (
    `Id` bigint(20) NOT NULL AUTO_INCREMENT,
    `Name` varchar(200) CHARACTER SET utf8 NOT NULL,
    `WeatherControllerId` bigint(20) NULL,
    CONSTRAINT `PK_Areas` PRIMARY KEY (`Id`),
    CONSTRAINT `FK_Areas_WeatherControllers` FOREIGN KEY (`WeatherControllerId`) REFERENCES `WeatherControllers` (`Id`) ON DELETE SET NULL
);

CREATE TABLE `Terrains` (
    `Id` bigint(20) NOT NULL AUTO_INCREMENT,
    `Name` varchar(255) CHARACTER SET utf8 NOT NULL,
    `MovementRate` double NOT NULL,
    `DefaultTerrain` bit(1) NOT NULL DEFAULT b'0',
    `TerrainBehaviourMode` varchar(50) CHARACTER SET utf8 NOT NULL,
    `HideDifficulty` int(11) NOT NULL,
    `SpotDifficulty` int(11) NOT NULL,
    `StaminaCost` double NOT NULL,
    `ForagableProfileId` bigint(20) NOT NULL,
    `InfectionMultiplier` double NOT NULL DEFAULT '1',
    `InfectionType` int(11) NOT NULL,
    `InfectionVirulence` int(11) NOT NULL DEFAULT '5',
    `AtmosphereId` bigint(20) NULL,
    `AtmosphereType` varchar(45) CHARACTER SET utf8 NULL,
    `TerrainEditorColour` varchar(45) CHARACTER SET utf8 NOT NULL DEFAULT '#FFFFFFFF',
    `WeatherControllerId` bigint(20) NULL,
    CONSTRAINT `PK_Terrains` PRIMARY KEY (`Id`),
    CONSTRAINT `FK_Terrains_WeatherControllers` FOREIGN KEY (`WeatherControllerId`) REFERENCES `WeatherControllers` (`Id`) ON DELETE SET NULL
);

CREATE TABLE `EnforcementAuthorities_ParentAuthorities` (
    `ParentId` bigint(20) NOT NULL,
    `ChildId` bigint(20) NOT NULL,
    CONSTRAINT `PRIMARY` PRIMARY KEY (`ParentId`, `ChildId`),
    CONSTRAINT `FK_EnforcementAuthorities_ParentAuthorities_Child` FOREIGN KEY (`ChildId`) REFERENCES `EnforcementAuthorities` (`Id`) ON DELETE CASCADE,
    CONSTRAINT `FK_EnforcementAuthorities_ParentAuthorities_Parent` FOREIGN KEY (`ParentId`) REFERENCES `EnforcementAuthorities` (`Id`) ON DELETE CASCADE
);

CREATE TABLE `EnforcementAuthorities_AccusableClasses` (
    `EnforcementAuthorityId` bigint(20) NOT NULL,
    `LegalClassId` bigint(20) NOT NULL,
    CONSTRAINT `PRIMARY` PRIMARY KEY (`EnforcementAuthorityId`, `LegalClassId`),
    CONSTRAINT `FK_EnforcementAuthorities_AccusableClasses_Enforce` FOREIGN KEY (`EnforcementAuthorityId`) REFERENCES `EnforcementAuthorities` (`Id`) ON DELETE CASCADE,
    CONSTRAINT `FK_EnforcementAuthorities_AccusableClasses_LegalClasses` FOREIGN KEY (`LegalClassId`) REFERENCES `LegalClasses` (`Id`) ON DELETE CASCADE
);

CREATE TABLE `EnforcementAuthorities_ArrestableClasses` (
    `EnforcementAuthorityId` bigint(20) NOT NULL,
    `LegalClassId` bigint(20) NOT NULL,
    CONSTRAINT `PRIMARY` PRIMARY KEY (`EnforcementAuthorityId`),
    CONSTRAINT `FK_EnforcementAuthorities_ArrestableClasses_Enforce` FOREIGN KEY (`EnforcementAuthorityId`) REFERENCES `EnforcementAuthorities` (`Id`) ON DELETE CASCADE,
    CONSTRAINT `FK_EnforcementAuthorities_ArrestableClasses_LegalClasses` FOREIGN KEY (`LegalClassId`) REFERENCES `LegalClasses` (`Id`) ON DELETE CASCADE
);

CREATE TABLE `Laws_OffenderClasses` (
    `LawId` bigint(20) NOT NULL,
    `LegalClassId` bigint(20) NOT NULL,
    CONSTRAINT `PRIMARY` PRIMARY KEY (`LawId`, `LegalClassId`),
    CONSTRAINT `FK_Laws_OffenderClasses_Laws` FOREIGN KEY (`LawId`) REFERENCES `Laws` (`Id`) ON DELETE CASCADE,
    CONSTRAINT `FK_Laws_OffenderClasses_LegalClasses` FOREIGN KEY (`LegalClassId`) REFERENCES `LegalClasses` (`Id`) ON DELETE CASCADE
);

CREATE TABLE `Laws_VictimClasses` (
    `LawId` bigint(20) NOT NULL,
    `LegalClassId` bigint(20) NOT NULL,
    CONSTRAINT `PRIMARY` PRIMARY KEY (`LawId`, `LegalClassId`),
    CONSTRAINT `FK_Laws_VictimClasses_Laws` FOREIGN KEY (`LawId`) REFERENCES `Laws` (`Id`) ON DELETE CASCADE,
    CONSTRAINT `FK_Laws_VictimClasses_LegalClasses` FOREIGN KEY (`LegalClassId`) REFERENCES `LegalClasses` (`Id`) ON DELETE CASCADE
);

CREATE TABLE `WitnessProfiles_IgnoredCriminalClasses` (
    `WitnessProfileId` bigint(20) NOT NULL,
    `LegalClassId` bigint(20) NOT NULL,
    CONSTRAINT `PRIMARY` PRIMARY KEY (`WitnessProfileId`, `LegalClassId`),
    CONSTRAINT `FK_WitnessProfiles_IgnoredCriminalClasses_LegalClasses` FOREIGN KEY (`LegalClassId`) REFERENCES `LegalClasses` (`Id`) ON DELETE CASCADE,
    CONSTRAINT `FK_WitnessProfiles_IgnoredCriminalClasses_WitnessProfiles` FOREIGN KEY (`WitnessProfileId`) REFERENCES `WitnessProfiles` (`Id`) ON DELETE CASCADE
);

CREATE TABLE `WitnessProfiles_IgnoredVictimClasses` (
    `WitnessProfileId` bigint(20) NOT NULL,
    `LegalClassId` bigint(20) NOT NULL,
    CONSTRAINT `PRIMARY` PRIMARY KEY (`WitnessProfileId`, `LegalClassId`),
    CONSTRAINT `FK_WitnessProfiles_IgnoredVictimClasses_LegalClasses` FOREIGN KEY (`LegalClassId`) REFERENCES `LegalClasses` (`Id`) ON DELETE CASCADE,
    CONSTRAINT `FK_WitnessProfiles_IgnoredVictimClasses_WitnessProfiles` FOREIGN KEY (`WitnessProfileId`) REFERENCES `WitnessProfiles` (`Id`) ON DELETE CASCADE
);

CREATE TABLE `ProjectActions` (
    `Id` bigint(20) NOT NULL AUTO_INCREMENT,
    `Name` varchar(200) CHARACTER SET utf8 NOT NULL,
    `Description` varchar(1000) CHARACTER SET utf8 NOT NULL,
    `Type` varchar(45) CHARACTER SET utf8 NOT NULL,
    `SortOrder` int(11) NOT NULL,
    `Definition` text CHARACTER SET utf8 NOT NULL,
    `ProjectPhaseId` bigint(20) NOT NULL,
    CONSTRAINT `PK_ProjectActions` PRIMARY KEY (`Id`),
    CONSTRAINT `FK_ProjectActions_ProjectPhases` FOREIGN KEY (`ProjectPhaseId`) REFERENCES `ProjectPhases` (`Id`) ON DELETE CASCADE
);

CREATE TABLE `ProjectLabourRequirements` (
    `Id` bigint(20) NOT NULL AUTO_INCREMENT,
    `Type` varchar(45) CHARACTER SET utf8 NOT NULL,
    `Name` varchar(200) CHARACTER SET utf8 NOT NULL,
    `ProjectPhaseId` bigint(20) NOT NULL,
    `Description` text CHARACTER SET utf8 NOT NULL,
    `TotalProgressRequired` double NOT NULL,
    `MaximumSimultaneousWorkers` int(11) NOT NULL,
    `Definition` text CHARACTER SET utf8 NOT NULL,
    CONSTRAINT `PK_ProjectLabourRequirements` PRIMARY KEY (`Id`),
    CONSTRAINT `FK_ProjectLabourRequirements_ProjectPhases` FOREIGN KEY (`ProjectPhaseId`) REFERENCES `ProjectPhases` (`Id`) ON DELETE CASCADE
);

CREATE TABLE `ProjectMaterialRequirements` (
    `Id` bigint(20) NOT NULL AUTO_INCREMENT,
    `Type` varchar(45) CHARACTER SET utf8 NOT NULL,
    `ProjectPhaseId` bigint(20) NOT NULL,
    `Name` varchar(200) CHARACTER SET utf8 NOT NULL,
    `Description` text CHARACTER SET utf8 NOT NULL,
    `Definition` text CHARACTER SET utf8 NOT NULL,
    `IsMandatoryForProjectCompletion` bit(1) NOT NULL DEFAULT b'1',
    CONSTRAINT `PK_ProjectMaterialRequirements` PRIMARY KEY (`Id`),
    CONSTRAINT `FK_ProjectMaterialRequirements_ProjectPhases` FOREIGN KEY (`ProjectPhaseId`) REFERENCES `ProjectPhases` (`Id`) ON DELETE CASCADE
);

CREATE TABLE `CurrencyDescriptionPatternElementSpecialValues` (
    `Value` decimal(18,0) NOT NULL,
    `CurrencyDescriptionPatternElementId` bigint(20) NOT NULL,
    `Text` varchar(4000) CHARACTER SET utf8 NOT NULL,
    CONSTRAINT `PRIMARY` PRIMARY KEY (`Value`, `CurrencyDescriptionPatternElementId`),
    CONSTRAINT `FK_CDPESV_CDPE` FOREIGN KEY (`CurrencyDescriptionPatternElementId`) REFERENCES `CurrencyDescriptionPatternElements` (`Id`) ON DELETE CASCADE
);

CREATE TABLE `SurgicalProcedurePhases` (
    `SurgicalProcedureId` bigint(20) NOT NULL,
    `PhaseNumber` int(11) NOT NULL,
    `BaseLengthInSeconds` double NOT NULL,
    `PhaseEmote` text CHARACTER SET utf8mb4 NOT NULL,
    `PhaseSpecialEffects` varchar(500) CHARACTER SET utf8mb4 NULL,
    `OnPhaseProgId` bigint(20) NULL,
    `InventoryActionPlan` text CHARACTER SET utf8mb4 NULL,
    CONSTRAINT `PRIMARY` PRIMARY KEY (`SurgicalProcedureId`, `PhaseNumber`),
    CONSTRAINT `FK_SurgicalProcedurePhases_FutureProgs_OnPhaseProg` FOREIGN KEY (`OnPhaseProgId`) REFERENCES `FutureProgs` (`Id`) ON DELETE SET NULL,
    CONSTRAINT `FK_SurgicalProcedurePhases_SurgicalProcudres` FOREIGN KEY (`SurgicalProcedureId`) REFERENCES `SurgicalProcedures` (`Id`) ON DELETE CASCADE
);

CREATE TABLE `RaceButcheryProfiles_BreakdownChecks` (
    `RaceButcheryProfileId` bigint(20) NOT NULL,
    `Subcageory` varchar(100) CHARACTER SET utf8 NOT NULL,
    `TraitDefinitionId` bigint(20) NOT NULL,
    `Difficulty` int(11) NOT NULL,
    CONSTRAINT `PRIMARY` PRIMARY KEY (`RaceButcheryProfileId`, `Subcageory`),
    CONSTRAINT `FK_RaceButcheryProfiles_BreakdownChecks_RaceButcheryProfiles` FOREIGN KEY (`RaceButcheryProfileId`) REFERENCES `RaceButcheryProfiles` (`Id`) ON DELETE CASCADE,
    CONSTRAINT `FK_RaceButcheryProfiles_BreakdownChecks_TraitDefinitions` FOREIGN KEY (`TraitDefinitionId`) REFERENCES `TraitDefinitions` (`Id`) ON DELETE RESTRICT
);

CREATE TABLE `RaceButcheryProfiles_BreakdownEmotes` (
    `RaceButcheryProfileId` bigint(20) NOT NULL,
    `Subcategory` varchar(100) CHARACTER SET utf8 NOT NULL,
    `Order` int(11) NOT NULL,
    `Emote` text CHARACTER SET utf8 NOT NULL,
    `Delay` double NOT NULL,
    CONSTRAINT `PRIMARY` PRIMARY KEY (`RaceButcheryProfileId`, `Subcategory`, `Order`),
    CONSTRAINT `FK_RaceButcheryProfiles_BreakdownEmotes_RaceButcheryProfiles` FOREIGN KEY (`RaceButcheryProfileId`) REFERENCES `RaceButcheryProfiles` (`Id`) ON DELETE CASCADE
);

CREATE TABLE `RaceButcheryProfiles_SkinningEmotes` (
    `RaceButcheryProfileId` bigint(20) NOT NULL,
    `Subcategory` varchar(100) CHARACTER SET utf8 NOT NULL,
    `Order` int(11) NOT NULL,
    `Emote` text CHARACTER SET utf8 NOT NULL,
    `Delay` double NOT NULL,
    CONSTRAINT `PRIMARY` PRIMARY KEY (`RaceButcheryProfileId`, `Subcategory`, `Order`),
    CONSTRAINT `FK_RaceButcheryProfiles_SkinningEmotes_RaceButcheryProfiles` FOREIGN KEY (`RaceButcheryProfileId`) REFERENCES `RaceButcheryProfiles` (`Id`) ON DELETE CASCADE
);

CREATE TABLE `Gases_Tags` (
    `GasId` bigint(20) NOT NULL,
    `TagId` bigint(20) NOT NULL,
    CONSTRAINT `PRIMARY` PRIMARY KEY (`GasId`, `TagId`),
    CONSTRAINT `FK_Gases_Tags_Gases` FOREIGN KEY (`GasId`) REFERENCES `Gases` (`Id`) ON DELETE CASCADE,
    CONSTRAINT `FK_Gases_Tags_Tags` FOREIGN KEY (`TagId`) REFERENCES `Tags` (`Id`) ON DELETE CASCADE
);

CREATE TABLE `CraftInputs` (
    `Id` bigint(20) NOT NULL AUTO_INCREMENT,
    `CraftId` bigint(20) NOT NULL,
    `CraftRevisionNumber` int(11) NOT NULL,
    `InputType` varchar(45) CHARACTER SET utf8 NOT NULL,
    `InputQualityWeight` double NOT NULL,
    `Definition` text CHARACTER SET utf8 NOT NULL,
    `OriginalAdditionTime` datetime NOT NULL,
    CONSTRAINT `PK_CraftInputs` PRIMARY KEY (`Id`),
    CONSTRAINT `FK_CraftInputs_Crafts` FOREIGN KEY (`CraftId`, `CraftRevisionNumber`) REFERENCES `Crafts` (`Id`, `RevisionNumber`) ON DELETE CASCADE
);

CREATE TABLE `CraftPhases` (
    `CraftPhaseId` bigint(20) NOT NULL,
    `CraftPhaseRevisionNumber` int(11) NOT NULL,
    `PhaseNumber` int(11) NOT NULL,
    `PhaseLengthInSeconds` double NOT NULL,
    `Echo` text CHARACTER SET utf8 NOT NULL,
    `FailEcho` text CHARACTER SET utf8 NULL,
    CONSTRAINT `PRIMARY` PRIMARY KEY (`CraftPhaseId`, `CraftPhaseRevisionNumber`, `PhaseNumber`),
    CONSTRAINT `FK_CraftPhases_Crafts` FOREIGN KEY (`CraftPhaseId`, `CraftPhaseRevisionNumber`) REFERENCES `Crafts` (`Id`, `RevisionNumber`) ON DELETE CASCADE
);

CREATE TABLE `CraftProducts` (
    `Id` bigint(20) NOT NULL AUTO_INCREMENT,
    `CraftId` bigint(20) NOT NULL,
    `CraftRevisionNumber` int(11) NOT NULL,
    `ProductType` varchar(45) CHARACTER SET utf8 NOT NULL,
    `Definition` text CHARACTER SET utf8 NOT NULL,
    `OriginalAdditionTime` datetime NOT NULL,
    `IsFailProduct` bit(1) NOT NULL DEFAULT b'0',
    `MaterialDefiningInputIndex` int(11) NULL,
    CONSTRAINT `PK_CraftProducts` PRIMARY KEY (`Id`),
    CONSTRAINT `FK_CraftProducts_Crafts` FOREIGN KEY (`CraftId`, `CraftRevisionNumber`) REFERENCES `Crafts` (`Id`, `RevisionNumber`) ON DELETE CASCADE
);

CREATE TABLE `CraftTools` (
    `Id` bigint(20) NOT NULL AUTO_INCREMENT,
    `CraftId` bigint(20) NOT NULL,
    `CraftRevisionNumber` int(11) NOT NULL,
    `OriginalAdditionTime` datetime NOT NULL,
    `ToolType` varchar(45) CHARACTER SET utf8 NOT NULL,
    `ToolQualityWeight` double NOT NULL,
    `DesiredState` int(11) NOT NULL,
    `Definition` text CHARACTER SET utf8 NOT NULL,
    CONSTRAINT `PK_CraftTools` PRIMARY KEY (`Id`),
    CONSTRAINT `FK_CraftTools_Crafts` FOREIGN KEY (`CraftId`, `CraftRevisionNumber`) REFERENCES `Crafts` (`Id`, `RevisionNumber`) ON DELETE CASCADE
);

CREATE TABLE `WeaponAttacks` (
    `Id` bigint(20) NOT NULL AUTO_INCREMENT,
    `WeaponTypeId` bigint(20) NULL,
    `Verb` int(11) NOT NULL,
    `FutureProgId` bigint(20) NULL,
    `BaseAttackerDifficulty` int(11) NOT NULL DEFAULT '5',
    `BaseBlockDifficulty` int(11) NOT NULL DEFAULT '5',
    `BaseDodgeDifficulty` int(11) NOT NULL DEFAULT '5',
    `BaseParryDifficulty` int(11) NOT NULL DEFAULT '5',
    `BaseAngleOfIncidence` double NOT NULL DEFAULT '1.5708',
    `RecoveryDifficultySuccess` int(11) NOT NULL DEFAULT '5',
    `RecoveryDifficultyFailure` int(11) NOT NULL DEFAULT '5',
    `MoveType` int(11) NOT NULL,
    `Intentions` bigint(20) NOT NULL,
    `ExertionLevel` int(11) NOT NULL,
    `DamageType` int(11) NOT NULL,
    `DamageExpressionId` bigint(20) NOT NULL,
    `StunExpressionId` bigint(20) NOT NULL,
    `PainExpressionId` bigint(20) NOT NULL,
    `Weighting` double NOT NULL DEFAULT '1',
    `BodypartShapeId` bigint(20) NULL,
    `StaminaCost` double NOT NULL,
    `BaseDelay` double NOT NULL DEFAULT '1',
    `Name` text CHARACTER SET utf8mb4 NULL,
    `Orientation` int(11) NOT NULL,
    `Alignment` int(11) NOT NULL,
    `AdditionalInfo` text CHARACTER SET utf8mb4 NULL,
    `HandednessOptions` int(11) NOT NULL,
    CONSTRAINT `PK_WeaponAttacks` PRIMARY KEY (`Id`),
    CONSTRAINT `FK_WeaponAttacks_TraitExpression_Damage` FOREIGN KEY (`DamageExpressionId`) REFERENCES `TraitExpression` (`Id`) ON DELETE CASCADE,
    CONSTRAINT `FK_WeaponAttacks_FutureProgs` FOREIGN KEY (`FutureProgId`) REFERENCES `FutureProgs` (`Id`) ON DELETE SET NULL,
    CONSTRAINT `FK_WeaponAttacks_TraitExpression_Pain` FOREIGN KEY (`PainExpressionId`) REFERENCES `TraitExpression` (`Id`) ON DELETE CASCADE,
    CONSTRAINT `FK_WeaponAttacks_TraitExpression_Stun` FOREIGN KEY (`StunExpressionId`) REFERENCES `TraitExpression` (`Id`) ON DELETE CASCADE,
    CONSTRAINT `FK_WeaponAttacks_WeaponTypes` FOREIGN KEY (`WeaponTypeId`) REFERENCES `WeaponTypes` (`Id`) ON DELETE CASCADE
);

CREATE TABLE `Terrains_RangedCovers` (
    `TerrainId` bigint(20) NOT NULL,
    `RangedCoverId` bigint(20) NOT NULL,
    CONSTRAINT `PRIMARY` PRIMARY KEY (`TerrainId`, `RangedCoverId`),
    CONSTRAINT `FK_Terrains_RangedCovers_RangedCovers` FOREIGN KEY (`RangedCoverId`) REFERENCES `RangedCovers` (`Id`) ON DELETE CASCADE,
    CONSTRAINT `FK_Terrains_RangedCovers_Terrains` FOREIGN KEY (`TerrainId`) REFERENCES `Terrains` (`Id`) ON DELETE CASCADE
);

CREATE TABLE `ProjectLabourImpacts` (
    `Id` bigint(20) NOT NULL AUTO_INCREMENT,
    `Name` varchar(200) CHARACTER SET utf8 NOT NULL,
    `Description` varchar(1000) CHARACTER SET utf8 NOT NULL,
    `Type` varchar(45) CHARACTER SET utf8 NOT NULL,
    `Definition` text CHARACTER SET utf8 NOT NULL,
    `ProjectLabourRequirementId` bigint(20) NOT NULL,
    `MinimumHoursForImpactToKickIn` double NOT NULL,
    CONSTRAINT `PK_ProjectLabourImpacts` PRIMARY KEY (`Id`),
    CONSTRAINT `FK_ProjectLabourImpacts_ProjectLabourRequirements` FOREIGN KEY (`ProjectLabourRequirementId`) REFERENCES `ProjectLabourRequirements` (`Id`) ON DELETE CASCADE
);

CREATE TABLE `CombatMessages_WeaponAttacks` (
    `CombatMessageId` bigint(20) NOT NULL,
    `WeaponAttackId` bigint(20) NOT NULL,
    CONSTRAINT `PRIMARY` PRIMARY KEY (`CombatMessageId`, `WeaponAttackId`),
    CONSTRAINT `FK_CombatMessages_WeaponAttacks_CombatMessages` FOREIGN KEY (`CombatMessageId`) REFERENCES `CombatMessages` (`Id`) ON DELETE CASCADE,
    CONSTRAINT `FK_CombatMessages_WeaponAttacks_WeaponAttacks` FOREIGN KEY (`WeaponAttackId`) REFERENCES `WeaponAttacks` (`Id`) ON DELETE CASCADE
);

CREATE TABLE `Characters` (
    `Id` bigint(20) NOT NULL AUTO_INCREMENT,
    `Name` varchar(4000) CHARACTER SET utf8 NOT NULL,
    `AccountId` bigint(20) NULL,
    `CreationTime` datetime NOT NULL,
    `DeathTime` datetime NULL,
    `Status` int(11) NOT NULL,
    `State` int(11) NOT NULL,
    `Gender` smallint(6) NOT NULL,
    `Location` bigint(20) NOT NULL,
    `BodyId` bigint(20) NOT NULL,
    `CultureId` bigint(20) NOT NULL,
    `EffectData` mediumtext CHARACTER SET utf8 NOT NULL,
    `BirthdayDate` varchar(4000) CHARACTER SET utf8 NOT NULL,
    `BirthdayCalendarId` bigint(20) NOT NULL,
    `IsAdminAvatar` bit(1) NOT NULL DEFAULT b'0',
    `CurrencyId` bigint(20) NULL,
    `TotalMinutesPlayed` int(11) NOT NULL,
    `AlcoholLitres` double NOT NULL,
    `WaterLitres` double NOT NULL,
    `FoodSatiatedHours` double NOT NULL,
    `DrinkSatiatedHours` double NOT NULL,
    `Calories` double NOT NULL,
    `NeedsModel` varchar(50) CHARACTER SET utf8 NOT NULL DEFAULT 'NoNeeds',
    `LongTermPlan` text CHARACTER SET utf8 NULL,
    `ShortTermPlan` text CHARACTER SET utf8 NULL,
    `ShownIntroductionMessage` bit(1) NOT NULL DEFAULT b'0',
    `IntroductionMessage` text CHARACTER SET utf8 NULL,
    `ChargenId` bigint(20) NULL,
    `CurrentCombatSettingId` bigint(20) NULL,
    `PreferredDefenseType` int(11) NOT NULL,
    `PositionId` int(11) NOT NULL DEFAULT '1',
    `PositionModifier` int(11) NOT NULL,
    `PositionTargetId` bigint(20) NULL,
    `PositionTargetType` varchar(45) CHARACTER SET utf8 NULL,
    `PositionEmote` text CHARACTER SET utf8 NULL,
    `CurrentLanguageId` bigint(20) NULL,
    `CurrentAccentId` bigint(20) NULL,
    `CurrentWritingLanguageId` bigint(20) NULL,
    `WritingStyle` int(11) NOT NULL DEFAULT '8256',
    `CurrentScriptId` bigint(20) NULL,
    `DominantHandAlignment` int(11) NOT NULL DEFAULT '3',
    `LastLoginTime` datetime NULL,
    `CombatBrief` bit(1) NOT NULL DEFAULT b'0',
    `RoomBrief` bit(1) NOT NULL DEFAULT b'0',
    `LastLogoutTime` datetime NULL,
    `Outfits` text CHARACTER SET utf8 NULL,
    `CurrentProjectLabourId` bigint(20) NULL,
    `CurrentProjectId` bigint(20) NULL,
    `CurrentProjectHours` double NOT NULL,
    `NameInfo` text CHARACTER SET utf8 NULL,
    `RoomLayer` int(11) NOT NULL,
    `NoMercy` bit(1) NOT NULL DEFAULT b'0',
    CONSTRAINT `PK_Characters` PRIMARY KEY (`Id`),
    CONSTRAINT `FK_Characters_Accounts` FOREIGN KEY (`AccountId`) REFERENCES `Accounts` (`Id`) ON DELETE CASCADE,
    CONSTRAINT `FK_Characters_Chargens` FOREIGN KEY (`ChargenId`) REFERENCES `Chargens` (`Id`) ON DELETE SET NULL,
    CONSTRAINT `FK_Characters_Cultures` FOREIGN KEY (`CultureId`) REFERENCES `Cultures` (`Id`) ON DELETE RESTRICT,
    CONSTRAINT `FK_Characters_Currencies` FOREIGN KEY (`CurrencyId`) REFERENCES `Currencies` (`Id`) ON DELETE RESTRICT,
    CONSTRAINT `FK_Characters_ProjectLabourRequirements` FOREIGN KEY (`CurrentProjectLabourId`) REFERENCES `ProjectLabourRequirements` (`Id`) ON DELETE SET NULL,
    CONSTRAINT `FK_Characters_Scripts` FOREIGN KEY (`CurrentScriptId`) REFERENCES `Scripts` (`Id`) ON DELETE SET NULL
);

CREATE TABLE `Allies` (
    `CharacterId` bigint(20) NOT NULL,
    `AllyId` bigint(20) NOT NULL,
    `Trusted` bit(1) NOT NULL DEFAULT b'0',
    CONSTRAINT `PRIMARY` PRIMARY KEY (`CharacterId`, `AllyId`),
    CONSTRAINT `FK_Allies_Characters_Target` FOREIGN KEY (`AllyId`) REFERENCES `Characters` (`Id`) ON DELETE CASCADE,
    CONSTRAINT `FK_Allies_Characters_Owner` FOREIGN KEY (`CharacterId`) REFERENCES `Characters` (`Id`) ON DELETE CASCADE
);

CREATE TABLE `CharacterCombatSettings` (
    `Id` bigint(20) NOT NULL AUTO_INCREMENT,
    `Name` text CHARACTER SET utf8mb4 NOT NULL,
    `Description` text CHARACTER SET utf8mb4 NOT NULL,
    `GlobalTemplate` bit(1) NOT NULL DEFAULT b'0',
    `AvailabilityProgId` bigint(20) NULL,
    `CharacterOwnerId` bigint(20) NULL,
    `WeaponUsePercentage` double NOT NULL,
    `MagicUsePercentage` double NOT NULL,
    `PsychicUsePercentage` double NOT NULL,
    `NaturalWeaponPercentage` double NOT NULL,
    `AuxillaryPercentage` double NOT NULL,
    `PreferToFightArmed` bit(1) NOT NULL DEFAULT b'0',
    `PreferFavouriteWeapon` bit(1) NOT NULL DEFAULT b'0',
    `PreferShieldUse` bit(1) NOT NULL DEFAULT b'0',
    `ClassificationsAllowed` text CHARACTER SET utf8mb4 NOT NULL,
    `RequiredIntentions` bigint(20) NOT NULL,
    `ForbiddenIntentions` bigint(20) NOT NULL,
    `PreferredIntentions` bigint(20) NOT NULL,
    `AttackUnarmedOrHelpless` bit(1) NOT NULL DEFAULT b'0',
    `FallbackToUnarmedIfNoWeapon` bit(1) NOT NULL DEFAULT b'0',
    `AttackCriticallyInjured` bit(1) NOT NULL DEFAULT b'0',
    `SkirmishToOtherLocations` bit(1) NOT NULL DEFAULT b'0',
    `PursuitMode` int(11) NOT NULL DEFAULT '1',
    `DefaultPreferredDefenseType` int(11) NOT NULL,
    `PreferredMeleeMode` int(11) NOT NULL,
    `PreferredRangedMode` int(11) NOT NULL DEFAULT '1',
    `AutomaticallyMoveTowardsTarget` bit(1) NOT NULL DEFAULT b'0',
    `PreferNonContactClinchBreaking` bit(1) NOT NULL DEFAULT b'1',
    `InventoryManagement` int(11) NOT NULL,
    `MovementManagement` int(11) NOT NULL,
    `RangedManagement` int(11) NOT NULL,
    `ManualPositionManagement` bit(1) NOT NULL DEFAULT b'0',
    `MinimumStaminaToAttack` double NOT NULL,
    `MoveToMeleeIfCannotEngageInRangedCombat` bit(1) NOT NULL DEFAULT b'1',
    `PreferredWeaponSetup` int(11) NOT NULL,
    `RequiredMinimumAim` double NOT NULL DEFAULT '0.5',
    `MeleeAttackOrderPreference` varchar(100) CHARACTER SET utf8mb4 NOT NULL DEFAULT '0 1 2 3 4',
    `GrappleResponse` int(11) NOT NULL,
    CONSTRAINT `PK_CharacterCombatSettings` PRIMARY KEY (`Id`),
    CONSTRAINT `FK_CharacterCombatSettings_FutureProgs` FOREIGN KEY (`AvailabilityProgId`) REFERENCES `FutureProgs` (`Id`) ON DELETE SET NULL,
    CONSTRAINT `FK_CharacterCombatSettings_Characters` FOREIGN KEY (`CharacterOwnerId`) REFERENCES `Characters` (`Id`) ON DELETE CASCADE
);

CREATE TABLE `CharacterKnowledges` (
    `Id` bigint(20) NOT NULL AUTO_INCREMENT,
    `CharacterId` bigint(20) NOT NULL,
    `KnowledgeId` bigint(20) NOT NULL,
    `WhenAcquired` datetime NOT NULL,
    `HowAcquired` varchar(200) CHARACTER SET utf8 NOT NULL,
    `TimesTaught` int(11) NOT NULL,
    CONSTRAINT `PK_CharacterKnowledges` PRIMARY KEY (`Id`),
    CONSTRAINT `FK_CHARACTERKNOWLEDGES_CHARACTERS` FOREIGN KEY (`CharacterId`) REFERENCES `Characters` (`Id`) ON DELETE CASCADE,
    CONSTRAINT `FK_CHARACTERKNOWLEDGES_KNOWLEDGES` FOREIGN KEY (`KnowledgeId`) REFERENCES `knowledges` (`Id`) ON DELETE CASCADE
);

CREATE TABLE `Characters_ChargenRoles` (
    `CharacterId` bigint(20) NOT NULL,
    `ChargenRoleId` bigint(20) NOT NULL,
    CONSTRAINT `PRIMARY` PRIMARY KEY (`CharacterId`, `ChargenRoleId`),
    CONSTRAINT `FK_Characters_ChargenRoles_Characters` FOREIGN KEY (`CharacterId`) REFERENCES `Characters` (`Id`) ON DELETE CASCADE,
    CONSTRAINT `FK_Characters_ChargenRoles_ChargenRoles` FOREIGN KEY (`ChargenRoleId`) REFERENCES `ChargenRoles` (`Id`) ON DELETE CASCADE
);

CREATE TABLE `Characters_MagicResources` (
    `CharacterId` bigint(20) NOT NULL,
    `MagicResourceId` bigint(20) NOT NULL,
    `Amount` double NOT NULL,
    CONSTRAINT `PRIMARY` PRIMARY KEY (`CharacterId`, `MagicResourceId`),
    CONSTRAINT `FK_Characters_MagicResources_Characters` FOREIGN KEY (`CharacterId`) REFERENCES `Characters` (`Id`) ON DELETE CASCADE,
    CONSTRAINT `FK_Characters_MagicResources_MagicResources` FOREIGN KEY (`MagicResourceId`) REFERENCES `MagicResources` (`Id`) ON DELETE CASCADE
);

CREATE TABLE `Characters_Scripts` (
    `CharacterId` bigint(20) NOT NULL,
    `ScriptId` bigint(20) NOT NULL,
    CONSTRAINT `PRIMARY` PRIMARY KEY (`CharacterId`, `ScriptId`),
    CONSTRAINT `FK_Characters_Scripts_Characters` FOREIGN KEY (`CharacterId`) REFERENCES `Characters` (`Id`) ON DELETE CASCADE,
    CONSTRAINT `FK_Characters_Scripts_Scripts` FOREIGN KEY (`ScriptId`) REFERENCES `Scripts` (`Id`) ON DELETE CASCADE
);

CREATE TABLE `Clans` (
    `Id` bigint(20) NOT NULL AUTO_INCREMENT,
    `Name` varchar(100) CHARACTER SET utf8 NOT NULL,
    `Alias` varchar(100) CHARACTER SET utf8 NOT NULL,
    `FullName` varchar(4000) CHARACTER SET utf8 NULL,
    `Description` varchar(4000) CHARACTER SET utf8 NULL,
    `ParentClanId` bigint(20) NULL,
    `PayIntervalType` int(11) NOT NULL,
    `PayIntervalModifier` int(11) NOT NULL,
    `PayIntervalOther` int(11) NOT NULL,
    `CalendarId` bigint(20) NOT NULL,
    `PayIntervalReferenceDate` varchar(100) CHARACTER SET utf8 NOT NULL,
    `PayIntervalReferenceTime` varchar(100) CHARACTER SET utf8 NOT NULL,
    `IsTemplate` bit(1) NOT NULL DEFAULT b'0',
    `ShowClanMembersInWho` bit(1) NOT NULL DEFAULT b'0',
    `PaymasterId` bigint(20) NULL,
    `PaymasterItemProtoId` bigint(20) NULL,
    `OnPayProgId` bigint(20) NULL,
    `MaximumPeriodsOfUncollectedBackPay` int(11) NULL,
    CONSTRAINT `PK_Clans` PRIMARY KEY (`Id`),
    CONSTRAINT `FK_Clans_Calendars` FOREIGN KEY (`CalendarId`) REFERENCES `Calendars` (`Id`) ON DELETE RESTRICT,
    CONSTRAINT `FK_Clans_FutureProgs` FOREIGN KEY (`OnPayProgId`) REFERENCES `FutureProgs` (`Id`) ON DELETE SET NULL,
    CONSTRAINT `FK_Clans_Parent` FOREIGN KEY (`ParentClanId`) REFERENCES `Clans` (`Id`) ON DELETE RESTRICT,
    CONSTRAINT `FK_Clans_Characters` FOREIGN KEY (`PaymasterId`) REFERENCES `Characters` (`Id`) ON DELETE SET NULL
);

CREATE TABLE `Drawings` (
    `Id` bigint(20) NOT NULL AUTO_INCREMENT,
    `AuthorId` bigint(20) NOT NULL,
    `ShortDescription` text CHARACTER SET utf8 NOT NULL,
    `FullDescription` text CHARACTER SET utf8 NOT NULL,
    `ImplementType` int(11) NOT NULL,
    `DrawingSkill` double NOT NULL,
    `DrawingSize` int(11) NOT NULL,
    CONSTRAINT `PK_Drawings` PRIMARY KEY (`Id`),
    CONSTRAINT `FK_Drawings_Characters` FOREIGN KEY (`AuthorId`) REFERENCES `Characters` (`Id`) ON DELETE RESTRICT
);

CREATE TABLE `Dreams_Already_Dreamt` (
    `DreamId` bigint(20) NOT NULL,
    `CharacterId` bigint(20) NOT NULL,
    CONSTRAINT `PRIMARY` PRIMARY KEY (`DreamId`, `CharacterId`),
    CONSTRAINT `FK_Dreams_Dreamt_Characters` FOREIGN KEY (`CharacterId`) REFERENCES `Characters` (`Id`) ON DELETE CASCADE,
    CONSTRAINT `FK_Dreams_Dreamt_Dreams` FOREIGN KEY (`DreamId`) REFERENCES `Dreams` (`Id`) ON DELETE CASCADE
);

CREATE TABLE `Dreams_Characters` (
    `DreamId` bigint(20) NOT NULL,
    `CharacterId` bigint(20) NOT NULL,
    CONSTRAINT `PRIMARY` PRIMARY KEY (`DreamId`, `CharacterId`),
    CONSTRAINT `FK_Dreams_Characters_Characters` FOREIGN KEY (`CharacterId`) REFERENCES `Characters` (`Id`) ON DELETE CASCADE,
    CONSTRAINT `FK_Dreams_Characters_Dreams` FOREIGN KEY (`DreamId`) REFERENCES `Dreams` (`Id`) ON DELETE CASCADE
);

CREATE TABLE `Dubs` (
    `Id` bigint(20) NOT NULL AUTO_INCREMENT,
    `Keywords` varchar(4000) CHARACTER SET utf8 NOT NULL,
    `TargetId` bigint(20) NOT NULL,
    `TargetType` varchar(100) CHARACTER SET utf8 NOT NULL,
    `LastDescription` varchar(4000) CHARACTER SET utf8 NOT NULL,
    `LastUsage` datetime NOT NULL,
    `CharacterId` bigint(20) NOT NULL,
    `IntroducedName` varchar(4000) CHARACTER SET utf8 NULL,
    CONSTRAINT `PK_Dubs` PRIMARY KEY (`Id`),
    CONSTRAINT `FK_Dubs_Characters` FOREIGN KEY (`CharacterId`) REFERENCES `Characters` (`Id`) ON DELETE CASCADE
);

CREATE TABLE `Guests` (
    `CharacterId` bigint(20) NOT NULL,
    CONSTRAINT `PRIMARY` PRIMARY KEY (`CharacterId`),
    CONSTRAINT `FK_Guests_Characters` FOREIGN KEY (`CharacterId`) REFERENCES `Characters` (`Id`) ON DELETE CASCADE
);

CREATE TABLE `NPCs` (
    `Id` bigint(20) NOT NULL AUTO_INCREMENT,
    `CharacterId` bigint(20) NOT NULL,
    `TemplateId` bigint(20) NOT NULL,
    `TemplateRevnum` int(11) NOT NULL,
    `BodyguardCharacterId` bigint(20) NULL,
    CONSTRAINT `PK_NPCs` PRIMARY KEY (`Id`),
    CONSTRAINT `FK_NPCs_Characters_Bodyguard` FOREIGN KEY (`BodyguardCharacterId`) REFERENCES `Characters` (`Id`) ON DELETE SET NULL,
    CONSTRAINT `FK_NPCs_Characters` FOREIGN KEY (`CharacterId`) REFERENCES `Characters` (`Id`) ON DELETE CASCADE,
    CONSTRAINT `FK_NPCs_NPCTemplates` FOREIGN KEY (`TemplateId`, `TemplateRevnum`) REFERENCES `NPCTemplates` (`Id`, `RevisionNumber`) ON DELETE CASCADE
);

CREATE TABLE `ChargenRoles_ClanMemberships` (
    `ChargenRoleId` bigint(20) NOT NULL,
    `ClanId` bigint(20) NOT NULL,
    `RankId` bigint(20) NOT NULL,
    `PaygradeId` bigint(20) NULL,
    CONSTRAINT `PRIMARY` PRIMARY KEY (`ChargenRoleId`, `ClanId`),
    CONSTRAINT `FK_ChargenRoles_ClanMemberships_ChargenRoles` FOREIGN KEY (`ChargenRoleId`) REFERENCES `ChargenRoles` (`Id`) ON DELETE CASCADE,
    CONSTRAINT `FK_ChargenRoles_ClanMemberships_Clans` FOREIGN KEY (`ClanId`) REFERENCES `Clans` (`Id`) ON DELETE CASCADE
);

CREATE TABLE `ClanMemberships` (
    `ClanId` bigint(20) NOT NULL,
    `CharacterId` bigint(20) NOT NULL,
    `RankId` bigint(20) NOT NULL,
    `PaygradeId` bigint(20) NULL,
    `JoinDate` varchar(100) CHARACTER SET utf8 NOT NULL,
    `ManagerId` bigint(20) NULL,
    `PersonalName` text CHARACTER SET utf8 NULL,
    CONSTRAINT `PRIMARY` PRIMARY KEY (`ClanId`, `CharacterId`),
    CONSTRAINT `FK_ClanMemberships_Characters` FOREIGN KEY (`CharacterId`) REFERENCES `Characters` (`Id`) ON DELETE CASCADE,
    CONSTRAINT `FK_ClanMemberships_Clans` FOREIGN KEY (`ClanId`) REFERENCES `Clans` (`Id`) ON DELETE CASCADE,
    CONSTRAINT `FK_ClanMemberships_Manager` FOREIGN KEY (`ManagerId`) REFERENCES `Characters` (`Id`) ON DELETE RESTRICT
);

CREATE TABLE `Paygrades` (
    `Id` bigint(20) NOT NULL AUTO_INCREMENT,
    `Name` varchar(100) CHARACTER SET utf8 NOT NULL,
    `Abbreviation` varchar(100) CHARACTER SET utf8 NOT NULL,
    `CurrencyId` bigint(20) NOT NULL,
    `PayAmount` decimal(18,0) NOT NULL,
    `ClanId` bigint(20) NOT NULL,
    CONSTRAINT `PK_Paygrades` PRIMARY KEY (`Id`),
    CONSTRAINT `FK_Paygrades_Clans` FOREIGN KEY (`ClanId`) REFERENCES `Clans` (`Id`) ON DELETE RESTRICT,
    CONSTRAINT `FK_Paygrades_Currencies` FOREIGN KEY (`CurrencyId`) REFERENCES `Currencies` (`Id`) ON DELETE RESTRICT
);

CREATE TABLE `Ranks` (
    `Id` bigint(20) NOT NULL AUTO_INCREMENT,
    `Name` varchar(100) CHARACTER SET utf8 NOT NULL,
    `InsigniaGameItemId` bigint(20) NULL,
    `InsigniaGameItemRevnum` int(11) NULL,
    `ClanId` bigint(20) NOT NULL,
    `Privileges` bigint(20) NOT NULL,
    `RankPath` varchar(50) CHARACTER SET utf8 NULL,
    `RankNumber` int(11) NOT NULL,
    CONSTRAINT `PK_Ranks` PRIMARY KEY (`Id`),
    CONSTRAINT `FK_Ranks_Clans` FOREIGN KEY (`ClanId`) REFERENCES `Clans` (`Id`) ON DELETE CASCADE,
    CONSTRAINT `FK_Ranks_GameItemProtos` FOREIGN KEY (`InsigniaGameItemId`, `InsigniaGameItemRevnum`) REFERENCES `GameItemProtos` (`Id`, `RevisionNumber`) ON DELETE RESTRICT
);

CREATE TABLE `NPCs_ArtificialIntelligences` (
    `NPCId` bigint(20) NOT NULL,
    `ArtificialIntelligenceId` bigint(20) NOT NULL,
    CONSTRAINT `PRIMARY` PRIMARY KEY (`ArtificialIntelligenceId`, `NPCId`),
    CONSTRAINT `FK_NPCs_ArtificialIntelligences_ArtificialIntelligences` FOREIGN KEY (`ArtificialIntelligenceId`) REFERENCES `ArtificialIntelligences` (`Id`) ON DELETE CASCADE,
    CONSTRAINT `FK_NPCs_ArtificialIntelligences_NPCs` FOREIGN KEY (`NPCId`) REFERENCES `NPCs` (`Id`) ON DELETE CASCADE
);

CREATE TABLE `ChargenRoles_ClanMemberships_Appointments` (
    `ChargenRoleId` bigint(20) NOT NULL,
    `ClanId` bigint(20) NOT NULL,
    `AppointmentId` bigint(20) NOT NULL,
    CONSTRAINT `PRIMARY` PRIMARY KEY (`ChargenRoleId`, `ClanId`, `AppointmentId`),
    CONSTRAINT `FK_CRCMA_ChargenRoles_ClanMemberships` FOREIGN KEY (`ChargenRoleId`, `ClanId`) REFERENCES `ChargenRoles_ClanMemberships` (`ChargenRoleId`, `ClanId`) ON DELETE CASCADE
);

CREATE TABLE `ClanMemberships_Backpay` (
    `ClanId` bigint(20) NOT NULL,
    `CharacterId` bigint(20) NOT NULL,
    `CurrencyId` bigint(20) NOT NULL,
    `Amount` decimal(18,2) NOT NULL,
    CONSTRAINT `PRIMARY` PRIMARY KEY (`CurrencyId`, `ClanId`, `CharacterId`),
    CONSTRAINT `FK_ClanMemberships_Backpay_Currencies` FOREIGN KEY (`CurrencyId`) REFERENCES `Currencies` (`Id`) ON DELETE CASCADE,
    CONSTRAINT `FK_ClanMemberships_Backpay_ClanMemberships` FOREIGN KEY (`ClanId`, `CharacterId`) REFERENCES `ClanMemberships` (`ClanId`, `CharacterId`) ON DELETE CASCADE
);

CREATE TABLE `Appointments` (
    `Id` bigint(20) NOT NULL AUTO_INCREMENT,
    `Name` varchar(100) CHARACTER SET utf8 NOT NULL,
    `MaximumSimultaneousHolders` int(11) NOT NULL DEFAULT '1',
    `MinimumRankId` bigint(20) NULL,
    `ParentAppointmentId` bigint(20) NULL,
    `PaygradeId` bigint(20) NULL,
    `InsigniaGameItemId` bigint(20) NULL,
    `InsigniaGameItemRevnum` int(11) NULL,
    `ClanId` bigint(20) NOT NULL,
    `Privileges` bigint(20) NOT NULL,
    `MinimumRankToAppointId` bigint(20) NULL,
    CONSTRAINT `PK_Appointments` PRIMARY KEY (`Id`),
    CONSTRAINT `FK_Appointments_Clans` FOREIGN KEY (`ClanId`) REFERENCES `Clans` (`Id`) ON DELETE RESTRICT,
    CONSTRAINT `FK_Appointments_Ranks` FOREIGN KEY (`MinimumRankId`) REFERENCES `Ranks` (`Id`) ON DELETE SET NULL,
    CONSTRAINT `FK_Appointments_Ranks_2` FOREIGN KEY (`MinimumRankToAppointId`) REFERENCES `Ranks` (`Id`) ON DELETE RESTRICT,
    CONSTRAINT `FK_Appointments_ParentAppointment` FOREIGN KEY (`ParentAppointmentId`) REFERENCES `Appointments` (`Id`) ON DELETE RESTRICT,
    CONSTRAINT `FK_Appointments_Paygrades` FOREIGN KEY (`PaygradeId`) REFERENCES `Paygrades` (`Id`) ON DELETE SET NULL,
    CONSTRAINT `FK_Appointments_GameItemProtos` FOREIGN KEY (`InsigniaGameItemId`, `InsigniaGameItemRevnum`) REFERENCES `GameItemProtos` (`Id`, `RevisionNumber`) ON DELETE RESTRICT
);

CREATE TABLE `Ranks_Abbreviations` (
    `RankId` bigint(20) NOT NULL,
    `Abbreviation` varchar(50) CHARACTER SET utf8 NOT NULL,
    `FutureProgId` bigint(20) NULL,
    `Order` int(11) NOT NULL,
    CONSTRAINT `PRIMARY` PRIMARY KEY (`RankId`, `Abbreviation`),
    CONSTRAINT `FK_Ranks_Abbreviations_FutureProgs` FOREIGN KEY (`FutureProgId`) REFERENCES `FutureProgs` (`Id`) ON DELETE SET NULL,
    CONSTRAINT `FK_Ranks_Abbreviations_Ranks` FOREIGN KEY (`RankId`) REFERENCES `Ranks` (`Id`) ON DELETE CASCADE
);

CREATE TABLE `Ranks_Paygrades` (
    `RankId` bigint(20) NOT NULL,
    `PaygradeId` bigint(20) NOT NULL,
    `Order` int(11) NOT NULL,
    CONSTRAINT `PRIMARY` PRIMARY KEY (`RankId`, `PaygradeId`),
    CONSTRAINT `FK_Ranks_Paygrades_Paygrades` FOREIGN KEY (`PaygradeId`) REFERENCES `Paygrades` (`Id`) ON DELETE CASCADE,
    CONSTRAINT `FK_Ranks_Paygrades_Ranks` FOREIGN KEY (`RankId`) REFERENCES `Ranks` (`Id`) ON DELETE CASCADE
);

CREATE TABLE `Ranks_Titles` (
    `RankId` bigint(20) NOT NULL,
    `Title` varchar(50) CHARACTER SET utf8 NOT NULL,
    `FutureProgId` bigint(20) NULL,
    `Order` int(11) NOT NULL,
    CONSTRAINT `PRIMARY` PRIMARY KEY (`RankId`, `Title`),
    CONSTRAINT `FK_Ranks_Titles_FutureProgs` FOREIGN KEY (`FutureProgId`) REFERENCES `FutureProgs` (`Id`) ON DELETE SET NULL,
    CONSTRAINT `FK_Ranks_Titles_Ranks` FOREIGN KEY (`RankId`) REFERENCES `Ranks` (`Id`) ON DELETE CASCADE
);

CREATE TABLE `Appointments_Abbreviations` (
    `AppointmentId` bigint(20) NOT NULL,
    `Abbreviation` varchar(50) CHARACTER SET utf8 NOT NULL,
    `FutureProgId` bigint(20) NULL,
    `Order` int(11) NOT NULL,
    CONSTRAINT `PRIMARY` PRIMARY KEY (`Abbreviation`, `AppointmentId`),
    CONSTRAINT `FK_Appointments_Abbreviations_Appointments` FOREIGN KEY (`AppointmentId`) REFERENCES `Appointments` (`Id`) ON DELETE RESTRICT,
    CONSTRAINT `FK_Appointments_Abbreviations_FutureProgs` FOREIGN KEY (`FutureProgId`) REFERENCES `FutureProgs` (`Id`) ON DELETE RESTRICT
);

CREATE TABLE `Appointments_Titles` (
    `AppointmentId` bigint(20) NOT NULL,
    `Title` varchar(200) CHARACTER SET utf8 NOT NULL,
    `FutureProgId` bigint(20) NULL,
    `Order` int(11) NULL,
    CONSTRAINT `PRIMARY` PRIMARY KEY (`Title`, `AppointmentId`),
    CONSTRAINT `FK_Appointments_Titles_Appointments` FOREIGN KEY (`AppointmentId`) REFERENCES `Appointments` (`Id`) ON DELETE RESTRICT,
    CONSTRAINT `FK_Appointments_Titles_FutureProgs` FOREIGN KEY (`FutureProgId`) REFERENCES `FutureProgs` (`Id`) ON DELETE RESTRICT
);

CREATE TABLE `ClanMemberships_Appointments` (
    `ClanId` bigint(20) NOT NULL,
    `CharacterId` bigint(20) NOT NULL,
    `AppointmentId` bigint(20) NOT NULL,
    CONSTRAINT `PRIMARY` PRIMARY KEY (`ClanId`, `CharacterId`, `AppointmentId`),
    CONSTRAINT `FK_ClanMemberships_Appointments_Appointments` FOREIGN KEY (`AppointmentId`) REFERENCES `Appointments` (`Id`) ON DELETE CASCADE,
    CONSTRAINT `FK_ClanMemberships_Appointments_ClanMemberships` FOREIGN KEY (`ClanId`, `CharacterId`) REFERENCES `ClanMemberships` (`ClanId`, `CharacterId`) ON DELETE CASCADE
);

CREATE TABLE `ExternalClanControls` (
    `VassalClanId` bigint(20) NOT NULL,
    `LiegeClanId` bigint(20) NOT NULL,
    `ControlledAppointmentId` bigint(20) NOT NULL,
    `ControllingAppointmentId` bigint(20) NULL,
    `NumberOfAppointments` int(11) NOT NULL,
    CONSTRAINT `PRIMARY` PRIMARY KEY (`VassalClanId`, `LiegeClanId`, `ControlledAppointmentId`),
    CONSTRAINT `FK_ECC_Appointments_Controlled` FOREIGN KEY (`ControlledAppointmentId`) REFERENCES `Appointments` (`Id`) ON DELETE RESTRICT,
    CONSTRAINT `FK_ECC_Appointments_Controlling` FOREIGN KEY (`ControllingAppointmentId`) REFERENCES `Appointments` (`Id`) ON DELETE RESTRICT,
    CONSTRAINT `FK_ECC_Clans_Liege` FOREIGN KEY (`LiegeClanId`) REFERENCES `Clans` (`Id`) ON DELETE RESTRICT,
    CONSTRAINT `FK_ECC_Clans_Vassal` FOREIGN KEY (`VassalClanId`) REFERENCES `Clans` (`Id`) ON DELETE RESTRICT
);

CREATE TABLE `ExternalClanControls_Appointments` (
    `VassalClanId` bigint(20) NOT NULL,
    `LiegeClanId` bigint(20) NOT NULL,
    `ControlledAppointmentId` bigint(20) NOT NULL,
    `CharacterId` bigint(20) NOT NULL,
    CONSTRAINT `PRIMARY` PRIMARY KEY (`CharacterId`, `VassalClanId`, `LiegeClanId`, `ControlledAppointmentId`),
    CONSTRAINT `FK_ECC_Appointments_ClanMemberships` FOREIGN KEY (`VassalClanId`, `CharacterId`) REFERENCES `ClanMemberships` (`ClanId`, `CharacterId`) ON DELETE RESTRICT,
    CONSTRAINT `FK_ECC_Appointments_ExternalClanControls` FOREIGN KEY (`VassalClanId`, `LiegeClanId`, `ControlledAppointmentId`) REFERENCES `ExternalClanControls` (`VassalClanId`, `LiegeClanId`, `ControlledAppointmentId`) ON DELETE RESTRICT
);

CREATE TABLE `Characters_Accents` (
    `CharacterId` bigint(20) NOT NULL,
    `AccentId` bigint(20) NOT NULL,
    `Familiarity` int(11) NOT NULL,
    `IsPreferred` bit(1) NOT NULL DEFAULT b'0',
    CONSTRAINT `PRIMARY` PRIMARY KEY (`CharacterId`, `AccentId`),
    CONSTRAINT `FK_Characters_Accents_Characters` FOREIGN KEY (`CharacterId`) REFERENCES `Characters` (`Id`) ON DELETE CASCADE
);

CREATE TABLE `Languages` (
    `Id` bigint(20) NOT NULL AUTO_INCREMENT,
    `DifficultyModel` bigint(20) NOT NULL,
    `LinkedTraitId` bigint(20) NOT NULL,
    `UnknownLanguageDescription` varchar(255) CHARACTER SET utf8 NOT NULL,
    `LanguageObfuscationFactor` double NOT NULL,
    `Name` varchar(255) CHARACTER SET utf8 NOT NULL,
    `DefaultLearnerAccentId` bigint(20) NULL,
    CONSTRAINT `PK_Languages` PRIMARY KEY (`Id`),
    CONSTRAINT `FK_Languages_LanguageDifficultyModels` FOREIGN KEY (`DifficultyModel`) REFERENCES `LanguageDifficultyModels` (`Id`) ON DELETE RESTRICT,
    CONSTRAINT `FK_Languages_TraitDefinitions` FOREIGN KEY (`LinkedTraitId`) REFERENCES `TraitDefinitions` (`Id`) ON DELETE RESTRICT
);

CREATE TABLE `Accents` (
    `Id` bigint(20) NOT NULL AUTO_INCREMENT,
    `LanguageId` bigint(20) NOT NULL,
    `Name` varchar(50) CHARACTER SET utf8 NOT NULL,
    `Suffix` varchar(4000) CHARACTER SET utf8 NOT NULL,
    `VagueSuffix` varchar(4000) CHARACTER SET utf8 NOT NULL,
    `Difficulty` int(11) NOT NULL,
    `Description` varchar(4000) CHARACTER SET utf8 NOT NULL,
    `Group` varchar(50) CHARACTER SET utf8 NULL,
    `ChargenAvailabilityProgId` bigint(20) NULL,
    CONSTRAINT `PK_Accents` PRIMARY KEY (`Id`),
    CONSTRAINT `FK_Accents_Languages` FOREIGN KEY (`LanguageId`) REFERENCES `Languages` (`Id`) ON DELETE RESTRICT
);

CREATE TABLE `Characters_Languages` (
    `CharacterId` bigint(20) NOT NULL,
    `LanguageId` bigint(20) NOT NULL,
    CONSTRAINT `PRIMARY` PRIMARY KEY (`CharacterId`, `LanguageId`),
    CONSTRAINT `FK_Characters_Languages_Characters` FOREIGN KEY (`CharacterId`) REFERENCES `Characters` (`Id`) ON DELETE CASCADE,
    CONSTRAINT `FK_Characters_Languages_Languages` FOREIGN KEY (`LanguageId`) REFERENCES `Languages` (`Id`) ON DELETE CASCADE
);

CREATE TABLE `MutualIntelligabilities` (
    `ListenerLanguageId` bigint(20) NOT NULL,
    `TargetLanguageId` bigint(20) NOT NULL,
    `IntelligabilityDifficulty` int(11) NOT NULL,
    CONSTRAINT `PRIMARY` PRIMARY KEY (`ListenerLanguageId`, `TargetLanguageId`),
    CONSTRAINT `FK_Languages_MutualIntelligabilities_Listener` FOREIGN KEY (`ListenerLanguageId`) REFERENCES `Languages` (`Id`) ON DELETE CASCADE,
    CONSTRAINT `FK_Languages_MutualIntelligabilities_Target` FOREIGN KEY (`TargetLanguageId`) REFERENCES `Languages` (`Id`) ON DELETE CASCADE
);

CREATE TABLE `Scripts_DesignedLanguages` (
    `ScriptId` bigint(20) NOT NULL,
    `LanguageId` bigint(20) NOT NULL,
    CONSTRAINT `PRIMARY` PRIMARY KEY (`ScriptId`, `LanguageId`),
    CONSTRAINT `FK_Scripts_DesignedLanguages_Languages` FOREIGN KEY (`LanguageId`) REFERENCES `Languages` (`Id`) ON DELETE CASCADE,
    CONSTRAINT `FK_Scripts_DesignedLanguages_Scripts` FOREIGN KEY (`ScriptId`) REFERENCES `Scripts` (`Id`) ON DELETE CASCADE
);

CREATE TABLE `Writings` (
    `Id` bigint(20) NOT NULL AUTO_INCREMENT,
    `WritingType` varchar(45) CHARACTER SET utf8 NOT NULL,
    `Style` int(11) NOT NULL,
    `LanguageId` bigint(20) NOT NULL,
    `ScriptId` bigint(20) NOT NULL,
    `AuthorId` bigint(20) NOT NULL,
    `TrueAuthorId` bigint(20) NULL,
    `HandwritingSkill` double NOT NULL,
    `LiteracySkill` double NOT NULL,
    `ForgerySkill` double NOT NULL,
    `LanguageSkill` double NOT NULL,
    `Definition` text CHARACTER SET utf8 NOT NULL,
    `WritingColour` bigint(20) NOT NULL,
    `ImplementType` int(11) NOT NULL,
    CONSTRAINT `PK_Writings` PRIMARY KEY (`Id`),
    CONSTRAINT `FK_Writings_Characters_Author` FOREIGN KEY (`AuthorId`) REFERENCES `Characters` (`Id`) ON DELETE RESTRICT,
    CONSTRAINT `FK_Writings_Languages` FOREIGN KEY (`LanguageId`) REFERENCES `Languages` (`Id`) ON DELETE RESTRICT,
    CONSTRAINT `FK_Writings_Scripts` FOREIGN KEY (`ScriptId`) REFERENCES `Scripts` (`Id`) ON DELETE RESTRICT,
    CONSTRAINT `FK_Writings_Characters_TrueAuthor` FOREIGN KEY (`TrueAuthorId`) REFERENCES `Characters` (`Id`) ON DELETE SET NULL
);

CREATE TABLE `CharacterLog` (
    `Id` bigint(20) NOT NULL AUTO_INCREMENT,
    `AccountId` bigint(20) NULL,
    `CharacterId` bigint(20) NOT NULL,
    `CellId` bigint(20) NOT NULL,
    `Command` text CHARACTER SET utf8mb4 NOT NULL,
    `Time` datetime NOT NULL,
    `IsPlayerCharacter` bit(1) NOT NULL,
    CONSTRAINT `PK_CharacterLog` PRIMARY KEY (`Id`),
    CONSTRAINT `FK_CharacterLog_Accounts` FOREIGN KEY (`AccountId`) REFERENCES `Accounts` (`Id`) ON DELETE CASCADE,
    CONSTRAINT `FK_CharacterLog_Characters` FOREIGN KEY (`CharacterId`) REFERENCES `Characters` (`Id`) ON DELETE CASCADE
);

CREATE TABLE `ActiveProjectLabours` (
    `ActiveProjectId` bigint(20) NOT NULL,
    `ProjectLabourRequirementsId` bigint(20) NOT NULL,
    `Progress` double NOT NULL,
    CONSTRAINT `PRIMARY` PRIMARY KEY (`ActiveProjectId`, `ProjectLabourRequirementsId`),
    CONSTRAINT `FK_ActiveProjectLabours_ProjectLabourRequirements` FOREIGN KEY (`ProjectLabourRequirementsId`) REFERENCES `ProjectLabourRequirements` (`Id`) ON DELETE CASCADE
);

CREATE TABLE `ActiveProjectMaterials` (
    `ActiveProjectId` bigint(20) NOT NULL,
    `ProjectMaterialRequirementsId` bigint(20) NOT NULL,
    `Progress` double NOT NULL,
    CONSTRAINT `PRIMARY` PRIMARY KEY (`ActiveProjectId`, `ProjectMaterialRequirementsId`),
    CONSTRAINT `FK_ActiveProjectMaterials_ProjectMaterialRequirements` FOREIGN KEY (`ProjectMaterialRequirementsId`) REFERENCES `ProjectMaterialRequirements` (`Id`) ON DELETE CASCADE
);

CREATE TABLE `Areas_Rooms` (
    `AreaId` bigint(20) NOT NULL,
    `RoomId` bigint(20) NOT NULL,
    CONSTRAINT `PRIMARY` PRIMARY KEY (`AreaId`, `RoomId`),
    CONSTRAINT `FK_Areas_Rooms_Areas` FOREIGN KEY (`AreaId`) REFERENCES `Areas` (`Id`) ON DELETE CASCADE
);

CREATE TABLE `BodypartProto` (
    `Id` bigint(20) NOT NULL AUTO_INCREMENT,
    `BodypartType` int(11) NOT NULL,
    `BodyId` bigint(20) NOT NULL,
    `Name` varchar(4000) CHARACTER SET utf8 NULL,
    `Description` varchar(4000) CHARACTER SET utf8 NULL,
    `CountAsId` bigint(20) NULL,
    `BodypartShapeId` bigint(20) NOT NULL,
    `DisplayOrder` int(11) NULL,
    `MaxLife` int(11) NOT NULL DEFAULT '100',
    `SeveredThreshold` int(11) NOT NULL DEFAULT '100',
    `PainModifier` double NOT NULL DEFAULT '1',
    `BleedModifier` double NOT NULL DEFAULT '0.1',
    `RelativeHitChance` int(11) NOT NULL DEFAULT '100',
    `Location` int(11) NOT NULL,
    `Alignment` int(11) NOT NULL,
    `Unary` bit(1) NULL,
    `MaxSingleSize` int(11) NULL,
    `IsOrgan` int(11) NOT NULL,
    `WeightLimit` double NOT NULL,
    `IsCore` bit(1) NOT NULL DEFAULT b'1',
    `StunModifier` double NOT NULL DEFAULT '1',
    `DamageModifier` double NOT NULL DEFAULT '1',
    `DefaultMaterialId` bigint(20) NOT NULL DEFAULT '1',
    `Significant` bit(1) NOT NULL,
    `RelativeInfectability` double NOT NULL DEFAULT '1',
    `HypoxiaDamagePerTick` double NOT NULL DEFAULT '0.2',
    `IsVital` bit(1) NOT NULL DEFAULT b'0',
    `ArmourTypeId` bigint(20) NULL,
    `ImplantSpace` double NOT NULL,
    `ImplantSpaceOccupied` double NOT NULL,
    `Size` int(11) NOT NULL DEFAULT '5',
    CONSTRAINT `PK_BodypartProto` PRIMARY KEY (`Id`),
    CONSTRAINT `FK_BodypartProto_ArmourTypes` FOREIGN KEY (`ArmourTypeId`) REFERENCES `ArmourTypes` (`Id`) ON DELETE SET NULL,
    CONSTRAINT `FK_BodypartProto_BodypartShape` FOREIGN KEY (`BodypartShapeId`) REFERENCES `BodypartShape` (`Id`) ON DELETE RESTRICT,
    CONSTRAINT `FK_BodypartProto_BodypartProto` FOREIGN KEY (`CountAsId`) REFERENCES `BodypartProto` (`Id`) ON DELETE SET NULL,
    CONSTRAINT `FK_BodypartProto_Materials` FOREIGN KEY (`DefaultMaterialId`) REFERENCES `Materials` (`Id`) ON DELETE RESTRICT
);

CREATE TABLE `BodypartGroupDescribers_BodypartProtos` (
    `BodypartGroupDescriberId` bigint(20) NOT NULL,
    `BodypartProtoId` bigint(20) NOT NULL,
    `Mandatory` bit(1) NOT NULL DEFAULT b'0',
    CONSTRAINT `PRIMARY` PRIMARY KEY (`BodypartGroupDescriberId`, `BodypartProtoId`),
    CONSTRAINT `FK_BGD_BodypartProtos_BodypartGroupDescribers` FOREIGN KEY (`BodypartGroupDescriberId`) REFERENCES `BodypartGroupDescribers` (`Id`) ON DELETE CASCADE,
    CONSTRAINT `FK_BGD_BodypartProtos_BodypartProto` FOREIGN KEY (`BodypartProtoId`) REFERENCES `BodypartProto` (`Id`) ON DELETE CASCADE
);

CREATE TABLE `BodypartInternalInfos` (
    `BodypartProtoId` bigint(20) NOT NULL,
    `InternalPartId` bigint(20) NOT NULL,
    `IsPrimaryOrganLocation` bit(1) NOT NULL DEFAULT b'0',
    `HitChance` double NOT NULL DEFAULT '5',
    `ProximityGroup` varchar(45) CHARACTER SET utf8 NULL,
    CONSTRAINT `PRIMARY` PRIMARY KEY (`BodypartProtoId`, `InternalPartId`),
    CONSTRAINT `FK_BodypartInternalInfos_BodypartProtos` FOREIGN KEY (`BodypartProtoId`) REFERENCES `BodypartProto` (`Id`) ON DELETE CASCADE,
    CONSTRAINT `FK_BodypartInternalInfos_BodypartProtos_Internal` FOREIGN KEY (`InternalPartId`) REFERENCES `BodypartProto` (`Id`) ON DELETE CASCADE
);

CREATE TABLE `BodypartProto_AlignmentHits` (
    `Id` bigint(20) NOT NULL AUTO_INCREMENT,
    `BodypartProtoId` bigint(20) NOT NULL,
    `Alignment` int(11) NOT NULL,
    `HitChance` int(11) NULL,
    CONSTRAINT `PK_BodypartProto_AlignmentHits` PRIMARY KEY (`Id`),
    CONSTRAINT `FK_BodypartProto_AlignmentHits_BodypartProto` FOREIGN KEY (`BodypartProtoId`) REFERENCES `BodypartProto` (`Id`) ON DELETE RESTRICT
);

CREATE TABLE `BodypartProto_BodypartProto_Upstream` (
    `Child` bigint(20) NOT NULL,
    `Parent` bigint(20) NOT NULL,
    CONSTRAINT `PRIMARY` PRIMARY KEY (`Child`, `Parent`),
    CONSTRAINT `FKChild` FOREIGN KEY (`Child`) REFERENCES `BodypartProto` (`Id`) ON DELETE RESTRICT,
    CONSTRAINT `FKParent` FOREIGN KEY (`Parent`) REFERENCES `BodypartProto` (`Id`) ON DELETE RESTRICT
);

CREATE TABLE `BodypartProto_OrientationHits` (
    `Id` bigint(20) NOT NULL AUTO_INCREMENT,
    `BodypartProtoId` bigint(20) NOT NULL,
    `Orientation` int(11) NOT NULL,
    `HitChance` int(11) NULL,
    CONSTRAINT `PK_BodypartProto_OrientationHits` PRIMARY KEY (`Id`),
    CONSTRAINT `FK_BodypartProto_OrientationHits_BodypartProto` FOREIGN KEY (`BodypartProtoId`) REFERENCES `BodypartProto` (`Id`) ON DELETE RESTRICT
);

CREATE TABLE `BodyProtos` (
    `Id` bigint(20) NOT NULL AUTO_INCREMENT,
    `Name` varchar(4000) CHARACTER SET utf8 NULL,
    `CountsAsId` bigint(20) NULL,
    `WearSizeParameterId` bigint(20) NOT NULL,
    `WielderDescriptionPlural` varchar(4000) CHARACTER SET utf8 NOT NULL DEFAULT 'hands',
    `WielderDescriptionSingle` varchar(4000) CHARACTER SET utf8 NOT NULL DEFAULT 'hand',
    `ConsiderString` varchar(4000) CHARACTER SET utf8 NOT NULL,
    `StaminaRecoveryProgId` bigint(20) NULL,
    `MinimumLegsToStand` int(11) NOT NULL DEFAULT '2',
    `MinimumWingsToFly` int(11) NOT NULL DEFAULT '2',
    `LegDescriptionSingular` varchar(1000) CHARACTER SET utf8 NOT NULL DEFAULT 'leg',
    `LegDescriptionPlural` varchar(1000) CHARACTER SET utf8 NOT NULL DEFAULT 'legs',
    `DefaultSmashingBodypartId` bigint(20) NULL,
    CONSTRAINT `PK_BodyProtos` PRIMARY KEY (`Id`),
    CONSTRAINT `FK_BodyPrototype_BodyPrototype` FOREIGN KEY (`CountsAsId`) REFERENCES `BodyProtos` (`Id`) ON DELETE SET NULL,
    CONSTRAINT `FK_BodyPrototype_Bodyparts` FOREIGN KEY (`DefaultSmashingBodypartId`) REFERENCES `BodypartProto` (`Id`) ON DELETE SET NULL,
    CONSTRAINT `FK_BodyPrototype_WearableSizeParameterRule` FOREIGN KEY (`WearSizeParameterId`) REFERENCES `WearableSizeParameterRule` (`Id`) ON DELETE RESTRICT
);

CREATE TABLE `BoneOrganCoverages` (
    `BoneId` bigint(20) NOT NULL,
    `OrganId` bigint(20) NOT NULL,
    `CoverageChance` double NOT NULL,
    CONSTRAINT `PRIMARY` PRIMARY KEY (`BoneId`, `OrganId`),
    CONSTRAINT `FK_BoneOrganCoverages_BodypartProto_Bone` FOREIGN KEY (`BoneId`) REFERENCES `BodypartProto` (`Id`) ON DELETE CASCADE,
    CONSTRAINT `FK_BoneOrganCoverages_BodypartProto_Organ` FOREIGN KEY (`OrganId`) REFERENCES `BodypartProto` (`Id`) ON DELETE CASCADE
);

CREATE TABLE `BodypartGroupDescribers_BodyProtos` (
    `BodypartGroupDescriberId` bigint(20) NOT NULL,
    `BodyProtoId` bigint(20) NOT NULL,
    CONSTRAINT `PRIMARY` PRIMARY KEY (`BodypartGroupDescriberId`, `BodyProtoId`),
    CONSTRAINT `FK_BGD_BodyProtos_BodyProtos` FOREIGN KEY (`BodyProtoId`) REFERENCES `BodyProtos` (`Id`) ON DELETE RESTRICT,
    CONSTRAINT `FK_BGD_BodyProtos_BodypartGroupDescribers` FOREIGN KEY (`BodypartGroupDescriberId`) REFERENCES `BodypartGroupDescribers` (`Id`) ON DELETE RESTRICT
);

CREATE TABLE `BodyProtos_AdditionalBodyparts` (
    `BodyProtoId` bigint(20) NOT NULL,
    `BodypartId` bigint(20) NOT NULL,
    `Usage` varchar(50) CHARACTER SET utf8 NOT NULL,
    CONSTRAINT `PRIMARY` PRIMARY KEY (`BodyProtoId`, `BodypartId`, `Usage`),
    CONSTRAINT `FK_BodyProtos_AdditionalBodyparts_BodyProtos` FOREIGN KEY (`BodyProtoId`) REFERENCES `BodyProtos` (`Id`) ON DELETE RESTRICT,
    CONSTRAINT `FK_BodyProtos_AdditionalBodyparts_BodypartProto` FOREIGN KEY (`BodypartId`) REFERENCES `BodypartProto` (`Id`) ON DELETE RESTRICT
);

CREATE TABLE `ButcheryProducts` (
    `Id` bigint(20) NOT NULL AUTO_INCREMENT,
    `Name` varchar(100) CHARACTER SET utf8 NOT NULL,
    `TargetBodyId` bigint(20) NOT NULL,
    `IsPelt` bit(1) NOT NULL,
    `Subcategory` varchar(100) CHARACTER SET utf8 NOT NULL,
    `CanProduceProgId` bigint(20) NULL,
    CONSTRAINT `PK_ButcheryProducts` PRIMARY KEY (`Id`),
    CONSTRAINT `FK_ButcheryProducts_FutureProgs` FOREIGN KEY (`CanProduceProgId`) REFERENCES `FutureProgs` (`Id`) ON DELETE SET NULL,
    CONSTRAINT `FK_ButcheryProducts_BodyProtos` FOREIGN KEY (`TargetBodyId`) REFERENCES `BodyProtos` (`Id`) ON DELETE CASCADE
);

CREATE TABLE `Limbs` (
    `Id` bigint(20) NOT NULL AUTO_INCREMENT,
    `Name` text CHARACTER SET utf8mb4 NULL,
    `RootBodypartId` bigint(20) NOT NULL,
    `LimbType` int(11) NOT NULL,
    `RootBodyId` bigint(20) NOT NULL,
    `LimbDamageThresholdMultiplier` double NOT NULL,
    `LimbPainThresholdMultiplier` double NOT NULL,
    CONSTRAINT `PK_Limbs` PRIMARY KEY (`Id`),
    CONSTRAINT `FK_Limbs_BodyProtos` FOREIGN KEY (`RootBodyId`) REFERENCES `BodyProtos` (`Id`) ON DELETE CASCADE,
    CONSTRAINT `FK_Limbs_BodypartProto` FOREIGN KEY (`RootBodypartId`) REFERENCES `BodypartProto` (`Id`) ON DELETE RESTRICT
);

CREATE TABLE `MoveSpeeds` (
    `Id` bigint(20) NOT NULL AUTO_INCREMENT,
    `BodyProtoId` bigint(20) NOT NULL,
    `Multiplier` double NOT NULL,
    `Alias` varchar(255) CHARACTER SET utf8 NOT NULL,
    `FirstPersonVerb` varchar(255) CHARACTER SET utf8 NOT NULL,
    `ThirdPersonVerb` varchar(255) CHARACTER SET utf8 NOT NULL,
    `PresentParticiple` varchar(255) CHARACTER SET utf8 NOT NULL,
    `PositionId` bigint(20) NOT NULL,
    `StaminaMultiplier` double NOT NULL DEFAULT '1',
    CONSTRAINT `PK_MoveSpeeds` PRIMARY KEY (`Id`),
    CONSTRAINT `FK_MoveSpeeds_BodyPrototype` FOREIGN KEY (`BodyProtoId`) REFERENCES `BodyProtos` (`Id`) ON DELETE RESTRICT
);

CREATE TABLE `Races` (
    `Id` bigint(20) NOT NULL AUTO_INCREMENT,
    `Name` varchar(255) CHARACTER SET utf8 NOT NULL,
    `Description` varchar(4000) CHARACTER SET utf8 NOT NULL,
    `BaseBodyId` bigint(20) NOT NULL,
    `AllowedGenders` varchar(255) CHARACTER SET utf8 NOT NULL,
    `ParentRaceId` bigint(20) NULL,
    `AttributeBonusProgId` bigint(20) NOT NULL,
    `AttributeTotalCap` int(11) NOT NULL,
    `IndividualAttributeCap` int(11) NOT NULL,
    `DiceExpression` varchar(255) CHARACTER SET utf8 NOT NULL,
    `IlluminationPerceptionMultiplier` double NOT NULL DEFAULT '1',
    `AvailabilityProgId` bigint(20) NULL,
    `CorpseModelId` bigint(20) NOT NULL,
    `DefaultHealthStrategyId` bigint(20) NOT NULL,
    `CanUseWeapons` bit(1) NOT NULL DEFAULT b'1',
    `CanAttack` bit(1) NOT NULL DEFAULT b'1',
    `CanDefend` bit(1) NOT NULL DEFAULT b'1',
    `NaturalArmourTypeId` bigint(20) NULL,
    `NaturalArmourQuality` bigint(20) NOT NULL,
    `NaturalArmourMaterialId` bigint(20) NULL,
    `BloodLiquidId` bigint(20) NULL,
    `NeedsToBreathe` bit(1) NOT NULL DEFAULT b'1',
    `SweatLiquidId` bigint(20) NULL,
    `SweatRateInLitresPerMinute` double NOT NULL DEFAULT '0.8',
    `SizeStanding` int(11) NOT NULL DEFAULT '6',
    `SizeProne` int(11) NOT NULL DEFAULT '5',
    `SizeSitting` int(11) NOT NULL DEFAULT '6',
    `CommunicationStrategyType` varchar(45) CHARACTER SET utf8 NOT NULL DEFAULT 'humanoid',
    `DefaultHandedness` int(11) NOT NULL DEFAULT '3',
    `HandednessOptions` varchar(100) CHARACTER SET utf8 NOT NULL DEFAULT '1 3',
    `MaximumDragWeightExpression` text CHARACTER SET utf8 NOT NULL,
    `MaximumLiftWeightExpression` text CHARACTER SET utf8 NOT NULL,
    `RaceButcheryProfileId` bigint(20) NULL,
    `BloodModelId` bigint(20) NULL,
    `RaceUsesStamina` bit(1) NOT NULL DEFAULT b'1',
    `CanEatCorpses` bit(1) NOT NULL DEFAULT b'0',
    `BiteWeight` double NOT NULL DEFAULT '1000',
    `EatCorpseEmoteText` varchar(500) CHARACTER SET utf8 NOT NULL DEFAULT '@ eat|eats {0}$1',
    `CanEatMaterialsOptIn` bit(1) NOT NULL DEFAULT b'0',
    `TemperatureRangeFloor` double NOT NULL,
    `TemperatureRangeCeiling` double NOT NULL DEFAULT '40',
    `BodypartSizeModifier` int(11) NOT NULL,
    `BodypartHealthMultiplier` double NOT NULL DEFAULT '1',
    `BreathingVolumeExpression` varchar(500) CHARACTER SET utf8 NOT NULL DEFAULT '7',
    `HoldBreathLengthExpression` varchar(500) CHARACTER SET utf8 NOT NULL DEFAULT '120',
    `CanClimb` bit(1) NOT NULL DEFAULT b'0',
    `CanSwim` bit(1) NOT NULL DEFAULT b'1',
    `MinimumSleepingPosition` int(11) NOT NULL DEFAULT '4',
    `ChildAge` int(11) NOT NULL DEFAULT '3',
    `YouthAge` int(11) NOT NULL DEFAULT '10',
    `YoungAdultAge` int(11) NOT NULL DEFAULT '16',
    `AdultAge` int(11) NOT NULL DEFAULT '21',
    `ElderAge` int(11) NOT NULL DEFAULT '55',
    `VenerableAge` int(11) NOT NULL DEFAULT '75',
    CONSTRAINT `PK_Races` PRIMARY KEY (`Id`),
    CONSTRAINT `FK_Races_AttributeBonusProg` FOREIGN KEY (`AttributeBonusProgId`) REFERENCES `FutureProgs` (`Id`) ON DELETE RESTRICT,
    CONSTRAINT `FK_Races_AvailabilityProg` FOREIGN KEY (`AvailabilityProgId`) REFERENCES `FutureProgs` (`Id`) ON DELETE SET NULL,
    CONSTRAINT `FK_Races_BodyProtos` FOREIGN KEY (`BaseBodyId`) REFERENCES `BodyProtos` (`Id`) ON DELETE RESTRICT,
    CONSTRAINT `FK_Races_Liquids_Blood` FOREIGN KEY (`BloodLiquidId`) REFERENCES `Liquids` (`Id`) ON DELETE RESTRICT,
    CONSTRAINT `FK_Races_BloodModels` FOREIGN KEY (`BloodModelId`) REFERENCES `BloodModels` (`Id`) ON DELETE SET NULL,
    CONSTRAINT `FK_Races_CorpseModels` FOREIGN KEY (`CorpseModelId`) REFERENCES `CorpseModels` (`Id`) ON DELETE RESTRICT,
    CONSTRAINT `FK_Races_HealthStrategies` FOREIGN KEY (`DefaultHealthStrategyId`) REFERENCES `HealthStrategies` (`Id`) ON DELETE RESTRICT,
    CONSTRAINT `FK_Races_Materials` FOREIGN KEY (`NaturalArmourMaterialId`) REFERENCES `Materials` (`Id`) ON DELETE SET NULL,
    CONSTRAINT `FK_Races_ArmourTypes` FOREIGN KEY (`NaturalArmourTypeId`) REFERENCES `ArmourTypes` (`Id`) ON DELETE SET NULL,
    CONSTRAINT `FK_Races_Races` FOREIGN KEY (`ParentRaceId`) REFERENCES `Races` (`Id`) ON DELETE RESTRICT,
    CONSTRAINT `FK_Races_RaceButcheryProfiles` FOREIGN KEY (`RaceButcheryProfileId`) REFERENCES `RaceButcheryProfiles` (`Id`) ON DELETE SET NULL,
    CONSTRAINT `FK_Races_Liqiuds_Sweat` FOREIGN KEY (`SweatLiquidId`) REFERENCES `Liquids` (`Id`) ON DELETE SET NULL
);

CREATE TABLE `ButcheryProductItems` (
    `Id` bigint(20) NOT NULL AUTO_INCREMENT,
    `ButcheryProductId` bigint(20) NOT NULL,
    `NormalProtoId` bigint(20) NOT NULL,
    `DamagedProtoId` bigint(20) NULL,
    `NormalQuantity` int(11) NOT NULL,
    `DamagedQuantity` int(11) NOT NULL,
    `ButcheryProductItemscol` varchar(45) CHARACTER SET utf8 NULL,
    `DamageThreshold` double NOT NULL DEFAULT '10',
    CONSTRAINT `PK_ButcheryProductItems` PRIMARY KEY (`Id`),
    CONSTRAINT `FK_ButcheryProductItems_ButcheryProducts` FOREIGN KEY (`ButcheryProductId`) REFERENCES `ButcheryProducts` (`Id`) ON DELETE CASCADE
);

CREATE TABLE `ButcheryProducts_BodypartProtos` (
    `ButcheryProductId` bigint(20) NOT NULL,
    `BodypartProtoId` bigint(20) NOT NULL,
    CONSTRAINT `PRIMARY` PRIMARY KEY (`ButcheryProductId`, `BodypartProtoId`),
    CONSTRAINT `FK_ButcheryProducts_BodypartProtos_BodypartProtos` FOREIGN KEY (`BodypartProtoId`) REFERENCES `BodypartProto` (`Id`) ON DELETE CASCADE,
    CONSTRAINT `FK_ButcheryProducts_BodypartProtos_ButcheryProducts` FOREIGN KEY (`ButcheryProductId`) REFERENCES `ButcheryProducts` (`Id`) ON DELETE CASCADE
);

CREATE TABLE `RaceButcheryProfiles_ButcheryProducts` (
    `RaceButcheryProfileId` bigint(20) NOT NULL,
    `ButcheryProductId` bigint(20) NOT NULL,
    CONSTRAINT `PRIMARY` PRIMARY KEY (`RaceButcheryProfileId`, `ButcheryProductId`),
    CONSTRAINT `FK_RaceButcheryProfiles_ButcheryProducts_ButcheryProducts` FOREIGN KEY (`ButcheryProductId`) REFERENCES `ButcheryProducts` (`Id`) ON DELETE CASCADE,
    CONSTRAINT `FK_RaceButcheryProfiles_ButcheryProducts_RaceButcheryProfiles` FOREIGN KEY (`RaceButcheryProfileId`) REFERENCES `RaceButcheryProfiles` (`Id`) ON DELETE CASCADE
);

CREATE TABLE `Limbs_BodypartProto` (
    `BodypartProtoId` bigint(20) NOT NULL,
    `LimbId` bigint(20) NOT NULL,
    CONSTRAINT `PRIMARY` PRIMARY KEY (`BodypartProtoId`, `LimbId`),
    CONSTRAINT `FK_Limbs_BodypartProto_BodypartProto` FOREIGN KEY (`BodypartProtoId`) REFERENCES `BodypartProto` (`Id`) ON DELETE CASCADE,
    CONSTRAINT `FK_Limbs_BodypartProto_Limbs` FOREIGN KEY (`LimbId`) REFERENCES `Limbs` (`Id`) ON DELETE CASCADE
);

CREATE TABLE `Limbs_SpinalParts` (
    `LimbId` bigint(20) NOT NULL,
    `BodypartProtoId` bigint(20) NOT NULL,
    CONSTRAINT `PRIMARY` PRIMARY KEY (`LimbId`, `BodypartProtoId`),
    CONSTRAINT `FK_Limbs_SpinalParts_BodypartProtos` FOREIGN KEY (`BodypartProtoId`) REFERENCES `BodypartProto` (`Id`) ON DELETE CASCADE,
    CONSTRAINT `FK_Limbs_SpinalParts_Limbs` FOREIGN KEY (`LimbId`) REFERENCES `Limbs` (`Id`) ON DELETE CASCADE
);

CREATE TABLE `ChargenAdvices_Races` (
    `ChargenAdviceId` bigint(20) NOT NULL,
    `RaceId` bigint(20) NOT NULL,
    CONSTRAINT `PRIMARY` PRIMARY KEY (`ChargenAdviceId`, `RaceId`),
    CONSTRAINT `FK_ChargenAdvices_Races_ChargenAdvices` FOREIGN KEY (`ChargenAdviceId`) REFERENCES `ChargenAdvices` (`Id`) ON DELETE CASCADE,
    CONSTRAINT `FK_ChargenAdvices_Races_Races` FOREIGN KEY (`RaceId`) REFERENCES `Races` (`Id`) ON DELETE CASCADE
);

CREATE TABLE `Ethnicities` (
    `Id` bigint(20) NOT NULL AUTO_INCREMENT,
    `Name` varchar(4000) CHARACTER SET utf8 NOT NULL,
    `ChargenBlurb` varchar(4000) CHARACTER SET utf8 NOT NULL,
    `AvailabilityProgId` bigint(20) NULL,
    `ParentRaceId` bigint(20) NULL,
    `EthnicGroup` text CHARACTER SET utf8 NULL,
    `EthnicSubgroup` text CHARACTER SET utf8 NULL,
    `PopulationBloodModelId` bigint(20) NULL,
    `TolerableTemperatureFloorEffect` double NOT NULL,
    `TolerableTemperatureCeilingEffect` double NOT NULL,
    CONSTRAINT `PK_Ethnicities` PRIMARY KEY (`Id`),
    CONSTRAINT `FK_Ethnicities_AvailabilityProg` FOREIGN KEY (`AvailabilityProgId`) REFERENCES `FutureProgs` (`Id`) ON DELETE SET NULL,
    CONSTRAINT `FK_Ethnicities_Races` FOREIGN KEY (`ParentRaceId`) REFERENCES `Races` (`Id`) ON DELETE CASCADE,
    CONSTRAINT `FK_Ethnicities_PopulationBloodModels` FOREIGN KEY (`PopulationBloodModelId`) REFERENCES `PopulationBloodModels` (`Id`) ON DELETE SET NULL
);

CREATE TABLE `RaceEdibleForagableYields` (
    `RaceId` bigint(20) NOT NULL,
    `YieldType` varchar(50) CHARACTER SET utf8 NOT NULL,
    `BiteYield` double NOT NULL,
    `CaloriesPerYield` double NOT NULL,
    `HungerPerYield` double NOT NULL,
    `WaterPerYield` double NOT NULL,
    `ThirstPerYield` double NOT NULL,
    `AlcoholPerYield` double NOT NULL,
    `EatEmote` varchar(1000) CHARACTER SET utf8 NOT NULL DEFAULT '@ eat|eats {0} from the location.',
    CONSTRAINT `PRIMARY` PRIMARY KEY (`RaceId`, `YieldType`),
    CONSTRAINT `FK_RaceEdibleForagableYields_Races` FOREIGN KEY (`RaceId`) REFERENCES `Races` (`Id`) ON DELETE CASCADE
);

CREATE TABLE `Races_AdditionalBodyparts` (
    `Usage` varchar(50) CHARACTER SET utf8 NOT NULL,
    `BodypartId` bigint(20) NOT NULL,
    `RaceId` bigint(20) NOT NULL,
    CONSTRAINT `PRIMARY` PRIMARY KEY (`Usage`, `RaceId`, `BodypartId`),
    CONSTRAINT `FK_Races_AdditionalBodyparts_BodypartProto` FOREIGN KEY (`BodypartId`) REFERENCES `BodypartProto` (`Id`) ON DELETE RESTRICT,
    CONSTRAINT `FK_Races_AdditionalBodyparts_Races` FOREIGN KEY (`RaceId`) REFERENCES `Races` (`Id`) ON DELETE CASCADE
);

CREATE TABLE `Races_AdditionalCharacteristics` (
    `RaceId` bigint(20) NOT NULL,
    `CharacteristicDefinitionId` bigint(20) NOT NULL,
    `Usage` varchar(255) CHARACTER SET utf8 NOT NULL,
    CONSTRAINT `PRIMARY` PRIMARY KEY (`RaceId`, `CharacteristicDefinitionId`),
    CONSTRAINT `FK_RAC_CharacteristicDefinitions` FOREIGN KEY (`CharacteristicDefinitionId`) REFERENCES `CharacteristicDefinitions` (`Id`) ON DELETE RESTRICT,
    CONSTRAINT `FK_RAC_Races` FOREIGN KEY (`RaceId`) REFERENCES `Races` (`Id`) ON DELETE CASCADE
);

CREATE TABLE `Races_Attributes` (
    `RaceId` bigint(20) NOT NULL,
    `AttributeId` bigint(20) NOT NULL,
    `IsHealthAttribute` bit(1) NOT NULL DEFAULT b'0',
    CONSTRAINT `PRIMARY` PRIMARY KEY (`RaceId`, `AttributeId`),
    CONSTRAINT `FK_Races_Attributes_TraitDefinitions` FOREIGN KEY (`AttributeId`) REFERENCES `TraitDefinitions` (`Id`) ON DELETE CASCADE,
    CONSTRAINT `FK_Races_Attributes_Races` FOREIGN KEY (`RaceId`) REFERENCES `Races` (`Id`) ON DELETE CASCADE
);

CREATE TABLE `Races_BreathableGases` (
    `RaceId` bigint(20) NOT NULL,
    `GasId` bigint(20) NOT NULL,
    `Multiplier` double NOT NULL DEFAULT '1',
    CONSTRAINT `PRIMARY` PRIMARY KEY (`RaceId`, `GasId`),
    CONSTRAINT `FK_Races_BreathableGases_Gases` FOREIGN KEY (`GasId`) REFERENCES `Gases` (`Id`) ON DELETE CASCADE,
    CONSTRAINT `FK_Races_BreathableGases_Races` FOREIGN KEY (`RaceId`) REFERENCES `Races` (`Id`) ON DELETE CASCADE
);

CREATE TABLE `Races_BreathableLiquids` (
    `RaceId` bigint(20) NOT NULL,
    `LiquidId` bigint(20) NOT NULL,
    `Multiplier` double NOT NULL DEFAULT '1',
    CONSTRAINT `PRIMARY` PRIMARY KEY (`RaceId`, `LiquidId`),
    CONSTRAINT `FK_Races_BreathableLiquids_Liquids` FOREIGN KEY (`LiquidId`) REFERENCES `Liquids` (`Id`) ON DELETE CASCADE,
    CONSTRAINT `FK_Races_BreathableLiquids_Races` FOREIGN KEY (`RaceId`) REFERENCES `Races` (`Id`) ON DELETE CASCADE
);

CREATE TABLE `Races_ChargenResources` (
    `RaceId` bigint(20) NOT NULL,
    `ChargenResourceId` bigint(20) NOT NULL,
    `RequirementOnly` bit(1) NOT NULL DEFAULT b'0',
    `Amount` int(11) NOT NULL,
    CONSTRAINT `PRIMARY` PRIMARY KEY (`RaceId`, `ChargenResourceId`, `RequirementOnly`),
    CONSTRAINT `FK_Races_ChargenResources_ChargenResources` FOREIGN KEY (`ChargenResourceId`) REFERENCES `ChargenResources` (`Id`) ON DELETE CASCADE,
    CONSTRAINT `FK_Races_ChargenResources_Races` FOREIGN KEY (`RaceId`) REFERENCES `Races` (`Id`) ON DELETE CASCADE
);

CREATE TABLE `Races_EdibleMaterials` (
    `RaceId` bigint(20) NOT NULL,
    `MaterialId` bigint(20) NOT NULL,
    `CaloriesPerKilogram` double NOT NULL,
    `HungerPerKilogram` double NOT NULL,
    `ThirstPerKilogram` double NOT NULL,
    `WaterPerKilogram` double NOT NULL,
    `AlcoholPerKilogram` double NOT NULL,
    CONSTRAINT `PRIMARY` PRIMARY KEY (`RaceId`, `MaterialId`),
    CONSTRAINT `FK_Races_EdibleMaterials_Materials` FOREIGN KEY (`MaterialId`) REFERENCES `Materials` (`Id`) ON DELETE CASCADE,
    CONSTRAINT `FK_Races_EdibleMaterials_Races` FOREIGN KEY (`RaceId`) REFERENCES `Races` (`Id`) ON DELETE CASCADE
);

CREATE TABLE `Races_WeaponAttacks` (
    `RaceId` bigint(20) NOT NULL,
    `WeaponAttackId` bigint(20) NOT NULL,
    `BodypartId` bigint(20) NOT NULL,
    `Quality` int(11) NOT NULL,
    CONSTRAINT `PRIMARY` PRIMARY KEY (`RaceId`, `WeaponAttackId`, `BodypartId`),
    CONSTRAINT `FK_Races_WeaponAttacks_BodypartProto` FOREIGN KEY (`BodypartId`) REFERENCES `BodypartProto` (`Id`) ON DELETE CASCADE,
    CONSTRAINT `FK_Races_WeaponAttacks_Races` FOREIGN KEY (`RaceId`) REFERENCES `Races` (`Id`) ON DELETE CASCADE,
    CONSTRAINT `FK_Races_WeaponAttacks_WeaponAttacks` FOREIGN KEY (`WeaponAttackId`) REFERENCES `WeaponAttacks` (`Id`) ON DELETE CASCADE
);

CREATE TABLE `Bodies` (
    `Id` bigint(20) NOT NULL AUTO_INCREMENT,
    `BodyPrototypeID` bigint(20) NOT NULL,
    `Height` double NOT NULL,
    `Weight` double NOT NULL,
    `Position` bigint(20) NOT NULL,
    `CurrentSpeed` bigint(20) NULL,
    `RaceId` bigint(20) NOT NULL,
    `CurrentStamina` double NOT NULL,
    `CurrentBloodVolume` double NOT NULL DEFAULT '-1',
    `EthnicityId` bigint(20) NOT NULL,
    `BloodtypeId` bigint(20) NULL,
    `Gender` smallint(6) NOT NULL,
    `ShortDescription` varchar(1000) CHARACTER SET utf8 NULL,
    `FullDescription` varchar(4000) CHARACTER SET utf8 NULL,
    `ShortDescriptionPatternId` bigint(20) NULL,
    `FullDescriptionPatternId` bigint(20) NULL,
    `Tattoos` mediumtext CHARACTER SET utf8 NULL,
    `HeldBreathLength` int(11) NOT NULL,
    `EffectData` mediumtext CHARACTER SET utf8 NOT NULL,
    `Scars` mediumtext CHARACTER SET utf8 NULL,
    CONSTRAINT `PK_Bodies` PRIMARY KEY (`Id`),
    CONSTRAINT `FK_Bodies_Bloodtypes` FOREIGN KEY (`BloodtypeId`) REFERENCES `Bloodtypes` (`Id`) ON DELETE SET NULL,
    CONSTRAINT `FK_Bodies_Ethnicities` FOREIGN KEY (`EthnicityId`) REFERENCES `Ethnicities` (`Id`) ON DELETE RESTRICT,
    CONSTRAINT `FK_Bodies_EntityDescriptionPatterns_Full` FOREIGN KEY (`FullDescriptionPatternId`) REFERENCES `EntityDescriptionPatterns` (`Id`) ON DELETE RESTRICT,
    CONSTRAINT `FK_Bodies_Races` FOREIGN KEY (`RaceId`) REFERENCES `Races` (`Id`) ON DELETE RESTRICT,
    CONSTRAINT `FK_Bodies_EntityDescriptionPatterns_Short` FOREIGN KEY (`ShortDescriptionPatternId`) REFERENCES `EntityDescriptionPatterns` (`Id`) ON DELETE RESTRICT
);

CREATE TABLE `ChargenAdvices_Ethnicities` (
    `ChargenAdviceId` bigint(20) NOT NULL,
    `EthnicityId` bigint(20) NOT NULL,
    CONSTRAINT `PRIMARY` PRIMARY KEY (`ChargenAdviceId`, `EthnicityId`),
    CONSTRAINT `FK_ChargenAdvices_Ethnicities_ChargenAdvices` FOREIGN KEY (`ChargenAdviceId`) REFERENCES `ChargenAdvices` (`Id`) ON DELETE CASCADE,
    CONSTRAINT `FK_ChargenAdvices_Ethnicities_Ethnicities` FOREIGN KEY (`EthnicityId`) REFERENCES `Ethnicities` (`Id`) ON DELETE CASCADE
);

CREATE TABLE `Ethnicities_Characteristics` (
    `EthnicityId` bigint(20) NOT NULL,
    `CharacteristicDefinitionId` bigint(20) NOT NULL,
    `CharacteristicProfileId` bigint(20) NOT NULL,
    CONSTRAINT `PRIMARY` PRIMARY KEY (`EthnicityId`, `CharacteristicDefinitionId`, `CharacteristicProfileId`),
    CONSTRAINT `FK_Ethnicities_Characteristics_CharacteristicDefinitions` FOREIGN KEY (`CharacteristicDefinitionId`) REFERENCES `CharacteristicDefinitions` (`Id`) ON DELETE RESTRICT,
    CONSTRAINT `FK_Ethnicities_Characteristics_CharacteristicProfiles` FOREIGN KEY (`CharacteristicProfileId`) REFERENCES `CharacteristicProfiles` (`Id`) ON DELETE RESTRICT,
    CONSTRAINT `FK_Ethnicities_Characteristics_Ethnicities` FOREIGN KEY (`EthnicityId`) REFERENCES `Ethnicities` (`Id`) ON DELETE CASCADE
);

CREATE TABLE `Ethnicities_ChargenResources` (
    `EthnicityId` bigint(20) NOT NULL,
    `ChargenResourceId` bigint(20) NOT NULL,
    `RequirementOnly` bit(1) NOT NULL DEFAULT b'0',
    `Amount` int(11) NOT NULL,
    CONSTRAINT `PRIMARY` PRIMARY KEY (`EthnicityId`, `ChargenResourceId`, `RequirementOnly`),
    CONSTRAINT `FK_Ethnicities_ChargenResources_ChargenResources` FOREIGN KEY (`ChargenResourceId`) REFERENCES `ChargenResources` (`Id`) ON DELETE CASCADE,
    CONSTRAINT `FK_Ethnicities_ChargenResources_Ethnicities` FOREIGN KEY (`EthnicityId`) REFERENCES `Ethnicities` (`Id`) ON DELETE CASCADE
);

CREATE TABLE `Bodies_DrugDoses` (
    `BodyId` bigint(20) NOT NULL,
    `DrugId` bigint(20) NOT NULL,
    `Active` bit(1) NOT NULL,
    `Grams` double NOT NULL,
    `OriginalVector` int(11) NOT NULL,
    CONSTRAINT `PRIMARY` PRIMARY KEY (`BodyId`, `DrugId`, `Active`),
    CONSTRAINT `FK_Bodies_DrugDoses_Bodies` FOREIGN KEY (`BodyId`) REFERENCES `Bodies` (`Id`) ON DELETE CASCADE,
    CONSTRAINT `FK_Bodies_DrugDoses_Drugs` FOREIGN KEY (`DrugId`) REFERENCES `Drugs` (`Id`) ON DELETE CASCADE
);

CREATE TABLE `Bodies_GameItems` (
    `BodyId` bigint(20) NOT NULL,
    `GameItemId` bigint(20) NOT NULL,
    `EquippedOrder` int(11) NOT NULL,
    `WearProfile` bigint(20) NULL,
    `Wielded` int(11) NULL,
    CONSTRAINT `PRIMARY` PRIMARY KEY (`BodyId`, `GameItemId`),
    CONSTRAINT `FK_Bodies_GameItems_Bodies` FOREIGN KEY (`BodyId`) REFERENCES `Bodies` (`Id`) ON DELETE CASCADE,
    CONSTRAINT `FK_Bodies_GameItems_GameItems` FOREIGN KEY (`GameItemId`) REFERENCES `GameItems` (`Id`) ON DELETE CASCADE
);

CREATE TABLE `Bodies_Implants` (
    `BodyId` bigint(20) NOT NULL,
    `ImplantId` bigint(20) NOT NULL,
    CONSTRAINT `PRIMARY` PRIMARY KEY (`BodyId`, `ImplantId`),
    CONSTRAINT `FK_Bodies_Implants_Bodies` FOREIGN KEY (`BodyId`) REFERENCES `Bodies` (`Id`) ON DELETE CASCADE,
    CONSTRAINT `FK_Bodies_Implants_GameItems` FOREIGN KEY (`ImplantId`) REFERENCES `GameItems` (`Id`) ON DELETE CASCADE
);

CREATE TABLE `Bodies_Prosthetics` (
    `BodyId` bigint(20) NOT NULL,
    `ProstheticId` bigint(20) NOT NULL,
    CONSTRAINT `PRIMARY` PRIMARY KEY (`BodyId`, `ProstheticId`),
    CONSTRAINT `FK_Bodies_Prosthetics_Bodies` FOREIGN KEY (`BodyId`) REFERENCES `Bodies` (`Id`) ON DELETE CASCADE,
    CONSTRAINT `FK_Bodies_Prosthetics_GameItems` FOREIGN KEY (`ProstheticId`) REFERENCES `GameItems` (`Id`) ON DELETE CASCADE
);

CREATE TABLE `Bodies_SeveredParts` (
    `BodiesId` bigint(20) NOT NULL,
    `BodypartProtoId` bigint(20) NOT NULL,
    CONSTRAINT `PRIMARY` PRIMARY KEY (`BodiesId`, `BodypartProtoId`),
    CONSTRAINT `FK_Bodies_SeveredParts_Bodies` FOREIGN KEY (`BodiesId`) REFERENCES `Bodies` (`Id`) ON DELETE CASCADE,
    CONSTRAINT `FK_Bodies_SeveredParts_BodypartProtos` FOREIGN KEY (`BodypartProtoId`) REFERENCES `BodypartProto` (`Id`) ON DELETE CASCADE
);

CREATE TABLE `Characteristics` (
    `BodyId` bigint(20) NOT NULL,
    `Type` int(11) NOT NULL,
    `CharacteristicId` bigint(20) NOT NULL,
    CONSTRAINT `PRIMARY` PRIMARY KEY (`BodyId`, `Type`),
    CONSTRAINT `FK_Characteristics_Bodies` FOREIGN KEY (`BodyId`) REFERENCES `Bodies` (`Id`) ON DELETE RESTRICT,
    CONSTRAINT `FK_Characteristics_CharacteristicValues` FOREIGN KEY (`CharacteristicId`) REFERENCES `CharacteristicValues` (`Id`) ON DELETE CASCADE
);

CREATE TABLE `PerceiverMerits` (
    `Id` bigint(20) NOT NULL AUTO_INCREMENT,
    `MeritId` bigint(20) NOT NULL,
    `BodyId` bigint(20) NULL,
    `CharacterId` bigint(20) NULL,
    `GameItemId` bigint(20) NULL,
    CONSTRAINT `PK_PerceiverMerits` PRIMARY KEY (`Id`),
    CONSTRAINT `FK_PerceiverMerits_Bodies` FOREIGN KEY (`BodyId`) REFERENCES `Bodies` (`Id`) ON DELETE CASCADE,
    CONSTRAINT `FK_PerceiverMerits_Characters` FOREIGN KEY (`CharacterId`) REFERENCES `Characters` (`Id`) ON DELETE CASCADE,
    CONSTRAINT ` FK_PerceiverMerits_Merits` FOREIGN KEY (`MeritId`) REFERENCES `Merits` (`Id`) ON DELETE CASCADE
);

CREATE TABLE `Traits` (
    `BodyId` bigint(20) NOT NULL,
    `TraitDefinitionId` bigint(20) NOT NULL,
    `Value` double NOT NULL,
    `AdditionalValue` double NOT NULL,
    CONSTRAINT `PRIMARY` PRIMARY KEY (`BodyId`, `TraitDefinitionId`),
    CONSTRAINT `FK_Traits_Bodies` FOREIGN KEY (`BodyId`) REFERENCES `Bodies` (`Id`) ON DELETE CASCADE,
    CONSTRAINT `FK_Traits_TraitDefinitions` FOREIGN KEY (`TraitDefinitionId`) REFERENCES `TraitDefinitions` (`Id`) ON DELETE CASCADE
);

CREATE TABLE `Wounds` (
    `Id` bigint(20) NOT NULL AUTO_INCREMENT,
    `BodyId` bigint(20) NULL,
    `GameItemId` bigint(20) NULL,
    `OriginalDamage` double NOT NULL,
    `CurrentDamage` double NOT NULL,
    `CurrentPain` double NOT NULL,
    `CurrentShock` double NOT NULL,
    `CurrentStun` double NOT NULL,
    `LodgedItemId` bigint(20) NULL,
    `DamageType` int(11) NOT NULL,
    `Internal` bit(1) NOT NULL DEFAULT b'0',
    `BodypartProtoId` bigint(20) NULL,
    `ExtraInformation` text CHARACTER SET utf8mb4 NULL,
    `ActorOriginId` bigint(20) NULL,
    `ToolOriginId` bigint(20) NULL,
    `WoundType` varchar(50) CHARACTER SET utf8mb4 NOT NULL,
    CONSTRAINT `PK_Wounds` PRIMARY KEY (`Id`),
    CONSTRAINT `FK_Wounds_Characters` FOREIGN KEY (`ActorOriginId`) REFERENCES `Characters` (`Id`) ON DELETE SET NULL,
    CONSTRAINT `FK_Wounds_Bodies` FOREIGN KEY (`BodyId`) REFERENCES `Bodies` (`Id`) ON DELETE CASCADE,
    CONSTRAINT `FK_Wounds_GameItemOwner` FOREIGN KEY (`GameItemId`) REFERENCES `GameItems` (`Id`) ON DELETE CASCADE,
    CONSTRAINT `FK_Wounds_GameItems` FOREIGN KEY (`LodgedItemId`) REFERENCES `GameItems` (`Id`) ON DELETE SET NULL,
    CONSTRAINT `FK_Wounds_GameItems_Tool` FOREIGN KEY (`ToolOriginId`) REFERENCES `GameItems` (`Id`) ON DELETE SET NULL
);

CREATE TABLE `Infections` (
    `Id` bigint(20) NOT NULL AUTO_INCREMENT,
    `InfectionType` int(11) NOT NULL,
    `Virulence` int(11) NOT NULL,
    `Intensity` double NOT NULL,
    `OwnerId` bigint(20) NOT NULL,
    `WoundId` bigint(20) NULL,
    `BodypartId` bigint(20) NULL,
    `Immunity` double NOT NULL,
    CONSTRAINT `PK_Infections` PRIMARY KEY (`Id`),
    CONSTRAINT `FK_Infections_Bodyparts` FOREIGN KEY (`BodypartId`) REFERENCES `BodypartProto` (`Id`) ON DELETE CASCADE,
    CONSTRAINT `FK_Infections_Bodies` FOREIGN KEY (`OwnerId`) REFERENCES `Bodies` (`Id`) ON DELETE CASCADE,
    CONSTRAINT `FK_Infections_Wounds` FOREIGN KEY (`WoundId`) REFERENCES `Wounds` (`Id`) ON DELETE CASCADE
);

CREATE TABLE `Hooks_Perceivables` (
    `Id` bigint(20) NOT NULL AUTO_INCREMENT,
    `HookId` bigint(20) NOT NULL,
    `BodyId` bigint(20) NULL,
    `CharacterId` bigint(20) NULL,
    `GameItemId` bigint(20) NULL,
    `CellId` bigint(20) NULL,
    `ZoneId` bigint(20) NULL,
    `ShardId` bigint(20) NULL,
    CONSTRAINT `PK_Hooks_Perceivables` PRIMARY KEY (`Id`),
    CONSTRAINT `FK_Hooks_Perceivables_Bodies` FOREIGN KEY (`BodyId`) REFERENCES `Bodies` (`Id`) ON DELETE CASCADE,
    CONSTRAINT `FK_Hooks_Perceivables_Characters` FOREIGN KEY (`CharacterId`) REFERENCES `Characters` (`Id`) ON DELETE CASCADE,
    CONSTRAINT `FK_Hooks_Perceivables_GameItems` FOREIGN KEY (`GameItemId`) REFERENCES `GameItems` (`Id`) ON DELETE CASCADE,
    CONSTRAINT `FK_Hooks_Perceivables_Hooks` FOREIGN KEY (`HookId`) REFERENCES `Hooks` (`Id`) ON DELETE CASCADE,
    CONSTRAINT `FK_Hooks_Perceivables_Shards` FOREIGN KEY (`ShardId`) REFERENCES `Shards` (`Id`) ON DELETE CASCADE
);

CREATE TABLE `EconomicZones` (
    `Id` bigint(20) NOT NULL AUTO_INCREMENT,
    `Name` varchar(200) CHARACTER SET utf8 NOT NULL,
    `PreviousFinancialPeriodsToKeep` int(11) NOT NULL DEFAULT '50',
    `ZoneForTimePurposesId` bigint(20) NOT NULL,
    `PermitTaxableLosses` bit(1) NOT NULL DEFAULT b'1',
    `OutstandingTaxesOwed` decimal(10,0) NOT NULL,
    `CurrencyId` bigint(20) NOT NULL,
    `CurrentFinancialPeriodId` bigint(20) NULL,
    `ReferenceCalendarId` bigint(20) NULL,
    `ReferenceClockId` bigint(20) NOT NULL,
    `ReferenceTime` varchar(100) CHARACTER SET utf8 NOT NULL,
    `IntervalType` int(11) NOT NULL DEFAULT '2',
    `IntervalModifier` int(11) NOT NULL,
    `IntervalAmount` int(11) NOT NULL DEFAULT '1',
    CONSTRAINT `PK_EconomicZones` PRIMARY KEY (`Id`),
    CONSTRAINT `FK_EconomicZones_Currencies` FOREIGN KEY (`CurrencyId`) REFERENCES `Currencies` (`Id`) ON DELETE RESTRICT,
    CONSTRAINT `FK_EconomicZones_Calendars` FOREIGN KEY (`ReferenceCalendarId`) REFERENCES `Calendars` (`Id`) ON DELETE SET NULL,
    CONSTRAINT `FK_EconomicZones_Clocks` FOREIGN KEY (`ReferenceClockId`) REFERENCES `Clocks` (`Id`) ON DELETE RESTRICT,
    CONSTRAINT `FK_EconomicZones_Timezones` FOREIGN KEY (`ReferenceClockId`) REFERENCES `Timezones` (`Id`) ON DELETE RESTRICT
);

CREATE TABLE `FinancialPeriods` (
    `Id` bigint(20) NOT NULL AUTO_INCREMENT,
    `EconomicZoneId` bigint(20) NOT NULL,
    `PeriodStart` datetime NOT NULL,
    `PeriodEnd` datetime NOT NULL,
    `MudPeriodStart` varchar(200) CHARACTER SET utf8 NOT NULL,
    `MudPeriodEnd` varchar(200) CHARACTER SET utf8 NOT NULL,
    CONSTRAINT `PK_FinancialPeriods` PRIMARY KEY (`Id`),
    CONSTRAINT `FK_FinancialPeriods_EconomicZones` FOREIGN KEY (`EconomicZoneId`) REFERENCES `EconomicZones` (`Id`) ON DELETE CASCADE
);

CREATE TABLE `EconomicZoneRevenues` (
    `EconomicZoneId` bigint(20) NOT NULL,
    `FinancialPeriodId` bigint(20) NOT NULL,
    `TotalTaxRevenue` decimal(10,0) NOT NULL,
    CONSTRAINT `PRIMARY` PRIMARY KEY (`EconomicZoneId`, `FinancialPeriodId`),
    CONSTRAINT `FK_EconomicZoneRevenues` FOREIGN KEY (`EconomicZoneId`) REFERENCES `EconomicZones` (`Id`) ON DELETE CASCADE,
    CONSTRAINT `FK_EconomicZoneRevenues_FinancialPeriods` FOREIGN KEY (`FinancialPeriodId`) REFERENCES `FinancialPeriods` (`Id`) ON DELETE CASCADE
);

CREATE TABLE `CellOverlays` (
    `Id` bigint(20) NOT NULL AUTO_INCREMENT,
    `Name` varchar(4000) CHARACTER SET utf8 NOT NULL,
    `CellName` varchar(4000) CHARACTER SET utf8 NOT NULL,
    `CellDescription` varchar(4000) CHARACTER SET utf8 NOT NULL,
    `CellOverlayPackageId` bigint(20) NOT NULL,
    `CellId` bigint(20) NOT NULL,
    `CellOverlayPackageRevisionNumber` int(11) NOT NULL,
    `TerrainId` bigint(20) NOT NULL,
    `HearingProfileId` bigint(20) NULL,
    `OutdoorsType` int(11) NOT NULL,
    `AmbientLightFactor` double NOT NULL DEFAULT '1',
    `AddedLight` double NOT NULL,
    `AtmosphereId` bigint(20) NULL,
    `AtmosphereType` varchar(45) CHARACTER SET utf8 NOT NULL DEFAULT 'gas',
    CONSTRAINT `PK_CellOverlays` PRIMARY KEY (`Id`),
    CONSTRAINT `FK_CellOverlays_HearingProfiles` FOREIGN KEY (`HearingProfileId`) REFERENCES `HearingProfiles` (`Id`) ON DELETE SET NULL,
    CONSTRAINT `FK_CellOverlays_Terrains` FOREIGN KEY (`TerrainId`) REFERENCES `Terrains` (`Id`) ON DELETE RESTRICT,
    CONSTRAINT `FK_CellOverlays_CellOverlayPackages` FOREIGN KEY (`CellOverlayPackageId`, `CellOverlayPackageRevisionNumber`) REFERENCES `CellOverlayPackages` (`Id`, `RevisionNumber`) ON DELETE RESTRICT
);

CREATE TABLE `CellOverlays_Exits` (
    `CellOverlayId` bigint(20) NOT NULL,
    `ExitId` bigint(20) NOT NULL,
    CONSTRAINT `PRIMARY` PRIMARY KEY (`CellOverlayId`, `ExitId`),
    CONSTRAINT `FK_CellOverlays_Exits_CellOverlays` FOREIGN KEY (`CellOverlayId`) REFERENCES `CellOverlays` (`Id`) ON DELETE RESTRICT,
    CONSTRAINT `FK_CellOverlays_Exits_Exits` FOREIGN KEY (`ExitId`) REFERENCES `Exits` (`Id`) ON DELETE RESTRICT
);

CREATE TABLE `Cells` (
    `Id` bigint(20) NOT NULL AUTO_INCREMENT,
    `RoomId` bigint(20) NOT NULL,
    `CurrentOverlayId` bigint(20) NULL,
    `ForagableProfileId` bigint(20) NULL,
    `Temporary` bit(1) NOT NULL DEFAULT b'0',
    `EffectData` mediumtext CHARACTER SET utf8 NOT NULL,
    CONSTRAINT `PK_Cells` PRIMARY KEY (`Id`),
    CONSTRAINT `FK_Cells_CellOverlays` FOREIGN KEY (`CurrentOverlayId`) REFERENCES `CellOverlays` (`Id`) ON DELETE SET NULL
);

CREATE TABLE `ActiveProjects` (
    `Id` bigint(20) NOT NULL AUTO_INCREMENT,
    `ProjectId` bigint(20) NOT NULL,
    `ProjectRevisionNumber` int(11) NOT NULL,
    `CurrentPhaseId` bigint(20) NOT NULL,
    `CharacterId` bigint(20) NULL,
    `CellId` bigint(20) NULL,
    CONSTRAINT `PK_ActiveProjects` PRIMARY KEY (`Id`),
    CONSTRAINT `FK_ActiveProjects_Cells` FOREIGN KEY (`CellId`) REFERENCES `Cells` (`Id`) ON DELETE RESTRICT,
    CONSTRAINT `FK_ActiveProjects_Characters` FOREIGN KEY (`CharacterId`) REFERENCES `Characters` (`Id`) ON DELETE RESTRICT,
    CONSTRAINT `FK_ActiveProjects_ProjectPhases` FOREIGN KEY (`CurrentPhaseId`) REFERENCES `ProjectPhases` (`Id`) ON DELETE CASCADE,
    CONSTRAINT `FK_ActiveProjects_Projects` FOREIGN KEY (`ProjectId`, `ProjectRevisionNumber`) REFERENCES `Projects` (`Id`, `RevisionNumber`) ON DELETE CASCADE
);

CREATE TABLE `Cells_ForagableYields` (
    `CellId` bigint(20) NOT NULL,
    `ForagableType` varchar(100) CHARACTER SET utf8mb4 NOT NULL,
    `Yield` double NOT NULL,
    CONSTRAINT `PRIMARY` PRIMARY KEY (`CellId`, `ForagableType`),
    CONSTRAINT `FK_Cells_ForagableYields_Cells` FOREIGN KEY (`CellId`) REFERENCES `Cells` (`Id`) ON DELETE CASCADE
);

CREATE TABLE `Cells_GameItems` (
    `CellId` bigint(20) NOT NULL,
    `GameItemId` bigint(20) NOT NULL,
    CONSTRAINT `PRIMARY` PRIMARY KEY (`CellId`, `GameItemId`),
    CONSTRAINT `FK_Cells_GameItems_Cells` FOREIGN KEY (`CellId`) REFERENCES `Cells` (`Id`) ON DELETE CASCADE,
    CONSTRAINT `FK_Cells_GameItems_GameItems` FOREIGN KEY (`GameItemId`) REFERENCES `GameItems` (`Id`) ON DELETE CASCADE
);

CREATE TABLE `Cells_MagicResources` (
    `CellId` bigint(20) NOT NULL,
    `MagicResourceId` bigint(20) NOT NULL,
    `Amount` double NOT NULL,
    CONSTRAINT `PRIMARY` PRIMARY KEY (`CellId`, `MagicResourceId`),
    CONSTRAINT `FK_Cells_MagicResources_Cells` FOREIGN KEY (`CellId`) REFERENCES `Cells` (`Id`) ON DELETE CASCADE,
    CONSTRAINT `FK_Cells_MagicResources_MagicResources` FOREIGN KEY (`MagicResourceId`) REFERENCES `MagicResources` (`Id`) ON DELETE CASCADE
);

CREATE TABLE `Cells_RangedCovers` (
    `CellId` bigint(20) NOT NULL,
    `RangedCoverId` bigint(20) NOT NULL,
    CONSTRAINT `PRIMARY` PRIMARY KEY (`CellId`, `RangedCoverId`),
    CONSTRAINT `FK_Cells_RangedCovers_Cells` FOREIGN KEY (`CellId`) REFERENCES `Cells` (`Id`) ON DELETE CASCADE,
    CONSTRAINT `FK_Cells_RangedCovers_RangedCovers` FOREIGN KEY (`RangedCoverId`) REFERENCES `RangedCovers` (`Id`) ON DELETE CASCADE
);

CREATE TABLE `Cells_Tags` (
    `CellId` bigint(20) NOT NULL,
    `TagId` bigint(20) NOT NULL,
    CONSTRAINT `PRIMARY` PRIMARY KEY (`CellId`, `TagId`),
    CONSTRAINT `FK_Cells_Tags_Cells` FOREIGN KEY (`CellId`) REFERENCES `Cells` (`Id`) ON DELETE RESTRICT,
    CONSTRAINT `FK_Cells_Tags_Tags` FOREIGN KEY (`TagId`) REFERENCES `Tags` (`Id`) ON DELETE CASCADE
);

CREATE TABLE `Clans_AdministrationCells` (
    `ClanId` bigint(20) NOT NULL,
    `CellId` bigint(20) NOT NULL,
    CONSTRAINT `PRIMARY` PRIMARY KEY (`ClanId`, `CellId`),
    CONSTRAINT `FK_Clans_AdministrationCells_Cells` FOREIGN KEY (`CellId`) REFERENCES `Cells` (`Id`) ON DELETE CASCADE,
    CONSTRAINT `FK_Clans_AdministrationCells_Clans` FOREIGN KEY (`ClanId`) REFERENCES `Clans` (`Id`) ON DELETE CASCADE
);

CREATE TABLE `Clans_TreasuryCells` (
    `ClanId` bigint(20) NOT NULL,
    `CellId` bigint(20) NOT NULL,
    CONSTRAINT `PRIMARY` PRIMARY KEY (`ClanId`, `CellId`),
    CONSTRAINT `FK_Clans_TreasuryCells_Cells` FOREIGN KEY (`CellId`) REFERENCES `Cells` (`Id`) ON DELETE CASCADE,
    CONSTRAINT `FK_Clans_TreasuryCells_Clans` FOREIGN KEY (`ClanId`) REFERENCES `Clans` (`Id`) ON DELETE CASCADE
);

CREATE TABLE `Crimes` (
    `Id` bigint(20) NOT NULL AUTO_INCREMENT,
    `LawId` bigint(20) NOT NULL,
    `CriminalId` bigint(20) NOT NULL,
    `VictimId` bigint(20) NULL,
    `TimeOfCrime` varchar(200) CHARACTER SET utf8 NOT NULL,
    `RealTimeOfCrime` datetime NOT NULL,
    `LocationId` bigint(20) NULL,
    `TimeOfReport` varchar(200) CHARACTER SET utf8 NULL,
    `AccuserId` bigint(20) NULL,
    `CriminalShortDescription` varchar(200) CHARACTER SET utf8 NOT NULL,
    `CriminalFullDescription` varchar(1000) CHARACTER SET utf8 NOT NULL,
    `CriminalCharacteristics` varchar(500) CHARACTER SET utf8 NOT NULL,
    `IsKnownCrime` bit(1) NOT NULL,
    `IsStaleCrime` bit(1) NOT NULL,
    `IsFinalised` bit(1) NOT NULL,
    `ConvictionRecorded` bit(1) NOT NULL,
    `IsCriminalIdentityKnown` bit(1) NOT NULL,
    `BailHasBeenPosted` bit(1) NOT NULL,
    CONSTRAINT `PK_Crimes` PRIMARY KEY (`Id`),
    CONSTRAINT `FK_Crimes_Accuser` FOREIGN KEY (`AccuserId`) REFERENCES `Characters` (`Id`) ON DELETE SET NULL,
    CONSTRAINT `FK_Crimes_Criminal` FOREIGN KEY (`CriminalId`) REFERENCES `Characters` (`Id`) ON DELETE CASCADE,
    CONSTRAINT `FK_Crimes_Laws` FOREIGN KEY (`LawId`) REFERENCES `Laws` (`Id`) ON DELETE CASCADE,
    CONSTRAINT `FK_Crimes_Location` FOREIGN KEY (`LocationId`) REFERENCES `Cells` (`Id`) ON DELETE SET NULL,
    CONSTRAINT `FK_Crimes_Victim` FOREIGN KEY (`VictimId`) REFERENCES `Characters` (`Id`) ON DELETE SET NULL
);

CREATE TABLE `Shops` (
    `Id` bigint(20) NOT NULL AUTO_INCREMENT,
    `Name` varchar(200) CHARACTER SET utf8 NOT NULL,
    `WorkshopCellId` bigint(20) NULL,
    `StockroomCellId` bigint(20) NULL,
    `CanShopProgId` bigint(20) NULL,
    `WhyCannotShopProgId` bigint(20) NULL,
    `CurrencyId` bigint(20) NOT NULL,
    `IsTrading` bit(1) NOT NULL,
    `EconomicZoneId` bigint(20) NOT NULL,
    `EmployeeRecords` mediumtext CHARACTER SET utf8 NOT NULL,
    CONSTRAINT `PK_Shops` PRIMARY KEY (`Id`),
    CONSTRAINT `FK_Shops_FutureProgs_Can` FOREIGN KEY (`CanShopProgId`) REFERENCES `FutureProgs` (`Id`) ON DELETE SET NULL,
    CONSTRAINT `FK_Shops_Currencies` FOREIGN KEY (`CurrencyId`) REFERENCES `Currencies` (`Id`) ON DELETE RESTRICT,
    CONSTRAINT `FK_Shops_EconomicZones` FOREIGN KEY (`EconomicZoneId`) REFERENCES `EconomicZones` (`Id`) ON DELETE CASCADE,
    CONSTRAINT `FK_Shops_Cells_Stockroom` FOREIGN KEY (`StockroomCellId`) REFERENCES `Cells` (`Id`) ON DELETE SET NULL,
    CONSTRAINT `FK_Shops_FutureProgs_WhyCant` FOREIGN KEY (`WhyCannotShopProgId`) REFERENCES `FutureProgs` (`Id`) ON DELETE SET NULL,
    CONSTRAINT `FK_Shops_Cells_Workshop` FOREIGN KEY (`WorkshopCellId`) REFERENCES `Cells` (`Id`) ON DELETE SET NULL
);

CREATE TABLE `Zones` (
    `Id` bigint(20) NOT NULL AUTO_INCREMENT,
    `Name` varchar(255) CHARACTER SET utf8 NOT NULL,
    `ShardId` bigint(20) NOT NULL,
    `Latitude` double NOT NULL,
    `Longitude` double NOT NULL,
    `Elevation` double NOT NULL,
    `DefaultCellId` bigint(20) NULL,
    `AmbientLightPollution` double NOT NULL,
    `ForagableProfileId` bigint(20) NULL,
    `WeatherControllerId` bigint(20) NULL,
    CONSTRAINT `PK_Zones` PRIMARY KEY (`Id`),
    CONSTRAINT `FK_Zones_Cells` FOREIGN KEY (`DefaultCellId`) REFERENCES `Cells` (`Id`) ON DELETE RESTRICT,
    CONSTRAINT `FK_Zones_Shards` FOREIGN KEY (`ShardId`) REFERENCES `Shards` (`Id`) ON DELETE RESTRICT,
    CONSTRAINT `FK_Zones_WeatherControllers` FOREIGN KEY (`WeatherControllerId`) REFERENCES `WeatherControllers` (`Id`) ON DELETE RESTRICT
);

CREATE TABLE `EconomicZoneShopTaxes` (
    `EconomicZoneId` bigint(20) NOT NULL,
    `ShopId` bigint(20) NOT NULL,
    `OutstandingProfitTaxes` decimal(10,0) NOT NULL,
    `OutstandingSalesTaxes` decimal(10,0) NOT NULL,
    `TaxesInCredits` decimal(10,0) NOT NULL,
    CONSTRAINT `PRIMARY` PRIMARY KEY (`EconomicZoneId`, `ShopId`),
    CONSTRAINT `FK_EconomicZoneShopTaxes_EconomicZones` FOREIGN KEY (`EconomicZoneId`) REFERENCES `EconomicZones` (`Id`) ON DELETE CASCADE,
    CONSTRAINT `FK_EconomicZoneShopTaxes_Shops` FOREIGN KEY (`ShopId`) REFERENCES `Shops` (`Id`) ON DELETE CASCADE
);

CREATE TABLE `Merchandises` (
    `Id` bigint(20) NOT NULL AUTO_INCREMENT,
    `Name` varchar(1000) CHARACTER SET utf8 NOT NULL,
    `ShopId` bigint(20) NOT NULL,
    `AutoReordering` bit(1) NOT NULL,
    `AutoReorderPrice` decimal(10,0) NOT NULL,
    `BasePrice` decimal(10,0) NOT NULL,
    `DefaultMerchandiseForItem` bit(1) NOT NULL,
    `ItemProtoId` bigint(20) NOT NULL,
    `PreferredDisplayContainerId` bigint(20) NULL,
    `ListDescription` varchar(500) CHARACTER SET utf8 NULL,
    `MinimumStockLevels` int(11) NOT NULL,
    `MinimumStockLevelsByWeight` double NOT NULL,
    `PreserveVariablesOnReorder` bit(1) NOT NULL,
    CONSTRAINT `PK_Merchandises` PRIMARY KEY (`Id`),
    CONSTRAINT `FK_Merchandises_GameItems` FOREIGN KEY (`PreferredDisplayContainerId`) REFERENCES `GameItems` (`Id`) ON DELETE CASCADE,
    CONSTRAINT `FK_Merchandises_Shops` FOREIGN KEY (`ShopId`) REFERENCES `Shops` (`Id`) ON DELETE CASCADE
);

CREATE TABLE `ShopFinancialPeriodResults` (
    `EconomicZoneId` bigint(20) NOT NULL,
    `ShopId` bigint(20) NOT NULL,
    `FinancialPeriodId` bigint(20) NOT NULL,
    `GrossRevenue` decimal(10,0) NOT NULL,
    `NetRevenue` decimal(10,0) NOT NULL,
    `SalesTax` decimal(10,0) NOT NULL,
    `ProfitsTax` decimal(10,0) NOT NULL,
    CONSTRAINT `PRIMARY` PRIMARY KEY (`EconomicZoneId`, `ShopId`, `FinancialPeriodId`),
    CONSTRAINT `FK_ShopFinancialPeriodResults_EconomicZones` FOREIGN KEY (`EconomicZoneId`) REFERENCES `EconomicZones` (`Id`) ON DELETE CASCADE,
    CONSTRAINT `FK_ShopFinancialPeriodResults_FinancialPeriods` FOREIGN KEY (`FinancialPeriodId`) REFERENCES `FinancialPeriods` (`Id`) ON DELETE CASCADE,
    CONSTRAINT `FK_ShopFinancialPeriodResults_Shops` FOREIGN KEY (`ShopId`) REFERENCES `Shops` (`Id`) ON DELETE CASCADE
);

CREATE TABLE `Shops_StoreroomCells` (
    `ShopId` bigint(20) NOT NULL,
    `CellId` bigint(20) NOT NULL,
    CONSTRAINT `PRIMARY` PRIMARY KEY (`ShopId`, `CellId`),
    CONSTRAINT `FK_Shops_StoreroomCells_Cells` FOREIGN KEY (`CellId`) REFERENCES `Cells` (`Id`) ON DELETE CASCADE,
    CONSTRAINT `FK_Shops_StoreroomCells_Shops` FOREIGN KEY (`ShopId`) REFERENCES `Shops` (`Id`) ON DELETE CASCADE
);

CREATE TABLE `ShopsTills` (
    `ShopId` bigint(20) NOT NULL,
    `GameItemId` bigint(20) NOT NULL,
    CONSTRAINT `PRIMARY` PRIMARY KEY (`ShopId`, `GameItemId`),
    CONSTRAINT `FK_ShopTills_GameItems` FOREIGN KEY (`GameItemId`) REFERENCES `GameItems` (`Id`) ON DELETE CASCADE,
    CONSTRAINT `FK_ShopTills_Shops` FOREIGN KEY (`ShopId`) REFERENCES `Shops` (`Id`) ON DELETE CASCADE
);

CREATE TABLE `ShopTransactionRecords` (
    `Id` bigint(20) NOT NULL AUTO_INCREMENT,
    `CurrencyId` bigint(20) NOT NULL,
    `PretaxValue` decimal(10,0) NOT NULL,
    `Tax` decimal(10,0) NOT NULL,
    `TransactionType` int(11) NOT NULL,
    `ShopId` bigint(20) NOT NULL,
    `ThirdPartyId` bigint(20) NULL,
    `RealDateTime` datetime NOT NULL,
    `MudDateTime` varchar(500) CHARACTER SET utf8 NOT NULL,
    CONSTRAINT `PK_ShopTransactionRecords` PRIMARY KEY (`Id`),
    CONSTRAINT `FK_ShopTransactionRecords_Currencies` FOREIGN KEY (`CurrencyId`) REFERENCES `Currencies` (`Id`) ON DELETE CASCADE,
    CONSTRAINT `FK_ShopTransactionRecords_Shops` FOREIGN KEY (`ShopId`) REFERENCES `Shops` (`Id`) ON DELETE CASCADE
);

CREATE TABLE `LegalAuthorities_Zones` (
    `ZoneId` bigint(20) NOT NULL,
    `LegalAuthorityId` bigint(20) NOT NULL,
    CONSTRAINT `PRIMARY` PRIMARY KEY (`ZoneId`, `LegalAuthorityId`),
    CONSTRAINT `FK_LegalAuthorities_Zones_LegalAuthorities` FOREIGN KEY (`LegalAuthorityId`) REFERENCES `LegalAuthorities` (`Id`) ON DELETE CASCADE,
    CONSTRAINT `FK_LegalAuthorities_Zones_Zones` FOREIGN KEY (`ZoneId`) REFERENCES `Zones` (`Id`) ON DELETE CASCADE
);

CREATE TABLE `Rooms` (
    `Id` bigint(20) NOT NULL AUTO_INCREMENT,
    `ZoneId` bigint(20) NOT NULL,
    `X` int(11) NOT NULL,
    `Y` int(11) NOT NULL,
    `Z` int(11) NOT NULL,
    CONSTRAINT `PK_Rooms` PRIMARY KEY (`Id`),
    CONSTRAINT `FK_Rooms_Zones` FOREIGN KEY (`ZoneId`) REFERENCES `Zones` (`Id`) ON DELETE RESTRICT
);

CREATE TABLE `Zones_Timezones` (
    `ZoneId` bigint(20) NOT NULL,
    `ClockId` bigint(20) NOT NULL,
    `TimezoneId` bigint(20) NOT NULL,
    CONSTRAINT `PRIMARY` PRIMARY KEY (`ZoneId`, `ClockId`, `TimezoneId`),
    CONSTRAINT `FK_Zones_Timezones_Zones` FOREIGN KEY (`ZoneId`) REFERENCES `Zones` (`Id`) ON DELETE RESTRICT
);

CREATE INDEX `FK_Accents_Languages` ON `Accents` (`LanguageId`);

CREATE INDEX `FK_AccountNotes_Accounts` ON `AccountNotes` (`AccountId`);

CREATE INDEX `FK_AccountNotes_Author` ON `AccountNotes` (`AuthorId`);

CREATE INDEX `FK_Accounts_AuthorityGroups` ON `Accounts` (`AuthorityGroupId`);

CREATE INDEX `FK_Accounts_ChargenResources_ChargenResources` ON `Accounts_ChargenResources` (`ChargenResourceId`);

CREATE INDEX `FK_ActiveProjectLabours_ProjectLabourRequirements_idx` ON `ActiveProjectLabours` (`ProjectLabourRequirementsId`);

CREATE INDEX `FK_ActiveProjectMaterials_ProjectMaterialRequirements_idx` ON `ActiveProjectMaterials` (`ProjectMaterialRequirementsId`);

CREATE INDEX `FK_ActiveProjects_Cells_idx` ON `ActiveProjects` (`CellId`);

CREATE INDEX `FK_ActiveProjects_Characters_idx` ON `ActiveProjects` (`CharacterId`);

CREATE INDEX `FK_ActiveProjects_ProjectPhases_idx` ON `ActiveProjects` (`CurrentPhaseId`);

CREATE INDEX `FK_ActiveProjects_Projects_idx` ON `ActiveProjects` (`ProjectId`, `ProjectRevisionNumber`);

CREATE INDEX `FK_Allies_Characters_Target_idx` ON `Allies` (`AllyId`);

CREATE INDEX `FK_Appointments_Clans` ON `Appointments` (`ClanId`);

CREATE INDEX `FK_Appointments_Ranks` ON `Appointments` (`MinimumRankId`);

CREATE INDEX `FK_Appointments_Ranks_2` ON `Appointments` (`MinimumRankToAppointId`);

CREATE INDEX `FK_Appointments_ParentAppointment` ON `Appointments` (`ParentAppointmentId`);

CREATE INDEX `FK_Appointments_Paygrades` ON `Appointments` (`PaygradeId`);

CREATE INDEX `FK_Appointments_GameItemProtos` ON `Appointments` (`InsigniaGameItemId`, `InsigniaGameItemRevnum`);

CREATE INDEX `FK_Appointments_Abbreviations_Appointments` ON `Appointments_Abbreviations` (`AppointmentId`);

CREATE INDEX `FK_Appointments_Abbreviations_FutureProgs` ON `Appointments_Abbreviations` (`FutureProgId`);

CREATE INDEX `FK_Appointments_Titles_Appointments` ON `Appointments_Titles` (`AppointmentId`);

CREATE INDEX `FK_Appointments_Titles_FutureProgs` ON `Appointments_Titles` (`FutureProgId`);

CREATE INDEX `FK_Areas_WeatherControllers_idx` ON `Areas` (`WeatherControllerId`);

CREATE INDEX `FK_Areas_Rooms_Rooms_idx` ON `Areas_Rooms` (`RoomId`);

CREATE INDEX `FK_Bans_Accounts` ON `Bans` (`BannerAccountId`);

CREATE INDEX `FK_BloodModels_Bloodtypes_Bloodtypes_idx` ON `BloodModels_Bloodtypes` (`BloodtypeId`);

CREATE INDEX `FK_Bloodtypes_BloodtypeAntigens_BloodtypeAntigens_idx` ON `Bloodtypes_BloodtypeAntigens` (`BloodtypeAntigenId`);

CREATE INDEX `FK_BoardsPosts_Accounts_idx` ON `BoardPosts` (`AuthorId`);

CREATE INDEX `FK_BoardPosts_Boards_idx` ON `BoardPosts` (`BoardId`);

CREATE INDEX `FK_Bodies_Bloodtypes_idx` ON `Bodies` (`BloodtypeId`);

CREATE INDEX `FK_Bodies_Ethnicities_idx` ON `Bodies` (`EthnicityId`);

CREATE INDEX `FK_Bodies_EntityDescriptionPatterns_Full_idx` ON `Bodies` (`FullDescriptionPatternId`);

CREATE INDEX `FK_Bodies_Races` ON `Bodies` (`RaceId`);

CREATE INDEX `FK_Bodies_EntityDescriptionPatterns_Short_idx` ON `Bodies` (`ShortDescriptionPatternId`);

CREATE INDEX `FK_Bodies_DrugDoses_Drugs_idx` ON `Bodies_DrugDoses` (`DrugId`);

CREATE INDEX `FK_Bodies_GameItems_GameItems` ON `Bodies_GameItems` (`GameItemId`);

CREATE INDEX `FK_Bodies_Implants_GameItems_idx` ON `Bodies_Implants` (`ImplantId`);

CREATE INDEX `FK_Bodies_Prosthetics_GameItems_idx` ON `Bodies_Prosthetics` (`ProstheticId`);

CREATE INDEX `FK_Bodies_SeveredParts_BodypartProtos_idx` ON `Bodies_SeveredParts` (`BodypartProtoId`);

CREATE INDEX `FK_BGD_BodypartProtos_BodypartProto` ON `BodypartGroupDescribers_BodypartProtos` (`BodypartProtoId`);

CREATE INDEX `FK_BGD_BodyProtos_BodyProtos` ON `BodypartGroupDescribers_BodyProtos` (`BodyProtoId`);

CREATE INDEX `FK_BGD_ShapeCount_BodypartShape` ON `BodypartGroupDescribers_ShapeCount` (`TargetId`);

CREATE INDEX `FK_BodypartInternalInfos_BodypartProtos_Internal_idx` ON `BodypartInternalInfos` (`InternalPartId`);

CREATE INDEX `FK_BodypartProto_ArmourTypes_idx` ON `BodypartProto` (`ArmourTypeId`);

CREATE INDEX `FK_BodypartProto_BodyPrototype` ON `BodypartProto` (`BodyId`);

CREATE INDEX `FK_BodypartProto_BodypartShape` ON `BodypartProto` (`BodypartShapeId`);

CREATE INDEX `FK_BodypartProto_BodypartProto_idx` ON `BodypartProto` (`CountAsId`);

CREATE INDEX `FK_BodypartProto_Materials_idx` ON `BodypartProto` (`DefaultMaterialId`);

CREATE INDEX `FK_BodypartProto_AlignmentHits_BodypartProto` ON `BodypartProto_AlignmentHits` (`BodypartProtoId`);

CREATE INDEX `FKParent` ON `BodypartProto_BodypartProto_Upstream` (`Parent`);

CREATE INDEX `FK_BodypartProto_OrientationHits_BodypartProto` ON `BodypartProto_OrientationHits` (`BodypartProtoId`);

CREATE INDEX `FK_BodyPrototype_BodyPrototype_idx` ON `BodyProtos` (`CountsAsId`);

CREATE INDEX `FK_BodyPrototype_Bodyparts_idx` ON `BodyProtos` (`DefaultSmashingBodypartId`);

CREATE INDEX `FK_BodyPrototype_WearableSizeParameterRule` ON `BodyProtos` (`WearSizeParameterId`);

CREATE INDEX `FK_BodyProtos_AdditionalBodyparts_BodypartProto` ON `BodyProtos_AdditionalBodyparts` (`BodypartId`);

CREATE INDEX `FK_BoneOrganCoverages_BodypartProto_Organ_idx` ON `BoneOrganCoverages` (`OrganId`);

CREATE INDEX `FK_ButcheryProductItems_ButcheryProducts_idx` ON `ButcheryProductItems` (`ButcheryProductId`);

CREATE INDEX `FK_ButcheryProducts_FutureProgs_idx` ON `ButcheryProducts` (`CanProduceProgId`);

CREATE INDEX `FK_ButcheryProducts_BodyProtos_idx` ON `ButcheryProducts` (`TargetBodyId`);

CREATE INDEX `FK_ButcheryProducts_BodypartProtos_BodypartProtos_idx` ON `ButcheryProducts_BodypartProtos` (`BodypartProtoId`);

CREATE INDEX `FK_CellOverlayPackages_EditableItems` ON `CellOverlayPackages` (`EditableItemId`);

CREATE INDEX `FK_CellOverlays_Cells` ON `CellOverlays` (`CellId`);

CREATE INDEX `FK_CellOverlays_HearingProfiles` ON `CellOverlays` (`HearingProfileId`);

CREATE INDEX `FK_CellOverlays_Terrains` ON `CellOverlays` (`TerrainId`);

CREATE INDEX `FK_CellOverlays_CellOverlayPackages` ON `CellOverlays` (`CellOverlayPackageId`, `CellOverlayPackageRevisionNumber`);

CREATE INDEX `FK_CellOverlays_Exits_Exits` ON `CellOverlays_Exits` (`ExitId`);

CREATE INDEX `FK_Cells_CellOverlays` ON `Cells` (`CurrentOverlayId`);

CREATE INDEX `FK_Cells_Rooms` ON `Cells` (`RoomId`);

CREATE INDEX `FK_Cells_GameItems_GameItems` ON `Cells_GameItems` (`GameItemId`);

CREATE INDEX `FK_Cells_MagicResources_MagicResources_idx` ON `Cells_MagicResources` (`MagicResourceId`);

CREATE INDEX `FK_Cells_RangedCovers_RangedCovers_idx` ON `Cells_RangedCovers` (`RangedCoverId`);

CREATE INDEX `FK_Cells_Tags_Tags_idx` ON `Cells_Tags` (`TagId`);

CREATE INDEX `FK_ChannelCommandWords_Channels` ON `ChannelCommandWords` (`ChannelId`);

CREATE INDEX `FK_ChannelIgnorers_Accounts` ON `ChannelIgnorers` (`AccountId`);

CREATE INDEX `FK_Channels_FutureProgs_Listener` ON `Channels` (`ChannelListenerProgId`);

CREATE INDEX `FK_Channels_FutureProgs_Speaker` ON `Channels` (`ChannelSpeakerProgId`);

CREATE INDEX `FK_CharacterCombatSettings_FutureProgs_idx` ON `CharacterCombatSettings` (`AvailabilityProgId`);

CREATE INDEX `FK_CharacterCombatSettings_Characters_idx` ON `CharacterCombatSettings` (`CharacterOwnerId`);

CREATE INDEX `FK_CharacterIntroTemplates_FutureProgs_idx` ON `CharacterIntroTemplates` (`AppliesToCharacterProgId`);

CREATE INDEX `FK_CharacteristicDefinitions_Parent` ON `CharacteristicDefinitions` (`ParentId`);

CREATE INDEX `FK_CharacteristicProfiles_CharacteristicDefinitions` ON `CharacteristicProfiles` (`TargetDefinitionId`);

CREATE INDEX `FK_Characteristics_CharacteristicValues` ON `Characteristics` (`CharacteristicId`);

CREATE INDEX `FK_CharacteristicValues_CharacteristicDefinitions` ON `CharacteristicValues` (`DefinitionId`);

CREATE INDEX `FK_CharacteristicValues_FutureProgs` ON `CharacteristicValues` (`FutureProgId`);

CREATE INDEX `FK_CHARACTERKNOWLEDGES_CHARACTERS` ON `CharacterKnowledges` (`CharacterId`);

CREATE INDEX `FK_CHARACTERKNOWLEDGES_KNOWLEDGES_idx` ON `CharacterKnowledges` (`KnowledgeId`);

CREATE INDEX `FK_CharacterLog_Accounts_idx` ON `CharacterLog` (`AccountId`);

CREATE INDEX `FK_CharacterLog_Cells_idx` ON `CharacterLog` (`CellId`);

CREATE INDEX `FK_CharacterLog_Characters_idx` ON `CharacterLog` (`CharacterId`);

CREATE INDEX `FK_Characters_Accounts` ON `Characters` (`AccountId`);

CREATE INDEX `FK_Characters_Bodies` ON `Characters` (`BodyId`);

CREATE INDEX `FK_Characters_Chargens_idx` ON `Characters` (`ChargenId`);

CREATE INDEX `FK_Characters_Cultures` ON `Characters` (`CultureId`);

CREATE INDEX `FK_Characters_Currencies` ON `Characters` (`CurrencyId`);

CREATE INDEX `FK_Characters_Accents_idx` ON `Characters` (`CurrentAccentId`);

CREATE INDEX `FK_Characters_Languages_idx` ON `Characters` (`CurrentLanguageId`);

CREATE INDEX `FK_Characters_ActiveProjects_idx` ON `Characters` (`CurrentProjectId`);

CREATE INDEX `FK_Characters_ProjectLabourRequirements_idx` ON `Characters` (`CurrentProjectLabourId`);

CREATE INDEX `FK_Characters_Scripts_idx` ON `Characters` (`CurrentScriptId`);

CREATE INDEX `FK_Characters_Languages_Written_idx` ON `Characters` (`CurrentWritingLanguageId`);

CREATE INDEX `FK_Characters_Cells` ON `Characters` (`Location`);

CREATE INDEX `FK_Characters_Accents_Accents_idx` ON `Characters_Accents` (`AccentId`);

CREATE INDEX `FK_Characters_ChargenRoles_ChargenRoles` ON `Characters_ChargenRoles` (`ChargenRoleId`);

CREATE INDEX `FK_Characters_Languages_Languages_idx` ON `Characters_Languages` (`LanguageId`);

CREATE INDEX `FK_Characters_MagicResources_MagicResources_idx` ON `Characters_MagicResources` (`MagicResourceId`);

CREATE INDEX `FK_Characters_Scripts_Scripts_idx` ON `Characters_Scripts` (`ScriptId`);

CREATE INDEX `FK_ChargenAdvices_FutureProgs_idx` ON `ChargenAdvices` (`ShouldShowAdviceProgId`);

CREATE INDEX `FK_ChargenAdvices_ChargenRoles_ChargenRoles_idx` ON `ChargenAdvices_ChargenRoles` (`ChargenRoleId`);

CREATE INDEX `FK_ChargenAdvices_Cultures_Cultures_idx` ON `ChargenAdvices_Cultures` (`CultureId`);

CREATE INDEX `FK_ChargenAdvices_Ethnicities_Ethnicities_idx` ON `ChargenAdvices_Ethnicities` (`EthnicityId`);

CREATE INDEX `FK_ChargenAdvices_Races_Races_idx` ON `ChargenAdvices_Races` (`RaceId`);

CREATE INDEX `FK_ChargenRoles_FutureProgs` ON `ChargenRoles` (`AvailabilityProgId`);

CREATE INDEX `FK_ChargenRoles_Accounts` ON `ChargenRoles` (`PosterId`);

CREATE INDEX `FK_ChargenRoles_Approvers_Accounts` ON `ChargenRoles_Approvers` (`ApproverId`);

CREATE INDEX `FK_ChargenRoles_ClanMemberships_Clans` ON `ChargenRoles_ClanMemberships` (`ClanId`);

CREATE INDEX `FK_ChargenRoles_Costs_ChargenResources` ON `ChargenRoles_Costs` (`ChargenResourceId`);

CREATE INDEX `FK_ChargenRoles_Currencies_Currencies` ON `ChargenRoles_Currencies` (`CurrencyId`);

CREATE INDEX `FK_ChargenRoles_Merits_Merits_idx` ON `ChargenRoles_Merits` (`MeritId`);

CREATE INDEX `FK_ChargenRoles_Traits_Currencies` ON `ChargenRoles_Traits` (`TraitId`);

CREATE INDEX `FK_Chargens_Accounts` ON `Chargens` (`AccountId`);

CREATE INDEX `FK_Checks_CheckTemplates` ON `Checks` (`CheckTemplateId`);

CREATE INDEX `FK_Checks_TraitExpression` ON `Checks` (`TraitExpressionId`);

CREATE INDEX `FK_CheckTemplateDifficulties_CheckTemplates` ON `CheckTemplateDifficulties` (`CheckTemplateId`);

CREATE INDEX `FK_ClanMemberships_Characters` ON `ClanMemberships` (`CharacterId`);

CREATE INDEX `FK_ClanMemberships_Manager` ON `ClanMemberships` (`ManagerId`);

CREATE INDEX `FK_ClanMemberships_Appointments_Appointments` ON `ClanMemberships_Appointments` (`AppointmentId`);

CREATE INDEX `FK_ClanMemberships_Backpay_ClanMemberships` ON `ClanMemberships_Backpay` (`ClanId`, `CharacterId`);

CREATE INDEX `FK_Clans_Calendars` ON `Clans` (`CalendarId`);

CREATE INDEX `FK_Clans_FutureProgs_idx` ON `Clans` (`OnPayProgId`);

CREATE INDEX `FK_Clans_Parent` ON `Clans` (`ParentClanId`);

CREATE INDEX `FK_Clans_Characters_idx` ON `Clans` (`PaymasterId`);

CREATE INDEX `FK_Clans_AdministrationCells_Cells_idx` ON `Clans_AdministrationCells` (`CellId`);

CREATE INDEX `FK_Clans_TreasuryCells_Cells_idx` ON `Clans_TreasuryCells` (`CellId`);

CREATE INDEX `FK_Coins_Currencies` ON `Coins` (`CurrencyId`);

CREATE INDEX `FK_CombatMessages_FutureProgs_idx` ON `CombatMessages` (`ProgId`);

CREATE INDEX `FK_CombatMessages_WeaponAttacks_WeaponAttacks_idx` ON `CombatMessages_WeaponAttacks` (`WeaponAttackId`);

CREATE INDEX `FK_CraftInputs_Crafts_idx` ON `CraftInputs` (`CraftId`, `CraftRevisionNumber`);

CREATE INDEX `FK_CraftProducts_Crafts_idx` ON `CraftProducts` (`CraftId`, `CraftRevisionNumber`);

CREATE INDEX `FK_Crafts_FutureProgs_AppearInCraftsListProg_idx` ON `Crafts` (`AppearInCraftsListProgId`);

CREATE INDEX `FK_Crafts_FutureProgs_CanUseProg_idx` ON `Crafts` (`CanUseProgId`);

CREATE INDEX `FK_Crafts_TraitDefinitions_idx` ON `Crafts` (`CheckTraitId`);

CREATE INDEX `FK_Crafts_EditableItems_idx` ON `Crafts` (`EditableItemId`);

CREATE INDEX `FK_Crafts_FutureProgs_OnUseProgCancel_idx` ON `Crafts` (`OnUseProgCancelId`);

CREATE INDEX `FK_Crafts_FutureProgs_OnUseProgComplete_idx` ON `Crafts` (`OnUseProgCompleteId`);

CREATE INDEX `FK_Crafts_FutureProgs_OnUseProgStart_idx` ON `Crafts` (`OnUseProgStartId`);

CREATE INDEX `FK_Crafts_FutureProgs_WhyCannotUseProg_idx` ON `Crafts` (`WhyCannotUseProgId`);

CREATE INDEX `FK_CraftTools_Crafts_idx` ON `CraftTools` (`CraftId`, `CraftRevisionNumber`);

CREATE INDEX `FK_Crimes_Accuser_idx` ON `Crimes` (`AccuserId`);

CREATE INDEX `FK_Crimes_Criminal_idx` ON `Crimes` (`CriminalId`);

CREATE INDEX `FK_Crimes_Laws_idx` ON `Crimes` (`LawId`);

CREATE INDEX `FK_Crimes_Location_idx` ON `Crimes` (`LocationId`);

CREATE INDEX `FK_Crimes_Victim_idx` ON `Crimes` (`VictimId`);

CREATE INDEX `FK_Cultures_AvailabilityProg` ON `Cultures` (`AvailabilityProgId`);

CREATE INDEX `FK_Cultures_NameCulture` ON `Cultures` (`NameCultureId`);

CREATE INDEX `FK_Cultures_SkillStartingProg` ON `Cultures` (`SkillStartingValueProgId`);

CREATE INDEX `FK_Cultures_ChargenResources_ChargenResources` ON `Cultures_ChargenResources` (`ChargenResourceId`);

CREATE INDEX `FK_CDPE_CurrencyDescriptionPatterns` ON `CurrencyDescriptionPatternElements` (`CurrencyDescriptionPatternId`);

CREATE INDEX `FK_CDPE_CurrencyDivisions` ON `CurrencyDescriptionPatternElements` (`CurrencyDivisionId`);

CREATE INDEX `FK_CDPESV_CDPE` ON `CurrencyDescriptionPatternElementSpecialValues` (`CurrencyDescriptionPatternElementId`);

CREATE INDEX `FK_CurrencyDescriptionPatterns_Currencies` ON `CurrencyDescriptionPatterns` (`CurrencyId`);

CREATE INDEX `FK_CurrencyDescriptionPatterns_FutureProgs` ON `CurrencyDescriptionPatterns` (`FutureProgId`);

CREATE INDEX `FK_CurrencyDivisionAbbreviations_CurrencyDivisions` ON `CurrencyDivisionAbbreviations` (`CurrencyDivisionId`);

CREATE INDEX `FK_CurrencyDivisions_Currencies` ON `CurrencyDivisions` (`CurrencyId`);

CREATE INDEX `FK_DefaultHooks_Futureprogs_idx` ON `DefaultHooks` (`FutureProgId`);

CREATE INDEX `FK_DisfigurementTemplates_EditableItems_idx` ON `DisfigurementTemplates` (`EditableItemId`);

CREATE INDEX `FK_Doors_Locks` ON `Doors` (`LockedWith`);

CREATE INDEX `FK_Drawings_Characters_idx` ON `Drawings` (`AuthorId`);

CREATE INDEX `FK_Dreams_FutureProgs_CanDream_idx` ON `Dreams` (`CanDreamProgId`);

CREATE INDEX `FK_Dreams_FutureProgs_OnDream_idx` ON `Dreams` (`OnDreamProgId`);

CREATE INDEX `FK_Dreams_FutureProgs_OnWake_idx` ON `Dreams` (`OnWakeDuringDreamingProgId`);

CREATE INDEX `FK_Dreams_Dreamt_Characters_idx` ON `Dreams_Already_Dreamt` (`CharacterId`);

CREATE INDEX `FK_Dreams_Characters_Characters_idx` ON `Dreams_Characters` (`CharacterId`);

CREATE INDEX `FK_Dubs_Characters` ON `Dubs` (`CharacterId`);

CREATE INDEX `FK_EconomicZoneRevenues_FinancialPeriods_idx` ON `EconomicZoneRevenues` (`FinancialPeriodId`);

CREATE INDEX `FK_EconomicZones_Currencies_idx` ON `EconomicZones` (`CurrencyId`);

CREATE INDEX `FK_EconomicZones_FinancialPeriods_idx` ON `EconomicZones` (`CurrentFinancialPeriodId`);

CREATE INDEX `FK_EconomicZones_Calendars_idx` ON `EconomicZones` (`ReferenceCalendarId`);

CREATE INDEX `FK_EconomicZones_Timezones_idx` ON `EconomicZones` (`ReferenceClockId`);

CREATE INDEX `FK_EconomicZoneShopTaxes_Shops_idx` ON `EconomicZoneShopTaxes` (`ShopId`);

CREATE INDEX `FK_EnforcementAuthorities_LegalAuthorities_idx` ON `EnforcementAuthorities` (`LegalAuthorityId`);

CREATE INDEX `FK_EnforcementAuthorities_AccusableClasses_LegalClasses_idx` ON `EnforcementAuthorities_AccusableClasses` (`LegalClassId`);

CREATE INDEX `FK_EnforcementAuthorities_ArrestableClasses_LegalClasses_idx` ON `EnforcementAuthorities_ArrestableClasses` (`LegalClassId`);

CREATE INDEX `FK_EnforcementAuthorities_ParentAuthorities_Child_idx` ON `EnforcementAuthorities_ParentAuthorities` (`ChildId`);

CREATE INDEX `FK_EntityDescriptionPatterns_FutureProgs` ON `EntityDescriptionPatterns` (`ApplicabilityProgId`);

CREATE INDEX `FK_EDP_EntityDescriptions_EntityDescriptions` ON `EntityDescriptionPatterns_EntityDescriptions` (`EntityDescriptionId`);

CREATE INDEX `FK_Ethnicities_AvailabilityProg` ON `Ethnicities` (`AvailabilityProgId`);

CREATE INDEX `FK_Ethnicities_Races_idx` ON `Ethnicities` (`ParentRaceId`);

CREATE INDEX `FK_Ethnicities_PopulationBloodModels_idx` ON `Ethnicities` (`PopulationBloodModelId`);

CREATE INDEX `FK_Ethnicities_Characteristics_CharacteristicDefinitions` ON `Ethnicities_Characteristics` (`CharacteristicDefinitionId`);

CREATE INDEX `FK_Ethnicities_Characteristics_CharacteristicProfiles` ON `Ethnicities_Characteristics` (`CharacteristicProfileId`);

CREATE INDEX `FK_Ethnicities_ChargenResources_ChargenResources` ON `Ethnicities_ChargenResources` (`ChargenResourceId`);

CREATE INDEX `FK_ECC_Appointments_Controlled` ON `ExternalClanControls` (`ControlledAppointmentId`);

CREATE INDEX `FK_ECC_Appointments_Controlling` ON `ExternalClanControls` (`ControllingAppointmentId`);

CREATE INDEX `FK_ECC_Clans_Liege` ON `ExternalClanControls` (`LiegeClanId`);

CREATE INDEX `FK_ECC_Appointments_ClanMemberships` ON `ExternalClanControls_Appointments` (`VassalClanId`, `CharacterId`);

CREATE INDEX `FK_ECC_Appointments_ExternalClanControls` ON `ExternalClanControls_Appointments` (`VassalClanId`, `LiegeClanId`, `ControlledAppointmentId`);

CREATE INDEX `FK_FinancialPeriods_EconomicZones_idx` ON `FinancialPeriods` (`EconomicZoneId`);

CREATE INDEX `FK_ForagableProfiles_EditableItems_idx` ON `ForagableProfiles` (`EditableItemId`);

CREATE INDEX `FK_Foragables_EditableItems` ON `Foragables` (`EditableItemId`);

CREATE INDEX `FK_GameItemComponentProtos_EditableItems` ON `GameItemComponentProtos` (`EditableItemId`);

CREATE INDEX `FK_GameItemComponents_GameItems` ON `GameItemComponents` (`GameItemId`);

CREATE INDEX `FK_GameItemProtos_EditableItems` ON `GameItemProtos` (`EditableItemId`);

CREATE INDEX `FK_GameItemProtos_ItemGroups_idx` ON `GameItemProtos` (`ItemGroupId`);

CREATE INDEX `FK_GIPGICP_GameItemComponentProtos` ON `GameItemProtos_GameItemComponentProtos` (`GameItemComponentProtoId`, `GameItemComponentRevision`);

CREATE INDEX `FK_GIPGICP_GameItemProtos` ON `GameItemProtos_GameItemComponentProtos` (`GameItemProtoId`, `GameItemProtoRevision`);

CREATE INDEX `FK_GameItemProtos_OnLoadProgs_FutureProgs_idx` ON `GameItemProtos_OnLoadProgs` (`FutureProgId`);

CREATE INDEX `FK_GameItemProtos_Tags_Tags` ON `GameItemProtos_Tags` (`TagId`);

CREATE INDEX `FK_GameItemProtos_Tags_GameItemProtos` ON `GameItemProtos_Tags` (`GameItemProtoId`, `GameItemProtoRevisionNumber`);

CREATE INDEX `FK_GameItems_GameItems_Containers_idx` ON `GameItems` (`ContainerId`);

CREATE INDEX `FK_GameItems_MagicResources_MagicResources_idx` ON `GameItems_MagicResources` (`MagicResourceId`);

CREATE INDEX `FK_Gases_Gases_idx` ON `Gases` (`CountAsId`);

CREATE INDEX `FK_Gases_Liquids_idx` ON `Gases` (`PrecipitateId`);

CREATE INDEX `FK_Gases_Tags_Tags_idx` ON `Gases_Tags` (`TagId`);

CREATE INDEX `FK_GroupAIs_GroupAITemplates_idx` ON `GroupAIs` (`GroupAITemplateId`);

CREATE INDEX `FK_Helpfiles_FutureProgs` ON `Helpfiles` (`RuleId`);

CREATE INDEX `FK_Helpfiles_ExtraTexts_FutureProgs` ON `Helpfiles_ExtraTexts` (`RuleId`);

CREATE INDEX `FK_Hooks_Perceivables_Bodies_idx` ON `Hooks_Perceivables` (`BodyId`);

CREATE INDEX `FK_Hooks_Perceivables_Cells_idx` ON `Hooks_Perceivables` (`CellId`);

CREATE INDEX `FK_Hooks_Perceivables_Characters_idx` ON `Hooks_Perceivables` (`CharacterId`);

CREATE INDEX `FK_Hooks_Perceivables_GameItems_idx` ON `Hooks_Perceivables` (`GameItemId`);

CREATE INDEX `FK_Hooks_Perceivables_Hooks_idx` ON `Hooks_Perceivables` (`HookId`);

CREATE INDEX `FK_Hooks_Perceivables_Shards_idx` ON `Hooks_Perceivables` (`ShardId`);

CREATE INDEX `FK_Hooks_Perceivables_Zones_idx` ON `Hooks_Perceivables` (`ZoneId`);

CREATE INDEX `FK_Infections_Bodyparts_idx` ON `Infections` (`BodypartId`);

CREATE INDEX `FK_Infections_Bodies_idx` ON `Infections` (`OwnerId`);

CREATE INDEX `FK_Infections_Wounds_idx` ON `Infections` (`WoundId`);

CREATE INDEX `FK_ItemGroupForms_ItemGroups_idx` ON `ItemGroupForms` (`ItemGroupId`);

CREATE INDEX `FK_KNOWLEDGES_FUTUREPROGS_ACQUIRE_idx` ON `knowledges` (`CanAcquireProgId`);

CREATE INDEX `FK_KNOWLEDGES_FUTUREPROGS_LEARN_idx` ON `knowledges` (`CanLearnProgId`);

CREATE INDEX `FK_Languages_Accents_idx` ON `Languages` (`DefaultLearnerAccentId`);

CREATE INDEX `FK_Languages_LanguageDifficultyModels` ON `Languages` (`DifficultyModel`);

CREATE INDEX `FK_Languages_TraitDefinitions` ON `Languages` (`LinkedTraitId`);

CREATE INDEX `FK_Laws_FutureProgs_idx` ON `Laws` (`LawAppliesProgId`);

CREATE INDEX `FK_Laws_LegalAuthority_idx` ON `Laws` (`LegalAuthorityId`);

CREATE INDEX `FK_Laws_OffenderClasses_LegalClasses_idx` ON `Laws_OffenderClasses` (`LegalClassId`);

CREATE INDEX `FK_Laws_VictimClasses_LegalClasses_idx` ON `Laws_VictimClasses` (`LegalClassId`);

CREATE INDEX `FK_LegalAuthorities_Currencies_idx` ON `LegalAuthorities` (`CurrencyId`);

CREATE INDEX `FK_LegalAuthorities_Zones_LegalAuthorities_idx` ON `LegalAuthorities_Zones` (`LegalAuthorityId`);

CREATE INDEX `FK_LegalClasses_LegalAuthorities_idx` ON `LegalClasses` (`LegalAuthorityId`);

CREATE INDEX `FK_LegalClasses_FutureProgs_idx` ON `LegalClasses` (`MembershipProgId`);

CREATE INDEX `FK_Limbs_BodyProtos_idx` ON `Limbs` (`RootBodyId`);

CREATE INDEX `FK_Limbs_BodypartProto_idx` ON `Limbs` (`RootBodypartId`);

CREATE INDEX `FK_Limbs_BodypartProto_Limbs_idx` ON `Limbs_BodypartProto` (`LimbId`);

CREATE INDEX `FK_Limbs_SpinalParts_BodypartProtos_idx` ON `Limbs_SpinalParts` (`BodypartProtoId`);

CREATE INDEX `FK_Liquids_Liquids_CountasAs_idx` ON `Liquids` (`CountAsId`);

CREATE INDEX `FK_Liquids_Materials_idx` ON `Liquids` (`DriedResidueId`);

CREATE INDEX `FK_Liquids_Drugs_idx` ON `Liquids` (`DrugId`);

CREATE INDEX `FK_Liquids_Liquids_idx` ON `Liquids` (`SolventId`);

CREATE INDEX `FK_Liquids_Tags_Tags_idx` ON `Liquids_Tags` (`TagId`);

CREATE INDEX `FK_LoginIPs_Accounts` ON `LoginIPs` (`AccountId`);

CREATE INDEX `FK_MagicCapabilities_MagicSchools_idx` ON `MagicCapabilities` (`MagicSchoolId`);

CREATE INDEX `FK_MagicPowers_MagicSchools_idx` ON `MagicPowers` (`MagicSchoolId`);

CREATE INDEX `FK_MagicSchools_MagicSchools_idx` ON `MagicSchools` (`ParentSchoolId`);

CREATE INDEX `Materials_Tags_Tags_idx` ON `Materials_Tags` (`TagId`);

CREATE INDEX `FK_Merchandises_GameItems_idx` ON `Merchandises` (`PreferredDisplayContainerId`);

CREATE INDEX `FK_Merchandises_Shops_idx` ON `Merchandises` (`ShopId`);

CREATE INDEX `FK_Merits_Merits_idx` ON `Merits` (`ParentId`);

CREATE INDEX `FK_Merits_ChargenResources_ChargenResources_idx` ON `Merits_ChargenResources` (`ChargenResourceId`);

CREATE INDEX `FK_MoveSpeeds_BodyPrototype` ON `MoveSpeeds` (`BodyProtoId`);

CREATE INDEX `FK_Languages_MutualIntelligabilities_Target_idx` ON `MutualIntelligabilities` (`TargetLanguageId`);

CREATE INDEX `FK_NPCs_Characters_Bodyguard_idx` ON `NPCs` (`BodyguardCharacterId`);

CREATE INDEX `FK_NPCs_Characters` ON `NPCs` (`CharacterId`);

CREATE INDEX `FK_NPCs_NPCTemplates` ON `NPCs` (`TemplateId`, `TemplateRevnum`);

CREATE INDEX `FK_NPCs_ArtificialIntelligences_NPCs` ON `NPCs_ArtificialIntelligences` (`NPCId`);

CREATE INDEX `FK_NPCTemplates_EditableItems` ON `NPCTemplates` (`EditableItemId`);

CREATE INDEX `FK_NTAI_ArtificalIntelligences` ON `NPCTemplates_ArtificalIntelligences` (`AIId`);

CREATE INDEX `FK_NTAI_NPCTemplates` ON `NPCTemplates_ArtificalIntelligences` (`NPCTemplateId`, `NPCTemplateRevisionNumber`);

CREATE INDEX `FK_Paygrades_Clans` ON `Paygrades` (`ClanId`);

CREATE INDEX `FK_Paygrades_Currencies` ON `Paygrades` (`CurrencyId`);

CREATE INDEX `FK_PerceiverMerits_Bodies_idx` ON `PerceiverMerits` (`BodyId`);

CREATE INDEX `FK_PerceiverMerits_Characters_idx` ON `PerceiverMerits` (`CharacterId`);

CREATE INDEX `FK_PerceiverMerits_GameItems_idx` ON `PerceiverMerits` (`GameItemId`);

CREATE INDEX ` FK_PerceiverMerits_Merits_idx` ON `PerceiverMerits` (`MeritId`);

CREATE INDEX `FK_PopulationBloodModels_Bloodtypes_PopulationBloodModels_idx` ON `PopulationBloodModels_Bloodtypes` (`PopulationBloodModelId`);

CREATE INDEX `FK_ProgSchedules_FutureProgs_idx` ON `ProgSchedules` (`FutureProgId`);

CREATE INDEX `FK_ProjectActions_ProjectPhases_idx` ON `ProjectActions` (`ProjectPhaseId`);

CREATE INDEX `FK_ProjectLabourImpacts_ProjectLabourRequirements_idx` ON `ProjectLabourImpacts` (`ProjectLabourRequirementId`);

CREATE INDEX `FK_ProjectLabourRequirements_ProjectPhases_idx` ON `ProjectLabourRequirements` (`ProjectPhaseId`);

CREATE INDEX `FK_ProjectMaterialRequirements_ProjectPhases_idx` ON `ProjectMaterialRequirements` (`ProjectPhaseId`);

CREATE INDEX `FK_ProjectPhases_Projects_idx` ON `ProjectPhases` (`ProjectId`, `ProjectRevisionNumber`);

CREATE INDEX `FK_Projects_EditableItems_idx` ON `Projects` (`EditableItemId`);

CREATE INDEX `FK_RaceButcheryProfiles_FutureProgs_Can_idx` ON `RaceButcheryProfiles` (`CanButcherProgId`);

CREATE INDEX `FK_RaceButcheryProfiles_Tags_idx` ON `RaceButcheryProfiles` (`RequiredToolTagId`);

CREATE INDEX `FK_RaceButcheryProfiles_FutureProgs_Why_idx` ON `RaceButcheryProfiles` (`WhyCannotButcherProgId`);

CREATE INDEX `FK_RaceButcheryProfiles_BreakdownChecks_TraitDefinitions_idx` ON `RaceButcheryProfiles_BreakdownChecks` (`TraitDefinitionId`);

CREATE INDEX `FK_RaceButcheryProfiles_ButcheryProducts_ButcheryProducts_idx` ON `RaceButcheryProfiles_ButcheryProducts` (`ButcheryProductId`);

CREATE INDEX `FK_Races_AttributeBonusProg` ON `Races` (`AttributeBonusProgId`);

CREATE INDEX `FK_Races_AvailabilityProg` ON `Races` (`AvailabilityProgId`);

CREATE INDEX `FK_Races_BodyProtos` ON `Races` (`BaseBodyId`);

CREATE INDEX `FK_Races_Liquids_Blood_idx` ON `Races` (`BloodLiquidId`);

CREATE INDEX `FK_Races_BloodModels_idx` ON `Races` (`BloodModelId`);

CREATE INDEX `FK_Races_CorpseModels_idx` ON `Races` (`CorpseModelId`);

CREATE INDEX `FK_Races_HealthStrategies_idx` ON `Races` (`DefaultHealthStrategyId`);

CREATE INDEX `FK_Races_Materials_idx` ON `Races` (`NaturalArmourMaterialId`);

CREATE INDEX `FK_Races_ArmourTypes_idx` ON `Races` (`NaturalArmourTypeId`);

CREATE INDEX `FK_Races_Races` ON `Races` (`ParentRaceId`);

CREATE INDEX `FK_Races_RaceButcheryProfiles_idx` ON `Races` (`RaceButcheryProfileId`);

CREATE INDEX `FK_Races_Liqiuds_Sweat_idx` ON `Races` (`SweatLiquidId`);

CREATE INDEX `FK_Races_AdditionalBodyparts_BodypartProto` ON `Races_AdditionalBodyparts` (`BodypartId`);

CREATE INDEX `FK_Races_AdditionalBodyparts_Races` ON `Races_AdditionalBodyparts` (`RaceId`);

CREATE INDEX `FK_RAC_CharacteristicDefinitions` ON `Races_AdditionalCharacteristics` (`CharacteristicDefinitionId`);

CREATE INDEX `FK_Races_Attributes_TraitDefinitions` ON `Races_Attributes` (`AttributeId`);

CREATE INDEX `FK_Races-BreathableGases_Gases_idx` ON `Races_BreathableGases` (`GasId`);

CREATE INDEX `FK_Races_BreathableLiquids_Liquids_idx` ON `Races_BreathableLiquids` (`LiquidId`);

CREATE INDEX `FK_Races_ChargenResources_ChargenResources` ON `Races_ChargenResources` (`ChargenResourceId`);

CREATE INDEX `FK_Races_EdibleMaterials_Materials_idx` ON `Races_EdibleMaterials` (`MaterialId`);

CREATE INDEX `FK_Races_WeaponAttacks_BodypartProto_idx` ON `Races_WeaponAttacks` (`BodypartId`);

CREATE INDEX `FK_Races_WeaponAttacks_WeaponAttacks_idx` ON `Races_WeaponAttacks` (`WeaponAttackId`);

CREATE INDEX `FK_RandomNameProfiles_NameCulture` ON `RandomNameProfiles` (`NameCultureId`);

CREATE INDEX `FK_RangedWeaponTypes_TraitDefinitions_Fire_idx` ON `RangedWeaponTypes` (`FireTraitId`);

CREATE INDEX `FK_RangedWeaponTypes_TraitDefinitions_Operate_idx` ON `RangedWeaponTypes` (`OperateTraitId`);

CREATE INDEX `FK_Ranks_Clans` ON `Ranks` (`ClanId`);

CREATE INDEX `FK_Ranks_GameItemProtos` ON `Ranks` (`InsigniaGameItemId`, `InsigniaGameItemRevnum`);

CREATE INDEX `FK_Ranks_Abbreviations_FutureProgs` ON `Ranks_Abbreviations` (`FutureProgId`);

CREATE INDEX `FK_Ranks_Paygrades_Paygrades` ON `Ranks_Paygrades` (`PaygradeId`);

CREATE INDEX `FK_Ranks_Titles_FutureProgs` ON `Ranks_Titles` (`FutureProgId`);

CREATE INDEX `FK_RegionalClimates_Seasons_Seasons_idx` ON `RegionalClimates_Seasons` (`SeasonId`);

CREATE INDEX `FK_Rooms_Zones` ON `Rooms` (`ZoneId`);

CREATE INDEX `FK_Scripts_Knowledges_idx` ON `Scripts` (`KnowledgeId`);

CREATE INDEX `FK_Scripts_DesignedLanguages_Languages_idx` ON `Scripts_DesignedLanguages` (`LanguageId`);

CREATE INDEX `FK_Seasons_Celestials_idx` ON `Seasons` (`CelestialId`);

CREATE INDEX `FK_Shards_SkyDescriptionTemplates` ON `Shards` (`SkyDescriptionTemplateId`);

CREATE INDEX `FK_ShieldTypes_TraitDefinitions_idx` ON `ShieldTypes` (`BlockTraitId`);

CREATE INDEX `FK_ShieldTypes_ArmourTypes_idx` ON `ShieldTypes` (`EffectiveArmourTypeId`);

CREATE INDEX `FK_ShopFinancialPeriodResults_FinancialPeriods_idx` ON `ShopFinancialPeriodResults` (`FinancialPeriodId`);

CREATE INDEX `FK_ShopFinancialPeriodResults_Shops_idx` ON `ShopFinancialPeriodResults` (`ShopId`);

CREATE INDEX `FK_Shops_FutureProgs_Can_idx` ON `Shops` (`CanShopProgId`);

CREATE INDEX `FK_Shops_Currencies_idx` ON `Shops` (`CurrencyId`);

CREATE INDEX `FK_Shops_EconomicZonesa_idx` ON `Shops` (`EconomicZoneId`);

CREATE INDEX `FK_Shops_Cells_Stockroom_idx` ON `Shops` (`StockroomCellId`);

CREATE INDEX `FK_Shops_FutureProgs_WhyCant_idx` ON `Shops` (`WhyCannotShopProgId`);

CREATE INDEX `FK_Shops_Cells_Workshop_idx` ON `Shops` (`WorkshopCellId`);

CREATE INDEX `FK_Shops_StoreroomCells_Cells_idx` ON `Shops_StoreroomCells` (`CellId`);

CREATE INDEX `FK_ShopTills_GameItems_idx` ON `ShopsTills` (`GameItemId`);

CREATE INDEX `FK_ShopTransactionRecords_Currencies_idx` ON `ShopTransactionRecords` (`CurrencyId`);

CREATE INDEX `FK_ShopTransactionRecords_Shops_idx` ON `ShopTransactionRecords` (`ShopId`);

CREATE INDEX `FK_Socials_FutureProgs` ON `Socials` (`FutureProgId`);

CREATE INDEX `FK_SurgicalProcedurePhases_FutureProgs_OnPhaseProg_idx` ON `SurgicalProcedurePhases` (`OnPhaseProgId`);

CREATE INDEX `FK_SurgicalProcedures_FutureProgs_AbortProg_idx` ON `SurgicalProcedures` (`AbortProgId`);

CREATE INDEX `FK_SurgicalProcedures_FutureProgs_CompletionProg_idx` ON `SurgicalProcedures` (`CompletionProgId`);

CREATE INDEX `FK_SurgicalProcedures_Knowledges_idx` ON `SurgicalProcedures` (`KnowledgeRequiredId`);

CREATE INDEX `FK_SurgicalProcedures_FutureProgs_Usability_idx` ON `SurgicalProcedures` (`UsabilityProgId`);

CREATE INDEX `FK_SurgicalProcedures_FutureProgs_WhyCannotUseProg_idx` ON `SurgicalProcedures` (`WhyCannotUseProgId`);

CREATE INDEX `FK_Tags_Parent_idx` ON `Tags` (`ParentId`);

CREATE INDEX `FK_Tags_Futureprogs_idx` ON `Tags` (`ShouldSeeProgId`);

CREATE INDEX `FK_Terrains_WeatherControllers_idx` ON `Terrains` (`WeatherControllerId`);

CREATE INDEX `FK_Terrains_RangedCovers_RangedCovers_idx` ON `Terrains_RangedCovers` (`RangedCoverId`);

CREATE INDEX `FK_Timezones_Clocks` ON `Timezones` (`ClockId`);

CREATE INDEX `FK_TraitDefinitions_AvailabilityProg` ON `TraitDefinitions` (`AvailabilityProgId`);

CREATE INDEX `FK_TraitDefinitions_TraitExpression` ON `TraitDefinitions` (`ExpressionId`);

CREATE INDEX `FK_TraitDefinitions_LearnableProg_idx` ON `TraitDefinitions` (`LearnableProgId`);

CREATE INDEX `FK_TraitDefinitions_TeachableProg_idx` ON `TraitDefinitions` (`TeachableProgId`);

CREATE INDEX `FK_TraitDefinitions_ChargenResources_ChargenResources` ON `TraitDefinitions_ChargenResources` (`ChargenResourceId`);

CREATE INDEX `FK_TraitExpressionParameters_TraitDefinitions` ON `TraitExpressionParameters` (`TraitDefinitionId`);

CREATE INDEX `FK_TraitExpressionParameters_TraitExpression` ON `TraitExpressionParameters` (`TraitExpressionId`);

CREATE INDEX `FK_Traits_TraitDefinitions` ON `Traits` (`TraitDefinitionId`);

CREATE INDEX `FK_WeaponAttacks_TraitExpression_Damage_idx` ON `WeaponAttacks` (`DamageExpressionId`);

CREATE INDEX `FK_WeaponAttacks_FutureProgs_idx` ON `WeaponAttacks` (`FutureProgId`);

CREATE INDEX `FK_WeaponAttacks_TraitExpression_Pain_idx` ON `WeaponAttacks` (`PainExpressionId`);

CREATE INDEX `FK_WeaponAttacks_TraitExpression_Stun_idx` ON `WeaponAttacks` (`StunExpressionId`);

CREATE INDEX `FK_WeaponAttacks_WeaponTypes_idx` ON `WeaponAttacks` (`WeaponTypeId`);

CREATE INDEX `FK_WeaponTypes_TraitDefinitions_Attack_idx` ON `WeaponTypes` (`AttackTraitId`);

CREATE INDEX `FK_WeaponTypes_TraitDefinitions_Parry_idx` ON `WeaponTypes` (`ParryTraitId`);

CREATE INDEX `FK_WearableSizeParameterRule_TraitDefinitions` ON `WearableSizeParameterRule` (`TraitId`);

CREATE INDEX `FK_WeatherControllers_Celestials_idx` ON `WeatherControllers` (`CelestialId`);

CREATE INDEX `FK_WeatherControllers_Seasons_idx` ON `WeatherControllers` (`CurrentSeasonId`);

CREATE INDEX `FK_WeatherControllers_WeatherEvents_idx` ON `WeatherControllers` (`CurrentWeatherEventId`);

CREATE INDEX `FK_WeatherControllers_Clocks_idx` ON `WeatherControllers` (`FeedClockId`);

CREATE INDEX `FK_WeatherControllers_TimeZones_idx` ON `WeatherControllers` (`FeedClockTimeZoneId`);

CREATE INDEX `FK_WeatherControllers_RegionalClimates_idx` ON `WeatherControllers` (`RegionalClimateId`);

CREATE INDEX `FK_WeatherEvents_WeatherEvents_idx` ON `WeatherEvents` (`CountsAsId`);

CREATE INDEX `FK_WitnessProfiles_IdentityProg_idx` ON `WitnessProfiles` (`IdentityKnownProgId`);

CREATE INDEX `FK_WitnessProfiles_MultiplierProg_idx` ON `WitnessProfiles` (`ReportingMultiplierProgId`);

CREATE INDEX `FK_WitnessProfiles_CooperatingAuthorities_LegalAuthorities_idx` ON `WitnessProfiles_CooperatingAuthorities` (`LegalAuthorityId`);

CREATE INDEX `FK_WitnessProfiles_IgnoredCriminalClasses_LegalClasses_idx` ON `WitnessProfiles_IgnoredCriminalClasses` (`LegalClassId`);

CREATE INDEX `FK_WitnessProfiles_IgnoredVictimClasses_LegalClasses_idx` ON `WitnessProfiles_IgnoredVictimClasses` (`LegalClassId`);

CREATE INDEX `FK_Wounds_Characters_idx` ON `Wounds` (`ActorOriginId`);

CREATE INDEX `FK_Wounds_Bodies_idx` ON `Wounds` (`BodyId`);

CREATE INDEX `FK_Wounds_GameItemOwner_idx` ON `Wounds` (`GameItemId`);

CREATE INDEX `FK_Wounds_GameItems_idx` ON `Wounds` (`LodgedItemId`);

CREATE INDEX `FK_Wounds_GameItems_Tool_idx` ON `Wounds` (`ToolOriginId`);

CREATE INDEX `FK_Writings_Characters_Author_idx` ON `Writings` (`AuthorId`);

CREATE INDEX `FK_Writings_Languages_idx` ON `Writings` (`LanguageId`);

CREATE INDEX `FK_Writings_Scripts_idx` ON `Writings` (`ScriptId`);

CREATE INDEX `FK_Writings_Characters_TrueAuthor_idx` ON `Writings` (`TrueAuthorId`);

CREATE INDEX `FK_Zones_Cells` ON `Zones` (`DefaultCellId`);

CREATE INDEX `FK_Zones_Shards` ON `Zones` (`ShardId`);

CREATE INDEX `FK_Zones_WeatherControllers_idx` ON `Zones` (`WeatherControllerId`);

ALTER TABLE `Characters` ADD CONSTRAINT `FK_Characters_Languages_Spoken` FOREIGN KEY (`CurrentLanguageId`) REFERENCES `Languages` (`Id`) ON DELETE SET NULL;

ALTER TABLE `Characters` ADD CONSTRAINT `FK_Characters_Languages_Written` FOREIGN KEY (`CurrentWritingLanguageId`) REFERENCES `Languages` (`Id`) ON DELETE SET NULL;

ALTER TABLE `Characters` ADD CONSTRAINT `FK_Characters_ActiveProjects` FOREIGN KEY (`CurrentProjectId`) REFERENCES `ActiveProjects` (`Id`) ON DELETE SET NULL;

ALTER TABLE `Characters` ADD CONSTRAINT `FK_Characters_Cells` FOREIGN KEY (`Location`) REFERENCES `Cells` (`Id`) ON DELETE RESTRICT;

ALTER TABLE `Characters` ADD CONSTRAINT `FK_Characters_Bodies` FOREIGN KEY (`BodyId`) REFERENCES `Bodies` (`Id`) ON DELETE CASCADE;

ALTER TABLE `Characters` ADD CONSTRAINT `FK_Characters_Accents` FOREIGN KEY (`CurrentAccentId`) REFERENCES `Accents` (`Id`) ON DELETE SET NULL;

ALTER TABLE `Characters_Accents` ADD CONSTRAINT `FK_Characters_Accents_Accents` FOREIGN KEY (`AccentId`) REFERENCES `Accents` (`Id`) ON DELETE CASCADE;

ALTER TABLE `Languages` ADD CONSTRAINT `FK_Languages_Accents` FOREIGN KEY (`DefaultLearnerAccentId`) REFERENCES `Accents` (`Id`) ON DELETE RESTRICT;

ALTER TABLE `CharacterLog` ADD CONSTRAINT `FK_CharacterLog_Cells` FOREIGN KEY (`CellId`) REFERENCES `Cells` (`Id`) ON DELETE CASCADE;

ALTER TABLE `ActiveProjectLabours` ADD CONSTRAINT `FK_ActiveProjectLabours_ActiveProjects` FOREIGN KEY (`ActiveProjectId`) REFERENCES `ActiveProjects` (`Id`) ON DELETE CASCADE;

ALTER TABLE `ActiveProjectMaterials` ADD CONSTRAINT `FK_ActiveProjectMaterials_ActiveProjects` FOREIGN KEY (`ActiveProjectId`) REFERENCES `ActiveProjects` (`Id`) ON DELETE CASCADE;

ALTER TABLE `Areas_Rooms` ADD CONSTRAINT `FK_Areas_Rooms_Rooms` FOREIGN KEY (`RoomId`) REFERENCES `Rooms` (`Id`) ON DELETE CASCADE;

ALTER TABLE `BodypartProto` ADD CONSTRAINT `FK_BodypartProto_BodyPrototype` FOREIGN KEY (`BodyId`) REFERENCES `BodyProtos` (`Id`) ON DELETE RESTRICT;

ALTER TABLE `Hooks_Perceivables` ADD CONSTRAINT `FK_Hooks_Perceivables_Cells` FOREIGN KEY (`CellId`) REFERENCES `Cells` (`Id`) ON DELETE CASCADE;

ALTER TABLE `Hooks_Perceivables` ADD CONSTRAINT `FK_Hooks_Perceivables_Zones` FOREIGN KEY (`ZoneId`) REFERENCES `Zones` (`Id`) ON DELETE CASCADE;

ALTER TABLE `EconomicZones` ADD CONSTRAINT `FK_EconomicZones_FinancialPeriods` FOREIGN KEY (`CurrentFinancialPeriodId`) REFERENCES `FinancialPeriods` (`Id`) ON DELETE SET NULL;

ALTER TABLE `CellOverlays` ADD CONSTRAINT `FK_CellOverlays_Cells` FOREIGN KEY (`CellId`) REFERENCES `Cells` (`Id`) ON DELETE RESTRICT;

ALTER TABLE `Cells` ADD CONSTRAINT `FK_Cells_Rooms` FOREIGN KEY (`RoomId`) REFERENCES `Rooms` (`Id`) ON DELETE RESTRICT;

INSERT INTO `__EFMigrationsHistory` (`MigrationId`, `ProductVersion`)
VALUES ('20200626070704_InitialDatabase', '9.0.11');

CREATE TABLE `ChargenScreenStoryboards` (
    `Id` bigint(20) NOT NULL AUTO_INCREMENT,
    `ChargenType` varchar(50) NULL,
    `ChargenStage` int(11) NOT NULL,
    `Order` int(11) NOT NULL,
    `StageDefinition` mediumtext CHARACTER SET utf8 NOT NULL,
    `NextStage` int(11) NOT NULL,
    CONSTRAINT `PK_ChargenScreenStoryboards` PRIMARY KEY (`Id`)
);

CREATE TABLE `ChargenScreenStoryboardDependentStages` (
    `OwnerId` bigint(20) NOT NULL,
    `Dependency` int(11) NOT NULL,
    CONSTRAINT `PRIMARY` PRIMARY KEY (`OwnerId`, `Dependency`),
    CONSTRAINT `FK_ChargenScreenStoryboardDependentStages_Owner` FOREIGN KEY (`OwnerId`) REFERENCES `ChargenScreenStoryboards` (`Id`) ON DELETE CASCADE
);

CREATE INDEX `FK_ChargenScreenStoryboardDependentStages_Owner` ON `ChargenScreenStoryboardDependentStages` (`OwnerId`);

INSERT INTO `__EFMigrationsHistory` (`MigrationId`, `ProductVersion`)
VALUES ('20200728125151_MoveChargenToTables', '9.0.11');

ALTER TABLE `EnforcementAuthorities` ADD `FilterProgId` bigint(20) NULL;

CREATE TABLE `PatrolRoutes` (
    `Id` bigint(20) NOT NULL AUTO_INCREMENT,
    `Name` varchar(100) NULL,
    `LegalAuthorityId` bigint(20) NOT NULL,
    `LingerTimeMajorNode` DOUBLE NOT NULL,
    `LingerTimeMinorNode` DOUBLE NOT NULL,
    `Priority` int(11) NOT NULL,
    `PatrolStrategy` varchar(100) NULL,
    CONSTRAINT `PK_PatrolRoutes` PRIMARY KEY (`Id`),
    CONSTRAINT `FK_PatrolRoutes_LegalAuthorities` FOREIGN KEY (`LegalAuthorityId`) REFERENCES `LegalAuthorities` (`Id`) ON DELETE CASCADE
);

CREATE TABLE `PatrolRoutesNodes` (
    `PatrolRouteId` bigint(20) NOT NULL,
    `CellId` bigint(20) NOT NULL,
    `Order` int(11) NOT NULL,
    CONSTRAINT `PRIMARY` PRIMARY KEY (`PatrolRouteId`, `CellId`),
    CONSTRAINT `FK_PatrolRoutesNodes_Cells` FOREIGN KEY (`CellId`) REFERENCES `Cells` (`Id`) ON DELETE CASCADE,
    CONSTRAINT `FK_PatrolRoutesNodes_PatrolRoutes` FOREIGN KEY (`PatrolRouteId`) REFERENCES `PatrolRoutes` (`Id`) ON DELETE CASCADE
);

CREATE TABLE `PatrolRoutesNumbers` (
    `PatrolRouteId` bigint(20) NOT NULL,
    `EnforcementAuthorityId` bigint(20) NOT NULL,
    `NumberRequired` int(11) NOT NULL,
    CONSTRAINT `PRIMARY` PRIMARY KEY (`PatrolRouteId`, `EnforcementAuthorityId`),
    CONSTRAINT `FK_PatrolRoutesNumbers_EnforcementAuthorities` FOREIGN KEY (`EnforcementAuthorityId`) REFERENCES `EnforcementAuthorities` (`Id`) ON DELETE CASCADE,
    CONSTRAINT `FK_PatrolRoutesNumbers_PatrolRoutes` FOREIGN KEY (`PatrolRouteId`) REFERENCES `PatrolRoutes` (`Id`) ON DELETE CASCADE
);

CREATE TABLE `PatrolRoutesTimesOfDay` (
    `PatrolRouteId` bigint(20) NOT NULL,
    `TimeOfDay` int(11) NOT NULL,
    CONSTRAINT `PRIMARY` PRIMARY KEY (`PatrolRouteId`, `TimeOfDay`),
    CONSTRAINT `FK_PatrolRoutesTimesOfDay_PatrolRoutes` FOREIGN KEY (`PatrolRouteId`) REFERENCES `PatrolRoutes` (`Id`) ON DELETE CASCADE
);

CREATE INDEX `FK_EnforcementAuthorities_FutureProgs_idx` ON `EnforcementAuthorities` (`FilterProgId`);

CREATE INDEX `FK_PatrolRoutes_LegalAuthorities_idx` ON `PatrolRoutes` (`LegalAuthorityId`);

CREATE INDEX `FK_PatrolRoutesNodes_Cells_idx` ON `PatrolRoutesNodes` (`CellId`);

CREATE INDEX `FK_PatrolRoutesNodes_PatrolRoutes_idx` ON `PatrolRoutesNodes` (`PatrolRouteId`);

CREATE INDEX `FK_PatrolRoutesNumbers_EnforcementAuthorities_idx` ON `PatrolRoutesNumbers` (`EnforcementAuthorityId`);

CREATE INDEX `FK_PatrolRoutesNumbers_PatrolRoutes_idx` ON `PatrolRoutesNumbers` (`PatrolRouteId`);

CREATE INDEX `FK_PatrolRoutesTimesOfDay_PatrolRoutes_idx` ON `PatrolRoutesTimesOfDay` (`PatrolRouteId`);

ALTER TABLE `EnforcementAuthorities` ADD CONSTRAINT `FK_EnforcementAuthorities_FutureProgs` FOREIGN KEY (`FilterProgId`) REFERENCES `FutureProgs` (`Id`) ON DELETE RESTRICT;

INSERT INTO `__EFMigrationsHistory` (`MigrationId`, `ProductVersion`)
VALUES ('20200807044450_EnforcementUpdate', '9.0.11');

ALTER TABLE `Crimes` ADD `ThirdPartyIItemType` varchar(100) NULL;

ALTER TABLE `Crimes` ADD `ThirdPartyId` bigint(20) NULL;

ALTER TABLE `Appointments` ADD `CanNominateProgId` bigint(20) NULL;

ALTER TABLE `Appointments` ADD `ElectionLeadTimeMinutes` double NULL;

ALTER TABLE `Appointments` ADD `ElectionTermMinutes` double NULL;

ALTER TABLE `Appointments` ADD `IsAppointedByElection` bit(1) NOT NULL DEFAULT 0;

ALTER TABLE `Appointments` ADD `IsSecretBallot` bit(1) NULL;

ALTER TABLE `Appointments` ADD `MaximumConsecutiveTerms` int(11) NULL;

ALTER TABLE `Appointments` ADD `MaximumTotalTerms` int(11) NULL;

ALTER TABLE `Appointments` ADD `NominationPeriodMinutes` double NULL;

ALTER TABLE `Appointments` ADD `NumberOfVotesProgId` bigint(20) NULL;

ALTER TABLE `Appointments` ADD `VotingPeriodMinutes` double NULL;

CREATE INDEX `FK_Appointments_CanNominateProg_idx` ON `Appointments` (`CanNominateProgId`);

CREATE INDEX `FK_Appointments_NumberOfVotesProg_idx` ON `Appointments` (`NumberOfVotesProgId`);

ALTER TABLE `Appointments` ADD CONSTRAINT `FK_Appointments_CanNominateProg` FOREIGN KEY (`CanNominateProgId`) REFERENCES `FutureProgs` (`Id`) ON DELETE RESTRICT;

ALTER TABLE `Appointments` ADD CONSTRAINT `FK_Appointments_NumberOfVotesProg` FOREIGN KEY (`NumberOfVotesProgId`) REFERENCES `FutureProgs` (`Id`) ON DELETE RESTRICT;

INSERT INTO `__EFMigrationsHistory` (`MigrationId`, `ProductVersion`)
VALUES ('20200810141606_ClanVoting', '9.0.11');

ALTER TABLE `ClanMemberships` ADD `ArchivedMembership` bit(1) NOT NULL DEFAULT 0;

ALTER TABLE `Appointments` MODIFY COLUMN `CanNominateProgId` bigint(20) NULL;

ALTER TABLE `Appointments` ADD `WhyCantNominateProgId` bigint(20) NULL;

CREATE TABLE `Elections` (
    `Id` bigint(20) NOT NULL AUTO_INCREMENT,
    `AppointmentId` bigint(20) NOT NULL,
    `NominationStartDate` varchar(100) NULL,
    `VotingStartDate` varchar(100) NULL,
    `VotingEndDate` varchar(100) NULL,
    `ResultsInEffectDate` varchar(100) NULL,
    `IsFinalised` bit(1) NOT NULL,
    `NumberOfAppointments` int(11) NOT NULL,
    `IsByElection` bit(1) NOT NULL,
    `ElectionStage` int(11) NOT NULL,
    CONSTRAINT `PK_Elections` PRIMARY KEY (`Id`),
    CONSTRAINT `FK_Elections_Appointments` FOREIGN KEY (`AppointmentId`) REFERENCES `Appointments` (`Id`) ON DELETE CASCADE
);

CREATE TABLE `ElectionsNominees` (
    `ElectionId` bigint(20) NOT NULL,
    `NomineeId` bigint(20) NOT NULL,
    `NomineeClanId` bigint(20) NOT NULL,
    CONSTRAINT `PRIMARY` PRIMARY KEY (`ElectionId`, `NomineeId`),
    CONSTRAINT `FK_ElectionsNominees_Elections` FOREIGN KEY (`ElectionId`) REFERENCES `Elections` (`Id`) ON DELETE CASCADE,
    CONSTRAINT `FK_ElectionsNominees_ClanMemberships` FOREIGN KEY (`NomineeClanId`, `NomineeId`) REFERENCES `ClanMemberships` (`ClanId`, `CharacterId`) ON DELETE CASCADE
);

CREATE TABLE `ElectionsVotes` (
    `ElectionId` bigint(20) NOT NULL,
    `VoterId` bigint(20) NOT NULL,
    `NomineeId` bigint(20) NOT NULL,
    `VoterClanId` bigint(20) NOT NULL,
    `NomineeClanId` bigint(20) NOT NULL,
    `NumberOfVotes` int(11) NOT NULL,
    CONSTRAINT `PRIMARY` PRIMARY KEY (`ElectionId`, `NomineeId`, `VoterId`),
    CONSTRAINT `FK_ElectionsVotes_Elections` FOREIGN KEY (`ElectionId`) REFERENCES `Elections` (`Id`) ON DELETE CASCADE,
    CONSTRAINT `FK_ElectionsVotes_Nominees` FOREIGN KEY (`NomineeClanId`, `NomineeId`) REFERENCES `ClanMemberships` (`ClanId`, `CharacterId`) ON DELETE CASCADE,
    CONSTRAINT `FK_ElectionsVotes_Voters` FOREIGN KEY (`VoterClanId`, `VoterId`) REFERENCES `ClanMemberships` (`ClanId`, `CharacterId`) ON DELETE CASCADE
);

CREATE INDEX `FK_Appointments_WhyCantNominateProg_idx` ON `Appointments` (`WhyCantNominateProgId`);

CREATE INDEX `FK_Elections_Appointments_idx` ON `Elections` (`AppointmentId`);

CREATE INDEX `FK_ElectionsNominees_Elections_idx` ON `ElectionsNominees` (`ElectionId`);

CREATE INDEX `FK_ElectionsNominees_ClanMemberships_idx` ON `ElectionsNominees` (`NomineeClanId`, `NomineeId`);

CREATE INDEX `FK_ElectionsVotes_Elections_idx` ON `ElectionsVotes` (`ElectionId`);

CREATE INDEX `FK_ElectionsVotes_Nominees_idx` ON `ElectionsVotes` (`NomineeClanId`, `NomineeId`);

CREATE INDEX `FK_ElectionsVotes_Voters_idx` ON `ElectionsVotes` (`VoterClanId`, `VoterId`);

ALTER TABLE `Appointments` ADD CONSTRAINT `FK_Appointments_WhyCantNominateProg` FOREIGN KEY (`WhyCantNominateProgId`) REFERENCES `FutureProgs` (`Id`) ON DELETE RESTRICT;

INSERT INTO `__EFMigrationsHistory` (`MigrationId`, `ProductVersion`)
VALUES ('20200817061844_Elections', '9.0.11');

ALTER TABLE `Materials` DROP COLUMN `GasFormId`;

ALTER TABLE `Materials` DROP COLUMN `SolidFormId`;

ALTER TABLE `Terrains` ADD `DefaultCellOutdoorsType` int(11) NOT NULL DEFAULT 0;

ALTER TABLE `Terrains` ADD `TerrainEditorText` varchar(45) NULL;

ALTER TABLE `Materials` MODIFY COLUMN `LiquidFormId` bigint NULL;

INSERT INTO `__EFMigrationsHistory` (`MigrationId`, `ProductVersion`)
VALUES ('20200830233741_TerrainUpdate', '9.0.11');

ALTER TABLE `CurrencyDescriptionPatterns` ADD `UseNaturalAggregationStyle` bit(1) NOT NULL DEFAULT 0;

INSERT INTO `__EFMigrationsHistory` (`MigrationId`, `ProductVersion`)
VALUES ('20200905062837_CurrencyPatternEnhancement', '9.0.11');

CREATE TABLE `KnowledgesCosts` (
    `KnowledgeId` bigint(20) NOT NULL,
    `ChargenResourceId` bigint(20) NOT NULL,
    `Cost` int(11) NOT NULL,
    CONSTRAINT `PRIMARY` PRIMARY KEY (`KnowledgeId`, `ChargenResourceId`),
    CONSTRAINT `FK_KnowledgesCosts_ChargenResources` FOREIGN KEY (`ChargenResourceId`) REFERENCES `ChargenResources` (`Id`) ON DELETE CASCADE,
    CONSTRAINT `FK_KnowledgesCosts_Knowledges` FOREIGN KEY (`KnowledgeId`) REFERENCES `knowledges` (`Id`) ON DELETE CASCADE
);

CREATE INDEX `FK_KnowledgesCosts_ChargenResources_idx` ON `KnowledgesCosts` (`ChargenResourceId`);

CREATE INDEX `FK_KnowledgesCosts_Knowledges_idx` ON `KnowledgesCosts` (`KnowledgeId`);

INSERT INTO `__EFMigrationsHistory` (`MigrationId`, `ProductVersion`)
VALUES ('20200928025908_KnowledgeBuilding', '9.0.11');

ALTER TABLE `Checks` MODIFY COLUMN `Type` int(11) NOT NULL;

INSERT INTO `__EFMigrationsHistory` (`MigrationId`, `ProductVersion`)
VALUES ('20201013213328_CheckFixing', '9.0.11');

ALTER TABLE `EmailTemplates` MODIFY COLUMN `TemplateType` int(11) NOT NULL;

INSERT INTO `__EFMigrationsHistory` (`MigrationId`, `ProductVersion`)
VALUES ('20201014230837_FixingEmailTemplates', '9.0.11');

CREATE TABLE `LineOfCreditAccounts` (
    `Id` bigint(20) NOT NULL AUTO_INCREMENT,
    `AccountName` longtext CHARACTER SET utf8mb4 NULL,
    `ShopId` bigint(20) NOT NULL,
    `IsSuspended` bit(1) NOT NULL,
    `AccountLimit` decimal(10,0) NOT NULL,
    `OutstandingBalance` decimal(10,0) NOT NULL,
    `AccountOwnerId` bigint(20) NOT NULL,
    `AccountOwnerName` text CHARACTER SET utf8 NULL,
    CONSTRAINT `PK_LineOfCreditAccounts` PRIMARY KEY (`Id`),
    CONSTRAINT `FK_LineOfCreditAccounts_Characters` FOREIGN KEY (`AccountOwnerId`) REFERENCES `Characters` (`Id`) ON DELETE CASCADE,
    CONSTRAINT `FK_LineOfCreditAccounts_Shops` FOREIGN KEY (`ShopId`) REFERENCES `Shops` (`Id`) ON DELETE CASCADE
);

CREATE TABLE `LineOfCreditAccountUsers` (
    `LineOfCreditAccountId` bigint(20) NOT NULL,
    `AccountUserId` bigint(20) NOT NULL,
    `AccountUserName` text CHARACTER SET utf8 NULL,
    `SpendingLimit` decimal(10,0) NULL,
    CONSTRAINT `PRIMARY` PRIMARY KEY (`LineOfCreditAccountId`, `AccountUserId`),
    CONSTRAINT `FK_LineOfCreditAccountUsers_Characters` FOREIGN KEY (`AccountUserId`) REFERENCES `Characters` (`Id`) ON DELETE CASCADE,
    CONSTRAINT `FK_LineOfCreditAccountUsers_LineOfCreditAccounts` FOREIGN KEY (`LineOfCreditAccountId`) REFERENCES `LineOfCreditAccounts` (`Id`) ON DELETE CASCADE
);

CREATE INDEX `FK_LineOfCreditAccounts_Characters_idx` ON `LineOfCreditAccounts` (`AccountOwnerId`);

CREATE INDEX `FK_LineOfCreditAccounts_Shops_idx` ON `LineOfCreditAccounts` (`ShopId`);

CREATE INDEX `FK_LineOfCreditAccountUsers_Characters_idx` ON `LineOfCreditAccountUsers` (`AccountUserId`);

CREATE INDEX `FK_LineOfCreditAccountUsers_LineOfCreditAccounts_idx` ON `LineOfCreditAccountUsers` (`LineOfCreditAccountId`);

INSERT INTO `__EFMigrationsHistory` (`MigrationId`, `ProductVersion`)
VALUES ('20201106014706_LineOfCreditAccounts', '9.0.11');

ALTER TABLE `TraitDefinitions` ADD `DisplayAsSubAttribute` bit(1) NOT NULL DEFAULT b'0';

ALTER TABLE `TraitDefinitions` ADD `DisplayOrder` int(11) NOT NULL DEFAULT 1;

ALTER TABLE `TraitDefinitions` ADD `ShowInAttributeCommand` bit(1) NOT NULL DEFAULT b'1';

ALTER TABLE `TraitDefinitions` ADD `ShowInScoreCommand` bit(1) NOT NULL DEFAULT b'1';

INSERT INTO `__EFMigrationsHistory` (`MigrationId`, `ProductVersion`)
VALUES ('20201106040133_AttributesUpdate', '9.0.11');

ALTER TABLE `EconomicZones` ADD `TotalRevenueHeld` decimal(10,0) NOT NULL DEFAULT 0;

CREATE TABLE `EconomicZoneTaxes` (
    `Id` bigint(20) NOT NULL AUTO_INCREMENT,
    `EconomicZoneId` bigint(20) NOT NULL,
    `Name` varchar(200) NULL,
    `MerchantDescription` varchar(200) NULL,
    `MerchandiseFilterProgId` bigint(20) NULL,
    `TaxType` varchar(50) NULL,
    `Definition` text CHARACTER SET utf8 NULL,
    CONSTRAINT `PK_EconomicZoneTaxes` PRIMARY KEY (`Id`),
    CONSTRAINT `FK_EconomicZoneTaxes_EconomicZones` FOREIGN KEY (`EconomicZoneId`) REFERENCES `EconomicZones` (`Id`) ON DELETE CASCADE,
    CONSTRAINT `FK_EconomicZoneTaxes_FutureProgs` FOREIGN KEY (`MerchandiseFilterProgId`) REFERENCES `FutureProgs` (`Id`) ON DELETE RESTRICT
);

CREATE INDEX `FK_EconomicZoneTaxes_EconomicZones_idx` ON `EconomicZoneTaxes` (`EconomicZoneId`);

CREATE INDEX `FK_EconomicZoneTaxes_FutureProgs_idx` ON `EconomicZoneTaxes` (`MerchandiseFilterProgId`);

INSERT INTO `__EFMigrationsHistory` (`MigrationId`, `ProductVersion`)
VALUES ('20201108122141_EconomicZoneUpdate', '9.0.11');

ALTER TABLE `EconomicZones` ADD `ControllingClanId` bigint(20) NULL;

CREATE INDEX `FK_EconomicZones_ControllingClans_idx` ON `EconomicZones` (`ControllingClanId`);

ALTER TABLE `EconomicZones` ADD CONSTRAINT `FK_EconomicZones_ControllingClans` FOREIGN KEY (`ControllingClanId`) REFERENCES `Clans` (`Id`) ON DELETE RESTRICT;

INSERT INTO `__EFMigrationsHistory` (`MigrationId`, `ProductVersion`)
VALUES ('20201113050353_EconomicZonesTouchup', '9.0.11');

ALTER TABLE `Zones` MODIFY COLUMN `Name` varchar(255) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL;

ALTER TABLE `Writings` MODIFY COLUMN `WritingType` varchar(45) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL;

ALTER TABLE `Writings` MODIFY COLUMN `Definition` text CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL;

ALTER TABLE `Wounds` MODIFY COLUMN `WoundType` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NOT NULL;

ALTER TABLE `Wounds` MODIFY COLUMN `ExtraInformation` text CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NULL;

ALTER TABLE `WitnessProfiles` MODIFY COLUMN `Name` varchar(250) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL;

ALTER TABLE `WeatherEvents` MODIFY COLUMN `WeatherRoomAddendum` text CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL;

ALTER TABLE `WeatherEvents` MODIFY COLUMN `WeatherEventType` varchar(45) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL;

ALTER TABLE `WeatherEvents` MODIFY COLUMN `WeatherDescription` text CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL;

ALTER TABLE `WeatherEvents` MODIFY COLUMN `Name` varchar(200) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL;

ALTER TABLE `WeatherEvents` MODIFY COLUMN `AdditionalInfo` text CHARACTER SET utf8 COLLATE utf8_general_ci NULL;

ALTER TABLE `WeatherControllers` MODIFY COLUMN `Name` varchar(500) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL;

ALTER TABLE `WearProfiles` MODIFY COLUMN `WearlocProfiles` text CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL;

ALTER TABLE `WearProfiles` MODIFY COLUMN `WearStringInventory` varchar(255) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL DEFAULT 'worn on';

ALTER TABLE `WearProfiles` MODIFY COLUMN `WearAffix` varchar(255) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL DEFAULT 'on';

ALTER TABLE `WearProfiles` MODIFY COLUMN `WearAction3rd` varchar(255) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL DEFAULT 'puts';

ALTER TABLE `WearProfiles` MODIFY COLUMN `WearAction1st` varchar(255) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL DEFAULT 'put';

ALTER TABLE `WearProfiles` MODIFY COLUMN `Type` varchar(50) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL DEFAULT 'Direct';

ALTER TABLE `WearProfiles` MODIFY COLUMN `Name` varchar(255) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL;

ALTER TABLE `WearProfiles` MODIFY COLUMN `Description` text CHARACTER SET utf8 COLLATE utf8_general_ci NULL;

ALTER TABLE `WearableSizeParameterRule` MODIFY COLUMN `WeightVolumeRatios` varchar(4000) CHARACTER SET utf8 COLLATE utf8_general_ci NULL;

ALTER TABLE `WearableSizeParameterRule` MODIFY COLUMN `TraitVolumeRatios` varchar(4000) CHARACTER SET utf8 COLLATE utf8_general_ci NULL;

ALTER TABLE `WearableSizeParameterRule` MODIFY COLUMN `HeightLinearRatios` varchar(4000) CHARACTER SET utf8 COLLATE utf8_general_ci NULL;

ALTER TABLE `WeaponTypes` MODIFY COLUMN `Name` varchar(200) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL;

ALTER TABLE `WeaponAttacks` MODIFY COLUMN `Name` text CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NULL;

ALTER TABLE `WeaponAttacks` MODIFY COLUMN `AdditionalInfo` text CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NULL;

ALTER TABLE `VariableValues` MODIFY COLUMN `ValueDefinition` varchar(4000) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL;

ALTER TABLE `VariableValues` MODIFY COLUMN `ReferenceProperty` varchar(50) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL;

ALTER TABLE `VariableDefinitions` MODIFY COLUMN `Property` varchar(50) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL;

ALTER TABLE `VariableDefaults` MODIFY COLUMN `DefaultValue` varchar(4000) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL;

ALTER TABLE `VariableDefaults` MODIFY COLUMN `Property` varchar(50) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL;

ALTER TABLE `UnitOfMeasure` MODIFY COLUMN `System` varchar(50) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL;

ALTER TABLE `UnitOfMeasure` MODIFY COLUMN `PrimaryAbbreviation` varchar(45) CHARACTER SET utf8 COLLATE utf8_general_ci NULL;

ALTER TABLE `UnitOfMeasure` MODIFY COLUMN `Name` varchar(50) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL;

ALTER TABLE `UnitOfMeasure` MODIFY COLUMN `Abbreviations` varchar(4000) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL;

ALTER TABLE `TraitExpressionParameters` MODIFY COLUMN `Parameter` varchar(50) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL;

ALTER TABLE `TraitExpression` MODIFY COLUMN `Name` varchar(200) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL DEFAULT 'Unnamed Expression';

ALTER TABLE `TraitExpression` MODIFY COLUMN `Expression` varchar(4000) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL;

ALTER TABLE `TraitDefinitions` MODIFY COLUMN `ValueExpression` text CHARACTER SET utf8 COLLATE utf8_general_ci NULL;

ALTER TABLE `TraitDefinitions` MODIFY COLUMN `TraitGroup` varchar(255) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL;

ALTER TABLE `TraitDefinitions` MODIFY COLUMN `Name` varchar(255) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL;

ALTER TABLE `TraitDefinitions` MODIFY COLUMN `ChargenBlurb` varchar(4000) CHARACTER SET utf8 COLLATE utf8_general_ci NULL;

ALTER TABLE `TraitDefinitions` MODIFY COLUMN `Alias` varchar(255) CHARACTER SET utf8 COLLATE utf8_general_ci NULL;

ALTER TABLE `TraitDecorators` MODIFY COLUMN `Type` varchar(255) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL;

ALTER TABLE `TraitDecorators` MODIFY COLUMN `Name` varchar(255) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL;

ALTER TABLE `TraitDecorators` MODIFY COLUMN `Contents` text CHARACTER SET utf8 COLLATE utf8_general_ci NULL;

ALTER TABLE `Timezones` MODIFY COLUMN `Name` varchar(255) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL;

ALTER TABLE `Timezones` MODIFY COLUMN `Description` varchar(4000) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL;

ALTER TABLE `TimeZoneInfos` MODIFY COLUMN `Display` varchar(1000) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL;

ALTER TABLE `TimeZoneInfos` MODIFY COLUMN `Id` varchar(100) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL;

ALTER TABLE `Terrains` MODIFY COLUMN `TerrainEditorColour` varchar(45) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL DEFAULT '#FFFFFFFF';

ALTER TABLE `Terrains` MODIFY COLUMN `TerrainBehaviourMode` varchar(50) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL;

ALTER TABLE `Terrains` MODIFY COLUMN `Name` varchar(255) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL;

ALTER TABLE `Terrains` MODIFY COLUMN `AtmosphereType` varchar(45) CHARACTER SET utf8 COLLATE utf8_general_ci NULL;

ALTER TABLE `Tags` MODIFY COLUMN `Name` varchar(255) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL;

ALTER TABLE `SurgicalProcedures` MODIFY COLUMN `ProcedureName` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL;

ALTER TABLE `SurgicalProcedures` MODIFY COLUMN `ProcedureGerund` text CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NULL;

ALTER TABLE `SurgicalProcedures` MODIFY COLUMN `ProcedureDescriptionEmote` text CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NULL;

ALTER TABLE `SurgicalProcedures` MODIFY COLUMN `ProcedureBeginEmote` text CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NULL;

ALTER TABLE `SurgicalProcedures` MODIFY COLUMN `Name` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL;

ALTER TABLE `SurgicalProcedures` MODIFY COLUMN `MedicalSchool` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NULL;

ALTER TABLE `SurgicalProcedures` MODIFY COLUMN `Definition` text CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NULL;

ALTER TABLE `SurgicalProcedurePhases` MODIFY COLUMN `PhaseSpecialEffects` varchar(500) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NULL;

ALTER TABLE `SurgicalProcedurePhases` MODIFY COLUMN `PhaseEmote` text CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL;

ALTER TABLE `SurgicalProcedurePhases` MODIFY COLUMN `InventoryActionPlan` text CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NULL;

ALTER TABLE `StaticStrings` MODIFY COLUMN `Text` text CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL;

ALTER TABLE `StaticStrings` MODIFY COLUMN `Id` varchar(50) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL;

ALTER TABLE `StaticConfigurations` MODIFY COLUMN `Definition` text CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL;

ALTER TABLE `StaticConfigurations` MODIFY COLUMN `SettingName` varchar(100) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL;

ALTER TABLE `StackDecorators` MODIFY COLUMN `Type` varchar(50) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL;

ALTER TABLE `StackDecorators` MODIFY COLUMN `Name` varchar(255) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL;

ALTER TABLE `StackDecorators` MODIFY COLUMN `Description` varchar(1000) CHARACTER SET utf8 COLLATE utf8_general_ci NULL;

ALTER TABLE `StackDecorators` MODIFY COLUMN `Definition` varchar(10000) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL;

ALTER TABLE `Socials` MODIFY COLUMN `OneTargetEcho` varchar(1000) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL;

ALTER TABLE `Socials` MODIFY COLUMN `NoTargetEcho` varchar(1000) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL;

ALTER TABLE `Socials` MODIFY COLUMN `MultiTargetEcho` varchar(1000) CHARACTER SET utf8 COLLATE utf8_general_ci NULL;

ALTER TABLE `Socials` MODIFY COLUMN `DirectionTargetEcho` varchar(1000) CHARACTER SET utf8 COLLATE utf8_general_ci NULL;

ALTER TABLE `Socials` MODIFY COLUMN `Name` varchar(100) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL;

ALTER TABLE `SkyDescriptionTemplates_Values` MODIFY COLUMN `Description` varchar(4000) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL;

ALTER TABLE `SkyDescriptionTemplates` MODIFY COLUMN `Name` varchar(255) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL;

ALTER TABLE `ShopTransactionRecords` MODIFY COLUMN `MudDateTime` varchar(500) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL;

ALTER TABLE `Shops` MODIFY COLUMN `Name` varchar(200) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL;

ALTER TABLE `Shops` MODIFY COLUMN `EmployeeRecords` mediumtext CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL;

ALTER TABLE `ShieldTypes` MODIFY COLUMN `Name` text CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL;

ALTER TABLE `Shards` MODIFY COLUMN `Name` varchar(255) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL;

ALTER TABLE `Seasons` MODIFY COLUMN `Name` varchar(200) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL;

ALTER TABLE `Seasons` MODIFY COLUMN `Description` text CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL;

ALTER TABLE `Scripts` MODIFY COLUMN `UnknownScriptDescription` text CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL;

ALTER TABLE `Scripts` MODIFY COLUMN `Name` text CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL;

ALTER TABLE `Scripts` MODIFY COLUMN `KnownScriptDescription` text CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL;

ALTER TABLE `RegionalClimates_Seasons` MODIFY COLUMN `TemperatureInfo` text CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL;

ALTER TABLE `RegionalClimates` MODIFY COLUMN `Name` varchar(200) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL;

ALTER TABLE `Ranks_Titles` MODIFY COLUMN `Title` varchar(50) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL;

ALTER TABLE `Ranks_Abbreviations` MODIFY COLUMN `Abbreviation` varchar(50) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL;

ALTER TABLE `Ranks` MODIFY COLUMN `RankPath` varchar(50) CHARACTER SET utf8 COLLATE utf8_general_ci NULL;

ALTER TABLE `Ranks` MODIFY COLUMN `Name` varchar(100) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL;

ALTER TABLE `RangedWeaponTypes` MODIFY COLUMN `SpecificAmmunitionGrade` text CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL;

ALTER TABLE `RangedWeaponTypes` MODIFY COLUMN `Name` varchar(200) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL;

ALTER TABLE `RangedWeaponTypes` MODIFY COLUMN `DamageBonusExpression` text CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL;

ALTER TABLE `RangedWeaponTypes` MODIFY COLUMN `AccuracyBonusExpression` text CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL;

ALTER TABLE `RangedCovers` MODIFY COLUMN `Name` varchar(200) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL;

ALTER TABLE `RangedCovers` MODIFY COLUMN `DescriptionString` text CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL;

ALTER TABLE `RangedCovers` MODIFY COLUMN `ActionDescriptionString` text CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL;

ALTER TABLE `RandomNameProfiles_Elements` MODIFY COLUMN `Name` varchar(100) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL;

ALTER TABLE `RandomNameProfiles_DiceExpressions` MODIFY COLUMN `DiceExpression` varchar(255) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL;

ALTER TABLE `RandomNameProfiles` MODIFY COLUMN `Name` varchar(255) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL;

ALTER TABLE `Races_AdditionalCharacteristics` MODIFY COLUMN `Usage` varchar(255) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL;

ALTER TABLE `Races_AdditionalBodyparts` MODIFY COLUMN `Usage` varchar(50) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL;

ALTER TABLE `Races` MODIFY COLUMN `Name` varchar(255) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL;

ALTER TABLE `Races` MODIFY COLUMN `MaximumLiftWeightExpression` text CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL;

ALTER TABLE `Races` MODIFY COLUMN `MaximumDragWeightExpression` text CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL;

ALTER TABLE `Races` MODIFY COLUMN `HoldBreathLengthExpression` varchar(500) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL DEFAULT '120';

ALTER TABLE `Races` MODIFY COLUMN `HandednessOptions` varchar(100) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL DEFAULT '1 3';

ALTER TABLE `Races` MODIFY COLUMN `EatCorpseEmoteText` varchar(500) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL DEFAULT '@ eat|eats {0}$1';

ALTER TABLE `Races` MODIFY COLUMN `DiceExpression` varchar(255) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL;

ALTER TABLE `Races` MODIFY COLUMN `Description` varchar(4000) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL;

ALTER TABLE `Races` MODIFY COLUMN `CommunicationStrategyType` varchar(45) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL DEFAULT 'humanoid';

ALTER TABLE `Races` MODIFY COLUMN `BreathingVolumeExpression` varchar(500) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL DEFAULT '7';

ALTER TABLE `Races` MODIFY COLUMN `AllowedGenders` varchar(255) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL;

ALTER TABLE `RaceEdibleForagableYields` MODIFY COLUMN `EatEmote` varchar(1000) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL DEFAULT '@ eat|eats {0} from the location.';

ALTER TABLE `RaceEdibleForagableYields` MODIFY COLUMN `YieldType` varchar(50) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL;

ALTER TABLE `RaceButcheryProfiles_SkinningEmotes` MODIFY COLUMN `Emote` text CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL;

ALTER TABLE `RaceButcheryProfiles_SkinningEmotes` MODIFY COLUMN `Subcategory` varchar(100) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL;

ALTER TABLE `RaceButcheryProfiles_BreakdownEmotes` MODIFY COLUMN `Emote` text CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL;

ALTER TABLE `RaceButcheryProfiles_BreakdownEmotes` MODIFY COLUMN `Subcategory` varchar(100) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL;

ALTER TABLE `RaceButcheryProfiles_BreakdownChecks` MODIFY COLUMN `Subcageory` varchar(100) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL;

ALTER TABLE `RaceButcheryProfiles` MODIFY COLUMN `Name` varchar(100) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL;

ALTER TABLE `Projects` MODIFY COLUMN `Type` varchar(45) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL;

ALTER TABLE `Projects` MODIFY COLUMN `Name` varchar(200) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL;

ALTER TABLE `Projects` MODIFY COLUMN `Definition` text CHARACTER SET utf8 COLLATE utf8_general_ci NULL;

ALTER TABLE `ProjectPhases` MODIFY COLUMN `Description` text CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL;

ALTER TABLE `ProjectMaterialRequirements` MODIFY COLUMN `Type` varchar(45) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL;

ALTER TABLE `ProjectMaterialRequirements` MODIFY COLUMN `Name` varchar(200) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL;

ALTER TABLE `ProjectMaterialRequirements` MODIFY COLUMN `Description` text CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL;

ALTER TABLE `ProjectMaterialRequirements` MODIFY COLUMN `Definition` text CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL;

ALTER TABLE `ProjectLabourRequirements` MODIFY COLUMN `Type` varchar(45) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL;

ALTER TABLE `ProjectLabourRequirements` MODIFY COLUMN `Name` varchar(200) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL;

ALTER TABLE `ProjectLabourRequirements` MODIFY COLUMN `Description` text CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL;

ALTER TABLE `ProjectLabourRequirements` MODIFY COLUMN `Definition` text CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL;

ALTER TABLE `ProjectLabourImpacts` MODIFY COLUMN `Type` varchar(45) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL;

ALTER TABLE `ProjectLabourImpacts` MODIFY COLUMN `Name` varchar(200) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL;

ALTER TABLE `ProjectLabourImpacts` MODIFY COLUMN `Description` varchar(1000) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL;

ALTER TABLE `ProjectLabourImpacts` MODIFY COLUMN `Definition` text CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL;

ALTER TABLE `ProjectActions` MODIFY COLUMN `Type` varchar(45) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL;

ALTER TABLE `ProjectActions` MODIFY COLUMN `Name` varchar(200) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL;

ALTER TABLE `ProjectActions` MODIFY COLUMN `Description` varchar(1000) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL;

ALTER TABLE `ProjectActions` MODIFY COLUMN `Definition` text CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL;

ALTER TABLE `ProgSchedules` MODIFY COLUMN `ReferenceTime` varchar(500) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL;

ALTER TABLE `ProgSchedules` MODIFY COLUMN `ReferenceDate` varchar(500) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL;

ALTER TABLE `ProgSchedules` MODIFY COLUMN `Name` varchar(200) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL;

ALTER TABLE `PopulationBloodModels` MODIFY COLUMN `Name` varchar(200) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL;

ALTER TABLE `Paygrades` MODIFY COLUMN `Name` varchar(100) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL;

ALTER TABLE `Paygrades` MODIFY COLUMN `Abbreviation` varchar(100) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL;

ALTER TABLE `PatrolRoutes` ADD `StartPatrolProgId` bigint(20) NULL;

ALTER TABLE `NPCTemplates` MODIFY COLUMN `Type` varchar(255) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL;

ALTER TABLE `NPCTemplates` MODIFY COLUMN `Name` varchar(255) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL;

ALTER TABLE `NPCTemplates` MODIFY COLUMN `Definition` text CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL;

ALTER TABLE `NonCardinalExitTemplates` MODIFY COLUMN `OutboundVerb` varchar(255) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL;

ALTER TABLE `NonCardinalExitTemplates` MODIFY COLUMN `OriginOutboundPreface` varchar(255) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL;

ALTER TABLE `NonCardinalExitTemplates` MODIFY COLUMN `OriginInboundPreface` varchar(255) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL;

ALTER TABLE `NonCardinalExitTemplates` MODIFY COLUMN `Name` varchar(255) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL;

ALTER TABLE `NonCardinalExitTemplates` MODIFY COLUMN `InboundVerb` varchar(255) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL;

ALTER TABLE `NonCardinalExitTemplates` MODIFY COLUMN `DestinationOutboundPreface` varchar(255) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL;

ALTER TABLE `NonCardinalExitTemplates` MODIFY COLUMN `DestinationInboundPreface` varchar(255) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL;

ALTER TABLE `NameCulture` MODIFY COLUMN `Name` varchar(255) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL;

ALTER TABLE `NameCulture` MODIFY COLUMN `Definition` text CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL;

ALTER TABLE `MoveSpeeds` MODIFY COLUMN `ThirdPersonVerb` varchar(255) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL;

ALTER TABLE `MoveSpeeds` MODIFY COLUMN `PresentParticiple` varchar(255) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL;

ALTER TABLE `MoveSpeeds` MODIFY COLUMN `FirstPersonVerb` varchar(255) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL;

ALTER TABLE `MoveSpeeds` MODIFY COLUMN `Alias` varchar(255) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL;

ALTER TABLE `Merits` MODIFY COLUMN `Type` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL;

ALTER TABLE `Merits` MODIFY COLUMN `Name` varchar(200) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL;

ALTER TABLE `Merits` MODIFY COLUMN `Definition` text CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL;

ALTER TABLE `Merchandises` MODIFY COLUMN `Name` varchar(1000) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL;

ALTER TABLE `Merchandises` MODIFY COLUMN `ListDescription` varchar(500) CHARACTER SET utf8 COLLATE utf8_general_ci NULL;

ALTER TABLE `Materials` MODIFY COLUMN `ResidueSdesc` text CHARACTER SET utf8 COLLATE utf8_general_ci NULL;

ALTER TABLE `Materials` MODIFY COLUMN `ResidueDesc` text CHARACTER SET utf8 COLLATE utf8_general_ci NULL;

ALTER TABLE `Materials` MODIFY COLUMN `ResidueColour` varchar(45) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT 'white';

ALTER TABLE `Materials` MODIFY COLUMN `Name` varchar(255) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL;

ALTER TABLE `Materials` MODIFY COLUMN `MaterialDescription` varchar(4000) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL;

ALTER TABLE `MagicSchools` MODIFY COLUMN `SchoolVerb` varchar(45) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL;

ALTER TABLE `MagicSchools` MODIFY COLUMN `SchoolAdjective` varchar(45) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL;

ALTER TABLE `MagicSchools` MODIFY COLUMN `PowerListColour` varchar(45) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL;

ALTER TABLE `MagicSchools` MODIFY COLUMN `Name` varchar(200) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL;

ALTER TABLE `MagicResources` MODIFY COLUMN `Type` varchar(45) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL;

ALTER TABLE `MagicResources` MODIFY COLUMN `Name` varchar(200) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL;

ALTER TABLE `MagicResources` MODIFY COLUMN `Definition` text CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL;

ALTER TABLE `MagicPowers` MODIFY COLUMN `ShowHelp` text CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL;

ALTER TABLE `MagicPowers` MODIFY COLUMN `PowerModel` varchar(45) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL;

ALTER TABLE `MagicPowers` MODIFY COLUMN `Name` varchar(200) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL;

ALTER TABLE `MagicPowers` MODIFY COLUMN `Definition` text CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL;

ALTER TABLE `MagicPowers` MODIFY COLUMN `Blurb` varchar(500) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL;

ALTER TABLE `MagicGenerators` MODIFY COLUMN `Type` varchar(45) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL;

ALTER TABLE `MagicGenerators` MODIFY COLUMN `Name` varchar(200) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL;

ALTER TABLE `MagicGenerators` MODIFY COLUMN `Definition` text CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL;

ALTER TABLE `MagicCapabilities` MODIFY COLUMN `Name` varchar(200) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL;

ALTER TABLE `MagicCapabilities` MODIFY COLUMN `Definition` text CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL;

ALTER TABLE `MagicCapabilities` MODIFY COLUMN `CapabilityModel` varchar(45) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL;

ALTER TABLE `LoginIPs` MODIFY COLUMN `IpAddress` varchar(200) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL;

ALTER TABLE `Locks` MODIFY COLUMN `Name` varchar(255) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL;

ALTER TABLE `Liquids` MODIFY COLUMN `WetShortDescription` text CHARACTER SET utf8 COLLATE utf8_general_ci NULL;

ALTER TABLE `Liquids` MODIFY COLUMN `WetDescription` text CHARACTER SET utf8 COLLATE utf8_general_ci NULL;

ALTER TABLE `Liquids` MODIFY COLUMN `VagueTasteText` text CHARACTER SET utf8 COLLATE utf8_general_ci NULL;

ALTER TABLE `Liquids` MODIFY COLUMN `VagueSmellText` text CHARACTER SET utf8 COLLATE utf8_general_ci NULL;

ALTER TABLE `Liquids` MODIFY COLUMN `TasteText` text CHARACTER SET utf8 COLLATE utf8_general_ci NULL;

ALTER TABLE `Liquids` MODIFY COLUMN `SmellText` text CHARACTER SET utf8 COLLATE utf8_general_ci NULL;

ALTER TABLE `Liquids` MODIFY COLUMN `Name` varchar(200) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL;

ALTER TABLE `Liquids` MODIFY COLUMN `LongDescription` text CHARACTER SET utf8 COLLATE utf8_general_ci NULL;

ALTER TABLE `Liquids` MODIFY COLUMN `DrenchedShortDescription` text CHARACTER SET utf8 COLLATE utf8_general_ci NULL;

ALTER TABLE `Liquids` MODIFY COLUMN `DrenchedDescription` text CHARACTER SET utf8 COLLATE utf8_general_ci NULL;

ALTER TABLE `Liquids` MODIFY COLUMN `DisplayColour` varchar(45) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL DEFAULT 'blue';

ALTER TABLE `Liquids` MODIFY COLUMN `Description` text CHARACTER SET utf8 COLLATE utf8_general_ci NULL;

ALTER TABLE `Liquids` MODIFY COLUMN `DampShortDescription` text CHARACTER SET utf8 COLLATE utf8_general_ci NULL;

ALTER TABLE `Liquids` MODIFY COLUMN `DampDescription` text CHARACTER SET utf8 COLLATE utf8_general_ci NULL;

ALTER TABLE `LineOfCreditAccountUsers` MODIFY COLUMN `AccountUserName` text CHARACTER SET utf8 COLLATE utf8_general_ci NULL;

ALTER TABLE `LineOfCreditAccounts` MODIFY COLUMN `AccountOwnerName` text CHARACTER SET utf8 COLLATE utf8_general_ci NULL;

ALTER TABLE `Limbs` MODIFY COLUMN `Name` text CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NULL;

ALTER TABLE `LegalClasses` MODIFY COLUMN `Name` varchar(250) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL;

ALTER TABLE `LegalAuthorities` MODIFY COLUMN `Name` varchar(250) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL;

ALTER TABLE `LegalAuthorities` ADD `EnforcerStowingLocationId` bigint(20) NULL;

ALTER TABLE `LegalAuthorities` ADD `MarshallingLocationId` bigint(20) NULL;

ALTER TABLE `LegalAuthorities` ADD `PlayersKnowTheirCrimes` bit(1) NOT NULL DEFAULT b'0';

ALTER TABLE `LegalAuthorities` ADD `PreparingLocationId` bigint(20) NULL;

ALTER TABLE `LegalAuthorities` ADD `PrisonLocationId` bigint(20) NULL;

ALTER TABLE `Laws` MODIFY COLUMN `Name` varchar(250) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL;

ALTER TABLE `Laws` MODIFY COLUMN `EnforcementStrategy` varchar(500) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL;

ALTER TABLE `Languages` MODIFY COLUMN `UnknownLanguageDescription` varchar(255) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL;

ALTER TABLE `Languages` MODIFY COLUMN `Name` varchar(255) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL;

ALTER TABLE `LanguageDifficultyModels` MODIFY COLUMN `Type` varchar(255) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL;

ALTER TABLE `LanguageDifficultyModels` MODIFY COLUMN `Name` varchar(255) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL;

ALTER TABLE `LanguageDifficultyModels` MODIFY COLUMN `Definition` longtext CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL;

ALTER TABLE `knowledges` MODIFY COLUMN `Type` varchar(200) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL;

ALTER TABLE `knowledges` MODIFY COLUMN `Subtype` varchar(200) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL;

ALTER TABLE `knowledges` MODIFY COLUMN `Name` varchar(200) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL;

ALTER TABLE `knowledges` MODIFY COLUMN `LongDescription` text CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL;

ALTER TABLE `knowledges` MODIFY COLUMN `Description` text CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL;

ALTER TABLE `ItemGroups` MODIFY COLUMN `Name` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NULL;

ALTER TABLE `ItemGroups` MODIFY COLUMN `Keywords` varchar(1000) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NULL;

ALTER TABLE `ItemGroupForms` MODIFY COLUMN `Type` varchar(45) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL;

ALTER TABLE `ItemGroupForms` MODIFY COLUMN `Definition` text CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL;

ALTER TABLE `Improvers` MODIFY COLUMN `Type` varchar(50) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL;

ALTER TABLE `Improvers` MODIFY COLUMN `Name` varchar(255) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL;

ALTER TABLE `Improvers` MODIFY COLUMN `Definition` text CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL;

ALTER TABLE `Hooks` MODIFY COLUMN `Type` varchar(255) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL;

ALTER TABLE `Hooks` MODIFY COLUMN `Name` varchar(255) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL;

ALTER TABLE `Hooks` MODIFY COLUMN `Definition` text CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NOT NULL;

ALTER TABLE `Hooks` MODIFY COLUMN `Category` varchar(255) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL;

ALTER TABLE `Helpfiles_ExtraTexts` MODIFY COLUMN `Text` text CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL;

ALTER TABLE `Helpfiles` MODIFY COLUMN `TagLine` varchar(500) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL;

ALTER TABLE `Helpfiles` MODIFY COLUMN `Subcategory` varchar(255) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL;

ALTER TABLE `Helpfiles` MODIFY COLUMN `PublicText` text CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL;

ALTER TABLE `Helpfiles` MODIFY COLUMN `Name` varchar(255) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL;

ALTER TABLE `Helpfiles` MODIFY COLUMN `LastEditedBy` varchar(100) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL;

ALTER TABLE `Helpfiles` MODIFY COLUMN `Keywords` varchar(500) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL;

ALTER TABLE `Helpfiles` MODIFY COLUMN `Category` varchar(255) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL;

ALTER TABLE `HeightWeightModels` MODIFY COLUMN `Name` varchar(4000) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL;

ALTER TABLE `HearingProfiles` MODIFY COLUMN `Type` varchar(4000) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL;

ALTER TABLE `HearingProfiles` MODIFY COLUMN `SurveyDescription` varchar(4000) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL;

ALTER TABLE `HearingProfiles` MODIFY COLUMN `Name` varchar(4000) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL;

ALTER TABLE `HearingProfiles` MODIFY COLUMN `Definition` text CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL;

ALTER TABLE `HealthStrategies` MODIFY COLUMN `Type` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NOT NULL;

ALTER TABLE `HealthStrategies` MODIFY COLUMN `Name` varchar(200) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NOT NULL;

ALTER TABLE `HealthStrategies` MODIFY COLUMN `Definition` text CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NOT NULL;

ALTER TABLE `GroupAITemplates` MODIFY COLUMN `Name` varchar(200) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL;

ALTER TABLE `GroupAITemplates` MODIFY COLUMN `Definition` mediumtext CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL;

ALTER TABLE `GroupAIs` MODIFY COLUMN `Name` varchar(200) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL;

ALTER TABLE `GroupAIs` MODIFY COLUMN `Definition` mediumtext CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL;

ALTER TABLE `GroupAIs` MODIFY COLUMN `Data` mediumtext CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL;

ALTER TABLE `Grids` MODIFY COLUMN `GridType` varchar(45) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL;

ALTER TABLE `Grids` MODIFY COLUMN `Definition` mediumtext CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL;

ALTER TABLE `Gases` MODIFY COLUMN `VagueSmellText` text CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL;

ALTER TABLE `Gases` MODIFY COLUMN `SmellText` text CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL;

ALTER TABLE `Gases` MODIFY COLUMN `Name` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL;

ALTER TABLE `Gases` MODIFY COLUMN `DisplayColour` varchar(40) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL;

ALTER TABLE `Gases` MODIFY COLUMN `Description` varchar(200) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL;

ALTER TABLE `GameItems` MODIFY COLUMN `PositionTargetType` varchar(45) CHARACTER SET utf8 COLLATE utf8_general_ci NULL;

ALTER TABLE `GameItems` MODIFY COLUMN `PositionEmote` text CHARACTER SET utf8 COLLATE utf8_general_ci NULL;

ALTER TABLE `GameItems` MODIFY COLUMN `EffectData` mediumtext CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL;

ALTER TABLE `GameItemProtos_DefaultVariables` MODIFY COLUMN `VariableValue` text CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL;

ALTER TABLE `GameItemProtos_DefaultVariables` MODIFY COLUMN `VariableName` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL;

ALTER TABLE `GameItemProtos` MODIFY COLUMN `ShortDescription` varchar(1000) CHARACTER SET utf8 COLLATE utf8_general_ci NULL;

ALTER TABLE `GameItemProtos` MODIFY COLUMN `Name` varchar(255) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL;

ALTER TABLE `GameItemProtos` MODIFY COLUMN `MorphEmote` varchar(1000) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL DEFAULT '$0 $?1|morphs into $1|decays into nothing$.';

ALTER TABLE `GameItemProtos` MODIFY COLUMN `LongDescription` text CHARACTER SET utf8 COLLATE utf8_general_ci NULL;

ALTER TABLE `GameItemProtos` MODIFY COLUMN `Keywords` text CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL;

ALTER TABLE `GameItemProtos` MODIFY COLUMN `FullDescription` varchar(4000) CHARACTER SET utf8 COLLATE utf8_general_ci NULL;

ALTER TABLE `GameItemProtos` MODIFY COLUMN `CustomColour` varchar(45) CHARACTER SET utf8 COLLATE utf8_general_ci NULL;

ALTER TABLE `GameItemComponents` MODIFY COLUMN `Definition` text CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL;

ALTER TABLE `GameItemComponentProtos` MODIFY COLUMN `Type` varchar(50) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL;

ALTER TABLE `GameItemComponentProtos` MODIFY COLUMN `Name` varchar(4000) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL;

ALTER TABLE `GameItemComponentProtos` MODIFY COLUMN `Description` text CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL;

ALTER TABLE `GameItemComponentProtos` MODIFY COLUMN `Definition` text CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL;

ALTER TABLE `FutureProgs_Parameters` MODIFY COLUMN `ParameterName` varchar(4000) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL;

ALTER TABLE `FutureProgs` MODIFY COLUMN `Subcategory` varchar(255) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL;

ALTER TABLE `FutureProgs` MODIFY COLUMN `FunctionText` text CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NOT NULL;

ALTER TABLE `FutureProgs` MODIFY COLUMN `FunctionName` varchar(255) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL;

ALTER TABLE `FutureProgs` MODIFY COLUMN `FunctionComment` text CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL;

ALTER TABLE `FutureProgs` MODIFY COLUMN `Category` varchar(255) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL;

ALTER TABLE `Foragables` MODIFY COLUMN `QuantityDiceExpression` varchar(200) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL;

ALTER TABLE `Foragables` MODIFY COLUMN `Name` varchar(200) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL;

ALTER TABLE `Foragables` MODIFY COLUMN `ForagableTypes` text CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL;

ALTER TABLE `ForagableProfiles_MaximumYields` MODIFY COLUMN `ForageType` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL;

ALTER TABLE `ForagableProfiles_HourlyYieldGains` MODIFY COLUMN `ForageType` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL;

ALTER TABLE `ForagableProfiles` MODIFY COLUMN `Name` varchar(200) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL;

ALTER TABLE `FinancialPeriods` MODIFY COLUMN `MudPeriodStart` varchar(200) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL;

ALTER TABLE `FinancialPeriods` MODIFY COLUMN `MudPeriodEnd` varchar(200) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL;

ALTER TABLE `Exits` MODIFY COLUMN `Verb2` varchar(255) CHARACTER SET utf8 COLLATE utf8_general_ci NULL;

ALTER TABLE `Exits` MODIFY COLUMN `Verb1` varchar(255) CHARACTER SET utf8 COLLATE utf8_general_ci NULL;

ALTER TABLE `Exits` MODIFY COLUMN `PrimaryKeyword2` varchar(255) CHARACTER SET utf8 COLLATE utf8_general_ci NULL;

ALTER TABLE `Exits` MODIFY COLUMN `PrimaryKeyword1` varchar(255) CHARACTER SET utf8 COLLATE utf8_general_ci NULL;

ALTER TABLE `Exits` MODIFY COLUMN `OutboundTarget2` varchar(255) CHARACTER SET utf8 COLLATE utf8_general_ci NULL;

ALTER TABLE `Exits` MODIFY COLUMN `OutboundTarget1` varchar(255) CHARACTER SET utf8 COLLATE utf8_general_ci NULL;

ALTER TABLE `Exits` MODIFY COLUMN `OutboundDescription2` varchar(255) CHARACTER SET utf8 COLLATE utf8_general_ci NULL;

ALTER TABLE `Exits` MODIFY COLUMN `OutboundDescription1` varchar(255) CHARACTER SET utf8 COLLATE utf8_general_ci NULL;

ALTER TABLE `Exits` MODIFY COLUMN `Keywords2` varchar(255) CHARACTER SET utf8 COLLATE utf8_general_ci NULL;

ALTER TABLE `Exits` MODIFY COLUMN `Keywords1` varchar(255) CHARACTER SET utf8 COLLATE utf8_general_ci NULL;

ALTER TABLE `Exits` MODIFY COLUMN `InboundTarget2` varchar(255) CHARACTER SET utf8 COLLATE utf8_general_ci NULL;

ALTER TABLE `Exits` MODIFY COLUMN `InboundTarget1` varchar(255) CHARACTER SET utf8 COLLATE utf8_general_ci NULL;

ALTER TABLE `Exits` MODIFY COLUMN `InboundDescription2` varchar(255) CHARACTER SET utf8 COLLATE utf8_general_ci NULL;

ALTER TABLE `Exits` MODIFY COLUMN `InboundDescription1` varchar(255) CHARACTER SET utf8 COLLATE utf8_general_ci NULL;

ALTER TABLE `Exits` MODIFY COLUMN `BlockedLayers` varchar(255) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL DEFAULT '';

ALTER TABLE `Ethnicities` MODIFY COLUMN `Name` varchar(4000) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL;

ALTER TABLE `Ethnicities` MODIFY COLUMN `EthnicSubgroup` text CHARACTER SET utf8 COLLATE utf8_general_ci NULL;

ALTER TABLE `Ethnicities` MODIFY COLUMN `EthnicGroup` text CHARACTER SET utf8 COLLATE utf8_general_ci NULL;

ALTER TABLE `Ethnicities` MODIFY COLUMN `ChargenBlurb` varchar(4000) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL;

ALTER TABLE `EntityDescriptions` MODIFY COLUMN `ShortDescription` varchar(4000) CHARACTER SET utf8 COLLATE utf8_general_ci NULL;

ALTER TABLE `EntityDescriptions` MODIFY COLUMN `FullDescription` varchar(4000) CHARACTER SET utf8 COLLATE utf8_general_ci NULL;

ALTER TABLE `EntityDescriptionPatterns` MODIFY COLUMN `Pattern` varchar(4000) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL;

ALTER TABLE `EnforcementAuthorities` MODIFY COLUMN `Name` varchar(250) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL;

ALTER TABLE `EmailTemplates` MODIFY COLUMN `Subject` varchar(4000) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL;

ALTER TABLE `EmailTemplates` MODIFY COLUMN `ReturnAddress` varchar(4000) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL;

ALTER TABLE `EmailTemplates` MODIFY COLUMN `Content` varchar(4000) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL;

ALTER TABLE `EditableItems` MODIFY COLUMN `ReviewerComment` varchar(4000) CHARACTER SET utf8 COLLATE utf8_general_ci NULL;

ALTER TABLE `EditableItems` MODIFY COLUMN `BuilderComment` varchar(4000) CHARACTER SET utf8 COLLATE utf8_general_ci NULL;

ALTER TABLE `EconomicZoneTaxes` MODIFY COLUMN `Definition` text CHARACTER SET utf8 COLLATE utf8_general_ci NULL;

ALTER TABLE `EconomicZones` MODIFY COLUMN `ReferenceTime` varchar(100) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL;

ALTER TABLE `EconomicZones` MODIFY COLUMN `Name` varchar(200) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL;

ALTER TABLE `Dubs` MODIFY COLUMN `TargetType` varchar(100) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL;

ALTER TABLE `Dubs` MODIFY COLUMN `LastDescription` varchar(4000) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL;

ALTER TABLE `Dubs` MODIFY COLUMN `Keywords` varchar(4000) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL;

ALTER TABLE `Dubs` MODIFY COLUMN `IntroducedName` varchar(4000) CHARACTER SET utf8 COLLATE utf8_general_ci NULL;

ALTER TABLE `DrugsIntensities` MODIFY COLUMN `AdditionalEffects` text CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NULL;

ALTER TABLE `Drugs` MODIFY COLUMN `Name` varchar(200) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL;

ALTER TABLE `Dreams` MODIFY COLUMN `Name` varchar(200) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL;

ALTER TABLE `Dream_Phases` MODIFY COLUMN `DreamerText` text CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL;

ALTER TABLE `Dream_Phases` MODIFY COLUMN `DreamerCommand` text CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL;

ALTER TABLE `Drawings` MODIFY COLUMN `ShortDescription` text CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL;

ALTER TABLE `Drawings` MODIFY COLUMN `FullDescription` text CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL;

ALTER TABLE `Doors` MODIFY COLUMN `Name` varchar(4000) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL;

ALTER TABLE `DisfigurementTemplates` MODIFY COLUMN `Type` varchar(50) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL;

ALTER TABLE `DisfigurementTemplates` MODIFY COLUMN `ShortDescription` varchar(500) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL;

ALTER TABLE `DisfigurementTemplates` MODIFY COLUMN `Name` varchar(500) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL;

ALTER TABLE `DisfigurementTemplates` MODIFY COLUMN `FullDescription` varchar(5000) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL;

ALTER TABLE `DisfigurementTemplates` MODIFY COLUMN `Definition` text CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL;

ALTER TABLE `DefaultHooks` MODIFY COLUMN `PerceivableType` varchar(45) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL;

ALTER TABLE `DamagePatterns` MODIFY COLUMN `Name` varchar(4000) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL;

ALTER TABLE `CurrencyDivisions` MODIFY COLUMN `Name` varchar(4000) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL;

ALTER TABLE `CurrencyDivisionAbbreviations` MODIFY COLUMN `Pattern` varchar(150) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL;

ALTER TABLE `CurrencyDescriptionPatterns` MODIFY COLUMN `NegativePrefix` varchar(4000) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL;

ALTER TABLE `CurrencyDescriptionPatternElementSpecialValues` MODIFY COLUMN `Text` varchar(4000) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL;

ALTER TABLE `CurrencyDescriptionPatternElements` MODIFY COLUMN `PluraliseWord` varchar(4000) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL;

ALTER TABLE `CurrencyDescriptionPatternElements` MODIFY COLUMN `Pattern` varchar(4000) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL;

ALTER TABLE `CurrencyDescriptionPatternElements` MODIFY COLUMN `AlternatePattern` varchar(4000) CHARACTER SET utf8 COLLATE utf8_general_ci NULL;

ALTER TABLE `Currencies` MODIFY COLUMN `Name` varchar(4000) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL;

ALTER TABLE `Cultures` MODIFY COLUMN `PersonWordNeuter` varchar(255) CHARACTER SET utf8 COLLATE utf8_general_ci NULL;

ALTER TABLE `Cultures` MODIFY COLUMN `PersonWordMale` varchar(255) CHARACTER SET utf8 COLLATE utf8_general_ci NULL;

ALTER TABLE `Cultures` MODIFY COLUMN `PersonWordIndeterminate` varchar(255) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL;

ALTER TABLE `Cultures` MODIFY COLUMN `PersonWordFemale` varchar(255) CHARACTER SET utf8 COLLATE utf8_general_ci NULL;

ALTER TABLE `Cultures` MODIFY COLUMN `Name` varchar(255) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL;

ALTER TABLE `Cultures` MODIFY COLUMN `Description` varchar(4000) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL;

ALTER TABLE `CultureInfos` MODIFY COLUMN `DisplayName` varchar(4000) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL;

ALTER TABLE `CultureInfos` MODIFY COLUMN `Id` varchar(50) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL;

ALTER TABLE `Crimes` MODIFY COLUMN `TimeOfReport` varchar(200) CHARACTER SET utf8 COLLATE utf8_general_ci NULL;

ALTER TABLE `Crimes` MODIFY COLUMN `TimeOfCrime` varchar(200) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL;

ALTER TABLE `Crimes` MODIFY COLUMN `CriminalShortDescription` varchar(200) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL;

ALTER TABLE `Crimes` MODIFY COLUMN `CriminalFullDescription` varchar(1000) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL;

ALTER TABLE `Crimes` MODIFY COLUMN `CriminalCharacteristics` varchar(500) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL;

ALTER TABLE `CraftTools` MODIFY COLUMN `ToolType` varchar(45) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL;

ALTER TABLE `CraftTools` MODIFY COLUMN `Definition` text CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL;

ALTER TABLE `Crafts` MODIFY COLUMN `QualityFormula` text CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL;

ALTER TABLE `Crafts` MODIFY COLUMN `Name` varchar(200) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL;

ALTER TABLE `Crafts` MODIFY COLUMN `Category` varchar(200) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL;

ALTER TABLE `Crafts` MODIFY COLUMN `Blurb` text CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL;

ALTER TABLE `Crafts` MODIFY COLUMN `ActiveCraftItemSDesc` varchar(200) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL DEFAULT 'a craft in progress';

ALTER TABLE `Crafts` MODIFY COLUMN `ActionDescription` varchar(200) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL;

ALTER TABLE `CraftProducts` MODIFY COLUMN `ProductType` varchar(45) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL;

ALTER TABLE `CraftProducts` MODIFY COLUMN `Definition` text CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL;

ALTER TABLE `CraftPhases` MODIFY COLUMN `FailEcho` text CHARACTER SET utf8 COLLATE utf8_general_ci NULL;

ALTER TABLE `CraftPhases` MODIFY COLUMN `Echo` text CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL;

ALTER TABLE `CraftInputs` MODIFY COLUMN `InputType` varchar(45) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL;

ALTER TABLE `CraftInputs` MODIFY COLUMN `Definition` text CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL;

ALTER TABLE `CorpseModels` MODIFY COLUMN `Type` varchar(45) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL;

ALTER TABLE `CorpseModels` MODIFY COLUMN `Name` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NULL;

ALTER TABLE `CorpseModels` MODIFY COLUMN `Description` text CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NULL;

ALTER TABLE `CorpseModels` MODIFY COLUMN `Definition` text CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NULL;

ALTER TABLE `CombatMessages` MODIFY COLUMN `Message` text CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL;

ALTER TABLE `CombatMessages` MODIFY COLUMN `FailureMessage` text CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL;

ALTER TABLE `Colours` MODIFY COLUMN `Name` varchar(50) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL;

ALTER TABLE `Colours` MODIFY COLUMN `Fancy` varchar(4000) CHARACTER SET utf8 COLLATE utf8_general_ci NULL;

ALTER TABLE `Coins` MODIFY COLUMN `ShortDescription` varchar(4000) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL;

ALTER TABLE `Coins` MODIFY COLUMN `PluralWord` varchar(4000) CHARACTER SET utf8 COLLATE utf8_general_ci NULL;

ALTER TABLE `Coins` MODIFY COLUMN `Name` varchar(4000) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL;

ALTER TABLE `Coins` MODIFY COLUMN `GeneralForm` varchar(4000) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL;

ALTER TABLE `Coins` MODIFY COLUMN `FullDescription` varchar(4000) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL;

ALTER TABLE `Clocks` MODIFY COLUMN `Definition` text CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL;

ALTER TABLE `ClimateModels` MODIFY COLUMN `Type` varchar(45) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL;

ALTER TABLE `ClimateModels` MODIFY COLUMN `Name` varchar(200) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL;

ALTER TABLE `ClimateModels` MODIFY COLUMN `Definition` mediumtext CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL;

ALTER TABLE `Clans` MODIFY COLUMN `PayIntervalReferenceTime` varchar(100) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL;

ALTER TABLE `Clans` MODIFY COLUMN `PayIntervalReferenceDate` varchar(100) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL;

ALTER TABLE `Clans` MODIFY COLUMN `Name` varchar(100) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL;

ALTER TABLE `Clans` MODIFY COLUMN `FullName` varchar(4000) CHARACTER SET utf8 COLLATE utf8_general_ci NULL;

ALTER TABLE `Clans` MODIFY COLUMN `Description` varchar(4000) CHARACTER SET utf8 COLLATE utf8_general_ci NULL;

ALTER TABLE `Clans` MODIFY COLUMN `Alias` varchar(100) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL;

ALTER TABLE `ClanMemberships` MODIFY COLUMN `PersonalName` text CHARACTER SET utf8 COLLATE utf8_general_ci NULL;

ALTER TABLE `ClanMemberships` MODIFY COLUMN `JoinDate` varchar(100) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL;

ALTER TABLE `CheckTemplates` MODIFY COLUMN `Name` varchar(4000) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL;

ALTER TABLE `CheckTemplates` MODIFY COLUMN `Definition` text CHARACTER SET utf8 COLLATE utf8_general_ci NULL;

ALTER TABLE `CheckTemplates` MODIFY COLUMN `CheckMethod` varchar(25) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL DEFAULT 'Standard';

ALTER TABLE `ChargenScreenStoryboards` MODIFY COLUMN `StageDefinition` mediumtext CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL;

ALTER TABLE `Chargens` MODIFY COLUMN `Name` varchar(12000) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL;

ALTER TABLE `Chargens` MODIFY COLUMN `Definition` text CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL;

ALTER TABLE `ChargenRoles` MODIFY COLUMN `Name` varchar(4000) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL;

ALTER TABLE `ChargenRoles` MODIFY COLUMN `ChargenBlurb` varchar(4000) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL;

ALTER TABLE `ChargenResources` MODIFY COLUMN `Type` varchar(50) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL;

ALTER TABLE `ChargenResources` MODIFY COLUMN `TextDisplayedToPlayerOnDeduct` varchar(4000) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL;

ALTER TABLE `ChargenResources` MODIFY COLUMN `TextDisplayedToPlayerOnAward` varchar(4000) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL;

ALTER TABLE `ChargenResources` MODIFY COLUMN `PluralName` varchar(50) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL;

ALTER TABLE `ChargenResources` MODIFY COLUMN `Name` varchar(50) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL;

ALTER TABLE `ChargenResources` MODIFY COLUMN `MaximumResourceFormula` varchar(4000) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL;

ALTER TABLE `ChargenResources` MODIFY COLUMN `Alias` varchar(50) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL;

ALTER TABLE `ChargenAdvices` MODIFY COLUMN `AdviceTitle` text CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL;

ALTER TABLE `ChargenAdvices` MODIFY COLUMN `AdviceText` text CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL;

ALTER TABLE `Characters` MODIFY COLUMN `ShortTermPlan` text CHARACTER SET utf8 COLLATE utf8_general_ci NULL;

ALTER TABLE `Characters` MODIFY COLUMN `PositionTargetType` varchar(45) CHARACTER SET utf8 COLLATE utf8_general_ci NULL;

ALTER TABLE `Characters` MODIFY COLUMN `PositionEmote` text CHARACTER SET utf8 COLLATE utf8_general_ci NULL;

ALTER TABLE `Characters` MODIFY COLUMN `Outfits` text CHARACTER SET utf8 COLLATE utf8_general_ci NULL;

ALTER TABLE `Characters` MODIFY COLUMN `NeedsModel` varchar(50) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL DEFAULT 'NoNeeds';

ALTER TABLE `Characters` MODIFY COLUMN `NameInfo` text CHARACTER SET utf8 COLLATE utf8_general_ci NULL;

ALTER TABLE `Characters` MODIFY COLUMN `Name` varchar(4000) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL;

ALTER TABLE `Characters` MODIFY COLUMN `LongTermPlan` text CHARACTER SET utf8 COLLATE utf8_general_ci NULL;

ALTER TABLE `Characters` MODIFY COLUMN `IntroductionMessage` text CHARACTER SET utf8 COLLATE utf8_general_ci NULL;

ALTER TABLE `Characters` MODIFY COLUMN `EffectData` mediumtext CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL;

ALTER TABLE `Characters` MODIFY COLUMN `BirthdayDate` varchar(4000) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL;

ALTER TABLE `CharacterLog` MODIFY COLUMN `Command` text CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL;

ALTER TABLE `CharacterKnowledges` MODIFY COLUMN `HowAcquired` varchar(200) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL;

ALTER TABLE `CharacteristicValues` MODIFY COLUMN `Value` varchar(50) CHARACTER SET utf8 COLLATE utf8_general_ci NULL;

ALTER TABLE `CharacteristicValues` MODIFY COLUMN `Name` varchar(4000) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL;

ALTER TABLE `CharacteristicValues` MODIFY COLUMN `AdditionalValue` varchar(4000) CHARACTER SET utf8 COLLATE utf8_general_ci NULL;

ALTER TABLE `CharacteristicProfiles` MODIFY COLUMN `Type` varchar(50) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL;

ALTER TABLE `CharacteristicProfiles` MODIFY COLUMN `Name` varchar(50) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL;

ALTER TABLE `CharacteristicProfiles` MODIFY COLUMN `Description` varchar(4000) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL;

ALTER TABLE `CharacteristicProfiles` MODIFY COLUMN `Definition` text CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL;

ALTER TABLE `CharacteristicDefinitions` MODIFY COLUMN `Pattern` varchar(4000) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL;

ALTER TABLE `CharacteristicDefinitions` MODIFY COLUMN `Name` varchar(4000) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL;

ALTER TABLE `CharacteristicDefinitions` MODIFY COLUMN `Model` varchar(45) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL DEFAULT 'standard';

ALTER TABLE `CharacteristicDefinitions` MODIFY COLUMN `Description` varchar(4000) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL;

ALTER TABLE `CharacteristicDefinitions` MODIFY COLUMN `Definition` text CHARACTER SET utf8 COLLATE utf8_general_ci NULL;

ALTER TABLE `CharacterIntroTemplates` MODIFY COLUMN `Name` varchar(200) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL;

ALTER TABLE `CharacterIntroTemplates` MODIFY COLUMN `Definition` text CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL;

ALTER TABLE `CharacterCombatSettings` MODIFY COLUMN `Name` text CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL;

ALTER TABLE `CharacterCombatSettings` MODIFY COLUMN `MeleeAttackOrderPreference` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL DEFAULT '0 1 2 3 4';

ALTER TABLE `CharacterCombatSettings` MODIFY COLUMN `Description` text CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL;

ALTER TABLE `CharacterCombatSettings` MODIFY COLUMN `ClassificationsAllowed` text CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL;

ALTER TABLE `Channels` MODIFY COLUMN `ChannelName` varchar(4000) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL;

ALTER TABLE `Channels` MODIFY COLUMN `ChannelColour` char(10) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL;

ALTER TABLE `ChannelCommandWords` MODIFY COLUMN `Word` varchar(50) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL;

ALTER TABLE `Cells_ForagableYields` MODIFY COLUMN `ForagableType` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL;

ALTER TABLE `Cells` MODIFY COLUMN `EffectData` mediumtext CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL;

ALTER TABLE `CellOverlays` MODIFY COLUMN `Name` varchar(4000) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL;

ALTER TABLE `CellOverlays` MODIFY COLUMN `CellName` varchar(4000) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL;

ALTER TABLE `CellOverlays` MODIFY COLUMN `CellDescription` varchar(4000) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL;

ALTER TABLE `CellOverlays` MODIFY COLUMN `AtmosphereType` varchar(45) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL DEFAULT 'gas';

ALTER TABLE `CellOverlayPackages` MODIFY COLUMN `Name` varchar(4000) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL;

ALTER TABLE `Celestials` MODIFY COLUMN `Definition` text CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL;

ALTER TABLE `Calendars` MODIFY COLUMN `Definition` text CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL;

ALTER TABLE `Calendars` MODIFY COLUMN `Date` varchar(50) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL;

ALTER TABLE `ButcheryProducts` MODIFY COLUMN `Subcategory` varchar(100) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL;

ALTER TABLE `ButcheryProducts` MODIFY COLUMN `Name` varchar(100) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL;

ALTER TABLE `ButcheryProductItems` MODIFY COLUMN `ButcheryProductItemscol` varchar(45) CHARACTER SET utf8 COLLATE utf8_general_ci NULL;

ALTER TABLE `BodyProtos_AdditionalBodyparts` MODIFY COLUMN `Usage` varchar(50) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL;

ALTER TABLE `BodyProtos` MODIFY COLUMN `WielderDescriptionSingle` varchar(4000) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL DEFAULT 'hand';

ALTER TABLE `BodyProtos` MODIFY COLUMN `WielderDescriptionPlural` varchar(4000) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL DEFAULT 'hands';

ALTER TABLE `BodyProtos` MODIFY COLUMN `Name` varchar(4000) CHARACTER SET utf8 COLLATE utf8_general_ci NULL;

ALTER TABLE `BodyProtos` MODIFY COLUMN `LegDescriptionSingular` varchar(1000) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL DEFAULT 'leg';

ALTER TABLE `BodyProtos` MODIFY COLUMN `LegDescriptionPlural` varchar(1000) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL DEFAULT 'legs';

ALTER TABLE `BodyProtos` MODIFY COLUMN `ConsiderString` varchar(4000) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL;

ALTER TABLE `BodypartShape` MODIFY COLUMN `Name` varchar(4000) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL;

ALTER TABLE `BodypartProto` MODIFY COLUMN `Name` varchar(4000) CHARACTER SET utf8 COLLATE utf8_general_ci NULL;

ALTER TABLE `BodypartProto` MODIFY COLUMN `Description` varchar(4000) CHARACTER SET utf8 COLLATE utf8_general_ci NULL;

ALTER TABLE `BodypartInternalInfos` MODIFY COLUMN `ProximityGroup` varchar(45) CHARACTER SET utf8 COLLATE utf8_general_ci NULL;

ALTER TABLE `BodypartGroupDescribers` MODIFY COLUMN `Type` varchar(50) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL;

ALTER TABLE `BodypartGroupDescribers` MODIFY COLUMN `DescribedAs` varchar(4000) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL;

ALTER TABLE `BodypartGroupDescribers` MODIFY COLUMN `Comment` varchar(4000) CHARACTER SET utf8 COLLATE utf8_general_ci NULL;

ALTER TABLE `Bodies` MODIFY COLUMN `Tattoos` mediumtext CHARACTER SET utf8 COLLATE utf8_general_ci NULL;

ALTER TABLE `Bodies` MODIFY COLUMN `ShortDescription` varchar(1000) CHARACTER SET utf8 COLLATE utf8_general_ci NULL;

ALTER TABLE `Bodies` MODIFY COLUMN `Scars` mediumtext CHARACTER SET utf8 COLLATE utf8_general_ci NULL;

ALTER TABLE `Bodies` MODIFY COLUMN `FullDescription` varchar(4000) CHARACTER SET utf8 COLLATE utf8_general_ci NULL;

ALTER TABLE `Bodies` MODIFY COLUMN `EffectData` mediumtext CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL;

ALTER TABLE `Boards` MODIFY COLUMN `Name` varchar(45) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL;

ALTER TABLE `BoardPosts` MODIFY COLUMN `Title` varchar(200) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL;

ALTER TABLE `BoardPosts` MODIFY COLUMN `Content` text CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL;

ALTER TABLE `Bloodtypes` MODIFY COLUMN `Name` varchar(200) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL;

ALTER TABLE `BloodtypeAntigens` MODIFY COLUMN `Name` varchar(200) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL;

ALTER TABLE `BloodModels` MODIFY COLUMN `Name` varchar(200) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL;

ALTER TABLE `Bans` MODIFY COLUMN `Reason` varchar(4000) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL;

ALTER TABLE `Bans` MODIFY COLUMN `IpMask` varchar(200) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL;

ALTER TABLE `AutobuilderRoomTemplates` MODIFY COLUMN `TemplateType` varchar(45) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL;

ALTER TABLE `AutobuilderRoomTemplates` MODIFY COLUMN `Name` varchar(100) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL;

ALTER TABLE `AutobuilderRoomTemplates` MODIFY COLUMN `Definition` longtext CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL;

ALTER TABLE `AutobuilderAreaTemplates` MODIFY COLUMN `TemplateType` varchar(45) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL;

ALTER TABLE `AutobuilderAreaTemplates` MODIFY COLUMN `Name` varchar(100) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL;

ALTER TABLE `AutobuilderAreaTemplates` MODIFY COLUMN `Definition` longtext CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL;

ALTER TABLE `AuthorityGroups` MODIFY COLUMN `Name` varchar(4000) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL;

ALTER TABLE `ArtificialIntelligences` MODIFY COLUMN `Type` varchar(4000) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL;

ALTER TABLE `ArtificialIntelligences` MODIFY COLUMN `Name` varchar(4000) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL;

ALTER TABLE `ArtificialIntelligences` MODIFY COLUMN `Definition` mediumtext CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL;

ALTER TABLE `ArmourTypes` MODIFY COLUMN `Name` varchar(200) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL;

ALTER TABLE `ArmourTypes` MODIFY COLUMN `Definition` text CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL;

ALTER TABLE `Areas` MODIFY COLUMN `Name` varchar(200) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL;

ALTER TABLE `Appointments_Titles` MODIFY COLUMN `Title` varchar(200) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL;

ALTER TABLE `Appointments_Abbreviations` MODIFY COLUMN `Abbreviation` varchar(50) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL;

ALTER TABLE `Appointments` MODIFY COLUMN `Name` varchar(100) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL;

ALTER TABLE `AmmunitionTypes` MODIFY COLUMN `StunExpression` text CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL;

ALTER TABLE `AmmunitionTypes` MODIFY COLUMN `SpecificType` text CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL;

ALTER TABLE `AmmunitionTypes` MODIFY COLUMN `RangedWeaponTypes` varchar(200) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL;

ALTER TABLE `AmmunitionTypes` MODIFY COLUMN `PainExpression` text CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL;

ALTER TABLE `AmmunitionTypes` MODIFY COLUMN `Name` varchar(200) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL;

ALTER TABLE `AmmunitionTypes` MODIFY COLUMN `DamageExpression` text CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL;

ALTER TABLE `Accounts` MODIFY COLUMN `UnitPreference` varchar(50) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL;

ALTER TABLE `Accounts` MODIFY COLUMN `TimeZoneId` varchar(100) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL;

ALTER TABLE `Accounts` MODIFY COLUMN `RegistrationCode` varchar(50) CHARACTER SET utf8 COLLATE utf8_general_ci NULL;

ALTER TABLE `Accounts` MODIFY COLUMN `RecoveryCode` varchar(4000) CHARACTER SET utf8 COLLATE utf8_general_ci NULL;

ALTER TABLE `Accounts` MODIFY COLUMN `Password` varchar(1000) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NULL;

ALTER TABLE `Accounts` MODIFY COLUMN `Name` varchar(4000) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL;

ALTER TABLE `Accounts` MODIFY COLUMN `LastLoginIP` varchar(50) CHARACTER SET utf8 COLLATE utf8_general_ci NULL;

ALTER TABLE `Accounts` MODIFY COLUMN `Email` varchar(4000) CHARACTER SET utf8 COLLATE utf8_general_ci NULL;

ALTER TABLE `Accounts` MODIFY COLUMN `CultureName` varchar(50) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL;

ALTER TABLE `Accounts` ADD `ActLawfully` bit(1) NOT NULL DEFAULT b'1';

ALTER TABLE `AccountNotes` MODIFY COLUMN `Text` text CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL;

ALTER TABLE `AccountNotes` MODIFY COLUMN `Subject` varchar(4000) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL;

ALTER TABLE `Accents` MODIFY COLUMN `VagueSuffix` varchar(4000) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL;

ALTER TABLE `Accents` MODIFY COLUMN `Suffix` varchar(4000) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL;

ALTER TABLE `Accents` MODIFY COLUMN `Name` varchar(50) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL;

ALTER TABLE `Accents` MODIFY COLUMN `Group` varchar(50) CHARACTER SET utf8 COLLATE utf8_general_ci NULL;

ALTER TABLE `Accents` MODIFY COLUMN `Description` varchar(4000) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL;

CREATE TABLE `LegalAuthoritiyCells` (
    `LegalAuthorityId` bigint(20) NOT NULL,
    `CellId` bigint(20) NOT NULL,
    CONSTRAINT `PRIMARY` PRIMARY KEY (`LegalAuthorityId`, `CellId`),
    CONSTRAINT `FK_LegalAuthoritiesCells_Cells` FOREIGN KEY (`CellId`) REFERENCES `Cells` (`Id`) ON DELETE CASCADE,
    CONSTRAINT `FK_LegalAuthoritiesCells_LegalAuthorities` FOREIGN KEY (`LegalAuthorityId`) REFERENCES `LegalAuthorities` (`Id`) ON DELETE CASCADE
);

CREATE TABLE `Patrols` (
    `Id` bigint(20) NOT NULL AUTO_INCREMENT,
    `PatrolRouteId` bigint(20) NOT NULL,
    `LegalAuthorityId` bigint(20) NOT NULL,
    `PatrolPhase` int(11) NOT NULL,
    `LastMajorNodeId` bigint(20) NULL,
    `NextMajorNodeId` bigint(20) NULL,
    `PatrolLeaderId` bigint(20) NULL,
    `CharacterId` bigint(20) NULL,
    CONSTRAINT `PK_Patrols` PRIMARY KEY (`Id`),
    CONSTRAINT `FK_Patrols_Characters` FOREIGN KEY (`PatrolLeaderId`) REFERENCES `Characters` (`Id`) ON DELETE RESTRICT,
    CONSTRAINT `FK_Patrols_Characters_CharacterId` FOREIGN KEY (`CharacterId`) REFERENCES `Characters` (`Id`) ON DELETE RESTRICT,
    CONSTRAINT `FK_Patrols_LastMajorNode` FOREIGN KEY (`LastMajorNodeId`) REFERENCES `Cells` (`Id`) ON DELETE RESTRICT,
    CONSTRAINT `FK_Patrols_LegalAuthorities` FOREIGN KEY (`LegalAuthorityId`) REFERENCES `LegalAuthorities` (`Id`) ON DELETE CASCADE,
    CONSTRAINT `FK_Patrols_NextMajorNode` FOREIGN KEY (`NextMajorNodeId`) REFERENCES `Cells` (`Id`) ON DELETE RESTRICT,
    CONSTRAINT `FK_Patrols_PatrolRoutes` FOREIGN KEY (`PatrolRouteId`) REFERENCES `PatrolRoutes` (`Id`) ON DELETE CASCADE
);

CREATE TABLE `PatrolMembers` (
    `PatrolId` bigint(20) NOT NULL,
    `CharacterId` bigint(20) NOT NULL,
    CONSTRAINT `PRIMARY` PRIMARY KEY (`PatrolId`, `CharacterId`),
    CONSTRAINT `FK_PatrolMembers_Characters` FOREIGN KEY (`CharacterId`) REFERENCES `Characters` (`Id`) ON DELETE CASCADE,
    CONSTRAINT `FK_PatrolsMembers_Patrols` FOREIGN KEY (`PatrolId`) REFERENCES `Patrols` (`Id`) ON DELETE CASCADE
);

CREATE INDEX `IX_PatrolRoutes_StartPatrolProgId` ON `PatrolRoutes` (`StartPatrolProgId`);

CREATE INDEX `FK_LegalAuthorities_MarshallingCells_idx` ON `LegalAuthorities` (`MarshallingLocationId`);

CREATE INDEX `FK_LegalAuthorities_PreparingCells_idx` ON `LegalAuthorities` (`PreparingLocationId`);

CREATE INDEX `FK_LegalAuthorities_PrisonCells_idx` ON `LegalAuthorities` (`PrisonLocationId`);

CREATE INDEX `FK_LegalAuthorities_StowingCells_idx` ON `LegalAuthorities` (`EnforcerStowingLocationId`);

CREATE INDEX `FK_LegalAuthoritiesCells_Cells_idx` ON `LegalAuthoritiyCells` (`CellId`);

CREATE INDEX `FK_LegalAuthoritiesCells_LegalAuthorities_idx` ON `LegalAuthoritiyCells` (`LegalAuthorityId`);

CREATE INDEX `IX_PatrolMembers_CharacterId` ON `PatrolMembers` (`CharacterId`);

CREATE INDEX `FK_Patrols_Characters_idx` ON `Patrols` (`PatrolLeaderId`);

CREATE INDEX `FK_Patrols_LastMajorNode_idx` ON `Patrols` (`LastMajorNodeId`);

CREATE INDEX `FK_Patrols_LegalAuthorities_idx` ON `Patrols` (`LegalAuthorityId`);

CREATE INDEX `FK_Patrols_NextMajorNode_idx` ON `Patrols` (`NextMajorNodeId`);

CREATE INDEX `FK_Patrols_PatrolRoutes_idx` ON `Patrols` (`PatrolRouteId`);

CREATE INDEX `IX_Patrols_CharacterId` ON `Patrols` (`CharacterId`);

ALTER TABLE `LegalAuthorities` ADD CONSTRAINT `FK_LegalAuthorities_MarshallingCells` FOREIGN KEY (`MarshallingLocationId`) REFERENCES `Cells` (`Id`) ON DELETE RESTRICT;

ALTER TABLE `LegalAuthorities` ADD CONSTRAINT `FK_LegalAuthorities_PreparingCells` FOREIGN KEY (`PreparingLocationId`) REFERENCES `Cells` (`Id`) ON DELETE RESTRICT;

ALTER TABLE `LegalAuthorities` ADD CONSTRAINT `FK_LegalAuthorities_PrisonCells` FOREIGN KEY (`PrisonLocationId`) REFERENCES `Cells` (`Id`) ON DELETE RESTRICT;

ALTER TABLE `LegalAuthorities` ADD CONSTRAINT `FK_LegalAuthorities_StowingCells` FOREIGN KEY (`EnforcerStowingLocationId`) REFERENCES `Cells` (`Id`) ON DELETE RESTRICT;

ALTER TABLE `PatrolRoutes` ADD CONSTRAINT `FK_PatrolRoutes_FutureProgs_StartPatrolProgId` FOREIGN KEY (`StartPatrolProgId`) REFERENCES `FutureProgs` (`Id`) ON DELETE RESTRICT;

INSERT INTO `__EFMigrationsHistory` (`MigrationId`, `ProductVersion`)
VALUES ('20201120022913_EnforcermentAndMisc', '9.0.11');

ALTER TABLE `Crimes` ADD `WitnessIds` varchar(1000) NULL;

INSERT INTO `__EFMigrationsHistory` (`MigrationId`, `ProductVersion`)
VALUES ('20201120045951_MinorFixForCrime', '9.0.11');

ALTER TABLE `CellOverlays` ADD `SafeQuit` bit(1) NOT NULL DEFAULT b'1';

INSERT INTO `__EFMigrationsHistory` (`MigrationId`, `ProductVersion`)
VALUES ('20201129225407_SafeQuit', '9.0.11');

ALTER TABLE `AccountNotes` ADD `InGameTimeStamp` varchar(200) CHARACTER SET utf8 COLLATE utf8_general_ci NULL;

ALTER TABLE `AccountNotes` ADD `IsJournalEntry` bit(1) NOT NULL DEFAULT b'0';

INSERT INTO `__EFMigrationsHistory` (`MigrationId`, `ProductVersion`)
VALUES ('20201130014025_JournalUpdates', '9.0.11');

ALTER TABLE `AccountNotes` ADD `CharacterId` bigint(20) NULL;

CREATE INDEX `FK_AccountNotes_Characters_idx` ON `AccountNotes` (`CharacterId`);

ALTER TABLE `AccountNotes` ADD CONSTRAINT `FK_AccountNotes_Characters` FOREIGN KEY (`CharacterId`) REFERENCES `Characters` (`Id`) ON DELETE RESTRICT;

INSERT INTO `__EFMigrationsHistory` (`MigrationId`, `ProductVersion`)
VALUES ('20201130041538_JournalUpdate', '9.0.11');

ALTER TABLE `Drugs` DROP COLUMN `DrugTypes`;

INSERT INTO `__EFMigrationsHistory` (`MigrationId`, `ProductVersion`)
VALUES ('20201201052916_DrugReform', '9.0.11');

INSERT INTO `__EFMigrationsHistory` (`MigrationId`, `ProductVersion`)
VALUES ('20201217051236_Changes', '9.0.11');

CREATE TABLE `GameItemProtoExtraDescriptions` (
    `GameItemProtoId` bigint(20) NOT NULL,
    `GameItemProtoRevisionNumber` int(11) NOT NULL,
    `ApplicabilityProgId` bigint(20) NOT NULL,
    `ShortDescription` varchar(500) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NULL,
    `FullDescription` varchar(2000) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NULL,
    `FullDescriptionAddendum` varchar(500) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NULL,
    `Priority` int(20) NOT NULL,
    CONSTRAINT `PRIMARY` PRIMARY KEY (`GameItemProtoId`, `GameItemProtoRevisionNumber`, `ApplicabilityProgId`),
    CONSTRAINT `FK_GameItemProtoExtraDescriptions_FutureProgs` FOREIGN KEY (`ApplicabilityProgId`) REFERENCES `FutureProgs` (`Id`) ON DELETE CASCADE,
    CONSTRAINT `FK_GameItemProtoExtraDescriptions_GameItemProtos` FOREIGN KEY (`GameItemProtoId`, `GameItemProtoRevisionNumber`) REFERENCES `GameItemProtos` (`Id`, `RevisionNumber`) ON DELETE CASCADE
);

CREATE INDEX `IX_GameItemProtoExtraDescriptions_ApplicabilityProgId` ON `GameItemProtoExtraDescriptions` (`ApplicabilityProgId`);

INSERT INTO `__EFMigrationsHistory` (`MigrationId`, `ProductVersion`)
VALUES ('20201217051726_ExtraDescriptions', '9.0.11');

ALTER TABLE `Races` ADD `BreathingModel` varchar(100) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT 'simple';

INSERT INTO `__EFMigrationsHistory` (`MigrationId`, `ProductVersion`)
VALUES ('20201218014631_RacialBreathingChange', '9.0.11');

ALTER TABLE `Ranks` ADD `FameType` int(11) NOT NULL DEFAULT 0;

ALTER TABLE `Clans` ADD `ShowFamousMembersInNotables` bit(1) NOT NULL DEFAULT b'0';

ALTER TABLE `Clans` ADD `Sphere` varchar(100) CHARACTER SET utf8 COLLATE utf8_general_ci NULL;

ALTER TABLE `Appointments` ADD `FameType` int(11) NOT NULL DEFAULT 0;

INSERT INTO `__EFMigrationsHistory` (`MigrationId`, `ProductVersion`)
VALUES ('20201221031703_ClanFame', '9.0.11');

CREATE TABLE `BodyProtosPositions` (
    `BodyProtoId` bigint(20) NOT NULL,
    `Position` int(11) NOT NULL,
    CONSTRAINT `PK_BodyProtosPositions` PRIMARY KEY (`BodyProtoId`, `Position`),
    CONSTRAINT `FK_BodyProtosPositions_BodyProtos` FOREIGN KEY (`BodyProtoId`) REFERENCES `BodyProtos` (`Id`) ON DELETE CASCADE
);

INSERT INTO `__EFMigrationsHistory` (`MigrationId`, `ProductVersion`)
VALUES ('20201227120935_CantRemember', '9.0.11');

CREATE INDEX `FK_BodyProtosPositions_BodyProtos_idx` ON `BodyProtosPositions` (`BodyProtoId`);

INSERT INTO `__EFMigrationsHistory` (`MigrationId`, `ProductVersion`)
VALUES ('20210113052107_IndexFixForBodyparts', '9.0.11');

ALTER TABLE `BodyProtos_AdditionalBodyparts` RENAME INDEX `FK_BodyProtos_AdditionalBodyparts_BodypartProto` TO `FK_BodyProtos_AdditionalBodyparts_BodypartProto_idx`;

CREATE INDEX `FK_BodyProtos_AdditionalBodyparts_BodyProtos_idx` ON `BodyProtos_AdditionalBodyparts` (`BodyProtoId`);

CREATE INDEX `FK_BodypartInternalInfos_BodypartProtos_idx` ON `BodypartInternalInfos` (`BodypartProtoId`);

INSERT INTO `__EFMigrationsHistory` (`MigrationId`, `ProductVersion`)
VALUES ('20210114010706_IndexAdditions', '9.0.11');

CREATE TABLE `MagicSpells` (
    `Id` bigint(20) NOT NULL AUTO_INCREMENT,
    `Name` varchar(100) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
    `Blurb` text CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
    `Description` text CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
    `SpellKnownProgId` bigint(20) NOT NULL,
    `MagicSchoolId` bigint(20) NOT NULL,
    `ExclusiveDelay` double NOT NULL,
    `NonExclusiveDelay` double NOT NULL,
    `Definition` mediumtext CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
    CONSTRAINT `PK_MagicSpells` PRIMARY KEY (`Id`),
    CONSTRAINT `FK_MagicSpells_Futureprogs` FOREIGN KEY (`SpellKnownProgId`) REFERENCES `FutureProgs` (`Id`) ON DELETE CASCADE,
    CONSTRAINT `FK_MagicSpells_MagicSchools` FOREIGN KEY (`MagicSchoolId`) REFERENCES `MagicSchools` (`Id`) ON DELETE CASCADE
);

CREATE INDEX `FK_MagicSpells_Futureprogs_idx` ON `MagicSpells` (`SpellKnownProgId`);

CREATE INDEX `FK_MagicSpells_MagicSchools_idx` ON `MagicSpells` (`MagicSchoolId`);

INSERT INTO `__EFMigrationsHistory` (`MigrationId`, `ProductVersion`)
VALUES ('20210116210204_MagicSpells', '9.0.11');

ALTER TABLE `MagicSpells` ADD `CastingDifficulty` int(11) NOT NULL DEFAULT 0;

ALTER TABLE `MagicSpells` ADD `CastingTraitDefinitionId` bigint(20) NOT NULL DEFAULT 0;

ALTER TABLE `MagicSpells` ADD `ResistingDifficulty` int(11) NULL;

ALTER TABLE `MagicSpells` ADD `ResistingTraitDefinitionId` bigint(20) NULL;

CREATE INDEX `FK_MagicSpells_TraitDefinitions_Casting_idx` ON `MagicSpells` (`CastingTraitDefinitionId`);

CREATE INDEX `FK_MagicSpells_TraitDefinitions_Resisting_idx` ON `MagicSpells` (`ResistingTraitDefinitionId`);

ALTER TABLE `MagicSpells` ADD CONSTRAINT `FK_MagicSpells_TraitDefinitions_Casting` FOREIGN KEY (`CastingTraitDefinitionId`) REFERENCES `TraitDefinitions` (`Id`) ON DELETE CASCADE;

ALTER TABLE `MagicSpells` ADD CONSTRAINT `FK_MagicSpells_TraitDefinitions_Resisting` FOREIGN KEY (`ResistingTraitDefinitionId`) REFERENCES `TraitDefinitions` (`Id`) ON DELETE RESTRICT;

INSERT INTO `__EFMigrationsHistory` (`MigrationId`, `ProductVersion`)
VALUES ('20210118053537_MoreSpellStuff', '9.0.11');

ALTER TABLE `MagicSpells` ADD `CastingEmote` text CHARACTER SET utf8 COLLATE utf8_general_ci NULL;

ALTER TABLE `MagicSpells` ADD `CastingEmoteFlags` int(11) NOT NULL DEFAULT 0;

ALTER TABLE `MagicSpells` ADD `EffectDurationExpressionId` bigint(20) NULL;

ALTER TABLE `MagicSpells` ADD `FailCastingEmote` text CHARACTER SET utf8 COLLATE utf8_general_ci NULL;

ALTER TABLE `MagicSpells` ADD `TargetEmote` text CHARACTER SET utf8 COLLATE utf8_general_ci NULL;

ALTER TABLE `MagicSpells` ADD `TargetEmoteFlags` int(11) NOT NULL DEFAULT 0;

ALTER TABLE `MagicSpells` ADD `TargetResistedEmote` text CHARACTER SET utf8 COLLATE utf8_general_ci NULL;

CREATE INDEX `FK_MagicSpells_TraitExpressions_idx` ON `MagicSpells` (`EffectDurationExpressionId`);

ALTER TABLE `MagicSpells` ADD CONSTRAINT `FK_MagicSpells_TraitExpressions` FOREIGN KEY (`EffectDurationExpressionId`) REFERENCES `TraitExpression` (`Id`) ON DELETE RESTRICT;

INSERT INTO `__EFMigrationsHistory` (`MigrationId`, `ProductVersion`)
VALUES ('20210119034150_MoreSpellStuff2', '9.0.11');

ALTER TABLE `MagicSpells` DROP FOREIGN KEY `FK_MagicSpells_TraitDefinitions_Casting`;

ALTER TABLE `MagicSpells` MODIFY COLUMN `CastingTraitDefinitionId` bigint(20) NULL;

ALTER TABLE `MagicSpells` ADD CONSTRAINT `FK_MagicSpells_TraitDefinitions_Casting` FOREIGN KEY (`CastingTraitDefinitionId`) REFERENCES `TraitDefinitions` (`Id`) ON DELETE RESTRICT;

INSERT INTO `__EFMigrationsHistory` (`MigrationId`, `ProductVersion`)
VALUES ('20210119035740_MoreSpellStuff3', '9.0.11');

ALTER TABLE `MagicSpells` ADD `MinimumSuccessThreshold` int(11) NOT NULL DEFAULT 4;

INSERT INTO `__EFMigrationsHistory` (`MigrationId`, `ProductVersion`)
VALUES ('20210120031933_MoreSpellStuff4', '9.0.11');

DROP TABLE `EnforcementAuthorities_ArrestableClasses`;

ALTER TABLE `PatrolRoutes` ADD `IsReady` bit(1) NOT NULL DEFAULT b'0';

ALTER TABLE `LegalAuthorities` ADD `OnImprisonProgId` bigint(20) NULL;

ALTER TABLE `LegalAuthorities` ADD `OnReleaseProgId` bigint(20) NULL;

ALTER TABLE `LegalAuthorities` ADD `PrisonBelongingsLocationId` bigint(20) NULL;

ALTER TABLE `LegalAuthorities` ADD `PrisonReleaseLocationId` bigint(20) NULL;

CREATE TABLE `EnforcementAuthoritiesArrestableClasses` (
    `EnforcementAuthorityId` bigint(20) NOT NULL,
    `LegalClassId` bigint(20) NOT NULL,
    CONSTRAINT `PK_EnforcementAuthoritiesArrestableClasses` PRIMARY KEY (`EnforcementAuthorityId`, `LegalClassId`),
    CONSTRAINT `FK_EnforcementAuthoritiesArrestableLegalClasses_Enforce` FOREIGN KEY (`EnforcementAuthorityId`) REFERENCES `EnforcementAuthorities` (`Id`) ON DELETE CASCADE,
    CONSTRAINT `FK_EnforcementAuthoritiesArrestableLegalClasses_LegalClasses` FOREIGN KEY (`LegalClassId`) REFERENCES `LegalClasses` (`Id`) ON DELETE CASCADE
);

CREATE INDEX `FK_LegalAuthorities_FutureprogsImprison_idx` ON `LegalAuthorities` (`OnImprisonProgId`);

CREATE INDEX `FK_LegalAuthorities_FutureprogsRelease_idx` ON `LegalAuthorities` (`OnReleaseProgId`);

CREATE INDEX `FK_LegalAuthorities_PrisonBelongingsCells_idx` ON `LegalAuthorities` (`PrisonBelongingsLocationId`);

CREATE INDEX `FK_LegalAuthorities_PrisonReleaseCells_idx` ON `LegalAuthorities` (`PrisonReleaseLocationId`);

CREATE INDEX `FK_EnforcementAuthoritiesArrestableLegalClasses_Enforce_idx` ON `EnforcementAuthoritiesArrestableClasses` (`EnforcementAuthorityId`);

CREATE INDEX `FK_EnforcementAuthoritiesArrestableLegalClasses_LegalClasses_idx` ON `EnforcementAuthoritiesArrestableClasses` (`LegalClassId`);

ALTER TABLE `LegalAuthorities` ADD CONSTRAINT `FK_LegalAuthorities_FutureprogsImprison` FOREIGN KEY (`OnImprisonProgId`) REFERENCES `FutureProgs` (`Id`) ON DELETE RESTRICT;

ALTER TABLE `LegalAuthorities` ADD CONSTRAINT `FK_LegalAuthorities_FutureprogsRelease` FOREIGN KEY (`OnReleaseProgId`) REFERENCES `FutureProgs` (`Id`) ON DELETE RESTRICT;

ALTER TABLE `LegalAuthorities` ADD CONSTRAINT `FK_LegalAuthorities_PrisonBelongingsCells` FOREIGN KEY (`PrisonBelongingsLocationId`) REFERENCES `Cells` (`Id`) ON DELETE RESTRICT;

ALTER TABLE `LegalAuthorities` ADD CONSTRAINT `FK_LegalAuthorities_PrisonReleaseCells` FOREIGN KEY (`PrisonReleaseLocationId`) REFERENCES `Cells` (`Id`) ON DELETE RESTRICT;

INSERT INTO `__EFMigrationsHistory` (`MigrationId`, `ProductVersion`)
VALUES ('20210127032929_Jan21EnforcementWorkaround', '9.0.11');

ALTER TABLE `Clans` DROP FOREIGN KEY `FK_Clans_Parent`;

ALTER TABLE `Clans` RENAME COLUMN `ParentClanId` TO `ClanId`;

ALTER TABLE `Clans` RENAME INDEX `FK_Clans_Parent` TO `IX_Clans_ClanId`;

ALTER TABLE `Clans` ADD CONSTRAINT `FK_Clans_Clans_ClanId` FOREIGN KEY (`ClanId`) REFERENCES `Clans` (`Id`) ON DELETE RESTRICT;

INSERT INTO `__EFMigrationsHistory` (`MigrationId`, `ProductVersion`)
VALUES ('20210202002906_RemovingChildClans', '9.0.11');

ALTER TABLE `Accounts` ADD `HasBeenActiveInWeek` bit(1) NOT NULL DEFAULT b'0';

CREATE TABLE `PlayerActivitySnapshots` (
    `Id` bigint(20) NOT NULL AUTO_INCREMENT,
    `DateTime` datetime NOT NULL,
    `OnlinePlayers` int(11) NOT NULL,
    `OnlineAdmins` int(11) NOT NULL,
    `AvailableAdmins` int(11) NOT NULL,
    `IdlePlayers` int(11) NOT NULL,
    `UniquePCLocations` int(11) NOT NULL,
    `OnlineGuests` int(11) NOT NULL,
    CONSTRAINT `PRIMARY` PRIMARY KEY (`Id`)
);

CREATE TABLE `WeeklyStatistics` (
    `Id` bigint(20) NOT NULL AUTO_INCREMENT,
    `Start` datetime NOT NULL,
    `End` datetime NOT NULL,
    `TotalAccounts` int(11) NOT NULL,
    `ActiveAccounts` int(11) NOT NULL,
    `NewAccounts` int(11) NOT NULL,
    `ApplicationsSubmitted` int(11) NOT NULL,
    `ApplicationsApproved` int(11) NOT NULL,
    `PlayerDeaths` int(11) NOT NULL,
    `NonPlayerDeaths` int(11) NOT NULL,
    CONSTRAINT `PRIMARY` PRIMARY KEY (`Id`)
);

INSERT INTO `__EFMigrationsHistory` (`MigrationId`, `ProductVersion`)
VALUES ('20210211035327_GameStatistics', '9.0.11');

ALTER TABLE `Celestials` ADD `CelestialType` varchar(30) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL DEFAULT 'OldSun';

INSERT INTO `__EFMigrationsHistory` (`MigrationId`, `ProductVersion`)
VALUES ('20210224105856_NewSun', '9.0.11');

ALTER TABLE `CharacteristicValues` ADD `OngoingValidityProgId` bigint(20) NULL;

CREATE INDEX `IX_CharacteristicValues_OngoingValidityProgId` ON `CharacteristicValues` (`OngoingValidityProgId`);

ALTER TABLE `CharacteristicValues` ADD CONSTRAINT `FK_CharacteristicValues_FutureProgs_Ongoing` FOREIGN KEY (`OngoingValidityProgId`) REFERENCES `FutureProgs` (`Id`) ON DELETE SET NULL;

INSERT INTO `__EFMigrationsHistory` (`MigrationId`, `ProductVersion`)
VALUES ('20210302112347_OngoingCheckForCharacteristics', '9.0.11');

ALTER TABLE `TimeZoneInfos` MODIFY COLUMN `Order` decimal(58,29) NOT NULL;

ALTER TABLE `ShopTransactionRecords` MODIFY COLUMN `Tax` decimal(58,29) NOT NULL;

ALTER TABLE `ShopTransactionRecords` MODIFY COLUMN `PretaxValue` decimal(58,29) NOT NULL;

ALTER TABLE `ShopFinancialPeriodResults` MODIFY COLUMN `SalesTax` decimal(58,29) NOT NULL;

ALTER TABLE `ShopFinancialPeriodResults` MODIFY COLUMN `ProfitsTax` decimal(58,29) NOT NULL;

ALTER TABLE `ShopFinancialPeriodResults` MODIFY COLUMN `NetRevenue` decimal(58,29) NOT NULL;

ALTER TABLE `ShopFinancialPeriodResults` MODIFY COLUMN `GrossRevenue` decimal(58,29) NOT NULL;

ALTER TABLE `Paygrades` MODIFY COLUMN `PayAmount` decimal(58,29) NOT NULL;

ALTER TABLE `Merchandises` MODIFY COLUMN `BasePrice` decimal(58,29) NOT NULL;

ALTER TABLE `Merchandises` MODIFY COLUMN `AutoReorderPrice` decimal(58,29) NOT NULL;

ALTER TABLE `LineOfCreditAccountUsers` MODIFY COLUMN `SpendingLimit` decimal(58,29) NULL;

ALTER TABLE `LineOfCreditAccounts` MODIFY COLUMN `OutstandingBalance` decimal(58,29) NOT NULL;

ALTER TABLE `LineOfCreditAccounts` MODIFY COLUMN `AccountLimit` decimal(58,29) NOT NULL;

ALTER TABLE `Laws` MODIFY COLUMN `StandardFine` decimal(58,29) NOT NULL;

ALTER TABLE `Laws` MODIFY COLUMN `MinimumFine` decimal(58,29) NOT NULL;

ALTER TABLE `Laws` MODIFY COLUMN `MaximumFine` decimal(58,29) NOT NULL;

ALTER TABLE `EconomicZoneShopTaxes` MODIFY COLUMN `TaxesInCredits` decimal(58,29) NOT NULL;

ALTER TABLE `EconomicZoneShopTaxes` MODIFY COLUMN `OutstandingSalesTaxes` decimal(58,29) NOT NULL;

ALTER TABLE `EconomicZoneShopTaxes` MODIFY COLUMN `OutstandingProfitTaxes` decimal(58,29) NOT NULL;

ALTER TABLE `EconomicZones` MODIFY COLUMN `TotalRevenueHeld` decimal(58,29) NOT NULL DEFAULT 0;

ALTER TABLE `EconomicZones` MODIFY COLUMN `OutstandingTaxesOwed` decimal(58,29) NOT NULL;

ALTER TABLE `EconomicZoneRevenues` MODIFY COLUMN `TotalTaxRevenue` decimal(58,29) NOT NULL;

ALTER TABLE `CurrencyDivisions` MODIFY COLUMN `BaseUnitConversionRate` decimal(58,29) NOT NULL;

ALTER TABLE `CurrencyDescriptionPatternElementSpecialValues` MODIFY COLUMN `Value` decimal(58,29) NOT NULL;

ALTER TABLE `Coins` MODIFY COLUMN `Value` decimal(58,29) NOT NULL;

ALTER TABLE `ClanMemberships_Backpay` MODIFY COLUMN `Amount` decimal(58,29) NOT NULL;

ALTER TABLE `ChargenRoles_Currencies` MODIFY COLUMN `Amount` decimal(58,29) NOT NULL;

CREATE TABLE `Banks` (
    `Id` bigint(20) NOT NULL AUTO_INCREMENT,
    `Name` varchar(255) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
    `Code` varchar(255) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
    `EconomicZoneId` bigint(20) NOT NULL,
    `PrimaryCurrencyId` bigint(20) NOT NULL,
    `MaximumBankAccountsPerCustomer` int(11) NOT NULL,
    CONSTRAINT `PRIMARY` PRIMARY KEY (`Id`),
    CONSTRAINT `FK_Banks_Currencies` FOREIGN KEY (`PrimaryCurrencyId`) REFERENCES `Currencies` (`Id`) ON DELETE CASCADE,
    CONSTRAINT `FK_Banks_EconomicZones` FOREIGN KEY (`EconomicZoneId`) REFERENCES `EconomicZones` (`Id`) ON DELETE CASCADE
);

CREATE TABLE `BankAccountTypes` (
    `Id` bigint(20) NOT NULL AUTO_INCREMENT,
    `Name` varchar(255) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
    `CustomerDescription` varchar(1000) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
    `MaximumOverdrawAmount` decimal(58,29) NOT NULL,
    `WithdrawalFleeFlat` decimal(58,29) NOT NULL,
    `WithdrawalFleeRate` decimal(58,29) NOT NULL,
    `DepositFeeFlat` decimal(58,29) NOT NULL,
    `DepositFeeRate` decimal(58,29) NOT NULL,
    `TransferFeeFlat` decimal(58,29) NOT NULL,
    `TransferFeeRate` decimal(58,29) NOT NULL,
    `TransferFeeOtherBankFlat` decimal(58,29) NOT NULL,
    `TransferFeeOtherBankRate` decimal(58,29) NOT NULL,
    `DailyFee` decimal(58,29) NOT NULL,
    `DailyInterestRate` decimal(58,29) NOT NULL,
    `OverdrawFeeFlat` decimal(58,29) NOT NULL,
    `OverdrawFeeRate` decimal(58,29) NOT NULL,
    `DailyOverdrawnFee` decimal(58,29) NOT NULL,
    `DailyOverdrawnInterestRate` decimal(58,29) NOT NULL,
    `BankId` bigint(20) NOT NULL,
    `CanOpenAccountProgCharacterId` bigint(20) NULL,
    `CanOpenAccountProgClanId` bigint(20) NULL,
    `CanOpenAccountProgShopId` bigint(20) NULL,
    `CanCloseAccountProgId` bigint(20) NULL,
    CONSTRAINT `PRIMARY` PRIMARY KEY (`Id`),
    CONSTRAINT `FK_BankAccountTypes_Banks` FOREIGN KEY (`BankId`) REFERENCES `Banks` (`Id`) ON DELETE RESTRICT,
    CONSTRAINT `FK_BankAccountTypes_CanCloseProg` FOREIGN KEY (`CanCloseAccountProgId`) REFERENCES `FutureProgs` (`Id`) ON DELETE RESTRICT,
    CONSTRAINT `FK_BankAccountTypes_CharacterProgs` FOREIGN KEY (`CanOpenAccountProgCharacterId`) REFERENCES `FutureProgs` (`Id`) ON DELETE RESTRICT,
    CONSTRAINT `FK_BankAccountTypes_ClanProgs` FOREIGN KEY (`CanOpenAccountProgClanId`) REFERENCES `FutureProgs` (`Id`) ON DELETE RESTRICT,
    CONSTRAINT `FK_BankAccountTypes_ShopProgs` FOREIGN KEY (`CanOpenAccountProgShopId`) REFERENCES `FutureProgs` (`Id`) ON DELETE RESTRICT
);

CREATE TABLE `BankBranches` (
    `BankId` bigint(20) NOT NULL,
    `CellId` bigint(20) NOT NULL,
    CONSTRAINT `PRIMARY` PRIMARY KEY (`BankId`, `CellId`),
    CONSTRAINT `FK_BankBranches_Banks` FOREIGN KEY (`BankId`) REFERENCES `Banks` (`Id`) ON DELETE RESTRICT,
    CONSTRAINT `FK_BankBranches_Cells` FOREIGN KEY (`CellId`) REFERENCES `Cells` (`Id`) ON DELETE RESTRICT
);

CREATE TABLE `BankCurrencyReserves` (
    `BankId` bigint(20) NOT NULL,
    `CurrencyId` bigint(20) NOT NULL,
    `Amount` decimal(58,29) NOT NULL,
    CONSTRAINT `PRIMARY` PRIMARY KEY (`BankId`, `CurrencyId`),
    CONSTRAINT `FK_BankCurrencyReserves_Banks` FOREIGN KEY (`BankId`) REFERENCES `Banks` (`Id`) ON DELETE RESTRICT,
    CONSTRAINT `FK_BankCurrencyReserves_Currencies` FOREIGN KEY (`CurrencyId`) REFERENCES `Currencies` (`Id`) ON DELETE RESTRICT
);

CREATE TABLE `BankExchangeRates` (
    `BankId` bigint(20) NOT NULL,
    `FromCurrencyId` bigint(20) NOT NULL,
    `ToCurrencyId` bigint(20) NOT NULL,
    `ExchangeRate` decimal(58,29) NOT NULL,
    CONSTRAINT `PRIMARY` PRIMARY KEY (`BankId`, `FromCurrencyId`, `ToCurrencyId`),
    CONSTRAINT `FK_BankExchangeRates_Banks` FOREIGN KEY (`BankId`) REFERENCES `Banks` (`Id`) ON DELETE RESTRICT,
    CONSTRAINT `FK_BankExchangeRates_Currencies_From` FOREIGN KEY (`FromCurrencyId`) REFERENCES `Currencies` (`Id`) ON DELETE RESTRICT,
    CONSTRAINT `FK_BankExchangeRates_Currencies_To` FOREIGN KEY (`ToCurrencyId`) REFERENCES `Currencies` (`Id`) ON DELETE RESTRICT
);

CREATE TABLE `BankManagerAuditLogs` (
    `Id` bigint(20) NOT NULL AUTO_INCREMENT,
    `BankId` bigint(20) NOT NULL,
    `CharacterId` bigint(20) NOT NULL,
    `DateTime` varchar(255) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
    `Detail` varchar(1000) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
    CONSTRAINT `PRIMARY` PRIMARY KEY (`Id`),
    CONSTRAINT `FK_BankManagerAuditLogs_Banks` FOREIGN KEY (`BankId`) REFERENCES `Banks` (`Id`) ON DELETE RESTRICT,
    CONSTRAINT `FK_BankManagerAuditLogs_Characters` FOREIGN KEY (`CharacterId`) REFERENCES `Characters` (`Id`) ON DELETE CASCADE
);

CREATE TABLE `BankManagers` (
    `BankId` bigint(20) NOT NULL,
    `CharacterId` bigint(20) NOT NULL,
    CONSTRAINT `PRIMARY` PRIMARY KEY (`BankId`, `CharacterId`),
    CONSTRAINT `FK_BankManagers_Banks` FOREIGN KEY (`BankId`) REFERENCES `Banks` (`Id`) ON DELETE RESTRICT,
    CONSTRAINT `FK_BankManagers_Characters` FOREIGN KEY (`CharacterId`) REFERENCES `Characters` (`Id`) ON DELETE RESTRICT
);

CREATE TABLE `BankAccounts` (
    `Id` bigint(20) NOT NULL AUTO_INCREMENT,
    `Name` varchar(255) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
    `AccountNumber` int(11) NOT NULL,
    `BankId` bigint(20) NOT NULL,
    `BankAccountTypeId` bigint(20) NOT NULL,
    `CurrentBalance` decimal(58,29) NOT NULL,
    `AccountOwnerCharacterId` bigint(20) NULL,
    `AccountOwnerClanId` bigint(20) NULL,
    `AccountOwnerShopId` bigint(20) NULL,
    `NominatedBenefactorAccountId` bigint(20) NULL,
    `AccountCreationDate` varchar(255) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
    `AccountStatus` int(11) NOT NULL,
    `CurrentMonthInterest` decimal(58,29) NOT NULL,
    `CurrentMonthFees` decimal(58,29) NOT NULL,
    CONSTRAINT `PRIMARY` PRIMARY KEY (`Id`),
    CONSTRAINT `FK_BankAccounts_BankAccounts` FOREIGN KEY (`NominatedBenefactorAccountId`) REFERENCES `BankAccounts` (`Id`) ON DELETE RESTRICT,
    CONSTRAINT `FK_BankAccounts_BankAccountTypes` FOREIGN KEY (`BankAccountTypeId`) REFERENCES `BankAccountTypes` (`Id`) ON DELETE RESTRICT,
    CONSTRAINT `FK_BankAccounts_Banks` FOREIGN KEY (`BankId`) REFERENCES `Banks` (`Id`) ON DELETE RESTRICT,
    CONSTRAINT `FK_BankAccounts_Characters` FOREIGN KEY (`AccountOwnerCharacterId`) REFERENCES `Characters` (`Id`) ON DELETE RESTRICT,
    CONSTRAINT `FK_BankAccounts_Clans` FOREIGN KEY (`AccountOwnerClanId`) REFERENCES `Clans` (`Id`) ON DELETE RESTRICT,
    CONSTRAINT `FK_BankAccounts_Shops` FOREIGN KEY (`AccountOwnerShopId`) REFERENCES `Shops` (`Id`) ON DELETE RESTRICT
);

CREATE TABLE `BankAccountTransactions` (
    `Id` bigint(20) NOT NULL AUTO_INCREMENT,
    `BankAccountId` bigint(20) NOT NULL,
    `TransactionType` int(11) NOT NULL,
    `Amount` decimal(58,29) NOT NULL,
    `TransactionTime` varchar(255) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
    `TransactionDescription` varchar(255) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
    `AccountBalanceAfter` decimal(58,29) NOT NULL,
    CONSTRAINT `PRIMARY` PRIMARY KEY (`Id`),
    CONSTRAINT `FK_BankAccountTransactions_BankAccounts` FOREIGN KEY (`BankAccountId`) REFERENCES `BankAccounts` (`Id`) ON DELETE RESTRICT
);

CREATE INDEX `IX_BankAccounts_AccountOwnerCharacterId` ON `BankAccounts` (`AccountOwnerCharacterId`);

CREATE INDEX `IX_BankAccounts_AccountOwnerClanId` ON `BankAccounts` (`AccountOwnerClanId`);

CREATE INDEX `IX_BankAccounts_AccountOwnerShopId` ON `BankAccounts` (`AccountOwnerShopId`);

CREATE INDEX `IX_BankAccounts_BankAccountTypeId` ON `BankAccounts` (`BankAccountTypeId`);

CREATE INDEX `IX_BankAccounts_BankId` ON `BankAccounts` (`BankId`);

CREATE INDEX `IX_BankAccounts_NominatedBenefactorAccountId` ON `BankAccounts` (`NominatedBenefactorAccountId`);

CREATE INDEX `IX_BankAccountTransactions_BankAccountId` ON `BankAccountTransactions` (`BankAccountId`);

CREATE INDEX `IX_BankAccountTypes_BankId` ON `BankAccountTypes` (`BankId`);

CREATE INDEX `IX_BankAccountTypes_CanCloseAccountProgId` ON `BankAccountTypes` (`CanCloseAccountProgId`);

CREATE INDEX `IX_BankAccountTypes_CanOpenAccountProgCharacterId` ON `BankAccountTypes` (`CanOpenAccountProgCharacterId`);

CREATE INDEX `IX_BankAccountTypes_CanOpenAccountProgClanId` ON `BankAccountTypes` (`CanOpenAccountProgClanId`);

CREATE INDEX `IX_BankAccountTypes_CanOpenAccountProgShopId` ON `BankAccountTypes` (`CanOpenAccountProgShopId`);

CREATE INDEX `IX_BankBranches_CellId` ON `BankBranches` (`CellId`);

CREATE INDEX `IX_BankCurrencyReserves_CurrencyId` ON `BankCurrencyReserves` (`CurrencyId`);

CREATE INDEX `IX_BankExchangeRates_FromCurrencyId` ON `BankExchangeRates` (`FromCurrencyId`);

CREATE INDEX `IX_BankExchangeRates_ToCurrencyId` ON `BankExchangeRates` (`ToCurrencyId`);

CREATE INDEX `IX_BankManagerAuditLogs_BankId` ON `BankManagerAuditLogs` (`BankId`);

CREATE INDEX `IX_BankManagerAuditLogs_CharacterId` ON `BankManagerAuditLogs` (`CharacterId`);

CREATE INDEX `IX_BankManagers_CharacterId` ON `BankManagers` (`CharacterId`);

CREATE INDEX `IX_Banks_EconomicZoneId` ON `Banks` (`EconomicZoneId`);

CREATE INDEX `IX_Banks_PrimaryCurrencyId` ON `Banks` (`PrimaryCurrencyId`);

INSERT INTO `__EFMigrationsHistory` (`MigrationId`, `ProductVersion`)
VALUES ('20210331025006_BanksV1', '9.0.11');

ALTER TABLE `WeaponAttacks` ADD `RequiredPositionStateIds` varchar(250) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL DEFAULT '1 16 17 18';

INSERT INTO `__EFMigrationsHistory` (`MigrationId`, `ProductVersion`)
VALUES ('20210423014825_WeaponAttackAddPositionRequirement', '9.0.11');

CREATE TABLE `AuctionHouses` (
    `Id` bigint(20) NOT NULL AUTO_INCREMENT,
    `Name` varchar(250) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
    `EconomicZoneId` bigint(20) NOT NULL,
    `AuctionHouseCellId` bigint(20) NOT NULL,
    `ProfitsBankAccountId` bigint(20) NOT NULL,
    `AuctionListingFeeFlat` decimal(58,29) NOT NULL,
    `AuctionListingFeeRate` decimal(58,29) NOT NULL,
    `Definition` mediumtext CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
    `DefaultListingTime` double NOT NULL,
    CONSTRAINT `PK_AuctionHouses` PRIMARY KEY (`Id`),
    CONSTRAINT `FK_AuctionHouses_BankAccounts` FOREIGN KEY (`ProfitsBankAccountId`) REFERENCES `BankAccounts` (`Id`) ON DELETE RESTRICT,
    CONSTRAINT `FK_AuctionHouses_Cells` FOREIGN KEY (`AuctionHouseCellId`) REFERENCES `Cells` (`Id`) ON DELETE RESTRICT,
    CONSTRAINT `FK_AuctionHouses_EconomicZones` FOREIGN KEY (`EconomicZoneId`) REFERENCES `EconomicZones` (`Id`) ON DELETE RESTRICT
);

CREATE INDEX `IX_AuctionHouses_AuctionHouseCellId` ON `AuctionHouses` (`AuctionHouseCellId`);

CREATE INDEX `IX_AuctionHouses_EconomicZoneId` ON `AuctionHouses` (`EconomicZoneId`);

CREATE INDEX `IX_AuctionHouses_ProfitsBankAccountId` ON `AuctionHouses` (`ProfitsBankAccountId`);

INSERT INTO `__EFMigrationsHistory` (`MigrationId`, `ProductVersion`)
VALUES ('20210626110830_AuctionHouses', '9.0.11');

CREATE TABLE `ConveyancingLocations` (
    `EconomicZoneId` bigint(20) NOT NULL,
    `CellId` bigint(20) NOT NULL,
    CONSTRAINT `PRIMARY` PRIMARY KEY (`EconomicZoneId`, `CellId`),
    CONSTRAINT `FK_ConveyancingLocations_Cells` FOREIGN KEY (`CellId`) REFERENCES `Cells` (`Id`) ON DELETE RESTRICT,
    CONSTRAINT `FK_ConveyancingLocations_EconomicZones` FOREIGN KEY (`EconomicZoneId`) REFERENCES `EconomicZones` (`Id`) ON DELETE RESTRICT
);

CREATE TABLE `PropertyLeaseOrders` (
    `Id` bigint(20) NOT NULL AUTO_INCREMENT,
    `PropertyId` bigint(20) NOT NULL,
    `PricePerInterval` decimal(58,29) NOT NULL,
    `BondRequired` decimal(58,29) NOT NULL,
    `Interval` varchar(100) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
    `CanLeaseProgCharacterId` bigint(20) NULL,
    `CanLeaseProgClanId` bigint(20) NULL,
    `MinimumLeaseDurationDays` double NOT NULL,
    `MaximumLeaseDurationDays` double NOT NULL,
    `AllowAutoRenew` bit(1) NOT NULL,
    `AutomaticallyRelistAfterLeaseTerm` bit(1) NOT NULL,
    `AllowLeaseNovation` bit(1) NOT NULL,
    `ListedForLease` bit(1) NOT NULL,
    `FeeIncreasePercentageAfterLeaseTerm` decimal(58,29) NOT NULL,
    `PropertyOwnerConsentInfo` text CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
    CONSTRAINT `PK_PropertyLeaseOrders` PRIMARY KEY (`Id`),
    CONSTRAINT `FK_PropertyLeaseOrders_FutureProgs_Character` FOREIGN KEY (`CanLeaseProgCharacterId`) REFERENCES `FutureProgs` (`Id`) ON DELETE RESTRICT,
    CONSTRAINT `FK_PropertyLeaseOrders_FutureProgs_Clan` FOREIGN KEY (`CanLeaseProgClanId`) REFERENCES `FutureProgs` (`Id`) ON DELETE RESTRICT
);

CREATE TABLE `PropertyLeases` (
    `Id` bigint(20) NOT NULL AUTO_INCREMENT,
    `PropertyId` bigint(20) NOT NULL,
    `LeaseOrderId` bigint(20) NOT NULL,
    `LeaseholderReference` varchar(100) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
    `PricePerInterval` decimal(58,29) NOT NULL,
    `BondPayment` decimal(58,29) NOT NULL,
    `PaymentBalance` decimal(58,29) NOT NULL,
    `LeaseStart` varchar(100) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
    `LeaseEnd` varchar(100) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
    `LastLeasePayment` varchar(100) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
    `AutoRenew` bit(1) NOT NULL,
    `BondReturned` bit(1) NOT NULL,
    `Interval` varchar(100) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
    `TenantInfo` text CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
    CONSTRAINT `PK_PropertyLeases` PRIMARY KEY (`Id`),
    CONSTRAINT `FK_PropertyLeases_PropertyLeaseOrders` FOREIGN KEY (`LeaseOrderId`) REFERENCES `PropertyLeaseOrders` (`Id`) ON DELETE CASCADE
);

CREATE TABLE `PropertyLocations` (
    `PropertyId` bigint(20) NOT NULL,
    `CellId` bigint(20) NOT NULL,
    CONSTRAINT `PRIMARY` PRIMARY KEY (`PropertyId`, `CellId`),
    CONSTRAINT `FK_PropertyLocations_Cell` FOREIGN KEY (`CellId`) REFERENCES `Cells` (`Id`) ON DELETE CASCADE
);

CREATE TABLE `PropertyOwners` (
    `Id` bigint(20) NOT NULL AUTO_INCREMENT,
    `PropertyId` bigint(20) NOT NULL,
    `FrameworkItemId` bigint(20) NOT NULL,
    `FrameworkItemType` varchar(100) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
    `ShareOfOwnership` decimal(58,29) NOT NULL,
    `RevenueAccountId` bigint(20) NULL,
    CONSTRAINT `PK_PropertyOwners` PRIMARY KEY (`Id`),
    CONSTRAINT `FK_PropertyOwners_BankAccounts` FOREIGN KEY (`RevenueAccountId`) REFERENCES `BankAccounts` (`Id`) ON DELETE RESTRICT
);

CREATE TABLE `PropertySalesOrders` (
    `Id` bigint(20) NOT NULL AUTO_INCREMENT,
    `PropertyId` bigint(20) NOT NULL,
    `ReservePrice` decimal(58,29) NOT NULL,
    `OrderStatus` int(11) NOT NULL,
    `StartOfListing` varchar(100) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
    `DurationOfListingDays` double NOT NULL,
    `PropertyOwnerConsentInfo` text CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
    CONSTRAINT `PK_PropertySalesOrders` PRIMARY KEY (`Id`)
);

CREATE TABLE `Properties` (
    `Id` bigint(20) NOT NULL AUTO_INCREMENT,
    `Name` varchar(250) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
    `EconomicZoneId` bigint(20) NOT NULL,
    `DetailedDescription` text CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
    `LastChangeOfOwnership` varchar(100) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
    `ApplyCriminalCodeInProperty` bit(1) NOT NULL,
    `LeaseId` bigint(20) NULL,
    `LeaseOrderId` bigint(20) NULL,
    `SaleOrderId` bigint(20) NULL,
    `LastSaleValue` decimal(58,29) NOT NULL,
    CONSTRAINT `PK_Properties` PRIMARY KEY (`Id`),
    CONSTRAINT `FK_Properties_EconomicZones` FOREIGN KEY (`EconomicZoneId`) REFERENCES `EconomicZones` (`Id`) ON DELETE CASCADE,
    CONSTRAINT `FK_Properties_Lease` FOREIGN KEY (`LeaseId`) REFERENCES `PropertyLeases` (`Id`) ON DELETE RESTRICT,
    CONSTRAINT `FK_Properties_LeaseOrder` FOREIGN KEY (`LeaseOrderId`) REFERENCES `PropertyLeaseOrders` (`Id`) ON DELETE RESTRICT,
    CONSTRAINT `FK_Properties_SaleOrder` FOREIGN KEY (`SaleOrderId`) REFERENCES `PropertySalesOrders` (`Id`) ON DELETE RESTRICT
);

CREATE INDEX `IX_ConveyancingLocations_CellId` ON `ConveyancingLocations` (`CellId`);

CREATE INDEX `IX_Properties_EconomicZoneId` ON `Properties` (`EconomicZoneId`);

CREATE INDEX `IX_Properties_LeaseId` ON `Properties` (`LeaseId`);

CREATE INDEX `IX_Properties_LeaseOrderId` ON `Properties` (`LeaseOrderId`);

CREATE INDEX `IX_Properties_SaleOrderId` ON `Properties` (`SaleOrderId`);

CREATE INDEX `IX_PropertyLeaseOrders_CanLeaseProgCharacterId` ON `PropertyLeaseOrders` (`CanLeaseProgCharacterId`);

CREATE INDEX `IX_PropertyLeaseOrders_CanLeaseProgClanId` ON `PropertyLeaseOrders` (`CanLeaseProgClanId`);

CREATE INDEX `IX_PropertyLeaseOrders_PropertyId` ON `PropertyLeaseOrders` (`PropertyId`);

CREATE INDEX `IX_PropertyLeases_LeaseOrderId` ON `PropertyLeases` (`LeaseOrderId`);

CREATE INDEX `IX_PropertyLeases_PropertyId` ON `PropertyLeases` (`PropertyId`);

CREATE INDEX `IX_PropertyLocations_CellId` ON `PropertyLocations` (`CellId`);

CREATE INDEX `IX_PropertyOwners_PropertyId` ON `PropertyOwners` (`PropertyId`);

CREATE INDEX `IX_PropertyOwners_RevenueAccountId` ON `PropertyOwners` (`RevenueAccountId`);

CREATE INDEX `IX_PropertySalesOrders_PropertyId` ON `PropertySalesOrders` (`PropertyId`);

ALTER TABLE `PropertyLeaseOrders` ADD CONSTRAINT `FK_PropertyLeaseOrders_Property` FOREIGN KEY (`PropertyId`) REFERENCES `Properties` (`Id`) ON DELETE CASCADE;

ALTER TABLE `PropertyLeases` ADD CONSTRAINT `FK_PropertyLeases_Property` FOREIGN KEY (`PropertyId`) REFERENCES `Properties` (`Id`) ON DELETE CASCADE;

ALTER TABLE `PropertyLocations` ADD CONSTRAINT `FK_PropertyLocations_Property` FOREIGN KEY (`PropertyId`) REFERENCES `Properties` (`Id`) ON DELETE CASCADE;

ALTER TABLE `PropertyOwners` ADD CONSTRAINT `FK_PropertyOwners_Properties` FOREIGN KEY (`PropertyId`) REFERENCES `Properties` (`Id`) ON DELETE CASCADE;

ALTER TABLE `PropertySalesOrders` ADD CONSTRAINT `FK_PropertySaleOrders_Property` FOREIGN KEY (`PropertyId`) REFERENCES `Properties` (`Id`) ON DELETE CASCADE;

INSERT INTO `__EFMigrationsHistory` (`MigrationId`, `ProductVersion`)
VALUES ('20210810123837_PropertyV1', '9.0.11');

ALTER TABLE `PropertyLeases` ADD `BondClaimed` decimal(65,30) NOT NULL DEFAULT 0.0;

CREATE TABLE `PropertyKeys` (
    `Id` bigint(20) NOT NULL AUTO_INCREMENT,
    `Name` varchar(250) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
    `GameItemId` bigint(20) NOT NULL,
    `PropertyId` bigint(20) NOT NULL,
    `AddedToPropertyOnDate` varchar(100) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
    `CostToReplace` decimal(58,29) NOT NULL,
    `IsReturned` bit(1) NOT NULL,
    CONSTRAINT `PK_PropertyKeys` PRIMARY KEY (`Id`),
    CONSTRAINT `FK_PropertyKeys_GameItems` FOREIGN KEY (`GameItemId`) REFERENCES `GameItems` (`Id`) ON DELETE RESTRICT,
    CONSTRAINT `FK_PropertyKeys_Property` FOREIGN KEY (`PropertyId`) REFERENCES `Properties` (`Id`) ON DELETE RESTRICT
);

CREATE INDEX `IX_PropertyKeys_GameItemId` ON `PropertyKeys` (`GameItemId`);

CREATE INDEX `IX_PropertyKeys_PropertyId` ON `PropertyKeys` (`PropertyId`);

INSERT INTO `__EFMigrationsHistory` (`MigrationId`, `ProductVersion`)
VALUES ('20210902052233_PropertyV2', '9.0.11');

ALTER TABLE `Laws` ADD `DoNotAutomaticallyApplyRepeats` bit(1) NOT NULL DEFAULT 0;

INSERT INTO `__EFMigrationsHistory` (`MigrationId`, `ProductVersion`)
VALUES ('20210914132733_Sep21LawUpdate', '9.0.11');

ALTER TABLE `Laws` DROP COLUMN `MaximumFine`;

ALTER TABLE `Laws` DROP COLUMN `MinimumFine`;

ALTER TABLE `Laws` DROP COLUMN `StandardFine`;

ALTER TABLE `LegalAuthorities` ADD `AutomaticConvictionTime` double NOT NULL DEFAULT 0.0;

ALTER TABLE `LegalAuthorities` ADD `AutomaticallyConvict` tinyint(1) NOT NULL DEFAULT FALSE;

ALTER TABLE `LegalAuthorities` ADD `BailCalculationProgId` bigint(20) NULL;

ALTER TABLE `LegalAuthorities` ADD `BankAccountId` bigint(20) NULL;

ALTER TABLE `LegalAuthorities` ADD `CourtLocationId` bigint(20) NULL;

ALTER TABLE `LegalAuthorities` ADD `GuardianDiscordChannel` decimal(20,0) NULL;

ALTER TABLE `LegalAuthorities` ADD `JailLocationId` bigint(20) NULL;

ALTER TABLE `LegalAuthorities` ADD `OnHoldProgId` bigint(20) NULL;

ALTER TABLE `Laws` ADD `PunishmentStrategy` text CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL;

ALTER TABLE `Crimes` ADD `CalculatedBail` decimal(58,29) NOT NULL DEFAULT 0.0;

ALTER TABLE `Crimes` ADD `CustodialSentenceLength` double NOT NULL DEFAULT 0.0;

ALTER TABLE `Crimes` ADD `FineRecorded` decimal(58,29) NOT NULL DEFAULT 0.0;

ALTER TABLE `Crimes` ADD `HasBeenEnforced` bit(1) NOT NULL DEFAULT 0;

CREATE TABLE `LegalAuthorityFines` (
    `LegalAuthorityId` bigint(20) NOT NULL,
    `CharacterId` bigint(20) NOT NULL,
    `FinesOwned` decimal(58,29) NOT NULL,
    `PaymentRequiredBy` varchar(100) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
    CONSTRAINT `PRIMARY` PRIMARY KEY (`LegalAuthorityId`, `CharacterId`),
    CONSTRAINT `FK_LegalAuthorityFines_Characters` FOREIGN KEY (`CharacterId`) REFERENCES `Characters` (`Id`) ON DELETE CASCADE,
    CONSTRAINT `FK_LegalAuthorityFines_LegalAuthorities` FOREIGN KEY (`LegalAuthorityId`) REFERENCES `LegalAuthorities` (`Id`) ON DELETE CASCADE
);

CREATE TABLE `LegalAuthorityJailCells` (
    `LegalAuthorityId` bigint(20) NOT NULL,
    `CellId` bigint(20) NOT NULL,
    CONSTRAINT `PRIMARY` PRIMARY KEY (`LegalAuthorityId`, `CellId`),
    CONSTRAINT `FK_LegalAuthoritiesCells_Cells_Jail` FOREIGN KEY (`CellId`) REFERENCES `Cells` (`Id`) ON DELETE CASCADE,
    CONSTRAINT `FK_LegalAuthoritiesCells_LegalAuthorities_Jail` FOREIGN KEY (`LegalAuthorityId`) REFERENCES `LegalAuthorities` (`Id`) ON DELETE CASCADE
);

CREATE INDEX `IX_LegalAuthorities_BailCalculationProgId` ON `LegalAuthorities` (`BailCalculationProgId`);

CREATE INDEX `IX_LegalAuthorities_BankAccountId` ON `LegalAuthorities` (`BankAccountId`);

CREATE INDEX `IX_LegalAuthorities_CourtLocationId` ON `LegalAuthorities` (`CourtLocationId`);

CREATE INDEX `IX_LegalAuthorities_JailLocationId` ON `LegalAuthorities` (`JailLocationId`);

CREATE INDEX `IX_LegalAuthorities_OnHoldProgId` ON `LegalAuthorities` (`OnHoldProgId`);

CREATE INDEX `IX_LegalAuthorityFines_CharacterId` ON `LegalAuthorityFines` (`CharacterId`);

CREATE INDEX `FK_LegalAuthoritiesCells_Cells_Jail_idx` ON `LegalAuthorityJailCells` (`CellId`);

CREATE INDEX `FK_LegalAuthoritiesCells_LegalAuthorities_Jail_idx` ON `LegalAuthorityJailCells` (`LegalAuthorityId`);

ALTER TABLE `LegalAuthorities` ADD CONSTRAINT `FK_LegalAuthorities_BankAccounts_BankAccountId` FOREIGN KEY (`BankAccountId`) REFERENCES `BankAccounts` (`Id`) ON DELETE RESTRICT;

ALTER TABLE `LegalAuthorities` ADD CONSTRAINT `FK_LegalAuthorities_CourtroomCell` FOREIGN KEY (`CourtLocationId`) REFERENCES `Cells` (`Id`) ON DELETE RESTRICT;

ALTER TABLE `LegalAuthorities` ADD CONSTRAINT `FK_LegalAuthorities_FutureprogsBailCalc` FOREIGN KEY (`BailCalculationProgId`) REFERENCES `FutureProgs` (`Id`) ON DELETE RESTRICT;

ALTER TABLE `LegalAuthorities` ADD CONSTRAINT `FK_LegalAuthorities_FutureprogsHold` FOREIGN KEY (`OnHoldProgId`) REFERENCES `FutureProgs` (`Id`) ON DELETE RESTRICT;

ALTER TABLE `LegalAuthorities` ADD CONSTRAINT `FK_LegalAuthorities_PrisonJailCells` FOREIGN KEY (`JailLocationId`) REFERENCES `Cells` (`Id`) ON DELETE RESTRICT;

INSERT INTO `__EFMigrationsHistory` (`MigrationId`, `ProductVersion`)
VALUES ('20211025020630_JusticeOverhaulOct21', '9.0.11');

ALTER TABLE `Terrains` ADD `TerrainANSIColour` varchar(45) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL DEFAULT '7';

INSERT INTO `__EFMigrationsHistory` (`MigrationId`, `ProductVersion`)
VALUES ('20211217034326_TerrainMapColourAddition', '9.0.11');

ALTER TABLE `GameItems` ADD `SkinId` bigint(20) NULL;

ALTER TABLE `GameItemProtos` ADD `PermitPlayerSkins` bit(1) NOT NULL DEFAULT b'0';

CREATE TABLE `GameItemSkins` (
    `Id` bigint(20) NOT NULL,
    `RevisionNumber` int(11) NOT NULL,
    `EditableItemId` bigint(20) NOT NULL,
    `Name` varchar(500) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NULL,
    `ItemProtoId` bigint(20) NOT NULL,
    `ItemName` varchar(500) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NULL,
    `ShortDescription` varchar(500) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NULL,
    `FullDescription` varchar(2000) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NULL,
    `LongDescription` varchar(500) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NULL,
    `Quality` int(11) NULL,
    `IsPublic` bit(1) NOT NULL,
    `CanUseSkinProgId` bigint(20) NULL,
    CONSTRAINT `PRIMARY` PRIMARY KEY (`Id`, `RevisionNumber`),
    CONSTRAINT `FK_GameItemSkins_EditableItems` FOREIGN KEY (`EditableItemId`) REFERENCES `EditableItems` (`Id`)
) CHARACTER SET=utf8mb4;

CREATE INDEX `IX_GameItemSkins_EditableItemId` ON `GameItemSkins` (`EditableItemId`);

INSERT INTO `__EFMigrationsHistory` (`MigrationId`, `ProductVersion`)
VALUES ('20211220045847_Skins', '9.0.11');

ALTER TABLE `Merchandises` ADD `SkinId` bigint(20) NULL;

INSERT INTO `__EFMigrationsHistory` (`MigrationId`, `ProductVersion`)
VALUES ('20211222033658_Skins-Pt2', '9.0.11');

ALTER TABLE `Clans` ADD `BankAccountId` bigint(20) NULL;

CREATE INDEX `IX_Clans_BankAccountId` ON `Clans` (`BankAccountId`);

ALTER TABLE `Clans` ADD CONSTRAINT `FK_Clans_BankAccounts` FOREIGN KEY (`BankAccountId`) REFERENCES `BankAccounts` (`Id`);

INSERT INTO `__EFMigrationsHistory` (`MigrationId`, `ProductVersion`)
VALUES ('20211226134159_ClanBankAccounts', '9.0.11');

ALTER TABLE `Boards` ADD `CalendarId` bigint(20) NULL;

ALTER TABLE `BoardPosts` ADD `AuthorIsCharacter` bit(1) NOT NULL DEFAULT 0;

ALTER TABLE `BoardPosts` ADD `InGameDateTime` varchar(200) CHARACTER SET utf8 COLLATE utf8_general_ci NULL;

CREATE INDEX `IX_Boards_CalendarId` ON `Boards` (`CalendarId`);

ALTER TABLE `Boards` ADD CONSTRAINT `FK_Boards_Calendars` FOREIGN KEY (`CalendarId`) REFERENCES `Calendars` (`Id`);

INSERT INTO `__EFMigrationsHistory` (`MigrationId`, `ProductVersion`)
VALUES ('20211229004501_PlayerBoards', '9.0.11');

CREATE TABLE `JobFindingLocations` (
    `EconomicZoneId` bigint(20) NOT NULL,
    `CellId` bigint(20) NOT NULL,
    CONSTRAINT `PRIMARY` PRIMARY KEY (`EconomicZoneId`, `CellId`),
    CONSTRAINT `FK_JobFindingLocations_Cells` FOREIGN KEY (`CellId`) REFERENCES `Cells` (`Id`),
    CONSTRAINT `FK_JobFindingLocations_EconomicZones` FOREIGN KEY (`EconomicZoneId`) REFERENCES `EconomicZones` (`Id`)
) CHARACTER SET=utf8mb4;

CREATE TABLE `JobListings` (
    `Id` bigint(20) NOT NULL AUTO_INCREMENT,
    `PosterId` bigint(20) NOT NULL,
    `IsReadyToBePosted` bit(1) NOT NULL,
    `IsArchived` bit(1) NOT NULL,
    `Name` varchar(200) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
    `PosterType` varchar(100) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
    `JobListingType` varchar(100) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
    `Definition` text CHARACTER SET utf8 COLLATE utf8_general_ci NULL,
    `Description` text CHARACTER SET utf8 COLLATE utf8_general_ci NULL,
    `MoneyPaidIn` text CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
    `MaximumDuration` varchar(200) CHARACTER SET utf8 COLLATE utf8_general_ci NULL,
    `EligibilityProgId` bigint(20) NOT NULL,
    `ClanId` bigint(20) NULL,
    `RankId` bigint(20) NULL,
    `PaygradeId` bigint(20) NULL,
    `AppointmentId` bigint(20) NULL,
    `PersonalProjectId` bigint(20) NULL,
    `PersonalProjectRevisionNumber` int(11) NULL,
    `RequiredProjectId` bigint(20) NULL,
    `RequiredProjectLabourId` bigint(20) NULL,
    `BankAccountId` bigint(20) NULL,
    `EconomicZoneId` bigint(20) NOT NULL,
    `MaximumNumberOfSimultaneousEmployees` int(11) NOT NULL,
    `FullTimeEquivalentRatio` double NOT NULL,
    CONSTRAINT `PK_JobListings` PRIMARY KEY (`Id`),
    CONSTRAINT `FK_JobListings_ActiveProjectLabours` FOREIGN KEY (`RequiredProjectId`, `RequiredProjectLabourId`) REFERENCES `ActiveProjectLabours` (`ActiveProjectId`, `ProjectLabourRequirementsId`),
    CONSTRAINT `FK_JobListings_ActiveProjects` FOREIGN KEY (`RequiredProjectId`) REFERENCES `ActiveProjects` (`Id`),
    CONSTRAINT `FK_JobListings_Appointments` FOREIGN KEY (`AppointmentId`) REFERENCES `Appointments` (`Id`),
    CONSTRAINT `FK_JobListings_BankAccounts` FOREIGN KEY (`BankAccountId`) REFERENCES `BankAccounts` (`Id`),
    CONSTRAINT `FK_JobListings_Clans` FOREIGN KEY (`ClanId`) REFERENCES `Clans` (`Id`),
    CONSTRAINT `FK_JobListings_EconomicZones` FOREIGN KEY (`EconomicZoneId`) REFERENCES `EconomicZones` (`Id`),
    CONSTRAINT `FK_JobListings_FutureProgs` FOREIGN KEY (`EligibilityProgId`) REFERENCES `FutureProgs` (`Id`) ON DELETE CASCADE,
    CONSTRAINT `FK_JobListings_Paygrades` FOREIGN KEY (`PaygradeId`) REFERENCES `Paygrades` (`Id`),
    CONSTRAINT `FK_JobListings_Projects` FOREIGN KEY (`PersonalProjectId`, `PersonalProjectRevisionNumber`) REFERENCES `Projects` (`Id`, `RevisionNumber`),
    CONSTRAINT `FK_JobListings_Ranks` FOREIGN KEY (`RankId`) REFERENCES `Ranks` (`Id`)
) CHARACTER SET=utf8mb4;

CREATE TABLE `ActiveJobs` (
    `Id` bigint(20) NOT NULL AUTO_INCREMENT,
    `JobListingId` bigint(20) NOT NULL,
    `CharacterId` bigint(20) NOT NULL,
    `JobCommenced` varchar(200) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
    `JobDueToEnd` varchar(200) CHARACTER SET utf8 COLLATE utf8_general_ci NULL,
    `JobEnded` longtext CHARACTER SET utf8mb4 NULL,
    `IsJobComplete` bit(1) NOT NULL,
    `AlreadyHadClanPosition` bit(1) NOT NULL,
    `BackpayOwed` text CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
    `RevenueEarned` text CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
    `CurrentPerformance` double NOT NULL,
    `ActiveProjectId` bigint(20) NULL,
    CONSTRAINT `PK_ActiveJobs` PRIMARY KEY (`Id`),
    CONSTRAINT `FK_ActiveJobs_ActiveProjects` FOREIGN KEY (`ActiveProjectId`) REFERENCES `ActiveProjects` (`Id`),
    CONSTRAINT `FK_ActiveJobs_Characters` FOREIGN KEY (`CharacterId`) REFERENCES `Characters` (`Id`),
    CONSTRAINT `FK_ActiveJobs_JobListings` FOREIGN KEY (`JobListingId`) REFERENCES `JobListings` (`Id`)
) CHARACTER SET=utf8mb4;

CREATE INDEX `IX_ActiveJobs_ActiveProjectId` ON `ActiveJobs` (`ActiveProjectId`);

CREATE INDEX `IX_ActiveJobs_CharacterId` ON `ActiveJobs` (`CharacterId`);

CREATE INDEX `IX_ActiveJobs_JobListingId` ON `ActiveJobs` (`JobListingId`);

CREATE INDEX `IX_JobFindingLocations_CellId` ON `JobFindingLocations` (`CellId`);

CREATE INDEX `IX_JobListings_AppointmentId` ON `JobListings` (`AppointmentId`);

CREATE INDEX `IX_JobListings_BankAccountId` ON `JobListings` (`BankAccountId`);

CREATE INDEX `IX_JobListings_ClanId` ON `JobListings` (`ClanId`);

CREATE INDEX `IX_JobListings_EconomicZoneId` ON `JobListings` (`EconomicZoneId`);

CREATE INDEX `IX_JobListings_EligibilityProgId` ON `JobListings` (`EligibilityProgId`);

CREATE INDEX `IX_JobListings_PaygradeId` ON `JobListings` (`PaygradeId`);

CREATE INDEX `IX_JobListings_PersonalProjectId_PersonalProjectRevisionNumber` ON `JobListings` (`PersonalProjectId`, `PersonalProjectRevisionNumber`);

CREATE INDEX `IX_JobListings_RankId` ON `JobListings` (`RankId`);

CREATE INDEX `IX_JobListings_RequiredProjectId_RequiredProjectLabourId` ON `JobListings` (`RequiredProjectId`, `RequiredProjectLabourId`);

INSERT INTO `__EFMigrationsHistory` (`MigrationId`, `ProductVersion`)
VALUES ('20220104134109_JobsV1', '9.0.11');

ALTER TABLE `Projects` ADD `AppearInJobsList` bit(1) NOT NULL DEFAULT 0;

INSERT INTO `__EFMigrationsHistory` (`MigrationId`, `ProductVersion`)
VALUES ('20220105004035_ProjectsJobsUpdate', '9.0.11');

ALTER TABLE `BoardPosts` DROP FOREIGN KEY `FK_BoardsPosts_Accounts`;

INSERT INTO `__EFMigrationsHistory` (`MigrationId`, `ProductVersion`)
VALUES ('20220108004307_BoardBugFix', '9.0.11');

ALTER TABLE `BoardPosts` ADD `AuthorFullDescription` varchar(1000) CHARACTER SET utf8 COLLATE utf8_general_ci NULL;

ALTER TABLE `BoardPosts` ADD `AuthorName` varchar(200) CHARACTER SET utf8 COLLATE utf8_general_ci NULL;

ALTER TABLE `BoardPosts` ADD `AuthorShortDescription` varchar(200) CHARACTER SET utf8 COLLATE utf8_general_ci NULL;

INSERT INTO `__EFMigrationsHistory` (`MigrationId`, `ProductVersion`)
VALUES ('20220117102755_BoardsDescriptions', '9.0.11');

ALTER TABLE `BoardPosts` MODIFY COLUMN `AuthorFullDescription` varchar(8000) CHARACTER SET utf8 COLLATE utf8_general_ci NULL;

INSERT INTO `__EFMigrationsHistory` (`MigrationId`, `ProductVersion`)
VALUES ('20220210215752_LongerAuthorFullDescs', '9.0.11');

ALTER TABLE `Appointments` DROP FOREIGN KEY `FK_Appointments_Clans`;

ALTER TABLE `Appointments_Abbreviations` DROP FOREIGN KEY `FK_Appointments_Abbreviations_Appointments`;

ALTER TABLE `Appointments_Titles` DROP FOREIGN KEY `FK_Appointments_Titles_Appointments`;

ALTER TABLE `Paygrades` DROP FOREIGN KEY `FK_Paygrades_Clans`;

ALTER TABLE `Appointments` ADD CONSTRAINT `FK_Appointments_Clans` FOREIGN KEY (`ClanId`) REFERENCES `Clans` (`Id`) ON DELETE CASCADE;

ALTER TABLE `Appointments_Abbreviations` ADD CONSTRAINT `FK_Appointments_Abbreviations_Appointments` FOREIGN KEY (`AppointmentId`) REFERENCES `Appointments` (`Id`) ON DELETE CASCADE;

ALTER TABLE `Appointments_Titles` ADD CONSTRAINT `FK_Appointments_Titles_Appointments` FOREIGN KEY (`AppointmentId`) REFERENCES `Appointments` (`Id`) ON DELETE CASCADE;

ALTER TABLE `Paygrades` ADD CONSTRAINT `FK_Paygrades_Clans` FOREIGN KEY (`ClanId`) REFERENCES `Clans` (`Id`) ON DELETE CASCADE;

INSERT INTO `__EFMigrationsHistory` (`MigrationId`, `ProductVersion`)
VALUES ('20220225125641_ClanFKFixing', '9.0.11');

CREATE TABLE `NPCSpawners` (
    `Id` bigint(20) NOT NULL AUTO_INCREMENT,
    `Name` varchar(255) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
    `TargetTemplateId` bigint(20) NULL,
    `TargetCount` int(11) NOT NULL,
    `MinimumCount` int(11) NOT NULL,
    `OnSpawnProgId` bigint(20) NULL,
    `CountsAsProgId` bigint(20) NULL,
    `IsActiveProgId` bigint(20) NULL,
    `SpawnStrategy` int(11) NOT NULL,
    CONSTRAINT `PK_NPCSpawners` PRIMARY KEY (`Id`),
    CONSTRAINT `FK_NPCSpawners_CountsAsProg` FOREIGN KEY (`CountsAsProgId`) REFERENCES `FutureProgs` (`Id`) ON DELETE SET NULL,
    CONSTRAINT `FK_NPCSpawners_IsActiveProg` FOREIGN KEY (`IsActiveProgId`) REFERENCES `FutureProgs` (`Id`) ON DELETE SET NULL,
    CONSTRAINT `FK_NPCSpawners_OnSpawnProg` FOREIGN KEY (`OnSpawnProgId`) REFERENCES `FutureProgs` (`Id`) ON DELETE SET NULL
) CHARACTER SET=utf8mb4;

CREATE TABLE `NPCSpawnerCells` (
    `NPCSpawnerId` bigint(20) NOT NULL,
    `CellId` bigint(20) NOT NULL,
    CONSTRAINT `PRIMARY` PRIMARY KEY (`NPCSpawnerId`, `CellId`),
    CONSTRAINT `FK_NPCSpawnerCells_Cell` FOREIGN KEY (`CellId`) REFERENCES `Cells` (`Id`) ON DELETE CASCADE,
    CONSTRAINT `FK_NPCSpawnerCells_NPCSpawner` FOREIGN KEY (`NPCSpawnerId`) REFERENCES `NPCSpawners` (`Id`) ON DELETE CASCADE
) CHARACTER SET=utf8mb4;

CREATE TABLE `NPCSpawnerZones` (
    `NPCSpawnerId` bigint(20) NOT NULL,
    `ZoneId` bigint(20) NOT NULL,
    CONSTRAINT `PRIMARY` PRIMARY KEY (`NPCSpawnerId`, `ZoneId`),
    CONSTRAINT `FK_NPCSpawnerZones_NPCSpawner` FOREIGN KEY (`NPCSpawnerId`) REFERENCES `NPCSpawners` (`Id`) ON DELETE CASCADE,
    CONSTRAINT `FK_NPCSpawnerZones_Zone` FOREIGN KEY (`ZoneId`) REFERENCES `Zones` (`Id`) ON DELETE CASCADE
) CHARACTER SET=utf8mb4;

CREATE INDEX `IX_NPCSpawnerCells_CellId` ON `NPCSpawnerCells` (`CellId`);

CREATE INDEX `IX_NPCSpawners_CountsAsProgId` ON `NPCSpawners` (`CountsAsProgId`);

CREATE INDEX `IX_NPCSpawners_IsActiveProgId` ON `NPCSpawners` (`IsActiveProgId`);

CREATE INDEX `IX_NPCSpawners_OnSpawnProgId` ON `NPCSpawners` (`OnSpawnProgId`);

CREATE INDEX `IX_NPCSpawnerZones_ZoneId` ON `NPCSpawnerZones` (`ZoneId`);

INSERT INTO `__EFMigrationsHistory` (`MigrationId`, `ProductVersion`)
VALUES ('20220327052829_NPCSpawners', '9.0.11');

ALTER TABLE `Characteristics` DROP FOREIGN KEY `FK_Characteristics_Bodies`;

ALTER TABLE `Characteristics` ADD CONSTRAINT `FK_Characteristics_Bodies` FOREIGN KEY (`BodyId`) REFERENCES `Bodies` (`Id`) ON DELETE CASCADE;

INSERT INTO `__EFMigrationsHistory` (`MigrationId`, `ProductVersion`)
VALUES ('20220421132846_BodyCharacteristicsFix', '9.0.11');

ALTER TABLE `Clans` ADD `DiscordChannelId` decimal(20,0) NULL;

INSERT INTO `__EFMigrationsHistory` (`MigrationId`, `ProductVersion`)
VALUES ('20220625122517_ClanDiscordUpdate', '9.0.11');

ALTER TABLE `ChargenResources` MODIFY COLUMN `MaximumNumberAwardedPerAward` double NOT NULL;

INSERT INTO `__EFMigrationsHistory` (`MigrationId`, `ProductVersion`)
VALUES ('20220625125136_ChargenResourcesAsDouble', '9.0.11');

ALTER TABLE `Liquids` ADD `GasFormId` bigint(20) NULL;

ALTER TABLE `Gases` ADD `DrugGramsPerUnitVolume` double NOT NULL DEFAULT '0.0';

ALTER TABLE `Gases` ADD `DrugId` bigint(20) NULL;

CREATE INDEX `IX_Liquids_GasFormId` ON `Liquids` (`GasFormId`);

CREATE INDEX `IX_Gases_DrugId` ON `Gases` (`DrugId`);

ALTER TABLE `Gases` ADD CONSTRAINT `FK_Gases_Drugs` FOREIGN KEY (`DrugId`) REFERENCES `Drugs` (`Id`) ON DELETE SET NULL;

ALTER TABLE `Liquids` ADD CONSTRAINT `FK_Liquids_Gases` FOREIGN KEY (`GasFormId`) REFERENCES `Gases` (`Id`) ON DELETE SET NULL;

INSERT INTO `__EFMigrationsHistory` (`MigrationId`, `ProductVersion`)
VALUES ('20220718132632_MaterialsRefactor', '9.0.11');

ALTER TABLE `Crafts` ADD `IsPracticalCheck` bit(1) NOT NULL DEFAULT b'1';

INSERT INTO `__EFMigrationsHistory` (`MigrationId`, `ProductVersion`)
VALUES ('20220731064708_TheoreticalCraftChecks', '9.0.11');

ALTER TABLE `WeaponTypes` MODIFY COLUMN `ParryBonus` double NOT NULL;

INSERT INTO `__EFMigrationsHistory` (`MigrationId`, `ProductVersion`)
VALUES ('20220807101509_IntToDoubleParryBonus', '9.0.11');

ALTER TABLE `Races` ADD `DefaultHeightWeightModelFemaleId` bigint(20) NULL;

ALTER TABLE `Races` ADD `DefaultHeightWeightModelMaleId` bigint(20) NULL;

ALTER TABLE `Races` ADD `DefaultHeightWeightModelNeuterId` bigint(20) NULL;

ALTER TABLE `Races` ADD `DefaultHeightWeightModelNonBinaryId` bigint(20) NULL;

CREATE INDEX `IX_Races_DefaultHeightWeightModelFemaleId` ON `Races` (`DefaultHeightWeightModelFemaleId`);

CREATE INDEX `IX_Races_DefaultHeightWeightModelMaleId` ON `Races` (`DefaultHeightWeightModelMaleId`);

CREATE INDEX `IX_Races_DefaultHeightWeightModelNeuterId` ON `Races` (`DefaultHeightWeightModelNeuterId`);

CREATE INDEX `IX_Races_DefaultHeightWeightModelNonBinaryId` ON `Races` (`DefaultHeightWeightModelNonBinaryId`);

ALTER TABLE `Races` ADD CONSTRAINT `FK_Races_HeightWeightModelsFemale` FOREIGN KEY (`DefaultHeightWeightModelFemaleId`) REFERENCES `HeightWeightModels` (`Id`);

ALTER TABLE `Races` ADD CONSTRAINT `FK_Races_HeightWeightModelsMale` FOREIGN KEY (`DefaultHeightWeightModelMaleId`) REFERENCES `HeightWeightModels` (`Id`);

ALTER TABLE `Races` ADD CONSTRAINT `FK_Races_HeightWeightModelsNeuter` FOREIGN KEY (`DefaultHeightWeightModelNeuterId`) REFERENCES `HeightWeightModels` (`Id`);

ALTER TABLE `Races` ADD CONSTRAINT `FK_Races_HeightWeightModelsNonBinary` FOREIGN KEY (`DefaultHeightWeightModelNonBinaryId`) REFERENCES `HeightWeightModels` (`Id`);

INSERT INTO `__EFMigrationsHistory` (`MigrationId`, `ProductVersion`)
VALUES ('20220814231930_RaceDefaultHwModels', '9.0.11');

ALTER TABLE `Shops` ADD `BankAccountId` bigint(20) NULL;

ALTER TABLE `Shops` ADD `CashBalance` decimal(58,29) NOT NULL DEFAULT 0.0;

CREATE INDEX `IX_Shops_BankAccountId` ON `Shops` (`BankAccountId`);

ALTER TABLE `Shops` ADD CONSTRAINT `FK_Shops_BankAccounts` FOREIGN KEY (`BankAccountId`) REFERENCES `BankAccounts` (`Id`) ON DELETE SET NULL;

INSERT INTO `__EFMigrationsHistory` (`MigrationId`, `ProductVersion`)
VALUES ('20221030044209_ShopBankAccountsAndFinance', '9.0.11');

ALTER TABLE `BankAccountTypes` ADD `NumberOfPermittedPaymentItems` int NOT NULL DEFAULT 0;

ALTER TABLE `BankAccountTypes` ADD `PaymentItemPrototypeId` bigint NULL;

ALTER TABLE `BankAccounts` ADD `AuthorisedBankPaymentItems` longtext CHARACTER SET utf8mb4 NULL;

INSERT INTO `__EFMigrationsHistory` (`MigrationId`, `ProductVersion`)
VALUES ('20221030125929_BankPaymentsAtShops', '9.0.11');

ALTER TABLE `MagicSpells` ADD `AppliedEffectsAreExclusive` tinyint(1) NOT NULL DEFAULT FALSE;

INSERT INTO `__EFMigrationsHistory` (`MigrationId`, `ProductVersion`)
VALUES ('20221031113757_MagicSpellExclusivity', '9.0.11');

ALTER TABLE `ActiveProjects` DROP FOREIGN KEY `FK_ActiveProjects_Characters`;

ALTER TABLE `Cultures` DROP FOREIGN KEY `FK_Cultures_NameCulture`;

ALTER TABLE `Cultures` DROP INDEX `FK_Cultures_NameCulture`;

CREATE TABLE `CulturesNameCultures` (
    `CultureId` bigint(20) NOT NULL,
    `NameCultureId` bigint(20) NOT NULL,
    `Gender` smallint(6) NOT NULL,
    CONSTRAINT `PRIMARY` PRIMARY KEY (`CultureId`, `NameCultureId`, `Gender`),
    CONSTRAINT `FK_CulturesNameCultures_Cultures` FOREIGN KEY (`CultureId`) REFERENCES `Cultures` (`Id`) ON DELETE CASCADE,
    CONSTRAINT `FK_CulturesNameCultures_NameCultures` FOREIGN KEY (`NameCultureId`) REFERENCES `NameCulture` (`Id`) ON DELETE CASCADE
) CHARACTER SET=utf8mb4;

CREATE INDEX `IX_CulturesNameCultures_NameCultureId` ON `CulturesNameCultures` (`NameCultureId`);

INSERT INTO CulturesNameCultures (CultureId, NameCultureId, Gender)
SELECT Id, NameCultureId, 0
FROM Cultures

INSERT INTO CulturesNameCultures (CultureId, NameCultureId, Gender)
SELECT Id, NameCultureId, 1
FROM Cultures

INSERT INTO CulturesNameCultures (CultureId, NameCultureId, Gender)
SELECT Id, NameCultureId, 2
FROM Cultures

INSERT INTO CulturesNameCultures (CultureId, NameCultureId, Gender)
SELECT Id, NameCultureId, 3
FROM Cultures

INSERT INTO CulturesNameCultures (CultureId, NameCultureId, Gender)
SELECT Id, NameCultureId, 4
FROM Cultures

ALTER TABLE `ActiveProjects` ADD CONSTRAINT `FK_ActiveProjects_Characters` FOREIGN KEY (`CharacterId`) REFERENCES `Characters` (`Id`) ON DELETE CASCADE;

ALTER TABLE `Cultures` DROP COLUMN `NameCultureId`;

INSERT INTO `__EFMigrationsHistory` (`MigrationId`, `ProductVersion`)
VALUES ('20221201081057_NameCulturesGenderExpansion', '9.0.11');

ALTER TABLE `RandomNameProfiles` ADD `UseForChargenSuggestionsProgId` bigint(20) NULL;

CREATE INDEX `IX_RandomNameProfiles_UseForChargenSuggestionsProgId` ON `RandomNameProfiles` (`UseForChargenSuggestionsProgId`);

ALTER TABLE `RandomNameProfiles` ADD CONSTRAINT `FK_RandomNameProfiles_FutureProgs_UseForChargenSuggestionsProgId` FOREIGN KEY (`UseForChargenSuggestionsProgId`) REFERENCES `FutureProgs` (`Id`);

INSERT INTO `__EFMigrationsHistory` (`MigrationId`, `ProductVersion`)
VALUES ('20221201133628_NameCulturesChargenExpansion', '9.0.11');

ALTER TABLE `CurrencyDivisions` ADD `IgnoreCase` bit(1) NOT NULL DEFAULT 1;

INSERT INTO `__EFMigrationsHistory` (`MigrationId`, `ProductVersion`)
VALUES ('20230101133831_CurrencyPatternRegexCaseFix', '9.0.11');

ALTER TABLE `Liquids` ADD `RelativeEnthalpy` double NOT NULL DEFAULT '1.0';

INSERT INTO `__EFMigrationsHistory` (`MigrationId`, `ProductVersion`)
VALUES ('20230110120837_RelativeEnthalpyForLiquids', '9.0.11');

ALTER TABLE `SurgicalProcedures` ADD `CheckTraitDefinitionId` bigint(20) NULL;

CREATE INDEX `IX_SurgicalProcedures_CheckTraitDefinitionId` ON `SurgicalProcedures` (`CheckTraitDefinitionId`);

ALTER TABLE `SurgicalProcedures` ADD CONSTRAINT `FK_SurgicalProcedures_TraitDefinitions` FOREIGN KEY (`CheckTraitDefinitionId`) REFERENCES `TraitDefinitions` (`Id`) ON DELETE SET NULL;

INSERT INTO `__EFMigrationsHistory` (`MigrationId`, `ProductVersion`)
VALUES ('20230124124618_SurgicalProcedureCheckTraits', '9.0.11');

ALTER TABLE `SurgicalProcedures` ADD `TargetBodyTypeId` bigint(20) NOT NULL DEFAULT 0;

CREATE INDEX `IX_SurgicalProcedures_TargetBodyTypeId` ON `SurgicalProcedures` (`TargetBodyTypeId`);

UPDATE SurgicalProcedures
SET TargetBodyTypeId = 1
WHERE Id > 0

ALTER TABLE `SurgicalProcedures` ADD CONSTRAINT `FK_SurgicalProcedures_BodyProtos` FOREIGN KEY (`TargetBodyTypeId`) REFERENCES `BodyProtos` (`Id`) ON DELETE CASCADE;

INSERT INTO `__EFMigrationsHistory` (`MigrationId`, `ProductVersion`)
VALUES ('20230311060208_SurgeryBodyUpdate', '9.0.11');

CREATE TABLE `GPTThreads` (
    `Id` bigint(20) NOT NULL AUTO_INCREMENT,
    `Name` varchar(200) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
    `Prompt` text CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
    `Model` varchar(200) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
    `Temperature` double NOT NULL,
    CONSTRAINT `PK_GPTThreads` PRIMARY KEY (`Id`)
) CHARACTER SET=utf8mb4;

CREATE TABLE `GPTMessages` (
    `Id` bigint(20) NOT NULL AUTO_INCREMENT,
    `GPTThreadId` bigint(20) NOT NULL,
    `CharacterId` bigint(20) NULL,
    `Message` text CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
    `Response` text CHARACTER SET utf8 COLLATE utf8_general_ci NULL,
    CONSTRAINT `PK_GPTMessages` PRIMARY KEY (`Id`),
    CONSTRAINT `FK_GPTMessages_Characters` FOREIGN KEY (`CharacterId`) REFERENCES `Characters` (`Id`) ON DELETE CASCADE,
    CONSTRAINT `FK_GPTMessages_GPTThreads` FOREIGN KEY (`GPTThreadId`) REFERENCES `GPTThreads` (`Id`) ON DELETE CASCADE
) CHARACTER SET=utf8mb4;

CREATE INDEX `IX_GPTMessages_CharacterId` ON `GPTMessages` (`CharacterId`);

CREATE INDEX `IX_GPTMessages_GPTThreadId` ON `GPTMessages` (`GPTThreadId`);

INSERT INTO `__EFMigrationsHistory` (`MigrationId`, `ProductVersion`)
VALUES ('20230407151210_OpenAIv1', '9.0.11');

ALTER TABLE `GameItemProtos` ADD `CostInBaseCurrency` decimal(58,29) NOT NULL DEFAULT '0';

ALTER TABLE `Currencies` ADD `BaseCurrencyToGlobalBaseCurrencyConversion` decimal(58,29) NOT NULL DEFAULT 1.0;

INSERT INTO `__EFMigrationsHistory` (`MigrationId`, `ProductVersion`)
VALUES ('20230428004425_GlobalCurrencyChanges', '9.0.11');

ALTER TABLE `CraftTools` ADD `UseToolDuration` bit(1) NOT NULL DEFAULT 1;

INSERT INTO `__EFMigrationsHistory` (`MigrationId`, `ProductVersion`)
VALUES ('20230603125906_CraftUseToolDuration', '9.0.11');

ALTER TABLE `CharacterCombatSettings` RENAME COLUMN `AuxillaryPercentage` TO `AuxiliaryPercentage`;

CREATE TABLE `CombatActions` (
    `Id` bigint NOT NULL AUTO_INCREMENT,
    `Name` longtext CHARACTER SET utf8mb4 NULL,
    `UsabilityProgId` bigint(20) NULL,
    `RecoveryDifficultySuccess` int NOT NULL,
    `RecoveryDifficultyFailure` int NOT NULL,
    `MoveType` int NOT NULL,
    `Intentions` bigint NOT NULL,
    `ExertionLevel` int NOT NULL,
    `Weighting` double NOT NULL,
    `StaminaCost` double NOT NULL,
    `BaseDelay` double NOT NULL,
    `AdditionalInfo` longtext CHARACTER SET utf8mb4 NULL,
    `RequiredPositionStateIds` longtext CHARACTER SET utf8mb4 NULL,
    `MoveDifficulty` int NOT NULL,
    `TraitDefinitionId` bigint(20) NOT NULL,
    CONSTRAINT `PK_CombatActions` PRIMARY KEY (`Id`),
    CONSTRAINT `FK_CombatActions_FutureProgs_UsabilityProgId` FOREIGN KEY (`UsabilityProgId`) REFERENCES `FutureProgs` (`Id`),
    CONSTRAINT `FK_CombatActions_TraitDefinitions_TraitDefinitionId` FOREIGN KEY (`TraitDefinitionId`) REFERENCES `TraitDefinitions` (`Id`) ON DELETE CASCADE
) CHARACTER SET=utf8mb4;

CREATE TABLE `CombatMessages_CombatActions` (
    `CombatMessageId` bigint(20) NOT NULL,
    `CombatActionId` bigint(20) NOT NULL,
    CONSTRAINT `PRIMARY` PRIMARY KEY (`CombatMessageId`, `CombatActionId`),
    CONSTRAINT `FK_CombatMessages_CombatActions_CombatMessages` FOREIGN KEY (`CombatMessageId`) REFERENCES `CombatMessages` (`Id`) ON DELETE CASCADE,
    CONSTRAINT `FK_CombatMessages_CombatActions_WeaponAttacks` FOREIGN KEY (`CombatActionId`) REFERENCES `CombatActions` (`Id`) ON DELETE CASCADE
) CHARACTER SET=utf8mb4;

CREATE INDEX `IX_CombatActions_TraitDefinitionId` ON `CombatActions` (`TraitDefinitionId`);

CREATE INDEX `IX_CombatActions_UsabilityProgId` ON `CombatActions` (`UsabilityProgId`);

CREATE INDEX `FK_CombatMessages_CombatActions_WeaponAttacks_idx` ON `CombatMessages_CombatActions` (`CombatActionId`);

INSERT INTO `__EFMigrationsHistory` (`MigrationId`, `ProductVersion`)
VALUES ('20230706055610_AuxiliaryMoves', '9.0.11');

ALTER TABLE `CombatMessages` ADD `AuxiliaryProgId` bigint(20) NULL;

CREATE TABLE `Races_CombatActions` (
    `RaceId` bigint(20) NOT NULL,
    `CombatActionId` bigint(20) NOT NULL,
    CONSTRAINT `PRIMARY` PRIMARY KEY (`RaceId`, `CombatActionId`),
    CONSTRAINT `FK_Races_CombatActions_CombatActions` FOREIGN KEY (`CombatActionId`) REFERENCES `CombatActions` (`Id`) ON DELETE CASCADE,
    CONSTRAINT `FK_Races_CombatActions_Races` FOREIGN KEY (`RaceId`) REFERENCES `Races` (`Id`) ON DELETE CASCADE
) CHARACTER SET=utf8mb4;

CREATE INDEX `IX_CombatMessages_AuxiliaryProgId` ON `CombatMessages` (`AuxiliaryProgId`);

CREATE INDEX `IX_Races_CombatActions_CombatActionId` ON `Races_CombatActions` (`CombatActionId`);

ALTER TABLE `CombatMessages` ADD CONSTRAINT `FK_CombatMessages_FutureProgs_Auxiliary` FOREIGN KEY (`AuxiliaryProgId`) REFERENCES `FutureProgs` (`Id`) ON DELETE SET NULL;

INSERT INTO `__EFMigrationsHistory` (`MigrationId`, `ProductVersion`)
VALUES ('20230714035824_AuxiliaryMoves2', '9.0.11');

ALTER TABLE `Seasons` DROP COLUMN `Description`;

ALTER TABLE `Seasons` ADD `DisplayName` varchar(200) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL DEFAULT '';

ALTER TABLE `Seasons` ADD `SeasonGroup` varchar(200) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL DEFAULT '';

INSERT INTO `__EFMigrationsHistory` (`MigrationId`, `ProductVersion`)
VALUES ('20230727121209_SeasonsDisplayUpdate', '9.0.11');

CREATE TABLE `SeederChoices` (
    `Id` bigint NOT NULL AUTO_INCREMENT,
    `Version` longtext CHARACTER SET utf8mb4 NULL,
    `Seeder` longtext CHARACTER SET utf8mb4 NULL,
    `Choice` longtext CHARACTER SET utf8mb4 NULL,
    `Answer` longtext CHARACTER SET utf8mb4 NULL,
    `DateTime` datetime(6) NOT NULL,
    CONSTRAINT `PK_SeederChoices` PRIMARY KEY (`Id`)
) CHARACTER SET=utf8mb4;

INSERT INTO `__EFMigrationsHistory` (`MigrationId`, `ProductVersion`)
VALUES ('20230731055842_SeederChoices', '9.0.11');

ALTER TABLE `ActiveJobs` DROP FOREIGN KEY `FK_ActiveJobs_Characters`;

ALTER TABLE `ActiveJobs` DROP FOREIGN KEY `FK_ActiveJobs_JobListings`;

ALTER TABLE `AuctionHouses` DROP FOREIGN KEY `FK_AuctionHouses_BankAccounts`;

ALTER TABLE `AuctionHouses` DROP FOREIGN KEY `FK_AuctionHouses_Cells`;

ALTER TABLE `AuctionHouses` DROP FOREIGN KEY `FK_AuctionHouses_EconomicZones`;

ALTER TABLE `BankAccounts` DROP FOREIGN KEY `FK_BankAccounts_BankAccountTypes`;

ALTER TABLE `BankAccounts` DROP FOREIGN KEY `FK_BankAccounts_BankAccounts`;

ALTER TABLE `BankAccounts` DROP FOREIGN KEY `FK_BankAccounts_Banks`;

ALTER TABLE `BankAccounts` DROP FOREIGN KEY `FK_BankAccounts_Characters`;

ALTER TABLE `BankAccounts` DROP FOREIGN KEY `FK_BankAccounts_Clans`;

ALTER TABLE `BankAccounts` DROP FOREIGN KEY `FK_BankAccounts_Shops`;

ALTER TABLE `BankAccountTransactions` DROP FOREIGN KEY `FK_BankAccountTransactions_BankAccounts`;

ALTER TABLE `BankAccountTypes` DROP FOREIGN KEY `FK_BankAccountTypes_Banks`;

ALTER TABLE `BankAccountTypes` DROP FOREIGN KEY `FK_BankAccountTypes_CanCloseProg`;

ALTER TABLE `BankAccountTypes` DROP FOREIGN KEY `FK_BankAccountTypes_CharacterProgs`;

ALTER TABLE `BankAccountTypes` DROP FOREIGN KEY `FK_BankAccountTypes_ClanProgs`;

ALTER TABLE `BankAccountTypes` DROP FOREIGN KEY `FK_BankAccountTypes_ShopProgs`;

ALTER TABLE `BankBranches` DROP FOREIGN KEY `FK_BankBranches_Banks`;

ALTER TABLE `BankBranches` DROP FOREIGN KEY `FK_BankBranches_Cells`;

ALTER TABLE `BankCurrencyReserves` DROP FOREIGN KEY `FK_BankCurrencyReserves_Banks`;

ALTER TABLE `BankCurrencyReserves` DROP FOREIGN KEY `FK_BankCurrencyReserves_Currencies`;

ALTER TABLE `BankExchangeRates` DROP FOREIGN KEY `FK_BankExchangeRates_Banks`;

ALTER TABLE `BankExchangeRates` DROP FOREIGN KEY `FK_BankExchangeRates_Currencies_From`;

ALTER TABLE `BankExchangeRates` DROP FOREIGN KEY `FK_BankExchangeRates_Currencies_To`;

ALTER TABLE `BankManagerAuditLogs` DROP FOREIGN KEY `FK_BankManagerAuditLogs_Banks`;

ALTER TABLE `BankManagers` DROP FOREIGN KEY `FK_BankManagers_Banks`;

ALTER TABLE `BankManagers` DROP FOREIGN KEY `FK_BankManagers_Characters`;

ALTER TABLE `CellOverlays` DROP FOREIGN KEY `FK_CellOverlays_CellOverlayPackages`;

ALTER TABLE `CellOverlays` DROP FOREIGN KEY `FK_CellOverlays_Cells`;

ALTER TABLE `CellOverlays` DROP FOREIGN KEY `FK_CellOverlays_Terrains`;

ALTER TABLE `CellOverlays_Exits` DROP FOREIGN KEY `FK_CellOverlays_Exits_CellOverlays`;

ALTER TABLE `CellOverlays_Exits` DROP FOREIGN KEY `FK_CellOverlays_Exits_Exits`;

ALTER TABLE `Cells` DROP FOREIGN KEY `FK_Cells_Rooms`;

ALTER TABLE `Cells_Tags` DROP FOREIGN KEY `FK_Cells_Tags_Cells`;

ALTER TABLE `ConveyancingLocations` DROP FOREIGN KEY `FK_ConveyancingLocations_Cells`;

ALTER TABLE `ConveyancingLocations` DROP FOREIGN KEY `FK_ConveyancingLocations_EconomicZones`;

ALTER TABLE `JobFindingLocations` DROP FOREIGN KEY `FK_JobFindingLocations_Cells`;

ALTER TABLE `JobFindingLocations` DROP FOREIGN KEY `FK_JobFindingLocations_EconomicZones`;

ALTER TABLE `JobListings` DROP FOREIGN KEY `FK_JobListings_EconomicZones`;

ALTER TABLE `PropertyKeys` DROP FOREIGN KEY `FK_PropertyKeys_GameItems`;

ALTER TABLE `PropertyKeys` DROP FOREIGN KEY `FK_PropertyKeys_Property`;

ALTER TABLE `ActiveJobs` ADD CONSTRAINT `FK_ActiveJobs_Characters` FOREIGN KEY (`CharacterId`) REFERENCES `Characters` (`Id`) ON DELETE CASCADE;

ALTER TABLE `ActiveJobs` ADD CONSTRAINT `FK_ActiveJobs_JobListings` FOREIGN KEY (`JobListingId`) REFERENCES `JobListings` (`Id`) ON DELETE CASCADE;

ALTER TABLE `AuctionHouses` ADD CONSTRAINT `FK_AuctionHouses_BankAccounts` FOREIGN KEY (`ProfitsBankAccountId`) REFERENCES `BankAccounts` (`Id`) ON DELETE CASCADE;

ALTER TABLE `AuctionHouses` ADD CONSTRAINT `FK_AuctionHouses_Cells` FOREIGN KEY (`AuctionHouseCellId`) REFERENCES `Cells` (`Id`) ON DELETE CASCADE;

ALTER TABLE `AuctionHouses` ADD CONSTRAINT `FK_AuctionHouses_EconomicZones` FOREIGN KEY (`EconomicZoneId`) REFERENCES `EconomicZones` (`Id`) ON DELETE CASCADE;

ALTER TABLE `BankAccounts` ADD CONSTRAINT `FK_BankAccounts_BankAccountTypes` FOREIGN KEY (`BankAccountTypeId`) REFERENCES `BankAccountTypes` (`Id`) ON DELETE CASCADE;

ALTER TABLE `BankAccounts` ADD CONSTRAINT `FK_BankAccounts_BankAccounts` FOREIGN KEY (`NominatedBenefactorAccountId`) REFERENCES `BankAccounts` (`Id`) ON DELETE CASCADE;

ALTER TABLE `BankAccounts` ADD CONSTRAINT `FK_BankAccounts_Banks` FOREIGN KEY (`BankId`) REFERENCES `Banks` (`Id`) ON DELETE CASCADE;

ALTER TABLE `BankAccounts` ADD CONSTRAINT `FK_BankAccounts_Characters` FOREIGN KEY (`AccountOwnerCharacterId`) REFERENCES `Characters` (`Id`) ON DELETE CASCADE;

ALTER TABLE `BankAccounts` ADD CONSTRAINT `FK_BankAccounts_Clans` FOREIGN KEY (`AccountOwnerClanId`) REFERENCES `Clans` (`Id`) ON DELETE CASCADE;

ALTER TABLE `BankAccounts` ADD CONSTRAINT `FK_BankAccounts_Shops` FOREIGN KEY (`AccountOwnerShopId`) REFERENCES `Shops` (`Id`) ON DELETE CASCADE;

ALTER TABLE `BankAccountTransactions` ADD CONSTRAINT `FK_BankAccountTransactions_BankAccounts` FOREIGN KEY (`BankAccountId`) REFERENCES `BankAccounts` (`Id`) ON DELETE CASCADE;

ALTER TABLE `BankAccountTypes` ADD CONSTRAINT `FK_BankAccountTypes_Banks` FOREIGN KEY (`BankId`) REFERENCES `Banks` (`Id`) ON DELETE CASCADE;

ALTER TABLE `BankAccountTypes` ADD CONSTRAINT `FK_BankAccountTypes_CanCloseProg` FOREIGN KEY (`CanCloseAccountProgId`) REFERENCES `FutureProgs` (`Id`) ON DELETE CASCADE;

ALTER TABLE `BankAccountTypes` ADD CONSTRAINT `FK_BankAccountTypes_CharacterProgs` FOREIGN KEY (`CanOpenAccountProgCharacterId`) REFERENCES `FutureProgs` (`Id`) ON DELETE CASCADE;

ALTER TABLE `BankAccountTypes` ADD CONSTRAINT `FK_BankAccountTypes_ClanProgs` FOREIGN KEY (`CanOpenAccountProgClanId`) REFERENCES `FutureProgs` (`Id`) ON DELETE CASCADE;

ALTER TABLE `BankAccountTypes` ADD CONSTRAINT `FK_BankAccountTypes_ShopProgs` FOREIGN KEY (`CanOpenAccountProgShopId`) REFERENCES `FutureProgs` (`Id`) ON DELETE CASCADE;

ALTER TABLE `BankBranches` ADD CONSTRAINT `FK_BankBranches_Banks` FOREIGN KEY (`BankId`) REFERENCES `Banks` (`Id`) ON DELETE CASCADE;

ALTER TABLE `BankBranches` ADD CONSTRAINT `FK_BankBranches_Cells` FOREIGN KEY (`CellId`) REFERENCES `Cells` (`Id`) ON DELETE CASCADE;

ALTER TABLE `BankCurrencyReserves` ADD CONSTRAINT `FK_BankCurrencyReserves_Banks` FOREIGN KEY (`BankId`) REFERENCES `Banks` (`Id`) ON DELETE CASCADE;

ALTER TABLE `BankCurrencyReserves` ADD CONSTRAINT `FK_BankCurrencyReserves_Currencies` FOREIGN KEY (`CurrencyId`) REFERENCES `Currencies` (`Id`) ON DELETE CASCADE;

ALTER TABLE `BankExchangeRates` ADD CONSTRAINT `FK_BankExchangeRates_Banks` FOREIGN KEY (`BankId`) REFERENCES `Banks` (`Id`) ON DELETE CASCADE;

ALTER TABLE `BankExchangeRates` ADD CONSTRAINT `FK_BankExchangeRates_Currencies_From` FOREIGN KEY (`FromCurrencyId`) REFERENCES `Currencies` (`Id`) ON DELETE CASCADE;

ALTER TABLE `BankExchangeRates` ADD CONSTRAINT `FK_BankExchangeRates_Currencies_To` FOREIGN KEY (`ToCurrencyId`) REFERENCES `Currencies` (`Id`) ON DELETE CASCADE;

ALTER TABLE `BankManagerAuditLogs` ADD CONSTRAINT `FK_BankManagerAuditLogs_Banks` FOREIGN KEY (`BankId`) REFERENCES `Banks` (`Id`) ON DELETE CASCADE;

ALTER TABLE `BankManagers` ADD CONSTRAINT `FK_BankManagers_Banks` FOREIGN KEY (`BankId`) REFERENCES `Banks` (`Id`) ON DELETE CASCADE;

ALTER TABLE `BankManagers` ADD CONSTRAINT `FK_BankManagers_Characters` FOREIGN KEY (`CharacterId`) REFERENCES `Characters` (`Id`) ON DELETE CASCADE;

ALTER TABLE `CellOverlays` ADD CONSTRAINT `FK_CellOverlays_CellOverlayPackages` FOREIGN KEY (`CellOverlayPackageId`, `CellOverlayPackageRevisionNumber`) REFERENCES `CellOverlayPackages` (`Id`, `RevisionNumber`) ON DELETE CASCADE;

ALTER TABLE `CellOverlays` ADD CONSTRAINT `FK_CellOverlays_Cells` FOREIGN KEY (`CellId`) REFERENCES `Cells` (`Id`) ON DELETE CASCADE;

ALTER TABLE `CellOverlays` ADD CONSTRAINT `FK_CellOverlays_Terrains` FOREIGN KEY (`TerrainId`) REFERENCES `Terrains` (`Id`) ON DELETE RESTRICT;

ALTER TABLE `CellOverlays_Exits` ADD CONSTRAINT `FK_CellOverlays_Exits_CellOverlays` FOREIGN KEY (`CellOverlayId`) REFERENCES `CellOverlays` (`Id`) ON DELETE CASCADE;

ALTER TABLE `CellOverlays_Exits` ADD CONSTRAINT `FK_CellOverlays_Exits_Exits` FOREIGN KEY (`ExitId`) REFERENCES `Exits` (`Id`) ON DELETE CASCADE;

ALTER TABLE `Cells` ADD CONSTRAINT `FK_Cells_Rooms` FOREIGN KEY (`RoomId`) REFERENCES `Rooms` (`Id`) ON DELETE CASCADE;

ALTER TABLE `Cells_Tags` ADD CONSTRAINT `FK_Cells_Tags_Cells` FOREIGN KEY (`CellId`) REFERENCES `Cells` (`Id`) ON DELETE CASCADE;

ALTER TABLE `ConveyancingLocations` ADD CONSTRAINT `FK_ConveyancingLocations_Cells` FOREIGN KEY (`CellId`) REFERENCES `Cells` (`Id`) ON DELETE CASCADE;

ALTER TABLE `ConveyancingLocations` ADD CONSTRAINT `FK_ConveyancingLocations_EconomicZones` FOREIGN KEY (`EconomicZoneId`) REFERENCES `EconomicZones` (`Id`) ON DELETE CASCADE;

ALTER TABLE `JobFindingLocations` ADD CONSTRAINT `FK_JobFindingLocations_Cells` FOREIGN KEY (`CellId`) REFERENCES `Cells` (`Id`) ON DELETE CASCADE;

ALTER TABLE `JobFindingLocations` ADD CONSTRAINT `FK_JobFindingLocations_EconomicZones` FOREIGN KEY (`EconomicZoneId`) REFERENCES `EconomicZones` (`Id`) ON DELETE CASCADE;

ALTER TABLE `JobListings` ADD CONSTRAINT `FK_JobListings_EconomicZones` FOREIGN KEY (`EconomicZoneId`) REFERENCES `EconomicZones` (`Id`) ON DELETE CASCADE;

ALTER TABLE `PropertyKeys` ADD CONSTRAINT `FK_PropertyKeys_GameItems` FOREIGN KEY (`GameItemId`) REFERENCES `GameItems` (`Id`) ON DELETE CASCADE;

ALTER TABLE `PropertyKeys` ADD CONSTRAINT `FK_PropertyKeys_Property` FOREIGN KEY (`PropertyId`) REFERENCES `Properties` (`Id`) ON DELETE CASCADE;

INSERT INTO `__EFMigrationsHistory` (`MigrationId`, `ProductVersion`)
VALUES ('20230810071403_CellForeignKeyUpdates', '9.0.11');

ALTER TABLE `NPCSpawners` ADD `Definition` longtext CHARACTER SET utf8mb4 NULL;

INSERT INTO `__EFMigrationsHistory` (`MigrationId`, `ProductVersion`)
VALUES ('20230825052231_NpcSpawnerMulti', '9.0.11');

ALTER TABLE `Shops` ADD `ShopType` varchar(200) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL DEFAULT 'Permanent';

INSERT INTO `__EFMigrationsHistory` (`MigrationId`, `ProductVersion`)
VALUES ('20230825061651_ShopTypes', '9.0.11');

CREATE TABLE `ScriptedEvents` (
    `Id` bigint(20) NOT NULL AUTO_INCREMENT,
    `Name` mediumtext CHARACTER SET utf8 COLLATE utf8_general_ci NULL,
    `CharacterId` bigint(20) NULL,
    `CharacterFilterProgId` bigint(20) NULL,
    `IsReady` bit(1) NOT NULL,
    `EarliestDate` datetime NOT NULL,
    `IsFinished` bit(1) NOT NULL,
    `IsTemplate` bit(1) NOT NULL,
    CONSTRAINT `PK_ScriptedEvents` PRIMARY KEY (`Id`),
    CONSTRAINT `FK_ScriptedEvents_Characters` FOREIGN KEY (`CharacterId`) REFERENCES `Characters` (`Id`),
    CONSTRAINT `FK_ScriptedEvents_FutureProgs` FOREIGN KEY (`CharacterFilterProgId`) REFERENCES `FutureProgs` (`Id`)
) CHARACTER SET=utf8mb4;

CREATE TABLE `ScriptedEventFreeTextQuestions` (
    `Id` bigint(20) NOT NULL AUTO_INCREMENT,
    `ScriptedEventId` bigint(20) NOT NULL,
    `Question` mediumtext CHARACTER SET utf8 COLLATE utf8_general_ci NULL,
    `Answer` mediumtext CHARACTER SET utf8 COLLATE utf8_general_ci NULL,
    CONSTRAINT `PK_ScriptedEventFreeTextQuestions` PRIMARY KEY (`Id`),
    CONSTRAINT `FK_ScriptedEventFreeTextQuestions_ScriptedEvents` FOREIGN KEY (`ScriptedEventId`) REFERENCES `ScriptedEvents` (`Id`) ON DELETE CASCADE
) CHARACTER SET=utf8mb4;

CREATE TABLE `ScriptedEventMultipleChoiceQuestionAnswers` (
    `Id` bigint(20) NOT NULL AUTO_INCREMENT,
    `ScriptedEventMultipleChoiceQuestionId` bigint(20) NOT NULL,
    `DescriptionBeforeChoice` mediumtext CHARACTER SET utf8 COLLATE utf8_general_ci NULL,
    `DescriptionAfterChoice` mediumtext CHARACTER SET utf8 COLLATE utf8_general_ci NULL,
    `AnswerFilterProgId` bigint(20) NULL,
    `AfterChoiceProgId` bigint(20) NULL,
    CONSTRAINT `PK_ScriptedEventMultipleChoiceQuestionAnswers` PRIMARY KEY (`Id`),
    CONSTRAINT `FK_ScriptedEventMultipleChoiceQuestionAnswers_FutureProgs_After` FOREIGN KEY (`AfterChoiceProgId`) REFERENCES `FutureProgs` (`Id`),
    CONSTRAINT `FK_ScriptedEventMultipleChoiceQuestionAnswers_FutureProgs_Filter` FOREIGN KEY (`AnswerFilterProgId`) REFERENCES `FutureProgs` (`Id`)
) CHARACTER SET=utf8mb4;

CREATE TABLE `ScriptedEventMultipleChoiceQuestions` (
    `Id` bigint(20) NOT NULL AUTO_INCREMENT,
    `ScriptedEventId` bigint(20) NOT NULL,
    `Question` mediumtext CHARACTER SET utf8 COLLATE utf8_general_ci NULL,
    `ChosenAnswerId` bigint(20) NULL,
    CONSTRAINT `PK_ScriptedEventMultipleChoiceQuestions` PRIMARY KEY (`Id`),
    CONSTRAINT `FK_ScriptedEventMultipleChoiceQuestions_ScriptedEventMultipleCho` FOREIGN KEY (`ChosenAnswerId`) REFERENCES `ScriptedEventMultipleChoiceQuestionAnswers` (`Id`),
    CONSTRAINT `FK_ScriptedEventMultipleChoiceQuestions_ScriptedEvents` FOREIGN KEY (`ScriptedEventId`) REFERENCES `ScriptedEvents` (`Id`) ON DELETE CASCADE
) CHARACTER SET=utf8mb4;

CREATE INDEX `IX_ScriptedEventFreeTextQuestions_ScriptedEventId` ON `ScriptedEventFreeTextQuestions` (`ScriptedEventId`);

CREATE INDEX `IX_ScriptedEventMultipleChoiceQuestionAnswers_AfterChoiceProgId` ON `ScriptedEventMultipleChoiceQuestionAnswers` (`AfterChoiceProgId`);

CREATE INDEX `IX_ScriptedEventMultipleChoiceQuestionAnswers_AnswerFilterProgId` ON `ScriptedEventMultipleChoiceQuestionAnswers` (`AnswerFilterProgId`);

CREATE INDEX `IX_ScriptedEventMultipleChoiceQuestionAnswers_ScriptedEventMult~` ON `ScriptedEventMultipleChoiceQuestionAnswers` (`ScriptedEventMultipleChoiceQuestionId`);

CREATE INDEX `IX_ScriptedEventMultipleChoiceQuestions_ChosenAnswerId` ON `ScriptedEventMultipleChoiceQuestions` (`ChosenAnswerId`);

CREATE INDEX `IX_ScriptedEventMultipleChoiceQuestions_ScriptedEventId` ON `ScriptedEventMultipleChoiceQuestions` (`ScriptedEventId`);

CREATE INDEX `IX_ScriptedEvents_CharacterFilterProgId` ON `ScriptedEvents` (`CharacterFilterProgId`);

CREATE INDEX `IX_ScriptedEvents_CharacterId` ON `ScriptedEvents` (`CharacterId`);

ALTER TABLE `ScriptedEventMultipleChoiceQuestionAnswers` ADD CONSTRAINT `FK_ScriptedEventMultipleChoiceQuestionAnswers_ScriptedEventMulti` FOREIGN KEY (`ScriptedEventMultipleChoiceQuestionId`) REFERENCES `ScriptedEventMultipleChoiceQuestions` (`Id`) ON DELETE CASCADE;

INSERT INTO `__EFMigrationsHistory` (`MigrationId`, `ProductVersion`)
VALUES ('20230914142042_ScriptedEvents', '9.0.11');

ALTER TABLE `ExternalClanControls` DROP FOREIGN KEY `FK_ECC_Appointments_Controlled`;

ALTER TABLE `ExternalClanControls` DROP FOREIGN KEY `FK_ECC_Appointments_Controlling`;

ALTER TABLE `ExternalClanControls` DROP FOREIGN KEY `FK_ECC_Clans_Liege`;

ALTER TABLE `ExternalClanControls` DROP FOREIGN KEY `FK_ECC_Clans_Vassal`;

ALTER TABLE `ExternalClanControls_Appointments` DROP FOREIGN KEY `FK_ECC_Appointments_ExternalClanControls`;

ALTER TABLE `ExternalClanControls` ADD CONSTRAINT `FK_ECC_Appointments_Controlled` FOREIGN KEY (`ControlledAppointmentId`) REFERENCES `Appointments` (`Id`) ON DELETE CASCADE;

ALTER TABLE `ExternalClanControls` ADD CONSTRAINT `FK_ECC_Appointments_Controlling` FOREIGN KEY (`ControllingAppointmentId`) REFERENCES `Appointments` (`Id`) ON DELETE CASCADE;

ALTER TABLE `ExternalClanControls` ADD CONSTRAINT `FK_ECC_Clans_Liege` FOREIGN KEY (`LiegeClanId`) REFERENCES `Clans` (`Id`) ON DELETE CASCADE;

ALTER TABLE `ExternalClanControls` ADD CONSTRAINT `FK_ECC_Clans_Vassal` FOREIGN KEY (`VassalClanId`) REFERENCES `Clans` (`Id`) ON DELETE CASCADE;

ALTER TABLE `ExternalClanControls_Appointments` ADD CONSTRAINT `FK_ECC_Appointments_ExternalClanControls` FOREIGN KEY (`VassalClanId`, `LiegeClanId`, `ControlledAppointmentId`) REFERENCES `ExternalClanControls` (`VassalClanId`, `LiegeClanId`, `ControlledAppointmentId`) ON DELETE CASCADE;

INSERT INTO `__EFMigrationsHistory` (`MigrationId`, `ProductVersion`)
VALUES ('20230917131132_ClanForeignKeyUpdate', '9.0.11');

ALTER TABLE `MagicResources` ADD `BottomColour` varchar(100) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT '[35m';

ALTER TABLE `MagicResources` ADD `MidColour` varchar(100) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT '[1;35m';

ALTER TABLE `MagicResources` ADD `ShortName` varchar(200) CHARACTER SET utf8 COLLATE utf8_general_ci NULL;

ALTER TABLE `MagicResources` ADD `TopColour` varchar(100) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT '[0m[38;5;171m';

INSERT INTO `__EFMigrationsHistory` (`MigrationId`, `ProductVersion`)
VALUES ('20231031085439_MagicResourceColours', '9.0.11');

ALTER TABLE `Accounts` ADD `HintsEnabled` bit(1) NOT NULL DEFAULT b'1';

CREATE TABLE `NewPlayerHints` (
    `Id` bigint(20) NOT NULL AUTO_INCREMENT,
    `Text` text CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
    `FilterProgId` bigint(20) NULL,
    `Priority` int(11) NOT NULL,
    `CanRepeat` bit(1) NOT NULL,
    CONSTRAINT `PK_NewPlayerHints` PRIMARY KEY (`Id`),
    CONSTRAINT `FK_NewPlayerHints_FutureProgs` FOREIGN KEY (`FilterProgId`) REFERENCES `FutureProgs` (`Id`)
) CHARACTER SET=utf8mb4;

CREATE INDEX `IX_NewPlayerHints_FilterProgId` ON `NewPlayerHints` (`FilterProgId`);

INSERT INTO `__EFMigrationsHistory` (`MigrationId`, `ProductVersion`)
VALUES ('20231102120820_NewPlayerHints', '9.0.11');

ALTER TABLE `Races` ADD `HungerRate` double NOT NULL DEFAULT 1.0;

ALTER TABLE `Races` ADD `ThirstRate` double NOT NULL DEFAULT 1.0;

INSERT INTO `__EFMigrationsHistory` (`MigrationId`, `ProductVersion`)
VALUES ('20231110224309_HungerThirstRatesForRaces', '9.0.11');

ALTER TABLE `ClimateModels` DROP COLUMN `Definition`;

CREATE TABLE `ClimateModelSeason` (
    `ClimateModelId` bigint(20) NOT NULL,
    `SeasonId` bigint(20) NOT NULL,
    `MaximumAdditionalChangeChanceFromStableWeather` double NOT NULL,
    `IncrementalAdditionalChangeChanceFromStableWeather` double NOT NULL,
    CONSTRAINT `PRIMARY` PRIMARY KEY (`ClimateModelId`, `SeasonId`),
    CONSTRAINT `FK_ClimateModelSeasons_ClimateModels` FOREIGN KEY (`ClimateModelId`) REFERENCES `ClimateModels` (`Id`) ON DELETE CASCADE,
    CONSTRAINT `FK_ClimateModelSeasons_Seasons` FOREIGN KEY (`SeasonId`) REFERENCES `Seasons` (`Id`) ON DELETE CASCADE
) CHARACTER SET=utf8mb4;

CREATE TABLE `ClimateModelSeasonEvent` (
    `ClimateModelId` bigint(20) NOT NULL,
    `SeasonId` bigint(20) NOT NULL,
    `WeatherEventId` bigint(20) NOT NULL,
    `ChangeChance` double NOT NULL,
    `Transitions` mediumtext CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
    CONSTRAINT `PRIMARY` PRIMARY KEY (`ClimateModelId`, `SeasonId`, `WeatherEventId`),
    CONSTRAINT `FK_ClimateModelSeasonEvent_ClimateModelSeason_ClimateModelId_Se~` FOREIGN KEY (`ClimateModelId`, `SeasonId`) REFERENCES `ClimateModelSeason` (`ClimateModelId`, `SeasonId`) ON DELETE CASCADE,
    CONSTRAINT `FK_ClimateModelSeasonEvents_ClimateModels` FOREIGN KEY (`ClimateModelId`) REFERENCES `ClimateModels` (`Id`) ON DELETE CASCADE,
    CONSTRAINT `FK_ClimateModelSeasonEvents_Seasons` FOREIGN KEY (`SeasonId`) REFERENCES `Seasons` (`Id`) ON DELETE CASCADE,
    CONSTRAINT `FK_ClimateModelSeasonEvents_WeatherEvents` FOREIGN KEY (`WeatherEventId`) REFERENCES `WeatherEvents` (`Id`) ON DELETE CASCADE
) CHARACTER SET=utf8mb4;

CREATE INDEX `IX_ClimateModelSeason_SeasonId` ON `ClimateModelSeason` (`SeasonId`);

CREATE INDEX `IX_ClimateModelSeasonEvent_SeasonId` ON `ClimateModelSeasonEvent` (`SeasonId`);

CREATE INDEX `IX_ClimateModelSeasonEvent_WeatherEventId` ON `ClimateModelSeasonEvent` (`WeatherEventId`);

INSERT INTO `__EFMigrationsHistory` (`MigrationId`, `ProductVersion`)
VALUES ('20231125084220_ClimateModelSimplification', '9.0.11');

DROP PROCEDURE IF EXISTS `POMELO_BEFORE_DROP_PRIMARY_KEY`;
DELIMITER //
CREATE PROCEDURE `POMELO_BEFORE_DROP_PRIMARY_KEY`(IN `SCHEMA_NAME_ARGUMENT` VARCHAR(255), IN `TABLE_NAME_ARGUMENT` VARCHAR(255))
BEGIN
	DECLARE HAS_AUTO_INCREMENT_ID TINYINT(1);
	DECLARE PRIMARY_KEY_COLUMN_NAME VARCHAR(255);
	DECLARE PRIMARY_KEY_TYPE VARCHAR(255);
	DECLARE SQL_EXP VARCHAR(1000);
	SELECT COUNT(*)
		INTO HAS_AUTO_INCREMENT_ID
		FROM `information_schema`.`COLUMNS`
		WHERE `TABLE_SCHEMA` = (SELECT IFNULL(SCHEMA_NAME_ARGUMENT, SCHEMA()))
			AND `TABLE_NAME` = TABLE_NAME_ARGUMENT
			AND `Extra` = 'auto_increment'
			AND `COLUMN_KEY` = 'PRI'
			LIMIT 1;
	IF HAS_AUTO_INCREMENT_ID THEN
		SELECT `COLUMN_TYPE`
			INTO PRIMARY_KEY_TYPE
			FROM `information_schema`.`COLUMNS`
			WHERE `TABLE_SCHEMA` = (SELECT IFNULL(SCHEMA_NAME_ARGUMENT, SCHEMA()))
				AND `TABLE_NAME` = TABLE_NAME_ARGUMENT
				AND `COLUMN_KEY` = 'PRI'
			LIMIT 1;
		SELECT `COLUMN_NAME`
			INTO PRIMARY_KEY_COLUMN_NAME
			FROM `information_schema`.`COLUMNS`
			WHERE `TABLE_SCHEMA` = (SELECT IFNULL(SCHEMA_NAME_ARGUMENT, SCHEMA()))
				AND `TABLE_NAME` = TABLE_NAME_ARGUMENT
				AND `COLUMN_KEY` = 'PRI'
			LIMIT 1;
		SET SQL_EXP = CONCAT('ALTER TABLE `', (SELECT IFNULL(SCHEMA_NAME_ARGUMENT, SCHEMA())), '`.`', TABLE_NAME_ARGUMENT, '` MODIFY COLUMN `', PRIMARY_KEY_COLUMN_NAME, '` ', PRIMARY_KEY_TYPE, ' NOT NULL;');
		SET @SQL_EXP = SQL_EXP;
		PREPARE SQL_EXP_EXECUTE FROM @SQL_EXP;
		EXECUTE SQL_EXP_EXECUTE;
		DEALLOCATE PREPARE SQL_EXP_EXECUTE;
	END IF;
END //
DELIMITER ;

DROP PROCEDURE IF EXISTS `POMELO_AFTER_ADD_PRIMARY_KEY`;
DELIMITER //
CREATE PROCEDURE `POMELO_AFTER_ADD_PRIMARY_KEY`(IN `SCHEMA_NAME_ARGUMENT` VARCHAR(255), IN `TABLE_NAME_ARGUMENT` VARCHAR(255), IN `COLUMN_NAME_ARGUMENT` VARCHAR(255))
BEGIN
	DECLARE HAS_AUTO_INCREMENT_ID INT(11);
	DECLARE PRIMARY_KEY_COLUMN_NAME VARCHAR(255);
	DECLARE PRIMARY_KEY_TYPE VARCHAR(255);
	DECLARE SQL_EXP VARCHAR(1000);
	SELECT COUNT(*)
		INTO HAS_AUTO_INCREMENT_ID
		FROM `information_schema`.`COLUMNS`
		WHERE `TABLE_SCHEMA` = (SELECT IFNULL(SCHEMA_NAME_ARGUMENT, SCHEMA()))
			AND `TABLE_NAME` = TABLE_NAME_ARGUMENT
			AND `COLUMN_NAME` = COLUMN_NAME_ARGUMENT
			AND `COLUMN_TYPE` LIKE '%int%'
			AND `COLUMN_KEY` = 'PRI';
	IF HAS_AUTO_INCREMENT_ID THEN
		SELECT `COLUMN_TYPE`
			INTO PRIMARY_KEY_TYPE
			FROM `information_schema`.`COLUMNS`
			WHERE `TABLE_SCHEMA` = (SELECT IFNULL(SCHEMA_NAME_ARGUMENT, SCHEMA()))
				AND `TABLE_NAME` = TABLE_NAME_ARGUMENT
				AND `COLUMN_NAME` = COLUMN_NAME_ARGUMENT
				AND `COLUMN_TYPE` LIKE '%int%'
				AND `COLUMN_KEY` = 'PRI';
		SELECT `COLUMN_NAME`
			INTO PRIMARY_KEY_COLUMN_NAME
			FROM `information_schema`.`COLUMNS`
			WHERE `TABLE_SCHEMA` = (SELECT IFNULL(SCHEMA_NAME_ARGUMENT, SCHEMA()))
				AND `TABLE_NAME` = TABLE_NAME_ARGUMENT
				AND `COLUMN_NAME` = COLUMN_NAME_ARGUMENT
				AND `COLUMN_TYPE` LIKE '%int%'
				AND `COLUMN_KEY` = 'PRI';
		SET SQL_EXP = CONCAT('ALTER TABLE `', (SELECT IFNULL(SCHEMA_NAME_ARGUMENT, SCHEMA())), '`.`', TABLE_NAME_ARGUMENT, '` MODIFY COLUMN `', PRIMARY_KEY_COLUMN_NAME, '` ', PRIMARY_KEY_TYPE, ' NOT NULL AUTO_INCREMENT;');
		SET @SQL_EXP = SQL_EXP;
		PREPARE SQL_EXP_EXECUTE FROM @SQL_EXP;
		EXECUTE SQL_EXP_EXECUTE;
		DEALLOCATE PREPARE SQL_EXP_EXECUTE;
	END IF;
END //
DELIMITER ;

ALTER TABLE `Cultures_ChargenResources` DROP FOREIGN KEY `FK_Cultures_ChargenResources_ChargenResources`;

ALTER TABLE `Cultures_ChargenResources` DROP FOREIGN KEY `FK_Cultures_ChargenResources_Races`;

ALTER TABLE `Ethnicities_ChargenResources` DROP FOREIGN KEY `FK_Ethnicities_ChargenResources_ChargenResources`;

ALTER TABLE `Ethnicities_ChargenResources` DROP FOREIGN KEY `FK_Ethnicities_ChargenResources_Ethnicities`;

ALTER TABLE `Races_ChargenResources` DROP FOREIGN KEY `FK_Races_ChargenResources_ChargenResources`;

ALTER TABLE `Races_ChargenResources` DROP FOREIGN KEY `FK_Races_ChargenResources_Races`;

CALL POMELO_BEFORE_DROP_PRIMARY_KEY(NULL, 'Races_ChargenResources');
ALTER TABLE `Races_ChargenResources` DROP PRIMARY KEY;

CALL POMELO_BEFORE_DROP_PRIMARY_KEY(NULL, 'Ethnicities_ChargenResources');
ALTER TABLE `Ethnicities_ChargenResources` DROP PRIMARY KEY;

CALL POMELO_BEFORE_DROP_PRIMARY_KEY(NULL, 'Cultures_ChargenResources');
ALTER TABLE `Cultures_ChargenResources` DROP PRIMARY KEY;

ALTER TABLE `Ethnicities_ChargenResources` RENAME INDEX `FK_Ethnicities_ChargenResources_ChargenResources` TO `IX_Ethnicities_ChargenResources_ChargenResourceId`;

ALTER TABLE `Cultures_ChargenResources` RENAME INDEX `FK_Cultures_ChargenResources_ChargenResources` TO `IX_Cultures_ChargenResources_ChargenResourceId`;

ALTER TABLE `Races_ChargenResources` ADD CONSTRAINT `PRIMARY` PRIMARY KEY (`RaceId`, `ChargenResourceId`);

ALTER TABLE `Ethnicities_ChargenResources` ADD CONSTRAINT `PRIMARY` PRIMARY KEY (`EthnicityId`, `ChargenResourceId`);

ALTER TABLE `Cultures_ChargenResources` ADD CONSTRAINT `PRIMARY` PRIMARY KEY (`CultureId`, `ChargenResourceId`);

ALTER TABLE `Cultures_ChargenResources` ADD CONSTRAINT `FK_Cultures_ChargenResources_ChargenResources_ChargenResourceId` FOREIGN KEY (`ChargenResourceId`) REFERENCES `ChargenResources` (`Id`) ON DELETE CASCADE;

ALTER TABLE `Cultures_ChargenResources` ADD CONSTRAINT `FK_Cultures_ChargenResources_Cultures_CultureId` FOREIGN KEY (`CultureId`) REFERENCES `Cultures` (`Id`) ON DELETE CASCADE;

ALTER TABLE `Ethnicities_ChargenResources` ADD CONSTRAINT `FK_Ethnicities_ChargenResources_ChargenResources_ChargenResourc~` FOREIGN KEY (`ChargenResourceId`) REFERENCES `ChargenResources` (`Id`) ON DELETE CASCADE;

ALTER TABLE `Ethnicities_ChargenResources` ADD CONSTRAINT `FK_Ethnicities_ChargenResources_Ethnicities_EthnicityId` FOREIGN KEY (`EthnicityId`) REFERENCES `Ethnicities` (`Id`) ON DELETE CASCADE;

ALTER TABLE `Races_ChargenResources` ADD CONSTRAINT `FK_Races_ChargenResources_ChargenResources_ChargenResourceId` FOREIGN KEY (`ChargenResourceId`) REFERENCES `ChargenResources` (`Id`) ON DELETE CASCADE;

ALTER TABLE `Races_ChargenResources` ADD CONSTRAINT `FK_Races_ChargenResources_Races_RaceId` FOREIGN KEY (`RaceId`) REFERENCES `Races` (`Id`) ON DELETE CASCADE;

INSERT INTO `__EFMigrationsHistory` (`MigrationId`, `ProductVersion`)
VALUES ('20231208235024_HeritageChargenCostBugFix', '9.0.11');

DROP PROCEDURE `POMELO_BEFORE_DROP_PRIMARY_KEY`;

DROP PROCEDURE `POMELO_AFTER_ADD_PRIMARY_KEY`;

ALTER TABLE `ChargenResources` ADD `ControlProgId` bigint(20) NULL;

ALTER TABLE `ChargenResources` ADD `FK_ChargenResources_FutureProgs` bigint(20) NULL;

ALTER TABLE `Accounts_ChargenResources` MODIFY COLUMN `Amount` double NOT NULL;

CREATE INDEX `IX_ChargenResources_FK_ChargenResources_FutureProgs` ON `ChargenResources` (`FK_ChargenResources_FutureProgs`);

ALTER TABLE `ChargenResources` ADD CONSTRAINT `FK_ChargenResources_FutureProgs_FK_ChargenResources_FutureProgs` FOREIGN KEY (`FK_ChargenResources_FutureProgs`) REFERENCES `FutureProgs` (`Id`) ON DELETE SET NULL;

INSERT INTO `__EFMigrationsHistory` (`MigrationId`, `ProductVersion`)
VALUES ('20240112055830_ChargenResourcesControlProg', '9.0.11');

ALTER TABLE `Coins` ADD `UseForChange` bit(1) NOT NULL DEFAULT 1;

INSERT INTO `__EFMigrationsHistory` (`MigrationId`, `ProductVersion`)
VALUES ('20240119120217_CoinsChangeFlag', '9.0.11');

ALTER TABLE `CurrencyDescriptionPatternElements` DROP FOREIGN KEY `FK_CDPE_CurrencyDivisions`;

ALTER TABLE `CurrencyDescriptionPatternElements` ADD CONSTRAINT `FK_CDPE_CurrencyDivisions` FOREIGN KEY (`CurrencyDivisionId`) REFERENCES `CurrencyDivisions` (`Id`) ON DELETE CASCADE;

INSERT INTO `__EFMigrationsHistory` (`MigrationId`, `ProductVersion`)
VALUES ('20240129025113_CurrencyForeignKeyUpdateJan24', '9.0.11');

ALTER TABLE `Shops` ADD `MinimumFloatToBuyItems` decimal(65,30) NOT NULL DEFAULT 0.0;

ALTER TABLE `Merchandises` ADD `BaseBuyModifier` decimal(58,29) NOT NULL DEFAULT 0.3;

ALTER TABLE `Merchandises` ADD `MaximumStockLevelsToBuy` int NOT NULL DEFAULT 0;

ALTER TABLE `Merchandises` ADD `MinimumConditionToBuy` double NOT NULL DEFAULT 0.94999999999999996;

ALTER TABLE `Merchandises` ADD `WillBuy` bit(1) NOT NULL DEFAULT 0;

ALTER TABLE `Merchandises` ADD `WillSell` bit(1) NOT NULL DEFAULT 1;

INSERT INTO `__EFMigrationsHistory` (`MigrationId`, `ProductVersion`)
VALUES ('20240305110906_BuyingMerchandise', '9.0.11');

CREATE TABLE `MarketCategories` (
    `Id` bigint NOT NULL AUTO_INCREMENT,
    `Name` longtext CHARACTER SET utf8mb4 NULL,
    `Description` longtext CHARACTER SET utf8mb4 NULL,
    `ElasticityFactorAbove` double NOT NULL,
    `ElasticityFactorBelow` double NOT NULL,
    `Tags` longtext CHARACTER SET utf8mb4 NULL,
    CONSTRAINT `PK_MarketCategories` PRIMARY KEY (`Id`)
) CHARACTER SET=utf8mb4;

CREATE TABLE `MarketInfluenceTemplates` (
    `Id` bigint NOT NULL AUTO_INCREMENT,
    `Name` longtext CHARACTER SET utf8mb4 NULL,
    `Description` longtext CHARACTER SET utf8mb4 NULL,
    `CharacterKnowsAboutInfluenceProgId` bigint(20) NOT NULL,
    `Impacts` longtext CHARACTER SET utf8mb4 NULL,
    `TemplateSummary` longtext CHARACTER SET utf8mb4 NULL,
    CONSTRAINT `PK_MarketInfluenceTemplates` PRIMARY KEY (`Id`),
    CONSTRAINT `FK_MarketInfluenceTemplates_FutureProgs_CharacterKnowsAboutInfl~` FOREIGN KEY (`CharacterKnowsAboutInfluenceProgId`) REFERENCES `FutureProgs` (`Id`) ON DELETE CASCADE
) CHARACTER SET=utf8mb4;

CREATE TABLE `Markets` (
    `Id` bigint NOT NULL AUTO_INCREMENT,
    `Name` longtext CHARACTER SET utf8mb4 NULL,
    `Description` longtext CHARACTER SET utf8mb4 NULL,
    `EconomicZoneId` bigint(20) NOT NULL,
    `MarketPriceFormula` longtext CHARACTER SET utf8mb4 NULL,
    CONSTRAINT `PK_Markets` PRIMARY KEY (`Id`),
    CONSTRAINT `FK_Markets_EconomicZones_EconomicZoneId` FOREIGN KEY (`EconomicZoneId`) REFERENCES `EconomicZones` (`Id`) ON DELETE CASCADE
) CHARACTER SET=utf8mb4;

CREATE TABLE `MarketInfluences` (
    `Id` bigint NOT NULL AUTO_INCREMENT,
    `Name` longtext CHARACTER SET utf8mb4 NULL,
    `Description` longtext CHARACTER SET utf8mb4 NULL,
    `MarketId` bigint NOT NULL,
    `AppliesFrom` longtext CHARACTER SET utf8mb4 NULL,
    `AppliesUntil` longtext CHARACTER SET utf8mb4 NULL,
    `CharacterKnowsAboutInfluenceProgId` bigint(20) NOT NULL,
    `MarketInfluenceTemplateId` bigint NULL,
    `Impacts` longtext CHARACTER SET utf8mb4 NULL,
    CONSTRAINT `PK_MarketInfluences` PRIMARY KEY (`Id`),
    CONSTRAINT `FK_MarketInfluences_FutureProgs_CharacterKnowsAboutInfluencePro~` FOREIGN KEY (`CharacterKnowsAboutInfluenceProgId`) REFERENCES `FutureProgs` (`Id`) ON DELETE CASCADE,
    CONSTRAINT `FK_MarketInfluences_MarketInfluenceTemplates_MarketInfluenceTem~` FOREIGN KEY (`MarketInfluenceTemplateId`) REFERENCES `MarketInfluenceTemplates` (`Id`),
    CONSTRAINT `FK_MarketInfluences_Markets_MarketId` FOREIGN KEY (`MarketId`) REFERENCES `Markets` (`Id`) ON DELETE CASCADE
) CHARACTER SET=utf8mb4;

CREATE TABLE `MarketMarketCategory` (
    `MarketCategoriesId` bigint NOT NULL,
    `MarketsId` bigint NOT NULL,
    CONSTRAINT `PK_MarketMarketCategory` PRIMARY KEY (`MarketCategoriesId`, `MarketsId`),
    CONSTRAINT `FK_MarketMarketCategory_MarketCategories_MarketCategoriesId` FOREIGN KEY (`MarketCategoriesId`) REFERENCES `MarketCategories` (`Id`) ON DELETE CASCADE,
    CONSTRAINT `FK_MarketMarketCategory_Markets_MarketsId` FOREIGN KEY (`MarketsId`) REFERENCES `Markets` (`Id`) ON DELETE CASCADE
) CHARACTER SET=utf8mb4;

CREATE INDEX `IX_MarketInfluences_CharacterKnowsAboutInfluenceProgId` ON `MarketInfluences` (`CharacterKnowsAboutInfluenceProgId`);

CREATE INDEX `IX_MarketInfluences_MarketId` ON `MarketInfluences` (`MarketId`);

CREATE INDEX `IX_MarketInfluences_MarketInfluenceTemplateId` ON `MarketInfluences` (`MarketInfluenceTemplateId`);

CREATE INDEX `IX_MarketInfluenceTemplates_CharacterKnowsAboutInfluenceProgId` ON `MarketInfluenceTemplates` (`CharacterKnowsAboutInfluenceProgId`);

CREATE INDEX `IX_MarketMarketCategory_MarketsId` ON `MarketMarketCategory` (`MarketsId`);

CREATE INDEX `IX_Markets_EconomicZoneId` ON `Markets` (`EconomicZoneId`);

INSERT INTO `__EFMigrationsHistory` (`MigrationId`, `ProductVersion`)
VALUES ('20240325104238_MarketsV1', '9.0.11');

ALTER TABLE `Merchandises` ADD `IgnoreMarketPricing` tinyint(1) NOT NULL DEFAULT FALSE;

ALTER TABLE `Shops` ADD `MarketId` bigint NULL;

CREATE INDEX `IX_Shops_MarketId` ON `Shops` (`MarketId`);

ALTER TABLE `Shops` ADD CONSTRAINT `FK_Shops_Markets_MarketId` FOREIGN KEY (`MarketId`) REFERENCES `Markets` (`Id`);

INSERT INTO `__EFMigrationsHistory` (`MigrationId`, `ProductVersion`)
VALUES ('20240418112441_MarketsShopIntegration', '9.0.11');

CREATE TABLE `MarketPopulations` (
    `Id` bigint NOT NULL AUTO_INCREMENT,
    `Name` longtext CHARACTER SET utf8mb4 NULL,
    `Description` longtext CHARACTER SET utf8mb4 NULL,
    `PopulationScale` int NOT NULL,
    `MarketId` bigint NOT NULL,
    `MarketPopulationNeeds` longtext CHARACTER SET utf8mb4 NULL,
    `MarketStressPoints` longtext CHARACTER SET utf8mb4 NULL,
    CONSTRAINT `PK_MarketPopulations` PRIMARY KEY (`Id`),
    CONSTRAINT `FK_MarketPopulations_Markets_MarketId` FOREIGN KEY (`MarketId`) REFERENCES `Markets` (`Id`) ON DELETE CASCADE
) CHARACTER SET=utf8mb4;

CREATE INDEX `IX_MarketPopulations_MarketId` ON `MarketPopulations` (`MarketId`);

INSERT INTO `__EFMigrationsHistory` (`MigrationId`, `ProductVersion`)
VALUES ('20240427013621_MarketPopulations', '9.0.11');

ALTER TABLE `Channels` ADD `DiscordChannelId` bigint unsigned NULL;

INSERT INTO `__EFMigrationsHistory` (`MigrationId`, `ProductVersion`)
VALUES ('20240601141550_DiscordOutputForChannels', '9.0.11');

ALTER TABLE `Shops` ADD `AutopayTaxes` bit(1) NOT NULL DEFAULT 1;

INSERT INTO `__EFMigrationsHistory` (`MigrationId`, `ProductVersion`)
VALUES ('20240615065145_ShopAutopayTaxes', '9.0.11');

ALTER TABLE `Terrains` ADD `CanHaveTracks` bit(1) NOT NULL DEFAULT 1;

ALTER TABLE `Terrains` ADD `TrackIntensityMultiplierOlfactory` double NOT NULL DEFAULT 1.0;

ALTER TABLE `Terrains` ADD `TrackIntensityMultiplierVisual` double NOT NULL DEFAULT 1.0;

ALTER TABLE `Races` ADD `TrackIntensityOlfactory` double NOT NULL DEFAULT 1.0;

ALTER TABLE `Races` ADD `TrackIntensityVisual` double NOT NULL DEFAULT 1.0;

ALTER TABLE `Races` ADD `TrackingAbilityOlfactory` double NOT NULL DEFAULT 0.0;

ALTER TABLE `Races` ADD `TrackingAbilityVisual` double NOT NULL DEFAULT 1.0;

CREATE TABLE `Tracks` (
    `Id` bigint(20) NOT NULL AUTO_INCREMENT,
    `CharacterId` bigint(20) NOT NULL,
    `BodyPrototypeId` bigint(20) NOT NULL,
    `CellId` bigint(20) NOT NULL,
    `RoomLayer` int(11) NOT NULL,
    `MudDateTime` varchar(100) CHARACTER SET utf8 COLLATE utf8_general_ci NULL,
    `FromDirectionExitId` bigint(20) NULL,
    `ToDirectionExitId` bigint(20) NULL,
    `TrackCircumstances` int(11) NOT NULL,
    `FromMoveSpeedId` bigint(20) NULL,
    `ToMoveSpeedId` bigint(20) NULL,
    `ExertionLevel` int(11) NOT NULL,
    `TrackIntensityVisual` double NOT NULL,
    `TrackIntensityOlfactory` double NOT NULL,
    `TurnedAround` bit(1) NOT NULL,
    CONSTRAINT `PRIMARY` PRIMARY KEY (`Id`),
    CONSTRAINT `FK_Tracks_BodyProtos` FOREIGN KEY (`BodyPrototypeId`) REFERENCES `BodyProtos` (`Id`) ON DELETE CASCADE,
    CONSTRAINT `FK_Tracks_Cells` FOREIGN KEY (`CellId`) REFERENCES `Cells` (`Id`) ON DELETE CASCADE,
    CONSTRAINT `FK_Tracks_Characters` FOREIGN KEY (`CharacterId`) REFERENCES `Characters` (`Id`) ON DELETE CASCADE,
    CONSTRAINT `FK_Tracks_Exits_From` FOREIGN KEY (`FromDirectionExitId`) REFERENCES `Exits` (`Id`) ON DELETE CASCADE,
    CONSTRAINT `FK_Tracks_Exits_To` FOREIGN KEY (`ToDirectionExitId`) REFERENCES `Exits` (`Id`) ON DELETE CASCADE,
    CONSTRAINT `FK_Tracks_MoveSpeeds_From` FOREIGN KEY (`FromMoveSpeedId`) REFERENCES `MoveSpeeds` (`Id`) ON DELETE CASCADE,
    CONSTRAINT `FK_Tracks_MoveSpeeds_To` FOREIGN KEY (`ToMoveSpeedId`) REFERENCES `MoveSpeeds` (`Id`) ON DELETE CASCADE
) CHARACTER SET=utf8mb4;

CREATE INDEX `IX_Tracks_BodyPrototypeId` ON `Tracks` (`BodyPrototypeId`);

CREATE INDEX `IX_Tracks_CellId` ON `Tracks` (`CellId`);

CREATE INDEX `IX_Tracks_CharacterId` ON `Tracks` (`CharacterId`);

CREATE INDEX `IX_Tracks_FromDirectionExitId` ON `Tracks` (`FromDirectionExitId`);

CREATE INDEX `IX_Tracks_FromMoveSpeedId` ON `Tracks` (`FromMoveSpeedId`);

CREATE INDEX `IX_Tracks_ToDirectionExitId` ON `Tracks` (`ToDirectionExitId`);

CREATE INDEX `IX_Tracks_ToMoveSpeedId` ON `Tracks` (`ToMoveSpeedId`);

INSERT INTO `__EFMigrationsHistory` (`MigrationId`, `ProductVersion`)
VALUES ('20240730123726_TrackingV1', '9.0.11');

ALTER TABLE `CharacterCombatSettings` MODIFY COLUMN `PreferToFightArmed` bit(1) NOT NULL;

ALTER TABLE `CharacterCombatSettings` MODIFY COLUMN `PreferShieldUse` bit(1) NOT NULL;

ALTER TABLE `CharacterCombatSettings` MODIFY COLUMN `PreferNonContactClinchBreaking` bit(1) NOT NULL;

ALTER TABLE `CharacterCombatSettings` MODIFY COLUMN `MoveToMeleeIfCannotEngageInRangedCombat` bit(1) NOT NULL;

ALTER TABLE `CellOverlays` MODIFY COLUMN `SafeQuit` bit(1) NOT NULL;

ALTER TABLE `BodypartProto` MODIFY COLUMN `IsCore` bit(1) NOT NULL;

INSERT INTO `__EFMigrationsHistory` (`MigrationId`, `ProductVersion`)
VALUES ('20240804070126_FixDatabaseAutoTrueBug', '9.0.11');

ALTER TABLE `BodyProtos` MODIFY COLUMN `WielderDescriptionSingle` varchar(100) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL DEFAULT 'hand';

ALTER TABLE `BodyProtos` MODIFY COLUMN `WielderDescriptionPlural` varchar(100) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL DEFAULT 'hands';

ALTER TABLE `BodyProtos` MODIFY COLUMN `Name` varchar(100) CHARACTER SET utf8 COLLATE utf8_general_ci NULL;

ALTER TABLE `BodyProtos` MODIFY COLUMN `LegDescriptionSingular` varchar(100) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL DEFAULT 'leg';

ALTER TABLE `BodyProtos` MODIFY COLUMN `LegDescriptionPlural` varchar(100) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL DEFAULT 'legs';

ALTER TABLE `BodyProtos` ADD `NameForTracking` varchar(100) CHARACTER SET utf8 COLLATE utf8_general_ci NULL;

INSERT INTO `__EFMigrationsHistory` (`MigrationId`, `ProductVersion`)
VALUES ('20240808232211_TrackingNameForBodyProtos', '9.0.11');

INSERT INTO `__EFMigrationsHistory` (`MigrationId`, `ProductVersion`)
VALUES ('20240809155707_BMIUnits', '9.0.11');

ALTER TABLE `ArmourTypes` MODIFY COLUMN `StackedDifficultyDegrees` double NOT NULL;

ALTER TABLE `ArmourTypes` MODIFY COLUMN `BaseDifficultyDegrees` double NOT NULL;

INSERT INTO `__EFMigrationsHistory` (`MigrationId`, `ProductVersion`)
VALUES ('20240816134208_ArmourPenaltyToDouble', '9.0.11');

ALTER TABLE `HeightWeightModels` ADD `MeanWeight` double NULL;

ALTER TABLE `HeightWeightModels` ADD `StddevWeight` double NULL;

INSERT INTO `__EFMigrationsHistory` (`MigrationId`, `ProductVersion`)
VALUES ('20240817112644_HeightWeightModelDirectSetWeights', '9.0.11');

ALTER TABLE `Accounts` ADD `AutoReacquireTargets` bit(1) NOT NULL DEFAULT b'1';

INSERT INTO `__EFMigrationsHistory` (`MigrationId`, `ProductVersion`)
VALUES ('20240828105208_AutoReacquireTargetsSetting', '9.0.11');

ALTER TABLE `CharacterCombatSettings` RENAME COLUMN `AttackUnarmedOrHelpless` TO `AttackUnarmed`;

ALTER TABLE `CharacterCombatSettings` ADD `AttackHelpless` bit(1) NOT NULL DEFAULT b'0';

INSERT INTO `__EFMigrationsHistory` (`MigrationId`, `ProductVersion`)
VALUES ('20240828124859_CombatSettingsAugust24', '9.0.11');

ALTER TABLE `BodypartGroupDescribers` ADD `BodyProtoId` bigint(20) NULL;

CREATE INDEX `IX_BodypartGroupDescribers_BodyProtoId` ON `BodypartGroupDescribers` (`BodyProtoId`);

ALTER TABLE `BodypartGroupDescribers` ADD CONSTRAINT `FK_BodypartGroupDescribers_BodyProtos_BodyProtoId` FOREIGN KEY (`BodyProtoId`) REFERENCES `BodyProtos` (`Id`);

INSERT INTO `__EFMigrationsHistory` (`MigrationId`, `ProductVersion`)
VALUES ('20240831005804_BodypartGroupDescribersAugust2024', '9.0.11');

ALTER TABLE `Crimes` MODIFY COLUMN `HasBeenEnforced` bit(1) NOT NULL DEFAULT 0;

ALTER TABLE `Crimes` MODIFY COLUMN `BailHasBeenPosted` bit(1) NOT NULL DEFAULT 0;

ALTER TABLE `Crimes` ADD `ExecutionPunishment` bit(1) NOT NULL DEFAULT 0;

ALTER TABLE `Crimes` ADD `FineHasBeenPaid` bit(1) NOT NULL DEFAULT 0;

ALTER TABLE `Crimes` ADD `GoodBehaviourBond` double NOT NULL DEFAULT 0.0;

ALTER TABLE `Crimes` ADD `SentenceHasBeenServed` bit(1) NOT NULL DEFAULT 0;

INSERT INTO `__EFMigrationsHistory` (`MigrationId`, `ProductVersion`)
VALUES ('20240925062238_CrimesUpdate2024Sep25', '9.0.11');

ALTER TABLE `Merchandises` ADD `PermitItemDecayOnStockedItems` tinyint(1) NOT NULL DEFAULT FALSE;

INSERT INTO `__EFMigrationsHistory` (`MigrationId`, `ProductVersion`)
VALUES ('20241011073405_StockroomNonMorphing', '9.0.11');

ALTER TABLE `GameItemProtos` ADD `IsHiddenFromPlayers` tinyint(1) NOT NULL DEFAULT FALSE;

INSERT INTO `__EFMigrationsHistory` (`MigrationId`, `ProductVersion`)
VALUES ('20241016054103_ItemProtoIsHiddenFromPlayers', '9.0.11');

ALTER TABLE `Bodies` ADD `HealthStrategyId` bigint(20) NULL;

CREATE INDEX `IX_Bodies_HealthStrategyId` ON `Bodies` (`HealthStrategyId`);

ALTER TABLE `Bodies` ADD CONSTRAINT `FK_Bodies_HealthStrategies_HealthStrategyId` FOREIGN KEY (`HealthStrategyId`) REFERENCES `HealthStrategies` (`Id`);

INSERT INTO `__EFMigrationsHistory` (`MigrationId`, `ProductVersion`)
VALUES ('20241016123415_BodyOverrideHealthStrategy', '9.0.11');

ALTER TABLE `Liquids` ADD `LeaveResidueInRooms` tinyint(1) NOT NULL DEFAULT FALSE;

INSERT INTO `__EFMigrationsHistory` (`MigrationId`, `ProductVersion`)
VALUES ('20241018071518_LiquidLeaveResidueInRooms', '9.0.11');

CREATE TABLE `Races_RemoveBreathableGases` (
    `RaceId` bigint(20) NOT NULL,
    `GasId` bigint(20) NOT NULL,
    CONSTRAINT `PRIMARY` PRIMARY KEY (`RaceId`, `GasId`),
    CONSTRAINT `FK_Races_RemoveBreathableGases_Gases` FOREIGN KEY (`GasId`) REFERENCES `Gases` (`Id`) ON DELETE CASCADE,
    CONSTRAINT `FK_Races_RemoveBreathableGases_Races` FOREIGN KEY (`RaceId`) REFERENCES `Races` (`Id`) ON DELETE CASCADE
) CHARACTER SET=utf8mb4;

CREATE TABLE `Races_RemoveBreathableLiquids` (
    `RaceId` bigint(20) NOT NULL,
    `LiquidId` bigint(20) NOT NULL,
    CONSTRAINT `PRIMARY` PRIMARY KEY (`RaceId`, `LiquidId`),
    CONSTRAINT `FK_Races_RemoveBreathableLiquids_Liquids` FOREIGN KEY (`LiquidId`) REFERENCES `Liquids` (`Id`) ON DELETE CASCADE,
    CONSTRAINT `FK_Races_RemoveBreathableLiquids_Races` FOREIGN KEY (`RaceId`) REFERENCES `Races` (`Id`) ON DELETE CASCADE
) CHARACTER SET=utf8mb4;

CREATE INDEX `FK_Races_RemoveBreathableGases_Gases_idx` ON `Races_RemoveBreathableGases` (`GasId`);

CREATE INDEX `FK_Races_RemoveBreathableLiquids_Liquids_idx` ON `Races_RemoveBreathableLiquids` (`LiquidId`);

INSERT INTO `__EFMigrationsHistory` (`MigrationId`, `ProductVersion`)
VALUES ('20241121010653_RemovingBreathableFluidsRaces', '9.0.11');

ALTER TABLE `Crimes` MODIFY COLUMN `CriminalShortDescription` varchar(500) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL;

ALTER TABLE `Crimes` MODIFY COLUMN `CriminalFullDescription` text CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL;

ALTER TABLE `Crimes` MODIFY COLUMN `CriminalCharacteristics` text CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL;

INSERT INTO `__EFMigrationsHistory` (`MigrationId`, `ProductVersion`)
VALUES ('20241129002416_CriminalDescUpdates', '9.0.11');

ALTER TABLE `RandomNameProfiles_Elements` MODIFY COLUMN `Name` varchar(100) CHARACTER SET utf8 COLLATE utf8_bin NOT NULL;

INSERT INTO `__EFMigrationsHistory` (`MigrationId`, `ProductVersion`)
VALUES ('20241216062012_RandomNamesBinaryUnicodeSort', '9.0.11');

CREATE TABLE `EthnicitiesNameCultures` (
    `EthnicityId` bigint(20) NOT NULL,
    `NameCultureId` bigint(20) NOT NULL,
    `Gender` smallint(6) NOT NULL,
    CONSTRAINT `PRIMARY` PRIMARY KEY (`EthnicityId`, `NameCultureId`, `Gender`),
    CONSTRAINT `FK_EthnicitiesNameCultures_Ethnicities` FOREIGN KEY (`EthnicityId`) REFERENCES `Ethnicities` (`Id`) ON DELETE CASCADE,
    CONSTRAINT `FK_EthnicitiesNameCultures_NameCultures` FOREIGN KEY (`NameCultureId`) REFERENCES `NameCulture` (`Id`) ON DELETE CASCADE
) CHARACTER SET=utf8mb4;

CREATE INDEX `IX_EthnicitiesNameCultures_NameCultureId` ON `EthnicitiesNameCultures` (`NameCultureId`);

INSERT INTO `__EFMigrationsHistory` (`MigrationId`, `ProductVersion`)
VALUES ('20241220091815_EthnicitiesNameCultures', '9.0.11');

ALTER TABLE `Terrains` ADD `TagInformation` longtext CHARACTER SET utf8mb4 NULL;

INSERT INTO `__EFMigrationsHistory` (`MigrationId`, `ProductVersion`)
VALUES ('20241231030836_TagsForTerrains', '9.0.11');

ALTER TABLE `HeightWeightModels` ADD `SkewnessBMI` double NULL;

ALTER TABLE `HeightWeightModels` ADD `SkewnessHeight` double NULL;

ALTER TABLE `HeightWeightModels` ADD `SkewnessWeight` double NULL;

INSERT INTO `__EFMigrationsHistory` (`MigrationId`, `ProductVersion`)
VALUES ('20250101232454_SkewnessForHWModels', '9.0.11');

CREATE TABLE `Shoppers` (
    `Id` bigint NOT NULL AUTO_INCREMENT,
    `Name` longtext CHARACTER SET utf8mb4 NULL,
    `EconomicZoneId` bigint(20) NOT NULL,
    `Interval` longtext CHARACTER SET utf8mb4 NULL,
    `NextDate` longtext CHARACTER SET utf8mb4 NULL,
    `Type` longtext CHARACTER SET utf8mb4 NULL,
    `Definition` longtext CHARACTER SET utf8mb4 NULL,
    CONSTRAINT `PK_Shoppers` PRIMARY KEY (`Id`),
    CONSTRAINT `FK_Shoppers_EconomicZones_EconomicZoneId` FOREIGN KEY (`EconomicZoneId`) REFERENCES `EconomicZones` (`Id`) ON DELETE CASCADE
) CHARACTER SET=utf8mb4;

CREATE TABLE `ShopperLogs` (
    `Id` bigint NOT NULL AUTO_INCREMENT,
    `ShopperId` bigint NOT NULL,
    `DateTime` datetime(6) NOT NULL,
    `MudDateTime` longtext CHARACTER SET utf8mb4 NULL,
    `LogType` longtext CHARACTER SET utf8mb4 NULL,
    `LogEntry` longtext CHARACTER SET utf8mb4 NULL,
    CONSTRAINT `PK_ShopperLogs` PRIMARY KEY (`Id`),
    CONSTRAINT `FK_ShopperLogs_Shoppers_ShopperId` FOREIGN KEY (`ShopperId`) REFERENCES `Shoppers` (`Id`) ON DELETE CASCADE
) CHARACTER SET=utf8mb4;

CREATE INDEX `IX_ShopperLogs_ShopperId` ON `ShopperLogs` (`ShopperId`);

CREATE INDEX `IX_Shoppers_EconomicZoneId` ON `Shoppers` (`EconomicZoneId`);

INSERT INTO `__EFMigrationsHistory` (`MigrationId`, `ProductVersion`)
VALUES ('20250210095915_Shoppers', '9.0.11');

ALTER TABLE `ShopTransactionRecords` ADD `MerchandiseId` bigint(20) NULL;

ALTER TABLE `Merchandises` ADD `SalesMarkupMultiplier` decimal(65,30) NULL;

CREATE INDEX `IX_ShopTransactionRecords_MerchandiseId` ON `ShopTransactionRecords` (`MerchandiseId`);

ALTER TABLE `ShopTransactionRecords` ADD CONSTRAINT `FK_ShopTransactionRecords_Merchandises_MerchandiseId` FOREIGN KEY (`MerchandiseId`) REFERENCES `Merchandises` (`Id`);

INSERT INTO `__EFMigrationsHistory` (`MigrationId`, `ProductVersion`)
VALUES ('20250210233555_ShopsFeb25', '9.0.11');

ALTER TABLE `Shops` ADD `ExpectedCashBalance` decimal(65,30) NOT NULL DEFAULT 0.0;

INSERT INTO `__EFMigrationsHistory` (`MigrationId`, `ProductVersion`)
VALUES ('20250211100238_ShopsFeb25P2', '9.0.11');

ALTER TABLE `GameItemProtos` ADD `PreserveRegisterVariables` tinyint(1) NOT NULL DEFAULT FALSE;

INSERT INTO `__EFMigrationsHistory` (`MigrationId`, `ProductVersion`)
VALUES ('20250304042559_PreserveRegisterVariablesItemFlag', '9.0.11');

ALTER TABLE `CraftPhases` ADD `ExertionLevel` int NOT NULL DEFAULT 0;

ALTER TABLE `CraftPhases` ADD `StaminaUsage` double NOT NULL DEFAULT 0.0;

INSERT INTO `__EFMigrationsHistory` (`MigrationId`, `ProductVersion`)
VALUES ('20250304104024_CraftPhaseExertionAndStamina', '9.0.11');

ALTER TABLE `ShopTransactionRecords` DROP FOREIGN KEY `FK_ShopTransactionRecords_Merchandises_MerchandiseId`;

ALTER TABLE `ShopTransactionRecords` ADD CONSTRAINT `FK_ShopTransactionRecords_Merchandises` FOREIGN KEY (`MerchandiseId`) REFERENCES `Merchandises` (`Id`) ON DELETE SET NULL;

INSERT INTO `__EFMigrationsHistory` (`MigrationId`, `ProductVersion`)
VALUES ('20250304114440_MerchandiseTransactionRecordsFix', '9.0.11');

ALTER TABLE `MagicSpells` ADD `TargetNullEmote` longtext CHARACTER SET utf8mb4 NULL;

INSERT INTO `__EFMigrationsHistory` (`MigrationId`, `ProductVersion`)
VALUES ('20250424052852_SpellTriggerNullTargets', '9.0.11');

ALTER TABLE `PropertyLeaseOrders` ADD `RekeyOnLeaseEnd` tinyint(1) NOT NULL DEFAULT FALSE;

INSERT INTO `__EFMigrationsHistory` (`MigrationId`, `ProductVersion`)
VALUES ('20250628230040_PropertyRekeyOnLeaseEnd', '9.0.11');

CREATE TABLE `Arenas` (
    `Id` bigint(20) NOT NULL AUTO_INCREMENT,
    `Name` varchar(200) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
    `EconomicZoneId` bigint(20) NOT NULL,
    `CurrencyId` bigint(20) NOT NULL,
    `BankAccountId` bigint(20) NULL,
    `VirtualBalance` decimal(58,29) NOT NULL,
    `CreatedAt` datetime NOT NULL,
    `IsDeleted` bit(1) NOT NULL DEFAULT b'0',
    CONSTRAINT `PK_Arenas` PRIMARY KEY (`Id`),
    CONSTRAINT `FK_Arenas_BankAccounts` FOREIGN KEY (`BankAccountId`) REFERENCES `BankAccounts` (`Id`) ON DELETE SET NULL,
    CONSTRAINT `FK_Arenas_Currencies` FOREIGN KEY (`CurrencyId`) REFERENCES `Currencies` (`Id`),
    CONSTRAINT `FK_Arenas_EconomicZones` FOREIGN KEY (`EconomicZoneId`) REFERENCES `EconomicZones` (`Id`)
) CHARACTER SET=utf8mb4;

CREATE TABLE `ArenaCells` (
    `Id` bigint(20) NOT NULL AUTO_INCREMENT,
    `ArenaId` bigint(20) NOT NULL,
    `CellId` bigint(20) NOT NULL,
    `Role` int(11) NOT NULL,
    CONSTRAINT `PK_ArenaCells` PRIMARY KEY (`Id`),
    CONSTRAINT `FK_ArenaCells_Arenas` FOREIGN KEY (`ArenaId`) REFERENCES `Arenas` (`Id`) ON DELETE CASCADE,
    CONSTRAINT `FK_ArenaCells_Cells` FOREIGN KEY (`CellId`) REFERENCES `Cells` (`Id`) ON DELETE CASCADE
) CHARACTER SET=utf8mb4;

CREATE TABLE `ArenaCombatantClasses` (
    `Id` bigint(20) NOT NULL AUTO_INCREMENT,
    `ArenaId` bigint(20) NOT NULL,
    `Name` varchar(200) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
    `Description` varchar(4000) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
    `EligibilityProgId` bigint(20) NOT NULL,
    `AdminNpcLoaderProgId` bigint(20) NULL,
    `ResurrectNpcOnDeath` bit(1) NOT NULL DEFAULT b'0',
    `DefaultStageNameTemplate` varchar(200) CHARACTER SET utf8 COLLATE utf8_general_ci NULL,
    `DefaultSignatureColour` varchar(50) CHARACTER SET utf8 COLLATE utf8_general_ci NULL,
    CONSTRAINT `PK_ArenaCombatantClasses` PRIMARY KEY (`Id`),
    CONSTRAINT `FK_ArenaCombatantClasses_AdminNpcLoaderProg` FOREIGN KEY (`AdminNpcLoaderProgId`) REFERENCES `FutureProgs` (`Id`) ON DELETE SET NULL,
    CONSTRAINT `FK_ArenaCombatantClasses_Arenas` FOREIGN KEY (`ArenaId`) REFERENCES `Arenas` (`Id`) ON DELETE CASCADE,
    CONSTRAINT `FK_ArenaCombatantClasses_EligibilityProg` FOREIGN KEY (`EligibilityProgId`) REFERENCES `FutureProgs` (`Id`)
) CHARACTER SET=utf8mb4;

CREATE TABLE `ArenaEventTypes` (
    `Id` bigint(20) NOT NULL AUTO_INCREMENT,
    `ArenaId` bigint(20) NOT NULL,
    `Name` varchar(200) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
    `BringYourOwn` bit(1) NOT NULL DEFAULT b'0',
    `RegistrationDurationSeconds` int(11) NOT NULL,
    `PreparationDurationSeconds` int(11) NOT NULL,
    `TimeLimitSeconds` int(11) NULL,
    `BettingModel` int(11) NOT NULL,
    `AppearanceFee` decimal(58,29) NOT NULL,
    `VictoryFee` decimal(58,29) NOT NULL,
    `IntroProgId` bigint(20) NULL,
    `ScoringProgId` bigint(20) NULL,
    `ResolutionOverrideProgId` bigint(20) NULL,
    CONSTRAINT `PK_ArenaEventTypes` PRIMARY KEY (`Id`),
    CONSTRAINT `FK_ArenaEventTypes_Arenas` FOREIGN KEY (`ArenaId`) REFERENCES `Arenas` (`Id`) ON DELETE CASCADE,
    CONSTRAINT `FK_ArenaEventTypes_IntroProg` FOREIGN KEY (`IntroProgId`) REFERENCES `FutureProgs` (`Id`) ON DELETE SET NULL,
    CONSTRAINT `FK_ArenaEventTypes_ResolutionProg` FOREIGN KEY (`ResolutionOverrideProgId`) REFERENCES `FutureProgs` (`Id`) ON DELETE SET NULL,
    CONSTRAINT `FK_ArenaEventTypes_ScoringProg` FOREIGN KEY (`ScoringProgId`) REFERENCES `FutureProgs` (`Id`) ON DELETE SET NULL
) CHARACTER SET=utf8mb4;

CREATE TABLE `ArenaManagers` (
    `Id` bigint(20) NOT NULL AUTO_INCREMENT,
    `ArenaId` bigint(20) NOT NULL,
    `CharacterId` bigint(20) NOT NULL,
    `CreatedAt` datetime NOT NULL,
    CONSTRAINT `PK_ArenaManagers` PRIMARY KEY (`Id`),
    CONSTRAINT `FK_ArenaManagers_Arenas` FOREIGN KEY (`ArenaId`) REFERENCES `Arenas` (`Id`) ON DELETE CASCADE,
    CONSTRAINT `FK_ArenaManagers_Characters` FOREIGN KEY (`CharacterId`) REFERENCES `Characters` (`Id`) ON DELETE CASCADE
) CHARACTER SET=utf8mb4;

CREATE TABLE `ArenaRatings` (
    `Id` bigint(20) NOT NULL AUTO_INCREMENT,
    `ArenaId` bigint(20) NOT NULL,
    `CharacterId` bigint(20) NOT NULL,
    `CombatantClassId` bigint(20) NOT NULL,
    `Rating` decimal(58,29) NOT NULL,
    `LastUpdatedAt` datetime NOT NULL,
    CONSTRAINT `PK_ArenaRatings` PRIMARY KEY (`Id`),
    CONSTRAINT `FK_ArenaRatings_Arenas` FOREIGN KEY (`ArenaId`) REFERENCES `Arenas` (`Id`) ON DELETE CASCADE,
    CONSTRAINT `FK_ArenaRatings_Characters` FOREIGN KEY (`CharacterId`) REFERENCES `Characters` (`Id`) ON DELETE CASCADE,
    CONSTRAINT `FK_ArenaRatings_CombatantClasses` FOREIGN KEY (`CombatantClassId`) REFERENCES `ArenaCombatantClasses` (`Id`) ON DELETE CASCADE
) CHARACTER SET=utf8mb4;

CREATE TABLE `ArenaEvents` (
    `Id` bigint(20) NOT NULL AUTO_INCREMENT,
    `ArenaId` bigint(20) NOT NULL,
    `ArenaEventTypeId` bigint(20) NOT NULL,
    `State` int(11) NOT NULL,
    `BringYourOwn` bit(1) NOT NULL DEFAULT b'0',
    `RegistrationDurationSeconds` int(11) NOT NULL,
    `PreparationDurationSeconds` int(11) NOT NULL,
    `TimeLimitSeconds` int(11) NULL,
    `BettingModel` int(11) NOT NULL,
    `AppearanceFee` decimal(58,29) NOT NULL,
    `VictoryFee` decimal(58,29) NOT NULL,
    `CreatedAt` datetime NOT NULL,
    `ScheduledAt` datetime NOT NULL,
    `RegistrationOpensAt` datetime NULL,
    `StartedAt` datetime NULL,
    `ResolvedAt` datetime NULL,
    `CompletedAt` datetime NULL,
    `AbortedAt` datetime NULL,
    `CancellationReason` varchar(4000) CHARACTER SET utf8 COLLATE utf8_general_ci NULL,
    CONSTRAINT `PK_ArenaEvents` PRIMARY KEY (`Id`),
    CONSTRAINT `FK_ArenaEvents_Arenas` FOREIGN KEY (`ArenaId`) REFERENCES `Arenas` (`Id`) ON DELETE CASCADE,
    CONSTRAINT `FK_ArenaEvents_EventTypes` FOREIGN KEY (`ArenaEventTypeId`) REFERENCES `ArenaEventTypes` (`Id`)
) CHARACTER SET=utf8mb4;

CREATE TABLE `ArenaEventTypeSides` (
    `Id` bigint(20) NOT NULL AUTO_INCREMENT,
    `ArenaEventTypeId` bigint(20) NOT NULL,
    `Index` int(11) NOT NULL,
    `Capacity` int(11) NOT NULL,
    `Policy` int(11) NOT NULL,
    `AllowNpcSignup` bit(1) NOT NULL DEFAULT b'0',
    `AutoFillNpc` bit(1) NOT NULL DEFAULT b'0',
    `OutfitProgId` bigint(20) NULL,
    `NpcLoaderProgId` bigint(20) NULL,
    CONSTRAINT `PK_ArenaEventTypeSides` PRIMARY KEY (`Id`),
    CONSTRAINT `FK_ArenaEventTypeSides_EventTypes` FOREIGN KEY (`ArenaEventTypeId`) REFERENCES `ArenaEventTypes` (`Id`) ON DELETE CASCADE,
    CONSTRAINT `FK_ArenaEventTypeSides_NpcLoaderProg` FOREIGN KEY (`NpcLoaderProgId`) REFERENCES `FutureProgs` (`Id`) ON DELETE SET NULL,
    CONSTRAINT `FK_ArenaEventTypeSides_OutfitProg` FOREIGN KEY (`OutfitProgId`) REFERENCES `FutureProgs` (`Id`) ON DELETE SET NULL
) CHARACTER SET=utf8mb4;

CREATE TABLE `ArenaBetPayouts` (
    `Id` bigint(20) NOT NULL AUTO_INCREMENT,
    `ArenaEventId` bigint(20) NOT NULL,
    `CharacterId` bigint(20) NOT NULL,
    `Amount` decimal(58,29) NOT NULL,
    `IsBlocked` bit(1) NOT NULL DEFAULT b'0',
    `CreatedAt` datetime NOT NULL,
    `CollectedAt` datetime NULL,
    CONSTRAINT `PK_ArenaBetPayouts` PRIMARY KEY (`Id`),
    CONSTRAINT `FK_ArenaBetPayouts_ArenaEvents` FOREIGN KEY (`ArenaEventId`) REFERENCES `ArenaEvents` (`Id`) ON DELETE CASCADE,
    CONSTRAINT `FK_ArenaBetPayouts_Characters` FOREIGN KEY (`CharacterId`) REFERENCES `Characters` (`Id`) ON DELETE CASCADE
) CHARACTER SET=utf8mb4;

CREATE TABLE `ArenaBetPools` (
    `Id` bigint(20) NOT NULL AUTO_INCREMENT,
    `ArenaEventId` bigint(20) NOT NULL,
    `SideIndex` int(11) NULL,
    `TotalStake` decimal(58,29) NOT NULL,
    `TakeRate` decimal(58,29) NOT NULL,
    CONSTRAINT `PK_ArenaBetPools` PRIMARY KEY (`Id`),
    CONSTRAINT `FK_ArenaBetPools_ArenaEvents` FOREIGN KEY (`ArenaEventId`) REFERENCES `ArenaEvents` (`Id`) ON DELETE CASCADE
) CHARACTER SET=utf8mb4;

CREATE TABLE `ArenaBets` (
    `Id` bigint(20) NOT NULL AUTO_INCREMENT,
    `ArenaEventId` bigint(20) NOT NULL,
    `CharacterId` bigint(20) NOT NULL,
    `SideIndex` int(11) NULL,
    `Stake` decimal(58,29) NOT NULL,
    `FixedDecimalOdds` decimal(58,29) NULL,
    `ModelSnapshot` text CHARACTER SET utf8 COLLATE utf8_general_ci NULL,
    `IsCancelled` bit(1) NOT NULL DEFAULT b'0',
    `PlacedAt` datetime NOT NULL,
    `CancelledAt` datetime NULL,
    CONSTRAINT `PK_ArenaBets` PRIMARY KEY (`Id`),
    CONSTRAINT `FK_ArenaBets_ArenaEvents` FOREIGN KEY (`ArenaEventId`) REFERENCES `ArenaEvents` (`Id`) ON DELETE CASCADE,
    CONSTRAINT `FK_ArenaBets_Characters` FOREIGN KEY (`CharacterId`) REFERENCES `Characters` (`Id`) ON DELETE CASCADE
) CHARACTER SET=utf8mb4;

CREATE TABLE `ArenaEventSides` (
    `Id` bigint(20) NOT NULL AUTO_INCREMENT,
    `ArenaEventId` bigint(20) NOT NULL,
    `SideIndex` int(11) NOT NULL,
    `Capacity` int(11) NOT NULL,
    `Policy` int(11) NOT NULL,
    `AllowNpcSignup` bit(1) NOT NULL DEFAULT b'0',
    `AutoFillNpc` bit(1) NOT NULL DEFAULT b'0',
    `OutfitProgId` bigint(20) NULL,
    `NpcLoaderProgId` bigint(20) NULL,
    CONSTRAINT `PK_ArenaEventSides` PRIMARY KEY (`Id`),
    CONSTRAINT `FK_ArenaEventSides_ArenaEvents` FOREIGN KEY (`ArenaEventId`) REFERENCES `ArenaEvents` (`Id`) ON DELETE CASCADE,
    CONSTRAINT `FK_ArenaEventSides_NpcLoaderProg` FOREIGN KEY (`NpcLoaderProgId`) REFERENCES `FutureProgs` (`Id`) ON DELETE SET NULL,
    CONSTRAINT `FK_ArenaEventSides_OutfitProg` FOREIGN KEY (`OutfitProgId`) REFERENCES `FutureProgs` (`Id`) ON DELETE SET NULL
) CHARACTER SET=utf8mb4;

CREATE TABLE `ArenaFinanceSnapshots` (
    `Id` bigint(20) NOT NULL AUTO_INCREMENT,
    `ArenaId` bigint(20) NOT NULL,
    `ArenaEventId` bigint(20) NULL,
    `Period` varchar(100) CHARACTER SET utf8 COLLATE utf8_general_ci NULL,
    `Revenue` decimal(58,29) NOT NULL,
    `Costs` decimal(58,29) NOT NULL,
    `TaxWithheld` decimal(58,29) NOT NULL,
    `Profit` decimal(58,29) NOT NULL,
    `CreatedAt` datetime NOT NULL,
    CONSTRAINT `PK_ArenaFinanceSnapshots` PRIMARY KEY (`Id`),
    CONSTRAINT `FK_ArenaFinanceSnapshots_ArenaEvents` FOREIGN KEY (`ArenaEventId`) REFERENCES `ArenaEvents` (`Id`) ON DELETE SET NULL,
    CONSTRAINT `FK_ArenaFinanceSnapshots_Arenas` FOREIGN KEY (`ArenaId`) REFERENCES `Arenas` (`Id`) ON DELETE CASCADE
) CHARACTER SET=utf8mb4;

CREATE TABLE `ArenaReservations` (
    `Id` bigint(20) NOT NULL AUTO_INCREMENT,
    `ArenaEventId` bigint(20) NOT NULL,
    `SideIndex` int(11) NOT NULL,
    `CharacterId` bigint(20) NULL,
    `ClanId` bigint(20) NULL,
    `ReservedAt` datetime NOT NULL,
    `ExpiresAt` datetime NOT NULL,
    CONSTRAINT `PK_ArenaReservations` PRIMARY KEY (`Id`),
    CONSTRAINT `FK_ArenaReservations_ArenaEvents` FOREIGN KEY (`ArenaEventId`) REFERENCES `ArenaEvents` (`Id`) ON DELETE CASCADE,
    CONSTRAINT `FK_ArenaReservations_Characters` FOREIGN KEY (`CharacterId`) REFERENCES `Characters` (`Id`) ON DELETE SET NULL,
    CONSTRAINT `FK_ArenaReservations_Clans` FOREIGN KEY (`ClanId`) REFERENCES `Clans` (`Id`) ON DELETE SET NULL
) CHARACTER SET=utf8mb4;

CREATE TABLE `ArenaEventTypeSideAllowedClasses` (
    `ArenaEventTypeSideId` bigint(20) NOT NULL,
    `ArenaCombatantClassId` bigint(20) NOT NULL,
    CONSTRAINT `PK_ArenaEventTypeSideAllowedClasses` PRIMARY KEY (`ArenaEventTypeSideId`, `ArenaCombatantClassId`),
    CONSTRAINT `FK_ArenaEventTypeSideAllowedClasses_Classes` FOREIGN KEY (`ArenaCombatantClassId`) REFERENCES `ArenaCombatantClasses` (`Id`) ON DELETE CASCADE,
    CONSTRAINT `FK_ArenaEventTypeSideAllowedClasses_Sides` FOREIGN KEY (`ArenaEventTypeSideId`) REFERENCES `ArenaEventTypeSides` (`Id`) ON DELETE CASCADE
) CHARACTER SET=utf8mb4;

CREATE TABLE `ArenaSignups` (
    `Id` bigint(20) NOT NULL AUTO_INCREMENT,
    `ArenaEventId` bigint(20) NOT NULL,
    `CharacterId` bigint(20) NOT NULL,
    `CombatantClassId` bigint(20) NOT NULL,
    `SideIndex` int(11) NOT NULL,
    `IsNpc` bit(1) NOT NULL DEFAULT b'0',
    `StageName` varchar(200) CHARACTER SET utf8 COLLATE utf8_general_ci NULL,
    `SignatureColour` varchar(50) CHARACTER SET utf8 COLLATE utf8_general_ci NULL,
    `StartingRating` decimal(58,29) NULL,
    `SignedUpAt` datetime NOT NULL,
    `ArenaReservationId` bigint(20) NULL,
    CONSTRAINT `PK_ArenaSignups` PRIMARY KEY (`Id`),
    CONSTRAINT `FK_ArenaSignups_ArenaEvents` FOREIGN KEY (`ArenaEventId`) REFERENCES `ArenaEvents` (`Id`) ON DELETE CASCADE,
    CONSTRAINT `FK_ArenaSignups_Characters` FOREIGN KEY (`CharacterId`) REFERENCES `Characters` (`Id`) ON DELETE CASCADE,
    CONSTRAINT `FK_ArenaSignups_CombatantClasses` FOREIGN KEY (`CombatantClassId`) REFERENCES `ArenaCombatantClasses` (`Id`) ON DELETE CASCADE,
    CONSTRAINT `FK_ArenaSignups_Reservations` FOREIGN KEY (`ArenaReservationId`) REFERENCES `ArenaReservations` (`Id`) ON DELETE SET NULL
) CHARACTER SET=utf8mb4;

CREATE TABLE `ArenaEliminations` (
    `Id` bigint(20) NOT NULL AUTO_INCREMENT,
    `ArenaEventId` bigint(20) NOT NULL,
    `ArenaSignupId` bigint(20) NOT NULL,
    `CharacterId` bigint(20) NOT NULL,
    `Reason` int(11) NOT NULL,
    `OccurredAt` datetime NOT NULL,
    CONSTRAINT `PK_ArenaEliminations` PRIMARY KEY (`Id`),
    CONSTRAINT `FK_ArenaEliminations_ArenaEvents` FOREIGN KEY (`ArenaEventId`) REFERENCES `ArenaEvents` (`Id`) ON DELETE CASCADE,
    CONSTRAINT `FK_ArenaEliminations_Characters` FOREIGN KEY (`CharacterId`) REFERENCES `Characters` (`Id`) ON DELETE CASCADE,
    CONSTRAINT `FK_ArenaEliminations_Signups` FOREIGN KEY (`ArenaSignupId`) REFERENCES `ArenaSignups` (`Id`) ON DELETE CASCADE
) CHARACTER SET=utf8mb4;

CREATE INDEX `FK_ArenaBetPayouts_ArenaEvents` ON `ArenaBetPayouts` (`ArenaEventId`);

CREATE INDEX `FK_ArenaBetPayouts_Characters` ON `ArenaBetPayouts` (`CharacterId`);

CREATE INDEX `FK_ArenaBetPools_ArenaEvents` ON `ArenaBetPools` (`ArenaEventId`);

CREATE INDEX `FK_ArenaBets_ArenaEvents` ON `ArenaBets` (`ArenaEventId`);

CREATE INDEX `FK_ArenaBets_Characters` ON `ArenaBets` (`CharacterId`);

CREATE INDEX `FK_ArenaCells_Arenas` ON `ArenaCells` (`ArenaId`);

CREATE INDEX `FK_ArenaCells_Cells` ON `ArenaCells` (`CellId`);

CREATE INDEX `FK_ArenaCombatantClasses_AdminNpcLoaderProg` ON `ArenaCombatantClasses` (`AdminNpcLoaderProgId`);

CREATE INDEX `FK_ArenaCombatantClasses_Arenas` ON `ArenaCombatantClasses` (`ArenaId`);

CREATE INDEX `FK_ArenaCombatantClasses_EligibilityProg` ON `ArenaCombatantClasses` (`EligibilityProgId`);

CREATE INDEX `FK_ArenaEliminations_ArenaEvents` ON `ArenaEliminations` (`ArenaEventId`);

CREATE INDEX `FK_ArenaEliminations_Characters` ON `ArenaEliminations` (`CharacterId`);

CREATE INDEX `FK_ArenaEliminations_Signups` ON `ArenaEliminations` (`ArenaSignupId`);

CREATE INDEX `FK_ArenaEvents_Arenas` ON `ArenaEvents` (`ArenaId`);

CREATE INDEX `FK_ArenaEvents_EventTypes` ON `ArenaEvents` (`ArenaEventTypeId`);

CREATE INDEX `FK_ArenaEventSides_ArenaEvents` ON `ArenaEventSides` (`ArenaEventId`);

CREATE INDEX `FK_ArenaEventSides_NpcLoaderProg` ON `ArenaEventSides` (`NpcLoaderProgId`);

CREATE INDEX `FK_ArenaEventSides_OutfitProg` ON `ArenaEventSides` (`OutfitProgId`);

CREATE INDEX `FK_ArenaEventTypes_Arenas` ON `ArenaEventTypes` (`ArenaId`);

CREATE INDEX `FK_ArenaEventTypes_IntroProg` ON `ArenaEventTypes` (`IntroProgId`);

CREATE INDEX `FK_ArenaEventTypes_ResolutionProg` ON `ArenaEventTypes` (`ResolutionOverrideProgId`);

CREATE INDEX `FK_ArenaEventTypes_ScoringProg` ON `ArenaEventTypes` (`ScoringProgId`);

CREATE INDEX `IX_ArenaEventTypeSideAllowedClasses_ArenaCombatantClassId` ON `ArenaEventTypeSideAllowedClasses` (`ArenaCombatantClassId`);

CREATE INDEX `FK_ArenaEventTypeSides_EventTypes` ON `ArenaEventTypeSides` (`ArenaEventTypeId`);

CREATE INDEX `FK_ArenaEventTypeSides_NpcLoaderProg` ON `ArenaEventTypeSides` (`NpcLoaderProgId`);

CREATE INDEX `FK_ArenaEventTypeSides_OutfitProg` ON `ArenaEventTypeSides` (`OutfitProgId`);

CREATE INDEX `FK_ArenaFinanceSnapshots_ArenaEvents` ON `ArenaFinanceSnapshots` (`ArenaEventId`);

CREATE INDEX `FK_ArenaFinanceSnapshots_Arenas` ON `ArenaFinanceSnapshots` (`ArenaId`);

CREATE INDEX `FK_ArenaManagers_Arenas` ON `ArenaManagers` (`ArenaId`);

CREATE INDEX `FK_ArenaManagers_Characters` ON `ArenaManagers` (`CharacterId`);

CREATE INDEX `FK_ArenaRatings_Arenas` ON `ArenaRatings` (`ArenaId`);

CREATE INDEX `FK_ArenaRatings_Characters` ON `ArenaRatings` (`CharacterId`);

CREATE INDEX `FK_ArenaRatings_CombatantClasses` ON `ArenaRatings` (`CombatantClassId`);

CREATE UNIQUE INDEX `UX_ArenaRatings_UniqueParticipant` ON `ArenaRatings` (`ArenaId`, `CharacterId`, `CombatantClassId`);

CREATE INDEX `FK_ArenaReservations_ArenaEvents` ON `ArenaReservations` (`ArenaEventId`);

CREATE INDEX `FK_ArenaReservations_Characters` ON `ArenaReservations` (`CharacterId`);

CREATE INDEX `FK_ArenaReservations_Clans` ON `ArenaReservations` (`ClanId`);

CREATE INDEX `FK_Arenas_BankAccounts` ON `Arenas` (`BankAccountId`);

CREATE INDEX `FK_Arenas_Currencies` ON `Arenas` (`CurrencyId`);

CREATE INDEX `FK_Arenas_EconomicZones` ON `Arenas` (`EconomicZoneId`);

CREATE INDEX `FK_ArenaSignups_ArenaEvents` ON `ArenaSignups` (`ArenaEventId`);

CREATE INDEX `FK_ArenaSignups_Characters` ON `ArenaSignups` (`CharacterId`);

CREATE INDEX `FK_ArenaSignups_CombatantClasses` ON `ArenaSignups` (`CombatantClassId`);

CREATE INDEX `FK_ArenaSignups_Reservations` ON `ArenaSignups` (`ArenaReservationId`);

INSERT INTO `__EFMigrationsHistory` (`MigrationId`, `ProductVersion`)
VALUES ('20251110082110_CombatArenaSchema', '9.0.11');

ALTER TABLE `Arenas` ADD `SignupEcho` varchar(4000) CHARACTER SET utf8 COLLATE utf8_general_ci NULL;

INSERT INTO `__EFMigrationsHistory` (`MigrationId`, `ProductVersion`)
VALUES ('20251115120000_ArenaSignupEcho', '9.0.11');

CREATE TABLE `AIStorytellerReferenceDocuments` (
    `Id` bigint NOT NULL AUTO_INCREMENT,
    `Name` longtext CHARACTER SET utf8mb4 NULL,
    `Description` longtext CHARACTER SET utf8mb4 NULL,
    `FolderName` longtext CHARACTER SET utf8mb4 NULL,
    `DocumentType` longtext CHARACTER SET utf8mb4 NULL,
    `Keywords` longtext CHARACTER SET utf8mb4 NULL,
    `DocumentContents` longtext CHARACTER SET utf8mb4 NULL,
    `RestrictedStorytellerIds` longtext CHARACTER SET utf8mb4 NULL,
    CONSTRAINT `PK_AIStorytellerReferenceDocuments` PRIMARY KEY (`Id`)
) CHARACTER SET=utf8mb4;

CREATE TABLE `AIStorytellers` (
    `Id` bigint NOT NULL AUTO_INCREMENT,
    `Name` longtext CHARACTER SET utf8mb4 NULL,
    `Description` longtext CHARACTER SET utf8mb4 NULL,
    `Model` longtext CHARACTER SET utf8mb4 NULL,
    `SystemPrompt` longtext CHARACTER SET utf8mb4 NULL,
    `AttentionAgentPrompt` longtext CHARACTER SET utf8mb4 NULL,
    `SurveillanceStrategyDefinition` longtext CHARACTER SET utf8mb4 NULL,
    `ReasoningEffort` longtext CHARACTER SET utf8mb4 NULL,
    `CustomToolCallsDefinition` longtext CHARACTER SET utf8mb4 NULL,
    `SubscribeTo5mHeartbeat` tinyint(1) NOT NULL,
    `SubscribeTo10mHeartbeat` tinyint(1) NOT NULL,
    `SubscribeTo30mHeartbeat` tinyint(1) NOT NULL,
    `SubscribeToHourHeartbeat` tinyint(1) NOT NULL,
    `HeartbeatStatus5mProgId` bigint(20) NULL,
    `HeartbeatStatus10mProgId` bigint(20) NULL,
    `HeartbeatStatus30mProgId` bigint(20) NULL,
    `HeartbeatStatus1hProgId` bigint(20) NULL,
    `IsPaused` tinyint(1) NOT NULL,
    `SubscribeToRoomEvents` tinyint(1) NOT NULL,
    CONSTRAINT `PK_AIStorytellers` PRIMARY KEY (`Id`),
    CONSTRAINT `FK_AIStorytellers_FutureProgs_HeartbeatStatus10mProgId` FOREIGN KEY (`HeartbeatStatus10mProgId`) REFERENCES `FutureProgs` (`Id`),
    CONSTRAINT `FK_AIStorytellers_FutureProgs_HeartbeatStatus1hProgId` FOREIGN KEY (`HeartbeatStatus1hProgId`) REFERENCES `FutureProgs` (`Id`),
    CONSTRAINT `FK_AIStorytellers_FutureProgs_HeartbeatStatus30mProgId` FOREIGN KEY (`HeartbeatStatus30mProgId`) REFERENCES `FutureProgs` (`Id`),
    CONSTRAINT `FK_AIStorytellers_FutureProgs_HeartbeatStatus5mProgId` FOREIGN KEY (`HeartbeatStatus5mProgId`) REFERENCES `FutureProgs` (`Id`)
) CHARACTER SET=utf8mb4;

CREATE TABLE `AIStorytellerCharacterMemories` (
    `Id` bigint NOT NULL AUTO_INCREMENT,
    `AIStorytellerId` bigint NOT NULL,
    `CharacterId` bigint(20) NOT NULL,
    `MemoryTitle` longtext CHARACTER SET utf8mb4 NULL,
    `MemoryText` longtext CHARACTER SET utf8mb4 NULL,
    `CreatedOn` datetime(6) NOT NULL,
    CONSTRAINT `PK_AIStorytellerCharacterMemories` PRIMARY KEY (`Id`),
    CONSTRAINT `FK_AIStorytellerCharacterMemories_AIStorytellers_AIStorytellerId` FOREIGN KEY (`AIStorytellerId`) REFERENCES `AIStorytellers` (`Id`) ON DELETE CASCADE,
    CONSTRAINT `FK_AIStorytellerCharacterMemories_Characters_CharacterId` FOREIGN KEY (`CharacterId`) REFERENCES `Characters` (`Id`) ON DELETE CASCADE
) CHARACTER SET=utf8mb4;

CREATE TABLE `AIStorytellerSituations` (
    `Id` bigint NOT NULL AUTO_INCREMENT,
    `AIStorytellerId` bigint NOT NULL,
    `Name` longtext CHARACTER SET utf8mb4 NULL,
    `SituationText` longtext CHARACTER SET utf8mb4 NULL,
    `CreatedOn` datetime(6) NOT NULL,
    `IsResolved` tinyint(1) NOT NULL,
    CONSTRAINT `PK_AIStorytellerSituations` PRIMARY KEY (`Id`),
    CONSTRAINT `FK_AIStorytellerSituations_AIStorytellers_AIStorytellerId` FOREIGN KEY (`AIStorytellerId`) REFERENCES `AIStorytellers` (`Id`) ON DELETE CASCADE
) CHARACTER SET=utf8mb4;

CREATE INDEX `IX_AIStorytellerCharacterMemories_AIStorytellerId` ON `AIStorytellerCharacterMemories` (`AIStorytellerId`);

CREATE INDEX `IX_AIStorytellerCharacterMemories_CharacterId` ON `AIStorytellerCharacterMemories` (`CharacterId`);

CREATE INDEX `IX_AIStorytellers_HeartbeatStatus10mProgId` ON `AIStorytellers` (`HeartbeatStatus10mProgId`);

CREATE INDEX `IX_AIStorytellers_HeartbeatStatus1hProgId` ON `AIStorytellers` (`HeartbeatStatus1hProgId`);

CREATE INDEX `IX_AIStorytellers_HeartbeatStatus30mProgId` ON `AIStorytellers` (`HeartbeatStatus30mProgId`);

CREATE INDEX `IX_AIStorytellers_HeartbeatStatus5mProgId` ON `AIStorytellers` (`HeartbeatStatus5mProgId`);

CREATE INDEX `IX_AIStorytellerSituations_AIStorytellerId` ON `AIStorytellerSituations` (`AIStorytellerId`);

INSERT INTO `__EFMigrationsHistory` (`MigrationId`, `ProductVersion`)
VALUES ('20260211095519_AIStorytellers', '9.0.11');

ALTER TABLE `AIStorytellers` ADD `SubscribeToCrimeEvents` tinyint(1) NOT NULL DEFAULT FALSE;

ALTER TABLE `AIStorytellers` ADD `SubscribeToSpeechEvents` tinyint(1) NOT NULL DEFAULT FALSE;

ALTER TABLE `AIStorytellers` ADD `SubscribeToStateEvents` tinyint(1) NOT NULL DEFAULT FALSE;

INSERT INTO `__EFMigrationsHistory` (`MigrationId`, `ProductVersion`)
VALUES ('20260211124139_AIStorytellerEventSubscriptions', '9.0.11');

ALTER TABLE `AIStorytellers` ADD `TimeSystemPrompt` longtext CHARACTER SET utf8mb4 NULL;

INSERT INTO `__EFMigrationsHistory` (`MigrationId`, `ProductVersion`)
VALUES ('20260216092441_AIStorytellerTimeSystemPrompt', '9.0.11');

ALTER TABLE `AIStorytellers` ADD `AttentionClassifierModel` longtext CHARACTER SET utf8mb4 NULL;

ALTER TABLE `AIStorytellers` ADD `AttentionClassifierReasoningEffort` longtext CHARACTER SET utf8mb4 NULL;

ALTER TABLE `AIStorytellers` ADD `TimeModel` longtext CHARACTER SET utf8mb4 NULL;

ALTER TABLE `AIStorytellers` ADD `TimeReasoningEffort` longtext CHARACTER SET utf8mb4 NULL;

INSERT INTO `__EFMigrationsHistory` (`MigrationId`, `ProductVersion`)
VALUES ('20260216095426_AIStorytellerScopedModelReasoning', '9.0.11');

ALTER TABLE `ArenaEventTypes` ADD `AutoScheduleIntervalSeconds` int(11) NULL;

ALTER TABLE `ArenaEventTypes` ADD `AutoScheduleReferenceTime` datetime NULL;

INSERT INTO `__EFMigrationsHistory` (`MigrationId`, `ProductVersion`)
VALUES ('20260218120142_ArenaAutoScheduling', '9.0.11');

ALTER TABLE `AIStorytellerSituations` ADD `ScopeCharacterId` bigint(20) NULL;

ALTER TABLE `AIStorytellerSituations` ADD `ScopeRoomId` bigint(20) NULL;

CREATE INDEX `IX_AIStorytellerSituations_ScopeCharacterId` ON `AIStorytellerSituations` (`ScopeCharacterId`);

CREATE INDEX `IX_AIStorytellerSituations_ScopeRoomId` ON `AIStorytellerSituations` (`ScopeRoomId`);

ALTER TABLE `AIStorytellerSituations` ADD CONSTRAINT `FK_AIStorytellerSituations_Cells_ScopeRoomId` FOREIGN KEY (`ScopeRoomId`) REFERENCES `Cells` (`Id`);

ALTER TABLE `AIStorytellerSituations` ADD CONSTRAINT `FK_AIStorytellerSituations_Characters_ScopeCharacterId` FOREIGN KEY (`ScopeCharacterId`) REFERENCES `Characters` (`Id`);

INSERT INTO `__EFMigrationsHistory` (`MigrationId`, `ProductVersion`)
VALUES ('20260221112947_AIStorytellerSituationScopes', '9.0.11');

ALTER TABLE `ArenaEventTypes` ADD `AllowSurrender` bit(1) NOT NULL DEFAULT b'1';

ALTER TABLE `ArenaEventTypes` ADD `EliminationMode` int(11) NOT NULL DEFAULT 0;

INSERT INTO `__EFMigrationsHistory` (`MigrationId`, `ProductVersion`)
VALUES ('20260222081900_ArenaEventTypeEliminationModes', '9.0.11');

ALTER TABLE `ArenaCombatantClasses` ADD `FullyRestoreNpcOnCompletion` bit(1) NOT NULL DEFAULT b'0';

INSERT INTO `__EFMigrationsHistory` (`MigrationId`, `ProductVersion`)
VALUES ('20260222112522_ArenaNpcCompletionRestore', '9.0.11');

ALTER TABLE `ArenaCombatantClasses` DROP COLUMN `DefaultStageNameTemplate`;

ALTER TABLE `ArenaCombatantClasses` ADD `DefaultStageNameProfileId` bigint(20) NULL;

CREATE INDEX `FK_ArenaCombatantClasses_DefaultStageNameProfile` ON `ArenaCombatantClasses` (`DefaultStageNameProfileId`);

ALTER TABLE `ArenaCombatantClasses` ADD CONSTRAINT `FK_ArenaCombatantClasses_DefaultStageNameProfile` FOREIGN KEY (`DefaultStageNameProfileId`) REFERENCES `RandomNameProfiles` (`Id`) ON DELETE SET NULL;

INSERT INTO `__EFMigrationsHistory` (`MigrationId`, `ProductVersion`)
VALUES ('20260225115630_ArenaStageNameProfile', '9.0.11');

ALTER TABLE `ArenaEventTypes` ADD `EloKFactor` decimal(58,29) NOT NULL DEFAULT 32.0;

ALTER TABLE `ArenaEventTypes` ADD `EloStyle` int(11) NOT NULL DEFAULT 0;

INSERT INTO `__EFMigrationsHistory` (`MigrationId`, `ProductVersion`)
VALUES ('20260225233442_ArenaEloStrategyOptions', '9.0.11');

ALTER TABLE `ArenaEventTypeSides` ADD `MaximumRating` decimal(58,29) NULL;

ALTER TABLE `ArenaEventTypeSides` ADD `MinimumRating` decimal(58,29) NULL;

INSERT INTO `__EFMigrationsHistory` (`MigrationId`, `ProductVersion`)
VALUES ('20260226124500_ArenaSideRatingRanges', '9.0.11');

ALTER TABLE `Arenas` ADD `OnArenaEventPhaseProgId` bigint(20) NULL;

ALTER TABLE `ArenaEventTypes` ADD `PayNpcAppearanceFee` bit(1) NOT NULL DEFAULT b'0';

ALTER TABLE `ArenaEvents` ADD `PayNpcAppearanceFee` bit(1) NOT NULL DEFAULT b'0';

ALTER TABLE `ArenaBetPayouts` ADD `PayoutType` int(11) NOT NULL DEFAULT 0;

CREATE INDEX `FK_Arenas_OnArenaEventPhaseProg` ON `Arenas` (`OnArenaEventPhaseProgId`);

ALTER TABLE `Arenas` ADD CONSTRAINT `FK_Arenas_OnArenaEventPhaseProg` FOREIGN KEY (`OnArenaEventPhaseProgId`) REFERENCES `FutureProgs` (`Id`) ON DELETE SET NULL;

INSERT INTO `__EFMigrationsHistory` (`MigrationId`, `ProductVersion`)
VALUES ('20260227120000_ArenaPhaseProgAppearancePayouts', '9.0.11');

ALTER TABLE `RegionalClimates` ADD `Description` text CHARACTER SET utf8 COLLATE utf8_general_ci NULL;

ALTER TABLE `ClimateModels` ADD `Description` text CHARACTER SET utf8 COLLATE utf8_general_ci NULL;

INSERT INTO `__EFMigrationsHistory` (`MigrationId`, `ProductVersion`)
VALUES ('20260308065322_ClimateDescriptions', '9.0.11');

ALTER TABLE `WeatherControllers` ADD `CurrentTemperatureFluctuation` double NOT NULL DEFAULT 0.0;

ALTER TABLE `WeatherControllers` ADD `OppositeHemisphere` bit(1) NOT NULL DEFAULT 0;

ALTER TABLE `RegionalClimates` ADD `TemperatureFluctuationPeriodMinutes` int(11) NOT NULL DEFAULT 0;

ALTER TABLE `RegionalClimates` ADD `TemperatureFluctuationStandardDeviation` double NOT NULL DEFAULT 0.0;

INSERT INTO `__EFMigrationsHistory` (`MigrationId`, `ProductVersion`)
VALUES ('20260309072751_WeatherModelSimplification', '9.0.11');

ALTER TABLE `Characters` ADD `SatiationReserve` double NOT NULL DEFAULT 0.0;

ALTER TABLE `Characters` DROP COLUMN `Calories`;

INSERT INTO `__EFMigrationsHistory` (`MigrationId`, `ProductVersion`)
VALUES ('20260309222608_ReplaceCharacterCaloriesWithSatiationReserve', '9.0.11');

ALTER TABLE `Races_EdibleMaterials` DROP COLUMN `CaloriesPerKilogram`;

ALTER TABLE `RaceEdibleForagableYields` DROP COLUMN `CaloriesPerYield`;

ALTER TABLE `Liquids` DROP COLUMN `CaloriesPerLitre`;

INSERT INTO `__EFMigrationsHistory` (`MigrationId`, `ProductVersion`)
VALUES ('20260309225356_DropObsoleteNutritionCalories', '9.0.11');

ALTER TABLE `Infections` ADD `VirulenceMultiplier` double NOT NULL DEFAULT 1.0;

INSERT INTO `__EFMigrationsHistory` (`MigrationId`, `ProductVersion`)
VALUES ('20260310122815_InfectionVirulenceMultiplier', '9.0.11');

ALTER TABLE `WeaponAttacks` ADD `OnUseProgId` bigint(20) NULL;

ALTER TABLE `Liquids` ADD `SurfaceReactionInfo` text CHARACTER SET utf8 COLLATE utf8_general_ci NULL;

ALTER TABLE `Gases` ADD `OxidationFactor` double NOT NULL DEFAULT '1.0';

INSERT INTO `__EFMigrationsHistory` (`MigrationId`, `ProductVersion`)
VALUES ('20260316112529_NaturalRangedAttacksAndElementalContact', '9.0.11');

ALTER TABLE `CharacterCombatSettings` ADD `PriorityProgId` bigint(20) NULL;

ALTER TABLE `Races` ADD `DefaultCombatSettingId` bigint(20) NULL;

CREATE INDEX `FK_CharacterCombatSettings_PriorityProg_idx` ON `CharacterCombatSettings` (`PriorityProgId`);

CREATE INDEX `FK_Races_CharacterCombatSettings_idx` ON `Races` (`DefaultCombatSettingId`);

ALTER TABLE `CharacterCombatSettings` ADD CONSTRAINT `FK_CharacterCombatSettings_PriorityProg` FOREIGN KEY (`PriorityProgId`) REFERENCES `FutureProgs` (`Id`) ON DELETE SET NULL;

ALTER TABLE `Races` ADD CONSTRAINT `FK_Races_CharacterCombatSettings` FOREIGN KEY (`DefaultCombatSettingId`) REFERENCES `CharacterCombatSettings` (`Id`) ON DELETE SET NULL;

INSERT INTO `__EFMigrationsHistory` (`MigrationId`, `ProductVersion`)
VALUES ('20260320180000_CombatSettingPriorityAndRaceDefault', '9.0.11');

ALTER TABLE `Races` RENAME INDEX `FK_Races_CharacterCombatSettings_idx` TO `IX_Races_DefaultCombatSettingId`;

ALTER TABLE `CharacterCombatSettings` RENAME INDEX `FK_CharacterCombatSettings_PriorityProg_idx` TO `IX_CharacterCombatSettings_PriorityProgId`;

ALTER TABLE `VariableValues` ADD `ReferenceTypeDefinition` varchar(255) CHARACTER SET utf8 COLLATE utf8_general_ci NULL;

ALTER TABLE `VariableValues` ADD `ValueTypeDefinition` varchar(255) CHARACTER SET utf8 COLLATE utf8_general_ci NULL;

ALTER TABLE `VariableDefinitions` ADD `ContainedTypeDefinition` varchar(255) CHARACTER SET utf8 COLLATE utf8_general_ci NULL;

ALTER TABLE `VariableDefinitions` ADD `OwnerTypeDefinition` varchar(255) CHARACTER SET utf8 COLLATE utf8_general_ci NULL;

ALTER TABLE `VariableDefaults` ADD `OwnerTypeDefinition` varchar(255) CHARACTER SET utf8 COLLATE utf8_general_ci NULL;

ALTER TABLE `FutureProgs_Parameters` ADD `ParameterTypeDefinition` varchar(255) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL DEFAULT '';

ALTER TABLE `FutureProgs` ADD `ReturnTypeDefinition` varchar(255) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL DEFAULT '';

UPDATE FutureProgs SET ReturnTypeDefinition = CONCAT('v1:', LOWER(HEX(ReturnType)));

UPDATE FutureProgs_Parameters SET ParameterTypeDefinition = CONCAT('v1:', LOWER(HEX(ParameterType)));

UPDATE VariableDefaults SET OwnerTypeDefinition = CONCAT('v1:', LOWER(HEX(OwnerType)));

UPDATE VariableDefinitions SET OwnerTypeDefinition = CONCAT('v1:', LOWER(HEX(OwnerType))), ContainedTypeDefinition = CONCAT('v1:', LOWER(HEX(ContainedType)));

UPDATE VariableValues SET ReferenceTypeDefinition = CONCAT('v1:', LOWER(HEX(ReferenceType))), ValueTypeDefinition = CONCAT('v1:', LOWER(HEX(ValueType)));

INSERT INTO `__EFMigrationsHistory` (`MigrationId`, `ProductVersion`)
VALUES ('20260321102002_FutureProgTypeDefinitionsStage1', '9.0.11');

DROP PROCEDURE IF EXISTS `POMELO_BEFORE_DROP_PRIMARY_KEY`;
DELIMITER //
CREATE PROCEDURE `POMELO_BEFORE_DROP_PRIMARY_KEY`(IN `SCHEMA_NAME_ARGUMENT` VARCHAR(255), IN `TABLE_NAME_ARGUMENT` VARCHAR(255))
BEGIN
	DECLARE HAS_AUTO_INCREMENT_ID TINYINT(1);
	DECLARE PRIMARY_KEY_COLUMN_NAME VARCHAR(255);
	DECLARE PRIMARY_KEY_TYPE VARCHAR(255);
	DECLARE SQL_EXP VARCHAR(1000);
	SELECT COUNT(*)
		INTO HAS_AUTO_INCREMENT_ID
		FROM `information_schema`.`COLUMNS`
		WHERE `TABLE_SCHEMA` = (SELECT IFNULL(SCHEMA_NAME_ARGUMENT, SCHEMA()))
			AND `TABLE_NAME` = TABLE_NAME_ARGUMENT
			AND `Extra` = 'auto_increment'
			AND `COLUMN_KEY` = 'PRI'
			LIMIT 1;
	IF HAS_AUTO_INCREMENT_ID THEN
		SELECT `COLUMN_TYPE`
			INTO PRIMARY_KEY_TYPE
			FROM `information_schema`.`COLUMNS`
			WHERE `TABLE_SCHEMA` = (SELECT IFNULL(SCHEMA_NAME_ARGUMENT, SCHEMA()))
				AND `TABLE_NAME` = TABLE_NAME_ARGUMENT
				AND `COLUMN_KEY` = 'PRI'
			LIMIT 1;
		SELECT `COLUMN_NAME`
			INTO PRIMARY_KEY_COLUMN_NAME
			FROM `information_schema`.`COLUMNS`
			WHERE `TABLE_SCHEMA` = (SELECT IFNULL(SCHEMA_NAME_ARGUMENT, SCHEMA()))
				AND `TABLE_NAME` = TABLE_NAME_ARGUMENT
				AND `COLUMN_KEY` = 'PRI'
			LIMIT 1;
		SET SQL_EXP = CONCAT('ALTER TABLE `', (SELECT IFNULL(SCHEMA_NAME_ARGUMENT, SCHEMA())), '`.`', TABLE_NAME_ARGUMENT, '` MODIFY COLUMN `', PRIMARY_KEY_COLUMN_NAME, '` ', PRIMARY_KEY_TYPE, ' NOT NULL;');
		SET @SQL_EXP = SQL_EXP;
		PREPARE SQL_EXP_EXECUTE FROM @SQL_EXP;
		EXECUTE SQL_EXP_EXECUTE;
		DEALLOCATE PREPARE SQL_EXP_EXECUTE;
	END IF;
END //
DELIMITER ;

DROP PROCEDURE IF EXISTS `POMELO_AFTER_ADD_PRIMARY_KEY`;
DELIMITER //
CREATE PROCEDURE `POMELO_AFTER_ADD_PRIMARY_KEY`(IN `SCHEMA_NAME_ARGUMENT` VARCHAR(255), IN `TABLE_NAME_ARGUMENT` VARCHAR(255), IN `COLUMN_NAME_ARGUMENT` VARCHAR(255))
BEGIN
	DECLARE HAS_AUTO_INCREMENT_ID INT(11);
	DECLARE PRIMARY_KEY_COLUMN_NAME VARCHAR(255);
	DECLARE PRIMARY_KEY_TYPE VARCHAR(255);
	DECLARE SQL_EXP VARCHAR(1000);
	SELECT COUNT(*)
		INTO HAS_AUTO_INCREMENT_ID
		FROM `information_schema`.`COLUMNS`
		WHERE `TABLE_SCHEMA` = (SELECT IFNULL(SCHEMA_NAME_ARGUMENT, SCHEMA()))
			AND `TABLE_NAME` = TABLE_NAME_ARGUMENT
			AND `COLUMN_NAME` = COLUMN_NAME_ARGUMENT
			AND `COLUMN_TYPE` LIKE '%int%'
			AND `COLUMN_KEY` = 'PRI';
	IF HAS_AUTO_INCREMENT_ID THEN
		SELECT `COLUMN_TYPE`
			INTO PRIMARY_KEY_TYPE
			FROM `information_schema`.`COLUMNS`
			WHERE `TABLE_SCHEMA` = (SELECT IFNULL(SCHEMA_NAME_ARGUMENT, SCHEMA()))
				AND `TABLE_NAME` = TABLE_NAME_ARGUMENT
				AND `COLUMN_NAME` = COLUMN_NAME_ARGUMENT
				AND `COLUMN_TYPE` LIKE '%int%'
				AND `COLUMN_KEY` = 'PRI';
		SELECT `COLUMN_NAME`
			INTO PRIMARY_KEY_COLUMN_NAME
			FROM `information_schema`.`COLUMNS`
			WHERE `TABLE_SCHEMA` = (SELECT IFNULL(SCHEMA_NAME_ARGUMENT, SCHEMA()))
				AND `TABLE_NAME` = TABLE_NAME_ARGUMENT
				AND `COLUMN_NAME` = COLUMN_NAME_ARGUMENT
				AND `COLUMN_TYPE` LIKE '%int%'
				AND `COLUMN_KEY` = 'PRI';
		SET SQL_EXP = CONCAT('ALTER TABLE `', (SELECT IFNULL(SCHEMA_NAME_ARGUMENT, SCHEMA())), '`.`', TABLE_NAME_ARGUMENT, '` MODIFY COLUMN `', PRIMARY_KEY_COLUMN_NAME, '` ', PRIMARY_KEY_TYPE, ' NOT NULL AUTO_INCREMENT;');
		SET @SQL_EXP = SQL_EXP;
		PREPARE SQL_EXP_EXECUTE FROM @SQL_EXP;
		EXECUTE SQL_EXP_EXECUTE;
		DEALLOCATE PREPARE SQL_EXP_EXECUTE;
	END IF;
END //
DELIMITER ;

CALL POMELO_BEFORE_DROP_PRIMARY_KEY(NULL, 'VariableValues');
ALTER TABLE `VariableValues` DROP PRIMARY KEY;

CALL POMELO_BEFORE_DROP_PRIMARY_KEY(NULL, 'VariableDefinitions');
ALTER TABLE `VariableDefinitions` DROP PRIMARY KEY;

CALL POMELO_BEFORE_DROP_PRIMARY_KEY(NULL, 'VariableDefaults');
ALTER TABLE `VariableDefaults` DROP PRIMARY KEY;

ALTER TABLE `VariableValues` DROP COLUMN `ReferenceType`;

ALTER TABLE `VariableValues` DROP COLUMN `ValueType`;

ALTER TABLE `VariableDefinitions` DROP COLUMN `OwnerType`;

ALTER TABLE `VariableDefinitions` DROP COLUMN `ContainedType`;

ALTER TABLE `VariableDefaults` DROP COLUMN `OwnerType`;

ALTER TABLE `FutureProgs_Parameters` DROP COLUMN `ParameterType`;

ALTER TABLE `FutureProgs` DROP COLUMN `ReturnType`;

UPDATE `VariableValues` SET `ReferenceTypeDefinition` = ''
WHERE `ReferenceTypeDefinition` IS NULL;
SELECT ROW_COUNT();


ALTER TABLE `VariableValues` MODIFY COLUMN `ReferenceTypeDefinition` varchar(255) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL;

UPDATE `VariableDefinitions` SET `OwnerTypeDefinition` = ''
WHERE `OwnerTypeDefinition` IS NULL;
SELECT ROW_COUNT();


ALTER TABLE `VariableDefinitions` MODIFY COLUMN `OwnerTypeDefinition` varchar(255) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL;

UPDATE `VariableDefaults` SET `OwnerTypeDefinition` = ''
WHERE `OwnerTypeDefinition` IS NULL;
SELECT ROW_COUNT();


ALTER TABLE `VariableDefaults` MODIFY COLUMN `OwnerTypeDefinition` varchar(255) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL;

ALTER TABLE `VariableValues` ADD CONSTRAINT `PRIMARY` PRIMARY KEY (`ReferenceTypeDefinition`, `ReferenceId`, `ReferenceProperty`);

ALTER TABLE `VariableDefinitions` ADD CONSTRAINT `PRIMARY` PRIMARY KEY (`OwnerTypeDefinition`, `Property`);

ALTER TABLE `VariableDefaults` ADD CONSTRAINT `PRIMARY` PRIMARY KEY (`OwnerTypeDefinition`, `Property`);

INSERT INTO `__EFMigrationsHistory` (`MigrationId`, `ProductVersion`)
VALUES ('20260321102139_FutureProgTypeDefinitionsStage2', '9.0.11');

DROP PROCEDURE `POMELO_BEFORE_DROP_PRIMARY_KEY`;

DROP PROCEDURE `POMELO_AFTER_ADD_PRIMARY_KEY`;

CREATE TABLE `ShopDeals` (
    `Id` bigint(20) NOT NULL AUTO_INCREMENT,
    `ShopId` bigint(20) NOT NULL,
    `Name` varchar(200) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
    `DealType` int(11) NOT NULL,
    `TargetType` int(11) NOT NULL,
    `MerchandiseId` bigint(20) NULL,
    `TagId` bigint(20) NULL,
    `PriceAdjustmentPercentage` decimal(58,29) NOT NULL,
    `MinimumQuantity` int(11) NULL,
    `Applicability` int(11) NOT NULL,
    `EligibilityProgId` bigint(20) NULL,
    `ExpiryDateTime` varchar(500) CHARACTER SET utf8 COLLATE utf8_general_ci NULL,
    `IsCumulative` bit(1) NOT NULL DEFAULT 1,
    CONSTRAINT `PK_ShopDeals` PRIMARY KEY (`Id`),
    CONSTRAINT `FK_ShopDeals_FutureProgs` FOREIGN KEY (`EligibilityProgId`) REFERENCES `FutureProgs` (`Id`) ON DELETE SET NULL,
    CONSTRAINT `FK_ShopDeals_Merchandises` FOREIGN KEY (`MerchandiseId`) REFERENCES `Merchandises` (`Id`) ON DELETE SET NULL,
    CONSTRAINT `FK_ShopDeals_Shops` FOREIGN KEY (`ShopId`) REFERENCES `Shops` (`Id`) ON DELETE CASCADE,
    CONSTRAINT `FK_ShopDeals_Tags` FOREIGN KEY (`TagId`) REFERENCES `Tags` (`Id`) ON DELETE SET NULL
) CHARACTER SET=utf8mb4;

CREATE INDEX `FK_ShopDeals_FutureProgs_idx` ON `ShopDeals` (`EligibilityProgId`);

CREATE INDEX `FK_ShopDeals_Merchandises_idx` ON `ShopDeals` (`MerchandiseId`);

CREATE INDEX `FK_ShopDeals_Shops_idx` ON `ShopDeals` (`ShopId`);

CREATE INDEX `FK_ShopDeals_Tags_idx` ON `ShopDeals` (`TagId`);

INSERT INTO `__EFMigrationsHistory` (`MigrationId`, `ProductVersion`)
VALUES ('20260327103014_ShopDeals', '9.0.11');

ALTER TABLE `GameItems` ADD `OwnerId` bigint(20) NULL;

ALTER TABLE `GameItems` ADD `OwnerType` varchar(100) CHARACTER SET utf8 COLLATE utf8_general_ci NULL;

ALTER TABLE `EconomicZones` ADD `EstateAuctionHouseId` bigint(20) NULL;

ALTER TABLE `EconomicZones` ADD `EstateClaimPeriodLength` varchar(200) CHARACTER SET utf8 COLLATE utf8_general_ci NULL;

ALTER TABLE `EconomicZones` ADD `EstateDefaultDiscoverTime` varchar(200) CHARACTER SET utf8 COLLATE utf8_general_ci NULL;

ALTER TABLE `Characters` ADD `EstateHeirId` bigint(20) NULL;

ALTER TABLE `Characters` ADD `EstateHeirType` varchar(100) CHARACTER SET utf8 COLLATE utf8_general_ci NULL;

ALTER TABLE `BankAccounts` ADD `AccountOwnerFrameworkItemId` bigint(20) NULL;

ALTER TABLE `BankAccounts` ADD `AccountOwnerFrameworkItemType` varchar(100) CHARACTER SET utf8 COLLATE utf8_general_ci NULL;


UPDATE `BankAccounts`
SET `AccountOwnerFrameworkItemId` = `AccountOwnerCharacterId`,
	`AccountOwnerFrameworkItemType` = 'Character'
WHERE `AccountOwnerFrameworkItemId` IS NULL
  AND `AccountOwnerCharacterId` IS NOT NULL;

UPDATE `BankAccounts`
SET `AccountOwnerFrameworkItemId` = `AccountOwnerClanId`,
	`AccountOwnerFrameworkItemType` = 'Clan'
WHERE `AccountOwnerFrameworkItemId` IS NULL
  AND `AccountOwnerClanId` IS NOT NULL;

UPDATE `BankAccounts`
SET `AccountOwnerFrameworkItemId` = `AccountOwnerShopId`,
	`AccountOwnerFrameworkItemType` = 'Shop'
WHERE `AccountOwnerFrameworkItemId` IS NULL
  AND `AccountOwnerShopId` IS NOT NULL;


CREATE TABLE `Estates` (
    `Id` bigint(20) NOT NULL AUTO_INCREMENT,
    `EconomicZoneId` bigint(20) NOT NULL,
    `CharacterId` bigint(20) NOT NULL,
    `EstateStatus` int(11) NOT NULL,
    `EstateStartTime` varchar(255) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
    `FinalisationDate` varchar(255) CHARACTER SET utf8 COLLATE utf8_general_ci NULL,
    `InheritorId` bigint(20) NULL,
    `InheritorType` varchar(100) CHARACTER SET utf8 COLLATE utf8_general_ci NULL,
    CONSTRAINT `PRIMARY` PRIMARY KEY (`Id`),
    CONSTRAINT `FK_Estates_Characters` FOREIGN KEY (`CharacterId`) REFERENCES `Characters` (`Id`) ON DELETE CASCADE,
    CONSTRAINT `FK_Estates_EconomicZones` FOREIGN KEY (`EconomicZoneId`) REFERENCES `EconomicZones` (`Id`) ON DELETE CASCADE
) CHARACTER SET=utf8mb4;

CREATE TABLE `EstateAssets` (
    `Id` bigint(20) NOT NULL AUTO_INCREMENT,
    `EstateId` bigint(20) NOT NULL,
    `FrameworkItemId` bigint(20) NOT NULL,
    `FrameworkItemType` varchar(100) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
    `IsPresumedOwnership` bit(1) NOT NULL,
    `IsTransferred` bit(1) NOT NULL,
    `IsLiquidated` bit(1) NOT NULL,
    `LiquidatedValue` decimal(58,29) NULL,
    CONSTRAINT `PRIMARY` PRIMARY KEY (`Id`),
    CONSTRAINT `FK_EstateAssets_Estates` FOREIGN KEY (`EstateId`) REFERENCES `Estates` (`Id`) ON DELETE CASCADE
) CHARACTER SET=utf8mb4;

CREATE TABLE `EstateClaims` (
    `Id` bigint(20) NOT NULL AUTO_INCREMENT,
    `EstateId` bigint(20) NOT NULL,
    `ClaimantId` bigint(20) NOT NULL,
    `ClaimantType` varchar(100) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
    `TargetId` bigint(20) NULL,
    `TargetType` varchar(100) CHARACTER SET utf8 COLLATE utf8_general_ci NULL,
    `Amount` decimal(58,29) NOT NULL,
    `Reason` text CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
    `ClaimStatus` int(11) NOT NULL,
    `StatusReason` text CHARACTER SET utf8 COLLATE utf8_general_ci NULL,
    `IsSecured` bit(1) NOT NULL,
    `ClaimDate` varchar(255) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
    CONSTRAINT `PRIMARY` PRIMARY KEY (`Id`),
    CONSTRAINT `FK_EstateClaims_Estates` FOREIGN KEY (`EstateId`) REFERENCES `Estates` (`Id`) ON DELETE CASCADE
) CHARACTER SET=utf8mb4;

CREATE INDEX `FK_EconomicZones_EstateAuctionHouses_idx` ON `EconomicZones` (`EstateAuctionHouseId`);

CREATE INDEX `FK_EstateAssets_Estates_idx` ON `EstateAssets` (`EstateId`);

CREATE INDEX `FK_EstateClaims_Estates_idx` ON `EstateClaims` (`EstateId`);

CREATE INDEX `FK_Estates_Characters_idx` ON `Estates` (`CharacterId`);

CREATE INDEX `FK_Estates_EconomicZones_idx` ON `Estates` (`EconomicZoneId`);

ALTER TABLE `EconomicZones` ADD CONSTRAINT `FK_EconomicZones_EstateAuctionHouses` FOREIGN KEY (`EstateAuctionHouseId`) REFERENCES `AuctionHouses` (`Id`) ON DELETE SET NULL;

INSERT INTO `__EFMigrationsHistory` (`MigrationId`, `ProductVersion`)
VALUES ('20260327124234_EstateProbateAuctionLiquidation', '9.0.11');

ALTER TABLE `EconomicZones` ADD `MorgueOfficeLocationId` bigint(20) NULL;

ALTER TABLE `EconomicZones` ADD `MorgueStorageLocationId` bigint(20) NULL;

CREATE TABLE `CorpseRecoveryReports` (
    `Id` bigint(20) NOT NULL AUTO_INCREMENT,
    `LegalAuthorityId` bigint(20) NOT NULL,
    `EconomicZoneId` bigint(20) NOT NULL,
    `CorpseId` bigint(20) NOT NULL,
    `SourceCellId` bigint(20) NOT NULL,
    `DestinationCellId` bigint(20) NOT NULL,
    `ReporterId` bigint(20) NULL,
    `AssignedPatrolId` bigint(20) NULL,
    `Status` int(11) NOT NULL,
    CONSTRAINT `PK_CorpseRecoveryReports` PRIMARY KEY (`Id`),
    CONSTRAINT `FK_CorpseRecoveryReports_Characters` FOREIGN KEY (`ReporterId`) REFERENCES `Characters` (`Id`) ON DELETE SET NULL,
    CONSTRAINT `FK_CorpseRecoveryReports_DestinationCells` FOREIGN KEY (`DestinationCellId`) REFERENCES `Cells` (`Id`) ON DELETE CASCADE,
    CONSTRAINT `FK_CorpseRecoveryReports_EconomicZones` FOREIGN KEY (`EconomicZoneId`) REFERENCES `EconomicZones` (`Id`) ON DELETE CASCADE,
    CONSTRAINT `FK_CorpseRecoveryReports_GameItems` FOREIGN KEY (`CorpseId`) REFERENCES `GameItems` (`Id`) ON DELETE CASCADE,
    CONSTRAINT `FK_CorpseRecoveryReports_LegalAuthorities` FOREIGN KEY (`LegalAuthorityId`) REFERENCES `LegalAuthorities` (`Id`) ON DELETE CASCADE,
    CONSTRAINT `FK_CorpseRecoveryReports_Patrols` FOREIGN KEY (`AssignedPatrolId`) REFERENCES `Patrols` (`Id`) ON DELETE SET NULL,
    CONSTRAINT `FK_CorpseRecoveryReports_SourceCells` FOREIGN KEY (`SourceCellId`) REFERENCES `Cells` (`Id`) ON DELETE CASCADE
) CHARACTER SET=utf8mb4;

CREATE TABLE `ProbateLocations` (
    `EconomicZoneId` bigint(20) NOT NULL,
    `CellId` bigint(20) NOT NULL,
    CONSTRAINT `PRIMARY` PRIMARY KEY (`EconomicZoneId`, `CellId`),
    CONSTRAINT `FK_ProbateLocations_Cells` FOREIGN KEY (`CellId`) REFERENCES `Cells` (`Id`) ON DELETE CASCADE,
    CONSTRAINT `FK_ProbateLocations_EconomicZones` FOREIGN KEY (`EconomicZoneId`) REFERENCES `EconomicZones` (`Id`) ON DELETE CASCADE
) CHARACTER SET=utf8mb4;

CREATE INDEX `IX_EconomicZones_MorgueOfficeLocationId` ON `EconomicZones` (`MorgueOfficeLocationId`);

CREATE INDEX `IX_EconomicZones_MorgueStorageLocationId` ON `EconomicZones` (`MorgueStorageLocationId`);

CREATE INDEX `IX_CorpseRecoveryReports_AssignedPatrolId` ON `CorpseRecoveryReports` (`AssignedPatrolId`);

CREATE INDEX `IX_CorpseRecoveryReports_CorpseId` ON `CorpseRecoveryReports` (`CorpseId`);

CREATE INDEX `IX_CorpseRecoveryReports_DestinationCellId` ON `CorpseRecoveryReports` (`DestinationCellId`);

CREATE INDEX `IX_CorpseRecoveryReports_EconomicZoneId` ON `CorpseRecoveryReports` (`EconomicZoneId`);

CREATE INDEX `IX_CorpseRecoveryReports_LegalAuthorityId` ON `CorpseRecoveryReports` (`LegalAuthorityId`);

CREATE INDEX `IX_CorpseRecoveryReports_ReporterId` ON `CorpseRecoveryReports` (`ReporterId`);

CREATE INDEX `IX_CorpseRecoveryReports_SourceCellId` ON `CorpseRecoveryReports` (`SourceCellId`);

CREATE INDEX `IX_ProbateLocations_CellId` ON `ProbateLocations` (`CellId`);

ALTER TABLE `EconomicZones` ADD CONSTRAINT `FK_EconomicZones_MorgueOfficeLocations` FOREIGN KEY (`MorgueOfficeLocationId`) REFERENCES `Cells` (`Id`) ON DELETE SET NULL;

ALTER TABLE `EconomicZones` ADD CONSTRAINT `FK_EconomicZones_MorgueStorageLocations` FOREIGN KEY (`MorgueStorageLocationId`) REFERENCES `Cells` (`Id`) ON DELETE SET NULL;

INSERT INTO `__EFMigrationsHistory` (`MigrationId`, `ProductVersion`)
VALUES ('20260328123631_EstateProbateMorgueWorkflow', '9.0.11');

ALTER TABLE `EconomicZones` ADD `EstatesEnabled` bit(1) NOT NULL DEFAULT b'1';

INSERT INTO `__EFMigrationsHistory` (`MigrationId`, `ProductVersion`)
VALUES ('20260329110346_EconomicZoneEstatesEnabledToggle', '9.0.11');

ALTER TABLE `EstateAssets` ADD `OwnershipShare` decimal(58,29) NOT NULL DEFAULT 0.0;

CREATE TABLE `EstatePayouts` (
    `Id` bigint(20) NOT NULL AUTO_INCREMENT,
    `EstateId` bigint(20) NOT NULL,
    `RecipientId` bigint(20) NOT NULL,
    `RecipientType` varchar(100) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
    `Amount` decimal(58,29) NOT NULL,
    `Reason` text CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
    `CreatedDate` varchar(255) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
    `CollectedDate` varchar(255) CHARACTER SET utf8 COLLATE utf8_general_ci NULL,
    CONSTRAINT `PRIMARY` PRIMARY KEY (`Id`),
    CONSTRAINT `FK_EstatePayouts_Estates` FOREIGN KEY (`EstateId`) REFERENCES `Estates` (`Id`) ON DELETE CASCADE
) CHARACTER SET=utf8mb4;

CREATE INDEX `FK_EstatePayouts_Estates_idx` ON `EstatePayouts` (`EstateId`);

INSERT INTO `__EFMigrationsHistory` (`MigrationId`, `ProductVersion`)
VALUES ('20260329223130_EstateWillsPayoutsAndPropertyShares', '9.0.11');

CREATE TABLE `Materials_Aliases` (
    `MaterialId` bigint(20) NOT NULL,
    `Alias` varchar(255) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
    CONSTRAINT `PRIMARY` PRIMARY KEY (`MaterialId`, `Alias`),
    CONSTRAINT `Materials_Aliases_Materials` FOREIGN KEY (`MaterialId`) REFERENCES `Materials` (`Id`) ON DELETE CASCADE
) CHARACTER SET=utf8mb4;

CREATE UNIQUE INDEX `Materials_Aliases_Alias_UNIQUE` ON `Materials_Aliases` (`Alias`);

INSERT INTO `__EFMigrationsHistory` (`MigrationId`, `ProductVersion`)
VALUES ('20260331222122_AddSolidMaterialAliases', '9.0.11');

ALTER TABLE `Celestials` MODIFY COLUMN `CelestialType` varchar(30) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL DEFAULT 'Sun';

INSERT INTO `__EFMigrationsHistory` (`MigrationId`, `ProductVersion`)
VALUES ('20260402053811_RemoveOldSunCelestialDefault', '9.0.11');

COMMIT;

