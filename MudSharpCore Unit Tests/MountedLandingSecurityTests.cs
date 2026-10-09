#nullable enable

using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Body;
using MudSharp.Body.Position;
using MudSharp.Body.Position.PositionStates;
using MudSharp.Character;
using MudSharp.Construction;
using MudSharp.Framework;
using MudSharp.PerceptionEngine;

namespace MudSharp_Unit_Tests;

[TestClass]
public class MountedLandingSecurityTests
{
	[DataTestMethod]
	[DataRow("You are not flying.")]
	[DataRow("You cannot land while moving.")]
	[DataRow("A movement blocking effect prevents landing.")]
	public void Land_PrimaryRiderWhenMountCannotLand_RejectsBeforePositionChange(string error)
	{
		var (rider, mount, output) = Fixture(false);
		mount.Setup(x => x.CanLand()).Returns((false, error));
		rider.Land();
		mount.Verify(x => x.RiderMovePosition(It.IsAny<IPositionState>(), It.IsAny<PositionModifier>(),
			It.IsAny<IPerceivable>(), rider, It.IsAny<IEmote>(), It.IsAny<IEmote>(), false, false), Times.Never);
		output.Verify(x => x.Send(error, true, false), Times.Once);
	}

	[DataTestMethod]
	[DataRow(false)]
	[DataRow(true)]
	public void Land_PrimaryRiderWhenMountCanLand_PreservesControlledDestinationPosition(bool swimming)
	{
		var (rider, mount, _) = Fixture(swimming);
		mount.Setup(x => x.CanLand()).Returns((true, string.Empty));
		rider.Land();
		mount.Verify(x => x.RiderMovePosition(swimming ? PositionSwimming.Instance : PositionStanding.Instance,
			PositionModifier.None, null, rider, null, null, false, false), Times.Once);
		Assert.AreSame(PositionRiding.Instance, rider.PositionState,
			"The rider's riding posture must not be used as the mount's flying precondition.");
	}

	private static (MudSharp.Character.Character Rider, Mock<ICharacter> Mount, Mock<IOutputHandler> Output) Fixture(bool swimming)
	{
		var rider = TestObjectFactory.CreateUninitialized<MudSharp.Character.Character>();
		rider.SetNoSave(true);
		var body = new Mock<IBody>();
		body.SetupGet(x => x.PositionState).Returns(PositionRiding.Instance);
		typeof(MudSharp.Character.Character).GetProperty(nameof(MudSharp.Character.Character.Body))!.SetValue(rider, body.Object);
		var room = new Mock<IRoom>();
		room.Setup(x => x.IsSwimmingLayer(RoomLayer.GroundLevel)).Returns(swimming);
		typeof(PerceivedItem).GetProperty(nameof(PerceivedItem.Location))!.SetValue(rider, room.Object);
		var mount = new Mock<ICharacter>();
		mount.Setup(x => x.IsPrimaryRider(rider)).Returns(true);
		rider.RidingMount = mount.Object;
		var output = new Mock<IOutputHandler>();
		typeof(PerceivedItem).GetProperty(nameof(PerceivedItem.OutputHandler))!.SetValue(rider, output.Object);
		return (rider, mount, output);
	}
}
