#nullable enable

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Character;
using MudSharp.Framework;
using MudSharp.PerceptionEngine;

namespace MudSharp_Unit_Tests;

[TestClass]
public class PossessionVictimContextTests
{
	[TestMethod]
	public void ExecuteCommand_BindsVictimAsSpectatorUntilReleased()
	{
		var victim = new Mock<ICharacter>();
		var possessor = new Mock<ICharacter>();
		var output = new Mock<IOutputHandler>();
		output.Setup(x => x.Send(It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<bool>())).Returns(true);
		var controller = new Mock<ICharacterController>();
		controller.SetupGet(x => x.OutputHandler).Returns(output.Object);
		var context = new PossessionVictimContext(victim.Object, possessor.Object, "start", "end");

		context.AssumeControl(controller.Object);
		var handled = context.ExecuteCommand("north");
		context.LoseControl(controller.Object);

		Assert.IsTrue(handled);
		controller.Verify(x => x.UpdateControlFocus(victim.Object), Times.Once);
		controller.Verify(x => x.UpdateControlFocus(It.Is<ICharacter>(value => value == null)), Times.Once);
		output.Verify(x => x.Send("start", true, false), Times.Once);
		output.Verify(x => x.Send(It.Is<string>(text => text.Contains("cannot command")), true, false), Times.Once);
		output.Verify(x => x.Send("end", true, false), Times.Once);
	}
}
