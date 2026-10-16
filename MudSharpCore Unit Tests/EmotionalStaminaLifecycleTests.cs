#nullable enable

using System;
using System.Linq;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Body;
using MudSharp.Body.Traits;
using MudSharp.Character;
using MudSharp.Effects;
using MudSharp.Effects.Concrete;
using MudSharp.Effects.Concrete.SpellEffects;
using MudSharp.Effects.Interfaces;
using MudSharp.Framework;
using MudSharp.Framework.Scheduling;
using MudSharp.FutureProg;
using MudSharp.Magic;
using MudSharp.Magic.Emotions;
using MudSharp.Magic.SpellEffects;
using MudSharp.RPG.Checks;
using ConcreteBody = MudSharp.Body.Implementations.Body;

namespace MudSharp_Unit_Tests;

[TestClass]
[DoNotParallelize]
public class EmotionalStaminaLifecycleTests
{
	[DataTestMethod]
	[DataRow(false, false)]
	[DataRow(true, false)]
	[DataRow(false, true)]
	[DataRow(true, true)]
	public void Fury_NativeParentExpiryOrDispel_ClampsCapacityAndRetainsOtherFury(bool dispel, bool retainOther)
	{
		var f = new MagicCastingFixture();
		var clock = new LifecycleClock(f.Now);
		using var time = RuntimeClock.Push(clock);
		var scheduler = new EffectScheduler(f.World.Object, clock);
		f.World.SetupGet(x => x.EffectScheduler).Returns(scheduler);
		var body = TestObjectFactory.CreateUninitialized<ConcreteBody>();
		body.Actor = f.Actor.Object;
		typeof(ConcreteBody).GetProperty(nameof(body.Gameworld))!.SetValue(body, f.World.Object);
		typeof(PerceivedItem).GetProperty(nameof(body.EffectHandler))!.SetValue(body, new EffectHandler(body));
		foreach (var name in new[] { "_traits", "_merits", "_implants", "_externalItems" })
		{
			var field = typeof(ConcreteBody).GetField(name, BindingFlags.Instance | BindingFlags.NonPublic)!;
			field.SetValue(body, Activator.CreateInstance(field.FieldType));
		}
		f.Actor.SetupGet(x => x.Body).Returns(body);
		f.Actor.SetupGet(x => x.Merits).Returns([]);
		f.Actor.Setup(x => x.CombinedEffectsOfType<PsychicSuppressionEffect>()).Returns([]);
		var handler = new EffectHandler(f.Actor.Object);
		f.Actor.SetupGet(x => x.Effects).Returns(() => handler.Effects);
		f.Actor.Setup(x => x.EffectsOfType<ITraitBonusEffect>(null)).Returns(() => handler.EffectsOfType<ITraitBonusEffect>());
		f.Actor.Setup(x => x.EffectsOfType<MagicSpellParent>(It.IsAny<Predicate<MagicSpellParent>>()))
			.Returns<Predicate<MagicSpellParent>>(predicate => handler.EffectsOfType(predicate));
		f.Actor.Setup(x => x.EffectsOfType<IDispelMagicProxyEffect>(null)).Returns([]);
		f.Actor.Setup(x => x.RemoveEffect(It.IsAny<IEffect>(), It.IsAny<bool>())).Callback<IEffect, bool>(handler.RemoveEffect);
		var definition = new Mock<ITraitDefinition>();
		definition.SetupGet(x => x.Id).Returns(844);
		definition.SetupGet(x => x.TraitType).Returns(TraitType.Attribute);
		definition.SetupGet(x => x.OwnerScope).Returns(TraitOwnerScope.Body);
		var trait = new Mock<ITrait>(); trait.SetupGet(x => x.Definition).Returns(definition.Object);
		trait.SetupGet(x => x.Value).Returns(10);
		((System.Collections.Generic.List<ITrait>)typeof(ConcreteBody).GetField("_traits", BindingFlags.Instance | BindingFlags.NonPublic)!.GetValue(body)!).Add(trait.Object);
		var capacity = new Mock<IFutureProg>();
		capacity.Setup(x => x.Execute(It.IsAny<object[]>())).Returns(() => (object)(decimal)(100 + body.TraitValue(definition.Object) * 10));
		var staminaProg = typeof(MudSharp.Character.Character).GetField("MaximumStaminaProg", BindingFlags.Static | BindingFlags.NonPublic)!;
		var oldProg = staminaProg.GetValue(null);
		staminaProg.SetValue(null, capacity.Object);
		try
		{
			body.MaximumStamina = 200; body.CurrentStamina = 150;
			SetBodyField(body, "_currentExertion", ExertionLevel.Heavy);
			SetBodyField(body, "_longtermExertion", ExertionLevel.Normal);
			SetBodyField(body, "_tenSecondStaminaActive", true);
			SetBodyField(body, "_minuteStaminaActive", true);
			var departing = Attach(f.Actor.Object, 4, TimeSpan.FromSeconds(1));
			Assert.AreEqual(240.0, body.MaximumStamina); Assert.AreEqual(150.0, body.CurrentStamina, "Attach must not refill stamina.");
			MagicSpellParent? other = null;
			if (retainOther)
			{
				var caster = new Mock<ICharacter>(); caster.SetupGet(x => x.Id).Returns(101); caster.SetupGet(x => x.InstanceId).Returns(101);
				other = Attach(caster.Object, 2, TimeSpan.FromSeconds(100));
				Assert.AreEqual(260.0, body.MaximumStamina); Assert.AreEqual(150.0, body.CurrentStamina);
			}
			body.CurrentStamina = body.MaximumStamina - 1;
			if (dispel)
			{
				var effect = f.NewSpell(844, "Explicit Fury lifecycle dispel", "<Effect type='dispelmagic'><Mode>0</Mode><CasterPolicy>0</CasterPolicy><EffectKey>any</EffectKey></Effect>").SpellEffects.OfType<DispelMagicEffect>().Single();
				Assert.AreEqual(MagicEffectOperationStatus.Applied, effect.Apply(f.Actor.Object, f.Actor.Object, OpposedOutcomeDegree.None, SpellPower.ExtremelyStrong, null!, []).Status);
			}
			else { clock.Advance(TimeSpan.FromSeconds(2)); scheduler.CheckSchedules(); Assert.AreEqual(1, scheduler.LastCheckFiredCount); }
			Assert.IsFalse(handler.Effects.Contains(departing)); Assert.IsFalse(scheduler.IsScheduled(departing));
			Assert.IsFalse(handler.Effects.OfType<SpellSourceFuryEffect>().Any(x => ReferenceEquals(x.ParentEffect, departing)));
			Assert.AreEqual(retainOther ? 220.0 : 200.0, body.MaximumStamina, "Departing child must be excluded during the native pre-detach callback.");
			Assert.AreEqual(body.MaximumStamina, body.CurrentStamina, "Removal must clamp excess current stamina.");
			Assert.AreEqual(retainOther ? 12.0 : 10.0, body.TraitValue(definition.Object));
			if (other is not null) { Assert.IsTrue(handler.Effects.Contains(other)); Assert.IsTrue(scheduler.IsScheduled(other)); }
			Assert.AreEqual(ExertionLevel.Heavy, body.CurrentExertion); Assert.AreEqual(ExertionLevel.Normal, body.LongtermExertion);
			Assert.AreEqual(true, GetBodyField(body, "_tenSecondStaminaActive")); Assert.AreEqual(true, GetBodyField(body, "_minuteStaminaActive"));
		}
		finally { staminaProg.SetValue(null, oldProg); }

		MagicSpellParent Attach(ICharacter caster, double points, TimeSpan duration)
		{
			var parent = new MagicSpellParent(f.Actor.Object, f.Spell, caster);
			var child = new SpellSourceFuryEffect(f.Actor.Object, parent, "test.explicit.fury", 600, 36,
				new EmotionalRetainedState(7, SpellPower.Standard, 6, points), definition.Object, 1);
			parent.AddSpellEffect(child); handler.AddEffect(child); handler.AddEffect(parent, duration);
			return parent;
		}
	}

	private static void SetBodyField(ConcreteBody body, string name, object value) =>
		typeof(ConcreteBody).GetField(name, BindingFlags.Instance | BindingFlags.NonPublic)!.SetValue(body, value);
	private static object? GetBodyField(ConcreteBody body, string name) =>
		typeof(ConcreteBody).GetField(name, BindingFlags.Instance | BindingFlags.NonPublic)!.GetValue(body);
	private sealed class LifecycleClock(DateTime now) : TimeProvider
	{
		private DateTime _now = now;
		public override DateTimeOffset GetUtcNow() => new(_now);
		public void Advance(TimeSpan duration) => _now += duration;
	}
}
