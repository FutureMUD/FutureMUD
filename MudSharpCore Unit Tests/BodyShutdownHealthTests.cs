#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Body;
using MudSharp.Body.PartProtos;
using MudSharp.Character;
using MudSharp.Character.Heritage;
using MudSharp.Effects;
using MudSharp.Effects.Concrete;
using MudSharp.Form.Shape;
using MudSharp.Framework;
using MudSharp.Framework.Scheduling;
using MudSharp.Health;
using ConcreteBody = MudSharp.Body.Implementations.Body;

namespace MudSharp_Unit_Tests;

[TestClass]
public class BodyShutdownHealthTests
{
	[DataTestMethod]
	[DataRow(false)]
	[DataRow(true)]
	public void Shutdown_ColdDeadSpinalBody_DoesNotManufactureEffectsAndUnregistersOnce(bool archival)
	{
		var f = BuildBody();
		var ten = (HeartbeatManagerDelegate)Delegate.CreateDelegate(typeof(HeartbeatManagerDelegate), f.Body,
			typeof(ConcreteBody).GetMethod("HealthTick_TenSecondHeartbeat", BindingFlags.Instance | BindingFlags.NonPublic)!);
		var minute = (HeartbeatManagerDelegate)Delegate.CreateDelegate(typeof(HeartbeatManagerDelegate), f.Body,
			typeof(ConcreteBody).GetMethod("HealingTick_MinuteHeartbeat", BindingFlags.Instance | BindingFlags.NonPublic)!);
		f.Heartbeats.Object.TenSecondHeartbeat += ten;
		f.Heartbeats.Object.MinuteHeartbeat += minute;
		SetField(f.Body, "_healthTickActive", true);
		Assert.AreEqual(0.0, f.Body.OrganFunction(f.Spine));
		Assert.IsFalse(f.Body.Effects.Any());

		Shutdown();
		Shutdown();

		Assert.IsFalse(f.Body.Effects.Any(), "Unloading a cold dead body must not reevaluate its uninitialized organ cache.");
		f.Heartbeats.VerifyRemove(x => x.TenSecondHeartbeat -= ten, Times.Once);
		f.Heartbeats.VerifyRemove(x => x.MinuteHeartbeat -= minute, Times.Once);
		f.Heartbeats.Raise(x => x.TenSecondHeartbeat += null);
		f.Heartbeats.Raise(x => x.MinuteHeartbeat += null);
		Assert.IsFalse(f.Body.Effects.Any());

		void Shutdown()
		{
			if (archival)
				typeof(ConcreteBody).GetMethod("ReleaseArchivedRuntime", BindingFlags.Instance | BindingFlags.NonPublic)!.Invoke(f.Body, null);
			else
				f.Body.Quit();
		}
	}

	[TestMethod]
	public void EndHealthTick_ExplicitHealthProcessing_StillCreatesAndRemovesDerivedSpinalEffects()
	{
		var f = BuildBody();
		f.Body.EndHealthTick();
		var effect = f.Body.Effects.OfType<LimbSpinalDamageEffect>().Single();
		Assert.AreSame(f.Limb, effect.Limb);
		Assert.IsFalse(effect.SavingEffect);
		f.Body.EndHealthTick();
		Assert.AreEqual(1, f.Body.Effects.Count());
		((DoubleCounter<IOrganProto>)GetField(f.Body, "_cachedOrganFunctionsByOrgan").GetValue(f.Body)!)[f.Spine] = 1.0;
		f.Body.EndHealthTick();
		Assert.IsFalse(f.Body.Effects.Any());
	}

	[TestMethod]
	public void Quit_ExistingRuntimeEffect_IsRetainedForArchivalGuard()
	{
		var f = BuildBody();
		f.Body.EndHealthTick();
		var effect = f.Body.Effects.Single();
		f.Body.Quit();
		Assert.AreSame(effect, f.Body.Effects.Single(), "Shutdown must not discard existing effects to permit archival.");
	}

	private static (ConcreteBody Body, ISpineProto Spine, ILimb Limb, Mock<IHeartbeatManager> Heartbeats) BuildBody()
	{
		var body = TestObjectFactory.CreateUninitialized<ConcreteBody>();
		var world = new Mock<IFuturemud> { DefaultValue = DefaultValue.Mock };
		var heartbeats = new Mock<IHeartbeatManager>();
		world.SetupGet(x => x.HeartbeatManager).Returns(heartbeats.Object);
		typeof(ConcreteBody).GetProperty(nameof(body.Gameworld))!.SetValue(body, world.Object);
		typeof(PerceivedItem).GetProperty(nameof(body.EffectHandler))!.SetValue(body, new EffectHandler(body));
		foreach (var name in new[] { "_merits", "_limbs", "_wounds", "_bodyparts", "_bones", "_organs", "_allItems",
			"_cachedEffects", "_cachedOrganFunctionsByOrgan", "_implants", "_prosthetics", "_externalItems", "_heldItems", "_wieldedItems" })
		{
			var field = GetField(body, name);
			field.SetValue(body, Activator.CreateInstance(field.FieldType));
		}
		var actor = new Mock<ICharacter>();
		actor.SetupGet(x => x.State).Returns(CharacterState.Dead);
		actor.SetupGet(x => x.CurrentBody).Returns(body);
		actor.SetupGet(x => x.Body).Returns(body);
		body.Actor = actor.Object;
		var race = new Mock<IRace>();
		race.SetupGet(x => x.DamageToleranceModifier).Returns(1.0);
		var prototype = new Mock<IBodyPrototype>();
		prototype.Setup(x => x.BodypartsFor(race.Object, Gender.Male)).Returns([]);
		typeof(ConcreteBody).GetProperty(nameof(body.Race))!.SetValue(body, race.Object);
		typeof(ConcreteBody).GetProperty(nameof(body.Prototype))!.SetValue(body, prototype.Object);
		typeof(ConcreteBody).GetProperty(nameof(body.Gender), BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly)!
			.SetValue(body, Gendering.Get(Gender.Male));
		var health = new Mock<IHealthStrategy>();
		health.SetupGet(x => x.RequiresSpinalCord).Returns(true);
		health.Setup(x => x.MaxHP(actor.Object)).Returns(100);
		health.Setup(x => x.MaxPain(actor.Object)).Returns(100);
		SetField(body, "_healthStrategy", health.Object);
		var spine = new Mock<ISpineProto>();
		var limb = new Mock<ILimb>();
		limb.SetupGet(x => x.Parts).Returns([]);
		limb.SetupGet(x => x.SpineProtos).Returns([spine.Object]);
		limb.SetupGet(x => x.LimbType).Returns(LimbType.Arm);
		limb.SetupGet(x => x.LimbDamageThresholdMultiplier).Returns(1);
		limb.SetupGet(x => x.LimbPainThresholdMultiplier).Returns(1);
		((List<ILimb>)GetField(body, "_limbs").GetValue(body)!).Add(limb.Object);
		return (body, spine.Object, limb.Object, heartbeats);
	}

	private static void SetField(ConcreteBody body, string name, object value) => GetField(body, name).SetValue(body, value);

	private static FieldInfo GetField(ConcreteBody body, string name)
	{
		for (var type = body.GetType(); type is not null; type = type.BaseType)
		{
			if (type.GetField(name, BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.DeclaredOnly) is { } field)
				return field;
		}
		throw new InvalidOperationException($"Missing fixture field {name}.");
	}
}
