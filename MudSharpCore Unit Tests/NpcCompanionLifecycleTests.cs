#nullable enable
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Xml.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Construction;
using MudSharp.Character;
using MudSharp.Framework;
using MudSharp.NPC;
using MudSharp.NPC.AI;
using MudSharp.Movement;

namespace MudSharp_Unit_Tests;

[TestClass]
public class NpcCompanionLifecycleTests
{
	private static MountAI Load(bool? nearby, bool follow = false)
	{
		var xml = new XElement("Definition");
		foreach (var name in new[] { "PermitRiderProg", "PermitControlProg", "WhyCannotPermitRiderProg", "MountNonConsensualMountDifficulty", "MountControlDifficulty", "MountResistBuckDifficulty", "MaximumNumberOfRiders" })
			xml.Add(new XElement(name, 0));
		foreach (var name in new[] { "RawMountEmote", "RawDismountEmote", "RawControlDeniedEmote", "RawBuckEmote" }) xml.Add(new XElement(name, ""));
		if (nearby.HasValue) xml.Add(new XElement("CompanionRequiresColocation", nearby.Value));
		xml.Add(new XElement("CompanionFollowsHandler", follow));
		return (MountAI)Activator.CreateInstance(typeof(MountAI), BindingFlags.Instance | BindingFlags.NonPublic,
			null, [new MudSharp.Models.ArtificialIntelligence { Id = 1, Name = "Mount", Type = "Mount", Definition = xml.ToString() },
				new Mock<IFuturemud> { DefaultValue = DefaultValue.Mock }.Object], null)!;
	}

	[TestMethod]
	public void NearbyCompanionPolicy_DefaultsOffAndSavesExplicitSetting()
	{
		Assert.IsFalse(Load(null).CompanionRequiresColocation);
		var configured = Load(true);
		Assert.IsTrue(configured.CompanionRequiresColocation);
		var xml = XElement.Parse((string)typeof(MountAI).GetMethod("SaveToXml", BindingFlags.Instance | BindingFlags.NonPublic)!.Invoke(configured, null)!);
		Assert.IsTrue((bool)xml.Element("CompanionRequiresColocation")!);
		Assert.IsFalse(Load(null).CompanionFollowsHandler);
		Assert.IsTrue(Load(true, true).CompanionFollowsHandler);
	}

	[TestMethod]
	public void CompanionLogout_RemovesRidingLinks_WithoutAffectingLegacyMounts()
	{
		var npc = new Mock<INPC>(); var rider = Mock.Of<ICharacter>();
		npc.SetupGet(x => x.Riders).Returns([rider]);
		npc.SetupGet(x => x.AIs).Returns([Load(true, true)]);
		NpcCompanionLifecycle.PrepareForLogout(npc.Object);
		npc.Verify(x => x.RemoveRider(rider), Times.Once);
		npc.SetupGet(x => x.AIs).Returns([Load(null)]);
		NpcCompanionLifecycle.PrepareForLogout(npc.Object);
		npc.Verify(x => x.RemoveRider(rider), Times.Once);
	}

	[TestMethod]
	public void SetFollowing_RefusesCyclesWithoutBreakingExistingRelationship()
	{
		var follower = new Mock<ICharacter>(); var leader = new Mock<ICharacter>(); var oldLeader = new Mock<ICharacter>();
		follower.Setup(x => x.ColocatedWith(leader.Object)).Returns(true);
		follower.SetupGet(x => x.Following).Returns(oldLeader.Object);
		leader.SetupGet(x => x.Following).Returns(follower.Object);
		Assert.IsFalse(NpcCompanionLifecycle.SetFollowing(follower.Object, leader.Object));
		follower.Verify(x => x.Follow(It.IsAny<IMove>()), Times.Never);
		leader.SetupGet(x => x.Following).Returns(leader.Object);
		Assert.IsFalse(NpcCompanionLifecycle.SetFollowing(follower.Object, leader.Object));
	}

	[TestMethod]
	public void Assignment_RejectsDistantOrOfflineNpcAndPersistsIdentityWithoutFollowing()
	{
		var world = new Mock<IFuturemud>(); var npc = new Mock<INPC>(); var player = new Mock<ICharacter>();
		var cache = new Dictionary<long, List<ICharacter>>();
		world.SetupGet(x => x.CachedBodyguards).Returns(cache);
		npc.SetupGet(x => x.Gameworld).Returns(world.Object);
		npc.SetupProperty(x => x.BodyguardingCharacterID);
		player.SetupGet(x => x.IsPlayerCharacter).Returns(true); player.SetupGet(x => x.Id).Returns(42);
		Assert.IsFalse(NpcCompanionLifecycle.Assign(npc.Object, player.Object));
		npc.Setup(x => x.ColocatedWith(player.Object)).Returns(true);
		Assert.IsTrue(NpcCompanionLifecycle.Assign(npc.Object, player.Object));
		Assert.AreEqual(42L, npc.Object.BodyguardingCharacterID);
		npc.Verify(x => x.Follow(It.IsAny<IMove>()), Times.Never);
		cache[42] = [npc.Object];
		Assert.IsFalse(NpcCompanionLifecycle.Assign(npc.Object, null));
		cache.Clear();
		Assert.IsTrue(NpcCompanionLifecycle.Assign(npc.Object, null));
		Assert.IsNull(npc.Object.BodyguardingCharacterID);
	}

	[TestMethod]
	public void Reunion_RequiresAssignedPlayerEvenWhenNearbyOptionOff_AndPreservesHitchChain()
	{
		var npc = new Mock<INPC>(); var player = new Mock<ICharacter>(); var stranger = new Mock<ICharacter>();
		npc.SetupGet(x => x.AIs).Returns([Load(false, true)]);
		npc.SetupGet(x => x.BodyguardingCharacterID).Returns(42);
		player.SetupGet(x => x.Id).Returns(42); player.SetupGet(x => x.IsPlayerCharacter).Returns(true);
		stranger.SetupGet(x => x.Id).Returns(43); stranger.SetupGet(x => x.IsPlayerCharacter).Returns(true);
		npc.Setup(x => x.ColocatedWith(It.IsAny<ICharacter>())).Returns(true);
		NpcCompanionLifecycle.Reunited(npc.Object, stranger.Object);
		npc.Verify(x => x.Follow(It.IsAny<IMove>()), Times.Never);
		NpcCompanionLifecycle.Reunited(npc.Object, player.Object);
		npc.Verify(x => x.Follow(player.Object), Times.Once);
		npc.SetupGet(x => x.Following).Returns(stranger.Object);
		NpcCompanionLifecycle.Reunited(npc.Object, player.Object);
		npc.Verify(x => x.Follow(player.Object), Times.Once);
	}

	[TestMethod]
	public void NearbyCompanionPolicy_OnlyReunitesInSameRoomAndLayer_LeavesLegacyBodyguardsUnchanged()
	{
		var npc = new Mock<INPC>(); var room = new Mock<IRoom>();
		room.SetupGet(x => x.Id).Returns(42);
		npc.SetupGet(x => x.Location).Returns(room.Object);
		npc.SetupGet(x => x.RoomLayer).Returns(RoomLayer.GroundLevel);
		npc.SetupGet(x => x.AIs).Returns([Load(true)]);
		Assert.IsTrue(NpcCompanionLifecycle.CanReunite(npc.Object, 42, (int)RoomLayer.GroundLevel));
		Assert.IsFalse(NpcCompanionLifecycle.CanReunite(npc.Object, 43, (int)RoomLayer.GroundLevel));
		Assert.IsFalse(NpcCompanionLifecycle.CanReunite(npc.Object, 42, (int)RoomLayer.InTrees));
		Assert.IsFalse(NpcCompanionLifecycle.CanReunite(npc.Object, null, (int)RoomLayer.GroundLevel));
		var handler = new Mock<ICharacter>(); handler.SetupGet(x => x.Id).Returns(123);
		handler.SetupGet(x => x.Location).Returns(room.Object); handler.SetupGet(x => x.RoomLayer).Returns(RoomLayer.GroundLevel);
		npc.SetupProperty(x => x.BodyguardingCharacterID, 456L);
		Assert.IsFalse(NpcCompanionLifecycle.CanReuniteWith(npc.Object, handler.Object), "A stale cache must not return a reassigned NPC to its former handler.");
		npc.Object.BodyguardingCharacterID = CharacterInstanceIdentityComparer.IdentityId(handler.Object);
		Assert.IsTrue(NpcCompanionLifecycle.CanReuniteWith(npc.Object, handler.Object));
		handler.SetupGet(x => x.State).Returns(CharacterState.Dead);
		Assert.IsFalse(NpcCompanionLifecycle.CanReuniteWith(npc.Object, handler.Object));
		npc.SetupGet(x => x.AIs).Returns([Load(null)]);
		Assert.IsTrue(NpcCompanionLifecycle.CanReunite(npc.Object, 43, (int)RoomLayer.InTrees));
	}
}
