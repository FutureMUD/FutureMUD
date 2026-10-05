#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Body.Traits;
using MudSharp.Construction;
using MudSharp.Effects;
using MudSharp.Effects.Concrete;
using MudSharp.Effects.Concrete.SpellEffects;
using MudSharp.Framework;
using MudSharp.FutureProg;
using MudSharp.Magic;
using MudSharp.Magic.Emotions;
using MudSharp.RPG.Checks;

namespace MudSharp_Unit_Tests;

[TestClass]
public class FuryCalmStockProfileTests
{
	internal static EmotionalStockProfile Fury() => ArmageddonRousedFuryStock.Profile(10, 0.5, 5, 20, new(101, 102, 103, 104, 105, 106, 107));
	internal static EmotionalStockProfile Calm() => ArmageddonStillAngerStock.Profile(11, 11, 20,
		Enumerable.Range(1, 7).Select(x => new KeyValuePair<int, Difficulty>(x, Difficulty.Normal)));

	[DataTestMethod]
	[DataRow(101L, 1, 2), DataRow(101L, 7, 14), DataRow(102L, 1, 2), DataRow(102L, 7, 17),
	 DataRow(103L, 1, 2), DataRow(103L, 7, 17), DataRow(104L, 1, 3), DataRow(104L, 7, 24),
	 DataRow(105L, 1, 3), DataRow(105L, 7, 24), DataRow(106L, 1, 3), DataRow(106L, 7, 23),
	 DataRow(107L, 1, 4), DataRow(107L, 7, 28), DataRow(999L, 1, 3), DataRow(999L, 7, 21)]
	public void FuryTerrain_SourceMappingsAndFallback_ResolveLowHighUnits(long terrain, int grade, int units)
	{
		var calls = 0;
		var result = Fury().ResolveLifetime(grade, terrain, SpellPower.Standard, (lower, _) => { calls++; return lower; });
		Assert.AreEqual(TimeSpan.FromSeconds(units * 600), result.Duration);
		Assert.AreEqual(grade * (terrain == 107 ? 2 : 1), result.State.EndurancePoints);
		Assert.AreEqual(grade == 1 ? 0 : 1, calls);
	}

	[TestMethod]
	public void FuryEarth_HighGradeEndpointsAndEditableUnits_RetainIndependentNativeFields()
	{
		var profile = Fury();
		var lower = profile.ResolveLifetime(7, 107, SpellPower.ExtremelyWeak, (from, _) => from);
		var upper = profile.ResolveLifetime(7, 107, SpellPower.ExtremelyWeak, (_, to) => to);
		Assert.AreEqual(14.0, lower.State.EndurancePoints); Assert.AreEqual(31.0, upper.State.EndurancePoints);
		Assert.AreEqual(15.5, upper.State.EndurancePoints * profile.UnitsPerSourcePoint);
		Assert.AreEqual(5.0, upper.State.Intensity); Assert.AreEqual(SpellPower.ExtremelyWeak, upper.State.Power);
	}

	[DataTestMethod]
	[DataRow(EmotionalSpellKind.Fury), DataRow(EmotionalSpellKind.Calm)]
	public void ProfileXml_RoundTripAndImmutableInputs_PreserveAllEditableFields(EmotionalSpellKind kind)
	{
		var profile = kind == EmotionalSpellKind.Fury ? Fury() : Calm();
		var loaded = EmotionalStockProfile.Read(XElement.Parse(profile.SaveToXml().ToString()));
		Assert.IsTrue(XNode.DeepEquals(profile.SaveToXml(), loaded.SaveToXml()));
		var source = profile.Terrains.ToList();
		var saves = profile.Saves.ToList();
		var copy = new EmotionalStockProfile(profile.Kind, profile.Group, profile.UnitSeconds, profile.CapUnits,
			profile.EligibilityProgId, profile.Intensity, profile.TraitId, profile.UnitsPerSourcePoint,
			profile.Fallback, source, saves, profile.BreakOnAdmittedAttack);
		source.Clear(); saves.Clear();
		Assert.IsTrue(XNode.DeepEquals(profile.SaveToXml(), copy.SaveToXml()));
	}

	[DataTestMethod]
	[DataRow(1), DataRow(2), DataRow(3), DataRow(4), DataRow(5), DataRow(6), DataRow(7)]
	public void CalmDurationAndSave_UseResidualGradeWithoutEnduranceDraw(int grade)
	{
		var result = Calm().ResolveLifetime(grade, 999, SpellPower.ExtremelyStrong, (_, _) => throw new AssertFailedException("Calm must not draw endurance."));
		Assert.AreEqual(TimeSpan.FromSeconds(grade * 1200), result.Duration);
		Assert.AreEqual(0.0, result.State.EndurancePoints);
		Assert.AreEqual(Difficulty.Normal, Calm().SaveDifficulty(grade));
		Assert.IsTrue(Calm().BreakOnAdmittedAttack);
	}

	[TestMethod]
	public void FuryRecast_CapAndOldCompleteState_SurviveHighIncomingGrade()
	{
		var old = new EmotionalRetainedState(1, SpellPower.ExtremelyWeak, 12, 2);
		var result = Fury().ResolveLifetime(7, 107, SpellPower.ExtremelyStrong, (_, to) => to, TimeSpan.FromSeconds(35 * 600), old);
		Assert.AreSame(old, result.State); Assert.AreEqual(TimeSpan.FromSeconds(36 * 600), result.Duration);
	}

	[TestMethod]
	public void CalmRecast_StrongestStateAndCap_DoNotOrderNativePower()
	{
		var old = new EmotionalRetainedState(7, SpellPower.ExtremelyWeak, 11, 0);
		var result = Calm().ResolveLifetime(1, 999, SpellPower.ExtremelyStrong, (_, _) => 0, TimeSpan.FromSeconds(23 * 600), old);
		Assert.AreSame(old, result.State); Assert.AreEqual(TimeSpan.FromSeconds(24 * 600), result.Duration);
		Assert.ThrowsException<ArgumentException>(() => Calm().ResolveLifetime(7, 999, SpellPower.ExtremelyStrong, (_, _) => 0,
			TimeSpan.FromSeconds(600), old));
	}

	[DataTestMethod]
	[DataRow("missing trait"), DataRow("missing eligibility"), DataRow("zero units"), DataRow("nonfinite intensity"),
	 DataRow("duplicate terrain"), DataRow("duplicate lifetime"), DataRow("mapped exceptions"), DataRow("unsupported version")]
	public void FuryProfile_MalformedConfiguration_RefusesWithoutFallback(string scenario)
	{
		var xml = Fury().SaveToXml();
		switch (scenario)
		{
			case "missing trait": xml.SetAttributeValue("trait", 0); break;
			case "missing eligibility": xml.SetAttributeValue("eligibility", 0); break;
			case "zero units": xml.SetAttributeValue("units", 0); break;
			case "nonfinite intensity": xml.SetAttributeValue("intensity", "NaN"); break;
			case "duplicate terrain": xml.Element("Terrains")!.Add(new XElement(xml.Element("Terrains")!.Elements().First())); break;
			case "duplicate lifetime": xml.Add(new XElement(xml.Element("Lifetime")!)); break;
			case "mapped exceptions": xml.Element("HistoricalExceptions")!.SetAttributeValue("status", "guessed"); break;
			case "unsupported version": xml.SetAttributeValue("version", 2); break;
		}
		Assert.ThrowsException<ArgumentException>(() => EmotionalStockProfile.Read(xml));
	}

	[DataTestMethod]
	[DataRow("missing grade"), DataRow("duplicate grade"), DataRow("invalid difficulty"), DataRow("endurance bonus")]
	public void CalmProfile_IncompleteOrAmbiguousSave_Refuses(string scenario)
	{
		var xml = Calm().SaveToXml();
		if (scenario == "missing grade") xml.Element("Saves")!.Elements().Last().Remove();
		if (scenario == "duplicate grade") xml.Element("Saves")!.Add(new XElement(xml.Element("Saves")!.Elements().First()));
		if (scenario == "invalid difficulty") xml.Element("Saves")!.Elements().First().SetAttributeValue("difficulty", 999);
		if (scenario == "endurance bonus") xml.Element("Fallback")!.SetAttributeValue("endurance", 1);
		Assert.ThrowsException<ArgumentException>(() => EmotionalStockProfile.Read(xml));
	}

	[TestMethod]
	public void Profile_WrongKindMissingTerrainAndOverflow_Refuse()
	{
		Assert.ThrowsException<ArgumentException>(() => ArmageddonRousedFuryStock.Definition(1, 2, Calm()));
		Assert.ThrowsException<ArgumentException>(() => ArmageddonStillAngerStock.Definition(1, 2, Fury()));
		Assert.ThrowsException<ArgumentException>(() => Fury().TerrainRule(0));
		Assert.ThrowsException<InvalidOperationException>(() => Fury().SaveDifficulty(1));
		Assert.ThrowsException<ArgumentException>(() => ArmageddonRousedFuryStock.Profile(10, double.MaxValue, 5, 20, new(101, 102, 103, 104, 105, 106, 107)));
	}

	[TestMethod]
	public void Bindings_ExplicitOrdinaryAttributeAndCompilingProg_RejectDerivedOrMissingMappings()
	{
		var f = new MagicCastingFixture();
		var trait = new Mock<ITraitDefinition>(); trait.SetupGet(x => x.Id).Returns(10); trait.SetupGet(x => x.TraitType).Returns(TraitType.Attribute);
		f.Traits.Add(trait.Object);
		var prog = new Mock<IFutureProg>(); prog.SetupGet(x => x.Id).Returns(20); prog.SetupGet(x => x.ReturnType).Returns(ProgVariableTypes.Boolean);
		prog.Setup(x => x.MatchesParameters(It.IsAny<IEnumerable<ProgVariableTypes>>())).Returns(true); prog.Setup(x => x.Compile()).Returns(true);
		f.World.SetupGet(x => x.FutureProgs).Returns(MagicCastingFixture.Collection(() => new[] { prog.Object }));
		var terrains = Enumerable.Range(101, 7).Select(x => Mock.Of<ITerrain>(t => t.Id == x)).ToList();
		f.World.SetupGet(x => x.Terrains).Returns(MagicCastingFixture.Collection(() => terrains));
		Fury().ValidateBindings(f.World.Object);
		trait.SetupGet(x => x.TraitType).Returns(TraitType.DerivedAttribute);
		Assert.ThrowsException<InvalidOperationException>(() => Fury().ValidateBindings(f.World.Object));
		trait.SetupGet(x => x.TraitType).Returns(TraitType.Attribute);
		terrains.RemoveAt(0); Assert.ThrowsException<InvalidOperationException>(() => Fury().ValidateBindings(f.World.Object));
	}

	[TestMethod]
	public void CanonicalMetadata_PreservesSourceTreeAndMendCap60()
	{
		Assert.AreEqual(ArmageddonUnravelEnchantmentStock.Key, ArmageddonRousedFuryStock.PrerequisiteKey);
		Assert.AreEqual(ArmageddonRousedFuryStock.Key, ArmageddonStillAngerStock.PrerequisiteKey);
		Assert.AreEqual(ArmageddonMendFleshStock.Key, ArmageddonStillAngerStock.NextKey);
		Assert.AreEqual(80.0, ArmageddonRousedFuryStock.PrerequisiteRaw); Assert.AreEqual(80.0, ArmageddonStillAngerStock.PrerequisiteRaw);
		Assert.AreEqual(30, ArmageddonRousedFuryStock.Opening); Assert.AreEqual(30, ArmageddonStillAngerStock.Opening);
		Assert.AreEqual(90.0, ArmageddonRousedFuryStock.RawCap); Assert.AreEqual(90.0, ArmageddonStillAngerStock.RawCap);
		Assert.AreEqual(60.0, ArmageddonStillAngerStock.NextRawCap);
	}

	[TestMethod]
	public void ReviewedChildren_ProfileDerivedStates_PersistAndCounterWithoutChangingEndurance()
	{
		var f = new MagicCastingFixture();
		var trait = Mock.Of<ITraitDefinition>(x => x.Id == 10 && x.TraitType == TraitType.Attribute); f.Traits.Add(trait);
		var profile = Fury(); var state = profile.ResolveLifetime(7, 107, SpellPower.ExtremelyWeak, (_, to) => to).State;
		var parent = new MagicSpellParent(f.Actor.Object, f.Spell, f.Actor.Object, state.Power);
		var fury = new SpellSourceFuryEffect(f.Actor.Object, parent, profile.Group, profile.UnitSeconds, profile.CapUnits,
			state, trait, profile.UnitsPerSourcePoint);
		fury.ReduceSourceGrade(2);
		SpellSourceFuryEffect.InitialiseEffectType(); SpellSourceCalmEffect.InitialiseEffectType();
		var loaded = (SpellSourceFuryEffect)Effect.LoadEffect(fury.SaveToXml(new Dictionary<IEffect, TimeSpan>()), f.Actor.Object);
		Assert.IsNull(loaded.DefinitionError); Assert.AreEqual(5, loaded.State.SourceGrade);
		Assert.AreEqual(31.0, loaded.State.EndurancePoints); Assert.AreEqual(SpellPower.ExtremelyWeak, loaded.State.Power);
		Assert.AreSame(trait, loaded.EnduranceTrait); Assert.AreEqual(0.5, loaded.UnitsPerSourcePoint);
		var calmState = Calm().ResolveLifetime(1, 999, SpellPower.ExtremelyStrong, (_, _) => 0).State;
		var calmParent = new MagicSpellParent(f.Actor.Object, f.Spell, f.Actor.Object, calmState.Power);
		var calm = new SpellSourceCalmEffect(f.Actor.Object, calmParent, Calm().Group, 600, 24, calmState);
		var calmLoaded = (SpellSourceCalmEffect)Effect.LoadEffect(calm.SaveToXml(new Dictionary<IEffect, TimeSpan>()), f.Actor.Object);
		Assert.IsTrue(calmLoaded.IsPeaceful); Assert.IsTrue(calmLoaded.IsSuperPeaceful);
		Assert.AreEqual(calmState, calmLoaded.State);
	}
}
