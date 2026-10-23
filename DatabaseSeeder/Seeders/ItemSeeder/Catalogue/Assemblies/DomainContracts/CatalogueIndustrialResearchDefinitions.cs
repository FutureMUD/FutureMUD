#nullable enable

using System.Collections.Generic;

namespace DatabaseSeeder.Seeders;

public sealed record IndustrialisedCraftCatalogueRow(
	string Source,
	int Line,
	string StableKey,
	IReadOnlyList<string> EraAdmissions,
	string Category,
	string Trait,
	int MinimumTraitValue,
	string Difficulty,
	string ProductStableReference,
	string InputMaterial,
	double InputGrams);

public sealed record IndustrialisedOutfitCatalogueRow(
	string Source,
	int Line,
	string OutfitReference,
	string Name,
	string Description,
	IReadOnlyList<string> EraAdmissions,
	IReadOnlyList<string> ItemStableReferences);

public sealed record IndustrialisedTechnologyBindingRow(
	string Source,
	int Line,
	string Profile,
	string Dimension,
	string Family,
	IReadOnlyList<string> Values,
	bool ComponentBacked);

public enum IndustrialisedFoodPhysicalKind
{
	Item,
	Liquid
}

public enum IndustrialisedFoodReviewState
{
	Proposed,
	DependencyReviewed,
	ContentReviewed,
	ProductionReady
}

public sealed record IndustrialisedFoodConceptRow(
	string Source, int Line, string StableReference, IndustrialisedFoodPhysicalKind PhysicalKind,
	string Family, IReadOnlyList<string> Admissions, string Concept, string Ownership,
	string Material, string LiquidDependency, IReadOnlyList<string> Components, IReadOnlyList<string> Tags,
	string IngredientProfile, string NutritionProfile, string DietaryProfile, string FreshnessProfile,
	IReadOnlyList<string> HealthBindings, string ProductionRoute, string FailureFamily, string EvidenceRoute,
	string Rationale, IndustrialisedFoodReviewState ReviewState);

public sealed record IndustrialisedFoodDependencyRow(
	string Source, int Line, string StableReference, string ConsumerReference, string ProductionRole,
	IndustrialisedFoodReviewState ReviewState);

public sealed record IndustrialisedFoodServingRow(
	string Source, int Line, string StableReference, IReadOnlyList<string> Admissions, string Context,
	string Name, IndustrialisedFoodReviewState ReviewState);

public sealed record IndustrialisedFoodServingEntryRow(
	string Source, int Line, string ServingReference, int Order, string Kind, string StableReference,
	double Quantity, string Unit);

