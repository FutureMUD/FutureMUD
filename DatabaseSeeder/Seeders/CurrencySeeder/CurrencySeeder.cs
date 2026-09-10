using MudSharp.Database;
using MudSharp.Economy.Currency;
using MudSharp.Framework;
using MudSharp.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DatabaseSeeder.Seeders;

public class CurrencySeeder : IDatabaseSeeder
{
    public bool SafeToRunMoreThanOnce => true;

    public IEnumerable<(string Id, string Question,
        Func<FuturemudDatabaseContext, IReadOnlyDictionary<string, string>, bool> Filter,
        Func<string, FuturemudDatabaseContext, (bool Success, string error)> Validator)> SeederQuestions =>
        new List<(string Id, string Question,
            Func<FuturemudDatabaseContext, IReadOnlyDictionary<string, string>, bool> Filter,
            Func<string, FuturemudDatabaseContext, (bool Success, string error)> Validator)>
        {
            ("currency", @"You can choose from the following pre-made currency options:

#BDollars#F - a system with dollars and cents. Uses US coins and notes but you can change these as you desire.
#BPounds#F - a system with farthings, pennies, shillings and pounds. Uses UK coins and notes but you can change these.
#BFantasy#F - set up a currency with copper, silver and gold pieces each worth 10 of the former.
#BRoman#F - set up a roman currency (denarii, sestertii, etc) equivalent to the republic
#BBits#F - a fantasy system with a single coin and denomination (bits), similar to Armageddon and superficially similar to old SOI
#BGondor#F - a system using farthings, pennies and crowns with coins including Castar and Tharni
#BSOI#F - the coinage system from Shadows of Isildur (Gondor, Mordor, Tur Edendor, Harad, Northlands included)

#CNote#F: It's perfectly acceptable to run this seeder option multiple times to add different currencies

Please make your choice: ",
                (context, questions) => true,
                (answer, context) =>
                {
                    switch (answer.ToLowerInvariant())
                    {
                        case "dollars":
                        case "bits":
                        case "roman":
                        case "fantasy":
                        case "pounds":
                        case "gondor":
                            case "soi":
                            return (true, string.Empty);
                    }

                    return (false, "Invalid answer");
                })
        };

    public string SeedData(FuturemudDatabaseContext context, IReadOnlyDictionary<string, string> questionAnswers)
    {
        context.Database.BeginTransaction();
        List<string> errors = new();
        switch (questionAnswers["currency"].ToLowerInvariant())
        {
            case "dollars":
                SeedDollars(context, errors);
                break;
            case "bits":
                SeedBits(context, errors);
                break;
            case "roman":
                SeedRoman(context, errors);
                break;
            case "fantasy":
                SeedFantasy(context, errors);
                break;
            case "pounds":
                SeedPounds(context, errors);
                break;
            case "gondor":
                SeedGondorian(context, errors);
                break;
            case "soi":
                SeedShadowsOfIsildur(context, errors);
                break;
        }

        context.Database.CommitTransaction();

        if (errors.Count == 0)
        {
            return "The operation completed successfully.";
        }

        return
            $"The operation completed with the following errors or warnings:\n\n{errors.ListToCommaSeparatedValues("\n")}";
    }

    public ShouldSeedResult ShouldSeedData(FuturemudDatabaseContext context)
    {
        // Only requires the core data seeder
        if (!context.Accounts.Any())
        {
            return ShouldSeedResult.PrerequisitesNotMet;
        }

        if (context.Currencies.Any())
        {
            return ShouldSeedResult.ExtraPackagesAvailable;
        }

        return ShouldSeedResult.ReadyToInstall;
    }

    public int SortOrder => 20;
    public string Name => "Currency";
    public string Tagline => "Set up a currency (or currencies) for your game";

	public string FullDescription =>
		"This package sets up everything you need to get a currency in game. It is intended to be additive, so reruns can install more stock currency packages. This is required for some of the clan templates.";

	private static FutureProg EnsureNumericLessThanProg(FuturemudDatabaseContext context, string functionName, string thresholdText)
	{
		var prog = context.FutureProgs.FirstOrDefault(x => x.FunctionName == functionName);
		if (prog is not null)
		{
			return prog;
		}

		prog = new FutureProg
		{
			FunctionName = functionName,
			AcceptsAnyParameters = false,
			ReturnType = 4,
			Category = "Core",
			Subcategory = "Universal",
			Public = true,
			FunctionComment = $"Accepts a number and returns true if it is less than {thresholdText}.",
			FunctionText = $"return @number < {thresholdText}",
			StaticType = 0
		};
		prog.FutureProgsParameters.Add(new FutureProgsParameter
		{
			FutureProg = prog,
			ParameterIndex = 0,
			ParameterName = "number",
			ParameterType = 2
		});
		context.FutureProgs.Add(prog);
		context.SaveChanges();
		return prog;
	}

	private static void AddSpecialValue(CurrencyDescriptionPatternElement element, decimal value, string text)
	{
		element.CurrencyDescriptionPatternElementSpecialValues.Add(new CurrencyDescriptionPatternElementSpecialValues
		{
			CurrencyDescriptionPatternElement = element,
			Value = value,
			Text = text
		});
	}

	private static void AddSterlingFractionalPenceSpecialValues(CurrencyDescriptionPatternElement element, string suffix)
	{
		foreach (var (fraction, glyph) in new[]
		         {
			         (0.25M, "¼"),
			         (0.5M, "½"),
			         (0.75M, "¾")
		         })
		{
			for (var wholePence = 0; wholePence < 12; wholePence++)
			{
				var value = wholePence + fraction;
				var text = wholePence == 0
					? $"{glyph}{suffix}"
					: $"{wholePence}{glyph}{suffix}";
				AddSpecialValue(element, value, text);
			}
		}
	}

	private void SeedDollars(FuturemudDatabaseContext context, ICollection<string> errors)
	{
        Currency currency = new()
        {
            Name = "Dollars"
        };
        context.Currencies.Add(currency);
        context.SaveChanges();

        if (!context.FutureProgs.Any(x => x.FunctionName == "IsLessThanOneHundred"))
        {
            FutureProg prog = new()
            {
                FunctionName = "IsLessThanOneHundred",
                AcceptsAnyParameters = false,
                ReturnType = 4,
                Category = "Core",
                Subcategory = "Universal",
                Public = true,
                FunctionComment = "Accepts a number and returns true if it is less than 100.",
                FunctionText = "return @number < 100",
                StaticType = 0
            };
            prog.FutureProgsParameters.Add(new FutureProgsParameter
            { FutureProg = prog, ParameterIndex = 0, ParameterName = "number", ParameterType = 2 });
            context.FutureProgs.Add(prog);
            context.SaveChanges();
        }

        CurrencyDivision division = new()
        {
            Currency = currency,
            Name = "cent",
            BaseUnitConversionRate = 1.0M
        };
        context.CurrencyDivisions.Add(division);
        CurrencyDivision cent = division;
        division.CurrencyDivisionAbbreviations.Add(new CurrencyDivisionAbbreviation
        { CurrencyDivision = division, Pattern = @"(-?\d+(?:\.\d+)*)(?:\s*(?:c|cent|cents))$" });
        context.SaveChanges();

        division = new CurrencyDivision
        {
            Currency = currency,
            Name = "dollar",
            BaseUnitConversionRate = 100.0M
        };
        context.CurrencyDivisions.Add(division);
        CurrencyDivision dollar = division;
        division.CurrencyDivisionAbbreviations.Add(new CurrencyDivisionAbbreviation
        { CurrencyDivision = division, Pattern = @"(-?\d+(?:\.\d+)*)(?:\s*(?:c|cent|cents))$" });
        context.SaveChanges();

        foreach (CurrencyDescriptionPatternType type in Enum.GetValues(typeof(CurrencyDescriptionPatternType))
                     .OfType<CurrencyDescriptionPatternType>())
        {
            string prefix;
            switch (type)
            {
                case CurrencyDescriptionPatternType.Casual:
                case CurrencyDescriptionPatternType.Wordy:
                    prefix = "negative ";
                    break;
                case CurrencyDescriptionPatternType.Long:
                case CurrencyDescriptionPatternType.Short:
                case CurrencyDescriptionPatternType.ShortDecimal:
                    prefix = "-";
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            CurrencyDescriptionPattern pattern = new()
            {
                Currency = currency,
                Type = (int)type,
                NegativePrefix = prefix,
                Order = 1,
                FutureProg = context.FutureProgs.First(x => x.FunctionName == "IsLessThanOneHundred")
            };
            context.CurrencyDescriptionPatterns.Add(pattern);

            switch (type)
            {
                case CurrencyDescriptionPatternType.Casual:
                    pattern.CurrencyDescriptionPatternElements.Add(new CurrencyDescriptionPatternElement
                    {
                        CurrencyDescriptionPattern = pattern,
                        Pattern = "{0} cent",
                        Order = 1,
                        ShowIfZero = true,
                        CurrencyDivision = cent,
                        PluraliseWord = "cent",
                        RoundingMode = (int)RoundingMode.Truncate,
                        SpecialValuesOverrideFormat = false
                    });
                    break;
                case CurrencyDescriptionPatternType.Wordy:
                    pattern.CurrencyDescriptionPatternElements.Add(new CurrencyDescriptionPatternElement
                    {
                        CurrencyDescriptionPattern = pattern,
                        Pattern = "{0} cent",
                        Order = 1,
                        ShowIfZero = true,
                        CurrencyDivision = cent,
                        PluraliseWord = "cent",
                        RoundingMode = (int)RoundingMode.Truncate,
                        SpecialValuesOverrideFormat = false
                    });
                    break;
                case CurrencyDescriptionPatternType.Long:
                    pattern.CurrencyDescriptionPatternElements.Add(new CurrencyDescriptionPatternElement
                    {
                        CurrencyDescriptionPattern = pattern,
                        Pattern = "{0} cent",
                        Order = 1,
                        ShowIfZero = true,
                        CurrencyDivision = cent,
                        PluraliseWord = "cent",
                        RoundingMode = (int)RoundingMode.Truncate,
                        SpecialValuesOverrideFormat = false
                    });
                    break;
                case CurrencyDescriptionPatternType.Short:
                    pattern.CurrencyDescriptionPatternElements.Add(new CurrencyDescriptionPatternElement
                    {
                        CurrencyDescriptionPattern = pattern,
                        Pattern = "{0:#0}c",
                        Order = 1,
                        ShowIfZero = true,
                        CurrencyDivision = cent,
                        PluraliseWord = "cent",
                        RoundingMode = (int)RoundingMode.Truncate,
                        SpecialValuesOverrideFormat = false
                    });
                    break;
                case CurrencyDescriptionPatternType.ShortDecimal:
                    pattern.CurrencyDescriptionPatternElements.Add(new CurrencyDescriptionPatternElement
                    {
                        CurrencyDescriptionPattern = pattern,
                        Pattern = "${0:N2}",
                        Order = 1,
                        ShowIfZero = true,
                        CurrencyDivision = cent,
                        PluraliseWord = "",
                        RoundingMode = (int)RoundingMode.NoRounding,
                        SpecialValuesOverrideFormat = false
                    });
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            pattern = new CurrencyDescriptionPattern
            {
                Currency = currency,
                Type = (int)type,
                NegativePrefix = prefix,
                Order = 2
            };
            context.CurrencyDescriptionPatterns.Add(pattern);

            switch (type)
            {
                case CurrencyDescriptionPatternType.Casual:
                    pattern.CurrencyDescriptionPatternElements.Add(new CurrencyDescriptionPatternElement
                    {
                        CurrencyDescriptionPattern = pattern,
                        Pattern = "{0} buck",
                        Order = 1,
                        ShowIfZero = true,
                        CurrencyDivision = dollar,
                        PluraliseWord = "buck",
                        RoundingMode = (int)RoundingMode.Truncate,
                        SpecialValuesOverrideFormat = false
                    });
                    pattern.CurrencyDescriptionPatternElements.Add(new CurrencyDescriptionPatternElement
                    {
                        CurrencyDescriptionPattern = pattern,
                        Pattern = " {0}",
                        Order = 2,
                        ShowIfZero = false,
                        CurrencyDivision = cent,
                        PluraliseWord = "",
                        RoundingMode = (int)RoundingMode.Truncate,
                        SpecialValuesOverrideFormat = false
                    });
                    break;
                case CurrencyDescriptionPatternType.Wordy:
                    pattern.CurrencyDescriptionPatternElements.Add(new CurrencyDescriptionPatternElement
                    {
                        CurrencyDescriptionPattern = pattern,
                        Pattern = "{0} dollar",
                        Order = 1,
                        ShowIfZero = true,
                        CurrencyDivision = dollar,
                        PluraliseWord = "dollar",
                        RoundingMode = (int)RoundingMode.Truncate,
                        SpecialValuesOverrideFormat = false
                    });
                    pattern.CurrencyDescriptionPatternElements.Add(new CurrencyDescriptionPatternElement
                    {
                        CurrencyDescriptionPattern = pattern,
                        Pattern = " and {0} cent",
                        Order = 2,
                        ShowIfZero = false,
                        CurrencyDivision = cent,
                        PluraliseWord = "cent",
                        RoundingMode = (int)RoundingMode.Truncate,
                        SpecialValuesOverrideFormat = false
                    });
                    break;
                case CurrencyDescriptionPatternType.Long:
                    pattern.CurrencyDescriptionPatternElements.Add(new CurrencyDescriptionPatternElement
                    {
                        CurrencyDescriptionPattern = pattern,
                        Pattern = "{0} dollar",
                        Order = 1,
                        ShowIfZero = true,
                        CurrencyDivision = dollar,
                        PluraliseWord = "dollar",
                        RoundingMode = (int)RoundingMode.Truncate,
                        SpecialValuesOverrideFormat = false
                    });
                    pattern.CurrencyDescriptionPatternElements.Add(new CurrencyDescriptionPatternElement
                    {
                        CurrencyDescriptionPattern = pattern,
                        Pattern = " and {0} cent",
                        Order = 2,
                        ShowIfZero = false,
                        CurrencyDivision = cent,
                        PluraliseWord = "cent",
                        RoundingMode = (int)RoundingMode.Truncate,
                        SpecialValuesOverrideFormat = false
                    });
                    break;
                case CurrencyDescriptionPatternType.Short:
                    pattern.CurrencyDescriptionPatternElements.Add(new CurrencyDescriptionPatternElement
                    {
                        CurrencyDescriptionPattern = pattern,
                        Pattern = "${0}.",
                        Order = 1,
                        ShowIfZero = true,
                        CurrencyDivision = dollar,
                        PluraliseWord = "",
                        RoundingMode = (int)RoundingMode.Truncate,
                        SpecialValuesOverrideFormat = false
                    });
                    pattern.CurrencyDescriptionPatternElements.Add(new CurrencyDescriptionPatternElement
                    {
                        CurrencyDescriptionPattern = pattern,
                        Pattern = "{0:00}c",
                        Order = 2,
                        ShowIfZero = true,
                        CurrencyDivision = cent,
                        PluraliseWord = "cent",
                        RoundingMode = (int)RoundingMode.Truncate,
                        SpecialValuesOverrideFormat = false
                    });
                    break;
                case CurrencyDescriptionPatternType.ShortDecimal:
                    pattern.CurrencyDescriptionPatternElements.Add(new CurrencyDescriptionPatternElement
                    {
                        CurrencyDescriptionPattern = pattern,
                        Pattern = "${0:N2}",
                        Order = 1,
                        ShowIfZero = true,
                        CurrencyDivision = cent,
                        PluraliseWord = "",
                        RoundingMode = (int)RoundingMode.NoRounding,
                        SpecialValuesOverrideFormat = false
                    });
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        context.SaveChanges();

        Coin coin = new()
        {
            Name = "penny",
            ShortDescription = "a penny",
            FullDescription = "This is a small coin made of nickel-brass called a penny.",
            Value = 1.0M,
            Currency = currency,
            Weight = 2.0,
            GeneralForm = "coin",
            PluralWord = "penny"
        };
        context.Coins.Add(coin);
        context.SaveChanges();

        coin = new Coin
        {
            Name = "nickel",
            ShortDescription = "a nickel",
            FullDescription = "This is a small coin made of nickel-brass called a nickel.",
            Value = 5.0M,
            Currency = currency,
            Weight = 5.0,
            GeneralForm = "coin",
            PluralWord = "nickel"
        };
        context.Coins.Add(coin);
        context.SaveChanges();

        coin = new Coin
        {
            Name = "dime",
            ShortDescription = "a dime",
            FullDescription = "This is a small coin made of nickel-brass called a dime.",
            Value = 10.0M,
            Currency = currency,
            Weight = 2.0,
            GeneralForm = "coin",
            PluralWord = "dime"
        };
        context.Coins.Add(coin);
        context.SaveChanges();

        coin = new Coin
        {
            Name = "quarter",
            ShortDescription = "a quarter",
            FullDescription = "This is a small coin made called a quarter.",
            Value = 25.0M,
            Currency = currency,
            Weight = 6.0,
            GeneralForm = "coin",
            PluralWord = "quarter"
        };
        context.Coins.Add(coin);
        context.SaveChanges();

        coin = new Coin
        {
            Name = "dollar bill",
            ShortDescription = "a dollar bill",
            FullDescription = "This is a small bank note with the value of one dollar.",
            Value = 100.0M,
            Currency = currency,
            Weight = 1.0,
            GeneralForm = "note",
            PluralWord = "bill"
        };
        context.Coins.Add(coin);
        context.SaveChanges();

        coin = new Coin
        {
            Name = "five dollar bill",
            ShortDescription = "a five dollar bill",
            FullDescription = "This is a small bank note with the value of five dollars.",
            Value = 500.0M,
            Currency = currency,
            Weight = 1.0,
            GeneralForm = "note",
            PluralWord = "bill"
        };
        context.Coins.Add(coin);
        context.SaveChanges();

        coin = new Coin
        {
            Name = "ten dollar bill",
            ShortDescription = "a ten dollar bill",
            FullDescription = "This is a small bank note with the value of ten dollars.",
            Value = 1000.0M,
            Currency = currency,
            Weight = 1.0,
            GeneralForm = "note",
            PluralWord = "bill"
        };
        context.Coins.Add(coin);
        context.SaveChanges();

        coin = new Coin
        {
            Name = "twenty dollar bill",
            ShortDescription = "a twenty dollar bill",
            FullDescription = "This is a small bank note with the value of twenty dollars.",
            Value = 2000.0M,
            Currency = currency,
            Weight = 1.0,
            GeneralForm = "note",
            PluralWord = "bill"
        };
        context.Coins.Add(coin);
        context.SaveChanges();

        coin = new Coin
        {
            Name = "fifty dollar bill",
            ShortDescription = "a fifty dollar bill",
            FullDescription = "This is a small bank note with the value of fifty dollars.",
            Value = 5000.0M,
            Currency = currency,
            Weight = 1.0,
            GeneralForm = "note",
            PluralWord = "bill"
        };
        context.Coins.Add(coin);
        context.SaveChanges();

        coin = new Coin
        {
            Name = "hundred dollar bill",
            ShortDescription = "a hundred dollar bill",
            FullDescription = "This is a small bank note with the value of one hundred dollars.",
            Value = 10000.0M,
            Currency = currency,
            Weight = 1.0,
            GeneralForm = "note",
            PluralWord = "bill"
        };
        context.Coins.Add(coin);
        context.SaveChanges();
    }

    private void SeedFantasy(FuturemudDatabaseContext context, ICollection<string> errors)
    {
        Currency currency = new()
        {
            Name = "Standard"
        };
        context.Currencies.Add(currency);
        context.SaveChanges();

        CurrencyDivision division = new()
        {
            Currency = currency,
            Name = "brass",
            BaseUnitConversionRate = 1.0M
        };
        context.CurrencyDivisions.Add(division);
        CurrencyDivision brass = division;
        division.CurrencyDivisionAbbreviations.Add(new CurrencyDivisionAbbreviation
        { CurrencyDivision = division, Pattern = @"(-?\d+(?:\.\d+)*)(?:\s*(?:b|brass|brasses))$" });
        context.SaveChanges();

        division = new CurrencyDivision
        {
            Currency = currency,
            Name = "copper",
            BaseUnitConversionRate = 10.0M
        };
        context.CurrencyDivisions.Add(division);
        CurrencyDivision copper = division;
        division.CurrencyDivisionAbbreviations.Add(new CurrencyDivisionAbbreviation
        { CurrencyDivision = division, Pattern = @"(-?\d+(?:\.\d+)*)(?:\s*(?:c|copper|coppers))$" });
        context.SaveChanges();

        division = new CurrencyDivision
        {
            Currency = currency,
            Name = "silver",
            BaseUnitConversionRate = 100.0M
        };
        context.CurrencyDivisions.Add(division);
        CurrencyDivision silver = division;
        division.CurrencyDivisionAbbreviations.Add(new CurrencyDivisionAbbreviation
        { CurrencyDivision = division, Pattern = @"(-?\d+(?:\.\d+)*)(?:\s*(?:s|silver|silvers))$" });
        context.SaveChanges();

        division = new CurrencyDivision
        {
            Currency = currency,
            Name = "gold",
            BaseUnitConversionRate = 1000.0M
        };
        context.CurrencyDivisions.Add(division);
        CurrencyDivision gold = division;
        division.CurrencyDivisionAbbreviations.Add(new CurrencyDivisionAbbreviation
        { CurrencyDivision = division, Pattern = @"(-?\d+(?:\.\d+)*)(?:\s*(?:g|gold|golds))$" });
        context.SaveChanges();

        division = new CurrencyDivision
        {
            Currency = currency,
            Name = "platinum",
            BaseUnitConversionRate = 10000.0M
        };
        context.CurrencyDivisions.Add(division);
        CurrencyDivision platinum = division;
        division.CurrencyDivisionAbbreviations.Add(new CurrencyDivisionAbbreviation
        { CurrencyDivision = division, Pattern = @"(-?\d+(?:\.\d+)*)(?:\s*(?:p|platinum|platinums))$" });
        context.SaveChanges();

        foreach (CurrencyDescriptionPatternType type in Enum.GetValues(typeof(CurrencyDescriptionPatternType))
                     .OfType<CurrencyDescriptionPatternType>())
        {
            string prefix;
            switch (type)
            {
                case CurrencyDescriptionPatternType.Casual:
                case CurrencyDescriptionPatternType.Wordy:
                    prefix = "negative ";
                    break;
                case CurrencyDescriptionPatternType.Long:
                case CurrencyDescriptionPatternType.Short:
                case CurrencyDescriptionPatternType.ShortDecimal:
                    prefix = "-";
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            CurrencyDescriptionPattern pattern = new()
            {
                Currency = currency,
                Type = (int)type,
                NegativePrefix = prefix,
                Order = 1,
                UseNaturalAggregationStyle = true
            };
            context.CurrencyDescriptionPatterns.Add(pattern);

            switch (type)
            {
                case CurrencyDescriptionPatternType.Casual:
                    pattern.CurrencyDescriptionPatternElements.Add(new CurrencyDescriptionPatternElement
                    {
                        CurrencyDescriptionPattern = pattern,
                        Pattern = "{0} platinum",
                        Order = 1,
                        ShowIfZero = false,
                        CurrencyDivision = platinum,
                        PluraliseWord = "",
                        RoundingMode = (int)RoundingMode.NoRounding,
                        SpecialValuesOverrideFormat = false
                    });
                    pattern.CurrencyDescriptionPatternElements.Add(new CurrencyDescriptionPatternElement
                    {
                        CurrencyDescriptionPattern = pattern,
                        Pattern = "{0} gold",
                        Order = 2,
                        ShowIfZero = false,
                        CurrencyDivision = gold,
                        PluraliseWord = "",
                        RoundingMode = (int)RoundingMode.NoRounding,
                        SpecialValuesOverrideFormat = false
                    });
                    pattern.CurrencyDescriptionPatternElements.Add(new CurrencyDescriptionPatternElement
                    {
                        CurrencyDescriptionPattern = pattern,
                        Pattern = "{0} silver",
                        Order = 3,
                        ShowIfZero = false,
                        CurrencyDivision = silver,
                        PluraliseWord = "",
                        RoundingMode = (int)RoundingMode.NoRounding,
                        SpecialValuesOverrideFormat = false
                    });
                    pattern.CurrencyDescriptionPatternElements.Add(new CurrencyDescriptionPatternElement
                    {
                        CurrencyDescriptionPattern = pattern,
                        Pattern = "{0} copper",
                        Order = 4,
                        ShowIfZero = false,
                        CurrencyDivision = copper,
                        PluraliseWord = "",
                        RoundingMode = (int)RoundingMode.NoRounding,
                        SpecialValuesOverrideFormat = false
                    });
                    pattern.CurrencyDescriptionPatternElements.Add(new CurrencyDescriptionPatternElement
                    {
                        CurrencyDescriptionPattern = pattern,
                        Pattern = "{0} brass",
                        Order = 5,
                        ShowIfZero = false,
                        CurrencyDivision = brass,
                        PluraliseWord = "",
                        RoundingMode = (int)RoundingMode.NoRounding,
                        SpecialValuesOverrideFormat = false
                    });
                    break;
                case CurrencyDescriptionPatternType.Wordy:
                    pattern.CurrencyDescriptionPatternElements.Add(new CurrencyDescriptionPatternElement
                    {
                        CurrencyDescriptionPattern = pattern,
                        Pattern = "{0} platinum piece",
                        Order = 1,
                        ShowIfZero = false,
                        CurrencyDivision = platinum,
                        PluraliseWord = "piece",
                        RoundingMode = (int)RoundingMode.NoRounding,
                        SpecialValuesOverrideFormat = false
                    });
                    pattern.CurrencyDescriptionPatternElements.Add(new CurrencyDescriptionPatternElement
                    {
                        CurrencyDescriptionPattern = pattern,
                        Pattern = "{0} gold piece",
                        Order = 2,
                        ShowIfZero = false,
                        CurrencyDivision = gold,
                        PluraliseWord = "piece",
                        RoundingMode = (int)RoundingMode.NoRounding,
                        SpecialValuesOverrideFormat = false
                    });
                    pattern.CurrencyDescriptionPatternElements.Add(new CurrencyDescriptionPatternElement
                    {
                        CurrencyDescriptionPattern = pattern,
                        Pattern = "{0} silver piece",
                        Order = 3,
                        ShowIfZero = false,
                        CurrencyDivision = silver,
                        PluraliseWord = "piece",
                        RoundingMode = (int)RoundingMode.NoRounding,
                        SpecialValuesOverrideFormat = false
                    });
                    pattern.CurrencyDescriptionPatternElements.Add(new CurrencyDescriptionPatternElement
                    {
                        CurrencyDescriptionPattern = pattern,
                        Pattern = "{0} copper piece",
                        Order = 4,
                        ShowIfZero = false,
                        CurrencyDivision = copper,
                        PluraliseWord = "piece",
                        RoundingMode = (int)RoundingMode.NoRounding,
                        SpecialValuesOverrideFormat = false
                    });
                    pattern.CurrencyDescriptionPatternElements.Add(new CurrencyDescriptionPatternElement
                    {
                        CurrencyDescriptionPattern = pattern,
                        Pattern = "{0} brass piece",
                        Order = 5,
                        ShowIfZero = false,
                        CurrencyDivision = brass,
                        PluraliseWord = "piece",
                        RoundingMode = (int)RoundingMode.NoRounding,
                        SpecialValuesOverrideFormat = false
                    });
                    break;
                case CurrencyDescriptionPatternType.Long:
                case CurrencyDescriptionPatternType.Short:
                case CurrencyDescriptionPatternType.ShortDecimal:
                    pattern.CurrencyDescriptionPatternElements.Add(new CurrencyDescriptionPatternElement
                    {
                        CurrencyDescriptionPattern = pattern,
                        Pattern = "{0}p",
                        Order = 1,
                        ShowIfZero = false,
                        CurrencyDivision = platinum,
                        PluraliseWord = "",
                        RoundingMode = (int)RoundingMode.NoRounding,
                        SpecialValuesOverrideFormat = false
                    });
                    pattern.CurrencyDescriptionPatternElements.Add(new CurrencyDescriptionPatternElement
                    {
                        CurrencyDescriptionPattern = pattern,
                        Pattern = "{0}g",
                        Order = 2,
                        ShowIfZero = false,
                        CurrencyDivision = gold,
                        PluraliseWord = "",
                        RoundingMode = (int)RoundingMode.NoRounding,
                        SpecialValuesOverrideFormat = false
                    });
                    pattern.CurrencyDescriptionPatternElements.Add(new CurrencyDescriptionPatternElement
                    {
                        CurrencyDescriptionPattern = pattern,
                        Pattern = "{0}s",
                        Order = 3,
                        ShowIfZero = false,
                        CurrencyDivision = silver,
                        PluraliseWord = "",
                        RoundingMode = (int)RoundingMode.NoRounding,
                        SpecialValuesOverrideFormat = false
                    });
                    pattern.CurrencyDescriptionPatternElements.Add(new CurrencyDescriptionPatternElement
                    {
                        CurrencyDescriptionPattern = pattern,
                        Pattern = "{0}c",
                        Order = 4,
                        ShowIfZero = false,
                        CurrencyDivision = copper,
                        PluraliseWord = "",
                        RoundingMode = (int)RoundingMode.NoRounding,
                        SpecialValuesOverrideFormat = false
                    });
                    pattern.CurrencyDescriptionPatternElements.Add(new CurrencyDescriptionPatternElement
                    {
                        CurrencyDescriptionPattern = pattern,
                        Pattern = "{0}b",
                        Order = 5,
                        ShowIfZero = false,
                        CurrencyDivision = brass,
                        PluraliseWord = "",
                        RoundingMode = (int)RoundingMode.NoRounding,
                        SpecialValuesOverrideFormat = false
                    });
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        context.SaveChanges();


        Coin coin = new()
        {
            Name = "brass",
            ShortDescription = "a brass coin",
            FullDescription = "This is a small coin made of brass.",
            Value = 1.0M,
            Currency = currency,
            Weight = 30.0,
            GeneralForm = "coin",
            PluralWord = "coin"
        };
        context.Coins.Add(coin);
        context.SaveChanges();

        coin = new Coin
        {
            Name = "copper",
            ShortDescription = "a copper coin",
            FullDescription = "This is a small coin made of copper.",
            Value = 10.0M,
            Currency = currency,
            Weight = 30.0,
            GeneralForm = "coin",
            PluralWord = "coin"
        };
        context.Coins.Add(coin);
        context.SaveChanges();

        coin = new Coin
        {
            Name = "silver",
            ShortDescription = "a silver coin",
            FullDescription = "This is a small coin made of silver.",
            Value = 100.0M,
            Currency = currency,
            Weight = 30.0,
            GeneralForm = "coin",
            PluralWord = "coin"
        };
        context.Coins.Add(coin);
        context.SaveChanges();

        coin = new Coin
        {
            Name = "gold",
            ShortDescription = "a gold coin",
            FullDescription = "This is a small coin made of gold.",
            Value = 1000.0M,
            Currency = currency,
            Weight = 30.0,
            GeneralForm = "coin",
            PluralWord = "coin"
        };
        context.Coins.Add(coin);
        context.SaveChanges();

        coin = new Coin
        {
            Name = "platinum",
            ShortDescription = "a platinum coin",
            FullDescription = "This is a small coin made of platinum.",
            Value = 10000.0M,
            Currency = currency,
            Weight = 30.0,
            GeneralForm = "coin",
            PluralWord = "coin"
        };
        context.Coins.Add(coin);
        context.SaveChanges();
    }

	private void SeedPounds(FuturemudDatabaseContext context, ICollection<string> errors)
	{
		var currency = new Currency
		{
			Name = "Pounds"
		};
		context.Currencies.Add(currency);
		context.SaveChanges();

		var division = new CurrencyDivision
		{
			Currency = currency,
			Name = "penny",
			BaseUnitConversionRate = 4.0M
		};
		context.CurrencyDivisions.Add(division);
		var pennyDivision = division;
		division.CurrencyDivisionAbbreviations.Add(new CurrencyDivisionAbbreviation
		{
			CurrencyDivision = division,
			Pattern = @"(-?\d+(?:\.\d+)*)(?:\s*(?:penny|pennies|p|d))$"
		});
		context.SaveChanges();

		division = new CurrencyDivision
		{
			Currency = currency,
			Name = "shilling",
			BaseUnitConversionRate = 48.0M
		};
		context.CurrencyDivisions.Add(division);
		var shillingDivision = division;
		division.CurrencyDivisionAbbreviations.Add(new CurrencyDivisionAbbreviation
		{
			CurrencyDivision = division,
			Pattern = @"(-?\d+(?:\.\d+)*)(?:\s*(?:shillings|shilling|s))$"
		});
		context.SaveChanges();

		division = new CurrencyDivision
		{
			Currency = currency,
			Name = "pound",
			BaseUnitConversionRate = 960.0M
		};
		context.CurrencyDivisions.Add(division);
		var poundDivision = division;
		division.CurrencyDivisionAbbreviations.Add(new CurrencyDivisionAbbreviation
		{
			CurrencyDivision = division,
			Pattern = @"(-?\d+(?:\.\d+)*)(?:\s*(?:pounds|pound|l|lb|£))$"
		});
		context.SaveChanges();

		division = new CurrencyDivision
		{
			Currency = currency,
			Name = "farthing",
			BaseUnitConversionRate = 1.0M
		};
		context.CurrencyDivisions.Add(division);
		var farthingDivision = division;
		division.CurrencyDivisionAbbreviations.Add(new CurrencyDivisionAbbreviation
		{
			CurrencyDivision = division,
			Pattern = @"(-?\d+(?:\.\d+)*)(?:\s*(?:farthings|farthing|f))$"
		});
		context.SaveChanges();

		var lessThanOneShillingProg = EnsureNumericLessThanProg(context, "IsLessThanFortyEight", "48");
		var lessThanOnePoundProg = EnsureNumericLessThanProg(context, "IsLessThanNineHundredSixty", "960");

		foreach (var type in Enum.GetValues(typeof(CurrencyDescriptionPatternType)).OfType<CurrencyDescriptionPatternType>())
		{
			string prefix;
			switch (type)
			{
				case CurrencyDescriptionPatternType.Casual:
				case CurrencyDescriptionPatternType.Wordy:
					prefix = "negative ";
					break;
				case CurrencyDescriptionPatternType.Long:
				case CurrencyDescriptionPatternType.Short:
				case CurrencyDescriptionPatternType.ShortDecimal:
					prefix = "-";
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}

			if (type is CurrencyDescriptionPatternType.Casual or CurrencyDescriptionPatternType.Wordy)
			{
				var pattern = new CurrencyDescriptionPattern
				{
					Currency = currency,
					Type = (int)type,
					NegativePrefix = prefix,
					Order = 1,
					UseNaturalAggregationStyle = true
				};
				context.CurrencyDescriptionPatterns.Add(pattern);

				pattern.CurrencyDescriptionPatternElements.Add(new CurrencyDescriptionPatternElement
				{
					CurrencyDescriptionPattern = pattern,
					Pattern = "{0} pound",
					Order = 1,
					ShowIfZero = false,
					CurrencyDivision = poundDivision,
					PluraliseWord = "pound",
					RoundingMode = (int)RoundingMode.Truncate,
					SpecialValuesOverrideFormat = false
				});
				pattern.CurrencyDescriptionPatternElements.Add(new CurrencyDescriptionPatternElement
				{
					CurrencyDescriptionPattern = pattern,
					Pattern = "{0} shilling",
					Order = 2,
					ShowIfZero = false,
					CurrencyDivision = shillingDivision,
					PluraliseWord = "shilling",
					RoundingMode = (int)RoundingMode.Truncate,
					SpecialValuesOverrideFormat = false
				});

				var pennyElement = new CurrencyDescriptionPatternElement
				{
					CurrencyDescriptionPattern = pattern,
					Pattern = "{0} penny",
					Order = 3,
					ShowIfZero = false,
					CurrencyDivision = pennyDivision,
					PluraliseWord = "penny",
					RoundingMode = (int)RoundingMode.Truncate,
					SpecialValuesOverrideFormat = true
				};
				pattern.CurrencyDescriptionPatternElements.Add(pennyElement);
				AddSpecialValue(pennyElement, 2.0M, "tuppence");
				AddSpecialValue(pennyElement, 3.0M, "thruppence");
				AddSpecialValue(pennyElement, 4.0M, "fourpence");
				AddSpecialValue(pennyElement, 6.0M, "sixpence");

				var farthingElement = new CurrencyDescriptionPatternElement
				{
					CurrencyDescriptionPattern = pattern,
					Pattern = "{0} farthing",
					Order = 4,
					ShowIfZero = false,
					CurrencyDivision = farthingDivision,
					PluraliseWord = "farthing",
					RoundingMode = (int)RoundingMode.Truncate,
					SpecialValuesOverrideFormat = true
				};
				pattern.CurrencyDescriptionPatternElements.Add(farthingElement);
				AddSpecialValue(farthingElement, 1.0M, "farthing");
				AddSpecialValue(farthingElement, 2.0M, "ha'penny");
				AddSpecialValue(farthingElement, 3.0M, "three farthings");
				continue;
			}

			var pencePattern = new CurrencyDescriptionPattern
			{
				Currency = currency,
				Type = (int)type,
				NegativePrefix = prefix,
				Order = 10,
				UseNaturalAggregationStyle = false,
				FutureProg = lessThanOneShillingProg
			};
			context.CurrencyDescriptionPatterns.Add(pencePattern);
			var penceElement = new CurrencyDescriptionPatternElement
			{
				CurrencyDescriptionPattern = pencePattern,
				Pattern = "{0:0}d",
				Order = 1,
				ShowIfZero = false,
				CurrencyDivision = pennyDivision,
				PluraliseWord = "",
				RoundingMode = (int)RoundingMode.NoRounding,
				SpecialValuesOverrideFormat = true
			};
			pencePattern.CurrencyDescriptionPatternElements.Add(penceElement);
			AddSterlingFractionalPenceSpecialValues(penceElement, "d");

			var shillingPattern = new CurrencyDescriptionPattern
			{
				Currency = currency,
				Type = (int)type,
				NegativePrefix = prefix,
				Order = 20,
				UseNaturalAggregationStyle = false,
				FutureProg = lessThanOnePoundProg
			};
			context.CurrencyDescriptionPatterns.Add(shillingPattern);
			shillingPattern.CurrencyDescriptionPatternElements.Add(new CurrencyDescriptionPatternElement
			{
				CurrencyDescriptionPattern = shillingPattern,
				Pattern = "{0:0}/",
				Order = 1,
				ShowIfZero = false,
				CurrencyDivision = shillingDivision,
				PluraliseWord = "",
				RoundingMode = (int)RoundingMode.Truncate,
				SpecialValuesOverrideFormat = false
			});
			var slashPenceElement = new CurrencyDescriptionPatternElement
			{
				CurrencyDescriptionPattern = shillingPattern,
				Pattern = "{0:0}",
				Order = 2,
				ShowIfZero = true,
				CurrencyDivision = pennyDivision,
				PluraliseWord = "",
				RoundingMode = (int)RoundingMode.NoRounding,
				SpecialValuesOverrideFormat = true
			};
			shillingPattern.CurrencyDescriptionPatternElements.Add(slashPenceElement);
			AddSpecialValue(slashPenceElement, 0.0M, "–");
			AddSterlingFractionalPenceSpecialValues(slashPenceElement, string.Empty);

			var poundPattern = new CurrencyDescriptionPattern
			{
				Currency = currency,
				Type = (int)type,
				NegativePrefix = prefix,
				Order = 30,
				UseNaturalAggregationStyle = false
			};
			context.CurrencyDescriptionPatterns.Add(poundPattern);
			poundPattern.CurrencyDescriptionPatternElements.Add(new CurrencyDescriptionPatternElement
			{
				CurrencyDescriptionPattern = poundPattern,
				Pattern = "£{0:0}/",
				Order = 1,
				ShowIfZero = false,
				CurrencyDivision = poundDivision,
				PluraliseWord = "",
				RoundingMode = (int)RoundingMode.Truncate,
				SpecialValuesOverrideFormat = false
			});
			var poundShillingElement = new CurrencyDescriptionPatternElement
			{
				CurrencyDescriptionPattern = poundPattern,
				Pattern = "{0:0}/",
				Order = 2,
				ShowIfZero = true,
				CurrencyDivision = shillingDivision,
				PluraliseWord = "",
				RoundingMode = (int)RoundingMode.Truncate,
				SpecialValuesOverrideFormat = true
			};
			poundPattern.CurrencyDescriptionPatternElements.Add(poundShillingElement);
			AddSpecialValue(poundShillingElement, 0.0M, "–/");

			var poundPenceElement = new CurrencyDescriptionPatternElement
			{
				CurrencyDescriptionPattern = poundPattern,
				Pattern = "{0:0}",
				Order = 3,
				ShowIfZero = true,
				CurrencyDivision = pennyDivision,
				PluraliseWord = "",
				RoundingMode = (int)RoundingMode.NoRounding,
				SpecialValuesOverrideFormat = true
			};
			poundPattern.CurrencyDescriptionPatternElements.Add(poundPenceElement);
			AddSpecialValue(poundPenceElement, 0.0M, "–");
			AddSterlingFractionalPenceSpecialValues(poundPenceElement, string.Empty);
		}

		context.SaveChanges();

		var coin = new Coin
		{
			Name = "farthing",
			ShortDescription = "a farthing",
            FullDescription =
                "This is a small coin made of copper approximately 21mm in diameter. It is worth the least of all of the coins.",
            Value = 1.0M,
            Currency = currency,
            Weight = 2.8,
            GeneralForm = "coin",
            PluralWord = "farthing"
        };
        context.Coins.Add(coin);
        context.SaveChanges();

		coin = new Coin
		{
			Name = "ha'penny",
            ShortDescription = "a ha'penny bit",
            FullDescription = "This is a small coin made of copper approximately 26mm in diameter.",
            Value = 2.0M,
            Currency = currency,
            Weight = 5.6,
            GeneralForm = "coin",
            PluralWord = "bit"
        };
        context.Coins.Add(coin);
        context.SaveChanges();

        coin = new Coin
        {
            Name = "penny",
            ShortDescription = "a penny",
            FullDescription = "This is a small coin made of bronze approximately 31mm in diameter.",
            Value = 4.0M,
            Currency = currency,
            Weight = 7.0,
            GeneralForm = "coin",
            PluralWord = "penny"
        };
        context.Coins.Add(coin);
        context.SaveChanges();

        coin = new Coin
        {
            Name = "thrupenny",
            ShortDescription = "a thrupenny bit",
            FullDescription =
                "This is a small coin made of silver approximately 16mm in diameter and with 12 straight edges.",
            Value = 12.0M,
            Currency = currency,
            Weight = 1.4,
            GeneralForm = "coin",
            PluralWord = "bit"
        };
        context.Coins.Add(coin);
        context.SaveChanges();

        coin = new Coin
        {
            Name = "sixpenny",
            ShortDescription = "a sixpenny bit",
            FullDescription = "This is a small round coin made of silver approximately 19mm in diameter.",
            Value = 24.0M,
            Currency = currency,
            Weight = 2.83,
            GeneralForm = "coin",
            PluralWord = "bit"
        };
        context.Coins.Add(coin);
        context.SaveChanges();

        coin = new Coin
        {
            Name = "shilling",
            ShortDescription = "a shilling",
            FullDescription = "This is a small round coin made of silver approximately 23mm in diameter.",
            Value = 48.0M,
            Currency = currency,
            Weight = 5.66,
            GeneralForm = "coin",
            PluralWord = "shilling"
        };
        context.Coins.Add(coin);
        context.SaveChanges();

        coin = new Coin
        {
            Name = "florin",
            ShortDescription = "a florin",
            FullDescription = "This is a round coin made of silver approximately 30mm in diameter.",
            Value = 96.0M,
            Currency = currency,
            Weight = 11.3,
            GeneralForm = "coin",
            PluralWord = "florin"
        };
        context.Coins.Add(coin);
        context.SaveChanges();

        coin = new Coin
        {
            Name = "half crown",
            ShortDescription = "a half crown",
            FullDescription = "This is a round coin made of silver approximately 32mm in diameter.",
            Value = 120.0M,
            Currency = currency,
            Weight = 14.4,
            GeneralForm = "coin",
            PluralWord = "crown"
        };
        context.Coins.Add(coin);
        context.SaveChanges();

        coin = new Coin
        {
            Name = "crown",
            ShortDescription = "a crown",
            FullDescription = "This is a round coin made of silver approximately 38mm in diameter.",
            Value = 240.0M,
            Currency = currency,
            Weight = 18.0,
            GeneralForm = "coin",
            PluralWord = "crown"
        };
        context.Coins.Add(coin);
        context.SaveChanges();

        coin = new Coin
        {
            Name = "half sovereign",
            ShortDescription = "a half sovereign",
            FullDescription = "This is a round coin made of gold approximately 19mm in diameter.",
            Value = 480.0M,
            Currency = currency,
            Weight = 3.9,
            GeneralForm = "coin",
            PluralWord = "sovereign"
        };
        context.Coins.Add(coin);
        context.SaveChanges();

        coin = new Coin
        {
            Name = "sovereign",
            ShortDescription = "a sovereign",
            FullDescription = "This is a round coin made of gold approximately 22mm in diameter.",
            Value = 960.0M,
            Currency = currency,
            Weight = 7.98,
            GeneralForm = "coin",
            PluralWord = "sovereign"
        };
        context.Coins.Add(coin);
        context.SaveChanges();
    }

    private void SeedRoman(FuturemudDatabaseContext context, ICollection<string> errors)
    {
        Currency currency = new()
        {
            Name = "Roman"
        };
        context.Currencies.Add(currency);
        context.SaveChanges();

        CurrencyDivision division = new()
        {
            Currency = currency,
            Name = "uncia",
            BaseUnitConversionRate = 1.0M
        };
        context.CurrencyDivisions.Add(division);
        CurrencyDivision unciaDivision = division;
        division.CurrencyDivisionAbbreviations.Add(new CurrencyDivisionAbbreviation
        { CurrencyDivision = division, Pattern = @"(-?\d+(?:\.\d+)*)(?:\s*(?:uncia|uncii|unc|u))$" });
        context.SaveChanges();

        division = new CurrencyDivision
        {
            Currency = currency,
            Name = "asarius",
            BaseUnitConversionRate = 12.0M
        };
        context.CurrencyDivisions.Add(division);
        CurrencyDivision asariusDivision = division;
        division.CurrencyDivisionAbbreviations.Add(new CurrencyDivisionAbbreviation
        { CurrencyDivision = division, Pattern = @"(-?\d+(?:\.\d+)*)(?:\s*(?:asarius|asarii|a|as))$" });
        context.SaveChanges();

        division = new CurrencyDivision
        {
            Currency = currency,
            Name = "sestertius",
            BaseUnitConversionRate = 48.0M
        };

        context.CurrencyDivisions.Add(division);
        division.CurrencyDivisionAbbreviations.Add(new CurrencyDivisionAbbreviation
        { CurrencyDivision = division, Pattern = @"(-?\d+(?:\.\d+)*)(?:\s*(?:sestertius|sestertii|s))$" });
        foreach (CurrencyDescriptionPatternType type in Enum.GetValues(typeof(CurrencyDescriptionPatternType))
                     .OfType<CurrencyDescriptionPatternType>())
        {
            string prefix;
            switch (type)
            {
                case CurrencyDescriptionPatternType.Casual:
                case CurrencyDescriptionPatternType.Wordy:
                    prefix = "negative ";
                    break;
                case CurrencyDescriptionPatternType.Long:
                case CurrencyDescriptionPatternType.Short:
                case CurrencyDescriptionPatternType.ShortDecimal:
                    prefix = "-";
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            CurrencyDescriptionPattern pattern = new()
            {
                Currency = currency,
                Type = (int)type,
                NegativePrefix = prefix,
                Order = 1,
                UseNaturalAggregationStyle = true
            };
            context.CurrencyDescriptionPatterns.Add(pattern);
            switch (type)
            {
                case CurrencyDescriptionPatternType.Casual:
                case CurrencyDescriptionPatternType.Wordy:
                    pattern.CurrencyDescriptionPatternElements.Add(new CurrencyDescriptionPatternElement
                    {
                        CurrencyDescriptionPattern = pattern,
                        Pattern = "{0} sestertius",
                        Order = 1,
                        ShowIfZero = false,
                        CurrencyDivision = division,
                        PluraliseWord = "sestertius",
                        RoundingMode = (int)RoundingMode.Truncate,
                        SpecialValuesOverrideFormat = false
                    });
                    pattern.CurrencyDescriptionPatternElements.Add(new CurrencyDescriptionPatternElement
                    {
                        CurrencyDescriptionPattern = pattern,
                        Pattern = "{0} as",
                        Order = 2,
                        ShowIfZero = false,
                        CurrencyDivision = asariusDivision,
                        PluraliseWord = "",
                        RoundingMode = (int)RoundingMode.Truncate,
                        SpecialValuesOverrideFormat = false
                    });
                    pattern.CurrencyDescriptionPatternElements.Add(new CurrencyDescriptionPatternElement
                    {
                        CurrencyDescriptionPattern = pattern,
                        Pattern = "{0} uncia",
                        Order = 3,
                        ShowIfZero = false,
                        CurrencyDivision = unciaDivision,
                        PluraliseWord = "uncia",
                        RoundingMode = (int)RoundingMode.Truncate,
                        SpecialValuesOverrideFormat = false
                    });
                    break;
                case CurrencyDescriptionPatternType.Long:
                case CurrencyDescriptionPatternType.Short:
                case CurrencyDescriptionPatternType.ShortDecimal:
                    pattern.CurrencyDescriptionPatternElements.Add(new CurrencyDescriptionPatternElement
                    {
                        CurrencyDescriptionPattern = pattern,
                        Pattern = "{0}s",
                        Order = 1,
                        ShowIfZero = false,
                        CurrencyDivision = division,
                        PluraliseWord = "",
                        RoundingMode = (int)RoundingMode.Truncate,
                        SpecialValuesOverrideFormat = false
                    });
                    pattern.CurrencyDescriptionPatternElements.Add(new CurrencyDescriptionPatternElement
                    {
                        CurrencyDescriptionPattern = pattern,
                        Pattern = "{0}as",
                        Order = 2,
                        ShowIfZero = false,
                        CurrencyDivision = asariusDivision,
                        PluraliseWord = "",
                        RoundingMode = (int)RoundingMode.Truncate,
                        SpecialValuesOverrideFormat = false
                    });
                    pattern.CurrencyDescriptionPatternElements.Add(new CurrencyDescriptionPatternElement
                    {
                        CurrencyDescriptionPattern = pattern,
                        Pattern = "{0}u",
                        Order = 3,
                        ShowIfZero = false,
                        CurrencyDivision = unciaDivision,
                        PluraliseWord = "",
                        RoundingMode = (int)RoundingMode.Truncate,
                        SpecialValuesOverrideFormat = false
                    });
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        context.SaveChanges();

        division = new CurrencyDivision
        {
            Currency = currency,
            Name = "denarius",
            BaseUnitConversionRate = 192.0M
        };
        context.CurrencyDivisions.Add(division);
        division.CurrencyDivisionAbbreviations.Add(new CurrencyDivisionAbbreviation
        { CurrencyDivision = division, Pattern = @"(-?\d+(?:\.\d+)*)(?:\s*(?:denarius|denarii|d))$" });

        division = new CurrencyDivision
        {
            Currency = currency,
            Name = "quadrans",
            BaseUnitConversionRate = 3.0M
        };
        context.CurrencyDivisions.Add(division);
        division.CurrencyDivisionAbbreviations.Add(new CurrencyDivisionAbbreviation
        { CurrencyDivision = division, Pattern = @"(-?\d+(?:\.\d+)*)(?:\s*(?:quadrans|quadrantes|quad|q))$" });

        Coin coin = new()
        {
            Name = "uncia",
            ShortDescription = "an uncia",
            FullDescription =
                "This is a small coin made of cast brass approximately 14mm in diameter. On the obverse is an image of the helmeted bust of Roma, the personification of Rome. It is worth the least of all the roman coins.",
            Value = 1.0M,
            Currency = currency,
            Weight = 1.0,
            GeneralForm = "coin",
            PluralWord = "uncia"
        };
        context.Coins.Add(coin);
        context.SaveChanges();

        coin = new Coin
        {
            Name = "quadrans",
            ShortDescription = "a quadrans",
            FullDescription =
                "This is a small coin made of bronze and stamped with 3 circular marks representing its value as three unciae; this gives rise to another name for the coin, which is the tercunius (three unciae). On the obverse is an image of a bust of Hercules. On the reverse is an image of the prow of a Roman galley.",
            Value = 3.0M,
            Currency = currency,
            Weight = 3.0,
            GeneralForm = "coin",
            PluralWord = "uncia"
        };
        context.Coins.Add(coin);
        context.SaveChanges();

        coin = new Coin
        {
            Name = "semis",
            ShortDescription = "a semi",
            FullDescription =
                "This is a small coin made of bronze, cast in a die, and stamped with 6 circular marks representing its value as six unciae. On the obverse is an image of Saturn. On the reverse is an image of the prow of a Roman galley.",
            Value = 6.0M,
            Currency = currency,
            Weight = 6.0,
            GeneralForm = "coin",
            PluralWord = "semi"
        };
        context.Coins.Add(coin);
        context.SaveChanges();

        coin = new Coin
        {
            Name = "assarius",
            ShortDescription = "an assarius",
            FullDescription =
                "This is a medium-sized thin coin made of bronze and cast in a die. On the obverse is an image of the two-headed god Janus. On the reverse is an image of the prow of a Roman galley.",
            Value = 12.0M,
            Currency = currency,
            Weight = 12.0,
            GeneralForm = "coin",
            PluralWord = "assarius"
        };
        context.Coins.Add(coin);
        context.SaveChanges();

        coin = new Coin
        {
            Name = "dupondius",
            ShortDescription = "a dupondius",
            FullDescription =
                "This is a large cast brass coin. On the obverse is an image of the helmeted bust of Roma, the personification of the city of Rome. On the reverse is an image of a large chariot wheel. The details on the obverse and reverse images are of superb clarity.",
            Value = 24.0M,
            Currency = currency,
            Weight = 15.0,
            GeneralForm = "coin",
            PluralWord = "dupondius"
        };
        context.Coins.Add(coin);
        context.SaveChanges();

        coin = new Coin
        {
            Name = "sestertius",
            ShortDescription = "a sestertius",
            FullDescription =
                "This is a small silver coin. On the obverse is an image of the twin gods Castor and Pollux (known together as Dioscuri). On the reverse is an image of the helmeted bust of Roma, the personification of the city of Rome. The details on the obverse and reverse images are of superb clarity.",
            Value = 48.0M,
            Currency = currency,
            Weight = 1.5,
            GeneralForm = "coin",
            PluralWord = "sestertius"
        };
        context.Coins.Add(coin);
        context.SaveChanges();

        coin = new Coin
        {
            Name = "denarius",
            ShortDescription = "a denarius",
            FullDescription =
                "This is a silver coin. On the obverse is an image of the twin gods Castor and Pollux (known together as Dioscuri). On the reverse is an image of the helmeted bust of Roma, the personification of the city of Rome. The details on the obverse and reverse images are of superb clarity.",
            Value = 192.0M,
            Currency = currency,
            Weight = 3.9,
            GeneralForm = "coin",
            PluralWord = "denarius"
        };
        context.Coins.Add(coin);
        context.SaveChanges();

        coin = new Coin
        {
            Name = "aureus",
            ShortDescription = "an aureus",
            FullDescription =
                "This is a gold coin approximately the same size as a sestertius. On the obverse is an image of the goddess Vesta, who is veiled. On the reverse are images of the lituus (the wand of an augur), a jug and and axe. The details on the obverse and reverse images are of superb clarity.",
            Value = 4800.0M,
            Currency = currency,
            Weight = 1.9,
            GeneralForm = "coin",
            PluralWord = "aureus"
        };
        context.Coins.Add(coin);
        context.SaveChanges();
    }

    private void SeedBits(FuturemudDatabaseContext context, ICollection<string> errors)
    {
        Currency currency = new()
        {
            Name = "Bits"
        };
        context.Currencies.Add(currency);
        context.SaveChanges();

        CurrencyDivision division = new()
        {
            Currency = currency,
            Name = "bits",
            BaseUnitConversionRate = 1.0M
        };
        context.CurrencyDivisions.Add(division);
        division.CurrencyDivisionAbbreviations.Add(new CurrencyDivisionAbbreviation
        { CurrencyDivision = division, Pattern = @"(-?\d+(?:\.\d+)*)(?:\s*(?:b|bits|bit))$" });
        foreach (CurrencyDescriptionPatternType type in Enum.GetValues(typeof(CurrencyDescriptionPatternType))
                     .OfType<CurrencyDescriptionPatternType>())
        {
            string prefix;
            switch (type)
            {
                case CurrencyDescriptionPatternType.Casual:
                case CurrencyDescriptionPatternType.Wordy:
                    prefix = "negative ";
                    break;
                case CurrencyDescriptionPatternType.Long:
                case CurrencyDescriptionPatternType.Short:
                case CurrencyDescriptionPatternType.ShortDecimal:
                    prefix = "-";
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            CurrencyDescriptionPattern pattern = new()
            {
                Currency = currency,
                Type = (int)type,
                NegativePrefix = prefix,
                Order = 1
            };
            context.CurrencyDescriptionPatterns.Add(pattern);
            switch (type)
            {
                case CurrencyDescriptionPatternType.Casual:
                    pattern.CurrencyDescriptionPatternElements.Add(new CurrencyDescriptionPatternElement
                    {
                        CurrencyDescriptionPattern = pattern,
                        Pattern = "{0} bit",
                        Order = 1,
                        ShowIfZero = false,
                        CurrencyDivision = division,
                        PluraliseWord = "bit",
                        RoundingMode = (int)RoundingMode.Truncate,
                        SpecialValuesOverrideFormat = false
                    });
                    break;
                case CurrencyDescriptionPatternType.Wordy:
                    pattern.CurrencyDescriptionPatternElements.Add(new CurrencyDescriptionPatternElement
                    {
                        CurrencyDescriptionPattern = pattern,
                        Pattern = "{0} bit",
                        Order = 1,
                        ShowIfZero = false,
                        CurrencyDivision = division,
                        PluraliseWord = "bit",
                        RoundingMode = (int)RoundingMode.Truncate,
                        SpecialValuesOverrideFormat = false
                    });
                    break;
                case CurrencyDescriptionPatternType.Long:
                case CurrencyDescriptionPatternType.Short:
                case CurrencyDescriptionPatternType.ShortDecimal:
                    pattern.CurrencyDescriptionPatternElements.Add(new CurrencyDescriptionPatternElement
                    {
                        CurrencyDescriptionPattern = pattern,
                        Pattern = "{0}b",
                        Order = 1,
                        ShowIfZero = false,
                        CurrencyDivision = division,
                        PluraliseWord = "",
                        RoundingMode = (int)RoundingMode.Truncate,
                        SpecialValuesOverrideFormat = false
                    });
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        context.SaveChanges();


        Coin coin = new()
        {
            Name = "bit",
            ShortDescription = "a copper bit",
            FullDescription = "This is a small coin made of copper.",
            Value = 1.0M,
            Currency = currency,
            Weight = 30.0,
            GeneralForm = "coin",
            PluralWord = "bit"
        };
        context.Coins.Add(coin);
        context.SaveChanges();
    }

    private void SeedShadowsOfIsildur(FuturemudDatabaseContext context, ICollection<string> errors)
    {
        void AddCoin(Currency coinCurrency, string name, string plural, string sdesc, string desc, decimal value)
        {
            Coin coin = new()
            {
                Name = name,
                ShortDescription = sdesc,
                FullDescription = desc,
                Value = value,
                Currency = coinCurrency,
                Weight = 4.54,
                GeneralForm = "coin",
                PluralWord = plural,
                UseForChange = true
            };
            context.Coins.Add(coin);
        }

        #region Gondor
        Currency currency = new()
        {
            Name = "Gondorian"
        };
        context.Currencies.Add(currency);
        context.SaveChanges();

        CurrencyDivision division = new()
        {
            Currency = currency,
            Name = "copper",
            BaseUnitConversionRate = 1.0M
        };
        context.CurrencyDivisions.Add(division);
        division.CurrencyDivisionAbbreviations.Add(new CurrencyDivisionAbbreviation
        { CurrencyDivision = division, Pattern = @"(-?\d+(?:\.\d+)*)(?:\s*(?:c|copper|cp|b|bits|bit))$" });
        foreach (CurrencyDescriptionPatternType type in Enum.GetValues(typeof(CurrencyDescriptionPatternType))
                     .OfType<CurrencyDescriptionPatternType>())
        {
            string prefix;
            switch (type)
            {
                case CurrencyDescriptionPatternType.Casual:
                case CurrencyDescriptionPatternType.Wordy:
                    prefix = "negative ";
                    break;
                case CurrencyDescriptionPatternType.Long:
                case CurrencyDescriptionPatternType.Short:
                case CurrencyDescriptionPatternType.ShortDecimal:
                    prefix = "-";
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            CurrencyDescriptionPattern pattern = new()
            {
                Currency = currency,
                Type = (int)type,
                NegativePrefix = prefix,
                Order = 1
            };
            context.CurrencyDescriptionPatterns.Add(pattern);
            switch (type)
            {
                case CurrencyDescriptionPatternType.Casual:
                    pattern.CurrencyDescriptionPatternElements.Add(new CurrencyDescriptionPatternElement
                    {
                        CurrencyDescriptionPattern = pattern,
                        Pattern = "{0} copper",
                        Order = 1,
                        ShowIfZero = false,
                        CurrencyDivision = division,
                        PluraliseWord = "copper",
                        RoundingMode = (int)RoundingMode.Truncate,
                        SpecialValuesOverrideFormat = false
                    });
                    break;
                case CurrencyDescriptionPatternType.Wordy:
                    pattern.CurrencyDescriptionPatternElements.Add(new CurrencyDescriptionPatternElement
                    {
                        CurrencyDescriptionPattern = pattern,
                        Pattern = "{0} copper",
                        Order = 1,
                        ShowIfZero = false,
                        CurrencyDivision = division,
                        PluraliseWord = "copper",
                        RoundingMode = (int)RoundingMode.Truncate,
                        SpecialValuesOverrideFormat = false
                    });
                    break;
                case CurrencyDescriptionPatternType.Long:
                case CurrencyDescriptionPatternType.Short:
                case CurrencyDescriptionPatternType.ShortDecimal:
                    pattern.CurrencyDescriptionPatternElements.Add(new CurrencyDescriptionPatternElement
                    {
                        CurrencyDescriptionPattern = pattern,
                        Pattern = "{0}cp",
                        Order = 1,
                        ShowIfZero = false,
                        CurrencyDivision = division,
                        PluraliseWord = "",
                        RoundingMode = (int)RoundingMode.Truncate,
                        SpecialValuesOverrideFormat = false
                    });
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        context.SaveChanges();

        AddCoin(currency, "copper bit", "coin", "semicircular copper coin", @"This small, semicircular wedge of copper, known as a 'copper bit', or, in Sindarin, 'benhar', has slightly serrated edges. The face of the coin bears the graven image of the Steward of Gondor, and the obverse, a carefully-wrought likeness of the Star of Elendil.", 1);
        AddCoin(currency, "bronze copper", "coin", "large, rounded bronze coin", @"This bronze coin, known commonly as a 'bronze copper', or, in Sindarin, as 'tamb', bears the noble likeness of the Steward of Gondor upon its face. The obverse has been cast with an image of the city of Minas Tirith itself, its great size and strength evident from its size against the large Mountain upon which it was constructed.", 5);
        AddCoin(currency, "silver royal", "coin", "thin, ridged silver coin", @"This thin silver coin, known as a 'silver royal', or, in Sindarin, 'celebarn', bears a slightly serrated circumference. The face of the coin has been cast with the solemn likeness of the Steward of Gondor, and the obverse features the image of what appears to be the mythical Stone of Anor.", 50);
        AddCoin(currency, "silver tree", "coin", "heavy, oblong silver coin", @"   This heavy, oblong silver coin, known as a 'silver tree', or, in Sindarin, 'nimloth', gleams dully. Graven on its face is the noble likeness of the Steward of Gondor, and on the obverse, an image of the legendary Telperion, the eldest of the long lost Two Trees of Valinor.", 200);
        AddCoin(currency, "gold crown", "coin", "thick, hexagonal gold coin", @"This heavy golden coin, known as a 'gold crown', or, in Sindarin, 'erin', has been cast in a hexagonal shape. Its face bears the painstakingly graven image of the Steward of Gondor, and on the obverse rests a likeness of Minas Tirith's gallant and fabled White Citadel.", 1000);
        AddCoin(currency, "gold hundredpiece", "coin", "thin, slightly fluted gold coin", @"This small, delicately-wrought golden coin is known as a 'gold hundredpiece', or, in Sindarin, 'harancor'. Upon its face it bears the graven likeness of the latest Steward of Gondor, and upon the obverse, an image of Galathilion, the fabled White Tree of Yavanna Herself.", 10000);
        AddCoin(currency, "mithril orb", "coin", "glittering mithril coin", @"Known in the Kingdom of Gondor as the 'mithril orb', or, in Sindarin, as 'malanor', this brilliant mithril coin is worth a good deal. On the front side rests a graven image of the Steward of Gondor, and on the obverse, the image of Minas Tirith's White Tree has been lovingly cast.", 10000);
        context.SaveChanges();
        #endregion

        #region Mordor
        currency = new()
        {
            Name = "Mordorian"
        };
        context.Currencies.Add(currency);
        context.SaveChanges();

        division = new()
        {
            Currency = currency,
            Name = "copper",
            BaseUnitConversionRate = 1.0M
        };
        context.CurrencyDivisions.Add(division);
        division.CurrencyDivisionAbbreviations.Add(new CurrencyDivisionAbbreviation
        { CurrencyDivision = division, Pattern = @"(-?\d+(?:\.\d+)*)(?:\s*(?:c|copper|cp|b|bits|bit))$" });
        foreach (CurrencyDescriptionPatternType type in Enum.GetValues(typeof(CurrencyDescriptionPatternType))
                     .OfType<CurrencyDescriptionPatternType>())
        {
            string prefix;
            switch (type)
            {
                case CurrencyDescriptionPatternType.Casual:
                case CurrencyDescriptionPatternType.Wordy:
                    prefix = "negative ";
                    break;
                case CurrencyDescriptionPatternType.Long:
                case CurrencyDescriptionPatternType.Short:
                case CurrencyDescriptionPatternType.ShortDecimal:
                    prefix = "-";
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            CurrencyDescriptionPattern pattern = new()
            {
                Currency = currency,
                Type = (int)type,
                NegativePrefix = prefix,
                Order = 1
            };
            context.CurrencyDescriptionPatterns.Add(pattern);
            switch (type)
            {
                case CurrencyDescriptionPatternType.Casual:
                    pattern.CurrencyDescriptionPatternElements.Add(new CurrencyDescriptionPatternElement
                    {
                        CurrencyDescriptionPattern = pattern,
                        Pattern = "{0} copper",
                        Order = 1,
                        ShowIfZero = false,
                        CurrencyDivision = division,
                        PluraliseWord = "copper",
                        RoundingMode = (int)RoundingMode.Truncate,
                        SpecialValuesOverrideFormat = false
                    });
                    break;
                case CurrencyDescriptionPatternType.Wordy:
                    pattern.CurrencyDescriptionPatternElements.Add(new CurrencyDescriptionPatternElement
                    {
                        CurrencyDescriptionPattern = pattern,
                        Pattern = "{0} copper",
                        Order = 1,
                        ShowIfZero = false,
                        CurrencyDivision = division,
                        PluraliseWord = "copper",
                        RoundingMode = (int)RoundingMode.Truncate,
                        SpecialValuesOverrideFormat = false
                    });
                    break;
                case CurrencyDescriptionPatternType.Long:
                case CurrencyDescriptionPatternType.Short:
                case CurrencyDescriptionPatternType.ShortDecimal:
                    pattern.CurrencyDescriptionPatternElements.Add(new CurrencyDescriptionPatternElement
                    {
                        CurrencyDescriptionPattern = pattern,
                        Pattern = "{0}cp",
                        Order = 1,
                        ShowIfZero = false,
                        CurrencyDivision = division,
                        PluraliseWord = "",
                        RoundingMode = (int)RoundingMode.Truncate,
                        SpecialValuesOverrideFormat = false
                    });
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        context.SaveChanges();

        AddCoin(currency, "bit", "coin", "crudely-hewn token of dark granite", @"This crudely-hewn token of dark granite is small and uneven, its edges chipped into a rough semblance of a coin. One face bears the shallow mark of a lidless eye, scratched rather than properly graven, while the obverse has been cut with a jagged tower rising beneath a thin crescent moon. Its surface is dark, cold, and pitted, with pale grit caught in the grooves.", 1);
        AddCoin(currency, "obsidian", "coin", "razor-sharp flake of obsidian", @"This razor-sharp flake of black obsidian has been knapped into a narrow, coin-like shard, though its edges remain keen enough to draw blood. A crude sigil of fangs has been etched across one glossy face, and the obverse shows a crooked mountain peak beneath a staring eye. The glassy stone catches the light in hard, smoky flashes, giving it a cruel and splintered beauty.", 5);
        AddCoin(currency, "brass", "coin", "rectangular token of dusky brass", @"This rectangular token of dusky brass is thin and worn, its corners blunted from long handling. Upon one face is stamped a blackened gate flanked by cruel, narrow towers, while the obverse bears a hook-bladed crown above a line of angular runes. The metal has tarnished to a dull brown-gold, darker in the recessed marks where old grime and soot have gathered.", 50);
        AddCoin(currency, "bronze", "coin", "weighty pentagonal bronze coin", @"This weighty pentagonal bronze coin is thick for its size, with bevelled sides and a dark, oily patina. The face bears the harsh profile of a helmed lord, the features stern and almost skull-like beneath a spiked war-crown. On the obverse is stamped a gauntleted hand gripping a barbed mace, surrounded by a ring of angular script worn nearly smooth by use.", 200);
        AddCoin(currency, "steel", "coin", "hexagonal token of blackened steel", @"This hexagonal token of blackened steel is heavy, hard-edged, and cold to the touch. Its face has been stamped with the image of a tall tower under a lidless eye, the lines stark and severe against the dark metal. The obverse bears a ring of cruel runes surrounding three downward-pointing blades, their points meeting at the centre. The token's edges are darkened with soot and age, as though it has passed through many grim hands.", 1000);
        AddCoin(currency, "silver", "coin", "octagonal coin of smoky silver", @"This octagonal coin of smoky silver is broad and finely made, though its surface has been deliberately darkened to a dull, ghostly sheen. The face bears the likeness of a crowned and mail-clad sorcerer-king, his features hidden beneath a high helm and shadowed brow. Upon the obverse is graven the image of Minas Morgul, its narrow towers rising beneath a sickle moon, with a border of sharp runes worked carefully around the edge.", 10000);
        context.SaveChanges();
        #endregion

        #region Tur Edendor
        currency = new()
        {
            Name = "Tur Edendor"
        };
        context.Currencies.Add(currency);
        context.SaveChanges();

        division = new()
        {
            Currency = currency,
            Name = "engren",
            BaseUnitConversionRate = 1.0M
        };
        context.CurrencyDivisions.Add(division);
        division.CurrencyDivisionAbbreviations.Add(new CurrencyDivisionAbbreviation
        { CurrencyDivision = division, Pattern = @"(-?\d+(?:\.\d+)*)(?:\s*(?:c|copper|coppers|cp|b|bits|bit|e|en|eng|engren|engrens))$" });
        foreach (CurrencyDescriptionPatternType type in Enum.GetValues(typeof(CurrencyDescriptionPatternType))
                     .OfType<CurrencyDescriptionPatternType>())
        {
            string prefix;
            switch (type)
            {
                case CurrencyDescriptionPatternType.Casual:
                case CurrencyDescriptionPatternType.Wordy:
                    prefix = "negative ";
                    break;
                case CurrencyDescriptionPatternType.Long:
                case CurrencyDescriptionPatternType.Short:
                case CurrencyDescriptionPatternType.ShortDecimal:
                    prefix = "-";
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            CurrencyDescriptionPattern pattern = new()
            {
                Currency = currency,
                Type = (int)type,
                NegativePrefix = prefix,
                Order = 1
            };
            context.CurrencyDescriptionPatterns.Add(pattern);
            switch (type)
            {
                case CurrencyDescriptionPatternType.Casual:
                    pattern.CurrencyDescriptionPatternElements.Add(new CurrencyDescriptionPatternElement
                    {
                        CurrencyDescriptionPattern = pattern,
                        Pattern = "{0} engren",
                        Order = 1,
                        ShowIfZero = false,
                        CurrencyDivision = division,
                        PluraliseWord = "engren",
                        RoundingMode = (int)RoundingMode.Truncate,
                        SpecialValuesOverrideFormat = false
                    });
                    break;
                case CurrencyDescriptionPatternType.Wordy:
                    pattern.CurrencyDescriptionPatternElements.Add(new CurrencyDescriptionPatternElement
                    {
                        CurrencyDescriptionPattern = pattern,
                        Pattern = "{0} engren",
                        Order = 1,
                        ShowIfZero = false,
                        CurrencyDivision = division,
                        PluraliseWord = "engren",
                        RoundingMode = (int)RoundingMode.Truncate,
                        SpecialValuesOverrideFormat = false
                    });
                    break;
                case CurrencyDescriptionPatternType.Long:
                case CurrencyDescriptionPatternType.Short:
                case CurrencyDescriptionPatternType.ShortDecimal:
                    pattern.CurrencyDescriptionPatternElements.Add(new CurrencyDescriptionPatternElement
                    {
                        CurrencyDescriptionPattern = pattern,
                        Pattern = "{0}en",
                        Order = 1,
                        ShowIfZero = false,
                        CurrencyDivision = division,
                        PluraliseWord = "",
                        RoundingMode = (int)RoundingMode.Truncate,
                        SpecialValuesOverrideFormat = false
                    });
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        context.SaveChanges();

        AddCoin(currency, "iron", "coin", "small iron disc", @"This small iron disc looks no bigger than a woman's thumb, and has a design lightly stamped on each side. On the obverse, a regal image of an eldery, grim-faced man is in profile, a spiked crown set on top of his head. The reverse of the coin bears a picture of a ring of mountains rising up from the flat earth.", 1);
        AddCoin(currency, "copper", "coin", "dull copper coin", @"This small coin, easily of size to fit inside a man's palm, has been minted of some sort of copper alloy. The metal is rather dull, and looks to be somewhat tarnished and corroded about the edges of the coin. On the obverse, a regal image of an eldery, grim-faced man is in profile, a spiked crown set on top of his head. The reverse bears a design of a fort covered in flames, the base surrounded by tiny armoured men.", 5);
        AddCoin(currency, "silver", "coin", "small, circular silver coin", @"This palm-sized coin has been minted from an alloy of silver, the metal somewhat dark and dusky hinting at traces of nickel and tin. On the obverse, a regal image of an eldery, grim-faced man is in profile, a spiked crown set on top of his head. The reverse bears a picture of two jagged lightning bolts crossed over one another, a slender halo encircling them both.", 50);
        AddCoin(currency, "silver", "coin", "brazen silver coin", @"This palm-sized coin has been minted from a strange alloy of silver, the pale silver bearing a slight brazen tinge to it when viewed in certain lights. On the obverse, a regal image of an eldery, grim-faced man is in profile, a spiked crown set on top of his head. The reverse bears a design of this crown, its simplistic, craggy points depicted from a slight angle.", 200);
        AddCoin(currency, "silver", "coin", "ornate silver coin", @"This shiny silver coin is quite large, and ornate, swirling curves have been pressed along the edges of both side of the coin. On the obverse, a regal image of an elderly, grim-faced man is in profile, a spiked crown set on top of his head. The reverse bears a picture of a large section of land, complete with fields, forests, mountains and rivers, set above it a spiked crown.", 1000);
        AddCoin(currency, "gold", "coin", "heavy, gleaming, gold coin", @"This heavy gold coin is also quite large, and elaborately decorated with a design of vines and flowers about the borders of both side of the coin. On the obverse, a detailed and regal image of an elderly, grim-faced man is in profile, a spiked crown set on top of his head. The reverse bears the man in full, clad in a suit of plate armour and with a wicked-headed polearm held in one hand, still in profile.", 10000);
        context.SaveChanges();
        #endregion

        #region Harad
        currency = new()
        {
            Name = "Harad"
        };
        context.Currencies.Add(currency);
        context.SaveChanges();

        division = new()
        {
            Currency = currency,
            Name = "copper",
            BaseUnitConversionRate = 1.0M
        };
        context.CurrencyDivisions.Add(division);
        division.CurrencyDivisionAbbreviations.Add(new CurrencyDivisionAbbreviation
        { CurrencyDivision = division, Pattern = @"(-?\d+(?:\.\d+)*)(?:\s*(?:c|copper|cp|b|bits|bit))$" });
        foreach (CurrencyDescriptionPatternType type in Enum.GetValues(typeof(CurrencyDescriptionPatternType))
                     .OfType<CurrencyDescriptionPatternType>())
        {
            string prefix;
            switch (type)
            {
                case CurrencyDescriptionPatternType.Casual:
                case CurrencyDescriptionPatternType.Wordy:
                    prefix = "negative ";
                    break;
                case CurrencyDescriptionPatternType.Long:
                case CurrencyDescriptionPatternType.Short:
                case CurrencyDescriptionPatternType.ShortDecimal:
                    prefix = "-";
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            CurrencyDescriptionPattern pattern = new()
            {
                Currency = currency,
                Type = (int)type,
                NegativePrefix = prefix,
                Order = 1
            };
            context.CurrencyDescriptionPatterns.Add(pattern);
            switch (type)
            {
                case CurrencyDescriptionPatternType.Casual:
                    pattern.CurrencyDescriptionPatternElements.Add(new CurrencyDescriptionPatternElement
                    {
                        CurrencyDescriptionPattern = pattern,
                        Pattern = "{0} copper",
                        Order = 1,
                        ShowIfZero = false,
                        CurrencyDivision = division,
                        PluraliseWord = "copper",
                        RoundingMode = (int)RoundingMode.Truncate,
                        SpecialValuesOverrideFormat = false
                    });
                    break;
                case CurrencyDescriptionPatternType.Wordy:
                    pattern.CurrencyDescriptionPatternElements.Add(new CurrencyDescriptionPatternElement
                    {
                        CurrencyDescriptionPattern = pattern,
                        Pattern = "{0} copper",
                        Order = 1,
                        ShowIfZero = false,
                        CurrencyDivision = division,
                        PluraliseWord = "copper",
                        RoundingMode = (int)RoundingMode.Truncate,
                        SpecialValuesOverrideFormat = false
                    });
                    break;
                case CurrencyDescriptionPatternType.Long:
                case CurrencyDescriptionPatternType.Short:
                case CurrencyDescriptionPatternType.ShortDecimal:
                    pattern.CurrencyDescriptionPatternElements.Add(new CurrencyDescriptionPatternElement
                    {
                        CurrencyDescriptionPattern = pattern,
                        Pattern = "{0}cp",
                        Order = 1,
                        ShowIfZero = false,
                        CurrencyDivision = division,
                        PluraliseWord = "",
                        RoundingMode = (int)RoundingMode.Truncate,
                        SpecialValuesOverrideFormat = false
                    });
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        context.SaveChanges();

        AddCoin(currency, "copper", "coin", "small, round copper coin", @"Covered with a idealized likeness of a mumak, this small, round copper coin is smooth but shows some wear. The obverse depicts a shaft of wheat. The edges are crudely serrated and some wording is rubbed to smooth to read. It is about the size of a man's thumb nail.", 1);
        AddCoin(currency, "bronze", "coin", "large, round bronze coin", @"Minted from a mixture of tin and copper, this large round coin boasts the etching of a stylized foot bone. The obverse depicts an etching of five shafts of wheat tied in a bundle. The edges are serrated and what appears to be wording is rubbed too smooth to read. It fits snuggly in a curled up fist.", 5);
        AddCoin(currency, "silver lion", "coin", "thin, round silver coin", @"This thin and round silver coin is no bigger than an inch in diameter. The face side depicts the head of a roaring lion while the obverse depicts an etching of ten bundles of wheat. The serrated edges are beveled on either side and they are almost worn smooth.", 50);
        AddCoin(currency, "silver fish", "coin", "thick, heavy silver coin", @"A stylized cornucopia graces this thick, heavy silver coin. Round in shape and about the diameter of a small fig, the sides of this coin bevel out to its serrated edges. The obverse boasts the image of a fish swimming in a stylized river.", 200);
        AddCoin(currency, "gold scimitar", "coin", "small, round gold coin", @"This small, round gold coin is no bigger than the nail on a man's little finger. It's thin edges are smooth but beveled out from the center. The image of a bearded man graces the face of this coin while the obverse displays a miniature broad-bladed scimitar. It is carefully rendered but shows some wear.", 1000);
        AddCoin(currency, "gold lord", "coin", "thick, heavy gold coin", @"A careful depiction of a bearded man wearing a turban dominates the face of this thick, heavy, round coin. The coin is large with smooth beveled edges. It is about the size of small child's palm and the obverse is etched with a single bundle of wheat neatly centered. The coin shows little wear but bears a delicate patina across it's gleaming surface.", 10000);
        context.SaveChanges();
        #endregion

        #region Northlands
        currency = new Currency
        {
            Name = "Northlands"
        };
        context.Currencies.Add(currency);
        context.SaveChanges();

        division = new CurrencyDivision
        {
            Currency = currency,
            Name = "penny",
            BaseUnitConversionRate = 4.0M
        };
        context.CurrencyDivisions.Add(division);
        var pennyDivision = division;
        division.CurrencyDivisionAbbreviations.Add(new CurrencyDivisionAbbreviation
        {
            CurrencyDivision = division,
            Pattern = @"(-?\d+(?:\.\d+)*)(?:\s*(?:penny|pennies|p|d))$"
        });
        context.SaveChanges();

        division = new CurrencyDivision
        {
            Currency = currency,
            Name = "shilling",
            BaseUnitConversionRate = 48.0M
        };
        context.CurrencyDivisions.Add(division);
        var shillingDivision = division;
        division.CurrencyDivisionAbbreviations.Add(new CurrencyDivisionAbbreviation
        {
            CurrencyDivision = division,
            Pattern = @"(-?\d+(?:\.\d+)*)(?:\s*(?:shillings|shilling|s))$"
        });
        context.SaveChanges();

        division = new CurrencyDivision
        {
            Currency = currency,
            Name = "pound",
            BaseUnitConversionRate = 960.0M
        };
        context.CurrencyDivisions.Add(division);
        var poundDivision = division;
        division.CurrencyDivisionAbbreviations.Add(new CurrencyDivisionAbbreviation
        {
            CurrencyDivision = division,
            Pattern = @"(-?\d+(?:\.\d+)*)(?:\s*(?:pounds|pound|l|lb|£))$"
        });
        context.SaveChanges();

        division = new CurrencyDivision
        {
            Currency = currency,
            Name = "farthing",
            BaseUnitConversionRate = 1.0M
        };
        context.CurrencyDivisions.Add(division);
        var farthingDivision = division;
        division.CurrencyDivisionAbbreviations.Add(new CurrencyDivisionAbbreviation
        {
            CurrencyDivision = division,
            Pattern = @"(-?\d+(?:\.\d+)*)(?:\s*(?:farthings|farthing|f))$"
        });
        context.SaveChanges();

        var lessThanOneShillingProg = EnsureNumericLessThanProg(context, "IsLessThanFortyEight", "48");
        var lessThanOnePoundProg = EnsureNumericLessThanProg(context, "IsLessThanNineHundredSixty", "960");

        foreach (var type in Enum.GetValues(typeof(CurrencyDescriptionPatternType)).OfType<CurrencyDescriptionPatternType>())
        {
            string prefix;
            switch (type)
            {
                case CurrencyDescriptionPatternType.Casual:
                case CurrencyDescriptionPatternType.Wordy:
                    prefix = "negative ";
                    break;
                case CurrencyDescriptionPatternType.Long:
                case CurrencyDescriptionPatternType.Short:
                case CurrencyDescriptionPatternType.ShortDecimal:
                    prefix = "-";
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            if (type is CurrencyDescriptionPatternType.Casual or CurrencyDescriptionPatternType.Wordy)
            {
                var pattern = new CurrencyDescriptionPattern
                {
                    Currency = currency,
                    Type = (int)type,
                    NegativePrefix = prefix,
                    Order = 1,
                    UseNaturalAggregationStyle = true
                };
                context.CurrencyDescriptionPatterns.Add(pattern);

                pattern.CurrencyDescriptionPatternElements.Add(new CurrencyDescriptionPatternElement
                {
                    CurrencyDescriptionPattern = pattern,
                    Pattern = "{0} pound",
                    Order = 1,
                    ShowIfZero = false,
                    CurrencyDivision = poundDivision,
                    PluraliseWord = "pound",
                    RoundingMode = (int)RoundingMode.Truncate,
                    SpecialValuesOverrideFormat = false
                });
                pattern.CurrencyDescriptionPatternElements.Add(new CurrencyDescriptionPatternElement
                {
                    CurrencyDescriptionPattern = pattern,
                    Pattern = "{0} shilling",
                    Order = 2,
                    ShowIfZero = false,
                    CurrencyDivision = shillingDivision,
                    PluraliseWord = "shilling",
                    RoundingMode = (int)RoundingMode.Truncate,
                    SpecialValuesOverrideFormat = false
                });

                var pennyElement = new CurrencyDescriptionPatternElement
                {
                    CurrencyDescriptionPattern = pattern,
                    Pattern = "{0} penny",
                    Order = 3,
                    ShowIfZero = false,
                    CurrencyDivision = pennyDivision,
                    PluraliseWord = "penny",
                    RoundingMode = (int)RoundingMode.Truncate,
                    SpecialValuesOverrideFormat = true
                };
                pattern.CurrencyDescriptionPatternElements.Add(pennyElement);
                AddSpecialValue(pennyElement, 2.0M, "tuppence");
                AddSpecialValue(pennyElement, 3.0M, "thruppence");
                AddSpecialValue(pennyElement, 4.0M, "fourpence");
                AddSpecialValue(pennyElement, 6.0M, "sixpence");

                var farthingElement = new CurrencyDescriptionPatternElement
                {
                    CurrencyDescriptionPattern = pattern,
                    Pattern = "{0} farthing",
                    Order = 4,
                    ShowIfZero = false,
                    CurrencyDivision = farthingDivision,
                    PluraliseWord = "farthing",
                    RoundingMode = (int)RoundingMode.Truncate,
                    SpecialValuesOverrideFormat = true
                };
                pattern.CurrencyDescriptionPatternElements.Add(farthingElement);
                AddSpecialValue(farthingElement, 1.0M, "farthing");
                AddSpecialValue(farthingElement, 2.0M, "ha'penny");
                AddSpecialValue(farthingElement, 3.0M, "three farthings");
                continue;
            }

            var pencePattern = new CurrencyDescriptionPattern
            {
                Currency = currency,
                Type = (int)type,
                NegativePrefix = prefix,
                Order = 10,
                UseNaturalAggregationStyle = false,
                FutureProg = lessThanOneShillingProg
            };
            context.CurrencyDescriptionPatterns.Add(pencePattern);
            var penceElement = new CurrencyDescriptionPatternElement
            {
                CurrencyDescriptionPattern = pencePattern,
                Pattern = "{0:0}d",
                Order = 1,
                ShowIfZero = false,
                CurrencyDivision = pennyDivision,
                PluraliseWord = "",
                RoundingMode = (int)RoundingMode.NoRounding,
                SpecialValuesOverrideFormat = true
            };
            pencePattern.CurrencyDescriptionPatternElements.Add(penceElement);
            AddSterlingFractionalPenceSpecialValues(penceElement, "d");

            var shillingPattern = new CurrencyDescriptionPattern
            {
                Currency = currency,
                Type = (int)type,
                NegativePrefix = prefix,
                Order = 20,
                UseNaturalAggregationStyle = false,
                FutureProg = lessThanOnePoundProg
            };
            context.CurrencyDescriptionPatterns.Add(shillingPattern);
            shillingPattern.CurrencyDescriptionPatternElements.Add(new CurrencyDescriptionPatternElement
            {
                CurrencyDescriptionPattern = shillingPattern,
                Pattern = "{0:0}/",
                Order = 1,
                ShowIfZero = false,
                CurrencyDivision = shillingDivision,
                PluraliseWord = "",
                RoundingMode = (int)RoundingMode.Truncate,
                SpecialValuesOverrideFormat = false
            });
            var slashPenceElement = new CurrencyDescriptionPatternElement
            {
                CurrencyDescriptionPattern = shillingPattern,
                Pattern = "{0:0}",
                Order = 2,
                ShowIfZero = true,
                CurrencyDivision = pennyDivision,
                PluraliseWord = "",
                RoundingMode = (int)RoundingMode.NoRounding,
                SpecialValuesOverrideFormat = true
            };
            shillingPattern.CurrencyDescriptionPatternElements.Add(slashPenceElement);
            AddSpecialValue(slashPenceElement, 0.0M, "–");
            AddSterlingFractionalPenceSpecialValues(slashPenceElement, string.Empty);

            var poundPattern = new CurrencyDescriptionPattern
            {
                Currency = currency,
                Type = (int)type,
                NegativePrefix = prefix,
                Order = 30,
                UseNaturalAggregationStyle = false
            };
            context.CurrencyDescriptionPatterns.Add(poundPattern);
            poundPattern.CurrencyDescriptionPatternElements.Add(new CurrencyDescriptionPatternElement
            {
                CurrencyDescriptionPattern = poundPattern,
                Pattern = "£{0:0}/",
                Order = 1,
                ShowIfZero = false,
                CurrencyDivision = poundDivision,
                PluraliseWord = "",
                RoundingMode = (int)RoundingMode.Truncate,
                SpecialValuesOverrideFormat = false
            });
            var poundShillingElement = new CurrencyDescriptionPatternElement
            {
                CurrencyDescriptionPattern = poundPattern,
                Pattern = "{0:0}/",
                Order = 2,
                ShowIfZero = true,
                CurrencyDivision = shillingDivision,
                PluraliseWord = "",
                RoundingMode = (int)RoundingMode.Truncate,
                SpecialValuesOverrideFormat = true
            };
            poundPattern.CurrencyDescriptionPatternElements.Add(poundShillingElement);
            AddSpecialValue(poundShillingElement, 0.0M, "–/");

            var poundPenceElement = new CurrencyDescriptionPatternElement
            {
                CurrencyDescriptionPattern = poundPattern,
                Pattern = "{0:0}",
                Order = 3,
                ShowIfZero = true,
                CurrencyDivision = pennyDivision,
                PluraliseWord = "",
                RoundingMode = (int)RoundingMode.NoRounding,
                SpecialValuesOverrideFormat = true
            };
            poundPattern.CurrencyDescriptionPatternElements.Add(poundPenceElement);
            AddSpecialValue(poundPenceElement, 0.0M, "–");
            AddSterlingFractionalPenceSpecialValues(poundPenceElement, string.Empty);
        }

        context.SaveChanges();

        AddCoin(currency, "farthing", "coin", "a small, round bronze coin", @"This small, round bronze coin is plain and sturdy, its surface dulled by handling and age. One face bears the stamped image of a boar, low and bristling, with its tusks picked out in shallow lines. The obverse shows a single sheaf of grain beneath a short line of angular lettering. Its edges are slightly uneven, but worn smooth from long use.", 1);
        AddCoin(currency, "penny", "coin", "a heavy, stamped bronze coin", @"This heavy bronze coin has been stamped thickly, with a broad rim and a dark, weathered surface. Upon the face is the image of a powerful boar, head lowered as if ready to charge, surrounded by small marks of value. The obverse bears a simple hill-fort above a line of plain northern script. The coin is practical rather than elegant, but solidly made.", 4);
        AddCoin(currency, "shilling", "coin", "a hexagonal, stamped silver coin", @"This hexagonal silver coin is broad and carefully stamped, its six edges rubbed bright from use. The face bears the image of a spread-winged eagle, its head turned sharply to one side and its talons extended beneath it. On the obverse is a second eagle in flight above a stylized mountain line, with terse lettering pressed around the border. The silver is somewhat worn, but still catches the light cleanly.", 48);
        AddCoin(currency, "pound", "coin", "a large, ornately-detailed silver coin", @"This large, ornately-detailed silver coin bears fine Tengwar abbreviations around its outer edge, each mark pressed with careful precision. One face shows the portrait of a stern Dalish man, bearded and broad-featured, his hair bound back and his cloak fastened at the shoulder. The obverse bears the same man in profile, framed by a border of eagles and boars worked in miniature. The coin is heavy, bright, and finely struck, plainly intended as a mark of wealth as much as currency.", 960);
        context.SaveChanges();
        #endregion
    }

    private void SeedGondorian(FuturemudDatabaseContext context, ICollection<string> errors)
    {
        Currency currency = new()
        {
            Name = "Gondorian"
        };
        context.Currencies.Add(currency);
        context.SaveChanges();

        CurrencyDivision division = new()
        {
            Currency = currency,
            Name = "penny",
            BaseUnitConversionRate = 4.0M
        };
        context.CurrencyDivisions.Add(division);
        CurrencyDivision pennyDivision = division;
        division.CurrencyDivisionAbbreviations.Add(new CurrencyDivisionAbbreviation
        { CurrencyDivision = division, Pattern = @"(-?\d+(?:\.\d+)*)(?:\s*(?:penny|pennies|p|d))$" });
        context.SaveChanges();

        division = new CurrencyDivision
        {
            Currency = currency,
            Name = "crown",
            BaseUnitConversionRate = 400.0M
        };
        context.CurrencyDivisions.Add(division);
        CurrencyDivision crownDivision = division;
        division.CurrencyDivisionAbbreviations.Add(new CurrencyDivisionAbbreviation
        { CurrencyDivision = division, Pattern = @"(-?\d+(?:\.\d+)*)(?:\s*(?:crowns|crown|c))$" });
        context.SaveChanges();

        division = new CurrencyDivision
        {
            Currency = currency,
            Name = "farthing",
            BaseUnitConversionRate = 1.0M
        };
        context.CurrencyDivisions.Add(division);
        CurrencyDivision farthingDivision = division;
        division.CurrencyDivisionAbbreviations.Add(new CurrencyDivisionAbbreviation
        { CurrencyDivision = division, Pattern = @"(-?\d+(?:\.\d+)*)(?:\s*(?:farthings|farthing|f))$" });
        context.SaveChanges();

        foreach (CurrencyDescriptionPatternType type in Enum.GetValues(typeof(CurrencyDescriptionPatternType))
                     .OfType<CurrencyDescriptionPatternType>())
        {
            string prefix;
            switch (type)
            {
                case CurrencyDescriptionPatternType.Casual:
                case CurrencyDescriptionPatternType.Wordy:
                    prefix = "negative ";
                    break;
                case CurrencyDescriptionPatternType.Long:
                case CurrencyDescriptionPatternType.Short:
                case CurrencyDescriptionPatternType.ShortDecimal:
                    prefix = "-";
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            CurrencyDescriptionPattern pattern = new()
            {
                Currency = currency,
                Type = (int)type,
                NegativePrefix = prefix,
                Order = 1,
                UseNaturalAggregationStyle = true
            };
            context.CurrencyDescriptionPatterns.Add(pattern);
            switch (type)
            {
                case CurrencyDescriptionPatternType.Casual:
                case CurrencyDescriptionPatternType.Wordy:
                    pattern.CurrencyDescriptionPatternElements.Add(new CurrencyDescriptionPatternElement
                    {
                        CurrencyDescriptionPattern = pattern,
                        Pattern = "{0} crown",
                        Order = 1,
                        ShowIfZero = false,
                        CurrencyDivision = crownDivision,
                        PluraliseWord = "crown",
                        RoundingMode = (int)RoundingMode.Truncate,
                        SpecialValuesOverrideFormat = false
                    });
                    CurrencyDescriptionPatternElement pennyElement = new()
                    {
                        CurrencyDescriptionPattern = pattern,
                        Pattern = "{0} penny",
                        Order = 2,
                        ShowIfZero = false,
                        CurrencyDivision = pennyDivision,
                        PluraliseWord = "penny",
                        RoundingMode = (int)RoundingMode.Truncate,
                        SpecialValuesOverrideFormat = true
                    };
                    pattern.CurrencyDescriptionPatternElements.Add(pennyElement);

                    pennyElement.CurrencyDescriptionPatternElementSpecialValues.Add(
                        new CurrencyDescriptionPatternElementSpecialValues
                        {
                            CurrencyDescriptionPatternElement = pennyElement,
                            Value = 8.0M,
                            Text = "tuppence"
                        });

                    pennyElement.CurrencyDescriptionPatternElementSpecialValues.Add(
                        new CurrencyDescriptionPatternElementSpecialValues
                        {
                            CurrencyDescriptionPatternElement = pennyElement,
                            Value = 12.0M,
                            Text = "thruppence"
                        });

                    pennyElement.CurrencyDescriptionPatternElementSpecialValues.Add(
                        new CurrencyDescriptionPatternElementSpecialValues
                        {
                            CurrencyDescriptionPatternElement = pennyElement,
                            Value = 16.0M,
                            Text = "fourpence"
                        });

                    pennyElement.CurrencyDescriptionPatternElementSpecialValues.Add(
                        new CurrencyDescriptionPatternElementSpecialValues
                        {
                            CurrencyDescriptionPatternElement = pennyElement,
                            Value = 24.0M,
                            Text = "sixpence"
                        });

                    CurrencyDescriptionPatternElement farthingElement = new()
                    {
                        CurrencyDescriptionPattern = pattern,
                        Pattern = "{0} farthing",
                        Order = 4,
                        ShowIfZero = false,
                        CurrencyDivision = farthingDivision,
                        PluraliseWord = "farthing",
                        RoundingMode = (int)RoundingMode.Truncate,
                        SpecialValuesOverrideFormat = true
                    };
                    pattern.CurrencyDescriptionPatternElements.Add(farthingElement);

                    farthingElement.CurrencyDescriptionPatternElementSpecialValues.Add(
                        new CurrencyDescriptionPatternElementSpecialValues
                        {
                            CurrencyDescriptionPatternElement = farthingElement,
                            Value = 1.0M,
                            Text = "farthing"
                        });

                    farthingElement.CurrencyDescriptionPatternElementSpecialValues.Add(
                        new CurrencyDescriptionPatternElementSpecialValues
                        {
                            CurrencyDescriptionPatternElement = farthingElement,
                            Value = 2.0M,
                            Text = "hapenny"
                        });

                    farthingElement.CurrencyDescriptionPatternElementSpecialValues.Add(
                        new CurrencyDescriptionPatternElementSpecialValues
                        {
                            CurrencyDescriptionPatternElement = farthingElement,
                            Value = 3.0M,
                            Text = "three farthing"
                        });
                    break;
                case CurrencyDescriptionPatternType.Long:
                case CurrencyDescriptionPatternType.Short:
                case CurrencyDescriptionPatternType.ShortDecimal:
                    pattern.CurrencyDescriptionPatternElements.Add(new CurrencyDescriptionPatternElement
                    {
                        CurrencyDescriptionPattern = pattern,
                        Pattern = "{0}C",
                        Order = 1,
                        ShowIfZero = false,
                        CurrencyDivision = crownDivision,
                        PluraliseWord = "",
                        RoundingMode = (int)RoundingMode.Truncate,
                        SpecialValuesOverrideFormat = false
                    });
                    pattern.CurrencyDescriptionPatternElements.Add(new CurrencyDescriptionPatternElement
                    {
                        CurrencyDescriptionPattern = pattern,
                        Pattern = "{0}p",
                        Order = 2,
                        ShowIfZero = false,
                        CurrencyDivision = pennyDivision,
                        PluraliseWord = "",
                        RoundingMode = (int)RoundingMode.Truncate,
                        SpecialValuesOverrideFormat = false
                    });
                    pattern.CurrencyDescriptionPatternElements.Add(new CurrencyDescriptionPatternElement
                    {
                        CurrencyDescriptionPattern = pattern,
                        Pattern = "{0}f",
                        Order = 3,
                        ShowIfZero = false,
                        CurrencyDivision = farthingDivision,
                        PluraliseWord = "",
                        RoundingMode = (int)RoundingMode.Truncate,
                        SpecialValuesOverrideFormat = false
                    });
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        context.SaveChanges();

        Coin coin = new()
        {
            Name = "farthing",
            ShortDescription = "a farthing",
            FullDescription = "This is a small round coin made primarily of copper.",
            Value = 1.0M,
            Currency = currency,
            Weight = 2.8,
            GeneralForm = "coin",
            PluralWord = "farthing"
        };
        context.Coins.Add(coin);
        context.SaveChanges();

        coin = new Coin
        {
            Name = "penny",
            ShortDescription = "a penny",
            FullDescription = "This is a large round coin made primarily of copper.",
            Value = 4.0M,
            Currency = currency,
            Weight = 10.0,
            GeneralForm = "coin",
            PluralWord = "penny"
        };
        context.Coins.Add(coin);
        context.SaveChanges();

        coin = new Coin
        {
            Name = "tharni",
            ShortDescription = "a tharni",
            FullDescription = "This is a small round coin made of silver.",
            Value = 100.0M,
            Currency = currency,
            Weight = 1.4,
            GeneralForm = "coin",
            PluralWord = "tharni"
        };
        context.Coins.Add(coin);
        context.SaveChanges();

        coin = new Coin
        {
            Name = "castar",
            ShortDescription = "a castar",
            FullDescription = "This is a small round coin made of gold.",
            Value = 400.0M,
            Currency = currency,
            Weight = 2.83,
            GeneralForm = "coin",
            PluralWord = "castar"
        };
        context.Coins.Add(coin);
        context.SaveChanges();
    }
}
