#nullable enable

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;

namespace DatabaseSeeder.Seeders;

internal sealed record ClothingDependencyPlanRow(ClothingSourceLocation Source, string PlanningKey,
	string ItemReference, IReadOnlyList<string> EraAdmissions, bool Reused, string Material,
	IReadOnlyList<string> Components, IReadOnlyList<string> Tags, string? WearProfile,
	double? RequiredLayerWeight, IReadOnlyList<string> OpenRequirements);

internal sealed record ClothingWearLayerStock(string Name, string SourceComponent, double LayerWeight, bool? Bulky = null)
{
	internal string Description => $"Clothing wear configuration based on {SourceComponent}, consuming {LayerWeight.ToString("R", CultureInfo.InvariantCulture)} layer weight." +
		(Bulky is { } bulky ? bulky ? " This configuration is bulky." : " This configuration permits non-bulky layering." : "");
}

/// <summary>
/// Full approved-base prerequisite inventory, separate from finished item authoring. Entries record
/// proposed physical bindings and unresolved work; membership is not proof of runtime validity.
/// This developer catalogue never creates items, descriptions, recipes or a selectable package.
/// </summary>
internal static partial class IndustrialisedClothingDependencyPlan
{
	private sealed record NewBaseDependency(string Key, string Eras, string Material, string WearProfile,
		double LayerWeight, string Colours, string? Storage, string? Insulation, string? Extra,
		string Tag, string? Work, bool? Bulky, int Line);

	private static NewBaseDependency N(string key, string eras, string material, string wearProfile,
		double layerWeight, string colours = "2Colour", string? storage = null, string? insulation = null,
		string? extra = null, string tag = "Bodywear", string? work = null, bool? bulky = null, [CallerLineNumber] int line = 0) =>
		new(key, eras, material, wearProfile.Replace('_', ' '), layerWeight, colours, storage, insulation, extra, tag, work, bulky, line);

	private static readonly Lazy<IReadOnlyList<ClothingDependencyPlanRow>> Plan = new(Build);
	internal static IReadOnlyList<ClothingDependencyPlanRow> Rows => Plan.Value;

	// Consume authored geometry/thickness pairs only. This is not an item/description generator and
	// does not evaluate the historical-item plan while HumanSeeder's stock is being initialised.
	internal static IReadOnlyList<ClothingWearLayerStock> WearLayerStock => Array.AsReadOnly(NewBases
		.Where(x => x.LayerWeight != 1.0 || x.Bulky is not null)
		.Select(x => new ClothingWearLayerStock(WearComponent(x.WearProfile, x.LayerWeight, x.Bulky),
			$"Wear_{x.WearProfile.Replace(' ', '_')}", x.LayerWeight, x.Bulky))
		.Concat(HistoricalWearLayerStock)
		.Distinct()
		.OrderBy(x => x.Name, StringComparer.Ordinal)
		.ToArray());

	// Consumed by the explicit historical source corrections; these are reusable component
	// configurations, not permission to substitute every robe or change a shared profile globally.
	private static readonly ClothingWearLayerStock[] HistoricalWearLayerStock =
	[
		new("Wear_Garters_Layer_0_1", "Wear_Garters", 0.1),
		new("Wear_Waist_Layer_0_1", "Wear_Waist", 0.1),
		new("Wear_Long-Sleeved_Gown_Layer_0_25_NonBulky", "Wear_Long-Sleeved_Gown", 0.25, false),
		new("Wear_Robe_Layer_0_5_NonBulky", "Wear_Robe", 0.5, false),
		new("Wear_Robe_Layer_0_75_NonBulky", "Wear_Robe", 0.75, false)
	];

	private static string WearComponent(string profile, double layerWeight, bool? bulky)
	{
		if (!double.IsFinite(layerWeight) || layerWeight < 0)
			throw new InvalidOperationException($"Invalid clothing layer weight for {profile}.");
		var name = $"Wear_{profile.Replace(' ', '_')}";
		var layered = layerWeight == 1.0 ? name : $"{name}_Layer_{layerWeight.ToString("R", CultureInfo.InvariantCulture).Replace('.', '_')}";
		return bulky is { } value ? $"{layered}_{(value ? "Bulky" : "NonBulky")}" : layered;
	}

	private static IReadOnlyList<ClothingDependencyPlanRow> Build()
	{
		var rows = new List<ClothingDependencyPlanRow>();
		foreach (var item in NewBases)
		{
			var admissions = Admissions(item.Eras);
			var prefix = admissions.Count == 1 ? admissions[0] : "industrialised";
			var components = new List<string>
			{
				"Holdable", "Destroyable_Clothing", WearComponent(item.WearProfile, item.LayerWeight, item.Bulky), $"Variable_{item.Colours}"
			};
			if (item.Storage is not null) components.Add($"Container_{item.Storage}");
			if (item.Insulation is not null) components.Add($"Insulation_{item.Insulation}");
			if (item.Extra is not null) components.Add(item.Extra);
			var requirements = new List<string>
			{
				"Validate actual wear geometry and the requested layer consumption; stock thickness variants preserve shared historical components rather than changing them globally.",
				"Bind every colour/finish channel and material-appropriate palette, including standalone defaults and compatible skin accents.",
				"Prove complete outfit anatomy, fit, optional/shape placement and attachments before Gate 5 outfit acceptance."
			};
			if (item.Work is not null) requirements.Add(item.Work);
			rows.Add(new(new("DatabaseSeeder/Seeders/IndustrialisedClothingDependencyPlan.Data.cs", item.Line), item.Key,
				$"{prefix}_clothing_{item.Key}", admissions, false, item.Material, components.AsReadOnly(),
				Array.AsReadOnly(new[] { $"Functions / Worn Items / {item.Tag}" }), item.WearProfile, item.LayerWeight,
				requirements.AsReadOnly()));
		}
		foreach (var source in ItemSeeder.ApprovedHistoricalClothingSourcesForAudit())
		{
			// Source graphs are preserved verbatim here. Missing variables, poor wear geometry and
			// unsupported stock comments must remain visible rather than being silently 'fixed' in an audit.
			rows.Add(new(ItemSeeder.HistoricalClothingSourceProviderLocation, source.StableReference,
				source.StableReference, Admissions("IMNF"), true, source.Material,
				Array.AsReadOnly(source.Components.ToArray()), Array.AsReadOnly(source.Tags.ToArray()), null, null,
				Array.AsReadOnly(new[]
				{
					"Resolve actual attached/proposed component revisions and retain the historical owner and stable identity.",
					"Repair missing/incompatible variable bindings and fixed-colour prose through managed source updates; conventional colours are outfit defaults.",
					"Review exact coverage/layering and material-native finishes against every admitted outfit; historical source existence is not physical proof."
				})));
		}
		var duplicate = rows.GroupBy(x => x.ItemReference, StringComparer.Ordinal).FirstOrDefault(x => x.Count() != 1);
		if (duplicate is not null) throw duplicate.First().Source.Error($"Duplicate dependency-plan item {duplicate.Key}.");
		return rows.OrderBy(x => x.ItemReference, StringComparer.Ordinal).ToList().AsReadOnly();
	}

	private static IReadOnlyList<string> Admissions(string letters)
	{
		var names = new Dictionary<char, string> { ['I'] = "industrial", ['M'] = "modern", ['N'] = "nuclear", ['F'] = "information" };
		if (letters.Length == 0 || letters.Any(x => !names.ContainsKey(x)) ||
			new string("IMNF".Where(letters.Contains).ToArray()) != letters)
			throw new InvalidOperationException($"Invalid dependency-plan admissions {letters}.");
		return Array.AsReadOnly(letters.Select(x => names[x]).ToArray());
	}
}
