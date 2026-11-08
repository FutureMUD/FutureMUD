#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using DatabaseSeeder.Seeders;
using MudSharp.Database;
using MudSharp.Models;
using MudSharp.RPG.Checks;

namespace DatabaseSeeder;

/// <summary>Optional setting policies supplied by a standalone product, never loaded by the base installer.</summary>
internal interface ISettingSeederIntegration
{
	SeederMetadata AdjustMetadata(IDatabaseSeeder seeder, SeederMetadata metadata) => metadata;
	bool IsInstalled(FuturemudDatabaseContext context);
	IReadOnlyDictionary<string, TraitDefinition> Resolve(FuturemudDatabaseContext context);
	IReadOnlyDictionary<string, TraitDefinition> EngineBindings(FuturemudDatabaseContext context);
	string SeedSkills(SkillPackageSeeder services, FuturemudDatabaseContext context, IReadOnlyDictionary<string, string> answers);
	string? Formula(CheckType type, IReadOnlyDictionary<string, TraitDefinition> skills);
	void EnsureCheck(FuturemudDatabaseContext context, CheckType type, string formula, long templateId, Difficulty maximum, ICollection<string> messages, bool rejectUnowned = false);
	void ValidateCombat(FuturemudDatabaseContext context, IReadOnlyDictionary<string, string> answers);
	void ValidateCombatIdentities(FuturemudDatabaseContext context);
	void CompleteCombat(FuturemudDatabaseContext context);
	bool HasFarmingFoundation(FuturemudDatabaseContext context);
	TraitDefinition? ResolveFarmingTrait(FuturemudDatabaseContext context);
	TraitDefinition EnsureFarmingTrait(FuturemudDatabaseContext context);
}

internal static class SettingSeederIntegration
{
	internal static ISettingSeederIntegration? Profile { get; set; }
	internal static bool IsInstalled(FuturemudDatabaseContext context) => Profile?.IsInstalled(context) ??
		context.SeederManagedRecords.Any(x => x.Module == "setting-skills" && !x.Retired);
	internal static string? CompatibilityIssue(FuturemudDatabaseContext context) => Profile is null && IsInstalled(context)
		? "This world uses setting-owned skill foundations. Run its standalone setting seeder to install or update packages. Database/schema preparation remains available in Database Seeder."
		: null;
	internal static ISettingSeederIntegration Require(FuturemudDatabaseContext context) => Profile ??
		throw new InvalidOperationException("This operation requires the world's standalone setting seeder. Run that product instead of Database Seeder; no foundation data has been changed.");
}
