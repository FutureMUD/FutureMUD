#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Security.Cryptography;
using System.Text;
using Microsoft.EntityFrameworkCore;
using MudSharp.Database;
using MudSharp.Models;

namespace DatabaseSeeder.Seeders;

public sealed record ArmageddonSkillEntry(string Key, string Name, string Group, bool Language, bool Scaffold);

/// <summary>Source identities and explicit engine bindings. Display names never establish ownership.</summary>
public static class ArmageddonSkillPackage
{
	public const string Module = "setting-skills";
	public const string Version = "setting-20261019-1";
	private sealed record Manifest(int Format, string Version, string Owner,
		Dictionary<string, string> Aliases, string[] Capabilities, ArmageddonSkillEntry[] Skills,
		Dictionary<string, string> Bindings);
	private static readonly Lazy<Manifest> Data = new(() =>
	{
		using var stream = typeof(ArmageddonSkillPackage).Assembly.GetManifestResourceStream("DatabaseSeeder.Armageddon.skills.json")
			?? throw new InvalidOperationException("Missing Armageddon skill manifest.");
		var manifest = JsonSerializer.Deserialize<Manifest>(stream, new JsonSerializerOptions { PropertyNameCaseInsensitive = true })
			?? throw new InvalidOperationException("Invalid Armageddon skill manifest.");
		if (manifest.Format != 1 || manifest.Version != Version || manifest.Owner != ArmageddonMagicInstaller.Package ||
			manifest.Skills.Select(x => x.Key).Distinct().Count() != manifest.Skills.Length ||
			manifest.Skills.Count(x => !x.Scaffold) != 112 ||
			manifest.Skills.Select(x => x.Name).Distinct(StringComparer.OrdinalIgnoreCase).Count() != manifest.Skills.Length ||
			manifest.Bindings.Values.Concat(manifest.Aliases.Values).Any(x => !manifest.Skills.Any(y => y.Key == x)))
			throw new InvalidOperationException("Armageddon skill manifest identity/reference drift.");
		return manifest;
	});
	public static IReadOnlyList<ArmageddonSkillEntry> Skills => Array.AsReadOnly(Data.Value.Skills);
	/// <summary>First-install bindings only. Later phases and reruns must retain builder configuration.</summary>
	public static IReadOnlyDictionary<string, string> StaticTraitBindings { get; } =
		new Dictionary<string, string>(StringComparer.Ordinal)
		{
			["AppraiseCommandSkill"] = "arm.skill.value",
			["HaggleTraitId"] = "arm.skill.haggle",
			["SkinningTraitId"] = "arm.skill.skinning",
			["LiteracySkillId"] = "arm.skill.literacy",
			["HandwritingSkillId"] = "arm.skill.handwriting",
			["ForgerySkillId"] = "arm.skill.forgery",
			["DrawingTraitId"] = "arm.scaffold.drawing",
			["TattooistTrait"] = "arm.scaffold.tattoo",
			["DefaultAuxiliaryMoveTraitId"] = "arm.scaffold.auxiliary",
			["VehiclePilotTraitId"] = "arm.skill.pilot"
		};

	internal static void BindStaticTraits(FuturemudDatabaseContext db, IReadOnlyDictionary<string, TraitDefinition> skills)
	{
		foreach (var binding in StaticTraitBindings)
		{
			var setting = db.StaticConfigurations.Find(binding.Key);
			if (setting is null)
			{
				setting = new StaticConfiguration { SettingName = binding.Key };
				db.StaticConfigurations.Add(setting);
			}
			setting.Definition = skills[binding.Value].Id.ToString(System.Globalization.CultureInfo.InvariantCulture);
		}

	}
	internal static void EnableBoundNewWorldFeatures(FuturemudDatabaseContext db)
	{
		// Called once, after all required checks/traits resolve and inside the fresh-world transaction.
		// Subsequent early-package runs are read-only and preserve builder opt-outs.
		foreach (var key in new[] { "VehiclePilotEnabled", "DirectionSenseEnabled", "ArmageddonHostileGuardAttemptsEnabled", "HagglingEnabled" })
		{
			var setting = db.StaticConfigurations.Find(key);
			if (setting is null) db.StaticConfigurations.Add(new StaticConfiguration { SettingName = key, Definition = "true" });
			else setting.Definition = "true";
		}
		foreach (var key in new[] { "ElectricalCommandEnabled", "ProgrammingCommandEnabled" })
		{
			var setting = db.StaticConfigurations.Find(key);
			if (setting is null) db.StaticConfigurations.Add(new StaticConfiguration { SettingName = key, Definition = "false" });
			else setting.Definition = "false";
		}
	}

	public static string CanonicalKey(string key) => Data.Value.Aliases.GetValueOrDefault(key) ?? key;
	public static bool IsInstalled(FuturemudDatabaseContext db) => db.SeederManagedRecords.Any(x =>
		x.Seeder == ArmageddonMagicInstaller.Package && x.Module == Module);

	public static IReadOnlyDictionary<string, TraitDefinition> Resolve(FuturemudDatabaseContext db)
	{
        var records = db.SeederManagedRecords.Where(x => x.Seeder == ArmageddonMagicInstaller.Package && x.Module == Module).ToArray();
        var expected = Skills.SelectMany(x => new[] { x.Key, x.Key + ".cap" }).ToHashSet(StringComparer.Ordinal);
        if (records.Length != expected.Count || records.Select(x => x.StableKey).Distinct().Count() != expected.Count ||
            records.Any(x => !expected.Contains(x.StableKey) || x.Retired || x.ManifestVersion != Version || x.SeedBaseline is null ||
                x.AppliedFingerprint != Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(x.SeedBaseline))).ToLowerInvariant()))
            throw new InvalidOperationException("Armageddon skill ownership inventory/baselines are incomplete or corrupt.");
        foreach (var cap in records.Where(x => x.StableKey.EndsWith(".cap", StringComparison.Ordinal)))
            if (cap.EntityType != nameof(TraitExpression) || cap.LogicalId is not { } capId || !db.TraitExpressions.Any(x => x.Id == capId) ||
                db.SeederManagedRecords.Any(x => x.Id != cap.Id && x.EntityType == cap.EntityType && x.LogicalId == capId))
                throw new InvalidOperationException($"Invalid or competing skill cap ownership: {cap.StableKey}.");
		var result = new Dictionary<string, TraitDefinition>(StringComparer.Ordinal);
		foreach (var skill in Skills)
		{
			var record = db.SeederManagedRecords.SingleOrDefault(x => x.Seeder == ArmageddonMagicInstaller.Package &&
				x.Module == Module && x.StableKey == skill.Key && x.EntityType == nameof(TraitDefinition));
			if (record is null || record.Retired || record.LogicalId is not { } id ||
				db.SeederManagedRecords.Any(x => x.Id != record.Id && x.EntityType == nameof(TraitDefinition) && x.LogicalId == id))
				throw new InvalidOperationException($"Missing, retired or competing owned skill: {skill.Key}.");
			var trait = db.TraitDefinitions.Include(x => x.Expression).SingleOrDefault(x => x.Id == id);
			if (trait is null || trait.Type != 0 || trait.OwnerScope != (int)MudSharp.Body.Traits.TraitOwnerScope.Character || trait.Expression is null ||
				!db.TraitDecorators.Any(x => x.Id == trait.DecoratorId) || !db.Improvers.Any(x => x.Id == trait.ImproverId))
				throw new InvalidOperationException($"Invalid native character skill graph: {skill.Key} #{id}.");
			result.Add(skill.Key, trait);
		}
		foreach (var alias in Data.Value.Aliases) result.Add(alias.Key, result[alias.Value]);
		return result;
	}

	internal static IReadOnlyDictionary<string, TraitDefinition> EngineBindings(FuturemudDatabaseContext db)
	{
		var owned = Resolve(db);
		return Data.Value.Bindings.ToDictionary(x => x.Key, x => owned[x.Value], StringComparer.OrdinalIgnoreCase);
	}

	internal static void RequireFreshWorld(FuturemudDatabaseContext db)
	{
		_ = Skills; // Validate the complete immutable inventory before the first write.
		foreach (var binding in StaticTraitBindings)
		{
			var setting = db.StaticConfigurations.Find(binding.Key);
			if (setting is not null && (!long.TryParse(setting.Definition, out var id) || id != 0))
				throw new InvalidOperationException($"{binding.Key} is already customised. Resolve the new-world skill binding explicitly before installation.");
		}
        var existingNames = db.TraitDefinitions.Select(x => x.Name).ToArray().ToHashSet(StringComparer.OrdinalIgnoreCase);
        var collision = Skills.FirstOrDefault(x => existingNames.Contains(x.Name));
        if (collision is not null) throw new InvalidOperationException($"Armageddon skill name collides with an existing trait: {collision.Name}.");
        foreach (var name in new[] { "DefaultSkillDecorator", "DefaultSkillImprover" })
            if (!db.StaticConfigurations.Any(x => x.SettingName == name)) throw new InvalidOperationException($"Core foundation missing: {name}.");
        if (db.FutureProgs.Count(x => x.FunctionName == "AlwaysTrue" || x.FunctionName == "AlwaysFalse") != 2 || !db.LanguageDifficultyModels.Any())
            throw new InvalidOperationException("Core Boolean progs and language difficulty foundations are required.");
		if (db.TraitDefinitions.Any(x => x.Type == 0) || db.Improvers.Any(x => x.Name == "Skill Improver") ||
			db.Languages.Any(x => x.Name == "Admin Speech") || db.SeederManagedRecords.Any(x => x.Module == Module))
			throw new InvalidOperationException("Armageddon replaces the new-world skill package. Existing skills require a separately reviewed conversion; no records changed.");
	}
}
