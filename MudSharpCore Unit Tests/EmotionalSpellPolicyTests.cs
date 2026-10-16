#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Body.Traits;
using MudSharp.Character;
using MudSharp.Combat;
using MudSharp.Effects;
using MudSharp.Effects.Concrete;
using MudSharp.Effects.Concrete.SpellEffects;
using MudSharp.Effects.Interfaces;
using MudSharp.Framework;
using MudSharp.Magic;
using MudSharp.Magic.Emotions;

namespace MudSharp_Unit_Tests;

[TestClass]
public class EmotionalSpellPolicyTests
{
	private static readonly EmotionalTerrainRule Ordinary = new(3, 1, 0);
	private static EmotionalRetainedState State(int grade, SpellPower power = SpellPower.Standard,
		double intensity = 6, double endurance = 1) => new(grade, power, intensity, endurance);

	[DataTestMethod]
	[DataRow(1, 7, 0, 6, false)]
	[DataRow(7, 7, 0, 0, true)]
	[DataRow(7, 1, 6, 0, true)]
	public void Counter_StrongerEqualWeaker_UsesOnlySourceGrades(int incoming, int opposing, int remaining,
		int retained, bool remove)
	{
		var result = EmotionalSpellPolicy.Counter(incoming, opposing);
		Assert.AreEqual(remaining, result.RemainingIncomingGrade);
		Assert.AreEqual(retained, result.RemainingOpposingGrade);
		Assert.AreEqual(remove, result.RemoveOpposing);
		Assert.AreEqual(remaining > 0, result.Continue);
	}

	[TestMethod]
	public void Counter_NoOpposition_LeavesIncomingGrade()
	{
		var result = EmotionalSpellPolicy.Counter(7, null);
		Assert.AreEqual(7, result.RemainingIncomingGrade); Assert.IsFalse(result.ChangesOpposing);
	}

	[DataTestMethod]
	[DataRow(1, 1, 1)]
	[DataRow(2, 2, 2)]
	[DataRow(3, 3, 4)]
	[DataRow(7, 7, 24)]
	public void EnduranceRoll_SourceHelperKeepsInvertedLowerBoundAndInclusiveEndpoints(int grade, int lower, int upper)
	{
		var calls = 0;
		var low = EmotionalSpellPolicy.RollEndurance(grade, Ordinary, (from, to) => { calls++; Assert.AreEqual(lower, from); Assert.AreEqual(upper, to); return from; });
		var high = EmotionalSpellPolicy.RollEndurance(grade, Ordinary, (_, to) => to);
		Assert.AreEqual(lower, low); Assert.AreEqual(upper, high);
		Assert.AreEqual(grade == 1 ? 0 : 1, calls);
	}

	[TestMethod]
	public void EnduranceRoll_EarthPlane_AddsResidualGradeAfterRandomDraw()
	{
		Assert.AreEqual(31, EmotionalSpellPolicy.RollEndurance(7, new(4, 1, 1), (_, to) => to));
		Assert.ThrowsException<InvalidOperationException>(() => EmotionalSpellPolicy.RollEndurance(7, Ordinary, (_, _) => 25));
	}

	[DataTestMethod]
	[DataRow(2, 1, 7, 14)]
	[DataRow(5, 2, 7, 17)]
	[DataRow(7, 2, 7, 24)]
	[DataRow(33, 10, 7, 23)]
	[DataRow(4, 1, 7, 28)]
	[DataRow(3, 1, 7, 21)]
	[DataRow(5, 2, 1, 2)]
	public void TerrainDuration_ExplicitRationalMapping_TruncatesSourceUnits(int numerator, int denominator, int grade, int expected)
	{
		Assert.AreEqual(expected, new EmotionalTerrainRule(numerator, denominator, 0).DurationUnits(grade));
	}

	[TestMethod]
	public void FuryRecast_PreservesOldSourcePowerIntensityAndEnduranceWhileAccumulating()
	{
		var previous = State(1, SpellPower.ExtremelyWeak, 5, 1);
		var result = EmotionalSpellPolicy.ResolveLifetime(EmotionalSpellKind.Fury, 7, Ordinary, 600, 36,
			State(7, SpellPower.ExtremelyStrong, 12, 24), TimeSpan.FromSeconds(601), previous);
		Assert.AreSame(previous, result.State); Assert.AreEqual(TimeSpan.FromSeconds(23 * 600), result.Duration);
	}

	[TestMethod]
	public void CalmRecast_RetainsStrongestSourceGradeIndependentlyOfNativePower()
	{
		var previous = State(7, SpellPower.ExtremelyWeak, 11, 0);
		var result = EmotionalSpellPolicy.ResolveLifetime(EmotionalSpellKind.Calm, 1, Ordinary, 600, 24,
			State(1, SpellPower.ExtremelyStrong, 11, 0), TimeSpan.FromSeconds(601), previous);
		Assert.AreSame(previous, result.State); Assert.AreEqual(TimeSpan.FromSeconds(4 * 600), result.Duration);
	}

	[DataTestMethod]
	[DataRow(EmotionalSpellKind.Fury, 36)]
	[DataRow(EmotionalSpellKind.Calm, 24)]
	public void LifetimeRecast_ClampsDistinctSourceCaps(EmotionalSpellKind kind, int cap)
	{
		var state = State(7);
		var result = EmotionalSpellPolicy.ResolveLifetime(kind, 7, Ordinary, 600, cap, state,
			TimeSpan.FromSeconds(cap * 600), state);
		Assert.AreEqual(TimeSpan.FromSeconds(cap * 600), result.Duration);
	}

	[DataTestMethod]
	[DataRow(1, 3)]
	[DataRow(599, 3)]
	[DataRow(600, 3)]
	[DataRow(601, 4)]
	public void CalmRemaining_NormalisedInclusiveSourceBoundary_UsesWholeUnits(int remaining, int units)
	{
		var state = State(1);
		Assert.AreEqual(TimeSpan.FromSeconds(units * 600), EmotionalSpellPolicy.ResolveLifetime(
			EmotionalSpellKind.Calm, 1, Ordinary, 600, 24, state, TimeSpan.FromSeconds(remaining), state).Duration);
	}

	[TestMethod]
	public void Lifetime_InvalidOrConflictingMetadata_RefusesInsteadOfGuessing()
	{
		Assert.ThrowsException<ArgumentException>(() => EmotionalSpellPolicy.ResolveLifetime(
			EmotionalSpellKind.Calm, 1, Ordinary, 600, 24, State(1), TimeSpan.Zero, State(1)));
		Assert.ThrowsException<ArgumentException>(() => EmotionalSpellPolicy.ResolveLifetime(
			EmotionalSpellKind.Calm, 1, Ordinary, 600, 24, State(1), TimeSpan.FromSeconds(600), State(1, SpellPower.Weak)));
		Assert.ThrowsException<ArgumentOutOfRangeException>(() => EmotionalSpellPolicy.Counter(0, null));
		Assert.ThrowsException<ArgumentException>(() => new EmotionalTerrainRule(1, 0, 0).DurationUnits(1));
	}

	[TestMethod]
	public void FuryChild_WeakeningAndXmlReload_RetainsOriginalNativeEnduranceAndPower()
	{
		var f = new MagicCastingFixture();
		var trait = new Mock<ITraitDefinition>(); trait.SetupGet(x => x.Id).Returns(844);
		trait.SetupGet(x => x.TraitType).Returns(TraitType.Attribute);
		f.World.Setup(x => x.Traits).Returns(new All<ITraitDefinition> { trait.Object });
		SpellSourceFuryEffect.InitialiseEffectType();
		var parent = new MagicSpellParent(f.Actor.Object, f.Spell, f.Actor.Object, SpellPower.ExtremelyWeak);
		var child = new SpellSourceFuryEffect(f.Actor.Object, parent, "test.emotion", 600, 36,
			State(7, SpellPower.ExtremelyWeak, 6, 24), trait.Object, 0.5);
		child.ReduceSourceGrade(6);
		var loaded = (SpellSourceFuryEffect)Effect.LoadEffect(child.SaveToXml(new()), f.Actor.Object);
		Assert.IsNull(loaded.DefinitionError); Assert.AreEqual(1, loaded.State.SourceGrade);
		Assert.AreEqual(24, loaded.State.EndurancePoints); Assert.AreEqual(SpellPower.ExtremelyWeak, loaded.State.Power);
		var heldTrait = new Mock<ITrait>(); heldTrait.SetupGet(x => x.Definition).Returns(trait.Object);
		Assert.AreEqual(12, loaded.GetBonus(heldTrait.Object)); Assert.IsTrue(loaded.IsRaging); Assert.IsFalse(loaded.IsSuperRaging);
		Assert.AreEqual(0, loaded.GetBonus(new Mock<ITrait>().Object));
	}

	[TestMethod]
	public void CalmChild_PersistedMalformedGrade_IsPreservedAndCannotBeWeakened()
	{
		var f = new MagicCastingFixture(); SpellSourceCalmEffect.InitialiseEffectType();
		var parent = new MagicSpellParent(f.Actor.Object, f.Spell, f.Actor.Object);
		var child = new SpellSourceCalmEffect(f.Actor.Object, parent, "test.emotion", 600, 24, State(7, endurance: 0));
		var xml = child.SaveToXml(new()); xml.Element("Effect")!.SetAttributeValue("grade", "corrupt");
		var expected = xml.Element("Effect")!.ToString();
		var loaded = (SpellSourceCalmEffect)Effect.LoadEffect(xml, f.Actor.Object);
		Assert.IsNotNull(loaded.DefinitionError); Assert.AreEqual(expected, loaded.SaveToXml(new()).Element("Effect")!.ToString());
		Assert.ThrowsException<ArgumentException>(() => loaded.ReduceSourceGrade(1));
	}

	[TestMethod]
	public void CanonicalQuery_PreservesApplicabilityAndNativeThresholdsAcrossOwners()
	{
		var actor = new Mock<ICharacter>();
		var inactive = new Mock<IPacifismEffect>(); inactive.SetupGet(x => x.IsSuperPeaceful).Returns(true);
		inactive.Setup(x => x.Applies(actor.Object)).Returns(false);
		var peaceful = new Mock<IPacifismEffect>(); peaceful.SetupGet(x => x.IsPeaceful).Returns(true);
		peaceful.Setup(x => x.Applies(actor.Object)).Returns(true);
		actor.Setup(x => x.CombinedEffectsOfType<IPacifismEffect>()).Returns([inactive.Object, peaceful.Object, peaceful.Object]);
		Assert.IsTrue(EmotionalCombatPolicy.IsPeaceful(actor.Object)); Assert.IsFalse(EmotionalCombatPolicy.IsPeaceful(actor.Object, true));
		var rage = new Mock<IRageEffect>(); rage.SetupGet(x => x.IsSuperRaging).Returns(true);
		rage.SetupGet(x => x.IsRaging).Returns(true); rage.Setup(x => x.Applies(actor.Object)).Returns(true);
		actor.Setup(x => x.CombinedEffectsOfType<IRageEffect>()).Returns([rage.Object]);
		Assert.IsTrue(EmotionalCombatPolicy.IsRaging(actor.Object)); Assert.IsTrue(EmotionalCombatPolicy.IsRaging(actor.Object, true));
	}

	[TestMethod]
	public void FuryMapping_OverflowingDeliveredAttributeBonus_RefusesConstruction()
	{
		var f = new MagicCastingFixture(); var trait = new Mock<ITraitDefinition>();
		trait.SetupGet(x => x.TraitType).Returns(TraitType.Attribute);
		var parent = new MagicSpellParent(f.Actor.Object, f.Spell, f.Actor.Object);
		Assert.ThrowsException<ArgumentException>(() => new SpellSourceFuryEffect(f.Actor.Object, parent,
			"test.emotion", 600, 36, State(7, endurance: 24), trait.Object, double.MaxValue));
	}

	[TestMethod]
	public void EmotionalChild_InvalidGroup_UsesExistingLifetimeDescriptorValidation()
	{
		var f = new MagicCastingFixture(); var parent = new MagicSpellParent(f.Actor.Object, f.Spell, f.Actor.Object);
		Assert.ThrowsException<ArgumentException>(() => new SpellSourceCalmEffect(f.Actor.Object, parent,
			"invalid group", 600, 24, State(1, endurance: 0)));
	}
}
