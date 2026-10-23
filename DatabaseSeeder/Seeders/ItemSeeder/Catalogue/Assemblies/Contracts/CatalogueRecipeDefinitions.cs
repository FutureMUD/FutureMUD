#nullable enable

using MudSharp.Body;
using MudSharp.GameItems.Inventory.Plans;
using MudSharp.RPG.Checks;
using MudSharp.RPG.Knowledge;
using System.Collections.Generic;

namespace DatabaseSeeder.Seeders;

// Compatibility prose is retained solely for the existing ownership fingerprint projection.
// Persistence resolves the typed references below; it never parses this text.
public sealed record CatalogueImportProjection(string Details, IReadOnlyList<string> Options);

public abstract record CatalogueRecipeInput
{
	public CatalogueImportProjection? Compatibility { get; init; }
	public double QualityWeight { get; init; } = 1;
}

public enum CatalogueCharacteristicMode { Any, None, Specific }
public sealed record CatalogueCharacteristicRequirement(string Definition, string? Value);
public sealed record CatalogueCommodityRecipeInput(double Grams, string Material, string? PileTag,
	CatalogueCharacteristicMode CharacteristicMode, IReadOnlyList<CatalogueCharacteristicRequirement> Characteristics) : CatalogueRecipeInput;
public sealed record CatalogueCommodityTagRecipeInput(double Grams, string MaterialTag, string? PileTag,
	CatalogueCharacteristicMode CharacteristicMode, IReadOnlyList<CatalogueCharacteristicRequirement> Characteristics) : CatalogueRecipeInput;
public sealed record CatalogueTagRecipeInput(string Tag, int Quantity) : CatalogueRecipeInput;
public sealed record CatalogueItemRecipeInput(string ItemStableReference, int Quantity) : CatalogueRecipeInput;
public sealed record CatalogueLiquidRecipeInput(string Liquid, double Litres) : CatalogueRecipeInput;
public sealed record CatalogueLiquidTagRecipeInput(string LiquidTag, double Litres) : CatalogueRecipeInput;

public abstract record CatalogueRecipeTool(DesiredItemState State)
{
	public CatalogueImportProjection? Compatibility { get; init; }
	public double QualityWeight { get; init; } = 1;
	public bool UseToolDuration { get; init; } = true;
}
public sealed record CatalogueTagRecipeTool(string Tag, DesiredItemState State) : CatalogueRecipeTool(State);
public sealed record CatalogueItemRecipeTool(string ItemStableReference, DesiredItemState State) : CatalogueRecipeTool(State);

public abstract record CatalogueRecipeProduct
{
	public CatalogueImportProjection? Compatibility { get; init; }
	public bool IsFailProduct { get; init; }
	public int? MaterialDefiningInputIndex { get; init; }
}
public sealed record CatalogueIngredientSlot(int InputIndex, string Role);
public sealed record CatalogueVariableInput(string Definition, int InputIndex);
public sealed record CatalogueCommodityCharacteristic(string Definition, string? Value, int? InputIndex);
public sealed record CatalogueItemRecipeProduct(string ItemStableReference, int Quantity, string? SkinStableReference) : CatalogueRecipeProduct;
public sealed record CatalogueCookedRecipeProduct(string ItemStableReference, int Quantity, string? SkinStableReference,
	bool RemoveDrugsAndFoodEffects, IReadOnlyList<CatalogueIngredientSlot> Ingredients) : CatalogueRecipeProduct;
public sealed record CatalogueProgCookedRecipeProduct(string Prog, bool RemoveDrugsAndFoodEffects,
	IReadOnlyList<CatalogueIngredientSlot> Ingredients) : CatalogueRecipeProduct;
public sealed record CatalogueLiquidRecipeProduct(string ItemStableReference, int Quantity, string? SkinStableReference,
	string Liquid, double Litres) : CatalogueRecipeProduct;
public sealed record CatalogueVariableRecipeProduct(string ItemStableReference, int Quantity, string? SkinStableReference,
	IReadOnlyList<CatalogueVariableInput> Variables) : CatalogueRecipeProduct;
public sealed record CatalogueCommodityRecipeProduct(double Grams, string Material, string? Tag,
	IReadOnlyList<CatalogueCommodityCharacteristic> Characteristics) : CatalogueRecipeProduct;
public sealed record CatalogueProgRecipeProduct(string Prog) : CatalogueRecipeProduct;
public sealed record CatalogueScrapRecipeProduct(int InputIndex, double FractionRecovered, string? Tag) : CatalogueRecipeProduct;
public sealed record CatalogueUnusedRecipeProduct(int InputIndex, double FractionRecovered) : CatalogueRecipeProduct;

public abstract record CatalogueRecipeGate(string Trait, int? MinimumTraitValue);
public sealed record CatalogueTraitRecipeGate(string Trait, int? MinimumTraitValue) : CatalogueRecipeGate(Trait, MinimumTraitValue);
public sealed record CatalogueKnowledgeRecipeGate(string Trait, int? MinimumTraitValue, string Knowledge,
	string Type, string Subtype, string? Description, string? LongDescription, int LearningSessionsRequired,
	Difficulty LearnDifficulty, Difficulty TeachDifficulty, LearnableType Learnable) : CatalogueRecipeGate(Trait, MinimumTraitValue);

public sealed record CatalogueRecipePhase(int Seconds, string Echo, string FailEcho, ExertionLevel Exertion, double Stamina);
public sealed record CatalogueRecipeDefinition
{
	public required CatalogueMetadata Metadata { get; init; }
	public required IReadOnlyList<string> RequiredAnyEra { get; init; }
	public bool IndustrialCaptureOnly { get; init; }
	public bool UseExplicitStableReference { get; init; }
	public bool PreserveAuthoredText { get; init; }
	// Version 1 is the original stock formula, unit weightings and practical-check policy.
	public int QualityPolicyVersion { get; init; } = 1;
	public required string Name { get; init; }
	public required string Category { get; init; }
	public required string Blurb { get; init; }
	public required string Action { get; init; }
	public required string ActiveCraftItemSdesc { get; init; }
	public CatalogueRecipeGate? Gate { get; init; }
	public string? Trait { get; init; }
	// Explicit configured-skill alternatives, before the captured/default trait.
	// Shared knowledge-only programs do not determine a recipe's check skill.
	public IReadOnlyList<string> CheckTraitChoices { get; init; } = [];
	public string? AppearProg { get; init; }
	public string? CanUseProg { get; init; }
	public string? WhyCannotUseProg { get; init; }
	public string? OnStartProg { get; init; }
	public string? OnFinishProg { get; init; }
	public string? OnCancelProg { get; init; }
	public Difficulty Difficulty { get; init; }
	public Outcome Threshold { get; init; }
	public int FreeChecks { get; init; }
	public int FailPhase { get; init; }
	public bool Interruptable { get; init; }
	public required IReadOnlyList<CatalogueRecipePhase> Phases { get; init; }
	public required IReadOnlyList<CatalogueRecipeInput> Inputs { get; init; }
	public required IReadOnlyList<CatalogueRecipeTool> Tools { get; init; }
	public required IReadOnlyList<CatalogueRecipeProduct> Products { get; init; }
	public required IReadOnlyList<CatalogueRecipeProduct> FailProducts { get; init; }
}
