#nullable enable

using System;
using System.Collections.Generic;
using MudSharp.Database;
using MudSharp.RPG.Checks;

namespace DatabaseSeeder.Seeders;

/// <summary>
/// The initial non-magic action binding contract. The full role installer owns openings, caps and grants;
/// this manifest never substitutes stock skill identities or grants a skill to an existing player.
/// </summary>
public static class ArmageddonNonMagicSkillManifest
{
	public const string PeekTraitKey = "arm.skill.peek";
	public const string PeekAttemptKey = "arm.check.peek.attempt";
	public const string PeekRevealKey = "arm.check.peek.reveal";
	public static IReadOnlyList<string> PeekSourceMemberships { get; } = Array.AsReadOnly(new[] { "Burglar", "Pick-pocket" });
	public static IReadOnlyDictionary<string, CheckType> PeekChecks { get; } =
		new System.Collections.ObjectModel.ReadOnlyDictionary<string, CheckType>(new Dictionary<string, CheckType>
		{
			[PeekAttemptKey] = CheckType.PeekCheck,
			[PeekRevealKey] = CheckType.PeekRevealCheck
		});

	/// <summary>Bind the explicit ID already resolved for arm.skill.peek, retaining it across reruns and renames.</summary>
	public static PeekBindingResult BindPeek(FuturemudDatabaseContext db, long ownedPeekTraitId) =>
		PeekSeederHelper.BindExistingSkill(db, ownedPeekTraitId);
}
