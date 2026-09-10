using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using MudSharp.Communication.Language;
using MudSharp.Database;
using MudSharp.Models;

#nullable enable
namespace DatabaseSeeder.Seeders.CultureToolkit;

/// <summary>Explicit classifications of existing stock content, not a runtime name heuristic.</summary>
public static class CultureStockAccentRoles
{
	public static void ApplyLegacy(FuturemudDatabaseContext context, Accent accent, bool fresh,
		ICollection<string> conflicts)
	{
		var key = $"legacy.accent-metadata.{accent.Id}";
		var record = CultureToolkitManagedEntities.Find(context, "AccentMetadata", key);
		var role = Role(accent.Language.Name, accent.Name, accent.Group);
		if (record is null)
		{
			record = new SeederManagedRecord
			{
				Seeder = "CultureSeeder", EntityType = "AccentMetadata", StableKey = key,
				LogicalId = accent.Id, Module = "legacy", ManifestVersion = "2026-09-10", AppliedAt = DateTime.UtcNow,
				SeedBaseline = JsonSerializer.Serialize(new Dictionary<string, string>
				{
					["role"] = role == AccentRole.Fallback && !fresh ? "2" : "0",
					["languages"] = "[]"
				})
			};
			context.SeederManagedRecords.Add(record);
		}

		var sources = new List<long>();
		foreach (var name in Associations(accent.Language.Name, accent.Name))
		{
			var matches = context.Languages.AsEnumerable().Where(x => x.Name.Equals(name, StringComparison.OrdinalIgnoreCase)).ToArray();
			if (matches.Length == 1) sources.Add(matches[0].Id);
			else conflicts.Add($"Accent {accent.Language.Name}/{accent.Name}: associated language {name} is missing or ambiguous.");
		}
		var merged = SeederManagedRecordReconciler.Reconcile(record,
			new Dictionary<string, string> { ["role"] = accent.Role.ToString(),
				["languages"] = JsonSerializer.Serialize(accent.AssociatedLanguages.Select(x => x.Id).Order().ToArray()) },
			new Dictionary<string, string> { ["role"] = ((int)role).ToString(),
				["languages"] = JsonSerializer.Serialize(sources.Distinct().Order().ToArray()) }, fresh, conflicts);
		accent.Role = int.Parse(merged["role"]);
		accent.AssociatedLanguages.Clear();
		foreach (var id in JsonSerializer.Deserialize<long[]>(merged["languages"])!) accent.AssociatedLanguages.Add(context.Languages.Find(id)!);
	}

	private static readonly Dictionary<string, string[]> EnglishForeign = new(StringComparer.OrdinalIgnoreCase)
	{
		["chinese"] = [], // The source does not identify a particular Chinese language.
		["japanese"] = ["Japanese"],
		["korean"] = ["Korean"],
		["southeast asian"] = [],
		["eastern european"] = [],
		["french"] = ["French"],
		["german"] = ["German"],
		["italian"] = ["Italian"],
		["spanish"] = ["Spanish"],
		["middle-eastern"] = ["Arabic"]
	};

	public static AccentRole Role(string language, string accent, string group)
	{
		if (accent.Equals("Foreign", StringComparison.OrdinalIgnoreCase) ||
			accent.Equals("Learner", StringComparison.OrdinalIgnoreCase) ||
			accent.Equals("Crude", StringComparison.OrdinalIgnoreCase)) return AccentRole.Fallback;
		if (language.Equals("English", StringComparison.OrdinalIgnoreCase) && EnglishForeign.ContainsKey(accent))
			return AccentRole.Foreign;
		return group.Equals("foreign", StringComparison.OrdinalIgnoreCase) ? AccentRole.Foreign : AccentRole.Native;
	}

	public static IReadOnlyList<string> Associations(string language, string accent, string? sourceModule = null)
	{
		if (!language.Equals("English", StringComparison.OrdinalIgnoreCase)) return [];
		// The historical generator names these languages explicitly; modern display labels
		// must not be used as historical source identities.
		if (sourceModule == "earthrenaissanceeurope")
		{
			if (accent.Equals("german", StringComparison.OrdinalIgnoreCase)) return ["High German", "Low German"];
			if (accent.Equals("spanish", StringComparison.OrdinalIgnoreCase)) return ["Castilian"];
		}
		return EnglishForeign.TryGetValue(accent, out var sources) ? sources : [];
	}
}
