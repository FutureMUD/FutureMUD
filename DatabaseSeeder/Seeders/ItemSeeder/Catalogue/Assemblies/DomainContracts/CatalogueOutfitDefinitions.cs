#nullable enable

using MudSharp.GameItems;
using System.Collections.Generic;

namespace DatabaseSeeder.Seeders;

public sealed record CatalogueOutfitItem(
	string ItemStableReference, string? SkinStableReference, string? EntryKey, string? WearProfile,
	OutfitTemplateItemPlacement Placement, string? ContainerKey, string LoadArguments);

public sealed record CatalogueOutfitDefinition(
	CatalogueMetadata Metadata, string Name, string Description, IReadOnlyList<CatalogueOutfitItem> Items);

