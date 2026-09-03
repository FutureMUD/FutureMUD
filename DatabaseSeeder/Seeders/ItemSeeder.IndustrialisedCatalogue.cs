#nullable enable

using MudSharp.GameItems;
using MudSharp.Framework;
using MudSharp.Form.Material;
using MudSharp.Models;
using MudSharp.RPG.Checks;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace DatabaseSeeder.Seeders;

public partial class ItemSeeder
{
	private readonly IndustrialisedItemCatalogueDocument? _industrialisedCatalogueOverride;
	private IndustrialisedItemCatalogueDocument IndustrialisedCatalogue => _industrialisedCatalogueOverride ?? IndustrialisedItemCatalogue.Document;

	public ItemSeeder() { }

	internal ItemSeeder(IndustrialisedItemCatalogueDocument catalogue)
	{
		_industrialisedCatalogueOverride = catalogue;
	}

	private sealed record ResolvedIndustrialisedTechnologyProfile(
		string Key,
		IReadOnlyDictionary<string, IReadOnlyDictionary<string, IReadOnlyList<string>>> Bindings);

	internal static IndustrialisedItemCatalogueDocument IndustrialisedCatalogueForTesting =>
		IndustrialisedItemCatalogue.Document;

	private ResolvedIndustrialisedTechnologyProfile ResolveIndustrialisedTechnologyProfile()
	{
		var profileKey = _questionAnswers?.GetValueOrDefault("technologyprofile", "neutral").Trim().ToLowerInvariant() ?? "neutral";
		if (profileKey == "custom")
		{
			IReadOnlyList<string> Values(string key) => (_questionAnswers?.GetValueOrDefault(key, string.Empty) ?? string.Empty)
				.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
				.Distinct(StringComparer.OrdinalIgnoreCase)
				.OrderBy(x => x, StringComparer.OrdinalIgnoreCase)
				.ToArray();

			return new ResolvedIndustrialisedTechnologyProfile("custom",
				new Dictionary<string, IReadOnlyDictionary<string, IReadOnlyList<string>>>(StringComparer.OrdinalIgnoreCase)
				{
					["power"] = Family("mains", Values("technologypower")),
					["paper"] = Family("office", Values("technologypaper")),
					["telecommunications"] = Family("standard", Values("technologytelecom")),
					["networkmedia"] = Family("wired", Values("technologynetworkmedia")),
					["vehicle"] = Family("service", Values("technologyvehicle"))
				});
		}

		var rows = IndustrialisedCatalogue.TechnologyBindings
			.Where(x => x.Profile.Equals(profileKey, StringComparison.OrdinalIgnoreCase))
			.ToArray();
		if (rows.Length == 0)
		{
			throw new InvalidOperationException($"Technology profile {profileKey} has no binding map.");
		}

		return new ResolvedIndustrialisedTechnologyProfile(profileKey, rows
			.GroupBy(x => x.Dimension, StringComparer.OrdinalIgnoreCase)
			.ToDictionary(
				x => x.Key,
				x => (IReadOnlyDictionary<string, IReadOnlyList<string>>)x.ToDictionary(
					y => y.Family,
					y => y.Values,
					StringComparer.OrdinalIgnoreCase),
				StringComparer.OrdinalIgnoreCase));
	}

	private static IReadOnlyDictionary<string, IReadOnlyList<string>> Family(string name, IReadOnlyList<string> values) =>
		new Dictionary<string, IReadOnlyList<string>>(StringComparer.OrdinalIgnoreCase) { [name] = values };

	private IReadOnlyList<string> ResolveIndustrialisedComponents(
		IndustrialisedItemCatalogueRow row,
		ResolvedIndustrialisedTechnologyProfile profile)
	{
		var components = row.FixedComponents.ToList();
		foreach (var binding in row.ProfileBindings)
		{
			var parts = binding.Split(':', StringSplitOptions.TrimEntries);
			if (parts.Length != 2 || !profile.Bindings.TryGetValue(parts[0], out var families) ||
			    !families.TryGetValue(parts[1], out var values))
			{
				throw new InvalidOperationException($"{row.Source}:{row.Line}: profile {profile.Key} cannot resolve {binding}.");
			}

			var metadata = IndustrialisedCatalogue.TechnologyBindings.FirstOrDefault(x =>
				x.Profile.Equals(profile.Key, StringComparison.OrdinalIgnoreCase) &&
				x.Dimension.Equals(parts[0], StringComparison.OrdinalIgnoreCase) &&
				x.Family.Equals(parts[1], StringComparison.OrdinalIgnoreCase));
			var componentBacked = profile.Key == "custom" ? !parts[0].Equals("paper", StringComparison.OrdinalIgnoreCase) : metadata?.ComponentBacked == true;
			if (componentBacked)
			{
				components.AddRange(values);
			}
		}

		return components.Distinct(StringComparer.OrdinalIgnoreCase).OrderBy(x => x, StringComparer.OrdinalIgnoreCase).ToArray();
	}

	private void ValidateIndustrialisedCataloguePrerequisites(ResolvedIndustrialisedTechnologyProfile profile)
	{
		var catalogue = IndustrialisedCatalogue;
		var componentMetadata = IndustrialisedComponentMetadataCatalogue.Document;
		var itemByKey = catalogue.Items.ToDictionary(x => x.StableReference, StringComparer.OrdinalIgnoreCase);
		var issues = new List<string>();
		foreach (var row in catalogue.Items)
		{
			if (!_materials.TryGetValue(row.Material, out var material) || material.Type != (int)MaterialType.Solid)
			{
				issues.Add($"{row.Source}:{row.Line}: missing solid material {row.Material}");
			}
			foreach (var tag in row.Tags.Where(x => !_tagsByFullPath.ContainsKey(x)))
			{
				issues.Add($"{row.Source}:{row.Line}: missing tag {tag}");
			}
			IReadOnlyList<string> components;
			try
			{
				components = ResolveIndustrialisedComponents(row, profile);
			}
			catch (InvalidOperationException ex)
			{
				issues.Add(ex.Message);
				continue;
			}
			foreach (var component in components.Where(x => !_components.ContainsKey(x)))
			{
				issues.Add($"{row.Source}:{row.Line}: missing component {component}");
			}
			var metadataRows = components
				.Where(componentMetadata.Prototypes.ContainsKey)
				.Select(x => componentMetadata.Prototypes[x])
				.Where(x => componentMetadata.Types.ContainsKey(x.Type))
				.Select(x => (Prototype: x, Type: componentMetadata.Types[x.Type]))
				.ToArray();
			foreach (var component in components.Where(x => !componentMetadata.Prototypes.ContainsKey(x)))
			{
				issues.Add($"{row.Source}:{row.Line}: component {component} is absent from the embedded Stage 1 catalogue");
			}
			var capabilities = metadataRows.SelectMany(x => x.Type.Capabilities).ToHashSet(StringComparer.OrdinalIgnoreCase);
			foreach (var required in metadataRows.Where(x => !x.Type.ContextDependentRequirements)
				         .SelectMany(x => x.Type.RequiredSiblingTypes).Where(x => !capabilities.Contains(x)))
			{
				issues.Add($"{row.Source}:{row.Line}: missing required sibling capability {required}");
			}
			foreach (var exclusive in metadataRows.SelectMany(x => x.Type.ExclusiveTypes).Distinct(StringComparer.OrdinalIgnoreCase))
			{
				if (metadataRows.Count(x => x.Type.Capabilities.Contains(exclusive, StringComparer.OrdinalIgnoreCase)) > 1)
				{
					issues.Add($"{row.Source}:{row.Line}: multiple components provide exclusive capability {exclusive}");
				}
			}
			if (row.SupportedClaims.Contains("portable", StringComparer.OrdinalIgnoreCase) &&
			    !metadataRows.Any(x => x.Type.Name is "Holdable" or "Wearable"))
			{
				issues.Add($"{row.Source}:{row.Line}: portable claim lacks Holdable or Wearable capability");
			}
			if (row.Tags.Any(parent => row.Tags.Any(child => child.StartsWith(parent + " / ", StringComparison.OrdinalIgnoreCase))))
			{
				issues.Add($"{row.Source}:{row.Line}: contains a redundant parent tag");
			}
			if (row.DestroyedItem is not null && (!itemByKey.TryGetValue(row.DestroyedItem, out var target) ||
			    row.EraAdmissions.Except(target.EraAdmissions, StringComparer.OrdinalIgnoreCase).Any()))
			{
				issues.Add($"{row.Source}:{row.Line}: destroyed-item target {row.DestroyedItem} is missing or not admitted in every source era");
			}
		}

		foreach (var craft in catalogue.Crafts)
		{
			if (!itemByKey.TryGetValue(craft.ProductStableReference, out var product) ||
			    craft.EraAdmissions.Except(product.EraAdmissions, StringComparer.OrdinalIgnoreCase).Any())
			{
				issues.Add($"{craft.Source}:{craft.Line}: craft product is missing or has incompatible admissions");
			}
			if (!_materials.TryGetValue(craft.InputMaterial, out var material) || material.Type != (int)MaterialType.Solid)
			{
				issues.Add($"{craft.Source}:{craft.Line}: missing craft material {craft.InputMaterial}");
			}
		}

		if (issues.Count > 0 && !_manifestCaptureOnly)
		{
			throw new InvalidOperationException("Industrialised catalogue preflight failed before persistence:" +
				Environment.NewLine + string.Join(Environment.NewLine, issues.Take(100).Select(x => $" - {x}")) +
				(issues.Count > 100 ? $"{Environment.NewLine} - ...and {issues.Count - 100:N0} more issue(s)." : string.Empty));
		}
	}

	private void ValidateIndustrialisedTechnologyProfileImmutability(ResolvedIndustrialisedTechnologyProfile profile)
	{
		if (_manifestCaptureOnly || !_context!.SeederManagedRecords.Any(x =>
			x.Seeder == Name && !x.Retired && (x.Module == "shared-industrialised" || x.Module == "industrial")))
		{
			return;
		}

		var remembered = SeederAnswerMemory.GetLatestSeederAnswer(_context, Name, "technologyprofile")?.Trim().ToLowerInvariant();
		if (!string.IsNullOrWhiteSpace(remembered) && !remembered.Equals(profile.Key, StringComparison.OrdinalIgnoreCase))
		{
			throw new InvalidOperationException(
				$"This world already has managed Industrialised ItemSeeder content using technology profile '{remembered}'. " +
				$"Changing it to '{profile.Key}' requires a future component-graph migration workflow; no changes were made.");
		}
	}

	private void SeedIndustrialisedCatalogueItems(string eras)
	{
		// Recheck the complete new-clothing ownership batch before the first reuse or new-item write.
		foreach (var item in IndustrialisedCatalogue.Clothing.Bases.Where(x => _clothingPhysicalDefinitions.ContainsKey(x.ItemReference)))
			ResolveClothingPhysicalItem(_clothingPhysicalDefinitions[item.ItemReference], item.Source);
		SeedIndustrialisedClothingReuse(eras);
		var profile = ResolveIndustrialisedTechnologyProfile();
		var selected = ParseEraTokens(eras).ToHashSet(StringComparer.OrdinalIgnoreCase);
		var rows = IndustrialisedCatalogue.Items
			.Where(x => _manifestCaptureOnly || x.EraAdmissions.Any(selected.Contains))
			.OrderBy(x => x.Layer, StringComparer.Ordinal)
			.ThenBy(x => x.StableReference, StringComparer.Ordinal)
			.ToArray();

		foreach (var row in rows)
		{
			using var manifestModule = UseManifestModule(row.Layer, row.EraAdmissions.ToArray());
			CreateItem(
				row.StableReference,
				row.Noun,
				row.ShortDescription,
				null,
				row.FullDescription,
				row.Size,
				row.Quality,
				row.WeightGrams,
				row.CostIndex,
				IndustrialisedCatalogue.Clothing.Skins.Any(x => x.BaseItemReference == row.StableReference),
				false,
				row.Material,
				row.Tags,
				ResolveIndustrialisedComponents(row, profile),
				row.MorphTo,
				row.MorphEmote,
				row.MorphSeconds > 0 ? TimeSpan.FromSeconds(row.MorphSeconds) : null,
				row.DestroyedItem,
				null,
				allowLegacyShortDescriptionMatch: false);
		}
	}

	private void SeedIndustrialisedCatalogueCrafts(string eras)
	{
		var selected = ParseEraTokens(eras).ToHashSet(StringComparer.OrdinalIgnoreCase);
		foreach (var craft in IndustrialisedCatalogue.Crafts
			         .Where(x => _manifestCaptureOnly || x.EraAdmissions.Any(selected.Contains)))
		{
			if (!_itemsByStableReference.TryGetValue(craft.ProductStableReference, out var item))
			{
				throw new InvalidOperationException($"{craft.Source}:{craft.Line}: admitted craft product {craft.ProductStableReference} was not seeded.");
			}
			if (!Enum.TryParse<Difficulty>(craft.Difficulty, true, out var difficulty))
			{
				throw new InvalidOperationException($"{craft.Source}:{craft.Line}: invalid difficulty {craft.Difficulty}.");
			}
			using var manifestModule = UseManifestModule("crafts", craft.EraAdmissions.ToArray());
			var productName = item.ShortDescription.Strip_A_An();
			AddCraft(
				$"make {productName} [{craft.StableKey}]",
				craft.Category,
				$"Make {item.ShortDescription} from prepared {craft.InputMaterial} stock.",
				$"making {productName}",
				$"an in-progress {productName} craft",
				craft.Trait,
				craft.MinimumTraitValue,
				difficulty,
				Outcome.MinorFail,
				5,
				2,
				false,
				[
					(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work."),
					(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.")
				],
				[$"Commodity - {craft.InputGrams.ToString("0", System.Globalization.CultureInfo.InvariantCulture)} grams of {craft.InputMaterial}"],
				[],
				[$"SimpleProduct - 1x {item.ShortDescription} (#{item.Id})"],
				[]);
		}
		SeedIndustrialisedClothingCrafts(eras);
	}

	private void SeedIndustrialisedCatalogueOutfits(string eras)
	{
		var selected = ParseEraTokens(eras).ToHashSet(StringComparer.OrdinalIgnoreCase);
		var outfits = IndustrialisedCatalogue.Outfits
			.Where(x => _manifestCaptureOnly || x.EraAdmissions.Any(selected.Contains))
			.Select(x => new OutfitManifestSpec(
				x.OutfitReference,
				x.Name,
				x.Description,
				x.ItemStableReferences.Select(y => new OutfitManifestItemSpec(y, null)).ToArray()))
			.ToArray();
		using var manifestModule = UseManifestModule("outfits", "industrial", "modern", "nuclear", "information");
		UpsertOutfitManifests(outfits);
		SeedIndustrialisedClothingPresentations(eras);
	}
}
