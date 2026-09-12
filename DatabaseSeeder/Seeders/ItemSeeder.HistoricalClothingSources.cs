#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using MudSharp.Database;
using MudSharp.GameItems;
using MudSharp.Models;

namespace DatabaseSeeder.Seeders;

public partial class ItemSeeder
{
	internal sealed record HistoricalClothingItemSpec(
		string StableReference, string Noun, string ShortDescription, string? LongDescription, string FullDescription,
		SizeCategory Size, ItemQuality Quality, double WeightInGrams, decimal Cost, bool Skinnable, bool HiddenFromPlayers,
		string Material, IReadOnlyCollection<string> Tags, IReadOnlyCollection<string> Components,
		string? MorphTo, string? MorphEmote, TimeSpan? MorphTimer, string? DestroyedItem, string? BuilderNotes = null)
	{
		public string? LegacyAliasReference { get; init; }
		public bool AllowLegacyShortDescriptionMatch { get; init; } = true;
		public string OwningModule { get; init; } = "medieval";
		public IReadOnlyList<string> HistoricalEraAdmissions { get; init; } = Array.AsReadOnly(new[] { "medieval" });
	}

	private static readonly Lazy<IReadOnlyDictionary<string, HistoricalClothingItemSpec>> HistoricalClothingSources = new(BuildHistoricalClothingSources);

	private static IReadOnlyDictionary<string, HistoricalClothingItemSpec> BuildHistoricalClothingSources()
	{
		var sources = HistoricalClothingPrimaryItems.Select(x => x.LegacyAliasReference is null ? x : x with
		{
			OwningModule = "shared-preindustrial",
			HistoricalEraAdmissions = Array.AsReadOnly(new[] { "antiquity", "medieval", "renaissance", "earlymodern" })
		}).ToDictionary(x => x.StableReference, StringComparer.Ordinal);
		// The same first-definition order is used by the normal historical installer.
		foreach (var spec in AntiquityOutfitSupplementalItemSpecs.Concat(RenaissanceClothingItemSpecs).Concat(EarlyModernOutfitReferencedItemSpecs))
		{
			if (!ApprovedHistoricalClothingReuseReferences.Contains(spec.StableReference)) continue;
			sources.TryAdd(spec.StableReference, new(spec.StableReference, spec.Noun, spec.ShortDescription, null,
				spec.FullDescription, spec.Size, spec.Quality, spec.WeightInGrams, spec.Cost, spec.Skinnable, false,
				spec.Material, spec.Tags, spec.Components, null, null, null, null, spec.BuilderNotes)
			{
				AllowLegacyShortDescriptionMatch = false,
				OwningModule = "outfits",
				HistoricalEraAdmissions = Array.AsReadOnly(new[] { "antiquity", "medieval", "renaissance", "earlymodern" })
			});
		}
		if (!sources.Keys.ToHashSet(StringComparer.Ordinal).SetEquals(ApprovedHistoricalClothingReuseReferences))
			throw new InvalidOperationException("Approved historical clothing reuse references do not all have authoritative definitions.");
		return sources.AsReadOnly();
	}

	internal static HistoricalClothingItemSpec? FindHistoricalClothingSource(string reference) =>
		HistoricalClothingSources.Value.GetValueOrDefault(reference);

	internal static IReadOnlyList<HistoricalClothingItemSpec> ApprovedHistoricalClothingSourcesForAudit() =>
		Array.AsReadOnly(HistoricalClothingSources.Value.Values.OrderBy(x => x.StableReference, StringComparer.Ordinal).ToArray());

	// This locates the first-definition provider, not the original literal of every supplied item.
	internal static ClothingSourceLocation HistoricalClothingSourceProviderLocation { get; } = HistoricalProviderLocation();
	private static ClothingSourceLocation HistoricalProviderLocation([CallerLineNumber] int line = 0) =>
		new("DatabaseSeeder/Seeders/ItemSeeder.HistoricalClothingSources.cs", line);

	private ItemManifestDefinition BuildHistoricalClothingManifestDefinition(HistoricalClothingItemSpec spec)
	{
		var tags = BuildReworkItemTagList(spec.LegacyAliasReference is null ? spec.Tags : BuildPreIndustrialAliasTags(spec.Tags));
		var components = EnsureBeltCapacityComponent(spec.Noun, spec.ShortDescription, tags, spec.Components);
		return BuildItemManifestDefinition(spec.StableReference, spec.Noun, spec.ShortDescription, spec.LongDescription,
			spec.FullDescription, (int)spec.Size, (int)spec.Quality, spec.WeightInGrams, spec.Cost, spec.Skinnable,
			spec.HiddenFromPlayers, spec.Material, tags, components, spec.MorphTo, spec.MorphEmote, spec.MorphTimer, spec.DestroyedItem);
	}

	private GameItemProto? ValidateHistoricalClothingReuseOwnership(HistoricalClothingItemSpec spec,
		ItemManifestDefinition expected, ClothingSourceLocation source)
	{
		var existing = FindItemByStableReference(spec.StableReference);
		if (existing is null && spec.LegacyAliasReference is null && spec.AllowLegacyShortDescriptionMatch)
		{
			try
			{
				existing = FindExactLegacyItemMatch(spec.StableReference, spec.ShortDescription, expected);
			}
			catch (InvalidOperationException ex)
			{
				throw source.Error(ex.Message);
			}
		}
		if (existing is null) return null;
		var managed = FindManagedRecord("item", spec.StableReference);
		if (managed?.LogicalId is { } logicalId && logicalId != existing.Id)
			throw source.Error($"Historical clothing ownership conflict for {spec.StableReference}: provenance names ID {logicalId}, but the item resolves to {existing.Id}.");
		if (managed is null && !ItemSeederManifestCatalogue.Fingerprint(BuildLiveItemManifestDefinition(existing, spec.StableReference))
			.Equals(ItemSeederManifestCatalogue.Fingerprint(expected), StringComparison.OrdinalIgnoreCase))
			throw source.Error($"Unmanaged historical clothing conflict for {spec.StableReference}; its complete stock signature does not match.");
		// Do not adopt, mark customized, refresh provenance or change the entity during preflight.
		return existing;
	}

	private GameItemProto? CreateHistoricalClothingItem(HistoricalClothingItemSpec spec)
	{
		if (spec.LegacyAliasReference is { } legacy)
			return CreatePreIndustrialAlias(legacy, spec.StableReference, spec.Noun, spec.ShortDescription,
				spec.LongDescription, spec.FullDescription, spec.Size, spec.Quality, spec.WeightInGrams, spec.Cost,
				spec.Skinnable, spec.HiddenFromPlayers, spec.Material, spec.Tags, spec.Components,
				spec.MorphTo, spec.MorphEmote, spec.MorphTimer, spec.DestroyedItem, spec.BuilderNotes);
		return CreateItem(spec.StableReference, spec.Noun, spec.ShortDescription, spec.LongDescription, spec.FullDescription,
			spec.Size, spec.Quality, spec.WeightInGrams, spec.Cost, spec.Skinnable, spec.HiddenFromPlayers, spec.Material,
			spec.Tags, spec.Components, spec.MorphTo, spec.MorphEmote, spec.MorphTimer, spec.DestroyedItem,
			spec.BuilderNotes, spec.AllowLegacyShortDescriptionMatch);
	}

	private void SeedIndustrialisedClothingReuse(string eras)
	{
		if (IndustrialisedCatalogue.Clothing.Bases.Count == 0) return;
		if (!_clothingPreflightComplete) throw new InvalidOperationException("Clothing reuse cannot be applied before complete successful preflight.");
		var selected = ParseEraTokens(eras).ToHashSet(StringComparer.OrdinalIgnoreCase);
		foreach (var row in IndustrialisedCatalogue.Clothing.Bases
			.Where(x => _manifestCaptureOnly || x.EraAdmissions.Any(selected.Contains)))
		{
			if (!_clothingReusePlans.TryGetValue(row.ItemReference, out var spec))
			{
				if (FindHistoricalClothingSource(row.ItemReference) is not null)
					throw row.Source.Error("Historical clothing reuse requires a resolved preflight plan before persistence.");
				continue;
			}
			if (IsManifestAggregateRegistered("item", row.ItemReference)) continue;
			using var module = UseManifestModule(spec.OwningModule,
				spec.HistoricalEraAdmissions.Concat(row.EraAdmissions).Distinct(StringComparer.Ordinal).ToArray());
			CreateHistoricalClothingItem(spec);
		}
	}

	internal void ValidateClothingPrerequisitesForTesting(FuturemudDatabaseContext context, string eras)
	{
		_context = context;
		_questionAnswers = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase) { ["eras"] = eras, ["technologyprofile"] = "neutral" };
		InitialiseDependencies();
		ValidateIndustrialisedClothingPrerequisites(eras, ResolveIndustrialisedTechnologyProfile());
	}

	internal void ApplyClothingReuseForTesting(string eras) => SeedIndustrialisedClothingReuse(eras);
	internal IReadOnlyCollection<string> ResolveSelectedErasForTesting(FuturemudDatabaseContext context,
		IReadOnlyDictionary<string, string> answers) => ResolveSelectedEras(context, answers);
}
