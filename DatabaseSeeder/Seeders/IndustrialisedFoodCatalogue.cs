#nullable enable

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace DatabaseSeeder.Seeders;

internal enum IndustrialisedFoodPhysicalKind
{
	Item,
	Liquid
}

internal enum IndustrialisedFoodReviewState
{
	Proposed,
	DependencyReviewed,
	ContentReviewed,
	ProductionReady
}

internal sealed record IndustrialisedFoodConceptRow(
	string Source, int Line, string StableReference, IndustrialisedFoodPhysicalKind PhysicalKind,
	string Family, IReadOnlyList<string> Admissions, string Concept, string Ownership,
	string Material, string LiquidDependency, IReadOnlyList<string> Components, IReadOnlyList<string> Tags,
	string IngredientProfile, string NutritionProfile, string DietaryProfile, string FreshnessProfile,
	IReadOnlyList<string> HealthBindings, string ProductionRoute, string FailureFamily, string EvidenceRoute,
	string Rationale, IndustrialisedFoodReviewState ReviewState);

internal sealed record IndustrialisedFoodDependencyRow(
	string Source, int Line, string StableReference, string ConsumerReference, string ProductionRole,
	IndustrialisedFoodReviewState ReviewState);

internal sealed record IndustrialisedFoodServingRow(
	string Source, int Line, string StableReference, IReadOnlyList<string> Admissions, string Context,
	string Name, IndustrialisedFoodReviewState ReviewState);

internal sealed record IndustrialisedFoodServingEntryRow(
	string Source, int Line, string ServingReference, int Order, string Kind, string StableReference,
	double Quantity, string Unit);

internal sealed record IndustrialisedFoodCatalogueDocument(
	IReadOnlyList<IndustrialisedFoodConceptRow> Concepts,
	IReadOnlyList<IndustrialisedFoodDependencyRow> AdoptedDependencies,
	IReadOnlyList<IndustrialisedFoodServingRow> Servings,
	IReadOnlyList<IndustrialisedFoodServingEntryRow> ServingEntries,
	IReadOnlyDictionary<string, IReadOnlyList<string[]>> SupportingSources)
{
	internal int ItemCount => Concepts.Count(x => x.PhysicalKind == IndustrialisedFoodPhysicalKind.Item);
	internal int LiquidCount => Concepts.Count(x => x.PhysicalKind == IndustrialisedFoodPhysicalKind.Liquid);

	internal void EnsureProductionReadyForSeeding()
	{
		var incomplete = Concepts.Where(x => x.ReviewState != IndustrialisedFoodReviewState.ProductionReady).ToArray();
		if (incomplete.Length > 0)
		{
			throw new InvalidDataException($"Food catalogue installation is blocked: {incomplete.Length:N0} concepts are not ProductionReady; first at {incomplete[0].Source}:{incomplete[0].Line}.");
		}
	}
}

internal static class IndustrialisedFoodCatalogue
{
	private static readonly string[] ConceptHeaders =
	[
		"StableReference", "PhysicalKind", "Family", "Admissions", "Concept", "Ownership",
		"Material", "LiquidDependency", "Components", "Tags",
		"IngredientProfile", "NutritionProfile", "DietaryProfile", "FreshnessProfile", "HealthBindings",
		"ProductionRoute", "FailureFamily", "EvidenceRoute", "Rationale", "ReviewState"
	];
	private static readonly string[] AdoptedHeaders =
	[
		"StableReference", "ConsumerReference", "ProductionRole", "ReviewState"
	];
	private static readonly string[] ServingHeaders =
	[
		"StableReference", "Admissions", "Context", "Name", "ReviewState"
	];
	private static readonly string[] ServingEntryHeaders =
	[
		"ServingReference", "Order", "Kind", "StableReference", "Quantity", "Unit"
	];
	private static readonly IReadOnlyDictionary<string, string[]> SupportingHeaders = new Dictionary<string, string[]>(StringComparer.Ordinal)
	{
		["ingredients.tsv"] = ["Profile", "Order", "Role", "Category", "Description", "SourceReference", "Quantity", "Unit", "Allergens"],
		["nutrition-profiles.tsv"] = ["Profile", "SatiationHours", "ThirstHours", "WaterMillilitres", "AlcoholMillilitres", "Basis", "ReviewState"],
		["dietary-metadata.tsv"] = ["Profile", "Contents", "AnimalFeedPurposes", "ReviewState"],
		["freshness-profiles.tsv"] = ["Profile", "StaleDuration", "SpoilDuration", "StaleTarget", "SpoiledTarget", "ReviewState"],
		["exceptional-skins.tsv"] = ["StableReference", "BaseReference", "Rationale", "ReviewState"],
		["production-routes.tsv"] = ["StableReference", "Kind", "Mechanisation", "ReviewState"],
		["craft-inputs.tsv"] = ["Route", "Order", "Reference", "Quantity", "Unit"],
		["craft-tools.tsv"] = ["Route", "Order", "Tag", "Use"],
		["craft-phases.tsv"] = ["Route", "Order", "DurationSeconds", "Description"],
		["craft-products.tsv"] = ["Route", "Order", "Reference", "Quantity", "Unit", "FailureFamily"],
		["failure-families.tsv"] = ["StableReference", "Description", "Hazard", "ReviewState"],
		["evidence-candidates.tsv"] = ["StableReference", "EvidenceClass", "Source", "ComparableFamily", "ReviewState"]
	};

	internal static IndustrialisedFoodCatalogueDocument Load(IEnumerable<IndustrialisedCatalogueSource> input)
	{
		var sources = input.OrderBy(x => x.Name, StringComparer.Ordinal).ToArray();
		var required = SupportingHeaders.Keys.Concat(["concepts.tsv", "adopted-dependencies.tsv", "servings.tsv", "serving-entries.tsv"]).ToArray();
		foreach (var name in required)
		{
			if (sources.Count(x => x.Name == name) != 1) throw new InvalidDataException($"Food catalogue requires {name} exactly once.");
		}
		var unexpected = sources.Where(x => !required.Contains(x.Name, StringComparer.Ordinal)).ToArray();
		if (unexpected.Length > 0) throw new InvalidDataException($"Unrecognised food catalogue source {unexpected[0].Name}.");

		var concepts = sources.Single(x => x.Name == "concepts.tsv").Read(ConceptHeaders, ParseConcept).ToArray();
		var adopted = sources.Single(x => x.Name == "adopted-dependencies.tsv").Read(AdoptedHeaders,
			(s, l, x) => new IndustrialisedFoodDependencyRow(s, l, x[0], x[1], x[2], Review(x[3]))).ToArray();
		var servings = sources.Single(x => x.Name == "servings.tsv").Read(ServingHeaders,
			(s, l, x) => new IndustrialisedFoodServingRow(s, l, x[0], IndustrialisedCatalogueValues.List(x[1]), x[2], x[3], Review(x[4]))).ToArray();
		var entries = sources.Single(x => x.Name == "serving-entries.tsv").Read(ServingEntryHeaders,
			(s, l, x) => new IndustrialisedFoodServingEntryRow(s, l, x[0], IndustrialisedCatalogueValues.Int(x[1]), x[2], x[3], IndustrialisedCatalogueValues.Double(x[4]), x[5])).ToArray();
		var supporting = SupportingHeaders.ToDictionary(x => x.Key,
			x => (IReadOnlyList<string[]>)sources.Single(y => y.Name == x.Key).Read(x.Value, (_, _, row) => row).ToArray(), StringComparer.Ordinal);
		Validate(concepts, adopted, servings, entries, supporting);
		return new(Array.AsReadOnly(concepts), Array.AsReadOnly(adopted), Array.AsReadOnly(servings), Array.AsReadOnly(entries), supporting);
	}

	private static IndustrialisedFoodConceptRow ParseConcept(string source, int line, string[] x) => new(
		source, line, x[0], EnumValue<IndustrialisedFoodPhysicalKind>(x[1]), x[2], IndustrialisedCatalogueValues.List(x[3]),
		x[4], x[5], x[6], x[7], IndustrialisedCatalogueValues.List(x[8]), IndustrialisedCatalogueValues.List(x[9]),
		x[10], x[11], x[12], x[13], IndustrialisedCatalogueValues.List(x[14]), x[15], x[16], x[17], x[18], Review(x[19]));

	private static T EnumValue<T>(string text) where T : struct, Enum => IndustrialisedCatalogueValues.EnumValue<T>(text);
	private static IndustrialisedFoodReviewState Review(string text) => EnumValue<IndustrialisedFoodReviewState>(text);

	private static void Validate(IndustrialisedFoodConceptRow[] concepts, IndustrialisedFoodDependencyRow[] adopted,
		IndustrialisedFoodServingRow[] servings, IndustrialisedFoodServingEntryRow[] entries,
		IReadOnlyDictionary<string, IReadOnlyList<string[]>> supporting)
	{
		if (concepts.Length != 464) throw new InvalidDataException($"Food Gate 2 requires exactly 464 concepts; found {concepts.Length:N0}.");
		if (adopted.Length != 307) throw new InvalidDataException($"Food Gate 2 requires exactly 307 adopted dependencies; found {adopted.Length:N0}.");
		if (servings.Length != 26) throw new InvalidDataException($"Food Gate 2 requires exactly 26 serving manifests; found {servings.Length:N0}.");
		Unique(concepts, x => x.StableReference, "concept");
		Unique(adopted, x => x.StableReference, "adopted dependency");
		Unique(servings, x => x.StableReference, "serving manifest");
		if (concepts.Any(x => x.ReviewState < IndustrialisedFoodReviewState.DependencyReviewed) || adopted.Any(x => x.ReviewState < IndustrialisedFoodReviewState.DependencyReviewed))
			throw new InvalidDataException("Every Gate 2 concept and adopted dependency must be DependencyReviewed.");

		var conceptIds = concepts.Select(x => x.StableReference).ToHashSet(StringComparer.Ordinal);
		var ingredientProfiles = supporting["ingredients.tsv"].Select(x => x[0]).ToHashSet(StringComparer.Ordinal);
		var nutritionProfiles = supporting["nutrition-profiles.tsv"].Select(x => x[0]).ToHashSet(StringComparer.Ordinal);
		var dietaryProfiles = supporting["dietary-metadata.tsv"].Select(x => x[0]).ToHashSet(StringComparer.Ordinal);
		var freshnessProfiles = supporting["freshness-profiles.tsv"].Select(x => x[0]).ToHashSet(StringComparer.Ordinal);
		var routes = supporting["production-routes.tsv"].Select(x => x[0]).ToHashSet(StringComparer.Ordinal);
		var failures = supporting["failure-families.tsv"].Select(x => x[0]).ToHashSet(StringComparer.Ordinal);
		var evidence = supporting["evidence-candidates.tsv"].Select(x => x[0]).ToHashSet(StringComparer.Ordinal);
		var componentTypes = IndustrialisedComponentMetadataCatalogue.Document.Types;
		var validAdmissions = new HashSet<string>(["industrial", "modern", "nuclear", "information"], StringComparer.Ordinal);
		var validHealthBindings = new HashSet<string>(["Dietary Caffeine", "Food-Borne Illness"], StringComparer.Ordinal);
		foreach (var row in concepts)
		{
			if (row.Admissions.Count == 0 || row.Admissions.Any(x => !validAdmissions.Contains(x)) ||
				row.HealthBindings.Any(x => !validHealthBindings.Contains(x)) ||
				!ingredientProfiles.Contains(row.IngredientProfile) || !nutritionProfiles.Contains(row.NutritionProfile) ||
				!dietaryProfiles.Contains(row.DietaryProfile) || (row.FreshnessProfile.Length > 0 && !freshnessProfiles.Contains(row.FreshnessProfile)) ||
				!routes.Contains(row.ProductionRoute) || !failures.Contains(row.FailureFamily) || !evidence.Contains(row.EvidenceRoute))
				throw new InvalidDataException($"{row.Source}:{row.Line}: unresolved concept dependency graph for {row.StableReference}.");
			if (row.PhysicalKind == IndustrialisedFoodPhysicalKind.Item && (row.Material.Length == 0 || row.Components.Count == 0))
				throw new InvalidDataException($"{row.Source}:{row.Line}: item concepts require material and component dependencies.");
			if (row.PhysicalKind == IndustrialisedFoodPhysicalKind.Liquid && row.LiquidDependency != $"same-graph:{row.StableReference}")
				throw new InvalidDataException($"{row.Source}:{row.Line}: liquid concept must resolve its own same-graph liquid identity.");
			foreach (var component in row.Components)
				if (!componentTypes.ContainsKey(component)) throw new InvalidDataException($"{row.Source}:{row.Line}: unknown component type {component}.");
			var ingredientRows = supporting["ingredients.tsv"].Where(x => x[0] == row.IngredientProfile).ToArray();
			if (ingredientRows.Length == 0 || ingredientRows.Any(x => x[7] != (row.PhysicalKind == IndustrialisedFoodPhysicalKind.Item ? "g" : "ml")))
				throw new InvalidDataException($"{row.Source}:{row.Line}: ingredient unit is incompatible with physical kind.");
		}
		foreach (var freshness in supporting["freshness-profiles.tsv"])
		{
			if (freshness[3] == freshness[4]) throw new InvalidDataException($"Freshness profile {freshness[0]} must use distinct result liquids.");
			foreach (var target in new[] { freshness[3], freshness[4] })
			{
				var targetRow = concepts.SingleOrDefault(x => x.StableReference == target);
				if (targetRow is null || targetRow.PhysicalKind != IndustrialisedFoodPhysicalKind.Liquid || targetRow.FreshnessProfile.Length > 0)
					throw new InvalidDataException($"Freshness profile {freshness[0]} target {target} must be a terminal liquid concept.");
			}
		}
		foreach (var row in adopted)
			if (!conceptIds.Contains(row.ConsumerReference)) throw new InvalidDataException($"{row.Source}:{row.Line}: adopted dependency has no exact consumer.");
		foreach (var source in new[] { "production-routes.tsv", "failure-families.tsv", "evidence-candidates.tsv", "nutrition-profiles.tsv", "dietary-metadata.tsv", "freshness-profiles.tsv" })
		{
			var duplicate = supporting[source]
				.GroupBy(x => x[0], StringComparer.Ordinal)
				.FirstOrDefault(x => x.Count() > 1);
			if (duplicate is not null) throw new InvalidDataException($"Duplicate {source} identity {duplicate.Key}.");
		}
		foreach (var product in supporting["craft-products.tsv"])
		{
			if (!routes.Contains(product[0]) || !conceptIds.Contains(product[2]) || !failures.Contains(product[5]))
				throw new InvalidDataException($"Craft product has unresolved route, product, or failure reference: {product[0]}.");
		}
		foreach (var input in supporting["craft-inputs.tsv"])
			if (!routes.Contains(input[0]) || !ingredientProfiles.Contains(input[2]))
				throw new InvalidDataException($"Craft input has unresolved route or ingredient reference: {input[0]}.");
		foreach (var tool in supporting["craft-tools.tsv"])
			if (!routes.Contains(tool[0])) throw new InvalidDataException($"Craft tool has unresolved route: {tool[0]}.");
		foreach (var phase in supporting["craft-phases.tsv"])
			if (!routes.Contains(phase[0])) throw new InvalidDataException($"Craft phase has unresolved route: {phase[0]}.");
		var servingIds = servings.Select(x => x.StableReference).ToHashSet(StringComparer.Ordinal);
		foreach (var entry in entries)
		{
			var concept = concepts.SingleOrDefault(x => x.StableReference == entry.StableReference);
			if (!servingIds.Contains(entry.ServingReference) || concept is null || entry.Quantity <= 0 ||
				(entry.Kind == "item" && (entry.Unit != "g" || concept.PhysicalKind != IndustrialisedFoodPhysicalKind.Item)) ||
				(entry.Kind == "liquid" && (entry.Unit != "ml" || concept.PhysicalKind != IndustrialisedFoodPhysicalKind.Liquid)))
				throw new InvalidDataException($"{entry.Source}:{entry.Line}: invalid serving entry or unit compatibility.");
			var serving = servings.Single(x => x.StableReference == entry.ServingReference);
			if (!entry.StableReference.StartsWith("industrialised_", StringComparison.Ordinal) &&
				!concept.Admissions.Intersect(serving.Admissions, StringComparer.Ordinal).Any())
				throw new InvalidDataException($"{entry.Source}:{entry.Line}: serving and concept admissions are incompatible.");
		}
		if (servings.Any(x => !entries.Any(y => y.ServingReference == x.StableReference))) throw new InvalidDataException("Every serving requires at least one ordered entry.");
	}

	private static void Unique<T>(IEnumerable<T> rows, Func<T, string> key, string label)
	{
		var duplicate = rows.GroupBy(key, StringComparer.Ordinal).FirstOrDefault(x => x.Count() > 1);
		if (duplicate is not null) throw new InvalidDataException($"Duplicate {label} identity {duplicate.Key}.");
	}
}
