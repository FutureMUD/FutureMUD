INSERT INTO Currencies (Name) VALUES ('Dollars');
set @currencyid = LAST_INSERT_ID();

-- FutureProg
INSERT INTO FutureProgs (FunctionName, FunctionComment, FunctionText, ReturnType, Category, Subcategory, `Public`) VALUES ('DollarCurrencyDollarOrGreater', 'Returns true if the amount of currency specified is greater than or equal to a dollar.', 'return @number >= 100', 4, 'Currency', 'Patterns', b'0');
set @progid = LAST_INSERT_ID();

INSERT INTO FutureProgs_Parameters (FutureProgId, ParameterIndex, ParameterType, ParameterName) VALUES (@progid, 0, 2, 'number');

-- Divisions

INSERT INTO CurrencyDivisions (Name, BaseUnitConversionRate, CurrencyId) VALUES ('dollar', 100, @currencyid);
set @dollardiv = LAST_INSERT_ID();
INSERT INTO CurrencyDivisions (Name, BaseUnitConversionRate, CurrencyId) VALUES ('cent', 1, @currencyid);
set @centdiv = LAST_INSERT_ID();

-- Division Abbreviations
INSERT INTO CurrencyDivisionAbbreviations (Pattern, CurrencyDivisionId) VALUES ('(\d+(?:\.\d+)*) (?:dollars|dollar|bucks|buck)', @dollardiv);
INSERT INTO CurrencyDivisionAbbreviations (Pattern, CurrencyDivisionId) VALUES ('\$(\d+(?:\.\d+)*)', @dollardiv);
INSERT INTO CurrencyDivisionAbbreviations (Pattern, CurrencyDivisionId) VALUES ('(\d+(?:\.\d+)*)(?: cents| cent|c| c)', @centdiv);

-- Patterns
INSERT INTO CurrencyDescriptionPatterns (`Type`, CurrencyId, FutureProgId, NegativePrefix, `Order`) VALUES (0, @currencyid, @progid, 'negative ', 1);
set @patternid = LAST_INSERT_ID();
INSERT INTO CurrencyDescriptionPatternElements (Pattern, `Order`, ShowIfZero, CurrencyDivisionId, CurrencyDescriptionPatternId, PluraliseWord, AlternatePattern, RoundingMode, SpecialValuesOverrideFormat) VALUES ('{0} buck', 1, b'0', @dollardiv, @patternid, 'buck', NULL, 0, b'0');
INSERT INTO CurrencyDescriptionPatternElements (Pattern, `Order`, ShowIfZero, CurrencyDivisionId, CurrencyDescriptionPatternId, PluraliseWord, AlternatePattern, RoundingMode, SpecialValuesOverrideFormat) VALUES ('{0}', 2, b'0', @centdiv, @patternid, '', NULL, 0, b'0');
set @specialid = LAST_INSERT_ID();
INSERT INTO CurrencyDescriptionPatternElementSpecialValues (`Value`, `Text`, CurrencyDescriptionPatternElementId) VALUES (1, 'oh-one', @specialid);
INSERT INTO CurrencyDescriptionPatternElementSpecialValues (`Value`, `Text`, CurrencyDescriptionPatternElementId) VALUES (2, 'oh-two', @specialid);
INSERT INTO CurrencyDescriptionPatternElementSpecialValues (`Value`, `Text`, CurrencyDescriptionPatternElementId) VALUES (3, 'oh-three', @specialid);
INSERT INTO CurrencyDescriptionPatternElementSpecialValues (`Value`, `Text`, CurrencyDescriptionPatternElementId) VALUES (4, 'oh-four', @specialid);
INSERT INTO CurrencyDescriptionPatternElementSpecialValues (`Value`, `Text`, CurrencyDescriptionPatternElementId) VALUES (5, 'oh-five', @specialid);
INSERT INTO CurrencyDescriptionPatternElementSpecialValues (`Value`, `Text`, CurrencyDescriptionPatternElementId) VALUES (6, 'oh-six', @specialid);
INSERT INTO CurrencyDescriptionPatternElementSpecialValues (`Value`, `Text`, CurrencyDescriptionPatternElementId) VALUES (7, 'oh-seven', @specialid);
INSERT INTO CurrencyDescriptionPatternElementSpecialValues (`Value`, `Text`, CurrencyDescriptionPatternElementId) VALUES (8, 'oh-eight', @specialid);
INSERT INTO CurrencyDescriptionPatternElementSpecialValues (`Value`, `Text`, CurrencyDescriptionPatternElementId) VALUES (9, 'oh-nine', @specialid);

INSERT INTO CurrencyDescriptionPatterns (`Type`, CurrencyId, FutureProgId, NegativePrefix, `Order`) VALUES (0, @currencyid, NULL, 'negative ', 2);
set @patternid = LAST_INSERT_ID();
INSERT INTO CurrencyDescriptionPatternElements (Pattern, `Order`, ShowIfZero, CurrencyDivisionId, CurrencyDescriptionPatternId, PluraliseWord, AlternatePattern, RoundingMode, SpecialValuesOverrideFormat) VALUES ('{0} cent', 1, b'0', @centdiv, @patternid, 'cent', NULL, 0, b'0');

INSERT INTO CurrencyDescriptionPatterns (`Type`, CurrencyId, FutureProgId, NegativePrefix, `Order`) VALUES (1, @currencyid, @progid, '-', 1);
set @patternid = LAST_INSERT_ID();
INSERT INTO CurrencyDescriptionPatternElements (Pattern, `Order`, ShowIfZero, CurrencyDivisionId, CurrencyDescriptionPatternId, PluraliseWord, AlternatePattern, RoundingMode, SpecialValuesOverrideFormat) VALUES ('${0}.', 1, b'0', @dollardiv, @patternid, '', NULL, 0, b'0');
INSERT INTO CurrencyDescriptionPatternElements (Pattern, `Order`, ShowIfZero, CurrencyDivisionId, CurrencyDescriptionPatternId, PluraliseWord, AlternatePattern, RoundingMode, SpecialValuesOverrideFormat) VALUES ('{0:00}c', 2, b'1', @centdiv, @patternid, '', NULL, 0, b'0');

INSERT INTO CurrencyDescriptionPatterns (`Type`, CurrencyId, FutureProgId, NegativePrefix, `Order`) VALUES (2, @currencyid, NULL, '-', 1);
set @patternid = LAST_INSERT_ID();
INSERT INTO CurrencyDescriptionPatternElements (Pattern, `Order`, ShowIfZero, CurrencyDivisionId, CurrencyDescriptionPatternId, PluraliseWord, AlternatePattern, RoundingMode, SpecialValuesOverrideFormat) VALUES ('${0:N2}', 1, b'1', @dollardiv, @patternid, '', NULL, 2, b'0');

INSERT INTO CurrencyDescriptionPatterns (`Type`, CurrencyId, FutureProgId, NegativePrefix, `Order`) VALUES (3, @currencyid, @progid, 'negative ', 1);
set @patternid = LAST_INSERT_ID();
INSERT INTO CurrencyDescriptionPatternElements (Pattern, `Order`, ShowIfZero, CurrencyDivisionId, CurrencyDescriptionPatternId, PluraliseWord, AlternatePattern, RoundingMode, SpecialValuesOverrideFormat) VALUES ('{0} dollar', 1, b'0', @dollardiv, @patternid, 'dollar', NULL, 0, b'0');
INSERT INTO CurrencyDescriptionPatternElements (Pattern, `Order`, ShowIfZero, CurrencyDivisionId, CurrencyDescriptionPatternId, PluraliseWord, AlternatePattern, RoundingMode, SpecialValuesOverrideFormat) VALUES ('and {0} cent', 2, b'0', @centdiv, @patternid, 'cent', NULL, 0, b'0');

INSERT INTO CurrencyDescriptionPatterns (`Type`, CurrencyId, FutureProgId, NegativePrefix, `Order`) VALUES (3, @currencyid, NULL, 'negative ', 2);
set @patternid = LAST_INSERT_ID();
INSERT INTO CurrencyDescriptionPatternElements (Pattern, `Order`, ShowIfZero, CurrencyDivisionId, CurrencyDescriptionPatternId, PluraliseWord, AlternatePattern, RoundingMode, SpecialValuesOverrideFormat) VALUES ('{0} cent', 1, b'1', @centdiv, @patternid, 'cent', NULL, 0, b'0');