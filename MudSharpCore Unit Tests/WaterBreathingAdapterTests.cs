#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Body.Traits;
using MudSharp.Body;
using MudSharp.Character;
using MudSharp.Effects;
using MudSharp.Effects.Concrete;
using MudSharp.Effects.Concrete.SpellEffects;
using MudSharp.Form.Material;
using MudSharp.Framework;
using MudSharp.Framework.Scheduling;
using MudSharp.Health.Breathing;
using MudSharp.Magic;
using MudSharp.Magic.SpellEffects;
using MudSharp.RPG.Checks;

namespace MudSharp_Unit_Tests;

[TestClass, DoNotParallelize]
public class WaterBreathingAdapterTests
{
	private const string Group = "armageddon.water_breathing";
	private const string Definition = "<Effect type='sourcewaterbreathing'><WaterScope version='1'><Liquid id='51'/></WaterScope><LifetimePolicy version='1' mode='accumulate' group='armageddon.water_breathing' unitSeconds='600' maximumUnits='36' retainStrongestGrade='true'/></Effect>";

	[DataTestMethod, DataRow(1, 0, 600), DataRow(1, 3, 1800), DataRow(7, 3, 1800), DataRow(7, 21, 12600)]
	public void PaidCasts_SelectSourceDurationOnceAcrossAllPreparations(int grade, int draw, int seconds)
	{
		using var f = new Fixture(); using var random = Constants.PushRandom(f.Random); f.Random.Value = draw;
		var result = f.Cast(grade);
		Assert.AreEqual(MagicCastingStatus.Succeeded, result.Status, result.Message);
		Assert.AreEqual(1, f.Random.Draws); Assert.AreEqual((grade / 2, grade * 3 + 1), f.Random.Bounds);
		Assert.AreEqual(f.Clock.Now.AddSeconds(seconds), f.Scheduler.ScheduledExpiry(f.Parent));
		Assert.AreEqual(grade, f.Parent.LifetimeState!.Grade);
		Assert.AreEqual(100.0 - grade * 5, f.F.Balances[f.F.Resources[1]]); Assert.IsTrue(f.Applied(result));
		Assert.IsTrue(f.Child.AppliesToFluid(f.Water)); Assert.IsFalse(f.Child.AppliesToFluid(f.OtherWater));
		Assert.IsFalse(f.Child.AppliesToFluid(Mock.Of<IGas>()));
	}

	[TestMethod]
	public void Recast_AccumulatesToCap36RetainsStrongestGradeAndPaysNoChangeWithoutMastery()
	{
		using var f = new Fixture(); using var random = Constants.PushRandom(f.Random);
		var old = f.Seed(21001, 7, SpellPower.ExtremelyStrong);
		var result = f.Cast(1);
		Assert.AreEqual(MagicCastingStatus.Succeeded, result.Status, result.Message); Assert.IsTrue(f.Applied(result));
		Assert.AreEqual(f.Clock.Now.AddSeconds(21600), f.Scheduler.ScheduledExpiry(f.Parent));
		Assert.AreEqual(7, f.Parent.LifetimeState!.Grade); Assert.AreEqual(SpellPower.ExtremelyStrong, f.Parent.Power);
		Assert.IsFalse(f.Handler.Effects.Contains(old)); Assert.IsFalse(f.Scheduler.IsScheduled(old)); Assert.IsFalse(old.SpellEffects.Any());
		f.F.Acquire(1); result = f.Cast(2, true);
		Assert.AreEqual(MagicCastingStatus.Succeeded, result.Status, result.Message); Assert.IsFalse(f.Applied(result));
		Assert.AreEqual(0, f.F.Samples); Assert.AreEqual(1, f.F.Service.Acquisition(f.F.Actor.Object, 1)!.ControlledGrade);
		Assert.IsTrue(f.F.Balances[f.F.Resources[1]] < 100); Assert.AreEqual(2, f.Random.Draws);
	}

	[TestMethod]
	public void PreparedDelivery_UsesSameNativeLifetimeWithoutSecondPayment()
	{
		using var f = new Fixture(); using var random = Constants.PushRandom(f.Random); f.Random.Value = 3;
		f.Seed(3001, 6, SpellPower.Weak);
		var copy = f.Copy(1); copy.ResolveTriggeredSpell(f.F.Actor.Object, f.F.Actor.Object, copy.GradeProfile!.Grades.Single(x => x.Grade == 1).Power);
		Assert.AreEqual(1, f.Random.Draws); Assert.AreEqual(100.0, f.F.Balances[f.F.Resources[1]]);
		Assert.AreEqual(f.Clock.Now.AddSeconds(5400), f.Scheduler.ScheduledExpiry(f.Parent));
		Assert.AreEqual(TimeSpan.FromSeconds(5400), f.Parent.ResolvedDuration); Assert.AreEqual(6, f.Parent.LifetimeState!.Grade);
	}

	[TestMethod]
	public void RetainedStrength_UsesSourceGradeEvenWhenLowGradeHasStrongerNativePower()
	{
		using var f = new Fixture(); using var random = Constants.PushRandom(f.Random);
		Assert.IsTrue(f.Spell.BuildingCommand(f.F.Actor.Object, new StringStack("grades grade 1 ExtremelyStrong 0 0")));
		f.Seed(3000, 7, SpellPower.ExtremelyWeak); var result = f.Cast(1);
		Assert.AreEqual(MagicCastingStatus.Succeeded, result.Status, result.Message);
		Assert.AreEqual(7, f.Parent.LifetimeState!.Grade); Assert.AreEqual(SpellPower.ExtremelyWeak, f.Parent.Power);
		Assert.AreEqual(f.Clock.Now.AddSeconds(3600), f.Scheduler.ScheduledExpiry(f.Parent));
	}

	[TestMethod]
	public void OriginalAndPotentialReflectedRecipient_ReuseOneSelectionAndRefuseCasterCohortDrift()
	{
		using var f = new Fixture(); using var random = Constants.PushRandom(f.Random); var old = f.Seed(3000, 7, SpellPower.Weak);
		var target = new Mock<ICharacter>(); target.SetupGet(x => x.Gameworld).Returns(f.F.World.Object);
		target.SetupGet(x => x.Body).Returns(Mock.Of<IBody>()); target.SetupGet(x => x.Location).Returns(f.F.Actor.Object.Location);
		target.SetupGet(x => x.Effects).Returns(Array.Empty<IEffect>());
		var copy = f.Copy(1); var effect = (IMagicSpellEffectPreparedSelection)copy.SpellEffects.Single();
		var token = effect.CapturePreparedSelection(f.F.Actor.Object, target.Object)!;
		Assert.AreSame(token, effect.CapturePreparedSelection(f.F.Actor.Object, f.F.Actor.Object));
		var fresh = (IMagicSpellEffectPreparedSelection)f.Copy(1).SpellEffects.Single();
		Assert.IsTrue(fresh.TryReusePreparedSelection(token, f.F.Actor.Object, target.Object, out var error), error);
		Assert.IsTrue(fresh.TryConfirmPreparedSelection(f.F.Actor.Object, f.F.Actor.Object, out error), error);
		f.Scheduler.Reschedule(old, TimeSpan.FromSeconds(3600));
		Assert.IsFalse(fresh.TryConfirmPreparedSelection(f.F.Actor.Object, target.Object, out error));
		Assert.AreEqual(1, f.Random.Draws); Assert.AreEqual(100.0, f.F.Balances[f.F.Resources[1]]);
	}

	[TestMethod]
	public void GenericWaterBreathingRemoval_RemovesScopedSubtypeAndNativeParentSchedule()
	{
		using var f = new Fixture(); using var random = Constants.PushRandom(f.Random); var result = f.Cast(7); var old = f.Parent;
		f.F.Actor.Setup(x => x.RemoveAllEffects<SpellWaterBreathingEffect>(It.IsAny<Predicate<SpellWaterBreathingEffect>>(), It.IsAny<bool>()))
			.Returns<Predicate<SpellWaterBreathingEffect>, bool>((predicate, fire) => f.Handler.RemoveAllEffects(predicate, fire));
		var remove = SpellEffectFactory.LoadEffect(XElement.Parse("<Effect type='removewaterbreathing'/>"), f.Spell);
		remove.GetOrApplyEffect(f.F.Actor.Object, f.F.Actor.Object, OpposedOutcomeDegree.None, SpellPower.Standard,
			new MagicSpellParent(f.F.Actor.Object, f.Spell, f.F.Actor.Object), []);
		Assert.IsFalse(f.Handler.Effects.OfType<SpellScopedWaterBreathingEffect>().Any());
		Assert.IsFalse(f.Handler.Effects.Contains(old)); Assert.IsFalse(f.Scheduler.IsScheduled(old)); Assert.IsFalse(old.SpellEffects.Any());
		Assert.AreEqual(65.0, f.F.Balances[f.F.Resources[1]]); Assert.IsTrue(f.Applied(result));
	}

	[DataTestMethod, DataRow("BeforePayment"), DataRow("Committed")]
	public void CallbackCohortDrift_RefusesBeforeDebitOrQuarantinesPaidUncertainty(string stage)
	{
		using var f = new Fixture(); using var random = Constants.PushRandom(f.Random); var old = f.Seed(3000, 4, SpellPower.Weak);
		f.F.Checkpoint = current => { if (current == stage) f.Scheduler.Reschedule(old, TimeSpan.FromSeconds(3600)); };
		var result = f.Cast(1);
		Assert.AreEqual(stage == "BeforePayment" ? MagicCastingStatus.Refused : MagicCastingStatus.NeedsReview, result.Status, result.Message);
		Assert.AreSame(old, f.Parent); Assert.AreEqual(1, f.Random.Draws); Assert.AreEqual(0, f.F.Samples);
		Assert.AreEqual(stage == "BeforePayment" ? 100.0 : 95.0, f.F.Balances[f.F.Resources[1]]);
		if (stage == "BeforePayment") Assert.IsNull(result.OperationId);
		else Assert.IsFalse(f.Applied(result));
	}

	[TestMethod]
	public void PreparedDurationCallback_CannotRecaptureChangedCohortOrRedraw()
	{
		using var f = new Fixture(); using var random = Constants.PushRandom(f.Random); var old = f.Seed(3000, 4, SpellPower.Weak);
		var copy = f.Copy(1); copy.EffectDurationExpression = new CallbackExpression(f.F.World.Object,
			() => f.Scheduler.Reschedule(old, TimeSpan.FromSeconds(3600)));
		Assert.ThrowsException<InvalidOperationException>(() => copy.ResolveTriggeredSpell(f.F.Actor.Object, f.F.Actor.Object, SpellPower.ExtremelyWeak));
		Assert.AreSame(old, f.Parent); Assert.AreEqual(1, f.Random.Draws); Assert.AreEqual(100.0, f.F.Balances[f.F.Resources[1]]);
	}

	[DataTestMethod, DataRow("missing"), DataRow("unavailable"), DataRow("scope"), DataRow("permanent"), DataRow("mixed"), DataRow("formula"), DataRow("childprog")]
	public void InvalidConfigurationOrCohort_RefusesBeforePayment(string scenario)
	{
		using var f = new Fixture(); using var random = Constants.PushRandom(f.Random);
		if (scenario == "missing") Assert.IsTrue(f.Spell.BuildingCommand(f.F.Actor.Object, new StringStack("effect 1 water remove 51")));
		if (scenario == "unavailable") f.Liquids.Clear();
		if (scenario == "scope") f.Seed(3000, 1, SpellPower.Weak, new([f.OtherWater]));
		if (scenario == "permanent") { var parent = f.Seed(3000, 1, SpellPower.Weak); f.Scheduler.Unschedule(parent); }
		if (scenario == "mixed") { var parent = f.Seed(3000, 1, SpellPower.Weak); var child = new SpellDetectInvisibleEffect(f.F.Actor.Object, parent); parent.AddSpellEffect(child); f.Handler.AddEffect(child); }
		if (scenario == "formula") f.Spell.EffectDurationExpression = new TraitExpression("grade*600", f.F.World.Object);
		if (scenario == "childprog") { f.Seed(3000, 1, SpellPower.Weak); f.Child.ApplicabilityProg = Mock.Of<MudSharp.FutureProg.IFutureProg>(); }
		var result = f.Cast(1); Assert.AreEqual(MagicCastingStatus.Refused, result.Status, result.Message);
		Assert.IsNull(result.OperationId); Assert.AreEqual(100.0, f.F.Balances[f.F.Resources[1]]); Assert.AreEqual(0, f.Random.Draws);
	}

	[TestMethod]
	public void MappingReplacement_RefusesSealedSelectionWithoutRedraw()
	{
		using var f = new Fixture(); using var random = Constants.PushRandom(f.Random);
		f.F.Checkpoint = stage => { if (stage == "BeforePayment") { f.Liquids[0] = Mock.Of<ILiquid>(x => x.Id == 51); } };
		var result = f.Cast(1); Assert.AreEqual(MagicCastingStatus.Refused, result.Status, result.Message);
		Assert.AreEqual(1, f.Random.Draws); Assert.AreEqual(100.0, f.F.Balances[f.F.Resources[1]]); Assert.IsFalse(f.Handler.Effects.Any());
	}

	[TestMethod]
	public void ChildParentReload_PreservesMappedScopeGradeDeadlineAndExpiresWithoutRefund()
	{
		using var f = new Fixture(); using var random = Constants.PushRandom(f.Random); var result = f.Cast(7); var parent = f.Parent;
		var duration = f.Scheduler.RemainingDuration(parent); var deadline = f.Scheduler.ScheduledExpiry(parent);
		var xml = parent.SaveToXml(new Dictionary<IEffect, TimeSpan> { [parent] = duration }); f.Handler.RemoveEffect(parent, true);
		SpellScopedWaterBreathingEffect.InitialiseEffectType();
		var restored = (MagicSpellParent)typeof(MagicSpellParent).GetConstructor(BindingFlags.Instance | BindingFlags.NonPublic,
			null, [typeof(XElement), typeof(IPerceivable)], null)!.Invoke([xml, f.F.Actor.Object]);
		f.Handler.AddEffect(restored, duration);
		Assert.AreEqual(parent.Identity, restored.Identity); Assert.AreEqual(7, restored.LifetimeState!.Grade);
		Assert.AreEqual(deadline, f.Scheduler.ScheduledExpiry(restored)); Assert.IsTrue(f.Child.AppliesToFluid(f.Water));
		Assert.IsFalse(f.Child.AppliesToFluid(f.OtherWater)); Assert.AreEqual(1, f.Random.Draws);
		f.Clock.Advance(duration + TimeSpan.FromSeconds(1)); f.Scheduler.CheckSchedules();
		Assert.IsFalse(f.Handler.Effects.OfType<SpellScopedWaterBreathingEffect>().Any()); Assert.IsFalse(f.Handler.Effects.OfType<MagicSpellParent>().Any());
		Assert.AreEqual(65.0, f.F.Balances[f.F.Resources[1]]); Assert.IsTrue(f.Applied(result));
	}

	[TestMethod]
	public void OrdinaryBuilder_CloneReloadAndInvalidScopePreservation()
	{
		using var f = new Fixture();
		Assert.IsTrue(f.Spell.BuildingCommand(f.F.Actor.Object, new StringStack("effect 1 water add 52")));
		Assert.IsTrue(f.Spell.BuildingCommand(f.F.Actor.Object, new StringStack("effect 1 lifetime custom.water 300 72")));
		var template = f.Spell.SpellEffects.Single(); var clone = template.Clone();
		Assert.IsTrue(XNode.DeepEquals(template.SaveToXml(), clone.SaveToXml()));
		Assert.AreEqual(new MagicSpellLifetimePolicy("custom.water", 300, 72), ((IMagicSpellEffectLifetimePolicy)clone).LifetimePolicy);
		var xml = template.SaveToXml(); xml.Element("WaterScope")!.SetAttributeValue("version", 99);
		var malformed = SpellEffectFactory.LoadEffect(xml, f.Spell);
		Assert.IsTrue(XNode.DeepEquals(xml, malformed.SaveToXml()));
		((List<IMagicSpellEffectTemplate>)f.Spell.SpellEffects)[0] = malformed;
		Assert.AreEqual(MagicCastingStatus.Refused, f.Cast(1).Status); Assert.AreEqual(100.0, f.F.Balances[f.F.Resources[1]]);
	}

	private sealed class CallbackExpression(IFuturemud world, Action callback) : TraitExpression("0", world)
	{
		public override double EvaluateWith(IHaveTraits owner, ITraitDefinition variable = null!, TraitBonusContext context = TraitBonusContext.None,
			params (string Name, object Value)[] values) { callback(); return 0; }
	}
	private sealed class Clock(DateTime now) : TimeProvider
	{
		public DateTime Now { get; private set; } = now;
		public override DateTimeOffset GetUtcNow() => new(Now);
		public void Advance(TimeSpan duration) => Now += duration;
	}
	private sealed class CountingRandom : Random
	{
		public int Draws; public int Value; public (int, int) Bounds;
		public override int Next(int minValue, int maxValue) { Draws++; Bounds = (minValue, maxValue); return Math.Clamp(Value, minValue, maxValue - 1); }
	}
	private sealed class Fixture : IDisposable
	{
		public MagicCastingFixture F { get; } = new();
		public MagicSpell Spell { get; }
		public Clock Clock { get; }
		public CountingRandom Random { get; } = new();
		public EffectScheduler Scheduler { get; }
		public EffectHandler Handler { get; }
		public ILiquid Water { get; } = Mock.Of<ILiquid>(x => x.Id == 51 && x.Name == "water");
		public ILiquid OtherWater { get; } = Mock.Of<ILiquid>(x => x.Id == 52 && x.Name == "saltwater");
		public List<ILiquid> Liquids { get; }
		public MagicSpellParent Parent => Handler.Effects.OfType<MagicSpellParent>().Single();
		public SpellScopedWaterBreathingEffect Child => Handler.Effects.OfType<SpellScopedWaterBreathingEffect>().Single();
		private readonly IDisposable _time;
		public Fixture()
		{
			Liquids = [Water, OtherWater]; F.World.SetupGet(x => x.Liquids).Returns(MagicCastingFixture.Collection(() => Liquids));
			Clock = new(F.Now); _time = RuntimeClock.Push(Clock); Scheduler = new(F.World.Object, Clock);
			F.World.SetupGet(x => x.EffectScheduler).Returns(Scheduler); Handler = new(F.Actor.Object);
			F.Actor.SetupGet(x => x.Effects).Returns(() => Handler.Effects);
			F.Actor.Setup(x => x.AddEffect(It.IsAny<IEffect>())).Callback<IEffect>(Handler.AddEffect);
			F.Actor.Setup(x => x.AddEffect(It.IsAny<IEffect>(), It.IsAny<TimeSpan>())).Callback<IEffect, TimeSpan>(Handler.AddEffect);
			F.Actor.Setup(x => x.RemoveEffect(It.IsAny<IEffect>(), It.IsAny<bool>())).Callback<IEffect, bool>(Handler.RemoveEffect);
			F.Actor.Setup(x => x.RemoveAllEffects<MagicSpellParent>(It.IsAny<Predicate<MagicSpellParent>>(), It.IsAny<bool>()))
				.Returns<Predicate<MagicSpellParent>, bool>((predicate, fire) => Handler.RemoveAllEffects(predicate, fire));
			SourceWaterBreathingEffect.RegisterFactory(); F.Spells.Remove(F.Spell); Spell = F.NewSpell(1, "Water adapter", Definition);
			Assert.IsTrue(Spell.BuildingCommand(F.Actor.Object, new StringStack("grades fixture")));
			Assert.IsTrue(Spell.BuildingCommand(F.Actor.Object, new StringStack("exclusiveeffect")));
			Spell.EffectDurationExpression = new TraitExpression("0", F.World.Object); F.Skills[1] = 100; F.Acquire(7);
		}
		public MagicSpellParent Seed(double seconds, int grade, SpellPower power, MudSharp.Magic.WaterBreathing.WaterBreathingFluidScope? scope = null)
		{
			var parent = new MagicSpellParent(F.Actor.Object, Spell, F.Actor.Object, power) { LifetimeState = new(new(Group, 600, 36), grade) };
			var child = new SpellScopedWaterBreathingEffect(F.Actor.Object, parent, scope ?? new([Water]));
			parent.AddSpellEffect(child); Handler.AddEffect(child); Handler.AddEffect(parent, TimeSpan.FromSeconds(seconds)); return parent;
		}
		public MagicCastingResult Cast(int grade, bool overreach = false)
		{ F.Balances[F.Resources[1]] = 100; F.Actor.Object.RemoveAllEffects<MagicSpellLockout>(null, true); return F.Service.Cast(F.Intent(grade, overreach)); }
		public bool Applied(MagicCastingResult result) => (bool?)XElement.Parse(F.Store.Operations[result.OperationId!.Value].Definition).Attribute("applied") == true;
		public MagicSpell Copy(int grade) => Spell.CastingCopy(F.Actor.Object, F.Traits[0], grade, Spell.GradeProfile!.Grades.Single(x => x.Grade == grade).Power, Difficulty.Easy, 7);
		public void Dispose() => _time.Dispose();
	}
}
