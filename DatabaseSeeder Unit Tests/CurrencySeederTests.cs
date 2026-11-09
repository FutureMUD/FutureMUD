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
}
