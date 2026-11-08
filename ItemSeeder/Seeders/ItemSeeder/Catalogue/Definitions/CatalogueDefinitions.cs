#nullable enable

using MudSharp.Body;
using MudSharp.GameItems;
using MudSharp.GameItems.Inventory.Plans;
using MudSharp.Models;
using MudSharp.RPG.Checks;
using System;
using System.Collections.Generic;

namespace DatabaseSeeder.Seeders;

internal sealed record CatalogueCommodityInput(double Grams, string Material, string? PileTag, double QualityWeight);
internal sealed record CatalogueTagTool(string Tag, DesiredItemState State, double QualityWeight, bool UseToolDuration);
internal sealed record CatalogueSimpleProduct(string ItemStableReference, int Quantity, bool IsFailProduct, int? MaterialDefiningInputIndex);

internal sealed record CatalogueCraftDefinition
{
	public required CatalogueMetadata Metadata { get; init; }
	// During stage 2 preserve the old target lookup as well as its category/name-derived stable identity.
	public bool UseExplicitStableReference { get; init; }
	public bool PreserveAuthoredText { get; init; }
	public required string Name { get; init; }
	public required string Category { get; init; }
	public required string Blurb { get; init; }
	public required string Action { get; init; }
	public required string ActiveCraftItemSdesc { get; init; }
	public required string Knowledge { get; init; }
	public required string KnowledgeSubtype { get; init; }
	public required string Trait { get; init; }
	public int? MinimumTraitValue { get; init; }
	public string? OnStartProg { get; init; }
	public string? OnFinishProg { get; init; }
	public string? OnCancelProg { get; init; }
	public Difficulty Difficulty { get; init; }
	public Outcome Threshold { get; init; }
	public int FreeChecks { get; init; }
	public int FailPhase { get; init; }
	public bool Interruptable { get; init; }
	public required IReadOnlyList<ItemSeeder.CraftPhaseSpec> Phases { get; init; }
	public required IReadOnlyList<CatalogueCommodityInput> Inputs { get; init; }
	public required IReadOnlyList<CatalogueTagTool> Tools { get; init; }
	public required IReadOnlyList<CatalogueSimpleProduct> Products { get; init; }
	public required IReadOnlyList<CatalogueSimpleProduct> FailProducts { get; init; }
}
