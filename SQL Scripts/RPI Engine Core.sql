-- Roleplay Points
INSERT INTO ChargenResources (Id, `Name`, PluralName, Alias, MinimumTimeBetweenAwards, MaximumNumberAwardedPerAward, PermissionLevelRequiredToAward, PermissionLevelRequiredToCircumventMinimumTime, ShowToPlayerInScore, TextDisplayedToPlayerOnAward, TextDisplayedToPlayerOnDeduct, MaximumResourceId, MaximumResourceFormula, `Type`) VALUES (1, 'Roleplay Point', 'Roleplay Points', 'rpp', 43200, 1, 4, 7, 1, 'Congratulations, you have been awarded a Roleplay Point for your excellent conduct.', 'You have been penalised for improper behaviour, and have had a Roleplay Point deducted from your account.', NULL, '-1', 'Simple');

-- Wiznet
INSERT INTO Channels (ChannelName, ChannelListenerProgId, ChannelSpeakerProgId, AnnounceChannelJoiners, ChannelColour, `Mode`, AnnounceMissedListeners, AddToPlayerCommandTree, AddToGuideCommandTree) VALUES ('Wiznet', @isadminprog, @isadminprog, b'1', '#1', 0, b'1', b'0', b'0');
set @wiznetid = LAST_INSERT_ID();
INSERT INTO ChannelCommandWords (Word, ChannelId) VALUES ('*', @wiznetid);
INSERT INTO ChannelCommandWords (Word, ChannelId) VALUES ('wiznet', @wiznetid);

-- Core Humanoid Body
-- Wearable Rule
INSERT INTO WearableSizeParameterRule (MinHeightFactor, MaxHeightFactor, MinWeightFactor,MaxWeightFactor, WeightVolumeRatios, TraitVolumeRatios, HeightLinearRatios) VALUES (0.5, 1.5, 0.5, 1.5, '<Ratios><Ratio Item="5" Min="0" Max="0.75"/><Ratio Item="4" Min="0.75" Max="0.95"/><Ratio Item="3" Min="0.95" Max="0.99"/><Ratio Item="2" Min="0.99" Max="1.01"/><Ratio Item="3" Min="1.01" Max="1.05"/><Ratio Item="1" Min="1.05" Max="1.3"/><Ratio Item="0" Min="1.3" Max="1000"/></Ratios>', '<Ratios><Ratio Item="5" Min="0" Max="0.75"/><Ratio Item="4" Min="0.75" Max="0.95"/><Ratio Item="3" Min="0.95" Max="0.99"/><Ratio Item="2" Min="0.99" Max="1.01"/><Ratio Item="3" Min="1.01" Max="1.05"/><Ratio Item="1" Min="1.05" Max="1.3"/><Ratio Item="0" Min="1.3" Max="1000"/></Ratios>', '<Ratios><Ratio Item="0" Min="0" Max="0.90"/><Ratio Item="1" Min="0.90" Max="0.95"/><Ratio Item="2" Min="0.95" Max="0.99"/><Ratio Item="3" Min="0.99" Max="1.01"/><Ratio Item="2" Min="1.01" Max="1.05"/><Ratio Item="4" Min="1.05" Max="1.1"/><Ratio Item="5" Min="1.1" Max="1000"/></Ratios>');
set @wearrule = LAST_INSERT_ID();

-- Stamina Recovery Prog
INSERT INTO FutureProgs (FunctionName, FunctionComment, FunctionText, ReturnType, Category, Subcategory, `Public`) VALUES ('HumanoidStaminaRecoveryFunction', 'Recover constitution score in stamina per tick.', 'return GetTrait(@ch, ToTrait("Constitution"))', 2, 'Character', 'Stamina', b'0');
set @staminaprog = LAST_INSERT_ID();

INSERT INTO FutureProgs_Parameters (FutureProgId, ParameterIndex, ParameterType, ParameterName) VALUES (@staminaprog, 0, 8, 'ch');

-- Body Proto itself
INSERT INTO BodyProtos (`Name`, WearSizeParameterId, WielderDescriptionSingle, WielderDescriptionPlural, ConsiderString, StaminaRecoveryProgId) VALUES('Humanoid', @wearrule, 'hand', 'hands', '$skincolour[&he has @ skin][You cannot make out &his skin tone] and $frame[you would describe &his build as @.][you cannot make out &his build.]
&he has $eyeshape[@, $eyecolour@ eyes][eyes obscured by $].
$?hairlength[&he has &a_an$hairlength, $haircolour $hairstyle.][&he is completely bald].
$?facialhairlength[&he has &a_an$facialhairlength, $facialhaircolour $facialhairstyle].', @staminaprog);
set @humanoid = LAST_INSERT_ID();

UPDATE WearableSizeParameterRule set BodyProtoId = @humanoid where Id = @wearrule;

-- Blood Volume

insert into Futureprogs (FunctionName, FunctionComment, FunctionText, ReturnType, Category, Subcategory, Public, AcceptsAnyParameters) values ('BloodVolumeNader', 'Nadler''s Formula for Blood Volume:

Males: Vol = 0.3669 * Ht^3 + 0.03219 * Wt + 0.6041
Females: Vol = 0.3561 * Ht^3 + 0.03308 * Wt + 0.1833

Where Vol is in L, Ht is in M, and Wt is in KG', 'if (@ch.Gender == ToGender("Male"))
  return 0.3669 * ((@ch.Height / 100) ^ 0.03219) * @ch.Weight / 1000 + 0.6041
else
  return 0.3561* ((@ch.Height / 100) ^ 0.03308) * @ch.Weight / 1000 + 0.1833
end if', 4, 'Core', 'Character', b'1', b'0');

set @bloodvolumeprog = LAST_INSERT_ID();

insert into Futureprogs_Parameters (FutureProgId, ParameterIndex, ParameterType, ParameterName) values (@bloodvolumeprog, 0, 4200, 'ch');

insert into StaticConfigurations (SettingName, Definition) values ('TotalBloodVolumeProg', @bloodvolumeprog);

-- Move Speeds
INSERT INTO MoveSpeeds (BodyProtoId, Multiplier, Alias, FirstPersonVerb, ThirdPersonVerb, PresentParticiple, PositionId, StaminaMultiplier) VALUES (@humanoid, 1, 'walk', 'walk', 'walks', 'walking', 1, 1);
INSERT INTO MoveSpeeds (BodyProtoId, Multiplier, Alias, FirstPersonVerb, ThirdPersonVerb, PresentParticiple, PositionId, StaminaMultiplier) VALUES (@humanoid, 2, 'stroll', 'stroll', 'strolls', 'strolling', 1, 0.6);
INSERT INTO MoveSpeeds (BodyProtoId, Multiplier, Alias, FirstPersonVerb, ThirdPersonVerb, PresentParticiple, PositionId, StaminaMultiplier) VALUES (@humanoid, 0.75, 'jog', 'jog', 'jogs', 'jogging', 1, 1.5);
INSERT INTO MoveSpeeds (BodyProtoId, Multiplier, Alias, FirstPersonVerb, ThirdPersonVerb, PresentParticiple, PositionId, StaminaMultiplier) VALUES (@humanoid, 0.5, 'run', 'run', 'runs', 'running', 1, 1.9);
INSERT INTO MoveSpeeds (BodyProtoId, Multiplier, Alias, FirstPersonVerb, ThirdPersonVerb, PresentParticiple, PositionId, StaminaMultiplier) VALUES (@humanoid, 0.33, 'sprint', 'sprint', 'sprints', 'sprinting', 1, 2.4);
INSERT INTO MoveSpeeds (BodyProtoId, Multiplier, Alias, FirstPersonVerb, ThirdPersonVerb, PresentParticiple, PositionId, StaminaMultiplier) VALUES (@humanoid, 7, 'crawl', 'crawl', 'crawls', 'crawling', 6, 1.25);
INSERT INTO MoveSpeeds (BodyProtoId, Multiplier, Alias, FirstPersonVerb, ThirdPersonVerb, PresentParticiple, PositionId, StaminaMultiplier) VALUES (@humanoid, 4.3, 'fastcrawl', 'crawl quickly', 'crawls quickly', 'crawling quickly', 6, 2);
INSERT INTO MoveSpeeds (BodyProtoId, Multiplier, Alias, FirstPersonVerb, ThirdPersonVerb, PresentParticiple, PositionId, StaminaMultiplier) VALUES (@humanoid, 10, 'slowcrawl', 'crawl slowly', 'crawls slowly', 'crawling slowly', 6, 0.6);
INSERT INTO MoveSpeeds (BodyProtoId, Multiplier, Alias, FirstPersonVerb, ThirdPersonVerb, PresentParticiple, PositionId, StaminaMultiplier) VALUES (@humanoid, 10, 'shuffle', 'shuffle', 'shuffles', 'shuffling', 7, 2);

-- Bodypart Shapes
INSERT INTO BodypartShape (Name) VALUES ('forearm');
set @forearmshape = LAST_INSERT_ID();

INSERT INTO BodypartShape (Name) VALUES ('upperarm');
set @upperarmshape = LAST_INSERT_ID();

INSERT INTO BodypartShape (Name) VALUES ('hand');
set @handshape = LAST_INSERT_ID();

INSERT INTO BodypartShape (Name) VALUES ('finger');
set @fingershape = LAST_INSERT_ID();

INSERT INTO BodypartShape (Name) VALUES ('shoulder');
set @shouldershape = LAST_INSERT_ID();

INSERT INTO BodypartShape (Name) VALUES ('neck');
set @neckshape = LAST_INSERT_ID();

INSERT INTO BodypartShape (Name) VALUES ('head');
set @headshape = LAST_INSERT_ID();

INSERT INTO BodypartShape (Name) VALUES ('face');
set @faceshape = LAST_INSERT_ID();

INSERT INTO BodypartShape (Name) VALUES ('eye');
set @eyeshape = LAST_INSERT_ID();

INSERT INTO BodypartShape (Name) VALUES ('nose');
set @noseshape = LAST_INSERT_ID();

INSERT INTO BodypartShape (Name) VALUES ('ear');
set @earshape = LAST_INSERT_ID();

INSERT INTO BodypartShape (Name) VALUES ('chest');
set @chestshape = LAST_INSERT_ID();

INSERT INTO BodypartShape (Name) VALUES ('upperback');
set @upperbackshape = LAST_INSERT_ID();

INSERT INTO BodypartShape (Name) VALUES ('lowerback');
set @lowerbackshape = LAST_INSERT_ID();

INSERT INTO BodypartShape (Name) VALUES ('abdomen');
set @abdomenshape = LAST_INSERT_ID();

INSERT INTO BodypartShape (Name) VALUES ('hip');
set @hipshape = LAST_INSERT_ID();

INSERT INTO BodypartShape (Name) VALUES ('groin');
set @groinshape = LAST_INSERT_ID();

INSERT INTO BodypartShape (Name) VALUES ('buttocks');
set @buttocksshape = LAST_INSERT_ID();

INSERT INTO BodypartShape (Name) VALUES ('thigh');
set @thighshape = LAST_INSERT_ID();

INSERT INTO BodypartShape (Name) VALUES ('calf');
set @calfshape = LAST_INSERT_ID();

INSERT INTO BodypartShape (Name) VALUES ('ankle');
set @ankleshape = LAST_INSERT_ID();

INSERT INTO BodypartShape (Name) VALUES ('foot');
set @footshape = LAST_INSERT_ID();

INSERT INTO BodypartShape (Name) VALUES ('toe');
set @toeshape = LAST_INSERT_ID();

INSERT INTO BodypartShape (Name) VALUES ('knee');
set @kneeshape = LAST_INSERT_ID();

INSERT INTO BodypartShape (Name) VALUES ('elbow');
set @elbowshape = LAST_INSERT_ID();

INSERT INTO BodypartShape (Name) VALUES ('wrist');
set @wristshape = LAST_INSERT_ID();

INSERT INTO BodypartShape (Name) VALUES ('penis');
set @penisshape = LAST_INSERT_ID();

INSERT INTO BodypartShape (Name) VALUES ('throat');
set @throatshape = LAST_INSERT_ID();

INSERT INTO BodypartShape (Name) VALUES ('brow');
set @browshape = LAST_INSERT_ID();

INSERT INTO BodypartShape (Name) VALUES ('thumb');
set @thumbshape = LAST_INSERT_ID();

INSERT INTO BodypartShape (Name) VALUES ('buttock');
set @buttockshape = LAST_INSERT_ID();

-- Bodypart Group Describers
INSERT INTO BodypartGroupDescribers (DescribedAs, `Comment`, `Type`) VALUES ('body', 'Full Torso + All Arms', 'shape');
set @lastBGD = LAST_INSERT_ID();
INSERT INTO BodypartGroupDescribers_BodyProtos (BodyPartGroupDescriberId, BodyProtoId) VALUES (@lastBGD, @humanoid);
INSERT INTO BodypartGroupDescribers_ShapeCount (BodypartGroupDescriptionRuleId, MinCount, MaxCount, TargetId) VALUES (@lastBGD, 0, 2, @forearmshape);
INSERT INTO BodypartGroupDescribers_ShapeCount (BodypartGroupDescriptionRuleId, MinCount, MaxCount, TargetId) VALUES (@lastBGD, 0, 2, @upperarmshape);
INSERT INTO BodypartGroupDescribers_ShapeCount (BodypartGroupDescriptionRuleId, MinCount, MaxCount, TargetId) VALUES (@lastBGD, 0, 2, @shouldershape);
INSERT INTO BodypartGroupDescribers_ShapeCount (BodypartGroupDescriptionRuleId, MinCount, MaxCount, TargetId) VALUES (@lastBGD, 1, 1, @chestshape);
INSERT INTO BodypartGroupDescribers_ShapeCount (BodypartGroupDescriptionRuleId, MinCount, MaxCount, TargetId) VALUES (@lastBGD, 0, 1, @upperbackshape);
INSERT INTO BodypartGroupDescribers_ShapeCount (BodypartGroupDescriptionRuleId, MinCount, MaxCount, TargetId) VALUES (@lastBGD, 0, 1, @lowerbackshape);
INSERT INTO BodypartGroupDescribers_ShapeCount (BodypartGroupDescriptionRuleId, MinCount, MaxCount, TargetId) VALUES (@lastBGD, 0, 1, @abdomenshape);
INSERT INTO BodypartGroupDescribers_ShapeCount (BodypartGroupDescriptionRuleId, MinCount, MaxCount, TargetId) VALUES (@lastBGD, 0, 2, @hipshape);
INSERT INTO BodypartGroupDescribers_ShapeCount (BodypartGroupDescriptionRuleId, MinCount, MaxCount, TargetId) VALUES (@lastBGD, 0, 1, @groinshape);
INSERT INTO BodypartGroupDescribers_ShapeCount (BodypartGroupDescriptionRuleId, MinCount, MaxCount, TargetId) VALUES (@lastBGD, 0, 2, @elbowshape);

INSERT INTO BodypartGroupDescribers (DescribedAs, `Comment`, `Type`) VALUES ('arms', '2 full arms', 'shape');
set @lastBGD = LAST_INSERT_ID();
INSERT INTO BodypartGroupDescribers_BodyProtos (BodyPartGroupDescriberId, BodyProtoId) VALUES (@lastBGD, @humanoid);
INSERT INTO BodypartGroupDescribers_ShapeCount (BodypartGroupDescriptionRuleId, MinCount, MaxCount, TargetId) VALUES (@lastBGD, 0, 2, @forearmshape);
INSERT INTO BodypartGroupDescribers_ShapeCount (BodypartGroupDescriptionRuleId, MinCount, MaxCount, TargetId) VALUES (@lastBGD, 2, 2, @upperarmshape);

INSERT INTO BodypartGroupDescribers (DescribedAs, `Comment`, `Type`) VALUES ('forearms', '2 forearms', 'shape');
set @lastBGD = LAST_INSERT_ID();
INSERT INTO BodypartGroupDescribers_BodyProtos (BodyPartGroupDescriberId, BodyProtoId) VALUES (@lastBGD, @humanoid);
INSERT INTO BodypartGroupDescribers_ShapeCount (BodypartGroupDescriptionRuleId, MinCount, MaxCount, TargetId) VALUES (@lastBGD, 2, 2, @forearmshape);

INSERT INTO BodypartGroupDescribers (DescribedAs, `Comment`, `Type`) VALUES ('legs', 'Everything from ankles to hips', 'shape');
set @lastBGD = LAST_INSERT_ID();
INSERT INTO BodypartGroupDescribers_BodyProtos (BodyPartGroupDescriberId, BodyProtoId) VALUES (@lastBGD, @humanoid);
INSERT INTO BodypartGroupDescribers_ShapeCount (BodypartGroupDescriptionRuleId, MinCount, MaxCount, TargetId) VALUES (@lastBGD, 0, 2, @hipshape);
INSERT INTO BodypartGroupDescribers_ShapeCount (BodypartGroupDescriptionRuleId, MinCount, MaxCount, TargetId) VALUES (@lastBGD, 1, 1, @groinshape);
INSERT INTO BodypartGroupDescribers_ShapeCount (BodypartGroupDescriptionRuleId, MinCount, MaxCount, TargetId) VALUES (@lastBGD, 0, 2, @buttocksshape);
INSERT INTO BodypartGroupDescribers_ShapeCount (BodypartGroupDescriptionRuleId, MinCount, MaxCount, TargetId) VALUES (@lastBGD, 2, 2, @thighshape);
INSERT INTO BodypartGroupDescribers_ShapeCount (BodypartGroupDescriptionRuleId, MinCount, MaxCount, TargetId) VALUES (@lastBGD, 0, 2, @calfshape);
INSERT INTO BodypartGroupDescribers_ShapeCount (BodypartGroupDescriptionRuleId, MinCount, MaxCount, TargetId) VALUES (@lastBGD, 0, 2, @ankleshape);
INSERT INTO BodypartGroupDescribers_ShapeCount (BodypartGroupDescriptionRuleId, MinCount, MaxCount, TargetId) VALUES (@lastBGD, 0, 2, @kneeshape);
INSERT INTO BodypartGroupDescribers_ShapeCount (BodypartGroupDescriptionRuleId, MinCount, MaxCount, TargetId) VALUES (@lastBGD, 0, 1, @penisshape);

INSERT INTO BodypartGroupDescribers (DescribedAs, `Comment`, `Type`) VALUES ('shoulders', '2 shoulders', 'shape');
set @lastBGD = LAST_INSERT_ID();
INSERT INTO BodypartGroupDescribers_BodyProtos (BodyPartGroupDescriberId, BodyProtoId) VALUES (@lastBGD, @humanoid);
INSERT INTO BodypartGroupDescribers_ShapeCount (BodypartGroupDescriptionRuleId, MinCount, MaxCount, TargetId) VALUES (@lastBGD, 2, 2, @shouldershape);

INSERT INTO BodypartGroupDescribers (DescribedAs, `Comment`, `Type`) VALUES ('crotch', 'hips + groin', 'shape');
set @lastBGD = LAST_INSERT_ID();
INSERT INTO BodypartGroupDescribers_BodyProtos (BodyPartGroupDescriberId, BodyProtoId) VALUES (@lastBGD, @humanoid);
INSERT INTO BodypartGroupDescribers_ShapeCount (BodypartGroupDescriptionRuleId, MinCount, MaxCount, TargetId) VALUES (@lastBGD, 2, 2, @hipshape);
INSERT INTO BodypartGroupDescribers_ShapeCount (BodypartGroupDescriptionRuleId, MinCount, MaxCount, TargetId) VALUES (@lastBGD, 1, 1, @groinshape);
INSERT INTO BodypartGroupDescribers_ShapeCount (BodypartGroupDescriptionRuleId, MinCount, MaxCount, TargetId) VALUES (@lastBGD, 0, 2, @buttocksshape);
INSERT INTO BodypartGroupDescribers_ShapeCount (BodypartGroupDescriptionRuleId, MinCount, MaxCount, TargetId) VALUES (@lastBGD, 0, 1, @penisshape);

INSERT INTO BodypartGroupDescribers (DescribedAs, `Comment`, `Type`) VALUES ('eyes', 'Right and Left eyes', 'shape');
set @lastBGD = LAST_INSERT_ID();
INSERT INTO BodypartGroupDescribers_BodyProtos (BodyPartGroupDescriberId, BodyProtoId) VALUES (@lastBGD, @humanoid);
INSERT INTO BodypartGroupDescribers_ShapeCount (BodypartGroupDescriptionRuleId, MinCount, MaxCount, TargetId) VALUES (@lastBGD, 2, 2, @eyeshape);

INSERT INTO BodypartGroupDescribers (DescribedAs, `Comment`, `Type`) VALUES ('upper legs', 'Just the upper portion of the legs', 'shape');
set @lastBGD = LAST_INSERT_ID();
INSERT INTO BodypartGroupDescribers_BodyProtos (BodyPartGroupDescriberId, BodyProtoId) VALUES (@lastBGD, @humanoid);
INSERT INTO BodypartGroupDescribers_ShapeCount (BodypartGroupDescriptionRuleId, MinCount, MaxCount, TargetId) VALUES (@lastBGD, 0, 2, @hipshape);
INSERT INTO BodypartGroupDescribers_ShapeCount (BodypartGroupDescriptionRuleId, MinCount, MaxCount, TargetId) VALUES (@lastBGD, 0, 1, @groinshape);
INSERT INTO BodypartGroupDescribers_ShapeCount (BodypartGroupDescriptionRuleId, MinCount, MaxCount, TargetId) VALUES (@lastBGD, 0, 2, @buttocksshape);
INSERT INTO BodypartGroupDescribers_ShapeCount (BodypartGroupDescriptionRuleId, MinCount, MaxCount, TargetId) VALUES (@lastBGD, 2, 2, @thighshape);
INSERT INTO BodypartGroupDescribers_ShapeCount (BodypartGroupDescriptionRuleId, MinCount, MaxCount, TargetId) VALUES (@lastBGD, 0, 1, @penisshape);

INSERT INTO BodypartGroupDescribers (DescribedAs, `Comment`, `Type`) VALUES ('thighs', '2 thighs', 'shape');
set @lastBGD = LAST_INSERT_ID();
INSERT INTO BodypartGroupDescribers_BodyProtos (BodyPartGroupDescriberId, BodyProtoId) VALUES (@lastBGD, @humanoid);
INSERT INTO BodypartGroupDescribers_ShapeCount (BodypartGroupDescriptionRuleId, MinCount, MaxCount, TargetId) VALUES (@lastBGD, 2, 2, @thighshape);

INSERT INTO BodypartGroupDescribers (DescribedAs, `Comment`, `Type`) VALUES ('calves', '2 calves', 'shape');
set @lastBGD = LAST_INSERT_ID();
INSERT INTO BodypartGroupDescribers_BodyProtos (BodyPartGroupDescriberId, BodyProtoId) VALUES (@lastBGD, @humanoid);
INSERT INTO BodypartGroupDescribers_ShapeCount (BodypartGroupDescriptionRuleId, MinCount, MaxCount, TargetId) VALUES (@lastBGD, 2, 2, @calfshape);

INSERT INTO BodypartGroupDescribers (DescribedAs, `Comment`, `Type`) VALUES ('knees', '2 knees', 'shape');
set @lastBGD = LAST_INSERT_ID();
INSERT INTO BodypartGroupDescribers_BodyProtos (BodyPartGroupDescriberId, BodyProtoId) VALUES (@lastBGD, @humanoid);
INSERT INTO BodypartGroupDescribers_ShapeCount (BodypartGroupDescriptionRuleId, MinCount, MaxCount, TargetId) VALUES (@lastBGD, 2, 2, @kneeshape);

INSERT INTO BodypartGroupDescribers (DescribedAs, `Comment`, `Type`) VALUES ('elbows', '2 elbows', 'shape');
set @lastBGD = LAST_INSERT_ID();
INSERT INTO BodypartGroupDescribers_BodyProtos (BodyPartGroupDescriberId, BodyProtoId) VALUES (@lastBGD, @humanoid);
INSERT INTO BodypartGroupDescribers_ShapeCount (BodypartGroupDescriptionRuleId, MinCount, MaxCount, TargetId) VALUES (@lastBGD, 2, 2, @elbowshape);

INSERT INTO BodypartGroupDescribers (DescribedAs, `Comment`, `Type`) VALUES ('ankles', '2 ankles', 'shape');
set @lastBGD = LAST_INSERT_ID();
INSERT INTO BodypartGroupDescribers_BodyProtos (BodyPartGroupDescriberId, BodyProtoId) VALUES (@lastBGD, @humanoid);
INSERT INTO BodypartGroupDescribers_ShapeCount (BodypartGroupDescriptionRuleId, MinCount, MaxCount, TargetId) VALUES (@lastBGD, 2, 2, @ankleshape);

INSERT INTO BodypartGroupDescribers (DescribedAs, `Comment`, `Type`) VALUES ('ears', '2 ears', 'shape');
set @lastBGD = LAST_INSERT_ID();
INSERT INTO BodypartGroupDescribers_BodyProtos (BodyPartGroupDescriberId, BodyProtoId) VALUES (@lastBGD, @humanoid);
INSERT INTO BodypartGroupDescribers_ShapeCount (BodypartGroupDescriptionRuleId, MinCount, MaxCount, TargetId) VALUES (@lastBGD, 2, 2, @earshape);

INSERT INTO BodypartGroupDescribers (DescribedAs, `Comment`, `Type`) VALUES ('hips', '2 hips', 'shape');
set @lastBGD = LAST_INSERT_ID();
INSERT INTO BodypartGroupDescribers_BodyProtos (BodyPartGroupDescriberId, BodyProtoId) VALUES (@lastBGD, @humanoid);
INSERT INTO BodypartGroupDescribers_ShapeCount (BodypartGroupDescriptionRuleId, MinCount, MaxCount, TargetId) VALUES (@lastBGD, 2, 2, @hipshape);

INSERT INTO BodypartGroupDescribers (DescribedAs, `Comment`, `Type`) VALUES ('buttocks', '2 buttocks', 'shape');
set @lastBGD = LAST_INSERT_ID();
INSERT INTO BodypartGroupDescribers_BodyProtos (BodyPartGroupDescriberId, BodyProtoId) VALUES (@lastBGD, @humanoid);
INSERT INTO BodypartGroupDescribers_ShapeCount (BodypartGroupDescriptionRuleId, MinCount, MaxCount, TargetId) VALUES (@lastBGD, 2, 2, @buttocksshape);

INSERT INTO BodypartGroupDescribers (DescribedAs, `Comment`, `Type`) VALUES ('wrists', '2 wrists', 'shape');
set @lastBGD = LAST_INSERT_ID();
INSERT INTO BodypartGroupDescribers_BodyProtos (BodyPartGroupDescriberId, BodyProtoId) VALUES (@lastBGD, @humanoid);
INSERT INTO BodypartGroupDescribers_ShapeCount (BodypartGroupDescriptionRuleId, MinCount, MaxCount, TargetId) VALUES (@lastBGD, 2, 2, @wristshape);

INSERT INTO BodypartGroupDescribers (DescribedAs, `Comment`, `Type`) VALUES ('fingers', '2 or more fingers', 'shape');
set @lastBGD = LAST_INSERT_ID();
INSERT INTO BodypartGroupDescribers_BodyProtos (BodyPartGroupDescriberId, BodyProtoId) VALUES (@lastBGD, @humanoid);
INSERT INTO BodypartGroupDescribers_ShapeCount (BodypartGroupDescriptionRuleId, MinCount, MaxCount, TargetId) VALUES (@lastBGD, 2, 8, @fingershape);

INSERT INTO BodypartGroupDescribers (DescribedAs, `Comment`, `Type`) VALUES ('thumbs', '2 or more thumbs', 'shape');
set @lastBGD = LAST_INSERT_ID();
INSERT INTO BodypartGroupDescribers_BodyProtos (BodyPartGroupDescriberId, BodyProtoId) VALUES (@lastBGD, @humanoid);
INSERT INTO BodypartGroupDescribers_ShapeCount (BodypartGroupDescriptionRuleId, MinCount, MaxCount, TargetId) VALUES (@lastBGD, 2, 2, @thumbshape);

INSERT INTO BodypartGroupDescribers (DescribedAs, `Comment`, `Type`) VALUES ('hands', 'hands and fingers', 'shape');
set @lastBGD = LAST_INSERT_ID();
INSERT INTO BodypartGroupDescribers_BodyProtos (BodyPartGroupDescriberId, BodyProtoId) VALUES (@lastBGD, @humanoid);
INSERT INTO BodypartGroupDescribers_ShapeCount (BodypartGroupDescriptionRuleId, MinCount, MaxCount, TargetId) VALUES (@lastBGD, 2, 2, @handshape);
INSERT INTO BodypartGroupDescribers_ShapeCount (BodypartGroupDescriptionRuleId, MinCount, MaxCount, TargetId) VALUES (@lastBGD, 0, 8, @fingershape);
INSERT INTO BodypartGroupDescribers_ShapeCount (BodypartGroupDescriptionRuleId, MinCount, MaxCount, TargetId) VALUES (@lastBGD, 0, 2, @thumbshape);

-- Bodyparts
INSERT INTO BodypartProto (BodypartType, BodyId, `Name`, Description, BodypartShapeId, DisplayOrder, MaxLife, SeveredThreshold, PainModifier, Location, Alignment, Unary, MaxSingleSize, IsOrgan, WeightLimit, IsCore) VALUES (0, @humanoid, 'chest', 'chest', @chestshape, 10, 100, 95, 1, 0, 0, NULL, NULL, 0, 0, 1);
set @bodypartchest = LAST_INSERT_ID();
INSERT INTO BodypartProto (BodypartType, BodyId, `Name`, Description, BodypartShapeId, DisplayOrder, MaxLife, SeveredThreshold, PainModifier, Location, Alignment, Unary, MaxSingleSize, IsOrgan, WeightLimit, IsCore) VALUES (1, @humanoid, 'rhand', 'right hand', @handshape, 8, 50, 45, 1, 0, 0, 1, 6, 0, 100000, 1);
set @bodypartrhand = LAST_INSERT_ID();
INSERT INTO BodypartProto (BodypartType, BodyId, `Name`, Description, BodypartShapeId, DisplayOrder, MaxLife, SeveredThreshold, PainModifier, Location, Alignment, Unary, MaxSingleSize, IsOrgan, WeightLimit, IsCore) VALUES (1, @humanoid, 'lhand', 'left hand', @handshape, 8, 50, 45, 1, 0, 0, 1, 6, 0, 100000, 1);
set @bodypartlhand = LAST_INSERT_ID();
INSERT INTO BodypartProto (BodypartType, BodyId, `Name`, Description, BodypartShapeId, DisplayOrder, MaxLife, SeveredThreshold, PainModifier, Location, Alignment, Unary, MaxSingleSize, IsOrgan, WeightLimit, IsCore) VALUES (0, @humanoid, 'rupperarm', 'right upper arm', @upperarmshape, 4, 50, 45, 1, 0, 0, NULL, NULL, 0, 0, 1);
set @bodypartrupperarm = LAST_INSERT_ID();
INSERT INTO BodypartProto (BodypartType, BodyId, `Name`, Description, BodypartShapeId, DisplayOrder, MaxLife, SeveredThreshold, PainModifier, Location, Alignment, Unary, MaxSingleSize, IsOrgan, WeightLimit, IsCore) VALUES (0, @humanoid, 'lupperarm', 'left upper arm', @upperarmshape, 4, 50, 45, 1, 0, 0, NULL, NULL, 0, 0, 1);
set @bodypartlupperarm = LAST_INSERT_ID();
INSERT INTO BodypartProto (BodypartType, BodyId, `Name`, Description, BodypartShapeId, DisplayOrder, MaxLife, SeveredThreshold, PainModifier, Location, Alignment, Unary, MaxSingleSize, IsOrgan, WeightLimit, IsCore) VALUES (0, @humanoid, 'rcalf', 'right calf', @calfshape, 17, 60, 53, 1, 0, 0, NULL, NULL, 0, 0, 1);
set @bodypartrcalf = LAST_INSERT_ID();
INSERT INTO BodypartProto (BodypartType, BodyId, `Name`, Description, BodypartShapeId, DisplayOrder, MaxLife, SeveredThreshold, PainModifier, Location, Alignment, Unary, MaxSingleSize, IsOrgan, WeightLimit, IsCore) VALUES (0, @humanoid, 'lcalf', 'left calf', @calfshape, 17, 60, 53, 1, 0, 0, NULL, NULL, 0, 0, 1);
set @bodypartlcalf = LAST_INSERT_ID();
INSERT INTO BodypartProto (BodypartType, BodyId, `Name`, Description, BodypartShapeId, DisplayOrder, MaxLife, SeveredThreshold, PainModifier, Location, Alignment, Unary, MaxSingleSize, IsOrgan, WeightLimit, IsCore) VALUES (0, @humanoid, 'head', 'head', @headshape, 1, 30, 25, 1, 0, 0, NULL, NULL, 0, 0, 1);
set @bodyparthead = LAST_INSERT_ID();
INSERT INTO BodypartProto (BodypartType, BodyId, `Name`, Description, BodypartShapeId, DisplayOrder, MaxLife, SeveredThreshold, PainModifier, Location, Alignment, Unary, MaxSingleSize, IsOrgan, WeightLimit, IsCore) VALUES (0, @humanoid, 'lfoot', 'left foot', @footshape, 19, 30, 25, 1, 0, 0, NULL, NULL, 0, 0, 1);
set @bodypartlfoot = LAST_INSERT_ID();
INSERT INTO BodypartProto (BodypartType, BodyId, `Name`, Description, BodypartShapeId, DisplayOrder, MaxLife, SeveredThreshold, PainModifier, Location, Alignment, Unary, MaxSingleSize, IsOrgan, WeightLimit, IsCore) VALUES (0, @humanoid, 'rfoot', 'right foot', @footshape, 19, 30, 25, 1, 0, 0, NULL, NULL, 0, 0, 1);
set @bodypartrfoot = LAST_INSERT_ID();
INSERT INTO BodypartProto (BodypartType, BodyId, `Name`, Description, BodypartShapeId, DisplayOrder, MaxLife, SeveredThreshold, PainModifier, Location, Alignment, Unary, MaxSingleSize, IsOrgan, WeightLimit, IsCore) VALUES (0, @humanoid, 'leye', 'left eye', @eyeshape, 2, 10, 9, 2, 0, 0, NULL, NULL, 0, 0, 1);
set @bodypartleye = LAST_INSERT_ID();
INSERT INTO BodypartProto (BodypartType, BodyId, `Name`, Description, BodypartShapeId, DisplayOrder, MaxLife, SeveredThreshold, PainModifier, Location, Alignment, Unary, MaxSingleSize, IsOrgan, WeightLimit, IsCore) VALUES (0, @humanoid, 'reye', 'right eye', @eyeshape, 2, 10, 9, 2, 0, 0, NULL, NULL, 0, 0, 1);
set @bodypartreye = LAST_INSERT_ID();
INSERT INTO BodypartProto (BodypartType, BodyId, `Name`, Description, BodypartShapeId, DisplayOrder, MaxLife, SeveredThreshold, PainModifier, Location, Alignment, Unary, MaxSingleSize, IsOrgan, WeightLimit, IsCore) VALUES (0, @humanoid, 'rshoulder', 'right shoulder', @shouldershape, 3, 40, 35, 1, 0, 0, NULL, NULL, 0, 0, 1);
set @bodypartrshoulder = LAST_INSERT_ID();
INSERT INTO BodypartProto (BodypartType, BodyId, `Name`, Description, BodypartShapeId, DisplayOrder, MaxLife, SeveredThreshold, PainModifier, Location, Alignment, Unary, MaxSingleSize, IsOrgan, WeightLimit, IsCore) VALUES (0, @humanoid, 'lshoulder', 'left shoulder', @shouldershape, 3, 40, 35, 1, 0, 0, NULL, NULL, 0, 0, 1);
set @bodypartlshoulder = LAST_INSERT_ID();
INSERT INTO BodypartProto (BodypartType, BodyId, `Name`, Description, BodypartShapeId, DisplayOrder, MaxLife, SeveredThreshold, PainModifier, Location, Alignment, Unary, MaxSingleSize, IsOrgan, WeightLimit, IsCore) VALUES (0, @humanoid, 'face', 'face', @faceshape, 1, 30, 30, 1, 0, 0, NULL, NULL, 0, 0, 1);
set @bodypartface = LAST_INSERT_ID();
INSERT INTO BodypartProto (BodypartType, BodyId, `Name`, Description, BodypartShapeId, DisplayOrder, MaxLife, SeveredThreshold, PainModifier, Location, Alignment, Unary, MaxSingleSize, IsOrgan, WeightLimit, IsCore) VALUES (0, @humanoid, 'waist', 'waist', @hipshape, 13, 30, 30, 1, 0, 0, NULL, NULL, 0, 0, 1);
set @bodypartwaist = LAST_INSERT_ID();
INSERT INTO BodypartProto (BodypartType, BodyId, `Name`, Description, BodypartShapeId, DisplayOrder, MaxLife, SeveredThreshold, PainModifier, Location, Alignment, Unary, MaxSingleSize, IsOrgan, WeightLimit, IsCore) VALUES (0, @humanoid, 'groin', 'groin', @groinshape, 14, 30, 30, 1, 0, 0, NULL, NULL, 0, 0, 1);
set @bodypartgroin = LAST_INSERT_ID();
INSERT INTO BodypartProto (BodypartType, BodyId, `Name`, Description, BodypartShapeId, DisplayOrder, MaxLife, SeveredThreshold, PainModifier, Location, Alignment, Unary, MaxSingleSize, IsOrgan, WeightLimit, IsCore) VALUES (0, @humanoid, 'rthigh', 'right thigh', @thighshape, 15, 30, 30, 1, 0, 0, NULL, NULL, 0, 0, 1);
set @bodypartrthigh = LAST_INSERT_ID();
INSERT INTO BodypartProto (BodypartType, BodyId, `Name`, Description, BodypartShapeId, DisplayOrder, MaxLife, SeveredThreshold, PainModifier, Location, Alignment, Unary, MaxSingleSize, IsOrgan, WeightLimit, IsCore) VALUES (0, @humanoid, 'lthigh', 'left thigh', @thighshape, 15, 30, 30, 1, 0, 0, NULL, NULL, 0, 0, 1);
set @bodypartlthigh = LAST_INSERT_ID();
INSERT INTO BodypartProto (BodypartType, BodyId, `Name`, Description, BodypartShapeId, DisplayOrder, MaxLife, SeveredThreshold, PainModifier, Location, Alignment, Unary, MaxSingleSize, IsOrgan, WeightLimit, IsCore) VALUES (0, @humanoid, 'rknee', 'right knee', @kneeshape, 16, 30, 30, 1, 0, 0, NULL, NULL, 0, 0, 1);
set @bodypartrknee = LAST_INSERT_ID();
INSERT INTO BodypartProto (BodypartType, BodyId, `Name`, Description, BodypartShapeId, DisplayOrder, MaxLife, SeveredThreshold, PainModifier, Location, Alignment, Unary, MaxSingleSize, IsOrgan, WeightLimit, IsCore) VALUES (0, @humanoid, 'lknee', 'left knee', @kneeshape, 16, 30, 30, 1, 0, 0, NULL, NULL, 0, 0, 1);
set @bodypartlknee = LAST_INSERT_ID();
INSERT INTO BodypartProto (BodypartType, BodyId, `Name`, Description, BodypartShapeId, DisplayOrder, MaxLife, SeveredThreshold, PainModifier, Location, Alignment, Unary, MaxSingleSize, IsOrgan, WeightLimit, IsCore) VALUES (0, @humanoid, 'rankle', 'right ankle', @ankleshape, 18, 30, 30, 1, 0, 0, NULL, NULL, 0, 0, 1);
set @bodypartrankle = LAST_INSERT_ID();
INSERT INTO BodypartProto (BodypartType, BodyId, `Name`, Description, BodypartShapeId, DisplayOrder, MaxLife, SeveredThreshold, PainModifier, Location, Alignment, Unary, MaxSingleSize, IsOrgan, WeightLimit, IsCore) VALUES (0, @humanoid, 'lankle', 'left ankle', @ankleshape, 18, 30, 30, 1, 0, 0, NULL, NULL, 0, 0, 1);
set @bodypartlankle = LAST_INSERT_ID();
INSERT INTO BodypartProto (BodypartType, BodyId, `Name`, Description, BodypartShapeId, DisplayOrder, MaxLife, SeveredThreshold, PainModifier, Location, Alignment, Unary, MaxSingleSize, IsOrgan, WeightLimit, IsCore) VALUES (0, @humanoid, 'rforearm', 'right forearm', @forearmshape, 6, 30, 30, 1, 0, 0, NULL, NULL, 0, 0, 1);
set @bodypartrforearm = LAST_INSERT_ID();
INSERT INTO BodypartProto (BodypartType, BodyId, `Name`, Description, BodypartShapeId, DisplayOrder, MaxLife, SeveredThreshold, PainModifier, Location, Alignment, Unary, MaxSingleSize, IsOrgan, WeightLimit, IsCore) VALUES (0, @humanoid, 'lforearm', 'left forearm', @forearmshape, 6, 30, 30, 1, 0, 0, NULL, NULL, 0, 0, 1);
set @bodypartlforearm = LAST_INSERT_ID();
INSERT INTO BodypartProto (BodypartType, BodyId, `Name`, Description, BodypartShapeId, DisplayOrder, MaxLife, SeveredThreshold, PainModifier, Location, Alignment, Unary, MaxSingleSize, IsOrgan, WeightLimit, IsCore) VALUES (0, @humanoid, 'relbow', 'right elbow', @elbowshape, 5, 30, 30, 1, 0, 0, NULL, NULL, 0, 0, 1);
set @bodypartrelbow = LAST_INSERT_ID();
INSERT INTO BodypartProto (BodypartType, BodyId, `Name`, Description, BodypartShapeId, DisplayOrder, MaxLife, SeveredThreshold, PainModifier, Location, Alignment, Unary, MaxSingleSize, IsOrgan, WeightLimit, IsCore) VALUES (0, @humanoid, 'lelbow', 'left elbow', @elbowshape, 5, 30, 30, 1, 0, 0, NULL, NULL, 0, 0, 1);
set @bodypartlelbow = LAST_INSERT_ID();
INSERT INTO BodypartProto (BodypartType, BodyId, `Name`, Description, BodypartShapeId, DisplayOrder, MaxLife, SeveredThreshold, PainModifier, Location, Alignment, Unary, MaxSingleSize, IsOrgan, WeightLimit, IsCore) VALUES (0, @humanoid, 'rwrist', 'right wrist', @wristshape, 7, 30, 30, 1, 0, 0, NULL, NULL, 0, 0, 1);
set @bodypartrwrist = LAST_INSERT_ID();
INSERT INTO BodypartProto (BodypartType, BodyId, `Name`, Description, BodypartShapeId, DisplayOrder, MaxLife, SeveredThreshold, PainModifier, Location, Alignment, Unary, MaxSingleSize, IsOrgan, WeightLimit, IsCore) VALUES (0, @humanoid, 'lwrist', 'left wrist', @wristshape, 7, 30, 30, 1, 0, 0, NULL, NULL, 0, 0, 1);
set @bodypartlwrist = LAST_INSERT_ID();
INSERT INTO BodypartProto (BodypartType, BodyId, `Name`, Description, BodypartShapeId, DisplayOrder, MaxLife, SeveredThreshold, PainModifier, Location, Alignment, Unary, MaxSingleSize, IsOrgan, WeightLimit, IsCore) VALUES (0, @humanoid, 'penis', 'penis', @penisshape, 14, 10, 9, 1, 0, 0, NULL, NULL, 0, 0, 0);
set @bodypartpenis = LAST_INSERT_ID();
INSERT INTO BodypartProto (BodypartType, BodyId, `Name`, Description, BodypartShapeId, DisplayOrder, MaxLife, SeveredThreshold, PainModifier, Location, Alignment, Unary, MaxSingleSize, IsOrgan, WeightLimit, IsCore) VALUES (0, @humanoid, 'uback', 'upper back', @upperbackshape, 10, 40, 30, 1, 0, 0, NULL, NULL, 0, 0, 1);
set @bodypartuback = LAST_INSERT_ID();
INSERT INTO BodypartProto (BodypartType, BodyId, `Name`, Description, BodypartShapeId, DisplayOrder, MaxLife, SeveredThreshold, PainModifier, Location, Alignment, Unary, MaxSingleSize, IsOrgan, WeightLimit, IsCore) VALUES (0, @humanoid, 'lback', 'lower back', @lowerbackshape, 11, 40, 30, 1, 0, 0, NULL, NULL, 0, 0, 1);
set @bodypartlback = LAST_INSERT_ID();
INSERT INTO BodypartProto (BodypartType, BodyId, `Name`, Description, BodypartShapeId, DisplayOrder, MaxLife, SeveredThreshold, PainModifier, Location, Alignment, Unary, MaxSingleSize, IsOrgan, WeightLimit, IsCore) VALUES (0, @humanoid, 'rbuttock', 'right buttock', @buttockshape, 14, 30, 25, 1, 0, 0, NULL, NULL, 0, 0, 1);
set @bodypartrbuttock = LAST_INSERT_ID();
INSERT INTO BodypartProto (BodypartType, BodyId, `Name`, Description, BodypartShapeId, DisplayOrder, MaxLife, SeveredThreshold, PainModifier, Location, Alignment, Unary, MaxSingleSize, IsOrgan, WeightLimit, IsCore) VALUES (0, @humanoid, 'lbuttock', 'left buttock', @buttockshape, 14, 30, 25, 1, 0, 0, NULL, NULL, 0, 0, 1);
set @bodypartlbuttock = LAST_INSERT_ID();
INSERT INTO BodypartProto (BodypartType, BodyId, `Name`, Description, BodypartShapeId, DisplayOrder, MaxLife, SeveredThreshold, PainModifier, Location, Alignment, Unary, MaxSingleSize, IsOrgan, WeightLimit, IsCore) VALUES (0, @humanoid, 'nose', 'nose', @noseshape, 2, 15, 10, 1, 0, 0, NULL, NULL, 0, 0, 1);
set @bodypartnose = LAST_INSERT_ID();
INSERT INTO BodypartProto (BodypartType, BodyId, `Name`, Description, BodypartShapeId, DisplayOrder, MaxLife, SeveredThreshold, PainModifier, Location, Alignment, Unary, MaxSingleSize, IsOrgan, WeightLimit, IsCore) VALUES (0, @humanoid, 'neck', 'neck', @neckshape, 3, 30, 25, 1, 0, 0, NULL, NULL, 0, 0, 1);
set @bodypartneck = LAST_INSERT_ID();
INSERT INTO BodypartProto (BodypartType, BodyId, `Name`, Description, BodypartShapeId, DisplayOrder, MaxLife, SeveredThreshold, PainModifier, Location, Alignment, Unary, MaxSingleSize, IsOrgan, WeightLimit, IsCore) VALUES (0, @humanoid, 'throat', 'throat', @throatshape, 3, 15, 10, 1, 0, 0, NULL, NULL, 0, 0, 1);
set @bodypartthroat = LAST_INSERT_ID();
INSERT INTO BodypartProto (BodypartType, BodyId, `Name`, Description, BodypartShapeId, DisplayOrder, MaxLife, SeveredThreshold, PainModifier, Location, Alignment, Unary, MaxSingleSize, IsOrgan, WeightLimit, IsCore) VALUES (0, @humanoid, 'rbrow', 'right eyebrow', @browshape, 2, 5, 3, 2, 0, 0, NULL, NULL, 0, 0, 1);
set @bodypartrbrow = LAST_INSERT_ID();
INSERT INTO BodypartProto (BodypartType, BodyId, `Name`, Description, BodypartShapeId, DisplayOrder, MaxLife, SeveredThreshold, PainModifier, Location, Alignment, Unary, MaxSingleSize, IsOrgan, WeightLimit, IsCore) VALUES (0, @humanoid, 'lbrow', 'left eyebrow', @browshape, 2, 5, 3, 2, 0, 0, NULL, NULL, 0, 0, 1);
set @bodypartlbrow = LAST_INSERT_ID();
INSERT INTO BodypartProto (BodypartType, BodyId, `Name`, Description, BodypartShapeId, DisplayOrder, MaxLife, SeveredThreshold, PainModifier, Location, Alignment, Unary, MaxSingleSize, IsOrgan, WeightLimit, IsCore) VALUES (0, @humanoid, 'rthumb', 'right thumb', @thumbshape, 8, 5, 3, 1, 0, 0, NULL, NULL, 0, 0, 1);
set @bodypartrthumb = LAST_INSERT_ID();
INSERT INTO BodypartProto (BodypartType, BodyId, `Name`, Description, BodypartShapeId, DisplayOrder, MaxLife, SeveredThreshold, PainModifier, Location, Alignment, Unary, MaxSingleSize, IsOrgan, WeightLimit, IsCore) VALUES (0, @humanoid, 'lthumb', 'left thumb', @thumbshape, 8, 5, 3, 1, 0, 0, NULL, NULL, 0, 0, 1);
set @bodypartlthumb = LAST_INSERT_ID();
INSERT INTO BodypartProto (BodypartType, BodyId, `Name`, Description, BodypartShapeId, DisplayOrder, MaxLife, SeveredThreshold, PainModifier, Location, Alignment, Unary, MaxSingleSize, IsOrgan, WeightLimit, IsCore) VALUES (0, @humanoid, 'rindexfinger', 'right index finger', @fingershape, 8, 5, 3, 1, 0, 0, NULL, NULL, 0, 0, 1);
set @bodypartrindexfinger = LAST_INSERT_ID();
INSERT INTO BodypartProto (BodypartType, BodyId, `Name`, Description, BodypartShapeId, DisplayOrder, MaxLife, SeveredThreshold, PainModifier, Location, Alignment, Unary, MaxSingleSize, IsOrgan, WeightLimit, IsCore) VALUES (0, @humanoid, 'lindexfinger', 'left index finger', @fingershape, 8, 5, 3, 1, 0, 0, NULL, NULL, 0, 0, 1);
set @bodypartlindexfinger = LAST_INSERT_ID();
INSERT INTO BodypartProto (BodypartType, BodyId, `Name`, Description, BodypartShapeId, DisplayOrder, MaxLife, SeveredThreshold, PainModifier, Location, Alignment, Unary, MaxSingleSize, IsOrgan, WeightLimit, IsCore) VALUES (0, @humanoid, 'rmiddlefinger', 'right middle finger', @fingershape, 8, 5, 3, 1, 0, 0, NULL, NULL, 0, 0, 1);
set @bodypartrmiddlefinger = LAST_INSERT_ID();
INSERT INTO BodypartProto (BodypartType, BodyId, `Name`, Description, BodypartShapeId, DisplayOrder, MaxLife, SeveredThreshold, PainModifier, Location, Alignment, Unary, MaxSingleSize, IsOrgan, WeightLimit, IsCore) VALUES (0, @humanoid, 'lmiddlefinger', 'left middle finger', @fingershape, 8, 5, 3, 1, 0, 0, NULL, NULL, 0, 0, 1);
set @bodypartlmiddlefinger = LAST_INSERT_ID();
INSERT INTO BodypartProto (BodypartType, BodyId, `Name`, Description, BodypartShapeId, DisplayOrder, MaxLife, SeveredThreshold, PainModifier, Location, Alignment, Unary, MaxSingleSize, IsOrgan, WeightLimit, IsCore) VALUES (0, @humanoid, 'rringfinger', 'right ring finger', @fingershape, 8, 5, 3, 1, 0, 0, NULL, NULL, 0, 0, 1);
set @bodypartrringfinger = LAST_INSERT_ID();
INSERT INTO BodypartProto (BodypartType, BodyId, `Name`, Description, BodypartShapeId, DisplayOrder, MaxLife, SeveredThreshold, PainModifier, Location, Alignment, Unary, MaxSingleSize, IsOrgan, WeightLimit, IsCore) VALUES (0, @humanoid, 'lringfinger', 'left ring finger', @fingershape, 8, 5, 3, 1, 0, 0, NULL, NULL, 0, 0, 1);
set @bodypartlringfinger = LAST_INSERT_ID();
INSERT INTO BodypartProto (BodypartType, BodyId, `Name`, Description, BodypartShapeId, DisplayOrder, MaxLife, SeveredThreshold, PainModifier, Location, Alignment, Unary, MaxSingleSize, IsOrgan, WeightLimit, IsCore) VALUES (0, @humanoid, 'rpinkyfinger', 'right pinky finger', @fingershape, 8, 5, 3, 1, 0, 0, NULL, NULL, 0, 0, 1);
set @bodypartrpinkyfinger = LAST_INSERT_ID();
INSERT INTO BodypartProto (BodypartType, BodyId, `Name`, Description, BodypartShapeId, DisplayOrder, MaxLife, SeveredThreshold, PainModifier, Location, Alignment, Unary, MaxSingleSize, IsOrgan, WeightLimit, IsCore) VALUES (0, @humanoid, 'lpinkyfinger', 'left pinky finger', @fingershape, 8, 5, 3, 1, 0, 0, NULL, NULL, 0, 0, 1);
set @bodypartlpinkyfinger = LAST_INSERT_ID();
INSERT INTO BodypartProto (BodypartType, BodyId, `Name`, Description, BodypartShapeId, DisplayOrder, MaxLife, SeveredThreshold, PainModifier, Location, Alignment, Unary, MaxSingleSize, IsOrgan, WeightLimit, IsCore) VALUES (0, @humanoid, 'belly', 'belly', @abdomenshape, 11, 20, 15, 1, 0, 0, NULL, NULL, 0, 0, 1);
set @bodypartbelly = LAST_INSERT_ID();

-- Connect up all the bodyparts
INSERT INTO BodypartProto_BodypartProto_Upstream (Child, Parent) VALUES (@bodypartrhand, @bodypartrwrist);
INSERT INTO BodypartProto_BodypartProto_Upstream (Child, Parent) VALUES (@bodypartlhand, @bodypartlwrist);
INSERT INTO BodypartProto_BodypartProto_Upstream (Child, Parent) VALUES (@bodypartrupperarm, @bodypartrshoulder);
INSERT INTO BodypartProto_BodypartProto_Upstream (Child, Parent) VALUES (@bodypartlupperarm, @bodypartlshoulder);
INSERT INTO BodypartProto_BodypartProto_Upstream (Child, Parent) VALUES (@bodypartrcalf, @bodypartrknee);
INSERT INTO BodypartProto_BodypartProto_Upstream (Child, Parent) VALUES (@bodypartlcalf, @bodypartlknee);
INSERT INTO BodypartProto_BodypartProto_Upstream (Child, Parent) VALUES (@bodyparthead, @bodypartneck);
INSERT INTO BodypartProto_BodypartProto_Upstream (Child, Parent) VALUES (@bodypartlfoot, @bodypartlankle);
INSERT INTO BodypartProto_BodypartProto_Upstream (Child, Parent) VALUES (@bodypartrfoot, @bodypartrankle);
INSERT INTO BodypartProto_BodypartProto_Upstream (Child, Parent) VALUES (@bodypartleye, @bodypartface);
INSERT INTO BodypartProto_BodypartProto_Upstream (Child, Parent) VALUES (@bodypartreye, @bodypartface);
INSERT INTO BodypartProto_BodypartProto_Upstream (Child, Parent) VALUES (@bodypartrshoulder, @bodypartchest);
INSERT INTO BodypartProto_BodypartProto_Upstream (Child, Parent) VALUES (@bodypartlshoulder, @bodypartchest);
INSERT INTO BodypartProto_BodypartProto_Upstream (Child, Parent) VALUES (@bodypartface, @bodyparthead);
INSERT INTO BodypartProto_BodypartProto_Upstream (Child, Parent) VALUES (@bodypartwaist, @bodypartbelly);
INSERT INTO BodypartProto_BodypartProto_Upstream (Child, Parent) VALUES (@bodypartgroin, @bodypartwaist);
INSERT INTO BodypartProto_BodypartProto_Upstream (Child, Parent) VALUES (@bodypartrthigh, @bodypartwaist);
INSERT INTO BodypartProto_BodypartProto_Upstream (Child, Parent) VALUES (@bodypartlthigh, @bodypartwaist);
INSERT INTO BodypartProto_BodypartProto_Upstream (Child, Parent) VALUES (@bodypartrknee, @bodypartrthigh);
INSERT INTO BodypartProto_BodypartProto_Upstream (Child, Parent) VALUES (@bodypartlknee, @bodypartlthigh);
INSERT INTO BodypartProto_BodypartProto_Upstream (Child, Parent) VALUES (@bodypartrankle, @bodypartrcalf);
INSERT INTO BodypartProto_BodypartProto_Upstream (Child, Parent) VALUES (@bodypartlankle, @bodypartlcalf);
INSERT INTO BodypartProto_BodypartProto_Upstream (Child, Parent) VALUES (@bodypartrforearm, @bodypartrelbow);
INSERT INTO BodypartProto_BodypartProto_Upstream (Child, Parent) VALUES (@bodypartlforearm, @bodypartlelbow);
INSERT INTO BodypartProto_BodypartProto_Upstream (Child, Parent) VALUES (@bodypartrelbow, @bodypartrupperarm);
INSERT INTO BodypartProto_BodypartProto_Upstream (Child, Parent) VALUES (@bodypartlelbow, @bodypartlupperarm);
INSERT INTO BodypartProto_BodypartProto_Upstream (Child, Parent) VALUES (@bodypartrwrist, @bodypartrforearm);
INSERT INTO BodypartProto_BodypartProto_Upstream (Child, Parent) VALUES (@bodypartlwrist, @bodypartlforearm);
INSERT INTO BodypartProto_BodypartProto_Upstream (Child, Parent) VALUES (@bodypartpenis, @bodypartgroin);
INSERT INTO BodypartProto_BodypartProto_Upstream (Child, Parent) VALUES (@bodypartuback, @bodypartchest);
INSERT INTO BodypartProto_BodypartProto_Upstream (Child, Parent) VALUES (@bodypartlback, @bodypartbelly);
INSERT INTO BodypartProto_BodypartProto_Upstream (Child, Parent) VALUES (@bodypartrbuttock, @bodypartlback);
INSERT INTO BodypartProto_BodypartProto_Upstream (Child, Parent) VALUES (@bodypartlbuttock, @bodypartlback);
INSERT INTO BodypartProto_BodypartProto_Upstream (Child, Parent) VALUES (@bodypartnose, @bodypartface);
INSERT INTO BodypartProto_BodypartProto_Upstream (Child, Parent) VALUES (@bodypartneck, @bodypartchest);
INSERT INTO BodypartProto_BodypartProto_Upstream (Child, Parent) VALUES (@bodypartthroat, @bodypartchest);
INSERT INTO BodypartProto_BodypartProto_Upstream (Child, Parent) VALUES (@bodypartrbrow, @bodypartface);
INSERT INTO BodypartProto_BodypartProto_Upstream (Child, Parent) VALUES (@bodypartlbrow, @bodypartface);
INSERT INTO BodypartProto_BodypartProto_Upstream (Child, Parent) VALUES (@bodypartrthumb, @bodypartrhand);
INSERT INTO BodypartProto_BodypartProto_Upstream (Child, Parent) VALUES (@bodypartlthumb, @bodypartlhand);
INSERT INTO BodypartProto_BodypartProto_Upstream (Child, Parent) VALUES (@bodypartrindexfinger, @bodypartrhand);
INSERT INTO BodypartProto_BodypartProto_Upstream (Child, Parent) VALUES (@bodypartlindexfinger, @bodypartlhand);
INSERT INTO BodypartProto_BodypartProto_Upstream (Child, Parent) VALUES (@bodypartrmiddlefinger, @bodypartrhand);
INSERT INTO BodypartProto_BodypartProto_Upstream (Child, Parent) VALUES (@bodypartlmiddlefinger, @bodypartlhand);
INSERT INTO BodypartProto_BodypartProto_Upstream (Child, Parent) VALUES (@bodypartrringfinger, @bodypartrhand);
INSERT INTO BodypartProto_BodypartProto_Upstream (Child, Parent) VALUES (@bodypartlringfinger, @bodypartlhand);
INSERT INTO BodypartProto_BodypartProto_Upstream (Child, Parent) VALUES (@bodypartrpinkyfinger, @bodypartrhand);
INSERT INTO BodypartProto_BodypartProto_Upstream (Child, Parent) VALUES (@bodypartlpinkyfinger, @bodypartlhand);
INSERT INTO BodypartProto_BodypartProto_Upstream (Child, Parent) VALUES (@bodypartbelly, @bodypartchest);

-- Extra bodypart definitions
INSERT INTO bodyprotos_additionalbodyparts (BodyProtoId, BodypartId, `Usage`) VALUES (@humanoid, @bodypartpenis, 'male');

-- Stack Decorators
INSERT INTO StackDecorators (`Name`, `Type`, Definition, Description) VALUES ('Simple Suffix', 'Suffix', '<Definition/>', 'Appends (xN) after the item''s description if quantity > 0, where N is the quantity');
INSERT INTO StackDecorators (`Name`, `Type`, Definition, Description) VALUES ('Simple Pile', 'Pile', '<Definition><Range Item="a couple of " Min="1" Max="2"/><Range Item="a few " Min="2" Max="5"/><Range Item="some " Min="5" Max="10"/><Range Item="numerous " Min="10" Max="30"/><Range Item="many " Min="30" Max="100"/><Range Item="a great many " Min="100" Max="1000"/><Range Item="an enormous quantity of " Min="1000" Max="10000"/><Range Item="countless " Min="10000" Max="1000000"/></Definition>', 'Describes stacks in abstract terms like "some", "a few", "many"');
INSERT INTO StackDecorators (`Name`, `Type`, Definition, Description) VALUES ('Simple Food Pile', 'Bites', '<Definition><Range Item="scraps of {0}" Min="0" Max="15"/><Range Item="a small amount of {0}" Min="15" Max="25"/><Range Item="less than half of {0}" Min="25" Max="40"/><Range Item="about half of {0}" Min="40" Max="60"/><Range Item="most of {0}" Min="60" Max="90"/><Range Item="almost all of {0}" Min="90" Max="100"/></Definition>', 'Describes bites remaining for food in general terms');
-- Food Stack Decorator Config
INSERT INTO StaticConfigurations (SettingName, Definition) VALUES ('DefaultFoodStackDecorator', LAST_INSERT_ID());

-- Socials
INSERT INTO Socials (`Name`, NoTargetEcho, OneTargetEcho, FutureProgId, DirectionTargetEcho, MultiTargetEcho) VALUES ('adminstuff', '@ do|does admin stuff', '@ do|does admin stuff for $1', @adminprog, '@ do|does admin stuff towards {0}', '@ do|does admin stuff for {0}');
INSERT INTO Socials (`Name`, NoTargetEcho, OneTargetEcho, FutureProgId, DirectionTargetEcho, MultiTargetEcho) VALUES ('gan', '@ grin|grins and nod|nods', '@ grin|grins and nod|nods at $1', NULL, '', '@ grin|grins and nod|nods to {0}');
INSERT INTO Socials (`Name`, NoTargetEcho, OneTargetEcho, FutureProgId, DirectionTargetEcho, MultiTargetEcho) VALUES ('grin', '@ grin|grins', '@ grin|grins at $1', NULL, '', '@ grin|grins at {0}');
INSERT INTO Socials (`Name`, NoTargetEcho, OneTargetEcho, FutureProgId, DirectionTargetEcho, MultiTargetEcho) VALUES ('nod', '@ nod|nods', '@ nod|nods to $1', NULL, '@ nod|nods towards {0}', '@ nod|nods to {0} each in turn');
INSERT INTO Socials (`Name`, NoTargetEcho, OneTargetEcho, FutureProgId, DirectionTargetEcho, MultiTargetEcho) VALUES ('smile', '@ smile|smiles', '@ smile|smiles at $1', NULL, '', '@ smile|smiles at {0}');
INSERT INTO Socials (`Name`, NoTargetEcho, OneTargetEcho, FutureProgId, DirectionTargetEcho, MultiTargetEcho) VALUES ('shake', '@ shake|shakes &0\'s head', '@ shake|shakes &0\'s head at $1', NULL, '', '@ shake|shakes &0\'s head at {0}');

-- Static Configurations
INSERT INTO StaticConfigurations (SettingName, Definition) VALUES ('PlayersCanCreateClansProg', CAST(@alwaystrueprog as CHAR(10)));
INSERT INTO StaticConfigurations (SettingName, Definition) VALUES ('RecordOnlinePlayers', '0');
INSERT INTO StaticConfigurations (SettingName, Definition) VALUES ('RecordOnlinePlayersDateTime', '01/01/1900 0:00');

-- Hearing Profiles
INSERT INTO HearingProfiles (`Name`, Definition, `Type`, SurveyDescription) VALUES ('Universal', '<Definition>
  <Difficulties>
    <Difficulty Volume="0" Proximity="0">2</Difficulty>
    <Difficulty Volume="1" Proximity="0">1</Difficulty>
    <Difficulty Volume="0" Proximity="1">3</Difficulty>
    <Difficulty Volume="1" Proximity="1">2</Difficulty>
    <Difficulty Volume="2" Proximity="1">1</Difficulty>
    <Difficulty Volume="0" Proximity="2">4</Difficulty>
    <Difficulty Volume="1" Proximity="2">3</Difficulty>
    <Difficulty Volume="2" Proximity="2">2</Difficulty>
    <Difficulty Volume="0" Proximity="3">7</Difficulty>
    <Difficulty Volume="1" Proximity="3">6</Difficulty>
    <Difficulty Volume="2" Proximity="3">5</Difficulty>
    <Difficulty Volume="3" Proximity="3">4</Difficulty>
    <Difficulty Volume="4" Proximity="3">3</Difficulty>
    <Difficulty Volume="5" Proximity="3">2</Difficulty>
    <Difficulty Volume="6" Proximity="3">1</Difficulty>
  </Difficulties>
</Definition>', 'Simple', 'The noise level is generally low and otherwise unremarkable.');

INSERT INTO HearingProfiles (`Name`, Definition, `Type`, SurveyDescription) VALUES ('Noisy', '<Definition>
  <Difficulties>
    <Difficulty Volume="0" Proximity="0">3</Difficulty>
    <Difficulty Volume="1" Proximity="0">2</Difficulty>
    <Difficulty Volume="2" Proximity="0">1</Difficulty>
    <Difficulty Volume="0" Proximity="1">4</Difficulty>
    <Difficulty Volume="1" Proximity="1">3</Difficulty>
    <Difficulty Volume="2" Proximity="1">2</Difficulty>
    <Difficulty Volume="3" Proximity="1">1</Difficulty>
    <Difficulty Volume="0" Proximity="2">5</Difficulty>
    <Difficulty Volume="1" Proximity="2">4</Difficulty>
    <Difficulty Volume="2" Proximity="2">3</Difficulty>
    <Difficulty Volume="3" Proximity="2">2</Difficulty>
    <Difficulty Volume="4" Proximity="2">1</Difficulty>
    <Difficulty Volume="0" Proximity="3">7</Difficulty>
    <Difficulty Volume="1" Proximity="3">7</Difficulty>
    <Difficulty Volume="2" Proximity="3">6</Difficulty>
    <Difficulty Volume="3" Proximity="3">5</Difficulty>
    <Difficulty Volume="4" Proximity="3">4</Difficulty>
    <Difficulty Volume="5" Proximity="3">3</Difficulty>
    <Difficulty Volume="6" Proximity="3">2</Difficulty>
  </Difficulties>
</Definition>', 'Simple', 'The noise level here is slightly raised, making hearing harder.');

-- Terrain
INSERT INTO Terrains (`Name`, MovementRate, DefaultTerrain, TerrainBehaviourMode, HideDifficulty, SpotDifficulty, StaminaCost) VALUES ('City',  1, 1, 'Standard', 2, 4, 10);
INSERT INTO Terrains (`Name`, MovementRate, DefaultTerrain, TerrainBehaviourMode, HideDifficulty, SpotDifficulty, StaminaCost) VALUES ('Good Road',  0.5, 0, 'Standard', 4, 3, 8);
INSERT INTO Terrains (`Name`, MovementRate, DefaultTerrain, TerrainBehaviourMode, HideDifficulty, SpotDifficulty, StaminaCost) VALUES ('Road',  0.66, 0, 'Standard', 3, 3, 10);
INSERT INTO Terrains (`Name`, MovementRate, DefaultTerrain, TerrainBehaviourMode, HideDifficulty, SpotDifficulty, StaminaCost) VALUES ('Dirt Road',  0.75, 0, 'Standard', 3, 3, 11);
INSERT INTO Terrains (`Name`, MovementRate, DefaultTerrain, TerrainBehaviourMode, HideDifficulty, SpotDifficulty, StaminaCost) VALUES ('Plains',  1.25, 0, 'Standard', 4, 2, 20);
INSERT INTO Terrains (`Name`, MovementRate, DefaultTerrain, TerrainBehaviourMode, HideDifficulty, SpotDifficulty, StaminaCost) VALUES ('Swamp',  4, 0, 'Standard', 1, 4, 30);
INSERT INTO Terrains (`Name`, MovementRate, DefaultTerrain, TerrainBehaviourMode, HideDifficulty, SpotDifficulty, StaminaCost) VALUES ('Wasteland',  1.4, 0, 'Standard', 2, 2, 25);
INSERT INTO Terrains (`Name`, MovementRate, DefaultTerrain, TerrainBehaviourMode, HideDifficulty, SpotDifficulty, StaminaCost) VALUES ('Forest',  2.5, 0, 'Standard', 1, 4, 30);
INSERT INTO Terrains (`Name`, MovementRate, DefaultTerrain, TerrainBehaviourMode, HideDifficulty, SpotDifficulty, StaminaCost) VALUES ('Woodland',  1.75, 0, 'Standard', 2, 4, 25);
INSERT INTO Terrains (`Name`, MovementRate, DefaultTerrain, TerrainBehaviourMode, HideDifficulty, SpotDifficulty, StaminaCost) VALUES ('Shallow Water',  2.5, 0, 'Standard', 2, 3, 50);
INSERT INTO Terrains (`Name`, MovementRate, DefaultTerrain, TerrainBehaviourMode, HideDifficulty, SpotDifficulty, StaminaCost) VALUES ('Deep Water',  10, 0, 'Standard', 5, 3, 100);
INSERT INTO Terrains (`Name`, MovementRate, DefaultTerrain, TerrainBehaviourMode, HideDifficulty, SpotDifficulty, StaminaCost) VALUES ('Hilly',  2, 0, 'Standard', 2, 3, 35);
INSERT INTO Terrains (`Name`, MovementRate, DefaultTerrain, TerrainBehaviourMode, HideDifficulty, SpotDifficulty, StaminaCost) VALUES ('Frictionless',  0, 0, 'Standard', 0, 3, 0);
INSERT INTO Terrains (`Name`, MovementRate, DefaultTerrain, TerrainBehaviourMode, HideDifficulty, SpotDifficulty, StaminaCost) VALUES ('Deep Forest',  3.5, 0, 'Standard', 0, 5, 40);
INSERT INTO Terrains (`Name`, MovementRate, DefaultTerrain, TerrainBehaviourMode, HideDifficulty, SpotDifficulty, StaminaCost) VALUES ('Beach',  1.8, 0, 'Standard', 4, 3, 25);
INSERT INTO Terrains (`Name`, MovementRate, DefaultTerrain, TerrainBehaviourMode, HideDifficulty, SpotDifficulty, StaminaCost) VALUES ('Indoors',  1, 0, 'Standard', 3, 1, 8);
INSERT INTO Terrains (`Name`, MovementRate, DefaultTerrain, TerrainBehaviourMode, HideDifficulty, SpotDifficulty, StaminaCost) VALUES ('Refuse Heap',  4, 0, 'Standard', 0, 4, 40);
INSERT INTO Terrains (`Name`, MovementRate, DefaultTerrain, TerrainBehaviourMode, HideDifficulty, SpotDifficulty, StaminaCost) VALUES ('Jungle',  8, 0, 'Standard', 0, 6, 50);

-- TimeZoneInfos
INSERT INTO TimeZoneInfos (Id, Display, `Order`) VALUES ('Afghanistan Standard Time', '(UTC+04:30) Kabul', 4.50);
INSERT INTO TimeZoneInfos (Id, Display, `Order`) VALUES ('Alaskan Standard Time', '(UTC-09:00) Alaska', -9.00);
INSERT INTO TimeZoneInfos (Id, Display, `Order`) VALUES ('Arab Standard Time', '(UTC+03:00) Kuwait, Riyadh', 3.00);
INSERT INTO TimeZoneInfos (Id, Display, `Order`) VALUES ('Arabian Standard Time', '(UTC+04:00) Abu Dhabi, Muscat', 4.00);
INSERT INTO TimeZoneInfos (Id, Display, `Order`) VALUES ('Arabic Standard Time', '(UTC+03:00) Baghdad', 3.00);
INSERT INTO TimeZoneInfos (Id, Display, `Order`) VALUES ('Argentina Standard Time', '(UTC-03:00) Buenos Aires', -3.00);
INSERT INTO TimeZoneInfos (Id, Display, `Order`) VALUES ('Atlantic Standard Time', '(UTC-04:00) Atlantic Time (Canada)', -4.00);
INSERT INTO TimeZoneInfos (Id, Display, `Order`) VALUES ('AUS Central Standard Time', '(UTC+09:30) Darwin', 9.50);
INSERT INTO TimeZoneInfos (Id, Display, `Order`) VALUES ('AUS Eastern Standard Time', '(UTC+10:00) Canberra, Melbourne, Sydney', 10.00);
INSERT INTO TimeZoneInfos (Id, Display, `Order`) VALUES ('Azerbaijan Standard Time', '(UTC+04:00) Baku', 4.00);
INSERT INTO TimeZoneInfos (Id, Display, `Order`) VALUES ('Azores Standard Time', '(UTC-01:00) Azores', -1.00);
INSERT INTO TimeZoneInfos (Id, Display, `Order`) VALUES ('Bahia Standard Time', '(UTC-03:00) Salvador', -3.00);
INSERT INTO TimeZoneInfos (Id, Display, `Order`) VALUES ('Bangladesh Standard Time', '(UTC+06:00) Dhaka', 6.00);
INSERT INTO TimeZoneInfos (Id, Display, `Order`) VALUES ('Canada Central Standard Time', '(UTC-06:00) Saskatchewan', -6.00);
INSERT INTO TimeZoneInfos (Id, Display, `Order`) VALUES ('Cape Verde Standard Time', '(UTC-01:00) Cape Verde Is.', -1.00);
INSERT INTO TimeZoneInfos (Id, Display, `Order`) VALUES ('Caucasus Standard Time', '(UTC+04:00) Yerevan', 4.00);
INSERT INTO TimeZoneInfos (Id, Display, `Order`) VALUES ('Cen. Australia Standard Time', '(UTC+09:30) Adelaide', 9.50);
INSERT INTO TimeZoneInfos (Id, Display, `Order`) VALUES ('Central America Standard Time', '(UTC-06:00) Central America', -6.00);
INSERT INTO TimeZoneInfos (Id, Display, `Order`) VALUES ('Central Asia Standard Time', '(UTC+06:00) Astana', 6.00);
INSERT INTO TimeZoneInfos (Id, Display, `Order`) VALUES ('Central Brazilian Standard Time', '(UTC-04:00) Cuiaba', -4.00);
INSERT INTO TimeZoneInfos (Id, Display, `Order`) VALUES ('Central Europe Standard Time', '(UTC+01:00) Belgrade, Bratislava, Budapest, Ljubljana, Prague', 1.00);
INSERT INTO TimeZoneInfos (Id, Display, `Order`) VALUES ('Central European Standard Time', '(UTC+01:00) Sarajevo, Skopje, Warsaw, Zagreb', 1.00);
INSERT INTO TimeZoneInfos (Id, Display, `Order`) VALUES ('Central Pacific Standard Time', '(UTC+11:00) Solomon Is., New Caledonia', 11.00);
INSERT INTO TimeZoneInfos (Id, Display, `Order`) VALUES ('Central Standard Time', '(UTC-06:00) Central Time (US & Canada)', -6.00);
INSERT INTO TimeZoneInfos (Id, Display, `Order`) VALUES ('Central Standard Time (Mexico)', '(UTC-06:00) Guadalajara, Mexico City, Monterrey', -6.00);
INSERT INTO TimeZoneInfos (Id, Display, `Order`) VALUES ('China Standard Time', '(UTC+08:00) Beijing, Chongqing, Hong Kong, Urumqi', 8.00);
INSERT INTO TimeZoneInfos (Id, Display, `Order`) VALUES ('Dateline Standard Time', '(UTC-12:00) International Date Line West', -12.00);
INSERT INTO TimeZoneInfos (Id, Display, `Order`) VALUES ('E. Africa Standard Time', '(UTC+03:00) Nairobi', 3.00);
INSERT INTO TimeZoneInfos (Id, Display, `Order`) VALUES ('E. Australia Standard Time', '(UTC+10:00) Brisbane', 10.00);
INSERT INTO TimeZoneInfos (Id, Display, `Order`) VALUES ('E. Europe Standard Time', '(UTC+02:00) E. Europe', 2.00);
INSERT INTO TimeZoneInfos (Id, Display, `Order`) VALUES ('E. South America Standard Time', '(UTC-03:00) Brasilia', -3.00);
INSERT INTO TimeZoneInfos (Id, Display, `Order`) VALUES ('Eastern Standard Time', '(UTC-05:00) Eastern Time (US & Canada)', -5.00);
INSERT INTO TimeZoneInfos (Id, Display, `Order`) VALUES ('Egypt Standard Time', '(UTC+02:00) Cairo', 2.00);
INSERT INTO TimeZoneInfos (Id, Display, `Order`) VALUES ('Ekaterinburg Standard Time', '(UTC+06:00) Ekaterinburg', 6.00);
INSERT INTO TimeZoneInfos (Id, Display, `Order`) VALUES ('Fiji Standard Time', '(UTC+12:00) Fiji', 12.00);
INSERT INTO TimeZoneInfos (Id, Display, `Order`) VALUES ('FLE Standard Time', '(UTC+02:00) Helsinki, Kyiv, Riga, Sofia, Tallinn, Vilnius', 2.00);
INSERT INTO TimeZoneInfos (Id, Display, `Order`) VALUES ('Georgian Standard Time', '(UTC+04:00) Tbilisi', 4.00);
INSERT INTO TimeZoneInfos (Id, Display, `Order`) VALUES ('GMT Standard Time', '(UTC) Dublin, Edinburgh, Lisbon, London', 0.00);
INSERT INTO TimeZoneInfos (Id, Display, `Order`) VALUES ('Greenland Standard Time', '(UTC-03:00) Greenland', -3.00);
INSERT INTO TimeZoneInfos (Id, Display, `Order`) VALUES ('Greenwich Standard Time', '(UTC) Monrovia, Reykjavik', 0.00);
INSERT INTO TimeZoneInfos (Id, Display, `Order`) VALUES ('GTB Standard Time', '(UTC+02:00) Athens, Bucharest', 2.00);
INSERT INTO TimeZoneInfos (Id, Display, `Order`) VALUES ('Hawaiian Standard Time', '(UTC-10:00) Hawaii', -10.00);
INSERT INTO TimeZoneInfos (Id, Display, `Order`) VALUES ('India Standard Time', '(UTC+05:30) Chennai, Kolkata, Mumbai, New Delhi', 5.50);
INSERT INTO TimeZoneInfos (Id, Display, `Order`) VALUES ('Iran Standard Time', '(UTC+03:30) Tehran', 3.50);
INSERT INTO TimeZoneInfos (Id, Display, `Order`) VALUES ('Israel Standard Time', '(UTC+02:00) Jerusalem', 2.00);
INSERT INTO TimeZoneInfos (Id, Display, `Order`) VALUES ('Jordan Standard Time', '(UTC+03:00) Amman', 3.00);
INSERT INTO TimeZoneInfos (Id, Display, `Order`) VALUES ('Kaliningrad Standard Time', '(UTC+03:00) Kaliningrad, Minsk', 3.00);
INSERT INTO TimeZoneInfos (Id, Display, `Order`) VALUES ('Kamchatka Standard Time', '(UTC+12:00) Petropavlovsk-Kamchatsky - Old', 12.00);
INSERT INTO TimeZoneInfos (Id, Display, `Order`) VALUES ('Korea Standard Time', '(UTC+09:00) Seoul', 9.00);
INSERT INTO TimeZoneInfos (Id, Display, `Order`) VALUES ('Libya Standard Time', '(UTC+02:00) Tripoli', 2.00);
INSERT INTO TimeZoneInfos (Id, Display, `Order`) VALUES ('Magadan Standard Time', '(UTC+12:00) Magadan', 12.00);
INSERT INTO TimeZoneInfos (Id, Display, `Order`) VALUES ('Mauritius Standard Time', '(UTC+04:00) Port Louis', 4.00);
INSERT INTO TimeZoneInfos (Id, Display, `Order`) VALUES ('Mid-Atlantic Standard Time', '(UTC-02:00) Mid-Atlantic - Old', -2.00);
INSERT INTO TimeZoneInfos (Id, Display, `Order`) VALUES ('Middle East Standard Time', '(UTC+02:00) Beirut', 2.00);
INSERT INTO TimeZoneInfos (Id, Display, `Order`) VALUES ('Montevideo Standard Time', '(UTC-03:00) Montevideo', -3.00);
INSERT INTO TimeZoneInfos (Id, Display, `Order`) VALUES ('Morocco Standard Time', '(UTC) Casablanca', 0.00);
INSERT INTO TimeZoneInfos (Id, Display, `Order`) VALUES ('Mountain Standard Time', '(UTC-07:00) Mountain Time (US & Canada)', -7.00);
INSERT INTO TimeZoneInfos (Id, Display, `Order`) VALUES ('Mountain Standard Time (Mexico)', '(UTC-07:00) Chihuahua, La Paz, Mazatlan', -7.00);
INSERT INTO TimeZoneInfos (Id, Display, `Order`) VALUES ('Myanmar Standard Time', '(UTC+06:30) Yangon (Rangoon)', 6.50);
INSERT INTO TimeZoneInfos (Id, Display, `Order`) VALUES ('N. Central Asia Standard Time', '(UTC+07:00) Novosibirsk', 7.00);
INSERT INTO TimeZoneInfos (Id, Display, `Order`) VALUES ('Namibia Standard Time', '(UTC+01:00) Windhoek', 1.00);
INSERT INTO TimeZoneInfos (Id, Display, `Order`) VALUES ('Nepal Standard Time', '(UTC+05:45) Kathmandu', 5.75);
INSERT INTO TimeZoneInfos (Id, Display, `Order`) VALUES ('New Zealand Standard Time', '(UTC+12:00) Auckland, Wellington', 12.00);
INSERT INTO TimeZoneInfos (Id, Display, `Order`) VALUES ('Newfoundland Standard Time', '(UTC-03:30) Newfoundland', -3.50);
INSERT INTO TimeZoneInfos (Id, Display, `Order`) VALUES ('North Asia East Standard Time', '(UTC+09:00) Irkutsk', 9.00);
INSERT INTO TimeZoneInfos (Id, Display, `Order`) VALUES ('North Asia Standard Time', '(UTC+08:00) Krasnoyarsk', 8.00);
INSERT INTO TimeZoneInfos (Id, Display, `Order`) VALUES ('Pacific SA Standard Time', '(UTC-04:00) Santiago', -4.00);
INSERT INTO TimeZoneInfos (Id, Display, `Order`) VALUES ('Pacific Standard Time', '(UTC-08:00) Pacific Time (US & Canada)', -8.00);
INSERT INTO TimeZoneInfos (Id, Display, `Order`) VALUES ('Pacific Standard Time (Mexico)', '(UTC-08:00) Baja California', -8.00);
INSERT INTO TimeZoneInfos (Id, Display, `Order`) VALUES ('Pakistan Standard Time', '(UTC+05:00) Islamabad, Karachi', 5.00);
INSERT INTO TimeZoneInfos (Id, Display, `Order`) VALUES ('Paraguay Standard Time', '(UTC-04:00) Asuncion', -4.00);
INSERT INTO TimeZoneInfos (Id, Display, `Order`) VALUES ('Romance Standard Time', '(UTC+01:00) Brussels, Copenhagen, Madrid, Paris', 1.00);
INSERT INTO TimeZoneInfos (Id, Display, `Order`) VALUES ('Russian Standard Time', '(UTC+04:00) Moscow, St. Petersburg, Volgograd', 4.00);
INSERT INTO TimeZoneInfos (Id, Display, `Order`) VALUES ('SA Eastern Standard Time', '(UTC-03:00) Cayenne, Fortaleza', -3.00);
INSERT INTO TimeZoneInfos (Id, Display, `Order`) VALUES ('SA Pacific Standard Time', '(UTC-05:00) Bogota, Lima, Quito', -5.00);
INSERT INTO TimeZoneInfos (Id, Display, `Order`) VALUES ('SA Western Standard Time', '(UTC-04:00) Georgetown, La Paz, Manaus, San Juan', -4.00);
INSERT INTO TimeZoneInfos (Id, Display, `Order`) VALUES ('Samoa Standard Time', '(UTC+13:00) Samoa', 13.00);
INSERT INTO TimeZoneInfos (Id, Display, `Order`) VALUES ('SE Asia Standard Time', '(UTC+07:00) Bangkok, Hanoi, Jakarta', 7.00);
INSERT INTO TimeZoneInfos (Id, Display, `Order`) VALUES ('Singapore Standard Time', '(UTC+08:00) Kuala Lumpur, Singapore', 8.00);
INSERT INTO TimeZoneInfos (Id, Display, `Order`) VALUES ('South Africa Standard Time', '(UTC+02:00) Harare, Pretoria', 2.00);
INSERT INTO TimeZoneInfos (Id, Display, `Order`) VALUES ('Sri Lanka Standard Time', '(UTC+05:30) Sri Jayawardenepura', 5.50);
INSERT INTO TimeZoneInfos (Id, Display, `Order`) VALUES ('Syria Standard Time', '(UTC+02:00) Damascus', 2.00);
INSERT INTO TimeZoneInfos (Id, Display, `Order`) VALUES ('Taipei Standard Time', '(UTC+08:00) Taipei', 8.00);
INSERT INTO TimeZoneInfos (Id, Display, `Order`) VALUES ('Tasmania Standard Time', '(UTC+10:00) Hobart', 10.00);
INSERT INTO TimeZoneInfos (Id, Display, `Order`) VALUES ('Tokyo Standard Time', '(UTC+09:00) Osaka, Sapporo, Tokyo', 9.00);
INSERT INTO TimeZoneInfos (Id, Display, `Order`) VALUES ('Tonga Standard Time', '(UTC+13:00) Nuku''alofa', 13.00);
INSERT INTO TimeZoneInfos (Id, Display, `Order`) VALUES ('Turkey Standard Time', '(UTC+02:00) Istanbul', 2.00);
INSERT INTO TimeZoneInfos (Id, Display, `Order`) VALUES ('Ulaanbaatar Standard Time', '(UTC+08:00) Ulaanbaatar', 8.00);
INSERT INTO TimeZoneInfos (Id, Display, `Order`) VALUES ('US Eastern Standard Time', '(UTC-05:00) Indiana (East)', -5.00);
INSERT INTO TimeZoneInfos (Id, Display, `Order`) VALUES ('US Mountain Standard Time', '(UTC-07:00) Arizona', -7.00);
INSERT INTO TimeZoneInfos (Id, Display, `Order`) VALUES ('UTC', '(UTC) Coordinated Universal Time', 0.00);
INSERT INTO TimeZoneInfos (Id, Display, `Order`) VALUES ('UTC+12', '(UTC+12:00) Coordinated Universal Time+12', 12.00);
INSERT INTO TimeZoneInfos (Id, Display, `Order`) VALUES ('UTC-02', '(UTC-02:00) Coordinated Universal Time-02', -2.00);
INSERT INTO TimeZoneInfos (Id, Display, `Order`) VALUES ('UTC-11', '(UTC-11:00) Coordinated Universal Time-11', -11.00);
INSERT INTO TimeZoneInfos (Id, Display, `Order`) VALUES ('Venezuela Standard Time', '(UTC-04:30) Caracas', -4.50);
INSERT INTO TimeZoneInfos (Id, Display, `Order`) VALUES ('Vladivostok Standard Time', '(UTC+11:00) Vladivostok', 11.00);
INSERT INTO TimeZoneInfos (Id, Display, `Order`) VALUES ('W. Australia Standard Time', '(UTC+08:00) Perth', 8.00);
INSERT INTO TimeZoneInfos (Id, Display, `Order`) VALUES ('W. Central Africa Standard Time', '(UTC+01:00) West Central Africa', 1.00);
INSERT INTO TimeZoneInfos (Id, Display, `Order`) VALUES ('W. Europe Standard Time', '(UTC+01:00) Amsterdam, Berlin, Bern, Rome, Stockholm, Vienna', 1.00);
INSERT INTO TimeZoneInfos (Id, Display, `Order`) VALUES ('West Asia Standard Time', '(UTC+05:00) Ashgabat, Tashkent', 5.00);
INSERT INTO TimeZoneInfos (Id, Display, `Order`) VALUES ('West Pacific Standard Time', '(UTC+10:00) Guam, Port Moresby', 10.00);
INSERT INTO TimeZoneInfos (Id, Display, `Order`) VALUES ('Yakutsk Standard Time', '(UTC+10:00) Yakutsk', 10.00);

-- Culture Infos
INSERT INTO CultureInfos (Id, DisplayName, `Order`) VALUES ('de-DE', 'German (Germany)', 11);
INSERT INTO CultureInfos (Id, DisplayName, `Order`) VALUES ('el-GR', 'Greek (Greece)', 12);
INSERT INTO CultureInfos (Id, DisplayName, `Order`) VALUES ('en-AU', 'English (Australia)', 1);
INSERT INTO CultureInfos (Id, DisplayName, `Order`) VALUES ('en-CA', 'English (Canada)', 2);
INSERT INTO CultureInfos (Id, DisplayName, `Order`) VALUES ('en-GB', 'English (Great Britain)', 3);
INSERT INTO CultureInfos (Id, DisplayName, `Order`) VALUES ('en-NZ', 'English (New Zealand)', 4);
INSERT INTO CultureInfos (Id, DisplayName, `Order`) VALUES ('en-US', 'English (United States)', 5);
INSERT INTO CultureInfos (Id, DisplayName, `Order`) VALUES ('en-ZA', 'English (South Africa)', 6);
INSERT INTO CultureInfos (Id, DisplayName, `Order`) VALUES ('es-ES', 'Spanish (Spain)', 7);
INSERT INTO CultureInfos (Id, DisplayName, `Order`) VALUES ('es-MX', 'Spanish (Mexico)', 8);
INSERT INTO CultureInfos (Id, DisplayName, `Order`) VALUES ('fr-CA', 'French (Canadian)', 10);
INSERT INTO CultureInfos (Id, DisplayName, `Order`) VALUES ('fr-FR', 'French (France)', 9);
INSERT INTO CultureInfos (Id, DisplayName, `Order`) VALUES ('ja-JP', 'Japanese (Japan)', 15);
INSERT INTO CultureInfos (Id, DisplayName, `Order`) VALUES ('ru-RU', 'Russian (Russian)', 16);
INSERT INTO CultureInfos (Id, DisplayName, `Order`) VALUES ('zh-C', 'Chinese (P.R.C)', 13);
INSERT INTO CultureInfos (Id, DisplayName, `Order`) VALUES ('zn-TW', 'Chinese (Taiwan)', 14);