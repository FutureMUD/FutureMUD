using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Character;
using MudSharp.Events;
using MudSharp.Framework;
using MudSharp.Framework.Save;
using MudSharp.Framework.Scheduling;
using MudSharp.NPC.AI;
using MudNpc = MudSharp.NPC.NPC;

#nullable enable

namespace MudSharp_Unit_Tests.NPC;

[TestClass]
public class NPCAIEventSubscriptionTests
{
	[TestMethod]
	public void AddAIRegistersHeartbeatSubscriptionsForLiveNpc()
	{
		var heartbeat = new Mock<IHeartbeatManager>();
		var npc = BuildUninitialisedNpc(heartbeat.Object);
		var ai = MinuteTickAI();

		npc.AddAI(ai.Object);

		heartbeat.VerifyAdd(x => x.FuzzyMinuteHeartbeat += It.IsAny<HeartbeatManagerDelegate>(), Times.Once);
	}

	[TestMethod]
	public void RemoveAIReleasesHeartbeatSubscriptionsForLiveNpc()
	{
		var heartbeat = new Mock<IHeartbeatManager>();
		var ai = MinuteTickAI();
		var npc = BuildUninitialisedNpc(heartbeat.Object, [ai.Object]);

		npc.RemoveAI(ai.Object);

		heartbeat.VerifyRemove(x => x.FuzzyMinuteHeartbeat -= It.IsAny<HeartbeatManagerDelegate>(), Times.Once);
		heartbeat.VerifyAdd(x => x.FuzzyMinuteHeartbeat += It.IsAny<HeartbeatManagerDelegate>(), Times.Never);
	}

	[TestMethod]
	public void NpcSecondaryRegistersHeartbeatSubscriptionsForItsOwnAIs()
	{
		var heartbeat = new Mock<IHeartbeatManager>();
		var secondary = BuildUninitialisedNpcSecondary(heartbeat.Object, [MinuteTickAI().Object]);

		secondary.SetupEventSubscriptions();

		heartbeat.VerifyAdd(x => x.FuzzyMinuteHeartbeat += It.IsAny<HeartbeatManagerDelegate>(), Times.Once);
	}

	[TestMethod]
	public void NpcSecondaryReleaseDoesNotReleaseOwnerSubscription()
	{
		var heartbeat = new Mock<IHeartbeatManager>();
		var ai = MinuteTickAI();
		var npc = BuildUninitialisedNpc(heartbeat.Object, [ai.Object]);
		var secondary = BuildUninitialisedNpcSecondary(heartbeat.Object, [ai.Object]);

		npc.SetupEventSubscriptions();
		secondary.SetupEventSubscriptions();
		secondary.ReleaseEventSubscriptions();

		heartbeat.VerifyAdd(x => x.FuzzyMinuteHeartbeat += It.IsAny<HeartbeatManagerDelegate>(), Times.Exactly(2));
		heartbeat.VerifyRemove(x => x.FuzzyMinuteHeartbeat -= It.IsAny<HeartbeatManagerDelegate>(), Times.Exactly(3));
	}

	private static MudNpc BuildUninitialisedNpc(IHeartbeatManager heartbeatManager,
		IEnumerable<IArtificialIntelligence>? ais = null)
	{
		var saveManager = new Mock<ISaveManager>();
		var gameworld = new Mock<IFuturemud>();
		gameworld.SetupGet(x => x.HeartbeatManager).Returns(heartbeatManager);
		gameworld.SetupGet(x => x.SaveManager).Returns(saveManager.Object);
		var npc = (MudNpc)RuntimeHelpers.GetUninitializedObject(typeof(MudNpc));
		SetField(typeof(MudNpc), npc, "_AIs", new List<IArtificialIntelligence>(ais ?? []));
		typeof(LateKeywordedInitialisingItem)
			.GetProperty("Gameworld", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)!
			.SetValue(npc, gameworld.Object);
		return npc;
	}

	private static NpcCharacterInstance BuildUninitialisedNpcSecondary(IHeartbeatManager heartbeatManager,
		IEnumerable<IArtificialIntelligence>? ais = null)
	{
		var saveManager = new Mock<ISaveManager>();
		var gameworld = new Mock<IFuturemud>();
		gameworld.SetupGet(x => x.HeartbeatManager).Returns(heartbeatManager);
		gameworld.SetupGet(x => x.SaveManager).Returns(saveManager.Object);
		var secondary = (NpcCharacterInstance)RuntimeHelpers.GetUninitializedObject(typeof(NpcCharacterInstance));
		SetField(typeof(NpcCharacterInstance), secondary, "_AIs",
			new List<IArtificialIntelligence>(ais ?? []));
		typeof(LateKeywordedInitialisingItem)
			.GetProperty("Gameworld", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)!
			.SetValue(secondary, gameworld.Object);
		return secondary;
	}

	private static Mock<IArtificialIntelligence> MinuteTickAI()
	{
		var ai = new Mock<IArtificialIntelligence>();
		ai.Setup(x => x.HandlesEvent(It.IsAny<EventType[]>()))
		  .Returns((EventType[] types) => types.Contains(EventType.MinuteTick));
		return ai;
	}

	private static void SetField<T>(Type type, object target, string name, T value)
	{
		type.GetField(name, BindingFlags.Instance | BindingFlags.NonPublic)!
		    .SetValue(target, value);
	}
}
