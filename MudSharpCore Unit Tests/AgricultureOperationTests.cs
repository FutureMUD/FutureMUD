using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Climate;
using MudSharp.Construction;
using MudSharp.Framework;
using MudSharp.Framework.Save;
using MudSharp.Work.Agriculture;
using MudSharp.Economy.Property;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

#nullable enable

namespace MudSharp_Unit_Tests;

[TestClass]
public class AgricultureOperationTests
{
	[TestMethod]
	public void CustomScoreTypes_DisabledSlotsDoNotAppearAsActiveScores()
	{
		var gameworld = BuildGameworldWithCustomScore(enabled: false);

		var scores = AgricultureScoreTypeExtensions.ActiveScoreTypes(gameworld.Object).ToList();

		Assert.IsFalse(scores.Contains(AgricultureScoreType.Custom1));
		Assert.IsTrue(scores.Contains(AgricultureScoreType.Moisture));
	}

	[TestMethod]
	public void CustomScoreTypes_EnabledSlotsParseBySlotAndConfiguredName()
	{
		var gameworld = BuildGameworldWithCustomScore(enabled: true, name: "Mana Saturation");

		Assert.IsTrue(AgricultureScoreTypeExtensions.TryParseScoreType("custom1", gameworld.Object, out var slot));
		Assert.AreEqual(AgricultureScoreType.Custom1, slot);
		Assert.IsTrue(AgricultureScoreTypeExtensions.TryParseScoreType("Mana Saturation", gameworld.Object, out var named));
		Assert.AreEqual(AgricultureScoreType.Custom1, named);
		Assert.AreEqual("Mana Saturation", AgricultureScoreType.Custom1.DescribeFor(gameworld.Object));
	}

	[TestMethod]
	public void CustomScoreTypes_HarmfulDirectionMakesNegativeOperationDeltaBeneficial()
	{
		var gameworld = BuildFieldGameworld(enabled: true, higherIsGood: false);
		var neutralField = BuildFieldWithCustomScore(gameworld.Object, 50, AgricultureFieldUse.Fallow);
		var skilledField = BuildFieldWithCustomScore(gameworld.Object, 50, AgricultureFieldUse.Fallow);
		var operation = BuildOperation(AgricultureOperationType.Improve, AgricultureFieldUse.Fallow,
			AgricultureFieldUse.Fallow, scoreDeltas: [(AgricultureScoreType.Custom1, -10)], gameworldOverride: gameworld.Object);

		neutralField.ApplyOperation(operation, null, null!, false, AgricultureWorkOutcome.Neutral, out _);
		skilledField.ApplyOperation(operation, null, null!, false,
			AgricultureWorkOutcome.FromSkill(800.0, 10.0, 0.0, 0.0), out _);

		Assert.IsTrue(skilledField.Score(AgricultureScoreType.Custom1) < neutralField.Score(AgricultureScoreType.Custom1));
	}

	[TestMethod]
	public void CropDefinition_LoadsAndSavesCustomScoreRanges()
	{
		var crop = new AgricultureCropDefinition(new MudSharp.Models.AgricultureCropDefinition
		{
			Id = 1,
			Name = "Moonwheat",
			Description = "A test crop.",
			Category = "test",
			Definition = @"<Crop growthDays=""30"" harvestWindowDays=""5"" minMoisture=""10"" maxMoisture=""90"" minTemperature=""0"" maxTemperature=""40""><ScoreRanges><Score type=""Custom1"" min=""20"" max=""70"" /></ScoreRanges></Crop>"
		}, BuildGameworldWithCustomScore(enabled: true).Object);

		Assert.AreEqual(1, crop.ScoreRanges.Count);
		var range = crop.ScoreRanges.Single();
		Assert.AreEqual(AgricultureScoreType.Custom1, range.Score);
		Assert.AreEqual(20, range.Minimum);
		Assert.AreEqual(70, range.Maximum);
	}

	[TestMethod]
	public void CropTick_CustomScoreOutsideRangeStressesCrop()
	{
		var gameworld = BuildFieldGameworld(enabled: true);
		var field = BuildFieldWithCustomScore(gameworld.Object, 10, AgricultureFieldUse.Crop, withCrop: true);

		field.DailyTick();

		Assert.AreEqual(46, field.CropHealth);
	}

	[TestMethod]
	public void AgricultureWorkOutcome_HighFarmingSkillImprovesYieldAndQuality()
	{
		var outcome = AgricultureWorkOutcome.FromSkill(800.0, 10.0, 0.0, 0.0);

		Assert.IsTrue(outcome.CropYieldMultiplier > 1.0);
		Assert.IsTrue(outcome.SeedYieldMultiplier > outcome.CropYieldMultiplier);
		Assert.AreEqual(MudSharp.GameItems.ItemQuality.Great, outcome.OutputQuality);
		Assert.IsTrue(outcome.CropYieldDelta > 0);
	}

	[TestMethod]
	public void AgricultureWorkOutcome_SkilledSupervisorImprovesLowSkillLabour()
	{
		var unsupervised = AgricultureWorkOutcome.FromSkill(200.0, 10.0, 0.0, 0.0);
		var supervised = AgricultureWorkOutcome.FromSkill(200.0, 10.0, 160.0, 2.0);

		Assert.IsTrue(supervised.CropYieldMultiplier > unsupervised.CropYieldMultiplier);
		Assert.IsTrue(supervised.BeneficialScoreMultiplier > unsupervised.BeneficialScoreMultiplier);
	}

	[TestMethod]
	public void WhyCannotApply_ImproveOperationRequiresConfiguredFieldUse()
	{
		var operation = BuildOperation(AgricultureOperationType.Improve, AgricultureFieldUse.Woodland, AgricultureFieldUse.Woodland);
		var field = new Mock<IAgricultureField>();
		field.SetupGet(x => x.CurrentUse).Returns(AgricultureFieldUse.Crop);

		var reason = operation.WhyCannotApply(field.Object, null);

		StringAssert.Contains(reason, "woodland");
	}

	[TestMethod]
	public void WhyCannotApply_ImproveOperationAllowsMatchingFieldUse()
	{
		var operation = BuildOperation(AgricultureOperationType.Improve, AgricultureFieldUse.Woodland, AgricultureFieldUse.Woodland);
		var field = new Mock<IAgricultureField>();
		field.SetupGet(x => x.CurrentUse).Returns(AgricultureFieldUse.Woodland);

		var reason = operation.WhyCannotApply(field.Object, null);

		Assert.AreEqual(string.Empty, reason);
	}

	[TestMethod]
	public void WhyCannotApply_SowRejectsPerennialCropTarget()
	{
		var operation = BuildOperation(AgricultureOperationType.Sow, AgricultureFieldUse.Fallow, AgricultureFieldUse.Crop, AgricultureTargetType.Crop);
		var field = new Mock<IAgricultureField>();
		field.SetupGet(x => x.CurrentUse).Returns(AgricultureFieldUse.Fallow);
		var crop = new Mock<IAgricultureCropDefinition>();
		crop.SetupGet(x => x.IsPerennial).Returns(true);

		var reason = operation.WhyCannotApply(field.Object, crop.Object);

		StringAssert.Contains(reason, "Perennial");
	}

	[TestMethod]
	public void WhyCannotApply_PlantOrchardRejectsAnnualCropTarget()
	{
		var operation = BuildOperation(AgricultureOperationType.PlantOrchard, AgricultureFieldUse.Fallow,
			AgricultureFieldUse.Orchard, AgricultureTargetType.Crop);
		var field = new Mock<IAgricultureField>();
		field.SetupGet(x => x.CurrentUse).Returns(AgricultureFieldUse.Fallow);
		var crop = new Mock<IAgricultureCropDefinition>();
		crop.SetupGet(x => x.IsPerennial).Returns(false);

		var reason = operation.WhyCannotApply(field.Object, crop.Object);

		StringAssert.Contains(reason, "Annual");
	}

	private static AgricultureOperation BuildOperation(AgricultureOperationType type, AgricultureFieldUse requiredUse,
		AgricultureFieldUse resultUse, AgricultureTargetType targetType = AgricultureTargetType.None,
		IEnumerable<(AgricultureScoreType Score, int Delta)>? scoreDeltas = null,
		IFuturemud? gameworldOverride = null)
	{
		var model = new MudSharp.Models.AgricultureOperation
		{
			Id = 1,
			Name = "Test Operation",
			Description = "A test agriculture operation.",
			OperationType = (int)type,
			TargetType = (int)targetType,
			RequiredUse = (int)requiredUse,
			ResultUse = (int)resultUse,
			Definition = new XElement("Operation",
				scoreDeltas?.Select(x => new XElement("Score",
					new XAttribute("type", x.Score.ToString()),
					new XAttribute("value", x.Delta))) ?? Enumerable.Empty<XElement>()).ToString()
		};
		var gameworld = gameworldOverride ?? new Mock<IFuturemud>().Object;
		return new AgricultureOperation(model, gameworld);
	}

	private static Mock<IFuturemud> BuildGameworldWithCustomScore(bool enabled, bool higherIsGood = true,
		string name = "Mana Saturation")
	{
		var definitions = AgricultureScoreTypeExtensions.GetCustomScoreConfiguration(null).ToDictionary(x => x.Key, x => x.Value);
		definitions[AgricultureScoreType.Custom1] = new AgricultureCustomScoreDefinition(
			AgricultureScoreType.Custom1,
			enabled,
			name,
			higherIsGood);
		var text = AgricultureScoreTypeExtensions.SaveCustomScoreConfiguration(definitions).ToString();
		var gameworld = new Mock<IFuturemud>();
		gameworld
			.Setup(x => x.GetStaticConfiguration(AgricultureScoreTypeExtensions.CustomScoreConfigurationStaticConfiguration))
			.Returns(text);
		return gameworld;
	}

	private static Mock<IFuturemud> BuildFieldGameworld(bool enabled, bool higherIsGood = true)
	{
		var gameworld = BuildGameworldWithCustomScore(enabled, higherIsGood);
		var saveManager = new Mock<ISaveManager>();
		gameworld.SetupGet(x => x.SaveManager).Returns(saveManager.Object);
		var cells = new All<ICell>();
		var cell = new Mock<ICell>();
		cell.SetupGet(x => x.Id).Returns(1);
		cell.SetupGet(x => x.Name).Returns("Test Cell");
		cell.SetupGet(x => x.FrameworkItemType).Returns("Cell");
		cell.SetupGet(x => x.Gameworld).Returns(gameworld.Object);
		cell.Setup(x => x.CurrentTemperature(It.IsAny<IPerceiver>())).Returns(20.0);
		cell.Setup(x => x.CurrentWeather(It.IsAny<IPerceiver>())).Returns(default(IWeatherEvent)!);
		cells.Add(cell.Object);
		gameworld.SetupGet(x => x.Cells).Returns(cells);

		var profiles = new All<IAgricultureFieldProfile>();
		var profile = new Mock<IAgricultureFieldProfile>();
		profile.SetupGet(x => x.Id).Returns(1);
		profile.SetupGet(x => x.Name).Returns("Test Profile");
		profile.SetupGet(x => x.FrameworkItemType).Returns("AgricultureFieldProfile");
		profile.SetupGet(x => x.DefaultScores).Returns(new Dictionary<AgricultureScoreType, int>
		{
			[AgricultureScoreType.Custom1] = 50
		});
		profile.Setup(x => x.AllowsUse(It.IsAny<AgricultureFieldUse>())).Returns(true);
		profiles.Add(profile.Object);
		gameworld.SetupGet(x => x.AgricultureFieldProfiles).Returns(profiles);

		var crops = new All<IAgricultureCropDefinition>();
		var crop = new Mock<IAgricultureCropDefinition>();
		crop.SetupGet(x => x.Id).Returns(1);
		crop.SetupGet(x => x.Name).Returns("Test Crop");
		crop.SetupGet(x => x.FrameworkItemType).Returns("AgricultureCropDefinition");
		crop.SetupGet(x => x.BaseGrowthDays).Returns(30);
		crop.SetupGet(x => x.HarvestWindowDays).Returns(5);
		crop.SetupGet(x => x.MinimumMoisture).Returns(0);
		crop.SetupGet(x => x.MaximumMoisture).Returns(100);
		crop.SetupGet(x => x.MinimumTemperature).Returns(0);
		crop.SetupGet(x => x.MaximumTemperature).Returns(40);
		crop.SetupGet(x => x.HarvestCycleDays).Returns(30);
		crop.SetupGet(x => x.ScoreRanges).Returns(
		[
			new AgricultureScoreRange(AgricultureScoreType.Custom1, 30, 70)
		]);
		crops.Add(crop.Object);
		gameworld.SetupGet(x => x.AgricultureCropDefinitions).Returns(crops);

		gameworld.SetupGet(x => x.Properties).Returns(new All<IProperty>());
		return gameworld;
	}

	private static AgricultureField BuildFieldWithCustomScore(IFuturemud gameworld, int customScore,
		AgricultureFieldUse use, bool withCrop = false)
	{
		var model = new MudSharp.Models.AgricultureField
		{
			Id = 1,
			CellId = 1,
			ProfileId = 1,
			CurrentUse = (int)use,
			Moisture = 50,
			Drainage = 50,
			Nutrients = 50,
			Salinity = 0,
			Topsoil = 50,
			Tilth = 50,
			Rockiness = 0,
			Weeds = 0,
			Pests = 0,
			Fence = 50,
			Pasture = 50,
			Condition = 50,
			Definition = new XElement("Field",
				new XElement("CustomScores",
					new XElement("Score",
						new XAttribute("type", AgricultureScoreType.Custom1.ToString()),
						new XAttribute("value", customScore)))).ToString()
		};

		if (withCrop)
		{
			model.AgricultureFieldCrop = new MudSharp.Models.AgricultureFieldCrop
			{
				AgricultureFieldId = 1,
				CropDefinitionId = 1,
				Stage = (int)AgricultureCropStage.Growing,
				GrowthDays = 10,
				Health = 50,
				YieldPotential = 50,
				Definition = "<Crop />"
			};
		}

		return new AgricultureField(model, gameworld);
	}
}
