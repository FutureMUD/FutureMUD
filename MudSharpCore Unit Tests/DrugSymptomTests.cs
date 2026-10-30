using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Body;
using MudSharp.Body.Needs;
using MudSharp.Body.Traits;
using MudSharp.Character;
using MudSharp.Effects;
using MudSharp.Effects.Concrete;
using MudSharp.Framework;
using MudSharp.FutureProg;
using MudSharp.Health;
using MudSharp.Models;
using MudSharp.FutureProg.Functions.Health;
using MudSharp.RPG.Checks;

#nullable enable
namespace MudSharp_Unit_Tests;

[TestClass]
public class DrugSymptomTests
{
	[TestMethod]
	public void OralConsumptionHelper_RefusesSleepingAndGenerallyBlockedActors()
	{
		var actor = new Mock<ICharacter>();
		var body = new Mock<IBody>();
		var item = new Mock<MudSharp.GameItems.IGameItem>();
		actor.SetupGet(x => x.Body).Returns(body.Object);
		actor.SetupGet(x => x.GetObject).Returns(actor.Object);
		body.SetupGet(x => x.Actor).Returns(actor.Object);
		item.SetupGet(x => x.GetObject).Returns(item.Object);
		var function = new MudSharp.FutureProg.Functions.GameItem.SwallowItemFunction([
			new MudSharp.FutureProg.Functions.ConstantFunction(actor.Object),
			new MudSharp.FutureProg.Functions.ConstantFunction(item.Object)]);
		actor.SetupGet(x => x.State).Returns(CharacterState.Sleeping);
		Assert.AreEqual(StatementResult.Normal, function.Execute(Mock.Of<IVariableSpace>()));
		Assert.AreEqual(false, function.Result.GetObject);
		actor.SetupGet(x => x.State).Returns(CharacterState.Awake);
		var blocker = new Mock<IEffect>();
		blocker.Setup(x => x.Applies()).Returns(true);
		blocker.Setup(x => x.IsBlockingEffect("general")).Returns(true);
		actor.SetupGet(x => x.Effects).Returns(new[] { blocker.Object });
		Assert.AreEqual(StatementResult.Normal, function.Execute(Mock.Of<IVariableSpace>()));
		Assert.AreEqual(false, function.Result.GetObject);
		item.Verify(x => x.GetItemType<MudSharp.GameItems.Interfaces.ISwallowable>(), Times.Never());
	}

	[TestMethod]
	public void DrugAdministrationPolicy_FixedDoseAndDuplicateRefusalAreIndependentOfEffects()
	{
		var drug = new MudSharp.Health.Drug(new MudSharp.Models.Drug { Id = 71,
			DoseBehaviour = (int)(DrugDoseBehaviour.FixedDose | DrugDoseBehaviour.IgnoreAdditionalExposure),
			FixedDoseGrams = 0.05 }, Mock.Of<IFuturemud>());
		Assert.AreEqual(0.05, DrugDosePolicy.Normalise(drug, 0.0001));
		Assert.AreEqual(0.05, DrugDosePolicy.Normalise(drug, 1));
		Assert.AreEqual(0.0, DrugDosePolicy.Normalise(drug, 0));
		Assert.AreEqual(0.0, DrugDosePolicy.Normalise(drug, double.NaN));
		var body = new Mock<IBody>();
		body.SetupGet(x => x.ActiveDrugDosages).Returns(Array.Empty<DrugDosage>());
		body.SetupGet(x => x.LatentDrugDosages).Returns(Array.Empty<DrugDosage>());
		Assert.IsTrue(DrugDosePolicy.CanAccept(body.Object, drug));
		body.SetupGet(x => x.LatentDrugDosages).Returns(new[] { new DrugDosage { Drug = drug, Grams = 0.001 } });
		Assert.IsFalse(DrugDosePolicy.CanAccept(body.Object, drug));
		body.SetupGet(x => x.LatentDrugDosages).Returns(Array.Empty<DrugDosage>());
		body.SetupGet(x => x.ActiveDrugDosages).Returns(new[] { new DrugDosage { Drug = drug, Grams = 0.001 } });
		Assert.IsFalse(DrugDosePolicy.CanAccept(body.Object, drug));
		var normal = new MudSharp.Health.Drug(new MudSharp.Models.Drug { Id = 72 }, Mock.Of<IFuturemud>());
		Assert.AreEqual(0.001, DrugDosePolicy.Normalise(normal, 0.001));
		Assert.IsTrue(DrugDosePolicy.CanAccept(body.Object, normal));
	}

	[TestMethod]
	public void Symptoms_MetadataRoundTripsAndMalformedLegacyDataIsInert()
	{
		var config = new DrugSymptomsAdditionalInfo { ResistanceTraitId = 42, InitialStage = 1,
			Stages = [new(), new() { Chance = "0.5", Damage = "rand(1, 5)", NextStages = [0], Echoes = ["A strange vision."] }] };
		var drug = new MudSharp.Models.Drug { Id = 1, Name = "Toxin", IntensityPerGram = 1 };
		drug.DrugsIntensities.Add(new DrugIntensity { DrugType = (int)DrugType.Symptoms, RelativeIntensity = 1, AdditionalEffects = config.DatabaseString });
		var loaded = new MudSharp.Health.Drug(drug, Mock.Of<IFuturemud>()).AdditionalInfoFor<DrugSymptomsAdditionalInfo>(DrugType.Symptoms);
		Assert.AreEqual(42L, loaded.ResistanceTraitId);
		Assert.AreEqual(1, loaded.InitialStage);
		Assert.AreEqual("rand(1, 5)", loaded.Stages[1].Damage);
		Assert.AreEqual("0", DrugSymptomsAdditionalInfo.Parse("broken").Stages.Single().Chance);
		var normalised = DrugSymptomsAdditionalInfo.Parse("{\"Stages\":[{\"Echoes\":null,\"NextStages\":null}]}");
		Assert.AreEqual(0, normalised.Stages.Single().Echoes.Count);
		Assert.AreEqual(0, normalised.Stages.Single().NextStages.Count);
	}

	[TestMethod]
	public void Symptoms_NonnumericFormulaIsInertAndTransitionRespectsDestinationAwakeGate()
	{
		var config = new DrugSymptomsAdditionalInfo { Stages = [new() { Chance = "'oops'", Stamina = "5" }] };
		var (body, actor, effect, _) = Fixture(config);
		effect.Advance(1);
		body.Verify(x => x.SpendStamina(It.IsAny<double>()), Times.Never());
		config.Stages = [new() { TransitionChance = "1", NextStages = [1] },
			new() { AwakeOnly = true, Chance = "1", Stamina = "5" }];
		actor.SetupGet(x => x.State).Returns(CharacterState.Sleeping);
		effect.Advance(1);
		Assert.AreEqual(1, effect.Stage);
		body.Verify(x => x.SpendStamina(It.IsAny<double>()), Times.Never());
	}

	[TestMethod]
	public void Symptoms_AdvanceOnlyElapsedSecondsAndPersistStageWithoutOfflineDamage()
	{
		var config = new DrugSymptomsAdditionalInfo { Stages = [
			new() { TransitionChance = "1", NextStages = [1] }, new() { Chance = "1", Stamina = "5" }] };
		var (body, _, effect, world) = Fixture(config);
		effect.Advance(0);
		body.Verify(x => x.SpendStamina(It.IsAny<double>()), Times.Never());
		effect.Advance(2);
		body.Verify(x => x.SpendStamina(5), Times.Exactly(2));
		Assert.AreEqual(1, effect.Stage);
		DrugSymptomEffect.InitialiseEffectType();
		world.SetupGet(x => x.FutureProgs).Returns(new All<IFutureProg>());
		var restored = (DrugSymptomEffect)Effect.LoadEffect(effect.SaveToXml(new Dictionary<IEffect, TimeSpan>()), body.Object);
		Assert.AreEqual(1, restored.Stage);
		Assert.AreEqual(effect.DrugId, restored.DrugId);
		restored.Advance(0);
		body.Verify(x => x.SpendStamina(5), Times.Exactly(2));
	}

	[TestMethod]
	public void Symptoms_PreserveOptInSleepingLethalityAndStopAfterBodyChange()
	{
		var config = new DrugSymptomsAdditionalInfo { Stages = [new() { Chance = "1", KillSleeping = true }] };
		var (_, actor, effect, _) = Fixture(config);
		actor.SetupGet(x => x.State).Returns(CharacterState.Sleeping);
		effect.Advance(1);
		actor.Verify(x => x.Die(), Times.Once());
		actor.SetupGet(x => x.Body).Returns(Mock.Of<IBody>());
		effect.Advance(1);
		actor.Verify(x => x.Die(), Times.Once());
	}

	[TestMethod]
	[DataRow(Outcome.Pass, true)]
	[DataRow(Outcome.Fail, false)]
	public void PoisonResistance_UsesDedicatedNativeCheckAndRejectsBodyChanges(Outcome outcome, bool expected)
	{
		var world = new Mock<IFuturemud>();
		var actor = new Mock<ICharacter>();
		var body = new Mock<IBody>();
		actor.SetupGet(x => x.Gameworld).Returns(world.Object);
		actor.SetupGet(x => x.Body).Returns(body.Object);
		actor.SetupGet(x => x.State).Returns(CharacterState.Awake);
		body.SetupGet(x => x.Actor).Returns(actor.Object);
		var check = new Mock<ICheck>();
		world.Setup(x => x.GetCheck(CheckType.PoisonResistanceCheck)).Returns(check.Object);
		check.Setup(x => x.Check(actor.Object, Difficulty.Normal, (IPerceivable)null!, null, 0.0,
			TraitUseType.Practical, It.IsAny<(string, object)[]>())).Returns(new CheckOutcome { Outcome = outcome });
		Assert.AreEqual<bool?>(expected, ResistPoisonFunction.TryResist(actor.Object, Difficulty.Normal));
		check.Setup(x => x.Check(actor.Object, Difficulty.Normal, (IPerceivable)null!, null, 0.0,
			TraitUseType.Practical, It.IsAny<(string, object)[]>())).Returns(() =>
			{
				actor.SetupGet(x => x.Body).Returns(Mock.Of<IBody>());
				return new CheckOutcome { Outcome = Outcome.Fail };
			});
		Assert.IsNull(ResistPoisonFunction.TryResist(actor.Object, Difficulty.Normal));
	}

	[TestMethod]
	public void Symptoms_CureCallbackStopsRemainingPermanentOnsetChanges()
	{
		var config = new DrugSymptomsAdditionalInfo { OnsetPerDose = true,
			OnsetTraitChanges = [new() { TraitId = 42, Chance = "1", Amount = -1 }, new() { TraitId = 43, Chance = "1", Amount = -1 }] };
		var (body, actor, effect, world) = Fixture(config);
		var first = Mock.Of<ITraitDefinition>(x => x.Id == 42L);
		var second = Mock.Of<ITraitDefinition>(x => x.Id == 43L);
		var traits = new All<ITraitDefinition>(); traits.Add(first); traits.Add(second);
		world.SetupGet(x => x.Traits).Returns(traits);
		actor.Setup(x => x.HasTrait(It.IsAny<ITraitDefinition>())).Returns(true);
		actor.Setup(x => x.TraitRawValue(It.IsAny<ITraitDefinition>())).Returns(20);
		actor.Setup(x => x.SetTraitValue(first, 19)).Callback(() => body
			.Setup(x => x.EffectsOfType<DrugSymptomEffect>(It.IsAny<Predicate<DrugSymptomEffect>>())).Returns(Array.Empty<DrugSymptomEffect>())).Returns(true);
		effect.ApplyAdministeredDose(1);
		actor.Verify(x => x.SetTraitValue(first, 19), Times.Once());
		actor.Verify(x => x.SetTraitValue(second, It.IsAny<double>()), Times.Never());
	}

	[TestMethod]
	public void Symptoms_PermanentConsequencesUseRawTraitsAndDoNotReplayOnReconciliation()
	{
		var config = new DrugSymptomsAdditionalInfo { OnsetPerDose = true,
			OnsetTraitChanges = [new() { TraitId = 42, Chance = "1", Amount = -1, MinimumValue = 1 }],
			ExpiryTraitChanges = [new() { TraitId = 42, Chance = "1", Amount = -1, MinimumValue = 0 }] };
		var (_, actor, effect, world) = Fixture(config);
		var trait = Mock.Of<ITraitDefinition>(x => x.Id == 42L);
		var traits = new All<ITraitDefinition>(); traits.Add(trait);
		world.SetupGet(x => x.Traits).Returns(traits);
		actor.Setup(x => x.HasTrait(trait)).Returns(true);
		var rawValue = 20.0;
		actor.Setup(x => x.TraitRawValue(trait)).Returns(() => rawValue);
		actor.Setup(x => x.SetTraitValue(trait, It.IsAny<double>())).Callback<ITraitDefinition, double>((_, value) => rawValue = value).Returns(true);
		effect.ApplyAdministeredDose(1); effect.ApplyAdministeredDose(1);
		effect.ApplyOnset();
		Assert.AreEqual(18.0, rawValue);
		effect.ApplyOnset();
		Assert.AreEqual(18.0, rawValue);
		effect.ApplyNaturalExpiry();
		effect.ApplyNaturalExpiry();
		Assert.AreEqual(17.0, rawValue);
	}

	[TestMethod]
	public void Symptoms_ConfiguredTrialIntervalPreservesThreeSecondCadence()
	{
		var config = new DrugSymptomsAdditionalInfo { TickIntervalSeconds = 3,
			Stages = [new() { Chance = "1", Stamina = "5" }] };
		var (body, _, effect, _) = Fixture(config);
		effect.Advance(10);
		body.Verify(x => x.SpendStamina(5), Times.Exactly(3));
	}

	[TestMethod]
	public void Symptoms_ReactivationDiscardsPendingInactiveTime()
	{
		var config = new DrugSymptomsAdditionalInfo { Stages = [new() { Chance = "1", Stamina = "5" }] };
		var (body, _, effect, _) = Fixture(config);
		effect.Advance(0.75);
		effect.ResetClock();
		effect.Advance(0.25);
		body.Verify(x => x.SpendStamina(It.IsAny<double>()), Times.Never());
		effect.Advance(0.75);
		body.Verify(x => x.SpendStamina(5), Times.Once());
	}

	[TestMethod]
	public void Symptoms_TransitionWeightsCanExcludeADestination()
	{
		var config = new DrugSymptomsAdditionalInfo { Stages = [
			new() { TransitionChance = "1", NextStages = [1, 2], NextStageWeights = new() { [1] = "0", [2] = "1" } },
			new() { Chance = "1", Stamina = "99" }, new() { Chance = "1", Stamina = "5" }] };
		var (body, _, effect, _) = Fixture(config);
		effect.Advance(1);
		Assert.AreEqual(2, effect.Stage);
		body.Verify(x => x.SpendStamina(5), Times.Once());
		body.Verify(x => x.SpendStamina(99), Times.Never());
	}

	[TestMethod]
	public void Symptoms_CureDuringStaminaCallbackPreventsFurtherEpisodes()
	{
		var config = new DrugSymptomsAdditionalInfo { Stages = [new() { Chance = "1", Stamina = "5" }] };
		var (body, _, effect, _) = Fixture(config);
		body.Setup(x => x.SpendStamina(5)).Callback(() => body
			.Setup(x => x.EffectsOfType<DrugSymptomEffect>(It.IsAny<Predicate<DrugSymptomEffect>>())).Returns(Array.Empty<DrugSymptomEffect>()));
		effect.Advance(10);
		body.Verify(x => x.SpendStamina(5), Times.Once());
	}

	[TestMethod]
	public void Symptoms_TimedCapacityPenaltyUsesTheSameDamageRollAndIndependentTimer()
	{
		var config = new DrugSymptomsAdditionalInfo { Stages = [new() { Chance = "1", Damage = "3", MaximumHitPointsBonus = "-damage", MaximumHitPointsDurationSeconds = 5400, MaximumHitPointsCapacityOnly = true }] };
		var (body, _, effect, _) = Fixture(config);
		effect.Advance(1);
		body.Verify(x => x.AddEffect(It.Is<TimedMaximumHitPointsEffect>(penalty => penalty.MaximumHitPointsBonus == -3 && penalty.SourceDrugId == 1 && penalty.CapacityOnly), TimeSpan.FromMinutes(90)), Times.Once());
	}

	[TestMethod]
	public void Symptoms_HeramidePolicyBlocksOnlyNaturalRecoveryWhileUnconscious()
	{
		var config = new DrugSymptomsAdditionalInfo { Stages = [new() { PreventNaturalStunRecovery = true, RecoveryBlockOnlyWhenUnconscious = true }] };
		var (_, actor, effect, _) = Fixture(config);
		Assert.IsFalse(effect.PreventNaturalStunRecovery);
		actor.SetupGet(x => x.State).Returns(CharacterState.Unconscious);
		Assert.IsTrue(effect.PreventNaturalStunRecovery);
		effect.Intensity = 0;
		Assert.IsFalse(effect.PreventNaturalStunRecovery);
	}

	[TestMethod]
	public void Symptoms_GrishenExhaustionUsesNativeSleepAndPeraineOptsIntoSelectiveCessation()
	{
		var config = new DrugSymptomsAdditionalInfo { Stages = [new() { Chance = "1", Stamina = "5", SleepAtZeroStamina = true, CeaseCombatOnParalysis = true }] };
		var (body, actor, effect, _) = Fixture(config);
		body.SetupGet(x => x.CurrentStamina).Returns(0);
		Assert.IsTrue(effect.CeaseCombatOnParalysis);
		effect.Advance(1);
		actor.Verify(x => x.Sleep(null), Times.Once());
		body.Verify(x => x.SpendStamina(It.IsAny<double>()), Times.Never());
	}

	[TestMethod]
	public void Outcome_SitUsesForcedNativeTransitionOnlyAboveSittingHeight()
	{
		var (body, actor, _, _) = Fixture(new());
		actor.SetupGet(x => x.PositionState).Returns(MudSharp.Body.Position.PositionStates.PositionStanding.Instance);
		DrugSymptomEffect.ApplyOutcome(body.Object, new() { Action = DrugSymptomAction.Sit });
		actor.Verify(x => x.MovePosition(MudSharp.Body.Position.PositionStates.PositionSitting.Instance,
			MudSharp.Body.Position.PositionModifier.None, null, null, null, true, true), Times.Once());
		actor.SetupGet(x => x.PositionState).Returns(MudSharp.Body.Position.PositionStates.PositionLyingDown.Instance);
		DrugSymptomEffect.ApplyOutcome(body.Object, new() { Action = DrugSymptomAction.Sit });
		actor.Verify(x => x.MovePosition(MudSharp.Body.Position.PositionStates.PositionSitting.Instance,
			MudSharp.Body.Position.PositionModifier.None, null, null, null, true, true), Times.Once());
	}

	[TestMethod]
	public void Outcome_BabbleUsesNativeTimedEffectAndDoesNotRefreshExistingBabble()
	{
		var (body, actor, _, world) = Fixture(new());
		actor.SetupGet(x => x.Gameworld).Returns(world.Object);
		var outcome = new DrugSymptomOutcome { Action = DrugSymptomAction.Babble, DurationSeconds = 600 };
		DrugSymptomEffect.ApplyOutcome(body.Object, outcome);
		actor.Verify(x => x.AddEffect(It.IsAny<SymptomBabbleEffect>(), TimeSpan.FromMinutes(10)), Times.Once());
		var existing = new Mock<MudSharp.Effects.Interfaces.IBabbleSpeechEffect>();
		existing.Setup(x => x.Applies()).Returns(true);
		actor.Setup(x => x.CombinedEffectsOfType<MudSharp.Effects.Interfaces.IBabbleSpeechEffect>()).Returns(new[] { existing.Object });
		DrugSymptomEffect.ApplyOutcome(body.Object, outcome);
		actor.Verify(x => x.AddEffect(It.IsAny<SymptomBabbleEffect>(), TimeSpan.FromMinutes(10)), Times.Once());
	}

	[TestMethod]
	public void Outcome_DropsWieldedEquipmentButPreservesAbstractInventory()
	{
		var (body, actor, _, _) = Fixture(new());
		var weapon = Mock.Of<MudSharp.GameItems.IGameItem>();
		var inventory = Mock.Of<MudSharp.GameItems.IGameItem>();
		actor.SetupGet(x => x.Location).Returns(Mock.Of<MudSharp.Construction.IRoom>());
		body.SetupGet(x => x.WieldedItems).Returns(new[] { weapon });
		body.SetupGet(x => x.HeldItems).Returns(new[] { inventory });
		DrugSymptomEffect.ApplyOutcome(body.Object, new() { Action = DrugSymptomAction.DropReadiedItems });
		body.Verify(x => x.Drop(weapon, 0, false, null, false), Times.Once());
		body.Verify(x => x.Drop(inventory, 0, false, null, false), Times.Never());
	}

	[TestMethod]
	public void Outcome_ResidueUsesNativeBodySurfaceAndRoundTripsConfiguration()
	{
		var (body, actor, _, world) = Fixture(new());
		actor.SetupGet(x => x.Gameworld).Returns(world.Object);
		var material = new Mock<MudSharp.Form.Material.ISolid>();
		material.SetupGet(x => x.Id).Returns(42);
		var materials = new All<MudSharp.Form.Material.ISolid>(); materials.Add(material.Object);
		world.SetupGet(x => x.Materials).Returns(materials);
		var surface = new Mock<MudSharp.Form.Material.ISurfaceLiquidState>();
		body.SetupGet(x => x.SurfaceLiquidState).Returns(surface.Object);
		var info = new DrugSymptomsAdditionalInfo { Stages = [new() { Outcomes = [new() { Action = DrugSymptomAction.Residue, ResidueMaterialId = 42, ResidueKilograms = 0.01 }] }] };
		var restored = DrugSymptomsAdditionalInfo.Parse(info.DatabaseString);
		DrugSymptomEffect.ApplyOutcome(body.Object, restored.Stages[0].Outcomes[0]);
		surface.Verify(x => x.DepositResidue(material.Object, 0.01), Times.Once());
	}

	private static (Mock<IBody>, Mock<ICharacter>, DrugSymptomEffect, Mock<IFuturemud>) Fixture(DrugSymptomsAdditionalInfo config)
	{
		var world = new Mock<IFuturemud>();
		var drug = new Mock<IDrug>();
		drug.SetupGet(x => x.Id).Returns(1L);
		drug.Setup(x => x.AdditionalInfoFor<DrugSymptomsAdditionalInfo>(DrugType.Symptoms)).Returns(config);
		var drugs = new All<IDrug>(); drugs.Add(drug.Object);
		world.SetupGet(x => x.Drugs).Returns(drugs);
		world.SetupGet(x => x.Traits).Returns(new All<ITraitDefinition>());
		var actor = new Mock<ICharacter>();
		var body = new Mock<IBody>();
		body.SetupGet(x => x.Gameworld).Returns(world.Object);
		body.SetupGet(x => x.Actor).Returns(actor.Object);
		body.SetupGet(x => x.NeedsModel).Returns(Mock.Of<INeedsModel>());
		actor.SetupGet(x => x.Body).Returns(body.Object);
		actor.SetupGet(x => x.State).Returns(CharacterState.Awake);
		var effect = new DrugSymptomEffect(body.Object, 1, config.InitialStage) { Intensity = 1 };
		body.Setup(x => x.EffectsOfType<DrugSymptomEffect>(It.IsAny<Predicate<DrugSymptomEffect>>())).Returns(new[] { effect });
		return (body, actor, effect, world);
	}
}
