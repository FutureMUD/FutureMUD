#nullable enable

using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Body;
using MudSharp.Body.Traits;
using MudSharp.Health;
using MudSharp.Magic;
using MudSharp.Effects.Interfaces;
using MudSharp.Construction;
using MudSharp.RPG.Checks;

namespace MudSharp_Unit_Tests;

public partial class MagicEngineV4Tests
{
	private static IMagicalSubstance PreparedSubstance(MagicCastingFixture f, params SubstanceEffectEntry[] entries)
	{
		var substance = new Mock<IMagicalSubstance>();
		substance.SetupGet(x => x.Id).Returns(900);
		substance.SetupGet(x => x.Gameworld).Returns(f.World.Object);
		substance.SetupGet(x => x.ReferenceDose).Returns(1);
		substance.SetupGet(x => x.Vectors).Returns(DrugVector.Ingested);
		substance.SetupGet(x => x.Entries).Returns(entries);
		substance.SetupGet(x => x.ReadinessErrors).Returns(() => entries.SelectMany(x => SubstanceSpellResolver.Errors(f.World.Object.MagicSpells.Get(x.SpellId), x)).ToArray());
		f.Body.SetupGet(x => x.Actor).Returns(f.Actor.Object);
		return substance.Object;
	}

	private static SubstanceEffectEntry PreparedEntry(long spell = 1) => new()
	{
		SpellId = spell, NativeSpellGrade = 2, MinimumDose = 1, MaximumDose = 1,
		Lifecycle = SubstanceLifecycle.Activation, Scaling = SubstanceScaling.Magnitude
	};

	[TestMethod]
	public void NativeSubstance_WaystepUsesRecipientWithoutCastingResourcesOrSkill()
	{
		var f = new WeightedFixture(false);
		var substance = PreparedSubstance(f.Pair.F, PreparedEntry());
		Assert.AreEqual("", string.Join("; ", substance.ReadinessErrors));
		Assert.IsTrue(MagicalExposure.Scripted(f.Pair.F.Actor.Object, substance, 1, DrugVector.Ingested));
		Assert.AreEqual(1, f.Moves);
		Assert.AreEqual(0, f.Pair.F.Rolls);
		Assert.AreEqual(0, f.Pair.F.SkillUses);
		Assert.AreEqual(100.0, f.Pair.F.Balances[f.Pair.F.Resources[1]]);
	}

	[TestMethod]
	public void NativeSubstance_SelfRecipientStillReceivesConfiguredResistanceCheck()
	{
		var f = new WeightedFixture(false);
		var spell = (MagicSpell)f.Pair.F.Spells.Single();
		spell.OpposedTrait = spell.CastingTrait;
		spell.OpposedDifficulty = Difficulty.Normal;
		f.Pair.F.Outcome = Outcome.MajorPass;
		var substance = PreparedSubstance(f.Pair.F, PreparedEntry());
		Assert.IsTrue(MagicalExposure.Scripted(f.Pair.F.Actor.Object, substance, 1, DrugVector.Ingested));
		Assert.AreEqual(1, f.Pair.F.Rolls, "Only the recipient's resistance check should roll.");
		Assert.AreEqual(0, f.Moves);
	}

	[TestMethod]
	public void NativeSubstance_NoMagicRoomPreventsTravel()
	{
		var f = new WeightedFixture(false);
		var policy = new Mock<IRoomPolicyEffect>();
		policy.SetupGet(x => x.Restrictions).Returns(RoomRestrictions.Magic);
		policy.Setup(x => x.Applies()).Returns(true);
		f.Pair.Source.Setup(x => x.EffectsOfType<IRoomPolicyEffect>(It.IsAny<Predicate<IRoomPolicyEffect>>())).Returns([policy.Object]);
		var substance = PreparedSubstance(f.Pair.F, PreparedEntry());
		Assert.IsTrue(MagicalExposure.Scripted(f.Pair.F.Actor.Object, substance, 1, DrugVector.Ingested));
		Assert.AreEqual(0, f.Moves);
	}

	[TestMethod]
	public void NativeSubstance_DuplicateEntriesBothExecute()
	{
		var f = new WeightedFixture(false);
		var substance = PreparedSubstance(f.Pair.F, PreparedEntry(), PreparedEntry());
		Assert.IsTrue(MagicalExposure.Scripted(f.Pair.F.Actor.Object, substance, 1, DrugVector.Ingested));
		Assert.AreEqual(2, f.Moves);
	}

	[TestMethod]
	public void NativeSubstance_InactiveBodyCannotMoveActiveBody()
	{
		var f = new WeightedFixture(false);
		var substance = PreparedSubstance(f.Pair.F, PreparedEntry());
		var inactive = new Mock<IBody>();
		inactive.SetupGet(x => x.Actor).Returns(f.Pair.F.Actor.Object);
		inactive.SetupGet(x => x.Gameworld).Returns(f.Pair.F.World.Object);
		MagicalExposure.Scripted(inactive.Object, substance, 1, DrugVector.Ingested);
		Assert.AreEqual(0, f.Moves);
	}

	[TestMethod]
	public void NativeSubstance_InvalidLaterEntryPreventsPartialActivation()
	{
		var f = new WeightedFixture(false);
		var substance = PreparedSubstance(f.Pair.F, PreparedEntry(), PreparedEntry(999));
		Assert.IsFalse(MagicalExposure.Scripted(f.Pair.F.Actor.Object, substance, 1, DrugVector.Ingested));
		Assert.AreEqual(0, f.Moves);
	}

	[TestMethod]
	public void NativeSubstance_RequiresDurationAndFixedInputs()
	{
		var f = new MagicCastingFixture();
		f.Spell.EffectDurationExpression = null!;
		Assert.IsTrue(SubstanceSpellResolver.Errors(f.Spell, PreparedEntry()).Any(x => x.Contains("duration expression")));
		f.Spell.EffectDurationExpression = new TraitExpression("variable+10", f.World.Object);
		Assert.IsTrue(SubstanceSpellResolver.Errors(f.Spell, PreparedEntry()).Any(x => x.Contains("variable inputs")));
	}

	[TestMethod]
	public void NativeSubstance_DurationOverflowDoesNotAbortLaterEntry()
	{
		var f = new WeightedFixture(false);
		var invalid = f.Pair.F.Spell;
		invalid.EffectDurationExpression = new TraitExpression("1000000000000", f.Pair.F.World.Object);
		// The fixture's original spell is no longer in the catalogue; assign a distinct ID via its model.
		var row = invalid.SnapshotModel();
		row.Id = 50;
		var oversized = new MagicSpell(row, f.Pair.F.World.Object)
		{
			EffectDurationExpression = new TraitExpression("1000000000000", f.Pair.F.World.Object)
		};
		f.Pair.F.Spells.Add(oversized);
		var substance = PreparedSubstance(f.Pair.F, PreparedEntry(50), PreparedEntry());
		Assert.AreEqual("", string.Join("; ", substance.ReadinessErrors));
		Assert.IsTrue(MagicalExposure.Scripted(f.Pair.F.Actor.Object, substance, 1, DrugVector.Ingested));
		Assert.AreEqual(1, f.Moves);
	}

	[TestMethod]
	public void NativeSubstance_GradeRoundTripsAndLegacyDefaultsToZero()
	{
		var xml = MagicalSubstance.SaveEntry(PreparedEntry());
		Assert.AreEqual(2, MagicalSubstance.LoadEntry(xml).NativeSpellGrade);
		xml.Attribute("nativegrade")!.Remove();
		Assert.AreEqual(0, MagicalSubstance.LoadEntry(xml).NativeSpellGrade);
	}
}
