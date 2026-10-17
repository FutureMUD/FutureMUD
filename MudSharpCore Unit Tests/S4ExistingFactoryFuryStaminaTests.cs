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
public class S4ExistingFactoryFuryStaminaTests
{
	[DataTestMethod]
	[DataRow(1, 7)] [DataRow(7, 1)]
	public void CanonicalFuryInPlaceUpdate_StrongerOrWeaker_ConservesStaminaAndReloadsGrade(int oldGrade, int newGrade)
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
		f.Traits.Add(definition.Object);
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
			var parent = new MagicSpellParent(f.Actor.Object, f.Spell, f.Actor.Object, SpellPower.ExtremelyWeak);
			var child = new SpellSourceFuryEffect(f.Actor.Object, parent, "armageddon.roused_fury", 60, 7,
				new(oldGrade, SpellPower.ExtremelyWeak, 5, oldGrade), definition.Object, 1);
			parent.AddSpellEffect(child); handler.AddEffect(child); handler.AddEffect(parent, TimeSpan.FromSeconds(oldGrade * 60));
			body.CurrentStamina = body.MaximumStamina - 1;
			var before = body.CurrentStamina;
			child.UpdateCanonicalState(new(newGrade, SpellPower.ExtremelyWeak, 5, newGrade));
			Assert.AreEqual(200.0 + newGrade * 10, body.MaximumStamina);
			Assert.AreEqual(Math.Min(before, body.MaximumStamina), body.CurrentStamina);
			Assert.AreSame(child, parent.SpellEffects.Single()); Assert.IsTrue(handler.Effects.Contains(child));
			Assert.AreEqual(1, handler.Effects.OfType<SpellSourceFuryEffect>().Count());
			SpellSourceFuryEffect.InitialiseEffectType();
			var loaded = (SpellSourceFuryEffect)Effect.LoadEffect(child.SaveToXml(new System.Collections.Generic.Dictionary<IEffect, TimeSpan>()), f.Actor.Object);
			Assert.IsNull(loaded.DefinitionError); Assert.AreEqual(child.State, loaded.State);
			Assert.AreEqual((double)newGrade, loaded.GetBonus(trait.Object));
		}
		finally { staminaProg.SetValue(null, oldProg); }
	}
	private sealed class LifecycleClock(DateTime now) : TimeProvider
	{
		public override DateTimeOffset GetUtcNow() => new(now);
	}
}
