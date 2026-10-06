using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Body.Traits;
using MudSharp.Character;
using MudSharp.Framework;
using MudSharp.Framework.Scheduling;
using MudSharp.Magic;
using MudSharp.Magic.Generators;
using MudSharp.RPG.Checks;
using MudSharp.RPG.Merits;
using MudSharp.RPG.Merits.Interfaces;
using MudSharp.Effects;
using MudSharp.Body;
using MudSharp.Communication.Language;
using MudSharp.Effects.Interfaces;
using MudSharp.Effects.Concrete;
using MudSharp.Effects.Concrete.SpellEffects;
using ConcreteCharacter = MudSharp.Character.Character;

#nullable enable
namespace MudSharp_Unit_Tests;

[TestClass]
public class MagicCastingOwnershipTests
{
	[TestMethod]
	public void SecondarySkillAcquisition_UsesOneNativeTraitAndPropagatesLinkedLanguages()
	{
		var f = new MagicCastingFixture();
		var primary = Holder.Create(f.World.Object, 100); var secondary = Holder.Create(f.World.Object, 101); secondary.Canonical = primary;
		Set(primary, "_secondaryInstances", new List<ICharacterInstance> { secondary });
		var spoken = new Mock<ILanguage>(); spoken.SetupGet(x => x.LinkedTrait).Returns(f.Traits[0]); spoken.SetupGet(x => x.Accents).Returns([]);
		var signed = new Mock<ISignedLanguage>(); signed.SetupGet(x => x.LinkedTrait).Returns(f.Traits[0]);
		f.World.SetupGet(x => x.Languages).Returns(MagicCastingFixture.Collection<ILanguage>(() => [spoken.Object]));
		f.World.SetupGet(x => x.SignedLanguages).Returns(MagicCastingFixture.Collection<ISignedLanguage>(() => [signed.Object]));
		Mock.Get(f.Traits[0]).Setup(x => x.NewTrait(primary, 10)).Returns(f.NativeSkill.Object);
		f.NativeSkill.SetupGet(x => x.Definition).Returns(f.Traits[0]);
		Assert.IsTrue(secondary.AddTrait(f.Traits[0], 10));
		Assert.AreSame(f.NativeSkill.Object, primary.GetTrait(f.Traits[0]));
		Assert.AreSame(primary.GetTrait(f.Traits[0]), secondary.GetTrait(f.Traits[0]));
		Assert.IsTrue(primary.Languages.Contains(spoken.Object) && secondary.Languages.Contains(spoken.Object));
		Assert.IsTrue(primary.SignedLanguages.Contains(signed.Object) && secondary.SignedLanguages.Contains(signed.Object));
	}

	[TestMethod]
	public void NativeImprovementScope_SuppressesOnlyActingCastCheck_AndRestoresOtherChecks()
	{
		var f = new MagicCastingFixture();
		f.Expressions.Add(new TraitExpression(new MudSharp.Models.TraitExpression { Id = 33, Expression = "variable" }, f.World.Object));
		var template = new MudSharp.Models.CheckTemplate { Name = "ARM02 native check", ImproveTraits = true, CanBranchIfTraitMissing = true };
		foreach (var d in Enum.GetValues<Difficulty>()) template.CheckTemplateDifficulties.Add(new() { Difficulty = (int)d });
		var model = new MudSharp.Models.Check { Type = (int)CheckType.CastSpellCheck, TraitExpressionId = 33, CheckTemplate = template,
			MaximumDifficultyForImprovement = (int)Difficulty.Impossible };
		var check = new NativeCheck(model, f.World.Object);
		using (new CheckImprovementScope(f.Actor.Object))
		{
			check.Resolve(f.Actor.Object, f.Traits[0]); Assert.AreEqual(0, f.SkillUses);
			model.Type = (int)CheckType.TraitBranchCheck;
			new NativeCheck(model, f.World.Object).Resolve(f.Actor.Object, f.Traits[0]); Assert.AreEqual(1, f.SkillUses);
		}
		check.Resolve(f.Actor.Object, f.Traits[0]); Assert.AreEqual(2, f.SkillUses);
	}

	[TestMethod]
	public void ConfiguredGeneration_UsesAuthoredSleepPolicy_AndPreservesIndependentLegacyGenerator()
	{
		var f = new MagicCastingFixture();
		var actor = Holder.Create(f.World.Object, 100); actor.Available.Add(f.Earth);
		Mock.Get(f.Resources[1]).Setup(x => x.ResourceCap(It.IsAny<IHaveMagicResource>())).Returns(100);
		var generator = new LinearTimeBasedGenerator(new MudSharp.Models.MagicGenerator { Id = 20, Name = "sleep allowed",
			Definition = "<Definition><WhichResource>11</WhichResource><AmountPerMinute>2</AmountPerMinute><ConsciousOnly>false</ConsciousOnly></Definition>" }, f.World.Object);
		Set(f.Earth, "_resourceRegenerators", new List<IMagicResourceRegenerator> { generator });
		actor.ReconcileCastingResources(); Set(actor, "_state", CharacterState.Sleeping);
		Mock.Get(f.World.Object.HeartbeatManager).Raise(x => x.FuzzyMinuteHeartbeat += null);
		Assert.AreEqual(2.0, actor.MagicResourceAmounts[f.Resources[1]]);
		Assert.IsTrue(generator.BuildingCommand(f.Actor.Object, new StringStack("conscious")));
		Mock.Get(f.World.Object.HeartbeatManager).Raise(x => x.FuzzyMinuteHeartbeat += null);
		Assert.AreEqual(2.0, actor.MagicResourceAmounts[f.Resources[1]]);
		Set(actor, "_state", CharacterState.Awake);
		var legacy = new Mock<IMagicCapability>(); legacy.SetupGet(x => x.Regenerators).Returns([generator]);
		actor.Available.Clear(); actor.Available.Add(legacy.Object); actor.ReconcileCastingResources();
		Mock.Get(f.World.Object.HeartbeatManager).Raise(x => x.FuzzyMinuteHeartbeat += null);
		Assert.AreEqual(4.0, actor.MagicResourceAmounts[f.Resources[1]]);
		Assert.AreEqual(1, actor.MagicResourceGenerators.Count());
	}

	[TestMethod]
	public void GenerationFilter_DoesNotCaptureOtherIdentities_AndNpcSecondaryOwnsNoDuplicatePool()
	{
		var f = new MagicCastingFixture();
		var primary = Holder.Create(f.World.Object, 100); primary.Player = false;
		var secondary = Holder.Create(f.World.Object, 101); secondary.Player = false; secondary.Canonical = primary;
		Set(primary, "_secondaryInstances", new List<ICharacterInstance> { secondary });
		secondary.Available.Add(f.Earth);
		var other = Holder.Create(f.World.Object, 200);
		foreach (var resource in f.Resources) Mock.Get(resource).Setup(x => x.ResourceCap(It.IsAny<IHaveMagicResource>())).Returns(100);
		var generator = new Mock<IMagicResourceRegenerator>(); generator.SetupGet(x => x.GeneratedResources).Returns([f.Resources[1], f.Resources[0]]);
		generator.Setup(x => x.GetOnMinuteDelegate(It.IsAny<IHaveMagicResource>())).Returns<IHaveMagicResource>(recipient => () =>
		{
			other.AddResource(f.Resources[0], 7); recipient.AddResource(f.Resources[1], 2); recipient.AddResource(f.Resources[0], 9);
		});
		Set(f.Earth, "_resourceRegenerators", new List<IMagicResourceRegenerator> { generator.Object });
		primary.ReconcileCastingResources(); primary.ReconcileCastingResources();
		Mock.Get(f.World.Object.HeartbeatManager).Raise(x => x.FuzzyMinuteHeartbeat += null);
		Assert.AreEqual(7.0, other.MagicResourceAmounts[f.Resources[0]]);
		Assert.AreEqual(2.0, primary.MagicResourceAmounts[f.Resources[1]]);
		Assert.AreEqual(2.0, secondary.MagicResourceAmounts[f.Resources[1]]);
		Assert.IsFalse(secondary.MagicResourceAmounts.ContainsKey(f.Resources[0]));
		Assert.AreEqual(1, primary.MagicResourceGenerators.Count());
	}

	[TestMethod]
	public void NativeReserve_ReconciliationClampsOnlyDown_NoRefillOnCapRiseOrRouteChanges()
	{
		var f = new MagicCastingFixture(); var actor = Holder.Create(f.World.Object, 100); actor.Available.Add(f.Earth);
		actor.ReconcileCastingResources(); actor.AddResource(f.Resources[1], 77);
		Assert.AreEqual(77.0, actor.MagicResourceAmounts[f.Resources[1]]);
		Mock.Get(f.Resources[1]).Setup(x => x.ResourceCap(It.IsAny<IHaveMagicResource>())).Returns(10);
		actor.ReconcileCastingResources(); Assert.AreEqual(10.0, actor.MagicResourceAmounts[f.Resources[1]]);
		Mock.Get(f.Resources[1]).Setup(x => x.ResourceCap(It.IsAny<IHaveMagicResource>())).Returns(200);
		actor.Available.Clear(); actor.ReconcileCastingResources(); actor.Available.Add(f.Earth); actor.ReconcileCastingResources();
		Assert.AreEqual(10.0, actor.MagicResourceAmounts[f.Resources[1]]);
		Assert.IsFalse(actor.CanUseResource(f.Resources[1], 11));
	}

	[DataTestMethod]
	[DataRow(double.NaN)]
	[DataRow(double.PositiveInfinity)]
	[DataRow(-1.0)]
	public void NativeReserve_InvalidCapRejectsCreditAndDebitWithoutCorruptingSpentBalance(double invalid)
	{
		var f = new MagicCastingFixture(); var actor = Holder.Create(f.World.Object, 100); actor.Available.Add(f.Earth);
		actor.ReconcileCastingResources(); actor.AddResource(f.Resources[1], 17);
		Mock.Get(f.Resources[1]).Setup(x => x.ResourceCap(It.IsAny<IHaveMagicResource>())).Returns(invalid);
		actor.AddResource(f.Resources[1], 5);
		Assert.AreEqual(17.0, actor.MagicResourceAmounts[f.Resources[1]]);
		Assert.IsFalse(actor.CanUseResource(f.Resources[1], 5)); Assert.IsFalse(actor.UseResource(f.Resources[1], 5));
		actor.ReconcileCastingResources(); Assert.AreEqual(17.0, actor.MagicResourceAmounts[f.Resources[1]]);
		Mock.Get(f.Resources[1]).Setup(x => x.ResourceCap(It.IsAny<IHaveMagicResource>())).Returns(10);
		actor.ReconcileCastingResources(); Assert.AreEqual(10.0, actor.MagicResourceAmounts[f.Resources[1]]);
		Mock.Get(f.Resources[1]).Setup(x => x.ResourceCap(It.IsAny<IHaveMagicResource>())).Returns(100);
		actor.ReconcileCastingResources(); Assert.AreEqual(10.0, actor.MagicResourceAmounts[f.Resources[1]]);
	}

	[TestMethod]
	public void NativeReserve_EffectiveAttributePenaltyClampsImmediately_ExpiryDoesNotRefill()
	{
		var f = new MagicCastingFixture(); var actor = Holder.Create(f.World.Object, 100); actor.Available.Add(f.Earth);
		Mock.Get(f.Resources[1]).Setup(x => x.ResourceCap(actor)).Returns(() =>
			100 + actor.EffectsOfType<ITraitBonusEffect>().Sum(x => x.GetBonus(f.NativeSkill.Object, TraitBonusContext.None)));
		actor.ReconcileCastingResources(); actor.AddResource(f.Resources[1], 100);
		var penalty = new Mock<ITraitBonusEffect>();
		penalty.Setup(x => x.GetBonus(It.IsAny<ITrait>(), It.IsAny<TraitBonusContext>())).Returns(-50);
		actor.AddEffect(penalty.Object);
		Assert.AreEqual(50.0, actor.MagicResourceAmounts[f.Resources[1]], "Applying a penalty clamps before any accounting action.");
		actor.RemoveEffect(penalty.Object);
		Assert.AreEqual(100.0, f.Resources[1].ResourceCap(actor));
		Assert.AreEqual(50.0, actor.MagicResourceAmounts[f.Resources[1]], "Removing a penalty restores the maximum without restoring spent energy.");
	}

	[TestMethod]
	public void NativeReserve_NegativeBalanceRejectsOrdinaryCreditUntilExplicitRepair()
	{
		var f = new MagicCastingFixture(); var actor = Holder.Create(f.World.Object, 100); actor.Available.Add(f.Earth);
		actor.ReconcileCastingResources();
		((DoubleCounter<IMagicResource>)typeof(ConcreteCharacter).GetField("_magicResourceAmounts", BindingFlags.Instance | BindingFlags.NonPublic)!.GetValue(actor)!)[f.Resources[1]] = -5;
		actor.AddResource(f.Resources[1], 10);
		Assert.AreEqual(-5.0, actor.MagicResourceAmounts[f.Resources[1]]);
		Assert.IsFalse(actor.CanUseResource(f.Resources[1], 0)); Assert.IsFalse(actor.UseResource(f.Resources[1], 0));
		actor.ReconcileCastingResources(); Assert.AreEqual(-5.0, actor.MagicResourceAmounts[f.Resources[1]]);
	}

	private sealed class NativeCheck(MudSharp.Models.Check model, IFuturemud world) : StandardCheck(model, world)
	{
		public CheckOutcome Resolve(ICharacter actor, ITraitDefinition trait) => HandleStandardCheck(actor, actor, Outcome.Pass, Difficulty.Normal, trait);
	}

	[TestMethod]
	public void NativeReserve_AttributeMeritRemovalRestoresMaximumWithoutRefilling()
	{
		var f = new MagicCastingFixture(); var actor = Holder.Create(f.World.Object, 100); actor.Available.Add(f.Earth);
		var merit = new Mock<ITraitBonusMerit>(); merit.SetupGet(x => x.MeritScope).Returns(MeritScope.Character);
		Mock.Get(f.Resources[1]).Setup(x => x.ResourceCap(actor)).Returns(() => actor.Merits.Contains(merit.Object) ? 50 : 100);
		actor.ReconcileCastingResources(); actor.AddResource(f.Resources[1], 100);
		Assert.IsTrue(actor.AddMerit(merit.Object)); Assert.AreEqual(50.0, actor.MagicResourceAmounts[f.Resources[1]]);
		Assert.IsTrue(actor.RemoveMerit(merit.Object)); Assert.AreEqual(50.0, actor.MagicResourceAmounts[f.Resources[1]]);
	}

	[DataTestMethod]
	[DataRow(70.0, 150.0)]
	[DataRow(-10.0, 120.0)]
	public void NativeReserve_LoadRestorationDefersPartialInventoryAndEffectCaps_ThenClampsOnce(double secondBonus, double finalBalance)
	{
		var f = new MagicCastingFixture(); var actor = Holder.Create(f.World.Object, 100); actor.Available.Add(f.Earth);
		Mock.Get(f.Resources[1]).Setup(x => x.ResourceCap(actor)).Returns(() =>
			100 + actor.EffectsOfType<ITraitBonusEffect>().Sum(x => x.GetBonus(f.NativeSkill.Object, TraitBonusContext.None)));
		actor.ReconcileCastingResources();
		var balances = (DoubleCounter<IMagicResource>)typeof(ConcreteCharacter).GetField("_magicResourceAmounts", BindingFlags.Instance | BindingFlags.NonPublic)!.GetValue(actor)!;
		balances[f.Resources[1]] = 150; // Restored balance; contributors follow inventory in production load order.
		using (actor.DeferCastingCapacityReconciliation())
		{
			actor.ReconcileCastingResources(); Assert.AreEqual(150.0, actor.MagicResourceAmounts[f.Resources[1]]);
			foreach (var bonus in new[] { 30.0, secondBonus })
			{
				var effect = new Mock<ITraitBonusEffect>(); effect.Setup(x => x.GetBonus(It.IsAny<ITrait>(), It.IsAny<TraitBonusContext>())).Returns(bonus);
				actor.AddEffect(effect.Object); Assert.AreEqual(150.0, actor.MagicResourceAmounts[f.Resources[1]]);
			}
		}
		Assert.AreEqual(finalBalance, actor.MagicResourceAmounts[f.Resources[1]]);
	}

	[DataTestMethod]
	[DataRow(false)]
	[DataRow(true)]
	public void NativeReserve_LabourHoursChangeClampsAtActivationThreshold(bool projectHours)
	{
		var f = new MagicCastingFixture(); var actor = Holder.Create(f.World.Object, 100); actor.Available.Add(f.Earth);
		Mock.Get(f.Resources[1]).Setup(x => x.ResourceCap(actor)).Returns(() =>
			(projectHours ? actor.CurrentProjectProjectHours : actor.CurrentProjectHours) >= 1 ? 50 : 100);
		actor.ReconcileCastingResources(); actor.AddResource(f.Resources[1], 100);
		if (projectHours) actor.CurrentProjectProjectHours = 1; else actor.CurrentProjectHours = 1;
		Assert.AreEqual(50.0, actor.MagicResourceAmounts[f.Resources[1]]);
		if (projectHours) actor.CurrentProjectProjectHours = 0; else actor.CurrentProjectHours = 0;
		Assert.AreEqual(100.0, f.Resources[1].ResourceCap(actor)); Assert.AreEqual(50.0, actor.MagicResourceAmounts[f.Resources[1]]);
	}

	[DataTestMethod]
	[DataRow(true, false, 5.0, 90.0, 90.0)]
	[DataRow(false, false, 5.0, 90.0, 90.0)]
	[DataRow(true, true, 5.0, 90.0, 90.0)]
	[DataRow(false, true, 5.0, 90.0, 90.0)]
	[DataRow(true, false, 8.0, 140.0, 100.0)]
	[DataRow(false, false, 8.0, 140.0, 100.0)]
	public void NativeReserve_CompoundSpellRemoval_ReconcilesCompletedStateOnce(bool positiveFirst, bool expire,
		double positive, double balance, double expectedBalance)
	{
		var (f, actor) = CompoundCapacityFixture();
		var parent = new MagicSpellParent(actor, f.Spell, actor);
		var bonuses = positiveFirst ? new[] { positive, positive == 5 ? -5.0 : -3.0 } : new[] { positive == 5 ? -5.0 : -3.0, positive };
		using (actor.DeferCastingCapacityReconciliation())
		{
			foreach (var bonus in bonuses)
			{
				var child = new SpellTraitBoostEffect(actor, parent, null!) { Trait = f.Traits[0], Bonus = bonus };
				parent.AddSpellEffect(child); actor.AddEffect(child);
			}
			actor.AddEffect(parent);
		}
		actor.AddResource(f.Resources[1], balance);
		Mock.Get(f.Resources[1]).Invocations.Clear();
		if (expire) parent.ExpireEffect(); else actor.RemoveEffect(parent, true);
		Assert.AreEqual(expectedBalance, actor.MagicResourceAmounts[f.Resources[1]]);
		Mock.Get(f.Resources[1]).Verify(x => x.ResourceCap(actor), Times.Once);
		Assert.AreEqual(100.0, f.Resources[1].ResourceCap(actor));
		Assert.IsFalse(actor.Effects.Contains(parent)); Assert.IsFalse(parent.SpellEffects.Any());
		actor.ReconcileCastingResources();
		Assert.AreEqual(expectedBalance, actor.MagicResourceAmounts[f.Resources[1]], "Reconciliation cannot refill after removal.");
	}

	[DataTestMethod]
	[DataRow("cast", true, -5.0, 90.0)]
	[DataRow("cast", false, -5.0, 90.0)]
	[DataRow("cast", true, -8.0, 70.0)]
	[DataRow("cast", false, -8.0, 70.0)]
	[DataRow("target", true, -5.0, 90.0)]
	[DataRow("target", false, -5.0, 90.0)]
	[DataRow("caster", true, -5.0, 90.0)]
	[DataRow("caster", false, -5.0, 90.0)]
	[DataRow("combined", true, -5.0, 90.0)]
	[DataRow("combined", false, -5.0, 90.0)]
	[DataRow("combined", true, -8.0, 70.0)]
	[DataRow("combined", false, -8.0, 70.0)]
	public void NativeReserve_CompoundSpellApplication_ReconcilesCompletedStateOnce(string mode, bool positiveFirst,
		double negative, double expectedBalance)
	{
		var (f, actor) = CompoundCapacityFixture(); actor.AddResource(f.Resources[1], 90);
		var bonuses = positiveFirst ? new[] { 5.0, negative } : new[] { negative, 5.0 };
		var spell = f.NewSpell(10, "Compound test", string.Concat(bonuses.Select(b => $"<Effect type='boost' trait='1' bonus='{b}' context='0' />")));
		Set(spell, "<EffectDurationExpression>k__BackingField", new TraitExpression("600", f.World.Object));
		if (mode == "caster")
		{
			((List<IMagicSpellEffectTemplate>)spell.CasterSpellEffects).AddRange(spell.SpellEffects);
			((List<IMagicSpellEffectTemplate>)spell.SpellEffects).Clear();
		}
		if (mode == "combined")
		{
			var effects = (List<IMagicSpellEffectTemplate>)spell.SpellEffects;
			((List<IMagicSpellEffectTemplate>)spell.CasterSpellEffects).Add(effects[1]); effects.RemoveAt(1);
		}
		Mock.Get(f.Resources[1]).Invocations.Clear();
		if (mode == "cast")
		{
			f.ActiveCapabilities.Add(Mock.Of<IMagicCapability>(x => x.School == f.School));
			spell.CastSpell(f.Actor.Object, actor, SpellPower.Standard);
		}
		else spell.ResolveTriggeredSpell(actor, mode == "caster" ? null! : actor, SpellPower.Standard);
		Assert.AreEqual(2, actor.EffectsOfType<SpellTraitBoostEffect>().Count(), string.Join("; ", f.Messages));
		Assert.AreEqual(expectedBalance, actor.MagicResourceAmounts[f.Resources[1]]);
		Mock.Get(f.Resources[1]).Verify(x => x.ResourceCap(actor), Times.Once);
		Assert.AreEqual((10 + 5 + negative) * 10, f.Resources[1].ResourceCap(actor));
		actor.RemoveAllEffects<MagicSpellParent>(fireRemovalAction: true);
		Assert.AreEqual(expectedBalance, actor.MagicResourceAmounts[f.Resources[1]], "A capacity rise cannot restore lost energy.");
	}

	[DataTestMethod]
	[DataRow(false)]
	[DataRow(true)]
	public void NativeReserve_CompoundSpellApplication_ExceptionReleasesNestedScopeAndClampsRemainingState(bool nested)
	{
		var (f, actor) = CompoundCapacityFixture(); actor.AddResource(f.Resources[1], 90);
		var spell = f.NewSpell(10, "Interrupted compound", "<Effect type='boost' trait='1' bonus='-5' context='0' />");
		Set(spell, "<EffectDurationExpression>k__BackingField", new TraitExpression("600", f.World.Object));
		var failure = new Mock<IMagicSpellEffectTemplate>();
		failure.Setup(x => x.GetOrApplyEffect(It.IsAny<ICharacter>(), It.IsAny<IPerceivable>(), It.IsAny<OpposedOutcomeDegree>(),
			It.IsAny<SpellPower>(), It.IsAny<IMagicSpellEffectParent>(), It.IsAny<SpellAdditionalParameter[]>())).Throws(new InvalidOperationException("test application interruption"));
		((List<IMagicSpellEffectTemplate>)spell.SpellEffects).Add(failure.Object);
		using (nested ? actor.DeferCastingCapacityReconciliation() : null)
		{
			Assert.ThrowsException<InvalidOperationException>(() => spell.ResolveTriggeredSpell(actor, actor, SpellPower.Standard));
			Assert.AreEqual(nested ? 90.0 : 50.0, actor.MagicResourceAmounts[f.Resources[1]]);
			Assert.AreEqual(nested, actor.CastingCapacityRestorationActive);
		}
		Assert.IsFalse(actor.CastingCapacityRestorationActive);
		Assert.AreEqual(50.0, actor.MagicResourceAmounts[f.Resources[1]]);
		actor.RemoveAllEffects<SpellTraitBoostEffect>(fireRemovalAction: true);
		Assert.AreEqual(100.0, f.Resources[1].ResourceCap(actor));
		Assert.AreEqual(50.0, actor.MagicResourceAmounts[f.Resources[1]], "Exception recovery does not refill energy.");
	}

	[TestMethod]
	public void NativeReserve_CompoundSpellRemoval_ExceptionResetsParentGuardAndAllowsRetry()
	{
		var (f, actor) = CompoundCapacityFixture();
		var parent = new MagicSpellParent(actor, f.Spell, actor);
		var failure = new Mock<IMagicSpellEffect>(); var firstAttempt = true;
		failure.Setup(x => x.RemovalEffect()).Callback(() =>
		{
			if (firstAttempt) { firstAttempt = false; throw new InvalidOperationException("test removal interruption"); }
			parent.RemoveSpellEffect(failure.Object);
		});
		using (actor.DeferCastingCapacityReconciliation())
		{
			var positive = new SpellTraitBoostEffect(actor, parent, null!) { Trait = f.Traits[0], Bonus = 5 };
			var negative = new SpellTraitBoostEffect(actor, parent, null!) { Trait = f.Traits[0], Bonus = -5 };
			foreach (var child in new IMagicSpellEffect[] { positive, failure.Object, negative }) { parent.AddSpellEffect(child); actor.AddEffect(child); }
			actor.AddEffect(parent);
		}
		actor.AddResource(f.Resources[1], 90);
		Assert.ThrowsException<InvalidOperationException>(() => actor.RemoveEffect(parent, true));
		Assert.IsFalse((bool)typeof(MagicSpellParent).GetField("_removingSpellEffects", BindingFlags.Instance | BindingFlags.NonPublic)!.GetValue(parent)!);
		Assert.IsFalse(actor.CastingCapacityRestorationActive);
		Assert.AreEqual(50.0, actor.MagicResourceAmounts[f.Resources[1]], "The interrupted operation retains a genuine lower remaining maximum.");
		actor.RemoveEffect(parent, true);
		Assert.IsFalse(parent.SpellEffects.Any()); Assert.AreEqual(100.0, f.Resources[1].ResourceCap(actor));
		Assert.AreEqual(50.0, actor.MagicResourceAmounts[f.Resources[1]]);
	}

	[DataTestMethod]
	[DataRow(false)]
	[DataRow(true)]
	public void NativeReserve_CompoundSpellApplication_NestedCompletionWaitsForOutermostScope(bool restoration)
	{
		var (f, actor) = CompoundCapacityFixture(); actor.AddResource(f.Resources[1], 90);
		var spell = f.NewSpell(10, "Nested compound", "<Effect type='boost' trait='1' bonus='-8' context='0' /><Effect type='boost' trait='1' bonus='5' context='0' />");
		Set(spell, "<EffectDurationExpression>k__BackingField", new TraitExpression("600", f.World.Object));
		var outer = restoration ? actor.DeferCastingCapacityReconciliation() : actor.DeferCastingCapacityReconciliationForMutation();
		using (outer)
		{
			spell.ResolveTriggeredSpell(actor, actor, SpellPower.Standard);
			Assert.AreEqual(restoration, actor.CastingCapacityRestorationActive);
			Assert.AreEqual(90.0, actor.MagicResourceAmounts[f.Resources[1]]);
			Mock.Get(f.Resources[1]).Invocations.Clear();
		}
		outer.Dispose();
		Assert.IsFalse(actor.CastingCapacityRestorationActive);
		Assert.AreEqual(70.0, actor.MagicResourceAmounts[f.Resources[1]]);
		Mock.Get(f.Resources[1]).Verify(x => x.ResourceCap(actor), Times.Once, "Repeated disposal must not release or reconcile twice.");
		actor.RemoveAllEffects<MagicSpellParent>(fireRemovalAction: true);
		Assert.AreEqual(70.0, actor.MagicResourceAmounts[f.Resources[1]]);
	}

	[TestMethod]
	public void NativeReserve_CompoundSpellApplication_MutationBatchKeepsOrdinaryResourceEffectsAvailable()
	{
		var (f, actor) = CompoundCapacityFixture(); actor.AddResource(f.Resources[1], 17);
		var spell = f.NewSpell(10, "Resource delta", "");
		Set(spell, "<EffectDurationExpression>k__BackingField", new TraitExpression("600", f.World.Object));
		var delta = new Mock<IMagicSpellEffectTemplate>();
		delta.Setup(x => x.GetOrApplyEffect(It.IsAny<ICharacter>(), actor, It.IsAny<OpposedOutcomeDegree>(),
			It.IsAny<SpellPower>(), It.IsAny<IMagicSpellEffectParent>(), It.IsAny<SpellAdditionalParameter[]>()))
			.Callback(() =>
			{
				Assert.IsFalse(actor.CastingCapacityRestorationActive, "A complete live actor is not an incomplete reconstruction.");
				actor.AddResource(f.Resources[1], 7);
			}).Returns((IMagicSpellEffect)null!);
		((List<IMagicSpellEffectTemplate>)spell.SpellEffects).Add(delta.Object);
		spell.ResolveTriggeredSpell(actor, actor, SpellPower.Standard);
		Assert.AreEqual(24.0, actor.MagicResourceAmounts[f.Resources[1]]);
	}

	[DataTestMethod]
	[DataRow("generic", true)]
	[DataRow("generic", false)]
	[DataRow("predicate", true)]
	[DataRow("predicate", false)]
	[DataRow("all", true)]
	[DataRow("all", false)]
	public void NativeReserve_CompoundSpellRemoval_AggregateRemovalBatchesAllSelectedParents(string mode, bool positiveFirst)
	{
		var (f, actor) = CompoundCapacityFixture();
		using (actor.DeferCastingCapacityReconciliation())
		{
			foreach (var bonus in positiveFirst ? new[] { 5.0, -5.0 } : new[] { -5.0, 5.0 })
			{
				var parent = new MagicSpellParent(actor, f.Spell, actor);
				var child = new SpellTraitBoostEffect(actor, parent, null!) { Trait = f.Traits[0], Bonus = bonus };
				parent.AddSpellEffect(child); actor.AddEffect(child); actor.AddEffect(parent);
			}
		}
		actor.AddResource(f.Resources[1], 90); Mock.Get(f.Resources[1]).Invocations.Clear();
		if (mode == "generic") actor.RemoveAllEffects<MagicSpellParent>(fireRemovalAction: true);
		else if (mode == "predicate") actor.RemoveAllEffects(x => x is MagicSpellParent, true);
		else actor.RemoveAllEffects();
		Assert.AreEqual(90.0, actor.MagicResourceAmounts[f.Resources[1]]);
		Mock.Get(f.Resources[1]).Verify(x => x.ResourceCap(actor), Times.Once);
		Assert.IsFalse(actor.Effects.Any());
	}

	[DataTestMethod]
	[DataRow("cast", true, 5.0, -5.0, 7.0, 97.0)]
	[DataRow("cast", false, 5.0, -5.0, 7.0, 97.0)]
	[DataRow("target", true, 5.0, -5.0, 7.0, 97.0)]
	[DataRow("target", false, 5.0, -5.0, 7.0, 97.0)]
	[DataRow("combined", true, 5.0, -5.0, 7.0, 97.0)]
	[DataRow("combined", false, 5.0, -5.0, 7.0, 97.0)]
	[DataRow("cast", true, 5.0, -8.0, 7.0, 70.0)]
	[DataRow("cast", false, 5.0, -8.0, 7.0, 70.0)]
	[DataRow("target", true, 5.0, -8.0, 7.0, 70.0)]
	[DataRow("target", false, 5.0, -8.0, 7.0, 70.0)]
	[DataRow("combined", true, 5.0, -8.0, 7.0, 70.0)]
	[DataRow("combined", false, 5.0, -8.0, 7.0, 70.0)]
	[DataRow("cast", true, 10.0, -5.0, 50.0, 140.0)]
	[DataRow("cast", false, 10.0, -5.0, 50.0, 140.0)]
	[DataRow("target", true, 10.0, -5.0, 50.0, 140.0)]
	[DataRow("target", false, 10.0, -5.0, 50.0, 140.0)]
	[DataRow("combined", true, 10.0, -5.0, 50.0, 140.0)]
	[DataRow("combined", false, 10.0, -5.0, 50.0, 140.0)]
	[DataRow("cast", true, 5.0, -5.0, -7.0, 83.0)]
	[DataRow("cast", false, 5.0, -5.0, -7.0, 83.0)]
	[DataRow("target", true, 5.0, -5.0, -7.0, 83.0)]
	[DataRow("target", false, 5.0, -5.0, -7.0, 83.0)]
	[DataRow("combined", true, 5.0, -5.0, -7.0, 83.0)]
	[DataRow("combined", false, 5.0, -5.0, -7.0, 83.0)]
	public void NativeReserve_CompoundSpellMixedDelta_ClampsLegitimateArithmeticAgainstCompletedMaximum(string mode,
		bool positiveFirst, double positive, double negative, double amount, double expectedBalance)
	{
		var (f, actor) = CompoundCapacityFixture(); actor.AddResource(f.Resources[1], 90);
		var first = positiveFirst ? positive : negative; var second = positiveFirst ? negative : positive;
		var spell = f.NewSpell(10, "Mixed compound", $"<Effect type='boost' trait='1' bonus='{first}' context='0' /><Effect type='boost' trait='1' bonus='{second}' context='0' />");
		Set(spell, "<EffectDurationExpression>k__BackingField", new TraitExpression("600", f.World.Object));
		var effects = (List<IMagicSpellEffectTemplate>)spell.SpellEffects;
		var delta = new Mock<IMagicSpellEffectTemplate>();
		delta.Setup(x => x.GetOrApplyEffect(It.IsAny<ICharacter>(), actor, It.IsAny<OpposedOutcomeDegree>(),
			It.IsAny<SpellPower>(), It.IsAny<IMagicSpellEffectParent>(), It.IsAny<SpellAdditionalParameter[]>()))
			.Callback(() => actor.AddResource(f.Resources[1], amount)).Returns((IMagicSpellEffect)null!);
		effects.Insert(1, delta.Object);
		if (mode == "combined") { ((List<IMagicSpellEffectTemplate>)spell.CasterSpellEffects).Add(effects[2]); effects.RemoveAt(2); }
		if (mode == "cast")
		{
			f.ActiveCapabilities.Add(Mock.Of<IMagicCapability>(x => x.School == f.School));
			spell.CastSpell(f.Actor.Object, actor, SpellPower.Standard);
		}
		else spell.ResolveTriggeredSpell(actor, actor, SpellPower.Standard);
		Assert.AreEqual(2, actor.EffectsOfType<SpellTraitBoostEffect>().Count());
		Assert.AreEqual((10 + positive + negative) * 10, f.Resources[1].ResourceCap(actor));
		Assert.AreEqual(expectedBalance, actor.MagicResourceAmounts[f.Resources[1]]);
		actor.RemoveAllEffects<MagicSpellParent>(fireRemovalAction: true);
		Assert.AreEqual(Math.Min(expectedBalance, 100), actor.MagicResourceAmounts[f.Resources[1]], "Cleanup clamps a genuine decrease without refilling.");
	}

	[DataTestMethod]
	[DataRow(true, 7.0)]
	[DataRow(false, 7.0)]
	[DataRow(true, 60.0)]
	[DataRow(false, 60.0)]
	public void NativeReserve_CompoundSpellMixedDebit_UsesActualFundsWithoutTransientClipping(bool positiveFirst, double amount)
	{
		var (f, actor) = CompoundCapacityFixture(); actor.AddResource(f.Resources[1], 90);
		var spell = f.NewSpell(10, "Debit compound", positiveFirst
			? "<Effect type='boost' trait='1' bonus='5' context='0' /><Effect type='boost' trait='1' bonus='-5' context='0' />"
			: "<Effect type='boost' trait='1' bonus='-5' context='0' /><Effect type='boost' trait='1' bonus='5' context='0' />");
		Set(spell, "<EffectDurationExpression>k__BackingField", new TraitExpression("600", f.World.Object));
		var debit = new Mock<IMagicSpellEffectTemplate>();
		debit.Setup(x => x.GetOrApplyEffect(It.IsAny<ICharacter>(), actor, It.IsAny<OpposedOutcomeDegree>(),
			It.IsAny<SpellPower>(), It.IsAny<IMagicSpellEffectParent>(), It.IsAny<SpellAdditionalParameter[]>()))
			.Callback(() => { Assert.IsTrue(actor.CanUseResource(f.Resources[1], amount)); Assert.IsTrue(actor.UseResource(f.Resources[1], amount)); })
			.Returns((IMagicSpellEffect)null!);
		((List<IMagicSpellEffectTemplate>)spell.SpellEffects).Insert(1, debit.Object);
		spell.ResolveTriggeredSpell(actor, actor, SpellPower.Standard);
		Assert.AreEqual(90 - amount, actor.MagicResourceAmounts[f.Resources[1]]);
		actor.RemoveAllEffects<MagicSpellParent>(fireRemovalAction: true);
		Assert.AreEqual(90 - amount, actor.MagicResourceAmounts[f.Resources[1]]);
	}

	[DataTestMethod]
	[DataRow(double.NaN)]
	[DataRow(double.PositiveInfinity)]
	[DataRow(-1.0)]
	public void NativeReserve_CompoundSpellMixedAccounting_InvalidCapacityStillRefusesCreditAndDebit(double invalid)
	{
		var (f, actor) = CompoundCapacityFixture(); actor.AddResource(f.Resources[1], 17);
		Mock.Get(f.Resources[1]).Setup(x => x.ResourceCap(actor)).Returns(invalid);
		using (actor.DeferCastingCapacityReconciliationForMutation())
		{
			actor.AddResource(f.Resources[1], 7);
			Assert.IsFalse(actor.CanUseResource(f.Resources[1], 0)); Assert.IsFalse(actor.UseResource(f.Resources[1], 0));
			Assert.AreEqual(17.0, actor.MagicResourceAmounts[f.Resources[1]]);
		}
		Assert.AreEqual(17.0, actor.MagicResourceAmounts[f.Resources[1]]);
	}

	private static (MagicCastingFixture Fixture, Holder Actor) CompoundCapacityFixture()
	{
		var f = new MagicCastingFixture(); var actor = Holder.Create(f.World.Object, 100); actor.Available.Add(f.Earth);
		Set(actor, "<OutputHandler>k__BackingField", f.Actor.Object.OutputHandler);
		f.NativeSkill.SetupGet(x => x.Definition).Returns(f.Traits[0]);
		Mock.Get(f.Resources[1]).Setup(x => x.ResourceCap(actor)).Returns(() =>
			10 * (10 + actor.EffectsOfType<ITraitBonusEffect>().Sum(x => x.GetBonus(f.NativeSkill.Object, TraitBonusContext.None))));
		actor.ReconcileCastingResources();
		return (f, actor);
	}

	private sealed class Holder : ConcreteCharacter
	{
		private Holder() : base(null!, null!, true) { }
		public List<IMagicCapability> Available = null!;
		public ICharacterIdentity? Canonical;
		public bool Player = true;
		public override ICharacterIdentity Identity => Canonical ?? this;
		public override bool IsPlayerCharacter => Player;
		public static Holder Create(IFuturemud world, long id)
		{
			var actor = (Holder)RuntimeHelpers.GetUninitializedObject(typeof(Holder)); actor.Available = []; actor.Player = true;
			Set(actor, "<Gameworld>k__BackingField", world); Set(actor, "_id", id); Set(actor, "_noSave", true);
			Set(actor, "_state", CharacterState.Awake); Set(actor, "_positionState", MudSharp.Body.Position.PositionStates.PositionStanding.Instance);
			Set(actor, "<Body>k__BackingField", new Mock<IBody> { DefaultValue = DefaultValue.Mock }.Object);
			Set(actor, "<EffectHandler>k__BackingField", new EffectHandler(actor));
			var merit = new Mock<IMagicCapabilityMerit>(); merit.Setup(x => x.Applies(actor)).Returns(true);
			merit.SetupGet(x => x.Capabilities).Returns(() => actor.Available);
			Set(actor, "_merits", new List<IMerit> { merit.Object });
			Set(actor, "_secondaryInstances", new List<ICharacterInstance>());
			Set(actor, "_characterTraits", new List<ITrait>());
			Set(actor, "_languages", new List<ILanguage>()); Set(actor, "_signedLanguages", new List<ISignedLanguage>());
			Set(actor, "_acquisitionAccents", new Dictionary<ILanguage, IAccent>());
			Set(actor, "_preferredAccents", new Dictionary<ILanguage, IAccent>());
			Set(actor, "_accents", new Dictionary<IAccent, Difficulty>());
			Set(actor, "_magicResourceAmounts", new DoubleCounter<IMagicResource>());
			Set(actor, "_magicResourceGenerators", new List<IMagicResourceRegenerator>());
			Set(actor, "_generatorDelegateDictionary", new Dictionary<IMagicResourceRegenerator, HeartbeatManagerDelegate>());
			Set(actor, "_castingGenerators", new Dictionary<IMagicResourceRegenerator, HeartbeatManagerDelegate>());
			return actor;
		}
	}

	private static void Set(object target, string field, object value)
	{
		for (var type = target.GetType(); type is not null; type = type.BaseType)
			if (type.GetField(field, BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public) is { } member) { member.SetValue(target, value); return; }
		throw new MissingFieldException(field);
	}
}
