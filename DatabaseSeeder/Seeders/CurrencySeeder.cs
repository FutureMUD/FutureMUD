using System;
using System.Collections.Generic;
using System.Linq;
using MudSharp.Database;
using MudSharp.Economy.Currency;
using MudSharp.Framework;
using MudSharp.Models;

namespace DatabaseSeeder.Seeders;

public class CurrencySeeder : IDatabaseSeeder
{
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
							return (true, string.Empty);
					}

					return (false, "Invalid answer");
				})
		};

	public string SeedData(FuturemudDatabaseContext context, IReadOnlyDictionary<string, string> questionAnswers)
	{
		context.Database.BeginTransaction();
		var errors = new List<string>();
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
		}

		context.Database.CommitTransaction();

		if (errors.Count == 0) return "The operation completed successfully.";

		return
			$"The operation completed with the following errors or warnings:\n\n{errors.ListToCommaSeparatedValues("\n")}";
	}

	public ShouldSeedResult ShouldSeedData(FuturemudDatabaseContext context)
	{
		// Only requires the core data seeder
		if (!context.Accounts.Any()) return ShouldSeedResult.PrerequisitesNotMet;

		if (context.Currencies.Any()) return ShouldSeedResult.MayAlreadyBeInstalled;

		return ShouldSeedResult.ReadyToInstall;
	}

	public int SortOrder => 20;
	public string Name => "Currency";
	public string Tagline => "Set up a currency (or currencies) for your game";

	public string FullDescription =>
		"This package sets up everything you need to get a currency in game. This is required for some of the clan templates.";

	private void SeedDollars(FuturemudDatabaseContext context, ICollection<string> errors)
	{
		var currency = new Currency
		{
			Name = "Dollars"
		};
		context.Currencies.Add(currency);
		context.SaveChanges();

		if (!context.FutureProgs.Any(x => x.FunctionName == "IsLessThanOneHundred"))
		{
			var prog = new FutureProg
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

		var division = new CurrencyDivision
		{
			Currency = currency,
			Name = "cent",
			BaseUnitConversionRate = 1.0M
		};
		context.CurrencyDivisions.Add(division);
		var cent = division;
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
		var dollar = division;
		division.CurrencyDivisionAbbreviations.Add(new CurrencyDivisionAbbreviation
			{ CurrencyDivision = division, Pattern = @"(-?\d+(?:\.\d+)*)(?:\s*(?:c|cent|cents))$" });
		context.SaveChanges();

		foreach (var type in Enum.GetValues(typeof(CurrencyDescriptionPatternType))
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

			var pattern = new CurrencyDescriptionPattern
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

		var coin = new Coin
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
		var currency = new Currency
		{
			Name = "Standard"
		};
		context.Currencies.Add(currency);
		context.SaveChanges();

		var division = new CurrencyDivision
		{
			Currency = currency,
			Name = "brass",
			BaseUnitConversionRate = 1.0M
		};
		context.CurrencyDivisions.Add(division);
		var brass = division;
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
		var copper = division;
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
		var silver = division;
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
		var gold = division;
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
		var platinum = division;
		division.CurrencyDivisionAbbreviations.Add(new CurrencyDivisionAbbreviation
			{ CurrencyDivision = division, Pattern = @"(-?\d+(?:\.\d+)*)(?:\s*(?:p|platinum|platinums))$" });
		context.SaveChanges();

		foreach (var type in Enum.GetValues(typeof(CurrencyDescriptionPatternType))
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

			var pattern = new CurrencyDescriptionPattern
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


		var coin = new Coin
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
			{ CurrencyDivision = division, Pattern = @"(-?\d+(?:\.\d+)*)(?:\s*(?:penny|pennies|p|d))$" });
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
			{ CurrencyDivision = division, Pattern = @"(-?\d+(?:\.\d+)*)(?:\s*(?:shillings|shilling|s))$" });
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
			{ CurrencyDivision = division, Pattern = @"(-?\d+(?:\.\d+)*)(?:\s*(?:pounds|pound|l|lb|£))$" });
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
			{ CurrencyDivision = division, Pattern = @"(-?\d+(?:\.\d+)*)(?:\s*(?:farthings|farthing|f))$" });
		context.SaveChanges();

		foreach (var type in Enum.GetValues(typeof(CurrencyDescriptionPatternType))
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

			var pattern = new CurrencyDescriptionPattern
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
						Pattern = "£{0}",
						Order = 1,
						ShowIfZero = false,
						CurrencyDivision = poundDivision,
						PluraliseWord = "",
						RoundingMode = (int)RoundingMode.Truncate,
						SpecialValuesOverrideFormat = false
					});
					pattern.CurrencyDescriptionPatternElements.Add(new CurrencyDescriptionPatternElement
					{
						CurrencyDescriptionPattern = pattern,
						Pattern = "{0}s",
						Order = 2,
						ShowIfZero = false,
						CurrencyDivision = shillingDivision,
						PluraliseWord = "",
						RoundingMode = (int)RoundingMode.Truncate,
						SpecialValuesOverrideFormat = false
					});
					pattern.CurrencyDescriptionPatternElements.Add(new CurrencyDescriptionPatternElement
					{
						CurrencyDescriptionPattern = pattern,
						Pattern = "{0}d",
						Order = 3,
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
						Order = 4,
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
		var currency = new Currency
		{
			Name = "Roman"
		};
		context.Currencies.Add(currency);
		context.SaveChanges();

		var division = new CurrencyDivision
		{
			Currency = currency,
			Name = "uncia",
			BaseUnitConversionRate = 1.0M
		};
		context.CurrencyDivisions.Add(division);
		var unciaDivision = division;
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
		var asariusDivision = division;
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
		foreach (var type in Enum.GetValues(typeof(CurrencyDescriptionPatternType))
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

			var pattern = new CurrencyDescriptionPattern
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

		var coin = new Coin
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
		var currency = new Currency
		{
			Name = "Bits"
		};
		context.Currencies.Add(currency);
		context.SaveChanges();

		var division = new CurrencyDivision
		{
			Currency = currency,
			Name = "bits",
			BaseUnitConversionRate = 1.0M
		};
		context.CurrencyDivisions.Add(division);
		division.CurrencyDivisionAbbreviations.Add(new CurrencyDivisionAbbreviation
			{ CurrencyDivision = division, Pattern = @"(-?\d+(?:\.\d+)*)(?:\s*(?:b|bits|bit))$" });
		foreach (var type in Enum.GetValues(typeof(CurrencyDescriptionPatternType))
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

			var pattern = new CurrencyDescriptionPattern
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


		var coin = new Coin
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

	private void SeedGondorian(FuturemudDatabaseContext context, ICollection<string> errors)
	{
		var currency = new Currency
		{
			Name = "Gondorian"
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
			{ CurrencyDivision = division, Pattern = @"(-?\d+(?:\.\d+)*)(?:\s*(?:penny|pennies|p|d))$" });
		context.SaveChanges();

		division = new CurrencyDivision
		{
			Currency = currency,
			Name = "crown",
			BaseUnitConversionRate = 400.0M
		};
		context.CurrencyDivisions.Add(division);
		var crownDivision = division;
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
		var farthingDivision = division;
		division.CurrencyDivisionAbbreviations.Add(new CurrencyDivisionAbbreviation
			{ CurrencyDivision = division, Pattern = @"(-?\d+(?:\.\d+)*)(?:\s*(?:farthings|farthing|f))$" });
		context.SaveChanges();

		foreach (var type in Enum.GetValues(typeof(CurrencyDescriptionPatternType))
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

			var pattern = new CurrencyDescriptionPattern
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
					var pennyElement = new CurrencyDescriptionPatternElement
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

		var coin = new Coin
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