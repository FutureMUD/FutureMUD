#nullable enable

using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Character;
using MudSharp.Construction;
using MudSharp.Events;
using MudSharp.Framework;
using MudSharp.FutureProg;
using MudSharp.Magic;

namespace MudSharp_Unit_Tests;

public partial class MagicGatheringServiceTests
{
	[TestMethod]
	[DataRow(MagicGatheringMethodKind.Land)]
	[DataRow(MagicGatheringMethodKind.Gentle)]
	[DataRow(MagicGatheringMethodKind.Self)]
	public void AcceptedLandStart_NotifiesWitnessOnceWithoutConsumingTheActionOrSpendingResources(MagicGatheringMethodKind kind)
	{
		using var f = new GatheringFixture(kind, stamina: kind == MagicGatheringMethodKind.Self ? 2 : 0);
		if (kind != MagicGatheringMethodKind.Self) f.SetSourceBalance(20);
		var observer = new Mock<ICharacter>(); observer.SetupGet(x => x.Location).Returns(f.Room);
		observer.SetupGet(x => x.State).Returns(CharacterState.Awake);
		observer.SetupGet(x => x.RoomLayer).Returns(RoomLayer.GroundLevel);
		observer.SetupGet(x => x.SpatialLocation).Returns(new SpatialLocation(f.Room, RoomLayer.GroundLevel));
		var received = new List<object[]>();
		observer.Setup(x => x.HandleEvent(EventType.CharacterBeginsLandMagicGatheringWitness, It.IsAny<object[]>()))
			.Callback<EventType, object[]>((_, values) => received.Add(values)).Returns(true);
		((List<ICharacter>)typeof(Location).GetField("_characters", BindingFlags.Instance | BindingFlags.NonPublic)!.GetValue(f.Room)!).Add(observer.Object);
		var sourceBefore = f.SourceBalance; var destinationBefore = f.DestinationBalance;
		var preview = f.Service.Preview(f.Actor.Object, f.Capability.Object, "draw", 5);
		Assert.IsTrue(preview.Success, preview.Message); Assert.AreEqual(0, received.Count);
		var started = f.Service.Begin(f.Actor.Object, f.Capability.Object, "draw", 5);
		Assert.IsTrue(started.Success, started.Message);
		Assert.AreEqual(kind == MagicGatheringMethodKind.Land ? 1 : 0, received.Count);
		Assert.AreEqual(sourceBefore, f.SourceBalance); Assert.AreEqual(destinationBefore, f.DestinationBalance);
		if (received.Count > 0)
		{
			var values = received.Single(); Assert.AreSame(f.Actor.Object, values[0]); Assert.AreSame(observer.Object, values[1]);
			Assert.AreSame(f.Room, values[2]); Assert.AreSame(f.Capability.Object, values[3]);
			Assert.AreEqual(f.Method.Key.ToString(), values[4]); Assert.AreEqual(5.0, values[5]); Assert.AreEqual(started.OperationId!.Value.ToString(), values[6]);
		}
		var repeated = f.Service.Begin(f.Actor.Object, f.Capability.Object, "draw", 5);
		Assert.IsFalse(repeated.Success); Assert.AreEqual(kind == MagicGatheringMethodKind.Land ? 1 : 0, received.Count);
	}

	[TestMethod]
	public void LandStartWitnessMetadata_MatchesTheRuntimePayloadWithoutRenumberingExistingEvents()
	{
		Assert.AreEqual(155, (int)EventType.LocationOnGameLoadFinished);
		Assert.AreEqual(156, (int)EventType.CharacterBeginsLandMagicGatheringWitness);
		var info = typeof(EventType).GetField(nameof(EventType.CharacterBeginsLandMagicGatheringWitness))!.GetCustomAttribute<EventInfoAttribute>()!;
		CollectionAssert.AreEqual(new[] { "actor", "witness", "location", "capability", "method", "amount", "operation" }, info.Parameters.Select(x => x.name).ToArray());
		CollectionAssert.AreEqual(new[] { ProgVariableTypes.Character, ProgVariableTypes.Perceivable, ProgVariableTypes.Location,
			ProgVariableTypes.MagicCapability, ProgVariableTypes.Text, ProgVariableTypes.Number, ProgVariableTypes.Text }, info.ProgTypes.ToArray());
	}
}
