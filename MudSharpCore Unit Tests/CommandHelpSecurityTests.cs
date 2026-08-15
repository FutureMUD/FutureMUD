#nullable enable

using Microsoft.VisualStudio.TestTools.UnitTesting;
using MudSharp.Commands.Modules;
using System.Linq;
using System.Reflection;

namespace MudSharp_Unit_Tests;

[TestClass]
public class CommandHelpSecurityTests
{
	[TestMethod]
	public void ShowCommand_HelpInfo_SeparatesPlayerGuideAndAdministratorTopics()
	{
		var command = typeof(ShowModule).GetMethod("Show", BindingFlags.Static | BindingFlags.NonPublic)!;
		Assert.IsNotNull(command);
		var helpInfo = command.GetCustomAttribute<HelpInfo>()!;
		Assert.IsNotNull(helpInfo);
		var guideHelp = command.GetCustomAttributes<ConditionalHelpInfo>()
			.Single(x => x.PredicateMethodName == "CanSeeGuideShowHelp");

		foreach (var privilegedTopic in new[] { "#3account <account>#0", "#3accounts", "#3character <id>#0",
			         "#3permissions#0", "#3staticconfig <which>#0", "#3staticstring <which>#0" })
		{
			Assert.IsFalse(helpInfo.DefaultHelp.Contains(privilegedTopic));
			Assert.IsFalse(guideHelp.HelpText.Contains(privilegedTopic));
			Assert.IsTrue(helpInfo.AdminHelp.Contains(privilegedTopic));
		}
	}
}
