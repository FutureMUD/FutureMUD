#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Construction;
using MudSharp.Effects;
using MudSharp.Effects.Concrete;
using MudSharp.Effects.Concrete.SpellEffects;
using MudSharp.Effects.Interfaces;
using MudSharp.Framework;
using MudSharp.Framework.Scheduling;
using MudSharp.Magic;
using MudSharp.RPG.Checks;

namespace MudSharp_Unit_Tests;

[TestClass]
public class ArmageddonRoomLightPersistenceTests
{
	[TestMethod]
	[DataRow(20.0)]
	[DataRow(-140.0)]
	public void RoomLight_ChildAndParentNativeReloadRetainAreaLightAndRemainingLifetime(double lux)
	{
		var f = new MagicCastingFixture();
		var clock = new ReloadClock(f.Now);
		using var time = RuntimeClock.Push(clock);
		var scheduler = new EffectScheduler(f.World.Object, clock);
		f.World.SetupGet(x => x.EffectScheduler).Returns(scheduler);
		var room = new Mock<IRoom>(); room.SetupGet(x => x.Gameworld).Returns(f.World.Object);
		var effects = new List<IEffect>();
		room.SetupGet(x => x.Effects).Returns(() => effects);
		room.Setup(x => x.AddEffect(It.IsAny<IEffect>())).Callback<IEffect>(effects.Add);
		room.Setup(x => x.RemoveEffect(It.IsAny<IEffect>(), It.IsAny<bool>())).Callback<IEffect, bool>((effect, fire) =>
		{
			if (fire) effect.RemovalEffect();
			effects.Remove(effect);
			scheduler.Unschedule(effect);
		});
		MagicSpellParent.InitialiseEffectType();
		SpellRoomLightEffect.InitialiseEffectType();
		SpellGlowEffect.InitialiseEffectType();
		var parent = new MagicSpellParent(room.Object, f.Spell, f.Actor.Object, SpellPower.ExtremelyWeak);
		var child = new SpellRoomLightEffect(room.Object, parent, null!, lux, "", Telnet.BoldWhite);
		parent.AddSpellEffect(child);
		scheduler.AddSchedule(new EffectSchedule(parent, TimeSpan.FromSeconds(60)));
		clock.Advance(TimeSpan.FromSeconds(15));
		var remaining = scheduler.RemainingDuration(parent);
		Assert.AreEqual(TimeSpan.FromSeconds(45), remaining);
		var saved = parent.SaveToXml(new Dictionary<IEffect, TimeSpan> { [parent] = remaining });
		var childXml = saved.Element("Effect")!.Element("Children")!.Elements().Single();
		Assert.AreEqual("SpellRoomLight", childXml.Element("Type")!.Value);
		var reloadedChild = Effect.LoadEffect(childXml, room.Object);
		Assert.IsInstanceOfType(reloadedChild, typeof(IAreaLightEffect));
		Assert.AreEqual(lux, ((IAreaLightEffect)reloadedChild).AddedLight);
		scheduler.Unschedule(parent);
		var loadedParent = (MagicSpellParent)Effect.LoadEffect(saved, room.Object);
		var loadedChild = loadedParent.SpellEffects.Single();
		Assert.IsInstanceOfType(loadedChild, typeof(IAreaLightEffect));
		Assert.AreSame(loadedParent, loadedChild.ParentEffect);
		Assert.AreEqual(lux, ((IAreaLightEffect)loadedChild).AddedLight);
		effects.Add(loadedParent);
		scheduler.AddSchedule(new EffectSchedule(loadedParent, TimeSpan.FromMilliseconds((long)saved.Element("Remaining")!)));
		Assert.AreEqual(TimeSpan.FromSeconds(45), scheduler.RemainingDuration(loadedParent));
		clock.Advance(TimeSpan.FromSeconds(44)); scheduler.CheckSchedules();
		Assert.IsTrue(effects.Contains(loadedParent));
		Assert.IsTrue(effects.Contains(loadedChild));
		clock.Advance(TimeSpan.FromSeconds(1)); scheduler.CheckSchedules();
		Assert.IsFalse(effects.Contains(loadedParent));
		Assert.IsFalse(effects.Contains(loadedChild));
		var historical = new XElement(childXml); historical.Element("Type")!.Value = "SpellGlow";
		Assert.IsNull(historical.Element("Effect")!.Element("SDescAddendum"));
		Assert.ThrowsExactly<NullReferenceException>(() => Effect.LoadEffect(historical, room.Object),
			"Historical mislabelled rows are not reinterpreted as part of this correction.");
	}

	private sealed class ReloadClock(DateTime now) : TimeProvider
	{
		private DateTime _now = now;
		public override DateTimeOffset GetUtcNow() => new(_now);
		public void Advance(TimeSpan duration) => _now += duration;
	}
}
