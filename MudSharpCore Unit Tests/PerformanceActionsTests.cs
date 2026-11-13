#nullable enable

using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Character;
using MudSharp.Construction;
using MudSharp.Construction.Boundary;
using MudSharp.Effects.Concrete;
using MudSharp.Movement;
using MudSharp.RPG.Performances;

namespace MudSharp_Unit_Tests;

[TestClass]
public class PerformanceActionsTests
{
	[TestMethod]
	public void Try_SpeechVerification_DoesNotExecuteForPlayer()
	{
		var performer = new Mock<ICharacter>(MockBehavior.Strict);

		bool succeeded = PerformanceActions.Try("speak", performer.Object, null, null, null, null,
			"The reading", Guid.NewGuid(), false, out string reason);

		Assert.IsFalse(succeeded);
		StringAssert.Contains(reason, "Waiting for the participant");
		performer.VerifyNoOtherCalls();
	}

	[TestMethod]
	public void Try_UnknownVerb_ReturnsActionableFailure()
	{
		var performer = new Mock<ICharacter>(MockBehavior.Strict);

		bool succeeded = PerformanceActions.Try("custom-action", performer.Object, null, null, null, null,
			string.Empty, Guid.NewGuid(), true, out string reason);

		Assert.IsFalse(succeeded);
		StringAssert.Contains(reason, "not supported");
		performer.VerifyNoOtherCalls();
	}

	[TestMethod]
	public void BlockedLegWaitsWithoutStartingAnotherMove()
	{
		var actor = new Mock<ICharacter>();
		var origin = Mock.Of<IRoom>();
		var destination = Mock.Of<IRoom>();
		var exit = new Mock<IRoomExit>();
		exit.SetupGet(x => x.Origin).Returns(origin);
		exit.SetupGet(x => x.Destination).Returns(destination);
		actor.SetupGet(x => x.Location).Returns(origin);
		actor.Setup(x => x.CanMove(exit.Object, CanMoveFlags.None)).Returns(CanMoveResponse.True);
		actor.Setup(x => x.CanCross(exit.Object)).Returns((false, null!));

		Assert.IsFalse(PerformanceActions.Try("travel", actor.Object, destination, exit.Object, null, null,
			"", Guid.NewGuid(), true, out var reason));
		StringAssert.Contains(reason, "blocked");
		actor.Verify(x => x.Move(exit.Object, null, false), Times.Never);
	}

	[TestMethod]
	[DataRow(true, MovementPhase.OriginalRoom, false, true)]
	[DataRow(false, MovementPhase.OriginalRoom, false, false)]
	[DataRow(true, MovementPhase.NewRoom, false, false)]
	[DataRow(true, MovementPhase.OriginalRoom, true, false)]
	public void ReleasingAClaimStopsOnlyItsOwnSoloDeparture(bool sameMove, MovementPhase phase, bool shared, bool shouldCancel)
	{
		var actor = new Mock<ICharacter>();
		var move = new Mock<IMovement>();
		move.SetupGet(x => x.Phase).Returns(phase);
		move.SetupGet(x => x.CharacterMovers).Returns(shared ? [actor.Object, Mock.Of<ICharacter>()] : [actor.Object]);
		actor.SetupGet(x => x.Movement).Returns(sameMove ? move.Object : Mock.Of<IMovement>());
		var claim = new PerformanceClaimEffect(actor.Object, Guid.NewGuid()) { StartedMovement = move.Object };
		claim.RemovalEffect();
		move.Verify(x => x.Cancel(), shouldCancel ? Times.Once : Times.Never);
		move.Verify(x => x.CancelForMoverOnly(actor.Object, false), Times.Never);
		Assert.IsNull(claim.StartedMovement);
	}

	[TestMethod]
	public void ExpectedArrivalRequiresTheDirectedBoundLeg()
	{
		var plan = new PerformancePlan();
		plan.Places["square"] = new PerformanceMark(1, RoomLayer.GroundLevel);
		plan.Places["shrine"] = new PerformanceMark(2, RoomLayer.GroundLevel);
		plan.CurrentMarks["leader"] = "square";
		plan.Exits["gate"] = 4;
		var cue = new PerformanceCue { Role = "leader", Verb = PerformanceVerb.Travel, Place = "shrine", Target = "gate" };
		var actor = new Mock<ICharacter>();
		var gate = new Mock<IRoomExit> { DefaultValue = DefaultValue.Mock };
		gate.SetupGet(x => x.Exit.Id).Returns(4);
		gate.SetupGet(x => x.Origin.Id).Returns(1);
		gate.SetupGet(x => x.Destination.Id).Returns(2);
		actor.SetupGet(x => x.Location).Returns(gate.Object.Destination);
		actor.SetupGet(x => x.RoomLayer).Returns(RoomLayer.GroundLevel);
		Assert.IsTrue(PerformanceService.ExpectedMove(plan, cue, "leader", actor.Object, gate.Object));
		Assert.IsFalse(PerformanceService.ExpectedMove(plan, cue, "leader", actor.Object, null));
		gate.SetupGet(x => x.Exit.Id).Returns(5);
		Assert.IsFalse(PerformanceService.ExpectedMove(plan, cue, "leader", actor.Object, gate.Object));
	}

	[TestMethod]
	public void FailureSpeakerWithoutALanguageDoesNotConsumeTheAnnouncement()
	{
		var actor = new Mock<ICharacter> { DefaultValue = DefaultValue.Mock };
		actor.Setup(x => x.Gameworld.GetStaticInt("MaximumSayLength")).Returns(200);
		actor.SetupGet(x => x.Body.CurrentLanguage).Returns((MudSharp.Communication.Language.ILanguage)null!);
		Assert.IsFalse(PerformanceActions.Try("speak", actor.Object, null, null, null, null,
			"We must pause here.", Guid.NewGuid(), true, out var reason));
		StringAssert.Contains(reason, "language");
		Mock.Get(actor.Object.Body).Verify(x => x.Say(null, "We must pause here.", null), Times.Never);
	}
}
