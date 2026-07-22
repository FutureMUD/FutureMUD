-- MySqlBackup.NET 2.6.5.0
-- Dump Time: 2026-07-22 20:54:42
-- --------------------------------------
-- Server version 8.0.45 MySQL Community Server - GPL



DROP DATABASE `__FUTUREMUD_DATABASE__`;
CREATE DATABASE IF NOT EXISTS `__FUTUREMUD_DATABASE__` /*!40100 DEFAULT CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci */ /*!80016 DEFAULT ENCRYPTION='N' */;
USE `__FUTUREMUD_DATABASE__`;



/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!40101 SET NAMES utf8mb4 */;
/*!40103 SET @OLD_TIME_ZONE=@@TIME_ZONE */;
/*!40103 SET TIME_ZONE='+00:00' */;
/*!40014 SET @OLD_UNIQUE_CHECKS=@@UNIQUE_CHECKS, UNIQUE_CHECKS=0 */;
/*!40014 SET @OLD_FOREIGN_KEY_CHECKS=@@FOREIGN_KEY_CHECKS, FOREIGN_KEY_CHECKS=0 */;
/*!40101 SET @OLD_SQL_MODE=@@SQL_MODE, SQL_MODE='NO_AUTO_VALUE_ON_ZERO' */;
/*!40111 SET @OLD_SQL_NOTES=@@SQL_NOTES, SQL_NOTES=0 */;


--
-- Definition of __efmigrationshistory
--

DROP TABLE IF EXISTS `__efmigrationshistory`;
CREATE TABLE IF NOT EXISTS `__efmigrationshistory` (
  `MigrationId` varchar(150) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `ProductVersion` varchar(32) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  PRIMARY KEY (`MigrationId`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Dumping data for table __efmigrationshistory
--

/*!40000 ALTER TABLE `__efmigrationshistory` DISABLE KEYS */;
INSERT INTO `__efmigrationshistory`(`MigrationId`,`ProductVersion`) VALUES('20200626070704_InitialDatabase','9.0.11'),('20200728125151_MoveChargenToTables','9.0.11'),('20200807044450_EnforcementUpdate','9.0.11'),('20200810141606_ClanVoting','9.0.11'),('20200817061844_Elections','9.0.11'),('20200830233741_TerrainUpdate','9.0.11'),('20200905062837_CurrencyPatternEnhancement','9.0.11'),('20200928025908_KnowledgeBuilding','9.0.11'),('20201013213328_CheckFixing','9.0.11'),('20201014230837_FixingEmailTemplates','9.0.11'),('20201106014706_LineOfCreditAccounts','9.0.11'),('20201106040133_AttributesUpdate','9.0.11'),('20201108122141_EconomicZoneUpdate','9.0.11'),('20201113050353_EconomicZonesTouchup','9.0.11'),('20201120022913_EnforcermentAndMisc','9.0.11'),('20201120045951_MinorFixForCrime','9.0.11'),('20201129225407_SafeQuit','9.0.11'),('20201130014025_JournalUpdates','9.0.11'),('20201130041538_JournalUpdate','9.0.11'),('20201201052916_DrugReform','9.0.11'),('20201217051236_Changes','9.0.11'),('20201217051726_ExtraDescriptions','9.0.11'),('20201218014631_RacialBreathingChange','9.0.11'),('20201221031703_ClanFame','9.0.11'),('20201227120935_CantRemember','9.0.11'),('20210113052107_IndexFixForBodyparts','9.0.11'),('20210114010706_IndexAdditions','9.0.11'),('20210116210204_MagicSpells','9.0.11'),('20210118053537_MoreSpellStuff','9.0.11'),('20210119034150_MoreSpellStuff2','9.0.11'),('20210119035740_MoreSpellStuff3','9.0.11'),('20210120031933_MoreSpellStuff4','9.0.11'),('20210127032929_Jan21EnforcementWorkaround','9.0.11'),('20210202002906_RemovingChildClans','9.0.11'),('20210211035327_GameStatistics','9.0.11'),('20210224105856_NewSun','9.0.11'),('20210302112347_OngoingCheckForCharacteristics','9.0.11'),('20210331025006_BanksV1','9.0.11'),('20210423014825_WeaponAttackAddPositionRequirement','9.0.11'),('20210626110830_AuctionHouses','9.0.11'),('20210810123837_PropertyV1','9.0.11'),('20210902052233_PropertyV2','9.0.11'),('20210914132733_Sep21LawUpdate','9.0.11'),('20211025020630_JusticeOverhaulOct21','9.0.11'),('20211217034326_TerrainMapColourAddition','9.0.11'),('20211220045847_Skins','9.0.11'),('20211222033658_Skins-Pt2','9.0.11'),('20211226134159_ClanBankAccounts','9.0.11'),('20211229004501_PlayerBoards','9.0.11'),('20220104134109_JobsV1','9.0.11'),('20220105004035_ProjectsJobsUpdate','9.0.11'),('20220108004307_BoardBugFix','9.0.11'),('20220117102755_BoardsDescriptions','9.0.11'),('20220210215752_LongerAuthorFullDescs','9.0.11'),('20220225125641_ClanFKFixing','9.0.11'),('20220327052829_NPCSpawners','9.0.11'),('20220421132846_BodyCharacteristicsFix','9.0.11'),('20220625122517_ClanDiscordUpdate','9.0.11'),('20220625125136_ChargenResourcesAsDouble','9.0.11'),('20220718132632_MaterialsRefactor','9.0.11'),('20220731064708_TheoreticalCraftChecks','9.0.11'),('20220807101509_IntToDoubleParryBonus','9.0.11'),('20220814231930_RaceDefaultHwModels','9.0.11'),('20221030044209_ShopBankAccountsAndFinance','9.0.11'),('20221030125929_BankPaymentsAtShops','9.0.11'),('20221031113757_MagicSpellExclusivity','9.0.11'),('20221201081057_NameCulturesGenderExpansion','9.0.11'),('20221201133628_NameCulturesChargenExpansion','9.0.11'),('20230101133831_CurrencyPatternRegexCaseFix','9.0.11'),('20230110120837_RelativeEnthalpyForLiquids','9.0.11'),('20230124124618_SurgicalProcedureCheckTraits','9.0.11'),('20230311060208_SurgeryBodyUpdate','9.0.11'),('20230407151210_OpenAIv1','9.0.11'),('20230428004425_GlobalCurrencyChanges','9.0.11'),('20230603125906_CraftUseToolDuration','9.0.11'),('20230706055610_AuxiliaryMoves','9.0.11'),('20230714035824_AuxiliaryMoves2','9.0.11'),('20230727121209_SeasonsDisplayUpdate','9.0.11'),('20230731055842_SeederChoices','9.0.11'),('20230810071403_CellForeignKeyUpdates','9.0.11'),('20230825052231_NpcSpawnerMulti','9.0.11'),('20230825061651_ShopTypes','9.0.11'),('20230914142042_ScriptedEvents','9.0.11'),('20230917131132_ClanForeignKeyUpdate','9.0.11'),('20231031085439_MagicResourceColours','9.0.11'),('20231102120820_NewPlayerHints','9.0.11'),('20231110224309_HungerThirstRatesForRaces','9.0.11'),('20231125084220_ClimateModelSimplification','9.0.11'),('20231208235024_HeritageChargenCostBugFix','9.0.11'),('20240112055830_ChargenResourcesControlProg','9.0.11'),('20240119120217_CoinsChangeFlag','9.0.11'),('20240129025113_CurrencyForeignKeyUpdateJan24','9.0.11'),('20240305110906_BuyingMerchandise','9.0.11'),('20240325104238_MarketsV1','9.0.11'),('20240418112441_MarketsShopIntegration','9.0.11'),('20240427013621_MarketPopulations','9.0.11'),('20240601141550_DiscordOutputForChannels','9.0.11'),('20240615065145_ShopAutopayTaxes','9.0.11'),('20240730123726_TrackingV1','9.0.11'),('20240804070126_FixDatabaseAutoTrueBug','9.0.11'),('20240808232211_TrackingNameForBodyProtos','9.0.11'),('20240809155707_BMIUnits','9.0.11'),('20240816134208_ArmourPenaltyToDouble','9.0.11'),('20240817112644_HeightWeightModelDirectSetWeights','9.0.11'),('20240828105208_AutoReacquireTargetsSetting','9.0.11'),('20240828124859_CombatSettingsAugust24','9.0.11'),('20240831005804_BodypartGroupDescribersAugust2024','9.0.11'),('20240925062238_CrimesUpdate2024Sep25','9.0.11'),('20241011073405_StockroomNonMorphing','9.0.11'),('20241016054103_ItemProtoIsHiddenFromPlayers','9.0.11'),('20241016123415_BodyOverrideHealthStrategy','9.0.11'),('20241018071518_LiquidLeaveResidueInRooms','9.0.11'),('20241121010653_RemovingBreathableFluidsRaces','9.0.11'),('20241129002416_CriminalDescUpdates','9.0.11'),('20241216062012_RandomNamesBinaryUnicodeSort','9.0.11'),('20241220091815_EthnicitiesNameCultures','9.0.11'),('20241231030836_TagsForTerrains','9.0.11'),('20250101232454_SkewnessForHWModels','9.0.11'),('20250210095915_Shoppers','9.0.11'),('20250210233555_ShopsFeb25','9.0.11'),('20250211100238_ShopsFeb25P2','9.0.11'),('20250304042559_PreserveRegisterVariablesItemFlag','9.0.11'),('20250304104024_CraftPhaseExertionAndStamina','9.0.11'),('20250304114440_MerchandiseTransactionRecordsFix','9.0.11'),('20250424052852_SpellTriggerNullTargets','9.0.11'),('20250628230040_PropertyRekeyOnLeaseEnd','9.0.11'),('20251110082110_CombatArenaSchema','9.0.11'),('20251115120000_ArenaSignupEcho','9.0.11'),('20260211095519_AIStorytellers','9.0.11'),('20260211124139_AIStorytellerEventSubscriptions','9.0.11'),('20260216092441_AIStorytellerTimeSystemPrompt','9.0.11'),('20260216095426_AIStorytellerScopedModelReasoning','9.0.11'),('20260218120142_ArenaAutoScheduling','9.0.11'),('20260221112947_AIStorytellerSituationScopes','9.0.11'),('20260222081900_ArenaEventTypeEliminationModes','9.0.11'),('20260222112522_ArenaNpcCompletionRestore','9.0.11'),('20260225115630_ArenaStageNameProfile','9.0.11'),('20260225233442_ArenaEloStrategyOptions','9.0.11'),('20260226124500_ArenaSideRatingRanges','9.0.11'),('20260227120000_ArenaPhaseProgAppearancePayouts','9.0.11'),('20260308065322_ClimateDescriptions','9.0.11'),('20260309072751_WeatherModelSimplification','9.0.11'),('20260309222608_ReplaceCharacterCaloriesWithSatiationReserve','9.0.11'),('20260309225356_DropObsoleteNutritionCalories','9.0.11'),('20260310122815_InfectionVirulenceMultiplier','9.0.11'),('20260316112529_NaturalRangedAttacksAndElementalContact','9.0.11'),('20260320180000_CombatSettingPriorityAndRaceDefault','9.0.11'),('20260321102002_FutureProgTypeDefinitionsStage1','9.0.11'),('20260321102139_FutureProgTypeDefinitionsStage2','9.0.11'),('20260327103014_ShopDeals','9.0.11'),('20260327124234_EstateProbateAuctionLiquidation','9.0.11'),('20260328123631_EstateProbateMorgueWorkflow','9.0.11'),('20260329110346_EconomicZoneEstatesEnabledToggle','9.0.11'),('20260329223130_EstateWillsPayoutsAndPropertyShares','9.0.11'),('20260331222122_AddSolidMaterialAliases','9.0.11'),('20260402053811_RemoveOldSunCelestialDefault','9.0.11'),('20260415122407_MarketPopulationIncomeAndPricePressure','9.0.11'),('20260415225956_MarketCombinationCategoriesAndStressHysteresisReady','9.0.11'),('20260416225128_AddCharacterComputerWorkspace','9.0.11'),('20260418111319_AddComputerMailService','9.0.11'),('20260420105205_AddBodypartSeverFormula','9.0.11'),('20260421064024_ProjectQueueAndCancellationContinuity','9.0.11'),('20260422035105_Phase1MultiBodyForms','9.0.11'),('20260423090353_Phase15BodyFormProvisioning','9.0.11'),('20260423093000_CharacterBodyTraumaMode','9.0.11'),('20260423114308_Phase15FormTransformationEcho','9.0.11'),('20260424000000_RaceAttributeAlterations','9.0.11'),('20260424035904_RaceSatiationLimits','9.0.11'),('20260424040025_PlanesAndPlanarData','9.0.11'),('20260424044647_PlaneRoomPresentation','9.0.11'),('20260425131140_HotelRoomRentals','9.0.11'),('20260425131520_StableMountStabling','9.0.11'),('20260425132107_AddTerrainGravity','9.0.11'),('20260426114113_DefaultFormTransformationEchoNonSelf','9.0.11'),('20260427110022_RecurringIntervalOrdinalFields','9.0.11'),('20260501090000_PlaneRemoteObservationTag','9.0.11'),('20260501132243_RaceAgeColumnsNoDatabaseDefaults','9.0.11'),('20260506102652_PrintedWritingAuthorNullable','9.0.11'),('20260506103358_ClanBudgetsAndPayrollHistory','9.0.11'),('20260507091523_VirtualCashLedgerAndBanklessSettlement','9.0.11'),('20260507092824_ClanBudgetVirtualTreasuryFallback','9.0.11'),('20260510212610_PatrolRouteStrategyData','9.0.11'),('20260513234412_ManualCombatCommands','9.0.11'),('20260515130602_VehiclesHybridModel','9.0.11'),('20260515232922_VehicleSystemsPhase2','9.0.11'),('20260519132419_VehicleCharacterHitchPullMultiplier','9.0.11'),('20260520000533_VehiclePersistentHitchLinks','9.0.11'),('20260520011927_AgricultureSystem','9.0.11'),('20260523000000_CommoditySpoilageRules','9.0.11'),('20260523125349_ItemProtoUniqueNameBuilderNotes','9.0.11'),('20260523134149_NpcTemplateUniqueNameBuilderNotes','9.0.11'),('20260523205847_ForagableCommodityOutput','9.0.11'),('20260526091744_UnifiedEmploymentPersistence','9.0.11'),('20260526120456_SurfaceLiquidState','9.0.11'),('20260529001356_EmploymentPayrollLiabilities','9.0.11'),('20260529025718_EmploymentActionStepOperationalState','9.0.11'),('20260529230237_MagicPortalTopology','9.0.11'),('20260530235915_EmploymentScheduledRuleStatus','9.0.11'),('20260601012012_AutomaticCrimeContext','9.0.11'),('20260601111355_HotelNormalizedPersistence','9.0.11'),('20260601122909_CommodityMerchandiseWeightedSales','9.0.11'),('20260606072630_RichEmploymentScheduledRuleExpressions','9.0.11'),('20260606103736_OutfitTemplates','9.0.11'),('20260612134150_CharacterInstances','9.0.11'),('20260614233932_CharacterInstanceNpcPatrolStableInstances','9.0.11'),('20260615024353_CharacterInstanceActorReferences','9.0.11'),('20260615120000_CharacterInstanceProjectLabour','9.0.11'),('20260616135417_DrugExpansionDependenceExposures','9.0.11'),('20260620054424_EmploymentApplicationSnapshots','9.0.11'),('20260627000000_VehicleTowStressPolicy','9.0.11'),('20260701121756_AlertEmotes','9.0.11'),('20260701122720_ClanHallCellsForEmploymentHosts','9.0.11'),('20260703095705_WritingCollections','9.0.11'),('20260703125041_HospitalServicesAndEmploymentHosts','9.0.11'),('20260703143217_HospitalAnesthesiaBloodStockPolicies','9.0.11'),('20260703232840_HospitalCannulationAndCombinedServices','9.0.11'),('20260706141430_ActiveProjectPaymentSettings','9.0.11'),('20260708120000_HospitalClinicalPlanning','9.0.11'),('20260719104654_VehicleSurfaceWaterMovementProfiles','9.0.11'),('20260719124626_VehicleSurfaceWaterPropulsion','9.0.11'),('20260720063425_VehicleBoatCombat','9.0.11'),('20260721125028_MultiTargetCombatActions','9.0.11'),('20260722063400_RouteCellSpatialFoundation','9.0.11'),('20260722071041_RoomScaleVehicleInteriors','9.0.11'),('20260722100951_VehicleRoutesAndServices','9.0.11');
/*!40000 ALTER TABLE `__efmigrationshistory` ENABLE KEYS */;

--
-- Definition of agriculturecropdefinitions
--

DROP TABLE IF EXISTS `agriculturecropdefinitions`;
CREATE TABLE IF NOT EXISTS `agriculturecropdefinitions` (
  `Id` bigint NOT NULL AUTO_INCREMENT,
  `Name` varchar(200) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `Description` mediumtext CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `Category` varchar(100) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `Definition` mediumtext CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Dumping data for table agriculturecropdefinitions
--

/*!40000 ALTER TABLE `agriculturecropdefinitions` DISABLE KEYS */;
/*!40000 ALTER TABLE `agriculturecropdefinitions` ENABLE KEYS */;

--
-- Definition of agriculturefieldprofiles
--

DROP TABLE IF EXISTS `agriculturefieldprofiles`;
CREATE TABLE IF NOT EXISTS `agriculturefieldprofiles` (
  `Id` bigint NOT NULL AUTO_INCREMENT,
  `Name` varchar(200) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `Description` mediumtext CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `Definition` mediumtext CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Dumping data for table agriculturefieldprofiles
--

/*!40000 ALTER TABLE `agriculturefieldprofiles` DISABLE KEYS */;
/*!40000 ALTER TABLE `agriculturefieldprofiles` ENABLE KEYS */;

--
-- Definition of agriculturefields
--

DROP TABLE IF EXISTS `agriculturefields`;
CREATE TABLE IF NOT EXISTS `agriculturefields` (
  `Id` bigint NOT NULL AUTO_INCREMENT,
  `CellId` bigint NOT NULL,
  `ProfileId` bigint NOT NULL,
  `CurrentUse` int NOT NULL,
  `Moisture` int NOT NULL,
  `Drainage` int NOT NULL,
  `Nutrients` int NOT NULL,
  `Salinity` int NOT NULL,
  `Topsoil` int NOT NULL,
  `Tilth` int NOT NULL,
  `Rockiness` int NOT NULL,
  `Weeds` int NOT NULL,
  `Pests` int NOT NULL,
  `Fence` int NOT NULL,
  `Pasture` int NOT NULL,
  `Condition` int NOT NULL,
  `LastTickMudDateTime` varchar(500) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci DEFAULT NULL,
  `Definition` mediumtext CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  PRIMARY KEY (`Id`),
  UNIQUE KEY `IX_AgricultureFields_CellId` (`CellId`),
  KEY `FK_AgricultureFields_Profiles_idx` (`ProfileId`),
  CONSTRAINT `FK_AgricultureFields_Cells` FOREIGN KEY (`CellId`) REFERENCES `cells` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_AgricultureFields_Profiles` FOREIGN KEY (`ProfileId`) REFERENCES `agriculturefieldprofiles` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Dumping data for table agriculturefields
--

/*!40000 ALTER TABLE `agriculturefields` DISABLE KEYS */;
/*!40000 ALTER TABLE `agriculturefields` ENABLE KEYS */;

--
-- Definition of agriculturefieldcrops
--

DROP TABLE IF EXISTS `agriculturefieldcrops`;
CREATE TABLE IF NOT EXISTS `agriculturefieldcrops` (
  `AgricultureFieldId` bigint NOT NULL,
  `CropDefinitionId` bigint NOT NULL,
  `Stage` int NOT NULL,
  `GrowthDays` int NOT NULL,
  `Health` int NOT NULL,
  `YieldPotential` int NOT NULL,
  `PlantedMudDateTime` varchar(500) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci DEFAULT NULL,
  `Definition` mediumtext CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  PRIMARY KEY (`AgricultureFieldId`),
  KEY `FK_AgricultureFieldCrops_Crops_idx` (`CropDefinitionId`),
  CONSTRAINT `FK_AgricultureFieldCrops_Crops` FOREIGN KEY (`CropDefinitionId`) REFERENCES `agriculturecropdefinitions` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_AgricultureFieldCrops_Fields` FOREIGN KEY (`AgricultureFieldId`) REFERENCES `agriculturefields` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Dumping data for table agriculturefieldcrops
--

/*!40000 ALTER TABLE `agriculturefieldcrops` DISABLE KEYS */;
/*!40000 ALTER TABLE `agriculturefieldcrops` ENABLE KEYS */;

--
-- Definition of agriculturewoodlanddefinitions
--

DROP TABLE IF EXISTS `agriculturewoodlanddefinitions`;
CREATE TABLE IF NOT EXISTS `agriculturewoodlanddefinitions` (
  `Id` bigint NOT NULL AUTO_INCREMENT,
  `Name` varchar(200) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `Description` mediumtext CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `WoodlandType` varchar(100) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `Definition` mediumtext CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Dumping data for table agriculturewoodlanddefinitions
--

/*!40000 ALTER TABLE `agriculturewoodlanddefinitions` DISABLE KEYS */;
/*!40000 ALTER TABLE `agriculturewoodlanddefinitions` ENABLE KEYS */;

--
-- Definition of agriculturefieldwoodlands
--

DROP TABLE IF EXISTS `agriculturefieldwoodlands`;
CREATE TABLE IF NOT EXISTS `agriculturefieldwoodlands` (
  `AgricultureFieldId` bigint NOT NULL,
  `WoodlandDefinitionId` bigint NOT NULL,
  `GrowthDays` int NOT NULL,
  `Health` int NOT NULL,
  `YieldPotential` int NOT NULL,
  `PlantedMudDateTime` varchar(500) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci DEFAULT NULL,
  `Definition` mediumtext CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  PRIMARY KEY (`AgricultureFieldId`),
  KEY `FK_AgricultureFieldWoodlands_Woodlands_idx` (`WoodlandDefinitionId`),
  CONSTRAINT `FK_AgricultureFieldWoodlands_Fields` FOREIGN KEY (`AgricultureFieldId`) REFERENCES `agriculturefields` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_AgricultureFieldWoodlands_Woodlands` FOREIGN KEY (`WoodlandDefinitionId`) REFERENCES `agriculturewoodlanddefinitions` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Dumping data for table agriculturefieldwoodlands
--

/*!40000 ALTER TABLE `agriculturefieldwoodlands` DISABLE KEYS */;
/*!40000 ALTER TABLE `agriculturefieldwoodlands` ENABLE KEYS */;

--
-- Definition of aistorytellerreferencedocuments
--

DROP TABLE IF EXISTS `aistorytellerreferencedocuments`;
CREATE TABLE IF NOT EXISTS `aistorytellerreferencedocuments` (
  `Id` bigint NOT NULL AUTO_INCREMENT,
  `Name` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  `Description` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  `FolderName` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  `DocumentType` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  `Keywords` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  `DocumentContents` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  `RestrictedStorytellerIds` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Dumping data for table aistorytellerreferencedocuments
--

/*!40000 ALTER TABLE `aistorytellerreferencedocuments` DISABLE KEYS */;
/*!40000 ALTER TABLE `aistorytellerreferencedocuments` ENABLE KEYS */;

--
-- Definition of armourtypes
--

DROP TABLE IF EXISTS `armourtypes`;
CREATE TABLE IF NOT EXISTS `armourtypes` (
  `Id` bigint NOT NULL AUTO_INCREMENT,
  `Name` varchar(200) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL,
  `MinimumPenetrationDegree` int NOT NULL,
  `BaseDifficultyDegrees` double NOT NULL,
  `StackedDifficultyDegrees` double NOT NULL,
  `Definition` text CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Dumping data for table armourtypes
--

/*!40000 ALTER TABLE `armourtypes` DISABLE KEYS */;
/*!40000 ALTER TABLE `armourtypes` ENABLE KEYS */;

--
-- Definition of artificialintelligences
--

DROP TABLE IF EXISTS `artificialintelligences`;
CREATE TABLE IF NOT EXISTS `artificialintelligences` (
  `Id` bigint NOT NULL AUTO_INCREMENT,
  `Name` varchar(4000) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `Type` varchar(4000) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `Definition` mediumtext CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Dumping data for table artificialintelligences
--

/*!40000 ALTER TABLE `artificialintelligences` DISABLE KEYS */;
/*!40000 ALTER TABLE `artificialintelligences` ENABLE KEYS */;

--
-- Definition of authoritygroups
--

DROP TABLE IF EXISTS `authoritygroups`;
CREATE TABLE IF NOT EXISTS `authoritygroups` (
  `Id` bigint NOT NULL AUTO_INCREMENT,
  `Name` varchar(4000) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `AuthorityLevel` int NOT NULL,
  `InformationLevel` int NOT NULL,
  `AccountsLevel` int NOT NULL,
  `CharactersLevel` int NOT NULL,
  `CharacterApprovalLevel` int NOT NULL,
  `CharacterApprovalRisk` int NOT NULL,
  `ItemsLevel` int NOT NULL,
  `PlanesLevel` int NOT NULL,
  `RoomsLevel` int NOT NULL,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Dumping data for table authoritygroups
--

/*!40000 ALTER TABLE `authoritygroups` DISABLE KEYS */;
/*!40000 ALTER TABLE `authoritygroups` ENABLE KEYS */;

--
-- Definition of accounts
--

DROP TABLE IF EXISTS `accounts`;
CREATE TABLE IF NOT EXISTS `accounts` (
  `Id` bigint NOT NULL AUTO_INCREMENT,
  `Name` varchar(4000) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `Password` varchar(1000) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `Salt` bigint NOT NULL,
  `AccessStatus` int NOT NULL,
  `AuthorityGroupId` bigint DEFAULT '0',
  `Email` varchar(4000) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci DEFAULT NULL,
  `LastLoginTime` datetime DEFAULT NULL,
  `LastLoginIP` varchar(50) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci DEFAULT NULL,
  `FormatLength` int NOT NULL DEFAULT '110',
  `InnerFormatLength` int NOT NULL DEFAULT '80',
  `UseMXP` bit(1) NOT NULL DEFAULT b'0',
  `UseMSP` bit(1) NOT NULL DEFAULT b'0',
  `UseMCCP` bit(1) NOT NULL DEFAULT b'0',
  `ActiveCharactersAllowed` int NOT NULL DEFAULT '1',
  `UseUnicode` bit(1) NOT NULL DEFAULT b'0',
  `TimeZoneId` varchar(100) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `CultureName` varchar(50) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `RegistrationCode` varchar(50) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci DEFAULT NULL,
  `IsRegistered` bit(1) NOT NULL DEFAULT b'0',
  `RecoveryCode` varchar(4000) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci DEFAULT NULL,
  `UnitPreference` varchar(50) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `CreationDate` datetime NOT NULL,
  `PageLength` int NOT NULL DEFAULT '22',
  `PromptType` int NOT NULL,
  `TabRoomDescriptions` bit(1) NOT NULL DEFAULT b'1',
  `CodedRoomDescriptionAdditionsOnNewLine` bit(1) NOT NULL DEFAULT b'1',
  `CharacterNameOverlaySetting` int NOT NULL,
  `AppendNewlinesBetweenMultipleEchoesPerPrompt` bit(1) NOT NULL DEFAULT b'1',
  `ActLawfully` bit(1) NOT NULL DEFAULT b'1',
  `HasBeenActiveInWeek` bit(1) NOT NULL DEFAULT b'0',
  `HintsEnabled` bit(1) NOT NULL DEFAULT b'1',
  `AutoReacquireTargets` bit(1) NOT NULL DEFAULT b'1',
  PRIMARY KEY (`Id`),
  KEY `FK_Accounts_AuthorityGroups` (`AuthorityGroupId`),
  CONSTRAINT `FK_Accounts_AuthorityGroups` FOREIGN KEY (`AuthorityGroupId`) REFERENCES `authoritygroups` (`Id`) ON DELETE RESTRICT
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Dumping data for table accounts
--

/*!40000 ALTER TABLE `accounts` DISABLE KEYS */;
/*!40000 ALTER TABLE `accounts` ENABLE KEYS */;

--
-- Definition of autobuilderareatemplates
--

DROP TABLE IF EXISTS `autobuilderareatemplates`;
CREATE TABLE IF NOT EXISTS `autobuilderareatemplates` (
  `ID` bigint NOT NULL AUTO_INCREMENT,
  `Name` varchar(100) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `TemplateType` varchar(45) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `Definition` longtext CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  PRIMARY KEY (`ID`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Dumping data for table autobuilderareatemplates
--

/*!40000 ALTER TABLE `autobuilderareatemplates` DISABLE KEYS */;
/*!40000 ALTER TABLE `autobuilderareatemplates` ENABLE KEYS */;

--
-- Definition of autobuilderroomtemplates
--

DROP TABLE IF EXISTS `autobuilderroomtemplates`;
CREATE TABLE IF NOT EXISTS `autobuilderroomtemplates` (
  `ID` bigint NOT NULL AUTO_INCREMENT,
  `Name` varchar(100) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `TemplateType` varchar(45) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `Definition` longtext CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  PRIMARY KEY (`ID`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Dumping data for table autobuilderroomtemplates
--

/*!40000 ALTER TABLE `autobuilderroomtemplates` DISABLE KEYS */;
/*!40000 ALTER TABLE `autobuilderroomtemplates` ENABLE KEYS */;

--
-- Definition of bans
--

DROP TABLE IF EXISTS `bans`;
CREATE TABLE IF NOT EXISTS `bans` (
  `Id` bigint NOT NULL AUTO_INCREMENT,
  `IpMask` varchar(200) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `BannerAccountId` bigint DEFAULT NULL,
  `Reason` varchar(4000) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `Expiry` datetime DEFAULT NULL,
  PRIMARY KEY (`Id`),
  KEY `FK_Bans_Accounts` (`BannerAccountId`),
  CONSTRAINT `FK_Bans_Accounts` FOREIGN KEY (`BannerAccountId`) REFERENCES `accounts` (`Id`) ON DELETE SET NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Dumping data for table bans
--

/*!40000 ALTER TABLE `bans` DISABLE KEYS */;
/*!40000 ALTER TABLE `bans` ENABLE KEYS */;

--
-- Definition of bloodmodels
--

DROP TABLE IF EXISTS `bloodmodels`;
CREATE TABLE IF NOT EXISTS `bloodmodels` (
  `Id` bigint NOT NULL AUTO_INCREMENT,
  `Name` varchar(200) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Dumping data for table bloodmodels
--

/*!40000 ALTER TABLE `bloodmodels` DISABLE KEYS */;
/*!40000 ALTER TABLE `bloodmodels` ENABLE KEYS */;

--
-- Definition of bloodtypeantigens
--

DROP TABLE IF EXISTS `bloodtypeantigens`;
CREATE TABLE IF NOT EXISTS `bloodtypeantigens` (
  `Id` bigint NOT NULL AUTO_INCREMENT,
  `Name` varchar(200) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Dumping data for table bloodtypeantigens
--

/*!40000 ALTER TABLE `bloodtypeantigens` DISABLE KEYS */;
/*!40000 ALTER TABLE `bloodtypeantigens` ENABLE KEYS */;

--
-- Definition of bloodtypes
--

DROP TABLE IF EXISTS `bloodtypes`;
CREATE TABLE IF NOT EXISTS `bloodtypes` (
  `Id` bigint NOT NULL AUTO_INCREMENT,
  `Name` varchar(200) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Dumping data for table bloodtypes
--

/*!40000 ALTER TABLE `bloodtypes` DISABLE KEYS */;
/*!40000 ALTER TABLE `bloodtypes` ENABLE KEYS */;

--
-- Definition of bloodmodels_bloodtypes
--

DROP TABLE IF EXISTS `bloodmodels_bloodtypes`;
CREATE TABLE IF NOT EXISTS `bloodmodels_bloodtypes` (
  `BloodModelId` bigint NOT NULL,
  `BloodtypeId` bigint NOT NULL,
  PRIMARY KEY (`BloodModelId`,`BloodtypeId`),
  KEY `FK_BloodModels_Bloodtypes_Bloodtypes_idx` (`BloodtypeId`),
  CONSTRAINT `FK_BloodModels_Bloodtypes_BloodModels` FOREIGN KEY (`BloodModelId`) REFERENCES `bloodmodels` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_BloodModels_Bloodtypes_Bloodtypes` FOREIGN KEY (`BloodtypeId`) REFERENCES `bloodtypes` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Dumping data for table bloodmodels_bloodtypes
--

/*!40000 ALTER TABLE `bloodmodels_bloodtypes` DISABLE KEYS */;
/*!40000 ALTER TABLE `bloodmodels_bloodtypes` ENABLE KEYS */;

--
-- Definition of bloodtypes_bloodtypeantigens
--

DROP TABLE IF EXISTS `bloodtypes_bloodtypeantigens`;
CREATE TABLE IF NOT EXISTS `bloodtypes_bloodtypeantigens` (
  `BloodtypeId` bigint NOT NULL,
  `BloodtypeAntigenId` bigint NOT NULL,
  PRIMARY KEY (`BloodtypeId`,`BloodtypeAntigenId`),
  KEY `FK_Bloodtypes_BloodtypeAntigens_BloodtypeAntigens_idx` (`BloodtypeAntigenId`),
  CONSTRAINT `FK_Bloodtypes_BloodtypeAntigens_BloodtypeAntigens` FOREIGN KEY (`BloodtypeAntigenId`) REFERENCES `bloodtypeantigens` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_Bloodtypes_BloodtypeAntigens_Bloodtypes` FOREIGN KEY (`BloodtypeId`) REFERENCES `bloodtypes` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Dumping data for table bloodtypes_bloodtypeantigens
--

/*!40000 ALTER TABLE `bloodtypes_bloodtypeantigens` DISABLE KEYS */;
/*!40000 ALTER TABLE `bloodtypes_bloodtypeantigens` ENABLE KEYS */;

--
-- Definition of bodypartshape
--

DROP TABLE IF EXISTS `bodypartshape`;
CREATE TABLE IF NOT EXISTS `bodypartshape` (
  `Id` bigint NOT NULL AUTO_INCREMENT,
  `Name` varchar(4000) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Dumping data for table bodypartshape
--

/*!40000 ALTER TABLE `bodypartshape` DISABLE KEYS */;
/*!40000 ALTER TABLE `bodypartshape` ENABLE KEYS */;

--
-- Definition of bodypartgroupdescribers_shapecount
--

DROP TABLE IF EXISTS `bodypartgroupdescribers_shapecount`;
CREATE TABLE IF NOT EXISTS `bodypartgroupdescribers_shapecount` (
  `BodypartGroupDescriptionRuleId` bigint NOT NULL,
  `TargetId` bigint NOT NULL,
  `MinCount` int NOT NULL,
  `MaxCount` int NOT NULL,
  PRIMARY KEY (`BodypartGroupDescriptionRuleId`,`TargetId`),
  KEY `FK_BGD_ShapeCount_BodypartShape` (`TargetId`),
  CONSTRAINT `FK_BGD_ShapeCount_BodypartGroupDescribers` FOREIGN KEY (`BodypartGroupDescriptionRuleId`) REFERENCES `bodypartgroupdescribers` (`Id`) ON DELETE RESTRICT,
  CONSTRAINT `FK_BGD_ShapeCount_BodypartShape` FOREIGN KEY (`TargetId`) REFERENCES `bodypartshape` (`Id`) ON DELETE RESTRICT
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Dumping data for table bodypartgroupdescribers_shapecount
--

/*!40000 ALTER TABLE `bodypartgroupdescribers_shapecount` DISABLE KEYS */;
/*!40000 ALTER TABLE `bodypartgroupdescribers_shapecount` ENABLE KEYS */;

--
-- Definition of bodypartshapecountview
--

DROP TABLE IF EXISTS `bodypartshapecountview`;
CREATE TABLE IF NOT EXISTS `bodypartshapecountview` (
  `BodypartGroupDescriptionRuleId` tinyint NOT NULL,
  `DescribedAs` tinyint NOT NULL,
  `MinCount` tinyint NOT NULL,
  `MaxCount` tinyint NOT NULL,
  `TargetId` tinyint NOT NULL,
  `Name` tinyint NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Dumping data for table bodypartshapecountview
--

/*!40000 ALTER TABLE `bodypartshapecountview` DISABLE KEYS */;
/*!40000 ALTER TABLE `bodypartshapecountview` ENABLE KEYS */;

--
-- Definition of calendars
--

DROP TABLE IF EXISTS `calendars`;
CREATE TABLE IF NOT EXISTS `calendars` (
  `Id` bigint NOT NULL AUTO_INCREMENT,
  `Definition` text CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `Date` varchar(50) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `FeedClockId` bigint NOT NULL,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Dumping data for table calendars
--

/*!40000 ALTER TABLE `calendars` DISABLE KEYS */;
/*!40000 ALTER TABLE `calendars` ENABLE KEYS */;

--
-- Definition of boards
--

DROP TABLE IF EXISTS `boards`;
CREATE TABLE IF NOT EXISTS `boards` (
  `Id` bigint NOT NULL AUTO_INCREMENT,
  `Name` varchar(45) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `ShowOnLogin` bit(1) NOT NULL,
  `CalendarId` bigint DEFAULT NULL,
  PRIMARY KEY (`Id`),
  KEY `IX_Boards_CalendarId` (`CalendarId`),
  CONSTRAINT `FK_Boards_Calendars` FOREIGN KEY (`CalendarId`) REFERENCES `calendars` (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Dumping data for table boards
--

/*!40000 ALTER TABLE `boards` DISABLE KEYS */;
/*!40000 ALTER TABLE `boards` ENABLE KEYS */;

--
-- Definition of boardposts
--

DROP TABLE IF EXISTS `boardposts`;
CREATE TABLE IF NOT EXISTS `boardposts` (
  `Id` bigint NOT NULL AUTO_INCREMENT,
  `BoardId` bigint NOT NULL,
  `Title` varchar(200) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `Content` text CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `AuthorId` bigint DEFAULT NULL,
  `PostTime` datetime NOT NULL,
  `AuthorIsCharacter` bit(1) NOT NULL DEFAULT b'0',
  `InGameDateTime` varchar(200) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci DEFAULT NULL,
  `AuthorFullDescription` varchar(8000) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci DEFAULT NULL,
  `AuthorName` varchar(200) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci DEFAULT NULL,
  `AuthorShortDescription` varchar(200) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci DEFAULT NULL,
  PRIMARY KEY (`Id`),
  KEY `FK_BoardsPosts_Accounts_idx` (`AuthorId`),
  KEY `FK_BoardPosts_Boards_idx` (`BoardId`),
  CONSTRAINT `FK_BoardPosts_Boards` FOREIGN KEY (`BoardId`) REFERENCES `boards` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Dumping data for table boardposts
--

/*!40000 ALTER TABLE `boardposts` DISABLE KEYS */;
/*!40000 ALTER TABLE `boardposts` ENABLE KEYS */;

--
-- Definition of celestials
--

DROP TABLE IF EXISTS `celestials`;
CREATE TABLE IF NOT EXISTS `celestials` (
  `Id` bigint NOT NULL AUTO_INCREMENT,
  `Definition` text CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `Minutes` int NOT NULL,
  `FeedClockId` bigint NOT NULL,
  `CelestialYear` int NOT NULL,
  `LastYearBump` int NOT NULL,
  `CelestialType` varchar(30) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL DEFAULT 'Sun',
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Dumping data for table celestials
--

/*!40000 ALTER TABLE `celestials` DISABLE KEYS */;
/*!40000 ALTER TABLE `celestials` ENABLE KEYS */;

--
-- Definition of characterinstances
--

DROP TABLE IF EXISTS `characterinstances`;
CREATE TABLE IF NOT EXISTS `characterinstances` (
  `Id` bigint NOT NULL AUTO_INCREMENT,
  `CharacterId` bigint NOT NULL,
  `BodyId` bigint NOT NULL,
  `InstanceName` varchar(100) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci DEFAULT NULL,
  `InstanceKind` int NOT NULL,
  `ControlPolicy` int NOT NULL,
  `DeathPolicy` int NOT NULL,
  `PerceptionPolicy` int NOT NULL,
  `PersistencePolicy` int NOT NULL,
  `LocationId` bigint DEFAULT NULL,
  `RoomLayer` int NOT NULL,
  `PositionId` int NOT NULL,
  `PositionModifier` int NOT NULL,
  `PositionTargetId` bigint DEFAULT NULL,
  `PositionTargetType` varchar(50) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci DEFAULT NULL,
  `PositionEmote` text CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci,
  `State` int NOT NULL,
  `Status` int NOT NULL,
  `IsPrimary` bit(1) NOT NULL DEFAULT b'0',
  `IsEmbodied` bit(1) NOT NULL DEFAULT b'1',
  `IsControllable` bit(1) NOT NULL DEFAULT b'1',
  `PrimaryCharacterId` bigint GENERATED ALWAYS AS ((case when (`IsPrimary` = 0x01) then `CharacterId` else NULL end)) STORED,
  `EmbodiedBodyId` bigint GENERATED ALWAYS AS ((case when (`IsEmbodied` = 0x01) then `BodyId` else NULL end)) STORED,
  `AnchorInstanceId` bigint DEFAULT NULL,
  `CreatedBySourceType` int DEFAULT NULL,
  `CreatedBySourceId` bigint DEFAULT NULL,
  `CreatedBySourceKey` varchar(200) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci DEFAULT NULL,
  `CreatedDateTime` datetime NOT NULL,
  `ExpiryDateTime` datetime DEFAULT NULL,
  `EffectData` mediumtext CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `CurrentProjectId` bigint DEFAULT NULL,
  `CurrentProjectLabourId` bigint DEFAULT NULL,
  `CurrentProjectHours` double NOT NULL DEFAULT '0',
  `CurrentProjectProjectHours` double NOT NULL DEFAULT '0',
  `RoutePosition` decimal(18,3) DEFAULT NULL,
  PRIMARY KEY (`Id`),
  UNIQUE KEY `UQ_CharacterInstances_EmbodiedBody` (`EmbodiedBodyId`),
  UNIQUE KEY `UQ_CharacterInstances_PrimaryCharacter` (`PrimaryCharacterId`),
  KEY `FK_CharacterInstances_AnchorInstance_idx` (`AnchorInstanceId`),
  KEY `FK_CharacterInstances_Bodies_idx` (`BodyId`),
  KEY `FK_CharacterInstances_Cells_idx` (`LocationId`),
  KEY `FK_CharacterInstances_Characters_idx` (`CharacterId`),
  KEY `IX_CharacterInstances_Location_Layer` (`LocationId`,`RoomLayer`),
  KEY `FK_CharacterInstances_ActiveProjects_idx` (`CurrentProjectId`),
  KEY `FK_CharacterInstances_ProjectLabourRequirements_idx` (`CurrentProjectLabourId`),
  KEY `IX_CharacterInstances_Location_Layer_RoutePosition` (`LocationId`,`RoomLayer`,`RoutePosition`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Dumping data for table characterinstances
--

/*!40000 ALTER TABLE `characterinstances` DISABLE KEYS */;
/*!40000 ALTER TABLE `characterinstances` ENABLE KEYS */;

--
-- Definition of characteristicdefinitions
--

DROP TABLE IF EXISTS `characteristicdefinitions`;
CREATE TABLE IF NOT EXISTS `characteristicdefinitions` (
  `Id` bigint NOT NULL AUTO_INCREMENT,
  `Name` varchar(4000) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `Type` int NOT NULL,
  `Pattern` varchar(4000) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `Description` varchar(4000) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `ParentId` bigint DEFAULT NULL,
  `ChargenDisplayType` int DEFAULT NULL,
  `Model` varchar(45) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL DEFAULT 'standard',
  `Definition` text CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci,
  PRIMARY KEY (`Id`),
  KEY `FK_CharacteristicDefinitions_Parent` (`ParentId`),
  CONSTRAINT `FK_CharacteristicDefinitions_Parent` FOREIGN KEY (`ParentId`) REFERENCES `characteristicdefinitions` (`Id`) ON DELETE RESTRICT
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Dumping data for table characteristicdefinitions
--

/*!40000 ALTER TABLE `characteristicdefinitions` DISABLE KEYS */;
/*!40000 ALTER TABLE `characteristicdefinitions` ENABLE KEYS */;

--
-- Definition of characteristicprofiles
--

DROP TABLE IF EXISTS `characteristicprofiles`;
CREATE TABLE IF NOT EXISTS `characteristicprofiles` (
  `Id` bigint NOT NULL AUTO_INCREMENT,
  `Name` varchar(50) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `Definition` text CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `Type` varchar(50) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `TargetDefinitionId` bigint NOT NULL,
  `Description` varchar(4000) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  PRIMARY KEY (`Id`),
  KEY `FK_CharacteristicProfiles_CharacteristicDefinitions` (`TargetDefinitionId`),
  CONSTRAINT `FK_CharacteristicProfiles_CharacteristicDefinitions` FOREIGN KEY (`TargetDefinitionId`) REFERENCES `characteristicdefinitions` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Dumping data for table characteristicprofiles
--

/*!40000 ALTER TABLE `characteristicprofiles` DISABLE KEYS */;
/*!40000 ALTER TABLE `characteristicprofiles` ENABLE KEYS */;

--
-- Definition of chargens
--

DROP TABLE IF EXISTS `chargens`;
CREATE TABLE IF NOT EXISTS `chargens` (
  `Id` bigint NOT NULL AUTO_INCREMENT,
  `AccountId` bigint NOT NULL,
  `Name` varchar(12000) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `Definition` text CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `Status` int NOT NULL,
  `SubmitTime` datetime DEFAULT NULL,
  `MinimumApprovalAuthority` int DEFAULT NULL,
  `ApprovedById` bigint DEFAULT NULL,
  `ApprovalTime` datetime DEFAULT NULL,
  PRIMARY KEY (`Id`),
  KEY `FK_Chargens_Accounts` (`AccountId`),
  CONSTRAINT `FK_Chargens_Accounts` FOREIGN KEY (`AccountId`) REFERENCES `accounts` (`Id`) ON DELETE RESTRICT
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Dumping data for table chargens
--

/*!40000 ALTER TABLE `chargens` DISABLE KEYS */;
/*!40000 ALTER TABLE `chargens` ENABLE KEYS */;

--
-- Definition of chargenscreenstoryboards
--

DROP TABLE IF EXISTS `chargenscreenstoryboards`;
CREATE TABLE IF NOT EXISTS `chargenscreenstoryboards` (
  `Id` bigint NOT NULL AUTO_INCREMENT,
  `ChargenType` varchar(50) DEFAULT NULL,
  `ChargenStage` int NOT NULL,
  `Order` int NOT NULL,
  `StageDefinition` mediumtext CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `NextStage` int NOT NULL,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Dumping data for table chargenscreenstoryboards
--

/*!40000 ALTER TABLE `chargenscreenstoryboards` DISABLE KEYS */;
/*!40000 ALTER TABLE `chargenscreenstoryboards` ENABLE KEYS */;

--
-- Definition of chargenscreenstoryboarddependentstages
--

DROP TABLE IF EXISTS `chargenscreenstoryboarddependentstages`;
CREATE TABLE IF NOT EXISTS `chargenscreenstoryboarddependentstages` (
  `OwnerId` bigint NOT NULL,
  `Dependency` int NOT NULL,
  PRIMARY KEY (`OwnerId`,`Dependency`),
  KEY `FK_ChargenScreenStoryboardDependentStages_Owner` (`OwnerId`),
  CONSTRAINT `FK_ChargenScreenStoryboardDependentStages_Owner` FOREIGN KEY (`OwnerId`) REFERENCES `chargenscreenstoryboards` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Dumping data for table chargenscreenstoryboarddependentstages
--

/*!40000 ALTER TABLE `chargenscreenstoryboarddependentstages` DISABLE KEYS */;
/*!40000 ALTER TABLE `chargenscreenstoryboarddependentstages` ENABLE KEYS */;

--
-- Definition of checktemplates
--

DROP TABLE IF EXISTS `checktemplates`;
CREATE TABLE IF NOT EXISTS `checktemplates` (
  `Id` bigint NOT NULL AUTO_INCREMENT,
  `Name` varchar(4000) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `Definition` text CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci,
  `CheckMethod` varchar(25) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL DEFAULT 'Standard',
  `ImproveTraits` bit(1) NOT NULL DEFAULT b'0',
  `FailIfTraitMissingMode` smallint NOT NULL,
  `CanBranchIfTraitMissing` bit(1) NOT NULL DEFAULT b'0',
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Dumping data for table checktemplates
--

/*!40000 ALTER TABLE `checktemplates` DISABLE KEYS */;
/*!40000 ALTER TABLE `checktemplates` ENABLE KEYS */;

--
-- Definition of checktemplatedifficulties
--

DROP TABLE IF EXISTS `checktemplatedifficulties`;
CREATE TABLE IF NOT EXISTS `checktemplatedifficulties` (
  `CheckTemplateId` bigint NOT NULL,
  `Difficulty` int NOT NULL,
  `Modifier` double NOT NULL,
  PRIMARY KEY (`Difficulty`,`CheckTemplateId`),
  KEY `FK_CheckTemplateDifficulties_CheckTemplates` (`CheckTemplateId`),
  CONSTRAINT `FK_CheckTemplateDifficulties_CheckTemplates` FOREIGN KEY (`CheckTemplateId`) REFERENCES `checktemplates` (`Id`) ON DELETE RESTRICT
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Dumping data for table checktemplatedifficulties
--

/*!40000 ALTER TABLE `checktemplatedifficulties` DISABLE KEYS */;
/*!40000 ALTER TABLE `checktemplatedifficulties` ENABLE KEYS */;

--
-- Definition of climatemodels
--

DROP TABLE IF EXISTS `climatemodels`;
CREATE TABLE IF NOT EXISTS `climatemodels` (
  `Id` bigint NOT NULL AUTO_INCREMENT,
  `Name` varchar(200) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `Type` varchar(45) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `MinuteProcessingInterval` int NOT NULL,
  `MinimumMinutesBetweenFlavourEchoes` int NOT NULL,
  `MinuteFlavourEchoChance` double NOT NULL,
  `Description` text CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Dumping data for table climatemodels
--

/*!40000 ALTER TABLE `climatemodels` DISABLE KEYS */;
/*!40000 ALTER TABLE `climatemodels` ENABLE KEYS */;

--
-- Definition of clocks
--

DROP TABLE IF EXISTS `clocks`;
CREATE TABLE IF NOT EXISTS `clocks` (
  `Id` bigint NOT NULL AUTO_INCREMENT,
  `Definition` text CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `Seconds` int NOT NULL,
  `Minutes` int NOT NULL,
  `Hours` int NOT NULL,
  `PrimaryTimezoneId` bigint NOT NULL,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Dumping data for table clocks
--

/*!40000 ALTER TABLE `clocks` DISABLE KEYS */;
/*!40000 ALTER TABLE `clocks` ENABLE KEYS */;

--
-- Definition of colours
--

DROP TABLE IF EXISTS `colours`;
CREATE TABLE IF NOT EXISTS `colours` (
  `Id` bigint NOT NULL AUTO_INCREMENT,
  `Name` varchar(50) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `Basic` int NOT NULL,
  `Red` int NOT NULL,
  `Green` int NOT NULL,
  `Blue` int NOT NULL,
  `Fancy` varchar(4000) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci DEFAULT NULL,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Dumping data for table colours
--

/*!40000 ALTER TABLE `colours` DISABLE KEYS */;
/*!40000 ALTER TABLE `colours` ENABLE KEYS */;

--
-- Definition of computermailmessages
--

DROP TABLE IF EXISTS `computermailmessages`;
CREATE TABLE IF NOT EXISTS `computermailmessages` (
  `Id` bigint NOT NULL AUTO_INCREMENT,
  `SenderAddress` varchar(255) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `RecipientAddress` varchar(255) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `Subject` varchar(500) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `Body` longtext CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `SentAtUtc` datetime NOT NULL,
  PRIMARY KEY (`Id`),
  KEY `IX_ComputerMailMessages_SentAtUtc` (`SentAtUtc`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Dumping data for table computermailmessages
--

/*!40000 ALTER TABLE `computermailmessages` DISABLE KEYS */;
/*!40000 ALTER TABLE `computermailmessages` ENABLE KEYS */;

--
-- Definition of computermailmailboxentries
--

DROP TABLE IF EXISTS `computermailmailboxentries`;
CREATE TABLE IF NOT EXISTS `computermailmailboxentries` (
  `Id` bigint NOT NULL AUTO_INCREMENT,
  `ComputerMailAccountId` bigint NOT NULL,
  `ComputerMailMessageId` bigint NOT NULL,
  `IsSentFolder` bit(1) NOT NULL,
  `IsRead` bit(1) NOT NULL,
  `IsDeleted` bit(1) NOT NULL,
  `DeliveredAtUtc` datetime NOT NULL,
  PRIMARY KEY (`Id`),
  KEY `FK_ComputerMailMailboxEntries_ComputerMailAccounts_idx` (`ComputerMailAccountId`),
  KEY `FK_ComputerMailMailboxEntries_ComputerMailMessages_idx` (`ComputerMailMessageId`),
  KEY `IX_ComputerMailMailboxEntries_Account_Folder_Delivered` (`ComputerMailAccountId`,`IsDeleted`,`IsSentFolder`,`DeliveredAtUtc`),
  CONSTRAINT `FK_ComputerMailMailboxEntries_ComputerMailAccounts` FOREIGN KEY (`ComputerMailAccountId`) REFERENCES `computermailaccounts` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_ComputerMailMailboxEntries_ComputerMailMessages` FOREIGN KEY (`ComputerMailMessageId`) REFERENCES `computermailmessages` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Dumping data for table computermailmailboxentries
--

/*!40000 ALTER TABLE `computermailmailboxentries` DISABLE KEYS */;
/*!40000 ALTER TABLE `computermailmailboxentries` ENABLE KEYS */;

--
-- Definition of corpsemodels
--

DROP TABLE IF EXISTS `corpsemodels`;
CREATE TABLE IF NOT EXISTS `corpsemodels` (
  `Id` bigint NOT NULL AUTO_INCREMENT,
  `Name` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `Description` text CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci,
  `Definition` text CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci,
  `Type` varchar(45) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Dumping data for table corpsemodels
--

/*!40000 ALTER TABLE `corpsemodels` DISABLE KEYS */;
/*!40000 ALTER TABLE `corpsemodels` ENABLE KEYS */;

--
-- Definition of cultureinfos
--

DROP TABLE IF EXISTS `cultureinfos`;
CREATE TABLE IF NOT EXISTS `cultureinfos` (
  `Id` varchar(50) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `DisplayName` varchar(4000) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `Order` int NOT NULL,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Dumping data for table cultureinfos
--

/*!40000 ALTER TABLE `cultureinfos` DISABLE KEYS */;
/*!40000 ALTER TABLE `cultureinfos` ENABLE KEYS */;

--
-- Definition of currencies
--

DROP TABLE IF EXISTS `currencies`;
CREATE TABLE IF NOT EXISTS `currencies` (
  `Id` bigint NOT NULL AUTO_INCREMENT,
  `Name` varchar(4000) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `BaseCurrencyToGlobalBaseCurrencyConversion` decimal(58,29) NOT NULL DEFAULT '1.00000000000000000000000000000',
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Dumping data for table currencies
--

/*!40000 ALTER TABLE `currencies` DISABLE KEYS */;
/*!40000 ALTER TABLE `currencies` ENABLE KEYS */;

--
-- Definition of bankcurrencyreserves
--

DROP TABLE IF EXISTS `bankcurrencyreserves`;
CREATE TABLE IF NOT EXISTS `bankcurrencyreserves` (
  `BankId` bigint NOT NULL,
  `CurrencyId` bigint NOT NULL,
  `Amount` decimal(58,29) NOT NULL,
  PRIMARY KEY (`BankId`,`CurrencyId`),
  KEY `IX_BankCurrencyReserves_CurrencyId` (`CurrencyId`),
  CONSTRAINT `FK_BankCurrencyReserves_Banks` FOREIGN KEY (`BankId`) REFERENCES `banks` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_BankCurrencyReserves_Currencies` FOREIGN KEY (`CurrencyId`) REFERENCES `currencies` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Dumping data for table bankcurrencyreserves
--

/*!40000 ALTER TABLE `bankcurrencyreserves` DISABLE KEYS */;
/*!40000 ALTER TABLE `bankcurrencyreserves` ENABLE KEYS */;

--
-- Definition of bankexchangerates
--

DROP TABLE IF EXISTS `bankexchangerates`;
CREATE TABLE IF NOT EXISTS `bankexchangerates` (
  `BankId` bigint NOT NULL,
  `FromCurrencyId` bigint NOT NULL,
  `ToCurrencyId` bigint NOT NULL,
  `ExchangeRate` decimal(58,29) NOT NULL,
  PRIMARY KEY (`BankId`,`FromCurrencyId`,`ToCurrencyId`),
  KEY `IX_BankExchangeRates_FromCurrencyId` (`FromCurrencyId`),
  KEY `IX_BankExchangeRates_ToCurrencyId` (`ToCurrencyId`),
  CONSTRAINT `FK_BankExchangeRates_Banks` FOREIGN KEY (`BankId`) REFERENCES `banks` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_BankExchangeRates_Currencies_From` FOREIGN KEY (`FromCurrencyId`) REFERENCES `currencies` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_BankExchangeRates_Currencies_To` FOREIGN KEY (`ToCurrencyId`) REFERENCES `currencies` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Dumping data for table bankexchangerates
--

/*!40000 ALTER TABLE `bankexchangerates` DISABLE KEYS */;
/*!40000 ALTER TABLE `bankexchangerates` ENABLE KEYS */;

--
-- Definition of chargenroles_currencies
--

DROP TABLE IF EXISTS `chargenroles_currencies`;
CREATE TABLE IF NOT EXISTS `chargenroles_currencies` (
  `ChargenRoleId` bigint NOT NULL,
  `CurrencyId` bigint NOT NULL,
  `Amount` decimal(58,29) NOT NULL,
  PRIMARY KEY (`ChargenRoleId`,`CurrencyId`),
  KEY `FK_ChargenRoles_Currencies_Currencies` (`CurrencyId`),
  CONSTRAINT `FK_ChargenRoles_Currencies_ChargenRoles` FOREIGN KEY (`ChargenRoleId`) REFERENCES `chargenroles` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_ChargenRoles_Currencies_Currencies` FOREIGN KEY (`CurrencyId`) REFERENCES `currencies` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Dumping data for table chargenroles_currencies
--

/*!40000 ALTER TABLE `chargenroles_currencies` DISABLE KEYS */;
/*!40000 ALTER TABLE `chargenroles_currencies` ENABLE KEYS */;

--
-- Definition of clanbudgets
--

DROP TABLE IF EXISTS `clanbudgets`;
CREATE TABLE IF NOT EXISTS `clanbudgets` (
  `Id` bigint NOT NULL AUTO_INCREMENT,
  `ClanId` bigint NOT NULL,
  `AppointmentId` bigint NOT NULL,
  `BankAccountId` bigint DEFAULT NULL,
  `CurrencyId` bigint NOT NULL,
  `Name` varchar(200) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `AmountPerPeriod` decimal(58,29) NOT NULL,
  `PeriodIntervalType` int NOT NULL,
  `PeriodIntervalModifier` int NOT NULL,
  `PeriodIntervalOther` int NOT NULL,
  `PeriodIntervalOtherSecondary` int NOT NULL,
  `PeriodIntervalFallback` int NOT NULL,
  `CurrentPeriodStart` varchar(255) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `CurrentPeriodEnd` varchar(255) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `CurrentPeriodDrawdown` decimal(58,29) NOT NULL,
  `IsActive` bit(1) NOT NULL DEFAULT b'1',
  PRIMARY KEY (`Id`),
  KEY `FK_ClanBudgets_Appointments_idx` (`AppointmentId`),
  KEY `FK_ClanBudgets_BankAccounts_idx` (`BankAccountId`),
  KEY `FK_ClanBudgets_Clans_idx` (`ClanId`),
  KEY `FK_ClanBudgets_Currencies_idx` (`CurrencyId`),
  CONSTRAINT `FK_ClanBudgets_Appointments` FOREIGN KEY (`AppointmentId`) REFERENCES `appointments` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_ClanBudgets_BankAccounts` FOREIGN KEY (`BankAccountId`) REFERENCES `bankaccounts` (`Id`) ON DELETE SET NULL,
  CONSTRAINT `FK_ClanBudgets_Clans` FOREIGN KEY (`ClanId`) REFERENCES `clans` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_ClanBudgets_Currencies` FOREIGN KEY (`CurrencyId`) REFERENCES `currencies` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Dumping data for table clanbudgets
--

/*!40000 ALTER TABLE `clanbudgets` DISABLE KEYS */;
/*!40000 ALTER TABLE `clanbudgets` ENABLE KEYS */;

--
-- Definition of clanbudgettransactions
--

DROP TABLE IF EXISTS `clanbudgettransactions`;
CREATE TABLE IF NOT EXISTS `clanbudgettransactions` (
  `Id` bigint NOT NULL AUTO_INCREMENT,
  `ClanBudgetId` bigint NOT NULL,
  `ActorId` bigint NOT NULL,
  `BankAccountId` bigint DEFAULT NULL,
  `CurrencyId` bigint NOT NULL,
  `Amount` decimal(58,29) NOT NULL,
  `TransactionTime` varchar(255) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `PeriodStart` varchar(255) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `PeriodEnd` varchar(255) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `BankBalanceAfter` decimal(58,29) NOT NULL,
  `Reason` varchar(1000) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  PRIMARY KEY (`Id`),
  KEY `FK_ClanBudgetTransactions_Actors_idx` (`ActorId`),
  KEY `FK_ClanBudgetTransactions_BankAccounts_idx` (`BankAccountId`),
  KEY `FK_ClanBudgetTransactions_Budgets_idx` (`ClanBudgetId`),
  KEY `FK_ClanBudgetTransactions_Currencies_idx` (`CurrencyId`),
  CONSTRAINT `FK_ClanBudgetTransactions_BankAccounts` FOREIGN KEY (`BankAccountId`) REFERENCES `bankaccounts` (`Id`) ON DELETE SET NULL,
  CONSTRAINT `FK_ClanBudgetTransactions_Budgets` FOREIGN KEY (`ClanBudgetId`) REFERENCES `clanbudgets` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_ClanBudgetTransactions_Characters` FOREIGN KEY (`ActorId`) REFERENCES `characters` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_ClanBudgetTransactions_Currencies` FOREIGN KEY (`CurrencyId`) REFERENCES `currencies` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Dumping data for table clanbudgettransactions
--

/*!40000 ALTER TABLE `clanbudgettransactions` DISABLE KEYS */;
/*!40000 ALTER TABLE `clanbudgettransactions` ENABLE KEYS */;

--
-- Definition of clanmemberships_backpay
--

DROP TABLE IF EXISTS `clanmemberships_backpay`;
CREATE TABLE IF NOT EXISTS `clanmemberships_backpay` (
  `ClanId` bigint NOT NULL,
  `CharacterId` bigint NOT NULL,
  `CurrencyId` bigint NOT NULL,
  `Amount` decimal(58,29) NOT NULL,
  PRIMARY KEY (`CurrencyId`,`ClanId`,`CharacterId`),
  KEY `FK_ClanMemberships_Backpay_ClanMemberships` (`ClanId`,`CharacterId`),
  CONSTRAINT `FK_ClanMemberships_Backpay_ClanMemberships` FOREIGN KEY (`ClanId`, `CharacterId`) REFERENCES `clanmemberships` (`ClanId`, `CharacterId`) ON DELETE CASCADE,
  CONSTRAINT `FK_ClanMemberships_Backpay_Currencies` FOREIGN KEY (`CurrencyId`) REFERENCES `currencies` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Dumping data for table clanmemberships_backpay
--

/*!40000 ALTER TABLE `clanmemberships_backpay` DISABLE KEYS */;
/*!40000 ALTER TABLE `clanmemberships_backpay` ENABLE KEYS */;

--
-- Definition of coins
--

DROP TABLE IF EXISTS `coins`;
CREATE TABLE IF NOT EXISTS `coins` (
  `Id` bigint NOT NULL AUTO_INCREMENT,
  `Name` varchar(4000) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `ShortDescription` varchar(4000) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `FullDescription` varchar(4000) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `Value` decimal(58,29) NOT NULL,
  `CurrencyId` bigint NOT NULL,
  `Weight` double NOT NULL,
  `GeneralForm` varchar(4000) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `PluralWord` varchar(4000) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci DEFAULT NULL,
  `UseForChange` bit(1) NOT NULL DEFAULT b'1',
  PRIMARY KEY (`Id`),
  KEY `FK_Coins_Currencies` (`CurrencyId`),
  CONSTRAINT `FK_Coins_Currencies` FOREIGN KEY (`CurrencyId`) REFERENCES `currencies` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Dumping data for table coins
--

/*!40000 ALTER TABLE `coins` DISABLE KEYS */;
/*!40000 ALTER TABLE `coins` ENABLE KEYS */;

--
-- Definition of currencydivisions
--

DROP TABLE IF EXISTS `currencydivisions`;
CREATE TABLE IF NOT EXISTS `currencydivisions` (
  `Id` bigint NOT NULL AUTO_INCREMENT,
  `Name` varchar(4000) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `BaseUnitConversionRate` decimal(58,29) NOT NULL,
  `CurrencyId` bigint NOT NULL,
  `IgnoreCase` bit(1) NOT NULL DEFAULT b'1',
  PRIMARY KEY (`Id`),
  KEY `FK_CurrencyDivisions_Currencies` (`CurrencyId`),
  CONSTRAINT `FK_CurrencyDivisions_Currencies` FOREIGN KEY (`CurrencyId`) REFERENCES `currencies` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Dumping data for table currencydivisions
--

/*!40000 ALTER TABLE `currencydivisions` DISABLE KEYS */;
/*!40000 ALTER TABLE `currencydivisions` ENABLE KEYS */;

--
-- Definition of currencydescriptionpatternelements
--

DROP TABLE IF EXISTS `currencydescriptionpatternelements`;
CREATE TABLE IF NOT EXISTS `currencydescriptionpatternelements` (
  `Id` bigint NOT NULL AUTO_INCREMENT,
  `Pattern` varchar(4000) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `Order` int NOT NULL,
  `ShowIfZero` bit(1) NOT NULL,
  `CurrencyDivisionId` bigint NOT NULL,
  `CurrencyDescriptionPatternId` bigint NOT NULL,
  `PluraliseWord` varchar(4000) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `AlternatePattern` varchar(4000) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci DEFAULT NULL,
  `RoundingMode` int NOT NULL,
  `SpecialValuesOverrideFormat` bit(1) NOT NULL DEFAULT b'0',
  PRIMARY KEY (`Id`),
  KEY `FK_CDPE_CurrencyDescriptionPatterns` (`CurrencyDescriptionPatternId`),
  KEY `FK_CDPE_CurrencyDivisions` (`CurrencyDivisionId`),
  CONSTRAINT `FK_CDPE_CurrencyDescriptionPatterns` FOREIGN KEY (`CurrencyDescriptionPatternId`) REFERENCES `currencydescriptionpatterns` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_CDPE_CurrencyDivisions` FOREIGN KEY (`CurrencyDivisionId`) REFERENCES `currencydivisions` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Dumping data for table currencydescriptionpatternelements
--

/*!40000 ALTER TABLE `currencydescriptionpatternelements` DISABLE KEYS */;
/*!40000 ALTER TABLE `currencydescriptionpatternelements` ENABLE KEYS */;

--
-- Definition of currencydescriptionpatternelementspecialvalues
--

DROP TABLE IF EXISTS `currencydescriptionpatternelementspecialvalues`;
CREATE TABLE IF NOT EXISTS `currencydescriptionpatternelementspecialvalues` (
  `Value` decimal(58,29) NOT NULL,
  `CurrencyDescriptionPatternElementId` bigint NOT NULL,
  `Text` varchar(4000) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  PRIMARY KEY (`Value`,`CurrencyDescriptionPatternElementId`),
  KEY `FK_CDPESV_CDPE` (`CurrencyDescriptionPatternElementId`),
  CONSTRAINT `FK_CDPESV_CDPE` FOREIGN KEY (`CurrencyDescriptionPatternElementId`) REFERENCES `currencydescriptionpatternelements` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Dumping data for table currencydescriptionpatternelementspecialvalues
--

/*!40000 ALTER TABLE `currencydescriptionpatternelementspecialvalues` DISABLE KEYS */;
/*!40000 ALTER TABLE `currencydescriptionpatternelementspecialvalues` ENABLE KEYS */;

--
-- Definition of currencydivisionabbreviations
--

DROP TABLE IF EXISTS `currencydivisionabbreviations`;
CREATE TABLE IF NOT EXISTS `currencydivisionabbreviations` (
  `Pattern` varchar(150) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `CurrencyDivisionId` bigint NOT NULL,
  PRIMARY KEY (`Pattern`,`CurrencyDivisionId`),
  KEY `FK_CurrencyDivisionAbbreviations_CurrencyDivisions` (`CurrencyDivisionId`),
  CONSTRAINT `FK_CurrencyDivisionAbbreviations_CurrencyDivisions` FOREIGN KEY (`CurrencyDivisionId`) REFERENCES `currencydivisions` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Dumping data for table currencydivisionabbreviations
--

/*!40000 ALTER TABLE `currencydivisionabbreviations` DISABLE KEYS */;
/*!40000 ALTER TABLE `currencydivisionabbreviations` ENABLE KEYS */;

--
-- Definition of damagepatterns
--

DROP TABLE IF EXISTS `damagepatterns`;
CREATE TABLE IF NOT EXISTS `damagepatterns` (
  `Id` bigint NOT NULL AUTO_INCREMENT,
  `Name` varchar(4000) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `DamageType` int NOT NULL,
  `Dice` int NOT NULL,
  `Sides` int NOT NULL,
  `Bonus` int NOT NULL,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Dumping data for table damagepatterns
--

/*!40000 ALTER TABLE `damagepatterns` DISABLE KEYS */;
/*!40000 ALTER TABLE `damagepatterns` ENABLE KEYS */;

--
-- Definition of drugs
--

DROP TABLE IF EXISTS `drugs`;
CREATE TABLE IF NOT EXISTS `drugs` (
  `Id` bigint NOT NULL AUTO_INCREMENT,
  `Name` varchar(200) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL,
  `DrugVectors` int NOT NULL,
  `IntensityPerGram` double NOT NULL,
  `RelativeMetabolisationRate` double NOT NULL,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Dumping data for table drugs
--

/*!40000 ALTER TABLE `drugs` DISABLE KEYS */;
/*!40000 ALTER TABLE `drugs` ENABLE KEYS */;

--
-- Definition of bodies_drugdoses
--

DROP TABLE IF EXISTS `bodies_drugdoses`;
CREATE TABLE IF NOT EXISTS `bodies_drugdoses` (
  `BodyId` bigint NOT NULL,
  `DrugId` bigint NOT NULL,
  `Active` bit(1) NOT NULL,
  `Grams` double NOT NULL,
  `OriginalVector` int NOT NULL,
  PRIMARY KEY (`BodyId`,`DrugId`,`Active`),
  KEY `FK_Bodies_DrugDoses_Drugs_idx` (`DrugId`),
  CONSTRAINT `FK_Bodies_DrugDoses_Bodies` FOREIGN KEY (`BodyId`) REFERENCES `bodies` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_Bodies_DrugDoses_Drugs` FOREIGN KEY (`DrugId`) REFERENCES `drugs` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Dumping data for table bodies_drugdoses
--

/*!40000 ALTER TABLE `bodies_drugdoses` DISABLE KEYS */;
/*!40000 ALTER TABLE `bodies_drugdoses` ENABLE KEYS */;

--
-- Definition of bodies_drugexposures
--

DROP TABLE IF EXISTS `bodies_drugexposures`;
CREATE TABLE IF NOT EXISTS `bodies_drugexposures` (
  `BodyId` bigint NOT NULL,
  `DrugId` bigint NOT NULL,
  `Exposure` double NOT NULL,
  `PeakExposure` double NOT NULL,
  `WithdrawalIntensity` double NOT NULL,
  `LastUpdatedAtUtc` datetime NOT NULL,
  PRIMARY KEY (`BodyId`,`DrugId`),
  KEY `FK_Bodies_DrugExposures_Drugs_idx` (`DrugId`),
  CONSTRAINT `FK_Bodies_DrugExposures_Bodies` FOREIGN KEY (`BodyId`) REFERENCES `bodies` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_Bodies_DrugExposures_Drugs` FOREIGN KEY (`DrugId`) REFERENCES `drugs` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Dumping data for table bodies_drugexposures
--

/*!40000 ALTER TABLE `bodies_drugexposures` DISABLE KEYS */;
/*!40000 ALTER TABLE `bodies_drugexposures` ENABLE KEYS */;

--
-- Definition of drugsintensities
--

DROP TABLE IF EXISTS `drugsintensities`;
CREATE TABLE IF NOT EXISTS `drugsintensities` (
  `DrugId` bigint NOT NULL AUTO_INCREMENT,
  `DrugType` int NOT NULL,
  `RelativeIntensity` double NOT NULL,
  `AdditionalEffects` text CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci,
  PRIMARY KEY (`DrugId`,`DrugType`),
  CONSTRAINT `FK_Drugs_DrugIntensities` FOREIGN KEY (`DrugId`) REFERENCES `drugs` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Dumping data for table drugsintensities
--

/*!40000 ALTER TABLE `drugsintensities` DISABLE KEYS */;
/*!40000 ALTER TABLE `drugsintensities` ENABLE KEYS */;

--
-- Definition of editableitems
--

DROP TABLE IF EXISTS `editableitems`;
CREATE TABLE IF NOT EXISTS `editableitems` (
  `Id` bigint NOT NULL AUTO_INCREMENT,
  `RevisionNumber` int NOT NULL,
  `RevisionStatus` int NOT NULL,
  `BuilderAccountId` bigint NOT NULL,
  `ReviewerAccountId` bigint DEFAULT NULL,
  `BuilderComment` varchar(4000) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci DEFAULT NULL,
  `ReviewerComment` varchar(4000) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci DEFAULT NULL,
  `BuilderDate` datetime NOT NULL,
  `ReviewerDate` datetime DEFAULT NULL,
  `ObsoleteDate` datetime DEFAULT NULL,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Dumping data for table editableitems
--

/*!40000 ALTER TABLE `editableitems` DISABLE KEYS */;
/*!40000 ALTER TABLE `editableitems` ENABLE KEYS */;

--
-- Definition of celloverlaypackages
--

DROP TABLE IF EXISTS `celloverlaypackages`;
CREATE TABLE IF NOT EXISTS `celloverlaypackages` (
  `Id` bigint NOT NULL,
  `RevisionNumber` int NOT NULL,
  `Name` varchar(4000) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `EditableItemId` bigint NOT NULL,
  PRIMARY KEY (`Id`,`RevisionNumber`),
  KEY `FK_CellOverlayPackages_EditableItems` (`EditableItemId`),
  CONSTRAINT `FK_CellOverlayPackages_EditableItems` FOREIGN KEY (`EditableItemId`) REFERENCES `editableitems` (`Id`) ON DELETE RESTRICT
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Dumping data for table celloverlaypackages
--

/*!40000 ALTER TABLE `celloverlaypackages` DISABLE KEYS */;
/*!40000 ALTER TABLE `celloverlaypackages` ENABLE KEYS */;

--
-- Definition of disfigurementtemplates
--

DROP TABLE IF EXISTS `disfigurementtemplates`;
CREATE TABLE IF NOT EXISTS `disfigurementtemplates` (
  `Id` bigint NOT NULL,
  `RevisionNumber` int NOT NULL,
  `Name` varchar(500) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `Type` varchar(50) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `EditableItemId` bigint NOT NULL,
  `ShortDescription` varchar(500) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `FullDescription` varchar(5000) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `Definition` text CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  PRIMARY KEY (`Id`,`RevisionNumber`),
  KEY `FK_DisfigurementTemplates_EditableItems_idx` (`EditableItemId`),
  CONSTRAINT `FK_DisfigurementTemplates_EditableItems` FOREIGN KEY (`EditableItemId`) REFERENCES `editableitems` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Dumping data for table disfigurementtemplates
--

/*!40000 ALTER TABLE `disfigurementtemplates` DISABLE KEYS */;
/*!40000 ALTER TABLE `disfigurementtemplates` ENABLE KEYS */;

--
-- Definition of emailtemplates
--

DROP TABLE IF EXISTS `emailtemplates`;
CREATE TABLE IF NOT EXISTS `emailtemplates` (
  `TemplateType` int NOT NULL,
  `Content` varchar(4000) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `Subject` varchar(4000) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `ReturnAddress` varchar(4000) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  PRIMARY KEY (`TemplateType`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Dumping data for table emailtemplates
--

/*!40000 ALTER TABLE `emailtemplates` DISABLE KEYS */;
/*!40000 ALTER TABLE `emailtemplates` ENABLE KEYS */;

--
-- Definition of employmenthoststates
--

DROP TABLE IF EXISTS `employmenthoststates`;
CREATE TABLE IF NOT EXISTS `employmenthoststates` (
  `Id` bigint NOT NULL AUTO_INCREMENT,
  `HostType` varchar(50) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `HostId` bigint NOT NULL,
  `BoardId` bigint NOT NULL,
  `CreatedAt` datetime NOT NULL,
  `LastUpdatedAt` datetime NOT NULL,
  PRIMARY KEY (`Id`),
  UNIQUE KEY `IX_EmploymentHostStates_Host` (`HostType`,`HostId`),
  KEY `FK_EmploymentHostStates_Boards_idx` (`BoardId`),
  CONSTRAINT `FK_EmploymentHostStates_Boards` FOREIGN KEY (`BoardId`) REFERENCES `boards` (`Id`) ON DELETE RESTRICT
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Dumping data for table employmenthoststates
--

/*!40000 ALTER TABLE `employmenthoststates` DISABLE KEYS */;
/*!40000 ALTER TABLE `employmenthoststates` ENABLE KEYS */;

--
-- Definition of employmentactionplans
--

DROP TABLE IF EXISTS `employmentactionplans`;
CREATE TABLE IF NOT EXISTS `employmentactionplans` (
  `Id` bigint NOT NULL AUTO_INCREMENT,
  `EmploymentHostStateId` bigint NOT NULL,
  `Name` varchar(200) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  PRIMARY KEY (`Id`),
  KEY `FK_EmploymentActionPlans_HostStates_idx` (`EmploymentHostStateId`),
  CONSTRAINT `FK_EmploymentActionPlans_HostStates` FOREIGN KEY (`EmploymentHostStateId`) REFERENCES `employmenthoststates` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Dumping data for table employmentactionplans
--

/*!40000 ALTER TABLE `employmentactionplans` DISABLE KEYS */;
/*!40000 ALTER TABLE `employmentactionplans` ENABLE KEYS */;

--
-- Definition of employmentactionsteps
--

DROP TABLE IF EXISTS `employmentactionsteps`;
CREATE TABLE IF NOT EXISTS `employmentactionsteps` (
  `Id` bigint NOT NULL AUTO_INCREMENT,
  `EmploymentActionPlanId` bigint NOT NULL,
  `SortOrder` int NOT NULL,
  `StepType` int NOT NULL,
  `RequiredAuthority` bigint NOT NULL,
  `RequiredCapabilities` varchar(500) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `RequiresPaymentAuthorisation` bit(1) NOT NULL,
  `IsFinancialStep` bit(1) NOT NULL,
  `Description` mediumtext CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci,
  `AmountCurrencyId` bigint DEFAULT NULL,
  `Amount` decimal(58,29) DEFAULT NULL,
  `ExistingFinancialRecord` varchar(200) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci DEFAULT NULL,
  `DestinationCellId` bigint DEFAULT NULL,
  `ExecutionCellId` bigint DEFAULT NULL,
  `CommandName` varchar(100) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci DEFAULT NULL,
  `CommandArguments` mediumtext CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci,
  `AccountName` varchar(200) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci DEFAULT NULL,
  `BoardTitle` varchar(200) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci DEFAULT NULL,
  `BoardText` mediumtext CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci,
  PRIMARY KEY (`Id`),
  KEY `FK_EmploymentActionSteps_Plans_idx` (`EmploymentActionPlanId`),
  CONSTRAINT `FK_EmploymentActionSteps_Plans` FOREIGN KEY (`EmploymentActionPlanId`) REFERENCES `employmentactionplans` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Dumping data for table employmentactionsteps
--

/*!40000 ALTER TABLE `employmentactionsteps` DISABLE KEYS */;
/*!40000 ALTER TABLE `employmentactionsteps` ENABLE KEYS */;

--
-- Definition of employmentactivetasks
--

DROP TABLE IF EXISTS `employmentactivetasks`;
CREATE TABLE IF NOT EXISTS `employmentactivetasks` (
  `Id` bigint NOT NULL AUTO_INCREMENT,
  `PublicId` varchar(36) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `EmploymentHostStateId` bigint NOT NULL,
  `Name` varchar(200) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `EmploymentActionPlanId` bigint NOT NULL,
  `Status` int NOT NULL,
  `AssignedEmployeeId` bigint DEFAULT NULL,
  `BlockedReason` mediumtext CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci,
  `CorrelationId` varchar(36) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `IdempotencyKey` varchar(200) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  PRIMARY KEY (`Id`),
  UNIQUE KEY `IX_EmploymentActiveTasks_PublicId` (`PublicId`),
  KEY `FK_EmploymentActiveTasks_HostStates_idx` (`EmploymentHostStateId`),
  KEY `FK_EmploymentActiveTasks_Plans_idx` (`EmploymentActionPlanId`),
  CONSTRAINT `FK_EmploymentActiveTasks_HostStates` FOREIGN KEY (`EmploymentHostStateId`) REFERENCES `employmenthoststates` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_EmploymentActiveTasks_Plans` FOREIGN KEY (`EmploymentActionPlanId`) REFERENCES `employmentactionplans` (`Id`) ON DELETE RESTRICT
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Dumping data for table employmentactivetasks
--

/*!40000 ALTER TABLE `employmentactivetasks` DISABLE KEYS */;
/*!40000 ALTER TABLE `employmentactivetasks` ENABLE KEYS */;

--
-- Definition of employmentactivetaskstepstates
--

DROP TABLE IF EXISTS `employmentactivetaskstepstates`;
CREATE TABLE IF NOT EXISTS `employmentactivetaskstepstates` (
  `Id` bigint NOT NULL AUTO_INCREMENT,
  `EmploymentActiveTaskId` bigint NOT NULL,
  `SortOrder` int NOT NULL,
  `Status` int NOT NULL,
  `CraftJobReference` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  `FailureDiagnostic` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  `LoadedAssets` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  `OperationalPayload` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  `ReservationReference` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  `RouteResult` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  `SelectedResources` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  `TransactionReference` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  PRIMARY KEY (`Id`),
  KEY `FK_EmploymentActiveTaskStepStates_Tasks_idx` (`EmploymentActiveTaskId`),
  CONSTRAINT `FK_EmploymentActiveTaskStepStates_Tasks` FOREIGN KEY (`EmploymentActiveTaskId`) REFERENCES `employmentactivetasks` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Dumping data for table employmentactivetaskstepstates
--

/*!40000 ALTER TABLE `employmentactivetaskstepstates` DISABLE KEYS */;
/*!40000 ALTER TABLE `employmentactivetaskstepstates` ENABLE KEYS */;

--
-- Definition of employmentconditionpredicates
--

DROP TABLE IF EXISTS `employmentconditionpredicates`;
CREATE TABLE IF NOT EXISTS `employmentconditionpredicates` (
  `Id` bigint NOT NULL AUTO_INCREMENT,
  `PublicId` varchar(36) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `EmploymentHostStateId` bigint NOT NULL,
  `Name` varchar(200) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `ExpressionJson` text CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  PRIMARY KEY (`Id`),
  UNIQUE KEY `IX_EmploymentConditionPredicates_Host_Name` (`EmploymentHostStateId`,`Name`),
  UNIQUE KEY `IX_EmploymentConditionPredicates_PublicId` (`PublicId`),
  KEY `FK_EmploymentConditionPredicates_HostStates_idx` (`EmploymentHostStateId`),
  CONSTRAINT `FK_EmploymentConditionPredicates_HostStates` FOREIGN KEY (`EmploymentHostStateId`) REFERENCES `employmenthoststates` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Dumping data for table employmentconditionpredicates
--

/*!40000 ALTER TABLE `employmentconditionpredicates` DISABLE KEYS */;
/*!40000 ALTER TABLE `employmentconditionpredicates` ENABLE KEYS */;

--
-- Definition of employmentcontracts
--

DROP TABLE IF EXISTS `employmentcontracts`;
CREATE TABLE IF NOT EXISTS `employmentcontracts` (
  `Id` bigint NOT NULL AUTO_INCREMENT,
  `RuntimeId` bigint NOT NULL,
  `EmploymentHostStateId` bigint NOT NULL,
  `EmployeeId` bigint NOT NULL,
  `Role` int NOT NULL,
  `Status` int NOT NULL,
  `Authority` bigint NOT NULL,
  `FixedRateCurrencyId` bigint DEFAULT NULL,
  `FixedRateAmount` decimal(58,29) DEFAULT NULL,
  `MarketBindingType` int NOT NULL,
  `MarketBindingValue` decimal(58,29) DEFAULT NULL,
  `PayCadence` int NOT NULL,
  `MinimumEffectivePayCurrencyId` bigint DEFAULT NULL,
  `MinimumEffectivePayAmount` decimal(58,29) DEFAULT NULL,
  `EmployerPaymentSource` int NOT NULL,
  `ScheduleDescription` varchar(500) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `ScheduleStartTicks` bigint DEFAULT NULL,
  `ScheduleEndTicks` bigint DEFAULT NULL,
  `DurationType` int NOT NULL,
  `DurationTicks` bigint DEFAULT NULL,
  `PaymentMethodKind` int NOT NULL,
  `PaymentBankAccountId` bigint DEFAULT NULL,
  `PaymentItemId` bigint DEFAULT NULL,
  `PaymentItemType` varchar(100) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci DEFAULT NULL,
  `PaymentNotes` mediumtext CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci,
  `StartedAt` datetime NOT NULL,
  `EndsAt` datetime DEFAULT NULL,
  `EndReason` int DEFAULT NULL,
  `OriginApplicationId` bigint DEFAULT NULL,
  `OriginOpeningId` bigint DEFAULT NULL,
  PRIMARY KEY (`Id`),
  UNIQUE KEY `IX_EmploymentContracts_Host_Runtime` (`EmploymentHostStateId`,`RuntimeId`),
  KEY `FK_EmploymentContracts_HostStates_idx` (`EmploymentHostStateId`),
  KEY `IX_EmploymentContracts_Employee` (`EmployeeId`),
  CONSTRAINT `FK_EmploymentContracts_HostStates` FOREIGN KEY (`EmploymentHostStateId`) REFERENCES `employmenthoststates` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Dumping data for table employmentcontracts
--

/*!40000 ALTER TABLE `employmentcontracts` DISABLE KEYS */;
/*!40000 ALTER TABLE `employmentcontracts` ENABLE KEYS */;

--
-- Definition of employmentjobopenings
--

DROP TABLE IF EXISTS `employmentjobopenings`;
CREATE TABLE IF NOT EXISTS `employmentjobopenings` (
  `Id` bigint NOT NULL AUTO_INCREMENT,
  `RuntimeId` bigint NOT NULL,
  `EmploymentHostStateId` bigint NOT NULL,
  `Role` int NOT NULL,
  `Status` int NOT NULL,
  `MaxPositions` int NOT NULL,
  `NpcApplicationsOnly` bit(1) NOT NULL,
  `Authority` bigint NOT NULL,
  `FixedRateCurrencyId` bigint DEFAULT NULL,
  `FixedRateAmount` decimal(58,29) DEFAULT NULL,
  `MarketBindingType` int NOT NULL,
  `MarketBindingValue` decimal(58,29) DEFAULT NULL,
  `PayCadence` int NOT NULL,
  `MinimumEffectivePayCurrencyId` bigint DEFAULT NULL,
  `MinimumEffectivePayAmount` decimal(58,29) DEFAULT NULL,
  `EmployerPaymentSource` int NOT NULL,
  `ScheduleDescription` varchar(500) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `ScheduleStartTicks` bigint DEFAULT NULL,
  `ScheduleEndTicks` bigint DEFAULT NULL,
  `DurationType` int NOT NULL,
  `DurationTicks` bigint DEFAULT NULL,
  `PaymentMethodKind` int NOT NULL,
  `PaymentBankAccountId` bigint DEFAULT NULL,
  `PaymentItemId` bigint DEFAULT NULL,
  `PaymentItemType` varchar(100) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci DEFAULT NULL,
  `PaymentNotes` mediumtext CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci,
  `RevisionNumber` int NOT NULL DEFAULT '1',
  PRIMARY KEY (`Id`),
  UNIQUE KEY `IX_EmploymentJobOpenings_Host_Runtime` (`EmploymentHostStateId`,`RuntimeId`),
  KEY `FK_EmploymentJobOpenings_HostStates_idx` (`EmploymentHostStateId`),
  CONSTRAINT `FK_EmploymentJobOpenings_HostStates` FOREIGN KEY (`EmploymentHostStateId`) REFERENCES `employmenthoststates` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Dumping data for table employmentjobopenings
--

/*!40000 ALTER TABLE `employmentjobopenings` DISABLE KEYS */;
/*!40000 ALTER TABLE `employmentjobopenings` ENABLE KEYS */;

--
-- Definition of employmentapplications
--

DROP TABLE IF EXISTS `employmentapplications`;
CREATE TABLE IF NOT EXISTS `employmentapplications` (
  `Id` bigint NOT NULL AUTO_INCREMENT,
  `RuntimeId` bigint NOT NULL,
  `EmploymentJobOpeningId` bigint NOT NULL,
  `CandidateId` bigint NOT NULL,
  `AppliedAt` datetime NOT NULL,
  `Status` int NOT NULL,
  `DecisionReason` mediumtext CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci,
  `CandidateProfileJson` mediumtext CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci,
  `OfferedOpeningRevision` int NOT NULL DEFAULT '1',
  PRIMARY KEY (`Id`),
  UNIQUE KEY `IX_EmploymentApplications_Opening_Runtime` (`EmploymentJobOpeningId`,`RuntimeId`),
  KEY `FK_EmploymentApplications_Openings_idx` (`EmploymentJobOpeningId`),
  KEY `IX_EmploymentApplications_Candidate` (`CandidateId`),
  CONSTRAINT `FK_EmploymentApplications_Openings` FOREIGN KEY (`EmploymentJobOpeningId`) REFERENCES `employmentjobopenings` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Dumping data for table employmentapplications
--

/*!40000 ALTER TABLE `employmentapplications` DISABLE KEYS */;
/*!40000 ALTER TABLE `employmentapplications` ENABLE KEYS */;

--
-- Definition of employmentjobopeningrequirements
--

DROP TABLE IF EXISTS `employmentjobopeningrequirements`;
CREATE TABLE IF NOT EXISTS `employmentjobopeningrequirements` (
  `Id` bigint NOT NULL AUTO_INCREMENT,
  `EmploymentJobOpeningId` bigint NOT NULL,
  `RequirementType` int NOT NULL,
  `Name` varchar(200) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `NumericValue` double DEFAULT NULL,
  `Capability` int DEFAULT NULL,
  PRIMARY KEY (`Id`),
  KEY `FK_EmploymentJobOpeningRequirements_Openings_idx` (`EmploymentJobOpeningId`),
  CONSTRAINT `FK_EmploymentJobOpeningRequirements_Openings` FOREIGN KEY (`EmploymentJobOpeningId`) REFERENCES `employmentjobopenings` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Dumping data for table employmentjobopeningrequirements
--

/*!40000 ALTER TABLE `employmentjobopeningrequirements` DISABLE KEYS */;
/*!40000 ALTER TABLE `employmentjobopeningrequirements` ENABLE KEYS */;

--
-- Definition of employmentledgerentries
--

DROP TABLE IF EXISTS `employmentledgerentries`;
CREATE TABLE IF NOT EXISTS `employmentledgerentries` (
  `Id` bigint NOT NULL AUTO_INCREMENT,
  `EmploymentHostStateId` bigint NOT NULL,
  `CorrelationId` varchar(36) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `EntryType` int NOT NULL,
  `ActorId` bigint DEFAULT NULL,
  `AmountCurrencyId` bigint DEFAULT NULL,
  `Amount` decimal(58,29) DEFAULT NULL,
  `Description` mediumtext CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `RecordedAt` datetime NOT NULL,
  PRIMARY KEY (`Id`),
  KEY `FK_EmploymentLedgerEntries_HostStates_idx` (`EmploymentHostStateId`),
  KEY `IX_EmploymentLedgerEntries_Correlation` (`CorrelationId`),
  CONSTRAINT `FK_EmploymentLedgerEntries_HostStates` FOREIGN KEY (`EmploymentHostStateId`) REFERENCES `employmenthoststates` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Dumping data for table employmentledgerentries
--

/*!40000 ALTER TABLE `employmentledgerentries` DISABLE KEYS */;
/*!40000 ALTER TABLE `employmentledgerentries` ENABLE KEYS */;

--
-- Definition of employmentmanagergoals
--

DROP TABLE IF EXISTS `employmentmanagergoals`;
CREATE TABLE IF NOT EXISTS `employmentmanagergoals` (
  `Id` bigint NOT NULL AUTO_INCREMENT,
  `RuntimeId` bigint NOT NULL,
  `EmploymentHostStateId` bigint NOT NULL,
  `GoalType` int NOT NULL,
  `RequiredAuthority` bigint NOT NULL,
  `Status` int NOT NULL,
  `ConfigurationDescription` mediumtext CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `EmploymentActionPlanId` bigint DEFAULT NULL,
  `Priority` int NOT NULL,
  `EvaluationCadenceTicks` bigint NOT NULL,
  `LastEvaluatedAt` datetime DEFAULT NULL,
  `LastEvaluationResult` mediumtext CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci,
  `CorrelationId` varchar(36) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  PRIMARY KEY (`Id`),
  UNIQUE KEY `IX_EmploymentManagerGoals_Host_Runtime` (`EmploymentHostStateId`,`RuntimeId`),
  KEY `FK_EmploymentManagerGoals_HostStates_idx` (`EmploymentHostStateId`),
  KEY `FK_EmploymentManagerGoals_Plans_idx` (`EmploymentActionPlanId`),
  CONSTRAINT `FK_EmploymentManagerGoals_HostStates` FOREIGN KEY (`EmploymentHostStateId`) REFERENCES `employmenthoststates` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_EmploymentManagerGoals_Plans` FOREIGN KEY (`EmploymentActionPlanId`) REFERENCES `employmentactionplans` (`Id`) ON DELETE RESTRICT
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Dumping data for table employmentmanagergoals
--

/*!40000 ALTER TABLE `employmentmanagergoals` DISABLE KEYS */;
/*!40000 ALTER TABLE `employmentmanagergoals` ENABLE KEYS */;

--
-- Definition of employmentpayables
--

DROP TABLE IF EXISTS `employmentpayables`;
CREATE TABLE IF NOT EXISTS `employmentpayables` (
  `Id` bigint NOT NULL AUTO_INCREMENT,
  `RuntimeId` bigint NOT NULL,
  `EmploymentHostStateId` bigint NOT NULL,
  `CorrelationId` varchar(36) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `ContractRuntimeId` bigint DEFAULT NULL,
  `EmployeeId` bigint NOT NULL,
  `EmployeeName` varchar(200) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `Role` int NOT NULL,
  `AmountCurrencyId` bigint NOT NULL,
  `Amount` decimal(58,29) NOT NULL,
  `PayCadence` int NOT NULL,
  `PaymentMethodKind` int NOT NULL,
  `PaymentBankAccountId` bigint DEFAULT NULL,
  `PaymentItemId` bigint DEFAULT NULL,
  `PaymentItemType` varchar(100) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci DEFAULT NULL,
  `PaymentNotes` mediumtext CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci,
  `PayPeriodStart` datetime NOT NULL,
  `PayPeriodEnd` datetime NOT NULL,
  `DueAt` datetime NOT NULL,
  `AccruedAt` datetime NOT NULL,
  `Status` int NOT NULL,
  `SettledAt` datetime DEFAULT NULL,
  `ClaimedAt` datetime DEFAULT NULL,
  `SettlementNote` mediumtext CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci,
  PRIMARY KEY (`Id`),
  UNIQUE KEY `IX_EmploymentPayables_Host_Runtime` (`EmploymentHostStateId`,`RuntimeId`),
  UNIQUE KEY `IX_EmploymentPayables_Contract_Period` (`EmploymentHostStateId`,`ContractRuntimeId`,`PayPeriodStart`,`PayPeriodEnd`),
  KEY `FK_EmploymentPayables_HostStates_idx` (`EmploymentHostStateId`),
  KEY `IX_EmploymentPayables_Correlation` (`CorrelationId`),
  KEY `IX_EmploymentPayables_Employee` (`EmployeeId`),
  CONSTRAINT `FK_EmploymentPayables_HostStates` FOREIGN KEY (`EmploymentHostStateId`) REFERENCES `employmenthoststates` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Dumping data for table employmentpayables
--

/*!40000 ALTER TABLE `employmentpayables` DISABLE KEYS */;
/*!40000 ALTER TABLE `employmentpayables` ENABLE KEYS */;

--
-- Definition of employmentregisterentries
--

DROP TABLE IF EXISTS `employmentregisterentries`;
CREATE TABLE IF NOT EXISTS `employmentregisterentries` (
  `Id` bigint NOT NULL AUTO_INCREMENT,
  `EmploymentHostStateId` bigint NOT NULL,
  `CorrelationId` varchar(36) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `EntryType` int NOT NULL,
  `ActorId` bigint DEFAULT NULL,
  `Description` mediumtext CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `RecordedAt` datetime NOT NULL,
  PRIMARY KEY (`Id`),
  KEY `FK_EmploymentRegisterEntries_HostStates_idx` (`EmploymentHostStateId`),
  KEY `IX_EmploymentRegisterEntries_Correlation` (`CorrelationId`),
  CONSTRAINT `FK_EmploymentRegisterEntries_HostStates` FOREIGN KEY (`EmploymentHostStateId`) REFERENCES `employmenthoststates` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Dumping data for table employmentregisterentries
--

/*!40000 ALTER TABLE `employmentregisterentries` DISABLE KEYS */;
/*!40000 ALTER TABLE `employmentregisterentries` ENABLE KEYS */;

--
-- Definition of employmentscheduledruletemplates
--

DROP TABLE IF EXISTS `employmentscheduledruletemplates`;
CREATE TABLE IF NOT EXISTS `employmentscheduledruletemplates` (
  `Id` bigint NOT NULL AUTO_INCREMENT,
  `PublicId` varchar(36) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `EmploymentHostStateId` bigint NOT NULL,
  `Name` varchar(200) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `IdempotencyKeyPattern` varchar(200) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `EmploymentActionPlanId` bigint NOT NULL,
  `ExpressionJson` text CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  `CooldownTicks` bigint NOT NULL,
  PRIMARY KEY (`Id`),
  UNIQUE KEY `IX_EmploymentScheduledRuleTemplates_Host_Name` (`EmploymentHostStateId`,`Name`),
  UNIQUE KEY `IX_EmploymentScheduledRuleTemplates_PublicId` (`PublicId`),
  KEY `FK_EmploymentScheduledRuleTemplates_HostStates_idx` (`EmploymentHostStateId`),
  KEY `FK_EmploymentScheduledRuleTemplates_Plans_idx` (`EmploymentActionPlanId`),
  CONSTRAINT `FK_EmploymentScheduledRuleTemplates_HostStates` FOREIGN KEY (`EmploymentHostStateId`) REFERENCES `employmenthoststates` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_EmploymentScheduledRuleTemplates_Plans` FOREIGN KEY (`EmploymentActionPlanId`) REFERENCES `employmentactionplans` (`Id`) ON DELETE RESTRICT
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Dumping data for table employmentscheduledruletemplates
--

/*!40000 ALTER TABLE `employmentscheduledruletemplates` DISABLE KEYS */;
/*!40000 ALTER TABLE `employmentscheduledruletemplates` ENABLE KEYS */;

--
-- Definition of employmentscheduledtaskrules
--

DROP TABLE IF EXISTS `employmentscheduledtaskrules`;
CREATE TABLE IF NOT EXISTS `employmentscheduledtaskrules` (
  `Id` bigint NOT NULL AUTO_INCREMENT,
  `PublicId` varchar(36) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `EmploymentHostStateId` bigint NOT NULL,
  `Name` varchar(200) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `IdempotencyKey` varchar(200) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `EmploymentActionPlanId` bigint NOT NULL,
  `CooldownTicks` bigint NOT NULL,
  `LastSpawnedAt` datetime DEFAULT NULL,
  `Status` int NOT NULL DEFAULT '0',
  `ExpressionJson` text CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  PRIMARY KEY (`Id`),
  UNIQUE KEY `IX_EmploymentScheduledTaskRules_PublicId` (`PublicId`),
  KEY `FK_EmploymentScheduledTaskRules_HostStates_idx` (`EmploymentHostStateId`),
  KEY `FK_EmploymentScheduledTaskRules_Plans_idx` (`EmploymentActionPlanId`),
  CONSTRAINT `FK_EmploymentScheduledTaskRules_HostStates` FOREIGN KEY (`EmploymentHostStateId`) REFERENCES `employmenthoststates` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_EmploymentScheduledTaskRules_Plans` FOREIGN KEY (`EmploymentActionPlanId`) REFERENCES `employmentactionplans` (`Id`) ON DELETE RESTRICT
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Dumping data for table employmentscheduledtaskrules
--

/*!40000 ALTER TABLE `employmentscheduledtaskrules` DISABLE KEYS */;
/*!40000 ALTER TABLE `employmentscheduledtaskrules` ENABLE KEYS */;

--
-- Definition of employmenttaskconditions
--

DROP TABLE IF EXISTS `employmenttaskconditions`;
CREATE TABLE IF NOT EXISTS `employmenttaskconditions` (
  `Id` bigint NOT NULL AUTO_INCREMENT,
  `ScheduledTaskRuleId` bigint DEFAULT NULL,
  `ManagerGoalId` bigint DEFAULT NULL,
  `SortOrder` int NOT NULL,
  `ConditionType` int NOT NULL,
  `Key` varchar(200) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci DEFAULT NULL,
  `ThresholdInt` int DEFAULT NULL,
  `ThresholdDecimal` decimal(58,29) DEFAULT NULL,
  `BoolValue` bit(1) DEFAULT NULL,
  `EarliestTicks` bigint DEFAULT NULL,
  `LatestTicks` bigint DEFAULT NULL,
  `ConditionPredicateId` bigint DEFAULT NULL,
  `ScheduledRuleTemplateId` bigint DEFAULT NULL,
  PRIMARY KEY (`Id`),
  KEY `FK_EmploymentTaskConditions_ManagerGoals_idx` (`ManagerGoalId`),
  KEY `FK_EmploymentTaskConditions_ScheduledRules_idx` (`ScheduledTaskRuleId`),
  KEY `FK_EmploymentTaskConditions_ConditionPredicates_idx` (`ConditionPredicateId`),
  KEY `FK_EmploymentTaskConditions_ScheduledRuleTemplates_idx` (`ScheduledRuleTemplateId`),
  CONSTRAINT `FK_EmploymentTaskConditions_ConditionPredicates` FOREIGN KEY (`ConditionPredicateId`) REFERENCES `employmentconditionpredicates` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_EmploymentTaskConditions_ManagerGoals` FOREIGN KEY (`ManagerGoalId`) REFERENCES `employmentmanagergoals` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_EmploymentTaskConditions_ScheduledRules` FOREIGN KEY (`ScheduledTaskRuleId`) REFERENCES `employmentscheduledtaskrules` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_EmploymentTaskConditions_ScheduledRuleTemplates` FOREIGN KEY (`ScheduledRuleTemplateId`) REFERENCES `employmentscheduledruletemplates` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Dumping data for table employmenttaskconditions
--

/*!40000 ALTER TABLE `employmenttaskconditions` DISABLE KEYS */;
/*!40000 ALTER TABLE `employmenttaskconditions` ENABLE KEYS */;

--
-- Definition of entitydescriptions
--

DROP TABLE IF EXISTS `entitydescriptions`;
CREATE TABLE IF NOT EXISTS `entitydescriptions` (
  `Id` bigint NOT NULL AUTO_INCREMENT,
  `ShortDescription` varchar(4000) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci DEFAULT NULL,
  `FullDescription` varchar(4000) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci DEFAULT NULL,
  `DisplaySex` smallint NOT NULL,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Dumping data for table entitydescriptions
--

/*!40000 ALTER TABLE `entitydescriptions` DISABLE KEYS */;
/*!40000 ALTER TABLE `entitydescriptions` ENABLE KEYS */;

--
-- Definition of entitydescriptionpatterns_entitydescriptions
--

DROP TABLE IF EXISTS `entitydescriptionpatterns_entitydescriptions`;
CREATE TABLE IF NOT EXISTS `entitydescriptionpatterns_entitydescriptions` (
  `PatternId` bigint NOT NULL,
  `EntityDescriptionId` bigint NOT NULL,
  PRIMARY KEY (`PatternId`,`EntityDescriptionId`),
  KEY `FK_EDP_EntityDescriptions_EntityDescriptions` (`EntityDescriptionId`),
  CONSTRAINT `FK_EDP_EntityDescriptions_EntityDescriptionPatterns` FOREIGN KEY (`PatternId`) REFERENCES `entitydescriptionpatterns` (`Id`) ON DELETE RESTRICT,
  CONSTRAINT `FK_EDP_EntityDescriptions_EntityDescriptions` FOREIGN KEY (`EntityDescriptionId`) REFERENCES `entitydescriptions` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Dumping data for table entitydescriptionpatterns_entitydescriptions
--

/*!40000 ALTER TABLE `entitydescriptionpatterns_entitydescriptions` DISABLE KEYS */;
/*!40000 ALTER TABLE `entitydescriptionpatterns_entitydescriptions` ENABLE KEYS */;

--
-- Definition of exits
--

DROP TABLE IF EXISTS `exits`;
CREATE TABLE IF NOT EXISTS `exits` (
  `Id` bigint NOT NULL AUTO_INCREMENT,
  `Keywords1` varchar(255) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci DEFAULT NULL,
  `Keywords2` varchar(255) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci DEFAULT NULL,
  `CellId1` bigint NOT NULL,
  `CellId2` bigint NOT NULL,
  `DoorId` bigint DEFAULT NULL,
  `Direction1` int NOT NULL,
  `Direction2` int NOT NULL,
  `TimeMultiplier` double NOT NULL,
  `InboundDescription1` varchar(255) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci DEFAULT NULL,
  `InboundDescription2` varchar(255) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci DEFAULT NULL,
  `OutboundDescription1` varchar(255) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci DEFAULT NULL,
  `OutboundDescription2` varchar(255) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci DEFAULT NULL,
  `InboundTarget1` varchar(255) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci DEFAULT NULL,
  `InboundTarget2` varchar(255) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci DEFAULT NULL,
  `OutboundTarget1` varchar(255) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci DEFAULT NULL,
  `OutboundTarget2` varchar(255) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci DEFAULT NULL,
  `Verb1` varchar(255) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci DEFAULT NULL,
  `Verb2` varchar(255) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci DEFAULT NULL,
  `PrimaryKeyword1` varchar(255) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci DEFAULT NULL,
  `PrimaryKeyword2` varchar(255) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci DEFAULT NULL,
  `AcceptsDoor` bit(1) NOT NULL,
  `DoorSize` int DEFAULT NULL,
  `MaximumSizeToEnter` int NOT NULL DEFAULT '12',
  `MaximumSizeToEnterUpright` int NOT NULL DEFAULT '12',
  `FallCell` bigint DEFAULT NULL,
  `IsClimbExit` bit(1) NOT NULL DEFAULT b'0',
  `ClimbDifficulty` int NOT NULL DEFAULT '5',
  `BlockedLayers` varchar(255) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL DEFAULT '',
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Dumping data for table exits
--

/*!40000 ALTER TABLE `exits` DISABLE KEYS */;
/*!40000 ALTER TABLE `exits` ENABLE KEYS */;

--
-- Definition of celloverlays_exits
--

DROP TABLE IF EXISTS `celloverlays_exits`;
CREATE TABLE IF NOT EXISTS `celloverlays_exits` (
  `CellOverlayId` bigint NOT NULL,
  `ExitId` bigint NOT NULL,
  PRIMARY KEY (`CellOverlayId`,`ExitId`),
  KEY `FK_CellOverlays_Exits_Exits` (`ExitId`),
  CONSTRAINT `FK_CellOverlays_Exits_CellOverlays` FOREIGN KEY (`CellOverlayId`) REFERENCES `celloverlays` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_CellOverlays_Exits_Exits` FOREIGN KEY (`ExitId`) REFERENCES `exits` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Dumping data for table celloverlays_exits
--

/*!40000 ALTER TABLE `celloverlays_exits` DISABLE KEYS */;
/*!40000 ALTER TABLE `celloverlays_exits` ENABLE KEYS */;

--
-- Definition of foragableprofiles
--

DROP TABLE IF EXISTS `foragableprofiles`;
CREATE TABLE IF NOT EXISTS `foragableprofiles` (
  `Id` bigint NOT NULL,
  `RevisionNumber` int NOT NULL,
  `EditableItemId` bigint NOT NULL,
  `Name` varchar(200) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL,
  PRIMARY KEY (`Id`,`RevisionNumber`),
  KEY `FK_ForagableProfiles_EditableItems_idx` (`EditableItemId`),
  CONSTRAINT `FK_ForagableProfiles_EditableItems` FOREIGN KEY (`EditableItemId`) REFERENCES `editableitems` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Dumping data for table foragableprofiles
--

/*!40000 ALTER TABLE `foragableprofiles` DISABLE KEYS */;
/*!40000 ALTER TABLE `foragableprofiles` ENABLE KEYS */;

--
-- Definition of foragableprofiles_foragables
--

DROP TABLE IF EXISTS `foragableprofiles_foragables`;
CREATE TABLE IF NOT EXISTS `foragableprofiles_foragables` (
  `ForagableProfileId` bigint NOT NULL,
  `ForagableProfileRevisionNumber` int NOT NULL,
  `ForagableId` bigint NOT NULL,
  PRIMARY KEY (`ForagableProfileId`,`ForagableProfileRevisionNumber`,`ForagableId`),
  CONSTRAINT `FK_ForagableProfiles_Foragables_ForagableProfiles` FOREIGN KEY (`ForagableProfileId`, `ForagableProfileRevisionNumber`) REFERENCES `foragableprofiles` (`Id`, `RevisionNumber`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Dumping data for table foragableprofiles_foragables
--

/*!40000 ALTER TABLE `foragableprofiles_foragables` DISABLE KEYS */;
/*!40000 ALTER TABLE `foragableprofiles_foragables` ENABLE KEYS */;

--
-- Definition of foragableprofiles_hourlyyieldgains
--

DROP TABLE IF EXISTS `foragableprofiles_hourlyyieldgains`;
CREATE TABLE IF NOT EXISTS `foragableprofiles_hourlyyieldgains` (
  `ForagableProfileId` bigint NOT NULL,
  `ForagableProfileRevisionNumber` int NOT NULL,
  `ForageType` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL,
  `Yield` double NOT NULL,
  PRIMARY KEY (`ForagableProfileId`,`ForagableProfileRevisionNumber`,`ForageType`),
  CONSTRAINT `FK_ForagableProfiles_HourlyYieldGains_ForagableProfiles` FOREIGN KEY (`ForagableProfileId`, `ForagableProfileRevisionNumber`) REFERENCES `foragableprofiles` (`Id`, `RevisionNumber`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Dumping data for table foragableprofiles_hourlyyieldgains
--

/*!40000 ALTER TABLE `foragableprofiles_hourlyyieldgains` DISABLE KEYS */;
/*!40000 ALTER TABLE `foragableprofiles_hourlyyieldgains` ENABLE KEYS */;

--
-- Definition of foragableprofiles_maximumyields
--

DROP TABLE IF EXISTS `foragableprofiles_maximumyields`;
CREATE TABLE IF NOT EXISTS `foragableprofiles_maximumyields` (
  `ForagableProfileId` bigint NOT NULL,
  `ForagableProfileRevisionNumber` int NOT NULL,
  `ForageType` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL,
  `Yield` double NOT NULL,
  PRIMARY KEY (`ForagableProfileId`,`ForagableProfileRevisionNumber`,`ForageType`),
  CONSTRAINT `FK_ForagableProfiles_MaximumYields_ForagableProfiles` FOREIGN KEY (`ForagableProfileId`, `ForagableProfileRevisionNumber`) REFERENCES `foragableprofiles` (`Id`, `RevisionNumber`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Dumping data for table foragableprofiles_maximumyields
--

/*!40000 ALTER TABLE `foragableprofiles_maximumyields` DISABLE KEYS */;
/*!40000 ALTER TABLE `foragableprofiles_maximumyields` ENABLE KEYS */;

--
-- Definition of foragables
--

DROP TABLE IF EXISTS `foragables`;
CREATE TABLE IF NOT EXISTS `foragables` (
  `Id` bigint NOT NULL,
  `RevisionNumber` int NOT NULL,
  `Name` varchar(200) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL,
  `ForagableTypes` text CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL,
  `ForageDifficulty` int NOT NULL,
  `RelativeChance` int NOT NULL,
  `MinimumOutcome` int NOT NULL,
  `MaximumOutcome` int NOT NULL,
  `QuantityDiceExpression` varchar(200) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL,
  `ItemProtoId` bigint NOT NULL,
  `OnForageProgId` bigint DEFAULT NULL,
  `CanForageProgId` bigint DEFAULT NULL,
  `EditableItemId` bigint NOT NULL,
  `CommodityMaterialId` bigint DEFAULT NULL,
  `CommodityTagId` bigint DEFAULT NULL,
  `CommodityWeightExpression` varchar(200) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  PRIMARY KEY (`Id`,`RevisionNumber`),
  KEY `FK_Foragables_EditableItems` (`EditableItemId`),
  CONSTRAINT `FK_Foragables_EditableItems` FOREIGN KEY (`EditableItemId`) REFERENCES `editableitems` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Dumping data for table foragables
--

/*!40000 ALTER TABLE `foragables` DISABLE KEYS */;
/*!40000 ALTER TABLE `foragables` ENABLE KEYS */;

--
-- Definition of futureprogs
--

DROP TABLE IF EXISTS `futureprogs`;
CREATE TABLE IF NOT EXISTS `futureprogs` (
  `Id` bigint NOT NULL AUTO_INCREMENT,
  `FunctionName` varchar(255) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `FunctionComment` text CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `FunctionText` text CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NOT NULL,
  `Category` varchar(255) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `Subcategory` varchar(255) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `Public` bit(1) NOT NULL DEFAULT b'0',
  `AcceptsAnyParameters` bit(1) NOT NULL DEFAULT b'0',
  `StaticType` int NOT NULL,
  `ReturnTypeDefinition` varchar(255) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL DEFAULT '',
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Dumping data for table futureprogs
--

/*!40000 ALTER TABLE `futureprogs` DISABLE KEYS */;
/*!40000 ALTER TABLE `futureprogs` ENABLE KEYS */;

--
-- Definition of aistorytellers
--

DROP TABLE IF EXISTS `aistorytellers`;
CREATE TABLE IF NOT EXISTS `aistorytellers` (
  `Id` bigint NOT NULL AUTO_INCREMENT,
  `Name` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  `Description` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  `Model` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  `SystemPrompt` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  `AttentionAgentPrompt` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  `SurveillanceStrategyDefinition` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  `ReasoningEffort` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  `CustomToolCallsDefinition` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  `SubscribeTo5mHeartbeat` tinyint(1) NOT NULL,
  `SubscribeTo10mHeartbeat` tinyint(1) NOT NULL,
  `SubscribeTo30mHeartbeat` tinyint(1) NOT NULL,
  `SubscribeToHourHeartbeat` tinyint(1) NOT NULL,
  `HeartbeatStatus5mProgId` bigint DEFAULT NULL,
  `HeartbeatStatus10mProgId` bigint DEFAULT NULL,
  `HeartbeatStatus30mProgId` bigint DEFAULT NULL,
  `HeartbeatStatus1hProgId` bigint DEFAULT NULL,
  `IsPaused` tinyint(1) NOT NULL,
  `SubscribeToRoomEvents` tinyint(1) NOT NULL,
  `SubscribeToCrimeEvents` tinyint(1) NOT NULL DEFAULT '0',
  `SubscribeToSpeechEvents` tinyint(1) NOT NULL DEFAULT '0',
  `SubscribeToStateEvents` tinyint(1) NOT NULL DEFAULT '0',
  `TimeSystemPrompt` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  `AttentionClassifierModel` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  `AttentionClassifierReasoningEffort` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  `TimeModel` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  `TimeReasoningEffort` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  PRIMARY KEY (`Id`),
  KEY `IX_AIStorytellers_HeartbeatStatus10mProgId` (`HeartbeatStatus10mProgId`),
  KEY `IX_AIStorytellers_HeartbeatStatus1hProgId` (`HeartbeatStatus1hProgId`),
  KEY `IX_AIStorytellers_HeartbeatStatus30mProgId` (`HeartbeatStatus30mProgId`),
  KEY `IX_AIStorytellers_HeartbeatStatus5mProgId` (`HeartbeatStatus5mProgId`),
  CONSTRAINT `FK_AIStorytellers_FutureProgs_HeartbeatStatus10mProgId` FOREIGN KEY (`HeartbeatStatus10mProgId`) REFERENCES `futureprogs` (`Id`),
  CONSTRAINT `FK_AIStorytellers_FutureProgs_HeartbeatStatus1hProgId` FOREIGN KEY (`HeartbeatStatus1hProgId`) REFERENCES `futureprogs` (`Id`),
  CONSTRAINT `FK_AIStorytellers_FutureProgs_HeartbeatStatus30mProgId` FOREIGN KEY (`HeartbeatStatus30mProgId`) REFERENCES `futureprogs` (`Id`),
  CONSTRAINT `FK_AIStorytellers_FutureProgs_HeartbeatStatus5mProgId` FOREIGN KEY (`HeartbeatStatus5mProgId`) REFERENCES `futureprogs` (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Dumping data for table aistorytellers
--

/*!40000 ALTER TABLE `aistorytellers` DISABLE KEYS */;
/*!40000 ALTER TABLE `aistorytellers` ENABLE KEYS */;

--
-- Definition of appointments
--

DROP TABLE IF EXISTS `appointments`;
CREATE TABLE IF NOT EXISTS `appointments` (
  `Id` bigint NOT NULL AUTO_INCREMENT,
  `Name` varchar(100) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `MaximumSimultaneousHolders` int NOT NULL DEFAULT '1',
  `MinimumRankId` bigint DEFAULT NULL,
  `ParentAppointmentId` bigint DEFAULT NULL,
  `PaygradeId` bigint DEFAULT NULL,
  `InsigniaGameItemId` bigint DEFAULT NULL,
  `InsigniaGameItemRevnum` int DEFAULT NULL,
  `ClanId` bigint NOT NULL,
  `Privileges` bigint NOT NULL,
  `MinimumRankToAppointId` bigint DEFAULT NULL,
  `CanNominateProgId` bigint DEFAULT NULL,
  `ElectionLeadTimeMinutes` double DEFAULT NULL,
  `ElectionTermMinutes` double DEFAULT NULL,
  `IsAppointedByElection` bit(1) NOT NULL DEFAULT b'0',
  `IsSecretBallot` bit(1) DEFAULT NULL,
  `MaximumConsecutiveTerms` int DEFAULT NULL,
  `MaximumTotalTerms` int DEFAULT NULL,
  `NominationPeriodMinutes` double DEFAULT NULL,
  `NumberOfVotesProgId` bigint DEFAULT NULL,
  `VotingPeriodMinutes` double DEFAULT NULL,
  `WhyCantNominateProgId` bigint DEFAULT NULL,
  `FameType` int NOT NULL DEFAULT '0',
  PRIMARY KEY (`Id`),
  KEY `FK_Appointments_Clans` (`ClanId`),
  KEY `FK_Appointments_Ranks` (`MinimumRankId`),
  KEY `FK_Appointments_Ranks_2` (`MinimumRankToAppointId`),
  KEY `FK_Appointments_ParentAppointment` (`ParentAppointmentId`),
  KEY `FK_Appointments_Paygrades` (`PaygradeId`),
  KEY `FK_Appointments_GameItemProtos` (`InsigniaGameItemId`,`InsigniaGameItemRevnum`),
  KEY `FK_Appointments_CanNominateProg_idx` (`CanNominateProgId`),
  KEY `FK_Appointments_NumberOfVotesProg_idx` (`NumberOfVotesProgId`),
  KEY `FK_Appointments_WhyCantNominateProg_idx` (`WhyCantNominateProgId`),
  CONSTRAINT `FK_Appointments_CanNominateProg` FOREIGN KEY (`CanNominateProgId`) REFERENCES `futureprogs` (`Id`) ON DELETE RESTRICT,
  CONSTRAINT `FK_Appointments_Clans` FOREIGN KEY (`ClanId`) REFERENCES `clans` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_Appointments_GameItemProtos` FOREIGN KEY (`InsigniaGameItemId`, `InsigniaGameItemRevnum`) REFERENCES `gameitemprotos` (`Id`, `RevisionNumber`) ON DELETE RESTRICT,
  CONSTRAINT `FK_Appointments_NumberOfVotesProg` FOREIGN KEY (`NumberOfVotesProgId`) REFERENCES `futureprogs` (`Id`) ON DELETE RESTRICT,
  CONSTRAINT `FK_Appointments_ParentAppointment` FOREIGN KEY (`ParentAppointmentId`) REFERENCES `appointments` (`Id`) ON DELETE RESTRICT,
  CONSTRAINT `FK_Appointments_Paygrades` FOREIGN KEY (`PaygradeId`) REFERENCES `paygrades` (`Id`) ON DELETE SET NULL,
  CONSTRAINT `FK_Appointments_Ranks` FOREIGN KEY (`MinimumRankId`) REFERENCES `ranks` (`Id`) ON DELETE SET NULL,
  CONSTRAINT `FK_Appointments_Ranks_2` FOREIGN KEY (`MinimumRankToAppointId`) REFERENCES `ranks` (`Id`) ON DELETE RESTRICT,
  CONSTRAINT `FK_Appointments_WhyCantNominateProg` FOREIGN KEY (`WhyCantNominateProgId`) REFERENCES `futureprogs` (`Id`) ON DELETE RESTRICT
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Dumping data for table appointments
--

/*!40000 ALTER TABLE `appointments` DISABLE KEYS */;
/*!40000 ALTER TABLE `appointments` ENABLE KEYS */;

--
-- Definition of appointments_abbreviations
--

DROP TABLE IF EXISTS `appointments_abbreviations`;
CREATE TABLE IF NOT EXISTS `appointments_abbreviations` (
  `AppointmentId` bigint NOT NULL,
  `Abbreviation` varchar(50) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `FutureProgId` bigint DEFAULT NULL,
  `Order` int NOT NULL,
  PRIMARY KEY (`Abbreviation`,`AppointmentId`),
  KEY `FK_Appointments_Abbreviations_Appointments` (`AppointmentId`),
  KEY `FK_Appointments_Abbreviations_FutureProgs` (`FutureProgId`),
  CONSTRAINT `FK_Appointments_Abbreviations_Appointments` FOREIGN KEY (`AppointmentId`) REFERENCES `appointments` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_Appointments_Abbreviations_FutureProgs` FOREIGN KEY (`FutureProgId`) REFERENCES `futureprogs` (`Id`) ON DELETE RESTRICT
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Dumping data for table appointments_abbreviations
--

/*!40000 ALTER TABLE `appointments_abbreviations` DISABLE KEYS */;
/*!40000 ALTER TABLE `appointments_abbreviations` ENABLE KEYS */;

--
-- Definition of appointments_titles
--

DROP TABLE IF EXISTS `appointments_titles`;
CREATE TABLE IF NOT EXISTS `appointments_titles` (
  `AppointmentId` bigint NOT NULL,
  `Title` varchar(200) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `FutureProgId` bigint DEFAULT NULL,
  `Order` int DEFAULT NULL,
  PRIMARY KEY (`Title`,`AppointmentId`),
  KEY `FK_Appointments_Titles_Appointments` (`AppointmentId`),
  KEY `FK_Appointments_Titles_FutureProgs` (`FutureProgId`),
  CONSTRAINT `FK_Appointments_Titles_Appointments` FOREIGN KEY (`AppointmentId`) REFERENCES `appointments` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_Appointments_Titles_FutureProgs` FOREIGN KEY (`FutureProgId`) REFERENCES `futureprogs` (`Id`) ON DELETE RESTRICT
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Dumping data for table appointments_titles
--

/*!40000 ALTER TABLE `appointments_titles` DISABLE KEYS */;
/*!40000 ALTER TABLE `appointments_titles` ENABLE KEYS */;

--
-- Definition of arenacombatantclasses
--

DROP TABLE IF EXISTS `arenacombatantclasses`;
CREATE TABLE IF NOT EXISTS `arenacombatantclasses` (
  `Id` bigint NOT NULL AUTO_INCREMENT,
  `ArenaId` bigint NOT NULL,
  `Name` varchar(200) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `Description` varchar(4000) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `EligibilityProgId` bigint NOT NULL,
  `AdminNpcLoaderProgId` bigint DEFAULT NULL,
  `ResurrectNpcOnDeath` bit(1) NOT NULL DEFAULT b'0',
  `DefaultSignatureColour` varchar(50) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci DEFAULT NULL,
  `FullyRestoreNpcOnCompletion` bit(1) NOT NULL DEFAULT b'0',
  `DefaultStageNameProfileId` bigint DEFAULT NULL,
  PRIMARY KEY (`Id`),
  KEY `FK_ArenaCombatantClasses_AdminNpcLoaderProg` (`AdminNpcLoaderProgId`),
  KEY `FK_ArenaCombatantClasses_Arenas` (`ArenaId`),
  KEY `FK_ArenaCombatantClasses_EligibilityProg` (`EligibilityProgId`),
  KEY `FK_ArenaCombatantClasses_DefaultStageNameProfile` (`DefaultStageNameProfileId`),
  CONSTRAINT `FK_ArenaCombatantClasses_AdminNpcLoaderProg` FOREIGN KEY (`AdminNpcLoaderProgId`) REFERENCES `futureprogs` (`Id`) ON DELETE SET NULL,
  CONSTRAINT `FK_ArenaCombatantClasses_Arenas` FOREIGN KEY (`ArenaId`) REFERENCES `arenas` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_ArenaCombatantClasses_DefaultStageNameProfile` FOREIGN KEY (`DefaultStageNameProfileId`) REFERENCES `randomnameprofiles` (`Id`) ON DELETE SET NULL,
  CONSTRAINT `FK_ArenaCombatantClasses_EligibilityProg` FOREIGN KEY (`EligibilityProgId`) REFERENCES `futureprogs` (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Dumping data for table arenacombatantclasses
--

/*!40000 ALTER TABLE `arenacombatantclasses` DISABLE KEYS */;
/*!40000 ALTER TABLE `arenacombatantclasses` ENABLE KEYS */;

--
-- Definition of arenaeventsides
--

DROP TABLE IF EXISTS `arenaeventsides`;
CREATE TABLE IF NOT EXISTS `arenaeventsides` (
  `Id` bigint NOT NULL AUTO_INCREMENT,
  `ArenaEventId` bigint NOT NULL,
  `SideIndex` int NOT NULL,
  `Capacity` int NOT NULL,
  `Policy` int NOT NULL,
  `AllowNpcSignup` bit(1) NOT NULL DEFAULT b'0',
  `AutoFillNpc` bit(1) NOT NULL DEFAULT b'0',
  `OutfitProgId` bigint DEFAULT NULL,
  `NpcLoaderProgId` bigint DEFAULT NULL,
  PRIMARY KEY (`Id`),
  KEY `FK_ArenaEventSides_ArenaEvents` (`ArenaEventId`),
  KEY `FK_ArenaEventSides_NpcLoaderProg` (`NpcLoaderProgId`),
  KEY `FK_ArenaEventSides_OutfitProg` (`OutfitProgId`),
  CONSTRAINT `FK_ArenaEventSides_ArenaEvents` FOREIGN KEY (`ArenaEventId`) REFERENCES `arenaevents` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_ArenaEventSides_NpcLoaderProg` FOREIGN KEY (`NpcLoaderProgId`) REFERENCES `futureprogs` (`Id`) ON DELETE SET NULL,
  CONSTRAINT `FK_ArenaEventSides_OutfitProg` FOREIGN KEY (`OutfitProgId`) REFERENCES `futureprogs` (`Id`) ON DELETE SET NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Dumping data for table arenaeventsides
--

/*!40000 ALTER TABLE `arenaeventsides` DISABLE KEYS */;
/*!40000 ALTER TABLE `arenaeventsides` ENABLE KEYS */;

--
-- Definition of arenaeventtypes
--

DROP TABLE IF EXISTS `arenaeventtypes`;
CREATE TABLE IF NOT EXISTS `arenaeventtypes` (
  `Id` bigint NOT NULL AUTO_INCREMENT,
  `ArenaId` bigint NOT NULL,
  `Name` varchar(200) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `BringYourOwn` bit(1) NOT NULL DEFAULT b'0',
  `RegistrationDurationSeconds` int NOT NULL,
  `PreparationDurationSeconds` int NOT NULL,
  `TimeLimitSeconds` int DEFAULT NULL,
  `BettingModel` int NOT NULL,
  `AppearanceFee` decimal(58,29) NOT NULL,
  `VictoryFee` decimal(58,29) NOT NULL,
  `IntroProgId` bigint DEFAULT NULL,
  `ScoringProgId` bigint DEFAULT NULL,
  `ResolutionOverrideProgId` bigint DEFAULT NULL,
  `AutoScheduleIntervalSeconds` int DEFAULT NULL,
  `AutoScheduleReferenceTime` datetime DEFAULT NULL,
  `AllowSurrender` bit(1) NOT NULL DEFAULT b'1',
  `EliminationMode` int NOT NULL DEFAULT '0',
  `EloKFactor` decimal(58,29) NOT NULL DEFAULT '32.00000000000000000000000000000',
  `EloStyle` int NOT NULL DEFAULT '0',
  `PayNpcAppearanceFee` bit(1) NOT NULL DEFAULT b'0',
  PRIMARY KEY (`Id`),
  KEY `FK_ArenaEventTypes_Arenas` (`ArenaId`),
  KEY `FK_ArenaEventTypes_IntroProg` (`IntroProgId`),
  KEY `FK_ArenaEventTypes_ResolutionProg` (`ResolutionOverrideProgId`),
  KEY `FK_ArenaEventTypes_ScoringProg` (`ScoringProgId`),
  CONSTRAINT `FK_ArenaEventTypes_Arenas` FOREIGN KEY (`ArenaId`) REFERENCES `arenas` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_ArenaEventTypes_IntroProg` FOREIGN KEY (`IntroProgId`) REFERENCES `futureprogs` (`Id`) ON DELETE SET NULL,
  CONSTRAINT `FK_ArenaEventTypes_ResolutionProg` FOREIGN KEY (`ResolutionOverrideProgId`) REFERENCES `futureprogs` (`Id`) ON DELETE SET NULL,
  CONSTRAINT `FK_ArenaEventTypes_ScoringProg` FOREIGN KEY (`ScoringProgId`) REFERENCES `futureprogs` (`Id`) ON DELETE SET NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Dumping data for table arenaeventtypes
--

/*!40000 ALTER TABLE `arenaeventtypes` DISABLE KEYS */;
/*!40000 ALTER TABLE `arenaeventtypes` ENABLE KEYS */;

--
-- Definition of arenaevents
--

DROP TABLE IF EXISTS `arenaevents`;
CREATE TABLE IF NOT EXISTS `arenaevents` (
  `Id` bigint NOT NULL AUTO_INCREMENT,
  `ArenaId` bigint NOT NULL,
  `ArenaEventTypeId` bigint NOT NULL,
  `State` int NOT NULL,
  `BringYourOwn` bit(1) NOT NULL DEFAULT b'0',
  `RegistrationDurationSeconds` int NOT NULL,
  `PreparationDurationSeconds` int NOT NULL,
  `TimeLimitSeconds` int DEFAULT NULL,
  `BettingModel` int NOT NULL,
  `AppearanceFee` decimal(58,29) NOT NULL,
  `VictoryFee` decimal(58,29) NOT NULL,
  `CreatedAt` datetime NOT NULL,
  `ScheduledAt` datetime NOT NULL,
  `RegistrationOpensAt` datetime DEFAULT NULL,
  `StartedAt` datetime DEFAULT NULL,
  `ResolvedAt` datetime DEFAULT NULL,
  `CompletedAt` datetime DEFAULT NULL,
  `AbortedAt` datetime DEFAULT NULL,
  `CancellationReason` varchar(4000) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci DEFAULT NULL,
  `PayNpcAppearanceFee` bit(1) NOT NULL DEFAULT b'0',
  PRIMARY KEY (`Id`),
  KEY `FK_ArenaEvents_Arenas` (`ArenaId`),
  KEY `FK_ArenaEvents_EventTypes` (`ArenaEventTypeId`),
  CONSTRAINT `FK_ArenaEvents_Arenas` FOREIGN KEY (`ArenaId`) REFERENCES `arenas` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_ArenaEvents_EventTypes` FOREIGN KEY (`ArenaEventTypeId`) REFERENCES `arenaeventtypes` (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Dumping data for table arenaevents
--

/*!40000 ALTER TABLE `arenaevents` DISABLE KEYS */;
/*!40000 ALTER TABLE `arenaevents` ENABLE KEYS */;

--
-- Definition of arenabetpools
--

DROP TABLE IF EXISTS `arenabetpools`;
CREATE TABLE IF NOT EXISTS `arenabetpools` (
  `Id` bigint NOT NULL AUTO_INCREMENT,
  `ArenaEventId` bigint NOT NULL,
  `SideIndex` int DEFAULT NULL,
  `TotalStake` decimal(58,29) NOT NULL,
  `TakeRate` decimal(58,29) NOT NULL,
  PRIMARY KEY (`Id`),
  KEY `FK_ArenaBetPools_ArenaEvents` (`ArenaEventId`),
  CONSTRAINT `FK_ArenaBetPools_ArenaEvents` FOREIGN KEY (`ArenaEventId`) REFERENCES `arenaevents` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Dumping data for table arenabetpools
--

/*!40000 ALTER TABLE `arenabetpools` DISABLE KEYS */;
/*!40000 ALTER TABLE `arenabetpools` ENABLE KEYS */;

--
-- Definition of arenaeventtypesides
--

DROP TABLE IF EXISTS `arenaeventtypesides`;
CREATE TABLE IF NOT EXISTS `arenaeventtypesides` (
  `Id` bigint NOT NULL AUTO_INCREMENT,
  `ArenaEventTypeId` bigint NOT NULL,
  `Index` int NOT NULL,
  `Capacity` int NOT NULL,
  `Policy` int NOT NULL,
  `AllowNpcSignup` bit(1) NOT NULL DEFAULT b'0',
  `AutoFillNpc` bit(1) NOT NULL DEFAULT b'0',
  `OutfitProgId` bigint DEFAULT NULL,
  `NpcLoaderProgId` bigint DEFAULT NULL,
  `MaximumRating` decimal(58,29) DEFAULT NULL,
  `MinimumRating` decimal(58,29) DEFAULT NULL,
  PRIMARY KEY (`Id`),
  KEY `FK_ArenaEventTypeSides_EventTypes` (`ArenaEventTypeId`),
  KEY `FK_ArenaEventTypeSides_NpcLoaderProg` (`NpcLoaderProgId`),
  KEY `FK_ArenaEventTypeSides_OutfitProg` (`OutfitProgId`),
  CONSTRAINT `FK_ArenaEventTypeSides_EventTypes` FOREIGN KEY (`ArenaEventTypeId`) REFERENCES `arenaeventtypes` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_ArenaEventTypeSides_NpcLoaderProg` FOREIGN KEY (`NpcLoaderProgId`) REFERENCES `futureprogs` (`Id`) ON DELETE SET NULL,
  CONSTRAINT `FK_ArenaEventTypeSides_OutfitProg` FOREIGN KEY (`OutfitProgId`) REFERENCES `futureprogs` (`Id`) ON DELETE SET NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Dumping data for table arenaeventtypesides
--

/*!40000 ALTER TABLE `arenaeventtypesides` DISABLE KEYS */;
/*!40000 ALTER TABLE `arenaeventtypesides` ENABLE KEYS */;

--
-- Definition of arenaeventtypesideallowedclasses
--

DROP TABLE IF EXISTS `arenaeventtypesideallowedclasses`;
CREATE TABLE IF NOT EXISTS `arenaeventtypesideallowedclasses` (
  `ArenaEventTypeSideId` bigint NOT NULL,
  `ArenaCombatantClassId` bigint NOT NULL,
  PRIMARY KEY (`ArenaEventTypeSideId`,`ArenaCombatantClassId`),
  KEY `IX_ArenaEventTypeSideAllowedClasses_ArenaCombatantClassId` (`ArenaCombatantClassId`),
  CONSTRAINT `FK_ArenaEventTypeSideAllowedClasses_Classes` FOREIGN KEY (`ArenaCombatantClassId`) REFERENCES `arenacombatantclasses` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_ArenaEventTypeSideAllowedClasses_Sides` FOREIGN KEY (`ArenaEventTypeSideId`) REFERENCES `arenaeventtypesides` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Dumping data for table arenaeventtypesideallowedclasses
--

/*!40000 ALTER TABLE `arenaeventtypesideallowedclasses` DISABLE KEYS */;
/*!40000 ALTER TABLE `arenaeventtypesideallowedclasses` ENABLE KEYS */;

--
-- Definition of arenaratings
--

DROP TABLE IF EXISTS `arenaratings`;
CREATE TABLE IF NOT EXISTS `arenaratings` (
  `Id` bigint NOT NULL AUTO_INCREMENT,
  `ArenaId` bigint NOT NULL,
  `CharacterId` bigint NOT NULL,
  `CombatantClassId` bigint NOT NULL,
  `Rating` decimal(58,29) NOT NULL,
  `LastUpdatedAt` datetime NOT NULL,
  PRIMARY KEY (`Id`),
  UNIQUE KEY `UX_ArenaRatings_UniqueParticipant` (`ArenaId`,`CharacterId`,`CombatantClassId`),
  KEY `FK_ArenaRatings_Arenas` (`ArenaId`),
  KEY `FK_ArenaRatings_Characters` (`CharacterId`),
  KEY `FK_ArenaRatings_CombatantClasses` (`CombatantClassId`),
  CONSTRAINT `FK_ArenaRatings_Arenas` FOREIGN KEY (`ArenaId`) REFERENCES `arenas` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_ArenaRatings_Characters` FOREIGN KEY (`CharacterId`) REFERENCES `characters` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_ArenaRatings_CombatantClasses` FOREIGN KEY (`CombatantClassId`) REFERENCES `arenacombatantclasses` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Dumping data for table arenaratings
--

/*!40000 ALTER TABLE `arenaratings` DISABLE KEYS */;
/*!40000 ALTER TABLE `arenaratings` ENABLE KEYS */;

--
-- Definition of arenas
--

DROP TABLE IF EXISTS `arenas`;
CREATE TABLE IF NOT EXISTS `arenas` (
  `Id` bigint NOT NULL AUTO_INCREMENT,
  `Name` varchar(200) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `EconomicZoneId` bigint NOT NULL,
  `CurrencyId` bigint NOT NULL,
  `BankAccountId` bigint DEFAULT NULL,
  `VirtualBalance` decimal(58,29) NOT NULL,
  `CreatedAt` datetime NOT NULL,
  `IsDeleted` bit(1) NOT NULL DEFAULT b'0',
  `SignupEcho` varchar(4000) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci DEFAULT NULL,
  `OnArenaEventPhaseProgId` bigint DEFAULT NULL,
  PRIMARY KEY (`Id`),
  KEY `FK_Arenas_BankAccounts` (`BankAccountId`),
  KEY `FK_Arenas_Currencies` (`CurrencyId`),
  KEY `FK_Arenas_EconomicZones` (`EconomicZoneId`),
  KEY `FK_Arenas_OnArenaEventPhaseProg` (`OnArenaEventPhaseProgId`),
  CONSTRAINT `FK_Arenas_BankAccounts` FOREIGN KEY (`BankAccountId`) REFERENCES `bankaccounts` (`Id`) ON DELETE SET NULL,
  CONSTRAINT `FK_Arenas_Currencies` FOREIGN KEY (`CurrencyId`) REFERENCES `currencies` (`Id`),
  CONSTRAINT `FK_Arenas_EconomicZones` FOREIGN KEY (`EconomicZoneId`) REFERENCES `economiczones` (`Id`),
  CONSTRAINT `FK_Arenas_OnArenaEventPhaseProg` FOREIGN KEY (`OnArenaEventPhaseProgId`) REFERENCES `futureprogs` (`Id`) ON DELETE SET NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Dumping data for table arenas
--

/*!40000 ALTER TABLE `arenas` DISABLE KEYS */;
/*!40000 ALTER TABLE `arenas` ENABLE KEYS */;

--
-- Definition of arenafinancesnapshots
--

DROP TABLE IF EXISTS `arenafinancesnapshots`;
CREATE TABLE IF NOT EXISTS `arenafinancesnapshots` (
  `Id` bigint NOT NULL AUTO_INCREMENT,
  `ArenaId` bigint NOT NULL,
  `ArenaEventId` bigint DEFAULT NULL,
  `Period` varchar(100) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci DEFAULT NULL,
  `Revenue` decimal(58,29) NOT NULL,
  `Costs` decimal(58,29) NOT NULL,
  `TaxWithheld` decimal(58,29) NOT NULL,
  `Profit` decimal(58,29) NOT NULL,
  `CreatedAt` datetime NOT NULL,
  PRIMARY KEY (`Id`),
  KEY `FK_ArenaFinanceSnapshots_ArenaEvents` (`ArenaEventId`),
  KEY `FK_ArenaFinanceSnapshots_Arenas` (`ArenaId`),
  CONSTRAINT `FK_ArenaFinanceSnapshots_ArenaEvents` FOREIGN KEY (`ArenaEventId`) REFERENCES `arenaevents` (`Id`) ON DELETE SET NULL,
  CONSTRAINT `FK_ArenaFinanceSnapshots_Arenas` FOREIGN KEY (`ArenaId`) REFERENCES `arenas` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Dumping data for table arenafinancesnapshots
--

/*!40000 ALTER TABLE `arenafinancesnapshots` DISABLE KEYS */;
/*!40000 ALTER TABLE `arenafinancesnapshots` ENABLE KEYS */;

--
-- Definition of bankaccounttypes
--

DROP TABLE IF EXISTS `bankaccounttypes`;
CREATE TABLE IF NOT EXISTS `bankaccounttypes` (
  `Id` bigint NOT NULL AUTO_INCREMENT,
  `Name` varchar(255) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `CustomerDescription` varchar(1000) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
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
  `BankId` bigint NOT NULL,
  `CanOpenAccountProgCharacterId` bigint DEFAULT NULL,
  `CanOpenAccountProgClanId` bigint DEFAULT NULL,
  `CanOpenAccountProgShopId` bigint DEFAULT NULL,
  `CanCloseAccountProgId` bigint DEFAULT NULL,
  `NumberOfPermittedPaymentItems` int NOT NULL DEFAULT '0',
  `PaymentItemPrototypeId` bigint DEFAULT NULL,
  PRIMARY KEY (`Id`),
  KEY `IX_BankAccountTypes_BankId` (`BankId`),
  KEY `IX_BankAccountTypes_CanCloseAccountProgId` (`CanCloseAccountProgId`),
  KEY `IX_BankAccountTypes_CanOpenAccountProgCharacterId` (`CanOpenAccountProgCharacterId`),
  KEY `IX_BankAccountTypes_CanOpenAccountProgClanId` (`CanOpenAccountProgClanId`),
  KEY `IX_BankAccountTypes_CanOpenAccountProgShopId` (`CanOpenAccountProgShopId`),
  CONSTRAINT `FK_BankAccountTypes_Banks` FOREIGN KEY (`BankId`) REFERENCES `banks` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_BankAccountTypes_CanCloseProg` FOREIGN KEY (`CanCloseAccountProgId`) REFERENCES `futureprogs` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_BankAccountTypes_CharacterProgs` FOREIGN KEY (`CanOpenAccountProgCharacterId`) REFERENCES `futureprogs` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_BankAccountTypes_ClanProgs` FOREIGN KEY (`CanOpenAccountProgClanId`) REFERENCES `futureprogs` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_BankAccountTypes_ShopProgs` FOREIGN KEY (`CanOpenAccountProgShopId`) REFERENCES `futureprogs` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Dumping data for table bankaccounttypes
--

/*!40000 ALTER TABLE `bankaccounttypes` DISABLE KEYS */;
/*!40000 ALTER TABLE `bankaccounttypes` ENABLE KEYS */;

--
-- Definition of butcheryproducts
--

DROP TABLE IF EXISTS `butcheryproducts`;
CREATE TABLE IF NOT EXISTS `butcheryproducts` (
  `Id` bigint NOT NULL AUTO_INCREMENT,
  `Name` varchar(100) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `TargetBodyId` bigint NOT NULL,
  `IsPelt` bit(1) NOT NULL,
  `Subcategory` varchar(100) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `CanProduceProgId` bigint DEFAULT NULL,
  PRIMARY KEY (`Id`),
  KEY `FK_ButcheryProducts_FutureProgs_idx` (`CanProduceProgId`),
  KEY `FK_ButcheryProducts_BodyProtos_idx` (`TargetBodyId`),
  CONSTRAINT `FK_ButcheryProducts_BodyProtos` FOREIGN KEY (`TargetBodyId`) REFERENCES `bodyprotos` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_ButcheryProducts_FutureProgs` FOREIGN KEY (`CanProduceProgId`) REFERENCES `futureprogs` (`Id`) ON DELETE SET NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Dumping data for table butcheryproducts
--

/*!40000 ALTER TABLE `butcheryproducts` DISABLE KEYS */;
/*!40000 ALTER TABLE `butcheryproducts` ENABLE KEYS */;

--
-- Definition of butcheryproductitems
--

DROP TABLE IF EXISTS `butcheryproductitems`;
CREATE TABLE IF NOT EXISTS `butcheryproductitems` (
  `Id` bigint NOT NULL AUTO_INCREMENT,
  `ButcheryProductId` bigint NOT NULL,
  `NormalProtoId` bigint NOT NULL,
  `DamagedProtoId` bigint DEFAULT NULL,
  `NormalQuantity` int NOT NULL,
  `DamagedQuantity` int NOT NULL,
  `ButcheryProductItemscol` varchar(45) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci DEFAULT NULL,
  `DamageThreshold` double NOT NULL DEFAULT '10',
  PRIMARY KEY (`Id`),
  KEY `FK_ButcheryProductItems_ButcheryProducts_idx` (`ButcheryProductId`),
  CONSTRAINT `FK_ButcheryProductItems_ButcheryProducts` FOREIGN KEY (`ButcheryProductId`) REFERENCES `butcheryproducts` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Dumping data for table butcheryproductitems
--

/*!40000 ALTER TABLE `butcheryproductitems` DISABLE KEYS */;
/*!40000 ALTER TABLE `butcheryproductitems` ENABLE KEYS */;

--
-- Definition of butcheryproducts_bodypartprotos
--

DROP TABLE IF EXISTS `butcheryproducts_bodypartprotos`;
CREATE TABLE IF NOT EXISTS `butcheryproducts_bodypartprotos` (
  `ButcheryProductId` bigint NOT NULL,
  `BodypartProtoId` bigint NOT NULL,
  PRIMARY KEY (`ButcheryProductId`,`BodypartProtoId`),
  KEY `FK_ButcheryProducts_BodypartProtos_BodypartProtos_idx` (`BodypartProtoId`),
  CONSTRAINT `FK_ButcheryProducts_BodypartProtos_BodypartProtos` FOREIGN KEY (`BodypartProtoId`) REFERENCES `bodypartproto` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_ButcheryProducts_BodypartProtos_ButcheryProducts` FOREIGN KEY (`ButcheryProductId`) REFERENCES `butcheryproducts` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Dumping data for table butcheryproducts_bodypartprotos
--

/*!40000 ALTER TABLE `butcheryproducts_bodypartprotos` DISABLE KEYS */;
/*!40000 ALTER TABLE `butcheryproducts_bodypartprotos` ENABLE KEYS */;

--
-- Definition of channels
--

DROP TABLE IF EXISTS `channels`;
CREATE TABLE IF NOT EXISTS `channels` (
  `Id` bigint NOT NULL AUTO_INCREMENT,
  `ChannelName` varchar(4000) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `ChannelListenerProgId` bigint NOT NULL,
  `ChannelSpeakerProgId` bigint NOT NULL,
  `AnnounceChannelJoiners` bit(1) NOT NULL,
  `ChannelColour` char(10) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `Mode` int NOT NULL,
  `AnnounceMissedListeners` bit(1) NOT NULL,
  `AddToPlayerCommandTree` bit(1) NOT NULL DEFAULT b'0',
  `AddToGuideCommandTree` bit(1) NOT NULL DEFAULT b'0',
  `DiscordChannelId` bigint unsigned DEFAULT NULL,
  PRIMARY KEY (`Id`),
  KEY `FK_Channels_FutureProgs_Listener` (`ChannelListenerProgId`),
  KEY `FK_Channels_FutureProgs_Speaker` (`ChannelSpeakerProgId`),
  CONSTRAINT `FK_Channels_FutureProgs_Listener` FOREIGN KEY (`ChannelListenerProgId`) REFERENCES `futureprogs` (`Id`) ON DELETE RESTRICT,
  CONSTRAINT `FK_Channels_FutureProgs_Speaker` FOREIGN KEY (`ChannelSpeakerProgId`) REFERENCES `futureprogs` (`Id`) ON DELETE RESTRICT
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Dumping data for table channels
--

/*!40000 ALTER TABLE `channels` DISABLE KEYS */;
/*!40000 ALTER TABLE `channels` ENABLE KEYS */;

--
-- Definition of channelcommandwords
--

DROP TABLE IF EXISTS `channelcommandwords`;
CREATE TABLE IF NOT EXISTS `channelcommandwords` (
  `Word` varchar(50) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `ChannelId` bigint NOT NULL,
  PRIMARY KEY (`Word`),
  KEY `FK_ChannelCommandWords_Channels` (`ChannelId`),
  CONSTRAINT `FK_ChannelCommandWords_Channels` FOREIGN KEY (`ChannelId`) REFERENCES `channels` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Dumping data for table channelcommandwords
--

/*!40000 ALTER TABLE `channelcommandwords` DISABLE KEYS */;
/*!40000 ALTER TABLE `channelcommandwords` ENABLE KEYS */;

--
-- Definition of channelignorers
--

DROP TABLE IF EXISTS `channelignorers`;
CREATE TABLE IF NOT EXISTS `channelignorers` (
  `ChannelId` bigint NOT NULL,
  `AccountId` bigint NOT NULL,
  PRIMARY KEY (`ChannelId`,`AccountId`),
  KEY `FK_ChannelIgnorers_Accounts` (`AccountId`),
  CONSTRAINT `FK_ChannelIgnorers_Accounts` FOREIGN KEY (`AccountId`) REFERENCES `accounts` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_ChannelIgnorers_Channels` FOREIGN KEY (`ChannelId`) REFERENCES `channels` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Dumping data for table channelignorers
--

/*!40000 ALTER TABLE `channelignorers` DISABLE KEYS */;
/*!40000 ALTER TABLE `channelignorers` ENABLE KEYS */;

--
-- Definition of characterbodies
--

DROP TABLE IF EXISTS `characterbodies`;
CREATE TABLE IF NOT EXISTS `characterbodies` (
  `CharacterId` bigint NOT NULL,
  `BodyId` bigint NOT NULL,
  `Alias` varchar(255) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `SortOrder` int NOT NULL,
  `AllowVoluntarySwitch` bit(1) NOT NULL DEFAULT b'0',
  `CanVoluntarilySwitchProgId` bigint DEFAULT NULL,
  `WhyCannotVoluntarilySwitchProgId` bigint DEFAULT NULL,
  `CanSeeFormProgId` bigint DEFAULT NULL,
  `TraumaMode` int NOT NULL DEFAULT '0',
  `TransformationEcho` text CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci,
  PRIMARY KEY (`CharacterId`,`BodyId`),
  KEY `FK_CharacterBodies_Bodies_idx` (`BodyId`),
  KEY `FK_CharacterBodies_CanVoluntarilySwitchProg_idx` (`CanVoluntarilySwitchProgId`),
  KEY `FK_CharacterBodies_WhyCannotVoluntarilySwitchProg_idx` (`WhyCannotVoluntarilySwitchProgId`),
  KEY `FK_CharacterBodies_CanSeeFormProg_idx` (`CanSeeFormProgId`),
  CONSTRAINT `FK_CharacterBodies_Bodies` FOREIGN KEY (`BodyId`) REFERENCES `bodies` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_CharacterBodies_CanSeeFormProg` FOREIGN KEY (`CanSeeFormProgId`) REFERENCES `futureprogs` (`Id`) ON DELETE SET NULL,
  CONSTRAINT `FK_CharacterBodies_CanVoluntarilySwitchProg` FOREIGN KEY (`CanVoluntarilySwitchProgId`) REFERENCES `futureprogs` (`Id`) ON DELETE SET NULL,
  CONSTRAINT `FK_CharacterBodies_Characters` FOREIGN KEY (`CharacterId`) REFERENCES `characters` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_CharacterBodies_WhyCannotVoluntarilySwitchProg` FOREIGN KEY (`WhyCannotVoluntarilySwitchProgId`) REFERENCES `futureprogs` (`Id`) ON DELETE SET NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Dumping data for table characterbodies
--

/*!40000 ALTER TABLE `characterbodies` DISABLE KEYS */;
/*!40000 ALTER TABLE `characterbodies` ENABLE KEYS */;

--
-- Definition of charactercombatsettings
--

DROP TABLE IF EXISTS `charactercombatsettings`;
CREATE TABLE IF NOT EXISTS `charactercombatsettings` (
  `Id` bigint NOT NULL AUTO_INCREMENT,
  `Name` text CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL,
  `Description` text CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL,
  `GlobalTemplate` bit(1) NOT NULL DEFAULT b'0',
  `AvailabilityProgId` bigint DEFAULT NULL,
  `CharacterOwnerId` bigint DEFAULT NULL,
  `WeaponUsePercentage` double NOT NULL,
  `MagicUsePercentage` double NOT NULL,
  `PsychicUsePercentage` double NOT NULL,
  `NaturalWeaponPercentage` double NOT NULL,
  `AuxiliaryPercentage` double NOT NULL,
  `PreferToFightArmed` bit(1) NOT NULL,
  `PreferFavouriteWeapon` bit(1) NOT NULL DEFAULT b'0',
  `PreferShieldUse` bit(1) NOT NULL,
  `ClassificationsAllowed` text CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL,
  `RequiredIntentions` bigint NOT NULL,
  `ForbiddenIntentions` bigint NOT NULL,
  `PreferredIntentions` bigint NOT NULL,
  `AttackUnarmed` bit(1) NOT NULL DEFAULT b'0',
  `FallbackToUnarmedIfNoWeapon` bit(1) NOT NULL DEFAULT b'0',
  `AttackCriticallyInjured` bit(1) NOT NULL DEFAULT b'0',
  `SkirmishToOtherLocations` bit(1) NOT NULL DEFAULT b'0',
  `PursuitMode` int NOT NULL DEFAULT '1',
  `DefaultPreferredDefenseType` int NOT NULL,
  `PreferredMeleeMode` int NOT NULL,
  `PreferredRangedMode` int NOT NULL DEFAULT '1',
  `AutomaticallyMoveTowardsTarget` bit(1) NOT NULL DEFAULT b'0',
  `PreferNonContactClinchBreaking` bit(1) NOT NULL,
  `InventoryManagement` int NOT NULL,
  `MovementManagement` int NOT NULL,
  `RangedManagement` int NOT NULL,
  `ManualPositionManagement` bit(1) NOT NULL DEFAULT b'0',
  `MinimumStaminaToAttack` double NOT NULL,
  `MoveToMeleeIfCannotEngageInRangedCombat` bit(1) NOT NULL,
  `PreferredWeaponSetup` int NOT NULL,
  `RequiredMinimumAim` double NOT NULL DEFAULT '0.5',
  `MeleeAttackOrderPreference` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL DEFAULT '0 1 2 3 4',
  `GrappleResponse` int NOT NULL,
  `AttackHelpless` bit(1) NOT NULL DEFAULT b'0',
  `PriorityProgId` bigint DEFAULT NULL,
  `PreferTerrestrialCombat` bit(1) NOT NULL DEFAULT b'1',
  PRIMARY KEY (`Id`),
  KEY `FK_CharacterCombatSettings_FutureProgs_idx` (`AvailabilityProgId`),
  KEY `FK_CharacterCombatSettings_Characters_idx` (`CharacterOwnerId`),
  KEY `IX_CharacterCombatSettings_PriorityProgId` (`PriorityProgId`),
  CONSTRAINT `FK_CharacterCombatSettings_Characters` FOREIGN KEY (`CharacterOwnerId`) REFERENCES `characters` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_CharacterCombatSettings_FutureProgs` FOREIGN KEY (`AvailabilityProgId`) REFERENCES `futureprogs` (`Id`) ON DELETE SET NULL,
  CONSTRAINT `FK_CharacterCombatSettings_PriorityProg` FOREIGN KEY (`PriorityProgId`) REFERENCES `futureprogs` (`Id`) ON DELETE SET NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Dumping data for table charactercombatsettings
--

/*!40000 ALTER TABLE `charactercombatsettings` DISABLE KEYS */;
/*!40000 ALTER TABLE `charactercombatsettings` ENABLE KEYS */;

--
-- Definition of characterintrotemplates
--

DROP TABLE IF EXISTS `characterintrotemplates`;
CREATE TABLE IF NOT EXISTS `characterintrotemplates` (
  `Id` bigint NOT NULL AUTO_INCREMENT,
  `Name` varchar(200) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `ResolutionPriority` int NOT NULL DEFAULT '1',
  `AppliesToCharacterProgId` bigint NOT NULL,
  `Definition` text CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `Order` int NOT NULL,
  PRIMARY KEY (`Id`),
  KEY `FK_CharacterIntroTemplates_FutureProgs_idx` (`AppliesToCharacterProgId`),
  CONSTRAINT `FK_CharacterIntroTemplates_FutureProgs` FOREIGN KEY (`AppliesToCharacterProgId`) REFERENCES `futureprogs` (`Id`) ON DELETE RESTRICT
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Dumping data for table characterintrotemplates
--

/*!40000 ALTER TABLE `characterintrotemplates` DISABLE KEYS */;
/*!40000 ALTER TABLE `characterintrotemplates` ENABLE KEYS */;

--
-- Definition of characteristicvalues
--

DROP TABLE IF EXISTS `characteristicvalues`;
CREATE TABLE IF NOT EXISTS `characteristicvalues` (
  `Id` bigint NOT NULL AUTO_INCREMENT,
  `Name` varchar(4000) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `DefinitionId` bigint NOT NULL,
  `Value` varchar(50) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci DEFAULT NULL,
  `Default` bit(1) NOT NULL DEFAULT b'0',
  `AdditionalValue` varchar(4000) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci DEFAULT NULL,
  `FutureProgId` bigint DEFAULT NULL,
  `Pluralisation` int NOT NULL,
  `OngoingValidityProgId` bigint DEFAULT NULL,
  PRIMARY KEY (`Id`),
  KEY `FK_CharacteristicValues_CharacteristicDefinitions` (`DefinitionId`),
  KEY `FK_CharacteristicValues_FutureProgs` (`FutureProgId`),
  KEY `IX_CharacteristicValues_OngoingValidityProgId` (`OngoingValidityProgId`),
  CONSTRAINT `FK_CharacteristicValues_CharacteristicDefinitions` FOREIGN KEY (`DefinitionId`) REFERENCES `characteristicdefinitions` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_CharacteristicValues_FutureProgs` FOREIGN KEY (`FutureProgId`) REFERENCES `futureprogs` (`Id`) ON DELETE SET NULL,
  CONSTRAINT `FK_CharacteristicValues_FutureProgs_Ongoing` FOREIGN KEY (`OngoingValidityProgId`) REFERENCES `futureprogs` (`Id`) ON DELETE SET NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Dumping data for table characteristicvalues
--

/*!40000 ALTER TABLE `characteristicvalues` DISABLE KEYS */;
/*!40000 ALTER TABLE `characteristicvalues` ENABLE KEYS */;

--
-- Definition of characteristics
--

DROP TABLE IF EXISTS `characteristics`;
CREATE TABLE IF NOT EXISTS `characteristics` (
  `BodyId` bigint NOT NULL,
  `Type` int NOT NULL,
  `CharacteristicId` bigint NOT NULL,
  PRIMARY KEY (`BodyId`,`Type`),
  KEY `FK_Characteristics_CharacteristicValues` (`CharacteristicId`),
  CONSTRAINT `FK_Characteristics_Bodies` FOREIGN KEY (`BodyId`) REFERENCES `bodies` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_Characteristics_CharacteristicValues` FOREIGN KEY (`CharacteristicId`) REFERENCES `characteristicvalues` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Dumping data for table characteristics
--

/*!40000 ALTER TABLE `characteristics` DISABLE KEYS */;
/*!40000 ALTER TABLE `characteristics` ENABLE KEYS */;

--
-- Definition of chargenadvices
--

DROP TABLE IF EXISTS `chargenadvices`;
CREATE TABLE IF NOT EXISTS `chargenadvices` (
  `Id` bigint NOT NULL AUTO_INCREMENT,
  `ChargenStage` int NOT NULL,
  `AdviceTitle` text CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `AdviceText` text CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `ShouldShowAdviceProgId` bigint DEFAULT NULL,
  PRIMARY KEY (`Id`),
  KEY `FK_ChargenAdvices_FutureProgs_idx` (`ShouldShowAdviceProgId`),
  CONSTRAINT `FK_ChargenAdvices_FutureProgs` FOREIGN KEY (`ShouldShowAdviceProgId`) REFERENCES `futureprogs` (`Id`) ON DELETE SET NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Dumping data for table chargenadvices
--

/*!40000 ALTER TABLE `chargenadvices` DISABLE KEYS */;
/*!40000 ALTER TABLE `chargenadvices` ENABLE KEYS */;

--
-- Definition of chargenresources
--

DROP TABLE IF EXISTS `chargenresources`;
CREATE TABLE IF NOT EXISTS `chargenresources` (
  `Id` bigint NOT NULL AUTO_INCREMENT,
  `Name` varchar(50) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `PluralName` varchar(50) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `Alias` varchar(50) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `MinimumTimeBetweenAwards` int NOT NULL,
  `MaximumNumberAwardedPerAward` double NOT NULL,
  `PermissionLevelRequiredToAward` int NOT NULL,
  `PermissionLevelRequiredToCircumventMinimumTime` int NOT NULL,
  `ShowToPlayerInScore` bit(1) NOT NULL,
  `TextDisplayedToPlayerOnAward` varchar(4000) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `TextDisplayedToPlayerOnDeduct` varchar(4000) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `MaximumResourceId` bigint DEFAULT NULL,
  `MaximumResourceFormula` varchar(4000) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `Type` varchar(50) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `ControlProgId` bigint DEFAULT NULL,
  `FK_ChargenResources_FutureProgs` bigint DEFAULT NULL,
  PRIMARY KEY (`Id`),
  KEY `IX_ChargenResources_FK_ChargenResources_FutureProgs` (`FK_ChargenResources_FutureProgs`),
  CONSTRAINT `FK_ChargenResources_FutureProgs_FK_ChargenResources_FutureProgs` FOREIGN KEY (`FK_ChargenResources_FutureProgs`) REFERENCES `futureprogs` (`Id`) ON DELETE SET NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Dumping data for table chargenresources
--

/*!40000 ALTER TABLE `chargenresources` DISABLE KEYS */;
/*!40000 ALTER TABLE `chargenresources` ENABLE KEYS */;

--
-- Definition of accounts_chargenresources
--

DROP TABLE IF EXISTS `accounts_chargenresources`;
CREATE TABLE IF NOT EXISTS `accounts_chargenresources` (
  `AccountId` bigint NOT NULL,
  `ChargenResourceId` bigint NOT NULL,
  `Amount` double NOT NULL,
  `LastAwardDate` datetime NOT NULL,
  PRIMARY KEY (`AccountId`,`ChargenResourceId`),
  KEY `FK_Accounts_ChargenResources_ChargenResources` (`ChargenResourceId`),
  CONSTRAINT `FK_Accounts_ChargenResources_Accounts` FOREIGN KEY (`AccountId`) REFERENCES `accounts` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_Accounts_ChargenResources_ChargenResources` FOREIGN KEY (`ChargenResourceId`) REFERENCES `chargenresources` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Dumping data for table accounts_chargenresources
--

/*!40000 ALTER TABLE `accounts_chargenresources` DISABLE KEYS */;
/*!40000 ALTER TABLE `accounts_chargenresources` ENABLE KEYS */;

--
-- Definition of chargenroles
--

DROP TABLE IF EXISTS `chargenroles`;
CREATE TABLE IF NOT EXISTS `chargenroles` (
  `Id` bigint NOT NULL AUTO_INCREMENT,
  `Name` varchar(4000) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `Type` int NOT NULL,
  `PosterId` bigint NOT NULL,
  `MaximumNumberAlive` int NOT NULL,
  `MaximumNumberTotal` int NOT NULL,
  `ChargenBlurb` varchar(4000) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `AvailabilityProgId` bigint DEFAULT NULL,
  `Expired` bit(1) NOT NULL DEFAULT b'0',
  `MinimumAuthorityToApprove` int NOT NULL,
  `MinimumAuthorityToView` int NOT NULL,
  PRIMARY KEY (`Id`),
  KEY `FK_ChargenRoles_FutureProgs` (`AvailabilityProgId`),
  KEY `FK_ChargenRoles_Accounts` (`PosterId`),
  CONSTRAINT `FK_ChargenRoles_Accounts` FOREIGN KEY (`PosterId`) REFERENCES `accounts` (`Id`) ON DELETE RESTRICT,
  CONSTRAINT `FK_ChargenRoles_FutureProgs` FOREIGN KEY (`AvailabilityProgId`) REFERENCES `futureprogs` (`Id`) ON DELETE SET NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Dumping data for table chargenroles
--

/*!40000 ALTER TABLE `chargenroles` DISABLE KEYS */;
/*!40000 ALTER TABLE `chargenroles` ENABLE KEYS */;

--
-- Definition of characters_chargenroles
--

DROP TABLE IF EXISTS `characters_chargenroles`;
CREATE TABLE IF NOT EXISTS `characters_chargenroles` (
  `CharacterId` bigint NOT NULL,
  `ChargenRoleId` bigint NOT NULL,
  PRIMARY KEY (`CharacterId`,`ChargenRoleId`),
  KEY `FK_Characters_ChargenRoles_ChargenRoles` (`ChargenRoleId`),
  CONSTRAINT `FK_Characters_ChargenRoles_Characters` FOREIGN KEY (`CharacterId`) REFERENCES `characters` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_Characters_ChargenRoles_ChargenRoles` FOREIGN KEY (`ChargenRoleId`) REFERENCES `chargenroles` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Dumping data for table characters_chargenroles
--

/*!40000 ALTER TABLE `characters_chargenroles` DISABLE KEYS */;
/*!40000 ALTER TABLE `characters_chargenroles` ENABLE KEYS */;

--
-- Definition of chargenadvices_chargenroles
--

DROP TABLE IF EXISTS `chargenadvices_chargenroles`;
CREATE TABLE IF NOT EXISTS `chargenadvices_chargenroles` (
  `ChargenAdviceId` bigint NOT NULL,
  `ChargenRoleId` bigint NOT NULL,
  PRIMARY KEY (`ChargenAdviceId`,`ChargenRoleId`),
  KEY `FK_ChargenAdvices_ChargenRoles_ChargenRoles_idx` (`ChargenRoleId`),
  CONSTRAINT `FK_ChargenAdvices_ChargenRoles_ChargenAdvices` FOREIGN KEY (`ChargenAdviceId`) REFERENCES `chargenadvices` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_ChargenAdvices_ChargenRoles_ChargenRoles` FOREIGN KEY (`ChargenRoleId`) REFERENCES `chargenroles` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Dumping data for table chargenadvices_chargenroles
--

/*!40000 ALTER TABLE `chargenadvices_chargenroles` DISABLE KEYS */;
/*!40000 ALTER TABLE `chargenadvices_chargenroles` ENABLE KEYS */;

--
-- Definition of chargenroles_approvers
--

DROP TABLE IF EXISTS `chargenroles_approvers`;
CREATE TABLE IF NOT EXISTS `chargenroles_approvers` (
  `ChargenRoleId` bigint NOT NULL,
  `ApproverId` bigint NOT NULL,
  PRIMARY KEY (`ChargenRoleId`,`ApproverId`),
  KEY `FK_ChargenRoles_Approvers_Accounts` (`ApproverId`),
  CONSTRAINT `FK_ChargenRoles_Approvers_Accounts` FOREIGN KEY (`ApproverId`) REFERENCES `accounts` (`Id`) ON DELETE RESTRICT,
  CONSTRAINT `FK_ChargenRoles_Approvers_ChargenRoles` FOREIGN KEY (`ChargenRoleId`) REFERENCES `chargenroles` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Dumping data for table chargenroles_approvers
--

/*!40000 ALTER TABLE `chargenroles_approvers` DISABLE KEYS */;
/*!40000 ALTER TABLE `chargenroles_approvers` ENABLE KEYS */;

--
-- Definition of chargenroles_costs
--

DROP TABLE IF EXISTS `chargenroles_costs`;
CREATE TABLE IF NOT EXISTS `chargenroles_costs` (
  `ChargenRoleId` bigint NOT NULL,
  `ChargenResourceId` bigint NOT NULL,
  `RequirementOnly` bit(1) NOT NULL DEFAULT b'0',
  `Amount` int NOT NULL,
  PRIMARY KEY (`ChargenRoleId`,`ChargenResourceId`,`RequirementOnly`),
  KEY `FK_ChargenRoles_Costs_ChargenResources` (`ChargenResourceId`),
  CONSTRAINT `FK_ChargenRoles_Costs_ChargenResources` FOREIGN KEY (`ChargenResourceId`) REFERENCES `chargenresources` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_ChargenRoles_Costs_ChargenRoles` FOREIGN KEY (`ChargenRoleId`) REFERENCES `chargenroles` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Dumping data for table chargenroles_costs
--

/*!40000 ALTER TABLE `chargenroles_costs` DISABLE KEYS */;
/*!40000 ALTER TABLE `chargenroles_costs` ENABLE KEYS */;

--
-- Definition of clans
--

DROP TABLE IF EXISTS `clans`;
CREATE TABLE IF NOT EXISTS `clans` (
  `Id` bigint NOT NULL AUTO_INCREMENT,
  `Name` varchar(100) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `Alias` varchar(100) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `FullName` varchar(4000) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci DEFAULT NULL,
  `Description` varchar(4000) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci DEFAULT NULL,
  `ClanId` bigint DEFAULT NULL,
  `PayIntervalType` int NOT NULL,
  `PayIntervalModifier` int NOT NULL,
  `PayIntervalOther` int NOT NULL,
  `CalendarId` bigint NOT NULL,
  `PayIntervalReferenceDate` varchar(100) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `PayIntervalReferenceTime` varchar(100) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `IsTemplate` bit(1) NOT NULL DEFAULT b'0',
  `ShowClanMembersInWho` bit(1) NOT NULL DEFAULT b'0',
  `PaymasterId` bigint DEFAULT NULL,
  `PaymasterItemProtoId` bigint DEFAULT NULL,
  `OnPayProgId` bigint DEFAULT NULL,
  `MaximumPeriodsOfUncollectedBackPay` int DEFAULT NULL,
  `ShowFamousMembersInNotables` bit(1) NOT NULL DEFAULT b'0',
  `Sphere` varchar(100) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci DEFAULT NULL,
  `BankAccountId` bigint DEFAULT NULL,
  `DiscordChannelId` decimal(20,0) DEFAULT NULL,
  `PayIntervalFallback` int NOT NULL DEFAULT '0',
  `PayIntervalOtherSecondary` int NOT NULL DEFAULT '0',
  PRIMARY KEY (`Id`),
  KEY `FK_Clans_Calendars` (`CalendarId`),
  KEY `FK_Clans_FutureProgs_idx` (`OnPayProgId`),
  KEY `IX_Clans_ClanId` (`ClanId`),
  KEY `FK_Clans_Characters_idx` (`PaymasterId`),
  KEY `IX_Clans_BankAccountId` (`BankAccountId`),
  CONSTRAINT `FK_Clans_BankAccounts` FOREIGN KEY (`BankAccountId`) REFERENCES `bankaccounts` (`Id`),
  CONSTRAINT `FK_Clans_Calendars` FOREIGN KEY (`CalendarId`) REFERENCES `calendars` (`Id`) ON DELETE RESTRICT,
  CONSTRAINT `FK_Clans_Characters` FOREIGN KEY (`PaymasterId`) REFERENCES `characters` (`Id`) ON DELETE SET NULL,
  CONSTRAINT `FK_Clans_Clans_ClanId` FOREIGN KEY (`ClanId`) REFERENCES `clans` (`Id`) ON DELETE RESTRICT,
  CONSTRAINT `FK_Clans_FutureProgs` FOREIGN KEY (`OnPayProgId`) REFERENCES `futureprogs` (`Id`) ON DELETE SET NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Dumping data for table clans
--

/*!40000 ALTER TABLE `clans` DISABLE KEYS */;
/*!40000 ALTER TABLE `clans` ENABLE KEYS */;

--
-- Definition of arenareservations
--

DROP TABLE IF EXISTS `arenareservations`;
CREATE TABLE IF NOT EXISTS `arenareservations` (
  `Id` bigint NOT NULL AUTO_INCREMENT,
  `ArenaEventId` bigint NOT NULL,
  `SideIndex` int NOT NULL,
  `CharacterId` bigint DEFAULT NULL,
  `ClanId` bigint DEFAULT NULL,
  `ReservedAt` datetime NOT NULL,
  `ExpiresAt` datetime NOT NULL,
  PRIMARY KEY (`Id`),
  KEY `FK_ArenaReservations_ArenaEvents` (`ArenaEventId`),
  KEY `FK_ArenaReservations_Characters` (`CharacterId`),
  KEY `FK_ArenaReservations_Clans` (`ClanId`),
  CONSTRAINT `FK_ArenaReservations_ArenaEvents` FOREIGN KEY (`ArenaEventId`) REFERENCES `arenaevents` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_ArenaReservations_Characters` FOREIGN KEY (`CharacterId`) REFERENCES `characters` (`Id`) ON DELETE SET NULL,
  CONSTRAINT `FK_ArenaReservations_Clans` FOREIGN KEY (`ClanId`) REFERENCES `clans` (`Id`) ON DELETE SET NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Dumping data for table arenareservations
--

/*!40000 ALTER TABLE `arenareservations` DISABLE KEYS */;
/*!40000 ALTER TABLE `arenareservations` ENABLE KEYS */;

--
-- Definition of arenasignups
--

DROP TABLE IF EXISTS `arenasignups`;
CREATE TABLE IF NOT EXISTS `arenasignups` (
  `Id` bigint NOT NULL AUTO_INCREMENT,
  `ArenaEventId` bigint NOT NULL,
  `CharacterId` bigint NOT NULL,
  `CombatantClassId` bigint NOT NULL,
  `SideIndex` int NOT NULL,
  `IsNpc` bit(1) NOT NULL DEFAULT b'0',
  `StageName` varchar(200) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci DEFAULT NULL,
  `SignatureColour` varchar(50) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci DEFAULT NULL,
  `StartingRating` decimal(58,29) DEFAULT NULL,
  `SignedUpAt` datetime NOT NULL,
  `ArenaReservationId` bigint DEFAULT NULL,
  `ActiveCharacterInstanceId` bigint DEFAULT NULL,
  PRIMARY KEY (`Id`),
  KEY `FK_ArenaSignups_ArenaEvents` (`ArenaEventId`),
  KEY `FK_ArenaSignups_Characters` (`CharacterId`),
  KEY `FK_ArenaSignups_CombatantClasses` (`CombatantClassId`),
  KEY `FK_ArenaSignups_Reservations` (`ArenaReservationId`),
  KEY `FK_ArenaSignups_ActiveCharacterInstances` (`ActiveCharacterInstanceId`),
  CONSTRAINT `FK_ArenaSignups_ArenaEvents` FOREIGN KEY (`ArenaEventId`) REFERENCES `arenaevents` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_ArenaSignups_Characters` FOREIGN KEY (`CharacterId`) REFERENCES `characters` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_ArenaSignups_CombatantClasses` FOREIGN KEY (`CombatantClassId`) REFERENCES `arenacombatantclasses` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_ArenaSignups_Reservations` FOREIGN KEY (`ArenaReservationId`) REFERENCES `arenareservations` (`Id`) ON DELETE SET NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Dumping data for table arenasignups
--

/*!40000 ALTER TABLE `arenasignups` DISABLE KEYS */;
/*!40000 ALTER TABLE `arenasignups` ENABLE KEYS */;

--
-- Definition of arenaeliminations
--

DROP TABLE IF EXISTS `arenaeliminations`;
CREATE TABLE IF NOT EXISTS `arenaeliminations` (
  `Id` bigint NOT NULL AUTO_INCREMENT,
  `ArenaEventId` bigint NOT NULL,
  `ArenaSignupId` bigint NOT NULL,
  `CharacterId` bigint NOT NULL,
  `Reason` int NOT NULL,
  `OccurredAt` datetime NOT NULL,
  PRIMARY KEY (`Id`),
  KEY `FK_ArenaEliminations_ArenaEvents` (`ArenaEventId`),
  KEY `FK_ArenaEliminations_Characters` (`CharacterId`),
  KEY `FK_ArenaEliminations_Signups` (`ArenaSignupId`),
  CONSTRAINT `FK_ArenaEliminations_ArenaEvents` FOREIGN KEY (`ArenaEventId`) REFERENCES `arenaevents` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_ArenaEliminations_Characters` FOREIGN KEY (`CharacterId`) REFERENCES `characters` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_ArenaEliminations_Signups` FOREIGN KEY (`ArenaSignupId`) REFERENCES `arenasignups` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Dumping data for table arenaeliminations
--

/*!40000 ALTER TABLE `arenaeliminations` DISABLE KEYS */;
/*!40000 ALTER TABLE `arenaeliminations` ENABLE KEYS */;

--
-- Definition of chargenroles_clanmemberships
--

DROP TABLE IF EXISTS `chargenroles_clanmemberships`;
CREATE TABLE IF NOT EXISTS `chargenroles_clanmemberships` (
  `ChargenRoleId` bigint NOT NULL,
  `ClanId` bigint NOT NULL,
  `RankId` bigint NOT NULL,
  `PaygradeId` bigint DEFAULT NULL,
  PRIMARY KEY (`ChargenRoleId`,`ClanId`),
  KEY `FK_ChargenRoles_ClanMemberships_Clans` (`ClanId`),
  CONSTRAINT `FK_ChargenRoles_ClanMemberships_ChargenRoles` FOREIGN KEY (`ChargenRoleId`) REFERENCES `chargenroles` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_ChargenRoles_ClanMemberships_Clans` FOREIGN KEY (`ClanId`) REFERENCES `clans` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Dumping data for table chargenroles_clanmemberships
--

/*!40000 ALTER TABLE `chargenroles_clanmemberships` DISABLE KEYS */;
/*!40000 ALTER TABLE `chargenroles_clanmemberships` ENABLE KEYS */;

--
-- Definition of chargenroles_clanmemberships_appointments
--

DROP TABLE IF EXISTS `chargenroles_clanmemberships_appointments`;
CREATE TABLE IF NOT EXISTS `chargenroles_clanmemberships_appointments` (
  `ChargenRoleId` bigint NOT NULL,
  `ClanId` bigint NOT NULL,
  `AppointmentId` bigint NOT NULL,
  PRIMARY KEY (`ChargenRoleId`,`ClanId`,`AppointmentId`),
  CONSTRAINT `FK_CRCMA_ChargenRoles_ClanMemberships` FOREIGN KEY (`ChargenRoleId`, `ClanId`) REFERENCES `chargenroles_clanmemberships` (`ChargenRoleId`, `ClanId`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Dumping data for table chargenroles_clanmemberships_appointments
--

/*!40000 ALTER TABLE `chargenroles_clanmemberships_appointments` DISABLE KEYS */;
/*!40000 ALTER TABLE `chargenroles_clanmemberships_appointments` ENABLE KEYS */;

--
-- Definition of clans_administrationcells
--

DROP TABLE IF EXISTS `clans_administrationcells`;
CREATE TABLE IF NOT EXISTS `clans_administrationcells` (
  `ClanId` bigint NOT NULL,
  `CellId` bigint NOT NULL,
  PRIMARY KEY (`ClanId`,`CellId`),
  KEY `FK_Clans_AdministrationCells_Cells_idx` (`CellId`),
  CONSTRAINT `FK_Clans_AdministrationCells_Cells` FOREIGN KEY (`CellId`) REFERENCES `cells` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_Clans_AdministrationCells_Clans` FOREIGN KEY (`ClanId`) REFERENCES `clans` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Dumping data for table clans_administrationcells
--

/*!40000 ALTER TABLE `clans_administrationcells` DISABLE KEYS */;
/*!40000 ALTER TABLE `clans_administrationcells` ENABLE KEYS */;

--
-- Definition of clans_hallcells
--

DROP TABLE IF EXISTS `clans_hallcells`;
CREATE TABLE IF NOT EXISTS `clans_hallcells` (
  `ClanId` bigint NOT NULL,
  `CellId` bigint NOT NULL,
  PRIMARY KEY (`ClanId`,`CellId`),
  KEY `FK_Clans_HallCells_Cells_idx` (`CellId`),
  CONSTRAINT `FK_Clans_HallCells_Cells` FOREIGN KEY (`CellId`) REFERENCES `cells` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_Clans_HallCells_Clans` FOREIGN KEY (`ClanId`) REFERENCES `clans` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Dumping data for table clans_hallcells
--

/*!40000 ALTER TABLE `clans_hallcells` DISABLE KEYS */;
/*!40000 ALTER TABLE `clans_hallcells` ENABLE KEYS */;

--
-- Definition of clans_treasurycells
--

DROP TABLE IF EXISTS `clans_treasurycells`;
CREATE TABLE IF NOT EXISTS `clans_treasurycells` (
  `ClanId` bigint NOT NULL,
  `CellId` bigint NOT NULL,
  PRIMARY KEY (`ClanId`,`CellId`),
  KEY `FK_Clans_TreasuryCells_Cells_idx` (`CellId`),
  CONSTRAINT `FK_Clans_TreasuryCells_Cells` FOREIGN KEY (`CellId`) REFERENCES `cells` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_Clans_TreasuryCells_Clans` FOREIGN KEY (`ClanId`) REFERENCES `clans` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Dumping data for table clans_treasurycells
--

/*!40000 ALTER TABLE `clans_treasurycells` DISABLE KEYS */;
/*!40000 ALTER TABLE `clans_treasurycells` ENABLE KEYS */;

--
-- Definition of combatmessages
--

DROP TABLE IF EXISTS `combatmessages`;
CREATE TABLE IF NOT EXISTS `combatmessages` (
  `Id` bigint NOT NULL AUTO_INCREMENT,
  `Type` int NOT NULL,
  `Outcome` int DEFAULT NULL,
  `Message` text CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL,
  `ProgId` bigint DEFAULT NULL,
  `Priority` int NOT NULL,
  `Verb` int DEFAULT NULL,
  `Chance` double NOT NULL DEFAULT '1',
  `FailureMessage` text CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL,
  `AuxiliaryProgId` bigint DEFAULT NULL,
  PRIMARY KEY (`Id`),
  KEY `FK_CombatMessages_FutureProgs_idx` (`ProgId`),
  KEY `IX_CombatMessages_AuxiliaryProgId` (`AuxiliaryProgId`),
  CONSTRAINT `FK_CombatMessages_FutureProgs` FOREIGN KEY (`ProgId`) REFERENCES `futureprogs` (`Id`) ON DELETE SET NULL,
  CONSTRAINT `FK_CombatMessages_FutureProgs_Auxiliary` FOREIGN KEY (`AuxiliaryProgId`) REFERENCES `futureprogs` (`Id`) ON DELETE SET NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Dumping data for table combatmessages
--

/*!40000 ALTER TABLE `combatmessages` DISABLE KEYS */;
/*!40000 ALTER TABLE `combatmessages` ENABLE KEYS */;

--
-- Definition of cultures
--

DROP TABLE IF EXISTS `cultures`;
CREATE TABLE IF NOT EXISTS `cultures` (
  `Id` bigint NOT NULL AUTO_INCREMENT,
  `Name` varchar(255) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `Description` varchar(4000) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `PersonWordMale` varchar(255) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci DEFAULT NULL,
  `PersonWordFemale` varchar(255) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci DEFAULT NULL,
  `PersonWordNeuter` varchar(255) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci DEFAULT NULL,
  `PersonWordIndeterminate` varchar(255) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `PrimaryCalendarId` bigint NOT NULL,
  `SkillStartingValueProgId` bigint NOT NULL,
  `AvailabilityProgId` bigint DEFAULT NULL,
  `TolerableTemperatureFloorEffect` double NOT NULL,
  `TolerableTemperatureCeilingEffect` double NOT NULL,
  PRIMARY KEY (`Id`),
  KEY `FK_Cultures_AvailabilityProg` (`AvailabilityProgId`),
  KEY `FK_Cultures_SkillStartingProg` (`SkillStartingValueProgId`),
  CONSTRAINT `FK_Cultures_AvailabilityProg` FOREIGN KEY (`AvailabilityProgId`) REFERENCES `futureprogs` (`Id`) ON DELETE SET NULL,
  CONSTRAINT `FK_Cultures_SkillStartingProg` FOREIGN KEY (`SkillStartingValueProgId`) REFERENCES `futureprogs` (`Id`) ON DELETE RESTRICT
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Dumping data for table cultures
--

/*!40000 ALTER TABLE `cultures` DISABLE KEYS */;
/*!40000 ALTER TABLE `cultures` ENABLE KEYS */;

--
-- Definition of chargenadvices_cultures
--

DROP TABLE IF EXISTS `chargenadvices_cultures`;
CREATE TABLE IF NOT EXISTS `chargenadvices_cultures` (
  `ChargenAdviceId` bigint NOT NULL,
  `CultureId` bigint NOT NULL,
  PRIMARY KEY (`ChargenAdviceId`,`CultureId`),
  KEY `FK_ChargenAdvices_Cultures_Cultures_idx` (`CultureId`),
  CONSTRAINT `FK_ChargenAdvices_Cultures_ChargenAdvices` FOREIGN KEY (`ChargenAdviceId`) REFERENCES `chargenadvices` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_ChargenAdvices_Cultures_Cultures` FOREIGN KEY (`CultureId`) REFERENCES `cultures` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Dumping data for table chargenadvices_cultures
--

/*!40000 ALTER TABLE `chargenadvices_cultures` DISABLE KEYS */;
/*!40000 ALTER TABLE `chargenadvices_cultures` ENABLE KEYS */;

--
-- Definition of cultures_chargenresources
--

DROP TABLE IF EXISTS `cultures_chargenresources`;
CREATE TABLE IF NOT EXISTS `cultures_chargenresources` (
  `CultureId` bigint NOT NULL,
  `ChargenResourceId` bigint NOT NULL,
  `RequirementOnly` bit(1) NOT NULL DEFAULT b'0',
  `Amount` int NOT NULL,
  PRIMARY KEY (`CultureId`,`ChargenResourceId`),
  KEY `IX_Cultures_ChargenResources_ChargenResourceId` (`ChargenResourceId`),
  CONSTRAINT `FK_Cultures_ChargenResources_ChargenResources_ChargenResourceId` FOREIGN KEY (`ChargenResourceId`) REFERENCES `chargenresources` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_Cultures_ChargenResources_Cultures_CultureId` FOREIGN KEY (`CultureId`) REFERENCES `cultures` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Dumping data for table cultures_chargenresources
--

/*!40000 ALTER TABLE `cultures_chargenresources` DISABLE KEYS */;
/*!40000 ALTER TABLE `cultures_chargenresources` ENABLE KEYS */;

--
-- Definition of currencydescriptionpatterns
--

DROP TABLE IF EXISTS `currencydescriptionpatterns`;
CREATE TABLE IF NOT EXISTS `currencydescriptionpatterns` (
  `Id` bigint NOT NULL AUTO_INCREMENT,
  `Type` int NOT NULL,
  `CurrencyId` bigint NOT NULL,
  `FutureProgId` bigint DEFAULT NULL,
  `NegativePrefix` varchar(4000) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `Order` int NOT NULL,
  `UseNaturalAggregationStyle` bit(1) NOT NULL DEFAULT b'0',
  PRIMARY KEY (`Id`),
  KEY `FK_CurrencyDescriptionPatterns_Currencies` (`CurrencyId`),
  KEY `FK_CurrencyDescriptionPatterns_FutureProgs` (`FutureProgId`),
  CONSTRAINT `FK_CurrencyDescriptionPatterns_Currencies` FOREIGN KEY (`CurrencyId`) REFERENCES `currencies` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_CurrencyDescriptionPatterns_FutureProgs` FOREIGN KEY (`FutureProgId`) REFERENCES `futureprogs` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Dumping data for table currencydescriptionpatterns
--

/*!40000 ALTER TABLE `currencydescriptionpatterns` DISABLE KEYS */;
/*!40000 ALTER TABLE `currencydescriptionpatterns` ENABLE KEYS */;

--
-- Definition of dreams
--

DROP TABLE IF EXISTS `dreams`;
CREATE TABLE IF NOT EXISTS `dreams` (
  `Id` bigint NOT NULL AUTO_INCREMENT,
  `Name` varchar(200) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL,
  `CanDreamProgId` bigint DEFAULT NULL,
  `OnDreamProgId` bigint DEFAULT NULL,
  `OnWakeDuringDreamingProgId` bigint DEFAULT NULL,
  `OnlyOnce` bit(1) NOT NULL DEFAULT b'0',
  `Priority` int NOT NULL DEFAULT '100',
  PRIMARY KEY (`Id`),
  KEY `FK_Dreams_FutureProgs_CanDream_idx` (`CanDreamProgId`),
  KEY `FK_Dreams_FutureProgs_OnDream_idx` (`OnDreamProgId`),
  KEY `FK_Dreams_FutureProgs_OnWake_idx` (`OnWakeDuringDreamingProgId`),
  CONSTRAINT `FK_Dreams_FutureProgs_CanDream` FOREIGN KEY (`CanDreamProgId`) REFERENCES `futureprogs` (`Id`) ON DELETE SET NULL,
  CONSTRAINT `FK_Dreams_FutureProgs_OnDream` FOREIGN KEY (`OnDreamProgId`) REFERENCES `futureprogs` (`Id`) ON DELETE SET NULL,
  CONSTRAINT `FK_Dreams_FutureProgs_OnWake` FOREIGN KEY (`OnWakeDuringDreamingProgId`) REFERENCES `futureprogs` (`Id`) ON DELETE SET NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Dumping data for table dreams
--

/*!40000 ALTER TABLE `dreams` DISABLE KEYS */;
/*!40000 ALTER TABLE `dreams` ENABLE KEYS */;

--
-- Definition of dream_phases
--

DROP TABLE IF EXISTS `dream_phases`;
CREATE TABLE IF NOT EXISTS `dream_phases` (
  `DreamId` bigint NOT NULL,
  `PhaseId` int NOT NULL,
  `DreamerText` text CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL,
  `DreamerCommand` text CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL,
  `WaitSeconds` int NOT NULL DEFAULT '30',
  PRIMARY KEY (`DreamId`,`PhaseId`),
  CONSTRAINT `FK_Dream_Phases_Dreams` FOREIGN KEY (`DreamId`) REFERENCES `dreams` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Dumping data for table dream_phases
--

/*!40000 ALTER TABLE `dream_phases` DISABLE KEYS */;
/*!40000 ALTER TABLE `dream_phases` ENABLE KEYS */;

--
-- Definition of dreams_already_dreamt
--

DROP TABLE IF EXISTS `dreams_already_dreamt`;
CREATE TABLE IF NOT EXISTS `dreams_already_dreamt` (
  `DreamId` bigint NOT NULL,
  `CharacterId` bigint NOT NULL,
  PRIMARY KEY (`DreamId`,`CharacterId`),
  KEY `FK_Dreams_Dreamt_Characters_idx` (`CharacterId`),
  CONSTRAINT `FK_Dreams_Dreamt_Characters` FOREIGN KEY (`CharacterId`) REFERENCES `characters` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_Dreams_Dreamt_Dreams` FOREIGN KEY (`DreamId`) REFERENCES `dreams` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Dumping data for table dreams_already_dreamt
--

/*!40000 ALTER TABLE `dreams_already_dreamt` DISABLE KEYS */;
/*!40000 ALTER TABLE `dreams_already_dreamt` ENABLE KEYS */;

--
-- Definition of dreams_characters
--

DROP TABLE IF EXISTS `dreams_characters`;
CREATE TABLE IF NOT EXISTS `dreams_characters` (
  `DreamId` bigint NOT NULL,
  `CharacterId` bigint NOT NULL,
  PRIMARY KEY (`DreamId`,`CharacterId`),
  KEY `FK_Dreams_Characters_Characters_idx` (`CharacterId`),
  CONSTRAINT `FK_Dreams_Characters_Characters` FOREIGN KEY (`CharacterId`) REFERENCES `characters` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_Dreams_Characters_Dreams` FOREIGN KEY (`DreamId`) REFERENCES `dreams` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Dumping data for table dreams_characters
--

/*!40000 ALTER TABLE `dreams_characters` DISABLE KEYS */;
/*!40000 ALTER TABLE `dreams_characters` ENABLE KEYS */;

--
-- Definition of economiczonetaxes
--

DROP TABLE IF EXISTS `economiczonetaxes`;
CREATE TABLE IF NOT EXISTS `economiczonetaxes` (
  `Id` bigint NOT NULL AUTO_INCREMENT,
  `EconomicZoneId` bigint NOT NULL,
  `Name` varchar(200) DEFAULT NULL,
  `MerchantDescription` varchar(200) DEFAULT NULL,
  `MerchandiseFilterProgId` bigint DEFAULT NULL,
  `TaxType` varchar(50) DEFAULT NULL,
  `Definition` text CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci,
  PRIMARY KEY (`Id`),
  KEY `FK_EconomicZoneTaxes_EconomicZones_idx` (`EconomicZoneId`),
  KEY `FK_EconomicZoneTaxes_FutureProgs_idx` (`MerchandiseFilterProgId`),
  CONSTRAINT `FK_EconomicZoneTaxes_EconomicZones` FOREIGN KEY (`EconomicZoneId`) REFERENCES `economiczones` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_EconomicZoneTaxes_FutureProgs` FOREIGN KEY (`MerchandiseFilterProgId`) REFERENCES `futureprogs` (`Id`) ON DELETE RESTRICT
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Dumping data for table economiczonetaxes
--

/*!40000 ALTER TABLE `economiczonetaxes` DISABLE KEYS */;
/*!40000 ALTER TABLE `economiczonetaxes` ENABLE KEYS */;

--
-- Definition of elections
--

DROP TABLE IF EXISTS `elections`;
CREATE TABLE IF NOT EXISTS `elections` (
  `Id` bigint NOT NULL AUTO_INCREMENT,
  `AppointmentId` bigint NOT NULL,
  `NominationStartDate` varchar(100) DEFAULT NULL,
  `VotingStartDate` varchar(100) DEFAULT NULL,
  `VotingEndDate` varchar(100) DEFAULT NULL,
  `ResultsInEffectDate` varchar(100) DEFAULT NULL,
  `IsFinalised` bit(1) NOT NULL,
  `NumberOfAppointments` int NOT NULL,
  `IsByElection` bit(1) NOT NULL,
  `ElectionStage` int NOT NULL,
  PRIMARY KEY (`Id`),
  KEY `FK_Elections_Appointments_idx` (`AppointmentId`),
  CONSTRAINT `FK_Elections_Appointments` FOREIGN KEY (`AppointmentId`) REFERENCES `appointments` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Dumping data for table elections
--

/*!40000 ALTER TABLE `elections` DISABLE KEYS */;
/*!40000 ALTER TABLE `elections` ENABLE KEYS */;

--
-- Definition of electionsnominees
--

DROP TABLE IF EXISTS `electionsnominees`;
CREATE TABLE IF NOT EXISTS `electionsnominees` (
  `ElectionId` bigint NOT NULL,
  `NomineeId` bigint NOT NULL,
  `NomineeClanId` bigint NOT NULL,
  PRIMARY KEY (`ElectionId`,`NomineeId`),
  KEY `FK_ElectionsNominees_Elections_idx` (`ElectionId`),
  KEY `FK_ElectionsNominees_ClanMemberships_idx` (`NomineeClanId`,`NomineeId`),
  CONSTRAINT `FK_ElectionsNominees_ClanMemberships` FOREIGN KEY (`NomineeClanId`, `NomineeId`) REFERENCES `clanmemberships` (`ClanId`, `CharacterId`) ON DELETE CASCADE,
  CONSTRAINT `FK_ElectionsNominees_Elections` FOREIGN KEY (`ElectionId`) REFERENCES `elections` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Dumping data for table electionsnominees
--

/*!40000 ALTER TABLE `electionsnominees` DISABLE KEYS */;
/*!40000 ALTER TABLE `electionsnominees` ENABLE KEYS */;

--
-- Definition of entitydescriptionpatterns
--

DROP TABLE IF EXISTS `entitydescriptionpatterns`;
CREATE TABLE IF NOT EXISTS `entitydescriptionpatterns` (
  `Id` bigint NOT NULL AUTO_INCREMENT,
  `Pattern` varchar(4000) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `Type` int NOT NULL,
  `ApplicabilityProgId` bigint DEFAULT NULL,
  `RelativeWeight` int NOT NULL,
  PRIMARY KEY (`Id`),
  KEY `FK_EntityDescriptionPatterns_FutureProgs` (`ApplicabilityProgId`),
  CONSTRAINT `FK_EntityDescriptionPatterns_FutureProgs` FOREIGN KEY (`ApplicabilityProgId`) REFERENCES `futureprogs` (`Id`) ON DELETE RESTRICT
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Dumping data for table entitydescriptionpatterns
--

/*!40000 ALTER TABLE `entitydescriptionpatterns` DISABLE KEYS */;
/*!40000 ALTER TABLE `entitydescriptionpatterns` ENABLE KEYS */;

--
-- Definition of externalclancontrols
--

DROP TABLE IF EXISTS `externalclancontrols`;
CREATE TABLE IF NOT EXISTS `externalclancontrols` (
  `VassalClanId` bigint NOT NULL,
  `LiegeClanId` bigint NOT NULL,
  `ControlledAppointmentId` bigint NOT NULL,
  `ControllingAppointmentId` bigint DEFAULT NULL,
  `NumberOfAppointments` int NOT NULL,
  PRIMARY KEY (`VassalClanId`,`LiegeClanId`,`ControlledAppointmentId`),
  KEY `FK_ECC_Appointments_Controlled` (`ControlledAppointmentId`),
  KEY `FK_ECC_Appointments_Controlling` (`ControllingAppointmentId`),
  KEY `FK_ECC_Clans_Liege` (`LiegeClanId`),
  CONSTRAINT `FK_ECC_Appointments_Controlled` FOREIGN KEY (`ControlledAppointmentId`) REFERENCES `appointments` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_ECC_Appointments_Controlling` FOREIGN KEY (`ControllingAppointmentId`) REFERENCES `appointments` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_ECC_Clans_Liege` FOREIGN KEY (`LiegeClanId`) REFERENCES `clans` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_ECC_Clans_Vassal` FOREIGN KEY (`VassalClanId`) REFERENCES `clans` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Dumping data for table externalclancontrols
--

/*!40000 ALTER TABLE `externalclancontrols` DISABLE KEYS */;
/*!40000 ALTER TABLE `externalclancontrols` ENABLE KEYS */;

--
-- Definition of externalclancontrols_appointments
--

DROP TABLE IF EXISTS `externalclancontrols_appointments`;
CREATE TABLE IF NOT EXISTS `externalclancontrols_appointments` (
  `VassalClanId` bigint NOT NULL,
  `LiegeClanId` bigint NOT NULL,
  `ControlledAppointmentId` bigint NOT NULL,
  `CharacterId` bigint NOT NULL,
  PRIMARY KEY (`CharacterId`,`VassalClanId`,`LiegeClanId`,`ControlledAppointmentId`),
  KEY `FK_ECC_Appointments_ClanMemberships` (`VassalClanId`,`CharacterId`),
  KEY `FK_ECC_Appointments_ExternalClanControls` (`VassalClanId`,`LiegeClanId`,`ControlledAppointmentId`),
  CONSTRAINT `FK_ECC_Appointments_ClanMemberships` FOREIGN KEY (`VassalClanId`, `CharacterId`) REFERENCES `clanmemberships` (`ClanId`, `CharacterId`) ON DELETE RESTRICT,
  CONSTRAINT `FK_ECC_Appointments_ExternalClanControls` FOREIGN KEY (`VassalClanId`, `LiegeClanId`, `ControlledAppointmentId`) REFERENCES `externalclancontrols` (`VassalClanId`, `LiegeClanId`, `ControlledAppointmentId`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Dumping data for table externalclancontrols_appointments
--

/*!40000 ALTER TABLE `externalclancontrols_appointments` DISABLE KEYS */;
/*!40000 ALTER TABLE `externalclancontrols_appointments` ENABLE KEYS */;

--
-- Definition of futureprogs_parameters
--

DROP TABLE IF EXISTS `futureprogs_parameters`;
CREATE TABLE IF NOT EXISTS `futureprogs_parameters` (
  `FutureProgId` bigint NOT NULL,
  `ParameterIndex` int NOT NULL,
  `ParameterName` varchar(4000) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `ParameterTypeDefinition` varchar(255) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL DEFAULT '',
  PRIMARY KEY (`FutureProgId`,`ParameterIndex`),
  CONSTRAINT `FK_FutureProgs_Parameters_FutureProgs` FOREIGN KEY (`FutureProgId`) REFERENCES `futureprogs` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Dumping data for table futureprogs_parameters
--

/*!40000 ALTER TABLE `futureprogs_parameters` DISABLE KEYS */;
/*!40000 ALTER TABLE `futureprogs_parameters` ENABLE KEYS */;

--
-- Definition of gameitemcomponentprotos
--

DROP TABLE IF EXISTS `gameitemcomponentprotos`;
CREATE TABLE IF NOT EXISTS `gameitemcomponentprotos` (
  `Id` bigint NOT NULL,
  `RevisionNumber` int NOT NULL,
  `Type` varchar(50) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `Definition` text CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `EditableItemId` bigint NOT NULL,
  `Description` text CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `Name` varchar(4000) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  PRIMARY KEY (`Id`,`RevisionNumber`),
  KEY `FK_GameItemComponentProtos_EditableItems` (`EditableItemId`),
  CONSTRAINT `FK_GameItemComponentProtos_EditableItems` FOREIGN KEY (`EditableItemId`) REFERENCES `editableitems` (`Id`) ON DELETE RESTRICT
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Dumping data for table gameitemcomponentprotos
--

/*!40000 ALTER TABLE `gameitemcomponentprotos` DISABLE KEYS */;
/*!40000 ALTER TABLE `gameitemcomponentprotos` ENABLE KEYS */;

--
-- Definition of gameitemeditingview
--

DROP TABLE IF EXISTS `gameitemeditingview`;
CREATE TABLE IF NOT EXISTS `gameitemeditingview` (
  `Id` tinyint NOT NULL,
  `Name` tinyint NOT NULL,
  `MaterialId` tinyint NOT NULL,
  `ProtoMaterial` tinyint NOT NULL,
  `Quality` tinyint NOT NULL,
  `Size` tinyint NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Dumping data for table gameitemeditingview
--

/*!40000 ALTER TABLE `gameitemeditingview` DISABLE KEYS */;
/*!40000 ALTER TABLE `gameitemeditingview` ENABLE KEYS */;

--
-- Definition of gameitems
--

DROP TABLE IF EXISTS `gameitems`;
CREATE TABLE IF NOT EXISTS `gameitems` (
  `Id` bigint NOT NULL AUTO_INCREMENT,
  `Quality` int NOT NULL,
  `GameItemProtoId` bigint NOT NULL,
  `GameItemProtoRevision` int NOT NULL,
  `RoomLayer` int NOT NULL,
  `Condition` double NOT NULL DEFAULT '1',
  `MaterialId` bigint NOT NULL,
  `Size` int NOT NULL,
  `ContainerId` bigint DEFAULT NULL,
  `PositionId` int NOT NULL DEFAULT '1',
  `PositionModifier` int NOT NULL,
  `PositionTargetId` bigint DEFAULT NULL,
  `PositionTargetType` varchar(45) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci DEFAULT NULL,
  `PositionEmote` text CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci,
  `MorphTimeRemaining` int DEFAULT NULL,
  `EffectData` mediumtext CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `SkinId` bigint DEFAULT NULL,
  `OwnerId` bigint DEFAULT NULL,
  `OwnerType` varchar(100) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci DEFAULT NULL,
  `SurfaceLiquidData` mediumtext CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci,
  `RoutePosition` decimal(18,3) DEFAULT NULL,
  PRIMARY KEY (`Id`),
  KEY `FK_GameItems_GameItems_Containers_idx` (`ContainerId`),
  KEY `IX_GameItems_RoutePosition` (`RoutePosition`),
  CONSTRAINT `FK_GameItems_GameItems_Containers` FOREIGN KEY (`ContainerId`) REFERENCES `gameitems` (`Id`) ON DELETE SET NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Dumping data for table gameitems
--

/*!40000 ALTER TABLE `gameitems` DISABLE KEYS */;
/*!40000 ALTER TABLE `gameitems` ENABLE KEYS */;

--
-- Definition of bodies_gameitems
--

DROP TABLE IF EXISTS `bodies_gameitems`;
CREATE TABLE IF NOT EXISTS `bodies_gameitems` (
  `BodyId` bigint NOT NULL,
  `GameItemId` bigint NOT NULL,
  `EquippedOrder` int NOT NULL,
  `WearProfile` bigint DEFAULT NULL,
  `Wielded` int DEFAULT NULL,
  PRIMARY KEY (`BodyId`,`GameItemId`),
  KEY `FK_Bodies_GameItems_GameItems` (`GameItemId`),
  CONSTRAINT `FK_Bodies_GameItems_Bodies` FOREIGN KEY (`BodyId`) REFERENCES `bodies` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_Bodies_GameItems_GameItems` FOREIGN KEY (`GameItemId`) REFERENCES `gameitems` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Dumping data for table bodies_gameitems
--

/*!40000 ALTER TABLE `bodies_gameitems` DISABLE KEYS */;
/*!40000 ALTER TABLE `bodies_gameitems` ENABLE KEYS */;

--
-- Definition of bodies_implants
--

DROP TABLE IF EXISTS `bodies_implants`;
CREATE TABLE IF NOT EXISTS `bodies_implants` (
  `BodyId` bigint NOT NULL,
  `ImplantId` bigint NOT NULL,
  PRIMARY KEY (`BodyId`,`ImplantId`),
  KEY `FK_Bodies_Implants_GameItems_idx` (`ImplantId`),
  CONSTRAINT `FK_Bodies_Implants_Bodies` FOREIGN KEY (`BodyId`) REFERENCES `bodies` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_Bodies_Implants_GameItems` FOREIGN KEY (`ImplantId`) REFERENCES `gameitems` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Dumping data for table bodies_implants
--

/*!40000 ALTER TABLE `bodies_implants` DISABLE KEYS */;
/*!40000 ALTER TABLE `bodies_implants` ENABLE KEYS */;

--
-- Definition of bodies_prosthetics
--

DROP TABLE IF EXISTS `bodies_prosthetics`;
CREATE TABLE IF NOT EXISTS `bodies_prosthetics` (
  `BodyId` bigint NOT NULL,
  `ProstheticId` bigint NOT NULL,
  PRIMARY KEY (`BodyId`,`ProstheticId`),
  KEY `FK_Bodies_Prosthetics_GameItems_idx` (`ProstheticId`),
  CONSTRAINT `FK_Bodies_Prosthetics_Bodies` FOREIGN KEY (`BodyId`) REFERENCES `bodies` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_Bodies_Prosthetics_GameItems` FOREIGN KEY (`ProstheticId`) REFERENCES `gameitems` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Dumping data for table bodies_prosthetics
--

/*!40000 ALTER TABLE `bodies_prosthetics` DISABLE KEYS */;
/*!40000 ALTER TABLE `bodies_prosthetics` ENABLE KEYS */;

--
-- Definition of cells_gameitems
--

DROP TABLE IF EXISTS `cells_gameitems`;
CREATE TABLE IF NOT EXISTS `cells_gameitems` (
  `CellId` bigint NOT NULL,
  `GameItemId` bigint NOT NULL,
  PRIMARY KEY (`CellId`,`GameItemId`),
  KEY `FK_Cells_GameItems_GameItems` (`GameItemId`),
  CONSTRAINT `FK_Cells_GameItems_Cells` FOREIGN KEY (`CellId`) REFERENCES `cells` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_Cells_GameItems_GameItems` FOREIGN KEY (`GameItemId`) REFERENCES `gameitems` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Dumping data for table cells_gameitems
--

/*!40000 ALTER TABLE `cells_gameitems` DISABLE KEYS */;
/*!40000 ALTER TABLE `cells_gameitems` ENABLE KEYS */;

--
-- Definition of computermaildomains
--

DROP TABLE IF EXISTS `computermaildomains`;
CREATE TABLE IF NOT EXISTS `computermaildomains` (
  `Id` bigint NOT NULL AUTO_INCREMENT,
  `DomainName` varchar(255) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `HostItemId` bigint NOT NULL,
  `Enabled` bit(1) NOT NULL,
  `CreatedAtUtc` datetime NOT NULL,
  PRIMARY KEY (`Id`),
  UNIQUE KEY `IX_ComputerMailDomains_DomainName` (`DomainName`),
  KEY `FK_ComputerMailDomains_GameItems_idx` (`HostItemId`),
  CONSTRAINT `FK_ComputerMailDomains_GameItems` FOREIGN KEY (`HostItemId`) REFERENCES `gameitems` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Dumping data for table computermaildomains
--

/*!40000 ALTER TABLE `computermaildomains` DISABLE KEYS */;
/*!40000 ALTER TABLE `computermaildomains` ENABLE KEYS */;

--
-- Definition of computermailaccounts
--

DROP TABLE IF EXISTS `computermailaccounts`;
CREATE TABLE IF NOT EXISTS `computermailaccounts` (
  `Id` bigint NOT NULL AUTO_INCREMENT,
  `ComputerMailDomainId` bigint NOT NULL,
  `UserName` varchar(255) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `PasswordHash` text CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `PasswordSalt` bigint NOT NULL,
  `IsEnabled` bit(1) NOT NULL,
  `CreatedAtUtc` datetime NOT NULL,
  `LastModifiedAtUtc` datetime NOT NULL,
  PRIMARY KEY (`Id`),
  UNIQUE KEY `IX_ComputerMailAccounts_Domain_UserName` (`ComputerMailDomainId`,`UserName`),
  KEY `FK_ComputerMailAccounts_ComputerMailDomains_idx` (`ComputerMailDomainId`),
  CONSTRAINT `FK_ComputerMailAccounts_ComputerMailDomains` FOREIGN KEY (`ComputerMailDomainId`) REFERENCES `computermaildomains` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Dumping data for table computermailaccounts
--

/*!40000 ALTER TABLE `computermailaccounts` DISABLE KEYS */;
/*!40000 ALTER TABLE `computermailaccounts` ENABLE KEYS */;

--
-- Definition of gameitemcomponents
--

DROP TABLE IF EXISTS `gameitemcomponents`;
CREATE TABLE IF NOT EXISTS `gameitemcomponents` (
  `Id` bigint NOT NULL AUTO_INCREMENT,
  `GameItemComponentProtoId` bigint NOT NULL,
  `GameItemComponentProtoRevision` int NOT NULL,
  `Definition` text CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `GameItemId` bigint NOT NULL,
  PRIMARY KEY (`Id`),
  KEY `FK_GameItemComponents_GameItems` (`GameItemId`),
  CONSTRAINT `FK_GameItemComponents_GameItems` FOREIGN KEY (`GameItemId`) REFERENCES `gameitems` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Dumping data for table gameitemcomponents
--

/*!40000 ALTER TABLE `gameitemcomponents` DISABLE KEYS */;
/*!40000 ALTER TABLE `gameitemcomponents` ENABLE KEYS */;

--
-- Definition of gameitemskins
--

DROP TABLE IF EXISTS `gameitemskins`;
CREATE TABLE IF NOT EXISTS `gameitemskins` (
  `Id` bigint NOT NULL,
  `RevisionNumber` int NOT NULL,
  `EditableItemId` bigint NOT NULL,
  `Name` varchar(500) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `ItemProtoId` bigint NOT NULL,
  `ItemName` varchar(500) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `ShortDescription` varchar(500) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `FullDescription` varchar(2000) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `LongDescription` varchar(500) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `Quality` int DEFAULT NULL,
  `IsPublic` bit(1) NOT NULL,
  `CanUseSkinProgId` bigint DEFAULT NULL,
  PRIMARY KEY (`Id`,`RevisionNumber`),
  KEY `IX_GameItemSkins_EditableItemId` (`EditableItemId`),
  CONSTRAINT `FK_GameItemSkins_EditableItems` FOREIGN KEY (`EditableItemId`) REFERENCES `editableitems` (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Dumping data for table gameitemskins
--

/*!40000 ALTER TABLE `gameitemskins` DISABLE KEYS */;
/*!40000 ALTER TABLE `gameitemskins` ENABLE KEYS */;

--
-- Definition of gptthreads
--

DROP TABLE IF EXISTS `gptthreads`;
CREATE TABLE IF NOT EXISTS `gptthreads` (
  `Id` bigint NOT NULL AUTO_INCREMENT,
  `Name` varchar(200) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `Prompt` text CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `Model` varchar(200) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `Temperature` double NOT NULL,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Dumping data for table gptthreads
--

/*!40000 ALTER TABLE `gptthreads` DISABLE KEYS */;
/*!40000 ALTER TABLE `gptthreads` ENABLE KEYS */;

--
-- Definition of gptmessages
--

DROP TABLE IF EXISTS `gptmessages`;
CREATE TABLE IF NOT EXISTS `gptmessages` (
  `Id` bigint NOT NULL AUTO_INCREMENT,
  `GPTThreadId` bigint NOT NULL,
  `CharacterId` bigint DEFAULT NULL,
  `Message` text CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `Response` text CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci,
  PRIMARY KEY (`Id`),
  KEY `IX_GPTMessages_CharacterId` (`CharacterId`),
  KEY `IX_GPTMessages_GPTThreadId` (`GPTThreadId`),
  CONSTRAINT `FK_GPTMessages_Characters` FOREIGN KEY (`CharacterId`) REFERENCES `characters` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_GPTMessages_GPTThreads` FOREIGN KEY (`GPTThreadId`) REFERENCES `gptthreads` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Dumping data for table gptmessages
--

/*!40000 ALTER TABLE `gptmessages` DISABLE KEYS */;
/*!40000 ALTER TABLE `gptmessages` ENABLE KEYS */;

--
-- Definition of grids
--

DROP TABLE IF EXISTS `grids`;
CREATE TABLE IF NOT EXISTS `grids` (
  `Id` bigint NOT NULL AUTO_INCREMENT,
  `GridType` varchar(45) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `Definition` mediumtext CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Dumping data for table grids
--

/*!40000 ALTER TABLE `grids` DISABLE KEYS */;
/*!40000 ALTER TABLE `grids` ENABLE KEYS */;

--
-- Definition of groupaitemplates
--

DROP TABLE IF EXISTS `groupaitemplates`;
CREATE TABLE IF NOT EXISTS `groupaitemplates` (
  `Id` bigint NOT NULL AUTO_INCREMENT,
  `Name` varchar(200) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `Definition` mediumtext CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Dumping data for table groupaitemplates
--

/*!40000 ALTER TABLE `groupaitemplates` DISABLE KEYS */;
/*!40000 ALTER TABLE `groupaitemplates` ENABLE KEYS */;

--
-- Definition of groupais
--

DROP TABLE IF EXISTS `groupais`;
CREATE TABLE IF NOT EXISTS `groupais` (
  `Id` bigint NOT NULL AUTO_INCREMENT,
  `Name` varchar(200) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `GroupAITemplateId` bigint NOT NULL,
  `Data` mediumtext CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `Definition` mediumtext CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  PRIMARY KEY (`Id`),
  KEY `FK_GroupAIs_GroupAITemplates_idx` (`GroupAITemplateId`),
  CONSTRAINT `FK_GroupAIs_GroupAITemplates` FOREIGN KEY (`GroupAITemplateId`) REFERENCES `groupaitemplates` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Dumping data for table groupais
--

/*!40000 ALTER TABLE `groupais` DISABLE KEYS */;
/*!40000 ALTER TABLE `groupais` ENABLE KEYS */;

--
-- Definition of healthstrategies
--

DROP TABLE IF EXISTS `healthstrategies`;
CREATE TABLE IF NOT EXISTS `healthstrategies` (
  `Id` bigint NOT NULL AUTO_INCREMENT,
  `Name` varchar(200) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NOT NULL,
  `Type` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NOT NULL,
  `Definition` text CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NOT NULL,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Dumping data for table healthstrategies
--

/*!40000 ALTER TABLE `healthstrategies` DISABLE KEYS */;
/*!40000 ALTER TABLE `healthstrategies` ENABLE KEYS */;

--
-- Definition of hearingprofiles
--

DROP TABLE IF EXISTS `hearingprofiles`;
CREATE TABLE IF NOT EXISTS `hearingprofiles` (
  `Id` bigint NOT NULL AUTO_INCREMENT,
  `Name` varchar(4000) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `Definition` text CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `Type` varchar(4000) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `SurveyDescription` varchar(4000) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Dumping data for table hearingprofiles
--

/*!40000 ALTER TABLE `hearingprofiles` DISABLE KEYS */;
/*!40000 ALTER TABLE `hearingprofiles` ENABLE KEYS */;

--
-- Definition of heightweightmodels
--

DROP TABLE IF EXISTS `heightweightmodels`;
CREATE TABLE IF NOT EXISTS `heightweightmodels` (
  `Id` bigint NOT NULL AUTO_INCREMENT,
  `Name` varchar(4000) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `MeanHeight` double NOT NULL,
  `MeanBMI` double NOT NULL,
  `StddevHeight` double NOT NULL,
  `StddevBMI` double NOT NULL,
  `BMIMultiplier` double NOT NULL,
  `MeanWeight` double DEFAULT NULL,
  `StddevWeight` double DEFAULT NULL,
  `SkewnessBMI` double DEFAULT NULL,
  `SkewnessHeight` double DEFAULT NULL,
  `SkewnessWeight` double DEFAULT NULL,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Dumping data for table heightweightmodels
--

/*!40000 ALTER TABLE `heightweightmodels` DISABLE KEYS */;
/*!40000 ALTER TABLE `heightweightmodels` ENABLE KEYS */;

--
-- Definition of helpfiles
--

DROP TABLE IF EXISTS `helpfiles`;
CREATE TABLE IF NOT EXISTS `helpfiles` (
  `Id` bigint NOT NULL AUTO_INCREMENT,
  `Name` varchar(255) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `Category` varchar(255) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `Subcategory` varchar(255) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `TagLine` varchar(500) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `PublicText` text CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `RuleId` bigint DEFAULT NULL,
  `Keywords` varchar(500) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `LastEditedBy` varchar(100) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `LastEditedDate` datetime NOT NULL,
  PRIMARY KEY (`Id`),
  KEY `FK_Helpfiles_FutureProgs` (`RuleId`),
  CONSTRAINT `FK_Helpfiles_FutureProgs` FOREIGN KEY (`RuleId`) REFERENCES `futureprogs` (`Id`) ON DELETE RESTRICT
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Dumping data for table helpfiles
--

/*!40000 ALTER TABLE `helpfiles` DISABLE KEYS */;
/*!40000 ALTER TABLE `helpfiles` ENABLE KEYS */;

--
-- Definition of helpfiles_extratexts
--

DROP TABLE IF EXISTS `helpfiles_extratexts`;
CREATE TABLE IF NOT EXISTS `helpfiles_extratexts` (
  `HelpfileId` bigint NOT NULL,
  `DisplayOrder` int NOT NULL,
  `Text` text CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `RuleId` bigint NOT NULL,
  PRIMARY KEY (`HelpfileId`,`DisplayOrder`),
  KEY `FK_Helpfiles_ExtraTexts_FutureProgs` (`RuleId`),
  CONSTRAINT `FK_Helpfiles_ExtraTexts_FutureProgs` FOREIGN KEY (`RuleId`) REFERENCES `futureprogs` (`Id`) ON DELETE RESTRICT,
  CONSTRAINT `FK_Helpfiles_ExtraTexts_Helpfiles` FOREIGN KEY (`HelpfileId`) REFERENCES `helpfiles` (`Id`) ON DELETE RESTRICT
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Dumping data for table helpfiles_extratexts
--

/*!40000 ALTER TABLE `helpfiles_extratexts` DISABLE KEYS */;
/*!40000 ALTER TABLE `helpfiles_extratexts` ENABLE KEYS */;

--
-- Definition of hooks
--

DROP TABLE IF EXISTS `hooks`;
CREATE TABLE IF NOT EXISTS `hooks` (
  `Id` bigint NOT NULL AUTO_INCREMENT,
  `Name` varchar(255) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `Definition` text CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NOT NULL,
  `Type` varchar(255) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `Category` varchar(255) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `TargetEventType` int NOT NULL,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Dumping data for table hooks
--

/*!40000 ALTER TABLE `hooks` DISABLE KEYS */;
/*!40000 ALTER TABLE `hooks` ENABLE KEYS */;

--
-- Definition of defaulthooks
--

DROP TABLE IF EXISTS `defaulthooks`;
CREATE TABLE IF NOT EXISTS `defaulthooks` (
  `HookId` bigint NOT NULL,
  `PerceivableType` varchar(45) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL,
  `FutureProgId` bigint NOT NULL,
  PRIMARY KEY (`HookId`,`PerceivableType`,`FutureProgId`),
  KEY `FK_DefaultHooks_Futureprogs_idx` (`FutureProgId`),
  CONSTRAINT `FK_DefaultHooks_Futureprogs` FOREIGN KEY (`FutureProgId`) REFERENCES `futureprogs` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_DefaultHooks_Hooks` FOREIGN KEY (`HookId`) REFERENCES `hooks` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Dumping data for table defaulthooks
--

/*!40000 ALTER TABLE `defaulthooks` DISABLE KEYS */;
/*!40000 ALTER TABLE `defaulthooks` ENABLE KEYS */;

--
-- Definition of improvers
--

DROP TABLE IF EXISTS `improvers`;
CREATE TABLE IF NOT EXISTS `improvers` (
  `Id` bigint NOT NULL AUTO_INCREMENT,
  `Name` varchar(255) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `Definition` text CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `Type` varchar(50) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Dumping data for table improvers
--

/*!40000 ALTER TABLE `improvers` DISABLE KEYS */;
/*!40000 ALTER TABLE `improvers` ENABLE KEYS */;

--
-- Definition of itemgroups
--

DROP TABLE IF EXISTS `itemgroups`;
CREATE TABLE IF NOT EXISTS `itemgroups` (
  `Id` bigint NOT NULL AUTO_INCREMENT,
  `Name` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci DEFAULT NULL,
  `Keywords` varchar(1000) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci DEFAULT NULL,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Dumping data for table itemgroups
--

/*!40000 ALTER TABLE `itemgroups` DISABLE KEYS */;
/*!40000 ALTER TABLE `itemgroups` ENABLE KEYS */;

--
-- Definition of gameitemprotos
--

DROP TABLE IF EXISTS `gameitemprotos`;
CREATE TABLE IF NOT EXISTS `gameitemprotos` (
  `Id` bigint NOT NULL,
  `RevisionNumber` int NOT NULL,
  `Name` varchar(255) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `Keywords` text CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `MaterialId` bigint NOT NULL,
  `EditableItemId` bigint NOT NULL,
  `Size` int NOT NULL,
  `Weight` double NOT NULL,
  `ReadOnly` bit(1) NOT NULL DEFAULT b'0',
  `LongDescription` text CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci,
  `ItemGroupId` bigint DEFAULT NULL,
  `OnDestroyedGameItemProtoId` bigint DEFAULT NULL,
  `HealthStrategyId` bigint DEFAULT NULL,
  `BaseItemQuality` int NOT NULL DEFAULT '5',
  `CustomColour` varchar(45) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci DEFAULT NULL,
  `HighPriority` bit(1) NOT NULL DEFAULT b'0',
  `MorphGameItemProtoId` bigint DEFAULT NULL,
  `MorphTimeSeconds` int NOT NULL,
  `MorphEmote` varchar(1000) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL DEFAULT '$0 $?1|morphs into $1|decays into nothing$.',
  `ShortDescription` varchar(1000) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci DEFAULT NULL,
  `FullDescription` varchar(4000) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci DEFAULT NULL,
  `PermitPlayerSkins` bit(1) NOT NULL DEFAULT b'0',
  `CostInBaseCurrency` decimal(58,29) NOT NULL DEFAULT '0.00000000000000000000000000000',
  `IsHiddenFromPlayers` tinyint(1) NOT NULL DEFAULT '0',
  `PreserveRegisterVariables` tinyint(1) NOT NULL DEFAULT '0',
  `PlanarData` text CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci,
  `BuilderNotes` text CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci,
  `UniqueName` varchar(255) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci DEFAULT NULL,
  PRIMARY KEY (`Id`,`RevisionNumber`),
  KEY `FK_GameItemProtos_EditableItems` (`EditableItemId`),
  KEY `FK_GameItemProtos_ItemGroups_idx` (`ItemGroupId`),
  KEY `IX_GameItemProtos_UniqueName` (`UniqueName`),
  CONSTRAINT `FK_GameItemProtos_EditableItems` FOREIGN KEY (`EditableItemId`) REFERENCES `editableitems` (`Id`) ON DELETE RESTRICT,
  CONSTRAINT `FK_GameItemProtos_ItemGroups` FOREIGN KEY (`ItemGroupId`) REFERENCES `itemgroups` (`Id`) ON DELETE SET NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Dumping data for table gameitemprotos
--

/*!40000 ALTER TABLE `gameitemprotos` DISABLE KEYS */;
/*!40000 ALTER TABLE `gameitemprotos` ENABLE KEYS */;

--
-- Definition of gameitemprotoextradescriptions
--

DROP TABLE IF EXISTS `gameitemprotoextradescriptions`;
CREATE TABLE IF NOT EXISTS `gameitemprotoextradescriptions` (
  `GameItemProtoId` bigint NOT NULL,
  `GameItemProtoRevisionNumber` int NOT NULL,
  `ApplicabilityProgId` bigint NOT NULL,
  `ShortDescription` varchar(500) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `FullDescription` varchar(2000) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `FullDescriptionAddendum` varchar(500) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `Priority` int NOT NULL,
  PRIMARY KEY (`GameItemProtoId`,`GameItemProtoRevisionNumber`,`ApplicabilityProgId`),
  KEY `IX_GameItemProtoExtraDescriptions_ApplicabilityProgId` (`ApplicabilityProgId`),
  CONSTRAINT `FK_GameItemProtoExtraDescriptions_FutureProgs` FOREIGN KEY (`ApplicabilityProgId`) REFERENCES `futureprogs` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_GameItemProtoExtraDescriptions_GameItemProtos` FOREIGN KEY (`GameItemProtoId`, `GameItemProtoRevisionNumber`) REFERENCES `gameitemprotos` (`Id`, `RevisionNumber`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Dumping data for table gameitemprotoextradescriptions
--

/*!40000 ALTER TABLE `gameitemprotoextradescriptions` DISABLE KEYS */;
/*!40000 ALTER TABLE `gameitemprotoextradescriptions` ENABLE KEYS */;

--
-- Definition of gameitemprotos_defaultvariables
--

DROP TABLE IF EXISTS `gameitemprotos_defaultvariables`;
CREATE TABLE IF NOT EXISTS `gameitemprotos_defaultvariables` (
  `GameItemProtoId` bigint NOT NULL,
  `VariableName` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL,
  `GameItemProtoRevNum` int NOT NULL,
  `VariableValue` text CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL,
  PRIMARY KEY (`GameItemProtoId`,`GameItemProtoRevNum`,`VariableName`),
  CONSTRAINT `FK_GameItemProtos_DefaultValues_GameItemProtos` FOREIGN KEY (`GameItemProtoId`, `GameItemProtoRevNum`) REFERENCES `gameitemprotos` (`Id`, `RevisionNumber`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Dumping data for table gameitemprotos_defaultvariables
--

/*!40000 ALTER TABLE `gameitemprotos_defaultvariables` DISABLE KEYS */;
/*!40000 ALTER TABLE `gameitemprotos_defaultvariables` ENABLE KEYS */;

--
-- Definition of gameitemprotos_gameitemcomponentprotos
--

DROP TABLE IF EXISTS `gameitemprotos_gameitemcomponentprotos`;
CREATE TABLE IF NOT EXISTS `gameitemprotos_gameitemcomponentprotos` (
  `GameItemProtoId` bigint NOT NULL,
  `GameItemComponentProtoId` bigint NOT NULL,
  `GameItemProtoRevision` int NOT NULL,
  `GameItemComponentRevision` int NOT NULL,
  PRIMARY KEY (`GameItemProtoId`,`GameItemComponentProtoId`,`GameItemProtoRevision`,`GameItemComponentRevision`),
  KEY `FK_GIPGICP_GameItemComponentProtos` (`GameItemComponentProtoId`,`GameItemComponentRevision`),
  KEY `FK_GIPGICP_GameItemProtos` (`GameItemProtoId`,`GameItemProtoRevision`),
  CONSTRAINT `FK_GIPGICP_GameItemComponentProtos` FOREIGN KEY (`GameItemComponentProtoId`, `GameItemComponentRevision`) REFERENCES `gameitemcomponentprotos` (`Id`, `RevisionNumber`) ON DELETE CASCADE,
  CONSTRAINT `FK_GIPGICP_GameItemProtos` FOREIGN KEY (`GameItemProtoId`, `GameItemProtoRevision`) REFERENCES `gameitemprotos` (`Id`, `RevisionNumber`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Dumping data for table gameitemprotos_gameitemcomponentprotos
--

/*!40000 ALTER TABLE `gameitemprotos_gameitemcomponentprotos` DISABLE KEYS */;
/*!40000 ALTER TABLE `gameitemprotos_gameitemcomponentprotos` ENABLE KEYS */;

--
-- Definition of gameitemprotos_onloadprogs
--

DROP TABLE IF EXISTS `gameitemprotos_onloadprogs`;
CREATE TABLE IF NOT EXISTS `gameitemprotos_onloadprogs` (
  `GameItemProtoId` bigint NOT NULL,
  `GameItemProtoRevisionNumber` int NOT NULL,
  `FutureProgId` bigint NOT NULL,
  PRIMARY KEY (`GameItemProtoId`,`GameItemProtoRevisionNumber`,`FutureProgId`),
  KEY `FK_GameItemProtos_OnLoadProgs_FutureProgs_idx` (`FutureProgId`),
  CONSTRAINT `FK_GameItemProtos_OnLoadProgs_FutureProgs` FOREIGN KEY (`FutureProgId`) REFERENCES `futureprogs` (`Id`) ON DELETE RESTRICT,
  CONSTRAINT `FK_GameItemProtos_OnLoadProgs_GameItemProtos` FOREIGN KEY (`GameItemProtoId`, `GameItemProtoRevisionNumber`) REFERENCES `gameitemprotos` (`Id`, `RevisionNumber`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Dumping data for table gameitemprotos_onloadprogs
--

/*!40000 ALTER TABLE `gameitemprotos_onloadprogs` DISABLE KEYS */;
/*!40000 ALTER TABLE `gameitemprotos_onloadprogs` ENABLE KEYS */;

--
-- Definition of itemgroupforms
--

DROP TABLE IF EXISTS `itemgroupforms`;
CREATE TABLE IF NOT EXISTS `itemgroupforms` (
  `Id` bigint NOT NULL AUTO_INCREMENT,
  `ItemGroupId` bigint NOT NULL,
  `Type` varchar(45) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL,
  `Definition` text CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL,
  PRIMARY KEY (`Id`),
  KEY `FK_ItemGroupForms_ItemGroups_idx` (`ItemGroupId`),
  CONSTRAINT `FK_ItemGroupForms_ItemGroups` FOREIGN KEY (`ItemGroupId`) REFERENCES `itemgroups` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Dumping data for table itemgroupforms
--

/*!40000 ALTER TABLE `itemgroupforms` DISABLE KEYS */;
/*!40000 ALTER TABLE `itemgroupforms` ENABLE KEYS */;

--
-- Definition of knowledges
--

DROP TABLE IF EXISTS `knowledges`;
CREATE TABLE IF NOT EXISTS `knowledges` (
  `Id` bigint NOT NULL AUTO_INCREMENT,
  `Name` varchar(200) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `Description` text CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `LongDescription` text CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `Type` varchar(200) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `Subtype` varchar(200) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `LearnableType` int NOT NULL,
  `LearnDifficulty` int NOT NULL DEFAULT '7',
  `TeachDifficulty` int NOT NULL DEFAULT '7',
  `LearningSessionsRequired` int NOT NULL,
  `CanAcquireProgId` bigint DEFAULT NULL,
  `CanLearnProgId` bigint DEFAULT NULL,
  PRIMARY KEY (`Id`),
  KEY `FK_KNOWLEDGES_FUTUREPROGS_ACQUIRE_idx` (`CanAcquireProgId`),
  KEY `FK_KNOWLEDGES_FUTUREPROGS_LEARN_idx` (`CanLearnProgId`),
  CONSTRAINT `FK_KNOWLEDGES_FUTUREPROGS_ACQUIRE` FOREIGN KEY (`CanAcquireProgId`) REFERENCES `futureprogs` (`Id`) ON DELETE SET NULL,
  CONSTRAINT `FK_KNOWLEDGES_FUTUREPROGS_LEARN` FOREIGN KEY (`CanLearnProgId`) REFERENCES `futureprogs` (`Id`) ON DELETE SET NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Dumping data for table knowledges
--

/*!40000 ALTER TABLE `knowledges` DISABLE KEYS */;
/*!40000 ALTER TABLE `knowledges` ENABLE KEYS */;

--
-- Definition of characterknowledges
--

DROP TABLE IF EXISTS `characterknowledges`;
CREATE TABLE IF NOT EXISTS `characterknowledges` (
  `Id` bigint NOT NULL AUTO_INCREMENT,
  `CharacterId` bigint NOT NULL,
  `KnowledgeId` bigint NOT NULL,
  `WhenAcquired` datetime NOT NULL,
  `HowAcquired` varchar(200) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `TimesTaught` int NOT NULL,
  PRIMARY KEY (`Id`),
  KEY `FK_CHARACTERKNOWLEDGES_CHARACTERS` (`CharacterId`),
  KEY `FK_CHARACTERKNOWLEDGES_KNOWLEDGES_idx` (`KnowledgeId`),
  CONSTRAINT `FK_CHARACTERKNOWLEDGES_CHARACTERS` FOREIGN KEY (`CharacterId`) REFERENCES `characters` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_CHARACTERKNOWLEDGES_KNOWLEDGES` FOREIGN KEY (`KnowledgeId`) REFERENCES `knowledges` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Dumping data for table characterknowledges
--

/*!40000 ALTER TABLE `characterknowledges` DISABLE KEYS */;
/*!40000 ALTER TABLE `characterknowledges` ENABLE KEYS */;

--
-- Definition of knowledgescosts
--

DROP TABLE IF EXISTS `knowledgescosts`;
CREATE TABLE IF NOT EXISTS `knowledgescosts` (
  `KnowledgeId` bigint NOT NULL,
  `ChargenResourceId` bigint NOT NULL,
  `Cost` int NOT NULL,
  PRIMARY KEY (`KnowledgeId`,`ChargenResourceId`),
  KEY `FK_KnowledgesCosts_ChargenResources_idx` (`ChargenResourceId`),
  KEY `FK_KnowledgesCosts_Knowledges_idx` (`KnowledgeId`),
  CONSTRAINT `FK_KnowledgesCosts_ChargenResources` FOREIGN KEY (`ChargenResourceId`) REFERENCES `chargenresources` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_KnowledgesCosts_Knowledges` FOREIGN KEY (`KnowledgeId`) REFERENCES `knowledges` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Dumping data for table knowledgescosts
--

/*!40000 ALTER TABLE `knowledgescosts` DISABLE KEYS */;
/*!40000 ALTER TABLE `knowledgescosts` ENABLE KEYS */;

--
-- Definition of languagedifficultymodels
--

DROP TABLE IF EXISTS `languagedifficultymodels`;
CREATE TABLE IF NOT EXISTS `languagedifficultymodels` (
  `Id` bigint NOT NULL AUTO_INCREMENT,
  `Definition` longtext CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `Type` varchar(255) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `Name` varchar(255) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Dumping data for table languagedifficultymodels
--

/*!40000 ALTER TABLE `languagedifficultymodels` DISABLE KEYS */;
/*!40000 ALTER TABLE `languagedifficultymodels` ENABLE KEYS */;

--
-- Definition of locks
--

DROP TABLE IF EXISTS `locks`;
CREATE TABLE IF NOT EXISTS `locks` (
  `Id` bigint NOT NULL AUTO_INCREMENT,
  `Name` varchar(255) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `Style` int NOT NULL,
  `Strength` int NOT NULL,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Dumping data for table locks
--

/*!40000 ALTER TABLE `locks` DISABLE KEYS */;
/*!40000 ALTER TABLE `locks` ENABLE KEYS */;

--
-- Definition of doors
--

DROP TABLE IF EXISTS `doors`;
CREATE TABLE IF NOT EXISTS `doors` (
  `Id` bigint NOT NULL AUTO_INCREMENT,
  `Name` varchar(4000) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `Style` int NOT NULL,
  `IsOpen` bit(1) NOT NULL,
  `LockedWith` bigint DEFAULT NULL,
  PRIMARY KEY (`Id`),
  KEY `FK_Doors_Locks` (`LockedWith`),
  CONSTRAINT `FK_Doors_Locks` FOREIGN KEY (`LockedWith`) REFERENCES `locks` (`Id`) ON DELETE RESTRICT
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Dumping data for table doors
--

/*!40000 ALTER TABLE `doors` DISABLE KEYS */;
/*!40000 ALTER TABLE `doors` ENABLE KEYS */;

--
-- Definition of loginips
--

DROP TABLE IF EXISTS `loginips`;
CREATE TABLE IF NOT EXISTS `loginips` (
  `IpAddress` varchar(200) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `AccountId` bigint NOT NULL,
  `FirstDate` datetime NOT NULL,
  `AccountRegisteredOnThisIP` bit(1) NOT NULL DEFAULT b'0',
  PRIMARY KEY (`IpAddress`,`AccountId`),
  KEY `FK_LoginIPs_Accounts` (`AccountId`),
  CONSTRAINT `FK_LoginIPs_Accounts` FOREIGN KEY (`AccountId`) REFERENCES `accounts` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Dumping data for table loginips
--

/*!40000 ALTER TABLE `loginips` DISABLE KEYS */;
/*!40000 ALTER TABLE `loginips` ENABLE KEYS */;

--
-- Definition of magicgenerators
--

DROP TABLE IF EXISTS `magicgenerators`;
CREATE TABLE IF NOT EXISTS `magicgenerators` (
  `Id` bigint NOT NULL AUTO_INCREMENT,
  `Name` varchar(200) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `Type` varchar(45) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `Definition` text CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Dumping data for table magicgenerators
--

/*!40000 ALTER TABLE `magicgenerators` DISABLE KEYS */;
/*!40000 ALTER TABLE `magicgenerators` ENABLE KEYS */;

--
-- Definition of magicresources
--

DROP TABLE IF EXISTS `magicresources`;
CREATE TABLE IF NOT EXISTS `magicresources` (
  `Id` bigint NOT NULL AUTO_INCREMENT,
  `Name` varchar(200) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `Type` varchar(45) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `Definition` text CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `MagicResourceType` int NOT NULL,
  `BottomColour` varchar(100) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci DEFAULT '[35m',
  `MidColour` varchar(100) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci DEFAULT '[1;35m',
  `ShortName` varchar(200) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci DEFAULT NULL,
  `TopColour` varchar(100) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci DEFAULT '[0m[38;5;171m',
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Dumping data for table magicresources
--

/*!40000 ALTER TABLE `magicresources` DISABLE KEYS */;
/*!40000 ALTER TABLE `magicresources` ENABLE KEYS */;

--
-- Definition of cells_magicresources
--

DROP TABLE IF EXISTS `cells_magicresources`;
CREATE TABLE IF NOT EXISTS `cells_magicresources` (
  `CellId` bigint NOT NULL,
  `MagicResourceId` bigint NOT NULL,
  `Amount` double NOT NULL,
  PRIMARY KEY (`CellId`,`MagicResourceId`),
  KEY `FK_Cells_MagicResources_MagicResources_idx` (`MagicResourceId`),
  CONSTRAINT `FK_Cells_MagicResources_Cells` FOREIGN KEY (`CellId`) REFERENCES `cells` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_Cells_MagicResources_MagicResources` FOREIGN KEY (`MagicResourceId`) REFERENCES `magicresources` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Dumping data for table cells_magicresources
--

/*!40000 ALTER TABLE `cells_magicresources` DISABLE KEYS */;
/*!40000 ALTER TABLE `cells_magicresources` ENABLE KEYS */;

--
-- Definition of characters_magicresources
--

DROP TABLE IF EXISTS `characters_magicresources`;
CREATE TABLE IF NOT EXISTS `characters_magicresources` (
  `CharacterId` bigint NOT NULL,
  `MagicResourceId` bigint NOT NULL,
  `Amount` double NOT NULL,
  PRIMARY KEY (`CharacterId`,`MagicResourceId`),
  KEY `FK_Characters_MagicResources_MagicResources_idx` (`MagicResourceId`),
  CONSTRAINT `FK_Characters_MagicResources_Characters` FOREIGN KEY (`CharacterId`) REFERENCES `characters` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_Characters_MagicResources_MagicResources` FOREIGN KEY (`MagicResourceId`) REFERENCES `magicresources` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Dumping data for table characters_magicresources
--

/*!40000 ALTER TABLE `characters_magicresources` DISABLE KEYS */;
/*!40000 ALTER TABLE `characters_magicresources` ENABLE KEYS */;

--
-- Definition of gameitems_magicresources
--

DROP TABLE IF EXISTS `gameitems_magicresources`;
CREATE TABLE IF NOT EXISTS `gameitems_magicresources` (
  `GameItemId` bigint NOT NULL,
  `MagicResourceId` bigint NOT NULL,
  `Amount` double NOT NULL,
  PRIMARY KEY (`GameItemId`,`MagicResourceId`),
  KEY `FK_GameItems_MagicResources_MagicResources_idx` (`MagicResourceId`),
  CONSTRAINT `FK_GameItems_MagicResources_GameItems` FOREIGN KEY (`GameItemId`) REFERENCES `gameitems` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_GameItems_MagicResources_MagicResources` FOREIGN KEY (`MagicResourceId`) REFERENCES `magicresources` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Dumping data for table gameitems_magicresources
--

/*!40000 ALTER TABLE `gameitems_magicresources` DISABLE KEYS */;
/*!40000 ALTER TABLE `gameitems_magicresources` ENABLE KEYS */;

--
-- Definition of magicschools
--

DROP TABLE IF EXISTS `magicschools`;
CREATE TABLE IF NOT EXISTS `magicschools` (
  `Id` bigint NOT NULL AUTO_INCREMENT,
  `Name` varchar(200) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `ParentSchoolId` bigint DEFAULT NULL,
  `SchoolVerb` varchar(45) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `SchoolAdjective` varchar(45) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `PowerListColour` varchar(45) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  PRIMARY KEY (`Id`),
  KEY `FK_MagicSchools_MagicSchools_idx` (`ParentSchoolId`),
  CONSTRAINT `FK_MagicSchools_MagicSchools` FOREIGN KEY (`ParentSchoolId`) REFERENCES `magicschools` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Dumping data for table magicschools
--

/*!40000 ALTER TABLE `magicschools` DISABLE KEYS */;
/*!40000 ALTER TABLE `magicschools` ENABLE KEYS */;

--
-- Definition of magiccapabilities
--

DROP TABLE IF EXISTS `magiccapabilities`;
CREATE TABLE IF NOT EXISTS `magiccapabilities` (
  `Id` bigint NOT NULL AUTO_INCREMENT,
  `Name` varchar(200) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `CapabilityModel` varchar(45) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `PowerLevel` int NOT NULL DEFAULT '1',
  `Definition` text CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `MagicSchoolId` bigint NOT NULL,
  PRIMARY KEY (`Id`),
  KEY `FK_MagicCapabilities_MagicSchools_idx` (`MagicSchoolId`),
  CONSTRAINT `FK_MagicCapabilities_MagicSchools` FOREIGN KEY (`MagicSchoolId`) REFERENCES `magicschools` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Dumping data for table magiccapabilities
--

/*!40000 ALTER TABLE `magiccapabilities` DISABLE KEYS */;
/*!40000 ALTER TABLE `magiccapabilities` ENABLE KEYS */;

--
-- Definition of magicpowers
--

DROP TABLE IF EXISTS `magicpowers`;
CREATE TABLE IF NOT EXISTS `magicpowers` (
  `Id` bigint NOT NULL AUTO_INCREMENT,
  `Name` varchar(200) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `Blurb` varchar(500) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `ShowHelp` text CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `PowerModel` varchar(45) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `Definition` text CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `MagicSchoolId` bigint NOT NULL,
  PRIMARY KEY (`Id`),
  KEY `FK_MagicPowers_MagicSchools_idx` (`MagicSchoolId`),
  CONSTRAINT `FK_MagicPowers_MagicSchools` FOREIGN KEY (`MagicSchoolId`) REFERENCES `magicschools` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Dumping data for table magicpowers
--

/*!40000 ALTER TABLE `magicpowers` DISABLE KEYS */;
/*!40000 ALTER TABLE `magicpowers` ENABLE KEYS */;

--
-- Definition of marketinfluencetemplates
--

DROP TABLE IF EXISTS `marketinfluencetemplates`;
CREATE TABLE IF NOT EXISTS `marketinfluencetemplates` (
  `Id` bigint NOT NULL AUTO_INCREMENT,
  `Name` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  `Description` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  `CharacterKnowsAboutInfluenceProgId` bigint NOT NULL,
  `Impacts` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  `TemplateSummary` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  `PopulationImpacts` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  PRIMARY KEY (`Id`),
  KEY `IX_MarketInfluenceTemplates_CharacterKnowsAboutInfluenceProgId` (`CharacterKnowsAboutInfluenceProgId`),
  CONSTRAINT `FK_MarketInfluenceTemplates_FutureProgs_CharacterKnowsAboutInfl~` FOREIGN KEY (`CharacterKnowsAboutInfluenceProgId`) REFERENCES `futureprogs` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Dumping data for table marketinfluencetemplates
--

/*!40000 ALTER TABLE `marketinfluencetemplates` DISABLE KEYS */;
/*!40000 ALTER TABLE `marketinfluencetemplates` ENABLE KEYS */;

--
-- Definition of materials
--

DROP TABLE IF EXISTS `materials`;
CREATE TABLE IF NOT EXISTS `materials` (
  `Id` bigint NOT NULL AUTO_INCREMENT,
  `Name` varchar(255) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `MaterialDescription` varchar(4000) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `Density` double NOT NULL,
  `Organic` bit(1) NOT NULL,
  `Type` int NOT NULL,
  `BehaviourType` int DEFAULT NULL,
  `ThermalConductivity` double NOT NULL,
  `ElectricalConductivity` double NOT NULL,
  `SpecificHeatCapacity` double NOT NULL,
  `LiquidFormId` bigint DEFAULT NULL,
  `Viscosity` double DEFAULT NULL,
  `MeltingPoint` double DEFAULT NULL,
  `BoilingPoint` double DEFAULT NULL,
  `IgnitionPoint` double DEFAULT NULL,
  `HeatDamagePoint` double DEFAULT NULL,
  `ImpactFracture` double DEFAULT NULL,
  `ImpactYield` double DEFAULT NULL,
  `ImpactStrainAtYield` double DEFAULT NULL,
  `ShearFracture` double DEFAULT NULL,
  `ShearYield` double DEFAULT NULL,
  `ShearStrainAtYield` double DEFAULT NULL,
  `YoungsModulus` double DEFAULT NULL,
  `SolventId` bigint DEFAULT NULL,
  `SolventVolumeRatio` double NOT NULL DEFAULT '1',
  `ResidueSdesc` text CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci,
  `ResidueDesc` text CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci,
  `ResidueColour` varchar(45) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci DEFAULT 'white',
  `Absorbency` double NOT NULL DEFAULT '0.25',
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Dumping data for table materials
--

/*!40000 ALTER TABLE `materials` DISABLE KEYS */;
/*!40000 ALTER TABLE `materials` ENABLE KEYS */;

--
-- Definition of bodypartproto
--

DROP TABLE IF EXISTS `bodypartproto`;
CREATE TABLE IF NOT EXISTS `bodypartproto` (
  `Id` bigint NOT NULL AUTO_INCREMENT,
  `BodypartType` int NOT NULL,
  `BodyId` bigint NOT NULL,
  `Name` varchar(4000) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci DEFAULT NULL,
  `Description` varchar(4000) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci DEFAULT NULL,
  `CountAsId` bigint DEFAULT NULL,
  `BodypartShapeId` bigint NOT NULL,
  `DisplayOrder` int DEFAULT NULL,
  `MaxLife` int NOT NULL DEFAULT '100',
  `SeveredThreshold` int NOT NULL DEFAULT '100',
  `PainModifier` double NOT NULL DEFAULT '1',
  `BleedModifier` double NOT NULL DEFAULT '0.1',
  `RelativeHitChance` int NOT NULL DEFAULT '100',
  `Location` int NOT NULL,
  `Alignment` int NOT NULL,
  `Unary` bit(1) DEFAULT NULL,
  `MaxSingleSize` int DEFAULT NULL,
  `IsOrgan` int NOT NULL,
  `WeightLimit` double NOT NULL,
  `IsCore` bit(1) NOT NULL,
  `StunModifier` double NOT NULL DEFAULT '1',
  `DamageModifier` double NOT NULL DEFAULT '1',
  `DefaultMaterialId` bigint NOT NULL DEFAULT '1',
  `Significant` bit(1) NOT NULL,
  `RelativeInfectability` double NOT NULL DEFAULT '1',
  `HypoxiaDamagePerTick` double NOT NULL DEFAULT '0.2',
  `IsVital` bit(1) NOT NULL DEFAULT b'0',
  `ArmourTypeId` bigint DEFAULT NULL,
  `ImplantSpace` double NOT NULL,
  `ImplantSpaceOccupied` double NOT NULL,
  `Size` int NOT NULL DEFAULT '5',
  `SeverFormula` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  PRIMARY KEY (`Id`),
  KEY `FK_BodypartProto_ArmourTypes_idx` (`ArmourTypeId`),
  KEY `FK_BodypartProto_BodyPrototype` (`BodyId`),
  KEY `FK_BodypartProto_BodypartShape` (`BodypartShapeId`),
  KEY `FK_BodypartProto_BodypartProto_idx` (`CountAsId`),
  KEY `FK_BodypartProto_Materials_idx` (`DefaultMaterialId`),
  CONSTRAINT `FK_BodypartProto_ArmourTypes` FOREIGN KEY (`ArmourTypeId`) REFERENCES `armourtypes` (`Id`) ON DELETE SET NULL,
  CONSTRAINT `FK_BodypartProto_BodypartProto` FOREIGN KEY (`CountAsId`) REFERENCES `bodypartproto` (`Id`) ON DELETE SET NULL,
  CONSTRAINT `FK_BodypartProto_BodypartShape` FOREIGN KEY (`BodypartShapeId`) REFERENCES `bodypartshape` (`Id`) ON DELETE RESTRICT,
  CONSTRAINT `FK_BodypartProto_BodyPrototype` FOREIGN KEY (`BodyId`) REFERENCES `bodyprotos` (`Id`) ON DELETE RESTRICT,
  CONSTRAINT `FK_BodypartProto_Materials` FOREIGN KEY (`DefaultMaterialId`) REFERENCES `materials` (`Id`) ON DELETE RESTRICT
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Dumping data for table bodypartproto
--

/*!40000 ALTER TABLE `bodypartproto` DISABLE KEYS */;
/*!40000 ALTER TABLE `bodypartproto` ENABLE KEYS */;

--
-- Definition of bodies_severedparts
--

DROP TABLE IF EXISTS `bodies_severedparts`;
CREATE TABLE IF NOT EXISTS `bodies_severedparts` (
  `BodiesId` bigint NOT NULL,
  `BodypartProtoId` bigint NOT NULL,
  PRIMARY KEY (`BodiesId`,`BodypartProtoId`),
  KEY `FK_Bodies_SeveredParts_BodypartProtos_idx` (`BodypartProtoId`),
  CONSTRAINT `FK_Bodies_SeveredParts_Bodies` FOREIGN KEY (`BodiesId`) REFERENCES `bodies` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_Bodies_SeveredParts_BodypartProtos` FOREIGN KEY (`BodypartProtoId`) REFERENCES `bodypartproto` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Dumping data for table bodies_severedparts
--

/*!40000 ALTER TABLE `bodies_severedparts` DISABLE KEYS */;
/*!40000 ALTER TABLE `bodies_severedparts` ENABLE KEYS */;

--
-- Definition of bodypartgroupdescribers_bodypartprotos
--

DROP TABLE IF EXISTS `bodypartgroupdescribers_bodypartprotos`;
CREATE TABLE IF NOT EXISTS `bodypartgroupdescribers_bodypartprotos` (
  `BodypartGroupDescriberId` bigint NOT NULL,
  `BodypartProtoId` bigint NOT NULL,
  `Mandatory` bit(1) NOT NULL DEFAULT b'0',
  PRIMARY KEY (`BodypartGroupDescriberId`,`BodypartProtoId`),
  KEY `FK_BGD_BodypartProtos_BodypartProto` (`BodypartProtoId`),
  CONSTRAINT `FK_BGD_BodypartProtos_BodypartGroupDescribers` FOREIGN KEY (`BodypartGroupDescriberId`) REFERENCES `bodypartgroupdescribers` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_BGD_BodypartProtos_BodypartProto` FOREIGN KEY (`BodypartProtoId`) REFERENCES `bodypartproto` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Dumping data for table bodypartgroupdescribers_bodypartprotos
--

/*!40000 ALTER TABLE `bodypartgroupdescribers_bodypartprotos` DISABLE KEYS */;
/*!40000 ALTER TABLE `bodypartgroupdescribers_bodypartprotos` ENABLE KEYS */;

--
-- Definition of bodypartinternalinfos
--

DROP TABLE IF EXISTS `bodypartinternalinfos`;
CREATE TABLE IF NOT EXISTS `bodypartinternalinfos` (
  `BodypartProtoId` bigint NOT NULL,
  `InternalPartId` bigint NOT NULL,
  `IsPrimaryOrganLocation` bit(1) NOT NULL DEFAULT b'0',
  `HitChance` double NOT NULL DEFAULT '5',
  `ProximityGroup` varchar(45) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci DEFAULT NULL,
  PRIMARY KEY (`BodypartProtoId`,`InternalPartId`),
  KEY `FK_BodypartInternalInfos_BodypartProtos_Internal_idx` (`InternalPartId`),
  KEY `FK_BodypartInternalInfos_BodypartProtos_idx` (`BodypartProtoId`),
  CONSTRAINT `FK_BodypartInternalInfos_BodypartProtos` FOREIGN KEY (`BodypartProtoId`) REFERENCES `bodypartproto` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_BodypartInternalInfos_BodypartProtos_Internal` FOREIGN KEY (`InternalPartId`) REFERENCES `bodypartproto` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Dumping data for table bodypartinternalinfos
--

/*!40000 ALTER TABLE `bodypartinternalinfos` DISABLE KEYS */;
/*!40000 ALTER TABLE `bodypartinternalinfos` ENABLE KEYS */;

--
-- Definition of bodypartproto_alignmenthits
--

DROP TABLE IF EXISTS `bodypartproto_alignmenthits`;
CREATE TABLE IF NOT EXISTS `bodypartproto_alignmenthits` (
  `Id` bigint NOT NULL AUTO_INCREMENT,
  `BodypartProtoId` bigint NOT NULL,
  `Alignment` int NOT NULL,
  `HitChance` int DEFAULT NULL,
  PRIMARY KEY (`Id`),
  KEY `FK_BodypartProto_AlignmentHits_BodypartProto` (`BodypartProtoId`),
  CONSTRAINT `FK_BodypartProto_AlignmentHits_BodypartProto` FOREIGN KEY (`BodypartProtoId`) REFERENCES `bodypartproto` (`Id`) ON DELETE RESTRICT
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Dumping data for table bodypartproto_alignmenthits
--

/*!40000 ALTER TABLE `bodypartproto_alignmenthits` DISABLE KEYS */;
/*!40000 ALTER TABLE `bodypartproto_alignmenthits` ENABLE KEYS */;

--
-- Definition of bodypartproto_bodypartproto_upstream
--

DROP TABLE IF EXISTS `bodypartproto_bodypartproto_upstream`;
CREATE TABLE IF NOT EXISTS `bodypartproto_bodypartproto_upstream` (
  `Child` bigint NOT NULL,
  `Parent` bigint NOT NULL,
  PRIMARY KEY (`Child`,`Parent`),
  KEY `FKParent` (`Parent`),
  CONSTRAINT `FKChild` FOREIGN KEY (`Child`) REFERENCES `bodypartproto` (`Id`) ON DELETE RESTRICT,
  CONSTRAINT `FKParent` FOREIGN KEY (`Parent`) REFERENCES `bodypartproto` (`Id`) ON DELETE RESTRICT
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Dumping data for table bodypartproto_bodypartproto_upstream
--

/*!40000 ALTER TABLE `bodypartproto_bodypartproto_upstream` DISABLE KEYS */;
/*!40000 ALTER TABLE `bodypartproto_bodypartproto_upstream` ENABLE KEYS */;

--
-- Definition of bodypartproto_orientationhits
--

DROP TABLE IF EXISTS `bodypartproto_orientationhits`;
CREATE TABLE IF NOT EXISTS `bodypartproto_orientationhits` (
  `Id` bigint NOT NULL AUTO_INCREMENT,
  `BodypartProtoId` bigint NOT NULL,
  `Orientation` int NOT NULL,
  `HitChance` int DEFAULT NULL,
  PRIMARY KEY (`Id`),
  KEY `FK_BodypartProto_OrientationHits_BodypartProto` (`BodypartProtoId`),
  CONSTRAINT `FK_BodypartProto_OrientationHits_BodypartProto` FOREIGN KEY (`BodypartProtoId`) REFERENCES `bodypartproto` (`Id`) ON DELETE RESTRICT
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Dumping data for table bodypartproto_orientationhits
--

/*!40000 ALTER TABLE `bodypartproto_orientationhits` DISABLE KEYS */;
/*!40000 ALTER TABLE `bodypartproto_orientationhits` ENABLE KEYS */;

--
-- Definition of boneorgancoverages
--

DROP TABLE IF EXISTS `boneorgancoverages`;
CREATE TABLE IF NOT EXISTS `boneorgancoverages` (
  `BoneId` bigint NOT NULL,
  `OrganId` bigint NOT NULL,
  `CoverageChance` double NOT NULL,
  PRIMARY KEY (`BoneId`,`OrganId`),
  KEY `FK_BoneOrganCoverages_BodypartProto_Organ_idx` (`OrganId`),
  CONSTRAINT `FK_BoneOrganCoverages_BodypartProto_Bone` FOREIGN KEY (`BoneId`) REFERENCES `bodypartproto` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_BoneOrganCoverages_BodypartProto_Organ` FOREIGN KEY (`OrganId`) REFERENCES `bodypartproto` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Dumping data for table boneorgancoverages
--

/*!40000 ALTER TABLE `boneorgancoverages` DISABLE KEYS */;
/*!40000 ALTER TABLE `boneorgancoverages` ENABLE KEYS */;

--
-- Definition of liquids
--

DROP TABLE IF EXISTS `liquids`;
CREATE TABLE IF NOT EXISTS `liquids` (
  `Id` bigint NOT NULL AUTO_INCREMENT,
  `Name` varchar(200) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `Description` text CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci,
  `LongDescription` text CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci,
  `TasteText` text CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci,
  `VagueTasteText` text CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci,
  `SmellText` text CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci,
  `VagueSmellText` text CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci,
  `TasteIntensity` double NOT NULL DEFAULT '100',
  `SmellIntensity` double NOT NULL DEFAULT '10',
  `AlcoholLitresPerLitre` double NOT NULL,
  `WaterLitresPerLitre` double NOT NULL DEFAULT '1',
  `FoodSatiatedHoursPerLitre` double NOT NULL,
  `DrinkSatiatedHoursPerLitre` double NOT NULL DEFAULT '12',
  `Viscosity` double NOT NULL DEFAULT '1',
  `Density` double NOT NULL DEFAULT '1',
  `Organic` bit(1) NOT NULL DEFAULT b'0',
  `ThermalConductivity` double NOT NULL DEFAULT '0.609',
  `ElectricalConductivity` double NOT NULL DEFAULT '0.005',
  `SpecificHeatCapacity` double NOT NULL DEFAULT '4181',
  `IgnitionPoint` double DEFAULT NULL,
  `FreezingPoint` double DEFAULT '273.15',
  `BoilingPoint` double DEFAULT '373.15',
  `DraughtProgId` bigint DEFAULT NULL,
  `SolventId` bigint DEFAULT NULL,
  `CountAsId` bigint DEFAULT NULL,
  `CountAsQuality` int NOT NULL,
  `DisplayColour` varchar(45) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL DEFAULT 'blue',
  `DampDescription` text CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci,
  `WetDescription` text CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci,
  `DrenchedDescription` text CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci,
  `DampShortDescription` text CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci,
  `WetShortDescription` text CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci,
  `DrenchedShortDescription` text CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci,
  `SolventVolumeRatio` double NOT NULL DEFAULT '1',
  `DriedResidueId` bigint DEFAULT NULL,
  `DrugId` bigint DEFAULT NULL,
  `DrugGramsPerUnitVolume` double NOT NULL,
  `InjectionConsequence` int NOT NULL,
  `ResidueVolumePercentage` double NOT NULL DEFAULT '0.05',
  `GasFormId` bigint DEFAULT NULL,
  `RelativeEnthalpy` double NOT NULL DEFAULT '1',
  `LeaveResidueInRooms` tinyint(1) NOT NULL DEFAULT '0',
  `SurfaceReactionInfo` text CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci,
  PRIMARY KEY (`Id`),
  KEY `FK_Liquids_Liquids_CountasAs_idx` (`CountAsId`),
  KEY `FK_Liquids_Materials_idx` (`DriedResidueId`),
  KEY `FK_Liquids_Drugs_idx` (`DrugId`),
  KEY `FK_Liquids_Liquids_idx` (`SolventId`),
  KEY `IX_Liquids_GasFormId` (`GasFormId`),
  CONSTRAINT `FK_Liquids_Drugs` FOREIGN KEY (`DrugId`) REFERENCES `drugs` (`Id`) ON DELETE SET NULL,
  CONSTRAINT `FK_Liquids_Gases` FOREIGN KEY (`GasFormId`) REFERENCES `gases` (`Id`) ON DELETE SET NULL,
  CONSTRAINT `FK_Liquids_Liquids` FOREIGN KEY (`SolventId`) REFERENCES `liquids` (`Id`) ON DELETE SET NULL,
  CONSTRAINT `FK_Liquids_Liquids_CountasAs` FOREIGN KEY (`CountAsId`) REFERENCES `liquids` (`Id`) ON DELETE SET NULL,
  CONSTRAINT `FK_Liquids_Materials` FOREIGN KEY (`DriedResidueId`) REFERENCES `materials` (`Id`) ON DELETE SET NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Dumping data for table liquids
--

/*!40000 ALTER TABLE `liquids` DISABLE KEYS */;
/*!40000 ALTER TABLE `liquids` ENABLE KEYS */;

--
-- Definition of gases
--

DROP TABLE IF EXISTS `gases`;
CREATE TABLE IF NOT EXISTS `gases` (
  `Id` bigint NOT NULL AUTO_INCREMENT,
  `Name` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL,
  `Description` varchar(200) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL,
  `Density` double NOT NULL DEFAULT '0.001205',
  `ThermalConductivity` double NOT NULL DEFAULT '0.0257',
  `ElectricalConductivity` double NOT NULL DEFAULT '0.000005',
  `Organic` bit(1) NOT NULL DEFAULT b'0',
  `SpecificHeatCapacity` double NOT NULL DEFAULT '1.005',
  `BoilingPoint` double NOT NULL DEFAULT '5',
  `CountAsId` bigint DEFAULT NULL,
  `CountsAsQuality` int DEFAULT NULL,
  `DisplayColour` varchar(40) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL,
  `PrecipitateId` bigint DEFAULT NULL,
  `SmellIntensity` double NOT NULL,
  `SmellText` text CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL,
  `VagueSmellText` text CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL,
  `Viscosity` double NOT NULL DEFAULT '15',
  `DrugGramsPerUnitVolume` double NOT NULL DEFAULT '0',
  `DrugId` bigint DEFAULT NULL,
  `OxidationFactor` double NOT NULL DEFAULT '1',
  PRIMARY KEY (`Id`),
  KEY `FK_Gases_Gases_idx` (`CountAsId`),
  KEY `FK_Gases_Liquids_idx` (`PrecipitateId`),
  KEY `IX_Gases_DrugId` (`DrugId`),
  CONSTRAINT `FK_Gases_Drugs` FOREIGN KEY (`DrugId`) REFERENCES `drugs` (`Id`) ON DELETE SET NULL,
  CONSTRAINT `FK_Gases_Gases` FOREIGN KEY (`CountAsId`) REFERENCES `gases` (`Id`) ON DELETE SET NULL,
  CONSTRAINT `FK_Gases_Liquids` FOREIGN KEY (`PrecipitateId`) REFERENCES `liquids` (`Id`) ON DELETE SET NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Dumping data for table gases
--

/*!40000 ALTER TABLE `gases` DISABLE KEYS */;
/*!40000 ALTER TABLE `gases` ENABLE KEYS */;

--
-- Definition of materials_aliases
--

DROP TABLE IF EXISTS `materials_aliases`;
CREATE TABLE IF NOT EXISTS `materials_aliases` (
  `MaterialId` bigint NOT NULL,
  `Alias` varchar(255) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  PRIMARY KEY (`MaterialId`,`Alias`),
  UNIQUE KEY `Materials_Aliases_Alias_UNIQUE` (`Alias`),
  CONSTRAINT `Materials_Aliases_Materials` FOREIGN KEY (`MaterialId`) REFERENCES `materials` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Dumping data for table materials_aliases
--

/*!40000 ALTER TABLE `materials_aliases` DISABLE KEYS */;
/*!40000 ALTER TABLE `materials_aliases` ENABLE KEYS */;

--
-- Definition of merits
--

DROP TABLE IF EXISTS `merits`;
CREATE TABLE IF NOT EXISTS `merits` (
  `Id` bigint NOT NULL AUTO_INCREMENT,
  `Name` varchar(200) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL,
  `Type` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL,
  `MeritType` int NOT NULL,
  `MeritScope` int NOT NULL,
  `Definition` text CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL,
  `ParentId` bigint DEFAULT NULL,
  PRIMARY KEY (`Id`),
  KEY `FK_Merits_Merits_idx` (`ParentId`),
  CONSTRAINT `FK_Merits_Merits` FOREIGN KEY (`ParentId`) REFERENCES `merits` (`Id`) ON DELETE SET NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Dumping data for table merits
--

/*!40000 ALTER TABLE `merits` DISABLE KEYS */;
/*!40000 ALTER TABLE `merits` ENABLE KEYS */;

--
-- Definition of chargenroles_merits
--

DROP TABLE IF EXISTS `chargenroles_merits`;
CREATE TABLE IF NOT EXISTS `chargenroles_merits` (
  `ChargenRoleId` bigint NOT NULL,
  `MeritId` bigint NOT NULL,
  PRIMARY KEY (`ChargenRoleId`,`MeritId`),
  KEY `FK_ChargenRoles_Merits_Merits_idx` (`MeritId`),
  CONSTRAINT `FK_ChargenRoles_Merits_ChargenRoles` FOREIGN KEY (`ChargenRoleId`) REFERENCES `chargenroles` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_ChargenRoles_Merits_Merits` FOREIGN KEY (`MeritId`) REFERENCES `merits` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Dumping data for table chargenroles_merits
--

/*!40000 ALTER TABLE `chargenroles_merits` DISABLE KEYS */;
/*!40000 ALTER TABLE `chargenroles_merits` ENABLE KEYS */;

--
-- Definition of merits_chargenresources
--

DROP TABLE IF EXISTS `merits_chargenresources`;
CREATE TABLE IF NOT EXISTS `merits_chargenresources` (
  `MeritId` bigint NOT NULL,
  `ChargenResourceId` bigint NOT NULL,
  `RequirementOnly` bit(1) NOT NULL DEFAULT b'0',
  `Amount` int NOT NULL,
  PRIMARY KEY (`MeritId`,`ChargenResourceId`,`RequirementOnly`),
  KEY `FK_Merits_ChargenResources_ChargenResources_idx` (`ChargenResourceId`),
  CONSTRAINT `FK_Merits_ChargenResources_ChargenResources` FOREIGN KEY (`ChargenResourceId`) REFERENCES `chargenresources` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_Merits_ChargenResources_Merits` FOREIGN KEY (`MeritId`) REFERENCES `merits` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Dumping data for table merits_chargenresources
--

/*!40000 ALTER TABLE `merits_chargenresources` DISABLE KEYS */;
/*!40000 ALTER TABLE `merits_chargenresources` ENABLE KEYS */;

--
-- Definition of nameculture
--

DROP TABLE IF EXISTS `nameculture`;
CREATE TABLE IF NOT EXISTS `nameculture` (
  `Id` bigint NOT NULL AUTO_INCREMENT,
  `Name` varchar(255) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `Definition` text CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Dumping data for table nameculture
--

/*!40000 ALTER TABLE `nameculture` DISABLE KEYS */;
/*!40000 ALTER TABLE `nameculture` ENABLE KEYS */;

--
-- Definition of culturesnamecultures
--

DROP TABLE IF EXISTS `culturesnamecultures`;
CREATE TABLE IF NOT EXISTS `culturesnamecultures` (
  `CultureId` bigint NOT NULL,
  `NameCultureId` bigint NOT NULL,
  `Gender` smallint NOT NULL,
  PRIMARY KEY (`CultureId`,`NameCultureId`,`Gender`),
  KEY `IX_CulturesNameCultures_NameCultureId` (`NameCultureId`),
  CONSTRAINT `FK_CulturesNameCultures_Cultures` FOREIGN KEY (`CultureId`) REFERENCES `cultures` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_CulturesNameCultures_NameCultures` FOREIGN KEY (`NameCultureId`) REFERENCES `nameculture` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Dumping data for table culturesnamecultures
--

/*!40000 ALTER TABLE `culturesnamecultures` DISABLE KEYS */;
/*!40000 ALTER TABLE `culturesnamecultures` ENABLE KEYS */;

--
-- Definition of ethnicitiesnamecultures
--

DROP TABLE IF EXISTS `ethnicitiesnamecultures`;
CREATE TABLE IF NOT EXISTS `ethnicitiesnamecultures` (
  `EthnicityId` bigint NOT NULL,
  `NameCultureId` bigint NOT NULL,
  `Gender` smallint NOT NULL,
  PRIMARY KEY (`EthnicityId`,`NameCultureId`,`Gender`),
  KEY `IX_EthnicitiesNameCultures_NameCultureId` (`NameCultureId`),
  CONSTRAINT `FK_EthnicitiesNameCultures_Ethnicities` FOREIGN KEY (`EthnicityId`) REFERENCES `ethnicities` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_EthnicitiesNameCultures_NameCultures` FOREIGN KEY (`NameCultureId`) REFERENCES `nameculture` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Dumping data for table ethnicitiesnamecultures
--

/*!40000 ALTER TABLE `ethnicitiesnamecultures` DISABLE KEYS */;
/*!40000 ALTER TABLE `ethnicitiesnamecultures` ENABLE KEYS */;

--
-- Definition of newplayerhints
--

DROP TABLE IF EXISTS `newplayerhints`;
CREATE TABLE IF NOT EXISTS `newplayerhints` (
  `Id` bigint NOT NULL AUTO_INCREMENT,
  `Text` text CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `FilterProgId` bigint DEFAULT NULL,
  `Priority` int NOT NULL,
  `CanRepeat` bit(1) NOT NULL,
  PRIMARY KEY (`Id`),
  KEY `IX_NewPlayerHints_FilterProgId` (`FilterProgId`),
  CONSTRAINT `FK_NewPlayerHints_FutureProgs` FOREIGN KEY (`FilterProgId`) REFERENCES `futureprogs` (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Dumping data for table newplayerhints
--

/*!40000 ALTER TABLE `newplayerhints` DISABLE KEYS */;
/*!40000 ALTER TABLE `newplayerhints` ENABLE KEYS */;

--
-- Definition of noncardinalexittemplates
--

DROP TABLE IF EXISTS `noncardinalexittemplates`;
CREATE TABLE IF NOT EXISTS `noncardinalexittemplates` (
  `Id` bigint NOT NULL AUTO_INCREMENT,
  `Name` varchar(255) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `OriginOutboundPreface` varchar(255) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `OriginInboundPreface` varchar(255) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `DestinationOutboundPreface` varchar(255) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `DestinationInboundPreface` varchar(255) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `OutboundVerb` varchar(255) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `InboundVerb` varchar(255) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Dumping data for table noncardinalexittemplates
--

/*!40000 ALTER TABLE `noncardinalexittemplates` DISABLE KEYS */;
/*!40000 ALTER TABLE `noncardinalexittemplates` ENABLE KEYS */;

--
-- Definition of npcspawners
--

DROP TABLE IF EXISTS `npcspawners`;
CREATE TABLE IF NOT EXISTS `npcspawners` (
  `Id` bigint NOT NULL AUTO_INCREMENT,
  `Name` varchar(255) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `TargetTemplateId` bigint DEFAULT NULL,
  `TargetCount` int NOT NULL,
  `MinimumCount` int NOT NULL,
  `OnSpawnProgId` bigint DEFAULT NULL,
  `CountsAsProgId` bigint DEFAULT NULL,
  `IsActiveProgId` bigint DEFAULT NULL,
  `SpawnStrategy` int NOT NULL,
  `Definition` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  PRIMARY KEY (`Id`),
  KEY `IX_NPCSpawners_CountsAsProgId` (`CountsAsProgId`),
  KEY `IX_NPCSpawners_IsActiveProgId` (`IsActiveProgId`),
  KEY `IX_NPCSpawners_OnSpawnProgId` (`OnSpawnProgId`),
  CONSTRAINT `FK_NPCSpawners_CountsAsProg` FOREIGN KEY (`CountsAsProgId`) REFERENCES `futureprogs` (`Id`) ON DELETE SET NULL,
  CONSTRAINT `FK_NPCSpawners_IsActiveProg` FOREIGN KEY (`IsActiveProgId`) REFERENCES `futureprogs` (`Id`) ON DELETE SET NULL,
  CONSTRAINT `FK_NPCSpawners_OnSpawnProg` FOREIGN KEY (`OnSpawnProgId`) REFERENCES `futureprogs` (`Id`) ON DELETE SET NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Dumping data for table npcspawners
--

/*!40000 ALTER TABLE `npcspawners` DISABLE KEYS */;
/*!40000 ALTER TABLE `npcspawners` ENABLE KEYS */;

--
-- Definition of npcspawnercells
--

DROP TABLE IF EXISTS `npcspawnercells`;
CREATE TABLE IF NOT EXISTS `npcspawnercells` (
  `NPCSpawnerId` bigint NOT NULL,
  `CellId` bigint NOT NULL,
  PRIMARY KEY (`NPCSpawnerId`,`CellId`),
  KEY `IX_NPCSpawnerCells_CellId` (`CellId`),
  CONSTRAINT `FK_NPCSpawnerCells_Cell` FOREIGN KEY (`CellId`) REFERENCES `cells` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_NPCSpawnerCells_NPCSpawner` FOREIGN KEY (`NPCSpawnerId`) REFERENCES `npcspawners` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Dumping data for table npcspawnercells
--

/*!40000 ALTER TABLE `npcspawnercells` DISABLE KEYS */;
/*!40000 ALTER TABLE `npcspawnercells` ENABLE KEYS */;

--
-- Definition of npctemplates
--

DROP TABLE IF EXISTS `npctemplates`;
CREATE TABLE IF NOT EXISTS `npctemplates` (
  `Id` bigint NOT NULL,
  `RevisionNumber` int NOT NULL,
  `Type` varchar(255) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `Name` varchar(255) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `Definition` text CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `EditableItemId` bigint NOT NULL,
  `BuilderNotes` text CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci,
  `UniqueName` varchar(255) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci DEFAULT NULL,
  PRIMARY KEY (`Id`,`RevisionNumber`),
  KEY `FK_NPCTemplates_EditableItems` (`EditableItemId`),
  KEY `IX_NPCTemplates_UniqueName` (`UniqueName`),
  CONSTRAINT `FK_NPCTemplates_EditableItems` FOREIGN KEY (`EditableItemId`) REFERENCES `editableitems` (`Id`) ON DELETE RESTRICT
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Dumping data for table npctemplates
--

/*!40000 ALTER TABLE `npctemplates` DISABLE KEYS */;
/*!40000 ALTER TABLE `npctemplates` ENABLE KEYS */;

--
-- Definition of agricultureherddefinitions
--

DROP TABLE IF EXISTS `agricultureherddefinitions`;
CREATE TABLE IF NOT EXISTS `agricultureherddefinitions` (
  `Id` bigint NOT NULL AUTO_INCREMENT,
  `Name` varchar(200) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `Description` mediumtext CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `Definition` mediumtext CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `NpcTemplateId` bigint DEFAULT NULL,
  `NpcTemplateRevisionNumber` int DEFAULT NULL,
  PRIMARY KEY (`Id`),
  KEY `FK_AgricultureHerdDefinitions_NpcTemplates_idx` (`NpcTemplateId`,`NpcTemplateRevisionNumber`),
  CONSTRAINT `FK_AgricultureHerdDefinitions_NpcTemplates` FOREIGN KEY (`NpcTemplateId`, `NpcTemplateRevisionNumber`) REFERENCES `npctemplates` (`Id`, `RevisionNumber`) ON DELETE SET NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Dumping data for table agricultureherddefinitions
--

/*!40000 ALTER TABLE `agricultureherddefinitions` DISABLE KEYS */;
/*!40000 ALTER TABLE `agricultureherddefinitions` ENABLE KEYS */;

--
-- Definition of agriculturefieldherds
--

DROP TABLE IF EXISTS `agriculturefieldherds`;
CREATE TABLE IF NOT EXISTS `agriculturefieldherds` (
  `Id` bigint NOT NULL AUTO_INCREMENT,
  `AgricultureFieldId` bigint NOT NULL,
  `HerdDefinitionId` bigint NOT NULL,
  `HeadCount` int NOT NULL,
  `Condition` double NOT NULL,
  `Definition` mediumtext CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  PRIMARY KEY (`Id`),
  KEY `FK_AgricultureFieldHerds_Fields_idx` (`AgricultureFieldId`),
  KEY `FK_AgricultureFieldHerds_Herds_idx` (`HerdDefinitionId`),
  CONSTRAINT `FK_AgricultureFieldHerds_Fields` FOREIGN KEY (`AgricultureFieldId`) REFERENCES `agriculturefields` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_AgricultureFieldHerds_Herds` FOREIGN KEY (`HerdDefinitionId`) REFERENCES `agricultureherddefinitions` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Dumping data for table agriculturefieldherds
--

/*!40000 ALTER TABLE `agriculturefieldherds` DISABLE KEYS */;
/*!40000 ALTER TABLE `agriculturefieldherds` ENABLE KEYS */;

--
-- Definition of npcs
--

DROP TABLE IF EXISTS `npcs`;
CREATE TABLE IF NOT EXISTS `npcs` (
  `Id` bigint NOT NULL AUTO_INCREMENT,
  `CharacterId` bigint NOT NULL,
  `TemplateId` bigint NOT NULL,
  `TemplateRevnum` int NOT NULL,
  `BodyguardCharacterId` bigint DEFAULT NULL,
  PRIMARY KEY (`Id`),
  KEY `FK_NPCs_Characters_Bodyguard_idx` (`BodyguardCharacterId`),
  KEY `FK_NPCs_Characters` (`CharacterId`),
  KEY `FK_NPCs_NPCTemplates` (`TemplateId`,`TemplateRevnum`),
  CONSTRAINT `FK_NPCs_Characters` FOREIGN KEY (`CharacterId`) REFERENCES `characters` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_NPCs_Characters_Bodyguard` FOREIGN KEY (`BodyguardCharacterId`) REFERENCES `characters` (`Id`) ON DELETE SET NULL,
  CONSTRAINT `FK_NPCs_NPCTemplates` FOREIGN KEY (`TemplateId`, `TemplateRevnum`) REFERENCES `npctemplates` (`Id`, `RevisionNumber`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Dumping data for table npcs
--

/*!40000 ALTER TABLE `npcs` DISABLE KEYS */;
/*!40000 ALTER TABLE `npcs` ENABLE KEYS */;

--
-- Definition of npcs_artificialintelligences
--

DROP TABLE IF EXISTS `npcs_artificialintelligences`;
CREATE TABLE IF NOT EXISTS `npcs_artificialintelligences` (
  `NPCId` bigint NOT NULL,
  `ArtificialIntelligenceId` bigint NOT NULL,
  PRIMARY KEY (`ArtificialIntelligenceId`,`NPCId`),
  KEY `FK_NPCs_ArtificialIntelligences_NPCs` (`NPCId`),
  CONSTRAINT `FK_NPCs_ArtificialIntelligences_ArtificialIntelligences` FOREIGN KEY (`ArtificialIntelligenceId`) REFERENCES `artificialintelligences` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_NPCs_ArtificialIntelligences_NPCs` FOREIGN KEY (`NPCId`) REFERENCES `npcs` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Dumping data for table npcs_artificialintelligences
--

/*!40000 ALTER TABLE `npcs_artificialintelligences` DISABLE KEYS */;
/*!40000 ALTER TABLE `npcs_artificialintelligences` ENABLE KEYS */;

--
-- Definition of npctemplates_artificalintelligences
--

DROP TABLE IF EXISTS `npctemplates_artificalintelligences`;
CREATE TABLE IF NOT EXISTS `npctemplates_artificalintelligences` (
  `NPCTemplateId` bigint NOT NULL,
  `AIId` bigint NOT NULL,
  `NPCTemplateRevisionNumber` int NOT NULL,
  PRIMARY KEY (`NPCTemplateRevisionNumber`,`NPCTemplateId`,`AIId`),
  KEY `FK_NTAI_ArtificalIntelligences` (`AIId`),
  KEY `FK_NTAI_NPCTemplates` (`NPCTemplateId`,`NPCTemplateRevisionNumber`),
  CONSTRAINT `FK_NTAI_ArtificalIntelligences` FOREIGN KEY (`AIId`) REFERENCES `artificialintelligences` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_NTAI_NPCTemplates` FOREIGN KEY (`NPCTemplateId`, `NPCTemplateRevisionNumber`) REFERENCES `npctemplates` (`Id`, `RevisionNumber`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Dumping data for table npctemplates_artificalintelligences
--

/*!40000 ALTER TABLE `npctemplates_artificalintelligences` DISABLE KEYS */;
/*!40000 ALTER TABLE `npctemplates_artificalintelligences` ENABLE KEYS */;

--
-- Definition of outfittemplates
--

DROP TABLE IF EXISTS `outfittemplates`;
CREATE TABLE IF NOT EXISTS `outfittemplates` (
  `Id` bigint NOT NULL AUTO_INCREMENT,
  `Name` varchar(200) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `Description` mediumtext CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `Exclusivity` int NOT NULL,
  PRIMARY KEY (`Id`),
  UNIQUE KEY `IX_OutfitTemplates_Name` (`Name`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Dumping data for table outfittemplates
--

/*!40000 ALTER TABLE `outfittemplates` DISABLE KEYS */;
/*!40000 ALTER TABLE `outfittemplates` ENABLE KEYS */;

--
-- Definition of paygrades
--

DROP TABLE IF EXISTS `paygrades`;
CREATE TABLE IF NOT EXISTS `paygrades` (
  `Id` bigint NOT NULL AUTO_INCREMENT,
  `Name` varchar(100) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `Abbreviation` varchar(100) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `CurrencyId` bigint NOT NULL,
  `PayAmount` decimal(58,29) NOT NULL,
  `ClanId` bigint NOT NULL,
  PRIMARY KEY (`Id`),
  KEY `FK_Paygrades_Clans` (`ClanId`),
  KEY `FK_Paygrades_Currencies` (`CurrencyId`),
  CONSTRAINT `FK_Paygrades_Clans` FOREIGN KEY (`ClanId`) REFERENCES `clans` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_Paygrades_Currencies` FOREIGN KEY (`CurrencyId`) REFERENCES `currencies` (`Id`) ON DELETE RESTRICT
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Dumping data for table paygrades
--

/*!40000 ALTER TABLE `paygrades` DISABLE KEYS */;
/*!40000 ALTER TABLE `paygrades` ENABLE KEYS */;

--
-- Definition of planes
--

DROP TABLE IF EXISTS `planes`;
CREATE TABLE IF NOT EXISTS `planes` (
  `Id` bigint NOT NULL AUTO_INCREMENT,
  `Name` varchar(200) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `Alias` varchar(500) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL DEFAULT '',
  `Description` text CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `DisplayOrder` int NOT NULL,
  `IsDefault` bit(1) NOT NULL DEFAULT b'0',
  `RoomDescriptionAddendum` text CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci,
  `RoomNameFormat` varchar(500) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci DEFAULT NULL,
  `RemoteObservationTag` varchar(500) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci DEFAULT NULL,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Dumping data for table planes
--

/*!40000 ALTER TABLE `planes` DISABLE KEYS */;
/*!40000 ALTER TABLE `planes` ENABLE KEYS */;

--
-- Definition of playeractivitysnapshots
--

DROP TABLE IF EXISTS `playeractivitysnapshots`;
CREATE TABLE IF NOT EXISTS `playeractivitysnapshots` (
  `Id` bigint NOT NULL AUTO_INCREMENT,
  `DateTime` datetime NOT NULL,
  `OnlinePlayers` int NOT NULL,
  `OnlineAdmins` int NOT NULL,
  `AvailableAdmins` int NOT NULL,
  `IdlePlayers` int NOT NULL,
  `UniquePCLocations` int NOT NULL,
  `OnlineGuests` int NOT NULL,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Dumping data for table playeractivitysnapshots
--

/*!40000 ALTER TABLE `playeractivitysnapshots` DISABLE KEYS */;
/*!40000 ALTER TABLE `playeractivitysnapshots` ENABLE KEYS */;

--
-- Definition of populationbloodmodels
--

DROP TABLE IF EXISTS `populationbloodmodels`;
CREATE TABLE IF NOT EXISTS `populationbloodmodels` (
  `Id` bigint NOT NULL AUTO_INCREMENT,
  `Name` varchar(200) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Dumping data for table populationbloodmodels
--

/*!40000 ALTER TABLE `populationbloodmodels` DISABLE KEYS */;
/*!40000 ALTER TABLE `populationbloodmodels` ENABLE KEYS */;

--
-- Definition of populationbloodmodels_bloodtypes
--

DROP TABLE IF EXISTS `populationbloodmodels_bloodtypes`;
CREATE TABLE IF NOT EXISTS `populationbloodmodels_bloodtypes` (
  `BloodtypeId` bigint NOT NULL,
  `PopulationBloodModelId` bigint NOT NULL,
  `Weight` double NOT NULL DEFAULT '1',
  PRIMARY KEY (`BloodtypeId`,`PopulationBloodModelId`),
  KEY `FK_PopulationBloodModels_Bloodtypes_PopulationBloodModels_idx` (`PopulationBloodModelId`),
  CONSTRAINT `FK_PopulationBloodModels_Bloodtypes_Bloodtypes` FOREIGN KEY (`BloodtypeId`) REFERENCES `bloodtypes` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_PopulationBloodModels_Bloodtypes_PopulationBloodModels` FOREIGN KEY (`PopulationBloodModelId`) REFERENCES `populationbloodmodels` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Dumping data for table populationbloodmodels_bloodtypes
--

/*!40000 ALTER TABLE `populationbloodmodels_bloodtypes` DISABLE KEYS */;
/*!40000 ALTER TABLE `populationbloodmodels_bloodtypes` ENABLE KEYS */;

--
-- Definition of progschedules
--

DROP TABLE IF EXISTS `progschedules`;
CREATE TABLE IF NOT EXISTS `progschedules` (
  `Id` bigint NOT NULL AUTO_INCREMENT,
  `Name` varchar(200) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL,
  `IntervalType` int NOT NULL,
  `IntervalModifier` int NOT NULL,
  `IntervalOther` int NOT NULL,
  `ReferenceTime` varchar(500) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL,
  `ReferenceDate` varchar(500) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL,
  `FutureProgId` bigint NOT NULL,
  `IntervalFallback` int NOT NULL DEFAULT '0',
  `IntervalOtherSecondary` int NOT NULL DEFAULT '0',
  PRIMARY KEY (`Id`),
  KEY `FK_ProgSchedules_FutureProgs_idx` (`FutureProgId`),
  CONSTRAINT `FK_ProgSchedules_FutureProgs` FOREIGN KEY (`FutureProgId`) REFERENCES `futureprogs` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Dumping data for table progschedules
--

/*!40000 ALTER TABLE `progschedules` DISABLE KEYS */;
/*!40000 ALTER TABLE `progschedules` ENABLE KEYS */;

--
-- Definition of projectpayables
--

DROP TABLE IF EXISTS `projectpayables`;
CREATE TABLE IF NOT EXISTS `projectpayables` (
  `Id` bigint NOT NULL AUTO_INCREMENT,
  `ActiveProjectId` bigint DEFAULT NULL,
  `ProjectDefinitionId` bigint NOT NULL,
  `ProjectRevisionNumber` int NOT NULL,
  `ProjectName` varchar(500) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `ProjectOwnerCharacterId` bigint DEFAULT NULL,
  `CharacterId` bigint NOT NULL,
  `CurrencyId` bigint NOT NULL,
  `Amount` decimal(58,29) NOT NULL,
  `PayableType` int NOT NULL,
  `ProjectLabourRequirementId` bigint DEFAULT NULL,
  `RequirementName` varchar(500) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci DEFAULT NULL,
  `Reason` mediumtext CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `EarnedAt` datetime NOT NULL,
  `ClaimedAt` datetime DEFAULT NULL,
  `ClaimedBankAccountId` bigint DEFAULT NULL,
  PRIMARY KEY (`Id`),
  KEY `FK_ProjectPayables_BankAccounts_idx` (`ClaimedBankAccountId`),
  KEY `FK_ProjectPayables_Currencies_idx` (`CurrencyId`),
  KEY `IX_ProjectPayables_ActiveProjectId` (`ActiveProjectId`),
  KEY `IX_ProjectPayables_Character_Claimed` (`CharacterId`,`ClaimedAt`),
  CONSTRAINT `FK_ProjectPayables_BankAccounts` FOREIGN KEY (`ClaimedBankAccountId`) REFERENCES `bankaccounts` (`Id`) ON DELETE SET NULL,
  CONSTRAINT `FK_ProjectPayables_Currencies` FOREIGN KEY (`CurrencyId`) REFERENCES `currencies` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Dumping data for table projectpayables
--

/*!40000 ALTER TABLE `projectpayables` DISABLE KEYS */;
/*!40000 ALTER TABLE `projectpayables` ENABLE KEYS */;

--
-- Definition of projects
--

DROP TABLE IF EXISTS `projects`;
CREATE TABLE IF NOT EXISTS `projects` (
  `Id` bigint NOT NULL,
  `RevisionNumber` int NOT NULL,
  `Type` varchar(45) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `Name` varchar(200) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `EditableItemId` bigint NOT NULL,
  `Definition` text CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci,
  `AppearInJobsList` bit(1) NOT NULL DEFAULT b'0',
  PRIMARY KEY (`Id`,`RevisionNumber`),
  KEY `FK_Projects_EditableItems_idx` (`EditableItemId`),
  CONSTRAINT `FK_Projects_EditableItems` FOREIGN KEY (`EditableItemId`) REFERENCES `editableitems` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Dumping data for table projects
--

/*!40000 ALTER TABLE `projects` DISABLE KEYS */;
/*!40000 ALTER TABLE `projects` ENABLE KEYS */;

--
-- Definition of activeprojects
--

DROP TABLE IF EXISTS `activeprojects`;
CREATE TABLE IF NOT EXISTS `activeprojects` (
  `Id` bigint NOT NULL AUTO_INCREMENT,
  `ProjectId` bigint NOT NULL,
  `ProjectRevisionNumber` int NOT NULL,
  `CurrentPhaseId` bigint NOT NULL,
  `CharacterId` bigint DEFAULT NULL,
  `CellId` bigint DEFAULT NULL,
  `PaymentCurrencyId` bigint DEFAULT NULL,
  `RoomLayer` int NOT NULL DEFAULT '0',
  `RoutePosition` decimal(18,3) DEFAULT NULL,
  PRIMARY KEY (`Id`),
  KEY `FK_ActiveProjects_Cells_idx` (`CellId`),
  KEY `FK_ActiveProjects_Characters_idx` (`CharacterId`),
  KEY `FK_ActiveProjects_ProjectPhases_idx` (`CurrentPhaseId`),
  KEY `FK_ActiveProjects_Projects_idx` (`ProjectId`,`ProjectRevisionNumber`),
  KEY `IX_ActiveProjects_PaymentCurrencyId` (`PaymentCurrencyId`),
  CONSTRAINT `FK_ActiveProjects_Cells` FOREIGN KEY (`CellId`) REFERENCES `cells` (`Id`) ON DELETE RESTRICT,
  CONSTRAINT `FK_ActiveProjects_Characters` FOREIGN KEY (`CharacterId`) REFERENCES `characters` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_ActiveProjects_PaymentCurrencies` FOREIGN KEY (`PaymentCurrencyId`) REFERENCES `currencies` (`Id`) ON DELETE SET NULL,
  CONSTRAINT `FK_ActiveProjects_ProjectPhases` FOREIGN KEY (`CurrentPhaseId`) REFERENCES `projectphases` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_ActiveProjects_Projects` FOREIGN KEY (`ProjectId`, `ProjectRevisionNumber`) REFERENCES `projects` (`Id`, `RevisionNumber`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Dumping data for table activeprojects
--

/*!40000 ALTER TABLE `activeprojects` DISABLE KEYS */;
/*!40000 ALTER TABLE `activeprojects` ENABLE KEYS */;

--
-- Definition of agricultureoperations
--

DROP TABLE IF EXISTS `agricultureoperations`;
CREATE TABLE IF NOT EXISTS `agricultureoperations` (
  `Id` bigint NOT NULL AUTO_INCREMENT,
  `Name` varchar(200) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `Description` mediumtext CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `OperationType` int NOT NULL,
  `TargetType` int NOT NULL,
  `RequiredUse` int NOT NULL,
  `ResultUse` int NOT NULL,
  `ProjectId` bigint NOT NULL,
  `ProjectRevisionNumber` int NOT NULL,
  `CompletionProgId` bigint DEFAULT NULL,
  `Definition` mediumtext CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  PRIMARY KEY (`Id`),
  KEY `FK_AgricultureOperations_FutureProgs_idx` (`CompletionProgId`),
  KEY `FK_AgricultureOperations_Projects_idx` (`ProjectId`,`ProjectRevisionNumber`),
  CONSTRAINT `FK_AgricultureOperations_FutureProgs` FOREIGN KEY (`CompletionProgId`) REFERENCES `futureprogs` (`Id`) ON DELETE SET NULL,
  CONSTRAINT `FK_AgricultureOperations_Projects` FOREIGN KEY (`ProjectId`, `ProjectRevisionNumber`) REFERENCES `projects` (`Id`, `RevisionNumber`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Dumping data for table agricultureoperations
--

/*!40000 ALTER TABLE `agricultureoperations` DISABLE KEYS */;
/*!40000 ALTER TABLE `agricultureoperations` ENABLE KEYS */;

--
-- Definition of agricultureprojectcontexts
--

DROP TABLE IF EXISTS `agricultureprojectcontexts`;
CREATE TABLE IF NOT EXISTS `agricultureprojectcontexts` (
  `ActiveProjectId` bigint NOT NULL,
  `AgricultureFieldId` bigint NOT NULL,
  `OperationId` bigint NOT NULL,
  `TargetType` int NOT NULL,
  `TargetId` bigint DEFAULT NULL,
  `TargetText` varchar(500) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci DEFAULT NULL,
  `ActorId` bigint DEFAULT NULL,
  `Definition` mediumtext CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  PRIMARY KEY (`ActiveProjectId`),
  KEY `FK_AgricultureProjectContexts_Actors_idx` (`ActorId`),
  KEY `FK_AgricultureProjectContexts_Fields_idx` (`AgricultureFieldId`),
  KEY `FK_AgricultureProjectContexts_Operations_idx` (`OperationId`),
  CONSTRAINT `FK_AgricultureProjectContexts_ActiveProjects` FOREIGN KEY (`ActiveProjectId`) REFERENCES `activeprojects` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_AgricultureProjectContexts_Characters` FOREIGN KEY (`ActorId`) REFERENCES `characters` (`Id`) ON DELETE SET NULL,
  CONSTRAINT `FK_AgricultureProjectContexts_Fields` FOREIGN KEY (`AgricultureFieldId`) REFERENCES `agriculturefields` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_AgricultureProjectContexts_Operations` FOREIGN KEY (`OperationId`) REFERENCES `agricultureoperations` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Dumping data for table agricultureprojectcontexts
--

/*!40000 ALTER TABLE `agricultureprojectcontexts` DISABLE KEYS */;
/*!40000 ALTER TABLE `agricultureprojectcontexts` ENABLE KEYS */;

--
-- Definition of projectphases
--

DROP TABLE IF EXISTS `projectphases`;
CREATE TABLE IF NOT EXISTS `projectphases` (
  `Id` bigint NOT NULL AUTO_INCREMENT,
  `ProjectId` bigint NOT NULL,
  `ProjectRevisionNumber` int NOT NULL,
  `PhaseNumber` int NOT NULL,
  `Description` text CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  PRIMARY KEY (`Id`),
  KEY `FK_ProjectPhases_Projects_idx` (`ProjectId`,`ProjectRevisionNumber`),
  CONSTRAINT `FK_ProjectPhases_Projects` FOREIGN KEY (`ProjectId`, `ProjectRevisionNumber`) REFERENCES `projects` (`Id`, `RevisionNumber`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Dumping data for table projectphases
--

/*!40000 ALTER TABLE `projectphases` DISABLE KEYS */;
/*!40000 ALTER TABLE `projectphases` ENABLE KEYS */;

--
-- Definition of projectactions
--

DROP TABLE IF EXISTS `projectactions`;
CREATE TABLE IF NOT EXISTS `projectactions` (
  `Id` bigint NOT NULL AUTO_INCREMENT,
  `Name` varchar(200) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `Description` varchar(1000) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `Type` varchar(45) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `SortOrder` int NOT NULL,
  `Definition` text CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `ProjectPhaseId` bigint NOT NULL,
  PRIMARY KEY (`Id`),
  KEY `FK_ProjectActions_ProjectPhases_idx` (`ProjectPhaseId`),
  CONSTRAINT `FK_ProjectActions_ProjectPhases` FOREIGN KEY (`ProjectPhaseId`) REFERENCES `projectphases` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Dumping data for table projectactions
--

/*!40000 ALTER TABLE `projectactions` DISABLE KEYS */;
/*!40000 ALTER TABLE `projectactions` ENABLE KEYS */;

--
-- Definition of projectlabourrequirements
--

DROP TABLE IF EXISTS `projectlabourrequirements`;
CREATE TABLE IF NOT EXISTS `projectlabourrequirements` (
  `Id` bigint NOT NULL AUTO_INCREMENT,
  `Type` varchar(45) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `Name` varchar(200) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `ProjectPhaseId` bigint NOT NULL,
  `Description` text CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `TotalProgressRequired` double NOT NULL,
  `MaximumSimultaneousWorkers` int NOT NULL,
  `Definition` text CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  PRIMARY KEY (`Id`),
  KEY `FK_ProjectLabourRequirements_ProjectPhases_idx` (`ProjectPhaseId`),
  CONSTRAINT `FK_ProjectLabourRequirements_ProjectPhases` FOREIGN KEY (`ProjectPhaseId`) REFERENCES `projectphases` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Dumping data for table projectlabourrequirements
--

/*!40000 ALTER TABLE `projectlabourrequirements` DISABLE KEYS */;
/*!40000 ALTER TABLE `projectlabourrequirements` ENABLE KEYS */;

--
-- Definition of activeprojectlabours
--

DROP TABLE IF EXISTS `activeprojectlabours`;
CREATE TABLE IF NOT EXISTS `activeprojectlabours` (
  `ActiveProjectId` bigint NOT NULL,
  `ProjectLabourRequirementsId` bigint NOT NULL,
  `Progress` double NOT NULL,
  `PaymentPerHour` decimal(58,29) NOT NULL DEFAULT '0.00000000000000000000000000000',
  PRIMARY KEY (`ActiveProjectId`,`ProjectLabourRequirementsId`),
  KEY `FK_ActiveProjectLabours_ProjectLabourRequirements_idx` (`ProjectLabourRequirementsId`),
  CONSTRAINT `FK_ActiveProjectLabours_ActiveProjects` FOREIGN KEY (`ActiveProjectId`) REFERENCES `activeprojects` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_ActiveProjectLabours_ProjectLabourRequirements` FOREIGN KEY (`ProjectLabourRequirementsId`) REFERENCES `projectlabourrequirements` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Dumping data for table activeprojectlabours
--

/*!40000 ALTER TABLE `activeprojectlabours` DISABLE KEYS */;
/*!40000 ALTER TABLE `activeprojectlabours` ENABLE KEYS */;

--
-- Definition of projectlabourimpacts
--

DROP TABLE IF EXISTS `projectlabourimpacts`;
CREATE TABLE IF NOT EXISTS `projectlabourimpacts` (
  `Id` bigint NOT NULL AUTO_INCREMENT,
  `Name` varchar(200) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `Description` varchar(1000) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `Type` varchar(45) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `Definition` text CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `ProjectLabourRequirementId` bigint NOT NULL,
  `MinimumHoursForImpactToKickIn` double NOT NULL,
  PRIMARY KEY (`Id`),
  KEY `FK_ProjectLabourImpacts_ProjectLabourRequirements_idx` (`ProjectLabourRequirementId`),
  CONSTRAINT `FK_ProjectLabourImpacts_ProjectLabourRequirements` FOREIGN KEY (`ProjectLabourRequirementId`) REFERENCES `projectlabourrequirements` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Dumping data for table projectlabourimpacts
--

/*!40000 ALTER TABLE `projectlabourimpacts` DISABLE KEYS */;
/*!40000 ALTER TABLE `projectlabourimpacts` ENABLE KEYS */;

--
-- Definition of projectlabourqueues
--

DROP TABLE IF EXISTS `projectlabourqueues`;
CREATE TABLE IF NOT EXISTS `projectlabourqueues` (
  `Id` bigint NOT NULL AUTO_INCREMENT,
  `CharacterId` bigint NOT NULL,
  `ActiveProjectId` bigint NOT NULL,
  `ProjectLabourRequirementId` bigint NOT NULL,
  `QueueOrder` int NOT NULL,
  `QueuedAt` datetime NOT NULL,
  PRIMARY KEY (`Id`),
  UNIQUE KEY `IX_ProjectLabourQueues_Character_QueueOrder` (`CharacterId`,`QueueOrder`),
  KEY `FK_ProjectLabourQueues_ActiveProjects_idx` (`ActiveProjectId`),
  KEY `FK_ProjectLabourQueues_Characters_idx` (`CharacterId`),
  KEY `FK_ProjectLabourQueues_ProjectLabourRequirements_idx` (`ProjectLabourRequirementId`),
  CONSTRAINT `FK_ProjectLabourQueues_ActiveProjects` FOREIGN KEY (`ActiveProjectId`) REFERENCES `activeprojects` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_ProjectLabourQueues_Characters` FOREIGN KEY (`CharacterId`) REFERENCES `characters` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_ProjectLabourQueues_ProjectLabourRequirements` FOREIGN KEY (`ProjectLabourRequirementId`) REFERENCES `projectlabourrequirements` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Dumping data for table projectlabourqueues
--

/*!40000 ALTER TABLE `projectlabourqueues` DISABLE KEYS */;
/*!40000 ALTER TABLE `projectlabourqueues` ENABLE KEYS */;

--
-- Definition of projectmaterialrequirements
--

DROP TABLE IF EXISTS `projectmaterialrequirements`;
CREATE TABLE IF NOT EXISTS `projectmaterialrequirements` (
  `Id` bigint NOT NULL AUTO_INCREMENT,
  `Type` varchar(45) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `ProjectPhaseId` bigint NOT NULL,
  `Name` varchar(200) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `Description` text CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `Definition` text CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `IsMandatoryForProjectCompletion` bit(1) NOT NULL DEFAULT b'1',
  PRIMARY KEY (`Id`),
  KEY `FK_ProjectMaterialRequirements_ProjectPhases_idx` (`ProjectPhaseId`),
  CONSTRAINT `FK_ProjectMaterialRequirements_ProjectPhases` FOREIGN KEY (`ProjectPhaseId`) REFERENCES `projectphases` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Dumping data for table projectmaterialrequirements
--

/*!40000 ALTER TABLE `projectmaterialrequirements` DISABLE KEYS */;
/*!40000 ALTER TABLE `projectmaterialrequirements` ENABLE KEYS */;

--
-- Definition of activeprojectmaterials
--

DROP TABLE IF EXISTS `activeprojectmaterials`;
CREATE TABLE IF NOT EXISTS `activeprojectmaterials` (
  `ActiveProjectId` bigint NOT NULL,
  `ProjectMaterialRequirementsId` bigint NOT NULL,
  `Progress` double NOT NULL,
  `PaymentPerUnit` decimal(58,29) NOT NULL DEFAULT '0.00000000000000000000000000000',
  PRIMARY KEY (`ActiveProjectId`,`ProjectMaterialRequirementsId`),
  KEY `FK_ActiveProjectMaterials_ProjectMaterialRequirements_idx` (`ProjectMaterialRequirementsId`),
  CONSTRAINT `FK_ActiveProjectMaterials_ActiveProjects` FOREIGN KEY (`ActiveProjectId`) REFERENCES `activeprojects` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_ActiveProjectMaterials_ProjectMaterialRequirements` FOREIGN KEY (`ProjectMaterialRequirementsId`) REFERENCES `projectmaterialrequirements` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Dumping data for table activeprojectmaterials
--

/*!40000 ALTER TABLE `activeprojectmaterials` DISABLE KEYS */;
/*!40000 ALTER TABLE `activeprojectmaterials` ENABLE KEYS */;

--
-- Definition of races
--

DROP TABLE IF EXISTS `races`;
CREATE TABLE IF NOT EXISTS `races` (
  `Id` bigint NOT NULL AUTO_INCREMENT,
  `Name` varchar(255) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `Description` varchar(4000) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `BaseBodyId` bigint NOT NULL,
  `AllowedGenders` varchar(255) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `ParentRaceId` bigint DEFAULT NULL,
  `AttributeTotalCap` int NOT NULL,
  `IndividualAttributeCap` int NOT NULL,
  `DiceExpression` varchar(255) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `IlluminationPerceptionMultiplier` double NOT NULL DEFAULT '1',
  `AvailabilityProgId` bigint DEFAULT NULL,
  `CorpseModelId` bigint NOT NULL,
  `DefaultHealthStrategyId` bigint NOT NULL,
  `CanUseWeapons` bit(1) NOT NULL DEFAULT b'1',
  `CanAttack` bit(1) NOT NULL DEFAULT b'1',
  `CanDefend` bit(1) NOT NULL DEFAULT b'1',
  `NaturalArmourTypeId` bigint DEFAULT NULL,
  `NaturalArmourQuality` bigint NOT NULL,
  `NaturalArmourMaterialId` bigint DEFAULT NULL,
  `BloodLiquidId` bigint DEFAULT NULL,
  `NeedsToBreathe` bit(1) NOT NULL DEFAULT b'1',
  `SweatLiquidId` bigint DEFAULT NULL,
  `SweatRateInLitresPerMinute` double NOT NULL DEFAULT '0.8',
  `SizeStanding` int NOT NULL DEFAULT '6',
  `SizeProne` int NOT NULL DEFAULT '5',
  `SizeSitting` int NOT NULL DEFAULT '6',
  `CommunicationStrategyType` varchar(45) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL DEFAULT 'humanoid',
  `DefaultHandedness` int NOT NULL DEFAULT '3',
  `HandednessOptions` varchar(100) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL DEFAULT '1 3',
  `MaximumDragWeightExpression` text CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `MaximumLiftWeightExpression` text CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `RaceButcheryProfileId` bigint DEFAULT NULL,
  `BloodModelId` bigint DEFAULT NULL,
  `RaceUsesStamina` bit(1) NOT NULL DEFAULT b'1',
  `CanEatCorpses` bit(1) NOT NULL DEFAULT b'0',
  `BiteWeight` double NOT NULL DEFAULT '1000',
  `EatCorpseEmoteText` varchar(500) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL DEFAULT '@ eat|eats {0}$1',
  `CanEatMaterialsOptIn` bit(1) NOT NULL DEFAULT b'0',
  `TemperatureRangeFloor` double NOT NULL,
  `TemperatureRangeCeiling` double NOT NULL DEFAULT '40',
  `BodypartSizeModifier` int NOT NULL,
  `BodypartHealthMultiplier` double NOT NULL DEFAULT '1',
  `BreathingVolumeExpression` varchar(500) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL DEFAULT '7',
  `HoldBreathLengthExpression` varchar(500) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL DEFAULT '120',
  `CanClimb` bit(1) NOT NULL DEFAULT b'0',
  `CanSwim` bit(1) NOT NULL DEFAULT b'1',
  `MinimumSleepingPosition` int NOT NULL DEFAULT '4',
  `ChildAge` int NOT NULL,
  `YouthAge` int NOT NULL,
  `YoungAdultAge` int NOT NULL,
  `AdultAge` int NOT NULL,
  `ElderAge` int NOT NULL,
  `VenerableAge` int NOT NULL,
  `BreathingModel` varchar(100) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci DEFAULT 'simple',
  `DefaultHeightWeightModelFemaleId` bigint DEFAULT NULL,
  `DefaultHeightWeightModelMaleId` bigint DEFAULT NULL,
  `DefaultHeightWeightModelNeuterId` bigint DEFAULT NULL,
  `DefaultHeightWeightModelNonBinaryId` bigint DEFAULT NULL,
  `HungerRate` double NOT NULL DEFAULT '1',
  `ThirstRate` double NOT NULL DEFAULT '1',
  `TrackIntensityOlfactory` double NOT NULL DEFAULT '1',
  `TrackIntensityVisual` double NOT NULL DEFAULT '1',
  `TrackingAbilityOlfactory` double NOT NULL DEFAULT '0',
  `TrackingAbilityVisual` double NOT NULL DEFAULT '1',
  `DefaultCombatSettingId` bigint DEFAULT NULL,
  `MaximumDrinkSatiatedHours` double NOT NULL DEFAULT '8',
  `MaximumFoodSatiatedHours` double NOT NULL DEFAULT '16',
  `DefaultAlertEmote` varchar(500) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci DEFAULT NULL,
  `DefaultDistantAlertEmote` varchar(500) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci DEFAULT NULL,
  PRIMARY KEY (`Id`),
  KEY `FK_Races_AvailabilityProg` (`AvailabilityProgId`),
  KEY `FK_Races_BodyProtos` (`BaseBodyId`),
  KEY `FK_Races_Liquids_Blood_idx` (`BloodLiquidId`),
  KEY `FK_Races_BloodModels_idx` (`BloodModelId`),
  KEY `FK_Races_CorpseModels_idx` (`CorpseModelId`),
  KEY `FK_Races_HealthStrategies_idx` (`DefaultHealthStrategyId`),
  KEY `FK_Races_Materials_idx` (`NaturalArmourMaterialId`),
  KEY `FK_Races_ArmourTypes_idx` (`NaturalArmourTypeId`),
  KEY `FK_Races_Races` (`ParentRaceId`),
  KEY `FK_Races_RaceButcheryProfiles_idx` (`RaceButcheryProfileId`),
  KEY `FK_Races_Liqiuds_Sweat_idx` (`SweatLiquidId`),
  KEY `IX_Races_DefaultHeightWeightModelFemaleId` (`DefaultHeightWeightModelFemaleId`),
  KEY `IX_Races_DefaultHeightWeightModelMaleId` (`DefaultHeightWeightModelMaleId`),
  KEY `IX_Races_DefaultHeightWeightModelNeuterId` (`DefaultHeightWeightModelNeuterId`),
  KEY `IX_Races_DefaultHeightWeightModelNonBinaryId` (`DefaultHeightWeightModelNonBinaryId`),
  KEY `IX_Races_DefaultCombatSettingId` (`DefaultCombatSettingId`),
  CONSTRAINT `FK_Races_ArmourTypes` FOREIGN KEY (`NaturalArmourTypeId`) REFERENCES `armourtypes` (`Id`) ON DELETE SET NULL,
  CONSTRAINT `FK_Races_AvailabilityProg` FOREIGN KEY (`AvailabilityProgId`) REFERENCES `futureprogs` (`Id`) ON DELETE SET NULL,
  CONSTRAINT `FK_Races_BloodModels` FOREIGN KEY (`BloodModelId`) REFERENCES `bloodmodels` (`Id`) ON DELETE SET NULL,
  CONSTRAINT `FK_Races_BodyProtos` FOREIGN KEY (`BaseBodyId`) REFERENCES `bodyprotos` (`Id`) ON DELETE RESTRICT,
  CONSTRAINT `FK_Races_CharacterCombatSettings` FOREIGN KEY (`DefaultCombatSettingId`) REFERENCES `charactercombatsettings` (`Id`) ON DELETE SET NULL,
  CONSTRAINT `FK_Races_CorpseModels` FOREIGN KEY (`CorpseModelId`) REFERENCES `corpsemodels` (`Id`) ON DELETE RESTRICT,
  CONSTRAINT `FK_Races_HealthStrategies` FOREIGN KEY (`DefaultHealthStrategyId`) REFERENCES `healthstrategies` (`Id`) ON DELETE RESTRICT,
  CONSTRAINT `FK_Races_HeightWeightModelsFemale` FOREIGN KEY (`DefaultHeightWeightModelFemaleId`) REFERENCES `heightweightmodels` (`Id`),
  CONSTRAINT `FK_Races_HeightWeightModelsMale` FOREIGN KEY (`DefaultHeightWeightModelMaleId`) REFERENCES `heightweightmodels` (`Id`),
  CONSTRAINT `FK_Races_HeightWeightModelsNeuter` FOREIGN KEY (`DefaultHeightWeightModelNeuterId`) REFERENCES `heightweightmodels` (`Id`),
  CONSTRAINT `FK_Races_HeightWeightModelsNonBinary` FOREIGN KEY (`DefaultHeightWeightModelNonBinaryId`) REFERENCES `heightweightmodels` (`Id`),
  CONSTRAINT `FK_Races_Liqiuds_Sweat` FOREIGN KEY (`SweatLiquidId`) REFERENCES `liquids` (`Id`) ON DELETE SET NULL,
  CONSTRAINT `FK_Races_Liquids_Blood` FOREIGN KEY (`BloodLiquidId`) REFERENCES `liquids` (`Id`) ON DELETE RESTRICT,
  CONSTRAINT `FK_Races_Materials` FOREIGN KEY (`NaturalArmourMaterialId`) REFERENCES `materials` (`Id`) ON DELETE SET NULL,
  CONSTRAINT `FK_Races_RaceButcheryProfiles` FOREIGN KEY (`RaceButcheryProfileId`) REFERENCES `racebutcheryprofiles` (`Id`) ON DELETE SET NULL,
  CONSTRAINT `FK_Races_Races` FOREIGN KEY (`ParentRaceId`) REFERENCES `races` (`Id`) ON DELETE RESTRICT
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Dumping data for table races
--

/*!40000 ALTER TABLE `races` DISABLE KEYS */;
/*!40000 ALTER TABLE `races` ENABLE KEYS */;

--
-- Definition of bodies
--

DROP TABLE IF EXISTS `bodies`;
CREATE TABLE IF NOT EXISTS `bodies` (
  `Id` bigint NOT NULL AUTO_INCREMENT,
  `BodyPrototypeID` bigint NOT NULL,
  `Height` double NOT NULL,
  `Weight` double NOT NULL,
  `Position` bigint NOT NULL,
  `CurrentSpeed` bigint DEFAULT NULL,
  `RaceId` bigint NOT NULL,
  `CurrentStamina` double NOT NULL,
  `CurrentBloodVolume` double NOT NULL DEFAULT '-1',
  `EthnicityId` bigint NOT NULL,
  `BloodtypeId` bigint DEFAULT NULL,
  `Gender` smallint NOT NULL,
  `ShortDescription` varchar(1000) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci DEFAULT NULL,
  `FullDescription` varchar(4000) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci DEFAULT NULL,
  `ShortDescriptionPatternId` bigint DEFAULT NULL,
  `FullDescriptionPatternId` bigint DEFAULT NULL,
  `Tattoos` mediumtext CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci,
  `HeldBreathLength` int NOT NULL,
  `EffectData` mediumtext CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `Scars` mediumtext CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci,
  `HealthStrategyId` bigint DEFAULT NULL,
  `DominantHandAlignment` int NOT NULL DEFAULT '3',
  `SurfaceLiquidData` mediumtext CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci,
  PRIMARY KEY (`Id`),
  KEY `FK_Bodies_Bloodtypes_idx` (`BloodtypeId`),
  KEY `FK_Bodies_Ethnicities_idx` (`EthnicityId`),
  KEY `FK_Bodies_EntityDescriptionPatterns_Full_idx` (`FullDescriptionPatternId`),
  KEY `FK_Bodies_Races` (`RaceId`),
  KEY `FK_Bodies_EntityDescriptionPatterns_Short_idx` (`ShortDescriptionPatternId`),
  KEY `IX_Bodies_HealthStrategyId` (`HealthStrategyId`),
  CONSTRAINT `FK_Bodies_Bloodtypes` FOREIGN KEY (`BloodtypeId`) REFERENCES `bloodtypes` (`Id`) ON DELETE SET NULL,
  CONSTRAINT `FK_Bodies_EntityDescriptionPatterns_Full` FOREIGN KEY (`FullDescriptionPatternId`) REFERENCES `entitydescriptionpatterns` (`Id`) ON DELETE RESTRICT,
  CONSTRAINT `FK_Bodies_EntityDescriptionPatterns_Short` FOREIGN KEY (`ShortDescriptionPatternId`) REFERENCES `entitydescriptionpatterns` (`Id`) ON DELETE RESTRICT,
  CONSTRAINT `FK_Bodies_Ethnicities` FOREIGN KEY (`EthnicityId`) REFERENCES `ethnicities` (`Id`) ON DELETE RESTRICT,
  CONSTRAINT `FK_Bodies_HealthStrategies_HealthStrategyId` FOREIGN KEY (`HealthStrategyId`) REFERENCES `healthstrategies` (`Id`),
  CONSTRAINT `FK_Bodies_Races` FOREIGN KEY (`RaceId`) REFERENCES `races` (`Id`) ON DELETE RESTRICT
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Dumping data for table bodies
--

/*!40000 ALTER TABLE `bodies` DISABLE KEYS */;
/*!40000 ALTER TABLE `bodies` ENABLE KEYS */;

--
-- Definition of chargenadvices_races
--

DROP TABLE IF EXISTS `chargenadvices_races`;
CREATE TABLE IF NOT EXISTS `chargenadvices_races` (
  `ChargenAdviceId` bigint NOT NULL,
  `RaceId` bigint NOT NULL,
  PRIMARY KEY (`ChargenAdviceId`,`RaceId`),
  KEY `FK_ChargenAdvices_Races_Races_idx` (`RaceId`),
  CONSTRAINT `FK_ChargenAdvices_Races_ChargenAdvices` FOREIGN KEY (`ChargenAdviceId`) REFERENCES `chargenadvices` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_ChargenAdvices_Races_Races` FOREIGN KEY (`RaceId`) REFERENCES `races` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Dumping data for table chargenadvices_races
--

/*!40000 ALTER TABLE `chargenadvices_races` DISABLE KEYS */;
/*!40000 ALTER TABLE `chargenadvices_races` ENABLE KEYS */;

--
-- Definition of ethnicities
--

DROP TABLE IF EXISTS `ethnicities`;
CREATE TABLE IF NOT EXISTS `ethnicities` (
  `Id` bigint NOT NULL AUTO_INCREMENT,
  `Name` varchar(4000) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `ChargenBlurb` varchar(4000) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `AvailabilityProgId` bigint DEFAULT NULL,
  `ParentRaceId` bigint DEFAULT NULL,
  `EthnicGroup` text CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci,
  `EthnicSubgroup` text CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci,
  `PopulationBloodModelId` bigint DEFAULT NULL,
  `TolerableTemperatureFloorEffect` double NOT NULL,
  `TolerableTemperatureCeilingEffect` double NOT NULL,
  PRIMARY KEY (`Id`),
  KEY `FK_Ethnicities_AvailabilityProg` (`AvailabilityProgId`),
  KEY `FK_Ethnicities_Races_idx` (`ParentRaceId`),
  KEY `FK_Ethnicities_PopulationBloodModels_idx` (`PopulationBloodModelId`),
  CONSTRAINT `FK_Ethnicities_AvailabilityProg` FOREIGN KEY (`AvailabilityProgId`) REFERENCES `futureprogs` (`Id`) ON DELETE SET NULL,
  CONSTRAINT `FK_Ethnicities_PopulationBloodModels` FOREIGN KEY (`PopulationBloodModelId`) REFERENCES `populationbloodmodels` (`Id`) ON DELETE SET NULL,
  CONSTRAINT `FK_Ethnicities_Races` FOREIGN KEY (`ParentRaceId`) REFERENCES `races` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Dumping data for table ethnicities
--

/*!40000 ALTER TABLE `ethnicities` DISABLE KEYS */;
/*!40000 ALTER TABLE `ethnicities` ENABLE KEYS */;

--
-- Definition of chargenadvices_ethnicities
--

DROP TABLE IF EXISTS `chargenadvices_ethnicities`;
CREATE TABLE IF NOT EXISTS `chargenadvices_ethnicities` (
  `ChargenAdviceId` bigint NOT NULL,
  `EthnicityId` bigint NOT NULL,
  PRIMARY KEY (`ChargenAdviceId`,`EthnicityId`),
  KEY `FK_ChargenAdvices_Ethnicities_Ethnicities_idx` (`EthnicityId`),
  CONSTRAINT `FK_ChargenAdvices_Ethnicities_ChargenAdvices` FOREIGN KEY (`ChargenAdviceId`) REFERENCES `chargenadvices` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_ChargenAdvices_Ethnicities_Ethnicities` FOREIGN KEY (`EthnicityId`) REFERENCES `ethnicities` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Dumping data for table chargenadvices_ethnicities
--

/*!40000 ALTER TABLE `chargenadvices_ethnicities` DISABLE KEYS */;
/*!40000 ALTER TABLE `chargenadvices_ethnicities` ENABLE KEYS */;

--
-- Definition of ethnicities_characteristics
--

DROP TABLE IF EXISTS `ethnicities_characteristics`;
CREATE TABLE IF NOT EXISTS `ethnicities_characteristics` (
  `EthnicityId` bigint NOT NULL,
  `CharacteristicDefinitionId` bigint NOT NULL,
  `CharacteristicProfileId` bigint NOT NULL,
  PRIMARY KEY (`EthnicityId`,`CharacteristicDefinitionId`,`CharacteristicProfileId`),
  KEY `FK_Ethnicities_Characteristics_CharacteristicDefinitions` (`CharacteristicDefinitionId`),
  KEY `FK_Ethnicities_Characteristics_CharacteristicProfiles` (`CharacteristicProfileId`),
  CONSTRAINT `FK_Ethnicities_Characteristics_CharacteristicDefinitions` FOREIGN KEY (`CharacteristicDefinitionId`) REFERENCES `characteristicdefinitions` (`Id`) ON DELETE RESTRICT,
  CONSTRAINT `FK_Ethnicities_Characteristics_CharacteristicProfiles` FOREIGN KEY (`CharacteristicProfileId`) REFERENCES `characteristicprofiles` (`Id`) ON DELETE RESTRICT,
  CONSTRAINT `FK_Ethnicities_Characteristics_Ethnicities` FOREIGN KEY (`EthnicityId`) REFERENCES `ethnicities` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Dumping data for table ethnicities_characteristics
--

/*!40000 ALTER TABLE `ethnicities_characteristics` DISABLE KEYS */;
/*!40000 ALTER TABLE `ethnicities_characteristics` ENABLE KEYS */;

--
-- Definition of ethnicities_chargenresources
--

DROP TABLE IF EXISTS `ethnicities_chargenresources`;
CREATE TABLE IF NOT EXISTS `ethnicities_chargenresources` (
  `EthnicityId` bigint NOT NULL,
  `ChargenResourceId` bigint NOT NULL,
  `RequirementOnly` bit(1) NOT NULL DEFAULT b'0',
  `Amount` int NOT NULL,
  PRIMARY KEY (`EthnicityId`,`ChargenResourceId`),
  KEY `IX_Ethnicities_ChargenResources_ChargenResourceId` (`ChargenResourceId`),
  CONSTRAINT `FK_Ethnicities_ChargenResources_ChargenResources_ChargenResourc~` FOREIGN KEY (`ChargenResourceId`) REFERENCES `chargenresources` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_Ethnicities_ChargenResources_Ethnicities_EthnicityId` FOREIGN KEY (`EthnicityId`) REFERENCES `ethnicities` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Dumping data for table ethnicities_chargenresources
--

/*!40000 ALTER TABLE `ethnicities_chargenresources` DISABLE KEYS */;
/*!40000 ALTER TABLE `ethnicities_chargenresources` ENABLE KEYS */;

--
-- Definition of raceedibleforagableyields
--

DROP TABLE IF EXISTS `raceedibleforagableyields`;
CREATE TABLE IF NOT EXISTS `raceedibleforagableyields` (
  `RaceId` bigint NOT NULL,
  `YieldType` varchar(50) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `BiteYield` double NOT NULL,
  `HungerPerYield` double NOT NULL,
  `WaterPerYield` double NOT NULL,
  `ThirstPerYield` double NOT NULL,
  `AlcoholPerYield` double NOT NULL,
  `EatEmote` varchar(1000) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL DEFAULT '@ eat|eats {0} from the location.',
  PRIMARY KEY (`RaceId`,`YieldType`),
  CONSTRAINT `FK_RaceEdibleForagableYields_Races` FOREIGN KEY (`RaceId`) REFERENCES `races` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Dumping data for table raceedibleforagableyields
--

/*!40000 ALTER TABLE `raceedibleforagableyields` DISABLE KEYS */;
/*!40000 ALTER TABLE `raceedibleforagableyields` ENABLE KEYS */;

--
-- Definition of races_additionalbodyparts
--

DROP TABLE IF EXISTS `races_additionalbodyparts`;
CREATE TABLE IF NOT EXISTS `races_additionalbodyparts` (
  `Usage` varchar(50) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `BodypartId` bigint NOT NULL,
  `RaceId` bigint NOT NULL,
  PRIMARY KEY (`Usage`,`RaceId`,`BodypartId`),
  KEY `FK_Races_AdditionalBodyparts_BodypartProto` (`BodypartId`),
  KEY `FK_Races_AdditionalBodyparts_Races` (`RaceId`),
  CONSTRAINT `FK_Races_AdditionalBodyparts_BodypartProto` FOREIGN KEY (`BodypartId`) REFERENCES `bodypartproto` (`Id`) ON DELETE RESTRICT,
  CONSTRAINT `FK_Races_AdditionalBodyparts_Races` FOREIGN KEY (`RaceId`) REFERENCES `races` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Dumping data for table races_additionalbodyparts
--

/*!40000 ALTER TABLE `races_additionalbodyparts` DISABLE KEYS */;
/*!40000 ALTER TABLE `races_additionalbodyparts` ENABLE KEYS */;

--
-- Definition of races_additionalcharacteristics
--

DROP TABLE IF EXISTS `races_additionalcharacteristics`;
CREATE TABLE IF NOT EXISTS `races_additionalcharacteristics` (
  `RaceId` bigint NOT NULL,
  `CharacteristicDefinitionId` bigint NOT NULL,
  `Usage` varchar(255) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  PRIMARY KEY (`RaceId`,`CharacteristicDefinitionId`),
  KEY `FK_RAC_CharacteristicDefinitions` (`CharacteristicDefinitionId`),
  CONSTRAINT `FK_RAC_CharacteristicDefinitions` FOREIGN KEY (`CharacteristicDefinitionId`) REFERENCES `characteristicdefinitions` (`Id`) ON DELETE RESTRICT,
  CONSTRAINT `FK_RAC_Races` FOREIGN KEY (`RaceId`) REFERENCES `races` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Dumping data for table races_additionalcharacteristics
--

/*!40000 ALTER TABLE `races_additionalcharacteristics` DISABLE KEYS */;
/*!40000 ALTER TABLE `races_additionalcharacteristics` ENABLE KEYS */;

--
-- Definition of races_breathablegases
--

DROP TABLE IF EXISTS `races_breathablegases`;
CREATE TABLE IF NOT EXISTS `races_breathablegases` (
  `RaceId` bigint NOT NULL,
  `GasId` bigint NOT NULL,
  `Multiplier` double NOT NULL DEFAULT '1',
  PRIMARY KEY (`RaceId`,`GasId`),
  KEY `FK_Races-BreathableGases_Gases_idx` (`GasId`),
  CONSTRAINT `FK_Races_BreathableGases_Gases` FOREIGN KEY (`GasId`) REFERENCES `gases` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_Races_BreathableGases_Races` FOREIGN KEY (`RaceId`) REFERENCES `races` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Dumping data for table races_breathablegases
--

/*!40000 ALTER TABLE `races_breathablegases` DISABLE KEYS */;
/*!40000 ALTER TABLE `races_breathablegases` ENABLE KEYS */;

--
-- Definition of races_breathableliquids
--

DROP TABLE IF EXISTS `races_breathableliquids`;
CREATE TABLE IF NOT EXISTS `races_breathableliquids` (
  `RaceId` bigint NOT NULL,
  `LiquidId` bigint NOT NULL,
  `Multiplier` double NOT NULL DEFAULT '1',
  PRIMARY KEY (`RaceId`,`LiquidId`),
  KEY `FK_Races_BreathableLiquids_Liquids_idx` (`LiquidId`),
  CONSTRAINT `FK_Races_BreathableLiquids_Liquids` FOREIGN KEY (`LiquidId`) REFERENCES `liquids` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_Races_BreathableLiquids_Races` FOREIGN KEY (`RaceId`) REFERENCES `races` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Dumping data for table races_breathableliquids
--

/*!40000 ALTER TABLE `races_breathableliquids` DISABLE KEYS */;
/*!40000 ALTER TABLE `races_breathableliquids` ENABLE KEYS */;

--
-- Definition of races_chargenresources
--

DROP TABLE IF EXISTS `races_chargenresources`;
CREATE TABLE IF NOT EXISTS `races_chargenresources` (
  `RaceId` bigint NOT NULL,
  `ChargenResourceId` bigint NOT NULL,
  `RequirementOnly` bit(1) NOT NULL DEFAULT b'0',
  `Amount` int NOT NULL,
  PRIMARY KEY (`RaceId`,`ChargenResourceId`),
  KEY `FK_Races_ChargenResources_ChargenResources` (`ChargenResourceId`),
  CONSTRAINT `FK_Races_ChargenResources_ChargenResources_ChargenResourceId` FOREIGN KEY (`ChargenResourceId`) REFERENCES `chargenresources` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_Races_ChargenResources_Races_RaceId` FOREIGN KEY (`RaceId`) REFERENCES `races` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Dumping data for table races_chargenresources
--

/*!40000 ALTER TABLE `races_chargenresources` DISABLE KEYS */;
/*!40000 ALTER TABLE `races_chargenresources` ENABLE KEYS */;

--
-- Definition of races_combatactions
--

DROP TABLE IF EXISTS `races_combatactions`;
CREATE TABLE IF NOT EXISTS `races_combatactions` (
  `RaceId` bigint NOT NULL,
  `CombatActionId` bigint NOT NULL,
  PRIMARY KEY (`RaceId`,`CombatActionId`),
  KEY `IX_Races_CombatActions_CombatActionId` (`CombatActionId`),
  CONSTRAINT `FK_Races_CombatActions_CombatActions` FOREIGN KEY (`CombatActionId`) REFERENCES `combatactions` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_Races_CombatActions_Races` FOREIGN KEY (`RaceId`) REFERENCES `races` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Dumping data for table races_combatactions
--

/*!40000 ALTER TABLE `races_combatactions` DISABLE KEYS */;
/*!40000 ALTER TABLE `races_combatactions` ENABLE KEYS */;

--
-- Definition of races_ediblematerials
--

DROP TABLE IF EXISTS `races_ediblematerials`;
CREATE TABLE IF NOT EXISTS `races_ediblematerials` (
  `RaceId` bigint NOT NULL,
  `MaterialId` bigint NOT NULL,
  `HungerPerKilogram` double NOT NULL,
  `ThirstPerKilogram` double NOT NULL,
  `WaterPerKilogram` double NOT NULL,
  `AlcoholPerKilogram` double NOT NULL,
  PRIMARY KEY (`RaceId`,`MaterialId`),
  KEY `FK_Races_EdibleMaterials_Materials_idx` (`MaterialId`),
  CONSTRAINT `FK_Races_EdibleMaterials_Materials` FOREIGN KEY (`MaterialId`) REFERENCES `materials` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_Races_EdibleMaterials_Races` FOREIGN KEY (`RaceId`) REFERENCES `races` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Dumping data for table races_ediblematerials
--

/*!40000 ALTER TABLE `races_ediblematerials` DISABLE KEYS */;
/*!40000 ALTER TABLE `races_ediblematerials` ENABLE KEYS */;

--
-- Definition of races_removebreathablegases
--

DROP TABLE IF EXISTS `races_removebreathablegases`;
CREATE TABLE IF NOT EXISTS `races_removebreathablegases` (
  `RaceId` bigint NOT NULL,
  `GasId` bigint NOT NULL,
  PRIMARY KEY (`RaceId`,`GasId`),
  KEY `FK_Races_RemoveBreathableGases_Gases_idx` (`GasId`),
  CONSTRAINT `FK_Races_RemoveBreathableGases_Gases` FOREIGN KEY (`GasId`) REFERENCES `gases` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_Races_RemoveBreathableGases_Races` FOREIGN KEY (`RaceId`) REFERENCES `races` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Dumping data for table races_removebreathablegases
--

/*!40000 ALTER TABLE `races_removebreathablegases` DISABLE KEYS */;
/*!40000 ALTER TABLE `races_removebreathablegases` ENABLE KEYS */;

--
-- Definition of races_removebreathableliquids
--

DROP TABLE IF EXISTS `races_removebreathableliquids`;
CREATE TABLE IF NOT EXISTS `races_removebreathableliquids` (
  `RaceId` bigint NOT NULL,
  `LiquidId` bigint NOT NULL,
  PRIMARY KEY (`RaceId`,`LiquidId`),
  KEY `FK_Races_RemoveBreathableLiquids_Liquids_idx` (`LiquidId`),
  CONSTRAINT `FK_Races_RemoveBreathableLiquids_Liquids` FOREIGN KEY (`LiquidId`) REFERENCES `liquids` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_Races_RemoveBreathableLiquids_Races` FOREIGN KEY (`RaceId`) REFERENCES `races` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Dumping data for table races_removebreathableliquids
--

/*!40000 ALTER TABLE `races_removebreathableliquids` DISABLE KEYS */;
/*!40000 ALTER TABLE `races_removebreathableliquids` ENABLE KEYS */;

--
-- Definition of randomnameprofiles
--

DROP TABLE IF EXISTS `randomnameprofiles`;
CREATE TABLE IF NOT EXISTS `randomnameprofiles` (
  `Id` bigint NOT NULL AUTO_INCREMENT,
  `Gender` int NOT NULL,
  `NameCultureId` bigint NOT NULL,
  `Name` varchar(255) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `UseForChargenSuggestionsProgId` bigint DEFAULT NULL,
  PRIMARY KEY (`Id`),
  KEY `FK_RandomNameProfiles_NameCulture` (`NameCultureId`),
  KEY `IX_RandomNameProfiles_UseForChargenSuggestionsProgId` (`UseForChargenSuggestionsProgId`),
  CONSTRAINT `FK_RandomNameProfiles_FutureProgs_UseForChargenSuggestionsProgId` FOREIGN KEY (`UseForChargenSuggestionsProgId`) REFERENCES `futureprogs` (`Id`),
  CONSTRAINT `FK_RandomNameProfiles_NameCulture` FOREIGN KEY (`NameCultureId`) REFERENCES `nameculture` (`Id`) ON DELETE RESTRICT
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Dumping data for table randomnameprofiles
--

/*!40000 ALTER TABLE `randomnameprofiles` DISABLE KEYS */;
/*!40000 ALTER TABLE `randomnameprofiles` ENABLE KEYS */;

--
-- Definition of randomnameprofiles_diceexpressions
--

DROP TABLE IF EXISTS `randomnameprofiles_diceexpressions`;
CREATE TABLE IF NOT EXISTS `randomnameprofiles_diceexpressions` (
  `RandomNameProfileId` bigint NOT NULL,
  `NameUsage` int NOT NULL,
  `DiceExpression` varchar(255) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  PRIMARY KEY (`RandomNameProfileId`,`NameUsage`),
  CONSTRAINT `FK_RandomNameProfiles_DiceExpressions_RandomNameProfiles` FOREIGN KEY (`RandomNameProfileId`) REFERENCES `randomnameprofiles` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Dumping data for table randomnameprofiles_diceexpressions
--

/*!40000 ALTER TABLE `randomnameprofiles_diceexpressions` DISABLE KEYS */;
/*!40000 ALTER TABLE `randomnameprofiles_diceexpressions` ENABLE KEYS */;

--
-- Definition of randomnameprofiles_elements
--

DROP TABLE IF EXISTS `randomnameprofiles_elements`;
CREATE TABLE IF NOT EXISTS `randomnameprofiles_elements` (
  `RandomNameProfileId` bigint NOT NULL,
  `NameUsage` int NOT NULL,
  `Name` varchar(100) CHARACTER SET utf8mb3 COLLATE utf8mb3_bin NOT NULL,
  `Weighting` int NOT NULL,
  PRIMARY KEY (`RandomNameProfileId`,`NameUsage`,`Name`),
  CONSTRAINT `FK_RandomNameProfiles_Elements_RandomNameProfiles` FOREIGN KEY (`RandomNameProfileId`) REFERENCES `randomnameprofiles` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Dumping data for table randomnameprofiles_elements
--

/*!40000 ALTER TABLE `randomnameprofiles_elements` DISABLE KEYS */;
/*!40000 ALTER TABLE `randomnameprofiles_elements` ENABLE KEYS */;

--
-- Definition of rangedcovers
--

DROP TABLE IF EXISTS `rangedcovers`;
CREATE TABLE IF NOT EXISTS `rangedcovers` (
  `Id` bigint NOT NULL AUTO_INCREMENT,
  `Name` varchar(200) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL,
  `CoverType` int NOT NULL,
  `CoverExtent` int NOT NULL,
  `HighestPositionState` int NOT NULL,
  `DescriptionString` text CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL,
  `ActionDescriptionString` text CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL,
  `MaximumSimultaneousCovers` int NOT NULL,
  `CoverStaysWhileMoving` bit(1) NOT NULL,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Dumping data for table rangedcovers
--

/*!40000 ALTER TABLE `rangedcovers` DISABLE KEYS */;
/*!40000 ALTER TABLE `rangedcovers` ENABLE KEYS */;

--
-- Definition of cells_rangedcovers
--

DROP TABLE IF EXISTS `cells_rangedcovers`;
CREATE TABLE IF NOT EXISTS `cells_rangedcovers` (
  `CellId` bigint NOT NULL,
  `RangedCoverId` bigint NOT NULL,
  PRIMARY KEY (`CellId`,`RangedCoverId`),
  KEY `FK_Cells_RangedCovers_RangedCovers_idx` (`RangedCoverId`),
  CONSTRAINT `FK_Cells_RangedCovers_Cells` FOREIGN KEY (`CellId`) REFERENCES `cells` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_Cells_RangedCovers_RangedCovers` FOREIGN KEY (`RangedCoverId`) REFERENCES `rangedcovers` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Dumping data for table cells_rangedcovers
--

/*!40000 ALTER TABLE `cells_rangedcovers` DISABLE KEYS */;
/*!40000 ALTER TABLE `cells_rangedcovers` ENABLE KEYS */;

--
-- Definition of ranks
--

DROP TABLE IF EXISTS `ranks`;
CREATE TABLE IF NOT EXISTS `ranks` (
  `Id` bigint NOT NULL AUTO_INCREMENT,
  `Name` varchar(100) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `InsigniaGameItemId` bigint DEFAULT NULL,
  `InsigniaGameItemRevnum` int DEFAULT NULL,
  `ClanId` bigint NOT NULL,
  `Privileges` bigint NOT NULL,
  `RankPath` varchar(50) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci DEFAULT NULL,
  `RankNumber` int NOT NULL,
  `FameType` int NOT NULL DEFAULT '0',
  PRIMARY KEY (`Id`),
  KEY `FK_Ranks_Clans` (`ClanId`),
  KEY `FK_Ranks_GameItemProtos` (`InsigniaGameItemId`,`InsigniaGameItemRevnum`),
  CONSTRAINT `FK_Ranks_Clans` FOREIGN KEY (`ClanId`) REFERENCES `clans` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_Ranks_GameItemProtos` FOREIGN KEY (`InsigniaGameItemId`, `InsigniaGameItemRevnum`) REFERENCES `gameitemprotos` (`Id`, `RevisionNumber`) ON DELETE RESTRICT
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Dumping data for table ranks
--

/*!40000 ALTER TABLE `ranks` DISABLE KEYS */;
/*!40000 ALTER TABLE `ranks` ENABLE KEYS */;

--
-- Definition of clanpayrollhistories
--

DROP TABLE IF EXISTS `clanpayrollhistories`;
CREATE TABLE IF NOT EXISTS `clanpayrollhistories` (
  `Id` bigint NOT NULL AUTO_INCREMENT,
  `ClanId` bigint NOT NULL,
  `CharacterId` bigint NOT NULL,
  `RankId` bigint NOT NULL,
  `PaygradeId` bigint DEFAULT NULL,
  `AppointmentId` bigint DEFAULT NULL,
  `ActorId` bigint DEFAULT NULL,
  `CurrencyId` bigint NOT NULL,
  `Amount` decimal(58,29) NOT NULL,
  `EntryType` int NOT NULL,
  `DateTime` varchar(255) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `Description` varchar(1000) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  PRIMARY KEY (`Id`),
  KEY `FK_ClanPayrollHistories_Actors_idx` (`ActorId`),
  KEY `FK_ClanPayrollHistories_Appointments_idx` (`AppointmentId`),
  KEY `FK_ClanPayrollHistories_Characters_idx` (`CharacterId`),
  KEY `FK_ClanPayrollHistories_Clans_idx` (`ClanId`),
  KEY `FK_ClanPayrollHistories_Currencies_idx` (`CurrencyId`),
  KEY `FK_ClanPayrollHistories_Paygrades_idx` (`PaygradeId`),
  KEY `FK_ClanPayrollHistories_Ranks_idx` (`RankId`),
  CONSTRAINT `FK_ClanPayrollHistories_Actors` FOREIGN KEY (`ActorId`) REFERENCES `characters` (`Id`) ON DELETE SET NULL,
  CONSTRAINT `FK_ClanPayrollHistories_Appointments` FOREIGN KEY (`AppointmentId`) REFERENCES `appointments` (`Id`) ON DELETE SET NULL,
  CONSTRAINT `FK_ClanPayrollHistories_Characters` FOREIGN KEY (`CharacterId`) REFERENCES `characters` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_ClanPayrollHistories_Clans` FOREIGN KEY (`ClanId`) REFERENCES `clans` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_ClanPayrollHistories_Currencies` FOREIGN KEY (`CurrencyId`) REFERENCES `currencies` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_ClanPayrollHistories_Paygrades` FOREIGN KEY (`PaygradeId`) REFERENCES `paygrades` (`Id`) ON DELETE SET NULL,
  CONSTRAINT `FK_ClanPayrollHistories_Ranks` FOREIGN KEY (`RankId`) REFERENCES `ranks` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Dumping data for table clanpayrollhistories
--

/*!40000 ALTER TABLE `clanpayrollhistories` DISABLE KEYS */;
/*!40000 ALTER TABLE `clanpayrollhistories` ENABLE KEYS */;

--
-- Definition of joblistings
--

DROP TABLE IF EXISTS `joblistings`;
CREATE TABLE IF NOT EXISTS `joblistings` (
  `Id` bigint NOT NULL AUTO_INCREMENT,
  `PosterId` bigint NOT NULL,
  `IsReadyToBePosted` bit(1) NOT NULL,
  `IsArchived` bit(1) NOT NULL,
  `Name` varchar(200) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `PosterType` varchar(100) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `JobListingType` varchar(100) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `Definition` text CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci,
  `Description` text CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci,
  `MoneyPaidIn` text CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `MaximumDuration` varchar(200) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci DEFAULT NULL,
  `EligibilityProgId` bigint NOT NULL,
  `ClanId` bigint DEFAULT NULL,
  `RankId` bigint DEFAULT NULL,
  `PaygradeId` bigint DEFAULT NULL,
  `AppointmentId` bigint DEFAULT NULL,
  `PersonalProjectId` bigint DEFAULT NULL,
  `PersonalProjectRevisionNumber` int DEFAULT NULL,
  `RequiredProjectId` bigint DEFAULT NULL,
  `RequiredProjectLabourId` bigint DEFAULT NULL,
  `BankAccountId` bigint DEFAULT NULL,
  `EconomicZoneId` bigint NOT NULL,
  `MaximumNumberOfSimultaneousEmployees` int NOT NULL,
  `FullTimeEquivalentRatio` double NOT NULL,
  PRIMARY KEY (`Id`),
  KEY `IX_JobListings_AppointmentId` (`AppointmentId`),
  KEY `IX_JobListings_BankAccountId` (`BankAccountId`),
  KEY `IX_JobListings_ClanId` (`ClanId`),
  KEY `IX_JobListings_EconomicZoneId` (`EconomicZoneId`),
  KEY `IX_JobListings_EligibilityProgId` (`EligibilityProgId`),
  KEY `IX_JobListings_PaygradeId` (`PaygradeId`),
  KEY `IX_JobListings_PersonalProjectId_PersonalProjectRevisionNumber` (`PersonalProjectId`,`PersonalProjectRevisionNumber`),
  KEY `IX_JobListings_RankId` (`RankId`),
  KEY `IX_JobListings_RequiredProjectId_RequiredProjectLabourId` (`RequiredProjectId`,`RequiredProjectLabourId`),
  CONSTRAINT `FK_JobListings_ActiveProjectLabours` FOREIGN KEY (`RequiredProjectId`, `RequiredProjectLabourId`) REFERENCES `activeprojectlabours` (`ActiveProjectId`, `ProjectLabourRequirementsId`),
  CONSTRAINT `FK_JobListings_ActiveProjects` FOREIGN KEY (`RequiredProjectId`) REFERENCES `activeprojects` (`Id`),
  CONSTRAINT `FK_JobListings_Appointments` FOREIGN KEY (`AppointmentId`) REFERENCES `appointments` (`Id`),
  CONSTRAINT `FK_JobListings_BankAccounts` FOREIGN KEY (`BankAccountId`) REFERENCES `bankaccounts` (`Id`),
  CONSTRAINT `FK_JobListings_Clans` FOREIGN KEY (`ClanId`) REFERENCES `clans` (`Id`),
  CONSTRAINT `FK_JobListings_EconomicZones` FOREIGN KEY (`EconomicZoneId`) REFERENCES `economiczones` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_JobListings_FutureProgs` FOREIGN KEY (`EligibilityProgId`) REFERENCES `futureprogs` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_JobListings_Paygrades` FOREIGN KEY (`PaygradeId`) REFERENCES `paygrades` (`Id`),
  CONSTRAINT `FK_JobListings_Projects` FOREIGN KEY (`PersonalProjectId`, `PersonalProjectRevisionNumber`) REFERENCES `projects` (`Id`, `RevisionNumber`),
  CONSTRAINT `FK_JobListings_Ranks` FOREIGN KEY (`RankId`) REFERENCES `ranks` (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Dumping data for table joblistings
--

/*!40000 ALTER TABLE `joblistings` DISABLE KEYS */;
/*!40000 ALTER TABLE `joblistings` ENABLE KEYS */;

--
-- Definition of activejobs
--

DROP TABLE IF EXISTS `activejobs`;
CREATE TABLE IF NOT EXISTS `activejobs` (
  `Id` bigint NOT NULL AUTO_INCREMENT,
  `JobListingId` bigint NOT NULL,
  `CharacterId` bigint NOT NULL,
  `JobCommenced` varchar(200) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `JobDueToEnd` varchar(200) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci DEFAULT NULL,
  `JobEnded` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  `IsJobComplete` bit(1) NOT NULL,
  `AlreadyHadClanPosition` bit(1) NOT NULL,
  `BackpayOwed` text CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `RevenueEarned` text CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `CurrentPerformance` double NOT NULL,
  `ActiveProjectId` bigint DEFAULT NULL,
  PRIMARY KEY (`Id`),
  KEY `IX_ActiveJobs_ActiveProjectId` (`ActiveProjectId`),
  KEY `IX_ActiveJobs_CharacterId` (`CharacterId`),
  KEY `IX_ActiveJobs_JobListingId` (`JobListingId`),
  CONSTRAINT `FK_ActiveJobs_ActiveProjects` FOREIGN KEY (`ActiveProjectId`) REFERENCES `activeprojects` (`Id`),
  CONSTRAINT `FK_ActiveJobs_Characters` FOREIGN KEY (`CharacterId`) REFERENCES `characters` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_ActiveJobs_JobListings` FOREIGN KEY (`JobListingId`) REFERENCES `joblistings` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Dumping data for table activejobs
--

/*!40000 ALTER TABLE `activejobs` DISABLE KEYS */;
/*!40000 ALTER TABLE `activejobs` ENABLE KEYS */;

--
-- Definition of ranks_abbreviations
--

DROP TABLE IF EXISTS `ranks_abbreviations`;
CREATE TABLE IF NOT EXISTS `ranks_abbreviations` (
  `RankId` bigint NOT NULL,
  `Abbreviation` varchar(50) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `FutureProgId` bigint DEFAULT NULL,
  `Order` int NOT NULL,
  PRIMARY KEY (`RankId`,`Abbreviation`),
  KEY `FK_Ranks_Abbreviations_FutureProgs` (`FutureProgId`),
  CONSTRAINT `FK_Ranks_Abbreviations_FutureProgs` FOREIGN KEY (`FutureProgId`) REFERENCES `futureprogs` (`Id`) ON DELETE SET NULL,
  CONSTRAINT `FK_Ranks_Abbreviations_Ranks` FOREIGN KEY (`RankId`) REFERENCES `ranks` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Dumping data for table ranks_abbreviations
--

/*!40000 ALTER TABLE `ranks_abbreviations` DISABLE KEYS */;
/*!40000 ALTER TABLE `ranks_abbreviations` ENABLE KEYS */;

--
-- Definition of ranks_paygrades
--

DROP TABLE IF EXISTS `ranks_paygrades`;
CREATE TABLE IF NOT EXISTS `ranks_paygrades` (
  `RankId` bigint NOT NULL,
  `PaygradeId` bigint NOT NULL,
  `Order` int NOT NULL,
  PRIMARY KEY (`RankId`,`PaygradeId`),
  KEY `FK_Ranks_Paygrades_Paygrades` (`PaygradeId`),
  CONSTRAINT `FK_Ranks_Paygrades_Paygrades` FOREIGN KEY (`PaygradeId`) REFERENCES `paygrades` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_Ranks_Paygrades_Ranks` FOREIGN KEY (`RankId`) REFERENCES `ranks` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Dumping data for table ranks_paygrades
--

/*!40000 ALTER TABLE `ranks_paygrades` DISABLE KEYS */;
/*!40000 ALTER TABLE `ranks_paygrades` ENABLE KEYS */;

--
-- Definition of ranks_titles
--

DROP TABLE IF EXISTS `ranks_titles`;
CREATE TABLE IF NOT EXISTS `ranks_titles` (
  `RankId` bigint NOT NULL,
  `Title` varchar(50) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `FutureProgId` bigint DEFAULT NULL,
  `Order` int NOT NULL,
  PRIMARY KEY (`RankId`,`Title`),
  KEY `FK_Ranks_Titles_FutureProgs` (`FutureProgId`),
  CONSTRAINT `FK_Ranks_Titles_FutureProgs` FOREIGN KEY (`FutureProgId`) REFERENCES `futureprogs` (`Id`) ON DELETE SET NULL,
  CONSTRAINT `FK_Ranks_Titles_Ranks` FOREIGN KEY (`RankId`) REFERENCES `ranks` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Dumping data for table ranks_titles
--

/*!40000 ALTER TABLE `ranks_titles` DISABLE KEYS */;
/*!40000 ALTER TABLE `ranks_titles` ENABLE KEYS */;

--
-- Definition of regionalclimates
--

DROP TABLE IF EXISTS `regionalclimates`;
CREATE TABLE IF NOT EXISTS `regionalclimates` (
  `Id` bigint NOT NULL AUTO_INCREMENT,
  `Name` varchar(200) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `ClimateModelId` bigint NOT NULL,
  `Description` text CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci,
  `TemperatureFluctuationPeriodMinutes` int NOT NULL DEFAULT '0',
  `TemperatureFluctuationStandardDeviation` double NOT NULL DEFAULT '0',
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Dumping data for table regionalclimates
--

/*!40000 ALTER TABLE `regionalclimates` DISABLE KEYS */;
/*!40000 ALTER TABLE `regionalclimates` ENABLE KEYS */;

--
-- Definition of scriptedevents
--

DROP TABLE IF EXISTS `scriptedevents`;
CREATE TABLE IF NOT EXISTS `scriptedevents` (
  `Id` bigint NOT NULL AUTO_INCREMENT,
  `Name` mediumtext CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci,
  `CharacterId` bigint DEFAULT NULL,
  `CharacterFilterProgId` bigint DEFAULT NULL,
  `IsReady` bit(1) NOT NULL,
  `EarliestDate` datetime NOT NULL,
  `IsFinished` bit(1) NOT NULL,
  `IsTemplate` bit(1) NOT NULL,
  PRIMARY KEY (`Id`),
  KEY `IX_ScriptedEvents_CharacterFilterProgId` (`CharacterFilterProgId`),
  KEY `IX_ScriptedEvents_CharacterId` (`CharacterId`),
  CONSTRAINT `FK_ScriptedEvents_Characters` FOREIGN KEY (`CharacterId`) REFERENCES `characters` (`Id`),
  CONSTRAINT `FK_ScriptedEvents_FutureProgs` FOREIGN KEY (`CharacterFilterProgId`) REFERENCES `futureprogs` (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Dumping data for table scriptedevents
--

/*!40000 ALTER TABLE `scriptedevents` DISABLE KEYS */;
/*!40000 ALTER TABLE `scriptedevents` ENABLE KEYS */;

--
-- Definition of scriptedeventfreetextquestions
--

DROP TABLE IF EXISTS `scriptedeventfreetextquestions`;
CREATE TABLE IF NOT EXISTS `scriptedeventfreetextquestions` (
  `Id` bigint NOT NULL AUTO_INCREMENT,
  `ScriptedEventId` bigint NOT NULL,
  `Question` mediumtext CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci,
  `Answer` mediumtext CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci,
  PRIMARY KEY (`Id`),
  KEY `IX_ScriptedEventFreeTextQuestions_ScriptedEventId` (`ScriptedEventId`),
  CONSTRAINT `FK_ScriptedEventFreeTextQuestions_ScriptedEvents` FOREIGN KEY (`ScriptedEventId`) REFERENCES `scriptedevents` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Dumping data for table scriptedeventfreetextquestions
--

/*!40000 ALTER TABLE `scriptedeventfreetextquestions` DISABLE KEYS */;
/*!40000 ALTER TABLE `scriptedeventfreetextquestions` ENABLE KEYS */;

--
-- Definition of scriptedeventmultiplechoicequestions
--

DROP TABLE IF EXISTS `scriptedeventmultiplechoicequestions`;
CREATE TABLE IF NOT EXISTS `scriptedeventmultiplechoicequestions` (
  `Id` bigint NOT NULL AUTO_INCREMENT,
  `ScriptedEventId` bigint NOT NULL,
  `Question` mediumtext CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci,
  `ChosenAnswerId` bigint DEFAULT NULL,
  PRIMARY KEY (`Id`),
  KEY `IX_ScriptedEventMultipleChoiceQuestions_ChosenAnswerId` (`ChosenAnswerId`),
  KEY `IX_ScriptedEventMultipleChoiceQuestions_ScriptedEventId` (`ScriptedEventId`),
  CONSTRAINT `FK_ScriptedEventMultipleChoiceQuestions_ScriptedEventMultipleCho` FOREIGN KEY (`ChosenAnswerId`) REFERENCES `scriptedeventmultiplechoicequestionanswers` (`Id`),
  CONSTRAINT `FK_ScriptedEventMultipleChoiceQuestions_ScriptedEvents` FOREIGN KEY (`ScriptedEventId`) REFERENCES `scriptedevents` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Dumping data for table scriptedeventmultiplechoicequestions
--

/*!40000 ALTER TABLE `scriptedeventmultiplechoicequestions` DISABLE KEYS */;
/*!40000 ALTER TABLE `scriptedeventmultiplechoicequestions` ENABLE KEYS */;

--
-- Definition of scriptedeventmultiplechoicequestionanswers
--

DROP TABLE IF EXISTS `scriptedeventmultiplechoicequestionanswers`;
CREATE TABLE IF NOT EXISTS `scriptedeventmultiplechoicequestionanswers` (
  `Id` bigint NOT NULL AUTO_INCREMENT,
  `ScriptedEventMultipleChoiceQuestionId` bigint NOT NULL,
  `DescriptionBeforeChoice` mediumtext CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci,
  `DescriptionAfterChoice` mediumtext CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci,
  `AnswerFilterProgId` bigint DEFAULT NULL,
  `AfterChoiceProgId` bigint DEFAULT NULL,
  PRIMARY KEY (`Id`),
  KEY `IX_ScriptedEventMultipleChoiceQuestionAnswers_AfterChoiceProgId` (`AfterChoiceProgId`),
  KEY `IX_ScriptedEventMultipleChoiceQuestionAnswers_AnswerFilterProgId` (`AnswerFilterProgId`),
  KEY `IX_ScriptedEventMultipleChoiceQuestionAnswers_ScriptedEventMult~` (`ScriptedEventMultipleChoiceQuestionId`),
  CONSTRAINT `FK_ScriptedEventMultipleChoiceQuestionAnswers_FutureProgs_After` FOREIGN KEY (`AfterChoiceProgId`) REFERENCES `futureprogs` (`Id`),
  CONSTRAINT `FK_ScriptedEventMultipleChoiceQuestionAnswers_FutureProgs_Filter` FOREIGN KEY (`AnswerFilterProgId`) REFERENCES `futureprogs` (`Id`),
  CONSTRAINT `FK_ScriptedEventMultipleChoiceQuestionAnswers_ScriptedEventMulti` FOREIGN KEY (`ScriptedEventMultipleChoiceQuestionId`) REFERENCES `scriptedeventmultiplechoicequestions` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Dumping data for table scriptedeventmultiplechoicequestionanswers
--

/*!40000 ALTER TABLE `scriptedeventmultiplechoicequestionanswers` DISABLE KEYS */;
/*!40000 ALTER TABLE `scriptedeventmultiplechoicequestionanswers` ENABLE KEYS */;

--
-- Definition of scripts
--

DROP TABLE IF EXISTS `scripts`;
CREATE TABLE IF NOT EXISTS `scripts` (
  `Id` bigint NOT NULL AUTO_INCREMENT,
  `Name` text CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `KnownScriptDescription` text CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `UnknownScriptDescription` text CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `KnowledgeId` bigint NOT NULL,
  `DocumentLengthModifier` double NOT NULL DEFAULT '1',
  `InkUseModifier` double NOT NULL DEFAULT '1',
  PRIMARY KEY (`Id`),
  KEY `FK_Scripts_Knowledges_idx` (`KnowledgeId`),
  CONSTRAINT `FK_Scripts_Knowledges` FOREIGN KEY (`KnowledgeId`) REFERENCES `knowledges` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Dumping data for table scripts
--

/*!40000 ALTER TABLE `scripts` DISABLE KEYS */;
/*!40000 ALTER TABLE `scripts` ENABLE KEYS */;

--
-- Definition of characters
--

DROP TABLE IF EXISTS `characters`;
CREATE TABLE IF NOT EXISTS `characters` (
  `Id` bigint NOT NULL AUTO_INCREMENT,
  `Name` varchar(4000) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `AccountId` bigint DEFAULT NULL,
  `CreationTime` datetime NOT NULL,
  `DeathTime` datetime DEFAULT NULL,
  `Status` int NOT NULL,
  `State` int NOT NULL,
  `Gender` smallint NOT NULL,
  `Location` bigint NOT NULL,
  `BodyId` bigint NOT NULL,
  `CultureId` bigint NOT NULL,
  `EffectData` mediumtext CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `BirthdayDate` varchar(4000) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `BirthdayCalendarId` bigint NOT NULL,
  `IsAdminAvatar` bit(1) NOT NULL DEFAULT b'0',
  `CurrencyId` bigint DEFAULT NULL,
  `TotalMinutesPlayed` int NOT NULL,
  `AlcoholLitres` double NOT NULL,
  `WaterLitres` double NOT NULL,
  `FoodSatiatedHours` double NOT NULL,
  `DrinkSatiatedHours` double NOT NULL,
  `NeedsModel` varchar(50) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL DEFAULT 'NoNeeds',
  `LongTermPlan` text CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci,
  `ShortTermPlan` text CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci,
  `ShownIntroductionMessage` bit(1) NOT NULL DEFAULT b'0',
  `IntroductionMessage` text CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci,
  `ChargenId` bigint DEFAULT NULL,
  `CurrentCombatSettingId` bigint DEFAULT NULL,
  `PreferredDefenseType` int NOT NULL,
  `PositionId` int NOT NULL DEFAULT '1',
  `PositionModifier` int NOT NULL,
  `PositionTargetId` bigint DEFAULT NULL,
  `PositionTargetType` varchar(45) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci DEFAULT NULL,
  `PositionEmote` text CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci,
  `CurrentLanguageId` bigint DEFAULT NULL,
  `CurrentAccentId` bigint DEFAULT NULL,
  `CurrentWritingLanguageId` bigint DEFAULT NULL,
  `WritingStyle` int NOT NULL DEFAULT '8256',
  `CurrentScriptId` bigint DEFAULT NULL,
  `DominantHandAlignment` int NOT NULL DEFAULT '3',
  `LastLoginTime` datetime DEFAULT NULL,
  `CombatBrief` bit(1) NOT NULL DEFAULT b'0',
  `RoomBrief` bit(1) NOT NULL DEFAULT b'0',
  `LastLogoutTime` datetime DEFAULT NULL,
  `Outfits` text CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci,
  `CurrentProjectLabourId` bigint DEFAULT NULL,
  `CurrentProjectId` bigint DEFAULT NULL,
  `CurrentProjectHours` double NOT NULL,
  `NameInfo` text CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci,
  `RoomLayer` int NOT NULL,
  `NoMercy` bit(1) NOT NULL DEFAULT b'0',
  `SatiationReserve` double NOT NULL DEFAULT '0',
  `EstateHeirId` bigint DEFAULT NULL,
  `EstateHeirType` varchar(100) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci DEFAULT NULL,
  `CurrentProjectProjectHours` double NOT NULL DEFAULT '0',
  `CustomAlertEmote` varchar(500) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci DEFAULT NULL,
  `CustomDistantAlertEmote` varchar(500) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci DEFAULT NULL,
  `RoutePosition` decimal(18,3) DEFAULT NULL,
  PRIMARY KEY (`Id`),
  KEY `FK_Characters_Accounts` (`AccountId`),
  KEY `FK_Characters_Bodies` (`BodyId`),
  KEY `FK_Characters_Chargens_idx` (`ChargenId`),
  KEY `FK_Characters_Cultures` (`CultureId`),
  KEY `FK_Characters_Currencies` (`CurrencyId`),
  KEY `FK_Characters_Accents_idx` (`CurrentAccentId`),
  KEY `FK_Characters_Languages_idx` (`CurrentLanguageId`),
  KEY `FK_Characters_ActiveProjects_idx` (`CurrentProjectId`),
  KEY `FK_Characters_ProjectLabourRequirements_idx` (`CurrentProjectLabourId`),
  KEY `FK_Characters_Scripts_idx` (`CurrentScriptId`),
  KEY `FK_Characters_Languages_Written_idx` (`CurrentWritingLanguageId`),
  KEY `FK_Characters_Cells` (`Location`),
  KEY `IX_Characters_Location_Layer_RoutePosition` (`Location`,`RoomLayer`,`RoutePosition`),
  CONSTRAINT `FK_Characters_Accents` FOREIGN KEY (`CurrentAccentId`) REFERENCES `accents` (`Id`) ON DELETE SET NULL,
  CONSTRAINT `FK_Characters_Accounts` FOREIGN KEY (`AccountId`) REFERENCES `accounts` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_Characters_ActiveProjects` FOREIGN KEY (`CurrentProjectId`) REFERENCES `activeprojects` (`Id`) ON DELETE SET NULL,
  CONSTRAINT `FK_Characters_Bodies` FOREIGN KEY (`BodyId`) REFERENCES `bodies` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_Characters_Cells` FOREIGN KEY (`Location`) REFERENCES `cells` (`Id`) ON DELETE RESTRICT,
  CONSTRAINT `FK_Characters_Chargens` FOREIGN KEY (`ChargenId`) REFERENCES `chargens` (`Id`) ON DELETE SET NULL,
  CONSTRAINT `FK_Characters_Cultures` FOREIGN KEY (`CultureId`) REFERENCES `cultures` (`Id`) ON DELETE RESTRICT,
  CONSTRAINT `FK_Characters_Currencies` FOREIGN KEY (`CurrencyId`) REFERENCES `currencies` (`Id`) ON DELETE RESTRICT,
  CONSTRAINT `FK_Characters_Languages_Spoken` FOREIGN KEY (`CurrentLanguageId`) REFERENCES `languages` (`Id`) ON DELETE SET NULL,
  CONSTRAINT `FK_Characters_Languages_Written` FOREIGN KEY (`CurrentWritingLanguageId`) REFERENCES `languages` (`Id`) ON DELETE SET NULL,
  CONSTRAINT `FK_Characters_ProjectLabourRequirements` FOREIGN KEY (`CurrentProjectLabourId`) REFERENCES `projectlabourrequirements` (`Id`) ON DELETE SET NULL,
  CONSTRAINT `FK_Characters_Scripts` FOREIGN KEY (`CurrentScriptId`) REFERENCES `scripts` (`Id`) ON DELETE SET NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Dumping data for table characters
--

/*!40000 ALTER TABLE `characters` DISABLE KEYS */;
/*!40000 ALTER TABLE `characters` ENABLE KEYS */;

--
-- Definition of accountnotes
--

DROP TABLE IF EXISTS `accountnotes`;
CREATE TABLE IF NOT EXISTS `accountnotes` (
  `Id` bigint NOT NULL AUTO_INCREMENT,
  `AccountId` bigint NOT NULL,
  `Text` text CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `Subject` varchar(4000) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `TimeStamp` datetime NOT NULL,
  `AuthorId` bigint DEFAULT NULL,
  `InGameTimeStamp` varchar(200) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci DEFAULT NULL,
  `IsJournalEntry` bit(1) NOT NULL DEFAULT b'0',
  `CharacterId` bigint DEFAULT NULL,
  PRIMARY KEY (`Id`),
  KEY `FK_AccountNotes_Accounts` (`AccountId`),
  KEY `FK_AccountNotes_Author` (`AuthorId`),
  KEY `FK_AccountNotes_Characters_idx` (`CharacterId`),
  CONSTRAINT `FK_AccountNotes_Accounts` FOREIGN KEY (`AccountId`) REFERENCES `accounts` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_AccountNotes_Author` FOREIGN KEY (`AuthorId`) REFERENCES `accounts` (`Id`) ON DELETE RESTRICT,
  CONSTRAINT `FK_AccountNotes_Characters` FOREIGN KEY (`CharacterId`) REFERENCES `characters` (`Id`) ON DELETE RESTRICT
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Dumping data for table accountnotes
--

/*!40000 ALTER TABLE `accountnotes` DISABLE KEYS */;
/*!40000 ALTER TABLE `accountnotes` ENABLE KEYS */;

--
-- Definition of aistorytellercharactermemories
--

DROP TABLE IF EXISTS `aistorytellercharactermemories`;
CREATE TABLE IF NOT EXISTS `aistorytellercharactermemories` (
  `Id` bigint NOT NULL AUTO_INCREMENT,
  `AIStorytellerId` bigint NOT NULL,
  `CharacterId` bigint NOT NULL,
  `MemoryTitle` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  `MemoryText` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  `CreatedOn` datetime(6) NOT NULL,
  PRIMARY KEY (`Id`),
  KEY `IX_AIStorytellerCharacterMemories_AIStorytellerId` (`AIStorytellerId`),
  KEY `IX_AIStorytellerCharacterMemories_CharacterId` (`CharacterId`),
  CONSTRAINT `FK_AIStorytellerCharacterMemories_AIStorytellers_AIStorytellerId` FOREIGN KEY (`AIStorytellerId`) REFERENCES `aistorytellers` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_AIStorytellerCharacterMemories_Characters_CharacterId` FOREIGN KEY (`CharacterId`) REFERENCES `characters` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Dumping data for table aistorytellercharactermemories
--

/*!40000 ALTER TABLE `aistorytellercharactermemories` DISABLE KEYS */;
/*!40000 ALTER TABLE `aistorytellercharactermemories` ENABLE KEYS */;

--
-- Definition of aistorytellersituations
--

DROP TABLE IF EXISTS `aistorytellersituations`;
CREATE TABLE IF NOT EXISTS `aistorytellersituations` (
  `Id` bigint NOT NULL AUTO_INCREMENT,
  `AIStorytellerId` bigint NOT NULL,
  `Name` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  `SituationText` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  `CreatedOn` datetime(6) NOT NULL,
  `IsResolved` tinyint(1) NOT NULL,
  `ScopeCharacterId` bigint DEFAULT NULL,
  `ScopeRoomId` bigint DEFAULT NULL,
  PRIMARY KEY (`Id`),
  KEY `IX_AIStorytellerSituations_AIStorytellerId` (`AIStorytellerId`),
  KEY `IX_AIStorytellerSituations_ScopeCharacterId` (`ScopeCharacterId`),
  KEY `IX_AIStorytellerSituations_ScopeRoomId` (`ScopeRoomId`),
  CONSTRAINT `FK_AIStorytellerSituations_AIStorytellers_AIStorytellerId` FOREIGN KEY (`AIStorytellerId`) REFERENCES `aistorytellers` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_AIStorytellerSituations_Cells_ScopeRoomId` FOREIGN KEY (`ScopeRoomId`) REFERENCES `cells` (`Id`),
  CONSTRAINT `FK_AIStorytellerSituations_Characters_ScopeCharacterId` FOREIGN KEY (`ScopeCharacterId`) REFERENCES `characters` (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Dumping data for table aistorytellersituations
--

/*!40000 ALTER TABLE `aistorytellersituations` DISABLE KEYS */;
/*!40000 ALTER TABLE `aistorytellersituations` ENABLE KEYS */;

--
-- Definition of allies
--

DROP TABLE IF EXISTS `allies`;
CREATE TABLE IF NOT EXISTS `allies` (
  `CharacterId` bigint NOT NULL,
  `AllyId` bigint NOT NULL,
  `Trusted` bit(1) NOT NULL DEFAULT b'0',
  PRIMARY KEY (`CharacterId`,`AllyId`),
  KEY `FK_Allies_Characters_Target_idx` (`AllyId`),
  CONSTRAINT `FK_Allies_Characters_Owner` FOREIGN KEY (`CharacterId`) REFERENCES `characters` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_Allies_Characters_Target` FOREIGN KEY (`AllyId`) REFERENCES `characters` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Dumping data for table allies
--

/*!40000 ALTER TABLE `allies` DISABLE KEYS */;
/*!40000 ALTER TABLE `allies` ENABLE KEYS */;

--
-- Definition of arenabetpayouts
--

DROP TABLE IF EXISTS `arenabetpayouts`;
CREATE TABLE IF NOT EXISTS `arenabetpayouts` (
  `Id` bigint NOT NULL AUTO_INCREMENT,
  `ArenaEventId` bigint NOT NULL,
  `CharacterId` bigint NOT NULL,
  `Amount` decimal(58,29) NOT NULL,
  `IsBlocked` bit(1) NOT NULL DEFAULT b'0',
  `CreatedAt` datetime NOT NULL,
  `CollectedAt` datetime DEFAULT NULL,
  `PayoutType` int NOT NULL DEFAULT '0',
  PRIMARY KEY (`Id`),
  KEY `FK_ArenaBetPayouts_ArenaEvents` (`ArenaEventId`),
  KEY `FK_ArenaBetPayouts_Characters` (`CharacterId`),
  CONSTRAINT `FK_ArenaBetPayouts_ArenaEvents` FOREIGN KEY (`ArenaEventId`) REFERENCES `arenaevents` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_ArenaBetPayouts_Characters` FOREIGN KEY (`CharacterId`) REFERENCES `characters` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Dumping data for table arenabetpayouts
--

/*!40000 ALTER TABLE `arenabetpayouts` DISABLE KEYS */;
/*!40000 ALTER TABLE `arenabetpayouts` ENABLE KEYS */;

--
-- Definition of arenabets
--

DROP TABLE IF EXISTS `arenabets`;
CREATE TABLE IF NOT EXISTS `arenabets` (
  `Id` bigint NOT NULL AUTO_INCREMENT,
  `ArenaEventId` bigint NOT NULL,
  `CharacterId` bigint NOT NULL,
  `SideIndex` int DEFAULT NULL,
  `Stake` decimal(58,29) NOT NULL,
  `FixedDecimalOdds` decimal(58,29) DEFAULT NULL,
  `ModelSnapshot` text CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci,
  `IsCancelled` bit(1) NOT NULL DEFAULT b'0',
  `PlacedAt` datetime NOT NULL,
  `CancelledAt` datetime DEFAULT NULL,
  PRIMARY KEY (`Id`),
  KEY `FK_ArenaBets_ArenaEvents` (`ArenaEventId`),
  KEY `FK_ArenaBets_Characters` (`CharacterId`),
  CONSTRAINT `FK_ArenaBets_ArenaEvents` FOREIGN KEY (`ArenaEventId`) REFERENCES `arenaevents` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_ArenaBets_Characters` FOREIGN KEY (`CharacterId`) REFERENCES `characters` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Dumping data for table arenabets
--

/*!40000 ALTER TABLE `arenabets` DISABLE KEYS */;
/*!40000 ALTER TABLE `arenabets` ENABLE KEYS */;

--
-- Definition of arenamanagers
--

DROP TABLE IF EXISTS `arenamanagers`;
CREATE TABLE IF NOT EXISTS `arenamanagers` (
  `Id` bigint NOT NULL AUTO_INCREMENT,
  `ArenaId` bigint NOT NULL,
  `CharacterId` bigint NOT NULL,
  `CreatedAt` datetime NOT NULL,
  PRIMARY KEY (`Id`),
  KEY `FK_ArenaManagers_Arenas` (`ArenaId`),
  KEY `FK_ArenaManagers_Characters` (`CharacterId`),
  CONSTRAINT `FK_ArenaManagers_Arenas` FOREIGN KEY (`ArenaId`) REFERENCES `arenas` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_ArenaManagers_Characters` FOREIGN KEY (`CharacterId`) REFERENCES `characters` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Dumping data for table arenamanagers
--

/*!40000 ALTER TABLE `arenamanagers` DISABLE KEYS */;
/*!40000 ALTER TABLE `arenamanagers` ENABLE KEYS */;

--
-- Definition of bankmanagerauditlogs
--

DROP TABLE IF EXISTS `bankmanagerauditlogs`;
CREATE TABLE IF NOT EXISTS `bankmanagerauditlogs` (
  `Id` bigint NOT NULL AUTO_INCREMENT,
  `BankId` bigint NOT NULL,
  `CharacterId` bigint NOT NULL,
  `DateTime` varchar(255) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `Detail` varchar(1000) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  PRIMARY KEY (`Id`),
  KEY `IX_BankManagerAuditLogs_BankId` (`BankId`),
  KEY `IX_BankManagerAuditLogs_CharacterId` (`CharacterId`),
  CONSTRAINT `FK_BankManagerAuditLogs_Banks` FOREIGN KEY (`BankId`) REFERENCES `banks` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_BankManagerAuditLogs_Characters` FOREIGN KEY (`CharacterId`) REFERENCES `characters` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Dumping data for table bankmanagerauditlogs
--

/*!40000 ALTER TABLE `bankmanagerauditlogs` DISABLE KEYS */;
/*!40000 ALTER TABLE `bankmanagerauditlogs` ENABLE KEYS */;

--
-- Definition of bankmanagers
--

DROP TABLE IF EXISTS `bankmanagers`;
CREATE TABLE IF NOT EXISTS `bankmanagers` (
  `BankId` bigint NOT NULL,
  `CharacterId` bigint NOT NULL,
  PRIMARY KEY (`BankId`,`CharacterId`),
  KEY `IX_BankManagers_CharacterId` (`CharacterId`),
  CONSTRAINT `FK_BankManagers_Banks` FOREIGN KEY (`BankId`) REFERENCES `banks` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_BankManagers_Characters` FOREIGN KEY (`CharacterId`) REFERENCES `characters` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Dumping data for table bankmanagers
--

/*!40000 ALTER TABLE `bankmanagers` DISABLE KEYS */;
/*!40000 ALTER TABLE `bankmanagers` ENABLE KEYS */;

--
-- Definition of characterbodysources
--

DROP TABLE IF EXISTS `characterbodysources`;
CREATE TABLE IF NOT EXISTS `characterbodysources` (
  `CharacterId` bigint NOT NULL,
  `SourceType` int NOT NULL,
  `SourceId` bigint NOT NULL,
  `SourceKey` varchar(255) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `BodyId` bigint NOT NULL,
  PRIMARY KEY (`CharacterId`,`SourceType`,`SourceId`,`SourceKey`),
  KEY `FK_CharacterBodySources_Bodies_idx` (`BodyId`),
  CONSTRAINT `FK_CharacterBodySources_Bodies` FOREIGN KEY (`BodyId`) REFERENCES `bodies` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_CharacterBodySources_Characters` FOREIGN KEY (`CharacterId`) REFERENCES `characters` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Dumping data for table characterbodysources
--

/*!40000 ALTER TABLE `characterbodysources` DISABLE KEYS */;
/*!40000 ALTER TABLE `characterbodysources` ENABLE KEYS */;

--
-- Definition of charactercomputerexecutables
--

DROP TABLE IF EXISTS `charactercomputerexecutables`;
CREATE TABLE IF NOT EXISTS `charactercomputerexecutables` (
  `Id` bigint NOT NULL AUTO_INCREMENT,
  `OwnerCharacterId` bigint NOT NULL,
  `OwnerHostItemId` bigint DEFAULT NULL,
  `OwnerStorageItemId` bigint DEFAULT NULL,
  `Name` varchar(255) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `ExecutableKind` int NOT NULL,
  `CompilationContext` int NOT NULL,
  `ReturnTypeDefinition` varchar(255) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `SourceCode` mediumtext CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `CompilationStatus` int NOT NULL,
  `CompileError` text CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `AutorunOnBoot` bit(1) NOT NULL,
  `CreatedAtUtc` datetime NOT NULL,
  `LastModifiedAtUtc` datetime NOT NULL,
  PRIMARY KEY (`Id`),
  KEY `FK_CharacterComputerExecutables_Characters_idx` (`OwnerCharacterId`),
  KEY `IX_CharacterComputerExecutables_Owner_Name` (`OwnerCharacterId`,`Name`),
  CONSTRAINT `FK_CharacterComputerExecutables_Characters` FOREIGN KEY (`OwnerCharacterId`) REFERENCES `characters` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Dumping data for table charactercomputerexecutables
--

/*!40000 ALTER TABLE `charactercomputerexecutables` DISABLE KEYS */;
/*!40000 ALTER TABLE `charactercomputerexecutables` ENABLE KEYS */;

--
-- Definition of charactercomputerexecutableparameters
--

DROP TABLE IF EXISTS `charactercomputerexecutableparameters`;
CREATE TABLE IF NOT EXISTS `charactercomputerexecutableparameters` (
  `Id` bigint NOT NULL AUTO_INCREMENT,
  `CharacterComputerExecutableId` bigint NOT NULL,
  `ParameterIndex` int NOT NULL,
  `ParameterName` varchar(255) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `ParameterTypeDefinition` varchar(255) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  PRIMARY KEY (`Id`),
  UNIQUE KEY `IX_CharacterComputerExecutableParameters_Executable_Index` (`CharacterComputerExecutableId`,`ParameterIndex`),
  KEY `FK_CharacterComputerExecutableParameters_Executables_idx` (`CharacterComputerExecutableId`),
  CONSTRAINT `FK_CharacterComputerExecutableParameters_CharacterComputerExecut` FOREIGN KEY (`CharacterComputerExecutableId`) REFERENCES `charactercomputerexecutables` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Dumping data for table charactercomputerexecutableparameters
--

/*!40000 ALTER TABLE `charactercomputerexecutableparameters` DISABLE KEYS */;
/*!40000 ALTER TABLE `charactercomputerexecutableparameters` ENABLE KEYS */;

--
-- Definition of charactercomputerprogramprocesses
--

DROP TABLE IF EXISTS `charactercomputerprogramprocesses`;
CREATE TABLE IF NOT EXISTS `charactercomputerprogramprocesses` (
  `Id` bigint NOT NULL AUTO_INCREMENT,
  `CharacterComputerExecutableId` bigint NOT NULL,
  `OwnerCharacterId` bigint NOT NULL,
  `ProcessName` varchar(255) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `Status` int NOT NULL,
  `WaitType` int NOT NULL,
  `WakeTimeUtc` datetime DEFAULT NULL,
  `WaitArgument` varchar(1000) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci DEFAULT NULL,
  `PowerLossBehaviour` int NOT NULL,
  `StateJson` longtext CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `ResultJson` longtext CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci,
  `LastError` text CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci,
  `StartedAtUtc` datetime NOT NULL,
  `LastUpdatedAtUtc` datetime NOT NULL,
  `EndedAtUtc` datetime DEFAULT NULL,
  PRIMARY KEY (`Id`),
  KEY `FK_CharacterComputerProgramProcesses_Characters_idx` (`OwnerCharacterId`),
  KEY `FK_CharacterComputerProgramProcesses_Executables_idx` (`CharacterComputerExecutableId`),
  KEY `IX_CharacterComputerProgramProcesses_Owner_Status_Wake` (`OwnerCharacterId`,`Status`,`WakeTimeUtc`),
  CONSTRAINT `FK_CharacterComputerProgramProcesses_CharacterComputerExecutable` FOREIGN KEY (`CharacterComputerExecutableId`) REFERENCES `charactercomputerexecutables` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_CharacterComputerProgramProcesses_Characters` FOREIGN KEY (`OwnerCharacterId`) REFERENCES `characters` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Dumping data for table charactercomputerprogramprocesses
--

/*!40000 ALTER TABLE `charactercomputerprogramprocesses` DISABLE KEYS */;
/*!40000 ALTER TABLE `charactercomputerprogramprocesses` ENABLE KEYS */;

--
-- Definition of characterlog
--

DROP TABLE IF EXISTS `characterlog`;
CREATE TABLE IF NOT EXISTS `characterlog` (
  `Id` bigint NOT NULL AUTO_INCREMENT,
  `AccountId` bigint DEFAULT NULL,
  `CharacterId` bigint NOT NULL,
  `CellId` bigint NOT NULL,
  `Command` text CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL,
  `Time` datetime NOT NULL,
  `IsPlayerCharacter` bit(1) NOT NULL,
  PRIMARY KEY (`Id`),
  KEY `FK_CharacterLog_Accounts_idx` (`AccountId`),
  KEY `FK_CharacterLog_Cells_idx` (`CellId`),
  KEY `FK_CharacterLog_Characters_idx` (`CharacterId`),
  CONSTRAINT `FK_CharacterLog_Accounts` FOREIGN KEY (`AccountId`) REFERENCES `accounts` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_CharacterLog_Cells` FOREIGN KEY (`CellId`) REFERENCES `cells` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_CharacterLog_Characters` FOREIGN KEY (`CharacterId`) REFERENCES `characters` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Dumping data for table characterlog
--

/*!40000 ALTER TABLE `characterlog` DISABLE KEYS */;
/*!40000 ALTER TABLE `characterlog` ENABLE KEYS */;

--
-- Definition of characters_accents
--

DROP TABLE IF EXISTS `characters_accents`;
CREATE TABLE IF NOT EXISTS `characters_accents` (
  `CharacterId` bigint NOT NULL,
  `AccentId` bigint NOT NULL,
  `Familiarity` int NOT NULL,
  `IsPreferred` bit(1) NOT NULL DEFAULT b'0',
  PRIMARY KEY (`CharacterId`,`AccentId`),
  KEY `FK_Characters_Accents_Accents_idx` (`AccentId`),
  CONSTRAINT `FK_Characters_Accents_Accents` FOREIGN KEY (`AccentId`) REFERENCES `accents` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_Characters_Accents_Characters` FOREIGN KEY (`CharacterId`) REFERENCES `characters` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Dumping data for table characters_accents
--

/*!40000 ALTER TABLE `characters_accents` DISABLE KEYS */;
/*!40000 ALTER TABLE `characters_accents` ENABLE KEYS */;

--
-- Definition of characters_scripts
--

DROP TABLE IF EXISTS `characters_scripts`;
CREATE TABLE IF NOT EXISTS `characters_scripts` (
  `CharacterId` bigint NOT NULL,
  `ScriptId` bigint NOT NULL,
  PRIMARY KEY (`CharacterId`,`ScriptId`),
  KEY `FK_Characters_Scripts_Scripts_idx` (`ScriptId`),
  CONSTRAINT `FK_Characters_Scripts_Characters` FOREIGN KEY (`CharacterId`) REFERENCES `characters` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_Characters_Scripts_Scripts` FOREIGN KEY (`ScriptId`) REFERENCES `scripts` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Dumping data for table characters_scripts
--

/*!40000 ALTER TABLE `characters_scripts` DISABLE KEYS */;
/*!40000 ALTER TABLE `characters_scripts` ENABLE KEYS */;

--
-- Definition of clanmemberships
--

DROP TABLE IF EXISTS `clanmemberships`;
CREATE TABLE IF NOT EXISTS `clanmemberships` (
  `ClanId` bigint NOT NULL,
  `CharacterId` bigint NOT NULL,
  `RankId` bigint NOT NULL,
  `PaygradeId` bigint DEFAULT NULL,
  `JoinDate` varchar(100) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `ManagerId` bigint DEFAULT NULL,
  `PersonalName` text CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci,
  `ArchivedMembership` bit(1) NOT NULL DEFAULT b'0',
  PRIMARY KEY (`ClanId`,`CharacterId`),
  KEY `FK_ClanMemberships_Characters` (`CharacterId`),
  KEY `FK_ClanMemberships_Manager` (`ManagerId`),
  CONSTRAINT `FK_ClanMemberships_Characters` FOREIGN KEY (`CharacterId`) REFERENCES `characters` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_ClanMemberships_Clans` FOREIGN KEY (`ClanId`) REFERENCES `clans` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_ClanMemberships_Manager` FOREIGN KEY (`ManagerId`) REFERENCES `characters` (`Id`) ON DELETE RESTRICT
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Dumping data for table clanmemberships
--

/*!40000 ALTER TABLE `clanmemberships` DISABLE KEYS */;
/*!40000 ALTER TABLE `clanmemberships` ENABLE KEYS */;

--
-- Definition of clanmemberships_appointments
--

DROP TABLE IF EXISTS `clanmemberships_appointments`;
CREATE TABLE IF NOT EXISTS `clanmemberships_appointments` (
  `ClanId` bigint NOT NULL,
  `CharacterId` bigint NOT NULL,
  `AppointmentId` bigint NOT NULL,
  PRIMARY KEY (`ClanId`,`CharacterId`,`AppointmentId`),
  KEY `FK_ClanMemberships_Appointments_Appointments` (`AppointmentId`),
  CONSTRAINT `FK_ClanMemberships_Appointments_Appointments` FOREIGN KEY (`AppointmentId`) REFERENCES `appointments` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_ClanMemberships_Appointments_ClanMemberships` FOREIGN KEY (`ClanId`, `CharacterId`) REFERENCES `clanmemberships` (`ClanId`, `CharacterId`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Dumping data for table clanmemberships_appointments
--

/*!40000 ALTER TABLE `clanmemberships_appointments` DISABLE KEYS */;
/*!40000 ALTER TABLE `clanmemberships_appointments` ENABLE KEYS */;

--
-- Definition of crimes
--

DROP TABLE IF EXISTS `crimes`;
CREATE TABLE IF NOT EXISTS `crimes` (
  `Id` bigint NOT NULL AUTO_INCREMENT,
  `LawId` bigint NOT NULL,
  `CriminalId` bigint NOT NULL,
  `VictimId` bigint DEFAULT NULL,
  `TimeOfCrime` varchar(200) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `RealTimeOfCrime` datetime NOT NULL,
  `LocationId` bigint DEFAULT NULL,
  `TimeOfReport` varchar(200) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci DEFAULT NULL,
  `AccuserId` bigint DEFAULT NULL,
  `CriminalShortDescription` varchar(500) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `CriminalFullDescription` text CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `CriminalCharacteristics` text CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `IsKnownCrime` bit(1) NOT NULL,
  `IsStaleCrime` bit(1) NOT NULL,
  `IsFinalised` bit(1) NOT NULL,
  `ConvictionRecorded` bit(1) NOT NULL,
  `IsCriminalIdentityKnown` bit(1) NOT NULL,
  `BailHasBeenPosted` bit(1) NOT NULL DEFAULT b'0',
  `ThirdPartyIItemType` varchar(100) DEFAULT NULL,
  `ThirdPartyId` bigint DEFAULT NULL,
  `WitnessIds` varchar(1000) DEFAULT NULL,
  `CalculatedBail` decimal(58,29) NOT NULL DEFAULT '0.00000000000000000000000000000',
  `CustodialSentenceLength` double NOT NULL DEFAULT '0',
  `FineRecorded` decimal(58,29) NOT NULL DEFAULT '0.00000000000000000000000000000',
  `HasBeenEnforced` bit(1) NOT NULL DEFAULT b'0',
  `ExecutionPunishment` bit(1) NOT NULL DEFAULT b'0',
  `FineHasBeenPaid` bit(1) NOT NULL DEFAULT b'0',
  `GoodBehaviourBond` double NOT NULL DEFAULT '0',
  `SentenceHasBeenServed` bit(1) NOT NULL DEFAULT b'0',
  `AdditionalInformation` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  PRIMARY KEY (`Id`),
  KEY `FK_Crimes_Accuser_idx` (`AccuserId`),
  KEY `FK_Crimes_Criminal_idx` (`CriminalId`),
  KEY `FK_Crimes_Laws_idx` (`LawId`),
  KEY `FK_Crimes_Location_idx` (`LocationId`),
  KEY `FK_Crimes_Victim_idx` (`VictimId`),
  CONSTRAINT `FK_Crimes_Accuser` FOREIGN KEY (`AccuserId`) REFERENCES `characters` (`Id`) ON DELETE SET NULL,
  CONSTRAINT `FK_Crimes_Criminal` FOREIGN KEY (`CriminalId`) REFERENCES `characters` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_Crimes_Laws` FOREIGN KEY (`LawId`) REFERENCES `laws` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_Crimes_Location` FOREIGN KEY (`LocationId`) REFERENCES `cells` (`Id`) ON DELETE SET NULL,
  CONSTRAINT `FK_Crimes_Victim` FOREIGN KEY (`VictimId`) REFERENCES `characters` (`Id`) ON DELETE SET NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Dumping data for table crimes
--

/*!40000 ALTER TABLE `crimes` DISABLE KEYS */;
/*!40000 ALTER TABLE `crimes` ENABLE KEYS */;

--
-- Definition of drawings
--

DROP TABLE IF EXISTS `drawings`;
CREATE TABLE IF NOT EXISTS `drawings` (
  `Id` bigint NOT NULL AUTO_INCREMENT,
  `AuthorId` bigint NOT NULL,
  `ShortDescription` text CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `FullDescription` text CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `ImplementType` int NOT NULL,
  `DrawingSkill` double NOT NULL,
  `DrawingSize` int NOT NULL,
  PRIMARY KEY (`Id`),
  KEY `FK_Drawings_Characters_idx` (`AuthorId`),
  CONSTRAINT `FK_Drawings_Characters` FOREIGN KEY (`AuthorId`) REFERENCES `characters` (`Id`) ON DELETE RESTRICT
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Dumping data for table drawings
--

/*!40000 ALTER TABLE `drawings` DISABLE KEYS */;
/*!40000 ALTER TABLE `drawings` ENABLE KEYS */;

--
-- Definition of dubs
--

DROP TABLE IF EXISTS `dubs`;
CREATE TABLE IF NOT EXISTS `dubs` (
  `Id` bigint NOT NULL AUTO_INCREMENT,
  `Keywords` varchar(4000) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `TargetId` bigint NOT NULL,
  `TargetType` varchar(100) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `LastDescription` varchar(4000) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `LastUsage` datetime NOT NULL,
  `CharacterId` bigint NOT NULL,
  `IntroducedName` varchar(4000) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci DEFAULT NULL,
  PRIMARY KEY (`Id`),
  KEY `FK_Dubs_Characters` (`CharacterId`),
  CONSTRAINT `FK_Dubs_Characters` FOREIGN KEY (`CharacterId`) REFERENCES `characters` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Dumping data for table dubs
--

/*!40000 ALTER TABLE `dubs` DISABLE KEYS */;
/*!40000 ALTER TABLE `dubs` ENABLE KEYS */;

--
-- Definition of electionsvotes
--

DROP TABLE IF EXISTS `electionsvotes`;
CREATE TABLE IF NOT EXISTS `electionsvotes` (
  `ElectionId` bigint NOT NULL,
  `VoterId` bigint NOT NULL,
  `NomineeId` bigint NOT NULL,
  `VoterClanId` bigint NOT NULL,
  `NomineeClanId` bigint NOT NULL,
  `NumberOfVotes` int NOT NULL,
  PRIMARY KEY (`ElectionId`,`NomineeId`,`VoterId`),
  KEY `FK_ElectionsVotes_Elections_idx` (`ElectionId`),
  KEY `FK_ElectionsVotes_Nominees_idx` (`NomineeClanId`,`NomineeId`),
  KEY `FK_ElectionsVotes_Voters_idx` (`VoterClanId`,`VoterId`),
  CONSTRAINT `FK_ElectionsVotes_Elections` FOREIGN KEY (`ElectionId`) REFERENCES `elections` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_ElectionsVotes_Nominees` FOREIGN KEY (`NomineeClanId`, `NomineeId`) REFERENCES `clanmemberships` (`ClanId`, `CharacterId`) ON DELETE CASCADE,
  CONSTRAINT `FK_ElectionsVotes_Voters` FOREIGN KEY (`VoterClanId`, `VoterId`) REFERENCES `clanmemberships` (`ClanId`, `CharacterId`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Dumping data for table electionsvotes
--

/*!40000 ALTER TABLE `electionsvotes` DISABLE KEYS */;
/*!40000 ALTER TABLE `electionsvotes` ENABLE KEYS */;

--
-- Definition of guests
--

DROP TABLE IF EXISTS `guests`;
CREATE TABLE IF NOT EXISTS `guests` (
  `CharacterId` bigint NOT NULL,
  PRIMARY KEY (`CharacterId`),
  CONSTRAINT `FK_Guests_Characters` FOREIGN KEY (`CharacterId`) REFERENCES `characters` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Dumping data for table guests
--

/*!40000 ALTER TABLE `guests` DISABLE KEYS */;
/*!40000 ALTER TABLE `guests` ENABLE KEYS */;

--
-- Definition of perceivermerits
--

DROP TABLE IF EXISTS `perceivermerits`;
CREATE TABLE IF NOT EXISTS `perceivermerits` (
  `Id` bigint NOT NULL AUTO_INCREMENT,
  `MeritId` bigint NOT NULL,
  `BodyId` bigint DEFAULT NULL,
  `CharacterId` bigint DEFAULT NULL,
  `GameItemId` bigint DEFAULT NULL,
  PRIMARY KEY (`Id`),
  KEY `FK_PerceiverMerits_Bodies_idx` (`BodyId`),
  KEY `FK_PerceiverMerits_Characters_idx` (`CharacterId`),
  KEY `FK_PerceiverMerits_GameItems_idx` (`GameItemId`),
  KEY ` FK_PerceiverMerits_Merits_idx` (`MeritId`),
  CONSTRAINT ` FK_PerceiverMerits_Merits` FOREIGN KEY (`MeritId`) REFERENCES `merits` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_PerceiverMerits_Bodies` FOREIGN KEY (`BodyId`) REFERENCES `bodies` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_PerceiverMerits_Characters` FOREIGN KEY (`CharacterId`) REFERENCES `characters` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Dumping data for table perceivermerits
--

/*!40000 ALTER TABLE `perceivermerits` DISABLE KEYS */;
/*!40000 ALTER TABLE `perceivermerits` ENABLE KEYS */;

--
-- Definition of scripts_designedlanguages
--

DROP TABLE IF EXISTS `scripts_designedlanguages`;
CREATE TABLE IF NOT EXISTS `scripts_designedlanguages` (
  `ScriptId` bigint NOT NULL,
  `LanguageId` bigint NOT NULL,
  PRIMARY KEY (`ScriptId`,`LanguageId`),
  KEY `FK_Scripts_DesignedLanguages_Languages_idx` (`LanguageId`),
  CONSTRAINT `FK_Scripts_DesignedLanguages_Languages` FOREIGN KEY (`LanguageId`) REFERENCES `languages` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_Scripts_DesignedLanguages_Scripts` FOREIGN KEY (`ScriptId`) REFERENCES `scripts` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Dumping data for table scripts_designedlanguages
--

/*!40000 ALTER TABLE `scripts_designedlanguages` DISABLE KEYS */;
/*!40000 ALTER TABLE `scripts_designedlanguages` ENABLE KEYS */;

--
-- Definition of seasons
--

DROP TABLE IF EXISTS `seasons`;
CREATE TABLE IF NOT EXISTS `seasons` (
  `Id` bigint NOT NULL AUTO_INCREMENT,
  `Name` varchar(200) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `CelestialDayOnset` int NOT NULL,
  `CelestialId` bigint NOT NULL,
  `DisplayName` varchar(200) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL DEFAULT '',
  `SeasonGroup` varchar(200) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL DEFAULT '',
  PRIMARY KEY (`Id`),
  KEY `FK_Seasons_Celestials_idx` (`CelestialId`),
  CONSTRAINT `FK_Seasons_Celestials` FOREIGN KEY (`CelestialId`) REFERENCES `celestials` (`Id`) ON DELETE RESTRICT
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Dumping data for table seasons
--

/*!40000 ALTER TABLE `seasons` DISABLE KEYS */;
/*!40000 ALTER TABLE `seasons` ENABLE KEYS */;

--
-- Definition of climatemodelseason
--

DROP TABLE IF EXISTS `climatemodelseason`;
CREATE TABLE IF NOT EXISTS `climatemodelseason` (
  `ClimateModelId` bigint NOT NULL,
  `SeasonId` bigint NOT NULL,
  `MaximumAdditionalChangeChanceFromStableWeather` double NOT NULL,
  `IncrementalAdditionalChangeChanceFromStableWeather` double NOT NULL,
  PRIMARY KEY (`ClimateModelId`,`SeasonId`),
  KEY `IX_ClimateModelSeason_SeasonId` (`SeasonId`),
  CONSTRAINT `FK_ClimateModelSeasons_ClimateModels` FOREIGN KEY (`ClimateModelId`) REFERENCES `climatemodels` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_ClimateModelSeasons_Seasons` FOREIGN KEY (`SeasonId`) REFERENCES `seasons` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Dumping data for table climatemodelseason
--

/*!40000 ALTER TABLE `climatemodelseason` DISABLE KEYS */;
/*!40000 ALTER TABLE `climatemodelseason` ENABLE KEYS */;

--
-- Definition of regionalclimates_seasons
--

DROP TABLE IF EXISTS `regionalclimates_seasons`;
CREATE TABLE IF NOT EXISTS `regionalclimates_seasons` (
  `RegionalClimateId` bigint NOT NULL,
  `SeasonId` bigint NOT NULL,
  `TemperatureInfo` text CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  PRIMARY KEY (`RegionalClimateId`,`SeasonId`),
  KEY `FK_RegionalClimates_Seasons_Seasons_idx` (`SeasonId`),
  CONSTRAINT `FK_RegionalClimates_Seasons_RegionalClimates` FOREIGN KEY (`RegionalClimateId`) REFERENCES `regionalclimates` (`Id`) ON DELETE RESTRICT,
  CONSTRAINT `FK_RegionalClimates_Seasons_Seasons` FOREIGN KEY (`SeasonId`) REFERENCES `seasons` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Dumping data for table regionalclimates_seasons
--

/*!40000 ALTER TABLE `regionalclimates_seasons` DISABLE KEYS */;
/*!40000 ALTER TABLE `regionalclimates_seasons` ENABLE KEYS */;

--
-- Definition of seederchoices
--

DROP TABLE IF EXISTS `seederchoices`;
CREATE TABLE IF NOT EXISTS `seederchoices` (
  `Id` bigint NOT NULL AUTO_INCREMENT,
  `Version` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  `Seeder` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  `Choice` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  `Answer` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  `DateTime` datetime(6) NOT NULL,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Dumping data for table seederchoices
--

/*!40000 ALTER TABLE `seederchoices` DISABLE KEYS */;
/*!40000 ALTER TABLE `seederchoices` ENABLE KEYS */;

--
-- Definition of skydescriptiontemplates
--

DROP TABLE IF EXISTS `skydescriptiontemplates`;
CREATE TABLE IF NOT EXISTS `skydescriptiontemplates` (
  `Id` bigint NOT NULL AUTO_INCREMENT,
  `Name` varchar(255) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Dumping data for table skydescriptiontemplates
--

/*!40000 ALTER TABLE `skydescriptiontemplates` DISABLE KEYS */;
/*!40000 ALTER TABLE `skydescriptiontemplates` ENABLE KEYS */;

--
-- Definition of shards
--

DROP TABLE IF EXISTS `shards`;
CREATE TABLE IF NOT EXISTS `shards` (
  `Id` bigint NOT NULL AUTO_INCREMENT,
  `Name` varchar(255) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `MinimumTerrestrialLux` double NOT NULL,
  `SkyDescriptionTemplateId` bigint NOT NULL,
  `SphericalRadiusMetres` double NOT NULL DEFAULT '6371000',
  PRIMARY KEY (`Id`),
  KEY `FK_Shards_SkyDescriptionTemplates` (`SkyDescriptionTemplateId`),
  CONSTRAINT `FK_Shards_SkyDescriptionTemplates` FOREIGN KEY (`SkyDescriptionTemplateId`) REFERENCES `skydescriptiontemplates` (`Id`) ON DELETE RESTRICT
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Dumping data for table shards
--

/*!40000 ALTER TABLE `shards` DISABLE KEYS */;
/*!40000 ALTER TABLE `shards` ENABLE KEYS */;

--
-- Definition of shards_calendars
--

DROP TABLE IF EXISTS `shards_calendars`;
CREATE TABLE IF NOT EXISTS `shards_calendars` (
  `ShardId` bigint NOT NULL,
  `CalendarId` bigint NOT NULL,
  PRIMARY KEY (`ShardId`,`CalendarId`),
  CONSTRAINT `FK_Shards_Calendars_Shards` FOREIGN KEY (`ShardId`) REFERENCES `shards` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Dumping data for table shards_calendars
--

/*!40000 ALTER TABLE `shards_calendars` DISABLE KEYS */;
/*!40000 ALTER TABLE `shards_calendars` ENABLE KEYS */;

--
-- Definition of shards_celestials
--

DROP TABLE IF EXISTS `shards_celestials`;
CREATE TABLE IF NOT EXISTS `shards_celestials` (
  `ShardId` bigint NOT NULL,
  `CelestialId` bigint NOT NULL,
  PRIMARY KEY (`ShardId`,`CelestialId`),
  CONSTRAINT `FK_Shards_Celestials_Shards` FOREIGN KEY (`ShardId`) REFERENCES `shards` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Dumping data for table shards_celestials
--

/*!40000 ALTER TABLE `shards_celestials` DISABLE KEYS */;
/*!40000 ALTER TABLE `shards_celestials` ENABLE KEYS */;

--
-- Definition of shards_clocks
--

DROP TABLE IF EXISTS `shards_clocks`;
CREATE TABLE IF NOT EXISTS `shards_clocks` (
  `ShardId` bigint NOT NULL,
  `ClockId` bigint NOT NULL,
  PRIMARY KEY (`ShardId`,`ClockId`),
  CONSTRAINT `FK_Shards_Clocks_Shards` FOREIGN KEY (`ShardId`) REFERENCES `shards` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Dumping data for table shards_clocks
--

/*!40000 ALTER TABLE `shards_clocks` DISABLE KEYS */;
/*!40000 ALTER TABLE `shards_clocks` ENABLE KEYS */;

--
-- Definition of skydescriptiontemplates_values
--

DROP TABLE IF EXISTS `skydescriptiontemplates_values`;
CREATE TABLE IF NOT EXISTS `skydescriptiontemplates_values` (
  `SkyDescriptionTemplateId` bigint NOT NULL,
  `LowerBound` double NOT NULL,
  `UpperBound` double NOT NULL,
  `Description` varchar(4000) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  PRIMARY KEY (`SkyDescriptionTemplateId`,`LowerBound`),
  CONSTRAINT `FK_SkyDescriptionTemplates_Values_SkyDescriptionTemplates` FOREIGN KEY (`SkyDescriptionTemplateId`) REFERENCES `skydescriptiontemplates` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Dumping data for table skydescriptiontemplates_values
--

/*!40000 ALTER TABLE `skydescriptiontemplates_values` DISABLE KEYS */;
/*!40000 ALTER TABLE `skydescriptiontemplates_values` ENABLE KEYS */;

--
-- Definition of socials
--

DROP TABLE IF EXISTS `socials`;
CREATE TABLE IF NOT EXISTS `socials` (
  `Name` varchar(100) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `NoTargetEcho` varchar(1000) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `OneTargetEcho` varchar(1000) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `FutureProgId` bigint DEFAULT NULL,
  `DirectionTargetEcho` varchar(1000) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci DEFAULT NULL,
  `MultiTargetEcho` varchar(1000) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci DEFAULT NULL,
  PRIMARY KEY (`Name`),
  KEY `FK_Socials_FutureProgs` (`FutureProgId`),
  CONSTRAINT `FK_Socials_FutureProgs` FOREIGN KEY (`FutureProgId`) REFERENCES `futureprogs` (`Id`) ON DELETE SET NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Dumping data for table socials
--

/*!40000 ALTER TABLE `socials` DISABLE KEYS */;
/*!40000 ALTER TABLE `socials` ENABLE KEYS */;

--
-- Definition of stables
--

DROP TABLE IF EXISTS `stables`;
CREATE TABLE IF NOT EXISTS `stables` (
  `Id` bigint NOT NULL AUTO_INCREMENT,
  `Name` varchar(200) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `EconomicZoneId` bigint NOT NULL,
  `CellId` bigint NOT NULL,
  `BankAccountId` bigint DEFAULT NULL,
  `IsTrading` bit(1) NOT NULL DEFAULT b'1',
  `LodgeFee` decimal(58,29) NOT NULL,
  `DailyFee` decimal(58,29) NOT NULL,
  `LodgeFeeProgId` bigint DEFAULT NULL,
  `DailyFeeProgId` bigint DEFAULT NULL,
  `CanStableProgId` bigint DEFAULT NULL,
  `WhyCannotStableProgId` bigint DEFAULT NULL,
  `EmployeeRecords` mediumtext CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  PRIMARY KEY (`Id`),
  KEY `FK_Stables_BankAccounts_idx` (`BankAccountId`),
  KEY `FK_Stables_Cells_idx` (`CellId`),
  KEY `FK_Stables_EconomicZones_idx` (`EconomicZoneId`),
  KEY `FK_Stables_FutureProgs_Can_idx` (`CanStableProgId`),
  KEY `FK_Stables_FutureProgs_Daily_idx` (`DailyFeeProgId`),
  KEY `FK_Stables_FutureProgs_Lodge_idx` (`LodgeFeeProgId`),
  KEY `FK_Stables_FutureProgs_Why_idx` (`WhyCannotStableProgId`),
  CONSTRAINT `FK_Stables_BankAccounts` FOREIGN KEY (`BankAccountId`) REFERENCES `bankaccounts` (`Id`) ON DELETE SET NULL,
  CONSTRAINT `FK_Stables_Cells` FOREIGN KEY (`CellId`) REFERENCES `cells` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_Stables_EconomicZones` FOREIGN KEY (`EconomicZoneId`) REFERENCES `economiczones` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_Stables_FutureProgs_Can` FOREIGN KEY (`CanStableProgId`) REFERENCES `futureprogs` (`Id`) ON DELETE SET NULL,
  CONSTRAINT `FK_Stables_FutureProgs_Daily` FOREIGN KEY (`DailyFeeProgId`) REFERENCES `futureprogs` (`Id`) ON DELETE SET NULL,
  CONSTRAINT `FK_Stables_FutureProgs_Lodge` FOREIGN KEY (`LodgeFeeProgId`) REFERENCES `futureprogs` (`Id`) ON DELETE SET NULL,
  CONSTRAINT `FK_Stables_FutureProgs_Why` FOREIGN KEY (`WhyCannotStableProgId`) REFERENCES `futureprogs` (`Id`) ON DELETE SET NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Dumping data for table stables
--

/*!40000 ALTER TABLE `stables` DISABLE KEYS */;
/*!40000 ALTER TABLE `stables` ENABLE KEYS */;

--
-- Definition of stableaccounts
--

DROP TABLE IF EXISTS `stableaccounts`;
CREATE TABLE IF NOT EXISTS `stableaccounts` (
  `Id` bigint NOT NULL AUTO_INCREMENT,
  `StableId` bigint NOT NULL,
  `AccountName` varchar(200) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `AccountOwnerId` bigint NOT NULL,
  `AccountOwnerName` mediumtext CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `Balance` decimal(58,29) NOT NULL,
  `CreditLimit` decimal(58,29) NOT NULL,
  `IsSuspended` bit(1) NOT NULL,
  PRIMARY KEY (`Id`),
  KEY `FK_StableAccounts_Characters_idx` (`AccountOwnerId`),
  KEY `FK_StableAccounts_Stables_idx` (`StableId`),
  CONSTRAINT `FK_StableAccounts_Characters` FOREIGN KEY (`AccountOwnerId`) REFERENCES `characters` (`Id`) ON DELETE RESTRICT,
  CONSTRAINT `FK_StableAccounts_Stables` FOREIGN KEY (`StableId`) REFERENCES `stables` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Dumping data for table stableaccounts
--

/*!40000 ALTER TABLE `stableaccounts` DISABLE KEYS */;
/*!40000 ALTER TABLE `stableaccounts` ENABLE KEYS */;

--
-- Definition of stableaccountusers
--

DROP TABLE IF EXISTS `stableaccountusers`;
CREATE TABLE IF NOT EXISTS `stableaccountusers` (
  `StableAccountId` bigint NOT NULL,
  `AccountUserId` bigint NOT NULL,
  `AccountUserName` mediumtext CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `SpendingLimit` decimal(58,29) DEFAULT NULL,
  PRIMARY KEY (`StableAccountId`,`AccountUserId`),
  KEY `FK_StableAccountUsers_Characters_idx` (`AccountUserId`),
  CONSTRAINT `FK_StableAccountUsers_Characters` FOREIGN KEY (`AccountUserId`) REFERENCES `characters` (`Id`) ON DELETE RESTRICT,
  CONSTRAINT `FK_StableAccountUsers_StableAccounts` FOREIGN KEY (`StableAccountId`) REFERENCES `stableaccounts` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Dumping data for table stableaccountusers
--

/*!40000 ALTER TABLE `stableaccountusers` DISABLE KEYS */;
/*!40000 ALTER TABLE `stableaccountusers` ENABLE KEYS */;

--
-- Definition of stablestays
--

DROP TABLE IF EXISTS `stablestays`;
CREATE TABLE IF NOT EXISTS `stablestays` (
  `Id` bigint NOT NULL AUTO_INCREMENT,
  `StableId` bigint NOT NULL,
  `MountId` bigint NOT NULL,
  `OriginalOwnerId` bigint NOT NULL,
  `OriginalOwnerName` mediumtext CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `LodgedDateTime` varchar(500) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `LastDailyFeeDateTime` varchar(500) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `ClosedDateTime` varchar(500) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci DEFAULT NULL,
  `Status` int NOT NULL,
  `TicketItemId` bigint DEFAULT NULL,
  `TicketToken` varchar(200) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `AmountOwing` decimal(58,29) NOT NULL,
  `MountInstanceId` bigint DEFAULT NULL,
  PRIMARY KEY (`Id`),
  KEY `FK_StableStays_Characters_Mount_idx` (`MountId`),
  KEY `FK_StableStays_Characters_Owner_idx` (`OriginalOwnerId`),
  KEY `FK_StableStays_GameItems_Ticket_idx` (`TicketItemId`),
  KEY `FK_StableStays_Stables_idx` (`StableId`),
  KEY `FK_StableStays_CharacterInstances_Mount_idx` (`MountInstanceId`),
  CONSTRAINT `FK_StableStays_Characters_Mount` FOREIGN KEY (`MountId`) REFERENCES `characters` (`Id`) ON DELETE RESTRICT,
  CONSTRAINT `FK_StableStays_Characters_Owner` FOREIGN KEY (`OriginalOwnerId`) REFERENCES `characters` (`Id`) ON DELETE RESTRICT,
  CONSTRAINT `FK_StableStays_GameItems_Ticket` FOREIGN KEY (`TicketItemId`) REFERENCES `gameitems` (`Id`) ON DELETE SET NULL,
  CONSTRAINT `FK_StableStays_Stables` FOREIGN KEY (`StableId`) REFERENCES `stables` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Dumping data for table stablestays
--

/*!40000 ALTER TABLE `stablestays` DISABLE KEYS */;
/*!40000 ALTER TABLE `stablestays` ENABLE KEYS */;

--
-- Definition of stablestayledgerentries
--

DROP TABLE IF EXISTS `stablestayledgerentries`;
CREATE TABLE IF NOT EXISTS `stablestayledgerentries` (
  `Id` bigint NOT NULL AUTO_INCREMENT,
  `StableStayId` bigint NOT NULL,
  `EntryType` int NOT NULL,
  `MudDateTime` varchar(500) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `ActorId` bigint DEFAULT NULL,
  `ActorName` mediumtext CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci,
  `Amount` decimal(58,29) NOT NULL,
  `Note` mediumtext CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  PRIMARY KEY (`Id`),
  KEY `FK_StableStayLedgerEntries_Characters_idx` (`ActorId`),
  KEY `FK_StableStayLedgerEntries_StableStays_idx` (`StableStayId`),
  CONSTRAINT `FK_StableStayLedgerEntries_Characters` FOREIGN KEY (`ActorId`) REFERENCES `characters` (`Id`) ON DELETE SET NULL,
  CONSTRAINT `FK_StableStayLedgerEntries_StableStays` FOREIGN KEY (`StableStayId`) REFERENCES `stablestays` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Dumping data for table stablestayledgerentries
--

/*!40000 ALTER TABLE `stablestayledgerentries` DISABLE KEYS */;
/*!40000 ALTER TABLE `stablestayledgerentries` ENABLE KEYS */;

--
-- Definition of stackdecorators
--

DROP TABLE IF EXISTS `stackdecorators`;
CREATE TABLE IF NOT EXISTS `stackdecorators` (
  `Id` bigint NOT NULL AUTO_INCREMENT,
  `Name` varchar(255) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `Type` varchar(50) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `Definition` varchar(10000) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `Description` varchar(1000) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci DEFAULT NULL,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Dumping data for table stackdecorators
--

/*!40000 ALTER TABLE `stackdecorators` DISABLE KEYS */;
/*!40000 ALTER TABLE `stackdecorators` ENABLE KEYS */;

--
-- Definition of staticconfigurations
--

DROP TABLE IF EXISTS `staticconfigurations`;
CREATE TABLE IF NOT EXISTS `staticconfigurations` (
  `SettingName` varchar(100) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `Definition` text CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  PRIMARY KEY (`SettingName`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Dumping data for table staticconfigurations
--

/*!40000 ALTER TABLE `staticconfigurations` DISABLE KEYS */;
/*!40000 ALTER TABLE `staticconfigurations` ENABLE KEYS */;

--
-- Definition of staticstrings
--

DROP TABLE IF EXISTS `staticstrings`;
CREATE TABLE IF NOT EXISTS `staticstrings` (
  `Id` varchar(50) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `Text` text CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Dumping data for table staticstrings
--

/*!40000 ALTER TABLE `staticstrings` DISABLE KEYS */;
/*!40000 ALTER TABLE `staticstrings` ENABLE KEYS */;

--
-- Definition of tags
--

DROP TABLE IF EXISTS `tags`;
CREATE TABLE IF NOT EXISTS `tags` (
  `Id` bigint NOT NULL AUTO_INCREMENT,
  `Name` varchar(255) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `ParentId` bigint DEFAULT NULL,
  `ShouldSeeProgId` bigint DEFAULT NULL,
  PRIMARY KEY (`Id`),
  KEY `FK_Tags_Parent_idx` (`ParentId`),
  KEY `FK_Tags_Futureprogs_idx` (`ShouldSeeProgId`),
  CONSTRAINT `FK_Tags_Futureprogs` FOREIGN KEY (`ShouldSeeProgId`) REFERENCES `futureprogs` (`Id`) ON DELETE SET NULL,
  CONSTRAINT `FK_Tags_Parent` FOREIGN KEY (`ParentId`) REFERENCES `tags` (`Id`) ON DELETE SET NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Dumping data for table tags
--

/*!40000 ALTER TABLE `tags` DISABLE KEYS */;
/*!40000 ALTER TABLE `tags` ENABLE KEYS */;

--
-- Definition of cells_tags
--

DROP TABLE IF EXISTS `cells_tags`;
CREATE TABLE IF NOT EXISTS `cells_tags` (
  `CellId` bigint NOT NULL,
  `TagId` bigint NOT NULL,
  PRIMARY KEY (`CellId`,`TagId`),
  KEY `FK_Cells_Tags_Tags_idx` (`TagId`),
  CONSTRAINT `FK_Cells_Tags_Cells` FOREIGN KEY (`CellId`) REFERENCES `cells` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_Cells_Tags_Tags` FOREIGN KEY (`TagId`) REFERENCES `tags` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Dumping data for table cells_tags
--

/*!40000 ALTER TABLE `cells_tags` DISABLE KEYS */;
/*!40000 ALTER TABLE `cells_tags` ENABLE KEYS */;

--
-- Definition of commodityspoilagerules
--

DROP TABLE IF EXISTS `commodityspoilagerules`;
CREATE TABLE IF NOT EXISTS `commodityspoilagerules` (
  `Id` bigint NOT NULL AUTO_INCREMENT,
  `Name` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL,
  `Description` text CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL,
  `Enabled` bit(1) NOT NULL DEFAULT b'1',
  `Priority` int NOT NULL DEFAULT '0',
  `MaterialId` bigint DEFAULT NULL,
  `MaterialTagId` bigint DEFAULT NULL,
  `CommodityTagId` bigint DEFAULT NULL,
  `ResultMaterialId` bigint NOT NULL,
  `ResultCommodityTagId` bigint DEFAULT NULL,
  `SecondsUntilSpoiled` bigint NOT NULL,
  `SpoilEcho` text CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci,
  PRIMARY KEY (`Id`),
  UNIQUE KEY `IX_CommoditySpoilageRules_Name` (`Name`),
  KEY `FK_CommoditySpoilageRules_CommodityTags_idx` (`CommodityTagId`),
  KEY `FK_CommoditySpoilageRules_MaterialTags_idx` (`MaterialTagId`),
  KEY `FK_CommoditySpoilageRules_Materials_idx` (`MaterialId`),
  KEY `FK_CommoditySpoilageRules_ResultMaterials_idx` (`ResultMaterialId`),
  KEY `FK_CommoditySpoilageRules_ResultTags_idx` (`ResultCommodityTagId`),
  CONSTRAINT `FK_CommoditySpoilageRules_CommodityTags` FOREIGN KEY (`CommodityTagId`) REFERENCES `tags` (`Id`) ON DELETE SET NULL,
  CONSTRAINT `FK_CommoditySpoilageRules_Materials` FOREIGN KEY (`MaterialId`) REFERENCES `materials` (`Id`) ON DELETE SET NULL,
  CONSTRAINT `FK_CommoditySpoilageRules_MaterialTags` FOREIGN KEY (`MaterialTagId`) REFERENCES `tags` (`Id`) ON DELETE SET NULL,
  CONSTRAINT `FK_CommoditySpoilageRules_ResultMaterials` FOREIGN KEY (`ResultMaterialId`) REFERENCES `materials` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_CommoditySpoilageRules_ResultTags` FOREIGN KEY (`ResultCommodityTagId`) REFERENCES `tags` (`Id`) ON DELETE SET NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Dumping data for table commodityspoilagerules
--

/*!40000 ALTER TABLE `commodityspoilagerules` DISABLE KEYS */;
/*!40000 ALTER TABLE `commodityspoilagerules` ENABLE KEYS */;

--
-- Definition of gameitemprotos_tags
--

DROP TABLE IF EXISTS `gameitemprotos_tags`;
CREATE TABLE IF NOT EXISTS `gameitemprotos_tags` (
  `GameItemProtoId` bigint NOT NULL,
  `TagId` bigint NOT NULL,
  `GameItemProtoRevisionNumber` int NOT NULL,
  PRIMARY KEY (`GameItemProtoId`,`TagId`,`GameItemProtoRevisionNumber`),
  KEY `FK_GameItemProtos_Tags_Tags` (`TagId`),
  KEY `FK_GameItemProtos_Tags_GameItemProtos` (`GameItemProtoId`,`GameItemProtoRevisionNumber`),
  CONSTRAINT `FK_GameItemProtos_Tags_GameItemProtos` FOREIGN KEY (`GameItemProtoId`, `GameItemProtoRevisionNumber`) REFERENCES `gameitemprotos` (`Id`, `RevisionNumber`) ON DELETE CASCADE,
  CONSTRAINT `FK_GameItemProtos_Tags_Tags` FOREIGN KEY (`TagId`) REFERENCES `tags` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Dumping data for table gameitemprotos_tags
--

/*!40000 ALTER TABLE `gameitemprotos_tags` DISABLE KEYS */;
/*!40000 ALTER TABLE `gameitemprotos_tags` ENABLE KEYS */;

--
-- Definition of gases_tags
--

DROP TABLE IF EXISTS `gases_tags`;
CREATE TABLE IF NOT EXISTS `gases_tags` (
  `GasId` bigint NOT NULL,
  `TagId` bigint NOT NULL,
  PRIMARY KEY (`GasId`,`TagId`),
  KEY `FK_Gases_Tags_Tags_idx` (`TagId`),
  CONSTRAINT `FK_Gases_Tags_Gases` FOREIGN KEY (`GasId`) REFERENCES `gases` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_Gases_Tags_Tags` FOREIGN KEY (`TagId`) REFERENCES `tags` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Dumping data for table gases_tags
--

/*!40000 ALTER TABLE `gases_tags` DISABLE KEYS */;
/*!40000 ALTER TABLE `gases_tags` ENABLE KEYS */;

--
-- Definition of liquids_tags
--

DROP TABLE IF EXISTS `liquids_tags`;
CREATE TABLE IF NOT EXISTS `liquids_tags` (
  `LiquidId` bigint NOT NULL,
  `TagId` bigint NOT NULL,
  PRIMARY KEY (`LiquidId`,`TagId`),
  KEY `FK_Liquids_Tags_Tags_idx` (`TagId`),
  CONSTRAINT `FK_Liquids_Tags_Liquids` FOREIGN KEY (`LiquidId`) REFERENCES `liquids` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_Liquids_Tags_Tags` FOREIGN KEY (`TagId`) REFERENCES `tags` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Dumping data for table liquids_tags
--

/*!40000 ALTER TABLE `liquids_tags` DISABLE KEYS */;
/*!40000 ALTER TABLE `liquids_tags` ENABLE KEYS */;

--
-- Definition of marketcategories
--

DROP TABLE IF EXISTS `marketcategories`;
CREATE TABLE IF NOT EXISTS `marketcategories` (
  `Id` bigint NOT NULL AUTO_INCREMENT,
  `Name` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  `Description` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  `ElasticityFactorAbove` double NOT NULL,
  `ElasticityFactorBelow` double NOT NULL,
  `Tags` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  `CombinationCategories` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  `MarketCategoryType` int NOT NULL DEFAULT '0',
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Dumping data for table marketcategories
--

/*!40000 ALTER TABLE `marketcategories` DISABLE KEYS */;
/*!40000 ALTER TABLE `marketcategories` ENABLE KEYS */;

--
-- Definition of materials_tags
--

DROP TABLE IF EXISTS `materials_tags`;
CREATE TABLE IF NOT EXISTS `materials_tags` (
  `MaterialId` bigint NOT NULL,
  `TagId` bigint NOT NULL,
  PRIMARY KEY (`MaterialId`,`TagId`),
  KEY `Materials_Tags_Tags_idx` (`TagId`),
  CONSTRAINT `Materials_Tags_Materials` FOREIGN KEY (`MaterialId`) REFERENCES `materials` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `Materials_Tags_Tags` FOREIGN KEY (`TagId`) REFERENCES `tags` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Dumping data for table materials_tags
--

/*!40000 ALTER TABLE `materials_tags` DISABLE KEYS */;
/*!40000 ALTER TABLE `materials_tags` ENABLE KEYS */;

--
-- Definition of racebutcheryprofiles
--

DROP TABLE IF EXISTS `racebutcheryprofiles`;
CREATE TABLE IF NOT EXISTS `racebutcheryprofiles` (
  `Id` bigint NOT NULL AUTO_INCREMENT,
  `Name` varchar(100) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `Verb` int NOT NULL,
  `RequiredToolTagId` bigint DEFAULT NULL,
  `DifficultySkin` int NOT NULL,
  `CanButcherProgId` bigint DEFAULT NULL,
  `WhyCannotButcherProgId` bigint DEFAULT NULL,
  PRIMARY KEY (`Id`),
  KEY `FK_RaceButcheryProfiles_FutureProgs_Can_idx` (`CanButcherProgId`),
  KEY `FK_RaceButcheryProfiles_Tags_idx` (`RequiredToolTagId`),
  KEY `FK_RaceButcheryProfiles_FutureProgs_Why_idx` (`WhyCannotButcherProgId`),
  CONSTRAINT `FK_RaceButcheryProfiles_FutureProgs_Can` FOREIGN KEY (`CanButcherProgId`) REFERENCES `futureprogs` (`Id`) ON DELETE SET NULL,
  CONSTRAINT `FK_RaceButcheryProfiles_FutureProgs_Why` FOREIGN KEY (`WhyCannotButcherProgId`) REFERENCES `futureprogs` (`Id`) ON DELETE SET NULL,
  CONSTRAINT `FK_RaceButcheryProfiles_Tags` FOREIGN KEY (`RequiredToolTagId`) REFERENCES `tags` (`Id`) ON DELETE SET NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Dumping data for table racebutcheryprofiles
--

/*!40000 ALTER TABLE `racebutcheryprofiles` DISABLE KEYS */;
/*!40000 ALTER TABLE `racebutcheryprofiles` ENABLE KEYS */;

--
-- Definition of racebutcheryprofiles_breakdownemotes
--

DROP TABLE IF EXISTS `racebutcheryprofiles_breakdownemotes`;
CREATE TABLE IF NOT EXISTS `racebutcheryprofiles_breakdownemotes` (
  `RaceButcheryProfileId` bigint NOT NULL,
  `Subcategory` varchar(100) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `Order` int NOT NULL,
  `Emote` text CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `Delay` double NOT NULL,
  PRIMARY KEY (`RaceButcheryProfileId`,`Subcategory`,`Order`),
  CONSTRAINT `FK_RaceButcheryProfiles_BreakdownEmotes_RaceButcheryProfiles` FOREIGN KEY (`RaceButcheryProfileId`) REFERENCES `racebutcheryprofiles` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Dumping data for table racebutcheryprofiles_breakdownemotes
--

/*!40000 ALTER TABLE `racebutcheryprofiles_breakdownemotes` DISABLE KEYS */;
/*!40000 ALTER TABLE `racebutcheryprofiles_breakdownemotes` ENABLE KEYS */;

--
-- Definition of racebutcheryprofiles_butcheryproducts
--

DROP TABLE IF EXISTS `racebutcheryprofiles_butcheryproducts`;
CREATE TABLE IF NOT EXISTS `racebutcheryprofiles_butcheryproducts` (
  `RaceButcheryProfileId` bigint NOT NULL,
  `ButcheryProductId` bigint NOT NULL,
  PRIMARY KEY (`RaceButcheryProfileId`,`ButcheryProductId`),
  KEY `FK_RaceButcheryProfiles_ButcheryProducts_ButcheryProducts_idx` (`ButcheryProductId`),
  CONSTRAINT `FK_RaceButcheryProfiles_ButcheryProducts_ButcheryProducts` FOREIGN KEY (`ButcheryProductId`) REFERENCES `butcheryproducts` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_RaceButcheryProfiles_ButcheryProducts_RaceButcheryProfiles` FOREIGN KEY (`RaceButcheryProfileId`) REFERENCES `racebutcheryprofiles` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Dumping data for table racebutcheryprofiles_butcheryproducts
--

/*!40000 ALTER TABLE `racebutcheryprofiles_butcheryproducts` DISABLE KEYS */;
/*!40000 ALTER TABLE `racebutcheryprofiles_butcheryproducts` ENABLE KEYS */;

--
-- Definition of racebutcheryprofiles_skinningemotes
--

DROP TABLE IF EXISTS `racebutcheryprofiles_skinningemotes`;
CREATE TABLE IF NOT EXISTS `racebutcheryprofiles_skinningemotes` (
  `RaceButcheryProfileId` bigint NOT NULL,
  `Subcategory` varchar(100) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `Order` int NOT NULL,
  `Emote` text CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `Delay` double NOT NULL,
  PRIMARY KEY (`RaceButcheryProfileId`,`Subcategory`,`Order`),
  CONSTRAINT `FK_RaceButcheryProfiles_SkinningEmotes_RaceButcheryProfiles` FOREIGN KEY (`RaceButcheryProfileId`) REFERENCES `racebutcheryprofiles` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Dumping data for table racebutcheryprofiles_skinningemotes
--

/*!40000 ALTER TABLE `racebutcheryprofiles_skinningemotes` DISABLE KEYS */;
/*!40000 ALTER TABLE `racebutcheryprofiles_skinningemotes` ENABLE KEYS */;

--
-- Definition of shopdeals
--

DROP TABLE IF EXISTS `shopdeals`;
CREATE TABLE IF NOT EXISTS `shopdeals` (
  `Id` bigint NOT NULL AUTO_INCREMENT,
  `ShopId` bigint NOT NULL,
  `Name` varchar(200) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `DealType` int NOT NULL,
  `TargetType` int NOT NULL,
  `MerchandiseId` bigint DEFAULT NULL,
  `TagId` bigint DEFAULT NULL,
  `PriceAdjustmentPercentage` decimal(58,29) NOT NULL,
  `MinimumQuantity` int DEFAULT NULL,
  `Applicability` int NOT NULL,
  `EligibilityProgId` bigint DEFAULT NULL,
  `ExpiryDateTime` varchar(500) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci DEFAULT NULL,
  `IsCumulative` bit(1) NOT NULL DEFAULT b'1',
  PRIMARY KEY (`Id`),
  KEY `FK_ShopDeals_FutureProgs_idx` (`EligibilityProgId`),
  KEY `FK_ShopDeals_Merchandises_idx` (`MerchandiseId`),
  KEY `FK_ShopDeals_Shops_idx` (`ShopId`),
  KEY `FK_ShopDeals_Tags_idx` (`TagId`),
  CONSTRAINT `FK_ShopDeals_FutureProgs` FOREIGN KEY (`EligibilityProgId`) REFERENCES `futureprogs` (`Id`) ON DELETE SET NULL,
  CONSTRAINT `FK_ShopDeals_Merchandises` FOREIGN KEY (`MerchandiseId`) REFERENCES `merchandises` (`Id`) ON DELETE SET NULL,
  CONSTRAINT `FK_ShopDeals_Shops` FOREIGN KEY (`ShopId`) REFERENCES `shops` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_ShopDeals_Tags` FOREIGN KEY (`TagId`) REFERENCES `tags` (`Id`) ON DELETE SET NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Dumping data for table shopdeals
--

/*!40000 ALTER TABLE `shopdeals` DISABLE KEYS */;
/*!40000 ALTER TABLE `shopdeals` ENABLE KEYS */;

--
-- Definition of timezoneinfos
--

DROP TABLE IF EXISTS `timezoneinfos`;
CREATE TABLE IF NOT EXISTS `timezoneinfos` (
  `Id` varchar(100) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `Display` varchar(1000) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `Order` decimal(58,29) NOT NULL,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Dumping data for table timezoneinfos
--

/*!40000 ALTER TABLE `timezoneinfos` DISABLE KEYS */;
/*!40000 ALTER TABLE `timezoneinfos` ENABLE KEYS */;

--
-- Definition of timezones
--

DROP TABLE IF EXISTS `timezones`;
CREATE TABLE IF NOT EXISTS `timezones` (
  `Id` bigint NOT NULL AUTO_INCREMENT,
  `Name` varchar(255) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `Description` varchar(4000) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `OffsetMinutes` int NOT NULL,
  `OffsetHours` int NOT NULL,
  `ClockId` bigint NOT NULL,
  PRIMARY KEY (`Id`),
  KEY `FK_Timezones_Clocks` (`ClockId`),
  CONSTRAINT `FK_Timezones_Clocks` FOREIGN KEY (`ClockId`) REFERENCES `clocks` (`Id`) ON DELETE RESTRICT
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Dumping data for table timezones
--

/*!40000 ALTER TABLE `timezones` DISABLE KEYS */;
/*!40000 ALTER TABLE `timezones` ENABLE KEYS */;

--
-- Definition of economiczones
--

DROP TABLE IF EXISTS `economiczones`;
CREATE TABLE IF NOT EXISTS `economiczones` (
  `Id` bigint NOT NULL AUTO_INCREMENT,
  `Name` varchar(200) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `PreviousFinancialPeriodsToKeep` int NOT NULL DEFAULT '50',
  `ZoneForTimePurposesId` bigint NOT NULL,
  `PermitTaxableLosses` bit(1) NOT NULL DEFAULT b'1',
  `OutstandingTaxesOwed` decimal(58,29) NOT NULL,
  `CurrencyId` bigint NOT NULL,
  `CurrentFinancialPeriodId` bigint DEFAULT NULL,
  `ReferenceCalendarId` bigint DEFAULT NULL,
  `ReferenceClockId` bigint NOT NULL,
  `ReferenceTime` varchar(100) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `IntervalType` int NOT NULL DEFAULT '2',
  `IntervalModifier` int NOT NULL,
  `IntervalAmount` int NOT NULL DEFAULT '1',
  `TotalRevenueHeld` decimal(58,29) NOT NULL DEFAULT '0.00000000000000000000000000000',
  `ControllingClanId` bigint DEFAULT NULL,
  `EstateAuctionHouseId` bigint DEFAULT NULL,
  `EstateClaimPeriodLength` varchar(200) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci DEFAULT NULL,
  `EstateDefaultDiscoverTime` varchar(200) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci DEFAULT NULL,
  `MorgueOfficeLocationId` bigint DEFAULT NULL,
  `MorgueStorageLocationId` bigint DEFAULT NULL,
  `EstatesEnabled` bit(1) NOT NULL DEFAULT b'1',
  `IntervalFallback` int NOT NULL DEFAULT '0',
  `IntervalOther` int NOT NULL DEFAULT '0',
  PRIMARY KEY (`Id`),
  KEY `FK_EconomicZones_Currencies_idx` (`CurrencyId`),
  KEY `FK_EconomicZones_FinancialPeriods_idx` (`CurrentFinancialPeriodId`),
  KEY `FK_EconomicZones_Calendars_idx` (`ReferenceCalendarId`),
  KEY `FK_EconomicZones_Timezones_idx` (`ReferenceClockId`),
  KEY `FK_EconomicZones_ControllingClans_idx` (`ControllingClanId`),
  KEY `FK_EconomicZones_EstateAuctionHouses_idx` (`EstateAuctionHouseId`),
  KEY `IX_EconomicZones_MorgueOfficeLocationId` (`MorgueOfficeLocationId`),
  KEY `IX_EconomicZones_MorgueStorageLocationId` (`MorgueStorageLocationId`),
  CONSTRAINT `FK_EconomicZones_Calendars` FOREIGN KEY (`ReferenceCalendarId`) REFERENCES `calendars` (`Id`) ON DELETE SET NULL,
  CONSTRAINT `FK_EconomicZones_Clocks` FOREIGN KEY (`ReferenceClockId`) REFERENCES `clocks` (`Id`) ON DELETE RESTRICT,
  CONSTRAINT `FK_EconomicZones_ControllingClans` FOREIGN KEY (`ControllingClanId`) REFERENCES `clans` (`Id`) ON DELETE RESTRICT,
  CONSTRAINT `FK_EconomicZones_Currencies` FOREIGN KEY (`CurrencyId`) REFERENCES `currencies` (`Id`) ON DELETE RESTRICT,
  CONSTRAINT `FK_EconomicZones_EstateAuctionHouses` FOREIGN KEY (`EstateAuctionHouseId`) REFERENCES `auctionhouses` (`Id`) ON DELETE SET NULL,
  CONSTRAINT `FK_EconomicZones_FinancialPeriods` FOREIGN KEY (`CurrentFinancialPeriodId`) REFERENCES `financialperiods` (`Id`) ON DELETE SET NULL,
  CONSTRAINT `FK_EconomicZones_MorgueOfficeLocations` FOREIGN KEY (`MorgueOfficeLocationId`) REFERENCES `cells` (`Id`) ON DELETE SET NULL,
  CONSTRAINT `FK_EconomicZones_MorgueStorageLocations` FOREIGN KEY (`MorgueStorageLocationId`) REFERENCES `cells` (`Id`) ON DELETE SET NULL,
  CONSTRAINT `FK_EconomicZones_Timezones` FOREIGN KEY (`ReferenceClockId`) REFERENCES `timezones` (`Id`) ON DELETE RESTRICT
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Dumping data for table economiczones
--

/*!40000 ALTER TABLE `economiczones` DISABLE KEYS */;
/*!40000 ALTER TABLE `economiczones` ENABLE KEYS */;

--
-- Definition of auctionhouses
--

DROP TABLE IF EXISTS `auctionhouses`;
CREATE TABLE IF NOT EXISTS `auctionhouses` (
  `Id` bigint NOT NULL AUTO_INCREMENT,
  `Name` varchar(250) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `EconomicZoneId` bigint NOT NULL,
  `AuctionHouseCellId` bigint NOT NULL,
  `ProfitsBankAccountId` bigint DEFAULT NULL,
  `AuctionListingFeeFlat` decimal(58,29) NOT NULL,
  `AuctionListingFeeRate` decimal(58,29) NOT NULL,
  `Definition` mediumtext CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `DefaultListingTime` double NOT NULL,
  PRIMARY KEY (`Id`),
  KEY `IX_AuctionHouses_AuctionHouseCellId` (`AuctionHouseCellId`),
  KEY `IX_AuctionHouses_EconomicZoneId` (`EconomicZoneId`),
  KEY `IX_AuctionHouses_ProfitsBankAccountId` (`ProfitsBankAccountId`),
  CONSTRAINT `FK_AuctionHouses_BankAccounts` FOREIGN KEY (`ProfitsBankAccountId`) REFERENCES `bankaccounts` (`Id`) ON DELETE SET NULL,
  CONSTRAINT `FK_AuctionHouses_Cells` FOREIGN KEY (`AuctionHouseCellId`) REFERENCES `cells` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_AuctionHouses_EconomicZones` FOREIGN KEY (`EconomicZoneId`) REFERENCES `economiczones` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Dumping data for table auctionhouses
--

/*!40000 ALTER TABLE `auctionhouses` DISABLE KEYS */;
/*!40000 ALTER TABLE `auctionhouses` ENABLE KEYS */;

--
-- Definition of banks
--

DROP TABLE IF EXISTS `banks`;
CREATE TABLE IF NOT EXISTS `banks` (
  `Id` bigint NOT NULL AUTO_INCREMENT,
  `Name` varchar(255) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `Code` varchar(255) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `EconomicZoneId` bigint NOT NULL,
  `PrimaryCurrencyId` bigint NOT NULL,
  `MaximumBankAccountsPerCustomer` int NOT NULL,
  PRIMARY KEY (`Id`),
  KEY `IX_Banks_EconomicZoneId` (`EconomicZoneId`),
  KEY `IX_Banks_PrimaryCurrencyId` (`PrimaryCurrencyId`),
  CONSTRAINT `FK_Banks_Currencies` FOREIGN KEY (`PrimaryCurrencyId`) REFERENCES `currencies` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_Banks_EconomicZones` FOREIGN KEY (`EconomicZoneId`) REFERENCES `economiczones` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Dumping data for table banks
--

/*!40000 ALTER TABLE `banks` DISABLE KEYS */;
/*!40000 ALTER TABLE `banks` ENABLE KEYS */;

--
-- Definition of conveyancinglocations
--

DROP TABLE IF EXISTS `conveyancinglocations`;
CREATE TABLE IF NOT EXISTS `conveyancinglocations` (
  `EconomicZoneId` bigint NOT NULL,
  `CellId` bigint NOT NULL,
  PRIMARY KEY (`EconomicZoneId`,`CellId`),
  KEY `IX_ConveyancingLocations_CellId` (`CellId`),
  CONSTRAINT `FK_ConveyancingLocations_Cells` FOREIGN KEY (`CellId`) REFERENCES `cells` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_ConveyancingLocations_EconomicZones` FOREIGN KEY (`EconomicZoneId`) REFERENCES `economiczones` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Dumping data for table conveyancinglocations
--

/*!40000 ALTER TABLE `conveyancinglocations` DISABLE KEYS */;
/*!40000 ALTER TABLE `conveyancinglocations` ENABLE KEYS */;

--
-- Definition of estates
--

DROP TABLE IF EXISTS `estates`;
CREATE TABLE IF NOT EXISTS `estates` (
  `Id` bigint NOT NULL AUTO_INCREMENT,
  `EconomicZoneId` bigint NOT NULL,
  `CharacterId` bigint NOT NULL,
  `EstateStatus` int NOT NULL,
  `EstateStartTime` varchar(255) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `FinalisationDate` varchar(255) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci DEFAULT NULL,
  `InheritorId` bigint DEFAULT NULL,
  `InheritorType` varchar(100) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci DEFAULT NULL,
  PRIMARY KEY (`Id`),
  KEY `FK_Estates_Characters_idx` (`CharacterId`),
  KEY `FK_Estates_EconomicZones_idx` (`EconomicZoneId`),
  CONSTRAINT `FK_Estates_Characters` FOREIGN KEY (`CharacterId`) REFERENCES `characters` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_Estates_EconomicZones` FOREIGN KEY (`EconomicZoneId`) REFERENCES `economiczones` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Dumping data for table estates
--

/*!40000 ALTER TABLE `estates` DISABLE KEYS */;
/*!40000 ALTER TABLE `estates` ENABLE KEYS */;

--
-- Definition of estateassets
--

DROP TABLE IF EXISTS `estateassets`;
CREATE TABLE IF NOT EXISTS `estateassets` (
  `Id` bigint NOT NULL AUTO_INCREMENT,
  `EstateId` bigint NOT NULL,
  `FrameworkItemId` bigint NOT NULL,
  `FrameworkItemType` varchar(100) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `IsPresumedOwnership` bit(1) NOT NULL,
  `IsTransferred` bit(1) NOT NULL,
  `IsLiquidated` bit(1) NOT NULL,
  `LiquidatedValue` decimal(58,29) DEFAULT NULL,
  `OwnershipShare` decimal(58,29) NOT NULL DEFAULT '0.00000000000000000000000000000',
  PRIMARY KEY (`Id`),
  KEY `FK_EstateAssets_Estates_idx` (`EstateId`),
  CONSTRAINT `FK_EstateAssets_Estates` FOREIGN KEY (`EstateId`) REFERENCES `estates` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Dumping data for table estateassets
--

/*!40000 ALTER TABLE `estateassets` DISABLE KEYS */;
/*!40000 ALTER TABLE `estateassets` ENABLE KEYS */;

--
-- Definition of estateclaims
--

DROP TABLE IF EXISTS `estateclaims`;
CREATE TABLE IF NOT EXISTS `estateclaims` (
  `Id` bigint NOT NULL AUTO_INCREMENT,
  `EstateId` bigint NOT NULL,
  `ClaimantId` bigint NOT NULL,
  `ClaimantType` varchar(100) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `TargetId` bigint DEFAULT NULL,
  `TargetType` varchar(100) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci DEFAULT NULL,
  `Amount` decimal(58,29) NOT NULL,
  `Reason` text CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `ClaimStatus` int NOT NULL,
  `StatusReason` text CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci,
  `IsSecured` bit(1) NOT NULL,
  `ClaimDate` varchar(255) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  PRIMARY KEY (`Id`),
  KEY `FK_EstateClaims_Estates_idx` (`EstateId`),
  CONSTRAINT `FK_EstateClaims_Estates` FOREIGN KEY (`EstateId`) REFERENCES `estates` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Dumping data for table estateclaims
--

/*!40000 ALTER TABLE `estateclaims` DISABLE KEYS */;
/*!40000 ALTER TABLE `estateclaims` ENABLE KEYS */;

--
-- Definition of estatepayouts
--

DROP TABLE IF EXISTS `estatepayouts`;
CREATE TABLE IF NOT EXISTS `estatepayouts` (
  `Id` bigint NOT NULL AUTO_INCREMENT,
  `EstateId` bigint NOT NULL,
  `RecipientId` bigint NOT NULL,
  `RecipientType` varchar(100) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `Amount` decimal(58,29) NOT NULL,
  `Reason` text CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `CreatedDate` varchar(255) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `CollectedDate` varchar(255) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci DEFAULT NULL,
  PRIMARY KEY (`Id`),
  KEY `FK_EstatePayouts_Estates_idx` (`EstateId`),
  CONSTRAINT `FK_EstatePayouts_Estates` FOREIGN KEY (`EstateId`) REFERENCES `estates` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Dumping data for table estatepayouts
--

/*!40000 ALTER TABLE `estatepayouts` DISABLE KEYS */;
/*!40000 ALTER TABLE `estatepayouts` ENABLE KEYS */;

--
-- Definition of financialperiods
--

DROP TABLE IF EXISTS `financialperiods`;
CREATE TABLE IF NOT EXISTS `financialperiods` (
  `Id` bigint NOT NULL AUTO_INCREMENT,
  `EconomicZoneId` bigint NOT NULL,
  `PeriodStart` datetime NOT NULL,
  `PeriodEnd` datetime NOT NULL,
  `MudPeriodStart` varchar(200) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `MudPeriodEnd` varchar(200) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  PRIMARY KEY (`Id`),
  KEY `FK_FinancialPeriods_EconomicZones_idx` (`EconomicZoneId`),
  CONSTRAINT `FK_FinancialPeriods_EconomicZones` FOREIGN KEY (`EconomicZoneId`) REFERENCES `economiczones` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Dumping data for table financialperiods
--

/*!40000 ALTER TABLE `financialperiods` DISABLE KEYS */;
/*!40000 ALTER TABLE `financialperiods` ENABLE KEYS */;

--
-- Definition of economiczonerevenues
--

DROP TABLE IF EXISTS `economiczonerevenues`;
CREATE TABLE IF NOT EXISTS `economiczonerevenues` (
  `EconomicZoneId` bigint NOT NULL,
  `FinancialPeriodId` bigint NOT NULL,
  `TotalTaxRevenue` decimal(58,29) NOT NULL,
  PRIMARY KEY (`EconomicZoneId`,`FinancialPeriodId`),
  KEY `FK_EconomicZoneRevenues_FinancialPeriods_idx` (`FinancialPeriodId`),
  CONSTRAINT `FK_EconomicZoneRevenues` FOREIGN KEY (`EconomicZoneId`) REFERENCES `economiczones` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_EconomicZoneRevenues_FinancialPeriods` FOREIGN KEY (`FinancialPeriodId`) REFERENCES `financialperiods` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Dumping data for table economiczonerevenues
--

/*!40000 ALTER TABLE `economiczonerevenues` DISABLE KEYS */;
/*!40000 ALTER TABLE `economiczonerevenues` ENABLE KEYS */;

--
-- Definition of hospitals
--

DROP TABLE IF EXISTS `hospitals`;
CREATE TABLE IF NOT EXISTS `hospitals` (
  `Id` bigint NOT NULL AUTO_INCREMENT,
  `Name` varchar(200) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `EconomicZoneId` bigint NOT NULL,
  `BankAccountId` bigint DEFAULT NULL,
  `IsTrading` bit(1) NOT NULL DEFAULT b'1',
  `DefaultMaximumDebt` decimal(58,29) NOT NULL,
  PRIMARY KEY (`Id`),
  KEY `FK_Hospitals_BankAccounts_idx` (`BankAccountId`),
  KEY `FK_Hospitals_EconomicZones_idx` (`EconomicZoneId`),
  CONSTRAINT `FK_Hospitals_BankAccounts` FOREIGN KEY (`BankAccountId`) REFERENCES `bankaccounts` (`Id`) ON DELETE SET NULL,
  CONSTRAINT `FK_Hospitals_EconomicZones` FOREIGN KEY (`EconomicZoneId`) REFERENCES `economiczones` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Dumping data for table hospitals
--

/*!40000 ALTER TABLE `hospitals` DISABLE KEYS */;
/*!40000 ALTER TABLE `hospitals` ENABLE KEYS */;

--
-- Definition of hospitalbloodstockpolicies
--

DROP TABLE IF EXISTS `hospitalbloodstockpolicies`;
CREATE TABLE IF NOT EXISTS `hospitalbloodstockpolicies` (
  `Id` bigint NOT NULL AUTO_INCREMENT,
  `HospitalId` bigint NOT NULL,
  `BloodtypeId` bigint NOT NULL,
  `TargetLitres` double NOT NULL,
  `PricePerLitre` decimal(58,29) NOT NULL,
  PRIMARY KEY (`Id`),
  UNIQUE KEY `IX_HospitalBloodStockPolicies_Hospital_Bloodtype` (`HospitalId`,`BloodtypeId`),
  KEY `FK_HospitalBloodStockPolicies_Bloodtypes_idx` (`BloodtypeId`),
  KEY `FK_HospitalBloodStockPolicies_Hospitals_idx` (`HospitalId`),
  CONSTRAINT `FK_HospitalBloodStockPolicies_Bloodtypes` FOREIGN KEY (`BloodtypeId`) REFERENCES `bloodtypes` (`Id`) ON DELETE RESTRICT,
  CONSTRAINT `FK_HospitalBloodStockPolicies_Hospitals` FOREIGN KEY (`HospitalId`) REFERENCES `hospitals` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Dumping data for table hospitalbloodstockpolicies
--

/*!40000 ALTER TABLE `hospitalbloodstockpolicies` DISABLE KEYS */;
/*!40000 ALTER TABLE `hospitalbloodstockpolicies` ENABLE KEYS */;

--
-- Definition of hospitallocations
--

DROP TABLE IF EXISTS `hospitallocations`;
CREATE TABLE IF NOT EXISTS `hospitallocations` (
  `HospitalId` bigint NOT NULL,
  `CellId` bigint NOT NULL,
  `Role` int NOT NULL,
  PRIMARY KEY (`HospitalId`,`CellId`,`Role`),
  KEY `FK_HospitalLocations_Cells_idx` (`CellId`),
  KEY `IX_HospitalLocations_Hospital_Role` (`HospitalId`,`Role`),
  CONSTRAINT `FK_HospitalLocations_Cells` FOREIGN KEY (`CellId`) REFERENCES `cells` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_HospitalLocations_Hospitals` FOREIGN KEY (`HospitalId`) REFERENCES `hospitals` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Dumping data for table hospitallocations
--

/*!40000 ALTER TABLE `hospitallocations` DISABLE KEYS */;
/*!40000 ALTER TABLE `hospitallocations` ENABLE KEYS */;

--
-- Definition of hospitalpatientdebtaccounts
--

DROP TABLE IF EXISTS `hospitalpatientdebtaccounts`;
CREATE TABLE IF NOT EXISTS `hospitalpatientdebtaccounts` (
  `Id` bigint NOT NULL AUTO_INCREMENT,
  `HospitalId` bigint NOT NULL,
  `PatientId` bigint NOT NULL,
  `PatientName` mediumtext CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `Balance` decimal(58,29) NOT NULL,
  `MaximumDebt` decimal(58,29) NOT NULL,
  `IsSuspended` bit(1) NOT NULL,
  `LastUpdatedAtUtc` datetime(6) NOT NULL,
  PRIMARY KEY (`Id`),
  UNIQUE KEY `IX_HospitalPatientDebtAccounts_Hospital_Patient` (`HospitalId`,`PatientId`),
  KEY `FK_HospitalPatientDebtAccounts_Characters_idx` (`PatientId`),
  KEY `FK_HospitalPatientDebtAccounts_Hospitals_idx` (`HospitalId`),
  CONSTRAINT `FK_HospitalPatientDebtAccounts_Characters` FOREIGN KEY (`PatientId`) REFERENCES `characters` (`Id`) ON DELETE RESTRICT,
  CONSTRAINT `FK_HospitalPatientDebtAccounts_Hospitals` FOREIGN KEY (`HospitalId`) REFERENCES `hospitals` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Dumping data for table hospitalpatientdebtaccounts
--

/*!40000 ALTER TABLE `hospitalpatientdebtaccounts` DISABLE KEYS */;
/*!40000 ALTER TABLE `hospitalpatientdebtaccounts` ENABLE KEYS */;

--
-- Definition of jobfindinglocations
--

DROP TABLE IF EXISTS `jobfindinglocations`;
CREATE TABLE IF NOT EXISTS `jobfindinglocations` (
  `EconomicZoneId` bigint NOT NULL,
  `CellId` bigint NOT NULL,
  PRIMARY KEY (`EconomicZoneId`,`CellId`),
  KEY `IX_JobFindingLocations_CellId` (`CellId`),
  CONSTRAINT `FK_JobFindingLocations_Cells` FOREIGN KEY (`CellId`) REFERENCES `cells` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_JobFindingLocations_EconomicZones` FOREIGN KEY (`EconomicZoneId`) REFERENCES `economiczones` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Dumping data for table jobfindinglocations
--

/*!40000 ALTER TABLE `jobfindinglocations` DISABLE KEYS */;
/*!40000 ALTER TABLE `jobfindinglocations` ENABLE KEYS */;

--
-- Definition of markets
--

DROP TABLE IF EXISTS `markets`;
CREATE TABLE IF NOT EXISTS `markets` (
  `Id` bigint NOT NULL AUTO_INCREMENT,
  `Name` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  `Description` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  `EconomicZoneId` bigint NOT NULL,
  `MarketPriceFormula` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  PRIMARY KEY (`Id`),
  KEY `IX_Markets_EconomicZoneId` (`EconomicZoneId`),
  CONSTRAINT `FK_Markets_EconomicZones_EconomicZoneId` FOREIGN KEY (`EconomicZoneId`) REFERENCES `economiczones` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Dumping data for table markets
--

/*!40000 ALTER TABLE `markets` DISABLE KEYS */;
/*!40000 ALTER TABLE `markets` ENABLE KEYS */;

--
-- Definition of marketinfluences
--

DROP TABLE IF EXISTS `marketinfluences`;
CREATE TABLE IF NOT EXISTS `marketinfluences` (
  `Id` bigint NOT NULL AUTO_INCREMENT,
  `Name` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  `Description` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  `MarketId` bigint NOT NULL,
  `AppliesFrom` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  `AppliesUntil` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  `CharacterKnowsAboutInfluenceProgId` bigint NOT NULL,
  `MarketInfluenceTemplateId` bigint DEFAULT NULL,
  `Impacts` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  `PopulationImpacts` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  PRIMARY KEY (`Id`),
  KEY `IX_MarketInfluences_CharacterKnowsAboutInfluenceProgId` (`CharacterKnowsAboutInfluenceProgId`),
  KEY `IX_MarketInfluences_MarketId` (`MarketId`),
  KEY `IX_MarketInfluences_MarketInfluenceTemplateId` (`MarketInfluenceTemplateId`),
  CONSTRAINT `FK_MarketInfluences_FutureProgs_CharacterKnowsAboutInfluencePro~` FOREIGN KEY (`CharacterKnowsAboutInfluenceProgId`) REFERENCES `futureprogs` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_MarketInfluences_MarketInfluenceTemplates_MarketInfluenceTem~` FOREIGN KEY (`MarketInfluenceTemplateId`) REFERENCES `marketinfluencetemplates` (`Id`),
  CONSTRAINT `FK_MarketInfluences_Markets_MarketId` FOREIGN KEY (`MarketId`) REFERENCES `markets` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Dumping data for table marketinfluences
--

/*!40000 ALTER TABLE `marketinfluences` DISABLE KEYS */;
/*!40000 ALTER TABLE `marketinfluences` ENABLE KEYS */;

--
-- Definition of marketmarketcategory
--

DROP TABLE IF EXISTS `marketmarketcategory`;
CREATE TABLE IF NOT EXISTS `marketmarketcategory` (
  `MarketCategoriesId` bigint NOT NULL,
  `MarketsId` bigint NOT NULL,
  PRIMARY KEY (`MarketCategoriesId`,`MarketsId`),
  KEY `IX_MarketMarketCategory_MarketsId` (`MarketsId`),
  CONSTRAINT `FK_MarketMarketCategory_MarketCategories_MarketCategoriesId` FOREIGN KEY (`MarketCategoriesId`) REFERENCES `marketcategories` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_MarketMarketCategory_Markets_MarketsId` FOREIGN KEY (`MarketsId`) REFERENCES `markets` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Dumping data for table marketmarketcategory
--

/*!40000 ALTER TABLE `marketmarketcategory` DISABLE KEYS */;
/*!40000 ALTER TABLE `marketmarketcategory` ENABLE KEYS */;

--
-- Definition of marketpopulations
--

DROP TABLE IF EXISTS `marketpopulations`;
CREATE TABLE IF NOT EXISTS `marketpopulations` (
  `Id` bigint NOT NULL AUTO_INCREMENT,
  `Name` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  `Description` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  `PopulationScale` int NOT NULL,
  `MarketId` bigint NOT NULL,
  `MarketPopulationNeeds` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  `MarketStressPoints` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  `IncomeFactor` decimal(65,30) NOT NULL DEFAULT '1.000000000000000000000000000000',
  `Savings` decimal(65,30) NOT NULL DEFAULT '0.000000000000000000000000000000',
  `SavingsCap` decimal(65,30) NOT NULL DEFAULT '0.000000000000000000000000000000',
  `StressFlickerThreshold` decimal(65,30) NOT NULL DEFAULT '0.010000000000000000000000000000',
  PRIMARY KEY (`Id`),
  KEY `IX_MarketPopulations_MarketId` (`MarketId`),
  CONSTRAINT `FK_MarketPopulations_Markets_MarketId` FOREIGN KEY (`MarketId`) REFERENCES `markets` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Dumping data for table marketpopulations
--

/*!40000 ALTER TABLE `marketpopulations` DISABLE KEYS */;
/*!40000 ALTER TABLE `marketpopulations` ENABLE KEYS */;

--
-- Definition of probatelocations
--

DROP TABLE IF EXISTS `probatelocations`;
CREATE TABLE IF NOT EXISTS `probatelocations` (
  `EconomicZoneId` bigint NOT NULL,
  `CellId` bigint NOT NULL,
  PRIMARY KEY (`EconomicZoneId`,`CellId`),
  KEY `IX_ProbateLocations_CellId` (`CellId`),
  CONSTRAINT `FK_ProbateLocations_Cells` FOREIGN KEY (`CellId`) REFERENCES `cells` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_ProbateLocations_EconomicZones` FOREIGN KEY (`EconomicZoneId`) REFERENCES `economiczones` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Dumping data for table probatelocations
--

/*!40000 ALTER TABLE `probatelocations` DISABLE KEYS */;
/*!40000 ALTER TABLE `probatelocations` ENABLE KEYS */;

--
-- Definition of shoppers
--

DROP TABLE IF EXISTS `shoppers`;
CREATE TABLE IF NOT EXISTS `shoppers` (
  `Id` bigint NOT NULL AUTO_INCREMENT,
  `Name` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  `EconomicZoneId` bigint NOT NULL,
  `Interval` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  `NextDate` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  `Type` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  `Definition` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  PRIMARY KEY (`Id`),
  KEY `IX_Shoppers_EconomicZoneId` (`EconomicZoneId`),
  CONSTRAINT `FK_Shoppers_EconomicZones_EconomicZoneId` FOREIGN KEY (`EconomicZoneId`) REFERENCES `economiczones` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Dumping data for table shoppers
--

/*!40000 ALTER TABLE `shoppers` DISABLE KEYS */;
/*!40000 ALTER TABLE `shoppers` ENABLE KEYS */;

--
-- Definition of shopperlogs
--

DROP TABLE IF EXISTS `shopperlogs`;
CREATE TABLE IF NOT EXISTS `shopperlogs` (
  `Id` bigint NOT NULL AUTO_INCREMENT,
  `ShopperId` bigint NOT NULL,
  `DateTime` datetime(6) NOT NULL,
  `MudDateTime` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  `LogType` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  `LogEntry` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  PRIMARY KEY (`Id`),
  KEY `IX_ShopperLogs_ShopperId` (`ShopperId`),
  CONSTRAINT `FK_ShopperLogs_Shoppers_ShopperId` FOREIGN KEY (`ShopperId`) REFERENCES `shoppers` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Dumping data for table shopperlogs
--

/*!40000 ALTER TABLE `shopperlogs` DISABLE KEYS */;
/*!40000 ALTER TABLE `shopperlogs` ENABLE KEYS */;

--
-- Definition of shops
--

DROP TABLE IF EXISTS `shops`;
CREATE TABLE IF NOT EXISTS `shops` (
  `Id` bigint NOT NULL AUTO_INCREMENT,
  `Name` varchar(200) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `WorkshopCellId` bigint DEFAULT NULL,
  `StockroomCellId` bigint DEFAULT NULL,
  `CanShopProgId` bigint DEFAULT NULL,
  `WhyCannotShopProgId` bigint DEFAULT NULL,
  `CurrencyId` bigint NOT NULL,
  `IsTrading` bit(1) NOT NULL,
  `EconomicZoneId` bigint NOT NULL,
  `EmployeeRecords` mediumtext CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `BankAccountId` bigint DEFAULT NULL,
  `CashBalance` decimal(58,29) NOT NULL DEFAULT '0.00000000000000000000000000000',
  `ShopType` varchar(200) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL DEFAULT 'Permanent',
  `MinimumFloatToBuyItems` decimal(65,30) NOT NULL DEFAULT '0.000000000000000000000000000000',
  `MarketId` bigint DEFAULT NULL,
  `AutopayTaxes` bit(1) NOT NULL DEFAULT b'1',
  `ExpectedCashBalance` decimal(65,30) NOT NULL DEFAULT '0.000000000000000000000000000000',
  PRIMARY KEY (`Id`),
  KEY `FK_Shops_FutureProgs_Can_idx` (`CanShopProgId`),
  KEY `FK_Shops_Currencies_idx` (`CurrencyId`),
  KEY `FK_Shops_EconomicZonesa_idx` (`EconomicZoneId`),
  KEY `FK_Shops_Cells_Stockroom_idx` (`StockroomCellId`),
  KEY `FK_Shops_FutureProgs_WhyCant_idx` (`WhyCannotShopProgId`),
  KEY `FK_Shops_Cells_Workshop_idx` (`WorkshopCellId`),
  KEY `IX_Shops_BankAccountId` (`BankAccountId`),
  KEY `IX_Shops_MarketId` (`MarketId`),
  CONSTRAINT `FK_Shops_BankAccounts` FOREIGN KEY (`BankAccountId`) REFERENCES `bankaccounts` (`Id`) ON DELETE SET NULL,
  CONSTRAINT `FK_Shops_Cells_Stockroom` FOREIGN KEY (`StockroomCellId`) REFERENCES `cells` (`Id`) ON DELETE SET NULL,
  CONSTRAINT `FK_Shops_Cells_Workshop` FOREIGN KEY (`WorkshopCellId`) REFERENCES `cells` (`Id`) ON DELETE SET NULL,
  CONSTRAINT `FK_Shops_Currencies` FOREIGN KEY (`CurrencyId`) REFERENCES `currencies` (`Id`) ON DELETE RESTRICT,
  CONSTRAINT `FK_Shops_EconomicZones` FOREIGN KEY (`EconomicZoneId`) REFERENCES `economiczones` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_Shops_FutureProgs_Can` FOREIGN KEY (`CanShopProgId`) REFERENCES `futureprogs` (`Id`) ON DELETE SET NULL,
  CONSTRAINT `FK_Shops_FutureProgs_WhyCant` FOREIGN KEY (`WhyCannotShopProgId`) REFERENCES `futureprogs` (`Id`) ON DELETE SET NULL,
  CONSTRAINT `FK_Shops_Markets_MarketId` FOREIGN KEY (`MarketId`) REFERENCES `markets` (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Dumping data for table shops
--

/*!40000 ALTER TABLE `shops` DISABLE KEYS */;
/*!40000 ALTER TABLE `shops` ENABLE KEYS */;

--
-- Definition of bankaccounts
--

DROP TABLE IF EXISTS `bankaccounts`;
CREATE TABLE IF NOT EXISTS `bankaccounts` (
  `Id` bigint NOT NULL AUTO_INCREMENT,
  `Name` varchar(255) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `AccountNumber` int NOT NULL,
  `BankId` bigint NOT NULL,
  `BankAccountTypeId` bigint NOT NULL,
  `CurrentBalance` decimal(58,29) NOT NULL,
  `AccountOwnerCharacterId` bigint DEFAULT NULL,
  `AccountOwnerClanId` bigint DEFAULT NULL,
  `AccountOwnerShopId` bigint DEFAULT NULL,
  `NominatedBenefactorAccountId` bigint DEFAULT NULL,
  `AccountCreationDate` varchar(255) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `AccountStatus` int NOT NULL,
  `CurrentMonthInterest` decimal(58,29) NOT NULL,
  `CurrentMonthFees` decimal(58,29) NOT NULL,
  `AuthorisedBankPaymentItems` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  `AccountOwnerFrameworkItemId` bigint DEFAULT NULL,
  `AccountOwnerFrameworkItemType` varchar(100) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci DEFAULT NULL,
  PRIMARY KEY (`Id`),
  KEY `IX_BankAccounts_AccountOwnerCharacterId` (`AccountOwnerCharacterId`),
  KEY `IX_BankAccounts_AccountOwnerClanId` (`AccountOwnerClanId`),
  KEY `IX_BankAccounts_AccountOwnerShopId` (`AccountOwnerShopId`),
  KEY `IX_BankAccounts_BankAccountTypeId` (`BankAccountTypeId`),
  KEY `IX_BankAccounts_BankId` (`BankId`),
  KEY `IX_BankAccounts_NominatedBenefactorAccountId` (`NominatedBenefactorAccountId`),
  CONSTRAINT `FK_BankAccounts_BankAccounts` FOREIGN KEY (`NominatedBenefactorAccountId`) REFERENCES `bankaccounts` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_BankAccounts_BankAccountTypes` FOREIGN KEY (`BankAccountTypeId`) REFERENCES `bankaccounttypes` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_BankAccounts_Banks` FOREIGN KEY (`BankId`) REFERENCES `banks` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_BankAccounts_Characters` FOREIGN KEY (`AccountOwnerCharacterId`) REFERENCES `characters` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_BankAccounts_Clans` FOREIGN KEY (`AccountOwnerClanId`) REFERENCES `clans` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_BankAccounts_Shops` FOREIGN KEY (`AccountOwnerShopId`) REFERENCES `shops` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Dumping data for table bankaccounts
--

/*!40000 ALTER TABLE `bankaccounts` DISABLE KEYS */;
/*!40000 ALTER TABLE `bankaccounts` ENABLE KEYS */;

--
-- Definition of bankaccounttransactions
--

DROP TABLE IF EXISTS `bankaccounttransactions`;
CREATE TABLE IF NOT EXISTS `bankaccounttransactions` (
  `Id` bigint NOT NULL AUTO_INCREMENT,
  `BankAccountId` bigint NOT NULL,
  `TransactionType` int NOT NULL,
  `Amount` decimal(58,29) NOT NULL,
  `TransactionTime` varchar(255) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `TransactionDescription` varchar(255) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `AccountBalanceAfter` decimal(58,29) NOT NULL,
  PRIMARY KEY (`Id`),
  KEY `IX_BankAccountTransactions_BankAccountId` (`BankAccountId`),
  CONSTRAINT `FK_BankAccountTransactions_BankAccounts` FOREIGN KEY (`BankAccountId`) REFERENCES `bankaccounts` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Dumping data for table bankaccounttransactions
--

/*!40000 ALTER TABLE `bankaccounttransactions` DISABLE KEYS */;
/*!40000 ALTER TABLE `bankaccounttransactions` ENABLE KEYS */;

--
-- Definition of economiczoneshoptaxes
--

DROP TABLE IF EXISTS `economiczoneshoptaxes`;
CREATE TABLE IF NOT EXISTS `economiczoneshoptaxes` (
  `EconomicZoneId` bigint NOT NULL,
  `ShopId` bigint NOT NULL,
  `OutstandingProfitTaxes` decimal(58,29) NOT NULL,
  `OutstandingSalesTaxes` decimal(58,29) NOT NULL,
  `TaxesInCredits` decimal(58,29) NOT NULL,
  PRIMARY KEY (`EconomicZoneId`,`ShopId`),
  KEY `FK_EconomicZoneShopTaxes_Shops_idx` (`ShopId`),
  CONSTRAINT `FK_EconomicZoneShopTaxes_EconomicZones` FOREIGN KEY (`EconomicZoneId`) REFERENCES `economiczones` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_EconomicZoneShopTaxes_Shops` FOREIGN KEY (`ShopId`) REFERENCES `shops` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Dumping data for table economiczoneshoptaxes
--

/*!40000 ALTER TABLE `economiczoneshoptaxes` DISABLE KEYS */;
/*!40000 ALTER TABLE `economiczoneshoptaxes` ENABLE KEYS */;

--
-- Definition of lineofcreditaccounts
--

DROP TABLE IF EXISTS `lineofcreditaccounts`;
CREATE TABLE IF NOT EXISTS `lineofcreditaccounts` (
  `Id` bigint NOT NULL AUTO_INCREMENT,
  `AccountName` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  `ShopId` bigint NOT NULL,
  `IsSuspended` bit(1) NOT NULL,
  `AccountLimit` decimal(58,29) NOT NULL,
  `OutstandingBalance` decimal(58,29) NOT NULL,
  `AccountOwnerId` bigint NOT NULL,
  `AccountOwnerName` text CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci,
  PRIMARY KEY (`Id`),
  KEY `FK_LineOfCreditAccounts_Characters_idx` (`AccountOwnerId`),
  KEY `FK_LineOfCreditAccounts_Shops_idx` (`ShopId`),
  CONSTRAINT `FK_LineOfCreditAccounts_Characters` FOREIGN KEY (`AccountOwnerId`) REFERENCES `characters` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_LineOfCreditAccounts_Shops` FOREIGN KEY (`ShopId`) REFERENCES `shops` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Dumping data for table lineofcreditaccounts
--

/*!40000 ALTER TABLE `lineofcreditaccounts` DISABLE KEYS */;
/*!40000 ALTER TABLE `lineofcreditaccounts` ENABLE KEYS */;

--
-- Definition of lineofcreditaccountusers
--

DROP TABLE IF EXISTS `lineofcreditaccountusers`;
CREATE TABLE IF NOT EXISTS `lineofcreditaccountusers` (
  `LineOfCreditAccountId` bigint NOT NULL,
  `AccountUserId` bigint NOT NULL,
  `AccountUserName` text CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci,
  `SpendingLimit` decimal(58,29) DEFAULT NULL,
  PRIMARY KEY (`LineOfCreditAccountId`,`AccountUserId`),
  KEY `FK_LineOfCreditAccountUsers_Characters_idx` (`AccountUserId`),
  KEY `FK_LineOfCreditAccountUsers_LineOfCreditAccounts_idx` (`LineOfCreditAccountId`),
  CONSTRAINT `FK_LineOfCreditAccountUsers_Characters` FOREIGN KEY (`AccountUserId`) REFERENCES `characters` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_LineOfCreditAccountUsers_LineOfCreditAccounts` FOREIGN KEY (`LineOfCreditAccountId`) REFERENCES `lineofcreditaccounts` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Dumping data for table lineofcreditaccountusers
--

/*!40000 ALTER TABLE `lineofcreditaccountusers` DISABLE KEYS */;
/*!40000 ALTER TABLE `lineofcreditaccountusers` ENABLE KEYS */;

--
-- Definition of merchandises
--

DROP TABLE IF EXISTS `merchandises`;
CREATE TABLE IF NOT EXISTS `merchandises` (
  `Id` bigint NOT NULL AUTO_INCREMENT,
  `Name` varchar(1000) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `ShopId` bigint NOT NULL,
  `AutoReordering` bit(1) NOT NULL,
  `AutoReorderPrice` decimal(58,29) NOT NULL,
  `BasePrice` decimal(58,29) NOT NULL,
  `DefaultMerchandiseForItem` bit(1) NOT NULL,
  `ItemProtoId` bigint NOT NULL,
  `PreferredDisplayContainerId` bigint DEFAULT NULL,
  `ListDescription` varchar(500) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci DEFAULT NULL,
  `MinimumStockLevels` int NOT NULL,
  `MinimumStockLevelsByWeight` double NOT NULL,
  `PreserveVariablesOnReorder` bit(1) NOT NULL,
  `SkinId` bigint DEFAULT NULL,
  `BaseBuyModifier` decimal(58,29) NOT NULL DEFAULT '0.30000000000000000000000000000',
  `MaximumStockLevelsToBuy` int NOT NULL DEFAULT '0',
  `MinimumConditionToBuy` double NOT NULL DEFAULT '0.95',
  `WillBuy` bit(1) NOT NULL DEFAULT b'0',
  `WillSell` bit(1) NOT NULL DEFAULT b'1',
  `IgnoreMarketPricing` tinyint(1) NOT NULL DEFAULT '0',
  `PermitItemDecayOnStockedItems` tinyint(1) NOT NULL DEFAULT '0',
  `SalesMarkupMultiplier` decimal(65,30) DEFAULT NULL,
  `CommodityCharacteristics` text CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci,
  `CommodityMaterialId` bigint DEFAULT NULL,
  `CommodityPricingWeight` double NOT NULL DEFAULT '1',
  `CommodityTagId` bigint DEFAULT NULL,
  `MerchandiseType` int NOT NULL DEFAULT '0',
  PRIMARY KEY (`Id`),
  KEY `FK_Merchandises_GameItems_idx` (`PreferredDisplayContainerId`),
  KEY `FK_Merchandises_Shops_idx` (`ShopId`),
  CONSTRAINT `FK_Merchandises_GameItems` FOREIGN KEY (`PreferredDisplayContainerId`) REFERENCES `gameitems` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_Merchandises_Shops` FOREIGN KEY (`ShopId`) REFERENCES `shops` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Dumping data for table merchandises
--

/*!40000 ALTER TABLE `merchandises` DISABLE KEYS */;
/*!40000 ALTER TABLE `merchandises` ENABLE KEYS */;

--
-- Definition of shopfinancialperiodresults
--

DROP TABLE IF EXISTS `shopfinancialperiodresults`;
CREATE TABLE IF NOT EXISTS `shopfinancialperiodresults` (
  `EconomicZoneId` bigint NOT NULL,
  `ShopId` bigint NOT NULL,
  `FinancialPeriodId` bigint NOT NULL,
  `GrossRevenue` decimal(58,29) NOT NULL,
  `NetRevenue` decimal(58,29) NOT NULL,
  `SalesTax` decimal(58,29) NOT NULL,
  `ProfitsTax` decimal(58,29) NOT NULL,
  PRIMARY KEY (`EconomicZoneId`,`ShopId`,`FinancialPeriodId`),
  KEY `FK_ShopFinancialPeriodResults_FinancialPeriods_idx` (`FinancialPeriodId`),
  KEY `FK_ShopFinancialPeriodResults_Shops_idx` (`ShopId`),
  CONSTRAINT `FK_ShopFinancialPeriodResults_EconomicZones` FOREIGN KEY (`EconomicZoneId`) REFERENCES `economiczones` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_ShopFinancialPeriodResults_FinancialPeriods` FOREIGN KEY (`FinancialPeriodId`) REFERENCES `financialperiods` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_ShopFinancialPeriodResults_Shops` FOREIGN KEY (`ShopId`) REFERENCES `shops` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Dumping data for table shopfinancialperiodresults
--

/*!40000 ALTER TABLE `shopfinancialperiodresults` DISABLE KEYS */;
/*!40000 ALTER TABLE `shopfinancialperiodresults` ENABLE KEYS */;

--
-- Definition of shops_storeroomcells
--

DROP TABLE IF EXISTS `shops_storeroomcells`;
CREATE TABLE IF NOT EXISTS `shops_storeroomcells` (
  `ShopId` bigint NOT NULL,
  `CellId` bigint NOT NULL,
  PRIMARY KEY (`ShopId`,`CellId`),
  KEY `FK_Shops_StoreroomCells_Cells_idx` (`CellId`),
  CONSTRAINT `FK_Shops_StoreroomCells_Cells` FOREIGN KEY (`CellId`) REFERENCES `cells` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_Shops_StoreroomCells_Shops` FOREIGN KEY (`ShopId`) REFERENCES `shops` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Dumping data for table shops_storeroomcells
--

/*!40000 ALTER TABLE `shops_storeroomcells` DISABLE KEYS */;
/*!40000 ALTER TABLE `shops_storeroomcells` ENABLE KEYS */;

--
-- Definition of shopstills
--

DROP TABLE IF EXISTS `shopstills`;
CREATE TABLE IF NOT EXISTS `shopstills` (
  `ShopId` bigint NOT NULL,
  `GameItemId` bigint NOT NULL,
  PRIMARY KEY (`ShopId`,`GameItemId`),
  KEY `FK_ShopTills_GameItems_idx` (`GameItemId`),
  CONSTRAINT `FK_ShopTills_GameItems` FOREIGN KEY (`GameItemId`) REFERENCES `gameitems` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_ShopTills_Shops` FOREIGN KEY (`ShopId`) REFERENCES `shops` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Dumping data for table shopstills
--

/*!40000 ALTER TABLE `shopstills` DISABLE KEYS */;
/*!40000 ALTER TABLE `shopstills` ENABLE KEYS */;

--
-- Definition of shoptransactionrecords
--

DROP TABLE IF EXISTS `shoptransactionrecords`;
CREATE TABLE IF NOT EXISTS `shoptransactionrecords` (
  `Id` bigint NOT NULL AUTO_INCREMENT,
  `CurrencyId` bigint NOT NULL,
  `PretaxValue` decimal(58,29) NOT NULL,
  `Tax` decimal(58,29) NOT NULL,
  `TransactionType` int NOT NULL,
  `ShopId` bigint NOT NULL,
  `ThirdPartyId` bigint DEFAULT NULL,
  `RealDateTime` datetime NOT NULL,
  `MudDateTime` varchar(500) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `MerchandiseId` bigint DEFAULT NULL,
  PRIMARY KEY (`Id`),
  KEY `FK_ShopTransactionRecords_Currencies_idx` (`CurrencyId`),
  KEY `FK_ShopTransactionRecords_Shops_idx` (`ShopId`),
  KEY `IX_ShopTransactionRecords_MerchandiseId` (`MerchandiseId`),
  CONSTRAINT `FK_ShopTransactionRecords_Currencies` FOREIGN KEY (`CurrencyId`) REFERENCES `currencies` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_ShopTransactionRecords_Merchandises` FOREIGN KEY (`MerchandiseId`) REFERENCES `merchandises` (`Id`) ON DELETE SET NULL,
  CONSTRAINT `FK_ShopTransactionRecords_Shops` FOREIGN KEY (`ShopId`) REFERENCES `shops` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Dumping data for table shoptransactionrecords
--

/*!40000 ALTER TABLE `shoptransactionrecords` DISABLE KEYS */;
/*!40000 ALTER TABLE `shoptransactionrecords` ENABLE KEYS */;

--
-- Definition of traitdecorators
--

DROP TABLE IF EXISTS `traitdecorators`;
CREATE TABLE IF NOT EXISTS `traitdecorators` (
  `Id` bigint NOT NULL AUTO_INCREMENT,
  `Name` varchar(255) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `Type` varchar(255) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `Contents` text CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Dumping data for table traitdecorators
--

/*!40000 ALTER TABLE `traitdecorators` DISABLE KEYS */;
/*!40000 ALTER TABLE `traitdecorators` ENABLE KEYS */;

--
-- Definition of traitexpression
--

DROP TABLE IF EXISTS `traitexpression`;
CREATE TABLE IF NOT EXISTS `traitexpression` (
  `Id` bigint NOT NULL AUTO_INCREMENT,
  `Expression` varchar(4000) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `Name` varchar(200) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL DEFAULT 'Unnamed Expression',
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Dumping data for table traitexpression
--

/*!40000 ALTER TABLE `traitexpression` DISABLE KEYS */;
/*!40000 ALTER TABLE `traitexpression` ENABLE KEYS */;

--
-- Definition of checks
--

DROP TABLE IF EXISTS `checks`;
CREATE TABLE IF NOT EXISTS `checks` (
  `Type` int NOT NULL,
  `TraitExpressionId` bigint NOT NULL,
  `CheckTemplateId` bigint NOT NULL,
  `MaximumDifficultyForImprovement` int NOT NULL DEFAULT '10',
  PRIMARY KEY (`Type`),
  KEY `FK_Checks_CheckTemplates` (`CheckTemplateId`),
  KEY `FK_Checks_TraitExpression` (`TraitExpressionId`),
  CONSTRAINT `FK_Checks_CheckTemplates` FOREIGN KEY (`CheckTemplateId`) REFERENCES `checktemplates` (`Id`) ON DELETE RESTRICT,
  CONSTRAINT `FK_Checks_TraitExpression` FOREIGN KEY (`TraitExpressionId`) REFERENCES `traitexpression` (`Id`) ON DELETE RESTRICT
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Dumping data for table checks
--

/*!40000 ALTER TABLE `checks` DISABLE KEYS */;
/*!40000 ALTER TABLE `checks` ENABLE KEYS */;

--
-- Definition of magicspells
--

DROP TABLE IF EXISTS `magicspells`;
CREATE TABLE IF NOT EXISTS `magicspells` (
  `Id` bigint NOT NULL AUTO_INCREMENT,
  `Name` varchar(100) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `Blurb` text CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `Description` text CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `SpellKnownProgId` bigint NOT NULL,
  `MagicSchoolId` bigint NOT NULL,
  `ExclusiveDelay` double NOT NULL,
  `NonExclusiveDelay` double NOT NULL,
  `Definition` mediumtext CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `CastingDifficulty` int NOT NULL DEFAULT '0',
  `CastingTraitDefinitionId` bigint DEFAULT NULL,
  `ResistingDifficulty` int DEFAULT NULL,
  `ResistingTraitDefinitionId` bigint DEFAULT NULL,
  `CastingEmote` text CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci,
  `CastingEmoteFlags` int NOT NULL DEFAULT '0',
  `EffectDurationExpressionId` bigint DEFAULT NULL,
  `FailCastingEmote` text CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci,
  `TargetEmote` text CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci,
  `TargetEmoteFlags` int NOT NULL DEFAULT '0',
  `TargetResistedEmote` text CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci,
  `MinimumSuccessThreshold` int NOT NULL DEFAULT '4',
  `AppliedEffectsAreExclusive` tinyint(1) NOT NULL DEFAULT '0',
  `TargetNullEmote` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  PRIMARY KEY (`Id`),
  KEY `FK_MagicSpells_Futureprogs_idx` (`SpellKnownProgId`),
  KEY `FK_MagicSpells_MagicSchools_idx` (`MagicSchoolId`),
  KEY `FK_MagicSpells_TraitDefinitions_Casting_idx` (`CastingTraitDefinitionId`),
  KEY `FK_MagicSpells_TraitDefinitions_Resisting_idx` (`ResistingTraitDefinitionId`),
  KEY `FK_MagicSpells_TraitExpressions_idx` (`EffectDurationExpressionId`),
  CONSTRAINT `FK_MagicSpells_Futureprogs` FOREIGN KEY (`SpellKnownProgId`) REFERENCES `futureprogs` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_MagicSpells_MagicSchools` FOREIGN KEY (`MagicSchoolId`) REFERENCES `magicschools` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_MagicSpells_TraitDefinitions_Casting` FOREIGN KEY (`CastingTraitDefinitionId`) REFERENCES `traitdefinitions` (`Id`) ON DELETE RESTRICT,
  CONSTRAINT `FK_MagicSpells_TraitDefinitions_Resisting` FOREIGN KEY (`ResistingTraitDefinitionId`) REFERENCES `traitdefinitions` (`Id`) ON DELETE RESTRICT,
  CONSTRAINT `FK_MagicSpells_TraitExpressions` FOREIGN KEY (`EffectDurationExpressionId`) REFERENCES `traitexpression` (`Id`) ON DELETE RESTRICT
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Dumping data for table magicspells
--

/*!40000 ALTER TABLE `magicspells` DISABLE KEYS */;
/*!40000 ALTER TABLE `magicspells` ENABLE KEYS */;

--
-- Definition of magicportalendpoints
--

DROP TABLE IF EXISTS `magicportalendpoints`;
CREATE TABLE IF NOT EXISTS `magicportalendpoints` (
  `Id` bigint NOT NULL AUTO_INCREMENT,
  `MagicPortalNetworkId` bigint NOT NULL,
  `Key` varchar(100) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `Name` varchar(200) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `AnchorType` int NOT NULL,
  `CellId` bigint DEFAULT NULL,
  `GameItemId` bigint DEFAULT NULL,
  `IsActive` bit(1) NOT NULL DEFAULT b'1',
  `CreatedByCharacterId` bigint DEFAULT NULL,
  `CreatedBySpellId` bigint DEFAULT NULL,
  `CreatedDateTime` datetime NOT NULL,
  PRIMARY KEY (`Id`),
  UNIQUE KEY `IX_MagicPortalEndpoints_Network_Key` (`MagicPortalNetworkId`,`Key`),
  KEY `FK_MagicPortalEndpoints_Cells_idx` (`CellId`),
  KEY `FK_MagicPortalEndpoints_Characters_idx` (`CreatedByCharacterId`),
  KEY `FK_MagicPortalEndpoints_GameItems_idx` (`GameItemId`),
  KEY `FK_MagicPortalEndpoints_MagicSpells_idx` (`CreatedBySpellId`),
  CONSTRAINT `FK_MagicPortalEndpoints_Cells` FOREIGN KEY (`CellId`) REFERENCES `cells` (`Id`) ON DELETE SET NULL,
  CONSTRAINT `FK_MagicPortalEndpoints_Characters` FOREIGN KEY (`CreatedByCharacterId`) REFERENCES `characters` (`Id`) ON DELETE SET NULL,
  CONSTRAINT `FK_MagicPortalEndpoints_GameItems` FOREIGN KEY (`GameItemId`) REFERENCES `gameitems` (`Id`) ON DELETE SET NULL,
  CONSTRAINT `FK_MagicPortalEndpoints_MagicPortalNetworks` FOREIGN KEY (`MagicPortalNetworkId`) REFERENCES `magicportalnetworks` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_MagicPortalEndpoints_MagicSpells` FOREIGN KEY (`CreatedBySpellId`) REFERENCES `magicspells` (`Id`) ON DELETE SET NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Dumping data for table magicportalendpoints
--

/*!40000 ALTER TABLE `magicportalendpoints` DISABLE KEYS */;
/*!40000 ALTER TABLE `magicportalendpoints` ENABLE KEYS */;

--
-- Definition of magicportallinks
--

DROP TABLE IF EXISTS `magicportallinks`;
CREATE TABLE IF NOT EXISTS `magicportallinks` (
  `Id` bigint NOT NULL AUTO_INCREMENT,
  `MagicPortalNetworkId` bigint NOT NULL,
  `SourceEndpointId` bigint NOT NULL,
  `DestinationEndpointId` bigint NOT NULL,
  `IsActive` bit(1) NOT NULL DEFAULT b'1',
  `CreatedByCharacterId` bigint DEFAULT NULL,
  `CreatedBySpellId` bigint DEFAULT NULL,
  `CreatedDateTime` datetime NOT NULL,
  PRIMARY KEY (`Id`),
  UNIQUE KEY `IX_MagicPortalLinks_Network_Source_Destination` (`MagicPortalNetworkId`,`SourceEndpointId`,`DestinationEndpointId`),
  KEY `FK_MagicPortalLinks_Characters_idx` (`CreatedByCharacterId`),
  KEY `FK_MagicPortalLinks_DestinationEndpoints_idx` (`DestinationEndpointId`),
  KEY `FK_MagicPortalLinks_MagicSpells_idx` (`CreatedBySpellId`),
  KEY `FK_MagicPortalLinks_SourceEndpoints_idx` (`SourceEndpointId`),
  CONSTRAINT `FK_MagicPortalLinks_Characters` FOREIGN KEY (`CreatedByCharacterId`) REFERENCES `characters` (`Id`) ON DELETE SET NULL,
  CONSTRAINT `FK_MagicPortalLinks_DestinationEndpoints` FOREIGN KEY (`DestinationEndpointId`) REFERENCES `magicportalendpoints` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_MagicPortalLinks_MagicPortalNetworks` FOREIGN KEY (`MagicPortalNetworkId`) REFERENCES `magicportalnetworks` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_MagicPortalLinks_MagicSpells` FOREIGN KEY (`CreatedBySpellId`) REFERENCES `magicspells` (`Id`) ON DELETE SET NULL,
  CONSTRAINT `FK_MagicPortalLinks_SourceEndpoints` FOREIGN KEY (`SourceEndpointId`) REFERENCES `magicportalendpoints` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Dumping data for table magicportallinks
--

/*!40000 ALTER TABLE `magicportallinks` DISABLE KEYS */;
/*!40000 ALTER TABLE `magicportallinks` ENABLE KEYS */;

--
-- Definition of magicportalnetworks
--

DROP TABLE IF EXISTS `magicportalnetworks`;
CREATE TABLE IF NOT EXISTS `magicportalnetworks` (
  `Id` bigint NOT NULL AUTO_INCREMENT,
  `Name` varchar(200) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `MagicSchoolId` bigint DEFAULT NULL,
  `IsActive` bit(1) NOT NULL DEFAULT b'1',
  `AllowCrossZone` bit(1) NOT NULL,
  `Verb` varchar(100) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `OutboundKeyword` varchar(100) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `InboundKeyword` varchar(100) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `OutboundTarget` varchar(500) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `InboundTarget` varchar(500) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `OutboundDescription` varchar(100) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `InboundDescription` varchar(100) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `TimeMultiplier` double NOT NULL DEFAULT '1',
  `CreatedByCharacterId` bigint DEFAULT NULL,
  `CreatedBySpellId` bigint DEFAULT NULL,
  `CreatedDateTime` datetime NOT NULL,
  PRIMARY KEY (`Id`),
  KEY `FK_MagicPortalNetworks_Characters_idx` (`CreatedByCharacterId`),
  KEY `FK_MagicPortalNetworks_MagicSchools_idx` (`MagicSchoolId`),
  KEY `FK_MagicPortalNetworks_MagicSpells_idx` (`CreatedBySpellId`),
  CONSTRAINT `FK_MagicPortalNetworks_Characters` FOREIGN KEY (`CreatedByCharacterId`) REFERENCES `characters` (`Id`) ON DELETE SET NULL,
  CONSTRAINT `FK_MagicPortalNetworks_MagicSchools` FOREIGN KEY (`MagicSchoolId`) REFERENCES `magicschools` (`Id`) ON DELETE SET NULL,
  CONSTRAINT `FK_MagicPortalNetworks_MagicSpells` FOREIGN KEY (`CreatedBySpellId`) REFERENCES `magicspells` (`Id`) ON DELETE SET NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Dumping data for table magicportalnetworks
--

/*!40000 ALTER TABLE `magicportalnetworks` DISABLE KEYS */;
/*!40000 ALTER TABLE `magicportalnetworks` ENABLE KEYS */;

--
-- Definition of traitdefinitions
--

DROP TABLE IF EXISTS `traitdefinitions`;
CREATE TABLE IF NOT EXISTS `traitdefinitions` (
  `Id` bigint NOT NULL AUTO_INCREMENT,
  `Name` varchar(255) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `Type` int NOT NULL,
  `DecoratorId` bigint NOT NULL,
  `TraitGroup` varchar(255) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `DerivedType` int NOT NULL,
  `ExpressionId` bigint DEFAULT NULL,
  `ImproverId` bigint DEFAULT NULL,
  `Hidden` bit(1) DEFAULT b'0',
  `ChargenBlurb` varchar(4000) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci DEFAULT NULL,
  `BranchMultiplier` double NOT NULL DEFAULT '1',
  `Alias` varchar(255) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci DEFAULT NULL,
  `AvailabilityProgId` bigint DEFAULT NULL,
  `TeachableProgId` bigint DEFAULT NULL,
  `LearnableProgId` bigint DEFAULT NULL,
  `TeachDifficulty` int NOT NULL DEFAULT '7',
  `LearnDifficulty` int NOT NULL DEFAULT '7',
  `ValueExpression` text CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci,
  `DisplayAsSubAttribute` bit(1) NOT NULL DEFAULT b'0',
  `DisplayOrder` int NOT NULL DEFAULT '1',
  `ShowInAttributeCommand` bit(1) NOT NULL DEFAULT b'1',
  `ShowInScoreCommand` bit(1) NOT NULL DEFAULT b'1',
  `OwnerScope` int NOT NULL DEFAULT '0',
  PRIMARY KEY (`Id`),
  KEY `FK_TraitDefinitions_AvailabilityProg` (`AvailabilityProgId`),
  KEY `FK_TraitDefinitions_TraitExpression` (`ExpressionId`),
  KEY `FK_TraitDefinitions_LearnableProg_idx` (`LearnableProgId`),
  KEY `FK_TraitDefinitions_TeachableProg_idx` (`TeachableProgId`),
  CONSTRAINT `FK_TraitDefinitions_AvailabilityProg` FOREIGN KEY (`AvailabilityProgId`) REFERENCES `futureprogs` (`Id`) ON DELETE SET NULL,
  CONSTRAINT `FK_TraitDefinitions_LearnableProg` FOREIGN KEY (`LearnableProgId`) REFERENCES `futureprogs` (`Id`) ON DELETE SET NULL,
  CONSTRAINT `FK_TraitDefinitions_TeachableProg` FOREIGN KEY (`TeachableProgId`) REFERENCES `futureprogs` (`Id`) ON DELETE SET NULL,
  CONSTRAINT `FK_TraitDefinitions_TraitExpression` FOREIGN KEY (`ExpressionId`) REFERENCES `traitexpression` (`Id`) ON DELETE RESTRICT
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Dumping data for table traitdefinitions
--

/*!40000 ALTER TABLE `traitdefinitions` DISABLE KEYS */;
/*!40000 ALTER TABLE `traitdefinitions` ENABLE KEYS */;

--
-- Definition of charactertraits
--

DROP TABLE IF EXISTS `charactertraits`;
CREATE TABLE IF NOT EXISTS `charactertraits` (
  `CharacterId` bigint NOT NULL,
  `TraitDefinitionId` bigint NOT NULL,
  `Value` double NOT NULL,
  `AdditionalValue` double NOT NULL,
  PRIMARY KEY (`CharacterId`,`TraitDefinitionId`),
  KEY `FK_CharacterTraits_TraitDefinitions` (`TraitDefinitionId`),
  CONSTRAINT `FK_CharacterTraits_Characters` FOREIGN KEY (`CharacterId`) REFERENCES `characters` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_CharacterTraits_TraitDefinitions` FOREIGN KEY (`TraitDefinitionId`) REFERENCES `traitdefinitions` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Dumping data for table charactertraits
--

/*!40000 ALTER TABLE `charactertraits` DISABLE KEYS */;
/*!40000 ALTER TABLE `charactertraits` ENABLE KEYS */;

--
-- Definition of chargenroles_traits
--

DROP TABLE IF EXISTS `chargenroles_traits`;
CREATE TABLE IF NOT EXISTS `chargenroles_traits` (
  `ChargenRoleId` bigint NOT NULL,
  `TraitId` bigint NOT NULL,
  `Amount` double NOT NULL,
  `GiveIfDoesntHave` bit(1) NOT NULL DEFAULT b'0',
  PRIMARY KEY (`ChargenRoleId`,`TraitId`),
  KEY `FK_ChargenRoles_Traits_Currencies` (`TraitId`),
  CONSTRAINT `FK_ChargenRoles_Traits_ChargenRoles` FOREIGN KEY (`ChargenRoleId`) REFERENCES `chargenroles` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_ChargenRoles_Traits_Currencies` FOREIGN KEY (`TraitId`) REFERENCES `traitdefinitions` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Dumping data for table chargenroles_traits
--

/*!40000 ALTER TABLE `chargenroles_traits` DISABLE KEYS */;
/*!40000 ALTER TABLE `chargenroles_traits` ENABLE KEYS */;

--
-- Definition of combatactions
--

DROP TABLE IF EXISTS `combatactions`;
CREATE TABLE IF NOT EXISTS `combatactions` (
  `Id` bigint NOT NULL AUTO_INCREMENT,
  `Name` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  `UsabilityProgId` bigint DEFAULT NULL,
  `RecoveryDifficultySuccess` int NOT NULL,
  `RecoveryDifficultyFailure` int NOT NULL,
  `MoveType` int NOT NULL,
  `Intentions` bigint NOT NULL,
  `ExertionLevel` int NOT NULL,
  `Weighting` double NOT NULL,
  `StaminaCost` double NOT NULL,
  `BaseDelay` double NOT NULL,
  `AdditionalInfo` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  `RequiredPositionStateIds` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  `MoveDifficulty` int NOT NULL,
  `TraitDefinitionId` bigint NOT NULL,
  `MaximumTargets` int NOT NULL DEFAULT '1',
  PRIMARY KEY (`Id`),
  KEY `IX_CombatActions_TraitDefinitionId` (`TraitDefinitionId`),
  KEY `IX_CombatActions_UsabilityProgId` (`UsabilityProgId`),
  CONSTRAINT `FK_CombatActions_FutureProgs_UsabilityProgId` FOREIGN KEY (`UsabilityProgId`) REFERENCES `futureprogs` (`Id`),
  CONSTRAINT `FK_CombatActions_TraitDefinitions_TraitDefinitionId` FOREIGN KEY (`TraitDefinitionId`) REFERENCES `traitdefinitions` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Dumping data for table combatactions
--

/*!40000 ALTER TABLE `combatactions` DISABLE KEYS */;
/*!40000 ALTER TABLE `combatactions` ENABLE KEYS */;

--
-- Definition of combatmessages_combatactions
--

DROP TABLE IF EXISTS `combatmessages_combatactions`;
CREATE TABLE IF NOT EXISTS `combatmessages_combatactions` (
  `CombatMessageId` bigint NOT NULL,
  `CombatActionId` bigint NOT NULL,
  PRIMARY KEY (`CombatMessageId`,`CombatActionId`),
  KEY `FK_CombatMessages_CombatActions_WeaponAttacks_idx` (`CombatActionId`),
  CONSTRAINT `FK_CombatMessages_CombatActions_CombatMessages` FOREIGN KEY (`CombatMessageId`) REFERENCES `combatmessages` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_CombatMessages_CombatActions_WeaponAttacks` FOREIGN KEY (`CombatActionId`) REFERENCES `combatactions` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Dumping data for table combatmessages_combatactions
--

/*!40000 ALTER TABLE `combatmessages_combatactions` DISABLE KEYS */;
/*!40000 ALTER TABLE `combatmessages_combatactions` ENABLE KEYS */;

--
-- Definition of crafts
--

DROP TABLE IF EXISTS `crafts`;
CREATE TABLE IF NOT EXISTS `crafts` (
  `Id` bigint NOT NULL,
  `RevisionNumber` int NOT NULL,
  `EditableItemId` bigint NOT NULL,
  `Name` varchar(200) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `Blurb` text CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `ActionDescription` varchar(200) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `Category` varchar(200) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `Interruptable` bit(1) NOT NULL,
  `ToolQualityWeighting` double NOT NULL,
  `InputQualityWeighting` double NOT NULL,
  `CheckQualityWeighting` double NOT NULL,
  `FreeSkillChecks` int NOT NULL,
  `FailThreshold` int NOT NULL,
  `CheckTraitId` bigint DEFAULT NULL,
  `CheckDifficulty` int NOT NULL,
  `FailPhase` int NOT NULL,
  `QualityFormula` text CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `AppearInCraftsListProgId` bigint DEFAULT NULL,
  `CanUseProgId` bigint DEFAULT NULL,
  `WhyCannotUseProgId` bigint DEFAULT NULL,
  `OnUseProgStartId` bigint DEFAULT NULL,
  `OnUseProgCompleteId` bigint DEFAULT NULL,
  `OnUseProgCancelId` bigint DEFAULT NULL,
  `ActiveCraftItemSDesc` varchar(200) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL DEFAULT 'a craft in progress',
  `IsPracticalCheck` bit(1) NOT NULL DEFAULT b'1',
  PRIMARY KEY (`Id`,`RevisionNumber`),
  KEY `FK_Crafts_FutureProgs_AppearInCraftsListProg_idx` (`AppearInCraftsListProgId`),
  KEY `FK_Crafts_FutureProgs_CanUseProg_idx` (`CanUseProgId`),
  KEY `FK_Crafts_TraitDefinitions_idx` (`CheckTraitId`),
  KEY `FK_Crafts_EditableItems_idx` (`EditableItemId`),
  KEY `FK_Crafts_FutureProgs_OnUseProgCancel_idx` (`OnUseProgCancelId`),
  KEY `FK_Crafts_FutureProgs_OnUseProgComplete_idx` (`OnUseProgCompleteId`),
  KEY `FK_Crafts_FutureProgs_OnUseProgStart_idx` (`OnUseProgStartId`),
  KEY `FK_Crafts_FutureProgs_WhyCannotUseProg_idx` (`WhyCannotUseProgId`),
  CONSTRAINT `FK_Crafts_EditableItems` FOREIGN KEY (`EditableItemId`) REFERENCES `editableitems` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_Crafts_FutureProgs_AppearInCraftsListProg` FOREIGN KEY (`AppearInCraftsListProgId`) REFERENCES `futureprogs` (`Id`) ON DELETE RESTRICT,
  CONSTRAINT `FK_Crafts_FutureProgs_CanUseProg` FOREIGN KEY (`CanUseProgId`) REFERENCES `futureprogs` (`Id`) ON DELETE SET NULL,
  CONSTRAINT `FK_Crafts_FutureProgs_OnUseProgCancel` FOREIGN KEY (`OnUseProgCancelId`) REFERENCES `futureprogs` (`Id`) ON DELETE SET NULL,
  CONSTRAINT `FK_Crafts_FutureProgs_OnUseProgComplete` FOREIGN KEY (`OnUseProgCompleteId`) REFERENCES `futureprogs` (`Id`) ON DELETE SET NULL,
  CONSTRAINT `FK_Crafts_FutureProgs_OnUseProgStart` FOREIGN KEY (`OnUseProgStartId`) REFERENCES `futureprogs` (`Id`) ON DELETE SET NULL,
  CONSTRAINT `FK_Crafts_FutureProgs_WhyCannotUseProg` FOREIGN KEY (`WhyCannotUseProgId`) REFERENCES `futureprogs` (`Id`) ON DELETE SET NULL,
  CONSTRAINT `FK_Crafts_TraitDefinitions` FOREIGN KEY (`CheckTraitId`) REFERENCES `traitdefinitions` (`Id`) ON DELETE RESTRICT
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Dumping data for table crafts
--

/*!40000 ALTER TABLE `crafts` DISABLE KEYS */;
/*!40000 ALTER TABLE `crafts` ENABLE KEYS */;

--
-- Definition of craftinputs
--

DROP TABLE IF EXISTS `craftinputs`;
CREATE TABLE IF NOT EXISTS `craftinputs` (
  `Id` bigint NOT NULL AUTO_INCREMENT,
  `CraftId` bigint NOT NULL,
  `CraftRevisionNumber` int NOT NULL,
  `InputType` varchar(45) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `InputQualityWeight` double NOT NULL,
  `Definition` text CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `OriginalAdditionTime` datetime NOT NULL,
  PRIMARY KEY (`Id`),
  KEY `FK_CraftInputs_Crafts_idx` (`CraftId`,`CraftRevisionNumber`),
  CONSTRAINT `FK_CraftInputs_Crafts` FOREIGN KEY (`CraftId`, `CraftRevisionNumber`) REFERENCES `crafts` (`Id`, `RevisionNumber`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Dumping data for table craftinputs
--

/*!40000 ALTER TABLE `craftinputs` DISABLE KEYS */;
/*!40000 ALTER TABLE `craftinputs` ENABLE KEYS */;

--
-- Definition of craftphases
--

DROP TABLE IF EXISTS `craftphases`;
CREATE TABLE IF NOT EXISTS `craftphases` (
  `CraftPhaseId` bigint NOT NULL,
  `CraftPhaseRevisionNumber` int NOT NULL,
  `PhaseNumber` int NOT NULL,
  `PhaseLengthInSeconds` double NOT NULL,
  `Echo` text CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `FailEcho` text CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci,
  `ExertionLevel` int NOT NULL DEFAULT '0',
  `StaminaUsage` double NOT NULL DEFAULT '0',
  PRIMARY KEY (`CraftPhaseId`,`CraftPhaseRevisionNumber`,`PhaseNumber`),
  CONSTRAINT `FK_CraftPhases_Crafts` FOREIGN KEY (`CraftPhaseId`, `CraftPhaseRevisionNumber`) REFERENCES `crafts` (`Id`, `RevisionNumber`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Dumping data for table craftphases
--

/*!40000 ALTER TABLE `craftphases` DISABLE KEYS */;
/*!40000 ALTER TABLE `craftphases` ENABLE KEYS */;

--
-- Definition of craftproducts
--

DROP TABLE IF EXISTS `craftproducts`;
CREATE TABLE IF NOT EXISTS `craftproducts` (
  `Id` bigint NOT NULL AUTO_INCREMENT,
  `CraftId` bigint NOT NULL,
  `CraftRevisionNumber` int NOT NULL,
  `ProductType` varchar(45) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `Definition` text CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `OriginalAdditionTime` datetime NOT NULL,
  `IsFailProduct` bit(1) NOT NULL DEFAULT b'0',
  `MaterialDefiningInputIndex` int DEFAULT NULL,
  PRIMARY KEY (`Id`),
  KEY `FK_CraftProducts_Crafts_idx` (`CraftId`,`CraftRevisionNumber`),
  CONSTRAINT `FK_CraftProducts_Crafts` FOREIGN KEY (`CraftId`, `CraftRevisionNumber`) REFERENCES `crafts` (`Id`, `RevisionNumber`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Dumping data for table craftproducts
--

/*!40000 ALTER TABLE `craftproducts` DISABLE KEYS */;
/*!40000 ALTER TABLE `craftproducts` ENABLE KEYS */;

--
-- Definition of crafttools
--

DROP TABLE IF EXISTS `crafttools`;
CREATE TABLE IF NOT EXISTS `crafttools` (
  `Id` bigint NOT NULL AUTO_INCREMENT,
  `CraftId` bigint NOT NULL,
  `CraftRevisionNumber` int NOT NULL,
  `OriginalAdditionTime` datetime NOT NULL,
  `ToolType` varchar(45) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `ToolQualityWeight` double NOT NULL,
  `DesiredState` int NOT NULL,
  `Definition` text CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `UseToolDuration` bit(1) NOT NULL DEFAULT b'1',
  PRIMARY KEY (`Id`),
  KEY `FK_CraftTools_Crafts_idx` (`CraftId`,`CraftRevisionNumber`),
  CONSTRAINT `FK_CraftTools_Crafts` FOREIGN KEY (`CraftId`, `CraftRevisionNumber`) REFERENCES `crafts` (`Id`, `RevisionNumber`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Dumping data for table crafttools
--

/*!40000 ALTER TABLE `crafttools` DISABLE KEYS */;
/*!40000 ALTER TABLE `crafttools` ENABLE KEYS */;

--
-- Definition of languages
--

DROP TABLE IF EXISTS `languages`;
CREATE TABLE IF NOT EXISTS `languages` (
  `Id` bigint NOT NULL AUTO_INCREMENT,
  `DifficultyModel` bigint NOT NULL,
  `LinkedTraitId` bigint NOT NULL,
  `UnknownLanguageDescription` varchar(255) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `LanguageObfuscationFactor` double NOT NULL,
  `Name` varchar(255) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `DefaultLearnerAccentId` bigint DEFAULT NULL,
  PRIMARY KEY (`Id`),
  KEY `FK_Languages_Accents_idx` (`DefaultLearnerAccentId`),
  KEY `FK_Languages_LanguageDifficultyModels` (`DifficultyModel`),
  KEY `FK_Languages_TraitDefinitions` (`LinkedTraitId`),
  CONSTRAINT `FK_Languages_Accents` FOREIGN KEY (`DefaultLearnerAccentId`) REFERENCES `accents` (`Id`) ON DELETE RESTRICT,
  CONSTRAINT `FK_Languages_LanguageDifficultyModels` FOREIGN KEY (`DifficultyModel`) REFERENCES `languagedifficultymodels` (`Id`) ON DELETE RESTRICT,
  CONSTRAINT `FK_Languages_TraitDefinitions` FOREIGN KEY (`LinkedTraitId`) REFERENCES `traitdefinitions` (`Id`) ON DELETE RESTRICT
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Dumping data for table languages
--

/*!40000 ALTER TABLE `languages` DISABLE KEYS */;
/*!40000 ALTER TABLE `languages` ENABLE KEYS */;

--
-- Definition of accents
--

DROP TABLE IF EXISTS `accents`;
CREATE TABLE IF NOT EXISTS `accents` (
  `Id` bigint NOT NULL AUTO_INCREMENT,
  `LanguageId` bigint NOT NULL,
  `Name` varchar(50) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `Suffix` varchar(4000) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `VagueSuffix` varchar(4000) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `Difficulty` int NOT NULL,
  `Description` varchar(4000) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `Group` varchar(50) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci DEFAULT NULL,
  `ChargenAvailabilityProgId` bigint DEFAULT NULL,
  PRIMARY KEY (`Id`),
  KEY `FK_Accents_Languages` (`LanguageId`),
  CONSTRAINT `FK_Accents_Languages` FOREIGN KEY (`LanguageId`) REFERENCES `languages` (`Id`) ON DELETE RESTRICT
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Dumping data for table accents
--

/*!40000 ALTER TABLE `accents` DISABLE KEYS */;
/*!40000 ALTER TABLE `accents` ENABLE KEYS */;

--
-- Definition of characters_languages
--

DROP TABLE IF EXISTS `characters_languages`;
CREATE TABLE IF NOT EXISTS `characters_languages` (
  `CharacterId` bigint NOT NULL,
  `LanguageId` bigint NOT NULL,
  PRIMARY KEY (`CharacterId`,`LanguageId`),
  KEY `FK_Characters_Languages_Languages_idx` (`LanguageId`),
  CONSTRAINT `FK_Characters_Languages_Characters` FOREIGN KEY (`CharacterId`) REFERENCES `characters` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_Characters_Languages_Languages` FOREIGN KEY (`LanguageId`) REFERENCES `languages` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Dumping data for table characters_languages
--

/*!40000 ALTER TABLE `characters_languages` DISABLE KEYS */;
/*!40000 ALTER TABLE `characters_languages` ENABLE KEYS */;

--
-- Definition of mutualintelligabilities
--

DROP TABLE IF EXISTS `mutualintelligabilities`;
CREATE TABLE IF NOT EXISTS `mutualintelligabilities` (
  `ListenerLanguageId` bigint NOT NULL,
  `TargetLanguageId` bigint NOT NULL,
  `IntelligabilityDifficulty` int NOT NULL,
  PRIMARY KEY (`ListenerLanguageId`,`TargetLanguageId`),
  KEY `FK_Languages_MutualIntelligabilities_Target_idx` (`TargetLanguageId`),
  CONSTRAINT `FK_Languages_MutualIntelligabilities_Listener` FOREIGN KEY (`ListenerLanguageId`) REFERENCES `languages` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_Languages_MutualIntelligabilities_Target` FOREIGN KEY (`TargetLanguageId`) REFERENCES `languages` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Dumping data for table mutualintelligabilities
--

/*!40000 ALTER TABLE `mutualintelligabilities` DISABLE KEYS */;
/*!40000 ALTER TABLE `mutualintelligabilities` ENABLE KEYS */;

--
-- Definition of racebutcheryprofiles_breakdownchecks
--

DROP TABLE IF EXISTS `racebutcheryprofiles_breakdownchecks`;
CREATE TABLE IF NOT EXISTS `racebutcheryprofiles_breakdownchecks` (
  `RaceButcheryProfileId` bigint NOT NULL,
  `Subcageory` varchar(100) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `TraitDefinitionId` bigint NOT NULL,
  `Difficulty` int NOT NULL,
  PRIMARY KEY (`RaceButcheryProfileId`,`Subcageory`),
  KEY `FK_RaceButcheryProfiles_BreakdownChecks_TraitDefinitions_idx` (`TraitDefinitionId`),
  CONSTRAINT `FK_RaceButcheryProfiles_BreakdownChecks_RaceButcheryProfiles` FOREIGN KEY (`RaceButcheryProfileId`) REFERENCES `racebutcheryprofiles` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_RaceButcheryProfiles_BreakdownChecks_TraitDefinitions` FOREIGN KEY (`TraitDefinitionId`) REFERENCES `traitdefinitions` (`Id`) ON DELETE RESTRICT
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Dumping data for table racebutcheryprofiles_breakdownchecks
--

/*!40000 ALTER TABLE `racebutcheryprofiles_breakdownchecks` DISABLE KEYS */;
/*!40000 ALTER TABLE `racebutcheryprofiles_breakdownchecks` ENABLE KEYS */;

--
-- Definition of races_attributes
--

DROP TABLE IF EXISTS `races_attributes`;
CREATE TABLE IF NOT EXISTS `races_attributes` (
  `RaceId` bigint NOT NULL,
  `AttributeId` bigint NOT NULL,
  `IsHealthAttribute` bit(1) NOT NULL DEFAULT b'0',
  `AttributeBonus` double NOT NULL DEFAULT '0',
  `DiceExpression` varchar(255) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci DEFAULT NULL,
  PRIMARY KEY (`RaceId`,`AttributeId`),
  KEY `FK_Races_Attributes_TraitDefinitions` (`AttributeId`),
  CONSTRAINT `FK_Races_Attributes_Races` FOREIGN KEY (`RaceId`) REFERENCES `races` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_Races_Attributes_TraitDefinitions` FOREIGN KEY (`AttributeId`) REFERENCES `traitdefinitions` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Dumping data for table races_attributes
--

/*!40000 ALTER TABLE `races_attributes` DISABLE KEYS */;
/*!40000 ALTER TABLE `races_attributes` ENABLE KEYS */;

--
-- Definition of rangedweapontypes
--

DROP TABLE IF EXISTS `rangedweapontypes`;
CREATE TABLE IF NOT EXISTS `rangedweapontypes` (
  `Id` bigint NOT NULL AUTO_INCREMENT,
  `Name` varchar(200) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL,
  `Classification` int NOT NULL,
  `FireTraitId` bigint NOT NULL,
  `OperateTraitId` bigint NOT NULL,
  `FireableInMelee` bit(1) NOT NULL,
  `DefaultRangeInRooms` int NOT NULL,
  `AccuracyBonusExpression` text CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL,
  `DamageBonusExpression` text CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL,
  `AmmunitionLoadType` int NOT NULL,
  `SpecificAmmunitionGrade` text CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL,
  `AmmunitionCapacity` int NOT NULL,
  `RangedWeaponType` int NOT NULL,
  `StaminaToFire` double NOT NULL,
  `StaminaPerLoadStage` double NOT NULL,
  `CoverBonus` double NOT NULL,
  `BaseAimDifficulty` int NOT NULL,
  `LoadDelay` double NOT NULL DEFAULT '0.5',
  `ReadyDelay` double NOT NULL DEFAULT '0.1',
  `FireDelay` double NOT NULL DEFAULT '0.5',
  `AimBonusLostPerShot` double NOT NULL DEFAULT '1',
  `RequiresFreeHandToReady` bit(1) NOT NULL DEFAULT b'1',
  `AlwaysRequiresTwoHandsToWield` bit(1) NOT NULL DEFAULT b'0',
  PRIMARY KEY (`Id`),
  KEY `FK_RangedWeaponTypes_TraitDefinitions_Fire_idx` (`FireTraitId`),
  KEY `FK_RangedWeaponTypes_TraitDefinitions_Operate_idx` (`OperateTraitId`),
  CONSTRAINT `FK_RangedWeaponTypes_TraitDefinitions_Fire` FOREIGN KEY (`FireTraitId`) REFERENCES `traitdefinitions` (`Id`) ON DELETE RESTRICT,
  CONSTRAINT `FK_RangedWeaponTypes_TraitDefinitions_Operate` FOREIGN KEY (`OperateTraitId`) REFERENCES `traitdefinitions` (`Id`) ON DELETE RESTRICT
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Dumping data for table rangedweapontypes
--

/*!40000 ALTER TABLE `rangedweapontypes` DISABLE KEYS */;
/*!40000 ALTER TABLE `rangedweapontypes` ENABLE KEYS */;

--
-- Definition of ammunitiontypes
--

DROP TABLE IF EXISTS `ammunitiontypes`;
CREATE TABLE IF NOT EXISTS `ammunitiontypes` (
  `Id` bigint NOT NULL AUTO_INCREMENT,
  `Name` varchar(200) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL,
  `SpecificType` text CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL,
  `RangedWeaponTypes` varchar(200) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL,
  `BaseAccuracy` double NOT NULL,
  `Loudness` int NOT NULL,
  `BreakChanceOnHit` double NOT NULL,
  `BreakChanceOnMiss` double NOT NULL,
  `BaseBlockDifficulty` int NOT NULL,
  `BaseDodgeDifficulty` int NOT NULL,
  `DamageExpression` text CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL,
  `StunExpression` text CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL,
  `PainExpression` text CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL,
  `DamageType` int NOT NULL,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Dumping data for table ammunitiontypes
--

/*!40000 ALTER TABLE `ammunitiontypes` DISABLE KEYS */;
/*!40000 ALTER TABLE `ammunitiontypes` ENABLE KEYS */;

--
-- Definition of shieldtypes
--

DROP TABLE IF EXISTS `shieldtypes`;
CREATE TABLE IF NOT EXISTS `shieldtypes` (
  `Id` bigint NOT NULL AUTO_INCREMENT,
  `Name` text CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL,
  `BlockTraitId` bigint NOT NULL,
  `BlockBonus` double NOT NULL,
  `StaminaPerBlock` double NOT NULL,
  `EffectiveArmourTypeId` bigint DEFAULT NULL,
  PRIMARY KEY (`Id`),
  KEY `FK_ShieldTypes_TraitDefinitions_idx` (`BlockTraitId`),
  KEY `FK_ShieldTypes_ArmourTypes_idx` (`EffectiveArmourTypeId`),
  CONSTRAINT `FK_ShieldTypes_ArmourTypes` FOREIGN KEY (`EffectiveArmourTypeId`) REFERENCES `armourtypes` (`Id`) ON DELETE SET NULL,
  CONSTRAINT `FK_ShieldTypes_TraitDefinitions` FOREIGN KEY (`BlockTraitId`) REFERENCES `traitdefinitions` (`Id`) ON DELETE RESTRICT
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Dumping data for table shieldtypes
--

/*!40000 ALTER TABLE `shieldtypes` DISABLE KEYS */;
/*!40000 ALTER TABLE `shieldtypes` ENABLE KEYS */;

--
-- Definition of surgicalprocedures
--

DROP TABLE IF EXISTS `surgicalprocedures`;
CREATE TABLE IF NOT EXISTS `surgicalprocedures` (
  `Id` bigint NOT NULL AUTO_INCREMENT,
  `Name` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL,
  `ProcedureName` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL,
  `Procedure` int NOT NULL,
  `BaseCheckBonus` double NOT NULL,
  `Check` int NOT NULL,
  `MedicalSchool` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `KnowledgeRequiredId` bigint DEFAULT NULL,
  `UsabilityProgId` bigint DEFAULT NULL,
  `WhyCannotUseProgId` bigint DEFAULT NULL,
  `CompletionProgId` bigint DEFAULT NULL,
  `AbortProgId` bigint DEFAULT NULL,
  `ProcedureBeginEmote` text CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci,
  `ProcedureDescriptionEmote` text CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci,
  `ProcedureGerund` text CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci,
  `Definition` text CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci,
  `CheckTraitDefinitionId` bigint DEFAULT NULL,
  `TargetBodyTypeId` bigint NOT NULL DEFAULT '0',
  PRIMARY KEY (`Id`),
  KEY `FK_SurgicalProcedures_FutureProgs_AbortProg_idx` (`AbortProgId`),
  KEY `FK_SurgicalProcedures_FutureProgs_CompletionProg_idx` (`CompletionProgId`),
  KEY `FK_SurgicalProcedures_Knowledges_idx` (`KnowledgeRequiredId`),
  KEY `FK_SurgicalProcedures_FutureProgs_Usability_idx` (`UsabilityProgId`),
  KEY `FK_SurgicalProcedures_FutureProgs_WhyCannotUseProg_idx` (`WhyCannotUseProgId`),
  KEY `IX_SurgicalProcedures_CheckTraitDefinitionId` (`CheckTraitDefinitionId`),
  KEY `IX_SurgicalProcedures_TargetBodyTypeId` (`TargetBodyTypeId`),
  CONSTRAINT `FK_SurgicalProcedures_BodyProtos` FOREIGN KEY (`TargetBodyTypeId`) REFERENCES `bodyprotos` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_SurgicalProcedures_FutureProgs_AbortProg` FOREIGN KEY (`AbortProgId`) REFERENCES `futureprogs` (`Id`) ON DELETE SET NULL,
  CONSTRAINT `FK_SurgicalProcedures_FutureProgs_CompletionProg` FOREIGN KEY (`CompletionProgId`) REFERENCES `futureprogs` (`Id`) ON DELETE SET NULL,
  CONSTRAINT `FK_SurgicalProcedures_FutureProgs_Usability` FOREIGN KEY (`UsabilityProgId`) REFERENCES `futureprogs` (`Id`) ON DELETE SET NULL,
  CONSTRAINT `FK_SurgicalProcedures_FutureProgs_WhyCannotUseProg` FOREIGN KEY (`WhyCannotUseProgId`) REFERENCES `futureprogs` (`Id`) ON DELETE SET NULL,
  CONSTRAINT `FK_SurgicalProcedures_Knowledges` FOREIGN KEY (`KnowledgeRequiredId`) REFERENCES `knowledges` (`Id`) ON DELETE SET NULL,
  CONSTRAINT `FK_SurgicalProcedures_TraitDefinitions` FOREIGN KEY (`CheckTraitDefinitionId`) REFERENCES `traitdefinitions` (`Id`) ON DELETE SET NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Dumping data for table surgicalprocedures
--

/*!40000 ALTER TABLE `surgicalprocedures` DISABLE KEYS */;
/*!40000 ALTER TABLE `surgicalprocedures` ENABLE KEYS */;

--
-- Definition of hospitalservices
--

DROP TABLE IF EXISTS `hospitalservices`;
CREATE TABLE IF NOT EXISTS `hospitalservices` (
  `Id` bigint NOT NULL AUTO_INCREMENT,
  `HospitalId` bigint NOT NULL,
  `Name` varchar(200) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `Keywords` varchar(500) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `Description` mediumtext CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `ServiceType` int NOT NULL,
  `Price` decimal(58,29) NOT NULL,
  `IsActive` bit(1) NOT NULL DEFAULT b'1',
  `AllowDebt` bit(1) NOT NULL DEFAULT b'1',
  `PreferOperatingTheatre` bit(1) NOT NULL DEFAULT b'0',
  `SortOrder` int NOT NULL,
  `SurgicalProcedureId` bigint DEFAULT NULL,
  `ImplantItemPrototypeId` bigint DEFAULT NULL,
  `ImplantItemPrototypeRevisionNumber` int DEFAULT NULL,
  `ProcedureParameters` mediumtext CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `RequiredEquipmentJson` mediumtext CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci,
  `BloodVolumeLitres` double NOT NULL DEFAULT '0.5',
  `RequiresRecovery` bit(1) NOT NULL DEFAULT b'0',
  `AnesthesiaDrugId` bigint DEFAULT NULL,
  `AnesthesiaIntensity` double NOT NULL DEFAULT '1.25',
  `ImplantInterfaceProcedureId` bigint DEFAULT NULL,
  `ImplantPowerProcedureId` bigint DEFAULT NULL,
  `AnesthesiaCannulationProcedureId` bigint DEFAULT NULL,
  `OfferingMode` int NOT NULL DEFAULT '0',
  PRIMARY KEY (`Id`),
  KEY `FK_HospitalServices_GameItemProtos_idx` (`ImplantItemPrototypeId`,`ImplantItemPrototypeRevisionNumber`),
  KEY `FK_HospitalServices_Hospitals_idx` (`HospitalId`),
  KEY `FK_HospitalServices_SurgicalProcedures_idx` (`SurgicalProcedureId`),
  KEY `IX_HospitalServices_Hospital_Name` (`HospitalId`,`Name`),
  KEY `FK_HospitalServices_Drugs_Anesthesia_idx` (`AnesthesiaDrugId`),
  KEY `FK_HospitalServices_ImplantInterfaceProcedure_idx` (`ImplantInterfaceProcedureId`),
  KEY `FK_HospitalServices_ImplantPowerProcedure_idx` (`ImplantPowerProcedureId`),
  KEY `FK_HospitalServices_AnesthesiaCannulationProcedure_idx` (`AnesthesiaCannulationProcedureId`),
  CONSTRAINT `FK_HospitalServices_AnesthesiaCannulationProcedure` FOREIGN KEY (`AnesthesiaCannulationProcedureId`) REFERENCES `surgicalprocedures` (`Id`) ON DELETE SET NULL,
  CONSTRAINT `FK_HospitalServices_Drugs_Anesthesia` FOREIGN KEY (`AnesthesiaDrugId`) REFERENCES `drugs` (`Id`) ON DELETE SET NULL,
  CONSTRAINT `FK_HospitalServices_GameItemProtos` FOREIGN KEY (`ImplantItemPrototypeId`, `ImplantItemPrototypeRevisionNumber`) REFERENCES `gameitemprotos` (`Id`, `RevisionNumber`) ON DELETE SET NULL,
  CONSTRAINT `FK_HospitalServices_Hospitals` FOREIGN KEY (`HospitalId`) REFERENCES `hospitals` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_HospitalServices_ImplantInterfaceProcedure` FOREIGN KEY (`ImplantInterfaceProcedureId`) REFERENCES `surgicalprocedures` (`Id`) ON DELETE SET NULL,
  CONSTRAINT `FK_HospitalServices_ImplantPowerProcedure` FOREIGN KEY (`ImplantPowerProcedureId`) REFERENCES `surgicalprocedures` (`Id`) ON DELETE SET NULL,
  CONSTRAINT `FK_HospitalServices_SurgicalProcedures` FOREIGN KEY (`SurgicalProcedureId`) REFERENCES `surgicalprocedures` (`Id`) ON DELETE SET NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Dumping data for table hospitalservices
--

/*!40000 ALTER TABLE `hospitalservices` DISABLE KEYS */;
/*!40000 ALTER TABLE `hospitalservices` ENABLE KEYS */;

--
-- Definition of hospitalservicerequests
--

DROP TABLE IF EXISTS `hospitalservicerequests`;
CREATE TABLE IF NOT EXISTS `hospitalservicerequests` (
  `Id` bigint NOT NULL AUTO_INCREMENT,
  `HospitalId` bigint NOT NULL,
  `HospitalServiceId` bigint NOT NULL,
  `RequesterId` bigint NOT NULL,
  `RequesterName` mediumtext CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `PatientId` bigint NOT NULL,
  `PatientName` mediumtext CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `Status` int NOT NULL,
  `PaymentMethod` int NOT NULL,
  `Price` decimal(58,29) NOT NULL,
  `AmountPaid` decimal(58,29) NOT NULL,
  `DebtCharged` decimal(58,29) NOT NULL,
  `EmploymentTaskId` varchar(36) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci DEFAULT NULL,
  `AssignedEmployeeId` bigint DEFAULT NULL,
  `OperatingTheatreCellId` bigint DEFAULT NULL,
  `UsedInPlaceFallback` bit(1) NOT NULL,
  `SupplyPrepared` bit(1) NOT NULL,
  `PreparedByEmployeeId` bigint DEFAULT NULL,
  `PreparedAtUtc` datetime(6) DEFAULT NULL,
  `RecoveryRoomCellId` bigint DEFAULT NULL,
  `ReturnCellId` bigint DEFAULT NULL,
  `CreatedAtUtc` datetime(6) NOT NULL,
  `LastUpdatedAtUtc` datetime(6) NOT NULL,
  `CompletedAtUtc` datetime(6) DEFAULT NULL,
  `OperationalNotes` mediumtext CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `ProcedureParameters` mediumtext CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL DEFAULT (_utf8mb4''),
  PRIMARY KEY (`Id`),
  KEY `FK_HospitalServiceRequests_Cells_Recovery_idx` (`RecoveryRoomCellId`),
  KEY `FK_HospitalServiceRequests_Cells_Return_idx` (`ReturnCellId`),
  KEY `FK_HospitalServiceRequests_Cells_Theatre_idx` (`OperatingTheatreCellId`),
  KEY `FK_HospitalServiceRequests_Characters_Employee_idx` (`AssignedEmployeeId`),
  KEY `FK_HospitalServiceRequests_Characters_Patient_idx` (`PatientId`),
  KEY `FK_HospitalServiceRequests_Characters_PreparedBy_idx` (`PreparedByEmployeeId`),
  KEY `FK_HospitalServiceRequests_Characters_Requester_idx` (`RequesterId`),
  KEY `FK_HospitalServiceRequests_Hospitals_idx` (`HospitalId`),
  KEY `FK_HospitalServiceRequests_HospitalServices_idx` (`HospitalServiceId`),
  KEY `IX_HospitalServiceRequests_EmploymentTaskId` (`EmploymentTaskId`),
  KEY `IX_HospitalServiceRequests_Hospital_Status` (`HospitalId`,`Status`),
  CONSTRAINT `FK_HospitalServiceRequests_Cells_Recovery` FOREIGN KEY (`RecoveryRoomCellId`) REFERENCES `cells` (`Id`) ON DELETE SET NULL,
  CONSTRAINT `FK_HospitalServiceRequests_Cells_Return` FOREIGN KEY (`ReturnCellId`) REFERENCES `cells` (`Id`) ON DELETE SET NULL,
  CONSTRAINT `FK_HospitalServiceRequests_Cells_Theatre` FOREIGN KEY (`OperatingTheatreCellId`) REFERENCES `cells` (`Id`) ON DELETE SET NULL,
  CONSTRAINT `FK_HospitalServiceRequests_Characters_Employee` FOREIGN KEY (`AssignedEmployeeId`) REFERENCES `characters` (`Id`) ON DELETE SET NULL,
  CONSTRAINT `FK_HospitalServiceRequests_Characters_Patient` FOREIGN KEY (`PatientId`) REFERENCES `characters` (`Id`) ON DELETE RESTRICT,
  CONSTRAINT `FK_HospitalServiceRequests_Characters_PreparedBy` FOREIGN KEY (`PreparedByEmployeeId`) REFERENCES `characters` (`Id`) ON DELETE SET NULL,
  CONSTRAINT `FK_HospitalServiceRequests_Characters_Requester` FOREIGN KEY (`RequesterId`) REFERENCES `characters` (`Id`) ON DELETE RESTRICT,
  CONSTRAINT `FK_HospitalServiceRequests_Hospitals` FOREIGN KEY (`HospitalId`) REFERENCES `hospitals` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_HospitalServiceRequests_HospitalServices` FOREIGN KEY (`HospitalServiceId`) REFERENCES `hospitalservices` (`Id`) ON DELETE RESTRICT
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Dumping data for table hospitalservicerequests
--

/*!40000 ALTER TABLE `hospitalservicerequests` DISABLE KEYS */;
/*!40000 ALTER TABLE `hospitalservicerequests` ENABLE KEYS */;

--
-- Definition of surgicalprocedurephases
--

DROP TABLE IF EXISTS `surgicalprocedurephases`;
CREATE TABLE IF NOT EXISTS `surgicalprocedurephases` (
  `SurgicalProcedureId` bigint NOT NULL,
  `PhaseNumber` int NOT NULL,
  `BaseLengthInSeconds` double NOT NULL,
  `PhaseEmote` text CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL,
  `PhaseSpecialEffects` varchar(500) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `OnPhaseProgId` bigint DEFAULT NULL,
  `InventoryActionPlan` text CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci,
  PRIMARY KEY (`SurgicalProcedureId`,`PhaseNumber`),
  KEY `FK_SurgicalProcedurePhases_FutureProgs_OnPhaseProg_idx` (`OnPhaseProgId`),
  CONSTRAINT `FK_SurgicalProcedurePhases_FutureProgs_OnPhaseProg` FOREIGN KEY (`OnPhaseProgId`) REFERENCES `futureprogs` (`Id`) ON DELETE SET NULL,
  CONSTRAINT `FK_SurgicalProcedurePhases_SurgicalProcudres` FOREIGN KEY (`SurgicalProcedureId`) REFERENCES `surgicalprocedures` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Dumping data for table surgicalprocedurephases
--

/*!40000 ALTER TABLE `surgicalprocedurephases` DISABLE KEYS */;
/*!40000 ALTER TABLE `surgicalprocedurephases` ENABLE KEYS */;

--
-- Definition of traitdefinitions_chargenresources
--

DROP TABLE IF EXISTS `traitdefinitions_chargenresources`;
CREATE TABLE IF NOT EXISTS `traitdefinitions_chargenresources` (
  `TraitDefinitionId` bigint NOT NULL,
  `ChargenResourceId` bigint NOT NULL,
  `RequirementOnly` bit(1) NOT NULL DEFAULT b'0',
  `Amount` int NOT NULL,
  PRIMARY KEY (`TraitDefinitionId`,`ChargenResourceId`,`RequirementOnly`),
  KEY `FK_TraitDefinitions_ChargenResources_ChargenResources` (`ChargenResourceId`),
  CONSTRAINT `FK_TraitDefinitions_ChargenResources_ChargenResources` FOREIGN KEY (`ChargenResourceId`) REFERENCES `chargenresources` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_TraitDefinitions_ChargenResources_Races` FOREIGN KEY (`TraitDefinitionId`) REFERENCES `traitdefinitions` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Dumping data for table traitdefinitions_chargenresources
--

/*!40000 ALTER TABLE `traitdefinitions_chargenresources` DISABLE KEYS */;
/*!40000 ALTER TABLE `traitdefinitions_chargenresources` ENABLE KEYS */;

--
-- Definition of traitexpressionparameters
--

DROP TABLE IF EXISTS `traitexpressionparameters`;
CREATE TABLE IF NOT EXISTS `traitexpressionparameters` (
  `TraitExpressionId` bigint NOT NULL,
  `Parameter` varchar(50) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `TraitDefinitionId` bigint NOT NULL,
  `CanImprove` bit(1) NOT NULL DEFAULT b'1',
  `CanBranch` bit(1) NOT NULL DEFAULT b'1',
  PRIMARY KEY (`Parameter`,`TraitExpressionId`),
  KEY `FK_TraitExpressionParameters_TraitDefinitions` (`TraitDefinitionId`),
  KEY `FK_TraitExpressionParameters_TraitExpression` (`TraitExpressionId`),
  CONSTRAINT `FK_TraitExpressionParameters_TraitDefinitions` FOREIGN KEY (`TraitDefinitionId`) REFERENCES `traitdefinitions` (`Id`) ON DELETE RESTRICT,
  CONSTRAINT `FK_TraitExpressionParameters_TraitExpression` FOREIGN KEY (`TraitExpressionId`) REFERENCES `traitexpression` (`Id`) ON DELETE RESTRICT
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Dumping data for table traitexpressionparameters
--

/*!40000 ALTER TABLE `traitexpressionparameters` DISABLE KEYS */;
/*!40000 ALTER TABLE `traitexpressionparameters` ENABLE KEYS */;

--
-- Definition of traits
--

DROP TABLE IF EXISTS `traits`;
CREATE TABLE IF NOT EXISTS `traits` (
  `BodyId` bigint NOT NULL,
  `TraitDefinitionId` bigint NOT NULL,
  `Value` double NOT NULL,
  `AdditionalValue` double NOT NULL,
  PRIMARY KEY (`BodyId`,`TraitDefinitionId`),
  KEY `FK_Traits_TraitDefinitions` (`TraitDefinitionId`),
  CONSTRAINT `FK_Traits_Bodies` FOREIGN KEY (`BodyId`) REFERENCES `bodies` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_Traits_TraitDefinitions` FOREIGN KEY (`TraitDefinitionId`) REFERENCES `traitdefinitions` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Dumping data for table traits
--

/*!40000 ALTER TABLE `traits` DISABLE KEYS */;
/*!40000 ALTER TABLE `traits` ENABLE KEYS */;

--
-- Definition of unitofmeasure
--

DROP TABLE IF EXISTS `unitofmeasure`;
CREATE TABLE IF NOT EXISTS `unitofmeasure` (
  `Id` bigint NOT NULL AUTO_INCREMENT,
  `Name` varchar(50) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `PrimaryAbbreviation` varchar(45) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci DEFAULT NULL,
  `Abbreviations` varchar(4000) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `BaseMultiplier` double NOT NULL,
  `PreMultiplierBaseOffset` double NOT NULL,
  `PostMultiplierBaseOffset` double NOT NULL,
  `Type` int NOT NULL,
  `Describer` bit(1) NOT NULL,
  `SpaceBetween` bit(1) NOT NULL DEFAULT b'1',
  `System` varchar(50) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `DefaultUnitForSystem` bit(1) NOT NULL DEFAULT b'0',
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Dumping data for table unitofmeasure
--

/*!40000 ALTER TABLE `unitofmeasure` DISABLE KEYS */;
/*!40000 ALTER TABLE `unitofmeasure` ENABLE KEYS */;

--
-- Definition of variabledefaults
--

DROP TABLE IF EXISTS `variabledefaults`;
CREATE TABLE IF NOT EXISTS `variabledefaults` (
  `Property` varchar(50) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `DefaultValue` varchar(4000) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `OwnerTypeDefinition` varchar(255) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  PRIMARY KEY (`OwnerTypeDefinition`,`Property`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Dumping data for table variabledefaults
--

/*!40000 ALTER TABLE `variabledefaults` DISABLE KEYS */;
/*!40000 ALTER TABLE `variabledefaults` ENABLE KEYS */;

--
-- Definition of variabledefinitions
--

DROP TABLE IF EXISTS `variabledefinitions`;
CREATE TABLE IF NOT EXISTS `variabledefinitions` (
  `Property` varchar(50) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `ContainedTypeDefinition` varchar(255) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci DEFAULT NULL,
  `OwnerTypeDefinition` varchar(255) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  PRIMARY KEY (`OwnerTypeDefinition`,`Property`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Dumping data for table variabledefinitions
--

/*!40000 ALTER TABLE `variabledefinitions` DISABLE KEYS */;
/*!40000 ALTER TABLE `variabledefinitions` ENABLE KEYS */;

--
-- Definition of variablevalues
--

DROP TABLE IF EXISTS `variablevalues`;
CREATE TABLE IF NOT EXISTS `variablevalues` (
  `ReferenceId` bigint NOT NULL,
  `ReferenceProperty` varchar(50) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `ValueDefinition` varchar(4000) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `ReferenceTypeDefinition` varchar(255) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `ValueTypeDefinition` varchar(255) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci DEFAULT NULL,
  PRIMARY KEY (`ReferenceTypeDefinition`,`ReferenceId`,`ReferenceProperty`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Dumping data for table variablevalues
--

/*!40000 ALTER TABLE `variablevalues` DISABLE KEYS */;
/*!40000 ALTER TABLE `variablevalues` ENABLE KEYS */;

--
-- Definition of vehicleaccesspointlocks
--

DROP TABLE IF EXISTS `vehicleaccesspointlocks`;
CREATE TABLE IF NOT EXISTS `vehicleaccesspointlocks` (
  `Id` bigint NOT NULL AUTO_INCREMENT,
  `VehicleAccessPointId` bigint NOT NULL,
  `LockItemId` bigint NOT NULL,
  PRIMARY KEY (`Id`),
  UNIQUE KEY `FK_VehicleAccessPointLocks_GameItems_idx` (`LockItemId`),
  KEY `FK_VehicleAccessPointLocks_AccessPoints_idx` (`VehicleAccessPointId`),
  CONSTRAINT `FK_VehicleAccessPointLocks_AccessPoints` FOREIGN KEY (`VehicleAccessPointId`) REFERENCES `vehicleaccesspoints` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_VehicleAccessPointLocks_GameItems` FOREIGN KEY (`LockItemId`) REFERENCES `gameitems` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Dumping data for table vehicleaccesspointlocks
--

/*!40000 ALTER TABLE `vehicleaccesspointlocks` DISABLE KEYS */;
/*!40000 ALTER TABLE `vehicleaccesspointlocks` ENABLE KEYS */;

--
-- Definition of vehiclepropulsionprofileprotos
--

DROP TABLE IF EXISTS `vehiclepropulsionprofileprotos`;
CREATE TABLE IF NOT EXISTS `vehiclepropulsionprofileprotos` (
  `Id` bigint NOT NULL AUTO_INCREMENT,
  `VehicleMovementProfileProtoId` bigint NOT NULL,
  `PropulsionType` int NOT NULL,
  `IsDefault` bit(1) NOT NULL,
  `BaseMoveTimeMilliseconds` double NOT NULL,
  `PropulsionTraitDefinitionId` bigint DEFAULT NULL,
  `CheckDifficulty` int NOT NULL,
  `SpeedMultiplierExpression` varchar(1000) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `StaminaCostExpression` varchar(1000) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  PRIMARY KEY (`Id`),
  UNIQUE KEY `UX_VehiclePropulsionProfileProtos_Profile_Type` (`VehicleMovementProfileProtoId`,`PropulsionType`),
  KEY `FK_VehiclePropulsionProfileProtos_MovementProfiles_idx` (`VehicleMovementProfileProtoId`),
  KEY `FK_VehiclePropulsionProfileProtos_Traits_idx` (`PropulsionTraitDefinitionId`),
  CONSTRAINT `FK_VehiclePropulsionProfileProtos_MovementProfiles` FOREIGN KEY (`VehicleMovementProfileProtoId`) REFERENCES `vehiclemovementprofileprotos` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_VehiclePropulsionProfileProtos_Traits` FOREIGN KEY (`PropulsionTraitDefinitionId`) REFERENCES `traitdefinitions` (`Id`) ON DELETE SET NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Dumping data for table vehiclepropulsionprofileprotos
--

/*!40000 ALTER TABLE `vehiclepropulsionprofileprotos` DISABLE KEYS */;
/*!40000 ALTER TABLE `vehiclepropulsionprofileprotos` ENABLE KEYS */;

--
-- Definition of vehicleprotos
--

DROP TABLE IF EXISTS `vehicleprotos`;
CREATE TABLE IF NOT EXISTS `vehicleprotos` (
  `Id` bigint NOT NULL,
  `RevisionNumber` int NOT NULL,
  `EditableItemId` bigint NOT NULL,
  `Name` varchar(200) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `Description` mediumtext CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `VehicleScale` int NOT NULL,
  `ExteriorItemProtoId` bigint DEFAULT NULL,
  `ExteriorItemProtoRevision` int DEFAULT NULL,
  PRIMARY KEY (`Id`,`RevisionNumber`),
  KEY `FK_VehicleProtos_EditableItems_idx` (`EditableItemId`),
  KEY `FK_VehicleProtos_GameItemProtos_idx` (`ExteriorItemProtoId`,`ExteriorItemProtoRevision`),
  CONSTRAINT `FK_VehicleProtos_EditableItems` FOREIGN KEY (`EditableItemId`) REFERENCES `editableitems` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_VehicleProtos_GameItemProtos` FOREIGN KEY (`ExteriorItemProtoId`, `ExteriorItemProtoRevision`) REFERENCES `gameitemprotos` (`Id`, `RevisionNumber`) ON DELETE SET NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Dumping data for table vehicleprotos
--

/*!40000 ALTER TABLE `vehicleprotos` DISABLE KEYS */;
/*!40000 ALTER TABLE `vehicleprotos` ENABLE KEYS */;

--
-- Definition of vehicleaccesspointprotos
--

DROP TABLE IF EXISTS `vehicleaccesspointprotos`;
CREATE TABLE IF NOT EXISTS `vehicleaccesspointprotos` (
  `Id` bigint NOT NULL AUTO_INCREMENT,
  `VehicleProtoId` bigint NOT NULL,
  `VehicleProtoRevision` int NOT NULL,
  `VehicleCompartmentProtoId` bigint DEFAULT NULL,
  `Name` varchar(200) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `Description` mediumtext CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `AccessPointType` int NOT NULL,
  `ProjectionItemProtoId` bigint DEFAULT NULL,
  `ProjectionItemProtoRevision` int DEFAULT NULL,
  `StartsOpen` bit(1) NOT NULL,
  `MustBeClosedForMovement` bit(1) NOT NULL,
  `DisplayOrder` int NOT NULL,
  PRIMARY KEY (`Id`),
  KEY `FK_VehicleAccessPointProtos_Compartments_idx` (`VehicleCompartmentProtoId`),
  KEY `FK_VehicleAccessPointProtos_ItemProtos_idx` (`ProjectionItemProtoId`,`ProjectionItemProtoRevision`),
  KEY `FK_VehicleAccessPointProtos_VehicleProtos_idx` (`VehicleProtoId`,`VehicleProtoRevision`),
  CONSTRAINT `FK_VehicleAccessPointProtos_Compartments` FOREIGN KEY (`VehicleCompartmentProtoId`) REFERENCES `vehiclecompartmentprotos` (`Id`) ON DELETE SET NULL,
  CONSTRAINT `FK_VehicleAccessPointProtos_ItemProtos` FOREIGN KEY (`ProjectionItemProtoId`, `ProjectionItemProtoRevision`) REFERENCES `gameitemprotos` (`Id`, `RevisionNumber`) ON DELETE SET NULL,
  CONSTRAINT `FK_VehicleAccessPointProtos_VehicleProtos` FOREIGN KEY (`VehicleProtoId`, `VehicleProtoRevision`) REFERENCES `vehicleprotos` (`Id`, `RevisionNumber`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Dumping data for table vehicleaccesspointprotos
--

/*!40000 ALTER TABLE `vehicleaccesspointprotos` DISABLE KEYS */;
/*!40000 ALTER TABLE `vehicleaccesspointprotos` ENABLE KEYS */;

--
-- Definition of vehiclecargospaceprotos
--

DROP TABLE IF EXISTS `vehiclecargospaceprotos`;
CREATE TABLE IF NOT EXISTS `vehiclecargospaceprotos` (
  `Id` bigint NOT NULL AUTO_INCREMENT,
  `VehicleProtoId` bigint NOT NULL,
  `VehicleProtoRevision` int NOT NULL,
  `VehicleCompartmentProtoId` bigint DEFAULT NULL,
  `RequiredAccessPointProtoId` bigint DEFAULT NULL,
  `Name` varchar(200) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `Description` mediumtext CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `ProjectionItemProtoId` bigint DEFAULT NULL,
  `ProjectionItemProtoRevision` int DEFAULT NULL,
  `DisplayOrder` int NOT NULL,
  PRIMARY KEY (`Id`),
  KEY `FK_VehicleCargoSpaceProtos_AccessPoints_idx` (`RequiredAccessPointProtoId`),
  KEY `FK_VehicleCargoSpaceProtos_Compartments_idx` (`VehicleCompartmentProtoId`),
  KEY `FK_VehicleCargoSpaceProtos_ItemProtos_idx` (`ProjectionItemProtoId`,`ProjectionItemProtoRevision`),
  KEY `FK_VehicleCargoSpaceProtos_VehicleProtos_idx` (`VehicleProtoId`,`VehicleProtoRevision`),
  CONSTRAINT `FK_VehicleCargoSpaceProtos_AccessPoints` FOREIGN KEY (`RequiredAccessPointProtoId`) REFERENCES `vehicleaccesspointprotos` (`Id`) ON DELETE SET NULL,
  CONSTRAINT `FK_VehicleCargoSpaceProtos_Compartments` FOREIGN KEY (`VehicleCompartmentProtoId`) REFERENCES `vehiclecompartmentprotos` (`Id`) ON DELETE SET NULL,
  CONSTRAINT `FK_VehicleCargoSpaceProtos_ItemProtos` FOREIGN KEY (`ProjectionItemProtoId`, `ProjectionItemProtoRevision`) REFERENCES `gameitemprotos` (`Id`, `RevisionNumber`) ON DELETE SET NULL,
  CONSTRAINT `FK_VehicleCargoSpaceProtos_VehicleProtos` FOREIGN KEY (`VehicleProtoId`, `VehicleProtoRevision`) REFERENCES `vehicleprotos` (`Id`, `RevisionNumber`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Dumping data for table vehiclecargospaceprotos
--

/*!40000 ALTER TABLE `vehiclecargospaceprotos` DISABLE KEYS */;
/*!40000 ALTER TABLE `vehiclecargospaceprotos` ENABLE KEYS */;

--
-- Definition of vehiclecompartmentlinkprotos
--

DROP TABLE IF EXISTS `vehiclecompartmentlinkprotos`;
CREATE TABLE IF NOT EXISTS `vehiclecompartmentlinkprotos` (
  `Id` bigint NOT NULL AUTO_INCREMENT,
  `VehicleProtoId` bigint NOT NULL,
  `VehicleProtoRevision` int NOT NULL,
  `SourceVehicleCompartmentProtoId` bigint NOT NULL,
  `DestinationVehicleCompartmentProtoId` bigint NOT NULL,
  `OutboundDirection` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL,
  `InboundDirection` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL,
  `OutboundDescription` varchar(500) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL,
  `InboundDescription` varchar(500) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL,
  PRIMARY KEY (`Id`),
  KEY `FK_VehicleCompartmentLinkProtos_Destination_idx` (`DestinationVehicleCompartmentProtoId`),
  KEY `FK_VehicleCompartmentLinkProtos_Source_idx` (`SourceVehicleCompartmentProtoId`),
  KEY `FK_VehicleCompartmentLinkProtos_VehicleProtos_idx` (`VehicleProtoId`,`VehicleProtoRevision`),
  CONSTRAINT `FK_VehicleCompartmentLinkProtos_Destination` FOREIGN KEY (`DestinationVehicleCompartmentProtoId`) REFERENCES `vehiclecompartmentprotos` (`Id`) ON DELETE RESTRICT,
  CONSTRAINT `FK_VehicleCompartmentLinkProtos_Source` FOREIGN KEY (`SourceVehicleCompartmentProtoId`) REFERENCES `vehiclecompartmentprotos` (`Id`) ON DELETE RESTRICT,
  CONSTRAINT `FK_VehicleCompartmentLinkProtos_VehicleProtos` FOREIGN KEY (`VehicleProtoId`, `VehicleProtoRevision`) REFERENCES `vehicleprotos` (`Id`, `RevisionNumber`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Dumping data for table vehiclecompartmentlinkprotos
--

/*!40000 ALTER TABLE `vehiclecompartmentlinkprotos` DISABLE KEYS */;
/*!40000 ALTER TABLE `vehiclecompartmentlinkprotos` ENABLE KEYS */;

--
-- Definition of vehiclecompartmentprotos
--

DROP TABLE IF EXISTS `vehiclecompartmentprotos`;
CREATE TABLE IF NOT EXISTS `vehiclecompartmentprotos` (
  `Id` bigint NOT NULL AUTO_INCREMENT,
  `VehicleProtoId` bigint NOT NULL,
  `VehicleProtoRevision` int NOT NULL,
  `Name` varchar(200) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `Description` mediumtext CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `DisplayOrder` int NOT NULL,
  `InteriorOutdoorsType` int NOT NULL DEFAULT '0',
  `InteriorTerrainId` bigint DEFAULT NULL,
  PRIMARY KEY (`Id`),
  KEY `FK_VehicleCompartmentProtos_VehicleProtos_idx` (`VehicleProtoId`,`VehicleProtoRevision`),
  KEY `FK_VehicleCompartmentProtos_Terrains_idx` (`InteriorTerrainId`),
  CONSTRAINT `FK_VehicleCompartmentProtos_Terrains` FOREIGN KEY (`InteriorTerrainId`) REFERENCES `terrains` (`Id`) ON DELETE RESTRICT,
  CONSTRAINT `FK_VehicleCompartmentProtos_VehicleProtos` FOREIGN KEY (`VehicleProtoId`, `VehicleProtoRevision`) REFERENCES `vehicleprotos` (`Id`, `RevisionNumber`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Dumping data for table vehiclecompartmentprotos
--

/*!40000 ALTER TABLE `vehiclecompartmentprotos` DISABLE KEYS */;
/*!40000 ALTER TABLE `vehiclecompartmentprotos` ENABLE KEYS */;

--
-- Definition of vehiclecontrolstationprotos
--

DROP TABLE IF EXISTS `vehiclecontrolstationprotos`;
CREATE TABLE IF NOT EXISTS `vehiclecontrolstationprotos` (
  `Id` bigint NOT NULL AUTO_INCREMENT,
  `VehicleProtoId` bigint NOT NULL,
  `VehicleProtoRevision` int NOT NULL,
  `VehicleOccupantSlotProtoId` bigint NOT NULL,
  `Name` varchar(200) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `IsPrimary` bit(1) NOT NULL,
  PRIMARY KEY (`Id`),
  KEY `FK_VehicleControlStationProtos_Slots_idx` (`VehicleOccupantSlotProtoId`),
  KEY `FK_VehicleControlStationProtos_VehicleProtos_idx` (`VehicleProtoId`,`VehicleProtoRevision`),
  CONSTRAINT `FK_VehicleControlStationProtos_Slots` FOREIGN KEY (`VehicleOccupantSlotProtoId`) REFERENCES `vehicleoccupantslotprotos` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_VehicleControlStationProtos_VehicleProtos` FOREIGN KEY (`VehicleProtoId`, `VehicleProtoRevision`) REFERENCES `vehicleprotos` (`Id`, `RevisionNumber`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Dumping data for table vehiclecontrolstationprotos
--

/*!40000 ALTER TABLE `vehiclecontrolstationprotos` DISABLE KEYS */;
/*!40000 ALTER TABLE `vehiclecontrolstationprotos` ENABLE KEYS */;

--
-- Definition of vehicledamagezoneprotos
--

DROP TABLE IF EXISTS `vehicledamagezoneprotos`;
CREATE TABLE IF NOT EXISTS `vehicledamagezoneprotos` (
  `Id` bigint NOT NULL AUTO_INCREMENT,
  `VehicleProtoId` bigint NOT NULL,
  `VehicleProtoRevision` int NOT NULL,
  `Name` varchar(200) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `Description` mediumtext CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `MaximumDamage` double NOT NULL,
  `HitWeight` double NOT NULL,
  `DisabledThreshold` double NOT NULL,
  `DestroyedThreshold` double NOT NULL,
  `DisablesMovement` bit(1) NOT NULL,
  `DisplayOrder` int NOT NULL,
  PRIMARY KEY (`Id`),
  KEY `FK_VehicleDamageZoneProtos_VehicleProtos_idx` (`VehicleProtoId`,`VehicleProtoRevision`),
  CONSTRAINT `FK_VehicleDamageZoneProtos_VehicleProtos` FOREIGN KEY (`VehicleProtoId`, `VehicleProtoRevision`) REFERENCES `vehicleprotos` (`Id`, `RevisionNumber`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Dumping data for table vehicledamagezoneprotos
--

/*!40000 ALTER TABLE `vehicledamagezoneprotos` DISABLE KEYS */;
/*!40000 ALTER TABLE `vehicledamagezoneprotos` ENABLE KEYS */;

--
-- Definition of vehicledamagezoneeffectprotos
--

DROP TABLE IF EXISTS `vehicledamagezoneeffectprotos`;
CREATE TABLE IF NOT EXISTS `vehicledamagezoneeffectprotos` (
  `Id` bigint NOT NULL AUTO_INCREMENT,
  `VehicleDamageZoneProtoId` bigint NOT NULL,
  `TargetType` int NOT NULL,
  `TargetProtoId` bigint DEFAULT NULL,
  `MinimumStatus` int NOT NULL,
  PRIMARY KEY (`Id`),
  KEY `FK_VehicleDamageZoneEffectProtos_DamageZones_idx` (`VehicleDamageZoneProtoId`),
  CONSTRAINT `FK_VehicleDamageZoneEffectProtos_DamageZones` FOREIGN KEY (`VehicleDamageZoneProtoId`) REFERENCES `vehicledamagezoneprotos` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Dumping data for table vehicledamagezoneeffectprotos
--

/*!40000 ALTER TABLE `vehicledamagezoneeffectprotos` DISABLE KEYS */;
/*!40000 ALTER TABLE `vehicledamagezoneeffectprotos` ENABLE KEYS */;

--
-- Definition of vehicleinstallationpointprotos
--

DROP TABLE IF EXISTS `vehicleinstallationpointprotos`;
CREATE TABLE IF NOT EXISTS `vehicleinstallationpointprotos` (
  `Id` bigint NOT NULL AUTO_INCREMENT,
  `VehicleProtoId` bigint NOT NULL,
  `VehicleProtoRevision` int NOT NULL,
  `RequiredAccessPointProtoId` bigint DEFAULT NULL,
  `Name` varchar(200) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `Description` mediumtext CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `MountType` varchar(200) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `RequiredRole` varchar(200) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `RequiredForMovement` bit(1) NOT NULL,
  `DisplayOrder` int NOT NULL,
  PRIMARY KEY (`Id`),
  KEY `FK_VehicleInstallationPointProtos_AccessPoints_idx` (`RequiredAccessPointProtoId`),
  KEY `FK_VehicleInstallationPointProtos_VehicleProtos_idx` (`VehicleProtoId`,`VehicleProtoRevision`),
  CONSTRAINT `FK_VehicleInstallationPointProtos_AccessPoints` FOREIGN KEY (`RequiredAccessPointProtoId`) REFERENCES `vehicleaccesspointprotos` (`Id`) ON DELETE SET NULL,
  CONSTRAINT `FK_VehicleInstallationPointProtos_VehicleProtos` FOREIGN KEY (`VehicleProtoId`, `VehicleProtoRevision`) REFERENCES `vehicleprotos` (`Id`, `RevisionNumber`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Dumping data for table vehicleinstallationpointprotos
--

/*!40000 ALTER TABLE `vehicleinstallationpointprotos` DISABLE KEYS */;
/*!40000 ALTER TABLE `vehicleinstallationpointprotos` ENABLE KEYS */;

--
-- Definition of vehiclemovementprofileprotos
--

DROP TABLE IF EXISTS `vehiclemovementprofileprotos`;
CREATE TABLE IF NOT EXISTS `vehiclemovementprofileprotos` (
  `Id` bigint NOT NULL AUTO_INCREMENT,
  `VehicleProtoId` bigint NOT NULL,
  `VehicleProtoRevision` int NOT NULL,
  `Name` varchar(200) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `MovementType` int NOT NULL,
  `IsDefault` bit(1) NOT NULL,
  `FuelLiquidId` bigint DEFAULT NULL,
  `FuelVolumePerMove` double NOT NULL DEFAULT '0',
  `RequiredInstalledRole` varchar(200) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL DEFAULT '',
  `RequiredPowerSpikeInWatts` double NOT NULL DEFAULT '0',
  `RequiresAccessPointsClosed` bit(1) NOT NULL DEFAULT b'0',
  `RequiresTowLinksClosed` bit(1) NOT NULL DEFAULT b'0',
  `ExposesOccupantsToWater` bit(1) NOT NULL DEFAULT b'0',
  `MovementEnvironment` int NOT NULL DEFAULT '0',
  `AutomaticOperationCapable` bit(1) NOT NULL DEFAULT b'0',
  `RouteFuelVolumePerMetre` double NOT NULL DEFAULT '0',
  `RoutePowerDrawWatts` double NOT NULL DEFAULT '0',
  `RoutePropulsionMode` int NOT NULL DEFAULT '0',
  `RouteSpeedMetresPerSecond` double NOT NULL DEFAULT '0',
  PRIMARY KEY (`Id`),
  KEY `FK_VehicleMovementProfileProtos_VehicleProtos_idx` (`VehicleProtoId`,`VehicleProtoRevision`),
  CONSTRAINT `FK_VehicleMovementProfileProtos_VehicleProtos` FOREIGN KEY (`VehicleProtoId`, `VehicleProtoRevision`) REFERENCES `vehicleprotos` (`Id`, `RevisionNumber`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Dumping data for table vehiclemovementprofileprotos
--

/*!40000 ALTER TABLE `vehiclemovementprofileprotos` DISABLE KEYS */;
/*!40000 ALTER TABLE `vehiclemovementprofileprotos` ENABLE KEYS */;

--
-- Definition of vehicleoccupantslotprotos
--

DROP TABLE IF EXISTS `vehicleoccupantslotprotos`;
CREATE TABLE IF NOT EXISTS `vehicleoccupantslotprotos` (
  `Id` bigint NOT NULL AUTO_INCREMENT,
  `VehicleProtoId` bigint NOT NULL,
  `VehicleProtoRevision` int NOT NULL,
  `VehicleCompartmentProtoId` bigint NOT NULL,
  `Name` varchar(200) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `SlotType` int NOT NULL,
  `Capacity` int NOT NULL,
  `RequiredForMovement` bit(1) NOT NULL,
  `ContributesToPropulsion` bit(1) NOT NULL DEFAULT b'0',
  `AboveRangedCoverId` bigint DEFAULT NULL,
  `BelowRangedCoverId` bigint DEFAULT NULL,
  `BoatStabilityDifficulty` int NOT NULL DEFAULT '5',
  `SameLevelRangedCoverId` bigint DEFAULT NULL,
  PRIMARY KEY (`Id`),
  KEY `FK_VehicleOccupantSlotProtos_Compartments_idx` (`VehicleCompartmentProtoId`),
  KEY `FK_VehicleOccupantSlotProtos_VehicleProtos_idx` (`VehicleProtoId`,`VehicleProtoRevision`),
  KEY `FK_VehicleOccupantSlotProtos_AboveCover_idx` (`AboveRangedCoverId`),
  KEY `FK_VehicleOccupantSlotProtos_BelowCover_idx` (`BelowRangedCoverId`),
  KEY `FK_VehicleOccupantSlotProtos_SameLevelCover_idx` (`SameLevelRangedCoverId`),
  CONSTRAINT `FK_VehicleOccupantSlotProtos_AboveCover` FOREIGN KEY (`AboveRangedCoverId`) REFERENCES `rangedcovers` (`Id`) ON DELETE SET NULL,
  CONSTRAINT `FK_VehicleOccupantSlotProtos_BelowCover` FOREIGN KEY (`BelowRangedCoverId`) REFERENCES `rangedcovers` (`Id`) ON DELETE SET NULL,
  CONSTRAINT `FK_VehicleOccupantSlotProtos_Compartments` FOREIGN KEY (`VehicleCompartmentProtoId`) REFERENCES `vehiclecompartmentprotos` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_VehicleOccupantSlotProtos_SameLevelCover` FOREIGN KEY (`SameLevelRangedCoverId`) REFERENCES `rangedcovers` (`Id`) ON DELETE SET NULL,
  CONSTRAINT `FK_VehicleOccupantSlotProtos_VehicleProtos` FOREIGN KEY (`VehicleProtoId`, `VehicleProtoRevision`) REFERENCES `vehicleprotos` (`Id`, `RevisionNumber`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Dumping data for table vehicleoccupantslotprotos
--

/*!40000 ALTER TABLE `vehicleoccupantslotprotos` DISABLE KEYS */;
/*!40000 ALTER TABLE `vehicleoccupantslotprotos` ENABLE KEYS */;

--
-- Definition of vehicleroutes
--

DROP TABLE IF EXISTS `vehicleroutes`;
CREATE TABLE IF NOT EXISTS `vehicleroutes` (
  `Id` bigint NOT NULL,
  `RevisionNumber` int NOT NULL,
  `EditableItemId` bigint NOT NULL,
  `Name` varchar(200) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL,
  `Description` mediumtext CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL,
  PRIMARY KEY (`Id`,`RevisionNumber`),
  KEY `FK_VehicleRoutes_EditableItems_idx` (`EditableItemId`),
  CONSTRAINT `FK_VehicleRoutes_EditableItems` FOREIGN KEY (`EditableItemId`) REFERENCES `editableitems` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Dumping data for table vehicleroutes
--

/*!40000 ALTER TABLE `vehicleroutes` DISABLE KEYS */;
/*!40000 ALTER TABLE `vehicleroutes` ENABLE KEYS */;

--
-- Definition of vehicleroutelegs
--

DROP TABLE IF EXISTS `vehicleroutelegs`;
CREATE TABLE IF NOT EXISTS `vehicleroutelegs` (
  `Id` bigint NOT NULL AUTO_INCREMENT,
  `VehicleRouteId` bigint NOT NULL,
  `VehicleRouteRevision` int NOT NULL,
  `Sequence` int NOT NULL,
  `OriginStopId` bigint NOT NULL,
  `DestinationStopId` bigint NOT NULL,
  `RouteDistanceMetres` decimal(18,3) NOT NULL,
  `RoomEquivalentCost` decimal(18,6) NOT NULL,
  PRIMARY KEY (`Id`),
  UNIQUE KEY `UX_VehicleRouteLegs_Route_Sequence` (`VehicleRouteId`,`VehicleRouteRevision`,`Sequence`),
  KEY `FK_VehicleRouteLegs_DestinationStops_idx` (`DestinationStopId`,`VehicleRouteId`,`VehicleRouteRevision`),
  KEY `FK_VehicleRouteLegs_OriginStops_idx` (`OriginStopId`,`VehicleRouteId`,`VehicleRouteRevision`),
  CONSTRAINT `FK_VehicleRouteLegs_DestinationStops` FOREIGN KEY (`DestinationStopId`, `VehicleRouteId`, `VehicleRouteRevision`) REFERENCES `vehicleroutestops` (`Id`, `VehicleRouteId`, `VehicleRouteRevision`) ON DELETE CASCADE,
  CONSTRAINT `FK_VehicleRouteLegs_OriginStops` FOREIGN KEY (`OriginStopId`, `VehicleRouteId`, `VehicleRouteRevision`) REFERENCES `vehicleroutestops` (`Id`, `VehicleRouteId`, `VehicleRouteRevision`) ON DELETE CASCADE,
  CONSTRAINT `FK_VehicleRouteLegs_VehicleRoutes` FOREIGN KEY (`VehicleRouteId`, `VehicleRouteRevision`) REFERENCES `vehicleroutes` (`Id`, `RevisionNumber`) ON DELETE CASCADE,
  CONSTRAINT `CK_VehicleRouteLegs_Distance` CHECK ((`RouteDistanceMetres` >= 0)),
  CONSTRAINT `CK_VehicleRouteLegs_DistinctStops` CHECK ((`OriginStopId` <> `DestinationStopId`)),
  CONSTRAINT `CK_VehicleRouteLegs_RoomEquivalentCost` CHECK ((`RoomEquivalentCost` >= 0)),
  CONSTRAINT `CK_VehicleRouteLegs_Sequence` CHECK ((`Sequence` >= 0))
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Dumping data for table vehicleroutelegs
--

/*!40000 ALTER TABLE `vehicleroutelegs` DISABLE KEYS */;
/*!40000 ALTER TABLE `vehicleroutelegs` ENABLE KEYS */;

--
-- Definition of vehicleroutesteps
--

DROP TABLE IF EXISTS `vehicleroutesteps`;
CREATE TABLE IF NOT EXISTS `vehicleroutesteps` (
  `Id` bigint NOT NULL AUTO_INCREMENT,
  `VehicleRouteLegId` bigint NOT NULL,
  `Sequence` int NOT NULL,
  `StepType` int NOT NULL,
  `OriginCellId` bigint NOT NULL,
  `OriginRoomLayer` int NOT NULL,
  `OriginRoutePositionMetres` decimal(18,3) DEFAULT NULL,
  `DestinationCellId` bigint NOT NULL,
  `DestinationRoomLayer` int NOT NULL,
  `DestinationRoutePositionMetres` decimal(18,3) DEFAULT NULL,
  `DistanceMetres` decimal(18,3) DEFAULT NULL,
  `RoomEquivalentCost` decimal(18,6) NOT NULL,
  `Direction` int DEFAULT NULL,
  `PinnedTopologyVersion` bigint DEFAULT NULL,
  `DestinationTopologyVersion` bigint DEFAULT NULL,
  `ExitId` bigint DEFAULT NULL,
  PRIMARY KEY (`Id`),
  UNIQUE KEY `UX_VehicleRouteSteps_Leg_Sequence` (`VehicleRouteLegId`,`Sequence`),
  KEY `FK_VehicleRouteSteps_DestinationCells_idx` (`DestinationCellId`),
  KEY `FK_VehicleRouteSteps_Exits_idx` (`ExitId`),
  KEY `FK_VehicleRouteSteps_OriginCells_idx` (`OriginCellId`),
  CONSTRAINT `FK_VehicleRouteSteps_DestinationCells` FOREIGN KEY (`DestinationCellId`) REFERENCES `cells` (`Id`) ON DELETE RESTRICT,
  CONSTRAINT `FK_VehicleRouteSteps_Exits` FOREIGN KEY (`ExitId`) REFERENCES `exits` (`Id`) ON DELETE RESTRICT,
  CONSTRAINT `FK_VehicleRouteSteps_OriginCells` FOREIGN KEY (`OriginCellId`) REFERENCES `cells` (`Id`) ON DELETE RESTRICT,
  CONSTRAINT `FK_VehicleRouteSteps_VehicleRouteLegs` FOREIGN KEY (`VehicleRouteLegId`) REFERENCES `vehicleroutelegs` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `CK_VehicleRouteSteps_Positions` CHECK ((((`OriginRoutePositionMetres` is null) or (`OriginRoutePositionMetres` >= 0)) and ((`DestinationRoutePositionMetres` is null) or (`DestinationRoutePositionMetres` >= 0)) and (((`OriginRoutePositionMetres` is null) and (`PinnedTopologyVersion` is null)) or ((`OriginRoutePositionMetres` is not null) and (`PinnedTopologyVersion` is not null) and (`PinnedTopologyVersion` >= 1))) and (((`DestinationRoutePositionMetres` is null) and (`DestinationTopologyVersion` is null)) or ((`DestinationRoutePositionMetres` is not null) and (`DestinationTopologyVersion` is not null) and (`DestinationTopologyVersion` >= 1))))),
  CONSTRAINT `CK_VehicleRouteSteps_RoomEquivalentCost` CHECK ((`RoomEquivalentCost` >= 0)),
  CONSTRAINT `CK_VehicleRouteSteps_Sequence` CHECK ((`Sequence` >= 0)),
  CONSTRAINT `CK_VehicleRouteSteps_TypedPayload` CHECK ((((`StepType` = 0) and (`ExitId` is null) and (`Direction` is not null) and (`Direction` in (-(1),1)) and (`PinnedTopologyVersion` is not null) and (`DestinationTopologyVersion` = `PinnedTopologyVersion`) and (`DistanceMetres` is not null) and (`DistanceMetres` >= 0) and (`OriginRoutePositionMetres` is not null) and (`DestinationRoutePositionMetres` is not null) and (`OriginCellId` = `DestinationCellId`) and (`OriginRoomLayer` = `DestinationRoomLayer`)) or ((`StepType` = 1) and (`ExitId` is not null) and (`Direction` is null) and (`DistanceMetres` is null))))
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Dumping data for table vehicleroutesteps
--

/*!40000 ALTER TABLE `vehicleroutesteps` DISABLE KEYS */;
/*!40000 ALTER TABLE `vehicleroutesteps` ENABLE KEYS */;

--
-- Definition of vehicleroutestops
--

DROP TABLE IF EXISTS `vehicleroutestops`;
CREATE TABLE IF NOT EXISTS `vehicleroutestops` (
  `Id` bigint NOT NULL AUTO_INCREMENT,
  `VehicleRouteId` bigint NOT NULL,
  `VehicleRouteRevision` int NOT NULL,
  `Name` varchar(200) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL,
  `Sequence` int NOT NULL,
  `CellId` bigint NOT NULL,
  `RoomLayer` int NOT NULL,
  `RoutePositionMetres` decimal(18,3) DEFAULT NULL,
  `DwellDurationMilliseconds` bigint NOT NULL DEFAULT '0',
  PRIMARY KEY (`Id`),
  UNIQUE KEY `AK_VehicleRouteStops_Id_Route` (`Id`,`VehicleRouteId`,`VehicleRouteRevision`),
  UNIQUE KEY `UX_VehicleRouteStops_Route_Sequence` (`VehicleRouteId`,`VehicleRouteRevision`,`Sequence`),
  KEY `FK_VehicleRouteStops_Cells_idx` (`CellId`),
  CONSTRAINT `FK_VehicleRouteStops_Cells` FOREIGN KEY (`CellId`) REFERENCES `cells` (`Id`) ON DELETE RESTRICT,
  CONSTRAINT `FK_VehicleRouteStops_VehicleRoutes` FOREIGN KEY (`VehicleRouteId`, `VehicleRouteRevision`) REFERENCES `vehicleroutes` (`Id`, `RevisionNumber`) ON DELETE CASCADE,
  CONSTRAINT `CK_VehicleRouteStops_Dwell` CHECK ((`DwellDurationMilliseconds` >= 0)),
  CONSTRAINT `CK_VehicleRouteStops_RoutePosition` CHECK (((`RoutePositionMetres` is null) or (`RoutePositionMetres` >= 0))),
  CONSTRAINT `CK_VehicleRouteStops_Sequence` CHECK ((`Sequence` >= 0))
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Dumping data for table vehicleroutestops
--

/*!40000 ALTER TABLE `vehicleroutestops` DISABLE KEYS */;
/*!40000 ALTER TABLE `vehicleroutestops` ENABLE KEYS */;

--
-- Definition of vehiclerouteplatformbindings
--

DROP TABLE IF EXISTS `vehiclerouteplatformbindings`;
CREATE TABLE IF NOT EXISTS `vehiclerouteplatformbindings` (
  `Id` bigint NOT NULL AUTO_INCREMENT,
  `VehicleRouteStopId` bigint NOT NULL,
  `PlatformCellId` bigint NOT NULL,
  `VehicleAccessPointProtoId` bigint NOT NULL,
  `DockingToleranceMetres` decimal(18,3) NOT NULL DEFAULT '2.000',
  PRIMARY KEY (`Id`),
  UNIQUE KEY `UX_VehicleRoutePlatformBindings_Stop_Platform_AccessPoint` (`VehicleRouteStopId`,`PlatformCellId`,`VehicleAccessPointProtoId`),
  KEY `FK_VehicleRoutePlatformBindings_AccessPointProtos_idx` (`VehicleAccessPointProtoId`),
  KEY `FK_VehicleRoutePlatformBindings_Cells_idx` (`PlatformCellId`),
  CONSTRAINT `FK_VehicleRoutePlatformBindings_AccessPointProtos` FOREIGN KEY (`VehicleAccessPointProtoId`) REFERENCES `vehicleaccesspointprotos` (`Id`) ON DELETE RESTRICT,
  CONSTRAINT `FK_VehicleRoutePlatformBindings_Cells` FOREIGN KEY (`PlatformCellId`) REFERENCES `cells` (`Id`) ON DELETE RESTRICT,
  CONSTRAINT `FK_VehicleRoutePlatformBindings_VehicleRouteStops` FOREIGN KEY (`VehicleRouteStopId`) REFERENCES `vehicleroutestops` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `CK_VehicleRoutePlatformBindings_Tolerance` CHECK ((`DockingToleranceMetres` >= 0))
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Dumping data for table vehiclerouteplatformbindings
--

/*!40000 ALTER TABLE `vehiclerouteplatformbindings` DISABLE KEYS */;
/*!40000 ALTER TABLE `vehiclerouteplatformbindings` ENABLE KEYS */;

--
-- Definition of vehicleroutetopologypins
--

DROP TABLE IF EXISTS `vehicleroutetopologypins`;
CREATE TABLE IF NOT EXISTS `vehicleroutetopologypins` (
  `VehicleRouteId` bigint NOT NULL,
  `VehicleRouteRevision` int NOT NULL,
  `RouteCellId` bigint NOT NULL,
  `TopologyVersion` bigint NOT NULL,
  PRIMARY KEY (`VehicleRouteId`,`VehicleRouteRevision`,`RouteCellId`),
  KEY `FK_VehicleRouteTopologyPins_RouteCells_idx` (`RouteCellId`),
  CONSTRAINT `FK_VehicleRouteTopologyPins_RouteCells` FOREIGN KEY (`RouteCellId`) REFERENCES `routecells` (`CellId`) ON DELETE RESTRICT,
  CONSTRAINT `FK_VehicleRouteTopologyPins_VehicleRoutes` FOREIGN KEY (`VehicleRouteId`, `VehicleRouteRevision`) REFERENCES `vehicleroutes` (`Id`, `RevisionNumber`) ON DELETE CASCADE,
  CONSTRAINT `CK_VehicleRouteTopologyPins_Version` CHECK ((`TopologyVersion` >= 1))
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Dumping data for table vehicleroutetopologypins
--

/*!40000 ALTER TABLE `vehicleroutetopologypins` DISABLE KEYS */;
/*!40000 ALTER TABLE `vehicleroutetopologypins` ENABLE KEYS */;

--
-- Definition of vehicles
--

DROP TABLE IF EXISTS `vehicles`;
CREATE TABLE IF NOT EXISTS `vehicles` (
  `Id` bigint NOT NULL AUTO_INCREMENT,
  `VehicleProtoId` bigint NOT NULL,
  `VehicleProtoRevision` int NOT NULL,
  `Name` varchar(200) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `ExteriorItemId` bigint DEFAULT NULL,
  `LocationType` int NOT NULL,
  `CurrentCellId` bigint DEFAULT NULL,
  `CurrentRoomLayer` int NOT NULL,
  `MovementStatus` int NOT NULL,
  `CurrentExitId` bigint DEFAULT NULL,
  `DestinationCellId` bigint DEFAULT NULL,
  `MovementProfileProtoId` bigint DEFAULT NULL,
  `CreatedDateTime` datetime NOT NULL,
  `LastMovementDateTime` datetime DEFAULT NULL,
  `ActivePropulsionProfileProtoId` bigint DEFAULT NULL,
  `CurrentRoutePosition` decimal(18,3) DEFAULT NULL,
  PRIMARY KEY (`Id`),
  UNIQUE KEY `FK_Vehicles_GameItems_Exterior_idx` (`ExteriorItemId`),
  KEY `FK_Vehicles_Cells_Current_idx` (`CurrentCellId`),
  KEY `FK_Vehicles_Cells_Destination_idx` (`DestinationCellId`),
  KEY `FK_Vehicles_Exits_idx` (`CurrentExitId`),
  KEY `FK_Vehicles_MovementProfileProtos_idx` (`MovementProfileProtoId`),
  KEY `FK_Vehicles_VehicleProtos_idx` (`VehicleProtoId`,`VehicleProtoRevision`),
  KEY `FK_Vehicles_PropulsionProfileProtos_idx` (`ActivePropulsionProfileProtoId`),
  KEY `IX_Vehicles_Cell_Layer_RoutePosition` (`CurrentCellId`,`CurrentRoomLayer`,`CurrentRoutePosition`),
  CONSTRAINT `FK_Vehicles_Cells_Current` FOREIGN KEY (`CurrentCellId`) REFERENCES `cells` (`Id`) ON DELETE SET NULL,
  CONSTRAINT `FK_Vehicles_Cells_Destination` FOREIGN KEY (`DestinationCellId`) REFERENCES `cells` (`Id`) ON DELETE SET NULL,
  CONSTRAINT `FK_Vehicles_Exits` FOREIGN KEY (`CurrentExitId`) REFERENCES `exits` (`Id`) ON DELETE SET NULL,
  CONSTRAINT `FK_Vehicles_GameItems_Exterior` FOREIGN KEY (`ExteriorItemId`) REFERENCES `gameitems` (`Id`) ON DELETE SET NULL,
  CONSTRAINT `FK_Vehicles_MovementProfileProtos` FOREIGN KEY (`MovementProfileProtoId`) REFERENCES `vehiclemovementprofileprotos` (`Id`) ON DELETE SET NULL,
  CONSTRAINT `FK_Vehicles_PropulsionProfileProtos` FOREIGN KEY (`ActivePropulsionProfileProtoId`) REFERENCES `vehiclepropulsionprofileprotos` (`Id`) ON DELETE SET NULL,
  CONSTRAINT `FK_Vehicles_VehicleProtos` FOREIGN KEY (`VehicleProtoId`, `VehicleProtoRevision`) REFERENCES `vehicleprotos` (`Id`, `RevisionNumber`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Dumping data for table vehicles
--

/*!40000 ALTER TABLE `vehicles` DISABLE KEYS */;
/*!40000 ALTER TABLE `vehicles` ENABLE KEYS */;

--
-- Definition of tracks
--

DROP TABLE IF EXISTS `tracks`;
CREATE TABLE IF NOT EXISTS `tracks` (
  `Id` bigint NOT NULL AUTO_INCREMENT,
  `CharacterId` bigint DEFAULT NULL,
  `BodyPrototypeId` bigint DEFAULT NULL,
  `CellId` bigint NOT NULL,
  `RoomLayer` int NOT NULL,
  `MudDateTime` varchar(100) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci DEFAULT NULL,
  `FromDirectionExitId` bigint DEFAULT NULL,
  `ToDirectionExitId` bigint DEFAULT NULL,
  `TrackCircumstances` int NOT NULL,
  `FromMoveSpeedId` bigint DEFAULT NULL,
  `ToMoveSpeedId` bigint DEFAULT NULL,
  `ExertionLevel` int NOT NULL,
  `TrackIntensityVisual` double NOT NULL,
  `TrackIntensityOlfactory` double NOT NULL,
  `TurnedAround` bit(1) NOT NULL,
  `RouteDirection` int DEFAULT NULL,
  `RoutePosition` decimal(18,3) DEFAULT NULL,
  `VehicleId` bigint DEFAULT NULL,
  PRIMARY KEY (`Id`),
  KEY `IX_Tracks_BodyPrototypeId` (`BodyPrototypeId`),
  KEY `IX_Tracks_CharacterId` (`CharacterId`),
  KEY `IX_Tracks_FromDirectionExitId` (`FromDirectionExitId`),
  KEY `IX_Tracks_FromMoveSpeedId` (`FromMoveSpeedId`),
  KEY `IX_Tracks_ToDirectionExitId` (`ToDirectionExitId`),
  KEY `IX_Tracks_ToMoveSpeedId` (`ToMoveSpeedId`),
  KEY `IX_Tracks_Cell_Layer_RoutePosition` (`CellId`,`RoomLayer`,`RoutePosition`),
  KEY `FK_Tracks_Vehicles_idx` (`VehicleId`),
  CONSTRAINT `FK_Tracks_BodyProtos` FOREIGN KEY (`BodyPrototypeId`) REFERENCES `bodyprotos` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_Tracks_Cells` FOREIGN KEY (`CellId`) REFERENCES `cells` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_Tracks_Characters` FOREIGN KEY (`CharacterId`) REFERENCES `characters` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_Tracks_Exits_From` FOREIGN KEY (`FromDirectionExitId`) REFERENCES `exits` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_Tracks_Exits_To` FOREIGN KEY (`ToDirectionExitId`) REFERENCES `exits` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_Tracks_MoveSpeeds_From` FOREIGN KEY (`FromMoveSpeedId`) REFERENCES `movespeeds` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_Tracks_MoveSpeeds_To` FOREIGN KEY (`ToMoveSpeedId`) REFERENCES `movespeeds` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_Tracks_Vehicles` FOREIGN KEY (`VehicleId`) REFERENCES `vehicles` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `CK_Tracks_Owner` CHECK ((((`VehicleId` is null) and (`CharacterId` is not null) and (`BodyPrototypeId` is not null)) or ((`VehicleId` is not null) and (`CharacterId` is null) and (`BodyPrototypeId` is null))))
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Dumping data for table tracks
--

/*!40000 ALTER TABLE `tracks` DISABLE KEYS */;
/*!40000 ALTER TABLE `tracks` ENABLE KEYS */;

--
-- Definition of vehicleaccesspoints
--

DROP TABLE IF EXISTS `vehicleaccesspoints`;
CREATE TABLE IF NOT EXISTS `vehicleaccesspoints` (
  `Id` bigint NOT NULL AUTO_INCREMENT,
  `VehicleId` bigint NOT NULL,
  `VehicleAccessPointProtoId` bigint NOT NULL,
  `Name` varchar(200) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `ProjectionItemId` bigint DEFAULT NULL,
  `IsOpen` bit(1) NOT NULL,
  `IsDisabled` bit(1) NOT NULL,
  PRIMARY KEY (`Id`),
  UNIQUE KEY `FK_VehicleAccessPoints_GameItems_idx` (`ProjectionItemId`),
  KEY `FK_VehicleAccessPoints_Protos_idx` (`VehicleAccessPointProtoId`),
  KEY `FK_VehicleAccessPoints_Vehicles_idx` (`VehicleId`),
  CONSTRAINT `FK_VehicleAccessPoints_GameItems` FOREIGN KEY (`ProjectionItemId`) REFERENCES `gameitems` (`Id`) ON DELETE SET NULL,
  CONSTRAINT `FK_VehicleAccessPoints_Protos` FOREIGN KEY (`VehicleAccessPointProtoId`) REFERENCES `vehicleaccesspointprotos` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_VehicleAccessPoints_Vehicles` FOREIGN KEY (`VehicleId`) REFERENCES `vehicles` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Dumping data for table vehicleaccesspoints
--

/*!40000 ALTER TABLE `vehicleaccesspoints` DISABLE KEYS */;
/*!40000 ALTER TABLE `vehicleaccesspoints` ENABLE KEYS */;

--
-- Definition of vehicleaccessstates
--

DROP TABLE IF EXISTS `vehicleaccessstates`;
CREATE TABLE IF NOT EXISTS `vehicleaccessstates` (
  `Id` bigint NOT NULL AUTO_INCREMENT,
  `VehicleId` bigint NOT NULL,
  `CharacterId` bigint DEFAULT NULL,
  `AccessTag` varchar(200) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `AccessLevel` int NOT NULL,
  PRIMARY KEY (`Id`),
  KEY `FK_VehicleAccessStates_Characters_idx` (`CharacterId`),
  KEY `FK_VehicleAccessStates_Vehicles_idx` (`VehicleId`),
  CONSTRAINT `FK_VehicleAccessStates_Characters` FOREIGN KEY (`CharacterId`) REFERENCES `characters` (`Id`) ON DELETE SET NULL,
  CONSTRAINT `FK_VehicleAccessStates_Vehicles` FOREIGN KEY (`VehicleId`) REFERENCES `vehicles` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Dumping data for table vehicleaccessstates
--

/*!40000 ALTER TABLE `vehicleaccessstates` DISABLE KEYS */;
/*!40000 ALTER TABLE `vehicleaccessstates` ENABLE KEYS */;

--
-- Definition of vehiclecargospaces
--

DROP TABLE IF EXISTS `vehiclecargospaces`;
CREATE TABLE IF NOT EXISTS `vehiclecargospaces` (
  `Id` bigint NOT NULL AUTO_INCREMENT,
  `VehicleId` bigint NOT NULL,
  `VehicleCargoSpaceProtoId` bigint NOT NULL,
  `Name` varchar(200) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `ProjectionItemId` bigint DEFAULT NULL,
  `IsDisabled` bit(1) NOT NULL,
  PRIMARY KEY (`Id`),
  UNIQUE KEY `FK_VehicleCargoSpaces_GameItems_idx` (`ProjectionItemId`),
  KEY `FK_VehicleCargoSpaces_Protos_idx` (`VehicleCargoSpaceProtoId`),
  KEY `FK_VehicleCargoSpaces_Vehicles_idx` (`VehicleId`),
  CONSTRAINT `FK_VehicleCargoSpaces_GameItems` FOREIGN KEY (`ProjectionItemId`) REFERENCES `gameitems` (`Id`) ON DELETE SET NULL,
  CONSTRAINT `FK_VehicleCargoSpaces_Protos` FOREIGN KEY (`VehicleCargoSpaceProtoId`) REFERENCES `vehiclecargospaceprotos` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_VehicleCargoSpaces_Vehicles` FOREIGN KEY (`VehicleId`) REFERENCES `vehicles` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Dumping data for table vehiclecargospaces
--

/*!40000 ALTER TABLE `vehiclecargospaces` DISABLE KEYS */;
/*!40000 ALTER TABLE `vehiclecargospaces` ENABLE KEYS */;

--
-- Definition of vehiclecompartments
--

DROP TABLE IF EXISTS `vehiclecompartments`;
CREATE TABLE IF NOT EXISTS `vehiclecompartments` (
  `Id` bigint NOT NULL AUTO_INCREMENT,
  `VehicleId` bigint NOT NULL,
  `VehicleCompartmentProtoId` bigint NOT NULL,
  `Name` varchar(200) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `InteriorCellId` bigint DEFAULT NULL,
  PRIMARY KEY (`Id`),
  UNIQUE KEY `UX_VehicleCompartments_InteriorCell` (`InteriorCellId`),
  KEY `FK_VehicleCompartments_Protos_idx` (`VehicleCompartmentProtoId`),
  KEY `FK_VehicleCompartments_Vehicles_idx` (`VehicleId`),
  CONSTRAINT `FK_VehicleCompartments_InteriorCells` FOREIGN KEY (`InteriorCellId`) REFERENCES `cells` (`Id`) ON DELETE RESTRICT,
  CONSTRAINT `FK_VehicleCompartments_Protos` FOREIGN KEY (`VehicleCompartmentProtoId`) REFERENCES `vehiclecompartmentprotos` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_VehicleCompartments_Vehicles` FOREIGN KEY (`VehicleId`) REFERENCES `vehicles` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Dumping data for table vehiclecompartments
--

/*!40000 ALTER TABLE `vehiclecompartments` DISABLE KEYS */;
/*!40000 ALTER TABLE `vehiclecompartments` ENABLE KEYS */;

--
-- Definition of vehicledamagezones
--

DROP TABLE IF EXISTS `vehicledamagezones`;
CREATE TABLE IF NOT EXISTS `vehicledamagezones` (
  `Id` bigint NOT NULL AUTO_INCREMENT,
  `VehicleId` bigint NOT NULL,
  `VehicleDamageZoneProtoId` bigint NOT NULL,
  `Name` varchar(200) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `CurrentDamage` double NOT NULL,
  `Status` int NOT NULL,
  PRIMARY KEY (`Id`),
  KEY `FK_VehicleDamageZones_Protos_idx` (`VehicleDamageZoneProtoId`),
  KEY `FK_VehicleDamageZones_Vehicles_idx` (`VehicleId`),
  CONSTRAINT `FK_VehicleDamageZones_Protos` FOREIGN KEY (`VehicleDamageZoneProtoId`) REFERENCES `vehicledamagezoneprotos` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_VehicleDamageZones_Vehicles` FOREIGN KEY (`VehicleId`) REFERENCES `vehicles` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Dumping data for table vehicledamagezones
--

/*!40000 ALTER TABLE `vehicledamagezones` DISABLE KEYS */;
/*!40000 ALTER TABLE `vehicledamagezones` ENABLE KEYS */;

--
-- Definition of vehicledockings
--

DROP TABLE IF EXISTS `vehicledockings`;
CREATE TABLE IF NOT EXISTS `vehicledockings` (
  `Id` bigint NOT NULL AUTO_INCREMENT,
  `VehicleId` bigint NOT NULL,
  `VehicleAccessPointId` bigint NOT NULL,
  `VehicleCompartmentId` bigint NOT NULL,
  `ExteriorCellId` bigint NOT NULL,
  `ExteriorRoomLayer` int NOT NULL,
  `State` int NOT NULL DEFAULT '0',
  `VehicleRouteStopId` bigint DEFAULT NULL,
  PRIMARY KEY (`Id`),
  UNIQUE KEY `UX_VehicleDockings_AccessPoint` (`VehicleAccessPointId`),
  KEY `FK_VehicleDockings_Compartments_idx` (`VehicleCompartmentId`),
  KEY `IX_VehicleDockings_ExteriorCell_Layer` (`ExteriorCellId`,`ExteriorRoomLayer`),
  KEY `IX_VehicleDockings_Vehicle_State` (`VehicleId`,`State`),
  KEY `FK_VehicleDockings_VehicleRouteStops_idx` (`VehicleRouteStopId`),
  CONSTRAINT `FK_VehicleDockings_AccessPoints` FOREIGN KEY (`VehicleAccessPointId`) REFERENCES `vehicleaccesspoints` (`Id`) ON DELETE RESTRICT,
  CONSTRAINT `FK_VehicleDockings_Compartments` FOREIGN KEY (`VehicleCompartmentId`) REFERENCES `vehiclecompartments` (`Id`) ON DELETE RESTRICT,
  CONSTRAINT `FK_VehicleDockings_ExteriorCells` FOREIGN KEY (`ExteriorCellId`) REFERENCES `cells` (`Id`) ON DELETE RESTRICT,
  CONSTRAINT `FK_VehicleDockings_VehicleRouteStops` FOREIGN KEY (`VehicleRouteStopId`) REFERENCES `vehicleroutestops` (`Id`) ON DELETE RESTRICT,
  CONSTRAINT `FK_VehicleDockings_Vehicles` FOREIGN KEY (`VehicleId`) REFERENCES `vehicles` (`Id`) ON DELETE RESTRICT
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Dumping data for table vehicledockings
--

/*!40000 ALTER TABLE `vehicledockings` DISABLE KEYS */;
/*!40000 ALTER TABLE `vehicledockings` ENABLE KEYS */;

--
-- Definition of vehiclehitchlinks
--

DROP TABLE IF EXISTS `vehiclehitchlinks`;
CREATE TABLE IF NOT EXISTS `vehiclehitchlinks` (
  `Id` bigint NOT NULL AUTO_INCREMENT,
  `SourceType` int NOT NULL,
  `SourceVehicleId` bigint DEFAULT NULL,
  `SourceCharacterId` bigint DEFAULT NULL,
  `SourceTowPointProtoId` bigint DEFAULT NULL,
  `TargetType` int NOT NULL,
  `TargetVehicleId` bigint DEFAULT NULL,
  `TargetCharacterId` bigint DEFAULT NULL,
  `TargetTowPointProtoId` bigint DEFAULT NULL,
  `HitchItemId` bigint DEFAULT NULL,
  `IsDisabled` bit(1) NOT NULL,
  `CreatedDateTime` datetime NOT NULL,
  `SourceCharacterInstanceId` bigint DEFAULT NULL,
  `TargetCharacterInstanceId` bigint DEFAULT NULL,
  PRIMARY KEY (`Id`),
  KEY `FK_VehicleHitchLinks_GameItems_idx` (`HitchItemId`),
  KEY `FK_VehicleHitchLinks_SourceCharacters_idx` (`SourceCharacterId`),
  KEY `FK_VehicleHitchLinks_SourceTowPointProtos_idx` (`SourceTowPointProtoId`),
  KEY `FK_VehicleHitchLinks_SourceVehicles_idx` (`SourceVehicleId`),
  KEY `FK_VehicleHitchLinks_TargetCharacters_idx` (`TargetCharacterId`),
  KEY `FK_VehicleHitchLinks_TargetTowPointProtos_idx` (`TargetTowPointProtoId`),
  KEY `FK_VehicleHitchLinks_TargetVehicles_idx` (`TargetVehicleId`),
  KEY `FK_VehicleHitchLinks_SourceCharacterInstances_idx` (`SourceCharacterInstanceId`),
  KEY `FK_VehicleHitchLinks_TargetCharacterInstances_idx` (`TargetCharacterInstanceId`),
  CONSTRAINT `FK_VehicleHitchLinks_GameItems` FOREIGN KEY (`HitchItemId`) REFERENCES `gameitems` (`Id`) ON DELETE SET NULL,
  CONSTRAINT `FK_VehicleHitchLinks_SourceCharacters` FOREIGN KEY (`SourceCharacterId`) REFERENCES `characters` (`Id`) ON DELETE SET NULL,
  CONSTRAINT `FK_VehicleHitchLinks_SourceTowPointProtos` FOREIGN KEY (`SourceTowPointProtoId`) REFERENCES `vehicletowpointprotos` (`Id`) ON DELETE SET NULL,
  CONSTRAINT `FK_VehicleHitchLinks_SourceVehicles` FOREIGN KEY (`SourceVehicleId`) REFERENCES `vehicles` (`Id`) ON DELETE SET NULL,
  CONSTRAINT `FK_VehicleHitchLinks_TargetCharacters` FOREIGN KEY (`TargetCharacterId`) REFERENCES `characters` (`Id`) ON DELETE SET NULL,
  CONSTRAINT `FK_VehicleHitchLinks_TargetTowPointProtos` FOREIGN KEY (`TargetTowPointProtoId`) REFERENCES `vehicletowpointprotos` (`Id`) ON DELETE SET NULL,
  CONSTRAINT `FK_VehicleHitchLinks_TargetVehicles` FOREIGN KEY (`TargetVehicleId`) REFERENCES `vehicles` (`Id`) ON DELETE SET NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Dumping data for table vehiclehitchlinks
--

/*!40000 ALTER TABLE `vehiclehitchlinks` DISABLE KEYS */;
/*!40000 ALTER TABLE `vehiclehitchlinks` ENABLE KEYS */;

--
-- Definition of vehicleinstallations
--

DROP TABLE IF EXISTS `vehicleinstallations`;
CREATE TABLE IF NOT EXISTS `vehicleinstallations` (
  `Id` bigint NOT NULL AUTO_INCREMENT,
  `VehicleId` bigint NOT NULL,
  `VehicleInstallationPointProtoId` bigint NOT NULL,
  `InstalledItemId` bigint DEFAULT NULL,
  `IsDisabled` bit(1) NOT NULL,
  PRIMARY KEY (`Id`),
  UNIQUE KEY `FK_VehicleInstallations_GameItems_idx` (`InstalledItemId`),
  KEY `FK_VehicleInstallations_Protos_idx` (`VehicleInstallationPointProtoId`),
  KEY `FK_VehicleInstallations_Vehicles_idx` (`VehicleId`),
  CONSTRAINT `FK_VehicleInstallations_GameItems` FOREIGN KEY (`InstalledItemId`) REFERENCES `gameitems` (`Id`) ON DELETE SET NULL,
  CONSTRAINT `FK_VehicleInstallations_Protos` FOREIGN KEY (`VehicleInstallationPointProtoId`) REFERENCES `vehicleinstallationpointprotos` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_VehicleInstallations_Vehicles` FOREIGN KEY (`VehicleId`) REFERENCES `vehicles` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Dumping data for table vehicleinstallations
--

/*!40000 ALTER TABLE `vehicleinstallations` DISABLE KEYS */;
/*!40000 ALTER TABLE `vehicleinstallations` ENABLE KEYS */;

--
-- Definition of vehicleoccupancies
--

DROP TABLE IF EXISTS `vehicleoccupancies`;
CREATE TABLE IF NOT EXISTS `vehicleoccupancies` (
  `Id` bigint NOT NULL AUTO_INCREMENT,
  `VehicleId` bigint NOT NULL,
  `CharacterId` bigint NOT NULL,
  `VehicleOccupantSlotProtoId` bigint NOT NULL,
  `IsController` bit(1) NOT NULL,
  `CharacterInstanceId` bigint DEFAULT NULL,
  `CharacterInstanceKey` bigint GENERATED ALWAYS AS (coalesce(`CharacterInstanceId`,0)) STORED,
  PRIMARY KEY (`Id`),
  UNIQUE KEY `IX_VehicleOccupancies_Vehicle_Character_Instance` (`VehicleId`,`CharacterId`,`CharacterInstanceKey`),
  KEY `FK_VehicleOccupancies_Characters_idx` (`CharacterId`),
  KEY `FK_VehicleOccupancies_Slots_idx` (`VehicleOccupantSlotProtoId`),
  KEY `FK_VehicleOccupancies_Vehicles_idx` (`VehicleId`),
  KEY `FK_VehicleOccupancies_CharacterInstances_idx` (`CharacterInstanceId`),
  CONSTRAINT `FK_VehicleOccupancies_Characters` FOREIGN KEY (`CharacterId`) REFERENCES `characters` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_VehicleOccupancies_Slots` FOREIGN KEY (`VehicleOccupantSlotProtoId`) REFERENCES `vehicleoccupantslotprotos` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_VehicleOccupancies_Vehicles` FOREIGN KEY (`VehicleId`) REFERENCES `vehicles` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Dumping data for table vehicleoccupancies
--

/*!40000 ALTER TABLE `vehicleoccupancies` DISABLE KEYS */;
/*!40000 ALTER TABLE `vehicleoccupancies` ENABLE KEYS */;

--
-- Definition of vehicleservices
--

DROP TABLE IF EXISTS `vehicleservices`;
CREATE TABLE IF NOT EXISTS `vehicleservices` (
  `Id` bigint NOT NULL AUTO_INCREMENT,
  `Name` varchar(200) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL,
  `Keywords` varchar(1000) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL,
  `VehicleRouteId` bigint NOT NULL,
  `VehicleRouteRevision` int NOT NULL,
  `VehicleId` bigint NOT NULL,
  `OperatorMode` int NOT NULL,
  `RetryIntervalMilliseconds` bigint NOT NULL DEFAULT '30000',
  `MaximumHoldMilliseconds` bigint NOT NULL DEFAULT '900000',
  `Enabled` bit(1) NOT NULL DEFAULT b'1',
  PRIMARY KEY (`Id`),
  KEY `FK_VehicleServices_VehicleRoutes_idx` (`VehicleRouteId`,`VehicleRouteRevision`),
  KEY `IX_VehicleServices_Name` (`Name`),
  KEY `IX_VehicleServices_Vehicle_Enabled` (`VehicleId`,`Enabled`),
  CONSTRAINT `FK_VehicleServices_VehicleRoutes` FOREIGN KEY (`VehicleRouteId`, `VehicleRouteRevision`) REFERENCES `vehicleroutes` (`Id`, `RevisionNumber`) ON DELETE RESTRICT,
  CONSTRAINT `FK_VehicleServices_Vehicles` FOREIGN KEY (`VehicleId`) REFERENCES `vehicles` (`Id`) ON DELETE RESTRICT,
  CONSTRAINT `CK_VehicleServices_MaximumHold` CHECK ((`MaximumHoldMilliseconds` >= 0)),
  CONSTRAINT `CK_VehicleServices_OperatorMode` CHECK ((`OperatorMode` in (0,1))),
  CONSTRAINT `CK_VehicleServices_RetryInterval` CHECK ((`RetryIntervalMilliseconds` > 0))
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Dumping data for table vehicleservices
--

/*!40000 ALTER TABLE `vehicleservices` DISABLE KEYS */;
/*!40000 ALTER TABLE `vehicleservices` ENABLE KEYS */;

--
-- Definition of vehiclejourneys
--

DROP TABLE IF EXISTS `vehiclejourneys`;
CREATE TABLE IF NOT EXISTS `vehiclejourneys` (
  `Id` bigint NOT NULL AUTO_INCREMENT,
  `OperationId` varchar(64) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL,
  `VehicleServiceId` bigint NOT NULL,
  `VehicleRouteId` bigint NOT NULL,
  `VehicleRouteRevision` int NOT NULL,
  `VehicleId` bigint NOT NULL,
  `State` int NOT NULL DEFAULT '0',
  `CurrentStopId` bigint DEFAULT NULL,
  `NextStopId` bigint DEFAULT NULL,
  `ScheduledDeparture` varchar(500) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `ExpectedDeparture` varchar(500) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `DelayMilliseconds` bigint NOT NULL DEFAULT '0',
  `LastCheckpointUtc` datetime(6) NOT NULL,
  PRIMARY KEY (`Id`),
  UNIQUE KEY `UX_VehicleJourneys_Operation` (`OperationId`),
  UNIQUE KEY `UX_VehicleJourneys_Service_ScheduledDeparture` (`VehicleServiceId`,`ScheduledDeparture`),
  KEY `FK_VehicleJourneys_CurrentStops_idx` (`CurrentStopId`,`VehicleRouteId`,`VehicleRouteRevision`),
  KEY `FK_VehicleJourneys_NextStops_idx` (`NextStopId`,`VehicleRouteId`,`VehicleRouteRevision`),
  KEY `FK_VehicleJourneys_VehicleRoutes_idx` (`VehicleRouteId`,`VehicleRouteRevision`),
  KEY `FK_VehicleJourneys_Vehicles_idx` (`VehicleId`),
  KEY `FK_VehicleJourneys_VehicleServices_idx` (`VehicleServiceId`),
  KEY `IX_VehicleJourneys_Service_State` (`VehicleServiceId`,`State`),
  CONSTRAINT `FK_VehicleJourneys_CurrentStops` FOREIGN KEY (`CurrentStopId`, `VehicleRouteId`, `VehicleRouteRevision`) REFERENCES `vehicleroutestops` (`Id`, `VehicleRouteId`, `VehicleRouteRevision`) ON DELETE RESTRICT,
  CONSTRAINT `FK_VehicleJourneys_NextStops` FOREIGN KEY (`NextStopId`, `VehicleRouteId`, `VehicleRouteRevision`) REFERENCES `vehicleroutestops` (`Id`, `VehicleRouteId`, `VehicleRouteRevision`) ON DELETE RESTRICT,
  CONSTRAINT `FK_VehicleJourneys_VehicleRoutes` FOREIGN KEY (`VehicleRouteId`, `VehicleRouteRevision`) REFERENCES `vehicleroutes` (`Id`, `RevisionNumber`) ON DELETE RESTRICT,
  CONSTRAINT `FK_VehicleJourneys_Vehicles` FOREIGN KEY (`VehicleId`) REFERENCES `vehicles` (`Id`) ON DELETE RESTRICT,
  CONSTRAINT `FK_VehicleJourneys_VehicleServices` FOREIGN KEY (`VehicleServiceId`) REFERENCES `vehicleservices` (`Id`) ON DELETE RESTRICT,
  CONSTRAINT `CK_VehicleJourneys_Delay` CHECK ((`DelayMilliseconds` >= 0)),
  CONSTRAINT `CK_VehicleJourneys_State` CHECK ((`State` between 0 and 8))
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Dumping data for table vehiclejourneys
--

/*!40000 ALTER TABLE `vehiclejourneys` DISABLE KEYS */;
/*!40000 ALTER TABLE `vehiclejourneys` ENABLE KEYS */;

--
-- Definition of vehiclejourneyevents
--

DROP TABLE IF EXISTS `vehiclejourneyevents`;
CREATE TABLE IF NOT EXISTS `vehiclejourneyevents` (
  `Id` bigint NOT NULL AUTO_INCREMENT,
  `VehicleJourneyId` bigint NOT NULL,
  `Sequence` bigint NOT NULL,
  `IdempotencyKey` varchar(128) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL,
  `EventType` int NOT NULL,
  `State` int NOT NULL,
  `OccurredAtUtc` datetime(6) NOT NULL,
  `WorldTime` varchar(500) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci DEFAULT NULL,
  `Message` mediumtext CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL,
  PRIMARY KEY (`Id`),
  UNIQUE KEY `UX_VehicleJourneyEvents_Idempotency` (`IdempotencyKey`),
  UNIQUE KEY `UX_VehicleJourneyEvents_Journey_Sequence` (`VehicleJourneyId`,`Sequence`),
  CONSTRAINT `FK_VehicleJourneyEvents_VehicleJourneys` FOREIGN KEY (`VehicleJourneyId`) REFERENCES `vehiclejourneys` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `CK_VehicleJourneyEvents_EventType` CHECK ((`EventType` between 0 and 11)),
  CONSTRAINT `CK_VehicleJourneyEvents_Sequence` CHECK ((`Sequence` >= 0)),
  CONSTRAINT `CK_VehicleJourneyEvents_State` CHECK ((`State` between 0 and 8))
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Dumping data for table vehiclejourneyevents
--

/*!40000 ALTER TABLE `vehiclejourneyevents` DISABLE KEYS */;
/*!40000 ALTER TABLE `vehiclejourneyevents` ENABLE KEYS */;

--
-- Definition of vehicleserviceschedules
--

DROP TABLE IF EXISTS `vehicleserviceschedules`;
CREATE TABLE IF NOT EXISTS `vehicleserviceschedules` (
  `VehicleServiceId` bigint NOT NULL,
  `ReferenceDeparture` varchar(500) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `NextDeparture` varchar(500) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `RecurrenceType` int NOT NULL,
  `RecurrenceIntervalAmount` int NOT NULL,
  `RecurrenceModifier` int NOT NULL,
  `RecurrenceSecondaryModifier` int NOT NULL,
  `RecurrenceFallbackMode` int NOT NULL,
  PRIMARY KEY (`VehicleServiceId`),
  CONSTRAINT `FK_VehicleServiceSchedules_VehicleServices` FOREIGN KEY (`VehicleServiceId`) REFERENCES `vehicleservices` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `CK_VehicleServiceSchedules_RecurrenceInterval` CHECK ((`RecurrenceIntervalAmount` > 0))
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Dumping data for table vehicleserviceschedules
--

/*!40000 ALTER TABLE `vehicleserviceschedules` DISABLE KEYS */;
/*!40000 ALTER TABLE `vehicleserviceschedules` ENABLE KEYS */;

--
-- Definition of vehicletowlinks
--

DROP TABLE IF EXISTS `vehicletowlinks`;
CREATE TABLE IF NOT EXISTS `vehicletowlinks` (
  `Id` bigint NOT NULL AUTO_INCREMENT,
  `SourceVehicleId` bigint NOT NULL,
  `TargetVehicleId` bigint NOT NULL,
  `SourceTowPointProtoId` bigint NOT NULL,
  `TargetTowPointProtoId` bigint NOT NULL,
  `HitchItemId` bigint DEFAULT NULL,
  `IsDisabled` bit(1) NOT NULL,
  `CreatedDateTime` datetime NOT NULL,
  PRIMARY KEY (`Id`),
  KEY `FK_VehicleTowLinks_GameItems_idx` (`HitchItemId`),
  KEY `FK_VehicleTowLinks_SourceTowPointProtos_idx` (`SourceTowPointProtoId`),
  KEY `FK_VehicleTowLinks_SourceVehicles_idx` (`SourceVehicleId`),
  KEY `FK_VehicleTowLinks_TargetTowPointProtos_idx` (`TargetTowPointProtoId`),
  KEY `FK_VehicleTowLinks_TargetVehicles_idx` (`TargetVehicleId`),
  CONSTRAINT `FK_VehicleTowLinks_GameItems` FOREIGN KEY (`HitchItemId`) REFERENCES `gameitems` (`Id`) ON DELETE SET NULL,
  CONSTRAINT `FK_VehicleTowLinks_SourceTowPointProtos` FOREIGN KEY (`SourceTowPointProtoId`) REFERENCES `vehicletowpointprotos` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_VehicleTowLinks_SourceVehicles` FOREIGN KEY (`SourceVehicleId`) REFERENCES `vehicles` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_VehicleTowLinks_TargetTowPointProtos` FOREIGN KEY (`TargetTowPointProtoId`) REFERENCES `vehicletowpointprotos` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_VehicleTowLinks_TargetVehicles` FOREIGN KEY (`TargetVehicleId`) REFERENCES `vehicles` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Dumping data for table vehicletowlinks
--

/*!40000 ALTER TABLE `vehicletowlinks` DISABLE KEYS */;
/*!40000 ALTER TABLE `vehicletowlinks` ENABLE KEYS */;

--
-- Definition of vehicletowpointprotos
--

DROP TABLE IF EXISTS `vehicletowpointprotos`;
CREATE TABLE IF NOT EXISTS `vehicletowpointprotos` (
  `Id` bigint NOT NULL AUTO_INCREMENT,
  `VehicleProtoId` bigint NOT NULL,
  `VehicleProtoRevision` int NOT NULL,
  `RequiredAccessPointProtoId` bigint DEFAULT NULL,
  `Name` varchar(200) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `Description` mediumtext CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `TowType` varchar(200) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `CanTow` bit(1) NOT NULL,
  `CanBeTowed` bit(1) NOT NULL,
  `MaximumTowedWeight` double NOT NULL,
  `DisplayOrder` int NOT NULL,
  `CharacterPullMultiplier` double NOT NULL DEFAULT '1',
  `TowStressDamageMultiplier` double DEFAULT NULL,
  `TowStressFailureStartRatio` double DEFAULT NULL,
  `TowStressMaximumFailureChance` double DEFAULT NULL,
  `TowStressWarningRatio` double DEFAULT NULL,
  PRIMARY KEY (`Id`),
  KEY `FK_VehicleTowPointProtos_AccessPoints_idx` (`RequiredAccessPointProtoId`),
  KEY `FK_VehicleTowPointProtos_VehicleProtos_idx` (`VehicleProtoId`,`VehicleProtoRevision`),
  CONSTRAINT `FK_VehicleTowPointProtos_AccessPoints` FOREIGN KEY (`RequiredAccessPointProtoId`) REFERENCES `vehicleaccesspointprotos` (`Id`) ON DELETE SET NULL,
  CONSTRAINT `FK_VehicleTowPointProtos_VehicleProtos` FOREIGN KEY (`VehicleProtoId`, `VehicleProtoRevision`) REFERENCES `vehicleprotos` (`Id`, `RevisionNumber`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Dumping data for table vehicletowpointprotos
--

/*!40000 ALTER TABLE `vehicletowpointprotos` DISABLE KEYS */;
/*!40000 ALTER TABLE `vehicletowpointprotos` ENABLE KEYS */;

--
-- Definition of virtualcashbalances
--

DROP TABLE IF EXISTS `virtualcashbalances`;
CREATE TABLE IF NOT EXISTS `virtualcashbalances` (
  `Id` bigint NOT NULL AUTO_INCREMENT,
  `OwnerType` varchar(100) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `OwnerId` bigint NOT NULL,
  `CurrencyId` bigint NOT NULL,
  `Balance` decimal(58,29) NOT NULL,
  PRIMARY KEY (`Id`),
  UNIQUE KEY `IX_VirtualCashBalances_Owner_Currency` (`OwnerType`,`OwnerId`,`CurrencyId`),
  KEY `FK_VirtualCashBalances_Currencies_idx` (`CurrencyId`),
  CONSTRAINT `FK_VirtualCashBalances_Currencies` FOREIGN KEY (`CurrencyId`) REFERENCES `currencies` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Dumping data for table virtualcashbalances
--

/*!40000 ALTER TABLE `virtualcashbalances` DISABLE KEYS */;
/*!40000 ALTER TABLE `virtualcashbalances` ENABLE KEYS */;

--
-- Definition of virtualcashledgerentries
--

DROP TABLE IF EXISTS `virtualcashledgerentries`;
CREATE TABLE IF NOT EXISTS `virtualcashledgerentries` (
  `Id` bigint NOT NULL AUTO_INCREMENT,
  `OwnerType` varchar(100) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `OwnerId` bigint NOT NULL,
  `CurrencyId` bigint NOT NULL,
  `RealDateTime` datetime NOT NULL,
  `MudDateTime` varchar(500) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `ActorId` bigint DEFAULT NULL,
  `ActorName` mediumtext CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci,
  `CounterpartyId` bigint DEFAULT NULL,
  `CounterpartyType` varchar(100) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci DEFAULT NULL,
  `CounterpartyName` mediumtext CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci,
  `Amount` decimal(58,29) NOT NULL,
  `BalanceAfter` decimal(58,29) NOT NULL,
  `SourceKind` varchar(100) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `DestinationKind` varchar(100) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `LinkedBankAccountId` bigint DEFAULT NULL,
  `ReferenceType` varchar(100) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci DEFAULT NULL,
  `ReferenceId` bigint DEFAULT NULL,
  `Reference` mediumtext CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci,
  `Reason` mediumtext CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  PRIMARY KEY (`Id`),
  KEY `FK_VirtualCashLedgerEntries_BankAccounts_idx` (`LinkedBankAccountId`),
  KEY `FK_VirtualCashLedgerEntries_Currencies_idx` (`CurrencyId`),
  KEY `IX_VirtualCashLedgerEntries_Owner_Date` (`OwnerType`,`OwnerId`,`RealDateTime`),
  CONSTRAINT `FK_VirtualCashLedgerEntries_BankAccounts` FOREIGN KEY (`LinkedBankAccountId`) REFERENCES `bankaccounts` (`Id`) ON DELETE SET NULL,
  CONSTRAINT `FK_VirtualCashLedgerEntries_Currencies` FOREIGN KEY (`CurrencyId`) REFERENCES `currencies` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Dumping data for table virtualcashledgerentries
--

/*!40000 ALTER TABLE `virtualcashledgerentries` DISABLE KEYS */;
/*!40000 ALTER TABLE `virtualcashledgerentries` ENABLE KEYS */;

--
-- Definition of weapontypes
--

DROP TABLE IF EXISTS `weapontypes`;
CREATE TABLE IF NOT EXISTS `weapontypes` (
  `Id` bigint NOT NULL AUTO_INCREMENT,
  `Name` varchar(200) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL,
  `Classification` int NOT NULL,
  `AttackTraitId` bigint DEFAULT NULL,
  `ParryTraitId` bigint DEFAULT NULL,
  `ParryBonus` double NOT NULL,
  `Reach` int NOT NULL DEFAULT '1',
  `StaminaPerParry` double NOT NULL,
  PRIMARY KEY (`Id`),
  KEY `FK_WeaponTypes_TraitDefinitions_Attack_idx` (`AttackTraitId`),
  KEY `FK_WeaponTypes_TraitDefinitions_Parry_idx` (`ParryTraitId`),
  CONSTRAINT `FK_WeaponTypes_TraitDefinitions_Attack` FOREIGN KEY (`AttackTraitId`) REFERENCES `traitdefinitions` (`Id`) ON DELETE SET NULL,
  CONSTRAINT `FK_WeaponTypes_TraitDefinitions_Parry` FOREIGN KEY (`ParryTraitId`) REFERENCES `traitdefinitions` (`Id`) ON DELETE SET NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Dumping data for table weapontypes
--

/*!40000 ALTER TABLE `weapontypes` DISABLE KEYS */;
/*!40000 ALTER TABLE `weapontypes` ENABLE KEYS */;

--
-- Definition of weaponattacks
--

DROP TABLE IF EXISTS `weaponattacks`;
CREATE TABLE IF NOT EXISTS `weaponattacks` (
  `Id` bigint NOT NULL AUTO_INCREMENT,
  `WeaponTypeId` bigint DEFAULT NULL,
  `Verb` int NOT NULL,
  `FutureProgId` bigint DEFAULT NULL,
  `BaseAttackerDifficulty` int NOT NULL DEFAULT '5',
  `BaseBlockDifficulty` int NOT NULL DEFAULT '5',
  `BaseDodgeDifficulty` int NOT NULL DEFAULT '5',
  `BaseParryDifficulty` int NOT NULL DEFAULT '5',
  `BaseAngleOfIncidence` double NOT NULL DEFAULT '1.5708',
  `RecoveryDifficultySuccess` int NOT NULL DEFAULT '5',
  `RecoveryDifficultyFailure` int NOT NULL DEFAULT '5',
  `MoveType` int NOT NULL,
  `Intentions` bigint NOT NULL,
  `ExertionLevel` int NOT NULL,
  `DamageType` int NOT NULL,
  `DamageExpressionId` bigint NOT NULL,
  `StunExpressionId` bigint NOT NULL,
  `PainExpressionId` bigint NOT NULL,
  `Weighting` double NOT NULL DEFAULT '1',
  `BodypartShapeId` bigint DEFAULT NULL,
  `StaminaCost` double NOT NULL,
  `BaseDelay` double NOT NULL DEFAULT '1',
  `Name` text CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci,
  `Orientation` int NOT NULL,
  `Alignment` int NOT NULL,
  `AdditionalInfo` text CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci,
  `HandednessOptions` int NOT NULL,
  `RequiredPositionStateIds` varchar(250) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL DEFAULT '1 16 17 18',
  `OnUseProgId` bigint DEFAULT NULL,
  `MaximumTargets` int NOT NULL DEFAULT '1',
  PRIMARY KEY (`Id`),
  KEY `FK_WeaponAttacks_TraitExpression_Damage_idx` (`DamageExpressionId`),
  KEY `FK_WeaponAttacks_FutureProgs_idx` (`FutureProgId`),
  KEY `FK_WeaponAttacks_TraitExpression_Pain_idx` (`PainExpressionId`),
  KEY `FK_WeaponAttacks_TraitExpression_Stun_idx` (`StunExpressionId`),
  KEY `FK_WeaponAttacks_WeaponTypes_idx` (`WeaponTypeId`),
  CONSTRAINT `FK_WeaponAttacks_FutureProgs` FOREIGN KEY (`FutureProgId`) REFERENCES `futureprogs` (`Id`) ON DELETE SET NULL,
  CONSTRAINT `FK_WeaponAttacks_TraitExpression_Damage` FOREIGN KEY (`DamageExpressionId`) REFERENCES `traitexpression` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_WeaponAttacks_TraitExpression_Pain` FOREIGN KEY (`PainExpressionId`) REFERENCES `traitexpression` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_WeaponAttacks_TraitExpression_Stun` FOREIGN KEY (`StunExpressionId`) REFERENCES `traitexpression` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_WeaponAttacks_WeaponTypes` FOREIGN KEY (`WeaponTypeId`) REFERENCES `weapontypes` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Dumping data for table weaponattacks
--

/*!40000 ALTER TABLE `weaponattacks` DISABLE KEYS */;
/*!40000 ALTER TABLE `weaponattacks` ENABLE KEYS */;

--
-- Definition of combatmessages_weaponattacks
--

DROP TABLE IF EXISTS `combatmessages_weaponattacks`;
CREATE TABLE IF NOT EXISTS `combatmessages_weaponattacks` (
  `CombatMessageId` bigint NOT NULL,
  `WeaponAttackId` bigint NOT NULL,
  PRIMARY KEY (`CombatMessageId`,`WeaponAttackId`),
  KEY `FK_CombatMessages_WeaponAttacks_WeaponAttacks_idx` (`WeaponAttackId`),
  CONSTRAINT `FK_CombatMessages_WeaponAttacks_CombatMessages` FOREIGN KEY (`CombatMessageId`) REFERENCES `combatmessages` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_CombatMessages_WeaponAttacks_WeaponAttacks` FOREIGN KEY (`WeaponAttackId`) REFERENCES `weaponattacks` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Dumping data for table combatmessages_weaponattacks
--

/*!40000 ALTER TABLE `combatmessages_weaponattacks` DISABLE KEYS */;
/*!40000 ALTER TABLE `combatmessages_weaponattacks` ENABLE KEYS */;

--
-- Definition of manualcombatcommands
--

DROP TABLE IF EXISTS `manualcombatcommands`;
CREATE TABLE IF NOT EXISTS `manualcombatcommands` (
  `Id` bigint NOT NULL AUTO_INCREMENT,
  `Name` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL,
  `PrimaryVerb` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL,
  `AdditionalVerbs` varchar(500) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL DEFAULT '',
  `ActionKind` int NOT NULL,
  `WeaponAttackId` bigint DEFAULT NULL,
  `CombatActionId` bigint DEFAULT NULL,
  `PlayerUsable` bit(1) NOT NULL DEFAULT b'1',
  `NpcUsable` bit(1) NOT NULL DEFAULT b'1',
  `UsabilityProgId` bigint DEFAULT NULL,
  `CooldownSeconds` double NOT NULL,
  `CooldownMessage` varchar(500) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL DEFAULT 'You must wait a short time before doing that again.',
  `DefaultAiWeightMultiplier` double NOT NULL DEFAULT '1',
  PRIMARY KEY (`Id`),
  KEY `FK_ManualCombatCommands_CombatActions_idx` (`CombatActionId`),
  KEY `FK_ManualCombatCommands_FutureProgs_idx` (`UsabilityProgId`),
  KEY `FK_ManualCombatCommands_WeaponAttacks_idx` (`WeaponAttackId`),
  CONSTRAINT `FK_ManualCombatCommands_CombatActions` FOREIGN KEY (`CombatActionId`) REFERENCES `combatactions` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_ManualCombatCommands_FutureProgs` FOREIGN KEY (`UsabilityProgId`) REFERENCES `futureprogs` (`Id`) ON DELETE SET NULL,
  CONSTRAINT `FK_ManualCombatCommands_WeaponAttacks` FOREIGN KEY (`WeaponAttackId`) REFERENCES `weaponattacks` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Dumping data for table manualcombatcommands
--

/*!40000 ALTER TABLE `manualcombatcommands` DISABLE KEYS */;
/*!40000 ALTER TABLE `manualcombatcommands` ENABLE KEYS */;

--
-- Definition of charactercombatsettings_manualcombatcommands
--

DROP TABLE IF EXISTS `charactercombatsettings_manualcombatcommands`;
CREATE TABLE IF NOT EXISTS `charactercombatsettings_manualcombatcommands` (
  `CharacterCombatSettingId` bigint NOT NULL,
  `ManualCombatCommandId` bigint NOT NULL,
  `WeightMultiplier` double NOT NULL DEFAULT '1',
  PRIMARY KEY (`CharacterCombatSettingId`,`ManualCombatCommandId`),
  KEY `FK_CCS_ManualCombatCommands_ManualCombatCommands_idx` (`ManualCombatCommandId`),
  CONSTRAINT `FK_CCS_ManualCombatCommands_CharacterCombatSettings` FOREIGN KEY (`CharacterCombatSettingId`) REFERENCES `charactercombatsettings` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_CCS_ManualCombatCommands_ManualCombatCommands` FOREIGN KEY (`ManualCombatCommandId`) REFERENCES `manualcombatcommands` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Dumping data for table charactercombatsettings_manualcombatcommands
--

/*!40000 ALTER TABLE `charactercombatsettings_manualcombatcommands` DISABLE KEYS */;
/*!40000 ALTER TABLE `charactercombatsettings_manualcombatcommands` ENABLE KEYS */;

--
-- Definition of races_weaponattacks
--

DROP TABLE IF EXISTS `races_weaponattacks`;
CREATE TABLE IF NOT EXISTS `races_weaponattacks` (
  `RaceId` bigint NOT NULL,
  `WeaponAttackId` bigint NOT NULL,
  `BodypartId` bigint NOT NULL,
  `Quality` int NOT NULL,
  PRIMARY KEY (`RaceId`,`WeaponAttackId`,`BodypartId`),
  KEY `FK_Races_WeaponAttacks_BodypartProto_idx` (`BodypartId`),
  KEY `FK_Races_WeaponAttacks_WeaponAttacks_idx` (`WeaponAttackId`),
  CONSTRAINT `FK_Races_WeaponAttacks_BodypartProto` FOREIGN KEY (`BodypartId`) REFERENCES `bodypartproto` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_Races_WeaponAttacks_Races` FOREIGN KEY (`RaceId`) REFERENCES `races` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_Races_WeaponAttacks_WeaponAttacks` FOREIGN KEY (`WeaponAttackId`) REFERENCES `weaponattacks` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Dumping data for table races_weaponattacks
--

/*!40000 ALTER TABLE `races_weaponattacks` DISABLE KEYS */;
/*!40000 ALTER TABLE `races_weaponattacks` ENABLE KEYS */;

--
-- Definition of wearablesizeparameterrule
--

DROP TABLE IF EXISTS `wearablesizeparameterrule`;
CREATE TABLE IF NOT EXISTS `wearablesizeparameterrule` (
  `Id` bigint NOT NULL AUTO_INCREMENT,
  `MinHeightFactor` double NOT NULL,
  `MaxHeightFactor` double NOT NULL,
  `MinWeightFactor` double NOT NULL,
  `MaxWeightFactor` double DEFAULT NULL,
  `MinTraitFactor` double DEFAULT NULL,
  `MaxTraitFactor` double DEFAULT NULL,
  `TraitId` bigint DEFAULT NULL,
  `BodyProtoId` bigint NOT NULL,
  `IgnoreTrait` bit(1) NOT NULL DEFAULT b'1',
  `WeightVolumeRatios` varchar(4000) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci DEFAULT NULL,
  `TraitVolumeRatios` varchar(4000) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci DEFAULT NULL,
  `HeightLinearRatios` varchar(4000) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci DEFAULT NULL,
  PRIMARY KEY (`Id`),
  KEY `FK_WearableSizeParameterRule_TraitDefinitions` (`TraitId`),
  CONSTRAINT `FK_WearableSizeParameterRule_TraitDefinitions` FOREIGN KEY (`TraitId`) REFERENCES `traitdefinitions` (`Id`) ON DELETE RESTRICT
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Dumping data for table wearablesizeparameterrule
--

/*!40000 ALTER TABLE `wearablesizeparameterrule` DISABLE KEYS */;
/*!40000 ALTER TABLE `wearablesizeparameterrule` ENABLE KEYS */;

--
-- Definition of bodyprotos
--

DROP TABLE IF EXISTS `bodyprotos`;
CREATE TABLE IF NOT EXISTS `bodyprotos` (
  `Id` bigint NOT NULL AUTO_INCREMENT,
  `Name` varchar(100) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci DEFAULT NULL,
  `CountsAsId` bigint DEFAULT NULL,
  `WearSizeParameterId` bigint NOT NULL,
  `WielderDescriptionPlural` varchar(100) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL DEFAULT 'hands',
  `WielderDescriptionSingle` varchar(100) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL DEFAULT 'hand',
  `ConsiderString` varchar(4000) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `StaminaRecoveryProgId` bigint DEFAULT NULL,
  `MinimumLegsToStand` int NOT NULL DEFAULT '2',
  `MinimumWingsToFly` int NOT NULL DEFAULT '2',
  `LegDescriptionSingular` varchar(100) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL DEFAULT 'leg',
  `LegDescriptionPlural` varchar(100) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL DEFAULT 'legs',
  `DefaultSmashingBodypartId` bigint DEFAULT NULL,
  `NameForTracking` varchar(100) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci DEFAULT NULL,
  `PlanarData` text CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci,
  PRIMARY KEY (`Id`),
  KEY `FK_BodyPrototype_BodyPrototype_idx` (`CountsAsId`),
  KEY `FK_BodyPrototype_Bodyparts_idx` (`DefaultSmashingBodypartId`),
  KEY `FK_BodyPrototype_WearableSizeParameterRule` (`WearSizeParameterId`),
  CONSTRAINT `FK_BodyPrototype_Bodyparts` FOREIGN KEY (`DefaultSmashingBodypartId`) REFERENCES `bodypartproto` (`Id`) ON DELETE SET NULL,
  CONSTRAINT `FK_BodyPrototype_BodyPrototype` FOREIGN KEY (`CountsAsId`) REFERENCES `bodyprotos` (`Id`) ON DELETE SET NULL,
  CONSTRAINT `FK_BodyPrototype_WearableSizeParameterRule` FOREIGN KEY (`WearSizeParameterId`) REFERENCES `wearablesizeparameterrule` (`Id`) ON DELETE RESTRICT
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Dumping data for table bodyprotos
--

/*!40000 ALTER TABLE `bodyprotos` DISABLE KEYS */;
/*!40000 ALTER TABLE `bodyprotos` ENABLE KEYS */;

--
-- Definition of bodypartgroupdescribers
--

DROP TABLE IF EXISTS `bodypartgroupdescribers`;
CREATE TABLE IF NOT EXISTS `bodypartgroupdescribers` (
  `Id` bigint NOT NULL AUTO_INCREMENT,
  `DescribedAs` varchar(4000) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `Comment` varchar(4000) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci DEFAULT NULL,
  `Type` varchar(50) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `BodyProtoId` bigint DEFAULT NULL,
  PRIMARY KEY (`Id`),
  KEY `IX_BodypartGroupDescribers_BodyProtoId` (`BodyProtoId`),
  CONSTRAINT `FK_BodypartGroupDescribers_BodyProtos_BodyProtoId` FOREIGN KEY (`BodyProtoId`) REFERENCES `bodyprotos` (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Dumping data for table bodypartgroupdescribers
--

/*!40000 ALTER TABLE `bodypartgroupdescribers` DISABLE KEYS */;
/*!40000 ALTER TABLE `bodypartgroupdescribers` ENABLE KEYS */;

--
-- Definition of bodypartgroupdescribers_bodyprotos
--

DROP TABLE IF EXISTS `bodypartgroupdescribers_bodyprotos`;
CREATE TABLE IF NOT EXISTS `bodypartgroupdescribers_bodyprotos` (
  `BodypartGroupDescriberId` bigint NOT NULL,
  `BodyProtoId` bigint NOT NULL,
  PRIMARY KEY (`BodypartGroupDescriberId`,`BodyProtoId`),
  KEY `FK_BGD_BodyProtos_BodyProtos` (`BodyProtoId`),
  CONSTRAINT `FK_BGD_BodyProtos_BodypartGroupDescribers` FOREIGN KEY (`BodypartGroupDescriberId`) REFERENCES `bodypartgroupdescribers` (`Id`) ON DELETE RESTRICT,
  CONSTRAINT `FK_BGD_BodyProtos_BodyProtos` FOREIGN KEY (`BodyProtoId`) REFERENCES `bodyprotos` (`Id`) ON DELETE RESTRICT
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Dumping data for table bodypartgroupdescribers_bodyprotos
--

/*!40000 ALTER TABLE `bodypartgroupdescribers_bodyprotos` DISABLE KEYS */;
/*!40000 ALTER TABLE `bodypartgroupdescribers_bodyprotos` ENABLE KEYS */;

--
-- Definition of bodyprotos_additionalbodyparts
--

DROP TABLE IF EXISTS `bodyprotos_additionalbodyparts`;
CREATE TABLE IF NOT EXISTS `bodyprotos_additionalbodyparts` (
  `BodyProtoId` bigint NOT NULL,
  `BodypartId` bigint NOT NULL,
  `Usage` varchar(50) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  PRIMARY KEY (`BodyProtoId`,`BodypartId`,`Usage`),
  KEY `FK_BodyProtos_AdditionalBodyparts_BodypartProto_idx` (`BodypartId`),
  KEY `FK_BodyProtos_AdditionalBodyparts_BodyProtos_idx` (`BodyProtoId`),
  CONSTRAINT `FK_BodyProtos_AdditionalBodyparts_BodypartProto` FOREIGN KEY (`BodypartId`) REFERENCES `bodypartproto` (`Id`) ON DELETE RESTRICT,
  CONSTRAINT `FK_BodyProtos_AdditionalBodyparts_BodyProtos` FOREIGN KEY (`BodyProtoId`) REFERENCES `bodyprotos` (`Id`) ON DELETE RESTRICT
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Dumping data for table bodyprotos_additionalbodyparts
--

/*!40000 ALTER TABLE `bodyprotos_additionalbodyparts` DISABLE KEYS */;
/*!40000 ALTER TABLE `bodyprotos_additionalbodyparts` ENABLE KEYS */;

--
-- Definition of bodyprotospositions
--

DROP TABLE IF EXISTS `bodyprotospositions`;
CREATE TABLE IF NOT EXISTS `bodyprotospositions` (
  `BodyProtoId` bigint NOT NULL,
  `Position` int NOT NULL,
  PRIMARY KEY (`BodyProtoId`,`Position`),
  KEY `FK_BodyProtosPositions_BodyProtos_idx` (`BodyProtoId`),
  CONSTRAINT `FK_BodyProtosPositions_BodyProtos` FOREIGN KEY (`BodyProtoId`) REFERENCES `bodyprotos` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Dumping data for table bodyprotospositions
--

/*!40000 ALTER TABLE `bodyprotospositions` DISABLE KEYS */;
/*!40000 ALTER TABLE `bodyprotospositions` ENABLE KEYS */;

--
-- Definition of limbs
--

DROP TABLE IF EXISTS `limbs`;
CREATE TABLE IF NOT EXISTS `limbs` (
  `Id` bigint NOT NULL AUTO_INCREMENT,
  `Name` text CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci,
  `RootBodypartId` bigint NOT NULL,
  `LimbType` int NOT NULL,
  `RootBodyId` bigint NOT NULL,
  `LimbDamageThresholdMultiplier` double NOT NULL,
  `LimbPainThresholdMultiplier` double NOT NULL,
  PRIMARY KEY (`Id`),
  KEY `FK_Limbs_BodyProtos_idx` (`RootBodyId`),
  KEY `FK_Limbs_BodypartProto_idx` (`RootBodypartId`),
  CONSTRAINT `FK_Limbs_BodypartProto` FOREIGN KEY (`RootBodypartId`) REFERENCES `bodypartproto` (`Id`) ON DELETE RESTRICT,
  CONSTRAINT `FK_Limbs_BodyProtos` FOREIGN KEY (`RootBodyId`) REFERENCES `bodyprotos` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Dumping data for table limbs
--

/*!40000 ALTER TABLE `limbs` DISABLE KEYS */;
/*!40000 ALTER TABLE `limbs` ENABLE KEYS */;

--
-- Definition of limbs_bodypartproto
--

DROP TABLE IF EXISTS `limbs_bodypartproto`;
CREATE TABLE IF NOT EXISTS `limbs_bodypartproto` (
  `BodypartProtoId` bigint NOT NULL,
  `LimbId` bigint NOT NULL,
  PRIMARY KEY (`BodypartProtoId`,`LimbId`),
  KEY `FK_Limbs_BodypartProto_Limbs_idx` (`LimbId`),
  CONSTRAINT `FK_Limbs_BodypartProto_BodypartProto` FOREIGN KEY (`BodypartProtoId`) REFERENCES `bodypartproto` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_Limbs_BodypartProto_Limbs` FOREIGN KEY (`LimbId`) REFERENCES `limbs` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Dumping data for table limbs_bodypartproto
--

/*!40000 ALTER TABLE `limbs_bodypartproto` DISABLE KEYS */;
/*!40000 ALTER TABLE `limbs_bodypartproto` ENABLE KEYS */;

--
-- Definition of limbs_spinalparts
--

DROP TABLE IF EXISTS `limbs_spinalparts`;
CREATE TABLE IF NOT EXISTS `limbs_spinalparts` (
  `LimbId` bigint NOT NULL,
  `BodypartProtoId` bigint NOT NULL,
  PRIMARY KEY (`LimbId`,`BodypartProtoId`),
  KEY `FK_Limbs_SpinalParts_BodypartProtos_idx` (`BodypartProtoId`),
  CONSTRAINT `FK_Limbs_SpinalParts_BodypartProtos` FOREIGN KEY (`BodypartProtoId`) REFERENCES `bodypartproto` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_Limbs_SpinalParts_Limbs` FOREIGN KEY (`LimbId`) REFERENCES `limbs` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Dumping data for table limbs_spinalparts
--

/*!40000 ALTER TABLE `limbs_spinalparts` DISABLE KEYS */;
/*!40000 ALTER TABLE `limbs_spinalparts` ENABLE KEYS */;

--
-- Definition of movespeeds
--

DROP TABLE IF EXISTS `movespeeds`;
CREATE TABLE IF NOT EXISTS `movespeeds` (
  `Id` bigint NOT NULL AUTO_INCREMENT,
  `BodyProtoId` bigint NOT NULL,
  `Multiplier` double NOT NULL,
  `Alias` varchar(255) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `FirstPersonVerb` varchar(255) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `ThirdPersonVerb` varchar(255) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `PresentParticiple` varchar(255) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `PositionId` bigint NOT NULL,
  `StaminaMultiplier` double NOT NULL DEFAULT '1',
  PRIMARY KEY (`Id`),
  KEY `FK_MoveSpeeds_BodyPrototype` (`BodyProtoId`),
  CONSTRAINT `FK_MoveSpeeds_BodyPrototype` FOREIGN KEY (`BodyProtoId`) REFERENCES `bodyprotos` (`Id`) ON DELETE RESTRICT
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Dumping data for table movespeeds
--

/*!40000 ALTER TABLE `movespeeds` DISABLE KEYS */;
/*!40000 ALTER TABLE `movespeeds` ENABLE KEYS */;

--
-- Definition of wearablesizes
--

DROP TABLE IF EXISTS `wearablesizes`;
CREATE TABLE IF NOT EXISTS `wearablesizes` (
  `Id` bigint NOT NULL AUTO_INCREMENT,
  `OneSizeFitsAll` bit(1) NOT NULL,
  `Height` double DEFAULT NULL,
  `Weight` double DEFAULT NULL,
  `TraitValue` double DEFAULT NULL,
  `BodyPrototypeId` bigint NOT NULL,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Dumping data for table wearablesizes
--

/*!40000 ALTER TABLE `wearablesizes` DISABLE KEYS */;
/*!40000 ALTER TABLE `wearablesizes` ENABLE KEYS */;

--
-- Definition of wearprofiles
--

DROP TABLE IF EXISTS `wearprofiles`;
CREATE TABLE IF NOT EXISTS `wearprofiles` (
  `Id` bigint NOT NULL AUTO_INCREMENT,
  `Name` varchar(255) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `BodyPrototypeId` bigint NOT NULL,
  `WearStringInventory` varchar(255) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL DEFAULT 'worn on',
  `WearAction1st` varchar(255) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL DEFAULT 'put',
  `WearAction3rd` varchar(255) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL DEFAULT 'puts',
  `WearAffix` varchar(255) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL DEFAULT 'on',
  `WearlocProfiles` text CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `Type` varchar(50) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL DEFAULT 'Direct',
  `Description` text CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci,
  `RequireContainerIsEmpty` bit(1) NOT NULL DEFAULT b'0',
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Dumping data for table wearprofiles
--

/*!40000 ALTER TABLE `wearprofiles` DISABLE KEYS */;
/*!40000 ALTER TABLE `wearprofiles` ENABLE KEYS */;

--
-- Definition of outfittemplateitems
--

DROP TABLE IF EXISTS `outfittemplateitems`;
CREATE TABLE IF NOT EXISTS `outfittemplateitems` (
  `Id` bigint NOT NULL AUTO_INCREMENT,
  `OutfitTemplateId` bigint NOT NULL,
  `TemplateKey` varchar(100) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `GameItemProtoId` bigint NOT NULL,
  `WearProfileId` bigint DEFAULT NULL,
  `Placement` int NOT NULL,
  `ContainerKey` varchar(100) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci DEFAULT NULL,
  `LoadArguments` mediumtext CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `WearOrder` int NOT NULL,
  PRIMARY KEY (`Id`),
  UNIQUE KEY `IX_OutfitTemplateItems_Template_Key` (`OutfitTemplateId`,`TemplateKey`),
  KEY `FK_OutfitTemplateItems_OutfitTemplates_idx` (`OutfitTemplateId`),
  KEY `FK_OutfitTemplateItems_WearProfiles_idx` (`WearProfileId`),
  KEY `IX_OutfitTemplateItems_GameItemProtoId` (`GameItemProtoId`),
  CONSTRAINT `FK_OutfitTemplateItems_OutfitTemplates` FOREIGN KEY (`OutfitTemplateId`) REFERENCES `outfittemplates` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_OutfitTemplateItems_WearProfiles` FOREIGN KEY (`WearProfileId`) REFERENCES `wearprofiles` (`Id`) ON DELETE SET NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Dumping data for table outfittemplateitems
--

/*!40000 ALTER TABLE `outfittemplateitems` DISABLE KEYS */;
/*!40000 ALTER TABLE `outfittemplateitems` ENABLE KEYS */;

--
-- Definition of weatherevents
--

DROP TABLE IF EXISTS `weatherevents`;
CREATE TABLE IF NOT EXISTS `weatherevents` (
  `Id` bigint NOT NULL AUTO_INCREMENT,
  `Name` varchar(200) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `WeatherEventType` varchar(45) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `WeatherDescription` text CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `WeatherRoomAddendum` text CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `TemperatureEffect` double NOT NULL,
  `Precipitation` int NOT NULL,
  `Wind` int NOT NULL,
  `AdditionalInfo` text CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci,
  `PrecipitationTemperatureEffect` double NOT NULL,
  `WindTemperatureEffect` double NOT NULL,
  `LightLevelMultiplier` double NOT NULL,
  `ObscuresViewOfSky` bit(1) NOT NULL,
  `PermittedAtNight` bit(1) NOT NULL,
  `PermittedAtDawn` bit(1) NOT NULL,
  `PermittedAtMorning` bit(1) NOT NULL,
  `PermittedAtAfternoon` bit(1) NOT NULL,
  `PermittedAtDusk` bit(1) NOT NULL,
  `CountsAsId` bigint DEFAULT NULL,
  PRIMARY KEY (`Id`),
  KEY `FK_WeatherEvents_WeatherEvents_idx` (`CountsAsId`),
  CONSTRAINT `FK_WeatherEvents_WeatherEvents` FOREIGN KEY (`CountsAsId`) REFERENCES `weatherevents` (`Id`) ON DELETE SET NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Dumping data for table weatherevents
--

/*!40000 ALTER TABLE `weatherevents` DISABLE KEYS */;
/*!40000 ALTER TABLE `weatherevents` ENABLE KEYS */;

--
-- Definition of climatemodelseasonevent
--

DROP TABLE IF EXISTS `climatemodelseasonevent`;
CREATE TABLE IF NOT EXISTS `climatemodelseasonevent` (
  `ClimateModelId` bigint NOT NULL,
  `SeasonId` bigint NOT NULL,
  `WeatherEventId` bigint NOT NULL,
  `ChangeChance` double NOT NULL,
  `Transitions` mediumtext CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  PRIMARY KEY (`ClimateModelId`,`SeasonId`,`WeatherEventId`),
  KEY `IX_ClimateModelSeasonEvent_SeasonId` (`SeasonId`),
  KEY `IX_ClimateModelSeasonEvent_WeatherEventId` (`WeatherEventId`),
  CONSTRAINT `FK_ClimateModelSeasonEvent_ClimateModelSeason_ClimateModelId_Se~` FOREIGN KEY (`ClimateModelId`, `SeasonId`) REFERENCES `climatemodelseason` (`ClimateModelId`, `SeasonId`) ON DELETE CASCADE,
  CONSTRAINT `FK_ClimateModelSeasonEvents_ClimateModels` FOREIGN KEY (`ClimateModelId`) REFERENCES `climatemodels` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_ClimateModelSeasonEvents_Seasons` FOREIGN KEY (`SeasonId`) REFERENCES `seasons` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_ClimateModelSeasonEvents_WeatherEvents` FOREIGN KEY (`WeatherEventId`) REFERENCES `weatherevents` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Dumping data for table climatemodelseasonevent
--

/*!40000 ALTER TABLE `climatemodelseasonevent` DISABLE KEYS */;
/*!40000 ALTER TABLE `climatemodelseasonevent` ENABLE KEYS */;

--
-- Definition of weathercontrollers
--

DROP TABLE IF EXISTS `weathercontrollers`;
CREATE TABLE IF NOT EXISTS `weathercontrollers` (
  `Id` bigint NOT NULL AUTO_INCREMENT,
  `Name` varchar(500) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `FeedClockId` bigint NOT NULL,
  `FeedClockTimeZoneId` bigint NOT NULL,
  `RegionalClimateId` bigint NOT NULL,
  `CurrentWeatherEventId` bigint NOT NULL,
  `CurrentSeasonId` bigint NOT NULL,
  `ConsecutiveUnchangedPeriods` int NOT NULL,
  `MinutesCounter` int NOT NULL,
  `CelestialId` bigint DEFAULT NULL,
  `Elevation` double NOT NULL,
  `Radius` double NOT NULL,
  `Latitude` double NOT NULL,
  `Longitude` double NOT NULL,
  `HighestRecentPrecipitationLevel` int NOT NULL,
  `PeriodsSinceHighestPrecipitation` int NOT NULL,
  `CurrentTemperatureFluctuation` double NOT NULL DEFAULT '0',
  `OppositeHemisphere` bit(1) NOT NULL DEFAULT b'0',
  PRIMARY KEY (`Id`),
  KEY `FK_WeatherControllers_Celestials_idx` (`CelestialId`),
  KEY `FK_WeatherControllers_Seasons_idx` (`CurrentSeasonId`),
  KEY `FK_WeatherControllers_WeatherEvents_idx` (`CurrentWeatherEventId`),
  KEY `FK_WeatherControllers_Clocks_idx` (`FeedClockId`),
  KEY `FK_WeatherControllers_TimeZones_idx` (`FeedClockTimeZoneId`),
  KEY `FK_WeatherControllers_RegionalClimates_idx` (`RegionalClimateId`),
  CONSTRAINT `FK_WeatherControllers_Celestials` FOREIGN KEY (`CelestialId`) REFERENCES `celestials` (`Id`) ON DELETE RESTRICT,
  CONSTRAINT `FK_WeatherControllers_Clocks` FOREIGN KEY (`FeedClockId`) REFERENCES `clocks` (`Id`) ON DELETE RESTRICT,
  CONSTRAINT `FK_WeatherControllers_RegionalClimates` FOREIGN KEY (`RegionalClimateId`) REFERENCES `regionalclimates` (`Id`) ON DELETE RESTRICT,
  CONSTRAINT `FK_WeatherControllers_Seasons` FOREIGN KEY (`CurrentSeasonId`) REFERENCES `seasons` (`Id`) ON DELETE RESTRICT,
  CONSTRAINT `FK_WeatherControllers_TimeZones` FOREIGN KEY (`FeedClockTimeZoneId`) REFERENCES `timezones` (`Id`) ON DELETE RESTRICT,
  CONSTRAINT `FK_WeatherControllers_WeatherEvents` FOREIGN KEY (`CurrentWeatherEventId`) REFERENCES `weatherevents` (`Id`) ON DELETE RESTRICT
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Dumping data for table weathercontrollers
--

/*!40000 ALTER TABLE `weathercontrollers` DISABLE KEYS */;
/*!40000 ALTER TABLE `weathercontrollers` ENABLE KEYS */;

--
-- Definition of areas
--

DROP TABLE IF EXISTS `areas`;
CREATE TABLE IF NOT EXISTS `areas` (
  `Id` bigint NOT NULL AUTO_INCREMENT,
  `Name` varchar(200) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `WeatherControllerId` bigint DEFAULT NULL,
  PRIMARY KEY (`Id`),
  KEY `FK_Areas_WeatherControllers_idx` (`WeatherControllerId`),
  CONSTRAINT `FK_Areas_WeatherControllers` FOREIGN KEY (`WeatherControllerId`) REFERENCES `weathercontrollers` (`Id`) ON DELETE SET NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Dumping data for table areas
--

/*!40000 ALTER TABLE `areas` DISABLE KEYS */;
/*!40000 ALTER TABLE `areas` ENABLE KEYS */;

--
-- Definition of terrains
--

DROP TABLE IF EXISTS `terrains`;
CREATE TABLE IF NOT EXISTS `terrains` (
  `Id` bigint NOT NULL AUTO_INCREMENT,
  `Name` varchar(255) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `MovementRate` double NOT NULL,
  `DefaultTerrain` bit(1) NOT NULL DEFAULT b'0',
  `TerrainBehaviourMode` varchar(50) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `HideDifficulty` int NOT NULL,
  `SpotDifficulty` int NOT NULL,
  `StaminaCost` double NOT NULL,
  `ForagableProfileId` bigint NOT NULL,
  `InfectionMultiplier` double NOT NULL DEFAULT '1',
  `InfectionType` int NOT NULL,
  `InfectionVirulence` int NOT NULL DEFAULT '5',
  `AtmosphereId` bigint DEFAULT NULL,
  `AtmosphereType` varchar(45) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci DEFAULT NULL,
  `TerrainEditorColour` varchar(45) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL DEFAULT '#FFFFFFFF',
  `WeatherControllerId` bigint DEFAULT NULL,
  `DefaultCellOutdoorsType` int NOT NULL DEFAULT '0',
  `TerrainEditorText` varchar(45) DEFAULT NULL,
  `TerrainANSIColour` varchar(45) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL DEFAULT '7',
  `CanHaveTracks` bit(1) NOT NULL DEFAULT b'1',
  `TrackIntensityMultiplierOlfactory` double NOT NULL DEFAULT '1',
  `TrackIntensityMultiplierVisual` double NOT NULL DEFAULT '1',
  `TagInformation` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  `GravityModel` int NOT NULL DEFAULT '0',
  `DefaultAgricultureFieldProfileId` bigint DEFAULT NULL,
  PRIMARY KEY (`Id`),
  KEY `FK_Terrains_WeatherControllers_idx` (`WeatherControllerId`),
  KEY `FK_Terrains_AgricultureFieldProfiles_idx` (`DefaultAgricultureFieldProfileId`),
  CONSTRAINT `FK_Terrains_AgricultureFieldProfiles` FOREIGN KEY (`DefaultAgricultureFieldProfileId`) REFERENCES `agriculturefieldprofiles` (`Id`) ON DELETE SET NULL,
  CONSTRAINT `FK_Terrains_WeatherControllers` FOREIGN KEY (`WeatherControllerId`) REFERENCES `weathercontrollers` (`Id`) ON DELETE SET NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Dumping data for table terrains
--

/*!40000 ALTER TABLE `terrains` DISABLE KEYS */;
/*!40000 ALTER TABLE `terrains` ENABLE KEYS */;

--
-- Definition of celloverlays
--

DROP TABLE IF EXISTS `celloverlays`;
CREATE TABLE IF NOT EXISTS `celloverlays` (
  `Id` bigint NOT NULL AUTO_INCREMENT,
  `Name` varchar(4000) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `CellName` varchar(4000) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `CellDescription` varchar(4000) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `CellOverlayPackageId` bigint NOT NULL,
  `CellId` bigint NOT NULL,
  `CellOverlayPackageRevisionNumber` int NOT NULL,
  `TerrainId` bigint NOT NULL,
  `HearingProfileId` bigint DEFAULT NULL,
  `OutdoorsType` int NOT NULL,
  `AmbientLightFactor` double NOT NULL DEFAULT '1',
  `AddedLight` double NOT NULL,
  `AtmosphereId` bigint DEFAULT NULL,
  `AtmosphereType` varchar(45) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL DEFAULT 'gas',
  `SafeQuit` bit(1) NOT NULL,
  PRIMARY KEY (`Id`),
  KEY `FK_CellOverlays_Cells` (`CellId`),
  KEY `FK_CellOverlays_HearingProfiles` (`HearingProfileId`),
  KEY `FK_CellOverlays_Terrains` (`TerrainId`),
  KEY `FK_CellOverlays_CellOverlayPackages` (`CellOverlayPackageId`,`CellOverlayPackageRevisionNumber`),
  CONSTRAINT `FK_CellOverlays_CellOverlayPackages` FOREIGN KEY (`CellOverlayPackageId`, `CellOverlayPackageRevisionNumber`) REFERENCES `celloverlaypackages` (`Id`, `RevisionNumber`) ON DELETE CASCADE,
  CONSTRAINT `FK_CellOverlays_Cells` FOREIGN KEY (`CellId`) REFERENCES `cells` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_CellOverlays_HearingProfiles` FOREIGN KEY (`HearingProfileId`) REFERENCES `hearingprofiles` (`Id`) ON DELETE SET NULL,
  CONSTRAINT `FK_CellOverlays_Terrains` FOREIGN KEY (`TerrainId`) REFERENCES `terrains` (`Id`) ON DELETE RESTRICT
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Dumping data for table celloverlays
--

/*!40000 ALTER TABLE `celloverlays` DISABLE KEYS */;
/*!40000 ALTER TABLE `celloverlays` ENABLE KEYS */;

--
-- Definition of terrains_rangedcovers
--

DROP TABLE IF EXISTS `terrains_rangedcovers`;
CREATE TABLE IF NOT EXISTS `terrains_rangedcovers` (
  `TerrainId` bigint NOT NULL,
  `RangedCoverId` bigint NOT NULL,
  PRIMARY KEY (`TerrainId`,`RangedCoverId`),
  KEY `FK_Terrains_RangedCovers_RangedCovers_idx` (`RangedCoverId`),
  CONSTRAINT `FK_Terrains_RangedCovers_RangedCovers` FOREIGN KEY (`RangedCoverId`) REFERENCES `rangedcovers` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_Terrains_RangedCovers_Terrains` FOREIGN KEY (`TerrainId`) REFERENCES `terrains` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Dumping data for table terrains_rangedcovers
--

/*!40000 ALTER TABLE `terrains_rangedcovers` DISABLE KEYS */;
/*!40000 ALTER TABLE `terrains_rangedcovers` ENABLE KEYS */;

--
-- Definition of weeklystatistics
--

DROP TABLE IF EXISTS `weeklystatistics`;
CREATE TABLE IF NOT EXISTS `weeklystatistics` (
  `Id` bigint NOT NULL AUTO_INCREMENT,
  `Start` datetime NOT NULL,
  `End` datetime NOT NULL,
  `TotalAccounts` int NOT NULL,
  `ActiveAccounts` int NOT NULL,
  `NewAccounts` int NOT NULL,
  `ApplicationsSubmitted` int NOT NULL,
  `ApplicationsApproved` int NOT NULL,
  `PlayerDeaths` int NOT NULL,
  `NonPlayerDeaths` int NOT NULL,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Dumping data for table weeklystatistics
--

/*!40000 ALTER TABLE `weeklystatistics` DISABLE KEYS */;
/*!40000 ALTER TABLE `weeklystatistics` ENABLE KEYS */;

--
-- Definition of witnessprofiles
--

DROP TABLE IF EXISTS `witnessprofiles`;
CREATE TABLE IF NOT EXISTS `witnessprofiles` (
  `Id` bigint NOT NULL AUTO_INCREMENT,
  `Name` varchar(250) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `IdentityKnownProgId` bigint NOT NULL,
  `ReportingMultiplierProgId` bigint NOT NULL,
  `ReportingReliability` double NOT NULL,
  `MinimumSkillToDetermineTimeOfDay` double NOT NULL,
  `MinimumSkillToDetermineBiases` double NOT NULL,
  `BaseReportingChanceNight` double NOT NULL,
  `BaseReportingChanceDawn` double NOT NULL,
  `BaseReportingChanceMorning` double NOT NULL,
  `BaseReportingChanceAfternoon` double NOT NULL,
  `BaseReportingChanceDusk` double NOT NULL,
  PRIMARY KEY (`Id`),
  KEY `FK_WitnessProfiles_IdentityProg_idx` (`IdentityKnownProgId`),
  KEY `FK_WitnessProfiles_MultiplierProg_idx` (`ReportingMultiplierProgId`),
  CONSTRAINT `FK_WitnessProfiles_IdentityProg` FOREIGN KEY (`IdentityKnownProgId`) REFERENCES `futureprogs` (`Id`) ON DELETE RESTRICT,
  CONSTRAINT `FK_WitnessProfiles_MultiplierProg` FOREIGN KEY (`ReportingMultiplierProgId`) REFERENCES `futureprogs` (`Id`) ON DELETE RESTRICT
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Dumping data for table witnessprofiles
--

/*!40000 ALTER TABLE `witnessprofiles` DISABLE KEYS */;
/*!40000 ALTER TABLE `witnessprofiles` ENABLE KEYS */;

--
-- Definition of witnessprofiles_cooperatingauthorities
--

DROP TABLE IF EXISTS `witnessprofiles_cooperatingauthorities`;
CREATE TABLE IF NOT EXISTS `witnessprofiles_cooperatingauthorities` (
  `WitnessProfileId` bigint NOT NULL,
  `LegalAuthorityId` bigint NOT NULL,
  PRIMARY KEY (`WitnessProfileId`,`LegalAuthorityId`),
  KEY `FK_WitnessProfiles_CooperatingAuthorities_LegalAuthorities_idx` (`LegalAuthorityId`),
  CONSTRAINT `FK_WitnessProfiles_CooperatingAuthorities_LegalAuthorities` FOREIGN KEY (`LegalAuthorityId`) REFERENCES `legalauthorities` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_WitnessProfiles_CooperatingAuthorities_WitnessProfiles` FOREIGN KEY (`WitnessProfileId`) REFERENCES `witnessprofiles` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Dumping data for table witnessprofiles_cooperatingauthorities
--

/*!40000 ALTER TABLE `witnessprofiles_cooperatingauthorities` DISABLE KEYS */;
/*!40000 ALTER TABLE `witnessprofiles_cooperatingauthorities` ENABLE KEYS */;

--
-- Definition of witnessprofiles_ignoredcriminalclasses
--

DROP TABLE IF EXISTS `witnessprofiles_ignoredcriminalclasses`;
CREATE TABLE IF NOT EXISTS `witnessprofiles_ignoredcriminalclasses` (
  `WitnessProfileId` bigint NOT NULL,
  `LegalClassId` bigint NOT NULL,
  PRIMARY KEY (`WitnessProfileId`,`LegalClassId`),
  KEY `FK_WitnessProfiles_IgnoredCriminalClasses_LegalClasses_idx` (`LegalClassId`),
  CONSTRAINT `FK_WitnessProfiles_IgnoredCriminalClasses_LegalClasses` FOREIGN KEY (`LegalClassId`) REFERENCES `legalclasses` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_WitnessProfiles_IgnoredCriminalClasses_WitnessProfiles` FOREIGN KEY (`WitnessProfileId`) REFERENCES `witnessprofiles` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Dumping data for table witnessprofiles_ignoredcriminalclasses
--

/*!40000 ALTER TABLE `witnessprofiles_ignoredcriminalclasses` DISABLE KEYS */;
/*!40000 ALTER TABLE `witnessprofiles_ignoredcriminalclasses` ENABLE KEYS */;

--
-- Definition of witnessprofiles_ignoredvictimclasses
--

DROP TABLE IF EXISTS `witnessprofiles_ignoredvictimclasses`;
CREATE TABLE IF NOT EXISTS `witnessprofiles_ignoredvictimclasses` (
  `WitnessProfileId` bigint NOT NULL,
  `LegalClassId` bigint NOT NULL,
  PRIMARY KEY (`WitnessProfileId`,`LegalClassId`),
  KEY `FK_WitnessProfiles_IgnoredVictimClasses_LegalClasses_idx` (`LegalClassId`),
  CONSTRAINT `FK_WitnessProfiles_IgnoredVictimClasses_LegalClasses` FOREIGN KEY (`LegalClassId`) REFERENCES `legalclasses` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_WitnessProfiles_IgnoredVictimClasses_WitnessProfiles` FOREIGN KEY (`WitnessProfileId`) REFERENCES `witnessprofiles` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Dumping data for table witnessprofiles_ignoredvictimclasses
--

/*!40000 ALTER TABLE `witnessprofiles_ignoredvictimclasses` DISABLE KEYS */;
/*!40000 ALTER TABLE `witnessprofiles_ignoredvictimclasses` ENABLE KEYS */;

--
-- Definition of wounds
--

DROP TABLE IF EXISTS `wounds`;
CREATE TABLE IF NOT EXISTS `wounds` (
  `Id` bigint NOT NULL AUTO_INCREMENT,
  `BodyId` bigint DEFAULT NULL,
  `GameItemId` bigint DEFAULT NULL,
  `OriginalDamage` double NOT NULL,
  `CurrentDamage` double NOT NULL,
  `CurrentPain` double NOT NULL,
  `CurrentShock` double NOT NULL,
  `CurrentStun` double NOT NULL,
  `LodgedItemId` bigint DEFAULT NULL,
  `DamageType` int NOT NULL,
  `Internal` bit(1) NOT NULL DEFAULT b'0',
  `BodypartProtoId` bigint DEFAULT NULL,
  `ExtraInformation` text CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci,
  `ActorOriginId` bigint DEFAULT NULL,
  `ToolOriginId` bigint DEFAULT NULL,
  `WoundType` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NOT NULL,
  `VehicleDamageZoneId` bigint DEFAULT NULL,
  `VehicleId` bigint DEFAULT NULL,
  `RealTimeOfWound` datetime DEFAULT NULL,
  PRIMARY KEY (`Id`),
  KEY `FK_Wounds_Characters_idx` (`ActorOriginId`),
  KEY `FK_Wounds_Bodies_idx` (`BodyId`),
  KEY `FK_Wounds_GameItemOwner_idx` (`GameItemId`),
  KEY `FK_Wounds_GameItems_idx` (`LodgedItemId`),
  KEY `FK_Wounds_GameItems_Tool_idx` (`ToolOriginId`),
  KEY `FK_Wounds_VehicleDamageZones_idx` (`VehicleDamageZoneId`),
  KEY `FK_Wounds_Vehicles_idx` (`VehicleId`),
  CONSTRAINT `FK_Wounds_Bodies` FOREIGN KEY (`BodyId`) REFERENCES `bodies` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_Wounds_Characters` FOREIGN KEY (`ActorOriginId`) REFERENCES `characters` (`Id`) ON DELETE SET NULL,
  CONSTRAINT `FK_Wounds_GameItemOwner` FOREIGN KEY (`GameItemId`) REFERENCES `gameitems` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_Wounds_GameItems` FOREIGN KEY (`LodgedItemId`) REFERENCES `gameitems` (`Id`) ON DELETE SET NULL,
  CONSTRAINT `FK_Wounds_GameItems_Tool` FOREIGN KEY (`ToolOriginId`) REFERENCES `gameitems` (`Id`) ON DELETE SET NULL,
  CONSTRAINT `FK_Wounds_VehicleDamageZones` FOREIGN KEY (`VehicleDamageZoneId`) REFERENCES `vehicledamagezones` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_Wounds_Vehicles` FOREIGN KEY (`VehicleId`) REFERENCES `vehicles` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Dumping data for table wounds
--

/*!40000 ALTER TABLE `wounds` DISABLE KEYS */;
/*!40000 ALTER TABLE `wounds` ENABLE KEYS */;

--
-- Definition of infections
--

DROP TABLE IF EXISTS `infections`;
CREATE TABLE IF NOT EXISTS `infections` (
  `Id` bigint NOT NULL AUTO_INCREMENT,
  `InfectionType` int NOT NULL,
  `Virulence` int NOT NULL,
  `Intensity` double NOT NULL,
  `OwnerId` bigint NOT NULL,
  `WoundId` bigint DEFAULT NULL,
  `BodypartId` bigint DEFAULT NULL,
  `Immunity` double NOT NULL,
  `VirulenceMultiplier` double NOT NULL DEFAULT '1',
  PRIMARY KEY (`Id`),
  KEY `FK_Infections_Bodyparts_idx` (`BodypartId`),
  KEY `FK_Infections_Bodies_idx` (`OwnerId`),
  KEY `FK_Infections_Wounds_idx` (`WoundId`),
  CONSTRAINT `FK_Infections_Bodies` FOREIGN KEY (`OwnerId`) REFERENCES `bodies` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_Infections_Bodyparts` FOREIGN KEY (`BodypartId`) REFERENCES `bodypartproto` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_Infections_Wounds` FOREIGN KEY (`WoundId`) REFERENCES `wounds` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Dumping data for table infections
--

/*!40000 ALTER TABLE `infections` DISABLE KEYS */;
/*!40000 ALTER TABLE `infections` ENABLE KEYS */;

--
-- Definition of writingcollections
--

DROP TABLE IF EXISTS `writingcollections`;
CREATE TABLE IF NOT EXISTS `writingcollections` (
  `Id` bigint NOT NULL AUTO_INCREMENT,
  `Name` varchar(200) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `Description` mediumtext CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `DefaultTitle` varchar(500) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  PRIMARY KEY (`Id`),
  UNIQUE KEY `IX_WritingCollections_Name` (`Name`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Dumping data for table writingcollections
--

/*!40000 ALTER TABLE `writingcollections` DISABLE KEYS */;
/*!40000 ALTER TABLE `writingcollections` ENABLE KEYS */;

--
-- Definition of writings
--

DROP TABLE IF EXISTS `writings`;
CREATE TABLE IF NOT EXISTS `writings` (
  `Id` bigint NOT NULL AUTO_INCREMENT,
  `WritingType` varchar(45) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `Style` int NOT NULL,
  `LanguageId` bigint NOT NULL,
  `ScriptId` bigint NOT NULL,
  `AuthorId` bigint DEFAULT NULL,
  `TrueAuthorId` bigint DEFAULT NULL,
  `HandwritingSkill` double NOT NULL,
  `LiteracySkill` double NOT NULL,
  `ForgerySkill` double NOT NULL,
  `LanguageSkill` double NOT NULL,
  `Definition` text CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `WritingColour` bigint NOT NULL,
  `ImplementType` int NOT NULL,
  PRIMARY KEY (`Id`),
  KEY `FK_Writings_Characters_Author_idx` (`AuthorId`),
  KEY `FK_Writings_Languages_idx` (`LanguageId`),
  KEY `FK_Writings_Scripts_idx` (`ScriptId`),
  KEY `FK_Writings_Characters_TrueAuthor_idx` (`TrueAuthorId`),
  CONSTRAINT `FK_Writings_Characters_Author` FOREIGN KEY (`AuthorId`) REFERENCES `characters` (`Id`) ON DELETE SET NULL,
  CONSTRAINT `FK_Writings_Characters_TrueAuthor` FOREIGN KEY (`TrueAuthorId`) REFERENCES `characters` (`Id`) ON DELETE SET NULL,
  CONSTRAINT `FK_Writings_Languages` FOREIGN KEY (`LanguageId`) REFERENCES `languages` (`Id`) ON DELETE RESTRICT,
  CONSTRAINT `FK_Writings_Scripts` FOREIGN KEY (`ScriptId`) REFERENCES `scripts` (`Id`) ON DELETE RESTRICT
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Dumping data for table writings
--

/*!40000 ALTER TABLE `writings` DISABLE KEYS */;
/*!40000 ALTER TABLE `writings` ENABLE KEYS */;

--
-- Definition of writingcollectionentries
--

DROP TABLE IF EXISTS `writingcollectionentries`;
CREATE TABLE IF NOT EXISTS `writingcollectionentries` (
  `Id` bigint NOT NULL AUTO_INCREMENT,
  `WritingCollectionId` bigint NOT NULL,
  `PageNumber` int NOT NULL,
  `DisplayOrder` int NOT NULL,
  `WritingId` bigint DEFAULT NULL,
  `DrawingId` bigint DEFAULT NULL,
  PRIMARY KEY (`Id`),
  KEY `FK_WritingCollectionEntries_Collections_idx` (`WritingCollectionId`),
  KEY `FK_WritingCollectionEntries_Drawings_idx` (`DrawingId`),
  KEY `FK_WritingCollectionEntries_Writings_idx` (`WritingId`),
  KEY `IX_WritingCollectionEntries_Page_Order` (`WritingCollectionId`,`PageNumber`,`DisplayOrder`),
  CONSTRAINT `FK_WritingCollectionEntries_Collections` FOREIGN KEY (`WritingCollectionId`) REFERENCES `writingcollections` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_WritingCollectionEntries_Drawings` FOREIGN KEY (`DrawingId`) REFERENCES `drawings` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_WritingCollectionEntries_Writings` FOREIGN KEY (`WritingId`) REFERENCES `writings` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Dumping data for table writingcollectionentries
--

/*!40000 ALTER TABLE `writingcollectionentries` DISABLE KEYS */;
/*!40000 ALTER TABLE `writingcollectionentries` ENABLE KEYS */;

--
-- Definition of zones
--

DROP TABLE IF EXISTS `zones`;
CREATE TABLE IF NOT EXISTS `zones` (
  `Id` bigint NOT NULL AUTO_INCREMENT,
  `Name` varchar(255) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `ShardId` bigint NOT NULL,
  `Latitude` double NOT NULL,
  `Longitude` double NOT NULL,
  `Elevation` double NOT NULL,
  `DefaultCellId` bigint DEFAULT NULL,
  `AmbientLightPollution` double NOT NULL,
  `ForagableProfileId` bigint DEFAULT NULL,
  `WeatherControllerId` bigint DEFAULT NULL,
  PRIMARY KEY (`Id`),
  KEY `FK_Zones_Cells` (`DefaultCellId`),
  KEY `FK_Zones_Shards` (`ShardId`),
  KEY `FK_Zones_WeatherControllers_idx` (`WeatherControllerId`),
  CONSTRAINT `FK_Zones_Cells` FOREIGN KEY (`DefaultCellId`) REFERENCES `cells` (`Id`) ON DELETE RESTRICT,
  CONSTRAINT `FK_Zones_Shards` FOREIGN KEY (`ShardId`) REFERENCES `shards` (`Id`) ON DELETE RESTRICT,
  CONSTRAINT `FK_Zones_WeatherControllers` FOREIGN KEY (`WeatherControllerId`) REFERENCES `weathercontrollers` (`Id`) ON DELETE RESTRICT
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Dumping data for table zones
--

/*!40000 ALTER TABLE `zones` DISABLE KEYS */;
/*!40000 ALTER TABLE `zones` ENABLE KEYS */;

--
-- Definition of hooks_perceivables
--

DROP TABLE IF EXISTS `hooks_perceivables`;
CREATE TABLE IF NOT EXISTS `hooks_perceivables` (
  `Id` bigint NOT NULL AUTO_INCREMENT,
  `HookId` bigint NOT NULL,
  `BodyId` bigint DEFAULT NULL,
  `CharacterId` bigint DEFAULT NULL,
  `GameItemId` bigint DEFAULT NULL,
  `CellId` bigint DEFAULT NULL,
  `ZoneId` bigint DEFAULT NULL,
  `ShardId` bigint DEFAULT NULL,
  PRIMARY KEY (`Id`),
  KEY `FK_Hooks_Perceivables_Bodies_idx` (`BodyId`),
  KEY `FK_Hooks_Perceivables_Cells_idx` (`CellId`),
  KEY `FK_Hooks_Perceivables_Characters_idx` (`CharacterId`),
  KEY `FK_Hooks_Perceivables_GameItems_idx` (`GameItemId`),
  KEY `FK_Hooks_Perceivables_Hooks_idx` (`HookId`),
  KEY `FK_Hooks_Perceivables_Shards_idx` (`ShardId`),
  KEY `FK_Hooks_Perceivables_Zones_idx` (`ZoneId`),
  CONSTRAINT `FK_Hooks_Perceivables_Bodies` FOREIGN KEY (`BodyId`) REFERENCES `bodies` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_Hooks_Perceivables_Cells` FOREIGN KEY (`CellId`) REFERENCES `cells` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_Hooks_Perceivables_Characters` FOREIGN KEY (`CharacterId`) REFERENCES `characters` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_Hooks_Perceivables_GameItems` FOREIGN KEY (`GameItemId`) REFERENCES `gameitems` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_Hooks_Perceivables_Hooks` FOREIGN KEY (`HookId`) REFERENCES `hooks` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_Hooks_Perceivables_Shards` FOREIGN KEY (`ShardId`) REFERENCES `shards` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_Hooks_Perceivables_Zones` FOREIGN KEY (`ZoneId`) REFERENCES `zones` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Dumping data for table hooks_perceivables
--

/*!40000 ALTER TABLE `hooks_perceivables` DISABLE KEYS */;
/*!40000 ALTER TABLE `hooks_perceivables` ENABLE KEYS */;

--
-- Definition of legalauthorities_zones
--

DROP TABLE IF EXISTS `legalauthorities_zones`;
CREATE TABLE IF NOT EXISTS `legalauthorities_zones` (
  `ZoneId` bigint NOT NULL,
  `LegalAuthorityId` bigint NOT NULL,
  PRIMARY KEY (`ZoneId`,`LegalAuthorityId`),
  KEY `FK_LegalAuthorities_Zones_LegalAuthorities_idx` (`LegalAuthorityId`),
  CONSTRAINT `FK_LegalAuthorities_Zones_LegalAuthorities` FOREIGN KEY (`LegalAuthorityId`) REFERENCES `legalauthorities` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_LegalAuthorities_Zones_Zones` FOREIGN KEY (`ZoneId`) REFERENCES `zones` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Dumping data for table legalauthorities_zones
--

/*!40000 ALTER TABLE `legalauthorities_zones` DISABLE KEYS */;
/*!40000 ALTER TABLE `legalauthorities_zones` ENABLE KEYS */;

--
-- Definition of npcspawnerzones
--

DROP TABLE IF EXISTS `npcspawnerzones`;
CREATE TABLE IF NOT EXISTS `npcspawnerzones` (
  `NPCSpawnerId` bigint NOT NULL,
  `ZoneId` bigint NOT NULL,
  PRIMARY KEY (`NPCSpawnerId`,`ZoneId`),
  KEY `IX_NPCSpawnerZones_ZoneId` (`ZoneId`),
  CONSTRAINT `FK_NPCSpawnerZones_NPCSpawner` FOREIGN KEY (`NPCSpawnerId`) REFERENCES `npcspawners` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_NPCSpawnerZones_Zone` FOREIGN KEY (`ZoneId`) REFERENCES `zones` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Dumping data for table npcspawnerzones
--

/*!40000 ALTER TABLE `npcspawnerzones` DISABLE KEYS */;
/*!40000 ALTER TABLE `npcspawnerzones` ENABLE KEYS */;

--
-- Definition of rooms
--

DROP TABLE IF EXISTS `rooms`;
CREATE TABLE IF NOT EXISTS `rooms` (
  `Id` bigint NOT NULL AUTO_INCREMENT,
  `ZoneId` bigint NOT NULL,
  `X` int NOT NULL,
  `Y` int NOT NULL,
  `Z` int NOT NULL,
  PRIMARY KEY (`Id`),
  KEY `FK_Rooms_Zones` (`ZoneId`),
  CONSTRAINT `FK_Rooms_Zones` FOREIGN KEY (`ZoneId`) REFERENCES `zones` (`Id`) ON DELETE RESTRICT
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Dumping data for table rooms
--

/*!40000 ALTER TABLE `rooms` DISABLE KEYS */;
/*!40000 ALTER TABLE `rooms` ENABLE KEYS */;

--
-- Definition of areas_rooms
--

DROP TABLE IF EXISTS `areas_rooms`;
CREATE TABLE IF NOT EXISTS `areas_rooms` (
  `AreaId` bigint NOT NULL,
  `RoomId` bigint NOT NULL,
  PRIMARY KEY (`AreaId`,`RoomId`),
  KEY `FK_Areas_Rooms_Rooms_idx` (`RoomId`),
  CONSTRAINT `FK_Areas_Rooms_Areas` FOREIGN KEY (`AreaId`) REFERENCES `areas` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_Areas_Rooms_Rooms` FOREIGN KEY (`RoomId`) REFERENCES `rooms` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Dumping data for table areas_rooms
--

/*!40000 ALTER TABLE `areas_rooms` DISABLE KEYS */;
/*!40000 ALTER TABLE `areas_rooms` ENABLE KEYS */;

--
-- Definition of cells
--

DROP TABLE IF EXISTS `cells`;
CREATE TABLE IF NOT EXISTS `cells` (
  `Id` bigint NOT NULL AUTO_INCREMENT,
  `RoomId` bigint NOT NULL,
  `CurrentOverlayId` bigint DEFAULT NULL,
  `ForagableProfileId` bigint DEFAULT NULL,
  `Temporary` bit(1) NOT NULL DEFAULT b'0',
  `EffectData` mediumtext CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `SurfaceLiquidData` mediumtext CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci,
  `HostedVehicleCompartmentId` bigint DEFAULT NULL,
  `HostedVehicleId` bigint DEFAULT NULL,
  PRIMARY KEY (`Id`),
  UNIQUE KEY `UX_Cells_HostedVehicleCompartments` (`HostedVehicleCompartmentId`),
  KEY `FK_Cells_CellOverlays` (`CurrentOverlayId`),
  KEY `FK_Cells_Rooms` (`RoomId`),
  KEY `FK_Cells_HostedVehicles_idx` (`HostedVehicleId`),
  CONSTRAINT `FK_Cells_CellOverlays` FOREIGN KEY (`CurrentOverlayId`) REFERENCES `celloverlays` (`Id`) ON DELETE SET NULL,
  CONSTRAINT `FK_Cells_HostedVehicleCompartments` FOREIGN KEY (`HostedVehicleCompartmentId`) REFERENCES `vehiclecompartments` (`Id`) ON DELETE RESTRICT,
  CONSTRAINT `FK_Cells_HostedVehicles` FOREIGN KEY (`HostedVehicleId`) REFERENCES `vehicles` (`Id`) ON DELETE RESTRICT,
  CONSTRAINT `FK_Cells_Rooms` FOREIGN KEY (`RoomId`) REFERENCES `rooms` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `CK_Cells_HostedVehicleOwnership` CHECK ((((`HostedVehicleId` is null) and (`HostedVehicleCompartmentId` is null)) or ((`HostedVehicleId` is not null) and (`HostedVehicleCompartmentId` is not null))))
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Dumping data for table cells
--

/*!40000 ALTER TABLE `cells` DISABLE KEYS */;
/*!40000 ALTER TABLE `cells` ENABLE KEYS */;

--
-- Definition of arenacells
--

DROP TABLE IF EXISTS `arenacells`;
CREATE TABLE IF NOT EXISTS `arenacells` (
  `Id` bigint NOT NULL AUTO_INCREMENT,
  `ArenaId` bigint NOT NULL,
  `CellId` bigint NOT NULL,
  `Role` int NOT NULL,
  PRIMARY KEY (`Id`),
  KEY `FK_ArenaCells_Arenas` (`ArenaId`),
  KEY `FK_ArenaCells_Cells` (`CellId`),
  CONSTRAINT `FK_ArenaCells_Arenas` FOREIGN KEY (`ArenaId`) REFERENCES `arenas` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_ArenaCells_Cells` FOREIGN KEY (`CellId`) REFERENCES `cells` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Dumping data for table arenacells
--

/*!40000 ALTER TABLE `arenacells` DISABLE KEYS */;
/*!40000 ALTER TABLE `arenacells` ENABLE KEYS */;

--
-- Definition of bankbranches
--

DROP TABLE IF EXISTS `bankbranches`;
CREATE TABLE IF NOT EXISTS `bankbranches` (
  `BankId` bigint NOT NULL,
  `CellId` bigint NOT NULL,
  PRIMARY KEY (`BankId`,`CellId`),
  KEY `IX_BankBranches_CellId` (`CellId`),
  CONSTRAINT `FK_BankBranches_Banks` FOREIGN KEY (`BankId`) REFERENCES `banks` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_BankBranches_Cells` FOREIGN KEY (`CellId`) REFERENCES `cells` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Dumping data for table bankbranches
--

/*!40000 ALTER TABLE `bankbranches` DISABLE KEYS */;
/*!40000 ALTER TABLE `bankbranches` ENABLE KEYS */;

--
-- Definition of cells_foragableyields
--

DROP TABLE IF EXISTS `cells_foragableyields`;
CREATE TABLE IF NOT EXISTS `cells_foragableyields` (
  `CellId` bigint NOT NULL,
  `ForagableType` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL,
  `Yield` double NOT NULL,
  PRIMARY KEY (`CellId`,`ForagableType`),
  CONSTRAINT `FK_Cells_ForagableYields_Cells` FOREIGN KEY (`CellId`) REFERENCES `cells` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Dumping data for table cells_foragableyields
--

/*!40000 ALTER TABLE `cells_foragableyields` DISABLE KEYS */;
/*!40000 ALTER TABLE `cells_foragableyields` ENABLE KEYS */;

--
-- Definition of corpserecoveryreports
--

DROP TABLE IF EXISTS `corpserecoveryreports`;
CREATE TABLE IF NOT EXISTS `corpserecoveryreports` (
  `Id` bigint NOT NULL AUTO_INCREMENT,
  `LegalAuthorityId` bigint NOT NULL,
  `EconomicZoneId` bigint NOT NULL,
  `CorpseId` bigint NOT NULL,
  `SourceCellId` bigint NOT NULL,
  `DestinationCellId` bigint NOT NULL,
  `ReporterId` bigint DEFAULT NULL,
  `AssignedPatrolId` bigint DEFAULT NULL,
  `Status` int NOT NULL,
  PRIMARY KEY (`Id`),
  KEY `IX_CorpseRecoveryReports_AssignedPatrolId` (`AssignedPatrolId`),
  KEY `IX_CorpseRecoveryReports_CorpseId` (`CorpseId`),
  KEY `IX_CorpseRecoveryReports_DestinationCellId` (`DestinationCellId`),
  KEY `IX_CorpseRecoveryReports_EconomicZoneId` (`EconomicZoneId`),
  KEY `IX_CorpseRecoveryReports_LegalAuthorityId` (`LegalAuthorityId`),
  KEY `IX_CorpseRecoveryReports_ReporterId` (`ReporterId`),
  KEY `IX_CorpseRecoveryReports_SourceCellId` (`SourceCellId`),
  CONSTRAINT `FK_CorpseRecoveryReports_Characters` FOREIGN KEY (`ReporterId`) REFERENCES `characters` (`Id`) ON DELETE SET NULL,
  CONSTRAINT `FK_CorpseRecoveryReports_DestinationCells` FOREIGN KEY (`DestinationCellId`) REFERENCES `cells` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_CorpseRecoveryReports_EconomicZones` FOREIGN KEY (`EconomicZoneId`) REFERENCES `economiczones` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_CorpseRecoveryReports_GameItems` FOREIGN KEY (`CorpseId`) REFERENCES `gameitems` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_CorpseRecoveryReports_LegalAuthorities` FOREIGN KEY (`LegalAuthorityId`) REFERENCES `legalauthorities` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_CorpseRecoveryReports_Patrols` FOREIGN KEY (`AssignedPatrolId`) REFERENCES `patrols` (`Id`) ON DELETE SET NULL,
  CONSTRAINT `FK_CorpseRecoveryReports_SourceCells` FOREIGN KEY (`SourceCellId`) REFERENCES `cells` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Dumping data for table corpserecoveryreports
--

/*!40000 ALTER TABLE `corpserecoveryreports` DISABLE KEYS */;
/*!40000 ALTER TABLE `corpserecoveryreports` ENABLE KEYS */;

--
-- Definition of legalauthorities
--

DROP TABLE IF EXISTS `legalauthorities`;
CREATE TABLE IF NOT EXISTS `legalauthorities` (
  `Id` bigint NOT NULL AUTO_INCREMENT,
  `Name` varchar(250) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `CurrencyId` bigint NOT NULL,
  `EnforcerStowingLocationId` bigint DEFAULT NULL,
  `MarshallingLocationId` bigint DEFAULT NULL,
  `PlayersKnowTheirCrimes` bit(1) NOT NULL DEFAULT b'0',
  `PreparingLocationId` bigint DEFAULT NULL,
  `PrisonLocationId` bigint DEFAULT NULL,
  `OnImprisonProgId` bigint DEFAULT NULL,
  `OnReleaseProgId` bigint DEFAULT NULL,
  `PrisonBelongingsLocationId` bigint DEFAULT NULL,
  `PrisonReleaseLocationId` bigint DEFAULT NULL,
  `AutomaticConvictionTime` double NOT NULL DEFAULT '0',
  `AutomaticallyConvict` tinyint(1) NOT NULL DEFAULT '0',
  `BailCalculationProgId` bigint DEFAULT NULL,
  `BankAccountId` bigint DEFAULT NULL,
  `CourtLocationId` bigint DEFAULT NULL,
  `GuardianDiscordChannel` decimal(20,0) DEFAULT NULL,
  `JailLocationId` bigint DEFAULT NULL,
  `OnHoldProgId` bigint DEFAULT NULL,
  PRIMARY KEY (`Id`),
  KEY `FK_LegalAuthorities_Currencies_idx` (`CurrencyId`),
  KEY `FK_LegalAuthorities_MarshallingCells_idx` (`MarshallingLocationId`),
  KEY `FK_LegalAuthorities_PreparingCells_idx` (`PreparingLocationId`),
  KEY `FK_LegalAuthorities_PrisonCells_idx` (`PrisonLocationId`),
  KEY `FK_LegalAuthorities_StowingCells_idx` (`EnforcerStowingLocationId`),
  KEY `FK_LegalAuthorities_FutureprogsImprison_idx` (`OnImprisonProgId`),
  KEY `FK_LegalAuthorities_FutureprogsRelease_idx` (`OnReleaseProgId`),
  KEY `FK_LegalAuthorities_PrisonBelongingsCells_idx` (`PrisonBelongingsLocationId`),
  KEY `FK_LegalAuthorities_PrisonReleaseCells_idx` (`PrisonReleaseLocationId`),
  KEY `IX_LegalAuthorities_BailCalculationProgId` (`BailCalculationProgId`),
  KEY `IX_LegalAuthorities_BankAccountId` (`BankAccountId`),
  KEY `IX_LegalAuthorities_CourtLocationId` (`CourtLocationId`),
  KEY `IX_LegalAuthorities_JailLocationId` (`JailLocationId`),
  KEY `IX_LegalAuthorities_OnHoldProgId` (`OnHoldProgId`),
  CONSTRAINT `FK_LegalAuthorities_BankAccounts_BankAccountId` FOREIGN KEY (`BankAccountId`) REFERENCES `bankaccounts` (`Id`) ON DELETE RESTRICT,
  CONSTRAINT `FK_LegalAuthorities_CourtroomCell` FOREIGN KEY (`CourtLocationId`) REFERENCES `cells` (`Id`) ON DELETE RESTRICT,
  CONSTRAINT `FK_LegalAuthorities_Currencies` FOREIGN KEY (`CurrencyId`) REFERENCES `currencies` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_LegalAuthorities_FutureprogsBailCalc` FOREIGN KEY (`BailCalculationProgId`) REFERENCES `futureprogs` (`Id`) ON DELETE RESTRICT,
  CONSTRAINT `FK_LegalAuthorities_FutureprogsHold` FOREIGN KEY (`OnHoldProgId`) REFERENCES `futureprogs` (`Id`) ON DELETE RESTRICT,
  CONSTRAINT `FK_LegalAuthorities_FutureprogsImprison` FOREIGN KEY (`OnImprisonProgId`) REFERENCES `futureprogs` (`Id`) ON DELETE RESTRICT,
  CONSTRAINT `FK_LegalAuthorities_FutureprogsRelease` FOREIGN KEY (`OnReleaseProgId`) REFERENCES `futureprogs` (`Id`) ON DELETE RESTRICT,
  CONSTRAINT `FK_LegalAuthorities_MarshallingCells` FOREIGN KEY (`MarshallingLocationId`) REFERENCES `cells` (`Id`) ON DELETE RESTRICT,
  CONSTRAINT `FK_LegalAuthorities_PreparingCells` FOREIGN KEY (`PreparingLocationId`) REFERENCES `cells` (`Id`) ON DELETE RESTRICT,
  CONSTRAINT `FK_LegalAuthorities_PrisonBelongingsCells` FOREIGN KEY (`PrisonBelongingsLocationId`) REFERENCES `cells` (`Id`) ON DELETE RESTRICT,
  CONSTRAINT `FK_LegalAuthorities_PrisonCells` FOREIGN KEY (`PrisonLocationId`) REFERENCES `cells` (`Id`) ON DELETE RESTRICT,
  CONSTRAINT `FK_LegalAuthorities_PrisonJailCells` FOREIGN KEY (`JailLocationId`) REFERENCES `cells` (`Id`) ON DELETE RESTRICT,
  CONSTRAINT `FK_LegalAuthorities_PrisonReleaseCells` FOREIGN KEY (`PrisonReleaseLocationId`) REFERENCES `cells` (`Id`) ON DELETE RESTRICT,
  CONSTRAINT `FK_LegalAuthorities_StowingCells` FOREIGN KEY (`EnforcerStowingLocationId`) REFERENCES `cells` (`Id`) ON DELETE RESTRICT
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Dumping data for table legalauthorities
--

/*!40000 ALTER TABLE `legalauthorities` DISABLE KEYS */;
/*!40000 ALTER TABLE `legalauthorities` ENABLE KEYS */;

--
-- Definition of enforcementauthorities
--

DROP TABLE IF EXISTS `enforcementauthorities`;
CREATE TABLE IF NOT EXISTS `enforcementauthorities` (
  `Id` bigint NOT NULL AUTO_INCREMENT,
  `Name` varchar(250) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `LegalAuthorityId` bigint NOT NULL,
  `Priority` int NOT NULL,
  `CanAccuse` bit(1) NOT NULL,
  `CanForgive` bit(1) NOT NULL,
  `CanConvict` bit(1) NOT NULL,
  `FilterProgId` bigint DEFAULT NULL,
  PRIMARY KEY (`Id`),
  KEY `FK_EnforcementAuthorities_LegalAuthorities_idx` (`LegalAuthorityId`),
  KEY `FK_EnforcementAuthorities_FutureProgs_idx` (`FilterProgId`),
  CONSTRAINT `FK_EnforcementAuthorities_FutureProgs` FOREIGN KEY (`FilterProgId`) REFERENCES `futureprogs` (`Id`) ON DELETE RESTRICT,
  CONSTRAINT `FK_EnforcementAuthorities_LegalAuthorities` FOREIGN KEY (`LegalAuthorityId`) REFERENCES `legalauthorities` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Dumping data for table enforcementauthorities
--

/*!40000 ALTER TABLE `enforcementauthorities` DISABLE KEYS */;
/*!40000 ALTER TABLE `enforcementauthorities` ENABLE KEYS */;

--
-- Definition of enforcementauthorities_parentauthorities
--

DROP TABLE IF EXISTS `enforcementauthorities_parentauthorities`;
CREATE TABLE IF NOT EXISTS `enforcementauthorities_parentauthorities` (
  `ParentId` bigint NOT NULL,
  `ChildId` bigint NOT NULL,
  PRIMARY KEY (`ParentId`,`ChildId`),
  KEY `FK_EnforcementAuthorities_ParentAuthorities_Child_idx` (`ChildId`),
  CONSTRAINT `FK_EnforcementAuthorities_ParentAuthorities_Child` FOREIGN KEY (`ChildId`) REFERENCES `enforcementauthorities` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_EnforcementAuthorities_ParentAuthorities_Parent` FOREIGN KEY (`ParentId`) REFERENCES `enforcementauthorities` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Dumping data for table enforcementauthorities_parentauthorities
--

/*!40000 ALTER TABLE `enforcementauthorities_parentauthorities` DISABLE KEYS */;
/*!40000 ALTER TABLE `enforcementauthorities_parentauthorities` ENABLE KEYS */;

--
-- Definition of laws
--

DROP TABLE IF EXISTS `laws`;
CREATE TABLE IF NOT EXISTS `laws` (
  `Id` bigint NOT NULL AUTO_INCREMENT,
  `Name` varchar(250) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `LegalAuthorityId` bigint NOT NULL,
  `CrimeType` int NOT NULL,
  `ActivePeriod` double NOT NULL,
  `EnforcementStrategy` varchar(500) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `LawAppliesProgId` bigint DEFAULT NULL,
  `EnforcementPriority` int NOT NULL,
  `CanBeAppliedAutomatically` bit(1) NOT NULL,
  `CanBeArrested` bit(1) NOT NULL,
  `CanBeOfferedBail` bit(1) NOT NULL,
  `DoNotAutomaticallyApplyRepeats` bit(1) NOT NULL DEFAULT b'0',
  `PunishmentStrategy` text CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  PRIMARY KEY (`Id`),
  KEY `FK_Laws_FutureProgs_idx` (`LawAppliesProgId`),
  KEY `FK_Laws_LegalAuthority_idx` (`LegalAuthorityId`),
  CONSTRAINT `FK_Laws_FutureProgs` FOREIGN KEY (`LawAppliesProgId`) REFERENCES `futureprogs` (`Id`) ON DELETE SET NULL,
  CONSTRAINT `FK_Laws_LegalAuthority` FOREIGN KEY (`LegalAuthorityId`) REFERENCES `legalauthorities` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Dumping data for table laws
--

/*!40000 ALTER TABLE `laws` DISABLE KEYS */;
/*!40000 ALTER TABLE `laws` ENABLE KEYS */;

--
-- Definition of legalauthoritiycells
--

DROP TABLE IF EXISTS `legalauthoritiycells`;
CREATE TABLE IF NOT EXISTS `legalauthoritiycells` (
  `LegalAuthorityId` bigint NOT NULL,
  `CellId` bigint NOT NULL,
  PRIMARY KEY (`LegalAuthorityId`,`CellId`),
  KEY `FK_LegalAuthoritiesCells_Cells_idx` (`CellId`),
  KEY `FK_LegalAuthoritiesCells_LegalAuthorities_idx` (`LegalAuthorityId`),
  CONSTRAINT `FK_LegalAuthoritiesCells_Cells` FOREIGN KEY (`CellId`) REFERENCES `cells` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_LegalAuthoritiesCells_LegalAuthorities` FOREIGN KEY (`LegalAuthorityId`) REFERENCES `legalauthorities` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Dumping data for table legalauthoritiycells
--

/*!40000 ALTER TABLE `legalauthoritiycells` DISABLE KEYS */;
/*!40000 ALTER TABLE `legalauthoritiycells` ENABLE KEYS */;

--
-- Definition of legalauthorityfines
--

DROP TABLE IF EXISTS `legalauthorityfines`;
CREATE TABLE IF NOT EXISTS `legalauthorityfines` (
  `LegalAuthorityId` bigint NOT NULL,
  `CharacterId` bigint NOT NULL,
  `FinesOwned` decimal(58,29) NOT NULL,
  `PaymentRequiredBy` varchar(100) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  PRIMARY KEY (`LegalAuthorityId`,`CharacterId`),
  KEY `IX_LegalAuthorityFines_CharacterId` (`CharacterId`),
  CONSTRAINT `FK_LegalAuthorityFines_Characters` FOREIGN KEY (`CharacterId`) REFERENCES `characters` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_LegalAuthorityFines_LegalAuthorities` FOREIGN KEY (`LegalAuthorityId`) REFERENCES `legalauthorities` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Dumping data for table legalauthorityfines
--

/*!40000 ALTER TABLE `legalauthorityfines` DISABLE KEYS */;
/*!40000 ALTER TABLE `legalauthorityfines` ENABLE KEYS */;

--
-- Definition of legalauthorityjailcells
--

DROP TABLE IF EXISTS `legalauthorityjailcells`;
CREATE TABLE IF NOT EXISTS `legalauthorityjailcells` (
  `LegalAuthorityId` bigint NOT NULL,
  `CellId` bigint NOT NULL,
  PRIMARY KEY (`LegalAuthorityId`,`CellId`),
  KEY `FK_LegalAuthoritiesCells_Cells_Jail_idx` (`CellId`),
  KEY `FK_LegalAuthoritiesCells_LegalAuthorities_Jail_idx` (`LegalAuthorityId`),
  CONSTRAINT `FK_LegalAuthoritiesCells_Cells_Jail` FOREIGN KEY (`CellId`) REFERENCES `cells` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_LegalAuthoritiesCells_LegalAuthorities_Jail` FOREIGN KEY (`LegalAuthorityId`) REFERENCES `legalauthorities` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Dumping data for table legalauthorityjailcells
--

/*!40000 ALTER TABLE `legalauthorityjailcells` DISABLE KEYS */;
/*!40000 ALTER TABLE `legalauthorityjailcells` ENABLE KEYS */;

--
-- Definition of legalclasses
--

DROP TABLE IF EXISTS `legalclasses`;
CREATE TABLE IF NOT EXISTS `legalclasses` (
  `Id` bigint NOT NULL AUTO_INCREMENT,
  `Name` varchar(250) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `LegalAuthorityId` bigint NOT NULL,
  `LegalClassPriority` int NOT NULL,
  `MembershipProgId` bigint NOT NULL,
  `CanBeDetainedUntilFinesPaid` bit(1) NOT NULL,
  PRIMARY KEY (`Id`),
  KEY `FK_LegalClasses_LegalAuthorities_idx` (`LegalAuthorityId`),
  KEY `FK_LegalClasses_FutureProgs_idx` (`MembershipProgId`),
  CONSTRAINT `FK_LegalClasses_FutureProgs` FOREIGN KEY (`MembershipProgId`) REFERENCES `futureprogs` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_LegalClasses_LegalAuthorities` FOREIGN KEY (`LegalAuthorityId`) REFERENCES `legalauthorities` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Dumping data for table legalclasses
--

/*!40000 ALTER TABLE `legalclasses` DISABLE KEYS */;
/*!40000 ALTER TABLE `legalclasses` ENABLE KEYS */;

--
-- Definition of enforcementauthorities_accusableclasses
--

DROP TABLE IF EXISTS `enforcementauthorities_accusableclasses`;
CREATE TABLE IF NOT EXISTS `enforcementauthorities_accusableclasses` (
  `EnforcementAuthorityId` bigint NOT NULL,
  `LegalClassId` bigint NOT NULL,
  PRIMARY KEY (`EnforcementAuthorityId`,`LegalClassId`),
  KEY `FK_EnforcementAuthorities_AccusableClasses_LegalClasses_idx` (`LegalClassId`),
  CONSTRAINT `FK_EnforcementAuthorities_AccusableClasses_Enforce` FOREIGN KEY (`EnforcementAuthorityId`) REFERENCES `enforcementauthorities` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_EnforcementAuthorities_AccusableClasses_LegalClasses` FOREIGN KEY (`LegalClassId`) REFERENCES `legalclasses` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Dumping data for table enforcementauthorities_accusableclasses
--

/*!40000 ALTER TABLE `enforcementauthorities_accusableclasses` DISABLE KEYS */;
/*!40000 ALTER TABLE `enforcementauthorities_accusableclasses` ENABLE KEYS */;

--
-- Definition of enforcementauthoritiesarrestableclasses
--

DROP TABLE IF EXISTS `enforcementauthoritiesarrestableclasses`;
CREATE TABLE IF NOT EXISTS `enforcementauthoritiesarrestableclasses` (
  `EnforcementAuthorityId` bigint NOT NULL,
  `LegalClassId` bigint NOT NULL,
  PRIMARY KEY (`EnforcementAuthorityId`,`LegalClassId`),
  KEY `FK_EnforcementAuthoritiesArrestableLegalClasses_Enforce_idx` (`EnforcementAuthorityId`),
  KEY `FK_EnforcementAuthoritiesArrestableLegalClasses_LegalClasses_idx` (`LegalClassId`),
  CONSTRAINT `FK_EnforcementAuthoritiesArrestableLegalClasses_Enforce` FOREIGN KEY (`EnforcementAuthorityId`) REFERENCES `enforcementauthorities` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_EnforcementAuthoritiesArrestableLegalClasses_LegalClasses` FOREIGN KEY (`LegalClassId`) REFERENCES `legalclasses` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Dumping data for table enforcementauthoritiesarrestableclasses
--

/*!40000 ALTER TABLE `enforcementauthoritiesarrestableclasses` DISABLE KEYS */;
/*!40000 ALTER TABLE `enforcementauthoritiesarrestableclasses` ENABLE KEYS */;

--
-- Definition of laws_offenderclasses
--

DROP TABLE IF EXISTS `laws_offenderclasses`;
CREATE TABLE IF NOT EXISTS `laws_offenderclasses` (
  `LawId` bigint NOT NULL,
  `LegalClassId` bigint NOT NULL,
  PRIMARY KEY (`LawId`,`LegalClassId`),
  KEY `FK_Laws_OffenderClasses_LegalClasses_idx` (`LegalClassId`),
  CONSTRAINT `FK_Laws_OffenderClasses_Laws` FOREIGN KEY (`LawId`) REFERENCES `laws` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_Laws_OffenderClasses_LegalClasses` FOREIGN KEY (`LegalClassId`) REFERENCES `legalclasses` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Dumping data for table laws_offenderclasses
--

/*!40000 ALTER TABLE `laws_offenderclasses` DISABLE KEYS */;
/*!40000 ALTER TABLE `laws_offenderclasses` ENABLE KEYS */;

--
-- Definition of laws_victimclasses
--

DROP TABLE IF EXISTS `laws_victimclasses`;
CREATE TABLE IF NOT EXISTS `laws_victimclasses` (
  `LawId` bigint NOT NULL,
  `LegalClassId` bigint NOT NULL,
  PRIMARY KEY (`LawId`,`LegalClassId`),
  KEY `FK_Laws_VictimClasses_LegalClasses_idx` (`LegalClassId`),
  CONSTRAINT `FK_Laws_VictimClasses_Laws` FOREIGN KEY (`LawId`) REFERENCES `laws` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_Laws_VictimClasses_LegalClasses` FOREIGN KEY (`LegalClassId`) REFERENCES `legalclasses` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Dumping data for table laws_victimclasses
--

/*!40000 ALTER TABLE `laws_victimclasses` DISABLE KEYS */;
/*!40000 ALTER TABLE `laws_victimclasses` ENABLE KEYS */;

--
-- Definition of patrolroutes
--

DROP TABLE IF EXISTS `patrolroutes`;
CREATE TABLE IF NOT EXISTS `patrolroutes` (
  `Id` bigint NOT NULL AUTO_INCREMENT,
  `Name` varchar(100) DEFAULT NULL,
  `LegalAuthorityId` bigint NOT NULL,
  `LingerTimeMajorNode` double NOT NULL,
  `LingerTimeMinorNode` double NOT NULL,
  `Priority` int NOT NULL,
  `PatrolStrategy` varchar(100) DEFAULT NULL,
  `StartPatrolProgId` bigint DEFAULT NULL,
  `IsReady` bit(1) NOT NULL DEFAULT b'0',
  `StrategyData` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  PRIMARY KEY (`Id`),
  KEY `FK_PatrolRoutes_LegalAuthorities_idx` (`LegalAuthorityId`),
  KEY `IX_PatrolRoutes_StartPatrolProgId` (`StartPatrolProgId`),
  CONSTRAINT `FK_PatrolRoutes_FutureProgs_StartPatrolProgId` FOREIGN KEY (`StartPatrolProgId`) REFERENCES `futureprogs` (`Id`) ON DELETE RESTRICT,
  CONSTRAINT `FK_PatrolRoutes_LegalAuthorities` FOREIGN KEY (`LegalAuthorityId`) REFERENCES `legalauthorities` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Dumping data for table patrolroutes
--

/*!40000 ALTER TABLE `patrolroutes` DISABLE KEYS */;
/*!40000 ALTER TABLE `patrolroutes` ENABLE KEYS */;

--
-- Definition of patrolroutesnodes
--

DROP TABLE IF EXISTS `patrolroutesnodes`;
CREATE TABLE IF NOT EXISTS `patrolroutesnodes` (
  `PatrolRouteId` bigint NOT NULL,
  `CellId` bigint NOT NULL,
  `Order` int NOT NULL,
  PRIMARY KEY (`PatrolRouteId`,`CellId`),
  KEY `FK_PatrolRoutesNodes_Cells_idx` (`CellId`),
  KEY `FK_PatrolRoutesNodes_PatrolRoutes_idx` (`PatrolRouteId`),
  CONSTRAINT `FK_PatrolRoutesNodes_Cells` FOREIGN KEY (`CellId`) REFERENCES `cells` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_PatrolRoutesNodes_PatrolRoutes` FOREIGN KEY (`PatrolRouteId`) REFERENCES `patrolroutes` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Dumping data for table patrolroutesnodes
--

/*!40000 ALTER TABLE `patrolroutesnodes` DISABLE KEYS */;
/*!40000 ALTER TABLE `patrolroutesnodes` ENABLE KEYS */;

--
-- Definition of patrolroutesnumbers
--

DROP TABLE IF EXISTS `patrolroutesnumbers`;
CREATE TABLE IF NOT EXISTS `patrolroutesnumbers` (
  `PatrolRouteId` bigint NOT NULL,
  `EnforcementAuthorityId` bigint NOT NULL,
  `NumberRequired` int NOT NULL,
  PRIMARY KEY (`PatrolRouteId`,`EnforcementAuthorityId`),
  KEY `FK_PatrolRoutesNumbers_EnforcementAuthorities_idx` (`EnforcementAuthorityId`),
  KEY `FK_PatrolRoutesNumbers_PatrolRoutes_idx` (`PatrolRouteId`),
  CONSTRAINT `FK_PatrolRoutesNumbers_EnforcementAuthorities` FOREIGN KEY (`EnforcementAuthorityId`) REFERENCES `enforcementauthorities` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_PatrolRoutesNumbers_PatrolRoutes` FOREIGN KEY (`PatrolRouteId`) REFERENCES `patrolroutes` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Dumping data for table patrolroutesnumbers
--

/*!40000 ALTER TABLE `patrolroutesnumbers` DISABLE KEYS */;
/*!40000 ALTER TABLE `patrolroutesnumbers` ENABLE KEYS */;

--
-- Definition of patrolroutestimesofday
--

DROP TABLE IF EXISTS `patrolroutestimesofday`;
CREATE TABLE IF NOT EXISTS `patrolroutestimesofday` (
  `PatrolRouteId` bigint NOT NULL,
  `TimeOfDay` int NOT NULL,
  PRIMARY KEY (`PatrolRouteId`,`TimeOfDay`),
  KEY `FK_PatrolRoutesTimesOfDay_PatrolRoutes_idx` (`PatrolRouteId`),
  CONSTRAINT `FK_PatrolRoutesTimesOfDay_PatrolRoutes` FOREIGN KEY (`PatrolRouteId`) REFERENCES `patrolroutes` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Dumping data for table patrolroutestimesofday
--

/*!40000 ALTER TABLE `patrolroutestimesofday` DISABLE KEYS */;
/*!40000 ALTER TABLE `patrolroutestimesofday` ENABLE KEYS */;

--
-- Definition of patrols
--

DROP TABLE IF EXISTS `patrols`;
CREATE TABLE IF NOT EXISTS `patrols` (
  `Id` bigint NOT NULL AUTO_INCREMENT,
  `PatrolRouteId` bigint NOT NULL,
  `LegalAuthorityId` bigint NOT NULL,
  `PatrolPhase` int NOT NULL,
  `LastMajorNodeId` bigint DEFAULT NULL,
  `NextMajorNodeId` bigint DEFAULT NULL,
  `PatrolLeaderId` bigint DEFAULT NULL,
  `CharacterId` bigint DEFAULT NULL,
  `PatrolLeaderInstanceId` bigint DEFAULT NULL,
  PRIMARY KEY (`Id`),
  KEY `FK_Patrols_Characters_idx` (`PatrolLeaderId`),
  KEY `FK_Patrols_LastMajorNode_idx` (`LastMajorNodeId`),
  KEY `FK_Patrols_LegalAuthorities_idx` (`LegalAuthorityId`),
  KEY `FK_Patrols_NextMajorNode_idx` (`NextMajorNodeId`),
  KEY `FK_Patrols_PatrolRoutes_idx` (`PatrolRouteId`),
  KEY `IX_Patrols_CharacterId` (`CharacterId`),
  KEY `FK_Patrols_CharacterInstances_Leader_idx` (`PatrolLeaderInstanceId`),
  CONSTRAINT `FK_Patrols_Characters` FOREIGN KEY (`PatrolLeaderId`) REFERENCES `characters` (`Id`) ON DELETE RESTRICT,
  CONSTRAINT `FK_Patrols_Characters_CharacterId` FOREIGN KEY (`CharacterId`) REFERENCES `characters` (`Id`) ON DELETE RESTRICT,
  CONSTRAINT `FK_Patrols_LastMajorNode` FOREIGN KEY (`LastMajorNodeId`) REFERENCES `cells` (`Id`) ON DELETE RESTRICT,
  CONSTRAINT `FK_Patrols_LegalAuthorities` FOREIGN KEY (`LegalAuthorityId`) REFERENCES `legalauthorities` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_Patrols_NextMajorNode` FOREIGN KEY (`NextMajorNodeId`) REFERENCES `cells` (`Id`) ON DELETE RESTRICT,
  CONSTRAINT `FK_Patrols_PatrolRoutes` FOREIGN KEY (`PatrolRouteId`) REFERENCES `patrolroutes` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Dumping data for table patrols
--

/*!40000 ALTER TABLE `patrols` DISABLE KEYS */;
/*!40000 ALTER TABLE `patrols` ENABLE KEYS */;

--
-- Definition of patrolmembers
--

DROP TABLE IF EXISTS `patrolmembers`;
CREATE TABLE IF NOT EXISTS `patrolmembers` (
  `PatrolId` bigint NOT NULL,
  `CharacterId` bigint NOT NULL,
  `CharacterInstanceId` bigint NOT NULL DEFAULT '0',
  PRIMARY KEY (`PatrolId`,`CharacterId`),
  KEY `IX_PatrolMembers_CharacterId` (`CharacterId`),
  KEY `FK_PatrolMembers_CharacterInstances_idx` (`CharacterInstanceId`),
  CONSTRAINT `FK_PatrolMembers_Characters` FOREIGN KEY (`CharacterId`) REFERENCES `characters` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_PatrolsMembers_Patrols` FOREIGN KEY (`PatrolId`) REFERENCES `patrols` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Dumping data for table patrolmembers
--

/*!40000 ALTER TABLE `patrolmembers` DISABLE KEYS */;
/*!40000 ALTER TABLE `patrolmembers` ENABLE KEYS */;

--
-- Definition of routecells
--

DROP TABLE IF EXISTS `routecells`;
CREATE TABLE IF NOT EXISTS `routecells` (
  `CellId` bigint NOT NULL,
  `LengthMetres` decimal(18,3) NOT NULL,
  `DefaultPositionMetres` decimal(18,3) NOT NULL,
  `PositiveDirectionName` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL,
  `NegativeDirectionName` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL,
  `MetresPerRoomEquivalent` decimal(18,3) NOT NULL,
  `TopologyVersion` bigint NOT NULL DEFAULT '1',
  PRIMARY KEY (`CellId`),
  CONSTRAINT `FK_RouteCells_Cells` FOREIGN KEY (`CellId`) REFERENCES `cells` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `CK_RouteCells_DefaultPosition` CHECK (((`DefaultPositionMetres` >= 0) and (`DefaultPositionMetres` <= `LengthMetres`))),
  CONSTRAINT `CK_RouteCells_Length` CHECK ((`LengthMetres` > 0)),
  CONSTRAINT `CK_RouteCells_RoomEquivalent` CHECK ((`MetresPerRoomEquivalent` > 0)),
  CONSTRAINT `CK_RouteCells_TopologyVersion` CHECK ((`TopologyVersion` >= 1))
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Dumping data for table routecells
--

/*!40000 ALTER TABLE `routecells` DISABLE KEYS */;
/*!40000 ALTER TABLE `routecells` ENABLE KEYS */;

--
-- Definition of activeroutemotions
--

DROP TABLE IF EXISTS `activeroutemotions`;
CREATE TABLE IF NOT EXISTS `activeroutemotions` (
  `Id` bigint NOT NULL AUTO_INCREMENT,
  `MoverType` int NOT NULL,
  `MoverId` bigint NOT NULL,
  `RouteCellId` bigint NOT NULL,
  `RoomLayer` int NOT NULL,
  `CheckpointPositionMetres` decimal(18,3) NOT NULL,
  `TargetMinimumPositionMetres` decimal(18,3) NOT NULL,
  `TargetMaximumPositionMetres` decimal(18,3) NOT NULL,
  `Direction` int NOT NULL,
  `SpeedMetresPerSecond` decimal(18,6) NOT NULL,
  `RemainingDurationMilliseconds` bigint NOT NULL,
  `TopologyVersion` bigint NOT NULL,
  `Status` int NOT NULL,
  `OperationId` varchar(64) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL,
  `CheckpointSequence` bigint NOT NULL,
  `SelectedExitId` bigint DEFAULT NULL,
  `StateData` mediumtext CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci,
  `CreatedDateTime` datetime(6) NOT NULL,
  `LastCheckpointDateTime` datetime(6) NOT NULL,
  PRIMARY KEY (`Id`),
  UNIQUE KEY `UX_ActiveRouteMotions_Mover` (`MoverType`,`MoverId`),
  UNIQUE KEY `UX_ActiveRouteMotions_Operation` (`OperationId`),
  KEY `FK_ActiveRouteMotions_Exits_idx` (`SelectedExitId`),
  KEY `IX_ActiveRouteMotions_RouteCell_Layer_Status` (`RouteCellId`,`RoomLayer`,`Status`),
  CONSTRAINT `FK_ActiveRouteMotions_Exits` FOREIGN KEY (`SelectedExitId`) REFERENCES `exits` (`Id`) ON DELETE SET NULL,
  CONSTRAINT `FK_ActiveRouteMotions_RouteCells` FOREIGN KEY (`RouteCellId`) REFERENCES `routecells` (`CellId`) ON DELETE CASCADE,
  CONSTRAINT `CK_ActiveRouteMotions_Checkpoint` CHECK ((`CheckpointPositionMetres` >= 0)),
  CONSTRAINT `CK_ActiveRouteMotions_Direction` CHECK ((`Direction` in (-(1),1))),
  CONSTRAINT `CK_ActiveRouteMotions_RemainingDuration` CHECK ((`RemainingDurationMilliseconds` >= 0)),
  CONSTRAINT `CK_ActiveRouteMotions_Sequence` CHECK ((`CheckpointSequence` >= 0)),
  CONSTRAINT `CK_ActiveRouteMotions_Speed` CHECK ((`SpeedMetresPerSecond` > 0)),
  CONSTRAINT `CK_ActiveRouteMotions_TargetBand` CHECK (((`TargetMinimumPositionMetres` >= 0) and (`TargetMaximumPositionMetres` >= `TargetMinimumPositionMetres`))),
  CONSTRAINT `CK_ActiveRouteMotions_TopologyVersion` CHECK ((`TopologyVersion` >= 1))
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Dumping data for table activeroutemotions
--

/*!40000 ALTER TABLE `activeroutemotions` DISABLE KEYS */;
/*!40000 ALTER TABLE `activeroutemotions` ENABLE KEYS */;

--
-- Definition of routecelllandmarks
--

DROP TABLE IF EXISTS `routecelllandmarks`;
CREATE TABLE IF NOT EXISTS `routecelllandmarks` (
  `Id` bigint NOT NULL AUTO_INCREMENT,
  `RouteCellId` bigint NOT NULL,
  `Name` varchar(200) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL,
  `Keywords` varchar(500) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL,
  `Description` text CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL,
  `PositionMetres` decimal(18,3) NOT NULL,
  `DisplayOrder` int NOT NULL,
  PRIMARY KEY (`Id`),
  KEY `IX_RouteCellLandmarks_RouteCell_Position` (`RouteCellId`,`PositionMetres`),
  CONSTRAINT `FK_RouteCellLandmarks_RouteCells` FOREIGN KEY (`RouteCellId`) REFERENCES `routecells` (`CellId`) ON DELETE CASCADE,
  CONSTRAINT `CK_RouteCellLandmarks_Position` CHECK ((`PositionMetres` >= 0))
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Dumping data for table routecelllandmarks
--

/*!40000 ALTER TABLE `routecelllandmarks` DISABLE KEYS */;
/*!40000 ALTER TABLE `routecelllandmarks` ENABLE KEYS */;

--
-- Definition of routeexitanchors
--

DROP TABLE IF EXISTS `routeexitanchors`;
CREATE TABLE IF NOT EXISTS `routeexitanchors` (
  `ExitId` bigint NOT NULL,
  `RouteCellId` bigint NOT NULL,
  `MinimumPositionMetres` decimal(18,3) NOT NULL,
  `MaximumPositionMetres` decimal(18,3) NOT NULL,
  `ArrivalPositionMetres` decimal(18,3) NOT NULL,
  PRIMARY KEY (`ExitId`,`RouteCellId`),
  KEY `IX_RouteExitAnchors_RouteCell_Band` (`RouteCellId`,`MinimumPositionMetres`,`MaximumPositionMetres`),
  CONSTRAINT `FK_RouteExitAnchors_Exits` FOREIGN KEY (`ExitId`) REFERENCES `exits` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_RouteExitAnchors_RouteCells` FOREIGN KEY (`RouteCellId`) REFERENCES `routecells` (`CellId`) ON DELETE CASCADE,
  CONSTRAINT `CK_RouteExitAnchors_Arrival` CHECK (((`ArrivalPositionMetres` >= `MinimumPositionMetres`) and (`ArrivalPositionMetres` <= `MaximumPositionMetres`))),
  CONSTRAINT `CK_RouteExitAnchors_Band` CHECK (((`MinimumPositionMetres` >= 0) and (`MaximumPositionMetres` >= `MinimumPositionMetres`)))
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Dumping data for table routeexitanchors
--

/*!40000 ALTER TABLE `routeexitanchors` DISABLE KEYS */;
/*!40000 ALTER TABLE `routeexitanchors` ENABLE KEYS */;

--
-- Definition of routemotionresourceledgers
--

DROP TABLE IF EXISTS `routemotionresourceledgers`;
CREATE TABLE IF NOT EXISTS `routemotionresourceledgers` (
  `Id` bigint NOT NULL AUTO_INCREMENT,
  `ActiveRouteMotionId` bigint NOT NULL,
  `CheckpointSequence` bigint NOT NULL,
  `IdempotencyKey` varchar(200) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL,
  `ResourceOwnerType` int NOT NULL,
  `ResourceOwnerId` bigint NOT NULL,
  `ResourceType` int NOT NULL,
  `ResourceReferenceId` bigint DEFAULT NULL,
  `ResourceKey` varchar(200) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL,
  `ReservedAmount` decimal(18,6) NOT NULL,
  `ConsumedAmount` decimal(18,6) NOT NULL,
  `Status` int NOT NULL,
  `CreatedDateTime` datetime(6) NOT NULL,
  `CommittedDateTime` datetime(6) DEFAULT NULL,
  PRIMARY KEY (`Id`),
  UNIQUE KEY `UX_RouteMotionResourceLedgers_Idempotency` (`IdempotencyKey`),
  KEY `IX_RouteMotionResourceLedgers_Motion_Sequence` (`ActiveRouteMotionId`,`CheckpointSequence`),
  CONSTRAINT `FK_RouteMotionResourceLedgers_ActiveRouteMotions` FOREIGN KEY (`ActiveRouteMotionId`) REFERENCES `activeroutemotions` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `CK_RouteMotionResourceLedgers_Amounts` CHECK (((`ReservedAmount` >= 0) and (`ConsumedAmount` >= 0) and (`ConsumedAmount` <= `ReservedAmount`))),
  CONSTRAINT `CK_RouteMotionResourceLedgers_Sequence` CHECK ((`CheckpointSequence` >= 0))
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Dumping data for table routemotionresourceledgers
--

/*!40000 ALTER TABLE `routemotionresourceledgers` DISABLE KEYS */;
/*!40000 ALTER TABLE `routemotionresourceledgers` ENABLE KEYS */;

--
-- Definition of zones_timezones
--

DROP TABLE IF EXISTS `zones_timezones`;
CREATE TABLE IF NOT EXISTS `zones_timezones` (
  `ZoneId` bigint NOT NULL,
  `ClockId` bigint NOT NULL,
  `TimezoneId` bigint NOT NULL,
  PRIMARY KEY (`ZoneId`,`ClockId`,`TimezoneId`),
  CONSTRAINT `FK_Zones_Timezones_Zones` FOREIGN KEY (`ZoneId`) REFERENCES `zones` (`Id`) ON DELETE RESTRICT
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Dumping data for table zones_timezones
--

/*!40000 ALTER TABLE `zones_timezones` DISABLE KEYS */;
/*!40000 ALTER TABLE `zones_timezones` ENABLE KEYS */;

--
-- Definition of hotelbannedpatrons
--

DROP TABLE IF EXISTS `hotelbannedpatrons`;
CREATE TABLE IF NOT EXISTS `hotelbannedpatrons` (
  `HotelId` bigint NOT NULL,
  `PatronId` bigint NOT NULL,
  PRIMARY KEY (`HotelId`,`PatronId`),
  KEY `IX_HotelBannedPatrons_Patron` (`PatronId`),
  CONSTRAINT `FK_HotelBannedPatrons_Hotels` FOREIGN KEY (`HotelId`) REFERENCES `hotels` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Dumping data for table hotelbannedpatrons
--

/*!40000 ALTER TABLE `hotelbannedpatrons` DISABLE KEYS */;
/*!40000 ALTER TABLE `hotelbannedpatrons` ENABLE KEYS */;

--
-- Definition of hotellostproperties
--

DROP TABLE IF EXISTS `hotellostproperties`;
CREATE TABLE IF NOT EXISTS `hotellostproperties` (
  `Id` bigint NOT NULL AUTO_INCREMENT,
  `HotelId` bigint NOT NULL,
  `HotelRoomId` bigint NOT NULL,
  `OwnerId` bigint NOT NULL,
  `BundleId` bigint NOT NULL,
  `StoredUntil` varchar(100) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `Status` int NOT NULL,
  `AuctionHouseId` bigint DEFAULT NULL,
  `ReservePrice` decimal(58,29) NOT NULL,
  `Description` varchar(500) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  PRIMARY KEY (`Id`),
  KEY `FK_HotelLostProperties_HotelRooms_idx` (`HotelRoomId`),
  KEY `FK_HotelLostProperties_Hotels_idx` (`HotelId`),
  KEY `IX_HotelLostProperties_Bundle` (`BundleId`),
  KEY `IX_HotelLostProperties_Owner` (`OwnerId`),
  CONSTRAINT `FK_HotelLostProperties_HotelRooms` FOREIGN KEY (`HotelRoomId`) REFERENCES `hotelrooms` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_HotelLostProperties_Hotels` FOREIGN KEY (`HotelId`) REFERENCES `hotels` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Dumping data for table hotellostproperties
--

/*!40000 ALTER TABLE `hotellostproperties` DISABLE KEYS */;
/*!40000 ALTER TABLE `hotellostproperties` ENABLE KEYS */;

--
-- Definition of hotelpatronbalances
--

DROP TABLE IF EXISTS `hotelpatronbalances`;
CREATE TABLE IF NOT EXISTS `hotelpatronbalances` (
  `HotelId` bigint NOT NULL,
  `PatronId` bigint NOT NULL,
  `Balance` decimal(58,29) NOT NULL,
  PRIMARY KEY (`HotelId`,`PatronId`),
  KEY `IX_HotelPatronBalances_Patron` (`PatronId`),
  CONSTRAINT `FK_HotelPatronBalances_Hotels` FOREIGN KEY (`HotelId`) REFERENCES `hotels` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Dumping data for table hotelpatronbalances
--

/*!40000 ALTER TABLE `hotelpatronbalances` DISABLE KEYS */;
/*!40000 ALTER TABLE `hotelpatronbalances` ENABLE KEYS */;

--
-- Definition of hotelroomfurnishings
--

DROP TABLE IF EXISTS `hotelroomfurnishings`;
CREATE TABLE IF NOT EXISTS `hotelroomfurnishings` (
  `Id` bigint NOT NULL AUTO_INCREMENT,
  `HotelRoomId` bigint NOT NULL,
  `GameItemId` bigint NOT NULL,
  `Description` varchar(500) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `ReplacementValue` decimal(58,29) NOT NULL,
  `OriginalCondition` double NOT NULL,
  `OriginalDamageCondition` double NOT NULL,
  PRIMARY KEY (`Id`),
  KEY `FK_HotelRoomFurnishings_HotelRooms_idx` (`HotelRoomId`),
  KEY `IX_HotelRoomFurnishings_GameItem` (`GameItemId`),
  CONSTRAINT `FK_HotelRoomFurnishings_HotelRooms` FOREIGN KEY (`HotelRoomId`) REFERENCES `hotelrooms` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Dumping data for table hotelroomfurnishings
--

/*!40000 ALTER TABLE `hotelroomfurnishings` DISABLE KEYS */;
/*!40000 ALTER TABLE `hotelroomfurnishings` ENABLE KEYS */;

--
-- Definition of hotelroomkeys
--

DROP TABLE IF EXISTS `hotelroomkeys`;
CREATE TABLE IF NOT EXISTS `hotelroomkeys` (
  `HotelRoomId` bigint NOT NULL,
  `PropertyKeyId` bigint NOT NULL,
  PRIMARY KEY (`HotelRoomId`,`PropertyKeyId`),
  KEY `FK_HotelRoomKeys_PropertyKeys_idx` (`PropertyKeyId`),
  CONSTRAINT `FK_HotelRoomKeys_HotelRooms` FOREIGN KEY (`HotelRoomId`) REFERENCES `hotelrooms` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_HotelRoomKeys_PropertyKeys` FOREIGN KEY (`PropertyKeyId`) REFERENCES `propertykeys` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Dumping data for table hotelroomkeys
--

/*!40000 ALTER TABLE `hotelroomkeys` DISABLE KEYS */;
/*!40000 ALTER TABLE `hotelroomkeys` ENABLE KEYS */;

--
-- Definition of hotelroomrentals
--

DROP TABLE IF EXISTS `hotelroomrentals`;
CREATE TABLE IF NOT EXISTS `hotelroomrentals` (
  `Id` bigint NOT NULL AUTO_INCREMENT,
  `HotelRoomId` bigint NOT NULL,
  `GuestId` bigint NOT NULL,
  `StartTime` varchar(100) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `EndTime` varchar(100) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `RentalCharge` decimal(58,29) NOT NULL,
  `SecurityDeposit` decimal(58,29) NOT NULL,
  `TaxCharged` decimal(58,29) NOT NULL,
  PRIMARY KEY (`Id`),
  UNIQUE KEY `IX_HotelRoomRentals_Room` (`HotelRoomId`),
  KEY `IX_HotelRoomRentals_Guest` (`GuestId`),
  CONSTRAINT `FK_HotelRoomRentals_HotelRooms` FOREIGN KEY (`HotelRoomId`) REFERENCES `hotelrooms` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Dumping data for table hotelroomrentals
--

/*!40000 ALTER TABLE `hotelroomrentals` DISABLE KEYS */;
/*!40000 ALTER TABLE `hotelroomrentals` ENABLE KEYS */;

--
-- Definition of hotelrooms
--

DROP TABLE IF EXISTS `hotelrooms`;
CREATE TABLE IF NOT EXISTS `hotelrooms` (
  `Id` bigint NOT NULL AUTO_INCREMENT,
  `HotelId` bigint NOT NULL,
  `CellId` bigint NOT NULL,
  `Name` varchar(200) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `Listed` bit(1) NOT NULL,
  `PricePerDay` decimal(58,29) NOT NULL,
  `SecurityDeposit` decimal(58,29) NOT NULL,
  `MinimumDurationTicks` bigint NOT NULL,
  `MaximumDurationTicks` bigint NOT NULL,
  PRIMARY KEY (`Id`),
  UNIQUE KEY `IX_HotelRooms_Hotel_Cell` (`HotelId`,`CellId`),
  KEY `FK_HotelRooms_Cells_idx` (`CellId`),
  KEY `FK_HotelRooms_Hotels_idx` (`HotelId`),
  CONSTRAINT `FK_HotelRooms_Cells` FOREIGN KEY (`CellId`) REFERENCES `cells` (`Id`) ON DELETE RESTRICT,
  CONSTRAINT `FK_HotelRooms_Hotels` FOREIGN KEY (`HotelId`) REFERENCES `hotels` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Dumping data for table hotelrooms
--

/*!40000 ALTER TABLE `hotelrooms` DISABLE KEYS */;
/*!40000 ALTER TABLE `hotelrooms` ENABLE KEYS */;

--
-- Definition of hotels
--

DROP TABLE IF EXISTS `hotels`;
CREATE TABLE IF NOT EXISTS `hotels` (
  `Id` bigint NOT NULL AUTO_INCREMENT,
  `PropertyId` bigint NOT NULL,
  `BankAccountId` bigint DEFAULT NULL,
  `LicenseStatus` int NOT NULL,
  `CanRentProgId` bigint DEFAULT NULL,
  `LostPropertyRetention` varchar(200) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `OutstandingTaxes` decimal(58,29) NOT NULL,
  `CreatedAt` datetime NOT NULL,
  `LastUpdatedAt` datetime NOT NULL,
  PRIMARY KEY (`Id`),
  UNIQUE KEY `IX_Hotels_PropertyId` (`PropertyId`),
  KEY `FK_Hotels_BankAccounts_idx` (`BankAccountId`),
  KEY `FK_Hotels_FutureProgs_idx` (`CanRentProgId`),
  CONSTRAINT `FK_Hotels_BankAccounts` FOREIGN KEY (`BankAccountId`) REFERENCES `bankaccounts` (`Id`) ON DELETE SET NULL,
  CONSTRAINT `FK_Hotels_FutureProgs` FOREIGN KEY (`CanRentProgId`) REFERENCES `futureprogs` (`Id`) ON DELETE SET NULL,
  CONSTRAINT `FK_Hotels_Properties` FOREIGN KEY (`PropertyId`) REFERENCES `properties` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Dumping data for table hotels
--

/*!40000 ALTER TABLE `hotels` DISABLE KEYS */;
/*!40000 ALTER TABLE `hotels` ENABLE KEYS */;

--
-- Definition of properties
--

DROP TABLE IF EXISTS `properties`;
CREATE TABLE IF NOT EXISTS `properties` (
  `Id` bigint NOT NULL AUTO_INCREMENT,
  `Name` varchar(250) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `EconomicZoneId` bigint NOT NULL,
  `DetailedDescription` text CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `LastChangeOfOwnership` varchar(100) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `ApplyCriminalCodeInProperty` bit(1) NOT NULL,
  `LeaseId` bigint DEFAULT NULL,
  `LeaseOrderId` bigint DEFAULT NULL,
  `SaleOrderId` bigint DEFAULT NULL,
  `LastSaleValue` decimal(58,29) NOT NULL,
  PRIMARY KEY (`Id`),
  KEY `IX_Properties_EconomicZoneId` (`EconomicZoneId`),
  KEY `IX_Properties_LeaseId` (`LeaseId`),
  KEY `IX_Properties_LeaseOrderId` (`LeaseOrderId`),
  KEY `IX_Properties_SaleOrderId` (`SaleOrderId`),
  CONSTRAINT `FK_Properties_EconomicZones` FOREIGN KEY (`EconomicZoneId`) REFERENCES `economiczones` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_Properties_Lease` FOREIGN KEY (`LeaseId`) REFERENCES `propertyleases` (`Id`) ON DELETE RESTRICT,
  CONSTRAINT `FK_Properties_LeaseOrder` FOREIGN KEY (`LeaseOrderId`) REFERENCES `propertyleaseorders` (`Id`) ON DELETE RESTRICT,
  CONSTRAINT `FK_Properties_SaleOrder` FOREIGN KEY (`SaleOrderId`) REFERENCES `propertysalesorders` (`Id`) ON DELETE RESTRICT
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Dumping data for table properties
--

/*!40000 ALTER TABLE `properties` DISABLE KEYS */;
/*!40000 ALTER TABLE `properties` ENABLE KEYS */;

--
-- Definition of propertykeys
--

DROP TABLE IF EXISTS `propertykeys`;
CREATE TABLE IF NOT EXISTS `propertykeys` (
  `Id` bigint NOT NULL AUTO_INCREMENT,
  `Name` varchar(250) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `GameItemId` bigint NOT NULL,
  `PropertyId` bigint NOT NULL,
  `AddedToPropertyOnDate` varchar(100) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `CostToReplace` decimal(58,29) NOT NULL,
  `IsReturned` bit(1) NOT NULL,
  PRIMARY KEY (`Id`),
  KEY `IX_PropertyKeys_GameItemId` (`GameItemId`),
  KEY `IX_PropertyKeys_PropertyId` (`PropertyId`),
  CONSTRAINT `FK_PropertyKeys_GameItems` FOREIGN KEY (`GameItemId`) REFERENCES `gameitems` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_PropertyKeys_Property` FOREIGN KEY (`PropertyId`) REFERENCES `properties` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Dumping data for table propertykeys
--

/*!40000 ALTER TABLE `propertykeys` DISABLE KEYS */;
/*!40000 ALTER TABLE `propertykeys` ENABLE KEYS */;

--
-- Definition of propertyleaseorders
--

DROP TABLE IF EXISTS `propertyleaseorders`;
CREATE TABLE IF NOT EXISTS `propertyleaseorders` (
  `Id` bigint NOT NULL AUTO_INCREMENT,
  `PropertyId` bigint NOT NULL,
  `PricePerInterval` decimal(58,29) NOT NULL,
  `BondRequired` decimal(58,29) NOT NULL,
  `Interval` varchar(100) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `CanLeaseProgCharacterId` bigint DEFAULT NULL,
  `CanLeaseProgClanId` bigint DEFAULT NULL,
  `MinimumLeaseDurationDays` double NOT NULL,
  `MaximumLeaseDurationDays` double NOT NULL,
  `AllowAutoRenew` bit(1) NOT NULL,
  `AutomaticallyRelistAfterLeaseTerm` bit(1) NOT NULL,
  `AllowLeaseNovation` bit(1) NOT NULL,
  `ListedForLease` bit(1) NOT NULL,
  `FeeIncreasePercentageAfterLeaseTerm` decimal(58,29) NOT NULL,
  `PropertyOwnerConsentInfo` text CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `RekeyOnLeaseEnd` tinyint(1) NOT NULL DEFAULT '0',
  PRIMARY KEY (`Id`),
  KEY `IX_PropertyLeaseOrders_CanLeaseProgCharacterId` (`CanLeaseProgCharacterId`),
  KEY `IX_PropertyLeaseOrders_CanLeaseProgClanId` (`CanLeaseProgClanId`),
  KEY `IX_PropertyLeaseOrders_PropertyId` (`PropertyId`),
  CONSTRAINT `FK_PropertyLeaseOrders_FutureProgs_Character` FOREIGN KEY (`CanLeaseProgCharacterId`) REFERENCES `futureprogs` (`Id`) ON DELETE RESTRICT,
  CONSTRAINT `FK_PropertyLeaseOrders_FutureProgs_Clan` FOREIGN KEY (`CanLeaseProgClanId`) REFERENCES `futureprogs` (`Id`) ON DELETE RESTRICT,
  CONSTRAINT `FK_PropertyLeaseOrders_Property` FOREIGN KEY (`PropertyId`) REFERENCES `properties` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Dumping data for table propertyleaseorders
--

/*!40000 ALTER TABLE `propertyleaseorders` DISABLE KEYS */;
/*!40000 ALTER TABLE `propertyleaseorders` ENABLE KEYS */;

--
-- Definition of propertyleases
--

DROP TABLE IF EXISTS `propertyleases`;
CREATE TABLE IF NOT EXISTS `propertyleases` (
  `Id` bigint NOT NULL AUTO_INCREMENT,
  `PropertyId` bigint NOT NULL,
  `LeaseOrderId` bigint NOT NULL,
  `LeaseholderReference` varchar(100) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `PricePerInterval` decimal(58,29) NOT NULL,
  `BondPayment` decimal(58,29) NOT NULL,
  `PaymentBalance` decimal(58,29) NOT NULL,
  `LeaseStart` varchar(100) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `LeaseEnd` varchar(100) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `LastLeasePayment` varchar(100) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `AutoRenew` bit(1) NOT NULL,
  `BondReturned` bit(1) NOT NULL,
  `Interval` varchar(100) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `TenantInfo` text CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `BondClaimed` decimal(65,30) NOT NULL DEFAULT '0.000000000000000000000000000000',
  PRIMARY KEY (`Id`),
  KEY `IX_PropertyLeases_LeaseOrderId` (`LeaseOrderId`),
  KEY `IX_PropertyLeases_PropertyId` (`PropertyId`),
  CONSTRAINT `FK_PropertyLeases_Property` FOREIGN KEY (`PropertyId`) REFERENCES `properties` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_PropertyLeases_PropertyLeaseOrders` FOREIGN KEY (`LeaseOrderId`) REFERENCES `propertyleaseorders` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Dumping data for table propertyleases
--

/*!40000 ALTER TABLE `propertyleases` DISABLE KEYS */;
/*!40000 ALTER TABLE `propertyleases` ENABLE KEYS */;

--
-- Definition of propertylocations
--

DROP TABLE IF EXISTS `propertylocations`;
CREATE TABLE IF NOT EXISTS `propertylocations` (
  `PropertyId` bigint NOT NULL,
  `CellId` bigint NOT NULL,
  PRIMARY KEY (`PropertyId`,`CellId`),
  KEY `IX_PropertyLocations_CellId` (`CellId`),
  CONSTRAINT `FK_PropertyLocations_Cell` FOREIGN KEY (`CellId`) REFERENCES `cells` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_PropertyLocations_Property` FOREIGN KEY (`PropertyId`) REFERENCES `properties` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Dumping data for table propertylocations
--

/*!40000 ALTER TABLE `propertylocations` DISABLE KEYS */;
/*!40000 ALTER TABLE `propertylocations` ENABLE KEYS */;

--
-- Definition of propertyowners
--

DROP TABLE IF EXISTS `propertyowners`;
CREATE TABLE IF NOT EXISTS `propertyowners` (
  `Id` bigint NOT NULL AUTO_INCREMENT,
  `PropertyId` bigint NOT NULL,
  `FrameworkItemId` bigint NOT NULL,
  `FrameworkItemType` varchar(100) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `ShareOfOwnership` decimal(58,29) NOT NULL,
  `RevenueAccountId` bigint DEFAULT NULL,
  PRIMARY KEY (`Id`),
  KEY `IX_PropertyOwners_PropertyId` (`PropertyId`),
  KEY `IX_PropertyOwners_RevenueAccountId` (`RevenueAccountId`),
  CONSTRAINT `FK_PropertyOwners_BankAccounts` FOREIGN KEY (`RevenueAccountId`) REFERENCES `bankaccounts` (`Id`) ON DELETE RESTRICT,
  CONSTRAINT `FK_PropertyOwners_Properties` FOREIGN KEY (`PropertyId`) REFERENCES `properties` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Dumping data for table propertyowners
--

/*!40000 ALTER TABLE `propertyowners` DISABLE KEYS */;
/*!40000 ALTER TABLE `propertyowners` ENABLE KEYS */;

--
-- Definition of propertysalesorders
--

DROP TABLE IF EXISTS `propertysalesorders`;
CREATE TABLE IF NOT EXISTS `propertysalesorders` (
  `Id` bigint NOT NULL AUTO_INCREMENT,
  `PropertyId` bigint NOT NULL,
  `ReservePrice` decimal(58,29) NOT NULL,
  `OrderStatus` int NOT NULL,
  `StartOfListing` varchar(100) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `DurationOfListingDays` double NOT NULL,
  `PropertyOwnerConsentInfo` text CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  PRIMARY KEY (`Id`),
  KEY `IX_PropertySalesOrders_PropertyId` (`PropertyId`),
  CONSTRAINT `FK_PropertySaleOrders_Property` FOREIGN KEY (`PropertyId`) REFERENCES `properties` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Dumping data for table propertysalesorders
--

/*!40000 ALTER TABLE `propertysalesorders` DISABLE KEYS */;
/*!40000 ALTER TABLE `propertysalesorders` ENABLE KEYS */;


/*!40103 SET TIME_ZONE=@OLD_TIME_ZONE */;
/*!40101 SET SQL_MODE=@OLD_SQL_MODE */;
/*!40014 SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS */;
/*!40014 SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS */;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
/*!40111 SET SQL_NOTES=@OLD_SQL_NOTES */;


-- Dump completed on 2026-07-22 20:54:44
-- Total time: 0:0:0:1:986 (d:h:m:s:ms)
