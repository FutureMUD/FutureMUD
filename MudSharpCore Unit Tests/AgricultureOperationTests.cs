using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Accounts;
using MudSharp.Body;
using MudSharp.Celestial;
using MudSharp.Character;
using MudSharp.Character.Heritage;
using MudSharp.Climate;
using MudSharp.Construction;
using MudSharp.Construction.Boundary;
using MudSharp.Framework;
using MudSharp.Framework.Save;
using MudSharp.Work.Agriculture;
using MudSharp.Work.Crafts;
using MudSharp.Work.Crafts.Inputs;
using MudSharp.Economy.Property;
using MudSharp.NPC.Templates;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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
	public void CropDefinition_LoadsAndSavesPlantingWindows()
	{
		var crop = new AgricultureCropDefinition(new MudSharp.Models.AgricultureCropDefinition
		{
			Id = 1,
			Name = "Moonwheat",
			Description = "A test crop.",
			Category = "test",
			Definition = @"<Crop growthDays=""30"" harvestWindowDays=""5"" minMoisture=""10"" maxMoisture=""90"" minTemperature=""0"" maxTemperature=""40""><PlantingWindows><Window type=""group"" value=""Spring"" /><Window type=""season"" value=""Early Spring"" /></PlantingWindows></Crop>"
		}, BuildGameworldWithCustomScore(enabled: true).Object);

		Assert.AreEqual(2, crop.PlantingWindows.Count);
		Assert.IsTrue(crop.PlantingWindows.Any(x => x.Type == AgriculturePlantingWindowType.Group && x.Value == "Spring"));
		Assert.IsTrue(crop.PlantingWindows.Any(x => x.Type == AgriculturePlantingWindowType.Season && x.Value == "Early Spring"));

		var saveMethod = typeof(AgricultureCropDefinition).GetMethod("SaveDefinition",
			BindingFlags.Instance | BindingFlags.NonPublic);
		var saved = (XElement)saveMethod!.Invoke(crop, null)!;
		var windows = saved.Element("PlantingWindows")!.Elements("Window").ToList();
		Assert.AreEqual(2, windows.Count);
		Assert.IsTrue(windows.Any(x => x.Attribute("type")!.Value == "group" && x.Attribute("value")!.Value == "Spring"));
		Assert.IsTrue(windows.Any(x => x.Attribute("type")!.Value == "season" && x.Attribute("value")!.Value == "Early Spring"));
	}

	[TestMethod]
	public void CropDefinition_LoadsAndSavesPollinationMetadata()
	{
		var crop = new AgricultureCropDefinition(new MudSharp.Models.AgricultureCropDefinition
		{
			Id = 1,
			Name = "Moonmelon",
			Description = "A test crop.",
			Category = "test",
			Definition = @"<Crop growthDays=""30"" harvestWindowDays=""5"" minMoisture=""10"" maxMoisture=""90"" minTemperature=""0"" maxTemperature=""40""><Pollination dependency=""Required"" healthBonus=""1"" yieldBonus=""2"" /></Crop>"
		}, BuildGameworldWithCustomScore(enabled: true).Object);

		Assert.AreEqual(AgriculturePollinationDependency.Required, crop.PollinationDependency);
		Assert.AreEqual(1, crop.PollinationHealthBonus);
		Assert.AreEqual(2, crop.PollinationYieldBonus);

		var saveMethod = typeof(AgricultureCropDefinition).GetMethod("SaveDefinition",
			BindingFlags.Instance | BindingFlags.NonPublic);
		var saved = (XElement)saveMethod!.Invoke(crop, null)!;
		Assert.AreEqual("Required", saved.Element("Pollination")!.Attribute("dependency")!.Value);
		Assert.AreEqual("1", saved.Element("Pollination")!.Attribute("healthBonus")!.Value);
		Assert.AreEqual("2", saved.Element("Pollination")!.Attribute("yieldBonus")!.Value);
	}

	[TestMethod]
	public void HerdDefinition_LoadsAndSavesSecondaryOutputs()
	{
		var herd = new AgricultureHerdDefinition(new MudSharp.Models.AgricultureHerdDefinition
		{
			Id = 1,
			Name = "Test Herd",
			Description = "A test herd.",
			Definition = @"<Herd animalUnits=""1"" dailyGraze=""1"" maximumCondition=""100""><SecondaryOutputs><Commodity material=""milk"" weight=""3500"" tag=""Raw Milk"" /><Commodity material=""feces"" weight=""2500"" tag=""Manure Commodity"" /></SecondaryOutputs></Herd>"
		}, BuildGameworldWithCustomScore(enabled: true).Object);

		Assert.AreEqual(2, herd.SecondaryOutputs.Count);
		Assert.IsTrue(herd.SecondaryOutputs.Any(x =>
			x.MaterialName == "milk" &&
			x.BaseWeight == 3500 &&
			x.TagName == "Raw Milk"));

		var saveMethod = typeof(AgricultureHerdDefinition).GetMethod("SaveDefinition",
			BindingFlags.Instance | BindingFlags.NonPublic);
		var saved = (XElement)saveMethod!.Invoke(herd, null)!;
		var outputs = saved.Element("SecondaryOutputs")!.Elements("Commodity").ToArray();
		Assert.IsTrue(outputs.Any(x =>
			x.Attribute("material")!.Value == "milk" &&
			x.Attribute("tag")!.Value == "Raw Milk"));
		Assert.IsTrue(outputs.Any(x =>
			x.Attribute("material")!.Value == "feces" &&
			x.Attribute("tag")!.Value == "Manure Commodity"));
	}

	[TestMethod]
	public void WhyCannotApply_SowWithoutPlantingWindowsIsUnrestricted()
	{
		var gameworld = BuildFieldGameworld(enabled: false);
		var field = BuildFieldWithCustomScore(gameworld.Object, 50, AgricultureFieldUse.Fallow);
		var operation = BuildOperation(AgricultureOperationType.Sow, AgricultureFieldUse.Fallow,
			AgricultureFieldUse.Crop, AgricultureTargetType.Crop, gameworldOverride: gameworld.Object);
		var crop = BuildCrop(false).Object;

		var reason = operation.WhyCannotApply(field, crop);

		Assert.AreEqual(string.Empty, reason);
	}

	[TestMethod]
	public void WhyCannotApply_SeasonGroupMatchAllowsSowing()
	{
		var gameworld = BuildFieldGameworld(enabled: false, season: BuildSeason("early_spring", "Early Spring", "Spring"));
		var field = BuildFieldWithCustomScore(gameworld.Object, 50, AgricultureFieldUse.Fallow);
		var operation = BuildOperation(AgricultureOperationType.Sow, AgricultureFieldUse.Fallow,
			AgricultureFieldUse.Crop, AgricultureTargetType.Crop, gameworldOverride: gameworld.Object);
		var crop = BuildCrop(false, new AgriculturePlantingWindow(AgriculturePlantingWindowType.Group, "Spring")).Object;

		var reason = operation.WhyCannotApply(field, crop);

		Assert.AreEqual(string.Empty, reason);
	}

	[TestMethod]
	public void WhyCannotApply_SeasonGroupMismatchBlocksSowing()
	{
		var gameworld = BuildFieldGameworld(enabled: false, season: BuildSeason("mid_summer", "Midsummer", "Summer"));
		var field = BuildFieldWithCustomScore(gameworld.Object, 50, AgricultureFieldUse.Fallow);
		var operation = BuildOperation(AgricultureOperationType.Sow, AgricultureFieldUse.Fallow,
			AgricultureFieldUse.Crop, AgricultureTargetType.Crop, gameworldOverride: gameworld.Object);
		var crop = BuildCrop(false, new AgriculturePlantingWindow(AgriculturePlantingWindowType.Group, "Spring")).Object;

		var reason = operation.WhyCannotApply(field, crop);

		StringAssert.Contains(reason, "Test Crop");
		StringAssert.Contains(reason, "group Spring");
	}

	[TestMethod]
	public void WhyCannotApply_ExactSeasonNameAndDisplayNameAllowSowing()
	{
		var gameworld = BuildFieldGameworld(enabled: false, season: BuildSeason("temperate_early_spring", "Early Spring", "Spring"));
		var field = BuildFieldWithCustomScore(gameworld.Object, 50, AgricultureFieldUse.Fallow);
		var operation = BuildOperation(AgricultureOperationType.Sow, AgricultureFieldUse.Fallow,
			AgricultureFieldUse.Crop, AgricultureTargetType.Crop, gameworldOverride: gameworld.Object);
		var byName = BuildCrop(false, new AgriculturePlantingWindow(AgriculturePlantingWindowType.Season, "temperate_early_spring")).Object;
		var byDisplay = BuildCrop(false, new AgriculturePlantingWindow(AgriculturePlantingWindowType.Season, "Early Spring")).Object;

		Assert.AreEqual(string.Empty, operation.WhyCannotApply(field, byName));
		Assert.AreEqual(string.Empty, operation.WhyCannotApply(field, byDisplay));
	}

	[TestMethod]
	public void WhyCannotApply_StaticFallbackUsesCelestialYearFractionForSeasonGroups()
	{
		var gameworld = BuildFieldGameworld(enabled: false, celestialDay: 90.0);
		var field = BuildFieldWithCustomScore(gameworld.Object, 50, AgricultureFieldUse.Fallow);
		var operation = BuildOperation(AgricultureOperationType.Sow, AgricultureFieldUse.Fallow,
			AgricultureFieldUse.Crop, AgricultureTargetType.Crop, gameworldOverride: gameworld.Object);
		var crop = BuildCrop(false, new AgriculturePlantingWindow(AgriculturePlantingWindowType.Group, "Spring")).Object;

		var reason = operation.WhyCannotApply(field, crop);

		Assert.AreEqual(string.Empty, reason);
	}

	[TestMethod]
	public void WhyCannotApply_StaticFallbackHandlesCrossYearWinterRange()
	{
		var gameworld = BuildFieldGameworld(enabled: false, celestialDay: 350.0);
		var field = BuildFieldWithCustomScore(gameworld.Object, 50, AgricultureFieldUse.Fallow);
		var operation = BuildOperation(AgricultureOperationType.Sow, AgricultureFieldUse.Fallow,
			AgricultureFieldUse.Crop, AgricultureTargetType.Crop, gameworldOverride: gameworld.Object);
		var crop = BuildCrop(false, new AgriculturePlantingWindow(AgriculturePlantingWindowType.Group, "Winter")).Object;

		var reason = operation.WhyCannotApply(field, crop);

		Assert.AreEqual(string.Empty, reason);
	}

	[TestMethod]
	public void ApplyOperation_StartAndCompletionBothEnforcePlantingWindows()
	{
		var gameworld = BuildFieldGameworld(enabled: false, season: BuildSeason("mid_summer", "Midsummer", "Summer"));
		var field = BuildFieldWithCustomScore(gameworld.Object, 50, AgricultureFieldUse.Fallow);
		var operation = BuildOperation(AgricultureOperationType.Sow, AgricultureFieldUse.Fallow,
			AgricultureFieldUse.Crop, AgricultureTargetType.Crop, gameworldOverride: gameworld.Object);
		var crop = BuildCrop(false, new AgriculturePlantingWindow(AgriculturePlantingWindowType.Group, "Spring")).Object;

		Assert.IsFalse(field.CanBeginOperation(null!, operation, crop, out var startReason));
		StringAssert.Contains(startReason, "group Spring");
		Assert.IsFalse(field.ApplyOperation(operation, crop, null!, false, out var completionReason));
		StringAssert.Contains(completionReason, "group Spring");
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
	public void CropTick_HappyApiarySupportsPollinationCrop()
	{
		var gameworld = BuildFieldGameworld(enabled: false,
			cropPollination: AgriculturePollinationDependency.Strong,
			pollinationHealthBonus: 1,
			pollinationYieldBonus: 2,
			includeNeighbourCell: true,
			connectNeighbour: true);
		var cropField = BuildFieldWithCustomScore(gameworld.Object, 50, AgricultureFieldUse.Crop, withCrop: true);
		var apiaryField = BuildFieldWithCustomScore(gameworld.Object, 50, AgricultureFieldUse.Fallow,
			apiary: new TestApiaryState(2, 70, 40, 60, 2), fieldId: 2, cellId: 2);
		var fields = new All<IAgricultureField>();
		fields.Add(cropField);
		fields.Add(apiaryField);
		gameworld.SetupGet(x => x.AgricultureFields).Returns(fields);

		cropField.DailyTick();

		Assert.AreEqual(52, cropField.CropHealth);
		Assert.AreEqual(52, cropField.CropYieldPotential);
	}

	[TestMethod]
	public void CropTick_OutOfRangeApiaryDoesNotSupportPollinationCrop()
	{
		var gameworld = BuildFieldGameworld(enabled: false,
			cropPollination: AgriculturePollinationDependency.Strong,
			pollinationHealthBonus: 1,
			pollinationYieldBonus: 2,
			includeNeighbourCell: true,
			connectNeighbour: false);
		var cropField = BuildFieldWithCustomScore(gameworld.Object, 50, AgricultureFieldUse.Crop, withCrop: true);
		var apiaryField = BuildFieldWithCustomScore(gameworld.Object, 50, AgricultureFieldUse.Fallow,
			apiary: new TestApiaryState(2, 70, 40, 60, 2), fieldId: 2, cellId: 2);
		var fields = new All<IAgricultureField>();
		fields.Add(cropField);
		fields.Add(apiaryField);
		gameworld.SetupGet(x => x.AgricultureFields).Returns(fields);

		cropField.DailyTick();

		Assert.AreEqual(51, cropField.CropHealth);
		Assert.AreEqual(50, cropField.CropYieldPotential);
	}

	[TestMethod]
	public void CropTick_HappyApiaryDoesNotAffectNonPollinationCrop()
	{
		var gameworld = BuildFieldGameworld(enabled: false,
			cropPollination: AgriculturePollinationDependency.None,
			pollinationHealthBonus: 1,
			pollinationYieldBonus: 2);
		var field = BuildFieldWithCustomScore(gameworld.Object, 50, AgricultureFieldUse.Crop,
			withCrop: true, apiary: new TestApiaryState(2, 70, 40, 60, 2));
		var fields = new All<IAgricultureField>();
		fields.Add(field);
		gameworld.SetupGet(x => x.AgricultureFields).Returns(fields);

		field.DailyTick();

		Assert.AreEqual(51, field.CropHealth);
		Assert.AreEqual(50, field.CropYieldPotential);
	}

	[TestMethod]
	public void CropTick_RequiredPollinationPenaltyAppliesDuringSettingWithoutHappyApiary()
	{
		var gameworld = BuildFieldGameworld(enabled: false,
			cropPollination: AgriculturePollinationDependency.Required,
			pollinationHealthBonus: 1,
			pollinationYieldBonus: 2);
		var cropField = BuildFieldWithCustomScore(gameworld.Object, 50, AgricultureFieldUse.Crop,
			withCrop: true, cropStage: AgricultureCropStage.Setting);
		gameworld.SetupGet(x => x.AgricultureFields).Returns(new All<IAgricultureField>());

		cropField.DailyTick();

		Assert.AreEqual(46, cropField.CropHealth);
		Assert.AreEqual(47, cropField.CropYieldPotential);
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
	public void DriveHerdTo_MovesRequestedAnimalsAndSetsDestinationPasture()
	{
		var (gameworld, herd) = BuildTwoFieldGameworld();
		var source = BuildFieldWithHerd(gameworld.Object, 1, 1, AgricultureFieldUse.Pasture, herd, 10, 60.0);
		var destination = BuildFieldWithHerd(gameworld.Object, 2, 2, AgricultureFieldUse.Fallow, herd, 0, 50.0);

		var success = source.DriveHerdTo(destination, herd, 4, null!, out var result);

		Assert.IsTrue(success, result);
		Assert.AreEqual(6, source.Herds.Single().HeadCount);
		Assert.AreEqual(4, destination.Herds.Single().HeadCount);
		Assert.AreEqual(AgricultureFieldUse.Pasture, destination.CurrentUse);
		StringAssert.Contains(result, "4");
	}

	[TestMethod]
	public void DriveHerdTo_OmittedCountMovesWholeHerd()
	{
		var (gameworld, herd) = BuildTwoFieldGameworld();
		var source = BuildFieldWithHerd(gameworld.Object, 1, 1, AgricultureFieldUse.Pasture, herd, 7, 60.0);
		var destination = BuildFieldWithHerd(gameworld.Object, 2, 2, AgricultureFieldUse.Pasture, herd, 3, 50.0);

		var success = source.DriveHerdTo(destination, herd, 0, null!, out var result);

		Assert.IsTrue(success, result);
		Assert.IsFalse(source.Herds.Any());
		Assert.AreEqual(10, destination.Herds.Single().HeadCount);
		Assert.AreEqual(57.0, destination.Herds.Single().Condition, 0.01);
	}

	[TestMethod]
	public void DriveHerdTo_BlocksOwnedDestinationWithoutAccess()
	{
		var (gameworld, herd) = BuildTwoFieldGameworld(destinationOwned: true);
		var source = BuildFieldWithHerd(gameworld.Object, 1, 1, AgricultureFieldUse.Pasture, herd, 7, 60.0);
		var destination = BuildFieldWithHerd(gameworld.Object, 2, 2, AgricultureFieldUse.Pasture, herd, 0, 50.0);

		var success = source.DriveHerdTo(destination, herd, 1, null!, out var result);

		Assert.IsFalse(success);
		StringAssert.Contains(result, "drive herds into");
		Assert.AreEqual(7, source.Herds.Single().HeadCount);
		Assert.IsFalse(destination.Herds.Any());
	}

	[TestMethod]
	public void DriveHerdTo_RejectsDestinationAlreadyInAnotherUse()
	{
		var (gameworld, herd) = BuildTwoFieldGameworld();
		var source = BuildFieldWithHerd(gameworld.Object, 1, 1, AgricultureFieldUse.Pasture, herd, 7, 60.0);
		var destination = BuildFieldWithHerd(gameworld.Object, 2, 2, AgricultureFieldUse.Crop, herd, 0, 50.0);

		var success = source.DriveHerdTo(destination, herd, 1, null!, out var result);

		Assert.IsFalse(success);
		StringAssert.Contains(result, "fallow or pasture");
		Assert.AreEqual(7, source.Herds.Single().HeadCount);
		Assert.IsFalse(destination.Herds.Any());
	}

	[TestMethod]
	public void AgricultureFieldInput_ScoutInput_RejectsOwnedFieldWithoutAuthorisation()
	{
		var gameworld = BuildGameworldWithCustomScore(enabled: false);
		var cell = new Mock<ICell>();
		cell.SetupGet(x => x.Id).Returns(1L);
		cell.SetupGet(x => x.Name).Returns("Owned Crop Field");
		cell.SetupGet(x => x.FrameworkItemType).Returns("Cell");
		cell.SetupGet(x => x.Gameworld).Returns(gameworld.Object);
		var cells = new All<ICell>();
		cells.Add(cell.Object);
		gameworld.SetupGet(x => x.Cells).Returns(cells);
		var field = BuildFieldWithCustomScore(gameworld.Object, 50, AgricultureFieldUse.Fallow);
		cell.SetupGet(x => x.AgricultureField).Returns(field);
		var property = new Mock<IProperty>();
		property.SetupGet(x => x.Id).Returns(1L);
		property.SetupGet(x => x.Name).Returns("Owned Farm");
		property.SetupGet(x => x.FrameworkItemType).Returns("Property");
		property.SetupGet(x => x.PropertyLocations).Returns([cell.Object]);
		var properties = new All<IProperty>();
		properties.Add(property.Object);
		gameworld.SetupGet(x => x.Properties).Returns(properties);
		var actor = new Mock<ICharacter>();
		actor.SetupGet(x => x.Location).Returns(cell.Object);
		actor.SetupGet(x => x.Gameworld).Returns(gameworld.Object);
		actor.Setup(x => x.IsAdministrator(It.IsAny<PermissionLevel>())).Returns(false);
		var input = AgricultureFieldInputFromDefinition(gameworld.Object,
			new XElement("Definition",
				new XElement("RequiredUse", AgricultureFieldUse.Fallow),
				new XElement("CropDefinitionId", 0),
				new XElement("WoodlandDefinitionId", 0),
				new XElement("MinimumFieldCondition", 0),
				new XElement("MinimumHealth", 0),
				new XElement("MinimumYield", 0),
				new XElement("YieldConsumed", 0)));

		Assert.IsFalse(input.ScoutInput(actor.Object).Any());
	}

	[TestMethod]
	public void AbsorbNpcIntoHerd_RejectsNonAnimalNpc()
	{
		var (gameworld, herd) = BuildTwoFieldGameworld();
		gameworld.SetupGet(x => x.BodyPrototypes).Returns(new All<IBodyPrototype>());
		var source = BuildFieldWithHerd(gameworld.Object, 1, 1, AgricultureFieldUse.Pasture, herd, 0, 60.0);
		var baseBody = new Mock<IBodyPrototype>();
		baseBody.SetupGet(x => x.Id).Returns(1L);
		baseBody.SetupGet(x => x.Name).Returns("Humanoid");
		baseBody.SetupGet(x => x.FrameworkItemType).Returns("BodyPrototype");
		var race = new Mock<IRace>();
		race.SetupGet(x => x.BaseBody).Returns(baseBody.Object);
		var npc = new Mock<ICharacter>();
		npc.SetupGet(x => x.IsPlayerCharacter).Returns(false);
		npc.SetupGet(x => x.Location).Returns(source.Cell);
		npc.SetupGet(x => x.Gameworld).Returns(gameworld.Object);
		npc.SetupGet(x => x.Race).Returns(race.Object);
		var actor = new Mock<ICharacter>();

		var success = source.AbsorbNpcIntoHerd(npc.Object, herd, actor.Object, out var result);

		Assert.IsFalse(success);
		StringAssert.Contains(result, "non-player animal");
		npc.Verify(x => x.Quit(It.IsAny<bool>()), Times.Never);
	}

	[TestMethod]
	public void HerdTick_FedHerdBuildsSecondaryYieldPotential()
	{
		var (gameworld, herd) = BuildTwoFieldGameworld();
		var field = BuildFieldWithHerd(gameworld.Object, 1, 1, AgricultureFieldUse.Pasture, herd, 10, 60.0,
			secondaryYield: 10);

		field.DailyTick();

		Assert.IsTrue(field.Herds.Single().SecondaryYieldPotential > 10);
	}

	[TestMethod]
	public void WhyCannotApply_HarvestHerdProductsRequiresReadySecondaryYield()
	{
		var herd = new Mock<IAgricultureHerdDefinition>();
		herd.SetupGet(x => x.Id).Returns(1);
		herd.SetupGet(x => x.Name).Returns("Cattle Herd");
		herd.SetupGet(x => x.SecondaryOutputs).Returns([
			new AgricultureCommodityYield("milk", 3500, "Raw Milk")
		]);
		var field = new Mock<IAgricultureField>();
		field.SetupGet(x => x.CurrentUse).Returns(AgricultureFieldUse.Pasture);
		field.SetupGet(x => x.Herds).Returns([
			new AgricultureFieldHerd(1, herd.Object, 4, 60.0, 0)
		]);
		var operation = BuildOperation(AgricultureOperationType.HarvestHerdProducts, AgricultureFieldUse.Pasture,
			AgricultureFieldUse.Pasture, AgricultureTargetType.Herd, herdYieldMultiplier: 1.0, herdYieldCost: 55);

		var reason = operation.WhyCannotApply(field.Object, herd.Object);

		Assert.AreEqual("That herd does not have any secondary products ready to collect.", reason);

		field.SetupGet(x => x.Herds).Returns([
			new AgricultureFieldHerd(1, herd.Object, 4, 60.0, 40)
		]);

		Assert.AreEqual(string.Empty, operation.WhyCannotApply(field.Object, herd.Object));
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
	public void WhyCannotApply_AllowedUsesPermitApiaryOperationsWithoutChangingPrimaryUse()
	{
		var operation = BuildOperation(AgricultureOperationType.InstallApiary, AgricultureFieldUse.Fallow,
			AgricultureFieldUse.Fallow, allowedUses:
			[
				AgricultureFieldUse.Fallow,
				AgricultureFieldUse.Crop,
				AgricultureFieldUse.Orchard,
				AgricultureFieldUse.Pasture,
				AgricultureFieldUse.Woodland
			], apiaryInstallHives: 2, apiaryRadius: 2);
		var field = new Mock<IAgricultureField>();
		field.SetupGet(x => x.CurrentUse).Returns(AgricultureFieldUse.Crop);
		field.SetupGet(x => x.HasActiveApiary).Returns(false);

		var reason = operation.WhyCannotApply(field.Object, null);

		Assert.AreEqual(string.Empty, reason);
		CollectionAssert.Contains(operation.AllowedUses.ToList(), AgricultureFieldUse.Crop);
	}

	[TestMethod]
	public void ApplyOperation_InstallsTendsHarvestsAndRemovesApiaryState()
	{
		var gameworld = BuildFieldGameworld(enabled: false);
		var field = BuildFieldWithCustomScore(gameworld.Object, 50, AgricultureFieldUse.Fallow);
		gameworld.SetupGet(x => x.AgricultureFields).Returns(new All<IAgricultureField>());
		var install = BuildOperation(AgricultureOperationType.InstallApiary, AgricultureFieldUse.Fallow,
			AgricultureFieldUse.Fallow, apiaryInstallHives: 2, apiaryRadius: 2, gameworldOverride: gameworld.Object);
		var tend = BuildOperation(AgricultureOperationType.TendApiary, AgricultureFieldUse.Fallow,
			AgricultureFieldUse.Fallow, apiaryTendHealth: 5, apiaryTendStores: 4, apiaryTendYield: 3,
			gameworldOverride: gameworld.Object);
		var remove = BuildOperation(AgricultureOperationType.RemoveApiary, AgricultureFieldUse.Fallow,
			AgricultureFieldUse.Fallow, gameworldOverride: gameworld.Object);

		Assert.IsTrue(field.ApplyOperation(install, null, null!, false, out _));
		Assert.AreEqual(AgricultureFieldUse.Fallow, field.CurrentUse);
		Assert.IsTrue(field.HasActiveApiary);
		Assert.AreEqual(2, field.Apiary.HiveCount);

		Assert.IsTrue(field.ApplyOperation(tend, null, null!, false, out _));
		Assert.IsTrue(field.Apiary.ColonyHealth > 50);
		Assert.IsTrue(field.Apiary.Stores > 35);

		var saveMethod = typeof(AgricultureField).GetMethod("SaveFieldDefinition",
			BindingFlags.Instance | BindingFlags.NonPublic);
		var saved = (XElement)saveMethod!.Invoke(field, null)!;
		Assert.AreEqual("2", saved.Element("Apiary")!.Attribute("hives")!.Value);

		Assert.IsTrue(field.ApplyOperation(remove, null, null!, false, out _));
		Assert.IsFalse(field.HasActiveApiary);
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

	private static ISeason BuildSeason(string name, string displayName, string group)
	{
		var season = new Mock<ISeason>();
		season.SetupGet(x => x.Id).Returns(1);
		season.SetupGet(x => x.Name).Returns(name);
		season.SetupGet(x => x.DisplayName).Returns(displayName);
		season.SetupGet(x => x.SeasonGroup).Returns(group);
		season.SetupGet(x => x.FrameworkItemType).Returns("Season");
		return season.Object;
	}

	private static Mock<IAgricultureCropDefinition> BuildCrop(bool perennial,
		params AgriculturePlantingWindow[] plantingWindows)
	{
		var crop = new Mock<IAgricultureCropDefinition>();
		crop.SetupGet(x => x.Id).Returns(1);
		crop.SetupGet(x => x.Name).Returns("Test Crop");
		crop.SetupGet(x => x.FrameworkItemType).Returns("AgricultureCropDefinition");
		crop.SetupGet(x => x.IsPerennial).Returns(perennial);
		crop.SetupGet(x => x.PlantingWindows).Returns(plantingWindows);
		return crop;
	}

	private static AgricultureOperation BuildOperation(AgricultureOperationType type, AgricultureFieldUse requiredUse,
		AgricultureFieldUse resultUse, AgricultureTargetType targetType = AgricultureTargetType.None,
		IEnumerable<(AgricultureScoreType Score, int Delta)>? scoreDeltas = null,
		IFuturemud? gameworldOverride = null,
		IEnumerable<AgricultureFieldUse>? allowedUses = null,
		int apiaryInstallHives = 0,
		int apiaryRadius = 0,
		int apiaryTendHealth = 0,
		int apiaryTendStores = 0,
		int apiaryTendYield = 0,
		double herdYieldMultiplier = 0.0,
		int herdYieldCost = 0)
	{
		var root = new XElement("Operation",
			new XAttribute("herdYieldMultiplier", herdYieldMultiplier),
			new XAttribute("herdYieldCost", herdYieldCost),
			scoreDeltas?.Select(x => new XElement("Score",
				new XAttribute("type", x.Score.ToString()),
				new XAttribute("value", x.Delta))) ?? Enumerable.Empty<XElement>());
		if (allowedUses != null)
		{
			root.Add(new XElement("AllowedUses",
				new XAttribute("uses", string.Join(",", allowedUses.Select(x => x.ToString())))));
		}

		root.Add(new XElement("Apiary",
			new XAttribute("installHives", apiaryInstallHives),
			new XAttribute("pollinationRadius", apiaryRadius),
			new XAttribute("tendHealthDelta", apiaryTendHealth),
			new XAttribute("tendStoresDelta", apiaryTendStores),
			new XAttribute("tendYieldDelta", apiaryTendYield)));
		var model = new MudSharp.Models.AgricultureOperation
		{
			Id = 1,
			Name = "Test Operation",
			Description = "A test agriculture operation.",
			OperationType = (int)type,
			TargetType = (int)targetType,
			RequiredUse = (int)requiredUse,
			ResultUse = (int)resultUse,
			Definition = root.ToString()
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

	private static Mock<IFuturemud> BuildFieldGameworld(bool enabled, bool higherIsGood = true,
		ISeason? season = null, double? celestialDay = null,
		AgriculturePollinationDependency cropPollination = AgriculturePollinationDependency.None,
		int pollinationHealthBonus = 0,
		int pollinationYieldBonus = 0,
		bool includeNeighbourCell = false,
		bool connectNeighbour = false)
	{
		var gameworld = BuildGameworldWithCustomScore(enabled, higherIsGood);
		var saveManager = new Mock<ISaveManager>();
		gameworld.SetupGet(x => x.SaveManager).Returns(saveManager.Object);
		gameworld
			.Setup(x => x.GetStaticConfiguration(AgriculturePlantingWindowExtensions.SeasonGroupWindowsStaticConfiguration))
			.Returns(AgriculturePlantingWindowExtensions.DefaultSeasonGroupWindowsConfigurationText);
		var cells = new All<ICell>();
		var cell = new Mock<ICell>();
		cell.SetupGet(x => x.Id).Returns(1);
		cell.SetupGet(x => x.Name).Returns("Test Cell");
		cell.SetupGet(x => x.FrameworkItemType).Returns("Cell");
		cell.SetupGet(x => x.Gameworld).Returns(gameworld.Object);
		cell.Setup(x => x.CurrentTemperature(It.IsAny<IPerceiver>())).Returns(20.0);
		cell.Setup(x => x.CurrentWeather(It.IsAny<IPerceiver>())).Returns(default(IWeatherEvent)!);
		cell.Setup(x => x.CurrentSeason(It.IsAny<IPerceiver>())).Returns(season!);
		cell.Setup(x => x.ExitsFor(It.IsAny<IPerceiver>(), It.IsAny<bool>())).Returns(Array.Empty<ICellExit>());
		cells.Add(cell.Object);
		if (includeNeighbourCell)
		{
			var neighbour = new Mock<ICell>();
			neighbour.SetupGet(x => x.Id).Returns(2);
			neighbour.SetupGet(x => x.Name).Returns("Neighbour Cell");
			neighbour.SetupGet(x => x.FrameworkItemType).Returns("Cell");
			neighbour.SetupGet(x => x.Gameworld).Returns(gameworld.Object);
			neighbour.Setup(x => x.CurrentTemperature(It.IsAny<IPerceiver>())).Returns(20.0);
			neighbour.Setup(x => x.CurrentWeather(It.IsAny<IPerceiver>())).Returns(default(IWeatherEvent)!);
			neighbour.Setup(x => x.CurrentSeason(It.IsAny<IPerceiver>())).Returns(season!);
			neighbour.Setup(x => x.ExitsFor(It.IsAny<IPerceiver>(), It.IsAny<bool>())).Returns(Array.Empty<ICellExit>());
			if (connectNeighbour)
			{
				var outbound = new Mock<ICellExit>();
				outbound.SetupGet(x => x.Destination).Returns(neighbour.Object);
				var inbound = new Mock<ICellExit>();
				inbound.SetupGet(x => x.Destination).Returns(cell.Object);
				cell.Setup(x => x.ExitsFor(It.IsAny<IPerceiver>(), It.IsAny<bool>()))
				    .Returns([outbound.Object]);
				neighbour.Setup(x => x.ExitsFor(It.IsAny<IPerceiver>(), It.IsAny<bool>()))
				         .Returns([inbound.Object]);
			}

			cells.Add(neighbour.Object);
		}

		gameworld.SetupGet(x => x.Cells).Returns(cells);
		var celestials = new All<ICelestialObject>();
		if (celestialDay.HasValue)
		{
			var celestial = new Mock<ICelestialObject>();
			celestial.SetupGet(x => x.Id).Returns(1);
			celestial.SetupGet(x => x.Name).Returns("Test Sun");
			celestial.SetupGet(x => x.FrameworkItemType).Returns("Celestial");
			celestial.SetupGet(x => x.CurrentCelestialDay).Returns(celestialDay.Value);
			celestial.SetupGet(x => x.CelestialDaysPerYear).Returns(365.0);
			celestials.Add(celestial.Object);
		}

		gameworld.SetupGet(x => x.CelestialObjects).Returns(celestials);

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
		crop.SetupGet(x => x.PollinationDependency).Returns(cropPollination);
		crop.SetupGet(x => x.PollinationHealthBonus).Returns(pollinationHealthBonus);
		crop.SetupGet(x => x.PollinationYieldBonus).Returns(pollinationYieldBonus);
		crop.SetupGet(x => x.HarvestCycleDays).Returns(30);
		crop.SetupGet(x => x.PlantingWindows).Returns(Array.Empty<AgriculturePlantingWindow>());
		crop.SetupGet(x => x.ScoreRanges).Returns(
		[
			new AgricultureScoreRange(AgricultureScoreType.Custom1, 30, 70)
		]);
		crops.Add(crop.Object);
		gameworld.SetupGet(x => x.AgricultureCropDefinitions).Returns(crops);

		gameworld.SetupGet(x => x.Properties).Returns(new All<IProperty>());
		gameworld.SetupGet(x => x.AgricultureFields).Returns(new All<IAgricultureField>());
		return gameworld;
	}

	private static (Mock<IFuturemud> Gameworld, IAgricultureHerdDefinition Herd) BuildTwoFieldGameworld(
		bool destinationOwned = false)
	{
		var gameworld = BuildGameworldWithCustomScore(false);
		var saveManager = new Mock<ISaveManager>();
		gameworld.SetupGet(x => x.SaveManager).Returns(saveManager.Object);

		var cells = new All<ICell>();
		var sourceCell = BuildCell(gameworld.Object, 1, "Source Field");
		var destinationCell = BuildCell(gameworld.Object, 2, "Destination Field");
		cells.Add(sourceCell);
		cells.Add(destinationCell);
		gameworld.SetupGet(x => x.Cells).Returns(cells);

		var profiles = new All<IAgricultureFieldProfile>();
		var profile = new Mock<IAgricultureFieldProfile>();
		profile.SetupGet(x => x.Id).Returns(1);
		profile.SetupGet(x => x.Name).Returns("Pasture Profile");
		profile.SetupGet(x => x.FrameworkItemType).Returns("AgricultureFieldProfile");
		profile.SetupGet(x => x.DefaultScores).Returns(new Dictionary<AgricultureScoreType, int>());
		profile.Setup(x => x.AllowsUse(It.IsAny<AgricultureFieldUse>())).Returns(true);
		profiles.Add(profile.Object);
		gameworld.SetupGet(x => x.AgricultureFieldProfiles).Returns(profiles);

		var herd = new Mock<IAgricultureHerdDefinition>();
		herd.SetupGet(x => x.Id).Returns(1);
		herd.SetupGet(x => x.Name).Returns("Cattle Herd");
		herd.SetupGet(x => x.FrameworkItemType).Returns("AgricultureHerdDefinition");
		herd.SetupGet(x => x.MaximumCondition).Returns(100);
		herd.SetupGet(x => x.AnimalUnits).Returns(1.0);
		herd.SetupGet(x => x.DailyGraze).Returns(1.0);
		herd.SetupGet(x => x.SecondaryOutputs).Returns([
			new AgricultureCommodityYield("milk", 3500, "Raw Milk")
		]);
		var herds = new All<IAgricultureHerdDefinition>();
		herds.Add(herd.Object);
		gameworld.SetupGet(x => x.AgricultureHerdDefinitions).Returns(herds);

		var properties = new All<IProperty>();
		if (destinationOwned)
		{
			var property = new Mock<IProperty>();
			property.SetupGet(x => x.Id).Returns(1);
			property.SetupGet(x => x.Name).Returns("Owned Destination");
			property.SetupGet(x => x.FrameworkItemType).Returns("Property");
			property.SetupGet(x => x.PropertyLocations).Returns(new[] { destinationCell });
			properties.Add(property.Object);
		}

		gameworld.SetupGet(x => x.Properties).Returns(properties);
		return (gameworld, herd.Object);
	}

	private static ICell BuildCell(IFuturemud gameworld, long id, string name)
	{
		var cell = new Mock<ICell>();
		cell.SetupGet(x => x.Id).Returns(id);
		cell.SetupGet(x => x.Name).Returns(name);
		cell.SetupGet(x => x.FrameworkItemType).Returns("Cell");
		cell.SetupGet(x => x.Gameworld).Returns(gameworld);
		return cell.Object;
	}

	private static AgricultureField BuildFieldWithHerd(IFuturemud gameworld, long id, long cellId,
		AgricultureFieldUse use, IAgricultureHerdDefinition herd, int count, double herdCondition,
		int secondaryYield = 0)
	{
		var model = new MudSharp.Models.AgricultureField
		{
			Id = id,
			CellId = cellId,
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
			Definition = "<Field />"
		};
		if (count > 0)
		{
			model.AgricultureFieldHerds.Add(new MudSharp.Models.AgricultureFieldHerd
			{
				Id = id * 10,
				AgricultureFieldId = id,
				HerdDefinitionId = herd.Id,
				HeadCount = count,
				Condition = herdCondition,
				Definition = new XElement("Herd",
					new XAttribute("secondaryYield", secondaryYield)).ToString()
			});
		}

		return new AgricultureField(model, gameworld);
	}

	private static AgricultureField BuildFieldWithCustomScore(IFuturemud gameworld, int customScore,
		AgricultureFieldUse use, bool withCrop = false, AgricultureCropStage cropStage = AgricultureCropStage.Growing,
		TestApiaryState? apiary = null, long fieldId = 1, long cellId = 1)
	{
		var model = new MudSharp.Models.AgricultureField
		{
			Id = fieldId,
			CellId = cellId,
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
				apiary == null
					? null
					: new XElement("Apiary",
						new XAttribute("hives", apiary.HiveCount),
						new XAttribute("health", apiary.ColonyHealth),
						new XAttribute("stores", apiary.Stores),
						new XAttribute("yield", apiary.YieldPotential),
						new XAttribute("radius", apiary.PollinationRadius)),
				new XElement("CustomScores",
					new XElement("Score",
						new XAttribute("type", AgricultureScoreType.Custom1.ToString()),
						new XAttribute("value", customScore)))).ToString()
		};

		if (withCrop)
		{
			model.AgricultureFieldCrop = new MudSharp.Models.AgricultureFieldCrop
			{
				AgricultureFieldId = fieldId,
				CropDefinitionId = 1,
				Stage = (int)cropStage,
				GrowthDays = cropStage == AgricultureCropStage.Setting ? 20 : 10,
				Health = 50,
				YieldPotential = 50,
				Definition = "<Crop />"
			};
		}

		return new AgricultureField(model, gameworld);
	}

	private static AgricultureFieldInput AgricultureFieldInputFromDefinition(IFuturemud gameworld, XElement definition)
	{
		var model = new MudSharp.Models.CraftInput
		{
			Id = 1,
			InputQualityWeight = 1.0,
			OriginalAdditionTime = DateTime.UtcNow,
			Definition = definition.ToString()
		};

		return (AgricultureFieldInput)Activator.CreateInstance(
			typeof(AgricultureFieldInput),
			BindingFlags.Instance | BindingFlags.NonPublic,
			null,
			[model, new Mock<ICraft>().Object, gameworld],
			null)!;
	}

	private sealed record TestApiaryState(int HiveCount, int ColonyHealth, int Stores, int YieldPotential,
		int PollinationRadius);
}
