CREATE TABLE `dbo`.`LegalAuthorities` (
  `Id` BIGINT NOT NULL AUTO_INCREMENT,
  `Name` VARCHAR(250) NOT NULL,
  `CurrencyId` BIGINT NOT NULL,
  PRIMARY KEY (`Id`),
  INDEX `FK_LegalAuthorities_Currencies_idx` (`CurrencyId` ASC),
  CONSTRAINT `FK_LegalAuthorities_Currencies`
    FOREIGN KEY (`CurrencyId`)
    REFERENCES `dbo`.`Currencies` (`Id`)
    ON DELETE CASCADE
    ON UPDATE CASCADE);
	CREATE TABLE `dbo`.`LegalAuthorities_Zones` (
  `ZoneId` BIGINT NOT NULL,
  `LegalAuthorityId` BIGINT NOT NULL,
  PRIMARY KEY (`ZoneId`, `LegalAuthorityId`),
  INDEX `FK_LegalAuthorities_Zones_LegalAuthorities_idx` (`LegalAuthorityId` ASC),
  CONSTRAINT `FK_LegalAuthorities_Zones_Zones`
    FOREIGN KEY (`ZoneId`)
    REFERENCES `dbo`.`Zones` (`Id`)
    ON DELETE CASCADE
    ON UPDATE CASCADE,
  CONSTRAINT `FK_LegalAuthorities_Zones_LegalAuthorities`
    FOREIGN KEY (`LegalAuthorityId`)
    REFERENCES `dbo`.`LegalAuthorities` (`Id`)
    ON DELETE CASCADE
    ON UPDATE CASCADE);
CREATE TABLE `dbo`.`LegalClasses` (
  `Id` BIGINT NOT NULL AUTO_INCREMENT,
  `Name` VARCHAR(250) NOT NULL,
  `LegalAuthorityId` BIGINT NOT NULL,
  `LegalClassPriority` INT NOT NULL,
  `MembershipProgId` BIGINT NOT NULL,
  `CanBeDetainedUntilFinesPaid` BIT NOT NULL,
  PRIMARY KEY (`Id`),
  INDEX `FK_LegalClasses_LegalAuthorities_idx` (`LegalAuthorityId` ASC),
  INDEX `FK_LegalClasses_FutureProgs_idx` (`MembershipProgId` ASC),
  CONSTRAINT `FK_LegalClasses_LegalAuthorities`
    FOREIGN KEY (`LegalAuthorityId`)
    REFERENCES `dbo`.`LegalAuthorities` (`Id`)
    ON DELETE CASCADE
    ON UPDATE CASCADE,
  CONSTRAINT `FK_LegalClasses_FutureProgs`
    FOREIGN KEY (`MembershipProgId`)
    REFERENCES `dbo`.`FutureProgs` (`Id`)
    ON DELETE CASCADE
    ON UPDATE CASCADE);
CREATE TABLE `dbo`.`EnforcementAuthorities` (
  `Id` BIGINT NOT NULL AUTO_INCREMENT,
  `Name` VARCHAR(250) NOT NULL,
  `LegalAuthorityId` BIGINT NOT NULL,
  `Priority` INT NOT NULL,
  `CanAccuse` BIT NOT NULL,
  `CanForgive` BIT NOT NULL,
  `CanConvict` BIT NOT NULL,
  PRIMARY KEY (`Id`),
  INDEX `FK_EnforcementAuthorities_LegalAuthorities_idx` (`LegalAuthorityId` ASC),
  CONSTRAINT `FK_EnforcementAuthorities_LegalAuthorities`
    FOREIGN KEY (`LegalAuthorityId`)
    REFERENCES `dbo`.`LegalAuthorities` (`Id`)
    ON DELETE CASCADE
    ON UPDATE CASCADE);
CREATE TABLE `dbo`.`EnforcementAuthorities_ArrestableClasses` (
  `EnforcementAuthorityId` BIGINT NOT NULL,
  `LegalClassId` BIGINT NOT NULL,
  PRIMARY KEY (`EnforcementAuthorityId`),
  INDEX `FK_EnforcementAuthorities_ArrestableClasses_LegalClasses_idx` (`LegalClassId` ASC),
  CONSTRAINT `FK_EnforcementAuthorities_ArrestableClasses_LegalClasses`
    FOREIGN KEY (`LegalClassId`)
    REFERENCES `dbo`.`LegalClasses` (`Id`)
    ON DELETE CASCADE
    ON UPDATE CASCADE,
  CONSTRAINT `FK_EnforcementAuthorities_ArrestableClasses_Enforce`
    FOREIGN KEY (`EnforcementAuthorityId`)
    REFERENCES `dbo`.`EnforcementAuthorities` (`Id`)
    ON DELETE CASCADE
    ON UPDATE CASCADE);
CREATE TABLE `dbo`.`EnforcementAuthorities_AccusableClasses` (
  `EnforcementAuthorityId` BIGINT NOT NULL,
  `LegalClassId` BIGINT NOT NULL,
  PRIMARY KEY (`EnforcementAuthorityId`, `LegalClassId`),
  INDEX `FK_EnforcementAuthorities_AccusableClasses_LegalClasses_idx` (`LegalClassId` ASC),
  CONSTRAINT `FK_EnforcementAuthorities_AccusableClasses_Enforce`
    FOREIGN KEY (`EnforcementAuthorityId`)
    REFERENCES `dbo`.`EnforcementAuthorities` (`Id`)
    ON DELETE CASCADE
    ON UPDATE CASCADE,
  CONSTRAINT `FK_EnforcementAuthorities_AccusableClasses_LegalClasses`
    FOREIGN KEY (`LegalClassId`)
    REFERENCES `dbo`.`LegalClasses` (`Id`)
    ON DELETE CASCADE
    ON UPDATE CASCADE);
CREATE TABLE `dbo`.`EnforcementAuthorities_ParentAuthorities` (
  `ParentId` BIGINT NOT NULL,
  `ChildId` BIGINT NOT NULL,
  PRIMARY KEY (`ParentId`, `ChildId`),
  INDEX `FK_EnforcementAuthorities_ParentAuthorities_Child_idx` (`ChildId` ASC),
  CONSTRAINT `FK_EnforcementAuthorities_ParentAuthorities_Parent`
    FOREIGN KEY (`ParentId`)
    REFERENCES `dbo`.`EnforcementAuthorities` (`Id`)
    ON DELETE CASCADE
    ON UPDATE CASCADE,
  CONSTRAINT `FK_EnforcementAuthorities_ParentAuthorities_Child`
    FOREIGN KEY (`ChildId`)
    REFERENCES `dbo`.`EnforcementAuthorities` (`Id`)
    ON DELETE CASCADE
    ON UPDATE CASCADE);
CREATE TABLE `dbo`.`Laws` (
  `Id` BIGINT NOT NULL AUTO_INCREMENT,
  `Name` VARCHAR(250) NOT NULL,
  `LegalAuthorityId` BIGINT NOT NULL,
  `CrimeType` INT NOT NULL,
  `ActivePeriod` DOUBLE NOT NULL,
  `EnforcementStrategy` VARCHAR(500) NOT NULL,
  `LawAppliesProgId` BIGINT NULL,
  `EnforcementPriority` INT NOT NULL,
  `MinimumFine` DECIMAL NOT NULL,
  `StandardFine` DECIMAL NOT NULL,
  `MaximumFine` DECIMAL NOT NULL,
  `CanBeAppliedAutomatically` BIT NOT NULL,
  `CanBeArrested` BIT NOT NULL,
  `CanBeOfferedBail` BIT NOT NULL,
  PRIMARY KEY (`Id`),
  INDEX `FK_Laws_LegalAuthority_idx` (`LegalAuthorityId` ASC),
  INDEX `FK_Laws_FutureProgs_idx` (`LawAppliesProgId` ASC),
  CONSTRAINT `FK_Laws_LegalAuthority`
    FOREIGN KEY (`LegalAuthorityId`)
    REFERENCES `dbo`.`LegalAuthorities` (`Id`)
    ON DELETE CASCADE
    ON UPDATE CASCADE,
  CONSTRAINT `FK_Laws_FutureProgs`
    FOREIGN KEY (`LawAppliesProgId`)
    REFERENCES `dbo`.`FutureProgs` (`Id`)
    ON DELETE SET NULL
    ON UPDATE CASCADE);
CREATE TABLE `dbo`.`Laws_VictimClasses` (
  `LawId` BIGINT NOT NULL,
  `LegalClassId` BIGINT NOT NULL,
  PRIMARY KEY (`LawId`, `LegalClassId`),
  INDEX `FK_Laws_VictimClasses_LegalClasses_idx` (`LegalClassId` ASC),
  CONSTRAINT `FK_Laws_VictimClasses_Laws`
    FOREIGN KEY (`LawId`)
    REFERENCES `dbo`.`Laws` (`Id`)
    ON DELETE CASCADE
    ON UPDATE CASCADE,
  CONSTRAINT `FK_Laws_VictimClasses_LegalClasses`
    FOREIGN KEY (`LegalClassId`)
    REFERENCES `dbo`.`LegalClasses` (`Id`)
    ON DELETE CASCADE
    ON UPDATE CASCADE);
CREATE TABLE `dbo`.`Laws_OffenderClasses` (
  `LawId` BIGINT NOT NULL,
  `LegalClassId` BIGINT NOT NULL,
  PRIMARY KEY (`LawId`, `LegalClassId`),
  INDEX `FK_Laws_OffenderClasses_LegalClasses_idx` (`LegalClassId` ASC),
  CONSTRAINT `FK_Laws_OffenderClasses_Laws`
    FOREIGN KEY (`LawId`)
    REFERENCES `dbo`.`Laws` (`Id`)
    ON DELETE CASCADE
    ON UPDATE CASCADE,
  CONSTRAINT `FK_Laws_OffenderClasses_LegalClasses`
    FOREIGN KEY (`LegalClassId`)
    REFERENCES `dbo`.`LegalClasses` (`Id`)
    ON DELETE CASCADE
    ON UPDATE CASCADE);
CREATE TABLE `dbo`.`WitnessProfiles` (
  `Id` BIGINT NOT NULL AUTO_INCREMENT,
  `Name` VARCHAR(250) NOT NULL,
  `IdentityKnownProgId` BIGINT NOT NULL,
  `ReportingMultiplierProgId` BIGINT NOT NULL,
  `ReportingReliability` DOUBLE NOT NULL,
  `MinimumSkillToDetermineTimeOfDay` DOUBLE NOT NULL,
  `MinimumSkillToDetermineBiases` DOUBLE NOT NULL,
  `BaseReportingChanceNight` DOUBLE NOT NULL,
  `BaseReportingChanceDawn` DOUBLE NOT NULL,
  `BaseReportingChanceMorning` DOUBLE NOT NULL,
  `BaseReportingChanceAfternoon` DOUBLE NOT NULL,
  `BaseReportingChanceDusk` DOUBLE NOT NULL,
  PRIMARY KEY (`Id`),
  INDEX `FK_WitnessProfiles_IdentityProg_idx` (`IdentityKnownProgId` ASC),
  INDEX `FK_WitnessProfiles_MultiplierProg_idx` (`ReportingMultiplierProgId` ASC),
  CONSTRAINT `FK_WitnessProfiles_IdentityProg`
    FOREIGN KEY (`IdentityKnownProgId`)
    REFERENCES `dbo`.`FutureProgs` (`Id`)
    ON DELETE RESTRICT
    ON UPDATE CASCADE,
  CONSTRAINT `FK_WitnessProfiles_MultiplierProg`
    FOREIGN KEY (`ReportingMultiplierProgId`)
    REFERENCES `dbo`.`FutureProgs` (`Id`)
    ON DELETE RESTRICT
    ON UPDATE CASCADE);
CREATE TABLE `dbo`.`WitnessProfiles_CooperatingAuthorities` (
  `WitnessProfileId` BIGINT NOT NULL,
  `LegalAuthorityId` BIGINT NOT NULL,
  PRIMARY KEY (`WitnessProfileId`, `LegalAuthorityId`),
  INDEX `FK_WitnessProfiles_CooperatingAuthorities_LegalAuthorities_idx` (`LegalAuthorityId` ASC),
  CONSTRAINT `FK_WitnessProfiles_CooperatingAuthorities_WitnessProfiles`
    FOREIGN KEY (`WitnessProfileId`)
    REFERENCES `dbo`.`WitnessProfiles` (`Id`)
    ON DELETE CASCADE
    ON UPDATE CASCADE,
  CONSTRAINT `FK_WitnessProfiles_CooperatingAuthorities_LegalAuthorities`
    FOREIGN KEY (`LegalAuthorityId`)
    REFERENCES `dbo`.`LegalAuthorities` (`Id`)
    ON DELETE CASCADE
    ON UPDATE CASCADE);
CREATE TABLE `dbo`.`WitnessProfiles_IgnoredVictimClasses` (
  `WitnessProfileId` BIGINT NOT NULL,
  `LegalClassId` BIGINT NOT NULL,
  PRIMARY KEY (`WitnessProfileId`, `LegalClassId`),
  INDEX `FK_WitnessProfiles_IgnoredVictimClasses_LegalClasses_idx` (`LegalClassId` ASC),
  CONSTRAINT `FK_WitnessProfiles_IgnoredVictimClasses_WitnessProfiles`
    FOREIGN KEY (`WitnessProfileId`)
    REFERENCES `dbo`.`WitnessProfiles` (`Id`)
    ON DELETE CASCADE
    ON UPDATE CASCADE,
  CONSTRAINT `FK_WitnessProfiles_IgnoredVictimClasses_LegalClasses`
    FOREIGN KEY (`LegalClassId`)
    REFERENCES `dbo`.`LegalClasses` (`Id`)
    ON DELETE CASCADE
    ON UPDATE CASCADE);
CREATE TABLE `dbo`.`WitnessProfiles_IgnoredCriminalClasses` (
  `WitnessProfileId` BIGINT NOT NULL,
  `LegalClassId` BIGINT NULL,
  PRIMARY KEY (`WitnessProfileId`),
  INDEX `FK_WitnessProfiles_IgnoredCriminalClasses_LegalClasses_idx` (`LegalClassId` ASC),
  CONSTRAINT `FK_WitnessProfiles_IgnoredCriminalClasses_WitnessProfiles`
    FOREIGN KEY (`WitnessProfileId`)
    REFERENCES `dbo`.`WitnessProfiles` (`Id`)
    ON DELETE CASCADE
    ON UPDATE CASCADE,
  CONSTRAINT `FK_WitnessProfiles_IgnoredCriminalClasses_LegalClasses`
    FOREIGN KEY (`LegalClassId`)
    REFERENCES `dbo`.`LegalClasses` (`Id`)
    ON DELETE CASCADE
    ON UPDATE CASCADE);
CREATE TABLE `dbo`.`Crimes` (
  `Id` BIGINT NOT NULL AUTO_INCREMENT,
  `LawId` BIGINT NOT NULL,
  `CriminalId` BIGINT NOT NULL,
  `VictimId` BIGINT NULL,
  `TimeOfCrime` VARCHAR(200) NOT NULL,
  `RealTimeOfCrime` DATETIME NOT NULL,
  `LocationId` BIGINT NULL,
  `TimeOfReport` VARCHAR(200) NULL,
  `AccuserId` BIGINT NULL,
  `CriminalShortDescription` VARCHAR(200) NOT NULL,
  `CriminalFullDescription` VARCHAR(1000) NOT NULL,
  `CriminalCharacteristics` VARCHAR(500) NOT NULL,
  `IsKnownCrime` BIT NOT NULL,
  `IsStaleCrime` BIT NOT NULL,
  `IsFinalised` BIT NOT NULL,
  `ConvictionRecorded` BIT NOT NULL,
  `IsCriminalIdentityKnown` BIT NOT NULL,
  `BailHasBeenPosted` BIT NOT NULL,
  PRIMARY KEY (`Id`),
  INDEX `FK_Crimes_Laws_idx` (`LawId` ASC),
  INDEX `FK_Crimes_Criminal_idx` (`CriminalId` ASC),
  INDEX `FK_Crimes_Victim_idx` (`VictimId` ASC),
  INDEX `FK_Crimes_Location_idx` (`LocationId` ASC),
  INDEX `FK_Crimes_Accuser_idx` (`AccuserId` ASC),
  CONSTRAINT `FK_Crimes_Laws`
    FOREIGN KEY (`LawId`)
    REFERENCES `dbo`.`Laws` (`Id`)
    ON DELETE CASCADE
    ON UPDATE CASCADE,
  CONSTRAINT `FK_Crimes_Criminal`
    FOREIGN KEY (`CriminalId`)
    REFERENCES `dbo`.`Characters` (`Id`)
    ON DELETE CASCADE
    ON UPDATE CASCADE,
  CONSTRAINT `FK_Crimes_Victim`
    FOREIGN KEY (`VictimId`)
    REFERENCES `dbo`.`Characters` (`Id`)
    ON DELETE SET NULL
    ON UPDATE CASCADE,
  CONSTRAINT `FK_Crimes_Location`
    FOREIGN KEY (`LocationId`)
    REFERENCES `dbo`.`Cells` (`Id`)
    ON DELETE SET NULL
    ON UPDATE CASCADE,
  CONSTRAINT `FK_Crimes_Accuser`
    FOREIGN KEY (`AccuserId`)
    REFERENCES `dbo`.`Characters` (`Id`)
    ON DELETE SET NULL
    ON UPDATE CASCADE);
CREATE TABLE `labmud_dbo`.`FK_Crimes_Witnesses` (
  `CrimeId` BIGINT NOT NULL,
  `WitnessId` BIGINT NOT NULL,
  PRIMARY KEY (`CrimeId`, `WitnessId`),
  INDEX `FK_Crimes_Witnesses_Characters_idx` (`WitnessId` ASC),
  CONSTRAINT `FK_Crimes_Witnesses_Characters`
    FOREIGN KEY (`WitnessId`)
    REFERENCES `labmud_dbo`.`Characters` (`Id`)
    ON DELETE CASCADE
    ON UPDATE CASCADE,
  CONSTRAINT `FK_Crimes_Witnesses_Crime`
    FOREIGN KEY (`CrimeId`)
    REFERENCES `labmud_dbo`.`Crimes` (`Id`)
    ON DELETE CASCADE
    ON UPDATE CASCADE);
