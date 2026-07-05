using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Accounts;
using MudSharp.Character;
using MudSharp.Commands;
using MudSharp.Commands.Modules;
using MudSharp.Framework;
using MudSharp.Help;
using MudSharp.PerceptionEngine;

#nullable enable

namespace MudSharp_Unit_Tests.Commands;

[TestClass]
public class CommandHelpInfoTests
{
	[TestMethod]
	public void CheckHelp_DefaultActorUsesDefaultHelp()
	{
		var actor = Character(administrator: false);
		var output = Output(out var sent);
		var help = new CommandHelpInfo("sample", "default help", AutoHelp.HelpArg, "admin help", false);

		var shown = help.CheckHelp(actor.Object, "sample help", output.Object);

		Assert.IsTrue(shown);
		StringAssert.Contains(sent.Last!, "default help");
	}

	[TestMethod]
	public void CheckHelp_ConditionalActorUsesConditionalHelp()
	{
		var actor = Character(administrator: false);
		var output = Output(out var sent);
		var help = new CommandHelpInfo("sample", "default help", AutoHelp.HelpArg, "admin help", false)
			.AddConditionalHelp(_ => true, "conditional help");

		var shown = help.CheckHelp(actor.Object, "sample help", output.Object);

		Assert.IsTrue(shown);
		StringAssert.Contains(sent.Last!, "conditional help");
		Assert.IsFalse(sent.Last!.Contains("default help", StringComparison.OrdinalIgnoreCase));
	}

	[TestMethod]
	public void CheckHelp_AdminHelpWinsOverConditionalHelp()
	{
		var actor = Character(administrator: true);
		var output = Output(out var sent);
		var help = new CommandHelpInfo("sample", "default help", AutoHelp.HelpArg, "admin help", false)
			.AddConditionalHelp(_ => true, "conditional help");

		var shown = help.CheckHelp(actor.Object, "sample help", output.Object);

		Assert.IsTrue(shown);
		StringAssert.Contains(sent.Last!, "admin help");
		Assert.IsFalse(sent.Last!.Contains("conditional help", StringComparison.OrdinalIgnoreCase));
	}

	[TestMethod]
	public void DisplayHelpFile_UsesSameConditionalHelpSelection()
	{
		var actor = Character(administrator: false);
		var help = new CommandHelpInfo("sample", "default help", AutoHelp.HelpArg, "admin help", false)
			.AddConditionalHelp(_ => true, "conditional help");

		var text = ((IHelpInformation)help).DisplayHelpFile(actor.Object);

		StringAssert.Contains(text, "conditional help");
		Assert.IsFalse(text.Contains("default help", StringComparison.OrdinalIgnoreCase));
	}

	[TestMethod]
	public void ConditionalHelpInfoAttribute_ResolvesPredicateMethod()
	{
		ConditionalHelpTestModule.UseConditionalHelp = true;
		var actor = Character(administrator: false);
		var output = Output(out var sent);
		var module = new ConditionalHelpTestModule();
		var help = module.Commands.TCommands.Values.Select(x => x.HelpInfo).Single(x => x is not null)!;

		var shown = help.CheckHelp(actor.Object, "conditional help", output.Object);

		Assert.IsTrue(shown);
		StringAssert.Contains(sent.Last!, "conditional help");
	}

	[TestMethod]
	public void ConditionalHelpInfoAttribute_InvalidPredicateFailsModuleCompile()
	{
		Assert.ThrowsException<ApplicationException>(() => new InvalidConditionalHelpTestModule());
	}

	[TestMethod]
	public void ManipulationModule_InstallAndUninstallExposeBuiltInHelp()
	{
		var commands = ManipulationModule.Instance.Commands.TCommands;

		var installHelp = commands["install"].HelpInfo;
		Assert.IsNotNull(installHelp);
		Assert.AreEqual(AutoHelp.HelpArgOrNoArg, installHelp.AutoHelpSetting);
		StringAssert.Contains(installHelp.DefaultHelp, "#3install <door> <exit> [inwards|outwards]#0");
		StringAssert.Contains(installHelp.DefaultHelp, "#3install <lock> <door|container|exit>#0");

		var uninstallHelp = commands["uninstall"].HelpInfo;
		Assert.IsNotNull(uninstallHelp);
		Assert.AreEqual(AutoHelp.HelpArgOrNoArg, uninstallHelp.AutoHelpSetting);
		StringAssert.Contains(uninstallHelp.DefaultHelp, "#3uninstall <door|exit>#0");
		StringAssert.Contains(uninstallHelp.DefaultHelp, "#3uninstall <door|container|exit> [<lock>]#0");
	}

	[TestMethod]
	public void StorytellerModule_CommandsExposeDetailedBuiltInHelp()
	{
		var commands = StorytellerModule.Instance.Commands.TCommands;
		var missingHelp = commands
		                  .Where(x => x.Value.HelpInfo is null)
		                  .Select(x => x.Key)
		                  .OrderBy(x => x)
		                  .ToList();
		Assert.AreEqual(0, missingHelp.Count,
			$"Storyteller commands without help: {string.Join(", ", missingHelp)}");

		var helpTexts = commands.Values
		                        .Select(x => x.HelpInfo)
		                        .Where(x => x is not null)
		                        .Distinct()
		                        .ToList();
		var helpWithoutSyntax = helpTexts
		                        .Where(x => !x!.DefaultHelp.Contains("#3", StringComparison.Ordinal))
		                        .Select(x => x!.HelpName)
		                        .OrderBy(x => x)
		                        .ToList();
		Assert.AreEqual(0, helpWithoutSyntax.Count,
			$"Storyteller help missing syntax examples: {string.Join(", ", helpWithoutSyntax)}");

		var rigidHeadingHelp = helpTexts
		                       .Where(x => x!.DefaultHelp.Contains("General Usage:", StringComparison.Ordinal) ||
		                                   x.DefaultHelp.Contains("Edge Cases:", StringComparison.Ordinal) ||
		                                   x.DefaultHelp.StartsWith("Syntax:", StringComparison.Ordinal) ||
		                                   x.DefaultHelp.Contains("\nSyntax:", StringComparison.Ordinal))
		                       .Select(x => x!.HelpName)
		                       .OrderBy(x => x)
		                       .ToList();
		Assert.AreEqual(0, rigidHeadingHelp.Count,
			$"Storyteller help should not use rigid section headings: {string.Join(", ", rigidHeadingHelp)}");

		var forceHelp = commands["force"].HelpInfo!;
		StringAssert.Contains(forceHelp.DefaultHelp, "#3force npcshere <command>#0");
		Assert.IsFalse(forceHelp.DefaultHelp.Contains("npchere", StringComparison.OrdinalIgnoreCase));

		Assert.AreEqual(AutoHelp.HelpArg, commands["recentspeech"].HelpInfo!.AutoHelpSetting);
		Assert.AreEqual(AutoHelp.HelpArg, commands["drawings"].HelpInfo!.AutoHelpSetting);
		Assert.AreEqual(AutoHelp.HelpArg, commands["writings"].HelpInfo!.AutoHelpSetting);
		Assert.AreEqual(AutoHelp.HelpArg, commands["decay"].HelpInfo!.AutoHelpSetting);

		var scriptedEventHelp = commands["scriptedevent"].HelpInfo!;
		Assert.IsFalse(scriptedEventHelp.DefaultHelp.Contains("applyall", StringComparison.OrdinalIgnoreCase));
		StringAssert.Contains(scriptedEventHelp.DefaultHelp, "#3scriptedevent set text <number>#0");
	}

	private static Mock<ICharacter> Character(bool administrator)
	{
		var account = new Mock<IAccount>();
		account.SetupGet(x => x.InnerLineFormatLength).Returns(120);
		account.SetupGet(x => x.LineFormatLength).Returns(120);
		account.SetupGet(x => x.UseUnicode).Returns(false);
		account.SetupGet(x => x.TimeZone).Returns(TimeZoneInfo.Utc);
		var actor = new Mock<ICharacter>();
		actor.SetupGet(x => x.Account).Returns(account.Object);
		actor.Setup(x => x.IsAdministrator(It.IsAny<PermissionLevel>())).Returns(administrator);
		actor.SetupGet(x => x.InnerLineFormatLength).Returns(120);
		actor.SetupGet(x => x.LineFormatLength).Returns(120);
		return actor;
	}

	private static Mock<IOutputHandler> Output(out CapturedOutput sent)
	{
		var captured = new CapturedOutput();
		var output = new Mock<IOutputHandler>();
		output.Setup(x => x.Send(It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<bool>()))
		      .Callback<string, bool, bool>((text, _, _) => captured.Last = text)
		      .Returns(true);
		sent = captured;
		return output;
	}

	private sealed class CapturedOutput
	{
		public string? Last { get; set; }
	}

	private sealed class ConditionalHelpTestModule : Module<ICharacter>
	{
		public ConditionalHelpTestModule() : base("ConditionalHelpTest")
		{
		}

		public static bool UseConditionalHelp { get; set; }

		private static bool CanSeeConditionalHelp(ICharacter actor)
		{
			return UseConditionalHelp;
		}

		[PlayerCommand("Conditional", "conditional")]
		[HelpInfo("conditional", "default help", AutoHelp.HelpArg)]
		[ConditionalHelpInfo(nameof(CanSeeConditionalHelp), "conditional help")]
		private static void Conditional(ICharacter actor, string command)
		{
		}
	}

	private sealed class InvalidConditionalHelpTestModule : Module<ICharacter>
	{
		public InvalidConditionalHelpTestModule() : base("InvalidConditionalHelpTest")
		{
		}

		[PlayerCommand("InvalidConditional", "invalidconditional")]
		[HelpInfo("invalidconditional", "default help", AutoHelp.HelpArg)]
		[ConditionalHelpInfo(nameof(BadPredicate), "conditional help")]
		private static void Conditional(ICharacter actor, string command)
		{
		}

		private static string BadPredicate(ICharacter actor)
		{
			return "not a bool";
		}
	}
}
