#nullable enable

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using Microsoft.EntityFrameworkCore;
using MudSharp.Framework.Revision;
using MudSharp.Form.Material;
using MudSharp.GameItems;
using MudSharp.Models;
using GameItemComponentProto = MudSharp.Models.GameItemComponentProto;
using GameItemSkin = MudSharp.Models.GameItemSkin;
using CultureInfo = System.Globalization.CultureInfo;

namespace DatabaseSeeder.Seeders;

public partial class ItemSeeder
{
	private readonly Dictionary<string, IReadOnlyDictionary<string, ClothingBoundColour>> _clothingColourBindings = new(StringComparer.Ordinal);
	private readonly Dictionary<string, OutfitManifestItemSpec[]> _clothingOutfitEntries = new(StringComparer.Ordinal);
	private readonly Dictionary<string, CraftDefinitionSpec> _clothingCraftPlans = new(StringComparer.Ordinal);
	private readonly Dictionary<string, HistoricalClothingItemSpec> _clothingReusePlans = new(StringComparer.Ordinal);
	private bool _clothingPreflightComplete;

	private void ValidateIndustrialisedClothingPrerequisites(string eras, ResolvedIndustrialisedTechnologyProfile profile)
	{
		_clothingPreflightComplete = false;
		var document = IndustrialisedCatalogue.Clothing;
		IndustrialisedClothingCatalogue.ValidateStructure(document);
		_clothingColourBindings.Clear();
		_clothingOutfitEntries.Clear();
		_clothingCraftPlans.Clear();
		_clothingReusePlans.Clear();
		_clothingPhysicalDefinitions.Clear();
		if (document.Bases.Count == 0)
		{
			return; // Header-only infrastructure is not evidence for Gate 2 inventory completeness.
		}

		var selected = ParseEraTokens(eras).ToHashSet(StringComparer.OrdinalIgnoreCase);
		var componentRows = _context!.GameItemComponentProtos.Include(x => x.EditableItem).AsNoTracking().ToArray();
		var wearProfiles = _context.WearProfiles.AsNoTracking().ToArray();
		var progIds = _context.FutureProgs.AsNoTracking().Select(x => x.Id).ToArray();
		var geometry = IndustrialisedClothingWearProfiles.Read(_context);
		var maximumLayerWeight = IndustrialisedClothingWearProfiles.MaximumLayerWeight(
			_context.StaticConfigurations.AsNoTracking().ToArray(), document.Bases[0].Source);
		var snapshot = IndustrialisedClothingColourBindings.Read(_context);
		var physicalRows = IndustrialisedCatalogue.Items.ToDictionary(x => x.StableReference, StringComparer.Ordinal);
		var componentsByBase = new Dictionary<string, GameItemComponentProto[]>(StringComparer.Ordinal);
		var existingBaseIds = new Dictionary<string, long?>(StringComparer.Ordinal);
		var wearablesByBase = new Dictionary<string, ClothingWearableBinding>(StringComparer.Ordinal);
		var boundProfiles = new Dictionary<long, ClothingWearProfileBinding>();
		foreach (var item in document.Bases)
		{
			if (item.EraAdmissions.Any(selected.Contains)) RequireReviewed(item.ReviewStatus, item.Source);
			var historical = FindHistoricalClothingSource(item.ItemReference);
			GameItemComponentProto[]? historicalComponents = null;
			ItemManifestDefinition? historicalDefinition = null;
			GameItemProto? historicalExisting = null;
			if (historical is not null && !physicalRows.ContainsKey(item.ItemReference))
			{
				_clothingReusePlans.Add(item.ItemReference, historical);
				var expected = BuildHistoricalClothingManifestDefinition(historical);
				historicalDefinition = expected;
				historicalExisting = ValidateHistoricalClothingReuseOwnership(historical, expected, item.Source);
				if (!_materials.TryGetValue(historical.Material, out var material) || material.Type != (int)MaterialType.Solid)
					throw item.Source.Error($"Historical reuse requires solid material {historical.Material}.");
				foreach (var tag in expected.Tags)
					if (!_tagsByFullPath.ContainsKey(tag)) throw item.Source.Error($"Historical reuse requires exact tag {tag}.");
				historicalComponents = expected.Components.Select(name =>
				{
					var matches = componentRows.Where(x => x.Name.Equals(name, StringComparison.OrdinalIgnoreCase) &&
						x.EditableItem?.RevisionStatus == (int)RevisionStatus.Current).ToArray();
					return matches.Length == 1 ? matches[0] : throw item.Source.Error($"Missing or ambiguous historical reuse component {name}.");
				}).ToArray();
			}
			GameItemComponentProto[] components;
			if (physicalRows.TryGetValue(item.ItemReference, out var physical))
			{
				if (!item.EraAdmissions.SequenceEqual(physical.EraAdmissions, StringComparer.Ordinal))
				{
					throw item.Source.Error("Clothing metadata and physical item source must have identical exact admissions.");
				}
				var expected = BuildClothingPhysicalDefinition(physical, profile);
				_clothingPhysicalDefinitions.Add(item.ItemReference, expected);
				var existingPhysical = ResolveClothingPhysicalItem(expected, item.Source);
				historicalExisting = existingPhysical;
				components = expected.Components.Select(name =>
				{
					var matches = componentRows.Where(x => x.Name.Equals(name, StringComparison.OrdinalIgnoreCase) &&
						x.EditableItem?.RevisionStatus == (int)RevisionStatus.Current).ToArray();
					return matches.Length == 1 ? matches[0] : throw item.Source.Error($"Missing or ambiguous current component {name}.");
				}).ToArray();
				if (existingPhysical is not null)
				{
					if (!_materials.Values.Any(x => x.Id == existingPhysical.MaterialId && x.Type == (int)MaterialType.Solid))
						throw item.Source.Error("Preserved clothing must have a resolvable solid material.");
					var attached = ResolveReusedClothingComponents(existingPhysical, componentRows, item.Source);
					components = item.EraAdmissions.Any(selected.Contains) && !IsRenamedClothingPhysicalItem(existingPhysical, item.ItemReference)
						? ProjectHistoricalClothingComponentUpdate(existingPhysical, expected, attached, components)
						: attached;
				}
			}
			else if ((historicalExisting ?? _itemsByStableReference.GetValueOrDefault(item.ItemReference)) is { } existing)
			{
				if (!_materials.Values.Any(x => x.Id == existing.MaterialId && x.Type == (int)MaterialType.Solid))
				{
					throw item.Source.Error("Reused clothing must have a resolvable solid material.");
				}
				components = ResolveReusedClothingComponents(existing, componentRows, item.Source);
				if (historicalComponents is not null && historicalDefinition is not null && item.EraAdmissions.Any(selected.Contains))
				{
					components = ProjectHistoricalClothingComponentUpdate(existing, historicalDefinition, components, historicalComponents);
				}
			}
			else if (historicalComponents is not null)
			{
				components = historicalComponents;
			}
			else
			{
				throw item.Source.Error($"No physical source or installed reusable base resolves {item.ItemReference}; supply its authoritative admission dependency before installation.");
			}

			existingBaseIds.Add(item.ItemReference, (historicalExisting ?? _itemsByStableReference.GetValueOrDefault(item.ItemReference))?.Id);
			var wearable = IndustrialisedClothingPhysicalBindings.Bind(components, wearProfiles, progIds, item.Source);
			if (wearable.LayerWeight > maximumLayerWeight)
				throw item.Source.Error("Standalone garment exceeds MaximumLayerWeight even before other clothing is worn.");
			wearablesByBase.Add(item.ItemReference, wearable);
			foreach (var id in wearable.ProfileIds)
				if (!boundProfiles.ContainsKey(id)) boundProfiles.Add(id, geometry.Bind(wearProfiles.Single(x => x.Id == id), item.Source));
			componentsByBase.Add(item.ItemReference, components);
			_clothingColourBindings.Add(item.ItemReference, snapshot.Bind(
				IndustrialisedClothingColourPlan.Channels(document, item.ItemReference, ""), components, item.Source));
		}

		foreach (var skin in document.Skins)
		{
			if (skin.EraAdmissions.Any(selected.Contains)) RequireReviewed(skin.ReviewStatus, skin.Source);
			_clothingColourBindings.Add(skin.StableReference, snapshot.Bind(
				IndustrialisedClothingColourPlan.Channels(document, skin.BaseItemReference, skin.StableReference),
				componentsByBase[skin.BaseItemReference], skin.Source, requireStandaloneProfile: false));
		}
		if (document.Skins.Any(x => x.EraAdmissions.Any(selected.Contains)) &&
			_context.FutureProgs.Count(x => x.FunctionName == "AlwaysTrue") != 1)
		{
			throw document.Skins.First().Source.Error("Stock skins require exactly one existing AlwaysTrue FutureProg.");
		}

		foreach (var outfit in document.Outfits)
		{
			if (outfit.EraAdmissions.Any(selected.Contains)) RequireReviewed(outfit.ReviewStatus, outfit.Source);
			_clothingOutfitEntries.Add(outfit.StableReference, document.OutfitEntries
				.Where(x => x.OutfitReference == outfit.StableReference).OrderBy(x => x.Order)
				.Select(entry => BindClothingOutfitEntry(document, entry, componentsByBase[entry.ItemReference],
					_clothingColourBindings[entry.SkinReference.Length > 0 ? entry.SkinReference : entry.ItemReference], wearProfiles)).ToArray());
			IndustrialisedClothingWearProfiles.ValidateMandatoryLayers(document.OutfitEntries
				.Where(x => x.OutfitReference == outfit.StableReference && x.Placement == OutfitTemplateItemPlacement.Worn)
				.OrderBy(x => x.Order)
				.Select(entry => new ClothingWornEntryBinding(entry.Source, entry.EntryKey, wearablesByBase[entry.ItemReference],
					boundProfiles[wearProfiles.Single(x => x.Name == entry.WearProfile).Id])), maximumLayerWeight);
		}

		ValidateIndustrialisedClothingSkinOwnership(document, selected, existingBaseIds);
		ValidateIndustrialisedClothingOutfitOwnership(document, selected, componentsByBase, existingBaseIds,
			boundProfiles, geometry, snapshot, maximumLayerWeight, componentRows, wearProfiles);
		ValidateIndustrialisedClothingCrafts(document, selected, snapshot);
		ValidateIndustrialisedClothingCraftOwnership(document, selected);
		_clothingPreflightComplete = true;
	}

	/// <summary>
	/// Canonical capture has no installed component, characteristic or wear-profile registry. Build only the
	/// source-derived plans needed to register stable manifest aggregates; live installation still uses the
	/// complete database-backed prerequisite validation above.
	/// </summary>
	private void PrepareIndustrialisedClothingManifestCapture(string eras, ResolvedIndustrialisedTechnologyProfile profile)
	{
		_clothingPreflightComplete = false;
		var document = IndustrialisedCatalogue.Clothing;
		IndustrialisedClothingCatalogue.ValidateStructure(document);
		_clothingColourBindings.Clear();
		_clothingOutfitEntries.Clear();
		_clothingCraftPlans.Clear();
		_clothingReusePlans.Clear();
		_clothingPhysicalDefinitions.Clear();
		if (document.Bases.Count == 0)
		{
			return;
		}

		var selected = ParseEraTokens(eras).ToHashSet(StringComparer.OrdinalIgnoreCase);
		var physicalRows = IndustrialisedCatalogue.Items.ToDictionary(x => x.StableReference, StringComparer.Ordinal);
		foreach (var item in document.Bases)
		{
			if (item.EraAdmissions.Any(selected.Contains)) RequireReviewed(item.ReviewStatus, item.Source);
			if (physicalRows.TryGetValue(item.ItemReference, out var physical))
			{
				if (!item.EraAdmissions.SequenceEqual(physical.EraAdmissions, StringComparer.Ordinal))
				{
					throw item.Source.Error("Clothing metadata and physical item source must have identical exact admissions.");
				}
				_clothingPhysicalDefinitions.Add(item.ItemReference, BuildClothingPhysicalDefinition(physical, profile));
				continue;
			}

			var historical = FindHistoricalClothingSource(item.ItemReference);
			if (historical is null)
			{
				throw item.Source.Error($"No physical source or historical reuse dependency resolves {item.ItemReference} for canonical capture.");
			}
			_clothingReusePlans.Add(item.ItemReference, historical);
		}

		foreach (var skin in document.Skins)
		{
			if (skin.EraAdmissions.Any(selected.Contains)) RequireReviewed(skin.ReviewStatus, skin.Source);
		}
		foreach (var outfit in document.Outfits)
		{
			if (outfit.EraAdmissions.Any(selected.Contains)) RequireReviewed(outfit.ReviewStatus, outfit.Source);
			_clothingOutfitEntries.Add(outfit.StableReference, document.OutfitEntries
				.Where(x => x.OutfitReference == outfit.StableReference)
				.OrderBy(x => x.Order)
				.Select(entry => BindClothingOutfitEntryForManifestCapture(document, entry))
				.ToArray());
		}

		var craftNames = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
		foreach (var craft in document.Crafts)
		{
			if (craft.EraAdmissions.Any(selected.Contains)) RequireReviewed(craft.ReviewStatus, craft.Source);
			if (!craftNames.Add(CraftLookupKey(craft.Name, craft.Category)))
			{
				throw craft.Source.Error("Two clothing crafts have the same name/category.");
			}
			_clothingCraftPlans.Add(craft.StableReference,
				IndustrialisedClothingCraftPlan.Compile(document, craft) with { Trait = LookupTraitDefinition(craft.Trait) });
		}
		_clothingPreflightComplete = true;
	}

	internal static GameItemComponentProto[] ResolveReusedClothingComponents(GameItemProto item,
		IReadOnlyCollection<GameItemComponentProto> available, ClothingSourceLocation source)
	{
		var byRevision = available.ToLookup(x => (x.Id, x.RevisionNumber));
		var seen = new HashSet<long>();
		return item.GameItemProtosGameItemComponentProtos
			.OrderBy(x => x.GameItemComponentProtoId).ThenBy(x => x.GameItemComponentRevision)
			.Select(link =>
			{
				if (link.GameItemProtoId != item.Id || link.GameItemProtoRevision != item.RevisionNumber)
					throw source.Error($"Component link does not belong to reused item {item.UniqueName} revision {item.RevisionNumber}.");
				if (!seen.Add(link.GameItemComponentProtoId))
					throw source.Error($"Reused item {item.UniqueName} links multiple instances or revisions of component {link.GameItemComponentProtoId}.");
				var matches = byRevision[(link.GameItemComponentProtoId, link.GameItemComponentRevision)].ToArray();
				if (matches.Length != 1)
					throw source.Error($"Reused item {item.UniqueName} has a missing or ambiguous component {link.GameItemComponentProtoId} revision {link.GameItemComponentRevision}.");
				// An existing item retains its attached revision, even when a newer same-name revision is current.
				return matches[0];
			}).ToArray();
	}

	private GameItemComponentProto[] ProjectHistoricalClothingComponentUpdate(GameItemProto existing,
		ItemManifestDefinition expected, GameItemComponentProto[] attached, GameItemComponentProto[] proposed)
	{
		var managed = FindManagedRecord("item", expected.StableReference);
		var live = BuildLiveItemManifestDefinition(existing, expected.StableReference);
		var untouched = managed is not null && ItemSeederManifestCatalogue.Fingerprint(live)
			.Equals(managed.AppliedFingerprint, StringComparison.OrdinalIgnoreCase);
		if (managed is null || (!untouched && !IsRepairableMissingItemStock(live, expected)))
		{
			return attached; // Unmanaged/existing customised garments are not silently repaired by the writer.
		}

		// Mirror the writer: an exact untouched aggregate can replace obsolete stock links; a
		// missing-link repair is additive. Both retain the attached revision of each continuing ID.
		// This is a projection only; ownership and item/provenance state remain untouched until apply.
		var desired = proposed.Select(x => x.Id).ToHashSet();
		var continuing = attached.Where(x => !untouched || desired.Contains(x.Id)).ToArray();
		var retained = continuing.Select(x => x.Id).ToHashSet();
		return continuing.Concat(proposed.Where(x => retained.Add(x.Id))).ToArray();
	}

	private static void RequireReviewed(ClothingReviewStatus status, ClothingSourceLocation source)
	{
		if (status != ClothingReviewStatus.Reviewed)
		{
			throw source.Error("Admitted clothing content must be editorially Reviewed before installation; draft briefs are not stock.");
		}
	}

	internal static OutfitManifestItemSpec BindClothingOutfitEntry(IndustrialisedClothingCatalogueDocument document,
		ClothingOutfitEntryRow entry, IReadOnlyCollection<GameItemComponentProto> components,
		IReadOnlyDictionary<string, ClothingBoundColour> colours, IReadOnlyCollection<WearProfile> wearProfiles)
	{
		if (entry.Placement == OutfitTemplateItemPlacement.Worn)
		{
			var profiles = wearProfiles.Where(x => x.Name.Equals(entry.WearProfile, StringComparison.OrdinalIgnoreCase)).ToArray();
			var wearables = components.Where(x => x.Type == "Wearable").ToArray();
			if (profiles.Length != 1 || profiles[0].Name != entry.WearProfile || wearables.Length != 1)
			{
				throw entry.Source.Error($"Worn entry requires exactly one Wearable component and exact wear profile {entry.WearProfile}.");
			}
			try
			{
				var profileIds = XElement.Parse(wearables[0].Definition).Element("Profiles")?.Elements("Profile")
					.Select(x => long.Parse(x.Value, CultureInfo.InvariantCulture)).ToArray() ?? [];
				if (!profileIds.Contains(profiles[0].Id))
				{
					throw entry.Source.Error($"Wear profile {entry.WearProfile} is not supported by this garment's component.");
				}
			}
			catch (Exception ex) when (ex is XmlException or FormatException or OverflowException)
			{
				throw entry.Source.Error($"Invalid Wearable profile XML: {ex.Message}");
			}
		}
		else if (entry.WearProfile.Length > 0)
		{
			throw entry.Source.Error("Only a worn outfit entry may specify a wear profile.");
		}

		return new(entry.ItemReference, entry.SkinReference.Length == 0 ? null : entry.SkinReference)
		{
			EntryKey = entry.EntryKey,
			WearProfile = entry.WearProfile.Length == 0 ? null : entry.WearProfile,
			Placement = entry.Placement,
			ContainerKey = entry.ContainerKey.Length == 0 ? null : entry.ContainerKey,
			LoadArguments = IndustrialisedClothingColourBindings.LoadArguments(colours,
				IndustrialisedClothingColourPlan.OutfitValues(document, entry), entry.Source)
		};
	}

	internal static OutfitManifestItemSpec BindClothingOutfitEntryForManifestCapture(
		IndustrialisedClothingCatalogueDocument document, ClothingOutfitEntryRow entry)
	{
		if (entry.Placement == OutfitTemplateItemPlacement.Worn && entry.WearProfile.Length == 0)
		{
			throw entry.Source.Error("Worn entry requires an explicit wear profile.");
		}
		if (entry.Placement != OutfitTemplateItemPlacement.Worn && entry.WearProfile.Length > 0)
		{
			throw entry.Source.Error("Only a worn outfit entry may specify a wear profile.");
		}

		var presentation = entry.SkinReference.Length == 0 ? entry.ItemReference : entry.SkinReference;
		var channels = IndustrialisedClothingColourPlan.Channels(document, entry.ItemReference, entry.SkinReference);
		var choices = IndustrialisedClothingColourPlan.OutfitValues(document, entry);
		var loadArguments = string.Join(" ", choices.OrderBy(x => x.Key, StringComparer.Ordinal).Select(x =>
			$"{x.Key}=[{channels[x.Key].Definition}/{x.Value}]"));
		return new(entry.ItemReference, entry.SkinReference.Length == 0 ? null : presentation)
		{
			EntryKey = entry.EntryKey,
			WearProfile = entry.WearProfile.Length == 0 ? null : entry.WearProfile,
			Placement = entry.Placement,
			ContainerKey = entry.ContainerKey.Length == 0 ? null : entry.ContainerKey,
			LoadArguments = loadArguments
		};
	}

	private void SeedIndustrialisedClothingPresentations(string eras)
	{
		var document = IndustrialisedCatalogue.Clothing;
		if (document.Bases.Count == 0) return;
		if (!_clothingPreflightComplete) throw new InvalidOperationException("Clothing presentations cannot be applied before complete successful preflight.");
		var selected = ParseEraTokens(eras).ToHashSet(StringComparer.OrdinalIgnoreCase);
		var skins = new Dictionary<string, GameItemSkin>(StringComparer.OrdinalIgnoreCase);
		foreach (var skin in document.Skins.Where(x => _manifestCaptureOnly || x.EraAdmissions.Any(selected.Contains)))
		{
			using var module = UseManifestModule("outfits", skin.EraAdmissions.ToArray());
			var resolved = SeedDocumentedClothingSkins(new DocumentedClothingSkinSpec[]
			{
				new(skin.StableReference, skin.BaseItemReference, skin.Noun, skin.ShortDescription, skin.FullDescription, skin.QualityOverride)
			});
			foreach (var pair in resolved) skins.Add(pair.Key, pair.Value);
		}
		foreach (var outfit in document.Outfits.Where(x => _manifestCaptureOnly || x.EraAdmissions.Any(selected.Contains)))
		{
			if (!_clothingOutfitEntries.TryGetValue(outfit.StableReference, out var entries))
			{
				throw outfit.Source.Error("Clothing outfit must have a resolved preflight graph before persistence or canonical capture.");
			}
			using var module = UseManifestModule("outfits", outfit.EraAdmissions.ToArray());
			UpsertOutfitManifests([new(outfit.StableReference, outfit.Name, outfit.Description, entries)], skins);
		}
	}
}
