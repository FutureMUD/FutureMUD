-- This file sets up an "Admin Avatar" race, culture, ethnicity etc

-- Admin Avatar Resource
INSERT INTO ChargenResources (`Name`, PluralName, Alias, MinimumTimeBetweenAwards, MaximumNumberAwardedPerAward, PermissionLevelRequiredToAward, PermissionLevelRequiredToCircumventMinimumTime, ShowToPlayerInScore, TextDisplayedtoPlayerOnAward, TextDisplayedToPlayerOnDeduct, MaximumResourceId, MaximumResourceFormula, `Type`)
values ('Admin Avatar Token', 'Admin Avatar Tokens', 'aat', 0, 1, 7, 8, b'0', 'You have been granted an Admin Avatar Token. This will allow you to create an Admin Avatar in character creation.', 'Your Admin Avatar Token has been deducted.', null, 1, 'Simple');

set @adminavatartoken = LAST_INSERT_ID();

-- Chargen Progs
-- Selection
INSERT INTO FutureProgs (FunctionName, FunctionComment, FunctionText, ReturnType, Category, Subcategory, Public, AcceptsAnyParameters) values
('AdminAvatarSelection', 'This program allows admins with an Admin Creation Token to create an admin Avatar.', 'return GetResource(@ch, "aat") >= 1', 4, 'Chargen', 'Admin', b'1', b'0');

set @adminselectionprog = LAST_INSERT_ID();

INSERT INTO FutureProgs_Parameters (FutureProgId, ParameterIndex, ParameterType, ParameterName) values (@adminselectionprog, 0, 8, 'ch');

INSERT INTO Races (`Name`, Description, BaseBodyId, AllowedGenders, ParentRaceId, AttributeBonusProgId, IndividualAttributeCap, DiceExpression, IlluminationPerceptionMultiplier, AvailabilityProgId, AttributeTotalCap) 
values ('Admin', 'This is the race of Admin Avatars.', @humanoid, '1 2', null, @alwayszeroprog, 100, '7d10+10', 1000, @adminselectionprog, 1000);

set @adminrace = LAST_INSERT_ID();


-- Admin Avatar Token Cost
INSERT INTO Races_ChargenResources (RaceId, ChargenResourceId, Amount) values (@adminrace, @adminavatartoken, 1);

-- Give them all the attributes
INSERT INTO Races_Attributes (RaceId, AttributeId) SELECT @adminrace, td.Id from TraitDefinitions td where td.Type = 1;

-- Admin Name Culture
INSERT INTO NameCulture (`Name`, Definition) values ('Admin', '<NameCulture>
  <Counts>
    <Count Usage="0" Min="1" Max="1"/>
  </Counts>
  <Patterns>
    <Pattern Style="0" Text="{0}" Params="0"/>
    <Pattern Style="1" Text="{0}" Params="0"/>
    <Pattern Style="2" Text="{0}" Params="0"/>
    <Pattern Style="3" Text="{0}" Params="0"/>
    <Pattern Style="4" Text="{0}" Params="0"/>
    <Pattern Style="5" Text="{0}" Params="0"/>
  </Patterns>
  <Elements>
    <Element Usage="0" MinimumCount="1" MaximumCount="1" Name="Avatar Name"><![CDATA[This is the name by which your Admin Avatar will be known. Your admin Avatar name should ideally be thematically consistent with the world in which you are an administrator. You should consult with a senior administrator to determine the eligability of any name that you have in mind before you make this decision.]]></Element>
  </Elements>
  <NameEntryRegex>^(?<birthname>[\w''-]+)$</NameEntryRegex>
</NameCulture>');
set @adminnameculture = LAST_INSERT_ID();

-- Admin Culture
INSERT INTO Cultures (`Name`, Description, NameCultureId, PersonWordMale, PersonWordFemale, PersonWordNeuter, PersonWordIndeterminate, PrimaryCalendarId, SkillStartingValueProgId, AvailabilityProgId)
values ('Admin', 'This is the culture of Admin Avatars.', @adminnameculture, 'Administrator', 'Administrator', 'Administrator', 'Administrator', @calendarid, @alwayshundredprog, @adminselectionprog);

set @adminculture = LAST_INSERT_ID();

-- Admin Ethnicity
INSERT INTO Ethnicities (`Name`, ChargenBlurb, AvailabilityProgId) VALUES ('Admin', 'This is the ethnicity of Admin Avatars.', @adminselectionprog);

set @adminethnicity = LAST_INSERT_ID();

INSERT INTO Ethnicities_Characteristics (EthnicityId, CharacteristicDefinitionId, CharacteristicProfileId) VALUES (@adminethnicity, @humanoidframechardef, (select Id from CharacteristicProfiles where TargetDefinitionId = @humanoidframechardef and `Type` = 'all' ));
INSERT INTO Ethnicities_Characteristics (EthnicityId, CharacteristicDefinitionId, CharacteristicProfileId) VALUES (@adminethnicity, @personwordchardef, (select Id from CharacteristicProfiles where TargetDefinitionId = @personwordchardef and `Type` = 'all' ));
INSERT INTO Ethnicities_Characteristics (EthnicityId, CharacteristicDefinitionId, CharacteristicProfileId) VALUES (@adminethnicity, @eyeshapechardef, (select Id from CharacteristicProfiles where TargetDefinitionId = @eyeshapechardef and `Type` = 'all' ));
INSERT INTO Ethnicities_Characteristics (EthnicityId, CharacteristicDefinitionId, CharacteristicProfileId) VALUES (@adminethnicity, @hairlengthchardef, (select Id from CharacteristicProfiles where TargetDefinitionId = @hairlengthchardef and `Type` = 'all' ));
INSERT INTO Ethnicities_Characteristics (EthnicityId, CharacteristicDefinitionId, CharacteristicProfileId) VALUES (@adminethnicity, @facialhairlengthchardef, (select Id from CharacteristicProfiles where TargetDefinitionId = @facialhairlengthchardef and `Type` = 'all' ));
INSERT INTO Ethnicities_Characteristics (EthnicityId, CharacteristicDefinitionId, CharacteristicProfileId) VALUES (@adminethnicity, @hairstylechardef, (select Id from CharacteristicProfiles where TargetDefinitionId = @hairstylechardef and `Type` = 'all' ));
INSERT INTO Ethnicities_Characteristics (EthnicityId, CharacteristicDefinitionId, CharacteristicProfileId) VALUES (@adminethnicity, @facialhairstylechardef, (select Id from CharacteristicProfiles where TargetDefinitionId = @facialhairstylechardef and `Type` = 'all' ));
INSERT INTO Ethnicities_Characteristics (EthnicityId, CharacteristicDefinitionId, CharacteristicProfileId) VALUES (@adminethnicity, @skintonechardef, (select Id from CharacteristicProfiles where TargetDefinitionId = @skintonechardef and `Type` = 'all' ));