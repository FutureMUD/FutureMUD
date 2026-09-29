#nullable enable

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Events;
using MudSharp.NPC.AI;

namespace MudSharp_Unit_Tests.NPC;

[TestClass]
public class AIEventDispatcherTests
{
	[TestMethod]
	public void HandleEvent_ConsumedByPrimary_StillNotifiesLaterObserverAndPreservesOrdinaryPriority()
	{
		var primary = new Mock<IArtificialIntelligence>();
		var secondary = new Mock<IArtificialIntelligence>();
		var observer = new Mock<IEventObserverAI>();
		primary.Setup(x => x.HandleEvent(EventType.MinuteTick, It.IsAny<object[]>())).Returns(true);
		observer.Setup(x => x.HandlesEvent(EventType.MinuteTick)).Returns(true);
		observer.Setup(x => x.HandleEvent(EventType.MinuteTick, It.IsAny<object[]>())).Returns(true);

		Assert.IsTrue(AIEventDispatcher.HandleEvent([primary.Object, observer.Object, secondary.Object],
			EventType.MinuteTick, new object()));
		observer.Verify(x => x.HandleEvent(EventType.MinuteTick, It.IsAny<object[]>()), Times.Once);
		primary.Verify(x => x.HandleEvent(EventType.MinuteTick, It.IsAny<object[]>()), Times.Once);
		secondary.Verify(x => x.HandleEvent(It.IsAny<EventType>(), It.IsAny<object[]>()), Times.Never);
	}

	[TestMethod]
	public void HandleEvent_Observer_DoesNotConsumeOrReceiveUnsubscribedEvents()
	{
		var observer = new Mock<IEventObserverAI>();
		observer.Setup(x => x.HandlesEvent(EventType.MinuteTick)).Returns(true);
		observer.Setup(x => x.HandleEvent(It.IsAny<EventType>(), It.IsAny<object[]>())).Returns(true);
		Assert.IsFalse(AIEventDispatcher.HandleEvent([observer.Object], EventType.MinuteTick));
		Assert.IsFalse(AIEventDispatcher.HandleEvent([observer.Object], EventType.TenSecondTick));
		observer.Verify(x => x.HandleEvent(EventType.MinuteTick, It.IsAny<object[]>()), Times.Once);
		observer.Verify(x => x.HandleEvent(EventType.TenSecondTick, It.IsAny<object[]>()), Times.Never);
	}
}
