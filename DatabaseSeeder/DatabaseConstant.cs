namespace DatabaseSeeder;

internal class DatabaseConstant
{
	public const string DatabaseSQL = @"CREATE DATABASE IF NOT EXISTS `demo_dbo` /*!40100 DEFAULT CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci */ /*!80016 DEFAULT ENCRYPTION='N' */;
USE `demo_dbo`;

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
-- Table structure for table `__EFMigrationsHistory`
--

DROP TABLE IF EXISTS `__EFMigrationsHistory`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `__EFMigrationsHistory` (
  `MigrationId` varchar(150) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `ProductVersion` varchar(32) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  PRIMARY KEY (`MigrationId`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `__EFMigrationsHistory`
--

LOCK TABLES `__EFMigrationsHistory` WRITE;
/*!40000 ALTER TABLE `__EFMigrationsHistory` DISABLE KEYS */;
INSERT INTO `__EFMigrationsHistory` VALUES ('20200626070704_InitialDatabase','8.0.7'),('20200728125151_MoveChargenToTables','8.0.7'),('20200807044450_EnforcementUpdate','8.0.7'),('20200810141606_ClanVoting','8.0.7'),('20200817061844_Elections','8.0.7'),('20200830233741_TerrainUpdate','8.0.7'),('20200905062837_CurrencyPatternEnhancement','8.0.7'),('20200928025908_KnowledgeBuilding','8.0.7'),('20201013213328_CheckFixing','8.0.7'),('20201014230837_FixingEmailTemplates','8.0.7'),('20201106014706_LineOfCreditAccounts','8.0.7'),('20201106040133_AttributesUpdate','8.0.7'),('20201108122141_EconomicZoneUpdate','8.0.7'),('20201113050353_EconomicZonesTouchup','8.0.7'),('20201120022913_EnforcermentAndMisc','8.0.7'),('20201120045951_MinorFixForCrime','8.0.7'),('20201129225407_SafeQuit','8.0.7'),('20201130014025_JournalUpdates','8.0.7'),('20201130041538_JournalUpdate','8.0.7'),('20201201052916_DrugReform','8.0.7'),('20201217051236_Changes','8.0.7'),('20201217051726_ExtraDescriptions','8.0.7'),('20201218014631_RacialBreathingChange','8.0.7'),('20201221031703_ClanFame','8.0.7'),('20201227120935_CantRemember','8.0.7'),('20210113052107_IndexFixForBodyparts','8.0.7'),('20210114010706_IndexAdditions','8.0.7'),('20210116210204_MagicSpells','8.0.7'),('20210118053537_MoreSpellStuff','8.0.7'),('20210119034150_MoreSpellStuff2','8.0.7'),('20210119035740_MoreSpellStuff3','8.0.7'),('20210120031933_MoreSpellStuff4','8.0.7'),('20210127032929_Jan21EnforcementWorkaround','8.0.7'),('20210202002906_RemovingChildClans','8.0.7'),('20210211035327_GameStatistics','8.0.7'),('20210224105856_NewSun','8.0.7'),('20210302112347_OngoingCheckForCharacteristics','8.0.7'),('20210331025006_BanksV1','8.0.7'),('20210423014825_WeaponAttackAddPositionRequirement','8.0.7'),('20210626110830_AuctionHouses','8.0.7'),('20210810123837_PropertyV1','8.0.7'),('20210902052233_PropertyV2','8.0.7'),('20210914132733_Sep21LawUpdate','8.0.7'),('20211025020630_JusticeOverhaulOct21','8.0.7'),('20211217034326_TerrainMapColourAddition','8.0.7'),('20211220045847_Skins','8.0.7'),('20211222033658_Skins-Pt2','8.0.7'),('20211226134159_ClanBankAccounts','8.0.7'),('20211229004501_PlayerBoards','8.0.7'),('20220104134109_JobsV1','8.0.7'),('20220105004035_ProjectsJobsUpdate','8.0.7'),('20220108004307_BoardBugFix','8.0.7'),('20220117102755_BoardsDescriptions','8.0.7'),('20220210215752_LongerAuthorFullDescs','8.0.7'),('20220225125641_ClanFKFixing','8.0.7'),('20220327052829_NPCSpawners','8.0.7'),('20220421132846_BodyCharacteristicsFix','8.0.7'),('20220625122517_ClanDiscordUpdate','8.0.7'),('20220625125136_ChargenResourcesAsDouble','8.0.7'),('20220718132632_MaterialsRefactor','8.0.7'),('20220731064708_TheoreticalCraftChecks','8.0.7'),('20220807101509_IntToDoubleParryBonus','8.0.7'),('20220814231930_RaceDefaultHwModels','8.0.7'),('20221030044209_ShopBankAccountsAndFinance','8.0.7'),('20221030125929_BankPaymentsAtShops','8.0.7'),('20221031113757_MagicSpellExclusivity','8.0.7'),('20221201081057_NameCulturesGenderExpansion','8.0.7'),('20221201133628_NameCulturesChargenExpansion','8.0.7'),('20230101133831_CurrencyPatternRegexCaseFix','8.0.7'),('20230110120837_RelativeEnthalpyForLiquids','8.0.7'),('20230124124618_SurgicalProcedureCheckTraits','8.0.7'),('20230311060208_SurgeryBodyUpdate','8.0.7'),('20230407151210_OpenAIv1','8.0.7'),('20230428004425_GlobalCurrencyChanges','8.0.7'),('20230603125906_CraftUseToolDuration','8.0.7'),('20230706055610_AuxiliaryMoves','8.0.7'),('20230714035824_AuxiliaryMoves2','8.0.7'),('20230727121209_SeasonsDisplayUpdate','8.0.7'),('20230731055842_SeederChoices','8.0.7'),('20230810071403_CellForeignKeyUpdates','8.0.7'),('20230825052231_NpcSpawnerMulti','8.0.7'),('20230825061651_ShopTypes','8.0.7'),('20230914142042_ScriptedEvents','8.0.7'),('20230917131132_ClanForeignKeyUpdate','8.0.7'),('20231031085439_MagicResourceColours','8.0.7'),('20231102120820_NewPlayerHints','8.0.7'),('20231110224309_HungerThirstRatesForRaces','8.0.7'),('20231125084220_ClimateModelSimplification','8.0.7'),('20231208235024_HeritageChargenCostBugFix','8.0.7'),('20240112055830_ChargenResourcesControlProg','8.0.7'),('20240119120217_CoinsChangeFlag','8.0.7'),('20240129025113_CurrencyForeignKeyUpdateJan24','8.0.7'),('20240305110906_BuyingMerchandise','8.0.7'),('20240325104238_MarketsV1','8.0.7'),('20240418112441_MarketsShopIntegration','8.0.7'),('20240427013621_MarketPopulations','8.0.7'),('20240601141550_DiscordOutputForChannels','8.0.7'),('20240615065145_ShopAutopayTaxes','8.0.7'),('20240730123726_TrackingV1','8.0.7'),('20240804070126_FixDatabaseAutoTrueBug','8.0.7'),('20240808232211_TrackingNameForBodyProtos','8.0.7'),('20240809155707_BMIUnits','8.0.7'),('20240816134208_ArmourPenaltyToDouble','8.0.7'),('20240817112644_HeightWeightModelDirectSetWeights','8.0.7'),('20240828105208_AutoReacquireTargetsSetting','8.0.7'),('20240828124859_CombatSettingsAugust24','8.0.7'),('20240831005804_BodypartGroupDescribersAugust2024','8.0.7'),('20240925062238_CrimesUpdate2024Sep25','8.0.7'),('20241011073405_StockroomNonMorphing','8.0.7'),('20241016054103_ItemProtoIsHiddenFromPlayers','8.0.7'),('20241016123415_BodyOverrideHealthStrategy','8.0.7'),('20241018071518_LiquidLeaveResidueInRooms','8.0.7'),('20241121010653_RemovingBreathableFluidsRaces','8.0.7'),('20241129002416_CriminalDescUpdates','8.0.7'),('20241216062012_RandomNamesBinaryUnicodeSort','8.0.7'),('20241220091815_EthnicitiesNameCultures','8.0.7');
/*!40000 ALTER TABLE `__EFMigrationsHistory` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `Accents`
--

DROP TABLE IF EXISTS `Accents`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `Accents` (
  `Id` bigint NOT NULL AUTO_INCREMENT,
  `LanguageId` bigint NOT NULL,
  `Name` varchar(50) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
  `Suffix` varchar(4000) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
  `VagueSuffix` varchar(4000) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
  `Difficulty` int NOT NULL,
  `Description` varchar(4000) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
  `Group` varchar(50) CHARACTER SET utf8 COLLATE utf8_general_ci DEFAULT NULL,
  `ChargenAvailabilityProgId` bigint DEFAULT NULL,
  PRIMARY KEY (`Id`),
  KEY `FK_Accents_Languages` (`LanguageId`),
  CONSTRAINT `FK_Accents_Languages` FOREIGN KEY (`LanguageId`) REFERENCES `Languages` (`Id`) ON DELETE RESTRICT
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `Accents`
--

LOCK TABLES `Accents` WRITE;
/*!40000 ALTER TABLE `Accents` DISABLE KEYS */;
/*!40000 ALTER TABLE `Accents` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `AccountNotes`
--

DROP TABLE IF EXISTS `AccountNotes`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `AccountNotes` (
  `Id` bigint NOT NULL AUTO_INCREMENT,
  `AccountId` bigint NOT NULL,
  `Text` text CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
  `Subject` varchar(4000) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
  `TimeStamp` datetime NOT NULL,
  `AuthorId` bigint DEFAULT NULL,
  `InGameTimeStamp` varchar(200) CHARACTER SET utf8 COLLATE utf8_general_ci DEFAULT NULL,
  `IsJournalEntry` bit(1) NOT NULL DEFAULT b'0',
  `CharacterId` bigint DEFAULT NULL,
  PRIMARY KEY (`Id`),
  KEY `FK_AccountNotes_Accounts` (`AccountId`),
  KEY `FK_AccountNotes_Author` (`AuthorId`),
  KEY `FK_AccountNotes_Characters_idx` (`CharacterId`),
  CONSTRAINT `FK_AccountNotes_Accounts` FOREIGN KEY (`AccountId`) REFERENCES `Accounts` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_AccountNotes_Author` FOREIGN KEY (`AuthorId`) REFERENCES `Accounts` (`Id`) ON DELETE RESTRICT,
  CONSTRAINT `FK_AccountNotes_Characters` FOREIGN KEY (`CharacterId`) REFERENCES `Characters` (`Id`) ON DELETE RESTRICT
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `AccountNotes`
--

LOCK TABLES `AccountNotes` WRITE;
/*!40000 ALTER TABLE `AccountNotes` DISABLE KEYS */;
/*!40000 ALTER TABLE `AccountNotes` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `Accounts`
--

DROP TABLE IF EXISTS `Accounts`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `Accounts` (
  `Id` bigint NOT NULL AUTO_INCREMENT,
  `Name` varchar(4000) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
  `Password` varchar(1000) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `Salt` bigint NOT NULL,
  `AccessStatus` int NOT NULL,
  `AuthorityGroupId` bigint DEFAULT '0',
  `Email` varchar(4000) CHARACTER SET utf8 COLLATE utf8_general_ci DEFAULT NULL,
  `LastLoginTime` datetime DEFAULT NULL,
  `LastLoginIP` varchar(50) CHARACTER SET utf8 COLLATE utf8_general_ci DEFAULT NULL,
  `FormatLength` int NOT NULL DEFAULT '110',
  `InnerFormatLength` int NOT NULL DEFAULT '80',
  `UseMXP` bit(1) NOT NULL DEFAULT b'0',
  `UseMSP` bit(1) NOT NULL DEFAULT b'0',
  `UseMCCP` bit(1) NOT NULL DEFAULT b'0',
  `ActiveCharactersAllowed` int NOT NULL DEFAULT '1',
  `UseUnicode` bit(1) NOT NULL DEFAULT b'0',
  `TimeZoneId` varchar(100) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
  `CultureName` varchar(50) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
  `RegistrationCode` varchar(50) CHARACTER SET utf8 COLLATE utf8_general_ci DEFAULT NULL,
  `IsRegistered` bit(1) NOT NULL DEFAULT b'0',
  `RecoveryCode` varchar(4000) CHARACTER SET utf8 COLLATE utf8_general_ci DEFAULT NULL,
  `UnitPreference` varchar(50) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
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
  CONSTRAINT `FK_Accounts_AuthorityGroups` FOREIGN KEY (`AuthorityGroupId`) REFERENCES `AuthorityGroups` (`Id`) ON DELETE RESTRICT
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `Accounts`
--

LOCK TABLES `Accounts` WRITE;
/*!40000 ALTER TABLE `Accounts` DISABLE KEYS */;
/*!40000 ALTER TABLE `Accounts` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `Accounts_ChargenResources`
--

DROP TABLE IF EXISTS `Accounts_ChargenResources`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `Accounts_ChargenResources` (
  `AccountId` bigint NOT NULL,
  `ChargenResourceId` bigint NOT NULL,
  `Amount` double NOT NULL,
  `LastAwardDate` datetime NOT NULL,
  PRIMARY KEY (`AccountId`,`ChargenResourceId`),
  KEY `FK_Accounts_ChargenResources_ChargenResources` (`ChargenResourceId`),
  CONSTRAINT `FK_Accounts_ChargenResources_Accounts` FOREIGN KEY (`AccountId`) REFERENCES `Accounts` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_Accounts_ChargenResources_ChargenResources` FOREIGN KEY (`ChargenResourceId`) REFERENCES `ChargenResources` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `Accounts_ChargenResources`
--

LOCK TABLES `Accounts_ChargenResources` WRITE;
/*!40000 ALTER TABLE `Accounts_ChargenResources` DISABLE KEYS */;
/*!40000 ALTER TABLE `Accounts_ChargenResources` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `ActiveJobs`
--

DROP TABLE IF EXISTS `ActiveJobs`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `ActiveJobs` (
  `Id` bigint NOT NULL AUTO_INCREMENT,
  `JobListingId` bigint NOT NULL,
  `CharacterId` bigint NOT NULL,
  `JobCommenced` varchar(200) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
  `JobDueToEnd` varchar(200) CHARACTER SET utf8 COLLATE utf8_general_ci DEFAULT NULL,
  `JobEnded` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  `IsJobComplete` bit(1) NOT NULL,
  `AlreadyHadClanPosition` bit(1) NOT NULL,
  `BackpayOwed` text CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
  `RevenueEarned` text CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
  `CurrentPerformance` double NOT NULL,
  `ActiveProjectId` bigint DEFAULT NULL,
  PRIMARY KEY (`Id`),
  KEY `IX_ActiveJobs_ActiveProjectId` (`ActiveProjectId`),
  KEY `IX_ActiveJobs_CharacterId` (`CharacterId`),
  KEY `IX_ActiveJobs_JobListingId` (`JobListingId`),
  CONSTRAINT `FK_ActiveJobs_ActiveProjects` FOREIGN KEY (`ActiveProjectId`) REFERENCES `ActiveProjects` (`Id`),
  CONSTRAINT `FK_ActiveJobs_Characters` FOREIGN KEY (`CharacterId`) REFERENCES `Characters` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_ActiveJobs_JobListings` FOREIGN KEY (`JobListingId`) REFERENCES `JobListings` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `ActiveJobs`
--

LOCK TABLES `ActiveJobs` WRITE;
/*!40000 ALTER TABLE `ActiveJobs` DISABLE KEYS */;
/*!40000 ALTER TABLE `ActiveJobs` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `ActiveProjectLabours`
--

DROP TABLE IF EXISTS `ActiveProjectLabours`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `ActiveProjectLabours` (
  `ActiveProjectId` bigint NOT NULL,
  `ProjectLabourRequirementsId` bigint NOT NULL,
  `Progress` double NOT NULL,
  PRIMARY KEY (`ActiveProjectId`,`ProjectLabourRequirementsId`),
  KEY `FK_ActiveProjectLabours_ProjectLabourRequirements_idx` (`ProjectLabourRequirementsId`),
  CONSTRAINT `FK_ActiveProjectLabours_ActiveProjects` FOREIGN KEY (`ActiveProjectId`) REFERENCES `ActiveProjects` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_ActiveProjectLabours_ProjectLabourRequirements` FOREIGN KEY (`ProjectLabourRequirementsId`) REFERENCES `ProjectLabourRequirements` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `ActiveProjectLabours`
--

LOCK TABLES `ActiveProjectLabours` WRITE;
/*!40000 ALTER TABLE `ActiveProjectLabours` DISABLE KEYS */;
/*!40000 ALTER TABLE `ActiveProjectLabours` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `ActiveProjectMaterials`
--

DROP TABLE IF EXISTS `ActiveProjectMaterials`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `ActiveProjectMaterials` (
  `ActiveProjectId` bigint NOT NULL,
  `ProjectMaterialRequirementsId` bigint NOT NULL,
  `Progress` double NOT NULL,
  PRIMARY KEY (`ActiveProjectId`,`ProjectMaterialRequirementsId`),
  KEY `FK_ActiveProjectMaterials_ProjectMaterialRequirements_idx` (`ProjectMaterialRequirementsId`),
  CONSTRAINT `FK_ActiveProjectMaterials_ActiveProjects` FOREIGN KEY (`ActiveProjectId`) REFERENCES `ActiveProjects` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_ActiveProjectMaterials_ProjectMaterialRequirements` FOREIGN KEY (`ProjectMaterialRequirementsId`) REFERENCES `ProjectMaterialRequirements` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `ActiveProjectMaterials`
--

LOCK TABLES `ActiveProjectMaterials` WRITE;
/*!40000 ALTER TABLE `ActiveProjectMaterials` DISABLE KEYS */;
/*!40000 ALTER TABLE `ActiveProjectMaterials` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `ActiveProjects`
--

DROP TABLE IF EXISTS `ActiveProjects`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `ActiveProjects` (
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
  CONSTRAINT `FK_ActiveProjects_Cells` FOREIGN KEY (`CellId`) REFERENCES `Cells` (`Id`) ON DELETE RESTRICT,
  CONSTRAINT `FK_ActiveProjects_Characters` FOREIGN KEY (`CharacterId`) REFERENCES `Characters` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_ActiveProjects_ProjectPhases` FOREIGN KEY (`CurrentPhaseId`) REFERENCES `ProjectPhases` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_ActiveProjects_Projects` FOREIGN KEY (`ProjectId`, `ProjectRevisionNumber`) REFERENCES `Projects` (`Id`, `RevisionNumber`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `ActiveProjects`
--

LOCK TABLES `ActiveProjects` WRITE;
/*!40000 ALTER TABLE `ActiveProjects` DISABLE KEYS */;
/*!40000 ALTER TABLE `ActiveProjects` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `Allies`
--

DROP TABLE IF EXISTS `Allies`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `Allies` (
  `CharacterId` bigint NOT NULL,
  `AllyId` bigint NOT NULL,
  `Trusted` bit(1) NOT NULL DEFAULT b'0',
  PRIMARY KEY (`CharacterId`,`AllyId`),
  KEY `FK_Allies_Characters_Target_idx` (`AllyId`),
  CONSTRAINT `FK_Allies_Characters_Owner` FOREIGN KEY (`CharacterId`) REFERENCES `Characters` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_Allies_Characters_Target` FOREIGN KEY (`AllyId`) REFERENCES `Characters` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `Allies`
--

LOCK TABLES `Allies` WRITE;
/*!40000 ALTER TABLE `Allies` DISABLE KEYS */;
/*!40000 ALTER TABLE `Allies` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `AmmunitionTypes`
--

DROP TABLE IF EXISTS `AmmunitionTypes`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `AmmunitionTypes` (
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
-- Dumping data for table `AmmunitionTypes`
--

LOCK TABLES `AmmunitionTypes` WRITE;
/*!40000 ALTER TABLE `AmmunitionTypes` DISABLE KEYS */;
/*!40000 ALTER TABLE `AmmunitionTypes` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `Appointments`
--

DROP TABLE IF EXISTS `Appointments`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `Appointments` (
  `Id` bigint NOT NULL AUTO_INCREMENT,
  `Name` varchar(100) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
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
  CONSTRAINT `FK_Appointments_CanNominateProg` FOREIGN KEY (`CanNominateProgId`) REFERENCES `FutureProgs` (`Id`) ON DELETE RESTRICT,
  CONSTRAINT `FK_Appointments_Clans` FOREIGN KEY (`ClanId`) REFERENCES `Clans` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_Appointments_GameItemProtos` FOREIGN KEY (`InsigniaGameItemId`, `InsigniaGameItemRevnum`) REFERENCES `GameItemProtos` (`Id`, `RevisionNumber`) ON DELETE RESTRICT,
  CONSTRAINT `FK_Appointments_NumberOfVotesProg` FOREIGN KEY (`NumberOfVotesProgId`) REFERENCES `FutureProgs` (`Id`) ON DELETE RESTRICT,
  CONSTRAINT `FK_Appointments_ParentAppointment` FOREIGN KEY (`ParentAppointmentId`) REFERENCES `Appointments` (`Id`) ON DELETE RESTRICT,
  CONSTRAINT `FK_Appointments_Paygrades` FOREIGN KEY (`PaygradeId`) REFERENCES `Paygrades` (`Id`) ON DELETE SET NULL,
  CONSTRAINT `FK_Appointments_Ranks` FOREIGN KEY (`MinimumRankId`) REFERENCES `Ranks` (`Id`) ON DELETE SET NULL,
  CONSTRAINT `FK_Appointments_Ranks_2` FOREIGN KEY (`MinimumRankToAppointId`) REFERENCES `Ranks` (`Id`) ON DELETE RESTRICT,
  CONSTRAINT `FK_Appointments_WhyCantNominateProg` FOREIGN KEY (`WhyCantNominateProgId`) REFERENCES `FutureProgs` (`Id`) ON DELETE RESTRICT
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `Appointments`
--

LOCK TABLES `Appointments` WRITE;
/*!40000 ALTER TABLE `Appointments` DISABLE KEYS */;
/*!40000 ALTER TABLE `Appointments` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `Appointments_Abbreviations`
--

DROP TABLE IF EXISTS `Appointments_Abbreviations`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `Appointments_Abbreviations` (
  `AppointmentId` bigint NOT NULL,
  `Abbreviation` varchar(50) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
  `FutureProgId` bigint DEFAULT NULL,
  `Order` int NOT NULL,
  PRIMARY KEY (`Abbreviation`,`AppointmentId`),
  KEY `FK_Appointments_Abbreviations_Appointments` (`AppointmentId`),
  KEY `FK_Appointments_Abbreviations_FutureProgs` (`FutureProgId`),
  CONSTRAINT `FK_Appointments_Abbreviations_Appointments` FOREIGN KEY (`AppointmentId`) REFERENCES `Appointments` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_Appointments_Abbreviations_FutureProgs` FOREIGN KEY (`FutureProgId`) REFERENCES `FutureProgs` (`Id`) ON DELETE RESTRICT
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `Appointments_Abbreviations`
--

LOCK TABLES `Appointments_Abbreviations` WRITE;
/*!40000 ALTER TABLE `Appointments_Abbreviations` DISABLE KEYS */;
/*!40000 ALTER TABLE `Appointments_Abbreviations` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `Appointments_Titles`
--

DROP TABLE IF EXISTS `Appointments_Titles`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `Appointments_Titles` (
  `AppointmentId` bigint NOT NULL,
  `Title` varchar(200) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
  `FutureProgId` bigint DEFAULT NULL,
  `Order` int DEFAULT NULL,
  PRIMARY KEY (`Title`,`AppointmentId`),
  KEY `FK_Appointments_Titles_Appointments` (`AppointmentId`),
  KEY `FK_Appointments_Titles_FutureProgs` (`FutureProgId`),
  CONSTRAINT `FK_Appointments_Titles_Appointments` FOREIGN KEY (`AppointmentId`) REFERENCES `Appointments` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_Appointments_Titles_FutureProgs` FOREIGN KEY (`FutureProgId`) REFERENCES `FutureProgs` (`Id`) ON DELETE RESTRICT
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `Appointments_Titles`
--

LOCK TABLES `Appointments_Titles` WRITE;
/*!40000 ALTER TABLE `Appointments_Titles` DISABLE KEYS */;
/*!40000 ALTER TABLE `Appointments_Titles` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `Areas`
--

DROP TABLE IF EXISTS `Areas`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `Areas` (
  `Id` bigint NOT NULL AUTO_INCREMENT,
  `Name` varchar(200) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
  `WeatherControllerId` bigint DEFAULT NULL,
  PRIMARY KEY (`Id`),
  KEY `FK_Areas_WeatherControllers_idx` (`WeatherControllerId`),
  CONSTRAINT `FK_Areas_WeatherControllers` FOREIGN KEY (`WeatherControllerId`) REFERENCES `WeatherControllers` (`Id`) ON DELETE SET NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `Areas`
--

LOCK TABLES `Areas` WRITE;
/*!40000 ALTER TABLE `Areas` DISABLE KEYS */;
/*!40000 ALTER TABLE `Areas` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `Areas_Rooms`
--

DROP TABLE IF EXISTS `Areas_Rooms`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `Areas_Rooms` (
  `AreaId` bigint NOT NULL,
  `RoomId` bigint NOT NULL,
  PRIMARY KEY (`AreaId`,`RoomId`),
  KEY `FK_Areas_Rooms_Rooms_idx` (`RoomId`),
  CONSTRAINT `FK_Areas_Rooms_Areas` FOREIGN KEY (`AreaId`) REFERENCES `Areas` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_Areas_Rooms_Rooms` FOREIGN KEY (`RoomId`) REFERENCES `Rooms` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `Areas_Rooms`
--

LOCK TABLES `Areas_Rooms` WRITE;
/*!40000 ALTER TABLE `Areas_Rooms` DISABLE KEYS */;
/*!40000 ALTER TABLE `Areas_Rooms` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `ArmourTypes`
--

DROP TABLE IF EXISTS `ArmourTypes`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `ArmourTypes` (
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
-- Dumping data for table `ArmourTypes`
--

LOCK TABLES `ArmourTypes` WRITE;
/*!40000 ALTER TABLE `ArmourTypes` DISABLE KEYS */;
/*!40000 ALTER TABLE `ArmourTypes` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `ArtificialIntelligences`
--

DROP TABLE IF EXISTS `ArtificialIntelligences`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `ArtificialIntelligences` (
  `Id` bigint NOT NULL AUTO_INCREMENT,
  `Name` varchar(4000) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
  `Type` varchar(4000) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
  `Definition` mediumtext CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `ArtificialIntelligences`
--

LOCK TABLES `ArtificialIntelligences` WRITE;
/*!40000 ALTER TABLE `ArtificialIntelligences` DISABLE KEYS */;
/*!40000 ALTER TABLE `ArtificialIntelligences` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `AuctionHouses`
--

DROP TABLE IF EXISTS `AuctionHouses`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `AuctionHouses` (
  `Id` bigint NOT NULL AUTO_INCREMENT,
  `Name` varchar(250) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
  `EconomicZoneId` bigint NOT NULL,
  `AuctionHouseCellId` bigint NOT NULL,
  `ProfitsBankAccountId` bigint NOT NULL,
  `AuctionListingFeeFlat` decimal(58,29) NOT NULL,
  `AuctionListingFeeRate` decimal(58,29) NOT NULL,
  `Definition` mediumtext CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
  `DefaultListingTime` double NOT NULL,
  PRIMARY KEY (`Id`),
  KEY `IX_AuctionHouses_AuctionHouseCellId` (`AuctionHouseCellId`),
  KEY `IX_AuctionHouses_EconomicZoneId` (`EconomicZoneId`),
  KEY `IX_AuctionHouses_ProfitsBankAccountId` (`ProfitsBankAccountId`),
  CONSTRAINT `FK_AuctionHouses_BankAccounts` FOREIGN KEY (`ProfitsBankAccountId`) REFERENCES `BankAccounts` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_AuctionHouses_Cells` FOREIGN KEY (`AuctionHouseCellId`) REFERENCES `Cells` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_AuctionHouses_EconomicZones` FOREIGN KEY (`EconomicZoneId`) REFERENCES `EconomicZones` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `AuctionHouses`
--

LOCK TABLES `AuctionHouses` WRITE;
/*!40000 ALTER TABLE `AuctionHouses` DISABLE KEYS */;
/*!40000 ALTER TABLE `AuctionHouses` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `AuthorityGroups`
--

DROP TABLE IF EXISTS `AuthorityGroups`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `AuthorityGroups` (
  `Id` bigint NOT NULL AUTO_INCREMENT,
  `Name` varchar(4000) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
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
-- Dumping data for table `AuthorityGroups`
--

LOCK TABLES `AuthorityGroups` WRITE;
/*!40000 ALTER TABLE `AuthorityGroups` DISABLE KEYS */;
/*!40000 ALTER TABLE `AuthorityGroups` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `AutobuilderAreaTemplates`
--

DROP TABLE IF EXISTS `AutobuilderAreaTemplates`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `AutobuilderAreaTemplates` (
  `ID` bigint NOT NULL AUTO_INCREMENT,
  `Name` varchar(100) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
  `TemplateType` varchar(45) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
  `Definition` longtext CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
  PRIMARY KEY (`ID`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `AutobuilderAreaTemplates`
--

LOCK TABLES `AutobuilderAreaTemplates` WRITE;
/*!40000 ALTER TABLE `AutobuilderAreaTemplates` DISABLE KEYS */;
/*!40000 ALTER TABLE `AutobuilderAreaTemplates` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `AutobuilderRoomTemplates`
--

DROP TABLE IF EXISTS `AutobuilderRoomTemplates`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `AutobuilderRoomTemplates` (
  `ID` bigint NOT NULL AUTO_INCREMENT,
  `Name` varchar(100) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
  `TemplateType` varchar(45) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
  `Definition` longtext CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
  PRIMARY KEY (`ID`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `AutobuilderRoomTemplates`
--

LOCK TABLES `AutobuilderRoomTemplates` WRITE;
/*!40000 ALTER TABLE `AutobuilderRoomTemplates` DISABLE KEYS */;
/*!40000 ALTER TABLE `AutobuilderRoomTemplates` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `BankAccounts`
--

DROP TABLE IF EXISTS `BankAccounts`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `BankAccounts` (
  `Id` bigint NOT NULL AUTO_INCREMENT,
  `Name` varchar(255) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
  `AccountNumber` int NOT NULL,
  `BankId` bigint NOT NULL,
  `BankAccountTypeId` bigint NOT NULL,
  `CurrentBalance` decimal(58,29) NOT NULL,
  `AccountOwnerCharacterId` bigint DEFAULT NULL,
  `AccountOwnerClanId` bigint DEFAULT NULL,
  `AccountOwnerShopId` bigint DEFAULT NULL,
  `NominatedBenefactorAccountId` bigint DEFAULT NULL,
  `AccountCreationDate` varchar(255) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
  `AccountStatus` int NOT NULL,
  `CurrentMonthInterest` decimal(58,29) NOT NULL,
  `CurrentMonthFees` decimal(58,29) NOT NULL,
  `AuthorisedBankPaymentItems` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  PRIMARY KEY (`Id`),
  KEY `IX_BankAccounts_AccountOwnerCharacterId` (`AccountOwnerCharacterId`),
  KEY `IX_BankAccounts_AccountOwnerClanId` (`AccountOwnerClanId`),
  KEY `IX_BankAccounts_AccountOwnerShopId` (`AccountOwnerShopId`),
  KEY `IX_BankAccounts_BankAccountTypeId` (`BankAccountTypeId`),
  KEY `IX_BankAccounts_BankId` (`BankId`),
  KEY `IX_BankAccounts_NominatedBenefactorAccountId` (`NominatedBenefactorAccountId`),
  CONSTRAINT `FK_BankAccounts_BankAccounts` FOREIGN KEY (`NominatedBenefactorAccountId`) REFERENCES `BankAccounts` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_BankAccounts_BankAccountTypes` FOREIGN KEY (`BankAccountTypeId`) REFERENCES `BankAccountTypes` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_BankAccounts_Banks` FOREIGN KEY (`BankId`) REFERENCES `Banks` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_BankAccounts_Characters` FOREIGN KEY (`AccountOwnerCharacterId`) REFERENCES `Characters` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_BankAccounts_Clans` FOREIGN KEY (`AccountOwnerClanId`) REFERENCES `Clans` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_BankAccounts_Shops` FOREIGN KEY (`AccountOwnerShopId`) REFERENCES `Shops` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `BankAccounts`
--

LOCK TABLES `BankAccounts` WRITE;
/*!40000 ALTER TABLE `BankAccounts` DISABLE KEYS */;
/*!40000 ALTER TABLE `BankAccounts` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `BankAccountTransactions`
--

DROP TABLE IF EXISTS `BankAccountTransactions`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `BankAccountTransactions` (
  `Id` bigint NOT NULL AUTO_INCREMENT,
  `BankAccountId` bigint NOT NULL,
  `TransactionType` int NOT NULL,
  `Amount` decimal(58,29) NOT NULL,
  `TransactionTime` varchar(255) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
  `TransactionDescription` varchar(255) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
  `AccountBalanceAfter` decimal(58,29) NOT NULL,
  PRIMARY KEY (`Id`),
  KEY `IX_BankAccountTransactions_BankAccountId` (`BankAccountId`),
  CONSTRAINT `FK_BankAccountTransactions_BankAccounts` FOREIGN KEY (`BankAccountId`) REFERENCES `BankAccounts` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `BankAccountTransactions`
--

LOCK TABLES `BankAccountTransactions` WRITE;
/*!40000 ALTER TABLE `BankAccountTransactions` DISABLE KEYS */;
/*!40000 ALTER TABLE `BankAccountTransactions` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `BankAccountTypes`
--

DROP TABLE IF EXISTS `BankAccountTypes`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `BankAccountTypes` (
  `Id` bigint NOT NULL AUTO_INCREMENT,
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
  CONSTRAINT `FK_BankAccountTypes_Banks` FOREIGN KEY (`BankId`) REFERENCES `Banks` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_BankAccountTypes_CanCloseProg` FOREIGN KEY (`CanCloseAccountProgId`) REFERENCES `FutureProgs` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_BankAccountTypes_CharacterProgs` FOREIGN KEY (`CanOpenAccountProgCharacterId`) REFERENCES `FutureProgs` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_BankAccountTypes_ClanProgs` FOREIGN KEY (`CanOpenAccountProgClanId`) REFERENCES `FutureProgs` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_BankAccountTypes_ShopProgs` FOREIGN KEY (`CanOpenAccountProgShopId`) REFERENCES `FutureProgs` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `BankAccountTypes`
--

LOCK TABLES `BankAccountTypes` WRITE;
/*!40000 ALTER TABLE `BankAccountTypes` DISABLE KEYS */;
/*!40000 ALTER TABLE `BankAccountTypes` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `BankBranches`
--

DROP TABLE IF EXISTS `BankBranches`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `BankBranches` (
  `BankId` bigint NOT NULL,
  `CellId` bigint NOT NULL,
  PRIMARY KEY (`BankId`,`CellId`),
  KEY `IX_BankBranches_CellId` (`CellId`),
  CONSTRAINT `FK_BankBranches_Banks` FOREIGN KEY (`BankId`) REFERENCES `Banks` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_BankBranches_Cells` FOREIGN KEY (`CellId`) REFERENCES `Cells` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `BankBranches`
--

LOCK TABLES `BankBranches` WRITE;
/*!40000 ALTER TABLE `BankBranches` DISABLE KEYS */;
/*!40000 ALTER TABLE `BankBranches` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `BankCurrencyReserves`
--

DROP TABLE IF EXISTS `BankCurrencyReserves`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `BankCurrencyReserves` (
  `BankId` bigint NOT NULL,
  `CurrencyId` bigint NOT NULL,
  `Amount` decimal(58,29) NOT NULL,
  PRIMARY KEY (`BankId`,`CurrencyId`),
  KEY `IX_BankCurrencyReserves_CurrencyId` (`CurrencyId`),
  CONSTRAINT `FK_BankCurrencyReserves_Banks` FOREIGN KEY (`BankId`) REFERENCES `Banks` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_BankCurrencyReserves_Currencies` FOREIGN KEY (`CurrencyId`) REFERENCES `Currencies` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `BankCurrencyReserves`
--

LOCK TABLES `BankCurrencyReserves` WRITE;
/*!40000 ALTER TABLE `BankCurrencyReserves` DISABLE KEYS */;
/*!40000 ALTER TABLE `BankCurrencyReserves` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `BankExchangeRates`
--

DROP TABLE IF EXISTS `BankExchangeRates`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `BankExchangeRates` (
  `BankId` bigint NOT NULL,
  `FromCurrencyId` bigint NOT NULL,
  `ToCurrencyId` bigint NOT NULL,
  `ExchangeRate` decimal(58,29) NOT NULL,
  PRIMARY KEY (`BankId`,`FromCurrencyId`,`ToCurrencyId`),
  KEY `IX_BankExchangeRates_FromCurrencyId` (`FromCurrencyId`),
  KEY `IX_BankExchangeRates_ToCurrencyId` (`ToCurrencyId`),
  CONSTRAINT `FK_BankExchangeRates_Banks` FOREIGN KEY (`BankId`) REFERENCES `Banks` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_BankExchangeRates_Currencies_From` FOREIGN KEY (`FromCurrencyId`) REFERENCES `Currencies` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_BankExchangeRates_Currencies_To` FOREIGN KEY (`ToCurrencyId`) REFERENCES `Currencies` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `BankExchangeRates`
--

LOCK TABLES `BankExchangeRates` WRITE;
/*!40000 ALTER TABLE `BankExchangeRates` DISABLE KEYS */;
/*!40000 ALTER TABLE `BankExchangeRates` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `BankManagerAuditLogs`
--

DROP TABLE IF EXISTS `BankManagerAuditLogs`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `BankManagerAuditLogs` (
  `Id` bigint NOT NULL AUTO_INCREMENT,
  `BankId` bigint NOT NULL,
  `CharacterId` bigint NOT NULL,
  `DateTime` varchar(255) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
  `Detail` varchar(1000) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
  PRIMARY KEY (`Id`),
  KEY `IX_BankManagerAuditLogs_BankId` (`BankId`),
  KEY `IX_BankManagerAuditLogs_CharacterId` (`CharacterId`),
  CONSTRAINT `FK_BankManagerAuditLogs_Banks` FOREIGN KEY (`BankId`) REFERENCES `Banks` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_BankManagerAuditLogs_Characters` FOREIGN KEY (`CharacterId`) REFERENCES `Characters` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `BankManagerAuditLogs`
--

LOCK TABLES `BankManagerAuditLogs` WRITE;
/*!40000 ALTER TABLE `BankManagerAuditLogs` DISABLE KEYS */;
/*!40000 ALTER TABLE `BankManagerAuditLogs` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `BankManagers`
--

DROP TABLE IF EXISTS `BankManagers`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `BankManagers` (
  `BankId` bigint NOT NULL,
  `CharacterId` bigint NOT NULL,
  PRIMARY KEY (`BankId`,`CharacterId`),
  KEY `IX_BankManagers_CharacterId` (`CharacterId`),
  CONSTRAINT `FK_BankManagers_Banks` FOREIGN KEY (`BankId`) REFERENCES `Banks` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_BankManagers_Characters` FOREIGN KEY (`CharacterId`) REFERENCES `Characters` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `BankManagers`
--

LOCK TABLES `BankManagers` WRITE;
/*!40000 ALTER TABLE `BankManagers` DISABLE KEYS */;
/*!40000 ALTER TABLE `BankManagers` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `Banks`
--

DROP TABLE IF EXISTS `Banks`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `Banks` (
  `Id` bigint NOT NULL AUTO_INCREMENT,
  `Name` varchar(255) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
  `Code` varchar(255) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
  `EconomicZoneId` bigint NOT NULL,
  `PrimaryCurrencyId` bigint NOT NULL,
  `MaximumBankAccountsPerCustomer` int NOT NULL,
  PRIMARY KEY (`Id`),
  KEY `IX_Banks_EconomicZoneId` (`EconomicZoneId`),
  KEY `IX_Banks_PrimaryCurrencyId` (`PrimaryCurrencyId`),
  CONSTRAINT `FK_Banks_Currencies` FOREIGN KEY (`PrimaryCurrencyId`) REFERENCES `Currencies` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_Banks_EconomicZones` FOREIGN KEY (`EconomicZoneId`) REFERENCES `EconomicZones` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `Banks`
--

LOCK TABLES `Banks` WRITE;
/*!40000 ALTER TABLE `Banks` DISABLE KEYS */;
/*!40000 ALTER TABLE `Banks` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `Bans`
--

DROP TABLE IF EXISTS `Bans`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `Bans` (
  `Id` bigint NOT NULL AUTO_INCREMENT,
  `IpMask` varchar(200) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
  `BannerAccountId` bigint DEFAULT NULL,
  `Reason` varchar(4000) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
  `Expiry` datetime DEFAULT NULL,
  PRIMARY KEY (`Id`),
  KEY `FK_Bans_Accounts` (`BannerAccountId`),
  CONSTRAINT `FK_Bans_Accounts` FOREIGN KEY (`BannerAccountId`) REFERENCES `Accounts` (`Id`) ON DELETE SET NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `Bans`
--

LOCK TABLES `Bans` WRITE;
/*!40000 ALTER TABLE `Bans` DISABLE KEYS */;
/*!40000 ALTER TABLE `Bans` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `BloodModels`
--

DROP TABLE IF EXISTS `BloodModels`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `BloodModels` (
  `Id` bigint NOT NULL AUTO_INCREMENT,
  `Name` varchar(200) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `BloodModels`
--

LOCK TABLES `BloodModels` WRITE;
/*!40000 ALTER TABLE `BloodModels` DISABLE KEYS */;
/*!40000 ALTER TABLE `BloodModels` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `BloodModels_Bloodtypes`
--

DROP TABLE IF EXISTS `BloodModels_Bloodtypes`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `BloodModels_Bloodtypes` (
  `BloodModelId` bigint NOT NULL,
  `BloodtypeId` bigint NOT NULL,
  PRIMARY KEY (`BloodModelId`,`BloodtypeId`),
  KEY `FK_BloodModels_Bloodtypes_Bloodtypes_idx` (`BloodtypeId`),
  CONSTRAINT `FK_BloodModels_Bloodtypes_BloodModels` FOREIGN KEY (`BloodModelId`) REFERENCES `BloodModels` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_BloodModels_Bloodtypes_Bloodtypes` FOREIGN KEY (`BloodtypeId`) REFERENCES `Bloodtypes` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `BloodModels_Bloodtypes`
--

LOCK TABLES `BloodModels_Bloodtypes` WRITE;
/*!40000 ALTER TABLE `BloodModels_Bloodtypes` DISABLE KEYS */;
/*!40000 ALTER TABLE `BloodModels_Bloodtypes` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `BloodtypeAntigens`
--

DROP TABLE IF EXISTS `BloodtypeAntigens`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `BloodtypeAntigens` (
  `Id` bigint NOT NULL AUTO_INCREMENT,
  `Name` varchar(200) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `BloodtypeAntigens`
--

LOCK TABLES `BloodtypeAntigens` WRITE;
/*!40000 ALTER TABLE `BloodtypeAntigens` DISABLE KEYS */;
/*!40000 ALTER TABLE `BloodtypeAntigens` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `Bloodtypes`
--

DROP TABLE IF EXISTS `Bloodtypes`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `Bloodtypes` (
  `Id` bigint NOT NULL AUTO_INCREMENT,
  `Name` varchar(200) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `Bloodtypes`
--

LOCK TABLES `Bloodtypes` WRITE;
/*!40000 ALTER TABLE `Bloodtypes` DISABLE KEYS */;
/*!40000 ALTER TABLE `Bloodtypes` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `Bloodtypes_BloodtypeAntigens`
--

DROP TABLE IF EXISTS `Bloodtypes_BloodtypeAntigens`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `Bloodtypes_BloodtypeAntigens` (
  `BloodtypeId` bigint NOT NULL,
  `BloodtypeAntigenId` bigint NOT NULL,
  PRIMARY KEY (`BloodtypeId`,`BloodtypeAntigenId`),
  KEY `FK_Bloodtypes_BloodtypeAntigens_BloodtypeAntigens_idx` (`BloodtypeAntigenId`),
  CONSTRAINT `FK_Bloodtypes_BloodtypeAntigens_BloodtypeAntigens` FOREIGN KEY (`BloodtypeAntigenId`) REFERENCES `BloodtypeAntigens` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_Bloodtypes_BloodtypeAntigens_Bloodtypes` FOREIGN KEY (`BloodtypeId`) REFERENCES `Bloodtypes` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `Bloodtypes_BloodtypeAntigens`
--

LOCK TABLES `Bloodtypes_BloodtypeAntigens` WRITE;
/*!40000 ALTER TABLE `Bloodtypes_BloodtypeAntigens` DISABLE KEYS */;
/*!40000 ALTER TABLE `Bloodtypes_BloodtypeAntigens` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `BoardPosts`
--

DROP TABLE IF EXISTS `BoardPosts`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `BoardPosts` (
  `Id` bigint NOT NULL AUTO_INCREMENT,
  `BoardId` bigint NOT NULL,
  `Title` varchar(200) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
  `Content` text CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
  `AuthorId` bigint DEFAULT NULL,
  `PostTime` datetime NOT NULL,
  `AuthorIsCharacter` bit(1) NOT NULL DEFAULT b'0',
  `InGameDateTime` varchar(200) CHARACTER SET utf8 COLLATE utf8_general_ci DEFAULT NULL,
  `AuthorFullDescription` varchar(8000) CHARACTER SET utf8 COLLATE utf8_general_ci DEFAULT NULL,
  `AuthorName` varchar(200) CHARACTER SET utf8 COLLATE utf8_general_ci DEFAULT NULL,
  `AuthorShortDescription` varchar(200) CHARACTER SET utf8 COLLATE utf8_general_ci DEFAULT NULL,
  PRIMARY KEY (`Id`),
  KEY `FK_BoardsPosts_Accounts_idx` (`AuthorId`),
  KEY `FK_BoardPosts_Boards_idx` (`BoardId`),
  CONSTRAINT `FK_BoardPosts_Boards` FOREIGN KEY (`BoardId`) REFERENCES `Boards` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `BoardPosts`
--

LOCK TABLES `BoardPosts` WRITE;
/*!40000 ALTER TABLE `BoardPosts` DISABLE KEYS */;
/*!40000 ALTER TABLE `BoardPosts` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `Boards`
--

DROP TABLE IF EXISTS `Boards`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `Boards` (
  `Id` bigint NOT NULL AUTO_INCREMENT,
  `Name` varchar(45) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
  `ShowOnLogin` bit(1) NOT NULL,
  `CalendarId` bigint DEFAULT NULL,
  PRIMARY KEY (`Id`),
  KEY `IX_Boards_CalendarId` (`CalendarId`),
  CONSTRAINT `FK_Boards_Calendars` FOREIGN KEY (`CalendarId`) REFERENCES `Calendars` (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `Boards`
--

LOCK TABLES `Boards` WRITE;
/*!40000 ALTER TABLE `Boards` DISABLE KEYS */;
/*!40000 ALTER TABLE `Boards` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `Bodies`
--

DROP TABLE IF EXISTS `Bodies`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `Bodies` (
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
  `ShortDescription` varchar(1000) CHARACTER SET utf8 COLLATE utf8_general_ci DEFAULT NULL,
  `FullDescription` varchar(4000) CHARACTER SET utf8 COLLATE utf8_general_ci DEFAULT NULL,
  `ShortDescriptionPatternId` bigint DEFAULT NULL,
  `FullDescriptionPatternId` bigint DEFAULT NULL,
  `Tattoos` mediumtext CHARACTER SET utf8 COLLATE utf8_general_ci,
  `HeldBreathLength` int NOT NULL,
  `EffectData` mediumtext CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
  `Scars` mediumtext CHARACTER SET utf8 COLLATE utf8_general_ci,
  `HealthStrategyId` bigint DEFAULT NULL,
  PRIMARY KEY (`Id`),
  KEY `FK_Bodies_Bloodtypes_idx` (`BloodtypeId`),
  KEY `FK_Bodies_Ethnicities_idx` (`EthnicityId`),
  KEY `FK_Bodies_EntityDescriptionPatterns_Full_idx` (`FullDescriptionPatternId`),
  KEY `FK_Bodies_Races` (`RaceId`),
  KEY `FK_Bodies_EntityDescriptionPatterns_Short_idx` (`ShortDescriptionPatternId`),
  KEY `IX_Bodies_HealthStrategyId` (`HealthStrategyId`),
  CONSTRAINT `FK_Bodies_Bloodtypes` FOREIGN KEY (`BloodtypeId`) REFERENCES `Bloodtypes` (`Id`) ON DELETE SET NULL,
  CONSTRAINT `FK_Bodies_EntityDescriptionPatterns_Full` FOREIGN KEY (`FullDescriptionPatternId`) REFERENCES `EntityDescriptionPatterns` (`Id`) ON DELETE RESTRICT,
  CONSTRAINT `FK_Bodies_EntityDescriptionPatterns_Short` FOREIGN KEY (`ShortDescriptionPatternId`) REFERENCES `EntityDescriptionPatterns` (`Id`) ON DELETE RESTRICT,
  CONSTRAINT `FK_Bodies_Ethnicities` FOREIGN KEY (`EthnicityId`) REFERENCES `Ethnicities` (`Id`) ON DELETE RESTRICT,
  CONSTRAINT `FK_Bodies_HealthStrategies_HealthStrategyId` FOREIGN KEY (`HealthStrategyId`) REFERENCES `HealthStrategies` (`Id`),
  CONSTRAINT `FK_Bodies_Races` FOREIGN KEY (`RaceId`) REFERENCES `Races` (`Id`) ON DELETE RESTRICT
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `Bodies`
--

LOCK TABLES `Bodies` WRITE;
/*!40000 ALTER TABLE `Bodies` DISABLE KEYS */;
/*!40000 ALTER TABLE `Bodies` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `Bodies_DrugDoses`
--

DROP TABLE IF EXISTS `Bodies_DrugDoses`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `Bodies_DrugDoses` (
  `BodyId` bigint NOT NULL,
  `DrugId` bigint NOT NULL,
  `Active` bit(1) NOT NULL,
  `Grams` double NOT NULL,
  `OriginalVector` int NOT NULL,
  PRIMARY KEY (`BodyId`,`DrugId`,`Active`),
  KEY `FK_Bodies_DrugDoses_Drugs_idx` (`DrugId`),
  CONSTRAINT `FK_Bodies_DrugDoses_Bodies` FOREIGN KEY (`BodyId`) REFERENCES `Bodies` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_Bodies_DrugDoses_Drugs` FOREIGN KEY (`DrugId`) REFERENCES `Drugs` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `Bodies_DrugDoses`
--

LOCK TABLES `Bodies_DrugDoses` WRITE;
/*!40000 ALTER TABLE `Bodies_DrugDoses` DISABLE KEYS */;
/*!40000 ALTER TABLE `Bodies_DrugDoses` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `Bodies_GameItems`
--

DROP TABLE IF EXISTS `Bodies_GameItems`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `Bodies_GameItems` (
  `BodyId` bigint NOT NULL,
  `GameItemId` bigint NOT NULL,
  `EquippedOrder` int NOT NULL,
  `WearProfile` bigint DEFAULT NULL,
  `Wielded` int DEFAULT NULL,
  PRIMARY KEY (`BodyId`,`GameItemId`),
  KEY `FK_Bodies_GameItems_GameItems` (`GameItemId`),
  CONSTRAINT `FK_Bodies_GameItems_Bodies` FOREIGN KEY (`BodyId`) REFERENCES `Bodies` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_Bodies_GameItems_GameItems` FOREIGN KEY (`GameItemId`) REFERENCES `GameItems` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `Bodies_GameItems`
--

LOCK TABLES `Bodies_GameItems` WRITE;
/*!40000 ALTER TABLE `Bodies_GameItems` DISABLE KEYS */;
/*!40000 ALTER TABLE `Bodies_GameItems` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `Bodies_Implants`
--

DROP TABLE IF EXISTS `Bodies_Implants`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `Bodies_Implants` (
  `BodyId` bigint NOT NULL,
  `ImplantId` bigint NOT NULL,
  PRIMARY KEY (`BodyId`,`ImplantId`),
  KEY `FK_Bodies_Implants_GameItems_idx` (`ImplantId`),
  CONSTRAINT `FK_Bodies_Implants_Bodies` FOREIGN KEY (`BodyId`) REFERENCES `Bodies` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_Bodies_Implants_GameItems` FOREIGN KEY (`ImplantId`) REFERENCES `GameItems` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `Bodies_Implants`
--

LOCK TABLES `Bodies_Implants` WRITE;
/*!40000 ALTER TABLE `Bodies_Implants` DISABLE KEYS */;
/*!40000 ALTER TABLE `Bodies_Implants` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `Bodies_Prosthetics`
--

DROP TABLE IF EXISTS `Bodies_Prosthetics`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `Bodies_Prosthetics` (
  `BodyId` bigint NOT NULL,
  `ProstheticId` bigint NOT NULL,
  PRIMARY KEY (`BodyId`,`ProstheticId`),
  KEY `FK_Bodies_Prosthetics_GameItems_idx` (`ProstheticId`),
  CONSTRAINT `FK_Bodies_Prosthetics_Bodies` FOREIGN KEY (`BodyId`) REFERENCES `Bodies` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_Bodies_Prosthetics_GameItems` FOREIGN KEY (`ProstheticId`) REFERENCES `GameItems` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `Bodies_Prosthetics`
--

LOCK TABLES `Bodies_Prosthetics` WRITE;
/*!40000 ALTER TABLE `Bodies_Prosthetics` DISABLE KEYS */;
/*!40000 ALTER TABLE `Bodies_Prosthetics` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `Bodies_SeveredParts`
--

DROP TABLE IF EXISTS `Bodies_SeveredParts`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `Bodies_SeveredParts` (
  `BodiesId` bigint NOT NULL,
  `BodypartProtoId` bigint NOT NULL,
  PRIMARY KEY (`BodiesId`,`BodypartProtoId`),
  KEY `FK_Bodies_SeveredParts_BodypartProtos_idx` (`BodypartProtoId`),
  CONSTRAINT `FK_Bodies_SeveredParts_Bodies` FOREIGN KEY (`BodiesId`) REFERENCES `Bodies` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_Bodies_SeveredParts_BodypartProtos` FOREIGN KEY (`BodypartProtoId`) REFERENCES `BodypartProto` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `Bodies_SeveredParts`
--

LOCK TABLES `Bodies_SeveredParts` WRITE;
/*!40000 ALTER TABLE `Bodies_SeveredParts` DISABLE KEYS */;
/*!40000 ALTER TABLE `Bodies_SeveredParts` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `BodypartGroupDescribers`
--

DROP TABLE IF EXISTS `BodypartGroupDescribers`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `BodypartGroupDescribers` (
  `Id` bigint NOT NULL AUTO_INCREMENT,
  `DescribedAs` varchar(4000) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
  `Comment` varchar(4000) CHARACTER SET utf8 COLLATE utf8_general_ci DEFAULT NULL,
  `Type` varchar(50) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
  `BodyProtoId` bigint DEFAULT NULL,
  PRIMARY KEY (`Id`),
  KEY `IX_BodypartGroupDescribers_BodyProtoId` (`BodyProtoId`),
  CONSTRAINT `FK_BodypartGroupDescribers_BodyProtos_BodyProtoId` FOREIGN KEY (`BodyProtoId`) REFERENCES `BodyProtos` (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `BodypartGroupDescribers`
--

LOCK TABLES `BodypartGroupDescribers` WRITE;
/*!40000 ALTER TABLE `BodypartGroupDescribers` DISABLE KEYS */;
/*!40000 ALTER TABLE `BodypartGroupDescribers` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `BodypartGroupDescribers_BodypartProtos`
--

DROP TABLE IF EXISTS `BodypartGroupDescribers_BodypartProtos`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `BodypartGroupDescribers_BodypartProtos` (
  `BodypartGroupDescriberId` bigint NOT NULL,
  `BodypartProtoId` bigint NOT NULL,
  `Mandatory` bit(1) NOT NULL DEFAULT b'0',
  PRIMARY KEY (`BodypartGroupDescriberId`,`BodypartProtoId`),
  KEY `FK_BGD_BodypartProtos_BodypartProto` (`BodypartProtoId`),
  CONSTRAINT `FK_BGD_BodypartProtos_BodypartGroupDescribers` FOREIGN KEY (`BodypartGroupDescriberId`) REFERENCES `BodypartGroupDescribers` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_BGD_BodypartProtos_BodypartProto` FOREIGN KEY (`BodypartProtoId`) REFERENCES `BodypartProto` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `BodypartGroupDescribers_BodypartProtos`
--

LOCK TABLES `BodypartGroupDescribers_BodypartProtos` WRITE;
/*!40000 ALTER TABLE `BodypartGroupDescribers_BodypartProtos` DISABLE KEYS */;
/*!40000 ALTER TABLE `BodypartGroupDescribers_BodypartProtos` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `BodypartGroupDescribers_BodyProtos`
--

DROP TABLE IF EXISTS `BodypartGroupDescribers_BodyProtos`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `BodypartGroupDescribers_BodyProtos` (
  `BodypartGroupDescriberId` bigint NOT NULL,
  `BodyProtoId` bigint NOT NULL,
  PRIMARY KEY (`BodypartGroupDescriberId`,`BodyProtoId`),
  KEY `FK_BGD_BodyProtos_BodyProtos` (`BodyProtoId`),
  CONSTRAINT `FK_BGD_BodyProtos_BodypartGroupDescribers` FOREIGN KEY (`BodypartGroupDescriberId`) REFERENCES `BodypartGroupDescribers` (`Id`) ON DELETE RESTRICT,
  CONSTRAINT `FK_BGD_BodyProtos_BodyProtos` FOREIGN KEY (`BodyProtoId`) REFERENCES `BodyProtos` (`Id`) ON DELETE RESTRICT
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `BodypartGroupDescribers_BodyProtos`
--

LOCK TABLES `BodypartGroupDescribers_BodyProtos` WRITE;
/*!40000 ALTER TABLE `BodypartGroupDescribers_BodyProtos` DISABLE KEYS */;
/*!40000 ALTER TABLE `BodypartGroupDescribers_BodyProtos` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `BodypartGroupDescribers_ShapeCount`
--

DROP TABLE IF EXISTS `BodypartGroupDescribers_ShapeCount`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `BodypartGroupDescribers_ShapeCount` (
  `BodypartGroupDescriptionRuleId` bigint NOT NULL,
  `TargetId` bigint NOT NULL,
  `MinCount` int NOT NULL,
  `MaxCount` int NOT NULL,
  PRIMARY KEY (`BodypartGroupDescriptionRuleId`,`TargetId`),
  KEY `FK_BGD_ShapeCount_BodypartShape` (`TargetId`),
  CONSTRAINT `FK_BGD_ShapeCount_BodypartGroupDescribers` FOREIGN KEY (`BodypartGroupDescriptionRuleId`) REFERENCES `BodypartGroupDescribers` (`Id`) ON DELETE RESTRICT,
  CONSTRAINT `FK_BGD_ShapeCount_BodypartShape` FOREIGN KEY (`TargetId`) REFERENCES `BodypartShape` (`Id`) ON DELETE RESTRICT
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `BodypartGroupDescribers_ShapeCount`
--

LOCK TABLES `BodypartGroupDescribers_ShapeCount` WRITE;
/*!40000 ALTER TABLE `BodypartGroupDescribers_ShapeCount` DISABLE KEYS */;
/*!40000 ALTER TABLE `BodypartGroupDescribers_ShapeCount` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `BodypartInternalInfos`
--

DROP TABLE IF EXISTS `BodypartInternalInfos`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `BodypartInternalInfos` (
  `BodypartProtoId` bigint NOT NULL,
  `InternalPartId` bigint NOT NULL,
  `IsPrimaryOrganLocation` bit(1) NOT NULL DEFAULT b'0',
  `HitChance` double NOT NULL DEFAULT '5',
  `ProximityGroup` varchar(45) CHARACTER SET utf8 COLLATE utf8_general_ci DEFAULT NULL,
  PRIMARY KEY (`BodypartProtoId`,`InternalPartId`),
  KEY `FK_BodypartInternalInfos_BodypartProtos_Internal_idx` (`InternalPartId`),
  KEY `FK_BodypartInternalInfos_BodypartProtos_idx` (`BodypartProtoId`),
  CONSTRAINT `FK_BodypartInternalInfos_BodypartProtos` FOREIGN KEY (`BodypartProtoId`) REFERENCES `BodypartProto` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_BodypartInternalInfos_BodypartProtos_Internal` FOREIGN KEY (`InternalPartId`) REFERENCES `BodypartProto` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `BodypartInternalInfos`
--

LOCK TABLES `BodypartInternalInfos` WRITE;
/*!40000 ALTER TABLE `BodypartInternalInfos` DISABLE KEYS */;
/*!40000 ALTER TABLE `BodypartInternalInfos` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `BodypartProto`
--

DROP TABLE IF EXISTS `BodypartProto`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `BodypartProto` (
  `Id` bigint NOT NULL AUTO_INCREMENT,
  `BodypartType` int NOT NULL,
  `BodyId` bigint NOT NULL,
  `Name` varchar(4000) CHARACTER SET utf8 COLLATE utf8_general_ci DEFAULT NULL,
  `Description` varchar(4000) CHARACTER SET utf8 COLLATE utf8_general_ci DEFAULT NULL,
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
  CONSTRAINT `FK_BodypartProto_ArmourTypes` FOREIGN KEY (`ArmourTypeId`) REFERENCES `ArmourTypes` (`Id`) ON DELETE SET NULL,
  CONSTRAINT `FK_BodypartProto_BodypartProto` FOREIGN KEY (`CountAsId`) REFERENCES `BodypartProto` (`Id`) ON DELETE SET NULL,
  CONSTRAINT `FK_BodypartProto_BodypartShape` FOREIGN KEY (`BodypartShapeId`) REFERENCES `BodypartShape` (`Id`) ON DELETE RESTRICT,
  CONSTRAINT `FK_BodypartProto_BodyPrototype` FOREIGN KEY (`BodyId`) REFERENCES `BodyProtos` (`Id`) ON DELETE RESTRICT,
  CONSTRAINT `FK_BodypartProto_Materials` FOREIGN KEY (`DefaultMaterialId`) REFERENCES `Materials` (`Id`) ON DELETE RESTRICT
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `BodypartProto`
--

LOCK TABLES `BodypartProto` WRITE;
/*!40000 ALTER TABLE `BodypartProto` DISABLE KEYS */;
/*!40000 ALTER TABLE `BodypartProto` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `BodypartProto_AlignmentHits`
--

DROP TABLE IF EXISTS `BodypartProto_AlignmentHits`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `BodypartProto_AlignmentHits` (
  `Id` bigint NOT NULL AUTO_INCREMENT,
  `BodypartProtoId` bigint NOT NULL,
  `Alignment` int NOT NULL,
  `HitChance` int DEFAULT NULL,
  PRIMARY KEY (`Id`),
  KEY `FK_BodypartProto_AlignmentHits_BodypartProto` (`BodypartProtoId`),
  CONSTRAINT `FK_BodypartProto_AlignmentHits_BodypartProto` FOREIGN KEY (`BodypartProtoId`) REFERENCES `BodypartProto` (`Id`) ON DELETE RESTRICT
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `BodypartProto_AlignmentHits`
--

LOCK TABLES `BodypartProto_AlignmentHits` WRITE;
/*!40000 ALTER TABLE `BodypartProto_AlignmentHits` DISABLE KEYS */;
/*!40000 ALTER TABLE `BodypartProto_AlignmentHits` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `BodypartProto_BodypartProto_Upstream`
--

DROP TABLE IF EXISTS `BodypartProto_BodypartProto_Upstream`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `BodypartProto_BodypartProto_Upstream` (
  `Child` bigint NOT NULL,
  `Parent` bigint NOT NULL,
  PRIMARY KEY (`Child`,`Parent`),
  KEY `FKParent` (`Parent`),
  CONSTRAINT `FKChild` FOREIGN KEY (`Child`) REFERENCES `BodypartProto` (`Id`) ON DELETE RESTRICT,
  CONSTRAINT `FKParent` FOREIGN KEY (`Parent`) REFERENCES `BodypartProto` (`Id`) ON DELETE RESTRICT
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `BodypartProto_BodypartProto_Upstream`
--

LOCK TABLES `BodypartProto_BodypartProto_Upstream` WRITE;
/*!40000 ALTER TABLE `BodypartProto_BodypartProto_Upstream` DISABLE KEYS */;
/*!40000 ALTER TABLE `BodypartProto_BodypartProto_Upstream` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `BodypartProto_OrientationHits`
--

DROP TABLE IF EXISTS `BodypartProto_OrientationHits`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `BodypartProto_OrientationHits` (
  `Id` bigint NOT NULL AUTO_INCREMENT,
  `BodypartProtoId` bigint NOT NULL,
  `Orientation` int NOT NULL,
  `HitChance` int DEFAULT NULL,
  PRIMARY KEY (`Id`),
  KEY `FK_BodypartProto_OrientationHits_BodypartProto` (`BodypartProtoId`),
  CONSTRAINT `FK_BodypartProto_OrientationHits_BodypartProto` FOREIGN KEY (`BodypartProtoId`) REFERENCES `BodypartProto` (`Id`) ON DELETE RESTRICT
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `BodypartProto_OrientationHits`
--

LOCK TABLES `BodypartProto_OrientationHits` WRITE;
/*!40000 ALTER TABLE `BodypartProto_OrientationHits` DISABLE KEYS */;
/*!40000 ALTER TABLE `BodypartProto_OrientationHits` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `BodypartShape`
--

DROP TABLE IF EXISTS `BodypartShape`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `BodypartShape` (
  `Id` bigint NOT NULL AUTO_INCREMENT,
  `Name` varchar(4000) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `BodypartShape`
--

LOCK TABLES `BodypartShape` WRITE;
/*!40000 ALTER TABLE `BodypartShape` DISABLE KEYS */;
/*!40000 ALTER TABLE `BodypartShape` ENABLE KEYS */;
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
-- Table structure for table `BodyProtos`
--

DROP TABLE IF EXISTS `BodyProtos`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `BodyProtos` (
  `Id` bigint NOT NULL AUTO_INCREMENT,
  `Name` varchar(100) CHARACTER SET utf8 COLLATE utf8_general_ci DEFAULT NULL,
  `CountsAsId` bigint DEFAULT NULL,
  `WearSizeParameterId` bigint NOT NULL,
  `WielderDescriptionPlural` varchar(100) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL DEFAULT 'hands',
  `WielderDescriptionSingle` varchar(100) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL DEFAULT 'hand',
  `ConsiderString` varchar(4000) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
  `StaminaRecoveryProgId` bigint DEFAULT NULL,
  `MinimumLegsToStand` int NOT NULL DEFAULT '2',
  `MinimumWingsToFly` int NOT NULL DEFAULT '2',
  `LegDescriptionSingular` varchar(100) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL DEFAULT 'leg',
  `LegDescriptionPlural` varchar(100) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL DEFAULT 'legs',
  `DefaultSmashingBodypartId` bigint DEFAULT NULL,
  `NameForTracking` varchar(100) CHARACTER SET utf8 COLLATE utf8_general_ci DEFAULT NULL,
  PRIMARY KEY (`Id`),
  KEY `FK_BodyPrototype_BodyPrototype_idx` (`CountsAsId`),
  KEY `FK_BodyPrototype_Bodyparts_idx` (`DefaultSmashingBodypartId`),
  KEY `FK_BodyPrototype_WearableSizeParameterRule` (`WearSizeParameterId`),
  CONSTRAINT `FK_BodyPrototype_Bodyparts` FOREIGN KEY (`DefaultSmashingBodypartId`) REFERENCES `BodypartProto` (`Id`) ON DELETE SET NULL,
  CONSTRAINT `FK_BodyPrototype_BodyPrototype` FOREIGN KEY (`CountsAsId`) REFERENCES `BodyProtos` (`Id`) ON DELETE SET NULL,
  CONSTRAINT `FK_BodyPrototype_WearableSizeParameterRule` FOREIGN KEY (`WearSizeParameterId`) REFERENCES `WearableSizeParameterRule` (`Id`) ON DELETE RESTRICT
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `BodyProtos`
--

LOCK TABLES `BodyProtos` WRITE;
/*!40000 ALTER TABLE `BodyProtos` DISABLE KEYS */;
/*!40000 ALTER TABLE `BodyProtos` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `BodyProtos_AdditionalBodyparts`
--

DROP TABLE IF EXISTS `BodyProtos_AdditionalBodyparts`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `BodyProtos_AdditionalBodyparts` (
  `BodyProtoId` bigint NOT NULL,
  `BodypartId` bigint NOT NULL,
  `Usage` varchar(50) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
  PRIMARY KEY (`BodyProtoId`,`BodypartId`,`Usage`),
  KEY `FK_BodyProtos_AdditionalBodyparts_BodypartProto_idx` (`BodypartId`),
  KEY `FK_BodyProtos_AdditionalBodyparts_BodyProtos_idx` (`BodyProtoId`),
  CONSTRAINT `FK_BodyProtos_AdditionalBodyparts_BodypartProto` FOREIGN KEY (`BodypartId`) REFERENCES `BodypartProto` (`Id`) ON DELETE RESTRICT,
  CONSTRAINT `FK_BodyProtos_AdditionalBodyparts_BodyProtos` FOREIGN KEY (`BodyProtoId`) REFERENCES `BodyProtos` (`Id`) ON DELETE RESTRICT
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `BodyProtos_AdditionalBodyparts`
--

LOCK TABLES `BodyProtos_AdditionalBodyparts` WRITE;
/*!40000 ALTER TABLE `BodyProtos_AdditionalBodyparts` DISABLE KEYS */;
/*!40000 ALTER TABLE `BodyProtos_AdditionalBodyparts` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `BodyProtosPositions`
--

DROP TABLE IF EXISTS `BodyProtosPositions`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `BodyProtosPositions` (
  `BodyProtoId` bigint NOT NULL,
  `Position` int NOT NULL,
  PRIMARY KEY (`BodyProtoId`,`Position`),
  KEY `FK_BodyProtosPositions_BodyProtos_idx` (`BodyProtoId`),
  CONSTRAINT `FK_BodyProtosPositions_BodyProtos` FOREIGN KEY (`BodyProtoId`) REFERENCES `BodyProtos` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `BodyProtosPositions`
--

LOCK TABLES `BodyProtosPositions` WRITE;
/*!40000 ALTER TABLE `BodyProtosPositions` DISABLE KEYS */;
/*!40000 ALTER TABLE `BodyProtosPositions` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `BoneOrganCoverages`
--

DROP TABLE IF EXISTS `BoneOrganCoverages`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `BoneOrganCoverages` (
  `BoneId` bigint NOT NULL,
  `OrganId` bigint NOT NULL,
  `CoverageChance` double NOT NULL,
  PRIMARY KEY (`BoneId`,`OrganId`),
  KEY `FK_BoneOrganCoverages_BodypartProto_Organ_idx` (`OrganId`),
  CONSTRAINT `FK_BoneOrganCoverages_BodypartProto_Bone` FOREIGN KEY (`BoneId`) REFERENCES `BodypartProto` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_BoneOrganCoverages_BodypartProto_Organ` FOREIGN KEY (`OrganId`) REFERENCES `BodypartProto` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `BoneOrganCoverages`
--

LOCK TABLES `BoneOrganCoverages` WRITE;
/*!40000 ALTER TABLE `BoneOrganCoverages` DISABLE KEYS */;
/*!40000 ALTER TABLE `BoneOrganCoverages` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `ButcheryProductItems`
--

DROP TABLE IF EXISTS `ButcheryProductItems`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `ButcheryProductItems` (
  `Id` bigint NOT NULL AUTO_INCREMENT,
  `ButcheryProductId` bigint NOT NULL,
  `NormalProtoId` bigint NOT NULL,
  `DamagedProtoId` bigint DEFAULT NULL,
  `NormalQuantity` int NOT NULL,
  `DamagedQuantity` int NOT NULL,
  `ButcheryProductItemscol` varchar(45) CHARACTER SET utf8 COLLATE utf8_general_ci DEFAULT NULL,
  `DamageThreshold` double NOT NULL DEFAULT '10',
  PRIMARY KEY (`Id`),
  KEY `FK_ButcheryProductItems_ButcheryProducts_idx` (`ButcheryProductId`),
  CONSTRAINT `FK_ButcheryProductItems_ButcheryProducts` FOREIGN KEY (`ButcheryProductId`) REFERENCES `ButcheryProducts` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `ButcheryProductItems`
--

LOCK TABLES `ButcheryProductItems` WRITE;
/*!40000 ALTER TABLE `ButcheryProductItems` DISABLE KEYS */;
/*!40000 ALTER TABLE `ButcheryProductItems` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `ButcheryProducts`
--

DROP TABLE IF EXISTS `ButcheryProducts`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `ButcheryProducts` (
  `Id` bigint NOT NULL AUTO_INCREMENT,
  `Name` varchar(100) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
  `TargetBodyId` bigint NOT NULL,
  `IsPelt` bit(1) NOT NULL,
  `Subcategory` varchar(100) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
  `CanProduceProgId` bigint DEFAULT NULL,
  PRIMARY KEY (`Id`),
  KEY `FK_ButcheryProducts_FutureProgs_idx` (`CanProduceProgId`),
  KEY `FK_ButcheryProducts_BodyProtos_idx` (`TargetBodyId`),
  CONSTRAINT `FK_ButcheryProducts_BodyProtos` FOREIGN KEY (`TargetBodyId`) REFERENCES `BodyProtos` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_ButcheryProducts_FutureProgs` FOREIGN KEY (`CanProduceProgId`) REFERENCES `FutureProgs` (`Id`) ON DELETE SET NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `ButcheryProducts`
--

LOCK TABLES `ButcheryProducts` WRITE;
/*!40000 ALTER TABLE `ButcheryProducts` DISABLE KEYS */;
/*!40000 ALTER TABLE `ButcheryProducts` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `ButcheryProducts_BodypartProtos`
--

DROP TABLE IF EXISTS `ButcheryProducts_BodypartProtos`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `ButcheryProducts_BodypartProtos` (
  `ButcheryProductId` bigint NOT NULL,
  `BodypartProtoId` bigint NOT NULL,
  PRIMARY KEY (`ButcheryProductId`,`BodypartProtoId`),
  KEY `FK_ButcheryProducts_BodypartProtos_BodypartProtos_idx` (`BodypartProtoId`),
  CONSTRAINT `FK_ButcheryProducts_BodypartProtos_BodypartProtos` FOREIGN KEY (`BodypartProtoId`) REFERENCES `BodypartProto` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_ButcheryProducts_BodypartProtos_ButcheryProducts` FOREIGN KEY (`ButcheryProductId`) REFERENCES `ButcheryProducts` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `ButcheryProducts_BodypartProtos`
--

LOCK TABLES `ButcheryProducts_BodypartProtos` WRITE;
/*!40000 ALTER TABLE `ButcheryProducts_BodypartProtos` DISABLE KEYS */;
/*!40000 ALTER TABLE `ButcheryProducts_BodypartProtos` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `Calendars`
--

DROP TABLE IF EXISTS `Calendars`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `Calendars` (
  `Id` bigint NOT NULL AUTO_INCREMENT,
  `Definition` text CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
  `Date` varchar(50) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
  `FeedClockId` bigint NOT NULL,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `Calendars`
--

LOCK TABLES `Calendars` WRITE;
/*!40000 ALTER TABLE `Calendars` DISABLE KEYS */;
/*!40000 ALTER TABLE `Calendars` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `Celestials`
--

DROP TABLE IF EXISTS `Celestials`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `Celestials` (
  `Id` bigint NOT NULL AUTO_INCREMENT,
  `Definition` text CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
  `Minutes` int NOT NULL,
  `FeedClockId` bigint NOT NULL,
  `CelestialYear` int NOT NULL,
  `LastYearBump` int NOT NULL,
  `CelestialType` varchar(30) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL DEFAULT 'OldSun',
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `Celestials`
--

LOCK TABLES `Celestials` WRITE;
/*!40000 ALTER TABLE `Celestials` DISABLE KEYS */;
/*!40000 ALTER TABLE `Celestials` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `CellOverlayPackages`
--

DROP TABLE IF EXISTS `CellOverlayPackages`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `CellOverlayPackages` (
  `Id` bigint NOT NULL,
  `RevisionNumber` int NOT NULL,
  `Name` varchar(4000) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
  `EditableItemId` bigint NOT NULL,
  PRIMARY KEY (`Id`,`RevisionNumber`),
  KEY `FK_CellOverlayPackages_EditableItems` (`EditableItemId`),
  CONSTRAINT `FK_CellOverlayPackages_EditableItems` FOREIGN KEY (`EditableItemId`) REFERENCES `EditableItems` (`Id`) ON DELETE RESTRICT
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `CellOverlayPackages`
--

LOCK TABLES `CellOverlayPackages` WRITE;
/*!40000 ALTER TABLE `CellOverlayPackages` DISABLE KEYS */;
/*!40000 ALTER TABLE `CellOverlayPackages` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `CellOverlays`
--

DROP TABLE IF EXISTS `CellOverlays`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `CellOverlays` (
  `Id` bigint NOT NULL AUTO_INCREMENT,
  `Name` varchar(4000) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
  `CellName` varchar(4000) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
  `CellDescription` varchar(4000) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
  `CellOverlayPackageId` bigint NOT NULL,
  `CellId` bigint NOT NULL,
  `CellOverlayPackageRevisionNumber` int NOT NULL,
  `TerrainId` bigint NOT NULL,
  `HearingProfileId` bigint DEFAULT NULL,
  `OutdoorsType` int NOT NULL,
  `AmbientLightFactor` double NOT NULL DEFAULT '1',
  `AddedLight` double NOT NULL,
  `AtmosphereId` bigint DEFAULT NULL,
  `AtmosphereType` varchar(45) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL DEFAULT 'gas',
  `SafeQuit` bit(1) NOT NULL,
  PRIMARY KEY (`Id`),
  KEY `FK_CellOverlays_Cells` (`CellId`),
  KEY `FK_CellOverlays_HearingProfiles` (`HearingProfileId`),
  KEY `FK_CellOverlays_Terrains` (`TerrainId`),
  KEY `FK_CellOverlays_CellOverlayPackages` (`CellOverlayPackageId`,`CellOverlayPackageRevisionNumber`),
  CONSTRAINT `FK_CellOverlays_CellOverlayPackages` FOREIGN KEY (`CellOverlayPackageId`, `CellOverlayPackageRevisionNumber`) REFERENCES `CellOverlayPackages` (`Id`, `RevisionNumber`) ON DELETE CASCADE,
  CONSTRAINT `FK_CellOverlays_Cells` FOREIGN KEY (`CellId`) REFERENCES `Cells` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_CellOverlays_HearingProfiles` FOREIGN KEY (`HearingProfileId`) REFERENCES `HearingProfiles` (`Id`) ON DELETE SET NULL,
  CONSTRAINT `FK_CellOverlays_Terrains` FOREIGN KEY (`TerrainId`) REFERENCES `Terrains` (`Id`) ON DELETE RESTRICT
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `CellOverlays`
--

LOCK TABLES `CellOverlays` WRITE;
/*!40000 ALTER TABLE `CellOverlays` DISABLE KEYS */;
/*!40000 ALTER TABLE `CellOverlays` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `CellOverlays_Exits`
--

DROP TABLE IF EXISTS `CellOverlays_Exits`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `CellOverlays_Exits` (
  `CellOverlayId` bigint NOT NULL,
  `ExitId` bigint NOT NULL,
  PRIMARY KEY (`CellOverlayId`,`ExitId`),
  KEY `FK_CellOverlays_Exits_Exits` (`ExitId`),
  CONSTRAINT `FK_CellOverlays_Exits_CellOverlays` FOREIGN KEY (`CellOverlayId`) REFERENCES `CellOverlays` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_CellOverlays_Exits_Exits` FOREIGN KEY (`ExitId`) REFERENCES `Exits` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `CellOverlays_Exits`
--

LOCK TABLES `CellOverlays_Exits` WRITE;
/*!40000 ALTER TABLE `CellOverlays_Exits` DISABLE KEYS */;
/*!40000 ALTER TABLE `CellOverlays_Exits` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `Cells`
--

DROP TABLE IF EXISTS `Cells`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `Cells` (
  `Id` bigint NOT NULL AUTO_INCREMENT,
  `RoomId` bigint NOT NULL,
  `CurrentOverlayId` bigint DEFAULT NULL,
  `ForagableProfileId` bigint DEFAULT NULL,
  `Temporary` bit(1) NOT NULL DEFAULT b'0',
  `EffectData` mediumtext CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
  PRIMARY KEY (`Id`),
  KEY `FK_Cells_CellOverlays` (`CurrentOverlayId`),
  KEY `FK_Cells_Rooms` (`RoomId`),
  CONSTRAINT `FK_Cells_CellOverlays` FOREIGN KEY (`CurrentOverlayId`) REFERENCES `CellOverlays` (`Id`) ON DELETE SET NULL,
  CONSTRAINT `FK_Cells_Rooms` FOREIGN KEY (`RoomId`) REFERENCES `Rooms` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `Cells`
--

LOCK TABLES `Cells` WRITE;
/*!40000 ALTER TABLE `Cells` DISABLE KEYS */;
/*!40000 ALTER TABLE `Cells` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `Cells_ForagableYields`
--

DROP TABLE IF EXISTS `Cells_ForagableYields`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `Cells_ForagableYields` (
  `CellId` bigint NOT NULL,
  `ForagableType` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL,
  `Yield` double NOT NULL,
  PRIMARY KEY (`CellId`,`ForagableType`),
  CONSTRAINT `FK_Cells_ForagableYields_Cells` FOREIGN KEY (`CellId`) REFERENCES `Cells` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `Cells_ForagableYields`
--

LOCK TABLES `Cells_ForagableYields` WRITE;
/*!40000 ALTER TABLE `Cells_ForagableYields` DISABLE KEYS */;
/*!40000 ALTER TABLE `Cells_ForagableYields` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `Cells_GameItems`
--

DROP TABLE IF EXISTS `Cells_GameItems`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `Cells_GameItems` (
  `CellId` bigint NOT NULL,
  `GameItemId` bigint NOT NULL,
  PRIMARY KEY (`CellId`,`GameItemId`),
  KEY `FK_Cells_GameItems_GameItems` (`GameItemId`),
  CONSTRAINT `FK_Cells_GameItems_Cells` FOREIGN KEY (`CellId`) REFERENCES `Cells` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_Cells_GameItems_GameItems` FOREIGN KEY (`GameItemId`) REFERENCES `GameItems` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `Cells_GameItems`
--

LOCK TABLES `Cells_GameItems` WRITE;
/*!40000 ALTER TABLE `Cells_GameItems` DISABLE KEYS */;
/*!40000 ALTER TABLE `Cells_GameItems` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `Cells_MagicResources`
--

DROP TABLE IF EXISTS `Cells_MagicResources`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `Cells_MagicResources` (
  `CellId` bigint NOT NULL,
  `MagicResourceId` bigint NOT NULL,
  `Amount` double NOT NULL,
  PRIMARY KEY (`CellId`,`MagicResourceId`),
  KEY `FK_Cells_MagicResources_MagicResources_idx` (`MagicResourceId`),
  CONSTRAINT `FK_Cells_MagicResources_Cells` FOREIGN KEY (`CellId`) REFERENCES `Cells` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_Cells_MagicResources_MagicResources` FOREIGN KEY (`MagicResourceId`) REFERENCES `MagicResources` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `Cells_MagicResources`
--

LOCK TABLES `Cells_MagicResources` WRITE;
/*!40000 ALTER TABLE `Cells_MagicResources` DISABLE KEYS */;
/*!40000 ALTER TABLE `Cells_MagicResources` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `Cells_RangedCovers`
--

DROP TABLE IF EXISTS `Cells_RangedCovers`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `Cells_RangedCovers` (
  `CellId` bigint NOT NULL,
  `RangedCoverId` bigint NOT NULL,
  PRIMARY KEY (`CellId`,`RangedCoverId`),
  KEY `FK_Cells_RangedCovers_RangedCovers_idx` (`RangedCoverId`),
  CONSTRAINT `FK_Cells_RangedCovers_Cells` FOREIGN KEY (`CellId`) REFERENCES `Cells` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_Cells_RangedCovers_RangedCovers` FOREIGN KEY (`RangedCoverId`) REFERENCES `RangedCovers` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `Cells_RangedCovers`
--

LOCK TABLES `Cells_RangedCovers` WRITE;
/*!40000 ALTER TABLE `Cells_RangedCovers` DISABLE KEYS */;
/*!40000 ALTER TABLE `Cells_RangedCovers` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `Cells_Tags`
--

DROP TABLE IF EXISTS `Cells_Tags`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `Cells_Tags` (
  `CellId` bigint NOT NULL,
  `TagId` bigint NOT NULL,
  PRIMARY KEY (`CellId`,`TagId`),
  KEY `FK_Cells_Tags_Tags_idx` (`TagId`),
  CONSTRAINT `FK_Cells_Tags_Cells` FOREIGN KEY (`CellId`) REFERENCES `Cells` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_Cells_Tags_Tags` FOREIGN KEY (`TagId`) REFERENCES `Tags` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `Cells_Tags`
--

LOCK TABLES `Cells_Tags` WRITE;
/*!40000 ALTER TABLE `Cells_Tags` DISABLE KEYS */;
/*!40000 ALTER TABLE `Cells_Tags` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `ChannelCommandWords`
--

DROP TABLE IF EXISTS `ChannelCommandWords`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `ChannelCommandWords` (
  `Word` varchar(50) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
  `ChannelId` bigint NOT NULL,
  PRIMARY KEY (`Word`),
  KEY `FK_ChannelCommandWords_Channels` (`ChannelId`),
  CONSTRAINT `FK_ChannelCommandWords_Channels` FOREIGN KEY (`ChannelId`) REFERENCES `Channels` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `ChannelCommandWords`
--

LOCK TABLES `ChannelCommandWords` WRITE;
/*!40000 ALTER TABLE `ChannelCommandWords` DISABLE KEYS */;
/*!40000 ALTER TABLE `ChannelCommandWords` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `ChannelIgnorers`
--

DROP TABLE IF EXISTS `ChannelIgnorers`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `ChannelIgnorers` (
  `ChannelId` bigint NOT NULL,
  `AccountId` bigint NOT NULL,
  PRIMARY KEY (`ChannelId`,`AccountId`),
  KEY `FK_ChannelIgnorers_Accounts` (`AccountId`),
  CONSTRAINT `FK_ChannelIgnorers_Accounts` FOREIGN KEY (`AccountId`) REFERENCES `Accounts` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_ChannelIgnorers_Channels` FOREIGN KEY (`ChannelId`) REFERENCES `Channels` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `ChannelIgnorers`
--

LOCK TABLES `ChannelIgnorers` WRITE;
/*!40000 ALTER TABLE `ChannelIgnorers` DISABLE KEYS */;
/*!40000 ALTER TABLE `ChannelIgnorers` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `Channels`
--

DROP TABLE IF EXISTS `Channels`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `Channels` (
  `Id` bigint NOT NULL AUTO_INCREMENT,
  `ChannelName` varchar(4000) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
  `ChannelListenerProgId` bigint NOT NULL,
  `ChannelSpeakerProgId` bigint NOT NULL,
  `AnnounceChannelJoiners` bit(1) NOT NULL,
  `ChannelColour` char(10) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
  `Mode` int NOT NULL,
  `AnnounceMissedListeners` bit(1) NOT NULL,
  `AddToPlayerCommandTree` bit(1) NOT NULL DEFAULT b'0',
  `AddToGuideCommandTree` bit(1) NOT NULL DEFAULT b'0',
  `DiscordChannelId` bigint unsigned DEFAULT NULL,
  PRIMARY KEY (`Id`),
  KEY `FK_Channels_FutureProgs_Listener` (`ChannelListenerProgId`),
  KEY `FK_Channels_FutureProgs_Speaker` (`ChannelSpeakerProgId`),
  CONSTRAINT `FK_Channels_FutureProgs_Listener` FOREIGN KEY (`ChannelListenerProgId`) REFERENCES `FutureProgs` (`Id`) ON DELETE RESTRICT,
  CONSTRAINT `FK_Channels_FutureProgs_Speaker` FOREIGN KEY (`ChannelSpeakerProgId`) REFERENCES `FutureProgs` (`Id`) ON DELETE RESTRICT
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `Channels`
--

LOCK TABLES `Channels` WRITE;
/*!40000 ALTER TABLE `Channels` DISABLE KEYS */;
/*!40000 ALTER TABLE `Channels` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `CharacterCombatSettings`
--

DROP TABLE IF EXISTS `CharacterCombatSettings`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `CharacterCombatSettings` (
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
  PRIMARY KEY (`Id`),
  KEY `FK_CharacterCombatSettings_FutureProgs_idx` (`AvailabilityProgId`),
  KEY `FK_CharacterCombatSettings_Characters_idx` (`CharacterOwnerId`),
  CONSTRAINT `FK_CharacterCombatSettings_Characters` FOREIGN KEY (`CharacterOwnerId`) REFERENCES `Characters` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_CharacterCombatSettings_FutureProgs` FOREIGN KEY (`AvailabilityProgId`) REFERENCES `FutureProgs` (`Id`) ON DELETE SET NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `CharacterCombatSettings`
--

LOCK TABLES `CharacterCombatSettings` WRITE;
/*!40000 ALTER TABLE `CharacterCombatSettings` DISABLE KEYS */;
/*!40000 ALTER TABLE `CharacterCombatSettings` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `CharacterIntroTemplates`
--

DROP TABLE IF EXISTS `CharacterIntroTemplates`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `CharacterIntroTemplates` (
  `Id` bigint NOT NULL AUTO_INCREMENT,
  `Name` varchar(200) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
  `ResolutionPriority` int NOT NULL DEFAULT '1',
  `AppliesToCharacterProgId` bigint NOT NULL,
  `Definition` text CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
  `Order` int NOT NULL,
  PRIMARY KEY (`Id`),
  KEY `FK_CharacterIntroTemplates_FutureProgs_idx` (`AppliesToCharacterProgId`),
  CONSTRAINT `FK_CharacterIntroTemplates_FutureProgs` FOREIGN KEY (`AppliesToCharacterProgId`) REFERENCES `FutureProgs` (`Id`) ON DELETE RESTRICT
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `CharacterIntroTemplates`
--

LOCK TABLES `CharacterIntroTemplates` WRITE;
/*!40000 ALTER TABLE `CharacterIntroTemplates` DISABLE KEYS */;
/*!40000 ALTER TABLE `CharacterIntroTemplates` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `CharacteristicDefinitions`
--

DROP TABLE IF EXISTS `CharacteristicDefinitions`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `CharacteristicDefinitions` (
  `Id` bigint NOT NULL AUTO_INCREMENT,
  `Name` varchar(4000) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
  `Type` int NOT NULL,
  `Pattern` varchar(4000) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
  `Description` varchar(4000) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
  `ParentId` bigint DEFAULT NULL,
  `ChargenDisplayType` int DEFAULT NULL,
  `Model` varchar(45) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL DEFAULT 'standard',
  `Definition` text CHARACTER SET utf8 COLLATE utf8_general_ci,
  PRIMARY KEY (`Id`),
  KEY `FK_CharacteristicDefinitions_Parent` (`ParentId`),
  CONSTRAINT `FK_CharacteristicDefinitions_Parent` FOREIGN KEY (`ParentId`) REFERENCES `CharacteristicDefinitions` (`Id`) ON DELETE RESTRICT
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `CharacteristicDefinitions`
--

LOCK TABLES `CharacteristicDefinitions` WRITE;
/*!40000 ALTER TABLE `CharacteristicDefinitions` DISABLE KEYS */;
/*!40000 ALTER TABLE `CharacteristicDefinitions` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `CharacteristicProfiles`
--

DROP TABLE IF EXISTS `CharacteristicProfiles`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `CharacteristicProfiles` (
  `Id` bigint NOT NULL AUTO_INCREMENT,
  `Name` varchar(50) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
  `Definition` text CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
  `Type` varchar(50) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
  `TargetDefinitionId` bigint NOT NULL,
  `Description` varchar(4000) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
  PRIMARY KEY (`Id`),
  KEY `FK_CharacteristicProfiles_CharacteristicDefinitions` (`TargetDefinitionId`),
  CONSTRAINT `FK_CharacteristicProfiles_CharacteristicDefinitions` FOREIGN KEY (`TargetDefinitionId`) REFERENCES `CharacteristicDefinitions` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `CharacteristicProfiles`
--

LOCK TABLES `CharacteristicProfiles` WRITE;
/*!40000 ALTER TABLE `CharacteristicProfiles` DISABLE KEYS */;
/*!40000 ALTER TABLE `CharacteristicProfiles` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `Characteristics`
--

DROP TABLE IF EXISTS `Characteristics`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `Characteristics` (
  `BodyId` bigint NOT NULL,
  `Type` int NOT NULL,
  `CharacteristicId` bigint NOT NULL,
  PRIMARY KEY (`BodyId`,`Type`),
  KEY `FK_Characteristics_CharacteristicValues` (`CharacteristicId`),
  CONSTRAINT `FK_Characteristics_Bodies` FOREIGN KEY (`BodyId`) REFERENCES `Bodies` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_Characteristics_CharacteristicValues` FOREIGN KEY (`CharacteristicId`) REFERENCES `CharacteristicValues` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `Characteristics`
--

LOCK TABLES `Characteristics` WRITE;
/*!40000 ALTER TABLE `Characteristics` DISABLE KEYS */;
/*!40000 ALTER TABLE `Characteristics` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `CharacteristicValues`
--

DROP TABLE IF EXISTS `CharacteristicValues`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `CharacteristicValues` (
  `Id` bigint NOT NULL AUTO_INCREMENT,
  `Name` varchar(4000) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
  `DefinitionId` bigint NOT NULL,
  `Value` varchar(50) CHARACTER SET utf8 COLLATE utf8_general_ci DEFAULT NULL,
  `Default` bit(1) NOT NULL DEFAULT b'0',
  `AdditionalValue` varchar(4000) CHARACTER SET utf8 COLLATE utf8_general_ci DEFAULT NULL,
  `FutureProgId` bigint DEFAULT NULL,
  `Pluralisation` int NOT NULL,
  `OngoingValidityProgId` bigint DEFAULT NULL,
  PRIMARY KEY (`Id`),
  KEY `FK_CharacteristicValues_CharacteristicDefinitions` (`DefinitionId`),
  KEY `FK_CharacteristicValues_FutureProgs` (`FutureProgId`),
  KEY `IX_CharacteristicValues_OngoingValidityProgId` (`OngoingValidityProgId`),
  CONSTRAINT `FK_CharacteristicValues_CharacteristicDefinitions` FOREIGN KEY (`DefinitionId`) REFERENCES `CharacteristicDefinitions` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_CharacteristicValues_FutureProgs` FOREIGN KEY (`FutureProgId`) REFERENCES `FutureProgs` (`Id`) ON DELETE SET NULL,
  CONSTRAINT `FK_CharacteristicValues_FutureProgs_Ongoing` FOREIGN KEY (`OngoingValidityProgId`) REFERENCES `FutureProgs` (`Id`) ON DELETE SET NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `CharacteristicValues`
--

LOCK TABLES `CharacteristicValues` WRITE;
/*!40000 ALTER TABLE `CharacteristicValues` DISABLE KEYS */;
/*!40000 ALTER TABLE `CharacteristicValues` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `CharacterKnowledges`
--

DROP TABLE IF EXISTS `CharacterKnowledges`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `CharacterKnowledges` (
  `Id` bigint NOT NULL AUTO_INCREMENT,
  `CharacterId` bigint NOT NULL,
  `KnowledgeId` bigint NOT NULL,
  `WhenAcquired` datetime NOT NULL,
  `HowAcquired` varchar(200) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
  `TimesTaught` int NOT NULL,
  PRIMARY KEY (`Id`),
  KEY `FK_CHARACTERKNOWLEDGES_CHARACTERS` (`CharacterId`),
  KEY `FK_CHARACTERKNOWLEDGES_KNOWLEDGES_idx` (`KnowledgeId`),
  CONSTRAINT `FK_CHARACTERKNOWLEDGES_CHARACTERS` FOREIGN KEY (`CharacterId`) REFERENCES `Characters` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_CHARACTERKNOWLEDGES_KNOWLEDGES` FOREIGN KEY (`KnowledgeId`) REFERENCES `knowledges` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `CharacterKnowledges`
--

LOCK TABLES `CharacterKnowledges` WRITE;
/*!40000 ALTER TABLE `CharacterKnowledges` DISABLE KEYS */;
/*!40000 ALTER TABLE `CharacterKnowledges` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `CharacterLog`
--

DROP TABLE IF EXISTS `CharacterLog`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `CharacterLog` (
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
  CONSTRAINT `FK_CharacterLog_Accounts` FOREIGN KEY (`AccountId`) REFERENCES `Accounts` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_CharacterLog_Cells` FOREIGN KEY (`CellId`) REFERENCES `Cells` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_CharacterLog_Characters` FOREIGN KEY (`CharacterId`) REFERENCES `Characters` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `CharacterLog`
--

LOCK TABLES `CharacterLog` WRITE;
/*!40000 ALTER TABLE `CharacterLog` DISABLE KEYS */;
/*!40000 ALTER TABLE `CharacterLog` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `Characters`
--

DROP TABLE IF EXISTS `Characters`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `Characters` (
  `Id` bigint NOT NULL AUTO_INCREMENT,
  `Name` varchar(4000) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
  `AccountId` bigint DEFAULT NULL,
  `CreationTime` datetime NOT NULL,
  `DeathTime` datetime DEFAULT NULL,
  `Status` int NOT NULL,
  `State` int NOT NULL,
  `Gender` smallint NOT NULL,
  `Location` bigint NOT NULL,
  `BodyId` bigint NOT NULL,
  `CultureId` bigint NOT NULL,
  `EffectData` mediumtext CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
  `BirthdayDate` varchar(4000) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
  `BirthdayCalendarId` bigint NOT NULL,
  `IsAdminAvatar` bit(1) NOT NULL DEFAULT b'0',
  `CurrencyId` bigint DEFAULT NULL,
  `TotalMinutesPlayed` int NOT NULL,
  `AlcoholLitres` double NOT NULL,
  `WaterLitres` double NOT NULL,
  `FoodSatiatedHours` double NOT NULL,
  `DrinkSatiatedHours` double NOT NULL,
  `Calories` double NOT NULL,
  `NeedsModel` varchar(50) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL DEFAULT 'NoNeeds',
  `LongTermPlan` text CHARACTER SET utf8 COLLATE utf8_general_ci,
  `ShortTermPlan` text CHARACTER SET utf8 COLLATE utf8_general_ci,
  `ShownIntroductionMessage` bit(1) NOT NULL DEFAULT b'0',
  `IntroductionMessage` text CHARACTER SET utf8 COLLATE utf8_general_ci,
  `ChargenId` bigint DEFAULT NULL,
  `CurrentCombatSettingId` bigint DEFAULT NULL,
  `PreferredDefenseType` int NOT NULL,
  `PositionId` int NOT NULL DEFAULT '1',
  `PositionModifier` int NOT NULL,
  `PositionTargetId` bigint DEFAULT NULL,
  `PositionTargetType` varchar(45) CHARACTER SET utf8 COLLATE utf8_general_ci DEFAULT NULL,
  `PositionEmote` text CHARACTER SET utf8 COLLATE utf8_general_ci,
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
  `Outfits` text CHARACTER SET utf8 COLLATE utf8_general_ci,
  `CurrentProjectLabourId` bigint DEFAULT NULL,
  `CurrentProjectId` bigint DEFAULT NULL,
  `CurrentProjectHours` double NOT NULL,
  `NameInfo` text CHARACTER SET utf8 COLLATE utf8_general_ci,
  `RoomLayer` int NOT NULL,
  `NoMercy` bit(1) NOT NULL DEFAULT b'0',
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
  CONSTRAINT `FK_Characters_Accents` FOREIGN KEY (`CurrentAccentId`) REFERENCES `Accents` (`Id`) ON DELETE SET NULL,
  CONSTRAINT `FK_Characters_Accounts` FOREIGN KEY (`AccountId`) REFERENCES `Accounts` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_Characters_ActiveProjects` FOREIGN KEY (`CurrentProjectId`) REFERENCES `ActiveProjects` (`Id`) ON DELETE SET NULL,
  CONSTRAINT `FK_Characters_Bodies` FOREIGN KEY (`BodyId`) REFERENCES `Bodies` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_Characters_Cells` FOREIGN KEY (`Location`) REFERENCES `Cells` (`Id`) ON DELETE RESTRICT,
  CONSTRAINT `FK_Characters_Chargens` FOREIGN KEY (`ChargenId`) REFERENCES `Chargens` (`Id`) ON DELETE SET NULL,
  CONSTRAINT `FK_Characters_Cultures` FOREIGN KEY (`CultureId`) REFERENCES `Cultures` (`Id`) ON DELETE RESTRICT,
  CONSTRAINT `FK_Characters_Currencies` FOREIGN KEY (`CurrencyId`) REFERENCES `Currencies` (`Id`) ON DELETE RESTRICT,
  CONSTRAINT `FK_Characters_Languages_Spoken` FOREIGN KEY (`CurrentLanguageId`) REFERENCES `Languages` (`Id`) ON DELETE SET NULL,
  CONSTRAINT `FK_Characters_Languages_Written` FOREIGN KEY (`CurrentWritingLanguageId`) REFERENCES `Languages` (`Id`) ON DELETE SET NULL,
  CONSTRAINT `FK_Characters_ProjectLabourRequirements` FOREIGN KEY (`CurrentProjectLabourId`) REFERENCES `ProjectLabourRequirements` (`Id`) ON DELETE SET NULL,
  CONSTRAINT `FK_Characters_Scripts` FOREIGN KEY (`CurrentScriptId`) REFERENCES `Scripts` (`Id`) ON DELETE SET NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `Characters`
--

LOCK TABLES `Characters` WRITE;
/*!40000 ALTER TABLE `Characters` DISABLE KEYS */;
/*!40000 ALTER TABLE `Characters` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `Characters_Accents`
--

DROP TABLE IF EXISTS `Characters_Accents`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `Characters_Accents` (
  `CharacterId` bigint NOT NULL,
  `AccentId` bigint NOT NULL,
  `Familiarity` int NOT NULL,
  `IsPreferred` bit(1) NOT NULL DEFAULT b'0',
  PRIMARY KEY (`CharacterId`,`AccentId`),
  KEY `FK_Characters_Accents_Accents_idx` (`AccentId`),
  CONSTRAINT `FK_Characters_Accents_Accents` FOREIGN KEY (`AccentId`) REFERENCES `Accents` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_Characters_Accents_Characters` FOREIGN KEY (`CharacterId`) REFERENCES `Characters` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `Characters_Accents`
--

LOCK TABLES `Characters_Accents` WRITE;
/*!40000 ALTER TABLE `Characters_Accents` DISABLE KEYS */;
/*!40000 ALTER TABLE `Characters_Accents` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `Characters_ChargenRoles`
--

DROP TABLE IF EXISTS `Characters_ChargenRoles`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `Characters_ChargenRoles` (
  `CharacterId` bigint NOT NULL,
  `ChargenRoleId` bigint NOT NULL,
  PRIMARY KEY (`CharacterId`,`ChargenRoleId`),
  KEY `FK_Characters_ChargenRoles_ChargenRoles` (`ChargenRoleId`),
  CONSTRAINT `FK_Characters_ChargenRoles_Characters` FOREIGN KEY (`CharacterId`) REFERENCES `Characters` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_Characters_ChargenRoles_ChargenRoles` FOREIGN KEY (`ChargenRoleId`) REFERENCES `ChargenRoles` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `Characters_ChargenRoles`
--

LOCK TABLES `Characters_ChargenRoles` WRITE;
/*!40000 ALTER TABLE `Characters_ChargenRoles` DISABLE KEYS */;
/*!40000 ALTER TABLE `Characters_ChargenRoles` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `Characters_Languages`
--

DROP TABLE IF EXISTS `Characters_Languages`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `Characters_Languages` (
  `CharacterId` bigint NOT NULL,
  `LanguageId` bigint NOT NULL,
  PRIMARY KEY (`CharacterId`,`LanguageId`),
  KEY `FK_Characters_Languages_Languages_idx` (`LanguageId`),
  CONSTRAINT `FK_Characters_Languages_Characters` FOREIGN KEY (`CharacterId`) REFERENCES `Characters` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_Characters_Languages_Languages` FOREIGN KEY (`LanguageId`) REFERENCES `Languages` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `Characters_Languages`
--

LOCK TABLES `Characters_Languages` WRITE;
/*!40000 ALTER TABLE `Characters_Languages` DISABLE KEYS */;
/*!40000 ALTER TABLE `Characters_Languages` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `Characters_MagicResources`
--

DROP TABLE IF EXISTS `Characters_MagicResources`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `Characters_MagicResources` (
  `CharacterId` bigint NOT NULL,
  `MagicResourceId` bigint NOT NULL,
  `Amount` double NOT NULL,
  PRIMARY KEY (`CharacterId`,`MagicResourceId`),
  KEY `FK_Characters_MagicResources_MagicResources_idx` (`MagicResourceId`),
  CONSTRAINT `FK_Characters_MagicResources_Characters` FOREIGN KEY (`CharacterId`) REFERENCES `Characters` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_Characters_MagicResources_MagicResources` FOREIGN KEY (`MagicResourceId`) REFERENCES `MagicResources` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `Characters_MagicResources`
--

LOCK TABLES `Characters_MagicResources` WRITE;
/*!40000 ALTER TABLE `Characters_MagicResources` DISABLE KEYS */;
/*!40000 ALTER TABLE `Characters_MagicResources` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `Characters_Scripts`
--

DROP TABLE IF EXISTS `Characters_Scripts`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `Characters_Scripts` (
  `CharacterId` bigint NOT NULL,
  `ScriptId` bigint NOT NULL,
  PRIMARY KEY (`CharacterId`,`ScriptId`),
  KEY `FK_Characters_Scripts_Scripts_idx` (`ScriptId`),
  CONSTRAINT `FK_Characters_Scripts_Characters` FOREIGN KEY (`CharacterId`) REFERENCES `Characters` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_Characters_Scripts_Scripts` FOREIGN KEY (`ScriptId`) REFERENCES `Scripts` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `Characters_Scripts`
--

LOCK TABLES `Characters_Scripts` WRITE;
/*!40000 ALTER TABLE `Characters_Scripts` DISABLE KEYS */;
/*!40000 ALTER TABLE `Characters_Scripts` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `ChargenAdvices`
--

DROP TABLE IF EXISTS `ChargenAdvices`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `ChargenAdvices` (
  `Id` bigint NOT NULL AUTO_INCREMENT,
  `ChargenStage` int NOT NULL,
  `AdviceTitle` text CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
  `AdviceText` text CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
  `ShouldShowAdviceProgId` bigint DEFAULT NULL,
  PRIMARY KEY (`Id`),
  KEY `FK_ChargenAdvices_FutureProgs_idx` (`ShouldShowAdviceProgId`),
  CONSTRAINT `FK_ChargenAdvices_FutureProgs` FOREIGN KEY (`ShouldShowAdviceProgId`) REFERENCES `FutureProgs` (`Id`) ON DELETE SET NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `ChargenAdvices`
--

LOCK TABLES `ChargenAdvices` WRITE;
/*!40000 ALTER TABLE `ChargenAdvices` DISABLE KEYS */;
/*!40000 ALTER TABLE `ChargenAdvices` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `ChargenAdvices_ChargenRoles`
--

DROP TABLE IF EXISTS `ChargenAdvices_ChargenRoles`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `ChargenAdvices_ChargenRoles` (
  `ChargenAdviceId` bigint NOT NULL,
  `ChargenRoleId` bigint NOT NULL,
  PRIMARY KEY (`ChargenAdviceId`,`ChargenRoleId`),
  KEY `FK_ChargenAdvices_ChargenRoles_ChargenRoles_idx` (`ChargenRoleId`),
  CONSTRAINT `FK_ChargenAdvices_ChargenRoles_ChargenAdvices` FOREIGN KEY (`ChargenAdviceId`) REFERENCES `ChargenAdvices` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_ChargenAdvices_ChargenRoles_ChargenRoles` FOREIGN KEY (`ChargenRoleId`) REFERENCES `ChargenRoles` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `ChargenAdvices_ChargenRoles`
--

LOCK TABLES `ChargenAdvices_ChargenRoles` WRITE;
/*!40000 ALTER TABLE `ChargenAdvices_ChargenRoles` DISABLE KEYS */;
/*!40000 ALTER TABLE `ChargenAdvices_ChargenRoles` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `ChargenAdvices_Cultures`
--

DROP TABLE IF EXISTS `ChargenAdvices_Cultures`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `ChargenAdvices_Cultures` (
  `ChargenAdviceId` bigint NOT NULL,
  `CultureId` bigint NOT NULL,
  PRIMARY KEY (`ChargenAdviceId`,`CultureId`),
  KEY `FK_ChargenAdvices_Cultures_Cultures_idx` (`CultureId`),
  CONSTRAINT `FK_ChargenAdvices_Cultures_ChargenAdvices` FOREIGN KEY (`ChargenAdviceId`) REFERENCES `ChargenAdvices` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_ChargenAdvices_Cultures_Cultures` FOREIGN KEY (`CultureId`) REFERENCES `Cultures` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `ChargenAdvices_Cultures`
--

LOCK TABLES `ChargenAdvices_Cultures` WRITE;
/*!40000 ALTER TABLE `ChargenAdvices_Cultures` DISABLE KEYS */;
/*!40000 ALTER TABLE `ChargenAdvices_Cultures` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `ChargenAdvices_Ethnicities`
--

DROP TABLE IF EXISTS `ChargenAdvices_Ethnicities`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `ChargenAdvices_Ethnicities` (
  `ChargenAdviceId` bigint NOT NULL,
  `EthnicityId` bigint NOT NULL,
  PRIMARY KEY (`ChargenAdviceId`,`EthnicityId`),
  KEY `FK_ChargenAdvices_Ethnicities_Ethnicities_idx` (`EthnicityId`),
  CONSTRAINT `FK_ChargenAdvices_Ethnicities_ChargenAdvices` FOREIGN KEY (`ChargenAdviceId`) REFERENCES `ChargenAdvices` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_ChargenAdvices_Ethnicities_Ethnicities` FOREIGN KEY (`EthnicityId`) REFERENCES `Ethnicities` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `ChargenAdvices_Ethnicities`
--

LOCK TABLES `ChargenAdvices_Ethnicities` WRITE;
/*!40000 ALTER TABLE `ChargenAdvices_Ethnicities` DISABLE KEYS */;
/*!40000 ALTER TABLE `ChargenAdvices_Ethnicities` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `ChargenAdvices_Races`
--

DROP TABLE IF EXISTS `ChargenAdvices_Races`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `ChargenAdvices_Races` (
  `ChargenAdviceId` bigint NOT NULL,
  `RaceId` bigint NOT NULL,
  PRIMARY KEY (`ChargenAdviceId`,`RaceId`),
  KEY `FK_ChargenAdvices_Races_Races_idx` (`RaceId`),
  CONSTRAINT `FK_ChargenAdvices_Races_ChargenAdvices` FOREIGN KEY (`ChargenAdviceId`) REFERENCES `ChargenAdvices` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_ChargenAdvices_Races_Races` FOREIGN KEY (`RaceId`) REFERENCES `Races` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `ChargenAdvices_Races`
--

LOCK TABLES `ChargenAdvices_Races` WRITE;
/*!40000 ALTER TABLE `ChargenAdvices_Races` DISABLE KEYS */;
/*!40000 ALTER TABLE `ChargenAdvices_Races` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `ChargenResources`
--

DROP TABLE IF EXISTS `ChargenResources`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `ChargenResources` (
  `Id` bigint NOT NULL AUTO_INCREMENT,
  `Name` varchar(50) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
  `PluralName` varchar(50) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
  `Alias` varchar(50) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
  `MinimumTimeBetweenAwards` int NOT NULL,
  `MaximumNumberAwardedPerAward` double NOT NULL,
  `PermissionLevelRequiredToAward` int NOT NULL,
  `PermissionLevelRequiredToCircumventMinimumTime` int NOT NULL,
  `ShowToPlayerInScore` bit(1) NOT NULL,
  `TextDisplayedToPlayerOnAward` varchar(4000) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
  `TextDisplayedToPlayerOnDeduct` varchar(4000) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
  `MaximumResourceId` bigint DEFAULT NULL,
  `MaximumResourceFormula` varchar(4000) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
  `Type` varchar(50) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
  `ControlProgId` bigint DEFAULT NULL,
  `FK_ChargenResources_FutureProgs` bigint DEFAULT NULL,
  PRIMARY KEY (`Id`),
  KEY `IX_ChargenResources_FK_ChargenResources_FutureProgs` (`FK_ChargenResources_FutureProgs`),
  CONSTRAINT `FK_ChargenResources_FutureProgs_FK_ChargenResources_FutureProgs` FOREIGN KEY (`FK_ChargenResources_FutureProgs`) REFERENCES `FutureProgs` (`Id`) ON DELETE SET NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `ChargenResources`
--

LOCK TABLES `ChargenResources` WRITE;
/*!40000 ALTER TABLE `ChargenResources` DISABLE KEYS */;
/*!40000 ALTER TABLE `ChargenResources` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `ChargenRoles`
--

DROP TABLE IF EXISTS `ChargenRoles`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `ChargenRoles` (
  `Id` bigint NOT NULL AUTO_INCREMENT,
  `Name` varchar(4000) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
  `Type` int NOT NULL,
  `PosterId` bigint NOT NULL,
  `MaximumNumberAlive` int NOT NULL,
  `MaximumNumberTotal` int NOT NULL,
  `ChargenBlurb` varchar(4000) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
  `AvailabilityProgId` bigint DEFAULT NULL,
  `Expired` bit(1) NOT NULL DEFAULT b'0',
  `MinimumAuthorityToApprove` int NOT NULL,
  `MinimumAuthorityToView` int NOT NULL,
  PRIMARY KEY (`Id`),
  KEY `FK_ChargenRoles_FutureProgs` (`AvailabilityProgId`),
  KEY `FK_ChargenRoles_Accounts` (`PosterId`),
  CONSTRAINT `FK_ChargenRoles_Accounts` FOREIGN KEY (`PosterId`) REFERENCES `Accounts` (`Id`) ON DELETE RESTRICT,
  CONSTRAINT `FK_ChargenRoles_FutureProgs` FOREIGN KEY (`AvailabilityProgId`) REFERENCES `FutureProgs` (`Id`) ON DELETE SET NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `ChargenRoles`
--

LOCK TABLES `ChargenRoles` WRITE;
/*!40000 ALTER TABLE `ChargenRoles` DISABLE KEYS */;
/*!40000 ALTER TABLE `ChargenRoles` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `ChargenRoles_Approvers`
--

DROP TABLE IF EXISTS `ChargenRoles_Approvers`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `ChargenRoles_Approvers` (
  `ChargenRoleId` bigint NOT NULL,
  `ApproverId` bigint NOT NULL,
  PRIMARY KEY (`ChargenRoleId`,`ApproverId`),
  KEY `FK_ChargenRoles_Approvers_Accounts` (`ApproverId`),
  CONSTRAINT `FK_ChargenRoles_Approvers_Accounts` FOREIGN KEY (`ApproverId`) REFERENCES `Accounts` (`Id`) ON DELETE RESTRICT,
  CONSTRAINT `FK_ChargenRoles_Approvers_ChargenRoles` FOREIGN KEY (`ChargenRoleId`) REFERENCES `ChargenRoles` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `ChargenRoles_Approvers`
--

LOCK TABLES `ChargenRoles_Approvers` WRITE;
/*!40000 ALTER TABLE `ChargenRoles_Approvers` DISABLE KEYS */;
/*!40000 ALTER TABLE `ChargenRoles_Approvers` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `ChargenRoles_ClanMemberships`
--

DROP TABLE IF EXISTS `ChargenRoles_ClanMemberships`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `ChargenRoles_ClanMemberships` (
  `ChargenRoleId` bigint NOT NULL,
  `ClanId` bigint NOT NULL,
  `RankId` bigint NOT NULL,
  `PaygradeId` bigint DEFAULT NULL,
  PRIMARY KEY (`ChargenRoleId`,`ClanId`),
  KEY `FK_ChargenRoles_ClanMemberships_Clans` (`ClanId`),
  CONSTRAINT `FK_ChargenRoles_ClanMemberships_ChargenRoles` FOREIGN KEY (`ChargenRoleId`) REFERENCES `ChargenRoles` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_ChargenRoles_ClanMemberships_Clans` FOREIGN KEY (`ClanId`) REFERENCES `Clans` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `ChargenRoles_ClanMemberships`
--

LOCK TABLES `ChargenRoles_ClanMemberships` WRITE;
/*!40000 ALTER TABLE `ChargenRoles_ClanMemberships` DISABLE KEYS */;
/*!40000 ALTER TABLE `ChargenRoles_ClanMemberships` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `ChargenRoles_ClanMemberships_Appointments`
--

DROP TABLE IF EXISTS `ChargenRoles_ClanMemberships_Appointments`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `ChargenRoles_ClanMemberships_Appointments` (
  `ChargenRoleId` bigint NOT NULL,
  `ClanId` bigint NOT NULL,
  `AppointmentId` bigint NOT NULL,
  PRIMARY KEY (`ChargenRoleId`,`ClanId`,`AppointmentId`),
  CONSTRAINT `FK_CRCMA_ChargenRoles_ClanMemberships` FOREIGN KEY (`ChargenRoleId`, `ClanId`) REFERENCES `ChargenRoles_ClanMemberships` (`ChargenRoleId`, `ClanId`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `ChargenRoles_ClanMemberships_Appointments`
--

LOCK TABLES `ChargenRoles_ClanMemberships_Appointments` WRITE;
/*!40000 ALTER TABLE `ChargenRoles_ClanMemberships_Appointments` DISABLE KEYS */;
/*!40000 ALTER TABLE `ChargenRoles_ClanMemberships_Appointments` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `ChargenRoles_Costs`
--

DROP TABLE IF EXISTS `ChargenRoles_Costs`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `ChargenRoles_Costs` (
  `ChargenRoleId` bigint NOT NULL,
  `ChargenResourceId` bigint NOT NULL,
  `RequirementOnly` bit(1) NOT NULL DEFAULT b'0',
  `Amount` int NOT NULL,
  PRIMARY KEY (`ChargenRoleId`,`ChargenResourceId`,`RequirementOnly`),
  KEY `FK_ChargenRoles_Costs_ChargenResources` (`ChargenResourceId`),
  CONSTRAINT `FK_ChargenRoles_Costs_ChargenResources` FOREIGN KEY (`ChargenResourceId`) REFERENCES `ChargenResources` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_ChargenRoles_Costs_ChargenRoles` FOREIGN KEY (`ChargenRoleId`) REFERENCES `ChargenRoles` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `ChargenRoles_Costs`
--

LOCK TABLES `ChargenRoles_Costs` WRITE;
/*!40000 ALTER TABLE `ChargenRoles_Costs` DISABLE KEYS */;
/*!40000 ALTER TABLE `ChargenRoles_Costs` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `ChargenRoles_Currencies`
--

DROP TABLE IF EXISTS `ChargenRoles_Currencies`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `ChargenRoles_Currencies` (
  `ChargenRoleId` bigint NOT NULL,
  `CurrencyId` bigint NOT NULL,
  `Amount` decimal(58,29) NOT NULL,
  PRIMARY KEY (`ChargenRoleId`,`CurrencyId`),
  KEY `FK_ChargenRoles_Currencies_Currencies` (`CurrencyId`),
  CONSTRAINT `FK_ChargenRoles_Currencies_ChargenRoles` FOREIGN KEY (`ChargenRoleId`) REFERENCES `ChargenRoles` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_ChargenRoles_Currencies_Currencies` FOREIGN KEY (`CurrencyId`) REFERENCES `Currencies` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `ChargenRoles_Currencies`
--

LOCK TABLES `ChargenRoles_Currencies` WRITE;
/*!40000 ALTER TABLE `ChargenRoles_Currencies` DISABLE KEYS */;
/*!40000 ALTER TABLE `ChargenRoles_Currencies` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `ChargenRoles_Merits`
--

DROP TABLE IF EXISTS `ChargenRoles_Merits`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `ChargenRoles_Merits` (
  `ChargenRoleId` bigint NOT NULL,
  `MeritId` bigint NOT NULL,
  PRIMARY KEY (`ChargenRoleId`,`MeritId`),
  KEY `FK_ChargenRoles_Merits_Merits_idx` (`MeritId`),
  CONSTRAINT `FK_ChargenRoles_Merits_ChargenRoles` FOREIGN KEY (`ChargenRoleId`) REFERENCES `ChargenRoles` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_ChargenRoles_Merits_Merits` FOREIGN KEY (`MeritId`) REFERENCES `Merits` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `ChargenRoles_Merits`
--

LOCK TABLES `ChargenRoles_Merits` WRITE;
/*!40000 ALTER TABLE `ChargenRoles_Merits` DISABLE KEYS */;
/*!40000 ALTER TABLE `ChargenRoles_Merits` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `ChargenRoles_Traits`
--

DROP TABLE IF EXISTS `ChargenRoles_Traits`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `ChargenRoles_Traits` (
  `ChargenRoleId` bigint NOT NULL,
  `TraitId` bigint NOT NULL,
  `Amount` double NOT NULL,
  `GiveIfDoesntHave` bit(1) NOT NULL DEFAULT b'0',
  PRIMARY KEY (`ChargenRoleId`,`TraitId`),
  KEY `FK_ChargenRoles_Traits_Currencies` (`TraitId`),
  CONSTRAINT `FK_ChargenRoles_Traits_ChargenRoles` FOREIGN KEY (`ChargenRoleId`) REFERENCES `ChargenRoles` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_ChargenRoles_Traits_Currencies` FOREIGN KEY (`TraitId`) REFERENCES `TraitDefinitions` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `ChargenRoles_Traits`
--

LOCK TABLES `ChargenRoles_Traits` WRITE;
/*!40000 ALTER TABLE `ChargenRoles_Traits` DISABLE KEYS */;
/*!40000 ALTER TABLE `ChargenRoles_Traits` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `Chargens`
--

DROP TABLE IF EXISTS `Chargens`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `Chargens` (
  `Id` bigint NOT NULL AUTO_INCREMENT,
  `AccountId` bigint NOT NULL,
  `Name` varchar(12000) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
  `Definition` text CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
  `Status` int NOT NULL,
  `SubmitTime` datetime DEFAULT NULL,
  `MinimumApprovalAuthority` int DEFAULT NULL,
  `ApprovedById` bigint DEFAULT NULL,
  `ApprovalTime` datetime DEFAULT NULL,
  PRIMARY KEY (`Id`),
  KEY `FK_Chargens_Accounts` (`AccountId`),
  CONSTRAINT `FK_Chargens_Accounts` FOREIGN KEY (`AccountId`) REFERENCES `Accounts` (`Id`) ON DELETE RESTRICT
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `Chargens`
--

LOCK TABLES `Chargens` WRITE;
/*!40000 ALTER TABLE `Chargens` DISABLE KEYS */;
/*!40000 ALTER TABLE `Chargens` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `ChargenScreenStoryboardDependentStages`
--

DROP TABLE IF EXISTS `ChargenScreenStoryboardDependentStages`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `ChargenScreenStoryboardDependentStages` (
  `OwnerId` bigint NOT NULL,
  `Dependency` int NOT NULL,
  PRIMARY KEY (`OwnerId`,`Dependency`),
  KEY `FK_ChargenScreenStoryboardDependentStages_Owner` (`OwnerId`),
  CONSTRAINT `FK_ChargenScreenStoryboardDependentStages_Owner` FOREIGN KEY (`OwnerId`) REFERENCES `ChargenScreenStoryboards` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `ChargenScreenStoryboardDependentStages`
--

LOCK TABLES `ChargenScreenStoryboardDependentStages` WRITE;
/*!40000 ALTER TABLE `ChargenScreenStoryboardDependentStages` DISABLE KEYS */;
/*!40000 ALTER TABLE `ChargenScreenStoryboardDependentStages` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `ChargenScreenStoryboards`
--

DROP TABLE IF EXISTS `ChargenScreenStoryboards`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `ChargenScreenStoryboards` (
  `Id` bigint NOT NULL AUTO_INCREMENT,
  `ChargenType` varchar(50) DEFAULT NULL,
  `ChargenStage` int NOT NULL,
  `Order` int NOT NULL,
  `StageDefinition` mediumtext CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
  `NextStage` int NOT NULL,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `ChargenScreenStoryboards`
--

LOCK TABLES `ChargenScreenStoryboards` WRITE;
/*!40000 ALTER TABLE `ChargenScreenStoryboards` DISABLE KEYS */;
/*!40000 ALTER TABLE `ChargenScreenStoryboards` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `Checks`
--

DROP TABLE IF EXISTS `Checks`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `Checks` (
  `Type` int NOT NULL,
  `TraitExpressionId` bigint NOT NULL,
  `CheckTemplateId` bigint NOT NULL,
  `MaximumDifficultyForImprovement` int NOT NULL DEFAULT '10',
  PRIMARY KEY (`Type`),
  KEY `FK_Checks_CheckTemplates` (`CheckTemplateId`),
  KEY `FK_Checks_TraitExpression` (`TraitExpressionId`),
  CONSTRAINT `FK_Checks_CheckTemplates` FOREIGN KEY (`CheckTemplateId`) REFERENCES `CheckTemplates` (`Id`) ON DELETE RESTRICT,
  CONSTRAINT `FK_Checks_TraitExpression` FOREIGN KEY (`TraitExpressionId`) REFERENCES `TraitExpression` (`Id`) ON DELETE RESTRICT
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `Checks`
--

LOCK TABLES `Checks` WRITE;
/*!40000 ALTER TABLE `Checks` DISABLE KEYS */;
/*!40000 ALTER TABLE `Checks` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `CheckTemplateDifficulties`
--

DROP TABLE IF EXISTS `CheckTemplateDifficulties`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `CheckTemplateDifficulties` (
  `CheckTemplateId` bigint NOT NULL,
  `Difficulty` int NOT NULL,
  `Modifier` double NOT NULL,
  PRIMARY KEY (`Difficulty`,`CheckTemplateId`),
  KEY `FK_CheckTemplateDifficulties_CheckTemplates` (`CheckTemplateId`),
  CONSTRAINT `FK_CheckTemplateDifficulties_CheckTemplates` FOREIGN KEY (`CheckTemplateId`) REFERENCES `CheckTemplates` (`Id`) ON DELETE RESTRICT
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `CheckTemplateDifficulties`
--

LOCK TABLES `CheckTemplateDifficulties` WRITE;
/*!40000 ALTER TABLE `CheckTemplateDifficulties` DISABLE KEYS */;
/*!40000 ALTER TABLE `CheckTemplateDifficulties` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `CheckTemplates`
--

DROP TABLE IF EXISTS `CheckTemplates`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `CheckTemplates` (
  `Id` bigint NOT NULL AUTO_INCREMENT,
  `Name` varchar(4000) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
  `Definition` text CHARACTER SET utf8 COLLATE utf8_general_ci,
  `CheckMethod` varchar(25) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL DEFAULT 'Standard',
  `ImproveTraits` bit(1) NOT NULL DEFAULT b'0',
  `FailIfTraitMissingMode` smallint NOT NULL,
  `CanBranchIfTraitMissing` bit(1) NOT NULL DEFAULT b'0',
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `CheckTemplates`
--

LOCK TABLES `CheckTemplates` WRITE;
/*!40000 ALTER TABLE `CheckTemplates` DISABLE KEYS */;
/*!40000 ALTER TABLE `CheckTemplates` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `ClanMemberships`
--

DROP TABLE IF EXISTS `ClanMemberships`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `ClanMemberships` (
  `ClanId` bigint NOT NULL,
  `CharacterId` bigint NOT NULL,
  `RankId` bigint NOT NULL,
  `PaygradeId` bigint DEFAULT NULL,
  `JoinDate` varchar(100) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
  `ManagerId` bigint DEFAULT NULL,
  `PersonalName` text CHARACTER SET utf8 COLLATE utf8_general_ci,
  `ArchivedMembership` bit(1) NOT NULL DEFAULT b'0',
  PRIMARY KEY (`ClanId`,`CharacterId`),
  KEY `FK_ClanMemberships_Characters` (`CharacterId`),
  KEY `FK_ClanMemberships_Manager` (`ManagerId`),
  CONSTRAINT `FK_ClanMemberships_Characters` FOREIGN KEY (`CharacterId`) REFERENCES `Characters` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_ClanMemberships_Clans` FOREIGN KEY (`ClanId`) REFERENCES `Clans` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_ClanMemberships_Manager` FOREIGN KEY (`ManagerId`) REFERENCES `Characters` (`Id`) ON DELETE RESTRICT
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `ClanMemberships`
--

LOCK TABLES `ClanMemberships` WRITE;
/*!40000 ALTER TABLE `ClanMemberships` DISABLE KEYS */;
/*!40000 ALTER TABLE `ClanMemberships` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `ClanMemberships_Appointments`
--

DROP TABLE IF EXISTS `ClanMemberships_Appointments`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `ClanMemberships_Appointments` (
  `ClanId` bigint NOT NULL,
  `CharacterId` bigint NOT NULL,
  `AppointmentId` bigint NOT NULL,
  PRIMARY KEY (`ClanId`,`CharacterId`,`AppointmentId`),
  KEY `FK_ClanMemberships_Appointments_Appointments` (`AppointmentId`),
  CONSTRAINT `FK_ClanMemberships_Appointments_Appointments` FOREIGN KEY (`AppointmentId`) REFERENCES `Appointments` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_ClanMemberships_Appointments_ClanMemberships` FOREIGN KEY (`ClanId`, `CharacterId`) REFERENCES `ClanMemberships` (`ClanId`, `CharacterId`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `ClanMemberships_Appointments`
--

LOCK TABLES `ClanMemberships_Appointments` WRITE;
/*!40000 ALTER TABLE `ClanMemberships_Appointments` DISABLE KEYS */;
/*!40000 ALTER TABLE `ClanMemberships_Appointments` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `ClanMemberships_Backpay`
--

DROP TABLE IF EXISTS `ClanMemberships_Backpay`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `ClanMemberships_Backpay` (
  `ClanId` bigint NOT NULL,
  `CharacterId` bigint NOT NULL,
  `CurrencyId` bigint NOT NULL,
  `Amount` decimal(58,29) NOT NULL,
  PRIMARY KEY (`CurrencyId`,`ClanId`,`CharacterId`),
  KEY `FK_ClanMemberships_Backpay_ClanMemberships` (`ClanId`,`CharacterId`),
  CONSTRAINT `FK_ClanMemberships_Backpay_ClanMemberships` FOREIGN KEY (`ClanId`, `CharacterId`) REFERENCES `ClanMemberships` (`ClanId`, `CharacterId`) ON DELETE CASCADE,
  CONSTRAINT `FK_ClanMemberships_Backpay_Currencies` FOREIGN KEY (`CurrencyId`) REFERENCES `Currencies` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `ClanMemberships_Backpay`
--

LOCK TABLES `ClanMemberships_Backpay` WRITE;
/*!40000 ALTER TABLE `ClanMemberships_Backpay` DISABLE KEYS */;
/*!40000 ALTER TABLE `ClanMemberships_Backpay` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `Clans`
--

DROP TABLE IF EXISTS `Clans`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `Clans` (
  `Id` bigint NOT NULL AUTO_INCREMENT,
  `Name` varchar(100) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
  `Alias` varchar(100) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
  `FullName` varchar(4000) CHARACTER SET utf8 COLLATE utf8_general_ci DEFAULT NULL,
  `Description` varchar(4000) CHARACTER SET utf8 COLLATE utf8_general_ci DEFAULT NULL,
  `ClanId` bigint DEFAULT NULL,
  `PayIntervalType` int NOT NULL,
  `PayIntervalModifier` int NOT NULL,
  `PayIntervalOther` int NOT NULL,
  `CalendarId` bigint NOT NULL,
  `PayIntervalReferenceDate` varchar(100) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
  `PayIntervalReferenceTime` varchar(100) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
  `IsTemplate` bit(1) NOT NULL DEFAULT b'0',
  `ShowClanMembersInWho` bit(1) NOT NULL DEFAULT b'0',
  `PaymasterId` bigint DEFAULT NULL,
  `PaymasterItemProtoId` bigint DEFAULT NULL,
  `OnPayProgId` bigint DEFAULT NULL,
  `MaximumPeriodsOfUncollectedBackPay` int DEFAULT NULL,
  `ShowFamousMembersInNotables` bit(1) NOT NULL DEFAULT b'0',
  `Sphere` varchar(100) CHARACTER SET utf8 COLLATE utf8_general_ci DEFAULT NULL,
  `BankAccountId` bigint DEFAULT NULL,
  `DiscordChannelId` decimal(20,0) DEFAULT NULL,
  PRIMARY KEY (`Id`),
  KEY `FK_Clans_Calendars` (`CalendarId`),
  KEY `FK_Clans_FutureProgs_idx` (`OnPayProgId`),
  KEY `IX_Clans_ClanId` (`ClanId`),
  KEY `FK_Clans_Characters_idx` (`PaymasterId`),
  KEY `IX_Clans_BankAccountId` (`BankAccountId`),
  CONSTRAINT `FK_Clans_BankAccounts` FOREIGN KEY (`BankAccountId`) REFERENCES `BankAccounts` (`Id`),
  CONSTRAINT `FK_Clans_Calendars` FOREIGN KEY (`CalendarId`) REFERENCES `Calendars` (`Id`) ON DELETE RESTRICT,
  CONSTRAINT `FK_Clans_Characters` FOREIGN KEY (`PaymasterId`) REFERENCES `Characters` (`Id`) ON DELETE SET NULL,
  CONSTRAINT `FK_Clans_Clans_ClanId` FOREIGN KEY (`ClanId`) REFERENCES `Clans` (`Id`) ON DELETE RESTRICT,
  CONSTRAINT `FK_Clans_FutureProgs` FOREIGN KEY (`OnPayProgId`) REFERENCES `FutureProgs` (`Id`) ON DELETE SET NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `Clans`
--

LOCK TABLES `Clans` WRITE;
/*!40000 ALTER TABLE `Clans` DISABLE KEYS */;
/*!40000 ALTER TABLE `Clans` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `Clans_AdministrationCells`
--

DROP TABLE IF EXISTS `Clans_AdministrationCells`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `Clans_AdministrationCells` (
  `ClanId` bigint NOT NULL,
  `CellId` bigint NOT NULL,
  PRIMARY KEY (`ClanId`,`CellId`),
  KEY `FK_Clans_AdministrationCells_Cells_idx` (`CellId`),
  CONSTRAINT `FK_Clans_AdministrationCells_Cells` FOREIGN KEY (`CellId`) REFERENCES `Cells` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_Clans_AdministrationCells_Clans` FOREIGN KEY (`ClanId`) REFERENCES `Clans` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `Clans_AdministrationCells`
--

LOCK TABLES `Clans_AdministrationCells` WRITE;
/*!40000 ALTER TABLE `Clans_AdministrationCells` DISABLE KEYS */;
/*!40000 ALTER TABLE `Clans_AdministrationCells` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `Clans_TreasuryCells`
--

DROP TABLE IF EXISTS `Clans_TreasuryCells`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `Clans_TreasuryCells` (
  `ClanId` bigint NOT NULL,
  `CellId` bigint NOT NULL,
  PRIMARY KEY (`ClanId`,`CellId`),
  KEY `FK_Clans_TreasuryCells_Cells_idx` (`CellId`),
  CONSTRAINT `FK_Clans_TreasuryCells_Cells` FOREIGN KEY (`CellId`) REFERENCES `Cells` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_Clans_TreasuryCells_Clans` FOREIGN KEY (`ClanId`) REFERENCES `Clans` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `Clans_TreasuryCells`
--

LOCK TABLES `Clans_TreasuryCells` WRITE;
/*!40000 ALTER TABLE `Clans_TreasuryCells` DISABLE KEYS */;
/*!40000 ALTER TABLE `Clans_TreasuryCells` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `ClimateModels`
--

DROP TABLE IF EXISTS `ClimateModels`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `ClimateModels` (
  `Id` bigint NOT NULL AUTO_INCREMENT,
  `Name` varchar(200) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
  `Type` varchar(45) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
  `MinuteProcessingInterval` int NOT NULL,
  `MinimumMinutesBetweenFlavourEchoes` int NOT NULL,
  `MinuteFlavourEchoChance` double NOT NULL,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `ClimateModels`
--

LOCK TABLES `ClimateModels` WRITE;
/*!40000 ALTER TABLE `ClimateModels` DISABLE KEYS */;
/*!40000 ALTER TABLE `ClimateModels` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `ClimateModelSeason`
--

DROP TABLE IF EXISTS `ClimateModelSeason`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `ClimateModelSeason` (
  `ClimateModelId` bigint NOT NULL,
  `SeasonId` bigint NOT NULL,
  `MaximumAdditionalChangeChanceFromStableWeather` double NOT NULL,
  `IncrementalAdditionalChangeChanceFromStableWeather` double NOT NULL,
  PRIMARY KEY (`ClimateModelId`,`SeasonId`),
  KEY `IX_ClimateModelSeason_SeasonId` (`SeasonId`),
  CONSTRAINT `FK_ClimateModelSeasons_ClimateModels` FOREIGN KEY (`ClimateModelId`) REFERENCES `ClimateModels` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_ClimateModelSeasons_Seasons` FOREIGN KEY (`SeasonId`) REFERENCES `Seasons` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `ClimateModelSeason`
--

LOCK TABLES `ClimateModelSeason` WRITE;
/*!40000 ALTER TABLE `ClimateModelSeason` DISABLE KEYS */;
/*!40000 ALTER TABLE `ClimateModelSeason` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `ClimateModelSeasonEvent`
--

DROP TABLE IF EXISTS `ClimateModelSeasonEvent`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `ClimateModelSeasonEvent` (
  `ClimateModelId` bigint NOT NULL,
  `SeasonId` bigint NOT NULL,
  `WeatherEventId` bigint NOT NULL,
  `ChangeChance` double NOT NULL,
  `Transitions` mediumtext CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
  PRIMARY KEY (`ClimateModelId`,`SeasonId`,`WeatherEventId`),
  KEY `IX_ClimateModelSeasonEvent_SeasonId` (`SeasonId`),
  KEY `IX_ClimateModelSeasonEvent_WeatherEventId` (`WeatherEventId`),
  CONSTRAINT `FK_ClimateModelSeasonEvent_ClimateModelSeason_ClimateModelId_Se~` FOREIGN KEY (`ClimateModelId`, `SeasonId`) REFERENCES `ClimateModelSeason` (`ClimateModelId`, `SeasonId`) ON DELETE CASCADE,
  CONSTRAINT `FK_ClimateModelSeasonEvents_ClimateModels` FOREIGN KEY (`ClimateModelId`) REFERENCES `ClimateModels` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_ClimateModelSeasonEvents_Seasons` FOREIGN KEY (`SeasonId`) REFERENCES `Seasons` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_ClimateModelSeasonEvents_WeatherEvents` FOREIGN KEY (`WeatherEventId`) REFERENCES `WeatherEvents` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `ClimateModelSeasonEvent`
--

LOCK TABLES `ClimateModelSeasonEvent` WRITE;
/*!40000 ALTER TABLE `ClimateModelSeasonEvent` DISABLE KEYS */;
/*!40000 ALTER TABLE `ClimateModelSeasonEvent` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `Clocks`
--

DROP TABLE IF EXISTS `Clocks`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `Clocks` (
  `Id` bigint NOT NULL AUTO_INCREMENT,
  `Definition` text CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
  `Seconds` int NOT NULL,
  `Minutes` int NOT NULL,
  `Hours` int NOT NULL,
  `PrimaryTimezoneId` bigint NOT NULL,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `Clocks`
--

LOCK TABLES `Clocks` WRITE;
/*!40000 ALTER TABLE `Clocks` DISABLE KEYS */;
/*!40000 ALTER TABLE `Clocks` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `Coins`
--

DROP TABLE IF EXISTS `Coins`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `Coins` (
  `Id` bigint NOT NULL AUTO_INCREMENT,
  `Name` varchar(4000) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
  `ShortDescription` varchar(4000) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
  `FullDescription` varchar(4000) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
  `Value` decimal(58,29) NOT NULL,
  `CurrencyId` bigint NOT NULL,
  `Weight` double NOT NULL,
  `GeneralForm` varchar(4000) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
  `PluralWord` varchar(4000) CHARACTER SET utf8 COLLATE utf8_general_ci DEFAULT NULL,
  `UseForChange` bit(1) NOT NULL DEFAULT b'1',
  PRIMARY KEY (`Id`),
  KEY `FK_Coins_Currencies` (`CurrencyId`),
  CONSTRAINT `FK_Coins_Currencies` FOREIGN KEY (`CurrencyId`) REFERENCES `Currencies` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `Coins`
--

LOCK TABLES `Coins` WRITE;
/*!40000 ALTER TABLE `Coins` DISABLE KEYS */;
/*!40000 ALTER TABLE `Coins` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `Colours`
--

DROP TABLE IF EXISTS `Colours`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `Colours` (
  `Id` bigint NOT NULL AUTO_INCREMENT,
  `Name` varchar(50) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
  `Basic` int NOT NULL,
  `Red` int NOT NULL,
  `Green` int NOT NULL,
  `Blue` int NOT NULL,
  `Fancy` varchar(4000) CHARACTER SET utf8 COLLATE utf8_general_ci DEFAULT NULL,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `Colours`
--

LOCK TABLES `Colours` WRITE;
/*!40000 ALTER TABLE `Colours` DISABLE KEYS */;
/*!40000 ALTER TABLE `Colours` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `CombatActions`
--

DROP TABLE IF EXISTS `CombatActions`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `CombatActions` (
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
  CONSTRAINT `FK_CombatActions_FutureProgs_UsabilityProgId` FOREIGN KEY (`UsabilityProgId`) REFERENCES `FutureProgs` (`Id`),
  CONSTRAINT `FK_CombatActions_TraitDefinitions_TraitDefinitionId` FOREIGN KEY (`TraitDefinitionId`) REFERENCES `TraitDefinitions` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `CombatActions`
--

LOCK TABLES `CombatActions` WRITE;
/*!40000 ALTER TABLE `CombatActions` DISABLE KEYS */;
/*!40000 ALTER TABLE `CombatActions` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `CombatMessages`
--

DROP TABLE IF EXISTS `CombatMessages`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `CombatMessages` (
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
  CONSTRAINT `FK_CombatMessages_FutureProgs` FOREIGN KEY (`ProgId`) REFERENCES `FutureProgs` (`Id`) ON DELETE SET NULL,
  CONSTRAINT `FK_CombatMessages_FutureProgs_Auxiliary` FOREIGN KEY (`AuxiliaryProgId`) REFERENCES `FutureProgs` (`Id`) ON DELETE SET NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `CombatMessages`
--

LOCK TABLES `CombatMessages` WRITE;
/*!40000 ALTER TABLE `CombatMessages` DISABLE KEYS */;
/*!40000 ALTER TABLE `CombatMessages` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `CombatMessages_CombatActions`
--

DROP TABLE IF EXISTS `CombatMessages_CombatActions`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `CombatMessages_CombatActions` (
  `CombatMessageId` bigint NOT NULL,
  `CombatActionId` bigint NOT NULL,
  PRIMARY KEY (`CombatMessageId`,`CombatActionId`),
  KEY `FK_CombatMessages_CombatActions_WeaponAttacks_idx` (`CombatActionId`),
  CONSTRAINT `FK_CombatMessages_CombatActions_CombatMessages` FOREIGN KEY (`CombatMessageId`) REFERENCES `CombatMessages` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_CombatMessages_CombatActions_WeaponAttacks` FOREIGN KEY (`CombatActionId`) REFERENCES `CombatActions` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `CombatMessages_CombatActions`
--

LOCK TABLES `CombatMessages_CombatActions` WRITE;
/*!40000 ALTER TABLE `CombatMessages_CombatActions` DISABLE KEYS */;
/*!40000 ALTER TABLE `CombatMessages_CombatActions` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `CombatMessages_WeaponAttacks`
--

DROP TABLE IF EXISTS `CombatMessages_WeaponAttacks`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `CombatMessages_WeaponAttacks` (
  `CombatMessageId` bigint NOT NULL,
  `WeaponAttackId` bigint NOT NULL,
  PRIMARY KEY (`CombatMessageId`,`WeaponAttackId`),
  KEY `FK_CombatMessages_WeaponAttacks_WeaponAttacks_idx` (`WeaponAttackId`),
  CONSTRAINT `FK_CombatMessages_WeaponAttacks_CombatMessages` FOREIGN KEY (`CombatMessageId`) REFERENCES `CombatMessages` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_CombatMessages_WeaponAttacks_WeaponAttacks` FOREIGN KEY (`WeaponAttackId`) REFERENCES `WeaponAttacks` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `CombatMessages_WeaponAttacks`
--

LOCK TABLES `CombatMessages_WeaponAttacks` WRITE;
/*!40000 ALTER TABLE `CombatMessages_WeaponAttacks` DISABLE KEYS */;
/*!40000 ALTER TABLE `CombatMessages_WeaponAttacks` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `ConveyancingLocations`
--

DROP TABLE IF EXISTS `ConveyancingLocations`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `ConveyancingLocations` (
  `EconomicZoneId` bigint NOT NULL,
  `CellId` bigint NOT NULL,
  PRIMARY KEY (`EconomicZoneId`,`CellId`),
  KEY `IX_ConveyancingLocations_CellId` (`CellId`),
  CONSTRAINT `FK_ConveyancingLocations_Cells` FOREIGN KEY (`CellId`) REFERENCES `Cells` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_ConveyancingLocations_EconomicZones` FOREIGN KEY (`EconomicZoneId`) REFERENCES `EconomicZones` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `ConveyancingLocations`
--

LOCK TABLES `ConveyancingLocations` WRITE;
/*!40000 ALTER TABLE `ConveyancingLocations` DISABLE KEYS */;
/*!40000 ALTER TABLE `ConveyancingLocations` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `CorpseModels`
--

DROP TABLE IF EXISTS `CorpseModels`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `CorpseModels` (
  `Id` bigint NOT NULL AUTO_INCREMENT,
  `Name` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `Description` text CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci,
  `Definition` text CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci,
  `Type` varchar(45) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `CorpseModels`
--

LOCK TABLES `CorpseModels` WRITE;
/*!40000 ALTER TABLE `CorpseModels` DISABLE KEYS */;
/*!40000 ALTER TABLE `CorpseModels` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `CraftInputs`
--

DROP TABLE IF EXISTS `CraftInputs`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `CraftInputs` (
  `Id` bigint NOT NULL AUTO_INCREMENT,
  `CraftId` bigint NOT NULL,
  `CraftRevisionNumber` int NOT NULL,
  `InputType` varchar(45) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
  `InputQualityWeight` double NOT NULL,
  `Definition` text CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
  `OriginalAdditionTime` datetime NOT NULL,
  PRIMARY KEY (`Id`),
  KEY `FK_CraftInputs_Crafts_idx` (`CraftId`,`CraftRevisionNumber`),
  CONSTRAINT `FK_CraftInputs_Crafts` FOREIGN KEY (`CraftId`, `CraftRevisionNumber`) REFERENCES `Crafts` (`Id`, `RevisionNumber`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `CraftInputs`
--

LOCK TABLES `CraftInputs` WRITE;
/*!40000 ALTER TABLE `CraftInputs` DISABLE KEYS */;
/*!40000 ALTER TABLE `CraftInputs` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `CraftPhases`
--

DROP TABLE IF EXISTS `CraftPhases`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `CraftPhases` (
  `CraftPhaseId` bigint NOT NULL,
  `CraftPhaseRevisionNumber` int NOT NULL,
  `PhaseNumber` int NOT NULL,
  `PhaseLengthInSeconds` double NOT NULL,
  `Echo` text CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
  `FailEcho` text CHARACTER SET utf8 COLLATE utf8_general_ci,
  PRIMARY KEY (`CraftPhaseId`,`CraftPhaseRevisionNumber`,`PhaseNumber`),
  CONSTRAINT `FK_CraftPhases_Crafts` FOREIGN KEY (`CraftPhaseId`, `CraftPhaseRevisionNumber`) REFERENCES `Crafts` (`Id`, `RevisionNumber`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `CraftPhases`
--

LOCK TABLES `CraftPhases` WRITE;
/*!40000 ALTER TABLE `CraftPhases` DISABLE KEYS */;
/*!40000 ALTER TABLE `CraftPhases` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `CraftProducts`
--

DROP TABLE IF EXISTS `CraftProducts`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `CraftProducts` (
  `Id` bigint NOT NULL AUTO_INCREMENT,
  `CraftId` bigint NOT NULL,
  `CraftRevisionNumber` int NOT NULL,
  `ProductType` varchar(45) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
  `Definition` text CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
  `OriginalAdditionTime` datetime NOT NULL,
  `IsFailProduct` bit(1) NOT NULL DEFAULT b'0',
  `MaterialDefiningInputIndex` int DEFAULT NULL,
  PRIMARY KEY (`Id`),
  KEY `FK_CraftProducts_Crafts_idx` (`CraftId`,`CraftRevisionNumber`),
  CONSTRAINT `FK_CraftProducts_Crafts` FOREIGN KEY (`CraftId`, `CraftRevisionNumber`) REFERENCES `Crafts` (`Id`, `RevisionNumber`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `CraftProducts`
--

LOCK TABLES `CraftProducts` WRITE;
/*!40000 ALTER TABLE `CraftProducts` DISABLE KEYS */;
/*!40000 ALTER TABLE `CraftProducts` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `Crafts`
--

DROP TABLE IF EXISTS `Crafts`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `Crafts` (
  `Id` bigint NOT NULL,
  `RevisionNumber` int NOT NULL,
  `EditableItemId` bigint NOT NULL,
  `Name` varchar(200) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
  `Blurb` text CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
  `ActionDescription` varchar(200) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
  `Category` varchar(200) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
  `Interruptable` bit(1) NOT NULL,
  `ToolQualityWeighting` double NOT NULL,
  `InputQualityWeighting` double NOT NULL,
  `CheckQualityWeighting` double NOT NULL,
  `FreeSkillChecks` int NOT NULL,
  `FailThreshold` int NOT NULL,
  `CheckTraitId` bigint DEFAULT NULL,
  `CheckDifficulty` int NOT NULL,
  `FailPhase` int NOT NULL,
  `QualityFormula` text CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
  `AppearInCraftsListProgId` bigint DEFAULT NULL,
  `CanUseProgId` bigint DEFAULT NULL,
  `WhyCannotUseProgId` bigint DEFAULT NULL,
  `OnUseProgStartId` bigint DEFAULT NULL,
  `OnUseProgCompleteId` bigint DEFAULT NULL,
  `OnUseProgCancelId` bigint DEFAULT NULL,
  `ActiveCraftItemSDesc` varchar(200) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL DEFAULT 'a craft in progress',
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
  CONSTRAINT `FK_Crafts_EditableItems` FOREIGN KEY (`EditableItemId`) REFERENCES `EditableItems` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_Crafts_FutureProgs_AppearInCraftsListProg` FOREIGN KEY (`AppearInCraftsListProgId`) REFERENCES `FutureProgs` (`Id`) ON DELETE RESTRICT,
  CONSTRAINT `FK_Crafts_FutureProgs_CanUseProg` FOREIGN KEY (`CanUseProgId`) REFERENCES `FutureProgs` (`Id`) ON DELETE SET NULL,
  CONSTRAINT `FK_Crafts_FutureProgs_OnUseProgCancel` FOREIGN KEY (`OnUseProgCancelId`) REFERENCES `FutureProgs` (`Id`) ON DELETE SET NULL,
  CONSTRAINT `FK_Crafts_FutureProgs_OnUseProgComplete` FOREIGN KEY (`OnUseProgCompleteId`) REFERENCES `FutureProgs` (`Id`) ON DELETE SET NULL,
  CONSTRAINT `FK_Crafts_FutureProgs_OnUseProgStart` FOREIGN KEY (`OnUseProgStartId`) REFERENCES `FutureProgs` (`Id`) ON DELETE SET NULL,
  CONSTRAINT `FK_Crafts_FutureProgs_WhyCannotUseProg` FOREIGN KEY (`WhyCannotUseProgId`) REFERENCES `FutureProgs` (`Id`) ON DELETE SET NULL,
  CONSTRAINT `FK_Crafts_TraitDefinitions` FOREIGN KEY (`CheckTraitId`) REFERENCES `TraitDefinitions` (`Id`) ON DELETE RESTRICT
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `Crafts`
--

LOCK TABLES `Crafts` WRITE;
/*!40000 ALTER TABLE `Crafts` DISABLE KEYS */;
/*!40000 ALTER TABLE `Crafts` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `CraftTools`
--

DROP TABLE IF EXISTS `CraftTools`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `CraftTools` (
  `Id` bigint NOT NULL AUTO_INCREMENT,
  `CraftId` bigint NOT NULL,
  `CraftRevisionNumber` int NOT NULL,
  `OriginalAdditionTime` datetime NOT NULL,
  `ToolType` varchar(45) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
  `ToolQualityWeight` double NOT NULL,
  `DesiredState` int NOT NULL,
  `Definition` text CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
  `UseToolDuration` bit(1) NOT NULL DEFAULT b'1',
  PRIMARY KEY (`Id`),
  KEY `FK_CraftTools_Crafts_idx` (`CraftId`,`CraftRevisionNumber`),
  CONSTRAINT `FK_CraftTools_Crafts` FOREIGN KEY (`CraftId`, `CraftRevisionNumber`) REFERENCES `Crafts` (`Id`, `RevisionNumber`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `CraftTools`
--

LOCK TABLES `CraftTools` WRITE;
/*!40000 ALTER TABLE `CraftTools` DISABLE KEYS */;
/*!40000 ALTER TABLE `CraftTools` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `Crimes`
--

DROP TABLE IF EXISTS `Crimes`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `Crimes` (
  `Id` bigint NOT NULL AUTO_INCREMENT,
  `LawId` bigint NOT NULL,
  `CriminalId` bigint NOT NULL,
  `VictimId` bigint DEFAULT NULL,
  `TimeOfCrime` varchar(200) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
  `RealTimeOfCrime` datetime NOT NULL,
  `LocationId` bigint DEFAULT NULL,
  `TimeOfReport` varchar(200) CHARACTER SET utf8 COLLATE utf8_general_ci DEFAULT NULL,
  `AccuserId` bigint DEFAULT NULL,
  `CriminalShortDescription` varchar(500) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
  `CriminalFullDescription` text CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
  `CriminalCharacteristics` text CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
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
  CONSTRAINT `FK_Crimes_Accuser` FOREIGN KEY (`AccuserId`) REFERENCES `Characters` (`Id`) ON DELETE SET NULL,
  CONSTRAINT `FK_Crimes_Criminal` FOREIGN KEY (`CriminalId`) REFERENCES `Characters` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_Crimes_Laws` FOREIGN KEY (`LawId`) REFERENCES `Laws` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_Crimes_Location` FOREIGN KEY (`LocationId`) REFERENCES `Cells` (`Id`) ON DELETE SET NULL,
  CONSTRAINT `FK_Crimes_Victim` FOREIGN KEY (`VictimId`) REFERENCES `Characters` (`Id`) ON DELETE SET NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `Crimes`
--

LOCK TABLES `Crimes` WRITE;
/*!40000 ALTER TABLE `Crimes` DISABLE KEYS */;
/*!40000 ALTER TABLE `Crimes` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `CultureInfos`
--

DROP TABLE IF EXISTS `CultureInfos`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `CultureInfos` (
  `Id` varchar(50) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
  `DisplayName` varchar(4000) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
  `Order` int NOT NULL,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `CultureInfos`
--

LOCK TABLES `CultureInfos` WRITE;
/*!40000 ALTER TABLE `CultureInfos` DISABLE KEYS */;
/*!40000 ALTER TABLE `CultureInfos` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `Cultures`
--

DROP TABLE IF EXISTS `Cultures`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `Cultures` (
  `Id` bigint NOT NULL AUTO_INCREMENT,
  `Name` varchar(255) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
  `Description` varchar(4000) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
  `PersonWordMale` varchar(255) CHARACTER SET utf8 COLLATE utf8_general_ci DEFAULT NULL,
  `PersonWordFemale` varchar(255) CHARACTER SET utf8 COLLATE utf8_general_ci DEFAULT NULL,
  `PersonWordNeuter` varchar(255) CHARACTER SET utf8 COLLATE utf8_general_ci DEFAULT NULL,
  `PersonWordIndeterminate` varchar(255) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
  `PrimaryCalendarId` bigint NOT NULL,
  `SkillStartingValueProgId` bigint NOT NULL,
  `AvailabilityProgId` bigint DEFAULT NULL,
  `TolerableTemperatureFloorEffect` double NOT NULL,
  `TolerableTemperatureCeilingEffect` double NOT NULL,
  PRIMARY KEY (`Id`),
  KEY `FK_Cultures_AvailabilityProg` (`AvailabilityProgId`),
  KEY `FK_Cultures_SkillStartingProg` (`SkillStartingValueProgId`),
  CONSTRAINT `FK_Cultures_AvailabilityProg` FOREIGN KEY (`AvailabilityProgId`) REFERENCES `FutureProgs` (`Id`) ON DELETE SET NULL,
  CONSTRAINT `FK_Cultures_SkillStartingProg` FOREIGN KEY (`SkillStartingValueProgId`) REFERENCES `FutureProgs` (`Id`) ON DELETE RESTRICT
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `Cultures`
--

LOCK TABLES `Cultures` WRITE;
/*!40000 ALTER TABLE `Cultures` DISABLE KEYS */;
/*!40000 ALTER TABLE `Cultures` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `Cultures_ChargenResources`
--

DROP TABLE IF EXISTS `Cultures_ChargenResources`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `Cultures_ChargenResources` (
  `CultureId` bigint NOT NULL,
  `ChargenResourceId` bigint NOT NULL,
  `RequirementOnly` bit(1) NOT NULL DEFAULT b'0',
  `Amount` int NOT NULL,
  PRIMARY KEY (`CultureId`,`ChargenResourceId`),
  KEY `IX_Cultures_ChargenResources_ChargenResourceId` (`ChargenResourceId`),
  CONSTRAINT `FK_Cultures_ChargenResources_ChargenResources_ChargenResourceId` FOREIGN KEY (`ChargenResourceId`) REFERENCES `ChargenResources` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_Cultures_ChargenResources_Cultures_CultureId` FOREIGN KEY (`CultureId`) REFERENCES `Cultures` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `Cultures_ChargenResources`
--

LOCK TABLES `Cultures_ChargenResources` WRITE;
/*!40000 ALTER TABLE `Cultures_ChargenResources` DISABLE KEYS */;
/*!40000 ALTER TABLE `Cultures_ChargenResources` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `CulturesNameCultures`
--

DROP TABLE IF EXISTS `CulturesNameCultures`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `CulturesNameCultures` (
  `CultureId` bigint NOT NULL,
  `NameCultureId` bigint NOT NULL,
  `Gender` smallint NOT NULL,
  PRIMARY KEY (`CultureId`,`NameCultureId`,`Gender`),
  KEY `IX_CulturesNameCultures_NameCultureId` (`NameCultureId`),
  CONSTRAINT `FK_CulturesNameCultures_Cultures` FOREIGN KEY (`CultureId`) REFERENCES `Cultures` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_CulturesNameCultures_NameCultures` FOREIGN KEY (`NameCultureId`) REFERENCES `NameCulture` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `CulturesNameCultures`
--

LOCK TABLES `CulturesNameCultures` WRITE;
/*!40000 ALTER TABLE `CulturesNameCultures` DISABLE KEYS */;
/*!40000 ALTER TABLE `CulturesNameCultures` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `Currencies`
--

DROP TABLE IF EXISTS `Currencies`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `Currencies` (
  `Id` bigint NOT NULL AUTO_INCREMENT,
  `Name` varchar(4000) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
  `BaseCurrencyToGlobalBaseCurrencyConversion` decimal(58,29) NOT NULL DEFAULT '1.00000000000000000000000000000',
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `Currencies`
--

LOCK TABLES `Currencies` WRITE;
/*!40000 ALTER TABLE `Currencies` DISABLE KEYS */;
/*!40000 ALTER TABLE `Currencies` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `CurrencyDescriptionPatternElements`
--

DROP TABLE IF EXISTS `CurrencyDescriptionPatternElements`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `CurrencyDescriptionPatternElements` (
  `Id` bigint NOT NULL AUTO_INCREMENT,
  `Pattern` varchar(4000) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
  `Order` int NOT NULL,
  `ShowIfZero` bit(1) NOT NULL,
  `CurrencyDivisionId` bigint NOT NULL,
  `CurrencyDescriptionPatternId` bigint NOT NULL,
  `PluraliseWord` varchar(4000) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
  `AlternatePattern` varchar(4000) CHARACTER SET utf8 COLLATE utf8_general_ci DEFAULT NULL,
  `RoundingMode` int NOT NULL,
  `SpecialValuesOverrideFormat` bit(1) NOT NULL DEFAULT b'0',
  PRIMARY KEY (`Id`),
  KEY `FK_CDPE_CurrencyDescriptionPatterns` (`CurrencyDescriptionPatternId`),
  KEY `FK_CDPE_CurrencyDivisions` (`CurrencyDivisionId`),
  CONSTRAINT `FK_CDPE_CurrencyDescriptionPatterns` FOREIGN KEY (`CurrencyDescriptionPatternId`) REFERENCES `CurrencyDescriptionPatterns` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_CDPE_CurrencyDivisions` FOREIGN KEY (`CurrencyDivisionId`) REFERENCES `CurrencyDivisions` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `CurrencyDescriptionPatternElements`
--

LOCK TABLES `CurrencyDescriptionPatternElements` WRITE;
/*!40000 ALTER TABLE `CurrencyDescriptionPatternElements` DISABLE KEYS */;
/*!40000 ALTER TABLE `CurrencyDescriptionPatternElements` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `CurrencyDescriptionPatternElementSpecialValues`
--

DROP TABLE IF EXISTS `CurrencyDescriptionPatternElementSpecialValues`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `CurrencyDescriptionPatternElementSpecialValues` (
  `Value` decimal(58,29) NOT NULL,
  `CurrencyDescriptionPatternElementId` bigint NOT NULL,
  `Text` varchar(4000) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
  PRIMARY KEY (`Value`,`CurrencyDescriptionPatternElementId`),
  KEY `FK_CDPESV_CDPE` (`CurrencyDescriptionPatternElementId`),
  CONSTRAINT `FK_CDPESV_CDPE` FOREIGN KEY (`CurrencyDescriptionPatternElementId`) REFERENCES `CurrencyDescriptionPatternElements` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `CurrencyDescriptionPatternElementSpecialValues`
--

LOCK TABLES `CurrencyDescriptionPatternElementSpecialValues` WRITE;
/*!40000 ALTER TABLE `CurrencyDescriptionPatternElementSpecialValues` DISABLE KEYS */;
/*!40000 ALTER TABLE `CurrencyDescriptionPatternElementSpecialValues` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `CurrencyDescriptionPatterns`
--

DROP TABLE IF EXISTS `CurrencyDescriptionPatterns`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `CurrencyDescriptionPatterns` (
  `Id` bigint NOT NULL AUTO_INCREMENT,
  `Type` int NOT NULL,
  `CurrencyId` bigint NOT NULL,
  `FutureProgId` bigint DEFAULT NULL,
  `NegativePrefix` varchar(4000) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
  `Order` int NOT NULL,
  `UseNaturalAggregationStyle` bit(1) NOT NULL DEFAULT b'0',
  PRIMARY KEY (`Id`),
  KEY `FK_CurrencyDescriptionPatterns_Currencies` (`CurrencyId`),
  KEY `FK_CurrencyDescriptionPatterns_FutureProgs` (`FutureProgId`),
  CONSTRAINT `FK_CurrencyDescriptionPatterns_Currencies` FOREIGN KEY (`CurrencyId`) REFERENCES `Currencies` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_CurrencyDescriptionPatterns_FutureProgs` FOREIGN KEY (`FutureProgId`) REFERENCES `FutureProgs` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `CurrencyDescriptionPatterns`
--

LOCK TABLES `CurrencyDescriptionPatterns` WRITE;
/*!40000 ALTER TABLE `CurrencyDescriptionPatterns` DISABLE KEYS */;
/*!40000 ALTER TABLE `CurrencyDescriptionPatterns` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `CurrencyDivisionAbbreviations`
--

DROP TABLE IF EXISTS `CurrencyDivisionAbbreviations`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `CurrencyDivisionAbbreviations` (
  `Pattern` varchar(150) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
  `CurrencyDivisionId` bigint NOT NULL,
  PRIMARY KEY (`Pattern`,`CurrencyDivisionId`),
  KEY `FK_CurrencyDivisionAbbreviations_CurrencyDivisions` (`CurrencyDivisionId`),
  CONSTRAINT `FK_CurrencyDivisionAbbreviations_CurrencyDivisions` FOREIGN KEY (`CurrencyDivisionId`) REFERENCES `CurrencyDivisions` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `CurrencyDivisionAbbreviations`
--

LOCK TABLES `CurrencyDivisionAbbreviations` WRITE;
/*!40000 ALTER TABLE `CurrencyDivisionAbbreviations` DISABLE KEYS */;
/*!40000 ALTER TABLE `CurrencyDivisionAbbreviations` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `CurrencyDivisions`
--

DROP TABLE IF EXISTS `CurrencyDivisions`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `CurrencyDivisions` (
  `Id` bigint NOT NULL AUTO_INCREMENT,
  `Name` varchar(4000) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
  `BaseUnitConversionRate` decimal(58,29) NOT NULL,
  `CurrencyId` bigint NOT NULL,
  `IgnoreCase` bit(1) NOT NULL DEFAULT b'1',
  PRIMARY KEY (`Id`),
  KEY `FK_CurrencyDivisions_Currencies` (`CurrencyId`),
  CONSTRAINT `FK_CurrencyDivisions_Currencies` FOREIGN KEY (`CurrencyId`) REFERENCES `Currencies` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `CurrencyDivisions`
--

LOCK TABLES `CurrencyDivisions` WRITE;
/*!40000 ALTER TABLE `CurrencyDivisions` DISABLE KEYS */;
/*!40000 ALTER TABLE `CurrencyDivisions` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `DamagePatterns`
--

DROP TABLE IF EXISTS `DamagePatterns`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `DamagePatterns` (
  `Id` bigint NOT NULL AUTO_INCREMENT,
  `Name` varchar(4000) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
  `DamageType` int NOT NULL,
  `Dice` int NOT NULL,
  `Sides` int NOT NULL,
  `Bonus` int NOT NULL,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `DamagePatterns`
--

LOCK TABLES `DamagePatterns` WRITE;
/*!40000 ALTER TABLE `DamagePatterns` DISABLE KEYS */;
/*!40000 ALTER TABLE `DamagePatterns` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `DefaultHooks`
--

DROP TABLE IF EXISTS `DefaultHooks`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `DefaultHooks` (
  `HookId` bigint NOT NULL,
  `PerceivableType` varchar(45) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL,
  `FutureProgId` bigint NOT NULL,
  PRIMARY KEY (`HookId`,`PerceivableType`,`FutureProgId`),
  KEY `FK_DefaultHooks_Futureprogs_idx` (`FutureProgId`),
  CONSTRAINT `FK_DefaultHooks_Futureprogs` FOREIGN KEY (`FutureProgId`) REFERENCES `FutureProgs` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_DefaultHooks_Hooks` FOREIGN KEY (`HookId`) REFERENCES `Hooks` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `DefaultHooks`
--

LOCK TABLES `DefaultHooks` WRITE;
/*!40000 ALTER TABLE `DefaultHooks` DISABLE KEYS */;
/*!40000 ALTER TABLE `DefaultHooks` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `DisfigurementTemplates`
--

DROP TABLE IF EXISTS `DisfigurementTemplates`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `DisfigurementTemplates` (
  `Id` bigint NOT NULL,
  `RevisionNumber` int NOT NULL,
  `Name` varchar(500) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
  `Type` varchar(50) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
  `EditableItemId` bigint NOT NULL,
  `ShortDescription` varchar(500) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
  `FullDescription` varchar(5000) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
  `Definition` text CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
  PRIMARY KEY (`Id`,`RevisionNumber`),
  KEY `FK_DisfigurementTemplates_EditableItems_idx` (`EditableItemId`),
  CONSTRAINT `FK_DisfigurementTemplates_EditableItems` FOREIGN KEY (`EditableItemId`) REFERENCES `EditableItems` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `DisfigurementTemplates`
--

LOCK TABLES `DisfigurementTemplates` WRITE;
/*!40000 ALTER TABLE `DisfigurementTemplates` DISABLE KEYS */;
/*!40000 ALTER TABLE `DisfigurementTemplates` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `Doors`
--

DROP TABLE IF EXISTS `Doors`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `Doors` (
  `Id` bigint NOT NULL AUTO_INCREMENT,
  `Name` varchar(4000) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
  `Style` int NOT NULL,
  `IsOpen` bit(1) NOT NULL,
  `LockedWith` bigint DEFAULT NULL,
  PRIMARY KEY (`Id`),
  KEY `FK_Doors_Locks` (`LockedWith`),
  CONSTRAINT `FK_Doors_Locks` FOREIGN KEY (`LockedWith`) REFERENCES `Locks` (`Id`) ON DELETE RESTRICT
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `Doors`
--

LOCK TABLES `Doors` WRITE;
/*!40000 ALTER TABLE `Doors` DISABLE KEYS */;
/*!40000 ALTER TABLE `Doors` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `Drawings`
--

DROP TABLE IF EXISTS `Drawings`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `Drawings` (
  `Id` bigint NOT NULL AUTO_INCREMENT,
  `AuthorId` bigint NOT NULL,
  `ShortDescription` text CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
  `FullDescription` text CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
  `ImplementType` int NOT NULL,
  `DrawingSkill` double NOT NULL,
  `DrawingSize` int NOT NULL,
  PRIMARY KEY (`Id`),
  KEY `FK_Drawings_Characters_idx` (`AuthorId`),
  CONSTRAINT `FK_Drawings_Characters` FOREIGN KEY (`AuthorId`) REFERENCES `Characters` (`Id`) ON DELETE RESTRICT
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `Drawings`
--

LOCK TABLES `Drawings` WRITE;
/*!40000 ALTER TABLE `Drawings` DISABLE KEYS */;
/*!40000 ALTER TABLE `Drawings` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `Dream_Phases`
--

DROP TABLE IF EXISTS `Dream_Phases`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `Dream_Phases` (
  `DreamId` bigint NOT NULL,
  `PhaseId` int NOT NULL,
  `DreamerText` text CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL,
  `DreamerCommand` text CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL,
  `WaitSeconds` int NOT NULL DEFAULT '30',
  PRIMARY KEY (`DreamId`,`PhaseId`),
  CONSTRAINT `FK_Dream_Phases_Dreams` FOREIGN KEY (`DreamId`) REFERENCES `Dreams` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `Dream_Phases`
--

LOCK TABLES `Dream_Phases` WRITE;
/*!40000 ALTER TABLE `Dream_Phases` DISABLE KEYS */;
/*!40000 ALTER TABLE `Dream_Phases` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `Dreams`
--

DROP TABLE IF EXISTS `Dreams`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `Dreams` (
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
  CONSTRAINT `FK_Dreams_FutureProgs_CanDream` FOREIGN KEY (`CanDreamProgId`) REFERENCES `FutureProgs` (`Id`) ON DELETE SET NULL,
  CONSTRAINT `FK_Dreams_FutureProgs_OnDream` FOREIGN KEY (`OnDreamProgId`) REFERENCES `FutureProgs` (`Id`) ON DELETE SET NULL,
  CONSTRAINT `FK_Dreams_FutureProgs_OnWake` FOREIGN KEY (`OnWakeDuringDreamingProgId`) REFERENCES `FutureProgs` (`Id`) ON DELETE SET NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `Dreams`
--

LOCK TABLES `Dreams` WRITE;
/*!40000 ALTER TABLE `Dreams` DISABLE KEYS */;
/*!40000 ALTER TABLE `Dreams` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `Dreams_Already_Dreamt`
--

DROP TABLE IF EXISTS `Dreams_Already_Dreamt`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `Dreams_Already_Dreamt` (
  `DreamId` bigint NOT NULL,
  `CharacterId` bigint NOT NULL,
  PRIMARY KEY (`DreamId`,`CharacterId`),
  KEY `FK_Dreams_Dreamt_Characters_idx` (`CharacterId`),
  CONSTRAINT `FK_Dreams_Dreamt_Characters` FOREIGN KEY (`CharacterId`) REFERENCES `Characters` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_Dreams_Dreamt_Dreams` FOREIGN KEY (`DreamId`) REFERENCES `Dreams` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `Dreams_Already_Dreamt`
--

LOCK TABLES `Dreams_Already_Dreamt` WRITE;
/*!40000 ALTER TABLE `Dreams_Already_Dreamt` DISABLE KEYS */;
/*!40000 ALTER TABLE `Dreams_Already_Dreamt` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `Dreams_Characters`
--

DROP TABLE IF EXISTS `Dreams_Characters`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `Dreams_Characters` (
  `DreamId` bigint NOT NULL,
  `CharacterId` bigint NOT NULL,
  PRIMARY KEY (`DreamId`,`CharacterId`),
  KEY `FK_Dreams_Characters_Characters_idx` (`CharacterId`),
  CONSTRAINT `FK_Dreams_Characters_Characters` FOREIGN KEY (`CharacterId`) REFERENCES `Characters` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_Dreams_Characters_Dreams` FOREIGN KEY (`DreamId`) REFERENCES `Dreams` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `Dreams_Characters`
--

LOCK TABLES `Dreams_Characters` WRITE;
/*!40000 ALTER TABLE `Dreams_Characters` DISABLE KEYS */;
/*!40000 ALTER TABLE `Dreams_Characters` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `Drugs`
--

DROP TABLE IF EXISTS `Drugs`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `Drugs` (
  `Id` bigint NOT NULL AUTO_INCREMENT,
  `Name` varchar(200) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL,
  `DrugVectors` int NOT NULL,
  `IntensityPerGram` double NOT NULL,
  `RelativeMetabolisationRate` double NOT NULL,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `Drugs`
--

LOCK TABLES `Drugs` WRITE;
/*!40000 ALTER TABLE `Drugs` DISABLE KEYS */;
/*!40000 ALTER TABLE `Drugs` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `DrugsIntensities`
--

DROP TABLE IF EXISTS `DrugsIntensities`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `DrugsIntensities` (
  `DrugId` bigint NOT NULL AUTO_INCREMENT,
  `DrugType` int NOT NULL,
  `RelativeIntensity` double NOT NULL,
  `AdditionalEffects` text CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci,
  PRIMARY KEY (`DrugId`,`DrugType`),
  CONSTRAINT `FK_Drugs_DrugIntensities` FOREIGN KEY (`DrugId`) REFERENCES `Drugs` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `DrugsIntensities`
--

LOCK TABLES `DrugsIntensities` WRITE;
/*!40000 ALTER TABLE `DrugsIntensities` DISABLE KEYS */;
/*!40000 ALTER TABLE `DrugsIntensities` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `Dubs`
--

DROP TABLE IF EXISTS `Dubs`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `Dubs` (
  `Id` bigint NOT NULL AUTO_INCREMENT,
  `Keywords` varchar(4000) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
  `TargetId` bigint NOT NULL,
  `TargetType` varchar(100) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
  `LastDescription` varchar(4000) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
  `LastUsage` datetime NOT NULL,
  `CharacterId` bigint NOT NULL,
  `IntroducedName` varchar(4000) CHARACTER SET utf8 COLLATE utf8_general_ci DEFAULT NULL,
  PRIMARY KEY (`Id`),
  KEY `FK_Dubs_Characters` (`CharacterId`),
  CONSTRAINT `FK_Dubs_Characters` FOREIGN KEY (`CharacterId`) REFERENCES `Characters` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `Dubs`
--

LOCK TABLES `Dubs` WRITE;
/*!40000 ALTER TABLE `Dubs` DISABLE KEYS */;
/*!40000 ALTER TABLE `Dubs` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `EconomicZoneRevenues`
--

DROP TABLE IF EXISTS `EconomicZoneRevenues`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `EconomicZoneRevenues` (
  `EconomicZoneId` bigint NOT NULL,
  `FinancialPeriodId` bigint NOT NULL,
  `TotalTaxRevenue` decimal(58,29) NOT NULL,
  PRIMARY KEY (`EconomicZoneId`,`FinancialPeriodId`),
  KEY `FK_EconomicZoneRevenues_FinancialPeriods_idx` (`FinancialPeriodId`),
  CONSTRAINT `FK_EconomicZoneRevenues` FOREIGN KEY (`EconomicZoneId`) REFERENCES `EconomicZones` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_EconomicZoneRevenues_FinancialPeriods` FOREIGN KEY (`FinancialPeriodId`) REFERENCES `FinancialPeriods` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `EconomicZoneRevenues`
--

LOCK TABLES `EconomicZoneRevenues` WRITE;
/*!40000 ALTER TABLE `EconomicZoneRevenues` DISABLE KEYS */;
/*!40000 ALTER TABLE `EconomicZoneRevenues` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `EconomicZones`
--

DROP TABLE IF EXISTS `EconomicZones`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `EconomicZones` (
  `Id` bigint NOT NULL AUTO_INCREMENT,
  `Name` varchar(200) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
  `PreviousFinancialPeriodsToKeep` int NOT NULL DEFAULT '50',
  `ZoneForTimePurposesId` bigint NOT NULL,
  `PermitTaxableLosses` bit(1) NOT NULL DEFAULT b'1',
  `OutstandingTaxesOwed` decimal(58,29) NOT NULL,
  `CurrencyId` bigint NOT NULL,
  `CurrentFinancialPeriodId` bigint DEFAULT NULL,
  `ReferenceCalendarId` bigint DEFAULT NULL,
  `ReferenceClockId` bigint NOT NULL,
  `ReferenceTime` varchar(100) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
  `IntervalType` int NOT NULL DEFAULT '2',
  `IntervalModifier` int NOT NULL,
  `IntervalAmount` int NOT NULL DEFAULT '1',
  `TotalRevenueHeld` decimal(58,29) NOT NULL DEFAULT '0.00000000000000000000000000000',
  `ControllingClanId` bigint DEFAULT NULL,
  PRIMARY KEY (`Id`),
  KEY `FK_EconomicZones_Currencies_idx` (`CurrencyId`),
  KEY `FK_EconomicZones_FinancialPeriods_idx` (`CurrentFinancialPeriodId`),
  KEY `FK_EconomicZones_Calendars_idx` (`ReferenceCalendarId`),
  KEY `FK_EconomicZones_Timezones_idx` (`ReferenceClockId`),
  KEY `FK_EconomicZones_ControllingClans_idx` (`ControllingClanId`),
  CONSTRAINT `FK_EconomicZones_Calendars` FOREIGN KEY (`ReferenceCalendarId`) REFERENCES `Calendars` (`Id`) ON DELETE SET NULL,
  CONSTRAINT `FK_EconomicZones_Clocks` FOREIGN KEY (`ReferenceClockId`) REFERENCES `Clocks` (`Id`) ON DELETE RESTRICT,
  CONSTRAINT `FK_EconomicZones_ControllingClans` FOREIGN KEY (`ControllingClanId`) REFERENCES `Clans` (`Id`) ON DELETE RESTRICT,
  CONSTRAINT `FK_EconomicZones_Currencies` FOREIGN KEY (`CurrencyId`) REFERENCES `Currencies` (`Id`) ON DELETE RESTRICT,
  CONSTRAINT `FK_EconomicZones_FinancialPeriods` FOREIGN KEY (`CurrentFinancialPeriodId`) REFERENCES `FinancialPeriods` (`Id`) ON DELETE SET NULL,
  CONSTRAINT `FK_EconomicZones_Timezones` FOREIGN KEY (`ReferenceClockId`) REFERENCES `Timezones` (`Id`) ON DELETE RESTRICT
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `EconomicZones`
--

LOCK TABLES `EconomicZones` WRITE;
/*!40000 ALTER TABLE `EconomicZones` DISABLE KEYS */;
/*!40000 ALTER TABLE `EconomicZones` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `EconomicZoneShopTaxes`
--

DROP TABLE IF EXISTS `EconomicZoneShopTaxes`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `EconomicZoneShopTaxes` (
  `EconomicZoneId` bigint NOT NULL,
  `ShopId` bigint NOT NULL,
  `OutstandingProfitTaxes` decimal(58,29) NOT NULL,
  `OutstandingSalesTaxes` decimal(58,29) NOT NULL,
  `TaxesInCredits` decimal(58,29) NOT NULL,
  PRIMARY KEY (`EconomicZoneId`,`ShopId`),
  KEY `FK_EconomicZoneShopTaxes_Shops_idx` (`ShopId`),
  CONSTRAINT `FK_EconomicZoneShopTaxes_EconomicZones` FOREIGN KEY (`EconomicZoneId`) REFERENCES `EconomicZones` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_EconomicZoneShopTaxes_Shops` FOREIGN KEY (`ShopId`) REFERENCES `Shops` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `EconomicZoneShopTaxes`
--

LOCK TABLES `EconomicZoneShopTaxes` WRITE;
/*!40000 ALTER TABLE `EconomicZoneShopTaxes` DISABLE KEYS */;
/*!40000 ALTER TABLE `EconomicZoneShopTaxes` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `EconomicZoneTaxes`
--

DROP TABLE IF EXISTS `EconomicZoneTaxes`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `EconomicZoneTaxes` (
  `Id` bigint NOT NULL AUTO_INCREMENT,
  `EconomicZoneId` bigint NOT NULL,
  `Name` varchar(200) DEFAULT NULL,
  `MerchantDescription` varchar(200) DEFAULT NULL,
  `MerchandiseFilterProgId` bigint DEFAULT NULL,
  `TaxType` varchar(50) DEFAULT NULL,
  `Definition` text CHARACTER SET utf8 COLLATE utf8_general_ci,
  PRIMARY KEY (`Id`),
  KEY `FK_EconomicZoneTaxes_EconomicZones_idx` (`EconomicZoneId`),
  KEY `FK_EconomicZoneTaxes_FutureProgs_idx` (`MerchandiseFilterProgId`),
  CONSTRAINT `FK_EconomicZoneTaxes_EconomicZones` FOREIGN KEY (`EconomicZoneId`) REFERENCES `EconomicZones` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_EconomicZoneTaxes_FutureProgs` FOREIGN KEY (`MerchandiseFilterProgId`) REFERENCES `FutureProgs` (`Id`) ON DELETE RESTRICT
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `EconomicZoneTaxes`
--

LOCK TABLES `EconomicZoneTaxes` WRITE;
/*!40000 ALTER TABLE `EconomicZoneTaxes` DISABLE KEYS */;
/*!40000 ALTER TABLE `EconomicZoneTaxes` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `EditableItems`
--

DROP TABLE IF EXISTS `EditableItems`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `EditableItems` (
  `Id` bigint NOT NULL AUTO_INCREMENT,
  `RevisionNumber` int NOT NULL,
  `RevisionStatus` int NOT NULL,
  `BuilderAccountId` bigint NOT NULL,
  `ReviewerAccountId` bigint DEFAULT NULL,
  `BuilderComment` varchar(4000) CHARACTER SET utf8 COLLATE utf8_general_ci DEFAULT NULL,
  `ReviewerComment` varchar(4000) CHARACTER SET utf8 COLLATE utf8_general_ci DEFAULT NULL,
  `BuilderDate` datetime NOT NULL,
  `ReviewerDate` datetime DEFAULT NULL,
  `ObsoleteDate` datetime DEFAULT NULL,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `EditableItems`
--

LOCK TABLES `EditableItems` WRITE;
/*!40000 ALTER TABLE `EditableItems` DISABLE KEYS */;
/*!40000 ALTER TABLE `EditableItems` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `Elections`
--

DROP TABLE IF EXISTS `Elections`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `Elections` (
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
  CONSTRAINT `FK_Elections_Appointments` FOREIGN KEY (`AppointmentId`) REFERENCES `Appointments` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `Elections`
--

LOCK TABLES `Elections` WRITE;
/*!40000 ALTER TABLE `Elections` DISABLE KEYS */;
/*!40000 ALTER TABLE `Elections` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `ElectionsNominees`
--

DROP TABLE IF EXISTS `ElectionsNominees`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `ElectionsNominees` (
  `ElectionId` bigint NOT NULL,
  `NomineeId` bigint NOT NULL,
  `NomineeClanId` bigint NOT NULL,
  PRIMARY KEY (`ElectionId`,`NomineeId`),
  KEY `FK_ElectionsNominees_Elections_idx` (`ElectionId`),
  KEY `FK_ElectionsNominees_ClanMemberships_idx` (`NomineeClanId`,`NomineeId`),
  CONSTRAINT `FK_ElectionsNominees_ClanMemberships` FOREIGN KEY (`NomineeClanId`, `NomineeId`) REFERENCES `ClanMemberships` (`ClanId`, `CharacterId`) ON DELETE CASCADE,
  CONSTRAINT `FK_ElectionsNominees_Elections` FOREIGN KEY (`ElectionId`) REFERENCES `Elections` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `ElectionsNominees`
--

LOCK TABLES `ElectionsNominees` WRITE;
/*!40000 ALTER TABLE `ElectionsNominees` DISABLE KEYS */;
/*!40000 ALTER TABLE `ElectionsNominees` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `ElectionsVotes`
--

DROP TABLE IF EXISTS `ElectionsVotes`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `ElectionsVotes` (
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
  CONSTRAINT `FK_ElectionsVotes_Elections` FOREIGN KEY (`ElectionId`) REFERENCES `Elections` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_ElectionsVotes_Nominees` FOREIGN KEY (`NomineeClanId`, `NomineeId`) REFERENCES `ClanMemberships` (`ClanId`, `CharacterId`) ON DELETE CASCADE,
  CONSTRAINT `FK_ElectionsVotes_Voters` FOREIGN KEY (`VoterClanId`, `VoterId`) REFERENCES `ClanMemberships` (`ClanId`, `CharacterId`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `ElectionsVotes`
--

LOCK TABLES `ElectionsVotes` WRITE;
/*!40000 ALTER TABLE `ElectionsVotes` DISABLE KEYS */;
/*!40000 ALTER TABLE `ElectionsVotes` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `EmailTemplates`
--

DROP TABLE IF EXISTS `EmailTemplates`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `EmailTemplates` (
  `TemplateType` int NOT NULL,
  `Content` varchar(4000) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
  `Subject` varchar(4000) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
  `ReturnAddress` varchar(4000) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
  PRIMARY KEY (`TemplateType`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `EmailTemplates`
--

LOCK TABLES `EmailTemplates` WRITE;
/*!40000 ALTER TABLE `EmailTemplates` DISABLE KEYS */;
/*!40000 ALTER TABLE `EmailTemplates` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `EnforcementAuthorities`
--

DROP TABLE IF EXISTS `EnforcementAuthorities`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `EnforcementAuthorities` (
  `Id` bigint NOT NULL AUTO_INCREMENT,
  `Name` varchar(250) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
  `LegalAuthorityId` bigint NOT NULL,
  `Priority` int NOT NULL,
  `CanAccuse` bit(1) NOT NULL,
  `CanForgive` bit(1) NOT NULL,
  `CanConvict` bit(1) NOT NULL,
  `FilterProgId` bigint DEFAULT NULL,
  PRIMARY KEY (`Id`),
  KEY `FK_EnforcementAuthorities_LegalAuthorities_idx` (`LegalAuthorityId`),
  KEY `FK_EnforcementAuthorities_FutureProgs_idx` (`FilterProgId`),
  CONSTRAINT `FK_EnforcementAuthorities_FutureProgs` FOREIGN KEY (`FilterProgId`) REFERENCES `FutureProgs` (`Id`) ON DELETE RESTRICT,
  CONSTRAINT `FK_EnforcementAuthorities_LegalAuthorities` FOREIGN KEY (`LegalAuthorityId`) REFERENCES `LegalAuthorities` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `EnforcementAuthorities`
--

LOCK TABLES `EnforcementAuthorities` WRITE;
/*!40000 ALTER TABLE `EnforcementAuthorities` DISABLE KEYS */;
/*!40000 ALTER TABLE `EnforcementAuthorities` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `EnforcementAuthorities_AccusableClasses`
--

DROP TABLE IF EXISTS `EnforcementAuthorities_AccusableClasses`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `EnforcementAuthorities_AccusableClasses` (
  `EnforcementAuthorityId` bigint NOT NULL,
  `LegalClassId` bigint NOT NULL,
  PRIMARY KEY (`EnforcementAuthorityId`,`LegalClassId`),
  KEY `FK_EnforcementAuthorities_AccusableClasses_LegalClasses_idx` (`LegalClassId`),
  CONSTRAINT `FK_EnforcementAuthorities_AccusableClasses_Enforce` FOREIGN KEY (`EnforcementAuthorityId`) REFERENCES `EnforcementAuthorities` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_EnforcementAuthorities_AccusableClasses_LegalClasses` FOREIGN KEY (`LegalClassId`) REFERENCES `LegalClasses` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `EnforcementAuthorities_AccusableClasses`
--

LOCK TABLES `EnforcementAuthorities_AccusableClasses` WRITE;
/*!40000 ALTER TABLE `EnforcementAuthorities_AccusableClasses` DISABLE KEYS */;
/*!40000 ALTER TABLE `EnforcementAuthorities_AccusableClasses` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `EnforcementAuthorities_ParentAuthorities`
--

DROP TABLE IF EXISTS `EnforcementAuthorities_ParentAuthorities`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `EnforcementAuthorities_ParentAuthorities` (
  `ParentId` bigint NOT NULL,
  `ChildId` bigint NOT NULL,
  PRIMARY KEY (`ParentId`,`ChildId`),
  KEY `FK_EnforcementAuthorities_ParentAuthorities_Child_idx` (`ChildId`),
  CONSTRAINT `FK_EnforcementAuthorities_ParentAuthorities_Child` FOREIGN KEY (`ChildId`) REFERENCES `EnforcementAuthorities` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_EnforcementAuthorities_ParentAuthorities_Parent` FOREIGN KEY (`ParentId`) REFERENCES `EnforcementAuthorities` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `EnforcementAuthorities_ParentAuthorities`
--

LOCK TABLES `EnforcementAuthorities_ParentAuthorities` WRITE;
/*!40000 ALTER TABLE `EnforcementAuthorities_ParentAuthorities` DISABLE KEYS */;
/*!40000 ALTER TABLE `EnforcementAuthorities_ParentAuthorities` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `EnforcementAuthoritiesArrestableClasses`
--

DROP TABLE IF EXISTS `EnforcementAuthoritiesArrestableClasses`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `EnforcementAuthoritiesArrestableClasses` (
  `EnforcementAuthorityId` bigint NOT NULL,
  `LegalClassId` bigint NOT NULL,
  PRIMARY KEY (`EnforcementAuthorityId`,`LegalClassId`),
  KEY `FK_EnforcementAuthoritiesArrestableLegalClasses_Enforce_idx` (`EnforcementAuthorityId`),
  KEY `FK_EnforcementAuthoritiesArrestableLegalClasses_LegalClasses_idx` (`LegalClassId`),
  CONSTRAINT `FK_EnforcementAuthoritiesArrestableLegalClasses_Enforce` FOREIGN KEY (`EnforcementAuthorityId`) REFERENCES `EnforcementAuthorities` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_EnforcementAuthoritiesArrestableLegalClasses_LegalClasses` FOREIGN KEY (`LegalClassId`) REFERENCES `LegalClasses` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `EnforcementAuthoritiesArrestableClasses`
--

LOCK TABLES `EnforcementAuthoritiesArrestableClasses` WRITE;
/*!40000 ALTER TABLE `EnforcementAuthoritiesArrestableClasses` DISABLE KEYS */;
/*!40000 ALTER TABLE `EnforcementAuthoritiesArrestableClasses` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `EntityDescriptionPatterns`
--

DROP TABLE IF EXISTS `EntityDescriptionPatterns`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `EntityDescriptionPatterns` (
  `Id` bigint NOT NULL AUTO_INCREMENT,
  `Pattern` varchar(4000) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
  `Type` int NOT NULL,
  `ApplicabilityProgId` bigint DEFAULT NULL,
  `RelativeWeight` int NOT NULL,
  PRIMARY KEY (`Id`),
  KEY `FK_EntityDescriptionPatterns_FutureProgs` (`ApplicabilityProgId`),
  CONSTRAINT `FK_EntityDescriptionPatterns_FutureProgs` FOREIGN KEY (`ApplicabilityProgId`) REFERENCES `FutureProgs` (`Id`) ON DELETE RESTRICT
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `EntityDescriptionPatterns`
--

LOCK TABLES `EntityDescriptionPatterns` WRITE;
/*!40000 ALTER TABLE `EntityDescriptionPatterns` DISABLE KEYS */;
/*!40000 ALTER TABLE `EntityDescriptionPatterns` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `EntityDescriptionPatterns_EntityDescriptions`
--

DROP TABLE IF EXISTS `EntityDescriptionPatterns_EntityDescriptions`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `EntityDescriptionPatterns_EntityDescriptions` (
  `PatternId` bigint NOT NULL,
  `EntityDescriptionId` bigint NOT NULL,
  PRIMARY KEY (`PatternId`,`EntityDescriptionId`),
  KEY `FK_EDP_EntityDescriptions_EntityDescriptions` (`EntityDescriptionId`),
  CONSTRAINT `FK_EDP_EntityDescriptions_EntityDescriptionPatterns` FOREIGN KEY (`PatternId`) REFERENCES `EntityDescriptionPatterns` (`Id`) ON DELETE RESTRICT,
  CONSTRAINT `FK_EDP_EntityDescriptions_EntityDescriptions` FOREIGN KEY (`EntityDescriptionId`) REFERENCES `EntityDescriptions` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `EntityDescriptionPatterns_EntityDescriptions`
--

LOCK TABLES `EntityDescriptionPatterns_EntityDescriptions` WRITE;
/*!40000 ALTER TABLE `EntityDescriptionPatterns_EntityDescriptions` DISABLE KEYS */;
/*!40000 ALTER TABLE `EntityDescriptionPatterns_EntityDescriptions` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `EntityDescriptions`
--

DROP TABLE IF EXISTS `EntityDescriptions`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `EntityDescriptions` (
  `Id` bigint NOT NULL AUTO_INCREMENT,
  `ShortDescription` varchar(4000) CHARACTER SET utf8 COLLATE utf8_general_ci DEFAULT NULL,
  `FullDescription` varchar(4000) CHARACTER SET utf8 COLLATE utf8_general_ci DEFAULT NULL,
  `DisplaySex` smallint NOT NULL,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `EntityDescriptions`
--

LOCK TABLES `EntityDescriptions` WRITE;
/*!40000 ALTER TABLE `EntityDescriptions` DISABLE KEYS */;
/*!40000 ALTER TABLE `EntityDescriptions` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `Ethnicities`
--

DROP TABLE IF EXISTS `Ethnicities`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `Ethnicities` (
  `Id` bigint NOT NULL AUTO_INCREMENT,
  `Name` varchar(4000) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
  `ChargenBlurb` varchar(4000) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
  `AvailabilityProgId` bigint DEFAULT NULL,
  `ParentRaceId` bigint DEFAULT NULL,
  `EthnicGroup` text CHARACTER SET utf8 COLLATE utf8_general_ci,
  `EthnicSubgroup` text CHARACTER SET utf8 COLLATE utf8_general_ci,
  `PopulationBloodModelId` bigint DEFAULT NULL,
  `TolerableTemperatureFloorEffect` double NOT NULL,
  `TolerableTemperatureCeilingEffect` double NOT NULL,
  PRIMARY KEY (`Id`),
  KEY `FK_Ethnicities_AvailabilityProg` (`AvailabilityProgId`),
  KEY `FK_Ethnicities_Races_idx` (`ParentRaceId`),
  KEY `FK_Ethnicities_PopulationBloodModels_idx` (`PopulationBloodModelId`),
  CONSTRAINT `FK_Ethnicities_AvailabilityProg` FOREIGN KEY (`AvailabilityProgId`) REFERENCES `FutureProgs` (`Id`) ON DELETE SET NULL,
  CONSTRAINT `FK_Ethnicities_PopulationBloodModels` FOREIGN KEY (`PopulationBloodModelId`) REFERENCES `PopulationBloodModels` (`Id`) ON DELETE SET NULL,
  CONSTRAINT `FK_Ethnicities_Races` FOREIGN KEY (`ParentRaceId`) REFERENCES `Races` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `Ethnicities`
--

LOCK TABLES `Ethnicities` WRITE;
/*!40000 ALTER TABLE `Ethnicities` DISABLE KEYS */;
/*!40000 ALTER TABLE `Ethnicities` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `Ethnicities_Characteristics`
--

DROP TABLE IF EXISTS `Ethnicities_Characteristics`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `Ethnicities_Characteristics` (
  `EthnicityId` bigint NOT NULL,
  `CharacteristicDefinitionId` bigint NOT NULL,
  `CharacteristicProfileId` bigint NOT NULL,
  PRIMARY KEY (`EthnicityId`,`CharacteristicDefinitionId`,`CharacteristicProfileId`),
  KEY `FK_Ethnicities_Characteristics_CharacteristicDefinitions` (`CharacteristicDefinitionId`),
  KEY `FK_Ethnicities_Characteristics_CharacteristicProfiles` (`CharacteristicProfileId`),
  CONSTRAINT `FK_Ethnicities_Characteristics_CharacteristicDefinitions` FOREIGN KEY (`CharacteristicDefinitionId`) REFERENCES `CharacteristicDefinitions` (`Id`) ON DELETE RESTRICT,
  CONSTRAINT `FK_Ethnicities_Characteristics_CharacteristicProfiles` FOREIGN KEY (`CharacteristicProfileId`) REFERENCES `CharacteristicProfiles` (`Id`) ON DELETE RESTRICT,
  CONSTRAINT `FK_Ethnicities_Characteristics_Ethnicities` FOREIGN KEY (`EthnicityId`) REFERENCES `Ethnicities` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `Ethnicities_Characteristics`
--

LOCK TABLES `Ethnicities_Characteristics` WRITE;
/*!40000 ALTER TABLE `Ethnicities_Characteristics` DISABLE KEYS */;
/*!40000 ALTER TABLE `Ethnicities_Characteristics` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `Ethnicities_ChargenResources`
--

DROP TABLE IF EXISTS `Ethnicities_ChargenResources`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `Ethnicities_ChargenResources` (
  `EthnicityId` bigint NOT NULL,
  `ChargenResourceId` bigint NOT NULL,
  `RequirementOnly` bit(1) NOT NULL DEFAULT b'0',
  `Amount` int NOT NULL,
  PRIMARY KEY (`EthnicityId`,`ChargenResourceId`),
  KEY `IX_Ethnicities_ChargenResources_ChargenResourceId` (`ChargenResourceId`),
  CONSTRAINT `FK_Ethnicities_ChargenResources_ChargenResources_ChargenResourc~` FOREIGN KEY (`ChargenResourceId`) REFERENCES `ChargenResources` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_Ethnicities_ChargenResources_Ethnicities_EthnicityId` FOREIGN KEY (`EthnicityId`) REFERENCES `Ethnicities` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `Ethnicities_ChargenResources`
--

LOCK TABLES `Ethnicities_ChargenResources` WRITE;
/*!40000 ALTER TABLE `Ethnicities_ChargenResources` DISABLE KEYS */;
/*!40000 ALTER TABLE `Ethnicities_ChargenResources` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `EthnicitiesNameCultures`
--

DROP TABLE IF EXISTS `EthnicitiesNameCultures`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `EthnicitiesNameCultures` (
  `EthnicityId` bigint NOT NULL,
  `NameCultureId` bigint NOT NULL,
  `Gender` smallint NOT NULL,
  PRIMARY KEY (`EthnicityId`,`NameCultureId`,`Gender`),
  KEY `IX_EthnicitiesNameCultures_NameCultureId` (`NameCultureId`),
  CONSTRAINT `FK_EthnicitiesNameCultures_Ethnicities` FOREIGN KEY (`EthnicityId`) REFERENCES `Ethnicities` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_EthnicitiesNameCultures_NameCultures` FOREIGN KEY (`NameCultureId`) REFERENCES `NameCulture` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `EthnicitiesNameCultures`
--

LOCK TABLES `EthnicitiesNameCultures` WRITE;
/*!40000 ALTER TABLE `EthnicitiesNameCultures` DISABLE KEYS */;
/*!40000 ALTER TABLE `EthnicitiesNameCultures` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `Exits`
--

DROP TABLE IF EXISTS `Exits`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `Exits` (
  `Id` bigint NOT NULL AUTO_INCREMENT,
  `Keywords1` varchar(255) CHARACTER SET utf8 COLLATE utf8_general_ci DEFAULT NULL,
  `Keywords2` varchar(255) CHARACTER SET utf8 COLLATE utf8_general_ci DEFAULT NULL,
  `CellId1` bigint NOT NULL,
  `CellId2` bigint NOT NULL,
  `DoorId` bigint DEFAULT NULL,
  `Direction1` int NOT NULL,
  `Direction2` int NOT NULL,
  `TimeMultiplier` double NOT NULL,
  `InboundDescription1` varchar(255) CHARACTER SET utf8 COLLATE utf8_general_ci DEFAULT NULL,
  `InboundDescription2` varchar(255) CHARACTER SET utf8 COLLATE utf8_general_ci DEFAULT NULL,
  `OutboundDescription1` varchar(255) CHARACTER SET utf8 COLLATE utf8_general_ci DEFAULT NULL,
  `OutboundDescription2` varchar(255) CHARACTER SET utf8 COLLATE utf8_general_ci DEFAULT NULL,
  `InboundTarget1` varchar(255) CHARACTER SET utf8 COLLATE utf8_general_ci DEFAULT NULL,
  `InboundTarget2` varchar(255) CHARACTER SET utf8 COLLATE utf8_general_ci DEFAULT NULL,
  `OutboundTarget1` varchar(255) CHARACTER SET utf8 COLLATE utf8_general_ci DEFAULT NULL,
  `OutboundTarget2` varchar(255) CHARACTER SET utf8 COLLATE utf8_general_ci DEFAULT NULL,
  `Verb1` varchar(255) CHARACTER SET utf8 COLLATE utf8_general_ci DEFAULT NULL,
  `Verb2` varchar(255) CHARACTER SET utf8 COLLATE utf8_general_ci DEFAULT NULL,
  `PrimaryKeyword1` varchar(255) CHARACTER SET utf8 COLLATE utf8_general_ci DEFAULT NULL,
  `PrimaryKeyword2` varchar(255) CHARACTER SET utf8 COLLATE utf8_general_ci DEFAULT NULL,
  `AcceptsDoor` bit(1) NOT NULL,
  `DoorSize` int DEFAULT NULL,
  `MaximumSizeToEnter` int NOT NULL DEFAULT '12',
  `MaximumSizeToEnterUpright` int NOT NULL DEFAULT '12',
  `FallCell` bigint DEFAULT NULL,
  `IsClimbExit` bit(1) NOT NULL DEFAULT b'0',
  `ClimbDifficulty` int NOT NULL DEFAULT '5',
  `BlockedLayers` varchar(255) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL DEFAULT '',
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `Exits`
--

LOCK TABLES `Exits` WRITE;
/*!40000 ALTER TABLE `Exits` DISABLE KEYS */;
/*!40000 ALTER TABLE `Exits` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `ExternalClanControls`
--

DROP TABLE IF EXISTS `ExternalClanControls`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `ExternalClanControls` (
  `VassalClanId` bigint NOT NULL,
  `LiegeClanId` bigint NOT NULL,
  `ControlledAppointmentId` bigint NOT NULL,
  `ControllingAppointmentId` bigint DEFAULT NULL,
  `NumberOfAppointments` int NOT NULL,
  PRIMARY KEY (`VassalClanId`,`LiegeClanId`,`ControlledAppointmentId`),
  KEY `FK_ECC_Appointments_Controlled` (`ControlledAppointmentId`),
  KEY `FK_ECC_Appointments_Controlling` (`ControllingAppointmentId`),
  KEY `FK_ECC_Clans_Liege` (`LiegeClanId`),
  CONSTRAINT `FK_ECC_Appointments_Controlled` FOREIGN KEY (`ControlledAppointmentId`) REFERENCES `Appointments` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_ECC_Appointments_Controlling` FOREIGN KEY (`ControllingAppointmentId`) REFERENCES `Appointments` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_ECC_Clans_Liege` FOREIGN KEY (`LiegeClanId`) REFERENCES `Clans` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_ECC_Clans_Vassal` FOREIGN KEY (`VassalClanId`) REFERENCES `Clans` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `ExternalClanControls`
--

LOCK TABLES `ExternalClanControls` WRITE;
/*!40000 ALTER TABLE `ExternalClanControls` DISABLE KEYS */;
/*!40000 ALTER TABLE `ExternalClanControls` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `ExternalClanControls_Appointments`
--

DROP TABLE IF EXISTS `ExternalClanControls_Appointments`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `ExternalClanControls_Appointments` (
  `VassalClanId` bigint NOT NULL,
  `LiegeClanId` bigint NOT NULL,
  `ControlledAppointmentId` bigint NOT NULL,
  `CharacterId` bigint NOT NULL,
  PRIMARY KEY (`CharacterId`,`VassalClanId`,`LiegeClanId`,`ControlledAppointmentId`),
  KEY `FK_ECC_Appointments_ClanMemberships` (`VassalClanId`,`CharacterId`),
  KEY `FK_ECC_Appointments_ExternalClanControls` (`VassalClanId`,`LiegeClanId`,`ControlledAppointmentId`),
  CONSTRAINT `FK_ECC_Appointments_ClanMemberships` FOREIGN KEY (`VassalClanId`, `CharacterId`) REFERENCES `ClanMemberships` (`ClanId`, `CharacterId`) ON DELETE RESTRICT,
  CONSTRAINT `FK_ECC_Appointments_ExternalClanControls` FOREIGN KEY (`VassalClanId`, `LiegeClanId`, `ControlledAppointmentId`) REFERENCES `ExternalClanControls` (`VassalClanId`, `LiegeClanId`, `ControlledAppointmentId`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `ExternalClanControls_Appointments`
--

LOCK TABLES `ExternalClanControls_Appointments` WRITE;
/*!40000 ALTER TABLE `ExternalClanControls_Appointments` DISABLE KEYS */;
/*!40000 ALTER TABLE `ExternalClanControls_Appointments` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `FinancialPeriods`
--

DROP TABLE IF EXISTS `FinancialPeriods`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `FinancialPeriods` (
  `Id` bigint NOT NULL AUTO_INCREMENT,
  `EconomicZoneId` bigint NOT NULL,
  `PeriodStart` datetime NOT NULL,
  `PeriodEnd` datetime NOT NULL,
  `MudPeriodStart` varchar(200) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
  `MudPeriodEnd` varchar(200) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
  PRIMARY KEY (`Id`),
  KEY `FK_FinancialPeriods_EconomicZones_idx` (`EconomicZoneId`),
  CONSTRAINT `FK_FinancialPeriods_EconomicZones` FOREIGN KEY (`EconomicZoneId`) REFERENCES `EconomicZones` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `FinancialPeriods`
--

LOCK TABLES `FinancialPeriods` WRITE;
/*!40000 ALTER TABLE `FinancialPeriods` DISABLE KEYS */;
/*!40000 ALTER TABLE `FinancialPeriods` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `ForagableProfiles`
--

DROP TABLE IF EXISTS `ForagableProfiles`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `ForagableProfiles` (
  `Id` bigint NOT NULL,
  `RevisionNumber` int NOT NULL,
  `EditableItemId` bigint NOT NULL,
  `Name` varchar(200) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL,
  PRIMARY KEY (`Id`,`RevisionNumber`),
  KEY `FK_ForagableProfiles_EditableItems_idx` (`EditableItemId`),
  CONSTRAINT `FK_ForagableProfiles_EditableItems` FOREIGN KEY (`EditableItemId`) REFERENCES `EditableItems` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `ForagableProfiles`
--

LOCK TABLES `ForagableProfiles` WRITE;
/*!40000 ALTER TABLE `ForagableProfiles` DISABLE KEYS */;
/*!40000 ALTER TABLE `ForagableProfiles` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `ForagableProfiles_Foragables`
--

DROP TABLE IF EXISTS `ForagableProfiles_Foragables`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `ForagableProfiles_Foragables` (
  `ForagableProfileId` bigint NOT NULL,
  `ForagableProfileRevisionNumber` int NOT NULL,
  `ForagableId` bigint NOT NULL,
  PRIMARY KEY (`ForagableProfileId`,`ForagableProfileRevisionNumber`,`ForagableId`),
  CONSTRAINT `FK_ForagableProfiles_Foragables_ForagableProfiles` FOREIGN KEY (`ForagableProfileId`, `ForagableProfileRevisionNumber`) REFERENCES `ForagableProfiles` (`Id`, `RevisionNumber`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `ForagableProfiles_Foragables`
--

LOCK TABLES `ForagableProfiles_Foragables` WRITE;
/*!40000 ALTER TABLE `ForagableProfiles_Foragables` DISABLE KEYS */;
/*!40000 ALTER TABLE `ForagableProfiles_Foragables` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `ForagableProfiles_HourlyYieldGains`
--

DROP TABLE IF EXISTS `ForagableProfiles_HourlyYieldGains`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `ForagableProfiles_HourlyYieldGains` (
  `ForagableProfileId` bigint NOT NULL,
  `ForagableProfileRevisionNumber` int NOT NULL,
  `ForageType` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL,
  `Yield` double NOT NULL,
  PRIMARY KEY (`ForagableProfileId`,`ForagableProfileRevisionNumber`,`ForageType`),
  CONSTRAINT `FK_ForagableProfiles_HourlyYieldGains_ForagableProfiles` FOREIGN KEY (`ForagableProfileId`, `ForagableProfileRevisionNumber`) REFERENCES `ForagableProfiles` (`Id`, `RevisionNumber`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `ForagableProfiles_HourlyYieldGains`
--

LOCK TABLES `ForagableProfiles_HourlyYieldGains` WRITE;
/*!40000 ALTER TABLE `ForagableProfiles_HourlyYieldGains` DISABLE KEYS */;
/*!40000 ALTER TABLE `ForagableProfiles_HourlyYieldGains` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `ForagableProfiles_MaximumYields`
--

DROP TABLE IF EXISTS `ForagableProfiles_MaximumYields`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `ForagableProfiles_MaximumYields` (
  `ForagableProfileId` bigint NOT NULL,
  `ForagableProfileRevisionNumber` int NOT NULL,
  `ForageType` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL,
  `Yield` double NOT NULL,
  PRIMARY KEY (`ForagableProfileId`,`ForagableProfileRevisionNumber`,`ForageType`),
  CONSTRAINT `FK_ForagableProfiles_MaximumYields_ForagableProfiles` FOREIGN KEY (`ForagableProfileId`, `ForagableProfileRevisionNumber`) REFERENCES `ForagableProfiles` (`Id`, `RevisionNumber`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `ForagableProfiles_MaximumYields`
--

LOCK TABLES `ForagableProfiles_MaximumYields` WRITE;
/*!40000 ALTER TABLE `ForagableProfiles_MaximumYields` DISABLE KEYS */;
/*!40000 ALTER TABLE `ForagableProfiles_MaximumYields` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `Foragables`
--

DROP TABLE IF EXISTS `Foragables`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `Foragables` (
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
  CONSTRAINT `FK_Foragables_EditableItems` FOREIGN KEY (`EditableItemId`) REFERENCES `EditableItems` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `Foragables`
--

LOCK TABLES `Foragables` WRITE;
/*!40000 ALTER TABLE `Foragables` DISABLE KEYS */;
/*!40000 ALTER TABLE `Foragables` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `FutureProgs`
--

DROP TABLE IF EXISTS `FutureProgs`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `FutureProgs` (
  `Id` bigint NOT NULL AUTO_INCREMENT,
  `FunctionName` varchar(255) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
  `FunctionComment` text CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
  `FunctionText` text CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NOT NULL,
  `ReturnType` bigint NOT NULL,
  `Category` varchar(255) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
  `Subcategory` varchar(255) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
  `Public` bit(1) NOT NULL DEFAULT b'0',
  `AcceptsAnyParameters` bit(1) NOT NULL DEFAULT b'0',
  `StaticType` int NOT NULL,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `FutureProgs`
--

LOCK TABLES `FutureProgs` WRITE;
/*!40000 ALTER TABLE `FutureProgs` DISABLE KEYS */;
/*!40000 ALTER TABLE `FutureProgs` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `FutureProgs_Parameters`
--

DROP TABLE IF EXISTS `FutureProgs_Parameters`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `FutureProgs_Parameters` (
  `FutureProgId` bigint NOT NULL,
  `ParameterIndex` int NOT NULL,
  `ParameterType` bigint NOT NULL,
  `ParameterName` varchar(4000) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
  PRIMARY KEY (`FutureProgId`,`ParameterIndex`),
  CONSTRAINT `FK_FutureProgs_Parameters_FutureProgs` FOREIGN KEY (`FutureProgId`) REFERENCES `FutureProgs` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `FutureProgs_Parameters`
--

LOCK TABLES `FutureProgs_Parameters` WRITE;
/*!40000 ALTER TABLE `FutureProgs_Parameters` DISABLE KEYS */;
/*!40000 ALTER TABLE `FutureProgs_Parameters` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `GameItemComponentProtos`
--

DROP TABLE IF EXISTS `GameItemComponentProtos`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `GameItemComponentProtos` (
  `Id` bigint NOT NULL,
  `RevisionNumber` int NOT NULL,
  `Type` varchar(50) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
  `Definition` text CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
  `EditableItemId` bigint NOT NULL,
  `Description` text CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
  `Name` varchar(4000) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
  PRIMARY KEY (`Id`,`RevisionNumber`),
  KEY `FK_GameItemComponentProtos_EditableItems` (`EditableItemId`),
  CONSTRAINT `FK_GameItemComponentProtos_EditableItems` FOREIGN KEY (`EditableItemId`) REFERENCES `EditableItems` (`Id`) ON DELETE RESTRICT
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `GameItemComponentProtos`
--

LOCK TABLES `GameItemComponentProtos` WRITE;
/*!40000 ALTER TABLE `GameItemComponentProtos` DISABLE KEYS */;
/*!40000 ALTER TABLE `GameItemComponentProtos` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `GameItemComponents`
--

DROP TABLE IF EXISTS `GameItemComponents`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `GameItemComponents` (
  `Id` bigint NOT NULL AUTO_INCREMENT,
  `GameItemComponentProtoId` bigint NOT NULL,
  `GameItemComponentProtoRevision` int NOT NULL,
  `Definition` text CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
  `GameItemId` bigint NOT NULL,
  PRIMARY KEY (`Id`),
  KEY `FK_GameItemComponents_GameItems` (`GameItemId`),
  CONSTRAINT `FK_GameItemComponents_GameItems` FOREIGN KEY (`GameItemId`) REFERENCES `GameItems` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `GameItemComponents`
--

LOCK TABLES `GameItemComponents` WRITE;
/*!40000 ALTER TABLE `GameItemComponents` DISABLE KEYS */;
/*!40000 ALTER TABLE `GameItemComponents` ENABLE KEYS */;
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
-- Table structure for table `GameItemProtoExtraDescriptions`
--

DROP TABLE IF EXISTS `GameItemProtoExtraDescriptions`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `GameItemProtoExtraDescriptions` (
  `GameItemProtoId` bigint NOT NULL,
  `GameItemProtoRevisionNumber` int NOT NULL,
  `ApplicabilityProgId` bigint NOT NULL,
  `ShortDescription` varchar(500) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `FullDescription` varchar(2000) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `FullDescriptionAddendum` varchar(500) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `Priority` int NOT NULL,
  PRIMARY KEY (`GameItemProtoId`,`GameItemProtoRevisionNumber`,`ApplicabilityProgId`),
  KEY `IX_GameItemProtoExtraDescriptions_ApplicabilityProgId` (`ApplicabilityProgId`),
  CONSTRAINT `FK_GameItemProtoExtraDescriptions_FutureProgs` FOREIGN KEY (`ApplicabilityProgId`) REFERENCES `FutureProgs` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_GameItemProtoExtraDescriptions_GameItemProtos` FOREIGN KEY (`GameItemProtoId`, `GameItemProtoRevisionNumber`) REFERENCES `GameItemProtos` (`Id`, `RevisionNumber`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `GameItemProtoExtraDescriptions`
--

LOCK TABLES `GameItemProtoExtraDescriptions` WRITE;
/*!40000 ALTER TABLE `GameItemProtoExtraDescriptions` DISABLE KEYS */;
/*!40000 ALTER TABLE `GameItemProtoExtraDescriptions` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `GameItemProtos`
--

DROP TABLE IF EXISTS `GameItemProtos`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `GameItemProtos` (
  `Id` bigint NOT NULL,
  `RevisionNumber` int NOT NULL,
  `Name` varchar(255) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
  `Keywords` text CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
  `MaterialId` bigint NOT NULL,
  `EditableItemId` bigint NOT NULL,
  `Size` int NOT NULL,
  `Weight` double NOT NULL,
  `ReadOnly` bit(1) NOT NULL DEFAULT b'0',
  `LongDescription` text CHARACTER SET utf8 COLLATE utf8_general_ci,
  `ItemGroupId` bigint DEFAULT NULL,
  `OnDestroyedGameItemProtoId` bigint DEFAULT NULL,
  `HealthStrategyId` bigint DEFAULT NULL,
  `BaseItemQuality` int NOT NULL DEFAULT '5',
  `CustomColour` varchar(45) CHARACTER SET utf8 COLLATE utf8_general_ci DEFAULT NULL,
  `HighPriority` bit(1) NOT NULL DEFAULT b'0',
  `MorphGameItemProtoId` bigint DEFAULT NULL,
  `MorphTimeSeconds` int NOT NULL,
  `MorphEmote` varchar(1000) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL DEFAULT '$0 $?1|morphs into $1|decays into nothing$.',
  `ShortDescription` varchar(1000) CHARACTER SET utf8 COLLATE utf8_general_ci DEFAULT NULL,
  `FullDescription` varchar(4000) CHARACTER SET utf8 COLLATE utf8_general_ci DEFAULT NULL,
  `PermitPlayerSkins` bit(1) NOT NULL DEFAULT b'0',
  `CostInBaseCurrency` decimal(58,29) NOT NULL DEFAULT '0.00000000000000000000000000000',
  `IsHiddenFromPlayers` tinyint(1) NOT NULL DEFAULT '0',
  PRIMARY KEY (`Id`,`RevisionNumber`),
  KEY `FK_GameItemProtos_EditableItems` (`EditableItemId`),
  KEY `FK_GameItemProtos_ItemGroups_idx` (`ItemGroupId`),
  CONSTRAINT `FK_GameItemProtos_EditableItems` FOREIGN KEY (`EditableItemId`) REFERENCES `EditableItems` (`Id`) ON DELETE RESTRICT,
  CONSTRAINT `FK_GameItemProtos_ItemGroups` FOREIGN KEY (`ItemGroupId`) REFERENCES `ItemGroups` (`Id`) ON DELETE SET NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `GameItemProtos`
--

LOCK TABLES `GameItemProtos` WRITE;
/*!40000 ALTER TABLE `GameItemProtos` DISABLE KEYS */;
/*!40000 ALTER TABLE `GameItemProtos` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `GameItemProtos_DefaultVariables`
--

DROP TABLE IF EXISTS `GameItemProtos_DefaultVariables`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `GameItemProtos_DefaultVariables` (
  `GameItemProtoId` bigint NOT NULL,
  `VariableName` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL,
  `GameItemProtoRevNum` int NOT NULL,
  `VariableValue` text CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL,
  PRIMARY KEY (`GameItemProtoId`,`GameItemProtoRevNum`,`VariableName`),
  CONSTRAINT `FK_GameItemProtos_DefaultValues_GameItemProtos` FOREIGN KEY (`GameItemProtoId`, `GameItemProtoRevNum`) REFERENCES `GameItemProtos` (`Id`, `RevisionNumber`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `GameItemProtos_DefaultVariables`
--

LOCK TABLES `GameItemProtos_DefaultVariables` WRITE;
/*!40000 ALTER TABLE `GameItemProtos_DefaultVariables` DISABLE KEYS */;
/*!40000 ALTER TABLE `GameItemProtos_DefaultVariables` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `GameItemProtos_GameItemComponentProtos`
--

DROP TABLE IF EXISTS `GameItemProtos_GameItemComponentProtos`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `GameItemProtos_GameItemComponentProtos` (
  `GameItemProtoId` bigint NOT NULL,
  `GameItemComponentProtoId` bigint NOT NULL,
  `GameItemProtoRevision` int NOT NULL,
  `GameItemComponentRevision` int NOT NULL,
  PRIMARY KEY (`GameItemProtoId`,`GameItemComponentProtoId`,`GameItemProtoRevision`,`GameItemComponentRevision`),
  KEY `FK_GIPGICP_GameItemComponentProtos` (`GameItemComponentProtoId`,`GameItemComponentRevision`),
  KEY `FK_GIPGICP_GameItemProtos` (`GameItemProtoId`,`GameItemProtoRevision`),
  CONSTRAINT `FK_GIPGICP_GameItemComponentProtos` FOREIGN KEY (`GameItemComponentProtoId`, `GameItemComponentRevision`) REFERENCES `GameItemComponentProtos` (`Id`, `RevisionNumber`) ON DELETE CASCADE,
  CONSTRAINT `FK_GIPGICP_GameItemProtos` FOREIGN KEY (`GameItemProtoId`, `GameItemProtoRevision`) REFERENCES `GameItemProtos` (`Id`, `RevisionNumber`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `GameItemProtos_GameItemComponentProtos`
--

LOCK TABLES `GameItemProtos_GameItemComponentProtos` WRITE;
/*!40000 ALTER TABLE `GameItemProtos_GameItemComponentProtos` DISABLE KEYS */;
/*!40000 ALTER TABLE `GameItemProtos_GameItemComponentProtos` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `GameItemProtos_OnLoadProgs`
--

DROP TABLE IF EXISTS `GameItemProtos_OnLoadProgs`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `GameItemProtos_OnLoadProgs` (
  `GameItemProtoId` bigint NOT NULL,
  `GameItemProtoRevisionNumber` int NOT NULL,
  `FutureProgId` bigint NOT NULL,
  PRIMARY KEY (`GameItemProtoId`,`GameItemProtoRevisionNumber`,`FutureProgId`),
  KEY `FK_GameItemProtos_OnLoadProgs_FutureProgs_idx` (`FutureProgId`),
  CONSTRAINT `FK_GameItemProtos_OnLoadProgs_FutureProgs` FOREIGN KEY (`FutureProgId`) REFERENCES `FutureProgs` (`Id`) ON DELETE RESTRICT,
  CONSTRAINT `FK_GameItemProtos_OnLoadProgs_GameItemProtos` FOREIGN KEY (`GameItemProtoId`, `GameItemProtoRevisionNumber`) REFERENCES `GameItemProtos` (`Id`, `RevisionNumber`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `GameItemProtos_OnLoadProgs`
--

LOCK TABLES `GameItemProtos_OnLoadProgs` WRITE;
/*!40000 ALTER TABLE `GameItemProtos_OnLoadProgs` DISABLE KEYS */;
/*!40000 ALTER TABLE `GameItemProtos_OnLoadProgs` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `GameItemProtos_Tags`
--

DROP TABLE IF EXISTS `GameItemProtos_Tags`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `GameItemProtos_Tags` (
  `GameItemProtoId` bigint NOT NULL,
  `TagId` bigint NOT NULL,
  `GameItemProtoRevisionNumber` int NOT NULL,
  PRIMARY KEY (`GameItemProtoId`,`TagId`,`GameItemProtoRevisionNumber`),
  KEY `FK_GameItemProtos_Tags_Tags` (`TagId`),
  KEY `FK_GameItemProtos_Tags_GameItemProtos` (`GameItemProtoId`,`GameItemProtoRevisionNumber`),
  CONSTRAINT `FK_GameItemProtos_Tags_GameItemProtos` FOREIGN KEY (`GameItemProtoId`, `GameItemProtoRevisionNumber`) REFERENCES `GameItemProtos` (`Id`, `RevisionNumber`) ON DELETE CASCADE,
  CONSTRAINT `FK_GameItemProtos_Tags_Tags` FOREIGN KEY (`TagId`) REFERENCES `Tags` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `GameItemProtos_Tags`
--

LOCK TABLES `GameItemProtos_Tags` WRITE;
/*!40000 ALTER TABLE `GameItemProtos_Tags` DISABLE KEYS */;
/*!40000 ALTER TABLE `GameItemProtos_Tags` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `GameItems`
--

DROP TABLE IF EXISTS `GameItems`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `GameItems` (
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
  `PositionTargetType` varchar(45) CHARACTER SET utf8 COLLATE utf8_general_ci DEFAULT NULL,
  `PositionEmote` text CHARACTER SET utf8 COLLATE utf8_general_ci,
  `MorphTimeRemaining` int DEFAULT NULL,
  `EffectData` mediumtext CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
  `SkinId` bigint DEFAULT NULL,
  PRIMARY KEY (`Id`),
  KEY `FK_GameItems_GameItems_Containers_idx` (`ContainerId`),
  CONSTRAINT `FK_GameItems_GameItems_Containers` FOREIGN KEY (`ContainerId`) REFERENCES `GameItems` (`Id`) ON DELETE SET NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `GameItems`
--

LOCK TABLES `GameItems` WRITE;
/*!40000 ALTER TABLE `GameItems` DISABLE KEYS */;
/*!40000 ALTER TABLE `GameItems` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `GameItems_MagicResources`
--

DROP TABLE IF EXISTS `GameItems_MagicResources`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `GameItems_MagicResources` (
  `GameItemId` bigint NOT NULL,
  `MagicResourceId` bigint NOT NULL,
  `Amount` double NOT NULL,
  PRIMARY KEY (`GameItemId`,`MagicResourceId`),
  KEY `FK_GameItems_MagicResources_MagicResources_idx` (`MagicResourceId`),
  CONSTRAINT `FK_GameItems_MagicResources_GameItems` FOREIGN KEY (`GameItemId`) REFERENCES `GameItems` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_GameItems_MagicResources_MagicResources` FOREIGN KEY (`MagicResourceId`) REFERENCES `MagicResources` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `GameItems_MagicResources`
--

LOCK TABLES `GameItems_MagicResources` WRITE;
/*!40000 ALTER TABLE `GameItems_MagicResources` DISABLE KEYS */;
/*!40000 ALTER TABLE `GameItems_MagicResources` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `GameItemSkins`
--

DROP TABLE IF EXISTS `GameItemSkins`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `GameItemSkins` (
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
  CONSTRAINT `FK_GameItemSkins_EditableItems` FOREIGN KEY (`EditableItemId`) REFERENCES `EditableItems` (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `GameItemSkins`
--

LOCK TABLES `GameItemSkins` WRITE;
/*!40000 ALTER TABLE `GameItemSkins` DISABLE KEYS */;
/*!40000 ALTER TABLE `GameItemSkins` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `Gases`
--

DROP TABLE IF EXISTS `Gases`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `Gases` (
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
  PRIMARY KEY (`Id`),
  KEY `FK_Gases_Gases_idx` (`CountAsId`),
  KEY `FK_Gases_Liquids_idx` (`PrecipitateId`),
  KEY `IX_Gases_DrugId` (`DrugId`),
  CONSTRAINT `FK_Gases_Drugs` FOREIGN KEY (`DrugId`) REFERENCES `Drugs` (`Id`) ON DELETE SET NULL,
  CONSTRAINT `FK_Gases_Gases` FOREIGN KEY (`CountAsId`) REFERENCES `Gases` (`Id`) ON DELETE SET NULL,
  CONSTRAINT `FK_Gases_Liquids` FOREIGN KEY (`PrecipitateId`) REFERENCES `Liquids` (`Id`) ON DELETE SET NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `Gases`
--

LOCK TABLES `Gases` WRITE;
/*!40000 ALTER TABLE `Gases` DISABLE KEYS */;
/*!40000 ALTER TABLE `Gases` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `Gases_Tags`
--

DROP TABLE IF EXISTS `Gases_Tags`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `Gases_Tags` (
  `GasId` bigint NOT NULL,
  `TagId` bigint NOT NULL,
  PRIMARY KEY (`GasId`,`TagId`),
  KEY `FK_Gases_Tags_Tags_idx` (`TagId`),
  CONSTRAINT `FK_Gases_Tags_Gases` FOREIGN KEY (`GasId`) REFERENCES `Gases` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_Gases_Tags_Tags` FOREIGN KEY (`TagId`) REFERENCES `Tags` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `Gases_Tags`
--

LOCK TABLES `Gases_Tags` WRITE;
/*!40000 ALTER TABLE `Gases_Tags` DISABLE KEYS */;
/*!40000 ALTER TABLE `Gases_Tags` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `GPTMessages`
--

DROP TABLE IF EXISTS `GPTMessages`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `GPTMessages` (
  `Id` bigint NOT NULL AUTO_INCREMENT,
  `GPTThreadId` bigint NOT NULL,
  `CharacterId` bigint DEFAULT NULL,
  `Message` text CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
  `Response` text CHARACTER SET utf8 COLLATE utf8_general_ci,
  PRIMARY KEY (`Id`),
  KEY `IX_GPTMessages_CharacterId` (`CharacterId`),
  KEY `IX_GPTMessages_GPTThreadId` (`GPTThreadId`),
  CONSTRAINT `FK_GPTMessages_Characters` FOREIGN KEY (`CharacterId`) REFERENCES `Characters` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_GPTMessages_GPTThreads` FOREIGN KEY (`GPTThreadId`) REFERENCES `GPTThreads` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `GPTMessages`
--

LOCK TABLES `GPTMessages` WRITE;
/*!40000 ALTER TABLE `GPTMessages` DISABLE KEYS */;
/*!40000 ALTER TABLE `GPTMessages` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `GPTThreads`
--

DROP TABLE IF EXISTS `GPTThreads`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `GPTThreads` (
  `Id` bigint NOT NULL AUTO_INCREMENT,
  `Name` varchar(200) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
  `Prompt` text CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
  `Model` varchar(200) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
  `Temperature` double NOT NULL,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `GPTThreads`
--

LOCK TABLES `GPTThreads` WRITE;
/*!40000 ALTER TABLE `GPTThreads` DISABLE KEYS */;
/*!40000 ALTER TABLE `GPTThreads` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `Grids`
--

DROP TABLE IF EXISTS `Grids`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `Grids` (
  `Id` bigint NOT NULL AUTO_INCREMENT,
  `GridType` varchar(45) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
  `Definition` mediumtext CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `Grids`
--

LOCK TABLES `Grids` WRITE;
/*!40000 ALTER TABLE `Grids` DISABLE KEYS */;
/*!40000 ALTER TABLE `Grids` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `GroupAIs`
--

DROP TABLE IF EXISTS `GroupAIs`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `GroupAIs` (
  `Id` bigint NOT NULL AUTO_INCREMENT,
  `Name` varchar(200) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
  `GroupAITemplateId` bigint NOT NULL,
  `Data` mediumtext CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
  `Definition` mediumtext CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
  PRIMARY KEY (`Id`),
  KEY `FK_GroupAIs_GroupAITemplates_idx` (`GroupAITemplateId`),
  CONSTRAINT `FK_GroupAIs_GroupAITemplates` FOREIGN KEY (`GroupAITemplateId`) REFERENCES `GroupAITemplates` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `GroupAIs`
--

LOCK TABLES `GroupAIs` WRITE;
/*!40000 ALTER TABLE `GroupAIs` DISABLE KEYS */;
/*!40000 ALTER TABLE `GroupAIs` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `GroupAITemplates`
--

DROP TABLE IF EXISTS `GroupAITemplates`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `GroupAITemplates` (
  `Id` bigint NOT NULL AUTO_INCREMENT,
  `Name` varchar(200) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
  `Definition` mediumtext CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `GroupAITemplates`
--

LOCK TABLES `GroupAITemplates` WRITE;
/*!40000 ALTER TABLE `GroupAITemplates` DISABLE KEYS */;
/*!40000 ALTER TABLE `GroupAITemplates` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `Guests`
--

DROP TABLE IF EXISTS `Guests`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `Guests` (
  `CharacterId` bigint NOT NULL,
  PRIMARY KEY (`CharacterId`),
  CONSTRAINT `FK_Guests_Characters` FOREIGN KEY (`CharacterId`) REFERENCES `Characters` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `Guests`
--

LOCK TABLES `Guests` WRITE;
/*!40000 ALTER TABLE `Guests` DISABLE KEYS */;
/*!40000 ALTER TABLE `Guests` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `HealthStrategies`
--

DROP TABLE IF EXISTS `HealthStrategies`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `HealthStrategies` (
  `Id` bigint NOT NULL AUTO_INCREMENT,
  `Name` varchar(200) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NOT NULL,
  `Type` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NOT NULL,
  `Definition` text CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NOT NULL,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `HealthStrategies`
--

LOCK TABLES `HealthStrategies` WRITE;
/*!40000 ALTER TABLE `HealthStrategies` DISABLE KEYS */;
/*!40000 ALTER TABLE `HealthStrategies` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `HearingProfiles`
--

DROP TABLE IF EXISTS `HearingProfiles`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `HearingProfiles` (
  `Id` bigint NOT NULL AUTO_INCREMENT,
  `Name` varchar(4000) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
  `Definition` text CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
  `Type` varchar(4000) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
  `SurveyDescription` varchar(4000) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `HearingProfiles`
--

LOCK TABLES `HearingProfiles` WRITE;
/*!40000 ALTER TABLE `HearingProfiles` DISABLE KEYS */;
/*!40000 ALTER TABLE `HearingProfiles` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `HeightWeightModels`
--

DROP TABLE IF EXISTS `HeightWeightModels`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `HeightWeightModels` (
  `Id` bigint NOT NULL AUTO_INCREMENT,
  `Name` varchar(4000) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
  `MeanHeight` double NOT NULL,
  `MeanBMI` double NOT NULL,
  `StddevHeight` double NOT NULL,
  `StddevBMI` double NOT NULL,
  `BMIMultiplier` double NOT NULL,
  `MeanWeight` double DEFAULT NULL,
  `StddevWeight` double DEFAULT NULL,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `HeightWeightModels`
--

LOCK TABLES `HeightWeightModels` WRITE;
/*!40000 ALTER TABLE `HeightWeightModels` DISABLE KEYS */;
/*!40000 ALTER TABLE `HeightWeightModels` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `Helpfiles`
--

DROP TABLE IF EXISTS `Helpfiles`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `Helpfiles` (
  `Id` bigint NOT NULL AUTO_INCREMENT,
  `Name` varchar(255) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
  `Category` varchar(255) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
  `Subcategory` varchar(255) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
  `TagLine` varchar(500) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
  `PublicText` text CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
  `RuleId` bigint DEFAULT NULL,
  `Keywords` varchar(500) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
  `LastEditedBy` varchar(100) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
  `LastEditedDate` datetime NOT NULL,
  PRIMARY KEY (`Id`),
  KEY `FK_Helpfiles_FutureProgs` (`RuleId`),
  CONSTRAINT `FK_Helpfiles_FutureProgs` FOREIGN KEY (`RuleId`) REFERENCES `FutureProgs` (`Id`) ON DELETE RESTRICT
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `Helpfiles`
--

LOCK TABLES `Helpfiles` WRITE;
/*!40000 ALTER TABLE `Helpfiles` DISABLE KEYS */;
/*!40000 ALTER TABLE `Helpfiles` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `Helpfiles_ExtraTexts`
--

DROP TABLE IF EXISTS `Helpfiles_ExtraTexts`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `Helpfiles_ExtraTexts` (
  `HelpfileId` bigint NOT NULL,
  `DisplayOrder` int NOT NULL,
  `Text` text CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
  `RuleId` bigint NOT NULL,
  PRIMARY KEY (`HelpfileId`,`DisplayOrder`),
  KEY `FK_Helpfiles_ExtraTexts_FutureProgs` (`RuleId`),
  CONSTRAINT `FK_Helpfiles_ExtraTexts_FutureProgs` FOREIGN KEY (`RuleId`) REFERENCES `FutureProgs` (`Id`) ON DELETE RESTRICT,
  CONSTRAINT `FK_Helpfiles_ExtraTexts_Helpfiles` FOREIGN KEY (`HelpfileId`) REFERENCES `Helpfiles` (`Id`) ON DELETE RESTRICT
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `Helpfiles_ExtraTexts`
--

LOCK TABLES `Helpfiles_ExtraTexts` WRITE;
/*!40000 ALTER TABLE `Helpfiles_ExtraTexts` DISABLE KEYS */;
/*!40000 ALTER TABLE `Helpfiles_ExtraTexts` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `Hooks`
--

DROP TABLE IF EXISTS `Hooks`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `Hooks` (
  `Id` bigint NOT NULL AUTO_INCREMENT,
  `Name` varchar(255) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
  `Definition` text CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NOT NULL,
  `Type` varchar(255) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
  `Category` varchar(255) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
  `TargetEventType` int NOT NULL,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `Hooks`
--

LOCK TABLES `Hooks` WRITE;
/*!40000 ALTER TABLE `Hooks` DISABLE KEYS */;
/*!40000 ALTER TABLE `Hooks` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `Hooks_Perceivables`
--

DROP TABLE IF EXISTS `Hooks_Perceivables`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `Hooks_Perceivables` (
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
  CONSTRAINT `FK_Hooks_Perceivables_Bodies` FOREIGN KEY (`BodyId`) REFERENCES `Bodies` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_Hooks_Perceivables_Cells` FOREIGN KEY (`CellId`) REFERENCES `Cells` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_Hooks_Perceivables_Characters` FOREIGN KEY (`CharacterId`) REFERENCES `Characters` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_Hooks_Perceivables_GameItems` FOREIGN KEY (`GameItemId`) REFERENCES `GameItems` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_Hooks_Perceivables_Hooks` FOREIGN KEY (`HookId`) REFERENCES `Hooks` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_Hooks_Perceivables_Shards` FOREIGN KEY (`ShardId`) REFERENCES `Shards` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_Hooks_Perceivables_Zones` FOREIGN KEY (`ZoneId`) REFERENCES `Zones` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `Hooks_Perceivables`
--

LOCK TABLES `Hooks_Perceivables` WRITE;
/*!40000 ALTER TABLE `Hooks_Perceivables` DISABLE KEYS */;
/*!40000 ALTER TABLE `Hooks_Perceivables` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `Improvers`
--

DROP TABLE IF EXISTS `Improvers`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `Improvers` (
  `Id` bigint NOT NULL AUTO_INCREMENT,
  `Name` varchar(255) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
  `Definition` text CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
  `Type` varchar(50) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `Improvers`
--

LOCK TABLES `Improvers` WRITE;
/*!40000 ALTER TABLE `Improvers` DISABLE KEYS */;
/*!40000 ALTER TABLE `Improvers` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `Infections`
--

DROP TABLE IF EXISTS `Infections`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `Infections` (
  `Id` bigint NOT NULL AUTO_INCREMENT,
  `InfectionType` int NOT NULL,
  `Virulence` int NOT NULL,
  `Intensity` double NOT NULL,
  `OwnerId` bigint NOT NULL,
  `WoundId` bigint DEFAULT NULL,
  `BodypartId` bigint DEFAULT NULL,
  `Immunity` double NOT NULL,
  PRIMARY KEY (`Id`),
  KEY `FK_Infections_Bodyparts_idx` (`BodypartId`),
  KEY `FK_Infections_Bodies_idx` (`OwnerId`),
  KEY `FK_Infections_Wounds_idx` (`WoundId`),
  CONSTRAINT `FK_Infections_Bodies` FOREIGN KEY (`OwnerId`) REFERENCES `Bodies` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_Infections_Bodyparts` FOREIGN KEY (`BodypartId`) REFERENCES `BodypartProto` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_Infections_Wounds` FOREIGN KEY (`WoundId`) REFERENCES `Wounds` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `Infections`
--

LOCK TABLES `Infections` WRITE;
/*!40000 ALTER TABLE `Infections` DISABLE KEYS */;
/*!40000 ALTER TABLE `Infections` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `ItemGroupForms`
--

DROP TABLE IF EXISTS `ItemGroupForms`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `ItemGroupForms` (
  `Id` bigint NOT NULL AUTO_INCREMENT,
  `ItemGroupId` bigint NOT NULL,
  `Type` varchar(45) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL,
  `Definition` text CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL,
  PRIMARY KEY (`Id`),
  KEY `FK_ItemGroupForms_ItemGroups_idx` (`ItemGroupId`),
  CONSTRAINT `FK_ItemGroupForms_ItemGroups` FOREIGN KEY (`ItemGroupId`) REFERENCES `ItemGroups` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `ItemGroupForms`
--

LOCK TABLES `ItemGroupForms` WRITE;
/*!40000 ALTER TABLE `ItemGroupForms` DISABLE KEYS */;
/*!40000 ALTER TABLE `ItemGroupForms` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `ItemGroups`
--

DROP TABLE IF EXISTS `ItemGroups`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `ItemGroups` (
  `Id` bigint NOT NULL AUTO_INCREMENT,
  `Name` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci DEFAULT NULL,
  `Keywords` varchar(1000) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci DEFAULT NULL,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `ItemGroups`
--

LOCK TABLES `ItemGroups` WRITE;
/*!40000 ALTER TABLE `ItemGroups` DISABLE KEYS */;
/*!40000 ALTER TABLE `ItemGroups` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `JobFindingLocations`
--

DROP TABLE IF EXISTS `JobFindingLocations`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `JobFindingLocations` (
  `EconomicZoneId` bigint NOT NULL,
  `CellId` bigint NOT NULL,
  PRIMARY KEY (`EconomicZoneId`,`CellId`),
  KEY `IX_JobFindingLocations_CellId` (`CellId`),
  CONSTRAINT `FK_JobFindingLocations_Cells` FOREIGN KEY (`CellId`) REFERENCES `Cells` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_JobFindingLocations_EconomicZones` FOREIGN KEY (`EconomicZoneId`) REFERENCES `EconomicZones` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `JobFindingLocations`
--

LOCK TABLES `JobFindingLocations` WRITE;
/*!40000 ALTER TABLE `JobFindingLocations` DISABLE KEYS */;
/*!40000 ALTER TABLE `JobFindingLocations` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `JobListings`
--

DROP TABLE IF EXISTS `JobListings`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `JobListings` (
  `Id` bigint NOT NULL AUTO_INCREMENT,
  `PosterId` bigint NOT NULL,
  `IsReadyToBePosted` bit(1) NOT NULL,
  `IsArchived` bit(1) NOT NULL,
  `Name` varchar(200) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
  `PosterType` varchar(100) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
  `JobListingType` varchar(100) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
  `Definition` text CHARACTER SET utf8 COLLATE utf8_general_ci,
  `Description` text CHARACTER SET utf8 COLLATE utf8_general_ci,
  `MoneyPaidIn` text CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
  `MaximumDuration` varchar(200) CHARACTER SET utf8 COLLATE utf8_general_ci DEFAULT NULL,
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
  CONSTRAINT `FK_JobListings_ActiveProjectLabours` FOREIGN KEY (`RequiredProjectId`, `RequiredProjectLabourId`) REFERENCES `ActiveProjectLabours` (`ActiveProjectId`, `ProjectLabourRequirementsId`),
  CONSTRAINT `FK_JobListings_ActiveProjects` FOREIGN KEY (`RequiredProjectId`) REFERENCES `ActiveProjects` (`Id`),
  CONSTRAINT `FK_JobListings_Appointments` FOREIGN KEY (`AppointmentId`) REFERENCES `Appointments` (`Id`),
  CONSTRAINT `FK_JobListings_BankAccounts` FOREIGN KEY (`BankAccountId`) REFERENCES `BankAccounts` (`Id`),
  CONSTRAINT `FK_JobListings_Clans` FOREIGN KEY (`ClanId`) REFERENCES `Clans` (`Id`),
  CONSTRAINT `FK_JobListings_EconomicZones` FOREIGN KEY (`EconomicZoneId`) REFERENCES `EconomicZones` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_JobListings_FutureProgs` FOREIGN KEY (`EligibilityProgId`) REFERENCES `FutureProgs` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_JobListings_Paygrades` FOREIGN KEY (`PaygradeId`) REFERENCES `Paygrades` (`Id`),
  CONSTRAINT `FK_JobListings_Projects` FOREIGN KEY (`PersonalProjectId`, `PersonalProjectRevisionNumber`) REFERENCES `Projects` (`Id`, `RevisionNumber`),
  CONSTRAINT `FK_JobListings_Ranks` FOREIGN KEY (`RankId`) REFERENCES `Ranks` (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `JobListings`
--

LOCK TABLES `JobListings` WRITE;
/*!40000 ALTER TABLE `JobListings` DISABLE KEYS */;
/*!40000 ALTER TABLE `JobListings` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `knowledges`
--

DROP TABLE IF EXISTS `knowledges`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `knowledges` (
  `Id` bigint NOT NULL AUTO_INCREMENT,
  `Name` varchar(200) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
  `Description` text CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
  `LongDescription` text CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
  `Type` varchar(200) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
  `Subtype` varchar(200) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
  `LearnableType` int NOT NULL,
  `LearnDifficulty` int NOT NULL DEFAULT '7',
  `TeachDifficulty` int NOT NULL DEFAULT '7',
  `LearningSessionsRequired` int NOT NULL,
  `CanAcquireProgId` bigint DEFAULT NULL,
  `CanLearnProgId` bigint DEFAULT NULL,
  PRIMARY KEY (`Id`),
  KEY `FK_KNOWLEDGES_FUTUREPROGS_ACQUIRE_idx` (`CanAcquireProgId`),
  KEY `FK_KNOWLEDGES_FUTUREPROGS_LEARN_idx` (`CanLearnProgId`),
  CONSTRAINT `FK_KNOWLEDGES_FUTUREPROGS_ACQUIRE` FOREIGN KEY (`CanAcquireProgId`) REFERENCES `FutureProgs` (`Id`) ON DELETE SET NULL,
  CONSTRAINT `FK_KNOWLEDGES_FUTUREPROGS_LEARN` FOREIGN KEY (`CanLearnProgId`) REFERENCES `FutureProgs` (`Id`) ON DELETE SET NULL
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
-- Table structure for table `KnowledgesCosts`
--

DROP TABLE IF EXISTS `KnowledgesCosts`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `KnowledgesCosts` (
  `KnowledgeId` bigint NOT NULL,
  `ChargenResourceId` bigint NOT NULL,
  `Cost` int NOT NULL,
  PRIMARY KEY (`KnowledgeId`,`ChargenResourceId`),
  KEY `FK_KnowledgesCosts_ChargenResources_idx` (`ChargenResourceId`),
  KEY `FK_KnowledgesCosts_Knowledges_idx` (`KnowledgeId`),
  CONSTRAINT `FK_KnowledgesCosts_ChargenResources` FOREIGN KEY (`ChargenResourceId`) REFERENCES `ChargenResources` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_KnowledgesCosts_Knowledges` FOREIGN KEY (`KnowledgeId`) REFERENCES `knowledges` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `KnowledgesCosts`
--

LOCK TABLES `KnowledgesCosts` WRITE;
/*!40000 ALTER TABLE `KnowledgesCosts` DISABLE KEYS */;
/*!40000 ALTER TABLE `KnowledgesCosts` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `LanguageDifficultyModels`
--

DROP TABLE IF EXISTS `LanguageDifficultyModels`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `LanguageDifficultyModels` (
  `Id` bigint NOT NULL AUTO_INCREMENT,
  `Definition` longtext CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
  `Type` varchar(255) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
  `Name` varchar(255) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `LanguageDifficultyModels`
--

LOCK TABLES `LanguageDifficultyModels` WRITE;
/*!40000 ALTER TABLE `LanguageDifficultyModels` DISABLE KEYS */;
/*!40000 ALTER TABLE `LanguageDifficultyModels` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `Languages`
--

DROP TABLE IF EXISTS `Languages`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `Languages` (
  `Id` bigint NOT NULL AUTO_INCREMENT,
  `DifficultyModel` bigint NOT NULL,
  `LinkedTraitId` bigint NOT NULL,
  `UnknownLanguageDescription` varchar(255) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
  `LanguageObfuscationFactor` double NOT NULL,
  `Name` varchar(255) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
  `DefaultLearnerAccentId` bigint DEFAULT NULL,
  PRIMARY KEY (`Id`),
  KEY `FK_Languages_Accents_idx` (`DefaultLearnerAccentId`),
  KEY `FK_Languages_LanguageDifficultyModels` (`DifficultyModel`),
  KEY `FK_Languages_TraitDefinitions` (`LinkedTraitId`),
  CONSTRAINT `FK_Languages_Accents` FOREIGN KEY (`DefaultLearnerAccentId`) REFERENCES `Accents` (`Id`) ON DELETE RESTRICT,
  CONSTRAINT `FK_Languages_LanguageDifficultyModels` FOREIGN KEY (`DifficultyModel`) REFERENCES `LanguageDifficultyModels` (`Id`) ON DELETE RESTRICT,
  CONSTRAINT `FK_Languages_TraitDefinitions` FOREIGN KEY (`LinkedTraitId`) REFERENCES `TraitDefinitions` (`Id`) ON DELETE RESTRICT
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `Languages`
--

LOCK TABLES `Languages` WRITE;
/*!40000 ALTER TABLE `Languages` DISABLE KEYS */;
/*!40000 ALTER TABLE `Languages` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `Laws`
--

DROP TABLE IF EXISTS `Laws`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `Laws` (
  `Id` bigint NOT NULL AUTO_INCREMENT,
  `Name` varchar(250) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
  `LegalAuthorityId` bigint NOT NULL,
  `CrimeType` int NOT NULL,
  `ActivePeriod` double NOT NULL,
  `EnforcementStrategy` varchar(500) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
  `LawAppliesProgId` bigint DEFAULT NULL,
  `EnforcementPriority` int NOT NULL,
  `CanBeAppliedAutomatically` bit(1) NOT NULL,
  `CanBeArrested` bit(1) NOT NULL,
  `CanBeOfferedBail` bit(1) NOT NULL,
  `DoNotAutomaticallyApplyRepeats` bit(1) NOT NULL DEFAULT b'0',
  `PunishmentStrategy` text CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
  PRIMARY KEY (`Id`),
  KEY `FK_Laws_FutureProgs_idx` (`LawAppliesProgId`),
  KEY `FK_Laws_LegalAuthority_idx` (`LegalAuthorityId`),
  CONSTRAINT `FK_Laws_FutureProgs` FOREIGN KEY (`LawAppliesProgId`) REFERENCES `FutureProgs` (`Id`) ON DELETE SET NULL,
  CONSTRAINT `FK_Laws_LegalAuthority` FOREIGN KEY (`LegalAuthorityId`) REFERENCES `LegalAuthorities` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `Laws`
--

LOCK TABLES `Laws` WRITE;
/*!40000 ALTER TABLE `Laws` DISABLE KEYS */;
/*!40000 ALTER TABLE `Laws` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `Laws_OffenderClasses`
--

DROP TABLE IF EXISTS `Laws_OffenderClasses`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `Laws_OffenderClasses` (
  `LawId` bigint NOT NULL,
  `LegalClassId` bigint NOT NULL,
  PRIMARY KEY (`LawId`,`LegalClassId`),
  KEY `FK_Laws_OffenderClasses_LegalClasses_idx` (`LegalClassId`),
  CONSTRAINT `FK_Laws_OffenderClasses_Laws` FOREIGN KEY (`LawId`) REFERENCES `Laws` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_Laws_OffenderClasses_LegalClasses` FOREIGN KEY (`LegalClassId`) REFERENCES `LegalClasses` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `Laws_OffenderClasses`
--

LOCK TABLES `Laws_OffenderClasses` WRITE;
/*!40000 ALTER TABLE `Laws_OffenderClasses` DISABLE KEYS */;
/*!40000 ALTER TABLE `Laws_OffenderClasses` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `Laws_VictimClasses`
--

DROP TABLE IF EXISTS `Laws_VictimClasses`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `Laws_VictimClasses` (
  `LawId` bigint NOT NULL,
  `LegalClassId` bigint NOT NULL,
  PRIMARY KEY (`LawId`,`LegalClassId`),
  KEY `FK_Laws_VictimClasses_LegalClasses_idx` (`LegalClassId`),
  CONSTRAINT `FK_Laws_VictimClasses_Laws` FOREIGN KEY (`LawId`) REFERENCES `Laws` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_Laws_VictimClasses_LegalClasses` FOREIGN KEY (`LegalClassId`) REFERENCES `LegalClasses` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `Laws_VictimClasses`
--

LOCK TABLES `Laws_VictimClasses` WRITE;
/*!40000 ALTER TABLE `Laws_VictimClasses` DISABLE KEYS */;
/*!40000 ALTER TABLE `Laws_VictimClasses` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `LegalAuthorities`
--

DROP TABLE IF EXISTS `LegalAuthorities`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `LegalAuthorities` (
  `Id` bigint NOT NULL AUTO_INCREMENT,
  `Name` varchar(250) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
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
  CONSTRAINT `FK_LegalAuthorities_BankAccounts_BankAccountId` FOREIGN KEY (`BankAccountId`) REFERENCES `BankAccounts` (`Id`) ON DELETE RESTRICT,
  CONSTRAINT `FK_LegalAuthorities_CourtroomCell` FOREIGN KEY (`CourtLocationId`) REFERENCES `Cells` (`Id`) ON DELETE RESTRICT,
  CONSTRAINT `FK_LegalAuthorities_Currencies` FOREIGN KEY (`CurrencyId`) REFERENCES `Currencies` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_LegalAuthorities_FutureprogsBailCalc` FOREIGN KEY (`BailCalculationProgId`) REFERENCES `FutureProgs` (`Id`) ON DELETE RESTRICT,
  CONSTRAINT `FK_LegalAuthorities_FutureprogsHold` FOREIGN KEY (`OnHoldProgId`) REFERENCES `FutureProgs` (`Id`) ON DELETE RESTRICT,
  CONSTRAINT `FK_LegalAuthorities_FutureprogsImprison` FOREIGN KEY (`OnImprisonProgId`) REFERENCES `FutureProgs` (`Id`) ON DELETE RESTRICT,
  CONSTRAINT `FK_LegalAuthorities_FutureprogsRelease` FOREIGN KEY (`OnReleaseProgId`) REFERENCES `FutureProgs` (`Id`) ON DELETE RESTRICT,
  CONSTRAINT `FK_LegalAuthorities_MarshallingCells` FOREIGN KEY (`MarshallingLocationId`) REFERENCES `Cells` (`Id`) ON DELETE RESTRICT,
  CONSTRAINT `FK_LegalAuthorities_PreparingCells` FOREIGN KEY (`PreparingLocationId`) REFERENCES `Cells` (`Id`) ON DELETE RESTRICT,
  CONSTRAINT `FK_LegalAuthorities_PrisonBelongingsCells` FOREIGN KEY (`PrisonBelongingsLocationId`) REFERENCES `Cells` (`Id`) ON DELETE RESTRICT,
  CONSTRAINT `FK_LegalAuthorities_PrisonCells` FOREIGN KEY (`PrisonLocationId`) REFERENCES `Cells` (`Id`) ON DELETE RESTRICT,
  CONSTRAINT `FK_LegalAuthorities_PrisonJailCells` FOREIGN KEY (`JailLocationId`) REFERENCES `Cells` (`Id`) ON DELETE RESTRICT,
  CONSTRAINT `FK_LegalAuthorities_PrisonReleaseCells` FOREIGN KEY (`PrisonReleaseLocationId`) REFERENCES `Cells` (`Id`) ON DELETE RESTRICT,
  CONSTRAINT `FK_LegalAuthorities_StowingCells` FOREIGN KEY (`EnforcerStowingLocationId`) REFERENCES `Cells` (`Id`) ON DELETE RESTRICT
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `LegalAuthorities`
--

LOCK TABLES `LegalAuthorities` WRITE;
/*!40000 ALTER TABLE `LegalAuthorities` DISABLE KEYS */;
/*!40000 ALTER TABLE `LegalAuthorities` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `LegalAuthorities_Zones`
--

DROP TABLE IF EXISTS `LegalAuthorities_Zones`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `LegalAuthorities_Zones` (
  `ZoneId` bigint NOT NULL,
  `LegalAuthorityId` bigint NOT NULL,
  PRIMARY KEY (`ZoneId`,`LegalAuthorityId`),
  KEY `FK_LegalAuthorities_Zones_LegalAuthorities_idx` (`LegalAuthorityId`),
  CONSTRAINT `FK_LegalAuthorities_Zones_LegalAuthorities` FOREIGN KEY (`LegalAuthorityId`) REFERENCES `LegalAuthorities` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_LegalAuthorities_Zones_Zones` FOREIGN KEY (`ZoneId`) REFERENCES `Zones` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `LegalAuthorities_Zones`
--

LOCK TABLES `LegalAuthorities_Zones` WRITE;
/*!40000 ALTER TABLE `LegalAuthorities_Zones` DISABLE KEYS */;
/*!40000 ALTER TABLE `LegalAuthorities_Zones` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `LegalAuthoritiyCells`
--

DROP TABLE IF EXISTS `LegalAuthoritiyCells`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `LegalAuthoritiyCells` (
  `LegalAuthorityId` bigint NOT NULL,
  `CellId` bigint NOT NULL,
  PRIMARY KEY (`LegalAuthorityId`,`CellId`),
  KEY `FK_LegalAuthoritiesCells_Cells_idx` (`CellId`),
  KEY `FK_LegalAuthoritiesCells_LegalAuthorities_idx` (`LegalAuthorityId`),
  CONSTRAINT `FK_LegalAuthoritiesCells_Cells` FOREIGN KEY (`CellId`) REFERENCES `Cells` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_LegalAuthoritiesCells_LegalAuthorities` FOREIGN KEY (`LegalAuthorityId`) REFERENCES `LegalAuthorities` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `LegalAuthoritiyCells`
--

LOCK TABLES `LegalAuthoritiyCells` WRITE;
/*!40000 ALTER TABLE `LegalAuthoritiyCells` DISABLE KEYS */;
/*!40000 ALTER TABLE `LegalAuthoritiyCells` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `LegalAuthorityFines`
--

DROP TABLE IF EXISTS `LegalAuthorityFines`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `LegalAuthorityFines` (
  `LegalAuthorityId` bigint NOT NULL,
  `CharacterId` bigint NOT NULL,
  `FinesOwned` decimal(58,29) NOT NULL,
  `PaymentRequiredBy` varchar(100) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
  PRIMARY KEY (`LegalAuthorityId`,`CharacterId`),
  KEY `IX_LegalAuthorityFines_CharacterId` (`CharacterId`),
  CONSTRAINT `FK_LegalAuthorityFines_Characters` FOREIGN KEY (`CharacterId`) REFERENCES `Characters` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_LegalAuthorityFines_LegalAuthorities` FOREIGN KEY (`LegalAuthorityId`) REFERENCES `LegalAuthorities` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `LegalAuthorityFines`
--

LOCK TABLES `LegalAuthorityFines` WRITE;
/*!40000 ALTER TABLE `LegalAuthorityFines` DISABLE KEYS */;
/*!40000 ALTER TABLE `LegalAuthorityFines` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `LegalAuthorityJailCells`
--

DROP TABLE IF EXISTS `LegalAuthorityJailCells`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `LegalAuthorityJailCells` (
  `LegalAuthorityId` bigint NOT NULL,
  `CellId` bigint NOT NULL,
  PRIMARY KEY (`LegalAuthorityId`,`CellId`),
  KEY `FK_LegalAuthoritiesCells_Cells_Jail_idx` (`CellId`),
  KEY `FK_LegalAuthoritiesCells_LegalAuthorities_Jail_idx` (`LegalAuthorityId`),
  CONSTRAINT `FK_LegalAuthoritiesCells_Cells_Jail` FOREIGN KEY (`CellId`) REFERENCES `Cells` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_LegalAuthoritiesCells_LegalAuthorities_Jail` FOREIGN KEY (`LegalAuthorityId`) REFERENCES `LegalAuthorities` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `LegalAuthorityJailCells`
--

LOCK TABLES `LegalAuthorityJailCells` WRITE;
/*!40000 ALTER TABLE `LegalAuthorityJailCells` DISABLE KEYS */;
/*!40000 ALTER TABLE `LegalAuthorityJailCells` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `LegalClasses`
--

DROP TABLE IF EXISTS `LegalClasses`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `LegalClasses` (
  `Id` bigint NOT NULL AUTO_INCREMENT,
  `Name` varchar(250) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
  `LegalAuthorityId` bigint NOT NULL,
  `LegalClassPriority` int NOT NULL,
  `MembershipProgId` bigint NOT NULL,
  `CanBeDetainedUntilFinesPaid` bit(1) NOT NULL,
  PRIMARY KEY (`Id`),
  KEY `FK_LegalClasses_LegalAuthorities_idx` (`LegalAuthorityId`),
  KEY `FK_LegalClasses_FutureProgs_idx` (`MembershipProgId`),
  CONSTRAINT `FK_LegalClasses_FutureProgs` FOREIGN KEY (`MembershipProgId`) REFERENCES `FutureProgs` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_LegalClasses_LegalAuthorities` FOREIGN KEY (`LegalAuthorityId`) REFERENCES `LegalAuthorities` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `LegalClasses`
--

LOCK TABLES `LegalClasses` WRITE;
/*!40000 ALTER TABLE `LegalClasses` DISABLE KEYS */;
/*!40000 ALTER TABLE `LegalClasses` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `Limbs`
--

DROP TABLE IF EXISTS `Limbs`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `Limbs` (
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
  CONSTRAINT `FK_Limbs_BodypartProto` FOREIGN KEY (`RootBodypartId`) REFERENCES `BodypartProto` (`Id`) ON DELETE RESTRICT,
  CONSTRAINT `FK_Limbs_BodyProtos` FOREIGN KEY (`RootBodyId`) REFERENCES `BodyProtos` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `Limbs`
--

LOCK TABLES `Limbs` WRITE;
/*!40000 ALTER TABLE `Limbs` DISABLE KEYS */;
/*!40000 ALTER TABLE `Limbs` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `Limbs_BodypartProto`
--

DROP TABLE IF EXISTS `Limbs_BodypartProto`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `Limbs_BodypartProto` (
  `BodypartProtoId` bigint NOT NULL,
  `LimbId` bigint NOT NULL,
  PRIMARY KEY (`BodypartProtoId`,`LimbId`),
  KEY `FK_Limbs_BodypartProto_Limbs_idx` (`LimbId`),
  CONSTRAINT `FK_Limbs_BodypartProto_BodypartProto` FOREIGN KEY (`BodypartProtoId`) REFERENCES `BodypartProto` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_Limbs_BodypartProto_Limbs` FOREIGN KEY (`LimbId`) REFERENCES `Limbs` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `Limbs_BodypartProto`
--

LOCK TABLES `Limbs_BodypartProto` WRITE;
/*!40000 ALTER TABLE `Limbs_BodypartProto` DISABLE KEYS */;
/*!40000 ALTER TABLE `Limbs_BodypartProto` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `Limbs_SpinalParts`
--

DROP TABLE IF EXISTS `Limbs_SpinalParts`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `Limbs_SpinalParts` (
  `LimbId` bigint NOT NULL,
  `BodypartProtoId` bigint NOT NULL,
  PRIMARY KEY (`LimbId`,`BodypartProtoId`),
  KEY `FK_Limbs_SpinalParts_BodypartProtos_idx` (`BodypartProtoId`),
  CONSTRAINT `FK_Limbs_SpinalParts_BodypartProtos` FOREIGN KEY (`BodypartProtoId`) REFERENCES `BodypartProto` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_Limbs_SpinalParts_Limbs` FOREIGN KEY (`LimbId`) REFERENCES `Limbs` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `Limbs_SpinalParts`
--

LOCK TABLES `Limbs_SpinalParts` WRITE;
/*!40000 ALTER TABLE `Limbs_SpinalParts` DISABLE KEYS */;
/*!40000 ALTER TABLE `Limbs_SpinalParts` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `LineOfCreditAccounts`
--

DROP TABLE IF EXISTS `LineOfCreditAccounts`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `LineOfCreditAccounts` (
  `Id` bigint NOT NULL AUTO_INCREMENT,
  `AccountName` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  `ShopId` bigint NOT NULL,
  `IsSuspended` bit(1) NOT NULL,
  `AccountLimit` decimal(58,29) NOT NULL,
  `OutstandingBalance` decimal(58,29) NOT NULL,
  `AccountOwnerId` bigint NOT NULL,
  `AccountOwnerName` text CHARACTER SET utf8 COLLATE utf8_general_ci,
  PRIMARY KEY (`Id`),
  KEY `FK_LineOfCreditAccounts_Characters_idx` (`AccountOwnerId`),
  KEY `FK_LineOfCreditAccounts_Shops_idx` (`ShopId`),
  CONSTRAINT `FK_LineOfCreditAccounts_Characters` FOREIGN KEY (`AccountOwnerId`) REFERENCES `Characters` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_LineOfCreditAccounts_Shops` FOREIGN KEY (`ShopId`) REFERENCES `Shops` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `LineOfCreditAccounts`
--

LOCK TABLES `LineOfCreditAccounts` WRITE;
/*!40000 ALTER TABLE `LineOfCreditAccounts` DISABLE KEYS */;
/*!40000 ALTER TABLE `LineOfCreditAccounts` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `LineOfCreditAccountUsers`
--

DROP TABLE IF EXISTS `LineOfCreditAccountUsers`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `LineOfCreditAccountUsers` (
  `LineOfCreditAccountId` bigint NOT NULL,
  `AccountUserId` bigint NOT NULL,
  `AccountUserName` text CHARACTER SET utf8 COLLATE utf8_general_ci,
  `SpendingLimit` decimal(58,29) DEFAULT NULL,
  PRIMARY KEY (`LineOfCreditAccountId`,`AccountUserId`),
  KEY `FK_LineOfCreditAccountUsers_Characters_idx` (`AccountUserId`),
  KEY `FK_LineOfCreditAccountUsers_LineOfCreditAccounts_idx` (`LineOfCreditAccountId`),
  CONSTRAINT `FK_LineOfCreditAccountUsers_Characters` FOREIGN KEY (`AccountUserId`) REFERENCES `Characters` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_LineOfCreditAccountUsers_LineOfCreditAccounts` FOREIGN KEY (`LineOfCreditAccountId`) REFERENCES `LineOfCreditAccounts` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `LineOfCreditAccountUsers`
--

LOCK TABLES `LineOfCreditAccountUsers` WRITE;
/*!40000 ALTER TABLE `LineOfCreditAccountUsers` DISABLE KEYS */;
/*!40000 ALTER TABLE `LineOfCreditAccountUsers` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `Liquids`
--

DROP TABLE IF EXISTS `Liquids`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `Liquids` (
  `Id` bigint NOT NULL AUTO_INCREMENT,
  `Name` varchar(200) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
  `Description` text CHARACTER SET utf8 COLLATE utf8_general_ci,
  `LongDescription` text CHARACTER SET utf8 COLLATE utf8_general_ci,
  `TasteText` text CHARACTER SET utf8 COLLATE utf8_general_ci,
  `VagueTasteText` text CHARACTER SET utf8 COLLATE utf8_general_ci,
  `SmellText` text CHARACTER SET utf8 COLLATE utf8_general_ci,
  `VagueSmellText` text CHARACTER SET utf8 COLLATE utf8_general_ci,
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
  `IgnitionPoint` double DEFAULT NULL,
  `FreezingPoint` double DEFAULT '273.15',
  `BoilingPoint` double DEFAULT '373.15',
  `DraughtProgId` bigint DEFAULT NULL,
  `SolventId` bigint DEFAULT NULL,
  `CountAsId` bigint DEFAULT NULL,
  `CountAsQuality` int NOT NULL,
  `DisplayColour` varchar(45) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL DEFAULT 'blue',
  `DampDescription` text CHARACTER SET utf8 COLLATE utf8_general_ci,
  `WetDescription` text CHARACTER SET utf8 COLLATE utf8_general_ci,
  `DrenchedDescription` text CHARACTER SET utf8 COLLATE utf8_general_ci,
  `DampShortDescription` text CHARACTER SET utf8 COLLATE utf8_general_ci,
  `WetShortDescription` text CHARACTER SET utf8 COLLATE utf8_general_ci,
  `DrenchedShortDescription` text CHARACTER SET utf8 COLLATE utf8_general_ci,
  `SolventVolumeRatio` double NOT NULL DEFAULT '1',
  `DriedResidueId` bigint DEFAULT NULL,
  `DrugId` bigint DEFAULT NULL,
  `DrugGramsPerUnitVolume` double NOT NULL,
  `InjectionConsequence` int NOT NULL,
  `ResidueVolumePercentage` double NOT NULL DEFAULT '0.05',
  `GasFormId` bigint DEFAULT NULL,
  `RelativeEnthalpy` double NOT NULL DEFAULT '1',
  `LeaveResidueInRooms` tinyint(1) NOT NULL DEFAULT '0',
  PRIMARY KEY (`Id`),
  KEY `FK_Liquids_Liquids_CountasAs_idx` (`CountAsId`),
  KEY `FK_Liquids_Materials_idx` (`DriedResidueId`),
  KEY `FK_Liquids_Drugs_idx` (`DrugId`),
  KEY `FK_Liquids_Liquids_idx` (`SolventId`),
  KEY `IX_Liquids_GasFormId` (`GasFormId`),
  CONSTRAINT `FK_Liquids_Drugs` FOREIGN KEY (`DrugId`) REFERENCES `Drugs` (`Id`) ON DELETE SET NULL,
  CONSTRAINT `FK_Liquids_Gases` FOREIGN KEY (`GasFormId`) REFERENCES `Gases` (`Id`) ON DELETE SET NULL,
  CONSTRAINT `FK_Liquids_Liquids` FOREIGN KEY (`SolventId`) REFERENCES `Liquids` (`Id`) ON DELETE SET NULL,
  CONSTRAINT `FK_Liquids_Liquids_CountasAs` FOREIGN KEY (`CountAsId`) REFERENCES `Liquids` (`Id`) ON DELETE SET NULL,
  CONSTRAINT `FK_Liquids_Materials` FOREIGN KEY (`DriedResidueId`) REFERENCES `Materials` (`Id`) ON DELETE SET NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `Liquids`
--

LOCK TABLES `Liquids` WRITE;
/*!40000 ALTER TABLE `Liquids` DISABLE KEYS */;
/*!40000 ALTER TABLE `Liquids` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `Liquids_Tags`
--

DROP TABLE IF EXISTS `Liquids_Tags`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `Liquids_Tags` (
  `LiquidId` bigint NOT NULL,
  `TagId` bigint NOT NULL,
  PRIMARY KEY (`LiquidId`,`TagId`),
  KEY `FK_Liquids_Tags_Tags_idx` (`TagId`),
  CONSTRAINT `FK_Liquids_Tags_Liquids` FOREIGN KEY (`LiquidId`) REFERENCES `Liquids` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_Liquids_Tags_Tags` FOREIGN KEY (`TagId`) REFERENCES `Tags` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `Liquids_Tags`
--

LOCK TABLES `Liquids_Tags` WRITE;
/*!40000 ALTER TABLE `Liquids_Tags` DISABLE KEYS */;
/*!40000 ALTER TABLE `Liquids_Tags` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `Locks`
--

DROP TABLE IF EXISTS `Locks`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `Locks` (
  `Id` bigint NOT NULL AUTO_INCREMENT,
  `Name` varchar(255) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
  `Style` int NOT NULL,
  `Strength` int NOT NULL,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `Locks`
--

LOCK TABLES `Locks` WRITE;
/*!40000 ALTER TABLE `Locks` DISABLE KEYS */;
/*!40000 ALTER TABLE `Locks` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `LoginIPs`
--

DROP TABLE IF EXISTS `LoginIPs`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `LoginIPs` (
  `IpAddress` varchar(200) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
  `AccountId` bigint NOT NULL,
  `FirstDate` datetime NOT NULL,
  `AccountRegisteredOnThisIP` bit(1) NOT NULL DEFAULT b'0',
  PRIMARY KEY (`IpAddress`,`AccountId`),
  KEY `FK_LoginIPs_Accounts` (`AccountId`),
  CONSTRAINT `FK_LoginIPs_Accounts` FOREIGN KEY (`AccountId`) REFERENCES `Accounts` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `LoginIPs`
--

LOCK TABLES `LoginIPs` WRITE;
/*!40000 ALTER TABLE `LoginIPs` DISABLE KEYS */;
/*!40000 ALTER TABLE `LoginIPs` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `MagicCapabilities`
--

DROP TABLE IF EXISTS `MagicCapabilities`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `MagicCapabilities` (
  `Id` bigint NOT NULL AUTO_INCREMENT,
  `Name` varchar(200) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
  `CapabilityModel` varchar(45) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
  `PowerLevel` int NOT NULL DEFAULT '1',
  `Definition` text CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
  `MagicSchoolId` bigint NOT NULL,
  PRIMARY KEY (`Id`),
  KEY `FK_MagicCapabilities_MagicSchools_idx` (`MagicSchoolId`),
  CONSTRAINT `FK_MagicCapabilities_MagicSchools` FOREIGN KEY (`MagicSchoolId`) REFERENCES `MagicSchools` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `MagicCapabilities`
--

LOCK TABLES `MagicCapabilities` WRITE;
/*!40000 ALTER TABLE `MagicCapabilities` DISABLE KEYS */;
/*!40000 ALTER TABLE `MagicCapabilities` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `MagicGenerators`
--

DROP TABLE IF EXISTS `MagicGenerators`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `MagicGenerators` (
  `Id` bigint NOT NULL AUTO_INCREMENT,
  `Name` varchar(200) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
  `Type` varchar(45) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
  `Definition` text CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `MagicGenerators`
--

LOCK TABLES `MagicGenerators` WRITE;
/*!40000 ALTER TABLE `MagicGenerators` DISABLE KEYS */;
/*!40000 ALTER TABLE `MagicGenerators` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `MagicPowers`
--

DROP TABLE IF EXISTS `MagicPowers`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `MagicPowers` (
  `Id` bigint NOT NULL AUTO_INCREMENT,
  `Name` varchar(200) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
  `Blurb` varchar(500) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
  `ShowHelp` text CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
  `PowerModel` varchar(45) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
  `Definition` text CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
  `MagicSchoolId` bigint NOT NULL,
  PRIMARY KEY (`Id`),
  KEY `FK_MagicPowers_MagicSchools_idx` (`MagicSchoolId`),
  CONSTRAINT `FK_MagicPowers_MagicSchools` FOREIGN KEY (`MagicSchoolId`) REFERENCES `MagicSchools` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `MagicPowers`
--

LOCK TABLES `MagicPowers` WRITE;
/*!40000 ALTER TABLE `MagicPowers` DISABLE KEYS */;
/*!40000 ALTER TABLE `MagicPowers` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `MagicResources`
--

DROP TABLE IF EXISTS `MagicResources`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `MagicResources` (
  `Id` bigint NOT NULL AUTO_INCREMENT,
  `Name` varchar(200) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
  `Type` varchar(45) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
  `Definition` text CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
  `MagicResourceType` int NOT NULL,
  `BottomColour` varchar(100) CHARACTER SET utf8 COLLATE utf8_general_ci DEFAULT '[35m',
  `MidColour` varchar(100) CHARACTER SET utf8 COLLATE utf8_general_ci DEFAULT '[1;35m',
  `ShortName` varchar(200) CHARACTER SET utf8 COLLATE utf8_general_ci DEFAULT NULL,
  `TopColour` varchar(100) CHARACTER SET utf8 COLLATE utf8_general_ci DEFAULT '[0m[38;5;171m',
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `MagicResources`
--

LOCK TABLES `MagicResources` WRITE;
/*!40000 ALTER TABLE `MagicResources` DISABLE KEYS */;
/*!40000 ALTER TABLE `MagicResources` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `MagicSchools`
--

DROP TABLE IF EXISTS `MagicSchools`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `MagicSchools` (
  `Id` bigint NOT NULL AUTO_INCREMENT,
  `Name` varchar(200) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
  `ParentSchoolId` bigint DEFAULT NULL,
  `SchoolVerb` varchar(45) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
  `SchoolAdjective` varchar(45) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
  `PowerListColour` varchar(45) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
  PRIMARY KEY (`Id`),
  KEY `FK_MagicSchools_MagicSchools_idx` (`ParentSchoolId`),
  CONSTRAINT `FK_MagicSchools_MagicSchools` FOREIGN KEY (`ParentSchoolId`) REFERENCES `MagicSchools` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `MagicSchools`
--

LOCK TABLES `MagicSchools` WRITE;
/*!40000 ALTER TABLE `MagicSchools` DISABLE KEYS */;
/*!40000 ALTER TABLE `MagicSchools` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `MagicSpells`
--

DROP TABLE IF EXISTS `MagicSpells`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `MagicSpells` (
  `Id` bigint NOT NULL AUTO_INCREMENT,
  `Name` varchar(100) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
  `Blurb` text CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
  `Description` text CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
  `SpellKnownProgId` bigint NOT NULL,
  `MagicSchoolId` bigint NOT NULL,
  `ExclusiveDelay` double NOT NULL,
  `NonExclusiveDelay` double NOT NULL,
  `Definition` mediumtext CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
  `CastingDifficulty` int NOT NULL DEFAULT '0',
  `CastingTraitDefinitionId` bigint DEFAULT NULL,
  `ResistingDifficulty` int DEFAULT NULL,
  `ResistingTraitDefinitionId` bigint DEFAULT NULL,
  `CastingEmote` text CHARACTER SET utf8 COLLATE utf8_general_ci,
  `CastingEmoteFlags` int NOT NULL DEFAULT '0',
  `EffectDurationExpressionId` bigint DEFAULT NULL,
  `FailCastingEmote` text CHARACTER SET utf8 COLLATE utf8_general_ci,
  `TargetEmote` text CHARACTER SET utf8 COLLATE utf8_general_ci,
  `TargetEmoteFlags` int NOT NULL DEFAULT '0',
  `TargetResistedEmote` text CHARACTER SET utf8 COLLATE utf8_general_ci,
  `MinimumSuccessThreshold` int NOT NULL DEFAULT '4',
  `AppliedEffectsAreExclusive` tinyint(1) NOT NULL DEFAULT '0',
  PRIMARY KEY (`Id`),
  KEY `FK_MagicSpells_Futureprogs_idx` (`SpellKnownProgId`),
  KEY `FK_MagicSpells_MagicSchools_idx` (`MagicSchoolId`),
  KEY `FK_MagicSpells_TraitDefinitions_Casting_idx` (`CastingTraitDefinitionId`),
  KEY `FK_MagicSpells_TraitDefinitions_Resisting_idx` (`ResistingTraitDefinitionId`),
  KEY `FK_MagicSpells_TraitExpressions_idx` (`EffectDurationExpressionId`),
  CONSTRAINT `FK_MagicSpells_Futureprogs` FOREIGN KEY (`SpellKnownProgId`) REFERENCES `FutureProgs` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_MagicSpells_MagicSchools` FOREIGN KEY (`MagicSchoolId`) REFERENCES `MagicSchools` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_MagicSpells_TraitDefinitions_Casting` FOREIGN KEY (`CastingTraitDefinitionId`) REFERENCES `TraitDefinitions` (`Id`) ON DELETE RESTRICT,
  CONSTRAINT `FK_MagicSpells_TraitDefinitions_Resisting` FOREIGN KEY (`ResistingTraitDefinitionId`) REFERENCES `TraitDefinitions` (`Id`) ON DELETE RESTRICT,
  CONSTRAINT `FK_MagicSpells_TraitExpressions` FOREIGN KEY (`EffectDurationExpressionId`) REFERENCES `TraitExpression` (`Id`) ON DELETE RESTRICT
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `MagicSpells`
--

LOCK TABLES `MagicSpells` WRITE;
/*!40000 ALTER TABLE `MagicSpells` DISABLE KEYS */;
/*!40000 ALTER TABLE `MagicSpells` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `MarketCategories`
--

DROP TABLE IF EXISTS `MarketCategories`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `MarketCategories` (
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
-- Dumping data for table `MarketCategories`
--

LOCK TABLES `MarketCategories` WRITE;
/*!40000 ALTER TABLE `MarketCategories` DISABLE KEYS */;
/*!40000 ALTER TABLE `MarketCategories` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `MarketInfluences`
--

DROP TABLE IF EXISTS `MarketInfluences`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `MarketInfluences` (
  `Id` bigint NOT NULL AUTO_INCREMENT,
  `Name` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  `Description` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  `MarketId` bigint NOT NULL,
  `AppliesFrom` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  `AppliesUntil` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  `CharacterKnowsAboutInfluenceProgId` bigint NOT NULL,
  `MarketInfluenceTemplateId` bigint DEFAULT NULL,
  `Impacts` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  PRIMARY KEY (`Id`),
  KEY `IX_MarketInfluences_CharacterKnowsAboutInfluenceProgId` (`CharacterKnowsAboutInfluenceProgId`),
  KEY `IX_MarketInfluences_MarketId` (`MarketId`),
  KEY `IX_MarketInfluences_MarketInfluenceTemplateId` (`MarketInfluenceTemplateId`),
  CONSTRAINT `FK_MarketInfluences_FutureProgs_CharacterKnowsAboutInfluencePro~` FOREIGN KEY (`CharacterKnowsAboutInfluenceProgId`) REFERENCES `FutureProgs` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_MarketInfluences_MarketInfluenceTemplates_MarketInfluenceTem~` FOREIGN KEY (`MarketInfluenceTemplateId`) REFERENCES `MarketInfluenceTemplates` (`Id`),
  CONSTRAINT `FK_MarketInfluences_Markets_MarketId` FOREIGN KEY (`MarketId`) REFERENCES `Markets` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `MarketInfluences`
--

LOCK TABLES `MarketInfluences` WRITE;
/*!40000 ALTER TABLE `MarketInfluences` DISABLE KEYS */;
/*!40000 ALTER TABLE `MarketInfluences` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `MarketInfluenceTemplates`
--

DROP TABLE IF EXISTS `MarketInfluenceTemplates`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `MarketInfluenceTemplates` (
  `Id` bigint NOT NULL AUTO_INCREMENT,
  `Name` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  `Description` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  `CharacterKnowsAboutInfluenceProgId` bigint NOT NULL,
  `Impacts` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  `TemplateSummary` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  PRIMARY KEY (`Id`),
  KEY `IX_MarketInfluenceTemplates_CharacterKnowsAboutInfluenceProgId` (`CharacterKnowsAboutInfluenceProgId`),
  CONSTRAINT `FK_MarketInfluenceTemplates_FutureProgs_CharacterKnowsAboutInfl~` FOREIGN KEY (`CharacterKnowsAboutInfluenceProgId`) REFERENCES `FutureProgs` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `MarketInfluenceTemplates`
--

LOCK TABLES `MarketInfluenceTemplates` WRITE;
/*!40000 ALTER TABLE `MarketInfluenceTemplates` DISABLE KEYS */;
/*!40000 ALTER TABLE `MarketInfluenceTemplates` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `MarketMarketCategory`
--

DROP TABLE IF EXISTS `MarketMarketCategory`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `MarketMarketCategory` (
  `MarketCategoriesId` bigint NOT NULL,
  `MarketsId` bigint NOT NULL,
  PRIMARY KEY (`MarketCategoriesId`,`MarketsId`),
  KEY `IX_MarketMarketCategory_MarketsId` (`MarketsId`),
  CONSTRAINT `FK_MarketMarketCategory_MarketCategories_MarketCategoriesId` FOREIGN KEY (`MarketCategoriesId`) REFERENCES `MarketCategories` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_MarketMarketCategory_Markets_MarketsId` FOREIGN KEY (`MarketsId`) REFERENCES `Markets` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `MarketMarketCategory`
--

LOCK TABLES `MarketMarketCategory` WRITE;
/*!40000 ALTER TABLE `MarketMarketCategory` DISABLE KEYS */;
/*!40000 ALTER TABLE `MarketMarketCategory` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `MarketPopulations`
--

DROP TABLE IF EXISTS `MarketPopulations`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `MarketPopulations` (
  `Id` bigint NOT NULL AUTO_INCREMENT,
  `Name` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  `Description` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  `PopulationScale` int NOT NULL,
  `MarketId` bigint NOT NULL,
  `MarketPopulationNeeds` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  `MarketStressPoints` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  PRIMARY KEY (`Id`),
  KEY `IX_MarketPopulations_MarketId` (`MarketId`),
  CONSTRAINT `FK_MarketPopulations_Markets_MarketId` FOREIGN KEY (`MarketId`) REFERENCES `Markets` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `MarketPopulations`
--

LOCK TABLES `MarketPopulations` WRITE;
/*!40000 ALTER TABLE `MarketPopulations` DISABLE KEYS */;
/*!40000 ALTER TABLE `MarketPopulations` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `Markets`
--

DROP TABLE IF EXISTS `Markets`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `Markets` (
  `Id` bigint NOT NULL AUTO_INCREMENT,
  `Name` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  `Description` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  `EconomicZoneId` bigint NOT NULL,
  `MarketPriceFormula` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  PRIMARY KEY (`Id`),
  KEY `IX_Markets_EconomicZoneId` (`EconomicZoneId`),
  CONSTRAINT `FK_Markets_EconomicZones_EconomicZoneId` FOREIGN KEY (`EconomicZoneId`) REFERENCES `EconomicZones` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `Markets`
--

LOCK TABLES `Markets` WRITE;
/*!40000 ALTER TABLE `Markets` DISABLE KEYS */;
/*!40000 ALTER TABLE `Markets` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `Materials`
--

DROP TABLE IF EXISTS `Materials`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `Materials` (
  `Id` bigint NOT NULL AUTO_INCREMENT,
  `Name` varchar(255) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
  `MaterialDescription` varchar(4000) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
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
  `ResidueSdesc` text CHARACTER SET utf8 COLLATE utf8_general_ci,
  `ResidueDesc` text CHARACTER SET utf8 COLLATE utf8_general_ci,
  `ResidueColour` varchar(45) CHARACTER SET utf8 COLLATE utf8_general_ci DEFAULT 'white',
  `Absorbency` double NOT NULL DEFAULT '0.25',
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `Materials`
--

LOCK TABLES `Materials` WRITE;
/*!40000 ALTER TABLE `Materials` DISABLE KEYS */;
/*!40000 ALTER TABLE `Materials` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `Materials_Tags`
--

DROP TABLE IF EXISTS `Materials_Tags`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `Materials_Tags` (
  `MaterialId` bigint NOT NULL,
  `TagId` bigint NOT NULL,
  PRIMARY KEY (`MaterialId`,`TagId`),
  KEY `Materials_Tags_Tags_idx` (`TagId`),
  CONSTRAINT `Materials_Tags_Materials` FOREIGN KEY (`MaterialId`) REFERENCES `Materials` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `Materials_Tags_Tags` FOREIGN KEY (`TagId`) REFERENCES `Tags` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `Materials_Tags`
--

LOCK TABLES `Materials_Tags` WRITE;
/*!40000 ALTER TABLE `Materials_Tags` DISABLE KEYS */;
/*!40000 ALTER TABLE `Materials_Tags` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `Merchandises`
--

DROP TABLE IF EXISTS `Merchandises`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `Merchandises` (
  `Id` bigint NOT NULL AUTO_INCREMENT,
  `Name` varchar(1000) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
  `ShopId` bigint NOT NULL,
  `AutoReordering` bit(1) NOT NULL,
  `AutoReorderPrice` decimal(58,29) NOT NULL,
  `BasePrice` decimal(58,29) NOT NULL,
  `DefaultMerchandiseForItem` bit(1) NOT NULL,
  `ItemProtoId` bigint NOT NULL,
  `PreferredDisplayContainerId` bigint DEFAULT NULL,
  `ListDescription` varchar(500) CHARACTER SET utf8 COLLATE utf8_general_ci DEFAULT NULL,
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
  PRIMARY KEY (`Id`),
  KEY `FK_Merchandises_GameItems_idx` (`PreferredDisplayContainerId`),
  KEY `FK_Merchandises_Shops_idx` (`ShopId`),
  CONSTRAINT `FK_Merchandises_GameItems` FOREIGN KEY (`PreferredDisplayContainerId`) REFERENCES `GameItems` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_Merchandises_Shops` FOREIGN KEY (`ShopId`) REFERENCES `Shops` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `Merchandises`
--

LOCK TABLES `Merchandises` WRITE;
/*!40000 ALTER TABLE `Merchandises` DISABLE KEYS */;
/*!40000 ALTER TABLE `Merchandises` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `Merits`
--

DROP TABLE IF EXISTS `Merits`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `Merits` (
  `Id` bigint NOT NULL AUTO_INCREMENT,
  `Name` varchar(200) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL,
  `Type` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL,
  `MeritType` int NOT NULL,
  `MeritScope` int NOT NULL,
  `Definition` text CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL,
  `ParentId` bigint DEFAULT NULL,
  PRIMARY KEY (`Id`),
  KEY `FK_Merits_Merits_idx` (`ParentId`),
  CONSTRAINT `FK_Merits_Merits` FOREIGN KEY (`ParentId`) REFERENCES `Merits` (`Id`) ON DELETE SET NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `Merits`
--

LOCK TABLES `Merits` WRITE;
/*!40000 ALTER TABLE `Merits` DISABLE KEYS */;
/*!40000 ALTER TABLE `Merits` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `Merits_ChargenResources`
--

DROP TABLE IF EXISTS `Merits_ChargenResources`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `Merits_ChargenResources` (
  `MeritId` bigint NOT NULL,
  `ChargenResourceId` bigint NOT NULL,
  `RequirementOnly` bit(1) NOT NULL DEFAULT b'0',
  `Amount` int NOT NULL,
  PRIMARY KEY (`MeritId`,`ChargenResourceId`,`RequirementOnly`),
  KEY `FK_Merits_ChargenResources_ChargenResources_idx` (`ChargenResourceId`),
  CONSTRAINT `FK_Merits_ChargenResources_ChargenResources` FOREIGN KEY (`ChargenResourceId`) REFERENCES `ChargenResources` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_Merits_ChargenResources_Merits` FOREIGN KEY (`MeritId`) REFERENCES `Merits` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `Merits_ChargenResources`
--

LOCK TABLES `Merits_ChargenResources` WRITE;
/*!40000 ALTER TABLE `Merits_ChargenResources` DISABLE KEYS */;
/*!40000 ALTER TABLE `Merits_ChargenResources` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `MoveSpeeds`
--

DROP TABLE IF EXISTS `MoveSpeeds`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `MoveSpeeds` (
  `Id` bigint NOT NULL AUTO_INCREMENT,
  `BodyProtoId` bigint NOT NULL,
  `Multiplier` double NOT NULL,
  `Alias` varchar(255) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
  `FirstPersonVerb` varchar(255) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
  `ThirdPersonVerb` varchar(255) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
  `PresentParticiple` varchar(255) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
  `PositionId` bigint NOT NULL,
  `StaminaMultiplier` double NOT NULL DEFAULT '1',
  PRIMARY KEY (`Id`),
  KEY `FK_MoveSpeeds_BodyPrototype` (`BodyProtoId`),
  CONSTRAINT `FK_MoveSpeeds_BodyPrototype` FOREIGN KEY (`BodyProtoId`) REFERENCES `BodyProtos` (`Id`) ON DELETE RESTRICT
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `MoveSpeeds`
--

LOCK TABLES `MoveSpeeds` WRITE;
/*!40000 ALTER TABLE `MoveSpeeds` DISABLE KEYS */;
/*!40000 ALTER TABLE `MoveSpeeds` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `MutualIntelligabilities`
--

DROP TABLE IF EXISTS `MutualIntelligabilities`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `MutualIntelligabilities` (
  `ListenerLanguageId` bigint NOT NULL,
  `TargetLanguageId` bigint NOT NULL,
  `IntelligabilityDifficulty` int NOT NULL,
  PRIMARY KEY (`ListenerLanguageId`,`TargetLanguageId`),
  KEY `FK_Languages_MutualIntelligabilities_Target_idx` (`TargetLanguageId`),
  CONSTRAINT `FK_Languages_MutualIntelligabilities_Listener` FOREIGN KEY (`ListenerLanguageId`) REFERENCES `Languages` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_Languages_MutualIntelligabilities_Target` FOREIGN KEY (`TargetLanguageId`) REFERENCES `Languages` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `MutualIntelligabilities`
--

LOCK TABLES `MutualIntelligabilities` WRITE;
/*!40000 ALTER TABLE `MutualIntelligabilities` DISABLE KEYS */;
/*!40000 ALTER TABLE `MutualIntelligabilities` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `NameCulture`
--

DROP TABLE IF EXISTS `NameCulture`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `NameCulture` (
  `Id` bigint NOT NULL AUTO_INCREMENT,
  `Name` varchar(255) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
  `Definition` text CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `NameCulture`
--

LOCK TABLES `NameCulture` WRITE;
/*!40000 ALTER TABLE `NameCulture` DISABLE KEYS */;
/*!40000 ALTER TABLE `NameCulture` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `NewPlayerHints`
--

DROP TABLE IF EXISTS `NewPlayerHints`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `NewPlayerHints` (
  `Id` bigint NOT NULL AUTO_INCREMENT,
  `Text` text CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
  `FilterProgId` bigint DEFAULT NULL,
  `Priority` int NOT NULL,
  `CanRepeat` bit(1) NOT NULL,
  PRIMARY KEY (`Id`),
  KEY `IX_NewPlayerHints_FilterProgId` (`FilterProgId`),
  CONSTRAINT `FK_NewPlayerHints_FutureProgs` FOREIGN KEY (`FilterProgId`) REFERENCES `FutureProgs` (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `NewPlayerHints`
--

LOCK TABLES `NewPlayerHints` WRITE;
/*!40000 ALTER TABLE `NewPlayerHints` DISABLE KEYS */;
/*!40000 ALTER TABLE `NewPlayerHints` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `NonCardinalExitTemplates`
--

DROP TABLE IF EXISTS `NonCardinalExitTemplates`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `NonCardinalExitTemplates` (
  `Id` bigint NOT NULL AUTO_INCREMENT,
  `Name` varchar(255) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
  `OriginOutboundPreface` varchar(255) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
  `OriginInboundPreface` varchar(255) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
  `DestinationOutboundPreface` varchar(255) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
  `DestinationInboundPreface` varchar(255) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
  `OutboundVerb` varchar(255) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
  `InboundVerb` varchar(255) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `NonCardinalExitTemplates`
--

LOCK TABLES `NonCardinalExitTemplates` WRITE;
/*!40000 ALTER TABLE `NonCardinalExitTemplates` DISABLE KEYS */;
/*!40000 ALTER TABLE `NonCardinalExitTemplates` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `NPCs`
--

DROP TABLE IF EXISTS `NPCs`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `NPCs` (
  `Id` bigint NOT NULL AUTO_INCREMENT,
  `CharacterId` bigint NOT NULL,
  `TemplateId` bigint NOT NULL,
  `TemplateRevnum` int NOT NULL,
  `BodyguardCharacterId` bigint DEFAULT NULL,
  PRIMARY KEY (`Id`),
  KEY `FK_NPCs_Characters_Bodyguard_idx` (`BodyguardCharacterId`),
  KEY `FK_NPCs_Characters` (`CharacterId`),
  KEY `FK_NPCs_NPCTemplates` (`TemplateId`,`TemplateRevnum`),
  CONSTRAINT `FK_NPCs_Characters` FOREIGN KEY (`CharacterId`) REFERENCES `Characters` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_NPCs_Characters_Bodyguard` FOREIGN KEY (`BodyguardCharacterId`) REFERENCES `Characters` (`Id`) ON DELETE SET NULL,
  CONSTRAINT `FK_NPCs_NPCTemplates` FOREIGN KEY (`TemplateId`, `TemplateRevnum`) REFERENCES `NPCTemplates` (`Id`, `RevisionNumber`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `NPCs`
--

LOCK TABLES `NPCs` WRITE;
/*!40000 ALTER TABLE `NPCs` DISABLE KEYS */;
/*!40000 ALTER TABLE `NPCs` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `NPCs_ArtificialIntelligences`
--

DROP TABLE IF EXISTS `NPCs_ArtificialIntelligences`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `NPCs_ArtificialIntelligences` (
  `NPCId` bigint NOT NULL,
  `ArtificialIntelligenceId` bigint NOT NULL,
  PRIMARY KEY (`ArtificialIntelligenceId`,`NPCId`),
  KEY `FK_NPCs_ArtificialIntelligences_NPCs` (`NPCId`),
  CONSTRAINT `FK_NPCs_ArtificialIntelligences_ArtificialIntelligences` FOREIGN KEY (`ArtificialIntelligenceId`) REFERENCES `ArtificialIntelligences` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_NPCs_ArtificialIntelligences_NPCs` FOREIGN KEY (`NPCId`) REFERENCES `NPCs` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `NPCs_ArtificialIntelligences`
--

LOCK TABLES `NPCs_ArtificialIntelligences` WRITE;
/*!40000 ALTER TABLE `NPCs_ArtificialIntelligences` DISABLE KEYS */;
/*!40000 ALTER TABLE `NPCs_ArtificialIntelligences` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `NPCSpawnerCells`
--

DROP TABLE IF EXISTS `NPCSpawnerCells`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `NPCSpawnerCells` (
  `NPCSpawnerId` bigint NOT NULL,
  `CellId` bigint NOT NULL,
  PRIMARY KEY (`NPCSpawnerId`,`CellId`),
  KEY `IX_NPCSpawnerCells_CellId` (`CellId`),
  CONSTRAINT `FK_NPCSpawnerCells_Cell` FOREIGN KEY (`CellId`) REFERENCES `Cells` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_NPCSpawnerCells_NPCSpawner` FOREIGN KEY (`NPCSpawnerId`) REFERENCES `NPCSpawners` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `NPCSpawnerCells`
--

LOCK TABLES `NPCSpawnerCells` WRITE;
/*!40000 ALTER TABLE `NPCSpawnerCells` DISABLE KEYS */;
/*!40000 ALTER TABLE `NPCSpawnerCells` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `NPCSpawners`
--

DROP TABLE IF EXISTS `NPCSpawners`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `NPCSpawners` (
  `Id` bigint NOT NULL AUTO_INCREMENT,
  `Name` varchar(255) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
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
  CONSTRAINT `FK_NPCSpawners_CountsAsProg` FOREIGN KEY (`CountsAsProgId`) REFERENCES `FutureProgs` (`Id`) ON DELETE SET NULL,
  CONSTRAINT `FK_NPCSpawners_IsActiveProg` FOREIGN KEY (`IsActiveProgId`) REFERENCES `FutureProgs` (`Id`) ON DELETE SET NULL,
  CONSTRAINT `FK_NPCSpawners_OnSpawnProg` FOREIGN KEY (`OnSpawnProgId`) REFERENCES `FutureProgs` (`Id`) ON DELETE SET NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `NPCSpawners`
--

LOCK TABLES `NPCSpawners` WRITE;
/*!40000 ALTER TABLE `NPCSpawners` DISABLE KEYS */;
/*!40000 ALTER TABLE `NPCSpawners` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `NPCSpawnerZones`
--

DROP TABLE IF EXISTS `NPCSpawnerZones`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `NPCSpawnerZones` (
  `NPCSpawnerId` bigint NOT NULL,
  `ZoneId` bigint NOT NULL,
  PRIMARY KEY (`NPCSpawnerId`,`ZoneId`),
  KEY `IX_NPCSpawnerZones_ZoneId` (`ZoneId`),
  CONSTRAINT `FK_NPCSpawnerZones_NPCSpawner` FOREIGN KEY (`NPCSpawnerId`) REFERENCES `NPCSpawners` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_NPCSpawnerZones_Zone` FOREIGN KEY (`ZoneId`) REFERENCES `Zones` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `NPCSpawnerZones`
--

LOCK TABLES `NPCSpawnerZones` WRITE;
/*!40000 ALTER TABLE `NPCSpawnerZones` DISABLE KEYS */;
/*!40000 ALTER TABLE `NPCSpawnerZones` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `NPCTemplates`
--

DROP TABLE IF EXISTS `NPCTemplates`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `NPCTemplates` (
  `Id` bigint NOT NULL,
  `RevisionNumber` int NOT NULL,
  `Type` varchar(255) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
  `Name` varchar(255) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
  `Definition` text CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
  `EditableItemId` bigint NOT NULL,
  PRIMARY KEY (`Id`,`RevisionNumber`),
  KEY `FK_NPCTemplates_EditableItems` (`EditableItemId`),
  CONSTRAINT `FK_NPCTemplates_EditableItems` FOREIGN KEY (`EditableItemId`) REFERENCES `EditableItems` (`Id`) ON DELETE RESTRICT
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `NPCTemplates`
--

LOCK TABLES `NPCTemplates` WRITE;
/*!40000 ALTER TABLE `NPCTemplates` DISABLE KEYS */;
/*!40000 ALTER TABLE `NPCTemplates` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `NPCTemplates_ArtificalIntelligences`
--

DROP TABLE IF EXISTS `NPCTemplates_ArtificalIntelligences`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `NPCTemplates_ArtificalIntelligences` (
  `NPCTemplateId` bigint NOT NULL,
  `AIId` bigint NOT NULL,
  `NPCTemplateRevisionNumber` int NOT NULL,
  PRIMARY KEY (`NPCTemplateRevisionNumber`,`NPCTemplateId`,`AIId`),
  KEY `FK_NTAI_ArtificalIntelligences` (`AIId`),
  KEY `FK_NTAI_NPCTemplates` (`NPCTemplateId`,`NPCTemplateRevisionNumber`),
  CONSTRAINT `FK_NTAI_ArtificalIntelligences` FOREIGN KEY (`AIId`) REFERENCES `ArtificialIntelligences` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_NTAI_NPCTemplates` FOREIGN KEY (`NPCTemplateId`, `NPCTemplateRevisionNumber`) REFERENCES `NPCTemplates` (`Id`, `RevisionNumber`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `NPCTemplates_ArtificalIntelligences`
--

LOCK TABLES `NPCTemplates_ArtificalIntelligences` WRITE;
/*!40000 ALTER TABLE `NPCTemplates_ArtificalIntelligences` DISABLE KEYS */;
/*!40000 ALTER TABLE `NPCTemplates_ArtificalIntelligences` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `PatrolMembers`
--

DROP TABLE IF EXISTS `PatrolMembers`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `PatrolMembers` (
  `PatrolId` bigint NOT NULL,
  `CharacterId` bigint NOT NULL,
  PRIMARY KEY (`PatrolId`,`CharacterId`),
  KEY `IX_PatrolMembers_CharacterId` (`CharacterId`),
  CONSTRAINT `FK_PatrolMembers_Characters` FOREIGN KEY (`CharacterId`) REFERENCES `Characters` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_PatrolsMembers_Patrols` FOREIGN KEY (`PatrolId`) REFERENCES `Patrols` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `PatrolMembers`
--

LOCK TABLES `PatrolMembers` WRITE;
/*!40000 ALTER TABLE `PatrolMembers` DISABLE KEYS */;
/*!40000 ALTER TABLE `PatrolMembers` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `PatrolRoutes`
--

DROP TABLE IF EXISTS `PatrolRoutes`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `PatrolRoutes` (
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
  CONSTRAINT `FK_PatrolRoutes_FutureProgs_StartPatrolProgId` FOREIGN KEY (`StartPatrolProgId`) REFERENCES `FutureProgs` (`Id`) ON DELETE RESTRICT,
  CONSTRAINT `FK_PatrolRoutes_LegalAuthorities` FOREIGN KEY (`LegalAuthorityId`) REFERENCES `LegalAuthorities` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `PatrolRoutes`
--

LOCK TABLES `PatrolRoutes` WRITE;
/*!40000 ALTER TABLE `PatrolRoutes` DISABLE KEYS */;
/*!40000 ALTER TABLE `PatrolRoutes` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `PatrolRoutesNodes`
--

DROP TABLE IF EXISTS `PatrolRoutesNodes`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `PatrolRoutesNodes` (
  `PatrolRouteId` bigint NOT NULL,
  `CellId` bigint NOT NULL,
  `Order` int NOT NULL,
  PRIMARY KEY (`PatrolRouteId`,`CellId`),
  KEY `FK_PatrolRoutesNodes_Cells_idx` (`CellId`),
  KEY `FK_PatrolRoutesNodes_PatrolRoutes_idx` (`PatrolRouteId`),
  CONSTRAINT `FK_PatrolRoutesNodes_Cells` FOREIGN KEY (`CellId`) REFERENCES `Cells` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_PatrolRoutesNodes_PatrolRoutes` FOREIGN KEY (`PatrolRouteId`) REFERENCES `PatrolRoutes` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `PatrolRoutesNodes`
--

LOCK TABLES `PatrolRoutesNodes` WRITE;
/*!40000 ALTER TABLE `PatrolRoutesNodes` DISABLE KEYS */;
/*!40000 ALTER TABLE `PatrolRoutesNodes` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `PatrolRoutesNumbers`
--

DROP TABLE IF EXISTS `PatrolRoutesNumbers`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `PatrolRoutesNumbers` (
  `PatrolRouteId` bigint NOT NULL,
  `EnforcementAuthorityId` bigint NOT NULL,
  `NumberRequired` int NOT NULL,
  PRIMARY KEY (`PatrolRouteId`,`EnforcementAuthorityId`),
  KEY `FK_PatrolRoutesNumbers_EnforcementAuthorities_idx` (`EnforcementAuthorityId`),
  KEY `FK_PatrolRoutesNumbers_PatrolRoutes_idx` (`PatrolRouteId`),
  CONSTRAINT `FK_PatrolRoutesNumbers_EnforcementAuthorities` FOREIGN KEY (`EnforcementAuthorityId`) REFERENCES `EnforcementAuthorities` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_PatrolRoutesNumbers_PatrolRoutes` FOREIGN KEY (`PatrolRouteId`) REFERENCES `PatrolRoutes` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `PatrolRoutesNumbers`
--

LOCK TABLES `PatrolRoutesNumbers` WRITE;
/*!40000 ALTER TABLE `PatrolRoutesNumbers` DISABLE KEYS */;
/*!40000 ALTER TABLE `PatrolRoutesNumbers` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `PatrolRoutesTimesOfDay`
--

DROP TABLE IF EXISTS `PatrolRoutesTimesOfDay`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `PatrolRoutesTimesOfDay` (
  `PatrolRouteId` bigint NOT NULL,
  `TimeOfDay` int NOT NULL,
  PRIMARY KEY (`PatrolRouteId`,`TimeOfDay`),
  KEY `FK_PatrolRoutesTimesOfDay_PatrolRoutes_idx` (`PatrolRouteId`),
  CONSTRAINT `FK_PatrolRoutesTimesOfDay_PatrolRoutes` FOREIGN KEY (`PatrolRouteId`) REFERENCES `PatrolRoutes` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `PatrolRoutesTimesOfDay`
--

LOCK TABLES `PatrolRoutesTimesOfDay` WRITE;
/*!40000 ALTER TABLE `PatrolRoutesTimesOfDay` DISABLE KEYS */;
/*!40000 ALTER TABLE `PatrolRoutesTimesOfDay` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `Patrols`
--

DROP TABLE IF EXISTS `Patrols`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `Patrols` (
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
  CONSTRAINT `FK_Patrols_Characters` FOREIGN KEY (`PatrolLeaderId`) REFERENCES `Characters` (`Id`) ON DELETE RESTRICT,
  CONSTRAINT `FK_Patrols_Characters_CharacterId` FOREIGN KEY (`CharacterId`) REFERENCES `Characters` (`Id`) ON DELETE RESTRICT,
  CONSTRAINT `FK_Patrols_LastMajorNode` FOREIGN KEY (`LastMajorNodeId`) REFERENCES `Cells` (`Id`) ON DELETE RESTRICT,
  CONSTRAINT `FK_Patrols_LegalAuthorities` FOREIGN KEY (`LegalAuthorityId`) REFERENCES `LegalAuthorities` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_Patrols_NextMajorNode` FOREIGN KEY (`NextMajorNodeId`) REFERENCES `Cells` (`Id`) ON DELETE RESTRICT,
  CONSTRAINT `FK_Patrols_PatrolRoutes` FOREIGN KEY (`PatrolRouteId`) REFERENCES `PatrolRoutes` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `Patrols`
--

LOCK TABLES `Patrols` WRITE;
/*!40000 ALTER TABLE `Patrols` DISABLE KEYS */;
/*!40000 ALTER TABLE `Patrols` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `Paygrades`
--

DROP TABLE IF EXISTS `Paygrades`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `Paygrades` (
  `Id` bigint NOT NULL AUTO_INCREMENT,
  `Name` varchar(100) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
  `Abbreviation` varchar(100) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
  `CurrencyId` bigint NOT NULL,
  `PayAmount` decimal(58,29) NOT NULL,
  `ClanId` bigint NOT NULL,
  PRIMARY KEY (`Id`),
  KEY `FK_Paygrades_Clans` (`ClanId`),
  KEY `FK_Paygrades_Currencies` (`CurrencyId`),
  CONSTRAINT `FK_Paygrades_Clans` FOREIGN KEY (`ClanId`) REFERENCES `Clans` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_Paygrades_Currencies` FOREIGN KEY (`CurrencyId`) REFERENCES `Currencies` (`Id`) ON DELETE RESTRICT
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `Paygrades`
--

LOCK TABLES `Paygrades` WRITE;
/*!40000 ALTER TABLE `Paygrades` DISABLE KEYS */;
/*!40000 ALTER TABLE `Paygrades` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `PerceiverMerits`
--

DROP TABLE IF EXISTS `PerceiverMerits`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `PerceiverMerits` (
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
  CONSTRAINT ` FK_PerceiverMerits_Merits` FOREIGN KEY (`MeritId`) REFERENCES `Merits` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_PerceiverMerits_Bodies` FOREIGN KEY (`BodyId`) REFERENCES `Bodies` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_PerceiverMerits_Characters` FOREIGN KEY (`CharacterId`) REFERENCES `Characters` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `PerceiverMerits`
--

LOCK TABLES `PerceiverMerits` WRITE;
/*!40000 ALTER TABLE `PerceiverMerits` DISABLE KEYS */;
/*!40000 ALTER TABLE `PerceiverMerits` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `PlayerActivitySnapshots`
--

DROP TABLE IF EXISTS `PlayerActivitySnapshots`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `PlayerActivitySnapshots` (
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
-- Dumping data for table `PlayerActivitySnapshots`
--

LOCK TABLES `PlayerActivitySnapshots` WRITE;
/*!40000 ALTER TABLE `PlayerActivitySnapshots` DISABLE KEYS */;
/*!40000 ALTER TABLE `PlayerActivitySnapshots` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `PopulationBloodModels`
--

DROP TABLE IF EXISTS `PopulationBloodModels`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `PopulationBloodModels` (
  `Id` bigint NOT NULL AUTO_INCREMENT,
  `Name` varchar(200) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `PopulationBloodModels`
--

LOCK TABLES `PopulationBloodModels` WRITE;
/*!40000 ALTER TABLE `PopulationBloodModels` DISABLE KEYS */;
/*!40000 ALTER TABLE `PopulationBloodModels` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `PopulationBloodModels_Bloodtypes`
--

DROP TABLE IF EXISTS `PopulationBloodModels_Bloodtypes`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `PopulationBloodModels_Bloodtypes` (
  `BloodtypeId` bigint NOT NULL,
  `PopulationBloodModelId` bigint NOT NULL,
  `Weight` double NOT NULL DEFAULT '1',
  PRIMARY KEY (`BloodtypeId`,`PopulationBloodModelId`),
  KEY `FK_PopulationBloodModels_Bloodtypes_PopulationBloodModels_idx` (`PopulationBloodModelId`),
  CONSTRAINT `FK_PopulationBloodModels_Bloodtypes_Bloodtypes` FOREIGN KEY (`BloodtypeId`) REFERENCES `Bloodtypes` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_PopulationBloodModels_Bloodtypes_PopulationBloodModels` FOREIGN KEY (`PopulationBloodModelId`) REFERENCES `PopulationBloodModels` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `PopulationBloodModels_Bloodtypes`
--

LOCK TABLES `PopulationBloodModels_Bloodtypes` WRITE;
/*!40000 ALTER TABLE `PopulationBloodModels_Bloodtypes` DISABLE KEYS */;
/*!40000 ALTER TABLE `PopulationBloodModels_Bloodtypes` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `ProgSchedules`
--

DROP TABLE IF EXISTS `ProgSchedules`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `ProgSchedules` (
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
  CONSTRAINT `FK_ProgSchedules_FutureProgs` FOREIGN KEY (`FutureProgId`) REFERENCES `FutureProgs` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `ProgSchedules`
--

LOCK TABLES `ProgSchedules` WRITE;
/*!40000 ALTER TABLE `ProgSchedules` DISABLE KEYS */;
/*!40000 ALTER TABLE `ProgSchedules` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `ProjectActions`
--

DROP TABLE IF EXISTS `ProjectActions`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `ProjectActions` (
  `Id` bigint NOT NULL AUTO_INCREMENT,
  `Name` varchar(200) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
  `Description` varchar(1000) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
  `Type` varchar(45) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
  `SortOrder` int NOT NULL,
  `Definition` text CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
  `ProjectPhaseId` bigint NOT NULL,
  PRIMARY KEY (`Id`),
  KEY `FK_ProjectActions_ProjectPhases_idx` (`ProjectPhaseId`),
  CONSTRAINT `FK_ProjectActions_ProjectPhases` FOREIGN KEY (`ProjectPhaseId`) REFERENCES `ProjectPhases` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `ProjectActions`
--

LOCK TABLES `ProjectActions` WRITE;
/*!40000 ALTER TABLE `ProjectActions` DISABLE KEYS */;
/*!40000 ALTER TABLE `ProjectActions` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `ProjectLabourImpacts`
--

DROP TABLE IF EXISTS `ProjectLabourImpacts`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `ProjectLabourImpacts` (
  `Id` bigint NOT NULL AUTO_INCREMENT,
  `Name` varchar(200) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
  `Description` varchar(1000) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
  `Type` varchar(45) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
  `Definition` text CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
  `ProjectLabourRequirementId` bigint NOT NULL,
  `MinimumHoursForImpactToKickIn` double NOT NULL,
  PRIMARY KEY (`Id`),
  KEY `FK_ProjectLabourImpacts_ProjectLabourRequirements_idx` (`ProjectLabourRequirementId`),
  CONSTRAINT `FK_ProjectLabourImpacts_ProjectLabourRequirements` FOREIGN KEY (`ProjectLabourRequirementId`) REFERENCES `ProjectLabourRequirements` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `ProjectLabourImpacts`
--

LOCK TABLES `ProjectLabourImpacts` WRITE;
/*!40000 ALTER TABLE `ProjectLabourImpacts` DISABLE KEYS */;
/*!40000 ALTER TABLE `ProjectLabourImpacts` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `ProjectLabourRequirements`
--

DROP TABLE IF EXISTS `ProjectLabourRequirements`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `ProjectLabourRequirements` (
  `Id` bigint NOT NULL AUTO_INCREMENT,
  `Type` varchar(45) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
  `Name` varchar(200) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
  `ProjectPhaseId` bigint NOT NULL,
  `Description` text CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
  `TotalProgressRequired` double NOT NULL,
  `MaximumSimultaneousWorkers` int NOT NULL,
  `Definition` text CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
  PRIMARY KEY (`Id`),
  KEY `FK_ProjectLabourRequirements_ProjectPhases_idx` (`ProjectPhaseId`),
  CONSTRAINT `FK_ProjectLabourRequirements_ProjectPhases` FOREIGN KEY (`ProjectPhaseId`) REFERENCES `ProjectPhases` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `ProjectLabourRequirements`
--

LOCK TABLES `ProjectLabourRequirements` WRITE;
/*!40000 ALTER TABLE `ProjectLabourRequirements` DISABLE KEYS */;
/*!40000 ALTER TABLE `ProjectLabourRequirements` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `ProjectMaterialRequirements`
--

DROP TABLE IF EXISTS `ProjectMaterialRequirements`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `ProjectMaterialRequirements` (
  `Id` bigint NOT NULL AUTO_INCREMENT,
  `Type` varchar(45) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
  `ProjectPhaseId` bigint NOT NULL,
  `Name` varchar(200) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
  `Description` text CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
  `Definition` text CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
  `IsMandatoryForProjectCompletion` bit(1) NOT NULL DEFAULT b'1',
  PRIMARY KEY (`Id`),
  KEY `FK_ProjectMaterialRequirements_ProjectPhases_idx` (`ProjectPhaseId`),
  CONSTRAINT `FK_ProjectMaterialRequirements_ProjectPhases` FOREIGN KEY (`ProjectPhaseId`) REFERENCES `ProjectPhases` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `ProjectMaterialRequirements`
--

LOCK TABLES `ProjectMaterialRequirements` WRITE;
/*!40000 ALTER TABLE `ProjectMaterialRequirements` DISABLE KEYS */;
/*!40000 ALTER TABLE `ProjectMaterialRequirements` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `ProjectPhases`
--

DROP TABLE IF EXISTS `ProjectPhases`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `ProjectPhases` (
  `Id` bigint NOT NULL AUTO_INCREMENT,
  `ProjectId` bigint NOT NULL,
  `ProjectRevisionNumber` int NOT NULL,
  `PhaseNumber` int NOT NULL,
  `Description` text CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
  PRIMARY KEY (`Id`),
  KEY `FK_ProjectPhases_Projects_idx` (`ProjectId`,`ProjectRevisionNumber`),
  CONSTRAINT `FK_ProjectPhases_Projects` FOREIGN KEY (`ProjectId`, `ProjectRevisionNumber`) REFERENCES `Projects` (`Id`, `RevisionNumber`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `ProjectPhases`
--

LOCK TABLES `ProjectPhases` WRITE;
/*!40000 ALTER TABLE `ProjectPhases` DISABLE KEYS */;
/*!40000 ALTER TABLE `ProjectPhases` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `Projects`
--

DROP TABLE IF EXISTS `Projects`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `Projects` (
  `Id` bigint NOT NULL,
  `RevisionNumber` int NOT NULL,
  `Type` varchar(45) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
  `Name` varchar(200) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
  `EditableItemId` bigint NOT NULL,
  `Definition` text CHARACTER SET utf8 COLLATE utf8_general_ci,
  `AppearInJobsList` bit(1) NOT NULL DEFAULT b'0',
  PRIMARY KEY (`Id`,`RevisionNumber`),
  KEY `FK_Projects_EditableItems_idx` (`EditableItemId`),
  CONSTRAINT `FK_Projects_EditableItems` FOREIGN KEY (`EditableItemId`) REFERENCES `EditableItems` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `Projects`
--

LOCK TABLES `Projects` WRITE;
/*!40000 ALTER TABLE `Projects` DISABLE KEYS */;
/*!40000 ALTER TABLE `Projects` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `Properties`
--

DROP TABLE IF EXISTS `Properties`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `Properties` (
  `Id` bigint NOT NULL AUTO_INCREMENT,
  `Name` varchar(250) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
  `EconomicZoneId` bigint NOT NULL,
  `DetailedDescription` text CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
  `LastChangeOfOwnership` varchar(100) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
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
  CONSTRAINT `FK_Properties_EconomicZones` FOREIGN KEY (`EconomicZoneId`) REFERENCES `EconomicZones` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_Properties_Lease` FOREIGN KEY (`LeaseId`) REFERENCES `PropertyLeases` (`Id`) ON DELETE RESTRICT,
  CONSTRAINT `FK_Properties_LeaseOrder` FOREIGN KEY (`LeaseOrderId`) REFERENCES `PropertyLeaseOrders` (`Id`) ON DELETE RESTRICT,
  CONSTRAINT `FK_Properties_SaleOrder` FOREIGN KEY (`SaleOrderId`) REFERENCES `PropertySalesOrders` (`Id`) ON DELETE RESTRICT
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `Properties`
--

LOCK TABLES `Properties` WRITE;
/*!40000 ALTER TABLE `Properties` DISABLE KEYS */;
/*!40000 ALTER TABLE `Properties` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `PropertyKeys`
--

DROP TABLE IF EXISTS `PropertyKeys`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `PropertyKeys` (
  `Id` bigint NOT NULL AUTO_INCREMENT,
  `Name` varchar(250) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
  `GameItemId` bigint NOT NULL,
  `PropertyId` bigint NOT NULL,
  `AddedToPropertyOnDate` varchar(100) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
  `CostToReplace` decimal(58,29) NOT NULL,
  `IsReturned` bit(1) NOT NULL,
  PRIMARY KEY (`Id`),
  KEY `IX_PropertyKeys_GameItemId` (`GameItemId`),
  KEY `IX_PropertyKeys_PropertyId` (`PropertyId`),
  CONSTRAINT `FK_PropertyKeys_GameItems` FOREIGN KEY (`GameItemId`) REFERENCES `GameItems` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_PropertyKeys_Property` FOREIGN KEY (`PropertyId`) REFERENCES `Properties` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `PropertyKeys`
--

LOCK TABLES `PropertyKeys` WRITE;
/*!40000 ALTER TABLE `PropertyKeys` DISABLE KEYS */;
/*!40000 ALTER TABLE `PropertyKeys` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `PropertyLeaseOrders`
--

DROP TABLE IF EXISTS `PropertyLeaseOrders`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `PropertyLeaseOrders` (
  `Id` bigint NOT NULL AUTO_INCREMENT,
  `PropertyId` bigint NOT NULL,
  `PricePerInterval` decimal(58,29) NOT NULL,
  `BondRequired` decimal(58,29) NOT NULL,
  `Interval` varchar(100) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
  `CanLeaseProgCharacterId` bigint DEFAULT NULL,
  `CanLeaseProgClanId` bigint DEFAULT NULL,
  `MinimumLeaseDurationDays` double NOT NULL,
  `MaximumLeaseDurationDays` double NOT NULL,
  `AllowAutoRenew` bit(1) NOT NULL,
  `AutomaticallyRelistAfterLeaseTerm` bit(1) NOT NULL,
  `AllowLeaseNovation` bit(1) NOT NULL,
  `RekeyOnLeaseEnd` bit(1) NOT NULL,
  `ListedForLease` bit(1) NOT NULL,
  `FeeIncreasePercentageAfterLeaseTerm` decimal(58,29) NOT NULL,
  `PropertyOwnerConsentInfo` text CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
  PRIMARY KEY (`Id`),
  KEY `IX_PropertyLeaseOrders_CanLeaseProgCharacterId` (`CanLeaseProgCharacterId`),
  KEY `IX_PropertyLeaseOrders_CanLeaseProgClanId` (`CanLeaseProgClanId`),
  KEY `IX_PropertyLeaseOrders_PropertyId` (`PropertyId`),
  CONSTRAINT `FK_PropertyLeaseOrders_FutureProgs_Character` FOREIGN KEY (`CanLeaseProgCharacterId`) REFERENCES `FutureProgs` (`Id`) ON DELETE RESTRICT,
  CONSTRAINT `FK_PropertyLeaseOrders_FutureProgs_Clan` FOREIGN KEY (`CanLeaseProgClanId`) REFERENCES `FutureProgs` (`Id`) ON DELETE RESTRICT,
  CONSTRAINT `FK_PropertyLeaseOrders_Property` FOREIGN KEY (`PropertyId`) REFERENCES `Properties` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `PropertyLeaseOrders`
--

LOCK TABLES `PropertyLeaseOrders` WRITE;
/*!40000 ALTER TABLE `PropertyLeaseOrders` DISABLE KEYS */;
/*!40000 ALTER TABLE `PropertyLeaseOrders` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `PropertyLeases`
--

DROP TABLE IF EXISTS `PropertyLeases`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `PropertyLeases` (
  `Id` bigint NOT NULL AUTO_INCREMENT,
  `PropertyId` bigint NOT NULL,
  `LeaseOrderId` bigint NOT NULL,
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
  `BondClaimed` decimal(65,30) NOT NULL DEFAULT '0.000000000000000000000000000000',
  PRIMARY KEY (`Id`),
  KEY `IX_PropertyLeases_LeaseOrderId` (`LeaseOrderId`),
  KEY `IX_PropertyLeases_PropertyId` (`PropertyId`),
  CONSTRAINT `FK_PropertyLeases_Property` FOREIGN KEY (`PropertyId`) REFERENCES `Properties` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_PropertyLeases_PropertyLeaseOrders` FOREIGN KEY (`LeaseOrderId`) REFERENCES `PropertyLeaseOrders` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `PropertyLeases`
--

LOCK TABLES `PropertyLeases` WRITE;
/*!40000 ALTER TABLE `PropertyLeases` DISABLE KEYS */;
/*!40000 ALTER TABLE `PropertyLeases` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `PropertyLocations`
--

DROP TABLE IF EXISTS `PropertyLocations`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `PropertyLocations` (
  `PropertyId` bigint NOT NULL,
  `CellId` bigint NOT NULL,
  PRIMARY KEY (`PropertyId`,`CellId`),
  KEY `IX_PropertyLocations_CellId` (`CellId`),
  CONSTRAINT `FK_PropertyLocations_Cell` FOREIGN KEY (`CellId`) REFERENCES `Cells` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_PropertyLocations_Property` FOREIGN KEY (`PropertyId`) REFERENCES `Properties` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `PropertyLocations`
--

LOCK TABLES `PropertyLocations` WRITE;
/*!40000 ALTER TABLE `PropertyLocations` DISABLE KEYS */;
/*!40000 ALTER TABLE `PropertyLocations` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `PropertyOwners`
--

DROP TABLE IF EXISTS `PropertyOwners`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `PropertyOwners` (
  `Id` bigint NOT NULL AUTO_INCREMENT,
  `PropertyId` bigint NOT NULL,
  `FrameworkItemId` bigint NOT NULL,
  `FrameworkItemType` varchar(100) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
  `ShareOfOwnership` decimal(58,29) NOT NULL,
  `RevenueAccountId` bigint DEFAULT NULL,
  PRIMARY KEY (`Id`),
  KEY `IX_PropertyOwners_PropertyId` (`PropertyId`),
  KEY `IX_PropertyOwners_RevenueAccountId` (`RevenueAccountId`),
  CONSTRAINT `FK_PropertyOwners_BankAccounts` FOREIGN KEY (`RevenueAccountId`) REFERENCES `BankAccounts` (`Id`) ON DELETE RESTRICT,
  CONSTRAINT `FK_PropertyOwners_Properties` FOREIGN KEY (`PropertyId`) REFERENCES `Properties` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `PropertyOwners`
--

LOCK TABLES `PropertyOwners` WRITE;
/*!40000 ALTER TABLE `PropertyOwners` DISABLE KEYS */;
/*!40000 ALTER TABLE `PropertyOwners` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `PropertySalesOrders`
--

DROP TABLE IF EXISTS `PropertySalesOrders`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `PropertySalesOrders` (
  `Id` bigint NOT NULL AUTO_INCREMENT,
  `PropertyId` bigint NOT NULL,
  `ReservePrice` decimal(58,29) NOT NULL,
  `OrderStatus` int NOT NULL,
  `StartOfListing` varchar(100) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
  `DurationOfListingDays` double NOT NULL,
  `PropertyOwnerConsentInfo` text CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
  PRIMARY KEY (`Id`),
  KEY `IX_PropertySalesOrders_PropertyId` (`PropertyId`),
  CONSTRAINT `FK_PropertySaleOrders_Property` FOREIGN KEY (`PropertyId`) REFERENCES `Properties` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `PropertySalesOrders`
--

LOCK TABLES `PropertySalesOrders` WRITE;
/*!40000 ALTER TABLE `PropertySalesOrders` DISABLE KEYS */;
/*!40000 ALTER TABLE `PropertySalesOrders` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `RaceButcheryProfiles`
--

DROP TABLE IF EXISTS `RaceButcheryProfiles`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `RaceButcheryProfiles` (
  `Id` bigint NOT NULL AUTO_INCREMENT,
  `Name` varchar(100) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
  `Verb` int NOT NULL,
  `RequiredToolTagId` bigint DEFAULT NULL,
  `DifficultySkin` int NOT NULL,
  `CanButcherProgId` bigint DEFAULT NULL,
  `WhyCannotButcherProgId` bigint DEFAULT NULL,
  PRIMARY KEY (`Id`),
  KEY `FK_RaceButcheryProfiles_FutureProgs_Can_idx` (`CanButcherProgId`),
  KEY `FK_RaceButcheryProfiles_Tags_idx` (`RequiredToolTagId`),
  KEY `FK_RaceButcheryProfiles_FutureProgs_Why_idx` (`WhyCannotButcherProgId`),
  CONSTRAINT `FK_RaceButcheryProfiles_FutureProgs_Can` FOREIGN KEY (`CanButcherProgId`) REFERENCES `FutureProgs` (`Id`) ON DELETE SET NULL,
  CONSTRAINT `FK_RaceButcheryProfiles_FutureProgs_Why` FOREIGN KEY (`WhyCannotButcherProgId`) REFERENCES `FutureProgs` (`Id`) ON DELETE SET NULL,
  CONSTRAINT `FK_RaceButcheryProfiles_Tags` FOREIGN KEY (`RequiredToolTagId`) REFERENCES `Tags` (`Id`) ON DELETE SET NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `RaceButcheryProfiles`
--

LOCK TABLES `RaceButcheryProfiles` WRITE;
/*!40000 ALTER TABLE `RaceButcheryProfiles` DISABLE KEYS */;
/*!40000 ALTER TABLE `RaceButcheryProfiles` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `RaceButcheryProfiles_BreakdownChecks`
--

DROP TABLE IF EXISTS `RaceButcheryProfiles_BreakdownChecks`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `RaceButcheryProfiles_BreakdownChecks` (
  `RaceButcheryProfileId` bigint NOT NULL,
  `Subcageory` varchar(100) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
  `TraitDefinitionId` bigint NOT NULL,
  `Difficulty` int NOT NULL,
  PRIMARY KEY (`RaceButcheryProfileId`,`Subcageory`),
  KEY `FK_RaceButcheryProfiles_BreakdownChecks_TraitDefinitions_idx` (`TraitDefinitionId`),
  CONSTRAINT `FK_RaceButcheryProfiles_BreakdownChecks_RaceButcheryProfiles` FOREIGN KEY (`RaceButcheryProfileId`) REFERENCES `RaceButcheryProfiles` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_RaceButcheryProfiles_BreakdownChecks_TraitDefinitions` FOREIGN KEY (`TraitDefinitionId`) REFERENCES `TraitDefinitions` (`Id`) ON DELETE RESTRICT
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `RaceButcheryProfiles_BreakdownChecks`
--

LOCK TABLES `RaceButcheryProfiles_BreakdownChecks` WRITE;
/*!40000 ALTER TABLE `RaceButcheryProfiles_BreakdownChecks` DISABLE KEYS */;
/*!40000 ALTER TABLE `RaceButcheryProfiles_BreakdownChecks` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `RaceButcheryProfiles_BreakdownEmotes`
--

DROP TABLE IF EXISTS `RaceButcheryProfiles_BreakdownEmotes`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `RaceButcheryProfiles_BreakdownEmotes` (
  `RaceButcheryProfileId` bigint NOT NULL,
  `Subcategory` varchar(100) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
  `Order` int NOT NULL,
  `Emote` text CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
  `Delay` double NOT NULL,
  PRIMARY KEY (`RaceButcheryProfileId`,`Subcategory`,`Order`),
  CONSTRAINT `FK_RaceButcheryProfiles_BreakdownEmotes_RaceButcheryProfiles` FOREIGN KEY (`RaceButcheryProfileId`) REFERENCES `RaceButcheryProfiles` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `RaceButcheryProfiles_BreakdownEmotes`
--

LOCK TABLES `RaceButcheryProfiles_BreakdownEmotes` WRITE;
/*!40000 ALTER TABLE `RaceButcheryProfiles_BreakdownEmotes` DISABLE KEYS */;
/*!40000 ALTER TABLE `RaceButcheryProfiles_BreakdownEmotes` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `RaceButcheryProfiles_ButcheryProducts`
--

DROP TABLE IF EXISTS `RaceButcheryProfiles_ButcheryProducts`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `RaceButcheryProfiles_ButcheryProducts` (
  `RaceButcheryProfileId` bigint NOT NULL,
  `ButcheryProductId` bigint NOT NULL,
  PRIMARY KEY (`RaceButcheryProfileId`,`ButcheryProductId`),
  KEY `FK_RaceButcheryProfiles_ButcheryProducts_ButcheryProducts_idx` (`ButcheryProductId`),
  CONSTRAINT `FK_RaceButcheryProfiles_ButcheryProducts_ButcheryProducts` FOREIGN KEY (`ButcheryProductId`) REFERENCES `ButcheryProducts` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_RaceButcheryProfiles_ButcheryProducts_RaceButcheryProfiles` FOREIGN KEY (`RaceButcheryProfileId`) REFERENCES `RaceButcheryProfiles` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `RaceButcheryProfiles_ButcheryProducts`
--

LOCK TABLES `RaceButcheryProfiles_ButcheryProducts` WRITE;
/*!40000 ALTER TABLE `RaceButcheryProfiles_ButcheryProducts` DISABLE KEYS */;
/*!40000 ALTER TABLE `RaceButcheryProfiles_ButcheryProducts` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `RaceButcheryProfiles_SkinningEmotes`
--

DROP TABLE IF EXISTS `RaceButcheryProfiles_SkinningEmotes`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `RaceButcheryProfiles_SkinningEmotes` (
  `RaceButcheryProfileId` bigint NOT NULL,
  `Subcategory` varchar(100) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
  `Order` int NOT NULL,
  `Emote` text CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
  `Delay` double NOT NULL,
  PRIMARY KEY (`RaceButcheryProfileId`,`Subcategory`,`Order`),
  CONSTRAINT `FK_RaceButcheryProfiles_SkinningEmotes_RaceButcheryProfiles` FOREIGN KEY (`RaceButcheryProfileId`) REFERENCES `RaceButcheryProfiles` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `RaceButcheryProfiles_SkinningEmotes`
--

LOCK TABLES `RaceButcheryProfiles_SkinningEmotes` WRITE;
/*!40000 ALTER TABLE `RaceButcheryProfiles_SkinningEmotes` DISABLE KEYS */;
/*!40000 ALTER TABLE `RaceButcheryProfiles_SkinningEmotes` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `RaceEdibleForagableYields`
--

DROP TABLE IF EXISTS `RaceEdibleForagableYields`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `RaceEdibleForagableYields` (
  `RaceId` bigint NOT NULL,
  `YieldType` varchar(50) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
  `BiteYield` double NOT NULL,
  `CaloriesPerYield` double NOT NULL,
  `HungerPerYield` double NOT NULL,
  `WaterPerYield` double NOT NULL,
  `ThirstPerYield` double NOT NULL,
  `AlcoholPerYield` double NOT NULL,
  `EatEmote` varchar(1000) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL DEFAULT '@ eat|eats {{0}} from the location.',
  PRIMARY KEY (`RaceId`,`YieldType`),
  CONSTRAINT `FK_RaceEdibleForagableYields_Races` FOREIGN KEY (`RaceId`) REFERENCES `Races` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `RaceEdibleForagableYields`
--

LOCK TABLES `RaceEdibleForagableYields` WRITE;
/*!40000 ALTER TABLE `RaceEdibleForagableYields` DISABLE KEYS */;
/*!40000 ALTER TABLE `RaceEdibleForagableYields` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `Races`
--

DROP TABLE IF EXISTS `Races`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `Races` (
  `Id` bigint NOT NULL AUTO_INCREMENT,
  `Name` varchar(255) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
  `Description` varchar(4000) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
  `BaseBodyId` bigint NOT NULL,
  `AllowedGenders` varchar(255) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
  `ParentRaceId` bigint DEFAULT NULL,
  `AttributeBonusProgId` bigint NOT NULL,
  `AttributeTotalCap` int NOT NULL,
  `IndividualAttributeCap` int NOT NULL,
  `DiceExpression` varchar(255) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
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
  `CommunicationStrategyType` varchar(45) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL DEFAULT 'humanoid',
  `DefaultHandedness` int NOT NULL DEFAULT '3',
  `HandednessOptions` varchar(100) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL DEFAULT '1 3',
  `MaximumDragWeightExpression` text CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
  `MaximumLiftWeightExpression` text CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
  `RaceButcheryProfileId` bigint DEFAULT NULL,
  `BloodModelId` bigint DEFAULT NULL,
  `RaceUsesStamina` bit(1) NOT NULL DEFAULT b'1',
  `CanEatCorpses` bit(1) NOT NULL DEFAULT b'0',
  `BiteWeight` double NOT NULL DEFAULT '1000',
  `EatCorpseEmoteText` varchar(500) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL DEFAULT '@ eat|eats {{0}}$1',
  `CanEatMaterialsOptIn` bit(1) NOT NULL DEFAULT b'0',
  `TemperatureRangeFloor` double NOT NULL,
  `TemperatureRangeCeiling` double NOT NULL DEFAULT '40',
  `BodypartSizeModifier` int NOT NULL,
  `BodypartHealthMultiplier` double NOT NULL DEFAULT '1',
  `BreathingVolumeExpression` varchar(500) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL DEFAULT '7',
  `HoldBreathLengthExpression` varchar(500) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL DEFAULT '120',
  `CanClimb` bit(1) NOT NULL DEFAULT b'0',
  `CanSwim` bit(1) NOT NULL DEFAULT b'1',
  `MinimumSleepingPosition` int NOT NULL DEFAULT '4',
  `ChildAge` int NOT NULL DEFAULT '3',
  `YouthAge` int NOT NULL DEFAULT '10',
  `YoungAdultAge` int NOT NULL DEFAULT '16',
  `AdultAge` int NOT NULL DEFAULT '21',
  `ElderAge` int NOT NULL DEFAULT '55',
  `VenerableAge` int NOT NULL DEFAULT '75',
  `BreathingModel` varchar(100) CHARACTER SET utf8 COLLATE utf8_general_ci DEFAULT 'simple',
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
  CONSTRAINT `FK_Races_ArmourTypes` FOREIGN KEY (`NaturalArmourTypeId`) REFERENCES `ArmourTypes` (`Id`) ON DELETE SET NULL,
  CONSTRAINT `FK_Races_AttributeBonusProg` FOREIGN KEY (`AttributeBonusProgId`) REFERENCES `FutureProgs` (`Id`) ON DELETE RESTRICT,
  CONSTRAINT `FK_Races_AvailabilityProg` FOREIGN KEY (`AvailabilityProgId`) REFERENCES `FutureProgs` (`Id`) ON DELETE SET NULL,
  CONSTRAINT `FK_Races_BloodModels` FOREIGN KEY (`BloodModelId`) REFERENCES `BloodModels` (`Id`) ON DELETE SET NULL,
  CONSTRAINT `FK_Races_BodyProtos` FOREIGN KEY (`BaseBodyId`) REFERENCES `BodyProtos` (`Id`) ON DELETE RESTRICT,
  CONSTRAINT `FK_Races_CorpseModels` FOREIGN KEY (`CorpseModelId`) REFERENCES `CorpseModels` (`Id`) ON DELETE RESTRICT,
  CONSTRAINT `FK_Races_HealthStrategies` FOREIGN KEY (`DefaultHealthStrategyId`) REFERENCES `HealthStrategies` (`Id`) ON DELETE RESTRICT,
  CONSTRAINT `FK_Races_HeightWeightModelsFemale` FOREIGN KEY (`DefaultHeightWeightModelFemaleId`) REFERENCES `HeightWeightModels` (`Id`),
  CONSTRAINT `FK_Races_HeightWeightModelsMale` FOREIGN KEY (`DefaultHeightWeightModelMaleId`) REFERENCES `HeightWeightModels` (`Id`),
  CONSTRAINT `FK_Races_HeightWeightModelsNeuter` FOREIGN KEY (`DefaultHeightWeightModelNeuterId`) REFERENCES `HeightWeightModels` (`Id`),
  CONSTRAINT `FK_Races_HeightWeightModelsNonBinary` FOREIGN KEY (`DefaultHeightWeightModelNonBinaryId`) REFERENCES `HeightWeightModels` (`Id`),
  CONSTRAINT `FK_Races_Liqiuds_Sweat` FOREIGN KEY (`SweatLiquidId`) REFERENCES `Liquids` (`Id`) ON DELETE SET NULL,
  CONSTRAINT `FK_Races_Liquids_Blood` FOREIGN KEY (`BloodLiquidId`) REFERENCES `Liquids` (`Id`) ON DELETE RESTRICT,
  CONSTRAINT `FK_Races_Materials` FOREIGN KEY (`NaturalArmourMaterialId`) REFERENCES `Materials` (`Id`) ON DELETE SET NULL,
  CONSTRAINT `FK_Races_RaceButcheryProfiles` FOREIGN KEY (`RaceButcheryProfileId`) REFERENCES `RaceButcheryProfiles` (`Id`) ON DELETE SET NULL,
  CONSTRAINT `FK_Races_Races` FOREIGN KEY (`ParentRaceId`) REFERENCES `Races` (`Id`) ON DELETE RESTRICT
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `Races`
--

LOCK TABLES `Races` WRITE;
/*!40000 ALTER TABLE `Races` DISABLE KEYS */;
/*!40000 ALTER TABLE `Races` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `Races_AdditionalBodyparts`
--

DROP TABLE IF EXISTS `Races_AdditionalBodyparts`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `Races_AdditionalBodyparts` (
  `Usage` varchar(50) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
  `BodypartId` bigint NOT NULL,
  `RaceId` bigint NOT NULL,
  PRIMARY KEY (`Usage`,`RaceId`,`BodypartId`),
  KEY `FK_Races_AdditionalBodyparts_BodypartProto` (`BodypartId`),
  KEY `FK_Races_AdditionalBodyparts_Races` (`RaceId`),
  CONSTRAINT `FK_Races_AdditionalBodyparts_BodypartProto` FOREIGN KEY (`BodypartId`) REFERENCES `BodypartProto` (`Id`) ON DELETE RESTRICT,
  CONSTRAINT `FK_Races_AdditionalBodyparts_Races` FOREIGN KEY (`RaceId`) REFERENCES `Races` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `Races_AdditionalBodyparts`
--

LOCK TABLES `Races_AdditionalBodyparts` WRITE;
/*!40000 ALTER TABLE `Races_AdditionalBodyparts` DISABLE KEYS */;
/*!40000 ALTER TABLE `Races_AdditionalBodyparts` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `Races_AdditionalCharacteristics`
--

DROP TABLE IF EXISTS `Races_AdditionalCharacteristics`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `Races_AdditionalCharacteristics` (
  `RaceId` bigint NOT NULL,
  `CharacteristicDefinitionId` bigint NOT NULL,
  `Usage` varchar(255) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
  PRIMARY KEY (`RaceId`,`CharacteristicDefinitionId`),
  KEY `FK_RAC_CharacteristicDefinitions` (`CharacteristicDefinitionId`),
  CONSTRAINT `FK_RAC_CharacteristicDefinitions` FOREIGN KEY (`CharacteristicDefinitionId`) REFERENCES `CharacteristicDefinitions` (`Id`) ON DELETE RESTRICT,
  CONSTRAINT `FK_RAC_Races` FOREIGN KEY (`RaceId`) REFERENCES `Races` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `Races_AdditionalCharacteristics`
--

LOCK TABLES `Races_AdditionalCharacteristics` WRITE;
/*!40000 ALTER TABLE `Races_AdditionalCharacteristics` DISABLE KEYS */;
/*!40000 ALTER TABLE `Races_AdditionalCharacteristics` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `Races_Attributes`
--

DROP TABLE IF EXISTS `Races_Attributes`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `Races_Attributes` (
  `RaceId` bigint NOT NULL,
  `AttributeId` bigint NOT NULL,
  `IsHealthAttribute` bit(1) NOT NULL DEFAULT b'0',
  PRIMARY KEY (`RaceId`,`AttributeId`),
  KEY `FK_Races_Attributes_TraitDefinitions` (`AttributeId`),
  CONSTRAINT `FK_Races_Attributes_Races` FOREIGN KEY (`RaceId`) REFERENCES `Races` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_Races_Attributes_TraitDefinitions` FOREIGN KEY (`AttributeId`) REFERENCES `TraitDefinitions` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `Races_Attributes`
--

LOCK TABLES `Races_Attributes` WRITE;
/*!40000 ALTER TABLE `Races_Attributes` DISABLE KEYS */;
/*!40000 ALTER TABLE `Races_Attributes` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `Races_BreathableGases`
--

DROP TABLE IF EXISTS `Races_BreathableGases`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `Races_BreathableGases` (
  `RaceId` bigint NOT NULL,
  `GasId` bigint NOT NULL,
  `Multiplier` double NOT NULL DEFAULT '1',
  PRIMARY KEY (`RaceId`,`GasId`),
  KEY `FK_Races-BreathableGases_Gases_idx` (`GasId`),
  CONSTRAINT `FK_Races_BreathableGases_Gases` FOREIGN KEY (`GasId`) REFERENCES `Gases` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_Races_BreathableGases_Races` FOREIGN KEY (`RaceId`) REFERENCES `Races` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `Races_BreathableGases`
--

LOCK TABLES `Races_BreathableGases` WRITE;
/*!40000 ALTER TABLE `Races_BreathableGases` DISABLE KEYS */;
/*!40000 ALTER TABLE `Races_BreathableGases` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `Races_BreathableLiquids`
--

DROP TABLE IF EXISTS `Races_BreathableLiquids`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `Races_BreathableLiquids` (
  `RaceId` bigint NOT NULL,
  `LiquidId` bigint NOT NULL,
  `Multiplier` double NOT NULL DEFAULT '1',
  PRIMARY KEY (`RaceId`,`LiquidId`),
  KEY `FK_Races_BreathableLiquids_Liquids_idx` (`LiquidId`),
  CONSTRAINT `FK_Races_BreathableLiquids_Liquids` FOREIGN KEY (`LiquidId`) REFERENCES `Liquids` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_Races_BreathableLiquids_Races` FOREIGN KEY (`RaceId`) REFERENCES `Races` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `Races_BreathableLiquids`
--

LOCK TABLES `Races_BreathableLiquids` WRITE;
/*!40000 ALTER TABLE `Races_BreathableLiquids` DISABLE KEYS */;
/*!40000 ALTER TABLE `Races_BreathableLiquids` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `Races_ChargenResources`
--

DROP TABLE IF EXISTS `Races_ChargenResources`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `Races_ChargenResources` (
  `RaceId` bigint NOT NULL,
  `ChargenResourceId` bigint NOT NULL,
  `RequirementOnly` bit(1) NOT NULL DEFAULT b'0',
  `Amount` int NOT NULL,
  PRIMARY KEY (`RaceId`,`ChargenResourceId`),
  KEY `FK_Races_ChargenResources_ChargenResources` (`ChargenResourceId`),
  CONSTRAINT `FK_Races_ChargenResources_ChargenResources_ChargenResourceId` FOREIGN KEY (`ChargenResourceId`) REFERENCES `ChargenResources` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_Races_ChargenResources_Races_RaceId` FOREIGN KEY (`RaceId`) REFERENCES `Races` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `Races_ChargenResources`
--

LOCK TABLES `Races_ChargenResources` WRITE;
/*!40000 ALTER TABLE `Races_ChargenResources` DISABLE KEYS */;
/*!40000 ALTER TABLE `Races_ChargenResources` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `Races_CombatActions`
--

DROP TABLE IF EXISTS `Races_CombatActions`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `Races_CombatActions` (
  `RaceId` bigint NOT NULL,
  `CombatActionId` bigint NOT NULL,
  PRIMARY KEY (`RaceId`,`CombatActionId`),
  KEY `IX_Races_CombatActions_CombatActionId` (`CombatActionId`),
  CONSTRAINT `FK_Races_CombatActions_CombatActions` FOREIGN KEY (`CombatActionId`) REFERENCES `CombatActions` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_Races_CombatActions_Races` FOREIGN KEY (`RaceId`) REFERENCES `Races` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `Races_CombatActions`
--

LOCK TABLES `Races_CombatActions` WRITE;
/*!40000 ALTER TABLE `Races_CombatActions` DISABLE KEYS */;
/*!40000 ALTER TABLE `Races_CombatActions` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `Races_EdibleMaterials`
--

DROP TABLE IF EXISTS `Races_EdibleMaterials`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `Races_EdibleMaterials` (
  `RaceId` bigint NOT NULL,
  `MaterialId` bigint NOT NULL,
  `CaloriesPerKilogram` double NOT NULL,
  `HungerPerKilogram` double NOT NULL,
  `ThirstPerKilogram` double NOT NULL,
  `WaterPerKilogram` double NOT NULL,
  `AlcoholPerKilogram` double NOT NULL,
  PRIMARY KEY (`RaceId`,`MaterialId`),
  KEY `FK_Races_EdibleMaterials_Materials_idx` (`MaterialId`),
  CONSTRAINT `FK_Races_EdibleMaterials_Materials` FOREIGN KEY (`MaterialId`) REFERENCES `Materials` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_Races_EdibleMaterials_Races` FOREIGN KEY (`RaceId`) REFERENCES `Races` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `Races_EdibleMaterials`
--

LOCK TABLES `Races_EdibleMaterials` WRITE;
/*!40000 ALTER TABLE `Races_EdibleMaterials` DISABLE KEYS */;
/*!40000 ALTER TABLE `Races_EdibleMaterials` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `Races_RemoveBreathableGases`
--

DROP TABLE IF EXISTS `Races_RemoveBreathableGases`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `Races_RemoveBreathableGases` (
  `RaceId` bigint NOT NULL,
  `GasId` bigint NOT NULL,
  PRIMARY KEY (`RaceId`,`GasId`),
  KEY `FK_Races_RemoveBreathableGases_Gases_idx` (`GasId`),
  CONSTRAINT `FK_Races_RemoveBreathableGases_Gases` FOREIGN KEY (`GasId`) REFERENCES `Gases` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_Races_RemoveBreathableGases_Races` FOREIGN KEY (`RaceId`) REFERENCES `Races` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `Races_RemoveBreathableGases`
--

LOCK TABLES `Races_RemoveBreathableGases` WRITE;
/*!40000 ALTER TABLE `Races_RemoveBreathableGases` DISABLE KEYS */;
/*!40000 ALTER TABLE `Races_RemoveBreathableGases` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `Races_RemoveBreathableLiquids`
--

DROP TABLE IF EXISTS `Races_RemoveBreathableLiquids`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `Races_RemoveBreathableLiquids` (
  `RaceId` bigint NOT NULL,
  `LiquidId` bigint NOT NULL,
  PRIMARY KEY (`RaceId`,`LiquidId`),
  KEY `FK_Races_RemoveBreathableLiquids_Liquids_idx` (`LiquidId`),
  CONSTRAINT `FK_Races_RemoveBreathableLiquids_Liquids` FOREIGN KEY (`LiquidId`) REFERENCES `Liquids` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_Races_RemoveBreathableLiquids_Races` FOREIGN KEY (`RaceId`) REFERENCES `Races` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `Races_RemoveBreathableLiquids`
--

LOCK TABLES `Races_RemoveBreathableLiquids` WRITE;
/*!40000 ALTER TABLE `Races_RemoveBreathableLiquids` DISABLE KEYS */;
/*!40000 ALTER TABLE `Races_RemoveBreathableLiquids` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `Races_WeaponAttacks`
--

DROP TABLE IF EXISTS `Races_WeaponAttacks`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `Races_WeaponAttacks` (
  `RaceId` bigint NOT NULL,
  `WeaponAttackId` bigint NOT NULL,
  `BodypartId` bigint NOT NULL,
  `Quality` int NOT NULL,
  PRIMARY KEY (`RaceId`,`WeaponAttackId`,`BodypartId`),
  KEY `FK_Races_WeaponAttacks_BodypartProto_idx` (`BodypartId`),
  KEY `FK_Races_WeaponAttacks_WeaponAttacks_idx` (`WeaponAttackId`),
  CONSTRAINT `FK_Races_WeaponAttacks_BodypartProto` FOREIGN KEY (`BodypartId`) REFERENCES `BodypartProto` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_Races_WeaponAttacks_Races` FOREIGN KEY (`RaceId`) REFERENCES `Races` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_Races_WeaponAttacks_WeaponAttacks` FOREIGN KEY (`WeaponAttackId`) REFERENCES `WeaponAttacks` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `Races_WeaponAttacks`
--

LOCK TABLES `Races_WeaponAttacks` WRITE;
/*!40000 ALTER TABLE `Races_WeaponAttacks` DISABLE KEYS */;
/*!40000 ALTER TABLE `Races_WeaponAttacks` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `RandomNameProfiles`
--

DROP TABLE IF EXISTS `RandomNameProfiles`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `RandomNameProfiles` (
  `Id` bigint NOT NULL AUTO_INCREMENT,
  `Gender` int NOT NULL,
  `NameCultureId` bigint NOT NULL,
  `Name` varchar(255) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
  `UseForChargenSuggestionsProgId` bigint DEFAULT NULL,
  PRIMARY KEY (`Id`),
  KEY `FK_RandomNameProfiles_NameCulture` (`NameCultureId`),
  KEY `IX_RandomNameProfiles_UseForChargenSuggestionsProgId` (`UseForChargenSuggestionsProgId`),
  CONSTRAINT `FK_RandomNameProfiles_FutureProgs_UseForChargenSuggestionsProgId` FOREIGN KEY (`UseForChargenSuggestionsProgId`) REFERENCES `FutureProgs` (`Id`),
  CONSTRAINT `FK_RandomNameProfiles_NameCulture` FOREIGN KEY (`NameCultureId`) REFERENCES `NameCulture` (`Id`) ON DELETE RESTRICT
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `RandomNameProfiles`
--

LOCK TABLES `RandomNameProfiles` WRITE;
/*!40000 ALTER TABLE `RandomNameProfiles` DISABLE KEYS */;
/*!40000 ALTER TABLE `RandomNameProfiles` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `RandomNameProfiles_DiceExpressions`
--

DROP TABLE IF EXISTS `RandomNameProfiles_DiceExpressions`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `RandomNameProfiles_DiceExpressions` (
  `RandomNameProfileId` bigint NOT NULL,
  `NameUsage` int NOT NULL,
  `DiceExpression` varchar(255) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
  PRIMARY KEY (`RandomNameProfileId`,`NameUsage`),
  CONSTRAINT `FK_RandomNameProfiles_DiceExpressions_RandomNameProfiles` FOREIGN KEY (`RandomNameProfileId`) REFERENCES `RandomNameProfiles` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `RandomNameProfiles_DiceExpressions`
--

LOCK TABLES `RandomNameProfiles_DiceExpressions` WRITE;
/*!40000 ALTER TABLE `RandomNameProfiles_DiceExpressions` DISABLE KEYS */;
/*!40000 ALTER TABLE `RandomNameProfiles_DiceExpressions` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `RandomNameProfiles_Elements`
--

DROP TABLE IF EXISTS `RandomNameProfiles_Elements`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `RandomNameProfiles_Elements` (
  `RandomNameProfileId` bigint NOT NULL,
  `NameUsage` int NOT NULL,
  `Name` varchar(100) CHARACTER SET utf8 COLLATE utf8_bin NOT NULL,
  `Weighting` int NOT NULL,
  PRIMARY KEY (`RandomNameProfileId`,`NameUsage`,`Name`),
  CONSTRAINT `FK_RandomNameProfiles_Elements_RandomNameProfiles` FOREIGN KEY (`RandomNameProfileId`) REFERENCES `RandomNameProfiles` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `RandomNameProfiles_Elements`
--

LOCK TABLES `RandomNameProfiles_Elements` WRITE;
/*!40000 ALTER TABLE `RandomNameProfiles_Elements` DISABLE KEYS */;
/*!40000 ALTER TABLE `RandomNameProfiles_Elements` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `RangedCovers`
--

DROP TABLE IF EXISTS `RangedCovers`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `RangedCovers` (
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
-- Dumping data for table `RangedCovers`
--

LOCK TABLES `RangedCovers` WRITE;
/*!40000 ALTER TABLE `RangedCovers` DISABLE KEYS */;
/*!40000 ALTER TABLE `RangedCovers` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `RangedWeaponTypes`
--

DROP TABLE IF EXISTS `RangedWeaponTypes`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `RangedWeaponTypes` (
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
  CONSTRAINT `FK_RangedWeaponTypes_TraitDefinitions_Fire` FOREIGN KEY (`FireTraitId`) REFERENCES `TraitDefinitions` (`Id`) ON DELETE RESTRICT,
  CONSTRAINT `FK_RangedWeaponTypes_TraitDefinitions_Operate` FOREIGN KEY (`OperateTraitId`) REFERENCES `TraitDefinitions` (`Id`) ON DELETE RESTRICT
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `RangedWeaponTypes`
--

LOCK TABLES `RangedWeaponTypes` WRITE;
/*!40000 ALTER TABLE `RangedWeaponTypes` DISABLE KEYS */;
/*!40000 ALTER TABLE `RangedWeaponTypes` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `Ranks`
--

DROP TABLE IF EXISTS `Ranks`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `Ranks` (
  `Id` bigint NOT NULL AUTO_INCREMENT,
  `Name` varchar(100) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
  `InsigniaGameItemId` bigint DEFAULT NULL,
  `InsigniaGameItemRevnum` int DEFAULT NULL,
  `ClanId` bigint NOT NULL,
  `Privileges` bigint NOT NULL,
  `RankPath` varchar(50) CHARACTER SET utf8 COLLATE utf8_general_ci DEFAULT NULL,
  `RankNumber` int NOT NULL,
  `FameType` int NOT NULL DEFAULT '0',
  PRIMARY KEY (`Id`),
  KEY `FK_Ranks_Clans` (`ClanId`),
  KEY `FK_Ranks_GameItemProtos` (`InsigniaGameItemId`,`InsigniaGameItemRevnum`),
  CONSTRAINT `FK_Ranks_Clans` FOREIGN KEY (`ClanId`) REFERENCES `Clans` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_Ranks_GameItemProtos` FOREIGN KEY (`InsigniaGameItemId`, `InsigniaGameItemRevnum`) REFERENCES `GameItemProtos` (`Id`, `RevisionNumber`) ON DELETE RESTRICT
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `Ranks`
--

LOCK TABLES `Ranks` WRITE;
/*!40000 ALTER TABLE `Ranks` DISABLE KEYS */;
/*!40000 ALTER TABLE `Ranks` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `Ranks_Abbreviations`
--

DROP TABLE IF EXISTS `Ranks_Abbreviations`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `Ranks_Abbreviations` (
  `RankId` bigint NOT NULL,
  `Abbreviation` varchar(50) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
  `FutureProgId` bigint DEFAULT NULL,
  `Order` int NOT NULL,
  PRIMARY KEY (`RankId`,`Abbreviation`),
  KEY `FK_Ranks_Abbreviations_FutureProgs` (`FutureProgId`),
  CONSTRAINT `FK_Ranks_Abbreviations_FutureProgs` FOREIGN KEY (`FutureProgId`) REFERENCES `FutureProgs` (`Id`) ON DELETE SET NULL,
  CONSTRAINT `FK_Ranks_Abbreviations_Ranks` FOREIGN KEY (`RankId`) REFERENCES `Ranks` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `Ranks_Abbreviations`
--

LOCK TABLES `Ranks_Abbreviations` WRITE;
/*!40000 ALTER TABLE `Ranks_Abbreviations` DISABLE KEYS */;
/*!40000 ALTER TABLE `Ranks_Abbreviations` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `Ranks_Paygrades`
--

DROP TABLE IF EXISTS `Ranks_Paygrades`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `Ranks_Paygrades` (
  `RankId` bigint NOT NULL,
  `PaygradeId` bigint NOT NULL,
  `Order` int NOT NULL,
  PRIMARY KEY (`RankId`,`PaygradeId`),
  KEY `FK_Ranks_Paygrades_Paygrades` (`PaygradeId`),
  CONSTRAINT `FK_Ranks_Paygrades_Paygrades` FOREIGN KEY (`PaygradeId`) REFERENCES `Paygrades` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_Ranks_Paygrades_Ranks` FOREIGN KEY (`RankId`) REFERENCES `Ranks` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `Ranks_Paygrades`
--

LOCK TABLES `Ranks_Paygrades` WRITE;
/*!40000 ALTER TABLE `Ranks_Paygrades` DISABLE KEYS */;
/*!40000 ALTER TABLE `Ranks_Paygrades` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `Ranks_Titles`
--

DROP TABLE IF EXISTS `Ranks_Titles`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `Ranks_Titles` (
  `RankId` bigint NOT NULL,
  `Title` varchar(50) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
  `FutureProgId` bigint DEFAULT NULL,
  `Order` int NOT NULL,
  PRIMARY KEY (`RankId`,`Title`),
  KEY `FK_Ranks_Titles_FutureProgs` (`FutureProgId`),
  CONSTRAINT `FK_Ranks_Titles_FutureProgs` FOREIGN KEY (`FutureProgId`) REFERENCES `FutureProgs` (`Id`) ON DELETE SET NULL,
  CONSTRAINT `FK_Ranks_Titles_Ranks` FOREIGN KEY (`RankId`) REFERENCES `Ranks` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `Ranks_Titles`
--

LOCK TABLES `Ranks_Titles` WRITE;
/*!40000 ALTER TABLE `Ranks_Titles` DISABLE KEYS */;
/*!40000 ALTER TABLE `Ranks_Titles` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `RegionalClimates`
--

DROP TABLE IF EXISTS `RegionalClimates`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `RegionalClimates` (
  `Id` bigint NOT NULL AUTO_INCREMENT,
  `Name` varchar(200) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
  `ClimateModelId` bigint NOT NULL,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `RegionalClimates`
--

LOCK TABLES `RegionalClimates` WRITE;
/*!40000 ALTER TABLE `RegionalClimates` DISABLE KEYS */;
/*!40000 ALTER TABLE `RegionalClimates` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `RegionalClimates_Seasons`
--

DROP TABLE IF EXISTS `RegionalClimates_Seasons`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `RegionalClimates_Seasons` (
  `RegionalClimateId` bigint NOT NULL,
  `SeasonId` bigint NOT NULL,
  `TemperatureInfo` text CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
  PRIMARY KEY (`RegionalClimateId`,`SeasonId`),
  KEY `FK_RegionalClimates_Seasons_Seasons_idx` (`SeasonId`),
  CONSTRAINT `FK_RegionalClimates_Seasons_RegionalClimates` FOREIGN KEY (`RegionalClimateId`) REFERENCES `RegionalClimates` (`Id`) ON DELETE RESTRICT,
  CONSTRAINT `FK_RegionalClimates_Seasons_Seasons` FOREIGN KEY (`SeasonId`) REFERENCES `Seasons` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `RegionalClimates_Seasons`
--

LOCK TABLES `RegionalClimates_Seasons` WRITE;
/*!40000 ALTER TABLE `RegionalClimates_Seasons` DISABLE KEYS */;
/*!40000 ALTER TABLE `RegionalClimates_Seasons` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `Rooms`
--

DROP TABLE IF EXISTS `Rooms`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `Rooms` (
  `Id` bigint NOT NULL AUTO_INCREMENT,
  `ZoneId` bigint NOT NULL,
  `X` int NOT NULL,
  `Y` int NOT NULL,
  `Z` int NOT NULL,
  PRIMARY KEY (`Id`),
  KEY `FK_Rooms_Zones` (`ZoneId`),
  CONSTRAINT `FK_Rooms_Zones` FOREIGN KEY (`ZoneId`) REFERENCES `Zones` (`Id`) ON DELETE RESTRICT
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `Rooms`
--

LOCK TABLES `Rooms` WRITE;
/*!40000 ALTER TABLE `Rooms` DISABLE KEYS */;
/*!40000 ALTER TABLE `Rooms` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `ScriptedEventFreeTextQuestions`
--

DROP TABLE IF EXISTS `ScriptedEventFreeTextQuestions`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `ScriptedEventFreeTextQuestions` (
  `Id` bigint NOT NULL AUTO_INCREMENT,
  `ScriptedEventId` bigint NOT NULL,
  `Question` mediumtext CHARACTER SET utf8 COLLATE utf8_general_ci,
  `Answer` mediumtext CHARACTER SET utf8 COLLATE utf8_general_ci,
  PRIMARY KEY (`Id`),
  KEY `IX_ScriptedEventFreeTextQuestions_ScriptedEventId` (`ScriptedEventId`),
  CONSTRAINT `FK_ScriptedEventFreeTextQuestions_ScriptedEvents` FOREIGN KEY (`ScriptedEventId`) REFERENCES `ScriptedEvents` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `ScriptedEventFreeTextQuestions`
--

LOCK TABLES `ScriptedEventFreeTextQuestions` WRITE;
/*!40000 ALTER TABLE `ScriptedEventFreeTextQuestions` DISABLE KEYS */;
/*!40000 ALTER TABLE `ScriptedEventFreeTextQuestions` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `ScriptedEventMultipleChoiceQuestionAnswers`
--

DROP TABLE IF EXISTS `ScriptedEventMultipleChoiceQuestionAnswers`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `ScriptedEventMultipleChoiceQuestionAnswers` (
  `Id` bigint NOT NULL AUTO_INCREMENT,
  `ScriptedEventMultipleChoiceQuestionId` bigint NOT NULL,
  `DescriptionBeforeChoice` mediumtext CHARACTER SET utf8 COLLATE utf8_general_ci,
  `DescriptionAfterChoice` mediumtext CHARACTER SET utf8 COLLATE utf8_general_ci,
  `AnswerFilterProgId` bigint DEFAULT NULL,
  `AfterChoiceProgId` bigint DEFAULT NULL,
  PRIMARY KEY (`Id`),
  KEY `IX_ScriptedEventMultipleChoiceQuestionAnswers_AfterChoiceProgId` (`AfterChoiceProgId`),
  KEY `IX_ScriptedEventMultipleChoiceQuestionAnswers_AnswerFilterProgId` (`AnswerFilterProgId`),
  KEY `IX_ScriptedEventMultipleChoiceQuestionAnswers_ScriptedEventMult~` (`ScriptedEventMultipleChoiceQuestionId`),
  CONSTRAINT `FK_ScriptedEventMultipleChoiceQuestionAnswers_FutureProgs_After` FOREIGN KEY (`AfterChoiceProgId`) REFERENCES `FutureProgs` (`Id`),
  CONSTRAINT `FK_ScriptedEventMultipleChoiceQuestionAnswers_FutureProgs_Filter` FOREIGN KEY (`AnswerFilterProgId`) REFERENCES `FutureProgs` (`Id`),
  CONSTRAINT `FK_ScriptedEventMultipleChoiceQuestionAnswers_ScriptedEventMulti` FOREIGN KEY (`ScriptedEventMultipleChoiceQuestionId`) REFERENCES `ScriptedEventMultipleChoiceQuestions` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `ScriptedEventMultipleChoiceQuestionAnswers`
--

LOCK TABLES `ScriptedEventMultipleChoiceQuestionAnswers` WRITE;
/*!40000 ALTER TABLE `ScriptedEventMultipleChoiceQuestionAnswers` DISABLE KEYS */;
/*!40000 ALTER TABLE `ScriptedEventMultipleChoiceQuestionAnswers` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `ScriptedEventMultipleChoiceQuestions`
--

DROP TABLE IF EXISTS `ScriptedEventMultipleChoiceQuestions`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `ScriptedEventMultipleChoiceQuestions` (
  `Id` bigint NOT NULL AUTO_INCREMENT,
  `ScriptedEventId` bigint NOT NULL,
  `Question` mediumtext CHARACTER SET utf8 COLLATE utf8_general_ci,
  `ChosenAnswerId` bigint DEFAULT NULL,
  PRIMARY KEY (`Id`),
  KEY `IX_ScriptedEventMultipleChoiceQuestions_ChosenAnswerId` (`ChosenAnswerId`),
  KEY `IX_ScriptedEventMultipleChoiceQuestions_ScriptedEventId` (`ScriptedEventId`),
  CONSTRAINT `FK_ScriptedEventMultipleChoiceQuestions_ScriptedEventMultipleCho` FOREIGN KEY (`ChosenAnswerId`) REFERENCES `ScriptedEventMultipleChoiceQuestionAnswers` (`Id`),
  CONSTRAINT `FK_ScriptedEventMultipleChoiceQuestions_ScriptedEvents` FOREIGN KEY (`ScriptedEventId`) REFERENCES `ScriptedEvents` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `ScriptedEventMultipleChoiceQuestions`
--

LOCK TABLES `ScriptedEventMultipleChoiceQuestions` WRITE;
/*!40000 ALTER TABLE `ScriptedEventMultipleChoiceQuestions` DISABLE KEYS */;
/*!40000 ALTER TABLE `ScriptedEventMultipleChoiceQuestions` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `ScriptedEvents`
--

DROP TABLE IF EXISTS `ScriptedEvents`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `ScriptedEvents` (
  `Id` bigint NOT NULL AUTO_INCREMENT,
  `Name` mediumtext CHARACTER SET utf8 COLLATE utf8_general_ci,
  `CharacterId` bigint DEFAULT NULL,
  `CharacterFilterProgId` bigint DEFAULT NULL,
  `IsReady` bit(1) NOT NULL,
  `EarliestDate` datetime NOT NULL,
  `IsFinished` bit(1) NOT NULL,
  `IsTemplate` bit(1) NOT NULL,
  PRIMARY KEY (`Id`),
  KEY `IX_ScriptedEvents_CharacterFilterProgId` (`CharacterFilterProgId`),
  KEY `IX_ScriptedEvents_CharacterId` (`CharacterId`),
  CONSTRAINT `FK_ScriptedEvents_Characters` FOREIGN KEY (`CharacterId`) REFERENCES `Characters` (`Id`),
  CONSTRAINT `FK_ScriptedEvents_FutureProgs` FOREIGN KEY (`CharacterFilterProgId`) REFERENCES `FutureProgs` (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `ScriptedEvents`
--

LOCK TABLES `ScriptedEvents` WRITE;
/*!40000 ALTER TABLE `ScriptedEvents` DISABLE KEYS */;
/*!40000 ALTER TABLE `ScriptedEvents` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `Scripts`
--

DROP TABLE IF EXISTS `Scripts`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `Scripts` (
  `Id` bigint NOT NULL AUTO_INCREMENT,
  `Name` text CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
  `KnownScriptDescription` text CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
  `UnknownScriptDescription` text CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
  `KnowledgeId` bigint NOT NULL,
  `DocumentLengthModifier` double NOT NULL DEFAULT '1',
  `InkUseModifier` double NOT NULL DEFAULT '1',
  PRIMARY KEY (`Id`),
  KEY `FK_Scripts_Knowledges_idx` (`KnowledgeId`),
  CONSTRAINT `FK_Scripts_Knowledges` FOREIGN KEY (`KnowledgeId`) REFERENCES `knowledges` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `Scripts`
--

LOCK TABLES `Scripts` WRITE;
/*!40000 ALTER TABLE `Scripts` DISABLE KEYS */;
/*!40000 ALTER TABLE `Scripts` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `Scripts_DesignedLanguages`
--

DROP TABLE IF EXISTS `Scripts_DesignedLanguages`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `Scripts_DesignedLanguages` (
  `ScriptId` bigint NOT NULL,
  `LanguageId` bigint NOT NULL,
  PRIMARY KEY (`ScriptId`,`LanguageId`),
  KEY `FK_Scripts_DesignedLanguages_Languages_idx` (`LanguageId`),
  CONSTRAINT `FK_Scripts_DesignedLanguages_Languages` FOREIGN KEY (`LanguageId`) REFERENCES `Languages` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_Scripts_DesignedLanguages_Scripts` FOREIGN KEY (`ScriptId`) REFERENCES `Scripts` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `Scripts_DesignedLanguages`
--

LOCK TABLES `Scripts_DesignedLanguages` WRITE;
/*!40000 ALTER TABLE `Scripts_DesignedLanguages` DISABLE KEYS */;
/*!40000 ALTER TABLE `Scripts_DesignedLanguages` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `Seasons`
--

DROP TABLE IF EXISTS `Seasons`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `Seasons` (
  `Id` bigint NOT NULL AUTO_INCREMENT,
  `Name` varchar(200) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
  `CelestialDayOnset` int NOT NULL,
  `CelestialId` bigint NOT NULL,
  `DisplayName` varchar(200) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL DEFAULT '',
  `SeasonGroup` varchar(200) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL DEFAULT '',
  PRIMARY KEY (`Id`),
  KEY `FK_Seasons_Celestials_idx` (`CelestialId`),
  CONSTRAINT `FK_Seasons_Celestials` FOREIGN KEY (`CelestialId`) REFERENCES `Celestials` (`Id`) ON DELETE RESTRICT
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `Seasons`
--

LOCK TABLES `Seasons` WRITE;
/*!40000 ALTER TABLE `Seasons` DISABLE KEYS */;
/*!40000 ALTER TABLE `Seasons` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `SeederChoices`
--

DROP TABLE IF EXISTS `SeederChoices`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `SeederChoices` (
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
-- Dumping data for table `SeederChoices`
--

LOCK TABLES `SeederChoices` WRITE;
/*!40000 ALTER TABLE `SeederChoices` DISABLE KEYS */;
/*!40000 ALTER TABLE `SeederChoices` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `Shards`
--

DROP TABLE IF EXISTS `Shards`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `Shards` (
  `Id` bigint NOT NULL AUTO_INCREMENT,
  `Name` varchar(255) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
  `MinimumTerrestrialLux` double NOT NULL,
  `SkyDescriptionTemplateId` bigint NOT NULL,
  `SphericalRadiusMetres` double NOT NULL DEFAULT '6371000',
  PRIMARY KEY (`Id`),
  KEY `FK_Shards_SkyDescriptionTemplates` (`SkyDescriptionTemplateId`),
  CONSTRAINT `FK_Shards_SkyDescriptionTemplates` FOREIGN KEY (`SkyDescriptionTemplateId`) REFERENCES `SkyDescriptionTemplates` (`Id`) ON DELETE RESTRICT
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `Shards`
--

LOCK TABLES `Shards` WRITE;
/*!40000 ALTER TABLE `Shards` DISABLE KEYS */;
/*!40000 ALTER TABLE `Shards` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `Shards_Calendars`
--

DROP TABLE IF EXISTS `Shards_Calendars`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `Shards_Calendars` (
  `ShardId` bigint NOT NULL,
  `CalendarId` bigint NOT NULL,
  PRIMARY KEY (`ShardId`,`CalendarId`),
  CONSTRAINT `FK_Shards_Calendars_Shards` FOREIGN KEY (`ShardId`) REFERENCES `Shards` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `Shards_Calendars`
--

LOCK TABLES `Shards_Calendars` WRITE;
/*!40000 ALTER TABLE `Shards_Calendars` DISABLE KEYS */;
/*!40000 ALTER TABLE `Shards_Calendars` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `Shards_Celestials`
--

DROP TABLE IF EXISTS `Shards_Celestials`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `Shards_Celestials` (
  `ShardId` bigint NOT NULL,
  `CelestialId` bigint NOT NULL,
  PRIMARY KEY (`ShardId`,`CelestialId`),
  CONSTRAINT `FK_Shards_Celestials_Shards` FOREIGN KEY (`ShardId`) REFERENCES `Shards` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `Shards_Celestials`
--

LOCK TABLES `Shards_Celestials` WRITE;
/*!40000 ALTER TABLE `Shards_Celestials` DISABLE KEYS */;
/*!40000 ALTER TABLE `Shards_Celestials` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `Shards_Clocks`
--

DROP TABLE IF EXISTS `Shards_Clocks`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `Shards_Clocks` (
  `ShardId` bigint NOT NULL,
  `ClockId` bigint NOT NULL,
  PRIMARY KEY (`ShardId`,`ClockId`),
  CONSTRAINT `FK_Shards_Clocks_Shards` FOREIGN KEY (`ShardId`) REFERENCES `Shards` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `Shards_Clocks`
--

LOCK TABLES `Shards_Clocks` WRITE;
/*!40000 ALTER TABLE `Shards_Clocks` DISABLE KEYS */;
/*!40000 ALTER TABLE `Shards_Clocks` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `ShieldTypes`
--

DROP TABLE IF EXISTS `ShieldTypes`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `ShieldTypes` (
  `Id` bigint NOT NULL AUTO_INCREMENT,
  `Name` text CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL,
  `BlockTraitId` bigint NOT NULL,
  `BlockBonus` double NOT NULL,
  `StaminaPerBlock` double NOT NULL,
  `EffectiveArmourTypeId` bigint DEFAULT NULL,
  PRIMARY KEY (`Id`),
  KEY `FK_ShieldTypes_TraitDefinitions_idx` (`BlockTraitId`),
  KEY `FK_ShieldTypes_ArmourTypes_idx` (`EffectiveArmourTypeId`),
  CONSTRAINT `FK_ShieldTypes_ArmourTypes` FOREIGN KEY (`EffectiveArmourTypeId`) REFERENCES `ArmourTypes` (`Id`) ON DELETE SET NULL,
  CONSTRAINT `FK_ShieldTypes_TraitDefinitions` FOREIGN KEY (`BlockTraitId`) REFERENCES `TraitDefinitions` (`Id`) ON DELETE RESTRICT
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `ShieldTypes`
--

LOCK TABLES `ShieldTypes` WRITE;
/*!40000 ALTER TABLE `ShieldTypes` DISABLE KEYS */;
/*!40000 ALTER TABLE `ShieldTypes` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `ShopFinancialPeriodResults`
--

DROP TABLE IF EXISTS `ShopFinancialPeriodResults`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `ShopFinancialPeriodResults` (
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
  CONSTRAINT `FK_ShopFinancialPeriodResults_EconomicZones` FOREIGN KEY (`EconomicZoneId`) REFERENCES `EconomicZones` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_ShopFinancialPeriodResults_FinancialPeriods` FOREIGN KEY (`FinancialPeriodId`) REFERENCES `FinancialPeriods` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_ShopFinancialPeriodResults_Shops` FOREIGN KEY (`ShopId`) REFERENCES `Shops` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `ShopFinancialPeriodResults`
--

LOCK TABLES `ShopFinancialPeriodResults` WRITE;
/*!40000 ALTER TABLE `ShopFinancialPeriodResults` DISABLE KEYS */;
/*!40000 ALTER TABLE `ShopFinancialPeriodResults` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `Shops`
--

DROP TABLE IF EXISTS `Shops`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `Shops` (
  `Id` bigint NOT NULL AUTO_INCREMENT,
  `Name` varchar(200) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
  `WorkshopCellId` bigint DEFAULT NULL,
  `StockroomCellId` bigint DEFAULT NULL,
  `CanShopProgId` bigint DEFAULT NULL,
  `WhyCannotShopProgId` bigint DEFAULT NULL,
  `CurrencyId` bigint NOT NULL,
  `IsTrading` bit(1) NOT NULL,
  `EconomicZoneId` bigint NOT NULL,
  `EmployeeRecords` mediumtext CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
  `BankAccountId` bigint DEFAULT NULL,
  `CashBalance` decimal(58,29) NOT NULL DEFAULT '0.00000000000000000000000000000',
  `ShopType` varchar(200) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL DEFAULT 'Permanent',
  `MinimumFloatToBuyItems` decimal(65,30) NOT NULL DEFAULT '0.000000000000000000000000000000',
  `MarketId` bigint DEFAULT NULL,
  `AutopayTaxes` bit(1) NOT NULL DEFAULT b'1',
  PRIMARY KEY (`Id`),
  KEY `FK_Shops_FutureProgs_Can_idx` (`CanShopProgId`),
  KEY `FK_Shops_Currencies_idx` (`CurrencyId`),
  KEY `FK_Shops_EconomicZonesa_idx` (`EconomicZoneId`),
  KEY `FK_Shops_Cells_Stockroom_idx` (`StockroomCellId`),
  KEY `FK_Shops_FutureProgs_WhyCant_idx` (`WhyCannotShopProgId`),
  KEY `FK_Shops_Cells_Workshop_idx` (`WorkshopCellId`),
  KEY `IX_Shops_BankAccountId` (`BankAccountId`),
  KEY `IX_Shops_MarketId` (`MarketId`),
  CONSTRAINT `FK_Shops_BankAccounts` FOREIGN KEY (`BankAccountId`) REFERENCES `BankAccounts` (`Id`) ON DELETE SET NULL,
  CONSTRAINT `FK_Shops_Cells_Stockroom` FOREIGN KEY (`StockroomCellId`) REFERENCES `Cells` (`Id`) ON DELETE SET NULL,
  CONSTRAINT `FK_Shops_Cells_Workshop` FOREIGN KEY (`WorkshopCellId`) REFERENCES `Cells` (`Id`) ON DELETE SET NULL,
  CONSTRAINT `FK_Shops_Currencies` FOREIGN KEY (`CurrencyId`) REFERENCES `Currencies` (`Id`) ON DELETE RESTRICT,
  CONSTRAINT `FK_Shops_EconomicZones` FOREIGN KEY (`EconomicZoneId`) REFERENCES `EconomicZones` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_Shops_FutureProgs_Can` FOREIGN KEY (`CanShopProgId`) REFERENCES `FutureProgs` (`Id`) ON DELETE SET NULL,
  CONSTRAINT `FK_Shops_FutureProgs_WhyCant` FOREIGN KEY (`WhyCannotShopProgId`) REFERENCES `FutureProgs` (`Id`) ON DELETE SET NULL,
  CONSTRAINT `FK_Shops_Markets_MarketId` FOREIGN KEY (`MarketId`) REFERENCES `Markets` (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `Shops`
--

LOCK TABLES `Shops` WRITE;
/*!40000 ALTER TABLE `Shops` DISABLE KEYS */;
/*!40000 ALTER TABLE `Shops` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `Shops_StoreroomCells`
--

DROP TABLE IF EXISTS `Shops_StoreroomCells`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `Shops_StoreroomCells` (
  `ShopId` bigint NOT NULL,
  `CellId` bigint NOT NULL,
  PRIMARY KEY (`ShopId`,`CellId`),
  KEY `FK_Shops_StoreroomCells_Cells_idx` (`CellId`),
  CONSTRAINT `FK_Shops_StoreroomCells_Cells` FOREIGN KEY (`CellId`) REFERENCES `Cells` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_Shops_StoreroomCells_Shops` FOREIGN KEY (`ShopId`) REFERENCES `Shops` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `Shops_StoreroomCells`
--

LOCK TABLES `Shops_StoreroomCells` WRITE;
/*!40000 ALTER TABLE `Shops_StoreroomCells` DISABLE KEYS */;
/*!40000 ALTER TABLE `Shops_StoreroomCells` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `ShopsTills`
--

DROP TABLE IF EXISTS `ShopsTills`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `ShopsTills` (
  `ShopId` bigint NOT NULL,
  `GameItemId` bigint NOT NULL,
  PRIMARY KEY (`ShopId`,`GameItemId`),
  KEY `FK_ShopTills_GameItems_idx` (`GameItemId`),
  CONSTRAINT `FK_ShopTills_GameItems` FOREIGN KEY (`GameItemId`) REFERENCES `GameItems` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_ShopTills_Shops` FOREIGN KEY (`ShopId`) REFERENCES `Shops` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `ShopsTills`
--

LOCK TABLES `ShopsTills` WRITE;
/*!40000 ALTER TABLE `ShopsTills` DISABLE KEYS */;
/*!40000 ALTER TABLE `ShopsTills` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `ShopTransactionRecords`
--

DROP TABLE IF EXISTS `ShopTransactionRecords`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `ShopTransactionRecords` (
  `Id` bigint NOT NULL AUTO_INCREMENT,
  `CurrencyId` bigint NOT NULL,
  `PretaxValue` decimal(58,29) NOT NULL,
  `Tax` decimal(58,29) NOT NULL,
  `TransactionType` int NOT NULL,
  `ShopId` bigint NOT NULL,
  `ThirdPartyId` bigint DEFAULT NULL,
  `RealDateTime` datetime NOT NULL,
  `MudDateTime` varchar(500) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
  PRIMARY KEY (`Id`),
  KEY `FK_ShopTransactionRecords_Currencies_idx` (`CurrencyId`),
  KEY `FK_ShopTransactionRecords_Shops_idx` (`ShopId`),
  CONSTRAINT `FK_ShopTransactionRecords_Currencies` FOREIGN KEY (`CurrencyId`) REFERENCES `Currencies` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_ShopTransactionRecords_Shops` FOREIGN KEY (`ShopId`) REFERENCES `Shops` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `ShopTransactionRecords`
--

LOCK TABLES `ShopTransactionRecords` WRITE;
/*!40000 ALTER TABLE `ShopTransactionRecords` DISABLE KEYS */;
/*!40000 ALTER TABLE `ShopTransactionRecords` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `SkyDescriptionTemplates`
--

DROP TABLE IF EXISTS `SkyDescriptionTemplates`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `SkyDescriptionTemplates` (
  `Id` bigint NOT NULL AUTO_INCREMENT,
  `Name` varchar(255) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `SkyDescriptionTemplates`
--

LOCK TABLES `SkyDescriptionTemplates` WRITE;
/*!40000 ALTER TABLE `SkyDescriptionTemplates` DISABLE KEYS */;
/*!40000 ALTER TABLE `SkyDescriptionTemplates` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `SkyDescriptionTemplates_Values`
--

DROP TABLE IF EXISTS `SkyDescriptionTemplates_Values`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `SkyDescriptionTemplates_Values` (
  `SkyDescriptionTemplateId` bigint NOT NULL,
  `LowerBound` double NOT NULL,
  `UpperBound` double NOT NULL,
  `Description` varchar(4000) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
  PRIMARY KEY (`SkyDescriptionTemplateId`,`LowerBound`),
  CONSTRAINT `FK_SkyDescriptionTemplates_Values_SkyDescriptionTemplates` FOREIGN KEY (`SkyDescriptionTemplateId`) REFERENCES `SkyDescriptionTemplates` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `SkyDescriptionTemplates_Values`
--

LOCK TABLES `SkyDescriptionTemplates_Values` WRITE;
/*!40000 ALTER TABLE `SkyDescriptionTemplates_Values` DISABLE KEYS */;
/*!40000 ALTER TABLE `SkyDescriptionTemplates_Values` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `Socials`
--

DROP TABLE IF EXISTS `Socials`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `Socials` (
  `Name` varchar(100) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
  `NoTargetEcho` varchar(1000) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
  `OneTargetEcho` varchar(1000) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
  `FutureProgId` bigint DEFAULT NULL,
  `DirectionTargetEcho` varchar(1000) CHARACTER SET utf8 COLLATE utf8_general_ci DEFAULT NULL,
  `MultiTargetEcho` varchar(1000) CHARACTER SET utf8 COLLATE utf8_general_ci DEFAULT NULL,
  PRIMARY KEY (`Name`),
  KEY `FK_Socials_FutureProgs` (`FutureProgId`),
  CONSTRAINT `FK_Socials_FutureProgs` FOREIGN KEY (`FutureProgId`) REFERENCES `FutureProgs` (`Id`) ON DELETE SET NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `Socials`
--

LOCK TABLES `Socials` WRITE;
/*!40000 ALTER TABLE `Socials` DISABLE KEYS */;
/*!40000 ALTER TABLE `Socials` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `StackDecorators`
--

DROP TABLE IF EXISTS `StackDecorators`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `StackDecorators` (
  `Id` bigint NOT NULL AUTO_INCREMENT,
  `Name` varchar(255) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
  `Type` varchar(50) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
  `Definition` varchar(10000) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
  `Description` varchar(1000) CHARACTER SET utf8 COLLATE utf8_general_ci DEFAULT NULL,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `StackDecorators`
--

LOCK TABLES `StackDecorators` WRITE;
/*!40000 ALTER TABLE `StackDecorators` DISABLE KEYS */;
/*!40000 ALTER TABLE `StackDecorators` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `StaticConfigurations`
--

DROP TABLE IF EXISTS `StaticConfigurations`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `StaticConfigurations` (
  `SettingName` varchar(100) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
  `Definition` text CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
  PRIMARY KEY (`SettingName`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `StaticConfigurations`
--

LOCK TABLES `StaticConfigurations` WRITE;
/*!40000 ALTER TABLE `StaticConfigurations` DISABLE KEYS */;
/*!40000 ALTER TABLE `StaticConfigurations` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `StaticStrings`
--

DROP TABLE IF EXISTS `StaticStrings`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `StaticStrings` (
  `Id` varchar(50) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
  `Text` text CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `StaticStrings`
--

LOCK TABLES `StaticStrings` WRITE;
/*!40000 ALTER TABLE `StaticStrings` DISABLE KEYS */;
/*!40000 ALTER TABLE `StaticStrings` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `SurgicalProcedurePhases`
--

DROP TABLE IF EXISTS `SurgicalProcedurePhases`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `SurgicalProcedurePhases` (
  `SurgicalProcedureId` bigint NOT NULL,
  `PhaseNumber` int NOT NULL,
  `BaseLengthInSeconds` double NOT NULL,
  `PhaseEmote` text CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL,
  `PhaseSpecialEffects` varchar(500) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `OnPhaseProgId` bigint DEFAULT NULL,
  `InventoryActionPlan` text CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci,
  PRIMARY KEY (`SurgicalProcedureId`,`PhaseNumber`),
  KEY `FK_SurgicalProcedurePhases_FutureProgs_OnPhaseProg_idx` (`OnPhaseProgId`),
  CONSTRAINT `FK_SurgicalProcedurePhases_FutureProgs_OnPhaseProg` FOREIGN KEY (`OnPhaseProgId`) REFERENCES `FutureProgs` (`Id`) ON DELETE SET NULL,
  CONSTRAINT `FK_SurgicalProcedurePhases_SurgicalProcudres` FOREIGN KEY (`SurgicalProcedureId`) REFERENCES `SurgicalProcedures` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `SurgicalProcedurePhases`
--

LOCK TABLES `SurgicalProcedurePhases` WRITE;
/*!40000 ALTER TABLE `SurgicalProcedurePhases` DISABLE KEYS */;
/*!40000 ALTER TABLE `SurgicalProcedurePhases` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `SurgicalProcedures`
--

DROP TABLE IF EXISTS `SurgicalProcedures`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `SurgicalProcedures` (
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
  CONSTRAINT `FK_SurgicalProcedures_BodyProtos` FOREIGN KEY (`TargetBodyTypeId`) REFERENCES `BodyProtos` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_SurgicalProcedures_FutureProgs_AbortProg` FOREIGN KEY (`AbortProgId`) REFERENCES `FutureProgs` (`Id`) ON DELETE SET NULL,
  CONSTRAINT `FK_SurgicalProcedures_FutureProgs_CompletionProg` FOREIGN KEY (`CompletionProgId`) REFERENCES `FutureProgs` (`Id`) ON DELETE SET NULL,
  CONSTRAINT `FK_SurgicalProcedures_FutureProgs_Usability` FOREIGN KEY (`UsabilityProgId`) REFERENCES `FutureProgs` (`Id`) ON DELETE SET NULL,
  CONSTRAINT `FK_SurgicalProcedures_FutureProgs_WhyCannotUseProg` FOREIGN KEY (`WhyCannotUseProgId`) REFERENCES `FutureProgs` (`Id`) ON DELETE SET NULL,
  CONSTRAINT `FK_SurgicalProcedures_Knowledges` FOREIGN KEY (`KnowledgeRequiredId`) REFERENCES `knowledges` (`Id`) ON DELETE SET NULL,
  CONSTRAINT `FK_SurgicalProcedures_TraitDefinitions` FOREIGN KEY (`CheckTraitDefinitionId`) REFERENCES `TraitDefinitions` (`Id`) ON DELETE SET NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `SurgicalProcedures`
--

LOCK TABLES `SurgicalProcedures` WRITE;
/*!40000 ALTER TABLE `SurgicalProcedures` DISABLE KEYS */;
/*!40000 ALTER TABLE `SurgicalProcedures` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `Tags`
--

DROP TABLE IF EXISTS `Tags`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `Tags` (
  `Id` bigint NOT NULL AUTO_INCREMENT,
  `Name` varchar(255) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
  `ParentId` bigint DEFAULT NULL,
  `ShouldSeeProgId` bigint DEFAULT NULL,
  PRIMARY KEY (`Id`),
  KEY `FK_Tags_Parent_idx` (`ParentId`),
  KEY `FK_Tags_Futureprogs_idx` (`ShouldSeeProgId`),
  CONSTRAINT `FK_Tags_Futureprogs` FOREIGN KEY (`ShouldSeeProgId`) REFERENCES `FutureProgs` (`Id`) ON DELETE SET NULL,
  CONSTRAINT `FK_Tags_Parent` FOREIGN KEY (`ParentId`) REFERENCES `Tags` (`Id`) ON DELETE SET NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `Tags`
--

LOCK TABLES `Tags` WRITE;
/*!40000 ALTER TABLE `Tags` DISABLE KEYS */;
/*!40000 ALTER TABLE `Tags` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `Terrains`
--

DROP TABLE IF EXISTS `Terrains`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `Terrains` (
  `Id` bigint NOT NULL AUTO_INCREMENT,
  `Name` varchar(255) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
  `MovementRate` double NOT NULL,
  `DefaultTerrain` bit(1) NOT NULL DEFAULT b'0',
  `TerrainBehaviourMode` varchar(50) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
  `HideDifficulty` int NOT NULL,
  `SpotDifficulty` int NOT NULL,
  `StaminaCost` double NOT NULL,
  `ForagableProfileId` bigint NOT NULL,
  `InfectionMultiplier` double NOT NULL DEFAULT '1',
  `InfectionType` int NOT NULL,
  `InfectionVirulence` int NOT NULL DEFAULT '5',
  `AtmosphereId` bigint DEFAULT NULL,
  `AtmosphereType` varchar(45) CHARACTER SET utf8 COLLATE utf8_general_ci DEFAULT NULL,
  `TerrainEditorColour` varchar(45) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL DEFAULT '#FFFFFFFF',
  `WeatherControllerId` bigint DEFAULT NULL,
  `DefaultCellOutdoorsType` int NOT NULL DEFAULT '0',
  `TerrainEditorText` varchar(45) DEFAULT NULL,
  `TerrainANSIColour` varchar(45) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL DEFAULT '7',
  `CanHaveTracks` bit(1) NOT NULL DEFAULT b'1',
  `TrackIntensityMultiplierOlfactory` double NOT NULL DEFAULT '1',
  `TrackIntensityMultiplierVisual` double NOT NULL DEFAULT '1',
  PRIMARY KEY (`Id`),
  KEY `FK_Terrains_WeatherControllers_idx` (`WeatherControllerId`),
  CONSTRAINT `FK_Terrains_WeatherControllers` FOREIGN KEY (`WeatherControllerId`) REFERENCES `WeatherControllers` (`Id`) ON DELETE SET NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `Terrains`
--

LOCK TABLES `Terrains` WRITE;
/*!40000 ALTER TABLE `Terrains` DISABLE KEYS */;
/*!40000 ALTER TABLE `Terrains` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `Terrains_RangedCovers`
--

DROP TABLE IF EXISTS `Terrains_RangedCovers`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `Terrains_RangedCovers` (
  `TerrainId` bigint NOT NULL,
  `RangedCoverId` bigint NOT NULL,
  PRIMARY KEY (`TerrainId`,`RangedCoverId`),
  KEY `FK_Terrains_RangedCovers_RangedCovers_idx` (`RangedCoverId`),
  CONSTRAINT `FK_Terrains_RangedCovers_RangedCovers` FOREIGN KEY (`RangedCoverId`) REFERENCES `RangedCovers` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_Terrains_RangedCovers_Terrains` FOREIGN KEY (`TerrainId`) REFERENCES `Terrains` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `Terrains_RangedCovers`
--

LOCK TABLES `Terrains_RangedCovers` WRITE;
/*!40000 ALTER TABLE `Terrains_RangedCovers` DISABLE KEYS */;
/*!40000 ALTER TABLE `Terrains_RangedCovers` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `TimeZoneInfos`
--

DROP TABLE IF EXISTS `TimeZoneInfos`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `TimeZoneInfos` (
  `Id` varchar(100) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
  `Display` varchar(1000) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
  `Order` decimal(58,29) NOT NULL,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `TimeZoneInfos`
--

LOCK TABLES `TimeZoneInfos` WRITE;
/*!40000 ALTER TABLE `TimeZoneInfos` DISABLE KEYS */;
/*!40000 ALTER TABLE `TimeZoneInfos` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `Timezones`
--

DROP TABLE IF EXISTS `Timezones`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `Timezones` (
  `Id` bigint NOT NULL AUTO_INCREMENT,
  `Name` varchar(255) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
  `Description` varchar(4000) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
  `OffsetMinutes` int NOT NULL,
  `OffsetHours` int NOT NULL,
  `ClockId` bigint NOT NULL,
  PRIMARY KEY (`Id`),
  KEY `FK_Timezones_Clocks` (`ClockId`),
  CONSTRAINT `FK_Timezones_Clocks` FOREIGN KEY (`ClockId`) REFERENCES `Clocks` (`Id`) ON DELETE RESTRICT
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `Timezones`
--

LOCK TABLES `Timezones` WRITE;
/*!40000 ALTER TABLE `Timezones` DISABLE KEYS */;
/*!40000 ALTER TABLE `Timezones` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `Tracks`
--

DROP TABLE IF EXISTS `Tracks`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `Tracks` (
  `Id` bigint NOT NULL AUTO_INCREMENT,
  `CharacterId` bigint NOT NULL,
  `BodyPrototypeId` bigint NOT NULL,
  `CellId` bigint NOT NULL,
  `RoomLayer` int NOT NULL,
  `MudDateTime` varchar(100) CHARACTER SET utf8 COLLATE utf8_general_ci DEFAULT NULL,
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
  CONSTRAINT `FK_Tracks_BodyProtos` FOREIGN KEY (`BodyPrototypeId`) REFERENCES `BodyProtos` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_Tracks_Cells` FOREIGN KEY (`CellId`) REFERENCES `Cells` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_Tracks_Characters` FOREIGN KEY (`CharacterId`) REFERENCES `Characters` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_Tracks_Exits_From` FOREIGN KEY (`FromDirectionExitId`) REFERENCES `Exits` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_Tracks_Exits_To` FOREIGN KEY (`ToDirectionExitId`) REFERENCES `Exits` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_Tracks_MoveSpeeds_From` FOREIGN KEY (`FromMoveSpeedId`) REFERENCES `MoveSpeeds` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_Tracks_MoveSpeeds_To` FOREIGN KEY (`ToMoveSpeedId`) REFERENCES `MoveSpeeds` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `Tracks`
--

LOCK TABLES `Tracks` WRITE;
/*!40000 ALTER TABLE `Tracks` DISABLE KEYS */;
/*!40000 ALTER TABLE `Tracks` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `TraitDecorators`
--

DROP TABLE IF EXISTS `TraitDecorators`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `TraitDecorators` (
  `Id` bigint NOT NULL AUTO_INCREMENT,
  `Name` varchar(255) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
  `Type` varchar(255) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
  `Contents` text CHARACTER SET utf8 COLLATE utf8_general_ci,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `TraitDecorators`
--

LOCK TABLES `TraitDecorators` WRITE;
/*!40000 ALTER TABLE `TraitDecorators` DISABLE KEYS */;
/*!40000 ALTER TABLE `TraitDecorators` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `TraitDefinitions`
--

DROP TABLE IF EXISTS `TraitDefinitions`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `TraitDefinitions` (
  `Id` bigint NOT NULL AUTO_INCREMENT,
  `Name` varchar(255) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
  `Type` int NOT NULL,
  `DecoratorId` bigint NOT NULL,
  `TraitGroup` varchar(255) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
  `DerivedType` int NOT NULL,
  `ExpressionId` bigint DEFAULT NULL,
  `ImproverId` bigint DEFAULT NULL,
  `Hidden` bit(1) DEFAULT b'0',
  `ChargenBlurb` varchar(4000) CHARACTER SET utf8 COLLATE utf8_general_ci DEFAULT NULL,
  `BranchMultiplier` double NOT NULL DEFAULT '1',
  `Alias` varchar(255) CHARACTER SET utf8 COLLATE utf8_general_ci DEFAULT NULL,
  `AvailabilityProgId` bigint DEFAULT NULL,
  `TeachableProgId` bigint DEFAULT NULL,
  `LearnableProgId` bigint DEFAULT NULL,
  `TeachDifficulty` int NOT NULL DEFAULT '7',
  `LearnDifficulty` int NOT NULL DEFAULT '7',
  `ValueExpression` text CHARACTER SET utf8 COLLATE utf8_general_ci,
  `DisplayAsSubAttribute` bit(1) NOT NULL DEFAULT b'0',
  `DisplayOrder` int NOT NULL DEFAULT '1',
  `ShowInAttributeCommand` bit(1) NOT NULL DEFAULT b'1',
  `ShowInScoreCommand` bit(1) NOT NULL DEFAULT b'1',
  PRIMARY KEY (`Id`),
  KEY `FK_TraitDefinitions_AvailabilityProg` (`AvailabilityProgId`),
  KEY `FK_TraitDefinitions_TraitExpression` (`ExpressionId`),
  KEY `FK_TraitDefinitions_LearnableProg_idx` (`LearnableProgId`),
  KEY `FK_TraitDefinitions_TeachableProg_idx` (`TeachableProgId`),
  CONSTRAINT `FK_TraitDefinitions_AvailabilityProg` FOREIGN KEY (`AvailabilityProgId`) REFERENCES `FutureProgs` (`Id`) ON DELETE SET NULL,
  CONSTRAINT `FK_TraitDefinitions_LearnableProg` FOREIGN KEY (`LearnableProgId`) REFERENCES `FutureProgs` (`Id`) ON DELETE SET NULL,
  CONSTRAINT `FK_TraitDefinitions_TeachableProg` FOREIGN KEY (`TeachableProgId`) REFERENCES `FutureProgs` (`Id`) ON DELETE SET NULL,
  CONSTRAINT `FK_TraitDefinitions_TraitExpression` FOREIGN KEY (`ExpressionId`) REFERENCES `TraitExpression` (`Id`) ON DELETE RESTRICT
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `TraitDefinitions`
--

LOCK TABLES `TraitDefinitions` WRITE;
/*!40000 ALTER TABLE `TraitDefinitions` DISABLE KEYS */;
/*!40000 ALTER TABLE `TraitDefinitions` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `TraitDefinitions_ChargenResources`
--

DROP TABLE IF EXISTS `TraitDefinitions_ChargenResources`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `TraitDefinitions_ChargenResources` (
  `TraitDefinitionId` bigint NOT NULL,
  `ChargenResourceId` bigint NOT NULL,
  `RequirementOnly` bit(1) NOT NULL DEFAULT b'0',
  `Amount` int NOT NULL,
  PRIMARY KEY (`TraitDefinitionId`,`ChargenResourceId`,`RequirementOnly`),
  KEY `FK_TraitDefinitions_ChargenResources_ChargenResources` (`ChargenResourceId`),
  CONSTRAINT `FK_TraitDefinitions_ChargenResources_ChargenResources` FOREIGN KEY (`ChargenResourceId`) REFERENCES `ChargenResources` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_TraitDefinitions_ChargenResources_Races` FOREIGN KEY (`TraitDefinitionId`) REFERENCES `TraitDefinitions` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `TraitDefinitions_ChargenResources`
--

LOCK TABLES `TraitDefinitions_ChargenResources` WRITE;
/*!40000 ALTER TABLE `TraitDefinitions_ChargenResources` DISABLE KEYS */;
/*!40000 ALTER TABLE `TraitDefinitions_ChargenResources` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `TraitExpression`
--

DROP TABLE IF EXISTS `TraitExpression`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `TraitExpression` (
  `Id` bigint NOT NULL AUTO_INCREMENT,
  `Expression` varchar(4000) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
  `Name` varchar(200) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL DEFAULT 'Unnamed Expression',
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `TraitExpression`
--

LOCK TABLES `TraitExpression` WRITE;
/*!40000 ALTER TABLE `TraitExpression` DISABLE KEYS */;
/*!40000 ALTER TABLE `TraitExpression` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `TraitExpressionParameters`
--

DROP TABLE IF EXISTS `TraitExpressionParameters`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `TraitExpressionParameters` (
  `TraitExpressionId` bigint NOT NULL,
  `Parameter` varchar(50) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
  `TraitDefinitionId` bigint NOT NULL,
  `CanImprove` bit(1) NOT NULL DEFAULT b'1',
  `CanBranch` bit(1) NOT NULL DEFAULT b'1',
  PRIMARY KEY (`Parameter`,`TraitExpressionId`),
  KEY `FK_TraitExpressionParameters_TraitDefinitions` (`TraitDefinitionId`),
  KEY `FK_TraitExpressionParameters_TraitExpression` (`TraitExpressionId`),
  CONSTRAINT `FK_TraitExpressionParameters_TraitDefinitions` FOREIGN KEY (`TraitDefinitionId`) REFERENCES `TraitDefinitions` (`Id`) ON DELETE RESTRICT,
  CONSTRAINT `FK_TraitExpressionParameters_TraitExpression` FOREIGN KEY (`TraitExpressionId`) REFERENCES `TraitExpression` (`Id`) ON DELETE RESTRICT
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `TraitExpressionParameters`
--

LOCK TABLES `TraitExpressionParameters` WRITE;
/*!40000 ALTER TABLE `TraitExpressionParameters` DISABLE KEYS */;
/*!40000 ALTER TABLE `TraitExpressionParameters` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `Traits`
--

DROP TABLE IF EXISTS `Traits`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `Traits` (
  `BodyId` bigint NOT NULL,
  `TraitDefinitionId` bigint NOT NULL,
  `Value` double NOT NULL,
  `AdditionalValue` double NOT NULL,
  PRIMARY KEY (`BodyId`,`TraitDefinitionId`),
  KEY `FK_Traits_TraitDefinitions` (`TraitDefinitionId`),
  CONSTRAINT `FK_Traits_Bodies` FOREIGN KEY (`BodyId`) REFERENCES `Bodies` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_Traits_TraitDefinitions` FOREIGN KEY (`TraitDefinitionId`) REFERENCES `TraitDefinitions` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `Traits`
--

LOCK TABLES `Traits` WRITE;
/*!40000 ALTER TABLE `Traits` DISABLE KEYS */;
/*!40000 ALTER TABLE `Traits` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `UnitOfMeasure`
--

DROP TABLE IF EXISTS `UnitOfMeasure`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `UnitOfMeasure` (
  `Id` bigint NOT NULL AUTO_INCREMENT,
  `Name` varchar(50) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
  `PrimaryAbbreviation` varchar(45) CHARACTER SET utf8 COLLATE utf8_general_ci DEFAULT NULL,
  `Abbreviations` varchar(4000) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
  `BaseMultiplier` double NOT NULL,
  `PreMultiplierBaseOffset` double NOT NULL,
  `PostMultiplierBaseOffset` double NOT NULL,
  `Type` int NOT NULL,
  `Describer` bit(1) NOT NULL,
  `SpaceBetween` bit(1) NOT NULL DEFAULT b'1',
  `System` varchar(50) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
  `DefaultUnitForSystem` bit(1) NOT NULL DEFAULT b'0',
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `UnitOfMeasure`
--

LOCK TABLES `UnitOfMeasure` WRITE;
/*!40000 ALTER TABLE `UnitOfMeasure` DISABLE KEYS */;
/*!40000 ALTER TABLE `UnitOfMeasure` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `VariableDefaults`
--

DROP TABLE IF EXISTS `VariableDefaults`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `VariableDefaults` (
  `OwnerType` bigint NOT NULL,
  `Property` varchar(50) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
  `DefaultValue` varchar(4000) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
  PRIMARY KEY (`OwnerType`,`Property`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `VariableDefaults`
--

LOCK TABLES `VariableDefaults` WRITE;
/*!40000 ALTER TABLE `VariableDefaults` DISABLE KEYS */;
/*!40000 ALTER TABLE `VariableDefaults` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `VariableDefinitions`
--

DROP TABLE IF EXISTS `VariableDefinitions`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `VariableDefinitions` (
  `OwnerType` bigint NOT NULL,
  `Property` varchar(50) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
  `ContainedType` bigint NOT NULL,
  PRIMARY KEY (`OwnerType`,`Property`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `VariableDefinitions`
--

LOCK TABLES `VariableDefinitions` WRITE;
/*!40000 ALTER TABLE `VariableDefinitions` DISABLE KEYS */;
/*!40000 ALTER TABLE `VariableDefinitions` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `VariableValues`
--

DROP TABLE IF EXISTS `VariableValues`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `VariableValues` (
  `ReferenceType` bigint NOT NULL,
  `ReferenceId` bigint NOT NULL,
  `ReferenceProperty` varchar(50) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
  `ValueDefinition` varchar(4000) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
  `ValueType` bigint NOT NULL,
  PRIMARY KEY (`ReferenceType`,`ReferenceId`,`ReferenceProperty`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `VariableValues`
--

LOCK TABLES `VariableValues` WRITE;
/*!40000 ALTER TABLE `VariableValues` DISABLE KEYS */;
/*!40000 ALTER TABLE `VariableValues` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `WeaponAttacks`
--

DROP TABLE IF EXISTS `WeaponAttacks`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `WeaponAttacks` (
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
  `RequiredPositionStateIds` varchar(250) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL DEFAULT '1 16 17 18',
  PRIMARY KEY (`Id`),
  KEY `FK_WeaponAttacks_TraitExpression_Damage_idx` (`DamageExpressionId`),
  KEY `FK_WeaponAttacks_FutureProgs_idx` (`FutureProgId`),
  KEY `FK_WeaponAttacks_TraitExpression_Pain_idx` (`PainExpressionId`),
  KEY `FK_WeaponAttacks_TraitExpression_Stun_idx` (`StunExpressionId`),
  KEY `FK_WeaponAttacks_WeaponTypes_idx` (`WeaponTypeId`),
  CONSTRAINT `FK_WeaponAttacks_FutureProgs` FOREIGN KEY (`FutureProgId`) REFERENCES `FutureProgs` (`Id`) ON DELETE SET NULL,
  CONSTRAINT `FK_WeaponAttacks_TraitExpression_Damage` FOREIGN KEY (`DamageExpressionId`) REFERENCES `TraitExpression` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_WeaponAttacks_TraitExpression_Pain` FOREIGN KEY (`PainExpressionId`) REFERENCES `TraitExpression` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_WeaponAttacks_TraitExpression_Stun` FOREIGN KEY (`StunExpressionId`) REFERENCES `TraitExpression` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_WeaponAttacks_WeaponTypes` FOREIGN KEY (`WeaponTypeId`) REFERENCES `WeaponTypes` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `WeaponAttacks`
--

LOCK TABLES `WeaponAttacks` WRITE;
/*!40000 ALTER TABLE `WeaponAttacks` DISABLE KEYS */;
/*!40000 ALTER TABLE `WeaponAttacks` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `WeaponTypes`
--

DROP TABLE IF EXISTS `WeaponTypes`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `WeaponTypes` (
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
  CONSTRAINT `FK_WeaponTypes_TraitDefinitions_Attack` FOREIGN KEY (`AttackTraitId`) REFERENCES `TraitDefinitions` (`Id`) ON DELETE SET NULL,
  CONSTRAINT `FK_WeaponTypes_TraitDefinitions_Parry` FOREIGN KEY (`ParryTraitId`) REFERENCES `TraitDefinitions` (`Id`) ON DELETE SET NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `WeaponTypes`
--

LOCK TABLES `WeaponTypes` WRITE;
/*!40000 ALTER TABLE `WeaponTypes` DISABLE KEYS */;
/*!40000 ALTER TABLE `WeaponTypes` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `WearableSizeParameterRule`
--

DROP TABLE IF EXISTS `WearableSizeParameterRule`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `WearableSizeParameterRule` (
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
  `WeightVolumeRatios` varchar(4000) CHARACTER SET utf8 COLLATE utf8_general_ci DEFAULT NULL,
  `TraitVolumeRatios` varchar(4000) CHARACTER SET utf8 COLLATE utf8_general_ci DEFAULT NULL,
  `HeightLinearRatios` varchar(4000) CHARACTER SET utf8 COLLATE utf8_general_ci DEFAULT NULL,
  PRIMARY KEY (`Id`),
  KEY `FK_WearableSizeParameterRule_TraitDefinitions` (`TraitId`),
  CONSTRAINT `FK_WearableSizeParameterRule_TraitDefinitions` FOREIGN KEY (`TraitId`) REFERENCES `TraitDefinitions` (`Id`) ON DELETE RESTRICT
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `WearableSizeParameterRule`
--

LOCK TABLES `WearableSizeParameterRule` WRITE;
/*!40000 ALTER TABLE `WearableSizeParameterRule` DISABLE KEYS */;
/*!40000 ALTER TABLE `WearableSizeParameterRule` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `WearableSizes`
--

DROP TABLE IF EXISTS `WearableSizes`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `WearableSizes` (
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
-- Dumping data for table `WearableSizes`
--

LOCK TABLES `WearableSizes` WRITE;
/*!40000 ALTER TABLE `WearableSizes` DISABLE KEYS */;
/*!40000 ALTER TABLE `WearableSizes` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `WearProfiles`
--

DROP TABLE IF EXISTS `WearProfiles`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `WearProfiles` (
  `Id` bigint NOT NULL AUTO_INCREMENT,
  `Name` varchar(255) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
  `BodyPrototypeId` bigint NOT NULL,
  `WearStringInventory` varchar(255) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL DEFAULT 'worn on',
  `WearAction1st` varchar(255) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL DEFAULT 'put',
  `WearAction3rd` varchar(255) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL DEFAULT 'puts',
  `WearAffix` varchar(255) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL DEFAULT 'on',
  `WearlocProfiles` text CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
  `Type` varchar(50) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL DEFAULT 'Direct',
  `Description` text CHARACTER SET utf8 COLLATE utf8_general_ci,
  `RequireContainerIsEmpty` bit(1) NOT NULL DEFAULT b'0',
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `WearProfiles`
--

LOCK TABLES `WearProfiles` WRITE;
/*!40000 ALTER TABLE `WearProfiles` DISABLE KEYS */;
/*!40000 ALTER TABLE `WearProfiles` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `WeatherControllers`
--

DROP TABLE IF EXISTS `WeatherControllers`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `WeatherControllers` (
  `Id` bigint NOT NULL AUTO_INCREMENT,
  `Name` varchar(500) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
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
  PRIMARY KEY (`Id`),
  KEY `FK_WeatherControllers_Celestials_idx` (`CelestialId`),
  KEY `FK_WeatherControllers_Seasons_idx` (`CurrentSeasonId`),
  KEY `FK_WeatherControllers_WeatherEvents_idx` (`CurrentWeatherEventId`),
  KEY `FK_WeatherControllers_Clocks_idx` (`FeedClockId`),
  KEY `FK_WeatherControllers_TimeZones_idx` (`FeedClockTimeZoneId`),
  KEY `FK_WeatherControllers_RegionalClimates_idx` (`RegionalClimateId`),
  CONSTRAINT `FK_WeatherControllers_Celestials` FOREIGN KEY (`CelestialId`) REFERENCES `Celestials` (`Id`) ON DELETE RESTRICT,
  CONSTRAINT `FK_WeatherControllers_Clocks` FOREIGN KEY (`FeedClockId`) REFERENCES `Clocks` (`Id`) ON DELETE RESTRICT,
  CONSTRAINT `FK_WeatherControllers_RegionalClimates` FOREIGN KEY (`RegionalClimateId`) REFERENCES `RegionalClimates` (`Id`) ON DELETE RESTRICT,
  CONSTRAINT `FK_WeatherControllers_Seasons` FOREIGN KEY (`CurrentSeasonId`) REFERENCES `Seasons` (`Id`) ON DELETE RESTRICT,
  CONSTRAINT `FK_WeatherControllers_TimeZones` FOREIGN KEY (`FeedClockTimeZoneId`) REFERENCES `Timezones` (`Id`) ON DELETE RESTRICT,
  CONSTRAINT `FK_WeatherControllers_WeatherEvents` FOREIGN KEY (`CurrentWeatherEventId`) REFERENCES `WeatherEvents` (`Id`) ON DELETE RESTRICT
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `WeatherControllers`
--

LOCK TABLES `WeatherControllers` WRITE;
/*!40000 ALTER TABLE `WeatherControllers` DISABLE KEYS */;
/*!40000 ALTER TABLE `WeatherControllers` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `WeatherEvents`
--

DROP TABLE IF EXISTS `WeatherEvents`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `WeatherEvents` (
  `Id` bigint NOT NULL AUTO_INCREMENT,
  `Name` varchar(200) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
  `WeatherEventType` varchar(45) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
  `WeatherDescription` text CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
  `WeatherRoomAddendum` text CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
  `TemperatureEffect` double NOT NULL,
  `Precipitation` int NOT NULL,
  `Wind` int NOT NULL,
  `AdditionalInfo` text CHARACTER SET utf8 COLLATE utf8_general_ci,
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
  CONSTRAINT `FK_WeatherEvents_WeatherEvents` FOREIGN KEY (`CountsAsId`) REFERENCES `WeatherEvents` (`Id`) ON DELETE SET NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `WeatherEvents`
--

LOCK TABLES `WeatherEvents` WRITE;
/*!40000 ALTER TABLE `WeatherEvents` DISABLE KEYS */;
/*!40000 ALTER TABLE `WeatherEvents` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `WeeklyStatistics`
--

DROP TABLE IF EXISTS `WeeklyStatistics`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `WeeklyStatistics` (
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
-- Dumping data for table `WeeklyStatistics`
--

LOCK TABLES `WeeklyStatistics` WRITE;
/*!40000 ALTER TABLE `WeeklyStatistics` DISABLE KEYS */;
/*!40000 ALTER TABLE `WeeklyStatistics` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `WitnessProfiles`
--

DROP TABLE IF EXISTS `WitnessProfiles`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `WitnessProfiles` (
  `Id` bigint NOT NULL AUTO_INCREMENT,
  `Name` varchar(250) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
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
  CONSTRAINT `FK_WitnessProfiles_IdentityProg` FOREIGN KEY (`IdentityKnownProgId`) REFERENCES `FutureProgs` (`Id`) ON DELETE RESTRICT,
  CONSTRAINT `FK_WitnessProfiles_MultiplierProg` FOREIGN KEY (`ReportingMultiplierProgId`) REFERENCES `FutureProgs` (`Id`) ON DELETE RESTRICT
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `WitnessProfiles`
--

LOCK TABLES `WitnessProfiles` WRITE;
/*!40000 ALTER TABLE `WitnessProfiles` DISABLE KEYS */;
/*!40000 ALTER TABLE `WitnessProfiles` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `WitnessProfiles_CooperatingAuthorities`
--

DROP TABLE IF EXISTS `WitnessProfiles_CooperatingAuthorities`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `WitnessProfiles_CooperatingAuthorities` (
  `WitnessProfileId` bigint NOT NULL,
  `LegalAuthorityId` bigint NOT NULL,
  PRIMARY KEY (`WitnessProfileId`,`LegalAuthorityId`),
  KEY `FK_WitnessProfiles_CooperatingAuthorities_LegalAuthorities_idx` (`LegalAuthorityId`),
  CONSTRAINT `FK_WitnessProfiles_CooperatingAuthorities_LegalAuthorities` FOREIGN KEY (`LegalAuthorityId`) REFERENCES `LegalAuthorities` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_WitnessProfiles_CooperatingAuthorities_WitnessProfiles` FOREIGN KEY (`WitnessProfileId`) REFERENCES `WitnessProfiles` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `WitnessProfiles_CooperatingAuthorities`
--

LOCK TABLES `WitnessProfiles_CooperatingAuthorities` WRITE;
/*!40000 ALTER TABLE `WitnessProfiles_CooperatingAuthorities` DISABLE KEYS */;
/*!40000 ALTER TABLE `WitnessProfiles_CooperatingAuthorities` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `WitnessProfiles_IgnoredCriminalClasses`
--

DROP TABLE IF EXISTS `WitnessProfiles_IgnoredCriminalClasses`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `WitnessProfiles_IgnoredCriminalClasses` (
  `WitnessProfileId` bigint NOT NULL,
  `LegalClassId` bigint NOT NULL,
  PRIMARY KEY (`WitnessProfileId`,`LegalClassId`),
  KEY `FK_WitnessProfiles_IgnoredCriminalClasses_LegalClasses_idx` (`LegalClassId`),
  CONSTRAINT `FK_WitnessProfiles_IgnoredCriminalClasses_LegalClasses` FOREIGN KEY (`LegalClassId`) REFERENCES `LegalClasses` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_WitnessProfiles_IgnoredCriminalClasses_WitnessProfiles` FOREIGN KEY (`WitnessProfileId`) REFERENCES `WitnessProfiles` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `WitnessProfiles_IgnoredCriminalClasses`
--

LOCK TABLES `WitnessProfiles_IgnoredCriminalClasses` WRITE;
/*!40000 ALTER TABLE `WitnessProfiles_IgnoredCriminalClasses` DISABLE KEYS */;
/*!40000 ALTER TABLE `WitnessProfiles_IgnoredCriminalClasses` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `WitnessProfiles_IgnoredVictimClasses`
--

DROP TABLE IF EXISTS `WitnessProfiles_IgnoredVictimClasses`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `WitnessProfiles_IgnoredVictimClasses` (
  `WitnessProfileId` bigint NOT NULL,
  `LegalClassId` bigint NOT NULL,
  PRIMARY KEY (`WitnessProfileId`,`LegalClassId`),
  KEY `FK_WitnessProfiles_IgnoredVictimClasses_LegalClasses_idx` (`LegalClassId`),
  CONSTRAINT `FK_WitnessProfiles_IgnoredVictimClasses_LegalClasses` FOREIGN KEY (`LegalClassId`) REFERENCES `LegalClasses` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_WitnessProfiles_IgnoredVictimClasses_WitnessProfiles` FOREIGN KEY (`WitnessProfileId`) REFERENCES `WitnessProfiles` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `WitnessProfiles_IgnoredVictimClasses`
--

LOCK TABLES `WitnessProfiles_IgnoredVictimClasses` WRITE;
/*!40000 ALTER TABLE `WitnessProfiles_IgnoredVictimClasses` DISABLE KEYS */;
/*!40000 ALTER TABLE `WitnessProfiles_IgnoredVictimClasses` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `Wounds`
--

DROP TABLE IF EXISTS `Wounds`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `Wounds` (
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
  CONSTRAINT `FK_Wounds_Bodies` FOREIGN KEY (`BodyId`) REFERENCES `Bodies` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_Wounds_Characters` FOREIGN KEY (`ActorOriginId`) REFERENCES `Characters` (`Id`) ON DELETE SET NULL,
  CONSTRAINT `FK_Wounds_GameItemOwner` FOREIGN KEY (`GameItemId`) REFERENCES `GameItems` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_Wounds_GameItems` FOREIGN KEY (`LodgedItemId`) REFERENCES `GameItems` (`Id`) ON DELETE SET NULL,
  CONSTRAINT `FK_Wounds_GameItems_Tool` FOREIGN KEY (`ToolOriginId`) REFERENCES `GameItems` (`Id`) ON DELETE SET NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `Wounds`
--

LOCK TABLES `Wounds` WRITE;
/*!40000 ALTER TABLE `Wounds` DISABLE KEYS */;
/*!40000 ALTER TABLE `Wounds` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `Writings`
--

DROP TABLE IF EXISTS `Writings`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `Writings` (
  `Id` bigint NOT NULL AUTO_INCREMENT,
  `WritingType` varchar(45) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
  `Style` int NOT NULL,
  `LanguageId` bigint NOT NULL,
  `ScriptId` bigint NOT NULL,
  `AuthorId` bigint NOT NULL,
  `TrueAuthorId` bigint DEFAULT NULL,
  `HandwritingSkill` double NOT NULL,
  `LiteracySkill` double NOT NULL,
  `ForgerySkill` double NOT NULL,
  `LanguageSkill` double NOT NULL,
  `Definition` text CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
  `WritingColour` bigint NOT NULL,
  `ImplementType` int NOT NULL,
  PRIMARY KEY (`Id`),
  KEY `FK_Writings_Characters_Author_idx` (`AuthorId`),
  KEY `FK_Writings_Languages_idx` (`LanguageId`),
  KEY `FK_Writings_Scripts_idx` (`ScriptId`),
  KEY `FK_Writings_Characters_TrueAuthor_idx` (`TrueAuthorId`),
  CONSTRAINT `FK_Writings_Characters_Author` FOREIGN KEY (`AuthorId`) REFERENCES `Characters` (`Id`) ON DELETE RESTRICT,
  CONSTRAINT `FK_Writings_Characters_TrueAuthor` FOREIGN KEY (`TrueAuthorId`) REFERENCES `Characters` (`Id`) ON DELETE SET NULL,
  CONSTRAINT `FK_Writings_Languages` FOREIGN KEY (`LanguageId`) REFERENCES `Languages` (`Id`) ON DELETE RESTRICT,
  CONSTRAINT `FK_Writings_Scripts` FOREIGN KEY (`ScriptId`) REFERENCES `Scripts` (`Id`) ON DELETE RESTRICT
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `Writings`
--

LOCK TABLES `Writings` WRITE;
/*!40000 ALTER TABLE `Writings` DISABLE KEYS */;
/*!40000 ALTER TABLE `Writings` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `Zones`
--

DROP TABLE IF EXISTS `Zones`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `Zones` (
  `Id` bigint NOT NULL AUTO_INCREMENT,
  `Name` varchar(255) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
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
  CONSTRAINT `FK_Zones_Cells` FOREIGN KEY (`DefaultCellId`) REFERENCES `Cells` (`Id`) ON DELETE RESTRICT,
  CONSTRAINT `FK_Zones_Shards` FOREIGN KEY (`ShardId`) REFERENCES `Shards` (`Id`) ON DELETE RESTRICT,
  CONSTRAINT `FK_Zones_WeatherControllers` FOREIGN KEY (`WeatherControllerId`) REFERENCES `WeatherControllers` (`Id`) ON DELETE RESTRICT
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `Zones`
--

LOCK TABLES `Zones` WRITE;
/*!40000 ALTER TABLE `Zones` DISABLE KEYS */;
/*!40000 ALTER TABLE `Zones` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `Zones_Timezones`
--

DROP TABLE IF EXISTS `Zones_Timezones`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `Zones_Timezones` (
  `ZoneId` bigint NOT NULL,
  `ClockId` bigint NOT NULL,
  `TimezoneId` bigint NOT NULL,
  PRIMARY KEY (`ZoneId`,`ClockId`,`TimezoneId`),
  CONSTRAINT `FK_Zones_Timezones_Zones` FOREIGN KEY (`ZoneId`) REFERENCES `Zones` (`Id`) ON DELETE RESTRICT
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `Zones_Timezones`
--

LOCK TABLES `Zones_Timezones` WRITE;
/*!40000 ALTER TABLE `Zones_Timezones` DISABLE KEYS */;
/*!40000 ALTER TABLE `Zones_Timezones` ENABLE KEYS */;
UNLOCK TABLES;
/*!40103 SET TIME_ZONE=@OLD_TIME_ZONE */;

/*!40101 SET SQL_MODE=@OLD_SQL_MODE */;
/*!40014 SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS */;
/*!40014 SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS */;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
/*!40111 SET SQL_NOTES=@OLD_SQL_NOTES */;

";
}