#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using MudSharp.Framework.Revision;
using MudSharp.FutureProg;
using MudSharp.GameItems;
using MudSharp.Models;

namespace DatabaseSeeder.Seeders;

public partial class ItemSeeder
{
	private sealed record ValidatedClothingOutfitEntry(OutfitTemplateItem Row,
		GameItemProto Item, GameItemComponentProto[] Components,
		IReadOnlySet<string> Capabilities, ClothingWearableBinding Wearable,
		ClothingWearProfileBinding? Profile);

	private void ValidatePreservedClothingOutfitGraph(OutfitTemplate outfit, ClothingSourceLocation source,
		IReadOnlyDictionary<string, GameItemComponentProto[]> plannedComponents,
		IReadOnlyDictionary<string, long?> plannedItemIds,
		IReadOnlyDictionary<long, ClothingWearProfileBinding> boundProfiles,
		IndustrialisedClothingWearProfiles geometry, IndustrialisedClothingColourBindings colours,
		double maximumLayerWeight, IReadOnlyCollection<GameItemProto> itemRows,
		IReadOnlyCollection<GameItemComponentProto> componentRows,
		IReadOnlyCollection<WearProfile> wearProfiles, IReadOnlyCollection<FutureProg> progs,
		IReadOnlyCollection<GameItemSkin> skins)
	{
		var componentsByRevision = componentRows.ToLookup(x => (x.Id, x.RevisionNumber));
		var progIds = progs.Select(x => x.Id).ToArray();
		var stableSkinReferences = OutfitSkinReferences();
		var plannedKeyById = plannedItemIds.Where(x => x.Value.HasValue)
			.GroupBy(x => x.Value!.Value).ToDictionary(x => x.Key, x => x.Select(y => y.Key).ToArray());
		var seenKeys = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
		var entries = new List<ValidatedClothingOutfitEntry>();
		foreach (var row in outfit.OutfitTemplateItems.OrderBy(x => x.WearOrder).ThenBy(x => x.Id))
		{
			if (string.IsNullOrWhiteSpace(row.TemplateKey) || row.TemplateKey.Any(x => !char.IsLetterOrDigit(x) && x is not '_' and not '-') ||
				!seenKeys.Add(row.TemplateKey))
				throw source.Error($"Preserved outfit has an invalid or duplicate entry key {row.TemplateKey}.");
			var current = itemRows.Where(x => x.Id == row.GameItemProtoId &&
				x.EditableItem?.RevisionStatus == (int)RevisionStatus.Current).ToArray();
			if (current.Length != 1)
				throw source.Error($"Preserved outfit entry {row.TemplateKey} requires exactly one current item prototype; found {current.Length}.");
			var item = current[0];
			GameItemComponentProto[] components;
			if (plannedKeyById.TryGetValue(item.Id, out var keys))
			{
				if (keys.Length != 1) throw source.Error($"Preserved outfit entry {row.TemplateKey} has ambiguous clothing identity for item {item.Id}.");
				components = plannedComponents[keys[0]];
			}
			else
			{
				var seenComponents = new HashSet<long>();
				components = item.GameItemProtosGameItemComponentProtos.Select(link =>
				{
					if (link.GameItemProtoId != item.Id || link.GameItemProtoRevision != item.RevisionNumber || !seenComponents.Add(link.GameItemComponentProtoId))
						throw source.Error($"Preserved outfit entry {row.TemplateKey} has an invalid or duplicate component link.");
					var matches = componentsByRevision[(link.GameItemComponentProtoId, link.GameItemComponentRevision)].ToArray();
					return matches.Length == 1 ? matches[0] : throw source.Error($"Preserved outfit entry {row.TemplateKey} has a missing component revision.");
				}).ToArray();
			}

			var wearable = IndustrialisedClothingPhysicalBindings.Bind(components, wearProfiles, progIds, source);
			var capabilities = components.Select(component =>
			{
				if (!IndustrialisedComponentMetadataCatalogue.Document.Types.TryGetValue(component.Type, out var metadata))
					throw source.Error($"Preserved outfit entry {row.TemplateKey} has unknown component type {component.Type}.");
				if (metadata.PreventsManualLoad)
					throw source.Error($"Preserved outfit entry {row.TemplateKey} contains component {component.Name}, which prevents manual loading.");
				return metadata;
			}).SelectMany(x => x.Capabilities).ToHashSet(StringComparer.Ordinal);
			ClothingWearProfileBinding? profile = null;
			if (row.Placement == (int)OutfitTemplateItemPlacement.Worn)
			{
				if (row.WearProfileId is not { } profileId || !wearable.ProfileIds.Contains(profileId))
					throw source.Error($"Preserved outfit entry {row.TemplateKey} has a missing or unsupported wear profile.");
				if (!boundProfiles.TryGetValue(profileId, out profile)) profile = geometry.Bind(wearProfiles.Single(x => x.Id == profileId), source);
			}
			else if (row.WearProfileId is not null)
			{
				throw source.Error($"Preserved outfit entry {row.TemplateKey} has a wear profile but is not worn.");
			}

			IReadOnlyDictionary<string, ClothingBoundColour>? authored = null;
			if (row.SkinId is { } skinId)
			{
				var currentSkins = skins.Where(x => x.Id == skinId && x.EditableItem?.RevisionStatus == (int)RevisionStatus.Current).ToArray();
				if (currentSkins.Length != 1 || currentSkins[0].ItemProtoId != item.Id)
					throw source.Error($"Preserved outfit entry {row.TemplateKey} has a missing, ambiguous or wrong-base current skin.");
				var skin = currentSkins[0];
				if (skin.CanUseSkinProgId is not { } canUseId || !progs.Any(x => x.Id == canUseId && x.ReturnType == (long)ProgVariableTypes.Boolean))
					throw source.Error($"Preserved outfit entry {row.TemplateKey} skin has no resolvable boolean use-prog.");
				if (stableSkinReferences.TryGetValue(skinId, out var skinReference) && _clothingColourBindings.TryGetValue(skinReference, out var skinColours))
					authored = skinColours;
			}
			if (authored is null && plannedKeyById.TryGetValue(item.Id, out var plannedKeys) && plannedKeys.Length == 1)
				_clothingColourBindings.TryGetValue(plannedKeys[0], out authored);
			colours.ValidatePersistedLoadArguments(row.LoadArguments, components, source, authored);
			entries.Add(new(row, item, components, capabilities, wearable, profile));
		}

		foreach (var entry in entries)
		{
			if (!Enum.IsDefined(typeof(OutfitTemplateItemPlacement), entry.Row.Placement))
				throw source.Error($"Preserved outfit entry {entry.Row.TemplateKey} has unknown placement {entry.Row.Placement}.");
			var placement = (OutfitTemplateItemPlacement)entry.Row.Placement;
			var relationRequired = placement is OutfitTemplateItemPlacement.Container or OutfitTemplateItemPlacement.AttachedToBelt or OutfitTemplateItemPlacement.Sheathed;
			var target = string.IsNullOrWhiteSpace(entry.Row.ContainerKey)
				? null
				: entries.SingleOrDefault(x => x.Row.TemplateKey.Equals(entry.Row.ContainerKey, StringComparison.OrdinalIgnoreCase));
			if (relationRequired && (target is null || ReferenceEquals(target, entry)))
				throw source.Error($"Preserved outfit entry {entry.Row.TemplateKey} has a missing or self-referential target {entry.Row.ContainerKey}.");
			if (!relationRequired && !string.IsNullOrWhiteSpace(entry.Row.ContainerKey))
				throw source.Error($"Preserved outfit entry {entry.Row.TemplateKey} has an unexpected relation target.");
			if (placement == OutfitTemplateItemPlacement.Wielded && !entry.Capabilities.Contains("IWieldable"))
				throw source.Error($"Preserved outfit entry {entry.Row.TemplateKey} is not wieldable.");
			if (placement == OutfitTemplateItemPlacement.Container && !target!.Capabilities.Contains("IContainer"))
				throw source.Error($"Preserved outfit target {target.Row.TemplateKey} is not a container.");
			if (placement == OutfitTemplateItemPlacement.AttachedToBelt &&
				(!entry.Capabilities.Contains("IBeltable") || !target!.Capabilities.Contains("IBelt")))
				throw source.Error($"Preserved outfit entry {entry.Row.TemplateKey} or target {target!.Row.TemplateKey} has incompatible belt capabilities.");
			if (placement == OutfitTemplateItemPlacement.Sheathed &&
				(!entry.Capabilities.Contains("IWieldable") || !target!.Capabilities.Contains("ISheath")))
				throw source.Error($"Preserved outfit entry {entry.Row.TemplateKey} or target {target!.Row.TemplateKey} has incompatible sheath capabilities.");
		}
		foreach (var entry in entries.Where(x => x.Row.Placement == (int)OutfitTemplateItemPlacement.Container))
		{
			var seen = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
			for (var current = entry; current.Row.Placement == (int)OutfitTemplateItemPlacement.Container;)
			{
				if (!seen.Add(current.Row.TemplateKey)) throw source.Error($"Preserved outfit entry {entry.Row.TemplateKey} has a cyclic container graph.");
				current = entries.Single(x => x.Row.TemplateKey.Equals(current.Row.ContainerKey, StringComparison.OrdinalIgnoreCase));
			}
		}
		IndustrialisedClothingWearProfiles.ValidateMandatoryLayers(entries.Where(x => x.Profile is not null)
			.Select(x => new ClothingWornEntryBinding(source, x.Row.TemplateKey, x.Wearable, x.Profile!)), maximumLayerWeight);
	}
}
