#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Body.Position.PositionStates;
using MudSharp.Character;
using MudSharp.Communication.Language;
using MudSharp.Construction;
using MudSharp.Construction.Boundary;
using MudSharp.Effects;
using MudSharp.Effects.Concrete;
using MudSharp.Framework;
using MudSharp.Health;
using MudSharp.PerceptionEngine;

namespace MudSharp_Unit_Tests;

[TestClass]
public class RoomPolicyTests
{
	private static Mock<IRoom> Room(long id = 1, RoomRestrictions restrictions = RoomRestrictions.None, int capacity = 0)
	{
		var room = new Mock<IRoom>();
		var world = new Mock<IFuturemud>();
		world.SetupGet(x => x.FutureProgs).Returns(new All<MudSharp.FutureProg.IFutureProg>());
		room.SetupGet(x => x.Id).Returns(id);
		room.SetupGet(x => x.Gameworld).Returns(world.Object);
		room.SetupGet(x => x.Characters).Returns(Array.Empty<ICharacter>());
		room.Setup(x => x.EffectsOfType<IRoomPolicyEffect>(It.IsAny<Predicate<IRoomPolicyEffect>>()))
			.Returns([new RoomPolicyEffect(room.Object, restrictions, capacity)]);
		room.Setup(x => x.EffectsOfType<IRoomExitPolicyEffect>(It.IsAny<Predicate<IRoomExitPolicyEffect>>()))
			.Returns(Array.Empty<IRoomExitPolicyEffect>());
		return room;
	}

	[TestMethod]
	public void NoWandering_DoesNotBecomeACharacterEntryOrSpawnBan()
	{
		var origin = Room(); var destination = Room(2, RoomRestrictions.NpcWandering);
		var actor = new Mock<ICharacter>();
		actor.SetupGet(x => x.Riders).Returns(Array.Empty<ICharacter>());
		var exit = new Mock<IRoomExit>();
		exit.SetupGet(x => x.Origin).Returns(origin.Object); exit.SetupGet(x => x.Destination).Returns(destination.Object);
		Assert.IsFalse(RoomPolicyRules.AllowsWandering(destination.Object));
		Assert.IsNull(RoomPolicyRules.EntryDenial(actor.Object, exit.Object, PositionStanding.Instance));
	}

	[TestMethod]
	public void OccupiedTunnel_DeniesNewEntrantsButAllowsExistingOccupant()
	{
		var room = Room(1, capacity: 1); var resident = new Mock<ICharacter>().Object;
		room.SetupGet(x => x.Characters).Returns([resident]);
		Assert.IsNotNull(RoomPolicyRules.CapacityDenial(room.Object, new Mock<ICharacter>().Object));
		Assert.IsNull(RoomPolicyRules.CapacityDenial(room.Object, resident));
	}

	[TestMethod]
	public void OccupancyCost_CountsRidersAndRejectsInvalidOrOversizedCosts()
	{
		var room = Room(capacity: 1); var actor = new Mock<ICharacter>(); var rider = new Mock<ICharacter>();
		actor.SetupGet(x => x.Riders).Returns([rider.Object]);
		Assert.IsNotNull(RoomPolicyRules.CapacityDenial(room.Object, actor.Object));
		actor.SetupGet(x => x.Riders).Returns(Array.Empty<ICharacter>());
		var prog = new Mock<MudSharp.FutureProg.IFutureProg>(); prog.SetupGet(x => x.Id).Returns(9);
		var progs = new All<MudSharp.FutureProg.IFutureProg>(); progs.Add(prog.Object);
		Mock.Get(room.Object.Gameworld).SetupGet(x => x.FutureProgs).Returns(progs);
		room.Setup(x => x.EffectsOfType<IRoomPolicyEffect>(It.IsAny<Predicate<IRoomPolicyEffect>>()))
			.Returns([new RoomPolicyEffect(room.Object, RoomRestrictions.None, 1, 9)]);
		prog.Setup(x => x.ExecuteDouble(It.IsAny<object[]>())).Returns(2);
		Assert.IsNotNull(RoomPolicyRules.CapacityDenial(room.Object, actor.Object));
		prog.Setup(x => x.ExecuteDouble(It.IsAny<object[]>())).Returns(double.NaN);
		Assert.IsNotNull(RoomPolicyRules.CapacityDenial(room.Object, actor.Object));
		prog.Setup(x => x.ExecuteDouble(It.IsAny<object[]>())).Returns(1);
		Assert.IsNull(RoomPolicyRules.CapacityDenial(room.Object, actor.Object));
	}

	[TestMethod]
	public void GuardAccess_UsesSamePureProgForPassageAndOpening_AndPersistsOpeningFlag()
	{
		var origin = Room(); var destination = Room(2); var actor = new Mock<ICharacter>();
		actor.SetupGet(x => x.Location).Returns(origin.Object);
		var side = new Mock<IRoomExit>(); side.SetupGet(x => x.Origin).Returns(origin.Object);
		side.SetupGet(x => x.Destination).Returns(destination.Object); side.SetupGet(x => x.OutboundDirection).Returns(CardinalDirection.North);
		var exit = new Mock<IExit>(); exit.Setup(x => x.RoomExitFor(origin.Object)).Returns(side.Object);
		var policy = new RoomExitPolicyEffect(origin.Object, CardinalDirection.North, "The guard blocks you.", 9, controlsOpening: true);
		origin.Setup(x => x.EffectsOfType<IRoomExitPolicyEffect>(It.IsAny<Predicate<IRoomExitPolicyEffect>>())).Returns([policy]);
		var prog = new Mock<MudSharp.FutureProg.IFutureProg>(); prog.SetupGet(x => x.Id).Returns(9);
		var progs = new All<MudSharp.FutureProg.IFutureProg>(); progs.Add(prog.Object);
		Mock.Get(origin.Object.Gameworld).SetupGet(x => x.FutureProgs).Returns(progs);
		Assert.AreEqual("The guard blocks you.", RoomPolicyRules.ExitDenial(actor.Object, side.Object));
		Assert.AreEqual("The guard blocks you.", RoomPolicyRules.OpeningDenial(actor.Object, exit.Object));
		prog.Setup(x => x.ExecuteBool(It.IsAny<object[]>())).Returns(true);
		Assert.IsNull(RoomPolicyRules.ExitDenial(actor.Object, side.Object));
		Assert.IsNull(RoomPolicyRules.OpeningDenial(actor.Object, exit.Object));
		var restored = new RoomExitPolicyEffect(policy.SaveToXml(new Dictionary<IEffect, TimeSpan>()), origin.Object);
		Assert.IsTrue(restored.ControlsOpening); Assert.AreEqual(9L, restored.AccessProgId);
	}

	[TestMethod]
	public void DeniedAttemptProg_IsSeparateFromPureAdmissionAndSurvivesPersistence()
	{
		var origin = Room(); var actor = new Mock<ICharacter>(); actor.SetupGet(x => x.Location).Returns(origin.Object);
		var exit = new Mock<IRoomExit>(); exit.SetupGet(x => x.Origin).Returns(origin.Object);
		exit.SetupGet(x => x.Destination).Returns(Room(2).Object); exit.SetupGet(x => x.OutboundDirection).Returns(CardinalDirection.South);
		var hazard = new Mock<MudSharp.FutureProg.IFutureProg>(); hazard.SetupGet(x => x.Id).Returns(10);
		var progs = new All<MudSharp.FutureProg.IFutureProg>(); progs.Add(hazard.Object);
		Mock.Get(origin.Object.Gameworld).SetupGet(x => x.FutureProgs).Returns(progs);
		var policy = new RoomExitPolicyEffect(origin.Object, CardinalDirection.South, "No passage.", deniedAttemptProgId: 10);
		Assert.AreEqual("No passage.", policy.DenialReason(actor.Object, exit.Object));
		hazard.Verify(x => x.Execute(It.IsAny<object[]>()), Times.Never);
		var loaded = new RoomExitPolicyEffect(policy.SaveToXml(new Dictionary<IEffect, TimeSpan>()), origin.Object);
		Assert.AreEqual(10L, loaded.DeniedAttemptProgId);
		loaded.OnDeniedAttempt(actor.Object, exit.Object);
		hazard.Verify(x => x.Execute(It.IsAny<object[]>()), Times.Once);
	}

	[TestMethod]
	public void MagicRules_BlockBothSourceAndTargetButDoNotRestrictOrdinaryRooms()
	{
		var clear = Room(); var blocked = Room(2, RoomRestrictions.Magic);
		var actor = new Mock<ICharacter>(); actor.SetupGet(x => x.Location).Returns(clear.Object);
		Assert.IsNull(RoomPolicyRules.MagicDenial(actor.Object, [clear.Object]));
		Assert.IsNotNull(RoomPolicyRules.MagicDenial(actor.Object, [blocked.Object]));
		actor.SetupGet(x => x.Location).Returns(blocked.Object);
		Assert.IsNotNull(RoomPolicyRules.MagicDenial(actor.Object, []));
	}

	[TestMethod]
	public void ConfiguredCasting_NoMagicRefusesBeforeResourcesChecksOrProgress()
	{
		var fixture = new MagicCastingFixture(); fixture.Acquire();
		var room = Mock.Get(fixture.Actor.Object.Location);
		room.Setup(x => x.EffectsOfType<IRoomPolicyEffect>(It.IsAny<Predicate<IRoomPolicyEffect>>()))
			.Returns([new RoomPolicyEffect(room.Object, RoomRestrictions.Magic)]);
		var result = fixture.Service.Cast(fixture.Intent());
		Assert.AreEqual(MudSharp.Magic.MagicCastingStatus.Refused, result.Status);
		Assert.AreEqual(100.0, fixture.Balances[fixture.Resources[1]]);
		Assert.AreEqual(0, fixture.Rolls);
	}

	[TestMethod]
	public void LethalEntry_UsesNativeDeathAndLeavesOrdinaryRoomsAlone()
	{
		var actor = new Mock<ICharacter>();
		actor.SetupGet(x => x.OutputHandler).Returns(new Mock<IOutputHandler>().Object);
		Assert.IsFalse(RoomPolicyRules.ApplyEntryHazard(Room().Object, actor.Object));
		Assert.IsTrue(RoomPolicyRules.ApplyEntryHazard(Room(2, RoomRestrictions.Lethal).Object, actor.Object));
		actor.Verify(x => x.Die(), Times.Once);
	}

	[TestMethod]
	public void FlightExitRule_AppliesToMatchingDirectionAndSurvivesRoundTrip()
	{
		var origin = Room(); var exit = new Mock<IRoomExit>();
		exit.SetupGet(x => x.OutboundDirection).Returns(CardinalDirection.Up);
		var policy = new RoomExitPolicyEffect(origin.Object, CardinalDirection.Up, "You fall back.", requiresFlight: true);
		var actor = new Mock<ICharacter>(); actor.SetupGet(x => x.PositionState).Returns(PositionStanding.Instance);
		Assert.AreEqual("You fall back.", policy.DenialReason(actor.Object, exit.Object));
		actor.SetupGet(x => x.PositionState).Returns(PositionFlying.Instance);
		Assert.IsNull(policy.DenialReason(actor.Object, exit.Object));
		var copy = new RoomExitPolicyEffect(policy.SaveToXml(new Dictionary<IEffect, TimeSpan>()), origin.Object);
		Assert.AreEqual(policy.Direction, copy.Direction); Assert.IsTrue(copy.RequiresFlight);
		Assert.AreEqual(policy.Message, copy.Message);
	}

	[TestMethod]
	public void ViewLink_ForwardsVisibleCommunicationWithoutWordsOrRecursiveRoomOutput()
	{
		var watched = Room(); var observerRoom = Room(2);
		var rooms = new All<IRoom>(); rooms.Add(observerRoom.Object);
		Mock.Get(watched.Object.Gameworld).SetupGet(x => x.Rooms).Returns(rooms);
		var source = new Mock<ICharacter>(); source.SetupGet(x => x.Location).Returns(watched.Object);
		var observer = new Mock<ICharacter>(); observer.SetupGet(x => x.State).Returns(CharacterState.Awake);
		observer.Setup(x => x.CanSee(source.Object, It.IsAny<PerceiveIgnoreFlags>())).Returns(true);
		observerRoom.SetupGet(x => x.Characters).Returns([observer.Object]);
		var messages = new List<string>(); var handler = new Mock<IOutputHandler>();
		handler.Setup(x => x.Send(It.IsAny<string>(), true, false)).Callback<string, bool, bool>((s, _, _) => messages.Add(s));
		observer.SetupGet(x => x.OutputHandler).Returns(handler.Object);
		var emote = new Mock<IEmote>(); emote.Setup(x => x.ParseFor(observer.Object, It.IsAny<PerceiveIgnoreFlags>())).Returns("A traveller says");
		var output = new Mock<IRecordableLanguageOutput>(); output.SetupGet(x => x.DefaultSource).Returns(source.Object);
		output.SetupGet(x => x.LanguageInfo).Returns((SpokenLanguageInfo)RuntimeHelpers.GetUninitializedObject(typeof(SpokenLanguageInfo)));
		output.SetupGet(x => x.PreLanguageEmote).Returns(emote.Object);
		var link = new RoomObservationEffect(watched.Object, 2, "Below:");
		link.HandleOutput(output.Object, watched.Object);
		CollectionAssert.AreEqual(new[] { "Below: A traveller says something." }, messages);
		output.Verify(x => x.ParseFor(It.IsAny<IPerceiver>()), Times.Never);
		link.HandleOutput("a secret raw message", watched.Object);
		output.SetupGet(x => x.Flags).Returns(OutputFlags.IgnoreWatchers);
		link.HandleOutput(output.Object, watched.Object);
		Assert.AreEqual(1, messages.Count);
		output.SetupGet(x => x.Flags).Returns(OutputFlags.Normal);
		observer.SetupGet(x => x.State).Returns(CharacterState.Sleeping);
		link.HandleOutput(output.Object, watched.Object);
		Assert.AreEqual(1, messages.Count);
		observer.SetupGet(x => x.State).Returns(CharacterState.Awake);
		observer.Setup(x => x.CanSee(source.Object, It.IsAny<PerceiveIgnoreFlags>())).Returns(false);
		link.HandleOutput(output.Object, watched.Object);
		Assert.AreEqual(1, messages.Count);
	}

	[TestMethod]
	public void InteriorObservationRemap_UsesSameInstanceAndRetainsExternalObserver()
	{
		var source = Room(1); var templateObserver = Room(2); var instanceObserver = Room(22);
		var link = new RoomObservationEffect(source.Object, 2, "From the deck:");
		link.RemapObserver(new Dictionary<IRoom, IRoom> { [templateObserver.Object] = instanceObserver.Object });
		Assert.AreEqual(22L, link.ObserverRoomId);
		var copy = new RoomObservationEffect(link.SaveToXml(new Dictionary<IEffect, TimeSpan>()), source.Object);
		Assert.AreEqual(22L, copy.ObserverRoomId); Assert.AreEqual(link.Prefix, copy.Prefix);
		copy.RemapObserver(new Dictionary<IRoom, IRoom> { [templateObserver.Object] = Room(32).Object });
		Assert.AreEqual(22L, copy.ObserverRoomId);
	}
}
