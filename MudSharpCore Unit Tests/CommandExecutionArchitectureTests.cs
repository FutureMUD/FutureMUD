#nullable enable

using System;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MudSharp_Unit_Tests;

[TestClass]
[TestCategory("SourceCoupled")]
public class CommandExecutionArchitectureTests
{
	[TestMethod]
	public void AuthoredCommandPaths_UseTheSecureExecutor()
	{
		var guardedSources = new[]
		{
			("MudSharpCore", "FutureProg", "Statements", "Force.cs"),
			("MudSharpCore", "FutureProg", "Statements", "Delay.cs"),
			("MudSharpCore", "Effects", "Concrete", "Dreaming.cs"),
			("MudSharpCore", "Framework", "Scheduling", "Schedules.cs"),
			("MudSharpCore", "Magic", "SpellEffects", "MagicPhase3Effects.cs")
		};

		foreach (var parts in guardedSources)
		{
			var source = File.ReadAllText(GetSourcePath(parts.Item1, parts.Item2, parts.Item3, parts.Item4));
			Assert.IsTrue(source.Contains("CommandExecutionGuards.", StringComparison.Ordinal),
				$"{parts.Item4} should use CommandExecutionGuards for authored forced commands.");
		}

		var hookSource = File.ReadAllText(GetSourcePath("MudSharpCore", "Events", "Hooks", "CommandHook.cs"));
		StringAssert.Contains(hookSource, "if (target is ICharacter character)");
		StringAssert.Contains(hookSource, "CommandExecutionGuards.ExecuteForcedCommand(character, command);");
	}

	private static string GetSourcePath(params string[] parts)
	{
		return Path.GetFullPath(Path.Combine(
			AppContext.BaseDirectory,
			"..",
			"..",
			"..",
			"..",
			Path.Combine(parts)));
	}
}
