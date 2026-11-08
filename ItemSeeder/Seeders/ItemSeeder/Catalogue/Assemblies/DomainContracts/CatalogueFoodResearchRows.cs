#nullable enable

namespace DatabaseSeeder.Seeders;

public sealed record CatalogueFoodIngredientsResearchRow(
	string SourceFile, int SourceLine,
	string Profile, string Order, string Role, string Category, string Description, string SourceReference, string Quantity, string Unit, string Allergens);

public sealed record CatalogueFoodNutritionProfilesResearchRow(
	string SourceFile, int SourceLine,
	string Profile, string SatiationHours, string ThirstHours, string WaterMillilitres, string AlcoholMillilitres, string Basis, string ReviewState);

public sealed record CatalogueFoodDietaryMetadataResearchRow(
	string SourceFile, int SourceLine,
	string Profile, string Contents, string AnimalFeedPurposes, string ReviewState);

public sealed record CatalogueFoodFreshnessProfilesResearchRow(
	string SourceFile, int SourceLine,
	string Profile, string StaleDuration, string SpoilDuration, string StaleTarget, string SpoiledTarget, string ReviewState);

public sealed record CatalogueFoodExceptionalSkinsResearchRow(
	string SourceFile, int SourceLine,
	string StableReference, string BaseReference, string Rationale, string ReviewState);

public sealed record CatalogueFoodProductionRoutesResearchRow(
	string SourceFile, int SourceLine,
	string StableReference, string Kind, string Mechanisation, string ReviewState);

public sealed record CatalogueFoodCraftInputsResearchRow(
	string SourceFile, int SourceLine,
	string Route, string Order, string Reference, string Quantity, string Unit);

public sealed record CatalogueFoodCraftToolsResearchRow(
	string SourceFile, int SourceLine,
	string Route, string Order, string Tag, string Use);

public sealed record CatalogueFoodCraftPhasesResearchRow(
	string SourceFile, int SourceLine,
	string Route, string Order, string DurationSeconds, string Description);

public sealed record CatalogueFoodCraftProductsResearchRow(
	string SourceFile, int SourceLine,
	string Route, string Order, string Reference, string Quantity, string Unit, string FailureFamily);

public sealed record CatalogueFoodFailureFamiliesResearchRow(
	string SourceFile, int SourceLine,
	string StableReference, string Description, string Hazard, string ReviewState);

public sealed record CatalogueFoodEvidenceCandidatesResearchRow(
	string SourceFile, int SourceLine,
	string StableReference, string EvidenceClass, string Source, string ComparableFamily, string ReviewState);
