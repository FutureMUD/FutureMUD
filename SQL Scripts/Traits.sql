-- Trait Decorators
INSERT INTO TraitDecorators (`Name`, `Type`, Contents) VALUES ('Current / Max', 'CurrentMax', '');
INSERT INTO TraitDecorators (`Name`, `Type`, Contents) VALUES ('Percentage', 'Percentage', '');
INSERT INTO TraitDecorators (`Name`, `Type`, Contents) VALUES ('Simple Numeric', 'SimpleNumeric', '');
INSERT INTO TraitDecorators (`Name`, `Type`, Contents) VALUES ('Simple Attribute', 'Range', '<ranges name="General Attribute Range" prefix="" suffix="" colour_capped="false"><range low="-25" high="0" text="Abysmal"/><range low="0" high="3" text="Terrible"/><range low="3" high="6" text="Bad"/><range low="6" high="9" text="Poor"/><range low="9" high="11" text="Average"/><range low="11" high="13" text="Good"/><range low="13" high="15" text="Great"/><range low="15" high="17" text="Excellent"/><range low="17" high="20" text="Super"/><range low="20" high="23" text="Epic"/><range low="23" high="25" text="Legendary"/></ranges>');
set @tdsimpleattr = LAST_INSERT_ID();

INSERT INTO TraitDecorators (`Name`, `Type`, Contents) VALUES ('Simple Skill', 'Range', '<ranges name="General Skill Range"  prefix="(" suffix=")" colour_capped="true"><range low="-100" high="0" text="Incompetent"/><range low="0" high="15" text="Familiar"/><range low="15" high="30" text="Competent"/><range low="30" high="45" text="Skilled"/><range low="45" high="60" text="Expert"/><range low="60" high="75" text="Masterful"/><range low="75" high="85" text="Epic"/><range low="85" high="95" text="Legendary"/><range low="95" high="500" text="Godly"/></ranges>');
set @tdsimpleskill = LAST_INSERT_ID();

INSERT INTO TraitDecorators (`Name`, `Type`, Contents) VALUES ('Strength Attribute', 'Range', '<ranges name="Strength Attribute Range" prefix="" suffix="" colour_capped="false"><range low="-25" high="0" text="Abysmal"/><range low="0" high="3" text="Terrible"/><range low="3" high="6" text="Bad"/><range low="6" high="9" text="Poor"/><range low="9" high="11" text="Average"/><range low="11" high="13" text="Good"/><range low="13" high="15" text="Great"/><range low="15" high="17" text="Excellent"/><range low="17" high="20" text="Super"/><range low="20" high="23" text="Epic"/><range low="23" high="25" text="Legendary"/><range low="25" high="30" text="Herculean"/></ranges>');
set @tdstrength = LAST_INSERT_ID();

INSERT INTO TraitDecorators (`Name`, `Type`, Contents) VALUES ('Perception Attribute', 'Range', '<ranges name="Perception Attribute Range" prefix="" suffix="" colour_capped="false"><range low="-25" high="0" text="Abysmal"/><range low="0" high="3" text="Terrible"/><range low="3" high="6" text="Bad"/><range low="6" high="9" text="Poor"/><range low="9" high="11" text="Average"/><range low="11" high="13" text="Good"/><range low="13" high="15" text="Great"/><range low="15" high="17" text="Excellent"/><range low="17" high="20" text="Super"/><range low="20" high="23" text="Epic"/><range low="23" high="25" text="Legendary"/><range low="25" high="30" text="Epicurean"/></ranges>');
set @tdperception = LAST_INSERT_ID();

INSERT INTO TraitDecorators (`Name`, `Type`, Contents) VALUES ('Intelligence Attribute', 'Range', '<ranges name="Intelligence Attribute Range" prefix="" suffix="" colour_capped="false"><range low="-25" high="0" text="Abysmal"/><range low="0" high="3" text="Terrible"/><range low="3" high="6" text="Bad"/><range low="6" high="9" text="Poor"/><range low="9" high="11" text="Average"/><range low="11" high="13" text="Good"/><range low="13" high="15" text="Great"/><range low="15" high="17" text="Excellent"/><range low="17" high="20" text="Super"/><range low="20" high="23" text="Epic"/><range low="23" high="25" text="Legendary"/><range low="25" high="30" text="Odyssian"/></ranges>');
set @tdintelligence = LAST_INSERT_ID();

INSERT INTO TraitDecorators (`Name`, `Type`, Contents) VALUES ('Constitution Attribute', 'Range', '<ranges name="Constitution Attribute Range" prefix="" suffix="" colour_capped="false"><range low="-25" high="0" text="Abysmal"/><range low="0" high="3" text="Terrible"/><range low="3" high="6" text="Bad"/><range low="6" high="9" text="Poor"/><range low="9" high="11" text="Average"/><range low="11" high="13" text="Good"/><range low="13" high="15" text="Great"/><range low="15" high="17" text="Excellent"/><range low="17" high="20" text="Super"/><range low="20" high="23" text="Epic"/><range low="23" high="25" text="Legendary"/><range low="25" high="30" text="Atlasian"/></ranges>');
set @tdconstitution = LAST_INSERT_ID();

INSERT INTO TraitDecorators (`Name`, `Type`, Contents) VALUES ('Dexterity Attribute', 'Range', '<ranges name="Dexterity Attribute Range" prefix="" suffix="" colour_capped="false"><range low="-25" high="0" text="Abysmal"/><range low="0" high="3" text="Terrible"/><range low="3" high="6" text="Bad"/><range low="6" high="9" text="Poor"/><range low="9" high="11" text="Average"/><range low="11" high="13" text="Good"/><range low="13" high="15" text="Great"/><range low="15" high="17" text="Excellent"/><range low="17" high="20" text="Super"/><range low="20" high="23" text="Epic"/><range low="23" high="25" text="Legendary"/><range low="25" high="30" text="Achillean"/></ranges>');
set @tddexterity = LAST_INSERT_ID();

INSERT INTO TraitDecorators (`Name`, `Type`, Contents) VALUES ('Language Skill Range', 'Range', '<ranges name="Language Skill Range"  prefix="(" suffix=")" colour_capped="true"><range low="-100" high="0" text="Incompetent"/><range low="0" high="15" text="Amateur"/><range low="15" high="30" text="Coherent"/><range low="30" high="50" text="Conversant"/><range low="50" high="70" text="Fluent"/><range low="70" high="95" text="Articulate"/><range low="95" high="500" text="Masterful"/></ranges>');
set @tdlanguage = LAST_INSERT_ID();

-- Skill Improvers
INSERT INTO Improvers (Id, Name, Definition, Type) VALUES (1, 'Classic Skills (General)', '<Definition Chance="0.2" Expression="10 - (variable/10)" ImproveOnFail="false" ImproveOnSuccess="true" DifficultyThresholdInterval="20" NoGainSecondsDiceExpression="1d500+2000"/>', 'classic');
set @skillsimprover = LAST_INSERT_ID();

INSERT INTO Improvers (Id, Name, Definition, Type) VALUES (4, 'Non-Improving', '<Definition/>', 'non-improving');
set @nonimprover = LAST_INSERT_ID();

INSERT INTO Improvers (Id, Name, Definition, Type) VALUES (5, 'Classic Skills (Language)', '<Definition Chance="0.2" Expression="10 - (variable/10)" ImproveOnFail="false" ImproveOnSuccess="true" DifficultyThresholdInterval="50" NoGainSecondsDiceExpression="1d500+2000"/>', 'classic');
set @languageimprover = LAST_INSERT_ID();

-- Trait Definitions
-- Attributes
INSERT INTO TraitDefinitions (`Name`, `Type`, DecoratorId, TraitGroup, DerivedType, ExpressionId, ImproverId, Hidden, ChargenBlurb, Alias, AvailabilityProgId) VALUES ('Strength', 1, @tdstrength, 'Physical', 0, null, @nonimprover, 0, 'Measures a character''s raw strength and lifting power.', 'str', null);
set @attrstr = LAST_INSERT_ID();
INSERT INTO TraitDefinitions (`Name`, `Type`, DecoratorId, TraitGroup, DerivedType, ExpressionId, ImproverId, Hidden, ChargenBlurb, Alias, AvailabilityProgId) VALUES ('Constitution', 1, @tdconstitution, 'Physical', 0, null, @nonimprover, 0, 'Measures a character''s ability to withstand punishment and abuse, as well as physical fitness.', 'con', null);
set @attrcon = LAST_INSERT_ID();
INSERT INTO TraitDefinitions (`Name`, `Type`, DecoratorId, TraitGroup, DerivedType, ExpressionId, ImproverId, Hidden, ChargenBlurb, Alias, AvailabilityProgId) VALUES ('Intelligence', 1, @tdintelligence, 'Mental', 0, null, @nonimprover, 0, 'Measures a character''s ability for learning and reason.', 'int', null);
set @attrint = LAST_INSERT_ID();
INSERT INTO TraitDefinitions (`Name`, `Type`, DecoratorId, TraitGroup, DerivedType, ExpressionId, ImproverId, Hidden, ChargenBlurb, Alias, AvailabilityProgId) VALUES ('Dexterity', 1, @tddexterity, 'Physical', 0, null, @nonimprover, 0, 'Measures a character''s hand-eye coordination and flexibility.', 'dex', null);
set @attrdex = LAST_INSERT_ID();
INSERT INTO TraitDefinitions (`Name`, `Type`, DecoratorId, TraitGroup, DerivedType, ExpressionId, ImproverId, Hidden, ChargenBlurb, Alias, AvailabilityProgId) VALUES ('Willpower', 1, @tdsimpleattr, 'Mental', 0, null, @nonimprover, 0, 'Measures a character''s ability to override their primal, instinctual responses.', 'wil', null);
set @attrwil = LAST_INSERT_ID();
INSERT INTO TraitDefinitions (`Name`, `Type`, DecoratorId, TraitGroup, DerivedType, ExpressionId, ImproverId, Hidden, ChargenBlurb, Alias, AvailabilityProgId) VALUES ('Perception', 1, @tdperception, 'Mental', 0, null, @nonimprover, 0, 'Measures a character''s ability to notice things in the world around them.', 'per', null);
set @attrper = LAST_INSERT_ID();

-- Example Combat Skills
-- Swords
INSERT INTO TraitExpression (Expression) VALUES ('(3 * dex) + (1 * int) + (1 * wil)');
set @traitexpression = LAST_INSERT_ID();
INSERT INTO TraitExpressionParameters (TraitExpressionId, TraitDefinitionId, Parameter) VALUES (@traitexpression, @attrdex, 'dex');
INSERT INTO TraitExpressionParameters (TraitExpressionId, TraitDefinitionId, Parameter) VALUES (@traitexpression, @attrint, 'int');
INSERT INTO TraitExpressionParameters (TraitExpressionId, TraitDefinitionId, Parameter) VALUES (@traitexpression, @attrwil, 'wil');
INSERT INTO TraitDefinitions (`Name`, `Type`, DecoratorId, TraitGroup, DerivedType, ExpressionId, ImproverId, Hidden, ChargenBlurb, Alias, AvailabilityProgId) VALUES ('Swords', 0, @tdsimpleskill, 'Combat', 0, @traitexpression, @skillsimprover, 0, '', '', @alwaystrueprog);

-- Clubs
INSERT INTO TraitExpression (Expression) VALUES ('(3 * dex) + (1 * int) + (1 * wil)');
set @traitexpression = LAST_INSERT_ID();
INSERT INTO TraitExpressionParameters (TraitExpressionId, TraitDefinitionId, Parameter) VALUES (@traitexpression, @attrdex, 'dex');
INSERT INTO TraitExpressionParameters (TraitExpressionId, TraitDefinitionId, Parameter) VALUES (@traitexpression, @attrint, 'int');
INSERT INTO TraitExpressionParameters (TraitExpressionId, TraitDefinitionId, Parameter) VALUES (@traitexpression, @attrwil, 'wil');
INSERT INTO TraitDefinitions (`Name`, `Type`, DecoratorId, TraitGroup, DerivedType, ExpressionId, ImproverId, Hidden, ChargenBlurb, Alias, AvailabilityProgId) VALUES ('Clubs', 0, @tdsimpleskill, 'Combat', 0, @traitexpression, @skillsimprover, 0, '', '', @alwaystrueprog);

-- Axes
INSERT INTO TraitExpression (Expression) VALUES ('(3 * dex) + (1 * int) + (1 * wil)');
set @traitexpression = LAST_INSERT_ID();
INSERT INTO TraitExpressionParameters (TraitExpressionId, TraitDefinitionId, Parameter) VALUES (@traitexpression, @attrdex, 'dex');
INSERT INTO TraitExpressionParameters (TraitExpressionId, TraitDefinitionId, Parameter) VALUES (@traitexpression, @attrint, 'int');
INSERT INTO TraitExpressionParameters (TraitExpressionId, TraitDefinitionId, Parameter) VALUES (@traitexpression, @attrwil, 'wil');
INSERT INTO TraitDefinitions (`Name`, `Type`, DecoratorId, TraitGroup, DerivedType, ExpressionId, ImproverId, Hidden, ChargenBlurb, Alias, AvailabilityProgId) VALUES ('Axes', 0, @tdsimpleskill, 'Combat', 0, @traitexpression, @skillsimprover, 0, '', '', @alwaystrueprog);

-- Polearms
INSERT INTO TraitExpression (Expression) VALUES ('(3 * dex) + (1 * int) + (1 * wil)');
set @traitexpression = LAST_INSERT_ID();
INSERT INTO TraitExpressionParameters (TraitExpressionId, TraitDefinitionId, Parameter) VALUES (@traitexpression, @attrdex, 'dex');
INSERT INTO TraitExpressionParameters (TraitExpressionId, TraitDefinitionId, Parameter) VALUES (@traitexpression, @attrint, 'int');
INSERT INTO TraitExpressionParameters (TraitExpressionId, TraitDefinitionId, Parameter) VALUES (@traitexpression, @attrwil, 'wil');
INSERT INTO TraitDefinitions (`Name`, `Type`, DecoratorId, TraitGroup, DerivedType, ExpressionId, ImproverId, Hidden, ChargenBlurb, Alias, AvailabilityProgId) VALUES ('Polearms', 0, @tdsimpleskill, 'Combat', 0, @traitexpression, @skillsimprover, 0, '', '', @alwaystrueprog);

-- Daggers
INSERT INTO TraitExpression (Expression) VALUES ('(3 * dex) + (1 * int) + (1 * wil)');
set @traitexpression = LAST_INSERT_ID();
INSERT INTO TraitExpressionParameters (TraitExpressionId, TraitDefinitionId, Parameter) VALUES (@traitexpression, @attrdex, 'dex');
INSERT INTO TraitExpressionParameters (TraitExpressionId, TraitDefinitionId, Parameter) VALUES (@traitexpression, @attrint, 'int');
INSERT INTO TraitExpressionParameters (TraitExpressionId, TraitDefinitionId, Parameter) VALUES (@traitexpression, @attrwil, 'wil');
INSERT INTO TraitDefinitions (`Name`, `Type`, DecoratorId, TraitGroup, DerivedType, ExpressionId, ImproverId, Hidden, ChargenBlurb, Alias, AvailabilityProgId) VALUES ('Daggers', 0, @tdsimpleskill, 'Combat', 0, @traitexpression, @skillsimprover, 0, '', '', @alwaystrueprog);

-- Brawling
INSERT INTO TraitExpression (Expression) VALUES ('(3 * dex) + (1 * int) + (1 * wil)');
set @traitexpression = LAST_INSERT_ID();
INSERT INTO TraitExpressionParameters (TraitExpressionId, TraitDefinitionId, Parameter) VALUES (@traitexpression, @attrdex, 'dex');
INSERT INTO TraitExpressionParameters (TraitExpressionId, TraitDefinitionId, Parameter) VALUES (@traitexpression, @attrint, 'int');
INSERT INTO TraitExpressionParameters (TraitExpressionId, TraitDefinitionId, Parameter) VALUES (@traitexpression, @attrwil, 'wil');
INSERT INTO TraitDefinitions (`Name`, `Type`, DecoratorId, TraitGroup, DerivedType, ExpressionId, ImproverId, Hidden, ChargenBlurb, Alias, AvailabilityProgId) VALUES ('Brawling', 0, @tdsimpleskill, 'Combat', 0, @traitexpression, @skillsimprover, 0, '', '', @alwaystrueprog);

-- Bows
INSERT INTO TraitExpression (Expression) VALUES ('(3 * dex) + (1 * int) + (1 * wil)');
set @traitexpression = LAST_INSERT_ID();
INSERT INTO TraitExpressionParameters (TraitExpressionId, TraitDefinitionId, Parameter) VALUES (@traitexpression, @attrdex, 'dex');
INSERT INTO TraitExpressionParameters (TraitExpressionId, TraitDefinitionId, Parameter) VALUES (@traitexpression, @attrint, 'int');
INSERT INTO TraitExpressionParameters (TraitExpressionId, TraitDefinitionId, Parameter) VALUES (@traitexpression, @attrwil, 'wil');
INSERT INTO TraitDefinitions (`Name`, `Type`, DecoratorId, TraitGroup, DerivedType, ExpressionId, ImproverId, Hidden, ChargenBlurb, Alias, AvailabilityProgId) VALUES ('Bows', 0, @tdsimpleskill, 'Combat', 0, @traitexpression, @skillsimprover, 0, '', '', @alwaystrueprog);

-- Core Skills
-- Hide
INSERT INTO TraitExpression (Expression) VALUES ('(2 * dex) + (3 * int)');
set @traitexpression = LAST_INSERT_ID();
INSERT INTO TraitExpressionParameters (TraitExpressionId, TraitDefinitionId, Parameter) VALUES (@traitexpression, @attrdex, 'dex');
INSERT INTO TraitExpressionParameters (TraitExpressionId, TraitDefinitionId, Parameter) VALUES (@traitexpression, @attrint, 'int');
INSERT INTO TraitDefinitions (`Name`, `Type`, DecoratorId, TraitGroup, DerivedType, ExpressionId, ImproverId, Hidden, ChargenBlurb, Alias, AvailabilityProgId) VALUES ('Hide', 0, @tdsimpleskill, 'Stealth', 0, @traitexpression, @skillsimprover, 0, '', '', @alwaystrueprog);

-- Sneak
-- Note: Sneak only selectable if hide selected first
INSERT INTO FutureProgs (FunctionName, FunctionComment, FunctionText, ReturnType, Category, Subcategory, `Public`) VALUES ('ChargenSneakSkill', 'Specifies whether or not the sneak skill can be selected', 'var hide as trait
hide = ToTrait("Hide")
return @chargen.Skills.Any(x, @x == @hide)', 4, 'Chargen', 'Skills', b'0');
set @sneakprog = LAST_INSERT_ID();

INSERT INTO FutureProgs_Parameters (FutureProgId, ParameterIndex, ParameterType, ParameterName) values (@sneakprog, 0, 8192, 'chargen');

INSERT INTO TraitExpression (Expression) VALUES ('(2 * dex) + (1 * int) + (2 * per)');
set @traitexpression = LAST_INSERT_ID();
INSERT INTO TraitExpressionParameters (TraitExpressionId, TraitDefinitionId, Parameter) VALUES (@traitexpression, @attrdex, 'dex');
INSERT INTO TraitExpressionParameters (TraitExpressionId, TraitDefinitionId, Parameter) VALUES (@traitexpression, @attrint, 'int');
INSERT INTO TraitExpressionParameters (TraitExpressionId, TraitDefinitionId, Parameter) VALUES (@traitexpression, @attrper, 'per');
INSERT INTO TraitDefinitions (`Name`, `Type`, DecoratorId, TraitGroup, DerivedType, ExpressionId, ImproverId, Hidden, ChargenBlurb, Alias, AvailabilityProgId) VALUES ('Sneak', 0, @tdsimpleskill, 'Stealth', 0, @traitexpression, @skillsimprover, 0, '', '', @sneakprog);

-- Spot
INSERT INTO TraitExpression (Expression) VALUES ('(4 * per) + (1 * int)');
set @traitexpression = LAST_INSERT_ID();
INSERT INTO TraitExpressionParameters (TraitExpressionId, TraitDefinitionId, Parameter) VALUES (@traitexpression, @attrper, 'per');
INSERT INTO TraitExpressionParameters (TraitExpressionId, TraitDefinitionId, Parameter) VALUES (@traitexpression, @attrint, 'int');
INSERT INTO TraitDefinitions (`Name`, `Type`, DecoratorId, TraitGroup, DerivedType, ExpressionId, ImproverId, Hidden, ChargenBlurb, Alias, AvailabilityProgId) VALUES ('Spot', 0, @tdsimpleskill, 'Perception', 0, @traitexpression, @skillsimprover, 0, '', '', @alwaystrueprog);

-- Listen
INSERT INTO TraitExpression (Expression) VALUES ('(4 * per) + (1 * int)');
set @traitexpression = LAST_INSERT_ID();
INSERT INTO TraitExpressionParameters (TraitExpressionId, TraitDefinitionId, Parameter) VALUES (@traitexpression, @attrper, 'per');
INSERT INTO TraitExpressionParameters (TraitExpressionId, TraitDefinitionId, Parameter) VALUES (@traitexpression, @attrint, 'int');
INSERT INTO TraitDefinitions (`Name`, `Type`, DecoratorId, TraitGroup, DerivedType, ExpressionId, ImproverId, Hidden, ChargenBlurb, Alias, AvailabilityProgId) VALUES ('Listen', 0, @tdsimpleskill, 'Perception', 0, @traitexpression, @skillsimprover, 0, '', '', @alwaystrueprog);

-- Climb
INSERT INTO TraitExpression (Expression) VALUES ('(3 * dex) + (1 * str) + (1 * con)');
set @traitexpression = LAST_INSERT_ID();
INSERT INTO TraitExpressionParameters (TraitExpressionId, TraitDefinitionId, Parameter) VALUES (@traitexpression, @attrdex, 'dex');
INSERT INTO TraitExpressionParameters (TraitExpressionId, TraitDefinitionId, Parameter) VALUES (@traitexpression, @attrstr, 'str');
INSERT INTO TraitExpressionParameters (TraitExpressionId, TraitDefinitionId, Parameter) VALUES (@traitexpression, @attrcon, 'con');
INSERT INTO TraitDefinitions (`Name`, `Type`, DecoratorId, TraitGroup, DerivedType, ExpressionId, ImproverId, Hidden, ChargenBlurb, Alias, AvailabilityProgId) VALUES ('Climb', 0, @tdsimpleskill, 'Athletic', 0, @traitexpression, @skillsimprover, 0, '', '', @alwaysfalseprog);

-- Swim
INSERT INTO TraitExpression (Expression) VALUES ('(2 * str) + (3 * con)');
set @traitexpression = LAST_INSERT_ID();
INSERT INTO TraitExpressionParameters (TraitExpressionId, TraitDefinitionId, Parameter) VALUES (@traitexpression, @attrstr, 'str');
INSERT INTO TraitExpressionParameters (TraitExpressionId, TraitDefinitionId, Parameter) VALUES (@traitexpression, @attrcon, 'con');
INSERT INTO TraitDefinitions (`Name`, `Type`, DecoratorId, TraitGroup, DerivedType, ExpressionId, ImproverId, Hidden, ChargenBlurb, Alias, AvailabilityProgId) VALUES ('Swim', 0, @tdsimpleskill, 'Athletic', 0, @traitexpression, @skillsimprover, 0, '', '', @alwaystrueprog);

-- Forage
INSERT INTO TraitExpression (Expression) VALUES ('(2 * per) + (3 * int)');
set @traitexpression = LAST_INSERT_ID();
INSERT INTO TraitExpressionParameters (TraitExpressionId, TraitDefinitionId, Parameter) VALUES (@traitexpression, @attrper, 'per');
INSERT INTO TraitExpressionParameters (TraitExpressionId, TraitDefinitionId, Parameter) VALUES (@traitexpression, @attrint, 'int');
INSERT INTO TraitDefinitions (`Name`, `Type`, DecoratorId, TraitGroup, DerivedType, ExpressionId, ImproverId, Hidden, ChargenBlurb, Alias, AvailabilityProgId) VALUES ('Forage', 0, @tdsimpleskill, 'Trade', 0, @traitexpression, @skillsimprover, 0, '', '', @alwaystrueprog);

-- Larceny
INSERT INTO TraitExpression (Expression) VALUES ('(2 * dex) + (3 * int)');
set @traitexpression = LAST_INSERT_ID();
INSERT INTO TraitExpressionParameters (TraitExpressionId, TraitDefinitionId, Parameter) VALUES (@traitexpression, @attrdex, 'dex');
INSERT INTO TraitExpressionParameters (TraitExpressionId, TraitDefinitionId, Parameter) VALUES (@traitexpression, @attrint, 'int');
INSERT INTO TraitDefinitions (`Name`, `Type`, DecoratorId, TraitGroup, DerivedType, ExpressionId, ImproverId, Hidden, ChargenBlurb, Alias, AvailabilityProgId) VALUES ('Larceny', 0, @tdsimpleskill, 'Stealth', 0, @traitexpression, @skillsimprover, 0, '', '', @alwaystrueprog);