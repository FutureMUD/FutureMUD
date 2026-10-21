#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;

namespace DatabaseSeeder.Seeders;

/// <summary>Checks only declared reference fields in the approved stock graph.</summary>
public static class ArmageddonStockManifest
{
	public static string Read()
	{
		using var stream = typeof(ArmageddonStockManifest).Assembly.GetManifestResourceStream("ArmageddonStockDesign")
			?? throw new InvalidOperationException("The approved Armageddon stock manifest is missing.");
		using var reader = new System.IO.StreamReader(stream);
		return reader.ReadToEnd();
	}

	public static IReadOnlyList<string> Validate(string json)
	{
		var errors = new List<string>();
		try
		{
			using var document = JsonDocument.Parse(json);
			var root = document.RootElement;
			var entries = root.GetProperty("entries").EnumerateArray().ToArray();
			var blueprints = root.GetProperty("prototype_catalogue").EnumerateArray().ToArray();
			var capabilities = root.GetProperty("capabilities").EnumerateArray().ToArray();
			var keys = entries.Select(x => x.GetProperty("key").GetString()!).ToHashSet(StringComparer.Ordinal);
			var required = entries.Where(x => x.GetProperty("required").GetBoolean()).Select(x => x.GetProperty("key").GetString()!).ToHashSet(StringComparer.Ordinal);
			var protoKeys = blueprints.Select(x => x.GetProperty("key").GetString()!).ToHashSet(StringComparer.Ordinal);
			var capKeys = capabilities.Select(x => x.GetProperty("key").GetString()!).ToHashSet(StringComparer.Ordinal);
			if (entries.Length != keys.Count || keys.Count != 154 || required.Count != 152 || blueprints.Length != protoKeys.Count || protoKeys.Count != 210 || capabilities.Length != capKeys.Count || capKeys.Count != 10)
				errors.Add("Stock identities/counts differ from the approved 154 spells, 210 blueprints and ten capabilities.");
			foreach (var entry in entries)
			{
				if (!double.IsFinite(entry.GetProperty("minimum_energy").GetDouble()) || entry.GetProperty("minimum_energy").GetDouble() <= 0)
					errors.Add($"{entry.GetProperty("key")}: minimum energy must be finite and positive.");
				foreach (var dependency in entry.GetProperty("prototype_dependencies").EnumerateArray())
					if (!protoKeys.Contains(dependency.GetString()!)) errors.Add($"{entry.GetProperty("key")}: missing blueprint {dependency}.");
			}
			var graph = new Dictionary<string, string[]>(StringComparer.Ordinal);
			foreach (var blueprint in blueprints)
			{
				var key = blueprint.GetProperty("key").GetString()!;
				var dependencies = blueprint.GetProperty("dependencies").EnumerateArray().Select(x => x.GetString()!).ToArray();
				graph.Add(key, dependencies);
				var configuration = blueprint.GetProperty("configuration");
				foreach (var field in new[] { "race_bundle", "ai_bundle", "culture", "native_material" })
					if (configuration.TryGetProperty(field, out var reference) && reference.ValueKind == JsonValueKind.String && reference.GetString()!.StartsWith("arm.", StringComparison.Ordinal) &&
						(!protoKeys.Contains(reference.GetString()!) || !dependencies.Contains(reference.GetString()!))) errors.Add($"{key}: unresolved declared {field} reference.");
				if (configuration.TryGetProperty("recipe", out var recipe) && recipe.ValueKind == JsonValueKind.Object)
				{
					if (recipe.TryGetProperty("inputs", out var inputs))
						foreach (var input in inputs.EnumerateObject())
							if (!protoKeys.Contains(input.Name) || !dependencies.Contains(input.Name) || !double.IsFinite(input.Value.GetDouble()) || input.Value.GetDouble() <= 0)
								errors.Add($"{key}: unresolved or invalid declared recipe input {input.Name}.");
					if (recipe.TryGetProperty("min_component_crafting", out var minimum) && minimum.GetInt32() is < 1 or > 90) errors.Add($"{key}: crafting prerequisite exceeds the approved support cap.");
				}
			}
			CheckGraph(graph, errors, "Blueprint");
			var policy = root.GetProperty("classic_policy");
			var classic = policy.GetProperty("elemental_rows").EnumerateArray().ToArray();
			if (classic.Length != 180) errors.Add("Expected 180 elemental stock bindings.");
			var classicGraph = new Dictionary<string, string[]>(StringComparer.Ordinal);
			foreach (var row in classic)
			{
				var binding = row.GetProperty("binding_key").GetString()!;
				var key = row.GetProperty("key").GetString()!;
				var parent = row.GetProperty("parent_binding_key").GetString();
				var kind = row.GetProperty("kind").GetString();
				if (!capKeys.Contains(row.GetProperty("capability").GetString()!) || (kind == "spell" ? !required.Contains(key) : kind != "support" || key != "arm.support.component_crafting") || row.GetProperty("starting").GetBoolean() != (parent is null))
					errors.Add($"{binding}: invalid capability, stock identity, support or root.");
				if (!classicGraph.TryAdd(binding, parent is null ? [] : [parent])) errors.Add($"Duplicate binding {binding}.");
				if (row.GetProperty("opening").GetInt32() < 0 || row.GetProperty("raw_cap").GetInt32() is < 1 or > 100 || row.GetProperty("opening").GetInt32() > row.GetProperty("raw_cap").GetInt32()) errors.Add($"{binding}: invalid opening/cap.");
				if (parent is not null)
				{
					var parents = classic.Where(x => x.GetProperty("binding_key").GetString() == parent).ToArray();
					if (parents.Length != 1 || parents[0].GetProperty("capability").GetString() != row.GetProperty("capability").GetString() || parents[0].GetProperty("key").GetString() != row.GetProperty("parent_key").GetString() || row.GetProperty("parent_threshold").GetInt32() < 0 || row.GetProperty("parent_threshold").GetInt32() > parents[0].GetProperty("raw_cap").GetInt32())
						errors.Add($"{binding}: unresolved or unreachable typed parent.");
				}
			}
			CheckGraph(classicGraph, errors, "Classic");
			var source = policy.GetProperty("sorcerer_install_rows").EnumerateArray().ToArray();
			var sourceGraph = source.ToDictionary(x => x.GetProperty("key").GetString()!, x => x.GetProperty("parent_key").GetString() is { } parent ? new[] { parent } : Array.Empty<string>());
			CheckGraph(sourceGraph, errors, "Sorcerer");
			if (source.Length != 83 || source.Any(x => x.GetProperty("kind").GetString() != "spell" && x.GetProperty("key").GetString() != "arm.support.component_crafting")) errors.Add("Sorcerer installs only 82 spells and required crafting.");
			var simple = root.GetProperty("simple_progression").EnumerateArray().ToArray();
			var admissions = new HashSet<(string, string)>();
			foreach (var row in simple)
			{
				var cap = row.GetProperty("capability").GetString()!;
				var spell = row.GetProperty("spell").GetString()!;
				var threshold = row.GetProperty("unlock_at").GetInt32();
				if (!capKeys.Contains(cap) || !required.Contains(spell) || !admissions.Add((cap, spell)) || threshold is < 10 or > 90 || row.GetProperty("acquired_controlled_grade").GetInt32() != 1)
					errors.Add($"Simple {cap}/{spell}: invalid or duplicate admission.");
			}
			if (simple.Length != 419 || !required.SetEquals(simple.Select(x => x.GetProperty("spell").GetString()!))) errors.Add("Simple must contain 419 admissions covering all 152 required spells.");
			foreach (var capability in capabilities)
			{
				var key = capability.GetProperty("key").GetString();
				var own = simple.Where(x => x.GetProperty("capability").GetString() == key).ToArray();
				var roster = capability.GetProperty("required_spell_keys").EnumerateArray().Select(x => x.GetString()!).ToHashSet(StringComparer.Ordinal);
				var roots = capability.GetProperty("simple_starting_spells").EnumerateArray().Select(x => x.GetString()!).ToHashSet(StringComparer.Ordinal);
				if (!roster.SetEquals(own.Select(x => x.GetProperty("spell").GetString()!)) || !roots.SetEquals(own.Where(x => x.GetProperty("starting").GetBoolean()).Select(x => x.GetProperty("spell").GetString()!)) ||
					capability.GetProperty("simple_opening").GetInt32() != 10 || capability.GetProperty("simple_raw_cap").GetInt32() != 100 || own.Any(x => x.GetProperty("main_trait").GetString() != capability.GetProperty("simple_main_trait_key").GetString() || (x.GetProperty("unlock_at").GetInt32() == 10) != x.GetProperty("starting").GetBoolean())) errors.Add($"{key}: Simple roster, roots or main trait differs from the approved route.");
				foreach (var row in own)
					if (row.TryGetProperty("source_parent_spell", out var parent) && parent.GetString() is { } parentKey)
					{
						var preceding = own.Where(x => x.GetProperty("spell").GetString() == parentKey).ToArray();
						if (preceding.Length != 1 || preceding[0].GetProperty("unlock_at").GetInt32() >= row.GetProperty("unlock_at").GetInt32()) errors.Add($"{key}: Simple parent must precede its child.");
					}
			}
			if (!required.SetEquals(classic.Where(x => x.GetProperty("kind").GetString() == "spell").Concat(source.Where(x => x.GetProperty("kind").GetString() == "spell")).Select(x => x.GetProperty("key").GetString()!))) errors.Add("Classic does not cover all required spells.");
		}
		catch (Exception error) when (error is JsonException or KeyNotFoundException or InvalidOperationException or ArgumentException)
		{
			errors.Add("Invalid stock manifest: " + error.Message);
		}
		return errors;
	}

	private static void CheckGraph(IReadOnlyDictionary<string, string[]> graph, List<string> errors, string label)
	{
		var visited = new HashSet<string>(StringComparer.Ordinal);
		var visiting = new HashSet<string>(StringComparer.Ordinal);
		bool Visit(string key)
		{
			if (!graph.ContainsKey(key) || visiting.Contains(key)) return false;
			if (visited.Contains(key)) return true;
			visiting.Add(key);
			if (graph[key].Any(parent => !Visit(parent))) return false;
			visiting.Remove(key);
			visited.Add(key);
			return true;
		}
		if (graph.Keys.Any(key => !Visit(key))) errors.Add($"{label} dependencies contain a cycle or unresolved declared key.");
	}
}
