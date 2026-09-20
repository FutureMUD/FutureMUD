#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Character;
using MudSharp.Construction;
using MudSharp.FutureProg;
using MudSharp.GameItems;
using MudSharp.Framework;
using MudSharp.Magic;
using MudSharp.Magic.Environment;
using MudSharp.Magic.Generators;
using MudSharp.Magic.Resources;
using MudSharp.Testing.EnvironmentalMagic;
using MudSharp.Work.Agriculture;

namespace MudSharp_Unit_Tests;

[TestClass]
public class EnvironmentalMagicCoordinatorTests
{
	[TestMethod]
	[TestCategory("Y-T01")]
	[TestCategory("Y-T03")]
	public void OrganicSourceRequiresExplicitProfileAuthorisationAndInspectionIsPure()
	{
		using var world = new EnvironmentalMagicTestWorld(policy: "native-yield");
		var cell = (Cell)world.Cells.At(0);
		var saveRequests = world.SaveRequests;
		var before = world.Coordinator.InspectOrganicSource(cell, "forage:herbs");
		Assert.AreEqual(NativeOrganicSourceStatus.Unauthorised, before.Status);

		world.Edit("organic source add forage herbs");
		world.Edit("organic source add forage berries");
		world.ResetSavedFlags();
		world.Cells.ForbidEnumeration = true;
		world.Fields.ForbidEnumeration = true;
		var snapshot = world.Coordinator.InspectOrganicSource(cell, "FORAGE:Herbs");

		Assert.AreEqual(NativeOrganicSourceStatus.Available, snapshot.Status);
		Assert.AreEqual("forage:herbs", snapshot.Selector);
		Assert.AreEqual(100.0, snapshot.NativeStock);
		Assert.AreEqual(0m, snapshot.PrepaidFraction);
		Assert.IsNotNull(snapshot.Lifecycle);
		Assert.AreEqual(1L, snapshot.Lifecycle.ForageProfileId);
		Assert.AreEqual(NativeOrganicSourceStatus.Absent,
			world.Coordinator.InspectOrganicSource(cell, "forage:berries").Status);
		Assert.AreEqual(NativeOrganicSourceStatus.Invalid,
			world.Coordinator.InspectOrganicSource(cell, "not-a-selector").Status);
		Assert.AreEqual(NativeOrganicSourceStatus.Indeterminate,
			world.Coordinator.InspectOrganicSource(Mock.Of<ICell>(), "crop").Status);
		Assert.AreEqual(saveRequests, world.SaveRequests);
		Assert.AreEqual(1, world.SecondSubscriptions);
	}

	[TestMethod]
	[TestCategory("Y-T04")]
	[TestCategory("Y-T05")]
	public void ForagePlanAndApplyUsesExactOwnerDebitAndRejectsAStalePlan()
	{
		using var world = new EnvironmentalMagicTestWorld(policy: "native-yield");
		world.Edit("organic source add forage herbs");
		var cell = (Cell)world.Cells.At(0);

		Assert.IsTrue(world.Coordinator.TryPlanOrganicDebit(cell, "forage:herbs", 0.25,
			out var first, out var error), error);
		Assert.IsTrue(world.Coordinator.TryPlanOrganicDebit(cell, "forage:herbs", 0.5,
			out var stale, out error), error);
		Assert.IsTrue(world.Coordinator.TryApplyOrganicDebit(cell, first, out var result, out error), error);
		Assert.AreEqual(99.75, result.NativeStock, 1e-12);
		Assert.IsFalse(world.Coordinator.TryApplyOrganicDebit(cell, stale, out result, out error));
		StringAssert.Contains(error!, "changed");
		Assert.AreEqual(99.75, result.NativeStock, 1e-12);
		Assert.IsTrue(world.Coordinator.TryPlanOrganicDebit(cell, "forage:herbs", 99.75,
			out var remainder, out error), error);
		Assert.IsTrue(world.Coordinator.TryApplyOrganicDebit(cell, remainder, out result, out error), error);
		Assert.AreEqual(NativeOrganicSourceStatus.Exhausted, result.Status);
		Assert.AreEqual(0.0, result.NativeStock);
		Assert.AreEqual(0, world.Operations.Commits);
		Assert.AreEqual(1, world.SecondSubscriptions);
	}

	[TestMethod]
	[TestCategory("C-R4-01")]
	[TestCategory("C-R4-02")]
	public void OrganicConversion_DynamicInvalidPenaltyRejectsPlanAndApplyWithoutDebitingStock()
	{
		using var world = new EnvironmentalMagicTestWorld(policy: "native-yield");
		world.Edit("organic source add forage herbs");
		world.Edit("organic penalty forage 1 - scardamage / 100");
		var cell = (Cell)world.Cells.At(0);
		Assert.IsTrue(world.Coordinator.TryPlanOrganicDebit(cell, "forage:herbs", 0.25,
			out var plan, out var error), error);
		var before = world.Coordinator.InspectOrganicSource(cell, "forage:herbs");
		var damage = world.Coordinator.ApplyOperation(cell, Request(damage: 150.0));
		Assert.IsTrue(damage.Success, damage.Error);
		var invalid = world.Coordinator.InspectOrganicSource(cell, "forage:herbs");
		Assert.AreEqual(NativeOrganicSourceStatus.Invalid, invalid.Status);
		Assert.AreEqual(before.NativeStock, invalid.NativeStock);
		Assert.IsNotNull(invalid.Diagnostic);
		Assert.IsFalse(world.Coordinator.TryPlanOrganicDebit(cell, "forage:herbs", 0.25,
			out _, out error));
		Assert.IsFalse(world.Coordinator.TryApplyOrganicDebit(cell, plan, out var refused, out error));
		Assert.AreEqual(before.NativeStock, refused.NativeStock);
		Assert.AreEqual(before.SourceRevision, refused.SourceRevision);
		var repair = world.Coordinator.ApplyOperation(cell, Request(repair: 150.0));
		Assert.IsTrue(repair.Success, repair.Error);
		Assert.IsTrue(world.Coordinator.TryPlanOrganicDebit(cell, "forage:herbs", 0.25,
			out var corrected, out error), error);
		Assert.IsTrue(world.Coordinator.TryApplyOrganicDebit(cell, corrected, out var applied, out error), error);
		Assert.AreEqual(before.NativeStock - 0.25, applied.NativeStock, 1e-12);
	}

	[TestMethod]
	[TestCategory("C-R4-03")]
	[TestCategory("C-R4-06")]
	public void OrganicConversion_ZeroFactorIsValidAndForageBaselineIsUnclippedHourlyRate()
	{
		using var world = new EnvironmentalMagicTestWorld(policy: "native-yield");
		world.Edit("organic source add forage herbs");
		var cell = (Cell)world.Cells.At(0);
		cell.ConsumeYield("herbs", 1.0);
		world.Edit("organic penalty forage 0");
		Assert.IsTrue(world.Coordinator.TryPlanOrganicDebit(cell, "forage:herbs", 0.25,
			out _, out var error), error);
		world.Edit("organic penalty forage 10 / baselineincrease");
		Assert.IsTrue(world.Coordinator.TryPlanOrganicDebit(cell, "forage:herbs", 0.25,
			out _, out error), error);
		Assert.AreEqual(99.0, world.Coordinator.InspectOrganicSource(cell, "forage:herbs").NativeStock);
	}

	[TestMethod]
	[TestCategory("C-R4-01")]
	[TestCategory("C-R4-02")]
	public void OrganicConversion_UsesCurrentNativeCropPollinationBaseline()
	{
		foreach (var (formula, expectedValid, expectedHealth) in new[]
		         {
			         ("1 - baselineincrease / 2.0", false, 50),
			         ("2.0 / baselineincrease", true, 52)
		         })
		{
			using var world = new EnvironmentalMagicTestWorld(policy: "native-yield");
			world.Edit("organic source add crop");
			world.Edit($"organic penalty crophealth {formula}");
			var (field, setPollination) = BuildPollinatedNativeCrop(world);
			world.Fields.ForbidEnumeration = true;
			var context = field.InspectCurrentOrganicRecoveryContext(NativeOrganicPenaltyChannel.CropHealthRecovery);
			Assert.IsNotNull(context);
			Assert.AreEqual(4.0, context.BaselineIncrease);
			var source = world.Coordinator.InspectOrganicSource(field.Cell, "crop");
			Assert.AreEqual(expectedValid ? NativeOrganicSourceStatus.Available : NativeOrganicSourceStatus.Invalid,
				source.Status, formula);
			Assert.AreEqual(50.0, source.NativeStock);
			Assert.AreEqual(expectedValid, world.Coordinator.TryPlanOrganicDebit(field.Cell, "crop", 0.25,
				out _, out _), formula);
			if (expectedValid)
			{
				Assert.IsTrue(world.Coordinator.TryPlanOrganicDebit(field.Cell, "crop", 0.25,
					out var planned, out var error), error);
				var before = field.InspectNativeOrganicSource(NativeOrganicSourceKind.Crop);
				setPollination(false);
				Assert.IsFalse(world.Coordinator.TryApplyOrganicDebit(field.Cell, planned, out _, out _));
				var after = field.InspectNativeOrganicSource(NativeOrganicSourceKind.Crop);
				Assert.AreEqual(before.NativeStock, after.NativeStock);
				Assert.AreEqual(before.PrepaidFraction, after.PrepaidFraction);
				Assert.AreEqual(before.SourceRevision, after.SourceRevision);
				setPollination(true);
			}
			else
			{
				setPollination(false);
				var unpollinated = field.InspectCurrentOrganicRecoveryContext(
					NativeOrganicPenaltyChannel.CropHealthRecovery);
				Assert.IsNotNull(unpollinated);
				Assert.AreEqual(1.0, unpollinated.BaselineIncrease);
				Assert.AreEqual(NativeOrganicSourceStatus.Available,
					world.Coordinator.InspectOrganicSource(field.Cell, "crop").Status);
				Assert.IsTrue(world.Coordinator.TryPlanOrganicDebit(field.Cell, "crop", 0.25,
					out var planned, out var error), error);
				var before = field.InspectNativeOrganicSource(NativeOrganicSourceKind.Crop);
				setPollination(true);
				Assert.IsFalse(world.Coordinator.TryApplyOrganicDebit(field.Cell, planned, out _, out _));
				var after = field.InspectNativeOrganicSource(NativeOrganicSourceKind.Crop);
				Assert.AreEqual(before.NativeStock, after.NativeStock);
				Assert.AreEqual(before.PrepaidFraction, after.PrepaidFraction);
				Assert.AreEqual(before.SourceRevision, after.SourceRevision);
				setPollination(false);
			}
			field.DailyTick();
			Assert.AreEqual(expectedHealth, field.CropHealth, formula);
			if (!expectedValid)
				Assert.AreEqual(0.5m,
					field.InspectNativeOrganicSource(NativeOrganicSourceKind.Crop).RecoveryRemainders.Health);
		}
	}

	private static (AgricultureField Field, Action<bool> SetPollination) BuildPollinatedNativeCrop(
		EnvironmentalMagicTestWorld world)
	{
		var cell = (Cell)world.Cells.At(0);
		var profile = new Mock<IAgricultureFieldProfile>();
		profile.SetupGet(x => x.Id).Returns(1L);
		profile.SetupGet(x => x.DefaultScores).Returns(new Dictionary<AgricultureScoreType, int>());
		var profiles = new All<IAgricultureFieldProfile>();
		profiles.Add(profile.Object);
		world.World.SetupGet(x => x.AgricultureFieldProfiles).Returns(profiles);
		var crop = new Mock<IAgricultureCropDefinition>();
		crop.SetupGet(x => x.Id).Returns(1L);
		crop.SetupGet(x => x.BaseGrowthDays).Returns(30);
		crop.SetupGet(x => x.HarvestWindowDays).Returns(5);
		crop.SetupGet(x => x.MinimumMoisture).Returns(0);
		crop.SetupGet(x => x.MaximumMoisture).Returns(100);
		crop.SetupGet(x => x.MinimumTemperature).Returns(-100);
		crop.SetupGet(x => x.MaximumTemperature).Returns(100);
		crop.SetupGet(x => x.PollinationDependency).Returns(AgriculturePollinationDependency.Beneficial);
		crop.SetupGet(x => x.PollinationHealthBonus).Returns(3);
		crop.SetupGet(x => x.ScoreRanges).Returns(Array.Empty<AgricultureScoreRange>());
		var crops = new All<IAgricultureCropDefinition>();
		crops.Add(crop.Object);
		world.World.SetupGet(x => x.AgricultureCropDefinitions).Returns(crops);
		world.World.SetupGet(x => x.Properties).Returns(new All<MudSharp.Economy.Property.IProperty>());
		var model = new MudSharp.Models.AgricultureField
		{
			Id = 1, CellId = cell.Id, ProfileId = 1, CurrentUse = (int)AgricultureFieldUse.Crop,
			Moisture = 50, Drainage = 50, Nutrients = 100, Topsoil = 50, Tilth = 50,
			Pasture = 50, Condition = 50, Definition = "<Field />",
			AgricultureFieldCrop = new MudSharp.Models.AgricultureFieldCrop
			{
				CropDefinitionId = 1, Stage = (int)AgricultureCropStage.Growing, GrowthDays = 10,
				Health = 50, YieldPotential = 50, Definition = "<Crop />"
			}
		};
		var field = new AgricultureField(model, world.World.Object);
		world.Fields.Add(field);
		world.Coordinator.FieldChanged(field);
		var apiary = new Mock<IAgricultureFieldApiary>();
		apiary.SetupGet(x => x.PollinationRadius).Returns(1);
		apiary.SetupGet(x => x.PollinationStrength).Returns(50);
		var pollinator = new Mock<IAgricultureField>();
		pollinator.SetupGet(x => x.Id).Returns(2L);
		pollinator.SetupGet(x => x.Cell).Returns(cell);
		pollinator.SetupGet(x => x.HasActiveApiary).Returns(true);
		var pollinationActive = true;
		pollinator.SetupGet(x => x.IsApiaryHappy).Returns(() => pollinationActive);
		pollinator.SetupGet(x => x.Apiary).Returns(apiary.Object);
		world.Coordinator.RefreshPollinationCandidate(pollinator.Object);
		return (field, active => pollinationActive = active);
	}

	[TestMethod]
	[TestCategory("Y-T01")]
	public void OrganicPenaltyIsNeutralUntilMatchingSourceIsAuthorisedAndDoesNotEvaluateManaOutputs()
	{
		using var world = new EnvironmentalMagicTestWorld(policy: "native-yield");
		world.Edit("organic penalty forage 1 - hasdefile * 0.75");
		var cell = (Cell)world.Cells.At(0);
		var context = new NativeOrganicPenaltyContext(NativeOrganicSourceKind.Forage, "forage:herbs",
			100.0, 0.0, 100.0, 100.0, 0.0, 10.0);
		var beforeCount = world.Profile.FormulaEvaluationCount;
		var neutral = world.Coordinator.EvaluateOrganicPenalty(cell,
			NativeOrganicPenaltyChannel.ForageReplenishment, context);
		Assert.AreEqual(NativeOrganicPenaltyEvaluation.Neutral, neutral);
		Assert.AreEqual(beforeCount, world.Profile.FormulaEvaluationCount);

		world.Edit("organic source add forage herbs");
		beforeCount = world.Profile.FormulaEvaluationCount;
		var undamaged = world.Coordinator.EvaluateOrganicPenalty(cell,
			NativeOrganicPenaltyChannel.ForageReplenishment, context);
		Assert.IsTrue(undamaged.IsValid, undamaged.Error);
		Assert.AreEqual(1.0, undamaged.Factor);
		Assert.AreEqual(beforeCount, world.Profile.FormulaEvaluationCount);
		var operation = world.Coordinator.ApplyOperation(cell, new EnvironmentalMagicOperationRequest(
			Guid.NewGuid(), 1, "organic penalty test", Damage: 1.0, Pressure: 0.0));
		Assert.IsTrue(operation.Success, operation.Error);
		beforeCount = world.Profile.FormulaEvaluationCount;
		var damaged = world.Coordinator.EvaluateOrganicPenalty(cell,
			NativeOrganicPenaltyChannel.ForageReplenishment, context);
		Assert.IsTrue(damaged.IsValid, damaged.Error);
		Assert.AreEqual(0.25, damaged.Factor, 1e-12);
		Assert.AreEqual(beforeCount, world.Profile.FormulaEvaluationCount,
			"Organic evaluation must not evaluate mana maximum/rate expressions.");
	}

	[TestMethod]
	[TestCategory("Y-T02")]
	public void OrganicPenaltyRespectsFieldUseAndDefinitionRestrictionsWithoutRequiringPositiveStock()
	{
		using var world = new EnvironmentalMagicTestWorld();
		var allowedCrop = new Mock<IAgricultureCropDefinition>();
		allowedCrop.SetupGet(x => x.Id).Returns(100L);
		allowedCrop.SetupGet(x => x.Name).Returns("Allowed Crop");
		var otherCrop = new Mock<IAgricultureCropDefinition>();
		otherCrop.SetupGet(x => x.Id).Returns(200L);
		otherCrop.SetupGet(x => x.Name).Returns("Other Crop");
		var definitions = new EnvironmentalMagicTestRegistry<IAgricultureCropDefinition>();
		definitions.Add(allowedCrop.Object);
		definitions.Add(otherCrop.Object);
		world.World.SetupGet(x => x.AgricultureCropDefinitions).Returns(definitions);
		world.Edit("organic source add crop");
		world.Edit("organic source crop uses orchard");
		world.Edit("organic source crop definitions 100");
		world.Edit("organic penalty cropinitial 0.25");
		var cell = (Cell)world.Cells.At(0);
		var use = AgricultureFieldUse.Crop;
		IAgricultureCropDefinition crop = allowedCrop.Object;
		var field = new Mock<IAgricultureField>();
		field.SetupGet(x => x.Id).Returns(50L);
		field.SetupGet(x => x.Cell).Returns(cell);
		field.SetupGet(x => x.CurrentUse).Returns(() => use);
		field.SetupGet(x => x.CurrentCrop).Returns(() => crop);
		world.Coordinator.FieldChanged(field.Object);
		var context = new NativeOrganicPenaltyContext(NativeOrganicSourceKind.Crop, "crop",
			0.0, 0.0, 0.0, 100.0, 50.0, 50.0);

		Assert.AreEqual(NativeOrganicPenaltyEvaluation.Neutral,
			world.Coordinator.EvaluateOrganicPenalty(cell, NativeOrganicPenaltyChannel.CropInitialisation, context));
		use = AgricultureFieldUse.Orchard;
		crop = otherCrop.Object;
		Assert.AreEqual(NativeOrganicPenaltyEvaluation.Neutral,
			world.Coordinator.EvaluateOrganicPenalty(cell, NativeOrganicPenaltyChannel.CropInitialisation, context));
		crop = allowedCrop.Object;
		var applicable = world.Coordinator.EvaluateOrganicPenalty(cell,
			NativeOrganicPenaltyChannel.CropInitialisation, context);
		Assert.IsTrue(applicable.IsValid, applicable.Error);
		Assert.AreEqual(0.25, applicable.Factor);
	}

	[TestMethod]
	[TestCategory("Y-T02")]
	[TestCategory("Y-T22")]
	public void UnrelatedOrganicErrorsDoNotDisableValidForageSourceOrPenalty()
	{
		using var world = new EnvironmentalMagicTestWorld(policy: "native-yield");
		var definition = EnvironmentalMagicTestWorld.ProfileDefinition();
		definition.Add(new XElement("Organic", new XAttribute("version", 1),
			new XElement("ProtectionProg", "malformed"),
			new XElement("Sources",
				new XElement("Source", new XAttribute("selector", "forage:herbs"),
					new XAttribute("kind", NativeOrganicSourceKind.Forage), new XAttribute("foragekey", "herbs"),
					new XElement("Uses"), new XElement("Definitions")),
				new XElement("Source", new XAttribute("selector", "crop"),
					new XAttribute("kind", NativeOrganicSourceKind.Crop),
					new XElement("Uses", new XElement("Use", AgricultureFieldUse.Woodland)),
					new XElement("Definitions"))),
			new XElement("Penalties",
				new XElement("Penalty", new XAttribute("channel", NativeOrganicPenaltyChannel.ForageReplenishment), "0.5"),
				new XElement("Penalty", new XAttribute("channel", NativeOrganicPenaltyChannel.CropYieldRecovery), "2"))));
		var profile = world.AddProfile(2, definition);
		var cell = (Cell)world.Cells.At(0);
		world.Coordinator.SetBinding(cell, EnvironmentalMagicBindingMode.Explicit, profile.Id);

		Assert.IsTrue(profile.OrganicValidationErrors.Count >= 3);
		Assert.AreEqual(NativeOrganicSourceStatus.Available,
			world.Coordinator.InspectOrganicSource(cell, "forage:herbs").Status);
		var evaluation = world.Coordinator.EvaluateOrganicPenalty(cell,
			NativeOrganicPenaltyChannel.ForageReplenishment,
			new NativeOrganicPenaltyContext(NativeOrganicSourceKind.Forage, "forage:herbs",
				100.0, 0.0, 100.0, 100.0, 0.0, 10.0));
		Assert.IsTrue(evaluation.IsValid, evaluation.Error);
		Assert.AreEqual(0.5, evaluation.Factor);
	}

	[TestMethod]
	[TestCategory("Y-T02")]
	[TestCategory("Y-T22")]
	public void PersistedLegacyInputCannotShadowOrganicBuiltInContext()
	{
		using var world = new EnvironmentalMagicTestWorld(policy: "native-yield");
		var definition = EnvironmentalMagicTestWorld.ProfileDefinition();
		definition.Element("Inputs")!.Add(EnvironmentalMagicTestWorld.Input(
			"nativestock", "Forage", "missing-yield"));
		definition.Add(new XElement("Organic", new XAttribute("version", 1),
			new XElement("Sources", new XElement("Source",
				new XAttribute("selector", "forage:herbs"),
				new XAttribute("kind", NativeOrganicSourceKind.Forage), new XAttribute("foragekey", "herbs"),
				new XElement("Uses"), new XElement("Definitions"))),
			new XElement("Penalties", new XElement("Penalty",
				new XAttribute("channel", NativeOrganicPenaltyChannel.ForageReplenishment),
				"nativestock / 200"))));
		var profile = world.AddProfile(2, definition);
		var cell = (Cell)world.Cells.At(0);
		world.Coordinator.SetBinding(cell, EnvironmentalMagicBindingMode.Explicit, profile.Id);

		var evaluation = world.Coordinator.EvaluateOrganicPenalty(cell,
			NativeOrganicPenaltyChannel.ForageReplenishment,
			new NativeOrganicPenaltyContext(NativeOrganicSourceKind.Forage, "forage:herbs",
				100.0, 0.0, 100.0, 100.0, 0.0, 10.0));

		Assert.IsTrue(evaluation.IsValid, evaluation.Error);
		Assert.AreEqual(0.5, evaluation.Factor);
	}

	[TestMethod]
	[TestCategory("Y-T02")]
	[TestCategory("Y-T03")]
	public void MalformedRelevantSelectorIsInvalidForInspectionAndPenaltyEvaluation()
	{
		using var world = new EnvironmentalMagicTestWorld(policy: "native-yield");
		var definition = EnvironmentalMagicTestWorld.ProfileDefinition();
		definition.Add(new XElement("Organic", new XAttribute("version", 1),
			new XElement("Sources", new XElement("Source",
				new XAttribute("selector", "forage: herbs"),
				new XAttribute("kind", NativeOrganicSourceKind.Forage), new XAttribute("foragekey", "herbs"),
				new XElement("Uses"), new XElement("Definitions"))),
			new XElement("Penalties", new XElement("Penalty",
				new XAttribute("channel", NativeOrganicPenaltyChannel.ForageReplenishment), "0.5"))));
		var profile = world.AddProfile(2, definition);
		var cell = (Cell)world.Cells.At(0);
		world.Coordinator.SetBinding(cell, EnvironmentalMagicBindingMode.Explicit, profile.Id);

		var snapshot = world.Coordinator.InspectOrganicSource(cell, "forage:herbs");
		Assert.AreEqual(NativeOrganicSourceStatus.Invalid, snapshot.Status);
		StringAssert.Contains(snapshot.Diagnostic, "canonical selector");
		var evaluation = world.Coordinator.EvaluateOrganicPenalty(cell,
			NativeOrganicPenaltyChannel.ForageReplenishment,
			new NativeOrganicPenaltyContext(NativeOrganicSourceKind.Forage, "forage:herbs",
				100.0, 0.0, 100.0, 100.0, 0.0, 10.0));
		Assert.IsFalse(evaluation.IsValid);
		Assert.AreEqual(0.0, evaluation.Factor);
		StringAssert.Contains(evaluation.Error, "canonical selector");
	}

	[TestMethod]
	[TestCategory("Y-T22")]
	public void InvalidDynamicOrganicPenaltyFailsClosedAndThrottlesDiagnostics()
	{
		using var world = new EnvironmentalMagicTestWorld(policy: "native-yield");
		world.Edit("organic source add forage herbs");
		world.Edit("organic penalty forage 1 - scardamage");
		var cell = (Cell)world.Cells.At(0);
		var operation = world.Coordinator.ApplyOperation(cell, new EnvironmentalMagicOperationRequest(
			Guid.NewGuid(), 1, "invalid organic factor test", Damage: 2.0));
		Assert.IsTrue(operation.Success, operation.Error);
		var context = new NativeOrganicPenaltyContext(NativeOrganicSourceKind.Forage, "forage:herbs",
			100.0, 0.0, 100.0, 100.0, 0.0, 10.0);

		for (var i = 0; i < 10; i++)
		{
			var evaluation = world.Coordinator.EvaluateOrganicPenalty(cell,
				NativeOrganicPenaltyChannel.ForageReplenishment, context);
			Assert.IsTrue(evaluation.IsConfigured);
			Assert.IsFalse(evaluation.IsValid);
			Assert.AreEqual(0.0, evaluation.Factor);
		}
		Assert.AreEqual(1, world.Messages.Count(x => x.Contains("Environmental organic penalty")));
		world.Clock.Advance(TimeSpan.FromSeconds(61));
		world.Coordinator.EvaluateOrganicPenalty(cell, NativeOrganicPenaltyChannel.ForageReplenishment, context);
		Assert.AreEqual(2, world.Messages.Count(x => x.Contains("Environmental organic penalty")));
	}

	[TestMethod]
	[TestCategory("Y-P04")]
	[TestCategory("Y-T21")]
	[TestCategory("Y-T23")]
	public void NativeForageRecovery_BuilderOptInScarsAndCoordinatorRepairAffectOnlyFutureRecovery()
	{
		using var configured = new EnvironmentalMagicTestWorld(policy: "native-yield");
		configured.Edit("organic source add forage herbs");
		configured.Edit("organic source add crop");
		configured.Edit("organic penalty forage 1 - scardamage / 20");
		var cell = (Cell)configured.Cells.At(0);

		cell.ConsumeYield("herbs", 60.0);
		Assert.AreEqual(40.0, cell.GetForagableYield("herbs"), 1e-9);
		Assert.AreEqual(NativeOrganicSourceStatus.Absent,
			configured.Coordinator.InspectOrganicSource(cell, "crop").Status);
		Assert.IsNull(configured.Coordinator.FieldFor(cell));
		Assert.AreEqual(0, configured.Fields.Count);

		configured.Heartbeat.ManuallyFireHeartbeatHour();
		var baselineRecovery = cell.GetForagableYield("herbs") - 40.0;
		Assert.AreEqual(10.0, baselineRecovery, 1e-9);

		var damage = configured.Coordinator.ApplyOperation(cell, Request(damage: 10.0));
		Assert.IsTrue(damage.Success, damage.Error);
		configured.Heartbeat.ManuallyFireHeartbeatHour();
		var damagedRecovery = cell.GetForagableYield("herbs") - 50.0;
		Assert.AreEqual(5.0, damagedRecovery, 1e-9);

		var forageBeforeRepair = cell.GetForagableYield("herbs");
		var manaBeforeRepair = configured.Balance();
		var fieldEnumerationsBeforeRepair = configured.Fields.Enumerations;
		configured.Fields.ForbidEnumeration = true;
		var repair = configured.Coordinator.ApplyOperation(cell, Request(repair: 10.0));
		Assert.IsTrue(repair.Success, repair.Error);
		Assert.AreEqual(10.0, repair.AppliedRepair, 1e-9);
		Assert.AreEqual(0.0, cell.EnvironmentState.ScarDamage, 1e-9);
		Assert.AreEqual(forageBeforeRepair, cell.GetForagableYield("herbs"), 1e-9,
			"Repair must not instantly restore forage stock.");
		Assert.AreEqual(manaBeforeRepair, configured.Balance(), 1e-9,
			"Repair must not fill environmental mana.");
		Assert.AreEqual(NativeOrganicSourceStatus.Absent,
			configured.Coordinator.InspectOrganicSource(cell, "crop").Status);
		Assert.IsNull(configured.Coordinator.FieldFor(cell));
		Assert.AreEqual(0, configured.Fields.Count);
		Assert.AreEqual(fieldEnumerationsBeforeRepair, configured.Fields.Enumerations);

		configured.Heartbeat.ManuallyFireHeartbeatHour();
		var repairedRecovery = cell.GetForagableYield("herbs") - forageBeforeRepair;
		Assert.AreEqual(10.0, repairedRecovery, 1e-9);
		Assert.IsTrue(repairedRecovery > damagedRecovery);
		Assert.AreEqual(manaBeforeRepair, configured.Balance(), 1e-9);

		using var unconfigured = new EnvironmentalMagicTestWorld(policy: "native-yield");
		var unconfiguredCell = (Cell)unconfigured.Cells.At(0);
		unconfiguredCell.ConsumeYield("herbs", 60.0);
		Assert.IsTrue(unconfigured.Coordinator.ApplyOperation(unconfiguredCell, Request(damage: 10.0)).Success);
		unconfigured.Heartbeat.ManuallyFireHeartbeatHour();
		Assert.AreEqual(50.0, unconfiguredCell.GetForagableYield("herbs"), 1e-9,
			"A world without organic opt-in must retain baseline forage recovery despite scars.");
		var unconfiguredRepair = unconfigured.Coordinator.ApplyOperation(unconfiguredCell, Request(repair: 10.0));
		Assert.IsTrue(unconfiguredRepair.Success, unconfiguredRepair.Error);
		Assert.AreEqual(50.0, unconfiguredCell.GetForagableYield("herbs"), 1e-9);
		Assert.AreEqual(0.0, unconfigured.Balance(), 1e-9);
		unconfigured.Heartbeat.ManuallyFireHeartbeatHour();
		Assert.AreEqual(60.0, unconfiguredCell.GetForagableYield("herbs"), 1e-9);
	}

	[TestMethod]
	[TestCategory("Y-T25")]
	public void NativeOrganicSnapshotDebitAndRecovery_ThirtyThousandCellsRemainIndexedAndSubscriptionNeutral()
	{
		using var world = new EnvironmentalMagicTestWorld(30000, activePercent: 0.0,
			policy: "native-yield", options: Options(visits: 16));
		world.Edit("organic source add forage herbs");
		world.Edit("organic penalty forage 1");
		var cell = (Cell)world.Cells.At(29999);
		cell.ConsumeYield("herbs", 10.0);

		var cellEnumerations = world.Cells.Enumerations;
		var fieldEnumerations = world.Fields.Enumerations;
		var secondSubscriptions = world.SecondSubscriptions;
		var hourSubscriptions = EnvironmentalMagicTestWorld.InvocationCount(world.Heartbeat, "_hourHeartbeat");
		var profileDelegates = world.EnvironmentalDelegateCount;
		var schedulerEntries = world.SchedulerEntries;
		world.Cells.ForbidEnumeration = true;
		world.Fields.ForbidEnumeration = true;

		var snapshot = world.Coordinator.InspectOrganicSource(cell, "forage:herbs");
		Assert.AreEqual(NativeOrganicSourceStatus.Available, snapshot.Status);
		Assert.AreEqual(90.0, snapshot.NativeStock, 1e-9);
		Assert.IsTrue(world.Coordinator.TryPlanOrganicDebit(cell, "forage:herbs", 0.25,
			out var plan, out var error), error);
		Assert.IsTrue(world.Coordinator.TryApplyOrganicDebit(cell, plan, out var afterDebit, out error), error);
		Assert.AreEqual(89.75, afterDebit.NativeStock, 1e-9);
		world.Heartbeat.ManuallyFireHeartbeatHour();
		Assert.IsTrue(cell.TryPeekForagableYield("herbs", out NativeForageYieldSnapshot afterRecovery));
		Assert.AreEqual(99.75, afterRecovery.Stock, 1e-9);

		Assert.AreEqual(cellEnumerations, world.Cells.Enumerations);
		Assert.AreEqual(fieldEnumerations, world.Fields.Enumerations);
		Assert.AreEqual(secondSubscriptions, world.SecondSubscriptions);
		Assert.AreEqual(hourSubscriptions,
			EnvironmentalMagicTestWorld.InvocationCount(world.Heartbeat, "_hourHeartbeat"));
		Assert.AreEqual(profileDelegates, world.EnvironmentalDelegateCount);
		Assert.AreEqual(schedulerEntries, world.SchedulerEntries);
	}

	[TestMethod]
	[TestCategory("Y-T26")]
	public void NativePenaltyDependencies_RejectCapRecursionApplyOnceAndCoalesceProfileInvalidation()
	{
		const int cellCount = 512;
		const int visitBudget = 8;
		using var world = new EnvironmentalMagicTestWorld(cellCount, activePercent: 0.0,
			policy: "native-yield", options: Options(visits: visitBudget));
		world.Edit("organic source add forage herbs");
		world.Edit("organic penalty forage native / 100");
		var cell = (Cell)world.Cells.At(cellCount - 1);
		var resource = world.Resources.Get(1)!;
		cell.ConsumeYield("herbs", 60.0);
		Assert.AreEqual(40.0, resource.ResourceCap(cell), 1e-9);
		var outputEvaluations = world.Profile.FormulaEvaluationCount;

		world.Heartbeat.ManuallyFireHeartbeatHour();
		Assert.AreEqual(44.0, cell.GetForagableYield("herbs"), 1e-9,
			"The 0.4 native dependency factor must be applied exactly once to the 10-point baseline recovery.");
		Assert.AreEqual(outputEvaluations, world.Profile.FormulaEvaluationCount,
			"Organic recovery must not recurse into environmental cap/rate output evaluation.");
		Assert.AreEqual(44.0, resource.ResourceCap(cell), 1e-9);

		world.CompileProg(50, "return environmentcap(@where, 1)", "RecursiveNativePenalty");
		world.Edit("input recursive prog 50 1");
		world.Edit("organic penalty forage recursive / 100");
		var beforeRecursiveRecovery = cell.GetForagableYield("herbs");
		world.Heartbeat.ManuallyFireHeartbeatHour();
		Assert.AreEqual(beforeRecursiveRecovery, cell.GetForagableYield("herbs"), 1e-9,
			"A cap-recursive native dependency must fail closed without recovering stock.");
		Assert.IsTrue(world.Messages.Any(x => x.Contains("Input prog #50 failed", StringComparison.OrdinalIgnoreCase)),
			string.Join("; ", world.Messages));

		var evaluationsBeforeEdits = world.Coordinator.Diagnostics.TotalEvaluations;
		world.Cells.ForbidEnumeration = true;
		world.Fields.ForbidEnumeration = true;
		world.Edit("organic penalty forage native / 200");
		world.Edit("organic penalty forage native / 150");
		world.Edit("organic penalty forage native / 100");
		Assert.AreEqual(cellCount, world.Coordinator.Diagnostics.DiscoveryRemaining,
			"Repeated edits to one shared profile must coalesce into one bounded discovery pass.");
		Assert.AreEqual(evaluationsBeforeEdits, world.Coordinator.Diagnostics.TotalEvaluations);
		Assert.IsTrue(world.Coordinator.Diagnostics.Dirty <= 1);
		world.Tick();
		Assert.IsTrue(world.Coordinator.Diagnostics.LastCellVisits <= visitBudget);
		Assert.IsTrue(world.Coordinator.Diagnostics.DiscoveryRemaining >= cellCount - visitBudget);
	}

	[TestMethod]
	[TestCategory("E-P01")]
	[TestCategory("E-P02")]
	[TestCategory("E-P03")]
	[TestCategory("E-P22")]
	public void ThirtyThousandFullCells_UseOneHeartbeatAndOnlyBoundedIdleReconciliation()
	{
		using var world = new EnvironmentalMagicTestWorld(30000, activePercent: 0.0);
		var entries = world.SchedulerEntries;
		foreach (var cell in world.Cells) world.Coordinator.Register(cell);
		world.Coordinator.Initialise();
		Assert.AreEqual(1, world.SecondSubscriptions);
		Assert.AreEqual(1, entries);
		Assert.AreEqual(entries, world.SchedulerEntries);
		Assert.AreEqual(0, world.EnvironmentalDelegateCount);
		Assert.AreEqual(30000, world.Coordinator.Diagnostics.Configured);
		world.RunSeconds(120);
		Assert.AreEqual(30000, world.Coordinator.Diagnostics.Dormant);
		Assert.AreEqual(0, world.Coordinator.Diagnostics.ProductionQueue);
		world.ResetSavedFlags();
		world.Cells.ForbidEnumeration = true;
		world.Fields.ForbidEnumeration = true;
		var before = world.Coordinator.Diagnostics;
		for (var i = 0; i < 3600; i++)
		{
			world.Tick();
			Assert.IsTrue(world.Coordinator.Diagnostics.LastCellVisits <= 1024);
		}
		var after = world.Coordinator.Diagnostics;
		Assert.IsTrue(after.TotalEvaluations - before.TotalEvaluations <= 31024,
			$"Idle rooms were evaluated {after.TotalEvaluations - before.TotalEvaluations} times in one audit cycle.");
		Assert.AreEqual(before.TotalWrites, after.TotalWrites);
		Assert.AreEqual(0, world.SaveRequests);
		Assert.AreEqual(0, world.Operations.Reads);
		Assert.AreEqual(0, world.Operations.Commits);
		Assert.AreEqual(0, world.Flushes);
		Assert.AreEqual(entries, world.SchedulerEntries);
		world.Dispose();
		Assert.AreEqual(0, world.SecondSubscriptions);
		Assert.AreEqual(0, world.Coordinator.Diagnostics.Configured);
	}

	[TestMethod]
	[TestCategory("E-P04")]
	public void ThreeHundredActiveOfThirtyThousand_VisitsScaleWithUsefulProduction()
	{
		using var world = new EnvironmentalMagicTestWorld(30000, activePercent: 1.0);
		world.RunSeconds(120);
		Assert.AreEqual(300, world.Coordinator.Diagnostics.ActiveProduction);
		Assert.AreEqual(300, world.Coordinator.Diagnostics.ProductionQueue);
		var before = world.Coordinator.Diagnostics;
		world.Cells.ForbidEnumeration = true;
		world.RunSeconds(600);
		var evaluations = world.Coordinator.Diagnostics.TotalEvaluations - before.TotalEvaluations;
		// Ten active cadence updates plus one sixth of the hourly audit population, with one bounded slice of slack.
		Assert.IsTrue(evaluations <= 300 * 10 + 30000 / 6 + 1024,
			$"The 600-second sample performed {evaluations} input snapshots for 300 active cells plus hourly audits.");
		Assert.IsTrue(world.Balance() > 9.0);
		Assert.AreEqual(100.0, world.Balance(1000));
	}

	[TestMethod]
	[TestCategory("E-T03")]
	[TestCategory("E-T10")]
	[TestCategory("E-P05")]
	public void ThirtyThousandEmptyCells_MakeProgressThroughBoundedSlicesWithoutOccupants()
	{
		using var world = new EnvironmentalMagicTestWorld(30000, missingBalances: true,
			options: Options(visits: 256));
		Assert.AreEqual(0, ((Cell)world.Cells.At(0)).MagicResourceAmounts.Count);
		for (var i = 0; i < 360; i++)
		{
			world.Tick();
			Assert.IsTrue(world.Coordinator.Diagnostics.LastCellVisits <= 256);
		}
		Assert.AreEqual(30000, world.Coordinator.Diagnostics.ActiveProduction);
		Assert.IsTrue(world.Cells.Cast<Cell>().All(cell => cell.MagicResourceAmounts[world.Resources.Get(1)!] > 0.0));
		Assert.IsTrue(world.Coordinator.Diagnostics.BudgetLimitedPumps > 0);
		Assert.AreEqual(0, world.EnvironmentalDelegateCount);
		Assert.AreEqual(1, world.SecondSubscriptions);
	}

	[TestMethod]
	[TestCategory("E-P05")]
	public void OutputWorkBudget_CarriesForwardACompleteMultiOutputCellWithoutOvershooting()
	{
		using var world = new EnvironmentalMagicTestWorld(20, outputs: 3,
			options: Options() with { MaximumOutputWork = 8 });
		world.Tick(60);
		Assert.IsTrue(world.Coordinator.Diagnostics.LastCellVisits <= 2,
			"An eight-output pump budget cannot admit three three-output cells.");
		for (var i = 0; i < 20; i++)
		{
			world.Tick();
			Assert.IsTrue(world.Coordinator.Diagnostics.LastCellVisits <= 2);
		}
		Assert.AreEqual(20, world.Coordinator.Diagnostics.ActiveProduction);
	}

	[TestMethod]
	[TestCategory("E-P06")]
	[TestCategory("E-P24")]
	public void ContinuouslyDirtyCell_CoalescesAndCannotStarveOtherProductionOrAudits()
	{
		using var world = new EnvironmentalMagicTestWorld(300, activePercent: 2.0,
			options: Options(visits: 8, audit: 120));
		foreach (var cell in world.Cells) cell.AddResource(world.Resources.Get(1)!, 0.0);
		var flood = (Cell)world.Cells.At(0);
		for (var second = 0; second < 250; second++)
		{
			for (var i = 0; i < 1000; i++) world.Coordinator.MarkDirty(flood, EnvironmentalMagicDirtyReason.Policy);
			Assert.AreEqual(1, world.Coordinator.Diagnostics.Dirty);
			world.Tick();
			Assert.IsTrue(world.Coordinator.Diagnostics.LastCellVisits <= 8);
			Assert.IsTrue(world.Coordinator.Diagnostics.ProductionQueue <= 6);
		}
		Assert.IsTrue(world.Balance(1) >= 3.0);
		Assert.IsTrue(world.Coordinator.Diagnostics.OldestAuditSeconds < 240.0);
		Assert.AreEqual(300, world.Coordinator.Diagnostics.AuditQueue);
	}

	[TestMethod]
	[TestCategory("E-T04")]
	[TestCategory("E-P07")]
	public void DirectAndCompiledFutureProgMutations_UseOneCurrentMaximumAndUpdateEligibility()
	{
		using var world = new EnvironmentalMagicTestWorld();
		var cell = (Cell)world.Cells.At(0);
		var resource = world.Resources.Get(1)!;
		cell.AddResource(resource, 150.0);
		Assert.AreEqual(100.0, resource.ResourceCap(cell));
		Assert.AreEqual(100.0, world.Balance());
		Assert.AreEqual(0, world.Coordinator.Diagnostics.ActiveProduction);
		var subtract = world.CompileProg(10, "return subtractmagicresource(@where, 1, 25)");
		Assert.AreEqual(75.0, subtract.ExecuteDouble(cell));
		Assert.AreEqual(1, world.Coordinator.Diagnostics.ActiveProduction);
		var add = world.CompileProg(11, "return addmagicresource(@where, 1, 100)");
		Assert.AreEqual(100.0, add.ExecuteDouble(cell));
		Assert.AreEqual(0, world.Coordinator.Diagnostics.ActiveProduction);
		var set = world.CompileProg(12, "return setmagicresource(@where, 1, 10)");
		Assert.AreEqual(10.0, set.ExecuteDouble(cell));
		Assert.AreEqual(1, world.Coordinator.Diagnostics.ActiveProduction);
		world.Tick(60);
		Assert.AreEqual(11.0, world.Balance(), 1e-9);
	}

	[TestMethod]
	[TestCategory("E-P08")]
	public void NaturalProductionFillsTheLastUnit_ThenLeavesOnlyDormantReconciliation()
	{
		using var world = new EnvironmentalMagicTestWorld(activePercent: 0.0);
		var cell = (Cell)world.Cells.At(0);
		Assert.IsTrue(cell.UseResource(world.Resources.Get(1)!, 1.0));
		Assert.AreEqual(1, world.Coordinator.Diagnostics.ActiveProduction);
		world.Tick(60);
		Assert.AreEqual(100.0, world.Balance());
		Assert.AreEqual(0, world.Coordinator.Diagnostics.ActiveProduction);
		Assert.AreEqual(0, world.Coordinator.Diagnostics.ProductionQueue);
		Assert.AreEqual(1, world.Coordinator.Diagnostics.Dormant);
		Assert.AreEqual(1, world.Coordinator.Diagnostics.AuditQueue);
	}

	[TestMethod]
	[TestCategory("E-P06")]
	[TestCategory("E-P24")]
	public void ContinuousDirtyAndNoOpMutations_PreserveTheExistingProductionDeadline()
	{
		using var world = new EnvironmentalMagicTestWorld();
		var cell = (Cell)world.Cells.At(0);
		var resource = world.Resources.Get(1)!;
		cell.AddResource(resource, 0.0);
		for (var second = 1; second < 60; second++)
		{
			world.Coordinator.MarkDirty(cell, EnvironmentalMagicDirtyReason.Policy);
			world.Tick();
			cell.AddResource(resource, 0.0);
			world.Coordinator.Register(cell);
		}
		Assert.AreEqual(59.0 / 60.0, world.Balance(), 1e-8);
		var evaluations = world.Coordinator.Diagnostics.TotalEvaluations;
		world.Tick();
		Assert.AreEqual(evaluations + 1, world.Coordinator.Diagnostics.TotalEvaluations);
		Assert.AreEqual(1.0, world.Balance(), 1e-8);
	}

	[TestMethod]
	[TestCategory("E-P08")]
	public void FullRoomIdleForAnHour_ThenDepleted_EarnsOnlyAfterItsDebit()
	{
		using var world = new EnvironmentalMagicTestWorld(activePercent: 0.0);
		var cell = (Cell)world.Cells.At(0);
		var resource = world.Resources.Get(1)!;
		cell.AddResource(resource, 0.0);
		world.Tick(3600);
		Assert.IsTrue(cell.UseResource(resource, 10.0));
		Assert.AreEqual(90.0, world.Balance());
		for (var i = 0; i < 60; i++)
		{
			world.Tick();
			cell.AddResource(resource, 0.0);
			world.Coordinator.Register(cell);
		}
		Assert.AreEqual(91.0, world.Balance(), 1e-8);
		Assert.AreEqual(1, world.SecondSubscriptions);
	}

	[TestMethod]
	[TestCategory("E-P09")]
	[TestCategory("E-P11")]
	public void UnobservableZeroRateProgChange_WakesOnAuditWithoutHistoricalCredit()
	{
		using var world = new EnvironmentalMagicTestWorld(options: Options(audit: 120));
		world.CompileProg(10, "if (@where.name == \"Awakened Cell\")\nreturn 1\nend if\nreturn 0");
		world.Edit("input policy prog 10 1");
		world.Edit("output 1 rate policy");
		var cell = (Cell)world.Cells.At(0);
		cell.AddResource(world.Resources.Get(1)!, 0.0);
		world.Tick(3600);
		Assert.AreEqual(0.0, world.Balance());
		Assert.AreEqual(0, world.Coordinator.Diagnostics.ActiveProduction);
		// The unchanged compiled prog reads a mutable property whose setter has no environmental notification.
		var before = world.Coordinator.Diagnostics;
		((IEditableCellOverlay)cell.CurrentOverlay).CellName = "Awakened Cell";
		Assert.AreEqual(before.Dirty, world.Coordinator.Diagnostics.Dirty);
		Assert.AreEqual(before.DiscoveryRemaining, world.Coordinator.Diagnostics.DiscoveryRemaining);
		for (var second = 0; second < 120 && world.Coordinator.Diagnostics.ActiveProduction == 0; second++) world.Tick();
		Assert.AreEqual(0.0, world.Balance());
		Assert.AreEqual(1, world.Coordinator.Diagnostics.ActiveProduction);
		world.Tick(60);
		Assert.AreEqual(1.0, world.Balance(), 1e-9);
	}

	[TestMethod]
	[TestCategory("E-P11")]
	public void UnobservableCompiledCapDependencyChange_IsEnforcedByAMutationBeforeAnyAudit()
	{
		using var world = new EnvironmentalMagicTestWorld(activePercent: 0.0);
		world.CompileProg(10, "if (@where.name == \"Diminished Cell\")\nreturn 20\nend if\nreturn 100");
		world.Edit("input policy prog 10 1");
		world.Edit("output 1 maximum policy");
		var cell = (Cell)world.Cells.At(0);
		var resource = world.Resources.Get(1)!;
		cell.AddResource(resource, 0.0);
		world.Tick(0);
		((IEditableCellOverlay)cell.CurrentOverlay).CellName = "Diminished Cell";
		Assert.AreEqual(0, world.Coordinator.Diagnostics.Dirty);
		Assert.AreEqual(0, world.Coordinator.Diagnostics.DiscoveryRemaining);
		Assert.AreEqual(20.0, resource.ResourceCap(cell));
		Assert.AreEqual(100.0, world.Balance());
		Assert.IsFalse(cell.UseResource(resource, 21.0));
		Assert.AreEqual(20.0, world.Balance());
		Assert.IsTrue(cell.UseResource(resource, 5.0));
		Assert.AreEqual(15.0, world.Balance());
		Assert.AreEqual(1, world.Coordinator.Diagnostics.ActiveProduction);
	}

	[TestMethod]
	[TestCategory("E-T05")]
	[TestCategory("E-T06")]
	[TestCategory("E-P10")]
	public void NativeForageConsumptionAndRecovery_ClampAtMutationAndDoNotRefillMana()
	{
		using var world = new EnvironmentalMagicTestWorld(activePercent: 0.0, policy: "native-yield");
		var cell = (Cell)world.Cells.At(0);
		var resource = world.Resources.Get(1)!;
		cell.AddResource(resource, 0.0);
		cell.ConsumeYield("herbs", 60.0);
		Assert.AreEqual(1, world.Coordinator.Diagnostics.Dirty);
		Assert.AreEqual(40.0, resource.ResourceCap(cell));
		Assert.AreEqual(100.0, world.Balance());
		Assert.IsFalse(cell.CanUseResource(resource, 41.0));
		cell.AddResource(resource, 0.0);
		Assert.AreEqual(40.0, world.Balance());
		world.Heartbeat.ManuallyFireHeartbeatHour();
		Assert.AreEqual(50.0, resource.ResourceCap(cell));
		Assert.AreEqual(40.0, world.Balance());
		cell.AddResource(resource, 0.0);
		Assert.AreEqual(40.0, world.Balance());
	}

	[TestMethod]
	[TestCategory("E-T07")]
	[TestCategory("E-P13")]
	public void PureInspections_DoNotInitialiseBalancesRegisterWorkSaveOrMoveProductionEpoch()
	{
		using var world = new EnvironmentalMagicTestWorld(missingBalances: true, start: false);
		var cell = (Cell)world.Cells.At(0);
		var resource = world.Resources.Get(1)!;
		for (var i = 0; i < 100; i++)
		{
			world.Clock.Advance(TimeSpan.FromSeconds(1));
			Assert.AreEqual(100.0, resource.ResourceCap(cell));
			Assert.IsFalse(cell.CanUseResource(resource, 1.0));
			Assert.IsTrue(world.Coordinator.Inspect(cell).IsValid);
		}
		Assert.AreEqual(0, cell.MagicResourceAmounts.Count);
		Assert.AreEqual(0, world.Coordinator.Diagnostics.Configured);
		Assert.AreEqual(0, world.Coordinator.Diagnostics.ProductionQueue);
		Assert.AreEqual(0, world.SaveRequests);
		Assert.AreEqual(EnvironmentalMagicState.Empty, cell.EnvironmentState);
		world.Start();
		cell.AddResource(resource, 0.0);
		world.Clock.Advance(TimeSpan.FromSeconds(60));
		for (var i = 0; i < 50; i++) _ = world.Coordinator.Inspect(cell);
		Assert.AreEqual(0.0, world.Balance());
		world.Tick(0);
		Assert.AreEqual(1.0, world.Balance(), 1e-9);
	}

	[TestMethod]
	[TestCategory("E-P14")]
	[TestCategory("E-P15")]
	public void DelayedAdvancementAndUtcClockJumps_UseMonotonicAcceptedSegmentsExactlyOnce()
	{
		using var world = new EnvironmentalMagicTestWorld();
		var cell = (Cell)world.Cells.At(0);
		var resource = world.Resources.Get(1)!;
		cell.AddResource(resource, 0.0);
		world.Clock.Advance(TimeSpan.FromSeconds(120));
		world.Clock.SetUtcNow(world.Clock.GetUtcNow().AddYears(1));
		Assert.IsFalse(cell.CanUseResource(resource, 1.0));
		world.Tick(0);
		Assert.AreEqual(2.0, world.Balance(), 1e-9);
		world.Tick(0);
		Assert.AreEqual(2.0, world.Balance(), 1e-9);
		world.Clock.SetUtcNow(world.Clock.GetUtcNow().AddYears(-2));
		world.Edit("output 1 baserate 2");
		cell.AddResource(resource, 0.0);
		world.Tick(60);
		Assert.AreEqual(4.0, world.Balance(), 1e-9);
	}

	[TestMethod]
	[TestCategory("E-P14")]
	public void RateEditDuringAnUnaccountedInterval_AppliesNewRateFromTheNextAcceptedBoundary()
	{
		using var world = new EnvironmentalMagicTestWorld();
		var cell = (Cell)world.Cells.At(0);
		var resource = world.Resources.Get(1)!;
		cell.AddResource(resource, 0.0);
		world.Clock.Advance(TimeSpan.FromSeconds(30));
		world.Edit("output 1 baserate 10");
		Assert.AreEqual(0.0, world.Balance());
		Assert.AreEqual(10.0, world.Coordinator.Inspect(cell).Outputs[0].Rate);
		world.Clock.Advance(TimeSpan.FromSeconds(30));
		cell.AddResource(resource, 0.0);
		Assert.AreEqual(1.0, world.Balance(), 1e-9);
		world.Tick(60);
		Assert.AreEqual(11.0, world.Balance(), 1e-9);
	}

	[TestMethod]
	[TestCategory("E-T08")]
	[TestCategory("E-P19")]
	public void InvalidAndRecursiveInputs_PreserveStockRefuseSpendsAndRepairWithoutBackCredit()
	{
		using var world = new EnvironmentalMagicTestWorld();
		var cell = (Cell)world.Cells.At(0);
		var resource = world.Resources.Get(1)!;
		cell.AddResource(resource, 50.0);
		var prog = world.CompileProg(10, "return 0");
		world.Edit("input divisor prog 10 1");
		world.Edit("output 1 maximum basecapacity / divisor");
		Assert.IsTrue(double.IsNaN(resource.ResourceCap(cell)));
		Assert.IsFalse(cell.UseResource(resource, 1.0));
		cell.AddResource(resource, 1.0);
		cell.AddResource(resource, double.NaN);
		Assert.AreEqual(50.0, world.Balance());
		world.Tick(3600);
		Assert.AreEqual(50.0, world.Balance());
		prog.FunctionText = "return 1";
		Assert.IsTrue(prog.Compile());
		world.Coordinator.SourceDefinitionChanged();
		world.Tick();
		Assert.AreEqual(50.0, world.Balance());
		world.Tick(60);
		Assert.AreEqual(51.0, world.Balance(), 1e-9);
		prog.FunctionText = "return environmentcap(@where, 1)";
		Assert.IsTrue(prog.Compile(), prog.CompileError);
		Assert.IsFalse(world.Coordinator.Inspect(cell).IsValid);
		Assert.IsFalse(world.Coordinator.TryDebit(cell, resource, 1.0, out _));
		Assert.AreEqual(51.0, world.Balance(), 1e-9);
	}

	[TestMethod]
	[TestCategory("E-P19")]
	public void WidelyUsedInvalidProfile_LogsOnceAndCannotPreventHealthyCellsAdvancing()
	{
		using var world = new EnvironmentalMagicTestWorld(200, activePercent: 0.0);
		var healthyDefinition = EnvironmentalMagicTestWorld.ProfileDefinition();
		var healthyProfile = world.AddProfile(2, healthyDefinition);
		var healthy = (Cell)world.Cells.At(199);
		world.Coordinator.SetBinding(healthy, EnvironmentalMagicBindingMode.Explicit, healthyProfile.Id);
		healthy.UseResource(world.Resources.Get(1)!, 10.0);
		var invalid = EnvironmentalMagicTestWorld.ProfileDefinition(maximum: "-1");
		world.AddProfile(1, invalid);
		world.Coordinator.SourceDefinitionChanged();
		world.Tick();
		Assert.AreEqual(199, world.Coordinator.Diagnostics.Faulted);
		Assert.AreEqual(1, world.Messages.Count(message => message.StartsWith("Environmental magic:", StringComparison.Ordinal)));
		Assert.AreEqual(100.0, world.Balance());
		world.Tick(60);
		Assert.AreEqual(90.0 + 61.0 / 60.0, healthy.MagicResourceAmounts[world.Resources.Get(1)!], 1e-9);
		world.AddProfile(1, EnvironmentalMagicTestWorld.ProfileDefinition());
		world.Coordinator.SourceDefinitionChanged();
		world.Tick();
		Assert.AreEqual(0, world.Coordinator.Diagnostics.Faulted);
		Assert.AreEqual(100.0, world.Balance());
	}

	[TestMethod]
	[TestCategory("E-P12")]
	[TestCategory("E-P16")]
	public void FieldIndex_TracksAbsenceReplacementDeletionAndSharesOnlyNeededValues()
	{
		using var world = new EnvironmentalMagicTestWorld(outputs: 3);
		var cell = (Cell)world.Cells.At(0);
		world.Edit("input present agriculture hasfield 1");
		world.Edit("input health agriculture crophealth 0.5");
		foreach (var resource in world.Resources) world.Edit($"output {resource.Id} maximum present * 10 + health");
		world.Fields.ForbidEnumeration = true;
		Assert.AreEqual(0.0, world.Coordinator.Inspect(cell).Outputs[0].Maximum);
		var field = new Mock<IAgricultureField>();
		field.SetupGet(value => value.Id).Returns(1);
		field.SetupGet(value => value.Cell).Returns(cell);
		field.SetupGet(value => value.CurrentCrop).Returns(Mock.Of<IAgricultureCropDefinition>());
		field.SetupGet(value => value.CropHealth).Returns(80);
		world.Fields.Add(field.Object);
		world.Coordinator.FieldChanged(field.Object);
		field.Invocations.Clear();
		var snapshot = world.Coordinator.Inspect(cell);
		Assert.IsTrue(snapshot.Outputs.All(output => output.Maximum == 50.0));
		field.VerifyGet(value => value.CropHealth, Times.Once);
		var replacement = new Mock<IAgricultureField>();
		replacement.SetupGet(value => value.Id).Returns(2);
		replacement.SetupGet(value => value.Cell).Returns(cell);
		world.Coordinator.FieldChanged(replacement.Object);
		Assert.AreEqual(10.0, world.Coordinator.Inspect(cell).Outputs[0].Maximum);
		world.Coordinator.FieldChanged(field.Object, removed: true);
		Assert.AreSame(replacement.Object, world.Coordinator.FieldFor(cell));
		world.Coordinator.FieldChanged(replacement.Object, removed: true);
		Assert.IsNull(world.Coordinator.FieldFor(cell));
		Assert.AreEqual(0.0, world.Coordinator.Inspect(cell).Outputs[0].Maximum);
	}

	[TestMethod]
	[TestCategory("E-P16")]
	public void MultipleOutputs_ExecuteEachNeededCompiledInputOnceAndSkipUnusedBindings()
	{
		using var world = new EnvironmentalMagicTestWorld(outputs: 3, policy: "compiled-prog");
		world.CompileProg(2, "return 1 / 0");
		world.Edit("input unused prog 2 1");
		world.Edit("input unusedfield agriculture crophealth 1");
		var before = world.Coordinator.Diagnostics.TotalInputProgExecutions;
		var snapshot = world.Coordinator.Inspect(world.Cells.At(0));
		Assert.IsTrue(snapshot.IsValid, string.Join("; ", snapshot.Errors));
		Assert.AreEqual(3, snapshot.Outputs.Count);
		Assert.AreEqual(before + 1, world.Coordinator.Diagnostics.TotalInputProgExecutions);
		Assert.IsFalse(snapshot.Inputs.ContainsKey("unused"));
		Assert.IsFalse(snapshot.Inputs.ContainsKey("unusedfield"));
	}

	[TestMethod]
	[TestCategory("E-P05")]
	[TestCategory("E-P19")]
	public void SynchronousProgExceedingTheSoftBudget_StopsAfterOneCellAndThrottlesProfileLogs()
	{
		using var world = new EnvironmentalMagicTestWorld(100, policy: "compiled-prog", options: Options() with
		{
			SoftBudgetMilliseconds = double.Epsilon, SlowProgMilliseconds = double.Epsilon
		});
		world.Tick(60);
		Assert.AreEqual(1, world.Coordinator.Diagnostics.LastCellVisits);
		Assert.IsTrue(world.Coordinator.Diagnostics.BudgetLimitedPumps > 0);
		for (var pump = 0; pump < 10; pump++)
		{
			world.Tick(0);
			Assert.AreEqual(1, world.Coordinator.Diagnostics.LastCellVisits);
		}
		Assert.AreEqual(11, world.Coordinator.Diagnostics.TotalInputProgExecutions);
		Assert.AreEqual(11, world.Coordinator.Diagnostics.TotalSlowInputProgs);
		Assert.AreEqual(1, world.Messages.Count(message => message.StartsWith("Environmental magic slow input:", StringComparison.Ordinal)));
		StringAssert.Contains(world.Coordinator.Diagnostics.SlowProg!, "Profile #1, prog #1");
		StringAssert.Contains(world.Coordinator.Diagnostics.SlowProg!, "synchronous; not preempted");
	}

	[TestMethod]
	[TestCategory("E-T07")]
	[TestCategory("E-P13")]
	public void PolicyProgCannotDirtyItsEnvironmentDuringAPureCapInspection()
	{
		using var world = new EnvironmentalMagicTestWorld();
		world.CompileProg(10, "if (invalidateenvironment(@where))\nreturn 100\nend if\nreturn 100");
		world.Edit("input policy prog 10 1");
		world.Edit("output 1 maximum policy");
		var before = world.Coordinator.Diagnostics;
		var result = world.Coordinator.Inspect(world.Cells.At(0));
		Assert.IsFalse(result.IsValid);
		Assert.AreEqual(before.Dirty, world.Coordinator.Diagnostics.Dirty);
		Assert.AreEqual(before.ProductionQueue, world.Coordinator.Diagnostics.ProductionQueue);
		Assert.AreEqual(0.0, world.Balance());
	}

	[TestMethod]
	[TestCategory("E-T07")]
	[TestCategory("E-P16")]
	public void SeveralOutputs_UseOneUtcInstantForPressureAndEventAge()
	{
		using var world = new EnvironmentalMagicTestWorld(outputs: 3);
		world.Edit("halflife 60");
		foreach (var resource in world.Resources)
		{
			world.Edit($"output {resource.Id} maximum pressure + 100 * minutessincedefile");
			world.Edit($"output {resource.Id} rate minutessincedefile");
		}
		var cell = (Cell)world.Cells.At(0);
		Assert.IsTrue(world.Coordinator.ApplyOperation(cell, Request(pressure: 100)).Success);
		world.Clock.SetUtcNow(cell.EnvironmentState.LastDefileUtc!.Value.AddMinutes(1));
		world.Clock.UtcReadStep = TimeSpan.FromSeconds(30);
		var reads = world.Clock.UtcReads;
		var snapshot = world.Coordinator.Inspect(cell);
		Assert.AreEqual(reads + 1, world.Clock.UtcReads);
		Assert.IsTrue(snapshot.IsValid, string.Join("; ", snapshot.Errors));
		Assert.AreEqual(50.0, snapshot.Pressure, 1e-9);
		Assert.AreEqual(1.0, snapshot.Inputs["minutessincedefile"]);
		foreach (var output in snapshot.Outputs)
		{
			Assert.AreEqual(150.0, output.Maximum, 1e-9);
			Assert.AreEqual(1.0, output.Rate);
		}
	}

	[TestMethod]
	[TestCategory("E-T07")]
	[TestCategory("E-P13")]
	public void PureReadAfterSourceChange_RefreshesReferencesWithoutChangingDefinitionOrStateVersions()
	{
		using var world = new EnvironmentalMagicTestWorld(policy: "compiled-prog");
		var cell = (Cell)world.Cells.At(0);
		cell.AddResource(world.Resources.Get(1)!, 10.0);
		var revision = world.Profile.Revision;
		var definition = world.Profile.ExportDefinition();
		var state = cell.EnvironmentState;
		world.CompileProg(1, "return 50");
		world.Coordinator.SourceDefinitionChanged();
		world.ResetSavedFlags();
		var before = world.Coordinator.Diagnostics;
		var snapshot = world.Coordinator.Inspect(cell);
		Assert.AreEqual(50.0, snapshot.Outputs[0].Maximum);
		Assert.AreEqual(revision, world.Profile.Revision);
		Assert.AreEqual(definition, world.Profile.ExportDefinition());
		Assert.AreEqual(state, cell.EnvironmentState);
		Assert.AreEqual(before.ProductionQueue, world.Coordinator.Diagnostics.ProductionQueue);
		Assert.AreEqual(before.DiscoveryRemaining, world.Coordinator.Diagnostics.DiscoveryRemaining);
		Assert.AreEqual(10.0, world.Balance());
		Assert.AreEqual(0, world.SaveRequests);
	}

	[TestMethod]
	[TestCategory("E-T12")]
	[TestCategory("E-P18")]
	public void TransientNegativeIdCell_CannotJoinPersistentEnvironmentalWork()
	{
		using var world = new EnvironmentalMagicTestWorld(0);
		var cell = world.CreateCell(balance: 50.0, id: -1);
		var resource = (SimpleMagicResource)world.Resources.Get(1)!;
		var cap = new Mock<IFutureProg>();
		cap.Setup(prog => prog.ExecuteDouble(0.0, It.IsAny<object[]>())).Returns(1000.0);
		resource.ResourceCapProg = cap.Object;
		world.Coordinator.Register(cell);
		Assert.AreEqual(0, world.Coordinator.Diagnostics.Configured);
		Assert.IsNull(world.Coordinator.Inspect(cell).ProfileId);
		Assert.IsFalse(world.Coordinator.TryMutateResource(cell, resource, EnvironmentalResourceMutation.Add, 1, out _));
		cell.AddResource(resource, 5.0);
		Assert.AreEqual(55.0, cell.MagicResourceAmounts[resource]);
		Assert.AreEqual(1000.0, resource.ResourceCap(cell));
		Assert.IsFalse(world.Coordinator.ApplyOperation(cell, Request(damage: 1)).Success);
		Assert.ThrowsException<ArgumentException>(() =>
			world.Coordinator.SetBinding(cell, EnvironmentalMagicBindingMode.Explicit, 1));
		world.RunSeconds(120);
		Assert.AreEqual(55.0, cell.MagicResourceAmounts[resource]);
		Assert.AreEqual(0, world.Coordinator.Diagnostics.Configured);
		Assert.AreEqual(0, world.Operations.Commits);
	}

	[TestMethod]
	[TestCategory("E-P17")]
	public void TerrainAndProfileBulkEdits_AreIncrementalButMutationDiscoversPendingBindingImmediately()
	{
		using var world = new EnvironmentalMagicTestWorld(3000, activePercent: 0.0, options: Options(visits: 16), start: false);
		world.Terrain.SetEnvironmentalMagicProfile(null);
		world.Start();
		Assert.AreEqual(0, world.Coordinator.Diagnostics.Configured);
		world.Cells.ForbidEnumeration = true;
		var before = world.Coordinator.Diagnostics.TotalEvaluations;
		world.Terrain.SetEnvironmentalMagicProfile(world.Profile.Id);
		Assert.AreEqual(before, world.Coordinator.Diagnostics.TotalEvaluations);
		Assert.AreEqual(3000, world.Coordinator.Diagnostics.DiscoveryRemaining);
		var last = (Cell)world.Cells.At(2999);
		world.Edit("output 1 basecapacity 20");
		last.AddResource(world.Resources.Get(1)!, 0.0);
		Assert.AreEqual(20.0, world.Balance(2999));
		Assert.AreEqual(1, world.Coordinator.Diagnostics.Configured);
		world.Tick();
		Assert.IsTrue(world.Coordinator.Diagnostics.LastCellVisits <= 16);
		Assert.IsTrue(world.Coordinator.Diagnostics.Configured < 100);
	}

	[TestMethod]
	[TestCategory("E-T09")]
	[TestCategory("E-P17")]
	public void PressureHalfLifeBulkEdit_UsesExactEffectiveTimeEvenBeforeQueuedCellsAreVisited()
	{
		using var world = new EnvironmentalMagicTestWorld(20, options: Options(visits: 1));
		foreach (var cell in world.Cells)
		{
			var result = world.Coordinator.ApplyOperation(cell, Request(pressure: 100.0));
			Assert.IsTrue(result.Success, result.Error);
		}
		var untouched = (Cell)world.Cells.At(19);
		var persisted = untouched.EnvironmentState;
		world.Clock.Advance(TimeSpan.FromMinutes(30));
		world.Edit("halflife 1800");
		world.Clock.Advance(TimeSpan.FromMinutes(30));
		Assert.AreEqual(100.0 * Math.Pow(2.0, -1.5), world.Coordinator.Inspect(untouched).Pressure, 1e-7);
		Assert.AreEqual(persisted, untouched.EnvironmentState);
		Assert.IsTrue(world.Coordinator.Diagnostics.DiscoveryRemaining > 0);
	}

	[TestMethod]
	[TestCategory("E-T02")]
	[TestCategory("E-T13")]
	[TestCategory("E-P18")]
	public void BindingModesTerrainTransitionsAndQueuedDeletion_PreserveStateAndRejectObsoleteWork()
	{
		using var world = new EnvironmentalMagicTestWorld(2);
		var cell = (Cell)world.Cells.At(0);
		var fallback = world.Cells.At(1);
		var resource = world.Resources.Get(1)!;
		cell.AddResource(resource, 20.0);
		Assert.IsTrue(world.Coordinator.ApplyOperation(cell, Request(damage: 5.0)).Success);
		var state = cell.EnvironmentState;
		world.Coordinator.SetBinding(cell, EnvironmentalMagicBindingMode.Disabled, null);
		Assert.IsFalse(world.Coordinator.Inspect(cell).ProfileId.HasValue);
		world.Tick(3600);
		Assert.AreEqual(20.0, world.Balance());
		Assert.AreEqual(state.ScarDamage, cell.EnvironmentState.ScarDamage);
		world.Coordinator.SetBinding(cell, EnvironmentalMagicBindingMode.Explicit, 1);
		Assert.AreEqual(20.0, world.Balance());
		var noProfileTerrain = world.AddTerrain(2, null);
		((IEditableCellOverlay)cell.CurrentOverlay).Terrain = noProfileTerrain;
		Assert.AreEqual(1L, world.Coordinator.Inspect(cell).ProfileId);
		world.Coordinator.SetBinding(cell, EnvironmentalMagicBindingMode.Inherit, null);
		Assert.IsNull(world.Coordinator.Inspect(cell).ProfileId);
		((IEditableCellOverlay)cell.CurrentOverlay).Terrain = world.Terrain;
		Assert.AreEqual(1L, world.Coordinator.Inspect(cell).ProfileId);
		world.Coordinator.MarkDirty(cell, EnvironmentalMagicDirtyReason.Policy);
		cell.DestroyWithDatabaseAction(fallback);
		var deletedBalance = cell.MagicResourceAmounts[resource];
		world.Tick(3600);
		Assert.AreEqual(deletedBalance, cell.MagicResourceAmounts[resource]);
		Assert.AreEqual(1, world.Coordinator.Diagnostics.Configured);
		Assert.IsNull(world.Coordinator.FieldFor(cell));
	}

	[TestMethod]
	[TestCategory("E-P18")]
	public void LateUnregisterOfReplacedCell_PreservesNewPhysicalInstanceWorkAndFieldIndex()
	{
		using var world = new EnvironmentalMagicTestWorld();
		var old = (Cell)world.Cells.At(0);
		var resource = world.Resources.Get(1)!;
		old.AddResource(resource, 10.0);
		world.Coordinator.MarkDirty(old, EnvironmentalMagicDirtyReason.Policy);
		var replacement = world.CreateCell(balance: 20.0, id: old.Id);
		world.Coordinator.Register(replacement);
		replacement.AddResource(resource, 0.0);
		var field = new Mock<IAgricultureField>();
		field.SetupGet(value => value.Id).Returns(1);
		field.SetupGet(value => value.Cell).Returns(replacement);
		world.Coordinator.FieldChanged(field.Object);
		world.Coordinator.MarkDirty(replacement, EnvironmentalMagicDirtyReason.Policy);
		world.Coordinator.Unregister(old);
		Assert.AreEqual(1, world.Coordinator.Diagnostics.Configured);
		Assert.AreEqual(1, world.Coordinator.Diagnostics.Dirty);
		Assert.AreSame(field.Object, world.Coordinator.FieldFor(replacement));
		world.Tick(60);
		Assert.AreEqual(21.0, replacement.MagicResourceAmounts[resource], 1e-9);
		Assert.AreEqual(10.0, old.MagicResourceAmounts[resource]);
	}

	[DataTestMethod]
	[DataRow(EnvironmentalMagicBindingMode.Inherit)]
	[DataRow(EnvironmentalMagicBindingMode.Disabled)]
	[TestCategory("E-T02")]
	public void BindingModes_SaveAndReloadPreserveInheritanceAndExplicitDisable(EnvironmentalMagicBindingMode mode)
	{
		using var original = new EnvironmentalMagicTestWorld();
		var cell = (Cell)original.Cells.At(0);
		cell.AddResource(original.Resources.Get(1)!, 50.0);
		original.Coordinator.SetBinding(cell, mode, null);
		Assert.IsTrue(original.Coordinator.ApplyOperation(cell, Request(damage: 3)).Success);
		var model = original.Snapshot(cell);
		original.Dispose();
		using var restarted = new EnvironmentalMagicTestWorld(0, start: false);
		var loaded = restarted.LoadCell(model);
		restarted.Start();
		Assert.AreEqual(mode, loaded.EnvironmentBindingMode);
		Assert.IsNull(loaded.EnvironmentalMagicProfileId);
		Assert.AreEqual(mode == EnvironmentalMagicBindingMode.Inherit ? 1L : (long?)null,
			restarted.Coordinator.Inspect(loaded).ProfileId);
		Assert.AreEqual(3.0, loaded.EnvironmentState.ScarDamage);
		Assert.AreEqual(50.0, restarted.Balance());
	}

	[TestMethod]
	[TestCategory("E-T09")]
	[TestCategory("E-P20")]
	public void ExplicitRepair_UsesActualRemainingDamageAfterNaturalRepairAndIsIdempotent()
	{
		using var world = new EnvironmentalMagicTestWorld(activePercent: 0.0);
		var cell = (Cell)world.Cells.At(0);
		world.Edit("repair 5");
		var damage = Request(damage: 10.0, pressure: 8.0);
		Assert.IsTrue(world.Coordinator.ApplyOperation(cell, damage).Success);
		Assert.IsTrue(world.Coordinator.ApplyOperation(cell, damage).Replayed);
		Assert.AreEqual(10.0, cell.EnvironmentState.ScarDamage);
		var lastDefile = cell.EnvironmentState.LastDefileUtc;
		world.Clock.Advance(TimeSpan.FromSeconds(60));
		var request = Request(repair: 10.0);
		var repair = world.Coordinator.ApplyOperation(cell, request);
		Assert.IsTrue(repair.Success, repair.Error);
		Assert.AreEqual(5.0, repair.AppliedRepair, 1e-9);
		Assert.AreEqual(0.0, cell.EnvironmentState.ScarDamage);
		Assert.AreEqual(lastDefile, cell.EnvironmentState.LastDefileUtc);
		Assert.IsTrue(world.Coordinator.ApplyOperation(cell, request).Replayed);
		Assert.AreEqual(2, world.Operations.Commits);
		Assert.AreEqual(100.0, world.Balance());
	}

	[TestMethod]
	[TestCategory("E-T11")]
	[TestCategory("E-P20")]
	[TestCategory("E-P21")]
	public void SeveralOutputs_NaturalRepairRunsOnceAndTinyPositiveRatesSurvive()
	{
		using var world = new EnvironmentalMagicTestWorld(outputs: 3);
		world.Edit("repair 0.5");
		var cell = (Cell)world.Cells.At(0);
		Assert.IsTrue(world.Coordinator.ApplyOperation(cell, Request(damage: 3.0)).Success);
		world.Tick(60);
		Assert.AreEqual(2.5, cell.EnvironmentState.ScarDamage, 1e-9);
		foreach (var resource in world.Resources) Assert.AreEqual(1.0, cell.MagicResourceAmounts[resource], 1e-9);
		world.Edit("repair 0.00000000000001");
		foreach (var resource in world.Resources) world.Edit($"output {resource.Id} baserate 0.00000000000001");
		cell.AddResource(world.Resources.Get(1)!, 0.0);
		var scar = cell.EnvironmentState.ScarDamage;
		var balance = world.Balance();
		for (var i = 0; i < 100; i++) world.Tick(60);
		Assert.IsTrue(cell.EnvironmentState.ScarDamage < scar);
		Assert.IsTrue(world.Balance() > balance);
	}

	[TestMethod]
	[TestCategory("E-P20")]
	[TestCategory("E-P22")]
	public void FullScarredRoom_WithNoRepairIsDormantAndWithRepairGetsMaintenanceOnly()
	{
		using var world = new EnvironmentalMagicTestWorld(activePercent: 0.0);
		var cell = (Cell)world.Cells.At(0);
		world.Coordinator.ApplyOperation(cell, Request(damage: 5.0, pressure: 100.0));
		Assert.AreEqual(1, world.Coordinator.Diagnostics.Dormant);
		var writes = world.Coordinator.Diagnostics.TotalWrites;
		world.RunSeconds(120);
		Assert.AreEqual(writes, world.Coordinator.Diagnostics.TotalWrites);
		world.Edit("repair 1");
		cell.AddResource(world.Resources.Get(1)!, 0.0);
		Assert.AreEqual(1, world.Coordinator.Diagnostics.ActiveMaintenance);
		Assert.AreEqual(0, world.Coordinator.Diagnostics.ActiveProduction);
		world.Tick(60);
		Assert.AreEqual(4.0, cell.EnvironmentState.ScarDamage, 1e-9);
		Assert.AreEqual(100.0, world.Balance());
	}

	[TestMethod]
	[TestCategory("E-T02")]
	[TestCategory("E-T09")]
	[TestCategory("E-T10")]
	public void ProductionSaveAndReload_PreserveBindingBalanceDamageReceiptsAndExcludeDowntime()
	{
		using var original = new EnvironmentalMagicTestWorld();
		var cell = (Cell)original.Cells.At(0);
		original.Coordinator.SetBinding(cell, EnvironmentalMagicBindingMode.Explicit, 1);
		var request = Request(damage: 3.0, pressure: 10.0);
		original.Coordinator.ApplyOperation(cell, request);
		cell.AddResource(original.Resources.Get(1)!, 0.0);
		original.Tick(60);
		var model = original.Snapshot(cell);
		var balance = original.Balance();
		original.Dispose();
		var clock = new EnvironmentalMagicTestClock();
		clock.SetUtcNow(original.Clock.GetUtcNow().AddDays(1));
		using var restarted = new EnvironmentalMagicTestWorld(0, start: false, clock: clock, operations: original.Operations);
		var loaded = restarted.LoadCell(model);
		restarted.Start();
		Assert.AreEqual(EnvironmentalMagicBindingMode.Explicit, loaded.EnvironmentBindingMode);
		Assert.AreEqual(1L, loaded.EnvironmentalMagicProfileId);
		Assert.AreEqual(balance, restarted.Balance());
		Assert.AreEqual(3.0, loaded.EnvironmentState.ScarDamage);
		Assert.IsTrue(restarted.Coordinator.Inspect(loaded).Pressure < 0.000001);
		Assert.IsTrue(restarted.Coordinator.ApplyOperation(loaded, request).Replayed);
		loaded.AddResource(restarted.Resources.Get(1)!, 0.0);
		restarted.Tick(60);
		Assert.AreEqual(balance + 1.0, restarted.Balance(), 1e-9);
	}

	[TestMethod]
	[TestCategory("E-P23")]
	public void TwoWorlds_IsolateClocksRegistriesAndHeartbeatDisposal()
	{
		using var first = new EnvironmentalMagicTestWorld();
		using var second = new EnvironmentalMagicTestWorld();
		first.Cells.At(0).AddResource(first.Resources.Get(1)!, 0.0);
		second.Cells.At(0).AddResource(second.Resources.Get(1)!, 0.0);
		first.Tick(60);
		Assert.AreEqual(1.0, first.Balance());
		Assert.AreEqual(0.0, second.Balance());
		first.Dispose();
		Assert.AreEqual(0, first.SecondSubscriptions);
		Assert.AreEqual(1, second.SecondSubscriptions);
		second.Tick(60);
		Assert.AreEqual(1.0, second.Balance());
	}

	[TestMethod]
	[TestCategory("E-T12")]
	public void UnboundCellsAndNonCellCaps_RetainOrdinaryLinearAndStateBehaviour()
	{
		using var world = new EnvironmentalMagicTestWorld();
		var cell = (Cell)world.Cells.At(0);
		world.Coordinator.SetBinding(cell, EnvironmentalMagicBindingMode.Disabled, null);
		var resource = (SimpleMagicResource)world.Resources.Get(1)!;
		var cap = new Mock<IFutureProg>();
		cap.Setup(prog => prog.ExecuteDouble(0.0, It.IsAny<object[]>())).Returns(1000.0);
		resource.ResourceCapProg = cap.Object;
		Assert.AreEqual(1000.0, resource.ResourceCap(cell));
		Assert.AreEqual(1000.0, resource.ResourceCap(Mock.Of<ICharacter>()));
		Assert.AreEqual(1000.0, resource.ResourceCap(Mock.Of<IGameItem>()));
		cell.AddResource(resource, 50.0);
		MudSharp.FutureProg.FutureProg.Initialise();
		var stateProg = new MudSharp.FutureProg.FutureProg(world.World.Object, "LegacyEnvironmentalTestState",
			ProgVariableTypes.Boolean, new[] { Tuple.Create(ProgVariableTypes.MagicResourceHaver, "holder") }, "return true") { Id = 20 };
		Assert.IsTrue(stateProg.Compile(), stateProg.CompileError);
		world.Progs.Add(stateProg);
		var linear = BaseMagicResourceGenerator.LoadFromDatabase(new MudSharp.Models.MagicGenerator
		{
			Id = 20, Name = "Legacy Linear", Type = "linear",
			Definition = "<Definition><WhichResource>1</WhichResource><AmountPerMinute>2</AmountPerMinute></Definition>"
		}, world.World.Object);
		var state = BaseMagicResourceGenerator.LoadFromDatabase(new MudSharp.Models.MagicGenerator
		{
			Id = 21, Name = "Legacy State", Type = "state",
			Definition = "<Definition><States><State><StateProg>20</StateProg><Resource resource='1' amountperminute='3'/></State></States></Definition>"
		}, world.World.Object);
		cell.AddMagicResourceGenerator(linear);
		cell.AddMagicResourceGenerator(state);
		world.Heartbeat.ManuallyFireHeartbeatMinute();
		Assert.AreEqual(55.0, world.Balance());
		Assert.AreEqual(0, world.Coordinator.Diagnostics.Configured);
	}

	[TestMethod]
	[TestCategory("E-P24")]
	public void ShortIdleRecheck_WakesTimePolicyUnderTheSameCentralBudget()
	{
		using var world = new EnvironmentalMagicTestWorld(options: Options(visits: 1));
		world.Edit("output 1 rate if(hasdefile > 0 and minutessincedefile >= 1, 1, 0)");
		world.Edit("idle 10");
		var cell = (Cell)world.Cells.At(0);
		world.Coordinator.ApplyOperation(cell, Request(pressure: 1));
		Assert.AreEqual(0, world.Coordinator.Diagnostics.ActiveProduction);
		world.RunSeconds(60);
		for (var second = 0; second < 10 && world.Coordinator.Diagnostics.ActiveProduction == 0; second++) world.Tick();
		Assert.AreEqual(1, world.Coordinator.Diagnostics.ActiveProduction);
		Assert.AreEqual(0.0, world.Balance());
		world.RunSeconds(60);
		Assert.AreEqual(1.0, world.Balance(), 1e-9);
		Assert.AreEqual(1, world.SecondSubscriptions);
	}

	private static EnvironmentalMagicOptions Options(int visits = 1024, double audit = 3600.0) => new()
	{
		MaximumCellVisits = visits, ReconciliationSeconds = audit, SoftBudgetMilliseconds = 1000.0
	};
	private static EnvironmentalMagicOperationRequest Request(double damage = 0, double pressure = 0, double repair = 0) =>
		new(Guid.NewGuid(), 1, "Environmental coordinator regression", damage, pressure, repair);
}
