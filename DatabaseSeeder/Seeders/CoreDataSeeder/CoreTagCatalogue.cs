#nullable enable

using System;
using System.Collections.Generic;
using CultureInfo = System.Globalization.CultureInfo;
using System.IO;
using System.Linq;
using System.Text.Json;
using MudSharp.Database;
using MudSharp.Models;

namespace DatabaseSeeder.Seeders;

/// <summary>Immutable stock identities. Live names and parentage are never used to regenerate ownership keys.</summary>
internal sealed class CoreTagCatalogue
{
	internal sealed record Definition(string Key, string Path, string Family)
	{
		public string Name => Path.Split(" / ")[^1];
		public string? ParentPath => Path.Contains(" / ") ? Path[..Path.LastIndexOf(" / ", StringComparison.Ordinal)] : null;
	}

	private const string Owner = nameof(CoreDataSeeder);
	private const string Module = "TagCatalogue";
	internal static readonly IReadOnlyList<Definition> Declarations = ReadManifest();
	internal static readonly IReadOnlyList<Definition> Definitions = Declarations.DistinctBy(x => x.Key).ToArray();
	private readonly Dictionary<string, Tag> _resolved = new(StringComparer.OrdinalIgnoreCase);
	private readonly List<string> _messages = [];
	internal IReadOnlyList<string> Messages => _messages;

	private static IReadOnlyList<Definition> ReadManifest()
	{
		using var stream = typeof(CoreTagCatalogue).Assembly.GetManifestResourceStream("DatabaseSeeder.CoreTags.tsv")
			?? throw new InvalidOperationException("Core tag manifest is missing.");
		using var reader = new StreamReader(stream);
		reader.ReadLine();
		var rows = new List<Definition>();
		while (reader.ReadLine() is { } line)
		{
			var fields = line.Split('\t');
			if (fields.Length != 4) throw new InvalidOperationException("Malformed Core tag manifest row.");
			rows.Add(new(fields[0], fields[1], fields[2]));
		}
		var keys = new Dictionary<string, string>(StringComparer.Ordinal);
		var paths = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
		foreach (var row in rows)
		{
			if (keys.TryGetValue(row.Key, out var path) && !path.Equals(row.Path, StringComparison.OrdinalIgnoreCase) ||
				paths.TryGetValue(row.Path, out var key) && key != row.Key)
				throw new InvalidOperationException($"Competing Core tag identity: {row.Path}.");
			if (row.ParentPath is { } parent && !paths.ContainsKey(parent))
				throw new InvalidOperationException($"Missing or unordered Core tag parent: {parent}.");
			keys[row.Key] = row.Path;
			paths[row.Path] = row.Key;
		}
		return rows;
	}

	internal static string NormalisePath(string path) => string.Join(" / ", path.Split('/', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries));

	// Complete legacy graph validation happens before adding any rows. No first-by-leaf adoption.
	private static Dictionary<string, Tag> IndexGraph(IReadOnlyCollection<Tag> tags)
	{
		var byId = tags.ToDictionary(x => x.Id);
		var paths = new Dictionary<long, string>();
		var visiting = new HashSet<long>();
		string PathFor(Tag tag)
		{
			if (paths.TryGetValue(tag.Id, out var cached)) return cached;
			if (!visiting.Add(tag.Id)) throw new InvalidOperationException($"Ambiguous legacy tag graph: cycle at #{tag.Id}.");
			var path = tag.Name;
			if (tag.ParentId is { } parentId)
			{
				if (!byId.TryGetValue(parentId, out var parent))
					throw new InvalidOperationException($"Ambiguous legacy tag graph: #{tag.Id} has missing parent #{parentId}.");
				path = $"{PathFor(parent)} / {tag.Name}";
			}
			visiting.Remove(tag.Id);
			return paths[tag.Id] = path;
		}
		var result = new Dictionary<string, Tag>(StringComparer.OrdinalIgnoreCase);
		foreach (var tag in tags)
		{
			var path = PathFor(tag);
			if (result.TryGetValue(path, out var other))
				throw new InvalidOperationException($"Ambiguous legacy tag graph: {path} has competing IDs #{other.Id} and #{tag.Id}. No tags were reconciled.");
			result.Add(path, tag);
		}
		return result;
	}

	private static Dictionary<string, SeederManagedRecord> RetainedBindings(FuturemudDatabaseContext context)
	{
		var keys = Definitions.Select(x => x.Key).ToHashSet(StringComparer.Ordinal);
		var records = context.SeederManagedRecords.Where(x => x.Seeder == Owner).AsEnumerable()
			.Where(x => keys.Contains(x.StableKey)).ToArray();
		if (records.Any(x => x.Module != Module || x.EntityType != nameof(Tag) || x.Retired || x.LogicalId is null || x.RevisionNumber is not null) ||
			records.GroupBy(x => x.StableKey).Any(x => x.Count() > 1) || records.GroupBy(x => x.LogicalId).Any(x => x.Count() > 1))
			throw new InvalidOperationException("Ambiguous Core tag ownership: conflicting module, type, retired claim or shared identity. No tags were reconciled.");
		return records.ToDictionary(x => x.StableKey, StringComparer.Ordinal);
	}

	internal static CoreTagCatalogue Ensure(FuturemudDatabaseContext context)
	{
		var catalogue = new CoreTagCatalogue();
		var all = context.Tags.ToList();
		var byPath = IndexGraph(all);
		var byId = all.ToDictionary(x => x.Id);
		var records = RetainedBindings(context);
		// Validate retained bindings before writing any part of this phase.
		foreach (var definition in Definitions)
		{
			if (records.TryGetValue(definition.Key, out var record) &&
				(record.EntityType != nameof(Tag) || record.LogicalId is not { } id || !byId.ContainsKey(id)))
				throw new InvalidOperationException($"Core tag {definition.Path}: retained identity is missing or invalid; explicit reconciliation required.");
		}
		var pending = new List<(Definition Definition, Tag Tag, bool Created)>();
        var claimedRows = new HashSet<Tag>();
		foreach (var definition in Definitions)
		{
			Tag tag;
			var created = false;
			if (records.TryGetValue(definition.Key, out var record))
			{
				tag = byId[record.LogicalId!.Value];
				if (!byPath.TryGetValue(definition.Path, out var canonical) || canonical.Id != tag.Id)
					catalogue._messages.Add($"Preserved builder tag #{tag.Id} for {definition.Path}; its live name or parent differs.");
			}
			else if (definition.Path == "Materials / Liquids / Water Soluble" &&
                !byPath.ContainsKey(definition.Path) && byPath.TryGetValue("Materials / Liquids / Water Soluable", out tag!))
            {
                catalogue._messages.Add($"Preserved legacy Water Soluable tag #{tag.Id} as the Water Soluble identity without renaming it.");
            }
            else if (!byPath.TryGetValue(definition.Path, out tag!))
			{
				// The old autobuilder/offering writers could alias a foreign-parent leaf. Keep that row and all references.
				var foreign = all.Where(x => x.Name.Equals(definition.Name, StringComparison.OrdinalIgnoreCase)).ToArray();
				if (foreign.Length > 0)
					catalogue._messages.Add($"Legacy aliases for {definition.Path}: {string.Join(", ", foreign.Select(x => $"#{x.Id}"))}. Created the intended path; existing references were preserved for review.");
				tag = new Tag { Name = definition.Name, Parent = definition.ParentPath is { } parent ? catalogue._resolved[parent] : null };
				context.Tags.Add(tag);
				created = true;
			}
			if (!claimedRows.Add(tag))
				throw new InvalidOperationException($"Core tag {definition.Path}: another stock identity already binds #{tag.Id}.");
			catalogue._resolved.Add(definition.Path, tag);
			if (record is null) pending.Add((definition, tag, created));
		}
		context.SaveChanges();
		foreach (var (definition, tag, created) in pending)
		{
			context.SeederManagedRecords.Add(new SeederManagedRecord
			{
				Seeder = Owner, Module = Module, EntityType = nameof(Tag), StableKey = definition.Key, LogicalId = tag.Id,
				ManifestVersion = "2026-10-18", AppliedAt = DateTime.UtcNow,
				// Unproven legacy rows are bound read-only. This is identity provenance, not field ownership.
				SeedBaseline = JsonSerializer.Serialize(new { CreatedByCore = created, definition.Path })
			});
		}
		context.SaveChanges();
		return catalogue;
	}

	internal Tag this[string path] => _resolved[NormalisePath(path)];

	internal Dictionary<string, Tag> Family(string family) => Declarations.Where(x => x.Family == family)
		.GroupBy(x => x.Name, StringComparer.OrdinalIgnoreCase)
		.ToDictionary(x => x.Key, x => this[x.First().Path], StringComparer.OrdinalIgnoreCase);

    internal static IReadOnlyDictionary<string, Tag> ResolveFamily(FuturemudDatabaseContext context, string family)
    {
        var all = context.Tags.ToList();
        var byPath = IndexGraph(all);
        var byId = all.ToDictionary(x => x.Id);
        var records = RetainedBindings(context);
        return Declarations.Where(x => x.Family == family).DistinctBy(x => x.Key).ToDictionary(x => x.Path, x =>
        {
            if (records.TryGetValue(x.Key, out var record))
                return byId.GetValueOrDefault(record.LogicalId!.Value) ?? throw new InvalidOperationException($"Core tag {x.Path} retained ID is missing.");
            return byPath.GetValueOrDefault(x.Path) ?? throw new InvalidOperationException($"Core tag {x.Path} is missing. Rerun Core foundation catalogues first.");
        }, StringComparer.OrdinalIgnoreCase);
    }

	internal static Tag Require(FuturemudDatabaseContext context, string path)
	{
		var canonical = NormalisePath(path);
		var definition = Definitions.Single(x => x.Path.Equals(canonical, StringComparison.OrdinalIgnoreCase));
		var records = RetainedBindings(context);
		var record = records.GetValueOrDefault(definition.Key);
		if (record is not null)
			return context.Tags.Find(record.LogicalId) ?? throw new InvalidOperationException($"Core tag {canonical} retained ID is missing. Rerun Core and review diagnostics.");
		var index = IndexGraph(context.Tags.ToList());
		return index.TryGetValue(canonical, out var tag) ? tag :
			throw new InvalidOperationException($"Core tag {canonical} is missing. Rerun Core foundation catalogues first.");
	}

	internal static Tag RequireUniqueStockName(FuturemudDatabaseContext context, string name)
	{
		var matches = Definitions.Where(x => x.Name.Equals(name, StringComparison.OrdinalIgnoreCase)).ToArray();
		if (matches.Length != 1) throw new InvalidOperationException($"Core tag '{name}' requires an explicit full path ({matches.Length} stock identities).");
		return Require(context, matches[0].Path);
	}

	internal void BindSettings(FuturemudDatabaseContext context, ICollection<string> messages)
	{
		void Bind(string name, long target)
		{
			var setting = context.StaticConfigurations.Find(name);
			if (setting is null) context.StaticConfigurations.Add(new StaticConfiguration { SettingName = name, Definition = target.ToString(CultureInfo.InvariantCulture) });
			else if (string.IsNullOrWhiteSpace(setting.Definition) || setting.Definition == "0") setting.Definition = target.ToString(CultureInfo.InvariantCulture);
			else if (setting.Definition != target.ToString(CultureInfo.InvariantCulture)) messages.Add($"Preserved {name}={setting.Definition}; Core target is #{target}. Review custom or invalid binding.");
		}
		foreach (var (setting, name) in new[]
		{
			("MusketRamrodTag", "Musket Ramrod"), ("MusketCleaningRamrodTag", "Musket Cleaning Rod"),
			("MusketUnjammingToolTag", "Musket Unjamming Tool"), ("MusketMatchCordTag", "Match Cord"),
			("MusketIgnitionSourceTag", "Ignition Source"), ("WadItemTagId", "Musket Wadding")
		}) Bind(setting, this[Definitions.Single(x => x.Name == name).Path].Id);
		var gunpowder = context.Materials.AsEnumerable().Where(x => x.Name.Equals("gunpowder", StringComparison.OrdinalIgnoreCase)).ToArray();
		if (gunpowder.Length == 1) Bind("GunpowderMaterialId", gunpowder[0].Id);
		else messages.Add("GunpowderMaterialId was not bound: a unique gunpowder material is not installed.");
		context.SaveChanges();
	}
}
