#nullable enable

using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;

namespace MudSharp_Unit_Tests;

[TestClass]
public class PossessionReviewFindingSourceTests
{
	[TestMethod]
	public void PossessBody_UsesSharedPossessionAnchorCheck()
	{
		var source = File.ReadAllText(GetSourcePath("MudSharpCore", "Magic", "SpellEffects",
			"PossessBodySpellEffect.cs"));

		StringAssert.Contains(source, "PossessionControlService.AnyPossessionEffectsForAnchor(anchor)");
		Assert.IsFalse(source.Contains("CharacterInstanceService.AnyPossessedBodyEffectsForAnchor(anchor)",
			StringComparison.Ordinal));
	}

	[TestMethod]
	public void SeizeBody_RejectsDispelProxyActors()
	{
		var source = File.ReadAllText(GetSourcePath("MudSharpCore", "Magic", "SpellEffects",
			"DirectPossessionSpellEffects.cs"));
		var seizeStart = source.IndexOf("public sealed class SeizeBodySpellEffect", StringComparison.Ordinal);
		var corpseStart = source.IndexOf("public sealed class PossessCorpseSpellEffect", StringComparison.Ordinal);
		Assert.IsTrue(seizeStart >= 0);
		Assert.IsTrue(corpseStart > seizeStart);

		var seizeSource = source[seizeStart..corpseStart];
		StringAssert.Contains(seizeSource, "AffectedBy<IDispelMagicProxyEffect>()");
	}

	[TestMethod]
	public void AnimatedCorpseDeath_RetiresWithoutCreatingDuplicateRemains()
	{
		var source = File.ReadAllText(GetSourcePath("MudSharpCore", "Character", "ScriptedAiCharacterInstance.cs"));
		var animatedGuard = source.IndexOf("InstanceKind == CharacterInstanceKind.AnimatedCorpse",
			StringComparison.Ordinal);
		var createRemains = source.IndexOf("CreateNewBodyRemains", StringComparison.Ordinal);

		Assert.IsTrue(animatedGuard >= 0);
		Assert.IsTrue(createRemains > animatedGuard);
		StringAssert.Contains(source, "CharacterInstanceService.Retire(this, out _, deleteTemporaryRows: true, deathRetirement: true)");
	}

	[TestMethod]
	public void CommandCommand_AllowsAiControlledCharacters()
	{
		var source = File.ReadAllText(GetSourcePath("MudSharpCore", "Commands", "Modules", "GameModule.cs"));

		StringAssert.Contains(source, "target is not IArtificialIntelligenceControlledCharacter controlled");
		StringAssert.Contains(source, "controlled.HandleEvent(Events.EventType.CommandIssuedToCharacter");
		Assert.IsFalse(source.Contains("target is INPC npc", StringComparison.Ordinal));
	}

	[TestMethod]
	public void Quit_ChecksCurrentActorNoQuitBeforeLogoutActor()
	{
		var source = File.ReadAllText(GetSourcePath("MudSharpCore", "Commands", "Modules", "GameModule.cs"));
		var currentNoQuit = source.IndexOf("var currentActorNoQuit", StringComparison.Ordinal);
		var logoutActor = source.IndexOf("var logoutActor", StringComparison.Ordinal);
		Assert.IsTrue(currentNoQuit >= 0);
		Assert.IsTrue(logoutActor > currentNoQuit);
		StringAssert.Contains(source, "currentActorNoQuit.NoQuitReason");
		StringAssert.Contains(source, "!ReferenceEquals(actor, logoutActor)");
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
