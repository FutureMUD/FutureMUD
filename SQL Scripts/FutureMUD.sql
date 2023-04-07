CREATE DATABASE  IF NOT EXISTS `dbo` /*!40100 DEFAULT CHARACTER SET utf8 */;
USE `dbo`;

/*!40101 SET NAMES utf8 */;
SET character_set_client = utf8;
DELIMITER ;;
/****** Object:  StoredProcedure `CreateBodyTrait`    Script Date: 20/09/2014 7:16:11 PM ******/
CREATE PROCEDURE `CreateBodyTrait` (
	bodyid bigint,
	traitid bigint,
	tvalue double)

	INSERT INTO dbo.Traits
	(BodyId, TraitDefinitionId, Value)
	VALUES (@bodyid, @traitid, @tvalue);
;;

/****** Object:  StoredProcedure `InsertBodyAccent`    Script Date: 20/09/2014 7:16:11 PM ******/
CREATE PROCEDURE `InsertBodyAccent` (
	bodyid bigint,
	accentid bigint,
	familiarity bigint)

	INSERT INTO dbo.Bodies_Accents
	(BodyId, AccentId, Familiarity)
	VALUES (@bodyid, @accentid, @familiarity);
;;

/****** Object:  StoredProcedure `InsertBodyCharacteristic`    Script Date: 20/09/2014 7:16:11 PM ******/
CREATE PROCEDURE `InsertBodyCharacteristic` (
	bodyid bigint,
	definition bigint,
	tvalue bigint)

	INSERT INTO dbo.Characteristics
	(BodyId, Type, CharacteristicId)
	VALUES (@bodyid, @definition, @tvalue);
;;


/****** Object:  StoredProcedure `InsertBodyLanguage`    Script Date: 20/09/2014 7:16:11 PM ******/
CREATE PROCEDURE `InsertBodyLanguage` (
	bodyid bigint,
	definition bigint)

	INSERT INTO dbo.Bodies_Languages
	(BodyId, LanguageId)
	VALUES (@bodyid, @definition);
;;

/****** Object:  StoredProcedure `InsertCharacterRoles`    Script Date: 20/09/2014 7:16:11 PM ******/
CREATE PROCEDURE `InsertCharacterRoles` (
	characterid BIGINT,
	roles VARCHAR(1000))
BEGIN
	declare tvalue BIGINT;
	declare tindex INT;

	if (@roles IS NOT NULL) then
	   set @tindex = LOCATE(',', @roles);
	   while @tindex > 0 do
	      set @tvalue = SUBSTRING(@roles, 1, @tindex-1);
		  set @roles = SUBSTRING(@roles, @tindex+1, LENGTH(@tvalue)-@tindex);
		  INSERT INTO dbo.Characters_ChargenRoles (CharacterId, ChargenRoleId) VALUES (@characterid, @tvalue);
		  set @tindex = LOCATE(',', @roles);
	   END while;
	END if;
END;
;;

/****** Object:  StoredProcedure `InsertNewBody`    Script Date: 20/09/2014 7:16:11 PM ******/
CREATE PROCEDURE `InsertNewBody` (
	height double,
	weight double,
	race bigint,
	proto bigint,
	position bigint,
	sdesc varchar(4000),
	fdesc varchar(4000),
	sdescpattern bigint,
	fdescpattern bigint,
	gender smallint,
	OUT bodyid bigint)
BEGIN
	declare entitydescription int;
	INSERT INTO dbo.EntityDescriptions (ShortDescription, FullDescription, DisplaySex)
	VALUES (@sdesc, @fdesc, @gender);

	
	set @entitydescription = LAST_INSERT_ID('EntityDescriptions');

	if (@fdescpattern is not null) then
		INSERT INTO dbo.EntityDescriptionPatterns_EntityDescriptions (EntityDescriptionId, PatternId)
		VALUES (@entitydescription, @fdescpattern);
	end if;
	if (@sdescpattern is not null) then
		INSERT INTO dbo.EntityDescriptionPatterns_EntityDescriptions (EntityDescriptionId, PatternId)
		VALUES (@entitydescription, @sdescpattern);
	end if;

	INSERT INTO dbo.Bodies (Height, Weight, RaceId, BodyPrototypeID, Position, EntityDescriptionID, CurrentSpeed)
	VALUES (@height, @weight, @race, @proto, @position, @entitydescription, 1);
	set @bodyid = LAST_INSERT_ID('Bodies');
END
;;

/****** Object:  StoredProcedure `InsertNewCharacter`    Script Date: 20/09/2014 7:16:11 PM ******/
CREATE PROCEDURE `InsertNewCharacter` (
	location bigint,
	name varchar(4000),
	gender smallint,
	culture bigint,
	creationtime datetime,
	body bigint,
	birthday varchar(4000),
	birthdaycalendar bigint,
	account bigint,
	nameid bigint,
	out characterid bigint)
BEGIN
	-- Insert Character entry
	INSERT INTO dbo.Characters (IsAdminAvatar, Location, Name, State, Status, Gender, CultureId, CreationTime, BodyId, BirthdayDate, BirthdayCalendarId, AccountId, PersonalNameId, CurrentNameId)
	Values (0, @location, @name, 1, 2, @gender, @culture, @creationtime, @body, @birthday, @birthdaycalendar, @account, @nameid, @nameid);	

	-- Return value is the inserted identity of the character
	set @characterid = LAST_INSERT_ID('Characters');
END
;;

/****** Object:  StoredProcedure `sp_DeletePersonalName`    Script Date: 20/09/2014 7:16:11 PM ******/
CREATE PROCEDURE `sp_DeletePersonalName` (
	id bigint)

DELETE FROM dbo.PersonalName WHERE Id = @id;
;;

/****** Object:  StoredProcedure `sp_LoginIP`    Script Date: 20/09/2014 7:16:11 PM ******/
CREATE PROCEDURE `sp_LoginIP` (
	ipaddress varchar(200),
	accountid bigint)
BEGIN
	if (select count(0) from LoginIPs where AccountId = @accountid and IpAddress = @ipaddress) = 0 then
		insert into LoginIPs (AccountId, IpAddress, FirstDate) values (@accountid, @ipaddress, UTC_TIMESTAMP());
	end if;
END
;;

DELIMITER ;
/****** Object:  Table `Accents`    Script Date: 20/09/2014 7:16:11 PM ******/
CREATE TABLE `Accents`(
	`Id` bigint AUTO_INCREMENT NOT NULL,
	`LanguageId` bigint NOT NULL,
	`Name` varchar(50) NOT NULL,
	`Suffix` varchar(4000) NOT NULL,
	`VagueSuffix` varchar(4000) NOT NULL,
	`Difficulty` int NOT NULL,
	`Description` varchar(4000) NOT NULL,
	`Group` varchar(50) NULL,
 CONSTRAINT `PK_Accents` PRIMARY KEY CLUSTERED 
(
	`Id` ASC
)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

/****** Object:  Table `AccountNotes`    Script Date: 20/09/2014 7:16:11 PM ******/
CREATE TABLE `AccountNotes`(
	`Id` bigint AUTO_INCREMENT NOT NULL,
	`AccountId` bigint NOT NULL,
	`Text` varchar(4000) NOT NULL,
	`Subject` varchar(4000) NOT NULL,
	`TimeStamp` datetime NOT NULL,
	`AuthorId` bigint NULL,
PRIMARY KEY CLUSTERED 
(
	`Id` ASC
)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

/****** Object:  Table `Accounts`    Script Date: 20/09/2014 7:16:11 PM ******/
CREATE TABLE `Accounts`(
	`Name` varchar(4000) NOT NULL,
	`Id` bigint AUTO_INCREMENT NOT NULL,
	`Password` varchar(4000) NOT NULL,
	`Salt` bigint NOT NULL,
	`AccessStatus` int NOT NULL DEFAULT '0',
	`AuthorityGroupId` bigint NULL DEFAULT '0',
	`Email` varchar(4000) NULL,
	`LastLoginTime` datetime NULL,
	`LastLoginIP` varchar(50) NULL,
	`FormatLength` int NOT NULL DEFAULT '80',
	`UseMXP` bit(1) NOT NULL DEFAULT 0,
	`UseMSP` bit(1) NOT NULL DEFAULT 0,
	`UseMCCP` bit(1) NOT NULL DEFAULT 0,
	`ActiveCharactersAllowed` int NOT NULL DEFAULT '1',
	`UseUnicode` bit(1) NOT NULL DEFAULT 0,
	`TimeZoneId` varchar(100) NOT NULL,
	`CultureName` varchar(50) NOT NULL,
	`RegistrationCode` varchar(50) NULL,
	`IsRegistered` bit(1) NOT NULL DEFAULT 0,
	`RecoveryCode` varchar(4000) NULL,
	`UnitPreference` varchar(50) NOT NULL,
	`CreationDate` datetime NOT NULL,
	CONSTRAINT `PK_Accounts` PRIMARY KEY CLUSTERED 
	(
		`Id` ASC
	)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

/****** Object:  Table `Accounts_ChargenResources`    Script Date: 20/09/2014 7:16:11 PM ******/
CREATE TABLE `Accounts_ChargenResources`(
	`AccountId` bigint NOT NULL,
	`ChargenResourceId` bigint NOT NULL,
	`Amount` int NOT NULL,
	`LastAwardDate` datetime NOT NULL,
PRIMARY KEY CLUSTERED 
(
	`AccountId` ASC,
	`ChargenResourceId` ASC
)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

/****** Object:  Table `Appointments`    Script Date: 20/09/2014 7:16:11 PM ******/
CREATE TABLE `Appointments`(
	`Id` bigint AUTO_INCREMENT NOT NULL,
	`Name` varchar(100) NOT NULL,
	`MaximumSimultaneousHolders` int NOT NULL DEFAULT '1',
	`MinimumRankId` bigint NULL,
	`ParentAppointmentId` bigint NULL,
	`PaygradeId` bigint NULL,
	`InsigniaGameItemId` bigint NULL,
	`InsigniaGameItemRevnum` int NULL,
	`ClanId` bigint NOT NULL,
	`Privileges` bigint NOT NULL DEFAULT '0',
	`MinimumRankToAppointId` bigint NULL,
PRIMARY KEY CLUSTERED 
(
	`Id` ASC
)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

/****** Object:  Table `Appointments_Abbreviations`    Script Date: 20/09/2014 7:16:11 PM ******/
CREATE TABLE `Appointments_Abbreviations`(
	`AppointmentId` bigint NOT NULL,
	`Abbreviation` varchar(50) NOT NULL,
	`FutureProgId` bigint NULL,
	`Order` int NOT NULL,
PRIMARY KEY CLUSTERED 
(
	`Abbreviation` ASC,
	`AppointmentId` ASC
)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

/****** Object:  Table `Appointments_Titles`    Script Date: 20/09/2014 7:16:11 PM ******/
CREATE TABLE `Appointments_Titles`(
	`AppointmentId` bigint NOT NULL,
	`Title` varchar(200) NOT NULL,
	`FutureProgId` bigint NULL,
	`Order` int NULL,
PRIMARY KEY CLUSTERED 
(
	`Title` ASC,
	`AppointmentId` ASC
)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

/****** Object:  Table `ArtificialIntelligences`    Script Date: 20/09/2014 7:16:11 PM ******/
CREATE TABLE `ArtificialIntelligences`(
	`Id` bigint AUTO_INCREMENT NOT NULL,
	`Name` varchar(4000) NOT NULL,
	`Type` varchar(4000) NOT NULL,
	`Definition` varchar(4000) NOT NULL,
PRIMARY KEY CLUSTERED 
(
	`Id` ASC
)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

/****** Object:  Table `AuthorityGroups`    Script Date: 20/09/2014 7:16:11 PM ******/
CREATE TABLE `AuthorityGroups`(
	`Name` varchar(4000) NOT NULL,
	`Id` bigint AUTO_INCREMENT NOT NULL,
	`AuthorityLevel` int NOT NULL DEFAULT '0',
	`InformationLevel` int NOT NULL DEFAULT '0',
	`AccountsLevel` int NOT NULL DEFAULT '0',
	`CharactersLevel` int NOT NULL DEFAULT '0',
	`CharacterApprovalLevel` int NOT NULL DEFAULT '0',
	`CharacterApprovalRisk` int NOT NULL DEFAULT '0',
	`ItemsLevel` int NOT NULL DEFAULT '0',
	`PlanesLevel` int NOT NULL DEFAULT '0',
	`RoomsLevel` int NOT NULL DEFAULT '0',
 CONSTRAINT `PK_AuthorityGroups` PRIMARY KEY CLUSTERED 
(
	`Id` ASC
)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

/****** Object:  Table `Bans`    Script Date: 20/09/2014 7:16:11 PM ******/
CREATE TABLE `Bans`(
	`Id` bigint AUTO_INCREMENT NOT NULL,
	`IpMask` varchar(200) NOT NULL,
	`BannerAccountId` bigint NULL,
	`Reason` varchar(4000) NOT NULL,
	`Expiry` datetime NULL,
PRIMARY KEY CLUSTERED 
(
	`Id` ASC
)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

/****** Object:  Table `Bodies`    Script Date: 20/09/2014 7:16:11 PM ******/
CREATE TABLE `Bodies`(
	`Id` bigint AUTO_INCREMENT NOT NULL,
	`BodyPrototypeID` bigint NOT NULL,
	`Height` double NOT NULL,
	`Weight` double NOT NULL,
	`EntityDescriptionID` bigint NOT NULL,
	`Position` bigint NOT NULL,
	`CurrentSpeed` bigint NULL,
	`CurrentLanguageId` bigint NULL,
	`CurrentAccentId` bigint NULL,
	`RaceId` bigint NOT NULL,
	`CurrentStamina` double NOT NULL DEFAULT '0',
 CONSTRAINT `PK_Bodies` PRIMARY KEY CLUSTERED 
(
	`Id` ASC
)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

/****** Object:  Table `Bodies_Accents`    Script Date: 20/09/2014 7:16:11 PM ******/
CREATE TABLE `Bodies_Accents`(
	`BodyId` bigint NOT NULL,
	`AccentId` bigint NOT NULL,
	`Familiarity` int NOT NULL,
 CONSTRAINT `PK_Bodies_Accents` PRIMARY KEY CLUSTERED 
(
	`BodyId` ASC,
	`AccentId` ASC
)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

/****** Object:  Table `Bodies_GameItems`    Script Date: 20/09/2014 7:16:11 PM ******/
CREATE TABLE `Bodies_GameItems`(
	`BodyId` bigint NOT NULL,
	`GameItemId` bigint NOT NULL,
	`EquippedOrder` int NOT NULL,
	`WearProfile` bigint NULL,
	`Wielded` int NULL,
 CONSTRAINT `PK_Bodies_GameItems` PRIMARY KEY CLUSTERED 
(
	`BodyId` ASC,
	`GameItemId` ASC
)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

/****** Object:  Table `Bodies_Languages`    Script Date: 20/09/2014 7:16:11 PM ******/
CREATE TABLE `Bodies_Languages`(
	`BodyId` bigint NOT NULL,
	`LanguageId` bigint NOT NULL,
 CONSTRAINT `PK_Bodies_Languages` PRIMARY KEY CLUSTERED 
(
	`BodyId` ASC,
	`LanguageId` ASC
)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

/****** Object:  Table `BodypartGroupDescribers`    Script Date: 20/09/2014 7:16:11 PM ******/
CREATE TABLE `BodypartGroupDescribers`(
	`Id` bigint AUTO_INCREMENT NOT NULL,
	`DescribedAs` varchar(4000) NOT NULL,
	`Comment` varchar(4000) NULL,
	`Type` varchar(50) NOT NULL,
 CONSTRAINT `PK_BodypartGroupDescriptionRule` PRIMARY KEY CLUSTERED 
(
	`Id` ASC
)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

/****** Object:  Table `BodypartGroupDescribers_BodypartProtos`    Script Date: 20/09/2014 7:16:11 PM ******/
CREATE TABLE `BodypartGroupDescribers_BodypartProtos`(
	`BodypartGroupDescriberId` bigint NOT NULL,
	`BodypartProtoId` bigint NOT NULL,
	`Mandatory` bit(1) NOT NULL DEFAULT 0,
PRIMARY KEY CLUSTERED 
(
	`BodypartGroupDescriberId` ASC,
	`BodypartProtoId` ASC
)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

/****** Object:  Table `BodypartGroupDescribers_BodyProtos`    Script Date: 20/09/2014 7:16:11 PM ******/
CREATE TABLE `BodypartGroupDescribers_BodyProtos`(
	`BodypartGroupDescriberId` bigint NOT NULL,
	`BodyProtoId` bigint NOT NULL,
PRIMARY KEY CLUSTERED 
(
	`BodypartGroupDescriberId` ASC,
	`BodyProtoId` ASC
)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

/****** Object:  Table `BodypartGroupDescribers_ShapeCount`    Script Date: 20/09/2014 7:16:11 PM ******/
CREATE TABLE `BodypartGroupDescribers_ShapeCount`(
	`MinCount` int NOT NULL,
	`BodypartGroupDescriptionRuleId` bigint NOT NULL,
	`TargetId` bigint NOT NULL,
	`MaxCount` int NOT NULL,
 CONSTRAINT `PK_BodypartGroupDescribers_ShapeCount` PRIMARY KEY CLUSTERED 
(
	`BodypartGroupDescriptionRuleId` ASC,
	`TargetId` ASC
)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

/****** Object:  Table `BodypartProto`    Script Date: 20/09/2014 7:16:11 PM ******/
CREATE TABLE `BodypartProto`(
	`Id` bigint AUTO_INCREMENT NOT NULL,
	`BodypartType` int NOT NULL,
	`BodyId` bigint NOT NULL,
	`Name` varchar(4000) NULL,
	`Description` varchar(4000) NULL,
	`BodypartShapeId` bigint NOT NULL,
	`DisplayOrder` int NULL,
	`MaxLife` int NOT NULL,
	`SeveredThreshold` int NOT NULL,
	`PainModifier` double NOT NULL,
	`Location` int NOT NULL,
	`Alignment` int NOT NULL,
	`Unary` bit(1) NULL,
	`MaxSingleSize` int NULL,
	`IsOrgan` int NOT NULL DEFAULT '0',
	`WeightLimit` double NOT NULL DEFAULT '0',
	`IsCore` bit(1) NOT NULL DEFAULT 1,
 CONSTRAINT `PK_BodypartProto` PRIMARY KEY CLUSTERED 
(
	`Id` ASC
)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

/****** Object:  Table `BodypartProto_AlignmentHits`    Script Date: 20/09/2014 7:16:11 PM ******/
CREATE TABLE `BodypartProto_AlignmentHits`(
	`BodypartProtoId` bigint NOT NULL,
	`Alignment` int NOT NULL,
	`HitChance` int NULL,
	`Id` bigint AUTO_INCREMENT NOT NULL,
 CONSTRAINT `PK_BodypartProto_AlignmentHits` PRIMARY KEY CLUSTERED 
(
	`Id` ASC
)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

/****** Object:  Table `BodypartProto_BodypartProto_Upstream`    Script Date: 20/09/2014 7:16:11 PM ******/
CREATE TABLE `BodypartProto_BodypartProto_Upstream`(
	`Child` bigint NOT NULL,
	`Parent` bigint NOT NULL,
 CONSTRAINT `PK_BodypartProto_BodypartProto_Upstream` PRIMARY KEY CLUSTERED 
(
	`Child` ASC,
	`Parent` ASC
)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

/****** Object:  Table `BodypartProto_OrientationHits`    Script Date: 20/09/2014 7:16:11 PM ******/
CREATE TABLE `BodypartProto_OrientationHits`(
	`BodypartProtoId` bigint NOT NULL,
	`Orientation` int NOT NULL,
	`HitChance` int NULL,
	`Id` bigint AUTO_INCREMENT NOT NULL,
 CONSTRAINT `PK_BodypartProto_OrientationHits` PRIMARY KEY CLUSTERED 
(
	`Id` ASC
)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

/****** Object:  Table `BodypartShape`    Script Date: 20/09/2014 7:16:11 PM ******/
CREATE TABLE `BodypartShape`(
	`Id` bigint AUTO_INCREMENT NOT NULL,
	`Name` varchar(4000) NOT NULL,
 CONSTRAINT `PK_BodypartShape` PRIMARY KEY CLUSTERED 
(
	`Id` ASC
)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

/****** Object:  Table `BodyProtos`    Script Date: 20/09/2014 7:16:11 PM ******/
CREATE TABLE `BodyProtos`(
	`Id` bigint AUTO_INCREMENT NOT NULL,
	`EntityDescriptionId` bigint NULL,
	`Name` varchar(4000) NULL,
	`WearSizeParameterId` bigint NOT NULL,
	`WielderDescriptionPlural` varchar(4000) NOT NULL DEFAULT 'hands',
	`WielderDescriptionSingle` varchar(4000) NOT NULL DEFAULT 'hand',
	`ConsiderString` varchar(4000) NOT NULL,
	`StaminaRecoveryProgId` bigint NULL,
 CONSTRAINT `PK_BodyPrototype` PRIMARY KEY CLUSTERED 
(
	`Id` ASC
)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

/****** Object:  Table `BodyProtos_AdditionalBodyparts`    Script Date: 20/09/2014 7:16:11 PM ******/
CREATE TABLE `BodyProtos_AdditionalBodyparts`(
	`BodyProtoId` bigint NOT NULL,
	`BodypartId` bigint NOT NULL,
	`Usage` varchar(50) NOT NULL,
PRIMARY KEY CLUSTERED 
(
	`BodyProtoId` ASC,
	`BodypartId` ASC,
	`Usage` ASC
)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

/****** Object:  Table `Calendars`    Script Date: 20/09/2014 7:16:11 PM ******/
CREATE TABLE `Calendars`(
	`Id` bigint AUTO_INCREMENT NOT NULL,
	`Definition` varchar(20000) NOT NULL,
	`Date` varchar(50) NOT NULL,
	`FeedClockId` bigint NOT NULL,
 CONSTRAINT `PK_Calendars` PRIMARY KEY CLUSTERED 
(
	`Id` ASC
)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

/****** Object:  Table `Celestials`    Script Date: 20/09/2014 7:16:11 PM ******/
CREATE TABLE `Celestials`(
	`Id` bigint AUTO_INCREMENT NOT NULL,
	`Definition` varchar(20000) NOT NULL,
	`Minutes` int NOT NULL,
	`FeedClockId` bigint NOT NULL,
	`CelestialYear` int NOT NULL,
	`LastYearBump` int NOT NULL,
 CONSTRAINT `PK_Celestials` PRIMARY KEY CLUSTERED 
(
	`Id` ASC
)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

/****** Object:  Table `CellOverlayPackages`    Script Date: 20/09/2014 7:16:11 PM ******/
CREATE TABLE `CellOverlayPackages`(
	`Id` bigint NOT NULL,
	`Name` varchar(4000) NOT NULL,
	`EditableItemId` bigint NOT NULL,
	`RevisionNumber` int NOT NULL,
 CONSTRAINT `PK_CellOverlayPackages` PRIMARY KEY CLUSTERED 
(
	`Id` ASC,
	`RevisionNumber` ASC
)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

/****** Object:  Table `CellOverlays`    Script Date: 20/09/2014 7:16:11 PM ******/
CREATE TABLE `CellOverlays`(
	`Id` bigint AUTO_INCREMENT NOT NULL,
	`Name` varchar(4000) NOT NULL,
	`CellName` varchar(4000) NOT NULL,
	`CellDescription` varchar(4000) NOT NULL,
	`CellOverlayPackageId` bigint NOT NULL,
	`CellId` bigint NOT NULL,
	`CellOverlayPackageRevisionNumber` int NOT NULL,
	`TerrainId` bigint NOT NULL,
	`HearingProfileId` bigint NULL,
	`OutdoorsType` int NOT NULL DEFAULT '0',
	`AmbientLightFactor` double NOT NULL DEFAULT '1.0',
	`AddedLight` double NOT NULL DEFAULT '0',
 CONSTRAINT `PK_CellOverlays` PRIMARY KEY CLUSTERED 
(
	`Id` ASC
)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

/****** Object:  Table `CellOverlays_Exits`    Script Date: 20/09/2014 7:16:11 PM ******/
CREATE TABLE `CellOverlays_Exits`(
	`CellOverlayId` bigint NOT NULL,
	`ExitId` bigint NOT NULL,
PRIMARY KEY CLUSTERED 
(
	`CellOverlayId` ASC,
	`ExitId` ASC
)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

/****** Object:  Table `Cells`    Script Date: 20/09/2014 7:16:11 PM ******/
CREATE TABLE `Cells`(
	`Id` bigint AUTO_INCREMENT NOT NULL,
	`RoomId` bigint NOT NULL,
	`CurrentOverlayId` bigint NULL,
 CONSTRAINT `PK_Cells` PRIMARY KEY CLUSTERED 
(
	`Id` ASC
)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

/****** Object:  Table `Cells_GameItems`    Script Date: 20/09/2014 7:16:11 PM ******/
CREATE TABLE `Cells_GameItems`(
	`CellId` bigint NOT NULL,
	`GameItemId` bigint NOT NULL,
 CONSTRAINT `PK_Cells_GameItems` PRIMARY KEY CLUSTERED 
(
	`CellId` ASC,
	`GameItemId` ASC
)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

/****** Object:  Table `ChannelCommandWords`    Script Date: 20/09/2014 7:16:11 PM ******/
CREATE TABLE `ChannelCommandWords`(
	`Word` varchar(50) NOT NULL,
	`ChannelId` bigint NOT NULL,
PRIMARY KEY CLUSTERED 
(
	`Word` ASC
)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

/****** Object:  Table `ChannelIgnorers`    Script Date: 20/09/2014 7:16:11 PM ******/
CREATE TABLE `ChannelIgnorers`(
	`ChannelId` bigint NOT NULL,
	`AccountId` bigint NOT NULL,
PRIMARY KEY CLUSTERED 
(
	`ChannelId` ASC,
	`AccountId` ASC
)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

/****** Object:  Table `Channels`    Script Date: 20/09/2014 7:16:11 PM ******/
CREATE TABLE `Channels`(
	`Id` bigint AUTO_INCREMENT NOT NULL,
	`ChannelName` varchar(4000) NOT NULL,
	`ChannelListenerProgId` bigint NOT NULL,
	`ChannelSpeakerProgId` bigint NOT NULL,
	`AnnounceChannelJoiners` bit(1) NOT NULL,
	`ChannelColour` char(10) NOT NULL,
	`Mode` int NOT NULL,
	`AnnounceMissedListeners` bit(1) NOT NULL,
	`AddToPlayerCommandTree` bit(1) NOT NULL DEFAULT 0,
	`AddToGuideCommandTree` bit(1) NOT NULL DEFAULT 0,
PRIMARY KEY CLUSTERED 
(
	`Id` ASC
)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

/****** Object:  Table `CharacteristicDefinitions`    Script Date: 20/09/2014 7:16:11 PM ******/
CREATE TABLE `CharacteristicDefinitions`(
	`Id` bigint AUTO_INCREMENT NOT NULL,
	`Name` varchar(4000) NOT NULL,
	`Type` int NOT NULL,
	`Pattern` varchar(4000) NOT NULL,
	`Description` varchar(4000) NOT NULL,
	`ParentId` bigint NULL,
	`ChargenDisplayType` int NULL,
 CONSTRAINT `PK_CharacteristicDefinitions` PRIMARY KEY CLUSTERED 
(
	`Id` ASC
)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

/****** Object:  Table `CharacteristicProfiles`    Script Date: 20/09/2014 7:16:11 PM ******/
CREATE TABLE `CharacteristicProfiles`(
	`Id` bigint AUTO_INCREMENT NOT NULL,
	`Name` varchar(50) NOT NULL,
	`Definition` varchar(4000) NOT NULL,
	`Type` varchar(50) NOT NULL,
	`TargetDefinitionId` bigint NOT NULL,
	`Description` varchar(4000) NOT NULL,
 CONSTRAINT `PK_CharacteristicProfiles` PRIMARY KEY CLUSTERED 
(
	`Id` ASC
)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

/****** Object:  Table `Characteristics`    Script Date: 20/09/2014 7:16:11 PM ******/
CREATE TABLE `Characteristics`(
	`BodyId` bigint NOT NULL,
	`CharacteristicId` bigint NOT NULL,
	`Type` int NOT NULL,
PRIMARY KEY CLUSTERED 
(
	`BodyId` ASC,
	`Type` ASC
)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

/****** Object:  Table `CharacteristicValues`    Script Date: 20/09/2014 7:16:11 PM ******/
CREATE TABLE `CharacteristicValues`(
	`Id` bigint AUTO_INCREMENT NOT NULL,
	`Name` varchar(4000) NOT NULL,
	`DefinitionId` bigint NOT NULL,
	`Value` varchar(50) NULL,
	`Default` bit(1) NOT NULL DEFAULT 0,
	`AdditionalValue` varchar(4000) NULL,
	`FutureProgId` bigint NULL,
 CONSTRAINT `PK_CharacteristicValues` PRIMARY KEY CLUSTERED 
(
	`Id` ASC
)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

/****** Object:  Table `Characters`    Script Date: 20/09/2014 7:16:11 PM ******/
CREATE TABLE `Characters`(
	`Name` varchar(4000) NOT NULL,
	`Id` bigint AUTO_INCREMENT NOT NULL,
	`AccountId` bigint NULL,
	`CreationTime` datetime NOT NULL,
	`DeathTime` datetime NULL,
	`Status` int NOT NULL DEFAULT '0',
	`State` int NOT NULL DEFAULT '0',
	`Gender` smallint NOT NULL DEFAULT '0',
	`Location` bigint NOT NULL,
	`BodyId` bigint NOT NULL,
	`CultureId` bigint NOT NULL,
	`BirthdayDate` varchar(4000) NOT NULL,
	`BirthdayCalendarId` bigint NOT NULL,
	`IsAdminAvatar` bit(1) NOT NULL DEFAULT 0,
	`CurrencyId` bigint NULL,
	`TotalMinutesPlayed` int NOT NULL DEFAULT '0',
	`PersonalNameId` bigint NOT NULL,
	`CurrentNameId` bigint NOT NULL,
	`AlcoholLitres` double NOT NULL DEFAULT '0',
	`WaterLitres` double NOT NULL DEFAULT '0',
	`FoodSatiatedHours` double NOT NULL DEFAULT '0',
	`DrinkSatiatedHours` double NOT NULL DEFAULT '0',
	`Calories` double NOT NULL DEFAULT '0',
	`NeedsModel` varchar(50) NOT NULL DEFAULT 'NoNeeds',
 CONSTRAINT `PK_Characters` PRIMARY KEY CLUSTERED 
(
	`Id` ASC
)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

/****** Object:  Table `Characters_Aliases`    Script Date: 20/09/2014 7:16:11 PM ******/
CREATE TABLE `Characters_Aliases`(
	`CharacterId` bigint NOT NULL,
	`PersonalNameId` bigint NOT NULL,
PRIMARY KEY CLUSTERED 
(
	`CharacterId` ASC,
	`PersonalNameId` ASC
)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

/****** Object:  Table `Characters_ChargenRoles`    Script Date: 20/09/2014 7:16:11 PM ******/
CREATE TABLE `Characters_ChargenRoles`(
	`CharacterId` bigint NOT NULL,
	`ChargenRoleId` bigint NOT NULL,
 CONSTRAINT `PK_Characters_ChargenRoles` PRIMARY KEY CLUSTERED 
(
	`CharacterId` ASC,
	`ChargenRoleId` ASC
)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

/****** Object:  Table `ChargenResources`    Script Date: 20/09/2014 7:16:11 PM ******/
CREATE TABLE `ChargenResources`(
	`Id` bigint AUTO_INCREMENT NOT NULL,
	`Name` varchar(50) NOT NULL,
	`PluralName` varchar(50) NOT NULL,
	`Alias` varchar(50) NOT NULL,
	`MinimumTimeBetweenAwards` int NOT NULL,
	`MaximumNumberAwardedPerAward` int NOT NULL,
	`PermissionLevelRequiredToAward` int NOT NULL,
	`PermissionLevelRequiredToCircumventMinimumTime` int NOT NULL,
	`ShowToPlayerInScore` bit(1) NOT NULL,
	`TextDisplayedToPlayerOnAward` varchar(4000) NOT NULL,
	`TextDisplayedToPlayerOnDeduct` varchar(4000) NOT NULL,
	`MaximumResourceId` bigint NULL,
	`MaximumResourceFormula` varchar(4000) NOT NULL,
	`Type` varchar(50) NOT NULL,
PRIMARY KEY CLUSTERED 
(
	`Id` ASC
)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

/****** Object:  Table `ChargenRoles`    Script Date: 20/09/2014 7:16:11 PM ******/
CREATE TABLE `ChargenRoles`(
	`Id` bigint AUTO_INCREMENT NOT NULL,
	`Name` varchar(4000) NOT NULL,
	`Type` int NOT NULL,
	`PosterId` bigint NOT NULL,
	`MaximumNumberAlive` int NOT NULL DEFAULT '0',
	`MaximumNumberTotal` int NOT NULL DEFAULT '0',
	`ChargenBlurb` varchar(4000) NOT NULL,
	`AvailabilityProgId` bigint NULL,
	`Expired` bit(1) NOT NULL DEFAULT 0,
	`MinimumAuthorityToApprove` int NOT NULL DEFAULT '0',
	`MinimumAuthorityToView` int NOT NULL DEFAULT '0',
PRIMARY KEY CLUSTERED 
(
	`Id` ASC
)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

/****** Object:  Table `ChargenRoles_Approvers`    Script Date: 20/09/2014 7:16:11 PM ******/
CREATE TABLE `ChargenRoles_Approvers`(
	`ChargenRoleId` bigint NOT NULL,
	`ApproverId` bigint NOT NULL,
PRIMARY KEY CLUSTERED 
(
	`ChargenRoleId` ASC,
	`ApproverId` ASC
)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

/****** Object:  Table `ChargenRoles_ClanMemberships`    Script Date: 20/09/2014 7:16:11 PM ******/
CREATE TABLE `ChargenRoles_ClanMemberships`(
	`ChargenRoleId` bigint NOT NULL,
	`ClanId` bigint NOT NULL,
	`RankId` bigint NOT NULL,
	`PaygradeId` bigint NULL,
PRIMARY KEY CLUSTERED 
(
	`ChargenRoleId` ASC,
	`ClanId` ASC
)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

/****** Object:  Table `ChargenRoles_ClanMemberships_Appointments`    Script Date: 20/09/2014 7:16:11 PM ******/
CREATE TABLE `ChargenRoles_ClanMemberships_Appointments`(
	`ChargenRoleId` bigint NOT NULL,
	`ClanId` bigint NOT NULL,
	`AppointmentId` bigint NOT NULL,
PRIMARY KEY CLUSTERED 
(
	`ChargenRoleId` ASC,
	`ClanId` ASC,
	`AppointmentId` ASC
)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

/****** Object:  Table `ChargenRoles_Costs`    Script Date: 20/09/2014 7:16:11 PM ******/
CREATE TABLE `ChargenRoles_Costs`(
	`ChargenRoleId` bigint NOT NULL,
	`ChargenResourceId` bigint NOT NULL,
	`Amount` int NOT NULL,
PRIMARY KEY CLUSTERED 
(
	`ChargenRoleId` ASC,
	`ChargenResourceId` ASC
)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

/****** Object:  Table `ChargenRoles_Currencies`    Script Date: 20/09/2014 7:16:11 PM ******/
CREATE TABLE `ChargenRoles_Currencies`(
	`ChargenRoleId` bigint NOT NULL,
	`CurrencyId` bigint NOT NULL,
	`Amount` decimal(18, 0) NOT NULL,
PRIMARY KEY CLUSTERED 
(
	`ChargenRoleId` ASC,
	`CurrencyId` ASC
)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

/****** Object:  Table `ChargenRoles_Traits`    Script Date: 20/09/2014 7:16:11 PM ******/
CREATE TABLE `ChargenRoles_Traits`(
	`ChargenRoleId` bigint NOT NULL,
	`TraitId` bigint NOT NULL,
	`Amount` double NOT NULL,
PRIMARY KEY CLUSTERED 
(
	`ChargenRoleId` ASC,
	`TraitId` ASC
)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

/****** Object:  Table `Chargens`    Script Date: 20/09/2014 7:16:11 PM ******/
CREATE TABLE `Chargens`(
	`Id` bigint AUTO_INCREMENT NOT NULL,
	`AccountId` bigint NOT NULL,
	`Name` varchar(4000) NOT NULL,
	`Definition` varchar(4000) NOT NULL,
	`Status` int NOT NULL,
	`SubmitTime` datetime NULL,
	`MinimumApprovalAuthority` int NULL,
	`ApprovedById` bigint NULL,
	`ApprovalTime` datetime NULL,
PRIMARY KEY CLUSTERED 
(
	`Id` ASC
)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

/****** Object:  Table `Checks`    Script Date: 20/09/2014 7:16:11 PM ******/
CREATE TABLE `Checks`(
	`Type` int NOT NULL,
	`TraitExpressionId` bigint NOT NULL,
	`CheckTemplateId` bigint NOT NULL,
 CONSTRAINT `PK_Checks` PRIMARY KEY CLUSTERED 
(
	`Type` ASC
)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

/****** Object:  Table `CheckTemplateDifficulties`    Script Date: 20/09/2014 7:16:11 PM ******/
CREATE TABLE `CheckTemplateDifficulties`(
	`CheckTemplateId` bigint NOT NULL,
	`Difficulty` int NOT NULL,
	`Modifier` double NOT NULL,
PRIMARY KEY CLUSTERED 
(
	`Difficulty` ASC,
	`CheckTemplateId` ASC
)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

/****** Object:  Table `CheckTemplates`    Script Date: 20/09/2014 7:16:11 PM ******/
CREATE TABLE `CheckTemplates`(
	`Id` bigint AUTO_INCREMENT NOT NULL,
	`Name` varchar(4000) NOT NULL,
	`Definition` varchar(4000) NULL,
	`CheckMethod` varchar(25) NOT NULL DEFAULT 'Standard',
	`ImproveTraits` bit(1) NOT NULL DEFAULT 0,
	`FailIfTraitMissingMode` smallint NOT NULL DEFAULT '0',
	`CanBranchIfTraitMissing` bit(1) NOT NULL DEFAULT 0,
PRIMARY KEY CLUSTERED 
(
	`Id` ASC
)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

/****** Object:  Table `ClanMemberships`    Script Date: 20/09/2014 7:16:11 PM ******/
CREATE TABLE `ClanMemberships`(
	`ClanId` bigint NOT NULL,
	`CharacterId` bigint NOT NULL,
	`RankId` bigint NOT NULL,
	`PaygradeId` bigint NULL,
	`JoinDate` varchar(100) NOT NULL,
	`ManagerId` bigint NULL,
	`PersonalNameId` bigint NOT NULL,
PRIMARY KEY CLUSTERED 
(
	`ClanId` ASC,
	`CharacterId` ASC
)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

/****** Object:  Table `ClanMemberships_Appointments`    Script Date: 20/09/2014 7:16:11 PM ******/
CREATE TABLE `ClanMemberships_Appointments`(
	`ClanId` bigint NOT NULL,
	`CharacterId` bigint NOT NULL,
	`AppointmentId` bigint NOT NULL,
 CONSTRAINT `PK_ClanMemberships_Appointments` PRIMARY KEY CLUSTERED 
(
	`ClanId` ASC,
	`CharacterId` ASC,
	`AppointmentId` ASC
)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

/****** Object:  Table `ClanMemberships_Backpay`    Script Date: 20/09/2014 7:16:11 PM ******/
CREATE TABLE `ClanMemberships_Backpay`(
	`ClanId` bigint NOT NULL,
	`CharacterId` bigint NOT NULL,
	`CurrencyId` bigint NOT NULL,
	`Amount` decimal(18, 2) NOT NULL,
PRIMARY KEY CLUSTERED 
(
	`CurrencyId` ASC,
	`ClanId` ASC,
	`CharacterId` ASC
)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

/****** Object:  Table `Clans`    Script Date: 20/09/2014 7:16:11 PM ******/
CREATE TABLE `Clans`(
	`Id` bigint AUTO_INCREMENT NOT NULL,
	`Name` varchar(100) NOT NULL,
	`Alias` varchar(100) NOT NULL,
	`FullName` varchar(4000) NULL,
	`Description` varchar(4000) NULL,
	`ParentClanId` bigint NULL,
	`PayIntervalType` int NOT NULL,
	`PayIntervalModifier` int NOT NULL,
	`PayIntervalOther` int NOT NULL,
	`CalendarId` bigint NOT NULL,
	`PayIntervalReferenceDate` varchar(100) NOT NULL,
	`PayIntervalReferenceTime` varchar(100) NOT NULL,
	`IsTemplate` bit(1) NOT NULL DEFAULT 0,
PRIMARY KEY CLUSTERED 
(
	`Id` ASC
)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

/****** Object:  Table `Clocks`    Script Date: 20/09/2014 7:16:11 PM ******/
CREATE TABLE `Clocks`(
	`Id` bigint AUTO_INCREMENT NOT NULL,
	`Definition` varchar(4000) NOT NULL,
	`Seconds` int NOT NULL,
	`Minutes` int NOT NULL,
	`Hours` int NOT NULL,
	`PrimaryTimezoneId` bigint NOT NULL DEFAULT 0,
 CONSTRAINT `PK_Clocks` PRIMARY KEY CLUSTERED 
(
	`Id` ASC
)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

/****** Object:  Table `Coins`    Script Date: 20/09/2014 7:16:11 PM ******/
CREATE TABLE `Coins`(
	`Id` bigint AUTO_INCREMENT NOT NULL,
	`Name` varchar(4000) NOT NULL,
	`ShortDescription` varchar(4000) NOT NULL,
	`FullDescription` varchar(4000) NOT NULL,
	`Value` decimal(18, 0) NOT NULL,
	`CurrencyId` bigint NOT NULL,
	`Weight` double NOT NULL,
	`GeneralForm` varchar(4000) NOT NULL,
	`PluralWord` varchar(4000) NULL,
PRIMARY KEY CLUSTERED 
(
	`Id` ASC
)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

/****** Object:  Table `Colours`    Script Date: 20/09/2014 7:16:11 PM ******/
CREATE TABLE `Colours`(
	`Id` bigint AUTO_INCREMENT NOT NULL,
	`Name` varchar(50) NOT NULL,
	`Basic` int NOT NULL,
	`Red` int NOT NULL,
	`Green` int NOT NULL,
	`Blue` int NOT NULL,
	`Fancy` varchar(4000) NULL,
 CONSTRAINT `PK_Colours` PRIMARY KEY CLUSTERED 
(
	`Id` ASC
)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

/****** Object:  Table `CombatMoves`    Script Date: 20/09/2014 7:16:11 PM ******/
CREATE TABLE `CombatMoves`(
	`Name` varchar(4000) NOT NULL,
	`Id` bigint AUTO_INCREMENT NOT NULL,
	`Output` varchar(4000) NOT NULL,
 CONSTRAINT `PK_CombatMoves` PRIMARY KEY CLUSTERED 
(
	`Id` ASC
)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

/****** Object:  Table `CombatMoves_DamagePatterns`    Script Date: 20/09/2014 7:16:11 PM ******/
CREATE TABLE `CombatMoves_DamagePatterns`(
	`CombatMoveId` bigint NOT NULL,
	`DamageTemplateId` bigint NOT NULL,
	`Chance` double NOT NULL,
 CONSTRAINT `PK_CombatMoves_DamageTemplates` PRIMARY KEY CLUSTERED 
(
	`CombatMoveId` ASC,
	`DamageTemplateId` ASC
)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

/****** Object:  Table `CultureInfos`    Script Date: 20/09/2014 7:16:11 PM ******/
CREATE TABLE `CultureInfos`(
	`Id` varchar(50) NOT NULL,
	`DisplayName` varchar(4000) NOT NULL,
	`Order` int NOT NULL,
PRIMARY KEY CLUSTERED 
(
	`Id` ASC
)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

/****** Object:  Table `Cultures`    Script Date: 20/09/2014 7:16:11 PM ******/
CREATE TABLE `Cultures`(
	`Id` bigint AUTO_INCREMENT NOT NULL,
	`Name` varchar(255) NOT NULL,
	`Description` varchar(4000) NOT NULL,
	`NameCultureId` bigint NOT NULL,
	`PersonWordMale` varchar(255) NULL,
	`PersonWordFemale` varchar(255) NULL,
	`PersonWordNeuter` varchar(255) NULL,
	`PersonWordIndeterminate` varchar(255) NOT NULL,
	`PrimaryCalendarId` bigint NOT NULL,
	`SkillStartingValueProgId` bigint NOT NULL,
	`AvailabilityProgId` bigint NULL,
 CONSTRAINT `PK_Cultures` PRIMARY KEY CLUSTERED 
(
	`Id` ASC
)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

/****** Object:  Table `Cultures_ChargenResources`    Script Date: 20/09/2014 7:16:11 PM ******/
CREATE TABLE `Cultures_ChargenResources`(
	`CultureId` bigint NOT NULL,
	`ChargenResourceId` bigint NOT NULL,
	`Amount` int NOT NULL,
 CONSTRAINT `PK_Cultures_ChargenResources` PRIMARY KEY CLUSTERED 
(
	`CultureId` ASC,
	`ChargenResourceId` ASC
)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

/****** Object:  Table `Currencies`    Script Date: 20/09/2014 7:16:11 PM ******/
CREATE TABLE `Currencies`(
	`Id` bigint AUTO_INCREMENT NOT NULL,
	`Name` varchar(4000) NOT NULL,
PRIMARY KEY CLUSTERED 
(
	`Id` ASC
)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

/****** Object:  Table `CurrencyDescriptionPatternElements`    Script Date: 20/09/2014 7:16:11 PM ******/
CREATE TABLE `CurrencyDescriptionPatternElements`(
	`Id` bigint AUTO_INCREMENT NOT NULL,
	`Pattern` varchar(4000) NOT NULL,
	`Order` int NOT NULL,
	`ShowIfZero` bit(1) NOT NULL,
	`CurrencyDivisionId` bigint NOT NULL,
	`CurrencyDescriptionPatternId` bigint NOT NULL,
	`PluraliseWord` varchar(4000) NOT NULL,
	`AlternatePattern` varchar(4000) NULL,
	`RoundingMode` int NOT NULL,
	`SpecialValuesOverrideFormat` bit(1) NOT NULL DEFAULT 0,
PRIMARY KEY CLUSTERED 
(
	`Id` ASC
)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

/****** Object:  Table `CurrencyDescriptionPatternElementSpecialValues`    Script Date: 20/09/2014 7:16:11 PM ******/
CREATE TABLE `CurrencyDescriptionPatternElementSpecialValues`(
	`Value` decimal(18, 0) NOT NULL,
	`Text` varchar(4000) NOT NULL,
	`CurrencyDescriptionPatternElementId` bigint NOT NULL,
PRIMARY KEY CLUSTERED 
(
	`Value` ASC,
	`CurrencyDescriptionPatternElementId` ASC
)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

/****** Object:  Table `CurrencyDescriptionPatterns`    Script Date: 20/09/2014 7:16:11 PM ******/
CREATE TABLE `CurrencyDescriptionPatterns`(
	`Id` bigint AUTO_INCREMENT NOT NULL,
	`Type` int NOT NULL,
	`CurrencyId` bigint NOT NULL,
	`FutureProgId` bigint NULL,
	`NegativePrefix` varchar(4000) NOT NULL,
	`Order` int NOT NULL,
PRIMARY KEY CLUSTERED 
(
	`Id` ASC
)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

/****** Object:  Table `CurrencyDivisionAbbreviations`    Script Date: 20/09/2014 7:16:11 PM ******/
CREATE TABLE `CurrencyDivisionAbbreviations`(
	`Pattern` varchar(150) NOT NULL,
	`CurrencyDivisionId` bigint NOT NULL,
PRIMARY KEY CLUSTERED 
(
	`Pattern` ASC,
	`CurrencyDivisionId` ASC
)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

/****** Object:  Table `CurrencyDivisions`    Script Date: 20/09/2014 7:16:11 PM ******/
CREATE TABLE `CurrencyDivisions`(
	`Id` bigint AUTO_INCREMENT NOT NULL,
	`Name` varchar(4000) NOT NULL,
	`BaseUnitConversionRate` decimal(18, 4) NOT NULL,
	`CurrencyId` bigint NOT NULL,
PRIMARY KEY CLUSTERED 
(
	`Id` ASC
)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

/****** Object:  Table `DamagePatterns`    Script Date: 20/09/2014 7:16:11 PM ******/
CREATE TABLE `DamagePatterns`(
	`Name` varchar(4000) NOT NULL,
	`Id` bigint AUTO_INCREMENT NOT NULL,
	`DamageType` int NOT NULL,
	`Dice` int NOT NULL,
	`Sides` int NOT NULL,
	`Bonus` int NOT NULL,
 CONSTRAINT `PK_DamageTemplates` PRIMARY KEY CLUSTERED 
(
	`Id` ASC
)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

/****** Object:  Table `Doors`    Script Date: 20/09/2014 7:16:11 PM ******/
CREATE TABLE `Doors`(
	`Name` varchar(4000) NOT NULL,
	`Id` bigint AUTO_INCREMENT NOT NULL,
	`Style` int NOT NULL,
	`IsOpen` bit(1) NOT NULL,
	`LockedWith` bigint NULL,
 CONSTRAINT `PK_Doors` PRIMARY KEY CLUSTERED 
(
	`Id` ASC
)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

/****** Object:  Table `Dubs`    Script Date: 20/09/2014 7:16:11 PM ******/
CREATE TABLE `Dubs`(
	`Id` bigint AUTO_INCREMENT NOT NULL,
	`Keywords` varchar(4000) NOT NULL,
	`TargetId` bigint NOT NULL,
	`TargetType` varchar(100) NOT NULL,
	`LastDescription` varchar(4000) NOT NULL,
	`LastUsage` datetime NOT NULL,
	`CharacterId` bigint NOT NULL,
PRIMARY KEY CLUSTERED 
(
	`Id` ASC
)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

/****** Object:  Table `EditableItems`    Script Date: 20/09/2014 7:16:11 PM ******/
CREATE TABLE `EditableItems`(
	`Id` bigint AUTO_INCREMENT NOT NULL,
	`RevisionNumber` int NOT NULL,
	`RevisionStatus` int NOT NULL,
	`BuilderAccountId` bigint NOT NULL,
	`ReviewerAccountId` bigint NULL,
	`BuilderComment` varchar(4000) NULL,
	`ReviewerComment` varchar(4000) NULL,
	`BuilderDate` datetime NOT NULL,
	`ReviewerDate` datetime NULL,
	`ObsoleteDate` datetime NULL,
 CONSTRAINT `PK_EditableItems_1` PRIMARY KEY CLUSTERED 
(
	`Id` ASC
)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

/****** Object:  Table `Effects`    Script Date: 20/09/2014 7:16:11 PM ******/
CREATE TABLE `Effects`(
	`Id` bigint AUTO_INCREMENT NOT NULL,
	`OwnerId` bigint NOT NULL,
	`OwnerType` varchar(50) NOT NULL,
	`EffectType` varchar(50) NOT NULL,
	`Definition` varchar(4000) NOT NULL,
	`FutureProgId` bigint NULL,
	`OriginalDurationMilliseconds` bigint NULL,
	`CurrentDurationMilliseconds` bigint NULL,
PRIMARY KEY CLUSTERED 
(
	`Id` ASC
)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

/****** Object:  Table `EmailTemplates`    Script Date: 20/09/2014 7:16:11 PM ******/
CREATE TABLE `EmailTemplates`(
	`TemplateType` int NOT NULL,
	`Content` varchar(4000) NOT NULL,
	`Subject` varchar(4000) NOT NULL,
	`ReturnAddress` varchar(4000) NOT NULL,
PRIMARY KEY CLUSTERED 
(
	`TemplateType` ASC
)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

/****** Object:  Table `EntityDescriptionPatterns`    Script Date: 20/09/2014 7:16:11 PM ******/
CREATE TABLE `EntityDescriptionPatterns`(
	`Id` bigint AUTO_INCREMENT NOT NULL,
	`Pattern` varchar(4000) NOT NULL,
	`Type` int NOT NULL,
	`ApplicabilityProgId` bigint NULL,
	`RelativeWeight` int NOT NULL,
PRIMARY KEY CLUSTERED 
(
	`Id` ASC
)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

/****** Object:  Table `EntityDescriptionPatterns_EntityDescriptions`    Script Date: 20/09/2014 7:16:11 PM ******/
CREATE TABLE `EntityDescriptionPatterns_EntityDescriptions`(
	`PatternId` bigint NOT NULL,
	`EntityDescriptionId` bigint NOT NULL,
PRIMARY KEY CLUSTERED 
(
	`PatternId` ASC,
	`EntityDescriptionId` ASC
)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

/****** Object:  Table `EntityDescriptions`    Script Date: 20/09/2014 7:16:11 PM ******/
CREATE TABLE `EntityDescriptions`(
	`Id` bigint AUTO_INCREMENT NOT NULL,
	`ShortDescription` varchar(4000) NULL,
	`FullDescription` varchar(4000) NULL,
	`DisplaySex` smallint NOT NULL DEFAULT '0',
 CONSTRAINT `PK_CharacterDescriptions` PRIMARY KEY CLUSTERED 
(
	`Id` ASC
)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

/****** Object:  Table `Ethnicities`    Script Date: 20/09/2014 7:16:11 PM ******/
CREATE TABLE `Ethnicities`(
	`Id` bigint AUTO_INCREMENT NOT NULL,
	`Name` varchar(4000) NOT NULL,
	`ChargenBlurb` varchar(4000) NOT NULL,
	`AvailabilityProgId` bigint NULL,
PRIMARY KEY CLUSTERED 
(
	`Id` ASC
)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

/****** Object:  Table `Ethnicities_Characteristics`    Script Date: 20/09/2014 7:16:11 PM ******/
CREATE TABLE `Ethnicities_Characteristics`(
	`EthnicityId` bigint NOT NULL,
	`CharacteristicDefinitionId` bigint NOT NULL,
	`CharacteristicProfileId` bigint NOT NULL,
PRIMARY KEY CLUSTERED 
(
	`EthnicityId` ASC,
	`CharacteristicDefinitionId` ASC,
	`CharacteristicProfileId` ASC
)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

/****** Object:  Table `Ethnicities_ChargenResources`    Script Date: 20/09/2014 7:16:11 PM ******/
CREATE TABLE `Ethnicities_ChargenResources`(
	`EthnicityId` bigint NOT NULL,
	`ChargenResourceId` bigint NOT NULL,
	`Amount` int NOT NULL,
 CONSTRAINT `PK_Ethnicities_ChargenResources` PRIMARY KEY CLUSTERED 
(
	`EthnicityId` ASC,
	`ChargenResourceId` ASC
)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

/****** Object:  Table `Exits`    Script Date: 20/09/2014 7:16:11 PM ******/
CREATE TABLE `Exits`(
	`Id` bigint AUTO_INCREMENT NOT NULL,
	`Keywords1` varchar(255) NULL,
	`Keywords2` varchar(255) NULL,
	`CellId1` bigint NOT NULL,
	`CellId2` bigint NOT NULL,
	`DoorId` bigint NULL,
	`Direction1` int NOT NULL,
	`Direction2` int NOT NULL,
	`TimeMultiplier` double NOT NULL,
	`InboundDescription1` varchar(255) NULL,
	`InboundDescription2` varchar(255) NULL,
	`OutboundDescription1` varchar(255) NULL,
	`OutboundDescription2` varchar(255) NULL,
	`InboundTarget1` varchar(255) NULL,
	`InboundTarget2` varchar(255) NULL,
	`OutboundTarget1` varchar(255) NULL,
	`OutboundTarget2` varchar(255) NULL,
	`Verb1` varchar(255) NULL,
	`Verb2` varchar(255) NULL,
	`PrimaryKeyword1` varchar(255) NULL,
	`PrimaryKeyword2` varchar(255) NULL,
	`AcceptsDoor` bit(1) NOT NULL,
	`DoorSize` int NULL,
 CONSTRAINT `PK_Exits` PRIMARY KEY CLUSTERED 
(
	`Id` ASC
)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

/****** Object:  Table `ExternalClanControls`    Script Date: 20/09/2014 7:16:11 PM ******/
CREATE TABLE `ExternalClanControls`(
	`VassalClanId` bigint NOT NULL,
	`LiegeClanId` bigint NOT NULL,
	`ControlledAppointmentId` bigint NOT NULL,
	`ControllingAppointmentId` bigint NULL,
	`NumberOfAppointments` int NOT NULL,
PRIMARY KEY CLUSTERED 
(
	`VassalClanId` ASC,
	`LiegeClanId` ASC,
	`ControlledAppointmentId` ASC
)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

/****** Object:  Table `ExternalClanControls_Appointments`    Script Date: 20/09/2014 7:16:11 PM ******/
CREATE TABLE `ExternalClanControls_Appointments`(
	`VassalClanId` bigint NOT NULL,
	`LiegeClanId` bigint NOT NULL,
	`ControlledAppointmentId` bigint NOT NULL,
	`CharacterId` bigint NOT NULL,
PRIMARY KEY CLUSTERED 
(
	`CharacterId` ASC,
	`VassalClanId` ASC,
	`LiegeClanId` ASC,
	`ControlledAppointmentId` ASC
)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

/****** Object:  Table `FutureProgs`    Script Date: 20/09/2014 7:16:11 PM ******/
CREATE TABLE `FutureProgs`(
	`Id` bigint AUTO_INCREMENT NOT NULL,
	`FunctionName` varchar(255) NOT NULL,
	`FunctionComment` varchar(1000) NOT NULL,
	`FunctionText` varchar(10000) NOT NULL,
	`ReturnType` bigint NOT NULL,
	`Category` varchar(255) NOT NULL,
	`Subcategory` varchar(255) NOT NULL,
	`Public` bit(1) NOT NULL DEFAULT 0,
	`AcceptsAnyParameters` bit(1) NOT NULL DEFAULT 0,
 CONSTRAINT `PK_FutureProgs` PRIMARY KEY CLUSTERED 
(
	`Id` ASC
)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

/****** Object:  Table `FutureProgs_Parameters`    Script Date: 20/09/2014 7:16:11 PM ******/
CREATE TABLE `FutureProgs_Parameters`(
	`FutureProgId` bigint NOT NULL,
	`ParameterIndex` int NOT NULL,
	`ParameterType` bigint NOT NULL,
	`ParameterName` varchar(4000) NOT NULL,
PRIMARY KEY CLUSTERED 
(
	`FutureProgId` ASC,
	`ParameterIndex` ASC
)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

/****** Object:  Table `GameItemComponentProtos`    Script Date: 20/09/2014 7:16:11 PM ******/
CREATE TABLE `GameItemComponentProtos`(
	`Id` bigint NOT NULL,
	`Type` varchar(50) NOT NULL,
	`Definition` varchar(4000) NOT NULL,
	`EditableItemId` bigint NOT NULL,
	`Description` varchar(4000) NOT NULL,
	`RevisionNumber` int NOT NULL,
	`Name` varchar(4000) NOT NULL,
 CONSTRAINT `PK_GameItemComponentProtos` PRIMARY KEY CLUSTERED 
(
	`Id` ASC,
	`RevisionNumber` ASC
)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

/****** Object:  Table `GameItemComponents`    Script Date: 20/09/2014 7:16:11 PM ******/
CREATE TABLE `GameItemComponents`(
	`Id` bigint AUTO_INCREMENT NOT NULL,
	`GameItemComponentProtoId` bigint NOT NULL,
	`GameItemComponentProtoRevision` int NOT NULL,
	`Definition` varchar(4000) NOT NULL,
	`GameItemId` bigint NOT NULL,
 CONSTRAINT `PK_GameItemComponents` PRIMARY KEY CLUSTERED 
(
	`Id` ASC
)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

/****** Object:  Table `GameItemProtos`    Script Date: 20/09/2014 7:16:11 PM ******/
CREATE TABLE `GameItemProtos`(
	`Id` bigint NOT NULL,
	`Name` varchar(255) NOT NULL,
	`Keywords` varchar(4000) NOT NULL,
	`MaterialId` bigint NOT NULL,
	`EntityDescriptionId` bigint NOT NULL,
	`EditableItemId` bigint NOT NULL,
	`RevisionNumber` int NOT NULL,
	`Size` int NOT NULL,
	`Weight` double NOT NULL DEFAULT '0.00',
	`ReadOnly` bit(1) NOT NULL DEFAULT 0,
 CONSTRAINT `PK_GameItemProtos` PRIMARY KEY CLUSTERED 
(
	`Id` ASC,
	`RevisionNumber` ASC
)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

/****** Object:  Table `GameItemProtos_GameItemComponentProtos`    Script Date: 20/09/2014 7:16:11 PM ******/
CREATE TABLE `GameItemProtos_GameItemComponentProtos`(
	`GameItemProtoId` bigint NOT NULL,
	`GameItemComponentProtoId` bigint NOT NULL,
	`GameItemProtoRevision` int NOT NULL,
	`GameItemComponentRevision` int NOT NULL,
 CONSTRAINT `PK_GameItemProtos_GameItemComponentProtos` PRIMARY KEY CLUSTERED 
(
	`GameItemProtoId` ASC,
	`GameItemComponentProtoId` ASC,
	`GameItemProtoRevision` ASC,
	`GameItemComponentRevision` ASC
)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

/****** Object:  Table `GameItemProtos_Tags`    Script Date: 20/09/2014 7:16:11 PM ******/
CREATE TABLE `GameItemProtos_Tags`(
	`GameItemProtoId` bigint NOT NULL,
	`TagId` bigint NOT NULL,
	`GameItemProtoRevisionNumber` int NOT NULL,
PRIMARY KEY CLUSTERED 
(
	`GameItemProtoId` ASC,
	`TagId` ASC,
	`GameItemProtoRevisionNumber` ASC
)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

/****** Object:  Table `GameItems`    Script Date: 20/09/2014 7:16:11 PM ******/
CREATE TABLE `GameItems`(
	`Id` bigint AUTO_INCREMENT NOT NULL,
	`Quality` int NOT NULL,
	`GameItemProtoId` bigint NOT NULL,
	`GameItemProtoRevision` int NOT NULL,
	`MaterialId` bigint NOT NULL,
	`Size` int NOT NULL,
 CONSTRAINT `PK_GameItems_1` PRIMARY KEY CLUSTERED 
(
	`Id` ASC
)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

/****** Object:  Table `HearingProfiles`    Script Date: 20/09/2014 7:16:11 PM ******/
CREATE TABLE `HearingProfiles`(
	`Id` bigint AUTO_INCREMENT NOT NULL,
	`Name` varchar(4000) NOT NULL,
	`Definition` varchar(4000) NOT NULL,
	`Type` varchar(4000) NOT NULL,
	`SurveyDescription` varchar(4000) NOT NULL,
PRIMARY KEY CLUSTERED 
(
	`Id` ASC
)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

/****** Object:  Table `HeightWeightModels`    Script Date: 20/09/2014 7:16:11 PM ******/
CREATE TABLE `HeightWeightModels`(
	`Id` bigint AUTO_INCREMENT NOT NULL,
	`Name` varchar(4000) NOT NULL,
	`MeanHeight` double NOT NULL,
	`MeanBMI` double NOT NULL,
	`StddevHeight` double NOT NULL,
	`StddevBMI` double NOT NULL,
	`BMIMultiplier` double NOT NULL,
PRIMARY KEY CLUSTERED 
(
	`Id` ASC
)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

/****** Object:  Table `Helpfiles`    Script Date: 20/09/2014 7:16:11 PM ******/
CREATE TABLE `Helpfiles`(
	`Id` bigint AUTO_INCREMENT NOT NULL,
	`Name` varchar(255) NOT NULL,
	`Category` varchar(255) NOT NULL,
	`Subcategory` varchar(255) NOT NULL,
	`TagLine` varchar(500) NOT NULL,
	`PublicText` varchar(10000) NOT NULL,
	`RuleId` bigint NULL,
	`Keywords` varchar(500) NOT NULL,
	`LastEditedBy` varchar(100) NOT NULL,
	`LastEditedDate` datetime NOT NULL,
 CONSTRAINT `PK_Helpfiles` PRIMARY KEY CLUSTERED 
(
	`Id` ASC
)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

/****** Object:  Table `Helpfiles_ExtraTexts`    Script Date: 20/09/2014 7:16:11 PM ******/
CREATE TABLE `Helpfiles_ExtraTexts`(
	`HelpfileId` bigint NOT NULL,
	`Text` varchar(4000) NOT NULL,
	`RuleId` bigint NOT NULL,
	`DisplayOrder` int NOT NULL,
PRIMARY KEY CLUSTERED 
(
	`HelpfileId` ASC,
	`DisplayOrder` ASC
)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

/****** Object:  Table `Hooks`    Script Date: 20/09/2014 7:16:11 PM ******/
CREATE TABLE `Hooks`(
	`Id` bigint AUTO_INCREMENT NOT NULL,
	`Name` varchar(255) NOT NULL,
	`Definition` varchar(10000) NOT NULL,
	`Type` varchar(255) NOT NULL,
	`Category` varchar(255) NOT NULL,
	`TargetEventType` int NOT NULL,
PRIMARY KEY CLUSTERED 
(
	`Id` ASC
)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

/****** Object:  Table `Improvers`    Script Date: 20/09/2014 7:16:11 PM ******/
CREATE TABLE `Improvers`(
	`Id` bigint AUTO_INCREMENT NOT NULL,
	`Name` varchar(255) NOT NULL,
	`Definition` varchar(10000) NOT NULL,
	`Type` varchar(50) NOT NULL,
 CONSTRAINT `PK_Improvers` PRIMARY KEY CLUSTERED 
(
	`Id` ASC
)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

/****** Object:  Table `LanguageDifficultyModels`    Script Date: 20/09/2014 7:16:11 PM ******/
CREATE TABLE `LanguageDifficultyModels`(
	`Id` bigint AUTO_INCREMENT NOT NULL,
	`Definition` varchar(10000) NOT NULL,
	`Type` varchar(255) NOT NULL,
	`Name` varchar(255) NOT NULL,
PRIMARY KEY CLUSTERED 
(
	`Id` ASC
)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

/****** Object:  Table `Languages`    Script Date: 20/09/2014 7:16:11 PM ******/
CREATE TABLE `Languages`(
	`Id` bigint AUTO_INCREMENT NOT NULL,
	`DifficultyModel` bigint NOT NULL,
	`LinkedTraitId` bigint NOT NULL,
	`UnknownLanguageDescription` varchar(255) NOT NULL,
	`LanguageObfuscationFactor` double NOT NULL,
	`Name` varchar(255) NOT NULL,
 CONSTRAINT `PK_Languages` PRIMARY KEY CLUSTERED 
(
	`Id` ASC
)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

/****** Object:  Table `Locks`    Script Date: 20/09/2014 7:16:11 PM ******/
CREATE TABLE `Locks`(
	`Name` varchar(255) NOT NULL,
	`Id` bigint AUTO_INCREMENT NOT NULL,
	`Style` int NOT NULL,
	`Strength` int NOT NULL,
 CONSTRAINT `PK_Locks` PRIMARY KEY CLUSTERED 
(
	`Id` ASC
)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

/****** Object:  Table `LoginIPs`    Script Date: 20/09/2014 7:16:11 PM ******/
CREATE TABLE `LoginIPs`(
	`IpAddress` varchar(200) NOT NULL,
	`AccountId` bigint NOT NULL,
	`FirstDate` datetime NOT NULL,
	`AccountRegisteredOnThisIP` bit(1) NOT NULL DEFAULT 0,
PRIMARY KEY CLUSTERED 
(
	`IpAddress` ASC,
	`AccountId` ASC
)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

/****** Object:  Table `Materials`    Script Date: 20/09/2014 7:16:11 PM ******/
CREATE TABLE `Materials`(
	`Id` bigint AUTO_INCREMENT NOT NULL,
	`Name` varchar(255) NOT NULL,
	`MaterialDescription` varchar(4000) NOT NULL,
	`Density` double NOT NULL,
	`Organic` bit(1) NOT NULL,
	`Type` int NOT NULL,
	`BehaviourType` int NULL,
	`ThermalConductivity` double NOT NULL,
	`ElectricalConductivity` double NOT NULL,
	`SpecificHeatCapacity` double NOT NULL,
	`SolidFormId` bigint NULL,
	`LiquidFormId` bigint NULL,
	`GasFormId` bigint NULL,
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
 CONSTRAINT `PK_Materials` PRIMARY KEY CLUSTERED 
(
	`Id` ASC
)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

/****** Object:  Table `MoveSpeeds`    Script Date: 20/09/2014 7:16:11 PM ******/
CREATE TABLE `MoveSpeeds`(
	`Id` bigint AUTO_INCREMENT NOT NULL,
	`BodyProtoId` bigint NOT NULL,
	`Multiplier` double NOT NULL,
	`Alias` varchar(255) NOT NULL,
	`FirstPersonVerb` varchar(255) NOT NULL,
	`ThirdPersonVerb` varchar(255) NOT NULL,
	`PresentParticiple` varchar(255) NOT NULL,
	`PositionId` bigint NOT NULL,
	`StaminaMultiplier` double NOT NULL DEFAULT '1.0',
 CONSTRAINT `PK_MoveSpeeds` PRIMARY KEY CLUSTERED 
(
	`Id` ASC
)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

/****** Object:  Table `NameCulture`    Script Date: 20/09/2014 7:16:11 PM ******/
CREATE TABLE `NameCulture`(
	`Id` bigint AUTO_INCREMENT NOT NULL,
	`Name` varchar(255) NOT NULL,
	`Definition` varchar(10000) NOT NULL,
 CONSTRAINT `PK_NameCulture` PRIMARY KEY CLUSTERED 
(
	`Id` ASC
)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

/****** Object:  Table `NonCardinalExitTemplates`    Script Date: 20/09/2014 7:16:11 PM ******/
CREATE TABLE `NonCardinalExitTemplates`(
	`Id` bigint AUTO_INCREMENT NOT NULL,
	`Name` varchar(255) NOT NULL,
	`OriginOutboundPreface` varchar(255) NOT NULL,
	`OriginInboundPreface` varchar(255) NOT NULL,
	`DestinationOutboundPreface` varchar(255) NOT NULL,
	`DestinationInboundPreface` varchar(255) NOT NULL,
	`OutboundVerb` varchar(255) NOT NULL,
	`InboundVerb` varchar(255) NOT NULL,
PRIMARY KEY CLUSTERED 
(
	`Id` ASC
)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

/****** Object:  Table `NPCs`    Script Date: 20/09/2014 7:16:11 PM ******/
CREATE TABLE `NPCs`(
	`Id` bigint AUTO_INCREMENT NOT NULL,
	`CharacterId` bigint NOT NULL,
	`TemplateId` bigint NOT NULL,
	`TemplateRevnum` int NOT NULL,
PRIMARY KEY CLUSTERED 
(
	`Id` ASC
)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

/****** Object:  Table `NPCs_ArtificialIntelligences`    Script Date: 20/09/2014 7:16:11 PM ******/
CREATE TABLE `NPCs_ArtificialIntelligences`(
	`NPCId` bigint NOT NULL,
	`ArtificialIntelligenceId` bigint NOT NULL,
PRIMARY KEY CLUSTERED 
(
	`ArtificialIntelligenceId` ASC,
	`NPCId` ASC
)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

/****** Object:  Table `NPCTemplates`    Script Date: 20/09/2014 7:16:11 PM ******/
CREATE TABLE `NPCTemplates`(
	`Id` bigint AUTO_INCREMENT NOT NULL,
	`Type` varchar(255) NOT NULL,
	`Name` varchar(255) NOT NULL,
	`Definition` varchar(10000) NOT NULL,
	`EditableItemId` bigint NOT NULL,
	`RevisionNumber` int NOT NULL,
PRIMARY KEY CLUSTERED 
(
	`Id` ASC,
	`RevisionNumber` ASC
)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

/****** Object:  Table `NPCTemplates_ArtificalIntelligences`    Script Date: 20/09/2014 7:16:11 PM ******/
CREATE TABLE `NPCTemplates_ArtificalIntelligences`(
	`NPCTemplateId` bigint NOT NULL,
	`AIId` bigint NOT NULL,
	`NPCTemplateRevisionNumber` int NOT NULL,
PRIMARY KEY CLUSTERED 
(
	`NPCTemplateRevisionNumber` ASC,
	`NPCTemplateId` ASC,
	`AIId` ASC
)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

/****** Object:  Table `Paygrades`    Script Date: 20/09/2014 7:16:11 PM ******/
CREATE TABLE `Paygrades`(
	`Id` bigint AUTO_INCREMENT NOT NULL,
	`Name` varchar(100) NOT NULL,
	`Abbreviation` varchar(100) NOT NULL,
	`CurrencyId` bigint NOT NULL,
	`PayAmount` decimal(18, 0) NOT NULL,
	`ClanId` bigint NOT NULL,
PRIMARY KEY CLUSTERED 
(
	`Id` ASC
)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

/****** Object:  Table `PersonalName`    Script Date: 20/09/2014 7:16:11 PM ******/
CREATE TABLE `PersonalName`(
	`Id` bigint AUTO_INCREMENT NOT NULL,
	`CultureId` bigint NOT NULL,
 CONSTRAINT `PK_PersonalName` PRIMARY KEY CLUSTERED 
(
	`Id` ASC
)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

/****** Object:  Table `PersonalNameElement`    Script Date: 20/09/2014 7:16:11 PM ******/
CREATE TABLE `PersonalNameElement`(
	`Id` bigint AUTO_INCREMENT NOT NULL,
	`Name` varchar(255) NOT NULL,
	`Usage` int NOT NULL,
	`PersonalNameId` bigint NOT NULL,
 CONSTRAINT `PK_PersonalNameElement` PRIMARY KEY CLUSTERED 
(
	`Id` ASC
)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

/****** Object:  Table `Races`    Script Date: 20/09/2014 7:16:11 PM ******/
CREATE TABLE `Races`(
	`Id` bigint AUTO_INCREMENT NOT NULL,
	`Name` varchar(255) NOT NULL,
	`Description` varchar(4000) NOT NULL,
	`BaseBodyId` bigint NOT NULL,
	`AllowedGenders` varchar(255) NOT NULL,
	`ParentRaceId` bigint NULL,
	`AttributeBonusProgId` bigint NOT NULL,
	`AttributeTotalCap` int NOT NULL,
	`IndividualAttributeCap` int NOT NULL,
	`DiceExpression` varchar(255) NOT NULL,
	`IlluminationPerceptionMultiplier` double NOT NULL DEFAULT '1',
	`AvailabilityProgId` bigint NULL,
PRIMARY KEY CLUSTERED 
(
	`Id` ASC
)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

/****** Object:  Table `Races_AdditionalBodyparts`    Script Date: 20/09/2014 7:16:11 PM ******/
CREATE TABLE `Races_AdditionalBodyparts`(
	`Usage` varchar(50) NOT NULL,
	`BodypartId` bigint NOT NULL,
	`RaceId` bigint NOT NULL,
 CONSTRAINT `PK_Races_AdditionalBodyparts` PRIMARY KEY CLUSTERED 
(
	`Usage` ASC,
	`RaceId` ASC,
	`BodypartId` ASC
)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

/****** Object:  Table `Races_AdditionalCharacteristics`    Script Date: 20/09/2014 7:16:11 PM ******/
CREATE TABLE `Races_AdditionalCharacteristics`(
	`RaceId` bigint NOT NULL,
	`CharacteristicDefinitionId` bigint NOT NULL,
	`Usage` varchar(255) NOT NULL,
PRIMARY KEY CLUSTERED 
(
	`RaceId` ASC,
	`CharacteristicDefinitionId` ASC
)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

/****** Object:  Table `Races_Attributes`    Script Date: 20/09/2014 7:16:11 PM ******/
CREATE TABLE `Races_Attributes`(
	`RaceId` bigint NOT NULL,
	`AttributeId` bigint NOT NULL,
PRIMARY KEY CLUSTERED 
(
	`RaceId` ASC,
	`AttributeId` ASC
)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

/****** Object:  Table `Races_ChargenResources`    Script Date: 20/09/2014 7:16:11 PM ******/
CREATE TABLE `Races_ChargenResources`(
	`RaceId` bigint NOT NULL,
	`ChargenResourceId` bigint NOT NULL,
	`Amount` int NOT NULL,
 CONSTRAINT `PK_Races_ChargenResources` PRIMARY KEY CLUSTERED 
(
	`RaceId` ASC,
	`ChargenResourceId` ASC
)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

/****** Object:  Table `RandomNameProfiles`    Script Date: 20/09/2014 7:16:11 PM ******/
CREATE TABLE `RandomNameProfiles`(
	`Id` bigint AUTO_INCREMENT NOT NULL,
	`Gender` int NOT NULL,
	`NameCultureId` bigint NOT NULL,
	`Name` varchar(255) NOT NULL,
PRIMARY KEY CLUSTERED 
(
	`Id` ASC
)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

/****** Object:  Table `RandomNameProfiles_DiceExpressions`    Script Date: 20/09/2014 7:16:11 PM ******/
CREATE TABLE `RandomNameProfiles_DiceExpressions`(
	`RandomNameProfileId` bigint NOT NULL,
	`NameUsage` int NOT NULL,
	`DiceExpression` varchar(255) NOT NULL,
PRIMARY KEY CLUSTERED 
(
	`RandomNameProfileId` ASC,
	`NameUsage` ASC
)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

/****** Object:  Table `RandomNameProfiles_Elements`    Script Date: 20/09/2014 7:16:11 PM ******/
CREATE TABLE `RandomNameProfiles_Elements`(
	`RandomNameProfileId` bigint NOT NULL,
	`NameUsage` int NOT NULL,
	`Name` varchar(100) NOT NULL,
	`Weighting` int NOT NULL,
PRIMARY KEY CLUSTERED 
(
	`RandomNameProfileId` ASC,
	`NameUsage` ASC,
	`Name` ASC
)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

/****** Object:  Table `Ranks`    Script Date: 20/09/2014 7:16:11 PM ******/
CREATE TABLE `Ranks`(
	`Id` bigint AUTO_INCREMENT NOT NULL,
	`Name` varchar(100) NOT NULL,
	`InsigniaGameItemId` bigint NULL,
	`InsigniaGameItemRevnum` int NULL,
	`ClanId` bigint NOT NULL,
	`Privileges` bigint NOT NULL DEFAULT '0',
	`RankPath` varchar(50) NULL,
	`RankNumber` int NOT NULL,
PRIMARY KEY CLUSTERED 
(
	`Id` ASC
)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

/****** Object:  Table `Ranks_Abbreviations`    Script Date: 20/09/2014 7:16:11 PM ******/
CREATE TABLE `Ranks_Abbreviations`(
	`RankId` bigint NOT NULL,
	`Abbreviation` varchar(50) NOT NULL,
	`FutureProgId` bigint NULL,
	`Order` int NOT NULL,
PRIMARY KEY CLUSTERED 
(
	`RankId` ASC,
	`Abbreviation` ASC
)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

/****** Object:  Table `Ranks_Paygrades`    Script Date: 20/09/2014 7:16:11 PM ******/
CREATE TABLE `Ranks_Paygrades`(
	`RankId` bigint NOT NULL,
	`PaygradeId` bigint NOT NULL,
	`Order` int NOT NULL,
PRIMARY KEY CLUSTERED 
(
	`RankId` ASC,
	`PaygradeId` ASC
)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

/****** Object:  Table `Ranks_Titles`    Script Date: 20/09/2014 7:16:11 PM ******/
CREATE TABLE `Ranks_Titles`(
	`RankId` bigint NOT NULL,
	`Title` varchar(50) NOT NULL,
	`FutureProgId` bigint NULL,
	`Order` int NOT NULL,
PRIMARY KEY CLUSTERED 
(
	`RankId` ASC,
	`Title` ASC
)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

/****** Object:  Table `Rooms`    Script Date: 20/09/2014 7:16:11 PM ******/
CREATE TABLE `Rooms`(
	`Id` bigint AUTO_INCREMENT NOT NULL,
	`ZoneId` bigint NOT NULL,
	`X` int NOT NULL DEFAULT '0',
	`Y` int NOT NULL DEFAULT '0',
	`Z` int NOT NULL DEFAULT '0',
 CONSTRAINT `PK_Rooms` PRIMARY KEY CLUSTERED 
(
	`Id` ASC
)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

/****** Object:  Table `SavedPositions`    Script Date: 20/09/2014 7:16:11 PM ******/
CREATE TABLE `SavedPositions`(
	`PerceivableId` bigint NOT NULL,
	`PositionId` bigint NOT NULL,
	`Modifier` int NOT NULL,
	`TargetId` bigint NULL,
	`Emote` varchar(1000) NULL,
	`TargetType` varchar(100) NULL,
	`PerceivableType` varchar(100) NOT NULL,
PRIMARY KEY CLUSTERED 
(
	`PerceivableId` ASC,
	`PerceivableType` ASC
)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

/****** Object:  Table `Shards`    Script Date: 20/09/2014 7:16:11 PM ******/
CREATE TABLE `Shards`(
	`Name` varchar(255) NOT NULL,
	`Id` bigint AUTO_INCREMENT NOT NULL,
	`MinimumTerrestrialLux` double NOT NULL,
	`SkyDescriptionTemplateId` bigint NOT NULL,
 CONSTRAINT `PK_Shards` PRIMARY KEY CLUSTERED 
(
	`Id` ASC
)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

/****** Object:  Table `Shards_Calendars`    Script Date: 20/09/2014 7:16:11 PM ******/
CREATE TABLE `Shards_Calendars`(
	`ShardId` bigint NOT NULL,
	`CalendarId` bigint NOT NULL,
 CONSTRAINT `PK_Shards_Calendars` PRIMARY KEY CLUSTERED 
(
	`ShardId` ASC,
	`CalendarId` ASC
)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

/****** Object:  Table `Shards_Celestials`    Script Date: 20/09/2014 7:16:11 PM ******/
CREATE TABLE `Shards_Celestials`(
	`ShardId` bigint NOT NULL,
	`CelestialId` bigint NOT NULL,
 CONSTRAINT `PK_Shards_Celestials` PRIMARY KEY CLUSTERED 
(
	`ShardId` ASC,
	`CelestialId` ASC
)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

/****** Object:  Table `Shards_Clocks`    Script Date: 20/09/2014 7:16:11 PM ******/
CREATE TABLE `Shards_Clocks`(
	`ShardId` bigint NOT NULL,
	`ClockId` bigint NOT NULL,
 CONSTRAINT `PK_Shards_Clocks` PRIMARY KEY CLUSTERED 
(
	`ShardId` ASC,
	`ClockId` ASC
)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

/****** Object:  Table `SkyDescriptionTemplates`    Script Date: 20/09/2014 7:16:11 PM ******/
CREATE TABLE `SkyDescriptionTemplates`(
	`Id` bigint AUTO_INCREMENT NOT NULL,
	`Name` varchar(255) NOT NULL,
PRIMARY KEY CLUSTERED 
(
	`Id` ASC
)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

/****** Object:  Table `SkyDescriptionTemplates_Values`    Script Date: 20/09/2014 7:16:11 PM ******/
CREATE TABLE `SkyDescriptionTemplates_Values`(
	`SkyDescriptionTemplateId` bigint NOT NULL,
	`LowerBound` double NOT NULL,
	`UpperBound` double NOT NULL,
	`Description` varchar(4000) NOT NULL,
PRIMARY KEY CLUSTERED 
(
	`SkyDescriptionTemplateId` ASC,
	`LowerBound` ASC
)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

/****** Object:  Table `Socials`    Script Date: 20/09/2014 7:16:11 PM ******/
CREATE TABLE `Socials`(
	`Name` varchar(100) NOT NULL,
	`NoTargetEcho` varchar(1000) NOT NULL,
	`OneTargetEcho` varchar(1000) NOT NULL,
	`FutureProgId` bigint NULL,
	`DirectionTargetEcho` varchar(1000) NULL,
	`MultiTargetEcho` varchar(1000) NULL,
PRIMARY KEY CLUSTERED 
(
	`Name` ASC
)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

/****** Object:  Table `StackDecorators`    Script Date: 20/09/2014 7:16:11 PM ******/
CREATE TABLE `StackDecorators`(
	`Id` bigint AUTO_INCREMENT NOT NULL,
	`Name` varchar(255) NOT NULL,
	`Type` varchar(50) NOT NULL,
	`Definition` varchar(10000) NOT NULL,
	`Description` varchar(1000) NULL,
 CONSTRAINT `PK_StackDecorators` PRIMARY KEY CLUSTERED 
(
	`Id` ASC
)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

/****** Object:  Table `StaticConfigurations`    Script Date: 20/09/2014 7:16:11 PM ******/
CREATE TABLE `StaticConfigurations`(
	`SettingName` varchar(100) NOT NULL,
	`Definition` varchar(20000) NOT NULL,
PRIMARY KEY CLUSTERED 
(
	`SettingName` ASC
)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

/****** Object:  Table `StaticStrings`    Script Date: 20/09/2014 7:16:11 PM ******/
CREATE TABLE `StaticStrings`(
	`Id` varchar(50) NOT NULL,
	`Text` varchar(4000) NOT NULL,
PRIMARY KEY CLUSTERED 
(
	`Id` ASC
)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

/****** Object:  Table `Tags`    Script Date: 20/09/2014 7:16:11 PM ******/
CREATE TABLE `Tags`(
	`Name` varchar(255) NOT NULL,
	`Id` bigint AUTO_INCREMENT NOT NULL,
 CONSTRAINT `PK_Tags` PRIMARY KEY CLUSTERED 
(
	`Id` ASC
)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

/****** Object:  Table `TagsParents`    Script Date: 20/09/2014 7:16:11 PM ******/
CREATE TABLE `TagsParents`(
	`TagId` bigint NOT NULL,
	`ParentId` bigint NOT NULL,
PRIMARY KEY CLUSTERED 
(
	`TagId` ASC,
	`ParentId` ASC
)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

/****** Object:  Table `Terrains`    Script Date: 20/09/2014 7:16:11 PM ******/
CREATE TABLE `Terrains`(
	`Name` varchar(255) NOT NULL,
	`Id` bigint AUTO_INCREMENT NOT NULL,
	`MovementRate` double NOT NULL,
	`DefaultTerrain` bit(1) NOT NULL DEFAULT 0,
	`TerrainBehaviourMode` varchar(50) NOT NULL,
	`HideDifficulty` int NOT NULL DEFAULT '0',
	`SpotDifficulty` int NOT NULL DEFAULT '0',
	`StaminaCost` double NOT NULL DEFAULT '0',
 CONSTRAINT `PK_Terrains` PRIMARY KEY CLUSTERED 
(
	`Id` ASC
)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

/****** Object:  Table `TimeZoneInfos`    Script Date: 20/09/2014 7:16:11 PM ******/
CREATE TABLE `TimeZoneInfos`(
	`Id` varchar(100) NOT NULL,
	`Display` varchar(1000) NOT NULL,
	`Order` decimal(18, 2) NOT NULL,
PRIMARY KEY CLUSTERED 
(
	`Id` ASC
)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

/****** Object:  Table `Timezones`    Script Date: 20/09/2014 7:16:11 PM ******/
CREATE TABLE `Timezones`(
	`Id` bigint AUTO_INCREMENT NOT NULL,
	`Name` varchar(255) NOT NULL,
	`Description` varchar(4000) NOT NULL,
	`OffsetMinutes` int NOT NULL,
	`OffsetHours` int NOT NULL,
	`ClockId` bigint NOT NULL,
 CONSTRAINT `PK_Timezones` PRIMARY KEY CLUSTERED 
(
	`Id` ASC
)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

/****** Object:  Table `TraitDecorators`    Script Date: 20/09/2014 7:16:11 PM ******/
CREATE TABLE `TraitDecorators`(
	`Id` bigint AUTO_INCREMENT NOT NULL,
	`Name` varchar(255) NOT NULL,
	`Type` varchar(255) NOT NULL,
	`Contents` text NULL,
PRIMARY KEY CLUSTERED 
(
	`Id` ASC
)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

/****** Object:  Table `TraitDefinitions`    Script Date: 20/09/2014 7:16:11 PM ******/
CREATE TABLE `TraitDefinitions`(
	`Id` bigint AUTO_INCREMENT NOT NULL,
	`Name` varchar(255) NOT NULL,
	`Type` int NOT NULL,
	`DecoratorId` bigint NOT NULL,
	`TraitGroup` varchar(255) NOT NULL,
	`DerivedType` int NOT NULL,
	`ExpressionId` bigint NULL,
	`ImproverId` bigint NULL,
	`Hidden` bit(1) NULL DEFAULT 0,
	`ChargenBlurb` varchar(4000) NULL,
	`Alias` varchar(255) NULL,
	`AvailabilityProgId` bigint NULL,
 CONSTRAINT `PK_TraitDefinitions` PRIMARY KEY CLUSTERED 
(
	`Id` ASC
)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

/****** Object:  Table `TraitDefinitions_ChargenResources`    Script Date: 20/09/2014 7:16:11 PM ******/
CREATE TABLE `TraitDefinitions_ChargenResources`(
	`TraitDefinitionId` bigint NOT NULL,
	`ChargenResourceId` bigint NOT NULL,
	`Amount` int NOT NULL,
 CONSTRAINT `PK_TraitDefinitions_ChargenResources` PRIMARY KEY CLUSTERED 
(
	`TraitDefinitionId` ASC,
	`ChargenResourceId` ASC
)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

/****** Object:  Table `TraitExpression`    Script Date: 20/09/2014 7:16:11 PM ******/
CREATE TABLE `TraitExpression`(
	`Id` bigint AUTO_INCREMENT NOT NULL,
	`Expression` varchar(4000) NOT NULL,
 CONSTRAINT `PK_TraitExpression` PRIMARY KEY CLUSTERED 
(
	`Id` ASC
)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

/****** Object:  Table `TraitExpressionParameters`    Script Date: 20/09/2014 7:16:11 PM ******/
CREATE TABLE `TraitExpressionParameters`(
	`TraitExpressionId` bigint NOT NULL,
	`TraitDefinitionId` bigint NOT NULL,
	`Parameter` varchar(50) NOT NULL,
 CONSTRAINT `PK_TraitExpressionParameters` PRIMARY KEY CLUSTERED 
(
	`Parameter` ASC,
	`TraitExpressionId` ASC
)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

/****** Object:  Table `Traits`    Script Date: 20/09/2014 7:16:11 PM ******/
CREATE TABLE `Traits`(
	`BodyId` bigint NOT NULL,
	`Value` double NOT NULL,
	`TraitDefinitionId` bigint NOT NULL,
 CONSTRAINT `PK_Traits` PRIMARY KEY CLUSTERED 
(
	`BodyId` ASC,
	`TraitDefinitionId` ASC
)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

/****** Object:  Table `UnitOfMeasure`    Script Date: 20/09/2014 7:16:11 PM ******/
CREATE TABLE `UnitOfMeasure`(
	`Id` bigint AUTO_INCREMENT NOT NULL,
	`Name` varchar(50) NOT NULL,
	`Abbreviations` varchar(4000) NOT NULL,
	`BaseMultiplier` double NOT NULL,
	`PreMultiplierBaseOffset` double NOT NULL DEFAULT '0',
	`PostMultiplierBaseOffset` double NOT NULL DEFAULT '0',
	`Type` int NOT NULL,
	`Describer` bit(1) NOT NULL,
	`SpaceBetween` bit(1) NOT NULL DEFAULT 1,
	`System` varchar(50) NOT NULL,
 CONSTRAINT `PK_UnitOfMeasure` PRIMARY KEY CLUSTERED 
(
	`Id` ASC
)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

/****** Object:  Table `VariableDefaults`    Script Date: 20/09/2014 7:16:11 PM ******/
CREATE TABLE `VariableDefaults`(
	`OwnerType` bigint NOT NULL,
	`Property` varchar(50) NOT NULL,
	`DefaultValue` varchar(4000) NOT NULL,
PRIMARY KEY CLUSTERED 
(
	`OwnerType` ASC,
	`Property` ASC
)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

/****** Object:  Table `VariableDefinitions`    Script Date: 20/09/2014 7:16:11 PM ******/
CREATE TABLE `VariableDefinitions`(
	`OwnerType` bigint NOT NULL,
	`Property` varchar(50) NOT NULL,
	`ContainedType` bigint NOT NULL,
 CONSTRAINT `PK_VariableDefinitions` PRIMARY KEY CLUSTERED 
(
	`OwnerType` ASC,
	`Property` ASC
)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

/****** Object:  Table `VariableValues`    Script Date: 20/09/2014 7:16:11 PM ******/
CREATE TABLE `VariableValues`(
	`ReferenceType` bigint NOT NULL,
	`ReferenceId` bigint NOT NULL,
	`ReferenceProperty` varchar(50) NOT NULL,
	`ValueDefinition` varchar(4000) NOT NULL,
	`ValueType` bigint NOT NULL,
PRIMARY KEY CLUSTERED 
(
	`ReferenceType` ASC,
	`ReferenceId` ASC,
	`ReferenceProperty` ASC
)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

/****** Object:  Table `WearableSizeParameterRule`    Script Date: 20/09/2014 7:16:11 PM ******/
CREATE TABLE `WearableSizeParameterRule`(
	`Id` bigint AUTO_INCREMENT NOT NULL,
	`MinHeightFactor` double NOT NULL,
	`MaxHeightFactor` double NOT NULL,
	`MinWeightFactor` double NOT NULL,
	`MaxWeightFactor` double NULL,
	`MinTraitFactor` double NULL,
	`MaxTraitFactor` double NULL,
	`TraitId` bigint NULL,
	`BodyProtoId` bigint NOT NULL DEFAULT 0,
	`IgnoreTrait` bit(1) NOT NULL DEFAULT 1,
	`WeightVolumeRatios` varchar(4000) NULL,
	`TraitVolumeRatios` varchar(4000) NULL,
	`HeightLinearRatios` varchar(4000) NULL,
 CONSTRAINT `PK_WearableSizeParameterRule` PRIMARY KEY CLUSTERED 
(
	`Id` ASC
)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

/****** Object:  Table `WearableSizes`    Script Date: 20/09/2014 7:16:11 PM ******/
CREATE TABLE `WearableSizes`(
	`Id` bigint AUTO_INCREMENT NOT NULL,
	`OneSizeFitsAll` bit(1) NOT NULL,
	`Height` double NULL,
	`Weight` double NULL,
	`TraitValue` double NULL,
	`BodyPrototypeId` bigint NOT NULL,
PRIMARY KEY CLUSTERED 
(
	`Id` ASC
)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

/****** Object:  Table `WearProfiles`    Script Date: 20/09/2014 7:16:11 PM ******/
CREATE TABLE `WearProfiles`(
	`Id` bigint AUTO_INCREMENT NOT NULL,
	`Name` varchar(255) NOT NULL,
	`BodyPrototypeId` bigint NOT NULL,
	`WearStringInventory` varchar(255) NOT NULL,
	`WearAction1st` varchar(255) NOT NULL,
	`WearAction3rd` varchar(255) NOT NULL,
	`WearAffix` varchar(255) NOT NULL,
	`WearlocProfiles` varchar(4000) NOT NULL,
	`Type` varchar(50) NOT NULL,
	`Description` varchar(4000) NULL,
 CONSTRAINT `PK_WearProfiles` PRIMARY KEY CLUSTERED 
(
	`Id` ASC
)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

/****** Object:  Table `Zones`    Script Date: 20/09/2014 7:16:11 PM ******/
CREATE TABLE `Zones`(
	`Id` bigint AUTO_INCREMENT NOT NULL,
	`Name` varchar(255) NOT NULL,
	`ShardId` bigint NOT NULL,
	`Latitude` double NOT NULL,
	`Longitude` double NOT NULL,
	`Elevation` double NOT NULL,
	`DefaultCellId` bigint NULL,
	`AmbientLightPollution` double NOT NULL DEFAULT '0',
PRIMARY KEY CLUSTERED 
(
	`Id` ASC
)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

/****** Object:  Table `Zones_Timezones`    Script Date: 20/09/2014 7:16:11 PM ******/
CREATE TABLE `Zones_Timezones`(
	`ZoneId` bigint NOT NULL,
	`ClockId` bigint NOT NULL,
	`TimezoneId` bigint NOT NULL,
 CONSTRAINT `PK_Zones_Timezones_1` PRIMARY KEY CLUSTERED 
(
	`ZoneId` ASC,
	`ClockId` ASC,
	`TimezoneId` ASC
)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

/****** Object:  View `BodypartShapeCountView`    Script Date: 20/09/2014 7:16:11 PM ******/
CREATE VIEW `BodypartShapeCountView`
	AS 
	SELECT sc.BodypartGroupDescriptionRuleId, de.DescribedAs, sc.MinCount, sc.MaxCount, sc.TargetId, bs.Name FROM dbo.BodypartGroupDescribers_ShapeCount sc
	INNER JOIN dbo.BodypartShape bs on bs.Id = sc.TargetId
	INNER JOIN dbo.BodypartGroupDescribers de on sc.BodypartGroupDescriptionRuleId = de.Id
;

/****** Object:  View `GameItemEditingView`    Script Date: 20/09/2014 7:16:11 PM ******/
CREATE VIEW `GameItemEditingView`
	AS 
	SELECT `GameItems`.Id, GameItemProtos.Name, `GameItems`.MaterialId, GameItemProtos.MaterialId as ProtoMaterial, `GameItems`.Quality, `GameItems`.Size
	FROM `GameItems`
	INNER JOIN `GameItemProtos` on `GameItems`.GameItemProtoId = `GameItemProtos`.Id and `GameItems`.GameItemProtoRevision = `GameItemProtos`.RevisionNumber
;

ALTER TABLE `Accents` ADD  CONSTRAINT `FK_Accents_Languages` FOREIGN KEY(`LanguageId`) REFERENCES `Languages` (`Id`);

ALTER TABLE `AccountNotes` ADD  CONSTRAINT `FK_AccountNotes_Accounts` FOREIGN KEY(`AccountId`) REFERENCES `Accounts` (`Id`) ON DELETE CASCADE;

ALTER TABLE `AccountNotes` ADD  CONSTRAINT `FK_AccountNotes_Author` FOREIGN KEY(`AuthorId`) REFERENCES `Accounts` (`Id`);

ALTER TABLE `Accounts` ADD  CONSTRAINT `FK_Accounts_AuthorityGroups` FOREIGN KEY(`AuthorityGroupId`) REFERENCES `AuthorityGroups` (`Id`);

ALTER TABLE `Accounts_ChargenResources` ADD  CONSTRAINT `FK_Accounts_ChargenResources_Accounts` FOREIGN KEY(`AccountId`) REFERENCES `Accounts` (`Id`) ON DELETE CASCADE;

ALTER TABLE `Accounts_ChargenResources`  ADD  CONSTRAINT `FK_Accounts_ChargenResources_ChargenResources` FOREIGN KEY(`ChargenResourceId`) REFERENCES `ChargenResources` (`Id`) ON DELETE CASCADE;

ALTER TABLE `Appointments`  ADD  CONSTRAINT `FK_Appointments_Clans` FOREIGN KEY(`ClanId`) REFERENCES `Clans` (`Id`);

ALTER TABLE `Appointments`  ADD  CONSTRAINT `FK_Appointments_GameItemProtos` FOREIGN KEY(`InsigniaGameItemId`, `InsigniaGameItemRevnum`) REFERENCES `GameItemProtos` (`Id`, `RevisionNumber`);

ALTER TABLE `Appointments`  ADD  CONSTRAINT `FK_Appointments_ParentAppointment` FOREIGN KEY(`ParentAppointmentId`)
REFERENCES `Appointments` (`Id`)
;
ALTER TABLE `Appointments` ADD  CONSTRAINT `FK_Appointments_Paygrades` FOREIGN KEY(`PaygradeId`)
REFERENCES `Paygrades` (`Id`)
ON DELETE SET NULL
;
ALTER TABLE `Appointments` ADD  CONSTRAINT `FK_Appointments_Ranks` FOREIGN KEY(`MinimumRankId`)
REFERENCES `Ranks` (`Id`)
ON DELETE SET NULL
;
ALTER TABLE `Appointments` ADD  CONSTRAINT `FK_Appointments_Ranks_2` FOREIGN KEY(`MinimumRankToAppointId`)
REFERENCES `Ranks` (`Id`)
;
ALTER TABLE `Appointments_Abbreviations` ADD  CONSTRAINT `FK_Appointments_Abbreviations_Appointments` FOREIGN KEY(`AppointmentId`)
REFERENCES `Appointments` (`Id`)
;
ALTER TABLE `Appointments_Abbreviations` ADD  CONSTRAINT `FK_Appointments_Abbreviations_FutureProgs` FOREIGN KEY(`FutureProgId`)
REFERENCES `FutureProgs` (`Id`)
;
ALTER TABLE `Appointments_Titles`  ADD  CONSTRAINT `FK_Appointments_Titles_Appointments` FOREIGN KEY(`AppointmentId`)
REFERENCES `Appointments` (`Id`)
;
ALTER TABLE `Appointments_Titles` ADD  CONSTRAINT `FK_Appointments_Titles_FutureProgs` FOREIGN KEY(`FutureProgId`)
REFERENCES `FutureProgs` (`Id`)
;
ALTER TABLE `Bans`  ADD CONSTRAINT `FK_Bans_Accounts` FOREIGN KEY(`BannerAccountId`)
REFERENCES `Accounts` (`Id`)
ON DELETE SET NULL
;
ALTER TABLE `Bodies`  ADD CONSTRAINT `FK_Bodies_EntityDescriptions` FOREIGN KEY(`EntityDescriptionID`)
REFERENCES `EntityDescriptions` (`Id`)
;
ALTER TABLE `Bodies`  ADD CONSTRAINT `FK_Bodies_Races` FOREIGN KEY(`RaceId`)
REFERENCES `Races` (`Id`)
;
ALTER TABLE `Bodies_Accents`  ADD CONSTRAINT `FK_Bodies_Accents_Bodies` FOREIGN KEY(`BodyId`)
REFERENCES `Bodies` (`Id`)
;
ALTER TABLE `Bodies_GameItems`  ADD CONSTRAINT `FK_Bodies_GameItems_Bodies` FOREIGN KEY(`BodyId`)
REFERENCES `Bodies` (`Id`)
ON DELETE CASCADE
;
ALTER TABLE `Bodies_GameItems`  ADD CONSTRAINT `FK_Bodies_GameItems_GameItems` FOREIGN KEY(`GameItemId`)
REFERENCES `GameItems` (`Id`)
ON DELETE CASCADE
;
ALTER TABLE `Bodies_Languages`  ADD CONSTRAINT `FK_Bodies_Languages_Bodies` FOREIGN KEY(`BodyId`)
REFERENCES `Bodies` (`Id`)
;
ALTER TABLE `BodypartGroupDescribers_BodypartProtos`  ADD CONSTRAINT `FK_BGD_BodypartProtos_BodypartGroupDescribers` FOREIGN KEY(`BodypartGroupDescriberId`)
REFERENCES `BodypartGroupDescribers` (`Id`)
ON DELETE CASCADE
;
ALTER TABLE `BodypartGroupDescribers_BodypartProtos`  ADD CONSTRAINT `FK_BGD_BodypartProtos_BodypartProto` FOREIGN KEY(`BodypartProtoId`)
REFERENCES `BodypartProto` (`Id`)
ON DELETE CASCADE
;
ALTER TABLE `BodypartGroupDescribers_BodyProtos`  ADD CONSTRAINT `FK_BGD_BodyProtos_BodypartGroupDescribers` FOREIGN KEY(`BodypartGroupDescriberId`)
REFERENCES `BodypartGroupDescribers` (`Id`)
;
ALTER TABLE `BodypartGroupDescribers_BodyProtos`  ADD CONSTRAINT `FK_BGD_BodyProtos_BodyProtos` FOREIGN KEY(`BodyProtoId`)
REFERENCES `BodyProtos` (`Id`)
;
ALTER TABLE `BodypartGroupDescribers_ShapeCount`  ADD CONSTRAINT `FK_BGD_ShapeCount_BodypartGroupDescribers` FOREIGN KEY(`BodypartGroupDescriptionRuleId`)
REFERENCES `BodypartGroupDescribers` (`Id`)
;
ALTER TABLE `BodypartGroupDescribers_ShapeCount`  ADD CONSTRAINT `FK_BGD_ShapeCount_BodypartShape` FOREIGN KEY(`TargetId`)
REFERENCES `BodypartShape` (`Id`)
;
ALTER TABLE `BodypartProto`  ADD CONSTRAINT `FK_BodypartProto_BodypartShape` FOREIGN KEY(`BodypartShapeId`)
REFERENCES `BodypartShape` (`Id`)
;
ALTER TABLE `BodypartProto`  ADD CONSTRAINT `FK_BodypartProto_BodyPrototype` FOREIGN KEY(`BodyId`)
REFERENCES `BodyProtos` (`Id`)
;
ALTER TABLE `BodypartProto_AlignmentHits`  ADD CONSTRAINT `FK_BodypartProto_AlignmentHits_BodypartProto` FOREIGN KEY(`BodypartProtoId`)
REFERENCES `BodypartProto` (`Id`)
;
ALTER TABLE `BodypartProto_BodypartProto_Upstream`  ADD CONSTRAINT `FKChild` FOREIGN KEY(`Child`)
REFERENCES `BodypartProto` (`Id`)
;
ALTER TABLE `BodypartProto_BodypartProto_Upstream`  ADD CONSTRAINT `FKParent` FOREIGN KEY(`Parent`)
REFERENCES `BodypartProto` (`Id`)
;
ALTER TABLE `BodypartProto_OrientationHits`  ADD CONSTRAINT `FK_BodypartProto_OrientationHits_BodypartProto` FOREIGN KEY(`BodypartProtoId`)
REFERENCES `BodypartProto` (`Id`)
;
ALTER TABLE `BodyProtos`  ADD CONSTRAINT `FK_BodyPrototype_EntityDescriptions` FOREIGN KEY(`EntityDescriptionId`)
REFERENCES `EntityDescriptions` (`Id`)
;
ALTER TABLE `BodyProtos`  ADD CONSTRAINT `FK_BodyPrototype_WearableSizeParameterRule` FOREIGN KEY(`WearSizeParameterId`)
REFERENCES `WearableSizeParameterRule` (`Id`)
;
ALTER TABLE `BodyProtos_AdditionalBodyparts`  ADD CONSTRAINT `FK_BodyProtos_AdditionalBodyparts_BodypartProto` FOREIGN KEY(`BodypartId`)
REFERENCES `BodypartProto` (`Id`)
;
ALTER TABLE `BodyProtos_AdditionalBodyparts`  ADD CONSTRAINT `FK_BodyProtos_AdditionalBodyparts_BodyProtos` FOREIGN KEY(`BodyProtoId`)
REFERENCES `BodyProtos` (`Id`)
;
ALTER TABLE `CellOverlayPackages`  ADD CONSTRAINT `FK_CellOverlayPackages_EditableItems` FOREIGN KEY(`EditableItemId`)
REFERENCES `EditableItems` (`Id`)
;
ALTER TABLE `CellOverlays`  ADD CONSTRAINT `FK_CellOverlays_CellOverlayPackages` FOREIGN KEY(`CellOverlayPackageId`, `CellOverlayPackageRevisionNumber`)
REFERENCES `CellOverlayPackages` (`Id`, `RevisionNumber`)
;
ALTER TABLE `CellOverlays`  ADD CONSTRAINT `FK_CellOverlays_Cells` FOREIGN KEY(`CellId`)
REFERENCES `Cells` (`Id`)
;
ALTER TABLE `CellOverlays`  ADD CONSTRAINT `FK_CellOverlays_HearingProfiles` FOREIGN KEY(`HearingProfileId`)
REFERENCES `HearingProfiles` (`Id`)
ON DELETE SET NULL
;
ALTER TABLE `CellOverlays`  ADD CONSTRAINT `FK_CellOverlays_Terrains` FOREIGN KEY(`TerrainId`)
REFERENCES `Terrains` (`Id`)
;
ALTER TABLE `CellOverlays_Exits`  ADD CONSTRAINT `FK_CellOverlays_Exits_CellOverlays` FOREIGN KEY(`CellOverlayId`)
REFERENCES `CellOverlays` (`Id`)
;
ALTER TABLE `CellOverlays_Exits`  ADD CONSTRAINT `FK_CellOverlays_Exits_Exits` FOREIGN KEY(`ExitId`)
REFERENCES `Exits` (`Id`)
;
ALTER TABLE `Cells`  ADD CONSTRAINT `FK_Cells_CellOverlays` FOREIGN KEY(`CurrentOverlayId`)
REFERENCES `CellOverlays` (`Id`)
ON DELETE SET NULL
;
ALTER TABLE `Cells`  ADD CONSTRAINT `FK_Cells_Rooms` FOREIGN KEY(`RoomId`)
REFERENCES `Rooms` (`Id`)
;
ALTER TABLE `Cells_GameItems`  ADD CONSTRAINT `FK_Cells_GameItems_Cells` FOREIGN KEY(`CellId`)
REFERENCES `Cells` (`Id`)
ON DELETE CASCADE
;
ALTER TABLE `Cells_GameItems`  ADD CONSTRAINT `FK_Cells_GameItems_GameItems` FOREIGN KEY(`GameItemId`)
REFERENCES `GameItems` (`Id`)
ON DELETE CASCADE
;
ALTER TABLE `ChannelCommandWords`  ADD CONSTRAINT `FK_ChannelCommandWords_Channels` FOREIGN KEY(`ChannelId`)
REFERENCES `Channels` (`Id`)
ON DELETE CASCADE
;
ALTER TABLE `ChannelIgnorers`  ADD CONSTRAINT `FK_ChannelIgnorers_Accounts` FOREIGN KEY(`AccountId`)
REFERENCES `Accounts` (`Id`)
ON DELETE CASCADE
;
ALTER TABLE `ChannelIgnorers`  ADD CONSTRAINT `FK_ChannelIgnorers_Channels` FOREIGN KEY(`ChannelId`)
REFERENCES `Channels` (`Id`)
ON DELETE CASCADE
;
ALTER TABLE `Channels`  ADD CONSTRAINT `FK_Channels_FutureProgs_Listener` FOREIGN KEY(`ChannelListenerProgId`)
REFERENCES `FutureProgs` (`Id`)
;
ALTER TABLE `Channels`  ADD CONSTRAINT `FK_Channels_FutureProgs_Speaker` FOREIGN KEY(`ChannelSpeakerProgId`)
REFERENCES `FutureProgs` (`Id`)
;
ALTER TABLE `CharacteristicDefinitions`  ADD CONSTRAINT `FK_CharacteristicDefinitions_Parent` FOREIGN KEY(`ParentId`)
REFERENCES `CharacteristicDefinitions` (`Id`)
;
ALTER TABLE `CharacteristicProfiles`  ADD CONSTRAINT `FK_CharacteristicProfiles_CharacteristicDefinitions` FOREIGN KEY(`TargetDefinitionId`)
REFERENCES `CharacteristicDefinitions` (`Id`)
ON DELETE CASCADE
;
ALTER TABLE `Characteristics`  ADD CONSTRAINT `FK_Characteristics_Bodies` FOREIGN KEY(`BodyId`)
REFERENCES `Bodies` (`Id`)
;
ALTER TABLE `Characteristics`  ADD CONSTRAINT `FK_Characteristics_CharacteristicValues` FOREIGN KEY(`CharacteristicId`)
REFERENCES `CharacteristicValues` (`Id`)
ON DELETE CASCADE
;
ALTER TABLE `CharacteristicValues`  ADD CONSTRAINT `FK_CharacteristicValues_CharacteristicDefinitions` FOREIGN KEY(`DefinitionId`)
REFERENCES `CharacteristicDefinitions` (`Id`)
ON DELETE CASCADE
;
ALTER TABLE `CharacteristicValues`  ADD CONSTRAINT `FK_CharacteristicValues_FutureProgs` FOREIGN KEY(`FutureProgId`)
REFERENCES `FutureProgs` (`Id`)
ON DELETE SET NULL
;
ALTER TABLE `Characters`  ADD CONSTRAINT `FK_Characters_Accounts` FOREIGN KEY(`AccountId`)
REFERENCES `Accounts` (`Id`)
ON DELETE CASCADE
;
ALTER TABLE `Characters`  ADD CONSTRAINT `FK_Characters_Bodies` FOREIGN KEY(`BodyId`)
REFERENCES `Bodies` (`Id`)
ON DELETE CASCADE
;
ALTER TABLE `Characters`  ADD CONSTRAINT `FK_Characters_Cells` FOREIGN KEY(`Location`)
REFERENCES `Cells` (`Id`)
;
ALTER TABLE `Characters`  ADD CONSTRAINT `FK_Characters_Cultures` FOREIGN KEY(`CultureId`)
REFERENCES `Cultures` (`Id`)
;
ALTER TABLE `Characters`  ADD CONSTRAINT `FK_Characters_Currencies` FOREIGN KEY(`CurrencyId`)
REFERENCES `Currencies` (`Id`)
;
ALTER TABLE `Characters`  ADD CONSTRAINT `FK_Characters_PersonalName_Current` FOREIGN KEY(`CurrentNameId`)
REFERENCES `PersonalName` (`Id`)
;
ALTER TABLE `Characters`  ADD CONSTRAINT `FK_Characters_PersonalName_True` FOREIGN KEY(`PersonalNameId`)
REFERENCES `PersonalName` (`Id`)
;
ALTER TABLE `Characters_Aliases`  ADD CONSTRAINT `FK_Characters_Aliases_Characters` FOREIGN KEY(`CharacterId`)
REFERENCES `Characters` (`Id`)
ON DELETE CASCADE
;
ALTER TABLE `Characters_Aliases`  ADD CONSTRAINT `FK_Characters_Aliases_PersonalName` FOREIGN KEY(`PersonalNameId`)
REFERENCES `PersonalName` (`Id`)
ON DELETE CASCADE
;
ALTER TABLE `Characters_ChargenRoles`  ADD CONSTRAINT `FK_Characters_ChargenRoles_Characters` FOREIGN KEY(`CharacterId`)
REFERENCES `Characters` (`Id`)
ON DELETE CASCADE
;
ALTER TABLE `Characters_ChargenRoles`  ADD CONSTRAINT `FK_Characters_ChargenRoles_ChargenRoles` FOREIGN KEY(`ChargenRoleId`)
REFERENCES `ChargenRoles` (`Id`)
ON DELETE CASCADE
;
ALTER TABLE `ChargenRoles`  ADD CONSTRAINT `FK_ChargenRoles_Accounts` FOREIGN KEY(`PosterId`)
REFERENCES `Accounts` (`Id`)
;
ALTER TABLE `ChargenRoles`  ADD CONSTRAINT `FK_ChargenRoles_FutureProgs` FOREIGN KEY(`AvailabilityProgId`)
REFERENCES `FutureProgs` (`Id`)
ON DELETE SET NULL
;
ALTER TABLE `ChargenRoles_Approvers`  ADD CONSTRAINT `FK_ChargenRoles_Approvers_Accounts` FOREIGN KEY(`ApproverId`)
REFERENCES `Accounts` (`Id`)
;
ALTER TABLE `ChargenRoles_Approvers`  ADD CONSTRAINT `FK_ChargenRoles_Approvers_ChargenRoles` FOREIGN KEY(`ChargenRoleId`)
REFERENCES `ChargenRoles` (`Id`)
ON DELETE CASCADE
;
ALTER TABLE `ChargenRoles_ClanMemberships`  ADD CONSTRAINT `FK_ChargenRoles_ClanMemberships_ChargenRoles` FOREIGN KEY(`ChargenRoleId`)
REFERENCES `ChargenRoles` (`Id`)
ON DELETE CASCADE
;
ALTER TABLE `ChargenRoles_ClanMemberships`  ADD CONSTRAINT `FK_ChargenRoles_ClanMemberships_Clans` FOREIGN KEY(`ClanId`)
REFERENCES `Clans` (`Id`)
ON DELETE CASCADE
;
ALTER TABLE `ChargenRoles_ClanMemberships_Appointments`  ADD CONSTRAINT `FK_CRCMA_ChargenRoles_ClanMemberships` FOREIGN KEY(`ChargenRoleId`, `ClanId`)
REFERENCES `ChargenRoles_ClanMemberships` (`ChargenRoleId`, `ClanId`)
ON DELETE CASCADE
;
ALTER TABLE `ChargenRoles_Costs`  ADD CONSTRAINT `FK_ChargenRoles_Costs_ChargenResources` FOREIGN KEY(`ChargenResourceId`)
REFERENCES `ChargenResources` (`Id`)
ON DELETE CASCADE
;
ALTER TABLE `ChargenRoles_Costs`  ADD CONSTRAINT `FK_ChargenRoles_Costs_ChargenRoles` FOREIGN KEY(`ChargenRoleId`)
REFERENCES `ChargenRoles` (`Id`)
ON DELETE CASCADE
;
ALTER TABLE `ChargenRoles_Currencies`  ADD CONSTRAINT `FK_ChargenRoles_Currencies_ChargenRoles` FOREIGN KEY(`ChargenRoleId`)
REFERENCES `ChargenRoles` (`Id`)
ON DELETE CASCADE
;
ALTER TABLE `ChargenRoles_Currencies`  ADD CONSTRAINT `FK_ChargenRoles_Currencies_Currencies` FOREIGN KEY(`CurrencyId`)
REFERENCES `Currencies` (`Id`)
ON DELETE CASCADE
;
ALTER TABLE `ChargenRoles_Traits`  ADD CONSTRAINT `FK_ChargenRoles_Traits_ChargenRoles` FOREIGN KEY(`ChargenRoleId`)
REFERENCES `ChargenRoles` (`Id`)
ON DELETE CASCADE
;
ALTER TABLE `ChargenRoles_Traits`  ADD CONSTRAINT `FK_ChargenRoles_Traits_Currencies` FOREIGN KEY(`TraitId`)
REFERENCES `TraitDefinitions` (`Id`)
ON DELETE CASCADE
;
ALTER TABLE `Chargens`  ADD CONSTRAINT `FK_Chargens_Accounts` FOREIGN KEY(`AccountId`)
REFERENCES `Accounts` (`Id`)
;
ALTER TABLE `Checks`  ADD CONSTRAINT `FK_Checks_CheckTemplates` FOREIGN KEY(`CheckTemplateId`)
REFERENCES `CheckTemplates` (`Id`)
;
ALTER TABLE `Checks`  ADD CONSTRAINT `FK_Checks_TraitExpression` FOREIGN KEY(`TraitExpressionId`)
REFERENCES `TraitExpression` (`Id`)
;
ALTER TABLE `CheckTemplateDifficulties`  ADD CONSTRAINT `FK_CheckTemplateDifficulties_CheckTemplates` FOREIGN KEY(`CheckTemplateId`)
REFERENCES `CheckTemplates` (`Id`)
;
ALTER TABLE `ClanMemberships`  ADD CONSTRAINT `FK_ClanMemberships_Characters` FOREIGN KEY(`CharacterId`)
REFERENCES `Characters` (`Id`)
ON DELETE CASCADE
;
ALTER TABLE `ClanMemberships`  ADD CONSTRAINT `FK_ClanMemberships_Clans` FOREIGN KEY(`ClanId`)
REFERENCES `Clans` (`Id`)
ON DELETE CASCADE
;
ALTER TABLE `ClanMemberships`  ADD CONSTRAINT `FK_ClanMemberships_Manager` FOREIGN KEY(`ManagerId`)
REFERENCES `Characters` (`Id`)
;
ALTER TABLE `ClanMemberships`  ADD CONSTRAINT `FK_ClanMemberships_PersonalName` FOREIGN KEY(`PersonalNameId`)
REFERENCES `PersonalName` (`Id`)
;
ALTER TABLE `ClanMemberships_Appointments`  ADD CONSTRAINT `FK_ClanMemberships_Appointments_Appointments` FOREIGN KEY(`AppointmentId`)
REFERENCES `Appointments` (`Id`)
ON DELETE CASCADE
;
ALTER TABLE `ClanMemberships_Appointments`  ADD CONSTRAINT `FK_ClanMemberships_Appointments_ClanMemberships` FOREIGN KEY(`ClanId`, `CharacterId`)
REFERENCES `ClanMemberships` (`ClanId`, `CharacterId`)
ON DELETE CASCADE
;
ALTER TABLE `ClanMemberships_Backpay`  ADD CONSTRAINT `FK_ClanMemberships_Backpay_ClanMemberships` FOREIGN KEY(`ClanId`, `CharacterId`)
REFERENCES `ClanMemberships` (`ClanId`, `CharacterId`)
ON DELETE CASCADE
;
ALTER TABLE `ClanMemberships_Backpay`  ADD CONSTRAINT `FK_ClanMemberships_Backpay_Currencies` FOREIGN KEY(`CurrencyId`)
REFERENCES `Currencies` (`Id`)
ON DELETE CASCADE
;
ALTER TABLE `Clans`  ADD CONSTRAINT `FK_Clans_Calendars` FOREIGN KEY(`CalendarId`)
REFERENCES `Calendars` (`Id`)
;
ALTER TABLE `Clans`  ADD CONSTRAINT `FK_Clans_Parent` FOREIGN KEY(`ParentClanId`)
REFERENCES `Clans` (`Id`)
;
ALTER TABLE `Coins`  ADD CONSTRAINT `FK_Coins_Currencies` FOREIGN KEY(`CurrencyId`)
REFERENCES `Currencies` (`Id`)
ON DELETE CASCADE
;
ALTER TABLE `CombatMoves_DamagePatterns`  ADD CONSTRAINT `FK_CombatMoves_DamageTemplates_CombatMoves` FOREIGN KEY(`CombatMoveId`)
REFERENCES `CombatMoves` (`Id`)
;
ALTER TABLE `CombatMoves_DamagePatterns`  ADD CONSTRAINT `FK_CombatMoves_DamageTemplates_DamageTemplates` FOREIGN KEY(`DamageTemplateId`)
REFERENCES `DamagePatterns` (`Id`)
;
ALTER TABLE `Cultures`  ADD CONSTRAINT `FK_Cultures_AvailabilityProg` FOREIGN KEY(`AvailabilityProgId`)
REFERENCES `FutureProgs` (`Id`)
ON DELETE SET NULL
;
ALTER TABLE `Cultures`  ADD CONSTRAINT `FK_Cultures_NameCulture` FOREIGN KEY(`NameCultureId`)
REFERENCES `NameCulture` (`Id`)
;
ALTER TABLE `Cultures`  ADD CONSTRAINT `FK_Cultures_SkillStartingProg` FOREIGN KEY(`SkillStartingValueProgId`)
REFERENCES `FutureProgs` (`Id`)
;
ALTER TABLE `Cultures_ChargenResources`  ADD CONSTRAINT `FK_Cultures_ChargenResources_ChargenResources` FOREIGN KEY(`ChargenResourceId`)
REFERENCES `ChargenResources` (`Id`)
ON DELETE CASCADE
;
ALTER TABLE `Cultures_ChargenResources`  ADD CONSTRAINT `FK_Cultures_ChargenResources_Races` FOREIGN KEY(`CultureId`)
REFERENCES `Cultures` (`Id`)
ON DELETE CASCADE
;
ALTER TABLE `CurrencyDescriptionPatternElements`  ADD CONSTRAINT `FK_CDPE_CurrencyDescriptionPatterns` FOREIGN KEY(`CurrencyDescriptionPatternId`)
REFERENCES `CurrencyDescriptionPatterns` (`Id`)
ON DELETE CASCADE
;
ALTER TABLE `CurrencyDescriptionPatternElements`  ADD CONSTRAINT `FK_CDPE_CurrencyDivisions` FOREIGN KEY(`CurrencyDivisionId`)
REFERENCES `CurrencyDivisions` (`Id`)
;
ALTER TABLE `CurrencyDescriptionPatternElementSpecialValues`  ADD CONSTRAINT `FK_CDPESV_CDPE` FOREIGN KEY(`CurrencyDescriptionPatternElementId`)
REFERENCES `CurrencyDescriptionPatternElements` (`Id`)
ON DELETE CASCADE
;
ALTER TABLE `CurrencyDescriptionPatterns`  ADD CONSTRAINT `FK_CurrencyDescriptionPatterns_Currencies` FOREIGN KEY(`CurrencyId`)
REFERENCES `Currencies` (`Id`)
ON DELETE CASCADE
;
ALTER TABLE `CurrencyDescriptionPatterns`  ADD CONSTRAINT `FK_CurrencyDescriptionPatterns_FutureProgs` FOREIGN KEY(`FutureProgId`)
REFERENCES `FutureProgs` (`Id`)
ON DELETE CASCADE
;
ALTER TABLE `CurrencyDivisionAbbreviations`  ADD CONSTRAINT `FK_CurrencyDivisionAbbreviations_CurrencyDivisions` FOREIGN KEY(`CurrencyDivisionId`)
REFERENCES `CurrencyDivisions` (`Id`)
ON DELETE CASCADE
;
ALTER TABLE `CurrencyDivisions`  ADD CONSTRAINT `FK_CurrencyDivisions_Currencies` FOREIGN KEY(`CurrencyId`)
REFERENCES `Currencies` (`Id`)
ON DELETE CASCADE
;
ALTER TABLE `Doors`  ADD CONSTRAINT `FK_Doors_Locks` FOREIGN KEY(`LockedWith`)
REFERENCES `Locks` (`Id`)
;
ALTER TABLE `Dubs`  ADD CONSTRAINT `FK_Dubs_Characters` FOREIGN KEY(`CharacterId`)
REFERENCES `Characters` (`Id`)
ON DELETE CASCADE
;
ALTER TABLE `Effects`  ADD CONSTRAINT `FK_Effects_FutureProgs` FOREIGN KEY(`FutureProgId`)
REFERENCES `FutureProgs` (`Id`)
ON DELETE SET NULL
;
ALTER TABLE `EntityDescriptionPatterns`  ADD CONSTRAINT `FK_EntityDescriptionPatterns_FutureProgs` FOREIGN KEY(`ApplicabilityProgId`)
REFERENCES `FutureProgs` (`Id`)
;
ALTER TABLE `EntityDescriptionPatterns_EntityDescriptions`  ADD CONSTRAINT `FK_EDP_EntityDescriptions_EntityDescriptionPatterns` FOREIGN KEY(`PatternId`)
REFERENCES `EntityDescriptionPatterns` (`Id`)
;
ALTER TABLE `EntityDescriptionPatterns_EntityDescriptions`  ADD CONSTRAINT `FK_EDP_EntityDescriptions_EntityDescriptions` FOREIGN KEY(`EntityDescriptionId`)
REFERENCES `EntityDescriptions` (`Id`)
ON DELETE CASCADE
;
ALTER TABLE `Ethnicities`  ADD CONSTRAINT `FK_Ethnicities_AvailabilityProg` FOREIGN KEY(`AvailabilityProgId`)
REFERENCES `FutureProgs` (`Id`)
ON DELETE SET NULL
;
ALTER TABLE `Ethnicities_Characteristics`  ADD CONSTRAINT `FK_Ethnicities_Characteristics_CharacteristicDefinitions` FOREIGN KEY(`CharacteristicDefinitionId`)
REFERENCES `CharacteristicDefinitions` (`Id`)
;
ALTER TABLE `Ethnicities_Characteristics`  ADD CONSTRAINT `FK_Ethnicities_Characteristics_CharacteristicProfiles` FOREIGN KEY(`CharacteristicProfileId`)
REFERENCES `CharacteristicProfiles` (`Id`)
;
ALTER TABLE `Ethnicities_Characteristics`  ADD CONSTRAINT `FK_Ethnicities_Characteristics_Ethnicities` FOREIGN KEY(`EthnicityId`)
REFERENCES `Ethnicities` (`Id`)
ON DELETE CASCADE
;
ALTER TABLE `Ethnicities_ChargenResources`  ADD CONSTRAINT `FK_Ethnicities_ChargenResources_ChargenResources` FOREIGN KEY(`ChargenResourceId`)
REFERENCES `ChargenResources` (`Id`)
ON DELETE CASCADE
;
ALTER TABLE `Ethnicities_ChargenResources`  ADD CONSTRAINT `FK_Ethnicities_ChargenResources_Ethnicities` FOREIGN KEY(`EthnicityId`)
REFERENCES `Ethnicities` (`Id`)
ON DELETE CASCADE
;
ALTER TABLE `ExternalClanControls`  ADD CONSTRAINT `FK_ECC_Appointments_Controlled` FOREIGN KEY(`ControlledAppointmentId`)
REFERENCES `Appointments` (`Id`)
;
ALTER TABLE `ExternalClanControls`  ADD CONSTRAINT `FK_ECC_Appointments_Controlling` FOREIGN KEY(`ControllingAppointmentId`)
REFERENCES `Appointments` (`Id`)
;
ALTER TABLE `ExternalClanControls`  ADD CONSTRAINT `FK_ECC_Clans_Liege` FOREIGN KEY(`LiegeClanId`)
REFERENCES `Clans` (`Id`)
;
ALTER TABLE `ExternalClanControls`  ADD CONSTRAINT `FK_ECC_Clans_Vassal` FOREIGN KEY(`VassalClanId`)
REFERENCES `Clans` (`Id`)
;
ALTER TABLE `ExternalClanControls_Appointments`  ADD CONSTRAINT `FK_ECC_Appointments_ClanMemberships` FOREIGN KEY(`VassalClanId`, `CharacterId`)
REFERENCES `ClanMemberships` (`ClanId`, `CharacterId`)
;
ALTER TABLE `ExternalClanControls_Appointments`  ADD CONSTRAINT `FK_ECC_Appointments_ExternalClanControls` FOREIGN KEY(`VassalClanId`, `LiegeClanId`, `ControlledAppointmentId`)
REFERENCES `ExternalClanControls` (`VassalClanId`, `LiegeClanId`, `ControlledAppointmentId`)
;
ALTER TABLE `FutureProgs_Parameters`  ADD CONSTRAINT `FK_FutureProgs_Parameters_FutureProgs` FOREIGN KEY(`FutureProgId`)
REFERENCES `FutureProgs` (`Id`)
ON DELETE CASCADE
;
ALTER TABLE `GameItemComponentProtos`  ADD CONSTRAINT `FK_GameItemComponentProtos_EditableItems` FOREIGN KEY(`EditableItemId`)
REFERENCES `EditableItems` (`Id`)
;
ALTER TABLE `GameItemComponents`  ADD CONSTRAINT `FK_GameItemComponents_GameItems` FOREIGN KEY(`GameItemId`)
REFERENCES `GameItems` (`Id`)
ON DELETE CASCADE
;
ALTER TABLE `GameItemProtos`  ADD CONSTRAINT `FK_GameItemProtos_EditableItems` FOREIGN KEY(`EditableItemId`)
REFERENCES `EditableItems` (`Id`)
;
ALTER TABLE `GameItemProtos`  ADD CONSTRAINT `FK_GameItemProtos_EntityDescriptions` FOREIGN KEY(`EntityDescriptionId`)
REFERENCES `EntityDescriptions` (`Id`)
;
ALTER TABLE `GameItemProtos_GameItemComponentProtos`  ADD CONSTRAINT `FK_GIPGICP_GameItemComponentProtos` FOREIGN KEY(`GameItemComponentProtoId`, `GameItemComponentRevision`)
REFERENCES `GameItemComponentProtos` (`Id`, `RevisionNumber`)
ON DELETE CASCADE
;
ALTER TABLE `GameItemProtos_GameItemComponentProtos`  ADD CONSTRAINT `FK_GIPGICP_GameItemProtos` FOREIGN KEY(`GameItemProtoId`, `GameItemProtoRevision`)
REFERENCES `GameItemProtos` (`Id`, `RevisionNumber`)
ON DELETE CASCADE
;
ALTER TABLE `GameItemProtos_Tags`  ADD CONSTRAINT `FK_GameItemProtos_Tags_GameItemProtos` FOREIGN KEY(`GameItemProtoId`, `GameItemProtoRevisionNumber`)
REFERENCES `GameItemProtos` (`Id`, `RevisionNumber`)
ON DELETE CASCADE
;
ALTER TABLE `GameItemProtos_Tags`  ADD CONSTRAINT `FK_GameItemProtos_Tags_Tags` FOREIGN KEY(`TagId`)
REFERENCES `Tags` (`Id`)
ON DELETE CASCADE
;
ALTER TABLE `Helpfiles`  ADD CONSTRAINT `FK_Helpfiles_FutureProgs` FOREIGN KEY(`RuleId`)
REFERENCES `FutureProgs` (`Id`)
;
ALTER TABLE `Helpfiles_ExtraTexts`  ADD CONSTRAINT `FK_Helpfiles_ExtraTexts_FutureProgs` FOREIGN KEY(`RuleId`)
REFERENCES `FutureProgs` (`Id`)
;
ALTER TABLE `Helpfiles_ExtraTexts`  ADD CONSTRAINT `FK_Helpfiles_ExtraTexts_Helpfiles` FOREIGN KEY(`HelpfileId`)
REFERENCES `Helpfiles` (`Id`)
;
ALTER TABLE `Languages`  ADD CONSTRAINT `FK_Languages_LanguageDifficultyModels` FOREIGN KEY(`DifficultyModel`)
REFERENCES `LanguageDifficultyModels` (`Id`)
;
ALTER TABLE `Languages`  ADD CONSTRAINT `FK_Languages_TraitDefinitions` FOREIGN KEY(`LinkedTraitId`)
REFERENCES `TraitDefinitions` (`Id`)
;
ALTER TABLE `LoginIPs`  ADD CONSTRAINT `FK_LoginIPs_Accounts` FOREIGN KEY(`AccountId`)
REFERENCES `Accounts` (`Id`)
;
ALTER TABLE `MoveSpeeds`  ADD CONSTRAINT `FK_MoveSpeeds_BodyPrototype` FOREIGN KEY(`BodyProtoId`)
REFERENCES `BodyProtos` (`Id`)
;
ALTER TABLE `NPCs`  ADD CONSTRAINT `FK_NPCs_Characters` FOREIGN KEY(`CharacterId`)
REFERENCES `Characters` (`Id`)
ON DELETE CASCADE
;
ALTER TABLE `NPCs`  ADD CONSTRAINT `FK_NPCs_NPCTemplates` FOREIGN KEY(`TemplateId`, `TemplateRevnum`)
REFERENCES `NPCTemplates` (`Id`, `RevisionNumber`)
ON DELETE CASCADE
;
ALTER TABLE `NPCs_ArtificialIntelligences`  ADD CONSTRAINT `FK_NPCs_ArtificialIntelligences_ArtificialIntelligences` FOREIGN KEY(`ArtificialIntelligenceId`)
REFERENCES `ArtificialIntelligences` (`Id`)
ON DELETE CASCADE
;
ALTER TABLE `NPCs_ArtificialIntelligences`  ADD CONSTRAINT `FK_NPCs_ArtificialIntelligences_NPCs` FOREIGN KEY(`NPCId`)
REFERENCES `NPCs` (`Id`)
ON DELETE CASCADE
;
ALTER TABLE `NPCTemplates`  ADD CONSTRAINT `FK_NPCTemplates_EditableItems` FOREIGN KEY(`EditableItemId`)
REFERENCES `EditableItems` (`Id`)
;
ALTER TABLE `NPCTemplates_ArtificalIntelligences`  ADD CONSTRAINT `FK_NTAI_ArtificalIntelligences` FOREIGN KEY(`AIId`)
REFERENCES `ArtificialIntelligences` (`Id`)
ON DELETE CASCADE
;
ALTER TABLE `NPCTemplates_ArtificalIntelligences`  ADD CONSTRAINT `FK_NTAI_NPCTemplates` FOREIGN KEY(`NPCTemplateId`, `NPCTemplateRevisionNumber`)
REFERENCES `NPCTemplates` (`Id`, `RevisionNumber`)
ON DELETE CASCADE
;
ALTER TABLE `Paygrades`  ADD CONSTRAINT `FK_Paygrades_Clans` FOREIGN KEY(`ClanId`)
REFERENCES `Clans` (`Id`)
;
ALTER TABLE `Paygrades`  ADD CONSTRAINT `FK_Paygrades_Currencies` FOREIGN KEY(`CurrencyId`)
REFERENCES `Currencies` (`Id`)
;
ALTER TABLE `PersonalName`  ADD CONSTRAINT `FK_PersonalName_NameCulture` FOREIGN KEY(`CultureId`)
REFERENCES `NameCulture` (`Id`)
;
ALTER TABLE `PersonalNameElement`  ADD CONSTRAINT `FK_PersonalNameElement_PersonalName` FOREIGN KEY(`PersonalNameId`)
REFERENCES `PersonalName` (`Id`)
ON DELETE CASCADE
;
ALTER TABLE `Races`  ADD CONSTRAINT `FK_Races_AttributeBonusProg` FOREIGN KEY(`AttributeBonusProgId`)
REFERENCES `FutureProgs` (`Id`)
;
ALTER TABLE `Races`  ADD CONSTRAINT `FK_Races_AvailabilityProg` FOREIGN KEY(`AvailabilityProgId`)
REFERENCES `FutureProgs` (`Id`)
ON DELETE SET NULL
;
ALTER TABLE `Races`  ADD CONSTRAINT `FK_Races_BodyProtos` FOREIGN KEY(`BaseBodyId`)
REFERENCES `BodyProtos` (`Id`)
;
ALTER TABLE `Races`  ADD CONSTRAINT `FK_Races_Races` FOREIGN KEY(`ParentRaceId`)
REFERENCES `Races` (`Id`)
;
ALTER TABLE `Races_AdditionalBodyparts`  ADD CONSTRAINT `FK_Races_AdditionalBodyparts_BodypartProto` FOREIGN KEY(`BodypartId`)
REFERENCES `BodypartProto` (`Id`)
;
ALTER TABLE `Races_AdditionalBodyparts`  ADD CONSTRAINT `FK_Races_AdditionalBodyparts_Races` FOREIGN KEY(`RaceId`)
REFERENCES `Races` (`Id`)
ON DELETE CASCADE
;
ALTER TABLE `Races_AdditionalCharacteristics`  ADD CONSTRAINT `FK_RAC_CharacteristicDefinitions` FOREIGN KEY(`CharacteristicDefinitionId`)
REFERENCES `CharacteristicDefinitions` (`Id`)
;
ALTER TABLE `Races_AdditionalCharacteristics`  ADD CONSTRAINT `FK_RAC_Races` FOREIGN KEY(`RaceId`)
REFERENCES `Races` (`Id`)
ON DELETE CASCADE
;
ALTER TABLE `Races_Attributes`  ADD CONSTRAINT `FK_Races_Attributes_Races` FOREIGN KEY(`RaceId`)
REFERENCES `Races` (`Id`)
ON DELETE CASCADE
;
ALTER TABLE `Races_Attributes`  ADD CONSTRAINT `FK_Races_Attributes_TraitDefinitions` FOREIGN KEY(`AttributeId`)
REFERENCES `TraitDefinitions` (`Id`)
ON DELETE CASCADE
;
ALTER TABLE `Races_ChargenResources`  ADD CONSTRAINT `FK_Races_ChargenResources_ChargenResources` FOREIGN KEY(`ChargenResourceId`)
REFERENCES `ChargenResources` (`Id`)
ON DELETE CASCADE
;
ALTER TABLE `Races_ChargenResources`  ADD CONSTRAINT `FK_Races_ChargenResources_Races` FOREIGN KEY(`RaceId`)
REFERENCES `Races` (`Id`)
ON DELETE CASCADE
;
ALTER TABLE `RandomNameProfiles`  ADD CONSTRAINT `FK_RandomNameProfiles_NameCulture` FOREIGN KEY(`NameCultureId`)
REFERENCES `NameCulture` (`Id`)
;
ALTER TABLE `RandomNameProfiles_DiceExpressions`  ADD CONSTRAINT `FK_RandomNameProfiles_DiceExpressions_RandomNameProfiles` FOREIGN KEY(`RandomNameProfileId`)
REFERENCES `RandomNameProfiles` (`Id`)
ON DELETE CASCADE
;
ALTER TABLE `RandomNameProfiles_Elements`  ADD CONSTRAINT `FK_RandomNameProfiles_Elements_RandomNameProfiles` FOREIGN KEY(`RandomNameProfileId`)
REFERENCES `RandomNameProfiles` (`Id`)
ON DELETE CASCADE
;
ALTER TABLE `Ranks`  ADD CONSTRAINT `FK_Ranks_Clans` FOREIGN KEY(`ClanId`)
REFERENCES `Clans` (`Id`)
ON DELETE CASCADE
;
ALTER TABLE `Ranks`  ADD CONSTRAINT `FK_Ranks_GameItemProtos` FOREIGN KEY(`InsigniaGameItemId`, `InsigniaGameItemRevnum`)
REFERENCES `GameItemProtos` (`Id`, `RevisionNumber`)
;
ALTER TABLE `Ranks_Abbreviations`  ADD CONSTRAINT `FK_Ranks_Abbreviations_FutureProgs` FOREIGN KEY(`FutureProgId`)
REFERENCES `FutureProgs` (`Id`)
ON DELETE SET NULL
;
ALTER TABLE `Ranks_Abbreviations`  ADD CONSTRAINT `FK_Ranks_Abbreviations_Ranks` FOREIGN KEY(`RankId`)
REFERENCES `Ranks` (`Id`)
ON DELETE CASCADE
;
ALTER TABLE `Ranks_Paygrades`  ADD CONSTRAINT `FK_Ranks_Paygrades_Paygrades` FOREIGN KEY(`PaygradeId`)
REFERENCES `Paygrades` (`Id`)
ON DELETE CASCADE
;
ALTER TABLE `Ranks_Paygrades`  ADD CONSTRAINT `FK_Ranks_Paygrades_Ranks` FOREIGN KEY(`RankId`)
REFERENCES `Ranks` (`Id`)
ON DELETE CASCADE
;
ALTER TABLE `Ranks_Titles`  ADD CONSTRAINT `FK_Ranks_Titles_FutureProgs` FOREIGN KEY(`FutureProgId`)
REFERENCES `FutureProgs` (`Id`)
ON DELETE SET NULL
;
ALTER TABLE `Ranks_Titles`  ADD CONSTRAINT `FK_Ranks_Titles_Ranks` FOREIGN KEY(`RankId`)
REFERENCES `Ranks` (`Id`)
ON DELETE CASCADE
;
ALTER TABLE `Rooms`  ADD CONSTRAINT `FK_Rooms_Zones` FOREIGN KEY(`ZoneId`)
REFERENCES `Zones` (`Id`)
;
ALTER TABLE `Shards`  ADD CONSTRAINT `FK_Shards_SkyDescriptionTemplates` FOREIGN KEY(`SkyDescriptionTemplateId`)
REFERENCES `SkyDescriptionTemplates` (`Id`)
;
ALTER TABLE `Shards_Calendars`  ADD CONSTRAINT `FK_Shards_Calendars_Shards` FOREIGN KEY(`ShardId`)
REFERENCES `Shards` (`Id`)
ON DELETE CASCADE
;
ALTER TABLE `Shards_Celestials`  ADD CONSTRAINT `FK_Shards_Celestials_Shards` FOREIGN KEY(`ShardId`)
REFERENCES `Shards` (`Id`)
ON DELETE CASCADE
;
ALTER TABLE `Shards_Clocks`  ADD CONSTRAINT `FK_Shards_Clocks_Shards` FOREIGN KEY(`ShardId`)
REFERENCES `Shards` (`Id`)
ON DELETE CASCADE
;
ALTER TABLE `SkyDescriptionTemplates_Values`  ADD CONSTRAINT `FK_SkyDescriptionTemplates_Values_SkyDescriptionTemplates` FOREIGN KEY(`SkyDescriptionTemplateId`)
REFERENCES `SkyDescriptionTemplates` (`Id`)
ON DELETE CASCADE
;
ALTER TABLE `Socials`  ADD CONSTRAINT `FK_Socials_FutureProgs` FOREIGN KEY(`FutureProgId`)
REFERENCES `FutureProgs` (`Id`)
ON DELETE SET NULL
;
ALTER TABLE `TagsParents`  ADD CONSTRAINT `FK_TagsParents_Tags_Child` FOREIGN KEY(`TagId`)
REFERENCES `Tags` (`Id`)
ON DELETE CASCADE
;
ALTER TABLE `TagsParents`  ADD CONSTRAINT `FK_TagsParents_Tags_Parent` FOREIGN KEY(`ParentId`)
REFERENCES `Tags` (`Id`)
;
ALTER TABLE `Timezones`  ADD CONSTRAINT `FK_Timezones_Clocks` FOREIGN KEY(`ClockId`)
REFERENCES `Clocks` (`Id`)
;
ALTER TABLE `TraitDefinitions`  ADD CONSTRAINT `FK_TraitDefinitions_AvailabilityProg` FOREIGN KEY(`AvailabilityProgId`)
REFERENCES `FutureProgs` (`Id`)
ON DELETE SET NULL
;
ALTER TABLE `TraitDefinitions`  ADD CONSTRAINT `FK_TraitDefinitions_TraitExpression` FOREIGN KEY(`ExpressionId`)
REFERENCES `TraitExpression` (`Id`)
;
ALTER TABLE `TraitDefinitions_ChargenResources`  ADD CONSTRAINT `FK_TraitDefinitions_ChargenResources_ChargenResources` FOREIGN KEY(`ChargenResourceId`)
REFERENCES `ChargenResources` (`Id`)
ON DELETE CASCADE
;
ALTER TABLE `TraitDefinitions_ChargenResources`  ADD CONSTRAINT `FK_TraitDefinitions_ChargenResources_Races` FOREIGN KEY(`TraitDefinitionId`)
REFERENCES `TraitDefinitions` (`Id`)
ON DELETE CASCADE
;
ALTER TABLE `TraitExpressionParameters`  ADD CONSTRAINT `FK_TraitExpressionParameters_TraitDefinitions` FOREIGN KEY(`TraitDefinitionId`)
REFERENCES `TraitDefinitions` (`Id`)
;
ALTER TABLE `TraitExpressionParameters`  ADD CONSTRAINT `FK_TraitExpressionParameters_TraitExpression` FOREIGN KEY(`TraitExpressionId`)
REFERENCES `TraitExpression` (`Id`)
;
ALTER TABLE `Traits`  ADD CONSTRAINT `FK_Traits_Bodies` FOREIGN KEY(`BodyId`)
REFERENCES `Bodies` (`Id`)
ON DELETE CASCADE
;
ALTER TABLE `Traits`  ADD CONSTRAINT `FK_Traits_TraitDefinitions` FOREIGN KEY(`TraitDefinitionId`)
REFERENCES `TraitDefinitions` (`Id`)
ON DELETE CASCADE
;
ALTER TABLE `WearableSizeParameterRule`  ADD CONSTRAINT `FK_WearableSizeParameterRule_TraitDefinitions` FOREIGN KEY(`TraitId`)
REFERENCES `TraitDefinitions` (`Id`)
;
ALTER TABLE `Zones`  ADD CONSTRAINT `FK_Zones_Cells` FOREIGN KEY(`DefaultCellId`)
REFERENCES `Cells` (`Id`)
;
ALTER TABLE `Zones`  ADD CONSTRAINT `FK_Zones_Shards` FOREIGN KEY(`ShardId`)
REFERENCES `Shards` (`Id`)
;
ALTER TABLE `Zones_Timezones`  ADD CONSTRAINT `FK_Zones_Timezones_Zones` FOREIGN KEY(`ZoneId`)
REFERENCES `Zones` (`Id`)
;
ALTER TABLE `ExternalClanControls`  ADD CONSTRAINT `CK_ExternalClanControls_Controlled_Not_Controlling` CHECK  ((`ControlledAppointmentId`<>`ControllingAppointmentId`))
;
ALTER TABLE `ExternalClanControls`  ADD CONSTRAINT `CK_ExternalClanControls_Vassal_Not_Liege` CHECK  ((`VassalClanId`<>`LiegeClanId`))
;
