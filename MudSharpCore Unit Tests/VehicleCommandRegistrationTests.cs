#nullable enable

using Microsoft.VisualStudio.TestTools.UnitTesting;
using MudSharp.Commands.Trees;

namespace MudSharp_Unit_Tests;

[TestClass]
public class VehicleCommandRegistrationTests
{
	[TestMethod]
	public void ActorCommandTree_RegistersEmbarkWithoutBoardAliasCollision()
	{
		var commands = ActorCommandTree.Instance.Commands.TCommands;

		Assert.IsTrue(commands.ContainsKey("embark"));
		Assert.IsTrue(commands.ContainsKey("hitch"));
		Assert.IsTrue(commands.ContainsKey("unhitch"));
		Assert.IsTrue(commands.ContainsKey("vehiclestatus"));
		Assert.IsTrue(commands.ContainsKey("vehiclecontrol"));
		Assert.IsTrue(commands.ContainsKey("takecontrol"));
		Assert.IsTrue(commands.ContainsKey("releasecontrol"));
	}
}
