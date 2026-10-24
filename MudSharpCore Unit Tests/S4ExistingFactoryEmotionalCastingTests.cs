#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Body.Traits;
using MudSharp.Combat;
using MudSharp.Construction;
using MudSharp.Effects;
using MudSharp.Effects.Concrete;
using MudSharp.Effects.Concrete.SpellEffects;
using MudSharp.Effects.Interfaces;
using MudSharp.Framework;
using MudSharp.Framework.Scheduling;
using MudSharp.FutureProg;
using MudSharp.Magic;
using MudSharp.Magic.Casting;
using MudSharp.Magic.Emotions;
using MudSharp.Magic.SpellEffects;
using MudSharp.PerceptionEngine;
using MudSharp.Planes;
using MudSharp.RPG.Checks;
namespace MudSharp_Unit_Tests;
[TestClass]
[DoNotParallelize]
public class S4ExistingFactoryEmotionalCastingTests
{
	private static EmotionalStockProfile CanonicalProfile(EmotionalSpellKind kind) => EmotionalStockProfile.Read(
		(kind == EmotionalSpellKind.Fury ? ArmageddonRousedFuryStock.Content(new(10, 20)) :
		 ArmageddonStillAngerStock.Content(new(11, 20, Enumerable.Range(1, 7).ToDictionary(x => x, _ => Difficulty.Normal))))
		.BuildDefinition(10, 1, 0).Descendants("SourceProfile").Single());
	[TestMethod]
	[DataRow(EmotionalSpellKind.Fury, 1)] [DataRow(EmotionalSpellKind.Fury, 7)]
	[DataRow(EmotionalSpellKind.Calm, 1)] [DataRow(EmotionalSpellKind.Calm, 7)]
	public void CanonicalPaidCast_GradeBoundaries_ApplyActiveNativeEmotionAndLockedTiming(EmotionalSpellKind kind, int grade)
	{
		using var f = new Fixture(kind);
		var result = f.Cast(grade); Assert.AreEqual(MagicCastingStatus.Succeeded, result.Status, result.Message);
		var parent = f.Parents.Single(); var child = (SpellEmotionalEffect)parent.SpellEffects.Single();
		Assert.AreEqual(grade, child.State.SourceGrade);
		Assert.AreEqual(TimeSpan.FromSeconds(60 * grade), f.Scheduler.RemainingDuration(parent));
		if (child is SpellSourceFuryEffect fury) { Assert.IsTrue(fury.IsRaging); Assert.AreEqual((double)grade, fury.State.EndurancePoints); }
		if (child is SpellSourceCalmEffect calm) { Assert.IsTrue(calm.IsPeaceful); Assert.IsTrue(calm.IsSuperPeaceful); }
	}
	[TestMethod]
	[DataRow(1, 7)] [DataRow(7, 1)]
	public void CanonicalFuryRecast_StrongerAndWeaker_UpdatesSameChildWithoutRemoval(int oldGrade, int newGrade)
	{
		using var f = new Fixture(EmotionalSpellKind.Fury);
		var parent = f.Existing(EmotionalSpellKind.Fury, oldGrade, 60 * oldGrade, SpellPower.ExtremelyWeak, oldGrade);
		var child = (SpellSourceFuryEffect)parent.SpellEffects.Single();
		var result = f.Cast(newGrade); Assert.AreEqual(MagicCastingStatus.Succeeded, result.Status, result.Message);
		Assert.AreSame(parent, f.Parents.Single()); Assert.AreSame(child, parent.SpellEffects.Single());
		Assert.AreEqual(newGrade, child.State.SourceGrade); Assert.AreEqual((double)newGrade, child.State.EndurancePoints);
		Assert.AreEqual(parent.Power, child.State.Power); Assert.AreEqual(SpellPower.ExtremelyWeak, parent.Power);
		Assert.AreEqual(TimeSpan.FromSeconds(newGrade * 60), f.Scheduler.RemainingDuration(parent));
		Assert.AreEqual(0, f.EmotionalRemovals);
	}
	private sealed class Clock(DateTime now) : TimeProvider
	{
		internal DateTime Now = now;
		public override DateTimeOffset GetUtcNow() => new(Now);
		internal void Advance(int seconds) => Now = Now.AddSeconds(seconds);
	}

	private sealed class Fixture : IDisposable
	{
		internal readonly MagicCastingFixture F = new();
		internal readonly Clock Clock;
		internal readonly EffectScheduler Scheduler;
		internal readonly EffectHandler Handler;
		internal readonly MagicSpell Spell;
		internal readonly ITraitDefinition Attribute;
		private readonly IDisposable _clockScope;
		internal SourceEmotionalEffect Template => (SourceEmotionalEffect)Spell.SpellEffects.Single();
		internal IEnumerable<MagicSpellParent> Parents => Handler.Effects.OfType<MagicSpellParent>();
		internal int SaveCalls;
		internal Difficulty SaveDifficulty;
		internal Outcome Resistance = Outcome.Fail;
		internal Action? SaveCallback;
		internal int EmotionalRemovals;
		internal Action? ParentCallback;
		internal Action? BeforeChild;

		internal Fixture(EmotionalSpellKind kind)
		{
			Clock = new(F.Now); _clockScope = RuntimeClock.Push(Clock);
			Scheduler = new(F.World.Object, Clock); F.World.SetupGet(x => x.EffectScheduler).Returns(Scheduler);
			Handler = new(F.Actor.Object); F.Actor.SetupGet(x => x.Effects).Returns(() => Handler.Effects);
			F.Body.SetupGet(x => x.Actor).Returns(F.Actor.Object);
			F.Actor.SetupGet(x => x.Combat).Returns((ICombat)null!);
			F.Actor.SetupGet(x => x.CombatTarget).Returns((IPerceiver)null!);
			F.Actor.Setup(x => x.AddEffect(It.IsAny<IEffect>())).Callback<IEffect>(effect =>
			{ if (effect is SpellEmotionalEffect) BeforeChild?.Invoke(); Handler.AddEffect(effect); });
			F.Actor.Setup(x => x.AddEffect(It.IsAny<IEffect>(), It.IsAny<TimeSpan>())).Callback<IEffect, TimeSpan>((effect, duration) =>
			{ Handler.AddEffect(effect, duration); if (effect is MagicSpellParent) ParentCallback?.Invoke(); });
			F.Actor.Setup(x => x.RemoveEffect(It.IsAny<IEffect>(), It.IsAny<bool>())).Callback<IEffect, bool>((effect, fire) =>
			{ if (effect is SpellEmotionalEffect) EmotionalRemovals++; Handler.RemoveEffect(effect, fire); });
			F.Actor.Setup(x => x.CombinedEffectsOfType<IAdmittedHostileAttackEffect>()).Returns(() => Handler.Effects.OfType<IAdmittedHostileAttackEffect>());
			F.Actor.Setup(x => x.EffectsOfType<IAdmittedHostileAttackEffect>(It.IsAny<Predicate<IAdmittedHostileAttackEffect>>()))
				.Returns<Predicate<IAdmittedHostileAttackEffect>>(predicate => Handler.Effects.OfType<IAdmittedHostileAttackEffect>().Where(x => predicate is null || predicate(x)));
			Attribute = Mock.Of<ITraitDefinition>(x => x.Id == 10 && x.TraitType == TraitType.Attribute);
			F.Traits.Add(Attribute); F.Traits.Add(Mock.Of<ITraitDefinition>(x => x.Id == 11 && x.TraitType == TraitType.Skill));
			var terrains = Enumerable.Range(101, 7).Select(id => Mock.Of<ITerrain>(x => x.Id == id)).ToArray();
			F.World.SetupGet(x => x.Terrains).Returns(MagicCastingFixture.Collection(() => terrains));
			Mock.Get(F.Actor.Object.Location).Setup(x => x.Terrain(It.IsAny<IPerceiver>())).Returns(terrains.Last());
			var known = F.World.Object.FutureProgs.Get(1)!; var allowed = new Mock<IFutureProg>();
			allowed.SetupGet(x => x.Id).Returns(20); allowed.SetupGet(x => x.ReturnType).Returns(ProgVariableTypes.Boolean);
			allowed.SetupGet(x => x.FunctionText).Returns("return true"); allowed.Setup(x => x.Compile()).Returns(true);
			allowed.Setup(x => x.MatchesParameters(It.IsAny<IEnumerable<ProgVariableTypes>>())).Returns(true);
			object permitted = true; allowed.Setup(x => x.ExecuteWithStatus(out permitted, It.IsAny<object[]>())).Returns(true);
			F.World.SetupGet(x => x.FutureProgs).Returns(MagicCastingFixture.Collection(() => new[] { known, allowed.Object }));
			var model = (MudSharp.Models.MagicSpell)typeof(MagicSpell).GetMethod("SnapshotModel", BindingFlags.Instance | BindingFlags.NonPublic)!.Invoke(F.Spell, [])!;
			model.Definition = (kind == EmotionalSpellKind.Fury ? ArmageddonRousedFuryStock.Definition(10, 1, CanonicalProfile(EmotionalSpellKind.Fury))
				: ArmageddonStillAngerStock.Definition(10, 1, CanonicalProfile(EmotionalSpellKind.Calm))).ToString();
			model.AppliedEffectsAreExclusive = true; model.TargetNullEmote = "No suitable target.";
			Spell = new(model, F.World.Object) { EffectDurationExpression = new TraitExpression("0", F.World.Object) };
			F.Spells.Clear(); F.Spells.Add(Spell);
			F.Skills[1] = 100; F.Acquire(7); F.Actor.Setup(x => x.TargetActorOrCorpse("self", It.IsAny<PerceiveIgnoreFlags>())).Returns(F.Actor.Object);
			var save = new Mock<ICheck>();
			save.Setup(x => x.CheckAgainstAllDifficulties(It.IsAny<IPerceivableHaveTraits>(), It.IsAny<Difficulty>(), It.IsAny<ITraitDefinition>(),
				It.IsAny<IPerceivable>(), It.IsAny<double>(), It.IsAny<TraitUseType>(), It.IsAny<(string, object)[]>()))
				.Returns<IPerceivableHaveTraits, Difficulty, ITraitDefinition, IPerceivable, double, TraitUseType, (string, object)[]>((_, difficulty, _, _, _, _, _) =>
				{ SaveCalls++; SaveDifficulty = difficulty; SaveCallback?.Invoke(); return Enum.GetValues<Difficulty>().ToDictionary(x => x, _ => CheckOutcome.SimpleOutcome(CheckType.ResistMagicSpellCheck, Resistance)); });
			F.World.Setup(x => x.GetCheck(CheckType.ResistMagicSpellCheck)).Returns(save.Object);
		}

		internal MagicSpellParent Existing(EmotionalSpellKind kind, int grade, int seconds, SpellPower power = SpellPower.Standard, double endurance = 5)
		{
			var profile = kind == EmotionalSpellKind.Fury ? CanonicalProfile(EmotionalSpellKind.Fury) : CanonicalProfile(EmotionalSpellKind.Calm);
			var head = new MagicSpellParent(F.Actor.Object, Spell, F.Actor.Object, power);
			var state = new EmotionalRetainedState(grade, power, profile.Intensity, kind == EmotionalSpellKind.Fury ? endurance : 0);
			SpellEmotionalEffect child = kind == EmotionalSpellKind.Fury
				? new SpellSourceFuryEffect(F.Actor.Object, head, profile.Group, profile.UnitSeconds, profile.CapUnits, state, Attribute, profile.UnitsPerSourcePoint)
				: new SpellSourceCalmEffect(F.Actor.Object, head, profile.Group, profile.UnitSeconds, profile.CapUnits, state);
			head.AddSpellEffect(child); Handler.AddEffect(child); Handler.AddEffect(head, TimeSpan.FromSeconds(seconds)); return head;
		}
		internal MagicCastingResult Cast(int grade) { F.Now = Clock.Now; F.Balances[F.Resources[1]] = 100; return F.Service.Cast(F.Intent(grade, false)); }
		internal bool Applied(MagicCastingResult result) => (bool?)XElement.Parse(F.Store.Operations[result.OperationId!.Value].Definition).Attribute("applied") == true;
		public void Dispose() => _clockScope.Dispose();
	}
}

