#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using MudSharp.Framework.Revision;
using MudSharp.Models;

namespace DatabaseSeeder.Seeders;

public partial class ItemSeeder
{
	private static ItemSkinManifestDefinition DocumentedClothingSkinDefinition(DocumentedClothingSkinSpec spec,
		string alwaysTrueName) => new(spec.StableReference, spec.BaseItemStableReference, spec.ItemName,
		spec.ShortDescription, spec.FullDescription, null, (int?)spec.Quality, false, alwaysTrueName);

	/// <summary>
	/// Resolves stock ownership without adoption, fingerprint updates or item mutations. Both
	/// whole-catalogue preflight and the existing skin writer use this same target/signature check.
	/// </summary>
	private GameItemSkin? ValidateDocumentedClothingSkinOwnership(DocumentedClothingSkinSpec spec,
		long? baseItemId, IReadOnlyCollection<GameItemSkin> rows, string alwaysTrueName)
	{
		if (_manifestCaptureOnly) return null;
		var managed = FindManagedRecord("item-skin", spec.StableReference);
		var allNamed = rows.Where(x => x.Name.Equals(spec.StableReference, StringComparison.OrdinalIgnoreCase)).ToArray();
		var named = allNamed.Where(x => x.EditableItem?.RevisionStatus == (int)RevisionStatus.Current).ToArray();
		if (named.Length > 1)
			throw new InvalidOperationException($"Missing or ambiguous current skin {spec.StableReference}.");
		var existing = named.SingleOrDefault();
		if (managed?.LogicalId is { } logicalId)
		{
			var owned = rows.Where(x => x.Id == logicalId).ToArray();
			var current = owned.Where(x => x.EditableItem?.RevisionStatus == (int)RevisionStatus.Current).ToArray();
			if (current.Length > 1)
				throw new InvalidOperationException($"Missing or ambiguous current skin {spec.StableReference} on owned ID {logicalId}.");
			if (owned.Length > 0 && current.Length == 0)
				throw new InvalidOperationException($"Owned skin {spec.StableReference} has no current revision; resolve its revision state before installing clothing.");
			if (existing is not null && existing.Id != logicalId)
				throw new InvalidOperationException($"ItemSeeder ownership conflict for item-skin:{spec.StableReference}: provenance names ID {logicalId}, but the name resolves to {existing.Id}.");
			// A builder rename is a customization, not permission to create a replacement identity.
			existing = current.SingleOrDefault() ?? existing;
		}
		if (existing is null)
		{
			if (allNamed.Length > 0)
				throw new InvalidOperationException($"Skin {spec.StableReference} has no current revision; resolve its revision state before installing clothing.");
			return null;
		}

		var otherOwners = _managedRecordsByIdentity.Values.Where(x =>
			x.EntityType.Equals("item-skin", StringComparison.OrdinalIgnoreCase) && x.LogicalId == existing.Id &&
			!x.StableKey.Equals(spec.StableReference, StringComparison.OrdinalIgnoreCase)).ToArray();
		if (otherOwners.Length > 0)
			throw new InvalidOperationException($"ItemSeeder ownership conflict for item-skin:{spec.StableReference}: ID {existing.Id} is claimed by {string.Join(", ", otherOwners.Select(x => x.StableKey).OrderBy(x => x, StringComparer.Ordinal))}.");

		var liveFingerprint = ItemSeederManifestCatalogue.Fingerprint(BuildLiveDocumentedClothingSkinDefinition(existing));
		if (managed is null && !liveFingerprint.Equals(
			ItemSeederManifestCatalogue.Fingerprint(DocumentedClothingSkinDefinition(spec, alwaysTrueName)), StringComparison.OrdinalIgnoreCase))
			throw new InvalidOperationException($"Unmanaged item-skin conflict for '{spec.StableReference}'. The complete stock signature does not match; it will not be claimed or overwritten.");

		if (managed is not null && !liveFingerprint.Equals(managed.AppliedFingerprint, StringComparison.OrdinalIgnoreCase) &&
			(baseItemId is null || existing.ItemProtoId != baseItemId))
			throw new InvalidOperationException($"Customised skin {spec.StableReference} targets a different base from {spec.BaseItemStableReference}; it cannot be preserved and used by this clothing graph.");
		return existing;
	}

	private void ValidateIndustrialisedClothingSkinOwnership(IndustrialisedClothingCatalogueDocument document,
		IReadOnlySet<string> selected, IReadOnlyDictionary<string, long?> existingBaseIds)
	{
		var rows = _context!.GameItemSkins.Include(x => x.EditableItem).AsNoTracking().ToArray();
		foreach (var skin in document.Skins.Where(x => _manifestCaptureOnly || x.EraAdmissions.Any(selected.Contains)))
		{
			try
			{
				ValidateDocumentedClothingSkinOwnership(new(skin.StableReference, skin.BaseItemReference, skin.Noun,
					skin.ShortDescription, skin.FullDescription, skin.QualityOverride), existingBaseIds[skin.BaseItemReference], rows, "AlwaysTrue");
			}
			catch (InvalidOperationException ex)
			{
				throw skin.Source.Error(ex.Message);
			}
		}
	}
}
