#nullable enable

using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;
using System.Linq;

namespace MudSharp_Unit_Tests;

[TestClass]
public class LegalCommandPresentationTests
{
	[TestMethod]
	public void EngageLawyer_RemandsCellsAndSelfTarget_ShouldBeSupported()
	{
		string source = File.ReadAllText(GetCoreSourcePath("Commands", "Modules", "CrimeModule.cs"));
		string block = ExtractBlock(source, "protected static void EngageLawyer", "[PlayerCommand(\"PayFine\"");

		StringAssert.Contains(block, "ss.PopSpeech();");
		StringAssert.Contains(block, "x.IsInRemandCell(actor)");
		StringAssert.Contains(block, ".Where(x => !x.AffectedBy<HasLegalCounsel>())");
		StringAssert.Contains(block, "in a remand cell");
		StringAssert.Contains(block, "\"ENGAGELAWYER LIST\".MXPSend(\"engagelawyer list\")");
		StringAssert.Contains(block, "PrisonerBelongingsBundles(jurisdiction, actor)");
		StringAssert.Contains(block, "actor.Body.ExternalItems.Concat(remandBelongings)");
		StringAssert.Contains(block, "remand belongings");
	}

	[TestMethod]
	public void TrialDocket_LargeChargeLists_ShouldUseBoundedSummaryAndWrappedDetails()
	{
		string source = File.ReadAllText(GetCoreSourcePath("Commands", "Modules", "CrimeModule.cs"));
		string block = ExtractBlock(source, "private static void TrialDocket", "private static void TrialSummon");

		StringAssert.Contains(source, "private static string DescribeTrialDocketChargeSummary");
		StringAssert.Contains(source, "private static string DescribeTrialDocketCrimeIds");
		StringAssert.Contains(source, "const int maximumVisibleIds = 8;");
		StringAssert.Contains(block, "\"Open\",");
		StringAssert.Contains(block, "\"Charge Summary\",");
		StringAssert.Contains(block, "Charge IDs by Defendant:");
		StringAssert.Contains(block, ".Wrap(actor.InnerLineFormatLength, \"\\t\")");
		Assert.IsFalse(block.Contains("crimes.Select(x => x.Id.ToStringN0(actor)).ListToString()", StringComparison.Ordinal));
		Assert.IsFalse(block.Contains("crimes.Select(x => x.Name).Distinct().ListToString()", StringComparison.Ordinal));
	}

	[TestMethod]
	public void ShowEnforcers_ShouldReportRosterEligibilityAndUsePatrolPoolRules()
	{
		string legalModuleSource = File.ReadAllText(GetCoreSourcePath("Commands", "Modules", "LegalModule.cs"));
		string patrolControllerSource = File.ReadAllText(GetCoreSourcePath("RPG", "Law", "PatrolController.cs"));

		StringAssert.Contains(legalModuleSource, "[PlayerCommand(\"ShowEnforcers\", \"showenforcers\")]");
		StringAssert.Contains(legalModuleSource, "case \"roster\":");
		StringAssert.Contains(legalModuleSource, "actor.Gameworld.Zones.GetByIdOrName(targetText)");
		StringAssert.Contains(legalModuleSource, "private static string WhyCannotJoinPatrolPool");
		StringAssert.Contains(legalModuleSource, "does not have an Enforcer AI");
		StringAssert.Contains(legalModuleSource, "\"Duty\"");
		StringAssert.Contains(legalModuleSource, "\"Pool\"");
		StringAssert.Contains(legalModuleSource, "\"Authority\"");
		StringAssert.Contains(legalModuleSource, "Patrol Pool Details:");
		Assert.IsTrue(legalModuleSource.Contains("Location {item.Location}; Patrol {item.Patrol}; Status {item.Status}", StringComparison.Ordinal));
		Assert.IsTrue(patrolControllerSource.Contains("npc.AIs.Any(y => y is EnforcerAI)", StringComparison.Ordinal));
	}

	[TestMethod]
	public void RequestExecution_ShouldBringForwardDeathSentenceAndLaunchExecutionPatrol()
	{
		string crimeModuleSource = File.ReadAllText(GetCoreSourcePath("Commands", "Modules", "CrimeModule.cs"));
		string effectSource = File.ReadAllText(GetCoreSourcePath("Effects", "Concrete", "AwaitingExecution.cs"));
		string controllerInterfaceSource = File.ReadAllText(GetLibrarySourcePath("RPG", "Law", "IPatrolController.cs"));
		string controllerSource = File.ReadAllText(GetCoreSourcePath("RPG", "Law", "PatrolController.cs"));

		StringAssert.Contains(crimeModuleSource, "[PlayerCommand(\"RequestExecution\", \"requestexecution\")]");
		StringAssert.Contains(crimeModuleSource, "effect.BringForwardTo(now)");
		StringAssert.Contains(crimeModuleSource, "jurisdiction.PatrolController.TryBeginPatrol(route)");
		StringAssert.Contains(crimeModuleSource, "x.PatrolStrategy.Name == \"ExecutionPatrol\"");
		StringAssert.Contains(effectSource, "public bool BringForwardTo(MudDateTime executionDate)");
		StringAssert.Contains(controllerInterfaceSource, "bool TryBeginPatrol(IPatrolRoute route);");
		StringAssert.Contains(controllerSource, "public bool TryBeginPatrol(IPatrolRoute route)");
		StringAssert.Contains(controllerSource, "AvailableEnforcerPool()");
		StringAssert.Contains(controllerSource, "PatrolLaunchPrerequisitesReady()");
	}

	private static string ExtractBlock(string source, string startMarker, string endMarker)
	{
		int start = source.IndexOf(startMarker, StringComparison.Ordinal);
		Assert.IsTrue(start >= 0, $"Could not find start marker {startMarker}");

		int end = source.IndexOf(endMarker, start, StringComparison.Ordinal);
		Assert.IsTrue(end > start, $"Could not find end marker {endMarker}");

		return source[start..end];
	}

	private static string GetCoreSourcePath(params string[] segments)
	{
		return Path.GetFullPath(Path.Combine(
			new[]
			{
				AppContext.BaseDirectory,
				"..",
				"..",
				"..",
				"..",
				"MudSharpCore"
			}.Concat(segments).ToArray()));
	}

	private static string GetLibrarySourcePath(params string[] segments)
	{
		return Path.GetFullPath(Path.Combine(
			new[]
			{
				AppContext.BaseDirectory,
				"..",
				"..",
				"..",
				"..",
				"FutureMUDLibrary"
			}.Concat(segments).ToArray()));
	}
}
