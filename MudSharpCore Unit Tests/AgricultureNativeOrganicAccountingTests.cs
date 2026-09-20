using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Climate;
using MudSharp.Construction;
using MudSharp.Economy.Property;
using MudSharp.Form.Material;
using MudSharp.Framework;
using MudSharp.Framework.Save;
using MudSharp.GameItems;
using MudSharp.GameItems.Prototypes;
using MudSharp.Magic.Environment;
using MudSharp.Work.Agriculture;

#nullable enable

namespace MudSharp_Unit_Tests;

[TestClass]
public class AgricultureNativeOrganicAccountingTests
{
	[TestMethod]
	[TestCategory("Y-T13")]
	public void Accounting_DefinitionEditReloadAndRecreatedOwnerPreserveCorrectBoundaries()
	{
		var fixture = BuildFixture();
		var scar = new EnvironmentalMagicState
		{
			Revision = 9,
			ScarDamage = 17.5,
			RecentPressure = 3.25
		};
		fixture.Environment
			.Setup(x => x.InspectState(fixture.Cell.Object))
			.Returns(new EnvironmentalMagicStateSnapshot(scar, scar.RecentPressure));
		var accounting = FieldDefinition(new XElement("Source",
			new XAttribute("kind", "Crop"),
			new XAttribute("generation", 7),
			new XAttribute("revision", 12),
			new XAttribute("prepaid", "0.25"),
			new XAttribute("healthRemainder", "0.5"),
			new XAttribute("yieldRemainder", "0.75"),
			new XAttribute("biomassRemainder", "0")));
		var field = BuildField(fixture, AgricultureFieldUse.Crop, cropYield: 10, definition: accounting,
			fieldId: 41);
		var beforeEdit = field.InspectNativeOrganicSource(NativeOrganicSourceKind.Crop);

		fixture.Crop.SetupGet(x => x.Name).Returns("Edited Crop Definition");
		var editedProfile = new Mock<IAgricultureFieldProfile>();
		editedProfile.SetupGet(x => x.Id).Returns(2L);
		field.Profile = editedProfile.Object;
		var afterEdit = field.InspectNativeOrganicSource(NativeOrganicSourceKind.Crop);
		Assert.AreEqual(beforeEdit.PrepaidFraction, afterEdit.PrepaidFraction);
		Assert.AreEqual(beforeEdit.Lifecycle, afterEdit.Lifecycle);
		Assert.AreEqual(beforeEdit.RecoveryRemainders, afterEdit.RecoveryRemainders);
		Assert.AreEqual(beforeEdit.SourceRevision, afterEdit.SourceRevision);

		var saved = SaveDefinition(field).ToString();
		var reloaded = BuildField(fixture, AgricultureFieldUse.Crop, cropYield: 10, definition: saved,
			fieldId: 41);
		var afterReload = reloaded.InspectNativeOrganicSource(NativeOrganicSourceKind.Crop);
		Assert.AreEqual(afterEdit.PrepaidFraction, afterReload.PrepaidFraction);
		Assert.AreEqual(afterEdit.Lifecycle, afterReload.Lifecycle);
		Assert.AreEqual(afterEdit.RecoveryRemainders, afterReload.RecoveryRemainders);
		Assert.AreEqual(afterEdit.SourceRevision, afterReload.SourceRevision);

		var recreated = BuildField(fixture, AgricultureFieldUse.Crop, cropYield: 10, fieldId: 42);
		var recreatedSnapshot = recreated.InspectNativeOrganicSource(NativeOrganicSourceKind.Crop);
		Assert.AreEqual(42L, recreatedSnapshot.Lifecycle!.FieldId);
		Assert.AreEqual(1L, recreatedSnapshot.Lifecycle.Generation);
		Assert.AreEqual(0m, recreatedSnapshot.PrepaidFraction);
		Assert.AreEqual(NativeOrganicRecoveryRemainders.Empty, recreatedSnapshot.RecoveryRemainders);
		Assert.AreEqual(scar, fixture.Environment.Object.InspectState(fixture.Cell.Object).State);
	}

	[TestMethod]
	[TestCategory("Y-T14")]
	public void Accounting_MalformedNumericPersistenceFailsClosedAndPreservesRawEvidence()
	{
		var fixture = BuildFixture();
		var malformedValues = new (string Attribute, string Value)[]
		{
			("generation", "-1"),
			("revision", "-1"),
			("generation", long.MaxValue.ToString()),
			("prepaid", "NaN"),
			("prepaid", "Infinity"),
			("prepaid", "1"),
			("prepaid", "-0.000000001"),
			("healthRemainder", "1"),
			("yieldRemainder", "1E-9"),
			("biomassRemainder", "79228162514264337593543950336")
		};

		for (var i = 0; i < malformedValues.Length; i++)
		{
			var source = new XElement("Source",
				new XAttribute("kind", "Crop"),
				new XAttribute("generation", 4),
				new XAttribute("revision", 2),
				new XAttribute("prepaid", "0.25"),
				new XAttribute("healthRemainder", "0.5"),
				new XAttribute("yieldRemainder", "0.75"),
				new XAttribute("biomassRemainder", "0"));
			source.SetAttributeValue(malformedValues[i].Attribute, malformedValues[i].Value);
			var field = BuildField(fixture, AgricultureFieldUse.Crop, cropYield: 10,
				definition: FieldDefinition(source), fieldId: i + 1L);

			var snapshot = field.InspectNativeOrganicSource(NativeOrganicSourceKind.Crop);
			Assert.AreEqual(NativeOrganicSourceStatus.Invalid, snapshot.Status,
				$"{malformedValues[i].Attribute}={malformedValues[i].Value} should fail closed.");
			Assert.AreEqual(10.0, snapshot.NativeStock);
			var raw = SaveDefinition(field)
				.Element("NativeOrganicAccounting")!
				.Elements("Source")
				.Single(x => (string?)x.Attribute("kind") == "Crop");
			Assert.AreEqual(malformedValues[i].Value, raw.Attribute(malformedValues[i].Attribute)!.Value);
		}
	}

	[TestMethod]
	[TestCategory("Y-T14")]
	public void Debit_TinyBoundaryAndNonFiniteInputsCannotMutateOwnerAccounting()
	{
		Assert.IsFalse(NativeOrganicAccountingMath.TryAmount(double.NaN, out _, out _));
		Assert.IsFalse(NativeOrganicAccountingMath.TryAmount(double.PositiveInfinity, out _, out _));
		Assert.IsFalse(NativeOrganicAccountingMath.TryAmount(double.NegativeInfinity, out _, out _));
		Assert.IsFalse(NativeOrganicAccountingMath.TryAmount(double.MaxValue, out _, out _));
		Assert.IsFalse(NativeOrganicAccountingMath.TryAmount(1.0e-10, out _, out _));
		Assert.IsTrue(NativeOrganicAccountingMath.TryAmount(1.0e-9, out var minimum, out _));
		Assert.AreEqual(NativeOrganicAccountingMath.MinimumDebit, minimum);

		var fixture = BuildFixture();
		var field = BuildField(fixture, AgricultureFieldUse.Woodland, woodlandYield: 5);
		var valid = PlanDebit(field, NativeOrganicSourceKind.Woodland, 0.25m);
		var invalidPlans = new[]
		{
			valid with { RequestedAmount = NativeOrganicAccountingMath.MinimumDebit / 10m },
			valid with { RequestedAmount = NativeOrganicAccountingMath.MaximumDebit + NativeOrganicAccountingMath.MinimumDebit },
			valid with { ExpectedNativeStock = double.NaN },
			valid with { ExpectedNativeStock = double.PositiveInfinity },
			valid with { SourceRevision = -1L },
			valid with { EnvironmentalProfileRevision = -1L },
			valid with { Lifecycle = valid.Lifecycle with { Generation = -1L } },
			valid with { ClosingPrepaidFraction = 1m }
		};
		var before = field.InspectNativeOrganicSource(NativeOrganicSourceKind.Woodland);
		foreach (var plan in invalidPlans)
		{
			Assert.IsFalse(field.TryApplyNativeOrganicDebit(plan, out _));
			var afterRefusal = field.InspectNativeOrganicSource(NativeOrganicSourceKind.Woodland);
			Assert.AreEqual(before.NativeStock, afterRefusal.NativeStock);
			Assert.AreEqual(before.PrepaidFraction, afterRefusal.PrepaidFraction);
			Assert.AreEqual(before.SourceRevision, afterRefusal.SourceRevision);
		}

		Assert.IsTrue(ApplyPlannedDebit(field, NativeOrganicSourceKind.Woodland,
			NativeOrganicAccountingMath.MinimumDebit));
		var prepaid = field.InspectNativeOrganicSource(NativeOrganicSourceKind.Woodland);
		Assert.AreEqual(4.0, prepaid.NativeStock);
		Assert.AreEqual(0.999999999m, prepaid.PrepaidFraction);
		Assert.IsTrue(ApplyPlannedDebit(field, NativeOrganicSourceKind.Woodland, prepaid.PrepaidFraction));
		var closed = field.InspectNativeOrganicSource(NativeOrganicSourceKind.Woodland);
		Assert.AreEqual(4.0, closed.NativeStock);
		Assert.AreEqual(0m, closed.PrepaidFraction);
	}

	[TestMethod]
	[TestCategory("Y-T16")]
	public void Recovery_CropAndWoodlandChannelsSuppressPositiveButLeaveNegativeStressWhole()
	{
		var fixture = BuildFixture((_, channel, _) => channel switch
		{
			NativeOrganicPenaltyChannel.CropHealthRecovery or
				NativeOrganicPenaltyChannel.WoodlandHealthRecovery =>
				new NativeOrganicPenaltyEvaluation(true, true, 0.25, null),
			NativeOrganicPenaltyChannel.CropYieldRecovery or
				NativeOrganicPenaltyChannel.WoodlandYieldRecovery =>
				new NativeOrganicPenaltyEvaluation(true, true, 0.5, null),
			_ => NativeOrganicPenaltyEvaluation.Neutral
		});
		fixture.Crop.SetupGet(x => x.PollinationDependency).Returns(AgriculturePollinationDependency.Beneficial);
		fixture.Crop.SetupGet(x => x.PollinationHealthBonus).Returns(3);
		fixture.Crop.SetupGet(x => x.PollinationYieldBonus).Returns(3);
		var apiary = new Mock<IAgricultureFieldApiary>();
		apiary.SetupGet(x => x.PollinationRadius).Returns(1);
		apiary.SetupGet(x => x.PollinationStrength).Returns(100);
		var apiaryField = new Mock<IAgricultureField>();
		apiaryField.SetupGet(x => x.Id).Returns(99L);
		apiaryField.SetupGet(x => x.Cell).Returns(fixture.Cell.Object);
		apiaryField.SetupGet(x => x.HasActiveApiary).Returns(true);
		apiaryField.SetupGet(x => x.IsApiaryHappy).Returns(true);
		apiaryField.SetupGet(x => x.Apiary).Returns(apiary.Object);
		var fields = new All<IAgricultureField>();
		fields.Add(apiaryField.Object);
		fixture.Gameworld.SetupGet(x => x.AgricultureFields).Returns(fields);

		var crop = BuildField(fixture, AgricultureFieldUse.Crop, cropHealth: 50, cropYield: 50,
			nutrients: 100);
		crop.DailyTick();
		Assert.AreEqual(51, crop.CropHealth, "The +4 health contribution should be quartered once to +1.");
		Assert.AreEqual(52, crop.CropYieldPotential, "The +4 yield contribution should be halved once to +2.");
		fixture.Environment.Verify(x => x.EvaluateOrganicPenalty(fixture.Cell.Object,
			NativeOrganicPenaltyChannel.CropHealthRecovery,
			It.Is<NativeOrganicPenaltyContext>(context => context.BaselineIncrease == 4.0)), Times.Once);
		fixture.Environment.Verify(x => x.EvaluateOrganicPenalty(fixture.Cell.Object,
			NativeOrganicPenaltyChannel.CropYieldRecovery,
			It.Is<NativeOrganicPenaltyContext>(context => context.BaselineIncrease == 4.0)), Times.Once);

		var woodland = BuildField(fixture, AgricultureFieldUse.Woodland, woodlandHealth: 50,
			woodlandYield: 50, woodlandGrowthDays: 2, fieldId: 2);
		for (var i = 0; i < 4; i++)
		{
			woodland.DailyTick();
		}

		Assert.AreEqual(51, woodland.WoodlandHealth);
		Assert.AreEqual(52, woodland.WoodlandYieldPotential);

		fixture.Environment.Invocations.Clear();
		fixture.Crop.SetupGet(x => x.MinimumMoisture).Returns(60);
		var stressedCrop = BuildField(fixture, AgricultureFieldUse.Crop, cropHealth: 50, cropYield: 50,
			fieldId: 3);
		stressedCrop.DailyTick();
		Assert.AreEqual(46, stressedCrop.CropHealth);
		Assert.AreEqual(47, stressedCrop.CropYieldPotential,
			"The ordinary -3 yield stress must not be quartered or halved.");
		var stressedWoodland = BuildField(fixture, AgricultureFieldUse.Woodland, woodlandHealth: 50,
			woodlandYield: 50, fieldId: 4);
		stressedWoodland.Moisture = 0;
		stressedWoodland.DailyTick();
		Assert.AreEqual(48, stressedWoodland.WoodlandHealth);
		Assert.AreEqual(50, stressedWoodland.WoodlandYieldPotential);
		fixture.Environment.Verify(x => x.EvaluateOrganicPenalty(It.IsAny<ICell>(),
			It.IsAny<NativeOrganicPenaltyChannel>(), It.IsAny<NativeOrganicPenaltyContext>()), Times.Never);
	}

	[TestMethod]
	[TestCategory("Y-T19")]
	public void Initialisation_SuppressionDoesNotAlterGrowthOrHarvestWindowCalendars()
	{
		var neutralFixture = BuildFixture();
		var suppressedFixture = BuildFixture((_, channel, _) => channel switch
		{
			NativeOrganicPenaltyChannel.CropInitialisation or
				NativeOrganicPenaltyChannel.WoodlandInitialisation =>
				new NativeOrganicPenaltyEvaluation(true, true, 0.5, null),
			_ => NativeOrganicPenaltyEvaluation.Neutral
		});
		var neutralCrop = BuildField(neutralFixture, AgricultureFieldUse.Fallow);
		var suppressedCrop = BuildField(suppressedFixture, AgricultureFieldUse.Fallow);
		var neutralSow = BuildOperation(neutralFixture.Gameworld.Object, AgricultureOperationType.Sow,
			AgricultureFieldUse.Fallow, AgricultureFieldUse.Crop, AgricultureTargetType.Crop);
		var suppressedSow = BuildOperation(suppressedFixture.Gameworld.Object, AgricultureOperationType.Sow,
			AgricultureFieldUse.Fallow, AgricultureFieldUse.Crop, AgricultureTargetType.Crop);
		Assert.IsTrue(neutralCrop.ApplyOperation(neutralSow, neutralFixture.Crop.Object, null!, false, out _));
		Assert.IsTrue(suppressedCrop.ApplyOperation(suppressedSow, suppressedFixture.Crop.Object, null!, false,
			out _));
		Assert.AreEqual(0, neutralCrop.CropGrowthDays);
		Assert.AreEqual(neutralCrop.CropGrowthDays, suppressedCrop.CropGrowthDays);
		Assert.AreEqual(0, neutralCrop.CropHarvestCount);
		Assert.AreEqual(neutralCrop.CropHarvestCount, suppressedCrop.CropHarvestCount);

		for (var i = 0; i < 30; i++)
		{
			neutralCrop.DailyTick();
			suppressedCrop.DailyTick();
		}

		Assert.AreEqual(AgricultureCropStage.Harvestable, neutralCrop.CropStage);
		Assert.AreEqual(neutralCrop.CropStage, suppressedCrop.CropStage);
		Assert.AreEqual(30, neutralCrop.CropGrowthDays);
		Assert.AreEqual(neutralCrop.CropGrowthDays, suppressedCrop.CropGrowthDays);
		for (var i = 0; i < 5; i++)
		{
			neutralCrop.DailyTick();
			suppressedCrop.DailyTick();
		}

		Assert.AreEqual(AgricultureCropStage.Overripe, neutralCrop.CropStage);
		Assert.AreEqual(neutralCrop.CropStage, suppressedCrop.CropStage);
		Assert.AreEqual(35, neutralCrop.CropGrowthDays);
		Assert.AreEqual(neutralCrop.CropGrowthDays, suppressedCrop.CropGrowthDays);

		var neutralWoodland = BuildField(neutralFixture, AgricultureFieldUse.Fallow, fieldId: 2);
		var suppressedWoodland = BuildField(suppressedFixture, AgricultureFieldUse.Fallow, fieldId: 2);
		var neutralEstablish = BuildOperation(neutralFixture.Gameworld.Object, AgricultureOperationType.Woodland,
			AgricultureFieldUse.Fallow, AgricultureFieldUse.Woodland, AgricultureTargetType.Woodland);
		var suppressedEstablish = BuildOperation(suppressedFixture.Gameworld.Object,
			AgricultureOperationType.Woodland, AgricultureFieldUse.Fallow, AgricultureFieldUse.Woodland,
			AgricultureTargetType.Woodland);
		Assert.IsTrue(neutralWoodland.ApplyOperation(neutralEstablish, neutralFixture.Woodland.Object, null!, false,
			out _));
		Assert.IsTrue(suppressedWoodland.ApplyOperation(suppressedEstablish, suppressedFixture.Woodland.Object,
			null!, false, out _));
		for (var i = 0; i < 5; i++)
		{
			neutralWoodland.DailyTick();
			suppressedWoodland.DailyTick();
		}

		Assert.AreEqual(5, neutralWoodland.WoodlandGrowthDays);
		Assert.AreEqual(neutralWoodland.WoodlandGrowthDays, suppressedWoodland.WoodlandGrowthDays);
	}

	[TestMethod]
	[TestCategory("Y-T20")]
	public void Operation_RetainedOrchardPositiveRecoveryIsFactoredOnceWithoutRefactoringCostOrOutput()
	{
		var fixture = BuildFixture((_, channel, _) => channel switch
		{
			NativeOrganicPenaltyChannel.CropHealthRecovery or NativeOrganicPenaltyChannel.CropYieldRecovery =>
				new NativeOrganicPenaltyEvaluation(true, true, 0.5, null),
			_ => NativeOrganicPenaltyEvaluation.Neutral
		});
		fixture.Crop.SetupGet(x => x.IsPerennial).Returns(true);
		fixture.Crop.SetupGet(x => x.YieldOutputs).Returns(new[]
		{
			new AgricultureCommodityYield("test produce", 3.5)
		});
		var materials = new Mock<IUneditableAll<ISolid>>();
		fixture.Gameworld.SetupGet(x => x.Materials).Returns(materials.Object);
		var orchard = BuildField(fixture, AgricultureFieldUse.Orchard, cropHealth: 50, cropYield: 50,
			cropStage: AgricultureCropStage.Harvestable);
		var harvest = BuildOperation(fixture.Gameworld.Object, AgricultureOperationType.Harvest,
			AgricultureFieldUse.Orchard, AgricultureFieldUse.Orchard);
		var outcome = AgricultureWorkOutcome.FromSkill(1000.0, 10.0, 0.0, 0.0);
		var originalCommodityPrototype = CommodityGameItemComponentProto.ItemPrototype;
		CommodityGameItemComponentProto.ItemPrototype = new Mock<IGameItemProto>().Object;

		try
		{
			Assert.IsTrue(orchard.ApplyOperation(harvest, null!, null!, false, outcome, out _));
		}
		finally
		{
			CommodityGameItemComponentProto.ItemPrototype = originalCommodityPrototype;
		}

		Assert.AreEqual(50 + (int)Math.Floor(outcome.CropHealthDelta * 0.5), orchard.CropHealth);
		Assert.AreEqual(50 - 20 + (int)Math.Floor(outcome.CropYieldDelta * 0.5),
			orchard.CropYieldPotential,
			"The fixed -20 harvest cost must remain whole while only the positive outcome is factored once.");
		Assert.AreEqual(1, orchard.CropHarvestCount);
		Assert.AreEqual(0, orchard.CropGrowthDays);
		materials.Verify(x => x.GetByName("test produce"), Times.Once,
			"The pre-harvest output crossed the one-unit threshold and must not receive the ecological factor again.");
		fixture.Environment.Verify(x => x.EvaluateOrganicPenalty(fixture.Cell.Object,
			NativeOrganicPenaltyChannel.CropHealthRecovery,
			It.Is<NativeOrganicPenaltyContext>(context => context.BaselineIncrease == outcome.CropHealthDelta)),
			Times.Once);
		fixture.Environment.Verify(x => x.EvaluateOrganicPenalty(fixture.Cell.Object,
			NativeOrganicPenaltyChannel.CropYieldRecovery,
			It.Is<NativeOrganicPenaltyContext>(context => context.BaselineIncrease == outcome.CropYieldDelta)),
			Times.Once);
	}

	[TestMethod]
	[TestCategory("C-R1-01")]
	[TestCategory("C-R1-02")]
	[TestCategory("C-R1-03")]
	public void OrchardHarvest_OffsetsPositiveBonusAgainstTheSameHarvestCost()
	{
		var neutral = BuildFixture();
		neutral.Crop.SetupGet(x => x.IsPerennial).Returns(true);
		var suppressed = BuildFixture((_, channel, _) =>
			channel == NativeOrganicPenaltyChannel.CropYieldRecovery
				? new NativeOrganicPenaltyEvaluation(true, true, 0.5, null)
				: NativeOrganicPenaltyEvaluation.Neutral);
		suppressed.Crop.SetupGet(x => x.IsPerennial).Returns(true);
		var explicitNeutral = BuildFixture((_, channel, _) =>
			channel == NativeOrganicPenaltyChannel.CropYieldRecovery
				? new NativeOrganicPenaltyEvaluation(true, true, 1.0, null)
				: NativeOrganicPenaltyEvaluation.Neutral);
		explicitNeutral.Crop.SetupGet(x => x.IsPerennial).Returns(true);
		var constructor = typeof(AgricultureWorkOutcome).GetConstructors(BindingFlags.Instance | BindingFlags.NonPublic)
			.Single();
		AgricultureWorkOutcome Bonus(int value) => (AgricultureWorkOutcome)constructor.Invoke(new object[]
			{ 35.0, 1.0, 0.0, 0.0, 1.0, 1.0, 1.0, 1.0, 0, value, ItemQuality.Standard });
		foreach (var (fixture, opening, bonus, expected) in new[]
		         {
			         (neutral, 100, 5, 85), (neutral, 99, 5, 84),
			         (explicitNeutral, 100, 5, 85), (explicitNeutral, 99, 5, 84),
			         (neutral, 5, 10, 0), (suppressed, 100, 10, 85),
			         (suppressed, 50, 10, 35), (suppressed, 50, -5, 25)
		         })
		{
			var orchard = BuildField(fixture, AgricultureFieldUse.Orchard, cropYield: opening,
				cropStage: AgricultureCropStage.Harvestable);
			var harvest = BuildOperation(fixture.Gameworld.Object, AgricultureOperationType.Harvest,
				AgricultureFieldUse.Orchard, AgricultureFieldUse.Orchard);
			Assert.IsTrue(orchard.ApplyOperation(harvest, null!, null!, false, Bonus(bonus), out _));
			Assert.AreEqual(expected, orchard.CropYieldPotential,
				$"Opening {opening}, bonus {bonus}, expected {expected}.");
			Assert.AreEqual(1, orchard.CropHarvestCount);
		}
	}

	[TestMethod]
	[TestCategory("C-R1-04")]
	public void CropTick_MixedPollinationAndNutrientContributionsRetainSequentialNativeClamps()
	{
		foreach (var (opening, nutrients, pollinationBonus, factor, expected) in new[]
		         { (0, 40, 2, -1.0, 2), (100, 100, -2, -1.0, 98),
		           (100, 40, 2, -1.0, 100), (1, 40, 2, -1.0, 2),
		           (99, 100, -2, -1.0, 98), (100, 100, -2, 1.0, 98),
		           (0, 40, 2, 0.5, 1), (100, 100, -2, 0.5, 98) })
		{
			if (factor is < 0 or 1.0)
			{
				var native = Math.Clamp(Math.Clamp(opening + Math.Sign(nutrients - 50), 0, 100) +
				                        pollinationBonus, 0, 100);
				Assert.AreEqual(native, expected, "The neutral expectation must match the pre-feature tick.");
			}
			var fixture = BuildFixture((_, channel, _) =>
				factor < 0 || channel != NativeOrganicPenaltyChannel.CropYieldRecovery
					? NativeOrganicPenaltyEvaluation.Neutral
					: new NativeOrganicPenaltyEvaluation(true, true, factor, null));
			fixture.Crop.SetupGet(x => x.PollinationDependency)
				.Returns(AgriculturePollinationDependency.Beneficial);
			fixture.Crop.SetupGet(x => x.PollinationYieldBonus).Returns(pollinationBonus);
			var apiary = new Mock<IAgricultureFieldApiary>();
			apiary.SetupGet(x => x.PollinationRadius).Returns(1);
			apiary.SetupGet(x => x.PollinationStrength).Returns(50);
			var apiaryField = new Mock<IAgricultureField>();
			apiaryField.SetupGet(x => x.Id).Returns(2L);
			apiaryField.SetupGet(x => x.Cell).Returns(fixture.Cell.Object);
			apiaryField.SetupGet(x => x.HasActiveApiary).Returns(true);
			apiaryField.SetupGet(x => x.IsApiaryHappy).Returns(true);
			apiaryField.SetupGet(x => x.Apiary).Returns(apiary.Object);
			var fields = new All<IAgricultureField>();
			fields.Add(apiaryField.Object);
			fixture.Gameworld.SetupGet(x => x.AgricultureFields).Returns(fields);
			var field = BuildField(fixture, AgricultureFieldUse.Crop, cropYield: opening,
				nutrients: nutrients);
			field.DailyTick();
			Assert.AreEqual(expected, field.CropYieldPotential,
				$"Opening {opening}, nutrients {nutrients}, pollination bonus {pollinationBonus}.");
		}
	}

	[TestMethod]
	[TestCategory("C-R1-04")]
	public void CropTick_HealthClampsOrdinaryRecoveryBeforePollination()
	{
		foreach (var (opening, bonus, factor, expected) in new[]
		         { (100, -2, -1.0, 98), (1, 2, -1.0, 4),
		           (99, 3, -1.0, 100), (100, -2, 1.0, 98),
		           (50, -2, 0.5, 48) })
		{
			var fixture = BuildFixture((_, channel, _) =>
				factor < 0 || channel != NativeOrganicPenaltyChannel.CropHealthRecovery
					? NativeOrganicPenaltyEvaluation.Neutral
					: new NativeOrganicPenaltyEvaluation(true, true, factor, null));
			fixture.Crop.SetupGet(x => x.PollinationDependency)
				.Returns(AgriculturePollinationDependency.Beneficial);
			fixture.Crop.SetupGet(x => x.PollinationHealthBonus).Returns(bonus);
			var apiary = new Mock<IAgricultureFieldApiary>();
			apiary.SetupGet(x => x.PollinationRadius).Returns(1);
			apiary.SetupGet(x => x.PollinationStrength).Returns(50);
			var apiaryField = new Mock<IAgricultureField>();
			apiaryField.SetupGet(x => x.Id).Returns(2L);
			apiaryField.SetupGet(x => x.Cell).Returns(fixture.Cell.Object);
			apiaryField.SetupGet(x => x.HasActiveApiary).Returns(true);
			apiaryField.SetupGet(x => x.IsApiaryHappy).Returns(true);
			apiaryField.SetupGet(x => x.Apiary).Returns(apiary.Object);
			var fields = new All<IAgricultureField>();
			fields.Add(apiaryField.Object);
			fixture.Gameworld.SetupGet(x => x.AgricultureFields).Returns(fields);
			var field = BuildField(fixture, AgricultureFieldUse.Crop, cropHealth: opening,
				cropYield: 50, nutrients: 50);
			field.DailyTick();
			Assert.AreEqual(expected, field.CropHealth,
				$"Opening {opening}, pollination health {bonus}, factor {factor}.");
		}
	}

	[TestMethod]
	[TestCategory("Y-T23")]
	public void BindingToggle_DoesNotRefundStockOrPrepaidFractionAndUnboundGrowthIsNeutral()
	{
		var bound = true;
		var fixture = BuildFixture((_, _, _) => bound
			? new NativeOrganicPenaltyEvaluation(true, true, 0.25, null)
			: NativeOrganicPenaltyEvaluation.Neutral);
		var field = BuildField(fixture, AgricultureFieldUse.Crop, cropYield: 10);
		Assert.IsTrue(ApplyPlannedDebit(field, NativeOrganicSourceKind.Crop, 0.25m));
		var beforeToggle = field.InspectNativeOrganicSource(NativeOrganicSourceKind.Crop);

		bound = false;
		var whileUnbound = field.InspectNativeOrganicSource(NativeOrganicSourceKind.Crop);
		Assert.AreEqual(beforeToggle.NativeStock, whileUnbound.NativeStock);
		Assert.AreEqual(beforeToggle.PrepaidFraction, whileUnbound.PrepaidFraction);
		Assert.AreEqual(beforeToggle.Lifecycle, whileUnbound.Lifecycle);
		Assert.AreEqual(beforeToggle.SourceRevision, whileUnbound.SourceRevision);

		var unboundBaseline = BuildField(fixture, AgricultureFieldUse.Crop, cropHealth: 50, cropYield: 50,
			nutrients: 100, fieldId: 2);
		unboundBaseline.DailyTick();
		Assert.AreEqual(51, unboundBaseline.CropHealth);
		Assert.AreEqual(51, unboundBaseline.CropYieldPotential);

		bound = true;
		var afterRebind = field.InspectNativeOrganicSource(NativeOrganicSourceKind.Crop);
		Assert.AreEqual(beforeToggle.NativeStock, afterRebind.NativeStock);
		Assert.AreEqual(beforeToggle.PrepaidFraction, afterRebind.PrepaidFraction);
		Assert.IsTrue(ApplyPlannedDebit(field, NativeOrganicSourceKind.Crop, 0.75m));
		var afterRemainderUse = field.InspectNativeOrganicSource(NativeOrganicSourceKind.Crop);
		Assert.AreEqual(9.0, afterRemainderUse.NativeStock);
		Assert.AreEqual(0m, afterRemainderUse.PrepaidFraction);
	}

	[TestMethod]
	public void IntegerDebit_QuarterAndTenthSplitsMatchSingleDebit()
	{
		var fixture = BuildFixture();
		var quarters = BuildField(fixture, AgricultureFieldUse.Crop, cropYield: 10);
		var tenths = BuildField(fixture, AgricultureFieldUse.Crop, cropYield: 10, fieldId: 2);
		var single = BuildField(fixture, AgricultureFieldUse.Crop, cropYield: 10, fieldId: 3);

		for (var i = 0; i < 4; i++)
		{
			Assert.IsTrue(ApplyPlannedDebit(quarters, NativeOrganicSourceKind.Crop, 0.25m));
		}

		for (var i = 0; i < 10; i++)
		{
			Assert.IsTrue(ApplyPlannedDebit(tenths, NativeOrganicSourceKind.Crop, 0.1m));
		}

		Assert.IsTrue(ApplyPlannedDebit(single, NativeOrganicSourceKind.Crop, 1m));
		var quarterSnapshot = quarters.InspectNativeOrganicSource(NativeOrganicSourceKind.Crop);
		var tenthSnapshot = tenths.InspectNativeOrganicSource(NativeOrganicSourceKind.Crop);
		var singleSnapshot = single.InspectNativeOrganicSource(NativeOrganicSourceKind.Crop);
		Assert.AreEqual(9.0, quarterSnapshot.NativeStock);
		Assert.AreEqual(quarterSnapshot.NativeStock, tenthSnapshot.NativeStock);
		Assert.AreEqual(quarterSnapshot.NativeStock, singleSnapshot.NativeStock);
		Assert.AreEqual(0m, quarterSnapshot.PrepaidFraction);
		Assert.AreEqual(quarterSnapshot.PrepaidFraction, tenthSnapshot.PrepaidFraction);
		Assert.AreEqual(quarterSnapshot.PrepaidFraction, singleSnapshot.PrepaidFraction);
	}

	[TestMethod]
	public void IntegerDebit_OrdinaryConsumerCannotReusePrepaidWholeUnit()
	{
		var fixture = BuildFixture();
		var field = BuildField(fixture, AgricultureFieldUse.Crop, cropYield: 10);
		Assert.IsTrue(ApplyPlannedDebit(field, NativeOrganicSourceKind.Crop, 0.25m));
		var stalePlan = PlanDebit(field, NativeOrganicSourceKind.Crop, 0.25m);

		Assert.IsTrue(field.ConsumeCropYield(9, out _));
		Assert.IsFalse(field.TryApplyNativeOrganicDebit(stalePlan, out _));
		var beforeRemainderDebit = field.InspectNativeOrganicSource(NativeOrganicSourceKind.Crop);
		Assert.AreEqual(0.0, beforeRemainderDebit.NativeStock);
		Assert.AreEqual(0.75m, beforeRemainderDebit.PrepaidFraction);
		Assert.AreEqual(NativeOrganicSourceStatus.Exhausted, beforeRemainderDebit.Status);
		Assert.IsTrue(ApplyPlannedDebit(field, NativeOrganicSourceKind.Crop, 0.5m));
		var after = field.InspectNativeOrganicSource(NativeOrganicSourceKind.Crop);
		Assert.AreEqual(0.0, after.NativeStock);
		Assert.AreEqual(0.25m, after.PrepaidFraction);
	}

	[TestMethod]
	[TestCategory("Y-T05")]
	[TestCategory("Y-T08")]
	public void IntegerDebit_ConcurrentCopiesOfOnePlanHaveExactlyOneWinner()
	{
		var fixture = BuildFixture();
		var field = BuildField(fixture, AgricultureFieldUse.Crop, cropYield: 10);
		var plan = PlanDebit(field, NativeOrganicSourceKind.Crop, 0.25m);
		using var start = new Barrier(3);
		var first = Task.Run(() =>
		{
			start.SignalAndWait();
			return field.TryApplyNativeOrganicDebit(plan, out _);
		});
		var second = Task.Run(() =>
		{
			start.SignalAndWait();
			return field.TryApplyNativeOrganicDebit(plan, out _);
		});

		start.SignalAndWait();
		Task.WaitAll(first, second);

		Assert.AreEqual(1, new[] { first.Result, second.Result }.Count(x => x));
		var after = field.InspectNativeOrganicSource(NativeOrganicSourceKind.Crop);
		Assert.AreEqual(9.0, after.NativeStock);
		Assert.AreEqual(0.75m, after.PrepaidFraction);
		Assert.AreEqual(plan.SourceRevision + 1L, after.SourceRevision);
	}

	[TestMethod]
	[TestCategory("Y-T05")]
	[TestCategory("Y-T08")]
	public void IntegerDebit_ConcurrentPlanAndOrdinaryConsumerCannotReuseOneUnit()
	{
		var fixture = BuildFixture();
		var field = BuildField(fixture, AgricultureFieldUse.Crop, cropYield: 1);
		var plan = PlanDebit(field, NativeOrganicSourceKind.Crop, 0.25m);
		using var start = new Barrier(3);
		var plannedDebit = Task.Run(() =>
		{
			start.SignalAndWait();
			return field.TryApplyNativeOrganicDebit(plan, out _);
		});
		var ordinaryDebit = Task.Run(() =>
		{
			start.SignalAndWait();
			return field.ConsumeCropYield(1, out _);
		});

		start.SignalAndWait();
		Task.WaitAll(plannedDebit, ordinaryDebit);

		Assert.AreEqual(1, new[] { plannedDebit.Result, ordinaryDebit.Result }.Count(x => x));
		var after = field.InspectNativeOrganicSource(NativeOrganicSourceKind.Crop);
		Assert.AreEqual(0.0, after.NativeStock);
		Assert.AreEqual(plannedDebit.Result ? 0.75m : 0m, after.PrepaidFraction);
		Assert.AreEqual(plan.SourceRevision + 1L, after.SourceRevision);
	}

	[TestMethod]
	public void IntegerDebit_TamperedOrOutOfRangePlanIsAtomic()
	{
		var fixture = BuildFixture();
		var field = BuildField(fixture, AgricultureFieldUse.Woodland, woodlandYield: 5);
		Assert.IsFalse(field.Changed);
		var valid = PlanDebit(field, NativeOrganicSourceKind.Woodland, 0.25m);
		var tampered = valid with { ClosingPrepaidFraction = 0.5m };

		Assert.IsFalse(field.TryApplyNativeOrganicDebit(tampered, out _));
		var afterTamper = field.InspectNativeOrganicSource(NativeOrganicSourceKind.Woodland);
		Assert.AreEqual(5.0, afterTamper.NativeStock);
		Assert.AreEqual(0m, afterTamper.PrepaidFraction);

		var tooSmall = valid with
		{
			RequestedAmount = NativeOrganicAccountingMath.MinimumDebit / 10m,
			ClosingPrepaidFraction = 0m
		};
		Assert.IsFalse(field.TryApplyNativeOrganicDebit(tooSmall, out _));
		var afterSmall = field.InspectNativeOrganicSource(NativeOrganicSourceKind.Woodland);
		Assert.AreEqual(afterTamper.NativeStock, afterSmall.NativeStock);
		Assert.AreEqual(afterTamper.PrepaidFraction, afterSmall.PrepaidFraction);
		Assert.IsFalse(field.Changed);
		Assert.IsTrue(field.TryApplyNativeOrganicDebit(valid, out _));
		Assert.IsTrue(field.Changed);
	}

	[TestMethod]
	public void Inspection_IsPureAndOrchardUsesTheCropOwnerIdentity()
	{
		var fixture = BuildFixture();
		fixture.Crop.SetupGet(x => x.IsPerennial).Returns(true);
		var orchard = BuildField(fixture, AgricultureFieldUse.Orchard, cropYield: 20);

		var first = orchard.InspectNativeOrganicSource(NativeOrganicSourceKind.Crop);
		var second = orchard.InspectNativeOrganicSource(NativeOrganicSourceKind.Crop);

		Assert.AreEqual("crop", first.Selector);
		Assert.AreEqual(first, second);
		Assert.IsFalse(orchard.Changed);
		fixture.Environment.Verify(x => x.EvaluateOrganicPenalty(It.IsAny<ICell>(),
			It.IsAny<NativeOrganicPenaltyChannel>(), It.IsAny<NativeOrganicPenaltyContext>()), Times.Never);
		fixture.Environment.Verify(x => x.MarkDirty(It.IsAny<ICell>(), It.IsAny<EnvironmentalMagicDirtyReason>()),
			Times.Never);
	}

	[TestMethod]
	public void Accounting_SaveReloadRetainsStockFractionGenerationAndRemainders()
	{
		var fixture = BuildFixture();
		var definition = FieldDefinition(new XElement("Source",
			new XAttribute("kind", "Crop"),
			new XAttribute("generation", 7),
			new XAttribute("revision", 12),
			new XAttribute("prepaid", "0.25"),
			new XAttribute("healthRemainder", "0.5"),
			new XAttribute("yieldRemainder", "0.75"),
			new XAttribute("biomassRemainder", "0")));
		var field = BuildField(fixture, AgricultureFieldUse.Crop, cropYield: 10, definition: definition);
		Assert.IsTrue(ApplyPlannedDebit(field, NativeOrganicSourceKind.Crop, 0.5m));
		var before = field.InspectNativeOrganicSource(NativeOrganicSourceKind.Crop);
		var savedDefinition = SaveDefinition(field).ToString();

		var reloaded = BuildField(fixture, AgricultureFieldUse.Crop, cropYield: (int)before.NativeStock,
			definition: savedDefinition);
		var after = reloaded.InspectNativeOrganicSource(NativeOrganicSourceKind.Crop);
		Assert.AreEqual(before.NativeStock, after.NativeStock);
		Assert.AreEqual(before.PrepaidFraction, after.PrepaidFraction);
		Assert.AreEqual(before.Lifecycle, after.Lifecycle);
		Assert.AreEqual(before.RecoveryRemainders, after.RecoveryRemainders);
		Assert.AreEqual(before.SourceRevision, after.SourceRevision);
	}

	[TestMethod]
	public void Accounting_MalformedSourceIsPreservedUntilExplicitRepair()
	{
		var fixture = BuildFixture();
		var definition = FieldDefinition(new XElement("Source",
			new XAttribute("kind", "Crop"),
			new XAttribute("generation", 4),
			new XAttribute("revision", 2),
			new XAttribute("prepaid", "-0.25"),
			new XAttribute("healthRemainder", "0"),
			new XAttribute("yieldRemainder", "0"),
			new XAttribute("biomassRemainder", "0")));
		var field = BuildField(fixture, AgricultureFieldUse.Crop, cropYield: 10, definition: definition);

		var invalid = field.InspectNativeOrganicSource(NativeOrganicSourceKind.Crop);
		Assert.AreEqual(NativeOrganicSourceStatus.Invalid, invalid.Status);
		Assert.AreEqual("-0.25", SaveDefinition(field)
			.Element("NativeOrganicAccounting")!
			.Elements("Source")
			.Single(x => (string?)x.Attribute("kind") == "Crop")
			.Attribute("prepaid")!.Value);

		Assert.IsTrue(field.RepairNativeOrganicAccounting(NativeOrganicSourceKind.Crop, out var result));
		StringAssert.Contains(result, "without changing native stock");
		var repaired = field.InspectNativeOrganicSource(NativeOrganicSourceKind.Crop);
		Assert.AreEqual(NativeOrganicSourceStatus.Available, repaired.Status);
		Assert.AreEqual(10.0, repaired.NativeStock);
		Assert.AreEqual(0m, repaired.PrepaidFraction);
		Assert.AreEqual(4L, repaired.Lifecycle!.Generation);
	}

	[TestMethod]
	public void Lifecycle_AnnualReplacementClearsCreditButOrchardHarvestRetainsIt()
	{
		var fixture = BuildFixture();
		var accounting = FieldDefinition(new XElement("Source",
			new XAttribute("kind", "Crop"),
			new XAttribute("generation", 7),
			new XAttribute("revision", 3),
			new XAttribute("prepaid", "0.75"),
			new XAttribute("healthRemainder", "0.25"),
			new XAttribute("yieldRemainder", "0.5"),
			new XAttribute("biomassRemainder", "0")));
		var annual = BuildField(fixture, AgricultureFieldUse.Crop, cropYield: 50, definition: accounting);
		var clear = BuildOperation(fixture.Gameworld.Object, AgricultureOperationType.Clear,
			AgricultureFieldUse.Crop, AgricultureFieldUse.Fallow);
		Assert.IsTrue(annual.ApplyOperation(clear, null!, null!, false, out _));
		Assert.AreEqual(NativeOrganicSourceStatus.Absent,
			annual.InspectNativeOrganicSource(NativeOrganicSourceKind.Crop).Status);
		var sow = BuildOperation(fixture.Gameworld.Object, AgricultureOperationType.Sow,
			AgricultureFieldUse.Fallow, AgricultureFieldUse.Crop, AgricultureTargetType.Crop);
		Assert.IsTrue(annual.ApplyOperation(sow, fixture.Crop.Object, null!, false, out _));
		var replacement = annual.InspectNativeOrganicSource(NativeOrganicSourceKind.Crop);
		Assert.AreEqual(8L, replacement.Lifecycle!.Generation);
		Assert.AreEqual(0m, replacement.PrepaidFraction);
		Assert.AreEqual(NativeOrganicRecoveryRemainders.Empty, replacement.RecoveryRemainders);

		fixture.Crop.SetupGet(x => x.IsPerennial).Returns(true);
		var orchard = BuildField(fixture, AgricultureFieldUse.Orchard, cropYield: 50,
			cropStage: AgricultureCropStage.Harvestable, definition: accounting, fieldId: 2);
		var harvest = BuildOperation(fixture.Gameworld.Object, AgricultureOperationType.Harvest,
			AgricultureFieldUse.Orchard, AgricultureFieldUse.Orchard);
		Assert.IsTrue(orchard.ApplyOperation(harvest, null!, null!, false, out _));
		var survivingOrchard = orchard.InspectNativeOrganicSource(NativeOrganicSourceKind.Crop);
		Assert.AreEqual(7L, survivingOrchard.Lifecycle!.Generation);
		Assert.AreEqual(0.75m, survivingOrchard.PrepaidFraction);
	}

	[TestMethod]
	public void Lifecycle_WoodlandReplacementAndPastureReentryAdvanceTheirOwnGenerations()
	{
		var fixture = BuildFixture();
		var woodlandAccounting = FieldDefinition(new XElement("Source",
			new XAttribute("kind", "Woodland"),
			new XAttribute("generation", 4),
			new XAttribute("revision", 3),
			new XAttribute("prepaid", "0.75"),
			new XAttribute("healthRemainder", "0.25"),
			new XAttribute("yieldRemainder", "0.5"),
			new XAttribute("biomassRemainder", "0")));
		var woodland = BuildField(fixture, AgricultureFieldUse.Woodland, woodlandYield: 40,
			definition: woodlandAccounting);
		var replacementOperation = BuildOperation(fixture.Gameworld.Object, AgricultureOperationType.Woodland,
			AgricultureFieldUse.Woodland, AgricultureFieldUse.Woodland, AgricultureTargetType.Woodland);
		Assert.IsTrue(woodland.ApplyOperation(replacementOperation, fixture.Woodland.Object, null!, false, out _));
		var replacement = woodland.InspectNativeOrganicSource(NativeOrganicSourceKind.Woodland);
		Assert.AreEqual(5L, replacement.Lifecycle!.Generation);
		Assert.AreEqual(0m, replacement.PrepaidFraction);
		Assert.AreEqual(NativeOrganicRecoveryRemainders.Empty, replacement.RecoveryRemainders);

		var pastureAccounting = FieldDefinition(new XElement("Source",
			new XAttribute("kind", "Pasture"),
			new XAttribute("generation", 2),
			new XAttribute("revision", 7),
			new XAttribute("prepaid", "0.75"),
			new XAttribute("healthRemainder", "0"),
			new XAttribute("yieldRemainder", "0"),
			new XAttribute("biomassRemainder", "0.5")));
		var pasture = BuildField(fixture, AgricultureFieldUse.Pasture, pasture: 40,
			definition: pastureAccounting, fieldId: 2);
		var leavePasture = BuildOperation(fixture.Gameworld.Object, AgricultureOperationType.Improve,
			AgricultureFieldUse.Pasture, AgricultureFieldUse.Fallow);
		Assert.IsTrue(pasture.ApplyOperation(leavePasture, null!, null!, false, out _));
		Assert.AreEqual(NativeOrganicSourceStatus.Absent,
			pasture.InspectNativeOrganicSource(NativeOrganicSourceKind.Pasture).Status);
		var reenterPasture = BuildOperation(fixture.Gameworld.Object, AgricultureOperationType.Graze,
			AgricultureFieldUse.Fallow, AgricultureFieldUse.Pasture);
		Assert.IsTrue(pasture.ApplyOperation(reenterPasture, null!, null!, false, out _));
		var reestablished = pasture.InspectNativeOrganicSource(NativeOrganicSourceKind.Pasture);
		Assert.AreEqual(3L, reestablished.Lifecycle!.Generation);
		Assert.AreEqual(0m, reestablished.PrepaidFraction);
		Assert.AreEqual(NativeOrganicRecoveryRemainders.Empty, reestablished.RecoveryRemainders);
	}

	[TestMethod]
	public void Recovery_FractionalCropWoodlandAndPastureProgressWithoutSuppressingDebits()
	{
		var fixture = BuildFixture((_, channel, _) => channel switch
		{
			NativeOrganicPenaltyChannel.CropHealthRecovery or NativeOrganicPenaltyChannel.CropYieldRecovery or
				NativeOrganicPenaltyChannel.WoodlandHealthRecovery or NativeOrganicPenaltyChannel.WoodlandYieldRecovery or
				NativeOrganicPenaltyChannel.PastureRecovery => new NativeOrganicPenaltyEvaluation(true, true, 0.25, null),
			_ => NativeOrganicPenaltyEvaluation.Neutral
		});
		var crop = BuildField(fixture, AgricultureFieldUse.Crop, cropHealth: 50, cropYield: 50, nutrients: 100);
		var woodland = BuildField(fixture, AgricultureFieldUse.Woodland, woodlandHealth: 50, woodlandYield: 50,
			woodlandGrowthDays: 2, fieldId: 2);
		var pasture = BuildField(fixture, AgricultureFieldUse.Pasture, pasture: 50, fieldId: 3);

		for (var i = 0; i < 4; i++)
		{
			crop.DailyTick();
			woodland.DailyTick();
			var improve = BuildOperation(fixture.Gameworld.Object, AgricultureOperationType.Improve,
				AgricultureFieldUse.Pasture, AgricultureFieldUse.Pasture,
				scoreDelta: (AgricultureScoreType.Pasture, 1));
			Assert.IsTrue(pasture.ApplyOperation(improve, null!, null!, false, out _));
		}

		Assert.AreEqual(51, crop.CropHealth);
		Assert.AreEqual(51, crop.CropYieldPotential);
		Assert.AreEqual(14, crop.CropGrowthDays);
		Assert.AreEqual(51, woodland.WoodlandHealth);
		Assert.AreEqual(51, woodland.WoodlandYieldPotential);
		Assert.AreEqual(6, woodland.WoodlandGrowthDays);
		Assert.AreEqual(51, pasture.Pasture);

		var grazing = BuildOperation(fixture.Gameworld.Object, AgricultureOperationType.Improve,
			AgricultureFieldUse.Pasture, AgricultureFieldUse.Pasture,
			scoreDelta: (AgricultureScoreType.Pasture, -3));
		Assert.IsTrue(pasture.ApplyOperation(grazing, null!, null!, false, out _));
		Assert.AreEqual(48, pasture.Pasture);
	}

	[TestMethod]
	public void Recovery_InvalidPenaltySuppressesPositiveButNotExistingNegativeStress()
	{
		var fixture = BuildFixture((_, _, _) => NativeOrganicPenaltyEvaluation.Invalid("invalid test factor"));
		var accounting = FieldDefinition(new XElement("Source",
			new XAttribute("kind", "Crop"),
			new XAttribute("generation", 1),
			new XAttribute("revision", 1),
			new XAttribute("prepaid", "0"),
			new XAttribute("healthRemainder", "0.5"),
			new XAttribute("yieldRemainder", "0.5"),
			new XAttribute("biomassRemainder", "0")));
		var healthy = BuildField(fixture, AgricultureFieldUse.Crop, cropHealth: 50, cropYield: 50, nutrients: 100,
			definition: accounting);
		healthy.DailyTick();
		Assert.AreEqual(50, healthy.CropHealth);
		Assert.AreEqual(50, healthy.CropYieldPotential);
		Assert.AreEqual(NativeOrganicRecoveryRemainders.Empty,
			healthy.InspectNativeOrganicSource(NativeOrganicSourceKind.Crop).RecoveryRemainders);

		fixture.Crop.SetupGet(x => x.MinimumMoisture).Returns(60);
		var stressed = BuildField(fixture, AgricultureFieldUse.Crop, cropHealth: 50, cropYield: 50);
		stressed.DailyTick();
		Assert.AreEqual(46, stressed.CropHealth);
		Assert.AreEqual(47, stressed.CropYieldPotential);
	}

	[TestMethod]
	public void Recovery_AtCapacityDiscardsProgressInsteadOfBankingIt()
	{
		var fixture = BuildFixture((_, _, _) => new NativeOrganicPenaltyEvaluation(true, true, 0.25, null));
		var accounting = FieldDefinition(new XElement("Source",
			new XAttribute("kind", "Crop"),
			new XAttribute("generation", 1),
			new XAttribute("revision", 1),
			new XAttribute("prepaid", "0"),
			new XAttribute("healthRemainder", "0.75"),
			new XAttribute("yieldRemainder", "0.75"),
			new XAttribute("biomassRemainder", "0")));
		var field = BuildField(fixture, AgricultureFieldUse.Crop, cropHealth: 100, cropYield: 100, nutrients: 100,
			definition: accounting);

		field.DailyTick();

		Assert.AreEqual(100, field.CropHealth);
		Assert.AreEqual(100, field.CropYieldPotential);
		Assert.AreEqual(NativeOrganicRecoveryRemainders.Empty,
			field.InspectNativeOrganicSource(NativeOrganicSourceKind.Crop).RecoveryRemainders);
	}

	[TestMethod]
	[TestCategory("Y-T19")]
	public void Initialisation_CropWoodlandAndPastureUseTheirFactorsOnce()
	{
		var fixture = BuildFixture((_, channel, _) => channel switch
		{
			NativeOrganicPenaltyChannel.CropInitialisation or
				NativeOrganicPenaltyChannel.WoodlandInitialisation or
				NativeOrganicPenaltyChannel.PastureInitialisation =>
				new NativeOrganicPenaltyEvaluation(true, true, 0.5, null),
			_ => NativeOrganicPenaltyEvaluation.Neutral
		});
		var crop = BuildField(fixture, AgricultureFieldUse.Fallow);
		var woodland = BuildField(fixture, AgricultureFieldUse.Fallow, fieldId: 2);
		var pasture = BuildField(fixture, AgricultureFieldUse.Fallow, pasture: 50, fieldId: 3,
			definition: "<Field><NativeOrganicAccounting version=\"2\" pastureAssessment=\"pending\" /></Field>");
		var poorCrop = BuildField(fixture, AgricultureFieldUse.Fallow, nutrients: 0, fieldId: 4);

		var sow = BuildOperation(fixture.Gameworld.Object, AgricultureOperationType.Sow,
			AgricultureFieldUse.Fallow, AgricultureFieldUse.Crop, AgricultureTargetType.Crop);
		Assert.IsTrue(crop.ApplyOperation(sow, fixture.Crop.Object, null!, false, out _));
		Assert.AreEqual(25, crop.CropHealth);
		Assert.AreEqual(50, crop.CropYieldPotential);
		Assert.AreEqual(0, crop.CropGrowthDays);
		Assert.IsTrue(poorCrop.ApplyOperation(sow, fixture.Crop.Object, null!, false,
			AgricultureWorkOutcome.FromSkill(0.0, 10.0, 0.0, 0.0), out _));
		Assert.AreEqual(21, poorCrop.CropHealth);
		Assert.AreEqual(45, poorCrop.CropYieldPotential);

		var establishWoodland = BuildOperation(fixture.Gameworld.Object, AgricultureOperationType.Woodland,
			AgricultureFieldUse.Fallow, AgricultureFieldUse.Woodland, AgricultureTargetType.Woodland);
		Assert.IsTrue(woodland.ApplyOperation(establishWoodland, fixture.Woodland.Object, null!, false, out _));
		Assert.AreEqual(25, woodland.WoodlandHealth);
		Assert.AreEqual(0, woodland.WoodlandGrowthDays);

		var establishPasture = BuildOperation(fixture.Gameworld.Object, AgricultureOperationType.Graze,
			AgricultureFieldUse.Fallow, AgricultureFieldUse.Pasture,
			scoreDelta: (AgricultureScoreType.Pasture, 20));
		Assert.AreEqual(AgricultureFieldUse.Fallow, pasture.CurrentUse);
		Assert.AreEqual(50, pasture.Pasture);
		fixture.Environment.Verify(x => x.EvaluateOrganicPenalty(It.IsAny<ICell>(),
			NativeOrganicPenaltyChannel.PastureInitialisation, It.IsAny<NativeOrganicPenaltyContext>()), Times.Never,
			"A fallow field has no productive pasture lifecycle to initialise.");
		Assert.IsTrue(pasture.ApplyOperation(establishPasture, null!, null!, false, out _));
		Assert.AreEqual(35, pasture.Pasture);
		fixture.Environment.Verify(x => x.EvaluateOrganicPenalty(fixture.Cell.Object,
			NativeOrganicPenaltyChannel.PastureInitialisation,
			It.Is<NativeOrganicPenaltyContext>(context => context.BaselineIncrease == 70.0)), Times.Once,
			"The staged default and new allocation are assessed together once.");

		fixture.Crop.SetupGet(x => x.IsPerennial).Returns(true);
		var orchard = BuildField(fixture, AgricultureFieldUse.Fallow, fieldId: 5);
		var plantOrchard = BuildOperation(fixture.Gameworld.Object, AgricultureOperationType.PlantOrchard,
			AgricultureFieldUse.Fallow, AgricultureFieldUse.Orchard, AgricultureTargetType.Crop);
		Assert.IsTrue(orchard.ApplyOperation(plantOrchard, fixture.Crop.Object, null!, false, out _));
		Assert.AreEqual(25, orchard.CropHealth);
		Assert.AreEqual(25, orchard.CropYieldPotential);
		Assert.AreEqual(0, orchard.CropGrowthDays);
		Assert.AreEqual(0, orchard.CropHarvestCount);
	}

	[TestMethod]
	[TestCategory("C-R3-01")]
	[TestCategory("C-R3-04")]
	[TestCategory("C-R3-05")]
	public void Pasture_FirstProductiveEntryAssessesStagedDefaultOnceAcrossReload()
	{
		foreach (var (factor, expected) in new[] { (0.0, 0), (0.5, 25), (1.0, 50) })
		{
			var fixture = BuildFixture((_, channel, _) =>
				channel == NativeOrganicPenaltyChannel.PastureInitialisation
					? new NativeOrganicPenaltyEvaluation(true, true, factor, null)
					: NativeOrganicPenaltyEvaluation.Neutral);
			var pending = BuildField(fixture, AgricultureFieldUse.Fallow, pasture: 50,
				definition: "<Field><NativeOrganicAccounting version=\"2\" pastureAssessment=\"pending\" /></Field>");
			var pendingXml = SaveDefinition(pending).ToString();
			StringAssert.Contains(pendingXml, "pastureAssessment=\"pending\"");
			var reloaded = BuildField(fixture, AgricultureFieldUse.Fallow, pasture: 50,
				definition: pendingXml);
			var establish = BuildOperation(fixture.Gameworld.Object, AgricultureOperationType.Graze,
				AgricultureFieldUse.Fallow, AgricultureFieldUse.Pasture);
			Assert.IsTrue(reloaded.ApplyOperation(establish, null!, null!, false, out _));
			Assert.AreEqual(expected, reloaded.Pasture);
			var assessedXml = SaveDefinition(reloaded).ToString();
			StringAssert.Contains(assessedXml, "pastureAssessment=\"assessed\"");
			var afterReload = BuildField(fixture, AgricultureFieldUse.Pasture, pasture: expected,
				definition: assessedXml);
			Assert.AreEqual(expected, afterReload.Pasture);
			var legacy = BuildField(fixture, AgricultureFieldUse.Fallow, pasture: 50);
			Assert.IsTrue(legacy.ApplyOperation(establish, null!, null!, false, out _));
			Assert.AreEqual(50, legacy.Pasture, "Legacy stock must not be assessed retroactively.");
		}
	}

	[TestMethod]
	[TestCategory("C-R3-07")]
	public void Pasture_InvalidInitialFactorDiscardsPendingDefaultWithoutLaterRefill()
	{
		var valid = false;
		var fixture = BuildFixture((_, channel, _) => channel == NativeOrganicPenaltyChannel.PastureInitialisation
			? valid
				? new NativeOrganicPenaltyEvaluation(true, true, 1.0, null)
				: NativeOrganicPenaltyEvaluation.Invalid("Invalid pasture initial factor")
			: NativeOrganicPenaltyEvaluation.Neutral);
		var field = BuildField(fixture, AgricultureFieldUse.Fallow, pasture: 50,
			definition: "<Field><NativeOrganicAccounting version=\"2\" pastureAssessment=\"pending\" /></Field>");
		var enter = BuildOperation(fixture.Gameworld.Object, AgricultureOperationType.Graze,
			AgricultureFieldUse.Fallow, AgricultureFieldUse.Pasture);
		Assert.IsTrue(field.ApplyOperation(enter, null!, null!, false, out _));
		Assert.AreEqual(0, field.Pasture);
		StringAssert.Contains(SaveDefinition(field).ToString(), "pastureAssessment=\"assessed\"");
		valid = true;
		var leave = BuildOperation(fixture.Gameworld.Object, AgricultureOperationType.Improve,
			AgricultureFieldUse.Pasture, AgricultureFieldUse.Fallow);
		Assert.IsTrue(field.ApplyOperation(leave, null!, null!, false, out _));
		Assert.IsTrue(field.ApplyOperation(enter, null!, null!, false, out _));
		Assert.AreEqual(0, field.Pasture);
	}

	private static bool ApplyPlannedDebit(AgricultureField field, NativeOrganicSourceKind kind, decimal amount)
	{
		return field.TryApplyNativeOrganicDebit(PlanDebit(field, kind, amount), out _);
	}

	private static NativeOrganicDebitPlan PlanDebit(AgricultureField field, NativeOrganicSourceKind kind,
		decimal amount)
	{
		var snapshot = field.InspectNativeOrganicSource(kind);
		Assert.IsTrue(snapshot.IsEligible);
		Assert.IsNotNull(snapshot.Lifecycle);
		Assert.IsTrue(NativeOrganicAccountingMath.TryPlanInteger((int)snapshot.NativeStock,
			snapshot.PrepaidFraction, amount, out var wholeDebit, out var closingPrepaid, out var error), error);
		return new NativeOrganicDebitPlan(snapshot.Selector, kind, snapshot.Lifecycle!, snapshot.SourceRevision,
			1L, 1L, amount, snapshot.PrepaidFraction, wholeDebit, closingPrepaid, snapshot.NativeStock);
	}

	private static XElement SaveDefinition(AgricultureField field)
	{
		var method = typeof(AgricultureField).GetMethod("SaveFieldDefinition",
			BindingFlags.Instance | BindingFlags.NonPublic);
		return (XElement)method!.Invoke(field, null)!;
	}

	private static string FieldDefinition(params XElement[] sources)
	{
		return new XElement("Field",
			new XElement("NativeOrganicAccounting",
				new XAttribute("version", 1),
				sources),
			new XElement("CustomScores")).ToString();
	}

	private static AgricultureOperation BuildOperation(IFuturemud gameworld, AgricultureOperationType type,
		AgricultureFieldUse requiredUse, AgricultureFieldUse resultUse,
		AgricultureTargetType targetType = AgricultureTargetType.None,
		(AgricultureScoreType Score, int Delta)? scoreDelta = null)
	{
		return new AgricultureOperation(new MudSharp.Models.AgricultureOperation
		{
			Id = (long)type + 1L,
			Name = $"Test {type}",
			Description = "Native organic accounting test operation.",
			OperationType = (int)type,
			TargetType = (int)targetType,
			RequiredUse = (int)requiredUse,
			ResultUse = (int)resultUse,
			Definition = new XElement("Operation",
				scoreDelta.HasValue
					? new XElement("Score",
						new XAttribute("type", scoreDelta.Value.Score),
						new XAttribute("value", scoreDelta.Value.Delta))
					: null).ToString()
		}, gameworld);
	}

	private static AgricultureField BuildField(Fixture fixture, AgricultureFieldUse use, int cropHealth = 50,
		int cropYield = 0, AgricultureCropStage cropStage = AgricultureCropStage.Growing,
		int woodlandHealth = 50, int woodlandYield = 0, int woodlandGrowthDays = 1, int pasture = 50,
		int nutrients = 50, string? definition = null, long fieldId = 1)
	{
		var model = new MudSharp.Models.AgricultureField
		{
			Id = fieldId,
			CellId = 1,
			ProfileId = 1,
			CurrentUse = (int)use,
			Moisture = 50,
			Drainage = 50,
			Nutrients = nutrients,
			Salinity = 0,
			Topsoil = 50,
			Tilth = 50,
			Rockiness = 0,
			Weeds = 0,
			Pests = 0,
			Fence = 50,
			Pasture = pasture,
			Condition = 50,
			Definition = definition ?? "<Field />"
		};
		if (use is AgricultureFieldUse.Crop or AgricultureFieldUse.Orchard)
		{
			model.AgricultureFieldCrop = new MudSharp.Models.AgricultureFieldCrop
			{
				AgricultureFieldId = fieldId,
				CropDefinitionId = fixture.Crop.Object.Id,
				Stage = (int)cropStage,
				GrowthDays = cropStage == AgricultureCropStage.Harvestable ? 30 : 10,
				Health = cropHealth,
				YieldPotential = cropYield,
				Definition = "<Crop />"
			};
		}

		if (use == AgricultureFieldUse.Woodland)
		{
			model.AgricultureFieldWoodland = new MudSharp.Models.AgricultureFieldWoodland
			{
				AgricultureFieldId = fieldId,
				WoodlandDefinitionId = fixture.Woodland.Object.Id,
				GrowthDays = woodlandGrowthDays,
				Health = woodlandHealth,
				YieldPotential = woodlandYield,
				Definition = "<Woodland />"
			};
		}

		return new AgricultureField(model, fixture.Gameworld.Object);
	}

	private static Fixture BuildFixture(
		Func<ICell, NativeOrganicPenaltyChannel, NativeOrganicPenaltyContext, NativeOrganicPenaltyEvaluation>?
			evaluate = null)
	{
		var gameworld = new Mock<IFuturemud>();
		gameworld.SetupGet(x => x.SaveManager).Returns(new Mock<ISaveManager>().Object);
		var cell = new Mock<ICell>();
		cell.SetupGet(x => x.Id).Returns(1L);
		cell.SetupGet(x => x.Name).Returns("Test Cell");
		cell.SetupGet(x => x.FrameworkItemType).Returns("Cell");
		cell.SetupGet(x => x.Gameworld).Returns(gameworld.Object);
		cell.Setup(x => x.CurrentTemperature(It.IsAny<IPerceiver>())).Returns(20.0);
		cell.Setup(x => x.CurrentWeather(It.IsAny<IPerceiver>())).Returns(default(IWeatherEvent)!);
		var cells = new All<ICell>();
		cells.Add(cell.Object);
		gameworld.SetupGet(x => x.Cells).Returns(cells);

		var profile = new Mock<IAgricultureFieldProfile>();
		profile.SetupGet(x => x.Id).Returns(1L);
		profile.SetupGet(x => x.Name).Returns("Test Profile");
		profile.SetupGet(x => x.FrameworkItemType).Returns("AgricultureFieldProfile");
		profile.SetupGet(x => x.DefaultScores).Returns(new Dictionary<AgricultureScoreType, int>());
		profile.Setup(x => x.AllowsUse(It.IsAny<AgricultureFieldUse>())).Returns(true);
		var profiles = new All<IAgricultureFieldProfile>();
		profiles.Add(profile.Object);
		gameworld.SetupGet(x => x.AgricultureFieldProfiles).Returns(profiles);

		var crop = new Mock<IAgricultureCropDefinition>();
		crop.SetupGet(x => x.Id).Returns(1L);
		crop.SetupGet(x => x.Name).Returns("Test Crop");
		crop.SetupGet(x => x.FrameworkItemType).Returns("AgricultureCropDefinition");
		crop.SetupGet(x => x.BaseGrowthDays).Returns(30);
		crop.SetupGet(x => x.HarvestWindowDays).Returns(5);
		crop.SetupGet(x => x.MinimumMoisture).Returns(0);
		crop.SetupGet(x => x.MaximumMoisture).Returns(100);
		crop.SetupGet(x => x.MinimumTemperature).Returns(0);
		crop.SetupGet(x => x.MaximumTemperature).Returns(40);
		crop.SetupGet(x => x.HarvestCycleDays).Returns(30);
		crop.SetupGet(x => x.PlantingWindows).Returns(Array.Empty<AgriculturePlantingWindow>());
		crop.SetupGet(x => x.ScoreRanges).Returns(Array.Empty<AgricultureScoreRange>());
		crop.SetupGet(x => x.YieldOutputs).Returns(Array.Empty<AgricultureCommodityYield>());
		var crops = new All<IAgricultureCropDefinition>();
		crops.Add(crop.Object);
		gameworld.SetupGet(x => x.AgricultureCropDefinitions).Returns(crops);

		var woodland = new Mock<IAgricultureWoodlandDefinition>();
		woodland.SetupGet(x => x.Id).Returns(1L);
		woodland.SetupGet(x => x.Name).Returns("Test Woodland");
		woodland.SetupGet(x => x.FrameworkItemType).Returns("AgricultureWoodlandDefinition");
		woodland.SetupGet(x => x.EstablishmentDays).Returns(0);
		woodland.SetupGet(x => x.YieldOutputs).Returns(Array.Empty<AgricultureCommodityYield>());
		var woodlands = new All<IAgricultureWoodlandDefinition>();
		woodlands.Add(woodland.Object);
		gameworld.SetupGet(x => x.AgricultureWoodlandDefinitions).Returns(woodlands);

		gameworld.SetupGet(x => x.Properties).Returns(new All<IProperty>());
		gameworld.SetupGet(x => x.AgricultureFields).Returns(new All<IAgricultureField>());
		var environment = new Mock<IEnvironmentalMagicService>();
		environment
			.Setup(x => x.EvaluateOrganicPenalty(It.IsAny<ICell>(), It.IsAny<NativeOrganicPenaltyChannel>(),
				It.IsAny<NativeOrganicPenaltyContext>()))
			.Returns((ICell sourceCell, NativeOrganicPenaltyChannel channel, NativeOrganicPenaltyContext context) =>
				evaluate?.Invoke(sourceCell, channel, context) ?? NativeOrganicPenaltyEvaluation.Neutral);
		gameworld.SetupGet(x => x.EnvironmentalMagic).Returns(environment.Object);
		return new Fixture(gameworld, cell, crop, woodland, environment);
	}

	private sealed record Fixture(Mock<IFuturemud> Gameworld, Mock<ICell> Cell,
		Mock<IAgricultureCropDefinition> Crop, Mock<IAgricultureWoodlandDefinition> Woodland,
		Mock<IEnvironmentalMagicService> Environment);
}
