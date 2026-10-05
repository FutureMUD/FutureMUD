#nullable enable

using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using System.Xml.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Effects.Concrete;
using MudSharp.Framework;
using MudSharp.FutureProg;
using MudSharp.Magic;
using MudSharp.Magic.Emotions;
using MudSharp.Magic.SpellEffects;
using MudSharp.Magic.Casting;
using MudSharp.RPG.Checks;

namespace MudSharp_Unit_Tests;

[TestClass]
[DoNotParallelize]
public class FuryCalmPreparedTemplateTests
{
	private readonly List<(IDictionary Registry, DictionaryEntry[] Original)> _factorySnapshots = [];

	[TestInitialize]
	public void RegisterOnlyOwnedPreparationFactories()
	{
		// Own this bounded test scope; do not leave unqualified types in the process-wide native registry.
		_ = SpellEffectFactory.RegisteredLoadTypes;
		Assert.IsFalse(SpellEffectFactory.RegisteredLoadTypes.Contains("sourcefury"));
		Assert.IsFalse(SpellEffectFactory.RegisteredLoadTypes.Contains("sourcecalm"));
		foreach (var name in new[] { "_loadTimeFactories", "_builderFactories" })
		{
			var registry = (IDictionary)typeof(SpellEffectFactory).GetField(name, BindingFlags.Static | BindingFlags.NonPublic)!.GetValue(null)!;
			var entries = new List<DictionaryEntry>(); var enumerator = registry.GetEnumerator();
			while (enumerator.MoveNext()) entries.Add(enumerator.Entry);
			_factorySnapshots.Add((registry, entries.ToArray()));
		}
		SourceFuryEffect.RegisterPreparationFactory(); SourceCalmEffect.RegisterPreparationFactory();
	}

	[TestCleanup]
	public void RestoreNativeFactoryRegistries()
	{
		foreach (var (registry, original) in _factorySnapshots)
		{
			registry.Clear();
			foreach (var entry in original) registry.Add(entry.Key, entry.Value);
		}
		_factorySnapshots.Clear();
	}
	[DataTestMethod]
	[DataRow(EmotionalSpellKind.Fury), DataRow(EmotionalSpellKind.Calm)]
	public void Definition_NormalTemplateFactoriesAndClone_PreserveProfileAndControlledPower(EmotionalSpellKind kind)
	{
		var f = new MagicCastingFixture(); var spell = Load(f, kind);
		var template = (SourceEmotionalEffect)spell.SpellEffects.Single();
		Assert.IsNull(template.ProfileError); Assert.AreEqual(kind, template.Profile!.Kind);
		Assert.AreEqual(kind == EmotionalSpellKind.Fury ? 20.0 : 7.0, spell.GradeProfile!.Efficiency!.MinimumCost);
		Assert.AreEqual(30, spell.GradeProfile.OpeningSkill); Assert.IsTrue(spell.GradeProfile.Practice!.Enabled);
		Assert.IsNull(spell.LifetimeConfigurationError); Assert.IsFalse(template is IMagicSpellEffectLifetimePolicy);
		Assert.IsTrue(XNode.DeepEquals(template.SaveToXml(), template.Clone().SaveToXml()));
		Assert.IsFalse(spell.CasterSpellEffects.Any());
	}

	[DataTestMethod]
	[DataRow(EmotionalSpellKind.Fury), DataRow(EmotionalSpellKind.Calm)]
	public void Template_BuildersPersistEdits_RejectInvalidEditsWithoutMutation(EmotionalSpellKind kind)
	{
		var f = new MagicCastingFixture(); var spell = Load(f, kind);
		var effect = (SourceEmotionalEffect)spell.SpellEffects.Single();
		Assert.IsTrue(effect.BuildingCommand(f.Actor.Object, new StringStack("intensity 12.5")));
		Assert.IsTrue(effect.BuildingCommand(f.Actor.Object, new StringStack("lifetime builder.emotion 300 20")));
		Assert.AreEqual(12.5, effect.Profile!.Intensity); Assert.AreEqual(300, effect.Profile.UnitSeconds);
		if (kind == EmotionalSpellKind.Fury)
		{
			Assert.IsTrue(effect.BuildingCommand(f.Actor.Object, new StringStack("units 0.25")));
			Assert.IsTrue(effect.BuildingCommand(f.Actor.Object, new StringStack("fallback 5 2 0")));
			Assert.AreEqual(17, effect.Profile!.TerrainRule(999).DurationUnits(7));
		}
		else
		{
			Assert.IsTrue(effect.BuildingCommand(f.Actor.Object, new StringStack("save 7 Hard")));
			Assert.IsTrue(effect.BuildingCommand(f.Actor.Object, new StringStack("attackbreak off")));
			Assert.AreEqual(Difficulty.Hard, effect.Profile!.SaveDifficulty(7)); Assert.IsFalse(effect.Profile.BreakOnAdmittedAttack);
		}
		var saved = effect.SaveToXml();
		Assert.IsFalse(effect.BuildingCommand(f.Actor.Object, new StringStack("intensity NaN")));
		Assert.IsFalse(effect.BuildingCommand(f.Actor.Object, new StringStack("lifetime invalid 0 1")));
		Assert.IsFalse(effect.BuildingCommand(f.Actor.Object, new StringStack("intensity 1 extra")));
		Assert.IsTrue(XNode.DeepEquals(saved, effect.SaveToXml()));
		var restored = (SourceEmotionalEffect)SpellEffectFactory.LoadEffect(saved, spell);
		Assert.IsTrue(XNode.DeepEquals(saved, restored.SaveToXml())); Assert.IsNull(restored.ProfileError);
	}

	[DataTestMethod]
	[DataRow(EmotionalSpellKind.Fury), DataRow(EmotionalSpellKind.Calm)]
	public void Template_MalformedPersistedProfile_PreservesRepairBytesAndRefusesAllExecution(EmotionalSpellKind kind)
	{
		var f = new MagicCastingFixture(); var spell = Load(f, kind);
		var xml = spell.SpellEffects.Single().SaveToXml(); xml.Element("SourceProfile")!.SetAttributeValue("units", "broken");
		var effect = (SourceEmotionalEffect)SpellEffectFactory.LoadEffect(xml, spell);
		Assert.IsNotNull(effect.ProfileError); Assert.IsNull(effect.Profile);
		Assert.IsTrue(XNode.DeepEquals(xml, effect.SaveToXml()));
		Assert.IsTrue(XNode.DeepEquals(xml, effect.Clone().SaveToXml()));
		Assert.ThrowsException<InvalidOperationException>(() => effect.CapturePreparedSelection(f.Actor.Object, f.Actor.Object));
		Assert.IsFalse(effect.TryConfirmPreparedSelection(f.Actor.Object, f.Actor.Object, out var error));
		Assert.AreEqual(SourceEmotionalEffect.RuntimeIntegrationError, error);
		var parent = new MagicSpellParent(f.Actor.Object, spell, f.Actor.Object);
		Assert.ThrowsException<InvalidOperationException>(() => effect.Apply(f.Actor.Object, f.Actor.Object,
			OpposedOutcomeDegree.None, SpellPower.Standard, parent, []));
		Assert.ThrowsException<InvalidOperationException>(() => effect.GetOrApplyEffect(f.Actor.Object, f.Actor.Object,
			OpposedOutcomeDegree.None, SpellPower.Standard, parent, []));
	}

	[DataTestMethod]
	[DataRow(EmotionalSpellKind.Fury), DataRow(EmotionalSpellKind.Calm)]
	public void CastingPreflight_StagedValidContent_RefusesBeforePaymentJournalOrRoll(EmotionalSpellKind kind)
	{
		var f = new MagicCastingFixture(); var spell = Load(f, kind); f.Spells.Clear(); f.Spells.Add(spell); f.Acquire(1);
		f.Actor.Setup(x => x.TargetActorOrCorpse("self", It.IsAny<PerceiveIgnoreFlags>())).Returns(f.Actor.Object);
		Assert.IsTrue(spell.ReadyForGame, spell.ReadyForGame ? "" : spell.WhyNotReadyForGame(f.Actor.Object));
		var intent = f.Intent(1, false); var writes = f.Store.Writes;
		var quote = f.Service.Quote(intent); Assert.IsNull(quote.Invocation);
		StringAssert.Contains(quote.Reason, SourceEmotionalEffect.RuntimeIntegrationError);
		var result = f.Service.Cast(intent); Assert.AreEqual(MagicCastingStatus.Refused, result.Status);
		StringAssert.Contains(result.Message, SourceEmotionalEffect.RuntimeIntegrationError);
		Assert.AreEqual(writes, f.Store.Writes); Assert.AreEqual(0, f.Rolls); Assert.AreEqual(0, f.SkillUses);
		foreach (var balance in f.Balances.Values) Assert.AreEqual(100.0, balance);
	}

	[TestMethod]
	public void CalmBuilder_ResetAndIncrementalRepair_PreserveExplicitMappingsWithoutEnablingCast()
	{
		var f = new MagicCastingFixture(); var spell = Load(f, EmotionalSpellKind.Calm);
		var effect = (SourceEmotionalEffect)spell.SpellEffects.Single();
		Assert.IsTrue(effect.BuildingCommand(f.Actor.Object, new StringStack("reset"))); Assert.IsNull(effect.Profile);
		Assert.IsTrue(effect.BuildingCommand(f.Actor.Object, new StringStack("savetrait 1")));
		Assert.IsTrue(effect.BuildingCommand(f.Actor.Object, new StringStack("eligibility 20")));
		Assert.IsTrue(effect.BuildingCommand(f.Actor.Object, new StringStack("intensity 11")));
		for (var grade = 1; grade <= 7; grade++)
			Assert.IsTrue(effect.BuildingCommand(f.Actor.Object, new StringStack($"save {grade} Normal")));
		Assert.IsNotNull(effect.Profile); Assert.AreEqual(1L, effect.Profile!.TraitId);
		Assert.IsTrue(effect.Profile.BreakOnAdmittedAttack);
		Assert.ThrowsException<InvalidOperationException>(() => effect.CapturePreparedSelection(f.Actor.Object, f.Actor.Object));
	}

	private static MagicSpell Load(MagicCastingFixture f, EmotionalSpellKind kind)
	{
		var model = (MudSharp.Models.MagicSpell)typeof(MagicSpell).GetMethod("SnapshotModel", BindingFlags.NonPublic | BindingFlags.Instance)!.Invoke(f.Spell, [])!;
		var profile = kind == EmotionalSpellKind.Fury ? FuryCalmStockProfileTests.Fury() : FuryCalmStockProfileTests.Calm();
		var known = f.World.Object.FutureProgs.Get(1);
		var filter = new Mock<IFutureProg>(); filter.SetupGet(x => x.Id).Returns(20);
		filter.SetupGet(x => x.ReturnType).Returns(ProgVariableTypes.Boolean);
		filter.Setup(x => x.MatchesParameters(It.IsAny<IEnumerable<ProgVariableTypes>>())).Returns(true);
		filter.Setup(x => x.Compile()).Returns(true);
		object allowed = true; filter.Setup(x => x.ExecuteWithStatus(out allowed, It.IsAny<object[]>())).Returns(true);
		f.World.SetupGet(x => x.FutureProgs).Returns(MagicCastingFixture.Collection(() => new[] { known, filter.Object }));
		var definition = kind == EmotionalSpellKind.Fury ? ArmageddonRousedFuryStock.Definition(10, 1, profile) : ArmageddonStillAngerStock.Definition(10, 1, profile);
		model.Definition = definition.ToString(); model.AppliedEffectsAreExclusive = true;
		model.TargetNullEmote = "No suitable target.";
		return new MagicSpell(model, f.World.Object);
	}
}
