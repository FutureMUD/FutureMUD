#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using System.Globalization;
using System.Text.RegularExpressions;
using Microsoft.EntityFrameworkCore;
using MudSharp.Database;
using MudSharp.Framework.Revision;
using MudSharp.GameItems;
using MudSharp.Models;
using DbGameItemSkin = MudSharp.Models.GameItemSkin;
using CultureInfo = System.Globalization.CultureInfo;

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
		ItemQuality? Quality);

	internal sealed record OutfitManifestItemSpec(
		string ItemStableReference,
		string? SkinStableReference)
	{
		public string? EntryKey { get; init; }
		public string? WearProfile { get; init; }
		public OutfitTemplateItemPlacement Placement { get; init; } = OutfitTemplateItemPlacement.Worn;
		public string? ContainerKey { get; init; }
		public string LoadArguments { get; init; } = string.Empty;
		public string EffectiveKey => EntryKey ?? ItemStableReference;

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
		string? SkinStableReference,
		string LoadArguments);

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
		ItemQuality? Quality);

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
						item.SkinStableReference,
						item.LoadArguments))
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
		return SeedDocumentedClothingSkins(specs);
	}

	private IReadOnlyDictionary<string, DbGameItemSkin> SeedDocumentedClothingSkins(
		IReadOnlyCollection<DocumentedClothingSkinSpec> specs)
	{
		if (_context is null)
		{
			throw new InvalidOperationException("The item seeder context must be initialised before clothing skins are seeded.");
		}
		var alwaysTrue = _context.FutureProgs.SingleOrDefault(x => x.FunctionName == "AlwaysTrue");
		if (alwaysTrue is null && !_manifestCaptureOnly)
		{
			throw new InvalidOperationException("Documented clothing skins require the existing AlwaysTrue FutureProg.");
		}
		var alwaysTrueName = alwaysTrue?.FunctionName ?? "AlwaysTrue";
		var skinRows = _context.GameItemSkins
			.Include(x => x.EditableItem)
			.AsEnumerable()
			.Concat(_context.GameItemSkins.Local)
			.DistinctBy(x => (x.Id, x.RevisionNumber))
			.ToArray();
		var targets = new Dictionary<string, DbGameItemSkin?>(StringComparer.OrdinalIgnoreCase);
		// Check the whole batch before registering/adopting or changing even its first skin.
		foreach (var spec in specs)
		{
			if (!_itemsByStableReference.TryGetValue(spec.BaseItemStableReference, out var baseItem))
				throw new InvalidOperationException($"Documented clothing skin {spec.StableReference} references missing item prototype {spec.BaseItemStableReference}.");
			if (!targets.TryAdd(spec.StableReference, ValidateDocumentedClothingSkinOwnership(spec, baseItem.Id, skinRows, alwaysTrueName)))
				throw new InvalidOperationException($"A clothing skin batch repeats stable reference {spec.StableReference}.");
		}
		var nextSkinId = skinRows.Length > 0
			? skinRows.Max(x => x.Id) + 1
			: 1;
		var resolved = new Dictionary<string, DbGameItemSkin>(StringComparer.OrdinalIgnoreCase);

		foreach (var spec in specs)
		{
			if (!_itemsByStableReference.TryGetValue(spec.BaseItemStableReference, out var baseItem))
			{
				throw new InvalidOperationException(
					$"Documented clothing skin {spec.StableReference} references missing item prototype {spec.BaseItemStableReference}.");
			}

			var expectedDefinition = DocumentedClothingSkinDefinition(spec, alwaysTrueName);
			var manifestEntry = RegisterManifestAggregate(
				"item-skin",
				spec.StableReference,
				expectedDefinition,
				[$"item:{spec.BaseItemStableReference}"]);
			if (_manifestCaptureOnly)
			{
				continue;
			}

			if (targets[spec.StableReference] is { } existing)
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
				Quality = (int?)spec.Quality,
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
		skin.Quality = (int?)spec.Quality;
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
			.Concat(_context.OutfitTemplates.Local)
			.Distinct()
			.ToList();
		var wearProfiles = _context.WearProfiles.AsNoTracking().ToArray();
		var values = _context.CharacteristicValues.AsNoTracking().ToArray();
		var definitions = _context.CharacteristicDefinitions.AsNoTracking().ToArray();
		var manifestList = manifests.ToArray();
		var targets = new Dictionary<string, OutfitTemplate?>(StringComparer.OrdinalIgnoreCase);
		var skinReferences = _manifestCaptureOnly ? new Dictionary<long, string>() : OutfitSkinReferences(skins);
		if (manifestList.GroupBy(x => x.StableKey, StringComparer.OrdinalIgnoreCase).Any(x => x.Count() > 1) ||
			manifestList.GroupBy(x => x.Name, StringComparer.OrdinalIgnoreCase).Any(x => x.Count() > 1))
		{
			throw new InvalidOperationException("An outfit batch cannot repeat a stable key or template name.");
		}
		// Validate the whole requested batch before updating even its first template.
		foreach (var manifest in manifestList)
		{
			ValidateOutfitEntries(manifest, wearProfiles, !_manifestCaptureOnly);
			if (!_manifestCaptureOnly)
			{
				var existing = ValidateOutfitManifestTarget(manifest, _items, skins, templates,
					FindManagedRecord("outfit", manifest.StableKey)?.LogicalId);
				ValidateOutfitManifestOwnership(manifest, existing, BuildOutfitManifestDefinition(manifest, values, definitions),
					existing is null ? null : BuildLiveOutfitManifestDefinition(manifest.StableKey, existing, skinReferences, wearProfiles, values, definitions));
				targets.Add(manifest.StableKey, existing);
			}
		}
		foreach (var manifest in manifestList)
		{
			var manifestDefinition = BuildOutfitManifestDefinition(manifest, values, definitions);
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

			var existing = targets[manifest.StableKey];
			if (existing is not null)
			{
				var liveDefinition = BuildLiveOutfitManifestDefinition(manifest.StableKey, existing, skinReferences, wearProfiles, values, definitions);
				UpgradeLegacyOutfitFingerprint(manifest.StableKey, existing, liveDefinition);
				var disposition = InspectManifestAggregate(manifestEntry, existing.Id, liveDefinition);
				if (disposition is ManifestAggregateDisposition.Customized or ManifestAggregateDisposition.Unchanged)
				{
					continue;
				}

				var reconciled = UpsertOutfitManifest(_context, manifest, _items, skins, templates);
				CompleteManifestAggregate(manifestEntry, reconciled.Id, manifestDefinition, disposition);
				continue;
			}

			var created = UpsertOutfitManifest(_context, manifest, _items, skins, templates);
			CompleteGeneratedManifestAggregate(manifestEntry, created, manifestDefinition);
		}
	}

	private static OutfitTemplate UpsertOutfitManifest(
		FuturemudDatabaseContext context,
		OutfitManifestSpec manifest,
		IReadOnlyDictionary<string, GameItemProto> itemPrototypes,
		IReadOnlyDictionary<string, DbGameItemSkin> skins,
		ICollection<OutfitTemplate> templates)
	{
		var wearProfiles = context.WearProfiles.AsNoTracking().ToArray();
		ValidateOutfitEntries(manifest, wearProfiles);
		var outfitTemplate = ValidateOutfitManifestTarget(manifest, itemPrototypes, skins, templates);
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
		outfitTemplate.Description = $"{manifest.Description}{Environment.NewLine}{GetOutfitManifestMarker(manifest.StableKey)}";
		outfitTemplate.Exclusivity = (int)OutfitExclusivity.NonExclusive;
		foreach (var (item, wearOrder) in manifest.Items.Select((x, index) => (x, index)))
		{
			outfitTemplate.OutfitTemplateItems.Add(new OutfitTemplateItem
			{
				TemplateKey = item.EffectiveKey,
				GameItemProtoId = itemPrototypes[item.ItemStableReference].Id,
				SkinId = item.SkinStableReference is null ? null : skins[item.SkinStableReference].Id,
				WearProfileId = item.WearProfile is null ? null : wearProfiles.Single(x => x.Name == item.WearProfile).Id,
				Placement = (int)item.Placement,
				ContainerKey = item.ContainerKey,
				LoadArguments = item.LoadArguments,
				WearOrder = wearOrder
			});
		}

		return outfitTemplate;
	}

	private static OutfitTemplate? ValidateOutfitManifestTarget(OutfitManifestSpec manifest,
		IReadOnlyDictionary<string, GameItemProto> itemPrototypes,
		IReadOnlyDictionary<string, DbGameItemSkin> skins, ICollection<OutfitTemplate> templates, long? ownedLogicalId = null)
	{
		if (manifest.Name.Length > 200)
		{
			throw new InvalidOperationException(
				$"Outfit manifest {manifest.StableKey} has a name longer than the database limit of 200 characters.");
		}

		var duplicateReferences = manifest.Items
			.Select(x => x.EffectiveKey)
			.GroupBy(x => x, StringComparer.OrdinalIgnoreCase)
			.Where(x => x.Count() > 1)
			.Select(x => x.Key)
			.ToArray();
		if (duplicateReferences.Length > 0)
		{
			throw new InvalidOperationException(
				$"Outfit manifest {manifest.StableKey} repeats entry keys: {string.Join(", ", duplicateReferences)}.");
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

		return ResolveOutfitManifestIdentity(manifest, templates, ownedLogicalId);
	}

	private static void ValidateOutfitEntries(OutfitManifestSpec manifest, IReadOnlyCollection<WearProfile> wearProfiles,
		bool requireWearProfileResolution = true)
	{
		var earlierKeys = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
		foreach (var item in manifest.Items)
		{
			if (string.IsNullOrWhiteSpace(item.EffectiveKey) || item.EffectiveKey.Length > 100 || !Enum.IsDefined(item.Placement))
			{
				throw new InvalidOperationException($"Outfit {manifest.StableKey} has an invalid entry key or placement.");
			}
			if (item.WearProfile is not null && (item.Placement != OutfitTemplateItemPlacement.Worn ||
				requireWearProfileResolution &&
				(wearProfiles.Count(x => x.Name.Equals(item.WearProfile, StringComparison.OrdinalIgnoreCase)) != 1 ||
				 !wearProfiles.Any(x => x.Name == item.WearProfile))))
			{
				throw new InvalidOperationException($"Outfit {manifest.StableKey}/{item.EffectiveKey} has a missing, ambiguous or incompatible wear profile {item.WearProfile}.");
			}
			var requiresContainer = item.Placement is OutfitTemplateItemPlacement.Container or OutfitTemplateItemPlacement.AttachedToBelt or OutfitTemplateItemPlacement.Sheathed;
			if (requiresContainer != (item.ContainerKey is not null) ||
				(item.ContainerKey is not null && !earlierKeys.Contains(item.ContainerKey)))
			{
				throw new InvalidOperationException($"Outfit {manifest.StableKey}/{item.EffectiveKey} requires a compatible earlier related entry only for container, belt or sheath placement.");
			}
			if (!earlierKeys.Add(item.EffectiveKey))
			{
				throw new InvalidOperationException($"Outfit {manifest.StableKey} repeats entry keys: {item.EffectiveKey}.");
			}
		}
	}

	private static string NormalizeOutfitLoadArguments(string arguments, IReadOnlyCollection<CharacteristicValue> values,
		IReadOnlyCollection<CharacteristicDefinition> definitions) => Regex.Replace(arguments,
		@"(?<variable>\w+)(?<separator>=|:)(?<value>[0-9]+)(?!\w)", match =>
		{
			if (!long.TryParse(match.Groups["value"].Value, NumberStyles.None, CultureInfo.InvariantCulture, out var id))
			{
				return match.Value;
			}
			var value = values.SingleOrDefault(x => x.Id == id);
			var definition = value is null ? null : definitions.SingleOrDefault(x => x.Id == value.DefinitionId);
			return value is null || definition is null ? match.Value :
				$"{match.Groups["variable"].Value}{match.Groups["separator"].Value}[{definition.Name}/{value.Name}]";
		});

	private void UpgradeLegacyOutfitFingerprint(string stableKey, OutfitTemplate existing, OutfitManifestDefinition live)
	{
		var managed = FindManagedRecord("outfit", stableKey);
		if (managed is null || (managed.LogicalId is not null && managed.LogicalId != existing.Id) || live.Items.Where((x, order) =>
			x.EntryKey != x.ItemStableReference || x.WearOrder != order || x.WearProfile is not null ||
			x.Placement != (int)OutfitTemplateItemPlacement.Worn || x.ContainerKey is not null || x.LoadArguments.Length > 0).Any())
		{
			return;
		}

		// Version 2 did not fingerprint these fields. Only its actual default composition can be
		// upgraded; changed colours, item IDs, placement or order must remain builder-owned.
		var legacy = new
		{
			live.StableKey, live.Name, live.Description, live.Exclusivity,
			Items = live.Items.Select(x => new { x.ItemStableReference, x.SkinStableReference }).ToArray()
		};
		if (ItemSeederManifestCatalogue.Fingerprint(legacy) == managed.AppliedFingerprint)
		{
			managed.AppliedFingerprint = ItemSeederManifestCatalogue.Fingerprint(live);
		}
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

	internal static void ReconcileOutfitForTesting(FuturemudDatabaseContext context, string stableKey,
		string description, OutfitManifestItemSpec[] entries, IReadOnlyDictionary<string, GameItemProto> items,
		IReadOnlyDictionary<string, DbGameItemSkin>? skins = null)
	{
		var seeder = new ItemSeeder
		{
			_context = context,
			_items = new Dictionary<string, GameItemProto>(items, StringComparer.OrdinalIgnoreCase),
			_itemStableReferencesById = items.ToDictionary(x => x.Value.Id, x => x.Key),
			_managedRecordsByIdentity = context.SeederManagedRecords.Where(x => x.Seeder == "Items")
				.AsEnumerable().ToDictionary(x => ManagedRecordIdentity(x.EntityType, x.StableKey), StringComparer.OrdinalIgnoreCase)
		};
		using var module = seeder.UseManifestModule("outfits", "industrial");
		seeder.UpsertOutfitManifests([new(stableKey, "Test ensemble", description, entries)],
			skins ?? new Dictionary<string, DbGameItemSkin>());
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
