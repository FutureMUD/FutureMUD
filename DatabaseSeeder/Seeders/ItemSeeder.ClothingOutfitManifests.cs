#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using MudSharp.Database;
using MudSharp.GameItems;
using MudSharp.Models;

namespace DatabaseSeeder.Seeders;

public partial class ItemSeeder
{
	private const string OutfitManifestMarkerPrefix = "[[ItemSeederOutfitManifest:";

	private sealed record DocumentedClothingItemSpec(
		string StableReference,
		string Noun,
		string ShortDescription,
		string FullDescription,
		SizeCategory Size,
		ItemQuality Quality,
		double WeightInGrams,
		decimal Cost,
		bool Skinnable,
		string Material,
		string[] Tags,
		string[] Components,
		string BuilderNotes);

	private sealed record OutfitManifestSpec(
		string StableKey,
		string Name,
		string Description,
		string[] ItemStableReferences);

	internal sealed record ClothingOutfitManifestTestData(
		string StableKey,
		string Name,
		IReadOnlyList<string> ItemStableReferences);

	internal static IReadOnlyList<ClothingOutfitManifestTestData> AntiquityOutfitManifestSpecsForTesting =>
		ToTestData(AntiquityOutfitManifestSpecs);

	internal static IReadOnlyList<ClothingOutfitManifestTestData> MedievalOutfitManifestSpecsForTesting =>
		ToTestData(MedievalOutfitManifestSpecs);

	internal static IReadOnlyList<ClothingOutfitManifestTestData> RenaissanceOutfitManifestSpecsForTesting =>
		ToTestData(RenaissanceOutfitManifestSpecs);

	internal static IReadOnlyList<ClothingOutfitManifestTestData> EarlyModernOutfitManifestSpecsForTesting =>
		ToTestData(EarlyModernOutfitManifestSpecs);

	internal static IReadOnlySet<string> DocumentedClothingItemStableReferencesForTesting =>
		AntiquityOutfitSupplementalItemSpecs
			.Concat(RenaissanceOutfitReferencedItemSpecs)
			.Concat(EarlyModernOutfitReferencedItemSpecs)
			.Select(x => x.StableReference)
			.ToHashSet(StringComparer.OrdinalIgnoreCase);

	internal static IReadOnlySet<string> RenaissanceOutfitItemStableReferencesForTesting =>
		RenaissanceOutfitReferencedItemSpecs
			.Select(x => x.StableReference)
			.ToHashSet(StringComparer.OrdinalIgnoreCase);

	internal static IReadOnlyDictionary<string, string> RenaissanceOutfitWearComponentsForTesting =>
		RenaissanceOutfitReferencedItemSpecs
			.ToDictionary(
				x => x.StableReference,
				x => x.Components.Single(component => component.StartsWith("Wear_", StringComparison.Ordinal)),
				StringComparer.OrdinalIgnoreCase);

	private static IReadOnlyList<ClothingOutfitManifestTestData> ToTestData(IEnumerable<OutfitManifestSpec> specs)
	{
		return specs
			.Select(x => new ClothingOutfitManifestTestData(x.StableKey, x.Name, x.ItemStableReferences))
			.ToArray();
	}

	private void SeedDocumentedClothingOutfitManifests(string eras)
	{
		if (HasAnyEra(eras, "antiquity"))
		{
			SeedDocumentedClothingItems(AntiquityOutfitSupplementalItemSpecs);
			UpsertOutfitManifests(AntiquityOutfitManifestSpecs);
		}

		if (HasAnyEra(eras, "medieval"))
		{
			UpsertOutfitManifests(MedievalOutfitManifestSpecs);
		}

		if (HasAnyEra(eras, "renaissance"))
		{
			SeedDocumentedClothingItems(RenaissanceOutfitReferencedItemSpecs);
			UpsertOutfitManifests(RenaissanceOutfitManifestSpecs);
		}

		if (HasAnyEra(eras, "earlymodern"))
		{
			SeedDocumentedClothingItems(EarlyModernOutfitReferencedItemSpecs);
			UpsertOutfitManifests(EarlyModernOutfitManifestSpecs);
		}
	}

	private void SeedDocumentedClothingItems(IEnumerable<DocumentedClothingItemSpec> specs)
	{
		var itemSpecs = specs.ToArray();
		var dependencyIssues = ValidateDocumentedClothingItemDependencies(itemSpecs);
		if (dependencyIssues.Count > 0)
		{
			throw new InvalidOperationException(
				"Documented clothing outfit items cannot be seeded because required dependencies are missing:" +
				Environment.NewLine + string.Join(Environment.NewLine, dependencyIssues.Select(x => $" - {x}")));
		}

		foreach (var spec in itemSpecs)
		{
			var item = CreateItem(
				spec.StableReference,
				spec.Noun,
				spec.ShortDescription,
				null,
				spec.FullDescription,
				spec.Size,
				spec.Quality,
				spec.WeightInGrams,
				spec.Cost,
				spec.Skinnable,
				false,
				spec.Material,
				spec.Tags,
				spec.Components,
				null,
				null,
				null,
				null,
				spec.BuilderNotes,
				allowLegacyShortDescriptionMatch: false);
			if (item is null)
			{
				throw new InvalidOperationException(
					$"Unable to seed documented clothing outfit item {spec.StableReference}.");
			}
		}
	}

	private IReadOnlyList<string> ValidateDocumentedClothingItemDependencies(
		IEnumerable<DocumentedClothingItemSpec> specs)
	{
		var issues = new List<string>();
		foreach (var spec in specs)
		{
			if (!_materials.ContainsKey(spec.Material))
			{
				issues.Add($"Missing material {spec.Material} for {spec.StableReference}");
			}

			issues.AddRange(spec.Tags
				.Where(x => !_tagsByFullPath.ContainsKey(x))
				.Select(x => $"Missing tag {x} for {spec.StableReference}"));
			issues.AddRange(spec.Components
				.Where(x => !_components.ContainsKey(x))
				.Select(x => $"Missing component {x} for {spec.StableReference}"));
		}

		return issues
			.Distinct(StringComparer.OrdinalIgnoreCase)
			.ToArray();
	}

	private void UpsertOutfitManifests(IEnumerable<OutfitManifestSpec> manifests)
	{
		if (_context is null)
		{
			throw new InvalidOperationException("The item seeder context must be initialised before outfit manifests are seeded.");
		}

		var templates = _context.OutfitTemplates
			.Include(x => x.OutfitTemplateItems)
			.AsEnumerable()
			.ToList();
		foreach (var manifest in manifests)
		{
			UpsertOutfitManifest(_context, manifest, _items, templates);
		}
	}

	private static OutfitTemplate UpsertOutfitManifest(
		FuturemudDatabaseContext context,
		OutfitManifestSpec manifest,
		IReadOnlyDictionary<string, GameItemProto> itemPrototypes,
		ICollection<OutfitTemplate> templates)
	{
		if (manifest.Name.Length > 200)
		{
			throw new InvalidOperationException(
				$"Outfit manifest {manifest.StableKey} has a name longer than the database limit of 200 characters.");
		}

		var duplicateReferences = manifest.ItemStableReferences
			.GroupBy(x => x, StringComparer.OrdinalIgnoreCase)
			.Where(x => x.Count() > 1)
			.Select(x => x.Key)
			.ToArray();
		if (duplicateReferences.Length > 0)
		{
			throw new InvalidOperationException(
				$"Outfit manifest {manifest.StableKey} repeats item references: {string.Join(", ", duplicateReferences)}.");
		}

		var missingReferences = manifest.ItemStableReferences
			.Where(x => !itemPrototypes.ContainsKey(x))
			.ToArray();
		if (missingReferences.Length > 0)
		{
			throw new InvalidOperationException(
				$"Outfit manifest {manifest.StableKey} references missing item prototypes: {string.Join(", ", missingReferences)}.");
		}

		var marker = GetOutfitManifestMarker(manifest.StableKey);
		var ownedMatches = templates
			.Where(x => HasOutfitManifestMarker(x.Description, marker))
			.ToArray();
		if (ownedMatches.Length > 1)
		{
			throw new InvalidOperationException(
				$"Multiple outfit templates claim stock manifest key {manifest.StableKey}.");
		}

		var nameMatch = templates.FirstOrDefault(x => x.Name.Equals(manifest.Name, StringComparison.OrdinalIgnoreCase));
		var outfitTemplate = ownedMatches.SingleOrDefault();
		if (outfitTemplate is null && nameMatch is not null)
		{
			throw new InvalidOperationException(
				$"Cannot seed stock outfit manifest {manifest.StableKey} because a builder-authored template already uses the name {manifest.Name}.");
		}

		if (outfitTemplate is not null && nameMatch is not null && !ReferenceEquals(outfitTemplate, nameMatch))
		{
			throw new InvalidOperationException(
				$"Cannot restore stock outfit manifest {manifest.StableKey} to the name {manifest.Name} because another template already uses it.");
		}

		if (outfitTemplate is null)
		{
			outfitTemplate = new OutfitTemplate();
			context.OutfitTemplates.Add(outfitTemplate);
			templates.Add(outfitTemplate);
		}
		else
		{
			context.OutfitTemplateItems.RemoveRange(outfitTemplate.OutfitTemplateItems);
			outfitTemplate.OutfitTemplateItems.Clear();
		}

		outfitTemplate.Name = manifest.Name;
		outfitTemplate.Description = $"{manifest.Description}{Environment.NewLine}{marker}";
		outfitTemplate.Exclusivity = (int)OutfitExclusivity.NonExclusive;

		foreach (var (stableReference, wearOrder) in manifest.ItemStableReferences.Select((x, index) => (x, index)))
		{
			outfitTemplate.OutfitTemplateItems.Add(new OutfitTemplateItem
			{
				TemplateKey = stableReference,
				GameItemProtoId = itemPrototypes[stableReference].Id,
				WearProfileId = null,
				Placement = (int)OutfitTemplateItemPlacement.Worn,
				ContainerKey = null,
				LoadArguments = string.Empty,
				WearOrder = wearOrder
			});
		}

		return outfitTemplate;
	}

	private static string GetOutfitManifestMarker(string stableKey)
	{
		return $"{OutfitManifestMarkerPrefix}{stableKey}]]";
	}

	private static bool HasOutfitManifestMarker(string description, string marker)
	{
		return description
			.Split(["\r\n", "\n"], StringSplitOptions.RemoveEmptyEntries)
			.Any(x => x.Equals(marker, StringComparison.Ordinal));
	}

	internal static OutfitTemplate UpsertOutfitManifestForTesting(
		FuturemudDatabaseContext context,
		string stableKey,
		string name,
		string description,
		IEnumerable<(string StableReference, GameItemProto Prototype)> items)
	{
		var itemList = items.ToArray();
		var templates = context.OutfitTemplates
			.Include(x => x.OutfitTemplateItems)
			.AsEnumerable()
			.ToList();
		return UpsertOutfitManifest(
			context,
			new OutfitManifestSpec(stableKey, name, description, itemList.Select(x => x.StableReference).ToArray()),
			itemList.ToDictionary(x => x.StableReference, x => x.Prototype, StringComparer.OrdinalIgnoreCase),
			templates);
	}
}
