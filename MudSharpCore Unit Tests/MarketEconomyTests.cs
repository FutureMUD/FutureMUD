using Expression = ExpressionEngine.Expression;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Economy;
using MudSharp.Economy.Markets;
using MudSharp.Framework;
using MudSharp.FutureProg;
using MudSharp.TimeAndDate;
using MudSharp.TimeAndDate.Date;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;

#nullable enable
namespace MudSharp_Unit_Tests;

[TestClass]
public class MarketEconomyTests
{
	[TestMethod]
	public void PriceMultiplierForCategory_AddsFlatPricePressureToFormulaResult()
	{
		var category = CreateCategory(5L, "Staple Food");
		var influence = new Mock<IMarketInfluence>();
		influence.Setup(x => x.Applies(It.IsAny<IMarketCategory>(), It.IsAny<MudDateTime>()))
		         .Returns<IMarketCategory, MudDateTime>((target, _) => target == category.Object);
		influence.SetupGet(x => x.MarketImpacts).Returns(
		[
			new MarketImpact
			{
				MarketCategory = category.Object,
				SupplyImpact = 0.10,
				DemandImpact = 0.25,
				FlatPriceImpact = 0.15
			}
		]);
		influence.SetupGet(x => x.PopulationIncomeImpacts).Returns([]);

		var market = CreateMarket(new Expression("1.10"), influence.Object);

		Assert.AreEqual(0.15m, market.FlatPriceAdjustmentForCategory(category.Object));
		Assert.AreEqual(1.25m, market.PriceMultiplierForCategory(category.Object));
	}

	[TestMethod]
	public void EffectiveIncomeFactorForPopulation_StacksAdditiveAndMultiplicativeImpacts()
	{
		var population = new Mock<IMarketPopulation>();
		population.SetupGet(x => x.IncomeFactor).Returns(1.00m);

		var influenceOne = new Mock<IMarketInfluence>();
		influenceOne.Setup(x => x.Applies(It.IsAny<IMarketCategory>(), It.IsAny<MudDateTime>()))
		            .Returns<IMarketCategory, MudDateTime>((category, _) => category is null);
		influenceOne.SetupGet(x => x.MarketImpacts).Returns([]);
		influenceOne.SetupGet(x => x.PopulationIncomeImpacts).Returns(
		[
			new MarketPopulationIncomeImpact
			{
				MarketPopulation = population.Object,
				AdditiveIncomeImpact = -0.10m,
				MultiplicativeIncomeImpact = 0.90m
			}
		]);

		var influenceTwo = new Mock<IMarketInfluence>();
		influenceTwo.Setup(x => x.Applies(It.IsAny<IMarketCategory>(), It.IsAny<MudDateTime>()))
		            .Returns<IMarketCategory, MudDateTime>((category, _) => category is null);
		influenceTwo.SetupGet(x => x.MarketImpacts).Returns([]);
		influenceTwo.SetupGet(x => x.PopulationIncomeImpacts).Returns(
		[
			new MarketPopulationIncomeImpact
			{
				MarketPopulation = population.Object,
				AdditiveIncomeImpact = 0.05m,
				MultiplicativeIncomeImpact = 1.10m
			}
		]);

		var market = CreateMarket(new Expression("1.0"), influenceOne.Object, influenceTwo.Object);

		Assert.AreEqual(0.9405m, decimal.Round(market.EffectiveIncomeFactorForPopulation(population.Object), 4));
	}

	[TestMethod]
	public void MarketPopulationHeartbeat_AddsSavingsWhenIncomeExceedsCosts()
	{
		var category = CreateCategory(1L, "Staple Food");
		var market = new Mock<IMarket>();
		market.Setup(x => x.EffectiveIncomeFactorForPopulation(It.IsAny<IMarketPopulation>())).Returns(1.20m);
		market.Setup(x => x.PriceMultiplierForCategory(category.Object)).Returns(1.00m);

		var population = CreatePopulation(
			market.Object,
			incomeFactor: 1.00m,
			savings: 0.00m,
			savingsCap: 0.50m,
			new MarketPopulationNeed
			{
				MarketCategory = category.Object,
				BaseExpenditure = 100.0m
			});

		population.MarketPopulationHeartbeat();

		Assert.AreEqual(0.20m, population.Savings);
		Assert.AreEqual(0.00m, population.CurrentStress);
	}

	[TestMethod]
	public void MarketPopulationHeartbeat_ConsumesSavingsBeforeApplyingStress()
	{
		var category = CreateCategory(1L, "Staple Food");
		var market = new Mock<IMarket>();
		market.Setup(x => x.EffectiveIncomeFactorForPopulation(It.IsAny<IMarketPopulation>())).Returns(0.90m);
		market.Setup(x => x.PriceMultiplierForCategory(category.Object)).Returns(1.10m);

		var population = CreatePopulation(
			market.Object,
			incomeFactor: 1.00m,
			savings: 0.30m,
			savingsCap: 0.50m,
			new MarketPopulationNeed
			{
				MarketCategory = category.Object,
				BaseExpenditure = 100.0m
			});

		population.MarketPopulationHeartbeat();

		Assert.AreEqual(0.10m, population.Savings);
		Assert.AreEqual(0.00m, population.CurrentStress);
	}

	[TestMethod]
	public void MarketPopulationHeartbeat_LeavesResidualStressAfterSavingsAreExhausted()
	{
		var category = CreateCategory(1L, "Staple Food");
		var market = new Mock<IMarket>();
		market.Setup(x => x.EffectiveIncomeFactorForPopulation(It.IsAny<IMarketPopulation>())).Returns(0.80m);
		market.Setup(x => x.PriceMultiplierForCategory(category.Object)).Returns(1.10m);

		var population = CreatePopulation(
			market.Object,
			incomeFactor: 1.00m,
			savings: 0.10m,
			savingsCap: 0.50m,
			new MarketPopulationNeed
			{
				MarketCategory = category.Object,
				BaseExpenditure = 100.0m
			});

		population.MarketPopulationHeartbeat();

		Assert.AreEqual(0.00m, population.Savings);
		Assert.AreEqual(0.20m, population.CurrentStress);
	}

	[TestMethod]
	public void MarketInfluenceTemplate_LoadDefaultsMissingPriceAndResolvesPopulationImpactsLater()
	{
		var category = CreateCategory(5L, "Staple Food");
		var populationLookup = new Dictionary<long, IMarketPopulation>();
		var marketCategories = new Mock<IUneditableAll<IMarketCategory>>();
		marketCategories.Setup(x => x.Get(5L)).Returns(category.Object);
		var marketPopulations = new Mock<IUneditableAll<IMarketPopulation>>();
		marketPopulations.Setup(x => x.Get(It.IsAny<long>()))
		                 .Returns<long>(id => populationLookup.TryGetValue(id, out var population) ? population : null);
		var futureProgs = new Mock<IUneditableAll<IFutureProg>>();
		futureProgs.Setup(x => x.Get(It.IsAny<long>())).Returns((IFutureProg)null);
		var alwaysTrueProg = new Mock<IFutureProg>();
		var gameworld = new Mock<IFuturemud>();
		gameworld.SetupGet(x => x.MarketCategories).Returns(marketCategories.Object);
		gameworld.SetupGet(x => x.MarketPopulations).Returns(marketPopulations.Object);
		gameworld.SetupGet(x => x.FutureProgs).Returns(futureProgs.Object);
		gameworld.SetupGet(x => x.AlwaysTrueProg).Returns(alwaysTrueProg.Object);

		var template = new MarketInfluenceTemplate(gameworld.Object, new MudSharp.Models.MarketInfluenceTemplate
		{
			Id = 1L,
			Name = "Deferred Rural Shock",
			Description = "desc",
			TemplateSummary = "summary",
			CharacterKnowsAboutInfluenceProgId = 1L,
			Impacts = "<Impacts><Impact category=\"5\" supply=\"0.05\" demand=\"0.10\" /></Impacts>",
			PopulationImpacts =
				"<PopulationImpacts><PopulationImpact population=\"9\" additive=\"-0.10\" multiplier=\"0.90\" /></PopulationImpacts>"
		});

		Assert.AreEqual(0.0, template.MarketImpacts.Single().FlatPriceImpact, 0.0001);
		Assert.AreEqual(0, template.PopulationIncomeImpacts.Count());

		var population = new Mock<IMarketPopulation>();
		population.SetupGet(x => x.Id).Returns(9L);
		population.SetupGet(x => x.Name).Returns("Rural Smallholders");
		populationLookup[9L] = population.Object;

		template.ResolvePopulationImpacts();
		var populationImpact = template.PopulationIncomeImpacts.Single();

		Assert.AreSame(population.Object, populationImpact.MarketPopulation);
		Assert.AreEqual(-0.10m, populationImpact.AdditiveIncomeImpact);
		Assert.AreEqual(0.90m, populationImpact.MultiplicativeIncomeImpact);
	}

	private static Market CreateMarket(Expression formula, params IMarketInfluence[] influences)
	{
		var market = (Market)RuntimeHelpers.GetUninitializedObject(typeof(Market));
		var calendar = new Mock<ICalendar>();
		calendar.SetupGet(x => x.CurrentDateTime).Returns(MudDateTime.Never);
		var economicZone = new Mock<IEconomicZone>();
		economicZone.SetupGet(x => x.FinancialPeriodReferenceCalendar).Returns(calendar.Object);

		market.EconomicZone = economicZone.Object;
		SetAutoProperty(market, nameof(Market.MarketPriceFormula), formula);
		SetField(market, "_marketInfluences", influences.ToList());
		SetField(market, "_marketCategories", new List<IMarketCategory>());
		SetField(market, "_noSave", true);
		return market;
	}

	private static MarketPopulation CreatePopulation(
		IMarket market,
		decimal incomeFactor,
		decimal savings,
		decimal savingsCap,
		params MarketPopulationNeed[] needs)
	{
		var population = (MarketPopulation)RuntimeHelpers.GetUninitializedObject(typeof(MarketPopulation));
		population.Market = market;
		population.IncomeFactor = incomeFactor;
		population.SavingsCap = savingsCap;
		SetAutoProperty(population, nameof(MarketPopulation.Savings), savings);
		SetField(population, "_marketPopulationNeeds", needs.ToList());
		SetField(population, "_marketStressPoints", new List<MarketStressPoint>());
		SetField(population, "_noSave", true);
		return population;
	}

	private static Mock<IMarketCategory> CreateCategory(long id, string name)
	{
		var category = new Mock<IMarketCategory>();
		category.SetupGet(x => x.Id).Returns(id);
		category.SetupGet(x => x.Name).Returns(name);
		category.SetupGet(x => x.ElasticityFactorAbove).Returns(0.25);
		category.SetupGet(x => x.ElasticityFactorBelow).Returns(0.10);
		return category;
	}

	private static void SetField(object target, string fieldName, object value)
	{
		var field = FindField(target.GetType(), fieldName);
		Assert.IsNotNull(field, $"Expected field {fieldName} to exist on {target.GetType().Name}.");
		field!.SetValue(target, value);
	}

	private static void SetAutoProperty(object target, string propertyName, object value)
	{
		var field = FindField(target.GetType(), $"<{propertyName}>k__BackingField");
		Assert.IsNotNull(field, $"Expected backing field for {propertyName} to exist on {target.GetType().Name}.");
		field!.SetValue(target, value);
	}

	private static FieldInfo? FindField(Type type, string fieldName)
	{
		for (Type? current = type; current is not null; current = current.BaseType)
		{
			FieldInfo? field = current.GetField(fieldName, BindingFlags.Instance | BindingFlags.NonPublic);
			if (field is not null)
			{
				return field;
			}
		}

		return null;
	}
}
