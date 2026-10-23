#nullable enable

using System.Collections.Generic;
using MudSharp.RPG.Checks;

namespace DatabaseSeeder.Seeders;

// Non-executing dependency evidence retained alongside the fixed compiled recipes.
public sealed record CatalogueProductionDependency(string Contract, string? StableReference,
	string? SourceStatus, string SourceOwner, int SourcePhase)
{
	public string Import => Contract;
}
public sealed record CatalogueProductionPlan(int Phase, string Name, string Category, string Trait,
	int MinimumTraitValue, Difficulty Difficulty, string KnowledgeSubtype,
	IReadOnlyList<string> Inputs, IReadOnlyList<string> Tools, IReadOnlyList<string> Products,
	IReadOnlyList<CatalogueProductionDependency> Dependencies, IReadOnlyList<string> SourceOwnership);
public sealed record CatalogueFoodProductionPlan(FoodCatalogueScope Scope, string StableReference,
	FoodCatalogueKind Kind, FoodCatalogueFamily Family, string Trait, int MinimumTraitValue,
	Difficulty Difficulty, string KnowledgeSubtype, int Phase, string Category,
	IReadOnlyList<string> Inputs, IReadOnlyList<string> Tools, IReadOnlyList<string> Products,
	IReadOnlyList<CatalogueProductionDependency> Dependencies, IReadOnlyList<string> SourceOwnership);
public sealed record CataloguePrimaryProductionContract(string Name, string Category, string Trait,
	string KnowledgeSubtype, IReadOnlyList<string> Inputs, IReadOnlyList<string> Tools,
	IReadOnlyList<string> Products, IReadOnlyList<string> FailProducts);
public sealed record CatalogueToolProvider(string Tag, string ProviderStableReference, string RequiredTagPath);
