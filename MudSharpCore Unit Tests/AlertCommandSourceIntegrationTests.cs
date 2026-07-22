#nullable enable

using Microsoft.VisualStudio.TestTools.UnitTesting;
using MudSharp.Communication;
using MudSharp.Events;
using MudSharp.Framework;
using System;
using System.IO;

namespace MudSharp_Unit_Tests;

[TestClass]
public class AlertCommandSourceIntegrationTests
{
	[TestMethod]
	public void StaticDefaults_IncludeAlertEmotes()
	{
		Assert.IsTrue(DefaultStaticSettings.DefaultStaticStrings.ContainsKey("DefaultAlertEmote"));
		Assert.IsTrue(DefaultStaticSettings.DefaultStaticStrings.ContainsKey("DefaultDistantAlertEmote"));
		Assert.AreEqual("@ whistle|whistles a sharp, loud alert.",
			DefaultStaticSettings.DefaultStaticStrings["DefaultAlertEmote"]);
		Assert.AreEqual("A sharp, loud alert can be heard {0}.",
			DefaultStaticSettings.DefaultStaticStrings["DefaultDistantAlertEmote"]);
	}

	[TestMethod]
	public void EventType_CharacterAlertHeard_IsAppendOnlyRegistered()
	{
		Assert.AreEqual(129, (int)EventType.CharacterAlertHeard);
	}

	[TestMethod]
	public void CommunicationModule_ExposesAlertCommandAndCustomisation()
	{
		var source = File.ReadAllText(GetSourcePath("MudSharpCore", "Commands", "Modules",
			"CommunicationsModule.cs"));

		StringAssert.Contains(source, "[PlayerCommand(\"Alert\", \"alert\")]");
		StringAssert.Contains(source, "AlertUtilities.DoAlert(actor);");
		StringAssert.Contains(source, "AlertUtilities.DoAlert(actor, emoteText);");
		StringAssert.Contains(source, "actor.CustomAlertEmote = emoteText;");
		StringAssert.Contains(source, "actor.CustomDistantAlertEmote = emoteText;");
		StringAssert.Contains(source, "AlertUtilities.ValidateStoredAlertEmote(emoteText, actor, out var error)");
		StringAssert.Contains(source, "AlertUtilities.ValidateStoredDistantAlertEmote(emoteText, actor, out var error)");
	}

	[TestMethod]
	public void AlertUtilities_UsesHearingChecksAndFiresHeardEvent()
	{
		var source = File.ReadAllText(GetSourcePath("MudSharpCore", "Communication", "AlertUtilities.cs"));

		StringAssert.Contains(source, "public const uint AlertRoomRange = 2;");
		StringAssert.Contains(source, "public const AudioVolume AlertVolume = AudioVolume.ExtremelyLoud;");
		StringAssert.Contains(source, "public const int MaximumStoredAlertEmoteLength = 500;");
		StringAssert.Contains(source, "actor.Body.Communications.CanVocalise(actor.Body, AlertVolume)");
		StringAssert.Contains(source, "witness.CanHear(actor)");
		StringAssert.Contains(source, "witness.Location.LocalAudioDifficulty(witness, volume, proximity)");
		StringAssert.Contains(source, "CheckType.GenericListenCheck");
		StringAssert.Contains(source, "witness.HandleEvent(EventType.CharacterAlertHeard");
		StringAssert.Contains(source, "actor.Location.CellsInVicinity(AlertRoomRange");
		StringAssert.Contains(source, "npc.AIs.OfType<IOverrideAlertEmote>()");
	}

	[TestMethod]
	public void StoredAlertValidators_RejectValuesTooLongForPersistenceColumns()
	{
		var perceiver = new DummyPerceiver();
		var longLocal = $"@ whistle|whistles {new string('a', AlertUtilities.MaximumStoredAlertEmoteLength)}";
		var longDistant = $"A sharp, loud alert can be heard {{0}} {new string('a', AlertUtilities.MaximumStoredAlertEmoteLength)}";

		Assert.IsFalse(AlertUtilities.ValidateStoredAlertEmote(longLocal, perceiver, out var localError));
		StringAssert.Contains(localError, AlertUtilities.MaximumStoredAlertEmoteLength.ToString("N0"));
		Assert.IsFalse(AlertUtilities.ValidateStoredDistantAlertEmote(longDistant, perceiver, out var distantError));
		StringAssert.Contains(distantError, AlertUtilities.MaximumStoredAlertEmoteLength.ToString("N0"));
	}

	[TestMethod]
	public void EnforcerAI_RespondsToAlertAndCombatEvents()
	{
		var source = File.ReadAllText(GetSourcePath("MudSharpCore", "NPC", "AI", "EnforcerAI.cs"));

		StringAssert.Contains(source, "EnforcerAI : ArtificialIntelligenceBase, IOverrideAlertEmote");
		StringAssert.Contains(source, "case EventType.CharacterAlertHeard:");
		StringAssert.Contains(source, "case EventType.EngageInCombat:");
		StringAssert.Contains(source, "case EventType.EngagedInCombat:");
		StringAssert.Contains(source, "AlertUtilities.DoAlert(enforcer, echoFailure: false);");
		StringAssert.Contains(source, "patrolMember is null || patrolMember.Patrol.ActiveEnforcementTarget is not null");
		StringAssert.Contains(source, "enforcer.ColocatedWith(alerter)");
		StringAssert.Contains(source, "PatrolStrategyBase.TryCreatePatrolPath(");
		StringAssert.Contains(source, "PathSearch.PathIncludeUnlockableDoors(enforcer)");
		Assert.IsFalse(source.Contains("enforcer.PathBetween(origin, 20", StringComparison.Ordinal));
		StringAssert.Contains(source, "AssistNearbyEnforcerInCombat(enforcer, effect)");
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
