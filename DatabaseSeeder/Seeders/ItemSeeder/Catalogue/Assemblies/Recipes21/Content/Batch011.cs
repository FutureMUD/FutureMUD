#nullable enable

using MudSharp.Body;
using MudSharp.GameItems.Inventory.Plans;
using MudSharp.RPG.Checks;

namespace DatabaseSeeder.Seeders;

public static partial class Recipes21Stock
{
	public static CatalogueRecipeDefinition ConstructionAndUtilitiesMakeHeavyDutyOakTileForDomesticBuildingWorkCraftIndustrialisedConstructionUtilities0033HeavyDutyTile() => new()
	{
		Metadata = new("Construction and utilities/make heavy-duty oak tile for domestic building work [craft_industrialised_construction_utilities_0033_heavy_duty_tile]", "crafts", ["industrial", "information", "modern", "nuclear"], ["item:industrialised_construction_utilities_0033_heavy_duty_tile"], ItemSeederOwnershipPolicy.StockAggregate, CatalogueReadiness.ProductionReady, "DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedCatalogue.cs:279", []),
		RequiredAnyEra = ["industrial"],
		IndustrialCaptureOnly = false,
		UseExplicitStableReference = false,
		PreserveAuthoredText = false,
		QualityPolicyVersion = 1,
		Name = "make heavy-duty oak tile for domestic building work [craft_industrialised_construction_utilities_0033_heavy_duty_tile]",
		Category = "Construction and utilities",
		Blurb = "Make a heavy-duty oak tile for domestic building work from prepared oak stock.",
		Action = "making heavy-duty oak tile for domestic building work",
		ActiveCraftItemSdesc = "an in-progress heavy-duty oak tile for domestic building work craft",
		Gate = CatalogueRecipeGates.TraitLabouring0,
		Trait = null,
		AppearProg = null,
		CanUseProg = null,
		WhyCannotUseProg = null,
		OnStartProg = null,
		OnFinishProg = null,
		OnCancelProg = null,
		Difficulty = Difficulty.Normal,
		Threshold = Outcome.MinorFail,
		FreeChecks = 5,
		FailPhase = 2,
		Interruptable = false,
		Phases = [
			new(20, "$0 measure|measures and prepare|prepares $i1 for the work.", "$0 measure|measures and prepare|prepares $i1 for the work.", ExertionLevel.Stasis, 0),
			new(30, "$0 finish|finishes the assembly and set|sets aside $p1.", "$0 discover|discovers the work cannot be completed and salvage|salvages what remains.", ExertionLevel.Stasis, 0),
		],
		Inputs = [
			new CatalogueCommodityRecipeInput(Grams: 2842.0, Material: "oak", PileTag: null, CharacteristicMode: CatalogueCharacteristicMode.Any, Characteristics: []) { Compatibility = new("2842 grams of oak", []), QualityWeight = 1 },
		],
		Tools = [
		],
		Products = [
			new CatalogueItemRecipeProduct(ItemStableReference: "industrialised_construction_utilities_0033_heavy_duty_tile", Quantity: 1, SkinStableReference: null) { Compatibility = new("1x a heavy-duty oak tile for domestic building work (@industrialised_construction_utilities_0033_heavy_duty_tile)", []), IsFailProduct = false, MaterialDefiningInputIndex = null },
		],
		FailProducts = [
		],
	};
}
