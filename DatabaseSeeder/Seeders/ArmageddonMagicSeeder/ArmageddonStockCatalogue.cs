#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;

namespace DatabaseSeeder.Seeders;

public enum ArmageddonBlueprintKind
{
	Creature, MeleeItem, ComponentSupply, Focus, AiBundle, RaceBundle, Shelter, Food,
	LiquidContainer, DeviceBlank, CraftSupply, MaterialBinding, Culture, Light, Trigger,
	ProjectionAnchor, Pocket, TopicalDrug, Liquid, LiquidBinding, StackableSupply, Poison, Disease
}

public sealed record ArmageddonCatalogueReference(string Field, string Key, ArmageddonBlueprintKind Kind);
public sealed record ArmageddonCatalogueRecipe(IReadOnlyDictionary<string, double> Inputs, bool Kilograms,
	double Seconds, int MinimumComponentCrafting);
public sealed record ArmageddonStockBlueprint(string Key, string Name, ArmageddonBlueprintKind Kind,
	JsonElement Configuration, IReadOnlyList<string> Dependencies,
	IReadOnlyList<ArmageddonCatalogueReference> References, ArmageddonCatalogueRecipe? Recipe)
{
	// Real spell allowlists are an S4 dependency; no placeholder native IDs in S3.
	public bool DeferredToS4 => Kind is ArmageddonBlueprintKind.Focus or ArmageddonBlueprintKind.DeviceBlank;
}

/// <summary>
/// The approved catalogue in dependency order. Only declared, schema-specific reference fields
/// participate in its graph. Descriptions and arbitrary definition text are never identity sources.
/// </summary>
public sealed class ArmageddonStockCatalogue
{
	private static readonly IReadOnlyDictionary<string, ArmageddonBlueprintKind> Kinds =
		new Dictionary<string, ArmageddonBlueprintKind>(StringComparer.Ordinal)
		{
			["approved_simple_npc_template"] = ArmageddonBlueprintKind.Creature,
			["approved_melee_item"] = ArmageddonBlueprintKind.MeleeItem,
			["native_component_supply"] = ArmageddonBlueprintKind.ComponentSupply,
			["retained_magic_focus"] = ArmageddonBlueprintKind.Focus,
			["native_ai_binding_bundle"] = ArmageddonBlueprintKind.AiBundle,
			["race_ethnicity_body_bundle"] = ArmageddonBlueprintKind.RaceBundle,
			["native_shelter_template_bundle"] = ArmageddonBlueprintKind.Shelter,
			["native_food"] = ArmageddonBlueprintKind.Food,
			["native_liquid_container"] = ArmageddonBlueprintKind.LiquidContainer,
			["charged_device_blank"] = ArmageddonBlueprintKind.DeviceBlank,
			["native_craft_supply"] = ArmageddonBlueprintKind.CraftSupply,
			["native_material_binding"] = ArmageddonBlueprintKind.MaterialBinding,
			["culture_bundle"] = ArmageddonBlueprintKind.Culture,
			["wearable_light"] = ArmageddonBlueprintKind.Light,
			["native_trigger_item"] = ArmageddonBlueprintKind.Trigger,
			["projection_anchor"] = ArmageddonBlueprintKind.ProjectionAnchor,
			["pocket_vessel"] = ArmageddonBlueprintKind.Pocket,
			["native_topical_drug"] = ArmageddonBlueprintKind.TopicalDrug,
			["native_liquid"] = ArmageddonBlueprintKind.Liquid,
			["native_liquid_binding"] = ArmageddonBlueprintKind.LiquidBinding,
			["native_stackable_supply"] = ArmageddonBlueprintKind.StackableSupply,
			["native_poison_definition"] = ArmageddonBlueprintKind.Poison,
			["native_disease_definition"] = ArmageddonBlueprintKind.Disease
		};

	public IReadOnlyList<ArmageddonStockBlueprint> Blueprints { get; }
	public IReadOnlyDictionary<string, ArmageddonStockBlueprint> ByKey { get; }

	public ArmageddonStockCatalogue(string? source = null)
	{
		using var document = JsonDocument.Parse(source ?? ArmageddonStockManifest.Read());
		var rows = document.RootElement.GetProperty("prototype_catalogue").EnumerateArray().ToArray();
		var kinds = rows.ToDictionary(x => x.GetProperty("key").GetString()!,
			x => Kinds.TryGetValue(x.GetProperty("kind").GetString()!, out var kind) ? kind :
				throw new InvalidOperationException($"Unknown blueprint kind for {x.GetProperty("key")}.") , StringComparer.Ordinal);
		if (kinds.Count != 210) throw new InvalidOperationException("The approved catalogue requires exactly 210 distinct blueprints.");
		var parsed = rows.Select(row => Parse(row, kinds)).ToDictionary(x => x.Key, StringComparer.Ordinal);
		var ordered = new List<ArmageddonStockBlueprint>();
		var visiting = new HashSet<string>(StringComparer.Ordinal);
		var visited = new HashSet<string>(StringComparer.Ordinal);
		void Visit(string key)
		{
			if (visited.Contains(key)) return;
			if (!parsed.TryGetValue(key, out var blueprint) || !visiting.Add(key))
				throw new InvalidOperationException($"Unresolved or cyclic blueprint dependency: {key}.");
			foreach (var dependency in blueprint.Dependencies) Visit(dependency);
			visiting.Remove(key);
			visited.Add(key);
			ordered.Add(blueprint);
		}
		foreach (var row in rows) Visit(row.GetProperty("key").GetString()!);
		Blueprints = ordered.AsReadOnly();
		ByKey = new System.Collections.ObjectModel.ReadOnlyDictionary<string, ArmageddonStockBlueprint>(parsed);
	}

	private static ArmageddonStockBlueprint Parse(JsonElement row, IReadOnlyDictionary<string, ArmageddonBlueprintKind> kinds)
	{
		var key = row.GetProperty("key").GetString()!;
		var configuration = row.GetProperty("configuration");
		var dependencies = row.GetProperty("dependencies").EnumerateArray().Select(x => x.GetString()!).ToArray();
		if (dependencies.Distinct(StringComparer.Ordinal).Count() != dependencies.Length || dependencies.Any(x => !kinds.ContainsKey(x)))
			throw new InvalidOperationException($"{key}: duplicate or unresolved declared dependencies.");
		var references = new List<ArmageddonCatalogueReference>();
		void Reference(string field, string target, params ArmageddonBlueprintKind[] allowed)
		{
			if (!dependencies.Contains(target, StringComparer.Ordinal) || !kinds.TryGetValue(target, out var kind) || !allowed.Contains(kind))
				throw new InvalidOperationException($"{key}: {field} must reference a declared compatible blueprint dependency.");
			references.Add(new(field, target, kind));
		}
		foreach (var (field, kind) in new[]
		{
			("race_bundle", ArmageddonBlueprintKind.RaceBundle), ("ai_bundle", ArmageddonBlueprintKind.AiBundle),
			("culture", ArmageddonBlueprintKind.Culture), ("native_material", ArmageddonBlueprintKind.MaterialBinding)
		})
			if (configuration.TryGetProperty(field, out var value)) Reference(field, value.GetString()!, kind);
		ArmageddonCatalogueRecipe? recipe = null;
		if (configuration.TryGetProperty("recipe", out var recipeRow))
		{
			var inputs = recipeRow.GetProperty("inputs").EnumerateObject().ToDictionary(x => x.Name, x => x.Value.GetDouble(), StringComparer.Ordinal);
			if (inputs.Count == 0 || inputs.Values.Any(x => !double.IsFinite(x) || x <= 0))
				throw new InvalidOperationException($"{key}: recipes require positive, finite input quantities.");
			var kilograms = recipeRow.TryGetProperty("input_units", out var units) && units.GetString() == "kilograms";
			foreach (var input in inputs)
			{
				if (kilograms) Reference("recipe.inputs", input.Key, ArmageddonBlueprintKind.MaterialBinding);
				else
				{
					Reference("recipe.inputs", input.Key, ArmageddonBlueprintKind.CraftSupply, ArmageddonBlueprintKind.StackableSupply,
						ArmageddonBlueprintKind.ComponentSupply);
					if (input.Value != Math.Truncate(input.Value)) throw new InvalidOperationException($"{key}: item recipe quantities must be whole units.");
				}
			}
			var seconds = recipeRow.TryGetProperty("seconds", out var duration) ? duration.GetDouble() : recipeRow.GetProperty("minutes").GetDouble() * 60;
			var minimum = recipeRow.TryGetProperty("min_component_crafting", out var gate) ? gate.GetInt32() : 0;
			if (!double.IsFinite(seconds) || seconds <= 0 || minimum is < 0 or > 90)
				throw new InvalidOperationException($"{key}: invalid recipe duration or Component Crafting prerequisite.");
			recipe = new(new System.Collections.ObjectModel.ReadOnlyDictionary<string, double>(inputs), kilograms, seconds, minimum);
		}
		return new(key, row.GetProperty("name").GetString()!, kinds[key], configuration.Clone(), Array.AsReadOnly(dependencies),
			references.AsReadOnly(), recipe);
	}
}
