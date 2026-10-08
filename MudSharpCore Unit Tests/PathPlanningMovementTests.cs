#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Body;
using MudSharp.Body.Position;
using MudSharp.Body.Position.PositionStates;
using MudSharp.Character;
using MudSharp.Character.Heritage;
using MudSharp.Construction;
using MudSharp.Construction.Boundary;
using MudSharp.Effects;
using MudSharp.Form.Shape;
using MudSharp.Framework;
using MudSharp.Framework.Save;
using MudSharp.GameItems;
using MudSharp.Movement;
using MudSharp.NPC.AI;

namespace MudSharp_Unit_Tests;

[TestClass]
public class PathPlanningMovementTests
{
	[TestMethod]
	public void RemoteLeg_PlanningAllowsIt_ExecutionStillRequiresItsActualOrigin()
	{
		var f = new Fixture();
		Assert.IsTrue(f.Actor.CanMoveForPathPlanning(f.Second.Object));
		Assert.IsFalse(f.Actor.CanMove(f.Second.Object, CanMoveFlags.None));
		Assert.AreSame(f.Origin.Object, f.Actor.Location);
		f.Actor.At(f.Middle.Object);
		Assert.IsTrue(f.Actor.CanMove(f.Second.Object, CanMoveFlags.None));
	}

	[TestMethod]
	public void Planning_RetainsTopologySizeCrawlingAndSafeWaterChecks()
	{
		var f = new Fixture();
		Assert.IsFalse(f.Actor.CanMoveForPathPlanning(null!));
		f.Second.SetupGet(x => x.Destination).Returns((IRoom)null!);
		Assert.IsFalse(f.Actor.CanMoveForPathPlanning(f.Second.Object));
		f.Second.SetupGet(x => x.Destination).Returns(f.Home.Object);
		f.Second.Setup(x => x.MovementTransition(f.Actor)).Returns((RoomMovementTransition.NoViableTransition, RoomLayer.GroundLevel));
		Assert.IsFalse(f.Actor.CanMoveForPathPlanning(f.Second.Object));
		f.Second.Setup(x => x.MovementTransition(f.Actor)).Returns((RoomMovementTransition.GroundToGround, RoomLayer.GroundLevel));
		f.Second.SetupGet(x => x.Exit.MaximumSizeToEnter).Returns(SizeCategory.Tiny);
		Assert.IsFalse(f.Actor.CanMoveForPathPlanning(f.Second.Object));
		f.Second.SetupGet(x => x.Exit.MaximumSizeToEnter).Returns(SizeCategory.Normal);
		f.Actor.TestPosition = PositionProne.Instance;
		Assert.IsFalse(f.Actor.CanMoveForPathPlanning(f.Second.Object), "An unusable crawling body must not pass planning.");
		f.Actor.TestPosition = PositionStanding.Instance;
		f.Second.Setup(x => x.MovementTransition(f.Actor)).Returns((RoomMovementTransition.SwimOnly, RoomLayer.GroundLevel));
		Assert.IsFalse(f.Actor.CanMoveForPathPlanning(f.Second.Object), "Safe movement remains in force for a water entry.");
		Assert.IsTrue(f.Actor.CanMoveForPathPlanning(f.Second.Object, CanMoveFlags.IgnoreSafeMovement));
	}

	[TestMethod]
	public void SharedPathPredicate_FindsTwoLegs_AndExecutionRechecksChangedPhysicalConditions()
	{
		var f = new Fixture();
		var ai = TestObjectFactory.CreateUninitialized<PathToLocationAI>();
		var predicate = (Func<IRoomExit, bool>)typeof(PathingAIBase)
			.GetMethod("GetSuitabilityFunction", BindingFlags.NonPublic | BindingFlags.Instance)!
			.Invoke(ai, [f.Actor, true])!;
		var path = f.Actor.PathBetween(f.Home.Object, 3, predicate).ToList();
		CollectionAssert.AreEqual(new[] { f.First.Object, f.Second.Object }, path);
		Assert.IsTrue(f.Actor.CanMove(path[0], CanMoveFlags.None));
		f.Actor.At(f.Middle.Object);
		f.Second.SetupGet(x => x.Exit.MaximumSizeToEnter).Returns(SizeCategory.Tiny);
		Assert.IsFalse(f.Actor.CanMove(path[1], CanMoveFlags.None), "A previously planned edge is not execution authority.");
	}

	private sealed class Fixture
	{
		public Mock<IRoom> Origin { get; } = Room(1);
		public Mock<IRoom> Middle { get; } = Room(2);
		public Mock<IRoom> Home { get; } = Room(3);
		public Mock<IRoomExit> First { get; }
		public Mock<IRoomExit> Second { get; }
		public PlanningCharacter Actor { get; }
		public Fixture()
		{
			var world = new Mock<IFuturemud> { DefaultValue = DefaultValue.Mock };
			var body = new Mock<IBody> { DefaultValue = DefaultValue.Mock };
			body.Setup(x => x.CurrentContextualSize(SizeContext.RoomExit)).Returns(SizeCategory.Normal);
			body.SetupGet(x => x.CurrentSpeeds).Returns(new Dictionary<IPositionState, IMoveSpeed>
			{
				[PositionStanding.Instance] = Mock.Of<IMoveSpeed>(),
				[PositionProne.Instance] = Mock.Of<IMoveSpeed>(),
				[PositionSwimming.Instance] = Mock.Of<IMoveSpeed>()
			});
			Actor = PlanningCharacter.Create(world.Object, body.Object, Origin.Object);
			First = Exit(Origin.Object, Middle.Object); Second = Exit(Middle.Object, Home.Object);
			Origin.Setup(x => x.ExitsFor(It.IsAny<IPerceiver>(), It.IsAny<bool>())).Returns([First.Object]);
			Middle.Setup(x => x.ExitsFor(It.IsAny<IPerceiver>(), It.IsAny<bool>())).Returns([Second.Object]);
		}
		private static Mock<IRoom> Room(long id)
		{
			var room = new Mock<IRoom> { DefaultValue = DefaultValue.Mock };
			room.SetupGet(x => x.Id).Returns(id); room.SetupGet(x => x.Location).Returns(room.Object);
			room.SetupGet(x => x.RouteDefinition).Returns((IRouteRoomDefinition)null!);
			room.Setup(x => x.Terrain(It.IsAny<IPerceiver>()).GravityModel).Returns(GravityModel.Normal);
			return room;
		}
		private Mock<IRoomExit> Exit(IRoom origin, IRoom destination)
		{
			var exit = new Mock<IRoomExit> { DefaultValue = DefaultValue.Mock };
			exit.SetupGet(x => x.Origin).Returns(origin); exit.SetupGet(x => x.Destination).Returns(destination);
			exit.SetupGet(x => x.Exit.Door).Returns((MudSharp.GameItems.Interfaces.IDoor)null!);
			exit.SetupGet(x => x.Exit.MaximumSizeToEnter).Returns(SizeCategory.Normal);
			exit.SetupGet(x => x.Exit.MaximumSizeToEnterUpright).Returns(SizeCategory.Normal);
			exit.Setup(x => x.MovementTransition(Actor)).Returns((RoomMovementTransition.GroundToGround, RoomLayer.GroundLevel));
			return exit;
		}
	}

	private sealed class PlanningCharacter : MudSharp.Character.Character
	{
		private PlanningCharacter() : base(null!, null!, true) { }
		public IPositionState TestPosition { get; set; } = null!;
		public override IPositionState PositionState { get => TestPosition; set => TestPosition = value; }
		public static PlanningCharacter Create(IFuturemud world, IBody body, IRoom location)
		{
			var actor = TestObjectFactory.CreateUninitialized<PlanningCharacter>();
			typeof(LateKeywordedInitialisingItem).GetField("<Gameworld>k__BackingField", BindingFlags.NonPublic | BindingFlags.Instance)!.SetValue(actor, world);
			typeof(PerceivedItem).GetField("<EffectHandler>k__BackingField", BindingFlags.NonPublic | BindingFlags.Instance)!.SetValue(actor, Mock.Of<IEffectHandler>());
			actor.Body = body; actor.TestPosition = PositionStanding.Instance; actor.At(location);
			return actor;
		}
		public void At(IRoom room) => Location = room;
	}
}
