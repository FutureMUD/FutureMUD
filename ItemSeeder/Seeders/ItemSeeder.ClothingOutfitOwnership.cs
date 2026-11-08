#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using MudSharp.Framework.Revision;
using MudSharp.GameItems;
using MudSharp.Models;

namespace DatabaseSeeder.Seeders;

public partial class ItemSeeder
{
	private static OutfitManifestDefinition BuildOutfitManifestDefinition(OutfitManifestSpec manifest,
		IReadOnlyCollection<CharacteristicValue> values, IReadOnlyCollection<CharacteristicDefinition> definitions) => new(
		manifest.StableKey, manifest.Name, $"{manifest.Description}{Environment.NewLine}{GetOutfitManifestMarker(manifest.StableKey)}",
		(int)OutfitExclusivity.NonExclusive, manifest.Items.Select((item, order) => new OutfitManifestItemDefinition(
			item.ItemStableReference, item.SkinStableReference, item.EffectiveKey, item.WearProfile,
			(int)item.Placement, item.ContainerKey, NormalizeOutfitLoadArguments(item.LoadArguments, values, definitions), order)).ToArray());

	private OutfitManifestDefinition BuildLiveOutfitManifestDefinition(string stableKey, OutfitTemplate outfit,
		IReadOnlyDictionary<long, string> skinReferences, IReadOnlyCollection<WearProfile> profiles,
		IReadOnlyCollection<CharacteristicValue> values, IReadOnlyCollection<CharacteristicDefinition> definitions) => new(
		stableKey, outfit.Name, outfit.Description, outfit.Exclusivity,
		outfit.OutfitTemplateItems.OrderBy(x => x.WearOrder).ThenBy(x => x.Id).Select(item => new OutfitManifestItemDefinition(
			ResolveItemStableReference(item.GameItemProtoId) ?? $"<missing:{item.GameItemProtoId}>",
			item.SkinId is null ? null : skinReferences.GetValueOrDefault(item.SkinId.Value) ?? $"<missing:{item.SkinId}>",
			item.TemplateKey, item.WearProfileId is null ? null : profiles.SingleOrDefault(x => x.Id == item.WearProfileId)?.Name ?? $"<missing:{item.WearProfileId}>",
			item.Placement, item.ContainerKey, NormalizeOutfitLoadArguments(item.LoadArguments, values, definitions), item.WearOrder)).ToArray());

	private Dictionary<long, string> OutfitSkinReferences(IReadOnlyDictionary<string, GameItemSkin>? supplied = null)
	{
		var rows = _context!.GameItemSkins.Include(x => x.EditableItem).AsNoTracking().ToArray();
		var result = rows.GroupBy(x => x.Id).ToDictionary(x => x.Key, x => x
			.OrderByDescending(y => y.EditableItem?.RevisionStatus == (int)RevisionStatus.Current)
			.ThenByDescending(y => y.RevisionNumber).First().Name);
		if (supplied is not null)
			foreach (var pair in supplied) result[pair.Value.Id] = pair.Key;
		foreach (var group in _managedRecordsByIdentity.Values.Where(x => x.EntityType.Equals("item-skin", StringComparison.OrdinalIgnoreCase) &&
			x.LogicalId.HasValue).GroupBy(x => x.LogicalId!.Value))
		{
			if (!result.ContainsKey(group.Key)) continue;
			var keys = group.Select(x => x.StableKey).Distinct(StringComparer.OrdinalIgnoreCase).ToArray();
			result[group.Key] = keys.Length == 1 ? keys[0] : $"<ambiguous:{group.Key}>";
		}
		return result;
	}

	private static OutfitTemplate? ResolveOutfitManifestIdentity(OutfitManifestSpec manifest,
		ICollection<OutfitTemplate> templates, long? ownedLogicalId = null)
	{
		if (manifest.Name.Length > 200)
			throw new InvalidOperationException($"Outfit manifest {manifest.StableKey} has a name longer than the database limit of 200 characters.");
		var marker = GetOutfitManifestMarker(manifest.StableKey);
		var marked = templates.Where(x => HasOutfitManifestMarker(x.Description, marker)).ToArray();
		if (marked.Length > 1)
			throw new InvalidOperationException($"Multiple outfit templates claim stock manifest key {manifest.StableKey}.");
		if (ownedLogicalId.HasValue && marked.Length == 1 && marked[0].Id != ownedLogicalId)
			throw new InvalidOperationException($"ItemSeeder ownership conflict for outfit:{manifest.StableKey}: provenance and marker resolve different IDs.");
		var target = ownedLogicalId.HasValue ? templates.SingleOrDefault(x => x.Id == ownedLogicalId) : null;
		target ??= marked.SingleOrDefault();
		var names = templates.Where(x => x.Name.Equals(manifest.Name, StringComparison.OrdinalIgnoreCase)).ToArray();
		if (names.Any(x => target is null || !ReferenceEquals(x, target)))
			throw new InvalidOperationException($"Cannot seed stock outfit manifest {manifest.StableKey} because {(target is null ? "a builder-authored template" : "another template")} already uses the name {manifest.Name}.");
		if (target is not null && target.Description.Split(["\r\n", "\n"], StringSplitOptions.RemoveEmptyEntries)
			.Any(x => x.StartsWith(OutfitManifestMarkerPrefix, StringComparison.Ordinal) && x != marker))
			throw new InvalidOperationException($"ItemSeeder ownership conflict for outfit:{manifest.StableKey}: its target claims another stock key.");
		return target;
	}

	private void ValidateOutfitManifestOwnership(OutfitManifestSpec manifest, OutfitTemplate? existing,
		OutfitManifestDefinition expected, OutfitManifestDefinition? live)
	{
		if (_manifestCaptureOnly || existing is null) return;
		var managed = FindManagedRecord("outfit", manifest.StableKey);
		if (managed?.LogicalId is { } id && id != existing.Id)
			throw new InvalidOperationException($"ItemSeeder ownership conflict for outfit:{manifest.StableKey}: provenance names ID {id}, but the template resolves to {existing.Id}.");
		var others = _managedRecordsByIdentity.Values.Where(x => x.EntityType.Equals("outfit", StringComparison.OrdinalIgnoreCase) &&
			x.LogicalId == existing.Id && !x.StableKey.Equals(manifest.StableKey, StringComparison.OrdinalIgnoreCase)).ToArray();
		if (others.Length > 0)
			throw new InvalidOperationException($"ItemSeeder ownership conflict for outfit:{manifest.StableKey}: ID {existing.Id} is claimed by {string.Join(", ", others.Select(x => x.StableKey).OrderBy(x => x, StringComparer.Ordinal))}.");
		if (managed is null && !ItemSeederManifestCatalogue.Fingerprint(live!).Equals(
			ItemSeederManifestCatalogue.Fingerprint(expected), StringComparison.OrdinalIgnoreCase))
			throw new InvalidOperationException($"Unmanaged outfit conflict for '{manifest.StableKey}'. The complete stock signature does not match; it will not be claimed or overwritten.");
	}

	private void ValidateIndustrialisedClothingOutfitOwnership(IndustrialisedClothingCatalogueDocument document, IReadOnlySet<string> selected,
		IReadOnlyDictionary<string, GameItemComponentProto[]> plannedComponents,
		IReadOnlyDictionary<string, long?> plannedItemIds,
		IReadOnlyDictionary<long, ClothingWearProfileBinding> boundProfiles,
		IndustrialisedClothingWearProfiles geometry, IndustrialisedClothingColourBindings colours,
		double maximumLayerWeight, IReadOnlyCollection<GameItemComponentProto> componentRows,
		IReadOnlyCollection<WearProfile> wearProfiles)
	{
		if (_manifestCaptureOnly) return;
		var templates = _context!.OutfitTemplates.Include(x => x.OutfitTemplateItems).AsNoTracking().ToArray();
		var itemRows = _context.GameItemProtos
			.Include(x => x.EditableItem)
			.Include(x => x.GameItemProtosGameItemComponentProtos)
			.AsNoTracking().ToArray();
		var progs = _context.FutureProgs.AsNoTracking().ToArray();
		var skins = _context.GameItemSkins.Include(x => x.EditableItem).AsNoTracking().ToArray();
		var values = _context.CharacteristicValues.AsNoTracking().ToArray();
		var definitions = _context.CharacteristicDefinitions.AsNoTracking().ToArray();
		var profiles = wearProfiles.ToArray();
		var skinReferences = OutfitSkinReferences();
		var names = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
		foreach (var outfit in document.Outfits.Where(x => x.EraAdmissions.Any(selected.Contains)))
		{
			try
			{
				if (!names.Add(outfit.Name)) throw new InvalidOperationException($"Another clothing outfit already uses the name {outfit.Name} in the selected batch.");
				var manifest = new OutfitManifestSpec(outfit.StableReference, outfit.Name, outfit.Description, _clothingOutfitEntries[outfit.StableReference]);
				var existing = ResolveOutfitManifestIdentity(manifest, templates, FindManagedRecord("outfit", outfit.StableReference)?.LogicalId);
				ValidateOutfitManifestOwnership(manifest, existing, BuildOutfitManifestDefinition(manifest, values, definitions),
					existing is null ? null : BuildLiveOutfitManifestDefinition(manifest.StableKey, existing, skinReferences, profiles, values, definitions));
				if (existing is not null) ValidatePreservedClothingOutfitGraph(existing, outfit.Source, plannedComponents,
					plannedItemIds, boundProfiles, geometry, colours, maximumLayerWeight, itemRows, componentRows,
					wearProfiles, progs, skins);
			}
			catch (InvalidOperationException ex)
			{
				throw outfit.Source.Error(ex.Message);
			}
		}
	}
}
