using Expression = ExpressionEngine.Expression;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Accounts;
using MudSharp.Character;
using MudSharp.Economy;
using MudSharp.Economy.Markets;
using MudSharp.Framework;
using MudSharp.FutureProg;
using MudSharp.GameItems;
using MudSharp.TimeAndDate;
using MudSharp.TimeAndDate.Date;
using System;
using System.Collections.Generic;
using System.Globalization;
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
		var influence = CreateApplicableInfluence(
			new MarketImpact
			{
				MarketCategory = category.Object,
				SupplyImpact = 0.10,
				DemandImpact = 0.25,
				FlatPriceImpact = 0.15
			});

		var market = CreateMarket(new Expression("1.10"), influences: influence.Object);

		Assert.AreEqual(0.15m, market.FlatPriceAdjustmentForCategory(category.Object));
		Assert.AreEqual(1.25m, market.PriceMultiplierForCategory(category.Object));
	}

	[TestMethod]
	public void CombinationCategory_PricesAsWeightedAverageOfConstituentCategories()
	{
		var wheat = CreateCategory(1L, "Wheat");
		var oliveOil = CreateCategory(2L, "Olive Oil");
		var stapleFood = CreateCategory(3L, "Staple Food", MarketCategoryType.Combination,
			[
				new MarketCategoryComponent
				{
					MarketCategory = wheat.Object,
					Weight = 8.0m
				},
				new MarketCategoryComponent
				{
					MarketCategory = oliveOil.Object,
					Weight = 2.0m
				}
			]);
		var wheatInfluence = CreateApplicableInfluence(
			new MarketImpact
			{
				MarketCategory = wheat.Object,
				SupplyImpact = 0.0,
				DemandImpact = 0.0,
				FlatPriceImpact = 0.10
			});
		var oliveInfluence = CreateApplicableInfluence(
			new MarketImpact
			{
				MarketCategory = oliveOil.Object,
				SupplyImpact = 0.0,
				DemandImpact = 0.0,
				FlatPriceImpact = 0.20
			});

		var market = CreateMarket(new Expression("1.0"),
			[wheat.Object, oliveOil.Object, stapleFood.Object],
			wheatInfluence.Object,
			oliveInfluence.Object);

		Assert.AreEqual(1.10m, market.PriceMultiplierForCategory(wheat.Object));
		Assert.AreEqual(1.20m, market.PriceMultiplierForCategory(oliveOil.Object));
		Assert.AreEqual(0.12m, decimal.Round(market.FlatPriceAdjustmentForCategory(stapleFood.Object), 2));
		Assert.AreEqual(1.12m, decimal.Round(market.PriceMultiplierForCategory(stapleFood.Object), 2));
	}

	[TestMethod]
	public void NestedCombinationCategories_ResolveToNormalizedLeafWeights()
	{
		var wheat = CreateCategory(1L, "Wheat");
		var oliveOil = CreateCategory(2L, "Olive Oil");
		var stapleFood = CreateCategory(3L, "Staple Food", MarketCategoryType.Combination,
			[
				new MarketCategoryComponent
				{
					MarketCategory = wheat.Object,
					Weight = 8.0m
				},
				new MarketCategoryComponent
				{
					MarketCategory = oliveOil.Object,
					Weight = 2.0m
				}
			]);
		var feastFoods = CreateCategory(4L, "Feast Foods", MarketCategoryType.Combination,
			[
				new MarketCategoryComponent
				{
					MarketCategory = stapleFood.Object,
					Weight = 5.0m
				},
				new MarketCategoryComponent
				{
					MarketCategory = oliveOil.Object,
					Weight = 5.0m
				}
			]);
		var feastInfluence = CreateApplicableInfluence(
			new MarketImpact
			{
				MarketCategory = feastFoods.Object,
				SupplyImpact = 0.0,
				DemandImpact = 0.0,
				FlatPriceImpact = 0.10
			});

		var market = CreateMarket(new Expression("1.0"),
			[wheat.Object, oliveOil.Object, stapleFood.Object, feastFoods.Object],
			feastInfluence.Object);

		Assert.AreEqual(0.04m, decimal.Round(market.FlatPriceAdjustmentForCategory(wheat.Object), 2));
		Assert.AreEqual(0.06m, decimal.Round(market.FlatPriceAdjustmentForCategory(oliveOil.Object), 2));
		Assert.AreEqual(1.044m, decimal.Round(market.PriceMultiplierForCategory(stapleFood.Object), 3));
		Assert.AreEqual(1.052m, decimal.Round(market.PriceMultiplierForCategory(feastFoods.Object), 3));
	}

	[TestMethod]
	public void ComboTargetedInfluence_RedistributesSupplyDemandAndFlatPriceToConstituents()
	{
		var wheat = CreateCategory(1L, "Wheat");
		var oliveOil = CreateCategory(2L, "Olive Oil");
		var stapleFood = CreateCategory(3L, "Staple Food", MarketCategoryType.Combination,
			[
				new MarketCategoryComponent
				{
					MarketCategory = wheat.Object,
					Weight = 8.0m
				},
				new MarketCategoryComponent
				{
					MarketCategory = oliveOil.Object,
					Weight = 2.0m
				}
			]);
		var influence = CreateApplicableInfluence(
			new MarketImpact
			{
				MarketCategory = stapleFood.Object,
				SupplyImpact = 0.50,
				DemandImpact = 0.25,
				FlatPriceImpact = 0.10
			});

		var market = CreateMarket(new Expression("1.0"),
			[wheat.Object, oliveOil.Object, stapleFood.Object],
			influence.Object);

		Assert.AreEqual(1.40, market.NetSupply(wheat.Object), 0.0001);
		Assert.AreEqual(1.20, market.NetDemand(wheat.Object), 0.0001);
		Assert.AreEqual(0.08m, decimal.Round(market.FlatPriceAdjustmentForCategory(wheat.Object), 2));
		Assert.AreEqual(1.08m, decimal.Round(market.PriceMultiplierForCategory(wheat.Object), 2));

		Assert.AreEqual(1.10, market.NetSupply(oliveOil.Object), 0.0001);
		Assert.AreEqual(1.05, market.NetDemand(oliveOil.Object), 0.0001);
		Assert.AreEqual(0.02m, decimal.Round(market.FlatPriceAdjustmentForCategory(oliveOil.Object), 2));
		Assert.AreEqual(1.02m, decimal.Round(market.PriceMultiplierForCategory(oliveOil.Object), 2));

		Assert.AreEqual(1.34, market.NetSupply(stapleFood.Object), 0.0001);
		Assert.AreEqual(1.17, market.NetDemand(stapleFood.Object), 0.0001);
		Assert.AreEqual(0.068m, decimal.Round(market.FlatPriceAdjustmentForCategory(stapleFood.Object), 3));
		Assert.AreEqual(1.068m, decimal.Round(market.PriceMultiplierForCategory(stapleFood.Object), 3));
	}

	[TestMethod]
	public void ExpandImpactToLeafCategories_NestedCombinationTarget_UsesNormalizedLeafEffects()
	{
		var wheat = CreateCategory(1L, "Wheat");
		var oliveOil = CreateCategory(2L, "Olive Oil");
		var stapleFood = CreateCategory(3L, "Staple Food", MarketCategoryType.Combination,
			[
				new MarketCategoryComponent
				{
					MarketCategory = wheat.Object,
					Weight = 8.0m
				},
				new MarketCategoryComponent
				{
					MarketCategory = oliveOil.Object,
					Weight = 2.0m
				}
			]);
		var feastFoods = CreateCategory(4L, "Feast Foods", MarketCategoryType.Combination,
			[
				new MarketCategoryComponent
				{
					MarketCategory = stapleFood.Object,
					Weight = 5.0m
				},
				new MarketCategoryComponent
				{
					MarketCategory = oliveOil.Object,
					Weight = 5.0m
				}
			]);

		var expanded = MarketImpactExpansion.ExpandImpactToLeafCategories(new MarketImpact
		{
			MarketCategory = feastFoods.Object,
			SupplyImpact = 0.10,
			DemandImpact = 0.20,
			FlatPriceImpact = 0.05
		})
			.OrderBy(x => x.LeafCategory.Id)
			.ToList();

		Assert.AreEqual(2, expanded.Count);
		Assert.AreSame(feastFoods.Object, expanded[0].SourceCategory);
		Assert.AreSame(wheat.Object, expanded[0].LeafCategory);
		Assert.AreEqual(0.40m, expanded[0].NormalizedWeight);
		Assert.AreEqual(0.04, expanded[0].SupplyImpact, 0.0001);
		Assert.AreEqual(0.08, expanded[0].DemandImpact, 0.0001);
		Assert.AreEqual(0.02, expanded[0].FlatPriceImpact, 0.0001);
		Assert.AreSame(feastFoods.Object, expanded[1].SourceCategory);
		Assert.AreSame(oliveOil.Object, expanded[1].LeafCategory);
		Assert.AreEqual(0.60m, expanded[1].NormalizedWeight);
		Assert.AreEqual(0.06, expanded[1].SupplyImpact, 0.0001);
		Assert.AreEqual(0.12, expanded[1].DemandImpact, 0.0001);
		Assert.AreEqual(0.03, expanded[1].FlatPriceImpact, 0.0001);
	}

	[TestMethod]
	public void MarketInfluenceShow_CombinationImpact_AddsLeafExpansionPreview()
	{
		var actor = CreateFormattingActor();
		var wheat = CreateCategory(1L, "Wheat");
		var oliveOil = CreateCategory(2L, "Olive Oil");
		var stapleFood = CreateCategory(3L, "Staple Food", MarketCategoryType.Combination,
			[
				new MarketCategoryComponent
				{
					MarketCategory = wheat.Object,
					Weight = 8.0m
				},
				new MarketCategoryComponent
				{
					MarketCategory = oliveOil.Object,
					Weight = 2.0m
				}
			]);
		var market = new Mock<IMarket>();
		market.SetupGet(x => x.Id).Returns(7L);
		market.SetupGet(x => x.Name).Returns("Riverlands Exchange");

		var influence = (MarketInfluence)RuntimeHelpers.GetUninitializedObject(typeof(MarketInfluence));
		SetField(influence, "_id", 9L);
		SetField(influence, "_name", "Harvest Failure");
		SetAutoProperty(influence, nameof(MarketInfluence.Market), market.Object);
		SetAutoProperty(influence, nameof(MarketInfluence.MarketInfluenceTemplate), null!);
		SetAutoProperty(influence, nameof(MarketInfluence.Description), "desc");
		SetAutoProperty(influence, nameof(MarketInfluence.AppliesFrom), MudDateTime.Never);
		SetField(influence, "_appliesUntil", null!);
		SetField(influence, "_marketImpacts",
			new List<MarketImpact>
			{
				new()
				{
					MarketCategory = stapleFood.Object,
					SupplyImpact = 0.50,
					DemandImpact = 0.25,
					FlatPriceImpact = 0.10
				}
			});
		SetField(influence, "_populationIncomeImpacts", new List<MarketPopulationIncomeImpact>());
		SetAutoProperty(influence, nameof(MarketInfluence.CharacterKnowsAboutInfluenceProg), null!);

		var show = influence.Show(actor.Object).StripANSIColour();

		StringAssert.Contains(show, "Leaf Expansion Preview:");
		StringAssert.Contains(show, "Staple Food");
		StringAssert.Contains(show, "Wheat");
		StringAssert.Contains(show, "Olive Oil");
		StringAssert.Contains(show, "80.00 %");
		StringAssert.Contains(show, "20.00 %");
		StringAssert.Contains(show, "+40.00%");
		StringAssert.Contains(show, "+20.00%");
		StringAssert.Contains(show, "+8.00%");
		StringAssert.Contains(show, "+10.00%");
		StringAssert.Contains(show, "+5.00%");
		StringAssert.Contains(show, "+2.00%");
	}

	[TestMethod]
	public void CombinationCategories_ParticipateInItemPricingAlongsideStandaloneCategories()
	{
		var comboOnlyProto = new Mock<IGameItemProto>();
		var sharedProto = new Mock<IGameItemProto>();
		var wheat = CreateCategory(1L, "Wheat");
		var oliveOil = CreateCategory(2L, "Olive Oil");
		var stapleFood = CreateCategory(3L, "Staple Food", MarketCategoryType.Combination,
			[
				new MarketCategoryComponent
				{
					MarketCategory = wheat.Object,
					Weight = 8.0m
				},
				new MarketCategoryComponent
				{
					MarketCategory = oliveOil.Object,
					Weight = 2.0m
				}
			],
			belongsToProto: proto => ReferenceEquals(proto, comboOnlyProto.Object) || ReferenceEquals(proto, sharedProto.Object));
		var luxury = CreateCategory(4L, "Luxury Goods",
			belongsToProto: proto => ReferenceEquals(proto, sharedProto.Object));
		var influences =
			new[]
			{
				CreateApplicableInfluence(
					new MarketImpact
					{
						MarketCategory = wheat.Object,
						SupplyImpact = 0.0,
						DemandImpact = 0.0,
						FlatPriceImpact = 0.10
					}),
				CreateApplicableInfluence(
					new MarketImpact
					{
						MarketCategory = oliveOil.Object,
						SupplyImpact = 0.0,
						DemandImpact = 0.0,
						FlatPriceImpact = 0.20
					}),
				CreateApplicableInfluence(
					new MarketImpact
					{
						MarketCategory = luxury.Object,
						SupplyImpact = 0.0,
						DemandImpact = 0.0,
						FlatPriceImpact = 0.30
					})
			};

		var market = CreateMarket(new Expression("1.0"),
			[wheat.Object, oliveOil.Object, stapleFood.Object, luxury.Object],
			influences.Select(x => x.Object).ToArray());

		Assert.AreEqual(1.12m, decimal.Round(market.PriceMultiplierForItem(comboOnlyProto.Object), 2));
		Assert.AreEqual(0.12m, decimal.Round(market.FlatPriceAdjustmentForItem(comboOnlyProto.Object), 2));
		Assert.AreEqual(1.30m, decimal.Round(market.PriceMultiplierForItem(sharedProto.Object), 2));
		Assert.AreEqual(0.30m, decimal.Round(market.FlatPriceAdjustmentForItem(sharedProto.Object), 2));
	}

	[TestMethod]
	public void EffectiveIncomeFactorForPopulation_StacksAdditiveAndMultiplicativeImpacts()
	{
		var population = new Mock<IMarketPopulation>();
		population.SetupGet(x => x.IncomeFactor).Returns(1.00m);

		var influenceOne = new Mock<IMarketInfluence>();
		influenceOne.Setup(x => x.Applies(It.IsAny<IMarketCategory>(), It.IsAny<MudDateTime>()))
		            .Returns(true);
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
		            .Returns(true);
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
			stressFlickerThreshold: 0.01m,
			needs:
			[
				new MarketPopulationNeed
				{
					MarketCategory = category.Object,
					BaseExpenditure = 100.0m
				}
			]);

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
			stressFlickerThreshold: 0.01m,
			needs:
			[
				new MarketPopulationNeed
				{
					MarketCategory = category.Object,
					BaseExpenditure = 100.0m
				}
			]);

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
			stressFlickerThreshold: 0.01m,
			needs:
			[
				new MarketPopulationNeed
				{
					MarketCategory = category.Object,
					BaseExpenditure = 100.0m
				}
			]);

		population.MarketPopulationHeartbeat();

		Assert.AreEqual(0.00m, population.Savings);
		Assert.AreEqual(0.20m, population.CurrentStress);
	}

	[TestMethod]
	public void MarketPopulationHeartbeat_UsesFlickerThresholdWhenStressFalls()
	{
		var category = CreateCategory(1L, "Staple Food");
		var incomeFactor = 0.90m;
		var priceMultiplier = 1.00m;
		var market = new Mock<IMarket>();
		market.Setup(x => x.EffectiveIncomeFactorForPopulation(It.IsAny<IMarketPopulation>()))
		      .Returns(() => incomeFactor);
		market.Setup(x => x.PriceMultiplierForCategory(category.Object))
		      .Returns(() => priceMultiplier);
		var mildStress = new MarketStressPoint
		{
			Name = "Mild",
			Description = "desc",
			StressThreshold = 0.10m,
			ExecuteOnStart = null,
			ExecuteOnEnd = null
		};
		var population = CreatePopulation(
			market.Object,
			incomeFactor: 1.00m,
			savings: 0.00m,
			savingsCap: 0.00m,
			stressFlickerThreshold: 0.01m,
			needs:
			[
				new MarketPopulationNeed
				{
					MarketCategory = category.Object,
					BaseExpenditure = 100.0m
				}
			],
			stressPoints:
			[
				mildStress
			]);

		population.MarketPopulationHeartbeat();
		Assert.AreEqual(0.10m, population.CurrentStress);
		Assert.AreSame(mildStress, population.CurrentStressPoint);

		incomeFactor = 0.909m;
		population.MarketPopulationHeartbeat();
		Assert.AreEqual(0.091m, population.CurrentStress);
		Assert.AreSame(mildStress, population.CurrentStressPoint);

		incomeFactor = 0.911m;
		population.MarketPopulationHeartbeat();
		Assert.AreEqual(0.089m, population.CurrentStress);
		Assert.IsNull(population.CurrentStressPoint);
	}

	[TestMethod]
	public void MarketPopulationHeartbeat_PromotesAndDemotesAcrossMultipleThresholdsWithHysteresis()
	{
		var category = CreateCategory(1L, "Staple Food");
		var incomeFactor = 0.79m;
		var market = new Mock<IMarket>();
		market.Setup(x => x.EffectiveIncomeFactorForPopulation(It.IsAny<IMarketPopulation>()))
		      .Returns(() => incomeFactor);
		market.Setup(x => x.PriceMultiplierForCategory(category.Object)).Returns(1.00m);
		var mildStress = new MarketStressPoint
		{
			Name = "Mild",
			Description = "desc",
			StressThreshold = 0.10m,
			ExecuteOnStart = null,
			ExecuteOnEnd = null
		};
		var severeStress = new MarketStressPoint
		{
			Name = "Severe",
			Description = "desc",
			StressThreshold = 0.20m,
			ExecuteOnStart = null,
			ExecuteOnEnd = null
		};
		var population = CreatePopulation(
			market.Object,
			incomeFactor: 1.00m,
			savings: 0.00m,
			savingsCap: 0.00m,
			stressFlickerThreshold: 0.01m,
			needs:
			[
				new MarketPopulationNeed
				{
					MarketCategory = category.Object,
					BaseExpenditure = 100.0m
				}
			],
			stressPoints:
			[
				mildStress,
				severeStress
			]);

		population.MarketPopulationHeartbeat();
		Assert.AreEqual(0.21m, population.CurrentStress);
		Assert.AreSame(severeStress, population.CurrentStressPoint);

		incomeFactor = 0.805m;
		population.MarketPopulationHeartbeat();
		Assert.AreEqual(0.195m, population.CurrentStress);
		Assert.AreSame(severeStress, population.CurrentStressPoint);

		incomeFactor = 0.815m;
		population.MarketPopulationHeartbeat();
		Assert.AreEqual(0.185m, population.CurrentStress);
		Assert.AreSame(mildStress, population.CurrentStressPoint);

		incomeFactor = 0.905m;
		population.MarketPopulationHeartbeat();
		Assert.AreEqual(0.095m, population.CurrentStress);
		Assert.AreSame(mildStress, population.CurrentStressPoint);

		incomeFactor = 0.915m;
		population.MarketPopulationHeartbeat();
		Assert.AreEqual(0.085m, population.CurrentStress);
		Assert.IsNull(population.CurrentStressPoint);
	}

	[TestMethod]
	public void MarketPopulation_LoadDefaultsLegacyStressFlickerThreshold()
	{
		var market = new Mock<IMarket>();
		market.SetupGet(x => x.Id).Returns(7L);
		var markets = new Mock<IUneditableAll<IMarket>>();
		markets.Setup(x => x.Get(7L)).Returns(market.Object);
		var gameworld = new Mock<IFuturemud>();
		gameworld.SetupGet(x => x.Markets).Returns(markets.Object);

		var population = new MarketPopulation(gameworld.Object, new MudSharp.Models.MarketPopulation
		{
			Id = 1L,
			Name = "Legacy Population",
			MarketId = 7L,
			Description = "desc",
			PopulationScale = 1000,
			IncomeFactor = 1.0m,
			Savings = 0.0m,
			SavingsCap = 0.0m,
			StressFlickerThreshold = 0.0m,
			MarketPopulationNeeds = "<Needs />",
			MarketStressPoints = "<Stresses />"
		});

		Assert.AreEqual(0.01m, population.StressFlickerThreshold);
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
		futureProgs.Setup(x => x.Get(It.IsAny<long>())).Returns((IFutureProg)null!);
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

	private static Mock<IMarketInfluence> CreateApplicableInfluence(params MarketImpact[] impacts)
	{
		var influence = new Mock<IMarketInfluence>();
		influence.Setup(x => x.Applies(It.IsAny<IMarketCategory>(), It.IsAny<MudDateTime>()))
		         .Returns(true);
		influence.SetupGet(x => x.MarketImpacts).Returns(impacts);
		influence.SetupGet(x => x.PopulationIncomeImpacts).Returns([]);
		return influence;
	}

	private static Market CreateMarket(Expression formula, params IMarketInfluence[] influences)
	{
		return CreateMarket(formula, [], influences);
	}

	private static Market CreateMarket(Expression formula, IEnumerable<IMarketCategory> categories,
		params IMarketInfluence[] influences)
	{
		var market = (Market)RuntimeHelpers.GetUninitializedObject(typeof(Market));
		var calendar = new Mock<ICalendar>();
		calendar.SetupGet(x => x.CurrentDateTime).Returns(MudDateTime.Never);
		var economicZone = new Mock<IEconomicZone>();
		economicZone.SetupGet(x => x.FinancialPeriodReferenceCalendar).Returns(calendar.Object);

		market.EconomicZone = economicZone.Object;
		SetAutoProperty(market, nameof(Market.MarketPriceFormula), formula);
		SetField(market, "_marketInfluences", influences.ToList());
		SetField(market, "_marketCategories", categories.ToList());
		InitialiseField(market, "_categoryPricingCache");
		InitialiseField(market, "_expandedImpactLookup");
		SetField(market, "_pricingCacheDirty", true);
		SetField(market, "_pricingCacheLastUpdatedUtc", null!);
		SetField(market, "_rebuildingPricingCache", false);
		SetField(market, "_noSave", true);
		return market;
	}

	private static MarketPopulation CreatePopulation(
		IMarket market,
		decimal incomeFactor,
		decimal savings,
		decimal savingsCap,
		decimal stressFlickerThreshold,
		IEnumerable<MarketPopulationNeed> needs,
		IEnumerable<MarketStressPoint>? stressPoints = null)
	{
		var population = (MarketPopulation)RuntimeHelpers.GetUninitializedObject(typeof(MarketPopulation));
		population.Market = market;
		population.IncomeFactor = incomeFactor;
		population.SavingsCap = savingsCap;
		population.StressFlickerThreshold = stressFlickerThreshold;
		SetAutoProperty(population, nameof(MarketPopulation.Savings), savings);
		SetField(population, "_marketPopulationNeeds", needs.ToList());
		SetField(population, "_marketStressPoints", (stressPoints ?? []).OrderBy(x => x.StressThreshold).ToList());
		SetField(population, "_noSave", true);
		return population;
	}

	private static Mock<IMarketCategory> CreateCategory(long id, string name,
		MarketCategoryType categoryType = MarketCategoryType.Standalone,
		IEnumerable<MarketCategoryComponent>? combinationComponents = null,
		Func<IGameItem, bool>? belongsToItem = null,
		Func<IGameItemProto, bool>? belongsToProto = null)
	{
		var components = combinationComponents?.ToList() ?? [];
		var category = new Mock<IMarketCategory>();
		category.SetupGet(x => x.Id).Returns(id);
		category.SetupGet(x => x.Name).Returns(name);
		category.SetupGet(x => x.ElasticityFactorAbove).Returns(0.25);
		category.SetupGet(x => x.ElasticityFactorBelow).Returns(0.10);
		category.SetupGet(x => x.CategoryType).Returns(categoryType);
		category.SetupGet(x => x.CombinationComponents).Returns(components);
		category.Setup(x => x.BelongsToCategory(It.IsAny<IGameItem>()))
		        .Returns<IGameItem>(item => belongsToItem?.Invoke(item) ?? false);
		category.Setup(x => x.BelongsToCategory(It.IsAny<IGameItemProto>()))
		        .Returns<IGameItemProto>(proto => belongsToProto?.Invoke(proto) ?? false);
		return category;
	}

	private static Mock<ICharacter> CreateFormattingActor()
	{
		var account = new Mock<IAccount>();
		account.SetupGet(x => x.Culture).Returns(CultureInfo.InvariantCulture);
		account.SetupGet(x => x.LineFormatLength).Returns(160);
		account.SetupGet(x => x.InnerLineFormatLength).Returns(120);
		account.SetupGet(x => x.UseUnicode).Returns(false);

		var actor = new Mock<ICharacter>();
		actor.SetupGet(x => x.Account).Returns(account.Object);
		actor.SetupGet(x => x.LineFormatLength).Returns(160);
		actor.SetupGet(x => x.InnerLineFormatLength).Returns(120);
		actor.Setup(x => x.GetFormat(It.IsAny<Type>()))
			.Returns<Type>(type => CultureInfo.InvariantCulture.GetFormat(type));
		return actor;
	}

	private static void InitialiseField(object target, string fieldName)
	{
		var field = FindField(target.GetType(), fieldName);
		Assert.IsNotNull(field, $"Expected field {fieldName} to exist on {target.GetType().Name}.");
		var value = Activator.CreateInstance(field!.FieldType);
		field.SetValue(target, value);
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
