#nullable enable

using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Character;
using MudSharp.Framework;
using MudSharp.Framework.Save;
using MudSharp.NPC;

namespace MudSharp_Unit_Tests;

[TestClass]
public class NpcArchiveRuntimeTests
{
	[TestMethod]
	public void Dispose_MonitoredNpcController_DetachesBothDirectionsAndOutput()
	{
		var actor = new Mock<ICharacter>();
		var controller = new NPCController();
		controller.UpdateControlFocus(actor.Object);
		controller.OutputHandler.Register(actor.Object);
		var monitor = new Mock<IMonitor>();
		monitor.Setup(x => x.RemoveObservee(controller)).Callback(() => ((IMonitorable)controller).RemoveObserver(monitor.Object));
		((IMonitorable)controller).AddObserver(monitor.Object);
		var observed = new Mock<IMonitorable>();
		controller.AddObservee(observed.Object);
		controller.Dispose();
		controller.HandleCommand("look");
		controller.CuePrompt();
		controller.Dispose();
		Assert.IsNull(controller.Actor);
		Assert.IsNull(controller.OutputHandler.Perceiver);
		actor.Verify(x => x.LoseControl(controller), Times.Once);
		actor.Verify(x => x.ExecuteCommand(It.IsAny<string>()), Times.Never);
		monitor.Verify(x => x.RemoveObservee(controller), Times.Once);
		observed.Verify(x => x.RemoveObserver(controller), Times.Once);
	}

	[DataTestMethod]
	[DataRow(false)]
	[DataRow(true)]
	public void HandleCommand_ContextDisposesController_ReturnsWithoutAdvancing(bool inSubContext)
	{
		var actor = new Mock<ICharacter>(); var controller = new NPCController();
		controller.UpdateControlFocus(actor.Object);
		if (inSubContext) actor.Setup(x => x.HandleSubContext("die")).Callback(controller.Dispose).Returns(false);
		else actor.Setup(x => x.ExecuteCommand("die")).Callback(controller.Dispose).Returns(true);
		controller.HandleCommand("die");
		Assert.IsNull(controller.Actor);
		actor.VerifyGet(x => x.NextContext, Times.Never);
	}

	[TestMethod]
	public void Abort_DuplicateAndDelayedSaves_RemovesEveryOwnedReferenceOnly()
	{
		var manager = new DelayedSaveManager();
		var owned = Mock.Of<ISaveable>(); var foreign = Mock.Of<ISaveable>();
		manager.Add(owned); manager.Add(owned); manager.Add(foreign);
		manager.Delay(owned); manager.Delay(foreign);
		manager.Abort(owned);
		Assert.IsFalse(manager.IsQueued(owned));
		Assert.IsTrue(manager.IsQueued(foreign));
		manager.Abort(foreign);
		Assert.IsFalse(manager.IsQueued(foreign));
	}

	private sealed class DelayedSaveManager : SaveManager
	{
		public void Delay(ISaveable value) => _delayedSaveStack.Add(value);
	}
}
