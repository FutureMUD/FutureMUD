#nullable enable
using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MudSharp.Accounts;
using MudSharp.Commands;
using MudSharp.Commands.Modules;
using MudSharp.Framework;

namespace MudSharp_Unit_Tests.Commands;

[TestClass]
[DoNotParallelize]
public class BuilderHelpContractTests
{
	[TestMethod]
	public void PublishedTimezoneExamples_ReplayThroughRegisteredCommandAndDelegatedSetters()
	{
		var fixture = BuilderWorkflowImprovementTests.TimezoneFixture();
		var command = TimeModule.Instance.Commands.TCommands["timezone"];
		var help = (CommandHelpInfo)command.HelpInfo;
		Assert.IsTrue(help.Examples.Count > 0, "The executable contract requires published examples.");
		foreach (var example in help.Examples)
		{
			var input = new StringStack(example);
			var registered = TimeModule.Instance.Commands.TCommands[input.PopSpeech()];
			Assert.IsTrue(registered.Execute(fixture.Actor.Object, example, permissionLevel: PermissionLevel.Admin));
		}
		Assert.AreEqual("Port Standard Time", fixture.Zone.Description);
		Assert.AreEqual(2, fixture.Zone.OffsetHours);
		Assert.AreEqual(30, fixture.Zone.OffsetMinutes);
		Assert.AreEqual(0, fixture.Effects.Count, "The documented close must release selection without cancelling edits.");
	}

	[TestMethod]
	public void PublishedTimezoneExamples_DenyInsufficientPermissionBeforeHandlersRun()
	{
		var fixture = BuilderWorkflowImprovementTests.TimezoneFixture();
		var command = TimeModule.Instance.Commands.TCommands["timezone"];
		foreach (var example in ((CommandHelpInfo)command.HelpInfo).Examples)
			Assert.IsFalse(command.Execute(fixture.Actor.Object, example, permissionLevel: PermissionLevel.Any));
		Assert.AreEqual("Old description", fixture.Zone.Description);
		Assert.AreEqual(0, fixture.Zone.OffsetHours);
		Assert.AreEqual(0, fixture.Effects.Count);
	}

	[TestMethod]
	[DataRow("timezone set offset")]
	[DataRow("timezone set offset invalid")]
	[DataRow("timezone set offset 2 invalid")]
	public void RegisteredDelegatedSet_MalformedOffsetsLeaveStateUntouched(string input)
	{
		var fixture = BuilderWorkflowImprovementTests.TimezoneFixture();
		var command = TimeModule.Instance.Commands.TCommands["timezone"];
		var firstExample = ((CommandHelpInfo)command.HelpInfo).Examples.First();
		command.Execute(fixture.Actor.Object, firstExample, permissionLevel: PermissionLevel.Admin);
		var output = BuilderWorkflowImprovementTests.Capture(fixture.Actor);
		command.Execute(fixture.Actor.Object, input, permissionLevel: PermissionLevel.Admin);
		Assert.AreEqual(0, fixture.Zone.OffsetHours);
		Assert.AreEqual(0, fixture.Zone.OffsetMinutes);
		Assert.IsTrue(output.Count > 0);
	}

	[TestMethod]
	public void PublishedWorkflowHelp_ResetsColourOnEveryColouredLine()
	{
		var helps = new[]
		{
			(CommandHelpInfo)TimeModule.Instance.Commands.TCommands["timezone"].HelpInfo,
			(CommandHelpInfo)BuilderModule.Instance.Commands.TCommands["builderrefs"].HelpInfo,
			(CommandHelpInfo)BuilderModule.Instance.Commands.TCommands["buildereditors"].HelpInfo
		};
		foreach (var help in helps)
		foreach (var line in help.DefaultHelp.SubstituteANSIColour().Split('\n'))
		{
			var sequences = Regex.Matches(line, "\u001b\\[([0-9;]*)m");
			if (sequences.Count == 0) continue;
			Assert.IsTrue(sequences[^1].Value == Telnet.RESETALL || sequences[^1].Value == Telnet.RESET || sequences[^1].Groups[1].Value == "", $"{help.HelpName} leaks colour across a line boundary: {line.StripANSIColour()}");
		}
	}
}
