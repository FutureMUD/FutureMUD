#nullable enable

using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Character;
using MudSharp.Construction;
using MudSharp.Construction.Boundary;
using MudSharp.Effects;
using MudSharp.Effects.Concrete;
using MudSharp.Events;
using MudSharp.Framework;
using MudSharp.Movement;
using MudSharp.PerceptionEngine;

namespace MudSharp_Unit_Tests;

[TestClass]
public class CombatEscapeEventTests
{
	[TestMethod]
	[DataRow(true)]
	[DataRow(false)]
	public void OnlyCompletedDepartureReportsVisibleCharactersStillLeftBehind(bool complete)
	{
		var actor = new Mock<ICharacter>();
		var origin = new Mock<IRoom>(); var destination = Mock.Of<IRoom>();
		var room = origin.Object;
		actor.SetupGet(x => x.Location).Returns(() => room);
		var visible = Mock.Of<ICharacter>(x => x.Location == origin.Object);
		var hidden = Mock.Of<ICharacter>(x => x.Location == origin.Object);
		var companion = new Mock<ICharacter>(); var companionRoom = origin.Object;
		companion.SetupGet(x => x.Location).Returns(() => companionRoom);
		origin.SetupGet(x => x.Characters).Returns(new[] { actor.Object, visible, hidden, companion.Object });
		actor.Setup(x => x.CanSee(It.IsAny<IPerceivable>(), It.IsAny<PerceiveIgnoreFlags>())).Returns(true);
		actor.Setup(x => x.CanSee(hidden, It.IsAny<PerceiveIgnoreFlags>())).Returns(false);
		var exit = Mock.Of<IRoomExit>(x => x.Exit == Mock.Of<IExit>() && x.Origin == origin.Object && x.Destination == destination);
		var movement = Mock.Of<IMovement>(x => x.Exit == exit);
		var effect = new PendingCombatEscape(actor.Object, exit);
		actor.Setup(x => x.RemoveEffect(effect, true)).Callback(effect.RemovalEffect);
		if (complete)
		{
			// Ordinary movers notify OnMoved before Enter changes their location.
			actor.Raise(x => x.OnMoved += null, actor.Object, new MoveEventArgs(actor.Object, movement));
			room = destination; companionRoom = destination;
			actor.Raise(x => x.OnLocationChanged += null, actor.Object, exit);
		}
		else actor.Raise(x => x.OnStopMove += null, actor.Object, new MoveEventArgs(actor.Object, movement));
		actor.Verify(x => x.HandleEvent(EventType.CharacterEscapedCombat, It.Is<object[]>(args =>
			ReferenceEquals(args[0], actor.Object) && ReferenceEquals(args[1], origin.Object) &&
			((IEnumerable<ICharacter>)args[2]).Count() == 1 && ReferenceEquals(((IEnumerable<ICharacter>)args[2]).Single(), visible))), complete ? Times.Once : Times.Never);
		// Completion/cancellation releases observers; later movement cannot replay the event.
		actor.Raise(x => x.OnLocationChanged += null, actor.Object, exit);
		actor.Verify(x => x.RemoveEffect(effect, true), Times.Once);
	}
}
