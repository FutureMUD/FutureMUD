#nullable enable

using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Character;
using MudSharp.Events;
using MudSharp.Events.Hooks;
using MudSharp.Framework;
using MudSharp.Framework.Scheduling;
using MudSharp.NPC;
using MudSharp.NPC.AI;
using NativeCharacter = MudSharp.Character.Character;
using NativeNpc = MudSharp.NPC.NPC;

namespace MudSharp_Unit_Tests;

[TestClass]
public class NpcTickHookLifecycleTests
{
	[TestMethod]
	[DataRow("primary")]
	[DataRow("secondary")]
	[DataRow("scripted")]
	public void HookOnlyNpc_ResumesWithoutDuplicatesAndReleasesEveryTickOnInactivity(string kind)
	{
		NativeCharacter npc = kind switch
		{
			"primary" => TestObjectFactory.CreateUninitialized<NativeNpc>(),
			"secondary" => TestObjectFactory.CreateUninitialized<NpcCharacterInstance>(),
			_ => TestObjectFactory.CreateUninitialized<ScriptedAiCharacterInstance>()
		};
		var heartbeat = new Mock<IHeartbeatManager>(); var subscriptions = new HashSet<HeartbeatManagerDelegate>();
		heartbeat.SetupAdd(x => x.FuzzyFiveSecondHeartbeat += It.IsAny<HeartbeatManagerDelegate>()).Callback<HeartbeatManagerDelegate>(handler => { subscriptions.Add(handler); });
		heartbeat.SetupAdd(x => x.FuzzyTenSecondHeartbeat += It.IsAny<HeartbeatManagerDelegate>()).Callback<HeartbeatManagerDelegate>(handler => { subscriptions.Add(handler); });
		heartbeat.SetupAdd(x => x.FuzzyMinuteHeartbeat += It.IsAny<HeartbeatManagerDelegate>()).Callback<HeartbeatManagerDelegate>(handler => { subscriptions.Add(handler); });
		heartbeat.SetupAdd(x => x.FuzzyHourHeartbeat += It.IsAny<HeartbeatManagerDelegate>()).Callback<HeartbeatManagerDelegate>(handler => { subscriptions.Add(handler); });
		heartbeat.SetupRemove(x => x.FuzzyFiveSecondHeartbeat -= It.IsAny<HeartbeatManagerDelegate>()).Callback<HeartbeatManagerDelegate>(handler => { subscriptions.Remove(handler); });
		heartbeat.SetupRemove(x => x.FuzzyTenSecondHeartbeat -= It.IsAny<HeartbeatManagerDelegate>()).Callback<HeartbeatManagerDelegate>(handler => { subscriptions.Remove(handler); });
		heartbeat.SetupRemove(x => x.FuzzyMinuteHeartbeat -= It.IsAny<HeartbeatManagerDelegate>()).Callback<HeartbeatManagerDelegate>(handler => { subscriptions.Remove(handler); });
		heartbeat.SetupRemove(x => x.FuzzyHourHeartbeat -= It.IsAny<HeartbeatManagerDelegate>()).Callback<HeartbeatManagerDelegate>(handler => { subscriptions.Remove(handler); });
		var world = new Mock<IFuturemud>(); world.SetupGet(x => x.HeartbeatManager).Returns(heartbeat.Object);
		typeof(NativeCharacter).GetProperty("Gameworld")!.SetValue(npc, world.Object);
		Set(npc, "_AIs", new List<IArtificialIntelligence>());
		Set(npc, "_state", CharacterState.Awake);
		var hooks = new List<IHook>();
		foreach (var tick in new[] { EventType.FiveSecondTick, EventType.TenSecondTick, EventType.MinuteTick, EventType.HourTick })
			hooks.Add(Mock.Of<IHook>(x => x.Type == tick));
		Set(npc, "_installedHooks", hooks);
		void Invoke(string name) => npc.GetType().GetMethod(name)!.Invoke(npc, null);
		Invoke("SetupEventSubscriptions"); Invoke("SetupEventSubscriptions");
		Assert.AreEqual(4, subscriptions.Count, "Hook-only NPCs must subscribe exactly once per wanted tick.");
		Invoke("ReleaseEventSubscriptions"); Assert.AreEqual(0, subscriptions.Count);
		Invoke("SetupEventSubscriptions"); Assert.AreEqual(4, subscriptions.Count);
		Set(npc, "_state", CharacterState.Dead); Invoke("SetupEventSubscriptions"); Assert.AreEqual(0, subscriptions.Count);
		Set(npc, "_state", CharacterState.Stasis); Invoke("SetupEventSubscriptions"); Assert.AreEqual(0, subscriptions.Count);
		Set(npc, "_state", CharacterState.Awake); Invoke("SetupEventSubscriptions"); Assert.AreEqual(4, subscriptions.Count);
	}

	private static void Set(object target, string name, object value)
	{
		for (var type = target.GetType(); type is not null; type = type.BaseType)
			if (type.GetField(name, BindingFlags.Instance | BindingFlags.NonPublic) is { } field) { field.SetValue(target, value); return; }
		Assert.Fail("Missing native fixture field " + name);
	}
}
