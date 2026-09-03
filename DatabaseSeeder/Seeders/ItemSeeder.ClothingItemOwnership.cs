#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using MudSharp.Framework.Revision;
using MudSharp.Models;

namespace DatabaseSeeder.Seeders;

public partial class ItemSeeder
{
	private readonly Dictionary<string, ItemManifestDefinition> _clothingPhysicalDefinitions = new(StringComparer.Ordinal);

	private bool IsRenamedClothingPhysicalItem(GameItemProto item, string stableReference) =>
		_clothingPhysicalDefinitions.ContainsKey(stableReference) &&
		!string.Equals(item.UniqueName, stableReference, StringComparison.OrdinalIgnoreCase);

	private ItemManifestDefinition BuildClothingPhysicalDefinition(IndustrialisedItemCatalogueRow row,
		ResolvedIndustrialisedTechnologyProfile profile)
	{
		var tags = BuildReworkItemTagList(row.Tags);
		var components = EnsureBeltCapacityComponent(row.Noun, row.ShortDescription, tags, ResolveIndustrialisedComponents(row, profile));
		return BuildItemManifestDefinition(row.StableReference, row.Noun, row.ShortDescription, null,
			row.FullDescription, (int)row.Size, (int)row.Quality, row.WeightGrams, row.CostIndex,
			IndustrialisedCatalogue.Clothing.Skins.Any(x => x.BaseItemReference == row.StableReference), false,
			row.Material, tags, components, row.MorphTo, row.MorphEmote,
			row.MorphSeconds > 0 ? TimeSpan.FromSeconds(row.MorphSeconds) : null, row.DestroyedItem);
	}

	private GameItemProto? ResolveClothingPhysicalItem(ItemManifestDefinition expected, ClothingSourceLocation source)
	{
		if (_manifestCaptureOnly) return null;
		// InitialiseDependencies loads every revision and its children. Use those exact tracked
		// targets without assigning aliases, adopting ownership or changing any persistence state.
		var rows = _context!.GameItemProtos.Local.ToArray(); // Local excludes Deleted entries.
		var named = rows.Where(x => string.Equals(x.UniqueName, expected.StableReference, StringComparison.OrdinalIgnoreCase)).ToArray();
		var namedIds = named.Select(x => x.Id).Distinct().ToArray();
		var managed = FindManagedRecord("item", expected.StableReference);
		if (namedIds.Length > 1 || managed?.LogicalId is { } owned && namedIds.Any(x => x != owned))
			throw source.Error($"Clothing item ownership conflict for {expected.StableReference}: the stable name and provenance do not identify one logical item.");
		var id = managed?.LogicalId ?? namedIds.Cast<long?>().SingleOrDefault();
		if (id is null) return null;
		var revisions = rows.Where(x => x.Id == id).ToArray();
		if (revisions.Length == 0) return null; // The ordinary writer may restore genuinely missing owned stock.
		var current = revisions.Where(x => x.EditableItem?.RevisionStatus == (int)RevisionStatus.Current).ToArray();
		if (current.Length != 1)
			throw source.Error($"Clothing item {expected.StableReference} requires exactly one current revision; found {current.Length}.");
		var item = current[0];
		var others = _managedRecordsByIdentity.Values.Where(x => x.EntityType.Equals("item", StringComparison.OrdinalIgnoreCase) &&
			x.LogicalId == id && !x.StableKey.Equals(expected.StableReference, StringComparison.OrdinalIgnoreCase)).ToArray();
		if (others.Length > 0)
			throw source.Error($"Clothing item {expected.StableReference} is claimed by another aggregate: {string.Join(", ", others.Select(x => x.StableKey))}.");
		if (managed is null && !ItemSeederManifestCatalogue.Fingerprint(BuildLiveItemManifestDefinition(item, expected.StableReference))
			.Equals(ItemSeederManifestCatalogue.Fingerprint(expected), StringComparison.OrdinalIgnoreCase))
			throw source.Error($"Unmanaged clothing item conflict for {expected.StableReference}; the complete stock signature does not match.");
		return item;
	}
}
