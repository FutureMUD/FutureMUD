#nullable enable

using MudSharp.GameItems;

namespace DatabaseSeeder.Seeders;

public sealed record CatalogueSkinDefinition(
	string StableReference,
	string BaseItemStableReference,
	string ItemName,
	string ShortDescription,
	string FullDescription,
	ItemQuality? Quality);
