#nullable enable

using MudSharp.GameItems;
using System.Collections.Generic;

namespace DatabaseSeeder.Seeders;

public enum CatalogueReadiness { ProductionReady, DependencyReviewed, DesignOnly }

public sealed record CatalogueMetadata(
	string StableKey,
	string Module,
	IReadOnlyList<string> EraAdmissions,
	IReadOnlyList<string> Dependencies,
	ItemSeederOwnershipPolicy OwnershipPolicy,
	CatalogueReadiness Readiness,
	string Source,
	IReadOnlyList<string> Evidence);

public sealed record CatalogueItemDefinition
{
	public required CatalogueMetadata Metadata { get; init; }
	public required string Noun { get; init; }
	public required string ShortDescription { get; init; }
	public string? LongDescription { get; init; }
	public required string FullDescription { get; init; }
	public SizeCategory Size { get; init; }
	public ItemQuality Quality { get; init; }
	public double WeightInGrams { get; init; }
	public decimal Cost { get; init; }
	public bool Skinnable { get; init; }
	public bool HiddenFromPlayers { get; init; }
	public required string Material { get; init; }
	public required IReadOnlyList<string> Tags { get; init; }
	public required IReadOnlyList<string> Components { get; init; }
	public string? MorphToStableReference { get; init; }
	public string? MorphEmote { get; init; }
	public int? MorphSeconds { get; init; }
	public string? DestroyedItemStableReference { get; init; }
	public string? BuilderNotes { get; init; }
	public bool AllowLegacyShortDescriptionMatch { get; init; }
	public IReadOnlyList<string> ProfileBindings { get; init; } = [];
	public CatalogueIndustrialItemDetails? Industrial { get; init; }
	public string? MedicalCategory { get; init; }
	public CatalogueHistoricalItemDetails? Historical { get; init; }
}

public sealed record CatalogueHistoricalItemDetails(string? Category, string? Admission, string? Family);

public sealed record CatalogueIndustrialItemDetails(
	int SourceLine, string Domain, IReadOnlyList<string> SupportedClaims, string SourceNote,
	bool Craftable, string? LifecycleKind);

public sealed record CataloguePriceEvidenceDefinition(
	string Source, int Line, string EvidenceId, string EraBand, string Locale, int StartYear, int EndYear,
	string Currency, decimal? NominalPrice, string QuotedUnit, decimal? DailyWage, decimal? LabourDays,
	decimal? CostIndex, string SourceClass, string ComparableFamily, string Confidence,
	string SourceUrl, string SourcePage, string Notes);
