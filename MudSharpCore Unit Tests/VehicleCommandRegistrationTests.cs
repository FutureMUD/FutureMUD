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
	}
}
