CREATE DATABASE  IF NOT EXISTS `__FUTUREMUD_DATABASE__` /*!40100 DEFAULT CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci */ /*!80016 DEFAULT ENCRYPTION='N' */;
USE `__FUTUREMUD_DATABASE__`;
-- MySQL dump 10.13  Distrib 8.0.45, for Win64 (x86_64)
--
-- Host: localhost    Database: __FUTUREMUD_DATABASE__
-- ------------------------------------------------------
-- Server version	8.0.45

/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!50503 SET NAMES utf8 */;
/*!40103 SET @OLD_TIME_ZONE=@@TIME_ZONE */;
/*!40103 SET TIME_ZONE='+00:00' */;
/*!40014 SET @OLD_UNIQUE_CHECKS=@@UNIQUE_CHECKS, UNIQUE_CHECKS=0 */;
/*!40014 SET @OLD_FOREIGN_KEY_CHECKS=@@FOREIGN_KEY_CHECKS, FOREIGN_KEY_CHECKS=0 */;
/*!40101 SET @OLD_SQL_MODE=@@SQL_MODE, SQL_MODE='NO_AUTO_VALUE_ON_ZERO' */;
/*!40111 SET @OLD_SQL_NOTES=@@SQL_NOTES, SQL_NOTES=0 */;

--
-- Table structure for table `__efmigrationshistory`
--

DROP TABLE IF EXISTS `__efmigrationshistory`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `__efmigrationshistory` (
  `MigrationId` varchar(150) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `ProductVersion` varchar(32) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  PRIMARY KEY (`MigrationId`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `__efmigrationshistory`
--

LOCK TABLES `__efmigrationshistory` WRITE;
/*!40000 ALTER TABLE `__efmigrationshistory` DISABLE KEYS */;
INSERT INTO `__efmigrationshistory` VALUES ('20200626070704_InitialDatabase','9.0.11'),('20200728125151_MoveChargenToTables','9.0.11'),('20200807044450_EnforcementUpdate','9.0.11'),('20200810141606_ClanVoting','9.0.11'),('20200817061844_Elections','9.0.11'),('20200830233741_TerrainUpdate','9.0.11'),('20200905062837_CurrencyPatternEnhancement','9.0.11'),('20200928025908_KnowledgeBuilding','9.0.11'),('20201013213328_CheckFixing','9.0.11'),('20201014230837_FixingEmailTemplates','9.0.11'),('20201106014706_LineOfCreditAccounts','9.0.11'),('20201106040133_AttributesUpdate','9.0.11'),('20201108122141_EconomicZoneUpdate','9.0.11'),('20201113050353_EconomicZonesTouchup','9.0.11'),('20201120022913_EnforcermentAndMisc','9.0.11'),('20201120045951_MinorFixForCrime','9.0.11'),('20201129225407_SafeQuit','9.0.11'),('20201130014025_JournalUpdates','9.0.11'),('20201130041538_JournalUpdate','9.0.11'),('20201201052916_DrugReform','9.0.11'),('20201217051236_Changes','9.0.11'),('20201217051726_ExtraDescriptions','9.0.11'),('20201218014631_RacialBreathingChange','9.0.11'),('20201221031703_ClanFame','9.0.11'),('20201227120935_CantRemember','9.0.11'),('20210113052107_IndexFixForBodyparts','9.0.11'),('20210114010706_IndexAdditions','9.0.11'),('20210116210204_MagicSpells','9.0.11'),('20210118053537_MoreSpellStuff','9.0.11'),('20210119034150_MoreSpellStuff2','9.0.11'),('20210119035740_MoreSpellStuff3','9.0.11'),('20210120031933_MoreSpellStuff4','9.0.11'),('20210127032929_Jan21EnforcementWorkaround','9.0.11'),('20210202002906_RemovingChildClans','9.0.11'),('20210211035327_GameStatistics','9.0.11'),('20210224105856_NewSun','9.0.11'),('20210302112347_OngoingCheckForCharacteristics','9.0.11'),('20210331025006_BanksV1','9.0.11'),('20210423014825_WeaponAttackAddPositionRequirement','9.0.11'),('20210626110830_AuctionHouses','9.0.11'),('20210810123837_PropertyV1','9.0.11'),('20210902052233_PropertyV2','9.0.11'),('20210914132733_Sep21LawUpdate','9.0.11'),('20211025020630_JusticeOverhaulOct21','9.0.11'),('20211217034326_TerrainMapColourAddition','9.0.11'),('20211220045847_Skins','9.0.11'),('20211222033658_Skins-Pt2','9.0.11'),('20211226134159_ClanBankAccounts','9.0.11'),('20211229004501_PlayerBoards','9.0.11'),('20220104134109_JobsV1','9.0.11'),('20220105004035_ProjectsJobsUpdate','9.0.11'),('20220108004307_BoardBugFix','9.0.11'),('20220117102755_BoardsDescriptions','9.0.11'),('20220210215752_LongerAuthorFullDescs','9.0.11'),('20220225125641_ClanFKFixing','9.0.11'),('20220327052829_NPCSpawners','9.0.11'),('20220421132846_BodyCharacteristicsFix','9.0.11'),('20220625122517_ClanDiscordUpdate','9.0.11'),('20220625125136_ChargenResourcesAsDouble','9.0.11'),('20220718132632_MaterialsRefactor','9.0.11'),('20220731064708_TheoreticalCraftChecks','9.0.11'),('20220807101509_IntToDoubleParryBonus','9.0.11'),('20220814231930_RaceDefaultHwModels','9.0.11'),('20221030044209_ShopBankAccountsAndFinance','9.0.11'),('20221030125929_BankPaymentsAtShops','9.0.11'),('20221031113757_MagicSpellExclusivity','9.0.11'),('20221201081057_NameCulturesGenderExpansion','9.0.11'),('20221201133628_NameCulturesChargenExpansion','9.0.11'),('20230101133831_CurrencyPatternRegexCaseFix','9.0.11'),('20230110120837_RelativeEnthalpyForLiquids','9.0.11'),('20230124124618_SurgicalProcedureCheckTraits','9.0.11'),('20230311060208_SurgeryBodyUpdate','9.0.11'),('20230407151210_OpenAIv1','9.0.11'),('20230428004425_GlobalCurrencyChanges','9.0.11'),('20230603125906_CraftUseToolDuration','9.0.11'),('20230706055610_AuxiliaryMoves','9.0.11'),('20230714035824_AuxiliaryMoves2','9.0.11'),('20230727121209_SeasonsDisplayUpdate','9.0.11'),('20230731055842_SeederChoices','9.0.11'),('20230810071403_CellForeignKeyUpdates','9.0.11'),('20230825052231_NpcSpawnerMulti','9.0.11'),('20230825061651_ShopTypes','9.0.11'),('20230914142042_ScriptedEvents','9.0.11'),('20230917131132_ClanForeignKeyUpdate','9.0.11'),('20231031085439_MagicResourceColours','9.0.11'),('20231102120820_NewPlayerHints','9.0.11'),('20231110224309_HungerThirstRatesForRaces','9.0.11'),('20231125084220_ClimateModelSimplification','9.0.11'),('20231208235024_HeritageChargenCostBugFix','9.0.11'),('20240112055830_ChargenResourcesControlProg','9.0.11'),('20240119120217_CoinsChangeFlag','9.0.11'),('20240129025113_CurrencyForeignKeyUpdateJan24','9.0.11'),('20240305110906_BuyingMerchandise','9.0.11'),('20240325104238_MarketsV1','9.0.11'),('20240418112441_MarketsShopIntegration','9.0.11'),('20240427013621_MarketPopulations','9.0.11'),('20240601141550_DiscordOutputForChannels','9.0.11'),('20240615065145_ShopAutopayTaxes','9.0.11'),('20240730123726_TrackingV1','9.0.11'),('20240804070126_FixDatabaseAutoTrueBug','9.0.11'),('20240808232211_TrackingNameForBodyProtos','9.0.11'),('20240809155707_BMIUnits','9.0.11'),('20240816134208_ArmourPenaltyToDouble','9.0.11'),('20240817112644_HeightWeightModelDirectSetWeights','9.0.11'),('20240828105208_AutoReacquireTargetsSetting','9.0.11'),('20240828124859_CombatSettingsAugust24','9.0.11'),('20240831005804_BodypartGroupDescribersAugust2024','9.0.11'),('20240925062238_CrimesUpdate2024Sep25','9.0.11'),('20241011073405_StockroomNonMorphing','9.0.11'),('20241016054103_ItemProtoIsHiddenFromPlayers','9.0.11'),('20241016123415_BodyOverrideHealthStrategy','9.0.11'),('20241018071518_LiquidLeaveResidueInRooms','9.0.11'),('20241121010653_RemovingBreathableFluidsRaces','9.0.11'),('20241129002416_CriminalDescUpdates','9.0.11'),('20241216062012_RandomNamesBinaryUnicodeSort','9.0.11'),('20241220091815_EthnicitiesNameCultures','9.0.11'),('20241231030836_TagsForTerrains','9.0.11'),('20250101232454_SkewnessForHWModels','9.0.11'),('20250210095915_Shoppers','9.0.11'),('20250210233555_ShopsFeb25','9.0.11'),('20250211100238_ShopsFeb25P2','9.0.11'),('20250304042559_PreserveRegisterVariablesItemFlag','9.0.11'),('20250304104024_CraftPhaseExertionAndStamina','9.0.11'),('20250304114440_MerchandiseTransactionRecordsFix','9.0.11'),('20250424052852_SpellTriggerNullTargets','9.0.11'),('20250628230040_PropertyRekeyOnLeaseEnd','9.0.11'),('20251110082110_CombatArenaSchema','9.0.11'),('20251115120000_ArenaSignupEcho','9.0.11'),('20260211095519_AIStorytellers','9.0.11'),('20260211124139_AIStorytellerEventSubscriptions','9.0.11'),('20260216092441_AIStorytellerTimeSystemPrompt','9.0.11'),('20260216095426_AIStorytellerScopedModelReasoning','9.0.11'),('20260218120142_ArenaAutoScheduling','9.0.11'),('20260221112947_AIStorytellerSituationScopes','9.0.11'),('20260222081900_ArenaEventTypeEliminationModes','9.0.11'),('20260222112522_ArenaNpcCompletionRestore','9.0.11'),('20260225115630_ArenaStageNameProfile','9.0.11'),('20260225233442_ArenaEloStrategyOptions','9.0.11'),('20260226124500_ArenaSideRatingRanges','9.0.11'),('20260227120000_ArenaPhaseProgAppearancePayouts','9.0.11'),('20260308065322_ClimateDescriptions','9.0.11'),('20260309072751_WeatherModelSimplification','9.0.11'),('20260309222608_ReplaceCharacterCaloriesWithSatiationReserve','9.0.11'),('20260309225356_DropObsoleteNutritionCalories','9.0.11'),('20260310122815_InfectionVirulenceMultiplier','9.0.11'),('20260316112529_NaturalRangedAttacksAndElementalContact','9.0.11'),('20260320180000_CombatSettingPriorityAndRaceDefault','9.0.11'),('20260321102002_FutureProgTypeDefinitionsStage1','9.0.11'),('20260321102139_FutureProgTypeDefinitionsStage2','9.0.11'),('20260327103014_ShopDeals','9.0.11'),('20260327124234_EstateProbateAuctionLiquidation','9.0.11'),('20260328123631_EstateProbateMorgueWorkflow','9.0.11'),('20260329110346_EconomicZoneEstatesEnabledToggle','9.0.11'),('20260329223130_EstateWillsPayoutsAndPropertyShares','9.0.11'),('20260331222122_AddSolidMaterialAliases','9.0.11'),('20260402053811_RemoveOldSunCelestialDefault','9.0.11'),('20260415122407_MarketPopulationIncomeAndPricePressure','9.0.11');
/*!40000 ALTER TABLE `__efmigrationshistory` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `accents`
--

DROP TABLE IF EXISTS `accents`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `accents` (
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
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `accents`
--

LOCK TABLES `accents` WRITE;
/*!40000 ALTER TABLE `accents` DISABLE KEYS */;
/*!40000 ALTER TABLE `accents` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `accountnotes`
--

DROP TABLE IF EXISTS `accountnotes`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `accountnotes` (
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
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `accountnotes`
--

LOCK TABLES `accountnotes` WRITE;
/*!40000 ALTER TABLE `accountnotes` DISABLE KEYS */;
/*!40000 ALTER TABLE `accountnotes` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `accounts`
--

DROP TABLE IF EXISTS `accounts`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `accounts` (
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
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `accounts`
--

LOCK TABLES `accounts` WRITE;
/*!40000 ALTER TABLE `accounts` DISABLE KEYS */;
/*!40000 ALTER TABLE `accounts` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `accounts_chargenresources`
--

DROP TABLE IF EXISTS `accounts_chargenresources`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `accounts_chargenresources` (
  `AccountId` bigint NOT NULL,
  `ChargenResourceId` bigint NOT NULL,
  `Amount` double NOT NULL,
  `LastAwardDate` datetime NOT NULL,
  PRIMARY KEY (`AccountId`,`ChargenResourceId`),
  KEY `FK_Accounts_ChargenResources_ChargenResources` (`ChargenResourceId`),
  CONSTRAINT `FK_Accounts_ChargenResources_Accounts` FOREIGN KEY (`AccountId`) REFERENCES `accounts` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_Accounts_ChargenResources_ChargenResources` FOREIGN KEY (`ChargenResourceId`) REFERENCES `chargenresources` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `accounts_chargenresources`
--

LOCK TABLES `accounts_chargenresources` WRITE;
/*!40000 ALTER TABLE `accounts_chargenresources` DISABLE KEYS */;
/*!40000 ALTER TABLE `accounts_chargenresources` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `activejobs`
--

DROP TABLE IF EXISTS `activejobs`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `activejobs` (
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
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `activejobs`
--

LOCK TABLES `activejobs` WRITE;
/*!40000 ALTER TABLE `activejobs` DISABLE KEYS */;
/*!40000 ALTER TABLE `activejobs` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `activeprojectlabours`
--

DROP TABLE IF EXISTS `activeprojectlabours`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `activeprojectlabours` (
  `ActiveProjectId` bigint NOT NULL,
  `ProjectLabourRequirementsId` bigint NOT NULL,
  `Progress` double NOT NULL,
  PRIMARY KEY (`ActiveProjectId`,`ProjectLabourRequirementsId`),
  KEY `FK_ActiveProjectLabours_ProjectLabourRequirements_idx` (`ProjectLabourRequirementsId`),
  CONSTRAINT `FK_ActiveProjectLabours_ActiveProjects` FOREIGN KEY (`ActiveProjectId`) REFERENCES `activeprojects` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_ActiveProjectLabours_ProjectLabourRequirements` FOREIGN KEY (`ProjectLabourRequirementsId`) REFERENCES `projectlabourrequirements` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `activeprojectlabours`
--

LOCK TABLES `activeprojectlabours` WRITE;
/*!40000 ALTER TABLE `activeprojectlabours` DISABLE KEYS */;
/*!40000 ALTER TABLE `activeprojectlabours` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `activeprojectmaterials`
--

DROP TABLE IF EXISTS `activeprojectmaterials`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `activeprojectmaterials` (
  `ActiveProjectId` bigint NOT NULL,
  `ProjectMaterialRequirementsId` bigint NOT NULL,
  `Progress` double NOT NULL,
  PRIMARY KEY (`ActiveProjectId`,`ProjectMaterialRequirementsId`),
  KEY `FK_ActiveProjectMaterials_ProjectMaterialRequirements_idx` (`ProjectMaterialRequirementsId`),
  CONSTRAINT `FK_ActiveProjectMaterials_ActiveProjects` FOREIGN KEY (`ActiveProjectId`) REFERENCES `activeprojects` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_ActiveProjectMaterials_ProjectMaterialRequirements` FOREIGN KEY (`ProjectMaterialRequirementsId`) REFERENCES `projectmaterialrequirements` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `activeprojectmaterials`
--

LOCK TABLES `activeprojectmaterials` WRITE;
/*!40000 ALTER TABLE `activeprojectmaterials` DISABLE KEYS */;
/*!40000 ALTER TABLE `activeprojectmaterials` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `activeprojects`
--

DROP TABLE IF EXISTS `activeprojects`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `activeprojects` (
  `Id` bigint NOT NULL AUTO_INCREMENT,
  `ProjectId` bigint NOT NULL,
  `ProjectRevisionNumber` int NOT NULL,
  `CurrentPhaseId` bigint NOT NULL,
  `CharacterId` bigint DEFAULT NULL,
  `CellId` bigint DEFAULT NULL,
  PRIMARY KEY (`Id`),
  KEY `FK_ActiveProjects_Cells_idx` (`CellId`),
  KEY `FK_ActiveProjects_Characters_idx` (`CharacterId`),
  KEY `FK_ActiveProjects_ProjectPhases_idx` (`CurrentPhaseId`),
  KEY `FK_ActiveProjects_Projects_idx` (`ProjectId`,`ProjectRevisionNumber`),
  CONSTRAINT `FK_ActiveProjects_Cells` FOREIGN KEY (`CellId`) REFERENCES `cells` (`Id`) ON DELETE RESTRICT,
  CONSTRAINT `FK_ActiveProjects_Characters` FOREIGN KEY (`CharacterId`) REFERENCES `characters` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_ActiveProjects_ProjectPhases` FOREIGN KEY (`CurrentPhaseId`) REFERENCES `projectphases` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_ActiveProjects_Projects` FOREIGN KEY (`ProjectId`, `ProjectRevisionNumber`) REFERENCES `projects` (`Id`, `RevisionNumber`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `activeprojects`
--

LOCK TABLES `activeprojects` WRITE;
/*!40000 ALTER TABLE `activeprojects` DISABLE KEYS */;
/*!40000 ALTER TABLE `activeprojects` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `aistorytellercharactermemories`
--

DROP TABLE IF EXISTS `aistorytellercharactermemories`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `aistorytellercharactermemories` (
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
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `aistorytellercharactermemories`
--

LOCK TABLES `aistorytellercharactermemories` WRITE;
/*!40000 ALTER TABLE `aistorytellercharactermemories` DISABLE KEYS */;
/*!40000 ALTER TABLE `aistorytellercharactermemories` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `aistorytellerreferencedocuments`
--

DROP TABLE IF EXISTS `aistorytellerreferencedocuments`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `aistorytellerreferencedocuments` (
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
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `aistorytellerreferencedocuments`
--

LOCK TABLES `aistorytellerreferencedocuments` WRITE;
/*!40000 ALTER TABLE `aistorytellerreferencedocuments` DISABLE KEYS */;
/*!40000 ALTER TABLE `aistorytellerreferencedocuments` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `aistorytellers`
--

DROP TABLE IF EXISTS `aistorytellers`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `aistorytellers` (
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
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `aistorytellers`
--

LOCK TABLES `aistorytellers` WRITE;
/*!40000 ALTER TABLE `aistorytellers` DISABLE KEYS */;
/*!40000 ALTER TABLE `aistorytellers` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `aistorytellersituations`
--

DROP TABLE IF EXISTS `aistorytellersituations`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `aistorytellersituations` (
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
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `aistorytellersituations`
--

LOCK TABLES `aistorytellersituations` WRITE;
/*!40000 ALTER TABLE `aistorytellersituations` DISABLE KEYS */;
/*!40000 ALTER TABLE `aistorytellersituations` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `allies`
--

DROP TABLE IF EXISTS `allies`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `allies` (
  `CharacterId` bigint NOT NULL,
  `AllyId` bigint NOT NULL,
  `Trusted` bit(1) NOT NULL DEFAULT b'0',
  PRIMARY KEY (`CharacterId`,`AllyId`),
  KEY `FK_Allies_Characters_Target_idx` (`AllyId`),
  CONSTRAINT `FK_Allies_Characters_Owner` FOREIGN KEY (`CharacterId`) REFERENCES `characters` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_Allies_Characters_Target` FOREIGN KEY (`AllyId`) REFERENCES `characters` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `allies`
--

LOCK TABLES `allies` WRITE;
/*!40000 ALTER TABLE `allies` DISABLE KEYS */;
/*!40000 ALTER TABLE `allies` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `ammunitiontypes`
--

DROP TABLE IF EXISTS `ammunitiontypes`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `ammunitiontypes` (
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
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `ammunitiontypes`
--

LOCK TABLES `ammunitiontypes` WRITE;
/*!40000 ALTER TABLE `ammunitiontypes` DISABLE KEYS */;
/*!40000 ALTER TABLE `ammunitiontypes` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `appointments`
--

DROP TABLE IF EXISTS `appointments`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `appointments` (
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
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `appointments`
--

LOCK TABLES `appointments` WRITE;
/*!40000 ALTER TABLE `appointments` DISABLE KEYS */;
/*!40000 ALTER TABLE `appointments` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `appointments_abbreviations`
--

DROP TABLE IF EXISTS `appointments_abbreviations`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `appointments_abbreviations` (
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
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `appointments_abbreviations`
--

LOCK TABLES `appointments_abbreviations` WRITE;
/*!40000 ALTER TABLE `appointments_abbreviations` DISABLE KEYS */;
/*!40000 ALTER TABLE `appointments_abbreviations` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `appointments_titles`
--

DROP TABLE IF EXISTS `appointments_titles`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `appointments_titles` (
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
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `appointments_titles`
--

LOCK TABLES `appointments_titles` WRITE;
/*!40000 ALTER TABLE `appointments_titles` DISABLE KEYS */;
/*!40000 ALTER TABLE `appointments_titles` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `areas`
--

DROP TABLE IF EXISTS `areas`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `areas` (
  `Id` bigint NOT NULL AUTO_INCREMENT,
  `Name` varchar(200) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `WeatherControllerId` bigint DEFAULT NULL,
  PRIMARY KEY (`Id`),
  KEY `FK_Areas_WeatherControllers_idx` (`WeatherControllerId`),
  CONSTRAINT `FK_Areas_WeatherControllers` FOREIGN KEY (`WeatherControllerId`) REFERENCES `weathercontrollers` (`Id`) ON DELETE SET NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `areas`
--

LOCK TABLES `areas` WRITE;
/*!40000 ALTER TABLE `areas` DISABLE KEYS */;
/*!40000 ALTER TABLE `areas` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `areas_rooms`
--

DROP TABLE IF EXISTS `areas_rooms`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `areas_rooms` (
  `AreaId` bigint NOT NULL,
  `RoomId` bigint NOT NULL,
  PRIMARY KEY (`AreaId`,`RoomId`),
  KEY `FK_Areas_Rooms_Rooms_idx` (`RoomId`),
  CONSTRAINT `FK_Areas_Rooms_Areas` FOREIGN KEY (`AreaId`) REFERENCES `areas` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_Areas_Rooms_Rooms` FOREIGN KEY (`RoomId`) REFERENCES `rooms` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `areas_rooms`
--

LOCK TABLES `areas_rooms` WRITE;
/*!40000 ALTER TABLE `areas_rooms` DISABLE KEYS */;
/*!40000 ALTER TABLE `areas_rooms` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `arenabetpayouts`
--

DROP TABLE IF EXISTS `arenabetpayouts`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `arenabetpayouts` (
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
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `arenabetpayouts`
--

LOCK TABLES `arenabetpayouts` WRITE;
/*!40000 ALTER TABLE `arenabetpayouts` DISABLE KEYS */;
/*!40000 ALTER TABLE `arenabetpayouts` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `arenabetpools`
--

DROP TABLE IF EXISTS `arenabetpools`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `arenabetpools` (
  `Id` bigint NOT NULL AUTO_INCREMENT,
  `ArenaEventId` bigint NOT NULL,
  `SideIndex` int DEFAULT NULL,
  `TotalStake` decimal(58,29) NOT NULL,
  `TakeRate` decimal(58,29) NOT NULL,
  PRIMARY KEY (`Id`),
  KEY `FK_ArenaBetPools_ArenaEvents` (`ArenaEventId`),
  CONSTRAINT `FK_ArenaBetPools_ArenaEvents` FOREIGN KEY (`ArenaEventId`) REFERENCES `arenaevents` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `arenabetpools`
--

LOCK TABLES `arenabetpools` WRITE;
/*!40000 ALTER TABLE `arenabetpools` DISABLE KEYS */;
/*!40000 ALTER TABLE `arenabetpools` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `arenabets`
--

DROP TABLE IF EXISTS `arenabets`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `arenabets` (
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
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `arenabets`
--

LOCK TABLES `arenabets` WRITE;
/*!40000 ALTER TABLE `arenabets` DISABLE KEYS */;
/*!40000 ALTER TABLE `arenabets` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `arenacells`
--

DROP TABLE IF EXISTS `arenacells`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `arenacells` (
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
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `arenacells`
--

LOCK TABLES `arenacells` WRITE;
/*!40000 ALTER TABLE `arenacells` DISABLE KEYS */;
/*!40000 ALTER TABLE `arenacells` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `arenacombatantclasses`
--

DROP TABLE IF EXISTS `arenacombatantclasses`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `arenacombatantclasses` (
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
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `arenacombatantclasses`
--

LOCK TABLES `arenacombatantclasses` WRITE;
/*!40000 ALTER TABLE `arenacombatantclasses` DISABLE KEYS */;
/*!40000 ALTER TABLE `arenacombatantclasses` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `arenaeliminations`
--

DROP TABLE IF EXISTS `arenaeliminations`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `arenaeliminations` (
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
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `arenaeliminations`
--

LOCK TABLES `arenaeliminations` WRITE;
/*!40000 ALTER TABLE `arenaeliminations` DISABLE KEYS */;
/*!40000 ALTER TABLE `arenaeliminations` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `arenaevents`
--

DROP TABLE IF EXISTS `arenaevents`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `arenaevents` (
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
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `arenaevents`
--

LOCK TABLES `arenaevents` WRITE;
/*!40000 ALTER TABLE `arenaevents` DISABLE KEYS */;
/*!40000 ALTER TABLE `arenaevents` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `arenaeventsides`
--

DROP TABLE IF EXISTS `arenaeventsides`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `arenaeventsides` (
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
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `arenaeventsides`
--

LOCK TABLES `arenaeventsides` WRITE;
/*!40000 ALTER TABLE `arenaeventsides` DISABLE KEYS */;
/*!40000 ALTER TABLE `arenaeventsides` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `arenaeventtypes`
--

DROP TABLE IF EXISTS `arenaeventtypes`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `arenaeventtypes` (
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
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `arenaeventtypes`
--

LOCK TABLES `arenaeventtypes` WRITE;
/*!40000 ALTER TABLE `arenaeventtypes` DISABLE KEYS */;
/*!40000 ALTER TABLE `arenaeventtypes` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `arenaeventtypesideallowedclasses`
--

DROP TABLE IF EXISTS `arenaeventtypesideallowedclasses`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `arenaeventtypesideallowedclasses` (
  `ArenaEventTypeSideId` bigint NOT NULL,
  `ArenaCombatantClassId` bigint NOT NULL,
  PRIMARY KEY (`ArenaEventTypeSideId`,`ArenaCombatantClassId`),
  KEY `IX_ArenaEventTypeSideAllowedClasses_ArenaCombatantClassId` (`ArenaCombatantClassId`),
  CONSTRAINT `FK_ArenaEventTypeSideAllowedClasses_Classes` FOREIGN KEY (`ArenaCombatantClassId`) REFERENCES `arenacombatantclasses` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_ArenaEventTypeSideAllowedClasses_Sides` FOREIGN KEY (`ArenaEventTypeSideId`) REFERENCES `arenaeventtypesides` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `arenaeventtypesideallowedclasses`
--

LOCK TABLES `arenaeventtypesideallowedclasses` WRITE;
/*!40000 ALTER TABLE `arenaeventtypesideallowedclasses` DISABLE KEYS */;
/*!40000 ALTER TABLE `arenaeventtypesideallowedclasses` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `arenaeventtypesides`
--

DROP TABLE IF EXISTS `arenaeventtypesides`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `arenaeventtypesides` (
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
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `arenaeventtypesides`
--

LOCK TABLES `arenaeventtypesides` WRITE;
/*!40000 ALTER TABLE `arenaeventtypesides` DISABLE KEYS */;
/*!40000 ALTER TABLE `arenaeventtypesides` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `arenafinancesnapshots`
--

DROP TABLE IF EXISTS `arenafinancesnapshots`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `arenafinancesnapshots` (
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
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `arenafinancesnapshots`
--

LOCK TABLES `arenafinancesnapshots` WRITE;
/*!40000 ALTER TABLE `arenafinancesnapshots` DISABLE KEYS */;
/*!40000 ALTER TABLE `arenafinancesnapshots` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `arenamanagers`
--

DROP TABLE IF EXISTS `arenamanagers`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `arenamanagers` (
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
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `arenamanagers`
--

LOCK TABLES `arenamanagers` WRITE;
/*!40000 ALTER TABLE `arenamanagers` DISABLE KEYS */;
/*!40000 ALTER TABLE `arenamanagers` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `arenaratings`
--

DROP TABLE IF EXISTS `arenaratings`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `arenaratings` (
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
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `arenaratings`
--

LOCK TABLES `arenaratings` WRITE;
/*!40000 ALTER TABLE `arenaratings` DISABLE KEYS */;
/*!40000 ALTER TABLE `arenaratings` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `arenareservations`
--

DROP TABLE IF EXISTS `arenareservations`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `arenareservations` (
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
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `arenareservations`
--

LOCK TABLES `arenareservations` WRITE;
/*!40000 ALTER TABLE `arenareservations` DISABLE KEYS */;
/*!40000 ALTER TABLE `arenareservations` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `arenas`
--

DROP TABLE IF EXISTS `arenas`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `arenas` (
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
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `arenas`
--

LOCK TABLES `arenas` WRITE;
/*!40000 ALTER TABLE `arenas` DISABLE KEYS */;
/*!40000 ALTER TABLE `arenas` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `arenasignups`
--

DROP TABLE IF EXISTS `arenasignups`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `arenasignups` (
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
  PRIMARY KEY (`Id`),
  KEY `FK_ArenaSignups_ArenaEvents` (`ArenaEventId`),
  KEY `FK_ArenaSignups_Characters` (`CharacterId`),
  KEY `FK_ArenaSignups_CombatantClasses` (`CombatantClassId`),
  KEY `FK_ArenaSignups_Reservations` (`ArenaReservationId`),
  CONSTRAINT `FK_ArenaSignups_ArenaEvents` FOREIGN KEY (`ArenaEventId`) REFERENCES `arenaevents` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_ArenaSignups_Characters` FOREIGN KEY (`CharacterId`) REFERENCES `characters` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_ArenaSignups_CombatantClasses` FOREIGN KEY (`CombatantClassId`) REFERENCES `arenacombatantclasses` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_ArenaSignups_Reservations` FOREIGN KEY (`ArenaReservationId`) REFERENCES `arenareservations` (`Id`) ON DELETE SET NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `arenasignups`
--

LOCK TABLES `arenasignups` WRITE;
/*!40000 ALTER TABLE `arenasignups` DISABLE KEYS */;
/*!40000 ALTER TABLE `arenasignups` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `armourtypes`
--

DROP TABLE IF EXISTS `armourtypes`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `armourtypes` (
  `Id` bigint NOT NULL AUTO_INCREMENT,
  `Name` varchar(200) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL,
  `MinimumPenetrationDegree` int NOT NULL,
  `BaseDifficultyDegrees` double NOT NULL,
  `StackedDifficultyDegrees` double NOT NULL,
  `Definition` text CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `armourtypes`
--

LOCK TABLES `armourtypes` WRITE;
/*!40000 ALTER TABLE `armourtypes` DISABLE KEYS */;
/*!40000 ALTER TABLE `armourtypes` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `artificialintelligences`
--

DROP TABLE IF EXISTS `artificialintelligences`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `artificialintelligences` (
  `Id` bigint NOT NULL AUTO_INCREMENT,
  `Name` varchar(4000) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `Type` varchar(4000) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `Definition` mediumtext CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `artificialintelligences`
--

LOCK TABLES `artificialintelligences` WRITE;
/*!40000 ALTER TABLE `artificialintelligences` DISABLE KEYS */;
/*!40000 ALTER TABLE `artificialintelligences` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `auctionhouses`
--

DROP TABLE IF EXISTS `auctionhouses`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `auctionhouses` (
  `Id` bigint NOT NULL AUTO_INCREMENT,
  `Name` varchar(250) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `EconomicZoneId` bigint NOT NULL,
  `AuctionHouseCellId` bigint NOT NULL,
  `ProfitsBankAccountId` bigint NOT NULL,
  `AuctionListingFeeFlat` decimal(58,29) NOT NULL,
  `AuctionListingFeeRate` decimal(58,29) NOT NULL,
  `Definition` mediumtext CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `DefaultListingTime` double NOT NULL,
  PRIMARY KEY (`Id`),
  KEY `IX_AuctionHouses_AuctionHouseCellId` (`AuctionHouseCellId`),
  KEY `IX_AuctionHouses_EconomicZoneId` (`EconomicZoneId`),
  KEY `IX_AuctionHouses_ProfitsBankAccountId` (`ProfitsBankAccountId`),
  CONSTRAINT `FK_AuctionHouses_BankAccounts` FOREIGN KEY (`ProfitsBankAccountId`) REFERENCES `bankaccounts` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_AuctionHouses_Cells` FOREIGN KEY (`AuctionHouseCellId`) REFERENCES `cells` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_AuctionHouses_EconomicZones` FOREIGN KEY (`EconomicZoneId`) REFERENCES `economiczones` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `auctionhouses`
--

LOCK TABLES `auctionhouses` WRITE;
/*!40000 ALTER TABLE `auctionhouses` DISABLE KEYS */;
/*!40000 ALTER TABLE `auctionhouses` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `authoritygroups`
--

DROP TABLE IF EXISTS `authoritygroups`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `authoritygroups` (
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
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `authoritygroups`
--

LOCK TABLES `authoritygroups` WRITE;
/*!40000 ALTER TABLE `authoritygroups` DISABLE KEYS */;
/*!40000 ALTER TABLE `authoritygroups` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `autobuilderareatemplates`
--

DROP TABLE IF EXISTS `autobuilderareatemplates`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `autobuilderareatemplates` (
  `ID` bigint NOT NULL AUTO_INCREMENT,
  `Name` varchar(100) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `TemplateType` varchar(45) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `Definition` longtext CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  PRIMARY KEY (`ID`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `autobuilderareatemplates`
--

LOCK TABLES `autobuilderareatemplates` WRITE;
/*!40000 ALTER TABLE `autobuilderareatemplates` DISABLE KEYS */;
/*!40000 ALTER TABLE `autobuilderareatemplates` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `autobuilderroomtemplates`
--

DROP TABLE IF EXISTS `autobuilderroomtemplates`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `autobuilderroomtemplates` (
  `ID` bigint NOT NULL AUTO_INCREMENT,
  `Name` varchar(100) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `TemplateType` varchar(45) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `Definition` longtext CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  PRIMARY KEY (`ID`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `autobuilderroomtemplates`
--

LOCK TABLES `autobuilderroomtemplates` WRITE;
/*!40000 ALTER TABLE `autobuilderroomtemplates` DISABLE KEYS */;
/*!40000 ALTER TABLE `autobuilderroomtemplates` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `bankaccounts`
--

DROP TABLE IF EXISTS `bankaccounts`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `bankaccounts` (
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
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `bankaccounts`
--

LOCK TABLES `bankaccounts` WRITE;
/*!40000 ALTER TABLE `bankaccounts` DISABLE KEYS */;
/*!40000 ALTER TABLE `bankaccounts` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `bankaccounttransactions`
--

DROP TABLE IF EXISTS `bankaccounttransactions`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `bankaccounttransactions` (
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
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `bankaccounttransactions`
--

LOCK TABLES `bankaccounttransactions` WRITE;
/*!40000 ALTER TABLE `bankaccounttransactions` DISABLE KEYS */;
/*!40000 ALTER TABLE `bankaccounttransactions` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `bankaccounttypes`
--

DROP TABLE IF EXISTS `bankaccounttypes`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `bankaccounttypes` (
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
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `bankaccounttypes`
--

LOCK TABLES `bankaccounttypes` WRITE;
/*!40000 ALTER TABLE `bankaccounttypes` DISABLE KEYS */;
/*!40000 ALTER TABLE `bankaccounttypes` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `bankbranches`
--

DROP TABLE IF EXISTS `bankbranches`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `bankbranches` (
  `BankId` bigint NOT NULL,
  `CellId` bigint NOT NULL,
  PRIMARY KEY (`BankId`,`CellId`),
  KEY `IX_BankBranches_CellId` (`CellId`),
  CONSTRAINT `FK_BankBranches_Banks` FOREIGN KEY (`BankId`) REFERENCES `banks` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_BankBranches_Cells` FOREIGN KEY (`CellId`) REFERENCES `cells` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `bankbranches`
--

LOCK TABLES `bankbranches` WRITE;
/*!40000 ALTER TABLE `bankbranches` DISABLE KEYS */;
/*!40000 ALTER TABLE `bankbranches` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `bankcurrencyreserves`
--

DROP TABLE IF EXISTS `bankcurrencyreserves`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `bankcurrencyreserves` (
  `BankId` bigint NOT NULL,
  `CurrencyId` bigint NOT NULL,
  `Amount` decimal(58,29) NOT NULL,
  PRIMARY KEY (`BankId`,`CurrencyId`),
  KEY `IX_BankCurrencyReserves_CurrencyId` (`CurrencyId`),
  CONSTRAINT `FK_BankCurrencyReserves_Banks` FOREIGN KEY (`BankId`) REFERENCES `banks` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_BankCurrencyReserves_Currencies` FOREIGN KEY (`CurrencyId`) REFERENCES `currencies` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `bankcurrencyreserves`
--

LOCK TABLES `bankcurrencyreserves` WRITE;
/*!40000 ALTER TABLE `bankcurrencyreserves` DISABLE KEYS */;
/*!40000 ALTER TABLE `bankcurrencyreserves` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `bankexchangerates`
--

DROP TABLE IF EXISTS `bankexchangerates`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `bankexchangerates` (
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
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `bankexchangerates`
--

LOCK TABLES `bankexchangerates` WRITE;
/*!40000 ALTER TABLE `bankexchangerates` DISABLE KEYS */;
/*!40000 ALTER TABLE `bankexchangerates` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `bankmanagerauditlogs`
--

DROP TABLE IF EXISTS `bankmanagerauditlogs`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `bankmanagerauditlogs` (
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
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `bankmanagerauditlogs`
--

LOCK TABLES `bankmanagerauditlogs` WRITE;
/*!40000 ALTER TABLE `bankmanagerauditlogs` DISABLE KEYS */;
/*!40000 ALTER TABLE `bankmanagerauditlogs` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `bankmanagers`
--

DROP TABLE IF EXISTS `bankmanagers`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `bankmanagers` (
  `BankId` bigint NOT NULL,
  `CharacterId` bigint NOT NULL,
  PRIMARY KEY (`BankId`,`CharacterId`),
  KEY `IX_BankManagers_CharacterId` (`CharacterId`),
  CONSTRAINT `FK_BankManagers_Banks` FOREIGN KEY (`BankId`) REFERENCES `banks` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_BankManagers_Characters` FOREIGN KEY (`CharacterId`) REFERENCES `characters` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `bankmanagers`
--

LOCK TABLES `bankmanagers` WRITE;
/*!40000 ALTER TABLE `bankmanagers` DISABLE KEYS */;
/*!40000 ALTER TABLE `bankmanagers` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `banks`
--

DROP TABLE IF EXISTS `banks`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `banks` (
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
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `banks`
--

LOCK TABLES `banks` WRITE;
/*!40000 ALTER TABLE `banks` DISABLE KEYS */;
/*!40000 ALTER TABLE `banks` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `bans`
--

DROP TABLE IF EXISTS `bans`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `bans` (
  `Id` bigint NOT NULL AUTO_INCREMENT,
  `IpMask` varchar(200) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `BannerAccountId` bigint DEFAULT NULL,
  `Reason` varchar(4000) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `Expiry` datetime DEFAULT NULL,
  PRIMARY KEY (`Id`),
  KEY `FK_Bans_Accounts` (`BannerAccountId`),
  CONSTRAINT `FK_Bans_Accounts` FOREIGN KEY (`BannerAccountId`) REFERENCES `accounts` (`Id`) ON DELETE SET NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `bans`
--

LOCK TABLES `bans` WRITE;
/*!40000 ALTER TABLE `bans` DISABLE KEYS */;
/*!40000 ALTER TABLE `bans` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `bloodmodels`
--

DROP TABLE IF EXISTS `bloodmodels`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `bloodmodels` (
  `Id` bigint NOT NULL AUTO_INCREMENT,
  `Name` varchar(200) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `bloodmodels`
--

LOCK TABLES `bloodmodels` WRITE;
/*!40000 ALTER TABLE `bloodmodels` DISABLE KEYS */;
/*!40000 ALTER TABLE `bloodmodels` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `bloodmodels_bloodtypes`
--

DROP TABLE IF EXISTS `bloodmodels_bloodtypes`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `bloodmodels_bloodtypes` (
  `BloodModelId` bigint NOT NULL,
  `BloodtypeId` bigint NOT NULL,
  PRIMARY KEY (`BloodModelId`,`BloodtypeId`),
  KEY `FK_BloodModels_Bloodtypes_Bloodtypes_idx` (`BloodtypeId`),
  CONSTRAINT `FK_BloodModels_Bloodtypes_BloodModels` FOREIGN KEY (`BloodModelId`) REFERENCES `bloodmodels` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_BloodModels_Bloodtypes_Bloodtypes` FOREIGN KEY (`BloodtypeId`) REFERENCES `bloodtypes` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `bloodmodels_bloodtypes`
--

LOCK TABLES `bloodmodels_bloodtypes` WRITE;
/*!40000 ALTER TABLE `bloodmodels_bloodtypes` DISABLE KEYS */;
/*!40000 ALTER TABLE `bloodmodels_bloodtypes` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `bloodtypeantigens`
--

DROP TABLE IF EXISTS `bloodtypeantigens`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `bloodtypeantigens` (
  `Id` bigint NOT NULL AUTO_INCREMENT,
  `Name` varchar(200) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `bloodtypeantigens`
--

LOCK TABLES `bloodtypeantigens` WRITE;
/*!40000 ALTER TABLE `bloodtypeantigens` DISABLE KEYS */;
/*!40000 ALTER TABLE `bloodtypeantigens` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `bloodtypes`
--

DROP TABLE IF EXISTS `bloodtypes`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `bloodtypes` (
  `Id` bigint NOT NULL AUTO_INCREMENT,
  `Name` varchar(200) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `bloodtypes`
--

LOCK TABLES `bloodtypes` WRITE;
/*!40000 ALTER TABLE `bloodtypes` DISABLE KEYS */;
/*!40000 ALTER TABLE `bloodtypes` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `bloodtypes_bloodtypeantigens`
--

DROP TABLE IF EXISTS `bloodtypes_bloodtypeantigens`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `bloodtypes_bloodtypeantigens` (
  `BloodtypeId` bigint NOT NULL,
  `BloodtypeAntigenId` bigint NOT NULL,
  PRIMARY KEY (`BloodtypeId`,`BloodtypeAntigenId`),
  KEY `FK_Bloodtypes_BloodtypeAntigens_BloodtypeAntigens_idx` (`BloodtypeAntigenId`),
  CONSTRAINT `FK_Bloodtypes_BloodtypeAntigens_BloodtypeAntigens` FOREIGN KEY (`BloodtypeAntigenId`) REFERENCES `bloodtypeantigens` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_Bloodtypes_BloodtypeAntigens_Bloodtypes` FOREIGN KEY (`BloodtypeId`) REFERENCES `bloodtypes` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `bloodtypes_bloodtypeantigens`
--

LOCK TABLES `bloodtypes_bloodtypeantigens` WRITE;
/*!40000 ALTER TABLE `bloodtypes_bloodtypeantigens` DISABLE KEYS */;
/*!40000 ALTER TABLE `bloodtypes_bloodtypeantigens` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `boardposts`
--

DROP TABLE IF EXISTS `boardposts`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `boardposts` (
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
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `boardposts`
--

LOCK TABLES `boardposts` WRITE;
/*!40000 ALTER TABLE `boardposts` DISABLE KEYS */;
/*!40000 ALTER TABLE `boardposts` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `boards`
--

DROP TABLE IF EXISTS `boards`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `boards` (
  `Id` bigint NOT NULL AUTO_INCREMENT,
  `Name` varchar(45) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `ShowOnLogin` bit(1) NOT NULL,
  `CalendarId` bigint DEFAULT NULL,
  PRIMARY KEY (`Id`),
  KEY `IX_Boards_CalendarId` (`CalendarId`),
  CONSTRAINT `FK_Boards_Calendars` FOREIGN KEY (`CalendarId`) REFERENCES `calendars` (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `boards`
--

LOCK TABLES `boards` WRITE;
/*!40000 ALTER TABLE `boards` DISABLE KEYS */;
/*!40000 ALTER TABLE `boards` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `bodies`
--

DROP TABLE IF EXISTS `bodies`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `bodies` (
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
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `bodies`
--

LOCK TABLES `bodies` WRITE;
/*!40000 ALTER TABLE `bodies` DISABLE KEYS */;
/*!40000 ALTER TABLE `bodies` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `bodies_drugdoses`
--

DROP TABLE IF EXISTS `bodies_drugdoses`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `bodies_drugdoses` (
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
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `bodies_drugdoses`
--

LOCK TABLES `bodies_drugdoses` WRITE;
/*!40000 ALTER TABLE `bodies_drugdoses` DISABLE KEYS */;
/*!40000 ALTER TABLE `bodies_drugdoses` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `bodies_gameitems`
--

DROP TABLE IF EXISTS `bodies_gameitems`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `bodies_gameitems` (
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
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `bodies_gameitems`
--

LOCK TABLES `bodies_gameitems` WRITE;
/*!40000 ALTER TABLE `bodies_gameitems` DISABLE KEYS */;
/*!40000 ALTER TABLE `bodies_gameitems` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `bodies_implants`
--

DROP TABLE IF EXISTS `bodies_implants`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `bodies_implants` (
  `BodyId` bigint NOT NULL,
  `ImplantId` bigint NOT NULL,
  PRIMARY KEY (`BodyId`,`ImplantId`),
  KEY `FK_Bodies_Implants_GameItems_idx` (`ImplantId`),
  CONSTRAINT `FK_Bodies_Implants_Bodies` FOREIGN KEY (`BodyId`) REFERENCES `bodies` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_Bodies_Implants_GameItems` FOREIGN KEY (`ImplantId`) REFERENCES `gameitems` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `bodies_implants`
--

LOCK TABLES `bodies_implants` WRITE;
/*!40000 ALTER TABLE `bodies_implants` DISABLE KEYS */;
/*!40000 ALTER TABLE `bodies_implants` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `bodies_prosthetics`
--

DROP TABLE IF EXISTS `bodies_prosthetics`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `bodies_prosthetics` (
  `BodyId` bigint NOT NULL,
  `ProstheticId` bigint NOT NULL,
  PRIMARY KEY (`BodyId`,`ProstheticId`),
  KEY `FK_Bodies_Prosthetics_GameItems_idx` (`ProstheticId`),
  CONSTRAINT `FK_Bodies_Prosthetics_Bodies` FOREIGN KEY (`BodyId`) REFERENCES `bodies` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_Bodies_Prosthetics_GameItems` FOREIGN KEY (`ProstheticId`) REFERENCES `gameitems` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `bodies_prosthetics`
--

LOCK TABLES `bodies_prosthetics` WRITE;
/*!40000 ALTER TABLE `bodies_prosthetics` DISABLE KEYS */;
/*!40000 ALTER TABLE `bodies_prosthetics` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `bodies_severedparts`
--

DROP TABLE IF EXISTS `bodies_severedparts`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `bodies_severedparts` (
  `BodiesId` bigint NOT NULL,
  `BodypartProtoId` bigint NOT NULL,
  PRIMARY KEY (`BodiesId`,`BodypartProtoId`),
  KEY `FK_Bodies_SeveredParts_BodypartProtos_idx` (`BodypartProtoId`),
  CONSTRAINT `FK_Bodies_SeveredParts_Bodies` FOREIGN KEY (`BodiesId`) REFERENCES `bodies` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_Bodies_SeveredParts_BodypartProtos` FOREIGN KEY (`BodypartProtoId`) REFERENCES `bodypartproto` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `bodies_severedparts`
--

LOCK TABLES `bodies_severedparts` WRITE;
/*!40000 ALTER TABLE `bodies_severedparts` DISABLE KEYS */;
/*!40000 ALTER TABLE `bodies_severedparts` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `bodypartgroupdescribers`
--

DROP TABLE IF EXISTS `bodypartgroupdescribers`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `bodypartgroupdescribers` (
  `Id` bigint NOT NULL AUTO_INCREMENT,
  `DescribedAs` varchar(4000) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `Comment` varchar(4000) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci DEFAULT NULL,
  `Type` varchar(50) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `BodyProtoId` bigint DEFAULT NULL,
  PRIMARY KEY (`Id`),
  KEY `IX_BodypartGroupDescribers_BodyProtoId` (`BodyProtoId`),
  CONSTRAINT `FK_BodypartGroupDescribers_BodyProtos_BodyProtoId` FOREIGN KEY (`BodyProtoId`) REFERENCES `bodyprotos` (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `bodypartgroupdescribers`
--

LOCK TABLES `bodypartgroupdescribers` WRITE;
/*!40000 ALTER TABLE `bodypartgroupdescribers` DISABLE KEYS */;
/*!40000 ALTER TABLE `bodypartgroupdescribers` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `bodypartgroupdescribers_bodypartprotos`
--

DROP TABLE IF EXISTS `bodypartgroupdescribers_bodypartprotos`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `bodypartgroupdescribers_bodypartprotos` (
  `BodypartGroupDescriberId` bigint NOT NULL,
  `BodypartProtoId` bigint NOT NULL,
  `Mandatory` bit(1) NOT NULL DEFAULT b'0',
  PRIMARY KEY (`BodypartGroupDescriberId`,`BodypartProtoId`),
  KEY `FK_BGD_BodypartProtos_BodypartProto` (`BodypartProtoId`),
  CONSTRAINT `FK_BGD_BodypartProtos_BodypartGroupDescribers` FOREIGN KEY (`BodypartGroupDescriberId`) REFERENCES `bodypartgroupdescribers` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_BGD_BodypartProtos_BodypartProto` FOREIGN KEY (`BodypartProtoId`) REFERENCES `bodypartproto` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `bodypartgroupdescribers_bodypartprotos`
--

LOCK TABLES `bodypartgroupdescribers_bodypartprotos` WRITE;
/*!40000 ALTER TABLE `bodypartgroupdescribers_bodypartprotos` DISABLE KEYS */;
/*!40000 ALTER TABLE `bodypartgroupdescribers_bodypartprotos` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `bodypartgroupdescribers_bodyprotos`
--

DROP TABLE IF EXISTS `bodypartgroupdescribers_bodyprotos`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `bodypartgroupdescribers_bodyprotos` (
  `BodypartGroupDescriberId` bigint NOT NULL,
  `BodyProtoId` bigint NOT NULL,
  PRIMARY KEY (`BodypartGroupDescriberId`,`BodyProtoId`),
  KEY `FK_BGD_BodyProtos_BodyProtos` (`BodyProtoId`),
  CONSTRAINT `FK_BGD_BodyProtos_BodypartGroupDescribers` FOREIGN KEY (`BodypartGroupDescriberId`) REFERENCES `bodypartgroupdescribers` (`Id`) ON DELETE RESTRICT,
  CONSTRAINT `FK_BGD_BodyProtos_BodyProtos` FOREIGN KEY (`BodyProtoId`) REFERENCES `bodyprotos` (`Id`) ON DELETE RESTRICT
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `bodypartgroupdescribers_bodyprotos`
--

LOCK TABLES `bodypartgroupdescribers_bodyprotos` WRITE;
/*!40000 ALTER TABLE `bodypartgroupdescribers_bodyprotos` DISABLE KEYS */;
/*!40000 ALTER TABLE `bodypartgroupdescribers_bodyprotos` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `bodypartgroupdescribers_shapecount`
--

DROP TABLE IF EXISTS `bodypartgroupdescribers_shapecount`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `bodypartgroupdescribers_shapecount` (
  `BodypartGroupDescriptionRuleId` bigint NOT NULL,
  `TargetId` bigint NOT NULL,
  `MinCount` int NOT NULL,
  `MaxCount` int NOT NULL,
  PRIMARY KEY (`BodypartGroupDescriptionRuleId`,`TargetId`),
  KEY `FK_BGD_ShapeCount_BodypartShape` (`TargetId`),
  CONSTRAINT `FK_BGD_ShapeCount_BodypartGroupDescribers` FOREIGN KEY (`BodypartGroupDescriptionRuleId`) REFERENCES `bodypartgroupdescribers` (`Id`) ON DELETE RESTRICT,
  CONSTRAINT `FK_BGD_ShapeCount_BodypartShape` FOREIGN KEY (`TargetId`) REFERENCES `bodypartshape` (`Id`) ON DELETE RESTRICT
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `bodypartgroupdescribers_shapecount`
--

LOCK TABLES `bodypartgroupdescribers_shapecount` WRITE;
/*!40000 ALTER TABLE `bodypartgroupdescribers_shapecount` DISABLE KEYS */;
/*!40000 ALTER TABLE `bodypartgroupdescribers_shapecount` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `bodypartinternalinfos`
--

DROP TABLE IF EXISTS `bodypartinternalinfos`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `bodypartinternalinfos` (
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
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `bodypartinternalinfos`
--

LOCK TABLES `bodypartinternalinfos` WRITE;
/*!40000 ALTER TABLE `bodypartinternalinfos` DISABLE KEYS */;
/*!40000 ALTER TABLE `bodypartinternalinfos` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `bodypartproto`
--

DROP TABLE IF EXISTS `bodypartproto`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `bodypartproto` (
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
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `bodypartproto`
--

LOCK TABLES `bodypartproto` WRITE;
/*!40000 ALTER TABLE `bodypartproto` DISABLE KEYS */;
/*!40000 ALTER TABLE `bodypartproto` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `bodypartproto_alignmenthits`
--

DROP TABLE IF EXISTS `bodypartproto_alignmenthits`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `bodypartproto_alignmenthits` (
  `Id` bigint NOT NULL AUTO_INCREMENT,
  `BodypartProtoId` bigint NOT NULL,
  `Alignment` int NOT NULL,
  `HitChance` int DEFAULT NULL,
  PRIMARY KEY (`Id`),
  KEY `FK_BodypartProto_AlignmentHits_BodypartProto` (`BodypartProtoId`),
  CONSTRAINT `FK_BodypartProto_AlignmentHits_BodypartProto` FOREIGN KEY (`BodypartProtoId`) REFERENCES `bodypartproto` (`Id`) ON DELETE RESTRICT
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `bodypartproto_alignmenthits`
--

LOCK TABLES `bodypartproto_alignmenthits` WRITE;
/*!40000 ALTER TABLE `bodypartproto_alignmenthits` DISABLE KEYS */;
/*!40000 ALTER TABLE `bodypartproto_alignmenthits` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `bodypartproto_bodypartproto_upstream`
--

DROP TABLE IF EXISTS `bodypartproto_bodypartproto_upstream`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `bodypartproto_bodypartproto_upstream` (
  `Child` bigint NOT NULL,
  `Parent` bigint NOT NULL,
  PRIMARY KEY (`Child`,`Parent`),
  KEY `FKParent` (`Parent`),
  CONSTRAINT `FKChild` FOREIGN KEY (`Child`) REFERENCES `bodypartproto` (`Id`) ON DELETE RESTRICT,
  CONSTRAINT `FKParent` FOREIGN KEY (`Parent`) REFERENCES `bodypartproto` (`Id`) ON DELETE RESTRICT
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `bodypartproto_bodypartproto_upstream`
--

LOCK TABLES `bodypartproto_bodypartproto_upstream` WRITE;
/*!40000 ALTER TABLE `bodypartproto_bodypartproto_upstream` DISABLE KEYS */;
/*!40000 ALTER TABLE `bodypartproto_bodypartproto_upstream` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `bodypartproto_orientationhits`
--

DROP TABLE IF EXISTS `bodypartproto_orientationhits`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `bodypartproto_orientationhits` (
  `Id` bigint NOT NULL AUTO_INCREMENT,
  `BodypartProtoId` bigint NOT NULL,
  `Orientation` int NOT NULL,
  `HitChance` int DEFAULT NULL,
  PRIMARY KEY (`Id`),
  KEY `FK_BodypartProto_OrientationHits_BodypartProto` (`BodypartProtoId`),
  CONSTRAINT `FK_BodypartProto_OrientationHits_BodypartProto` FOREIGN KEY (`BodypartProtoId`) REFERENCES `bodypartproto` (`Id`) ON DELETE RESTRICT
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `bodypartproto_orientationhits`
--

LOCK TABLES `bodypartproto_orientationhits` WRITE;
/*!40000 ALTER TABLE `bodypartproto_orientationhits` DISABLE KEYS */;
/*!40000 ALTER TABLE `bodypartproto_orientationhits` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `bodypartshape`
--

DROP TABLE IF EXISTS `bodypartshape`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `bodypartshape` (
  `Id` bigint NOT NULL AUTO_INCREMENT,
  `Name` varchar(4000) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `bodypartshape`
--

LOCK TABLES `bodypartshape` WRITE;
/*!40000 ALTER TABLE `bodypartshape` DISABLE KEYS */;
/*!40000 ALTER TABLE `bodypartshape` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `bodypartshapecountview`
--

DROP TABLE IF EXISTS `bodypartshapecountview`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `bodypartshapecountview` (
  `BodypartGroupDescriptionRuleId` tinyint NOT NULL,
  `DescribedAs` tinyint NOT NULL,
  `MinCount` tinyint NOT NULL,
  `MaxCount` tinyint NOT NULL,
  `TargetId` tinyint NOT NULL,
  `Name` tinyint NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `bodypartshapecountview`
--

LOCK TABLES `bodypartshapecountview` WRITE;
/*!40000 ALTER TABLE `bodypartshapecountview` DISABLE KEYS */;
/*!40000 ALTER TABLE `bodypartshapecountview` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `bodyprotos`
--

DROP TABLE IF EXISTS `bodyprotos`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `bodyprotos` (
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
  PRIMARY KEY (`Id`),
  KEY `FK_BodyPrototype_BodyPrototype_idx` (`CountsAsId`),
  KEY `FK_BodyPrototype_Bodyparts_idx` (`DefaultSmashingBodypartId`),
  KEY `FK_BodyPrototype_WearableSizeParameterRule` (`WearSizeParameterId`),
  CONSTRAINT `FK_BodyPrototype_Bodyparts` FOREIGN KEY (`DefaultSmashingBodypartId`) REFERENCES `bodypartproto` (`Id`) ON DELETE SET NULL,
  CONSTRAINT `FK_BodyPrototype_BodyPrototype` FOREIGN KEY (`CountsAsId`) REFERENCES `bodyprotos` (`Id`) ON DELETE SET NULL,
  CONSTRAINT `FK_BodyPrototype_WearableSizeParameterRule` FOREIGN KEY (`WearSizeParameterId`) REFERENCES `wearablesizeparameterrule` (`Id`) ON DELETE RESTRICT
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `bodyprotos`
--

LOCK TABLES `bodyprotos` WRITE;
/*!40000 ALTER TABLE `bodyprotos` DISABLE KEYS */;
/*!40000 ALTER TABLE `bodyprotos` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `bodyprotos_additionalbodyparts`
--

DROP TABLE IF EXISTS `bodyprotos_additionalbodyparts`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `bodyprotos_additionalbodyparts` (
  `BodyProtoId` bigint NOT NULL,
  `BodypartId` bigint NOT NULL,
  `Usage` varchar(50) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  PRIMARY KEY (`BodyProtoId`,`BodypartId`,`Usage`),
  KEY `FK_BodyProtos_AdditionalBodyparts_BodypartProto_idx` (`BodypartId`),
  KEY `FK_BodyProtos_AdditionalBodyparts_BodyProtos_idx` (`BodyProtoId`),
  CONSTRAINT `FK_BodyProtos_AdditionalBodyparts_BodypartProto` FOREIGN KEY (`BodypartId`) REFERENCES `bodypartproto` (`Id`) ON DELETE RESTRICT,
  CONSTRAINT `FK_BodyProtos_AdditionalBodyparts_BodyProtos` FOREIGN KEY (`BodyProtoId`) REFERENCES `bodyprotos` (`Id`) ON DELETE RESTRICT
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `bodyprotos_additionalbodyparts`
--

LOCK TABLES `bodyprotos_additionalbodyparts` WRITE;
/*!40000 ALTER TABLE `bodyprotos_additionalbodyparts` DISABLE KEYS */;
/*!40000 ALTER TABLE `bodyprotos_additionalbodyparts` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `bodyprotospositions`
--

DROP TABLE IF EXISTS `bodyprotospositions`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `bodyprotospositions` (
  `BodyProtoId` bigint NOT NULL,
  `Position` int NOT NULL,
  PRIMARY KEY (`BodyProtoId`,`Position`),
  KEY `FK_BodyProtosPositions_BodyProtos_idx` (`BodyProtoId`),
  CONSTRAINT `FK_BodyProtosPositions_BodyProtos` FOREIGN KEY (`BodyProtoId`) REFERENCES `bodyprotos` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `bodyprotospositions`
--

LOCK TABLES `bodyprotospositions` WRITE;
/*!40000 ALTER TABLE `bodyprotospositions` DISABLE KEYS */;
/*!40000 ALTER TABLE `bodyprotospositions` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `boneorgancoverages`
--

DROP TABLE IF EXISTS `boneorgancoverages`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `boneorgancoverages` (
  `BoneId` bigint NOT NULL,
  `OrganId` bigint NOT NULL,
  `CoverageChance` double NOT NULL,
  PRIMARY KEY (`BoneId`,`OrganId`),
  KEY `FK_BoneOrganCoverages_BodypartProto_Organ_idx` (`OrganId`),
  CONSTRAINT `FK_BoneOrganCoverages_BodypartProto_Bone` FOREIGN KEY (`BoneId`) REFERENCES `bodypartproto` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_BoneOrganCoverages_BodypartProto_Organ` FOREIGN KEY (`OrganId`) REFERENCES `bodypartproto` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `boneorgancoverages`
--

LOCK TABLES `boneorgancoverages` WRITE;
/*!40000 ALTER TABLE `boneorgancoverages` DISABLE KEYS */;
/*!40000 ALTER TABLE `boneorgancoverages` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `butcheryproductitems`
--

DROP TABLE IF EXISTS `butcheryproductitems`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `butcheryproductitems` (
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
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `butcheryproductitems`
--

LOCK TABLES `butcheryproductitems` WRITE;
/*!40000 ALTER TABLE `butcheryproductitems` DISABLE KEYS */;
/*!40000 ALTER TABLE `butcheryproductitems` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `butcheryproducts`
--

DROP TABLE IF EXISTS `butcheryproducts`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `butcheryproducts` (
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
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `butcheryproducts`
--

LOCK TABLES `butcheryproducts` WRITE;
/*!40000 ALTER TABLE `butcheryproducts` DISABLE KEYS */;
/*!40000 ALTER TABLE `butcheryproducts` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `butcheryproducts_bodypartprotos`
--

DROP TABLE IF EXISTS `butcheryproducts_bodypartprotos`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `butcheryproducts_bodypartprotos` (
  `ButcheryProductId` bigint NOT NULL,
  `BodypartProtoId` bigint NOT NULL,
  PRIMARY KEY (`ButcheryProductId`,`BodypartProtoId`),
  KEY `FK_ButcheryProducts_BodypartProtos_BodypartProtos_idx` (`BodypartProtoId`),
  CONSTRAINT `FK_ButcheryProducts_BodypartProtos_BodypartProtos` FOREIGN KEY (`BodypartProtoId`) REFERENCES `bodypartproto` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_ButcheryProducts_BodypartProtos_ButcheryProducts` FOREIGN KEY (`ButcheryProductId`) REFERENCES `butcheryproducts` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `butcheryproducts_bodypartprotos`
--

LOCK TABLES `butcheryproducts_bodypartprotos` WRITE;
/*!40000 ALTER TABLE `butcheryproducts_bodypartprotos` DISABLE KEYS */;
/*!40000 ALTER TABLE `butcheryproducts_bodypartprotos` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `calendars`
--

DROP TABLE IF EXISTS `calendars`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `calendars` (
  `Id` bigint NOT NULL AUTO_INCREMENT,
  `Definition` text CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `Date` varchar(50) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `FeedClockId` bigint NOT NULL,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `calendars`
--

LOCK TABLES `calendars` WRITE;
/*!40000 ALTER TABLE `calendars` DISABLE KEYS */;
/*!40000 ALTER TABLE `calendars` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `celestials`
--

DROP TABLE IF EXISTS `celestials`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `celestials` (
  `Id` bigint NOT NULL AUTO_INCREMENT,
  `Definition` text CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `Minutes` int NOT NULL,
  `FeedClockId` bigint NOT NULL,
  `CelestialYear` int NOT NULL,
  `LastYearBump` int NOT NULL,
  `CelestialType` varchar(30) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL DEFAULT 'Sun',
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `celestials`
--

LOCK TABLES `celestials` WRITE;
/*!40000 ALTER TABLE `celestials` DISABLE KEYS */;
/*!40000 ALTER TABLE `celestials` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `celloverlaypackages`
--

DROP TABLE IF EXISTS `celloverlaypackages`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `celloverlaypackages` (
  `Id` bigint NOT NULL,
  `RevisionNumber` int NOT NULL,
  `Name` varchar(4000) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `EditableItemId` bigint NOT NULL,
  PRIMARY KEY (`Id`,`RevisionNumber`),
  KEY `FK_CellOverlayPackages_EditableItems` (`EditableItemId`),
  CONSTRAINT `FK_CellOverlayPackages_EditableItems` FOREIGN KEY (`EditableItemId`) REFERENCES `editableitems` (`Id`) ON DELETE RESTRICT
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `celloverlaypackages`
--

LOCK TABLES `celloverlaypackages` WRITE;
/*!40000 ALTER TABLE `celloverlaypackages` DISABLE KEYS */;
/*!40000 ALTER TABLE `celloverlaypackages` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `celloverlays`
--

DROP TABLE IF EXISTS `celloverlays`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `celloverlays` (
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
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `celloverlays`
--

LOCK TABLES `celloverlays` WRITE;
/*!40000 ALTER TABLE `celloverlays` DISABLE KEYS */;
/*!40000 ALTER TABLE `celloverlays` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `celloverlays_exits`
--

DROP TABLE IF EXISTS `celloverlays_exits`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `celloverlays_exits` (
  `CellOverlayId` bigint NOT NULL,
  `ExitId` bigint NOT NULL,
  PRIMARY KEY (`CellOverlayId`,`ExitId`),
  KEY `FK_CellOverlays_Exits_Exits` (`ExitId`),
  CONSTRAINT `FK_CellOverlays_Exits_CellOverlays` FOREIGN KEY (`CellOverlayId`) REFERENCES `celloverlays` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_CellOverlays_Exits_Exits` FOREIGN KEY (`ExitId`) REFERENCES `exits` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `celloverlays_exits`
--

LOCK TABLES `celloverlays_exits` WRITE;
/*!40000 ALTER TABLE `celloverlays_exits` DISABLE KEYS */;
/*!40000 ALTER TABLE `celloverlays_exits` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `cells`
--

DROP TABLE IF EXISTS `cells`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `cells` (
  `Id` bigint NOT NULL AUTO_INCREMENT,
  `RoomId` bigint NOT NULL,
  `CurrentOverlayId` bigint DEFAULT NULL,
  `ForagableProfileId` bigint DEFAULT NULL,
  `Temporary` bit(1) NOT NULL DEFAULT b'0',
  `EffectData` mediumtext CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  PRIMARY KEY (`Id`),
  KEY `FK_Cells_CellOverlays` (`CurrentOverlayId`),
  KEY `FK_Cells_Rooms` (`RoomId`),
  CONSTRAINT `FK_Cells_CellOverlays` FOREIGN KEY (`CurrentOverlayId`) REFERENCES `celloverlays` (`Id`) ON DELETE SET NULL,
  CONSTRAINT `FK_Cells_Rooms` FOREIGN KEY (`RoomId`) REFERENCES `rooms` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `cells`
--

LOCK TABLES `cells` WRITE;
/*!40000 ALTER TABLE `cells` DISABLE KEYS */;
/*!40000 ALTER TABLE `cells` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `cells_foragableyields`
--

DROP TABLE IF EXISTS `cells_foragableyields`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `cells_foragableyields` (
  `CellId` bigint NOT NULL,
  `ForagableType` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL,
  `Yield` double NOT NULL,
  PRIMARY KEY (`CellId`,`ForagableType`),
  CONSTRAINT `FK_Cells_ForagableYields_Cells` FOREIGN KEY (`CellId`) REFERENCES `cells` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `cells_foragableyields`
--

LOCK TABLES `cells_foragableyields` WRITE;
/*!40000 ALTER TABLE `cells_foragableyields` DISABLE KEYS */;
/*!40000 ALTER TABLE `cells_foragableyields` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `cells_gameitems`
--

DROP TABLE IF EXISTS `cells_gameitems`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `cells_gameitems` (
  `CellId` bigint NOT NULL,
  `GameItemId` bigint NOT NULL,
  PRIMARY KEY (`CellId`,`GameItemId`),
  KEY `FK_Cells_GameItems_GameItems` (`GameItemId`),
  CONSTRAINT `FK_Cells_GameItems_Cells` FOREIGN KEY (`CellId`) REFERENCES `cells` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_Cells_GameItems_GameItems` FOREIGN KEY (`GameItemId`) REFERENCES `gameitems` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `cells_gameitems`
--

LOCK TABLES `cells_gameitems` WRITE;
/*!40000 ALTER TABLE `cells_gameitems` DISABLE KEYS */;
/*!40000 ALTER TABLE `cells_gameitems` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `cells_magicresources`
--

DROP TABLE IF EXISTS `cells_magicresources`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `cells_magicresources` (
  `CellId` bigint NOT NULL,
  `MagicResourceId` bigint NOT NULL,
  `Amount` double NOT NULL,
  PRIMARY KEY (`CellId`,`MagicResourceId`),
  KEY `FK_Cells_MagicResources_MagicResources_idx` (`MagicResourceId`),
  CONSTRAINT `FK_Cells_MagicResources_Cells` FOREIGN KEY (`CellId`) REFERENCES `cells` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_Cells_MagicResources_MagicResources` FOREIGN KEY (`MagicResourceId`) REFERENCES `magicresources` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `cells_magicresources`
--

LOCK TABLES `cells_magicresources` WRITE;
/*!40000 ALTER TABLE `cells_magicresources` DISABLE KEYS */;
/*!40000 ALTER TABLE `cells_magicresources` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `cells_rangedcovers`
--

DROP TABLE IF EXISTS `cells_rangedcovers`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `cells_rangedcovers` (
  `CellId` bigint NOT NULL,
  `RangedCoverId` bigint NOT NULL,
  PRIMARY KEY (`CellId`,`RangedCoverId`),
  KEY `FK_Cells_RangedCovers_RangedCovers_idx` (`RangedCoverId`),
  CONSTRAINT `FK_Cells_RangedCovers_Cells` FOREIGN KEY (`CellId`) REFERENCES `cells` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_Cells_RangedCovers_RangedCovers` FOREIGN KEY (`RangedCoverId`) REFERENCES `rangedcovers` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `cells_rangedcovers`
--

LOCK TABLES `cells_rangedcovers` WRITE;
/*!40000 ALTER TABLE `cells_rangedcovers` DISABLE KEYS */;
/*!40000 ALTER TABLE `cells_rangedcovers` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `cells_tags`
--

DROP TABLE IF EXISTS `cells_tags`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `cells_tags` (
  `CellId` bigint NOT NULL,
  `TagId` bigint NOT NULL,
  PRIMARY KEY (`CellId`,`TagId`),
  KEY `FK_Cells_Tags_Tags_idx` (`TagId`),
  CONSTRAINT `FK_Cells_Tags_Cells` FOREIGN KEY (`CellId`) REFERENCES `cells` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_Cells_Tags_Tags` FOREIGN KEY (`TagId`) REFERENCES `tags` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `cells_tags`
--

LOCK TABLES `cells_tags` WRITE;
/*!40000 ALTER TABLE `cells_tags` DISABLE KEYS */;
/*!40000 ALTER TABLE `cells_tags` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `channelcommandwords`
--

DROP TABLE IF EXISTS `channelcommandwords`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `channelcommandwords` (
  `Word` varchar(50) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `ChannelId` bigint NOT NULL,
  PRIMARY KEY (`Word`),
  KEY `FK_ChannelCommandWords_Channels` (`ChannelId`),
  CONSTRAINT `FK_ChannelCommandWords_Channels` FOREIGN KEY (`ChannelId`) REFERENCES `channels` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `channelcommandwords`
--

LOCK TABLES `channelcommandwords` WRITE;
/*!40000 ALTER TABLE `channelcommandwords` DISABLE KEYS */;
/*!40000 ALTER TABLE `channelcommandwords` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `channelignorers`
--

DROP TABLE IF EXISTS `channelignorers`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `channelignorers` (
  `ChannelId` bigint NOT NULL,
  `AccountId` bigint NOT NULL,
  PRIMARY KEY (`ChannelId`,`AccountId`),
  KEY `FK_ChannelIgnorers_Accounts` (`AccountId`),
  CONSTRAINT `FK_ChannelIgnorers_Accounts` FOREIGN KEY (`AccountId`) REFERENCES `accounts` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_ChannelIgnorers_Channels` FOREIGN KEY (`ChannelId`) REFERENCES `channels` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `channelignorers`
--

LOCK TABLES `channelignorers` WRITE;
/*!40000 ALTER TABLE `channelignorers` DISABLE KEYS */;
/*!40000 ALTER TABLE `channelignorers` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `channels`
--

DROP TABLE IF EXISTS `channels`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `channels` (
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
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `channels`
--

LOCK TABLES `channels` WRITE;
/*!40000 ALTER TABLE `channels` DISABLE KEYS */;
/*!40000 ALTER TABLE `channels` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `charactercombatsettings`
--

DROP TABLE IF EXISTS `charactercombatsettings`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `charactercombatsettings` (
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
  PRIMARY KEY (`Id`),
  KEY `FK_CharacterCombatSettings_FutureProgs_idx` (`AvailabilityProgId`),
  KEY `FK_CharacterCombatSettings_Characters_idx` (`CharacterOwnerId`),
  KEY `IX_CharacterCombatSettings_PriorityProgId` (`PriorityProgId`),
  CONSTRAINT `FK_CharacterCombatSettings_Characters` FOREIGN KEY (`CharacterOwnerId`) REFERENCES `characters` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_CharacterCombatSettings_FutureProgs` FOREIGN KEY (`AvailabilityProgId`) REFERENCES `futureprogs` (`Id`) ON DELETE SET NULL,
  CONSTRAINT `FK_CharacterCombatSettings_PriorityProg` FOREIGN KEY (`PriorityProgId`) REFERENCES `futureprogs` (`Id`) ON DELETE SET NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `charactercombatsettings`
--

LOCK TABLES `charactercombatsettings` WRITE;
/*!40000 ALTER TABLE `charactercombatsettings` DISABLE KEYS */;
/*!40000 ALTER TABLE `charactercombatsettings` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `characterintrotemplates`
--

DROP TABLE IF EXISTS `characterintrotemplates`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `characterintrotemplates` (
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
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `characterintrotemplates`
--

LOCK TABLES `characterintrotemplates` WRITE;
/*!40000 ALTER TABLE `characterintrotemplates` DISABLE KEYS */;
/*!40000 ALTER TABLE `characterintrotemplates` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `characteristicdefinitions`
--

DROP TABLE IF EXISTS `characteristicdefinitions`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `characteristicdefinitions` (
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
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `characteristicdefinitions`
--

LOCK TABLES `characteristicdefinitions` WRITE;
/*!40000 ALTER TABLE `characteristicdefinitions` DISABLE KEYS */;
/*!40000 ALTER TABLE `characteristicdefinitions` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `characteristicprofiles`
--

DROP TABLE IF EXISTS `characteristicprofiles`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `characteristicprofiles` (
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
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `characteristicprofiles`
--

LOCK TABLES `characteristicprofiles` WRITE;
/*!40000 ALTER TABLE `characteristicprofiles` DISABLE KEYS */;
/*!40000 ALTER TABLE `characteristicprofiles` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `characteristics`
--

DROP TABLE IF EXISTS `characteristics`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `characteristics` (
  `BodyId` bigint NOT NULL,
  `Type` int NOT NULL,
  `CharacteristicId` bigint NOT NULL,
  PRIMARY KEY (`BodyId`,`Type`),
  KEY `FK_Characteristics_CharacteristicValues` (`CharacteristicId`),
  CONSTRAINT `FK_Characteristics_Bodies` FOREIGN KEY (`BodyId`) REFERENCES `bodies` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_Characteristics_CharacteristicValues` FOREIGN KEY (`CharacteristicId`) REFERENCES `characteristicvalues` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `characteristics`
--

LOCK TABLES `characteristics` WRITE;
/*!40000 ALTER TABLE `characteristics` DISABLE KEYS */;
/*!40000 ALTER TABLE `characteristics` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `characteristicvalues`
--

DROP TABLE IF EXISTS `characteristicvalues`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `characteristicvalues` (
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
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `characteristicvalues`
--

LOCK TABLES `characteristicvalues` WRITE;
/*!40000 ALTER TABLE `characteristicvalues` DISABLE KEYS */;
/*!40000 ALTER TABLE `characteristicvalues` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `characterknowledges`
--

DROP TABLE IF EXISTS `characterknowledges`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `characterknowledges` (
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
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `characterknowledges`
--

LOCK TABLES `characterknowledges` WRITE;
/*!40000 ALTER TABLE `characterknowledges` DISABLE KEYS */;
/*!40000 ALTER TABLE `characterknowledges` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `characterlog`
--

DROP TABLE IF EXISTS `characterlog`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `characterlog` (
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
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `characterlog`
--

LOCK TABLES `characterlog` WRITE;
/*!40000 ALTER TABLE `characterlog` DISABLE KEYS */;
/*!40000 ALTER TABLE `characterlog` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `characters`
--

DROP TABLE IF EXISTS `characters`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `characters` (
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
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `characters`
--

LOCK TABLES `characters` WRITE;
/*!40000 ALTER TABLE `characters` DISABLE KEYS */;
/*!40000 ALTER TABLE `characters` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `characters_accents`
--

DROP TABLE IF EXISTS `characters_accents`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `characters_accents` (
  `CharacterId` bigint NOT NULL,
  `AccentId` bigint NOT NULL,
  `Familiarity` int NOT NULL,
  `IsPreferred` bit(1) NOT NULL DEFAULT b'0',
  PRIMARY KEY (`CharacterId`,`AccentId`),
  KEY `FK_Characters_Accents_Accents_idx` (`AccentId`),
  CONSTRAINT `FK_Characters_Accents_Accents` FOREIGN KEY (`AccentId`) REFERENCES `accents` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_Characters_Accents_Characters` FOREIGN KEY (`CharacterId`) REFERENCES `characters` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `characters_accents`
--

LOCK TABLES `characters_accents` WRITE;
/*!40000 ALTER TABLE `characters_accents` DISABLE KEYS */;
/*!40000 ALTER TABLE `characters_accents` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `characters_chargenroles`
--

DROP TABLE IF EXISTS `characters_chargenroles`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `characters_chargenroles` (
  `CharacterId` bigint NOT NULL,
  `ChargenRoleId` bigint NOT NULL,
  PRIMARY KEY (`CharacterId`,`ChargenRoleId`),
  KEY `FK_Characters_ChargenRoles_ChargenRoles` (`ChargenRoleId`),
  CONSTRAINT `FK_Characters_ChargenRoles_Characters` FOREIGN KEY (`CharacterId`) REFERENCES `characters` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_Characters_ChargenRoles_ChargenRoles` FOREIGN KEY (`ChargenRoleId`) REFERENCES `chargenroles` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `characters_chargenroles`
--

LOCK TABLES `characters_chargenroles` WRITE;
/*!40000 ALTER TABLE `characters_chargenroles` DISABLE KEYS */;
/*!40000 ALTER TABLE `characters_chargenroles` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `characters_languages`
--

DROP TABLE IF EXISTS `characters_languages`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `characters_languages` (
  `CharacterId` bigint NOT NULL,
  `LanguageId` bigint NOT NULL,
  PRIMARY KEY (`CharacterId`,`LanguageId`),
  KEY `FK_Characters_Languages_Languages_idx` (`LanguageId`),
  CONSTRAINT `FK_Characters_Languages_Characters` FOREIGN KEY (`CharacterId`) REFERENCES `characters` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_Characters_Languages_Languages` FOREIGN KEY (`LanguageId`) REFERENCES `languages` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `characters_languages`
--

LOCK TABLES `characters_languages` WRITE;
/*!40000 ALTER TABLE `characters_languages` DISABLE KEYS */;
/*!40000 ALTER TABLE `characters_languages` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `characters_magicresources`
--

DROP TABLE IF EXISTS `characters_magicresources`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `characters_magicresources` (
  `CharacterId` bigint NOT NULL,
  `MagicResourceId` bigint NOT NULL,
  `Amount` double NOT NULL,
  PRIMARY KEY (`CharacterId`,`MagicResourceId`),
  KEY `FK_Characters_MagicResources_MagicResources_idx` (`MagicResourceId`),
  CONSTRAINT `FK_Characters_MagicResources_Characters` FOREIGN KEY (`CharacterId`) REFERENCES `characters` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_Characters_MagicResources_MagicResources` FOREIGN KEY (`MagicResourceId`) REFERENCES `magicresources` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `characters_magicresources`
--

LOCK TABLES `characters_magicresources` WRITE;
/*!40000 ALTER TABLE `characters_magicresources` DISABLE KEYS */;
/*!40000 ALTER TABLE `characters_magicresources` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `characters_scripts`
--

DROP TABLE IF EXISTS `characters_scripts`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `characters_scripts` (
  `CharacterId` bigint NOT NULL,
  `ScriptId` bigint NOT NULL,
  PRIMARY KEY (`CharacterId`,`ScriptId`),
  KEY `FK_Characters_Scripts_Scripts_idx` (`ScriptId`),
  CONSTRAINT `FK_Characters_Scripts_Characters` FOREIGN KEY (`CharacterId`) REFERENCES `characters` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_Characters_Scripts_Scripts` FOREIGN KEY (`ScriptId`) REFERENCES `scripts` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `characters_scripts`
--

LOCK TABLES `characters_scripts` WRITE;
/*!40000 ALTER TABLE `characters_scripts` DISABLE KEYS */;
/*!40000 ALTER TABLE `characters_scripts` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `chargenadvices`
--

DROP TABLE IF EXISTS `chargenadvices`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `chargenadvices` (
  `Id` bigint NOT NULL AUTO_INCREMENT,
  `ChargenStage` int NOT NULL,
  `AdviceTitle` text CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `AdviceText` text CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `ShouldShowAdviceProgId` bigint DEFAULT NULL,
  PRIMARY KEY (`Id`),
  KEY `FK_ChargenAdvices_FutureProgs_idx` (`ShouldShowAdviceProgId`),
  CONSTRAINT `FK_ChargenAdvices_FutureProgs` FOREIGN KEY (`ShouldShowAdviceProgId`) REFERENCES `futureprogs` (`Id`) ON DELETE SET NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `chargenadvices`
--

LOCK TABLES `chargenadvices` WRITE;
/*!40000 ALTER TABLE `chargenadvices` DISABLE KEYS */;
/*!40000 ALTER TABLE `chargenadvices` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `chargenadvices_chargenroles`
--

DROP TABLE IF EXISTS `chargenadvices_chargenroles`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `chargenadvices_chargenroles` (
  `ChargenAdviceId` bigint NOT NULL,
  `ChargenRoleId` bigint NOT NULL,
  PRIMARY KEY (`ChargenAdviceId`,`ChargenRoleId`),
  KEY `FK_ChargenAdvices_ChargenRoles_ChargenRoles_idx` (`ChargenRoleId`),
  CONSTRAINT `FK_ChargenAdvices_ChargenRoles_ChargenAdvices` FOREIGN KEY (`ChargenAdviceId`) REFERENCES `chargenadvices` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_ChargenAdvices_ChargenRoles_ChargenRoles` FOREIGN KEY (`ChargenRoleId`) REFERENCES `chargenroles` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `chargenadvices_chargenroles`
--

LOCK TABLES `chargenadvices_chargenroles` WRITE;
/*!40000 ALTER TABLE `chargenadvices_chargenroles` DISABLE KEYS */;
/*!40000 ALTER TABLE `chargenadvices_chargenroles` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `chargenadvices_cultures`
--

DROP TABLE IF EXISTS `chargenadvices_cultures`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `chargenadvices_cultures` (
  `ChargenAdviceId` bigint NOT NULL,
  `CultureId` bigint NOT NULL,
  PRIMARY KEY (`ChargenAdviceId`,`CultureId`),
  KEY `FK_ChargenAdvices_Cultures_Cultures_idx` (`CultureId`),
  CONSTRAINT `FK_ChargenAdvices_Cultures_ChargenAdvices` FOREIGN KEY (`ChargenAdviceId`) REFERENCES `chargenadvices` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_ChargenAdvices_Cultures_Cultures` FOREIGN KEY (`CultureId`) REFERENCES `cultures` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `chargenadvices_cultures`
--

LOCK TABLES `chargenadvices_cultures` WRITE;
/*!40000 ALTER TABLE `chargenadvices_cultures` DISABLE KEYS */;
/*!40000 ALTER TABLE `chargenadvices_cultures` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `chargenadvices_ethnicities`
--

DROP TABLE IF EXISTS `chargenadvices_ethnicities`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `chargenadvices_ethnicities` (
  `ChargenAdviceId` bigint NOT NULL,
  `EthnicityId` bigint NOT NULL,
  PRIMARY KEY (`ChargenAdviceId`,`EthnicityId`),
  KEY `FK_ChargenAdvices_Ethnicities_Ethnicities_idx` (`EthnicityId`),
  CONSTRAINT `FK_ChargenAdvices_Ethnicities_ChargenAdvices` FOREIGN KEY (`ChargenAdviceId`) REFERENCES `chargenadvices` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_ChargenAdvices_Ethnicities_Ethnicities` FOREIGN KEY (`EthnicityId`) REFERENCES `ethnicities` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `chargenadvices_ethnicities`
--

LOCK TABLES `chargenadvices_ethnicities` WRITE;
/*!40000 ALTER TABLE `chargenadvices_ethnicities` DISABLE KEYS */;
/*!40000 ALTER TABLE `chargenadvices_ethnicities` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `chargenadvices_races`
--

DROP TABLE IF EXISTS `chargenadvices_races`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `chargenadvices_races` (
  `ChargenAdviceId` bigint NOT NULL,
  `RaceId` bigint NOT NULL,
  PRIMARY KEY (`ChargenAdviceId`,`RaceId`),
  KEY `FK_ChargenAdvices_Races_Races_idx` (`RaceId`),
  CONSTRAINT `FK_ChargenAdvices_Races_ChargenAdvices` FOREIGN KEY (`ChargenAdviceId`) REFERENCES `chargenadvices` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_ChargenAdvices_Races_Races` FOREIGN KEY (`RaceId`) REFERENCES `races` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `chargenadvices_races`
--

LOCK TABLES `chargenadvices_races` WRITE;
/*!40000 ALTER TABLE `chargenadvices_races` DISABLE KEYS */;
/*!40000 ALTER TABLE `chargenadvices_races` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `chargenresources`
--

DROP TABLE IF EXISTS `chargenresources`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `chargenresources` (
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
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `chargenresources`
--

LOCK TABLES `chargenresources` WRITE;
/*!40000 ALTER TABLE `chargenresources` DISABLE KEYS */;
/*!40000 ALTER TABLE `chargenresources` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `chargenroles`
--

DROP TABLE IF EXISTS `chargenroles`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `chargenroles` (
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
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `chargenroles`
--

LOCK TABLES `chargenroles` WRITE;
/*!40000 ALTER TABLE `chargenroles` DISABLE KEYS */;
/*!40000 ALTER TABLE `chargenroles` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `chargenroles_approvers`
--

DROP TABLE IF EXISTS `chargenroles_approvers`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `chargenroles_approvers` (
  `ChargenRoleId` bigint NOT NULL,
  `ApproverId` bigint NOT NULL,
  PRIMARY KEY (`ChargenRoleId`,`ApproverId`),
  KEY `FK_ChargenRoles_Approvers_Accounts` (`ApproverId`),
  CONSTRAINT `FK_ChargenRoles_Approvers_Accounts` FOREIGN KEY (`ApproverId`) REFERENCES `accounts` (`Id`) ON DELETE RESTRICT,
  CONSTRAINT `FK_ChargenRoles_Approvers_ChargenRoles` FOREIGN KEY (`ChargenRoleId`) REFERENCES `chargenroles` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `chargenroles_approvers`
--

LOCK TABLES `chargenroles_approvers` WRITE;
/*!40000 ALTER TABLE `chargenroles_approvers` DISABLE KEYS */;
/*!40000 ALTER TABLE `chargenroles_approvers` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `chargenroles_clanmemberships`
--

DROP TABLE IF EXISTS `chargenroles_clanmemberships`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `chargenroles_clanmemberships` (
  `ChargenRoleId` bigint NOT NULL,
  `ClanId` bigint NOT NULL,
  `RankId` bigint NOT NULL,
  `PaygradeId` bigint DEFAULT NULL,
  PRIMARY KEY (`ChargenRoleId`,`ClanId`),
  KEY `FK_ChargenRoles_ClanMemberships_Clans` (`ClanId`),
  CONSTRAINT `FK_ChargenRoles_ClanMemberships_ChargenRoles` FOREIGN KEY (`ChargenRoleId`) REFERENCES `chargenroles` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_ChargenRoles_ClanMemberships_Clans` FOREIGN KEY (`ClanId`) REFERENCES `clans` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `chargenroles_clanmemberships`
--

LOCK TABLES `chargenroles_clanmemberships` WRITE;
/*!40000 ALTER TABLE `chargenroles_clanmemberships` DISABLE KEYS */;
/*!40000 ALTER TABLE `chargenroles_clanmemberships` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `chargenroles_clanmemberships_appointments`
--

DROP TABLE IF EXISTS `chargenroles_clanmemberships_appointments`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `chargenroles_clanmemberships_appointments` (
  `ChargenRoleId` bigint NOT NULL,
  `ClanId` bigint NOT NULL,
  `AppointmentId` bigint NOT NULL,
  PRIMARY KEY (`ChargenRoleId`,`ClanId`,`AppointmentId`),
  CONSTRAINT `FK_CRCMA_ChargenRoles_ClanMemberships` FOREIGN KEY (`ChargenRoleId`, `ClanId`) REFERENCES `chargenroles_clanmemberships` (`ChargenRoleId`, `ClanId`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `chargenroles_clanmemberships_appointments`
--

LOCK TABLES `chargenroles_clanmemberships_appointments` WRITE;
/*!40000 ALTER TABLE `chargenroles_clanmemberships_appointments` DISABLE KEYS */;
/*!40000 ALTER TABLE `chargenroles_clanmemberships_appointments` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `chargenroles_costs`
--

DROP TABLE IF EXISTS `chargenroles_costs`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `chargenroles_costs` (
  `ChargenRoleId` bigint NOT NULL,
  `ChargenResourceId` bigint NOT NULL,
  `RequirementOnly` bit(1) NOT NULL DEFAULT b'0',
  `Amount` int NOT NULL,
  PRIMARY KEY (`ChargenRoleId`,`ChargenResourceId`,`RequirementOnly`),
  KEY `FK_ChargenRoles_Costs_ChargenResources` (`ChargenResourceId`),
  CONSTRAINT `FK_ChargenRoles_Costs_ChargenResources` FOREIGN KEY (`ChargenResourceId`) REFERENCES `chargenresources` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_ChargenRoles_Costs_ChargenRoles` FOREIGN KEY (`ChargenRoleId`) REFERENCES `chargenroles` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `chargenroles_costs`
--

LOCK TABLES `chargenroles_costs` WRITE;
/*!40000 ALTER TABLE `chargenroles_costs` DISABLE KEYS */;
/*!40000 ALTER TABLE `chargenroles_costs` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `chargenroles_currencies`
--

DROP TABLE IF EXISTS `chargenroles_currencies`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `chargenroles_currencies` (
  `ChargenRoleId` bigint NOT NULL,
  `CurrencyId` bigint NOT NULL,
  `Amount` decimal(58,29) NOT NULL,
  PRIMARY KEY (`ChargenRoleId`,`CurrencyId`),
  KEY `FK_ChargenRoles_Currencies_Currencies` (`CurrencyId`),
  CONSTRAINT `FK_ChargenRoles_Currencies_ChargenRoles` FOREIGN KEY (`ChargenRoleId`) REFERENCES `chargenroles` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_ChargenRoles_Currencies_Currencies` FOREIGN KEY (`CurrencyId`) REFERENCES `currencies` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `chargenroles_currencies`
--

LOCK TABLES `chargenroles_currencies` WRITE;
/*!40000 ALTER TABLE `chargenroles_currencies` DISABLE KEYS */;
/*!40000 ALTER TABLE `chargenroles_currencies` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `chargenroles_merits`
--

DROP TABLE IF EXISTS `chargenroles_merits`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `chargenroles_merits` (
  `ChargenRoleId` bigint NOT NULL,
  `MeritId` bigint NOT NULL,
  PRIMARY KEY (`ChargenRoleId`,`MeritId`),
  KEY `FK_ChargenRoles_Merits_Merits_idx` (`MeritId`),
  CONSTRAINT `FK_ChargenRoles_Merits_ChargenRoles` FOREIGN KEY (`ChargenRoleId`) REFERENCES `chargenroles` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_ChargenRoles_Merits_Merits` FOREIGN KEY (`MeritId`) REFERENCES `merits` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `chargenroles_merits`
--

LOCK TABLES `chargenroles_merits` WRITE;
/*!40000 ALTER TABLE `chargenroles_merits` DISABLE KEYS */;
/*!40000 ALTER TABLE `chargenroles_merits` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `chargenroles_traits`
--

DROP TABLE IF EXISTS `chargenroles_traits`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `chargenroles_traits` (
  `ChargenRoleId` bigint NOT NULL,
  `TraitId` bigint NOT NULL,
  `Amount` double NOT NULL,
  `GiveIfDoesntHave` bit(1) NOT NULL DEFAULT b'0',
  PRIMARY KEY (`ChargenRoleId`,`TraitId`),
  KEY `FK_ChargenRoles_Traits_Currencies` (`TraitId`),
  CONSTRAINT `FK_ChargenRoles_Traits_ChargenRoles` FOREIGN KEY (`ChargenRoleId`) REFERENCES `chargenroles` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_ChargenRoles_Traits_Currencies` FOREIGN KEY (`TraitId`) REFERENCES `traitdefinitions` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `chargenroles_traits`
--

LOCK TABLES `chargenroles_traits` WRITE;
/*!40000 ALTER TABLE `chargenroles_traits` DISABLE KEYS */;
/*!40000 ALTER TABLE `chargenroles_traits` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `chargens`
--

DROP TABLE IF EXISTS `chargens`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `chargens` (
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
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `chargens`
--

LOCK TABLES `chargens` WRITE;
/*!40000 ALTER TABLE `chargens` DISABLE KEYS */;
/*!40000 ALTER TABLE `chargens` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `chargenscreenstoryboarddependentstages`
--

DROP TABLE IF EXISTS `chargenscreenstoryboarddependentstages`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `chargenscreenstoryboarddependentstages` (
  `OwnerId` bigint NOT NULL,
  `Dependency` int NOT NULL,
  PRIMARY KEY (`OwnerId`,`Dependency`),
  KEY `FK_ChargenScreenStoryboardDependentStages_Owner` (`OwnerId`),
  CONSTRAINT `FK_ChargenScreenStoryboardDependentStages_Owner` FOREIGN KEY (`OwnerId`) REFERENCES `chargenscreenstoryboards` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `chargenscreenstoryboarddependentstages`
--

LOCK TABLES `chargenscreenstoryboarddependentstages` WRITE;
/*!40000 ALTER TABLE `chargenscreenstoryboarddependentstages` DISABLE KEYS */;
/*!40000 ALTER TABLE `chargenscreenstoryboarddependentstages` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `chargenscreenstoryboards`
--

DROP TABLE IF EXISTS `chargenscreenstoryboards`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `chargenscreenstoryboards` (
  `Id` bigint NOT NULL AUTO_INCREMENT,
  `ChargenType` varchar(50) DEFAULT NULL,
  `ChargenStage` int NOT NULL,
  `Order` int NOT NULL,
  `StageDefinition` mediumtext CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `NextStage` int NOT NULL,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `chargenscreenstoryboards`
--

LOCK TABLES `chargenscreenstoryboards` WRITE;
/*!40000 ALTER TABLE `chargenscreenstoryboards` DISABLE KEYS */;
/*!40000 ALTER TABLE `chargenscreenstoryboards` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `checks`
--

DROP TABLE IF EXISTS `checks`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `checks` (
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
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `checks`
--

LOCK TABLES `checks` WRITE;
/*!40000 ALTER TABLE `checks` DISABLE KEYS */;
/*!40000 ALTER TABLE `checks` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `checktemplatedifficulties`
--

DROP TABLE IF EXISTS `checktemplatedifficulties`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `checktemplatedifficulties` (
  `CheckTemplateId` bigint NOT NULL,
  `Difficulty` int NOT NULL,
  `Modifier` double NOT NULL,
  PRIMARY KEY (`Difficulty`,`CheckTemplateId`),
  KEY `FK_CheckTemplateDifficulties_CheckTemplates` (`CheckTemplateId`),
  CONSTRAINT `FK_CheckTemplateDifficulties_CheckTemplates` FOREIGN KEY (`CheckTemplateId`) REFERENCES `checktemplates` (`Id`) ON DELETE RESTRICT
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `checktemplatedifficulties`
--

LOCK TABLES `checktemplatedifficulties` WRITE;
/*!40000 ALTER TABLE `checktemplatedifficulties` DISABLE KEYS */;
/*!40000 ALTER TABLE `checktemplatedifficulties` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `checktemplates`
--

DROP TABLE IF EXISTS `checktemplates`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `checktemplates` (
  `Id` bigint NOT NULL AUTO_INCREMENT,
  `Name` varchar(4000) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `Definition` text CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci,
  `CheckMethod` varchar(25) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL DEFAULT 'Standard',
  `ImproveTraits` bit(1) NOT NULL DEFAULT b'0',
  `FailIfTraitMissingMode` smallint NOT NULL,
  `CanBranchIfTraitMissing` bit(1) NOT NULL DEFAULT b'0',
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `checktemplates`
--

LOCK TABLES `checktemplates` WRITE;
/*!40000 ALTER TABLE `checktemplates` DISABLE KEYS */;
/*!40000 ALTER TABLE `checktemplates` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `clanmemberships`
--

DROP TABLE IF EXISTS `clanmemberships`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `clanmemberships` (
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
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `clanmemberships`
--

LOCK TABLES `clanmemberships` WRITE;
/*!40000 ALTER TABLE `clanmemberships` DISABLE KEYS */;
/*!40000 ALTER TABLE `clanmemberships` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `clanmemberships_appointments`
--

DROP TABLE IF EXISTS `clanmemberships_appointments`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `clanmemberships_appointments` (
  `ClanId` bigint NOT NULL,
  `CharacterId` bigint NOT NULL,
  `AppointmentId` bigint NOT NULL,
  PRIMARY KEY (`ClanId`,`CharacterId`,`AppointmentId`),
  KEY `FK_ClanMemberships_Appointments_Appointments` (`AppointmentId`),
  CONSTRAINT `FK_ClanMemberships_Appointments_Appointments` FOREIGN KEY (`AppointmentId`) REFERENCES `appointments` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_ClanMemberships_Appointments_ClanMemberships` FOREIGN KEY (`ClanId`, `CharacterId`) REFERENCES `clanmemberships` (`ClanId`, `CharacterId`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `clanmemberships_appointments`
--

LOCK TABLES `clanmemberships_appointments` WRITE;
/*!40000 ALTER TABLE `clanmemberships_appointments` DISABLE KEYS */;
/*!40000 ALTER TABLE `clanmemberships_appointments` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `clanmemberships_backpay`
--

DROP TABLE IF EXISTS `clanmemberships_backpay`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `clanmemberships_backpay` (
  `ClanId` bigint NOT NULL,
  `CharacterId` bigint NOT NULL,
  `CurrencyId` bigint NOT NULL,
  `Amount` decimal(58,29) NOT NULL,
  PRIMARY KEY (`CurrencyId`,`ClanId`,`CharacterId`),
  KEY `FK_ClanMemberships_Backpay_ClanMemberships` (`ClanId`,`CharacterId`),
  CONSTRAINT `FK_ClanMemberships_Backpay_ClanMemberships` FOREIGN KEY (`ClanId`, `CharacterId`) REFERENCES `clanmemberships` (`ClanId`, `CharacterId`) ON DELETE CASCADE,
  CONSTRAINT `FK_ClanMemberships_Backpay_Currencies` FOREIGN KEY (`CurrencyId`) REFERENCES `currencies` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `clanmemberships_backpay`
--

LOCK TABLES `clanmemberships_backpay` WRITE;
/*!40000 ALTER TABLE `clanmemberships_backpay` DISABLE KEYS */;
/*!40000 ALTER TABLE `clanmemberships_backpay` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `clans`
--

DROP TABLE IF EXISTS `clans`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `clans` (
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
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `clans`
--

LOCK TABLES `clans` WRITE;
/*!40000 ALTER TABLE `clans` DISABLE KEYS */;
/*!40000 ALTER TABLE `clans` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `clans_administrationcells`
--

DROP TABLE IF EXISTS `clans_administrationcells`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `clans_administrationcells` (
  `ClanId` bigint NOT NULL,
  `CellId` bigint NOT NULL,
  PRIMARY KEY (`ClanId`,`CellId`),
  KEY `FK_Clans_AdministrationCells_Cells_idx` (`CellId`),
  CONSTRAINT `FK_Clans_AdministrationCells_Cells` FOREIGN KEY (`CellId`) REFERENCES `cells` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_Clans_AdministrationCells_Clans` FOREIGN KEY (`ClanId`) REFERENCES `clans` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `clans_administrationcells`
--

LOCK TABLES `clans_administrationcells` WRITE;
/*!40000 ALTER TABLE `clans_administrationcells` DISABLE KEYS */;
/*!40000 ALTER TABLE `clans_administrationcells` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `clans_treasurycells`
--

DROP TABLE IF EXISTS `clans_treasurycells`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `clans_treasurycells` (
  `ClanId` bigint NOT NULL,
  `CellId` bigint NOT NULL,
  PRIMARY KEY (`ClanId`,`CellId`),
  KEY `FK_Clans_TreasuryCells_Cells_idx` (`CellId`),
  CONSTRAINT `FK_Clans_TreasuryCells_Cells` FOREIGN KEY (`CellId`) REFERENCES `cells` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_Clans_TreasuryCells_Clans` FOREIGN KEY (`ClanId`) REFERENCES `clans` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `clans_treasurycells`
--

LOCK TABLES `clans_treasurycells` WRITE;
/*!40000 ALTER TABLE `clans_treasurycells` DISABLE KEYS */;
/*!40000 ALTER TABLE `clans_treasurycells` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `climatemodels`
--

DROP TABLE IF EXISTS `climatemodels`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `climatemodels` (
  `Id` bigint NOT NULL AUTO_INCREMENT,
  `Name` varchar(200) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `Type` varchar(45) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `MinuteProcessingInterval` int NOT NULL,
  `MinimumMinutesBetweenFlavourEchoes` int NOT NULL,
  `MinuteFlavourEchoChance` double NOT NULL,
  `Description` text CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `climatemodels`
--

LOCK TABLES `climatemodels` WRITE;
/*!40000 ALTER TABLE `climatemodels` DISABLE KEYS */;
/*!40000 ALTER TABLE `climatemodels` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `climatemodelseason`
--

DROP TABLE IF EXISTS `climatemodelseason`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `climatemodelseason` (
  `ClimateModelId` bigint NOT NULL,
  `SeasonId` bigint NOT NULL,
  `MaximumAdditionalChangeChanceFromStableWeather` double NOT NULL,
  `IncrementalAdditionalChangeChanceFromStableWeather` double NOT NULL,
  PRIMARY KEY (`ClimateModelId`,`SeasonId`),
  KEY `IX_ClimateModelSeason_SeasonId` (`SeasonId`),
  CONSTRAINT `FK_ClimateModelSeasons_ClimateModels` FOREIGN KEY (`ClimateModelId`) REFERENCES `climatemodels` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_ClimateModelSeasons_Seasons` FOREIGN KEY (`SeasonId`) REFERENCES `seasons` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `climatemodelseason`
--

LOCK TABLES `climatemodelseason` WRITE;
/*!40000 ALTER TABLE `climatemodelseason` DISABLE KEYS */;
/*!40000 ALTER TABLE `climatemodelseason` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `climatemodelseasonevent`
--

DROP TABLE IF EXISTS `climatemodelseasonevent`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `climatemodelseasonevent` (
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
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `climatemodelseasonevent`
--

LOCK TABLES `climatemodelseasonevent` WRITE;
/*!40000 ALTER TABLE `climatemodelseasonevent` DISABLE KEYS */;
/*!40000 ALTER TABLE `climatemodelseasonevent` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `clocks`
--

DROP TABLE IF EXISTS `clocks`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `clocks` (
  `Id` bigint NOT NULL AUTO_INCREMENT,
  `Definition` text CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `Seconds` int NOT NULL,
  `Minutes` int NOT NULL,
  `Hours` int NOT NULL,
  `PrimaryTimezoneId` bigint NOT NULL,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `clocks`
--

LOCK TABLES `clocks` WRITE;
/*!40000 ALTER TABLE `clocks` DISABLE KEYS */;
/*!40000 ALTER TABLE `clocks` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `coins`
--

DROP TABLE IF EXISTS `coins`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `coins` (
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
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `coins`
--

LOCK TABLES `coins` WRITE;
/*!40000 ALTER TABLE `coins` DISABLE KEYS */;
/*!40000 ALTER TABLE `coins` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `colours`
--

DROP TABLE IF EXISTS `colours`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `colours` (
  `Id` bigint NOT NULL AUTO_INCREMENT,
  `Name` varchar(50) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `Basic` int NOT NULL,
  `Red` int NOT NULL,
  `Green` int NOT NULL,
  `Blue` int NOT NULL,
  `Fancy` varchar(4000) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci DEFAULT NULL,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `colours`
--

LOCK TABLES `colours` WRITE;
/*!40000 ALTER TABLE `colours` DISABLE KEYS */;
/*!40000 ALTER TABLE `colours` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `combatactions`
--

DROP TABLE IF EXISTS `combatactions`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `combatactions` (
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
  PRIMARY KEY (`Id`),
  KEY `IX_CombatActions_TraitDefinitionId` (`TraitDefinitionId`),
  KEY `IX_CombatActions_UsabilityProgId` (`UsabilityProgId`),
  CONSTRAINT `FK_CombatActions_FutureProgs_UsabilityProgId` FOREIGN KEY (`UsabilityProgId`) REFERENCES `futureprogs` (`Id`),
  CONSTRAINT `FK_CombatActions_TraitDefinitions_TraitDefinitionId` FOREIGN KEY (`TraitDefinitionId`) REFERENCES `traitdefinitions` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `combatactions`
--

LOCK TABLES `combatactions` WRITE;
/*!40000 ALTER TABLE `combatactions` DISABLE KEYS */;
/*!40000 ALTER TABLE `combatactions` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `combatmessages`
--

DROP TABLE IF EXISTS `combatmessages`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `combatmessages` (
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
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `combatmessages`
--

LOCK TABLES `combatmessages` WRITE;
/*!40000 ALTER TABLE `combatmessages` DISABLE KEYS */;
/*!40000 ALTER TABLE `combatmessages` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `combatmessages_combatactions`
--

DROP TABLE IF EXISTS `combatmessages_combatactions`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `combatmessages_combatactions` (
  `CombatMessageId` bigint NOT NULL,
  `CombatActionId` bigint NOT NULL,
  PRIMARY KEY (`CombatMessageId`,`CombatActionId`),
  KEY `FK_CombatMessages_CombatActions_WeaponAttacks_idx` (`CombatActionId`),
  CONSTRAINT `FK_CombatMessages_CombatActions_CombatMessages` FOREIGN KEY (`CombatMessageId`) REFERENCES `combatmessages` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_CombatMessages_CombatActions_WeaponAttacks` FOREIGN KEY (`CombatActionId`) REFERENCES `combatactions` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `combatmessages_combatactions`
--

LOCK TABLES `combatmessages_combatactions` WRITE;
/*!40000 ALTER TABLE `combatmessages_combatactions` DISABLE KEYS */;
/*!40000 ALTER TABLE `combatmessages_combatactions` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `combatmessages_weaponattacks`
--

DROP TABLE IF EXISTS `combatmessages_weaponattacks`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `combatmessages_weaponattacks` (
  `CombatMessageId` bigint NOT NULL,
  `WeaponAttackId` bigint NOT NULL,
  PRIMARY KEY (`CombatMessageId`,`WeaponAttackId`),
  KEY `FK_CombatMessages_WeaponAttacks_WeaponAttacks_idx` (`WeaponAttackId`),
  CONSTRAINT `FK_CombatMessages_WeaponAttacks_CombatMessages` FOREIGN KEY (`CombatMessageId`) REFERENCES `combatmessages` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_CombatMessages_WeaponAttacks_WeaponAttacks` FOREIGN KEY (`WeaponAttackId`) REFERENCES `weaponattacks` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `combatmessages_weaponattacks`
--

LOCK TABLES `combatmessages_weaponattacks` WRITE;
/*!40000 ALTER TABLE `combatmessages_weaponattacks` DISABLE KEYS */;
/*!40000 ALTER TABLE `combatmessages_weaponattacks` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `conveyancinglocations`
--

DROP TABLE IF EXISTS `conveyancinglocations`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `conveyancinglocations` (
  `EconomicZoneId` bigint NOT NULL,
  `CellId` bigint NOT NULL,
  PRIMARY KEY (`EconomicZoneId`,`CellId`),
  KEY `IX_ConveyancingLocations_CellId` (`CellId`),
  CONSTRAINT `FK_ConveyancingLocations_Cells` FOREIGN KEY (`CellId`) REFERENCES `cells` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_ConveyancingLocations_EconomicZones` FOREIGN KEY (`EconomicZoneId`) REFERENCES `economiczones` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `conveyancinglocations`
--

LOCK TABLES `conveyancinglocations` WRITE;
/*!40000 ALTER TABLE `conveyancinglocations` DISABLE KEYS */;
/*!40000 ALTER TABLE `conveyancinglocations` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `corpsemodels`
--

DROP TABLE IF EXISTS `corpsemodels`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `corpsemodels` (
  `Id` bigint NOT NULL AUTO_INCREMENT,
  `Name` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `Description` text CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci,
  `Definition` text CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci,
  `Type` varchar(45) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `corpsemodels`
--

LOCK TABLES `corpsemodels` WRITE;
/*!40000 ALTER TABLE `corpsemodels` DISABLE KEYS */;
/*!40000 ALTER TABLE `corpsemodels` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `corpserecoveryreports`
--

DROP TABLE IF EXISTS `corpserecoveryreports`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `corpserecoveryreports` (
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
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `corpserecoveryreports`
--

LOCK TABLES `corpserecoveryreports` WRITE;
/*!40000 ALTER TABLE `corpserecoveryreports` DISABLE KEYS */;
/*!40000 ALTER TABLE `corpserecoveryreports` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `craftinputs`
--

DROP TABLE IF EXISTS `craftinputs`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `craftinputs` (
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
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `craftinputs`
--

LOCK TABLES `craftinputs` WRITE;
/*!40000 ALTER TABLE `craftinputs` DISABLE KEYS */;
/*!40000 ALTER TABLE `craftinputs` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `craftphases`
--

DROP TABLE IF EXISTS `craftphases`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `craftphases` (
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
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `craftphases`
--

LOCK TABLES `craftphases` WRITE;
/*!40000 ALTER TABLE `craftphases` DISABLE KEYS */;
/*!40000 ALTER TABLE `craftphases` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `craftproducts`
--

DROP TABLE IF EXISTS `craftproducts`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `craftproducts` (
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
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `craftproducts`
--

LOCK TABLES `craftproducts` WRITE;
/*!40000 ALTER TABLE `craftproducts` DISABLE KEYS */;
/*!40000 ALTER TABLE `craftproducts` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `crafts`
--

DROP TABLE IF EXISTS `crafts`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `crafts` (
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
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `crafts`
--

LOCK TABLES `crafts` WRITE;
/*!40000 ALTER TABLE `crafts` DISABLE KEYS */;
/*!40000 ALTER TABLE `crafts` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `crafttools`
--

DROP TABLE IF EXISTS `crafttools`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `crafttools` (
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
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `crafttools`
--

LOCK TABLES `crafttools` WRITE;
/*!40000 ALTER TABLE `crafttools` DISABLE KEYS */;
/*!40000 ALTER TABLE `crafttools` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `crimes`
--

DROP TABLE IF EXISTS `crimes`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `crimes` (
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
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `crimes`
--

LOCK TABLES `crimes` WRITE;
/*!40000 ALTER TABLE `crimes` DISABLE KEYS */;
/*!40000 ALTER TABLE `crimes` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `cultureinfos`
--

DROP TABLE IF EXISTS `cultureinfos`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `cultureinfos` (
  `Id` varchar(50) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `DisplayName` varchar(4000) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `Order` int NOT NULL,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `cultureinfos`
--

LOCK TABLES `cultureinfos` WRITE;
/*!40000 ALTER TABLE `cultureinfos` DISABLE KEYS */;
/*!40000 ALTER TABLE `cultureinfos` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `cultures`
--

DROP TABLE IF EXISTS `cultures`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `cultures` (
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
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `cultures`
--

LOCK TABLES `cultures` WRITE;
/*!40000 ALTER TABLE `cultures` DISABLE KEYS */;
/*!40000 ALTER TABLE `cultures` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `cultures_chargenresources`
--

DROP TABLE IF EXISTS `cultures_chargenresources`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `cultures_chargenresources` (
  `CultureId` bigint NOT NULL,
  `ChargenResourceId` bigint NOT NULL,
  `RequirementOnly` bit(1) NOT NULL DEFAULT b'0',
  `Amount` int NOT NULL,
  PRIMARY KEY (`CultureId`,`ChargenResourceId`),
  KEY `IX_Cultures_ChargenResources_ChargenResourceId` (`ChargenResourceId`),
  CONSTRAINT `FK_Cultures_ChargenResources_ChargenResources_ChargenResourceId` FOREIGN KEY (`ChargenResourceId`) REFERENCES `chargenresources` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_Cultures_ChargenResources_Cultures_CultureId` FOREIGN KEY (`CultureId`) REFERENCES `cultures` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `cultures_chargenresources`
--

LOCK TABLES `cultures_chargenresources` WRITE;
/*!40000 ALTER TABLE `cultures_chargenresources` DISABLE KEYS */;
/*!40000 ALTER TABLE `cultures_chargenresources` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `culturesnamecultures`
--

DROP TABLE IF EXISTS `culturesnamecultures`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `culturesnamecultures` (
  `CultureId` bigint NOT NULL,
  `NameCultureId` bigint NOT NULL,
  `Gender` smallint NOT NULL,
  PRIMARY KEY (`CultureId`,`NameCultureId`,`Gender`),
  KEY `IX_CulturesNameCultures_NameCultureId` (`NameCultureId`),
  CONSTRAINT `FK_CulturesNameCultures_Cultures` FOREIGN KEY (`CultureId`) REFERENCES `cultures` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_CulturesNameCultures_NameCultures` FOREIGN KEY (`NameCultureId`) REFERENCES `nameculture` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `culturesnamecultures`
--

LOCK TABLES `culturesnamecultures` WRITE;
/*!40000 ALTER TABLE `culturesnamecultures` DISABLE KEYS */;
/*!40000 ALTER TABLE `culturesnamecultures` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `currencies`
--

DROP TABLE IF EXISTS `currencies`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `currencies` (
  `Id` bigint NOT NULL AUTO_INCREMENT,
  `Name` varchar(4000) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `BaseCurrencyToGlobalBaseCurrencyConversion` decimal(58,29) NOT NULL DEFAULT '1.00000000000000000000000000000',
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `currencies`
--

LOCK TABLES `currencies` WRITE;
/*!40000 ALTER TABLE `currencies` DISABLE KEYS */;
/*!40000 ALTER TABLE `currencies` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `currencydescriptionpatternelements`
--

DROP TABLE IF EXISTS `currencydescriptionpatternelements`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `currencydescriptionpatternelements` (
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
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `currencydescriptionpatternelements`
--

LOCK TABLES `currencydescriptionpatternelements` WRITE;
/*!40000 ALTER TABLE `currencydescriptionpatternelements` DISABLE KEYS */;
/*!40000 ALTER TABLE `currencydescriptionpatternelements` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `currencydescriptionpatternelementspecialvalues`
--

DROP TABLE IF EXISTS `currencydescriptionpatternelementspecialvalues`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `currencydescriptionpatternelementspecialvalues` (
  `Value` decimal(58,29) NOT NULL,
  `CurrencyDescriptionPatternElementId` bigint NOT NULL,
  `Text` varchar(4000) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  PRIMARY KEY (`Value`,`CurrencyDescriptionPatternElementId`),
  KEY `FK_CDPESV_CDPE` (`CurrencyDescriptionPatternElementId`),
  CONSTRAINT `FK_CDPESV_CDPE` FOREIGN KEY (`CurrencyDescriptionPatternElementId`) REFERENCES `currencydescriptionpatternelements` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `currencydescriptionpatternelementspecialvalues`
--

LOCK TABLES `currencydescriptionpatternelementspecialvalues` WRITE;
/*!40000 ALTER TABLE `currencydescriptionpatternelementspecialvalues` DISABLE KEYS */;
/*!40000 ALTER TABLE `currencydescriptionpatternelementspecialvalues` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `currencydescriptionpatterns`
--

DROP TABLE IF EXISTS `currencydescriptionpatterns`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `currencydescriptionpatterns` (
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
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `currencydescriptionpatterns`
--

LOCK TABLES `currencydescriptionpatterns` WRITE;
/*!40000 ALTER TABLE `currencydescriptionpatterns` DISABLE KEYS */;
/*!40000 ALTER TABLE `currencydescriptionpatterns` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `currencydivisionabbreviations`
--

DROP TABLE IF EXISTS `currencydivisionabbreviations`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `currencydivisionabbreviations` (
  `Pattern` varchar(150) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `CurrencyDivisionId` bigint NOT NULL,
  PRIMARY KEY (`Pattern`,`CurrencyDivisionId`),
  KEY `FK_CurrencyDivisionAbbreviations_CurrencyDivisions` (`CurrencyDivisionId`),
  CONSTRAINT `FK_CurrencyDivisionAbbreviations_CurrencyDivisions` FOREIGN KEY (`CurrencyDivisionId`) REFERENCES `currencydivisions` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `currencydivisionabbreviations`
--

LOCK TABLES `currencydivisionabbreviations` WRITE;
/*!40000 ALTER TABLE `currencydivisionabbreviations` DISABLE KEYS */;
/*!40000 ALTER TABLE `currencydivisionabbreviations` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `currencydivisions`
--

DROP TABLE IF EXISTS `currencydivisions`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `currencydivisions` (
  `Id` bigint NOT NULL AUTO_INCREMENT,
  `Name` varchar(4000) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `BaseUnitConversionRate` decimal(58,29) NOT NULL,
  `CurrencyId` bigint NOT NULL,
  `IgnoreCase` bit(1) NOT NULL DEFAULT b'1',
  PRIMARY KEY (`Id`),
  KEY `FK_CurrencyDivisions_Currencies` (`CurrencyId`),
  CONSTRAINT `FK_CurrencyDivisions_Currencies` FOREIGN KEY (`CurrencyId`) REFERENCES `currencies` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `currencydivisions`
--

LOCK TABLES `currencydivisions` WRITE;
/*!40000 ALTER TABLE `currencydivisions` DISABLE KEYS */;
/*!40000 ALTER TABLE `currencydivisions` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `damagepatterns`
--

DROP TABLE IF EXISTS `damagepatterns`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `damagepatterns` (
  `Id` bigint NOT NULL AUTO_INCREMENT,
  `Name` varchar(4000) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `DamageType` int NOT NULL,
  `Dice` int NOT NULL,
  `Sides` int NOT NULL,
  `Bonus` int NOT NULL,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `damagepatterns`
--

LOCK TABLES `damagepatterns` WRITE;
/*!40000 ALTER TABLE `damagepatterns` DISABLE KEYS */;
/*!40000 ALTER TABLE `damagepatterns` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `defaulthooks`
--

DROP TABLE IF EXISTS `defaulthooks`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `defaulthooks` (
  `HookId` bigint NOT NULL,
  `PerceivableType` varchar(45) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL,
  `FutureProgId` bigint NOT NULL,
  PRIMARY KEY (`HookId`,`PerceivableType`,`FutureProgId`),
  KEY `FK_DefaultHooks_Futureprogs_idx` (`FutureProgId`),
  CONSTRAINT `FK_DefaultHooks_Futureprogs` FOREIGN KEY (`FutureProgId`) REFERENCES `futureprogs` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_DefaultHooks_Hooks` FOREIGN KEY (`HookId`) REFERENCES `hooks` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `defaulthooks`
--

LOCK TABLES `defaulthooks` WRITE;
/*!40000 ALTER TABLE `defaulthooks` DISABLE KEYS */;
/*!40000 ALTER TABLE `defaulthooks` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `disfigurementtemplates`
--

DROP TABLE IF EXISTS `disfigurementtemplates`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `disfigurementtemplates` (
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
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `disfigurementtemplates`
--

LOCK TABLES `disfigurementtemplates` WRITE;
/*!40000 ALTER TABLE `disfigurementtemplates` DISABLE KEYS */;
/*!40000 ALTER TABLE `disfigurementtemplates` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `doors`
--

DROP TABLE IF EXISTS `doors`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `doors` (
  `Id` bigint NOT NULL AUTO_INCREMENT,
  `Name` varchar(4000) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `Style` int NOT NULL,
  `IsOpen` bit(1) NOT NULL,
  `LockedWith` bigint DEFAULT NULL,
  PRIMARY KEY (`Id`),
  KEY `FK_Doors_Locks` (`LockedWith`),
  CONSTRAINT `FK_Doors_Locks` FOREIGN KEY (`LockedWith`) REFERENCES `locks` (`Id`) ON DELETE RESTRICT
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `doors`
--

LOCK TABLES `doors` WRITE;
/*!40000 ALTER TABLE `doors` DISABLE KEYS */;
/*!40000 ALTER TABLE `doors` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `drawings`
--

DROP TABLE IF EXISTS `drawings`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `drawings` (
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
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `drawings`
--

LOCK TABLES `drawings` WRITE;
/*!40000 ALTER TABLE `drawings` DISABLE KEYS */;
/*!40000 ALTER TABLE `drawings` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `dream_phases`
--

DROP TABLE IF EXISTS `dream_phases`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `dream_phases` (
  `DreamId` bigint NOT NULL,
  `PhaseId` int NOT NULL,
  `DreamerText` text CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL,
  `DreamerCommand` text CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL,
  `WaitSeconds` int NOT NULL DEFAULT '30',
  PRIMARY KEY (`DreamId`,`PhaseId`),
  CONSTRAINT `FK_Dream_Phases_Dreams` FOREIGN KEY (`DreamId`) REFERENCES `dreams` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `dream_phases`
--

LOCK TABLES `dream_phases` WRITE;
/*!40000 ALTER TABLE `dream_phases` DISABLE KEYS */;
/*!40000 ALTER TABLE `dream_phases` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `dreams`
--

DROP TABLE IF EXISTS `dreams`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `dreams` (
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
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `dreams`
--

LOCK TABLES `dreams` WRITE;
/*!40000 ALTER TABLE `dreams` DISABLE KEYS */;
/*!40000 ALTER TABLE `dreams` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `dreams_already_dreamt`
--

DROP TABLE IF EXISTS `dreams_already_dreamt`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `dreams_already_dreamt` (
  `DreamId` bigint NOT NULL,
  `CharacterId` bigint NOT NULL,
  PRIMARY KEY (`DreamId`,`CharacterId`),
  KEY `FK_Dreams_Dreamt_Characters_idx` (`CharacterId`),
  CONSTRAINT `FK_Dreams_Dreamt_Characters` FOREIGN KEY (`CharacterId`) REFERENCES `characters` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_Dreams_Dreamt_Dreams` FOREIGN KEY (`DreamId`) REFERENCES `dreams` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `dreams_already_dreamt`
--

LOCK TABLES `dreams_already_dreamt` WRITE;
/*!40000 ALTER TABLE `dreams_already_dreamt` DISABLE KEYS */;
/*!40000 ALTER TABLE `dreams_already_dreamt` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `dreams_characters`
--

DROP TABLE IF EXISTS `dreams_characters`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `dreams_characters` (
  `DreamId` bigint NOT NULL,
  `CharacterId` bigint NOT NULL,
  PRIMARY KEY (`DreamId`,`CharacterId`),
  KEY `FK_Dreams_Characters_Characters_idx` (`CharacterId`),
  CONSTRAINT `FK_Dreams_Characters_Characters` FOREIGN KEY (`CharacterId`) REFERENCES `characters` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_Dreams_Characters_Dreams` FOREIGN KEY (`DreamId`) REFERENCES `dreams` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `dreams_characters`
--

LOCK TABLES `dreams_characters` WRITE;
/*!40000 ALTER TABLE `dreams_characters` DISABLE KEYS */;
/*!40000 ALTER TABLE `dreams_characters` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `drugs`
--

DROP TABLE IF EXISTS `drugs`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `drugs` (
  `Id` bigint NOT NULL AUTO_INCREMENT,
  `Name` varchar(200) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL,
  `DrugVectors` int NOT NULL,
  `IntensityPerGram` double NOT NULL,
  `RelativeMetabolisationRate` double NOT NULL,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `drugs`
--

LOCK TABLES `drugs` WRITE;
/*!40000 ALTER TABLE `drugs` DISABLE KEYS */;
/*!40000 ALTER TABLE `drugs` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `drugsintensities`
--

DROP TABLE IF EXISTS `drugsintensities`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `drugsintensities` (
  `DrugId` bigint NOT NULL AUTO_INCREMENT,
  `DrugType` int NOT NULL,
  `RelativeIntensity` double NOT NULL,
  `AdditionalEffects` text CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci,
  PRIMARY KEY (`DrugId`,`DrugType`),
  CONSTRAINT `FK_Drugs_DrugIntensities` FOREIGN KEY (`DrugId`) REFERENCES `drugs` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `drugsintensities`
--

LOCK TABLES `drugsintensities` WRITE;
/*!40000 ALTER TABLE `drugsintensities` DISABLE KEYS */;
/*!40000 ALTER TABLE `drugsintensities` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `dubs`
--

DROP TABLE IF EXISTS `dubs`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `dubs` (
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
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `dubs`
--

LOCK TABLES `dubs` WRITE;
/*!40000 ALTER TABLE `dubs` DISABLE KEYS */;
/*!40000 ALTER TABLE `dubs` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `economiczonerevenues`
--

DROP TABLE IF EXISTS `economiczonerevenues`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `economiczonerevenues` (
  `EconomicZoneId` bigint NOT NULL,
  `FinancialPeriodId` bigint NOT NULL,
  `TotalTaxRevenue` decimal(58,29) NOT NULL,
  PRIMARY KEY (`EconomicZoneId`,`FinancialPeriodId`),
  KEY `FK_EconomicZoneRevenues_FinancialPeriods_idx` (`FinancialPeriodId`),
  CONSTRAINT `FK_EconomicZoneRevenues` FOREIGN KEY (`EconomicZoneId`) REFERENCES `economiczones` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_EconomicZoneRevenues_FinancialPeriods` FOREIGN KEY (`FinancialPeriodId`) REFERENCES `financialperiods` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `economiczonerevenues`
--

LOCK TABLES `economiczonerevenues` WRITE;
/*!40000 ALTER TABLE `economiczonerevenues` DISABLE KEYS */;
/*!40000 ALTER TABLE `economiczonerevenues` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `economiczones`
--

DROP TABLE IF EXISTS `economiczones`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `economiczones` (
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
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `economiczones`
--

LOCK TABLES `economiczones` WRITE;
/*!40000 ALTER TABLE `economiczones` DISABLE KEYS */;
/*!40000 ALTER TABLE `economiczones` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `economiczoneshoptaxes`
--

DROP TABLE IF EXISTS `economiczoneshoptaxes`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `economiczoneshoptaxes` (
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
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `economiczoneshoptaxes`
--

LOCK TABLES `economiczoneshoptaxes` WRITE;
/*!40000 ALTER TABLE `economiczoneshoptaxes` DISABLE KEYS */;
/*!40000 ALTER TABLE `economiczoneshoptaxes` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `economiczonetaxes`
--

DROP TABLE IF EXISTS `economiczonetaxes`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `economiczonetaxes` (
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
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `economiczonetaxes`
--

LOCK TABLES `economiczonetaxes` WRITE;
/*!40000 ALTER TABLE `economiczonetaxes` DISABLE KEYS */;
/*!40000 ALTER TABLE `economiczonetaxes` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `editableitems`
--

DROP TABLE IF EXISTS `editableitems`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `editableitems` (
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
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `editableitems`
--

LOCK TABLES `editableitems` WRITE;
/*!40000 ALTER TABLE `editableitems` DISABLE KEYS */;
/*!40000 ALTER TABLE `editableitems` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `elections`
--

DROP TABLE IF EXISTS `elections`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `elections` (
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
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `elections`
--

LOCK TABLES `elections` WRITE;
/*!40000 ALTER TABLE `elections` DISABLE KEYS */;
/*!40000 ALTER TABLE `elections` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `electionsnominees`
--

DROP TABLE IF EXISTS `electionsnominees`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `electionsnominees` (
  `ElectionId` bigint NOT NULL,
  `NomineeId` bigint NOT NULL,
  `NomineeClanId` bigint NOT NULL,
  PRIMARY KEY (`ElectionId`,`NomineeId`),
  KEY `FK_ElectionsNominees_Elections_idx` (`ElectionId`),
  KEY `FK_ElectionsNominees_ClanMemberships_idx` (`NomineeClanId`,`NomineeId`),
  CONSTRAINT `FK_ElectionsNominees_ClanMemberships` FOREIGN KEY (`NomineeClanId`, `NomineeId`) REFERENCES `clanmemberships` (`ClanId`, `CharacterId`) ON DELETE CASCADE,
  CONSTRAINT `FK_ElectionsNominees_Elections` FOREIGN KEY (`ElectionId`) REFERENCES `elections` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `electionsnominees`
--

LOCK TABLES `electionsnominees` WRITE;
/*!40000 ALTER TABLE `electionsnominees` DISABLE KEYS */;
/*!40000 ALTER TABLE `electionsnominees` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `electionsvotes`
--

DROP TABLE IF EXISTS `electionsvotes`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `electionsvotes` (
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
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `electionsvotes`
--

LOCK TABLES `electionsvotes` WRITE;
/*!40000 ALTER TABLE `electionsvotes` DISABLE KEYS */;
/*!40000 ALTER TABLE `electionsvotes` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `emailtemplates`
--

DROP TABLE IF EXISTS `emailtemplates`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `emailtemplates` (
  `TemplateType` int NOT NULL,
  `Content` varchar(4000) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `Subject` varchar(4000) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `ReturnAddress` varchar(4000) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  PRIMARY KEY (`TemplateType`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `emailtemplates`
--

LOCK TABLES `emailtemplates` WRITE;
/*!40000 ALTER TABLE `emailtemplates` DISABLE KEYS */;
/*!40000 ALTER TABLE `emailtemplates` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `enforcementauthorities`
--

DROP TABLE IF EXISTS `enforcementauthorities`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `enforcementauthorities` (
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
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `enforcementauthorities`
--

LOCK TABLES `enforcementauthorities` WRITE;
/*!40000 ALTER TABLE `enforcementauthorities` DISABLE KEYS */;
/*!40000 ALTER TABLE `enforcementauthorities` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `enforcementauthorities_accusableclasses`
--

DROP TABLE IF EXISTS `enforcementauthorities_accusableclasses`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `enforcementauthorities_accusableclasses` (
  `EnforcementAuthorityId` bigint NOT NULL,
  `LegalClassId` bigint NOT NULL,
  PRIMARY KEY (`EnforcementAuthorityId`,`LegalClassId`),
  KEY `FK_EnforcementAuthorities_AccusableClasses_LegalClasses_idx` (`LegalClassId`),
  CONSTRAINT `FK_EnforcementAuthorities_AccusableClasses_Enforce` FOREIGN KEY (`EnforcementAuthorityId`) REFERENCES `enforcementauthorities` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_EnforcementAuthorities_AccusableClasses_LegalClasses` FOREIGN KEY (`LegalClassId`) REFERENCES `legalclasses` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `enforcementauthorities_accusableclasses`
--

LOCK TABLES `enforcementauthorities_accusableclasses` WRITE;
/*!40000 ALTER TABLE `enforcementauthorities_accusableclasses` DISABLE KEYS */;
/*!40000 ALTER TABLE `enforcementauthorities_accusableclasses` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `enforcementauthorities_parentauthorities`
--

DROP TABLE IF EXISTS `enforcementauthorities_parentauthorities`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `enforcementauthorities_parentauthorities` (
  `ParentId` bigint NOT NULL,
  `ChildId` bigint NOT NULL,
  PRIMARY KEY (`ParentId`,`ChildId`),
  KEY `FK_EnforcementAuthorities_ParentAuthorities_Child_idx` (`ChildId`),
  CONSTRAINT `FK_EnforcementAuthorities_ParentAuthorities_Child` FOREIGN KEY (`ChildId`) REFERENCES `enforcementauthorities` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_EnforcementAuthorities_ParentAuthorities_Parent` FOREIGN KEY (`ParentId`) REFERENCES `enforcementauthorities` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `enforcementauthorities_parentauthorities`
--

LOCK TABLES `enforcementauthorities_parentauthorities` WRITE;
/*!40000 ALTER TABLE `enforcementauthorities_parentauthorities` DISABLE KEYS */;
/*!40000 ALTER TABLE `enforcementauthorities_parentauthorities` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `enforcementauthoritiesarrestableclasses`
--

DROP TABLE IF EXISTS `enforcementauthoritiesarrestableclasses`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `enforcementauthoritiesarrestableclasses` (
  `EnforcementAuthorityId` bigint NOT NULL,
  `LegalClassId` bigint NOT NULL,
  PRIMARY KEY (`EnforcementAuthorityId`,`LegalClassId`),
  KEY `FK_EnforcementAuthoritiesArrestableLegalClasses_Enforce_idx` (`EnforcementAuthorityId`),
  KEY `FK_EnforcementAuthoritiesArrestableLegalClasses_LegalClasses_idx` (`LegalClassId`),
  CONSTRAINT `FK_EnforcementAuthoritiesArrestableLegalClasses_Enforce` FOREIGN KEY (`EnforcementAuthorityId`) REFERENCES `enforcementauthorities` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_EnforcementAuthoritiesArrestableLegalClasses_LegalClasses` FOREIGN KEY (`LegalClassId`) REFERENCES `legalclasses` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `enforcementauthoritiesarrestableclasses`
--

LOCK TABLES `enforcementauthoritiesarrestableclasses` WRITE;
/*!40000 ALTER TABLE `enforcementauthoritiesarrestableclasses` DISABLE KEYS */;
/*!40000 ALTER TABLE `enforcementauthoritiesarrestableclasses` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `entitydescriptionpatterns`
--

DROP TABLE IF EXISTS `entitydescriptionpatterns`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `entitydescriptionpatterns` (
  `Id` bigint NOT NULL AUTO_INCREMENT,
  `Pattern` varchar(4000) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `Type` int NOT NULL,
  `ApplicabilityProgId` bigint DEFAULT NULL,
  `RelativeWeight` int NOT NULL,
  PRIMARY KEY (`Id`),
  KEY `FK_EntityDescriptionPatterns_FutureProgs` (`ApplicabilityProgId`),
  CONSTRAINT `FK_EntityDescriptionPatterns_FutureProgs` FOREIGN KEY (`ApplicabilityProgId`) REFERENCES `futureprogs` (`Id`) ON DELETE RESTRICT
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `entitydescriptionpatterns`
--

LOCK TABLES `entitydescriptionpatterns` WRITE;
/*!40000 ALTER TABLE `entitydescriptionpatterns` DISABLE KEYS */;
/*!40000 ALTER TABLE `entitydescriptionpatterns` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `entitydescriptionpatterns_entitydescriptions`
--

DROP TABLE IF EXISTS `entitydescriptionpatterns_entitydescriptions`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `entitydescriptionpatterns_entitydescriptions` (
  `PatternId` bigint NOT NULL,
  `EntityDescriptionId` bigint NOT NULL,
  PRIMARY KEY (`PatternId`,`EntityDescriptionId`),
  KEY `FK_EDP_EntityDescriptions_EntityDescriptions` (`EntityDescriptionId`),
  CONSTRAINT `FK_EDP_EntityDescriptions_EntityDescriptionPatterns` FOREIGN KEY (`PatternId`) REFERENCES `entitydescriptionpatterns` (`Id`) ON DELETE RESTRICT,
  CONSTRAINT `FK_EDP_EntityDescriptions_EntityDescriptions` FOREIGN KEY (`EntityDescriptionId`) REFERENCES `entitydescriptions` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `entitydescriptionpatterns_entitydescriptions`
--

LOCK TABLES `entitydescriptionpatterns_entitydescriptions` WRITE;
/*!40000 ALTER TABLE `entitydescriptionpatterns_entitydescriptions` DISABLE KEYS */;
/*!40000 ALTER TABLE `entitydescriptionpatterns_entitydescriptions` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `entitydescriptions`
--

DROP TABLE IF EXISTS `entitydescriptions`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `entitydescriptions` (
  `Id` bigint NOT NULL AUTO_INCREMENT,
  `ShortDescription` varchar(4000) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci DEFAULT NULL,
  `FullDescription` varchar(4000) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci DEFAULT NULL,
  `DisplaySex` smallint NOT NULL,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `entitydescriptions`
--

LOCK TABLES `entitydescriptions` WRITE;
/*!40000 ALTER TABLE `entitydescriptions` DISABLE KEYS */;
/*!40000 ALTER TABLE `entitydescriptions` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `estateassets`
--

DROP TABLE IF EXISTS `estateassets`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `estateassets` (
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
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `estateassets`
--

LOCK TABLES `estateassets` WRITE;
/*!40000 ALTER TABLE `estateassets` DISABLE KEYS */;
/*!40000 ALTER TABLE `estateassets` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `estateclaims`
--

DROP TABLE IF EXISTS `estateclaims`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `estateclaims` (
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
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `estateclaims`
--

LOCK TABLES `estateclaims` WRITE;
/*!40000 ALTER TABLE `estateclaims` DISABLE KEYS */;
/*!40000 ALTER TABLE `estateclaims` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `estatepayouts`
--

DROP TABLE IF EXISTS `estatepayouts`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `estatepayouts` (
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
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `estatepayouts`
--

LOCK TABLES `estatepayouts` WRITE;
/*!40000 ALTER TABLE `estatepayouts` DISABLE KEYS */;
/*!40000 ALTER TABLE `estatepayouts` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `estates`
--

DROP TABLE IF EXISTS `estates`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `estates` (
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
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `estates`
--

LOCK TABLES `estates` WRITE;
/*!40000 ALTER TABLE `estates` DISABLE KEYS */;
/*!40000 ALTER TABLE `estates` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `ethnicities`
--

DROP TABLE IF EXISTS `ethnicities`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `ethnicities` (
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
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `ethnicities`
--

LOCK TABLES `ethnicities` WRITE;
/*!40000 ALTER TABLE `ethnicities` DISABLE KEYS */;
/*!40000 ALTER TABLE `ethnicities` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `ethnicities_characteristics`
--

DROP TABLE IF EXISTS `ethnicities_characteristics`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `ethnicities_characteristics` (
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
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `ethnicities_characteristics`
--

LOCK TABLES `ethnicities_characteristics` WRITE;
/*!40000 ALTER TABLE `ethnicities_characteristics` DISABLE KEYS */;
/*!40000 ALTER TABLE `ethnicities_characteristics` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `ethnicities_chargenresources`
--

DROP TABLE IF EXISTS `ethnicities_chargenresources`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `ethnicities_chargenresources` (
  `EthnicityId` bigint NOT NULL,
  `ChargenResourceId` bigint NOT NULL,
  `RequirementOnly` bit(1) NOT NULL DEFAULT b'0',
  `Amount` int NOT NULL,
  PRIMARY KEY (`EthnicityId`,`ChargenResourceId`),
  KEY `IX_Ethnicities_ChargenResources_ChargenResourceId` (`ChargenResourceId`),
  CONSTRAINT `FK_Ethnicities_ChargenResources_ChargenResources_ChargenResourc~` FOREIGN KEY (`ChargenResourceId`) REFERENCES `chargenresources` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_Ethnicities_ChargenResources_Ethnicities_EthnicityId` FOREIGN KEY (`EthnicityId`) REFERENCES `ethnicities` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `ethnicities_chargenresources`
--

LOCK TABLES `ethnicities_chargenresources` WRITE;
/*!40000 ALTER TABLE `ethnicities_chargenresources` DISABLE KEYS */;
/*!40000 ALTER TABLE `ethnicities_chargenresources` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `ethnicitiesnamecultures`
--

DROP TABLE IF EXISTS `ethnicitiesnamecultures`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `ethnicitiesnamecultures` (
  `EthnicityId` bigint NOT NULL,
  `NameCultureId` bigint NOT NULL,
  `Gender` smallint NOT NULL,
  PRIMARY KEY (`EthnicityId`,`NameCultureId`,`Gender`),
  KEY `IX_EthnicitiesNameCultures_NameCultureId` (`NameCultureId`),
  CONSTRAINT `FK_EthnicitiesNameCultures_Ethnicities` FOREIGN KEY (`EthnicityId`) REFERENCES `ethnicities` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_EthnicitiesNameCultures_NameCultures` FOREIGN KEY (`NameCultureId`) REFERENCES `nameculture` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `ethnicitiesnamecultures`
--

LOCK TABLES `ethnicitiesnamecultures` WRITE;
/*!40000 ALTER TABLE `ethnicitiesnamecultures` DISABLE KEYS */;
/*!40000 ALTER TABLE `ethnicitiesnamecultures` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `exits`
--

DROP TABLE IF EXISTS `exits`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `exits` (
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
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `exits`
--

LOCK TABLES `exits` WRITE;
/*!40000 ALTER TABLE `exits` DISABLE KEYS */;
/*!40000 ALTER TABLE `exits` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `externalclancontrols`
--

DROP TABLE IF EXISTS `externalclancontrols`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `externalclancontrols` (
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
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `externalclancontrols`
--

LOCK TABLES `externalclancontrols` WRITE;
/*!40000 ALTER TABLE `externalclancontrols` DISABLE KEYS */;
/*!40000 ALTER TABLE `externalclancontrols` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `externalclancontrols_appointments`
--

DROP TABLE IF EXISTS `externalclancontrols_appointments`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `externalclancontrols_appointments` (
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
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `externalclancontrols_appointments`
--

LOCK TABLES `externalclancontrols_appointments` WRITE;
/*!40000 ALTER TABLE `externalclancontrols_appointments` DISABLE KEYS */;
/*!40000 ALTER TABLE `externalclancontrols_appointments` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `financialperiods`
--

DROP TABLE IF EXISTS `financialperiods`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `financialperiods` (
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
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `financialperiods`
--

LOCK TABLES `financialperiods` WRITE;
/*!40000 ALTER TABLE `financialperiods` DISABLE KEYS */;
/*!40000 ALTER TABLE `financialperiods` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `foragableprofiles`
--

DROP TABLE IF EXISTS `foragableprofiles`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `foragableprofiles` (
  `Id` bigint NOT NULL,
  `RevisionNumber` int NOT NULL,
  `EditableItemId` bigint NOT NULL,
  `Name` varchar(200) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL,
  PRIMARY KEY (`Id`,`RevisionNumber`),
  KEY `FK_ForagableProfiles_EditableItems_idx` (`EditableItemId`),
  CONSTRAINT `FK_ForagableProfiles_EditableItems` FOREIGN KEY (`EditableItemId`) REFERENCES `editableitems` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `foragableprofiles`
--

LOCK TABLES `foragableprofiles` WRITE;
/*!40000 ALTER TABLE `foragableprofiles` DISABLE KEYS */;
/*!40000 ALTER TABLE `foragableprofiles` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `foragableprofiles_foragables`
--

DROP TABLE IF EXISTS `foragableprofiles_foragables`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `foragableprofiles_foragables` (
  `ForagableProfileId` bigint NOT NULL,
  `ForagableProfileRevisionNumber` int NOT NULL,
  `ForagableId` bigint NOT NULL,
  PRIMARY KEY (`ForagableProfileId`,`ForagableProfileRevisionNumber`,`ForagableId`),
  CONSTRAINT `FK_ForagableProfiles_Foragables_ForagableProfiles` FOREIGN KEY (`ForagableProfileId`, `ForagableProfileRevisionNumber`) REFERENCES `foragableprofiles` (`Id`, `RevisionNumber`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `foragableprofiles_foragables`
--

LOCK TABLES `foragableprofiles_foragables` WRITE;
/*!40000 ALTER TABLE `foragableprofiles_foragables` DISABLE KEYS */;
/*!40000 ALTER TABLE `foragableprofiles_foragables` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `foragableprofiles_hourlyyieldgains`
--

DROP TABLE IF EXISTS `foragableprofiles_hourlyyieldgains`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `foragableprofiles_hourlyyieldgains` (
  `ForagableProfileId` bigint NOT NULL,
  `ForagableProfileRevisionNumber` int NOT NULL,
  `ForageType` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL,
  `Yield` double NOT NULL,
  PRIMARY KEY (`ForagableProfileId`,`ForagableProfileRevisionNumber`,`ForageType`),
  CONSTRAINT `FK_ForagableProfiles_HourlyYieldGains_ForagableProfiles` FOREIGN KEY (`ForagableProfileId`, `ForagableProfileRevisionNumber`) REFERENCES `foragableprofiles` (`Id`, `RevisionNumber`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `foragableprofiles_hourlyyieldgains`
--

LOCK TABLES `foragableprofiles_hourlyyieldgains` WRITE;
/*!40000 ALTER TABLE `foragableprofiles_hourlyyieldgains` DISABLE KEYS */;
/*!40000 ALTER TABLE `foragableprofiles_hourlyyieldgains` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `foragableprofiles_maximumyields`
--

DROP TABLE IF EXISTS `foragableprofiles_maximumyields`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `foragableprofiles_maximumyields` (
  `ForagableProfileId` bigint NOT NULL,
  `ForagableProfileRevisionNumber` int NOT NULL,
  `ForageType` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL,
  `Yield` double NOT NULL,
  PRIMARY KEY (`ForagableProfileId`,`ForagableProfileRevisionNumber`,`ForageType`),
  CONSTRAINT `FK_ForagableProfiles_MaximumYields_ForagableProfiles` FOREIGN KEY (`ForagableProfileId`, `ForagableProfileRevisionNumber`) REFERENCES `foragableprofiles` (`Id`, `RevisionNumber`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `foragableprofiles_maximumyields`
--

LOCK TABLES `foragableprofiles_maximumyields` WRITE;
/*!40000 ALTER TABLE `foragableprofiles_maximumyields` DISABLE KEYS */;
/*!40000 ALTER TABLE `foragableprofiles_maximumyields` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `foragables`
--

DROP TABLE IF EXISTS `foragables`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `foragables` (
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
  PRIMARY KEY (`Id`,`RevisionNumber`),
  KEY `FK_Foragables_EditableItems` (`EditableItemId`),
  CONSTRAINT `FK_Foragables_EditableItems` FOREIGN KEY (`EditableItemId`) REFERENCES `editableitems` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `foragables`
--

LOCK TABLES `foragables` WRITE;
/*!40000 ALTER TABLE `foragables` DISABLE KEYS */;
/*!40000 ALTER TABLE `foragables` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `futureprogs`
--

DROP TABLE IF EXISTS `futureprogs`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `futureprogs` (
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
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `futureprogs`
--

LOCK TABLES `futureprogs` WRITE;
/*!40000 ALTER TABLE `futureprogs` DISABLE KEYS */;
/*!40000 ALTER TABLE `futureprogs` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `futureprogs_parameters`
--

DROP TABLE IF EXISTS `futureprogs_parameters`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `futureprogs_parameters` (
  `FutureProgId` bigint NOT NULL,
  `ParameterIndex` int NOT NULL,
  `ParameterName` varchar(4000) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `ParameterTypeDefinition` varchar(255) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL DEFAULT '',
  PRIMARY KEY (`FutureProgId`,`ParameterIndex`),
  CONSTRAINT `FK_FutureProgs_Parameters_FutureProgs` FOREIGN KEY (`FutureProgId`) REFERENCES `futureprogs` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `futureprogs_parameters`
--

LOCK TABLES `futureprogs_parameters` WRITE;
/*!40000 ALTER TABLE `futureprogs_parameters` DISABLE KEYS */;
/*!40000 ALTER TABLE `futureprogs_parameters` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `gameitemcomponentprotos`
--

DROP TABLE IF EXISTS `gameitemcomponentprotos`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `gameitemcomponentprotos` (
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
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `gameitemcomponentprotos`
--

LOCK TABLES `gameitemcomponentprotos` WRITE;
/*!40000 ALTER TABLE `gameitemcomponentprotos` DISABLE KEYS */;
/*!40000 ALTER TABLE `gameitemcomponentprotos` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `gameitemcomponents`
--

DROP TABLE IF EXISTS `gameitemcomponents`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `gameitemcomponents` (
  `Id` bigint NOT NULL AUTO_INCREMENT,
  `GameItemComponentProtoId` bigint NOT NULL,
  `GameItemComponentProtoRevision` int NOT NULL,
  `Definition` text CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `GameItemId` bigint NOT NULL,
  PRIMARY KEY (`Id`),
  KEY `FK_GameItemComponents_GameItems` (`GameItemId`),
  CONSTRAINT `FK_GameItemComponents_GameItems` FOREIGN KEY (`GameItemId`) REFERENCES `gameitems` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `gameitemcomponents`
--

LOCK TABLES `gameitemcomponents` WRITE;
/*!40000 ALTER TABLE `gameitemcomponents` DISABLE KEYS */;
/*!40000 ALTER TABLE `gameitemcomponents` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `gameitemeditingview`
--

DROP TABLE IF EXISTS `gameitemeditingview`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `gameitemeditingview` (
  `Id` tinyint NOT NULL,
  `Name` tinyint NOT NULL,
  `MaterialId` tinyint NOT NULL,
  `ProtoMaterial` tinyint NOT NULL,
  `Quality` tinyint NOT NULL,
  `Size` tinyint NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `gameitemeditingview`
--

LOCK TABLES `gameitemeditingview` WRITE;
/*!40000 ALTER TABLE `gameitemeditingview` DISABLE KEYS */;
/*!40000 ALTER TABLE `gameitemeditingview` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `gameitemprotoextradescriptions`
--

DROP TABLE IF EXISTS `gameitemprotoextradescriptions`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `gameitemprotoextradescriptions` (
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
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `gameitemprotoextradescriptions`
--

LOCK TABLES `gameitemprotoextradescriptions` WRITE;
/*!40000 ALTER TABLE `gameitemprotoextradescriptions` DISABLE KEYS */;
/*!40000 ALTER TABLE `gameitemprotoextradescriptions` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `gameitemprotos`
--

DROP TABLE IF EXISTS `gameitemprotos`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `gameitemprotos` (
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
  PRIMARY KEY (`Id`,`RevisionNumber`),
  KEY `FK_GameItemProtos_EditableItems` (`EditableItemId`),
  KEY `FK_GameItemProtos_ItemGroups_idx` (`ItemGroupId`),
  CONSTRAINT `FK_GameItemProtos_EditableItems` FOREIGN KEY (`EditableItemId`) REFERENCES `editableitems` (`Id`) ON DELETE RESTRICT,
  CONSTRAINT `FK_GameItemProtos_ItemGroups` FOREIGN KEY (`ItemGroupId`) REFERENCES `itemgroups` (`Id`) ON DELETE SET NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `gameitemprotos`
--

LOCK TABLES `gameitemprotos` WRITE;
/*!40000 ALTER TABLE `gameitemprotos` DISABLE KEYS */;
/*!40000 ALTER TABLE `gameitemprotos` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `gameitemprotos_defaultvariables`
--

DROP TABLE IF EXISTS `gameitemprotos_defaultvariables`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `gameitemprotos_defaultvariables` (
  `GameItemProtoId` bigint NOT NULL,
  `VariableName` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL,
  `GameItemProtoRevNum` int NOT NULL,
  `VariableValue` text CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL,
  PRIMARY KEY (`GameItemProtoId`,`GameItemProtoRevNum`,`VariableName`),
  CONSTRAINT `FK_GameItemProtos_DefaultValues_GameItemProtos` FOREIGN KEY (`GameItemProtoId`, `GameItemProtoRevNum`) REFERENCES `gameitemprotos` (`Id`, `RevisionNumber`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `gameitemprotos_defaultvariables`
--

LOCK TABLES `gameitemprotos_defaultvariables` WRITE;
/*!40000 ALTER TABLE `gameitemprotos_defaultvariables` DISABLE KEYS */;
/*!40000 ALTER TABLE `gameitemprotos_defaultvariables` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `gameitemprotos_gameitemcomponentprotos`
--

DROP TABLE IF EXISTS `gameitemprotos_gameitemcomponentprotos`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `gameitemprotos_gameitemcomponentprotos` (
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
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `gameitemprotos_gameitemcomponentprotos`
--

LOCK TABLES `gameitemprotos_gameitemcomponentprotos` WRITE;
/*!40000 ALTER TABLE `gameitemprotos_gameitemcomponentprotos` DISABLE KEYS */;
/*!40000 ALTER TABLE `gameitemprotos_gameitemcomponentprotos` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `gameitemprotos_onloadprogs`
--

DROP TABLE IF EXISTS `gameitemprotos_onloadprogs`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `gameitemprotos_onloadprogs` (
  `GameItemProtoId` bigint NOT NULL,
  `GameItemProtoRevisionNumber` int NOT NULL,
  `FutureProgId` bigint NOT NULL,
  PRIMARY KEY (`GameItemProtoId`,`GameItemProtoRevisionNumber`,`FutureProgId`),
  KEY `FK_GameItemProtos_OnLoadProgs_FutureProgs_idx` (`FutureProgId`),
  CONSTRAINT `FK_GameItemProtos_OnLoadProgs_FutureProgs` FOREIGN KEY (`FutureProgId`) REFERENCES `futureprogs` (`Id`) ON DELETE RESTRICT,
  CONSTRAINT `FK_GameItemProtos_OnLoadProgs_GameItemProtos` FOREIGN KEY (`GameItemProtoId`, `GameItemProtoRevisionNumber`) REFERENCES `gameitemprotos` (`Id`, `RevisionNumber`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `gameitemprotos_onloadprogs`
--

LOCK TABLES `gameitemprotos_onloadprogs` WRITE;
/*!40000 ALTER TABLE `gameitemprotos_onloadprogs` DISABLE KEYS */;
/*!40000 ALTER TABLE `gameitemprotos_onloadprogs` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `gameitemprotos_tags`
--

DROP TABLE IF EXISTS `gameitemprotos_tags`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `gameitemprotos_tags` (
  `GameItemProtoId` bigint NOT NULL,
  `TagId` bigint NOT NULL,
  `GameItemProtoRevisionNumber` int NOT NULL,
  PRIMARY KEY (`GameItemProtoId`,`TagId`,`GameItemProtoRevisionNumber`),
  KEY `FK_GameItemProtos_Tags_Tags` (`TagId`),
  KEY `FK_GameItemProtos_Tags_GameItemProtos` (`GameItemProtoId`,`GameItemProtoRevisionNumber`),
  CONSTRAINT `FK_GameItemProtos_Tags_GameItemProtos` FOREIGN KEY (`GameItemProtoId`, `GameItemProtoRevisionNumber`) REFERENCES `gameitemprotos` (`Id`, `RevisionNumber`) ON DELETE CASCADE,
  CONSTRAINT `FK_GameItemProtos_Tags_Tags` FOREIGN KEY (`TagId`) REFERENCES `tags` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `gameitemprotos_tags`
--

LOCK TABLES `gameitemprotos_tags` WRITE;
/*!40000 ALTER TABLE `gameitemprotos_tags` DISABLE KEYS */;
/*!40000 ALTER TABLE `gameitemprotos_tags` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `gameitems`
--

DROP TABLE IF EXISTS `gameitems`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `gameitems` (
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
  PRIMARY KEY (`Id`),
  KEY `FK_GameItems_GameItems_Containers_idx` (`ContainerId`),
  CONSTRAINT `FK_GameItems_GameItems_Containers` FOREIGN KEY (`ContainerId`) REFERENCES `gameitems` (`Id`) ON DELETE SET NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `gameitems`
--

LOCK TABLES `gameitems` WRITE;
/*!40000 ALTER TABLE `gameitems` DISABLE KEYS */;
/*!40000 ALTER TABLE `gameitems` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `gameitems_magicresources`
--

DROP TABLE IF EXISTS `gameitems_magicresources`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `gameitems_magicresources` (
  `GameItemId` bigint NOT NULL,
  `MagicResourceId` bigint NOT NULL,
  `Amount` double NOT NULL,
  PRIMARY KEY (`GameItemId`,`MagicResourceId`),
  KEY `FK_GameItems_MagicResources_MagicResources_idx` (`MagicResourceId`),
  CONSTRAINT `FK_GameItems_MagicResources_GameItems` FOREIGN KEY (`GameItemId`) REFERENCES `gameitems` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_GameItems_MagicResources_MagicResources` FOREIGN KEY (`MagicResourceId`) REFERENCES `magicresources` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `gameitems_magicresources`
--

LOCK TABLES `gameitems_magicresources` WRITE;
/*!40000 ALTER TABLE `gameitems_magicresources` DISABLE KEYS */;
/*!40000 ALTER TABLE `gameitems_magicresources` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `gameitemskins`
--

DROP TABLE IF EXISTS `gameitemskins`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `gameitemskins` (
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
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `gameitemskins`
--

LOCK TABLES `gameitemskins` WRITE;
/*!40000 ALTER TABLE `gameitemskins` DISABLE KEYS */;
/*!40000 ALTER TABLE `gameitemskins` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `gases`
--

DROP TABLE IF EXISTS `gases`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `gases` (
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
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `gases`
--

LOCK TABLES `gases` WRITE;
/*!40000 ALTER TABLE `gases` DISABLE KEYS */;
/*!40000 ALTER TABLE `gases` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `gases_tags`
--

DROP TABLE IF EXISTS `gases_tags`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `gases_tags` (
  `GasId` bigint NOT NULL,
  `TagId` bigint NOT NULL,
  PRIMARY KEY (`GasId`,`TagId`),
  KEY `FK_Gases_Tags_Tags_idx` (`TagId`),
  CONSTRAINT `FK_Gases_Tags_Gases` FOREIGN KEY (`GasId`) REFERENCES `gases` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_Gases_Tags_Tags` FOREIGN KEY (`TagId`) REFERENCES `tags` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `gases_tags`
--

LOCK TABLES `gases_tags` WRITE;
/*!40000 ALTER TABLE `gases_tags` DISABLE KEYS */;
/*!40000 ALTER TABLE `gases_tags` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `gptmessages`
--

DROP TABLE IF EXISTS `gptmessages`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `gptmessages` (
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
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `gptmessages`
--

LOCK TABLES `gptmessages` WRITE;
/*!40000 ALTER TABLE `gptmessages` DISABLE KEYS */;
/*!40000 ALTER TABLE `gptmessages` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `gptthreads`
--

DROP TABLE IF EXISTS `gptthreads`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `gptthreads` (
  `Id` bigint NOT NULL AUTO_INCREMENT,
  `Name` varchar(200) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `Prompt` text CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `Model` varchar(200) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `Temperature` double NOT NULL,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `gptthreads`
--

LOCK TABLES `gptthreads` WRITE;
/*!40000 ALTER TABLE `gptthreads` DISABLE KEYS */;
/*!40000 ALTER TABLE `gptthreads` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `grids`
--

DROP TABLE IF EXISTS `grids`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `grids` (
  `Id` bigint NOT NULL AUTO_INCREMENT,
  `GridType` varchar(45) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `Definition` mediumtext CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `grids`
--

LOCK TABLES `grids` WRITE;
/*!40000 ALTER TABLE `grids` DISABLE KEYS */;
/*!40000 ALTER TABLE `grids` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `groupais`
--

DROP TABLE IF EXISTS `groupais`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `groupais` (
  `Id` bigint NOT NULL AUTO_INCREMENT,
  `Name` varchar(200) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `GroupAITemplateId` bigint NOT NULL,
  `Data` mediumtext CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `Definition` mediumtext CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  PRIMARY KEY (`Id`),
  KEY `FK_GroupAIs_GroupAITemplates_idx` (`GroupAITemplateId`),
  CONSTRAINT `FK_GroupAIs_GroupAITemplates` FOREIGN KEY (`GroupAITemplateId`) REFERENCES `groupaitemplates` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `groupais`
--

LOCK TABLES `groupais` WRITE;
/*!40000 ALTER TABLE `groupais` DISABLE KEYS */;
/*!40000 ALTER TABLE `groupais` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `groupaitemplates`
--

DROP TABLE IF EXISTS `groupaitemplates`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `groupaitemplates` (
  `Id` bigint NOT NULL AUTO_INCREMENT,
  `Name` varchar(200) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `Definition` mediumtext CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `groupaitemplates`
--

LOCK TABLES `groupaitemplates` WRITE;
/*!40000 ALTER TABLE `groupaitemplates` DISABLE KEYS */;
/*!40000 ALTER TABLE `groupaitemplates` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `guests`
--

DROP TABLE IF EXISTS `guests`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `guests` (
  `CharacterId` bigint NOT NULL,
  PRIMARY KEY (`CharacterId`),
  CONSTRAINT `FK_Guests_Characters` FOREIGN KEY (`CharacterId`) REFERENCES `characters` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `guests`
--

LOCK TABLES `guests` WRITE;
/*!40000 ALTER TABLE `guests` DISABLE KEYS */;
/*!40000 ALTER TABLE `guests` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `healthstrategies`
--

DROP TABLE IF EXISTS `healthstrategies`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `healthstrategies` (
  `Id` bigint NOT NULL AUTO_INCREMENT,
  `Name` varchar(200) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NOT NULL,
  `Type` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NOT NULL,
  `Definition` text CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NOT NULL,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `healthstrategies`
--

LOCK TABLES `healthstrategies` WRITE;
/*!40000 ALTER TABLE `healthstrategies` DISABLE KEYS */;
/*!40000 ALTER TABLE `healthstrategies` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `hearingprofiles`
--

DROP TABLE IF EXISTS `hearingprofiles`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `hearingprofiles` (
  `Id` bigint NOT NULL AUTO_INCREMENT,
  `Name` varchar(4000) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `Definition` text CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `Type` varchar(4000) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `SurveyDescription` varchar(4000) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `hearingprofiles`
--

LOCK TABLES `hearingprofiles` WRITE;
/*!40000 ALTER TABLE `hearingprofiles` DISABLE KEYS */;
/*!40000 ALTER TABLE `hearingprofiles` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `heightweightmodels`
--

DROP TABLE IF EXISTS `heightweightmodels`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `heightweightmodels` (
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
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `heightweightmodels`
--

LOCK TABLES `heightweightmodels` WRITE;
/*!40000 ALTER TABLE `heightweightmodels` DISABLE KEYS */;
/*!40000 ALTER TABLE `heightweightmodels` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `helpfiles`
--

DROP TABLE IF EXISTS `helpfiles`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `helpfiles` (
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
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `helpfiles`
--

LOCK TABLES `helpfiles` WRITE;
/*!40000 ALTER TABLE `helpfiles` DISABLE KEYS */;
/*!40000 ALTER TABLE `helpfiles` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `helpfiles_extratexts`
--

DROP TABLE IF EXISTS `helpfiles_extratexts`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `helpfiles_extratexts` (
  `HelpfileId` bigint NOT NULL,
  `DisplayOrder` int NOT NULL,
  `Text` text CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `RuleId` bigint NOT NULL,
  PRIMARY KEY (`HelpfileId`,`DisplayOrder`),
  KEY `FK_Helpfiles_ExtraTexts_FutureProgs` (`RuleId`),
  CONSTRAINT `FK_Helpfiles_ExtraTexts_FutureProgs` FOREIGN KEY (`RuleId`) REFERENCES `futureprogs` (`Id`) ON DELETE RESTRICT,
  CONSTRAINT `FK_Helpfiles_ExtraTexts_Helpfiles` FOREIGN KEY (`HelpfileId`) REFERENCES `helpfiles` (`Id`) ON DELETE RESTRICT
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `helpfiles_extratexts`
--

LOCK TABLES `helpfiles_extratexts` WRITE;
/*!40000 ALTER TABLE `helpfiles_extratexts` DISABLE KEYS */;
/*!40000 ALTER TABLE `helpfiles_extratexts` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `hooks`
--

DROP TABLE IF EXISTS `hooks`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `hooks` (
  `Id` bigint NOT NULL AUTO_INCREMENT,
  `Name` varchar(255) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `Definition` text CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NOT NULL,
  `Type` varchar(255) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `Category` varchar(255) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `TargetEventType` int NOT NULL,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `hooks`
--

LOCK TABLES `hooks` WRITE;
/*!40000 ALTER TABLE `hooks` DISABLE KEYS */;
/*!40000 ALTER TABLE `hooks` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `hooks_perceivables`
--

DROP TABLE IF EXISTS `hooks_perceivables`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `hooks_perceivables` (
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
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `hooks_perceivables`
--

LOCK TABLES `hooks_perceivables` WRITE;
/*!40000 ALTER TABLE `hooks_perceivables` DISABLE KEYS */;
/*!40000 ALTER TABLE `hooks_perceivables` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `improvers`
--

DROP TABLE IF EXISTS `improvers`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `improvers` (
  `Id` bigint NOT NULL AUTO_INCREMENT,
  `Name` varchar(255) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `Definition` text CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `Type` varchar(50) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `improvers`
--

LOCK TABLES `improvers` WRITE;
/*!40000 ALTER TABLE `improvers` DISABLE KEYS */;
/*!40000 ALTER TABLE `improvers` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `infections`
--

DROP TABLE IF EXISTS `infections`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `infections` (
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
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `infections`
--

LOCK TABLES `infections` WRITE;
/*!40000 ALTER TABLE `infections` DISABLE KEYS */;
/*!40000 ALTER TABLE `infections` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `itemgroupforms`
--

DROP TABLE IF EXISTS `itemgroupforms`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `itemgroupforms` (
  `Id` bigint NOT NULL AUTO_INCREMENT,
  `ItemGroupId` bigint NOT NULL,
  `Type` varchar(45) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL,
  `Definition` text CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL,
  PRIMARY KEY (`Id`),
  KEY `FK_ItemGroupForms_ItemGroups_idx` (`ItemGroupId`),
  CONSTRAINT `FK_ItemGroupForms_ItemGroups` FOREIGN KEY (`ItemGroupId`) REFERENCES `itemgroups` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `itemgroupforms`
--

LOCK TABLES `itemgroupforms` WRITE;
/*!40000 ALTER TABLE `itemgroupforms` DISABLE KEYS */;
/*!40000 ALTER TABLE `itemgroupforms` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `itemgroups`
--

DROP TABLE IF EXISTS `itemgroups`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `itemgroups` (
  `Id` bigint NOT NULL AUTO_INCREMENT,
  `Name` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci DEFAULT NULL,
  `Keywords` varchar(1000) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci DEFAULT NULL,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `itemgroups`
--

LOCK TABLES `itemgroups` WRITE;
/*!40000 ALTER TABLE `itemgroups` DISABLE KEYS */;
/*!40000 ALTER TABLE `itemgroups` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `jobfindinglocations`
--

DROP TABLE IF EXISTS `jobfindinglocations`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `jobfindinglocations` (
  `EconomicZoneId` bigint NOT NULL,
  `CellId` bigint NOT NULL,
  PRIMARY KEY (`EconomicZoneId`,`CellId`),
  KEY `IX_JobFindingLocations_CellId` (`CellId`),
  CONSTRAINT `FK_JobFindingLocations_Cells` FOREIGN KEY (`CellId`) REFERENCES `cells` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_JobFindingLocations_EconomicZones` FOREIGN KEY (`EconomicZoneId`) REFERENCES `economiczones` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `jobfindinglocations`
--

LOCK TABLES `jobfindinglocations` WRITE;
/*!40000 ALTER TABLE `jobfindinglocations` DISABLE KEYS */;
/*!40000 ALTER TABLE `jobfindinglocations` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `joblistings`
--

DROP TABLE IF EXISTS `joblistings`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `joblistings` (
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
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `joblistings`
--

LOCK TABLES `joblistings` WRITE;
/*!40000 ALTER TABLE `joblistings` DISABLE KEYS */;
/*!40000 ALTER TABLE `joblistings` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `knowledges`
--

DROP TABLE IF EXISTS `knowledges`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `knowledges` (
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
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `knowledges`
--

LOCK TABLES `knowledges` WRITE;
/*!40000 ALTER TABLE `knowledges` DISABLE KEYS */;
/*!40000 ALTER TABLE `knowledges` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `knowledgescosts`
--

DROP TABLE IF EXISTS `knowledgescosts`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `knowledgescosts` (
  `KnowledgeId` bigint NOT NULL,
  `ChargenResourceId` bigint NOT NULL,
  `Cost` int NOT NULL,
  PRIMARY KEY (`KnowledgeId`,`ChargenResourceId`),
  KEY `FK_KnowledgesCosts_ChargenResources_idx` (`ChargenResourceId`),
  KEY `FK_KnowledgesCosts_Knowledges_idx` (`KnowledgeId`),
  CONSTRAINT `FK_KnowledgesCosts_ChargenResources` FOREIGN KEY (`ChargenResourceId`) REFERENCES `chargenresources` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_KnowledgesCosts_Knowledges` FOREIGN KEY (`KnowledgeId`) REFERENCES `knowledges` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `knowledgescosts`
--

LOCK TABLES `knowledgescosts` WRITE;
/*!40000 ALTER TABLE `knowledgescosts` DISABLE KEYS */;
/*!40000 ALTER TABLE `knowledgescosts` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `languagedifficultymodels`
--

DROP TABLE IF EXISTS `languagedifficultymodels`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `languagedifficultymodels` (
  `Id` bigint NOT NULL AUTO_INCREMENT,
  `Definition` longtext CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `Type` varchar(255) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `Name` varchar(255) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `languagedifficultymodels`
--

LOCK TABLES `languagedifficultymodels` WRITE;
/*!40000 ALTER TABLE `languagedifficultymodels` DISABLE KEYS */;
/*!40000 ALTER TABLE `languagedifficultymodels` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `languages`
--

DROP TABLE IF EXISTS `languages`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `languages` (
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
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `languages`
--

LOCK TABLES `languages` WRITE;
/*!40000 ALTER TABLE `languages` DISABLE KEYS */;
/*!40000 ALTER TABLE `languages` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `laws`
--

DROP TABLE IF EXISTS `laws`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `laws` (
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
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `laws`
--

LOCK TABLES `laws` WRITE;
/*!40000 ALTER TABLE `laws` DISABLE KEYS */;
/*!40000 ALTER TABLE `laws` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `laws_offenderclasses`
--

DROP TABLE IF EXISTS `laws_offenderclasses`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `laws_offenderclasses` (
  `LawId` bigint NOT NULL,
  `LegalClassId` bigint NOT NULL,
  PRIMARY KEY (`LawId`,`LegalClassId`),
  KEY `FK_Laws_OffenderClasses_LegalClasses_idx` (`LegalClassId`),
  CONSTRAINT `FK_Laws_OffenderClasses_Laws` FOREIGN KEY (`LawId`) REFERENCES `laws` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_Laws_OffenderClasses_LegalClasses` FOREIGN KEY (`LegalClassId`) REFERENCES `legalclasses` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `laws_offenderclasses`
--

LOCK TABLES `laws_offenderclasses` WRITE;
/*!40000 ALTER TABLE `laws_offenderclasses` DISABLE KEYS */;
/*!40000 ALTER TABLE `laws_offenderclasses` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `laws_victimclasses`
--

DROP TABLE IF EXISTS `laws_victimclasses`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `laws_victimclasses` (
  `LawId` bigint NOT NULL,
  `LegalClassId` bigint NOT NULL,
  PRIMARY KEY (`LawId`,`LegalClassId`),
  KEY `FK_Laws_VictimClasses_LegalClasses_idx` (`LegalClassId`),
  CONSTRAINT `FK_Laws_VictimClasses_Laws` FOREIGN KEY (`LawId`) REFERENCES `laws` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_Laws_VictimClasses_LegalClasses` FOREIGN KEY (`LegalClassId`) REFERENCES `legalclasses` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `laws_victimclasses`
--

LOCK TABLES `laws_victimclasses` WRITE;
/*!40000 ALTER TABLE `laws_victimclasses` DISABLE KEYS */;
/*!40000 ALTER TABLE `laws_victimclasses` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `legalauthorities`
--

DROP TABLE IF EXISTS `legalauthorities`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `legalauthorities` (
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
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `legalauthorities`
--

LOCK TABLES `legalauthorities` WRITE;
/*!40000 ALTER TABLE `legalauthorities` DISABLE KEYS */;
/*!40000 ALTER TABLE `legalauthorities` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `legalauthorities_zones`
--

DROP TABLE IF EXISTS `legalauthorities_zones`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `legalauthorities_zones` (
  `ZoneId` bigint NOT NULL,
  `LegalAuthorityId` bigint NOT NULL,
  PRIMARY KEY (`ZoneId`,`LegalAuthorityId`),
  KEY `FK_LegalAuthorities_Zones_LegalAuthorities_idx` (`LegalAuthorityId`),
  CONSTRAINT `FK_LegalAuthorities_Zones_LegalAuthorities` FOREIGN KEY (`LegalAuthorityId`) REFERENCES `legalauthorities` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_LegalAuthorities_Zones_Zones` FOREIGN KEY (`ZoneId`) REFERENCES `zones` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `legalauthorities_zones`
--

LOCK TABLES `legalauthorities_zones` WRITE;
/*!40000 ALTER TABLE `legalauthorities_zones` DISABLE KEYS */;
/*!40000 ALTER TABLE `legalauthorities_zones` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `legalauthoritiycells`
--

DROP TABLE IF EXISTS `legalauthoritiycells`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `legalauthoritiycells` (
  `LegalAuthorityId` bigint NOT NULL,
  `CellId` bigint NOT NULL,
  PRIMARY KEY (`LegalAuthorityId`,`CellId`),
  KEY `FK_LegalAuthoritiesCells_Cells_idx` (`CellId`),
  KEY `FK_LegalAuthoritiesCells_LegalAuthorities_idx` (`LegalAuthorityId`),
  CONSTRAINT `FK_LegalAuthoritiesCells_Cells` FOREIGN KEY (`CellId`) REFERENCES `cells` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_LegalAuthoritiesCells_LegalAuthorities` FOREIGN KEY (`LegalAuthorityId`) REFERENCES `legalauthorities` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `legalauthoritiycells`
--

LOCK TABLES `legalauthoritiycells` WRITE;
/*!40000 ALTER TABLE `legalauthoritiycells` DISABLE KEYS */;
/*!40000 ALTER TABLE `legalauthoritiycells` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `legalauthorityfines`
--

DROP TABLE IF EXISTS `legalauthorityfines`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `legalauthorityfines` (
  `LegalAuthorityId` bigint NOT NULL,
  `CharacterId` bigint NOT NULL,
  `FinesOwned` decimal(58,29) NOT NULL,
  `PaymentRequiredBy` varchar(100) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  PRIMARY KEY (`LegalAuthorityId`,`CharacterId`),
  KEY `IX_LegalAuthorityFines_CharacterId` (`CharacterId`),
  CONSTRAINT `FK_LegalAuthorityFines_Characters` FOREIGN KEY (`CharacterId`) REFERENCES `characters` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_LegalAuthorityFines_LegalAuthorities` FOREIGN KEY (`LegalAuthorityId`) REFERENCES `legalauthorities` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `legalauthorityfines`
--

LOCK TABLES `legalauthorityfines` WRITE;
/*!40000 ALTER TABLE `legalauthorityfines` DISABLE KEYS */;
/*!40000 ALTER TABLE `legalauthorityfines` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `legalauthorityjailcells`
--

DROP TABLE IF EXISTS `legalauthorityjailcells`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `legalauthorityjailcells` (
  `LegalAuthorityId` bigint NOT NULL,
  `CellId` bigint NOT NULL,
  PRIMARY KEY (`LegalAuthorityId`,`CellId`),
  KEY `FK_LegalAuthoritiesCells_Cells_Jail_idx` (`CellId`),
  KEY `FK_LegalAuthoritiesCells_LegalAuthorities_Jail_idx` (`LegalAuthorityId`),
  CONSTRAINT `FK_LegalAuthoritiesCells_Cells_Jail` FOREIGN KEY (`CellId`) REFERENCES `cells` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_LegalAuthoritiesCells_LegalAuthorities_Jail` FOREIGN KEY (`LegalAuthorityId`) REFERENCES `legalauthorities` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `legalauthorityjailcells`
--

LOCK TABLES `legalauthorityjailcells` WRITE;
/*!40000 ALTER TABLE `legalauthorityjailcells` DISABLE KEYS */;
/*!40000 ALTER TABLE `legalauthorityjailcells` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `legalclasses`
--

DROP TABLE IF EXISTS `legalclasses`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `legalclasses` (
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
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `legalclasses`
--

LOCK TABLES `legalclasses` WRITE;
/*!40000 ALTER TABLE `legalclasses` DISABLE KEYS */;
/*!40000 ALTER TABLE `legalclasses` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `limbs`
--

DROP TABLE IF EXISTS `limbs`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `limbs` (
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
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `limbs`
--

LOCK TABLES `limbs` WRITE;
/*!40000 ALTER TABLE `limbs` DISABLE KEYS */;
/*!40000 ALTER TABLE `limbs` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `limbs_bodypartproto`
--

DROP TABLE IF EXISTS `limbs_bodypartproto`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `limbs_bodypartproto` (
  `BodypartProtoId` bigint NOT NULL,
  `LimbId` bigint NOT NULL,
  PRIMARY KEY (`BodypartProtoId`,`LimbId`),
  KEY `FK_Limbs_BodypartProto_Limbs_idx` (`LimbId`),
  CONSTRAINT `FK_Limbs_BodypartProto_BodypartProto` FOREIGN KEY (`BodypartProtoId`) REFERENCES `bodypartproto` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_Limbs_BodypartProto_Limbs` FOREIGN KEY (`LimbId`) REFERENCES `limbs` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `limbs_bodypartproto`
--

LOCK TABLES `limbs_bodypartproto` WRITE;
/*!40000 ALTER TABLE `limbs_bodypartproto` DISABLE KEYS */;
/*!40000 ALTER TABLE `limbs_bodypartproto` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `limbs_spinalparts`
--

DROP TABLE IF EXISTS `limbs_spinalparts`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `limbs_spinalparts` (
  `LimbId` bigint NOT NULL,
  `BodypartProtoId` bigint NOT NULL,
  PRIMARY KEY (`LimbId`,`BodypartProtoId`),
  KEY `FK_Limbs_SpinalParts_BodypartProtos_idx` (`BodypartProtoId`),
  CONSTRAINT `FK_Limbs_SpinalParts_BodypartProtos` FOREIGN KEY (`BodypartProtoId`) REFERENCES `bodypartproto` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_Limbs_SpinalParts_Limbs` FOREIGN KEY (`LimbId`) REFERENCES `limbs` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `limbs_spinalparts`
--

LOCK TABLES `limbs_spinalparts` WRITE;
/*!40000 ALTER TABLE `limbs_spinalparts` DISABLE KEYS */;
/*!40000 ALTER TABLE `limbs_spinalparts` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `lineofcreditaccounts`
--

DROP TABLE IF EXISTS `lineofcreditaccounts`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `lineofcreditaccounts` (
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
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `lineofcreditaccounts`
--

LOCK TABLES `lineofcreditaccounts` WRITE;
/*!40000 ALTER TABLE `lineofcreditaccounts` DISABLE KEYS */;
/*!40000 ALTER TABLE `lineofcreditaccounts` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `lineofcreditaccountusers`
--

DROP TABLE IF EXISTS `lineofcreditaccountusers`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `lineofcreditaccountusers` (
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
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `lineofcreditaccountusers`
--

LOCK TABLES `lineofcreditaccountusers` WRITE;
/*!40000 ALTER TABLE `lineofcreditaccountusers` DISABLE KEYS */;
/*!40000 ALTER TABLE `lineofcreditaccountusers` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `liquids`
--

DROP TABLE IF EXISTS `liquids`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `liquids` (
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
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `liquids`
--

LOCK TABLES `liquids` WRITE;
/*!40000 ALTER TABLE `liquids` DISABLE KEYS */;
/*!40000 ALTER TABLE `liquids` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `liquids_tags`
--

DROP TABLE IF EXISTS `liquids_tags`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `liquids_tags` (
  `LiquidId` bigint NOT NULL,
  `TagId` bigint NOT NULL,
  PRIMARY KEY (`LiquidId`,`TagId`),
  KEY `FK_Liquids_Tags_Tags_idx` (`TagId`),
  CONSTRAINT `FK_Liquids_Tags_Liquids` FOREIGN KEY (`LiquidId`) REFERENCES `liquids` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_Liquids_Tags_Tags` FOREIGN KEY (`TagId`) REFERENCES `tags` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `liquids_tags`
--

LOCK TABLES `liquids_tags` WRITE;
/*!40000 ALTER TABLE `liquids_tags` DISABLE KEYS */;
/*!40000 ALTER TABLE `liquids_tags` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `locks`
--

DROP TABLE IF EXISTS `locks`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `locks` (
  `Id` bigint NOT NULL AUTO_INCREMENT,
  `Name` varchar(255) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `Style` int NOT NULL,
  `Strength` int NOT NULL,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `locks`
--

LOCK TABLES `locks` WRITE;
/*!40000 ALTER TABLE `locks` DISABLE KEYS */;
/*!40000 ALTER TABLE `locks` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `loginips`
--

DROP TABLE IF EXISTS `loginips`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `loginips` (
  `IpAddress` varchar(200) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `AccountId` bigint NOT NULL,
  `FirstDate` datetime NOT NULL,
  `AccountRegisteredOnThisIP` bit(1) NOT NULL DEFAULT b'0',
  PRIMARY KEY (`IpAddress`,`AccountId`),
  KEY `FK_LoginIPs_Accounts` (`AccountId`),
  CONSTRAINT `FK_LoginIPs_Accounts` FOREIGN KEY (`AccountId`) REFERENCES `accounts` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `loginips`
--

LOCK TABLES `loginips` WRITE;
/*!40000 ALTER TABLE `loginips` DISABLE KEYS */;
/*!40000 ALTER TABLE `loginips` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `magiccapabilities`
--

DROP TABLE IF EXISTS `magiccapabilities`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `magiccapabilities` (
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
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `magiccapabilities`
--

LOCK TABLES `magiccapabilities` WRITE;
/*!40000 ALTER TABLE `magiccapabilities` DISABLE KEYS */;
/*!40000 ALTER TABLE `magiccapabilities` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `magicgenerators`
--

DROP TABLE IF EXISTS `magicgenerators`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `magicgenerators` (
  `Id` bigint NOT NULL AUTO_INCREMENT,
  `Name` varchar(200) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `Type` varchar(45) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `Definition` text CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `magicgenerators`
--

LOCK TABLES `magicgenerators` WRITE;
/*!40000 ALTER TABLE `magicgenerators` DISABLE KEYS */;
/*!40000 ALTER TABLE `magicgenerators` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `magicpowers`
--

DROP TABLE IF EXISTS `magicpowers`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `magicpowers` (
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
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `magicpowers`
--

LOCK TABLES `magicpowers` WRITE;
/*!40000 ALTER TABLE `magicpowers` DISABLE KEYS */;
/*!40000 ALTER TABLE `magicpowers` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `magicresources`
--

DROP TABLE IF EXISTS `magicresources`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `magicresources` (
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
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `magicresources`
--

LOCK TABLES `magicresources` WRITE;
/*!40000 ALTER TABLE `magicresources` DISABLE KEYS */;
/*!40000 ALTER TABLE `magicresources` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `magicschools`
--

DROP TABLE IF EXISTS `magicschools`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `magicschools` (
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
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `magicschools`
--

LOCK TABLES `magicschools` WRITE;
/*!40000 ALTER TABLE `magicschools` DISABLE KEYS */;
/*!40000 ALTER TABLE `magicschools` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `magicspells`
--

DROP TABLE IF EXISTS `magicspells`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `magicspells` (
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
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `magicspells`
--

LOCK TABLES `magicspells` WRITE;
/*!40000 ALTER TABLE `magicspells` DISABLE KEYS */;
/*!40000 ALTER TABLE `magicspells` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `marketcategories`
--

DROP TABLE IF EXISTS `marketcategories`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `marketcategories` (
  `Id` bigint NOT NULL AUTO_INCREMENT,
  `Name` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  `Description` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  `ElasticityFactorAbove` double NOT NULL,
  `ElasticityFactorBelow` double NOT NULL,
  `Tags` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `marketcategories`
--

LOCK TABLES `marketcategories` WRITE;
/*!40000 ALTER TABLE `marketcategories` DISABLE KEYS */;
/*!40000 ALTER TABLE `marketcategories` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `marketinfluences`
--

DROP TABLE IF EXISTS `marketinfluences`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `marketinfluences` (
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
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `marketinfluences`
--

LOCK TABLES `marketinfluences` WRITE;
/*!40000 ALTER TABLE `marketinfluences` DISABLE KEYS */;
/*!40000 ALTER TABLE `marketinfluences` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `marketinfluencetemplates`
--

DROP TABLE IF EXISTS `marketinfluencetemplates`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `marketinfluencetemplates` (
  `Id` bigint NOT NULL AUTO_INCREMENT,
  `Name` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  `Description` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  `CharacterKnowsAboutInfluenceProgId` bigint NOT NULL,
  `Impacts` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  `PopulationImpacts` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  `TemplateSummary` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  PRIMARY KEY (`Id`),
  KEY `IX_MarketInfluenceTemplates_CharacterKnowsAboutInfluenceProgId` (`CharacterKnowsAboutInfluenceProgId`),
  CONSTRAINT `FK_MarketInfluenceTemplates_FutureProgs_CharacterKnowsAboutInfl~` FOREIGN KEY (`CharacterKnowsAboutInfluenceProgId`) REFERENCES `futureprogs` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `marketinfluencetemplates`
--

LOCK TABLES `marketinfluencetemplates` WRITE;
/*!40000 ALTER TABLE `marketinfluencetemplates` DISABLE KEYS */;
/*!40000 ALTER TABLE `marketinfluencetemplates` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `marketmarketcategory`
--

DROP TABLE IF EXISTS `marketmarketcategory`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `marketmarketcategory` (
  `MarketCategoriesId` bigint NOT NULL,
  `MarketsId` bigint NOT NULL,
  PRIMARY KEY (`MarketCategoriesId`,`MarketsId`),
  KEY `IX_MarketMarketCategory_MarketsId` (`MarketsId`),
  CONSTRAINT `FK_MarketMarketCategory_MarketCategories_MarketCategoriesId` FOREIGN KEY (`MarketCategoriesId`) REFERENCES `marketcategories` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_MarketMarketCategory_Markets_MarketsId` FOREIGN KEY (`MarketsId`) REFERENCES `markets` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `marketmarketcategory`
--

LOCK TABLES `marketmarketcategory` WRITE;
/*!40000 ALTER TABLE `marketmarketcategory` DISABLE KEYS */;
/*!40000 ALTER TABLE `marketmarketcategory` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `marketpopulations`
--

DROP TABLE IF EXISTS `marketpopulations`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `marketpopulations` (
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
  PRIMARY KEY (`Id`),
  KEY `IX_MarketPopulations_MarketId` (`MarketId`),
  CONSTRAINT `FK_MarketPopulations_Markets_MarketId` FOREIGN KEY (`MarketId`) REFERENCES `markets` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `marketpopulations`
--

LOCK TABLES `marketpopulations` WRITE;
/*!40000 ALTER TABLE `marketpopulations` DISABLE KEYS */;
/*!40000 ALTER TABLE `marketpopulations` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `markets`
--

DROP TABLE IF EXISTS `markets`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `markets` (
  `Id` bigint NOT NULL AUTO_INCREMENT,
  `Name` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  `Description` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  `EconomicZoneId` bigint NOT NULL,
  `MarketPriceFormula` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  PRIMARY KEY (`Id`),
  KEY `IX_Markets_EconomicZoneId` (`EconomicZoneId`),
  CONSTRAINT `FK_Markets_EconomicZones_EconomicZoneId` FOREIGN KEY (`EconomicZoneId`) REFERENCES `economiczones` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `markets`
--

LOCK TABLES `markets` WRITE;
/*!40000 ALTER TABLE `markets` DISABLE KEYS */;
/*!40000 ALTER TABLE `markets` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `materials`
--

DROP TABLE IF EXISTS `materials`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `materials` (
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
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `materials`
--

LOCK TABLES `materials` WRITE;
/*!40000 ALTER TABLE `materials` DISABLE KEYS */;
/*!40000 ALTER TABLE `materials` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `materials_aliases`
--

DROP TABLE IF EXISTS `materials_aliases`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `materials_aliases` (
  `MaterialId` bigint NOT NULL,
  `Alias` varchar(255) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  PRIMARY KEY (`MaterialId`,`Alias`),
  UNIQUE KEY `Materials_Aliases_Alias_UNIQUE` (`Alias`),
  CONSTRAINT `Materials_Aliases_Materials` FOREIGN KEY (`MaterialId`) REFERENCES `materials` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `materials_aliases`
--

LOCK TABLES `materials_aliases` WRITE;
/*!40000 ALTER TABLE `materials_aliases` DISABLE KEYS */;
/*!40000 ALTER TABLE `materials_aliases` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `materials_tags`
--

DROP TABLE IF EXISTS `materials_tags`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `materials_tags` (
  `MaterialId` bigint NOT NULL,
  `TagId` bigint NOT NULL,
  PRIMARY KEY (`MaterialId`,`TagId`),
  KEY `Materials_Tags_Tags_idx` (`TagId`),
  CONSTRAINT `Materials_Tags_Materials` FOREIGN KEY (`MaterialId`) REFERENCES `materials` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `Materials_Tags_Tags` FOREIGN KEY (`TagId`) REFERENCES `tags` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `materials_tags`
--

LOCK TABLES `materials_tags` WRITE;
/*!40000 ALTER TABLE `materials_tags` DISABLE KEYS */;
/*!40000 ALTER TABLE `materials_tags` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `merchandises`
--

DROP TABLE IF EXISTS `merchandises`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `merchandises` (
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
  PRIMARY KEY (`Id`),
  KEY `FK_Merchandises_GameItems_idx` (`PreferredDisplayContainerId`),
  KEY `FK_Merchandises_Shops_idx` (`ShopId`),
  CONSTRAINT `FK_Merchandises_GameItems` FOREIGN KEY (`PreferredDisplayContainerId`) REFERENCES `gameitems` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_Merchandises_Shops` FOREIGN KEY (`ShopId`) REFERENCES `shops` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `merchandises`
--

LOCK TABLES `merchandises` WRITE;
/*!40000 ALTER TABLE `merchandises` DISABLE KEYS */;
/*!40000 ALTER TABLE `merchandises` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `merits`
--

DROP TABLE IF EXISTS `merits`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `merits` (
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
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `merits`
--

LOCK TABLES `merits` WRITE;
/*!40000 ALTER TABLE `merits` DISABLE KEYS */;
/*!40000 ALTER TABLE `merits` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `merits_chargenresources`
--

DROP TABLE IF EXISTS `merits_chargenresources`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `merits_chargenresources` (
  `MeritId` bigint NOT NULL,
  `ChargenResourceId` bigint NOT NULL,
  `RequirementOnly` bit(1) NOT NULL DEFAULT b'0',
  `Amount` int NOT NULL,
  PRIMARY KEY (`MeritId`,`ChargenResourceId`,`RequirementOnly`),
  KEY `FK_Merits_ChargenResources_ChargenResources_idx` (`ChargenResourceId`),
  CONSTRAINT `FK_Merits_ChargenResources_ChargenResources` FOREIGN KEY (`ChargenResourceId`) REFERENCES `chargenresources` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_Merits_ChargenResources_Merits` FOREIGN KEY (`MeritId`) REFERENCES `merits` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `merits_chargenresources`
--

LOCK TABLES `merits_chargenresources` WRITE;
/*!40000 ALTER TABLE `merits_chargenresources` DISABLE KEYS */;
/*!40000 ALTER TABLE `merits_chargenresources` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `movespeeds`
--

DROP TABLE IF EXISTS `movespeeds`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `movespeeds` (
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
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `movespeeds`
--

LOCK TABLES `movespeeds` WRITE;
/*!40000 ALTER TABLE `movespeeds` DISABLE KEYS */;
/*!40000 ALTER TABLE `movespeeds` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `mutualintelligabilities`
--

DROP TABLE IF EXISTS `mutualintelligabilities`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `mutualintelligabilities` (
  `ListenerLanguageId` bigint NOT NULL,
  `TargetLanguageId` bigint NOT NULL,
  `IntelligabilityDifficulty` int NOT NULL,
  PRIMARY KEY (`ListenerLanguageId`,`TargetLanguageId`),
  KEY `FK_Languages_MutualIntelligabilities_Target_idx` (`TargetLanguageId`),
  CONSTRAINT `FK_Languages_MutualIntelligabilities_Listener` FOREIGN KEY (`ListenerLanguageId`) REFERENCES `languages` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_Languages_MutualIntelligabilities_Target` FOREIGN KEY (`TargetLanguageId`) REFERENCES `languages` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `mutualintelligabilities`
--

LOCK TABLES `mutualintelligabilities` WRITE;
/*!40000 ALTER TABLE `mutualintelligabilities` DISABLE KEYS */;
/*!40000 ALTER TABLE `mutualintelligabilities` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `nameculture`
--

DROP TABLE IF EXISTS `nameculture`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `nameculture` (
  `Id` bigint NOT NULL AUTO_INCREMENT,
  `Name` varchar(255) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `Definition` text CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `nameculture`
--

LOCK TABLES `nameculture` WRITE;
/*!40000 ALTER TABLE `nameculture` DISABLE KEYS */;
/*!40000 ALTER TABLE `nameculture` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `newplayerhints`
--

DROP TABLE IF EXISTS `newplayerhints`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `newplayerhints` (
  `Id` bigint NOT NULL AUTO_INCREMENT,
  `Text` text CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `FilterProgId` bigint DEFAULT NULL,
  `Priority` int NOT NULL,
  `CanRepeat` bit(1) NOT NULL,
  PRIMARY KEY (`Id`),
  KEY `IX_NewPlayerHints_FilterProgId` (`FilterProgId`),
  CONSTRAINT `FK_NewPlayerHints_FutureProgs` FOREIGN KEY (`FilterProgId`) REFERENCES `futureprogs` (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `newplayerhints`
--

LOCK TABLES `newplayerhints` WRITE;
/*!40000 ALTER TABLE `newplayerhints` DISABLE KEYS */;
/*!40000 ALTER TABLE `newplayerhints` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `noncardinalexittemplates`
--

DROP TABLE IF EXISTS `noncardinalexittemplates`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `noncardinalexittemplates` (
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
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `noncardinalexittemplates`
--

LOCK TABLES `noncardinalexittemplates` WRITE;
/*!40000 ALTER TABLE `noncardinalexittemplates` DISABLE KEYS */;
/*!40000 ALTER TABLE `noncardinalexittemplates` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `npcs`
--

DROP TABLE IF EXISTS `npcs`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `npcs` (
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
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `npcs`
--

LOCK TABLES `npcs` WRITE;
/*!40000 ALTER TABLE `npcs` DISABLE KEYS */;
/*!40000 ALTER TABLE `npcs` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `npcs_artificialintelligences`
--

DROP TABLE IF EXISTS `npcs_artificialintelligences`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `npcs_artificialintelligences` (
  `NPCId` bigint NOT NULL,
  `ArtificialIntelligenceId` bigint NOT NULL,
  PRIMARY KEY (`ArtificialIntelligenceId`,`NPCId`),
  KEY `FK_NPCs_ArtificialIntelligences_NPCs` (`NPCId`),
  CONSTRAINT `FK_NPCs_ArtificialIntelligences_ArtificialIntelligences` FOREIGN KEY (`ArtificialIntelligenceId`) REFERENCES `artificialintelligences` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_NPCs_ArtificialIntelligences_NPCs` FOREIGN KEY (`NPCId`) REFERENCES `npcs` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `npcs_artificialintelligences`
--

LOCK TABLES `npcs_artificialintelligences` WRITE;
/*!40000 ALTER TABLE `npcs_artificialintelligences` DISABLE KEYS */;
/*!40000 ALTER TABLE `npcs_artificialintelligences` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `npcspawnercells`
--

DROP TABLE IF EXISTS `npcspawnercells`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `npcspawnercells` (
  `NPCSpawnerId` bigint NOT NULL,
  `CellId` bigint NOT NULL,
  PRIMARY KEY (`NPCSpawnerId`,`CellId`),
  KEY `IX_NPCSpawnerCells_CellId` (`CellId`),
  CONSTRAINT `FK_NPCSpawnerCells_Cell` FOREIGN KEY (`CellId`) REFERENCES `cells` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_NPCSpawnerCells_NPCSpawner` FOREIGN KEY (`NPCSpawnerId`) REFERENCES `npcspawners` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `npcspawnercells`
--

LOCK TABLES `npcspawnercells` WRITE;
/*!40000 ALTER TABLE `npcspawnercells` DISABLE KEYS */;
/*!40000 ALTER TABLE `npcspawnercells` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `npcspawners`
--

DROP TABLE IF EXISTS `npcspawners`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `npcspawners` (
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
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `npcspawners`
--

LOCK TABLES `npcspawners` WRITE;
/*!40000 ALTER TABLE `npcspawners` DISABLE KEYS */;
/*!40000 ALTER TABLE `npcspawners` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `npcspawnerzones`
--

DROP TABLE IF EXISTS `npcspawnerzones`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `npcspawnerzones` (
  `NPCSpawnerId` bigint NOT NULL,
  `ZoneId` bigint NOT NULL,
  PRIMARY KEY (`NPCSpawnerId`,`ZoneId`),
  KEY `IX_NPCSpawnerZones_ZoneId` (`ZoneId`),
  CONSTRAINT `FK_NPCSpawnerZones_NPCSpawner` FOREIGN KEY (`NPCSpawnerId`) REFERENCES `npcspawners` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_NPCSpawnerZones_Zone` FOREIGN KEY (`ZoneId`) REFERENCES `zones` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `npcspawnerzones`
--

LOCK TABLES `npcspawnerzones` WRITE;
/*!40000 ALTER TABLE `npcspawnerzones` DISABLE KEYS */;
/*!40000 ALTER TABLE `npcspawnerzones` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `npctemplates`
--

DROP TABLE IF EXISTS `npctemplates`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `npctemplates` (
  `Id` bigint NOT NULL,
  `RevisionNumber` int NOT NULL,
  `Type` varchar(255) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `Name` varchar(255) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `Definition` text CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `EditableItemId` bigint NOT NULL,
  PRIMARY KEY (`Id`,`RevisionNumber`),
  KEY `FK_NPCTemplates_EditableItems` (`EditableItemId`),
  CONSTRAINT `FK_NPCTemplates_EditableItems` FOREIGN KEY (`EditableItemId`) REFERENCES `editableitems` (`Id`) ON DELETE RESTRICT
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `npctemplates`
--

LOCK TABLES `npctemplates` WRITE;
/*!40000 ALTER TABLE `npctemplates` DISABLE KEYS */;
/*!40000 ALTER TABLE `npctemplates` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `npctemplates_artificalintelligences`
--

DROP TABLE IF EXISTS `npctemplates_artificalintelligences`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `npctemplates_artificalintelligences` (
  `NPCTemplateId` bigint NOT NULL,
  `AIId` bigint NOT NULL,
  `NPCTemplateRevisionNumber` int NOT NULL,
  PRIMARY KEY (`NPCTemplateRevisionNumber`,`NPCTemplateId`,`AIId`),
  KEY `FK_NTAI_ArtificalIntelligences` (`AIId`),
  KEY `FK_NTAI_NPCTemplates` (`NPCTemplateId`,`NPCTemplateRevisionNumber`),
  CONSTRAINT `FK_NTAI_ArtificalIntelligences` FOREIGN KEY (`AIId`) REFERENCES `artificialintelligences` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_NTAI_NPCTemplates` FOREIGN KEY (`NPCTemplateId`, `NPCTemplateRevisionNumber`) REFERENCES `npctemplates` (`Id`, `RevisionNumber`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `npctemplates_artificalintelligences`
--

LOCK TABLES `npctemplates_artificalintelligences` WRITE;
/*!40000 ALTER TABLE `npctemplates_artificalintelligences` DISABLE KEYS */;
/*!40000 ALTER TABLE `npctemplates_artificalintelligences` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `patrolmembers`
--

DROP TABLE IF EXISTS `patrolmembers`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `patrolmembers` (
  `PatrolId` bigint NOT NULL,
  `CharacterId` bigint NOT NULL,
  PRIMARY KEY (`PatrolId`,`CharacterId`),
  KEY `IX_PatrolMembers_CharacterId` (`CharacterId`),
  CONSTRAINT `FK_PatrolMembers_Characters` FOREIGN KEY (`CharacterId`) REFERENCES `characters` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_PatrolsMembers_Patrols` FOREIGN KEY (`PatrolId`) REFERENCES `patrols` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `patrolmembers`
--

LOCK TABLES `patrolmembers` WRITE;
/*!40000 ALTER TABLE `patrolmembers` DISABLE KEYS */;
/*!40000 ALTER TABLE `patrolmembers` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `patrolroutes`
--

DROP TABLE IF EXISTS `patrolroutes`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `patrolroutes` (
  `Id` bigint NOT NULL AUTO_INCREMENT,
  `Name` varchar(100) DEFAULT NULL,
  `LegalAuthorityId` bigint NOT NULL,
  `LingerTimeMajorNode` double NOT NULL,
  `LingerTimeMinorNode` double NOT NULL,
  `Priority` int NOT NULL,
  `PatrolStrategy` varchar(100) DEFAULT NULL,
  `StartPatrolProgId` bigint DEFAULT NULL,
  `IsReady` bit(1) NOT NULL DEFAULT b'0',
  PRIMARY KEY (`Id`),
  KEY `FK_PatrolRoutes_LegalAuthorities_idx` (`LegalAuthorityId`),
  KEY `IX_PatrolRoutes_StartPatrolProgId` (`StartPatrolProgId`),
  CONSTRAINT `FK_PatrolRoutes_FutureProgs_StartPatrolProgId` FOREIGN KEY (`StartPatrolProgId`) REFERENCES `futureprogs` (`Id`) ON DELETE RESTRICT,
  CONSTRAINT `FK_PatrolRoutes_LegalAuthorities` FOREIGN KEY (`LegalAuthorityId`) REFERENCES `legalauthorities` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `patrolroutes`
--

LOCK TABLES `patrolroutes` WRITE;
/*!40000 ALTER TABLE `patrolroutes` DISABLE KEYS */;
/*!40000 ALTER TABLE `patrolroutes` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `patrolroutesnodes`
--

DROP TABLE IF EXISTS `patrolroutesnodes`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `patrolroutesnodes` (
  `PatrolRouteId` bigint NOT NULL,
  `CellId` bigint NOT NULL,
  `Order` int NOT NULL,
  PRIMARY KEY (`PatrolRouteId`,`CellId`),
  KEY `FK_PatrolRoutesNodes_Cells_idx` (`CellId`),
  KEY `FK_PatrolRoutesNodes_PatrolRoutes_idx` (`PatrolRouteId`),
  CONSTRAINT `FK_PatrolRoutesNodes_Cells` FOREIGN KEY (`CellId`) REFERENCES `cells` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_PatrolRoutesNodes_PatrolRoutes` FOREIGN KEY (`PatrolRouteId`) REFERENCES `patrolroutes` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `patrolroutesnodes`
--

LOCK TABLES `patrolroutesnodes` WRITE;
/*!40000 ALTER TABLE `patrolroutesnodes` DISABLE KEYS */;
/*!40000 ALTER TABLE `patrolroutesnodes` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `patrolroutesnumbers`
--

DROP TABLE IF EXISTS `patrolroutesnumbers`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `patrolroutesnumbers` (
  `PatrolRouteId` bigint NOT NULL,
  `EnforcementAuthorityId` bigint NOT NULL,
  `NumberRequired` int NOT NULL,
  PRIMARY KEY (`PatrolRouteId`,`EnforcementAuthorityId`),
  KEY `FK_PatrolRoutesNumbers_EnforcementAuthorities_idx` (`EnforcementAuthorityId`),
  KEY `FK_PatrolRoutesNumbers_PatrolRoutes_idx` (`PatrolRouteId`),
  CONSTRAINT `FK_PatrolRoutesNumbers_EnforcementAuthorities` FOREIGN KEY (`EnforcementAuthorityId`) REFERENCES `enforcementauthorities` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_PatrolRoutesNumbers_PatrolRoutes` FOREIGN KEY (`PatrolRouteId`) REFERENCES `patrolroutes` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `patrolroutesnumbers`
--

LOCK TABLES `patrolroutesnumbers` WRITE;
/*!40000 ALTER TABLE `patrolroutesnumbers` DISABLE KEYS */;
/*!40000 ALTER TABLE `patrolroutesnumbers` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `patrolroutestimesofday`
--

DROP TABLE IF EXISTS `patrolroutestimesofday`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `patrolroutestimesofday` (
  `PatrolRouteId` bigint NOT NULL,
  `TimeOfDay` int NOT NULL,
  PRIMARY KEY (`PatrolRouteId`,`TimeOfDay`),
  KEY `FK_PatrolRoutesTimesOfDay_PatrolRoutes_idx` (`PatrolRouteId`),
  CONSTRAINT `FK_PatrolRoutesTimesOfDay_PatrolRoutes` FOREIGN KEY (`PatrolRouteId`) REFERENCES `patrolroutes` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `patrolroutestimesofday`
--

LOCK TABLES `patrolroutestimesofday` WRITE;
/*!40000 ALTER TABLE `patrolroutestimesofday` DISABLE KEYS */;
/*!40000 ALTER TABLE `patrolroutestimesofday` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `patrols`
--

DROP TABLE IF EXISTS `patrols`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `patrols` (
  `Id` bigint NOT NULL AUTO_INCREMENT,
  `PatrolRouteId` bigint NOT NULL,
  `LegalAuthorityId` bigint NOT NULL,
  `PatrolPhase` int NOT NULL,
  `LastMajorNodeId` bigint DEFAULT NULL,
  `NextMajorNodeId` bigint DEFAULT NULL,
  `PatrolLeaderId` bigint DEFAULT NULL,
  `CharacterId` bigint DEFAULT NULL,
  PRIMARY KEY (`Id`),
  KEY `FK_Patrols_Characters_idx` (`PatrolLeaderId`),
  KEY `FK_Patrols_LastMajorNode_idx` (`LastMajorNodeId`),
  KEY `FK_Patrols_LegalAuthorities_idx` (`LegalAuthorityId`),
  KEY `FK_Patrols_NextMajorNode_idx` (`NextMajorNodeId`),
  KEY `FK_Patrols_PatrolRoutes_idx` (`PatrolRouteId`),
  KEY `IX_Patrols_CharacterId` (`CharacterId`),
  CONSTRAINT `FK_Patrols_Characters` FOREIGN KEY (`PatrolLeaderId`) REFERENCES `characters` (`Id`) ON DELETE RESTRICT,
  CONSTRAINT `FK_Patrols_Characters_CharacterId` FOREIGN KEY (`CharacterId`) REFERENCES `characters` (`Id`) ON DELETE RESTRICT,
  CONSTRAINT `FK_Patrols_LastMajorNode` FOREIGN KEY (`LastMajorNodeId`) REFERENCES `cells` (`Id`) ON DELETE RESTRICT,
  CONSTRAINT `FK_Patrols_LegalAuthorities` FOREIGN KEY (`LegalAuthorityId`) REFERENCES `legalauthorities` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_Patrols_NextMajorNode` FOREIGN KEY (`NextMajorNodeId`) REFERENCES `cells` (`Id`) ON DELETE RESTRICT,
  CONSTRAINT `FK_Patrols_PatrolRoutes` FOREIGN KEY (`PatrolRouteId`) REFERENCES `patrolroutes` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `patrols`
--

LOCK TABLES `patrols` WRITE;
/*!40000 ALTER TABLE `patrols` DISABLE KEYS */;
/*!40000 ALTER TABLE `patrols` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `paygrades`
--

DROP TABLE IF EXISTS `paygrades`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `paygrades` (
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
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `paygrades`
--

LOCK TABLES `paygrades` WRITE;
/*!40000 ALTER TABLE `paygrades` DISABLE KEYS */;
/*!40000 ALTER TABLE `paygrades` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `perceivermerits`
--

DROP TABLE IF EXISTS `perceivermerits`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `perceivermerits` (
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
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `perceivermerits`
--

LOCK TABLES `perceivermerits` WRITE;
/*!40000 ALTER TABLE `perceivermerits` DISABLE KEYS */;
/*!40000 ALTER TABLE `perceivermerits` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `playeractivitysnapshots`
--

DROP TABLE IF EXISTS `playeractivitysnapshots`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `playeractivitysnapshots` (
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
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `playeractivitysnapshots`
--

LOCK TABLES `playeractivitysnapshots` WRITE;
/*!40000 ALTER TABLE `playeractivitysnapshots` DISABLE KEYS */;
/*!40000 ALTER TABLE `playeractivitysnapshots` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `populationbloodmodels`
--

DROP TABLE IF EXISTS `populationbloodmodels`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `populationbloodmodels` (
  `Id` bigint NOT NULL AUTO_INCREMENT,
  `Name` varchar(200) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `populationbloodmodels`
--

LOCK TABLES `populationbloodmodels` WRITE;
/*!40000 ALTER TABLE `populationbloodmodels` DISABLE KEYS */;
/*!40000 ALTER TABLE `populationbloodmodels` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `populationbloodmodels_bloodtypes`
--

DROP TABLE IF EXISTS `populationbloodmodels_bloodtypes`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `populationbloodmodels_bloodtypes` (
  `BloodtypeId` bigint NOT NULL,
  `PopulationBloodModelId` bigint NOT NULL,
  `Weight` double NOT NULL DEFAULT '1',
  PRIMARY KEY (`BloodtypeId`,`PopulationBloodModelId`),
  KEY `FK_PopulationBloodModels_Bloodtypes_PopulationBloodModels_idx` (`PopulationBloodModelId`),
  CONSTRAINT `FK_PopulationBloodModels_Bloodtypes_Bloodtypes` FOREIGN KEY (`BloodtypeId`) REFERENCES `bloodtypes` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_PopulationBloodModels_Bloodtypes_PopulationBloodModels` FOREIGN KEY (`PopulationBloodModelId`) REFERENCES `populationbloodmodels` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `populationbloodmodels_bloodtypes`
--

LOCK TABLES `populationbloodmodels_bloodtypes` WRITE;
/*!40000 ALTER TABLE `populationbloodmodels_bloodtypes` DISABLE KEYS */;
/*!40000 ALTER TABLE `populationbloodmodels_bloodtypes` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `probatelocations`
--

DROP TABLE IF EXISTS `probatelocations`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `probatelocations` (
  `EconomicZoneId` bigint NOT NULL,
  `CellId` bigint NOT NULL,
  PRIMARY KEY (`EconomicZoneId`,`CellId`),
  KEY `IX_ProbateLocations_CellId` (`CellId`),
  CONSTRAINT `FK_ProbateLocations_Cells` FOREIGN KEY (`CellId`) REFERENCES `cells` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_ProbateLocations_EconomicZones` FOREIGN KEY (`EconomicZoneId`) REFERENCES `economiczones` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `probatelocations`
--

LOCK TABLES `probatelocations` WRITE;
/*!40000 ALTER TABLE `probatelocations` DISABLE KEYS */;
/*!40000 ALTER TABLE `probatelocations` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `progschedules`
--

DROP TABLE IF EXISTS `progschedules`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `progschedules` (
  `Id` bigint NOT NULL AUTO_INCREMENT,
  `Name` varchar(200) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL,
  `IntervalType` int NOT NULL,
  `IntervalModifier` int NOT NULL,
  `IntervalOther` int NOT NULL,
  `ReferenceTime` varchar(500) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL,
  `ReferenceDate` varchar(500) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL,
  `FutureProgId` bigint NOT NULL,
  PRIMARY KEY (`Id`),
  KEY `FK_ProgSchedules_FutureProgs_idx` (`FutureProgId`),
  CONSTRAINT `FK_ProgSchedules_FutureProgs` FOREIGN KEY (`FutureProgId`) REFERENCES `futureprogs` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `progschedules`
--

LOCK TABLES `progschedules` WRITE;
/*!40000 ALTER TABLE `progschedules` DISABLE KEYS */;
/*!40000 ALTER TABLE `progschedules` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `projectactions`
--

DROP TABLE IF EXISTS `projectactions`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `projectactions` (
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
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `projectactions`
--

LOCK TABLES `projectactions` WRITE;
/*!40000 ALTER TABLE `projectactions` DISABLE KEYS */;
/*!40000 ALTER TABLE `projectactions` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `projectlabourimpacts`
--

DROP TABLE IF EXISTS `projectlabourimpacts`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `projectlabourimpacts` (
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
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `projectlabourimpacts`
--

LOCK TABLES `projectlabourimpacts` WRITE;
/*!40000 ALTER TABLE `projectlabourimpacts` DISABLE KEYS */;
/*!40000 ALTER TABLE `projectlabourimpacts` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `projectlabourrequirements`
--

DROP TABLE IF EXISTS `projectlabourrequirements`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `projectlabourrequirements` (
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
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `projectlabourrequirements`
--

LOCK TABLES `projectlabourrequirements` WRITE;
/*!40000 ALTER TABLE `projectlabourrequirements` DISABLE KEYS */;
/*!40000 ALTER TABLE `projectlabourrequirements` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `projectmaterialrequirements`
--

DROP TABLE IF EXISTS `projectmaterialrequirements`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `projectmaterialrequirements` (
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
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `projectmaterialrequirements`
--

LOCK TABLES `projectmaterialrequirements` WRITE;
/*!40000 ALTER TABLE `projectmaterialrequirements` DISABLE KEYS */;
/*!40000 ALTER TABLE `projectmaterialrequirements` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `projectphases`
--

DROP TABLE IF EXISTS `projectphases`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `projectphases` (
  `Id` bigint NOT NULL AUTO_INCREMENT,
  `ProjectId` bigint NOT NULL,
  `ProjectRevisionNumber` int NOT NULL,
  `PhaseNumber` int NOT NULL,
  `Description` text CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  PRIMARY KEY (`Id`),
  KEY `FK_ProjectPhases_Projects_idx` (`ProjectId`,`ProjectRevisionNumber`),
  CONSTRAINT `FK_ProjectPhases_Projects` FOREIGN KEY (`ProjectId`, `ProjectRevisionNumber`) REFERENCES `projects` (`Id`, `RevisionNumber`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `projectphases`
--

LOCK TABLES `projectphases` WRITE;
/*!40000 ALTER TABLE `projectphases` DISABLE KEYS */;
/*!40000 ALTER TABLE `projectphases` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `projects`
--

DROP TABLE IF EXISTS `projects`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `projects` (
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
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `projects`
--

LOCK TABLES `projects` WRITE;
/*!40000 ALTER TABLE `projects` DISABLE KEYS */;
/*!40000 ALTER TABLE `projects` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `properties`
--

DROP TABLE IF EXISTS `properties`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `properties` (
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
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `properties`
--

LOCK TABLES `properties` WRITE;
/*!40000 ALTER TABLE `properties` DISABLE KEYS */;
/*!40000 ALTER TABLE `properties` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `propertykeys`
--

DROP TABLE IF EXISTS `propertykeys`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `propertykeys` (
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
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `propertykeys`
--

LOCK TABLES `propertykeys` WRITE;
/*!40000 ALTER TABLE `propertykeys` DISABLE KEYS */;
/*!40000 ALTER TABLE `propertykeys` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `propertyleaseorders`
--

DROP TABLE IF EXISTS `propertyleaseorders`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `propertyleaseorders` (
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
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `propertyleaseorders`
--

LOCK TABLES `propertyleaseorders` WRITE;
/*!40000 ALTER TABLE `propertyleaseorders` DISABLE KEYS */;
/*!40000 ALTER TABLE `propertyleaseorders` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `propertyleases`
--

DROP TABLE IF EXISTS `propertyleases`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `propertyleases` (
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
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `propertyleases`
--

LOCK TABLES `propertyleases` WRITE;
/*!40000 ALTER TABLE `propertyleases` DISABLE KEYS */;
/*!40000 ALTER TABLE `propertyleases` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `propertylocations`
--

DROP TABLE IF EXISTS `propertylocations`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `propertylocations` (
  `PropertyId` bigint NOT NULL,
  `CellId` bigint NOT NULL,
  PRIMARY KEY (`PropertyId`,`CellId`),
  KEY `IX_PropertyLocations_CellId` (`CellId`),
  CONSTRAINT `FK_PropertyLocations_Cell` FOREIGN KEY (`CellId`) REFERENCES `cells` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_PropertyLocations_Property` FOREIGN KEY (`PropertyId`) REFERENCES `properties` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `propertylocations`
--

LOCK TABLES `propertylocations` WRITE;
/*!40000 ALTER TABLE `propertylocations` DISABLE KEYS */;
/*!40000 ALTER TABLE `propertylocations` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `propertyowners`
--

DROP TABLE IF EXISTS `propertyowners`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `propertyowners` (
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
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `propertyowners`
--

LOCK TABLES `propertyowners` WRITE;
/*!40000 ALTER TABLE `propertyowners` DISABLE KEYS */;
/*!40000 ALTER TABLE `propertyowners` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `propertysalesorders`
--

DROP TABLE IF EXISTS `propertysalesorders`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `propertysalesorders` (
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
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `propertysalesorders`
--

LOCK TABLES `propertysalesorders` WRITE;
/*!40000 ALTER TABLE `propertysalesorders` DISABLE KEYS */;
/*!40000 ALTER TABLE `propertysalesorders` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `racebutcheryprofiles`
--

DROP TABLE IF EXISTS `racebutcheryprofiles`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `racebutcheryprofiles` (
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
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `racebutcheryprofiles`
--

LOCK TABLES `racebutcheryprofiles` WRITE;
/*!40000 ALTER TABLE `racebutcheryprofiles` DISABLE KEYS */;
/*!40000 ALTER TABLE `racebutcheryprofiles` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `racebutcheryprofiles_breakdownchecks`
--

DROP TABLE IF EXISTS `racebutcheryprofiles_breakdownchecks`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `racebutcheryprofiles_breakdownchecks` (
  `RaceButcheryProfileId` bigint NOT NULL,
  `Subcageory` varchar(100) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `TraitDefinitionId` bigint NOT NULL,
  `Difficulty` int NOT NULL,
  PRIMARY KEY (`RaceButcheryProfileId`,`Subcageory`),
  KEY `FK_RaceButcheryProfiles_BreakdownChecks_TraitDefinitions_idx` (`TraitDefinitionId`),
  CONSTRAINT `FK_RaceButcheryProfiles_BreakdownChecks_RaceButcheryProfiles` FOREIGN KEY (`RaceButcheryProfileId`) REFERENCES `racebutcheryprofiles` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_RaceButcheryProfiles_BreakdownChecks_TraitDefinitions` FOREIGN KEY (`TraitDefinitionId`) REFERENCES `traitdefinitions` (`Id`) ON DELETE RESTRICT
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `racebutcheryprofiles_breakdownchecks`
--

LOCK TABLES `racebutcheryprofiles_breakdownchecks` WRITE;
/*!40000 ALTER TABLE `racebutcheryprofiles_breakdownchecks` DISABLE KEYS */;
/*!40000 ALTER TABLE `racebutcheryprofiles_breakdownchecks` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `racebutcheryprofiles_breakdownemotes`
--

DROP TABLE IF EXISTS `racebutcheryprofiles_breakdownemotes`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `racebutcheryprofiles_breakdownemotes` (
  `RaceButcheryProfileId` bigint NOT NULL,
  `Subcategory` varchar(100) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `Order` int NOT NULL,
  `Emote` text CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `Delay` double NOT NULL,
  PRIMARY KEY (`RaceButcheryProfileId`,`Subcategory`,`Order`),
  CONSTRAINT `FK_RaceButcheryProfiles_BreakdownEmotes_RaceButcheryProfiles` FOREIGN KEY (`RaceButcheryProfileId`) REFERENCES `racebutcheryprofiles` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `racebutcheryprofiles_breakdownemotes`
--

LOCK TABLES `racebutcheryprofiles_breakdownemotes` WRITE;
/*!40000 ALTER TABLE `racebutcheryprofiles_breakdownemotes` DISABLE KEYS */;
/*!40000 ALTER TABLE `racebutcheryprofiles_breakdownemotes` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `racebutcheryprofiles_butcheryproducts`
--

DROP TABLE IF EXISTS `racebutcheryprofiles_butcheryproducts`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `racebutcheryprofiles_butcheryproducts` (
  `RaceButcheryProfileId` bigint NOT NULL,
  `ButcheryProductId` bigint NOT NULL,
  PRIMARY KEY (`RaceButcheryProfileId`,`ButcheryProductId`),
  KEY `FK_RaceButcheryProfiles_ButcheryProducts_ButcheryProducts_idx` (`ButcheryProductId`),
  CONSTRAINT `FK_RaceButcheryProfiles_ButcheryProducts_ButcheryProducts` FOREIGN KEY (`ButcheryProductId`) REFERENCES `butcheryproducts` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_RaceButcheryProfiles_ButcheryProducts_RaceButcheryProfiles` FOREIGN KEY (`RaceButcheryProfileId`) REFERENCES `racebutcheryprofiles` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `racebutcheryprofiles_butcheryproducts`
--

LOCK TABLES `racebutcheryprofiles_butcheryproducts` WRITE;
/*!40000 ALTER TABLE `racebutcheryprofiles_butcheryproducts` DISABLE KEYS */;
/*!40000 ALTER TABLE `racebutcheryprofiles_butcheryproducts` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `racebutcheryprofiles_skinningemotes`
--

DROP TABLE IF EXISTS `racebutcheryprofiles_skinningemotes`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `racebutcheryprofiles_skinningemotes` (
  `RaceButcheryProfileId` bigint NOT NULL,
  `Subcategory` varchar(100) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `Order` int NOT NULL,
  `Emote` text CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `Delay` double NOT NULL,
  PRIMARY KEY (`RaceButcheryProfileId`,`Subcategory`,`Order`),
  CONSTRAINT `FK_RaceButcheryProfiles_SkinningEmotes_RaceButcheryProfiles` FOREIGN KEY (`RaceButcheryProfileId`) REFERENCES `racebutcheryprofiles` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `racebutcheryprofiles_skinningemotes`
--

LOCK TABLES `racebutcheryprofiles_skinningemotes` WRITE;
/*!40000 ALTER TABLE `racebutcheryprofiles_skinningemotes` DISABLE KEYS */;
/*!40000 ALTER TABLE `racebutcheryprofiles_skinningemotes` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `raceedibleforagableyields`
--

DROP TABLE IF EXISTS `raceedibleforagableyields`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `raceedibleforagableyields` (
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
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `raceedibleforagableyields`
--

LOCK TABLES `raceedibleforagableyields` WRITE;
/*!40000 ALTER TABLE `raceedibleforagableyields` DISABLE KEYS */;
/*!40000 ALTER TABLE `raceedibleforagableyields` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `races`
--

DROP TABLE IF EXISTS `races`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `races` (
  `Id` bigint NOT NULL AUTO_INCREMENT,
  `Name` varchar(255) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `Description` varchar(4000) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `BaseBodyId` bigint NOT NULL,
  `AllowedGenders` varchar(255) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `ParentRaceId` bigint DEFAULT NULL,
  `AttributeBonusProgId` bigint NOT NULL,
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
  `ChildAge` int NOT NULL DEFAULT '3',
  `YouthAge` int NOT NULL DEFAULT '10',
  `YoungAdultAge` int NOT NULL DEFAULT '16',
  `AdultAge` int NOT NULL DEFAULT '21',
  `ElderAge` int NOT NULL DEFAULT '55',
  `VenerableAge` int NOT NULL DEFAULT '75',
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
  PRIMARY KEY (`Id`),
  KEY `FK_Races_AttributeBonusProg` (`AttributeBonusProgId`),
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
  CONSTRAINT `FK_Races_AttributeBonusProg` FOREIGN KEY (`AttributeBonusProgId`) REFERENCES `futureprogs` (`Id`) ON DELETE RESTRICT,
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
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `races`
--

LOCK TABLES `races` WRITE;
/*!40000 ALTER TABLE `races` DISABLE KEYS */;
/*!40000 ALTER TABLE `races` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `races_additionalbodyparts`
--

DROP TABLE IF EXISTS `races_additionalbodyparts`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `races_additionalbodyparts` (
  `Usage` varchar(50) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `BodypartId` bigint NOT NULL,
  `RaceId` bigint NOT NULL,
  PRIMARY KEY (`Usage`,`RaceId`,`BodypartId`),
  KEY `FK_Races_AdditionalBodyparts_BodypartProto` (`BodypartId`),
  KEY `FK_Races_AdditionalBodyparts_Races` (`RaceId`),
  CONSTRAINT `FK_Races_AdditionalBodyparts_BodypartProto` FOREIGN KEY (`BodypartId`) REFERENCES `bodypartproto` (`Id`) ON DELETE RESTRICT,
  CONSTRAINT `FK_Races_AdditionalBodyparts_Races` FOREIGN KEY (`RaceId`) REFERENCES `races` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `races_additionalbodyparts`
--

LOCK TABLES `races_additionalbodyparts` WRITE;
/*!40000 ALTER TABLE `races_additionalbodyparts` DISABLE KEYS */;
/*!40000 ALTER TABLE `races_additionalbodyparts` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `races_additionalcharacteristics`
--

DROP TABLE IF EXISTS `races_additionalcharacteristics`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `races_additionalcharacteristics` (
  `RaceId` bigint NOT NULL,
  `CharacteristicDefinitionId` bigint NOT NULL,
  `Usage` varchar(255) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  PRIMARY KEY (`RaceId`,`CharacteristicDefinitionId`),
  KEY `FK_RAC_CharacteristicDefinitions` (`CharacteristicDefinitionId`),
  CONSTRAINT `FK_RAC_CharacteristicDefinitions` FOREIGN KEY (`CharacteristicDefinitionId`) REFERENCES `characteristicdefinitions` (`Id`) ON DELETE RESTRICT,
  CONSTRAINT `FK_RAC_Races` FOREIGN KEY (`RaceId`) REFERENCES `races` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `races_additionalcharacteristics`
--

LOCK TABLES `races_additionalcharacteristics` WRITE;
/*!40000 ALTER TABLE `races_additionalcharacteristics` DISABLE KEYS */;
/*!40000 ALTER TABLE `races_additionalcharacteristics` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `races_attributes`
--

DROP TABLE IF EXISTS `races_attributes`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `races_attributes` (
  `RaceId` bigint NOT NULL,
  `AttributeId` bigint NOT NULL,
  `IsHealthAttribute` bit(1) NOT NULL DEFAULT b'0',
  PRIMARY KEY (`RaceId`,`AttributeId`),
  KEY `FK_Races_Attributes_TraitDefinitions` (`AttributeId`),
  CONSTRAINT `FK_Races_Attributes_Races` FOREIGN KEY (`RaceId`) REFERENCES `races` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_Races_Attributes_TraitDefinitions` FOREIGN KEY (`AttributeId`) REFERENCES `traitdefinitions` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `races_attributes`
--

LOCK TABLES `races_attributes` WRITE;
/*!40000 ALTER TABLE `races_attributes` DISABLE KEYS */;
/*!40000 ALTER TABLE `races_attributes` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `races_breathablegases`
--

DROP TABLE IF EXISTS `races_breathablegases`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `races_breathablegases` (
  `RaceId` bigint NOT NULL,
  `GasId` bigint NOT NULL,
  `Multiplier` double NOT NULL DEFAULT '1',
  PRIMARY KEY (`RaceId`,`GasId`),
  KEY `FK_Races-BreathableGases_Gases_idx` (`GasId`),
  CONSTRAINT `FK_Races_BreathableGases_Gases` FOREIGN KEY (`GasId`) REFERENCES `gases` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_Races_BreathableGases_Races` FOREIGN KEY (`RaceId`) REFERENCES `races` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `races_breathablegases`
--

LOCK TABLES `races_breathablegases` WRITE;
/*!40000 ALTER TABLE `races_breathablegases` DISABLE KEYS */;
/*!40000 ALTER TABLE `races_breathablegases` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `races_breathableliquids`
--

DROP TABLE IF EXISTS `races_breathableliquids`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `races_breathableliquids` (
  `RaceId` bigint NOT NULL,
  `LiquidId` bigint NOT NULL,
  `Multiplier` double NOT NULL DEFAULT '1',
  PRIMARY KEY (`RaceId`,`LiquidId`),
  KEY `FK_Races_BreathableLiquids_Liquids_idx` (`LiquidId`),
  CONSTRAINT `FK_Races_BreathableLiquids_Liquids` FOREIGN KEY (`LiquidId`) REFERENCES `liquids` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_Races_BreathableLiquids_Races` FOREIGN KEY (`RaceId`) REFERENCES `races` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `races_breathableliquids`
--

LOCK TABLES `races_breathableliquids` WRITE;
/*!40000 ALTER TABLE `races_breathableliquids` DISABLE KEYS */;
/*!40000 ALTER TABLE `races_breathableliquids` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `races_chargenresources`
--

DROP TABLE IF EXISTS `races_chargenresources`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `races_chargenresources` (
  `RaceId` bigint NOT NULL,
  `ChargenResourceId` bigint NOT NULL,
  `RequirementOnly` bit(1) NOT NULL DEFAULT b'0',
  `Amount` int NOT NULL,
  PRIMARY KEY (`RaceId`,`ChargenResourceId`),
  KEY `FK_Races_ChargenResources_ChargenResources` (`ChargenResourceId`),
  CONSTRAINT `FK_Races_ChargenResources_ChargenResources_ChargenResourceId` FOREIGN KEY (`ChargenResourceId`) REFERENCES `chargenresources` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_Races_ChargenResources_Races_RaceId` FOREIGN KEY (`RaceId`) REFERENCES `races` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `races_chargenresources`
--

LOCK TABLES `races_chargenresources` WRITE;
/*!40000 ALTER TABLE `races_chargenresources` DISABLE KEYS */;
/*!40000 ALTER TABLE `races_chargenresources` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `races_combatactions`
--

DROP TABLE IF EXISTS `races_combatactions`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `races_combatactions` (
  `RaceId` bigint NOT NULL,
  `CombatActionId` bigint NOT NULL,
  PRIMARY KEY (`RaceId`,`CombatActionId`),
  KEY `IX_Races_CombatActions_CombatActionId` (`CombatActionId`),
  CONSTRAINT `FK_Races_CombatActions_CombatActions` FOREIGN KEY (`CombatActionId`) REFERENCES `combatactions` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_Races_CombatActions_Races` FOREIGN KEY (`RaceId`) REFERENCES `races` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `races_combatactions`
--

LOCK TABLES `races_combatactions` WRITE;
/*!40000 ALTER TABLE `races_combatactions` DISABLE KEYS */;
/*!40000 ALTER TABLE `races_combatactions` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `races_ediblematerials`
--

DROP TABLE IF EXISTS `races_ediblematerials`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `races_ediblematerials` (
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
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `races_ediblematerials`
--

LOCK TABLES `races_ediblematerials` WRITE;
/*!40000 ALTER TABLE `races_ediblematerials` DISABLE KEYS */;
/*!40000 ALTER TABLE `races_ediblematerials` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `races_removebreathablegases`
--

DROP TABLE IF EXISTS `races_removebreathablegases`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `races_removebreathablegases` (
  `RaceId` bigint NOT NULL,
  `GasId` bigint NOT NULL,
  PRIMARY KEY (`RaceId`,`GasId`),
  KEY `FK_Races_RemoveBreathableGases_Gases_idx` (`GasId`),
  CONSTRAINT `FK_Races_RemoveBreathableGases_Gases` FOREIGN KEY (`GasId`) REFERENCES `gases` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_Races_RemoveBreathableGases_Races` FOREIGN KEY (`RaceId`) REFERENCES `races` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `races_removebreathablegases`
--

LOCK TABLES `races_removebreathablegases` WRITE;
/*!40000 ALTER TABLE `races_removebreathablegases` DISABLE KEYS */;
/*!40000 ALTER TABLE `races_removebreathablegases` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `races_removebreathableliquids`
--

DROP TABLE IF EXISTS `races_removebreathableliquids`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `races_removebreathableliquids` (
  `RaceId` bigint NOT NULL,
  `LiquidId` bigint NOT NULL,
  PRIMARY KEY (`RaceId`,`LiquidId`),
  KEY `FK_Races_RemoveBreathableLiquids_Liquids_idx` (`LiquidId`),
  CONSTRAINT `FK_Races_RemoveBreathableLiquids_Liquids` FOREIGN KEY (`LiquidId`) REFERENCES `liquids` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_Races_RemoveBreathableLiquids_Races` FOREIGN KEY (`RaceId`) REFERENCES `races` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `races_removebreathableliquids`
--

LOCK TABLES `races_removebreathableliquids` WRITE;
/*!40000 ALTER TABLE `races_removebreathableliquids` DISABLE KEYS */;
/*!40000 ALTER TABLE `races_removebreathableliquids` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `races_weaponattacks`
--

DROP TABLE IF EXISTS `races_weaponattacks`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `races_weaponattacks` (
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
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `races_weaponattacks`
--

LOCK TABLES `races_weaponattacks` WRITE;
/*!40000 ALTER TABLE `races_weaponattacks` DISABLE KEYS */;
/*!40000 ALTER TABLE `races_weaponattacks` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `randomnameprofiles`
--

DROP TABLE IF EXISTS `randomnameprofiles`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `randomnameprofiles` (
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
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `randomnameprofiles`
--

LOCK TABLES `randomnameprofiles` WRITE;
/*!40000 ALTER TABLE `randomnameprofiles` DISABLE KEYS */;
/*!40000 ALTER TABLE `randomnameprofiles` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `randomnameprofiles_diceexpressions`
--

DROP TABLE IF EXISTS `randomnameprofiles_diceexpressions`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `randomnameprofiles_diceexpressions` (
  `RandomNameProfileId` bigint NOT NULL,
  `NameUsage` int NOT NULL,
  `DiceExpression` varchar(255) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  PRIMARY KEY (`RandomNameProfileId`,`NameUsage`),
  CONSTRAINT `FK_RandomNameProfiles_DiceExpressions_RandomNameProfiles` FOREIGN KEY (`RandomNameProfileId`) REFERENCES `randomnameprofiles` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `randomnameprofiles_diceexpressions`
--

LOCK TABLES `randomnameprofiles_diceexpressions` WRITE;
/*!40000 ALTER TABLE `randomnameprofiles_diceexpressions` DISABLE KEYS */;
/*!40000 ALTER TABLE `randomnameprofiles_diceexpressions` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `randomnameprofiles_elements`
--

DROP TABLE IF EXISTS `randomnameprofiles_elements`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `randomnameprofiles_elements` (
  `RandomNameProfileId` bigint NOT NULL,
  `NameUsage` int NOT NULL,
  `Name` varchar(100) CHARACTER SET utf8mb3 COLLATE utf8mb3_bin NOT NULL,
  `Weighting` int NOT NULL,
  PRIMARY KEY (`RandomNameProfileId`,`NameUsage`,`Name`),
  CONSTRAINT `FK_RandomNameProfiles_Elements_RandomNameProfiles` FOREIGN KEY (`RandomNameProfileId`) REFERENCES `randomnameprofiles` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `randomnameprofiles_elements`
--

LOCK TABLES `randomnameprofiles_elements` WRITE;
/*!40000 ALTER TABLE `randomnameprofiles_elements` DISABLE KEYS */;
/*!40000 ALTER TABLE `randomnameprofiles_elements` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `rangedcovers`
--

DROP TABLE IF EXISTS `rangedcovers`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `rangedcovers` (
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
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `rangedcovers`
--

LOCK TABLES `rangedcovers` WRITE;
/*!40000 ALTER TABLE `rangedcovers` DISABLE KEYS */;
/*!40000 ALTER TABLE `rangedcovers` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `rangedweapontypes`
--

DROP TABLE IF EXISTS `rangedweapontypes`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `rangedweapontypes` (
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
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `rangedweapontypes`
--

LOCK TABLES `rangedweapontypes` WRITE;
/*!40000 ALTER TABLE `rangedweapontypes` DISABLE KEYS */;
/*!40000 ALTER TABLE `rangedweapontypes` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `ranks`
--

DROP TABLE IF EXISTS `ranks`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `ranks` (
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
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `ranks`
--

LOCK TABLES `ranks` WRITE;
/*!40000 ALTER TABLE `ranks` DISABLE KEYS */;
/*!40000 ALTER TABLE `ranks` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `ranks_abbreviations`
--

DROP TABLE IF EXISTS `ranks_abbreviations`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `ranks_abbreviations` (
  `RankId` bigint NOT NULL,
  `Abbreviation` varchar(50) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `FutureProgId` bigint DEFAULT NULL,
  `Order` int NOT NULL,
  PRIMARY KEY (`RankId`,`Abbreviation`),
  KEY `FK_Ranks_Abbreviations_FutureProgs` (`FutureProgId`),
  CONSTRAINT `FK_Ranks_Abbreviations_FutureProgs` FOREIGN KEY (`FutureProgId`) REFERENCES `futureprogs` (`Id`) ON DELETE SET NULL,
  CONSTRAINT `FK_Ranks_Abbreviations_Ranks` FOREIGN KEY (`RankId`) REFERENCES `ranks` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `ranks_abbreviations`
--

LOCK TABLES `ranks_abbreviations` WRITE;
/*!40000 ALTER TABLE `ranks_abbreviations` DISABLE KEYS */;
/*!40000 ALTER TABLE `ranks_abbreviations` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `ranks_paygrades`
--

DROP TABLE IF EXISTS `ranks_paygrades`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `ranks_paygrades` (
  `RankId` bigint NOT NULL,
  `PaygradeId` bigint NOT NULL,
  `Order` int NOT NULL,
  PRIMARY KEY (`RankId`,`PaygradeId`),
  KEY `FK_Ranks_Paygrades_Paygrades` (`PaygradeId`),
  CONSTRAINT `FK_Ranks_Paygrades_Paygrades` FOREIGN KEY (`PaygradeId`) REFERENCES `paygrades` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_Ranks_Paygrades_Ranks` FOREIGN KEY (`RankId`) REFERENCES `ranks` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `ranks_paygrades`
--

LOCK TABLES `ranks_paygrades` WRITE;
/*!40000 ALTER TABLE `ranks_paygrades` DISABLE KEYS */;
/*!40000 ALTER TABLE `ranks_paygrades` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `ranks_titles`
--

DROP TABLE IF EXISTS `ranks_titles`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `ranks_titles` (
  `RankId` bigint NOT NULL,
  `Title` varchar(50) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `FutureProgId` bigint DEFAULT NULL,
  `Order` int NOT NULL,
  PRIMARY KEY (`RankId`,`Title`),
  KEY `FK_Ranks_Titles_FutureProgs` (`FutureProgId`),
  CONSTRAINT `FK_Ranks_Titles_FutureProgs` FOREIGN KEY (`FutureProgId`) REFERENCES `futureprogs` (`Id`) ON DELETE SET NULL,
  CONSTRAINT `FK_Ranks_Titles_Ranks` FOREIGN KEY (`RankId`) REFERENCES `ranks` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `ranks_titles`
--

LOCK TABLES `ranks_titles` WRITE;
/*!40000 ALTER TABLE `ranks_titles` DISABLE KEYS */;
/*!40000 ALTER TABLE `ranks_titles` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `regionalclimates`
--

DROP TABLE IF EXISTS `regionalclimates`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `regionalclimates` (
  `Id` bigint NOT NULL AUTO_INCREMENT,
  `Name` varchar(200) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `ClimateModelId` bigint NOT NULL,
  `Description` text CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci,
  `TemperatureFluctuationPeriodMinutes` int NOT NULL DEFAULT '0',
  `TemperatureFluctuationStandardDeviation` double NOT NULL DEFAULT '0',
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `regionalclimates`
--

LOCK TABLES `regionalclimates` WRITE;
/*!40000 ALTER TABLE `regionalclimates` DISABLE KEYS */;
/*!40000 ALTER TABLE `regionalclimates` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `regionalclimates_seasons`
--

DROP TABLE IF EXISTS `regionalclimates_seasons`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `regionalclimates_seasons` (
  `RegionalClimateId` bigint NOT NULL,
  `SeasonId` bigint NOT NULL,
  `TemperatureInfo` text CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  PRIMARY KEY (`RegionalClimateId`,`SeasonId`),
  KEY `FK_RegionalClimates_Seasons_Seasons_idx` (`SeasonId`),
  CONSTRAINT `FK_RegionalClimates_Seasons_RegionalClimates` FOREIGN KEY (`RegionalClimateId`) REFERENCES `regionalclimates` (`Id`) ON DELETE RESTRICT,
  CONSTRAINT `FK_RegionalClimates_Seasons_Seasons` FOREIGN KEY (`SeasonId`) REFERENCES `seasons` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `regionalclimates_seasons`
--

LOCK TABLES `regionalclimates_seasons` WRITE;
/*!40000 ALTER TABLE `regionalclimates_seasons` DISABLE KEYS */;
/*!40000 ALTER TABLE `regionalclimates_seasons` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `rooms`
--

DROP TABLE IF EXISTS `rooms`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `rooms` (
  `Id` bigint NOT NULL AUTO_INCREMENT,
  `ZoneId` bigint NOT NULL,
  `X` int NOT NULL,
  `Y` int NOT NULL,
  `Z` int NOT NULL,
  PRIMARY KEY (`Id`),
  KEY `FK_Rooms_Zones` (`ZoneId`),
  CONSTRAINT `FK_Rooms_Zones` FOREIGN KEY (`ZoneId`) REFERENCES `zones` (`Id`) ON DELETE RESTRICT
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `rooms`
--

LOCK TABLES `rooms` WRITE;
/*!40000 ALTER TABLE `rooms` DISABLE KEYS */;
/*!40000 ALTER TABLE `rooms` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `scriptedeventfreetextquestions`
--

DROP TABLE IF EXISTS `scriptedeventfreetextquestions`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `scriptedeventfreetextquestions` (
  `Id` bigint NOT NULL AUTO_INCREMENT,
  `ScriptedEventId` bigint NOT NULL,
  `Question` mediumtext CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci,
  `Answer` mediumtext CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci,
  PRIMARY KEY (`Id`),
  KEY `IX_ScriptedEventFreeTextQuestions_ScriptedEventId` (`ScriptedEventId`),
  CONSTRAINT `FK_ScriptedEventFreeTextQuestions_ScriptedEvents` FOREIGN KEY (`ScriptedEventId`) REFERENCES `scriptedevents` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `scriptedeventfreetextquestions`
--

LOCK TABLES `scriptedeventfreetextquestions` WRITE;
/*!40000 ALTER TABLE `scriptedeventfreetextquestions` DISABLE KEYS */;
/*!40000 ALTER TABLE `scriptedeventfreetextquestions` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `scriptedeventmultiplechoicequestionanswers`
--

DROP TABLE IF EXISTS `scriptedeventmultiplechoicequestionanswers`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `scriptedeventmultiplechoicequestionanswers` (
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
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `scriptedeventmultiplechoicequestionanswers`
--

LOCK TABLES `scriptedeventmultiplechoicequestionanswers` WRITE;
/*!40000 ALTER TABLE `scriptedeventmultiplechoicequestionanswers` DISABLE KEYS */;
/*!40000 ALTER TABLE `scriptedeventmultiplechoicequestionanswers` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `scriptedeventmultiplechoicequestions`
--

DROP TABLE IF EXISTS `scriptedeventmultiplechoicequestions`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `scriptedeventmultiplechoicequestions` (
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
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `scriptedeventmultiplechoicequestions`
--

LOCK TABLES `scriptedeventmultiplechoicequestions` WRITE;
/*!40000 ALTER TABLE `scriptedeventmultiplechoicequestions` DISABLE KEYS */;
/*!40000 ALTER TABLE `scriptedeventmultiplechoicequestions` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `scriptedevents`
--

DROP TABLE IF EXISTS `scriptedevents`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `scriptedevents` (
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
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `scriptedevents`
--

LOCK TABLES `scriptedevents` WRITE;
/*!40000 ALTER TABLE `scriptedevents` DISABLE KEYS */;
/*!40000 ALTER TABLE `scriptedevents` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `scripts`
--

DROP TABLE IF EXISTS `scripts`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `scripts` (
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
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `scripts`
--

LOCK TABLES `scripts` WRITE;
/*!40000 ALTER TABLE `scripts` DISABLE KEYS */;
/*!40000 ALTER TABLE `scripts` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `scripts_designedlanguages`
--

DROP TABLE IF EXISTS `scripts_designedlanguages`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `scripts_designedlanguages` (
  `ScriptId` bigint NOT NULL,
  `LanguageId` bigint NOT NULL,
  PRIMARY KEY (`ScriptId`,`LanguageId`),
  KEY `FK_Scripts_DesignedLanguages_Languages_idx` (`LanguageId`),
  CONSTRAINT `FK_Scripts_DesignedLanguages_Languages` FOREIGN KEY (`LanguageId`) REFERENCES `languages` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_Scripts_DesignedLanguages_Scripts` FOREIGN KEY (`ScriptId`) REFERENCES `scripts` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `scripts_designedlanguages`
--

LOCK TABLES `scripts_designedlanguages` WRITE;
/*!40000 ALTER TABLE `scripts_designedlanguages` DISABLE KEYS */;
/*!40000 ALTER TABLE `scripts_designedlanguages` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `seasons`
--

DROP TABLE IF EXISTS `seasons`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `seasons` (
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
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `seasons`
--

LOCK TABLES `seasons` WRITE;
/*!40000 ALTER TABLE `seasons` DISABLE KEYS */;
/*!40000 ALTER TABLE `seasons` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `seederchoices`
--

DROP TABLE IF EXISTS `seederchoices`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `seederchoices` (
  `Id` bigint NOT NULL AUTO_INCREMENT,
  `Version` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  `Seeder` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  `Choice` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  `Answer` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  `DateTime` datetime(6) NOT NULL,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `seederchoices`
--

LOCK TABLES `seederchoices` WRITE;
/*!40000 ALTER TABLE `seederchoices` DISABLE KEYS */;
/*!40000 ALTER TABLE `seederchoices` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `shards`
--

DROP TABLE IF EXISTS `shards`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `shards` (
  `Id` bigint NOT NULL AUTO_INCREMENT,
  `Name` varchar(255) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `MinimumTerrestrialLux` double NOT NULL,
  `SkyDescriptionTemplateId` bigint NOT NULL,
  `SphericalRadiusMetres` double NOT NULL DEFAULT '6371000',
  PRIMARY KEY (`Id`),
  KEY `FK_Shards_SkyDescriptionTemplates` (`SkyDescriptionTemplateId`),
  CONSTRAINT `FK_Shards_SkyDescriptionTemplates` FOREIGN KEY (`SkyDescriptionTemplateId`) REFERENCES `skydescriptiontemplates` (`Id`) ON DELETE RESTRICT
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `shards`
--

LOCK TABLES `shards` WRITE;
/*!40000 ALTER TABLE `shards` DISABLE KEYS */;
/*!40000 ALTER TABLE `shards` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `shards_calendars`
--

DROP TABLE IF EXISTS `shards_calendars`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `shards_calendars` (
  `ShardId` bigint NOT NULL,
  `CalendarId` bigint NOT NULL,
  PRIMARY KEY (`ShardId`,`CalendarId`),
  CONSTRAINT `FK_Shards_Calendars_Shards` FOREIGN KEY (`ShardId`) REFERENCES `shards` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `shards_calendars`
--

LOCK TABLES `shards_calendars` WRITE;
/*!40000 ALTER TABLE `shards_calendars` DISABLE KEYS */;
/*!40000 ALTER TABLE `shards_calendars` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `shards_celestials`
--

DROP TABLE IF EXISTS `shards_celestials`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `shards_celestials` (
  `ShardId` bigint NOT NULL,
  `CelestialId` bigint NOT NULL,
  PRIMARY KEY (`ShardId`,`CelestialId`),
  CONSTRAINT `FK_Shards_Celestials_Shards` FOREIGN KEY (`ShardId`) REFERENCES `shards` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `shards_celestials`
--

LOCK TABLES `shards_celestials` WRITE;
/*!40000 ALTER TABLE `shards_celestials` DISABLE KEYS */;
/*!40000 ALTER TABLE `shards_celestials` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `shards_clocks`
--

DROP TABLE IF EXISTS `shards_clocks`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `shards_clocks` (
  `ShardId` bigint NOT NULL,
  `ClockId` bigint NOT NULL,
  PRIMARY KEY (`ShardId`,`ClockId`),
  CONSTRAINT `FK_Shards_Clocks_Shards` FOREIGN KEY (`ShardId`) REFERENCES `shards` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `shards_clocks`
--

LOCK TABLES `shards_clocks` WRITE;
/*!40000 ALTER TABLE `shards_clocks` DISABLE KEYS */;
/*!40000 ALTER TABLE `shards_clocks` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `shieldtypes`
--

DROP TABLE IF EXISTS `shieldtypes`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `shieldtypes` (
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
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `shieldtypes`
--

LOCK TABLES `shieldtypes` WRITE;
/*!40000 ALTER TABLE `shieldtypes` DISABLE KEYS */;
/*!40000 ALTER TABLE `shieldtypes` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `shopdeals`
--

DROP TABLE IF EXISTS `shopdeals`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `shopdeals` (
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
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `shopdeals`
--

LOCK TABLES `shopdeals` WRITE;
/*!40000 ALTER TABLE `shopdeals` DISABLE KEYS */;
/*!40000 ALTER TABLE `shopdeals` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `shopfinancialperiodresults`
--

DROP TABLE IF EXISTS `shopfinancialperiodresults`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `shopfinancialperiodresults` (
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
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `shopfinancialperiodresults`
--

LOCK TABLES `shopfinancialperiodresults` WRITE;
/*!40000 ALTER TABLE `shopfinancialperiodresults` DISABLE KEYS */;
/*!40000 ALTER TABLE `shopfinancialperiodresults` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `shopperlogs`
--

DROP TABLE IF EXISTS `shopperlogs`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `shopperlogs` (
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
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `shopperlogs`
--

LOCK TABLES `shopperlogs` WRITE;
/*!40000 ALTER TABLE `shopperlogs` DISABLE KEYS */;
/*!40000 ALTER TABLE `shopperlogs` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `shoppers`
--

DROP TABLE IF EXISTS `shoppers`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `shoppers` (
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
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `shoppers`
--

LOCK TABLES `shoppers` WRITE;
/*!40000 ALTER TABLE `shoppers` DISABLE KEYS */;
/*!40000 ALTER TABLE `shoppers` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `shops`
--

DROP TABLE IF EXISTS `shops`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `shops` (
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
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `shops`
--

LOCK TABLES `shops` WRITE;
/*!40000 ALTER TABLE `shops` DISABLE KEYS */;
/*!40000 ALTER TABLE `shops` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `shops_storeroomcells`
--

DROP TABLE IF EXISTS `shops_storeroomcells`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `shops_storeroomcells` (
  `ShopId` bigint NOT NULL,
  `CellId` bigint NOT NULL,
  PRIMARY KEY (`ShopId`,`CellId`),
  KEY `FK_Shops_StoreroomCells_Cells_idx` (`CellId`),
  CONSTRAINT `FK_Shops_StoreroomCells_Cells` FOREIGN KEY (`CellId`) REFERENCES `cells` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_Shops_StoreroomCells_Shops` FOREIGN KEY (`ShopId`) REFERENCES `shops` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `shops_storeroomcells`
--

LOCK TABLES `shops_storeroomcells` WRITE;
/*!40000 ALTER TABLE `shops_storeroomcells` DISABLE KEYS */;
/*!40000 ALTER TABLE `shops_storeroomcells` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `shopstills`
--

DROP TABLE IF EXISTS `shopstills`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `shopstills` (
  `ShopId` bigint NOT NULL,
  `GameItemId` bigint NOT NULL,
  PRIMARY KEY (`ShopId`,`GameItemId`),
  KEY `FK_ShopTills_GameItems_idx` (`GameItemId`),
  CONSTRAINT `FK_ShopTills_GameItems` FOREIGN KEY (`GameItemId`) REFERENCES `gameitems` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_ShopTills_Shops` FOREIGN KEY (`ShopId`) REFERENCES `shops` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `shopstills`
--

LOCK TABLES `shopstills` WRITE;
/*!40000 ALTER TABLE `shopstills` DISABLE KEYS */;
/*!40000 ALTER TABLE `shopstills` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `shoptransactionrecords`
--

DROP TABLE IF EXISTS `shoptransactionrecords`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `shoptransactionrecords` (
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
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `shoptransactionrecords`
--

LOCK TABLES `shoptransactionrecords` WRITE;
/*!40000 ALTER TABLE `shoptransactionrecords` DISABLE KEYS */;
/*!40000 ALTER TABLE `shoptransactionrecords` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `skydescriptiontemplates`
--

DROP TABLE IF EXISTS `skydescriptiontemplates`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `skydescriptiontemplates` (
  `Id` bigint NOT NULL AUTO_INCREMENT,
  `Name` varchar(255) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `skydescriptiontemplates`
--

LOCK TABLES `skydescriptiontemplates` WRITE;
/*!40000 ALTER TABLE `skydescriptiontemplates` DISABLE KEYS */;
/*!40000 ALTER TABLE `skydescriptiontemplates` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `skydescriptiontemplates_values`
--

DROP TABLE IF EXISTS `skydescriptiontemplates_values`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `skydescriptiontemplates_values` (
  `SkyDescriptionTemplateId` bigint NOT NULL,
  `LowerBound` double NOT NULL,
  `UpperBound` double NOT NULL,
  `Description` varchar(4000) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  PRIMARY KEY (`SkyDescriptionTemplateId`,`LowerBound`),
  CONSTRAINT `FK_SkyDescriptionTemplates_Values_SkyDescriptionTemplates` FOREIGN KEY (`SkyDescriptionTemplateId`) REFERENCES `skydescriptiontemplates` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `skydescriptiontemplates_values`
--

LOCK TABLES `skydescriptiontemplates_values` WRITE;
/*!40000 ALTER TABLE `skydescriptiontemplates_values` DISABLE KEYS */;
/*!40000 ALTER TABLE `skydescriptiontemplates_values` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `socials`
--

DROP TABLE IF EXISTS `socials`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `socials` (
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
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `socials`
--

LOCK TABLES `socials` WRITE;
/*!40000 ALTER TABLE `socials` DISABLE KEYS */;
/*!40000 ALTER TABLE `socials` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `stackdecorators`
--

DROP TABLE IF EXISTS `stackdecorators`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `stackdecorators` (
  `Id` bigint NOT NULL AUTO_INCREMENT,
  `Name` varchar(255) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `Type` varchar(50) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `Definition` varchar(10000) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `Description` varchar(1000) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci DEFAULT NULL,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `stackdecorators`
--

LOCK TABLES `stackdecorators` WRITE;
/*!40000 ALTER TABLE `stackdecorators` DISABLE KEYS */;
/*!40000 ALTER TABLE `stackdecorators` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `staticconfigurations`
--

DROP TABLE IF EXISTS `staticconfigurations`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `staticconfigurations` (
  `SettingName` varchar(100) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `Definition` text CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  PRIMARY KEY (`SettingName`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `staticconfigurations`
--

LOCK TABLES `staticconfigurations` WRITE;
/*!40000 ALTER TABLE `staticconfigurations` DISABLE KEYS */;
/*!40000 ALTER TABLE `staticconfigurations` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `staticstrings`
--

DROP TABLE IF EXISTS `staticstrings`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `staticstrings` (
  `Id` varchar(50) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `Text` text CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `staticstrings`
--

LOCK TABLES `staticstrings` WRITE;
/*!40000 ALTER TABLE `staticstrings` DISABLE KEYS */;
/*!40000 ALTER TABLE `staticstrings` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `surgicalprocedurephases`
--

DROP TABLE IF EXISTS `surgicalprocedurephases`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `surgicalprocedurephases` (
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
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `surgicalprocedurephases`
--

LOCK TABLES `surgicalprocedurephases` WRITE;
/*!40000 ALTER TABLE `surgicalprocedurephases` DISABLE KEYS */;
/*!40000 ALTER TABLE `surgicalprocedurephases` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `surgicalprocedures`
--

DROP TABLE IF EXISTS `surgicalprocedures`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `surgicalprocedures` (
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
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `surgicalprocedures`
--

LOCK TABLES `surgicalprocedures` WRITE;
/*!40000 ALTER TABLE `surgicalprocedures` DISABLE KEYS */;
/*!40000 ALTER TABLE `surgicalprocedures` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `tags`
--

DROP TABLE IF EXISTS `tags`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `tags` (
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
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `tags`
--

LOCK TABLES `tags` WRITE;
/*!40000 ALTER TABLE `tags` DISABLE KEYS */;
/*!40000 ALTER TABLE `tags` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `terrains`
--

DROP TABLE IF EXISTS `terrains`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `terrains` (
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
  PRIMARY KEY (`Id`),
  KEY `FK_Terrains_WeatherControllers_idx` (`WeatherControllerId`),
  CONSTRAINT `FK_Terrains_WeatherControllers` FOREIGN KEY (`WeatherControllerId`) REFERENCES `weathercontrollers` (`Id`) ON DELETE SET NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `terrains`
--

LOCK TABLES `terrains` WRITE;
/*!40000 ALTER TABLE `terrains` DISABLE KEYS */;
/*!40000 ALTER TABLE `terrains` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `terrains_rangedcovers`
--

DROP TABLE IF EXISTS `terrains_rangedcovers`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `terrains_rangedcovers` (
  `TerrainId` bigint NOT NULL,
  `RangedCoverId` bigint NOT NULL,
  PRIMARY KEY (`TerrainId`,`RangedCoverId`),
  KEY `FK_Terrains_RangedCovers_RangedCovers_idx` (`RangedCoverId`),
  CONSTRAINT `FK_Terrains_RangedCovers_RangedCovers` FOREIGN KEY (`RangedCoverId`) REFERENCES `rangedcovers` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_Terrains_RangedCovers_Terrains` FOREIGN KEY (`TerrainId`) REFERENCES `terrains` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `terrains_rangedcovers`
--

LOCK TABLES `terrains_rangedcovers` WRITE;
/*!40000 ALTER TABLE `terrains_rangedcovers` DISABLE KEYS */;
/*!40000 ALTER TABLE `terrains_rangedcovers` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `timezoneinfos`
--

DROP TABLE IF EXISTS `timezoneinfos`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `timezoneinfos` (
  `Id` varchar(100) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `Display` varchar(1000) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `Order` decimal(58,29) NOT NULL,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `timezoneinfos`
--

LOCK TABLES `timezoneinfos` WRITE;
/*!40000 ALTER TABLE `timezoneinfos` DISABLE KEYS */;
/*!40000 ALTER TABLE `timezoneinfos` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `timezones`
--

DROP TABLE IF EXISTS `timezones`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `timezones` (
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
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `timezones`
--

LOCK TABLES `timezones` WRITE;
/*!40000 ALTER TABLE `timezones` DISABLE KEYS */;
/*!40000 ALTER TABLE `timezones` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `tracks`
--

DROP TABLE IF EXISTS `tracks`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `tracks` (
  `Id` bigint NOT NULL AUTO_INCREMENT,
  `CharacterId` bigint NOT NULL,
  `BodyPrototypeId` bigint NOT NULL,
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
  PRIMARY KEY (`Id`),
  KEY `IX_Tracks_BodyPrototypeId` (`BodyPrototypeId`),
  KEY `IX_Tracks_CellId` (`CellId`),
  KEY `IX_Tracks_CharacterId` (`CharacterId`),
  KEY `IX_Tracks_FromDirectionExitId` (`FromDirectionExitId`),
  KEY `IX_Tracks_FromMoveSpeedId` (`FromMoveSpeedId`),
  KEY `IX_Tracks_ToDirectionExitId` (`ToDirectionExitId`),
  KEY `IX_Tracks_ToMoveSpeedId` (`ToMoveSpeedId`),
  CONSTRAINT `FK_Tracks_BodyProtos` FOREIGN KEY (`BodyPrototypeId`) REFERENCES `bodyprotos` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_Tracks_Cells` FOREIGN KEY (`CellId`) REFERENCES `cells` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_Tracks_Characters` FOREIGN KEY (`CharacterId`) REFERENCES `characters` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_Tracks_Exits_From` FOREIGN KEY (`FromDirectionExitId`) REFERENCES `exits` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_Tracks_Exits_To` FOREIGN KEY (`ToDirectionExitId`) REFERENCES `exits` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_Tracks_MoveSpeeds_From` FOREIGN KEY (`FromMoveSpeedId`) REFERENCES `movespeeds` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_Tracks_MoveSpeeds_To` FOREIGN KEY (`ToMoveSpeedId`) REFERENCES `movespeeds` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `tracks`
--

LOCK TABLES `tracks` WRITE;
/*!40000 ALTER TABLE `tracks` DISABLE KEYS */;
/*!40000 ALTER TABLE `tracks` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `traitdecorators`
--

DROP TABLE IF EXISTS `traitdecorators`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `traitdecorators` (
  `Id` bigint NOT NULL AUTO_INCREMENT,
  `Name` varchar(255) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `Type` varchar(255) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `Contents` text CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `traitdecorators`
--

LOCK TABLES `traitdecorators` WRITE;
/*!40000 ALTER TABLE `traitdecorators` DISABLE KEYS */;
/*!40000 ALTER TABLE `traitdecorators` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `traitdefinitions`
--

DROP TABLE IF EXISTS `traitdefinitions`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `traitdefinitions` (
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
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `traitdefinitions`
--

LOCK TABLES `traitdefinitions` WRITE;
/*!40000 ALTER TABLE `traitdefinitions` DISABLE KEYS */;
/*!40000 ALTER TABLE `traitdefinitions` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `traitdefinitions_chargenresources`
--

DROP TABLE IF EXISTS `traitdefinitions_chargenresources`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `traitdefinitions_chargenresources` (
  `TraitDefinitionId` bigint NOT NULL,
  `ChargenResourceId` bigint NOT NULL,
  `RequirementOnly` bit(1) NOT NULL DEFAULT b'0',
  `Amount` int NOT NULL,
  PRIMARY KEY (`TraitDefinitionId`,`ChargenResourceId`,`RequirementOnly`),
  KEY `FK_TraitDefinitions_ChargenResources_ChargenResources` (`ChargenResourceId`),
  CONSTRAINT `FK_TraitDefinitions_ChargenResources_ChargenResources` FOREIGN KEY (`ChargenResourceId`) REFERENCES `chargenresources` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_TraitDefinitions_ChargenResources_Races` FOREIGN KEY (`TraitDefinitionId`) REFERENCES `traitdefinitions` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `traitdefinitions_chargenresources`
--

LOCK TABLES `traitdefinitions_chargenresources` WRITE;
/*!40000 ALTER TABLE `traitdefinitions_chargenresources` DISABLE KEYS */;
/*!40000 ALTER TABLE `traitdefinitions_chargenresources` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `traitexpression`
--

DROP TABLE IF EXISTS `traitexpression`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `traitexpression` (
  `Id` bigint NOT NULL AUTO_INCREMENT,
  `Expression` varchar(4000) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `Name` varchar(200) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL DEFAULT 'Unnamed Expression',
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `traitexpression`
--

LOCK TABLES `traitexpression` WRITE;
/*!40000 ALTER TABLE `traitexpression` DISABLE KEYS */;
/*!40000 ALTER TABLE `traitexpression` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `traitexpressionparameters`
--

DROP TABLE IF EXISTS `traitexpressionparameters`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `traitexpressionparameters` (
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
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `traitexpressionparameters`
--

LOCK TABLES `traitexpressionparameters` WRITE;
/*!40000 ALTER TABLE `traitexpressionparameters` DISABLE KEYS */;
/*!40000 ALTER TABLE `traitexpressionparameters` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `traits`
--

DROP TABLE IF EXISTS `traits`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `traits` (
  `BodyId` bigint NOT NULL,
  `TraitDefinitionId` bigint NOT NULL,
  `Value` double NOT NULL,
  `AdditionalValue` double NOT NULL,
  PRIMARY KEY (`BodyId`,`TraitDefinitionId`),
  KEY `FK_Traits_TraitDefinitions` (`TraitDefinitionId`),
  CONSTRAINT `FK_Traits_Bodies` FOREIGN KEY (`BodyId`) REFERENCES `bodies` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_Traits_TraitDefinitions` FOREIGN KEY (`TraitDefinitionId`) REFERENCES `traitdefinitions` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `traits`
--

LOCK TABLES `traits` WRITE;
/*!40000 ALTER TABLE `traits` DISABLE KEYS */;
/*!40000 ALTER TABLE `traits` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `unitofmeasure`
--

DROP TABLE IF EXISTS `unitofmeasure`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `unitofmeasure` (
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
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `unitofmeasure`
--

LOCK TABLES `unitofmeasure` WRITE;
/*!40000 ALTER TABLE `unitofmeasure` DISABLE KEYS */;
/*!40000 ALTER TABLE `unitofmeasure` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `variabledefaults`
--

DROP TABLE IF EXISTS `variabledefaults`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `variabledefaults` (
  `Property` varchar(50) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `DefaultValue` varchar(4000) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `OwnerTypeDefinition` varchar(255) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  PRIMARY KEY (`OwnerTypeDefinition`,`Property`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `variabledefaults`
--

LOCK TABLES `variabledefaults` WRITE;
/*!40000 ALTER TABLE `variabledefaults` DISABLE KEYS */;
/*!40000 ALTER TABLE `variabledefaults` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `variabledefinitions`
--

DROP TABLE IF EXISTS `variabledefinitions`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `variabledefinitions` (
  `Property` varchar(50) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `ContainedTypeDefinition` varchar(255) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci DEFAULT NULL,
  `OwnerTypeDefinition` varchar(255) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  PRIMARY KEY (`OwnerTypeDefinition`,`Property`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `variabledefinitions`
--

LOCK TABLES `variabledefinitions` WRITE;
/*!40000 ALTER TABLE `variabledefinitions` DISABLE KEYS */;
/*!40000 ALTER TABLE `variabledefinitions` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `variablevalues`
--

DROP TABLE IF EXISTS `variablevalues`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `variablevalues` (
  `ReferenceId` bigint NOT NULL,
  `ReferenceProperty` varchar(50) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `ValueDefinition` varchar(4000) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `ReferenceTypeDefinition` varchar(255) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `ValueTypeDefinition` varchar(255) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci DEFAULT NULL,
  PRIMARY KEY (`ReferenceTypeDefinition`,`ReferenceId`,`ReferenceProperty`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `variablevalues`
--

LOCK TABLES `variablevalues` WRITE;
/*!40000 ALTER TABLE `variablevalues` DISABLE KEYS */;
/*!40000 ALTER TABLE `variablevalues` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `weaponattacks`
--

DROP TABLE IF EXISTS `weaponattacks`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `weaponattacks` (
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
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `weaponattacks`
--

LOCK TABLES `weaponattacks` WRITE;
/*!40000 ALTER TABLE `weaponattacks` DISABLE KEYS */;
/*!40000 ALTER TABLE `weaponattacks` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `weapontypes`
--

DROP TABLE IF EXISTS `weapontypes`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `weapontypes` (
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
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `weapontypes`
--

LOCK TABLES `weapontypes` WRITE;
/*!40000 ALTER TABLE `weapontypes` DISABLE KEYS */;
/*!40000 ALTER TABLE `weapontypes` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `wearablesizeparameterrule`
--

DROP TABLE IF EXISTS `wearablesizeparameterrule`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `wearablesizeparameterrule` (
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
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `wearablesizeparameterrule`
--

LOCK TABLES `wearablesizeparameterrule` WRITE;
/*!40000 ALTER TABLE `wearablesizeparameterrule` DISABLE KEYS */;
/*!40000 ALTER TABLE `wearablesizeparameterrule` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `wearablesizes`
--

DROP TABLE IF EXISTS `wearablesizes`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `wearablesizes` (
  `Id` bigint NOT NULL AUTO_INCREMENT,
  `OneSizeFitsAll` bit(1) NOT NULL,
  `Height` double DEFAULT NULL,
  `Weight` double DEFAULT NULL,
  `TraitValue` double DEFAULT NULL,
  `BodyPrototypeId` bigint NOT NULL,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `wearablesizes`
--

LOCK TABLES `wearablesizes` WRITE;
/*!40000 ALTER TABLE `wearablesizes` DISABLE KEYS */;
/*!40000 ALTER TABLE `wearablesizes` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `wearprofiles`
--

DROP TABLE IF EXISTS `wearprofiles`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `wearprofiles` (
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
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `wearprofiles`
--

LOCK TABLES `wearprofiles` WRITE;
/*!40000 ALTER TABLE `wearprofiles` DISABLE KEYS */;
/*!40000 ALTER TABLE `wearprofiles` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `weathercontrollers`
--

DROP TABLE IF EXISTS `weathercontrollers`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `weathercontrollers` (
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
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `weathercontrollers`
--

LOCK TABLES `weathercontrollers` WRITE;
/*!40000 ALTER TABLE `weathercontrollers` DISABLE KEYS */;
/*!40000 ALTER TABLE `weathercontrollers` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `weatherevents`
--

DROP TABLE IF EXISTS `weatherevents`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `weatherevents` (
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
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `weatherevents`
--

LOCK TABLES `weatherevents` WRITE;
/*!40000 ALTER TABLE `weatherevents` DISABLE KEYS */;
/*!40000 ALTER TABLE `weatherevents` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `weeklystatistics`
--

DROP TABLE IF EXISTS `weeklystatistics`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `weeklystatistics` (
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
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `weeklystatistics`
--

LOCK TABLES `weeklystatistics` WRITE;
/*!40000 ALTER TABLE `weeklystatistics` DISABLE KEYS */;
/*!40000 ALTER TABLE `weeklystatistics` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `witnessprofiles`
--

DROP TABLE IF EXISTS `witnessprofiles`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `witnessprofiles` (
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
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `witnessprofiles`
--

LOCK TABLES `witnessprofiles` WRITE;
/*!40000 ALTER TABLE `witnessprofiles` DISABLE KEYS */;
/*!40000 ALTER TABLE `witnessprofiles` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `witnessprofiles_cooperatingauthorities`
--

DROP TABLE IF EXISTS `witnessprofiles_cooperatingauthorities`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `witnessprofiles_cooperatingauthorities` (
  `WitnessProfileId` bigint NOT NULL,
  `LegalAuthorityId` bigint NOT NULL,
  PRIMARY KEY (`WitnessProfileId`,`LegalAuthorityId`),
  KEY `FK_WitnessProfiles_CooperatingAuthorities_LegalAuthorities_idx` (`LegalAuthorityId`),
  CONSTRAINT `FK_WitnessProfiles_CooperatingAuthorities_LegalAuthorities` FOREIGN KEY (`LegalAuthorityId`) REFERENCES `legalauthorities` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_WitnessProfiles_CooperatingAuthorities_WitnessProfiles` FOREIGN KEY (`WitnessProfileId`) REFERENCES `witnessprofiles` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `witnessprofiles_cooperatingauthorities`
--

LOCK TABLES `witnessprofiles_cooperatingauthorities` WRITE;
/*!40000 ALTER TABLE `witnessprofiles_cooperatingauthorities` DISABLE KEYS */;
/*!40000 ALTER TABLE `witnessprofiles_cooperatingauthorities` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `witnessprofiles_ignoredcriminalclasses`
--

DROP TABLE IF EXISTS `witnessprofiles_ignoredcriminalclasses`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `witnessprofiles_ignoredcriminalclasses` (
  `WitnessProfileId` bigint NOT NULL,
  `LegalClassId` bigint NOT NULL,
  PRIMARY KEY (`WitnessProfileId`,`LegalClassId`),
  KEY `FK_WitnessProfiles_IgnoredCriminalClasses_LegalClasses_idx` (`LegalClassId`),
  CONSTRAINT `FK_WitnessProfiles_IgnoredCriminalClasses_LegalClasses` FOREIGN KEY (`LegalClassId`) REFERENCES `legalclasses` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_WitnessProfiles_IgnoredCriminalClasses_WitnessProfiles` FOREIGN KEY (`WitnessProfileId`) REFERENCES `witnessprofiles` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `witnessprofiles_ignoredcriminalclasses`
--

LOCK TABLES `witnessprofiles_ignoredcriminalclasses` WRITE;
/*!40000 ALTER TABLE `witnessprofiles_ignoredcriminalclasses` DISABLE KEYS */;
/*!40000 ALTER TABLE `witnessprofiles_ignoredcriminalclasses` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `witnessprofiles_ignoredvictimclasses`
--

DROP TABLE IF EXISTS `witnessprofiles_ignoredvictimclasses`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `witnessprofiles_ignoredvictimclasses` (
  `WitnessProfileId` bigint NOT NULL,
  `LegalClassId` bigint NOT NULL,
  PRIMARY KEY (`WitnessProfileId`,`LegalClassId`),
  KEY `FK_WitnessProfiles_IgnoredVictimClasses_LegalClasses_idx` (`LegalClassId`),
  CONSTRAINT `FK_WitnessProfiles_IgnoredVictimClasses_LegalClasses` FOREIGN KEY (`LegalClassId`) REFERENCES `legalclasses` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_WitnessProfiles_IgnoredVictimClasses_WitnessProfiles` FOREIGN KEY (`WitnessProfileId`) REFERENCES `witnessprofiles` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `witnessprofiles_ignoredvictimclasses`
--

LOCK TABLES `witnessprofiles_ignoredvictimclasses` WRITE;
/*!40000 ALTER TABLE `witnessprofiles_ignoredvictimclasses` DISABLE KEYS */;
/*!40000 ALTER TABLE `witnessprofiles_ignoredvictimclasses` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `wounds`
--

DROP TABLE IF EXISTS `wounds`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `wounds` (
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
  PRIMARY KEY (`Id`),
  KEY `FK_Wounds_Characters_idx` (`ActorOriginId`),
  KEY `FK_Wounds_Bodies_idx` (`BodyId`),
  KEY `FK_Wounds_GameItemOwner_idx` (`GameItemId`),
  KEY `FK_Wounds_GameItems_idx` (`LodgedItemId`),
  KEY `FK_Wounds_GameItems_Tool_idx` (`ToolOriginId`),
  CONSTRAINT `FK_Wounds_Bodies` FOREIGN KEY (`BodyId`) REFERENCES `bodies` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_Wounds_Characters` FOREIGN KEY (`ActorOriginId`) REFERENCES `characters` (`Id`) ON DELETE SET NULL,
  CONSTRAINT `FK_Wounds_GameItemOwner` FOREIGN KEY (`GameItemId`) REFERENCES `gameitems` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_Wounds_GameItems` FOREIGN KEY (`LodgedItemId`) REFERENCES `gameitems` (`Id`) ON DELETE SET NULL,
  CONSTRAINT `FK_Wounds_GameItems_Tool` FOREIGN KEY (`ToolOriginId`) REFERENCES `gameitems` (`Id`) ON DELETE SET NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `wounds`
--

LOCK TABLES `wounds` WRITE;
/*!40000 ALTER TABLE `wounds` DISABLE KEYS */;
/*!40000 ALTER TABLE `wounds` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `writings`
--

DROP TABLE IF EXISTS `writings`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `writings` (
  `Id` bigint NOT NULL AUTO_INCREMENT,
  `WritingType` varchar(45) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
  `Style` int NOT NULL,
  `LanguageId` bigint NOT NULL,
  `ScriptId` bigint NOT NULL,
  `AuthorId` bigint NOT NULL,
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
  CONSTRAINT `FK_Writings_Characters_Author` FOREIGN KEY (`AuthorId`) REFERENCES `characters` (`Id`) ON DELETE RESTRICT,
  CONSTRAINT `FK_Writings_Characters_TrueAuthor` FOREIGN KEY (`TrueAuthorId`) REFERENCES `characters` (`Id`) ON DELETE SET NULL,
  CONSTRAINT `FK_Writings_Languages` FOREIGN KEY (`LanguageId`) REFERENCES `languages` (`Id`) ON DELETE RESTRICT,
  CONSTRAINT `FK_Writings_Scripts` FOREIGN KEY (`ScriptId`) REFERENCES `scripts` (`Id`) ON DELETE RESTRICT
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `writings`
--

LOCK TABLES `writings` WRITE;
/*!40000 ALTER TABLE `writings` DISABLE KEYS */;
/*!40000 ALTER TABLE `writings` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `zones`
--

DROP TABLE IF EXISTS `zones`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `zones` (
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
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `zones`
--

LOCK TABLES `zones` WRITE;
/*!40000 ALTER TABLE `zones` DISABLE KEYS */;
/*!40000 ALTER TABLE `zones` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `zones_timezones`
--

DROP TABLE IF EXISTS `zones_timezones`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `zones_timezones` (
  `ZoneId` bigint NOT NULL,
  `ClockId` bigint NOT NULL,
  `TimezoneId` bigint NOT NULL,
  PRIMARY KEY (`ZoneId`,`ClockId`,`TimezoneId`),
  CONSTRAINT `FK_Zones_Timezones_Zones` FOREIGN KEY (`ZoneId`) REFERENCES `zones` (`Id`) ON DELETE RESTRICT
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `zones_timezones`
--

LOCK TABLES `zones_timezones` WRITE;
/*!40000 ALTER TABLE `zones_timezones` DISABLE KEYS */;
/*!40000 ALTER TABLE `zones_timezones` ENABLE KEYS */;
UNLOCK TABLES;
/*!40103 SET TIME_ZONE=@OLD_TIME_ZONE */;

/*!40101 SET SQL_MODE=@OLD_SQL_MODE */;
/*!40014 SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS */;
/*!40014 SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS */;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
/*!40111 SET SQL_NOTES=@OLD_SQL_NOTES */;

-- Dump completed on 2026-04-03 10:34:31
