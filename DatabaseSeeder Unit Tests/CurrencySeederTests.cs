#nullable enable

using DatabaseSeeder.Seeders;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Database;
using MudSharp.Economy.Currency;
using MudSharp.Framework;
using MudSharp.Framework.Save;
using MudSharp.FutureProg;
using MudSharp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using RuntimeCurrency = MudSharp.Economy.Currency.Currency;
using ModelCurrency = MudSharp.Models.Currency;
using CultureInfo = System.Globalization.CultureInfo;

namespace MudSharp_Unit_Tests;

[TestClass]
public class CurrencySeederTests
{
	private static FuturemudDatabaseContext BuildContext()
	{
		DbContextOptions<FuturemudDatabaseContext> options = new DbContextOptionsBuilder<FuturemudDatabaseContext>()
			.UseInMemoryDatabase(Guid.NewGuid().ToString())
			.ConfigureWarnings(x => x.Ignore(InMemoryEventId.TransactionIgnoredWarning))
			.Options;
		return new FuturemudDatabaseContext(options);
	}

	private static RuntimeCurrency LoadRuntimeCurrency(FuturemudDatabaseContext context, string currencyName)
	{
		ModelCurrency dbCurrency = context.Currencies
			.Include(x => x.CurrencyDivisions)
				.ThenInclude(x => x.CurrencyDivisionAbbreviations)
			.Include(x => x.CurrencyDescriptionPatterns)
				.ThenInclude(x => x.CurrencyDescriptionPatternElements)
					.ThenInclude(x => x.CurrencyDescriptionPatternElementSpecialValues)
			.Include(x => x.Coins)
			.Single(x => x.Name == currencyName);

		List<IFutureProg> progs = context.FutureProgs
			.AsEnumerable()
			.Select(BuildFutureProg)
			.ToList();
		Mock<IUneditableAll<IFutureProg>> progRepository = BuildRepository(progs);
		Mock<IFuturemud> gameworld = new();
		gameworld.SetupGet(x => x.FutureProgs).Returns(progRepository.Object);
		gameworld.SetupGet(x => x.SaveManager).Returns(Mock.Of<ISaveManager>());
		gameworld.Setup(x => x.Add(It.IsAny<ICoin>()));

		return new RuntimeCurrency(dbCurrency, gameworld.Object);
	}

	private static IFutureProg BuildFutureProg(MudSharp.Models.FutureProg dbProg)
	{
		Mock<IFutureProg> prog = new();
		prog.SetupGet(x => x.Id).Returns(dbProg.Id);
		prog.SetupGet(x => x.Name).Returns(dbProg.FunctionName);
		prog.SetupGet(x => x.FrameworkItemType).Returns("FutureProg");
		prog.SetupGet(x => x.FunctionName).Returns(dbProg.FunctionName);
		prog.Setup(x => x.ExecuteBool(It.IsAny<object[]>()))
			.Returns((object[] values) => ExecuteComparison(dbProg.FunctionText, values));
		prog.Setup(x => x.ExecuteBool(It.IsAny<bool>(), It.IsAny<object[]>()))
			.Returns((bool _, object[] values) => ExecuteComparison(dbProg.FunctionText, values));
		return prog.Object;
	}

	private static bool ExecuteComparison(string functionText, IReadOnlyList<object> values)
	{
		decimal number = Convert.ToDecimal(values[0], CultureInfo.InvariantCulture);
		if (functionText.StartsWith("return @number < ", StringComparison.Ordinal))
		{
			decimal threshold = decimal.Parse(
				functionText["return @number < ".Length..],
				CultureInfo.InvariantCulture);
			return number < threshold;
		}

		throw new NotSupportedException($"Unsupported FutureProg comparison in test harness: {functionText}");
	}

	private static Mock<IUneditableAll<T>> BuildRepository<T>(IEnumerable<T> items) where T : class, IFrameworkItem
	{
		List<T> list = items.ToList();
		Dictionary<long, T> byId = list.ToDictionary(x => x.Id, x => x);
		Mock<IUneditableAll<T>> repository = new();
		repository.Setup(x => x.Get(It.IsAny<long>()))
			.Returns((long id) => byId.TryGetValue(id, out T? value) ? value : null);
		repository.Setup(x => x.GetEnumerator()).Returns(() => list.GetEnumerator());
		repository.SetupGet(x => x.Count).Returns(list.Count);
		return repository;
	}

	[TestMethod]
	public void SeedData_PoundsCompactPatterns_RenderHistoricalSterlingNotation()
	{
		using FuturemudDatabaseContext context = BuildContext();
		string result = new CurrencySeeder().SeedData(context, new Dictionary<string, string>
		{
			["currency"] = "pounds"
		});

		Assert.AreEqual("The operation completed successfully.", result);

		RuntimeCurrency currency = LoadRuntimeCurrency(context, "Pounds");
		Dictionary<decimal, string> expectations = new()
		{
			[3.0M] = "¾d",
			[6.0M] = "1½d",
			[44.0M] = "11d",
			[48.0M] = "1/–",
			[120.0M] = "2/6",
			[839.0M] = "17/5¾",
			[1440.0M] = "£1/10/–",
			[1919.0M] = "£1/19/11¾",
			[1920.0M] = "£2/–/–"
		};

		foreach (CurrencyDescriptionPatternType type in new[]
		         {
			         CurrencyDescriptionPatternType.Short,
			         CurrencyDescriptionPatternType.ShortDecimal,
			         CurrencyDescriptionPatternType.Long
		         })
		{
			foreach ((decimal amount, string expected) in expectations)
			{
				Assert.AreEqual(expected, currency.Describe(amount, type), $"{type} failed for {amount}.");
			}
		}
	}

	[TestMethod]
	public void SeedData_PoundsPatterns_SeedTieredCompactFormatsAndSterlingGlyphSpecials()
	{
		using FuturemudDatabaseContext context = BuildContext();
		new CurrencySeeder().SeedData(context, new Dictionary<string, string>
		{
			["currency"] = "pounds"
		});

		ModelCurrency currency = context.Currencies
			.Include(x => x.CurrencyDescriptionPatterns)
				.ThenInclude(x => x.CurrencyDescriptionPatternElements)
					.ThenInclude(x => x.CurrencyDescriptionPatternElementSpecialValues)
			.Include(x => x.CurrencyDescriptionPatterns)
				.ThenInclude(x => x.FutureProg)
			.Single(x => x.Name == "Pounds");

		foreach (int type in new[]
		         {
			         (int)CurrencyDescriptionPatternType.Short,
			         (int)CurrencyDescriptionPatternType.ShortDecimal,
			         (int)CurrencyDescriptionPatternType.Long
		         })
		{
			List<MudSharp.Models.CurrencyDescriptionPattern> patterns = currency.CurrencyDescriptionPatterns
				.Where(x => x.Type == type)
				.OrderBy(x => x.Order)
				.ToList();

			CollectionAssert.AreEqual(new[] { 10, 20, 30 }, patterns.Select(x => x.Order).ToArray());
			Assert.AreEqual("IsLessThanFortyEight", patterns[0].FutureProg?.FunctionName);
			Assert.AreEqual("IsLessThanNineHundredSixty", patterns[1].FutureProg?.FunctionName);
			Assert.IsNull(patterns[2].FutureProg);
		}

		MudSharp.Models.CurrencyDescriptionPattern shortPattern = currency.CurrencyDescriptionPatterns
			.Single(x => x.Type == (int)CurrencyDescriptionPatternType.Short && x.Order == 10);
		MudSharp.Models.CurrencyDescriptionPatternElement penceOnlyElement = shortPattern.CurrencyDescriptionPatternElements.Single();
		Dictionary<decimal, string> penceGlyphs = penceOnlyElement.CurrencyDescriptionPatternElementSpecialValues
			.ToDictionary(x => x.Value, x => x.Text);
		Assert.AreEqual("¾d", penceGlyphs[0.75M]);
		Assert.AreEqual("1½d", penceGlyphs[1.5M]);

		MudSharp.Models.CurrencyDescriptionPattern slashPattern = currency.CurrencyDescriptionPatterns
			.Single(x => x.Type == (int)CurrencyDescriptionPatternType.Short && x.Order == 20);
		MudSharp.Models.CurrencyDescriptionPatternElement slashPenceElement = slashPattern.CurrencyDescriptionPatternElements
			.Single(x => x.Order == 2);
		Dictionary<decimal, string> slashGlyphs = slashPenceElement.CurrencyDescriptionPatternElementSpecialValues
			.ToDictionary(x => x.Value, x => x.Text);
		Assert.AreEqual("–", slashGlyphs[0.0M]);
		Assert.AreEqual("5¾", slashGlyphs[5.75M]);

		MudSharp.Models.CurrencyDescriptionPattern wordyPattern = currency.CurrencyDescriptionPatterns
			.Single(x => x.Type == (int)CurrencyDescriptionPatternType.Wordy);
		MudSharp.Models.CurrencyDescriptionPatternElement wordyFarthingElement = wordyPattern.CurrencyDescriptionPatternElements
			.Single(x => x.Order == 4);
		Dictionary<decimal, string> wordyFarthings = wordyFarthingElement.CurrencyDescriptionPatternElementSpecialValues
			.ToDictionary(x => x.Value, x => x.Text);
		Assert.AreEqual("ha'penny", wordyFarthings[2.0M]);
		Assert.AreEqual("three farthings", wordyFarthings[3.0M]);
	}
}
