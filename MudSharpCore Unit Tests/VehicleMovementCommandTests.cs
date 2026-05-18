#nullable enable

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Character;
using MudSharp.Commands;
using MudSharp.Commands.Modules;
using MudSharp.Construction;
using MudSharp.Construction.Boundary;
using MudSharp.Form.Shape;
using MudSharp.Framework;
using MudSharp.Framework.Scheduling;
using MudSharp.GameItems;
using MudSharp.Movement;
using MudSharp.PerceptionEngine;
using MudSharp.Vehicles;
using System.Linq;
using System.Reflection;

namespace MudSharp_Unit_Tests;

[TestClass]
public class VehicleMovementCommandTests
{
	[TestMethod]
	public void Disembark_DoesNotShowHelpWhenUsedWithoutArguments()
	{
		var method = typeof(VehicleModule).GetMethod("Disembark", BindingFlags.Static | BindingFlags.NonPublic);
		var help = method!.GetCustomAttribute<HelpInfo>();

		Assert.IsNotNull(help);
		Assert.AreEqual(AutoHelp.HelpArg, help!.AutoHelpSetting);
	}

	[TestMethod]
	public void Drive_IsMovementCommandAndCanQueueWhileMoving()
	{
		var method = typeof(VehicleModule).GetMethod("Drive", BindingFlags.Static | BindingFlags.NonPublic);

		Assert.IsNotNull(method);
		Assert.IsNull(method!.GetCustomAttribute<NoMovementCommand>());
		Assert.IsNotNull(method.GetCustomAttribute<DelayBlock>());
	}

	[TestMethod]
	public void VehicleMovement_InitialAction_BeginsTransitAndSchedulesDelayedMove()
	{
		var scheduler = new Mock<IScheduler>();
		var gameworld = new Mock<IFuturemud>();
		gameworld.SetupGet(x => x.Scheduler).Returns(scheduler.Object);
		gameworld.Setup(x => x.GetStaticDouble("MaximumMoveTimeMilliseconds")).Returns(10_000.0);

		var output = new Mock<IOutputHandler>();
		var actor = new Mock<ICharacter>();
		actor.SetupProperty(x => x.Movement);
		actor.SetupGet(x => x.Gameworld).Returns(gameworld.Object);
		actor.SetupGet(x => x.OutputHandler).Returns(output.Object);
		actor.SetupGet(x => x.InnerLineFormatLength).Returns(120);
		actor.SetupGet(x => x.QueuedMoveCommands).Returns(new System.Collections.Generic.Queue<string>());
		actor.Setup(x => x.MoveSpeed(It.IsAny<ICellExit>())).Returns(250.0);
		actor.Setup(x => x.CanSee(actor.Object, It.IsAny<PerceiveIgnoreFlags>())).Returns(true);
		actor.Setup(x => x.IsSelf(actor.Object)).Returns(true);
		actor.Setup(x => x.HowSeen(actor.Object, It.IsAny<bool>(), It.IsAny<DescriptionType>(), It.IsAny<bool>(),
			It.IsAny<PerceiveIgnoreFlags>())).Returns("you");

		var origin = new Mock<ICell>();
		origin.SetupGet(x => x.Gameworld).Returns(gameworld.Object);
		origin.Setup(x => x.LayerCharacters(RoomLayer.GroundLevel)).Returns([actor.Object]);
		var destination = new Mock<ICell>();
		destination.SetupGet(x => x.Gameworld).Returns(gameworld.Object);
		destination.Setup(x => x.LayerCharacters(RoomLayer.GroundLevel)).Returns([]);

		var exitModel = new Mock<IExit>();
		exitModel.SetupGet(x => x.MaximumSizeToEnter).Returns(SizeCategory.Huge);
		var exit = new Mock<ICellExit>();
		exit.SetupGet(x => x.Origin).Returns(origin.Object);
		exit.SetupGet(x => x.Destination).Returns(destination.Object);
		exit.SetupGet(x => x.Exit).Returns(exitModel.Object);
		exit.SetupGet(x => x.OutboundMovementSuffix).Returns("north");
		exit.Setup(x => x.MovementTransition(actor.Object))
		    .Returns((CellMovementTransition.GroundToGround, RoomLayer.GroundLevel));

		var profile = new Mock<IVehicleMovementProfilePrototype>();
		profile.SetupGet(x => x.Id).Returns(1L);
		profile.SetupGet(x => x.MovementType).Returns(VehicleMovementProfileType.CellExit);
		profile.SetupGet(x => x.IsDefault).Returns(true);
		var prototype = new Mock<IVehiclePrototype>();
		prototype.SetupGet(x => x.MovementProfiles).Returns([profile.Object]);

		var exterior = new Mock<IGameItem>();
		exterior.SetupGet(x => x.Size).Returns(SizeCategory.Normal);
		exterior.Setup(x => x.HowSeen(actor.Object, It.IsAny<bool>(), It.IsAny<DescriptionType>(), It.IsAny<bool>(),
			It.IsAny<PerceiveIgnoreFlags>())).Returns("a QA test bicycle");
		actor.Setup(x => x.CanSee(exterior.Object, It.IsAny<PerceiveIgnoreFlags>())).Returns(true);
		var vehicle = new Mock<IVehicle>();
		vehicle.SetupGet(x => x.Id).Returns(1L);
		vehicle.SetupGet(x => x.Name).Returns("test bicycle");
		vehicle.SetupGet(x => x.Controller).Returns(actor.Object);
		vehicle.SetupGet(x => x.Location).Returns(origin.Object);
		vehicle.SetupGet(x => x.RoomLayer).Returns(RoomLayer.GroundLevel);
		vehicle.SetupGet(x => x.Prototype).Returns(prototype.Object);
		vehicle.SetupGet(x => x.ExteriorItem).Returns(exterior.Object);
		vehicle.SetupGet(x => x.Occupants).Returns([actor.Object]);
		vehicle.Setup(x => x.IsOccupant(actor.Object)).Returns(true);
		vehicle.SetupGet(x => x.AccessPoints).Returns([]);
		vehicle.SetupGet(x => x.Installations).Returns([]);
		vehicle.SetupGet(x => x.TowLinks).Returns([]);

		var movement = new VehicleMovement(vehicle.Object, actor.Object, exit.Object);

		movement.InitialAction();

		vehicle.Verify(x => x.BeginMoveToCell(destination.Object, RoomLayer.GroundLevel, exit.Object), Times.Once);
		vehicle.Verify(x => x.MoveToCell(It.IsAny<ICell>(), It.IsAny<RoomLayer>(), It.IsAny<ICellExit>(), It.IsAny<IMovement>()), Times.Never);
		scheduler.Verify(x => x.AddSchedule(It.Is<ISchedule>(schedule => schedule.Type == ScheduleType.Movement)), Times.Once);
		output.Verify(x => x.Send(It.Is<string>(text => text.Contains("begin riding") &&
		                                                text.Contains("QA test bicycle")), true, false), Times.Once);
		Assert.AreSame(movement, actor.Object.Movement);
	}
}
