#nullable enable

using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Commands.Trees;
using MudSharp.Framework;

namespace MudSharp_Unit_Tests;

[TestClass]
public class ChargedMagicDeviceCommandTests
{
	[TestMethod]
	public void MagicDevice_OrdinaryCommandTrees_DispatchShowAndDepletedRefusal()
	{
		var fixture = new ChargedMagicDeviceTests.Fixture();
		var actor = fixture.F.Actor;
		actor.Setup(x => x.TargetItem("wand")).Returns(fixture.Item.Object);
		var output = new List<string>();
		Mock.Get(actor.Object.OutputHandler)
			.Setup(x => x.Send(It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<bool>()))
			.Callback<string, bool, bool>((text, _, _) => output.Add(text));
		foreach (var tree in new ActorCommandTree[]
			{
				PlayerCommandTree.Instance, NPCCommandTree.Instance, GuideCommandTree.Instance,
				AdminCommandTree.StandardAdminCommandTree
			})
		{
			output.Clear();
			Assert.IsTrue(tree.Commands.Execute(actor.Object, "magicdevice show wand", actor.Object.State,
				tree.PermissionLevel, actor.Object.OutputHandler));
			Assert.IsTrue(output.Any(x => x.Contains("0/5 charges")), string.Join("\n", output));
		}

		output.Clear();
		Assert.IsTrue(PlayerCommandTree.Instance.Commands.Execute(actor.Object, "magicdevice charged wand self",
			actor.Object.State, MudSharp.Accounts.PermissionLevel.Player, actor.Object.OutputHandler));
		Assert.IsTrue(output.Any(x => x.Contains("Choose focus mode explicitly")), string.Join("\n", output));
		Assert.AreEqual(0, fixture.Device.Charges);
	}
}
