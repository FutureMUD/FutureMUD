#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using MudSharp.Database;
using MudSharp.Framework.Revision;
using MudSharp.GameItems;
using MudSharp.Models;
using DbGameItemSkin = MudSharp.Models.GameItemSkin;

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

	private sealed record DocumentedClothingSkinSpec(
		string StableReference,
		string BaseItemStableReference,
		string ItemName,
		string ShortDescription,
		string FullDescription,
		ItemQuality Quality);

	private sealed record OutfitManifestItemSpec(
		string ItemStableReference,
		string? SkinStableReference)
	{
		public static implicit operator OutfitManifestItemSpec(string itemStableReference)
		{
			return new OutfitManifestItemSpec(itemStableReference, null);
		}
	}

	private sealed record OutfitManifestSpec(
		string StableKey,
		string Name,
		string Description,
		OutfitManifestItemSpec[] Items)
	{
		public IReadOnlyList<string> ItemStableReferences => Items
			.Select(x => x.ItemStableReference)
			.ToArray();
	}

	internal sealed record ClothingOutfitManifestItemTestData(
		string ItemStableReference,
		string? SkinStableReference);

	internal sealed record ClothingOutfitManifestTestData(
		string StableKey,
		string Name,
		string Description,
		IReadOnlyList<ClothingOutfitManifestItemTestData> Items)
	{
		public IReadOnlyList<string> ItemStableReferences => Items
			.Select(x => x.ItemStableReference)
			.ToArray();
	}

	internal sealed record ClothingItemDescriptionTestData(
		string StableReference,
		string FullDescription);

	internal sealed record DocumentedClothingSkinTestData(
		string StableReference,
		string BaseItemStableReference,
		string ItemName,
		string ShortDescription,
		string FullDescription,
		ItemQuality Quality);

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
			.Concat(RenaissanceClothingItemSpecs)
			.Concat(EarlyModernOutfitReferencedItemSpecs)
			.Select(x => x.StableReference)
			.ToHashSet(StringComparer.OrdinalIgnoreCase);

	internal static IReadOnlyList<ClothingItemDescriptionTestData> DocumentedClothingItemDescriptionsForTesting =>
		AntiquityOutfitSupplementalItemSpecs
			.Concat(RenaissanceClothingItemSpecs)
			.Concat(EarlyModernOutfitReferencedItemSpecs)
			.Select(x => new ClothingItemDescriptionTestData(x.StableReference, x.FullDescription))
			.ToArray();

	internal static IReadOnlyList<DocumentedClothingSkinTestData> DocumentedClothingSkinsForTesting =>
		DocumentedClothingSkinSpecs
			.Select(x => new DocumentedClothingSkinTestData(
				x.StableReference,
				x.BaseItemStableReference,
				x.ItemName,
				x.ShortDescription,
				x.FullDescription,
				x.Quality))
			.ToArray();

	internal static IReadOnlySet<string> RenaissanceOutfitItemStableReferencesForTesting =>
		RenaissanceClothingItemSpecs
			.Where(x => RenaissanceOutfitManifestSpecs.Any(outfit =>
				outfit.ItemStableReferences.Contains(x.StableReference, StringComparer.OrdinalIgnoreCase)))
			.Select(x => x.StableReference)
			.ToHashSet(StringComparer.OrdinalIgnoreCase);

	internal static IReadOnlySet<string> RenaissanceClothingItemStableReferencesForTesting =>
		RenaissanceClothingItemSpecs
			.Select(x => x.StableReference)
			.ToHashSet(StringComparer.OrdinalIgnoreCase);

	internal static IReadOnlyList<string> EarlyModernFifthPassStandaloneItemStableReferencesForTesting =>
		EarlyModernOutfitReferencedItemSpecs
			.Where(x => x.StableReference.StartsWith("earlymodern_headwear_", StringComparison.Ordinal) ||
			            x.StableReference.StartsWith("earlymodern_footwear_", StringComparison.Ordinal))
			.Select(x => x.StableReference)
			.ToArray();

	internal static IReadOnlyDictionary<string, string> RenaissanceOutfitWearComponentsForTesting =>
		RenaissanceClothingItemSpecs
			.Where(x => RenaissanceOutfitManifestSpecs.Any(outfit =>
				outfit.ItemStableReferences.Contains(x.StableReference, StringComparer.OrdinalIgnoreCase)))
			.ToDictionary(
				x => x.StableReference,
				x => x.Components.Single(component => component.StartsWith("Wear_", StringComparison.Ordinal)),
				StringComparer.OrdinalIgnoreCase);

	private static IReadOnlyList<ClothingOutfitManifestTestData> ToTestData(IEnumerable<OutfitManifestSpec> specs)
	{
		return specs
			.Select(x => new ClothingOutfitManifestTestData(
				x.StableKey,
				x.Name,
				x.Description,
				x.Items
					.Select(item => new ClothingOutfitManifestItemTestData(
						item.ItemStableReference,
						item.SkinStableReference))
					.ToArray()))
			.ToArray();
	}

	private static string BuildDocumentedClothingFullDescription(
		string shortDescription,
		string noun,
		string material,
		IReadOnlyCollection<string> components,
		ItemQuality quality)
	{
		var article = char.ToUpperInvariant(shortDescription[0]) + shortDescription[1..];
		var materialKey = material.ToLowerInvariant();
		string construction;
		string materialDetail;
		if (new[] { "gold", "silver", "brass", "bronze", "iron", "steel" }.Contains(materialKey))
		{
			construction = "worked and joined";
			materialDetail = $"The {material} has been smoothed on the broad faces while shallow tool traces remain around the joins and recessed edges.";
		}
		else if (new[] { "leather", "deer leather", "rawhide", "fur" }.Contains(materialKey))
		{
			construction = "cut and stitched";
			materialDetail = $"The {material} shows a supple grain across the larger panels, with doubled edges and close stitching where repeated movement would otherwise pull it out of shape.";
		}
		else if (new[] { "wood", "straw", "raffia cloth", "barkcloth", "featherwork", "beadwork", "horsehair" }.Contains(materialKey))
		{
			construction = "shaped and bound";
			materialDetail = $"The {material} keeps its natural texture visible, and the bindings follow the change from broad surfaces to narrower edges without hiding how the piece was assembled.";
		}
		else
		{
			construction = "cut and sewn";
			materialDetail = $"The {material} falls in visible folds between reinforced seams, with the weave left clear at the hems and turned edges.";
		}

		var wearComponent = components.FirstOrDefault(x => x.StartsWith("Wear_", StringComparison.Ordinal)) ?? string.Empty;
		string formDetail;
		if (new[] { "Boot", "Shoe", "Sandal", "Stocking", "Leg_Wrap" }.Any(wearComponent.Contains))
			formDetail = $"The {noun} is built around the foot and ankle, with a firm lower edge and an opening arranged for secure wear without disguising the shape described above.";
		else if (new[] { "Hat", "Hood", "Turban", "Veil", "Mask", "Coif" }.Any(wearComponent.Contains))
			formDetail = $"Its crown, folds, or framing edges hold the {noun} around the head while leaving the characteristic outline plainly visible from the front and side.";
		else if (new[] { "Trousers", "Breeches", "Skirt", "Loincloth", "Breechcloth" }.Any(wearComponent.Contains))
			formDetail = $"A reinforced waist carries the {noun}, from which the lower panels fall with enough room for ordinary movement while retaining their deliberate cut.";
		else if (new[] { "Robe", "Dress", "Cloak", "Cape", "Mantle", "Tabard" }.Any(wearComponent.Contains))
			formDetail = $"The main panels settle from the shoulders into a controlled fall, and the hem and opening give the {noun} its recognisable proportion when worn.";
		else if (new[] { "Glove", "Sleeve", "Shirt", "Tunic", "Jacket", "Vest", "Bra" }.Any(wearComponent.Contains))
			formDetail = $"The body and openings are proportioned for close, practical wear, with reinforcement placed where the {noun} bends or fastens rather than spread as decoration.";
		else
			formDetail = $"The contact points and fastenings are kept smooth, while the visible body of the {noun} carries the shape and surface detail that distinguish it at a glance.";

		var finish = quality switch
		{
			ItemQuality.Standard => "The finish is practical and even, though small irregularities at the less-visible seams show ordinary hand work.",
			ItemQuality.Good => "Careful finishing keeps the seams, borders, and fastenings even, with only discreet hand-worked variation remaining.",
			ItemQuality.VeryGood => "Fine finishing has made the borders and fastenings exceptionally even, with ornament and structure resolved cleanly rather than heavily.",
			ItemQuality.Great => "Exceptionally precise finishing leaves the borders, joins, and ornament balanced from every commonly viewed angle.",
			_ => "The finish is serviceable, with the construction left legible wherever close inspection reaches an edge or join."
		};
		return $"{article} is {construction} from {material} so that the outward silhouette of the {noun} remains clear. {materialDetail} {formDetail} {finish}";
	}

	private void SeedDocumentedClothingOutfitManifests(string eras)
	{
		var manifests = new List<OutfitManifestSpec>();
		if (HasAnyEra(eras, "antiquity"))
		{
			SeedDocumentedClothingItems(AntiquityOutfitSupplementalItemSpecs);
			manifests.AddRange(AntiquityOutfitManifestSpecs);
		}

		if (HasAnyEra(eras, "medieval"))
		{
			manifests.AddRange(MedievalOutfitManifestSpecs);
		}

		if (HasAnyEra(eras, "renaissance"))
		{
			SeedDocumentedClothingItems(RenaissanceClothingItemSpecs);
			manifests.AddRange(RenaissanceOutfitManifestSpecs);
		}

		if (HasAnyEra(eras, "earlymodern"))
		{
			SeedDocumentedClothingItems(EarlyModernOutfitReferencedItemSpecs);
			manifests.AddRange(EarlyModernOutfitManifestSpecs);
		}

		var skins = SeedDocumentedClothingSkins(manifests
			.SelectMany(x => x.Items)
			.Where(x => x.SkinStableReference is not null)
			.Select(x => x.SkinStableReference!));
		UpsertOutfitManifests(manifests, skins);
	}

	private void SeedDocumentedClothingItems(IEnumerable<DocumentedClothingItemSpec> specs)
	{
		var itemSpecs = specs.ToArray();
		var dependencyIssues = ValidateDocumentedClothingItemDependencies(itemSpecs);
		if (!_manifestCaptureOnly && dependencyIssues.Count > 0)
		{
			throw new InvalidOperationException(
				"Documented clothing outfit items cannot be seeded because required dependencies are missing:" +
				Environment.NewLine + string.Join(Environment.NewLine, dependencyIssues.Select(x => $" - {x}")));
		}

		foreach (var spec in itemSpecs)
		{
			// Generated outfit admissions can reference a shared item already authored by an earlier
			// executable module. The earlier stock definition remains the single authority.
			if (IsManifestAggregateRegistered("item", spec.StableReference))
			{
				continue;
			}

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

	private IReadOnlyDictionary<string, DbGameItemSkin> SeedDocumentedClothingSkins(
		IEnumerable<string> requestedStableReferences)
	{
		if (_context is null)
		{
			throw new InvalidOperationException("The item seeder context must be initialised before documented clothing skins are seeded.");
		}

		var requested = requestedStableReferences
			.Distinct(StringComparer.OrdinalIgnoreCase)
			.ToHashSet(StringComparer.OrdinalIgnoreCase);
		if (requested.Count == 0)
		{
			return new Dictionary<string, DbGameItemSkin>(StringComparer.OrdinalIgnoreCase);
		}

		var specs = DocumentedClothingSkinSpecs
			.Where(x => requested.Contains(x.StableReference))
			.ToArray();
		var unknown = requested
			.Except(specs.Select(x => x.StableReference), StringComparer.OrdinalIgnoreCase)
			.ToArray();
		if (unknown.Length > 0)
		{
			throw new InvalidOperationException(
				$"Documented clothing outfits reference unknown skin keys: {string.Join(", ", unknown)}.");
		}

		var alwaysTrue = _context.FutureProgs.SingleOrDefault(x => x.FunctionName == "AlwaysTrue");
		if (alwaysTrue is null && !_manifestCaptureOnly)
		{
			throw new InvalidOperationException("Documented clothing skins require the existing AlwaysTrue FutureProg.");
		}
		var alwaysTrueName = alwaysTrue?.FunctionName ?? "AlwaysTrue";
		var currentSkins = _context.GameItemSkins
			.Include(x => x.EditableItem)
			.AsEnumerable()
			.Where(x => x.EditableItem?.RevisionStatus == (int)RevisionStatus.Current)
			.GroupBy(x => x.Name, StringComparer.OrdinalIgnoreCase)
			.ToDictionary(
				x => x.Key,
				x => x.Single(),
				StringComparer.OrdinalIgnoreCase);
		var nextSkinId = _context.GameItemSkins.Any()
			? _context.GameItemSkins.Max(x => x.Id) + 1
			: 1;
		var resolved = new Dictionary<string, DbGameItemSkin>(StringComparer.OrdinalIgnoreCase);

		foreach (var spec in specs)
		{
			if (!_itemsByStableReference.TryGetValue(spec.BaseItemStableReference, out var baseItem))
			{
				throw new InvalidOperationException(
					$"Documented clothing skin {spec.StableReference} references missing item prototype {spec.BaseItemStableReference}.");
			}

			var expectedDefinition = new ItemSkinManifestDefinition(
				spec.StableReference,
				spec.BaseItemStableReference,
				spec.ItemName,
				spec.ShortDescription,
				spec.FullDescription,
				null,
				(int)spec.Quality,
				false,
				alwaysTrueName);
			var manifestEntry = RegisterManifestAggregate(
				"item-skin",
				spec.StableReference,
				expectedDefinition,
				[$"item:{spec.BaseItemStableReference}"]);
			if (_manifestCaptureOnly)
			{
				continue;
			}

			if (currentSkins.TryGetValue(spec.StableReference, out var existing))
			{
				var liveDefinition = BuildLiveDocumentedClothingSkinDefinition(existing);
				var disposition = InspectManifestAggregate(manifestEntry, existing.Id, liveDefinition);
				if (disposition is ManifestAggregateDisposition.Customized or ManifestAggregateDisposition.Unchanged)
				{
					resolved.Add(spec.StableReference, existing);
					continue;
				}

				ApplyDocumentedClothingSkinSpec(existing, spec, baseItem, alwaysTrue!);
				CompleteManifestAggregate(manifestEntry, existing.Id, expectedDefinition, disposition);
				resolved.Add(spec.StableReference, existing);
				continue;
			}

			var skin = new DbGameItemSkin
			{
				Id = nextSkinId++,
				RevisionNumber = 0,
				Name = spec.StableReference,
				ItemProtoId = baseItem.Id,
				ItemName = spec.ItemName,
				ShortDescription = spec.ShortDescription,
				FullDescription = spec.FullDescription,
				LongDescription = null,
				Quality = (int)spec.Quality,
				IsPublic = false,
				CanUseSkinProgId = alwaysTrue!.Id,
				EditableItem = new EditableItem
				{
					RevisionNumber = 0,
					RevisionStatus = (int)RevisionStatus.Current,
					BuilderAccountId = _dbAccount.Id,
					BuilderDate = _now,
					BuilderComment = "Auto-generated by the system",
					ReviewerAccountId = _dbAccount.Id,
					ReviewerComment = "Auto-generated by the system",
					ReviewerDate = _now
				}
			};
			_context.GameItemSkins.Add(skin);
			CompleteManifestAggregate(manifestEntry, skin.Id, expectedDefinition, ManifestAggregateDisposition.Insert);
			resolved.Add(spec.StableReference, skin);
		}

		return resolved;
	}

	private ItemSkinManifestDefinition BuildLiveDocumentedClothingSkinDefinition(DbGameItemSkin skin)
	{
		var canUseSkinProg = skin.CanUseSkinProgId is null
			? null
			: _context!.FutureProgs
				.Where(x => x.Id == skin.CanUseSkinProgId.Value)
				.Select(x => x.FunctionName)
				.SingleOrDefault();
		return new ItemSkinManifestDefinition(
			skin.Name,
			ResolveItemStableReference(skin.ItemProtoId) ?? $"<missing:{skin.ItemProtoId}>",
			skin.ItemName,
			skin.ShortDescription,
			skin.FullDescription,
			skin.LongDescription,
			skin.Quality,
			skin.IsPublic,
			canUseSkinProg ?? $"<missing:{skin.CanUseSkinProgId}>");
	}

	private static void ApplyDocumentedClothingSkinSpec(
		DbGameItemSkin skin,
		DocumentedClothingSkinSpec spec,
		GameItemProto baseItem,
		FutureProg alwaysTrue)
	{
		skin.Name = spec.StableReference;
		skin.ItemProtoId = baseItem.Id;
		skin.ItemName = spec.ItemName;
		skin.ShortDescription = spec.ShortDescription;
		skin.FullDescription = spec.FullDescription;
		skin.LongDescription = null;
		skin.Quality = (int)spec.Quality;
		skin.IsPublic = false;
		skin.CanUseSkinProgId = alwaysTrue.Id;
	}

	private void UpsertOutfitManifests(IEnumerable<OutfitManifestSpec> manifests)
	{
		UpsertOutfitManifests(
			manifests,
			new Dictionary<string, DbGameItemSkin>(StringComparer.OrdinalIgnoreCase));
	}

	private void UpsertOutfitManifests(
		IEnumerable<OutfitManifestSpec> manifests,
		IReadOnlyDictionary<string, DbGameItemSkin> skins)
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
			var marker = GetOutfitManifestMarker(manifest.StableKey);
			var manifestDefinition = new OutfitManifestDefinition(
				manifest.StableKey,
				manifest.Name,
				$"{manifest.Description}{Environment.NewLine}{marker}",
				(int)OutfitExclusivity.NonExclusive,
				manifest.Items
					.Select(x => new OutfitManifestItemDefinition(
						x.ItemStableReference,
						x.SkinStableReference))
					.ToArray());
			var manifestEntry = RegisterManifestAggregate(
				"outfit",
				manifest.StableKey,
				manifestDefinition,
				manifest.Items
					.Select(x => $"item:{x.ItemStableReference}")
					.Concat(manifest.Items
						.Where(x => x.SkinStableReference is not null)
						.Select(x => $"item-skin:{x.SkinStableReference}")));
			if (_manifestCaptureOnly)
			{
				continue;
			}

			var existing = templates.SingleOrDefault(x => HasOutfitManifestMarker(x.Description, marker));
			if (existing is not null)
			{
				var liveDefinition = new OutfitManifestDefinition(
					manifest.StableKey,
					existing.Name,
					existing.Description,
					existing.Exclusivity,
					existing.OutfitTemplateItems
						.OrderBy(x => x.WearOrder)
						.Select(x => new OutfitManifestItemDefinition(
							x.TemplateKey,
							x.SkinId is null
								? null
								: skins.Values.SingleOrDefault(y => y.Id == x.SkinId.Value)?.Name ??
								  $"<missing:{x.SkinId}>"))
						.ToArray());
				var disposition = InspectManifestAggregate(manifestEntry, existing.Id, liveDefinition);
				if (disposition is ManifestAggregateDisposition.Customized or ManifestAggregateDisposition.Unchanged)
				{
					continue;
				}

				var reconciled = UpsertOutfitManifest(_context, manifest, _items, skins, templates);
				CompleteManifestAggregate(manifestEntry, reconciled.Id, manifestDefinition, disposition);
				continue;
			}

			UpsertOutfitManifest(_context, manifest, _items, skins, templates);
			CompleteManifestAggregate(manifestEntry, null, manifestDefinition, ManifestAggregateDisposition.Insert);
		}
	}

	private static OutfitTemplate UpsertOutfitManifest(
		FuturemudDatabaseContext context,
		OutfitManifestSpec manifest,
		IReadOnlyDictionary<string, GameItemProto> itemPrototypes,
		IReadOnlyDictionary<string, DbGameItemSkin> skins,
		ICollection<OutfitTemplate> templates)
	{
		if (manifest.Name.Length > 200)
		{
			throw new InvalidOperationException(
				$"Outfit manifest {manifest.StableKey} has a name longer than the database limit of 200 characters.");
		}

		var duplicateReferences = manifest.Items
			.Select(x => x.ItemStableReference)
			.GroupBy(x => x, StringComparer.OrdinalIgnoreCase)
			.Where(x => x.Count() > 1)
			.Select(x => x.Key)
			.ToArray();
		if (duplicateReferences.Length > 0)
		{
			throw new InvalidOperationException(
				$"Outfit manifest {manifest.StableKey} repeats item references: {string.Join(", ", duplicateReferences)}.");
		}

		var missingReferences = manifest.Items
			.Select(x => x.ItemStableReference)
			.Where(x => !itemPrototypes.ContainsKey(x))
			.ToArray();
		if (missingReferences.Length > 0)
		{
			throw new InvalidOperationException(
				$"Outfit manifest {manifest.StableKey} references missing item prototypes: {string.Join(", ", missingReferences)}.");
		}

		var missingSkins = manifest.Items
			.Where(x => x.SkinStableReference is not null && !skins.ContainsKey(x.SkinStableReference))
			.Select(x => x.SkinStableReference!)
			.ToArray();
		if (missingSkins.Length > 0)
		{
			throw new InvalidOperationException(
				$"Outfit manifest {manifest.StableKey} references missing item skins: {string.Join(", ", missingSkins)}.");
		}

		var mismatchedSkins = manifest.Items
			.Where(x => x.SkinStableReference is not null &&
			            skins[x.SkinStableReference!].ItemProtoId != itemPrototypes[x.ItemStableReference].Id)
			.Select(x => x.SkinStableReference!)
			.ToArray();
		if (mismatchedSkins.Length > 0)
		{
			throw new InvalidOperationException(
				$"Outfit manifest {manifest.StableKey} binds skins to incompatible item prototypes: {string.Join(", ", mismatchedSkins)}.");
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

		foreach (var (item, wearOrder) in manifest.Items.Select((x, index) => (x, index)))
		{
			outfitTemplate.OutfitTemplateItems.Add(new OutfitTemplateItem
			{
				TemplateKey = item.ItemStableReference,
				GameItemProtoId = itemPrototypes[item.ItemStableReference].Id,
				SkinId = item.SkinStableReference is null ? null : skins[item.SkinStableReference].Id,
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
			new OutfitManifestSpec(
				stableKey,
				name,
				description,
				itemList
					.Select(x => new OutfitManifestItemSpec(x.StableReference, null))
					.ToArray()),
			itemList.ToDictionary(x => x.StableReference, x => x.Prototype, StringComparer.OrdinalIgnoreCase),
			new Dictionary<string, DbGameItemSkin>(StringComparer.OrdinalIgnoreCase),
			templates);
	}

	internal static OutfitTemplate UpsertOutfitManifestWithSkinsForTesting(
		FuturemudDatabaseContext context,
		string stableKey,
		string name,
		string description,
		IEnumerable<(string StableReference, GameItemProto Prototype, string? SkinStableReference, DbGameItemSkin? Skin)> items)
	{
		var itemList = items.ToArray();
		var templates = context.OutfitTemplates
			.Include(x => x.OutfitTemplateItems)
			.AsEnumerable()
			.ToList();
		var skins = itemList
			.Where(x => x.SkinStableReference is not null && x.Skin is not null)
			.ToDictionary(x => x.SkinStableReference!, x => x.Skin!, StringComparer.OrdinalIgnoreCase);
		return UpsertOutfitManifest(
			context,
			new OutfitManifestSpec(
				stableKey,
				name,
				description,
				itemList
					.Select(x => new OutfitManifestItemSpec(x.StableReference, x.SkinStableReference))
					.ToArray()),
			itemList.ToDictionary(x => x.StableReference, x => x.Prototype, StringComparer.OrdinalIgnoreCase),
			skins,
			templates);
	}
}
